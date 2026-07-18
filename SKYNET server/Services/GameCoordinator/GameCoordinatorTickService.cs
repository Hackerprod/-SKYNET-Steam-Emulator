namespace SKYNET_server.Services;

/// <summary>
/// Invokes the optional `tick()` function of every loaded GC engine (Lua and JS)
/// on a fixed interval, so scripts can implement timers and proactive pushes
/// (via gc.QueueTo). Ticking starts for an app after its first GC exchange
/// loads the script. Tick goes through GcEngineRouter, the same router that
/// serves Exchange and Poll (regla 8 of the GC migration).
/// </summary>
public sealed class GameCoordinatorTickService : BackgroundService
{
    private readonly GcEngineRouter _router;
    private readonly ILogger<GameCoordinatorTickService> _logger;
    private readonly TimeSpan _interval;

    public GameCoordinatorTickService(
        GcEngineRouter router,
        IConfiguration configuration,
        ILogger<GameCoordinatorTickService> logger)
    {
        _router = router;
        _logger = logger;
        var intervalMs = configuration.GetValue("GameCoordinator:TickIntervalMs", 1000);
        _interval = TimeSpan.FromMilliseconds(Math.Clamp(intervalMs, 100, 60000));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                _router.Tick();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GC tick loop failed");
            }
        }
    }
}
