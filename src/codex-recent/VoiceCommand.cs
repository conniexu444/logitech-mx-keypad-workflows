namespace Loupedeck.CodexRecentPlugin;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

internal static class CodexVoiceRunner
{
    private const String WhisperPath = "/opt/homebrew/bin/whisper-cli";
    private const String LogPath = "/private/tmp/codex-keypad-voice.log";
    private const String SessionRoot = "/private/tmp/codex-keypad-voice";
    private static readonly String UserHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private static readonly String PluginRoot = Path.Combine(
        UserHome,
        "Library/Application Support/Logi/LogiPluginService/Plugins/CodexRecent");
    private static readonly String RecorderAppPath = Path.Combine(PluginRoot, "bin/CodexVoiceRecorder.app");
    private static readonly String InjectScriptPath = Path.Combine(PluginRoot, "bin/codex-voice-inject.applescript");
    private static readonly String ModelPath = Environment.GetEnvironmentVariable("CODEX_KEYPAD_WHISPER_MODEL")
        ?? Path.Combine(UserHome, ".cache/whisper/ggml-base.en.bin");

    private static readonly Object Gate = new();
    private static String _sessionPath;

    public static void Toggle()
    {
        lock (Gate)
        {
            if (_sessionPath is not null)
            {
                var stoppedSessionPath = _sessionPath;
                _sessionPath = null;
                File.WriteAllText(Path.Combine(stoppedSessionPath, "stop"), "stop");
                WriteDiagnostic("Stopped recording; transcribing locally.");
                _ = Task.Run(() => CompleteRecording(stoppedSessionPath));
                return;
            }

            if (!Directory.Exists(RecorderAppPath) || !File.Exists(WhisperPath) || !File.Exists(ModelPath))
            {
                WriteDiagnostic("Voice runtime or local Whisper model is missing.");
                return;
            }

            Directory.CreateDirectory(SessionRoot);
            var sessionPath = Path.Combine(SessionRoot, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(sessionPath);
            var startInfo = new ProcessStartInfo("/usr/bin/open")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
            };
            startInfo.ArgumentList.Add("-n");
            startInfo.ArgumentList.Add("-a");
            startInfo.ArgumentList.Add(RecorderAppPath);
            startInfo.ArgumentList.Add("--args");
            startInfo.ArgumentList.Add(Path.Combine(sessionPath, "speech.wav"));
            startInfo.ArgumentList.Add(Path.Combine(sessionPath, "stop"));
            startInfo.ArgumentList.Add(Path.Combine(sessionPath, "status"));
            try
            {
                using var launch = Process.Start(startInfo);
                if (launch is null || !launch.WaitForExit(8_000) || launch.ExitCode != 0)
                {
                    WriteDiagnostic($"Could not launch the recorder app: {launch?.StandardError.ReadToEnd().Trim()}");
                    Directory.Delete(sessionPath, recursive: true);
                    return;
                }
                _sessionPath = sessionPath;
                WriteDiagnostic("Recorder app launched. Grant microphone access if prompted; press the mic again to stop and send.");
            }
            catch (Exception exception)
            {
                _sessionPath = null;
                WriteDiagnostic($"Could not start recorder: {exception}");
            }
        }
    }

    private static void CompleteRecording(String sessionPath)
    {
        try
        {
            var statusPath = Path.Combine(sessionPath, "status");
            var deadline = DateTime.UtcNow.AddSeconds(30);
            while (DateTime.UtcNow < deadline)
            {
                if (File.Exists(statusPath))
                {
                    var status = File.ReadAllText(statusPath).Trim();
                    if (status == "done")
                    {
                        break;
                    }
                    if (status.StartsWith("error:", StringComparison.Ordinal))
                    {
                        WriteDiagnostic(status);
                        return;
                    }
                }
                Thread.Sleep(100);
            }
            if (!File.Exists(statusPath) || File.ReadAllText(statusPath).Trim() != "done")
            {
                WriteDiagnostic("Recorder did not finish within 30 seconds.");
                return;
            }

            var transcriptPrefix = Path.Combine(sessionPath, "transcript");
            var whisper = new ProcessStartInfo(WhisperPath)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = false,
            };
            foreach (var argument in new[] { "-ng", "-m", ModelPath, "-f", Path.Combine(sessionPath, "speech.wav"), "-otxt", "-of", transcriptPrefix, "-np", "-nt" })
            {
                whisper.ArgumentList.Add(argument);
            }

            using var transcription = Process.Start(whisper);
            if (transcription is null || !transcription.WaitForExit(120_000) || transcription.ExitCode != 0)
            {
                WriteDiagnostic("Local Whisper transcription failed or timed out.");
                return;
            }

            var transcriptPath = transcriptPrefix + ".txt";
            if (!File.Exists(transcriptPath))
            {
                WriteDiagnostic("Whisper produced no transcript file.");
                return;
            }
            var prompt = File.ReadAllText(transcriptPath).Trim();
            if (String.IsNullOrWhiteSpace(prompt))
            {
                WriteDiagnostic("Whisper produced an empty transcript.");
                return;
            }

            var inject = new ProcessStartInfo("/usr/bin/osascript")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
            };
            inject.ArgumentList.Add(InjectScriptPath);
            inject.ArgumentList.Add(prompt);
            using var injection = Process.Start(inject);
            if (injection is null || !injection.WaitForExit(15_000) || injection.ExitCode != 0)
            {
                WriteDiagnostic("Could not send the transcript to the current Codex task.");
                return;
            }
            WriteDiagnostic("Sent transcribed speech to Codex.");
        }
        catch (Exception exception)
        {
            WriteDiagnostic($"Voice workflow failed: {exception}");
        }
        finally
        {
            try
            {
                Directory.Delete(sessionPath, recursive: true);
            }
            catch
            {
                // Temporary audio is removed when possible without hiding the primary error.
            }
        }
    }

    private static void WriteDiagnostic(String message)
    {
        try
        {
            File.AppendAllText(LogPath, $"{DateTime.Now:O} {message}{Environment.NewLine}");
        }
        catch
        {
            // Diagnostics must never block the keypad.
        }
    }
}

public sealed class VoiceCommand : WorkflowCommand
{
    public VoiceCommand() : base("Mic Voice", "Tap to record; tap again to transcribe and send to Codex", "voice", CodexVoiceRunner.Toggle) { }
}
