using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using SKYNET.Client;
using SKYNET.Common;
using SKYNET.GUI;
using SKYNET.GUI.Controls;
using SKYNET.Helper;
using SKYNET.Managers;
using SKYNET.Network.Packets;
using SKYNET.Properties;
using SKYNET.Steamworks;
using SKYNET.Types;

namespace SKYNET
{
    public partial class frmMain : frmBase
    {
        public static frmMain frm;
        public static Types.Settings settings;
        public SteamClient SteamClient;

        private List<RunningGame> RunningGames;
        private GameBox SelectedBox;
        private GameBox MenuBox;
        private Dictionary<uint, List<string>> GameMessages;

        public frmMain()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            SetMouseMove(PN_Top);
            frm = this;

            //new Form1().ShowDialog();
            //Process.GetCurrentProcess().Kill();

            new DropShadow().ApplyShadows(this);

            Log.OnMessage += Log_OnMessage;
            ShadowBox.BackColor = Color.FromArgb(100, 0, 0, 0);

            settings = Types.Settings.Load();
            LB_NickName.Text = settings.PersonaName;
            LB_Menu_NickName.Text = settings.PersonaName.ToUpper();
            LB_SteamID.Text = new CSteamID(settings.AccountID).SteamID.ToString();

            GameMessages = new Dictionary<uint, List<string>>();
            RunningGames = new List<RunningGame>();
                 
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "Data"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "Data", "www"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "Data", "Storage"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "Data", "Injector"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "Data", "Images"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "Data", "Images", "AppCache"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "Data", "Images", "AvatarCache"));

            try
            {
                string AvatarPath = Path.Combine(modCommon.GetPath(), "Data", "Images", "AvatarCache", "Avatar.jpg");
                if (File.Exists(AvatarPath))
                {
                    var AvatarImage = ImageHelper.FromFile(AvatarPath);
                    PB_Avatar.Image = AvatarImage;
                    SteamClient.Avatar = (Bitmap)AvatarImage;
                }
            }
            catch (Exception)
            {
            }

            SteamClient = new SteamClient(settings);
            SteamClient.Initialize();
            SteamClient.DefaultAvatar = Resources.DefaultImage;

            GameManager.OnGameAdded += GameManager_OnGameAdded;
            GameManager.OnGameUpdated += GameManager_OnGameUpdated;
            GameManager.OnGameRemoved += GameManager_OnGameRemoved;
            GameManager.OnGameLaunched = GameManager_OnGameOpened;
            GameManager.OnUserGameOpened += GameManager_OnUserGameOpened;
            GameManager.OnGameClosed = GameManager_OnGameClosed;
            GameManager.Initialize();

            UserManager.OnUserAdded += UserManager_OnUserAdded;
            UserManager.OnUserUpdated += UserManager_OnUserUpdated;
            UserManager.OnUserRemoved += UserManager_OnUserRemoved;
            UserManager.OnAvatarReceived = UserManager_OnAvatarReceived;

            WebManager.OnGameLaunch += UserManager_OnGameLaunch;
            WebManager.Initialize();           
        }

        #region GameManager Events

        private void GameManager_OnGameAdded(object sender, Game e)
        {
            AddBoxGame(e);
        }

        private void GameManager_OnGameUpdated(object sender, Game e)
        {
            var game = RunningGames.Find(g => g.Game.Guid == e.Guid);
            if (game != null)
            {
                IPCManager.SendModifyFileLog(game.Game);
            }
            for (int i = 0; i < PN_GameContainer.Controls.Count; i++)
            {
                object control = PN_GameContainer.Controls[i];
                if (control is GameBox && ((GameBox)control).Game.Guid == e.Guid)
                {
                    ((GameBox)control).Game = e;
                }
            }
        }

        private void GameManager_OnGameRemoved(object sender, Game e)
        {
            for (int i = 0; i < PN_GameContainer.Controls.Count; i++)
            {
                object control = PN_GameContainer.Controls[i];
                if (control is GameBox && ((GameBox)control).Game.Guid == e.Guid)
                {
                    PN_GameContainer.Controls.RemoveAt(i);
                }
            }
        }

        private void GameManager_OnGameOpened(object sender, GameManager.GameLaunchedEventArgs e)
        {
            var game = new RunningGame(e.ProcessID, e.Game, e.GameClientID);
            if (game.Process == null) return;
            RunningGames.Add(game);
            NetworkManager.SendGameOpened(e.Game);

            if (e.Game.Guid == MenuBox?.Game.Guid)
            {
                BT_GameAction.Text = "CLOSE";
                BT_GameAction.BackColor = Color.Red;
            }
        }

        private void GameManager_OnUserGameOpened(object sender, NET_GameOpened e)
        {
            if (e.AccountID == SteamClient.AccountID)
            {
                var Game = GameManager.GetGame(e.AppID);
                if (Game != null)
                {
                    WebManager.SendGameOppened(Game.Guid);
                }
                return;
            } 

            Image Avatar = default;
            string personaName = "";

            var User = UserManager.GetUser(e.AccountID);
            if (User != null)
            {
                personaName = User.PersonaName;
                Avatar = User.Avatar;
            }
            else
            {
                personaName = "Some User";
                Avatar = Resources.DefaultImage;
            }

            new frmPlayerNotify(e, personaName, Avatar).ShowDialog();
        }

        private void GameManager_OnGameClosed(object sender, string gameClientID)
        {
            RunningGames.RemoveAll(g => g.GameClientID == gameClientID);

            BT_GameAction.Text = "PLAY";
            BT_GameAction.BackColor = Color.FromArgb(46, 186, 65);
            WebManager.SendGameClosed(gameClientID);
        }

        #endregion

        #region UserManager Events

        private void UserManager_OnUserAdded(object sender, SteamPlayer user)
        {
            try
            {
                for (int i = 0; i < PN_UserContainer.Controls.Count; i++)
                {
                    var control = PN_UserContainer.Controls[i];
                    if (control is SKYNET_UserControl)
                    {
                        var User = (SKYNET_UserControl)control;
                        if (User.SteamPlayer.AccountID == user.AccountID)
                        {
                            return;
                        }
                    }
                }

                var userControl = new SKYNET_UserControl();
                userControl.SteamPlayer = user;
                userControl.Dock = DockStyle.Top;

                modCommon.InvokeAction(PN_UserContainer, delegate
                {
                    PN_UserContainer.Controls.Add(userControl);
                });
            }
            catch 
            {

            }
        }

        private void UserManager_OnUserUpdated(object sender, SteamPlayer user)
        {

        }

        private void UserManager_OnUserRemoved(object sender, SteamPlayer user)
        {

        }

        private void UserManager_OnAvatarReceived(object sender, UserManager.AvatarReceivedEventArgs e)
        {
            for (int i = 0; i < PN_UserContainer.Controls.Count; i++)
            {
                var control = PN_UserContainer.Controls[i];
                if (control is SKYNET_UserControl)
                {
                    var User = (SKYNET_UserControl)control;
                    if (User.SteamPlayer.AccountID == e.AccountID)
                    {
                        User.Avatar = e.Avatar;
                    }
                }
            }
        }

        #endregion

        private void UserManager_OnGameLaunch(object sender, string Guid)
        {
            var Game = GameManager.GetGame(Guid);
            if (Game == null) return;
            OpenGame(Game);
        }

        private void Log_OnMessage(object sender, LogEventArgs Event)
        {
            Write(Event.Sender, Event.Message);
        }

        private void GameBox_Clicked(object sender, MouseEventArgs e )
        {
            GameBox b = (GameBox)sender;
            if (e.Button == MouseButtons.Left)
            {
                SelectBox(b);
                MenuBox = b;
            }
            if (e.Button == MouseButtons.Right)
            {
                MenuBox = b;
                CM_MenuGame.Show(b, new Point(e.Location.X, e.Location.Y));
            }
        }

        private void GameBox_DoubleClicked(object sender, GameBox e)
        {
            SelectBox(e);
            OpenGame(e.Game);
        }

        private void SelectBox(GameBox e)
        {
            SelectedBox = e;

            if (RunningGames.Find(x => x.Game == e.Game) != null)
            {
                BT_GameAction.Text = "CLOSE";
                BT_GameAction.BackColor = Color.Red;
            }
            else
            {
                BT_GameAction.Text = "PLAY";
                BT_GameAction.BackColor = Color.FromArgb(46, 186, 65);
            }

            LB_GameTittle.Text = e.Game.Name;

            try
            {
                var imageBytes = Convert.FromBase64String(e.Game.AvatarHex);
                Bitmap Avatar = (Bitmap)ImageHelper.ImageFromBytes(imageBytes);
                PB_Logo.Image = Avatar;
            }
            catch 
            {
            }

            string imagePath = Path.Combine(modCommon.GetPath(), "Data", "Images", "AppCache", e.Game.AppID + "_library_hero.jpg");
            if (File.Exists(imagePath))
            {
                PB_Banner.Image = Image.FromFile(imagePath);
            }
            else
            {
                PB_Banner.Image = Resources.Header_1;
            }

            foreach (var control in PN_GameContainer.Controls)
            {
                if (control is GameBox)
                {
                    ((GameBox)control).Selected = false;
                }
            }

            e.Selected = true;
            BT_GameAction.Visible = true;

        }

        private void AddBoxGame(Game game)
        {
            var module = new GameBox();
            module.Game = game;
            module.Dock = DockStyle.Top;
            module.BoxClicked += GameBox_Clicked;
            module.BoxDoubleClicked += GameBox_DoubleClicked;
            module.BackColor = Color.FromArgb(23, 33, 43);
            module.Color = Color.FromArgb(23, 33, 43);
            module.Color_MouseHover = Color.FromArgb(33, 43, 53);

            modCommon.InvokeAction(PN_GameContainer, delegate
            {
                PN_GameContainer.Controls.Add(module);
            });
        }


        private void OpenGame(Game game)
        {
            if (game.LaunchWithoutEmu)
            {
                Process.Start(game.ExecutablePath, game.Parameters);
                return;
            }

            Write("SteamClient", "Opening " + game.Name);

            DllInjector.Inject(game);
        }

        public static void AvatarUpdated(Bitmap Avatar)
        {
            SteamClient.Avatar = Avatar;
            frm.PB_Avatar.Image = Avatar;
            IPCManager.SendAvatarUpdated(SteamClient.AccountID, Avatar);

            try
            {
                string AvatarPath = Path.Combine(modCommon.GetPath(), "Data", "Images", "AvatarCache", "Avatar.jpg");
                modCommon.EnsureDirectoryExists(AvatarPath, true);
                ImageHelper.ToFile(AvatarPath, Avatar);
            }
            catch 
            {
                modCommon.Show("Error saving avatar image");
            }
        }

        public static void PersonaNameUpdated(string PersonaName)
        {
            SteamClient.PersonaName = PersonaName;
            frm.LB_NickName.Text = PersonaName;
            frm.LB_Menu_NickName.Text = PersonaName.ToUpper();
            IPCManager.SendUserDataUpdated(SteamClient.AccountID, PersonaName);
            settings.PersonaName = PersonaName;
            Types.Settings.Save(settings);
        }

        public static void AccountIDUpdated(uint accountID)
        {
            SteamClient.AccountID = accountID;
            SteamClient.SteamID = new Steamworks.CSteamID(accountID);
            frm.LB_SteamID.Text = SteamClient.SteamID.SteamID.ToString();
            settings.AccountID = accountID;
            Types.Settings.Save(settings);
        }

        private void Write(string sender, object msg)
        {
            WebLogger1.WriteLine(new ConsoleMessage(0, sender, msg));
        }

        private void Close_Clicked(object sender, EventArgs e)
        {
            string path = Path.Combine(modCommon.GetPath(), "Data", "Games.bin");
            modCommon.EnsureDirectoryExists(path, true);

            Types.Settings.Save(settings);

            List<Game> Games = new List<Game>();
            foreach (var control in PN_GameContainer.Controls)
            {
                if (control is GameBox)
                {
                    Games.Add(((GameBox)control).Game);
                }
            }

            string json = new JavaScriptSerializer().Serialize(Games);
            File.WriteAllText(path, json);

            Process.GetCurrentProcess().Kill();
        }

        private void Minimize_Clicked(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void TB_Search_KeyUp(object sender, KeyEventArgs e)
        {
            FindGame(TB_Search.Text);
        }

        private void FindGame(string word = "")
        {
            foreach (var control in PN_GameContainer.Controls)
            {
                if (control is GameBox)
                {
                    GameBox Game = (GameBox)control;

                    if (string.IsNullOrEmpty(word))
                    {
                        Game.Visible = true;
                    }
                    else
                    {
                        if (Game.Name.ToLower().Contains(word.ToLower()))
                        {
                            Game.Visible = true;
                        }
                        else
                        {
                            Game.Visible = false;
                        }
                    }
                }
            }
        }

        private void Add_MouseMove(object sender, MouseEventArgs e)
        {
            LB_Add.ForeColor = Color.White;
            PB_Add.Image = Resources.add_Selected;
        }

        private void Add_MouseLeave(object sender, EventArgs e)
        {
            LB_Add.ForeColor = Color.FromArgb(200, 200, 200);
            PB_Add.Image = Resources.add;
        }

        private void GameAction_Click(object sender, EventArgs e)
        {
            if (BT_GameAction.Text == "PLAY")
            {
                OpenGame(SelectedBox.Game);
            }
            else
            {
                var Game = RunningGames.Find(x => x.Game == SelectedBox.Game);
                if (Game != null)
                {
                    try
                    {
                        Game.Process.Kill();

                        foreach (var game in RunningGames)
                        {
                            if (GameMessages.ContainsKey(game.Game.AppID) && game.Game.LogToFile)
                            {
                                string logPath = Path.Combine(modCommon.GetPath(), "Data", "Storage", game.Game.AppID.ToString(), "GameMessages.log");
                                File.WriteAllLines(logPath, GameMessages[game.Game.AppID]);
                            }
                        }

                        RunningGames.Remove(Game);
                    }
                    catch { }
                }
            }
        }

        private void AddGame_Clicked(object sender, MouseEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                 Filter = "exe file | *.exe",
                 Multiselect = false
            };
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                new frmGameManager(fileDialog.FileName).ShowDialog();
            }
        }

        #region Menu items events
        private void ToTopMenuItem_Click(object sender, EventArgs e)
        {
            MenuBox.SendToBack();
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenGame(MenuBox.Game);
        }

        private void OpenWithoutEmuMenuItem_Click(object sender, EventArgs e)
        {
            var game = MenuBox.Game;
            Process.Start(game.ExecutablePath, game.Parameters);
        }

        private void OpenFileLocationMenuItem_Click(object sender, EventArgs e)
        {
            modCommon.OpenFolderAndSelectFile(MenuBox.Game.ExecutablePath);
        }

        private void ConfigureMenuItem_Click(object sender, EventArgs e)
        {
            new frmGameManager(MenuBox.Game).ShowDialog();
        }

        private void RemoveMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = modCommon.Show("You are sure you want to remove this game?", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                for (int i = 0; i < PN_GameContainer.Controls.Count; i++)
                {
                    object control = PN_GameContainer.Controls[i];
                    if (control is GameBox && ((GameBox)control).Game.Guid == MenuBox.Game.Guid)
                    {
                        Game game = ((GameBox)control).Game;
                        GameManager.Remove(game.AppID);
                        PN_GameContainer.Controls.RemoveAt(i);
                    }
                }
            }
        }

        private void ToButtomMenuItem_Click(object sender, EventArgs e)
        {
            MenuBox.BringToFront();
        }

        private void GameCacheMenuItem_Click(object sender, EventArgs e)
        {
            if (MenuBox.Game.AppID == 0)
            {
                modCommon.Show("Please configure a valid AppId for this game.");
                return;
            }
            new frmGameDownload(MenuBox).ShowDialog();
        }

        #endregion

        private void PN_GameContainer_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];
            new frmGameManager(path).ShowDialog();
        }

        private void PN_GameContainer_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void BT_Profile_Click(object sender, EventArgs e)
        {
            new frmUpdateProfile().ShowDialog();
        }

        private void BT_Connect_Click(object sender, EventArgs e)
        {
            NetworkManager.SendAnnounce();
        }

        private void LB_Browser_Click(object sender, EventArgs e)
        {
            var frmBrowser = new frmBrowser();
            frmBrowser.Show();
        }

        private void LB_Clear_Click(object sender, EventArgs e)
        {
            WebLogger1.ClearScreen();
        }

        private void Label2_Click(object sender, EventArgs e)
        {
            string exe = @"D:\Juegos\Steam\steamapps\common\dota 2 beta\game\bin\win64\dota2.exe";
            string arg = "-windowed -console -novid -high -480 -dx11 +map_enable_background_maps 0 -prewarm";
            string dll = @"D:\Instaladores\Programación\Projects\[SKYNET] Steam Emulator\[SKYNET] Steam Emulator\bin\Debug\x64\steam_api64.dll";
            Process.Start(@"C:\Users\Administrador\source\repos\SKYNET.Injector\SKYNET.Injector\bin\Debug\x64\SKYNET.Injector.exe", "\"" + exe + "\" \"" + arg + "\" \"" + dll + "\"");
        }

    }
}
