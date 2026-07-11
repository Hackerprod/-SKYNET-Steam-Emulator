namespace SKYNET_server.Services;

/// <summary>
/// Invokes the optional Lua `tick()` function of every loaded GC script on a fixed
/// interval, so scripts can implement timers and proactive pushes (via gc.QueueTo).
/// Ticking starts for an app after its first GC exchange loads the script.
/// </summary>
public sealed class GameCoordinatorTickService : BackgroundService
{
    private readonly LuaGameCoordinatorPlugin _plugin;
    private readonly ILogger<GameCoordinatorTickService> _logger;
    private readonly TimeSpan _interval;

    public GameCoordinatorTickService(
        LuaGameCoordinatorPlugin plugin,
        IConfiguration configuration,
        ILogger<GameCoordinatorTickService> logger)
    {
        _plugin = plugin;
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
                _plugin.Tick();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GC tick loop failed");
            }
        }
    }
}
