namespace Loupedeck.CursorWorkflowsPlugin;

using System;

public sealed class CursorWorkflowsPlugin : Plugin
{
    public override Boolean UsesApplicationApiOnly => true;
    public override Boolean HasNoApplication => true;

    public override void Load() { }

    public override void Unload() { }
}
