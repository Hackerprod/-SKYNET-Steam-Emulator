using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.Persistence;

/// <summary>
/// Self-check that a representative <see cref="ApiState"/> survives a
/// Save -> Load round-trip through steam.db and dota.db unchanged. Guards against the main
/// risk of the hand-written mapping in <see cref="StatePersistence"/>: a field
/// or aggregate silently dropped when a model class gains/loses a property.
/// Run with `--verify-persistence`.
/// </summary>
public static class PersistenceRoundTripCheck
{
    public static bool Run(Action<string> log)
    {
        var dir = Path.Combine(Path.GetTempPath(), $"skynet-roundtrip-{Guid.NewGuid():N}");
        var steamPath = Path.Combine(dir, "steam.db");
        var dotaPath = Path.Combine(dir, "dota.db");
        try
        {
            Directory.CreateDirectory(dir);
            var steamOptions = new DbContextOptionsBuilder<SteamDbContext>()
                .UseSqlite($"Data Source={steamPath}").Options;
            var dotaOptions = new DbContextOptionsBuilder<DotaDbContext>()
                .UseSqlite($"Data Source={dotaPath}").Options;

            var original = BuildSampleState();
            using (var steam = new SteamDbContext(steamOptions))
            using (var dota = new DotaDbContext(dotaOptions))
            {
                steam.Database.EnsureCreated();
                dota.Database.EnsureCreated();
                DatabaseSchemaMaintenance.EnsureCurrent(steam, dota);
                StatePersistence.Save(steam, dota, original, includeCatalog: true);
                WriteLegacyJsonShapes(dota);
            }

            ApiState loaded;
            using (var steam = new SteamDbContext(steamOptions))
            using (var dota = new DotaDbContext(dotaOptions))
            {
                loaded = StatePersistence.Load(steam, dota);
            }

            return Compare(original, loaded, log) && VerifySchemaMaintenance(log);
        }
        finally
        {
            try { Directory.Delete(dir, recursive: true); } catch { /* best effort */ }
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

        var fileContent = new byte[] { 9, 8, 7 };
        var fileSha = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(fileContent)).ToLowerInvariant();
        var normalizedName = "a/save.dat";
        var fileKey = SteamApiStateService.MakeRemoteStorageKey(a, 570, normalizedName);
        state.Files[fileKey] = new ApiRemoteStorageFile
        {
            OwnerSteamId = a,
            AppId = 570,
            FileName = "a/save.dat",
            ContentBase64 = Convert.ToBase64String(fileContent),
            Size = fileContent.Length,
            Timestamp = 1234,
            Sha256 = fileSha,
            SyncPlatforms = 0xFFFFFFFFU,
            Version = 1,
            Persisted = true,
            DeletedAt = null
        };

        state.FileShares[80000000000000001UL] = new ApiRemoteStorageShareRecord
        {
            Handle = 80000000000000001UL,
            OwnerSteamId = a,
            AppId = 570,
            NormalizedName = normalizedName
        };

        state.DotaItems[101] = new ApiDotaItem { DefIndex = 101, Name = "Blade", Slot = "weapon", HeroNames = { "npc_dota_hero_axe" }, HeroIds = { 2 } };
        state.DotaItems[102] = new ApiDotaItem { DefIndex = 102, Name = "Terrain", Slot = "terrain", Prefab = "terrain" };
        state.DotaItems[103] = new ApiDotaItem { DefIndex = 103, Name = "Map", Slot = "map", Prefab = "map" };

        state.DotaEquipment[a] = new List<ApiDotaEquipment>
        {
            new() { SteamId = a, HeroId = 2, SlotId = 1, DefIndex = 101, ItemId = 555, Slot = "weapon", HeroName = "axe", UpdatedAt = DateTime.UtcNow },
            new() { SteamId = a, HeroId = 1000, SlotId = 14, DefIndex = 102, ItemId = 556, Slot = "terrain", HeroName = "hero_1000", UpdatedAt = DateTime.UtcNow },
            new() { SteamId = a, HeroId = 1000, SlotId = 15, DefIndex = 103, ItemId = 557, Slot = "map", HeroName = "hero_1000", UpdatedAt = DateTime.UtcNow },
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

    private static void WriteLegacyJsonShapes(DotaDbContext dota)
    {
        const ulong a = 76561197960287930UL;
        const ulong lobbyId = 90000000000000123UL;
        var updatedAt = DateTime.UtcNow.ToString("O", System.Globalization.CultureInfo.InvariantCulture);
        var membersJson = """
            [{"SteamId":"76561197960287930","Data":{"slot":"1"}}]
            """;
        var gameServerJson = """
            {"SteamId":"76561197960287930","IP":"168496141","Port":"27015"}
            """;
        var equipmentJson = $$"""
            [{"SteamId":"{{a}}","HeroId":"2","HeroName":"axe","Slot":"weapon","SlotId":"1","DefIndex":"101","ItemId":"555","Style":null,"UpdatedAt":"{{updatedAt}}","IgnoredByCurrentModel":"legacy"}]
            """;

        dota.Database.ExecuteSqlRaw("UPDATE Lobbies SET Members = {0}, GameServer = {1} WHERE SteamId = {2};", membersJson, gameServerJson, unchecked((long)lobbyId));
        dota.Database.ExecuteSqlRaw("UPDATE DotaMatchPlayers SET Equipment = {0} WHERE LobbyId = {1} AND SteamId = {2};", equipmentJson, unchecked((long)lobbyId), unchecked((long)a));
    }

    private static bool Compare(ApiState original, ApiState loaded, Action<string> log)
    {
        const ulong a = 76561197960287930UL;
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

        var expectedKey = SteamApiStateService.MakeRemoteStorageKey(a, 570, "a/save.dat");
        Check("RemoteFiles", loaded.Files.Count == 1 && loaded.Files[expectedKey].ContentBase64 == original.Files[expectedKey].ContentBase64);
        Check("FileShares", loaded.FileShares.Count == 1 && loaded.FileShares[80000000000000001UL].NormalizedName == "a/save.dat");
        Check("DotaItems", loaded.DotaItems.Count == 3 && loaded.DotaItems[101].HeroNames.Count == 1 && loaded.DotaItems[101].HeroIds.Contains(2u));
        Check("DotaEquipment", loaded.DotaEquipment.TryGetValue(76561197960287930UL, out var eq) &&
                               eq.Count == 3 &&
                               eq.Any(item => item.ItemId == 555 && item.HeroId == 2) &&
                               eq.Any(item => item.DefIndex == 102 && item.HeroId == 1000 && item.SlotId == 14) &&
                               eq.Any(item => item.DefIndex == 103 && item.HeroId == 1000 && item.SlotId == 15));

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

    private static bool VerifySchemaMaintenance(Action<string> log)
    {
        var dir = Path.Combine(Path.GetTempPath(), $"skynet-schema-{Guid.NewGuid():N}");
        try
        {
            Directory.CreateDirectory(dir);
            var steamOptions = new DbContextOptionsBuilder<SteamDbContext>()
                .UseSqlite($"Data Source={Path.Combine(dir, "steam.db")}").Options;
            var dotaOptions = new DbContextOptionsBuilder<DotaDbContext>()
                .UseSqlite($"Data Source={Path.Combine(dir, "dota.db")}").Options;

            using var steam = new SteamDbContext(steamOptions);
            using var dota = new DotaDbContext(dotaOptions);
            steam.Database.EnsureCreated();
            dota.Database.EnsureCreated();
            RecreateOldDotaEquipmentSchema(dota);
            DatabaseSchemaMaintenance.EnsureCurrent(steam, dota);

            var ok = GetPrimaryKeyColumns(dota, "DotaEquipment")
                .SequenceEqual(new[] { "SteamId", "HeroId", "SlotId", "DefIndex" }, StringComparer.OrdinalIgnoreCase);
            log(ok ? "Schema maintenance: PASS." : "Schema maintenance: FAIL.");
            return ok;
        }
        finally
        {
            try { Directory.Delete(dir, recursive: true); } catch { /* best effort */ }
        }
    }

    private static void RecreateOldDotaEquipmentSchema(DotaDbContext dota)
    {
        dota.Database.ExecuteSqlRaw("DROP TABLE DotaEquipment;");
        dota.Database.ExecuteSqlRaw("""
            CREATE TABLE DotaEquipment (
                SteamId INTEGER NOT NULL,
                HeroId INTEGER NOT NULL,
                SlotId INTEGER NOT NULL,
                HeroName TEXT NOT NULL,
                Slot TEXT NOT NULL,
                DefIndex INTEGER NOT NULL,
                ItemId INTEGER NOT NULL,
                Style INTEGER NOT NULL,
                UpdatedAt TEXT NOT NULL,
                CONSTRAINT PK_DotaEquipment PRIMARY KEY (SteamId, HeroId, SlotId)
            );
            """);
        dota.Database.ExecuteSqlRaw("""
            INSERT INTO DotaEquipment (SteamId, HeroId, SlotId, HeroName, Slot, DefIndex, ItemId, Style, UpdatedAt)
            VALUES (76561197960287930, 0, 0, 'global', 'terrain', 102, 556, 0, '2026-01-01T00:00:00Z');
            """);
    }

    private static List<string> GetPrimaryKeyColumns(DbContext context, string tableName)
    {
        var connection = (SqliteConnection)context.Database.GetDbConnection();
        var closeWhenDone = connection.State == System.Data.ConnectionState.Closed;
        if (closeWhenDone)
        {
            connection.Open();
        }

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info(\"{tableName.Replace("\"", "\"\"")}\");";
            using var reader = command.ExecuteReader();
            var columns = new List<(int Order, string Name)>();
            while (reader.Read())
            {
                var pkOrder = reader.GetInt32(reader.GetOrdinal("pk"));
                if (pkOrder > 0)
                {
                    columns.Add((pkOrder, reader.GetString(reader.GetOrdinal("name"))));
                }
            }

            return columns.OrderBy(column => column.Order).Select(column => column.Name).ToList();
        }
        finally
        {
            if (closeWhenDone)
            {
                connection.Close();
            }
        }
    }
}
