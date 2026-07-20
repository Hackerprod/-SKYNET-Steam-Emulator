using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SKYNET.Client.Services;

internal static class GameWindowActivator
{
    public static void BringToFrontWhenReady(Process process)
    {
        _ = Task.Run(() => BringToFrontLoop(process));
    }

    private static void BringToFrontLoop(Process process)
    {
        var deadline = DateTime.UtcNow.AddSeconds(20);
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                if (process.HasExited)
                    return;

                process.Refresh();
                var hwnd = process.MainWindowHandle;
                if (hwnd == IntPtr.Zero)
                    hwnd = FindVisibleTopLevelWindow(process.Id);

                if (hwnd != IntPtr.Zero)
                {
                    ShowWindowAsync(hwnd, SW_SHOWNORMAL);
                    SetForegroundWindow(hwnd);
                    return;
                }
            }
            catch
            {
                return;
            }

            Thread.Sleep(250);
        }
    }

    private static IntPtr FindVisibleTopLevelWindow(int processId)
    {
        var found = IntPtr.Zero;
        EnumWindows((hwnd, _) =>
        {
            GetWindowThreadProcessId(hwnd, out var ownerPid);
            if (ownerPid == processId && IsWindowVisible(hwnd))
            {
                found = hwnd;
                return false;
            }

            return true;
        }, IntPtr.Zero);

        return found;
    }

    private const int SW_SHOWNORMAL = 1;

    private delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);
}
