using Microsoft.Data.Sqlite;

namespace SKYNET_server.Services;

public sealed class DotaDB : ScriptDatabase
{
    public DotaDB(IHostEnvironment environment, IConfiguration configuration)
        : base(environment, configuration, "dota.db")
    {
        InitializeDatabase();
    }

    protected override void Initialize()
    {
        using var connection = OpenConnection();
        Execute(connection, """
            CREATE TABLE IF NOT EXISTS Profiles (
                AccountId INTEGER PRIMARY KEY,
                SteamId TEXT NOT NULL DEFAULT '0',
                PersonaName TEXT NOT NULL DEFAULT '',
                ProfileJson TEXT NOT NULL DEFAULT '{}',
                CreatedAtUtc TEXT NOT NULL,
                UpdatedAtUtc TEXT NOT NULL
            );
            """);
        Execute(connection, """
            CREATE TABLE IF NOT EXISTS Teams (
                TeamId TEXT PRIMARY KEY,
                Name TEXT NOT NULL DEFAULT '',
                Tag TEXT NOT NULL DEFAULT '',
                TeamJson TEXT NOT NULL DEFAULT '{}',
                CreatedAtUtc TEXT NOT NULL,
                UpdatedAtUtc TEXT NOT NULL
            );
            """);
        Execute(connection, """
            CREATE TABLE IF NOT EXISTS TeamMembers (
                TeamId TEXT NOT NULL,
                AccountId INTEGER NOT NULL,
                Role INTEGER NOT NULL DEFAULT 0,
                UpdatedAtUtc TEXT NOT NULL,
                PRIMARY KEY (TeamId, AccountId)
            );
            """);
        Execute(connection, """
            CREATE TABLE IF NOT EXISTS QuestProgress (
                AccountId INTEGER NOT NULL,
                QuestId INTEGER NOT NULL,
                ProgressJson TEXT NOT NULL DEFAULT '{}',
                UpdatedAtUtc TEXT NOT NULL,
                PRIMARY KEY (AccountId, QuestId)
            );
            """);
        Execute(connection, """
            CREATE TABLE IF NOT EXISTS PeriodicResources (
                AccountId INTEGER NOT NULL,
                ResourceKey TEXT NOT NULL,
                MaxValue INTEGER NOT NULL DEFAULT 0,
                UsedValue INTEGER NOT NULL DEFAULT 0,
                ResourceJson TEXT NOT NULL DEFAULT '{}',
                UpdatedAtUtc TEXT NOT NULL,
                PRIMARY KEY (AccountId, ResourceKey)
            );
            """);
        Execute(connection, """
            CREATE TABLE IF NOT EXISTS HeroStickers (
                AccountId INTEGER NOT NULL,
                HeroId INTEGER NOT NULL DEFAULT 0,
                StickersJson TEXT NOT NULL DEFAULT '[]',
                UpdatedAtUtc TEXT NOT NULL,
                PRIMARY KEY (AccountId, HeroId)
            );
            """);
        Execute(connection, """
            CREATE TABLE IF NOT EXISTS Matches (
                MatchId TEXT PRIMARY KEY,
                LobbyId TEXT NOT NULL DEFAULT '0',
                OwnerAccountId INTEGER NOT NULL DEFAULT 0,
                MatchJson TEXT NOT NULL DEFAULT '{}',
                StartedAtUtc TEXT NOT NULL,
                UpdatedAtUtc TEXT NOT NULL
            );
            """);
        Execute(connection, """
            CREATE TABLE IF NOT EXISTS KeyValues (
                Scope TEXT NOT NULL,
                Key TEXT NOT NULL,
                ValueJson TEXT NOT NULL,
                UpdatedAtUtc TEXT NOT NULL,
                PRIMARY KEY (Scope, Key)
            );
            """);
    }

    internal string EnsureProfile(uint accountId, string steamId, string personaName)
    {
        if (accountId == 0)
        {
            return "{}";
        }

        var now = UtcNow();
        using var connection = OpenConnection();
        Execute(connection, """
            INSERT INTO Profiles (AccountId, SteamId, PersonaName, ProfileJson, CreatedAtUtc, UpdatedAtUtc)
            VALUES ($accountId, $steamId, $personaName, '{}', $now, $now)
            ON CONFLICT(AccountId) DO UPDATE SET
                SteamId = CASE WHEN excluded.SteamId != '0' THEN excluded.SteamId ELSE Profiles.SteamId END,
                PersonaName = CASE WHEN excluded.PersonaName != '' THEN excluded.PersonaName ELSE Profiles.PersonaName END,
                UpdatedAtUtc = excluded.UpdatedAtUtc;
            """,
            ("$accountId", accountId),
            ("$steamId", string.IsNullOrWhiteSpace(steamId) ? "0" : steamId),
            ("$personaName", personaName ?? string.Empty),
            ("$now", now));
        return GetProfile(accountId);
    }

    internal string GetProfile(uint accountId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT AccountId, SteamId, PersonaName, ProfileJson, CreatedAtUtc, UpdatedAtUtc
            FROM Profiles
            WHERE AccountId = $accountId;
            """;
        command.Parameters.AddWithValue("$accountId", accountId);
        using var reader = command.ExecuteReader();
        return reader.Read()
            ? Json(new
            {
                accountId = Convert.ToUInt32(reader.GetInt64(0)),
                steamId = reader.GetString(1),
                personaName = reader.GetString(2),
                profileJson = reader.GetString(3),
                createdAtUtc = reader.GetString(4),
                updatedAtUtc = reader.GetString(5)
            })
            : "{}";
    }

    internal string UpsertProfileJson(uint accountId, string steamId, string personaName, string profileJson)
    {
        if (accountId == 0)
        {
            return "{}";
        }

        var now = UtcNow();
        var normalized = NormalizeJson(profileJson, "{}");
        using var connection = OpenConnection();
        Execute(connection, """
            INSERT INTO Profiles (AccountId, SteamId, PersonaName, ProfileJson, CreatedAtUtc, UpdatedAtUtc)
            VALUES ($accountId, $steamId, $personaName, $profileJson, $now, $now)
            ON CONFLICT(AccountId) DO UPDATE SET
                SteamId = excluded.SteamId,
                PersonaName = excluded.PersonaName,
                ProfileJson = excluded.ProfileJson,
                UpdatedAtUtc = excluded.UpdatedAtUtc;
            """,
            ("$accountId", accountId),
            ("$steamId", string.IsNullOrWhiteSpace(steamId) ? "0" : steamId),
            ("$personaName", personaName ?? string.Empty),
            ("$profileJson", normalized),
            ("$now", now));
        return GetProfile(accountId);
    }

    internal string UpsertTeam(string teamId, string name, string tag, string teamJson)
    {
        if (string.IsNullOrWhiteSpace(teamId))
        {
            return "{}";
        }

        var now = UtcNow();
        var normalized = NormalizeJson(teamJson, "{}");
        using var connection = OpenConnection();
        Execute(connection, """
            INSERT INTO Teams (TeamId, Name, Tag, TeamJson, CreatedAtUtc, UpdatedAtUtc)
            VALUES ($teamId, $name, $tag, $teamJson, $now, $now)
            ON CONFLICT(TeamId) DO UPDATE SET
                Name = excluded.Name,
                Tag = excluded.Tag,
                TeamJson = excluded.TeamJson,
                UpdatedAtUtc = excluded.UpdatedAtUtc;
            """,
            ("$teamId", teamId),
            ("$name", name ?? string.Empty),
            ("$tag", tag ?? string.Empty),
            ("$teamJson", normalized),
            ("$now", now));
        return GetTeam(teamId);
    }

    internal string GetTeam(string teamId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT TeamId, Name, Tag, TeamJson, CreatedAtUtc, UpdatedAtUtc FROM Teams WHERE TeamId = $teamId;";
        command.Parameters.AddWithValue("$teamId", teamId ?? string.Empty);
        using var reader = command.ExecuteReader();
        return reader.Read()
            ? Json(new
            {
                teamId = reader.GetString(0),
                name = reader.GetString(1),
                tag = reader.GetString(2),
                teamJson = reader.GetString(3),
                createdAtUtc = reader.GetString(4),
                updatedAtUtc = reader.GetString(5)
            })
            : "{}";
    }

    internal bool AddTeamMember(string teamId, uint accountId, uint role)
    {
        if (string.IsNullOrWhiteSpace(teamId) || accountId == 0)
        {
            return false;
        }

        using var connection = OpenConnection();
        Execute(connection, """
            INSERT INTO TeamMembers (TeamId, AccountId, Role, UpdatedAtUtc)
            VALUES ($teamId, $accountId, $role, $now)
            ON CONFLICT(TeamId, AccountId) DO UPDATE SET
                Role = excluded.Role,
                UpdatedAtUtc = excluded.UpdatedAtUtc;
            """,
            ("$teamId", teamId),
            ("$accountId", accountId),
            ("$role", role),
            ("$now", UtcNow()));
        return true;
    }

    internal string GetTeamsForAccount(uint accountId)
    {
        var teams = new List<object>();
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT t.TeamId, t.Name, t.Tag, t.TeamJson, tm.Role
            FROM TeamMembers tm
            JOIN Teams t ON t.TeamId = tm.TeamId
            WHERE tm.AccountId = $accountId
            ORDER BY t.Name, t.TeamId;
            """;
        command.Parameters.AddWithValue("$accountId", accountId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            teams.Add(new
            {
                teamId = reader.GetString(0),
                name = reader.GetString(1),
                tag = reader.GetString(2),
                teamJson = reader.GetString(3),
                role = Convert.ToUInt32(reader.GetInt64(4))
            });
        }

        return Json(teams);
    }

    internal string SetQuestProgress(uint accountId, uint questId, string progressJson)
    {
        var normalized = NormalizeJson(progressJson, "{}");
        using var connection = OpenConnection();
        Execute(connection, """
            INSERT INTO QuestProgress (AccountId, QuestId, ProgressJson, UpdatedAtUtc)
            VALUES ($accountId, $questId, $progressJson, $now)
            ON CONFLICT(AccountId, QuestId) DO UPDATE SET
                ProgressJson = excluded.ProgressJson,
                UpdatedAtUtc = excluded.UpdatedAtUtc;
            """,
            ("$accountId", accountId),
            ("$questId", questId),
            ("$progressJson", normalized),
            ("$now", UtcNow()));
        return normalized;
    }

    internal string GetQuestProgress(uint accountId)
    {
        var rows = new List<object>();
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT QuestId, ProgressJson, UpdatedAtUtc FROM QuestProgress WHERE AccountId = $accountId ORDER BY QuestId;";
        command.Parameters.AddWithValue("$accountId", accountId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new
            {
                questId = Convert.ToUInt32(reader.GetInt64(0)),
                progressJson = reader.GetString(1),
                updatedAtUtc = reader.GetString(2)
            });
        }

        return Json(rows);
    }

    internal string SetPeriodicResource(uint accountId, string resourceKey, uint maxValue, uint usedValue, string resourceJson)
    {
        var normalized = NormalizeJson(resourceJson, "{}");
        using var connection = OpenConnection();
        Execute(connection, """
            INSERT INTO PeriodicResources (AccountId, ResourceKey, MaxValue, UsedValue, ResourceJson, UpdatedAtUtc)
            VALUES ($accountId, $resourceKey, $maxValue, $usedValue, $resourceJson, $now)
            ON CONFLICT(AccountId, ResourceKey) DO UPDATE SET
                MaxValue = excluded.MaxValue,
                UsedValue = excluded.UsedValue,
                ResourceJson = excluded.ResourceJson,
                UpdatedAtUtc = excluded.UpdatedAtUtc;
            """,
            ("$accountId", accountId),
            ("$resourceKey", string.IsNullOrWhiteSpace(resourceKey) ? "default" : resourceKey),
            ("$maxValue", maxValue),
            ("$usedValue", usedValue),
            ("$resourceJson", normalized),
            ("$now", UtcNow()));
        return GetPeriodicResource(accountId, resourceKey);
    }

    internal string GetPeriodicResource(uint accountId, string resourceKey)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT AccountId, ResourceKey, MaxValue, UsedValue, ResourceJson, UpdatedAtUtc
            FROM PeriodicResources
            WHERE AccountId = $accountId AND ResourceKey = $resourceKey;
            """;
        command.Parameters.AddWithValue("$accountId", accountId);
        command.Parameters.AddWithValue("$resourceKey", string.IsNullOrWhiteSpace(resourceKey) ? "default" : resourceKey);
        using var reader = command.ExecuteReader();
        return reader.Read()
            ? Json(new
            {
                accountId = Convert.ToUInt32(reader.GetInt64(0)),
                resourceKey = reader.GetString(1),
                maxValue = Convert.ToUInt32(reader.GetInt64(2)),
                usedValue = Convert.ToUInt32(reader.GetInt64(3)),
                resourceJson = reader.GetString(4),
                updatedAtUtc = reader.GetString(5)
            })
            : "{}";
    }

    internal string SetHeroStickers(uint accountId, uint heroId, string stickersJson)
    {
        var normalized = NormalizeJson(stickersJson, "[]");
        using var connection = OpenConnection();
        Execute(connection, """
            INSERT INTO HeroStickers (AccountId, HeroId, StickersJson, UpdatedAtUtc)
            VALUES ($accountId, $heroId, $stickersJson, $now)
            ON CONFLICT(AccountId, HeroId) DO UPDATE SET
                StickersJson = excluded.StickersJson,
                UpdatedAtUtc = excluded.UpdatedAtUtc;
            """,
            ("$accountId", accountId),
            ("$heroId", heroId),
            ("$stickersJson", normalized),
            ("$now", UtcNow()));
        return normalized;
    }

    internal string GetHeroStickers(uint accountId, uint heroId)
    {
        using var connection = OpenConnection();
        return Scalar(connection, "SELECT StickersJson FROM HeroStickers WHERE AccountId = $accountId AND HeroId = $heroId;",
            ("$accountId", accountId),
            ("$heroId", heroId))?.ToString() ?? "[]";
    }

    internal string UpsertMatch(string matchId, string lobbyId, uint ownerAccountId, string matchJson)
    {
        if (string.IsNullOrWhiteSpace(matchId))
        {
            return "{}";
        }

        var now = UtcNow();
        var normalized = NormalizeJson(matchJson, "{}");
        using var connection = OpenConnection();
        Execute(connection, """
            INSERT INTO Matches (MatchId, LobbyId, OwnerAccountId, MatchJson, StartedAtUtc, UpdatedAtUtc)
            VALUES ($matchId, $lobbyId, $ownerAccountId, $matchJson, $now, $now)
            ON CONFLICT(MatchId) DO UPDATE SET
                LobbyId = excluded.LobbyId,
                OwnerAccountId = excluded.OwnerAccountId,
                MatchJson = excluded.MatchJson,
                UpdatedAtUtc = excluded.UpdatedAtUtc;
            """,
            ("$matchId", matchId),
            ("$lobbyId", string.IsNullOrWhiteSpace(lobbyId) ? "0" : lobbyId),
            ("$ownerAccountId", ownerAccountId),
            ("$matchJson", normalized),
            ("$now", now));
        return GetMatch(matchId);
    }

    internal string GetMatch(string matchId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT MatchId, LobbyId, OwnerAccountId, MatchJson, StartedAtUtc, UpdatedAtUtc FROM Matches WHERE MatchId = $matchId;";
        command.Parameters.AddWithValue("$matchId", matchId ?? string.Empty);
        using var reader = command.ExecuteReader();
        return reader.Read()
            ? Json(new
            {
                matchId = reader.GetString(0),
                lobbyId = reader.GetString(1),
                ownerAccountId = Convert.ToUInt32(reader.GetInt64(2)),
                matchJson = reader.GetString(3),
                startedAtUtc = reader.GetString(4),
                updatedAtUtc = reader.GetString(5)
            })
            : "{}";
    }

    internal string GetRecentMatches(uint accountId, int limit)
    {
        var matches = new List<object>();
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT MatchId, LobbyId, OwnerAccountId, MatchJson, StartedAtUtc, UpdatedAtUtc
            FROM Matches
            WHERE $accountId = 0 OR OwnerAccountId = $accountId
            ORDER BY StartedAtUtc DESC
            LIMIT $limit;
            """;
        command.Parameters.AddWithValue("$accountId", accountId);
        command.Parameters.AddWithValue("$limit", Math.Clamp(limit, 1, 100));
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            matches.Add(new
            {
                matchId = reader.GetString(0),
                lobbyId = reader.GetString(1),
                ownerAccountId = Convert.ToUInt32(reader.GetInt64(2)),
                matchJson = reader.GetString(3),
                startedAtUtc = reader.GetString(4),
                updatedAtUtc = reader.GetString(5)
            });
        }

        return Json(matches);
    }

    internal string SetValue(string scope, string key, string valueJson)
    {
        var normalized = NormalizeJson(valueJson, "null");
        using var connection = OpenConnection();
        Execute(connection, """
            INSERT INTO KeyValues (Scope, Key, ValueJson, UpdatedAtUtc)
            VALUES ($scope, $key, $valueJson, $now)
            ON CONFLICT(Scope, Key) DO UPDATE SET
                ValueJson = excluded.ValueJson,
                UpdatedAtUtc = excluded.UpdatedAtUtc;
            """,
            ("$scope", scope ?? string.Empty),
            ("$key", key ?? string.Empty),
            ("$valueJson", normalized),
            ("$now", UtcNow()));
        return normalized;
    }

    internal string GetValue(string scope, string key)
    {
        using var connection = OpenConnection();
        return Scalar(connection, "SELECT ValueJson FROM KeyValues WHERE Scope = $scope AND Key = $key;",
            ("$scope", scope ?? string.Empty),
            ("$key", key ?? string.Empty))?.ToString() ?? "null";
    }
}
