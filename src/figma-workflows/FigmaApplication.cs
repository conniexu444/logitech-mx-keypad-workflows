namespace Loupedeck.FigmaWorkflowsPlugin;

using System;

public sealed class FigmaApplication : ClientApplication
{
    protected override String GetProcessName() => "Figma";

    protected override String GetBundleName() => "com.figma.Desktop";
}
