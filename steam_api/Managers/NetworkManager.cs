using SKYNET.Helper;
using SKYNET.Helper.JSON;
using SKYNET.Network;
using SKYNET.Network.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SKYNET.Managers
{
    public class NetworkManager
    {
        public static void Initialize()
        {
            TCPServer tcpServer = new TCPServer();
            tcpServer.OnDataReceived += TcpServer_OnDataReceived;
            tcpServer.OnConnected += TcpServer_OnConnected;
            ThreadPool.QueueUserWorkItem(tcpServer.Start);

            ThreadPool.QueueUserWorkItem(BroadcastAnnounce);
        }

        private static void TcpServer_OnConnected(object sender, Socket e)
        {
            Write("Client connected from " + e.RemoteEndPoint);
        }

        private static void TcpServer_OnDataReceived(object sender, Network.NetPacket packet)
        {
            NetworkMessage message = packet.Data.GetString().FromJson<NetworkMessage>();
            ProcessMessage(message, packet.Sender);
        }

        #region Message processors

        private static void ProcessMessage(NetworkMessage message, Socket socket)
        {
            switch ((MessageType)message.MessageType)
            {
                case MessageType.NET_Announce:
                    ProcessAnnounce(message, socket);
                    break;
                case MessageType.NET_AnnounceResponse:
                    ProcessAnnounce(message, socket);
                    break;
                case MessageType.NET_AvatarRequest:
                    break;
                case MessageType.NET_AvatarResponse:
                    break;
                default:
                    break;
            }

        }

        private static void ProcessAnnounce(NetworkMessage message, Socket socket)
        {
            NET_Announce announce = message.ParsedBody.FromJson<NET_Announce>();

            if (announce != null)
                SteamEmulator.SteamFriends.AddOrUpdateUser(announce.AccountID, announce.PersonaName, announce.AppID, ((IPEndPoint)socket.RemoteEndPoint).Address.ToString());

            if (message.MessageType == (int)MessageType.NET_Announce)
            {
                NET_Announce announceResponse = new NET_Announce()
                {
                    PersonaName = SteamEmulator.PersonaName,
                    AccountID = (uint)SteamEmulator.SteamId
                };
                NetworkMessage messageResponse = new NetworkMessage()
                {
                    MessageType = (int)MessageType.NET_AnnounceResponse,
                    ParsedBody = announceResponse.ToJson()
                };
                string json = messageResponse.ToJson();
                socket.Send(json.GetBytes());
            }
            else
            {
                Write("Closing connection after received NET_AnnounceResponse message");
                socket.Close();
                socket.Dispose();
            }
        }

        #endregion

        private static void BroadcastAnnounce(object state)
        {
            foreach (var item in GetIPAddresses())
            {
                var Addressess = GetIPAddressRange(item);
                foreach (var Address in Addressess)
                {
                    try
                    {
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.BeginConnect(new IPEndPoint(IPAddress.Parse(Address), 28880), AnnounceCallback, socket);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static void AnnounceCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = ((Socket)ar.AsyncState);
                socket.EndConnect(ar);

                NET_Announce announce = new NET_Announce()
                {
                    PersonaName = SteamEmulator.PersonaName,
                    AccountID = (uint)SteamEmulator.SteamId
                };

                NetworkMessage message = new NetworkMessage()
                {
                    MessageType = (int)MessageType.NET_AnnounceResponse,
                    ParsedBody = announce.ToJson()
                };
                string json = message.ToJson();
                byte[] bytes = Encoding.Default.GetBytes(json);
                socket.Send(bytes);

                ClientSockets client = new ClientSockets(socket);
                client.OnDataReceived += Client_OnDataReceived;
                client.BeginReceiving();
            }
            catch 
            {
            }
        }

        private static void Client_OnDataReceived(object sender, Network.NetPacket e)
        {
            try
            {
                Socket socket = e.Sender;
                NetworkMessage message = e.Data.GetString().FromJson<NetworkMessage>();
                ProcessMessage(message, socket);
            }
            catch
            {
            }
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("NetworkManager", msg);
        }

        public static List<IPAddress> GetIPAddresses()
        {
            var Addresses = new List<IPAddress>();
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            IPAddress iPAddress = null;
            IPAddress[] addressList = hostEntry.AddressList;
            foreach (IPAddress iPAddress2 in addressList)
            {
                if (iPAddress2.AddressFamily == AddressFamily.InterNetwork)
                {
                    Addresses.Add(iPAddress2);
                }
            }
            return Addresses;
        }

        public static IPAddress GetIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            IPAddress iPAddress = null;
            IPAddress[] addressList = hostEntry.AddressList;
            foreach (IPAddress iPAddress2 in addressList)
            {
                if (iPAddress2.AddressFamily == AddressFamily.InterNetwork)
                {
                    iPAddress = iPAddress2;
                }
            }
            return iPAddress;
        }

        private static List<string> GetIPAddressRange(IPAddress address)
        {
            List<string> rangeAddr = new List<string>();
            string[] ipParts = address.ToString().Split('.');
            for (int i = 1; i < 255; i++)
            {
                rangeAddr.Add($"{ipParts[0]}.{ipParts[1]}.{ipParts[2]}.{i}");
            }
            return rangeAddr;
        }
    }
}
