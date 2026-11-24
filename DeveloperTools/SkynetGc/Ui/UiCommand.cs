using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SKYNET.Gc.Ui;

/// <summary>
/// Resolution-independent UI primitives for an agent. Coordinates are fractions (0..1) of the game
/// window's client area, so nothing depends on resolution or Dota version.
///   ui shot [file.png]        capture the Dota window client area
///   ui click X Y [--right]    click at fractional position
///   ui key KEYS               SendKeys syntax, e.g. "{ENTER}" or "~"
///   ui type TEXT              type literal text
///   ui console CMD            open the console (`) and run a command
/// </summary>
internal static class UiCommand
{
    public static int Run(string[] a)
    {
        var verb = a.Length > 0 ? a[0] : "";
        MinimizeServerConsoles();
        var hwnd = FindWindow();
        if (hwnd == IntPtr.Zero) { Console.Error.WriteLine("No Dota 2 window found."); return 2; }
        ShowWindow(hwnd, 9);
        SetForegroundWindow(hwnd);
        Thread.Sleep(150);
        GetClientRect(hwnd, out var rc);
        var origin = new POINT();
        ClientToScreen(hwnd, ref origin);

        switch (verb)
        {
            case "shot":
            {
                var file = a.Length > 1 ? a[1] : Path.Combine(Path.GetTempPath(), "dota-shot.png");
                using (var bmp = new Bitmap(rc.Right, rc.Bottom))
                {
                    using (var g = Graphics.FromImage(bmp)) g.CopyFromScreen(origin.X, origin.Y, 0, 0, bmp.Size);
                    bmp.Save(file, ImageFormat.Png);
                }
                Console.WriteLine(file + " " + rc.Right + "x" + rc.Bottom);
                return 0;
            }
            case "click":
            {
                var x = double.Parse(a[1], System.Globalization.CultureInfo.InvariantCulture);
                var y = double.Parse(a[2], System.Globalization.CultureInfo.InvariantCulture);
                var right = a.Contains("--right");
                var sx = origin.X + (int)(x * rc.Right); var sy = origin.Y + (int)(y * rc.Bottom);
                if (!GameIsAt(hwnd, sx, sy)) { Console.Error.WriteLine("The game window is not at the click position (obscured); not clicking."); return 3; }
                SetCursorPos(sx, sy);
                Thread.Sleep(80);
                mouse_event(right ? 0x8u : 0x2u, 0, 0, 0, UIntPtr.Zero);
                Thread.Sleep(60);
                mouse_event(right ? 0x10u : 0x4u, 0, 0, 0, UIntPtr.Zero);
                return 0;
            }
            case "key": SendKeys.SendWait(a[1]); return 0;
            case "type": SendKeys.SendWait(Escape(string.Join(" ", a.Skip(1)))); return 0;
            case "console":
                SendKeys.SendWait("`"); Thread.Sleep(300);
                SendKeys.SendWait(Escape(string.Join(" ", a.Skip(1))) + "{ENTER}"); Thread.Sleep(200);
                SendKeys.SendWait("`");
                return 0;
        }
        Console.Error.WriteLine("ui: shot | click X Y | key KEYS | type TEXT | console CMD");
        return 2;
    }

    private static string Escape(string s)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var c in s) sb.Append("+^%~(){}[]".IndexOf(c) >= 0 ? "{" + c + "}" : c.ToString());
        return sb.ToString();
    }

    private static IntPtr FindWindow()
    {
        // A dedicated server can run beside the client; prefer the window with the largest client area.
        IntPtr best = IntPtr.Zero; long bestArea = -1;
        foreach (var p in Process.GetProcessesByName("dota2"))
        {
            if (p.MainWindowHandle == IntPtr.Zero || IsConsoleWindow(p.MainWindowHandle) || !GetClientRect(p.MainWindowHandle, out var r)) continue;
            long area = (long)r.Right * r.Bottom;
            if (area > bestArea) { bestArea = area; best = p.MainWindowHandle; }
        }
        return best;
    }

    // A dedicated server started by the emulator opens its own console window, which would cover the game.
    private static void MinimizeServerConsoles()
    {
        var dedicated = DedicatedPids();
        foreach (var p in Process.GetProcessesByName("dota2"))
            if (p.MainWindowHandle != IntPtr.Zero && (dedicated.Contains(p.Id) || IsConsoleWindow(p.MainWindowHandle)))
                ShowWindow(p.MainWindowHandle, 6);
    }

    // dota2.exe started with -dedicated is the server, never the game window to drive.
    private static HashSet<int> DedicatedPids()
    {
        var result = new HashSet<int>();
        using (var s = new System.Management.ManagementObjectSearcher("SELECT ProcessId, CommandLine FROM Win32_Process WHERE Name = 'dota2.exe'"))
        foreach (System.Management.ManagementObject o in s.Get())
        {
            var cl = o["CommandLine"] as string ?? "";
            if (cl.IndexOf("-dedicated", StringComparison.OrdinalIgnoreCase) >= 0) result.Add(Convert.ToInt32(o["ProcessId"]));
        }
        return result;
    }

    // Never click into a window that is not the game (a console window would freeze the server).
    private static bool GameIsAt(IntPtr hwnd, int x, int y)
    {
        var top = WindowFromPoint(new POINT { X = x, Y = y });
        return top != IntPtr.Zero && GetAncestor(top, 2) == hwnd;
    }

    [DllImport("user32.dll")] private static extern IntPtr WindowFromPoint(POINT p);
    [DllImport("user32.dll")] private static extern IntPtr GetAncestor(IntPtr h, uint flags);

    // The dedicated server is a console window; the game itself is an SDL window.
    private static bool IsConsoleWindow(IntPtr h)
    {
        var sb = new System.Text.StringBuilder(256);
        GetClassName(h, sb, sb.Capacity);
        var cls = sb.ToString();
        return cls == "ConsoleWindowClass" || cls.StartsWith("CASCADIA", StringComparison.Ordinal);
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetClassName(IntPtr h, System.Text.StringBuilder s, int n);
    [StructLayout(LayoutKind.Sequential)] private struct RECT { public int Left, Top, Right, Bottom; }
    [StructLayout(LayoutKind.Sequential)] private struct POINT { public int X, Y; }
    [DllImport("user32.dll")] private static extern bool GetClientRect(IntPtr h, out RECT r);
    [DllImport("user32.dll")] private static extern bool ClientToScreen(IntPtr h, ref POINT p);
    [DllImport("user32.dll")] private static extern bool SetForegroundWindow(IntPtr h);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr h, int cmd);
    [DllImport("user32.dll")] private static extern bool SetCursorPos(int x, int y);
    [DllImport("user32.dll")] private static extern void mouse_event(uint f, int dx, int dy, int d, UIntPtr extra);
}




