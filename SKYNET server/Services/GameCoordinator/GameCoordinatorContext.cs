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

    public GameCoordinatorPluginRegistry(IEnumerable<IGameCoordinatorPlugin> plugins)
    {
        _plugins = plugins.ToList();
    }

    public ApiGCExchangeResponse Exchange(GameCoordinatorContext context, ApiGCExchangeRequest request)
    {
        var plugin = _plugins.FirstOrDefault(candidate => candidate.CanHandle(context.AppId));
        if (plugin == null)
        {
            return new ApiGCExchangeResponse();
        }

        return plugin.Exchange(context, request) ?? new ApiGCExchangeResponse();
    }

    public ApiGCExchangeResponse Poll(GameCoordinatorContext context)
    {
        var plugin = _plugins.FirstOrDefault(candidate => candidate.CanHandle(context.AppId));
        if (plugin == null)
        {
            return new ApiGCExchangeResponse();
        }

        return plugin.Poll(context) ?? new ApiGCExchangeResponse();
    }
}
