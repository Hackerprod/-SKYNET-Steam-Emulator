using Microsoft.Data.Sqlite;

namespace SKYNET_server.Services;

public sealed class DotaPartyStore
{
    private readonly object _sync = new();
    private readonly string _dbPath;
    private readonly Func<uint, DotaStatsAccountIdentity?> _identityResolver;

    public DotaPartyStore(string dbPath, Func<uint, DotaStatsAccountIdentity?> identityResolver)
    {
        _identityResolver = identityResolver;
        _dbPath = AppDatabase.PrepareDatabase(dbPath, path =>
        {
            using var connection = AppDatabase.OpenConnection(path);
            EnsureSchema(connection);
        });
    }

    public DotaPartyState? GetParty(ulong partyId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            return ReadPartyLocked(connection, partyId);
        }
    }

    public DotaPartyState? GetPartyByMember(ulong steamId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            return ReadPartyByMemberLocked(connection, steamId);
        }
    }

    public DotaPartyState EnsureParty(ulong leaderSteamId, uint leaderAccountId, string leaderName, DotaPartyPingData ping)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            var existing = ReadPartyByMemberLocked(connection, leaderSteamId);
            if (existing != null)
            {
                return existing;
            }

            using var transaction = connection.BeginTransaction();
            var partyId = NextIdLocked(connection, transaction, "parties", "party_id", 100000);
            using (var party = connection.CreateCommand())
            {
                party.Transaction = transaction;
                party.CommandText = """
                    INSERT INTO parties (party_id, leader_steam_id, state, permanent, ready_start, ready_finish, ready_initiator_account_id, updated_at)
                    VALUES ($party_id, $leader_steam_id, 0, 1, 0, 0, 0, $updated_at)
                    """;
                Add(party, "$party_id", partyId);
                Add(party, "$leader_steam_id", leaderSteamId);
                Add(party, "$updated_at", Now());
                party.ExecuteNonQuery();
            }

            UpsertMemberLocked(connection, transaction, partyId, leaderSteamId, leaderAccountId, leaderName, 0, false, ping);
            transaction.Commit();
            return ReadPartyLocked(connection, partyId) ?? throw new InvalidOperationException("Party was not created.");
        }
    }

    public DotaPartyState? AddMember(
        ulong partyId,
        ulong steamId,
        uint accountId,
        string personaName,
        DotaPartyPingData ping,
        bool isCoach = false)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            var party = ReadPartyLocked(connection, partyId, transaction);
            if (party == null)
            {
                transaction.Commit();
                return null;
            }

            var position = party.Members.Any(member => member.SteamId == steamId)
                ? party.Members.First(member => member.SteamId == steamId).Position
                : party.Members.Select(member => member.Position).DefaultIfEmpty(-1).Max() + 1;
            UpsertMemberLocked(connection, transaction, partyId, steamId, accountId, personaName, position, isCoach, ping);
            TouchPartyLocked(connection, transaction, partyId);
            DeleteInvitesForTargetLocked(connection, transaction, partyId, steamId);
            transaction.Commit();
            return ReadPartyLocked(connection, partyId);
        }
    }

    public DotaPartyState? RemoveMember(ulong partyId, ulong steamId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "DELETE FROM party_members WHERE party_id = $party_id AND steam_id = $steam_id";
                Add(command, "$party_id", partyId);
                Add(command, "$steam_id", steamId);
                command.ExecuteNonQuery();
            }

            var remaining = ReadMembersLocked(connection, partyId, transaction);
            if (remaining.Count == 0)
            {
                DeletePartyLocked(connection, transaction, partyId);
                transaction.Commit();
                return null;
            }

            var leader = ScalarU64(connection, transaction, "SELECT leader_steam_id FROM parties WHERE party_id = $party_id", ("$party_id", partyId));
            if (leader == steamId || remaining.All(member => member.SteamId != leader))
            {
                using var leaderCommand = connection.CreateCommand();
                leaderCommand.Transaction = transaction;
                leaderCommand.CommandText = "UPDATE parties SET leader_steam_id = $leader_steam_id WHERE party_id = $party_id";
                Add(leaderCommand, "$leader_steam_id", remaining.OrderBy(member => member.Position).First().SteamId);
                Add(leaderCommand, "$party_id", partyId);
                leaderCommand.ExecuteNonQuery();
            }

            TouchPartyLocked(connection, transaction, partyId);
            transaction.Commit();
            return ReadPartyLocked(connection, partyId);
        }
    }

    public void DeleteParty(ulong partyId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            DeletePartyLocked(connection, transaction, partyId);
            transaction.Commit();
        }
    }

    public DotaPartyState? SetLeader(ulong partyId, ulong leaderSteamId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            var isMember = ScalarU64(
                connection,
                transaction,
                "SELECT COUNT(*) FROM party_members WHERE party_id = $party_id AND steam_id = $steam_id",
                ("$party_id", partyId),
                ("$steam_id", leaderSteamId)) > 0;
            if (isMember)
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = "UPDATE parties SET leader_steam_id = $leader_steam_id, updated_at = $updated_at WHERE party_id = $party_id";
                Add(command, "$leader_steam_id", leaderSteamId);
                Add(command, "$updated_at", Now());
                Add(command, "$party_id", partyId);
                command.ExecuteNonQuery();
            }

            transaction.Commit();
            return ReadPartyLocked(connection, partyId);
        }
    }

    public DotaPartyState? SetMemberCoach(ulong partyId, ulong steamId, bool isCoach)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "UPDATE party_members SET is_coach = $is_coach WHERE party_id = $party_id AND steam_id = $steam_id";
                Add(command, "$is_coach", isCoach ? 1 : 0);
                Add(command, "$party_id", partyId);
                Add(command, "$steam_id", steamId);
                command.ExecuteNonQuery();
            }

            TouchPartyLocked(connection, transaction, partyId);
            transaction.Commit();
            return ReadPartyLocked(connection, partyId);
        }
    }

    public DotaPartyState? SetMemberPing(ulong partyId, ulong steamId, DotaPartyPingData ping)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = """
                    UPDATE party_members
                    SET region_ping_codes_csv = $codes,
                        region_ping_times_csv = $times,
                        region_ping_failed_bitmask = $failed
                    WHERE party_id = $party_id AND steam_id = $steam_id
                    """;
                Add(command, "$codes", Csv(ping.RegionCodes));
                Add(command, "$times", Csv(ping.RegionPings));
                Add(command, "$failed", ping.RegionPingFailedBitmask);
                Add(command, "$party_id", partyId);
                Add(command, "$steam_id", steamId);
                command.ExecuteNonQuery();
            }

            TouchPartyLocked(connection, transaction, partyId);
            transaction.Commit();
            return ReadPartyLocked(connection, partyId);
        }
    }

    public DotaPartyInvite CreateInvite(
        ulong partyId,
        ulong targetSteamId,
        ulong senderSteamId,
        string senderName,
        uint teamId,
        bool asCoach,
        out DotaPartyInvite? replaced)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            replaced = ReadInviteForTargetLocked(connection, partyId, targetSteamId, transaction);
            var inviteGid = NextIdLocked(connection, transaction, "party_invites", "invite_gid", 200000);
            DeleteInviteForTargetLocked(connection, transaction, partyId, targetSteamId);
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = """
                INSERT INTO party_invites (invite_gid, party_id, target_steam_id, sender_steam_id, sender_name, team_id, as_coach, low_priority_status, created_at)
                VALUES ($invite_gid, $party_id, $target_steam_id, $sender_steam_id, $sender_name, $team_id, $as_coach, 0, $created_at)
                """;
            Add(command, "$invite_gid", inviteGid);
            Add(command, "$party_id", partyId);
            Add(command, "$target_steam_id", targetSteamId);
            Add(command, "$sender_steam_id", senderSteamId);
            Add(command, "$sender_name", senderName ?? string.Empty);
            Add(command, "$team_id", teamId);
            Add(command, "$as_coach", asCoach ? 1 : 0);
            Add(command, "$created_at", Now());
            command.ExecuteNonQuery();
            transaction.Commit();
            return ReadInviteLocked(connection, inviteGid) ?? throw new InvalidOperationException("Party invite was not created.");
        }
    }

    public DotaPartyInvite? GetInviteForTarget(ulong partyId, ulong targetSteamId, uint createdAtOrAfter)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            var invite = ReadInviteForTargetLocked(connection, partyId, targetSteamId);
            return invite != null && invite.CreatedAt >= createdAtOrAfter ? invite : null;
        }
    }

    public IReadOnlyList<DotaPartyInvite> GetInvitesForTarget(ulong targetSteamId, uint createdAtOrAfter = 0)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            return ReadInvitesLocked(
                connection,
                "target_steam_id = $target_steam_id AND created_at >= $created_at",
                ("$target_steam_id", targetSteamId),
                ("$created_at", createdAtOrAfter));
        }
    }

    public IReadOnlyList<DotaPartyInvite> GetInvitesForParty(ulong partyId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            return ReadInvitesLocked(connection, "party_id = $party_id", ("$party_id", partyId));
        }
    }

    public DotaPartyInvite? TakeInvite(ulong partyId, ulong targetSteamId, uint createdAtOrAfter)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            var invite = ReadInviteForTargetLocked(connection, partyId, targetSteamId, transaction);
            if (invite == null || invite.CreatedAt < createdAtOrAfter)
            {
                transaction.Commit();
                return null;
            }

            DeleteInviteForTargetLocked(connection, transaction, partyId, targetSteamId);
            transaction.Commit();
            return invite;
        }
    }

    public IReadOnlyList<DotaPartyInvite> DeleteInvitesForTarget(ulong targetSteamId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            var invites = ReadInvitesLocked(
                connection,
                "target_steam_id = $target_steam_id",
                transaction,
                ("$target_steam_id", targetSteamId));
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "DELETE FROM party_invites WHERE target_steam_id = $target_steam_id";
            Add(command, "$target_steam_id", targetSteamId);
            command.ExecuteNonQuery();
            transaction.Commit();
            return invites;
        }
    }

    public IReadOnlyList<DotaPartyInvite> DeleteInvitesForParty(ulong partyId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            var invites = ReadInvitesLocked(connection, "party_id = $party_id", transaction, ("$party_id", partyId));
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "DELETE FROM party_invites WHERE party_id = $party_id";
            Add(command, "$party_id", partyId);
            command.ExecuteNonQuery();
            transaction.Commit();
            return invites;
        }
    }

    public IReadOnlyList<DotaPartyInvite> PruneInvitesCreatedBefore(uint cutoff)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            var invites = ReadInvitesLocked(connection, "created_at < $cutoff", transaction, ("$cutoff", cutoff));
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "DELETE FROM party_invites WHERE created_at < $cutoff";
            Add(command, "$cutoff", cutoff);
            command.ExecuteNonQuery();
            transaction.Commit();
            return invites;
        }
    }

    public DotaPartyState? StartReadyCheck(ulong partyId, uint initiatorAccountId, uint durationSeconds)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            var now = Now();
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = """
                    UPDATE parties
                    SET ready_start = $ready_start,
                        ready_finish = $ready_finish,
                        ready_initiator_account_id = $initiator,
                        updated_at = $updated_at
                    WHERE party_id = $party_id
                    """;
                Add(command, "$ready_start", now);
                Add(command, "$ready_finish", now + Math.Max(1u, durationSeconds));
                Add(command, "$initiator", initiatorAccountId);
                Add(command, "$updated_at", now);
                Add(command, "$party_id", partyId);
                command.ExecuteNonQuery();
            }

            using (var clear = connection.CreateCommand())
            {
                clear.Transaction = transaction;
                clear.CommandText = "UPDATE party_members SET ready_status = 0 WHERE party_id = $party_id";
                Add(clear, "$party_id", partyId);
                clear.ExecuteNonQuery();
            }

            transaction.Commit();
            return ReadPartyLocked(connection, partyId);
        }
    }

    public DotaPartyState? AcknowledgeReadyCheck(ulong partyId, ulong steamId, uint readyStatus)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            var party = ReadPartyLocked(connection, partyId, transaction);
            if (party == null || party.ReadyFinishTimestamp < Now())
            {
                transaction.Commit();
                return party;
            }

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = "UPDATE party_members SET ready_status = $ready_status WHERE party_id = $party_id AND steam_id = $steam_id";
                Add(command, "$ready_status", readyStatus);
                Add(command, "$party_id", partyId);
                Add(command, "$steam_id", steamId);
                command.ExecuteNonQuery();
            }

            var members = ReadMembersLocked(connection, partyId, transaction);
            if (members.Count > 0 && members.All(member => member.ReadyStatus != 0))
            {
                using var finish = connection.CreateCommand();
                finish.Transaction = transaction;
                finish.CommandText = "UPDATE parties SET ready_finish = $ready_finish WHERE party_id = $party_id";
                Add(finish, "$ready_finish", Now());
                Add(finish, "$party_id", partyId);
                finish.ExecuteNonQuery();
            }

            TouchPartyLocked(connection, transaction, partyId);
            transaction.Commit();
            return ReadPartyLocked(connection, partyId);
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
            CREATE TABLE IF NOT EXISTS parties (
                party_id INTEGER PRIMARY KEY,
                leader_steam_id INTEGER NOT NULL,
                state INTEGER NOT NULL DEFAULT 0,
                permanent INTEGER NOT NULL DEFAULT 1,
                ready_start INTEGER NOT NULL DEFAULT 0,
                ready_finish INTEGER NOT NULL DEFAULT 0,
                ready_initiator_account_id INTEGER NOT NULL DEFAULT 0,
                updated_at INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS party_members (
                party_id INTEGER NOT NULL,
                steam_id INTEGER NOT NULL,
                account_id INTEGER NOT NULL,
                persona_name TEXT NOT NULL,
                position INTEGER NOT NULL,
                is_coach INTEGER NOT NULL DEFAULT 0,
                is_plus_subscriber INTEGER NOT NULL DEFAULT 0,
                region_ping_codes_csv TEXT NOT NULL,
                region_ping_times_csv TEXT NOT NULL,
                region_ping_failed_bitmask INTEGER NOT NULL DEFAULT 0,
                ready_status INTEGER NOT NULL DEFAULT 0,
                PRIMARY KEY (party_id, steam_id)
            );

            CREATE INDEX IF NOT EXISTS idx_party_members_steam ON party_members (steam_id);

            CREATE TABLE IF NOT EXISTS party_invites (
                invite_gid INTEGER PRIMARY KEY,
                party_id INTEGER NOT NULL,
                target_steam_id INTEGER NOT NULL,
                sender_steam_id INTEGER NOT NULL,
                sender_name TEXT NOT NULL,
                team_id INTEGER NOT NULL,
                as_coach INTEGER NOT NULL,
                low_priority_status INTEGER NOT NULL,
                created_at INTEGER NOT NULL
            );

            CREATE INDEX IF NOT EXISTS idx_party_invites_target ON party_invites (target_steam_id);

            DELETE FROM party_invites
            WHERE invite_gid NOT IN (
                SELECT MAX(invite_gid)
                FROM party_invites
                GROUP BY party_id, target_steam_id
            );

            CREATE UNIQUE INDEX IF NOT EXISTS idx_party_invites_party_target
            ON party_invites (party_id, target_steam_id);
            """;
        command.ExecuteNonQuery();
    }

    private DotaPartyState? ReadPartyByMemberLocked(SqliteConnection connection, ulong steamId, SqliteTransaction? transaction = null)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT party_id FROM party_members WHERE steam_id = $steam_id ORDER BY party_id DESC LIMIT 1";
        Add(command, "$steam_id", steamId);
        var value = command.ExecuteScalar();
        return value == null || value == DBNull.Value ? null : ReadPartyLocked(connection, Convert.ToUInt64(value), transaction);
    }

    private DotaPartyState? ReadPartyLocked(SqliteConnection connection, ulong partyId, SqliteTransaction? transaction = null)
    {
        ulong foundPartyId;
        ulong leaderSteamId;
        uint state;
        bool permanent;
        uint readyStart;
        uint readyFinish;
        uint readyInitiator;
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT * FROM parties WHERE party_id = $party_id";
        Add(command, "$party_id", partyId);
        using (var reader = command.ExecuteReader())
        {
            if (!reader.Read())
            {
                return null;
            }

            foundPartyId = U64(reader, "party_id");
            leaderSteamId = U64(reader, "leader_steam_id");
            state = U32(reader, "state");
            permanent = U32(reader, "permanent") != 0;
            readyStart = U32(reader, "ready_start");
            readyFinish = U32(reader, "ready_finish");
            readyInitiator = U32(reader, "ready_initiator_account_id");
        }

        return new DotaPartyState
        {
            PartyId = foundPartyId,
            LeaderSteamId = leaderSteamId,
            State = state,
            Permanent = permanent,
            ReadyStartTimestamp = readyStart,
            ReadyFinishTimestamp = readyFinish,
            ReadyInitiatorAccountId = readyInitiator,
            Members = ReadMembersLocked(connection, partyId, transaction)
        };
    }

    private IReadOnlyList<DotaPartyMember> ReadMembersLocked(SqliteConnection connection, ulong partyId, SqliteTransaction? transaction = null)
    {
        var members = new List<DotaPartyMember>();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT * FROM party_members WHERE party_id = $party_id ORDER BY position ASC, steam_id ASC";
        Add(command, "$party_id", partyId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            members.Add(new DotaPartyMember
            {
                SteamId = U64(reader, "steam_id"),
                AccountId = U32(reader, "account_id"),
                PersonaName = Str(reader, "persona_name"),
                Position = I32(reader, "position"),
                IsCoach = U32(reader, "is_coach") != 0,
                IsPlusSubscriber = U32(reader, "is_plus_subscriber") != 0,
                RegionCodes = ParseCsv(Str(reader, "region_ping_codes_csv")),
                RegionPings = ParseCsv(Str(reader, "region_ping_times_csv")),
                RegionPingFailedBitmask = U32(reader, "region_ping_failed_bitmask"),
                ReadyStatus = U32(reader, "ready_status")
            });
        }

        return members;
    }

    private DotaPartyInvite? ReadInviteLocked(SqliteConnection connection, ulong inviteGid, SqliteTransaction? transaction = null)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT * FROM party_invites WHERE invite_gid = $invite_gid";
        Add(command, "$invite_gid", inviteGid);
        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new DotaPartyInvite
        {
            InviteGid = U64(reader, "invite_gid"),
            PartyId = U64(reader, "party_id"),
            TargetSteamId = U64(reader, "target_steam_id"),
            SenderSteamId = U64(reader, "sender_steam_id"),
            SenderName = Str(reader, "sender_name"),
            TeamId = U32(reader, "team_id"),
            AsCoach = U32(reader, "as_coach") != 0,
            LowPriorityStatus = U32(reader, "low_priority_status") != 0,
            CreatedAt = U32(reader, "created_at")
        };
    }

    private DotaPartyInvite? ReadInviteForTargetLocked(
        SqliteConnection connection,
        ulong partyId,
        ulong targetSteamId,
        SqliteTransaction? transaction = null)
    {
        return ReadInvitesLocked(
            connection,
            "party_id = $party_id AND target_steam_id = $target_steam_id",
            transaction,
            ("$party_id", partyId),
            ("$target_steam_id", targetSteamId)).FirstOrDefault();
    }

    private static IReadOnlyList<DotaPartyInvite> ReadInvitesLocked(
        SqliteConnection connection,
        string where,
        params (string Name, object Value)[] parameters)
    {
        return ReadInvitesLocked(connection, where, null, parameters);
    }

    private static IReadOnlyList<DotaPartyInvite> ReadInvitesLocked(
        SqliteConnection connection,
        string where,
        SqliteTransaction? transaction,
        params (string Name, object Value)[] parameters)
    {
        var invites = new List<DotaPartyInvite>();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = $"SELECT * FROM party_invites WHERE {where} ORDER BY created_at DESC, invite_gid DESC";
        foreach (var parameter in parameters)
        {
            Add(command, parameter.Name, parameter.Value);
        }

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            invites.Add(new DotaPartyInvite
            {
                InviteGid = U64(reader, "invite_gid"),
                PartyId = U64(reader, "party_id"),
                TargetSteamId = U64(reader, "target_steam_id"),
                SenderSteamId = U64(reader, "sender_steam_id"),
                SenderName = Str(reader, "sender_name"),
                TeamId = U32(reader, "team_id"),
                AsCoach = U32(reader, "as_coach") != 0,
                LowPriorityStatus = U32(reader, "low_priority_status") != 0,
                CreatedAt = U32(reader, "created_at")
            });
        }

        return invites;
    }

    private void UpsertMemberLocked(
        SqliteConnection connection,
        SqliteTransaction transaction,
        ulong partyId,
        ulong steamId,
        uint accountId,
        string personaName,
        int position,
        bool isCoach,
        DotaPartyPingData ping)
    {
        var identity = ResolveIdentity(steamId, accountId, personaName);
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO party_members (
                party_id, steam_id, account_id, persona_name, position, is_coach, is_plus_subscriber,
                region_ping_codes_csv, region_ping_times_csv, region_ping_failed_bitmask, ready_status
            ) VALUES (
                $party_id, $steam_id, $account_id, $persona_name, $position, $is_coach, 0,
                $region_codes, $region_pings, $failed, 0
            )
            ON CONFLICT(party_id, steam_id) DO UPDATE SET
                account_id = excluded.account_id,
                persona_name = excluded.persona_name,
                position = excluded.position,
                is_coach = excluded.is_coach,
                region_ping_codes_csv = excluded.region_ping_codes_csv,
                region_ping_times_csv = excluded.region_ping_times_csv,
                region_ping_failed_bitmask = excluded.region_ping_failed_bitmask
            """;
        Add(command, "$party_id", partyId);
        Add(command, "$steam_id", identity.SteamId);
        Add(command, "$account_id", identity.AccountId);
        Add(command, "$persona_name", identity.PersonaName);
        Add(command, "$position", position);
        Add(command, "$is_coach", isCoach ? 1 : 0);
        Add(command, "$region_codes", Csv(ping.RegionCodes));
        Add(command, "$region_pings", Csv(ping.RegionPings));
        Add(command, "$failed", ping.RegionPingFailedBitmask);
        command.ExecuteNonQuery();
    }

    private DotaStatsAccountIdentity ResolveIdentity(ulong steamId, uint accountId, string personaName)
    {
        var resolvedAccountId = accountId != 0 ? accountId : SteamIdToAccountId(steamId);
        var resolvedSteamId = steamId != 0 ? steamId : ToSteamId(resolvedAccountId);
        var identity = resolvedAccountId == 0 ? null : _identityResolver(resolvedAccountId);
        if (identity != null)
        {
            return string.IsNullOrWhiteSpace(personaName)
                ? identity
                : new DotaStatsAccountIdentity(identity.AccountId, identity.SteamId, personaName);
        }

        if (resolvedAccountId == 0)
        {
            resolvedAccountId = SteamIdToAccountId(resolvedSteamId);
        }

        return new DotaStatsAccountIdentity(
            resolvedAccountId,
            resolvedSteamId,
            string.IsNullOrWhiteSpace(personaName) ? $"User{resolvedAccountId}" : personaName);
    }

    private static void DeleteInvitesForTargetLocked(SqliteConnection connection, SqliteTransaction transaction, ulong partyId, ulong steamId)
    {
        DeleteInviteForTargetLocked(connection, transaction, partyId, steamId);
    }

    private static void DeleteInviteForTargetLocked(SqliteConnection connection, SqliteTransaction transaction, ulong partyId, ulong steamId)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "DELETE FROM party_invites WHERE party_id = $party_id AND target_steam_id = $target_steam_id";
        Add(command, "$party_id", partyId);
        Add(command, "$target_steam_id", steamId);
        command.ExecuteNonQuery();
    }

    private static void DeletePartyLocked(SqliteConnection connection, SqliteTransaction transaction, ulong partyId)
    {
        foreach (var table in new[] { "party_invites", "party_members", "parties" })
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $"DELETE FROM {table} WHERE party_id = $party_id";
            Add(command, "$party_id", partyId);
            command.ExecuteNonQuery();
        }
    }

    private static void TouchPartyLocked(SqliteConnection connection, SqliteTransaction transaction, ulong partyId)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "UPDATE parties SET updated_at = $updated_at WHERE party_id = $party_id";
        Add(command, "$updated_at", Now());
        Add(command, "$party_id", partyId);
        command.ExecuteNonQuery();
    }

    private static ulong NextIdLocked(SqliteConnection connection, SqliteTransaction transaction, string table, string column, ulong minimum)
    {
        var current = ScalarU64(connection, transaction, $"SELECT COALESCE(MAX({column}), 0) FROM {table}");
        return Math.Max(current + 1, minimum);
    }

    private static ulong ScalarU64(SqliteConnection connection, SqliteTransaction? transaction, string sql, params (string Name, object Value)[] parameters)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        foreach (var parameter in parameters)
        {
            Add(command, parameter.Name, parameter.Value);
        }

        var value = command.ExecuteScalar();
        return value == null || value == DBNull.Value ? 0 : Convert.ToUInt64(value);
    }

    private static void Add(SqliteCommand command, string name, object? value)
    {
        command.Parameters.AddWithValue(name, value ?? DBNull.Value);
    }

    private static string Csv(IEnumerable<uint> values) => string.Join(",", values ?? Array.Empty<uint>());

    private static IReadOnlyList<uint> ParseCsv(string csv)
    {
        if (string.IsNullOrWhiteSpace(csv))
        {
            return Array.Empty<uint>();
        }

        var values = new List<uint>();
        foreach (var part in csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (uint.TryParse(part, out var value))
            {
                values.Add(value);
            }
        }

        return values;
    }

    private static uint U32(SqliteDataReader reader, string name) => Convert.ToUInt32(reader[name]);
    private static int I32(SqliteDataReader reader, string name) => Convert.ToInt32(reader[name]);
    private static ulong U64(SqliteDataReader reader, string name) => Convert.ToUInt64(reader[name]);
    private static string Str(SqliteDataReader reader, string name) => Convert.ToString(reader[name]) ?? string.Empty;
    private static uint Now() => (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    private static ulong ToSteamId(uint accountId) => 76561197960265728UL + accountId;
    private static uint SteamIdToAccountId(ulong steamId) => steamId > 76561197960265728UL ? (uint)(steamId - 76561197960265728UL) : 0;
}

public sealed class DotaPartyState
{
    public ulong PartyId { get; init; }
    public ulong LeaderSteamId { get; init; }
    public uint State { get; init; }
    public bool Permanent { get; init; }
    public uint ReadyStartTimestamp { get; init; }
    public uint ReadyFinishTimestamp { get; init; }
    public uint ReadyInitiatorAccountId { get; init; }
    public IReadOnlyList<DotaPartyMember> Members { get; init; } = Array.Empty<DotaPartyMember>();
}

public sealed class DotaPartyMember
{
    public ulong SteamId { get; init; }
    public uint AccountId { get; init; }
    public string PersonaName { get; init; } = string.Empty;
    public int Position { get; init; }
    public bool IsCoach { get; init; }
    public bool IsPlusSubscriber { get; init; }
    public IReadOnlyList<uint> RegionCodes { get; init; } = Array.Empty<uint>();
    public IReadOnlyList<uint> RegionPings { get; init; } = Array.Empty<uint>();
    public uint RegionPingFailedBitmask { get; init; }
    public uint ReadyStatus { get; init; }
}

public sealed class DotaPartyInvite
{
    public ulong InviteGid { get; init; }
    public ulong PartyId { get; init; }
    public ulong TargetSteamId { get; init; }
    public ulong SenderSteamId { get; init; }
    public string SenderName { get; init; } = string.Empty;
    public uint TeamId { get; init; }
    public bool AsCoach { get; init; }
    public bool LowPriorityStatus { get; init; }
    public uint CreatedAt { get; init; }
}

public sealed class DotaPartyPingData
{
    public IReadOnlyList<uint> RegionCodes { get; init; } = Array.Empty<uint>();
    public IReadOnlyList<uint> RegionPings { get; init; } = Array.Empty<uint>();
    public uint RegionPingFailedBitmask { get; init; }
}
