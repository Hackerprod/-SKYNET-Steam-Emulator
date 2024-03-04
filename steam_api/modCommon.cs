using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SKYNET.Steamworks;

namespace SKYNET
{
    public class modCommon
    {
        private struct LASTINPUTINFO
        {
            public uint cbSize;

            public uint dwTime;
        }

        private static bool ConsoleEnabled;

        private static bool SettingsLoaded;

        public static bool LogToFile { get; set; }

        public static DateTime LoadTime { get; set; } = DateTime.Now;


        public static IntPtr GetObjectPtr(object Obj)
        {
            IntPtr zero = IntPtr.Zero;
            GCHandle value = GCHandle.Alloc(Obj, GCHandleType.WeakTrackResurrection);
            zero = Marshal.ReadIntPtr(GCHandle.ToIntPtr(value));
            value.Free();
            return zero;
        }

        public static void EnsureDirectoryExists(string filePath, bool isFile = false)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            filePath = filePath.Trim().Replace("\0", string.Empty);
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            try
            {
                string text = (isFile ? Path.GetDirectoryName(filePath) : filePath);
                if (Path.IsPathRooted(filePath))
                {
                    text = text.Trim();
                    if (!Directory.Exists(text))
                    {
                        Directory.CreateDirectory(text);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static void Show(object msg)
        {
            MessageBox.Show(msg.ToString());
        }

        public static void ActiveConsoleOutput()
        {
            if (!ConsoleEnabled)
            {
                ConsoleEnabled = true;
                AllocConsole();
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        public static ulong GenerateSteamID()
        {
            return (ulong)CSteamID.CreateOne();
        }

        public static string GetPath()
        {
            try
            {
                Process currentProcess = Process.GetCurrentProcess();
                return new FileInfo(currentProcess.MainModule.FileName).Directory?.FullName;
            }
            finally
            {
                Process currentProcess = null;
            }
        }

        public static bool Is64Bit()
        {
            return IntPtr.Size == 8;
        }

        public static int MilisecondTime()
        {
            return (DateTime.Now - LoadTime).Milliseconds;
        }

        public static uint ToUnixTime(DateTime t)
        {
            return (uint)new DateTimeOffset(t).ToUnixTimeSeconds();
        }

        public static IPAddress GetIPAddress(uint IP)
        {
            return new IPAddress(new byte[4]
            {
            (byte)(IP >> 24),
            (byte)(IP >> 16),
            (byte)(IP >> 8),
            (byte)IP
            });
        }

        public static int GetInactiveTime()
        {
            LASTINPUTINFO plii = default(LASTINPUTINFO);
            checked
            {
                plii.cbSize = (uint)Marshal.SizeOf((object)plii);
                plii.dwTime = 0u;
                return GetLastInputInfo(ref plii) ? ((int)Math.Round((double)((unchecked((long)(Environment.TickCount & 0x7FFFFFFF)) - unchecked((long)plii.dwTime)) & 0x7FFFFFFF & 0x7FFFFFFF) / 1000.0)) : 0;
            }
        }

        public static TimeSpan? GetInactiveTimeSpan()
        {
            LASTINPUTINFO plii = default(LASTINPUTINFO);
            checked
            {
                plii.cbSize = (uint)Marshal.SizeOf((object)plii);
                plii.dwTime = 0u;
                return (!GetLastInputInfo(ref plii)) ? null : new TimeSpan?(TimeSpan.FromMilliseconds(unchecked((long)Environment.TickCount) - unchecked((long)plii.dwTime)));
            }
        }

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
    }
}
