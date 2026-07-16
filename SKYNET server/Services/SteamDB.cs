using Microsoft.Data.Sqlite;

namespace SKYNET_server.Services;

public sealed class SteamDB : ScriptDatabase
{
    public SteamDB(IHostEnvironment environment, IConfiguration configuration)
        : base(environment, configuration, "steam.db")
    {
        InitializeDatabase();
    }

    public LuaSteamDB ForLua(GameCoordinatorContext context)
    {
        return new LuaSteamDB(this, context);
    }

    protected override void Initialize()
    {
        using var connection = OpenConnection();
        Execute(connection, """
            CREATE TABLE IF NOT EXISTS Users (
                SteamId TEXT PRIMARY KEY,
                AccountId INTEGER NOT NULL UNIQUE,
                AppId INTEGER NOT NULL,
                PersonaName TEXT NOT NULL,
                PlayerLevel INTEGER NOT NULL DEFAULT 1,
                CreatedAtUtc TEXT NOT NULL,
                UpdatedAtUtc TEXT NOT NULL
            );
            """);
        Execute(connection, """
            CREATE TABLE IF NOT EXISTS Sessions (
                SteamId TEXT NOT NULL,
                ClientInstanceId TEXT NOT NULL,
                ProcessRole TEXT NOT NULL,
                ClientIp TEXT NOT NULL,
                LastSeenUtc TEXT NOT NULL,
                PRIMARY KEY (SteamId, ClientInstanceId, ProcessRole)
            );
            """);
        Execute(connection, """
            CREATE TABLE IF NOT EXISTS Presence (
                SteamId TEXT PRIMARY KEY,
                PersonaState INTEGER NOT NULL DEFAULT 1,
                GameState TEXT NOT NULL DEFAULT 'menu',
                HeroId INTEGER NOT NULL DEFAULT 0,
                RichPresenceJson TEXT NOT NULL DEFAULT '{}',
                UpdatedAtUtc TEXT NOT NULL
            );
            """);
        Execute(connection, """
            CREATE TABLE IF NOT EXISTS Friends (
                SteamId TEXT NOT NULL,
                FriendSteamId TEXT NOT NULL,
                CreatedAtUtc TEXT NOT NULL,
                PRIMARY KEY (SteamId, FriendSteamId)
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

    internal string UpsertUser(string steamId, uint accountId, uint appId, string personaName, uint playerLevel = 1)
    {
        var now = UtcNow();
        using var connection = OpenConnection();
        Execute(connection, """
            INSERT INTO Users (SteamId, AccountId, AppId, PersonaName, PlayerLevel, CreatedAtUtc, UpdatedAtUtc)
            VALUES ($steamId, $accountId, $appId, $personaName, $playerLevel, $now, $now)
            ON CONFLICT(SteamId) DO UPDATE SET
                AccountId = excluded.AccountId,
                AppId = excluded.AppId,
                PersonaName = excluded.PersonaName,
                PlayerLevel = excluded.PlayerLevel,
                UpdatedAtUtc = excluded.UpdatedAtUtc;
            """,
            ("$steamId", steamId),
            ("$accountId", accountId),
            ("$appId", appId),
            ("$personaName", personaName ?? string.Empty),
            ("$playerLevel", playerLevel == 0 ? 1 : playerLevel),
            ("$now", now));
        return GetUser(steamId);
    }

    internal string TouchSession(string steamId, string clientInstanceId, string processRole, string clientIp)
    {
        var now = UtcNow();
        using var connection = OpenConnection();
        Execute(connection, """
            INSERT INTO Sessions (SteamId, ClientInstanceId, ProcessRole, ClientIp, LastSeenUtc)
            VALUES ($steamId, $clientInstanceId, $processRole, $clientIp, $now)
            ON CONFLICT(SteamId, ClientInstanceId, ProcessRole) DO UPDATE SET
                ClientIp = excluded.ClientIp,
                LastSeenUtc = excluded.LastSeenUtc;
            """,
            ("$steamId", steamId),
            ("$clientInstanceId", clientInstanceId ?? string.Empty),
            ("$processRole", processRole ?? string.Empty),
            ("$clientIp", clientIp ?? string.Empty),
            ("$now", now));
        return Json(new { steamId, clientInstanceId, processRole, clientIp, lastSeenUtc = now });
    }

    internal string GetUser(string steamId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT SteamId, AccountId, AppId, PersonaName, PlayerLevel, CreatedAtUtc, UpdatedAtUtc
            FROM Users
            WHERE SteamId = $steamId;
            """;
        command.Parameters.AddWithValue("$steamId", steamId ?? string.Empty);
        using var reader = command.ExecuteReader();
        return reader.Read()
            ? Json(new
            {
                steamId = reader.GetString(0),
                accountId = Convert.ToUInt32(reader.GetInt64(1)),
                appId = Convert.ToUInt32(reader.GetInt64(2)),
                personaName = reader.GetString(3),
                playerLevel = Convert.ToUInt32(reader.GetInt64(4)),
                createdAtUtc = reader.GetString(5),
                updatedAtUtc = reader.GetString(6)
            })
            : "{}";
    }

    internal string GetUserByAccountId(uint accountId)
    {
        using var connection = OpenConnection();
        var steamId = Scalar(connection, "SELECT SteamId FROM Users WHERE AccountId = $accountId;", ("$accountId", accountId))?.ToString();
        return string.IsNullOrWhiteSpace(steamId) ? "{}" : GetUser(steamId);
    }

    internal bool UserExists(string steamId)
    {
        using var connection = OpenConnection();
        return Convert.ToInt64(Scalar(connection, "SELECT COUNT(1) FROM Users WHERE SteamId = $steamId;", ("$steamId", steamId ?? string.Empty)) ?? 0L) > 0;
    }

    internal string SetPresence(string steamId, int personaState, string gameState, uint heroId, string richPresenceJson)
    {
        var now = UtcNow();
        var normalized = NormalizeJson(richPresenceJson, "{}");
        using var connection = OpenConnection();
        Execute(connection, """
            INSERT INTO Presence (SteamId, PersonaState, GameState, HeroId, RichPresenceJson, UpdatedAtUtc)
            VALUES ($steamId, $personaState, $gameState, $heroId, $richPresenceJson, $now)
            ON CONFLICT(SteamId) DO UPDATE SET
                PersonaState = excluded.PersonaState,
                GameState = excluded.GameState,
                HeroId = excluded.HeroId,
                RichPresenceJson = excluded.RichPresenceJson,
                UpdatedAtUtc = excluded.UpdatedAtUtc;
            """,
            ("$steamId", steamId),
            ("$personaState", personaState),
            ("$gameState", string.IsNullOrWhiteSpace(gameState) ? "menu" : gameState),
            ("$heroId", heroId),
            ("$richPresenceJson", normalized),
            ("$now", now));
        return GetPresence(steamId);
    }

    internal string GetPresence(string steamId)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT SteamId, PersonaState, GameState, HeroId, RichPresenceJson, UpdatedAtUtc
            FROM Presence
            WHERE SteamId = $steamId;
            """;
        command.Parameters.AddWithValue("$steamId", steamId ?? string.Empty);
        using var reader = command.ExecuteReader();
        return reader.Read()
            ? Json(new
            {
                steamId = reader.GetString(0),
                personaState = reader.GetInt32(1),
                gameState = reader.GetString(2),
                heroId = Convert.ToUInt32(reader.GetInt64(3)),
                richPresenceJson = reader.GetString(4),
                updatedAtUtc = reader.GetString(5)
            })
            : "{}";
    }

    internal bool AddFriend(string steamId, string friendSteamId)
    {
        if (string.IsNullOrWhiteSpace(steamId) || string.IsNullOrWhiteSpace(friendSteamId))
        {
            return false;
        }

        var now = UtcNow();
        using var connection = OpenConnection();
        Execute(connection, "INSERT OR IGNORE INTO Friends (SteamId, FriendSteamId, CreatedAtUtc) VALUES ($steamId, $friendSteamId, $now);",
            ("$steamId", steamId),
            ("$friendSteamId", friendSteamId),
            ("$now", now));
        Execute(connection, "INSERT OR IGNORE INTO Friends (SteamId, FriendSteamId, CreatedAtUtc) VALUES ($steamId, $friendSteamId, $now);",
            ("$steamId", friendSteamId),
            ("$friendSteamId", steamId),
            ("$now", now));
        return true;
    }

    internal string GetFriends(string steamId)
    {
        var friends = new List<object>();
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT f.FriendSteamId, u.AccountId, u.PersonaName, u.AppId
            FROM Friends f
            LEFT JOIN Users u ON u.SteamId = f.FriendSteamId
            WHERE f.SteamId = $steamId
            ORDER BY u.PersonaName, f.FriendSteamId;
            """;
        command.Parameters.AddWithValue("$steamId", steamId ?? string.Empty);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            friends.Add(new
            {
                steamId = reader.GetString(0),
                accountId = reader.IsDBNull(1) ? 0u : Convert.ToUInt32(reader.GetInt64(1)),
                personaName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                appId = reader.IsDBNull(3) ? 0u : Convert.ToUInt32(reader.GetInt64(3))
            });
        }

        return Json(friends);
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

public sealed class LuaSteamDB
{
    private readonly SteamDB _db;
    private readonly GameCoordinatorContext _context;

    public LuaSteamDB(SteamDB db, GameCoordinatorContext context)
    {
        _db = db;
        _context = context;
    }

    public string Path => _db.DatabasePath;

    public string CurrentUserJson() => _db.UpsertUser(SteamId(), _context.AccountId, _context.AppId, _context.PersonaName);
    public string EnsureCurrentUser() => CurrentUserJson();
    public string UpsertUser(string steamId, uint accountId, uint appId, string personaName) => _db.UpsertUser(steamId, accountId, appId, personaName);
    public string GetUser(string steamId) => _db.GetUser(steamId);
    public string GetUserByAccountId(uint accountId) => _db.GetUserByAccountId(accountId);
    public bool UserExists(string steamId) => _db.UserExists(steamId);
    public string TouchCurrentSession(string clientInstanceId = "", string processRole = "client") =>
        _db.TouchSession(SteamId(), clientInstanceId, processRole, _context.ClientIp);
    public string GetPresence(string steamId) => _db.GetPresence(steamId);
    public string SetPresence(string steamId, int personaState, string gameState, uint heroId, string richPresenceJson) =>
        _db.SetPresence(steamId, personaState, gameState, heroId, richPresenceJson);
    public bool AddFriend(string steamId, string friendSteamId) => _db.AddFriend(steamId, friendSteamId);
    public string GetFriends(string steamId) => _db.GetFriends(steamId);
    public string SetValue(string scope, string key, string valueJson) => _db.SetValue(scope, key, valueJson);
    public string GetValue(string scope, string key) => _db.GetValue(scope, key);

    private string SteamId() => _context.SteamId.ToString(System.Globalization.CultureInfo.InvariantCulture);
}
