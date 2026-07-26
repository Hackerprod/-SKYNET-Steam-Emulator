using System;
using SKYNET.Steamworks;

namespace SKYNET.Types
{
    using SteamLeaderboard_t = System.UInt64;

    public class Leaderboard
    {
        public string Name { get; set; } = string.Empty;
        public ELeaderboardSortMethod ShortMethod { get; set; }
        public ELeaderboardDisplayType DisplayType { get; set; }
        public SteamLeaderboard_t SteamLeaderboard { get; set; }
        public int EntryCount { get; set; }
    }

    public sealed class LeaderboardEntryData
    {
        public ulong SteamId { get; set; }
        public int GlobalRank { get; set; }
        public int Score { get; set; }
        public int[] Details { get; set; } = Array.Empty<int>();
        public ulong UgcHandle { get; set; }
    }

    public sealed class LeaderboardEntriesData
    {
        public Leaderboard Leaderboard { get; set; }
        public LeaderboardEntryData[] Entries { get; set; } = Array.Empty<LeaderboardEntryData>();
    }

    public sealed class LeaderboardScoreUploadData
    {
        public bool Success { get; set; }
        public bool ScoreChanged { get; set; }
        public int Score { get; set; }
        public int GlobalRankNew { get; set; }
        public int GlobalRankPrevious { get; set; }
    }
}
