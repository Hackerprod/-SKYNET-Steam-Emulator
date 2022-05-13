using SKYNET.Helper;
using SKYNET.Helper.JSON;
using SKYNET.Network;
using SKYNET.Network.Packets;
using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SKYNET.Managers
{
    public class NetworkManager
    {
        public static int Port = 28880;

        public static void Initialize()
        {
            Port = SteamEmulator.BroadCastPort;
            Write($"Initializing TCP server on port {Port}");

            TCPServer tcpServer = new TCPServer(Port);
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
                case MessageType.NET_UserDataUpdated:
                    ProcessUserStatusChanged(message, socket);
                    break;
                case MessageType.NET_P2PPacket:
                    ProcessP2PPacket(message, socket);
                    break;
                case MessageType.NET_LobbyListRequest:
                    ProcessLobbyListRequest(message, socket);
                    break;
                case MessageType.NET_LobbyListResponse:
                    ProcessLobbyListResponse(message, socket);
                    break;
                default:
                    break;
            }

        }

        private static void ProcessLobbyListRequest(NetworkMessage message, Socket socket)
        {
            try
            {
                NET_LobbyListRequest lobbyListRequest = message.ParsedBody.FromJson<NET_LobbyListRequest>();
                //if (lobbyListRequest.RequestID == SteamEmulator.SteamMatchmaking.CurrentRequest)
                //{
                //    socket.Close();
                //    socket.Dispose();
                //    return;
                //}
                var lobby = SteamEmulator.SteamMatchmaking.Lobbies.Where(l => l.Value.Owner == (ulong)SteamEmulator.SteamId).Select(l => l.Value).FirstOrDefault();
                if (lobby == null)
                {
                    socket.Close();
                    socket.Dispose();
                }
                else
                {
                    string serialized = lobby.ToJson();

                    NET_LobbyListResponse lobbyListResponse = new NET_LobbyListResponse()
                    {
                        SerializedLobby = serialized
                    };

                    NetworkMessage messageResponse = new NetworkMessage()
                    {
                        MessageType = (int)MessageType.NET_LobbyListResponse,
                        ParsedBody = lobbyListResponse.ToJson()
                    };

                    string json = messageResponse.ToJson();
                    socket.Send(json.GetBytes());
                }
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        private static void ProcessLobbyListResponse(NetworkMessage message, Socket socket)
        {
            try
            {
                NET_LobbyListResponse lobbyListResponse = message.ParsedBody.FromJson<NET_LobbyListResponse>();
                var lobby = lobbyListResponse.SerializedLobby.FromJson<Steamworks.Implementation.SteamMatchmaking.SteamLobby>();
                if (lobby != null)
                {
                    SteamEmulator.SteamMatchmaking.Lobbies.TryAdd(lobby.SteamID, lobby);
                }
                Write($"Closing connection after received {(MessageType)message.MessageType} message");
                socket.Close();
                socket.Dispose();
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        private static void ProcessAnnounce(NetworkMessage message, Socket socket)
        {
            try
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
            catch 
            {

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
                        AccountID = (uint)SteamEmulator.SteamId.AccountId,
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

        private static void ProcessUserStatusChanged(NetworkMessage message, Socket socket)
        {
            try
            {
                NET_UserDataUpdated StatusChanged = message.ParsedBody.FromJson<NET_UserDataUpdated>();

                if (StatusChanged != null)
                {
                    if (StatusChanged.AccountID == (uint)SteamEmulator.SteamId.AccountId) return;
                    string ipaddress = "";
                    try { ipaddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(); } catch { }
                    SteamEmulator.SteamFriends.UpdateUserStatus(StatusChanged, ipaddress);
                }

                Write($"Closing connection after received {(MessageType)message.MessageType} message");
                socket.Close();
                socket.Dispose();
            }
            catch 
            {

            }
        }

        private static void ProcessP2PPacket(NetworkMessage message, Socket socket)
        {
            try
            {
                NET_P2PPacket p2p = message.ParsedBody.FromJson<NET_P2PPacket>();

                if (p2p != null && p2p.AccountID == (uint)SteamEmulator.SteamId.AccountId)
                {
                    byte[] bytes = Convert.FromBase64String(p2p.Buffer);
                    SteamEmulator.SteamNetworking.P2PIncoming.Add(p2p);
                }

                Write($"Closing connection after received {(MessageType)message.MessageType} message");
                socket.Close();
                socket.Dispose();
            }
            catch 
            {

            }
        }

        #endregion

        public static void BroadcastStatusUpdated(SteamUser user)
        {
            NET_UserDataUpdated status = new NET_UserDataUpdated()
            {
                PersonaName = user.PersonaName,
                AccountID = (uint)SteamEmulator.SteamId.AccountId,
                LobbyID = user.LobbyId.GetAccountID()
            };

            NetworkMessage message = new NetworkMessage()
            {
                MessageType = (int)MessageType.NET_UserDataUpdated,
                ParsedBody = status.ToJson()
            };

            ThreadPool.QueueUserWorkItem(SendBroadcast, message);
        }

        public static void SendP2PTo(ulong steamIDRemote, byte[] bytes, int eP2PSendType, int nChannel)
        {
            try
            {
                NET_P2PPacket p2p = new NET_P2PPacket()
                {
                    AccountID = steamIDRemote.GetAccountID(),
                    Buffer = Convert.ToBase64String(bytes),
                    IDRemote = (uint)SteamEmulator.SteamId.AccountId,
                    P2PSendType = eP2PSendType,
                    Channel = nChannel
                };

                NetworkMessage message = new NetworkMessage()
                {
                    MessageType = (int)MessageType.NET_P2PPacket,
                    ParsedBody = p2p.ToJson()
                };

                var user = SteamEmulator.SteamFriends.GetUser(steamIDRemote);
                if (user == null)
                    return;

                SendTo(user.IPAddress, message);
            }
            catch (Exception ex)
            {
                Write(ex.Message + " " + ex.StackTrace);
            }
        }

        public static void RequestLobbyList(uint currentRequest)
        {
            NET_LobbyListRequest lobbyListRequest = new NET_LobbyListRequest()
            {
                RequestID = currentRequest
            };

            NetworkMessage message = new NetworkMessage()
            {
                MessageType = (int)MessageType.NET_LobbyListRequest,
                ParsedBody = lobbyListRequest.ToJson()
            };

            SendBroadcast(message);
        }

        private static void BroadcastAnnounce(object state)
        {
            NET_Announce announce = new NET_Announce()
            {
                PersonaName = SteamEmulator.PersonaName,
                AccountID = (uint)SteamEmulator.SteamId.AccountId
            };

            NetworkMessage message = CreateNetworkMessage(announce, MessageType.NET_Announce);

            SendBroadcast(message);
        }

        public static void RequestAvatar(string IP)
        {
            NetworkMessage message = new NetworkMessage()
            {
                MessageType = (int)MessageType.NET_AvatarRequest,
                ParsedBody = ""
            };

            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                SocketData SocketData = new SocketData() { socket = socket, Message = message };
                socket.BeginConnect(IP, Port, ConnectionCallback, SocketData);
            }
            catch
            {
            }
        }

        private static void SendBroadcast(object state)
        {
            NetworkMessage message = (NetworkMessage)state;

            foreach (var item in GetIPAddresses())
            {
                var Addressess = GetIPAddressRange(item);
                foreach (var Address in Addressess)
                {
                    try
                    {
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        SocketData SocketData = new SocketData() { socket = socket, Message = message };
                        socket.BeginConnect(Address, Port, ConnectionCallback, SocketData);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static void SendTo(string iPAddress, NetworkMessage message)
        {
            if (IPAddress.TryParse(iPAddress, out _))
            {
                try
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    SocketData SocketData = new SocketData() { socket = socket, Message = message };
                    socket.BeginConnect(iPAddress, Port, ConnectionCallback, SocketData);
                }
                catch
                {
                }
            }
        }

        #region Conection callback

        private static void ConnectionCallback(IAsyncResult ar)
        {
            try
            {
                SocketData SocketData = ((SocketData)ar.AsyncState);
                SocketData.socket.EndConnect(ar);

                string json = SocketData.Message.ToJson();
                byte[] bytes = Encoding.Default.GetBytes(json);
                SocketData.socket.Send(bytes);

                ClientSocket client = new ClientSocket(SocketData.socket);
                client.OnDataReceived += Network_OnDataReceived;
                client.BeginReceiving();
            }
            catch 
            {
            }
        }

        #endregion

        private static NetworkMessage CreateNetworkMessage(NET_Base Base, MessageType type)
        {
            NetworkMessage message = new NetworkMessage()
            {
                MessageType = (int)type,
                ParsedBody = Base.ToJson()
            };
            return message;
        }

        private static void Write(object msg)
        {
            //SteamEmulator.Write("NetworkManager", msg);
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

        public static uint GetIPAddress(IPAddress ipAddr)
        {
            byte[] addressBytes = ipAddr.GetAddressBytes();
            Array.Reverse(addressBytes);
            return BitConverter.ToUInt32(addressBytes, 0);
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

        private class SocketData
        {
            public Socket socket { get; set; }
            public NetworkMessage Message { get; set; }
        }
    }
}
