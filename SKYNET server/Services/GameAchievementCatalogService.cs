using System.Text.Json;
using SKYNET_server.Models;
using SKYNET_server.Persistence;

namespace SKYNET_server.Services;

/// <summary>
/// Loads app-scoped achievement presentation metadata from operator-managed JSON
/// assets. Achievement state remains in steam.db; names, descriptions and images
/// are immutable game metadata and therefore belong in the server asset catalog.
/// Files are reloaded when their timestamp changes, without restarting the server.
/// </summary>
public sealed class GameAchievementCatalogService
{
    private readonly object _sync = new();
    private readonly string _dataRoot;
    private readonly string _templateRoot;
    private readonly ILogger<GameAchievementCatalogService> _logger;
    private readonly Dictionary<uint, CacheEntry> _cache = new();

    public GameAchievementCatalogService(
        IHostEnvironment environment,
        IConfiguration configuration,
        ILogger<GameAchievementCatalogService> logger)
    {
        _logger = logger;
        _dataRoot = Path.Combine(
            DatabaseSplitMigrator.ResolveDataRoot(environment.ContentRootPath, configuration),
            "achievements");
        _templateRoot = Path.Combine(environment.ContentRootPath, "Assets", "achievements");
        Directory.CreateDirectory(_dataRoot);
    }

    public List<ApiAchievementDefinition> Get(uint appId)
    {
        if (appId == 0)
        {
            return new List<ApiAchievementDefinition>();
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
            _logger.LogWarning(ex, "Could not initialize achievement catalog {Path}", destination);
        }
    }

    private List<ApiAchievementDefinition> Load(string path, uint appId)
    {
        if (!File.Exists(path))
        {
            return new List<ApiAchievementDefinition>();
        }

        try
        {
            var definitions = JsonSerializer.Deserialize<List<ApiAchievementDefinition>>(
                    File.ReadAllText(path),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<ApiAchievementDefinition>();

            var result = new List<ApiAchievementDefinition>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var definition in definitions)
            {
                definition.ApiName = definition.ApiName?.Trim() ?? string.Empty;
                if (definition.ApiName.Length == 0 || !seen.Add(definition.ApiName))
                {
                    continue;
                }

                definition.DisplayName = definition.DisplayName?.Trim() ?? string.Empty;
                definition.Description = definition.Description?.Trim() ?? string.Empty;
                definition.IconBase64 = NormalizeBase64(definition.IconBase64);
                definition.LockedIconBase64 = NormalizeBase64(definition.LockedIconBase64);
                result.Add(definition);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not load achievement catalog for AppID {AppId}", appId);
            return new List<ApiAchievementDefinition>();
        }
    }

    private static string NormalizeBase64(string? value)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length == 0)
        {
            return string.Empty;
        }

        try
        {
            _ = Convert.FromBase64String(normalized);
            return normalized;
        }
        catch (FormatException)
        {
            return string.Empty;
        }
    }

    private static List<ApiAchievementDefinition> Clone(IEnumerable<ApiAchievementDefinition> definitions)
    {
        return definitions.Select(definition => new ApiAchievementDefinition
        {
            ApiName = definition.ApiName,
            DisplayName = definition.DisplayName,
            Description = definition.Description,
            Hidden = definition.Hidden,
            IconBase64 = definition.IconBase64,
            LockedIconBase64 = definition.LockedIconBase64
        }).ToList();
    }

    private sealed record CacheEntry(
        DateTime WriteUtc,
        List<ApiAchievementDefinition> Definitions);
}
