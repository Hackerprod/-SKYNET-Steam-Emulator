namespace Overlay.Core;

public sealed class OverlayOptions
{
    public IntPtr TargetWindowHandle { get; set; }
    public bool FollowTargetWindow { get; set; } = true;
    public bool TopMost { get; set; } = true;
    public bool DimBackground { get; set; } = true;
    public int Margin { get; set; } = 48;
}
