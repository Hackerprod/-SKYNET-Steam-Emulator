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
using System.Windows.Forms.Design;

namespace SKYNET
{
    [Designer(typeof(ControlDesigner))]

    public partial class SKYNET_ShadowBox : UserControl
    {
        private int opacity = 100;
        private int alpha;
        Color _color;


        [Category("SKYNET")]
        public int Opacity
        {
            get
            {
                if (opacity > 100) { opacity = 100; }
                else if (opacity < 1) { opacity = 1; }
                return this.opacity;
            }
            set
            {
                this.opacity = value;
                if (this.Parent != null) Parent.Invalidate(this.Bounds, true);
            }
        }

        [Category("SKYNET")]
        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }
        public SKYNET_ShadowBox()
        {
            InitializeComponent();

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            this.BackColor = Color.Transparent;
            _color = Color.FromArgb(3, 155, 229);

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics G = e.Graphics;
            Rectangle bounds = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            Brush brushColor;

            alpha = (opacity * 255) / 100;


            {
                brushColor = new SolidBrush(Color.FromArgb(alpha, _color));
            }

            Pen pen = new Pen(Color.FromArgb(alpha, _color));

            G.SmoothingMode = SmoothingMode.AntiAlias;

            GraphicsPath bgbtn = new GraphicsPath();
            bgbtn.AddRectangle(new RectangleF(0, 0, Width, Height));

            G.FillPath(brushColor, bgbtn);
            G.DrawPath(pen, bgbtn);



            base.OnPaint(e);

        }
        protected override void OnBackColorChanged(EventArgs e)
        {
            if (this.Parent != null) Parent.Invalidate(this.Bounds, true);
            base.OnBackColorChanged(e);
        }

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnParentBackColorChanged(e);
        }

    }
}
