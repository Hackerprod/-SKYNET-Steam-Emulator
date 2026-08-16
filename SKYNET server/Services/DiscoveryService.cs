using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace SKYNET_server.Services;

public sealed class DiscoveryService : BackgroundService
{
    private const string DiscoveryRequest = "SKYNET_DISCOVER";
    private const string DiscoveryPrefix = "SKYNET_SERVER ";
    private const int SioUdpConnectionReset = unchecked((int)0x9800000C);

    private readonly ILogger<DiscoveryService> _logger;
    private readonly bool _enabled;
    private readonly int _discoveryPort;
    private readonly int _httpPort;

    public DiscoveryService(ILogger<DiscoveryService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _enabled = configuration.GetValue("Discovery:Enabled", true);
        _discoveryPort = Math.Clamp(configuration.GetValue("Discovery:Port", 27081), 1, ushort.MaxValue);
        _httpPort = Math.Clamp(configuration.GetValue("Server:HttpPort", 27080), 1, ushort.MaxValue);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_enabled)
        {
            _logger.LogInformation("SKYNET discovery is disabled");
            return;
        }

        using var udp = new UdpClient(new IPEndPoint(IPAddress.Any, _discoveryPort))
        {
            EnableBroadcast = true
        };
        DisableWindowsUdpConnectionReset(udp.Client);

        _logger.LogInformation("SKYNET discovery listening on UDP {Port}", _discoveryPort);

        while (!stoppingToken.IsCancellationRequested)
        {
            UdpReceiveResult result;
            try
            {
                result = await udp.ReceiveAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionReset)
            {
                // A client may close its UDP port before receiving our reply.
                // That ICMP notification does not invalidate the listener.
                continue;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SKYNET discovery receive failed");
                continue;
            }

            var message = Encoding.UTF8.GetString(result.Buffer).Trim();
            if (!string.Equals(message, DiscoveryRequest, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var serverUrl = $"{DiscoveryPrefix}http://{SelectReplyAddress(result.RemoteEndPoint.Address)}:{_httpPort}/";
            var payload = Encoding.UTF8.GetBytes(serverUrl);
            try
            {
                await udp.SendAsync(payload, result.RemoteEndPoint, stoppingToken);
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionReset)
            {
                // UDP has no session to recover; the next discovery request is
                // independent and remains valid.
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SKYNET discovery response failed");
            }
        }
    }

    private void DisableWindowsUdpConnectionReset(Socket socket)
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        try
        {
            // SIO_UDP_CONNRESET=FALSE prevents an ICMP Port Unreachable response
            // to one datagram from surfacing as an error on the shared listener.
            socket.IOControl(SioUdpConnectionReset, new byte[sizeof(int)], null);
        }
        catch (SocketException ex)
        {
            // The targeted receive guard above preserves correct behavior when a
            // Winsock provider does not support this optional control code.
            _logger.LogDebug(ex, "Could not disable UDP connection-reset reporting");
        }
        catch (PlatformNotSupportedException ex)
        {
            _logger.LogDebug(ex, "UDP connection-reset control is not supported");
        }
    }

    private static string SelectReplyAddress(IPAddress remoteAddress)
    {
        if (IPAddress.IsLoopback(remoteAddress))
        {
            return IPAddress.Loopback.ToString();
        }

        foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (networkInterface.OperationalStatus != OperationalStatus.Up ||
                networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
            {
                continue;
            }

            foreach (var address in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (address.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return address.Address.ToString();
                }
            }
        }

        return IPAddress.Loopback.ToString();
    }
}
