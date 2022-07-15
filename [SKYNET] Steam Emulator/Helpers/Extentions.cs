using SKYNET.GUI;
using System.Windows.Forms;

namespace SKYNET.Helpers
{
    public static class Extentions
    {
        public static DialogResult Show(this modCommon _, object msg, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            return new frmMessage(msg, buttons).ShowDialog();
        }
    }
}
