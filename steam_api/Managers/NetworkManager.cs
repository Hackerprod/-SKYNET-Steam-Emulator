using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Helpers.JSON;
using SKYNET.Network;
using SKYNET.Network.Packets;
using SKYNET.Steamworks;
using SKYNET.Steamworks.Implementation;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static SKYNET.Steamworks.Implementation.SteamMatchmaking;

namespace SKYNET.Managers
{
    public class NetworkManager
    {
        public static int Port = 28880;
        public static TCPServer TCPServer;

        public static void Initialize()
        {
            Port = SteamEmulator.BroadCastPort;
            Write($"Initializing TCP server on port {Port}");

            TCPServer = new TCPServer(Port);
            TCPServer.OnDataReceived += TCPServer_OnDataReceived;
            TCPServer.OnConnected += TCPServer_OnConnected;

            ThreadPool.QueueUserWorkItem(TCPServer.Start);
            ThreadPool.QueueUserWorkItem(BroadcastAnnounce);
        }

        private static void TCPServer_OnConnected(object sender, ClientSocket e)
        {
            Write("Client connected from " + e.RemoteEndPoint);
        }

        private static void TCPServer_OnDataReceived(object sender, NetPacket packet)
        {
            NetworkMessage message = packet.Data.GetString().FromJson<NetworkMessage>();
            ProcessMessage(message, packet.Sender.socket);
        }

        #region Message processors

        private static void ProcessMessage(NetworkMessage message, Socket socket)
        {
            Write($"Received message {(MessageType)message.MessageType} from {((IPEndPoint)socket.RemoteEndPoint).Address.ToString()}");
            //CheckInUserList(socket);
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
                case MessageType.NET_LobbyMetaDataUpdate:
                    ProcessLobbyMetaDataUpdate(message, socket);
                    break;
                case MessageType.NET_LobbyChatUpdate:
                    ProcessLobbyChatUpdate(message, socket);
                    break;
                case MessageType.NET_LobbyLeave:
                    ProcessLobbyLeave(message, socket);
                    break;
                case MessageType.NET_LobbyGameserver:
                    ProcessLobbyGameserver(message, socket);
                    break;

                default:
                    break;
            }

        }

        private static void CheckInUserList(Socket socket)
        {
            var IPAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
            if (IslocalAddress(IPAddress))
            {
                return;
            }
            var user = UserManager.GetUserByAddress(IPAddress);
            if (user == null)
            {
                AnnounceTo(IPAddress);
            }
        }

        private static void ProcessLobbyGameserver(NetworkMessage message, Socket socket)
        {
            NET_LobbyGameserver lobbyGameserver = message.ParsedBody.FromJson<NET_LobbyGameserver>();
            if (lobbyGameserver != null)
            {
                if (SteamMatchmaking.Instance.GetLobby(lobbyGameserver.LobbyID, out var lobby))
                {
                    lobby.Gameserver.SteamID = lobbyGameserver.SteamID;
                    lobby.Gameserver.IP = lobbyGameserver.IP;
                    lobby.Gameserver.Port = lobbyGameserver.Port;
                    lobby.Gameserver.Filled = true;
                }
            }
            CloseSocket(socket, (MessageType)message.MessageType);
        }

        private static void ProcessLobbyLeave(NetworkMessage message, Socket socket)
        {
            NET_LobbyLeave lobbyLeave = message.ParsedBody.FromJson<NET_LobbyLeave>();
            if (lobbyLeave != null)
            {
                if (SteamMatchmaking.Instance.GetLobby(lobbyLeave.LobbyID, out var lobby))
                {
                    var lobbyChatUpdate = new NET_LobbyChatUpdate()
                    {
                        SteamIDLobby = lobby.SteamID,
                        SteamIDUserChanged = lobbyLeave.SteamID,
                        SteamIDMakingChange = lobbyLeave.SteamID,
                        ChatMemberStateChange = (int)EChatMemberStateChange.k_EChatMemberStateChangeLeft
                    };

                    lobby.Members.RemoveAll(m => m.m_SteamID == lobbyLeave.SteamID);

                    var ChatUpdateMessage = CreateNetworkMessage(lobbyChatUpdate, MessageType.NET_LobbyChatUpdate);

                    foreach (var member in lobby.Members)
                    {
                        var user = SteamEmulator.SteamFriends.GetUser(member.m_SteamID);
                        if (user != null)
                        {
                            SendTo(user.IPAddress, ChatUpdateMessage);
                            //SteamMatchmaking.Instance.LobbyDataUpdated();
                        }
                    }

                    LobbyChatUpdate_t data = new LobbyChatUpdate_t()
                    {
                        m_ulSteamIDLobby = lobbyLeave.LobbyID,
                        m_ulSteamIDUserChanged = lobbyLeave.SteamID,
                        m_ulSteamIDMakingChange = lobbyLeave.SteamID,
                        m_rgfChatMemberStateChange = (int)EChatMemberStateChange.k_EChatMemberStateChangeLeft
                    };
                    CallbackManager.AddCallback(data);
                }
            }

            CloseSocket(socket, (MessageType)message.MessageType);
        }

        private static void ProcessLobbyChatUpdate(NetworkMessage message, Socket socket)
        {
            NET_LobbyChatUpdate lobbyChatUpdate = message.ParsedBody.FromJson<NET_LobbyChatUpdate>();
            if (lobbyChatUpdate != null)
            {
                SteamMatchmaking.Instance.LobbyChatUpdated(lobbyChatUpdate);
            }
        }

        private static void ProcessLobbyDataUpdate(NetworkMessage message, Socket socket)
        {
            NET_LobbyDataUpdate lobbyDataUpdate = message.ParsedBody.FromJson<NET_LobbyDataUpdate>();
            if (lobbyDataUpdate != null)
            {
                SteamMatchmaking.Instance.LobbyDataUpdated(lobbyDataUpdate);
            }
        }

        private static void ProcessLobbyMetaDataUpdate(NetworkMessage message, Socket socket)
        {
            /*
            NET_LobbyMetaDataUpdate lobbyMetaDataUpdate = message.ParsedBody.FromJson<NET_LobbyMetaDataUpdate>();
            if (lobbyMetaDataUpdate != null)
            {
                SteamMatchmaking.Instance.LobbyMetaDataUpdated(lobbyMetaDataUpdate);
            }
            */
        }

        private static void ProcessLobbyJoinRequest(NetworkMessage message, Socket socket)
        {
            var lobbyJoinRequest = message.ParsedBody.FromJson<NET_LobbyJoinRequest>();
            var lobbyJoinResponse = new NET_LobbyJoinResponse()
            {
                CallbackHandle = lobbyJoinRequest.CallbackHandle,
                ChatRoomEnterResponse = (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess,
            };
            if (lobbyJoinRequest != null)
            {
                if (SteamMatchmaking.Instance.GetLobby(lobbyJoinRequest.LobbyID, out var lobby))
                {
                    if (lobby.MaxMembers >= lobby.Members.Count)
                    {
                        lobbyJoinResponse.ChatRoomEnterResponse = (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseFull;
                    }

                    var Member = lobby.Members.Find(m => m.m_SteamID == lobbyJoinRequest.SteamID);
                    if (Member == null)
                    {
                        lobby.Members.Add(new SteamLobby.LobbyMember()
                        {
                            m_SteamID = lobbyJoinRequest.SteamID
                        });
                    }

                    string serialized = lobby.ToJson();

                    lobbyJoinResponse.SerializedLobby = serialized;

                    var lobbyJoinMessage = CreateNetworkMessage(lobbyJoinResponse, MessageType.NET_LobbyJoinResponse);

                    try
                    {
                        string json = lobbyJoinMessage.ToJson();
                        socket.Send(json.GetBytes());
                    }
                    catch
                    {
                    }

                    // LobbyDataUpdate
                    var lobbyDataUpdate = new NET_LobbyDataUpdate()
                    {
                        SteamIDLobby = lobby.SteamID,
                        SteamIDMember = lobbyJoinRequest.SteamID,
                        Success = true
                    };

                    var lobbyChatUpdate = new NET_LobbyChatUpdate()
                    {
                        SteamIDLobby = lobby.SteamID,
                        SteamIDUserChanged = lobbyJoinRequest.SteamID,
                        SteamIDMakingChange = lobbyJoinRequest.SteamID,
                        ChatMemberStateChange = (int)EChatMemberStateChange.k_EChatMemberStateChangeEntered
                    };

                    var DataUpdateMessage = CreateNetworkMessage(lobbyDataUpdate, MessageType.NET_LobbyDataUpdate);
                    var ChatUpdateMessage = CreateNetworkMessage(lobbyChatUpdate, MessageType.NET_LobbyChatUpdate);

                    foreach (var member in lobby.Members)
                    {
                        var user = SteamEmulator.SteamFriends.GetUser(member.m_SteamID);
                        if (user != null)
                        {
                            SendTo(user.IPAddress, DataUpdateMessage);
                            SendTo(user.IPAddress, ChatUpdateMessage);
                        }
                    }
                }
            }
        }

        private static void ProcessLobbyJoinResponse(NetworkMessage message, Socket socket)
        {
            /*
            try
            {
                NET_LobbyJoinResponse lobbyJoinResponse = message.ParsedBody.FromJson<NET_LobbyJoinResponse>();
                if (lobbyJoinResponse != null)
                {
                    var lobby = lobbyJoinResponse.SerializedLobby.FromJson<SteamMatchmaking.SteamLobby>();
                    if (lobby != null)
                    {
                        SteamMatchmaking.Instance.JoinResponse(lobbyJoinResponse, lobby);
                    }
                    else
                    {
                        SteamMatchmaking.Instance.JoinResponse(lobbyJoinResponse, lobby);
                    }
                }
            }
            catch (Exception ex)
            {
                Write(ex);
            }

            CloseSocket(socket, (MessageType)message.MessageType);
            */
        }

        private static void ProcessLobbyListRequest(NetworkMessage message, Socket socket)
        {
            /*
            try
            {
                NET_LobbyListRequest lobbyListRequest = message.ParsedBody.FromJson<NET_LobbyListRequest>();
                if (lobbyListRequest != null)
                {
                    if (lobbyListRequest.RequestID == SteamMatchmaking.Instance.CurrentRequest)
                    {
                        CloseSocket(socket, (MessageType)message.MessageType);
                        return;
                    }
                    if (lobbyListRequest.AppID != SteamEmulator.AppID)
                    {
                        CloseSocket(socket, (MessageType)message.MessageType);
                        return;
                    }
                }
                var lobby = SteamMatchmaking.Instance.GetLobbyByOwner((ulong)SteamEmulator.SteamID);
                if (lobby == null)
                {
                    CloseSocket(socket, (MessageType)message.MessageType);
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
            */
        }

        private static void ProcessLobbyListResponse(NetworkMessage message, Socket socket)
        {
            /*
            try
            {
                NET_LobbyListResponse lobbyListResponse = message.ParsedBody.FromJson<NET_LobbyListResponse>();
                var lobby = lobbyListResponse.SerializedLobby.FromJson<Steamworks.Implementation.SteamMatchmaking.SteamLobby>();
                if (lobby != null)
                {
                    string remoteAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
                    if (!lobby.LobbyData.ContainsKey("publicIP"))
                    {
                        lobby.LobbyData["publicIP"] = remoteAddress;
                    }
                    if (!lobby.LobbyData.ContainsKey("internalIP"))
                    {
                        lobby.LobbyData["internalIP"] = remoteAddress;
                    }

                    Write($"Adding lobby {lobby.SteamID} from {remoteAddress}");
                    SteamMatchmaking.Instance.Lobbies.TryAdd(lobby.SteamID, lobby);

                    var user = SteamEmulator.SteamFriends.GetUser(lobby.Owner);
                    if (user == null)
                    {
                        AnnounceTo(remoteAddress);
                    }
                }
            }
            catch (Exception ex)
            {
                Write(ex);
            }

            CloseSocket(socket, (MessageType)message.MessageType);
            */
        }

        private static void ProcessAnnounce(NetworkMessage message, Socket socket)
        {
            try
            {
                if (IslocalAddress(((IPEndPoint)socket.RemoteEndPoint).Address))
                {
                    Write("Returning because is local address");
                    CloseSocket(socket, (MessageType)message.MessageType);
                    return;
                }

                NET_Announce announce = message.ParsedBody.FromJson<NET_Announce>();
                string IPAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();

                // Add User to List on both cases (NET_Announce and NET_AnnounceResponse)
                if (announce != null && announce.AccountID != SteamEmulator.SteamID)
                    SteamEmulator.SteamFriends?.AddOrUpdateUser(announce.AccountID, announce.PersonaName, announce.AppID, IPAddress);

                if (message.MessageType == (int)MessageType.NET_Announce)
                {
                    NET_Announce announceResponse = new NET_Announce()
                    {
                        PersonaName = SteamEmulator.PersonaName,
                        AccountID = (uint)SteamEmulator.SteamID
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
                    CloseSocket(socket, (MessageType)message.MessageType);
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
                    var imageBytes = SteamEmulator.SteamFriends.GetAvatar((ulong)SteamEmulator.SteamID);
                    string hexAvatar = Convert.ToBase64String(imageBytes);
                    NET_AvatarResponse avatarResponse = new NET_AvatarResponse()
                    {
                        AccountID = (uint)SteamEmulator.SteamID,
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
                }
                catch (Exception ex)
                {
                    Write($"{ex}");
                }
                CloseSocket(socket, (MessageType)message.MessageType);
            }
        }

        private static void ProcessUserStatusChanged(NetworkMessage message, Socket socket)
        {
            try
            {
                NET_UserDataUpdated StatusChanged = message.ParsedBody.FromJson<NET_UserDataUpdated>();

                if (StatusChanged != null)
                {
                    if (StatusChanged.AccountID == (uint)SteamEmulator.SteamID.AccountID) return;
                    var IPAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
                    SteamEmulator.SteamFriends.UpdateUserStatus(StatusChanged, IPAddress);
                }
            }
            catch
            {

            }

            CloseSocket(socket, (MessageType)message.MessageType);
        }

        #endregion

        public static void BroadcastStatusUpdated(Types.SteamUser user)
        {
            NET_UserDataUpdated status = new NET_UserDataUpdated()
            {
                PersonaName = user.PersonaName,
                AccountID = (uint)SteamEmulator.SteamID.AccountID,
                LobbyID = user.LobbyID.GetAccountID()
            };

            NetworkMessage message = new NetworkMessage()
            {
                MessageType = (int)MessageType.NET_UserDataUpdated,
                ParsedBody = status.ToJson()
            };

            ThreadPool.QueueUserWorkItem(SendBroadcast, message);
        }

        public static void SendLobbyJoinRequest(ulong APICall, SteamLobby lobby)
        {
            Write($"Sending lobby request");
            try
            {
                NET_LobbyJoinRequest JoinRequest = new NET_LobbyJoinRequest()
                {
                    LobbyID = lobby.SteamID,
                    SteamID = (ulong)SteamEmulator.SteamID,
                    CallbackHandle = APICall
                };

                NetworkMessage message = new NetworkMessage()
                {
                    MessageType = (int)MessageType.NET_LobbyJoinRequest,
                    ParsedBody = JoinRequest.ToJson()
                };

                var user = SteamEmulator.SteamFriends.GetUser(lobby.Owner);
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

        public static void BroadcastLobbyMetaData(SteamLobby lobby, string pchKey, string pchValue)
        {
            NET_LobbyMetaDataUpdate dataUpdate = new NET_LobbyMetaDataUpdate()
            {
                LobbyID = lobby.SteamID,
                Key = pchKey,
                Value = pchValue
            };

            NetworkMessage message = new NetworkMessage()
            {
                MessageType = (int)MessageType.NET_LobbyMetaDataUpdate,
                ParsedBody = dataUpdate.ToJson()
            };

            foreach (var member in lobby.Members)
            {
                if (member.m_SteamID != (ulong)SteamEmulator.SteamID)
                {
                    var user = SteamEmulator.SteamFriends.GetUser(member.m_SteamID);
                    if (user != null)
                    {
                        SendTo(user.IPAddress, message);
                    }
                }
            }
        }

        public static void BroadcastLobbyGameServer(SteamLobby lobby)
        {
            var lobbyGameserver = new NET_LobbyGameserver()
            {
                LobbyID = lobby.SteamID,
                SteamID = lobby.Gameserver.SteamID,
                IP = lobby.Gameserver.IP,
                Port = lobby.Gameserver.Port
            };

            var GameserverMessage = CreateNetworkMessage(lobbyGameserver, MessageType.NET_LobbyGameserver);

            foreach (var member in lobby.Members)
            {
                if (member.m_SteamID != (ulong)SteamEmulator.SteamID)
                {
                    var user = SteamEmulator.SteamFriends.GetUser(member.m_SteamID);
                    if (user != null)
                    {
                        SendTo(user.IPAddress, GameserverMessage);
                    }
                }
            }
        }

        public static void RequestLobbyList(uint currentRequest)
        {
            NET_LobbyListRequest lobbyListRequest = new NET_LobbyListRequest()
            {
                AppID = SteamEmulator.AppID,
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
                AccountID = (uint)SteamEmulator.SteamID.AccountID,
                AppID = SteamEmulator.AppID
            };

            NetworkMessage message = CreateNetworkMessage(announce, MessageType.NET_Announce);

            SendBroadcast(message);
        }

        public static void SendLobbyDataUpdate(ulong SteamID, LobbyDataUpdate_t lobby)
        {
            NET_LobbyDataUpdate lobbyDataUpdate = new NET_LobbyDataUpdate()
            {
                SteamIDLobby = lobby.m_ulSteamIDLobby,
                SteamIDMember = lobby.m_ulSteamIDMember,
                Success = lobby.m_bSuccess
            };

            NetworkMessage message = new NetworkMessage()
            {
                MessageType = (int)MessageType.NET_LobbyChatUpdate,
                ParsedBody = lobbyDataUpdate.ToJson()
            };

            var user = SteamEmulator.SteamFriends.GetUser(SteamID);
            if (user == null)
            {
                return;
            }
            SendTo(user.IPAddress, message);
        }

        private static void AnnounceTo(string remoteAddress)
        {
            NET_Announce announce = new NET_Announce()
            {
                PersonaName = SteamEmulator.PersonaName,
                AccountID = (uint)SteamEmulator.SteamID.AccountID
            };

            NetworkMessage message = CreateNetworkMessage(announce, MessageType.NET_Announce);

            SendTo(remoteAddress, message);
        }

        public static void RequestAvatar(string IP)
        {
            NetworkMessage message = new NetworkMessage()
            {
                MessageType = (int)MessageType.NET_AvatarRequest,
                ParsedBody = ""
            };

            SendTo(IP, message);
        }

        public static void SendLobbyLeave(ulong owner, ulong lobbyID)
        {
            var user = SteamEmulator.SteamFriends.GetUser(owner);
            if (user != null)
            {
                NET_LobbyLeave lobbyLeave = new NET_LobbyLeave()
                {
                    LobbyID = lobbyID,
                    SteamID = (ulong)SteamEmulator.SteamID
                };

                NetworkMessage message = new NetworkMessage()
                {
                    MessageType = (int)MessageType.NET_LobbyLeave,
                    ParsedBody = lobbyLeave.ToJson()
                };

                SendTo(user.IPAddress, message);
            }
        }

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

                ClientSocket client = new ClientSocket(SocketData.socket);
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
            NetworkMessage message = new NetworkMessage()
            {
                MessageType = (int)type,
                ParsedBody = Base.ToJson()
            };
            return message;
        }

        private static void CloseSocket(Socket socket, MessageType messageType)
        {
            Write($"Closing connection after received {messageType} message");
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
            SteamEmulator.Write("NetworkManager", msg);
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

        public static void SendP2PTo(ulong steamIDRemote, byte[] bytes, int eP2PSendType, int nChannel)
        {
            try
            {
                NET_P2PPacket item = new NET_P2PPacket
                {
                    AccountID = steamIDRemote.GetAccountID(),
                    Buffer = Convert.ToBase64String(bytes),
                    IDRemote = SteamEmulator.SteamID.AccountID,
                    P2PSendType = eP2PSendType,
                    Channel = nChannel
                };
                NetworkMessage networkMessage = new NetworkMessage
                {
                    MessageType = 5,
                    ParsedBody = item.ToJson()
                };
                SKYNET.Types.SteamUser user = SteamEmulator.SteamFriends.GetUser2(steamIDRemote);
                if (user == null)
                {
                    ThreadPool.QueueUserWorkItem(SendBroadcast, networkMessage);
                }
                if (IPAddress.TryParse(user.IPAddress, out var _))
                {
                    try
                    {
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        SocketData state = new SocketData
                        {
                            socket = socket,
                            Message = networkMessage
                        };
                        socket.BeginConnect(user.IPAddress, 28880, ConnectionCallback, state);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Write(ex.Message + " " + ex.StackTrace);
            }
        }

        public static uint GetIPAddress(IPAddress ipAddr)
        {
            byte[] addressBytes = ipAddr.GetAddressBytes();
            Array.Reverse(addressBytes);
            return BitConverter.ToUInt32(addressBytes, 0);
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

        public static uint ConvertIPAddress(IPAddress ipAddr)
        {
            byte[] addressBytes = ipAddr.GetAddressBytes();
            Array.Reverse(addressBytes);
            return BitConverter.ToUInt32(addressBytes, 0);
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

        private class SocketData
        {
            public Socket socket { get; set; }
            public NetworkMessage Message { get; set; }
        }
    }
}
