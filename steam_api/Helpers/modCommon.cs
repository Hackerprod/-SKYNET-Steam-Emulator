using SKYNET;
using SKYNET.Overlay;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class modCommon
{
#if WIN32
        public const int StructPackSize = 4;
#else
    public const int StructPackSize = 8;
#endif

    public static bool LogToFile { get; set; }

    private static bool ConsoleEnabled;
    private static bool SettingsLoaded;

    public static DateTime LoadTime { get; set; } = DateTime.Now;
    public static frmOverlay Overlay;



    public static IntPtr GetObjectPtr(object Obj)
    {
        IntPtr Ptr = IntPtr.Zero;
        GCHandle gcHandle = GCHandle.Alloc(Obj, GCHandleType.WeakTrackResurrection);
        Ptr = Marshal.ReadIntPtr(GCHandle.ToIntPtr(gcHandle));
        gcHandle.Free();
        return Ptr;
    }
    public static void EnsureDirectoryExists(string filePath, bool isFile = false)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            filePath = filePath.Trim().Replace("\0", string.Empty);
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    string text = isFile ? Path.GetDirectoryName(filePath) : filePath;
                    if (Path.IsPathRooted(filePath))
                    {
                        text = text.Trim();
                        if (!Directory.Exists(text))
                        {
                            Directory.CreateDirectory(text);
                        }
                    }
                }
                catch (Exception exception)
                {

                }
            }
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
    static extern bool AllocConsole();

    public static ulong GenerateSteamID()
    {
        return (ulong)CSteamID.CreateOne();
    }

    public static string GetPath()
    {
        Process currentProcess;
        try
        {
            currentProcess = Process.GetCurrentProcess();
            return new FileInfo(currentProcess.MainModule.FileName).Directory?.FullName;
        }
        finally { currentProcess = null; }
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
        return (uint)(new DateTimeOffset(t)).ToUnixTimeSeconds();
    }

    public static IPAddress GetIPAddress(uint IP)
    {
        return new IPAddress(new byte[] { (byte)(IP >> 24), (byte)(IP >> 16), (byte)(IP >> 8), (byte)IP });
    }

    public static int GetInactiveTime()                         
    {
        LASTINPUTINFO plii = new LASTINPUTINFO();
        plii.cbSize = checked((uint)Marshal.SizeOf((object)plii));
        plii.dwTime = 0U;
        return !GetLastInputInfo(ref plii) ? 0 : checked((int)Math.Round(unchecked((double)(checked((long)(Environment.TickCount & int.MaxValue) - (long)plii.dwTime & (long)int.MaxValue) & (long)int.MaxValue) / 1000.0)));
    }

    public static TimeSpan? GetInactiveTimeSpan()
    {
        LASTINPUTINFO plii = new LASTINPUTINFO();
        plii.cbSize = checked((uint)Marshal.SizeOf((object)plii));
        plii.dwTime = 0U;
        return !GetLastInputInfo(ref plii) ? new TimeSpan?() : new TimeSpan?(TimeSpan.FromMilliseconds((double)checked((long)Environment.TickCount - (long)plii.dwTime)));
    }

    [DllImport("user32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);


    private struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }
}
