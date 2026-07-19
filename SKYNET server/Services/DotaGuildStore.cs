using Microsoft.Data.Sqlite;

namespace SKYNET_server.Services;

public sealed class DotaGuildStore
{
    private const uint DefaultLeaderRoleId = 1;
    private const uint DefaultMemberRoleId = 2;

    private readonly object _sync = new();
    private readonly string _dbPath;
    private readonly Func<uint, DotaStatsAccountIdentity?> _identityResolver;

    public DotaGuildStore(string dbPath, Func<uint, DotaStatsAccountIdentity?> identityResolver)
    {
        _identityResolver = identityResolver;
        _dbPath = AppDatabase.PrepareDatabase(dbPath, path =>
        {
            using var connection = AppDatabase.OpenConnection(path);
            EnsureSchema(connection);
        });
    }

    public DotaGuildSnapshot EnsureCurrentMembership(ulong steamId, uint accountId, string personaName)
    {
        if (accountId == 0)
        {
            throw new ArgumentException("Account id must be non-zero.", nameof(accountId));
        }

        lock (_sync)
        {
            using var connection = OpenConnection();
            var existing = ReadGuildByMemberLocked(connection, accountId);
            if (existing != null)
            {
                TouchMemberLocked(connection, null, existing.GuildId, accountId);
                return ReadGuildLocked(connection, existing.GuildId) ?? existing;
            }

            using var transaction = connection.BeginTransaction();
            var guildId = NextIdLocked(connection, transaction, "dota_guilds", "guild_id", 100000);
            var now = Now();
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = """
                    INSERT INTO dota_guilds (
                        guild_id, guild_name, guild_tag, created_at, guild_language, guild_flags, guild_logo,
                        guild_region, guild_chat_group_id, guild_description, default_chat_channel_id,
                        guild_primary_color, guild_secondary_color, guild_pattern, guild_refresh_time_offset,
                        guild_required_rank_tier, guild_motd_timestamp, guild_motd, updated_at
                    )
                    VALUES (
                        $guild_id, $guild_name, $guild_tag, $created_at, 0, 0, 0,
                        0, 0, $guild_description, 0,
                        4618341, 2184278, 0, 0,
                        0, $guild_motd_timestamp, $guild_motd, $updated_at
                    )
                    """;
                Add(command, "$guild_id", guildId);
                Add(command, "$guild_name", BuildGuildName(personaName, accountId));
                Add(command, "$guild_tag", BuildGuildTag(personaName, accountId));
                Add(command, "$created_at", now);
                Add(command, "$guild_description", $"Guild for {NormalizePersona(personaName, accountId)}");
                Add(command, "$guild_motd_timestamp", now);
                Add(command, "$guild_motd", string.Empty);
                Add(command, "$updated_at", now);
                command.ExecuteNonQuery();
            }

            UpsertRoleLocked(connection, transaction, guildId, DefaultLeaderRoleId, "Leader", 255, 0);
            UpsertRoleLocked(connection, transaction, guildId, DefaultMemberRoleId, "Member", 0, 1);
            UpsertMemberLocked(connection, transaction, guildId, accountId, DefaultLeaderRoleId, now, now);
            transaction.Commit();
            return ReadGuildLocked(connection, guildId) ?? throw new InvalidOperationException("Guild was not created.");
        }
    }

    public DotaGuildMembershipSnapshot GetMembership(uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            return new DotaGuildMembershipSnapshot
            {
                GuildIds = ReadGuildIdsByMemberLocked(connection, accountId),
                Invites = ReadAccountInvitesLocked(connection, accountId)
            };
        }
    }

    public DotaGuildSnapshot? GetGuild(uint guildId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            return ReadGuildLocked(connection, guildId);
        }
    }

    public List<DotaGuildPersonaSnapshot> GetPersonaInfo(uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            return ReadPersonaInfoLocked(connection, accountId);
        }
    }

    public DotaGuildEventDataSnapshot GetEventData(uint guildId, uint eventId, uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            var isMember = ScalarLong(
                connection,
                null,
                "SELECT COUNT(*) FROM dota_guild_members WHERE guild_id = $guild_id AND account_id = $account_id",
                ("$guild_id", guildId),
                ("$account_id", accountId)) > 0;

            return new DotaGuildEventDataSnapshot
            {
                GuildId = guildId,
                EventId = eventId,
                IsMember = isMember,
                ContractsRefreshedTimestamp = 0,
                ChallengesRefreshTimestamp = 0
            };
        }
    }

    private DotaGuildSnapshot? ReadGuildByMemberLocked(SqliteConnection connection, uint accountId)
    {
        var guildId = ScalarU32(
            connection,
            null,
            """
            SELECT guild_id
            FROM dota_guild_members
            WHERE account_id = $account_id
            ORDER BY joined_at ASC, guild_id ASC
            LIMIT 1
            """,
            ("$account_id", accountId));
        return guildId == 0 ? null : ReadGuildLocked(connection, guildId);
    }

    private DotaGuildSnapshot? ReadGuildLocked(SqliteConnection connection, uint guildId)
    {
        DotaGuildInfoSnapshot? info = null;
        using (var command = connection.CreateCommand())
        {
            command.CommandText = """
                SELECT guild_name, guild_tag, created_at, guild_language, guild_flags, guild_logo,
                       guild_region, guild_chat_group_id, guild_description, default_chat_channel_id,
                       guild_primary_color, guild_secondary_color, guild_pattern, guild_refresh_time_offset,
                       guild_required_rank_tier, guild_motd_timestamp, guild_motd
                FROM dota_guilds
                WHERE guild_id = $guild_id
                """;
            Add(command, "$guild_id", guildId);
            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            info = new DotaGuildInfoSnapshot
            {
                GuildName = reader.GetString(0),
                GuildTag = reader.GetString(1),
                CreatedTimestamp = U32(reader.GetInt64(2)),
                GuildLanguage = U32(reader.GetInt64(3)),
                GuildFlags = U32(reader.GetInt64(4)),
                GuildLogo = U64(reader.GetInt64(5)),
                GuildRegion = U32(reader.GetInt64(6)),
                GuildChatGroupId = U64(reader.GetInt64(7)),
                GuildDescription = reader.GetString(8),
                DefaultChatChannelId = U64(reader.GetInt64(9)),
                GuildPrimaryColor = U32(reader.GetInt64(10)),
                GuildSecondaryColor = U32(reader.GetInt64(11)),
                GuildPattern = U32(reader.GetInt64(12)),
                GuildRefreshTimeOffset = U32(reader.GetInt64(13)),
                GuildRequiredRankTier = U32(reader.GetInt64(14)),
                GuildMotdTimestamp = U32(reader.GetInt64(15)),
                GuildMotd = reader.GetString(16)
            };
        }

        return new DotaGuildSnapshot
        {
            GuildId = guildId,
            Info = info,
            Roles = ReadRolesLocked(connection, guildId),
            Members = ReadMembersLocked(connection, guildId),
            Invites = ReadGuildInvitesLocked(connection, guildId)
        };
    }

    private List<uint> ReadGuildIdsByMemberLocked(SqliteConnection connection, uint accountId)
    {
        var result = new List<uint>();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT guild_id
            FROM dota_guild_members
            WHERE account_id = $account_id
            ORDER BY joined_at ASC, guild_id ASC
            """;
        Add(command, "$account_id", accountId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(U32(reader.GetInt64(0)));
        }

        return result;
    }

    private List<DotaGuildRoleSnapshot> ReadRolesLocked(SqliteConnection connection, uint guildId)
    {
        var result = new List<DotaGuildRoleSnapshot>();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT role_id, role_name, role_flags, role_order
            FROM dota_guild_roles
            WHERE guild_id = $guild_id
            ORDER BY role_order ASC, role_id ASC
            """;
        Add(command, "$guild_id", guildId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new DotaGuildRoleSnapshot
            {
                RoleId = U32(reader.GetInt64(0)),
                RoleName = reader.GetString(1),
                RoleFlags = U32(reader.GetInt64(2)),
                RoleOrder = U32(reader.GetInt64(3))
            });
        }

        return result;
    }

    private List<DotaGuildMemberSnapshot> ReadMembersLocked(SqliteConnection connection, uint guildId)
    {
        var result = new List<DotaGuildMemberSnapshot>();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT account_id, role_id, joined_at, last_active_at
            FROM dota_guild_members
            WHERE guild_id = $guild_id
            ORDER BY joined_at ASC, account_id ASC
            """;
        Add(command, "$guild_id", guildId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new DotaGuildMemberSnapshot
            {
                AccountId = U32(reader.GetInt64(0)),
                RoleId = U32(reader.GetInt64(1)),
                JoinedTimestamp = U32(reader.GetInt64(2)),
                LastActiveTimestamp = U32(reader.GetInt64(3))
            });
        }

        return result;
    }

    private List<DotaGuildInviteSnapshot> ReadGuildInvitesLocked(SqliteConnection connection, uint guildId)
    {
        var result = new List<DotaGuildInviteSnapshot>();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT requester_account_id, target_account_id, timestamp_sent
            FROM dota_guild_invites
            WHERE guild_id = $guild_id
            ORDER BY timestamp_sent DESC
            """;
        Add(command, "$guild_id", guildId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new DotaGuildInviteSnapshot
            {
                GuildId = guildId,
                RequesterAccountId = U32(reader.GetInt64(0)),
                TargetAccountId = U32(reader.GetInt64(1)),
                TimestampSent = U32(reader.GetInt64(2))
            });
        }

        return result;
    }

    private List<DotaGuildInviteSnapshot> ReadAccountInvitesLocked(SqliteConnection connection, uint accountId)
    {
        var result = new List<DotaGuildInviteSnapshot>();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT guild_id, requester_account_id, target_account_id, timestamp_sent
            FROM dota_guild_invites
            WHERE target_account_id = $account_id
            ORDER BY timestamp_sent DESC
            """;
        Add(command, "$account_id", accountId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new DotaGuildInviteSnapshot
            {
                GuildId = U32(reader.GetInt64(0)),
                RequesterAccountId = U32(reader.GetInt64(1)),
                TargetAccountId = U32(reader.GetInt64(2)),
                TimestampSent = U32(reader.GetInt64(3))
            });
        }

        return result;
    }

    private List<DotaGuildPersonaSnapshot> ReadPersonaInfoLocked(SqliteConnection connection, uint accountId)
    {
        var result = new List<DotaGuildPersonaSnapshot>();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT g.guild_id, g.guild_tag, g.guild_flags
            FROM dota_guild_members m
            INNER JOIN dota_guilds g ON g.guild_id = m.guild_id
            WHERE m.account_id = $account_id
            ORDER BY m.joined_at ASC, g.guild_id ASC
            """;
        Add(command, "$account_id", accountId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new DotaGuildPersonaSnapshot
            {
                GuildId = U32(reader.GetInt64(0)),
                GuildTag = reader.GetString(1),
                GuildFlags = U32(reader.GetInt64(2))
            });
        }

        return result;
    }

    private static void EnsureSchema(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS dota_guilds (
                guild_id INTEGER PRIMARY KEY,
                guild_name TEXT NOT NULL,
                guild_tag TEXT NOT NULL,
                created_at INTEGER NOT NULL,
                guild_language INTEGER NOT NULL DEFAULT 0,
                guild_flags INTEGER NOT NULL DEFAULT 0,
                guild_logo INTEGER NOT NULL DEFAULT 0,
                guild_region INTEGER NOT NULL DEFAULT 0,
                guild_chat_group_id INTEGER NOT NULL DEFAULT 0,
                guild_description TEXT NOT NULL DEFAULT '',
                default_chat_channel_id INTEGER NOT NULL DEFAULT 0,
                guild_primary_color INTEGER NOT NULL DEFAULT 0,
                guild_secondary_color INTEGER NOT NULL DEFAULT 0,
                guild_pattern INTEGER NOT NULL DEFAULT 0,
                guild_refresh_time_offset INTEGER NOT NULL DEFAULT 0,
                guild_required_rank_tier INTEGER NOT NULL DEFAULT 0,
                guild_motd_timestamp INTEGER NOT NULL DEFAULT 0,
                guild_motd TEXT NOT NULL DEFAULT '',
                updated_at INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS dota_guild_roles (
                guild_id INTEGER NOT NULL,
                role_id INTEGER NOT NULL,
                role_name TEXT NOT NULL,
                role_flags INTEGER NOT NULL,
                role_order INTEGER NOT NULL,
                PRIMARY KEY (guild_id, role_id),
                FOREIGN KEY (guild_id) REFERENCES dota_guilds(guild_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS dota_guild_members (
                guild_id INTEGER NOT NULL,
                account_id INTEGER NOT NULL,
                role_id INTEGER NOT NULL,
                joined_at INTEGER NOT NULL,
                last_active_at INTEGER NOT NULL,
                PRIMARY KEY (guild_id, account_id),
                FOREIGN KEY (guild_id) REFERENCES dota_guilds(guild_id) ON DELETE CASCADE,
                FOREIGN KEY (guild_id, role_id) REFERENCES dota_guild_roles(guild_id, role_id) ON DELETE RESTRICT
            );

            CREATE INDEX IF NOT EXISTS ix_dota_guild_members_account_id
                ON dota_guild_members(account_id);

            CREATE TABLE IF NOT EXISTS dota_guild_invites (
                guild_id INTEGER NOT NULL,
                target_account_id INTEGER NOT NULL,
                requester_account_id INTEGER NOT NULL,
                timestamp_sent INTEGER NOT NULL,
                PRIMARY KEY (guild_id, target_account_id),
                FOREIGN KEY (guild_id) REFERENCES dota_guilds(guild_id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }

    private void UpsertMemberLocked(
        SqliteConnection connection,
        SqliteTransaction transaction,
        uint guildId,
        uint accountId,
        uint roleId,
        uint joinedAt,
        uint lastActiveAt)
    {
        _identityResolver(accountId);
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO dota_guild_members (guild_id, account_id, role_id, joined_at, last_active_at)
            VALUES ($guild_id, $account_id, $role_id, $joined_at, $last_active_at)
            ON CONFLICT(guild_id, account_id) DO UPDATE SET
                role_id = excluded.role_id,
                last_active_at = excluded.last_active_at
            """;
        Add(command, "$guild_id", guildId);
        Add(command, "$account_id", accountId);
        Add(command, "$role_id", roleId);
        Add(command, "$joined_at", joinedAt);
        Add(command, "$last_active_at", lastActiveAt);
        command.ExecuteNonQuery();
    }

    private static void UpsertRoleLocked(
        SqliteConnection connection,
        SqliteTransaction transaction,
        uint guildId,
        uint roleId,
        string roleName,
        uint roleFlags,
        uint roleOrder)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO dota_guild_roles (guild_id, role_id, role_name, role_flags, role_order)
            VALUES ($guild_id, $role_id, $role_name, $role_flags, $role_order)
            ON CONFLICT(guild_id, role_id) DO UPDATE SET
                role_name = excluded.role_name,
                role_flags = excluded.role_flags,
                role_order = excluded.role_order
            """;
        Add(command, "$guild_id", guildId);
        Add(command, "$role_id", roleId);
        Add(command, "$role_name", roleName);
        Add(command, "$role_flags", roleFlags);
        Add(command, "$role_order", roleOrder);
        command.ExecuteNonQuery();
    }

    private static void TouchMemberLocked(SqliteConnection connection, SqliteTransaction? transaction, uint guildId, uint accountId)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            UPDATE dota_guild_members
            SET last_active_at = $last_active_at
            WHERE guild_id = $guild_id AND account_id = $account_id
            """;
        Add(command, "$last_active_at", Now());
        Add(command, "$guild_id", guildId);
        Add(command, "$account_id", accountId);
        command.ExecuteNonQuery();
    }

    private static uint NextIdLocked(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string table,
        string column,
        uint minimum)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = $"SELECT COALESCE(MAX({column}), 0) FROM {table}";
        var current = Convert.ToUInt32(command.ExecuteScalar() ?? 0);
        return Math.Max(current + 1, minimum);
    }

    private SqliteConnection OpenConnection()
    {
        return AppDatabase.OpenConnection(_dbPath);
    }

    private static uint Now()
    {
        return (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    private static string BuildGuildName(string personaName, uint accountId)
    {
        return $"{NormalizePersona(personaName, accountId)} Guild";
    }

    private static string BuildGuildTag(string personaName, uint accountId)
    {
        var letters = new string((personaName ?? string.Empty)
            .Where(char.IsLetterOrDigit)
            .Take(4)
            .Select(char.ToUpperInvariant)
            .ToArray());
        return letters.Length == 0 ? $"G{accountId % 1000:000}" : letters;
    }

    private static string NormalizePersona(string personaName, uint accountId)
    {
        var value = (personaName ?? string.Empty).Trim();
        return value.Length == 0 ? $"Player {accountId}" : value;
    }

    private static long ScalarLong(SqliteConnection connection, SqliteTransaction? transaction, string sql, params (string Name, object Value)[] args)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        foreach (var (name, value) in args)
        {
            Add(command, name, value);
        }

        return Convert.ToInt64(command.ExecuteScalar() ?? 0);
    }

    private static uint ScalarU32(SqliteConnection connection, SqliteTransaction? transaction, string sql, params (string Name, object Value)[] args)
    {
        return U32(ScalarLong(connection, transaction, sql, args));
    }

    private static uint U32(long value)
    {
        return unchecked((uint)value);
    }

    private static ulong U64(long value)
    {
        return unchecked((ulong)value);
    }

    private static void Add(SqliteCommand command, string name, object value)
    {
        command.Parameters.AddWithValue(name, value);
    }
}

public sealed class DotaGuildSnapshot
{
    public uint GuildId { get; init; }
    public DotaGuildInfoSnapshot Info { get; init; } = new();
    public List<DotaGuildRoleSnapshot> Roles { get; init; } = new();
    public List<DotaGuildMemberSnapshot> Members { get; init; } = new();
    public List<DotaGuildInviteSnapshot> Invites { get; init; } = new();
}

public sealed class DotaGuildInfoSnapshot
{
    public string GuildName { get; init; } = string.Empty;
    public string GuildTag { get; init; } = string.Empty;
    public uint CreatedTimestamp { get; init; }
    public uint GuildLanguage { get; init; }
    public uint GuildFlags { get; init; }
    public ulong GuildLogo { get; init; }
    public uint GuildRegion { get; init; }
    public ulong GuildChatGroupId { get; init; }
    public string GuildDescription { get; init; } = string.Empty;
    public ulong DefaultChatChannelId { get; init; }
    public uint GuildPrimaryColor { get; init; }
    public uint GuildSecondaryColor { get; init; }
    public uint GuildPattern { get; init; }
    public uint GuildRefreshTimeOffset { get; init; }
    public uint GuildRequiredRankTier { get; init; }
    public uint GuildMotdTimestamp { get; init; }
    public string GuildMotd { get; init; } = string.Empty;
}

public sealed class DotaGuildRoleSnapshot
{
    public uint RoleId { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public uint RoleFlags { get; init; }
    public uint RoleOrder { get; init; }
}

public sealed class DotaGuildMemberSnapshot
{
    public uint AccountId { get; init; }
    public uint RoleId { get; init; }
    public uint JoinedTimestamp { get; init; }
    public uint LastActiveTimestamp { get; init; }
}

public sealed class DotaGuildInviteSnapshot
{
    public uint GuildId { get; init; }
    public uint RequesterAccountId { get; init; }
    public uint TargetAccountId { get; init; }
    public uint TimestampSent { get; init; }
}

public sealed class DotaGuildMembershipSnapshot
{
    public List<uint> GuildIds { get; init; } = new();
    public List<DotaGuildInviteSnapshot> Invites { get; init; } = new();
}

public sealed class DotaGuildPersonaSnapshot
{
    public uint GuildId { get; init; }
    public string GuildTag { get; init; } = string.Empty;
    public uint GuildFlags { get; init; }
}

public sealed class DotaGuildEventDataSnapshot
{
    public uint GuildId { get; init; }
    public uint EventId { get; init; }
    public bool IsMember { get; init; }
    public uint GuildPoints { get; init; }
    public uint ContractsRefreshedTimestamp { get; init; }
    public uint CompletedChallengeCount { get; init; }
    public uint ChallengesRefreshTimestamp { get; init; }
    public uint GuildWeeklyPercentile { get; init; }
    public uint GuildWeeklyLastTimestamp { get; init; }
    public uint LastWeeklyClaimTime { get; init; }
    public uint GuildCurrentPercentile { get; init; }
}
