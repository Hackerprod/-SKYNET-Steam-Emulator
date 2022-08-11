using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SKYNET.GUI.Controls
{
    public class SKYNET_Label : Label
    {
        private float borderSize;
        private Color borderColor;

        private PointF point;
        private SizeF drawSize;
        private Pen drawPen;
        private GraphicsPath drawPath;
        private SolidBrush forecolorBrush;
        private bool createBorder;

        // Constructor
        //-----------------------------------------------------

        #region Constructor
        public SKYNET_Label()
        {
            this.borderSize = 1f;
            this.borderColor = Color.White;
            this.drawPath = new GraphicsPath();
            this.drawPen = new Pen(new SolidBrush(this.borderColor), borderSize);
            this.forecolorBrush = new SolidBrush(this.ForeColor);

            this.Invalidate();
        }
        #endregion

        // Public Properties
        //-----------------------------------------------------

        #region Public Properties
        /// <span class="code-SummaryComment"><summary></span>
        ///   The border's thickness
        /// <span class="code-SummaryComment"></summary></span>
        [Browsable(true)]
        [Category("SKYNET")]
        [Description("The border's thickness")]
        [DefaultValue(1f)]
        public float BorderSize
        {
            get { return this.borderSize; }
            set
            {
                this.borderSize = value;
                if (value == 0)
                {
                    //If border size equals zero, disable the
                    // border by setting it as transparent
                    this.drawPen.Color = Color.Transparent;
                }
                else
                {
                    this.drawPen.Color = this.BorderColor;
                    this.drawPen.Width = value;
                }

                this.OnTextChanged(EventArgs.Empty);
            }
        }

        /// <span class="code-SummaryComment"><summary></span>
        ///   The border color of this component
        /// <span class="code-SummaryComment"></summary></span>
        [Browsable(true)]
        [Category("SKYNET")]
        [DefaultValue(typeof(Color), "White")]
        [Description("The border color of this component")]
        public Color BorderColor
        {
            get { return this.borderColor; }
            set
            {
                this.borderColor = value;

                if (this.BorderSize != 0)
                    this.drawPen.Color = value;

                this.Invalidate();
            }
        }
        #endregion

        /// <span class="code-SummaryComment"><summary></span>
        ///   The border color of this component
        /// <span class="code-SummaryComment"></summary></span>
        [Browsable(true)]
        [Category("SKYNET")]
        [DefaultValue(typeof(bool), "false")]
        [Description("Create border for this component")]
        public bool CreateBorder
        {
            get { return this.createBorder; }
            set
            {
                this.createBorder = value;

                this.Invalidate();
            }
        }

        // Event Handling
        //-----------------------------------------------------

        #region Event Handling
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            this.Invalidate();
        }

        protected override void OnTextAlignChanged(EventArgs e)
        {
            base.OnTextAlignChanged(e);
            this.Invalidate();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            this.forecolorBrush.Color = base.ForeColor;
            base.OnForeColorChanged(e);
            this.Invalidate();
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            if (CreateBorder)
            {
                PaintBorder(e);
            }

        }

        private void PaintBorder(PaintEventArgs e)
        {
            // First let's check if we indeed have text to draw.
            //  if we have no text, then we have nothing to do.
            if (this.Text.Length == 0)
                return;

            // Secondly, let's begin setting the smoothing mode to AntiAlias, to
            // reduce image sharpening and compositing quality to HighQuality,
            // to improve our drawnings and produce a better looking image.

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            // Next, we measure how much space our drawning will use on the control.
            //  this is important so we can determine the correct position for our text.
            this.drawSize = e.Graphics.MeasureString(this.Text, this.Font, new PointF(),
                        StringFormat.GenericTypographic);

            // Now, we can determine how we should align our text in the control
            //  area, both horizontally and vertically. If the control is set to auto
            //  size itself, then it should be automatically drawn to the standard position.

            if (this.AutoSize)
            {
                this.point.X = this.Padding.Left;
                this.point.Y = this.Padding.Top;
            }
            else
            {
                // Text is Left-Aligned:
                if (this.TextAlign == ContentAlignment.TopLeft ||
                    this.TextAlign == ContentAlignment.MiddleLeft ||
                    this.TextAlign == ContentAlignment.BottomLeft)
                    this.point.X = this.Padding.Left;

                // Text is Center-Aligned
                else if (this.TextAlign == ContentAlignment.TopCenter ||
                    this.TextAlign == ContentAlignment.MiddleCenter ||
                    this.TextAlign == ContentAlignment.BottomCenter)
                    point.X = (this.Width - this.drawSize.Width) / 2;

                // Text is Right-Aligned
                else point.X = this.Width - (this.Padding.Right + this.drawSize.Width);

                // Text is Top-Aligned
                if (this.TextAlign == ContentAlignment.TopLeft ||
                    this.TextAlign == ContentAlignment.TopCenter ||
                    this.TextAlign == ContentAlignment.TopRight)
                    point.Y = this.Padding.Top;

                // Text is Middle-Aligned
                else if (this.TextAlign == ContentAlignment.MiddleLeft ||
                    this.TextAlign == ContentAlignment.MiddleCenter ||
                    this.TextAlign == ContentAlignment.MiddleRight)
                    point.Y = (this.Height - this.drawSize.Height) / 2;

                // Text is Bottom-Aligned
                else point.Y = this.Height - (this.Padding.Bottom + this.drawSize.Height);
            }

            // Now we can draw our text to a graphics path.
            //  
            //   PS: this is a tricky part: AddString() expects float emSize in pixel, 
            //   but Font.Size measures it as points.
            //   So, we need to convert between points and pixels, which in
            //   turn requires detailed knowledge of the DPI of the device we are drawing on. 
            //
            //   The solution was to get the last value returned by the 
            //   Graphics.DpiY property and
            //   divide by 72, since point is 1/72 of an inch, 
            //   no matter on what device we draw.
            //
            //   The source of this solution can be seen on CodeProject's article
            //   'OSD window with animation effect' - 
            //   http://www.codeproject.com/csharp/OSDwindow.asp

            float fontSize = e.Graphics.DpiY * this.Font.SizeInPoints / 72;

            this.drawPath.Reset();
            this.drawPath.AddString(this.Text, this.Font.FontFamily,
                            (int)this.Font.Style, fontSize,
                                point, StringFormat.GenericTypographic);

            // And finally, using our pen, all we have to do now
            //  is draw our graphics path to the screen. Voila!
            e.Graphics.FillPath(this.forecolorBrush, this.drawPath);
            e.Graphics.DrawPath(this.drawPen, this.drawPath);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.forecolorBrush != null)
                    this.forecolorBrush.Dispose();

                if (this.drawPath != null)
                    this.drawPath.Dispose();

                if (this.drawPen != null)
                    this.drawPen.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
