using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using ProtoBuf;

namespace SKYNET_server.Services.Networking;

public sealed class SdrRelayService : BackgroundService
{
    private const byte MsgRouterPingRequest = 1;
    private const byte MsgRouterPingReply = 2;

    private readonly ILogger<SdrRelayService> _logger;
    private readonly int _port;
    private ulong _challenge;

    public SdrRelayService(
        IConfiguration configuration,
        ILogger<SdrRelayService> logger)
    {
        _logger = logger;
        _port = int.TryParse(configuration["Sdr:RelayPort"], out var p)
            ? p
            : 28009;

        _challenge = 1;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var socket = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Dgram,
            ProtocolType.Udp);

        socket.SetSocketOption(
            SocketOptionLevel.Socket,
            SocketOptionName.ReuseAddress,
            true);

        try
        {
            socket.Bind(new IPEndPoint(IPAddress.Any, _port));
        }
        catch (SocketException ex)
        {
            _logger.LogError(
                ex,
                "SDR relay failed to bind UDP port {Port}",
                _port);

            return;
        }

        _logger.LogInformation(
            "SDR relay listening on UDP 0.0.0.0:{Port}",
            _port);

        var buffer = new byte[4096];
        var remote = new IPEndPoint(IPAddress.Any, 0);

        while (!stoppingToken.IsCancellationRequested)
        {
            SocketReceiveFromResult result;

            try
            {
                result = await socket.ReceiveFromAsync(
                    buffer,
                    SocketFlags.None,
                    remote,
                    stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (SocketException)
            {
                continue;
            }

            if (result.ReceivedBytes < 1 ||
                result.RemoteEndPoint is not IPEndPoint sender)
            {
                continue;
            }

            try
            {
                HandlePacket(
                    socket,
                    buffer,
                    result.ReceivedBytes,
                    sender);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(
                    ex,
                    "SDR relay failed to process packet from {Sender}",
                    sender);
            }
        }
    }

    private void HandlePacket(
        Socket socket,
        byte[] data,
        int length,
        IPEndPoint sender)
    {
        var msgId = data[0];

        if (msgId != MsgRouterPingRequest)
        {
            _logger.LogDebug(
                "SDR relay: unhandled datagram msg {Msg} from {Sender}",
                msgId,
                sender);

            return;
        }

        // Пока сохраняем старый parsing request:
        // 1 byte msg-id + 7 byte SDR header + fixed32 timestamp + fixed32 cookie.
        if (length < 1 + 7 + 4 + 4)
        {
            return;
        }

        var clientTimestamp =
            BinaryPrimitives.ReadUInt32LittleEndian(
                data.AsSpan(1 + 7, 4));

        var clientCookie =
            BinaryPrimitives.ReadUInt32LittleEndian(
                data.AsSpan(1 + 7 + 4, 4));

        var challenge = unchecked(++_challenge);

        var reply = new CMsgSteamDatagramRouterPingReply
        {
            ClientTimestamp = clientTimestamp,
            YourPublicIp = PublicIp(sender.Address),
            YourPublicPort = (uint)sender.Port,
            ServerTime = UnixNow(),
            Challenge = challenge,
            ClientCookie = clientCookie
        };

        byte[] protobufBody;

        using (var ms = new MemoryStream())
        {
            Serializer.Serialize(ms, reply);
            protobufBody = ms.ToArray();
        }

        var packet = new byte[1 + protobufBody.Length];

        packet[0] = MsgRouterPingReply;

        Buffer.BlockCopy(
            protobufBody,
            0,
            packet,
            1,
            protobufBody.Length);

        socket.SendTo(packet, sender);

        _logger.LogDebug(
            "SDR relay: replied RouterPing to {Sender} " +
            "(cookie {Cookie}, public={PublicIp}:{PublicPort}, protobuf={Size})",
            sender,
            clientCookie,
            sender.Address,
            sender.Port,
            protobufBody.Length);
    }

    private static uint PublicIp(IPAddress ip)
    {
        var bytes = ip.GetAddressBytes();

        if (bytes.Length != 4)
        {
            return 0;
        }

        Array.Reverse(bytes);

        return BitConverter.ToUInt32(bytes, 0);
    }

    private static uint UnixNow()
        => (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}