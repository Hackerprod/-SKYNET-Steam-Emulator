using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKYNET.GUI
{
    public partial class frmMessage : frmBase
    {
        public frmMessage(object msg, MessageBoxButtons Dialog = MessageBoxButtons.OK)
        {
            InitializeComponent();

            foreach (Control control in Controls)
            {
                base.SetMouseMove(control);
            }

            switch (Dialog)
            {
                case MessageBoxButtons.OKCancel:
                    break;
                case MessageBoxButtons.YesNo:
                    BT_Cancel.Text = "YES";
                    BT_Cancel.Text = "NO";
                    break;
                default:
                    BT_Cancel.Visible = false;
                    break;
            }
            LB_Message.Text = msg.ToString();
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            ok.PerformClick();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            cancel.PerformClick();
        }
    }
}
