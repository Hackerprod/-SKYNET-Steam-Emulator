namespace Overlay.Core;

public sealed class OverlayActivatedEventArgs : EventArgs
{
    public bool Active { get; }
    public bool UserInitiated { get; }

    public OverlayActivatedEventArgs(bool active, bool userInitiated)
    {
        Active = active;
        UserInitiated = userInitiated;
    }
}
