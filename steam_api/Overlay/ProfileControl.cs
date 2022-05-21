using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYNET.Helper;
using SKYNET.Steamworks.Implementation;
using SKYNET.Types;

namespace SKYNET.Overlay
{
    public partial class ProfileControl : UserControl
    {

        public ProfileControl(SteamPlayer User)
        {
            InitializeComponent();

            LB_PersonaName.Text = User.PersonaName;
            LB_AccountID.Text = User.AccountID.ToString();
            LB_SteamID.Text = User.SteamID.ToString();
            LB_IPAddress.Text = User.IPAddress.ToString();
            LB_GameID.Text = User.GameID.ToString();

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


    }
}
