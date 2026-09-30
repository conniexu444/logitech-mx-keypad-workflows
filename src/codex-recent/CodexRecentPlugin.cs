namespace Loupedeck.CodexRecentPlugin;

using System;

public sealed class CodexRecentPlugin : Plugin
{
    public override Boolean UsesApplicationApiOnly => true;
    public override Boolean HasNoApplication => true;

    public override void Load() { }

    public override void Unload() { }
}
