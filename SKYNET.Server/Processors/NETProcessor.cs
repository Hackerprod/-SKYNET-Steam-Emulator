using System;
using System.IO;
using System.Net;
using SKYNET.DB;
using SKYNET.Helpers;
using SKYNET.Interfaces;
using SKYNET.Managers;
using SKYNET.Network;
using SKYNET.Network.Types;
using SKYNET.Types;

namespace SKYNET.Processors
{
    public class NETProcessor
    {
        public static void Process(IConnection connection, NETMessage message)
        {
            Write("Received NET message type " + message.MessageType);
            switch (message.MessageType)
            {
                case NETMessageType.NET_CreateAccountRequest:
                    ProcessCreateAccount(message, connection);
                    break;
                case NETMessageType.NET_AuthRequest:
                    ProcessAuthRequest(message, connection);
                    break;
                case NETMessageType.NET_ChatMessage:
                    ProcessChatMessage(message, connection);
                    break;
                case NETMessageType.NET_PrivateChatMessage:
                    ProcessPrivateChatMessage(message, connection);
                    break;
                case NETMessageType.NET_UpdateUserRequest:
                    ProcessUpdateUserRequest(message, connection);
                    break;
                case NETMessageType.NET_UpdateAvatarRequest:
                    ProcessUpdateAvatarRequest(message, connection);
                    break;
                case NETMessageType.NET_UserInfoRequest:
                    ProcessUserInfoRequest(message, connection);
                    break;
                case NETMessageType.NET_GetRichPresence:
                    ProcessGetRichPresence(message, connection);
                    break;
                case NETMessageType.NET_SetRichPresence:
                    ProcessSetRichPresence(message, connection);
                    break;
                case NETMessageType.NET_SetAchievement:
                    ProcessSetAchievement(message, connection);
                    break;
                case NETMessageType.NET_SetLeaderboard:
                    ProcessSetLeaderboard(message, connection);
                    break;
                case NETMessageType.NET_SetPlayerStat:
                    ProcessSetPlayerStat(message, connection);
                    break;
                case NETMessageType.NET_UpdateAchievement:
                    ProcessUpdateAchievement(message, connection);
                    break;
                case NETMessageType.NET_GameOpened:
                    ProcessGameOpened(message, connection);
                    break;
                case NETMessageType.NET_GameClosed:
                    ProcessGameClosed(message, connection);
                    break;
                default:
                    Write($"Not implemented message type {message.MessageType}");
                    break;
            }
        }

        private static void ProcessGameOpened(NETMessage message, IConnection connection)
        {
            var GameOpened = message.Deserialize<NET_GameOpened>();
            if (GameOpened == null) return;
            UserManager.SetPlayingState(GameOpened.AccountID, GameOpened.AppID);

            foreach (var friendSteamID in UserManager.GetFriends(message.SteamID))
            {
                var IConnection = ConnectionsManager.Get(friendSteamID);
                if (IConnection != null)
                {
                    Send(GameOpened, NETMessageType.NET_GameOpened, IConnection);
                }
            }
        }

        private static void ProcessGameClosed(NETMessage message, IConnection connection)
        {
            var GameClosed = message.Deserialize<NET_GameClosed>();
            if (GameClosed == null) return;
            UserManager.SetPlayingState(GameClosed.AccountID, 0);
        }

        private static void ProcessUpdateAchievement(NETMessage message, IConnection connection)
        {
            var UpdateAchievement = message.Deserialize<NET_UpdateAchievement>();
            if (UpdateAchievement == null) return;
            StatsDB.UpdateAchievement(message.SteamID, UpdateAchievement.Achievement);
        }

        private static void ProcessSetPlayerStat(NETMessage message, IConnection connection)
        {
            var SetPlayerStat = message.Deserialize<NET_SetPlayerStat>();
            if (SetPlayerStat == null) return;
            StatsDB.SetPlayerStat(message.SteamID, SetPlayerStat.PlayerStat);
        }

        private static void ProcessSetLeaderboard(NETMessage message, IConnection connection)
        {
            var SetLeaderboard = message.Deserialize<NET_SetLeaderboard>();
            if (SetLeaderboard == null) return;
            StatsDB.SetLeaderboard(message.SteamID, SetLeaderboard.Leaderboard);
        }

        private static void ProcessSetAchievement(NETMessage message, IConnection connection)
        {
            var SetAchievement = message.Deserialize<NET_SetAchievement>();
            if (SetAchievement == null) return;
            StatsDB.SetAchievement(message.SteamID, SetAchievement.Achievement);
        }

        private static void ProcessGetRichPresence(NETMessage message, IConnection connection)
        {
            var GetRichPresence = message.Deserialize<NET_GetRichPresence>();
            if (GetRichPresence == null) return;
            var RichPresence = new NET_RichPresenceUpdated()
            {
                AccountID = GetRichPresence.AccountID,
                RichPresence = UserManager.GetRichPresence(GetRichPresence.AccountID)
            };
            UserManager.RegisterToRichPresence(GetRichPresence.AccountID, message.SteamID.GetAccountID());
            Send(RichPresence, NETMessageType.NET_RichPresenceUpdated, connection);
        }

        private static void ProcessSetRichPresence(NETMessage message, IConnection connection)
        {
            var SetRichPresence = message.Deserialize<NET_SetRichPresence>();
            if (SetRichPresence == null) return;
            UserManager.SetRichPresence(SetRichPresence.AccountID, SetRichPresence.Key, SetRichPresence.Value);

            var RichPresenceUpdated = SetRichPresence.CopyTo<NET_RichPresenceUpdated>();

            var RegisteredClients = UserManager.GetRegisteredRichPresence(SetRichPresence.AccountID);
            foreach (var clientAccountID in RegisteredClients)
            {
                var registeredConnection = ConnectionsManager.Get(clientAccountID.ToSteamID());
                if (registeredConnection != null)
                {
                    Send(RichPresenceUpdated, NETMessageType.NET_RichPresenceUpdated, registeredConnection);
                }
            }
            Send(RichPresenceUpdated, NETMessageType.NET_RichPresenceUpdated, connection);
        }

        private static void ProcessUserInfoRequest(NETMessage message, IConnection connection)
        {
            var UserInfoRequest = message.Deserialize<NET_UserInfoRequest>();
            if (UserInfoRequest == null) return;

            var UserInfoResponse = new NET_UserInfoResponse();

            if (UserInfoRequest == null)
            {
                UserInfoResponse.Response = NET_UserInfoResponse.UserInfoResponseType.UnknownError;
                Send(UserInfoResponse, NETMessageType.NET_UserInfoResponse, connection);
                return;
            }

            var User = UserManager.Get(UserInfoRequest.AccountID);
            if (User == null)
            {
                UserInfoResponse.Response = NET_UserInfoResponse.UserInfoResponseType.AccountNotFound;
                Send(UserInfoResponse, NETMessageType.NET_UserInfoResponse, connection);
                return;
            }

            UserInfoResponse.Response = NET_UserInfoResponse.UserInfoResponseType.Success;
            UserInfoResponse.AccountID = User.AccountID;
            UserInfoResponse.PersonaName = User.PersonaName;
            UserInfoResponse.Playing = User.PlayingAppID;
            UserInfoResponse.LastLogon = User.LastLogon;

            Send(UserInfoResponse, NETMessageType.NET_UserInfoResponse, connection);
        }

        private static void ProcessUpdateUserRequest(NETMessage message, IConnection connection)
        {
            var UpdateUserRequest = message.Deserialize<NET_UpdateUserRequest>();
            if (UpdateUserRequest == null) return;

            var User = UserManager.Get(message.SteamID);
            if (User == null) return;

            var UpdateUserResponse = new NET_UpdateUserResponse()
            {
                PersonaName = User.PersonaName,
                AvatarName = User.AccountID.ToString()
            };

            if (UserManager.IsValidPersonaName(UpdateUserRequest.PersonaName))
            {
                UpdateUserResponse.PersonaName = UpdateUserRequest.PersonaName;
            }

            if (!string.IsNullOrEmpty(UpdateUserRequest.AvatarBase64))
            {
                var Bitmap = ImageHelper.ImageFromBase64(UpdateUserRequest.AvatarBase64);
                string AvatarPath = Path.Combine(Common.GetPath(), "Data", "Images", "AvatarCache", message.SteamID.GetAccountID() + ".jpg");
                ImageHelper.ToFile(AvatarPath, Bitmap);
                string avatarName = User.AccountID + "_" + Common.GetRandomString(6);
                WebServer.AddOrUpdateAvatarName(User.AccountID, avatarName);
                UpdateUserResponse.AvatarName = avatarName;
            }
            Send(UpdateUserResponse, NETMessageType.NET_UpdateUserResponse, connection);
        }

        private static void ProcessUpdateAvatarRequest(NETMessage message, IConnection connection)
        {
            var UpdateAvatarRequest = message.Deserialize<NET_UpdateAvatarRequest>();
            if (UpdateAvatarRequest == null) return;
            if (string.IsNullOrEmpty(UpdateAvatarRequest.AvatarBase64)) return;

            var Bitmap = ImageHelper.ImageFromBase64(UpdateAvatarRequest.AvatarBase64);
            string AvatarPath = Path.Combine(Common.GetPath(), "Data", "Images", "AvatarCache", message.SteamID.GetAccountID() + ".jpg");
            ImageHelper.ToFile(AvatarPath, Bitmap);

            var NET_UpdateAvatarResponse = new NET_UpdateAvatarResponse()
            {
                 AvatarBase64 = UpdateAvatarRequest.AvatarBase64
            };
            Send(NET_UpdateAvatarResponse, NETMessageType.NET_UpdateAvatarResponse, connection);
        }

        private static void ProcessPrivateChatMessage(NETMessage message, IConnection connection)
        {
            var PrivateChatMessage = message.Deserialize<NET_PrivateChatMessage>();
            if (PrivateChatMessage == null) return;
            var TargetID = PrivateChatMessage.TargetAccountID.ToSteamID();
            ConnectionsManager.Send(TargetID, PrivateChatMessage, NETMessageType.NET_PrivateChatMessage);
        }

        private static void ProcessChatMessage(NETMessage message, IConnection connection)
        {
            var ChatMessage = message.Deserialize<NET_ChatMessage>();
            if (ChatMessage == null) return;
            ConnectionsManager.BroadcastMessage(ChatMessage, NETMessageType.NET_ChatMessage);
        }

        private static void ProcessCreateAccount(NETMessage message, IConnection connection)
        {
            try
            {
                var createRequest = message.Deserialize<NET_CreateAccountRequest>();
                var createResponse = new NET_CreateAccountResponse()
                {
                     CreateAccountResult = NET_CreateAccountResponse.Result.SUCCESS
                };

                if (createRequest == null)
                {
                    createResponse.CreateAccountResult = NET_CreateAccountResponse.Result.ERROR;
                    Send(createResponse, NETMessageType.NET_CreateAccountResponse, connection);
                    return;
                }

                var user = UserManager.GetByAccountName(createRequest.AccountName);
                if (user != null)
                {
                    createResponse.CreateAccountResult = NET_CreateAccountResponse.Result.ACCOUNTEXISTS;
                    Write($"Error creating new user, the user {createRequest.AccountName} exists.");
                    Send(createResponse, NETMessageType.NET_CreateAccountResponse, connection);
                    return;
                }

                user = SteamPlayer.CreateOne(createRequest.AccountName, createRequest.Password);
                UserManager.CreateAccount(user);

                createResponse.AccountName = user.AccountName;
                createResponse.SteamID =  user.AccountID.ToSteamID();
                createResponse.AccountID = user.AccountID;

                Write($"Account for user {user.AccountName} created successfully.");
                Send(createResponse, NETMessageType.NET_CreateAccountResponse, connection);
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        private static void ProcessAuthRequest(NETMessage message, IConnection connection)
        {
            NET_AuthRequest AuthRequest = message.Deserialize<NET_AuthRequest>();
            NET_AuthResponse AuthResponse = new NET_AuthResponse()
            {
                Response = NET_AuthResponse.AuthResponseType.Success
            };

            if (AuthRequest == null)
            {
                AuthResponse.Response = NET_AuthResponse.AuthResponseType.UnknownError;
                Send(AuthResponse, NETMessageType.NET_AuthResponse, connection);
                return;
            }

            SteamPlayer user = UserManager.GetByAccountName(AuthRequest.Username);
            if (user == null)
            {
                AuthResponse.Response = NET_AuthResponse.AuthResponseType.AccountNotFound;
                Write($"Account {AuthRequest.Username} not found");
                Send(AuthResponse, NETMessageType.NET_AuthResponse, connection);
                return;
            }

            if (AuthRequest.Password != user.Password)
            {
                AuthResponse.Response = NET_AuthResponse.AuthResponseType.PasswordWrong;
                Write($"Invalid authentication from account {AuthRequest.Username}");
                Send(AuthResponse, NETMessageType.NET_AuthResponse, connection);
                return;
            }

            if (ConnectionsManager.IsLoggedIn(user.AccountID, out IPAddress ipAlreadyLoggedIn))
            {
                AuthResponse.Response = NET_AuthResponse.AuthResponseType.AlreadyConnected;
                Write($"User {user.AccountName} is connected using IP: {ipAlreadyLoggedIn}, and someone is trying to use his account from IP: {connection.RemoteEndPoint.Address}.");
                Send(AuthResponse, NETMessageType.NET_AuthResponse, connection);
                return;
            }

            uint SetLastLogOn = DateTime.Now.ToTimestamp();
            UserManager.SetPlayingState(user.AccountID, 0);
            UserManager.SetLastLogOn(user.AccountID, SetLastLogOn);

            AuthResponse.AccountName = user.AccountName;
            AuthResponse.PersonaName = user.PersonaName;
            AuthResponse.AccountID = user.AccountID;
            AuthResponse.Wallet = user.Wallet;

            Write($"Account {AuthRequest.Username} successfully authenticated");
            Send(AuthResponse, NETMessageType.NET_AuthResponse, connection);
        }

        private static void Send(NET_Base message, NETMessageType msgType, IConnection connection)
        {
            NETMessage NETMessage = new NETMessage(msgType, message);
            connection.Send(NETMessage);
        }

        private static void Write(object msg)
        {
            Log.Write("NET", msg);
        }
    }
}
