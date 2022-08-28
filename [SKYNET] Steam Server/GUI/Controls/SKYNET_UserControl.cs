using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SKYNET.Types;

namespace SKYNET.GUI.Controls
{
    public partial class SKYNET_UserControl : UserControl
    {
        private SteamPlayer steamPlayer;
        private Bitmap avatar;

        [Category("SKYNET")]
        public SteamPlayer SteamPlayer
        {
            get { return steamPlayer;  }
            set
            {
                steamPlayer = value;
                LB_PersonaName.Text = steamPlayer.PersonaName;
                //LB_IPAddress.Text   = steamPlayer.IPAddress;
            }
        }

        public Bitmap Avatar
        {
            get { return avatar; }
            set
            {
                avatar = value;
                PB_Avatar.Image = avatar;
            }
        }

        public SKYNET_UserControl()
        {
            InitializeComponent();
        }

        private void SKYNET_UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            BackColor = Color.FromArgb(43, 53, 63);
        }

        private void SKYNET_UserControl_MouseLeave(object sender, EventArgs e)
        {
            BackColor = Color.FromArgb(33, 43, 53);
        }
    }
}
