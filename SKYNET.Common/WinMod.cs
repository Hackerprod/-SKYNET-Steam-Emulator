using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SKYNET.Common
{
    public class WinMod
    {
        public static readonly IntPtr HWND_TOP = new IntPtr(0);
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private const int SW_SHOWMINNOACTIVE = 7;
        private const int SW_RESTORE = 9;
        private const int SWP_NOMOVE = 2;
        private const int SWP_NOSIZE = 1;
        private const int SWP_NOACTIVATE = 16;
        private const int SWP_SHOWWINDOW = 64;
        private const int SWP_NOOWNERZORDER = 32;
        private const int FLAGS = 3;
        private const int FLAGS1 = 0;

        [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int FlashWindow(int hwnd, int bInvert);

        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetWindowText(IntPtr hWnd, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpstring);

        [DllImport("user32", EntryPoint = "FindWindowA", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern long FindWindow([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpClassName, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SetFocus", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern long SetFocusAPI(long hWnd);

        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int cch);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern long OpenIcon(long hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern long GetActiveWindow();

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern IntPtr SetForegroundWindow(long hwnd);

        [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int SetFocus(int hwnd);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern bool BringWindowToTop(IntPtr HWnd);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern long SetWindowPos(IntPtr hwnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        [DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        public static void FlashWindow(long hwnd)
        {
            FlashWindow(checked((int)hwnd), 1);
        }

        public static void OpenIconHandle(object hw)
        {
            OpenIcon(Conversions.ToLong(hw));
        }

        public static long GetActiveWindowHandle()
        {
            return GetActiveWindow();
        }



        //public static bool IsActiveMainWindow()
        //{
        //    return (IntPtr)GetForegroundwindowHandle() == frmMain.frm.Handle;
        //}

        public static void SetForegroundwindowHandle(long hw)
        {
            SetForegroundWindow(hw);
        }

        public static void SetMinimizedNoFocusWindow(long hw)
        {
            ShowWindow((IntPtr)hw, 7);
        }

        public static void SetNormalNoFocusWindow(long hw)
        {
            ShowWindow((IntPtr)hw, 4);
        }

        public static void ExShowWindow(long hw, AppWinStyle windowStyle)
        {
            ShowWindow((IntPtr)hw, (int)windowStyle);
        }

        public static long FindwindowHandle(string ca)
        {
            string lpClassName = (string)null;
            return FindWindow(ref lpClassName, ref ca);
        }

        public static long GetForegroundwindowHandle()
        {
            return (long)GetForegroundWindow();
        }

        public static void SetActiveWindow(long hwnd)
        {
            try
            {

                MakeWindowTop(hwnd);
                SetFocus(checked((int)hwnd));
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                ProjectData.ClearProjectError();
            }
        }


        public static string GetCaption() //Obtiene el nombre de la ventana (SkynetChat)
        {
            StringBuilder lpString = new StringBuilder(256);
            GetWindowText(GetForegroundWindow(), lpString, lpString.Capacity);
            return lpString.ToString();
        }
        static long num;

        public static long BringWindowToTopAsInactiveform(long hwnd)        //Pone la ventana arriba a la izquierda
        {
            SetWindowPos((IntPtr)hwnd, HWND_TOPMOST, 0, 0, 0, 0, 0);        //Por ensima de lo demas
            //SetWindowPos((IntPtr)hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, 0);    //Normal
            return num;
        }

        public static void MakeWindowTop(long hwnd, bool topMost = false) //Pone la ventana arriba de las demas
        {
            SetFocus(checked((int)hwnd));
            if (topMost)
            {
                SetWindowPos((IntPtr)hwnd, HWND_TOP, 0, 0, 0, 0, 3);
                SetWindowPos((IntPtr)hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, 3);
                SetWindowPos((IntPtr)hwnd, HWND_TOPMOST, 0, 0, 0, 0, 3);
            }
            else
            {
                SetWindowPos((IntPtr)hwnd, HWND_TOPMOST, 0, 0, 0, 0, 3);
                SetWindowPos((IntPtr)hwnd, HWND_TOP, 0, 0, 0, 0, 3);
                SetWindowPos((IntPtr)hwnd, HWND_NOTOPMOST, 0, 0, 0, 0, 3);
            }
            SetFocus(checked((int)hwnd));
        }

        public static int GetInactiveTime()                                 //Captura inactividad del sistema
        {
            LASTINPUTINFO plii = new LASTINPUTINFO();
            plii.cbSize = checked((uint)Marshal.SizeOf((object)plii));
            plii.dwTime = 0U;
            return !GetLastInputInfo(ref plii) ? 0 : checked((int)Math.Round(unchecked((double)(checked((long)(Environment.TickCount & int.MaxValue) - (long)plii.dwTime & (long)int.MaxValue) & (long)int.MaxValue) / 1000.0)));
        }

        public struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }
    }

}
