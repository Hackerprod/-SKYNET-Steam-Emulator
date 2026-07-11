namespace SKYNET_server.Services;

/// <summary>
/// Periodically reconciles friend presence: a user is online only while a game
/// session keeps polling; once it stops (game closed), this flips them to
/// offline and emits persona_state_changed so friends update in real time.
/// Without this the sweep only ran lazily on requests and never marked anyone
/// offline, leaving friends stuck "online".
/// </summary>
public sealed class PresenceSweepService : BackgroundService
{
    private readonly SteamApiStateService _state;
    private readonly ILogger<PresenceSweepService> _logger;
    private readonly TimeSpan _interval;

    public PresenceSweepService(
        SteamApiStateService state,
        IConfiguration configuration,
        ILogger<PresenceSweepService> logger)
    {
        _state = state;
        _logger = logger;
        var intervalMs = configuration.GetValue("Presence:SweepIntervalMs", 20000);
        _interval = TimeSpan.FromMilliseconds(Math.Clamp(intervalMs, 5000, 120000));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                _state.ReconcilePresence();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Presence sweep failed");
            }
        }
    }
}
