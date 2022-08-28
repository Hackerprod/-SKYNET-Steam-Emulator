using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

public partial class Common
{
    public static Bitmap UpdatedAvatar { get; set; }

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

    public static int GetRandom()
    {
        return new Random().Next(1, 9999);
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

    public static string GetRootPath(string executablePath)
    {
        return new FileInfo(executablePath).Directory?.FullName;
    }

    public static bool Is64Bit => IntPtr.Size == 8;

    public static bool ShowShadow { get; set; }
    public static int BrowserHandle { get; set; }

    public static void OpenFolderAndSelectFile(string filePath)
    {
        if (filePath == null)
            return;

        IntPtr pidl = ILCreateFromPathW(filePath);
        SHOpenFolderAndSelectItems(pidl, 0, IntPtr.Zero, 0);
        ILFree(pidl);
    }
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr ILCreateFromPathW(string pszPath);

    [DllImport("shell32.dll")]
    private static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, int cild, IntPtr apidl, int dwFlags);

    [DllImport("shell32.dll")]
    private static extern void ILFree(IntPtr pidl);

    public static DialogResult Show(object msg, MessageBoxButtons buttons = MessageBoxButtons.OK)
    {
        string message = msg == null ? "NULL" : msg.ToString();
        return MessageBox.Show(message, "SKYNET", buttons);
    }

    public static string GetTotalTime(DateTime startTime)
    {
        TimeSpan duration = DateTime.Now - startTime;
        StringBuilder stringBuilder = new StringBuilder();
        if (duration.Days > 0)
        {
            stringBuilder.Append(duration.Days);
            stringBuilder.Append((duration.Days > 1) ? " days " : " day ");
        }
        stringBuilder.AppendFormat("{0:d2} : {1:d2} : {2:d2}", duration.Hours, duration.Minutes, duration.Seconds);
        return stringBuilder.ToString();
    }

    public static void InvokeAction(Control control, Action Action)
    {
        control.Invoke(Action);
    }

    public static string GetRandomString(int Length, bool Upper = false)
    {
        var Result = "";
        var random = new Random();
        var steps = new List<string>() { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "a", "s", "d", "f", "g", "h", "j", "k", "l", "z", "x", "c", "v", "b", "n", "m" };
        for (int i = 0; i < Length; i++)
        {
            int l = random.Next(0, 25);
            Result += steps[l];
        }
        return Upper ? Result.ToUpper() : Result;
    }
}