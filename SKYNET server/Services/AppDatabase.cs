using Microsoft.Data.Sqlite;

namespace SKYNET_server.Services;

/// <summary>
/// Opens connections to the consolidated app.db for the raw-SQL Dota stores.
/// The database is created and set to WAL once at startup (see Program.cs), so
/// connections here only need a busy timeout and foreign-key enforcement — no
/// journal-mode toggling or sidecar juggling. Replaces the old SqliteResilience.
/// </summary>
internal static class AppDatabase
{
    public static string PrepareDatabase(string dbPath, Action<string> initAt)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        initAt(dbPath);
        return dbPath;
    }

    public static SqliteConnection OpenConnection(string dbPath)
    {
        // Default (private) cache: shared-cache is a legacy mode discouraged by
        // SQLite and unnecessary with WAL, which already allows concurrent readers.
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
        };

        var connection = new SqliteConnection(builder.ToString());
        connection.Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "PRAGMA busy_timeout=5000; PRAGMA foreign_keys=ON;";
            command.ExecuteNonQuery();
        }

        return connection;
    }
}
