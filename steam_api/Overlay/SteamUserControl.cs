using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYNET.Types;
using SKYNET.Steamworks;
using SKYNET.Helper;
using SKYNET.Steamworks.Implementation;

namespace SKYNET.Overlay
{
    public partial class SteamUserControl : UserControl
    {
        public SteamPlayer User { get; }

        public SteamUserControl(SteamPlayer user)
        {
            InitializeComponent();
            
            User = user;
            LB_PersonaName.Text = User.PersonaName;
            LB_AccountID.Text = $"Account {(new CSteamID(User.SteamID)).ToString()}";
            LB_AppID.Text = $"Playing AppID {User.GameID}";
            LB_IPAddress.Text = $"IPAddress {User.IPAddress}";

            try
            {
                var avatarBytes = SteamFriends.Instance.GetAvatar(User.SteamID);
                if (avatarBytes != null && avatarBytes.Length > 0)
                {
                    var bitmap = ImageHelper.ImageFromBytes(avatarBytes);
                    PB_Avatar.Image = bitmap;
                }
            }
            catch
            {
            }
        }

        private void SteamUserControl_MouseMove(object sender, MouseEventArgs e)
        {
            BackColor = Color.FromArgb(70, 70, 70);
        }

        private void SteamUserControl_MouseLeave(object sender, EventArgs e)
        {
            BackColor = Color.FromArgb(60, 60, 60);
        }

        private void LB_Invite_MouseMove(object sender, MouseEventArgs e)
        {
            SteamUserControl_MouseMove(sender, e);
            LB_Invite.BackColor = Color.FromArgb(80, 80, 80);
        }

        private void LB_Invite_MouseLeave(object sender, EventArgs e)
        {
            LB_Invite.BackColor = Color.FromArgb(70, 70, 70);
        }
    }
}
