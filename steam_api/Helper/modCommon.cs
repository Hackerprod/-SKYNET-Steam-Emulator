using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class modCommon
{
    private static bool ConsoleEnabled;

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

    public static void Show(object name)
    {
        MessageBox.Show(name.ToString());
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
}

