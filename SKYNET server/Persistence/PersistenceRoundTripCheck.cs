using Microsoft.EntityFrameworkCore;
using SKYNET_server.Models;

namespace SKYNET_server.Persistence;

/// <summary>
/// Self-check that a representative <see cref="ApiState"/> survives a
/// Save -> Load round-trip through app.db unchanged. Guards against the main
/// risk of the hand-written mapping in <see cref="StatePersistence"/>: a field
/// or aggregate silently dropped when a model class gains/loses a property.
/// Run with `--verify-persistence`.
/// </summary>
public static class PersistenceRoundTripCheck
{
    public static bool Run(Action<string> log)
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"skynet-roundtrip-{Guid.NewGuid():N}.db");
        try
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data Source={dbPath}").Options;

            var original = BuildSampleState();
            using (var ctx = new AppDbContext(options))
            {
                ctx.Database.Migrate();
                StatePersistence.Save(ctx, original, includeCatalog: true);
            }

            ApiState loaded;
            using (var ctx = new AppDbContext(options))
            {
                loaded = StatePersistence.Load(ctx);
            }

            return Compare(original, loaded, log);
        }
        finally
        {
            foreach (var suffix in new[] { "", "-wal", "-shm" })
            {
                try { File.Delete(dbPath + suffix); } catch { /* best effort */ }
            }
        }
    }

    private static ApiState BuildSampleState()
    {
        const ulong a = 76561197960287930UL;
        const ulong b = 76561197960287931UL;
        const ulong lobbyId = 90000000000000123UL;

        var state = new ApiState { ActiveWebSteamId = a };

        state.Users[a] = new ApiUser { SteamId = a, AccountId = 22222, PersonaName = "Alice", AppId = 570, PlayerLevel = 42 };
        state.Users[b] = new ApiUser { SteamId = b, AccountId = 22223, PersonaName = "Bob", AppId = 570, PlayerLevel = 7 };

        state.FriendLinks[a] = new HashSet<ulong> { b };
        state.FriendLinks[b] = new HashSet<ulong> { a };
        state.FriendRequests.Add(new ApiFriendRequest { Id = "req1", FromSteamId = a, ToSteamId = b, Status = "pending", CreatedAt = DateTime.UtcNow });

        state.Avatars[a] = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 250 });

        state.Stats[a] = new ApiStatsEnvelope
        {
            SteamId = a,
            Stats = { new ApiStat { Name = "kills", Data = 100 }, new ApiStat { Name = "deaths", Data = 5 } },
            Achievements = { new ApiAchievement { Name = "first_blood", Earned = true, Progress = 1, MaxProgress = 1, Date = DateTime.UtcNow } },
        };

        state.WebAccounts["alice"] = new ApiWebAccount { Username = "alice", PasswordHash = "hash", SteamId = a, IsAdmin = true, CreatedAt = DateTime.UtcNow, LastLoginAt = DateTime.UtcNow };
        state.WebSessions["token-1"] = new ApiSession { AccessToken = "token-1", RefreshToken = "r1", SteamId = a, ProcessRole = "web", WebSession = true, Persistent = true, ExpiresAtUtc = DateTime.UtcNow.AddDays(30) };

        state.GameServers[a] = new ApiGameServer { SteamId = a, AppId = 570, ServerName = "srv", KeyValues = { ["k"] = "v", ["region"] = "sky" } };

        state.Lobbies[lobbyId] = new ApiLobby
        {
            SteamId = lobbyId,
            AppId = 570,
            OwnerSteamId = a,
            MaxMembers = 10,
            LobbyData = { ["name"] = "Room", ["mode"] = "ap" },
            Members = { new ApiLobbyMember { SteamId = a, Data = { new ApiLobbyMetaData { Key = "slot", Value = "1" } } } },
            GameServer = new ApiLobbyGameServer { SteamId = a, IP = 0x0A0B0C0D, Port = 27015 },
        };

        state.Files["a/save.dat"] = new ApiRemoteStorageFile { FileName = "save.dat", ContentBase64 = Convert.ToBase64String(new byte[] { 9, 8, 7 }), Size = 3, Timestamp = 1234 };

        state.DotaItems[101] = new ApiDotaItem { DefIndex = 101, Name = "Blade", Slot = "weapon", HeroNames = { "npc_dota_hero_axe" }, HeroIds = { 2 } };

        state.DotaEquipment[a] = new List<ApiDotaEquipment>
        {
            new() { SteamId = a, HeroId = 2, SlotId = 1, DefIndex = 101, ItemId = 555, Slot = "weapon", HeroName = "axe", UpdatedAt = DateTime.UtcNow },
        };

        state.DotaMatches[lobbyId] = new ApiDotaMatch
        {
            LobbyId = lobbyId,
            MatchId = 7777,
            ServerSteamId = a,
            Connect = "10.0.0.1:27015",
            Players =
            {
                new ApiDotaMatchPlayer
                {
                    SteamId = a, AccountId = 22222, PersonaName = "Alice", Team = 2, HeroId = 2,
                    Equipment = { new ApiDotaEquipment { SteamId = a, HeroId = 2, SlotId = 1, DefIndex = 101, ItemId = 555 } },
                },
            },
        };

        state.DotaHeroIds["npc_dota_hero_axe"] = 2;
        state.DotaHeroSlots["axe"] = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase) { ["weapon"] = 1 };
        state.DotaCosmetics = new ApiDotaCosmeticSettings { DotaPath = "C:/dota", LastImportStatus = "OK", EquipmentVersion = 3 };

        return state;
    }

    private static bool Compare(ApiState original, ApiState loaded, Action<string> log)
    {
        var failures = new List<string>();

        void Check(string name, bool ok)
        {
            if (!ok)
            {
                failures.Add(name);
            }
        }

        Check("ActiveWebSteamId", loaded.ActiveWebSteamId == original.ActiveWebSteamId);
        Check("Users.count", loaded.Users.Count == original.Users.Count);
        Check("User.PersonaName", loaded.Users.TryGetValue(76561197960287930UL, out var u) && u.PersonaName == "Alice" && u.PlayerLevel == 42);
        Check("FriendLinks", loaded.FriendLinks.Sum(p => p.Value.Count) == original.FriendLinks.Sum(p => p.Value.Count));
        Check("FriendRequests", loaded.FriendRequests.Count == 1 && loaded.FriendRequests[0].Id == "req1");
        Check("Avatar.base64", loaded.Avatars.TryGetValue(76561197960287930UL, out var av) && av == original.Avatars[76561197960287930UL]);
        Check("Stats", loaded.Stats.TryGetValue(76561197960287930UL, out var st) && st.Stats.Count == 2 && st.Achievements.Count == 1);
        Check("WebAccounts", loaded.WebAccounts.Count == 1 && loaded.WebAccounts["alice"].IsAdmin);
        Check("WebSessions", loaded.WebSessions.Count == 1 && loaded.WebSessions["token-1"].SteamId == 76561197960287930UL);
        Check("GameServer.KeyValues", loaded.GameServers.TryGetValue(76561197960287930UL, out var gs) && gs.KeyValues.TryGetValue("region", out var rv) && rv == "sky");

        var lobby = loaded.Lobbies.Values.FirstOrDefault();
        Check("Lobby.Data", lobby is not null && lobby.LobbyData.TryGetValue("mode", out var lm) && lm == "ap");
        Check("Lobby.Member.Data", lobby is not null && lobby.Members.Count == 1 && lobby.Members[0].Data.Count == 1 && lobby.Members[0].Data[0].Value == "1");
        Check("Lobby.GameServer", lobby?.GameServer is not null && lobby.GameServer.Port == 27015);

        Check("RemoteFiles", loaded.Files.Count == 1 && loaded.Files["a/save.dat"].ContentBase64 == original.Files["a/save.dat"].ContentBase64);
        Check("DotaItems", loaded.DotaItems.Count == 1 && loaded.DotaItems[101].HeroNames.Count == 1 && loaded.DotaItems[101].HeroIds.Contains(2u));
        Check("DotaEquipment", loaded.DotaEquipment.TryGetValue(76561197960287930UL, out var eq) && eq.Count == 1 && eq[0].ItemId == 555);

        var match = loaded.DotaMatches.Values.FirstOrDefault();
        Check("DotaMatch", match is not null && match.MatchId == 7777 && match.Players.Count == 1);
        Check("DotaMatch.Player.Equipment", match is not null && match.Players[0].Equipment.Count == 1 && match.Players[0].Equipment[0].DefIndex == 101);
        Check("DotaHeroIds", loaded.DotaHeroIds.TryGetValue("npc_dota_hero_axe", out var hid) && hid == 2);
        Check("DotaHeroSlots", loaded.DotaHeroSlots.TryGetValue("axe", out var slots) && slots.TryGetValue("weapon", out var sid) && sid == 1);
        Check("Cosmetics", loaded.DotaCosmetics.EquipmentVersion == 3 && loaded.DotaCosmetics.DotaPath == "C:/dota");

        if (failures.Count == 0)
        {
            log($"Persistence round-trip: PASS ({22} checks).");
            return true;
        }

        log($"Persistence round-trip: FAIL. Mismatched: {string.Join(", ", failures)}");
        return false;
    }
}
