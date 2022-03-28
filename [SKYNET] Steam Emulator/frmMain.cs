using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using EasyHook;
using SKYNET;
using SKYNET.GUI;
using SKYNET.Properties;
using SKYNET.Types;

namespace SKYNET
{
    public partial class frmMain : frmBase
    {
        public static frmMain frm;
        private List<RunningGame> RunningGames;

        public Process InjectedProcess { get; set; }
        internal HookInterface HookInterface { get; set; }

        private GameBox SelectedBox;
        private GameBox MenuBox;

        private string channel;
        private int ProcessId;

        public frmMain()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            SetMouseMove(PN_Top);
            frm = this;

            RunningGames = new List<RunningGame>();
                 
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "Data"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "Data", "Images"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "Data", "Images", "Banner"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "Data", "Images", "Avatars"));

            List<Game> Games = new List<Game>();
            string game = Path.Combine(modCommon.GetPath(), "Data", "Games.json");
            if (File.Exists(game))
            {
                try
                {
                    string json = File.ReadAllText(game);
                    Games = new JavaScriptSerializer().Deserialize<List<Game>>(json);
                }
                catch (Exception)
                {
                    Games = new List<Game>();
                    modCommon.Show("Error loading Game stored data");
                }
            }

            string AvatarDirectory = Path.Combine(modCommon.GetPath(), "Data", "Images", "Avatars");
            if (Directory.Exists(AvatarDirectory))
            {
                if (File.Exists(Path.Combine(AvatarDirectory, "Avatar.png")))
                {
                    PB_Avatar.Image = Image.FromFile(Path.Combine(AvatarDirectory, "Avatar.png"));
                }
            }

            foreach (var Game in Games)
            {
                AddBoxGame(Game);
            }

            shadowBox1.BackColor = Color.FromArgb(100, 0, 0, 0);

        }

        private void GameBox_Clicked(object sender, MouseEventArgs e )
        {
            GameBox b = (GameBox)sender;
            if (e.Button == MouseButtons.Left)
            {
                SelectBox(b);
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
            OpenGame(e);
        }

        private void SelectBox(GameBox e)
        {
            SelectedBox = e;

            if (RunningGames.Find(x => x.Game == e.GetGame()) != null)
            {
                BT_GameAction.Text = "CLOSE";
                BT_GameAction.BackColor = Color.Red;
            }
            else
            {
                BT_GameAction.Text = "PLAY";
                BT_GameAction.BackColor = Color.FromArgb(46, 186, 65);
            }

            PB_Logo.Image = e.Image;
            LB_GameTittle.Text = e.GameName;

            string imagePath = Path.Combine(modCommon.GetPath(), "Data", "Images", "Banner", e.AppId + "_library_hero.jpg");
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

        public void AddGame(Game game)
        {
            AddBoxGame(game);
        }
        public void UpdateGame(int boxHandle, Game game)
        {
            foreach (var control in PN_GameContainer.Controls)
            {
                if (control is GameBox && ((GameBox)control).Handle.ToInt32() == boxHandle)
                {
                    ((GameBox)control).SetGame(game);
                    return;
                }
            }
        }

        private void AddBoxGame(Game game)
        {
            var module = new GameBox();
            module.SetGame(game);
            module.Dock = DockStyle.Top;
            module.BoxClicked += GameBox_Clicked;
            module.BoxDoubleClicked += GameBox_DoubleClicked;
            module.BackColor = Color.FromArgb(36, 40, 47);
            module.Color = Color.FromArgb(36, 40, 47);
            module.Color_MouseHover = Color.FromArgb(50, 57, 74);
            
            PN_GameContainer.Controls.Add(module);
        }


        private void OpenGame(GameBox e)
        {
            Game game = e.GetGame();

            if (game.LaunchWithoutEmu)
            {
                Process.Start(game.ExecutablePath, game.Parameters);
                return;
            }

            Task.Run(() =>
            {
                richTextBox1.Clear();
                Write("Opening " + game.Name);

                HookInterface = new HookInterface();
                HookInterface.InjectionOptions = InjectionOptions.Default;
                HookInterface.AppId = game.AppId;
                HookInterface.OnMessage += this.HookInterface_OnMessage;
                HookInterface.OnShowMessage += this.HookInterface_OnShowMessage;

                channel = null;

                try
                {
                    var InObject = WellKnownObjectMode.Singleton;
                    RemoteHooking.IpcCreateServer(ref channel, InObject, HookInterface);
                    HookInterface.ChannelName = channel;
                    HookInterface.DllPath = Path.Combine(modCommon.GetPath(), "steam_api.dll");
                    RemoteHooking.CreateAndInject(game.ExecutablePath, game.Parameters, 0, HookInterface.InjectionOptions, HookInterface.DllPath, HookInterface.DllPath, out ProcessId, channel);
                    InjectedProcess = Process.GetProcessById(ProcessId);
                    WaitForExit();
                    RunningGames.Add(new RunningGame() { Game = e.GetGame(), Process = InjectedProcess });

                    BT_GameAction.Text = "CLOSE";
                    BT_GameAction.BackColor = Color.Red;
                }
                catch (Exception ex)
                {
                    Write($"Error Injecting {Path.GetFileName(e.GamePath)}");
                    Write(ex.Message);
                }
            });
        }

        private void HookInterface_OnShowMessage(object sender, string e)
        {
            new frmMessage(e).ShowDialog();
        }

        private void HookInterface_OnMessage(object sender, ConsoleMessage e)
        {
            Write(e.Sender + ":  " + e.Msg);
        }

        private void WaitForExit()
        {
            Task.Run(() =>
            {
                int closeId = 0;
                string processName = "";
                while (ProcessId != closeId)
                {
                    try
                    {
                        Process processById = Process.GetProcessById(ProcessId);
                        processName = processById.ProcessName;
                        closeId = ProcessId;
                        processById.WaitForExit();
                    }
                    catch (Exception)
                    {
                    }
                }
                Write($"The injected process {processName} are closed");

                BT_GameAction.Text = "PLAY";
                BT_GameAction.BackColor = Color.FromArgb(46, 186, 65);
            });
        }

        private void Write(object msg)
        {
            try
            {
                richTextBox1.Text += "   " + msg.ToString() + Environment.NewLine;
            }
            catch (Exception)
            {

            }
        }

        private void Close_Clicked(object sender, EventArgs e)
        {
            string path = Path.Combine(modCommon.GetPath(), "Data");
            modCommon.EnsureDirectoryExists(path);

            string game = Path.Combine(path, "Games.json");

            List<Game> Games = new List<Game>();
            foreach (var control in PN_GameContainer.Controls)
            {
                if (control is GameBox)
                {
                    Games.Add(((GameBox)control).GetGame());
                }
            }
            string json = new JavaScriptSerializer().Serialize(Games);
            File.WriteAllText(game, json);

            Process.GetCurrentProcess().Kill();
        }

        private void Minimize_Clicked(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            int attrValue = 2;
            DwmApi.DwmSetWindowAttribute(base.Handle, 2, ref attrValue, 16);
            DwmApi.MARGINS mARGINS = default(DwmApi.MARGINS);
            mARGINS.cyBottomHeight = 1;
            mARGINS.cxLeftWidth = 0;
            mARGINS.cxRightWidth = 0;
            mARGINS.cyTopHeight = 0;
            DwmApi.MARGINS marInset = mARGINS;
            DwmApi.DwmExtendFrameIntoClientArea(base.Handle, ref marInset);
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
                OpenGame(SelectedBox);
            }
            else
            {
                var Game = RunningGames.Find(x => x.Game == SelectedBox.GetGame());
                if (Game != null)
                {
                    try
                    {
                        Game.Process.Kill();
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
            OpenGame(MenuBox);
        }

        private void OpenWithoutEmuMenuItem_Click(object sender, EventArgs e)
        {
            var game = MenuBox.GetGame();
            Process.Start(game.ExecutablePath, game.Parameters);
            return;
        }

        private void OpenFileLocationMenuItem_Click(object sender, EventArgs e)
        {
            modCommon.OpenFolderAndSelectFile(MenuBox.GetGame().ExecutablePath);
        }

        private void ConfigureMenuItem_Click(object sender, EventArgs e)
        {
            new frmGameManager(MenuBox).ShowDialog();
        }

        private void RemoveMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = modCommon.Show("You are sure you want to remove this game?", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.OK)
            {
                for (int i = 0; i < PN_GameContainer.Controls.Count; i++)
                {
                    object control = PN_GameContainer.Controls[i];
                    if (control is GameBox && ((GameBox)control).Handle == MenuBox.Handle)
                    {
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
            if (MenuBox.AppId == 0)
            {
                modCommon.Show("Please configure a valid AppId for this game.");
                return;
            }
            new frmGameDownload(MenuBox).ShowDialog();
        }

        #endregion

        private void PictureBox2_Click(object sender, EventArgs e)
        {
            var dialog = new frmMessage("This is a example").ShowDialog();
            MessageBox.Show(dialog.ToString());
        }

        
    }
}
