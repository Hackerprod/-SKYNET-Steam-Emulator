using System.Text.Json;
using SKYNET_server.Models;
using SKYNET_server.Persistence;

namespace SKYNET_server.Services;

/// <summary>
/// Loads app-scoped Steam stat definitions. Values remain per-user records in
/// steam.db; this asset catalog supplies each game's immutable type/default
/// schema and is reloaded after operator edits without restarting the server.
/// </summary>
public sealed class GameStatCatalogService
{
    private readonly object _sync = new();
    private readonly string _dataRoot;
    private readonly string _templateRoot;
    private readonly ILogger<GameStatCatalogService> _logger;
    private readonly Dictionary<uint, CacheEntry> _cache = new();

    public GameStatCatalogService(
        IHostEnvironment environment,
        IConfiguration configuration,
        ILogger<GameStatCatalogService> logger)
    {
        _logger = logger;
        _dataRoot = Path.Combine(
            DatabaseSplitMigrator.ResolveDataRoot(environment.ContentRootPath, configuration),
            "stats");
        _templateRoot = Path.Combine(environment.ContentRootPath, "Assets", "stats");
        Directory.CreateDirectory(_dataRoot);
    }

    public List<ApiStatDefinition> Get(uint appId)
    {
        if (appId == 0)
        {
            return new List<ApiStatDefinition>();
        }

        EnsureDataFile(appId);
        var path = Path.Combine(_dataRoot, $"{appId}.json");
        var writeUtc = File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue;

        lock (_sync)
        {
            if (_cache.TryGetValue(appId, out var cached) && cached.WriteUtc == writeUtc)
            {
                return Clone(cached.Definitions);
            }

            var definitions = Load(path, appId);
            _cache[appId] = new CacheEntry(writeUtc, definitions);
            return Clone(definitions);
        }
    }

    private void EnsureDataFile(uint appId)
    {
        var destination = Path.Combine(_dataRoot, $"{appId}.json");
        if (File.Exists(destination))
        {
            return;
        }

        var template = Path.Combine(_templateRoot, $"{appId}.json");
        if (!File.Exists(template))
        {
            return;
        }

        try
        {
            File.Copy(template, destination);
        }
        catch (IOException) when (File.Exists(destination))
        {
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not initialize stat catalog {Path}", destination);
        }
    }

    private List<ApiStatDefinition> Load(string path, uint appId)
    {
        if (!File.Exists(path))
        {
            return new List<ApiStatDefinition>();
        }

        try
        {
            var definitions = JsonSerializer.Deserialize<List<ApiStatDefinition>>(
                    File.ReadAllText(path),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<ApiStatDefinition>();

            var result = new List<ApiStatDefinition>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var definition in definitions)
            {
                definition.Name = definition.Name?.Trim() ?? string.Empty;
                definition.Type = definition.Type?.Trim().ToLowerInvariant() ?? string.Empty;
                if (definition.Name.Length == 0 ||
                    !seen.Add(definition.Name) ||
                    definition.Type is not ("int" or "float" or "avgrate"))
                {
                    continue;
                }

                if (float.IsNaN(definition.DefaultFloat) || float.IsInfinity(definition.DefaultFloat))
                {
                    definition.DefaultFloat = 0;
                }
                result.Add(definition);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not load stat catalog for AppID {AppId}", appId);
            return new List<ApiStatDefinition>();
        }
    }

    private static List<ApiStatDefinition> Clone(IEnumerable<ApiStatDefinition> definitions)
    {
        return definitions.Select(definition => new ApiStatDefinition
        {
            Name = definition.Name,
            Type = definition.Type,
            DefaultInt = definition.DefaultInt,
            DefaultFloat = definition.DefaultFloat
        }).ToList();
    }

    private sealed record CacheEntry(
        DateTime WriteUtc,
        List<ApiStatDefinition> Definitions);
}
