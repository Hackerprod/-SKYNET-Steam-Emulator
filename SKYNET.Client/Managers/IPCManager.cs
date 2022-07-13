using SKYNET.Client;
using SKYNET.Common;
using SKYNET.Helper;
using SKYNET.IPC;
using SKYNET.IPC.Types;
using SKYNET.Network;
using SKYNET.Network.Packets;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace SKYNET.Managers
{
    public class IPCManager
    {
        private const ulong IPC_ToServer = 0;
        private const ulong IPC_Broadcast = 1;
        private static PipeServer<IPCMessage> server;
        private static string LastMessage;

        public static void Initialize()
        {
            server = new PipeServer<IPCMessage>("SKYNET");
            server.ClientConnected += OnClientConnected; 
            server.ClientDisconnected += OnClientDisconnected;
            server.MessageReceived += OnMessageReceived;
            server.ExceptionOccurred += OnExceptionOccurred;
            server.StartAsync();

            LastMessage = "";
        }

        private static void OnClientConnected(object sender, ConnectionEventArgs<IPCMessage> args)
        {
            Write($"Client {args.Connection.PipeName} is now connected!");
        }

        private static void OnMessageReceived(object sender, ConnectionMessageEventArgs<IPCMessage> e)
        {
            Write($"Received IPC message {(IPCMessageType)e.Message.MessageType}, JobID = {e.Message.JobID}");

            switch ((IPCMessageType)e.Message.MessageType)
            {
                case IPCMessageType.IPC_ClientHello:
                    Process_ClientHello(e.Connection, e.Message);
                    break;
                case IPCMessageType.IPC_AvatarRequest:
                    Process_AvatarRequest(e.Connection, e.Message);
                    break;
                case IPCMessageType.IPC_UserDataUpdated:
                    Process_UserDataUpdated(e.Message);
                    break;
                case IPCMessageType.IPC_P2PPacket:
                    Process_P2PPacket(e.Message);
                    break;
                case IPCMessageType.IPC_LobbyListRequest:
                    Process_LobbyListRequest(e.Message);
                    break;
                case IPCMessageType.IPC_LobbyJoinRequest:
                    Process_LobbyJoinRequest(e.Message);
                    break;
                case IPCMessageType.IPC_LobbyJoinResponse:
                    break;
                case IPCMessageType.IPC_LobbyDataUpdate:
                    Process_LobbyDataUpdate(e.Message);
                    break;
                case IPCMessageType.IPC_LobbyChatUpdate:
                    Process_LobbyChatUpdate(e.Message);
                    break;
                case IPCMessageType.IPC_LobbyLeave:
                    Process_LobbyLeave(e.Message);
                    break;
                case IPCMessageType.IPC_LobbyCreate:
                    Process_LobbyCreate(e.Message);
                    break;
                case IPCMessageType.IPC_LobbyRemove:
                    Process_LobbyRemove(e.Message);
                    break;
                case IPCMessageType.IPC_LobbyGameserver:
                    Process_LobbyGameserver(e.Message);
                    break;
                case IPCMessageType.IPC_LobbyRequest:
                    Process_LobbyRequest(e.Message);
                    break;
                case IPCMessageType.IPC_GCMessageRequest:
                    break;
                case IPCMessageType.IPC_GCMessageResponse:
                    break;
                case IPCMessageType.IPC_SetAchievement:
                    Process_SetAchievement(e.Message);
                    break;
                case IPCMessageType.IPC_SetLeaderboard:
                    Process_SetLeaderboard(e.Message);
                    break;
                case IPCMessageType.IPC_SetPlayerStat:
                    Process_SetPlayerStat(e.Message);
                    break;
                case IPCMessageType.IPC_UpdateAchievement:
                    Process_UpdateAchievement(e.Message);
                    break;
                case IPCMessageType.IPC_ResetAllStats:
                    Process_ResetAllStats(e.Message);
                    break;
                case IPCMessageType.IPC_GetUserRequest:
                    Process_GetUserRequest(e.Message);
                    break;
                case IPCMessageType.IPC_GetFriendsRequest:
                    Process_GetFriendsRequest(e.Message);
                    break;
                case IPCMessageType.IPC_ClearRichPresence:
                    Process_ClearRichPresence(e.Message);
                    break;
                case IPCMessageType.IPC_SetRichPresence:
                    Process_SetRichPresence(e.Message);
                    break;
                case IPCMessageType.IPC_LobbyByIndexRequest:
                    Process_LobbyByIndexRequest(e.Message);
                    break;
                case IPCMessageType.IPC_LobbyCountRequest:
                    Process_LobbyCountRequest(e.Message);
                    break;
                case IPCMessageType.IPC_LobbySetData:
                    Process_LobbySetData(e.Message);
                    break;
                case IPCMessageType.IPC_UsersRequest:
                    Process_UsersRequest(e.Message);
                    break;
                case IPCMessageType.IPC_LobbiesRequest:
                    Process_LobbiesRequest(e.Message);
                    break;
                default:
                    Write($"Not implemented Handle for message {(IPCMessageType)e.Message.MessageType}");
                    break;
            }
        }

        private static void Process_UsersRequest(IPCMessage message)
        {
            var UsersResponse = new IPC_UsersResponse()
            {
                Users = UserManager.Users
            };
            var welcome = CreateIPCMessage(UsersResponse, IPCMessageType.IPC_UsersResponse);
            SendIPCMessage(welcome);
        }

        private static void Process_LobbiesRequest(IPCMessage message)
        {
            var LobbiesResquest = message.ParsedBody.Deserialize<IPC_LobbiesRequest>();
            if (LobbiesResquest == null) return;
            var LobbiesResponse = new IPC_LobbiesResponse()
            {
                Lobbies = LobbyManager.GetLobbies(LobbiesResquest.AppID)
            };
            var welcome = CreateIPCMessage(LobbiesResponse, IPCMessageType.IPC_LobbiesResponse);
            SendIPCMessage(welcome);
        }

        private static void Process_LobbySetData(IPCMessage message)
        {
            var LobbySetData = message.ParsedBody.Deserialize<IPC_LobbySetData>();
            LobbyManager.SetLobbyData(LobbySetData.SteamID, LobbySetData.Key, LobbySetData.Value);
        }

        private static void Process_LobbyCountRequest(IPCMessage message)
        {
            var LobbyCountRequest = message.ParsedBody.Deserialize<IPC_LobbyCountRequest>();
            var LobbyCountResponse = new IPC_LobbyCountResponse();
            if (LobbyCountRequest != null)
            {
                var lobbies = LobbyManager.GetLobbies(LobbyCountRequest.AppID);
                LobbyCountResponse.Count = (uint)lobbies.Count;
            }
            var welcome = CreateIPCMessage(LobbyCountResponse, IPCMessageType.IPC_LobbyCountResponse, message.JobID);
            SendIPCMessage(welcome);
        }

        private static void Process_LobbyRequest(IPCMessage message)
        {
            var LobbyRequest = message.ParsedBody.Deserialize<IPC_LobbyRequest>();
            var LobbyResponse = new IPC_LobbyResponse();
            if (LobbyRequest != null)
            {
                if (LobbyRequest.ByOwner)
                {
                    LobbyResponse.Lobby = LobbyManager.GetLobbyByOwner(LobbyRequest.SteamID); 
                }
                else
                {
                    LobbyResponse.Lobby = LobbyManager.GetLobby(LobbyRequest.SteamID);
                }
            }
            var welcome = CreateIPCMessage(LobbyResponse, IPCMessageType.IPC_LobbyResponse, message.JobID);
            SendIPCMessage(welcome);
        }

        private static void Process_LobbyByIndexRequest(IPCMessage message)
        {
            var ByIndexRequest = message.ParsedBody.Deserialize<IPC_LobbyByIndexRequest>();
            var ByIndexResponse = new IPC_LobbyResponse();
            if (ByIndexRequest != null)
            {
                var Lobby = LobbyManager.GetLobbyByIndex(ByIndexRequest.AppID, ByIndexRequest.Index);
                if (Lobby != null)
                {
                    ByIndexResponse.Lobby = Lobby;
                }
            }
            var welcome = CreateIPCMessage(ByIndexResponse, IPCMessageType.IPC_LobbyByIndexResponse, message.JobID);
            SendIPCMessage(welcome);
        }

        private static void Process_SetRichPresence(IPCMessage message)
        {
            var SetRichPresence = message.ParsedBody.Deserialize<IPC_SetRichPresence>();
            if (SetRichPresence == null) return;

            UserManager.SetRichPresence(SteamClient.SteamID.AccountID, SetRichPresence.Key, SetRichPresence.Value);
            NetworkManager.SendRichPresence(SetRichPresence.Key, SetRichPresence.Value);

        }

        private static void Process_ClearRichPresence(IPCMessage message)
        {
            var user = UserManager.GetUser(SteamClient.SteamID);
            user.RichPresence.Clear();
        }

        private static void Process_GetFriendsRequest(IPCMessage message)
        {
            var FriendsResponse = new IPC_GetFriendsResponse();
            var Friends = UserManager.GetFriends();
            FriendsResponse.Friends = Friends;

            var messageResponse = CreateIPCMessage(FriendsResponse, IPCMessageType.IPC_GetFriendsResponse, message.JobID);
            SendIPCMessage(messageResponse);
        }

        private static void Process_GetUserRequest(IPCMessage message)
        {
            var UserRequest = message.ParsedBody.Deserialize<IPC_GetUserRequest>();
            IPC_GetUserResponse UserResponse = new IPC_GetUserResponse();
            if (UserRequest != null)
            {
                var User = UserManager.GetUser(UserRequest.SteamID);
                if (User != null)
                {
                    UserResponse.User = User;
                }
            }
            var messageResponse = CreateIPCMessage(UserResponse, IPCMessageType.IPC_GetUserResponse, message.JobID);
            SendIPCMessage(messageResponse);
        }

        private static void Process_SetAchievement(IPCMessage message)
        {
            var Achievement = message.ParsedBody.Deserialize<IPC_SetAchievement>();
            StatsManager.SetAchievement(Achievement.AppID, Achievement.Achievement);
        }

        private static void Process_SetLeaderboard(IPCMessage message)
        {
            var Leaderboard = message.ParsedBody.Deserialize<IPC_SetLeaderboard>();
            StatsManager.SetLeaderboard(Leaderboard.AppID, Leaderboard.Leaderboard);
        }

        private static void Process_SetPlayerStat(IPCMessage message)
        {
            var PlayerStat = message.ParsedBody.Deserialize<IPC_SetPlayerStat>();
            StatsManager.SetPlayerStat(PlayerStat.AppID, PlayerStat.PlayerStat);
        }

        private static void Process_UpdateAchievement(IPCMessage message)
        {
            var UpdateAchievement = message.ParsedBody.Deserialize<IPC_UpdateAchievement>();
            StatsManager.UpdateAchievement(UpdateAchievement.AppID, UpdateAchievement.Achievement);
        }

        private static void Process_ResetAllStats(IPCMessage message)
        {
            var ResetAllStats = message.ParsedBody.Deserialize<IPC_ResetAllStats>();
            StatsManager.ResetAllStats(ResetAllStats.AppID, ResetAllStats.AchievementsToo);
        }

        private static void Process_ClientHello(PipeConnection<IPCMessage> connection, IPCMessage message)
        {
            var ClientHello = message.ParsedBody.Deserialize<IPC_ClientHello>();
            if (ClientHello == null) return;

            var RemoteStoragePath = Path.Combine(modCommon.GetPath(), "Data", "Storage", "Remote");
            modCommon.EnsureDirectoryExists(RemoteStoragePath);
            // TODO: Send Client welcome with user data
            var ClientWelcome = new IPC_ClientWelcome()
            {
                AccountID = SteamClient.AccountID,
                PersonaName = SteamClient.PersonaName,
                Language = SteamClient.Language,
                GameServerID = SteamClient.SteamID_GS.AccountID,
                LogToFile = false,
                LogToConsole = false,
                RunCallbacks = true,
                ISteamHTTP = true,
                RemoteStoragePath = RemoteStoragePath
            };

            Game Game = null;
            if (ClientHello != null)
            {
                Game = GameManager.GetGameByPath(ClientHello.ExecutablePath);
                if (Game != null)
                {
                    Log.Write("IPCManager", $"Hello received from {Game.Name}");
                    GameManager.InvokeGameLaunched(Game, ClientHello.ProcessID, connection.PipeName);

                    ClientWelcome.LogToFile = Game.LogToFile;
                    ClientWelcome.LogToConsole = Game.LogToConsole;
                    ClientWelcome.RunCallbacks = Game.RunCallbacks;
                    ClientWelcome.ISteamHTTP = Game.ISteamHTTP;
                    ClientWelcome.AppID = Game.AppID;

                }
            }
            var welcome = CreateIPCMessage(ClientWelcome, IPCMessageType.IPC_ClientWelcome, message.JobID);
            connection.WriteAsync(welcome);

            SendUpdatedUsers();
            SendUpdatedLobbies(Game.AppID);

            // TODO: Send user avatar
            var hexAvatar = ImageHelper.GetImageBase64(SteamClient.Avatar);
            var AvatarResponse = new IPC_AvatarResponse()
            {
                AccountID = SteamClient.AccountID,
                HexAvatar = hexAvatar
            };
            var AvatarMessage = CreateIPCMessage(AvatarResponse, IPCMessageType.IPC_AvatarResponse);
            connection.WriteAsync(AvatarMessage);

            // TODO: Send user avatar
            hexAvatar = ImageHelper.GetImageBase64(SteamClient.DefaultAvatar);
            var DefaultAvatarResponse = new IPC_AvatarResponse()
            {
                AccountID = 0,
                HexAvatar = hexAvatar
            };
            var DefaultAvatarMessage = CreateIPCMessage(DefaultAvatarResponse, IPCMessageType.IPC_AvatarResponse);
            connection.WriteAsync(DefaultAvatarMessage);


            if (Game == null) return;

            // TODO: Send Achievements
            var Achievements = new IPC_Achievements()
            {
                Achievements = StatsManager.GetAchievements(Game.AppID)
            };
            var AchievementsMessage = CreateIPCMessage(Achievements, IPCMessageType.IPC_Achievements);
            connection.WriteAsync(AchievementsMessage);

            // TODO: Send Achievements
            var Leaderboards = new IPC_Leaderboards()
            {
                Leaderboards = StatsManager.GetLeaderboards(Game.AppID)
            };
            var LeaderboardsMessage = CreateIPCMessage(Leaderboards, IPCMessageType.IPC_Leaderboards);
            connection.WriteAsync(LeaderboardsMessage);


            // TODO: Send Achievements
            var PlayerStats = new IPC_PlayerStats()
            {
                PlayerStats = StatsManager.GetPlayerStats(Game.AppID)
            };
            var PlayerStatsMessage = CreateIPCMessage(Achievements, IPCMessageType.IPC_PlayerStats);
            connection.WriteAsync(PlayerStatsMessage);
        }

        #region IPC_P2PPacket
        internal static void SendP2PPacket(NET_P2PPacket p2PPacket)
        {
            var P2PPacket = new IPC_P2PPacket()
            {
                Sender = p2PPacket.Sender,
                AccountID = p2PPacket.AccountID,
                Buffer = p2PPacket.Buffer,
                Channel = p2PPacket.Channel,
                P2PSendType = p2PPacket.P2PSendType
            };
            var IPCMessage = CreateIPCMessage(P2PPacket, IPCMessageType.IPC_P2PPacket);
            SendIPCMessage(IPCMessage);
        }

        private static void Process_P2PPacket(IPCMessage message)
        {
            var P2PPacket = message.ParsedBody.Deserialize<IPC_P2PPacket>();
            if (P2PPacket != null)
                P2PNetworking.SendP2PTo(message.To, P2PPacket);
        }
        #endregion

        private static void Process_LobbyGameserver(IPCMessage message)
        {
            var LobbyGameserver = message.ParsedBody.Deserialize<IPC_LobbyGameserver>();
            if (LobbyGameserver != null)
                NetworkManager.SendLobbyGameserver(LobbyGameserver);
        }

        private static void Process_UserDataUpdated(IPCMessage message)
        {
            var UserDataUpdated = message.ParsedBody.Deserialize<IPC_UserDataUpdated>();
            if (UserDataUpdated != null)
            {
                UserManager.UserDataUpdated(UserDataUpdated.AccountID, UserDataUpdated.PersonaName, UserDataUpdated.LobbyID);
                NetworkManager.SendUserDataUpdated(UserDataUpdated);
            }
        }

        private static void Process_AvatarRequest(PipeConnection<IPCMessage> connection, IPCMessage message)
        {
            var AvatarRequest = message.ParsedBody.Deserialize<IPC_AvatarRequest>();
            if (AvatarRequest != null)
            {
                try
                {
                    // TODO: Response avatar stored in cache
                    string AvatarCachePath = Path.Combine(modCommon.GetPath(), "Data", "Images", "AvatarCache", AvatarRequest.AccountID + ".jpg");
                    if (File.Exists(AvatarCachePath))
                    {
                        var Avatar = ImageHelper.FromFile(AvatarCachePath);
                        var hexAvatar = ImageHelper.GetImageBase64(Avatar);
                        var AvatarResponse = new IPC_AvatarResponse()
                        {
                            AccountID = AvatarRequest.AccountID,
                            HexAvatar = hexAvatar
                        };
                        var AvatarMessage = CreateIPCMessage(AvatarResponse, IPCMessageType.IPC_AvatarResponse);
                        connection.WriteAsync(AvatarMessage);
                    }
                }
                catch 
                {
                }

                // Request updated avatar
                NetworkManager.SendAvatarRequest(AvatarRequest);
            }
        }

        public static void SendAvatarUpdated(uint AccountID, Bitmap Avatar)
        {
            var hexAvatar = ImageHelper.GetImageBase64(Avatar);
            var AvatarResponse = new IPC_AvatarResponse()
            {
                AccountID = AccountID,
                HexAvatar = hexAvatar
            };
            var AvatarMessage = CreateIPCMessage(AvatarResponse, IPCMessageType.IPC_AvatarResponse);
            foreach (var Client in server.ConnectedClients)
            {
                Client.WriteAsync(AvatarMessage);
            }
        }

        public static void SendUserDataUpdated(uint AccountID, string PersonaName)
        {
            var UserDataUpdated = new IPC_UserDataUpdated()
            {
                AccountID   = AccountID,
                PersonaName = PersonaName
            };
            var DataUpdatedMessage = CreateIPCMessage(UserDataUpdated, IPCMessageType.IPC_UserDataUpdated);
            foreach (var Client in server.ConnectedClients)
            {
                Client.WriteAsync(DataUpdatedMessage);
            }
        }

        private static void Process_LobbyChatUpdate(IPCMessage message)
        {
            var LobbyChatUpdate = message.ParsedBody.Deserialize<IPC_LobbyChatUpdate>();
            if (LobbyChatUpdate != null)
                NetworkManager.SendLobbyChatUpdate(message.To, LobbyChatUpdate);
        }

        private static void Process_LobbyListRequest(IPCMessage message)
        {
            var LobbyListRequest = message.ParsedBody.Deserialize<IPC_LobbyListRequest>();
            if (LobbyListRequest != null)
                NetworkManager.SendLobbyListRequest(LobbyListRequest);
        }

        private static void Process_LobbyLeave(IPCMessage message)
        {
            var LobbyRemove = message.ParsedBody.Deserialize<IPC_LobbyRemove>();
            if (LobbyRemove != null)
            {
                var lobby = LobbyManager.GetLobby(LobbyRemove.LobbyID);
                if (lobby == null) return;

                NetworkManager.SendLobbyLeave(lobby.Owner, lobby.SteamID);
                LobbyManager.Remove(LobbyRemove.LobbyID);
            }
        }

        private static void Process_LobbyRemove(IPCMessage message)
        {
            var LobbyRemove = message.ParsedBody.Deserialize<IPC_LobbyRemove>();
            if (LobbyRemove != null)
            {
                var lobby = LobbyManager.GetLobby(LobbyRemove.LobbyID);
                if (lobby == null) return;

                NetworkManager.SendLobbyRemove(lobby);
                LobbyManager.Remove(LobbyRemove.LobbyID);
                // TODO: Send lobby to steam server
            }
        }

        private static void Process_LobbyCreate(IPCMessage message)
        {
            var LobbyCreate = message.ParsedBody.Deserialize<IPC_LobbyCreate>();
            SteamLobby lobby = LobbyCreate.SerializedLobby.Deserialize<SteamLobby>();
            if (lobby != null)
            {
                LobbyManager.Create(lobby);
                // TODO: Send lobby to steam server
            }
        }

        private static void Process_LobbyJoinRequest(IPCMessage message)
        {
            var LobbyJoinRequest = message.ParsedBody.Deserialize<IPC_LobbyJoinRequest>();
            NetworkManager.SendLobbyJoinRequest(LobbyJoinRequest);
        }

        private static void Process_LobbyDataUpdate(IPCMessage message)
        {
            var LobbyDataUpdate = message.ParsedBody.Deserialize<IPC_LobbyDataUpdate>();
            NetworkManager.SendLobbyDataUpdate(LobbyDataUpdate);
            SteamLobby lobby = LobbyDataUpdate.SerializedLobby.Deserialize<SteamLobby>();
            if (lobby != null)
            {
                LobbyManager.Update(lobby);
                NetworkManager.SendLobbyDataUpdate(LobbyDataUpdate);
            }
        }

        private static void OnClientDisconnected(object sender, ConnectionEventArgs<IPCMessage> e)
        {
            Log.Write("IPCManager", $"Client {e.Connection.PipeName} disconnected");
            GameManager.InvokeGameClosed(e.Connection.PipeName);
        }

        private static void OnExceptionOccurred(object sender, ExceptionEventArgs e)
        {
            Log.Write("IPCManager", "Exception Occurred!!! " + e.Exception);
        }

        public static void AvatarResponse(string HexAvatar, uint AccountID)
        {
            var AvatarResponse = new IPC_AvatarResponse()
            {
                HexAvatar = HexAvatar,
                AccountID = AccountID
            };
            var IPCMessage = CreateIPCMessage(AvatarResponse, IPCMessageType.IPC_AvatarResponse);
            SendIPCMessage(IPCMessage);
        }

        public static void UpdateUserStatus(NET_UserDataUpdated statusChanged, IPC_UserDataUpdated.UpdateType UpdateType)
        {
            var UserDataUpdated = new IPC_UserDataUpdated()
            {
                LobbyID = statusChanged.LobbyID,
                AccountID = statusChanged.AccountID,
                PersonaName = statusChanged.PersonaName,
                Type = UpdateType
            };
            var IPCMessage = CreateIPCMessage(UserDataUpdated, IPCMessageType.IPC_UserDataUpdated);
            SendIPCMessage(IPCMessage);
        }

        internal static void SendLobbyLeave(NET_LobbyLeave lobbyLeave)
        {
            var LobbyLeave = new IPC_LobbyLeave()
            {
                LobbyID = lobbyLeave.SteamID,
                SteamID = lobbyLeave.SteamID
            };
            var IPCMessage = CreateIPCMessage(LobbyLeave, IPCMessageType.IPC_LobbyLeave);
            SendIPCMessage(IPCMessage);
        }

        internal static void SendLobbyChatUpdate(NET_LobbyChatUpdate lobbyChatUpdate)
        {
            var LobbyChatUpdate = new IPC_LobbyChatUpdate()
            {
                SteamIDLobby = lobbyChatUpdate.SteamIDLobby,
                ChatMemberStateChange = lobbyChatUpdate.ChatMemberStateChange,
                SteamIDMakingChange = lobbyChatUpdate.SteamIDMakingChange,
                SteamIDUserChanged = lobbyChatUpdate.SteamIDUserChanged
            };
            var IPCMessage = CreateIPCMessage(LobbyChatUpdate, IPCMessageType.IPC_LobbyChatUpdate);
            SendIPCMessage(IPCMessage);
        }

        private static IPCMessage CreateIPCMessage(IPC_MessageBase Base, IPCMessageType type, ulong jobId = 0)
        {
            var message = new IPCMessage()
            { 
                JobID = jobId,
                MessageType = (int)type,
                ParsedBody = Base.ToJson(), Result = "Theresult"
            };
            return message;
        }

        private static void SendIPCMessage(IPCMessage iPCMessage)
        {
            foreach (var client in server.ConnectedClients)
            {
                if (client.IsConnected)
                {
                    client.WriteAsync(iPCMessage);
                }
            }
        }

        internal static void SendLobbyListResponse(NET_LobbyListResponse lobbyListResponse)
        {
            var LobbyListResponse = new IPC_LobbyListResponse()
            {
                SerializedLobby = lobbyListResponse.SerializedLobby
            };
            var message = CreateIPCMessage(LobbyListResponse, IPCMessageType.IPC_LobbyListResponse);
            SendIPCMessage(message);
        }

        internal static void SendLobbyJoinResponse(NET_LobbyJoinResponse lobbyJoinResponse)
        {
            var LobbyListResponse = new IPC_LobbyJoinResponse()
            {
                SerializedLobby = lobbyJoinResponse.SerializedLobby
            };
            var message = CreateIPCMessage(LobbyListResponse, IPCMessageType.IPC_LobbyJoinResponse);
            SendIPCMessage(message);
        }

        internal static void SendLobbyJoinRequest(NET_LobbyJoinRequest lobbyJoinRequest)
        {
            var LobbyJoinRequest = new IPC_LobbyJoinRequest()
            {
                CallbackHandle = lobbyJoinRequest.CallbackHandle,
                LobbyID = lobbyJoinRequest.LobbyID,
                SteamID = lobbyJoinRequest.SteamID
            };
            var message = CreateIPCMessage(LobbyJoinRequest, IPCMessageType.IPC_LobbyJoinResponse);
            SendIPCMessage(message);
        }

        internal static void SendLobbyRemove(ulong lobbyID)
        {
            var LobbyRemove = new IPC_LobbyRemove()
            {
                LobbyID = lobbyID
            };
            var message = CreateIPCMessage(LobbyRemove, IPCMessageType.IPC_LobbyRemove);
            SendIPCMessage(message);
        }

        internal static void SendLobbyDataUpdate(NET_LobbyDataUpdate lobbyDataUpdate)
        {
            var LobbyDataUpdate = new IPC_LobbyDataUpdate()
            { 
                SteamIDLobby = lobbyDataUpdate.SteamIDLobby,
                SteamIDMember = lobbyDataUpdate.SteamIDMember,
                SerializedLobby = lobbyDataUpdate.SerializedLobby
            };
            var message = CreateIPCMessage(LobbyDataUpdate, IPCMessageType.IPC_LobbyDataUpdate);
            SendIPCMessage(message);
        }

        internal static void SendLobbyGameserver(NET_LobbyGameserver lobbyGameserver)
        {
            var LobbyGameserver = new IPC_LobbyGameserver()
            {
                IP = lobbyGameserver.IP,
                Port = lobbyGameserver.Port,
                LobbyID = lobbyGameserver.LobbyID,
                SteamID = lobbyGameserver.SteamID
            };
            var message = CreateIPCMessage(LobbyGameserver, IPCMessageType.IPC_LobbyGameserver);
            SendIPCMessage(message);
        }

        public static void SendModifyFileLog(Game game)
        {
            var ModifyFileLog = new IPC_ModifyFileLog()
            {
                Enabled = game.LogToFile
            };
            var message = CreateIPCMessage(ModifyFileLog, IPCMessageType.IPC_ModifyFileLog);
            SendIPCMessage(message);
        }

        public static void SendUpdatedUsers()
        {
            var UsersResponse = new IPC_UsersResponse()
            {
                Users = UserManager.Users
            };
            var welcome = CreateIPCMessage(UsersResponse, IPCMessageType.IPC_UsersResponse);
            SendIPCMessage(welcome);
        }

        public static void SendUpdatedLobbies(uint AppID = 0)
        {
            var LobbiesResponse = new IPC_LobbiesResponse()
            {
                Lobbies = AppID == 0 ? LobbyManager.Lobbies : LobbyManager.GetLobbies(AppID)
            };
            var welcome = CreateIPCMessage(LobbiesResponse, IPCMessageType.IPC_LobbiesResponse);
            SendIPCMessage(welcome);
        }


        private static void Write(object msg)
        {
            if (LastMessage != msg.ToString())
            {
                Log.Write("IPCManager", msg);
                LastMessage = msg.ToString();
            }
        }
    }
}