namespace SKYNET.Client.Models;

/// <summary>
/// Launcher-wide settings + the game library. Persisted to
/// %AppData%\SKYNET\client\config.json.
/// </summary>
public sealed class AppConfig
{
    /// <summary>Base URL of the SKYNET server (matches the emulator's ServerUrl).</summary>
    public string ServerUrl { get; set; } = "http://127.0.0.1:27080/";

    /// <summary>UDP port used to auto-discover the server by broadcast (matches the
    /// emulator's DiscoveryPort).</summary>
    public int DiscoveryPort { get; set; } = 27081;

    /// <summary>When true, try broadcast discovery if the configured URL is unreachable.</summary>
    public bool AutoDiscoverServer { get; set; } = true;

    /// <summary>Stable per-machine client id sent when resolving the web session.</summary>
    public string ClientInstanceId { get; set; } = Guid.NewGuid().ToString("N");

    public List<GameEntry> Games { get; set; } = new();
}
