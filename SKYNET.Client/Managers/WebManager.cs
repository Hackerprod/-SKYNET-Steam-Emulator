using SKYNET.Client;
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
                        ProcessChatMessage(e);
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
                    case WEBMessageType.WEB_UpdateUser:
                        ProcessUpdateUser(e);
                        break;
                    case WEBMessageType.WEB_UpdateAvatar:
                        ProcessUpdateAvatar(e);
                        break;
                    case WEBMessageType.WEB_GameStop:
                        ProcessGameStop(e);
                        break;
                    case WEBMessageType.WEB_UserLogOff:
                        ProcessUserLogOff(e);
                        break;
                }
            }
            catch (Exception ex)
            {
                Write($"Error proccessing WEB Message {ex}");
            }
        }

        private static void ProcessUserLogOff(WEBMessage e)
        {
            GameManager.Save();
            Process.GetCurrentProcess().Kill();
        }

        private static void ProcessChatMessage(WEBMessage e)
        {
            var ChatMessage = e.Deserialize<WEB_ChatMessage>();
            if (ChatMessage == null) return;
            NetworkManager.SendChatMessage(ChatMessage.Message);
        }

        private static void ProcessGameStop(WEBMessage e)
        {
            var GameStop = e.Deserialize<WEB_GameStop>();
            if (GameStop == null) return;
            try
            {
                var Game = GameManager.GetRunningGame(GameStop.Guid);

                if (Game != null)
                {
                    try
                    {
                        Game.Process.Kill();
                    }
                    catch { }
                }
            }
            catch { }
        }

        private static void ProcessUpdateUser(WEBMessage e)
        {
            var UpdateUser = e.Deserialize<WEB_UpdateUser>();
            if (UpdateUser == null) return;
            if (UserManager.UpdateUser(UpdateUser.Info))
            {
                var UserUpdated = new WEB_UserUpdated()
                {
                    Info = UpdateUser.Info
                };
                Send(UserUpdated, WEBMessageType.WEB_UserUpdated);
            }
        }

        private static void ProcessUpdateAvatar(WEBMessage e)
        {
            var UpdateAvatar = e.Deserialize<WEB_UpdateAvatar>();
            if (UpdateAvatar == null) return;
            var Image = ImageHelper.ImageFromBase64(UpdateAvatar.AvatarHex);
            if (UserManager.UpdateAvatar(Image))
            {
                WEB_AvatarUpdated AvatarUpdated = new WEB_AvatarUpdated()
                {
                    AvatarHex = UpdateAvatar.AvatarHex
                };
                Send(AvatarUpdated, WEBMessageType.WEB_AvatarUpdated);
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
                AuthResponse.Response = WEB_AuthResponse.AuthResponseType.UnknownError;
                Send(AuthResponse, WEBMessageType.WEB_AuthResponse);
            }
            else
            {
                //if (AuthRequest.Username.ToLower() != "hackerprod")
                //{
                //    Write($"Account {loginRequest.AccountName} not found");
                //    AuthResponse.Response = WEB_AuthResponseType.AccountNotFound;
                //    Send(AuthResponse, WEB_MessageType.WEB_AuthResponse);
                //}
                //else if (AuthRequest.Password != "123")
                //{
                //    Write($"Invalid authentication from account {loginRequest.AccountName}");
                //    AuthResponse.Response = WEB_AuthResponseType.PasswordWrong;
                //    Send(AuthResponse, WEB_MessageType.WEB_AuthResponse);
                //}
                //else
                {
                    AuthResponse.Response = WEB_AuthResponse.AuthResponseType.Success;

                    string hexAvatar = "";
                    if (SteamClient.Avatar != null)
                    {
                        hexAvatar = ImageHelper.ImageToBase64(SteamClient.Avatar);
                    }

                    AuthResponse.UserInfo = new UserInfo()
                    {
                        PersonaName = SteamClient.PersonaName,
                        AccountID = SteamClient.AccountID,
                        Language = SteamClient.Language,
                        AvatarHex = hexAvatar, 
                        Wallet = 10000.00
                    };
                    Write($"User {AuthRequest.Username} successfully authenticated");
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
            var InfoRequest = e.Deserialize<WEB_UserInfoRequest>();

            WEB_UserInfoResponse InfoResponse = new WEB_UserInfoResponse();

            if (InfoRequest == null)
            {
                InfoResponse.Response = WEB_UserInfoResponse.UserInfoResponseType.UnknownError;
                Send(InfoResponse, WEBMessageType.WEB_UserInfoResponse);
                return;
            }

            var User = UserManager.GetUser(InfoRequest.AccountID);
            if (User == null)
            {
                InfoResponse.Response = WEB_UserInfoResponse.UserInfoResponseType.AccountNotFound;
                Send(InfoResponse, WEBMessageType.WEB_UserInfoResponse);
                return;
            }

            var AvatarHex = "";
            if (UserManager.GetAvatar(User.AccountID, out var bitmap))
            {
                AvatarHex = ImageHelper.ImageToBase64(bitmap);
            }

            InfoResponse.Response = WEB_UserInfoResponse.UserInfoResponseType.Success;
            InfoResponse.Info = new UserInfo()
            {
                AccountID = User.AccountID,
                PersonaName = User.PersonaName,
                AvatarHex = AvatarHex
            };
            Send(InfoResponse, WEBMessageType.WEB_UserInfoResponse);
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
                            AvatarHex = ImageHelper.ImageToBase64(Avatar),
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
                    Header_Image = "http://127.0.0.1/Images/AppCache/{Game.AppID}/{Game.AppID}_header.jpg",
                    LibraryHero_Image = "http://127.0.0.1/Images/AppCache/{Game.AppID}/{Game.AppID}_library_hero.jpg"
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
            Common.OpenFolderAndSelectFile(Game.ExecutablePath);
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
                hexIcon = ImageHelper.ImageToBase64(bitmap);
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

        internal static void SendChatMessage(uint accountID, string personaName, string message)
        {
            var ChatMessage = new WEB_ChatMessage()
            {
                SenderAccountID = accountID,
                PersonaName = personaName,
                Message = message
            };
            Send(ChatMessage, WEBMessageType.WEB_ChatMessage);
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

        public static void SendConsoleMessage(string sender, object msg)
        {
            var ConsoleMessage = new WEB_ConsoleMessage()
            {
                Sender = sender,
                Message = msg.ToString()
            };
            Send(ConsoleMessage, WEBMessageType.WEB_ConsoleMessage);
        }

        public static void SendDownloadProcessCompleted(int currentDownloadID)
        {
            var CacheProcessCompleted = new WEB_GameCacheDownloadProgressCompleted()
            {
                DownloadID = currentDownloadID,
            };
            Send(CacheProcessCompleted, WEBMessageType.WEB_GameCacheDownloadProgressCompleted);
        }

        public static void SendGameClosed(string gameClientID)
        {
            var GameStoped = new WEB_GameStopped()
            {
                Guid = gameClientID
            };
            Send(GameStoped, WEBMessageType.WEB_GameStopped);
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
