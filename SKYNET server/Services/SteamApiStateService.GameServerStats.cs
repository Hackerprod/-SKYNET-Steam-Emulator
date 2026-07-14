using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    public ApiStatsEnvelope GetGameServerUserStats(ulong steamId)
    {
        lock (_sync)
        {
            if (!_state.Stats.TryGetValue(steamId, out var stats))
            {
                stats = new ApiStatsEnvelope { SteamId = steamId, CurrentPlayers = 1 };
                _state.Stats[steamId] = stats;
                SaveState();
            }

            return CloneStats(stats);
        }
    }

    public bool StoreGameServerUserStats(ulong steamId, ApiStoreStatsRequest request)
    {
        lock (_sync)
        {
            _state.Stats[steamId] = new ApiStatsEnvelope
            {
                SteamId = steamId,
                CurrentPlayers = 1,
                Stats = request.Stats ?? new List<ApiStat>(),
                Achievements = request.Achievements ?? new List<ApiAchievement>()
            };

            SaveState();
            return true;
        }
    }
}
