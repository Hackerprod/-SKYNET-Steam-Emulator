using System.Diagnostics;

namespace SKYNET.Cli.Capture;

/// <summary>capture start ...: runs the net472 hook exe (EasyHook cannot inject from .NET 8) and relays its output.</summary>
internal static class CaptureProxy
{
    public static int Run(string[] args)
    {
        var exe = Path.Combine(AppContext.BaseDirectory, "hook", "skynet-hook.exe");
        if (!File.Exists(exe)) { Console.Error.WriteLine("hook payload missing: " + exe); return 2; }
        var psi = new ProcessStartInfo(exe) { UseShellExecute = false };
        psi.ArgumentList.Add("start");
        foreach (var a in args) psi.ArgumentList.Add(a);
        using var p = Process.Start(psi)!;
        p.WaitForExit();
        return p.ExitCode;
    }
}
