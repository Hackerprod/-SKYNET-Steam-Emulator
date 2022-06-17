using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace SKYNET.Helper
{
    public class NetworkHelper
    {
        public static IPAddress GetIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            IPAddress iPAddress = null;
            IPAddress[] addressList = hostEntry.AddressList;
            foreach (IPAddress address in addressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    iPAddress = address;
                }
            }
            return iPAddress;
        }

        public static uint ConvertFromIPAddress(IPAddress iPAddress)
        {
            byte[] addressBytes = iPAddress.GetAddressBytes();
            uint num = (uint)(addressBytes[0] << 24);
            num += (uint)(addressBytes[1] << 16);
            num += (uint)(addressBytes[2] << 8);
            num += addressBytes[3];
            return num;
        }

        public static IPAddress ConvertToIPAddress(uint ipAddr)
        {
            return new IPAddress(ipAddr.Swap());
        }

        public static bool IsAvailablePort(int port)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();
            IPEndPoint[] udpConnInfoArray = ipGlobalProperties.GetActiveUdpListeners();

            foreach (IPEndPoint endpoint in tcpConnInfoArray)
            {
                if (endpoint.Port == port)
                {
                    return false;
                }
            }
            foreach (IPEndPoint endpoint in udpConnInfoArray)
            {
                if (endpoint.Port == port)
                {
                    return false;
                }
            }

            return true;
        }

        private static List<string> GetIPAddressRange(IPAddress address)
        {
            List<string> rangeAddr = new List<string>();
            if (IPAddress.IsLoopback(address))
            {
                rangeAddr.Add(address.ToString());
                return rangeAddr;
            }
            string[] ipParts = address.ToString().Split('.');
            for (int i = 1; i < 255; i++)
            {
                rangeAddr.Add($"{ipParts[0]}.{ipParts[1]}.{ipParts[2]}.{i}");
            }
            return rangeAddr;
        }

        private static bool IslocalAddress(IPAddress iPAddress)
        {
            return IslocalAddress(iPAddress.ToString());
        }

        private static bool IslocalAddress(string iPAddress)
        {
            try
            {
                IPAddress[] hostIPs = Dns.GetHostAddresses(iPAddress);
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                foreach (IPAddress hostIP in hostIPs)
                {
                    if (IPAddress.IsLoopback(hostIP)) return true;
                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP)) return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public static string IntToAddr(long address)
        {
            return IPAddress.Parse(address.ToString()).ToString();
        }

    }
}