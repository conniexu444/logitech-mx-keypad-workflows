namespace Loupedeck.StarlightPlugin;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

public abstract class MatrixPanelCommand : PluginDynamicCommand
{
    private const Int32 FrameMilliseconds = 100;
    private readonly Byte[][] _frames;
    private readonly Timer _timer;

    protected MatrixPanelCommand(String displayName, String panelName)
        : base(displayName, "Pulsing stars; pressing does nothing", "Starlight")
    {
        // Render as a full-key surface instead of an inset command icon.
        this.SetWidget(true);

        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames()
            .Where(name => name.Contains($".frames.{panelName}-", StringComparison.OrdinalIgnoreCase) && name.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        if (resourceNames.Length == 0)
        {
            throw new InvalidOperationException($"Missing embedded animation frames: {panelName}");
        }
        this._frames = resourceNames.Select(resourceName => ReadResource(assembly, resourceName)).ToArray();
        this._timer = new Timer(_ => this.NotifyFrameChanged(), null, 250, FrameMilliseconds);
    }

    protected override void RunCommand(String actionParameter)
    {
        // Decorative key: intentionally no action.
    }

    protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
    {
        // A blank-space label still makes Logitech reserve a 40-pixel text area,
        // shrinking the artwork to 50x50. Null enables the full 80x80 image area.
        return null;
    }

    protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
    {
        var frameIndex = (Int32)((Environment.TickCount64 / FrameMilliseconds) % this._frames.Length);
        return BitmapImage.FromArray(this._frames[frameIndex]);
    }

    private static Byte[] ReadResource(Assembly assembly, String resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }

    private void NotifyFrameChanged()
    {
        try
        {
            this.ActionImageChanged();
        }
        catch
        {
            // The service can briefly detach actions during profile transitions.
        }
    }
}

public sealed class MatrixLeftCommand : MatrixPanelCommand
{
    public MatrixLeftCommand() : base("Starlight Left", "left") { }
}

public sealed class MatrixCenterCommand : MatrixPanelCommand
{
    public MatrixCenterCommand() : base("Starlight Center", "center") { }
}

public sealed class MatrixRightCommand : MatrixPanelCommand
{
    public MatrixRightCommand() : base("Starlight Right", "right") { }
}
