using MoonSharp.Interpreter;
using SKYNET_server.GC.Dota2;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed class LuaGameCoordinatorPlugin : IGameCoordinatorPlugin, IGameCoordinatorTicker
{
    private readonly ILogger<LuaGameCoordinatorPlugin> _logger;
    private readonly GameCoordinatorTraceService _trace;
    private readonly string _gcRoot;
    private readonly object _scriptCacheSync = new();
    private readonly Dictionary<uint, CachedLuaScript> _scriptCache = new();

    static LuaGameCoordinatorPlugin()
    {
        UserData.RegisterType<LuaGameCoordinatorBackend>();
        UserData.RegisterType<LuaGameCoordinatorRuntime>();
        UserData.RegisterType<DotaGcBackend>();
        UserData.RegisterType<LuaSteamDB>();
        UserData.RegisterType<LuaDotaDB>();
        UserData.RegisterType<LuaDedicatedServerService>();
    }

    private readonly SteamDB _steamDb;
    private readonly DotaDB _dotaDb;
    private readonly DedicatedServerService _dedicatedServerService;

    public LuaGameCoordinatorPlugin(
        IHostEnvironment hostEnvironment,
        ILogger<LuaGameCoordinatorPlugin> logger,
        GameCoordinatorTraceService trace,
        SteamDB steamDb,
        DotaDB dotaDb,
        DedicatedServerService dedicatedServerService)
    {
        _logger = logger;
        _trace = trace;
        _steamDb = steamDb;
        _dotaDb = dotaDb;
        _dedicatedServerService = dedicatedServerService;
        _gcRoot = ResolveGcRoot(hostEnvironment.ContentRootPath);
        _logger.LogInformation("GC root resolved to {GCRoot}", _gcRoot);
    }

    public bool CanHandle(uint appId)
    {
        var scriptRoot = GetScriptRoot(appId);
        if (File.Exists(Path.Combine(scriptRoot, "main.ts")))
        {
            return false;
        }

        return File.Exists(Path.Combine(scriptRoot, "main.lua"));
    }

    public ApiGCExchangeResponse Exchange(GameCoordinatorContext context, ApiGCExchangeRequest request)
    {
        var scriptRoot = GetScriptRoot(context.AppId);
        var scriptPath = Path.Combine(scriptRoot, "main.lua");
        if (!File.Exists(scriptPath))
        {
            return new ApiGCExchangeResponse { Handled = false };
        }

        ILuaGameCoordinatorBackend backend = CreateBackend(context, request);
        var runtime = new LuaGameCoordinatorRuntime(scriptRoot, _logger, context, _trace);
        _trace.Record("in", context.AppId, context.SteamId, request.MessageType,
            GameCoordinatorTraceService.EstimatePayloadSize(request.BodyBase64), context.PersonaName);

        try
        {
            var cacheEntry = GetCachedScript(context.AppId, scriptRoot, scriptPath, backend, runtime);
            lock (cacheEntry.Sync)
            {
                var script = cacheEntry.Script;
                script.Globals["gc"] = UserData.Create(backend);
                script.Globals["runtime"] = UserData.Create(runtime);
                SetDatabaseGlobals(script, context);

                var handler = script.Globals.Get("handle");
                if (handler.Type != DataType.Function)
                {
                    return new ApiGCExchangeResponse { Handled = false };
                }

                var handled = script.Call(handler).CastToBool();
                if (!handled)
                {
                    _trace.Record("unhandled", context.AppId, context.SteamId, request.MessageType, 0);
                    return new ApiGCExchangeResponse { Handled = false };
                }

                foreach (var message in backend.Response.Messages)
                {
                    _trace.Record("out", context.AppId, context.SteamId, message.MessageType,
                        GameCoordinatorTraceService.EstimatePayloadSize(message.PayloadBase64),
                        message.TargetJobId == null ? string.Empty : $"job {message.TargetJobId}");
                }

                return backend.Response;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GC Lua script failed for app {AppId}, message {MessageType}", context.AppId, request.MessageType);
            _trace.Record("error", context.AppId, context.SteamId, request.MessageType, 0, ex.Message);
            return new ApiGCExchangeResponse { Handled = false };
        }
    }

    public ApiGCExchangeResponse Poll(GameCoordinatorContext context)
    {
        var response = context.AppId == 570
            ? DotaGcBackend.Poll(context)
            : new ApiGCExchangeResponse { Handled = true };

        response.Messages.AddRange(GameCoordinatorPendingMessages.Drain(context.AppId, context.SteamId));
        return response;
    }

    public void Tick()
    {
        List<uint> appIds;
        lock (_scriptCacheSync)
        {
            appIds = _scriptCache.Keys.ToList();
        }

        foreach (var appId in appIds)
        {
            var scriptRoot = GetScriptRoot(appId);
            var scriptPath = Path.Combine(scriptRoot, "main.lua");
            if (!File.Exists(scriptPath))
            {
                continue;
            }

            var context = new GameCoordinatorContext { AppId = appId };
            var backend = CreateBackend(context, new ApiGCExchangeRequest { AppId = appId });
            var runtime = new LuaGameCoordinatorRuntime(scriptRoot, _logger, context, _trace);

            try
            {
                var cacheEntry = GetCachedScript(appId, scriptRoot, scriptPath, backend, runtime);
                lock (cacheEntry.Sync)
                {
                    var script = cacheEntry.Script;
                    var handler = script.Globals.Get("tick");
                    if (handler.Type != DataType.Function)
                    {
                        continue;
                    }

                    script.Globals["gc"] = UserData.Create(backend);
                    script.Globals["runtime"] = UserData.Create(runtime);
                    SetDatabaseGlobals(script, context);
                    script.Call(handler);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GC Lua tick failed for app {AppId}", appId);
                _trace.Record("error", appId, 0, 0, 0, $"tick: {ex.Message}");
            }
        }
    }

    private ILuaGameCoordinatorBackend CreateBackend(GameCoordinatorContext context, ApiGCExchangeRequest request)
    {
        if (context.AppId == 570)
        {
            return new DotaGcBackend(context, request);
        }

        return new LuaGameCoordinatorBackend(context, request);
    }

    private DynValue Include(Script script, string scriptRoot, string relativePath)
    {
        var fullPath = ResolveScriptPath(scriptRoot, relativePath);
        return script.DoString(File.ReadAllText(fullPath), null, fullPath);
    }

    private CachedLuaScript GetCachedScript(
        uint appId,
        string scriptRoot,
        string scriptPath,
        ILuaGameCoordinatorBackend backend,
        LuaGameCoordinatorRuntime runtime)
    {
        var latestWriteUtc = GetScriptTreeStamp(scriptRoot);
        lock (_scriptCacheSync)
        {
            if (_scriptCache.TryGetValue(appId, out var cached) && cached.LastWriteUtc == latestWriteUtc)
            {
                return cached;
            }

            var script = new Script(CoreModules.Preset_SoftSandbox)
            {
                Options =
                {
                    DebugPrint = message => _logger.LogInformation("GC Lua {AppId}: {Message}", appId, message)
                }
            };

            script.Globals["include"] = (Func<string, DynValue>)(relativePath => Include(script, scriptRoot, relativePath));
            script.Globals["gc"] = UserData.Create(backend);
            script.Globals["runtime"] = UserData.Create(runtime);
            SetDatabaseGlobals(script, runtime.Context);
            script.DoString(File.ReadAllText(scriptPath), null, scriptPath);
            cached = new CachedLuaScript(script, latestWriteUtc);
            _scriptCache[appId] = cached;
            _logger.LogInformation("GC Lua script loaded for app {AppId} from {ScriptRoot}", appId, scriptRoot);
            return cached;
        }
    }

    private void SetDatabaseGlobals(Script script, GameCoordinatorContext context)
    {
        var steamDb = UserData.Create(_steamDb.ForLua(context));
        var dotaDb = UserData.Create(_dotaDb.ForLua(context));
        var dedicatedServerService = UserData.Create(_dedicatedServerService.ForLua(context));

        script.Globals["SteamDB"] = steamDb;
        script.Globals["steamDB"] = steamDb;
        script.Globals["DotaDB"] = dotaDb;
        script.Globals["dotaDB"] = dotaDb;
        script.Globals["DedicatedServerService"] = dedicatedServerService;
        script.Globals["dedicatedServerService"] = dedicatedServerService;
    }

    private static DateTime GetScriptTreeStamp(string scriptRoot)
    {
        return Directory
            .EnumerateFiles(scriptRoot, "*.lua", SearchOption.AllDirectories)
            .Select(File.GetLastWriteTimeUtc)
            .DefaultIfEmpty(DateTime.MinValue)
            .Max();
    }

    private string GetMainScriptPath(uint appId)
    {
        return Path.Combine(GetScriptRoot(appId), "main.lua");
    }

    private string GetScriptRoot(uint appId)
    {
        return Path.Combine(_gcRoot, appId.ToString());
    }

    private static string ResolveGcRoot(string contentRootPath)
    {
        var configuredRoot = Environment.GetEnvironmentVariable("SKYNET_GC_ROOT");
        if (IsValidGcRoot(configuredRoot))
        {
            return Path.GetFullPath(configuredRoot!);
        }

        var current = new DirectoryInfo(contentRootPath);
        while (current != null)
        {
            var candidate = Path.Combine(current.FullName, "GC");
            if (IsValidGcRoot(candidate))
            {
                return candidate;
            }

            current = current.Parent;
        }

        return Path.Combine(contentRootPath, "GC");
    }

    private static bool IsValidGcRoot(string? path)
    {
        return !string.IsNullOrWhiteSpace(path)
            && (File.Exists(Path.Combine(path, "570", "main.ts"))
                || File.Exists(Path.Combine(path, "570", "main.lua")));
    }

    private static string ResolveScriptPath(string scriptRoot, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new InvalidOperationException("GC Lua include path is empty.");
        }

        var root = Path.GetFullPath(scriptRoot);
        var normalized = relativePath.Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(root, normalized));
        if (!fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"GC Lua include escapes plugin root: {relativePath}");
        }

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"GC Lua include was not found: {relativePath}", fullPath);
        }

        return fullPath;
    }

    private sealed class CachedLuaScript
    {
        public CachedLuaScript(Script script, DateTime lastWriteUtc)
        {
            Script = script;
            LastWriteUtc = lastWriteUtc;
        }

        public Script Script { get; }
        public DateTime LastWriteUtc { get; }
        public object Sync { get; } = new();
    }
}
