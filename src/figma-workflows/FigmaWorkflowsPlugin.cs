namespace Loupedeck.FigmaWorkflowsPlugin;

using System;

public sealed class FigmaWorkflowsPlugin : Plugin
{
    public override Boolean UsesApplicationApiOnly => true;
    public override Boolean HasNoApplication => true;

    public override void Load() { }

    public override void Unload() { }
}
