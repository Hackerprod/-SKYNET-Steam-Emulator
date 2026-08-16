using System.Globalization;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text.Json;

namespace SKYNET_server.Services;

public sealed class GameCoordinatorAppCatalog
{
    public const string ManifestFileName = "gc.json";
    private const string DefaultEntryPoint = "main.ts";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public GameCoordinatorAppCatalog(string rootPath)
    {
        RootPath = Path.GetFullPath(rootPath);
    }

    public string RootPath { get; }

    public static string ResolveRoot(string contentRootPath)
    {
        var configuredRoot = Environment.GetEnvironmentVariable("SKYNET_GC_ROOT");
        if (IsValidRoot(configuredRoot))
        {
            return Path.GetFullPath(configuredRoot!);
        }

        var current = new DirectoryInfo(contentRootPath);
        while (current != null)
        {
            var candidate = Path.Combine(current.FullName, "GC");
            if (IsValidRoot(candidate))
            {
                return candidate;
            }

            current = current.Parent;
        }

        return Path.Combine(contentRootPath, "GC");
    }

    public bool TryGetApp(uint appId, out GameCoordinatorAppDefinition app)
    {
        return TryGetApp(appId, out app, out _);
    }

    public bool TryGetApp(uint appId, out GameCoordinatorAppDefinition app, out string error)
    {
        var appRoot = Path.Combine(RootPath, appId.ToString(CultureInfo.InvariantCulture));
        if (!Directory.Exists(appRoot))
        {
            app = default!;
            error = string.Empty;
            return false;
        }

        try
        {
            app = LoadApp(appId, appRoot);
            if (File.Exists(app.MainScriptPath))
            {
                error = string.Empty;
                return true;
            }

            app = default!;
            error = string.Empty;
            return false;
        }
        catch (Exception ex) when (IsCatalogLoadException(ex))
        {
            app = default!;
            error = ex.Message;
            return false;
        }
    }

    public IReadOnlyList<GameCoordinatorAppDefinition> ListApps()
    {
        if (!Directory.Exists(RootPath))
        {
            return Array.Empty<GameCoordinatorAppDefinition>();
        }

        var apps = new List<GameCoordinatorAppDefinition>();
        foreach (var directory in Directory.EnumerateDirectories(RootPath))
        {
            if (!TryParseAppId(directory, out var appId))
            {
                continue;
            }

            if (TryGetApp(appId, out var app))
            {
                apps.Add(app);
            }
        }

        return apps.OrderBy(app => app.AppId).ToList();
    }

    public static bool IsValidRoot(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
        {
            return false;
        }

        return Directory.EnumerateDirectories(path).Any(directory =>
        {
            try
            {
                return IsValidAppDirectory(directory);
            }
            catch
            {
                return false;
            }
        });
    }

    public static bool IsValidAppDirectory(string appRoot)
    {
        try
        {
            return TryParseAppId(appRoot, out var appId)
                && File.Exists(LoadApp(appId, appRoot).MainScriptPath);
        }
        catch (Exception ex) when (IsCatalogLoadException(ex))
        {
            return false;
        }
    }

    public static GameCoordinatorAppDefinition LoadApp(uint appId, string appRoot)
    {
        var root = Path.GetFullPath(appRoot);
        var manifestPath = Path.Combine(root, ManifestFileName);
        var manifest = File.Exists(manifestPath)
            ? JsonSerializer.Deserialize<GameCoordinatorAppManifest>(File.ReadAllText(manifestPath), JsonOptions) ?? new GameCoordinatorAppManifest()
            : new GameCoordinatorAppManifest();

        if (manifest.AppId is { } manifestAppId && manifestAppId != appId)
        {
            throw new InvalidOperationException(
                $"GC manifest appId {manifestAppId} does not match directory appId {appId}: {manifestPath}");
        }

        var entryPoint = NormalizeRelativePath(string.IsNullOrWhiteSpace(manifest.EntryPoint)
            ? DefaultEntryPoint
            : manifest.EntryPoint!);
        var mainScriptPath = ResolveContainedPath(root, entryPoint, "entryPoint");
        var hostServices = (manifest.HostServices ?? new List<string>())
            .Where(service => !string.IsNullOrWhiteSpace(service))
            .Select(service => service.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new GameCoordinatorAppDefinition(
            appId,
            string.IsNullOrWhiteSpace(manifest.Name) ? appId.ToString(CultureInfo.InvariantCulture) : manifest.Name!,
            root,
            entryPoint,
            mainScriptPath,
            manifestPath,
            hostServices,
            NormalizeProtoContracts(manifest.ProtoContracts),
            manifest.TypeScript ?? new GameCoordinatorTypeScriptOptions());
    }

    public static Assembly ResolveContractAssembly(
        GameCoordinatorAppDefinition app,
        GameCoordinatorProtoContractSource source,
        Assembly defaultAssembly)
    {
        var assembly = source.Assembly?.Trim();
        if (string.IsNullOrWhiteSpace(assembly)
            || string.Equals(assembly, "server", StringComparison.OrdinalIgnoreCase)
            || string.Equals(assembly, "default", StringComparison.OrdinalIgnoreCase)
            || string.Equals(assembly, defaultAssembly.GetName().Name, StringComparison.OrdinalIgnoreCase))
        {
            return defaultAssembly;
        }

        if (Path.IsPathRooted(assembly))
        {
            throw new InvalidOperationException(
                $"GC contract assembly must be server/default or an app-relative file: {assembly}");
        }

        var assemblyPath = ResolveContainedPath(
            app.RootPath,
            NormalizeRelativePath(assembly),
            "protoContracts.sources[].assembly");
        if (File.Exists(assemblyPath))
        {
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(assemblyPath));
        }

        var assemblyPathWithExtension = assembly.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
            ? assemblyPath
            : assemblyPath + ".dll";
        if (!string.Equals(assemblyPathWithExtension, assemblyPath, StringComparison.Ordinal)
            && File.Exists(assemblyPathWithExtension))
        {
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(assemblyPathWithExtension));
        }

        throw new InvalidOperationException(
            $"GC contract assembly was not found under app {app.AppId}: {assembly}");
    }

    private static bool TryParseAppId(string appRoot, out uint appId)
    {
        return uint.TryParse(Path.GetFileName(Path.TrimEndingDirectorySeparator(appRoot)), NumberStyles.None, CultureInfo.InvariantCulture, out appId);
    }

    private static string NormalizeRelativePath(string value)
    {
        return value.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
    }

    private static string ResolveContainedPath(string root, string relativePath, string fieldName)
    {
        if (Path.IsPathRooted(relativePath))
        {
            throw new InvalidOperationException($"GC manifest {fieldName} must be relative: {relativePath}");
        }

        var fullRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(fullRoot, relativePath));
        var comparison = PlatformPathComparison();
        if (!fullPath.StartsWith(fullRoot + Path.DirectorySeparatorChar, comparison)
            && !string.Equals(fullPath, fullRoot, comparison))
        {
            throw new InvalidOperationException($"GC manifest {fieldName} escapes the app root: {relativePath}");
        }

        return fullPath;
    }

    private static StringComparison PlatformPathComparison()
    {
        return OperatingSystem.IsWindows() || OperatingSystem.IsMacOS()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
    }

    private static bool IsCatalogLoadException(Exception ex)
    {
        return ex is IOException
            or UnauthorizedAccessException
            or InvalidOperationException
            or JsonException
            or NotSupportedException
            or ArgumentException
            or PathTooLongException;
    }

    private static GameCoordinatorProtoContractOptions NormalizeProtoContracts(GameCoordinatorProtoContractOptions? options)
    {
        options ??= new GameCoordinatorProtoContractOptions();
        options.Sources ??= new List<GameCoordinatorProtoContractSource>();

        foreach (var source in options.Sources)
        {
            source.TypeNames ??= new List<string>();
            source.TypeNamePrefixes ??= new List<string>();
            source.ContractNamePrefixes ??= new List<string>();
        }

        return options;
    }
}

public sealed record GameCoordinatorAppDefinition(
    uint AppId,
    string Name,
    string RootPath,
    string EntryPoint,
    string MainScriptPath,
    string ManifestPath,
    IReadOnlyList<string> HostServices,
    GameCoordinatorProtoContractOptions ProtoContracts,
    GameCoordinatorTypeScriptOptions TypeScript)
{
    public bool HasHostService(string serviceName)
    {
        return HostServices.Any(service => string.Equals(service, serviceName, StringComparison.OrdinalIgnoreCase));
    }

    public string RuntimeCacheKey
    {
        get
        {
            var hostServices = string.Join(',', HostServices.Order(StringComparer.OrdinalIgnoreCase));
            var contractSources = string.Join(';', ProtoContracts.Sources.Select(ProtoSourceCacheKey));
            return string.Join('|',
                AppId.ToString(CultureInfo.InvariantCulture),
                EntryPoint,
                FileIdentity(ManifestPath, includeContentHash: true),
                hostServices,
                contractSources,
                TypeScript.GeneratedContracts ?? string.Empty,
                TypeScript.ExtraMessageIds ?? string.Empty,
                TypeScript.Routes ?? string.Empty);
        }
    }

    private static string ProtoSourceCacheKey(GameCoordinatorProtoContractSource source)
    {
        return string.Join(',',
            source.Assembly ?? string.Empty,
            string.Join('+', source.TypeNames.Order(StringComparer.Ordinal)),
            string.Join('+', source.TypeNamePrefixes.Order(StringComparer.Ordinal)),
            string.Join('+', source.ContractNamePrefixes.Order(StringComparer.Ordinal)));
    }

    private static string FileIdentity(string path, bool includeContentHash)
    {
        if (!File.Exists(path))
        {
            return "missing";
        }

        var file = new FileInfo(path);
        var hash = includeContentHash
            ? Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)))
            : string.Empty;
        return string.Join(':',
            Path.GetFullPath(path),
            file.Length.ToString(CultureInfo.InvariantCulture),
            file.LastWriteTimeUtc.Ticks.ToString(CultureInfo.InvariantCulture),
            hash);
    }
}

public sealed class GameCoordinatorAppManifest
{
    public uint? AppId { get; set; }
    public string? Name { get; set; }
    public string? EntryPoint { get; set; }
    public List<string>? HostServices { get; set; }
    public GameCoordinatorProtoContractOptions? ProtoContracts { get; set; }
    public GameCoordinatorTypeScriptOptions? TypeScript { get; set; }
}

public sealed class GameCoordinatorProtoContractOptions
{
    public List<GameCoordinatorProtoContractSource> Sources { get; set; } = new();
}

public sealed class GameCoordinatorProtoContractSource
{
    public string? Assembly { get; set; }
    public List<string> TypeNames { get; set; } = new();
    public List<string> TypeNamePrefixes { get; set; } = new();
    public List<string> ContractNamePrefixes { get; set; } = new();
}

public sealed class GameCoordinatorTypeScriptOptions
{
    public string? GeneratedContracts { get; set; }
    public string? ExtraMessageIds { get; set; }
    public string? Routes { get; set; }
}
