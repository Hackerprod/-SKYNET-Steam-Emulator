using System.Net;
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

    }
}