using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYNET.Client;
using SKYNET.GUI.Controls;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Network.Types;
using SKYNET.Properties;
using SKYNET.Steamworks;
using SKYNET.Types;
using SKYNET.Wave;

namespace SKYNET.GUI
{
    public partial class frmMain : frmBase
    {
        public static frmMain frm;
        public static Types.Settings settings;
        public SteamClient SteamClient;

        private GameBox SelectedBox;
        private GameBox MenuBox;
        private Dictionary<uint, List<string>> GameMessages;

        public frmMain()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            SetMouseMove(PN_Top);
            frm = this;

            new DropShadow().ApplyShadows(this);

            Log.OnMessage += Log_OnMessage;
            ShadowBox.BackColor = Color.FromArgb(100, 0, 0, 0);

            settings = Types.Settings.Load();
            PaintSettings(settings);

            GameMessages = new Dictionary<uint, List<string>>();
                 
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "www"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "Storage"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "Injector"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "Images"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "Images", "AppCache"));
            Common.EnsureDirectoryExists(Path.Combine(Common.GetPath(), "Data", "Images", "AvatarCache"));

            try
            {
                string AvatarPath = Path.Combine(Common.GetPath(), "Data", "Images", "AvatarCache", "Avatar.jpg");
                if (!File.Exists(AvatarPath))
                {
                    var Image = ImageHelper.GetDesktopWallpaper(true);
                    Image = ImageHelper.Resize(Image, 200, 200);
                    ImageHelper.ToFile(AvatarPath, Image); 
                }
                if (File.Exists(AvatarPath))
                {
                    var AvatarImage = ImageHelper.FromFile(AvatarPath);
                    PB_Avatar.Image = AvatarImage;
                    PB_Profile_Avatar.Image = AvatarImage;
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

            NetworkManager.OnChatMessage = NetworkManager_OnChatMessage;

            WebManager.OnGameLaunch += UserManager_OnGameLaunch;
            WebManager.Initialize();
        }

        private void PaintSettings(Types.Settings settings)
        {
            LB_NickName.Text = settings.PersonaName;
            LB_Profile.Text = settings.PersonaName.ToUpper();
            LB_SteamID.Text = new CSteamID(settings.AccountID).SteamID.ToString();
            TB_Profile_PersonaName.Text = settings.PersonaName;
            TB_Profile_AccountID.Text = settings.AccountID.ToString();
            TB_Profile_Language.Text = settings.Language;
            CB_Profile_AllowRemoteAccess.Checked = settings.AllowRemoteAccess;
            CB_Profile_ShowDebugConsole.Checked = settings.ShowDebugConsole;

            if (settings.ShowDebugConsole)
            {
                LB_Console.Visible = true;
            }

            foreach (WavInDevice device in WaveIn.Devices)
            {
                CB_InputDeviceID.Items.Add(device.Name);
            }

            if (CB_InputDeviceID.Items.Count > 0)
            {
                if (CB_InputDeviceID.Items.Count > settings.InputDeviceID)
                    CB_InputDeviceID.SelectedIndex = settings.InputDeviceID;
                else
                    CB_InputDeviceID.SelectedIndex = 0;
            }

            // Debug 
            if (settings.PersonaName == "jst4rk" || settings.PersonaName == "Hackerprod")
            {
                LB_Browser.Visible = true;
            }
        }

        #region GameManager Events

        private void GameManager_OnGameAdded(object sender, Game e)
        {
            AddBoxGame(e);
        }

        private void GameManager_OnGameUpdated(object sender, Game e)
        {
            var game = GameManager.GetRunningGame(e.Guid); 
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

        private void GameManager_OnUserGameOpened(object sender, NET_GameOpened e)
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
                Avatar = Resources.DefaultImage;
            }

            new frmPlayerNotify(e, personaName, Avatar).ShowDialog();
        }

        private void GameManager_OnGameOpened(object sender, GameManager.GameLaunchedEventArgs e)
        {
            if (GameManager.AddRunningGame(e.ProcessID, e.Game, e.GameClientID, out RunningGame game))
            {
                NetworkManager.SendGameOpened(e.Game);

                if (e.Game.Guid == MenuBox?.Game.Guid)
                {
                    BT_GameAction.Text = "CLOSE";
                    BT_GameAction.BackColor = Color.Red;
                }
                GameManager.SetLastPlayedTime(e.Game.Guid);
                WebManager.SendGameOppened(e.Game.Guid);
            }
        }

        private void GameManager_OnGameClosed(object sender, string gameClientID)
        {
            var ClosedGame = GameManager.GetRunningGame(gameClientID, false);
            if (ClosedGame == null) return;

            GameManager.SetLastPlayedTime(ClosedGame.Game.Guid);
            GameManager.SetTimePlayed(ClosedGame.Game.Guid, ClosedGame.OppenedTime);
            GameManager.RemoveRunningGame(gameClientID);
            WebManager.SendGameClosed(gameClientID);

            if (MenuBox.Game.Guid == ClosedGame.Game.Guid)
            {
                BT_GameAction.Text = "PLAY";
                BT_GameAction.BackColor = Color.FromArgb(46, 186, 65);
            }
        }

        #endregion

        #region UserManager Events

        private void UserManager_OnGameLaunch(object sender, string Guid)
        {
            var Game = GameManager.GetGame(Guid);
            if (Game == null) return;
            OpenGame(Game);
        }

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

                Common.InvokeAction(PN_UserContainer, delegate
                {
                    PN_UserContainer.Controls.Add(userControl);
                });

                LB_UsersOnline.Text = $"Users Online {UserManager.Users.Count - 1}";
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

        private void Log_OnMessage(object sender, LogEventArgs Event)
        {
            if (!settings.ShowDebugConsole) return;

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
            if (!GameManager.IsRunningGame(e.Game.Guid))
            {
                OpenGame(e.Game);
            }
        }

        private void SelectBox(GameBox e)
        {
            SelectedBox = e;
            var Running = GameManager.GetRunningGame(e.Game.Guid);
            if (Running != null)
            {
                BT_GameAction.Text = "CLOSE";
                BT_GameAction.BackColor = Color.Red;
            }
            else
            {
                BT_GameAction.Text = "PLAY";
                BT_GameAction.BackColor = Color.FromArgb(46, 186, 65);
            }

            try
            {
                var Avatar = ImageHelper.ImageFromBase64(e.Game.AvatarHex);
                PB_Logo.Image = Avatar;
                PB_GameInfo.Image = Avatar;
            }
            catch 
            {
            }

            var GameDetails = StatsManager.GetGameDetails(e.Game.AppID);
            if (GameDetails != null && GameDetails.success)
            {
                LB_ShortDescription.Text = GameDetails.data.short_description;
                LB_GameTittle.Text = GameDetails.data.name;
            }
            else
            {
                LB_ShortDescription.Text = $"{e.Game.Name} is a Steam game with AppID {e.Game.AppID}. For more info please download Game cache.";
                LB_GameTittle.Text = e.Game.Name;
            }

            string imagePath = Path.Combine(Common.GetPath(), "Data", "Images", "AppCache", e.Game.AppID + "_library_hero.jpg");
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

            string LastPlayed = "";
            if (e.Game.LastPlayed == 0)
            {
                LastPlayed = "Never played";
            }
            else
            {
                var Spam = DateTime.Now - e.Game.LastPlayed.ToDateTime();
                if (Spam.Days != 0) LastPlayed = Spam.Days + (Spam.Days == 1 ? " Day ago" : " Days ago");
                else if (Spam.Hours != 0) LastPlayed = Spam.Hours + (Spam.Hours == 1 ? " Hour ago" : " Hours ago");
                else if (Spam.Minutes != 0) LastPlayed = Spam.Minutes + (Spam.Minutes == 1 ? " Minute ago" : " Minutes ago"); 
                else if (Spam.Seconds != 0) LastPlayed = Spam.Seconds + (Spam.Seconds == 1 ? " Second ago" : " Seconds ago"); 
            }

            LB_LastPlayed.Text = LastPlayed;

            string TimePlayed = "";
            if (e.Game.LastPlayed == 0)
            {
                TimePlayed = "Never played";
            }
            else
            {
                var Time = new DateTime().AddSeconds(e.Game.TimePlayed); 
                if (Time.Hour != 0) TimePlayed = Time.Day + " Hours";
                if (Time.Minute != 0) TimePlayed = Time.Minute + " Minutes";
                else if (Time.Second != 0) TimePlayed = Time.Second + " Seconds";
            }

            LB_PlayedTime.Text = TimePlayed;

            int playing = UserManager.GetUsersPlaying(e.Game.AppID, false).Count;
            LB_PlayingNow.Text = playing + " friends";

            e.Selected = true;
            BT_GameAction.Visible = true;

            LB_LastPlayed.Visible = true;
            LB_LastPlayedInfo.Visible = true;
            LB_PlayedTime.Visible = true;
            LB_PlayedTimeInfo.Visible = true;
            LB_PlayingNow.Visible = true;
            LB_PlayingNowInfo.Visible = true;
            LB_ShortDescription.Visible = true;
            PB_GameInfo.Visible = true;
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

            Common.InvokeAction(PN_GameContainer, delegate
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

            DLLInjector.Inject(game);
        }

        public static void AvatarUpdated(Bitmap Avatar)
        {
            Bitmap upDatedAvatar = Avatar;
            SteamClient.Avatar = upDatedAvatar;
            frm.PB_Avatar.Image = upDatedAvatar;
            frm.PB_Profile_Avatar.Image = upDatedAvatar;
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
            frm.LB_NickName.Text = PersonaName;
            frm.LB_Profile.Text = PersonaName.ToUpper();
            IPCManager.SendUserDataUpdated(SteamClient.AccountID, PersonaName);
            settings.PersonaName = PersonaName;
            Types.Settings.Save(settings);
        }

        public static void AccountIDUpdated(uint accountID)
        {
            SteamClient.AccountID = accountID;
            SteamClient.SteamID = new CSteamID(accountID);
            frm.LB_SteamID.Text = SteamClient.SteamID.SteamID.ToString();
            settings.AccountID = accountID;
            Types.Settings.Save(settings);
        }

        private void Write(string sender, object msg)
        {
            WebLogger1.WriteLine(new ConsoleMessage(0, sender, msg));
            if (!msg.ToString().Contains("WEB_ConsoleMessage"))
            {
                WebManager.SendConsoleMessage(sender, msg);
            }
        }

        private void Close_Clicked(object sender, EventArgs e)
        {
            GameManager.Save();

            Types.Settings.Save(settings);

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
                        if (Game.Game.Name.ToLower().Contains(word.ToLower()))
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
                var Game = GameManager.GetRunningGame(SelectedBox.Game.Guid);

                if (Game != null)
                {
                    try
                    {
                        Game.Process.Kill();
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
            Common.OpenFolderAndSelectFile(MenuBox.Game.ExecutablePath);
        }

        private void ConfigureMenuItem_Click(object sender, EventArgs e)
        {
            new frmGameManager(MenuBox.Game).ShowDialog();
        }

        private void RemoveMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = Common.Show("You are sure you want to remove this game?", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                for (int i = 0; i < PN_GameContainer.Controls.Count; i++)
                {
                    try
                    {
                        object control = PN_GameContainer.Controls[i];
                        if (control is GameBox && ((GameBox)control).Game.Guid == MenuBox.Game.Guid)
                        {
                            Game game = ((GameBox)control).Game;
                            GameManager.Remove(game.Guid);
                            PN_GameContainer.Controls.RemoveAt(i);
                        }
                    } catch { }
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
                Common.Show("Please configure a valid AppId for this game.");
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

        private void LB_Browser_Click(object sender, EventArgs e)
        {
            var frmBrowser = new frmBrowser();
            frmBrowser.Show();
        }

        private void LB_Clear_Click(object sender, EventArgs e)
        {
            WebLogger1.ClearScreen();
        }

        private void LB_NickName_Click(object sender, EventArgs e)
        {
            int id = Common.GetRandom();
            Task.Run(delegate
            {
                for (int i = 0; i < 10; i++)
                {
                    WebManager.SendDownloadProcess(id, i * 10, $"This is the step {i} of download.");
                    Thread.Sleep(1000);
                }
                WebManager.SendDownloadProcessCompleted(id);
            });
        }

        private void LB_Library_Click(object sender, EventArgs e)
        {
            OpenTab(TP_Library);
        }

        private void LB_Community_Click(object sender, EventArgs e)
        {
            OpenTab(TP_Community);
        }

        private void LB_Profile_Click(object sender, EventArgs e)
        {
            OpenTab(TP_Profile);
        }

        private void LB_Console_Click(object sender, EventArgs e)
        {
            OpenTab(TP_Console);
        }

        private void OpenTab(TabPage tabPage)
        {
            TabControl1.SelectTab(tabPage);
        }

        private void BT_Profile_Apply_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TB_Profile_PersonaName.Text)/* | string.IsNullOrEmpty(TB_Profile_Password.Text)*/)
            {
                Common.Show("Please... set valid Name.");
                return;
            }

            if (Common.UpdatedAvatar != null)
            {
                AvatarUpdated(Common.UpdatedAvatar);
                Common.UpdatedAvatar = null;
            }
            if (TB_Profile_PersonaName.Text != SteamClient.PersonaName)
                PersonaNameUpdated(TB_Profile_PersonaName.Text);

            if (!string.IsNullOrEmpty(TB_Profile_Language.Text))
            {
                SteamClient.Language = TB_Profile_Language.Text;
                settings.Language = TB_Profile_Language.Text;
            }

            settings.AllowRemoteAccess = CB_Profile_AllowRemoteAccess.Checked;
            settings.ShowDebugConsole = SteamClient.Debug = CB_Profile_ShowDebugConsole.Checked;

            if (CB_Profile_ShowDebugConsole.Checked)
            {
                LB_Console.Visible = true;
            }
            else
            {
                LB_Console.Visible = false;
                WebLogger1.ClearScreen();
            }

            settings.InputDeviceID = CB_InputDeviceID.SelectedIndex;
            SteamClient.InputDeviceID = CB_InputDeviceID.SelectedIndex;

            //if (Password.Text != SteamClient.Password)
            //    frmMain.PasswordUpdated(Password.Text);

            if (uint.TryParse(TB_Profile_AccountID.Text, out uint accountID))
            {
                if (accountID != SteamClient.AccountID)
                    AccountIDUpdated(accountID);
            }
            else
            {
                Common.Show("Please... set a valid account ID");
                return;
            }
        }

        private void PB_Profile_Avatar_MouseClick(object sender, MouseEventArgs e)
        {
            var ofdPhoto = new OpenFileDialog()
            {
                FileName = string.Empty,
                Filter = "Picture files|*.png;*.jpg;*.bmp;*.gif|All Files|*.*",
                Title = "Select Photo",
                RestoreDirectory = true
            };

            DialogResult Result = ofdPhoto.ShowDialog();

            if (Result == DialogResult.OK)
            {
                WindowState = FormWindowState.Minimized;

                frmCropEditor editor = new frmCropEditor(ofdPhoto.FileName);
                editor.BringToFront();
                editor.Activate();
                var CropResult = editor.ShowDialog();
                if (CropResult == DialogResult.OK)
                {
                    if (Common.UpdatedAvatar != null)
                    {
                        PB_Profile_Avatar.Image = Common.UpdatedAvatar;
                    }
                }
            }

            WindowState = FormWindowState.Normal;
        }

        private void TB_Chat_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TB_Chat.Text))
            {
                NetworkManager.SendChatMessage(TB_Chat.Text);
                TB_Chat.Text = "";
            }
        }

        private void TB_Chat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (!string.IsNullOrEmpty(TB_Chat.Text))
                {
                    NetworkManager.SendChatMessage(TB_Chat.Text);
                    TB_Chat.Text = "";
                }
            }
        }

        private void NetworkManager_OnChatMessage(object sender, NET_ChatMessage e)
        {
            WebChat.WriteLine(new ConsoleMessage(0, e.PersonaName, e.Message));
        }
    }
}
