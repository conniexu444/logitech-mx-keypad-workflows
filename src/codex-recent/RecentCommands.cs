namespace Loupedeck.CodexRecentPlugin;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

public abstract class RecentCommand : PluginDynamicCommand
{
    private const String LogPath = "/private/tmp/codex-recent-plugin.log";
    private static readonly String DatabasePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".codex",
        "state_5.sqlite");
    private readonly Int32 _offset;
    private readonly Byte[] _image;

    protected RecentCommand(Int32 index)
        : base($"Recent {index}", $"Open Codex task {index} in the current recency order", "Codex Recent")
    {
        this._offset = index - 1;
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .Single(name => name.EndsWith($"recent{index}.png", StringComparison.OrdinalIgnoreCase));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        this._image = memory.ToArray();
    }

    protected override void RunCommand(String actionParameter)
    {
        try
        {
            var taskId = QueryTaskId(this._offset);
            if (String.IsNullOrWhiteSpace(taskId))
            {
                WriteDiagnostic($"No task found for recency offset {this._offset}.");
                return;
            }

            WriteDiagnostic($"Opening recent task {this._offset + 1}: {taskId}");
            Start("/usr/bin/open", $"codex://threads/{taskId}");
        }
        catch (Exception exception)
        {
            WriteDiagnostic($"Action failed: {exception}");
        }
    }

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
        BitmapImage.FromArray(this._image);

    protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => null;

    private static String QueryTaskId(Int32 offset)
    {
        var query = $@"SELECT id
FROM threads
WHERE archived = 0 AND preview <> '' AND thread_source = 'user'
ORDER BY recency_at_ms DESC, id DESC
LIMIT 1 OFFSET {offset};";
        var process = Start("/usr/bin/sqlite3", DatabasePath, query);
        return process?.StandardOutput.ReadToEnd().Trim() ?? String.Empty;
    }

    private static Process Start(String executable, params String[] arguments)
    {
        var startInfo = new ProcessStartInfo(executable)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = executable.EndsWith("sqlite3", StringComparison.Ordinal),
            RedirectStandardError = false,
        };
        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }
        return Process.Start(startInfo);
    }

    private static void WriteDiagnostic(String message)
    {
        try
        {
            File.AppendAllText(LogPath, $"{DateTime.Now:O} {message}{Environment.NewLine}");
        }
        catch
        {
            // Diagnostics must never prevent the keypad action from running.
        }
    }
}

public sealed class Recent1Command : RecentCommand
{
    public Recent1Command() : base(1) { }
}

public sealed class Recent2Command : RecentCommand
{
    public Recent2Command() : base(2) { }
}

public sealed class Recent3Command : RecentCommand
{
    public Recent3Command() : base(3) { }
}
