using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using SKYNET.Types;
using SKYNET.Helper;
using System.Drawing;
using System.Diagnostics;

namespace SKYNET.Overlay
{
    public partial class frmOverlay : Form
    {
        private const int WM_HOTKEY = 0x0312;
        private List<Types.SteamUser> Users;
        private ulong UserSteamID;
        private OverlayType OverlayType;

        private bool OverlayActive;
        private IntPtr hWndHandle;
        public struct RECT { public int left, top, right, bottom; }

        public frmOverlay()
        {
            InitializeComponent();
        }

        private void LoopPosition(object state)
        {
            while (OverlayActive)
            {
                try
                {
                    if (GetForegroundWindow() != hWndHandle && GetForegroundWindow() != Handle)
                    {
                        TopMost = false;
                    }
                    else
                    {
                        TopMost = true;
                        GetWindowRect(hWndHandle, out var rect);
                        Top = rect.top + 50;
                        Left = rect.left + 50;
                    }
                }
                catch
                {
                }
                Thread.Sleep(50);
            }
        }

        private void CreateUsersList()
        {

        }

        private void CreateProfile()
        {
            Types.SteamUser User = Users.Find(u => u.SteamId == UserSteamID);
            if (User == null)
            {
                ShowErrorMessage($"User with Steam id {UserSteamID} not found");
                return;
            }

            ProfileControl control = new ProfileControl(User);
            control.Dock = DockStyle.Fill;
            PN_Container.Controls.Add(control);
        }

        private void ShowErrorMessage(string msg)
        {
            Label LB_Message = new Label()
            {
                Font = new Font("Segoe UI Emoji", 12F, FontStyle.Bold, GraphicsUnit.Point, 0),
                ForeColor = Color.White,
                Location = new Point(23, 72),
                Size = new Size(654, 36),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = msg
            };
            Controls.Add(LB_Message);
            LB_Message.BringToFront();
        }

        private void FrmOverlay_Paint(object sender, PaintEventArgs e)
        {

        }

        private void PB_Close_Click(object sender, EventArgs e)
        {
            OverlayActive = false;
            modCommon.Overlay = null;
            Close();
        }

        private void PB_Close_MouseMove(object sender, MouseEventArgs e)
        {
            PB_Close.BackColor = Color.Red;
        }

        private void PB_Close_MouseLeave(object sender, EventArgs e)
        {
            PB_Close.BackColor = Color.Transparent;
        }

        private void Event_MouseMove(object sender, MouseEventArgs e)
        {
            //if (mouseDown)
            //{
            //    Location = new Point((Location.X - lastLocation.X) + e.X, (Location.Y - lastLocation.Y) + e.Y);
            //    Update();
            //}
        }

        private void Event_MouseDown(object sender, MouseEventArgs e)
        {
            //mouseDown = true;
            //lastLocation = e.Location;
        }

        private void Event_MouseUp(object sender, MouseEventArgs e)
        {
            //mouseDown = false;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public void ProcessOverlay(List<Types.SteamUser> users, OverlayType overlayType, ulong steamID)
        {
            OverlayActive = false;
            Users = users;
            UserSteamID = steamID;
            OverlayType = overlayType;
            OverlayActive = true;

            hWndHandle = FindWindow(null, Process.GetCurrentProcess().ProcessName);
            ThreadPool.QueueUserWorkItem(LoopPosition);

            switch (OverlayType)
            {
                case OverlayType.UsersList:
                    CreateUsersList();
                    break;
                case OverlayType.SteamProfile:
                    CreateProfile();
                    break;
                case OverlayType.Chat:
                    break;
                case OverlayType.JoinTrade:
                    break;
                case OverlayType.Stats:
                    break;
                case OverlayType.Achievements:
                    break;
                case OverlayType.FriendAdd:
                    break;
                case OverlayType.FriendRemove:
                    break;
                case OverlayType.FriendRequestAccept:
                    break;
                case OverlayType.FriendRequestIgnore:
                    break;
                default:
                    break;
            }
        }
    }
}
