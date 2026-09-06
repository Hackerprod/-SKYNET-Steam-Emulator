using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed class GameCoordinatorContext
{
    public uint AppId { get; init; }
    public ulong SteamId { get; init; }
    public uint AccountId { get; init; }
    public string PersonaName { get; init; } = string.Empty;

    // Source IP of the machine sending this GC exchange. Used so a launched game
    // server advertises the launcher's reachable address (LAN/ZeroTier) instead
    // of 127.0.0.1, letting other players connect to the match.
    public string ClientIp { get; init; } = string.Empty;
}

public interface IGameCoordinatorPlugin
{
    bool CanHandle(uint appId);
    ApiGCExchangeResponse Exchange(GameCoordinatorContext context, ApiGCExchangeRequest request);
    ApiGCExchangeResponse Poll(GameCoordinatorContext context);
}

/// <summary>
/// Optional capability used by the admin GC message inspector to resolve a
/// numeric message type without hardcoding any game's numbering scheme.
/// </summary>
public interface IGameCoordinatorMessageTypeCatalog
{
    bool TryDescribeMessageType(uint appId, uint messageType, out GcMessageTypeDescriptor descriptor);
}

public readonly record struct GcMessageTypeDescriptor(string DisplayName, string ProtoTypeName);

public interface IGameCoordinatorTicker
{
    void Tick();
}

public static class GameCoordinatorPendingMessages
{
    private const int MaxQueuedPerRecipient = 256;
    private static readonly object Sync = new();
    private static readonly Dictionary<(uint AppId, ulong SteamId), Queue<ApiGCMessage>> Queues = new();

    public static void Enqueue(uint appId, ulong steamId, ApiGCMessage message)
    {
        lock (Sync)
        {
            if (!Queues.TryGetValue((appId, steamId), out var queue))
            {
                queue = new Queue<ApiGCMessage>();
                Queues[(appId, steamId)] = queue;
            }

            queue.Enqueue(message);
            while (queue.Count > MaxQueuedPerRecipient)
            {
                queue.Dequeue();
            }
        }
    }

    public static List<ApiGCMessage> Drain(uint appId, ulong steamId)
    {
        lock (Sync)
        {
            if (!Queues.TryGetValue((appId, steamId), out var queue) || queue.Count == 0)
            {
                return new List<ApiGCMessage>();
            }

            var drained = queue.ToList();
            Queues.Remove((appId, steamId));
            return drained;
        }
    }
}

public sealed class GameCoordinatorPluginRegistry
{
    private readonly List<IGameCoordinatorPlugin> _plugins;
    private readonly GameCoordinatorTraceService _trace;
    private readonly GameCoordinatorMessageDecoder _decoder;

    public GameCoordinatorPluginRegistry(
        IEnumerable<IGameCoordinatorPlugin> plugins,
        GameCoordinatorTraceService trace,
        GameCoordinatorMessageDecoder decoder)
    {
        _plugins = plugins.ToList();
        _trace = trace;
        _decoder = decoder;
    }

    public ApiGCExchangeResponse Exchange(GameCoordinatorContext context, ApiGCExchangeRequest request)
    {
        var plugin = _plugins.FirstOrDefault(candidate => candidate.CanHandle(context.AppId));
        RecordRequest(context, request, plugin);
        if (plugin == null)
        {
            RecordUnhandled(context, request);
            return new ApiGCExchangeResponse();
        }

        var response = plugin.Exchange(context, request) ?? new ApiGCExchangeResponse();
        RecordResponses(context, plugin, response, "out");
        if (!response.Handled)
        {
            RecordUnhandled(context, request);
        }

        return response;
    }

    public ApiGCExchangeResponse Poll(GameCoordinatorContext context)
    {
        var plugin = _plugins.FirstOrDefault(candidate => candidate.CanHandle(context.AppId));
        if (plugin == null)
        {
            return new ApiGCExchangeResponse();
        }

        var response = plugin.Poll(context) ?? new ApiGCExchangeResponse();
        RecordResponses(context, plugin, response, "push");
        return response;
    }

    private void RecordRequest(
        GameCoordinatorContext context,
        ApiGCExchangeRequest request,
        IGameCoordinatorPlugin? plugin)
    {
        RecordMessage(
            "in",
            context,
            request.MessageType,
            request.BodyBase64,
            request.SourceJobId,
            null,
            protobuf: false,
            detail: context.PersonaName,
            plugin: plugin);
    }

    private void RecordResponses(
        GameCoordinatorContext context,
        IGameCoordinatorPlugin plugin,
        ApiGCExchangeResponse response,
        string kind)
    {
        foreach (var message in response.Messages)
        {
            var detail = message.TargetJobId is { } targetJobId
                ? $"job {targetJobId}"
                : string.Empty;
            RecordMessage(
                kind,
                context,
                message.MessageType,
                message.PayloadBase64,
                null,
                message.TargetJobId,
                message.Protobuf,
                detail,
                plugin);
        }
    }

    private void RecordUnhandled(GameCoordinatorContext context, ApiGCExchangeRequest request)
    {
        _trace.Record(
            "unhandled",
            context.AppId,
            context.SteamId,
            request.MessageType,
            0,
            context.PersonaName);
    }

    private void RecordMessage(
        string kind,
        GameCoordinatorContext context,
        uint messageType,
        string? payloadBase64,
        ulong? sourceJobId,
        ulong? targetJobId,
        bool protobuf,
        string detail,
        IGameCoordinatorPlugin? plugin)
    {
        var payload = TryDecodePayload(payloadBase64, out var payloadError);
        string? decodedTypeName = null;
        string? decodedJson = null;
        string? decodeError = payloadError;
        if (payload is { Length: > 0 } && plugin != null)
        {
            if (_decoder.TryDecode(plugin, context.AppId, messageType, payload,
                    out var typeName, out var json, out var reason))
            {
                decodedTypeName = typeName;
                decodedJson = json;
            }
            else
            {
                decodedTypeName = string.IsNullOrEmpty(typeName) ? null : typeName;
                decodeError = reason;
            }
        }

        _trace.Record(
            kind,
            context.AppId,
            context.SteamId,
            messageType,
            payload?.Length ?? GameCoordinatorTraceService.EstimatePayloadSize(payloadBase64),
            detail,
            payloadBase64,
            sourceJobId,
            targetJobId,
            protobuf,
            decodedTypeName,
            decodedJson,
            decodeError);
    }

    private static byte[]? TryDecodePayload(string? payloadBase64, out string? error)
    {
        error = null;
        if (string.IsNullOrEmpty(payloadBase64))
        {
            return Array.Empty<byte>();
        }

        try
        {
            return Convert.FromBase64String(payloadBase64);
        }
        catch (FormatException)
        {
            error = "payload is not valid base64";
            return null;
        }
    }
}
