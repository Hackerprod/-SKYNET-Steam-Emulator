using SKYNET.Client;
using SKYNET.Common;
using SKYNET.Helper;
using SKYNET.IPC;
using SKYNET.IPC.Types;
using SKYNET.Network;
using SKYNET.Network.Packets;
using SKYNET.Types;
using System;
using System.Drawing;
using System.IO;

namespace SKYNET.Managers
{
    public class IPCManager
    {
        private const ulong IPC_ToServer = 0;
        private const ulong IPC_Broadcast = 1;
        private static PipeServer<IPCMessage> server;
        public static void Initialize()
        {
            server = new PipeServer<IPCMessage>("SKYNET");
            server.ClientConnected += OnClientConnected; 
            server.ClientDisconnected += OnClientDisconnected;
            server.MessageReceived += OnMessageReceived;
            server.ExceptionOccurred += OnExceptionOccurred;
            server.StartAsync();
        }

        private static void OnClientConnected(object sender, ConnectionEventArgs<IPCMessage> args)
        {
            Write($"Client {args.Connection.PipeName} is now connected!");
        }

        private static void OnMessageReceived(object sender, ConnectionMessageEventArgs<IPCMessage> e)
        {
            Log.Write("IPCManager", $"Received IPC message {(IPCMessageType)e.Message.MessageType}");

            switch ((IPCMessageType)e.Message.MessageType)
            {
                case IPCMessageType.IPC_ClientHello:
                    Process_ClientHello(e.Connection, e.Message);
                    break;
                case IPCMessageType.IPC_AddOrUpdateUser:
                    // TODO: Only receive in game IPC client
                    break;
                case IPCMessageType.IPC_AvatarRequest:
                    Process_AvatarRequest(e.Connection, e.Message);
                    break;
                case IPCMessageType.IPC_AvatarResponse:
                    // TODO: Only receive in game IPC client
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
                case IPCMessageType.IPC_LobbyListResponse:
                    // TODO: Only receive in game IPC client
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
                case IPCMessageType.IPC_GCMessageRequest:
                    break;
                case IPCMessageType.IPC_GCMessageResponse:
                    break;
                default:
                    break;
            }
        }

        private static void Process_ClientHello(PipeConnection<IPCMessage> connection, IPCMessage message)
        {
            var ClientHello = message.ParsedBody.FromJson<IPC_ClientHello>();

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
            };

            if (ClientHello != null)
            {
                var AppID = ClientHello.AppID;
                var Game = GameManager.GetGame(AppID);
                if (Game != null)
                {
                    GameManager.InvokeGameLaunched(Game, ClientHello.ProcessID, connection.PipeName);

                    ClientWelcome.LogToFile = Game.LogToFile;
                    ClientWelcome.LogToConsole = Game.LogToConsole;
                    ClientWelcome.RunCallbacks = Game.RunCallbacks;
                    ClientWelcome.ISteamHTTP = Game.ISteamHTTP;
                }
            }
            var welcome = CreateIPCMessage(ClientWelcome, IPCMessageType.IPC_ClientWelcome);
            connection.WriteAsync(welcome);

            // TODO: Send user avatar
            var imageBytes = ImageHelper.ImageToBytes(SteamClient.Avatar);
            var hexAvatar = Convert.ToBase64String(imageBytes);
            var AvatarResponse = new IPC_AvatarResponse()
            {
                AccountID = SteamClient.AccountID,
                HexAvatar = hexAvatar
            };
            var AvatarMessage = CreateIPCMessage(AvatarResponse, IPCMessageType.IPC_AvatarResponse);
            connection.WriteAsync(AvatarMessage);
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
            var P2PPacket = message.ParsedBody.FromJson<IPC_P2PPacket>();
            if (P2PPacket != null)
                P2PNetworking.SendP2PTo(message.To, P2PPacket);
        }
        #endregion

        private static void Process_LobbyGameserver(IPCMessage message)
        {
            var LobbyGameserver = message.ParsedBody.FromJson<IPC_LobbyGameserver>();
            if (LobbyGameserver != null)
                NetworkManager.SendLobbyGameserver(LobbyGameserver);
        }

        private static void Process_UserDataUpdated(IPCMessage message)
        {
            var UserDataUpdated = message.ParsedBody.FromJson<IPC_UserDataUpdated>();
            if (UserDataUpdated != null)
                NetworkManager.SendUserDataUpdated(UserDataUpdated);
        }

        private static void Process_AvatarRequest(PipeConnection<IPCMessage> connection, IPCMessage message)
        {
            var AvatarRequest = message.ParsedBody.FromJson<IPC_AvatarRequest>();
            if (AvatarRequest != null)
            {
                try
                {
                    // TODO: Response avatar stored in cache
                    string AvatarCachePath = Path.Combine(modCommon.GetPath(), "Data", "Images", "AvatarCache", AvatarRequest.AccountID + ".jpg");
                    if (File.Exists(AvatarCachePath))
                    {
                        var Avatar = ImageHelper.FromFile(AvatarCachePath);
                        var imageBytes = ImageHelper.ImageToBytes(Avatar);
                        var hexAvatar = Convert.ToBase64String(imageBytes);
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
            var imageBytes = ImageHelper.ImageToBytes(Avatar);
            var hexAvatar = Convert.ToBase64String(imageBytes);
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
            var LobbyChatUpdate = message.ParsedBody.FromJson<IPC_LobbyChatUpdate>();
            if (LobbyChatUpdate != null)
                NetworkManager.SendLobbyChatUpdate(message.To, LobbyChatUpdate);
        }

        private static void Process_LobbyListRequest(IPCMessage message)
        {
            var LobbyListRequest = message.ParsedBody.FromJson<IPC_LobbyListRequest>();
            if (LobbyListRequest != null)
                NetworkManager.SendLobbyListRequest(LobbyListRequest);
        }

        private static void Process_LobbyLeave(IPCMessage message)
        {
            var LobbyRemove = message.ParsedBody.FromJson<IPC_LobbyRemove>();
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
            var LobbyRemove = message.ParsedBody.FromJson<IPC_LobbyRemove>();
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
            var LobbyCreate = message.ParsedBody.FromJson<IPC_LobbyCreate>();
            SteamLobby lobby = LobbyCreate.SerializedLobby.FromJson<SteamLobby>();
            if (lobby != null)
            {
                LobbyManager.Create(lobby);
                // TODO: Send lobby to steam server
            }
        }

        private static void Process_LobbyJoinRequest(IPCMessage message)
        {
            var LobbyJoinRequest = message.ParsedBody.FromJson<IPC_LobbyJoinRequest>();
            NetworkManager.SendLobbyJoinRequest(LobbyJoinRequest);
        }

        private static void Process_LobbyDataUpdate(IPCMessage message)
        {
            var LobbyDataUpdate = message.ParsedBody.FromJson<IPC_LobbyDataUpdate>();
            NetworkManager.SendLobbyDataUpdate(LobbyDataUpdate);
            SteamLobby lobby = LobbyDataUpdate.SerializedLobby.FromJson<SteamLobby>();
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
            Log.Write("IPCManager", "Exception Occurred!!! " + e);
        }

        public static void AddOrUpdateUser(uint accountID, string personaName, uint appID, string iPAddress)
        {
            var update = new IPC_AddOrUpdateUser()
            {
                AccountID = accountID,
                PersonaName = personaName,
                AppID = appID,
                IPAddress = iPAddress
            };
            var IPCMessage = CreateIPCMessage(update, IPCMessageType.IPC_AddOrUpdateUser);
            SendIPCMessage(IPCMessage);
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

        public static void UpdateUserStatus(NET_UserDataUpdated statusChanged, string iPAddress)
        {
            var UserDataUpdated = new IPC_UserDataUpdated()
            {
                LobbyID = statusChanged.LobbyID,
                AccountID = statusChanged.AccountID,
                PersonaName = statusChanged.PersonaName,
                IPAddress = iPAddress
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

        private static IPCMessage CreateIPCMessage(IPC_MessageBase Base, IPCMessageType type)
        {
            var message = new IPCMessage()
            {
                MessageType = (int)type,
                ParsedBody = Base.ToJson()
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

        private static void Write(object msg)
        {
            Log.Write("IPCManager", msg);
        }
    }
}