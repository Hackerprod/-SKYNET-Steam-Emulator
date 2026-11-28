using System.Diagnostics;
using EasyHook;
using SKYNET.Gc;

namespace SKYNET.Iface;

/// <summary>
/// Entry point of skynet-iface-hook.exe (net472; EasyHook needs .NET Framework to inject, like skynet-gc-hook.exe).
///   start  [--out DIR] [--args "..."] [--dota DIR] [--interfaces LIST] [--methods LIST]   launch Dota on the real Steam, tracer injected
///   attach --pid N [--out DIR] [--emu-log PATH] [--interfaces LIST] [--methods LIST] [--via-emulator]
///                                                                                          inject into a running dota2.exe (real Steam, or a game not injected by the launcher);
///                                                                                          --via-emulator: the emulator DLL inside the game loads the payload itself (environment variable SKYNET_IFACE_TRACER, or ini [Debug] IfaceTracer)
///   selftest [--spec-dir DIR]                                                              offline checks of the decoders and the interposers
/// LIST for --interfaces: spec file names or interface:version, e.g. SteamFriends017,ISteamUser:023 (any file of
/// InterfaceLayoutCheck\header-spec). Default: SteamFriends017 with its 11 preset methods. --methods: names (default when
/// --interfaces is given: every method whose signature the type tables support; the others get a skip record).
/// Both inject verbs print {"dir":...,"pid":N,...} first. Normally run through `skynet-gc iface ...`.
/// </summary>
internal static class IfaceHookMain
{
    private static int Main(string[] args)
    {
        var rest = args.Skip(1).ToArray();
        switch (args.Length > 0 ? args[0] : "")
        {
            case "start": return Start(rest);
            case "attach": return Attach(rest);
            case "selftest": return SelfTest.Run(rest);
        }
        Console.Error.WriteLine("skynet-iface-hook.exe start|attach|selftest ...   (use: skynet-gc iface ...)");
        return 2;
    }

    private static string? Opt(string[] a, string name)
    {
        for (var i = 0; i < a.Length - 1; i++)
            if (string.Equals(a[i], name, StringComparison.OrdinalIgnoreCase)) return a[i + 1];
        return null;
    }

    private static string SpecDir => Path.Combine(AppContext.BaseDirectory, "meta");

    private static string OutDir(string[] a) =>
        Path.GetFullPath(Opt(a, "--out") ?? Path.Combine(Path.GetTempPath(), "skynet-iface", DateTime.Now.ToString("yyyyMMdd-HHmmss")));

    // Resolves --interfaces/--methods against the committed specs; prints why and returns false on a bad name.
    internal static bool Targets(string specDir, string? interfaces, string? methodsOpt, out string ifaces, out string methods, out string json)
    {
        ifaces = methods = json = "";
        var names = new List<string>();
        var defaulted = interfaces == null;
        if (defaulted)
        {
            names.Add(Spec.DefaultIface);
            Console.WriteLine("no --interfaces given: defaulting to " + Spec.DefaultIface + " (ISteamFriends v017) with its " + Spec.DefaultMethods.Length + " preset methods");
        }
        else
        {
            foreach (var tok in interfaces!.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var r = Spec.Resolve(specDir, tok, out var err);
                if (r == null) { Console.Error.WriteLine(err); return false; }
                names.Add(r);
            }
        }
        var list = methodsOpt == null || methodsOpt == "all"
            ? (defaulted && methodsOpt == null ? Spec.DefaultMethods : new string[0])
            : methodsOpt.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var m in list)
            if (!names.Any(n => Spec.Load(Path.Combine(specDir, n + ".txt")).Any(r => r.Name == m)))
            {
                Console.Error.WriteLine("method " + m + " is not in " + string.Join(", ", names));
                return false;
            }
        ifaces = string.Join(",", names);
        methods = string.Join(",", list);
        json = ",\"interfaces\":" + Json.Arr(names) + ",\"methods\":" + Json.Str(list.Length == 0 ? "all supported" : methods) + ",\"defaulted\":" + (defaulted ? "true" : "false");
        return true;
    }

    private static int Start(string[] a)
    {
        var outDir = OutDir(a);
        var win64 = Opt(a, "--dota") ?? DotaLocator.Win64Dir();
        if (win64 == null) { Console.Error.WriteLine("dota2.exe not found; pass --dota <...\\game\\bin\\win64>"); return 2; }
        if (Process.GetProcessesByName("steam").Length == 0) { Console.Error.WriteLine("Steam is not running. Start and log in to Steam first."); return 2; }
        if (!Targets(SpecDir, Opt(a, "--interfaces"), Opt(a, "--methods"), out var ifaces, out var methods, out var tj)) return 2;

        Directory.CreateDirectory(outDir);
        File.Delete(Path.Combine(outDir, "STOP"));
        var exe = Path.Combine(win64, "dota2.exe");
        var hook = typeof(IfaceHookMain).Assembly.Location;

        // What Steam itself provides when it starts a game (same as `capture start`).
        Environment.SetEnvironmentVariable("SteamAppId", "570");
        Environment.SetEnvironmentVariable("SteamGameId", "570");
        var steam = DotaLocator.SteamPath();
        if (steam != null) Environment.SetEnvironmentVariable("SteamPath", steam);

        Console.WriteLine("Launching Dota against the real Steam client with an injected DLL carries VAC risk; use it with an account you accept that risk for, and stop the capture if a VAC popup appears.");
        RemoteHooking.CreateAndInject(exe, ("-steam -console -novid " + (Opt(a, "--args") ?? "")).Trim(), 0, InjectionOptions.Default,
            hook, hook, out var pid, outDir, "launch", "", SpecDir, ifaces, methods);
        Console.WriteLine("{\"dir\":" + Json.Str(outDir) + ",\"pid\":" + pid + tj + "}");

        try { Process.GetProcessById(pid).WaitForExit(); } catch { }
        Console.WriteLine("game exited");
        return 0;
    }

    // A second EasyHook injection into a game the launcher already injected fails (Code 15). Instead the emulator DLL in the
    // game (steam_api, ini [Debug] IfaceTracer) polls this request file and runs the payload in-process. Written atomically
    // (temp + rename) so the poller never reads a half-written file.
    internal static string RequestFile => Path.Combine(Path.GetTempPath(), "skynet-iface", "attach-request.json");

    internal static void WriteRequest(string file, int pid, string outDir, string ifaces, string methods, string emuLog)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        var tmp = file + "." + Guid.NewGuid().ToString("N") + ".tmp";
        File.WriteAllText(tmp, "{\"dir\":" + Json.Str(outDir) + ",\"interfaces\":" + Json.Str(ifaces) + ",\"methods\":" + Json.Str(methods) +
                               ",\"emuLog\":" + Json.Str(emuLog) + ",\"pid\":" + pid + "}");
        if (File.Exists(file)) File.Delete(file);
        File.Move(tmp, file);
    }

    internal static string? LastStatus(string iface)
    {
        if (!File.Exists(iface)) return null;
        string? status = null;
        try
        {
            using (var fs = new FileStream(iface, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (var r = new StreamReader(fs))
            {
                string? line;
                while ((line = r.ReadLine()) != null)
                {
                    if (!line.StartsWith("{\"k\":\"meta\"", StringComparison.Ordinal)) continue;
                    var m = System.Text.RegularExpressions.Regex.Match(line, "\"status\":\"([^\"]*)\"");
                    if (m.Success) status = m.Groups[1].Value;
                }
            }
        }
        catch (IOException) { }
        return status;
    }

    internal static int ViaEmulator(string file, int tries, int pid, string outDir, string ifaces, string methods, string emuLog, string targetsJson)
    {
        WriteRequest(file, pid, outDir, ifaces, methods, emuLog);
        string? status = null;
        for (var i = 0; i < tries && (status == null || status == "waiting"); i++)   // ~20 s
        {
            Thread.Sleep(500);
            status = LastStatus(Path.Combine(outDir, "iface.jsonl"));
        }
        if (status == null)
        {
            try { File.Delete(file); } catch { }   // a stale request must not be picked up later
            Console.Error.WriteLine("emulator did not pick up the request (was SKYNET_IFACE_TRACER set when the game started, and is Dota running with the new DLL?)");
            return 2;
        }
        Console.WriteLine("{\"dir\":" + Json.Str(outDir) + ",\"pid\":" + pid + targetsJson + ",\"status\":" + Json.Str(status) + "}");
        return status == "ok" || status == "waiting" ? 0 : 1;
    }

    private static int Attach(string[] a)
    {
        if (!int.TryParse(Opt(a, "--pid"), out var pid)) { Console.Error.WriteLine("attach needs --pid N"); return 2; }
        var outDir = OutDir(a);
        if (!Targets(SpecDir, Opt(a, "--interfaces"), Opt(a, "--methods"), out var ifaces, out var methods, out var tj)) return 2;
        Directory.CreateDirectory(outDir);
        File.Delete(Path.Combine(outDir, "STOP"));
        if (a.Contains("--via-emulator")) return ViaEmulator(RequestFile, 40, pid, outDir, ifaces, methods, Opt(a, "--emu-log") ?? "", tj);
        var hook = typeof(IfaceHookMain).Assembly.Location;
        try
        {
            RemoteHooking.Inject(pid, InjectionOptions.Default, hook, hook, outDir, "attach", Opt(a, "--emu-log") ?? "", SpecDir, ifaces, methods);
        }
        catch (Exception ex)
        {
            // Stop-and-report: no fallback is built (a failed late injection is a finding, not something to work around).
            Console.Error.WriteLine("injection failed: " + ex.Message);
            return 2;
        }
        Console.WriteLine("{\"dir\":" + Json.Str(outDir) + ",\"pid\":" + pid + tj + "}");
        return 0;
    }
}
