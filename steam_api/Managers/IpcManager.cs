using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Helpers.JSON;
using SKYNET.IPC;
using SKYNET.IPC.Types;
using SKYNET.Steamworks;
using SKYNET.Steamworks.Implementation;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;

namespace SKYNET.Managers
{
    public class IPCManager
    {
        private const ulong IPC_ToServer  = 0;
        private const ulong IPC_Broadcast = 1;
        private const ulong IPC_Client = 2;
        
        private static PipeClient IPCClient;

        static IPCManager()
        {
            IPCClient = new PipeClient("SKYNET");
            IPCClient.AutoReconnect = true;
            IPCClient.Connected += IPCClient_Connected;
            IPCClient.Disconnected += IPCClient_Disconnected;
            IPCClient.MessageReceived += IPCClient_MessageReceived;
        }

        public static void Initialize()
        {
            ThreadPool.QueueUserWorkItem(Connect); 
        }

        private async static void Connect(object state)
        {
            await IPCClient.ConnectAsync();
        }

        private static void IPCClient_Connected(object sender, ConnectionEventArgs<IPCMessage> e)
        {
            SendClientHello();
        }

        private static void IPCClient_Disconnected(object sender, ConnectionEventArgs<IPCMessage> e)
        {
            Write("IPCClient Disconnected");
            ThreadPool.QueueUserWorkItem(Connect);
        }

        private static void IPCClient_MessageReceived(object sender, ConnectionMessageEventArgs<IPCMessage> message)
        {
            ProcessMessage(message.Message);
        }

        private static void ProcessMessage(IPCMessage message)
        {
            Write($"Received IPC message {(IPCMessageType)message.MessageType}, JobID = {message.JobID}");
            switch ((IPCMessageType)message.MessageType)
            {
                case IPCMessageType.IPC_ClientWelcome:
                    ProcessClientWelcome(message);
                    break;
                case IPCMessageType.IPC_AvatarResponse:
                    ProcessAvatarResponse(message);
                    break;
                case IPCMessageType.IPC_UserDataUpdated:
                    ProcessUserDataUpdated(message);
                    break;
                case IPCMessageType.IPC_LobbyListRequest:
                    ProcessLobbyListRequest(message);
                    break;
                case IPCMessageType.IPC_LobbyListResponse:
                    ProcessLobbyListResponse(message);
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
                case IPCMessageType.IPC_Achievements:
                    ProcessAchievements(message);
                    break;
                case IPCMessageType.IPC_Leaderboards:
                    ProcessLeaderboards(message);
                    break;
                case IPCMessageType.IPC_PlayerStats:
                    ProcessPlayerStats(message);
                    break;
                case IPCMessageType.IPC_P2PPacket:
                    ProcessP2PPacket(message);
                    break;
                case IPCMessageType.IPC_ModifyFileLog:
                    ProcessModifyFileLog(message);
                    break;
                case IPCMessageType.IPC_UsersResponse:
                    ProcessUsersResponse(message);
                    break;
                case IPCMessageType.IPC_LobbiesResponse:
                    ProcessLobbiesResponse(message);
                    break;
                case IPCMessageType.IPC_GCMessageResponse:
                    ProcessGCMessageResponse(message);
                    break;
                default:
                    Write($"Not found Handle for message {(IPCMessageType)message.MessageType}");
                    break;
            }

        }

        private static void ProcessLobbiesResponse(IPCMessage message)
        {
            var LobbiesResponse = message.ParsedBody.FromJson<IPC_LobbiesResponse>();
            if (LobbiesResponse == null) return;
            LobbyManager.UpdateLobbies(LobbiesResponse.Lobbies);
        }

        private static void ProcessUsersResponse(IPCMessage message)
        {
            var UsersResponse = message.ParsedBody.FromJson<IPC_UsersResponse>();
            if (UsersResponse == null) return;
            UserManager.UpdateUsers(UsersResponse.Users);
        }

        internal static void SendDirect3DVersion(OverlayManager.Direct3DVersion version)
        {
            var Direct3DVersion = new IPC_Direct3DVersionDetected()
            {
                Version = (IPC_Direct3DVersionDetected.Direct3DVersion)version
            };
            SendTo(IPC_Client, Direct3DVersion, IPCMessageType.IPC_Direct3DVersionDetected);
        }

        internal static void SendGCMessage(byte[] MsgBody, uint MsgType)
        {
            var IPC_GCRequest = new IPC_GCMessageRequest()
            {
                MsgType = MsgType,
                Buffer = MsgBody
            };
            SendTo(IPC_ToServer, IPC_GCRequest, IPCMessageType.IPC_GCMessageRequest);
        }

        private static void ProcessGCMessageResponse(IPCMessage message)
        {
            var GCMessageResponse = message.ParsedBody.FromJson<IPC_GCMessageResponse>();
            if (GCMessageResponse == null) return;
            SteamGameCoordinator.Instance.PushMessage(GCMessageResponse.MsgType, GCMessageResponse.Buffer);
        }


        private static void ProcessModifyFileLog(IPCMessage message)
        {
            var ModifyFileLog = message.ParsedBody.FromJson<IPC_ModifyFileLog>();
            if (ModifyFileLog != null)
            {
                SteamEmulator.LogToFile = ModifyFileLog.Enabled;
                if (ModifyFileLog.Enabled)
                {
                    string logPath = Path.Combine(Common.GetPath(), "SKYNET");
                    Common.EnsureDirectoryExists(logPath);
                }
            }
        }

        private static void ProcessDisableFileLog(IPCMessage message)
        {
            SteamEmulator.LogToFile = false;
        }

        #region IPC_ClientHello

        private static void SendClientHello()
        {
            var ClientHello = new IPC_ClientHello()
            {
                ExecutablePath = Common.GetExecutablePath(),
                ProcessID = Process.GetCurrentProcess().Id
            };

            SendTo(IPC_ToServer, ClientHello, IPCMessageType.IPC_ClientHello);
        }

        private static void ProcessClientWelcome(IPCMessage message)
        {
            try
            {
                var ClientWelcome = message.ParsedBody.FromJson<IPC_ClientWelcome>();
                SteamEmulator.PersonaName = ClientWelcome.PersonaName;
                SteamEmulator.SteamID = new CSteamID(ClientWelcome.AccountID);
                SteamEmulator.SteamID_GS = new CSteamID(ClientWelcome.GameServerID);
                SteamEmulator.GameOverlay = ClientWelcome.GameOverlay;
                SteamEmulator.LogToFile = ClientWelcome.LogToFile;
                SteamEmulator.LogToConsole = ClientWelcome.LogToConsole;
                SteamEmulator.RunCallbacks = ClientWelcome.RunCallbacks;
                SteamEmulator.AppID = ClientWelcome.AppID;
                SteamEmulator.ISteamHTTP = ClientWelcome.ISteamHTTP;
                SteamEmulator.SteamRemoteStorage.StoragePath = ClientWelcome.RemoteStoragePath;
                SteamEmulator.DLCs = ClientWelcome.DLCs;
                AudioManager.InputDeviceID = ClientWelcome.InputDeviceID;
                AudioManager.Initialize();

                if (ClientWelcome.LogToFile)
                {
                    Log.Initialize();
                    Write("Received Welcome from Emulator client");
                }
                if (ClientWelcome.LogToConsole)
                {
                    ConsoleHelper.CreateConsole("SKYNET");
                }
                if (ClientWelcome.GameOverlay)
                {
                    OverlayManager.Initialize();
                }

                SteamFriends.Instance.Initialize();
            }
            catch (Exception ex)
            {
                Write("Error in ClientWelcome: " + ex);
            }
        }

        #endregion

        internal static void SendLobbyGameServerEndPoint(ulong steamID, uint iP, uint usQueryPort)
        {
            var LobbyGameServerEndPoint = new IPC_LobbyGameServerEndPoint()
            {
                LobbySteamID = steamID,
                IP = iP,
                Port = usQueryPort
            };

            SendTo(IPC_ToServer, LobbyGameServerEndPoint, IPCMessageType.IPC_LobbyGameServerEndPoint);
        }

        private static void ProcessPlayerStats(IPCMessage message)
        {
            var PlayerStats = message.ParsedBody.FromJson<IPC_PlayerStats>();
            if (PlayerStats != null)
            {
                SteamUserStats.Instance?.SetPlayerStats(PlayerStats.SteamID, PlayerStats.PlayerStats);
            }
        }

        private static void ProcessAchievements(IPCMessage message)
        {
            var Achievements = message.ParsedBody.FromJson<IPC_Achievements>();
            if (Achievements != null)
            {
                SteamUserStats.Instance?.SetAchievements(Achievements.Achievements);
            }
        }

        private static void ProcessLeaderboards(IPCMessage message)
        {
            var Leaderboards = message.ParsedBody.FromJson<IPC_Leaderboards>();
            if (Leaderboards != null)
            {
                SteamUserStats.Instance?.SetLeaderboards(Leaderboards.Leaderboards);
            }
        }

        public static void SendP2PTo(ulong steamIDRemote, IPC_P2PPacket packet)
        {
            SendTo(steamIDRemote, packet, IPCMessageType.IPC_P2PPacket);
        }

        private static void ProcessP2PPacket(IPCMessage message)
        {
            var P2PPacket = message.ParsedBody.FromJson<IPC_P2PPacket>();
            if (P2PPacket == null) return;
            SteamNetworking.Instance.ProcessP2PPacket(P2PPacket); 
        }

        private static void ProcessLobbyGameserver(IPCMessage message)
        {
            var lobbyGameserver = message.ParsedBody.FromJson<IPC_LobbyGameserver>();
            if (lobbyGameserver != null)
            {
                if (SteamMatchmaking.Instance.GetLobby(lobbyGameserver.LobbyID, out var lobby))
                {
                    Write($"Received Gameserver data for lobby {lobbyGameserver.LobbyID}, IP = {lobbyGameserver.IP}, Port = {lobbyGameserver.Port}");
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

        public static void SendCreateLobby(SteamLobby lobby)
        {
            IPC_LobbyCreate CreateLobby = new IPC_LobbyCreate()
            {
                SerializedLobby = lobby.ToJson()
            };
            SendTo(IPC_ToServer, CreateLobby, IPCMessageType.IPC_LobbyCreate);
        }


        public static void SendAchievement(Achievement achievement)
        {
            IPC_SetAchievement Achievement = new IPC_SetAchievement()
            {
                AppID = SteamEmulator.AppID,
                Achievement = achievement
            };
            SendTo(IPC_ToServer, Achievement, IPCMessageType.IPC_SetAchievement);
        }

        public static void SendUpdateAchievement(Achievement achievement)
        {
            IPC_UpdateAchievement Achievement = new IPC_UpdateAchievement()
            {
                AppID = SteamEmulator.AppID,
                Achievement = achievement
            };
            SendTo(IPC_ToServer, Achievement, IPCMessageType.IPC_UpdateAchievement);
        }

        public static void SendPlayerStat(PlayerStat playerStat)
        {
            var PlayerStat = new IPC_SetPlayerStat()
            {
                AppID = SteamEmulator.AppID,
                PlayerStat = playerStat
            };
            SendTo(IPC_ToServer, PlayerStat, IPCMessageType.IPC_SetPlayerStat);
        }

        public static void SendLeaderboard(Leaderboard leaderboard)
        {
            var Leaderboard = new IPC_SetLeaderboard()
            {
                AppID = SteamEmulator.AppID,
                Leaderboard = leaderboard
            };
            SendTo(IPC_ToServer, Leaderboard, IPCMessageType.IPC_SetLeaderboard);
        }

        public static void SendResetAllStats(bool bAchievementsToo)
        {
            var ResetAllStats = new IPC_ResetAllStats()
            {
                AppID = SteamEmulator.AppID,
                AchievementsToo = bAchievementsToo
            };
            SendTo(IPC_ToServer, ResetAllStats, IPCMessageType.IPC_ResetAllStats);
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

        private static void ProcessLobbyJoinResponse(IPCMessage message)
        {
            try
            {
                var lobbyJoinResponse = message.ParsedBody.FromJson<IPC_LobbyJoinResponse>();
                if (lobbyJoinResponse != null)
                {
                    var lobby = lobbyJoinResponse.SerializedLobby.FromJson<SteamLobby>();
                    if (lobby != null)
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
            try
            {
                var lobbyListRequest = message.ParsedBody.FromJson<IPC_LobbyListRequest>();
                if (lobbyListRequest != null)
                {
                    //if (lobbyListRequest.RequestID == SteamMatchmaking.Instance.CurrentRequest)
                    //{

                    //    return;
                    //}
                    if (lobbyListRequest.AppID != 0 && lobbyListRequest.AppID != SteamEmulator.AppID)
                    {
                        return;
                    }
                }

                var lobby = LobbyManager.GetLobbyByOwner((ulong)SteamEmulator.SteamID);
                if (lobby != null)
                {
                    string serialized = lobby.ToJson();

                    var lobbyListResponse = new IPC_LobbyListResponse()
                    {
                        SerializedLobby = serialized
                    };
                    SendTo(IPC_Broadcast, lobbyListResponse, IPCMessageType.IPC_LobbyListResponse);
                }
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        internal static int GetUsersOnline(uint appID)
        {
            // TODO: Request to client
            return 100;
        }

        private static void ProcessLobbyListResponse(IPCMessage message)
        {
            try
            {
                var lobbyListResponse = message.ParsedBody.FromJson<IPC_LobbyListResponse>();
                var lobby = lobbyListResponse.SerializedLobby.FromJson<SteamLobby>();
                if (lobby != null)
                {
                    Write($"******** warn implement - Adding lobby {lobby.SteamID}");
                }
            }
            catch (Exception ex)
            {
                Write(ex);
            }

        }

        private static void ProcessAvatarResponse(IPCMessage message)
        {
            try
            {
                var AvatarResponse = message.ParsedBody.FromJson<IPC_AvatarResponse>();
                if (AvatarResponse != null)
                {
                    var imageBytes = Convert.FromBase64String(AvatarResponse.HexAvatar);
                    if (imageBytes.Length != 0)
                    {
                        Bitmap Avatar = (Bitmap)ImageHelper.ImageFromBytes(imageBytes);
                        if (AvatarResponse.AccountID == 0)
                        {
                            SteamFriends.Instance.AddOrUpdateAvatar(Avatar, 0);
                        }
                        else
                        {
                            ulong SteamID = (ulong)new CSteamID(AvatarResponse.AccountID);
                            SteamFriends.Instance.AddOrUpdateAvatar(Avatar, SteamID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Write($"{ex}");
            }
        }

        private static void ProcessUserDataUpdated(IPCMessage message)
        {
            try
            {
                var StatusChanged = message.ParsedBody.FromJson<IPC_UserDataUpdated>();
                if (StatusChanged != null)
                {
                    SteamFriends.Instance.UpdateUserStatus(StatusChanged);
                }
            }
            catch
            {
                //
            }
        }

        public static void SendUserDataUpdated(SteamPlayer user, IPC_UserDataUpdated.UpdateType Type)
        {
            var status = new IPC_UserDataUpdated()
            {
                PersonaName = user.PersonaName,
                AccountID = (uint)SteamEmulator.SteamID.AccountID, 
                LobbyID = user.LobbyID.GetAccountID(),
                Type = Type
            };

            SendTo(IPC_Broadcast, status, IPCMessageType.IPC_UserDataUpdated);
        }

        internal static void SendClearRichPresence()
        {
            var ClearRichPresence = new IPC_ClearRichPresence();
            SendTo(IPC_Broadcast, ClearRichPresence, IPCMessageType.IPC_ClearRichPresence);
        }

        public static async void SendLobbyJoinRequest(ulong APICall, SteamLobby lobby)
        {
            try
            {
                var JoinRequest = new IPC_LobbyJoinRequest()
                {
                    LobbyID = lobby.SteamID,
                    SteamID = (ulong)SteamEmulator.SteamID,
                    CallbackHandle = APICall
                };

                var user = GetUser(lobby.Owner);
                if (user == null)
                {
                    Write($"Not found user to send LobbyJoinRequest, SteamID {new CSteamID(lobby.Owner).ToString()}");
                    return;
                }

                SendTo(user.SteamID, JoinRequest, IPCMessageType.IPC_LobbyJoinRequest);
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        internal static void SendLobbyGameServer(ulong steamIDLobby, ulong steamIDGameServer, uint unGameServerIP, uint unGameServerPort)
        {
            var lobbyGameserver = new IPC_LobbyGameserver()
            {
                LobbyID = steamIDLobby,
                SteamID = steamIDGameServer, 
                IP = unGameServerIP,
                Port = unGameServerPort
            };

            SendTo(IPC_Client, lobbyGameserver, IPCMessageType.IPC_LobbyGameserver);
        }

        public static void SendLobbyListRequest(uint currentRequest)
        {
            var lobbyListRequest = new IPC_LobbyListRequest()
            {
                AppID = SteamEmulator.AppID,
                RequestID = currentRequest
            };

            SendTo(IPC_Broadcast, lobbyListRequest, IPCMessageType.IPC_LobbyListRequest);
        }

        public static async void SendLobbyDataUpdate(ulong IDTarget, ulong IDLobby, ulong IDMember, SteamLobby lobby)
        {
            var lobbyDataUpdate = new IPC_LobbyDataUpdate()
            { 
                TargetSteamID = IDTarget,
                SteamIDLobby = IDLobby,
                SteamIDMember = IDMember,
                SerializedLobby = lobby.ToJson()
            };

            var user = UserManager.GetUser(IDTarget);
            if (user != null)
            {
                SendTo(user.SteamID, lobbyDataUpdate, IPCMessageType.IPC_LobbyDataUpdate);
            }
        }

        internal static void SendSetRichPresence(string pchKey, string pchValue)
        {
            var SetRichPresence = new IPC_SetRichPresence()
            {
                Key = pchKey,
                Value = pchValue
            };
            SendTo(IPC_Client, SetRichPresence, IPCMessageType.IPC_SetRichPresence);
        }

        public static void RequestAvatar(ulong SteamID)
        {
            SendTo(SteamID, new IPC_MessageBase(), IPCMessageType.IPC_AvatarRequest);
        }

        public static async void SendLobbyLeave(ulong owner, ulong lobbyID)
        {
            var user = UserManager.GetUser(owner);
            if (user != null)
            {
                var lobbyLeave = new IPC_LobbyLeave()
                {
                    LobbyID = lobbyID,
                    SteamID = (ulong)SteamEmulator.SteamID
                };

                SendTo(user.SteamID, lobbyLeave, IPCMessageType.IPC_LobbyLeave);
            }
        }

        public static async void SendLobbyRemove(SteamLobby lobby)
        {
            foreach (var member in lobby.Members)
            {
                if (member.m_SteamID != lobby.Owner)
                {
                    var user = UserManager.GetUser(member.m_SteamID);
                    if (user != null)
                    {
                        var lobbyRemove = new IPC_LobbyRemove()
                        {
                            LobbyID = lobby.SteamID
                        };

                        SendTo(user.SteamID, lobbyRemove, IPCMessageType.IPC_LobbyRemove);
                    }
                }
            }
        }

        public static List<SteamPlayer> GetFriends()
        {
            var FriendsRequest = new IPC_GetFriendsRequest();
            var messageResponse = SendTo(IPC_Client, FriendsRequest, IPCMessageType.IPC_GetFriendsRequest, true);
            if (messageResponse != null)
            {
                var UserResponse = messageResponse.ParsedBody.FromJson<IPC_GetFriendsResponse>();
                return UserResponse.Friends;
            }
            return new List<SteamPlayer>();
        }

        public static SteamPlayer GetUser(ulong steamID)
        {
            var User = UserManager.GetUser(steamID);
            if (User != null)
            {
                return User;
            }
            else
            {
                SendUsersRequest();
                return null;
            }

            var UserRequest = new IPC_GetUserRequest()
            {
                SteamID = steamID,
            };
            var messageResponse = SendTo(IPC_ToServer, UserRequest, IPCMessageType.IPC_GetUserRequest, true);
            if (messageResponse != null)
            {
                var UserResponse = messageResponse.ParsedBody.FromJson<IPC_GetUserResponse>();
                return UserResponse.User;
            }
            return null;
        }

        public static void SendUsersRequest()
        {
            var UsersRequest = new IPC_UsersRequest();
            SendTo(IPC_ToServer, UsersRequest, IPCMessageType.IPC_UsersRequest);
        }

        public static void SendLobbiesRequest()
        {
            var LobbiesRequest = new IPC_LobbiesRequest();
            SendTo(IPC_ToServer, LobbiesRequest, IPCMessageType.IPC_LobbiesRequest);
        }

        //internal static SteamLobby GetLobby(ulong steamID, bool byOwner)
        //{
        //    var GetLobby = new IPC_LobbyRequest()
        //    {
        //        ByOwner = byOwner,
        //        SteamID = steamID,
        //    };
        //    var messageResponse = SendTo(IPC_ToServer, GetLobby, IPCMessageType.IPC_LobbyRequest, true);
        //    if (messageResponse != null)
        //    {
        //        var LobbyResponse = messageResponse.ParsedBody.FromJson<IPC_LobbyResponse>();
        //        if (LobbyResponse != null)
        //        {
        //            return LobbyResponse.Lobby;
        //        }
        //    }
        //    return null;
        //}

        //internal static SteamLobby GetLobbyByIndex(uint appID, int iLobby)
        //{
        //    var ByIndexRequest = new IPC_LobbyByIndexRequest()
        //    {
        //        AppID = appID,
        //        Index = iLobby,
        //    };
        //    var messageResponse = SendTo(IPC_ToServer, ByIndexRequest, IPCMessageType.IPC_LobbyByIndexRequest, true);
        //    if (messageResponse != null)
        //    {
        //        var ByIndexResponse = messageResponse.ParsedBody.FromJson<IPC_LobbyByIndexResponse>();
        //        if (ByIndexResponse != null)
        //        {
        //            return ByIndexResponse.Lobby;
        //        }
        //    }
        //    return null;
        //}

        public static bool SetLobbyData(ulong steamIDLobby, string pchKey, string pchValue)
        {
            var LobbySetData = new IPC_LobbySetData()
            {
                SteamID = steamIDLobby,
                Key = pchKey,
                Value = pchValue,
            };
            SendTo(IPC_ToServer, LobbySetData, IPCMessageType.IPC_LobbySetData);
            return true;
        }

        internal static uint GetLobbyCount(uint appID)
        {
            var CountRequest = new IPC_LobbyCountRequest()
            {
                AppID = appID,
            };
            var messageResponse = SendTo(IPC_ToServer, CountRequest, IPCMessageType.IPC_LobbyCountRequest, true);
            if (messageResponse != null)
            {
                var CountResponse = messageResponse.ParsedBody.FromJson<IPC_LobbyCountResponse>();
                if (CountResponse != null)
                {
                    return CountResponse.Count;
                }
            }
            return 0;
        }

        private static IPCMessage SendTo(ulong SteamID, IPC_MessageBase Base, IPCMessageType type, bool WaitResponse = false)
        {
            var message = new IPCMessage()
            {
                To = SteamID,
                JobID = (ulong)(WaitResponse ? new Random().Next(1000, 999999) : 0),
                MessageType = (int)type,
                ParsedBody = Base.ToJson()
            };
            message.To = SteamID;
            if (IPCClient.IsConnected)
            {
                if (WaitResponse)
                {
                    var jobID = (ulong)new Random().Next(1000, 999999);
                    message.JobID = jobID;
                }
                Write($"Sending IPC message type {(IPCMessageType)message.MessageType}");
                var TaskResponse = IPCClient.WriteAsync(message, WaitResponse);
                if (TaskResponse.Result == null) return null;
                return TaskResponse.Result;
            }
            else
            {
                IPCClient.ConnectAsync();
            }
            return null;
        }

        private static void Write(object msg)
        {
            SteamEmulator.Write("IPCManager", msg);
        }
    }
}
