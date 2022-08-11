using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace SKYNET.GUI.Controls
{
    public class SKYNET_Button : Control
    {
        [Category("SKYNET")]
        public bool MenuMode
        {
            get { return _menuMode; }
            set { _menuMode = value; }
        }
        private bool _menuMode;

        [Flags]
        public enum _Style
        {
            TextOnly = 0x0,
            ImageIconOnly = 0x1,
            ImageIconWithText = 0x2
        }
        private Color _BackColorMouseOver { get; set; }
        private Color _ForeColor { get; set; }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Refresh();
        }
        public void DoClick()
        {
            base.OnClick(new EventArgs());
        }
        public Color BackColorMouseOver
        {
            get
            {
                return _BackColorMouseOver;
            }
            set
            {
                _BackColorMouseOver = value;
            }
        }
        private Color _ForeColorMouseOver { get; set; }

        public Color ForeColorMouseOver
        {
            get
            {
                return _ForeColorMouseOver;
            }
            set
            {
                _ForeColorMouseOver = value;
            }
        }
        [Flags]
        public enum _ImgAlign
        {
            Left = 0x0,
            Center = 0x1,
            Right = 0x2
        }

        private int W;

        private int H;

        private int _ImageX;

        private int _ImageY;

        private bool _Rounded;

        private _Style _ButtonStyle;

        private Image _ImageIcon;

        public _ImgAlign _ImageAlign;

        private MouseState State;
        private bool first;

        [Category("SKYNET")]
        public bool Rounded
        {
            get
            {
                return _Rounded;
            }
            set
            {
                _Rounded = value;
            }
        }

        [Category("SKYNET")]
        public _Style Style
        {
            get
            {
                return _ButtonStyle;
            }
            set
            {
                _ButtonStyle = value;
            }
        }

        [Category("SKYNET")]
        public Image ImageIcon
        {
            get
            {
                return _ImageIcon;
            }
            set
            {
                _ImageIcon = value;
            }
        }

        [Category("SKYNET")]
        public _ImgAlign ImageAlignment
        {
            get
            {
                return _ImageAlign;
            }
            set
            {
                _ImageAlign = value;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            State = MouseState.Down;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            State = MouseState.Over;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            State = MouseState.Over;
            Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Invalidate();
            //ForeColor = ForeColorMouseOver;
        }


        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            State = MouseState.None;
            Invalidate();
            //ForeColor = _ForeColor;
        }

        public SKYNET_Button()
        {
            _Rounded = false;
            _ButtonStyle = _Style.TextOnly;
            _ImageAlign = _ImgAlign.Left;
            State = MouseState.None;
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
            DoubleBuffered = true;
            base.Size = new Size(100, 32);
            BackColor = Color.Transparent;
            Font = new Font("Segoe UI", 9f);
            Cursor = Cursors.Hand;
            _ButtonStyle = _Style.TextOnly;
            _ImageAlign = _ImgAlign.Left;
            _ImageX = 0;
            _ImageY = 0;
            _ImageIcon = null;
            _ForeColor = ForeColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (MenuMode)
            {
                MenuPaint(e);
                return;
            }
            if (first)
            {
                _ForeColor = ForeColor;
                first = false;
            }
            Bitmap B = new Bitmap(base.Width, base.Height);
            Graphics G = Graphics.FromImage(B);
            checked
            {
                W = base.Width - 1;
                H = base.Height - 1;
                if (_ButtonStyle == _Style.ImageIconOnly)
                {
                    if (_ImageAlign == _ImgAlign.Left)
                    {
                        _ImageX = 3;
                        _ImageY = (int)Math.Round(unchecked(Math.Round((double)base.Height / 2.0) - Math.Round((double)_ImageIcon.Height / 2.0)));
                    }
                    else if (_ImageAlign == _ImgAlign.Center)
                    {
                        _ImageX = (int)Math.Round(unchecked(Math.Round((double)base.Width / 2.0) - Math.Round((double)_ImageIcon.Width / 2.0)));
                        _ImageY = (int)Math.Round(unchecked(Math.Round((double)base.Height / 2.0) - Math.Round((double)_ImageIcon.Height / 2.0)));
                    }
                    else if (_ImageAlign == _ImgAlign.Right)
                    {
                        _ImageX = base.Width - _ImageIcon.Width - 3;
                        _ImageY = (int)Math.Round(unchecked(Math.Round((double)base.Height / 2.0) - Math.Round((double)_ImageIcon.Height / 2.0)));
                    }
                }
                else if (_ButtonStyle == _Style.ImageIconWithText)
                {
                    if (_ImageAlign == _ImgAlign.Left)
                    {
                        _ImageX = 5;
                        _ImageY = (int)Math.Round(unchecked(Math.Round((double)base.Height / 2.0) - Math.Round((double)_ImageIcon.Height / 2.0))) + 1;
                    }
                    else if (_ImageAlign == _ImgAlign.Center)
                    {
                        _ImageX = (int)Math.Round(unchecked(Math.Round((double)base.Width / 2.0) - Math.Round((double)_ImageIcon.Width / 2.0)));
                        _ImageY = (int)Math.Round(unchecked(Math.Round((double)base.Height / 2.0) - Math.Round((double)_ImageIcon.Height / 2.0)));
                    }
                    else if (_ImageAlign == _ImgAlign.Right)
                    {
                        _ImageX = base.Width - _ImageIcon.Width - 3;
                        _ImageY = (int)Math.Round(unchecked(Math.Round((double)base.Height / 2.0) - Math.Round((double)_ImageIcon.Height / 2.0)));
                    }
                }
                else if (_ButtonStyle == _Style.TextOnly)
                {
                    _ImageX = 0;
                    _ImageY = 0;
                }
                GraphicsPath graphicsPath = new GraphicsPath();
                Rectangle rectangle = new Rectangle(0, 0, W, H);
                new Point((int)Math.Round(Math.Round(unchecked((double)W / 2.0))), (int)Math.Round(Math.Round(unchecked((double)H / 2.0))));
                Graphics g = G;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.Clear(BackColor);
                switch (State)
                {
                    case MouseState.None:
                        if (Rounded)
                        {
                            graphicsPath = RoundRec(rectangle, 6);
                            g.FillPath(new SolidBrush(BackColor), graphicsPath);
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(BackColor), rectangle);
                        }
                        break;
                    case MouseState.Over:
                        if (Rounded)
                        {
                            graphicsPath = RoundRec(rectangle, 6);
                            g.FillPath(new SolidBrush(BackColor), graphicsPath);
                            g.FillPath(new SolidBrush(Color.FromArgb(20, Color.White)), graphicsPath);
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(BackColor), rectangle);
                            g.FillRectangle(new SolidBrush(Color.FromArgb(20, Color.White)), rectangle);
                        }
                        break;
                    case MouseState.Down:
                        if (Rounded)
                        {
                            graphicsPath = RoundRec(rectangle, 6);
                            g.FillPath(new SolidBrush(BackColor), graphicsPath);
                            g.FillPath(new SolidBrush(Color.FromArgb(20, Color.Black)), graphicsPath);
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(BackColor), rectangle);
                            g.FillRectangle(new SolidBrush(Color.FromArgb(20, Color.Black)), rectangle);
                        }
                        break;
                }
                if (_ButtonStyle == _Style.TextOnly)
                {
                    g.DrawString(Text, Font, new SolidBrush(ForeColor), rectangle, CenterSF);
                }
                else if (_ButtonStyle == _Style.ImageIconOnly)
                {
                    g.DrawImage(_ImageIcon, _ImageX, _ImageY, _ImageIcon.Width, _ImageIcon.Height);
                }
                else if (_ButtonStyle == _Style.ImageIconWithText)
                {
                    g.DrawImage(_ImageIcon, _ImageX, _ImageY, _ImageIcon.Width, _ImageIcon.Height);
                    g.DrawString(Text, Font, new SolidBrush(ForeColor), rectangle, CenterSF);
                }
                g = null;
                base.OnPaint(e);
                G.Dispose();
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImageUnscaled(B, 0, 0);
                B.Dispose();
            }
        }
        private void MenuPaint(PaintEventArgs e)
        {
            Bitmap B = new Bitmap(base.Width, base.Height);
            Graphics G = Graphics.FromImage(B);
            W = base.Width;
            H = base.Height;
            Rectangle rect = new Rectangle(0, 0, W, H);
            checked
            {
                Rectangle rect2 = new Rectangle(W - 40, 0, W, H);
                GraphicsPath graphicsPath = new GraphicsPath();
                G.SmoothingMode = SmoothingMode.HighQuality;
                G.PixelOffsetMode = PixelOffsetMode.HighQuality;
                G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                G.FillRectangle(new SolidBrush(BackColor), rect);
                graphicsPath.Reset();
                graphicsPath.AddRectangle(rect2);
                G.SetClip(graphicsPath);

                switch (State)
                {
                    case MouseState.Over:
                    case MouseState.Down:
                        {
                            G.FillRectangle(new SolidBrush(BackColor), rect2); // Color del cuadro a la derecha
                            G.ResetClip();
                            G.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), W - 10, 6, W - 30, 6);
                            G.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), W - 10, 12, W - 30, 12);
                            G.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), W - 10, 18, W - 30, 18);
                        }
                        break;
                    default:
                        {
                            G.FillRectangle(new SolidBrush(BackColor), rect2); // Color del cuadro a la derecha
                            G.ResetClip();
                            G.DrawLine(new Pen(Color.FromArgb(200, 200, 200)), W - 10, 6, W - 30, 6);
                            G.DrawLine(new Pen(Color.FromArgb(200, 200, 200)), W - 10, 12, W - 30, 12);
                            G.DrawLine(new Pen(Color.FromArgb(200, 200, 200)), W - 10, 18, W - 30, 18);
                        }
                        break;
                }
                G.Dispose();
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImageUnscaled(B, 0, 0);
                B.Dispose();
            }
        }
        internal static StringFormat NearSF = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Near
        };
        public static GraphicsPath RoundRec(Rectangle Rectangle, int Curve)
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            checked
            {
                int num = Curve * 2;
                graphicsPath.AddArc(new Rectangle(Rectangle.X, Rectangle.Y, num, num), -180f, 90f);
                graphicsPath.AddArc(new Rectangle(Rectangle.Width - num + Rectangle.X, Rectangle.Y, num, num), -90f, 90f);
                graphicsPath.AddArc(new Rectangle(Rectangle.Width - num + Rectangle.X, Rectangle.Height - num + Rectangle.Y, num, num), 0f, 90f);
                graphicsPath.AddArc(new Rectangle(Rectangle.X, Rectangle.Height - num + Rectangle.Y, num, num), 90f, 90f);
                graphicsPath.AddLine(new Point(Rectangle.X, Rectangle.Height - num + Rectangle.Y), new Point(Rectangle.X, Curve + Rectangle.Y));
                return graphicsPath;
            }
        }
        internal static StringFormat CenterSF = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        internal enum MouseState : byte
        {
            None,
            Over,
            Down,
            Block
        }

    }
}