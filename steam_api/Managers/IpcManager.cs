using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Helper.JSON;
using SKYNET.IPC;
using SKYNET.IPC.Types;
using SKYNET.Steamworks;
using SKYNET.Steamworks.Implementation;
using SKYNET.Types;
using System;
using System.Drawing;
using System.Threading;
using static SKYNET.Steamworks.Implementation.SteamMatchmaking;

namespace SKYNET.Managers
{
    public class IPCManager
    {
        private const ulong IPC_ToServer  = 0;
        private const ulong IPC_Broadcast = 1;
        private static PipeClient<IPCMessage> IPCClient;

        static IPCManager()
        {
            IPCClient = new PipeClient<IPCMessage>("SKYNET");
            IPCClient.AutoReconnect = true;
            IPCClient.Connected += IPCClient_Connected;
            IPCClient.Disconnected += IPCClient_Disconnected;
            IPCClient.MessageReceived += IPCClient_MessageReceived;
        }

        public static void Initialize()
        {
            ThreadPool.QueueUserWorkItem(Initialize); 
        }

        private async static void Initialize(object state)
        {
            await IPCClient.ConnectAsync();
        }

        private static void IPCClient_Connected(object sender, ConnectionEventArgs<IPCMessage> e)
        {
            Write("IPCClient Connected");
        }

        internal static void GCRequest(uint MsgType, byte[] bytes)
        {
            var IPC_GCRequest = new IPC_GCMessageRequest()
            {
                MsgType = MsgType,
                Buffer = bytes
            };
            //var message = CreateIPCMessage(IPC_GCRequest, IPCMessageType.IPC_GCMessageRequest);
            //SendTo(IPC_ToServer, message);
        }

        private static void IPCClient_Disconnected(object sender, ConnectionEventArgs<IPCMessage> e)
        {
            Write("IPCClient Disconnected");
        }

        private static void IPCClient_MessageReceived(object sender, ConnectionMessageEventArgs<IPCMessage> message)
        {
            Write("IPCClient MessageReceived");
            ProcessMessage(message.Message);
        }

        #region Message processors

        private static void ProcessMessage(IPCMessage message)
        {
            Write($"Received message {(IPCMessageType)message.MessageType}");
            switch ((IPCMessageType)message.MessageType)
            {
                case IPCMessageType.IPC_User:
                    ProcessUser(message);
                    break;
                case IPCMessageType.IPC_AddOrUpdateUser:
                    ProcessAddOrUpdateUser(message);
                    break;
                case IPCMessageType.IPC_AvatarResponse:
                    ProcessAvatarResponse(message);
                    break;
                case IPCMessageType.IPC_UserDataUpdated:
                    ProcessUserStatusChanged(message);
                    break;
                case IPCMessageType.IPC_LobbyListRequest:
                    ProcessLobbyListRequest(message);
                    break;
                case IPCMessageType.IPC_LobbyListResponse:
                    ProcessLobbyListResponse(message);
                    break;
                case IPCMessageType.IPC_LobbyJoinRequest:
                    ProcessLobbyJoinRequest(message);
                    break;
                case IPCMessageType.IPC_LobbyJoinResponse:
                    ProcessLobbyJoinResponse(message);
                    break;
                case IPCMessageType.IPC_LobbyDataUpdate:
                    ProcessLobbyDataUpdate(message);
                    break;
                case IPCMessageType.IPC_LobbyChatUpdate:
                    ProcessLobbyChatUpdate(message);
                    break;
                case IPCMessageType.IPC_LobbyLeave:
                    ProcessLobbyLeave(message);
                    break;
                case IPCMessageType.IPC_LobbyRemove:
                    ProcessLobbyRemove(message);
                    break;
                case IPCMessageType.IPC_LobbyGameserver:
                    ProcessLobbyGameserver(message);
                    break;

                default:
                    break;
            }

        }

        private static void ProcessAddOrUpdateUser(IPCMessage message)
        {
            try
            {
                var user = message.ParsedBody.FromJson<IPC_AddOrUpdateUser>();
                SteamFriends.Instance?.AddOrUpdateUser(user.AccountID, user.PersonaName, user.AppID, user.IPAddress);
            }
            catch
            {
            }
        }

        internal static void SendP2PTo(ulong steamIDRemote, IPC_P2PPacket packet)
        {
            var message = CreateIPCMessage(packet, IPCMessageType.IPC_P2PPacket);
            SendTo(steamIDRemote, message);
        }

        private static void ProcessLobbyGameserver(IPCMessage message)
        {
            var lobbyGameserver = message.ParsedBody.FromJson<IPC_LobbyGameserver>();
            if (lobbyGameserver != null)
            {
                if (SteamMatchmaking.Instance.GetLobby(lobbyGameserver.LobbyID, out var lobby))
                {
                    Write($"Received gameserver data for lobby {lobbyGameserver.LobbyID}, IP = {lobbyGameserver.IP}, Port = {lobbyGameserver.Port}");
                    lobby.Gameserver.SteamID = lobbyGameserver.SteamID;
                    lobby.Gameserver.IP = lobbyGameserver.IP;
                    lobby.Gameserver.Port = lobbyGameserver.Port;
                    lobby.Gameserver.Filled = true;

                    // TODO: Necessary?
                    GameServerChangeRequested_t data = new GameServerChangeRequested_t()
                    {
                        m_rgchServer = $"{lobbyGameserver.IP}:{lobbyGameserver.Port}"
                    };
                    CallbackManager.AddCallbackResult(data);
                }
            }
        }

        private static void ProcessLobbyLeave(IPCMessage message)
        {
            var lobbyLeave = message.ParsedBody.FromJson<IPC_LobbyLeave>();
            if (lobbyLeave != null)
            {
                if (SteamMatchmaking.Instance.GetLobby(lobbyLeave.LobbyID, out var lobby))
                {
                    var lobbyChatUpdate = new IPC_LobbyChatUpdate()
                    {
                        SteamIDLobby = lobby.SteamID,
                        SteamIDUserChanged = lobbyLeave.SteamID,
                        SteamIDMakingChange = lobbyLeave.SteamID,
                        ChatMemberStateChange = (int)EChatMemberStateChange.k_EChatMemberStateChangeLeft
                    };

                    lobby.Members.RemoveAll(m => m.m_SteamID == lobbyLeave.SteamID);

                    var ChatUpdateMessage = CreateIPCMessage(lobbyChatUpdate, IPCMessageType.IPC_LobbyChatUpdate);

                    foreach (var member in lobby.Members)
                    {
                        var user = SteamFriends.Instance.GetUser(member.m_SteamID);
                        if (user != null)
                        {
                            SendTo(user.SteamID, ChatUpdateMessage);
                            //SteamMatchmaking.Instance.LobbyDataUpdated();
                        }
                    }

                    var data = new LobbyChatUpdate_t()
                    {
                        m_ulSteamIDLobby = lobbyLeave.LobbyID,
                        m_ulSteamIDUserChanged = lobbyLeave.SteamID,
                        m_ulSteamIDMakingChange = lobbyLeave.SteamID,
                        m_rgfChatMemberStateChange = (int)EChatMemberStateChange.k_EChatMemberStateChangeLeft
                    };
                    CallbackManager.AddCallback(data);
                }
            }
        }

        internal static void SendCreateLobby(SteamLobby lobby)
        {
            IPC_CreateLobby CreateLobby = new IPC_CreateLobby()
            {
                SerializedLobby = lobby.ToJson()
            };
            var message = CreateIPCMessage(CreateLobby, IPCMessageType.IPC_LobbyCreate);
            SendTo(IPC_ToServer, message);
        }

        private static void ProcessLobbyRemove(IPCMessage message)
        {
            var lobbyRemove = message.ParsedBody.FromJson<IPC_LobbyRemove>();
            if (lobbyRemove != null)
            {
                SteamMatchmaking.Instance.RemoveLobby(lobbyRemove.LobbyID);
            }
        }

        private static void ProcessLobbyChatUpdate(IPCMessage message)
        {
            var lobbyChatUpdate = message.ParsedBody.FromJson<IPC_LobbyChatUpdate>();
            if (lobbyChatUpdate != null)
            {
                SteamMatchmaking.Instance.LobbyChatUpdated(lobbyChatUpdate);
            }
        }

        private static void ProcessLobbyDataUpdate(IPCMessage message)
        {
            var lobbyDataUpdate = message.ParsedBody.FromJson<IPC_LobbyDataUpdate>();
            if (lobbyDataUpdate != null)
            {
                SteamMatchmaking.Instance.LobbyDataUpdated(lobbyDataUpdate);
            }
        }

        private static void ProcessLobbyJoinRequest(IPCMessage message)
        {
            var lobbyJoinRequest = message.ParsedBody.FromJson<IPC_LobbyJoinRequest>();
            var lobbyJoinResponse = new IPC_LobbyJoinResponse()
            {
                CallbackHandle = lobbyJoinRequest.CallbackHandle,
                ChatRoomEnterResponse = (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess,
            };
            if (lobbyJoinRequest != null)
            {
                if (SteamMatchmaking.Instance.GetLobby(lobbyJoinRequest.LobbyID, out var lobby))
                {
                    if (lobby.Members.Count >= lobby.MaxMembers)
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

                    var lobbyJoinMessage = CreateIPCMessage(lobbyJoinResponse, IPCMessageType.IPC_LobbyJoinResponse);

                    try
                    {
                        SendTo(lobbyJoinRequest.SteamID, lobbyJoinMessage);
                    }
                    catch
                    {
                    }

                    // LobbyDataUpdate
                    var lobbyDataUpdate = new IPC_LobbyDataUpdate()
                    {
                        SteamIDLobby = lobby.SteamID,
                        SteamIDMember = lobbyJoinRequest.SteamID,
                        SerializedLobby = serialized
                    };

                    var lobbyChatUpdate = new IPC_LobbyChatUpdate()
                    {
                        SteamIDLobby = lobby.SteamID,
                        SteamIDUserChanged = lobbyJoinRequest.SteamID,
                        SteamIDMakingChange = lobbyJoinRequest.SteamID,
                        ChatMemberStateChange = (int)EChatMemberStateChange.k_EChatMemberStateChangeEntered
                    };

                    var DataUpdateMessage = CreateIPCMessage(lobbyDataUpdate, IPCMessageType.IPC_LobbyDataUpdate);
                    var ChatUpdateMessage = CreateIPCMessage(lobbyChatUpdate, IPCMessageType.IPC_LobbyChatUpdate);

                    foreach (var member in lobby.Members)
                    {
                        var user = SteamFriends.Instance.GetUser(member.m_SteamID);
                        if (user != null)
                        {
                            SendTo(user.SteamID, DataUpdateMessage);
                            SendTo(user.SteamID, ChatUpdateMessage);
                        }
                    }
                }
            }
        }

        private static void ProcessLobbyJoinResponse(IPCMessage message)
        {
            try
            {
                var lobbyJoinResponse = message.ParsedBody.FromJson<IPC_LobbyJoinResponse>();
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
        }

        private static void ProcessLobbyListRequest(IPCMessage message)
        {
            //try
            //{
            //    var lobbyListRequest = message.ParsedBody.FromJson<IPC_LobbyListRequest>();
            //    if (lobbyListRequest != null)
            //    {
            //        //if (lobbyListRequest.RequestID == SteamMatchmaking.Instance.CurrentRequest)
            //        //{

            //        //    return;
            //        //}
            //        if (lobbyListRequest.AppID != 0 && lobbyListRequest.AppID != SteamEmulator.AppID)
            //        {
            //            return;
            //        }
            //    }
            //    var lobby = SteamMatchmaking.Instance.GetLobbyByOwner((ulong)SteamEmulator.SteamID);
            //    if (lobby == null)
            //    {

            //    }
            //    else
            //    {
            //        string serialized = lobby.ToJson();

            //        var lobbyListResponse = new IPC_LobbyListResponse()
            //        {
            //            SerializedLobby = serialized
            //        };

            //        var messageResponse = CreateNetworkMessage(lobbyListResponse, IPCMessageType.IPC_LobbyListResponse);

            //        SendTo(, messageResponse);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Write(ex);
            //}
        }

        private static void ProcessLobbyListResponse(IPCMessage message)
        {
            try
            {
                var lobbyListResponse = message.ParsedBody.FromJson<IPC_LobbyListResponse>();
                var lobby = lobbyListResponse.SerializedLobby.FromJson<SteamMatchmaking.SteamLobby>();
                if (lobby != null)
                {
                    Write($"Adding lobby {lobby.SteamID}");
                    SteamMatchmaking.Instance.Lobbies.TryAdd(lobby.SteamID, lobby);
                }
            }
            catch (Exception ex)
            {
                Write(ex);
            }

        }

        private static void ProcessUser(IPCMessage message)
        {
            try
            {
                var user = message.ParsedBody.FromJson<IPC_User>();
                SteamEmulator.PersonaName = user.PersonaName;
                SteamEmulator.SteamID = new CSteamID(user.AccountID);
                SteamEmulator.SteamID_GS = new CSteamID(user.GameServerID);
                SteamEmulator.AppID = user.AppID;
            }
            catch
            {

            }
        }

        private static void ProcessAvatarResponse(IPCMessage message)
        {
            try
            {
                var announceResponse = message.ParsedBody.FromJson<IPC_AvatarResponse>();
                if (announceResponse != null)
                {
                    var imageBytes = Convert.FromBase64String(announceResponse.HexAvatar);
                    if (imageBytes.Length != 0)
                    {
                        Bitmap Avatar = (Bitmap)ImageHelper.ImageFromBytes(imageBytes);
                        ulong SteamID = (ulong)new CSteamID(announceResponse.AccountID);
                        SteamFriends.Instance.AddOrUpdateAvatar(Avatar, SteamID);
                        SteamRemoteStorage.Instance.StoreAvatar(Avatar, announceResponse.AccountID);
                    }
                }
            }
            catch (Exception ex)
            {
                Write($"{ex}");
            }
        }

        private static void ProcessUserStatusChanged(IPCMessage message)
        {
            try
            {
                var StatusChanged = message.ParsedBody.FromJson<IPC_UserDataUpdated>();
                if (StatusChanged != null)
                {
                    if (StatusChanged.AccountID == (uint)SteamEmulator.SteamID.AccountID) return;
                    SteamFriends.Instance.UpdateUserStatus(StatusChanged);
                }
            }
            catch
            {

            }
        }

        #endregion

        public static void BroadcastStatusUpdated(SteamPlayer user)
        {
            //var status = new IPC_UserDataUpdated()
            //{
            //    PersonaName = user.PersonaName,
            //    AccountID = (uint)SteamEmulator.SteamID.AccountID,
            //    LobbyID = user.LobbyID.GetAccountID()
            //};

            //var message = CreateNetworkMessage(status, IPCMessageType.IPC_UserDataUpdated);

            //ThreadPool.QueueUserWorkItem(SendBroadcast, message);
        }

        public static void SendLobbyJoinRequest(ulong APICall, SteamLobby lobby)
        {
            Write($"Sending lobby request");
            try
            {
                var JoinRequest = new IPC_LobbyJoinRequest()
                {
                    LobbyID = lobby.SteamID,
                    SteamID = (ulong)SteamEmulator.SteamID,
                    CallbackHandle = APICall
                };

                var message = CreateIPCMessage(JoinRequest, IPCMessageType.IPC_LobbyJoinRequest);

                var user = SteamFriends.Instance.GetUser(lobby.Owner);
                if (user == null)
                {
                    Write($"Not found user to send LobbyJoinRequest, SteamID {new CSteamID(lobby.Owner).ToString()}");
                    return;
                }

                SendTo(user.SteamID, message);
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        public static void BroadcastLobbyGameServer(SteamLobby lobby)
        {
            var lobbyGameserver = new IPC_LobbyGameserver()
            {
                LobbyID = lobby.SteamID,
                SteamID = lobby.Gameserver.SteamID,
                IP = lobby.Gameserver.IP,
                Port = lobby.Gameserver.Port
            };

            var GameserverMessage = CreateIPCMessage(lobbyGameserver, IPCMessageType.IPC_LobbyGameserver);

            foreach (var member in lobby.Members)
            {
                if (member.m_SteamID != (ulong)SteamEmulator.SteamID)
                {
                    var user = SteamFriends.Instance.GetUser(member.m_SteamID);
                    if (user != null)
                    {
                        SendTo(user.SteamID, GameserverMessage);
                    }
                }
            }
        }

        public static void RequestLobbyList(uint currentRequest)
        {
            var lobbyListRequest = new IPC_LobbyListRequest()
            {
                AppID = SteamEmulator.AppID,
                RequestID = currentRequest
            };

            var message = CreateIPCMessage(lobbyListRequest, IPCMessageType.IPC_LobbyListRequest);

            SendTo(IPC_Broadcast, message);
        }

        public static void SendLobbyDataUpdate(ulong IDTarget, ulong IDLobby, ulong IDMember, SteamLobby lobby)
        {
            var lobbyDataUpdate = new IPC_LobbyDataUpdate()
            {
                SteamIDLobby = IDLobby,
                SteamIDMember = IDMember,
                SerializedLobby = lobby.ToJson()
            };

            var message = CreateIPCMessage(lobbyDataUpdate, IPCMessageType.IPC_LobbyDataUpdate);

            var user = SteamFriends.Instance.GetUser(IDTarget);
            if (user == null)
            {
                return;
            }
            SendTo(user.SteamID, message);
        }

        public static void RequestAvatar(ulong SteamID)
        {
            var message = CreateIPCMessage(new IPC_MessageBase(), IPCMessageType.IPC_AvatarRequest);
            SendTo(SteamID, message);
        }

        public static void SendLobbyLeave(ulong owner, ulong lobbyID)
        {
            var user = SteamFriends.Instance.GetUser(owner);
            if (user != null)
            {
                var lobbyLeave = new IPC_LobbyLeave()
                {
                    LobbyID = lobbyID,
                    SteamID = (ulong)SteamEmulator.SteamID
                };

                var message = CreateIPCMessage(lobbyLeave, IPCMessageType.IPC_LobbyLeave);

                SendTo(user.SteamID, message);
            }
        }

        internal static void SendLobbyRemove(SteamLobby lobby)
        {
            foreach (var member in lobby.Members)
            {
                if (member.m_SteamID != lobby.Owner)
                {
                    var user = SteamFriends.Instance.GetUser(member.m_SteamID);
                    if (user != null)
                    {
                        var lobbyRemove = new IPC_LobbyRemove()
                        {
                            LobbyID = lobby.SteamID
                        };

                        var message = CreateIPCMessage(lobbyRemove, IPCMessageType.IPC_LobbyRemove);

                        SendTo(user.SteamID, message);
                    }
                }
            }
        }

        private static void SendTo(ulong SteamID, IPCMessage message)
        {
            message.To = SteamID;
            if (IPCClient.IsConnected)
            {
                IPCClient.WriteAsync(message);
            }
            else
            {
                IPCClient.ConnectAsync();
            }
        }

        private static IPCMessage CreateIPCMessage(IPC_MessageBase Base, IPCMessageType type)
        {
            var message = new IPCMessage()
            {
                MessageType = (int)type,
                ParsedBody = Base.ToJson()
            };
            return message;
        }

        private static void Write(object v)
        {
            SteamEmulator.Write("IpcManager", v);
        }


    }
}
