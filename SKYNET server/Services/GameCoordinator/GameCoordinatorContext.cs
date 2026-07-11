using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed class GameCoordinatorContext
{
    public uint AppId { get; init; }
    public ulong SteamId { get; init; }
    public uint AccountId { get; init; }
    public string PersonaName { get; init; } = string.Empty;
}

public interface IGameCoordinatorPlugin
{
    bool CanHandle(uint appId);
    SkyNetGCExchangeResponseDto Exchange(GameCoordinatorContext context, SkyNetGCExchangeRequestDto request);
    SkyNetGCExchangeResponseDto Poll(GameCoordinatorContext context);
}

public interface ILuaGameCoordinatorBackend
{
    SkyNetGCExchangeResponseDto Response { get; }
}

public static class GameCoordinatorPendingMessages
{
    private const int MaxQueuedPerRecipient = 256;
    private static readonly object Sync = new();
    private static readonly Dictionary<(uint AppId, ulong SteamId), Queue<SkyNetGCMessageDto>> Queues = new();

    public static void Enqueue(uint appId, ulong steamId, SkyNetGCMessageDto message)
    {
        lock (Sync)
        {
            if (!Queues.TryGetValue((appId, steamId), out var queue))
            {
                queue = new Queue<SkyNetGCMessageDto>();
                Queues[(appId, steamId)] = queue;
            }

            queue.Enqueue(message);
            while (queue.Count > MaxQueuedPerRecipient)
            {
                queue.Dequeue();
            }
        }
    }

    public static List<SkyNetGCMessageDto> Drain(uint appId, ulong steamId)
    {
        lock (Sync)
        {
            if (!Queues.TryGetValue((appId, steamId), out var queue) || queue.Count == 0)
            {
                return new List<SkyNetGCMessageDto>();
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

    public SkyNetGCExchangeResponseDto Exchange(GameCoordinatorContext context, SkyNetGCExchangeRequestDto request)
    {
        var plugin = _plugins.FirstOrDefault(candidate => candidate.CanHandle(context.AppId));
        if (plugin == null)
        {
            return new SkyNetGCExchangeResponseDto();
        }

        return plugin.Exchange(context, request) ?? new SkyNetGCExchangeResponseDto();
    }

    public SkyNetGCExchangeResponseDto Poll(GameCoordinatorContext context)
    {
        var plugin = _plugins.FirstOrDefault(candidate => candidate.CanHandle(context.AppId));
        if (plugin == null)
        {
            return new SkyNetGCExchangeResponseDto();
        }

        return plugin.Poll(context) ?? new SkyNetGCExchangeResponseDto();
    }
}
