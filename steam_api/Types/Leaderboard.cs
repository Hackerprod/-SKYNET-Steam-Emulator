using SKYNET.Steamworks;

namespace SKYNET.Types
{
    using SteamLeaderboard_t = System.UInt64;
    public class Leaderboard
    {
        public string Name { get; set; }
        public ELeaderboardSortMethod ShortMethod { get; set; }
        public ELeaderboardDisplayType DisplayType { get; set; }
        public SteamLeaderboard_t SteamLeaderboard { get; set; }
    }
}
