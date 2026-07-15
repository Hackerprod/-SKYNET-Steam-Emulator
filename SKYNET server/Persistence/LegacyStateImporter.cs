using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SKYNET_server.Models;

namespace SKYNET_server.Persistence;

/// <summary>
/// One-shot, idempotent importer that consolidates the pre-refactor stores into
/// the single app.db:
///   * api-state.json (the monolithic JSON) -> the EF-managed tables (via
///     <see cref="StatePersistence.Save"/>);
///   * the three legacy SQLite files (dota stats / party / lobby-invites) ->
///     copied table-by-table into app.db, preserving their schema and keys.
///
/// Idempotent: skips the JSON import when app.db already has users (unless
/// <paramref name="force"/>), and copies legacy rows with INSERT OR IGNORE so a
/// second run is a no-op. Always reports source vs. target counts.
/// </summary>
public static class LegacyStateImporter
{
    private static readonly string[] LegacyDbFiles =
    {
        "skynet-dota-stats.db",
        "skynet-dota-party.db",
        "skynet-dota-lobby-invites.db",
    };

    public static ImportReport Import(AppDbContext context, string dataDir, bool force, Action<string> log)
    {
        var report = new ImportReport();
        context.Database.Migrate();

        var appDbPath = context.Database.GetDbConnection().DataSource;

        if (context.Users.Any() && !force)
        {
            log("app.db already populated; skipping JSON import (pass force to re-import).");
        }
        else
        {
            var jsonPath = Path.Combine(dataDir, "api-state.json");
            if (File.Exists(jsonPath))
            {
                ImportJson(context, jsonPath, report, log);
            }
            else
            {
                log($"api-state.json not found in {dataDir}; nothing to import from JSON.");
            }
        }

        foreach (var file in LegacyDbFiles)
        {
            CopyLegacyDatabase(appDbPath, Path.Combine(dataDir, file), report, log);
        }

        report.Log(log);
        return report;
    }

    private static void ImportJson(AppDbContext context, string jsonPath, ImportReport report, Action<string> log)
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

        StatePersistence.Save(context, state, includeCatalog: true);

        report.Json["Users"] = (state.Users.Count, context.Users.Count());
        report.Json["Friends"] = (state.FriendLinks.Sum(p => p.Value.Count), context.Friends.Count());
        report.Json["FriendRequests"] = (state.FriendRequests.Count, context.FriendRequests.Count());
        report.Json["Avatars"] = (state.Avatars.Count, context.Avatars.Count());
        report.Json["Stats"] = (state.Stats.Sum(p => p.Value.Stats.Count), context.Stats.Count());
        report.Json["Achievements"] = (state.Stats.Sum(p => p.Value.Achievements.Count), context.Achievements.Count());
        report.Json["WebAccounts"] = (state.WebAccounts.Count, context.WebAccounts.Count());
        report.Json["WebSessions"] = (state.WebSessions.Count, context.WebSessions.Count());
        report.Json["GameServers"] = (state.GameServers.Count, context.GameServers.Count());
        report.Json["Lobbies"] = (state.Lobbies.Count, context.Lobbies.Count());
        report.Json["RemoteFiles"] = (state.Files.Count, context.RemoteFiles.Count());
        report.Json["DotaItems"] = (state.DotaItems.Count, context.DotaItems.Count());
        report.Json["DotaEquipment"] = (state.DotaEquipment.Sum(p => p.Value.Count), context.DotaEquipment.Count());
        report.Json["DotaMatches"] = (state.DotaMatches.Count, context.DotaMatches.Count());
        report.Json["DotaHeroIds"] = (state.DotaHeroIds.Count, context.DotaHeroIds.Count());
    }

    private static void CopyLegacyDatabase(string appDbPath, string legacyPath, ImportReport report, Action<string> log)
    {
        var fileName = Path.GetFileName(legacyPath);
        if (!File.Exists(legacyPath))
        {
            log($"legacy db not found: {fileName} (skipped).");
            return;
        }

        using var connection = new SqliteConnection($"Data Source={appDbPath}");
        connection.Open();

        using (var attach = connection.CreateCommand())
        {
            attach.CommandText = "ATTACH DATABASE $path AS legacy";
            attach.Parameters.AddWithValue("$path", legacyPath);
            attach.ExecuteNonQuery();
        }

        try
        {
            var tables = new List<(string Name, string Sql)>();
            using (var query = connection.CreateCommand())
            {
                query.CommandText = "SELECT name, sql FROM legacy.sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'";
                using var reader = query.ExecuteReader();
                while (reader.Read())
                {
                    tables.Add((reader.GetString(0), reader.GetString(1)));
                }
            }

            using var tx = connection.BeginTransaction();
            foreach (var (name, sql) in tables)
            {
                // Reuse the legacy DDL verbatim (keeps PKs/constraints), just made
                // conditional and targeted at the main database.
                var ddl = sql.Replace("CREATE TABLE ", "CREATE TABLE IF NOT EXISTS main.\"", StringComparison.Ordinal);
                var firstParen = ddl.IndexOf('(');
                ddl = ddl[..firstParen].TrimEnd() + "\" " + ddl[firstParen..];

                Execute(connection, tx, ddl);
                Execute(connection, tx, $"INSERT OR IGNORE INTO main.\"{name}\" SELECT * FROM legacy.\"{name}\"");

                var source = Count(connection, tx, $"legacy.\"{name}\"");
                var target = Count(connection, tx, $"main.\"{name}\"");
                report.Legacy[$"{fileName}:{name}"] = (source, target);
            }

            tx.Commit();
        }
        finally
        {
            using var detach = connection.CreateCommand();
            detach.CommandText = "DETACH DATABASE legacy";
            detach.ExecuteNonQuery();
        }
    }

    private static void Execute(SqliteConnection connection, SqliteTransaction tx, string sql)
    {
        using var command = connection.CreateCommand();
        command.Transaction = tx;
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static long Count(SqliteConnection connection, SqliteTransaction tx, string table)
    {
        using var command = connection.CreateCommand();
        command.Transaction = tx;
        command.CommandText = $"SELECT COUNT(*) FROM {table}";
        return Convert.ToInt64(command.ExecuteScalar());
    }
}

public sealed class ImportReport
{
    public Dictionary<string, (long Source, long Target)> Json { get; } = new();
    public Dictionary<string, (long Source, long Target)> Legacy { get; } = new();

    public bool AllMatched =>
        Json.Values.All(v => v.Source == v.Target) &&
        Legacy.Values.All(v => v.Source == v.Target);

    public void Log(Action<string> log)
    {
        log("== JSON -> EF tables ==");
        foreach (var (name, v) in Json.OrderBy(p => p.Key))
        {
            log($"  {name,-16} source={v.Source,-8} target={v.Target,-8} {(v.Source == v.Target ? "ok" : "MISMATCH")}");
        }

        log("== legacy SQLite -> app.db ==");
        foreach (var (name, v) in Legacy.OrderBy(p => p.Key))
        {
            log($"  {name,-40} source={v.Source,-6} target={v.Target,-6} {(v.Source == v.Target ? "ok" : "MISMATCH")}");
        }

        log(AllMatched ? "Import verification: ALL COUNTS MATCH." : "Import verification: MISMATCH DETECTED.");
    }
}
