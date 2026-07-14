using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace SKYNET_server.Services;

public sealed class DiscoveryService : BackgroundService
{
    private const int DiscoveryPort = 27081;
    private const string DiscoveryRequest = "SKYNET_DISCOVER";
    private const string DiscoveryPrefix = "SKYNET_SERVER ";

    private readonly ILogger<DiscoveryService> _logger;

    public DiscoveryService(ILogger<DiscoveryService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var udp = new UdpClient(new IPEndPoint(IPAddress.Any, DiscoveryPort))
        {
            EnableBroadcast = true
        };

        _logger.LogInformation("SKYNET discovery listening on UDP {Port}", DiscoveryPort);

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

            var serverUrl = $"{DiscoveryPrefix}http://{SelectReplyAddress(result.RemoteEndPoint.Address)}:27080/";
            var payload = Encoding.UTF8.GetBytes(serverUrl);
            try
            {
                await udp.SendAsync(payload, result.RemoteEndPoint, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "SKYNET discovery response failed");
            }
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
