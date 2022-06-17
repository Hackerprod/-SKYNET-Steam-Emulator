using System.Windows.Forms;

namespace SKYNET.GUI
{
    public partial class frmLogin : frmBase
    {
        public frmLogin()
        {
            InitializeComponent();
            base.SetMouseMove(PN_Top);
        }

        private void CloseBox_Clicked(object sender, System.EventArgs e)
        {
            Application.Exit();
        }

        private void MinimizeBox_Clicked(object sender, System.EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void BT_OfflineMode_Click(object sender, System.EventArgs e)
        {
            new frmMain().Show();
            Visible = false;
        }
    }
}
