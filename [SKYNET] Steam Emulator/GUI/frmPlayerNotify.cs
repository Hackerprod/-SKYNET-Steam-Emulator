using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using SKYNET.Network.Packets;
using SKYNET.GUI.Controls;
using System.Threading.Tasks;
using SKYNET.Types;

namespace SKYNET.GUI
{
    public partial class frmPlayerNotify : frmBase
    {
        private Rectangle rScreen;
        private int yy;
        private NET_GameOpened Game;

        public frmPlayerNotify(NET_GameOpened game, string personaName, Image avatar)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            Game = game;

            try
            {
                rScreen = Screen.GetWorkingArea(Screen.PrimaryScreen.Bounds);
                yy = checked(this.rScreen.Height - this.Height - yy);
                SetWindowPos(this.Handle, -1, checked(this.rScreen.Width - this.Width), yy, this.Width, this.Height, 16U);
            }
            catch { }

            Avatar.Image = avatar;
            LB_PersonaName.Text = personaName;
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
