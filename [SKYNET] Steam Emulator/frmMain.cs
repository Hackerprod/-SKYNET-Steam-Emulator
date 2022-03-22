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
        private List<Game> Games;
        private List<RunningGame> RunningGames;

        public Process InjectedProcess { get; set; }
        internal HookInterface HookInterface { get; set; }

        private GameBox SelectedBox;

        private string channel;
        private int ProcessId;

        public frmMain()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            SetMouseMove(PN_Top);

            Games = new List<Game>();
            RunningGames = new List<RunningGame>();
                 
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "Data"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "Data", "Images"));
            modCommon.EnsureDirectoryExists(Path.Combine(modCommon.GetPath(), "Data", "Images", "Banner"));

            string game = Path.Combine("Data", "Games.json");
            if (File.Exists(game))
            {
                string json = File.ReadAllText(game);
                Games = new JavaScriptSerializer().Deserialize<List<Game>>(json);
            }

            FindGame("");

            shadowBox1.BackColor = Color.FromArgb(100, 0, 0, 0);

        }

        private void GameBox_Clicked(object sender, GameBox e)
        {
            SelectBox(e);
        }

        private void GameBox_DoubleClicked(object sender, GameBox e)
        {
            SelectBox(e);
            Inject(e);
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
                PB_Banner.Image = Resources.Banner;
            }

        }

        private void Inject(GameBox e)
        {
            Task.Run(() =>
            {
                richTextBox1.Clear();
                Write("Opening " + e.GameName);

                HookInterface = new HookInterface();
                HookInterface.InjectionOptions = InjectionOptions.Default;
                HookInterface.AppId = e.AppId;
                HookInterface.OnMessage += this.HookInterface_OnMessage;

                channel = null;

                string InjectorPath = Path.Combine(modCommon.GetPath(), "steam_api.dll");

                try
                {
                    var InObject = WellKnownObjectMode.Singleton;
                    RemoteHooking.IpcCreateServer(ref channel, InObject, HookInterface);
                    HookInterface.ChannelName = channel;
                    RemoteHooking.CreateAndInject(e.GamePath, e.Parametters, 0, HookInterface.InjectionOptions, InjectorPath, InjectorPath, out ProcessId, channel);
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

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            modCommon.EnsureDirectoryExists("Data");

            string game = Path.Combine("Data", "Games.json");
            if (File.Exists(game))
            {
                string json = new JavaScriptSerializer().Serialize(Games);
                File.WriteAllText(game, json);
            }
            try
            {
                InjectedProcess.Kill();
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
            string json = new JavaScriptSerializer().Serialize(Games);
            File.WriteAllText(game, json);

            try
            {
                InjectedProcess?.Kill();
            }
            catch (Exception)
            {

            }

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

        private void FindGame(string word)
        {
            PN_GameContainer.Controls.Clear();

            foreach (var game in Games)
            {
                var module = new GameBox()
                {
                    GameName = game.Name,
                    GamePath = game.Path,
                    AppId = game.AppId,
                    Parametters = game.Parametters
                };
                module.Dock = DockStyle.Top;
                module.BoxClicked += GameBox_Clicked;
                module.BoxDoubleClicked += GameBox_DoubleClicked;
                module.BackColor = Color.FromArgb(36, 40, 47);
                module.Color = Color.FromArgb(36, 40, 47);
                module.Color_MouseHover = Color.FromArgb(50, 57, 74);
                module.SetGame(game);

                if (string.IsNullOrEmpty(word))
                {
                    PN_GameContainer.Controls.Add(module);
                }
                else if (game.Name.ToLower().Contains(word.ToLower()))
                {
                    PN_GameContainer.Controls.Add(module);
                }

                if (SelectedBox == null)
                {
                    SelectBox(module);
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
                Inject(SelectedBox);
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
    }
}
