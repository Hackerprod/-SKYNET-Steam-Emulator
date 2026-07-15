using Microsoft.EntityFrameworkCore;
using SKYNET_server.Persistence;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    private IDbContextFactory<AppDbContext> _dbFactory = null!;
    private readonly ManualResetEventSlim _flushSignal = new(false);
    private readonly object _flushIoLock = new();
    private Dictionary<string, string> _lastFlushHashes = new(StringComparer.Ordinal);
    private int _flushPending;
    private int _catalogPending;
    private int _flusherStarted;

    // Load the durable state from app.db, importing the legacy stores on first
    // run. Replaces the old JSON load. Presence, tickets and event queues are not
    // persisted, so they start empty and are rebuilt at runtime.
    private void InitializePersistence(string dataRoot)
    {
        using var context = _dbFactory.CreateDbContext();
        context.Database.Migrate();

        if (!context.Users.Any() && File.Exists(_statePath))
        {
            _logger.LogInformation("app.db is empty; importing legacy state from {Path}.", _statePath);
            LegacyStateImporter.Import(context, dataRoot, force: false, m => _logger.LogInformation("{ImportMessage}", m));
            ArchiveLegacyStateFile();
        }

        _state = StatePersistence.Load(context);

        // Derive the in-memory id counter from what is already persisted so a
        // restart never hands out a lobby id that still exists on disk.
        if (_state.Lobbies.Count > 0)
        {
            _nextLobbyId = Math.Max(_nextLobbyId, _state.Lobbies.Keys.Max() + 1);
        }
    }

    // After the one-time import, rename the JSON so it is not re-read and does
    // not confuse app.db as the source of truth. Rename back to re-import.
    private void ArchiveLegacyStateFile()
    {
        try
        {
            if (File.Exists(_statePath))
            {
                var archived = _statePath + ".imported";
                File.Move(_statePath, archived, overwrite: true);
                _logger.LogInformation("Archived legacy state file to {Path}.", archived);
            }
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "Could not archive legacy state file.");
        }
    }

    // Called from the ~54 mutation sites (via SaveState). Non-blocking: marks the
    // durable state dirty and wakes the background flusher, which coalesces bursts
    // and writes to app.db off the request path.
    private void RequestStateFlush()
    {
        Interlocked.Exchange(ref _flushPending, 1);
        _flushSignal.Set();
    }

    // Marks the (large, static) Dota item catalog dirty so the next flush rewrites
    // it. Called only when a cosmetics import changes the catalog.
    private void RequestCatalogFlush()
    {
        Interlocked.Exchange(ref _catalogPending, 1);
        Interlocked.Exchange(ref _flushPending, 1);
        _flushSignal.Set();
    }

    private void StartBackgroundFlusher()
    {
        if (Interlocked.Exchange(ref _flusherStarted, 1) == 1)
        {
            return;
        }

        var thread = new Thread(FlushLoop) { IsBackground = true, Name = "StatePersistenceFlusher" };
        thread.Start();
    }

    private void FlushLoop()
    {
        var consecutiveFailures = 0;
        while (true)
        {
            _flushSignal.Wait();
            _flushSignal.Reset();
            // Coalesce a burst of mutations into a single write.
            Thread.Sleep(500);

            var includeCatalog = Interlocked.Exchange(ref _catalogPending, 0) == 1;
            if (Interlocked.Exchange(ref _flushPending, 0) == 0 && !includeCatalog)
            {
                continue;
            }

            try
            {
                FlushToDatabase(includeCatalog);
                consecutiveFailures = 0;
            }
            catch (Exception ex)
            {
                consecutiveFailures++;
                // Keep the work pending and schedule a retry with capped backoff,
                // instead of waiting (possibly forever) for the next mutation.
                Interlocked.Exchange(ref _flushPending, 1);
                if (includeCatalog)
                {
                    Interlocked.Exchange(ref _catalogPending, 1);
                }

                _logger.LogWarning(ex, "State flush to app.db failed (attempt {Attempt}); retrying.", consecutiveFailures);
                Thread.Sleep(Math.Min(30_000, 500 * (1 << Math.Min(consecutiveFailures, 6))));
                _flushSignal.Set();
            }
        }
    }

    private void FlushToDatabase(bool includeCatalog)
    {
        // Project the state under the lock (fast, in-memory), then do the database
        // I/O outside it. Write rewrites only the tables whose content changed.
        StateSnapshot snapshot;
        lock (_sync)
        {
            snapshot = StatePersistence.BuildSnapshot(_state, includeCatalog);
        }

        lock (_flushIoLock)
        {
            using var context = _dbFactory.CreateDbContext();
            _lastFlushHashes = StatePersistence.Write(context, snapshot, _lastFlushHashes);
        }
    }

    // Synchronous, full flush (including the catalog). Used on graceful shutdown
    // so no in-flight change is lost between the last background write and exit.
    public void FlushStateToDatabase()
    {
        Interlocked.Exchange(ref _flushPending, 0);
        Interlocked.Exchange(ref _catalogPending, 0);
        FlushToDatabase(includeCatalog: true);
    }
}
