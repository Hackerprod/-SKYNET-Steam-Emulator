using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SKYNET_server.Models;

namespace SKYNET_server.Persistence;

/// <summary>
/// One-time, idempotent split from the former mixed app.db into steam.db and
/// dota.db. Runtime code must use only the split databases; app.db and older
/// feature DBs are accepted only as migration inputs and archived on success.
/// </summary>
public static class DatabaseSplitMigrator
{
    private static readonly string[] SteamTables =
    {
        "Users",
        "Friends",
        "FriendRequests",
        "Avatars",
        "Stats",
        "Achievements",
        "WebAccounts",
        "WebSessions",
        "RemoteFiles",
        "RemoteFileShares",
        "AppState",
    };

    private static readonly string[] DotaEfTables =
    {
        "GameServers",
        "Lobbies",
        "DotaItems",
        "DotaEquipment",
        "DotaMatches",
        "DotaMatchPlayers",
        "CosmeticSettings",
        "DotaHeroIds",
        "DotaHeroSlots",
    };

    private static readonly string[] DotaSqlTables =
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
        "reporter_update_acks",
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
        "monster_hunter_state",
        "lobby_invites",
        "parties",
        "party_members",
        "party_invites",
        "dota_guilds",
        "dota_guild_roles",
        "dota_guild_members",
        "dota_guild_invites",
        "DedicatedServers",
        "DedicatedServerEvents",
        "Teams",
        "TeamMembers",
        "KeyValues",
    };

    private static readonly string[] LegacyDotaDbFiles =
    {
        "skynet-dota-stats.db",
        "skynet-dota-party.db",
        "skynet-dota-lobby-invites.db",
        "dedicated-server.db",
    };

    public static SplitMigrationReport Migrate(string dataDir, bool force, Action<string> log)
    {
        Directory.CreateDirectory(dataDir);
        var report = new SplitMigrationReport();
        var steamPath = Path.Combine(dataDir, "steam.db");
        var dotaPath = Path.Combine(dataDir, "dota.db");
        var appPath = Path.Combine(dataDir, "app.db");

        PrepareTargetDatabase(steamPath, IsValidSteamDb, "steam.db", force, log);
        PrepareTargetDatabase(dotaPath, IsValidDotaDb, "dota.db", force, log);

        using var steam = CreateSteamContext(steamPath);
        using var dota = CreateDotaContext(dotaPath);
        steam.Database.EnsureCreated();
        dota.Database.EnsureCreated();
        EnableWal(steam);
        EnableWal(dota);

        if (File.Exists(appPath))
        {
            CopyTables(appPath, steamPath, SteamTables, createMissingFromSource: false, report, "app.db -> steam.db", log);
            CopyTables(appPath, dotaPath, DotaEfTables, createMissingFromSource: false, report, "app.db -> dota.db", log);
            CopyTables(appPath, dotaPath, DotaSqlTables, createMissingFromSource: true, report, "app.db raw Dota -> dota.db", log);
            ArchiveWithSidecars(appPath, dataDir, "legacy");
        }

        foreach (var file in LegacyDotaDbFiles)
        {
            var path = Path.Combine(dataDir, file);
            if (!File.Exists(path))
            {
                continue;
            }

            CopyTables(path, dotaPath, DotaSqlTables, createMissingFromSource: true, report, $"{file} -> dota.db", log);
            ArchiveWithSidecars(path, dataDir, "legacy");
        }

        var jsonPath = Path.Combine(dataDir, "api-state.json");
        if (!steam.Users.Any() && File.Exists(jsonPath))
        {
            ImportJson(steam, dota, jsonPath, includeCatalog: true, m => log(m));
            ArchivePlainFile(jsonPath, dataDir, "legacy");
        }

        report.Log(log);
        return report;
    }

    public static SplitMigrationReport MigrateIfNeeded(
        SteamDbContext steam,
        DotaDbContext dota,
        string dataDir,
        bool force,
        Action<string> log)
    {
        var appPath = Path.Combine(dataDir, "app.db");
        var hasSplitData = steam.Users.Any() || dota.Lobbies.Any() || dota.DotaItems.Any();
        if (!force && hasSplitData && !File.Exists(appPath))
        {
            return new SplitMigrationReport();
        }

        return Migrate(dataDir, force, log);
    }

    public static void ImportJson(SteamDbContext steam, DotaDbContext dota, string jsonPath, bool includeCatalog, Action<string> log)
    {
        ApiState state;
        try
        {
            state = JsonSerializer.Deserialize<ApiState>(File.ReadAllText(jsonPath)) ?? new ApiState();
        }
        catch (JsonException ex)
        {
            log($"api-state.json could not be parsed ({ex.Message}); JSON import aborted.");
            return;
        }

        StatePersistence.Save(steam, dota, state, includeCatalog);
        log($"api-state.json imported into steam.db/dota.db: users={state.Users.Count}, lobbies={state.Lobbies.Count}, dotaItems={state.DotaItems.Count}.");
    }

    public static string ResolveDataRoot(string contentRootPath, IConfiguration configuration)
    {
        var configuredRoot = configuration.GetValue<string>("Data:Root")?.Trim();
        if (string.IsNullOrWhiteSpace(configuredRoot))
        {
            configuredRoot = Environment.GetEnvironmentVariable("SKYNET_DATA_ROOT")?.Trim();
        }

        if (!string.IsNullOrWhiteSpace(configuredRoot))
        {
            return Path.GetFullPath(configuredRoot);
        }

        return Path.Combine(Path.GetFullPath(contentRootPath), "Data");
    }

    private static SteamDbContext CreateSteamContext(string path)
    {
        var options = new DbContextOptionsBuilder<SteamDbContext>()
            .UseSqlite($"Data Source={path}")
            .Options;
        return new SteamDbContext(options);
    }

    private static DotaDbContext CreateDotaContext(string path)
    {
        var options = new DbContextOptionsBuilder<DotaDbContext>()
            .UseSqlite($"Data Source={path}")
            .Options;
        return new DotaDbContext(options);
    }

    private static void EnableWal(DbContext context)
    {
        context.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
        context.Database.ExecuteSqlRaw("PRAGMA busy_timeout=5000;");
    }

    private static void PrepareTargetDatabase(string path, Func<string, bool> isValid, string name, bool force, Action<string> log)
    {
        if (!File.Exists(path))
        {
            return;
        }

        if (!force && isValid(path))
        {
            return;
        }

        ArchiveWithSidecars(path, Path.GetDirectoryName(path)!, "pre-split");
        log($"{name} archived before split because its schema was not the current split schema.");
    }

    private static bool IsValidSteamDb(string path) =>
        HasColumns(path, "Users", "SteamId", "AccountId", "AppId", "PersonaName", "PlayerLevel") &&
        HasColumns(path, "WebAccounts", "Username", "PasswordHash", "SteamId", "IsAdmin");

    private static bool IsValidDotaDb(string path) =>
        HasColumns(path, "DotaItems", "DefIndex", "Name", "HeroNames", "HeroIds") &&
        HasColumns(path, "Lobbies", "SteamId", "LobbyData", "Members");

    private static bool HasColumns(string path, string table, params string[] expected)
    {
        try
        {
            using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
            {
                DataSource = path,
                Mode = SqliteOpenMode.ReadOnly,
                Pooling = false
            }.ToString());
            connection.Open();
            var columns = ReadTableColumns(connection, "main", table).ToHashSet(StringComparer.OrdinalIgnoreCase);
            return expected.All(columns.Contains);
        }
        catch
        {
            return false;
        }
    }

    private static void CopyTables(
        string sourcePath,
        string targetPath,
        IEnumerable<string> tableNames,
        bool createMissingFromSource,
        SplitMigrationReport report,
        string label,
        Action<string> log)
    {
        if (!File.Exists(sourcePath))
        {
            return;
        }

        if (Path.GetFullPath(sourcePath).Equals(Path.GetFullPath(targetPath), StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
        {
            DataSource = targetPath,
            Pooling = false
        }.ToString());
        connection.Open();
        Execute(connection, null, "PRAGMA busy_timeout=5000; PRAGMA foreign_keys=ON;");
        Execute(connection, null, "ATTACH DATABASE $source AS legacy", ("$source", sourcePath));

        try
        {
            using var tx = connection.BeginTransaction();
            foreach (var table in tableNames.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!TableExists(connection, tx, "legacy", table))
                {
                    continue;
                }

                if (!TableExists(connection, tx, "main", table))
                {
                    if (!createMissingFromSource)
                    {
                        continue;
                    }

                    var ddl = ReadCreateSql(connection, tx, "legacy", table);
                    if (string.IsNullOrWhiteSpace(ddl))
                    {
                        continue;
                    }

                    Execute(connection, tx, BuildCreateTableForMain(table, ddl));
                }

                var targetColumns = ReadTableColumns(connection, "main", table);
                var sourceColumns = ReadTableColumns(connection, "legacy", table);
                var sourceSet = sourceColumns.ToHashSet(StringComparer.OrdinalIgnoreCase);
                var columns = targetColumns.Where(sourceSet.Contains).ToArray();
                if (columns.Length == 0)
                {
                    continue;
                }

                var columnList = string.Join(", ", columns.Select(QuoteIdentifier));
                Execute(
                    connection,
                    tx,
                    $"INSERT OR IGNORE INTO main.{QuoteIdentifier(table)} ({columnList}) SELECT {columnList} FROM legacy.{QuoteIdentifier(table)}");

                var source = Count(connection, tx, $"legacy.{QuoteIdentifier(table)}");
                var target = Count(connection, tx, $"main.{QuoteIdentifier(table)}");
                report.Tables[$"{label}:{table}"] = (source, target);
            }

            tx.Commit();
        }
        finally
        {
            Execute(connection, null, "DETACH DATABASE legacy");
        }
    }

    private static string BuildCreateTableForMain(string table, string sourceSql)
    {
        var firstParen = sourceSql.IndexOf('(');
        if (firstParen < 0)
        {
            throw new InvalidOperationException($"Invalid CREATE TABLE statement for {table}.");
        }

        return $"CREATE TABLE IF NOT EXISTS main.{QuoteIdentifier(table)} {sourceSql[firstParen..]}";
    }

    private static string? ReadCreateSql(SqliteConnection connection, SqliteTransaction? tx, string schema, string table)
    {
        using var command = connection.CreateCommand();
        command.Transaction = tx;
        command.CommandText = $"SELECT sql FROM {schema}.sqlite_master WHERE type='table' AND lower(name)=lower($table)";
        command.Parameters.AddWithValue("$table", table);
        return command.ExecuteScalar()?.ToString();
    }

    private static bool TableExists(SqliteConnection connection, SqliteTransaction? tx, string schema, string table)
    {
        using var command = connection.CreateCommand();
        command.Transaction = tx;
        command.CommandText = $"SELECT COUNT(1) FROM {schema}.sqlite_master WHERE type='table' AND lower(name)=lower($table)";
        command.Parameters.AddWithValue("$table", table);
        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    private static IReadOnlyList<string> ReadTableColumns(SqliteConnection connection, string schema, string table)
    {
        var result = new List<string>();
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA {schema}.table_info({QuoteIdentifier(table)})";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(reader.GetString(1));
        }

        return result;
    }

    private static void Execute(SqliteConnection connection, SqliteTransaction? tx, string sql, params (string Name, object Value)[] parameters)
    {
        using var command = connection.CreateCommand();
        command.Transaction = tx;
        command.CommandText = sql;
        foreach (var (name, value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        command.ExecuteNonQuery();
    }

    private static long Count(SqliteConnection connection, SqliteTransaction tx, string table)
    {
        using var command = connection.CreateCommand();
        command.Transaction = tx;
        command.CommandText = $"SELECT COUNT(*) FROM {table}";
        return Convert.ToInt64(command.ExecuteScalar());
    }

    private static string QuoteIdentifier(string identifier) => "\"" + identifier.Replace("\"", "\"\"") + "\"";

    private static void ArchiveWithSidecars(string path, string dataDir, string reason)
    {
        ArchivePlainFile(path, dataDir, reason);
        ArchivePlainFile(path + "-wal", dataDir, reason);
        ArchivePlainFile(path + "-shm", dataDir, reason);
    }

    private static void ArchivePlainFile(string path, string dataDir, string reason)
    {
        if (!File.Exists(path))
        {
            return;
        }

        var backupDir = Path.Combine(dataDir, "backups");
        Directory.CreateDirectory(backupDir);
        var stamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        var fileName = Path.GetFileName(path);
        var target = Path.Combine(backupDir, $"{Path.GetFileNameWithoutExtension(fileName)}-{reason}-{stamp}{Path.GetExtension(fileName)}");
        SqliteConnection.ClearAllPools();
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                File.Move(path, target, overwrite: true);
                return;
            }
            catch (IOException) when (attempt < 5)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Thread.Sleep(100 * attempt);
                SqliteConnection.ClearAllPools();
            }
        }
    }
}

public sealed class SplitMigrationReport
{
    public Dictionary<string, (long Source, long Target)> Tables { get; } = new(StringComparer.OrdinalIgnoreCase);

    public void Log(Action<string> log)
    {
        if (Tables.Count == 0)
        {
            log("Database split migration: no legacy rows needed importing.");
            return;
        }

        log("== database split migration ==");
        foreach (var (name, counts) in Tables.OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase))
        {
            var status = counts.Target >= counts.Source ? "ok" : "CHECK";
            log($"  {name,-55} source={counts.Source,-8} target={counts.Target,-8} {status}");
        }
    }
}
