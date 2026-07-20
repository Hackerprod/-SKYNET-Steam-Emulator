using Microsoft.Data.Sqlite;

namespace SKYNET_server.Services;

public sealed class DedicatedServerService : ScriptDatabase
{
    private readonly DotaDedicatedServerSupervisor _supervisor;
    private readonly ILogger<DedicatedServerService> _logger;

    public DedicatedServerService(
        IHostEnvironment environment,
        IConfiguration configuration,
        DotaDedicatedServerSupervisor supervisor,
        ILogger<DedicatedServerService> logger)
        : base(environment, configuration, "dedicated-server.db")
    {
        _supervisor = supervisor;
        _logger = logger;
        InitializeDatabase();
    }

    protected override void Initialize()
    {
        using var connection = OpenConnection();
        Execute(connection, """
            CREATE TABLE IF NOT EXISTS DedicatedServers (
                LobbyId TEXT PRIMARY KEY,
                Map TEXT NOT NULL DEFAULT 'dota',
                Port INTEGER NOT NULL DEFAULT 0,
                Started INTEGER NOT NULL DEFAULT 0,
                Status TEXT NOT NULL DEFAULT 'not_started',
                Error TEXT NOT NULL DEFAULT '',
                GameServerSteamId TEXT NOT NULL DEFAULT '0',
                UpdatedAtUtc TEXT NOT NULL
            );
            """);
        Execute(connection, """
            CREATE TABLE IF NOT EXISTS DedicatedServerEvents (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                LobbyId TEXT NOT NULL DEFAULT '0',
                EventType TEXT NOT NULL,
                DataJson TEXT NOT NULL DEFAULT '{}',
                CreatedAtUtc TEXT NOT NULL
            );
            """);
        Execute(connection, """
            CREATE INDEX IF NOT EXISTS IX_DedicatedServerEvents_LobbyId_Id
            ON DedicatedServerEvents (LobbyId, Id);
            """);
    }

    internal string Start(string lobbyId, string map)
    {
        var parsedLobbyId = ParseUInt64(lobbyId);
        var result = _supervisor.Start(parsedLobbyId, map);
        var status = result.Started ? result.State.ToString().ToLowerInvariant() : "failed";
        var port = result.Port <= 0 ? 0u : (uint)result.Port;
        Upsert(lobbyId, map, port, result.Started, status, result.Error, "0");
        RecordEvent(lobbyId, "start", Json(new
        {
            lobbyId,
            map = string.IsNullOrWhiteSpace(map) ? "dota" : map,
            started = result.Started,
            port,
            status,
            error = result.Error ?? string.Empty
        }));
        return GetStatus(lobbyId);
    }

    internal string Claim(string gameServerSteamId, uint reportedPort)
    {
        var lobbyId = _supervisor.ClaimLobby(ParseUInt64(gameServerSteamId), reportedPort);
        if (lobbyId != "0")
        {
            using var connection = OpenConnection();
            Execute(connection, """
                UPDATE DedicatedServers
                SET GameServerSteamId = $gameServerSteamId,
                    Port = CASE WHEN $reportedPort != 0 THEN $reportedPort ELSE Port END,
                    Status = 'claimed',
                    UpdatedAtUtc = $now
                WHERE LobbyId = $lobbyId;
                """,
                ("$gameServerSteamId", string.IsNullOrWhiteSpace(gameServerSteamId) ? "0" : gameServerSteamId),
                ("$reportedPort", reportedPort),
                ("$now", UtcNow()),
                ("$lobbyId", lobbyId));
        }

        RecordEvent(lobbyId, lobbyId == "0" ? "claim_rejected" : "claim", Json(new
        {
            lobbyId,
            gameServerSteamId,
            reportedPort
        }));
        return Json(new { lobbyId, gameServerSteamId, reportedPort, claimed = lobbyId != "0" });
    }

    internal bool HasReservationForPort(uint port)
    {
        return _supervisor.HasReservationForPort(port);
    }

    internal string GetStatus(string lobbyId)
    {
        var supervisorStatus = _supervisor.GetStatus(ParseUInt64(lobbyId));
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT LobbyId, Map, Port, Started, Status, Error, GameServerSteamId, UpdatedAtUtc
            FROM DedicatedServers
            WHERE LobbyId = $lobbyId;
            """;
        command.Parameters.AddWithValue("$lobbyId", lobbyId ?? string.Empty);
        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return Json(new
            {
                lobbyId = lobbyId ?? "0",
                status = supervisorStatus,
                found = false
            });
        }

        return Json(new
        {
            lobbyId = reader.GetString(0),
            map = reader.GetString(1),
            port = Convert.ToUInt32(reader.GetInt64(2)),
            started = reader.GetInt64(3) != 0,
            status = supervisorStatus == "not_found" ? reader.GetString(4) : supervisorStatus,
            recordedStatus = reader.GetString(4),
            error = reader.GetString(5),
            gameServerSteamId = reader.GetString(6),
            updatedAtUtc = reader.GetString(7),
            found = true
        });
    }

    internal bool Release(string lobbyId, string reason)
    {
        var released = _supervisor.Release(ParseUInt64(lobbyId), reason ?? string.Empty);
        using var connection = OpenConnection();
        Execute(connection, """
            UPDATE DedicatedServers
            SET Status = $status,
                UpdatedAtUtc = $now
            WHERE LobbyId = $lobbyId;
            """,
            ("$status", released ? "released" : "release_missing"),
            ("$now", UtcNow()),
            ("$lobbyId", lobbyId ?? string.Empty));
        RecordEvent(lobbyId ?? "0", released ? "release" : "release_missing", Json(new { lobbyId = lobbyId ?? "0", reason = reason ?? string.Empty }));
        return released;
    }

    internal string History(string lobbyId, int limit)
    {
        var events = new List<object>();
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, LobbyId, EventType, DataJson, CreatedAtUtc
            FROM DedicatedServerEvents
            WHERE $lobbyId = '' OR LobbyId = $lobbyId
            ORDER BY Id DESC
            LIMIT $limit;
            """;
        command.Parameters.AddWithValue("$lobbyId", lobbyId ?? string.Empty);
        command.Parameters.AddWithValue("$limit", Math.Clamp(limit, 1, 200));
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            events.Add(new
            {
                id = reader.GetInt64(0),
                lobbyId = reader.GetString(1),
                eventType = reader.GetString(2),
                dataJson = reader.GetString(3),
                createdAtUtc = reader.GetString(4)
            });
        }

        return Json(events);
    }

    private void Upsert(string lobbyId, string map, uint port, bool started, string status, string error, string gameServerSteamId)
    {
        using var connection = OpenConnection();
        Execute(connection, """
            INSERT INTO DedicatedServers (LobbyId, Map, Port, Started, Status, Error, GameServerSteamId, UpdatedAtUtc)
            VALUES ($lobbyId, $map, $port, $started, $status, $error, $gameServerSteamId, $now)
            ON CONFLICT(LobbyId) DO UPDATE SET
                Map = excluded.Map,
                Port = excluded.Port,
                Started = excluded.Started,
                Status = excluded.Status,
                Error = excluded.Error,
                GameServerSteamId = excluded.GameServerSteamId,
                UpdatedAtUtc = excluded.UpdatedAtUtc;
            """,
            ("$lobbyId", lobbyId ?? "0"),
            ("$map", string.IsNullOrWhiteSpace(map) ? "dota" : map),
            ("$port", port),
            ("$started", started ? 1 : 0),
            ("$status", status ?? string.Empty),
            ("$error", error ?? string.Empty),
            ("$gameServerSteamId", string.IsNullOrWhiteSpace(gameServerSteamId) ? "0" : gameServerSteamId),
            ("$now", UtcNow()));
    }

    private void RecordEvent(string lobbyId, string eventType, string dataJson)
    {
        try
        {
            using var connection = OpenConnection();
            Execute(connection, """
                INSERT INTO DedicatedServerEvents (LobbyId, EventType, DataJson, CreatedAtUtc)
                VALUES ($lobbyId, $eventType, $dataJson, $now);
                """,
                ("$lobbyId", string.IsNullOrWhiteSpace(lobbyId) ? "0" : lobbyId),
                ("$eventType", eventType ?? string.Empty),
                ("$dataJson", NormalizeJson(dataJson, "{}")),
                ("$now", UtcNow()));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not record dedicated server event {EventType} for lobby {LobbyId}", eventType, lobbyId);
        }
    }
}
