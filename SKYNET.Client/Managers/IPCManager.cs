using SKYNET.Client;
using SKYNET.Common;
using SKYNET.IPC;
using SKYNET.IPC.Types;
using SKYNET.Network.Packets;
using SKYNET.Types;
using System;
using System.Drawing;

namespace SKYNET.Managers
{
    public class IPCManager
    {
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

        private static void OnMessageReceived(object sender, ConnectionMessageEventArgs<IPCMessage> e)
        {
            Log.Write("IPCManager", $"MessageReceived {(IPCMessageType)e.Message.MessageType}");

            switch ((IPCMessageType)e.Message.MessageType)
            {
                case IPCMessageType.IPC_User:
                    break;
                case IPCMessageType.IPC_AddOrUpdateUser:
                    break;
                case IPCMessageType.IPC_AvatarRequest:
                    break;
                case IPCMessageType.IPC_AvatarResponse:
                    break;
                case IPCMessageType.IPC_UserDataUpdated:
                    break;
                case IPCMessageType.IPC_P2PPacket:
                    break;
                case IPCMessageType.IPC_LobbyListRequest:
                    break;
                case IPCMessageType.IPC_LobbyListResponse:
                    break;
                case IPCMessageType.IPC_LobbyJoinRequest:
                    break;
                case IPCMessageType.IPC_LobbyJoinResponse:
                    break;
                case IPCMessageType.IPC_LobbyDataUpdate:
                    Process_LobbyDataUpdate(e.Message);
                    break;
                case IPCMessageType.IPC_LobbyChatUpdate:
                    break;
                case IPCMessageType.IPC_LobbyLeave:
                    break;
                case IPCMessageType.IPC_LobbyCreate:
                    Process_LobbyCreate(e.Message);
                    break;
                case IPCMessageType.IPC_LobbyRemove:
                    break;
                case IPCMessageType.IPC_LobbyGameserver:
                    break;
                case IPCMessageType.IPC_GCMessageRequest:
                    break;
                case IPCMessageType.IPC_GCMessageResponse:
                    break;
                default:
                    break;
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

        private static void Process_LobbyDataUpdate(IPCMessage message)
        {
            var LobbyDataUpdate = message.ParsedBody.FromJson<IPC_LobbyDataUpdate>();
            NetworkManager.SendLobbyDataUpdate(message.To, LobbyDataUpdate);
            SteamLobby lobby = LobbyDataUpdate.SerializedLobby.FromJson<SteamLobby>();
            if (lobby != null)
            {
                LobbyManager.Update(lobby);
            }
        }

        private static void OnClientConnected(object sender, ConnectionEventArgs<IPCMessage> args)
        {
            Log.Write("IPCManager", $"Client {args.Connection.PipeName} is now connected!");

            IPC_User userData = new IPC_User()
            {
                AccountID = SteamClient.AccountID,
                PersonaName = SteamClient.PersonaName,
                Language = SteamClient.Language,
                GameServerID = SteamClient.SteamID_GS.AccountID,
                AppID = 0, 
            };
            var message = CreateIPCMessage(userData, IPCMessageType.IPC_User);

            args.Connection.WriteAsync(message);

            foreach (var user in UserManager.Users)
            {
                var _user = new IPC_AddOrUpdateUser()
                {
                    AccountID = user.AccountID,
                    PersonaName = user.PersonaName,
                    IPAddress = user.IPAddress
                };
                var updateMessage = CreateIPCMessage(userData, IPCMessageType.IPC_User);
                args.Connection.WriteAsync(updateMessage);
            }
        }

        private static void OnClientDisconnected(object sender, ConnectionEventArgs<IPCMessage> e)
        {
            Log.Write("IPCManager", $"Client {e.Connection.PipeName} disconnected");
        }

        private static void OnExceptionOccurred(object sender, ExceptionEventArgs e)
        {
            Log.Write("IPCManager", "Exception Occurred!!!");
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
    }
}