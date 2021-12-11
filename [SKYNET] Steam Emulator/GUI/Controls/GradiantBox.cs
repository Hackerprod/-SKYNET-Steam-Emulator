using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace SKYNET.Controls
{
    public partial class GradiantBox : UserControl
    {
        private Color _RigthColor;
        private Mode _Mode;
        public Mode Mode
        {
            get
            {
                return _Mode;
            }
            set
            {
                _Mode = value;
                PaintGradiant();
            }
        }
        public Color LeftColor
        {
            get
            {
                return _LeftColor;
            }
            set
            {
                _LeftColor = value;
                PaintGradiant();
            }
        }
        Color _LeftColor;

        public Color RigthColor
        {
            get
            {
                return _RigthColor;
            }
            set
            {
                _RigthColor = value;
                PaintGradiant();
            }
        }


        public GradiantBox()
        {
            InitializeComponent();
            Mode = Mode.Horizontal;
            PaintGradiant();
        }
        protected void PaintGradiant()
        {
            LinearGradientBrush gradBrush = new LinearGradientBrush(new Rectangle(0, 0, Width , Height), _LeftColor, _RigthColor, (float)Mode);

            Bitmap bmp = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(gradBrush, new Rectangle(0, 0, Width, Height));

            Box.Image = bmp;
        }

        private void Box_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        private void Box_MouseLeave(object sender, EventArgs e)
        {
            base.OnMouseLeave(e);
        }
    }

    public enum Mode : uint
    {
        Vertical = 90,
        Horizontal = 0
    }
}
