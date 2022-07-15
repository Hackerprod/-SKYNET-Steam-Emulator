using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;
using SKYNET.Client;
using SKYNET.Common;
using SKYNET.Helpers;
using SKYNET.IPC.Types;
using SKYNET.Network;
using SKYNET.Network.Types;
using SKYNET.Steamworks;
using SKYNET.Types;

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
            NETMessage message = packet.Data.GetString().Deserialize<NETMessage>();
            ProcessMessage(message, packet.Sender.Socket);
        }

        private static void ProcessMessage(NETMessage message, Socket socket)
        {
            Write($"Received message {(NETMessageType)message.MessageType} from {((IPEndPoint)socket.RemoteEndPoint).Address.ToString()}");
            try
            {
                switch ((NETMessageType)message.MessageType)
                {
                    case NETMessageType.NET_Announce:
                    case NETMessageType.NET_AnnounceResponse:
                        ProcessAnnounce(message, socket);
                        break;
                    case NETMessageType.NET_UserDataUpdated:
                        ProcessUserStatusChanged(message, socket);
                        break;
                    case NETMessageType.NET_LobbyListRequest:
                        ProcessLobbyListRequest(message, socket);
                        break;
                    case NETMessageType.NET_LobbyListResponse:
                        ProcessLobbyListResponse(message, socket);
                        break;
                    case NETMessageType.NET_LobbyJoinRequest:
                        ProcessLobbyJoinRequest(message, socket);
                        break;
                    case NETMessageType.NET_LobbyJoinResponse:
                        ProcessLobbyJoinResponse(message, socket);
                        break;
                    case NETMessageType.NET_LobbyDataUpdate:
                        ProcessLobbyDataUpdate(message, socket);
                        break;
                    case NETMessageType.NET_LobbyChatUpdate:
                        ProcessLobbyChatUpdate(message, socket);
                        break;
                    case NETMessageType.NET_LobbyLeave:
                        ProcessLobbyLeave(message, socket);
                        break;
                    case NETMessageType.NET_LobbyRemove:
                        ProcessLobbyRemove(message, socket);
                        break;
                    case NETMessageType.NET_LobbyGameserver:
                        ProcessLobbyGameserver(message, socket);
                        break;
                    case NETMessageType.NET_GameOpened:
                        ProcessGameOpened(message, socket);
                        break;
                    case NETMessageType.NET_SetRichPresence:
                        ProcessSetRichPresence(message, socket);
                        break;
                    case NETMessageType.NET_LobbyCreated:
                        ProcessLobbyCreated(message, socket);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Write($"Error processing message {(NETMessageType)message.MessageType}" + Environment.NewLine + ex.Message + ex.StackTrace);
            }
        }

        #region Received Messages
        private static void ProcessLobbyLeave(NETMessage message, Socket socket)
        {
            // Steam lobby owner
            var lobbyLeave = message.ParsedBody.Deserialize<NET_LobbyLeave>();
            if (lobbyLeave != null)
            {
                LobbyManager.LeaveLobby(lobbyLeave);
                IPCManager.SendLobbyLeave(lobbyLeave);
            }

            CloseSocket(socket, message.MessageType);
        }

        private static void ProcessLobbyRemove(NETMessage message, Socket socket)
        {
            var lobbyRemove = message.ParsedBody.Deserialize<NET_LobbyRemove>();
            if (lobbyRemove != null)
            {
                IPCManager.SendLobbyRemove(lobbyRemove.LobbyID);
                LobbyManager.Remove(lobbyRemove.LobbyID);
            }
            CloseSocket(socket, message.MessageType);
        }

        private static void ProcessLobbyDataUpdate(NETMessage message, Socket socket)
        {
            var LobbyDataUpdate = message.ParsedBody.Deserialize<NET_LobbyDataUpdate>();
            if (LobbyDataUpdate != null)
            {
                IPCManager.SendLobbyDataUpdate(LobbyDataUpdate);
            }
        }

        private static void ProcessLobbyJoinRequest(NETMessage message, Socket socket)
        {
            try
            {
                string RemoteIPAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
                var LobbyJoinRequest = message.ParsedBody.Deserialize<NET_LobbyJoinRequest>();
                var LobbyJoinResponse = new NET_LobbyJoinResponse()
                {
                    CallbackHandle = LobbyJoinRequest.CallbackHandle,
                    ChatRoomEnterResponse = (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess,
                };

                if (LobbyJoinRequest != null)
                {
                    if (LobbyManager.GetLobby(LobbyJoinRequest.LobbyID, out var lobby))
                    {
                        if (lobby.Members.Count >= lobby.MaxMembers)
                        {
                            LobbyJoinResponse.ChatRoomEnterResponse = (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseFull;
                            SendTo(RemoteIPAddress, LobbyJoinResponse, NETMessageType.NET_LobbyJoinResponse);
                        }

                        var Member = lobby.Members.Find(m => m.m_SteamID == LobbyJoinRequest.SteamID);
                        if (Member == null)
                        {
                            lobby.Members.Add(new SteamLobby.LobbyMember()
                            {
                                m_SteamID = LobbyJoinRequest.SteamID
                            });
                        }

                        UserManager.UpdateUserLobby(LobbyJoinRequest.SteamID, LobbyJoinRequest.LobbyID);

                        var SerializedLobby = lobby.Serialize();
                        LobbyJoinResponse.SerializedLobby = SerializedLobby;

                        SendUpdateLobby(lobby, false);

                        SendTo(RemoteIPAddress, LobbyJoinResponse, NETMessageType.NET_LobbyJoinResponse);

                        // LobbyDataUpdate
                        var lobbyDataUpdate = new NET_LobbyDataUpdate()
                        {
                            SteamIDLobby = lobby.SteamID,
                            SteamIDMember = LobbyJoinRequest.SteamID,
                            SerializedLobby = SerializedLobby
                        };

                        var lobbyChatUpdate = new NET_LobbyChatUpdate()
                        {
                            SteamIDLobby = lobby.SteamID,
                            SteamIDUserChanged = LobbyJoinRequest.SteamID,
                            SteamIDMakingChange = LobbyJoinRequest.SteamID,
                            ChatMemberStateChange = (int)EChatMemberStateChange.k_EChatMemberStateChangeEntered
                        };

                        foreach (var member in lobby.Members)
                        {
                            var user = UserManager.GetUser(member.m_SteamID);
                            if (user != null)
                            {
                                SendTo(user.IPAddress, lobbyDataUpdate, NETMessageType.NET_LobbyDataUpdate);
                                SendTo(user.IPAddress, lobbyChatUpdate, NETMessageType.NET_LobbyChatUpdate);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Write(ex);
            }

            CloseSocket(socket, message.MessageType);
        }

        private static void ProcessLobbyJoinResponse(NETMessage message, Socket socket)
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

        private static void ProcessLobbyListRequest(NETMessage message, Socket socket)
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

                    string serialized = lobby.Serialize();

                    var lobbyListResponse = new NET_LobbyListResponse()
                    {
                        SerializedLobby = serialized
                    };

                    string RemoteIPAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
                    SendTo(RemoteIPAddress, lobbyListResponse, NETMessageType.NET_LobbyListResponse);
                }
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        private static void ProcessLobbyListResponse(NETMessage message, Socket socket)
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

        internal static void SendRichPresence(string key, string value)
        {
            NET_SetRichPresence RichPresence = new NET_SetRichPresence()
            {
                AccountID = SteamClient.AccountID,
                Key = key,
                Value = value
            };
            SendBroadcast(RichPresence, NETMessageType.NET_SetRichPresence);
        }

        private static void ProcessSetRichPresence(NETMessage message, Socket socket)
        {
            try
            {
                var SetRichPresence = message.ParsedBody.Deserialize<NET_SetRichPresence>();
                if (SetRichPresence == null) return;
                if (SetRichPresence.AccountID == SteamClient.AccountID) return; 
                UserManager.SetRichPresence(SetRichPresence.AccountID, SetRichPresence.Key, SetRichPresence.Value);
            }
            catch (Exception)
            {
            }
        }

        private static void ProcessAnnounce(NETMessage message, Socket socket)
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

                if (message.MessageType == (int)NETMessageType.NET_Announce)
                {
                    var announceResponse = new NET_Announce()
                    {
                        PersonaName = SteamClient.PersonaName,
                        AccountID = SteamClient.AccountID
                    };
                    string RemoteIPAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
                    SendTo(RemoteIPAddress, announceResponse, NETMessageType.NET_AnnounceResponse);
                    UserManager.DownloadAvatar(announce.AccountID, IPAddress);
                }
                else
                {
                    UserManager.DownloadAvatar(announce.AccountID, IPAddress);
                    CloseSocket(socket, message.MessageType);
                }
            }
            catch
            {
                modCommon.Show("err");
            }
        }

        private static void ProcessUserStatusChanged(NETMessage message, Socket socket)
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
            SendBroadcast(GameOpened, NETMessageType.NET_GameOpened);
        }

        private static void ProcessGameOpened(NETMessage message, Socket socket)
        {
            try
            {
                var GameOpened = message.ParsedBody.Deserialize<NET_GameOpened>();
                if (GameOpened == null) return;
                UserManager.UpdateUserPlaying(GameOpened.AccountID, GameOpened.AppID);

                GameManager.InvokeUserGameOpened(GameOpened);
            }
            catch  { }
        }

        #endregion

        #region Send Messages

        internal static void SendUpdateLobby(SteamLobby lobby, bool IncludeOwner)
        {
            var LobbyUpdate = new NET_LobbyUpdate()
            {
                SteamID = lobby.SteamID,
                SerializedLobby = lobby.Serialize()
            };

            var members = IncludeOwner ? lobby.Members : lobby.Members.FindAll(m => m.m_SteamID != lobby.Owner); ;

            foreach (var member in members)
            {
                var user = UserManager.GetUser(member.m_SteamID);
                SendTo(user.IPAddress, LobbyUpdate, NETMessageType.NET_LobbyUpdate);
            }
        }

        internal static void ProcessLobbyUpdate(NETMessage message, Socket socket)
        {
            var LobbyUpdate = message.ParsedBody.Deserialize<NET_LobbyUpdate>();
            if (LobbyUpdate != null) return;
            var Lobby = LobbyUpdate.SerializedLobby.Deserialize<SteamLobby>();
            LobbyManager.Update(Lobby);
        }

        public static void SendLobbyJoinRequest(IPC_LobbyJoinRequest LobbyJoinRequest)
        {
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

                var user = UserManager.GetUser(lobby.Owner);
                if (user == null)
                {
                    Write($"Not found user to send LobbyJoinRequest, SteamID {new CSteamID(lobby.Owner).ToString()}");
                    return;
                }

                SendTo(user.IPAddress, JoinRequest, NETMessageType.NET_LobbyJoinRequest);
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        public static void ProcessLobbyJoinRequest(IPC_LobbyJoinRequest LobbyJoinRequest)
        {
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

                var user = UserManager.GetUser(lobby.Owner);
                if (user == null)
                {
                    Write($"Not found user to send LobbyJoinRequest, SteamID {new CSteamID(lobby.Owner).ToString()}");
                    return;
                }

                SendTo(user.IPAddress, JoinRequest, NETMessageType.NET_LobbyJoinRequest);
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
                SendTo(user.IPAddress, NETLobbyDataUpdate, NETMessageType.NET_LobbyDataUpdate);
            }
        }

        public static void SendLobbyListRequest(IPC_LobbyListRequest lobbyListRequest)
        {
            var LobbyListRequest = new NET_LobbyListRequest()
            {
                AppID = lobbyListRequest.AppID,
                RequestID = lobbyListRequest.RequestID
            };
            SendBroadcast(LobbyListRequest, NETMessageType.NET_LobbyListRequest);
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

                SendTo(user.IPAddress, LobbyLeave, NETMessageType.NET_LobbyLeave);
            }
        }

        public static void SendLobbyDataUpdate(ulong IDTarget, ulong IDLobby, ulong IDMember, SteamLobby lobby)
        {
            var lobbyDataUpdate = new NET_LobbyDataUpdate()
            {
                SteamIDLobby = IDLobby,
                SteamIDMember = IDMember,
                SerializedLobby = lobby.Serialize()
            };

            var user = UserManager.GetUser(IDTarget);
            if (user != null)
            {
                SendTo(user.IPAddress, lobbyDataUpdate, NETMessageType.NET_LobbyDataUpdate);
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

                        SendTo(user.IPAddress, lobbyRemove, NETMessageType.NET_LobbyRemove);
                    }
                }
            }
        }

        private static void SendBroadcast(NET_Base Base, NETMessageType type)
        {
            var message = new NETMessage()
            {
                MessageType = (int)type,
                ParsedBody = Base.Serialize()
            };

            Task.Run(() =>
            {
                foreach (var address in NetworkHelper.GetIPAddresses())
                {
                    var Addressess = NetworkHelper.GetIPAddressRange(address);
                    foreach (var Address in Addressess)
                    {
                        if (Address != address.ToString())
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

            });
        }

        #endregion

        #region NET_LobbyChatUpdate
        private static void ProcessLobbyChatUpdate(NETMessage message, Socket socket)
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
                SendTo(user.IPAddress, LobbyChatUpdate, NETMessageType.NET_LobbyChatUpdate);
            }
        }

        #endregion

        public static void SendAnnounce()
        {
            BroadcastAnnounce();
        }

        private static void BroadcastAnnounce()
        {
            NET_Announce announce = new NET_Announce()
            {
                PersonaName = SteamClient.PersonaName,
                AccountID = SteamClient.AccountID
            };

            SendBroadcast(announce, NETMessageType.NET_Announce);
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

        internal static void SendLobbyCreated(SteamLobby lobby)
        {
            var LobbyCreated = new NET_LobbyCreated()
            {
                SteamID = (ulong)SteamClient.SteamID,
                SerializedLobby = lobby.Serialize()
            };
            SendBroadcast(LobbyCreated, NETMessageType.NET_LobbyCreated);
        }

        private static void ProcessLobbyCreated(NETMessage message, Socket socket)
        {
            var LobbyCreated = message.ParsedBody.Deserialize<NET_LobbyCreated>();
            if (LobbyCreated == null) return;
            var Lobby = LobbyCreated.SerializedLobby.Deserialize<SteamLobby>();
            LobbyManager.Create(Lobby);
            UserManager.UpdateUserLobby(LobbyCreated.SteamID, Lobby.SteamID);
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

            SendBroadcast(UserDataUpdated, NETMessageType.NET_UserDataUpdated);
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

            var lobby = LobbyManager.GetLobby(lobbyGameserver.LobbyID);
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
                        SendTo(user.IPAddress, LobbyGameserver, NETMessageType.NET_LobbyGameserver);
                    }
                }
            }
        }

        private static void ProcessLobbyGameserver(NETMessage message, Socket socket)
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

            SendTo(remoteAddress, announce, NETMessageType.NET_Announce);
        }

        public static void SendTo(string iPAddress, NET_Base Base, NETMessageType type)
        {
            Write($"Sending {type} to {iPAddress}");

            var message = new NETMessage()
            {
                MessageType = (int)type,
                ParsedBody = Base.Serialize()
            };

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

                string json = SocketData.Message.Serialize();
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
            public NETMessage Message { get; set; }
        }
    }
}
