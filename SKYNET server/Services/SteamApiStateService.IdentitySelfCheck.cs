using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    /// <summary>
    /// Self-check for the ID rules: the server is the only authority over IDs, accounts are numbered
    /// "highest + 1" from 1000, and nothing a client sends ever creates a user or raises the counter.
    /// Run with `--verify-identity`.
    /// </summary>
    public static bool RunIdentitySelfCheck(Action<string>? write = null)
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

        SteamApiStateService NewService(ILogger<SteamApiStateService>? logger = null)
        {
            var service = (SteamApiStateService)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(SteamApiStateService));
            void Set(string field, object value) =>
                typeof(SteamApiStateService).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(service, value);
            Set("_sync", new object());
            Set("_sessions", new Dictionary<string, ApiSession>(StringComparer.Ordinal));
            Set("_events", new List<ApiQueuedEvent>());
            Set("_flushSignal", new ManualResetEventSlim(false));
            Set("_presenceTimeout", TimeSpan.FromSeconds(90));
            Set("_sessionTimeout", TimeSpan.FromMinutes(30));
            Set("_gameServerLeaseTimeout", TimeSpan.FromSeconds(90));
            Set("_gameServerLeases", new Dictionary<ulong, DateTime>());
            Set("_logger", logger ?? NullLogger<SteamApiStateService>.Instance);
            Set("_state", new ApiState());
            return service;
        }

        static ulong Individual(uint accountId) => SteamIdAllocator.IndividualSteamId(accountId);

        // ---- the rule itself -------------------------------------------------------------------
        Check(SteamIdAllocator.Next(Array.Empty<uint>()) == 1000, "no accounts => first ID is 1000");
        Check(SteamIdAllocator.Next(new uint[] { 1000, 1001 }) == 1002, "highest + 1");
        Check(SteamIdAllocator.Next(new uint[] { 1000, 5000 }) == 5001, "gaps do not matter, highest + 1");
        Check(SteamIdAllocator.Next(new uint[] { 999 }) == 1000, "999 is below the range and is ignored");
        Check(SteamIdAllocator.Next(new uint[] { 1000 }) == 1001, "1000 is the first valid ID");
        Check(SteamIdAllocator.Next(new uint[] { int.MaxValue - 1 }) == int.MaxValue, "int.MaxValue is the last valid ID");
        Check(SteamIdAllocator.Next(new uint[] { 3680690519U, 5 }) == 1000, "IDs above the range (legacy data) are ignored");
        Check(SteamIdAllocator.Next(new uint[] { (uint)int.MaxValue + 1U }) == 1000, "int.MaxValue + 1 is out of range");
        var exhausted = false;
        try { SteamIdAllocator.Next(new uint[] { int.MaxValue }); } catch (InvalidOperationException) { exhausted = true; }
        Check(exhausted, "an exhausted range fails loudly");
        Check(SteamIdAllocator.IsGameServerSteamId(SteamIdAllocator.GameServerSteamId(1234, false)), "GameServer SteamID recognised");
        Check(SteamIdAllocator.IsGameServerSteamId(SteamIdAllocator.GameServerSteamId(1234, true)), "AnonGameServer SteamID recognised");
        Check(!SteamIdAllocator.IsGameServerSteamId(Individual(1234)), "an Individual SteamID is not a game server");
        Check(!SteamIdAllocator.IsGameServerSteamId(SteamIdAllocator.GameServerSteamId(0, false)), "game server account 0 is not valid");

        // ---- web accounts, admin, one creation path --------------------------------------------
        var web = NewService();
        var alice = web.RegisterWeb("alice", "pw", "10.0.0.1");
        var bob = web.RegisterWeb("bob", "pw", "10.0.0.2");
        Check(alice != null && alice.User.AccountId == 1000 && alice.User.SteamId == Individual(1000), "first registration => 1000");
        Check(bob != null && bob.User.AccountId == 1001, "second registration => 1001");
        Check(alice != null && alice.IsAdmin, "the first account on an empty database becomes admin");

        var captured = new CapturingLogger();
        var stray = NewService(captured);
        stray._state.Users[Individual(3680690519U)] = new ApiUser { SteamId = Individual(3680690519U), AccountId = 3680690519U, PersonaName = "Stray" };
        var afterStray = stray.RegisterWeb("carol", "pw", "10.0.0.3");
        Check(afterStray != null && afterStray.User.AccountId == 1000, "a stray out-of-range user does not drag new accounts up");
        stray.WarnAboutInvalidAccountIds();
        Check(captured.Messages.Count == 1 && captured.Messages[0].Contains("Stray") && captured.Messages[0].Contains("3680690519"),
            "startup warns once about the user with an out-of-range ID, naming it");

        var reuse = NewService();
        reuse.RegisterWeb("one", "pw", "10.0.0.1");
        var two = reuse.RegisterWeb("two", "pw", "10.0.0.2")!;
        reuse._state.Users.Remove(two.User.SteamId);
        reuse._state.WebAccounts.Remove("two");
        var again = reuse.RegisterWeb("three", "pw", "10.0.0.3");
        Check(again != null && again.User.AccountId == two.User.AccountId, "documented: the highest ID is reused after its user is deleted");

        var emptyAdmin = NewService();
        emptyAdmin.EnsureDefaultAdminAccount();
        Check(emptyAdmin._state.Users.Values.Count == 1 && emptyAdmin._state.Users.Values.First().AccountId == 1000, "initial admin on an empty database => 1000");
        emptyAdmin.EnsureDefaultAdminAccount();
        Check(emptyAdmin._state.Users.Count == 1, "initial admin is not created twice");
        var withAdmin = NewService();
        withAdmin.RegisterWeb("root", "pw", "10.0.0.1");
        withAdmin.EnsureDefaultAdminAccount();
        Check(withAdmin._state.Users.Count == 1, "no extra admin when one already exists");

        var recreate = NewService();
        var dora = recreate.RegisterWeb("dora", "pw", "10.0.0.1")!;
        recreate._state.Users.Remove(dora.User.SteamId);
        var login = recreate.LoginWeb("dora", "pw", false, "10.0.0.1");
        Check(login != null && login.User.SteamId == dora.User.SteamId, "login recreates the ApiUser of an existing web account with its own ID");

        var social = NewService();
        var social1 = social.RegisterWeb("sol", "pw", "10.0.0.1")!;
        social._state.Users.Remove(social1.User.SteamId);
        social.GetWebUsersWithRelationships(social1.AccessToken);
        Check(social._state.Users.TryGetValue(social1.User.SteamId, out var socialUser) && socialUser.AccountId == social1.User.AccountId,
            "the users list recreates the ApiUser of an existing web account with its own ID");

        // ---- sessions never create users ---------------------------------------------------------
        ApiSessionRequest Request(ulong steamId, string instance, bool web = false, string role = "client") => new()
        {
            SteamId = steamId,
            PersonaName = "Client",
            ClientInstanceId = instance,
            ProcessRole = role,
            UseActiveWebUser = web
        };

        // CreateSessionLocked is the part under test; StartSession adds catalogs a bare service lacks.
        ApiSession? Start(SteamApiStateService service, ApiSessionRequest request, string? clientIp, out SessionFailure failure)
        {
            lock (service._sync)
            {
                return service.CreateSessionLocked(request, clientIp, out _, out failure);
            }
        }

        var sessions = NewService();
        var registered = sessions.RegisterWeb("erin", "pw", "10.0.0.1")!;
        var nextBefore = sessions.AllocateAccountIdLocked();

        var invented = Individual(3000000000U);
        var unknown = Start(sessions, Request(invented, "inst-a"), null, out var unknownFailure);
        Check(unknown == null && unknownFailure == SessionFailure.UnknownIdentity, "a session with an ID the client made up is refused");
        Check(!sessions._state.Users.ContainsKey(invented), "...and creates no user");
        var validButNew = Individual(1500);
        var unknown2 = Start(sessions, Request(validButNew, "inst-a"), null, out var unknown2Failure);
        Check(unknown2 == null && unknown2Failure == SessionFailure.UnknownIdentity && !sessions._state.Users.ContainsKey(validButNew),
            "even an in-range ID creates nothing");
        Check(sessions.AllocateAccountIdLocked() == nextBefore, "...and does not move the next registration ID");

        var known = Start(sessions, Request(registered.User.SteamId, "inst-a"), null, out var knownFailure);
        Check(known != null && knownFailure == SessionFailure.None, "a session naming an existing user is accepted");
        var sameInstance = Start(sessions, Request(registered.User.SteamId, "inst-a"), null, out _);
        Check(sameInstance != null, "the same client instance can start another session");
        var other = Start(sessions, Request(registered.User.SteamId, "inst-b"), null, out var otherFailure);
        Check(other == null && otherFailure == SessionFailure.IdentityInUse, "a second live instance with the same identity is refused (409)");
        var noWeb = Start(sessions, Request(0, "inst-c", web: true), "10.9.9.9", out var noWebFailure);
        Check(noWeb == null && noWebFailure == SessionFailure.NoWebUser, "web identity requested with nobody logged in from that IP");
        var viaWeb = Start(sessions, Request(0, "inst-d", web: true), "10.0.0.1", out var viaWebFailure);
        Check(viaWeb != null && viaWebFailure == SessionFailure.None && viaWeb.SteamId == registered.User.SteamId,
            "a web-resolved session gets the web user and ignores any client ID");

        var launcher = Start(sessions, Request(0, "inst-l", web: true, role: "launcher"), "10.0.0.1", out var launcherFailure);
        var probe = Start(sessions, Request(0, "inst-p", web: true, role: "probe"), "10.0.0.1", out var probeFailure);
        Check(launcher != null && launcher.SteamId == registered.User.SteamId && launcherFailure == SessionFailure.None,
            "a launcher session resolves to the web user");
        Check(probe != null && probe.SteamId == registered.User.SteamId && probeFailure == SessionFailure.None,
            "a probe session resolves to the web user");

        // ---- game servers --------------------------------------------------------------------------
        ApiGameServerState Register(ulong steamId, uint ip, int port, bool anonymous = false) => new()
        {
            Anonymous = anonymous,
            Server = new ApiGameServer { SteamId = steamId, IP = ip, Port = port, AppId = 570, Dedicated = true }
        };
        const string remote = "10.0.0.7";

        var gs = NewService();
        var proposed = SteamIdAllocator.GameServerSteamId(1_900_000_000U, false);
        var kept = gs.RegisterGameServer(Register(proposed, 0, 27015), remote);
        Check(kept.SteamId == proposed, "a valid ID the game server arrives with is kept as is (the ID the game sees is the final one)");
        var ipAfter = gs._state.GameServers[proposed].IP;
        var reregister = gs.RegisterGameServer(Register(proposed, 0, 27015), remote);
        Check(reregister.SteamId == proposed, "re-register / LogOn from the same endpoint keeps the ID");
        var collision = gs.RegisterGameServer(Register(proposed, 0, 27016), remote);
        Check(collision.SteamId != proposed && SteamIdAllocator.IsGameServerSteamId(collision.SteamId), "same ID from another endpoint while live => assigned a new one");
        Check(gs._state.GameServers.Count == 2 && gs._state.GameServers[proposed].Port == 27015, "...and the first server is not overwritten");

        // A game server that arrives with instance 0 (what the emulator makes) is registered under the same
        // account with the instance the game expects, and told so: the DLL adopts the ID it gets back.
        var instanceZero = gs.RegisterGameServer(Register(SteamIdAllocator.GameServerSteamId(1_800_000_000U, false) & ~(0xFFFFFUL << 32), 0, 27030), remote);
        Check(instanceZero.SteamId == SteamIdAllocator.GameServerSteamId(1_800_000_000U, false) && ((instanceZero.SteamId >> 32) & 0xFFFFF) == 1,
            "a game server ID with instance 0 is registered, and returned, with instance 1 (same account)");
        var instanceZeroUpdate = gs.UpdateGameServerState(new ApiGameServer { SteamId = SteamIdAllocator.GameServerSteamId(1_800_000_000U, false) & ~(0xFFFFFUL << 32), IP = gs._state.GameServers[instanceZero.SteamId].IP, Port = 27030, MapName = "dota" }, remote);
        Check(instanceZeroUpdate && gs._state.GameServers[instanceZero.SteamId].MapName == "dota",
            "a state update that still uses the instance-0 ID reaches the same server");
        Check(gs.LogOffGameServer(SteamIdAllocator.GameServerSteamId(1_800_000_000U, false) & ~(0xFFFFFUL << 32)) && !gs._state.GameServers.ContainsKey(instanceZero.SteamId),
            "a log off with the instance-0 ID removes the server");

        var randomHigh = SteamIdAllocator.GameServerSteamId(4_000_000_000U, false);
        var keptHigh = gs.RegisterGameServer(Register(randomHigh, 0, 27020), remote);
        Check(keptHigh.SteamId == randomHigh, "a 32-bit random ID (above the account range) is still accepted for a game server");

        var gsAnon = NewService();
        var anon1 = gsAnon.RegisterGameServer(Register(0, 0, 27015, anonymous: true), remote);
        var anon2 = gsAnon.RegisterGameServer(Register(0, 0, 27016, anonymous: true), remote);
        var named = gsAnon.RegisterGameServer(Register(0, 0, 27017, anonymous: false), remote);
        Check(anon1.SteamId == SteamIdAllocator.GameServerSteamId(1000, true) && anon2.SteamId == SteamIdAllocator.GameServerSteamId(1001, true),
            "no ID proposed => per-type counter from 1000 (AnonGameServer)");
        Check(named.SteamId == SteamIdAllocator.GameServerSteamId(1000, false), "...and an independent counter for GameServer");

        var wrongType = gsAnon.RegisterGameServer(Register(Individual(1234), 0, 27018), remote);
        Check(SteamIdAllocator.IsGameServerSteamId(wrongType.SteamId), "an ID of the wrong type is replaced by a game server ID");

        var expired = NewService();
        var oldId = SteamIdAllocator.GameServerSteamId(2000, false);
        expired.RegisterGameServer(Register(oldId, 0, 27015), remote);
        expired._gameServerLeases[oldId] = DateTime.UtcNow - TimeSpan.FromMinutes(10);
        var reuseExpired = expired.RegisterGameServer(Register(oldId, 0, 27099), remote);
        Check(reuseExpired.SteamId == oldId, "an expired registration no longer blocks the ID");

        var upd = NewService();
        var live = SteamIdAllocator.GameServerSteamId(3000, false);
        upd.RegisterGameServer(Register(live, 0, 27015), remote);
        var liveIp = upd._state.GameServers[live].IP;
        var unknownUpdate = upd.UpdateGameServerState(new ApiGameServer { SteamId = SteamIdAllocator.GameServerSteamId(9999, false), IP = liveIp, Port = 27015 }, remote);
        Check(!unknownUpdate && upd._state.GameServers.Count == 1, "a state update for an unknown server is rejected and registers nothing");
        var zeroByEndpoint = upd.UpdateGameServerState(new ApiGameServer { SteamId = 0, IP = liveIp, Port = 27015, ServerName = "renamed" }, remote);
        Check(zeroByEndpoint && upd._state.GameServers[live].ServerName == "renamed", "an update with no ID resolves the server by its endpoint");
        var zeroUnknown = upd.UpdateGameServerState(new ApiGameServer { SteamId = 0, IP = liveIp, Port = 30000 }, remote);
        Check(!zeroUnknown && upd._state.GameServers.Count == 1, "an update with no ID and an unknown endpoint is rejected (no ID invented from the IP)");
        var otherEndpointUpdate = upd.UpdateGameServerState(new ApiGameServer { SteamId = live, IP = liveIp, Port = 27099, MapName = "hijack" }, remote);
        Check(!otherEndpointUpdate && upd._state.GameServers[live].Port == 27015 && upd._state.GameServers[live].MapName != "hijack",
            "an update for a known ID from another endpoint is rejected and does not overwrite the server");
        upd._gameServerLeases[live] = DateTime.UtcNow - TimeSpan.FromMinutes(10);
        var expiredUpdate = upd.UpdateGameServerState(new ApiGameServer { SteamId = live, IP = liveIp, Port = 27015, MapName = "late" }, remote);
        Check(!expiredUpdate && upd._state.GameServers[live].MapName != "late", "an update for a server whose lease expired is rejected");
        upd._gameServerLeases[live] = DateTime.UtcNow;
        var knownUpdate = upd.UpdateGameServerState(new ApiGameServer { SteamId = live, IP = liveIp, Port = 27015, MapName = "dota" }, remote);
        Check(knownUpdate && upd._state.GameServers[live].MapName == "dota", "an update for a registered server is applied");

        // The fallback assignment must skip IDs reserved by a live dedicated lobby.
        var reserved = NewService();
        var supervisorType = typeof(DotaDedicatedServerSupervisor);
        var reservationType = supervisorType.GetNestedType("DedicatedReservation", BindingFlags.NonPublic)!;
        var supervisor = (DotaDedicatedServerSupervisor)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(supervisorType);
        supervisorType.GetField("_sync", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(supervisor, new object());
        supervisorType.GetField("_logger", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(supervisor, NullLogger<DotaDedicatedServerSupervisor>.Instance);
        var reservations = (System.Collections.IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(typeof(ulong), reservationType))!;
        supervisorType.GetField("_reservations", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(supervisor, reservations);
        var reservation = Activator.CreateInstance(reservationType, 777UL, 27015, "dota", System.Diagnostics.Process.GetCurrentProcess(), DateTime.UtcNow)!;
        reservationType.GetProperty("GameServerSteamId")!.SetValue(reservation, SteamIdAllocator.GameServerSteamId(1000, true));
        reservations.Add(777UL, reservation);
        typeof(SteamApiStateService).GetField("_dotaDedicatedServers", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(reserved, supervisor);
        var skipped = reserved.RegisterGameServer(Register(0, 0, 27015, anonymous: true), remote);
        Check(skipped.SteamId == SteamIdAllocator.GameServerSteamId(1001, true),
            "the fallback assignment skips an ID reserved by a live dedicated lobby");

        write(failures == 0 ? "Identity self-check passed." : $"Identity self-check FAILED ({failures}).");
        return failures == 0;
    }

    private sealed class CapturingLogger : ILogger<SteamApiStateService>
    {
        public List<string> Messages { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            Messages.Add(formatter(state, exception));
    }
}
