using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using SKYNET.Client;

namespace SKYNET.GUI
{
    public partial class frmUpdateProfile : frmBase
    {
        public static frmUpdateProfile frm;
        public byte[] ImageBytes { get; set; }
        public Bitmap UpdatedAvatar { get; set; }

        public frmUpdateProfile()
        {
            InitializeComponent();
            frm = this;
            CheckForIllegalCrossThreadCalls = false;
            base.SetMouseMove(panelTop);

            //modCommon.frmUpdateProfile = this;
            //modCommon.frmLogin.Visible = false;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            UserName.Text = SteamClient.PersonaName;
            //Password.Text = frmMain.Password;
            PB_Avatar.Image = SteamClient.Avatar;
            TB_AccountID.Text = SteamClient.AccountID.ToString();
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

            if (uint.TryParse(TB_AccountID.Text, out uint accountID))
            {
                if (accountID != SteamClient.AccountID)
                    frmMain.AccountIDUpdated(accountID);
            }
            else
            {
                modCommon.Show("Please... set a valid account ID");
                return;
            }

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
