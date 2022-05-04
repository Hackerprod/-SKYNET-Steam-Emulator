using SKYNET.Helper;
using SKYNET.Helper.JSON;
using SKYNET.Network;
using SKYNET.Network.Packets;
using SKYNET.Steamworks.Implementation;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            tcpServer.OnDataReceived += Network_OnDataReceived;
            tcpServer.OnConnected += TcpServer_OnConnected;
            ThreadPool.QueueUserWorkItem(tcpServer.Start);

            ThreadPool.QueueUserWorkItem(BroadcastAnnounce);
        }

        private static void TcpServer_OnConnected(object sender, Socket e)
        {
            Write("Client connected from " + e.RemoteEndPoint);
        }

        private static void Network_OnDataReceived(object sender, Network.NetPacket packet)
        {
            NetworkMessage message = packet.Data.GetString().FromJson<NetworkMessage>();
            ProcessMessage(message, packet.Sender);
        }

        #region Message processors

        private static void ProcessMessage(NetworkMessage message, Socket socket)
        {
            Write($"Received message {(MessageType)message.MessageType} from {((IPEndPoint)socket.RemoteEndPoint).Address.ToString()}");
            switch ((MessageType)message.MessageType)
            {
                case MessageType.NET_Announce:
                case MessageType.NET_AnnounceResponse:
                    ProcessAnnounce(message, socket);
                    break;
                case MessageType.NET_AvatarRequest:
                case MessageType.NET_AvatarResponse:
                    ProcessAvatar(message, socket);
                    break;
                default:
                    break;
            }

        }

        private static void ProcessAnnounce(NetworkMessage message, Socket socket)
        {
            NET_Announce announce = message.ParsedBody.FromJson<NET_Announce>();

            // Add User to List on both cases (NET_Announce and NET_AnnounceResponse)
            if (announce != null && announce.AccountID != SteamEmulator.SteamId.AccountId)
                SteamEmulator.SteamFriends?.AddOrUpdateUser(announce.AccountID, announce.PersonaName, announce.AppID, ((IPEndPoint)socket.RemoteEndPoint).Address.ToString());

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

                // Connection pair close the socket
            }
            else
            {
                Write($"Closing connection after received {(MessageType)message.MessageType} message");
                socket.Close();
                socket.Dispose();
            }
        }

        private static void ProcessAvatar(NetworkMessage message, Socket socket)
        {
            if (message.MessageType == (int)MessageType.NET_AvatarRequest)
            {
                try
                {
                    var imageBytes = SteamEmulator.SteamFriends.GetAvatar((ulong)SteamEmulator.SteamId);
                    string hexAvatar = Convert.ToBase64String(imageBytes);
                    NET_AvatarResponse avatarResponse = new NET_AvatarResponse()
                    {
                        AccountID = SteamEmulator.SteamId.AccountId,
                        HexAvatar = hexAvatar
                    };
                    string parsedResponse = avatarResponse.ToJson();
                    NetworkMessage messageResponse = new NetworkMessage()
                    {
                        MessageType = (int)MessageType.NET_AvatarResponse,
                        ParsedBody = parsedResponse
                    };
                    string json = messageResponse.ToJson();
                    socket.Send(json.GetBytes());
                }
                catch (Exception ex)
                {
                    Write($"{ex}");
                }
            }
            else
            {
                try
                {
                    NET_AvatarResponse announceResponse = message.ParsedBody.FromJson<NET_AvatarResponse>();
                    if (announceResponse != null)
                    {
                        var imageBytes = Convert.FromBase64String(announceResponse.HexAvatar);
                        if (imageBytes.Length != 0)
                        {
                            Bitmap Avatar = (Bitmap)ImageHelper.ImageFromBytes(imageBytes);
                            ulong SteamID = (ulong)new CSteamID(announceResponse.AccountID);
                            SteamEmulator.SteamFriends.AddOrUpdateAvatar(Avatar, SteamID);
                            SteamEmulator.SteamRemoteStorage.StoreAvatar(Avatar, announceResponse.AccountID);
                        }
                    }
                    Write($"Closing connection after received {(MessageType)message.MessageType} message");
                    socket.Close();
                    socket.Dispose();
                }
                catch (Exception ex)
                {
                    Write($"{ex}");
                }
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
                        socket.BeginConnect(Address, 28880, AnnounceCallback, socket);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static void RequestAvatar(string IP)
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.BeginConnect(IP, 28880, AvatarCallback, socket);
            }
            catch
            {
            }
        }

        #region Conection callback

        private static void AvatarCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = ((Socket)ar.AsyncState);
                socket.EndConnect(ar);

                NetworkMessage message = new NetworkMessage()
                {
                    MessageType = (int)MessageType.NET_AvatarRequest,
                    ParsedBody = ""
                };
                string json = message.ToJson();
                byte[] bytes = Encoding.Default.GetBytes(json);
                socket.Send(bytes);

                ClientSocket client = new ClientSocket(socket);
                client.OnDataReceived += Network_OnDataReceived;
                client.BeginReceiving();
            }
            catch
            {
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
                    AccountID = SteamEmulator.SteamId.AccountId
                };

                NetworkMessage message = new NetworkMessage()
                {
                    MessageType = (int)MessageType.NET_Announce,
                    ParsedBody = announce.ToJson()
                };

                string json = message.ToJson();
                byte[] bytes = Encoding.Default.GetBytes(json);
                socket.Send(bytes);

                ClientSocket client = new ClientSocket(socket);
                client.OnDataReceived += Network_OnDataReceived;
                client.BeginReceiving();
            }
            catch 
            {
            }
        }

        #endregion

        private static void Write(object msg)
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
