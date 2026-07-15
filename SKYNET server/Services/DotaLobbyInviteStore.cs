using Microsoft.Data.Sqlite;

namespace SKYNET_server.Services;

public sealed class DotaLobbyInviteStore
{
    private readonly object _sync = new();
    private readonly string _dbPath;

    public DotaLobbyInviteStore(string dbPath)
    {
        _dbPath = AppDatabase.PrepareDatabase(dbPath, path =>
        {
            using var connection = AppDatabase.OpenConnection(path);
            EnsureSchema(connection);
        });
    }

    public DotaLobbyInviteRecord Upsert(
        ulong lobbyId,
        ulong targetSteamId,
        ulong senderSteamId,
        string payloadJson,
        out DotaLobbyInviteRecord? replaced)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            replaced = ReadLocked(connection, lobbyId, targetSteamId, transaction);
            var inviteGid = NextIdLocked(connection, transaction);

            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = """
                INSERT INTO lobby_invites (
                    lobby_id, target_steam_id, invite_gid, sender_steam_id, payload_json, created_at
                ) VALUES (
                    $lobby_id, $target_steam_id, $invite_gid, $sender_steam_id, $payload_json, $created_at
                )
                ON CONFLICT(lobby_id, target_steam_id) DO UPDATE SET
                    invite_gid = excluded.invite_gid,
                    sender_steam_id = excluded.sender_steam_id,
                    payload_json = excluded.payload_json,
                    created_at = excluded.created_at
                """;
            Add(command, "$lobby_id", lobbyId);
            Add(command, "$target_steam_id", targetSteamId);
            Add(command, "$invite_gid", inviteGid);
            Add(command, "$sender_steam_id", senderSteamId);
            Add(command, "$payload_json", payloadJson ?? "{}");
            Add(command, "$created_at", Now());
            command.ExecuteNonQuery();
            transaction.Commit();
            return ReadLocked(connection, lobbyId, targetSteamId)
                ?? throw new InvalidOperationException("Lobby invite was not persisted.");
        }
    }

    public IReadOnlyList<DotaLobbyInviteRecord> GetForTarget(ulong targetSteamId, uint createdAtOrAfter)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            return ReadManyLocked(
                connection,
                "target_steam_id = $target_steam_id AND created_at >= $created_at",
                null,
                ("$target_steam_id", targetSteamId),
                ("$created_at", createdAtOrAfter));
        }
    }

    public DotaLobbyInviteRecord? Take(ulong lobbyId, ulong targetSteamId, uint createdAtOrAfter)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            var invite = ReadLocked(connection, lobbyId, targetSteamId, transaction);
            if (invite == null || invite.CreatedAt < createdAtOrAfter)
            {
                transaction.Commit();
                return null;
            }

            DeleteLocked(connection, transaction, lobbyId, targetSteamId);
            transaction.Commit();
            return invite;
        }
    }

    public IReadOnlyList<DotaLobbyInviteRecord> DeleteForTarget(ulong targetSteamId)
    {
        return DeleteMany("target_steam_id = $target_steam_id", ("$target_steam_id", targetSteamId));
    }

    public IReadOnlyList<DotaLobbyInviteRecord> DeleteForLobby(ulong lobbyId)
    {
        return DeleteMany("lobby_id = $lobby_id", ("$lobby_id", lobbyId));
    }

    public IReadOnlyList<DotaLobbyInviteRecord> PruneCreatedBefore(uint cutoff)
    {
        return DeleteMany("created_at < $cutoff", ("$cutoff", cutoff));
    }

    private IReadOnlyList<DotaLobbyInviteRecord> DeleteMany(
        string where,
        params (string Name, object Value)[] parameters)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            var invites = ReadManyLocked(connection, where, transaction, parameters);
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $"DELETE FROM lobby_invites WHERE {where}";
            foreach (var parameter in parameters)
            {
                Add(command, parameter.Name, parameter.Value);
            }

            command.ExecuteNonQuery();
            transaction.Commit();
            return invites;
        }
    }

    private SqliteConnection OpenConnection()
    {
        var connection = AppDatabase.OpenConnection(_dbPath);
        EnsureSchema(connection);
        return connection;
    }

    private static void EnsureSchema(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS lobby_invites (
                lobby_id INTEGER NOT NULL,
                target_steam_id INTEGER NOT NULL,
                invite_gid INTEGER NOT NULL,
                sender_steam_id INTEGER NOT NULL,
                payload_json TEXT NOT NULL,
                created_at INTEGER NOT NULL,
                PRIMARY KEY (lobby_id, target_steam_id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS idx_lobby_invites_gid
            ON lobby_invites (invite_gid);

            CREATE INDEX IF NOT EXISTS idx_lobby_invites_target
            ON lobby_invites (target_steam_id, created_at);
            """;
        command.ExecuteNonQuery();
    }

    private static DotaLobbyInviteRecord? ReadLocked(
        SqliteConnection connection,
        ulong lobbyId,
        ulong targetSteamId,
        SqliteTransaction? transaction = null)
    {
        return ReadManyLocked(
            connection,
            "lobby_id = $lobby_id AND target_steam_id = $target_steam_id",
            transaction,
            ("$lobby_id", lobbyId),
            ("$target_steam_id", targetSteamId)).FirstOrDefault();
    }

    private static IReadOnlyList<DotaLobbyInviteRecord> ReadManyLocked(
        SqliteConnection connection,
        string where,
        SqliteTransaction? transaction,
        params (string Name, object Value)[] parameters)
    {
        var invites = new List<DotaLobbyInviteRecord>();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = $"SELECT * FROM lobby_invites WHERE {where} ORDER BY created_at DESC, invite_gid DESC";
        foreach (var parameter in parameters)
        {
            Add(command, parameter.Name, parameter.Value);
        }

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            invites.Add(new DotaLobbyInviteRecord
            {
                LobbyId = U64(reader, "lobby_id"),
                TargetSteamId = U64(reader, "target_steam_id"),
                InviteGid = U64(reader, "invite_gid"),
                SenderSteamId = U64(reader, "sender_steam_id"),
                PayloadJson = Convert.ToString(reader["payload_json"]) ?? "{}",
                CreatedAt = Convert.ToUInt32(reader["created_at"])
            });
        }

        return invites;
    }

    private static void DeleteLocked(
        SqliteConnection connection,
        SqliteTransaction transaction,
        ulong lobbyId,
        ulong targetSteamId)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "DELETE FROM lobby_invites WHERE lobby_id = $lobby_id AND target_steam_id = $target_steam_id";
        Add(command, "$lobby_id", lobbyId);
        Add(command, "$target_steam_id", targetSteamId);
        command.ExecuteNonQuery();
    }

    private static ulong NextIdLocked(SqliteConnection connection, SqliteTransaction transaction)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT COALESCE(MAX(invite_gid), 0) FROM lobby_invites";
        var value = command.ExecuteScalar();
        var current = value == null || value == DBNull.Value ? 0UL : Convert.ToUInt64(value);
        var timeFloor = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000UL;
        return Math.Max(current + 1, timeFloor);
    }

    private static void Add(SqliteCommand command, string name, object? value)
    {
        command.Parameters.AddWithValue(name, value is ulong unsigned ? checked((long)unsigned) : value ?? DBNull.Value);
    }

    private static ulong U64(SqliteDataReader reader, string name) => Convert.ToUInt64(reader[name]);
    private static uint Now() => (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}

public sealed class DotaLobbyInviteRecord
{
    public ulong LobbyId { get; init; }
    public ulong TargetSteamId { get; init; }
    public ulong InviteGid { get; init; }
    public ulong SenderSteamId { get; init; }
    public string PayloadJson { get; init; } = "{}";
    public uint CreatedAt { get; init; }
}
