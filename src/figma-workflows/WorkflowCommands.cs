namespace Loupedeck.FigmaWorkflowsPlugin;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

internal static class FigmaWorkflowRunner
{
    private static readonly String ScriptPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        "Library/Application Support/Logi/LogiPluginService/Plugins/FigmaWorkflows/bin/figma-workflow.applescript");
    private const String LogPath = "/private/tmp/figma-workflows-plugin.log";

    public static void SelectTool(String key) => Run("key", key);

    public static void SelectShape(String shapeName) => Run("shape", shapeName);

    public static void WriteDiagnostic(String message)
    {
        try
        {
            File.AppendAllText(LogPath, $"{DateTime.Now:O} {message}{Environment.NewLine}");
        }
        catch
        {
            // Diagnostics must never prevent another keypad action from running.
        }
    }

    private static void Run(params String[] arguments)
    {
        WriteDiagnostic($"Invoking: {String.Join(" | ", arguments)}");
        try
        {
            var startInfo = new ProcessStartInfo("/usr/bin/osascript")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            startInfo.ArgumentList.Add(ScriptPath);
            foreach (var argument in arguments)
            {
                startInfo.ArgumentList.Add(argument);
            }

            using var process = Process.Start(startInfo);
            if (process is null)
            {
                WriteDiagnostic("Workflow failed: osascript did not start.");
                return;
            }

            if (!process.WaitForExit(5000))
            {
                WriteDiagnostic($"Workflow timed out (PID {process.Id}).");
                return;
            }

            var standardOutput = process.StandardOutput.ReadToEnd().Trim();
            var standardError = process.StandardError.ReadToEnd().Trim();
            WriteDiagnostic($"Completed with exit code {process.ExitCode}; stdout='{standardOutput}'; stderr='{standardError}'");
        }
        catch (Exception exception)
        {
            WriteDiagnostic($"Workflow failed: {exception}");
        }
    }
}

public abstract class FigmaCommand : PluginDynamicCommand
{
    private readonly Action _run;
    private readonly Byte[] _image;

    protected FigmaCommand(String name, String description, String iconName, Action run)
        : base(name, description, "FigJam Tools")
    {
        this._run = run;
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .Single(resource => resource.EndsWith($"{iconName}.png", StringComparison.OrdinalIgnoreCase));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        this._image = memory.ToArray();
    }

    protected override void RunCommand(String actionParameter)
    {
        FigmaWorkflowRunner.WriteDiagnostic($"Button pressed: {this.GetType().Name}");
        this._run();
    }

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
        BitmapImage.FromArray(this._image);

    protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => null;
}

public sealed class SquareCommand : FigmaCommand
{
    public SquareCommand() : base("Square", "Put a square on the FigJam cursor", "square", () => FigmaWorkflowRunner.SelectTool("r")) { }
}

public sealed class EllipseCommand : FigmaCommand
{
    public EllipseCommand() : base("Ellipse", "Put an ellipse on the FigJam cursor", "ellipse", () => FigmaWorkflowRunner.SelectTool("o")) { }
}

public sealed class DiamondCommand : FigmaCommand
{
    public DiamondCommand() : base("Diamond", "Put a diamond on the FigJam cursor", "diamond", () => FigmaWorkflowRunner.SelectShape("diamond")) { }
}

public sealed class TriangleCommand : FigmaCommand
{
    public TriangleCommand() : base("Triangle", "Put a triangle on the FigJam cursor", "triangle", () => FigmaWorkflowRunner.SelectShape("triangle")) { }
}

public sealed class StarCommand : FigmaCommand
{
    public StarCommand() : base("Star", "Put a star on the FigJam cursor", "star", () => FigmaWorkflowRunner.SelectShape("star")) { }
}

public sealed class ConnectorCommand : FigmaCommand
{
    public ConnectorCommand() : base("Connector", "Select the FigJam connector tool", "connector", () => FigmaWorkflowRunner.SelectTool("c")) { }
}

public sealed class StickyCommand : FigmaCommand
{
    public StickyCommand() : base("Sticky", "Put a sticky note on the FigJam cursor", "sticky", () => FigmaWorkflowRunner.SelectTool("s")) { }
}

public sealed class TextCommand : FigmaCommand
{
    public TextCommand() : base("Text", "Put text on the FigJam cursor", "text", () => FigmaWorkflowRunner.SelectTool("t")) { }
}
