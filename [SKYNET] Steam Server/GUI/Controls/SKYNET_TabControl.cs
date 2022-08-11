using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKYNET.Controls
{
    public class SKYNET_TabControl : TabControl
    {
        private bool _hideBorders;

        [Category("SKYNET")]
        public bool HideBorders
        {
            get { return _hideBorders; }
            set { _hideBorders = value; }
        }

        [Category("SKYNET")]
        public new Size ItemSize
        {
            get { return base.ItemSize; }
            set { base.ItemSize = value; }
        }

        [Category("SKYNET")]
        public new TabAlignment Alignment
        {
            get { return base.Alignment; }
            set { base.Alignment = value; }
        }
        public SKYNET_TabControl()
        {
            if (!this.DesignMode) this.Multiline = true;
        }

        protected override void WndProc(ref Message m)
        {
            if (HideBorders)
            {
                if (m.Msg == 0x1328 && !this.DesignMode)
                    m.Result = new IntPtr(1);
                else
                    base.WndProc(ref m);
            }
            else
                base.WndProc(ref m);
        }
    }
}
