namespace Loupedeck.StarlightPlugin;

using System;

// Logitech requires a ClientApplication type even for a universal plugin.
public class StarlightApplication : ClientApplication
{
    public StarlightApplication()
    {
    }

    protected override String GetProcessName() => String.Empty;

    protected override String GetBundleName() => String.Empty;
}
