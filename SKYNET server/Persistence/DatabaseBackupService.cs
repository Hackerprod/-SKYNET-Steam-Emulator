using Microsoft.EntityFrameworkCore;

namespace SKYNET_server.Persistence;

/// <summary>
/// Periodically snapshots app.db with <c>VACUUM INTO</c> — a consistent,
/// compacted copy safe to take while the database is in use — under
/// Data/backups, keeping the most recent N files and pruning the rest.
/// Replaces the ad-hoc *.bak copies that used to litter the Data folder.
/// Configurable via "Backup:IntervalHours" and "Backup:Retention".
/// </summary>
public sealed class DatabaseBackupService : BackgroundService
{
    private readonly IDbContextFactory<AppDbContext> _factory;
    private readonly ILogger<DatabaseBackupService> _logger;
    private readonly string _backupDir;
    private readonly TimeSpan _interval;
    private readonly int _retention;

    public DatabaseBackupService(
        IDbContextFactory<AppDbContext> factory,
        IHostEnvironment environment,
        IConfiguration configuration,
        ILogger<DatabaseBackupService> logger)
    {
        _factory = factory;
        _logger = logger;
        _backupDir = Path.Combine(environment.ContentRootPath, "Data", "backups");
        _interval = TimeSpan.FromHours(Math.Clamp(configuration.GetValue("Backup:IntervalHours", 6.0), 0.25, 168.0));
        _retention = Math.Clamp(configuration.GetValue("Backup:Retention", 14), 1, 500);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Let startup migration/import settle before the first snapshot.
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        using var timer = new PeriodicTimer(_interval);
        do
        {
            try
            {
                Backup();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Database backup failed.");
            }
        }
        while (await WaitForNextTick(timer, stoppingToken));
    }

    private static async Task<bool> WaitForNextTick(PeriodicTimer timer, CancellationToken token)
    {
        try
        {
            return await timer.WaitForNextTickAsync(token);
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    private void Backup()
    {
        Directory.CreateDirectory(_backupDir);
        var target = Path.Combine(_backupDir, $"app-{DateTime.UtcNow:yyyyMMdd-HHmmss}.db");

        // VACUUM INTO takes a string literal, not a parameter. The path is derived
        // from a timestamp under our own folder, but escape quotes defensively.
        var literal = "'" + target.Replace("'", "''") + "'";

        using var db = _factory.CreateDbContext();
        db.Database.ExecuteSqlRaw($"VACUUM INTO {literal};");
        _logger.LogInformation("Database backup written to {Path}", target);

        Prune();
    }

    private void Prune()
    {
        var stale = new DirectoryInfo(_backupDir)
            .GetFiles("app-*.db")
            .OrderByDescending(f => f.Name)
            .Skip(_retention)
            .ToList();

        foreach (var file in stale)
        {
            try
            {
                file.Delete();
            }
            catch (IOException)
            {
                // Locked/removed already: prune again next cycle.
            }
        }
    }
}
