using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SKYNET
{
    public partial class Form1 : Form
    {
        const int WM_NCPAINT = 0x85;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void DisableProcessWindowsGhosting();

        [DllImport("UxTheme.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

        public Form1()
        {
            //DisableProcessWindowsGhosting();
            InitializeComponent();

        }
        protected override void OnHandleCreated(EventArgs e)
        {
            //SetWindowTheme(this.Handle, "", "");
            base.OnHandleCreated(e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            const int WM_NCPAINT = 0x85;
            if (m.Msg == WM_NCPAINT)
            {
                IntPtr hdc = GetWindowDC(m.HWnd);
                if ((int)hdc != 0)
                {
                    Graphics g = Graphics.FromHdc(hdc);
                    g.FillRectangle(Brushes.Green, new Rectangle(0, 0, 4800, 23));
                    g.Flush();
                    ReleaseDC(m.HWnd, hdc);
                }
            }
        }
    }
}
