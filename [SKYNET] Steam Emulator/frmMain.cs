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
using _SKYNET__Steam_Emulator;
using SKYNET;
using SKYNET.GUI;
using SKYNET.GUI.Controls;
using SKYNET.Properties;

namespace SKYNET
{
    public partial class frmMain : frmBase
    {
        List<Game> Games;
        public Process InjectedProcess { get; private set; }

        private string channel;
        private int ProcessId;

        public frmMain()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            SetMouseMove(PN_Top);
            Games = new List<Game>();

            modCommon.EnsureDirectoryExists("Data");
            string game = Path.Combine("Data", "Games.json");
            if (File.Exists(game))
            {
                string json = File.ReadAllText(game);
                Games = new JavaScriptSerializer().Deserialize<List<Game>>(json);
            }

            FindGame("");


            shadowBox1.BackColor = Color.FromArgb(100, 0, 0, 0);

            //Size = new Size(1600, this.Height);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            WebControl control = new WebControl();
            control.Dock = DockStyle.Fill;
            this.webContainer.Controls.Add(control);
        }
        private void GameBox_Clicked(object sender, GameBox e)
        {
            //Inject(e);
            CreateInMemoryInterface(e);
        }

        //private void Inject(GameBox e)
        //{
        //    HookInterface = new HookInterface();
        //    HookInterface.MessageReceived += HookInterface_MessageReceived;
        //    HookInterface.InjectionOptions = InjectionOptions.Default;
        //    HookInterface.AppId = e.AppId;
        //    channel = null;

        //    string InjectorPath = Process.GetCurrentProcess().MainModule.FileName;
        //    //InjectorPath = @"C:\Users\Administrador\source\repos\[SKYNET] Steam Emulator\[SKYNET] Steam Emulator\bin\Debug\steam_api.dll";

        //    try
        //    {
        //        var InObject = WellKnownObjectMode.Singleton;
        //        RemoteHooking.IpcCreateServer(ref channel, InObject, HookInterface);
        //        RemoteHooking.CreateAndInject(e.GamePath, e.Parametters, 0, HookInterface.InjectionOptions, InjectorPath, InjectorPath, out ProcessId, channel);
        //        InjectedProcess = Process.GetProcessById(ProcessId);
        //        WaitForExit();
        //    }
        //    catch (Exception ex)
        //    {
        //        Write($"Error Injecting {Path.GetFileName(e.GamePath)}");
        //        Write(ex.Message);
        //    }

        //}

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
            });


        }

        private void Write(object msg)
        {
            try
            {
                richTextBox1.Text += " ---      " + msg.ToString() + Environment.NewLine;
            }
            catch (Exception)
            {

            }
        }

        private void HookInterface_MessageReceived(object sender, object msg)
        {
            Write(msg);
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
            modCommon.EnsureDirectoryExists("Data");

            string game = Path.Combine("Data", "Games.json");
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






        private void CreateInMemoryInterface(GameBox e)
        {

            string DllPath = @"D:\Instaladores\Programación\Projects\[SKYNET] Steam Emulator\[SKYNET] Steam Emulator\bin\Debug\x86\steam_api.dll";
            //DllPath = @"D:\Instaladores\Programación\Projects\[SKYNET] Dota2 GameCoordinator Server\steam_api.dll";

            string ExePath = @"C:\Steam\steamapps\Common\dota 2 beta\game\bin\win32\dota2.exe";

            Memory.CreateAndInject(ExePath, DllPath, "-novid");
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
                module.BackColor = Color.FromArgb(36, 40, 47);
                module.Color = Color.FromArgb(36, 40, 47);
                module.Color_MouseHover = Color.FromArgb(50, 57, 74);

                if (string.IsNullOrEmpty(word))
                {
                    PN_GameContainer.Controls.Add(module);
                }
                else if (game.Name.ToLower().Contains(word.ToLower()))
                {
                    PN_GameContainer.Controls.Add(module);
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
    }
}
