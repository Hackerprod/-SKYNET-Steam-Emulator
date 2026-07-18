using System.Text.Json;
using Jint;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

/// <summary>
/// JS Game Coordinator engine (Fase 1 of the GC migration). Runs the committed
/// gc-js bundle (GC/&lt;appId&gt;/js/dist/gc.js, protobufjs/light + Long bundled in)
/// under Jint with the proto descriptor generated from proto/.
///
/// Concurrency model (Fase 1 decision, option (a)): Jint.Engine is NOT
/// thread-safe, so each app gets a single engine guarded by a global lock.
/// All GC state lives in the host/DB (regla 10), the engine is stateless, and
/// the spike numbers (SOCache round-trip p50 ~35ms) make serialization
/// acceptable at this scale. Hot-reload builds the new engine off-lock and
/// swaps it atomically: in-flight calls finish on the old engine instance.
///
/// Sandbox: no CLR access, no fs/network/timers. The only injected capability
/// is `log`. Fixtures are never exposed here (regla 9: runtime.fixture does
/// not exist in production).
/// </summary>
public sealed class JintGcEngine
{
    private readonly ILogger<JintGcEngine> _logger;
    private readonly GameCoordinatorTraceService _trace;
    private readonly GcClientVersionCache _clientVersions;
    private readonly string _gcRoot;
    private readonly TimeSpan _timeout;
    private readonly object _cacheSync = new();
    private readonly Dictionary<uint, CachedJsEngine> _cache = new();

    public JintGcEngine(
        IHostEnvironment hostEnvironment,
        IConfiguration configuration,
        ILogger<JintGcEngine> logger,
        GameCoordinatorTraceService trace,
        GcClientVersionCache clientVersions)
    {
        _logger = logger;
        _trace = trace;
        _clientVersions = clientVersions;
        _gcRoot = GcPaths.ResolveGcRoot(hostEnvironment.ContentRootPath);
        var timeoutMs = configuration.GetValue("GameCoordinator:JsTimeoutMs", 5000);
        _timeout = TimeSpan.FromMilliseconds(Math.Clamp(timeoutMs, 250, 60000));
    }

    public bool IsAvailable(uint appId)
        => File.Exists(BundlePath(appId)) && File.Exists(DescriptorPath(appId));

    public ApiGCExchangeResponse Exchange(
        GameCoordinatorContext context,
        ApiGCExchangeRequest request,
        out string? failureReason)
    {
        failureReason = null;
        CachedJsEngine entry;
        try
        {
            entry = GetOrLoadEngine(context.AppId);
        }
        catch (Exception ex)
        {
            failureReason = $"js engine load failed: {ex.Message}";
            _logger.LogError(ex, "GC JS engine load failed for app {AppId}", context.AppId);
            return new ApiGCExchangeResponse { Handled = false };
        }

        var envelope = JsonSerializer.Serialize(new
        {
            appId = context.AppId,
            steamId = context.SteamId.ToString(),
            accountId = context.AccountId,
            personaName = context.PersonaName,
            clientIp = context.ClientIp,
            messageType = request.MessageType,
            sourceJobId = request.SourceJobId?.ToString(),
            targetJobId = (string?)null,
            isGameServer = request.GameServer,
            clientVersion = _clientVersions.Get(context.AppId, context.SteamId),
            payloadBase64 = request.BodyBase64 ?? string.Empty,
        });

        string resultJson;
        try
        {
            lock (entry.Sync)
            {
                resultJson = entry.Engine.Invoke("__dispatch", envelope).AsString();
            }
        }
        catch (Exception ex)
        {
            // A throwing/timed-out handler must never take the server down: log,
            // trace, and let the router fall back to the Lua/C# engine.
            failureReason = $"js dispatch threw: {ex.Message}";
            _logger.LogError(ex, "GC JS dispatch failed for app {AppId}, message {MessageType}", context.AppId, request.MessageType);
            _trace.Record("error", context.AppId, context.SteamId, request.MessageType, 0, $"js: {ex.Message}");
            return new ApiGCExchangeResponse { Handled = false };
        }

        JsDispatchResult? result;
        try
        {
            result = JsonSerializer.Deserialize<JsDispatchResult>(resultJson, ResultJsonOptions);
        }
        catch (Exception ex)
        {
            failureReason = $"js result parse failed: {ex.Message}";
            _logger.LogError(ex, "GC JS returned malformed dispatch result for app {AppId}", context.AppId);
            return new ApiGCExchangeResponse { Handled = false };
        }

        if (result == null || !result.Handled)
        {
            failureReason = result?.Reason ?? "js returned no result";
            return new ApiGCExchangeResponse { Handled = false };
        }

        var response = new ApiGCExchangeResponse { Handled = true };
        foreach (var message in result.Messages ?? new List<JsEmittedMessage>())
        {
            var apiMessage = new ApiGCMessage
            {
                AppId = context.AppId,
                MessageType = message.MsgId,
                PayloadBase64 = message.PayloadBase64 ?? string.Empty,
                Protobuf = true,
            };

            switch (message.Kind)
            {
                case "reply":
                    apiMessage.TargetJobId = request.SourceJobId;
                    response.Messages.Add(apiMessage);
                    break;
                case "proto":
                    response.Messages.Add(apiMessage);
                    break;
                case "queueReplyTo":
                    if (ulong.TryParse(message.TargetSteamId, out var replyTarget))
                    {
                        if (ulong.TryParse(message.TargetJobId, out var jobId))
                        {
                            apiMessage.TargetJobId = jobId;
                        }

                        LuaGameCoordinatorBackend.PendingMessageQueued?.Invoke(replyTarget, apiMessage);
                    }
                    break;
                case "queueToServer":
                    if (ulong.TryParse(message.TargetSteamId, out var serverTarget))
                    {
                        GameCoordinatorPendingMessages.Enqueue(context.AppId, serverTarget, apiMessage);
                    }
                    break;
                default:
                    _logger.LogWarning("GC JS emitted unknown message kind {Kind}", message.Kind);
                    break;
            }
        }

        return response;
    }

    /// <summary>
    /// Poll served by the JS engine. Until lobby/session state migrates
    /// (Fase 4), gc-routing.json keeps pollEngine=lua and this is not called.
    /// </summary>
    public ApiGCExchangeResponse Poll(GameCoordinatorContext context)
    {
        return new ApiGCExchangeResponse { Handled = true };
    }

    public void Tick()
    {
        List<(uint AppId, CachedJsEngine Entry)> entries;
        lock (_cacheSync)
        {
            entries = _cache.Select(pair => (pair.Key, pair.Value)).ToList();
        }

        foreach (var (appId, entry) in entries)
        {
            try
            {
                lock (entry.Sync)
                {
                    entry.Engine.Invoke("__tick", JsonSerializer.Serialize(new { appId }));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GC JS tick failed for app {AppId}", appId);
                _trace.Record("error", appId, 0, 0, 0, $"js tick: {ex.Message}");
            }
        }
    }

    private CachedJsEngine GetOrLoadEngine(uint appId)
    {
        var stamp = GetArtifactStamp(appId);
        lock (_cacheSync)
        {
            if (_cache.TryGetValue(appId, out var cached) && cached.StampUtc == stamp)
            {
                return cached;
            }
        }

        // Build outside the cache lock so a slow load never blocks dispatches
        // for other apps; the swap below is atomic and in-flight calls keep
        // running on the engine instance they already hold.
        var fresh = BuildEngine(appId, stamp);
        lock (_cacheSync)
        {
            if (_cache.TryGetValue(appId, out var raced) && raced.StampUtc == stamp)
            {
                return raced; // another thread won the rebuild race
            }

            _cache[appId] = fresh;
            _logger.LogInformation("GC JS engine loaded for app {AppId} (bundle stamp {Stamp:o})", appId, stamp);
            return fresh;
        }
    }

    private CachedJsEngine BuildEngine(uint appId, DateTime stamp)
    {
        var descriptor = File.ReadAllText(DescriptorPath(appId));
        var bundle = File.ReadAllText(BundlePath(appId));

        var engine = new Engine(options =>
        {
            options.LimitMemory(128 * 1024 * 1024);
            options.TimeoutInterval(_timeout);
            options.LimitRecursion(512);
        });
        engine.SetValue("log", new Action<string>(message =>
        {
            _logger.LogInformation("GC JS {AppId}: {Message}", appId, message);
            _trace.Record("log", appId, 0, 0, 0, message);
        }));
        engine.SetValue("__PROTO_DESCRIPTOR__", descriptor);
        engine.Execute(bundle);
        return new CachedJsEngine(engine, stamp);
    }

    private DateTime GetArtifactStamp(uint appId)
    {
        var bundleStamp = File.GetLastWriteTimeUtc(BundlePath(appId));
        var descriptorStamp = File.GetLastWriteTimeUtc(DescriptorPath(appId));
        return bundleStamp > descriptorStamp ? bundleStamp : descriptorStamp;
    }

    private string BundlePath(uint appId) => Path.Combine(_gcRoot, appId.ToString(), "js", "dist", "gc.js");
    private string DescriptorPath(uint appId) => Path.Combine(_gcRoot, appId.ToString(), "js", "proto-descriptor.json");

    private static readonly JsonSerializerOptions ResultJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private sealed class CachedJsEngine
    {
        public CachedJsEngine(Engine engine, DateTime stampUtc)
        {
            Engine = engine;
            StampUtc = stampUtc;
        }

        public Engine Engine { get; }
        public DateTime StampUtc { get; }
        public object Sync { get; } = new();
    }

    private sealed class JsDispatchResult
    {
        public bool Handled { get; set; }
        public string? Reason { get; set; }
        public List<JsEmittedMessage>? Messages { get; set; }
    }

    private sealed class JsEmittedMessage
    {
        public string Kind { get; set; } = string.Empty;
        public uint MsgId { get; set; }
        public string? PayloadBase64 { get; set; }
        public string? TargetSteamId { get; set; }
        public string? TargetJobId { get; set; }
    }
}
