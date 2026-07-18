using SKYNET_server.Models;

namespace SKYNET_server.Services;

/// <summary>
/// Per-message GC engine router (Fase B of the GC migration). Replaces the
/// appId-only plugin selection: every Exchange AND Poll consults the routing
/// table (GC/gc-routing.json, hot-reloadable) to decide which engine serves
/// the message - the new Jint/JS engine for migrated ids, the existing Lua/C#
/// plugin for everything else. With zero migrated ids the behavior is
/// identical to the pre-router server (Fase B gate).
///
/// Fallback rule: if the JS engine does not handle a routed message (throw,
/// timeout, notHandled), the router logs the reason and falls back to Lua/C#.
/// The GC never goes down because of a handler.
/// </summary>
public sealed class GcEngineRouter : IGameCoordinatorPlugin
{
    private const uint MsgGcClientHello = 4006;
    private const uint MsgGcGameServerHello = 4007;

    private readonly LuaGameCoordinatorPlugin _luaEngine;
    private readonly JintGcEngine _jsEngine;
    private readonly GcRoutingTable _routing;
    private readonly GcClientVersionCache _clientVersions;
    private readonly GameCoordinatorTraceService _trace;
    private readonly ILogger<GcEngineRouter> _logger;

    public GcEngineRouter(
        LuaGameCoordinatorPlugin luaEngine,
        JintGcEngine jsEngine,
        GcRoutingTable routing,
        GcClientVersionCache clientVersions,
        GameCoordinatorTraceService trace,
        ILogger<GcEngineRouter> logger)
    {
        _luaEngine = luaEngine;
        _jsEngine = jsEngine;
        _routing = routing;
        _clientVersions = clientVersions;
        _trace = trace;
        _logger = logger;
    }

    public bool CanHandle(uint appId)
    {
        return _luaEngine.CanHandle(appId) || _jsEngine.IsAvailable(appId);
    }

    public ApiGCExchangeResponse Exchange(GameCoordinatorContext context, ApiGCExchangeRequest request)
    {
        CaptureClientVersion(context, request);

        if (_routing.IsMigratedToJs(context.AppId, request.MessageType) && _jsEngine.IsAvailable(context.AppId))
        {
            var response = _jsEngine.Exchange(context, request, out var failureReason);
            if (response.Handled)
            {
                _trace.Record("route", context.AppId, context.SteamId, request.MessageType, 0, "engine=js");
                return response;
            }

            _trace.Record("route", context.AppId, context.SteamId, request.MessageType, 0,
                $"engine=js fallback=lua reason={failureReason}");
            _logger.LogWarning(
                "GC JS engine did not handle migrated message {MessageType} for app {AppId} ({Reason}); falling back to Lua/C#",
                request.MessageType, context.AppId, failureReason);
        }

        return _luaEngine.Exchange(context, request);
    }

    public ApiGCExchangeResponse Poll(GameCoordinatorContext context)
    {
        // Poll goes through the same routing decision as Exchange (regla 8).
        // pollEngine stays "lua" until the lobby/session state that
        // DotaGcBackend.Poll serves today is reclaimed by the JS engine
        // (Fase 4/5); flipping it back is a hot-reload of gc-routing.json.
        if (_routing.PollEngine(context.AppId) == "js" && _jsEngine.IsAvailable(context.AppId))
        {
            var response = _jsEngine.Poll(context);
            response.Messages.AddRange(GameCoordinatorPendingMessages.Drain(context.AppId, context.SteamId));
            return response;
        }

        return _luaEngine.Poll(context);
    }

    public void Tick()
    {
        _luaEngine.Tick();
        _jsEngine.Tick();
    }

    private void CaptureClientVersion(GameCoordinatorContext context, ApiGCExchangeRequest request)
    {
        if (request.MessageType != MsgGcClientHello && request.MessageType != MsgGcGameServerHello)
        {
            return;
        }

        try
        {
            var payload = string.IsNullOrEmpty(request.BodyBase64)
                ? Array.Empty<byte>()
                : Convert.FromBase64String(request.BodyBase64);
            var version = GcClientVersionCache.TryParseHelloVersion(payload);
            if (version.HasValue)
            {
                _clientVersions.Set(context.AppId, context.SteamId, version.Value);
            }
        }
        catch (FormatException)
        {
            // malformed base64: leave the cached version untouched
        }
    }
}
