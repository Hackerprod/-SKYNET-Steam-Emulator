using System.Text.Json;
using System.Text.Json.Serialization;

namespace SKYNET_server.Services;

/// <summary>
/// Per-message engine routing (Fase B of the GC migration). Loaded from
/// GC/gc-routing.json and hot-reloaded by mtime, so flipping a message id
/// between the JS and Lua/C# engines never requires a server restart.
/// Rollback controls: per-id (remove from migratedMessageIds or add to
/// disabledMessageIds), per-file (jintEnabled=false) and environment
/// kill-switch SKYNET_GC_JS_DISABLED=1.
/// </summary>
public sealed class GcRoutingTable
{
    public const string FileName = "gc-routing.json";

    private readonly ILogger<GcRoutingTable> _logger;
    private readonly string _configPath;
    private readonly object _sync = new();
    private GcRoutingConfig _config = new();
    private DateTime _loadedStampUtc = DateTime.MinValue;

    public GcRoutingTable(IHostEnvironment hostEnvironment, ILogger<GcRoutingTable> logger)
    {
        _logger = logger;
        _configPath = Path.Combine(GcPaths.ResolveGcRoot(hostEnvironment.ContentRootPath), FileName);
    }

    public bool IsJintGloballyEnabled
    {
        get
        {
            if (Environment.GetEnvironmentVariable("SKYNET_GC_JS_DISABLED") == "1")
            {
                return false;
            }

            return Current().JintEnabled;
        }
    }

    /// <summary>True when the given exchange message id is routed to the JS engine.</summary>
    public bool IsMigratedToJs(uint appId, uint messageType)
    {
        if (!IsJintGloballyEnabled)
        {
            return false;
        }

        var app = AppConfig(appId);
        return app != null
            && app.MigratedMessageIds.Contains(messageType)
            && !app.DisabledMessageIds.Contains(messageType);
    }

    /// <summary>Engine that serves Poll for the app: "lua" (default) or "js".</summary>
    public string PollEngine(uint appId)
    {
        if (!IsJintGloballyEnabled)
        {
            return "lua";
        }

        return AppConfig(appId)?.PollEngine ?? "lua";
    }

    private GcRoutingAppConfig? AppConfig(uint appId)
    {
        return Current().Apps.TryGetValue(appId.ToString(), out var app) ? app : null;
    }

    private GcRoutingConfig Current()
    {
        lock (_sync)
        {
            var stamp = File.Exists(_configPath) ? File.GetLastWriteTimeUtc(_configPath) : DateTime.MinValue;
            if (stamp == _loadedStampUtc)
            {
                return _config;
            }

            try
            {
                _config = File.Exists(_configPath)
                    ? JsonSerializer.Deserialize<GcRoutingConfig>(File.ReadAllText(_configPath), JsonOptions) ?? new GcRoutingConfig()
                    : new GcRoutingConfig();
                _loadedStampUtc = stamp;
                _logger.LogInformation(
                    "GC routing table loaded from {Path}: jintEnabled={JintEnabled}, apps={Apps}",
                    _configPath, _config.JintEnabled,
                    string.Join(",", _config.Apps.Select(a => $"{a.Key}[{a.Value.MigratedMessageIds.Count} js ids, poll={a.Value.PollEngine}]")));
            }
            catch (Exception ex)
            {
                // A malformed routing file must never take the GC down; keep the
                // previous table and retry on next mtime change.
                _logger.LogError(ex, "Failed to load GC routing table from {Path}; keeping previous table", _configPath);
                _loadedStampUtc = stamp;
            }

            return _config;
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };
}

public sealed class GcRoutingConfig
{
    [JsonPropertyName("jintEnabled")]
    public bool JintEnabled { get; set; } = true;

    [JsonPropertyName("apps")]
    public Dictionary<string, GcRoutingAppConfig> Apps { get; set; } = new();
}

public sealed class GcRoutingAppConfig
{
    [JsonPropertyName("pollEngine")]
    public string PollEngine { get; set; } = "lua";

    [JsonPropertyName("migratedMessageIds")]
    public HashSet<uint> MigratedMessageIds { get; set; } = new();

    [JsonPropertyName("disabledMessageIds")]
    public HashSet<uint> DisabledMessageIds { get; set; } = new();
}
