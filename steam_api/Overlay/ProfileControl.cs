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

namespace SKYNET.Overlay
{
    public partial class ProfileControl : UserControl
    {

        public ProfileControl(Types.SteamUser User)
        {
            InitializeComponent();

            LB_PersonaName.Text = User.PersonaName;
            LB_AccountID.Text = User.AccountId.ToString();
            LB_SteamID.Text = User.SteamId.ToString();
            LB_IPAddress.Text = User.IPAddress.ToString();
            LB_GameID.Text = User.GameId.ToString();

            try
            {
                var avatarBytes = SteamEmulator.SteamFriends.GetAvatar(User.SteamId);
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
