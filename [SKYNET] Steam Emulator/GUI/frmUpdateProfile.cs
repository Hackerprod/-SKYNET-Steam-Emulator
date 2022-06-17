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
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Threading;
using System.Timers;
using SKYNET.GUI;
using SKYNET.Client;

namespace SKYNET
{
    [ComVisibleAttribute(true)]
    public partial class frmUpdateProfile : frmBase
    {
        private readonly Dictionary<string, string> UsersAndIds = new Dictionary<string, string>();
        public static frmUpdateProfile frm;
        private readonly System.Timers.Timer ErrorTimer;
        public byte[] ImageBytes { get; set; }
        public Bitmap UpdatedAvatar { get; set; }

        public frmUpdateProfile()
        {
            modCommon.ShowShadow = true;
            InitializeComponent();
            frm = this;
            CheckForIllegalCrossThreadCalls = false;
            base.SetMouseMove(panelTop);

            ErrorTimer = new System.Timers.Timer();
            ErrorTimer.AutoReset = false;
            ErrorTimer.Elapsed += this._timer_Elapsed;

            //modCommon.frmUpdateProfile = this;
            //modCommon.frmLogin.Visible = false;

            LoadLanguage();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            UserName.Text = SteamClient.PersonaName;
            //Password.Text = frmMain.Password;
            PB_Avatar.Image = SteamClient.Avatar;
        }
        
        private void Logo_Timer_Tick(object sender, EventArgs e)
        {
            if (Logo.Size.Width < 105)
            {
                Logo.Size = new Size(Logo.Size.Width + 5, Logo.Size.Width + 5);
            }
            else
            {
                Logo.Size = new Size(105, 105);
                Logo_Timer.Enabled = false;
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ErrorLabel.Text = "";
            ErrorLabel.Visible = false;
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Apply_Button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(UserName.Text) | string.IsNullOrEmpty(Password.Text))
            {
                //modCommon.Show("Please... fill all data.");
                //return;
            }
            if (Password.Text.Length < 5)
            {
                //modCommon.Show("The password is very short...");
                //return;
            }

            if (UpdatedAvatar != null)
                frmMain.AvatarUpdated(UpdatedAvatar);

            if (UserName.Text != SteamClient.PersonaName)
                frmMain.PersonaNameUpdated(UserName.Text);

            //if (Password.Text != SteamClient.Password)
            //    frmMain.PasswordUpdated(Password.Text);

            Close();
        }

        private void PanelBody_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void Avatar_MouseClick(object sender, MouseEventArgs e)
        {
            var ofdPhoto = new OpenFileDialog();
            ofdPhoto.FileName = string.Empty;
            ofdPhoto.Filter = "Picture files|*.png;*.jpg;*.bmp;*.gif|All Files|*.*";
            ofdPhoto.Title = "Select Photo";
            ofdPhoto.RestoreDirectory = true;
            DialogResult num = ofdPhoto.ShowDialog();

            if (num == DialogResult.OK)
            {
                WindowState = FormWindowState.Minimized;

                frmCropEditor editor = new frmCropEditor(ofdPhoto.FileName);
                editor.BringToFront();
                editor.Activate();
                var CropResult = editor.ShowDialog();
                if (CropResult == DialogResult.OK)
                {
                    if (UpdatedAvatar != null)
                    {
                        PB_Avatar.Image = UpdatedAvatar;
                    }
                }
            }
            WindowState = FormWindowState.Normal;

            try
            {
                PB_Avatar.Image = Image.FromStream(new MemoryStream(ImageBytes));
            }
            catch { }
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
        }
    }
}
