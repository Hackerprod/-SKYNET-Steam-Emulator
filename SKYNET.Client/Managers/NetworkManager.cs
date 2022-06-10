using SKYNET.Client;
using SKYNET.Common;
using SKYNET.Helper;
using SKYNET.IPC.Types;
using SKYNET.Network;
using SKYNET.Network.Packets;
using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SKYNET.Managers
{
    public class NetworkManager
    {
        public static int Port = 28880;
        public static TCPServer TCPServer;
        public static FileServer FileServer;

        public static void Initialize()
        {
            //Port = SteamEmulator.BroadcastPort;
            Port = 28080;

            if (IsAvailablePort(Port))
            {
                Write($"Initializing TCP server on port {Port}");

                TCPServer = new TCPServer(Port);
                TCPServer.OnDataReceived += TCPServer_OnDataReceived;
                TCPServer.OnConnected += TCPServer_OnConnected;

                TCPServer.Start();
            }
            else
            {
                Write($"Error initializing TCP server, port {Port} is in use");
            }

            FileServer = new FileServer(80);
            FileServer.Start();

            BroadcastAnnounce();
        }

        private static void TCPServer_OnConnected(object sender, TCPClient e)
        {
            Write("Client connected from " + e.RemoteEndPoint);
        }

        private static void TCPServer_OnDataReceived(object sender, NetPacket packet)
        {
            NetworkMessage message = packet.Data.GetString().FromJson<NetworkMessage>();
            ProcessMessage(message, packet.Sender.Socket);
        }

        //#region Message processors

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
                case MessageType.NET_LobbyListRequest:
                    ProcessLobbyListRequest(message, socket);
                    break;
                case MessageType.NET_LobbyListResponse:
                    ProcessLobbyListResponse(message, socket);
                    break;
                //case MessageType.NET_LobbyJoinRequest:
                //    ProcessLobbyJoinRequest(message, socket);
                //    break;
                //case MessageType.NET_LobbyJoinResponse:
                //    ProcessLobbyJoinResponse(message, socket);
                //    break;
                //case MessageType.NET_LobbyDataUpdate:
                //    ProcessLobbyDataUpdate(message, socket);
                //    break;
                //case MessageType.NET_LobbyChatUpdate:
                //    ProcessLobbyChatUpdate(message, socket);
                //    break;
                //case MessageType.NET_LobbyLeave:
                //    ProcessLobbyLeave(message, socket);
                //    break;
                //case MessageType.NET_LobbyRemove:
                //    ProcessLobbyRemove(message, socket);
                //    break;
                //case MessageType.NET_LobbyGameserver:
                //    ProcessLobbyGameserver(message, socket);
                //    break;

                default:
                    break;
            }
        }

        internal static void SendLobbyDataUpdate(ulong userID, IPC_LobbyDataUpdate LobbyDataUpdate)
        {
            var user = UserManager.GetUser(userID);
            if (user != null)
            {
                var NETLobbyDataUpdate = new NET_LobbyDataUpdate()
                {
                    ParsedLobby = LobbyDataUpdate.ParsedLobby,
                    SteamIDLobby = LobbyDataUpdate.SteamIDLobby,
                    SteamIDMember = LobbyDataUpdate.SteamIDMember,
                };
                var message = CreateNetworkMessage(NETLobbyDataUpdate, MessageType.NET_LobbyDataUpdate);
                SendTo(user.IPAddress, message);
            }
        }

        private static void BroadcastAnnounce()
        {
            NET_Announce announce = new NET_Announce()
            {
                PersonaName = SteamClient.PersonaName,
                AccountID = SteamClient.AccountID
            };

            var message = CreateNetworkMessage(announce, MessageType.NET_Announce);
            ThreadPool.QueueUserWorkItem(SendBroadcast, message);
        }

        //private static void CheckInUserList(Socket socket)
        //{
        //    var IPAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
        //    if (IslocalAddress(IPAddress))
        //    {
        //        return;
        //    }
        //    var user = SteamFriends.Instance.GetUserByAddress(IPAddress);
        //    if (user == null)
        //    {
        //        AnnounceTo(IPAddress);
        //    }
        //}

        //private static void ProcessLobbyGameserver(NetworkMessage message, Socket socket)
        //{
        //    var lobbyGameserver = message.ParsedBody.FromJson<NET_LobbyGameserver>();
        //    if (lobbyGameserver != null)
        //    {
        //        if (SteamMatchmaking.Instance.GetLobby(lobbyGameserver.LobbyID, out var lobby))
        //        {
        //            Write($"Received gameserver data for lobby {lobbyGameserver.LobbyID}, IP = {lobbyGameserver.IP}, Port = {lobbyGameserver.Port}");
        //            lobby.Gameserver.SteamID = lobbyGameserver.SteamID;
        //            lobby.Gameserver.IP = lobbyGameserver.IP;
        //            lobby.Gameserver.Port = lobbyGameserver.Port;
        //            lobby.Gameserver.Filled = true;

        //            // TODO: Necessary?
        //            GameServerChangeRequested_t data = new GameServerChangeRequested_t()
        //            {
        //                m_rgchServer = $"{lobbyGameserver.IP}:{lobbyGameserver.Port}"
        //            };
        //            CallbackManager.AddCallbackResult(data);
        //        }
        //    }
        //    CloseSocket(socket, message.MessageType);
        //}

        //private static void ProcessLobbyLeave(NetworkMessage message, Socket socket)
        //{
        //    var lobbyLeave = message.ParsedBody.FromJson<NET_LobbyLeave>();
        //    if (lobbyLeave != null)
        //    {
        //        if (SteamMatchmaking.Instance.GetLobby(lobbyLeave.LobbyID, out var lobby))
        //        {
        //            var lobbyChatUpdate = new NET_LobbyChatUpdate()
        //            {
        //                SteamIDLobby = lobby.SteamID,
        //                SteamIDUserChanged = lobbyLeave.SteamID,
        //                SteamIDMakingChange = lobbyLeave.SteamID,
        //                ChatMemberStateChange = (int)EChatMemberStateChange.k_EChatMemberStateChangeLeft
        //            };

        //            lobby.Members.RemoveAll(m => m.m_SteamID == lobbyLeave.SteamID);

        //            var ChatUpdateMessage = CreateNetworkMessage(lobbyChatUpdate, MessageType.NET_LobbyChatUpdate);

        //            foreach (var member in lobby.Members)
        //            {
        //                var user = SteamFriends.Instance.GetUser(member.m_SteamID);
        //                if (user != null)
        //                {
        //                    SendTo(user.IPAddress, ChatUpdateMessage);
        //                    //SteamMatchmaking.Instance.LobbyDataUpdated();
        //                }
        //            }

        //            var data = new LobbyChatUpdate_t()
        //            {
        //                m_ulSteamIDLobby = lobbyLeave.LobbyID,
        //                m_ulSteamIDUserChanged = lobbyLeave.SteamID,
        //                m_ulSteamIDMakingChange = lobbyLeave.SteamID,
        //                m_rgfChatMemberStateChange = (int)EChatMemberStateChange.k_EChatMemberStateChangeLeft
        //            };
        //            CallbackManager.AddCallback(data);
        //        }
        //    }

        //    CloseSocket(socket, message.MessageType);
        //}

        //private static void ProcessLobbyRemove(NetworkMessage message, Socket socket)
        //{
        //    var lobbyRemove = message.ParsedBody.FromJson<NET_LobbyRemove>();
        //    if (lobbyRemove != null)
        //    {
        //        SteamMatchmaking.Instance.RemoveLobby(lobbyRemove.LobbyID);
        //    }
        //    CloseSocket(socket, message.MessageType);
        //}

        //private static void ProcessLobbyChatUpdate(NetworkMessage message, Socket socket)
        //{
        //    var lobbyChatUpdate = message.ParsedBody.FromJson<NET_LobbyChatUpdate>();
        //    if (lobbyChatUpdate != null)
        //    {
        //        SteamMatchmaking.Instance.LobbyChatUpdated(lobbyChatUpdate);
        //    }
        //}

        //private static void ProcessLobbyDataUpdate(NetworkMessage message, Socket socket)
        //{
        //    var lobbyDataUpdate = message.ParsedBody.FromJson<NET_LobbyDataUpdate>();
        //    if (lobbyDataUpdate != null)
        //    {
        //        SteamMatchmaking.Instance.LobbyDataUpdated(lobbyDataUpdate);
        //    }
        //}

        //private static void ProcessLobbyJoinRequest(NetworkMessage message, Socket socket)
        //{
        //    var lobbyJoinRequest = message.ParsedBody.FromJson<NET_LobbyJoinRequest>();
        //    var lobbyJoinResponse = new NET_LobbyJoinResponse()
        //    {
        //        CallbackHandle = lobbyJoinRequest.CallbackHandle,
        //        ChatRoomEnterResponse = (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess,
        //    };
        //    if (lobbyJoinRequest != null)
        //    {
        //        if (SteamMatchmaking.Instance.GetLobby(lobbyJoinRequest.LobbyID, out var lobby))
        //        {
        //            if (lobby.Members.Count >= lobby.MaxMembers)
        //            {
        //                lobbyJoinResponse.ChatRoomEnterResponse = (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseFull;
        //            }

        //            var Member = lobby.Members.Find(m => m.m_SteamID == lobbyJoinRequest.SteamID);
        //            if (Member == null)
        //            {
        //                lobby.Members.Add(new SteamLobby.LobbyMember()
        //                {
        //                    m_SteamID = lobbyJoinRequest.SteamID
        //                });
        //            }

        //            string serialized = lobby.ToJson();

        //            lobbyJoinResponse.SerializedLobby = serialized;

        //            var lobbyJoinMessage = CreateNetworkMessage(lobbyJoinResponse, MessageType.NET_LobbyJoinResponse);

        //            try
        //            {
        //                string json = lobbyJoinMessage.ToJson();
        //                socket.Send(json.GetBytes());
        //            }
        //            catch
        //            {
        //            }

        //            // LobbyDataUpdate
        //            var lobbyDataUpdate = new NET_LobbyDataUpdate()
        //            {
        //                SteamIDLobby = lobby.SteamID,
        //                SteamIDMember = lobbyJoinRequest.SteamID,
        //                ParsedLobby = serialized
        //            };

        //            var lobbyChatUpdate = new NET_LobbyChatUpdate()
        //            {
        //                SteamIDLobby = lobby.SteamID,
        //                SteamIDUserChanged = lobbyJoinRequest.SteamID,
        //                SteamIDMakingChange = lobbyJoinRequest.SteamID,
        //                ChatMemberStateChange = (int)EChatMemberStateChange.k_EChatMemberStateChangeEntered
        //            };

        //            var DataUpdateMessage = CreateNetworkMessage(lobbyDataUpdate, MessageType.NET_LobbyDataUpdate);
        //            var ChatUpdateMessage = CreateNetworkMessage(lobbyChatUpdate, MessageType.NET_LobbyChatUpdate);

        //            foreach (var member in lobby.Members)
        //            {
        //                var user = SteamFriends.Instance.GetUser(member.m_SteamID);
        //                if (user != null)
        //                {
        //                    SendTo(user.IPAddress, DataUpdateMessage);
        //                    SendTo(user.IPAddress, ChatUpdateMessage);
        //                }
        //            }
        //        }
        //    }
        //}

        //private static void ProcessLobbyJoinResponse(NetworkMessage message, Socket socket)
        //{
        //    try
        //    {
        //        var lobbyJoinResponse = message.ParsedBody.FromJson<NET_LobbyJoinResponse>();
        //        if (lobbyJoinResponse != null)
        //        {
        //            var lobby = lobbyJoinResponse.SerializedLobby.FromJson<SteamMatchmaking.SteamLobby>();
        //            if (lobby != null)
        //            {
        //                SteamMatchmaking.Instance.JoinResponse(lobbyJoinResponse, lobby);
        //            }
        //            else
        //            {
        //                SteamMatchmaking.Instance.JoinResponse(lobbyJoinResponse, lobby);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Write(ex);
        //    }

        //    CloseSocket(socket, message.MessageType);
        //}

        private static void ProcessLobbyListRequest(NetworkMessage message, Socket socket)
        {
            try
            {
                var lobbyListRequest = message.ParsedBody.FromJson<NET_LobbyListRequest>();
                if (lobbyListRequest != null)
                {
                    //if (lobbyListRequest.RequestID == SteamMatchmaking.Instance.CurrentRequest)
                    //{
                    //    CloseSocket(socket, message.MessageType);
                    //    return;
                    //}
                    if (lobbyListRequest.AppID != 0 && lobbyListRequest.AppID != SteamEmulator.AppID)
                    {
                        CloseSocket(socket, message.MessageType);
                        return;
                    }
                }
                var lobby = SteamMatchmaking.Instance.GetLobbyByOwner((ulong)SteamEmulator.SteamID);
                if (lobby == null)
                {
                    CloseSocket(socket, message.MessageType);
                }
                else
                {
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
                var lobbyListResponse = message.ParsedBody.FromJson<NET_LobbyListResponse>();
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
                //if (IslocalAddress(((IPEndPoint)socket.RemoteEndPoint).Address))
                //{
                //    Write("Returning because is local address");
                //    CloseSocket(socket, message.MessageType);
                //    return;
                //}

                var announce = message.ParsedBody.FromJson<NET_Announce>();
                string IPAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();

                // Add User to List on both cases (NET_Announce and NET_AnnounceResponse)
                if (announce != null /*&& announce.AccountID != SteamClient.AccountID*/)
                {
                    UserManager.AddOrUpdateUser(announce.AccountID, announce.PersonaName, announce.AppID, IPAddress);
                    IPCManager.AddOrUpdateUser(announce.AccountID, announce.PersonaName, announce.AppID, IPAddress); 
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

                    // Connection pair close the socket
                }
                else
                {
                    CloseSocket(socket, message.MessageType);
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
                    var AvatarResponse = message.ParsedBody.FromJson<NET_AvatarResponse>();
                    if (AvatarResponse != null)
                    {
                        var imageBytes = Convert.FromBase64String(AvatarResponse.HexAvatar);
                        if (imageBytes.Length != 0)
                        {
                            Bitmap Avatar = (Bitmap)ImageHelper.ImageFromBytes(imageBytes);
                            ulong SteamID = (ulong)new CSteamID(AvatarResponse.AccountID);
                            IPCManager.AvatarResponse(AvatarResponse.HexAvatar, AvatarResponse.AccountID);
                            try
                            {
                                string AvatarCachePath = Path.Combine(modCommon.GetPath(), "SKYNET", "AvatarCache");
                                modCommon.EnsureDirectoryExists(AvatarCachePath);
                                Avatar.Save(Path.Combine(AvatarCachePath, AvatarResponse.AccountID + ".jpg"));
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

        private static void ProcessUserStatusChanged(NetworkMessage message, Socket socket)
        {
            try
            {
                var StatusChanged = message.ParsedBody.FromJson<NET_UserDataUpdated>();
                if (StatusChanged != null)
                {
                    if (StatusChanged.AccountID == (uint)SteamClient.AccountID) return;
                    var IPAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
                    IPCManager.UpdateUserStatus(StatusChanged, IPAddress);
                }
            }
            catch
            {
            }

            CloseSocket(socket, message.MessageType);
        }

        public static void BroadcastStatusUpdated(SteamPlayer user)
        {
            var status = new NET_UserDataUpdated()
            {
                PersonaName = user.PersonaName,
                AccountID = (uint)SteamClient.AccountID,
                LobbyID = user.LobbyID.GetAccountID()
            };

            var message = CreateNetworkMessage(status, MessageType.NET_UserDataUpdated);

            ThreadPool.QueueUserWorkItem(SendBroadcast, message);
        }

        //public static void SendLobbyJoinRequest(ulong APICall, SteamLobby lobby)
        //{
        //    Write($"Sending lobby request");
        //    try
        //    {
        //        var JoinRequest = new NET_LobbyJoinRequest()
        //        {
        //            LobbyID = lobby.SteamID,
        //            SteamID = (ulong)SteamEmulator.SteamID,
        //            CallbackHandle = APICall
        //        };

        //        var message = CreateNetworkMessage(JoinRequest, MessageType.NET_LobbyJoinRequest);

        //        var user = SteamFriends.Instance.GetUser(lobby.Owner);
        //        if (user == null)
        //        {
        //            Write($"Not found user to send LobbyJoinRequest, SteamID {new CSteamID(lobby.Owner).ToString()}");
        //            return;
        //        }

        //        SendTo(user.IPAddress, message);
        //    }
        //    catch (Exception ex)
        //    {
        //        Write(ex);
        //    }
        //}

        //public static void BroadcastLobbyGameServer(SteamLobby lobby)
        //{
        //    var lobbyGameserver = new NET_LobbyGameserver()
        //    {
        //        LobbyID = lobby.SteamID,
        //        SteamID = lobby.Gameserver.SteamID,
        //        IP = lobby.Gameserver.IP,
        //        Port = lobby.Gameserver.Port
        //    };

        //    var GameserverMessage = CreateNetworkMessage(lobbyGameserver, MessageType.NET_LobbyGameserver);

        //    foreach (var member in lobby.Members)
        //    {
        //        if (member.m_SteamID != (ulong)SteamEmulator.SteamID)
        //        {
        //            var user = SteamFriends.Instance.GetUser(member.m_SteamID);
        //            if (user != null)
        //            {
        //                SendTo(user.IPAddress, GameserverMessage);
        //            }
        //        }
        //    }
        //}

        //public static void RequestLobbyList(uint currentRequest)
        //{
        //    var lobbyListRequest = new NET_LobbyListRequest()
        //    {
        //        AppID = SteamEmulator.AppID,
        //        RequestID = currentRequest
        //    };

        //    var message = CreateNetworkMessage(lobbyListRequest, MessageType.NET_LobbyListRequest);

        //    SendBroadcast(message);
        //}

        //private static void BroadcastAnnounce(object state)
        //{
        //    var announce = new NET_Announce()
        //    {
        //        PersonaName = SteamEmulator.PersonaName,
        //        AccountID = (uint)SteamEmulator.SteamID.AccountID,
        //        AppID = SteamEmulator.AppID
        //    };

        //    NetworkMessage message = CreateNetworkMessage(announce, MessageType.NET_Announce);

        //    SendBroadcast(message);
        //}

        //public static void SendLobbyDataUpdate(ulong IDTarget, ulong IDLobby, ulong IDMember, SteamLobby lobby)
        //{
        //    var lobbyDataUpdate = new NET_LobbyDataUpdate()
        //    {
        //        SteamIDLobby = IDLobby,
        //        SteamIDMember = IDMember,
        //        ParsedLobby = lobby.ToJson()
        //    };

        //    var message = CreateNetworkMessage(lobbyDataUpdate, MessageType.NET_LobbyDataUpdate);

        //    var user = SteamFriends.Instance.GetUser(IDTarget);
        //    if (user == null)
        //    {
        //        return;
        //    }
        //    SendTo(user.IPAddress, message);
        //}

        //private static void AnnounceTo(string remoteAddress)
        //{
        //    var announce = new NET_Announce()
        //    {
        //        PersonaName = SteamEmulator.PersonaName,
        //        AccountID = (uint)SteamEmulator.SteamID.AccountID
        //    };

        //    var message = CreateNetworkMessage(announce, MessageType.NET_Announce);

        //    SendTo(remoteAddress, message);
        //}

        //public static void RequestAvatar(string IP)
        //{
        //    var message = CreateNetworkMessage(new NET_Base(), MessageType.NET_AvatarRequest);
        //    SendTo(IP, message);
        //}

        //public static void SendLobbyLeave(ulong owner, ulong lobbyID)
        //{
        //    var user = SteamFriends.Instance.GetUser(owner);
        //    if (user != null)
        //    {
        //        var lobbyLeave = new NET_LobbyLeave()
        //        {
        //            LobbyID = lobbyID,
        //            SteamID = (ulong)SteamEmulator.SteamID
        //        };

        //        var message = CreateNetworkMessage(lobbyLeave, MessageType.NET_LobbyLeave);

        //        SendTo(user.IPAddress, message);
        //    }
        //}

        //internal static void SendLobbyRemove(SteamLobby lobby)
        //{
        //    foreach (var member in lobby.Members)
        //    {
        //        if (member.m_SteamID != lobby.Owner)
        //        {
        //            var user = SteamFriends.Instance.GetUser(member.m_SteamID);
        //            if (user != null)
        //            {
        //                var lobbyRemove = new NET_LobbyRemove()
        //                {
        //                    LobbyID = lobby.SteamID
        //                };

        //                var message = CreateNetworkMessage(lobbyRemove, MessageType.NET_LobbyRemove);

        //                SendTo(user.IPAddress, message);
        //            }
        //        }
        //    }
        //}

        private static void SendBroadcast(object state)
        {
            NetworkMessage message = (NetworkMessage)state;

            foreach (var address in GetIPAddresses())
            {
                var Addressess = GetIPAddressRange(address);
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
            Write($"Closing connection after received {(MessageType)messageType} message");
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

        public static List<IPAddress> GetIPAddresses()
        {
            var Addresses = new List<IPAddress>();
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            IPAddress[] addressList = hostEntry.AddressList;
            foreach (IPAddress iPAddress in addressList)
            {
                if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    Addresses.Add(iPAddress);
                }
            }
            return Addresses;
        }

        public static List<string> GetLocalAddresses()
        {
            var Addresses = new List<string>();
            foreach (IPAddress iPAddress in GetIPAddresses())
            {
                if (!Addresses.Contains(iPAddress.ToString()))
                {
                    Addresses.Add(iPAddress.ToString());
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
            Write($"Checking Port {port}");

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

        private class SocketData
        {
            public Socket socket { get; set; }
            public NetworkMessage Message { get; set; }
        }
    }
}
