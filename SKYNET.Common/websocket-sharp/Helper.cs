using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketSharp.Server
{
    public class Helper
    {
        public static IEnumerable<IPAddress> GetIPAddresses()
        {
            List<IPAddress> list = new List<IPAddress>();
            IEnumerable<NetworkInterface> enumerable = from nic in NetworkInterface.GetAllNetworkInterfaces()
                                                       where nic.OperationalStatus == OperationalStatus.Up
                                                       select nic;
            foreach (NetworkInterface item in enumerable)
            {
                IPInterfaceProperties iPProperties = item.GetIPProperties();
                foreach (UnicastIPAddressInformation unicastAddress in iPProperties.UnicastAddresses)
                {
                    if (!list.Contains(unicastAddress.Address))
                    {
                        list.Add(unicastAddress.Address);
                    }
                }
            }
            return (from ip in list
                    orderby RankIpAddress(ip) descending
                    select ip).ToList();
        }

        public static int RankIpAddress(IPAddress addr)
        {
            int num = 1000;
            if (IPAddress.IsLoopback(addr))
            {
                num = 300;
            }
            else if (addr.AddressFamily == AddressFamily.InterNetwork)
            {
                num += 100;
                if (addr.GetAddressBytes().Take(2).SequenceEqual(new byte[2]
                {
                    169,
                    254
                }))
                {
                    num = 0;
                }
            }
            if (num > 500)
            {
                foreach (NetworkInterface item in TryGetCurrentNetworkInterfaces())
                {
                    IPInterfaceProperties iPProperties = item.GetIPProperties();
                    if (iPProperties.GatewayAddresses.Any())
                    {
                        if (iPProperties.UnicastAddresses.Any((UnicastIPAddressInformation u) => u.Address.Equals(addr)))
                        {
                            num += 1000;
                        }
                        break;
                    }
                }
            }
            return num;
        }
        public static IEnumerable<NetworkInterface> TryGetCurrentNetworkInterfaces()
        {
            try
            {
                return from ni in NetworkInterface.GetAllNetworkInterfaces()
                       where ni.OperationalStatus == OperationalStatus.Up
                       select ni;
            }
            catch (NetworkInformationException)
            {
                return Enumerable.Empty<NetworkInterface>();
            }
        }

    }
}
