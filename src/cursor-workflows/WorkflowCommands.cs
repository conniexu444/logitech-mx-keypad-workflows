namespace Loupedeck.CursorWorkflowsPlugin;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

internal static class CursorWorkflowRunner
{
    private static readonly String ScriptPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        "Library/Application Support/Logi/LogiPluginService/Plugins/CursorWorkflows/bin/cursor-workflow.applescript");
    private const String LogPath = "/private/tmp/cursor-workflows-plugin.log";

    public static void RunMenuItem(String menuName, String itemName) =>
        Run("menu", menuName, itemName);

    public static void RunShortcut(String shortcut) => Run("shortcut", shortcut);

    public static void StartPrompt(String prompt) => Run("prompt", prompt);

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
}

public abstract class CursorCommand : PluginDynamicCommand
{
    private readonly Action _run;
    private readonly Byte[] _image;

    protected CursorCommand(String name, String description, String iconName, Action run)
        : base(name, description, "Cursor Workflows")
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
        CursorWorkflowRunner.WriteDiagnostic($"Button pressed: {this.GetType().Name}");
        this._run();
    }

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
        BitmapImage.FromArray(this._image);

    protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => null;
}

public sealed class NewAgentCommand : CursorCommand
{
    public NewAgentCommand() : base("New Agent", "Start a new Cursor Agent", "new_agent", () => CursorWorkflowRunner.RunShortcut("new_agent")) { }
}

public sealed class OpenFolderCommand : CursorCommand
{
    public OpenFolderCommand() : base("Open Folder", "Choose a project folder", "open_folder", () => CursorWorkflowRunner.RunShortcut("open_folder")) { }
}

public sealed class OpenIdeCommand : CursorCommand
{
    public OpenIdeCommand() : base("Open IDE", "Open Cursor's IDE view", "open_ide", () => CursorWorkflowRunner.RunShortcut("open_ide")) { }
}

public sealed class ChangesCommand : CursorCommand
{
    public ChangesCommand() : base("Changes", "Show the changes view", "changes", () => CursorWorkflowRunner.RunShortcut("changes")) { }
}

public sealed class BrowserCommand : CursorCommand
{
    public BrowserCommand() : base("Browser", "Show Cursor's browser", "explore", () => CursorWorkflowRunner.RunShortcut("browser")) { }
}

public sealed class FilesCommand : CursorCommand
{
    public FilesCommand() : base("Files", "Show project files", "document", () => CursorWorkflowRunner.RunShortcut("files")) { }
}

public sealed class NewTerminalCommand : CursorCommand
{
    public NewTerminalCommand() : base("New Terminal", "Create a new terminal", "terminal", () => CursorWorkflowRunner.RunMenuItem("File", "New Terminal")) { }
}

public sealed class TerminalCommand : CursorCommand
{
    public TerminalCommand() : base("Terminal", "Show or hide the terminal", "terminal", () => CursorWorkflowRunner.RunShortcut("terminal")) { }
}

public sealed class CommandPaletteCommand : CursorCommand
{
    public CommandPaletteCommand() : base("Command Palette", "Open Cursor's command palette", "command_palette", () => CursorWorkflowRunner.RunShortcut("command_palette")) { }
}

public sealed class VoiceNewAgentCommand : CursorCommand
{
    public VoiceNewAgentCommand() : base("New Agent", "Start a new Cursor Agent", "voice", () => CursorWorkflowRunner.RunShortcut("new_agent")) { }
}

public sealed class VoiceOpenFolderCommand : CursorCommand
{
    public VoiceOpenFolderCommand() : base("Open Folder", "Choose a project folder", "open_folder", () => CursorWorkflowRunner.RunShortcut("open_folder")) { }
}

public sealed class VoiceDraftCommand : CursorCommand
{
    public VoiceDraftCommand() : base("Command Palette", "Open Cursor's command palette", "voice_draft", () => CursorWorkflowRunner.RunShortcut("command_palette")) { }
}

public sealed class PreviousTabCommand : CursorCommand
{
    public PreviousTabCommand() : base("Previous Tab", "Move to the previous Cursor tab", "previous", () => CursorWorkflowRunner.RunShortcut("previous_tab")) { }
}

public sealed class NextTabCommand : CursorCommand
{
    public NextTabCommand() : base("Next Tab", "Move to the next Cursor tab", "next", () => CursorWorkflowRunner.RunShortcut("next_tab")) { }
}

public sealed class StatusChangesCommand : CursorCommand
{
    public StatusChangesCommand() : base("Changes", "Show the changes view", "changes", () => CursorWorkflowRunner.RunShortcut("changes")) { }
}

public sealed class ModelPaletteCommand : CursorCommand
{
    public ModelPaletteCommand() : base("Command Palette", "Open Cursor's command palette", "model", () => CursorWorkflowRunner.RunShortcut("command_palette")) { }
}

public sealed class CompactAgentCommand : CursorPromptCommand
{
    public CompactAgentCommand() : base("Compact Context", "Ask Cursor to compact the current context", "compact", "Summarize and compact the current context while preserving the important requirements, decisions, and next steps.") { }
}

public sealed class ArrowUpCommand : CursorCommand
{
    public ArrowUpCommand() : base("Up", "Move up in the active Cursor list", "up", () => CursorWorkflowRunner.RunShortcut("up")) { }
}

public sealed class EnterCommand : CursorCommand
{
    public EnterCommand() : base("Enter", "Confirm the active Cursor choice", "enter", () => CursorWorkflowRunner.RunShortcut("enter")) { }
}

public sealed class ArrowDownCommand : CursorCommand
{
    public ArrowDownCommand() : base("Down", "Move down in the active Cursor list", "down", () => CursorWorkflowRunner.RunShortcut("down")) { }
}

public sealed class ScrollDownCommand : CursorCommand
{
    public ScrollDownCommand() : base("Scroll Down", "Scroll the active Cursor view", "scroll_down", () => CursorWorkflowRunner.RunShortcut("scroll_down")) { }
}

public abstract class CursorPromptCommand : CursorCommand
{
    protected CursorPromptCommand(String name, String description, String iconName, String prompt)
        : base(name, description, iconName, () => CursorWorkflowRunner.StartPrompt(prompt)) { }
}

public sealed class ExploreCommand : CursorPromptCommand
{
    public ExploreCommand() : base("Explore", "Explore and summarize the project", "explore", "Explore this codebase and summarize its architecture, important entry points, and current risks. Do not change any files.") { }
}

public sealed class ExplainCommand : CursorPromptCommand
{
    public ExplainCommand() : base("Explain", "Explain the relevant code", "explain", "Explain the currently relevant code and how its main components interact. Do not change any files.") { }
}

public sealed class ReviewCodeCommand : CursorPromptCommand
{
    public ReviewCodeCommand() : base("Review Code", "Review the current changes", "review_code", "Review the current changes for correctness, regressions, security issues, and missing tests. Do not modify files.") { }
}

public sealed class OptimizeCommand : CursorPromptCommand
{
    public OptimizeCommand() : base("Optimize", "Find and implement safe optimizations", "optimize", "Identify the highest-impact performance improvements in this project, implement the safe improvements, and verify the result.") { }
}

public sealed class RefactorCommand : CursorPromptCommand
{
    public RefactorCommand() : base("Refactor", "Improve structure without changing behavior", "refactor", "Refactor the relevant code to improve clarity and maintainability without changing behavior. Run appropriate verification afterward.") { }
}

public sealed class WriteTestsCommand : CursorPromptCommand
{
    public WriteTestsCommand() : base("Write Tests", "Add meaningful test coverage", "write_tests", "Identify the most important missing tests, add meaningful coverage, and run the relevant test suite.") { }
}

public sealed class DocumentCommand : CursorPromptCommand
{
    public DocumentCommand() : base("Document", "Improve project documentation", "document", "Improve the documentation for the relevant code and workflows. Keep it concise, accurate, and aligned with the implementation.") { }
}

public sealed class FixBugCommand : CursorPromptCommand
{
    public FixBugCommand() : base("Fix Bug", "Diagnose and fix the current issue", "fix_bug", "Investigate the current bug or failing behavior, determine the root cause, implement a fix, and verify it.") { }
}

public sealed class SecurityCommand : CursorPromptCommand
{
    public SecurityCommand() : base("Security", "Audit the project for security", "security", "Audit the current changes and relevant code paths for security vulnerabilities, then fix and verify any concrete issues you find.") { }
}

public sealed class GitStatusCommand : CursorPromptCommand
{
    public GitStatusCommand() : base("Git Status", "Summarize the working tree", "git_status", "Inspect git status and summarize the current branch, staged changes, unstaged changes, and untracked files. Do not modify anything.") { }
}

public sealed class GitDiffCommand : CursorPromptCommand
{
    public GitDiffCommand() : base("Git Diff", "Explain the current diff", "git_diff", "Inspect the current git diff and summarize the intent, important changes, and possible problems. Do not modify files.") { }
}

public sealed class GitLogCommand : CursorPromptCommand
{
    public GitLogCommand() : base("Git Log", "Summarize recent history", "git_log", "Review the recent git history and summarize the most important changes, themes, and any notable risks. Do not modify anything.") { }
}

public sealed class GitCommitCommand : CursorPromptCommand
{
    public GitCommitCommand() : base("Commit", "Verify and commit the current work", "git_commit", "Review the current changes, run relevant checks, and create a clear git commit for the completed work. Preserve unrelated user changes.") { }
}

public sealed class CreatePrCommand : CursorPromptCommand
{
    public CreatePrCommand() : base("Create PR", "Review the branch and create a pull request", "create_pr", "Review and create a PR for this branch. Inspect the branch diff and recent commits, identify and fix any blocking correctness issues, run the relevant checks, then push the branch if needed and create the pull request with a concise title and a clear summary of the changes and verification performed. Preserve unrelated user changes and do not use destructive git commands.") { }
}

public sealed class GitPushCommand : CursorPromptCommand
{
    public GitPushCommand() : base("Push", "Verify and push the current branch", "git_push", "Verify the current branch and its commits, then push it to its configured remote. Report any issue without using destructive git commands.") { }
}
