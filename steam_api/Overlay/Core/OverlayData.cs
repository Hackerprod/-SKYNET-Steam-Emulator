namespace Overlay.Core;

public sealed class OverlayUser
{
    public ulong SteamId { get; set; }
    public uint AccountId { get; set; }
    public string PersonaName { get; set; } = string.Empty;
    public uint AppId { get; set; }
    public ulong LobbyId { get; set; }
    public int PersonaState { get; set; }
    public bool HasFriend { get; set; }
    public int FriendRelationship { get; set; }
    public bool IsSelf { get; set; }
    public string Status { get; set; } = string.Empty;
    public byte[] AvatarPng { get; set; } = new byte[0];
    public Dictionary<string, string> RichPresence { get; set; } = new Dictionary<string, string>();
}

public sealed class OverlayStat
{
    public string Name { get; set; } = string.Empty;
    public uint Value { get; set; }
    public string DisplayValue { get; set; } = string.Empty;
    public uint MaxValue { get; set; }
}

public sealed class OverlayAchievement
{
    public string Name { get; set; } = string.Empty;
    public bool Earned { get; set; }
    public DateTime Date { get; set; }
    public uint Progress { get; set; }
    public uint MaxProgress { get; set; }
}

public sealed class OverlaySummaryItem
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Tone { get; set; } = string.Empty;
}

public sealed class OverlayActivityItem
{
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public sealed class OverlayStoreItem
{
    public uint AppId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
