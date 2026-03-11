namespace SKYNET_server.Models;

public sealed class SteamUiSnapshot
{
    public SteamProfileCard Profile { get; init; } = new();
    public SteamGame FeaturedGame { get; init; } = new();
    public IReadOnlyList<SteamMetric> Metrics { get; init; } = Array.Empty<SteamMetric>();
    public IReadOnlyList<SteamGame> Games { get; init; } = Array.Empty<SteamGame>();
    public IReadOnlyList<SteamFriend> Friends { get; init; } = Array.Empty<SteamFriend>();
    public IReadOnlyList<SteamAchievement> Achievements { get; init; } = Array.Empty<SteamAchievement>();
    public IReadOnlyList<SteamActivity> Activities { get; init; } = Array.Empty<SteamActivity>();
}

public sealed class SteamProfileCard
{
    public string DisplayName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int Level { get; init; }
    public int LibraryCount { get; init; }
    public int CompletedGames { get; init; }
}

public sealed class SteamMetric
{
    public string Label { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string Detail { get; init; } = string.Empty;
}

public sealed class SteamGame
{
    public string Name { get; init; } = string.Empty;
    public string Genre { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int HoursPlayed { get; init; }
    public int Progress { get; init; }
    public int AchievementsUnlocked { get; init; }
    public int TotalAchievements { get; init; }
}

public sealed class SteamFriend
{
    public string DisplayName { get; init; } = string.Empty;
    public string Initials { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string CurrentGame { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
}

public sealed class SteamAchievement
{
    public string Title { get; init; } = string.Empty;
    public string Game { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Rarity { get; init; } = string.Empty;
    public bool Unlocked { get; init; }
    public int Progress { get; init; }
}

public sealed class SteamActivity
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string When { get; init; } = string.Empty;
}
