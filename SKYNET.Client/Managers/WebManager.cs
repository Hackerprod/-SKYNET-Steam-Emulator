using SKYNET.Client;
using SKYNET.Common;
using SKYNET.Helper;
using SKYNET.Managers;
using SKYNET.Network;
using SKYNET.WEB.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

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
            try
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
                    case WEB_MessageType.WEB_GameOpenContainerFolder:
                        ProcessGameOpenContainerFolder(e);
                        break;
                    case WEB_MessageType.WEB_GameOpenWithoutEmulation:
                        ProcessGameOpenWithoutEmulation(e);
                        break;
                    case WEB_MessageType.WEB_FileInfoRequest:
                        ProcessFileInfoRequest(e);
                        break;
                    case WEB_MessageType.WEB_GameOrderUpdated:
                        ProcessGameOrderUpdated(e);
                        break;
                    case WEB_MessageType.WEB_OpenFileDialogRequest:
                        ProcessOpenFileDialogRequest();
                        break;
                }
            }
            catch (Exception ex)
            {
                Write($"Error proccessing WEB Message {ex}");
            }
        }

        private static void ProcessOpenFileDialogRequest()
        {
            Thread s = new Thread(new ThreadStart(delegate
            {
                var Dialog = new Form()
                {
                    FormBorderStyle = FormBorderStyle.None,
                    TopMost = true,
                    Size = new Size(0, 0),
                    ShowInTaskbar = false,
                    BackColor = Color.Azure,
                    TransparencyKey = Color.Azure,
                };
                Dialog.Show();
                modCommon.InvokeAction(Dialog, delegate
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
                            hexIcon = ImageHelper.GetImageBase64(bitmap);
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
                        Send(FileDialogResponse, WEB_MessageType.WEB_OpenFileDialogResponse);
                    }
                });
                Dialog.Close();
            }));
            s.SetApartmentState(ApartmentState.STA);
            s.Start();

            //Thread s = new Thread(new ThreadStart(delegate
            //{
            //    OpenFileDialog fileDialog = new OpenFileDialog()
            //    {
            //        Filter = "exe file | *.exe",
            //        Multiselect = false
            //    };
            //    if (fileDialog.ShowDialog() == DialogResult.OK)
            //    {
            //        string FileName = fileDialog.FileName;
            //        FileInfo info = new FileInfo(FileName);
            //        var hexIcon = "";
            //        try
            //        {
            //            var bitmap = (Bitmap)ImageHelper.IconFromFile(FileName);
            //            var imageBytes = ImageHelper.ImageToBytes(bitmap);
            //            hexIcon = Convert.ToBase64String(imageBytes);
            //        }
            //        catch { }

            //        int posibleAppID = 0;
            //        try
            //        {
            //            var PathDirectory = Directory.GetParent(FileName).ToString();
            //            if (File.Exists(Path.Combine(PathDirectory, "steam_appid.txt")))
            //            {
            //                posibleAppID = int.Parse(File.ReadAllText(Path.Combine(PathDirectory, "steam_appid.txt")));
            //            }
            //        }
            //        catch { }

            //        var FileDialogResponse = new WEB_OpenFileDialogResponse()
            //        {
            //            FilePath = FileName,
            //            Size = info.Length,
            //            ImageHex = hexIcon,
            //            AppID = posibleAppID
            //        };
            //        Send(FileDialogResponse, WEB_MessageType.WEB_OpenFileDialogResponse);

            //    }
            //}));
            //s.SetApartmentState(ApartmentState.STA);
            //s.Start();
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
                        hexAvatar = ImageHelper.GetImageBase64(SteamClient.Avatar);
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
                var MinimalResponse = new WEB_GameInfoMinimalResponse()
                {
                    LastPlayed = GameManager.GetLastPlayed(GameInfoRequest.Guid),
                    TimePlayed = GameManager.GetTimePlayed(GameInfoRequest.Guid),
                    UsersPlaying = GameManager.GetUsersPlaying(GameInfoRequest.Guid),
                    Playing = GameManager.IsPlaying(GameInfoRequest.Guid),
                };
                Send(MinimalResponse, WEB_MessageType.WEB_GameInfoMinimalResponse);
            }
            else
            {
                var library_hero = "";
                try
                {
                    var bitmap = GameManager.GetGameImage(Game.AppID);
                    library_hero = ImageHelper.GetImageBase64(bitmap);
                }
                catch { }
                var header = "";
                try
                {
                    var bitmap = GameManager.GetGameImage(Game.AppID, true);
                    header = ImageHelper.GetImageBase64(bitmap);
                }
                catch { }

                var FriendsPlaying = new List<WEB_GameInfoResponse.FriendPlaying>();
                try
                {
                    var Users = UserManager.GetFriends();
                    for (int i = 0; i < Users.Count; i++)
                    {
                        FriendsPlaying.Add(new WEB_GameInfoResponse.FriendPlaying()
                        {
                            AccountID = Users[i].AccountID,
                            PersonaName = Users[i].PersonaName,
                            AvatarHex = ImageHelper.GetImageBase64(Users[i].Avatar),
                        });
                        if (i == 10) break;
                    }
                }
                catch { }

                string GameDescription = StatsManager.GetGameDescription(Game.AppID);
                if (string.IsNullOrEmpty(GameDescription))
                {
                    GameDescription = $"{Game.Name} is a Steam game with AppID {Game.AppID}";
                } 

                var Response = new WEB_GameInfoResponse()
                {
                    LastPlayed = GameManager.GetLastPlayed(GameInfoRequest.Guid),
                    TimePlayed = GameManager.GetTimePlayed(GameInfoRequest.Guid),
                    UsersPlaying = GameManager.GetUsersPlaying(GameInfoRequest.Guid),
                    FriendsPlaying = FriendsPlaying,
                    Playing = GameManager.IsPlaying(GameInfoRequest.Guid),
                    Description = GameDescription,
                    FreeToPlay = true,
                    LibraryHeroImage = library_hero,
                    HeaderImage = header
                };
                Send(Response, WEB_MessageType.WEB_GameInfoResponse);
            }
        }

        private static void ProcessGameLaunch(WebMessage e)
        {
            var GameLaunch = e.Deserialize<WEB_GameLaunch>();
            if (GameLaunch == null || string.IsNullOrEmpty(GameLaunch.Guid)) return;
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

        private static void ProcessGameOpenContainerFolder(WebMessage e)
        {
            var OpenContainerFolder = e.Deserialize<WEB_GameOpenContainerFolder>();
            if (OpenContainerFolder == null) return;
            var Game = GameManager.GetGame(OpenContainerFolder.Guid);
            if (Game == null) return;
            modCommon.OpenFolderAndSelectFile(Game.ExecutablePath);
        }

        private static void ProcessGameOpenWithoutEmulation(WebMessage e)
        {
            var OpenWithoutEmulation = e.Deserialize<WEB_GameOpenWithoutEmulation>();
            if (OpenWithoutEmulation == null) return;
            var Game = GameManager.GetGame(OpenWithoutEmulation.Guid);
            if (Game == null) return;
            try { Process.Start(Game.ExecutablePath, Game.Parameters); } catch { }
        }

        private static void ProcessFileInfoRequest(WebMessage e)
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
            Send(FileInfoResponse, WEB_MessageType.WEB_FileInfoResponse);
        }

        private static void ProcessGameOrderUpdated(WebMessage e)
        {
            // TODO
        }

        public static void SendGameOppened(string guid)
        {
            var GameStoped = new WEB_GameLaunched()
            {
                Guid = guid
            };
            Send(GameStoped, WEB_MessageType.WEB_GameLaunched);
        }

        public static void SendGameClosed(string gameClientID)
        {
            var GameStoped = new WEB_GameStoped()
            {
                Guid = gameClientID
            };
            Send(GameStoped, WEB_MessageType.WEB_GameStoped);
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
