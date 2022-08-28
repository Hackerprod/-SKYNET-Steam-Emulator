using SKYNET.Client;
using SKYNET.GUI;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Network;
using SKYNET.Network.Types;
using SKYNET.Steamworks;
using SKYNET.Types;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace SKYNET
{
    public class Main
    {
        public static frmMain frmBrowser;
        public static SteamClient SteamClient;

        public static void Initialize()
        {
            Settings.Load();

            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "www"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "Storage"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "Injector"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "Images"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "Images", "AppCache"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "Images", "AvatarCache"));

            Log.OnMessage += Log_OnMessage;
            
            SteamClient = new SteamClient();
            SteamClient.Initialize();
            SteamClient.DefaultAvatar = Properties.Resources.DefaultImage;

            GameManager.OnGameAdded += GameManager_OnGameAdded;
            GameManager.OnGameUpdated += GameManager_OnGameUpdated;
            GameManager.OnGameRemoved += GameManager_OnGameRemoved;
            GameManager.OnGameLaunched = GameManager_OnGameOpened;
            GameManager.OnUserGameOpened += GameManager_OnUserGameOpened;
            GameManager.OnGameClosed = GameManager_OnGameClosed;
            GameManager.Initialize();

            UserManager.OnUserUpdated += UserManager_OnUserUpdated;
            UserManager.OnUserRemoved += UserManager_OnUserRemoved;
            UserManager.OnAvatarReceived = UserManager_OnAvatarReceived;

            NetworkManager.OnChatMessage = NetworkManager_OnChatMessage;

            frmBrowser = new frmMain();
            frmBrowser.Show();
            frmBrowser.Activate();

            WebManager.OnGameLaunch += UserManager_OnGameLaunch;
            WebManager.Initialize(frmBrowser);

        }

        #region GameManager Events

        private static void GameManager_OnGameAdded(object sender, Game e)
        {
            //AddBoxGame(e);
        }

        private static void GameManager_OnGameUpdated(object sender, Game e)
        {
            var game = GameManager.GetRunningGame(e.Guid);
            if (game != null)
            {
                IPCManager.SendModifyFileLog(game.Game);
            }
            // Used to Update game from game list
        }

        private static void GameManager_OnGameRemoved(object sender, Game e)
        {
            // Used to Remove game from game list
        }

        private static void GameManager_OnUserGameOpened(object sender, NET_GameOpened e)
        {
            Bitmap Avatar = default;
            string personaName = "";

            var User = UserManager.GetUser(e.AccountID);
            if (User != null)
            {
                personaName = User.PersonaName;
                UserManager.GetAvatar(User.SteamID, out Avatar);
            }
            else
            {
                personaName = "Some User";
                Avatar = Properties.Resources.DefaultImage;
            }

            new frmPlayerNotify(e, personaName, Avatar).ShowDialog();
        }

        private static void GameManager_OnGameOpened(object sender, GameManager.GameLaunchedEventArgs e)
        {
            if (GameManager.AddRunningGame(e.ProcessID, e.Game, e.GameClientID, out RunningGame game))
            {
                //NETProcessor.SendGameOpened(e.Game);
                GameManager.SetLastPlayedTime(e.Game.Guid);
                WebManager.SendGameOppened(e.Game.Guid);
            }
        }

        private static void GameManager_OnGameClosed(object sender, string gameClientID)
        {
            var ClosedGame = GameManager.GetRunningGame(gameClientID, false);
            if (ClosedGame == null) return;

            GameManager.SetLastPlayedTime(ClosedGame.Game.Guid);
            GameManager.SetTimePlayed(ClosedGame.Game.Guid, ClosedGame.OppenedTime);
            GameManager.RemoveRunningGame(gameClientID);
            WebManager.SendGameClosed(gameClientID);
        }

        #endregion

        #region UserManager Events

        private static void UserManager_OnGameLaunch(object sender, string Guid)
        {
            var Game = GameManager.GetGame(Guid);
            if (Game == null) return;
            OpenGame(Game);
        }

        private static void UserManager_OnUserUpdated(object sender, SteamPlayer user)
        {
            if (user.AccountID == SteamClient.AccountID)
            {
                AccountIDUpdated(user.AccountID);
                PersonaNameUpdated(user.PersonaName);
            }
        }

        private static void UserManager_OnUserRemoved(object sender, SteamPlayer user)
        {

        }

        private static void UserManager_OnAvatarReceived(object sender, UserManager.AvatarReceivedEventArgs e)
        {
            if (e.AccountID == SteamClient.AccountID)
                AvatarUpdated(e.Avatar);
        }

        #endregion

        private static void NetworkManager_OnChatMessage(object sender, NET_ChatMessage e)
        {
            //WebChat.WriteLine(new ConsoleMessage(0, e.PersonaName, e.Message));
        }

        public static void AvatarUpdated(Bitmap Avatar)
        {
            Bitmap upDatedAvatar = Avatar;
            SteamClient.Avatar = upDatedAvatar;
            IPCManager.SendAvatarUpdated(SteamClient.AccountID, upDatedAvatar);

            try
            {
                string AvatarPath = Path.Combine(Common.GetPath(), "Data", "Images", "AvatarCache", "Avatar.jpg");
                Common.EnsureDirectoryExists(AvatarPath, true);
                ImageHelper.ToFile(AvatarPath, Avatar);
            }
            catch
            {
                Common.Show("Error saving avatar image");
            }
        }

        public static void PersonaNameUpdated(string PersonaName)
        {
            SteamClient.PersonaName = PersonaName;
            IPCManager.SendUserDataUpdated(SteamClient.AccountID, PersonaName);
            Settings.Save();
        }

        public static void AccountIDUpdated(uint accountID)
        {
            SteamClient.AccountID = accountID;
            SteamClient.SteamID = new CSteamID(accountID);
            Settings.Save();
        }

        private static void OpenGame(Game game)
        {
            if (game.LaunchWithoutEmu)
            {
                Process.Start(game.ExecutablePath, game.Parameters);
                return;
            }

            Write("SteamClient", "Opening " + game.Name);

            DLLInjector.Inject(game);
        }

        private static void Log_OnMessage(object sender, LogEventArgs Event)
        {
            if (!Settings.ShowDebugConsole) return;

            Write(Event.Sender, Event.Message);
        }

        private static void Write(string sender, object msg)
        {
            if (!Settings.ShowDebugConsole) return;

            if (msg.ToString().Contains("WEB_ConsoleMessage")) return;
            WebManager.SendConsoleMessage(sender, msg);
        }
    }
}
