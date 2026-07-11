using Microsoft.Data.Sqlite;

namespace SKYNET_server.Services;

/// <summary>
/// Helpers to open the local SQLite stores resiliently.
///
/// The server is a single local process, so the stores use rollback-journal
/// (DELETE) mode instead of WAL: WAL leaves persistent <c>-wal</c>/<c>-shm</c>
/// sidecar files that, when the server is force-killed, can get stuck in a
/// Windows "delete-pending" state and make every later open fail with
/// SQLite "disk I/O error". DELETE mode keeps no persistent sidecar, so that
/// class of failure cannot recur.
///
/// <see cref="PrepareDatabase"/> also self-heals one common local failure: if
/// the requested database cannot be initialised because a sidecar is stale, it
/// clears the sidecars and retries. It never switches to a secondary runtime
/// database; storage errors should stay visible now that the repo and game run
/// from a stable local disk.
/// </summary>
internal static class SqliteResilience
{
    /// <summary>
    /// Ensures the database at <paramref name="requestedPath"/> can be opened,
    /// returning the path actually usable (either the requested one or a
    /// fallback next to it). <paramref name="initAt"/> must open a connection
    /// at the given path and create the schema; it may throw
    /// <see cref="SqliteException"/> when the file is unusable.
    /// </summary>
    public static string PrepareDatabase(string requestedPath, Action<string> initAt)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(requestedPath)!);

        try
        {
            initAt(requestedPath);
            return requestedPath;
        }
        catch (SqliteException)
        {
            // A leftover -wal/-shm from a previous hard kill can block the open.
            TryDeleteSidecars(requestedPath);
        }

        try
        {
            initAt(requestedPath);
            return requestedPath;
        }
        catch (SqliteException ex)
        {
            throw new InvalidOperationException(
                $"SQLite database '{requestedPath}' could not be initialized after sidecar cleanup.",
                ex);
        }
    }

    /// <summary>
    /// Opens a connection in DELETE (rollback-journal) mode with a busy timeout.
    /// No persistent WAL sidecar is created.
    /// </summary>
    public static SqliteConnection OpenConnection(string dbPath)
    {
        if (HasBlockedSidecar(dbPath))
        {
            throw new IOException($"SQLite sidecar is locked for '{dbPath}'.");
        }

        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared
        };

        var connection = new SqliteConnection(builder.ToString());
        connection.Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "PRAGMA busy_timeout=5000; PRAGMA journal_mode=DELETE; PRAGMA foreign_keys=ON;";
            command.ExecuteNonQuery();
        }

        return connection;
    }

    private static bool HasBlockedSidecar(string dbPath)
    {
        foreach (var suffix in new[] { "-wal", "-shm", "-journal" })
        {
            var sidecar = dbPath + suffix;
            if (!File.Exists(sidecar))
            {
                continue;
            }

            try
            {
                File.Delete(sidecar);
            }
            catch (IOException)
            {
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return true;
            }
        }

        return false;
    }

    private static void TryDeleteSidecars(string dbPath)
    {
        foreach (var suffix in new[] { "-wal", "-shm", "-journal" })
        {
            try { File.Delete(dbPath + suffix); } catch { /* locked/absent: best effort */ }
        }
    }
}
