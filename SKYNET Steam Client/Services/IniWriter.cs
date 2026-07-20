using System.Globalization;
using System.Text;
using SKYNET.Client.Models;

namespace SKYNET.Client.Services;

/// <summary>
/// Writes a steam_api.ini into the game's SKYNET folder from a <see cref="GameEntry"/>.
/// Keys/sections match what the emulator parses (steam_api/Helpers/Settings.cs);
/// omitted keys are auto-filled by the emulator on load.
/// </summary>
public static class IniWriter
{
    private static string B(bool v) => v ? "true" : "false";
    private static string N(int v) => v.ToString(CultureInfo.InvariantCulture);

    /// <summary>The SKYNET config folder next to the game executable.</summary>
    public static string SkynetDir(GameEntry game) => Path.Combine(game.ExeFolder, "SKYNET");

    public static string IniPath(GameEntry game) => Path.Combine(SkynetDir(game), "steam_api.ini");

    public static void Write(GameEntry game, AppConfig app, WebUser? user)
    {
        var s = game.Ini;
        var serverUrl = ResolveServerUrl(s, app);
        s.ServerUrl = serverUrl;
        var sb = new StringBuilder();

        sb.AppendLine("[User Settings]");
        sb.AppendLine($"ClientInstanceId = {app.ClientInstanceId}");
        if (user != null)
        {
            sb.AppendLine($"FallbackPersonaName = {user.DisplayName}");
            sb.AppendLine($"FallbackAccountId = {user.AccountId}");
        }
        sb.AppendLine();

        sb.AppendLine("[Game Settings]");
        sb.AppendLine($"Languaje = {s.Language}");
        sb.AppendLine($"AppId = {game.AppId}");
        sb.AppendLine($"UnlockAllDLC = {B(s.UnlockAllDlc)}");
        sb.AppendLine();

        sb.AppendLine("[Network Settings]");
        sb.AppendLine($"UseServerApi = {B(s.UseServerApi)}");
        sb.AppendLine($"BroadCastPort = {N(s.BroadCastPort)}");
        sb.AppendLine($"SecureNetworking = {B(s.SecureNetworking)}");
        sb.AppendLine($"ServerUrl = {serverUrl}");
        sb.AppendLine($"PollIntervalMs = {N(s.PollIntervalMs)}");
        sb.AppendLine($"HttpTimeoutMs = {N(s.HttpTimeoutMs)}");
        sb.AppendLine($"DiscoveryPort = {N(s.DiscoveryPort)}");
        sb.AppendLine($"UseActiveWebUser = {B(s.UseActiveWebUser)}");
        sb.AppendLine();

        sb.AppendLine("[Log Settings]");
        sb.AppendLine($"File = {B(s.LogToFile)}");
        sb.AppendLine($"Console = {B(s.LogToConsole)}");
        sb.AppendLine();

        sb.AppendLine("[Audio Settings]");
        sb.AppendLine($"EnableVoiceCapture = {B(s.EnableVoiceCapture)}");
        sb.AppendLine();

        sb.AppendLine("[Inventory]");
        sb.AppendLine($"Enabled = {B(s.InventoryEnabled)}");
        sb.AppendLine($"AutoGrantPurchases = {B(s.AutoGrantPurchases)}");
        sb.AppendLine($"AutoGrantPromos = {B(s.AutoGrantPromos)}");
        sb.AppendLine();

        Directory.CreateDirectory(SkynetDir(game));
        File.WriteAllText(IniPath(game), sb.ToString(), new UTF8Encoding(false));
    }

    private static string ResolveServerUrl(GameIniSettings settings, AppConfig app)
    {
        var appUrl = NormalizeUrl(app.ServerUrl);
        var gameUrl = NormalizeUrl(settings.ServerUrl);
        if (string.IsNullOrWhiteSpace(gameUrl))
            return appUrl;

        // The launcher resolves/discovers the real backend before launch. The
        // injected emulator only reads steam_api.ini, so a copied launcher must not
        // keep a per-game localhost default when the app-level server already points
        // to the host machine.
        if (IsLoopbackUrl(gameUrl) && !IsLoopbackUrl(appUrl))
            return appUrl;

        return gameUrl;
    }

    private static string NormalizeUrl(string? url)
    {
        var trimmed = (url ?? string.Empty).Trim();
        if (trimmed.Length == 0)
            return "http://127.0.0.1:27080/";

        return trimmed.EndsWith("/", StringComparison.Ordinal) ? trimmed : trimmed + "/";
    }

    private static bool IsLoopbackUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return false;

        return uri.IsLoopback;
    }
}
