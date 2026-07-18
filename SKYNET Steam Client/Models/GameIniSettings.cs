namespace SKYNET.Client.Models;

/// <summary>
/// The subset of steam_api.ini the launcher manages per game. Mirrors the keys the
/// emulator reads (see steam_api/Helpers/Settings.cs). Values here are written into
/// a steam_api.ini in the game's SKYNET folder just before launch.
/// </summary>
public sealed class GameIniSettings
{
    // [Game Settings]
    public string Language { get; set; } = "english";
    public bool UnlockAllDlc { get; set; } = true;

    // [Network Settings]
    public bool UseServerApi { get; set; } = true;
    public bool SecureNetworking { get; set; } = true;
    public string ServerUrl { get; set; } = "http://127.0.0.1:27080/";
    public int BroadCastPort { get; set; } = 28032;
    public int DiscoveryPort { get; set; } = 27081;
    public int PollIntervalMs { get; set; } = 50;
    public int HttpTimeoutMs { get; set; } = 8000;
    public bool UseActiveWebUser { get; set; } = true;

    // [Log Settings]
    public bool LogToFile { get; set; } = true;
    public bool LogToConsole { get; set; } = false;

    // [Audio Settings]
    public bool EnableVoiceCapture { get; set; } = true;

    // [Inventory]
    public bool InventoryEnabled { get; set; } = true;
    public bool AutoGrantPurchases { get; set; } = true;
    public bool AutoGrantPromos { get; set; } = true;

    public GameIniSettings Clone() => (GameIniSettings)MemberwiseClone();
}
