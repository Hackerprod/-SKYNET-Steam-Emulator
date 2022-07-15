using SKYNET.Client;
using SKYNET.Common;
using SKYNET.Helpers;
using SKYNET.Network;
using SKYNET.WEB.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SKYNET.Managers
{
    public class WebManager
    {
        public static event EventHandler<string> OnGameLaunch;

        public static void Initialize()
        {
            WebSocketProcessor.OnMessageReceived += Web_OnMessageReceived;
        }

        private static void Web_OnMessageReceived(object sender, WEBMessage e)
        {
            try
            {
                Write($"Received WEB message {e.MessageType}");
                switch (e.MessageType)
                {
                    case WEBMessageType.WEB_AuthRequest:
                        ProcessAuthRequest(e);
                        break;
                    case WEBMessageType.WEB_CreateAccountRequest:
                        // TODO
                        break;
                    case WEBMessageType.WEB_GameListRequest:
                        ProcessGameListRequest(e);
                        break;
                    case WEBMessageType.WEB_GameAdded:
                        ProcessGameAdded(e);
                        break;
                    case WEBMessageType.WEB_GameUpdated:
                        ProcessGameUpdated(e);
                        break;
                    case WEBMessageType.WEB_GameRemoved:
                        ProcessGameRemoved(e);
                        break;
                    case WEBMessageType.WEB_GameLaunch:
                        ProcessGameLaunch(e);
                        break;
                    case WEBMessageType.WEB_GameInfoRequest:
                        ProcessGameInfoRequest(e);
                        break;
                    case WEBMessageType.WEB_UserInfoRequest:
                        ProcessUserInfoRequest(e);
                        break;
                    case WEBMessageType.WEB_ChatMessage:
                        // TODO
                        break;
                    case WEBMessageType.WEB_PrivateChatMessage:
                        // TODO
                        break;
                    case WEBMessageType.WEB_GameOpenContainerFolder:
                        ProcessGameOpenContainerFolder(e);
                        break;
                    case WEBMessageType.WEB_GameOpenWithoutEmulation:
                        ProcessGameOpenWithoutEmulation(e);
                        break;
                    case WEBMessageType.WEB_FileInfoRequest:
                        ProcessFileInfoRequest(e);
                        break;
                    case WEBMessageType.WEB_GameOrderUpdated:
                        ProcessGameOrderUpdated(e);
                        break;
                    case WEBMessageType.WEB_OpenFileDialogRequest:
                        ProcessOpenFileDialogRequest();
                        break;
                    case WEBMessageType.WEB_GameDownloadCache:
                        ProcessGameDownloadCache(e);
                        break;
                }
            }
            catch (Exception ex)
            {
                Write($"Error proccessing WEB Message {ex}");
            }
        }

        private static void ProcessGameDownloadCache(WEBMessage e)
        {
            var GameDownloadCache = e.Deserialize<WEB_GameDownloadCache>();
            if (GameDownloadCache == null) return;
            var Game = GameManager.GetGame(GameDownloadCache.Guid);
            if (Game == null) return;
            StatsManager.DownloadAppCache(Game.AppID);
        }

        private static void ProcessOpenFileDialogRequest()
        {
            Thread s = new Thread(new ThreadStart(delegate
            {
                OpenFileDialog fileDialog = new OpenFileDialog()
                {
                    Filter = "exe file | *.exe",
                    Multiselect = false
                };
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    string FileName = fileDialog.FileName;
                    FileInfo info = new FileInfo(FileName);
                    var hexIcon = "";
                    try
                    {
                        var bitmap = (Bitmap)ImageHelper.IconFromFile(FileName);
                        var imageBytes = ImageHelper.ImageToBytes(bitmap);
                        hexIcon = Convert.ToBase64String(imageBytes);
                    }
                    catch { }

                    int posibleAppID = 0;
                    try
                    {
                        var PathDirectory = Directory.GetParent(FileName).ToString();
                        if (File.Exists(Path.Combine(PathDirectory, "steam_appid.txt")))
                        {
                            posibleAppID = int.Parse(File.ReadAllText(Path.Combine(PathDirectory, "steam_appid.txt")));
                        }
                    }
                    catch { }

                    var FileDialogResponse = new WEB_OpenFileDialogResponse()
                    {
                        FilePath = FileName,
                        Size = info.Length,
                        ImageHex = hexIcon,
                        AppID = posibleAppID
                    };
                    Send(FileDialogResponse, WEBMessageType.WEB_OpenFileDialogResponse);

                }
            }));
            s.SetApartmentState(ApartmentState.STA);
            s.Start();
        }

        private static void ProcessAuthRequest(WEBMessage e)
        {
            var AuthRequest = e.Deserialize<WEB_AuthRequest>();
            var AuthResponse = new WEB_AuthResponse();
            if (AuthRequest == null)
            {
                AuthResponse.Response = WEB_AuthResponseType.UnknownError;
                Send(AuthResponse, WEBMessageType.WEB_AuthResponse);
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
                        hexAvatar = ImageHelper.GetImageBase64(SteamClient.Avatar);
                    }

                    AuthResponse.UserInfo = new UserInfo()
                    {
                        PersonaName = SteamClient.PersonaName,
                        AccountID = SteamClient.AccountID,
                        Language = SteamClient.Language,
                        AvatarHex = hexAvatar
                    };
                    Send(AuthResponse, WEBMessageType.WEB_AuthResponse);

                    var GameListResponse = new WEB_GameListResponse()
                    {
                        GameList = GameManager.GetGames()
                    };
                    Send(GameListResponse, WEBMessageType.WEB_GameListResponse);
                }
            }
        }

        private static void ProcessGameListRequest(WEBMessage e)
        {
            var GameListResponse = new WEB_GameListResponse()
            {
                GameList = GameManager.GetGames()
            };
            Send(GameListResponse, WEBMessageType.WEB_GameListResponse);
        }

        private static void ProcessUserInfoRequest(WEBMessage e)
        {
            // TODO?
            //var UserInfoRequest = e.Deserialize<WEB_UserInfoRequest>();
        }

        private static void ProcessGameInfoRequest(WEBMessage e)
        {
            var GameInfoRequest = e.Deserialize<WEB_GameInfoRequest>();
            if (GameInfoRequest == null) return;
            var Game = GameManager.GetGame(GameInfoRequest.Guid);
            if (Game == null) return;
            {
                var FriendsPlaying = new List<WEB_GameInfoResponse.FriendPlaying>();
                try
                {
                    var Users = UserManager.GetFriends();
                    for (int i = 0; i < Users.Count; i++)
                    {
                        UserManager.GetAvatar(Users[i].SteamID, out var Avatar);
                        FriendsPlaying.Add(new WEB_GameInfoResponse.FriendPlaying()
                        {
                            AccountID = Users[i].AccountID,
                            PersonaName = Users[i].PersonaName,
                            AvatarHex = ImageHelper.GetImageBase64(Avatar),
                        });
                        if (i == 10) break;
                    }
                }
                catch { }

                var Response = new WEB_GameInfoResponse()
                {
                    LastPlayed = GameManager.GetLastPlayed(GameInfoRequest.Guid),
                    TimePlayed = GameManager.GetTimePlayed(GameInfoRequest.Guid),
                    UsersPlaying = GameManager.GetUsersPlaying(GameInfoRequest.Guid),
                    FriendsPlaying = FriendsPlaying,
                    Playing = GameManager.IsPlaying(GameInfoRequest.Guid),
                };

                Send(Response, WEBMessageType.WEB_GameInfoResponse);
            }
        }

        private static void ProcessGameLaunch(WEBMessage e)
        {
            var GameLaunch = e.Deserialize<WEB_GameLaunch>();
            if (GameLaunch == null || string.IsNullOrEmpty(GameLaunch.Guid)) return;
            OnGameLaunch?.Invoke(null, GameLaunch.Guid);
        }

        private static void ProcessGameRemoved(WEBMessage e)
        {
            var GameRemoved = e.Deserialize<WEB_GameRemoved>();
            if (GameRemoved == null) return;
            GameManager.Remove(GameRemoved.Guid);
        }

        private static void ProcessGameUpdated(WEBMessage e)
        {
            var GameUpdated = e.Deserialize<WEB_GameUpdated>();
            if (GameUpdated == null) return;
            GameManager.Update(GameUpdated.Game); 
        }

        private static void ProcessGameAdded(WEBMessage e)
        {
            var GameAdded = e.Deserialize<WEB_GameAdded>();
            if (GameAdded == null) return;
            GameManager.AddGame(GameAdded.Game);
        }

        private static void ProcessGameOpenContainerFolder(WEBMessage e)
        {
            var OpenContainerFolder = e.Deserialize<WEB_GameOpenContainerFolder>();
            if (OpenContainerFolder == null) return;
            var Game = GameManager.GetGame(OpenContainerFolder.Guid);
            if (Game == null) return;
            modCommon.OpenFolderAndSelectFile(Game.ExecutablePath);
        }

        private static void ProcessGameOpenWithoutEmulation(WEBMessage e)
        {
            var OpenWithoutEmulation = e.Deserialize<WEB_GameOpenWithoutEmulation>();
            if (OpenWithoutEmulation == null) return;
            var Game = GameManager.GetGame(OpenWithoutEmulation.Guid);
            if (Game == null) return;
            try { Process.Start(Game.ExecutablePath, Game.Parameters); } catch { }
        }

        private static void ProcessFileInfoRequest(WEBMessage e)
        {
            var FileInfoRequest = e.Deserialize<WEB_FileInfoRequest>();
            if (FileInfoRequest == null) return;
            if (!File.Exists(FileInfoRequest.FilePath)) return;
            FileInfo info = new FileInfo(FileInfoRequest.FilePath);
            var hexIcon = "";
            try
            {
                var bitmap = (Bitmap)ImageHelper.IconFromFile(FileInfoRequest.FilePath);
                hexIcon = ImageHelper.GetImageBase64(bitmap);
            }
            catch { }
            int posibleAppID = 0;
            try
            {
                var PathDirectory = Directory.GetParent(FileInfoRequest.FilePath).ToString();
                if (File.Exists(Path.Combine(PathDirectory, "steam_appid.txt")))
                {
                    posibleAppID = int.Parse(File.ReadAllText(Path.Combine(PathDirectory, "steam_appid.txt")));
                }
            }
            catch { }
            var FileInfoResponse = new WEB_FileInfoResponse()
            {
                FilePath = FileInfoRequest.FilePath,
                Size = info.Length,
                ImageHex = hexIcon,
                AppID = posibleAppID
            };
            Send(FileInfoResponse, WEBMessageType.WEB_FileInfoResponse);
        }

        private static void ProcessGameOrderUpdated(WEBMessage e)
        {
            // TODO
        }

        public static void SendGameOppened(string guid)
        {
            var GameStoped = new WEB_GameLaunched()
            {
                Guid = guid
            };
            Send(GameStoped, WEBMessageType.WEB_GameLaunched);
        }

        public static void SendDownloadProcess(int currentDownloadID, int value, string info)
        {
            var CacheProcess = new WEB_GameCacheDownloadProgress()
            {
                DownloadID = currentDownloadID,
                Value = value,
                Info = info
            };
            Send(CacheProcess, WEBMessageType.WEB_GameCacheDownloadProgress);
        }

        public static void SendGameClosed(string gameClientID)
        {
            var GameStoped = new WEB_GameStoped()
            {
                Guid = gameClientID
            };
            Send(GameStoped, WEBMessageType.WEB_GameStoped);
        }

        public static void Send(WEB_Base msgBase, WEBMessageType Type)
        {
            WEBMessage WebMessage = new WEBMessage()
            {
                MessageType = Type,
                Body = msgBase.Serialize()
            };
            if (NetworkManager.WebClient != null)
            {
                Write($"Sending WEB message {Type}");
                NetworkManager.WebClient?.Send(WebMessage.Serialize());
            }
        }

        private static void Write(object msg)
        {
            Log.Write("WebManager", msg);
        }
    }
}
