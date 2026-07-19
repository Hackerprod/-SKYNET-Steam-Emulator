using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace SKYNET.Client.Services;

/// <summary>
/// Discovers the SKYNET server on the LAN / ZeroTier network by UDP broadcast,
/// exactly like the emulator DLL: send "SKYNET_DISCOVER" to the discovery port and
/// wait for a "SKYNET_SERVER &lt;url&gt;" reply. Broadcasts on every up interface
/// (including virtual ones like ZeroTier), since 255.255.255.255 alone often does
/// not cross a ZeroTier adapter.
/// </summary>
public static class ServerDiscovery
{
    private const string Request = "SKYNET_DISCOVER";
    private const string ReplyPrefix = "SKYNET_SERVER ";

    public static string? Discover(int discoveryPort, int timeoutMs = 1200)
    {
        try
        {
            using var udp = new UdpClient();
            udp.EnableBroadcast = true;
            udp.Client.ReceiveTimeout = timeoutMs;

            var payload = Encoding.UTF8.GetBytes(Request);

            // Global broadcast.
            TrySend(udp, payload, IPAddress.Broadcast, discoveryPort);
            // Per-interface directed broadcasts (covers ZeroTier / multi-homed hosts).
            foreach (var bcast in InterfaceBroadcasts())
                TrySend(udp, payload, bcast, discoveryPort);

            // Read replies until the timeout elapses; accept the first valid one.
            var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            while (DateTime.UtcNow < deadline)
            {
                try
                {
                    var remote = new IPEndPoint(IPAddress.Any, 0);
                    var response = udp.Receive(ref remote);
                    var text = Encoding.UTF8.GetString(response).Trim();
                    if (text.StartsWith(ReplyPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        var url = text.Substring(ReplyPrefix.Length).Trim();
                        if (!string.IsNullOrWhiteSpace(url)) return Normalize(url);
                    }
                }
                catch (SocketException)
                {
                    break; // receive timed out
                }
            }
        }
        catch { /* discovery is best-effort */ }
        return null;
    }

    private static void TrySend(UdpClient udp, byte[] payload, IPAddress address, int port)
    {
        try { udp.Send(payload, payload.Length, new IPEndPoint(address, port)); }
        catch { }
    }

    private static IEnumerable<IPAddress> InterfaceBroadcasts()
    {
        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.OperationalStatus != OperationalStatus.Up) continue;
            foreach (var ua in nic.GetIPProperties().UnicastAddresses)
            {
                if (ua.Address.AddressFamily != AddressFamily.InterNetwork) continue;
                var bcast = DirectedBroadcast(ua.Address, ua.IPv4Mask);
                if (bcast != null) yield return bcast;
            }
        }
    }

    private static IPAddress? DirectedBroadcast(IPAddress addr, IPAddress? mask)
    {
        if (mask == null) return null;
        var a = addr.GetAddressBytes();
        var m = mask.GetAddressBytes();
        if (a.Length != 4 || m.Length != 4) return null;
        var b = new byte[4];
        for (int i = 0; i < 4; i++) b[i] = (byte)(a[i] | ~m[i]);
        return new IPAddress(b);
    }

    private static string Normalize(string url)
    {
        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            url = "http://" + url;
        return url.EndsWith("/") ? url : url + "/";
    }
}
