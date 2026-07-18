namespace SKYNET.Client.Models;

/// <summary>
/// Launcher-wide settings + the game library. Persisted to
/// %AppData%\SKYNET\client\config.json.
/// </summary>
public sealed class AppConfig
{
    /// <summary>Base URL of the SKYNET server (matches the emulator's ServerUrl).</summary>
    public string ServerUrl { get; set; } = "http://127.0.0.1:27080/";

    /// <summary>Stable per-machine client id sent when resolving the web session.</summary>
    public string ClientInstanceId { get; set; } = Guid.NewGuid().ToString("N");

    public List<GameEntry> Games { get; set; } = new();
}
