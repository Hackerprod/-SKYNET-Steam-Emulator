using System.Text.Json;
using SKYNET_server.Models;
using SKYNET_server.Persistence;

namespace SKYNET_server.Services;

/// <summary>
/// Loads generic ISteamInventory definitions from app-scoped operator assets.
/// Ownership remains per-user state in steam.db and is never sourced from this catalog.
/// </summary>
public sealed class GameInventoryCatalogService
{
    private readonly object _sync = new();
    private readonly string _dataRoot;
    private readonly string _templateRoot;
    private readonly ILogger<GameInventoryCatalogService> _logger;
    private readonly Dictionary<uint, CacheEntry> _cache = new();

    public GameInventoryCatalogService(IHostEnvironment environment, IConfiguration configuration, ILogger<GameInventoryCatalogService> logger)
    {
        _logger = logger;
        _dataRoot = Path.Combine(DatabaseSplitMigrator.ResolveDataRoot(environment.ContentRootPath, configuration), "inventory");
        _templateRoot = Path.Combine(environment.ContentRootPath, "Assets", "inventory");
        Directory.CreateDirectory(_dataRoot);
    }

    public List<ApiInventoryItemDef> Get(uint appId)
    {
        if (appId == 0) return new List<ApiInventoryItemDef>();
        EnsureDataFile(appId);
        var path = Path.Combine(_dataRoot, $"{appId}.json");
        var writeUtc = File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue;
        lock (_sync)
        {
            if (_cache.TryGetValue(appId, out var cached) && cached.WriteUtc == writeUtc)
                return Clone(cached.Definitions);
            var definitions = Load(path, appId);
            _cache[appId] = new CacheEntry(writeUtc, definitions);
            return Clone(definitions);
        }
    }

    private void EnsureDataFile(uint appId)
    {
        var destination = Path.Combine(_dataRoot, $"{appId}.json");
        if (File.Exists(destination)) return;
        var template = Path.Combine(_templateRoot, $"{appId}.json");
        if (!File.Exists(template)) return;
        try { File.Copy(template, destination); }
        catch (IOException) when (File.Exists(destination)) { }
        catch (Exception ex) { _logger.LogWarning(ex, "Could not initialize inventory catalog {Path}", destination); }
    }

    private List<ApiInventoryItemDef> Load(string path, uint appId)
    {
        if (!File.Exists(path)) return new List<ApiInventoryItemDef>();
        try
        {
            var definitions = JsonSerializer.Deserialize<List<ApiInventoryItemDef>>(
                File.ReadAllText(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            var result = new List<ApiInventoryItemDef>();
            var seen = new HashSet<int>();
            foreach (var definition in definitions)
            {
                if (definition is null || !seen.Add(definition.DefId) || definition.DefId == 0) continue;
                definition.Name = definition.Name?.Trim() ?? string.Empty;
                definition.Type = definition.Type?.Trim().ToLowerInvariant() ?? string.Empty;
                definition.Properties ??= new Dictionary<string, string>();
                definition.Properties = new Dictionary<string, string>(definition.Properties, StringComparer.OrdinalIgnoreCase);
                result.Add(definition);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not load inventory catalog for AppID {AppId}", appId);
            return new List<ApiInventoryItemDef>();
        }
    }

    private static List<ApiInventoryItemDef> Clone(IEnumerable<ApiInventoryItemDef> definitions) => definitions.Select(definition => new ApiInventoryItemDef
    {
        DefId = definition.DefId,
        Name = definition.Name,
        Type = definition.Type,
        Tradable = definition.Tradable,
        Marketable = definition.Marketable,
        Properties = new Dictionary<string, string>(definition.Properties ?? new(), StringComparer.OrdinalIgnoreCase)
    }).ToList();

    private sealed record CacheEntry(DateTime WriteUtc, List<ApiInventoryItemDef> Definitions);
}
