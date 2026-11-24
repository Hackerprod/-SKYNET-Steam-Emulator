using System.Diagnostics;
using EasyHook;

namespace SKYNET.Cli.Capture;

/// <summary>
/// capture start [--out dir] [--args "extra dota args"]
/// Starts dota2.exe against the REAL Steam client (must be running and logged in) with the GC hook
/// injected, then follows the JSON-lines log until the game exits (or Ctrl+C).
/// </summary>
internal static class CaptureCommand
{
    public static int Run(string[] args)
    {
        var outDir = Option(args, "--out") ?? Path.Combine(Path.GetTempPath(), "skynet-capture", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
        var extra = Option(args, "--args") ?? "";
        var win64 = Option(args, "--dota") ?? DotaLocator.Win64Dir();
        if (win64 == null) { Console.Error.WriteLine("dota2.exe not found; pass --dota <...\\game\\bin\\win64>"); return 2; }
        if (Process.GetProcessesByName("steam").Length == 0) { Console.Error.WriteLine("Steam is not running. Start and log in to Steam first."); return 2; }

        Directory.CreateDirectory(outDir);
        var log = Path.Combine(outDir, "gc.jsonl");
        var exe = Path.Combine(win64, "dota2.exe");
        var hook = typeof(CaptureCommand).Assembly.Location;

        // What Steam itself provides when it starts a game.
        Environment.SetEnvironmentVariable("SteamAppId", "570");
        Environment.SetEnvironmentVariable("SteamGameId", "570");
        var steam = DotaLocator.SteamPath();
        if (steam != null) Environment.SetEnvironmentVariable("SteamPath", steam);

        Console.WriteLine("capture dir : " + outDir);
        Console.WriteLine("launching   : " + exe);
        RemoteHooking.CreateAndInject(exe, ("-steam -console -novid " + extra).Trim(), 0, InjectionOptions.Default,
            hook, hook, out var pid, log);
        Console.WriteLine("dota2 pid   : " + pid);

        Process? proc = null;
        try { proc = Process.GetProcessById(pid); } catch { }
        long pos = 0;
        int count = 0;
        while (proc != null && !proc.HasExited)
        {
            pos = Tail(log, pos, ref count);
            Thread.Sleep(500);
        }
        Tail(log, pos, ref count);
        Console.WriteLine("game exited; " + count + " records in " + log);
        return 0;
    }

    private static long Tail(string path, long pos, ref int count)
    {
        if (!File.Exists(path)) return pos;
        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            fs.Seek(pos, SeekOrigin.Begin);
            using (var r = new StreamReader(fs))
            {
                string? line;
                while ((line = r.ReadLine()) != null)
                {
                    count++;
                    Console.WriteLine(Summarize(line));
                }
                return fs.Position;
            }
        }
    }

    private static string Summarize(string line)
    {
        var i = line.IndexOf(",\"b64\"", StringComparison.Ordinal);
        return i > 0 ? line.Substring(0, i) + "}" : line;
    }

    internal static string? Option(string[] args, string name)
    {
        for (var i = 0; i < args.Length - 1; i++)
            if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase)) return args[i + 1];
        return null;
    }
}


