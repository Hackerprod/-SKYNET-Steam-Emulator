using SKYNET_server.Models;
using SKYNET_server.Persistence;

namespace SKYNET_server.Services;

/// <summary>
/// Resolves display names for Steam AppIDs from a small operator-managed catalog.
/// The catalog is intentionally external to the databases: names are descriptive
/// server assets, not per-user or game-coordinator state. File timestamp checks
/// make manual edits visible to live API consumers without a server restart.
/// </summary>
public sealed class GameCatalogService
{
    private readonly object _sync = new();
    private readonly string _catalogPath;
    private readonly string _templatePath;
    private readonly ILogger<GameCatalogService> _logger;
    private DateTime _loadedWriteUtc = DateTime.MinValue;
    private Dictionary<uint, string> _names = new();

    public GameCatalogService(IHostEnvironment environment, IConfiguration configuration, ILogger<GameCatalogService> logger)
    {
        _logger = logger;
        var dataRoot = DatabaseSplitMigrator.ResolveDataRoot(environment.ContentRootPath, configuration);
        _catalogPath = Path.Combine(dataRoot, "games.ini");
        _templatePath = Path.Combine(environment.ContentRootPath, "Assets", "games.ini");
        EnsureCatalogFile();
    }

    public ApiGameInfo Get(uint appId) => new()
    {
        AppId = appId,
        Name = GetName(appId)
    };

    public string GetName(uint appId)
    {
        if (appId == 0)
        {
            return "a game";
        }

        EnsureLoaded();
        return _names.TryGetValue(appId, out var name) ? name : "a game";
    }

    private void EnsureCatalogFile()
    {
        try
        {
            if (File.Exists(_catalogPath))
            {
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(_catalogPath)!);
            if (File.Exists(_templatePath))
            {
                File.Copy(_templatePath, _catalogPath);
            }
            else
            {
                File.WriteAllText(_catalogPath,
                    "[Games]" + Environment.NewLine +
                    "480 = Spacewar" + Environment.NewLine +
                    "570 = Dota 2" + Environment.NewLine +
                    "673950 = Farm Together" + Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not initialize game catalog at {Path}", _catalogPath);
        }
    }

    private void EnsureLoaded()
    {
        var writeUtc = File.Exists(_catalogPath) ? File.GetLastWriteTimeUtc(_catalogPath) : DateTime.MinValue;
        if (writeUtc == _loadedWriteUtc)
        {
            return;
        }

        lock (_sync)
        {
            writeUtc = File.Exists(_catalogPath) ? File.GetLastWriteTimeUtc(_catalogPath) : DateTime.MinValue;
            if (writeUtc == _loadedWriteUtc)
            {
                return;
            }

            var names = new Dictionary<uint, string>();
            try
            {
                foreach (var rawLine in File.ReadLines(_catalogPath))
                {
                    var line = rawLine.Trim();
                    if (line.Length == 0 || line.StartsWith(';') || line.StartsWith('#') || line.StartsWith('['))
                    {
                        continue;
                    }

                    var separator = line.IndexOf('=');
                    if (separator <= 0 || !uint.TryParse(line[..separator].Trim(), out var appId))
                    {
                        continue;
                    }

                    var name = line[(separator + 1)..].Trim();
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        names[appId] = name;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not read game catalog at {Path}", _catalogPath);
            }

            _names = names;
            _loadedWriteUtc = writeUtc;
        }
    }
}
