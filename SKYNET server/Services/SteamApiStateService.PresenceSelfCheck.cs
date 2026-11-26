using System.Reflection;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    /// <summary>
    /// Self-check for launcher-level presence: a launcher-only user must be "online"
    /// for the launcher Users view but never count as a live game session for the Dota GC.
    /// Run with `--verify-presence`.
    /// </summary>
    public static bool RunPresenceSelfCheck(Action<string>? write = null)
    {
        write ??= _ => { };
        var failures = 0;
        void Check(bool condition, string name)
        {
            write($"{(condition ? "PASS" : "FAIL")} {name}");
            if (!condition)
            {
                failures++;
            }
        }

        var service = (SteamApiStateService)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(SteamApiStateService));
        void Set(string field, object value) =>
            typeof(SteamApiStateService).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(service, value);
        Set("_sync", new object());
        Set("_sessions", new Dictionary<string, ApiSession>(StringComparer.Ordinal));
        Set("_events", new List<ApiQueuedEvent>());
        Set("_flushSignal", new ManualResetEventSlim(false));
        Set("_presenceTimeout", TimeSpan.FromSeconds(90));
        Set("_sessionTimeout", TimeSpan.FromMinutes(30));
        Set("_state", new ApiState());

        const ulong steamId = 76561197960287930UL;
        const uint gameAppId = 570;

        ApiSessionRequest Request(string role, string instance, uint appId = 0) => new()
        {
            SteamId = steamId,
            AppId = appId,
            PersonaName = "Alice",
            ClientInstanceId = instance,
            ProcessRole = role
        };

        ApiSession Start(ApiSessionRequest request)
        {
            lock (service._sync)
            {
                return service.CreateSessionLocked(request, null, out _)!;
            }
        }

        string Level()
        {
            lock (service._sync)
            {
                return service.GetPresenceLevelLocked(steamId);
            }
        }

        // What a friend (e.g. the in-game friends list) gets from GET /friends.
        ApiUser FriendView()
        {
            lock (service._sync)
            {
                return service.CloneUserForViewerLocked(service._state.Users[steamId], 76561197960287931UL);
            }
        }

        int Count(string role)
        {
            lock (service._sync)
            {
                return service._sessions.Values.Count(s => s.SteamId == steamId && s.ProcessRole == role);
            }
        }

        // Seed the user with a game AppId, then drop the seed game session.
        var seed = Start(Request("client", "seed", gameAppId))!;
        service.MarkSelfOffline(seed.AccessToken);
        Check(Level() == "offline", "no sessions => offline");

        Start(Request("launcher", "inst-1"));
        Start(Request("launcher", "inst-1"));
        var launcherToken = Start(Request("launcher", "inst-1"))!.AccessToken;
        Check(Count("launcher") == 1, "re-resolve keeps exactly one launcher session");
        Check(Level() == "online", "launcher-only => online");
        Check(!service.IsOnlineDotaUser(steamId), "launcher-only => IsOnlineDotaUser (GC provider) false");
        Check(service._state.Users[steamId].AppId == gameAppId, "launcher does not overwrite user AppId");

        Start(Request("probe", "inst-1"));
        Check(Count("probe") == 1 && Level() == "online" && !service.IsOnlineDotaUser(steamId), "probe counts for nothing");

        service.ReconcilePresence();
        Check(service._state.Users[steamId].PersonaState == 1, "launcher-only => stored PersonaState 1 (visible to friends)");
        var launcherOnlyView = FriendView();
        Check(launcherOnlyView.PersonaState == 1 && launcherOnlyView.GameState == "online",
            "launcher-only => friends (game friends list) see ONLINE, not offline");
        Check(launcherOnlyView.AppId == 0 && launcherOnlyView.RichPresence.Count == 0,
            "launcher-only => friends do not see a game being played");

        var game = Start(Request("client", "inst-1", gameAppId))!;
        service.SetPresence(game.AccessToken, "status", "In lobby");
        Check(Level() == "in_game" && service.IsOnlineDotaUser(steamId), "game session => in_game and GC online");
        var inGameView = FriendView();
        Check(inGameView.PersonaState == 1 && inGameView.AppId == gameAppId && inGameView.GameState != "online",
            "game session => friends see the game being played");
        Check(Count("launcher") == 1, "game start keeps launcher session (same ClientInstanceId)");

        service.MarkSelfOffline(game.AccessToken);
        Check(Count("launcher") == 1 && Level() == "online" && !service.IsOnlineDotaUser(steamId), "game offline => back to online, launcher survives");
        var afterGameView = FriendView();
        Check(afterGameView.PersonaState == 1 && afterGameView.AppId == 0 && afterGameView.RichPresence.Count == 0
            && service._state.Users[steamId].PersonaState == 1 && service._state.Users[steamId].RichPresence.Count == 0,
            "game offline => friends still see ONLINE, stale game data cleared");

        var game2 = Start(Request("client", "inst-1", gameAppId))!;
        service.MarkSelfOffline(launcherToken);
        Check(Count("launcher") == 0 && Level() == "in_game", "launcher offline while game runs leaves in_game");
        service.MarkSelfOffline(game2.AccessToken);
        Check(Level() == "offline", "all gone => offline");
        Check(service._state.Users[steamId].PersonaState == 0 && FriendView().PersonaState == 0 && FriendView().GameState == "offline",
            "all gone => friends see OFFLINE");

        // Same ClientInstanceId as above: a different one would look like an identity
        // collision to the (non-web) session resolver and remap the SteamId.
        var closingToken = Start(Request("launcher", "inst-1"))!.AccessToken;
        Check(FriendView().PersonaState == 1 && Level() == "online", "launcher start => friends see ONLINE immediately");
        service.MarkSelfOffline(closingToken);
        Check(service._state.Users[steamId].PersonaState == 0 && FriendView().PersonaState == 0,
            "launcher exit (no game) => friends see OFFLINE immediately");

        Start(Request("launcher", "inst-1"));
        lock (service._sync)
        {
            foreach (var session in service._sessions.Values)
            {
                session.LastSeenUtc = DateTime.UtcNow - TimeSpan.FromSeconds(300);
            }

            Check(service.GetPresenceLevelLocked(steamId) == "offline", "stale launcher session => offline");
        }

        write(failures == 0 ? "Presence self-check passed." : $"Presence self-check FAILED ({failures}).");
        return failures == 0;
    }
}
