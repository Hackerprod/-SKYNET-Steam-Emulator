using SKYNET.Client;
using SKYNET.Common;
using SKYNET.Helper;
using SKYNET.Managers;
using SKYNET.Network;
using SKYNET.WEB.Types;
using System;

namespace SKYNET
{
    public class WebManager
    {
        public static event EventHandler<string> OnGameLaunch;

        public static void Initialize()
        {
            WebSocketProcessor.OnMessageReceived += Web_OnMessageReceived;
        }

        private static void Web_OnMessageReceived(object sender, WebMessage e)
        {
            Write($"Received WEB message {e.MessageType}");
            switch (e.MessageType)
            {
                case WEB_MessageType.WEB_AuthRequest:
                    ProcessAuthRequest(e);
                    break;
                case WEB_MessageType.WEB_CreateAccountRequest:
                    // TODO
                    break;
                case WEB_MessageType.WEB_GameListRequest:
                    ProcessGameListRequest(e);
                    break;
                case WEB_MessageType.WEB_GameAdded:
                    ProcessGameAdded(e);
                    break;
                case WEB_MessageType.WEB_GameUpdated:
                    ProcessGameUpdated(e);
                    break;
                case WEB_MessageType.WEB_GameRemoved:
                    ProcessGameRemoved(e);
                    break;
                case WEB_MessageType.WEB_GameLaunch:
                    ProcessGameLaunch(e);
                    break;
                case WEB_MessageType.WEB_GameInfoRequest:
                    ProcessGameInfoRequest(e);
                    break;
                case WEB_MessageType.WEB_UserInfoRequest:
                    ProcessUserInfoRequest(e);
                    break;
                case WEB_MessageType.WEB_ChatMessage:
                    // TODO
                    break;
                case WEB_MessageType.WEB_PrivateChatMessage:
                    // TODO
                    break;
            }
        }

        private static void ProcessAuthRequest(WebMessage e)
        {
            var AuthRequest = e.Deserialize<WEB_AuthRequest>();
            var AuthResponse = new WEB_AuthResponse();
            if (AuthRequest == null)
            {
                AuthResponse.Response = WEB_AuthResponseType.UnknownError;
                Send(AuthResponse, WEB_MessageType.WEB_AuthResponse);
            }
            else
            {
                Write($"Requesting Auth for {AuthRequest.Username} = {AuthRequest.Password}, [hackerprod:123]");

                //if (AuthRequest.Username.ToLower() != "hackerprod")
                //{
                //    AuthResponse.Response = WEB_AuthResponseType.AccountNotFound;
                //    Send(AuthResponse, WEB_MessageType.WEB_AuthResponse);
                //}
                //else if (AuthRequest.Password != "123")
                //{
                //    AuthResponse.Response = WEB_AuthResponseType.PasswordWrong;
                //    Send(AuthResponse, WEB_MessageType.WEB_AuthResponse);
                //}
                //else
                {
                    AuthResponse.Response = WEB_AuthResponseType.Success;

                    string hexAvatar = "";
                    if (SteamClient.Avatar != null)
                    {
                        var imageBytes = ImageHelper.ImageToBytes(SteamClient.Avatar);
                        hexAvatar = Convert.ToBase64String(imageBytes);
                    }

                    AuthResponse.UserInfo = new UserInfo()
                    {
                        PersonaName = SteamClient.PersonaName,
                        AccountID = SteamClient.AccountID,
                        Language = SteamClient.Language,
                        AvatarHex = hexAvatar
                    };
                    Send(AuthResponse, WEB_MessageType.WEB_AuthResponse);

                    var GameListResponse = new WEB_GameListResponse()
                    {
                        GameList = GameManager.GetGames()
                    };
                    Send(GameListResponse, WEB_MessageType.WEB_GameListResponse);
                }
            }
        }

        private static void ProcessGameListRequest(WebMessage e)
        {
            var GameListResponse = new WEB_GameListResponse()
            {
                GameList = GameManager.GetGames()
            };
            Send(GameListResponse, WEB_MessageType.WEB_GameListResponse);
        }

        private static void ProcessUserInfoRequest(WebMessage e)
        {
            // TODO?
            //var UserInfoRequest = e.Deserialize<WEB_UserInfoRequest>();
        }

        private static void ProcessGameInfoRequest(WebMessage e)
        {
            var GameInfoRequest = e.Deserialize<WEB_GameInfoRequest>();
            if (GameInfoRequest == null) return;
            var Game = GameManager.GetGame(GameInfoRequest.Guid);
            if (Game == null) return;
            if (GameInfoRequest.Minimal)
            {
                Write($"Requesting minimal game info for {Game.Name}, AppID {Game.AppID}");
            }
            else
                Write($"Requesting game info for {Game.Name}, AppID {Game.AppID}");
            // TODO
        }

        private static void ProcessGameLaunch(WebMessage e)
        {
            var GameLaunch = e.Deserialize<WEB_GameLaunch>();
            if (GameLaunch == null) return;
            OnGameLaunch?.Invoke(null, GameLaunch.Guid);
        }

        private static void ProcessGameRemoved(WebMessage e)
        {
            var GameRemoved = e.Deserialize<WEB_GameRemoved>();
            if (GameRemoved == null) return;
            GameManager.Remove(GameRemoved.Guid);
        }

        private static void ProcessGameUpdated(WebMessage e)
        {
            var GameUpdated = e.Deserialize<WEB_GameUpdated>();
            if (GameUpdated == null) return;
            GameManager.Update(GameUpdated.Game); 
        }

        private static void ProcessGameAdded(WebMessage e)
        {
            var GameAdded = e.Deserialize<WEB_GameAdded>();
            if (GameAdded == null) return;
            GameManager.AddGame(GameAdded.Game);
        }

        public static void Send(WEB_Base msgBase, WEB_MessageType Type)
        {
            Write($"Sending WEB message {Type}");
            WebMessage WebMessage = new WebMessage()
            {
                MessageType = Type,
                Body = msgBase.Serialize()
            };
            NetworkManager.WebClient.Send(WebMessage.Serialize());
        }

        private static void Write(object msg)
        {
            Log.Write("WebManager", msg);
        }
    }
}
