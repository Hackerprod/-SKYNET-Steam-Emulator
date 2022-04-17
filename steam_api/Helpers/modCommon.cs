using SKYNET;
using SKYNET.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class modCommon
{
    public static bool LogToFile { get; set; }

    private static bool ConsoleEnabled;
    private static bool SettingsLoaded;


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

    public static string GenerateSteamID()
    {
        return "76561" + new Random().Next(100000, 999999) + new Random().Next(100000, 999999);
    }
    public static ulong CreateSteamID()
    {
        return ulong.Parse(GenerateSteamID());
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
}

namespace SKYNET.Helpers
{
}
