namespace SKYNET_server.Services;

public sealed class GameCoordinatorMessageDecoder
{
    private readonly GameCoordinatorProtoCodec _codec;

    public GameCoordinatorMessageDecoder(GameCoordinatorProtoCodec codec)
    {
        _codec = codec;
    }

    public bool TryDecode(
        IGameCoordinatorPlugin plugin,
        uint appId,
        uint messageType,
        byte[] payload,
        out string typeName,
        out string json,
        out string reason)
    {
        typeName = string.Empty;
        json = string.Empty;
        reason = string.Empty;

        if (plugin is not IGameCoordinatorMessageTypeCatalog catalog)
        {
            reason = "plugin does not expose a message-type catalog";
            return false;
        }

        if (!catalog.TryDescribeMessageType(appId, messageType, out var descriptor))
        {
            reason = $"no mapping for MessageType {messageType}";
            return false;
        }

        typeName = descriptor.ProtoTypeName;
        try
        {
            json = _codec.DecodeToJson(appId, descriptor.ProtoTypeName, payload);
            return true;
        }
        catch (Exception ex)
        {
            reason = ex.Message.Length <= 300 ? ex.Message : ex.Message[..300];
            return false;
        }
    }
}
