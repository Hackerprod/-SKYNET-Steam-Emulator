using SKYNET.Client;
using SKYNET.Common;
using SKYNET.Helper;
using SKYNET.IPC.Types;
using SKYNET.Network;
using SKYNET.Network.Packets;
using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp.Server;

namespace SKYNET.Managers
{
    public class NetworkManager
    {
        public static int Port = 28880;
        public static TCPServer TCPServer;
        public static WebServer WebServer;
        public static WebSocketServer WebSocket;
        public static WebSocketProcessor WebClient;

        public static void Initialize()
        {
            Port = 28080;

            if (NetworkHelper.IsAvailablePort(Port))
            {
                //Write($"Initializing TCP server on port {Port}");

                TCPServer = new TCPServer(Port);
                TCPServer.OnDataReceived += TCPServer_OnDataReceived;
                TCPServer.OnConnected += TCPServer_OnConnected;
                TCPServer.Start();
            }
            else
            {
                Write($"Error initializing TCP server, port {Port} is in use");
            }

            WebSocket = new WebSocketServer(8888);
            WebSocket.AddWebSocketService<WebSocketProcessor>("/OnMessage");
            WebSocket.Start();

            WebServer = new WebServer(80);
            WebServer.Start();

            BroadcastAnnounce();
        }

        private static void TCPServer_OnConnected(object sender, TCPClient e)
        {
            //Write("Client connected from " + e.RemoteEndPoint);
        }

        private static void TCPServer_OnDataReceived(object sender, NetPacket packet)
        {
            NetworkMessage message = packet.Data.GetString().Deserialize<NetworkMessage>();
            ProcessMessage(message, packet.Sender.Socket);
        }

        private static void ProcessMessage(NetworkMessage message, Socket socket)
        {
            Write($"Received message {(MessageType)message.MessageType} from {((IPEndPoint)socket.RemoteEndPoint).Address.ToString()}");
            try
            {
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
                    case MessageType.NET_LobbyListRequest:
                        ProcessLobbyListRequest(message, socket);
                        break;
                    case MessageType.NET_LobbyListResponse:
                        ProcessLobbyListResponse(message, socket);
                        break;
                    case MessageType.NET_LobbyJoinRequest:
                        ProcessLobbyJoinRequest(message, socket);
                        break;
                    case MessageType.NET_LobbyJoinResponse:
                        ProcessLobbyJoinResponse(message, socket);
                        break;
                    case MessageType.NET_LobbyDataUpdate:
                        ProcessLobbyDataUpdate(message, socket);
                        break;
                    case MessageType.NET_LobbyChatUpdate:
                        ProcessLobbyChatUpdate(message, socket);
                        break;
                    case MessageType.NET_LobbyLeave:
                        ProcessLobbyLeave(message, socket);
                        break;
                    case MessageType.NET_LobbyRemove:
                        ProcessLobbyRemove(message, socket);
                        break;
                    case MessageType.NET_LobbyGameserver:
                        ProcessLobbyGameserver(message, socket);
                        break;
                    case MessageType.NET_GameOpened:
                        ProcessGameOpened(message, socket);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Write($"Error processing message {(MessageType)message.MessageType}" + Environment.NewLine + ex.Message + ex.StackTrace);
            }
        }

        #region Received Messages
        private static void ProcessLobbyLeave(NetworkMessage message, Socket socket)
        {
            // Steam lobby owner
            var lobbyLeave = message.ParsedBody.Deserialize<NET_LobbyLeave>();
            if (lobbyLeave != null)
            {
                if (LobbyManager.GetLobby(lobbyLeave.LobbyID, out var lobby))
                {
                    lobby.Members.RemoveAll(m => m.m_SteamID == lobbyLeave.SteamID);

                    IPCManager.SendLobbyLeave(lobbyLeave);
                }
            }

            CloseSocket(socket, message.MessageType);
        }

        private static void ProcessLobbyRemove(NetworkMessage message, Socket socket)
        {
            var lobbyRemove = message.ParsedBody.Deserialize<NET_LobbyRemove>();
            if (lobbyRemove != null)
            {
                IPCManager.SendLobbyRemove(lobbyRemove.LobbyID);
                LobbyManager.Remove(lobbyRemove.LobbyID);
            }
            CloseSocket(socket, message.MessageType);
        }

        private static void ProcessLobbyDataUpdate(NetworkMessage message, Socket socket)
        {
            var LobbyDataUpdate = message.ParsedBody.Deserialize<NET_LobbyDataUpdate>();
            if (LobbyDataUpdate != null)
            {
                IPCManager.SendLobbyDataUpdate(LobbyDataUpdate);
            }
        }

        private static void ProcessLobbyJoinRequest(NetworkMessage message, Socket socket)
        {
            try
            {
                var LobbyJoinRequest = message.ParsedBody.Deserialize<NET_LobbyJoinRequest>();
                if (LobbyJoinRequest != null)
                {
                    IPCManager.SendLobbyJoinRequest(LobbyJoinRequest);
                }
            }
            catch (Exception ex)
            {
                Write(ex);
            }

            CloseSocket(socket, message.MessageType);
        }

        private static void ProcessLobbyJoinResponse(NetworkMessage message, Socket socket)
        {
            try
            {
                var lobbyJoinResponse = message.ParsedBody.Deserialize<NET_LobbyJoinResponse>();
                if (lobbyJoinResponse != null)
                {
                    IPCManager.SendLobbyJoinResponse(lobbyJoinResponse);
                }
            }
            catch (Exception ex)
            {
                Write(ex);
            }

            CloseSocket(socket, message.MessageType);
        }

        private static void ProcessLobbyListRequest(NetworkMessage message, Socket socket)
        {
            try
            {
                var lobbyListRequest = message.ParsedBody.Deserialize<NET_LobbyListRequest>();
                if (lobbyListRequest != null)
                {
                    // TODO: Request lobby list to server
                    //if (lobbyListRequest.RequestID == SteamMatchmaking.Instance.CurrentRequest)
                    //{
                    //    CloseSocket(socket, message.MessageType);
                    //    return;
                    //}
                }
                var lobby = LobbyManager.GetLobbyByOwner((ulong)SteamClient.SteamID);
                if (lobby == null)
                {
                    CloseSocket(socket, message.MessageType);
                }
                else
                {
                    if (lobby.AppID != lobbyListRequest.AppID)
                    {
                        CloseSocket(socket, message.MessageType);
                        return;
                    }

                    string serialized = lobby.ToJson();

                    var lobbyListResponse = new NET_LobbyListResponse()
                    {
                        SerializedLobby = serialized
                    };

                    var messageResponse = CreateNetworkMessage(lobbyListResponse, MessageType.NET_LobbyListResponse);

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
                var lobbyListResponse = message.ParsedBody.Deserialize<NET_LobbyListResponse>();
                var NET_LobbyListResponse = new NET_LobbyListResponse()
                {
                    SerializedLobby = lobbyListResponse.SerializedLobby
                };
                IPCManager.SendLobbyListResponse(lobbyListResponse);
            }
            catch (Exception ex)
            {
                Write(ex);
            }

            CloseSocket(socket, message.MessageType);
        }

        private static void ProcessAnnounce(NetworkMessage message, Socket socket)
        {
            try
            {
                if (NetworkHelper.IslocalAddress(((IPEndPoint)socket.RemoteEndPoint).Address))
                {
                    CloseSocket(socket, message.MessageType);
                    return;
                }

                var announce = message.ParsedBody.Deserialize<NET_Announce>();
                string IPAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();

                // Add User to List on both cases (NET_Announce and NET_AnnounceResponse)
                if (announce != null && announce.AccountID != SteamClient.AccountID)
                {
                    UserManager.AddOrUpdateUser(announce.AccountID, announce.PersonaName, announce.AppID, IPAddress);
                }

                if (message.MessageType == (int)MessageType.NET_Announce)
                {
                    var announceResponse = new NET_Announce()
                    {
                        PersonaName = SteamClient.PersonaName,
                        AccountID = SteamClient.AccountID
                    };
                    var messageResponse = CreateNetworkMessage(announceResponse, MessageType.NET_AnnounceResponse);
                    string json = messageResponse.ToJson();
                    socket.Send(json.GetBytes());

                    string hexAvatar = ImageHelper.GetImageBase64(SteamClient.Avatar);
                    var avatarResponse = new NET_AvatarResponse()
                    {
                        AccountID = (uint)SteamClient.AccountID,
                        HexAvatar = hexAvatar
                    };
                    var messageResponse2 = CreateNetworkMessage(avatarResponse, MessageType.NET_AvatarResponse);
                    SendTo(IPAddress, messageResponse2);

                    // Connection pair close the socket
                }
                else
                {
                    CloseSocket(socket, message.MessageType);
                }
            }
            catch
            {
                modCommon.Show("err");
            }
        }

        private static void ProcessUserStatusChanged(NetworkMessage message, Socket socket)
        {
            try
            {
                var StatusChanged = message.ParsedBody.Deserialize<NET_UserDataUpdated>();
                if (StatusChanged != null)
                {
                    if (StatusChanged.AccountID == (uint)SteamClient.AccountID) return;
                    var IPAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();

                    var user = UserManager.GetUser((ulong)new CSteamID(StatusChanged.AccountID));
                    IPC_UserDataUpdated.UpdateType updateType = default;
                    if (user != null)
                    {
                        user.IPAddress = IPAddress;

                        if (user.PersonaName != StatusChanged.PersonaName)
                        {
                            user.PersonaName = StatusChanged.PersonaName;
                            updateType = IPC_UserDataUpdated.UpdateType.PersonaName;
                        }

                        if (user.LobbyID != StatusChanged.LobbyID)
                        {
                            user.LobbyID = StatusChanged.LobbyID;
                            updateType = IPC_UserDataUpdated.UpdateType.LobbyID;
                        }
                    }

                    IPCManager.UpdateUserStatus(StatusChanged, updateType);

                }
            }
            catch
            {
            }

            CloseSocket(socket, message.MessageType);
        }


        public static void SendGameOpened(Game game)
        {
            var GameOpened = new NET_GameOpened()
            {
                AccountID = SteamClient.AccountID,
                AppID = game.AppID,  
                Name = game.Name
            };
            var message = CreateNetworkMessage(GameOpened, MessageType.NET_GameOpened);
            SendBroadcast(message);
        }

        private static void ProcessGameOpened(NetworkMessage message, Socket socket)
        {
            try
            {
                var GameOpened = message.ParsedBody.Deserialize<NET_GameOpened>();
                GameManager.InvokeUserGameOpened(GameOpened);
            }
            catch  { }
        }

        #endregion

        #region Send Messages

        public static void SendLobbyJoinRequest(IPC_LobbyJoinRequest LobbyJoinRequest)
        {
            Write($"Sending lobby request");
            try
            {
                var JoinRequest = new NET_LobbyJoinRequest()
                {
                    LobbyID = LobbyJoinRequest.LobbyID,
                    SteamID = LobbyJoinRequest.SteamID,
                    CallbackHandle = LobbyJoinRequest.CallbackHandle
                };

                var lobby = LobbyManager.GetLobby(LobbyJoinRequest.LobbyID);
                if (lobby == null)
                {
                    Write($"Lobby {LobbyJoinRequest.LobbyID} not exists");
                    return;
                }

                var message = CreateNetworkMessage(JoinRequest, MessageType.NET_LobbyJoinRequest);

                var user = UserManager.GetUser(lobby.Owner);
                if (user == null)
                {
                    Write($"Not found user to send LobbyJoinRequest, SteamID {new CSteamID(lobby.Owner).ToString()}");
                    return;
                }

                SendTo(user.IPAddress, message);
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        internal static void SendLobbyDataUpdate(IPC_LobbyDataUpdate LobbyDataUpdate)
        {
            var user = UserManager.GetUser(LobbyDataUpdate.TargetSteamID);
            if (user != null)
            {
                var NETLobbyDataUpdate = new NET_LobbyDataUpdate()
                {
                    SerializedLobby = LobbyDataUpdate.SerializedLobby,
                    SteamIDLobby = LobbyDataUpdate.SteamIDLobby,
                    SteamIDMember = LobbyDataUpdate.SteamIDMember,
                };
                var message = CreateNetworkMessage(NETLobbyDataUpdate, MessageType.NET_LobbyDataUpdate);
                SendTo(user.IPAddress, message);
            }
        }

        public static void SendLobbyListRequest(IPC_LobbyListRequest lobbyListRequest)
        {
            var LobbyListRequest = new NET_LobbyListRequest()
            {
                AppID = lobbyListRequest.AppID,
                RequestID = lobbyListRequest.RequestID
            };
            var message = CreateNetworkMessage(LobbyListRequest, MessageType.NET_LobbyListRequest);
            SendBroadcast(message);
        }

        public static void SendLobbyLeave(ulong owner, ulong lobbyID)
        {
            var user = UserManager.GetUser(owner);
            if (user != null)
            {
                var LobbyLeave = new NET_LobbyLeave()
                {
                    LobbyID = lobbyID,
                    SteamID = (ulong)SteamClient.SteamID
                };

                var message = CreateNetworkMessage(LobbyLeave, MessageType.NET_LobbyLeave);
                SendTo(user.IPAddress, message);
            }
        }

        public static void SendLobbyDataUpdate(ulong IDTarget, ulong IDLobby, ulong IDMember, SteamLobby lobby)
        {
            var lobbyDataUpdate = new NET_LobbyDataUpdate()
            {
                SteamIDLobby = IDLobby,
                SteamIDMember = IDMember,
                SerializedLobby = lobby.ToJson()
            };

            var message = CreateNetworkMessage(lobbyDataUpdate, MessageType.NET_LobbyDataUpdate);

            var user = UserManager.GetUser(IDTarget);
            if (user != null)
            {
                SendTo(user.IPAddress, message);
            }
        }

        public static void SendLobbyRemove(SteamLobby lobby)
        {
            foreach (var member in lobby.Members)
            {
                if (member.m_SteamID != lobby.Owner)
                {
                    var user = UserManager.GetUser(member.m_SteamID);
                    if (user != null)
                    {
                        var lobbyRemove = new NET_LobbyRemove()
                        {
                            LobbyID = lobby.SteamID
                        };

                        var message = CreateNetworkMessage(lobbyRemove, MessageType.NET_LobbyRemove);
                        SendTo(user.IPAddress, message);
                    }
                }
            }
        }

        private static void SendBroadcast(NetworkMessage message)
        {
            Task.Run(() =>
            {
                foreach (var address in NetworkHelper.GetIPAddresses())
                {
                    var Addressess = NetworkHelper.GetIPAddressRange(address);
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

            });
        }

        #endregion

        #region NET_LobbyChatUpdate
        private static void ProcessLobbyChatUpdate(NetworkMessage message, Socket socket)
        {
            var lobbyChatUpdate = message.ParsedBody.Deserialize<NET_LobbyChatUpdate>();
            if (lobbyChatUpdate != null)
            {
                IPCManager.SendLobbyChatUpdate(lobbyChatUpdate);
            }
        }

        public static void SendLobbyChatUpdate(ulong TargetUser, IPC_LobbyChatUpdate lobbyChatUpdate)
        {
            var LobbyChatUpdate = new NET_LobbyChatUpdate()
            {
                SteamIDLobby = lobbyChatUpdate.SteamIDLobby,
                ChatMemberStateChange = lobbyChatUpdate.ChatMemberStateChange,
                SteamIDMakingChange = lobbyChatUpdate.SteamIDMakingChange,
                SteamIDUserChanged = lobbyChatUpdate.SteamIDUserChanged
            };
            var user = UserManager.GetUser(TargetUser);
            if (user != null)
            {
                var message = CreateNetworkMessage(LobbyChatUpdate, MessageType.NET_LobbyChatUpdate);
                SendTo(user.IPAddress, message);
            }
        }

        #endregion

        #region NET_AvatarRequest&Response
        public static void SendAvatarRequest(IPC_AvatarRequest avatarRequest)
        {
            var message = CreateNetworkMessage(new NET_Base(), MessageType.NET_AvatarRequest);
            var user = UserManager.GetUser(avatarRequest.AccountID);
            if (user != null)
            {
                SendTo(user.IPAddress, message);
            }
        }

        public static void SendAnnounce()
        {
            BroadcastAnnounce();
        }

        private static void ProcessAvatar(NetworkMessage message, Socket socket)
        {
            if (message.MessageType == (int)MessageType.NET_AvatarRequest)
            {
                try
                {
                    var imageBytes = ImageHelper.ImageToBytes(SteamClient.Avatar);
                    string hexAvatar = Convert.ToBase64String(imageBytes);
                    var avatarResponse = new NET_AvatarResponse()
                    {
                        AccountID = (uint)SteamClient.AccountID,
                        HexAvatar = hexAvatar
                    };
                    var messageResponse = CreateNetworkMessage(avatarResponse, MessageType.NET_AvatarResponse);
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
                    var AvatarResponse = message.ParsedBody.Deserialize<NET_AvatarResponse>();
                    if (AvatarResponse != null)
                    {
                        var imageBytes = Convert.FromBase64String(AvatarResponse.HexAvatar);
                        if (imageBytes.Length != 0)
                        {
                            Bitmap Avatar = (Bitmap)ImageHelper.ImageFromBytes(imageBytes);
                            IPCManager.AvatarResponse(AvatarResponse.HexAvatar, AvatarResponse.AccountID);
                            try
                            {
                                string AvatarCachePath = Path.Combine(modCommon.GetPath(), "Data", "Images", "AvatarCache", AvatarResponse.AccountID + ".jpg");
                                modCommon.EnsureDirectoryExists(AvatarCachePath, true);
                                ImageHelper.ToFile(AvatarCachePath, Avatar);
                                UserManager.AvatarReceived(AvatarResponse.AccountID, Avatar);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Write($"{ex}");
                }
                CloseSocket(socket, message.MessageType);
            }
        }
        #endregion

        private static void BroadcastAnnounce()
        {
            NET_Announce announce = new NET_Announce()
            {
                PersonaName = SteamClient.PersonaName,
                AccountID = SteamClient.AccountID
            };

            var message = CreateNetworkMessage(announce, MessageType.NET_Announce);
            SendBroadcast(message);
        }

        private static void CheckInUserList(Socket socket)
        {
            var IPAddress = ((IPEndPoint)socket.RemoteEndPoint).Address;
            if (NetworkHelper.IslocalAddress(IPAddress))
            {
                return;
            }
            var user = UserManager.GetUserByAddress(IPAddress.ToString());
            if (user == null)
            {
                SendAnnounceTo(IPAddress.ToString());
            }
        }

        public static void SendUserDataUpdated(IPC_UserDataUpdated userDataUpdated)
        {
            // TODO: Broadcast to all users in network
            var UserDataUpdated = new NET_UserDataUpdated()
            {
                PersonaName = userDataUpdated.PersonaName,
                AccountID = userDataUpdated.AccountID,  
                LobbyID = userDataUpdated.LobbyID
            };

            var message = CreateNetworkMessage(UserDataUpdated, MessageType.NET_UserDataUpdated);

            SendBroadcast(message);
        }

        #region NET_LobbyGameserver
        public static void SendLobbyGameserver(IPC_LobbyGameserver lobbyGameserver)
        {
            var LobbyGameserver = new NET_LobbyGameserver()
            {
                LobbyID = lobbyGameserver.SteamID,
                SteamID = lobbyGameserver.SteamID,
                IP = lobbyGameserver.IP,
                Port = lobbyGameserver.Port
            };

            var GameserverMessage = CreateNetworkMessage(LobbyGameserver, MessageType.NET_LobbyGameserver);

            var lobby = LobbyManager.GetLobby(lobbyGameserver.SteamID);
            if (lobby == null)
            {
                Write($"Not found lobby {lobbyGameserver.SteamID} to send game server");
                return;
            }

            foreach (var member in lobby.Members)
            {
                if (member.m_SteamID != (ulong)SteamClient.SteamID)
                {
                    var user = UserManager.GetUser(member.m_SteamID);
                    if (user != null)
                    {
                        SendTo(user.IPAddress, GameserverMessage);
                    }
                }
            }
        }

        private static void ProcessLobbyGameserver(NetworkMessage message, Socket socket)
        {
            var lobbyGameserver = message.ParsedBody.Deserialize<NET_LobbyGameserver>();
            if (lobbyGameserver != null)
            {
                IPCManager.SendLobbyGameserver(lobbyGameserver);
            }
            CloseSocket(socket, message.MessageType);
        }
        #endregion


        public static void SendAnnounceTo(string remoteAddress)
        {
            var announce = new NET_Announce()
            {
                PersonaName = SteamClient.PersonaName,
                AccountID = (uint)SteamClient.SteamID.AccountID
            };

            var message = CreateNetworkMessage(announce, MessageType.NET_Announce);

            SendTo(remoteAddress, message);
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

                TCPClient client = new TCPClient(SocketData.socket);
                client.OnDataReceived += TCPServer_OnDataReceived;
                client.BeginReceiving();
            }
            catch
            {
            }
        }

        #endregion

        private static NetworkMessage CreateNetworkMessage(NET_Base Base, MessageType type)
        {
            var message = new NetworkMessage()
            {
                MessageType = (int)type,
                ParsedBody = Base.ToJson()
            };
            return message;
        }

        private static void CloseSocket(Socket socket, int messageType)
        {
            //Write($"Closing connection after received {(MessageType)messageType} message");
            try
            {
                socket.Close();
                socket.Dispose();
            }
            catch
            {
            }
        }

        private static void Write(object msg)
        {
            Log.Write("NetworkManager", msg);
        }

        private class SocketData
        {
            public Socket socket { get; set; }
            public NetworkMessage Message { get; set; }
        }
    }
}
