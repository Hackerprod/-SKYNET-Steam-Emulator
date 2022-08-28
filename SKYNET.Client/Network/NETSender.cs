using SKYNET.Client;
using SKYNET.Helpers;
using SKYNET.IPC.Types;
using SKYNET.Managers;
using SKYNET.Network.Types;
using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Drawing;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SKYNET.Network
{
    public partial class NETProcessor : INETMessages
    {
        public static void SendGetRichPresence(ulong friendSteamID, string key)
        {
            var GetRichPresence = new NET_GetRichPresence()
            {
                AccountID = friendSteamID.GetAccountID()
            };
            Send(GetRichPresence, NETMessageType.NET_GetRichPresence);
        }


        public static void SendRichPresence(string key, string value)
        {
            NET_SetRichPresence RichPresence = new NET_SetRichPresence()
            {
                AccountID = SteamClient.AccountID,
                Key = key,
                Value = value
            };
            Send(RichPresence, NETMessageType.NET_SetRichPresence);
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
                Send(LobbyChatUpdate, NETMessageType.NET_LobbyChatUpdate);
            }
        }

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
                        Send(LobbyGameserver, NETMessageType.NET_LobbyGameserver);
                    }
                }
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

            Send(UserDataUpdated, NETMessageType.NET_UserDataUpdated);
        }

        public static void SendUpdateAvatar(Bitmap image)
        {
            var UpdateAvatar = new NET_UpdateAvatarRequest()
            {
                AvatarBase64 = ImageHelper.ImageToBase64(image),
            };

            Send(UpdateAvatar, NETMessageType.NET_UpdateAvatarRequest);
        }

        public static void SendLobbyCreated(SteamLobby lobby)
        {
            var LobbyCreated = new NET_LobbyCreated()
            {
                SteamID = (ulong)SteamClient.SteamID,
                SerializedLobby = lobby.Serialize()
            };
            Send(LobbyCreated, NETMessageType.NET_LobbyCreated);
        }

        public static void SendChatMessage(string message)
        {
            var ChatMessage = new NET_ChatMessage()
            {
                ID = Common.GetRandom(),
                SenderAccountID = SteamClient.AccountID,
                PersonaName = SteamClient.PersonaName,
                Message = message
            };
            Send(ChatMessage, NETMessageType.NET_ChatMessage);
        }

        public static void SendUpdateLobby(SteamLobby lobby, bool IncludeOwner)
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
                Send(LobbyUpdate, NETMessageType.NET_LobbyUpdate);
            }
        }

        public static void ProcessLobbyUpdate(NETMessage message, Socket socket)
        {
            var LobbyUpdate = message.Body.Deserialize<NET_LobbyUpdate>();
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

                Send(JoinRequest, NETMessageType.NET_LobbyJoinRequest);
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

                Send(JoinRequest, NETMessageType.NET_LobbyJoinRequest);
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        public static void SendLobbyDataUpdate(IPC_LobbyDataUpdate LobbyDataUpdate)
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
                Send(NETLobbyDataUpdate, NETMessageType.NET_LobbyDataUpdate);
            }
        }

        public static void SendLobbyListRequest(IPC_LobbyListRequest lobbyListRequest)
        {
            var LobbyListRequest = new NET_LobbyListRequest()
            {
                AppID = lobbyListRequest.AppID,
                RequestID = lobbyListRequest.RequestID
            };
            Send(LobbyListRequest, NETMessageType.NET_LobbyListRequest);
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

                Send(LobbyLeave, NETMessageType.NET_LobbyLeave);
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
                Send(lobbyDataUpdate, NETMessageType.NET_LobbyDataUpdate);
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

                        Send(lobbyRemove, NETMessageType.NET_LobbyRemove);
                    }
                }
            }
        }

        public static async void Send(NET_Base message, NETMessageType msgType)
        {
            NETMessage NETMessage = new NETMessage((ulong)SteamClient.SteamID, msgType, message);

            if (NetworkManager.WebSocketClient.Connected)
            {
                NetworkManager.WebSocketClient.Send(NETMessage);
            }
            else
            {
                NetworkManager.WebSocketClient.Connect();

                await Task.Delay(500);

                if (NetworkManager.WebSocketClient.Connected)
                {
                    NetworkManager.WebSocketClient.Send(NETMessage);
                }
            }
        }
    }
}
