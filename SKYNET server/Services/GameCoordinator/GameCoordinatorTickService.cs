namespace SKYNET_server.Services;

/// <summary>
/// Invokes optional GC plugin timers on a fixed interval, so script backends can
/// implement proactive pushes after an app has been loaded by its first exchange.
/// </summary>
public sealed class GameCoordinatorTickService : BackgroundService
{
    private readonly IReadOnlyList<IGameCoordinatorTicker> _tickers;
    private readonly ILogger<GameCoordinatorTickService> _logger;
    private readonly TimeSpan _interval;

    public GameCoordinatorTickService(
        IEnumerable<IGameCoordinatorPlugin> plugins,
        IConfiguration configuration,
        ILogger<GameCoordinatorTickService> logger)
    {
        _tickers = plugins.OfType<IGameCoordinatorTicker>().ToList();
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
                foreach (var ticker in _tickers)
                {
                    ticker.Tick();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GC tick loop failed");
            }
        }
    }
}
