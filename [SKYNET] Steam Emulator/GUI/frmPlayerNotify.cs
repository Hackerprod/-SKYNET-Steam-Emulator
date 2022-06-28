using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using SKYNET.Network.Packets;
using SKYNET.GUI.Controls;
using System.Threading.Tasks;

namespace SKYNET.GUI
{
    public partial class frmPlayerNotify : frmBase
    {
        private Rectangle rScreen;
        private int yy;
        private NET_GameOpened Game;
        private SKYNET_UserControl User;

        public frmPlayerNotify(NET_GameOpened game, SKYNET_UserControl user)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            Game = game;
            User = user;

            try
            {
                rScreen = Screen.GetWorkingArea(Screen.PrimaryScreen.Bounds);
                yy = checked(this.rScreen.Height - this.Height - yy);
                SetWindowPos(this.Handle, -1, checked(this.rScreen.Width - this.Width), yy, this.Width, this.Height, 16U);
            }
            catch { }

            Avatar.Image = User.Avatar;
            LB_PersonaName.Text = User.SteamPlayer.PersonaName;
            LB_PlayingMsg.Text = "Is playing " + Game.Name + Environment.NewLine + Game.AppID;
            TopMost = true;

            //ShowWindow(this.Handle, 4);
            //WinMod.MakeWindowTop((long)this.Handle);
            //WinMod.BringWindowToTopAsInactiveform((long)this.Handle);

            Task.Run(() =>
            {
                Thread.Sleep(4000);
                Close();
            });
        }
       
        private void Controls_MouseClick(object sender, MouseEventArgs e)
        {
            //Close();
        }

        [DllImport("user32.dll")]
        protected static extern bool ShowWindow(IntPtr hWnd, int flags);

        [DllImport("user32.dll")]
        protected static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    }
}
