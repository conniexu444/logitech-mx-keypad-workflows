namespace Loupedeck.CodexRecentPlugin;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

internal static class CodexWorkflowRunner
{
    private static readonly String ScriptPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        "Library/Application Support/Logi/LogiPluginService/Plugins/CodexRecent/bin/codex-workflow.applescript");
    private const String LogPath = "/private/tmp/codex-recent-plugin.log";

    public static void OpenUrl(String url) => Run("/usr/bin/open", url);

    public static void RunMenuItem(String menuName, String itemName) =>
        Run("/usr/bin/osascript", ScriptPath, "menu", menuName, itemName);

    public static void StartPrompt(String prompt) =>
        Run("/usr/bin/osascript", ScriptPath, "prompt", prompt);

    private static void Run(String executable, params String[] arguments)
    {
        try
        {
            var startInfo = new ProcessStartInfo(executable)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            foreach (var argument in arguments)
            {
                startInfo.ArgumentList.Add(argument);
            }
            Process.Start(startInfo);
        }
        catch (Exception exception)
        {
            try
            {
                File.AppendAllText(LogPath, $"{DateTime.Now:O} Workflow failed: {exception}{Environment.NewLine}");
            }
            catch
            {
                // Diagnostics must never prevent another keypad action from running.
            }
        }
    }
}

public abstract class WorkflowCommand : PluginDynamicCommand
{
    private readonly Action _run;
    private readonly Byte[] _image;

    protected WorkflowCommand(String name, String description, String iconName, Action run)
        : base(name, description, "Codex Workflows")
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

    protected override void RunCommand(String actionParameter) => this._run();

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) =>
        BitmapImage.FromArray(this._image);

    protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) => null;
}

public sealed class PreviousTaskCommand : WorkflowCommand
{
    public PreviousTaskCommand() : base("Previous Task", "Open the previous Codex task", "previous", () => CodexWorkflowRunner.RunMenuItem("View", "Previous Chat")) { }
}

public sealed class NewTaskCommand : WorkflowCommand
{
    public NewTaskCommand() : base("New Task", "Start a new Codex task", "new", () => CodexWorkflowRunner.OpenUrl("codex://threads/new")) { }
}

public sealed class NextTaskCommand : WorkflowCommand
{
    public NextTaskCommand() : base("Next Task", "Open the next Codex task", "next", () => CodexWorkflowRunner.RunMenuItem("View", "Next Chat")) { }
}

public sealed class ToggleSidebarCommand : WorkflowCommand
{
    public ToggleSidebarCommand() : base("Toggle Sidebar", "Show or hide the Codex sidebar", "sidebar", () => CodexWorkflowRunner.RunMenuItem("View", "Toggle Sidebar")) { }
}

public sealed class OpenTerminalCommand : WorkflowCommand
{
    public OpenTerminalCommand() : base("Open Terminal", "Open the Codex terminal panel", "terminal", () => CodexWorkflowRunner.RunMenuItem("View", "Open Terminal")) { }
}

public sealed class ReviewPanelCommand : WorkflowCommand
{
    public ReviewPanelCommand() : base("Review Panel", "Show or hide the Codex review panel", "review_panel", () => CodexWorkflowRunner.RunMenuItem("View", "Toggle Review Panel")) { }
}

public abstract class PromptWorkflowCommand : WorkflowCommand
{
    protected PromptWorkflowCommand(String name, String description, String iconName, String prompt)
        : base(name, description, iconName, () => CodexWorkflowRunner.StartPrompt(prompt)) { }
}

public sealed class ExploreCommand : PromptWorkflowCommand
{
    public ExploreCommand() : base("Explore", "Explore and summarize the codebase", "explore", "Explore this codebase and summarize its architecture, important entry points, and current risks. Do not change any files.") { }
}

public sealed class ExplainCommand : PromptWorkflowCommand
{
    public ExplainCommand() : base("Explain", "Explain the relevant code", "explain", "Explain the currently relevant code and how its main components interact. Do not change any files.") { }
}

public sealed class ReviewCodeCommand : PromptWorkflowCommand
{
    public ReviewCodeCommand() : base("Review Code", "Review the current changes", "review_code", "Review the current changes for correctness, regressions, security issues, and missing tests. Do not modify files.") { }
}

public sealed class OptimizeCommand : PromptWorkflowCommand
{
    public OptimizeCommand() : base("Optimize", "Find and implement safe optimizations", "optimize", "Identify the highest-impact performance improvements in this project, implement the safe improvements, and verify the result.") { }
}

public sealed class RefactorCommand : PromptWorkflowCommand
{
    public RefactorCommand() : base("Refactor", "Improve structure without changing behavior", "refactor", "Refactor the relevant code to improve clarity and maintainability without changing behavior. Run appropriate verification afterward.") { }
}

public sealed class WriteTestsCommand : PromptWorkflowCommand
{
    public WriteTestsCommand() : base("Write Tests", "Add meaningful test coverage", "tests", "Identify the most important missing tests, add meaningful coverage, and run the relevant test suite.") { }
}

public sealed class DocumentCommand : PromptWorkflowCommand
{
    public DocumentCommand() : base("Document", "Improve project documentation", "document", "Improve the documentation for the relevant code and workflows. Keep it concise, accurate, and aligned with the implementation.") { }
}

public sealed class FixBugCommand : PromptWorkflowCommand
{
    public FixBugCommand() : base("Fix Bug", "Diagnose and fix the current issue", "fix_bug", "Investigate the current bug or failing behavior, determine the root cause, implement a fix, and verify it.") { }
}

public sealed class SecurityCommand : PromptWorkflowCommand
{
    public SecurityCommand() : base("Security", "Audit the current changes for security", "security", "Audit the current changes and relevant code paths for security vulnerabilities, then fix and verify any concrete issues you find.") { }
}

public sealed class GitStatusCommand : PromptWorkflowCommand
{
    public GitStatusCommand() : base("Git Status", "Summarize the working tree", "git_status", "Inspect git status and summarize the current branch, staged changes, unstaged changes, and untracked files. Do not modify anything.") { }
}

public sealed class GitDiffCommand : PromptWorkflowCommand
{
    public GitDiffCommand() : base("Git Diff", "Explain the current diff", "git_diff", "Inspect the current git diff and summarize the intent, important changes, and possible problems. Do not modify files.") { }
}

public sealed class GitLogCommand : PromptWorkflowCommand
{
    public GitLogCommand() : base("Git Log", "Summarize recent history", "git_log", "Review the recent git history and summarize the most important changes, themes, and any notable risks. Do not modify anything.") { }
}

public sealed class GitCommitCommand : PromptWorkflowCommand
{
    public GitCommitCommand() : base("Commit", "Verify and commit the current work", "git_commit", "Review the current changes, run relevant checks, and create a clear git commit for the completed work. Preserve unrelated user changes.") { }
}

public sealed class CreatePrCommand : PromptWorkflowCommand
{
    public CreatePrCommand() : base("Create PR", "Prepare and create a pull request", "create_pr", "Review the current branch, run relevant checks, push it if needed, and create a pull request with a concise title and description.") { }
}

public sealed class GitPushCommand : PromptWorkflowCommand
{
    public GitPushCommand() : base("Push", "Verify and push the current branch", "git_push", "Verify the current branch and its commits, then push it to its configured remote. Report any issue without using destructive git commands.") { }
}
