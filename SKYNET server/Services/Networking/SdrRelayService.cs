using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;

namespace SKYNET_server.Services.Networking;

/// <summary>
/// Live Steam Datagram (SDR) relay. The SDR network config we serve advertises a
/// relay endpoint (pops.SKY); once Dota fetches that config it PINGs the relay over
/// UDP (k_ESteamDatagramMsg_RouterPingRequest) and will only mark the relay/config
/// usable when it receives a matching k_ESteamDatagramMsg_RouterPingReply. Without a
/// live listener answering those pings the client stays at "config=Attempting".
///
/// This is a direct port of the reference coordinator's SDRProcessor + UDPServer:
/// a single UDP socket that echoes the ping with our datacenter id and a challenge.
/// </summary>
public sealed class SdrRelayService : BackgroundService
{
    // SDR message ids (ESteamDatagramMsgID). The datagram wire header is one msg-id
    // byte followed by 7 header bytes, then the message body.
    private const byte MsgRouterPingRequest = 1;
    private const byte MsgRouterPingReply = 2;

    private readonly ILogger<SdrRelayService> _logger;
    private readonly int _port;
    private readonly uint _dataCenterId;
    private ulong _challenge;

    public SdrRelayService(IConfiguration configuration, ILogger<SdrRelayService> logger)
    {
        _logger = logger;
        // Must match the port advertised in the served SDR config (pops.SKY relay).
        _port = int.TryParse(configuration["Sdr:RelayPort"], out var p) ? p : 28009;
        _dataCenterId = (uint)_port;
        _challenge = 1;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        try
        {
            socket.Bind(new IPEndPoint(IPAddress.Any, _port));
        }
        catch (SocketException ex)
        {
            _logger.LogError(ex, "SDR relay failed to bind UDP port {Port}", _port);
            return;
        }

        _logger.LogInformation("SDR relay listening on UDP 0.0.0.0:{Port} (datacenter id {Id})", _port, _dataCenterId);

        var buffer = new byte[4096];
        var remote = new IPEndPoint(IPAddress.Any, 0);

        while (!stoppingToken.IsCancellationRequested)
        {
            SocketReceiveFromResult result;
            try
            {
                result = await socket.ReceiveFromAsync(buffer, SocketFlags.None, remote, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (SocketException)
            {
                continue;
            }

            if (result.ReceivedBytes < 1 || result.RemoteEndPoint is not IPEndPoint sender)
            {
                continue;
            }

            try
            {
                HandlePacket(socket, buffer, result.ReceivedBytes, sender);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "SDR relay failed to process packet from {Sender}", sender);
            }
        }
    }

    private void HandlePacket(Socket socket, byte[] data, int length, IPEndPoint sender)
    {
        var msgId = data[0];
        if (msgId != MsgRouterPingRequest)
        {
            _logger.LogDebug("SDR relay: unhandled datagram msg {Msg} from {Sender}", msgId, sender);
            return;
        }

        // Layout after the msg-id byte: 7 header bytes, client_timestamp (u32 LE),
        // client_cookie (u32 LE). Matches the reference coordinator's reader.
        if (length < 1 + 7 + 4 + 4)
        {
            return;
        }

        var clientTimestamp = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(1 + 7, 4));
        var clientCookie = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(1 + 7 + 4, 4));

        var challenge = unchecked(++_challenge);
        var reply = BuildPingReply(clientTimestamp, clientCookie, challenge, sender.Address);

        var packet = new byte[reply.Length + 1];
        packet[0] = MsgRouterPingReply;
        Buffer.BlockCopy(reply, 0, packet, 1, reply.Length);

        socket.SendTo(packet, sender);
        _logger.LogDebug("SDR relay: replied RouterPing to {Sender} (cookie {Cookie})", sender, clientCookie);
    }

    private byte[] BuildPingReply(uint clientTimestamp, uint clientCookie, ulong challenge, IPAddress senderIp)
    {
        // CMsgSteamDatagramRouterPingReply, fields matching the reference coordinator.
        var w = new ProtoWriter();
        w.Fixed32(1, clientTimestamp);                       // client_timestamp
        w.PackedFixed32(2, _dataCenterId);                   // latency_datacenter_ids = [port]
        w.PackedVarint(3, 1);                                // latency_ping_ms = [1]
        w.Fixed32(4, PublicIp(senderIp));                    // your_public_ip
        w.Fixed32(5, UnixNow());                             // server_time
        w.Fixed64(6, challenge);                             // challenge
        w.Fixed32(8, clientCookie);                          // client_cookie
        return w.ToArray();
    }

    // Replicates NetHelpers.GetIPAddress: address bytes reversed into a host-order uint.
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

    private static uint UnixNow() => (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    /// <summary>Minimal protobuf writer for the ping reply's fixed layout.</summary>
    private sealed class ProtoWriter
    {
        private readonly List<byte> _buffer = new();

        public void Fixed32(int field, uint value)
        {
            WriteTag(field, 5);
            Span<byte> tmp = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(tmp, value);
            _buffer.AddRange(tmp.ToArray());
        }

        public void Fixed64(int field, ulong value)
        {
            WriteTag(field, 1);
            Span<byte> tmp = stackalloc byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(tmp, value);
            _buffer.AddRange(tmp.ToArray());
        }

        // Packed repeated fixed32 with a single element.
        public void PackedFixed32(int field, uint value)
        {
            WriteTag(field, 2);
            WriteRawVarint(4);
            Span<byte> tmp = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(tmp, value);
            _buffer.AddRange(tmp.ToArray());
        }

        // Packed repeated varint with a single element.
        public void PackedVarint(int field, ulong value)
        {
            WriteTag(field, 2);
            var element = new List<byte>();
            var v = value;
            while (v >= 0x80)
            {
                element.Add((byte)(v | 0x80));
                v >>= 7;
            }
            element.Add((byte)v);
            WriteRawVarint((ulong)element.Count);
            _buffer.AddRange(element);
        }

        public byte[] ToArray() => _buffer.ToArray();

        private void WriteTag(int field, int wireType) => WriteRawVarint(((ulong)field << 3) | (ulong)wireType);

        private void WriteRawVarint(ulong value)
        {
            while (value >= 0x80)
            {
                _buffer.Add((byte)(value | 0x80));
                value >>= 7;
            }
            _buffer.Add((byte)value);
        }
    }
}
