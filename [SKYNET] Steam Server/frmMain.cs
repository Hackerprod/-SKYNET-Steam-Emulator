using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Threading;
using SKYNET;
using SKYNET.Managers;
using SKYNET.DB;

namespace SKYNET.GUI
{
    [ComVisibleAttribute(true)]
    public partial class frmMain : frmBase
    {
        public static frmMain frm;

        //private methods
        private System.Timers.Timer AutoSave;
        private DateTime StartTime;
        private MainServer Server;


        public frmMain()
        {
            InitializeComponent();
            frm = this;
            CheckForIllegalCrossThreadCalls = false;

            base.SetMouseMove(PN_Top);
            base.SetMouseMove(LB_Tittle);

            Log.OnMessage += Ilog_OnNewMessage;

            StartTime = DateTime.Now;
        }

        private static void Ilog_OnNewMessage(object sender, LogEventArgs e)
        {
            frm.Logger.Invoke(new Action(() =>
            {
                frm.Logger.WriteLine(new ConsoleMessage(0, e.Sender, e.Message));
            }));
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Server = new MainServer();
            Server.Start();
        }

        private void closeBox_Click(object sender, EventArgs e)
        {
            bool result;
            var task = Task.Factory.StartNew(() =>
            {
                /*result = server.Stop()*/
            });
            task.ContinueWith(t =>
            {
                //FileManager.DeleteDirectory(Path.Combine(Common.GetPath(), "Data", "www", "ClientUpdate", "ClientUpdate"));
                //FileManager.DeleteDirectory(Path.Combine(Common.GetPath(), "Data", "www", "ClientUpdate", "SteamApiUpdate"));
                Process.GetCurrentProcess().Kill();
            },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.FromCurrentSynchronizationContext()
            );


        }

        private void Minimize_click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void Check_Tick(object sender, EventArgs e)
        {
            try
            {
                TimeOnline.Text = Common.GetTotalTime(StartTime);
                Accounts_Created.Text = UserDB.Count().ToString();
                ConnectedClients.Text = ConnectionsManager.ActiveConnections.ToString();
                //if (GameCoordinator?.Events != null)
                //{
                //    ConnectedClients.Text = GameCoordinator?.Events.Connections.ToString(); 
                //    GC_Version.Text = GameCoordinator?.Version.ToString();
                //    Users_playing.Text = GameCoordinator?.Events.UsersPlaying.ToString(); 
                //    Accounts_Created.Text = GameCoordinator?.Events.AccountsCreated.ToString();
                //}
                //Accounts_Created
            }
            catch { }
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            //frmSettings settings = new frmSettings(server.Settings);
            //settings.ShowDialog();
        }

        private void Btn_ContextMenu_Click(object sender, EventArgs e)
        {
            ContextMenu.Show((Location.X) + 12, (Location.Y + Height) - (ContextMenu.Items.Count * 34 )); // - 113
        }

        private void CM_UsersManager_Click(object sender, EventArgs e)
        {
            try
            {
                //frmUserManager userManager = new frmUserManager();
                //userManager.ShowDialog();
            }
            catch (Exception ex)
            {
                //Log.Write(ex);
            }
            
        }

        private void ClearScreen_MouseClick(object sender, MouseEventArgs e)
        {
            Logger.ClearScreen();
        }

        private void AutoSave_Tick(object sender, EventArgs e)
        {
            //ilog.Debug("Exporting mongodb database.");
            //GameCoordinator.DbManager.ExportDatabase();

            //int Auto_Save_Time = GameCoordinator.Settings.Auto_Save_Time;
            //int interval = Auto_Save_Time == 0 ? 0 : ((1000 * 60) * 60) * Auto_Save_Time;
            //AutoSave.Interval = interval;
            //AutoSave.Start();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            int attrValue = 2;
            DwmApi.DwmSetWindowAttribute(base.Handle, 2, ref attrValue, 4);
            DwmApi.MARGINS mARGINS = default(DwmApi.MARGINS);
            mARGINS.cyBottomHeight = 1;
            mARGINS.cxLeftWidth = 0;
            mARGINS.cxRightWidth = 0;
            mARGINS.cyTopHeight = 0;
            DwmApi.MARGINS marInset = mARGINS;
            DwmApi.DwmExtendFrameIntoClientArea(base.Handle, ref marInset);

            Common.ShowShadow = false;
            shadow.Dock = DockStyle.None;
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnActivated(e);

            if (Common.ShowShadow)
            {
                shadow.Dock = DockStyle.Fill;
            }
        }
    }
}
