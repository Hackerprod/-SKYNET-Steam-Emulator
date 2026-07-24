using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SKYNET_server.Services;

/// <summary>
/// A single snapshot of the network-facing game server settings. Immutable so a
/// live reader always sees a consistent set of values even while an admin edit
/// swaps in a new snapshot.
/// </summary>
public sealed record GameServerSettings
{
    /// <summary>Central IPv4 advertised to game clients. Empty/"auto" = derive per client subnet.</summary>
    public string AdvertisedServerIp { get; init; } = string.Empty;

    public bool DedicatedEnabled { get; init; } = true;

    /// <summary>Interface the dedicated binds. "0.0.0.0" = every interface.</summary>
    public string DedicatedBindIp { get; init; } = "0.0.0.0";

    public int DedicatedPortStart { get; init; } = 27025;
}

/// <summary>
/// Holds the game server settings in memory ("hot") so runtime consumers read the
/// current values without a restart, and mirrors every change back into
/// appsettings.json so edits survive a restart. Startup seeds the hot copy from
/// configuration; <see cref="Apply"/> is the only mutator and both updates the hot
/// copy and rewrites the file under one lock.
/// </summary>
public sealed class GameServerSettingsService
{
    private const string ServerSectionKey = "Server";
    private const string AdvertisedIpKey = "AdvertisedIp";

    private readonly object _sync = new();
    private readonly string _appSettingsPath;
    private readonly ILogger<GameServerSettingsService> _logger;
    private volatile GameServerSettings _current;

    public GameServerSettingsService(
        IHostEnvironment environment,
        IConfiguration configuration,
        ILogger<GameServerSettingsService> logger)
    {
        _logger = logger;
        _appSettingsPath = Path.Combine(environment.ContentRootPath, "appsettings.json");
        _current = Normalize(new GameServerSettings
        {
            AdvertisedServerIp = configuration.GetValue<string>("Server:AdvertisedIp") ?? string.Empty,
            DedicatedEnabled = configuration.GetValue("GameCoordinator:Dota:Dedicated:Enabled", true),
            DedicatedBindIp = configuration.GetValue<string>("GameCoordinator:Dota:Dedicated:BindIp") ?? "0.0.0.0",
            DedicatedPortStart = configuration.GetValue("GameCoordinator:Dota:Dedicated:PortStart", 27025)
        });
    }

    /// <summary>The live settings. Reading is lock-free; the reference is swapped atomically on edit.</summary>
    public GameServerSettings Current => _current;

    /// <summary>
    /// Validates and applies an edit: swaps in a normalized snapshot and persists it
    /// to appsettings.json. Returns the normalized settings on success, or an error
    /// message when a value is invalid (nothing is applied in that case).
    /// </summary>
    public (bool Success, string Message, GameServerSettings Settings) Apply(GameServerSettings incoming)
    {
        var advertised = (incoming.AdvertisedServerIp ?? string.Empty).Trim();
        if (advertised.Length != 0 &&
            !string.Equals(advertised, "auto", StringComparison.OrdinalIgnoreCase) &&
            !IsIPv4(advertised))
        {
            return (false, $"'{advertised}' is not a valid IPv4 address.", _current);
        }

        var bind = (incoming.DedicatedBindIp ?? string.Empty).Trim();
        if (bind.Length == 0)
        {
            bind = "0.0.0.0";
        }
        else if (!IsIPv4(bind))
        {
            return (false, $"Bind IP '{bind}' is not a valid IPv4 address.", _current);
        }

        var normalized = Normalize(incoming with
        {
            AdvertisedServerIp = advertised,
            DedicatedBindIp = bind
        });

        lock (_sync)
        {
            _current = normalized;
            Persist(normalized);
        }

        return (true, "Game server settings saved.", normalized);
    }

    private static GameServerSettings Normalize(GameServerSettings settings) => settings with
    {
        AdvertisedServerIp = (settings.AdvertisedServerIp ?? string.Empty).Trim(),
        DedicatedBindIp = string.IsNullOrWhiteSpace(settings.DedicatedBindIp) ? "0.0.0.0" : settings.DedicatedBindIp.Trim(),
        DedicatedPortStart = Math.Clamp(settings.DedicatedPortStart, 1024, 65534)
    };

    private void Persist(GameServerSettings settings)
    {
        try
        {
            JsonObject root = File.Exists(_appSettingsPath)
                ? JsonNode.Parse(File.ReadAllText(_appSettingsPath)) as JsonObject ?? new JsonObject()
                : new JsonObject();

            var server = EnsureObject(root, ServerSectionKey);
            server[AdvertisedIpKey] = settings.AdvertisedServerIp;

            var dota = EnsureObject(EnsureObject(root, "GameCoordinator"), "Dota");
            var dedicated = EnsureObject(dota, "Dedicated");
            dedicated["Enabled"] = settings.DedicatedEnabled;
            dedicated["BindIp"] = settings.DedicatedBindIp;
            dedicated["PortStart"] = settings.DedicatedPortStart;

            var json = root.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
            var tempPath = _appSettingsPath + ".tmp";
            File.WriteAllText(tempPath, json);
            // Atomic replace so a concurrent config reload never reads a half-written file.
            File.Move(tempPath, _appSettingsPath, overwrite: true);
        }
        catch (Exception ex)
        {
            // The hot copy is already updated, so the running server honors the edit;
            // only persistence across a restart is lost. Surface it, don't crash.
            _logger.LogWarning(ex, "Could not persist game server settings to {Path}", _appSettingsPath);
        }
    }

    private static JsonObject EnsureObject(JsonObject parent, string key)
    {
        if (parent[key] is JsonObject existing)
        {
            return existing;
        }

        var created = new JsonObject();
        parent[key] = created;
        return created;
    }

    private static bool IsIPv4(string value) =>
        IPAddress.TryParse(value, out var parsed) && parsed.AddressFamily == AddressFamily.InterNetwork;

    /// <summary>
    /// Usable IPv4 addresses of the host's up interfaces (skips loopback/APIPA), so
    /// the admin UI can offer the real candidates (WiFi, ZeroTier, etc.) to advertise.
    /// </summary>
    public static IReadOnlyList<string> GetHostIPv4Addresses()
    {
        var result = new List<string>();
        try
        {
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus != OperationalStatus.Up ||
                    nic.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                foreach (var unicast in nic.GetIPProperties().UnicastAddresses)
                {
                    var address = unicast.Address;
                    if (address.AddressFamily != AddressFamily.InterNetwork || IPAddress.IsLoopback(address))
                    {
                        continue;
                    }

                    var text = address.ToString();
                    // Skip APIPA (169.254.x) — never routable to a peer.
                    if (text.StartsWith("169.254.", StringComparison.Ordinal) || result.Contains(text))
                    {
                        continue;
                    }

                    result.Add(text);
                }
            }
        }
        catch
        {
            // Best-effort enumeration; an empty list just means the admin types the IP.
        }

        return result;
    }
}
