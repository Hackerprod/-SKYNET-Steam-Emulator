using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SKYNET.Properties;

namespace SKYNET.Controls
{
    public partial class SKYNET_Check : UserControl
    {
        [Category("SKYNET")]
        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
                CheckedChanged?.Invoke(this, _checked);
                pictureBox1.Image = value ? Resources.b_On : Resources.b_Off;
            }
        }
        bool _checked;

        [Category("SKYNET")]
        public event EventHandler<bool> CheckedChanged;

        public SKYNET_Check()
        {
            InitializeComponent();
        }

        private void T_CHECK_MouseClick(object sender, MouseEventArgs e)
        {
            Checked = !_checked;
            CheckedChanged?.Invoke(this, Checked);
            base.OnMouseClick(e);
        }
    }
}
