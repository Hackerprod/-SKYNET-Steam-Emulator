using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SKYNET.Helpers
{
    public class ConsoleHelper
    {
        #region Native

        static IntPtr h = GetStdHandle(STD_OUTPUT_HANDLE);
        static CharInfo[] buf = new CharInfo[80 * 25];
        static SmallRect rect = new SmallRect() { Left = 0, Top = 0, Right = 80, Bottom = 25 };
        const int STD_OUTPUT_HANDLE = -11;

        internal struct CharInfo
        {
            public ushort Char;

            public short Attributes;
        }

        internal struct SmallRect
        {
            public short Left;

            public short Top;

            internal short Right;

            internal short Bottom;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleTitleA(string hTittle);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteConsoleW(IntPtr hConsoleOutput, string lpBuffer, int nNumberOfCharsToWrite, out int lpNumberOfCharsWritten, IntPtr lpReservedMustBeNull);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
        string fileName,
        [MarshalAs(UnmanagedType.U4)] uint fileAccess,
        [MarshalAs(UnmanagedType.U4)] uint fileShare,
        IntPtr securityAttributes,
        [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
        [MarshalAs(UnmanagedType.U4)] int flags,
        IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutput(
          IntPtr hConsoleOutput,
          CharInfo[] lpBuffer,
          Coord dwBufferSize,
          Coord dwBufferCoord,
          ref SmallRect lpWriteRegion);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;
        }
        #endregion
        public static void CreateConsole(string Tittle = "")
        {
            AllocConsole();

            if (!string.IsNullOrEmpty(Tittle))
            {
                SetConsoleTitleA(Tittle);
            }
        }

        public static void SetConsoleTitle(string Tittle)
        {
            SetConsoleTitleA(Tittle);
        }

        public static void WriteLine(object msg, ConsoleColor color = ConsoleColor.White)
        {
            try
            {
                byte x = 0, y = 0;
                string a = msg.ToString();
                for (int ci = 0; ci < a.Length; ci++)
                {
                    switch (a[ci])
                    {
                        case '\n': // newline char, move to next line, aka y=y+1
                            y++;
                            break;
                        case '\r': // carriage return, aka back to start of line
                            x = 0;
                            break;
                        case ' ': // a space, move the cursor to the right
                            x++;
                            break;
                        default:
                            // calculate where we should be in the buffer
                            int i = y * 80 + x;
                            // color
                            buf[i].Attributes = (short)color;
                            // put the current char from the string in the buffer
                            buf[i].Char = (byte)a[ci];
                            x++;
                            break;
                    }
                }
                // we handled our string, let's write the whole screen at once
                bool success = WriteConsoleOutput(h, buf, new Coord() { X = 80, Y = 25 }, new Coord() { X = 0, Y = 0 }, ref rect);
            }
            catch 
            {
                Console.WriteLine(msg);
            }
        }

        public static void SetAsConsoleOutput(RichTextBox textBox)
        {
            var wrapper = new ConsoleWrapper(textBox);
            Console.SetOut(wrapper);
        }

        private class ConsoleWrapper : TextWriter
        {
            private readonly RichTextBox control;

            public override Encoding Encoding => Encoding.UTF8;

            [DllImport("user32.dll")]
            public static extern bool LockWindowUpdate(IntPtr hWndLock);

            public ConsoleWrapper(RichTextBox _control)
            {
                control = _control;
            }

            public void SetAsConsoleOutput()
            {
                Console.SetOut(this);
            }

            public override void Write(char value)
            {
                try
                {
                    control.Invoke((Action<RichTextBox>)delegate
                    {
                        LockWindowUpdate(control.Handle);
                        control.AppendText(value.ToString() ?? "");
                        LockWindowUpdate(IntPtr.Zero);
                    });
                }
                catch
                {
                }
            }

            public override void Write(string value)
            {
                try
                {
                    control.Invoke((Action<RichTextBox>)delegate
                    {
                        LockWindowUpdate(control.Handle);
                        control.AppendText(value);
                        control.ScrollToCaret();
                        LockWindowUpdate(IntPtr.Zero);
                    });
                }
                catch
                {
                }
            }
        }
    }
}
