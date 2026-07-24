namespace Overlay.Core;

public sealed class OverlayRequest
{
    public OverlayKind Kind { get; set; }
    public ulong? UserId { get; set; }
    public ulong? SessionId { get; set; }
    public uint? AppId { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string Message { get; set; }
    public string PrimaryActionText { get; set; }
    public string SecondaryActionText { get; set; }
    public Action PrimaryAction { get; set; }
    public Action SecondaryAction { get; set; }
    public Action<OverlayUser, Action<bool>> InviteUserAction { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
    public OverlayUser User { get; set; }
    public List<OverlayUser> Users { get; set; } = new List<OverlayUser>();
    public List<OverlayStat> Stats { get; set; } = new List<OverlayStat>();
    public List<OverlayAchievement> Achievements { get; set; } = new List<OverlayAchievement>();
    public List<OverlaySummaryItem> Summary { get; set; } = new List<OverlaySummaryItem>();
    public List<OverlayActivityItem> Activities { get; set; } = new List<OverlayActivityItem>();
    public List<OverlayStoreItem> StoreItems { get; set; } = new List<OverlayStoreItem>();
}
