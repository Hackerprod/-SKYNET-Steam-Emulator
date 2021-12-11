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
        private bool _blur;

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
        public frmBase()
        {
            InitializeComponent();

        }
        //public virtual void ApplyTheme(ColorTheme theme)
        //{

        //}
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
        public virtual void LoadLanguage()
        {

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
    }
}
