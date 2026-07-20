using Microsoft.EntityFrameworkCore;

namespace SKYNET_server.Persistence;

/// <summary>
/// Periodically snapshots steam.db and dota.db with VACUUM INTO: compact,
/// consistent copies that are safe to take while SQLite is in WAL mode.
/// </summary>
public sealed class DatabaseBackupService : BackgroundService
{
    private readonly IDbContextFactory<SteamDbContext> _steamFactory;
    private readonly IDbContextFactory<DotaDbContext> _dotaFactory;
    private readonly ILogger<DatabaseBackupService> _logger;
    private readonly string _backupDir;
    private readonly TimeSpan _interval;
    private readonly int _retention;

    public DatabaseBackupService(
        IDbContextFactory<SteamDbContext> steamFactory,
        IDbContextFactory<DotaDbContext> dotaFactory,
        IHostEnvironment environment,
        IConfiguration configuration,
        ILogger<DatabaseBackupService> logger)
    {
        _steamFactory = steamFactory;
        _dotaFactory = dotaFactory;
        _logger = logger;
        _backupDir = Path.Combine(environment.ContentRootPath, "Data", "backups");
        _interval = TimeSpan.FromHours(Math.Clamp(configuration.GetValue("Backup:IntervalHours", 6.0), 0.25, 168.0));
        _retention = Math.Clamp(configuration.GetValue("Backup:Retention", 14), 1, 500);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
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
        var stamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        BackupOne(_steamFactory.CreateDbContext(), Path.Combine(_backupDir, $"steam-{stamp}.db"));
        BackupOne(_dotaFactory.CreateDbContext(), Path.Combine(_backupDir, $"dota-{stamp}.db"));
        Prune("steam-*.db");
        Prune("dota-*.db");
    }

    private void BackupOne(DbContext db, string target)
    {
        using (db)
        {
            var literal = "'" + target.Replace("'", "''") + "'";
#pragma warning disable EF1002
            db.Database.ExecuteSqlRaw($"VACUUM INTO {literal};");
#pragma warning restore EF1002
        }

        _logger.LogInformation("Database backup written to {Path}", target);
    }

    private void Prune(string pattern)
    {
        var stale = new DirectoryInfo(_backupDir)
            .GetFiles(pattern)
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
