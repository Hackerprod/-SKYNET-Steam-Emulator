using Microsoft.Data.Sqlite;

namespace SKYNET_server.Services;

/// <summary>
/// Small Dota-owned key/team facade backed by dota.db. Profile, quest, match,
/// resource, sticker, party, guild, lobby, and stats tables are owned by the
/// dedicated Dota stores in the same database; Steam identity/friends remain in
/// steam.db and are exposed to the GC through SteamApiStateService providers.
/// </summary>
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
            CREATE TABLE IF NOT EXISTS KeyValues (
                Scope TEXT NOT NULL,
                Key TEXT NOT NULL,
                ValueJson TEXT NOT NULL,
                UpdatedAtUtc TEXT NOT NULL,
                PRIMARY KEY (Scope, Key)
            );
            """);
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
