using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SKYNET.GUI
{
    public partial class frmBase : Form
    {
        private bool mouseDown;
        private Point lastLocation;
        private Color _backColor;
        private bool _blur;
        private bool _shadows;
        private bool dragSize = true;

        const int HTLEFT = 10;
        const int HTRIGHT = 11;
        const int HTTOP = 12;
        const int HTTOPLEFT = 13;
        const int HTTOPRIGHT = 14;
        const int HTBOTTOM = 15;
        const int HTBOTTOMLEFT = 0x10;
        const int HTBOTTOMRIGHT = 17;
        const long WM_DRAG_SIZE = 0x0084;

        [Category("SKYNET")]
        public bool BlurEffect 
        {
            get { return _blur; }
            set 
            {
                _blur = value;
                if (value)
                {
                    BackColor = Color.Azure;
                    TransparencyKey = Color.Azure;
                    EnableBlur();
                }
                else
                {
                    BackColor = _backColor;
                }
            }
        }

        [Category("SKYNET")]
        [DefaultValue(typeof(bool), "true")]
        public bool DragSize { get { return dragSize; } set { dragSize = value; } }

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                if (BackColor != Color.Azure)
                {
                    _backColor = BackColor;
                }
            }
        }

        [Category("SKYNET")]
        public bool EnableShadows
        {
            get { return _shadows; }
            set { _shadows = value; }
        }

        public frmBase()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        private void EnableBlur()
        {
            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
            var accentStructSize = Marshal.SizeOf(accent);
            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);
            var Data = new WindowCompositionAttributeData();
            Data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            Data.SizeOfData = accentStructSize;
            Data.Data = accentPtr;
            SetWindowCompositionAttribute(this.Handle, ref Data);
            Marshal.FreeHGlobal(accentPtr);
        }

        public void SetMouseMove(Control control)
        {
            control.MouseMove += Event_MouseMove;
            control.MouseDown += Event_MouseDown;
            control.MouseUp += Event_MouseUp;
        }

        private void Event_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Location = new Point((Location.X - lastLocation.X) + e.X, (Location.Y - lastLocation.Y) + e.Y);
                Update();
            }
        }

        private void Event_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;

        }

        private void Event_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        [DllImport("user32.dll")]
        static internal extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        enum AccentState
        {
            ACCENT_ENABLE_BLURBEHIND = 3
        }
        struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlag;
            public int GradientColor;
            public int AnimationId;
        }
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }
        internal enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (EnableShadows)
            {
                int attrValue = 2;
                DwmSetWindowAttribute(base.Handle, 2, ref attrValue, 4);
                MARGINS marInset = new MARGINS()
                {
                    cyBottomHeight = 1,
                    cxLeftWidth = 0,
                    cxRightWidth = 0,
                    cyTopHeight = 0
                };
                DwmExtendFrameIntoClientArea(base.Handle, ref marInset);
            }
        }

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hdc, ref MARGINS marInset);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        public struct MARGINS
        {
            public int cxLeftWidth;

            public int cxRightWidth;

            public int cyTopHeight;

            public int cyBottomHeight;
        }

        #region Resize
        private const long WM_GETMINMAXINFO = 0x24;

        private struct POINTAPI
        {
            public int x;
            public int y;
        }

        private struct MINMAXINFO
        {
            public POINTAPI ptReserved;
            public POINTAPI ptMaxSize;
            public POINTAPI ptMaxPosition;
            public POINTAPI ptMinTrackSize;
            public POINTAPI ptMaxTrackSize;
        }

        public struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;//属性代表上、下、左、右
            public RECT rc;
            public IntPtr lParam;
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_GETMINMAXINFO)
            {
                this.MaximumSize = SystemInformation.WorkingArea.Size;
                MINMAXINFO mmi = (MINMAXINFO)m.GetLParam(typeof(MINMAXINFO));
                mmi.ptMinTrackSize.x = this.MinimumSize.Width;
                mmi.ptMinTrackSize.y = this.MinimumSize.Height;
                if (this.MaximumSize.Width != 0 || this.MaximumSize.Height != 0)
                {
                    mmi.ptMaxTrackSize.x = this.MaximumSize.Width;
                    mmi.ptMaxTrackSize.y = this.MaximumSize.Height;
                }
                //-------------------------
                int aaa = 0x00000005;
                APPBARDATA pdat = new APPBARDATA();
                SHAppBarMessage(aaa, ref pdat);

                if (pdat.uEdge == 0) //左
                {
                    mmi.ptMaxPosition.x = Screen.PrimaryScreen.Bounds.Width - SystemInformation.WorkingArea.Width;
                    mmi.ptMaxPosition.y = 0;
                }
                else if (pdat.uEdge == 1) //上
                {
                    mmi.ptMaxPosition.x = 0;
                    mmi.ptMaxPosition.y = Screen.PrimaryScreen.Bounds.Height - SystemInformation.WorkingArea.Height;
                }
                else if (pdat.uEdge == 2) //右
                {
                    mmi.ptMaxPosition.x = 0;
                    mmi.ptMaxPosition.y = 0;
                }
                else if (pdat.uEdge == 3) //下
                {
                    mmi.ptMaxPosition.x = 0;
                    mmi.ptMaxPosition.y = 0;
                }

                Marshal.StructureToPtr(mmi, m.LParam, true);
            }
            else if (DragSize && m.Msg == WM_DRAG_SIZE)
            {
                Point vPoint = new Point((int)m.LParam & 0xFFFF,
                           (int)m.LParam >> 16 & 0xFFFF);
                vPoint = PointToClient(vPoint);
                if (vPoint.X <= 5)
                    if (vPoint.Y <= 5)
                        m.Result = (IntPtr)HTTOPLEFT;
                    else if (vPoint.Y >= ClientSize.Height - 5)
                        m.Result = (IntPtr)HTBOTTOMLEFT;
                    else m.Result = (IntPtr)HTLEFT;
                else if (vPoint.X >= ClientSize.Width - 5)
                    if (vPoint.Y <= 5)
                        m.Result = (IntPtr)HTTOPRIGHT;
                    else if (vPoint.Y >= ClientSize.Height - 5)
                        m.Result = (IntPtr)HTBOTTOMRIGHT;
                    else m.Result = (IntPtr)HTRIGHT;
                else if (vPoint.Y <= 5)
                    m.Result = (IntPtr)HTTOP;
                else if (vPoint.Y >= ClientSize.Height - 5)
                    m.Result = (IntPtr)HTBOTTOM;
            }
        }

        [DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
        public static extern uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public override string ToString()
            {
                return "{left=" + left.ToString() + ", " + "top=" + top.ToString() + ", " +
                "right=" + right.ToString() + ", " + "bottom=" + bottom.ToString() + "}";
            }
        }

        #endregion
    }
}
