﻿using SKYNET.Steamworks;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public partial class Common
{
    public static DateTime LoadTime { get; set; } = DateTime.Now;

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
                catch { }
            }
        }
    }

    public static void Show(object msg)
    {
        MessageBox.Show(msg.ToString());
    }

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

    public static string GetExecutablePath()
    {
        Process currentProcess;
        try
        {
            currentProcess = Process.GetCurrentProcess();
            return currentProcess.MainModule.FileName;
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
