namespace Loupedeck.CodexRecentPlugin;

using System;

public sealed class CodexRecentApplication : ClientApplication
{
    protected override String GetProcessName() => "ChatGPT";

    protected override String GetBundleName() => "com.openai.codex";
}
