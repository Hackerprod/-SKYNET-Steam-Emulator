using System;
using System.Drawing;
using SKYNET.Client;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Network.Types;
using SKYNET.Steamworks;
using SKYNET.Types;
using SKYNET.WEB.Types;

namespace SKYNET.Network
{
    public partial class NETProcessor : INETMessages
    {
        internal static void Process(NETMessage message)
        {
            switch (message.MessageType)
            {
                case NETMessageType.NET_CreateAccountResponse:
                    ProcessCreateAccountResponse(message);
                    break;
                case NETMessageType.NET_AuthResponse:
                    ProcessAuthResponse(message);
                    break;
                case NETMessageType.NET_ChatMessage:
                    ProcessChatMessage(message);
                    break;
                case NETMessageType.NET_PrivateChatMessage:
                    ProcessPrivateChatMessage(message);
                    break;
                case NETMessageType.NET_UpdateUserResponse:
                    ProcessUpdateUserResponse(message);
                    break;
                case NETMessageType.NET_UserInfoResponse:
                    ProcessUserInfoResponse(message);
                    break;
                case NETMessageType.NET_RichPresenceUpdated:
                    ProcessRichPresenceUpdated(message);
                    break;
                case NETMessageType.NET_GameOpened:
                    ProcessGameOpened(message);
                    break;
                default:
                    Write($"Not implemented message type {message.MessageType}");
                    break;
            }
        }

        private static void ProcessGameOpened(NETMessage message)
        {
            var GameOpened = message.Deserialize<NET_GameOpened>();
            if (GameOpened == null) return;
            GameManager.InvokeUserGameOpened(GameOpened);
        }

        private static void ProcessRichPresenceUpdated(NETMessage message)
        {
            var RichPresenceUpdated = message.Deserialize<NET_RichPresenceUpdated>();
            if (RichPresenceUpdated == null) return;
            var User = UserManager.GetUser(RichPresenceUpdated.AccountID);
            User.RichPresence = RichPresenceUpdated.RichPresence;
            IPCManager.SendUpdatedUsers();
        }

        private static void ProcessUserInfoResponse(NETMessage message)
        {
            var UserInfoResponse = message.Body.Deserialize<NET_UserInfoResponse>();
            if (UserInfoResponse == null) return;
            var WUserInfoResponse = UserInfoResponse.CopyTo<WEB_UserInfoResponse>();
            WebManager.Send(WUserInfoResponse, WEBMessageType.WEB_UserInfoResponse);
        }

        private static void ProcessChatMessage(NETMessage message)
        {
            var ChatMessage = message.Body.Deserialize<NET_ChatMessage>();
            if (ChatMessage == null) return;
            var WChatMessage = ChatMessage.CopyTo<WEB_ChatMessage>();
            WebManager.Send(WChatMessage, WEBMessageType.WEB_ChatMessage);
        }

        private static void ProcessPrivateChatMessage(NETMessage message)
        {
            var PrivateChatMessage = message.Body.Deserialize<NET_PrivateChatMessage>();
            if (PrivateChatMessage == null) return;
            var WPrivateChatMessage = PrivateChatMessage.CopyTo<WEB_PrivateChatMessage>();
            WebManager.Send(WPrivateChatMessage, WEBMessageType.WEB_PrivateChatMessage);

        }

        private static void ProcessUpdateUserResponse(NETMessage message)
        {
            var UpdateUserResponse = message.Body.Deserialize<NET_UpdateUserResponse>();
            if (UpdateUserResponse == null) return;

            string URL = $"http://{Settings.ServerIP}:27080/Images/AvatarCache/{UpdateUserResponse.AvatarName}.jpg";
            var UserUpdated = new WEB_UserUpdated()
            {
                AllowRemoteAccess = Settings.AllowRemoteAccess,
                DeviceInSelected = Settings.InputDeviceID,
                Language = Settings.Language,
                ShowDebugConsole = Settings.ShowDebugConsole,
                PersonaName = UpdateUserResponse.PersonaName,
                AvatarURL = URL
            };

            SteamClient.PersonaName = UpdateUserResponse.PersonaName;
            if (UserManager.RequestAvatar(URL, out Bitmap avatar))
            {
                SteamClient.Avatar = avatar;
            }

            WebManager.Send(UserUpdated, WEBMessageType.WEB_UserUpdated);
        }

        private static void ProcessCreateAccountResponse(NETMessage message)
        {
            var CreateAccountResponse = message.Body.Deserialize<NET_CreateAccountResponse>();
            if (CreateAccountResponse == null) return;
            var W_CreateAccountResponse = CreateAccountResponse.CopyTo<WEB_CreateAccountResponse>();
            if (W_CreateAccountResponse == null) return;
            WebManager.Send(W_CreateAccountResponse, WEBMessageType.WEB_CreateAccountResponse);
        }

        private static void ProcessAuthResponse(NETMessage message)
        {
            var AuthResponse = message.Body.Deserialize<NET_AuthResponse>();
            if (AuthResponse == null) return;

            SteamClient.AccountID = AuthResponse.AccountID;
            SteamClient.AccountName = AuthResponse.AccountName;
            SteamClient.PersonaName = AuthResponse.PersonaName;
            SteamClient.SteamID = new CSteamID(AuthResponse.AccountID); 

            try
            {
                if (UserManager.RequestAvatar(AuthResponse.AccountID, out Bitmap avatar))
                {
                    SteamClient.Avatar = avatar;
                }
                else
                {
                    var Image = ImageHelper.GetDesktopWallpaper(true);
                    Image = ImageHelper.Resize(Image, 200, 200);
                    SendUpdateAvatar(Image);
                    SteamClient.Avatar = Image;
                }
            }
            catch 
            {
            }

            UserManager.AddOrUpdateUser(AuthResponse.AccountID, AuthResponse.PersonaName);

            var W_AuthResponse = AuthResponse.CopyTo<WEB_AuthResponse>();
            if (W_AuthResponse == null) return;

            W_AuthResponse.Language = SteamClient.Language;
            W_AuthResponse.DeviceInSelected = SteamClient.InputDeviceID;
            W_AuthResponse.ShowDebugConsole = Settings.ShowDebugConsole;
            W_AuthResponse.AllowRemoteAccess = Settings.AllowRemoteAccess;

            WebManager.Send(W_AuthResponse, WEBMessageType.WEB_AuthResponse);
        }

        private static void Write(object msg)
        {
            Log.Write("NETProcessor", msg);
        }
    }
}