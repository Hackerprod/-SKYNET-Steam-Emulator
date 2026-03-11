using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    public SkyNetStatsEnvelopeDto GetGameServerUserStats(ulong steamId)
    {
        lock (_sync)
        {
            if (!_state.Stats.TryGetValue(steamId, out var stats))
            {
                stats = new SkyNetStatsEnvelopeDto { SteamId = steamId, CurrentPlayers = 1 };
                _state.Stats[steamId] = stats;
                SaveState();
            }

            return CloneStats(stats);
        }
    }

    public bool StoreGameServerUserStats(ulong steamId, SkyNetStoreStatsRequestDto request)
    {
        lock (_sync)
        {
            _state.Stats[steamId] = new SkyNetStatsEnvelopeDto
            {
                SteamId = steamId,
                CurrentPlayers = 1,
                Stats = request.Stats ?? new List<SkyNetStatDto>(),
                Achievements = request.Achievements ?? new List<SkyNetAchievementDto>()
            };

            SaveState();
            return true;
        }
    }
}
