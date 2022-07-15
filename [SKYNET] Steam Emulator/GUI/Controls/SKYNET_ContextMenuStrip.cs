using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace SKYNET.GUI.Controls
{
    public class SKYNET_ContextMenuStrip : ContextMenuStrip
    {
        public class TColorTable : ProfessionalColorTable
        {
            private Color _CheckedColor { get; set; }

            private Color BorderColor;

            [Category("Colors")]
            public Color CheckedColor
            {
                get
                {
                    return _CheckedColor;
                }
                set
                {
                    _CheckedColor = value;
                }
            }

            [Category("Colors")]
            public Color _BorderColor
            {
                get
                {
                    return BorderColor;
                }
                set
                {
                    BorderColor = value;
                }
            }

            public override Color CheckBackground => _CheckedColor;

            public override Color CheckPressedBackground => _CheckedColor;

            public override Color CheckSelectedBackground => _CheckedColor;

            public override Color ImageMarginGradientBegin => _CheckedColor;

            public override Color ImageMarginGradientEnd => _CheckedColor;

            public override Color ImageMarginGradientMiddle => _CheckedColor;

            public override Color MenuBorder => BorderColor;

            public override Color MenuItemBorder => BorderColor;

            public override Color MenuItemSelected => Color.FromArgb(53, 64, 78);

            public override Color SeparatorDark => Color.Red;

            public override Color ToolStripDropDownBackground => Azul;

            public Color Azul = Color.FromArgb(43, 54, 68);

            public TColorTable()
            {
                CheckedColor = Azul;
                BorderColor = Azul;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        public SKYNET_ContextMenuStrip()
        {
            base.Renderer = new ToolStripProfessionalRenderer(new TColorTable());
            base.ShowImageMargin = false;
            base.ForeColor = Color.White;
            Font = new Font("Segoe UI", 8f);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        }
    }
}
