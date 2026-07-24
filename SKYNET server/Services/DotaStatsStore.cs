using Microsoft.Data.Sqlite;

namespace SKYNET_server.Services;

public sealed class DotaStatsStore
{
    private const int DefaultEmoticonAccessMaskBytes = 72;

    private static readonly uint[] AllHeroChallengeHeroIds =
    {
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
        21, 22, 23, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39,
        40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57,
        58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75,
        76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93,
        94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108,
        109, 110, 111, 112, 113, 114, 119, 120, 121
    };

    private readonly object _sync = new();
    private readonly string _dbPath;
    private readonly Func<uint, DotaStatsAccountIdentity?> _identityResolver;
    private static readonly string[] ImportTableNames =
    {
        "profiles",
        "profile_slots",
        "featured_heroes",
        "trophies",
        "all_hero_order",
        "matches",
        "match_players",
        "mvp_votes",
        "global_stats",
        "hero_stats",
        "reports",
        "match_comments",
        "match_votes",
        "emoticon_access",
        "social_feed",
        "social_feed_comments",
        "match_signout_permission_requests",
        "match_history_access",
        "server_status_requests",
        "match_leaver_events",
        "match_realtime_stats",
        "match_state_history",
        "match_spectator_counts",
        "live_scoreboard_updates",
        "quest_progress",
        "periodic_resources",
        "hero_stickers",
        "overworld_state",
        "monster_hunter_state"
    };

    public DotaStatsStore(string dbPath, Func<uint, DotaStatsAccountIdentity?> identityResolver)
    {
        _identityResolver = identityResolver ?? (_ => null);
        _dbPath = AppDatabase.PrepareDatabase(dbPath, path =>
        {
            using var connection = AppDatabase.OpenConnection(path);
            EnsureSchema(connection);
        });
    }

    public int ImportMissingFrom(string dbPath)
    {
        if (string.IsNullOrWhiteSpace(dbPath))
        {
            return 0;
        }

        var sourcePath = Path.GetFullPath(dbPath);
        var targetPath = Path.GetFullPath(_dbPath);
        if (sourcePath.Equals(targetPath, StringComparison.OrdinalIgnoreCase) ||
            !File.Exists(sourcePath) ||
            new FileInfo(sourcePath).Length == 0)
        {
            return 0;
        }

        lock (_sync)
        {
            using var connection = OpenConnection();
            using (var attach = connection.CreateCommand())
            {
                attach.CommandText = "ATTACH DATABASE $source AS legacy";
                Add(attach, "$source", sourcePath);
                attach.ExecuteNonQuery();
            }

            try
            {
                var imported = 0;
                foreach (var table in ImportTableNames)
                {
                    if (!TableExists(connection, "legacy", table))
                    {
                        continue;
                    }

                    var targetColumns = ReadTableColumns(connection, "main", table);
                    var sourceColumns = ReadTableColumns(connection, "legacy", table);
                    var sourceColumnSet = sourceColumns.ToHashSet(StringComparer.OrdinalIgnoreCase);
                    var columns = targetColumns
                        .Where(column => sourceColumnSet.Contains(column))
                        .ToArray();
                    if (columns.Length == 0)
                    {
                        continue;
                    }

                    var columnList = string.Join(", ", columns.Select(QuoteIdentifier));
                    using var copy = connection.CreateCommand();
                    copy.CommandText =
                        $"INSERT OR IGNORE INTO {QuoteIdentifier(table)} ({columnList}) " +
                        $"SELECT {columnList} FROM legacy.{QuoteIdentifier(table)}";
                    imported += Math.Max(0, copy.ExecuteNonQuery());
                }

                return imported;
            }
            finally
            {
                using var detach = connection.CreateCommand();
                detach.CommandText = "DETACH DATABASE legacy";
                detach.ExecuteNonQuery();
            }
        }
    }

    public DotaStatsProfile EnsureProfile(ulong steamId, uint accountId, string personaName)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            return EnsureProfileLocked(connection, steamId, accountId, personaName);
        }
    }

    public DotaStatsProfile GetProfile(uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            var identity = ResolveIdentity(accountId, 0, string.Empty);
            return EnsureProfileLocked(connection, identity.SteamId, identity.AccountId, identity.PersonaName);
        }
    }

    public DotaStatsGlobalStats GetGlobalStats(uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            return ReadGlobalStats(connection, accountId) ?? new DotaStatsGlobalStats { AccountId = accountId };
        }
    }

    public IReadOnlyList<DotaStatsHeroStats> GetHeroStandings(uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            var result = new List<DotaStatsHeroStats>();
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT account_id, steam_id, hero_id, wins, losses, win_streak, best_win_streak,
                       avg_kills, avg_deaths, avg_assists, avg_gpm, avg_xpm,
                       best_kills, best_assists, best_gpm, best_xpm, performance,
                       avg_lasthits, avg_denies, networth_peak, lasthit_peak, deny_peak, damage_peak, longest_game_peak, healing_peak
                FROM hero_stats
                WHERE account_id = $account_id
                ORDER BY (wins + losses) DESC, performance DESC, hero_id ASC
                """;
            Add(command, "$account_id", accountId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(ReadHeroStats(reader));
            }

            return result;
        }
    }

    public IReadOnlyList<DotaStatsMatchPlayer> GetMatchHistory(uint accountId, ulong startAtMatchId, uint requested, uint heroId, bool includePractice)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            var limit = requested == 0 ? 20 : Math.Min(requested, 50);
            var result = new List<DotaStatsMatchPlayer>();
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT mp.*, m.start_time AS match_start_time, m.duration AS match_duration, m.game_mode AS match_game_mode,
                       m.lobby_type AS match_lobby_type, m.good_guys_win, m.match_flags, m.radiant_score, m.dire_score,
                       m.cluster, m.first_blood_time
                FROM match_players mp
                JOIN matches m ON m.match_id = mp.match_id
                WHERE mp.account_id = $account_id
                  AND ($hero_id = 0 OR mp.hero_id = $hero_id)
                  AND ($include_practice = 1 OR m.lobby_type <> 1)
                  AND ($start_match = 0 OR mp.match_id < $start_match)
                ORDER BY m.start_time DESC, mp.match_id DESC
                LIMIT $limit
                """;
            Add(command, "$account_id", accountId);
            Add(command, "$hero_id", heroId);
            Add(command, "$include_practice", includePractice ? 1 : 0);
            Add(command, "$start_match", startAtMatchId);
            Add(command, "$limit", (int)limit);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(ReadMatchPlayer(reader));
            }

            return result;
        }
    }

    public DotaStatsMatch? GetMatch(ulong matchId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            return ReadMatch(connection, matchId);
        }
    }

    public IReadOnlyList<DotaStatsMatchPlayer> GetMatchPlayers(ulong matchId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            return ReadMatchPlayers(connection, matchId);
        }
    }

    public IReadOnlyList<DotaStatsMatchPlayer> GetRecentMatches(uint accountId, int count, uint heroId = 0)
    {
        return GetMatchHistory(accountId, 0, (uint)Math.Max(1, count), heroId, true);
    }

    public uint GetFirstMatchTime(uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            return (uint)ScalarLong(
                connection,
                null,
                "SELECT COALESCE(MIN(m.start_time), 0) FROM match_players mp JOIN matches m ON m.match_id = mp.match_id WHERE mp.account_id = $account_id",
                ("$account_id", accountId));
        }
    }

    public uint GetMatchCount(uint accountId, uint heroId = 0)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            return (uint)ScalarLong(
                connection,
                null,
                "SELECT COUNT(*) FROM match_players WHERE account_id = $account_id AND ($hero_id = 0 OR hero_id = $hero_id)",
                ("$account_id", accountId),
                ("$hero_id", heroId));
        }
    }

    public uint GetMvpCount(uint accountId, uint heroId = 0)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            return (uint)ScalarLong(
                connection,
                null,
                "SELECT COUNT(*) FROM match_players WHERE account_id = $account_id AND mvp = 1 AND ($hero_id = 0 OR hero_id = $hero_id)",
                ("$account_id", accountId),
                ("$hero_id", heroId));
        }
    }

    public bool HasMvpVote(ulong matchId, uint voterAccountId)
    {
        if (matchId == 0 || voterAccountId == 0)
        {
            return false;
        }

        lock (_sync)
        {
            using var connection = OpenConnection();
            return ScalarLong(
                connection,
                null,
                "SELECT COUNT(*) FROM mvp_votes WHERE match_id = $match_id AND voter_account_id = $voter_account_id",
                ("$match_id", matchId),
                ("$voter_account_id", voterAccountId)) > 0;
        }
    }

    public bool SaveMvpVote(ulong matchId, uint voterAccountId, uint votedAccountId)
    {
        if (matchId == 0 || voterAccountId == 0 || votedAccountId == 0)
        {
            return false;
        }

        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, voterAccountId, string.Empty);
            EnsureProfileLocked(connection, 0, votedAccountId, string.Empty);
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO mvp_votes (match_id, voter_account_id, voted_account_id, created_at)
                VALUES ($match_id, $voter_account_id, $voted_account_id, $created_at)
                ON CONFLICT(match_id, voter_account_id) DO UPDATE SET
                    voted_account_id = excluded.voted_account_id,
                    created_at = excluded.created_at
                """;
            Add(command, "$match_id", matchId);
            Add(command, "$voter_account_id", voterAccountId);
            Add(command, "$voted_account_id", votedAccountId);
            Add(command, "$created_at", Now());
            command.ExecuteNonQuery();
            return true;
        }
    }

    public bool FinalizeMvpVotes(ulong matchId)
    {
        if (matchId == 0)
        {
            return false;
        }

        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            var winners = new List<uint>();
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = """
                    SELECT voted_account_id, COUNT(*) AS votes
                    FROM mvp_votes
                    WHERE match_id = $match_id
                    GROUP BY voted_account_id
                    HAVING votes = (
                        SELECT MAX(vote_count)
                        FROM (
                            SELECT COUNT(*) AS vote_count
                            FROM mvp_votes
                            WHERE match_id = $match_id
                            GROUP BY voted_account_id
                        )
                    )
                    """;
                Add(command, "$match_id", matchId);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    winners.Add(U32(reader, "voted_account_id"));
                }
            }

            if (winners.Count == 0)
            {
                transaction.Commit();
                return false;
            }

            using (var clear = connection.CreateCommand())
            {
                clear.Transaction = transaction;
                clear.CommandText = "UPDATE match_players SET mvp = 0 WHERE match_id = $match_id";
                Add(clear, "$match_id", matchId);
                clear.ExecuteNonQuery();
            }

            foreach (var accountId in winners)
            {
                using var mark = connection.CreateCommand();
                mark.Transaction = transaction;
                mark.CommandText = "UPDATE match_players SET mvp = 1 WHERE match_id = $match_id AND account_id = $account_id";
                Add(mark, "$match_id", matchId);
                Add(mark, "$account_id", accountId);
                mark.ExecuteNonQuery();
            }

            transaction.Commit();
            return true;
        }
    }

    public bool SetMatchMvps(ulong matchId, IEnumerable<uint> accountIds)
    {
        if (matchId == 0)
        {
            return false;
        }

        var winners = accountIds
            .Where(accountId => accountId != 0)
            .Distinct()
            .ToList();
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            using (var clear = connection.CreateCommand())
            {
                clear.Transaction = transaction;
                clear.CommandText = "UPDATE match_players SET mvp = 0 WHERE match_id = $match_id";
                Add(clear, "$match_id", matchId);
                clear.ExecuteNonQuery();
            }

            foreach (var accountId in winners)
            {
                using var mark = connection.CreateCommand();
                mark.Transaction = transaction;
                mark.CommandText = "UPDATE match_players SET mvp = 1 WHERE match_id = $match_id AND account_id = $account_id";
                Add(mark, "$match_id", matchId);
                Add(mark, "$account_id", accountId);
                mark.ExecuteNonQuery();
            }

            transaction.Commit();
            return winners.Count > 0;
        }
    }

    public IReadOnlyList<DotaStatsTrophy> GetTrophies(uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            var result = new List<DotaStatsTrophy>();
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT account_id, steam_id, trophy_id, trophy_score, last_updated
                FROM trophies
                WHERE account_id = $account_id AND trophy_score <> 0
                ORDER BY trophy_id ASC
                """;
            Add(command, "$account_id", accountId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new DotaStatsTrophy
                {
                    AccountId = U32(reader, "account_id"),
                    SteamId = U64(reader, "steam_id"),
                    TrophyId = U32(reader, "trophy_id"),
                    TrophyScore = U32(reader, "trophy_score"),
                    LastUpdated = U32(reader, "last_updated")
                });
            }

            return result;
        }
    }

    public IReadOnlyList<DotaStatsProfileSlot> GetProfileSlots(uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            EnsureDefaultProfileSlotsLocked(connection, accountId);
            return ReadProfileSlots(connection, accountId);
        }
    }

    public void SaveProfileSlots(uint accountId, IEnumerable<DotaStatsProfileSlot> slots)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            using var transaction = connection.BeginTransaction();
            foreach (var slot in slots)
            {
                if (slot.SlotType == 0 || slot.SlotValue == 0)
                {
                    using var delete = connection.CreateCommand();
                    delete.Transaction = transaction;
                    delete.CommandText = "DELETE FROM profile_slots WHERE account_id = $account_id AND slot_id = $slot_id";
                    Add(delete, "$account_id", accountId);
                    Add(delete, "$slot_id", slot.SlotId);
                    delete.ExecuteNonQuery();
                    continue;
                }

                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = """
                    INSERT INTO profile_slots (account_id, slot_id, slot_type, slot_value)
                    VALUES ($account_id, $slot_id, $slot_type, $slot_value)
                    ON CONFLICT(account_id, slot_id) DO UPDATE SET
                        slot_type = excluded.slot_type,
                        slot_value = excluded.slot_value
                    """;
                Add(command, "$account_id", accountId);
                Add(command, "$slot_id", slot.SlotId);
                Add(command, "$slot_type", slot.SlotType);
                Add(command, "$slot_value", slot.SlotValue);
                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }
    }

    public void SaveProfileUpdate(uint accountId, ulong? backgroundItemId, IReadOnlyList<uint> featuredHeroIds)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            using var transaction = connection.BeginTransaction();
            // ProfileUpdate always carries featured heroes, but the background
            // is changed only after the GC resolves the submitted item id to an
            // owned defIndex. Invalid or absent background items leave the
            // previous background intact, matching the legacy GC flow.
            if (backgroundItemId.HasValue)
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = "UPDATE profiles SET background_item_id = $background_item_id, updated_at = $updated_at WHERE account_id = $account_id";
                Add(command, "$background_item_id", backgroundItemId.Value);
                Add(command, "$updated_at", Now());
                Add(command, "$account_id", accountId);
                command.ExecuteNonQuery();
            }

            using (var delete = connection.CreateCommand())
            {
                delete.Transaction = transaction;
                delete.CommandText = "DELETE FROM featured_heroes WHERE account_id = $account_id";
                Add(delete, "$account_id", accountId);
                delete.ExecuteNonQuery();
            }

            for (var i = 0; i < Math.Min(featuredHeroIds.Count, 3); i++)
            {
                using var insert = connection.CreateCommand();
                insert.Transaction = transaction;
                insert.CommandText = "INSERT INTO featured_heroes (account_id, position, hero_id) VALUES ($account_id, $position, $hero_id)";
                Add(insert, "$account_id", accountId);
                Add(insert, "$position", i);
                Add(insert, "$hero_id", featuredHeroIds[i]);
                insert.ExecuteNonQuery();
            }

            transaction.Commit();
        }
    }

    public IReadOnlyList<uint> GetFeaturedHeroes(uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            var heroes = new List<uint>();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT hero_id FROM featured_heroes WHERE account_id = $account_id ORDER BY position ASC";
            Add(command, "$account_id", accountId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                heroes.Add(U32(reader, "hero_id"));
            }

            if (heroes.Count > 0)
            {
                return heroes;
            }

            return GetHeroStandings(accountId).Take(3).Select(hero => hero.HeroId).DefaultIfEmpty(1u).ToList();
        }
    }

    public DotaStatsAllHeroProgress GetAllHeroProgress(uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            EnsureAllHeroProgressLocked(connection, accountId);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM all_hero_order WHERE account_id = $account_id";
            Add(command, "$account_id", accountId);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return ReadAllHeroProgress(reader);
            }

            throw new InvalidOperationException("All hero progress was not initialized.");
        }
    }

    public DotaStatsAllHeroProgress? RerollAllHeroChallenge(uint accountId)
    {
        if (accountId == 0)
        {
            return null;
        }

        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            EnsureProfileLocked(connection, 0, accountId, string.Empty, transaction);
            var progress = ReadAllHeroProgressLocked(connection, transaction, accountId);
            if (progress.HeroIds.Count <= 1)
            {
                transaction.Commit();
                return progress;
            }

            var currentIndex = progress.HeroIds.IndexOf(progress.CurrentHeroId);
            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            var nextIndex = (currentIndex + 1) % progress.HeroIds.Count;
            progress.PreviousHeroId = progress.CurrentHeroId;
            progress.PreviousHeroGames = progress.CurrentHeroGames;
            progress.CurrentHeroId = progress.HeroIds[nextIndex];
            progress.NextHeroId = progress.HeroIds[(nextIndex + 1) % progress.HeroIds.Count];
            progress.CurrentHeroGames = 0;
            SaveAllHeroProgressLocked(connection, transaction, progress);
            transaction.Commit();
            return progress;
        }
    }

    public void RecordMatch(DotaStatsMatch match)
    {
        if (match.MatchId == 0)
        {
            throw new ArgumentException("match_id is required", nameof(match));
        }

        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            var alreadyRecorded = ScalarLong(connection, transaction, "SELECT COUNT(*) FROM match_players WHERE match_id = $match_id", ("$match_id", match.MatchId)) > 0;
            UpsertMatchLocked(connection, transaction, match);
            if (alreadyRecorded)
            {
                transaction.Commit();
                return;
            }

            foreach (var player in match.Players.Where(player => player.SteamId != 0 && player.AccountId != 0))
            {
                var identity = ResolveIdentity(player.AccountId, player.SteamId, player.PersonaName);
                player.SteamId = identity.SteamId;
                player.AccountId = identity.AccountId;
                player.PersonaName = identity.PersonaName;
                var profile = EnsureProfileLocked(connection, identity.SteamId, identity.AccountId, identity.PersonaName, transaction);
                var playedHeroBefore = ScalarLong(
                    connection,
                    transaction,
                    "SELECT COUNT(*) FROM match_players WHERE account_id = $account_id AND hero_id = $hero_id",
                    ("$account_id", player.AccountId),
                    ("$hero_id", player.HeroId)) > 0;

                UpsertMatchPlayerLocked(connection, transaction, match, player);
                UpsertGlobalStatsLocked(connection, transaction, match, player, playedHeroBefore);
                UpsertHeroStatsLocked(connection, transaction, player);
                UpdateProfileProgressLocked(connection, transaction, profile, player.Winner, player.HeroId);
                UpdateAllHeroProgressLocked(connection, transaction, player.AccountId, player.Winner, player.HeroId);
                CreateSocialFeedForMatchLocked(connection, transaction, player, match);
            }

            transaction.Commit();
        }
    }

    public void SaveReport(ulong reporterSteamId, uint reporterAccountId, uint targetAccountId, ulong lobbyId, uint reportFlags, string comment)
    {
        TrySavePlayerReport(new DotaStatsPlayerReport
        {
            ReporterSteamId = reporterSteamId,
            ReporterAccountId = reporterAccountId,
            TargetAccountId = targetAccountId,
            LobbyId = lobbyId,
            ReportFlags = reportFlags,
            Comment = comment ?? string.Empty
        });
    }

    public bool TrySavePlayerReport(DotaStatsPlayerReport report)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            if (report.ReporterAccountId == 0 || report.TargetAccountId == 0)
            {
                return false;
            }

            EnsureProfileLocked(connection, report.ReporterSteamId, report.ReporterAccountId, string.Empty);
            var reasonsCsv = Csv(report.ReportReasons);
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO reports (
                    reporter_steam_id, reporter_account_id, target_account_id, lobby_id, report_flags,
                    report_reasons_csv, comment, game_time, debug_slot, debug_match_id, created_at
                )
                VALUES (
                    $reporter_steam_id, $reporter_account_id, $target_account_id, $lobby_id, $report_flags,
                    $report_reasons_csv, $comment, $game_time, $debug_slot, $debug_match_id, $created_at
                )
                """;
            Add(command, "$reporter_steam_id", report.ReporterSteamId);
            Add(command, "$reporter_account_id", report.ReporterAccountId);
            Add(command, "$target_account_id", report.TargetAccountId);
            Add(command, "$lobby_id", report.LobbyId);
            Add(command, "$report_flags", report.ReportFlags);
            Add(command, "$report_reasons_csv", reasonsCsv);
            Add(command, "$comment", report.Comment ?? string.Empty);
            Add(command, "$game_time", report.GameTime);
            Add(command, "$debug_slot", report.DebugSlot);
            Add(command, "$debug_match_id", report.DebugMatchId);
            Add(command, "$created_at", Now());
            command.ExecuteNonQuery();
            return true;
        }
    }

    public DotaStatsReporterUpdateSummary GetReporterUpdates(uint reporterAccountId, int limit = 50)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            var totalReports = (uint)ScalarLong(
                connection,
                null,
                "SELECT COUNT(*) FROM reports WHERE reporter_account_id = $reporter_account_id",
                ("$reporter_account_id", reporterAccountId));

            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT
                    CASE WHEN r.debug_match_id != 0 THEN r.debug_match_id ELSE r.lobby_id END AS match_id,
                    COALESCE(mp.hero_id, 0) AS hero_id,
                    r.report_reasons_csv,
                    r.report_flags,
                    r.created_at
                FROM reports r
                LEFT JOIN match_players mp ON mp.match_id = r.debug_match_id AND mp.account_id = r.target_account_id
                WHERE r.reporter_account_id = $reporter_account_id
                  AND NOT EXISTS (
                      SELECT 1
                      FROM reporter_update_acks a
                      WHERE a.account_id = r.reporter_account_id
                        AND a.match_id = CASE WHEN r.debug_match_id != 0 THEN r.debug_match_id ELSE r.lobby_id END
                  )
                ORDER BY r.created_at DESC, r.id DESC
                LIMIT $limit
                """;
            Add(command, "$reporter_account_id", reporterAccountId);
            Add(command, "$limit", Math.Clamp(limit, 1, 200));

            var updates = new List<DotaStatsReporterUpdate>();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                updates.Add(new DotaStatsReporterUpdate
                {
                    MatchId = unchecked((ulong)reader.GetInt64(0)),
                    HeroId = unchecked((uint)reader.GetInt64(1)),
                    ReportReason = FirstCsvUInt32(reader.GetString(2), unchecked((uint)reader.GetInt64(3))),
                    Timestamp = unchecked((uint)reader.GetInt64(4))
                });
            }

            return new DotaStatsReporterUpdateSummary
            {
                Updates = updates,
                NumReported = totalReports,
                NumNoActionTaken = totalReports
            };
        }
    }

    public bool AcknowledgeReporterUpdates(uint accountId, IEnumerable<ulong> matchIds)
    {
        var normalized = matchIds.Where(matchId => matchId != 0).Distinct().ToList();
        if (normalized.Count == 0)
        {
            return true;
        }

        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            foreach (var matchId in normalized)
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = """
                    INSERT INTO reporter_update_acks (account_id, match_id, acknowledged_at)
                    VALUES ($account_id, $match_id, $acknowledged_at)
                    ON CONFLICT(account_id, match_id) DO UPDATE SET acknowledged_at = excluded.acknowledged_at
                    """;
                Add(command, "$account_id", accountId);
                Add(command, "$match_id", matchId);
                Add(command, "$acknowledged_at", Now());
                command.ExecuteNonQuery();
            }

            transaction.Commit();
            return true;
        }
    }

    public bool RecordMatchSignOutPermission(DotaStatsMatchSignOutPermissionAudit request)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO match_signout_permission_requests (
                    server_steam_id, server_version, local_attempt, total_attempt, seconds_waited,
                    permission_granted, abandon_signout, retry_delay_seconds, created_at
                )
                VALUES (
                    $server_steam_id, $server_version, $local_attempt, $total_attempt, $seconds_waited,
                    $permission_granted, $abandon_signout, $retry_delay_seconds, $created_at
                )
                """;
            Add(command, "$server_steam_id", request.ServerSteamId);
            Add(command, "$server_version", request.ServerVersion);
            Add(command, "$local_attempt", request.LocalAttempt);
            Add(command, "$total_attempt", request.TotalAttempt);
            Add(command, "$seconds_waited", request.SecondsWaited);
            Add(command, "$permission_granted", request.PermissionGranted ? 1 : 0);
            Add(command, "$abandon_signout", request.AbandonSignout ? 1 : 0);
            Add(command, "$retry_delay_seconds", request.RetryDelaySeconds);
            Add(command, "$created_at", Now());
            command.ExecuteNonQuery();
            return true;
        }
    }

    public bool SetMatchHistoryAccess(ulong steamId, uint accountId, bool allow)
    {
        if (accountId == 0)
        {
            return false;
        }

        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, steamId, accountId, string.Empty);
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO match_history_access (account_id, steam_id, allow_3rd_party, updated_at)
                VALUES ($account_id, $steam_id, $allow_3rd_party, $updated_at)
                ON CONFLICT(account_id) DO UPDATE SET
                    steam_id = excluded.steam_id,
                    allow_3rd_party = excluded.allow_3rd_party,
                    updated_at = excluded.updated_at
                """;
            Add(command, "$account_id", accountId);
            Add(command, "$steam_id", steamId);
            Add(command, "$allow_3rd_party", allow ? 1 : 0);
            Add(command, "$updated_at", Now());
            command.ExecuteNonQuery();
            return true;
        }
    }

    public bool RecordServerStatusRequest(ulong serverSteamId, uint response)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO server_status_requests (server_steam_id, response, created_at)
                VALUES ($server_steam_id, $response, $created_at)
                """;
            Add(command, "$server_steam_id", serverSteamId);
            Add(command, "$response", response);
            Add(command, "$created_at", Now());
            command.ExecuteNonQuery();
            return true;
        }
    }

    public bool RecordLeaverDetected(DotaStatsLeaverEvent leaver)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = """
                    INSERT INTO match_leaver_events (
                        server_steam_id, leaver_steam_id, leaver_account_id, leaver_status, lobby_state,
                        game_state, leaver_detected, first_blood_happened, discard_match_results,
                        mass_disconnect, server_cluster, disconnect_reason, created_at
                    )
                    VALUES (
                        $server_steam_id, $leaver_steam_id, $leaver_account_id, $leaver_status, $lobby_state,
                        $game_state, $leaver_detected, $first_blood_happened, $discard_match_results,
                        $mass_disconnect, $server_cluster, $disconnect_reason, $created_at
                    )
                    """;
                Add(command, "$server_steam_id", leaver.ServerSteamId);
                Add(command, "$leaver_steam_id", leaver.LeaverSteamId);
                Add(command, "$leaver_account_id", leaver.LeaverAccountId);
                Add(command, "$leaver_status", leaver.LeaverStatus);
                Add(command, "$lobby_state", leaver.LobbyState);
                Add(command, "$game_state", leaver.GameState);
                Add(command, "$leaver_detected", leaver.LeaverDetected ? 1 : 0);
                Add(command, "$first_blood_happened", leaver.FirstBloodHappened ? 1 : 0);
                Add(command, "$discard_match_results", leaver.DiscardMatchResults ? 1 : 0);
                Add(command, "$mass_disconnect", leaver.MassDisconnect ? 1 : 0);
                Add(command, "$server_cluster", leaver.ServerCluster);
                Add(command, "$disconnect_reason", leaver.DisconnectReason);
                Add(command, "$created_at", Now());
                command.ExecuteNonQuery();
            }

            if (leaver.LeaverSteamId != 0 && leaver.LeaverStatus != 0)
            {
                using var update = connection.CreateCommand();
                update.Transaction = transaction;
                update.CommandText = """
                    UPDATE match_players
                    SET leaver_status = $leaver_status
                    WHERE steam_id = $leaver_steam_id
                      AND match_id = (
                          SELECT MAX(match_id)
                          FROM match_players
                          WHERE steam_id = $leaver_steam_id
                      )
                    """;
                Add(update, "$leaver_status", leaver.LeaverStatus);
                Add(update, "$leaver_steam_id", leaver.LeaverSteamId);
                update.ExecuteNonQuery();
            }

            transaction.Commit();
            return true;
        }
    }

    public bool RecordRealtimeStats(DotaStatsRealtimeStatsSnapshot snapshot)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO match_realtime_stats (
                    server_steam_id, match_id, timestamp, game_time, game_state, game_mode, lobby_type,
                    league_id, radiant_score, dire_score, player_count, building_count, delta_frame,
                    payload_size, created_at
                )
                VALUES (
                    $server_steam_id, $match_id, $timestamp, $game_time, $game_state, $game_mode, $lobby_type,
                    $league_id, $radiant_score, $dire_score, $player_count, $building_count, $delta_frame,
                    $payload_size, $created_at
                )
                """;
            AddRealtimeStatsParameters(command, snapshot);
            command.ExecuteNonQuery();
            return true;
        }
    }

    public bool RecordMatchStateHistory(DotaStatsMatchStateHistorySnapshot history)
    {
        if (history.MatchId == 0)
        {
            return false;
        }

        lock (_sync)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO match_state_history (
                    match_id, radiant_won, mmr, state_count, last_game_time,
                    radiant_kills, dire_kills, payload_size, created_at
                )
                VALUES (
                    $match_id, $radiant_won, $mmr, $state_count, $last_game_time,
                    $radiant_kills, $dire_kills, $payload_size, $created_at
                )
                ON CONFLICT(match_id) DO UPDATE SET
                    radiant_won = excluded.radiant_won,
                    mmr = excluded.mmr,
                    state_count = excluded.state_count,
                    last_game_time = excluded.last_game_time,
                    radiant_kills = excluded.radiant_kills,
                    dire_kills = excluded.dire_kills,
                    payload_size = excluded.payload_size,
                    created_at = excluded.created_at
                """;
            Add(command, "$match_id", history.MatchId);
            Add(command, "$radiant_won", history.RadiantWon ? 1 : 0);
            Add(command, "$mmr", history.Mmr);
            Add(command, "$state_count", history.StateCount);
            Add(command, "$last_game_time", history.LastGameTime);
            Add(command, "$radiant_kills", history.RadiantKills);
            Add(command, "$dire_kills", history.DireKills);
            Add(command, "$payload_size", history.PayloadSize);
            Add(command, "$created_at", Now());
            command.ExecuteNonQuery();
            return true;
        }
    }

    public bool RecordSpectatorCount(ulong serverSteamId, uint spectatorCount)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO match_spectator_counts (server_steam_id, spectator_count, created_at)
                VALUES ($server_steam_id, $spectator_count, $created_at)
                """;
            Add(command, "$server_steam_id", serverSteamId);
            Add(command, "$spectator_count", spectatorCount);
            Add(command, "$created_at", Now());
            command.ExecuteNonQuery();
            return true;
        }
    }

    public bool RecordLiveScoreboard(DotaStatsLiveScoreboardSnapshot snapshot)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO live_scoreboard_updates (
                    server_steam_id, match_id, tournament_id, tournament_game_id, duration, hltv_delay,
                    league_id, radiant_score, dire_score, player_count, roshan_respawn_timer,
                    payload_size, created_at
                )
                VALUES (
                    $server_steam_id, $match_id, $tournament_id, $tournament_game_id, $duration, $hltv_delay,
                    $league_id, $radiant_score, $dire_score, $player_count, $roshan_respawn_timer,
                    $payload_size, $created_at
                )
                """;
            Add(command, "$server_steam_id", snapshot.ServerSteamId);
            Add(command, "$match_id", snapshot.MatchId);
            Add(command, "$tournament_id", snapshot.TournamentId);
            Add(command, "$tournament_game_id", snapshot.TournamentGameId);
            Add(command, "$duration", snapshot.Duration);
            Add(command, "$hltv_delay", snapshot.HltvDelay);
            Add(command, "$league_id", snapshot.LeagueId);
            Add(command, "$radiant_score", snapshot.RadiantScore);
            Add(command, "$dire_score", snapshot.DireScore);
            Add(command, "$player_count", snapshot.PlayerCount);
            Add(command, "$roshan_respawn_timer", snapshot.RoshanRespawnTimer);
            Add(command, "$payload_size", snapshot.PayloadSize);
            Add(command, "$created_at", Now());
            command.ExecuteNonQuery();
            return true;
        }
    }

    public DotaStatsConduct GetConduct(uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            var commendCount = (uint)ScalarLong(connection, null, "SELECT COUNT(*) FROM reports WHERE target_account_id = $account_id AND report_flags = 3840", ("$account_id", accountId));
            var commReports = (uint)ScalarLong(connection, null, "SELECT COUNT(*) FROM reports WHERE target_account_id = $account_id AND report_flags = 2", ("$account_id", accountId));
            var reports = (uint)ScalarLong(connection, null, "SELECT COUNT(*) FROM reports WHERE target_account_id = $account_id AND report_flags <> 3840", ("$account_id", accountId));
            var abandoned = (uint)ScalarLong(connection, null, "SELECT COUNT(*) FROM match_players WHERE account_id = $account_id AND leaver_status <> 0", ("$account_id", accountId));
            var matchCount = (uint)ScalarLong(connection, null, "SELECT COUNT(*) FROM match_players WHERE account_id = $account_id", ("$account_id", accountId));
            var latestMatchId = (ulong)ScalarLong(connection, null, "SELECT COALESCE(MAX(match_id), 0) FROM match_players WHERE account_id = $account_id", ("$account_id", accountId));
            var rawScore = Math.Max(0, 10000 - (int)reports * 500 - (int)abandoned * 1000 + (int)commendCount * 100);

            return new DotaStatsConduct
            {
                AccountId = accountId,
                MatchId = latestMatchId,
                CommendCount = commendCount,
                CommsReports = commReports,
                MatchesAbandoned = abandoned,
                ReportsCount = reports,
                MatchesInReport = matchCount,
                MatchesClean = matchCount > reports ? matchCount - reports : 0,
                MatchesReported = reports,
                ReportsParties = reports,
                RawBehaviorScore = (uint)Math.Clamp(rawScore, 0, 10000),
                OldRawBehaviorScore = 10000,
                Date = Now(),
                BehaviorRating = rawScore >= 7000 ? 0u : rawScore >= 4000 ? 1u : 2u
            };
        }
    }

    public IReadOnlyList<DotaStatsQuestProgress> GetQuestProgress(uint accountId, IReadOnlyList<uint> questIds)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);

            var requested = questIds.Where(id => id != 0).Distinct().ToHashSet();
            var byQuest = new Dictionary<uint, DotaStatsQuestProgress>();
            using var command = connection.CreateCommand();
            command.CommandText = requested.Count == 0
                ? """
                    SELECT quest_id, challenge_id, time_completed, attempts, hero_id, template_id, quest_rank
                    FROM quest_progress
                    WHERE account_id = $account_id
                    ORDER BY quest_id, challenge_id
                    """
                : $"""
                    SELECT quest_id, challenge_id, time_completed, attempts, hero_id, template_id, quest_rank
                    FROM quest_progress
                    WHERE account_id = $account_id
                      AND quest_id IN ({string.Join(", ", requested.Select((_, index) => "$quest_" + index.ToString(System.Globalization.CultureInfo.InvariantCulture)))})
                    ORDER BY quest_id, challenge_id
                    """;
            Add(command, "$account_id", accountId);
            var parameterIndex = 0;
            foreach (var questId in requested)
            {
                Add(command, "$quest_" + parameterIndex.ToString(System.Globalization.CultureInfo.InvariantCulture), questId);
                parameterIndex++;
            }

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var questId = U32(reader, "quest_id");
                if (!byQuest.TryGetValue(questId, out var progress))
                {
                    progress = new DotaStatsQuestProgress { QuestId = questId };
                    byQuest.Add(questId, progress);
                }

                progress.CompletedChallenges.Add(new DotaStatsQuestChallenge
                {
                    ChallengeId = U32(reader, "challenge_id"),
                    TimeCompleted = U32(reader, "time_completed"),
                    Attempts = U32(reader, "attempts"),
                    HeroId = U32(reader, "hero_id"),
                    TemplateId = U32(reader, "template_id"),
                    QuestRank = U32(reader, "quest_rank")
                });
            }

            foreach (var questId in requested)
            {
                byQuest.TryAdd(questId, new DotaStatsQuestProgress { QuestId = questId });
            }

            return byQuest.Values.OrderBy(quest => quest.QuestId).ToList();
        }
    }

    public DotaStatsPeriodicResource GetPeriodicResource(uint accountId, uint resourceId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT account_id, resource_id, resource_max, resource_used
                FROM periodic_resources
                WHERE account_id = $account_id AND resource_id = $resource_id
                """;
            Add(command, "$account_id", accountId);
            Add(command, "$resource_id", resourceId);
            using var reader = command.ExecuteReader();
            return reader.Read()
                ? new DotaStatsPeriodicResource
                {
                    AccountId = U32(reader, "account_id"),
                    ResourceId = U32(reader, "resource_id"),
                    ResourceMax = U32(reader, "resource_max"),
                    ResourceUsed = U32(reader, "resource_used")
                }
                : new DotaStatsPeriodicResource { AccountId = accountId, ResourceId = resourceId };
        }
    }

    public IReadOnlyList<DotaStatsHeroSticker> GetHeroStickers(uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            var result = new List<DotaStatsHeroSticker>();
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT hero_id, item_def_id, quality, source_item_id
                FROM hero_stickers
                WHERE account_id = $account_id
                ORDER BY hero_id
                """;
            Add(command, "$account_id", accountId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new DotaStatsHeroSticker
                {
                    HeroId = U32(reader, "hero_id"),
                    ItemDefId = U32(reader, "item_def_id"),
                    Quality = U32(reader, "quality"),
                    SourceItemId = U64(reader, "source_item_id")
                });
            }

            return result;
        }
    }

    public bool SetHeroSticker(uint accountId, uint heroId, ulong itemId)
    {
        if (accountId == 0 || heroId == 0)
        {
            return false;
        }

        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            if (itemId == 0)
            {
                using var delete = connection.CreateCommand();
                delete.CommandText = "DELETE FROM hero_stickers WHERE account_id = $account_id AND hero_id = $hero_id";
                Add(delete, "$account_id", accountId);
                Add(delete, "$hero_id", heroId);
                delete.ExecuteNonQuery();
                return true;
            }

            var itemDefId = (uint)Math.Min(itemId & 0xffffffffUL, uint.MaxValue);
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO hero_stickers (account_id, hero_id, item_def_id, quality, source_item_id, updated_at)
                VALUES ($account_id, $hero_id, $item_def_id, $quality, $source_item_id, $updated_at)
                ON CONFLICT(account_id, hero_id) DO UPDATE SET
                    item_def_id = excluded.item_def_id,
                    quality = excluded.quality,
                    source_item_id = excluded.source_item_id,
                    updated_at = excluded.updated_at
                """;
            Add(command, "$account_id", accountId);
            Add(command, "$hero_id", heroId);
            Add(command, "$item_def_id", itemDefId);
            Add(command, "$quality", 0);
            Add(command, "$source_item_id", itemId);
            Add(command, "$updated_at", Now());
            command.ExecuteNonQuery();
            return true;
        }
    }

    public DotaStatsOverworldState GetOverworldState(uint accountId, uint overworldId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT overworld_id, current_node_id, last_related_hero_id, overworld_version
                FROM overworld_state
                WHERE account_id = $account_id AND overworld_id = $overworld_id
                """;
            Add(command, "$account_id", accountId);
            Add(command, "$overworld_id", overworldId);
            using var reader = command.ExecuteReader();
            return reader.Read()
                ? new DotaStatsOverworldState
                {
                    OverworldId = U32(reader, "overworld_id"),
                    CurrentNodeId = U32(reader, "current_node_id"),
                    LastRelatedHeroId = U32(reader, "last_related_hero_id"),
                    OverworldVersion = U32(reader, "overworld_version")
                }
                : new DotaStatsOverworldState { OverworldId = overworldId, OverworldVersion = 1 };
        }
    }

    public DotaStatsMonsterHunterState GetMonsterHunterState(uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT unlocked_count
                FROM monster_hunter_state
                WHERE account_id = $account_id
                """;
            Add(command, "$account_id", accountId);
            using var reader = command.ExecuteReader();
            return reader.Read()
                ? new DotaStatsMonsterHunterState { UnlockedCount = U32(reader, "unlocked_count") }
                : new DotaStatsMonsterHunterState();
        }
    }

    public bool SaveSocialMatchComment(ulong matchId, uint accountId, string personaName, string comment)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            if (ReadMatch(connection, matchId) == null)
            {
                return false;
            }

            EnsureProfileLocked(connection, 0, accountId, personaName);
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO match_comments (match_id, account_id, persona_name, comment, timestamp)
                VALUES ($match_id, $account_id, $persona_name, $comment, $timestamp)
                ON CONFLICT(match_id, account_id) DO UPDATE SET
                    persona_name = excluded.persona_name,
                    comment = excluded.comment,
                    timestamp = excluded.timestamp
                """;
            Add(command, "$match_id", matchId);
            Add(command, "$account_id", accountId);
            Add(command, "$persona_name", personaName ?? string.Empty);
            Add(command, "$comment", comment ?? string.Empty);
            Add(command, "$timestamp", Now());
            command.ExecuteNonQuery();
            UpdateSocialFeedCommentCount(connection, matchId);
            return true;
        }
    }

    public DotaStatsEmoticonAccess GetEmoticonAccess(uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT account_id, unlocked_mask, updated_at
                FROM emoticon_access
                WHERE account_id = $account_id
                """;
            Add(command, "$account_id", accountId);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new DotaStatsEmoticonAccess
                {
                    AccountId = U32(reader, "account_id"),
                    UnlockedMask = Blob(reader, "unlocked_mask"),
                    UpdatedAt = U32(reader, "updated_at")
                };
            }

            var access = new DotaStatsEmoticonAccess
            {
                AccountId = accountId,
                UnlockedMask = CreateDefaultEmoticonAccessMask(),
                UpdatedAt = Now()
            };
            SaveEmoticonAccessLocked(connection, access);
            return access;
        }
    }

    public bool SaveEmoticonAccess(uint accountId, byte[] unlockedMask)
    {
        if (accountId == 0 || unlockedMask == null)
        {
            return false;
        }

        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            SaveEmoticonAccessLocked(connection, new DotaStatsEmoticonAccess
            {
                AccountId = accountId,
                UnlockedMask = unlockedMask.ToArray(),
                UpdatedAt = Now()
            });
            return true;
        }
    }

    public IReadOnlyList<DotaStatsComment> GetSocialMatchComments(ulong matchId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            var result = new List<DotaStatsComment>();
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT match_id, account_id, persona_name, comment, timestamp
                FROM match_comments
                WHERE match_id = $match_id
                ORDER BY timestamp ASC
                """;
            Add(command, "$match_id", matchId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new DotaStatsComment
                {
                    MatchId = U64(reader, "match_id"),
                    AccountId = U32(reader, "account_id"),
                    PersonaName = Str(reader, "persona_name"),
                    Comment = Str(reader, "comment"),
                    Timestamp = U32(reader, "timestamp")
                });
            }

            return result;
        }
    }

    public IReadOnlyList<DotaStatsSocialFeedEvent> GetSocialFeed(uint accountId, bool selfOnly)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            EnsureProfileLocked(connection, 0, accountId, string.Empty);
            var result = new List<DotaStatsSocialFeedEvent>();
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT feed_event_id, account_id, timestamp, comment_count, event_type, event_sub_type,
                       param_big_int_1, param_int_1, param_int_2, param_int_3, param_string
                FROM social_feed
                WHERE ($self_only = 0 OR account_id = $account_id)
                ORDER BY timestamp DESC
                LIMIT 50
                """;
            Add(command, "$account_id", accountId);
            Add(command, "$self_only", selfOnly ? 1 : 0);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new DotaStatsSocialFeedEvent
                {
                    FeedEventId = U64(reader, "feed_event_id"),
                    AccountId = U32(reader, "account_id"),
                    Timestamp = U32(reader, "timestamp"),
                    CommentCount = U32(reader, "comment_count"),
                    EventType = U32(reader, "event_type"),
                    EventSubType = U32(reader, "event_sub_type"),
                    ParamBigInt1 = U64(reader, "param_big_int_1"),
                    ParamInt1 = U32(reader, "param_int_1"),
                    ParamInt2 = U32(reader, "param_int_2"),
                    ParamInt3 = U32(reader, "param_int_3"),
                    ParamString = Str(reader, "param_string")
                });
            }

            return result;
        }
    }

    public IReadOnlyList<DotaStatsComment> GetSocialFeedComments(ulong feedEventId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            var result = new List<DotaStatsComment>();
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT feed_event_id AS match_id, commenter_account_id AS account_id, '' AS persona_name, comment_text AS comment, timestamp
                FROM social_feed_comments
                WHERE feed_event_id = $feed_event_id
                ORDER BY timestamp ASC
                """;
            Add(command, "$feed_event_id", feedEventId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new DotaStatsComment
                {
                    MatchId = U64(reader, "match_id"),
                    AccountId = U32(reader, "account_id"),
                    PersonaName = Str(reader, "persona_name"),
                    Comment = Str(reader, "comment"),
                    Timestamp = U32(reader, "timestamp")
                });
            }

            return result;
        }
    }

    public bool SaveSocialFeedComment(ulong feedEventId, uint accountId, string comment)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            using var transaction = connection.BeginTransaction();
            var exists = ScalarLong(connection, transaction, "SELECT COUNT(*) FROM social_feed WHERE feed_event_id = $feed_event_id", ("$feed_event_id", feedEventId)) > 0;
            if (!exists)
            {
                transaction.Rollback();
                return false;
            }

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = """
                    INSERT INTO social_feed_comments (feed_event_id, commenter_account_id, timestamp, comment_text)
                    VALUES ($feed_event_id, $account_id, $timestamp, $comment)
                    """;
                Add(command, "$feed_event_id", feedEventId);
                Add(command, "$account_id", accountId);
                Add(command, "$timestamp", Now());
                Add(command, "$comment", comment ?? string.Empty);
                command.ExecuteNonQuery();
            }

            using (var update = connection.CreateCommand())
            {
                update.Transaction = transaction;
                update.CommandText = "UPDATE social_feed SET comment_count = comment_count + 1 WHERE feed_event_id = $feed_event_id";
                Add(update, "$feed_event_id", feedEventId);
                update.ExecuteNonQuery();
            }

            transaction.Commit();
            return true;
        }
    }

    public DotaStatsVoteSummary GetMatchVotes(ulong matchId, uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            var summary = new DotaStatsVoteSummary();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT account_id, vote FROM match_votes WHERE match_id = $match_id";
            Add(command, "$match_id", matchId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var voteAccount = U32(reader, "account_id");
                var vote = U32(reader, "vote");
                if (voteAccount == accountId)
                {
                    summary.Vote = vote;
                }

                if (vote == 1)
                {
                    summary.PositiveVotes++;
                }
                else if (vote == 2)
                {
                    summary.NegativeVotes++;
                }
            }

            summary.Success = ReadMatch(connection, matchId) != null;
            return summary;
        }
    }

    public IReadOnlyList<DotaStatsTeammate> GetTeammateStats(uint accountId)
    {
        lock (_sync)
        {
            using var connection = OpenConnection();
            var result = new List<DotaStatsTeammate>();
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT other.account_id,
                       COUNT(*) AS games,
                       SUM(CASE WHEN self.winner = 1 THEN 1 ELSE 0 END) AS wins,
                       MAX(m.start_time) AS most_recent_game_timestamp,
                       MAX(m.match_id) AS most_recent_game_match_id
                FROM match_players self
                JOIN match_players other ON other.match_id = self.match_id AND other.account_id <> self.account_id AND other.team = self.team
                JOIN matches m ON m.match_id = self.match_id
                WHERE self.account_id = $account_id
                GROUP BY other.account_id
                ORDER BY most_recent_game_timestamp DESC
                LIMIT 32
                """;
            Add(command, "$account_id", accountId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new DotaStatsTeammate
                {
                    AccountId = U32(reader, "account_id"),
                    Games = U32(reader, "games"),
                    Wins = U32(reader, "wins"),
                    MostRecentGameTimestamp = U32(reader, "most_recent_game_timestamp"),
                    MostRecentGameMatchId = U64(reader, "most_recent_game_match_id")
                });
            }

            return result;
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
            CREATE TABLE IF NOT EXISTS profiles (
                account_id INTEGER PRIMARY KEY,
                steam_id INTEGER NOT NULL,
                persona_name TEXT NOT NULL,
                rank_tier INTEGER NOT NULL DEFAULT 0,
                rank_tier_score INTEGER NOT NULL DEFAULT 0,
                leaderboard_rank INTEGER NOT NULL DEFAULT 0,
                rank_score INTEGER NOT NULL DEFAULT 0,
                event_points INTEGER NOT NULL DEFAULT 0,
                badge_points INTEGER NOT NULL DEFAULT 0,
                level INTEGER NOT NULL DEFAULT 1,
                xp INTEGER NOT NULL DEFAULT 0,
                wins INTEGER NOT NULL DEFAULT 0,
                losses INTEGER NOT NULL DEFAULT 0,
                background_item_id INTEGER NOT NULL DEFAULT 0,
                updated_at INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS profile_slots (
                account_id INTEGER NOT NULL,
                slot_id INTEGER NOT NULL,
                slot_type INTEGER NOT NULL,
                slot_value INTEGER NOT NULL,
                PRIMARY KEY (account_id, slot_id)
            );

            CREATE TABLE IF NOT EXISTS featured_heroes (
                account_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                hero_id INTEGER NOT NULL,
                PRIMARY KEY (account_id, position)
            );

            CREATE TABLE IF NOT EXISTS trophies (
                account_id INTEGER NOT NULL,
                steam_id INTEGER NOT NULL,
                trophy_id INTEGER NOT NULL,
                trophy_score INTEGER NOT NULL,
                last_updated INTEGER NOT NULL,
                PRIMARY KEY (account_id, trophy_id)
            );

            CREATE TABLE IF NOT EXISTS all_hero_order (
                account_id INTEGER PRIMARY KEY,
                hero_ids_csv TEXT NOT NULL,
                current_hero_id INTEGER NOT NULL,
                next_hero_id INTEGER NOT NULL,
                previous_hero_id INTEGER NOT NULL,
                start_hero_id INTEGER NOT NULL,
                laps_completed INTEGER NOT NULL,
                current_hero_games INTEGER NOT NULL,
                current_lap_started INTEGER NOT NULL,
                current_lap_games INTEGER NOT NULL,
                best_lap_games INTEGER NOT NULL,
                best_lap_time INTEGER NOT NULL,
                lap_heroes_completed INTEGER NOT NULL,
                lap_heroes_remaining INTEGER NOT NULL,
                previous_hero_games INTEGER NOT NULL,
                profile_name TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS matches (
                match_id INTEGER PRIMARY KEY,
                owner_steam_id INTEGER NOT NULL,
                server_steam_id INTEGER NOT NULL,
                start_time INTEGER NOT NULL,
                duration INTEGER NOT NULL,
                game_mode INTEGER NOT NULL,
                lobby_type INTEGER NOT NULL,
                good_guys_win INTEGER NOT NULL,
                match_flags INTEGER NOT NULL,
                radiant_score INTEGER NOT NULL,
                dire_score INTEGER NOT NULL,
                cluster INTEGER NOT NULL,
                first_blood_time INTEGER NOT NULL,
                raw_signout_base64 TEXT NOT NULL,
                created_at INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS match_players (
                match_id INTEGER NOT NULL,
                account_id INTEGER NOT NULL,
                steam_id INTEGER NOT NULL,
                persona_name TEXT NOT NULL,
                team INTEGER NOT NULL,
                player_slot INTEGER NOT NULL,
                hero_id INTEGER NOT NULL,
                kills INTEGER NOT NULL,
                deaths INTEGER NOT NULL,
                assists INTEGER NOT NULL,
                winner INTEGER NOT NULL,
                good_guys INTEGER NOT NULL,
                gold INTEGER NOT NULL,
                gold_spent INTEGER NOT NULL,
                gpm INTEGER NOT NULL,
                xpm INTEGER NOT NULL,
                last_hits INTEGER NOT NULL,
                denies INTEGER NOT NULL,
                hero_damage INTEGER NOT NULL,
                tower_damage INTEGER NOT NULL,
                hero_healing INTEGER NOT NULL,
                level INTEGER NOT NULL,
                net_worth REAL NOT NULL,
                support_gold INTEGER NOT NULL,
                claimed_farm_gold INTEGER NOT NULL,
                bounty_runes INTEGER NOT NULL,
                outposts_captured INTEGER NOT NULL,
                selected_facet INTEGER NOT NULL,
                leaver_status INTEGER NOT NULL,
                items_csv TEXT NOT NULL,
                talents_csv TEXT NOT NULL,
                rampages INTEGER NOT NULL,
                triple_kills INTEGER NOT NULL,
                first_blood_claimed INTEGER NOT NULL,
                first_blood_given INTEGER NOT NULL,
                couriers_killed INTEGER NOT NULL,
                aegises_snatched INTEGER NOT NULL,
                cheeses_eaten INTEGER NOT NULL,
                creeps_stacked INTEGER NOT NULL,
                fight_score REAL NOT NULL,
                farm_score REAL NOT NULL,
                support_score REAL NOT NULL,
                push_score REAL NOT NULL,
                damage REAL NOT NULL,
                heals REAL NOT NULL,
                rapiers_purchased INTEGER NOT NULL,
                mvp INTEGER NOT NULL DEFAULT 0,
                PRIMARY KEY (match_id, account_id)
            );

            CREATE TABLE IF NOT EXISTS mvp_votes (
                match_id INTEGER NOT NULL,
                voter_account_id INTEGER NOT NULL,
                voted_account_id INTEGER NOT NULL,
                created_at INTEGER NOT NULL,
                PRIMARY KEY (match_id, voter_account_id)
            );

            CREATE TABLE IF NOT EXISTS global_stats (
                account_id INTEGER PRIMARY KEY,
                steam_id INTEGER NOT NULL,
                persona_name TEXT NOT NULL,
                kills INTEGER NOT NULL,
                deaths INTEGER NOT NULL,
                assists INTEGER NOT NULL,
                media_kills REAL NOT NULL,
                media_deaths REAL NOT NULL,
                media_assists REAL NOT NULL,
                games_won INTEGER NOT NULL,
                games_lost INTEGER NOT NULL,
                best_gpm INTEGER NOT NULL,
                best_xpm INTEGER NOT NULL,
                media_gpm INTEGER NOT NULL,
                media_xpm INTEGER NOT NULL,
                last_hits INTEGER NOT NULL,
                media_last_hits INTEGER NOT NULL,
                denies INTEGER NOT NULL,
                rampages INTEGER NOT NULL,
                triple_kills INTEGER NOT NULL,
                couriers_killed INTEGER NOT NULL,
                first_bloods_given INTEGER NOT NULL,
                first_bloods_received INTEGER NOT NULL,
                cheeses_eaten INTEGER NOT NULL,
                aegises_snatched INTEGER NOT NULL,
                creeps_stacked INTEGER NOT NULL,
                rapiers_purchased INTEGER NOT NULL,
                mean_networth REAL NOT NULL,
                mean_damage REAL NOT NULL,
                mean_heals REAL NOT NULL,
                team_performance REAL NOT NULL,
                avg_fight_score REAL NOT NULL,
                avg_farm_score REAL NOT NULL,
                avg_support_score REAL NOT NULL,
                avg_push_score REAL NOT NULL,
                played_hero_count INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS hero_stats (
                account_id INTEGER NOT NULL,
                steam_id INTEGER NOT NULL,
                hero_id INTEGER NOT NULL,
                wins INTEGER NOT NULL,
                losses INTEGER NOT NULL,
                win_streak INTEGER NOT NULL,
                best_win_streak INTEGER NOT NULL,
                avg_kills REAL NOT NULL,
                avg_deaths REAL NOT NULL,
                avg_assists REAL NOT NULL,
                avg_gpm REAL NOT NULL,
                avg_xpm REAL NOT NULL,
                best_kills INTEGER NOT NULL,
                best_assists INTEGER NOT NULL,
                best_gpm INTEGER NOT NULL,
                best_xpm INTEGER NOT NULL,
                performance REAL NOT NULL,
                avg_lasthits REAL NOT NULL,
                avg_denies REAL NOT NULL,
                networth_peak INTEGER NOT NULL,
                lasthit_peak INTEGER NOT NULL,
                deny_peak INTEGER NOT NULL,
                damage_peak INTEGER NOT NULL,
                longest_game_peak INTEGER NOT NULL,
                healing_peak INTEGER NOT NULL,
                PRIMARY KEY (account_id, hero_id)
            );

            CREATE TABLE IF NOT EXISTS reports (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                reporter_steam_id INTEGER NOT NULL,
                reporter_account_id INTEGER NOT NULL,
                target_account_id INTEGER NOT NULL,
                lobby_id INTEGER NOT NULL,
                report_flags INTEGER NOT NULL,
                report_reasons_csv TEXT NOT NULL DEFAULT '',
                comment TEXT NOT NULL,
                game_time INTEGER NOT NULL DEFAULT 0,
                debug_slot INTEGER NOT NULL DEFAULT 0,
                debug_match_id INTEGER NOT NULL DEFAULT 0,
                created_at INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS reporter_update_acks (
                account_id INTEGER NOT NULL,
                match_id INTEGER NOT NULL,
                acknowledged_at INTEGER NOT NULL,
                PRIMARY KEY (account_id, match_id)
            );

            CREATE TABLE IF NOT EXISTS match_signout_permission_requests (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                server_steam_id INTEGER NOT NULL,
                server_version INTEGER NOT NULL,
                local_attempt INTEGER NOT NULL,
                total_attempt INTEGER NOT NULL,
                seconds_waited INTEGER NOT NULL,
                permission_granted INTEGER NOT NULL,
                abandon_signout INTEGER NOT NULL,
                retry_delay_seconds INTEGER NOT NULL,
                created_at INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS match_history_access (
                account_id INTEGER PRIMARY KEY,
                steam_id INTEGER NOT NULL,
                allow_3rd_party INTEGER NOT NULL,
                updated_at INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS server_status_requests (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                server_steam_id INTEGER NOT NULL,
                response INTEGER NOT NULL,
                created_at INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS match_leaver_events (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                server_steam_id INTEGER NOT NULL,
                leaver_steam_id INTEGER NOT NULL,
                leaver_account_id INTEGER NOT NULL,
                leaver_status INTEGER NOT NULL,
                lobby_state INTEGER NOT NULL,
                game_state INTEGER NOT NULL,
                leaver_detected INTEGER NOT NULL,
                first_blood_happened INTEGER NOT NULL,
                discard_match_results INTEGER NOT NULL,
                mass_disconnect INTEGER NOT NULL,
                server_cluster INTEGER NOT NULL,
                disconnect_reason INTEGER NOT NULL,
                created_at INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS match_realtime_stats (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                server_steam_id INTEGER NOT NULL,
                match_id INTEGER NOT NULL,
                timestamp INTEGER NOT NULL,
                game_time INTEGER NOT NULL,
                game_state INTEGER NOT NULL,
                game_mode INTEGER NOT NULL,
                lobby_type INTEGER NOT NULL,
                league_id INTEGER NOT NULL,
                radiant_score INTEGER NOT NULL,
                dire_score INTEGER NOT NULL,
                player_count INTEGER NOT NULL,
                building_count INTEGER NOT NULL,
                delta_frame INTEGER NOT NULL,
                payload_size INTEGER NOT NULL,
                created_at INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS match_state_history (
                match_id INTEGER PRIMARY KEY,
                radiant_won INTEGER NOT NULL,
                mmr INTEGER NOT NULL,
                state_count INTEGER NOT NULL,
                last_game_time INTEGER NOT NULL,
                radiant_kills INTEGER NOT NULL,
                dire_kills INTEGER NOT NULL,
                payload_size INTEGER NOT NULL,
                created_at INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS match_spectator_counts (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                server_steam_id INTEGER NOT NULL,
                spectator_count INTEGER NOT NULL,
                created_at INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS live_scoreboard_updates (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                server_steam_id INTEGER NOT NULL,
                match_id INTEGER NOT NULL,
                tournament_id INTEGER NOT NULL,
                tournament_game_id INTEGER NOT NULL,
                duration INTEGER NOT NULL,
                hltv_delay INTEGER NOT NULL,
                league_id INTEGER NOT NULL,
                radiant_score INTEGER NOT NULL,
                dire_score INTEGER NOT NULL,
                player_count INTEGER NOT NULL,
                roshan_respawn_timer INTEGER NOT NULL,
                payload_size INTEGER NOT NULL,
                created_at INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS quest_progress (
                account_id INTEGER NOT NULL,
                quest_id INTEGER NOT NULL,
                challenge_id INTEGER NOT NULL,
                time_completed INTEGER NOT NULL DEFAULT 0,
                attempts INTEGER NOT NULL DEFAULT 0,
                hero_id INTEGER NOT NULL DEFAULT 0,
                template_id INTEGER NOT NULL DEFAULT 0,
                quest_rank INTEGER NOT NULL DEFAULT 0,
                updated_at INTEGER NOT NULL,
                PRIMARY KEY (account_id, quest_id, challenge_id)
            );

            CREATE TABLE IF NOT EXISTS periodic_resources (
                account_id INTEGER NOT NULL,
                resource_id INTEGER NOT NULL,
                resource_max INTEGER NOT NULL DEFAULT 0,
                resource_used INTEGER NOT NULL DEFAULT 0,
                updated_at INTEGER NOT NULL,
                PRIMARY KEY (account_id, resource_id)
            );

            CREATE TABLE IF NOT EXISTS hero_stickers (
                account_id INTEGER NOT NULL,
                hero_id INTEGER NOT NULL,
                item_def_id INTEGER NOT NULL DEFAULT 0,
                quality INTEGER NOT NULL DEFAULT 0,
                source_item_id INTEGER NOT NULL DEFAULT 0,
                updated_at INTEGER NOT NULL,
                PRIMARY KEY (account_id, hero_id)
            );

            CREATE TABLE IF NOT EXISTS overworld_state (
                account_id INTEGER NOT NULL,
                overworld_id INTEGER NOT NULL,
                current_node_id INTEGER NOT NULL DEFAULT 0,
                last_related_hero_id INTEGER NOT NULL DEFAULT 0,
                overworld_version INTEGER NOT NULL DEFAULT 1,
                updated_at INTEGER NOT NULL,
                PRIMARY KEY (account_id, overworld_id)
            );

            CREATE TABLE IF NOT EXISTS monster_hunter_state (
                account_id INTEGER PRIMARY KEY,
                unlocked_count INTEGER NOT NULL DEFAULT 0,
                updated_at INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS match_comments (
                match_id INTEGER NOT NULL,
                account_id INTEGER NOT NULL,
                persona_name TEXT NOT NULL,
                comment TEXT NOT NULL,
                timestamp INTEGER NOT NULL,
                PRIMARY KEY (match_id, account_id)
            );

            CREATE TABLE IF NOT EXISTS match_votes (
                match_id INTEGER NOT NULL,
                account_id INTEGER NOT NULL,
                vote INTEGER NOT NULL,
                timestamp INTEGER NOT NULL,
                PRIMARY KEY (match_id, account_id)
            );

            CREATE TABLE IF NOT EXISTS emoticon_access (
                account_id INTEGER PRIMARY KEY,
                unlocked_mask BLOB NOT NULL,
                updated_at INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS social_feed (
                feed_event_id INTEGER PRIMARY KEY,
                account_id INTEGER NOT NULL,
                timestamp INTEGER NOT NULL,
                comment_count INTEGER NOT NULL,
                event_type INTEGER NOT NULL,
                event_sub_type INTEGER NOT NULL,
                param_big_int_1 INTEGER NOT NULL,
                param_int_1 INTEGER NOT NULL,
                param_int_2 INTEGER NOT NULL,
                param_int_3 INTEGER NOT NULL,
                param_string TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS social_feed_comments (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                feed_event_id INTEGER NOT NULL,
                commenter_account_id INTEGER NOT NULL,
                timestamp INTEGER NOT NULL,
                comment_text TEXT NOT NULL
            );
            """;
        command.ExecuteNonQuery();
        EnsureColumn(connection, "match_players", "mvp", "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(connection, "reports", "report_reasons_csv", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, "reports", "game_time", "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(connection, "reports", "debug_slot", "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(connection, "reports", "debug_match_id", "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(connection, "periodic_resources", "resource_max", "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(connection, "periodic_resources", "resource_used", "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(connection, "hero_stickers", "quality", "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(connection, "hero_stickers", "source_item_id", "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(connection, "overworld_state", "overworld_version", "INTEGER NOT NULL DEFAULT 1");
    }

    private DotaStatsProfile EnsureProfileLocked(SqliteConnection connection, ulong steamId, uint accountId, string personaName, SqliteTransaction? transaction = null)
    {
        var identity = ResolveIdentity(accountId, steamId, personaName);
        var now = Now();
        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = """
                INSERT INTO profiles (account_id, steam_id, persona_name, updated_at)
                VALUES ($account_id, $steam_id, $persona_name, $updated_at)
                ON CONFLICT(account_id) DO UPDATE SET
                    steam_id = excluded.steam_id,
                    persona_name = excluded.persona_name,
                    updated_at = excluded.updated_at
                """;
            Add(command, "$account_id", identity.AccountId);
            Add(command, "$steam_id", identity.SteamId);
            Add(command, "$persona_name", identity.PersonaName);
            Add(command, "$updated_at", now);
            command.ExecuteNonQuery();
        }

        EnsureTrophyLocked(connection, transaction, identity.AccountId, identity.SteamId, 4, 0);
        EnsureTrophyLocked(connection, transaction, identity.AccountId, identity.SteamId, 28, ReadProfileLocked(connection, identity.AccountId, transaction)?.Level ?? 1);
        EnsureAllHeroProgressLocked(connection, identity.AccountId, transaction);
        EnsureDefaultProfileSlotsLocked(connection, identity.AccountId, transaction);
        return ReadProfileLocked(connection, identity.AccountId, transaction)!;
    }

    private DotaStatsAccountIdentity ResolveIdentity(uint accountId, ulong steamId, string personaName)
    {
        if (accountId == 0 && steamId != 0)
        {
            accountId = SteamIdToAccountId(steamId);
        }

        if (accountId == 0)
        {
            accountId = 100000;
        }

        var resolved = _identityResolver?.Invoke(accountId);
        steamId = resolved is { SteamId: not 0 }
            ? resolved.SteamId
            : steamId != 0
                ? steamId
                : ToSteamId(accountId);
        personaName = !string.IsNullOrWhiteSpace(personaName)
            ? personaName.Trim()
            : !string.IsNullOrWhiteSpace(resolved?.PersonaName)
                ? resolved!.PersonaName
                : $"User{accountId}";

        return new DotaStatsAccountIdentity(accountId, steamId, personaName);
    }

    private static DotaStatsProfile? ReadProfileLocked(SqliteConnection connection, uint accountId, SqliteTransaction? transaction = null)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT * FROM profiles WHERE account_id = $account_id";
        Add(command, "$account_id", accountId);
        using var reader = command.ExecuteReader();
        return reader.Read()
            ? new DotaStatsProfile
            {
                AccountId = U32(reader, "account_id"),
                SteamId = U64(reader, "steam_id"),
                PersonaName = Str(reader, "persona_name"),
                RankTier = U32(reader, "rank_tier"),
                RankTierScore = U32(reader, "rank_tier_score"),
                LeaderboardRank = U32(reader, "leaderboard_rank"),
                RankScore = U32(reader, "rank_score"),
                EventPoints = U32(reader, "event_points"),
                BadgePoints = U32(reader, "badge_points"),
                Level = U32(reader, "level"),
                Xp = U32(reader, "xp"),
                Wins = U32(reader, "wins"),
                Losses = U32(reader, "losses"),
                BackgroundItemId = U64(reader, "background_item_id")
            }
            : null;
    }

    private static DotaStatsGlobalStats? ReadGlobalStats(SqliteConnection connection, uint accountId, SqliteTransaction? transaction = null)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT * FROM global_stats WHERE account_id = $account_id";
        Add(command, "$account_id", accountId);
        using var reader = command.ExecuteReader();
        return reader.Read()
            ? new DotaStatsGlobalStats
            {
                AccountId = U32(reader, "account_id"),
                SteamId = U64(reader, "steam_id"),
                PersonaName = Str(reader, "persona_name"),
                Kills = U32(reader, "kills"),
                Deaths = U32(reader, "deaths"),
                Assists = U32(reader, "assists"),
                MediaKills = Dbl(reader, "media_kills"),
                MediaDeaths = Dbl(reader, "media_deaths"),
                MediaAssists = Dbl(reader, "media_assists"),
                GamesWon = U32(reader, "games_won"),
                GamesLost = U32(reader, "games_lost"),
                BestGpm = U32(reader, "best_gpm"),
                BestXpm = U32(reader, "best_xpm"),
                MediaGpm = U32(reader, "media_gpm"),
                MediaXpm = U32(reader, "media_xpm"),
                LastHits = U32(reader, "last_hits"),
                MediaLastHits = U32(reader, "media_last_hits"),
                Denies = U32(reader, "denies"),
                Rampages = U32(reader, "rampages"),
                TripleKills = U32(reader, "triple_kills"),
                CouriersKilled = U32(reader, "couriers_killed"),
                FirstBloodsGiven = U32(reader, "first_bloods_given"),
                FirstBloodsReceived = U32(reader, "first_bloods_received"),
                CheesesEaten = U32(reader, "cheeses_eaten"),
                AegisesSnatched = U32(reader, "aegises_snatched"),
                CreepsStacked = U32(reader, "creeps_stacked"),
                RapiersPurchased = U32(reader, "rapiers_purchased"),
                MeanNetworth = Dbl(reader, "mean_networth"),
                MeanDamage = Dbl(reader, "mean_damage"),
                MeanHeals = Dbl(reader, "mean_heals"),
                TeamPerformance = Dbl(reader, "team_performance"),
                AvgFightScore = Dbl(reader, "avg_fight_score"),
                AvgFarmScore = Dbl(reader, "avg_farm_score"),
                AvgSupportScore = Dbl(reader, "avg_support_score"),
                AvgPushScore = Dbl(reader, "avg_push_score"),
                PlayedHeroCount = U32(reader, "played_hero_count")
            }
            : null;
    }

    private static DotaStatsHeroStats ReadHeroStats(SqliteDataReader reader) => new()
    {
        AccountId = U32(reader, "account_id"),
        SteamId = U64(reader, "steam_id"),
        HeroId = U32(reader, "hero_id"),
        Wins = U32(reader, "wins"),
        Losses = U32(reader, "losses"),
        WinStreak = U32(reader, "win_streak"),
        BestWinStreak = U32(reader, "best_win_streak"),
        AvgKills = Dbl(reader, "avg_kills"),
        AvgDeaths = Dbl(reader, "avg_deaths"),
        AvgAssists = Dbl(reader, "avg_assists"),
        AvgGpm = Dbl(reader, "avg_gpm"),
        AvgXpm = Dbl(reader, "avg_xpm"),
        BestKills = U32(reader, "best_kills"),
        BestAssists = U32(reader, "best_assists"),
        BestGpm = U32(reader, "best_gpm"),
        BestXpm = U32(reader, "best_xpm"),
        Performance = Dbl(reader, "performance"),
        AvgLastHits = Dbl(reader, "avg_lasthits"),
        AvgDenies = Dbl(reader, "avg_denies"),
        NetworthPeak = U32(reader, "networth_peak"),
        LasthitPeak = U32(reader, "lasthit_peak"),
        DenyPeak = U32(reader, "deny_peak"),
        DamagePeak = U32(reader, "damage_peak"),
        LongestGamePeak = U32(reader, "longest_game_peak"),
        HealingPeak = U32(reader, "healing_peak")
    };

    private static void UpsertMatchLocked(SqliteConnection connection, SqliteTransaction transaction, DotaStatsMatch match)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO matches (match_id, owner_steam_id, server_steam_id, start_time, duration, game_mode, lobby_type,
                                 good_guys_win, match_flags, radiant_score, dire_score, cluster, first_blood_time,
                                 raw_signout_base64, created_at)
            VALUES ($match_id, $owner_steam_id, $server_steam_id, $start_time, $duration, $game_mode, $lobby_type,
                    $good_guys_win, $match_flags, $radiant_score, $dire_score, $cluster, $first_blood_time,
                    $raw_signout_base64, $created_at)
            ON CONFLICT(match_id) DO UPDATE SET
                owner_steam_id = excluded.owner_steam_id,
                server_steam_id = excluded.server_steam_id,
                start_time = excluded.start_time,
                duration = excluded.duration,
                game_mode = excluded.game_mode,
                lobby_type = excluded.lobby_type,
                good_guys_win = excluded.good_guys_win,
                match_flags = excluded.match_flags,
                radiant_score = excluded.radiant_score,
                dire_score = excluded.dire_score,
                cluster = excluded.cluster,
                first_blood_time = excluded.first_blood_time,
                raw_signout_base64 = excluded.raw_signout_base64
            """;
        Add(command, "$match_id", match.MatchId);
        Add(command, "$owner_steam_id", match.OwnerSteamId);
        Add(command, "$server_steam_id", match.ServerSteamId);
        Add(command, "$start_time", match.StartTime == 0 ? Now() : match.StartTime);
        Add(command, "$duration", match.Duration);
        Add(command, "$game_mode", match.GameMode == 0 ? 1 : match.GameMode);
        Add(command, "$lobby_type", match.LobbyType);
        Add(command, "$good_guys_win", match.GoodGuysWin ? 1 : 0);
        Add(command, "$match_flags", match.MatchFlags);
        Add(command, "$radiant_score", match.RadiantScore);
        Add(command, "$dire_score", match.DireScore);
        Add(command, "$cluster", match.Cluster);
        Add(command, "$first_blood_time", match.FirstBloodTime);
        Add(command, "$raw_signout_base64", match.RawSignoutBase64 ?? string.Empty);
        Add(command, "$created_at", Now());
        command.ExecuteNonQuery();
    }

    private static void UpsertMatchPlayerLocked(SqliteConnection connection, SqliteTransaction transaction, DotaStatsMatch match, DotaStatsMatchPlayer player)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO match_players (
                match_id, account_id, steam_id, persona_name, team, player_slot, hero_id, kills, deaths, assists,
                winner, good_guys, gold, gold_spent, gpm, xpm, last_hits, denies, hero_damage, tower_damage,
                hero_healing, level, net_worth, support_gold, claimed_farm_gold, bounty_runes, outposts_captured,
                selected_facet, leaver_status, items_csv, talents_csv, rampages, triple_kills, first_blood_claimed,
                first_blood_given, couriers_killed, aegises_snatched, cheeses_eaten, creeps_stacked, fight_score,
                farm_score, support_score, push_score, damage, heals, rapiers_purchased, mvp
            )
            VALUES (
                $match_id, $account_id, $steam_id, $persona_name, $team, $player_slot, $hero_id, $kills, $deaths, $assists,
                $winner, $good_guys, $gold, $gold_spent, $gpm, $xpm, $last_hits, $denies, $hero_damage, $tower_damage,
                $hero_healing, $level, $net_worth, $support_gold, $claimed_farm_gold, $bounty_runes, $outposts_captured,
                $selected_facet, $leaver_status, $items_csv, $talents_csv, $rampages, $triple_kills, $first_blood_claimed,
                $first_blood_given, $couriers_killed, $aegises_snatched, $cheeses_eaten, $creeps_stacked, $fight_score,
                $farm_score, $support_score, $push_score, $damage, $heals, $rapiers_purchased, $mvp
            )
            """;
        Add(command, "$match_id", match.MatchId);
        AddMatchPlayerParameters(command, player);
        command.ExecuteNonQuery();
    }

    private static void AddMatchPlayerParameters(SqliteCommand command, DotaStatsMatchPlayer player)
    {
        Add(command, "$account_id", player.AccountId);
        Add(command, "$steam_id", player.SteamId);
        Add(command, "$persona_name", player.PersonaName ?? string.Empty);
        Add(command, "$team", player.Team);
        Add(command, "$player_slot", player.PlayerSlot);
        Add(command, "$hero_id", player.HeroId);
        Add(command, "$kills", player.Kills);
        Add(command, "$deaths", player.Deaths);
        Add(command, "$assists", player.Assists);
        Add(command, "$winner", player.Winner ? 1 : 0);
        Add(command, "$good_guys", player.GoodGuys ? 1 : 0);
        Add(command, "$gold", player.Gold);
        Add(command, "$gold_spent", player.GoldSpent);
        Add(command, "$gpm", player.Gpm);
        Add(command, "$xpm", player.Xpm);
        Add(command, "$last_hits", player.LastHits);
        Add(command, "$denies", player.Denies);
        Add(command, "$hero_damage", player.HeroDamage);
        Add(command, "$tower_damage", player.TowerDamage);
        Add(command, "$hero_healing", player.HeroHealing);
        Add(command, "$level", player.Level);
        Add(command, "$net_worth", player.NetWorth);
        Add(command, "$support_gold", player.SupportGold);
        Add(command, "$claimed_farm_gold", player.ClaimedFarmGold);
        Add(command, "$bounty_runes", player.BountyRunes);
        Add(command, "$outposts_captured", player.OutpostsCaptured);
        Add(command, "$selected_facet", player.SelectedFacet);
        Add(command, "$leaver_status", player.LeaverStatus);
        Add(command, "$items_csv", Csv(player.Items));
        Add(command, "$talents_csv", Csv(player.Talents));
        Add(command, "$rampages", player.Rampages);
        Add(command, "$triple_kills", player.TripleKills);
        Add(command, "$first_blood_claimed", player.FirstBloodClaimed);
        Add(command, "$first_blood_given", player.FirstBloodGiven);
        Add(command, "$couriers_killed", player.CouriersKilled);
        Add(command, "$aegises_snatched", player.AegisesSnatched);
        Add(command, "$cheeses_eaten", player.CheesesEaten);
        Add(command, "$creeps_stacked", player.CreepsStacked);
        Add(command, "$fight_score", player.FightScore);
        Add(command, "$farm_score", player.FarmScore);
        Add(command, "$support_score", player.SupportScore);
        Add(command, "$push_score", player.PushScore);
        Add(command, "$damage", player.Damage);
        Add(command, "$heals", player.Heals);
        Add(command, "$rapiers_purchased", player.RapiersPurchased);
        Add(command, "$mvp", player.Mvp ? 1 : 0);
    }

    private static void AddRealtimeStatsParameters(SqliteCommand command, DotaStatsRealtimeStatsSnapshot snapshot)
    {
        Add(command, "$server_steam_id", snapshot.ServerSteamId);
        Add(command, "$match_id", snapshot.MatchId);
        Add(command, "$timestamp", snapshot.Timestamp);
        Add(command, "$game_time", snapshot.GameTime);
        Add(command, "$game_state", snapshot.GameState);
        Add(command, "$game_mode", snapshot.GameMode);
        Add(command, "$lobby_type", snapshot.LobbyType);
        Add(command, "$league_id", snapshot.LeagueId);
        Add(command, "$radiant_score", snapshot.RadiantScore);
        Add(command, "$dire_score", snapshot.DireScore);
        Add(command, "$player_count", snapshot.PlayerCount);
        Add(command, "$building_count", snapshot.BuildingCount);
        Add(command, "$delta_frame", snapshot.DeltaFrame ? 1 : 0);
        Add(command, "$payload_size", snapshot.PayloadSize);
        Add(command, "$created_at", Now());
    }

    private static void UpsertGlobalStatsLocked(SqliteConnection connection, SqliteTransaction transaction, DotaStatsMatch match, DotaStatsMatchPlayer player, bool playedHeroBefore)
    {
        var existing = ReadGlobalStats(connection, player.AccountId, transaction);
        if (existing == null)
        {
            existing = new DotaStatsGlobalStats
            {
                AccountId = player.AccountId,
                SteamId = player.SteamId,
                PersonaName = player.PersonaName,
                Kills = player.Kills,
                Deaths = player.Deaths,
                Assists = player.Assists,
                MediaKills = player.Kills,
                MediaDeaths = player.Deaths,
                MediaAssists = player.Assists,
                GamesWon = player.Winner ? 1u : 0,
                GamesLost = player.Winner ? 0 : 1u,
                BestGpm = player.Gpm,
                BestXpm = player.Xpm,
                MediaGpm = player.Gpm,
                MediaXpm = player.Xpm,
                LastHits = player.LastHits,
                MediaLastHits = player.LastHits,
                Denies = player.Denies,
                PlayedHeroCount = playedHeroBefore ? 0u : 1u,
                TeamPerformance = player.Winner ? 100 : 0
            };
        }
        else
        {
            existing.SteamId = player.SteamId;
            existing.PersonaName = player.PersonaName;
            existing.Kills += player.Kills;
            existing.Deaths += player.Deaths;
            existing.Assists += player.Assists;
            existing.MediaKills = Avg(existing.MediaKills, player.Kills);
            existing.MediaDeaths = Avg(existing.MediaDeaths, player.Deaths);
            existing.MediaAssists = Avg(existing.MediaAssists, player.Assists);
            existing.GamesWon += player.Winner ? 1u : 0;
            existing.GamesLost += player.Winner ? 0 : 1u;
            existing.BestGpm = Math.Max(existing.BestGpm, player.Gpm);
            existing.BestXpm = Math.Max(existing.BestXpm, player.Xpm);
            existing.MediaGpm = existing.MediaGpm != 0 ? (existing.MediaGpm + player.Gpm) / 2 : player.Gpm;
            existing.MediaXpm = existing.MediaXpm != 0 ? (existing.MediaXpm + player.Xpm) / 2 : player.Xpm;
            existing.MediaLastHits = existing.MediaLastHits != 0 ? (existing.MediaLastHits + player.LastHits) / 2 : player.LastHits;
            existing.LastHits += player.LastHits;
            existing.Denies += player.Denies;
            existing.PlayedHeroCount += playedHeroBefore ? 0u : 1u;
            existing.TeamPerformance = existing.GamesWon * 100.0 / Math.Max(1, existing.GamesWon + existing.GamesLost);
        }

        existing.Rampages += player.Rampages;
        existing.TripleKills += player.TripleKills;
        existing.CheesesEaten += player.CheesesEaten;
        existing.AegisesSnatched += player.AegisesSnatched;
        existing.CouriersKilled += player.CouriersKilled;
        existing.CreepsStacked += player.CreepsStacked;
        existing.FirstBloodsGiven += player.FirstBloodGiven > 0 ? 1u : 0;
        existing.FirstBloodsReceived += player.FirstBloodClaimed > 0 ? 1u : 0;
        existing.RapiersPurchased += player.RapiersPurchased;
        existing.AvgFightScore = Round2(Avg(existing.AvgFightScore, player.FightScore));
        existing.AvgFarmScore = Round2(Avg(existing.AvgFarmScore, player.FarmScore));
        existing.AvgSupportScore = Round2(existing.AvgSupportScore == 0 ? player.SupportScore : (existing.AvgSupportScore + player.SupportScore) / 1.1);
        existing.AvgPushScore = Round2(Avg(existing.AvgPushScore, player.PushScore));
        existing.MeanNetworth = Round2(Avg(existing.MeanNetworth, player.NetWorth));
        existing.MeanDamage = Round2(Avg(existing.MeanDamage, player.Damage));
        existing.MeanHeals = Round2(Avg(existing.MeanHeals, player.Heals));

        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO global_stats (
                account_id, steam_id, persona_name, kills, deaths, assists, media_kills, media_deaths, media_assists,
                games_won, games_lost, best_gpm, best_xpm, media_gpm, media_xpm, last_hits, media_last_hits,
                denies, rampages, triple_kills, couriers_killed, first_bloods_given, first_bloods_received,
                cheeses_eaten, aegises_snatched, creeps_stacked, rapiers_purchased, mean_networth, mean_damage,
                mean_heals, team_performance, avg_fight_score, avg_farm_score, avg_support_score, avg_push_score,
                played_hero_count
            ) VALUES (
                $account_id, $steam_id, $persona_name, $kills, $deaths, $assists, $media_kills, $media_deaths, $media_assists,
                $games_won, $games_lost, $best_gpm, $best_xpm, $media_gpm, $media_xpm, $last_hits, $media_last_hits,
                $denies, $rampages, $triple_kills, $couriers_killed, $first_bloods_given, $first_bloods_received,
                $cheeses_eaten, $aegises_snatched, $creeps_stacked, $rapiers_purchased, $mean_networth, $mean_damage,
                $mean_heals, $team_performance, $avg_fight_score, $avg_farm_score, $avg_support_score, $avg_push_score,
                $played_hero_count
            )
            ON CONFLICT(account_id) DO UPDATE SET
                steam_id = excluded.steam_id, persona_name = excluded.persona_name, kills = excluded.kills,
                deaths = excluded.deaths, assists = excluded.assists, media_kills = excluded.media_kills,
                media_deaths = excluded.media_deaths, media_assists = excluded.media_assists,
                games_won = excluded.games_won, games_lost = excluded.games_lost, best_gpm = excluded.best_gpm,
                best_xpm = excluded.best_xpm, media_gpm = excluded.media_gpm, media_xpm = excluded.media_xpm,
                last_hits = excluded.last_hits, media_last_hits = excluded.media_last_hits, denies = excluded.denies,
                rampages = excluded.rampages, triple_kills = excluded.triple_kills, couriers_killed = excluded.couriers_killed,
                first_bloods_given = excluded.first_bloods_given, first_bloods_received = excluded.first_bloods_received,
                cheeses_eaten = excluded.cheeses_eaten, aegises_snatched = excluded.aegises_snatched,
                creeps_stacked = excluded.creeps_stacked, rapiers_purchased = excluded.rapiers_purchased,
                mean_networth = excluded.mean_networth, mean_damage = excluded.mean_damage, mean_heals = excluded.mean_heals,
                team_performance = excluded.team_performance, avg_fight_score = excluded.avg_fight_score,
                avg_farm_score = excluded.avg_farm_score, avg_support_score = excluded.avg_support_score,
                avg_push_score = excluded.avg_push_score, played_hero_count = excluded.played_hero_count
            """;
        AddGlobalStatsParameters(command, existing);
        command.ExecuteNonQuery();
    }

    private static void AddGlobalStatsParameters(SqliteCommand command, DotaStatsGlobalStats stats)
    {
        Add(command, "$account_id", stats.AccountId);
        Add(command, "$steam_id", stats.SteamId);
        Add(command, "$persona_name", stats.PersonaName ?? string.Empty);
        Add(command, "$kills", stats.Kills);
        Add(command, "$deaths", stats.Deaths);
        Add(command, "$assists", stats.Assists);
        Add(command, "$media_kills", stats.MediaKills);
        Add(command, "$media_deaths", stats.MediaDeaths);
        Add(command, "$media_assists", stats.MediaAssists);
        Add(command, "$games_won", stats.GamesWon);
        Add(command, "$games_lost", stats.GamesLost);
        Add(command, "$best_gpm", stats.BestGpm);
        Add(command, "$best_xpm", stats.BestXpm);
        Add(command, "$media_gpm", stats.MediaGpm);
        Add(command, "$media_xpm", stats.MediaXpm);
        Add(command, "$last_hits", stats.LastHits);
        Add(command, "$media_last_hits", stats.MediaLastHits);
        Add(command, "$denies", stats.Denies);
        Add(command, "$rampages", stats.Rampages);
        Add(command, "$triple_kills", stats.TripleKills);
        Add(command, "$couriers_killed", stats.CouriersKilled);
        Add(command, "$first_bloods_given", stats.FirstBloodsGiven);
        Add(command, "$first_bloods_received", stats.FirstBloodsReceived);
        Add(command, "$cheeses_eaten", stats.CheesesEaten);
        Add(command, "$aegises_snatched", stats.AegisesSnatched);
        Add(command, "$creeps_stacked", stats.CreepsStacked);
        Add(command, "$rapiers_purchased", stats.RapiersPurchased);
        Add(command, "$mean_networth", stats.MeanNetworth);
        Add(command, "$mean_damage", stats.MeanDamage);
        Add(command, "$mean_heals", stats.MeanHeals);
        Add(command, "$team_performance", stats.TeamPerformance);
        Add(command, "$avg_fight_score", stats.AvgFightScore);
        Add(command, "$avg_farm_score", stats.AvgFarmScore);
        Add(command, "$avg_support_score", stats.AvgSupportScore);
        Add(command, "$avg_push_score", stats.AvgPushScore);
        Add(command, "$played_hero_count", stats.PlayedHeroCount);
    }

    private static void UpsertHeroStatsLocked(SqliteConnection connection, SqliteTransaction transaction, DotaStatsMatchPlayer player)
    {
        DotaStatsHeroStats? existing = null;
        using (var select = connection.CreateCommand())
        {
            select.Transaction = transaction;
            select.CommandText = "SELECT * FROM hero_stats WHERE account_id = $account_id AND hero_id = $hero_id";
            Add(select, "$account_id", player.AccountId);
            Add(select, "$hero_id", player.HeroId);
            using var reader = select.ExecuteReader();
            if (reader.Read())
            {
                existing = ReadHeroStats(reader);
            }
        }

        var divisor = player.Deaths == 0 ? 1u : player.Deaths;
        if (existing == null)
        {
            existing = new DotaStatsHeroStats
            {
                AccountId = player.AccountId,
                SteamId = player.SteamId,
                HeroId = player.HeroId,
                Wins = player.Winner ? 1u : 0,
                Losses = player.Winner ? 0 : 1u,
                WinStreak = player.Winner ? 1u : 0,
                BestWinStreak = player.Winner ? 1u : 0,
                AvgKills = player.Kills,
                AvgDeaths = player.Deaths,
                AvgAssists = player.Assists,
                AvgGpm = player.Gpm,
                AvgXpm = player.Xpm,
                AvgLastHits = player.LastHits,
                AvgDenies = player.Denies,
                BestKills = player.Kills,
                BestAssists = player.Assists,
                BestGpm = player.Gpm,
                BestXpm = player.Xpm,
                Performance = Math.Min((player.Kills + player.Assists) / (double)divisor * 100.0, 100.0)
            };
        }
        else
        {
            existing.SteamId = player.SteamId;
            existing.Wins += player.Winner ? 1u : 0;
            existing.Losses += player.Winner ? 0 : 1u;
            existing.WinStreak = player.Winner ? existing.WinStreak + 1 : 0;
            existing.BestWinStreak = Math.Max(existing.BestWinStreak, existing.WinStreak);
            existing.AvgKills = Avg(existing.AvgKills, player.Kills);
            existing.AvgDeaths = Avg(existing.AvgDeaths, player.Deaths);
            existing.AvgAssists = Avg(existing.AvgAssists, player.Assists);
            existing.AvgGpm = Avg(existing.AvgGpm, player.Gpm);
            existing.AvgXpm = Avg(existing.AvgXpm, player.Xpm);
            existing.AvgLastHits = Avg(existing.AvgLastHits, player.LastHits);
            existing.AvgDenies = Avg(existing.AvgDenies, player.Denies);
            existing.BestKills = Math.Max(existing.BestKills, player.Kills);
            existing.BestAssists = Math.Max(existing.BestAssists, player.Assists);
            existing.BestGpm = Math.Max(existing.BestGpm, player.Gpm);
            existing.BestXpm = Math.Max(existing.BestXpm, player.Xpm);
            var avgDeaths = existing.AvgDeaths == 0 ? 1.0 : existing.AvgDeaths;
            existing.Performance = Math.Min((existing.AvgKills + existing.AvgAssists) / avgDeaths * 100.0, 100.0);
        }

        existing.NetworthPeak = Math.Max(existing.NetworthPeak, (uint)Math.Round(player.NetWorth));
        existing.LasthitPeak = Math.Max(existing.LasthitPeak, player.LastHits);
        existing.DenyPeak = Math.Max(existing.DenyPeak, player.Denies);
        existing.DamagePeak = Math.Max(existing.DamagePeak, player.HeroDamage);
        existing.HealingPeak = Math.Max(existing.HealingPeak, player.HeroHealing);

        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO hero_stats (
                account_id, steam_id, hero_id, wins, losses, win_streak, best_win_streak, avg_kills, avg_deaths,
                avg_assists, avg_gpm, avg_xpm, best_kills, best_assists, best_gpm, best_xpm, performance,
                avg_lasthits, avg_denies, networth_peak, lasthit_peak, deny_peak, damage_peak, longest_game_peak, healing_peak
            ) VALUES (
                $account_id, $steam_id, $hero_id, $wins, $losses, $win_streak, $best_win_streak, $avg_kills, $avg_deaths,
                $avg_assists, $avg_gpm, $avg_xpm, $best_kills, $best_assists, $best_gpm, $best_xpm, $performance,
                $avg_lasthits, $avg_denies, $networth_peak, $lasthit_peak, $deny_peak, $damage_peak, $longest_game_peak, $healing_peak
            )
            ON CONFLICT(account_id, hero_id) DO UPDATE SET
                steam_id = excluded.steam_id, wins = excluded.wins, losses = excluded.losses, win_streak = excluded.win_streak,
                best_win_streak = excluded.best_win_streak, avg_kills = excluded.avg_kills, avg_deaths = excluded.avg_deaths,
                avg_assists = excluded.avg_assists, avg_gpm = excluded.avg_gpm, avg_xpm = excluded.avg_xpm,
                best_kills = excluded.best_kills, best_assists = excluded.best_assists, best_gpm = excluded.best_gpm,
                best_xpm = excluded.best_xpm, performance = excluded.performance, avg_lasthits = excluded.avg_lasthits,
                avg_denies = excluded.avg_denies, networth_peak = excluded.networth_peak, lasthit_peak = excluded.lasthit_peak,
                deny_peak = excluded.deny_peak, damage_peak = excluded.damage_peak, longest_game_peak = excluded.longest_game_peak,
                healing_peak = excluded.healing_peak
            """;
        Add(command, "$account_id", existing.AccountId);
        Add(command, "$steam_id", existing.SteamId);
        Add(command, "$hero_id", existing.HeroId);
        Add(command, "$wins", existing.Wins);
        Add(command, "$losses", existing.Losses);
        Add(command, "$win_streak", existing.WinStreak);
        Add(command, "$best_win_streak", existing.BestWinStreak);
        Add(command, "$avg_kills", existing.AvgKills);
        Add(command, "$avg_deaths", existing.AvgDeaths);
        Add(command, "$avg_assists", existing.AvgAssists);
        Add(command, "$avg_gpm", existing.AvgGpm);
        Add(command, "$avg_xpm", existing.AvgXpm);
        Add(command, "$best_kills", existing.BestKills);
        Add(command, "$best_assists", existing.BestAssists);
        Add(command, "$best_gpm", existing.BestGpm);
        Add(command, "$best_xpm", existing.BestXpm);
        Add(command, "$performance", existing.Performance);
        Add(command, "$avg_lasthits", existing.AvgLastHits);
        Add(command, "$avg_denies", existing.AvgDenies);
        Add(command, "$networth_peak", existing.NetworthPeak);
        Add(command, "$lasthit_peak", existing.LasthitPeak);
        Add(command, "$deny_peak", existing.DenyPeak);
        Add(command, "$damage_peak", existing.DamagePeak);
        Add(command, "$longest_game_peak", Math.Max(existing.LongestGamePeak, player.Duration));
        Add(command, "$healing_peak", existing.HealingPeak);
        command.ExecuteNonQuery();
    }

    private static void UpdateProfileProgressLocked(SqliteConnection connection, SqliteTransaction transaction, DotaStatsProfile profile, bool winner, uint heroId)
    {
        var xpGain = winner ? 20u : 10u;
        var xp = profile.Xp + xpGain;
        var level = profile.Level == 0 ? 1 : profile.Level;
        if (xp > level * 100)
        {
            xp = 0;
            level++;
        }

        var rankScore = winner
            ? profile.RankScore + 25
            : profile.RankScore > 25 ? profile.RankScore - 25 : 0;
        var rank = CalculateRank(rankScore);

        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            UPDATE profiles
            SET wins = wins + $wins,
                losses = losses + $losses,
                xp = $xp,
                level = $level,
                rank_score = $rank_score,
                rank_tier = $rank_tier,
                rank_tier_score = $rank_tier_score,
                badge_points = $badge_points,
                event_points = event_points + $event_points,
                updated_at = $updated_at
            WHERE account_id = $account_id
            """;
        Add(command, "$wins", winner ? 1 : 0);
        Add(command, "$losses", winner ? 0 : 1);
        Add(command, "$xp", xp);
        Add(command, "$level", level);
        Add(command, "$rank_score", rankScore);
        Add(command, "$rank_tier", rank.Tier);
        Add(command, "$rank_tier_score", rank.Score);
        Add(command, "$badge_points", CalculateBadgePoints(level));
        Add(command, "$event_points", winner ? 100 : 25);
        Add(command, "$updated_at", Now());
        Add(command, "$account_id", profile.AccountId);
        command.ExecuteNonQuery();
        EnsureTrophyLocked(connection, transaction, profile.AccountId, profile.SteamId, 28, level);
    }

    private static void UpdateAllHeroProgressLocked(SqliteConnection connection, SqliteTransaction transaction, uint accountId, bool winner, uint heroId)
    {
        EnsureAllHeroProgressLocked(connection, accountId, transaction);
        var progress = ReadAllHeroProgressLocked(connection, transaction, accountId);
        if (progress.CurrentHeroId != heroId)
        {
            return;
        }

        progress.CurrentHeroGames++;
        progress.CurrentLapGames++;
        if (winner)
        {
            progress.PreviousHeroId = progress.CurrentHeroId;
            progress.PreviousHeroGames = progress.CurrentHeroGames;
            progress.LapHeroesCompleted++;
            progress.LapHeroesRemaining = progress.LapHeroesRemaining > 0 ? progress.LapHeroesRemaining - 1 : 0;
            var currentIndex = Math.Max(0, progress.HeroIds.IndexOf(progress.CurrentHeroId));
            var nextIndex = currentIndex + 1;
            if (nextIndex >= progress.HeroIds.Count)
            {
                progress.LapsCompleted++;
                progress.BestLapGames = progress.BestLapGames == 0 ? progress.CurrentLapGames : Math.Min(progress.BestLapGames, progress.CurrentLapGames);
                progress.BestLapTime = Math.Max(1, Now() - progress.CurrentLapStarted);
                progress.CurrentLapGames = 0;
                progress.LapHeroesCompleted = 0;
                progress.LapHeroesRemaining = (uint)progress.HeroIds.Count;
                progress.CurrentLapStarted = Now();
                nextIndex = 0;
                using var trophy = connection.CreateCommand();
                trophy.Transaction = transaction;
                trophy.CommandText = """
                    UPDATE trophies
                    SET trophy_score = trophy_score + 1, last_updated = $updated_at
                    WHERE account_id = $account_id AND trophy_id = 4
                    """;
                Add(trophy, "$updated_at", Now());
                Add(trophy, "$account_id", accountId);
                trophy.ExecuteNonQuery();
            }

            progress.CurrentHeroId = progress.HeroIds[nextIndex];
            progress.NextHeroId = progress.HeroIds[(nextIndex + 1) % progress.HeroIds.Count];
            progress.CurrentHeroGames = 0;
        }

        SaveAllHeroProgressLocked(connection, transaction, progress);
    }

    private static void EnsureTrophyLocked(SqliteConnection connection, SqliteTransaction? transaction, uint accountId, ulong steamId, uint trophyId, uint trophyScore)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO trophies (account_id, steam_id, trophy_id, trophy_score, last_updated)
            VALUES ($account_id, $steam_id, $trophy_id, $trophy_score, $last_updated)
            ON CONFLICT(account_id, trophy_id) DO UPDATE SET
                steam_id = excluded.steam_id,
                trophy_score = CASE WHEN excluded.trophy_score > trophies.trophy_score THEN excluded.trophy_score ELSE trophies.trophy_score END,
                last_updated = excluded.last_updated
            """;
        Add(command, "$account_id", accountId);
        Add(command, "$steam_id", steamId);
        Add(command, "$trophy_id", trophyId);
        Add(command, "$trophy_score", trophyScore);
        Add(command, "$last_updated", Now());
        command.ExecuteNonQuery();
    }

    private static void EnsureAllHeroProgressLocked(SqliteConnection connection, uint accountId, SqliteTransaction? transaction = null)
    {
        var profileName = ReadProfileLocked(connection, accountId, transaction)?.PersonaName ?? $"User{accountId}";
        var exists = ScalarLong(connection, transaction, "SELECT COUNT(*) FROM all_hero_order WHERE account_id = $account_id", ("$account_id", accountId)) > 0;
        if (exists)
        {
            var progress = ReadAllHeroProgressLocked(connection, transaction, accountId);
            if (progress.HeroIds.Count == AllHeroChallengeHeroIds.Length)
            {
                if (!string.Equals(progress.ProfileName, profileName, StringComparison.Ordinal))
                {
                    progress.ProfileName = profileName;
                    SaveAllHeroProgressLocked(connection, transaction, progress);
                }

                return;
            }
        }

        var now = Now();
        var initial = new DotaStatsAllHeroProgress
        {
            AccountId = accountId,
            HeroIds = AllHeroChallengeHeroIds.ToList(),
            CurrentHeroId = AllHeroChallengeHeroIds[0],
            NextHeroId = AllHeroChallengeHeroIds[1],
            PreviousHeroId = 0,
            StartHeroId = AllHeroChallengeHeroIds[0],
            LapHeroesRemaining = (uint)AllHeroChallengeHeroIds.Length,
            CurrentLapStarted = now,
            ProfileName = profileName
        };
        SaveAllHeroProgressLocked(connection, transaction, initial);
    }

    private static DotaStatsAllHeroProgress ReadAllHeroProgressLocked(SqliteConnection connection, SqliteTransaction? transaction, uint accountId)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT * FROM all_hero_order WHERE account_id = $account_id";
        Add(command, "$account_id", accountId);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return ReadAllHeroProgress(reader);
        }

        throw new InvalidOperationException("All hero order row missing.");
    }

    private static void SaveAllHeroProgressLocked(SqliteConnection connection, SqliteTransaction? transaction, DotaStatsAllHeroProgress progress)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO all_hero_order (
                account_id, hero_ids_csv, current_hero_id, next_hero_id, previous_hero_id, start_hero_id,
                laps_completed, current_hero_games, current_lap_started, current_lap_games, best_lap_games,
                best_lap_time, lap_heroes_completed, lap_heroes_remaining, previous_hero_games, profile_name
            ) VALUES (
                $account_id, $hero_ids_csv, $current_hero_id, $next_hero_id, $previous_hero_id, $start_hero_id,
                $laps_completed, $current_hero_games, $current_lap_started, $current_lap_games, $best_lap_games,
                $best_lap_time, $lap_heroes_completed, $lap_heroes_remaining, $previous_hero_games, $profile_name
            )
            ON CONFLICT(account_id) DO UPDATE SET
                hero_ids_csv = excluded.hero_ids_csv,
                current_hero_id = excluded.current_hero_id,
                next_hero_id = excluded.next_hero_id,
                previous_hero_id = excluded.previous_hero_id,
                start_hero_id = excluded.start_hero_id,
                laps_completed = excluded.laps_completed,
                current_hero_games = excluded.current_hero_games,
                current_lap_started = excluded.current_lap_started,
                current_lap_games = excluded.current_lap_games,
                best_lap_games = excluded.best_lap_games,
                best_lap_time = excluded.best_lap_time,
                lap_heroes_completed = excluded.lap_heroes_completed,
                lap_heroes_remaining = excluded.lap_heroes_remaining,
                previous_hero_games = excluded.previous_hero_games,
                profile_name = excluded.profile_name
            """;
        Add(command, "$account_id", progress.AccountId);
        Add(command, "$hero_ids_csv", Csv(progress.HeroIds));
        Add(command, "$current_hero_id", progress.CurrentHeroId);
        Add(command, "$next_hero_id", progress.NextHeroId);
        Add(command, "$previous_hero_id", progress.PreviousHeroId);
        Add(command, "$start_hero_id", progress.StartHeroId);
        Add(command, "$laps_completed", progress.LapsCompleted);
        Add(command, "$current_hero_games", progress.CurrentHeroGames);
        Add(command, "$current_lap_started", progress.CurrentLapStarted);
        Add(command, "$current_lap_games", progress.CurrentLapGames);
        Add(command, "$best_lap_games", progress.BestLapGames);
        Add(command, "$best_lap_time", progress.BestLapTime);
        Add(command, "$lap_heroes_completed", progress.LapHeroesCompleted);
        Add(command, "$lap_heroes_remaining", progress.LapHeroesRemaining);
        Add(command, "$previous_hero_games", progress.PreviousHeroGames);
        Add(command, "$profile_name", progress.ProfileName ?? string.Empty);
        command.ExecuteNonQuery();
    }

    private static DotaStatsAllHeroProgress ReadAllHeroProgress(SqliteDataReader reader) => new()
    {
        AccountId = U32(reader, "account_id"),
        HeroIds = ParseCsvU32(Str(reader, "hero_ids_csv")),
        CurrentHeroId = U32(reader, "current_hero_id"),
        NextHeroId = U32(reader, "next_hero_id"),
        PreviousHeroId = U32(reader, "previous_hero_id"),
        StartHeroId = U32(reader, "start_hero_id"),
        LapsCompleted = U32(reader, "laps_completed"),
        CurrentHeroGames = U32(reader, "current_hero_games"),
        CurrentLapStarted = U32(reader, "current_lap_started"),
        CurrentLapGames = U32(reader, "current_lap_games"),
        BestLapGames = U32(reader, "best_lap_games"),
        BestLapTime = U32(reader, "best_lap_time"),
        LapHeroesCompleted = U32(reader, "lap_heroes_completed"),
        LapHeroesRemaining = U32(reader, "lap_heroes_remaining"),
        PreviousHeroGames = U32(reader, "previous_hero_games"),
        ProfileName = Str(reader, "profile_name")
    };

    private static void EnsureDefaultProfileSlotsLocked(SqliteConnection connection, uint accountId, SqliteTransaction? transaction = null)
    {
        var count = ScalarLong(connection, transaction, "SELECT COUNT(*) FROM profile_slots WHERE account_id = $account_id", ("$account_id", accountId));
        if (count > 0)
        {
            return;
        }

        var heroes = new List<uint>();
        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = "SELECT hero_id FROM hero_stats WHERE account_id = $account_id ORDER BY (wins + losses) DESC, performance DESC LIMIT 3";
            Add(command, "$account_id", accountId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                heroes.Add(U32(reader, "hero_id"));
            }
        }

        while (heroes.Count < 3)
        {
            heroes.Add(new[] { 1u, 2u, 5u }[heroes.Count]);
        }

        var defaults = new[]
        {
            new DotaStatsProfileSlot { AccountId = accountId, SlotId = 1, SlotType = 4, SlotValue = heroes[0] },
            new DotaStatsProfileSlot { AccountId = accountId, SlotId = 2, SlotType = 4, SlotValue = heroes[1] },
            new DotaStatsProfileSlot { AccountId = accountId, SlotId = 3, SlotType = 4, SlotValue = heroes[2] },
            new DotaStatsProfileSlot { AccountId = accountId, SlotId = 4, SlotType = 1, SlotValue = 3 },
            new DotaStatsProfileSlot { AccountId = accountId, SlotId = 5, SlotType = 1, SlotValue = 5 }
        };

        foreach (var slot in defaults)
        {
            using var insert = connection.CreateCommand();
            insert.Transaction = transaction;
            insert.CommandText = "INSERT OR IGNORE INTO profile_slots (account_id, slot_id, slot_type, slot_value) VALUES ($account_id, $slot_id, $slot_type, $slot_value)";
            Add(insert, "$account_id", slot.AccountId);
            Add(insert, "$slot_id", slot.SlotId);
            Add(insert, "$slot_type", slot.SlotType);
            Add(insert, "$slot_value", slot.SlotValue);
            insert.ExecuteNonQuery();
        }
    }

    private static IReadOnlyList<DotaStatsProfileSlot> ReadProfileSlots(SqliteConnection connection, uint accountId)
    {
        var result = new List<DotaStatsProfileSlot>();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT account_id, slot_id, slot_type, slot_value FROM profile_slots WHERE account_id = $account_id ORDER BY slot_id ASC";
        Add(command, "$account_id", accountId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new DotaStatsProfileSlot
            {
                AccountId = U32(reader, "account_id"),
                SlotId = U32(reader, "slot_id"),
                SlotType = U32(reader, "slot_type"),
                SlotValue = U64(reader, "slot_value")
            });
        }

        return result;
    }

    private static DotaStatsMatch? ReadMatch(SqliteConnection connection, ulong matchId)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM matches WHERE match_id = $match_id";
        Add(command, "$match_id", matchId);
        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        var match = new DotaStatsMatch
        {
            MatchId = U64(reader, "match_id"),
            OwnerSteamId = U64(reader, "owner_steam_id"),
            ServerSteamId = U64(reader, "server_steam_id"),
            StartTime = U32(reader, "start_time"),
            Duration = U32(reader, "duration"),
            GameMode = U32(reader, "game_mode"),
            LobbyType = U32(reader, "lobby_type"),
            GoodGuysWin = U32(reader, "good_guys_win") != 0,
            MatchFlags = U32(reader, "match_flags"),
            RadiantScore = U32(reader, "radiant_score"),
            DireScore = U32(reader, "dire_score"),
            Cluster = U32(reader, "cluster"),
            FirstBloodTime = U32(reader, "first_blood_time"),
            RawSignoutBase64 = Str(reader, "raw_signout_base64")
        };
        match.Players.AddRange(ReadMatchPlayers(connection, matchId));
        return match;
    }

    private static IReadOnlyList<DotaStatsMatchPlayer> ReadMatchPlayers(SqliteConnection connection, ulong matchId)
    {
        var result = new List<DotaStatsMatchPlayer>();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT mp.*, m.start_time AS match_start_time, m.duration AS match_duration, m.game_mode AS match_game_mode,
                   m.lobby_type AS match_lobby_type, m.good_guys_win, m.match_flags, m.radiant_score, m.dire_score,
                   m.cluster, m.first_blood_time
            FROM match_players mp
            JOIN matches m ON m.match_id = mp.match_id
            WHERE mp.match_id = $match_id
            ORDER BY mp.team ASC, mp.player_slot ASC
            """;
        Add(command, "$match_id", matchId);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(ReadMatchPlayer(reader));
        }

        return result;
    }

    private static DotaStatsMatchPlayer ReadMatchPlayer(SqliteDataReader reader) => new()
    {
        MatchId = U64(reader, "match_id"),
        AccountId = U32(reader, "account_id"),
        SteamId = U64(reader, "steam_id"),
        PersonaName = Str(reader, "persona_name"),
        Team = U32(reader, "team"),
        PlayerSlot = U32(reader, "player_slot"),
        HeroId = U32(reader, "hero_id"),
        Kills = U32(reader, "kills"),
        Deaths = U32(reader, "deaths"),
        Assists = U32(reader, "assists"),
        Winner = U32(reader, "winner") != 0,
        GoodGuys = U32(reader, "good_guys") != 0,
        Gold = U32(reader, "gold"),
        GoldSpent = U32(reader, "gold_spent"),
        Gpm = U32(reader, "gpm"),
        Xpm = U32(reader, "xpm"),
        LastHits = U32(reader, "last_hits"),
        Denies = U32(reader, "denies"),
        HeroDamage = U32(reader, "hero_damage"),
        TowerDamage = U32(reader, "tower_damage"),
        HeroHealing = U32(reader, "hero_healing"),
        Level = U32(reader, "level"),
        NetWorth = Dbl(reader, "net_worth"),
        SupportGold = U32(reader, "support_gold"),
        ClaimedFarmGold = U32(reader, "claimed_farm_gold"),
        BountyRunes = U32(reader, "bounty_runes"),
        OutpostsCaptured = U32(reader, "outposts_captured"),
        SelectedFacet = U32(reader, "selected_facet"),
        LeaverStatus = U32(reader, "leaver_status"),
        Items = ParseCsvU32(Str(reader, "items_csv")),
        Talents = ParseCsvU32(Str(reader, "talents_csv")),
        Rampages = U32(reader, "rampages"),
        TripleKills = U32(reader, "triple_kills"),
        FirstBloodClaimed = U32(reader, "first_blood_claimed"),
        FirstBloodGiven = U32(reader, "first_blood_given"),
        CouriersKilled = U32(reader, "couriers_killed"),
        AegisesSnatched = U32(reader, "aegises_snatched"),
        CheesesEaten = U32(reader, "cheeses_eaten"),
        CreepsStacked = U32(reader, "creeps_stacked"),
        FightScore = Dbl(reader, "fight_score"),
        FarmScore = Dbl(reader, "farm_score"),
        SupportScore = Dbl(reader, "support_score"),
        PushScore = Dbl(reader, "push_score"),
        Damage = Dbl(reader, "damage"),
        Heals = Dbl(reader, "heals"),
        RapiersPurchased = U32(reader, "rapiers_purchased"),
        Mvp = HasColumn(reader, "mvp") && U32(reader, "mvp") != 0,
        StartTime = HasColumn(reader, "match_start_time") ? U32(reader, "match_start_time") : 0,
        Duration = HasColumn(reader, "match_duration") ? U32(reader, "match_duration") : 0,
        GameMode = HasColumn(reader, "match_game_mode") ? U32(reader, "match_game_mode") : 0,
        LobbyType = HasColumn(reader, "match_lobby_type") ? U32(reader, "match_lobby_type") : 0,
        GoodGuysWin = HasColumn(reader, "good_guys_win") && U32(reader, "good_guys_win") != 0,
        MatchFlags = HasColumn(reader, "match_flags") ? U32(reader, "match_flags") : 0,
        RadiantScore = HasColumn(reader, "radiant_score") ? U32(reader, "radiant_score") : 0,
        DireScore = HasColumn(reader, "dire_score") ? U32(reader, "dire_score") : 0,
        Cluster = HasColumn(reader, "cluster") ? U32(reader, "cluster") : 0,
        FirstBloodTime = HasColumn(reader, "first_blood_time") ? U32(reader, "first_blood_time") : 0
    };

    private static void CreateSocialFeedForMatchLocked(SqliteConnection connection, SqliteTransaction transaction, DotaStatsMatchPlayer player, DotaStatsMatch match)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT OR IGNORE INTO social_feed (
                feed_event_id, account_id, timestamp, comment_count, event_type, event_sub_type,
                param_big_int_1, param_int_1, param_int_2, param_int_3, param_string
            ) VALUES (
                $feed_event_id, $account_id, $timestamp, 0, $event_type, $event_sub_type,
                $match_id, $hero_id, $kills, $deaths, $persona_name
            )
            """;
        var feedEventId = (match.MatchId << 16) ^ player.AccountId;
        Add(command, "$feed_event_id", feedEventId);
        Add(command, "$account_id", player.AccountId);
        Add(command, "$timestamp", match.StartTime == 0 ? Now() : match.StartTime);
        Add(command, "$event_type", player.Rampages > 0 ? 2 : 1);
        Add(command, "$event_sub_type", player.Winner ? 1 : 0);
        Add(command, "$match_id", match.MatchId);
        Add(command, "$hero_id", player.HeroId);
        Add(command, "$kills", player.Kills);
        Add(command, "$deaths", player.Deaths);
        Add(command, "$persona_name", player.PersonaName ?? string.Empty);
        command.ExecuteNonQuery();
    }

    private static void UpdateSocialFeedCommentCount(SqliteConnection connection, ulong matchId)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE social_feed
            SET comment_count = (SELECT COUNT(*) FROM match_comments WHERE match_id = $match_id)
            WHERE param_big_int_1 = $match_id
            """;
        Add(command, "$match_id", matchId);
        command.ExecuteNonQuery();
    }

    private static void SaveEmoticonAccessLocked(SqliteConnection connection, DotaStatsEmoticonAccess access)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO emoticon_access (account_id, unlocked_mask, updated_at)
            VALUES ($account_id, $unlocked_mask, $updated_at)
            ON CONFLICT(account_id) DO UPDATE SET
                unlocked_mask = excluded.unlocked_mask,
                updated_at = excluded.updated_at
            """;
        Add(command, "$account_id", access.AccountId);
        Add(command, "$unlocked_mask", access.UnlockedMask.ToArray());
        Add(command, "$updated_at", access.UpdatedAt);
        command.ExecuteNonQuery();
    }

    private static byte[] CreateDefaultEmoticonAccessMask()
    {
        var mask = new byte[DefaultEmoticonAccessMaskBytes];
        Array.Fill(mask, (byte)0xFF);
        return mask;
    }

    private static (uint Tier, uint Score) CalculateRank(uint mmr)
    {
        if (mmr == 0)
        {
            return (0, 0);
        }

        var bands = new (uint Min, uint Max, uint Tier)[]
        {
            (1, 153, 11), (154, 307, 12), (308, 461, 13), (462, 615, 14), (616, 769, 15),
            (770, 923, 21), (924, 1077, 22), (1078, 1231, 23), (1232, 1385, 24), (1386, 1539, 25),
            (1540, 1693, 31), (1694, 1847, 32), (1848, 2001, 33), (2002, 2155, 34), (2156, 2309, 35),
            (2310, 2463, 41), (2464, 2617, 42), (2618, 2771, 43), (2772, 2925, 44), (2926, 3079, 45),
            (3080, 3233, 51), (3234, 3387, 52), (3388, 3541, 53), (3542, 3695, 54), (3696, 3849, 55),
            (3850, 4003, 61), (4004, 4157, 62), (4158, 4311, 63), (4312, 4465, 64), (4466, 4619, 65),
            (4620, 4819, 71), (4820, 5019, 72), (5020, 5219, 73), (5220, 5419, 74), (5420, 6420, 75)
        };

        foreach (var band in bands)
        {
            if (mmr >= band.Min && mmr <= band.Max)
            {
                return (band.Tier, MedalPercent(mmr, band.Min, band.Max));
            }
        }

        return (80, 100);
    }

    private static uint MedalPercent(uint value, uint min, uint max)
    {
        if (max <= min)
        {
            return 0;
        }

        return Math.Min(100, (value - min) * 100 / (max - min));
    }

    private static uint CalculateBadgePoints(uint level) => level * 100;

    private static long ScalarLong(SqliteConnection connection, SqliteTransaction? transaction, string sql, params (string Name, object Value)[] parameters)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        foreach (var parameter in parameters)
        {
            Add(command, parameter.Name, parameter.Value);
        }

        var value = command.ExecuteScalar();
        return value == null || value == DBNull.Value ? 0 : Convert.ToInt64(value);
    }

    private static bool TableExists(SqliteConnection connection, string schema, string table)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM {QuoteIdentifier(schema)}.sqlite_master WHERE type = 'table' AND name = $name";
        Add(command, "$name", table);
        var value = command.ExecuteScalar();
        return value != null && value != DBNull.Value && Convert.ToInt64(value) > 0;
    }

    private static IReadOnlyList<string> ReadTableColumns(SqliteConnection connection, string schema, string table)
    {
        var columns = new List<string>();
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA {QuoteIdentifier(schema)}.table_info({QuoteIdentifier(table)})";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var name = Convert.ToString(reader["name"]);
            if (!string.IsNullOrWhiteSpace(name))
            {
                columns.Add(name);
            }
        }

        return columns;
    }

    private static string QuoteIdentifier(string identifier)
    {
        return "\"" + identifier.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";
    }

    private static void EnsureColumn(SqliteConnection connection, string table, string column, string definition)
    {
        using (var check = connection.CreateCommand())
        {
            check.CommandText = $"PRAGMA table_info({table})";
            using var reader = check.ExecuteReader();
            while (reader.Read())
            {
                if (string.Equals(Convert.ToString(reader["name"]), column, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }
        }

        using var alter = connection.CreateCommand();
        alter.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {definition}";
        alter.ExecuteNonQuery();
    }

    private static void Add(SqliteCommand command, string name, object? value)
    {
        command.Parameters.AddWithValue(name, value ?? DBNull.Value);
    }

    private static string Csv(IEnumerable<uint> values) => string.Join(",", values ?? Array.Empty<uint>());

    private static uint FirstCsvUInt32(string csv, uint fallback)
    {
        if (!string.IsNullOrWhiteSpace(csv))
        {
            foreach (var part in csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (uint.TryParse(part, out var value))
                {
                    return value;
                }
            }
        }

        return fallback;
    }

    private static List<uint> ParseCsvU32(string csv)
    {
        var result = new List<uint>();
        if (string.IsNullOrWhiteSpace(csv))
        {
            return result;
        }

        foreach (var part in csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (uint.TryParse(part, out var value))
            {
                result.Add(value);
            }
        }

        return result;
    }

    private static uint U32(SqliteDataReader reader, string name) => Convert.ToUInt32(reader[name]);
    private static ulong U64(SqliteDataReader reader, string name) => Convert.ToUInt64(reader[name]);
    private static double Dbl(SqliteDataReader reader, string name) => Convert.ToDouble(reader[name]);
    private static string Str(SqliteDataReader reader, string name) => Convert.ToString(reader[name]) ?? string.Empty;
    private static byte[] Blob(SqliteDataReader reader, string name) => reader[name] is byte[] bytes ? bytes.ToArray() : Array.Empty<byte>();
    private static uint Now() => (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    private static double Avg(double existing, double value) => existing == 0 ? value : (existing + value) / 2.0;
    private static double Round2(double value) => Math.Round(value, 2);
    private static ulong ToSteamId(uint accountId) => 76561197960265728UL + accountId;
    private static uint SteamIdToAccountId(ulong steamId) => steamId > 76561197960265728UL ? (uint)(steamId - 76561197960265728UL) : 0;

    private static bool HasColumn(SqliteDataReader reader, string column)
    {
        for (var i = 0; i < reader.FieldCount; i++)
        {
            if (string.Equals(reader.GetName(i), column, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}

public sealed record DotaStatsAccountIdentity(uint AccountId, ulong SteamId, string PersonaName);

public sealed class DotaStatsProfile
{
    public uint AccountId { get; init; }
    public ulong SteamId { get; init; }
    public string PersonaName { get; init; } = string.Empty;
    public uint RankTier { get; init; }
    public uint RankTierScore { get; init; }
    public uint LeaderboardRank { get; init; }
    public uint RankScore { get; init; }
    public uint EventPoints { get; init; }
    public uint BadgePoints { get; init; }
    public uint Level { get; init; }
    public uint Xp { get; init; }
    public uint Wins { get; init; }
    public uint Losses { get; init; }
    public ulong BackgroundItemId { get; init; }
    public uint LifetimeGames => Wins + Losses;
}

public sealed class DotaStatsProfileSlot
{
    public uint AccountId { get; init; }
    public uint SlotId { get; init; }
    public uint SlotType { get; init; }
    public ulong SlotValue { get; init; }
}

public sealed class DotaStatsMatchSignOutPermissionAudit
{
    public ulong ServerSteamId { get; init; }
    public uint ServerVersion { get; init; }
    public uint LocalAttempt { get; init; }
    public uint TotalAttempt { get; init; }
    public uint SecondsWaited { get; init; }
    public bool PermissionGranted { get; init; }
    public bool AbandonSignout { get; init; }
    public uint RetryDelaySeconds { get; init; }
}

public sealed class DotaStatsLeaverEvent
{
    public ulong ServerSteamId { get; init; }
    public ulong LeaverSteamId { get; init; }
    public uint LeaverAccountId { get; init; }
    public uint LeaverStatus { get; init; }
    public uint LobbyState { get; init; }
    public uint GameState { get; init; }
    public bool LeaverDetected { get; init; }
    public bool FirstBloodHappened { get; init; }
    public bool DiscardMatchResults { get; init; }
    public bool MassDisconnect { get; init; }
    public uint ServerCluster { get; init; }
    public uint DisconnectReason { get; init; }
}

public sealed class DotaStatsRealtimeStatsSnapshot
{
    public ulong ServerSteamId { get; init; }
    public ulong MatchId { get; init; }
    public uint Timestamp { get; init; }
    public uint GameTime { get; init; }
    public uint GameState { get; init; }
    public uint GameMode { get; init; }
    public uint LobbyType { get; init; }
    public uint LeagueId { get; init; }
    public uint RadiantScore { get; init; }
    public uint DireScore { get; init; }
    public uint PlayerCount { get; init; }
    public uint BuildingCount { get; init; }
    public bool DeltaFrame { get; init; }
    public uint PayloadSize { get; init; }
}

public sealed class DotaStatsMatchStateHistorySnapshot
{
    public ulong MatchId { get; init; }
    public bool RadiantWon { get; init; }
    public uint Mmr { get; init; }
    public uint StateCount { get; init; }
    public uint LastGameTime { get; init; }
    public uint RadiantKills { get; init; }
    public uint DireKills { get; init; }
    public uint PayloadSize { get; init; }
}

public sealed class DotaStatsLiveScoreboardSnapshot
{
    public ulong ServerSteamId { get; init; }
    public ulong MatchId { get; init; }
    public uint TournamentId { get; init; }
    public uint TournamentGameId { get; init; }
    public uint Duration { get; init; }
    public uint HltvDelay { get; init; }
    public uint LeagueId { get; init; }
    public uint RadiantScore { get; init; }
    public uint DireScore { get; init; }
    public uint PlayerCount { get; init; }
    public uint RoshanRespawnTimer { get; init; }
    public uint PayloadSize { get; init; }
}

public sealed class DotaStatsPlayerReport
{
    public ulong ReporterSteamId { get; init; }
    public uint ReporterAccountId { get; init; }
    public uint TargetAccountId { get; init; }
    public ulong LobbyId { get; init; }
    public uint ReportFlags { get; init; }
    public List<uint> ReportReasons { get; init; } = new();
    public string Comment { get; init; } = string.Empty;
    public uint GameTime { get; init; }
    public uint DebugSlot { get; init; }
    public ulong DebugMatchId { get; init; }
}

public sealed class DotaStatsReporterUpdateSummary
{
    public List<DotaStatsReporterUpdate> Updates { get; init; } = new();
    public uint NumReported { get; init; }
    public uint NumNoActionTaken { get; init; }
}

public sealed class DotaStatsReporterUpdate
{
    public ulong MatchId { get; init; }
    public uint HeroId { get; init; }
    public uint ReportReason { get; init; }
    public uint Timestamp { get; init; }
}

public sealed class DotaStatsGlobalStats
{
    public uint AccountId { get; set; }
    public ulong SteamId { get; set; }
    public string PersonaName { get; set; } = string.Empty;
    public uint Kills { get; set; }
    public uint Deaths { get; set; }
    public uint Assists { get; set; }
    public double MediaKills { get; set; }
    public double MediaDeaths { get; set; }
    public double MediaAssists { get; set; }
    public uint GamesWon { get; set; }
    public uint GamesLost { get; set; }
    public uint BestGpm { get; set; }
    public uint BestXpm { get; set; }
    public uint MediaGpm { get; set; }
    public uint MediaXpm { get; set; }
    public uint LastHits { get; set; }
    public uint MediaLastHits { get; set; }
    public uint Denies { get; set; }
    public uint Rampages { get; set; }
    public uint TripleKills { get; set; }
    public uint CouriersKilled { get; set; }
    public uint FirstBloodsGiven { get; set; }
    public uint FirstBloodsReceived { get; set; }
    public uint CheesesEaten { get; set; }
    public uint AegisesSnatched { get; set; }
    public uint CreepsStacked { get; set; }
    public uint RapiersPurchased { get; set; }
    public double MeanNetworth { get; set; }
    public double MeanDamage { get; set; }
    public double MeanHeals { get; set; }
    public double TeamPerformance { get; set; }
    public double AvgFightScore { get; set; }
    public double AvgFarmScore { get; set; }
    public double AvgSupportScore { get; set; }
    public double AvgPushScore { get; set; }
    public uint PlayedHeroCount { get; set; }
    public uint MatchCount => GamesWon + GamesLost;
}

public sealed class DotaStatsHeroStats
{
    public uint AccountId { get; set; }
    public ulong SteamId { get; set; }
    public uint HeroId { get; set; }
    public uint Wins { get; set; }
    public uint Losses { get; set; }
    public uint WinStreak { get; set; }
    public uint BestWinStreak { get; set; }
    public double AvgKills { get; set; }
    public double AvgDeaths { get; set; }
    public double AvgAssists { get; set; }
    public double AvgGpm { get; set; }
    public double AvgXpm { get; set; }
    public uint BestKills { get; set; }
    public uint BestAssists { get; set; }
    public uint BestGpm { get; set; }
    public uint BestXpm { get; set; }
    public double Performance { get; set; }
    public double AvgLastHits { get; set; }
    public double AvgDenies { get; set; }
    public uint NetworthPeak { get; set; }
    public uint LasthitPeak { get; set; }
    public uint DenyPeak { get; set; }
    public uint DamagePeak { get; set; }
    public uint LongestGamePeak { get; set; }
    public uint HealingPeak { get; set; }
}

public sealed class DotaStatsMatch
{
    public ulong MatchId { get; set; }
    public ulong OwnerSteamId { get; set; }
    public ulong ServerSteamId { get; set; }
    public uint StartTime { get; set; }
    public uint Duration { get; set; }
    public uint GameMode { get; set; }
    public uint LobbyType { get; set; }
    public bool GoodGuysWin { get; set; }
    public uint MatchFlags { get; set; }
    public uint RadiantScore { get; set; }
    public uint DireScore { get; set; }
    public uint Cluster { get; set; }
    public uint FirstBloodTime { get; set; }
    public string RawSignoutBase64 { get; set; } = string.Empty;
    public List<DotaStatsMatchPlayer> Players { get; } = new();
}

public sealed class DotaStatsMatchPlayer
{
    public ulong MatchId { get; set; }
    public uint AccountId { get; set; }
    public ulong SteamId { get; set; }
    public string PersonaName { get; set; } = string.Empty;
    public uint Team { get; set; }
    public uint PlayerSlot { get; set; }
    public uint HeroId { get; set; }
    public uint Kills { get; set; }
    public uint Deaths { get; set; }
    public uint Assists { get; set; }
    public bool Winner { get; set; }
    public bool GoodGuys { get; set; }
    public uint Gold { get; set; }
    public uint GoldSpent { get; set; }
    public uint Gpm { get; set; }
    public uint Xpm { get; set; }
    public uint LastHits { get; set; }
    public uint Denies { get; set; }
    public uint HeroDamage { get; set; }
    public uint TowerDamage { get; set; }
    public uint HeroHealing { get; set; }
    public uint Level { get; set; }
    public double NetWorth { get; set; }
    public uint SupportGold { get; set; }
    public uint ClaimedFarmGold { get; set; }
    public uint BountyRunes { get; set; }
    public uint OutpostsCaptured { get; set; }
    public uint SelectedFacet { get; set; }
    public uint LeaverStatus { get; set; }
    public List<uint> Items { get; set; } = new();
    public List<uint> Talents { get; set; } = new();
    public uint Rampages { get; set; }
    public uint TripleKills { get; set; }
    public uint FirstBloodClaimed { get; set; }
    public uint FirstBloodGiven { get; set; }
    public uint CouriersKilled { get; set; }
    public uint AegisesSnatched { get; set; }
    public uint CheesesEaten { get; set; }
    public uint CreepsStacked { get; set; }
    public double FightScore { get; set; }
    public double FarmScore { get; set; }
    public double SupportScore { get; set; }
    public double PushScore { get; set; }
    public double Damage { get; set; }
    public double Heals { get; set; }
    public uint RapiersPurchased { get; set; }
    public bool Mvp { get; set; }
    public uint StartTime { get; set; }
    public uint Duration { get; set; }
    public uint GameMode { get; set; }
    public uint LobbyType { get; set; }
    public bool GoodGuysWin { get; set; }
    public uint MatchFlags { get; set; }
    public uint RadiantScore { get; set; }
    public uint DireScore { get; set; }
    public uint Cluster { get; set; }
    public uint FirstBloodTime { get; set; }
}

public sealed class DotaStatsTrophy
{
    public uint AccountId { get; init; }
    public ulong SteamId { get; init; }
    public uint TrophyId { get; init; }
    public uint TrophyScore { get; init; }
    public uint LastUpdated { get; init; }
}

public sealed class DotaStatsAllHeroProgress
{
    public uint AccountId { get; init; }
    public List<uint> HeroIds { get; set; } = new();
    public uint CurrentHeroId { get; set; }
    public uint NextHeroId { get; set; }
    public uint PreviousHeroId { get; set; }
    public uint StartHeroId { get; set; }
    public uint LapsCompleted { get; set; }
    public uint CurrentHeroGames { get; set; }
    public uint CurrentLapStarted { get; set; }
    public uint CurrentLapGames { get; set; }
    public uint BestLapGames { get; set; }
    public uint BestLapTime { get; set; }
    public uint LapHeroesCompleted { get; set; }
    public uint LapHeroesRemaining { get; set; }
    public uint PreviousHeroGames { get; set; }
    public string ProfileName { get; set; } = string.Empty;
}

public sealed class DotaStatsConduct
{
    public uint AccountId { get; init; }
    public ulong MatchId { get; init; }
    public uint CommendCount { get; init; }
    public uint CommsReports { get; init; }
    public uint MatchesAbandoned { get; init; }
    public uint ReportsCount { get; init; }
    public uint MatchesInReport { get; init; }
    public uint MatchesClean { get; init; }
    public uint MatchesReported { get; init; }
    public uint ReportsParties { get; init; }
    public uint RawBehaviorScore { get; init; }
    public uint OldRawBehaviorScore { get; init; }
    public uint Date { get; init; }
    public uint BehaviorRating { get; init; }
}

public sealed class DotaStatsQuestProgress
{
    public uint QuestId { get; init; }
    public List<DotaStatsQuestChallenge> CompletedChallenges { get; } = new();
}

public sealed class DotaStatsQuestChallenge
{
    public uint ChallengeId { get; init; }
    public uint TimeCompleted { get; init; }
    public uint Attempts { get; init; }
    public uint HeroId { get; init; }
    public uint TemplateId { get; init; }
    public uint QuestRank { get; init; }
}

public sealed class DotaStatsPeriodicResource
{
    public uint AccountId { get; init; }
    public uint ResourceId { get; init; }
    public uint ResourceMax { get; init; }
    public uint ResourceUsed { get; init; }
}

public sealed class DotaStatsHeroSticker
{
    public uint HeroId { get; init; }
    public uint ItemDefId { get; init; }
    public uint Quality { get; init; }
    public ulong SourceItemId { get; init; }
}

public sealed class DotaStatsOverworldState
{
    public uint OverworldId { get; init; }
    public uint CurrentNodeId { get; init; }
    public uint LastRelatedHeroId { get; init; }
    public uint OverworldVersion { get; init; } = 1;
}

public sealed class DotaStatsMonsterHunterState
{
    public uint UnlockedCount { get; init; }
}

public sealed class DotaStatsEmoticonAccess
{
    public uint AccountId { get; init; }
    public byte[] UnlockedMask { get; init; } = Array.Empty<byte>();
    public uint UpdatedAt { get; init; }
}

public sealed class DotaStatsComment
{
    public ulong MatchId { get; init; }
    public uint AccountId { get; init; }
    public string PersonaName { get; init; } = string.Empty;
    public string Comment { get; init; } = string.Empty;
    public uint Timestamp { get; init; }
}

public sealed class DotaStatsSocialFeedEvent
{
    public ulong FeedEventId { get; init; }
    public uint AccountId { get; init; }
    public uint Timestamp { get; init; }
    public uint CommentCount { get; init; }
    public uint EventType { get; init; }
    public uint EventSubType { get; init; }
    public ulong ParamBigInt1 { get; init; }
    public uint ParamInt1 { get; init; }
    public uint ParamInt2 { get; init; }
    public uint ParamInt3 { get; init; }
    public string ParamString { get; init; } = string.Empty;
}

public sealed class DotaStatsVoteSummary
{
    public bool Success { get; set; }
    public uint Vote { get; set; }
    public uint PositiveVotes { get; set; }
    public uint NegativeVotes { get; set; }
}

public sealed class DotaStatsTeammate
{
    public uint AccountId { get; init; }
    public uint Games { get; init; }
    public uint Wins { get; init; }
    public uint MostRecentGameTimestamp { get; init; }
    public ulong MostRecentGameMatchId { get; init; }
}
