using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace SKYNET.Gc.Iface;

/// <summary>
/// iface start|attach    run hook\skynet-iface-hook.exe (net472: EasyHook injection needs .NET Framework)
/// iface mark|stop|status|timeline|query|diff|selftest   net8 readers/writers of the capture folder
/// A capture folder (default %TEMP%\skynet-iface\yyyyMMdd-HHmmss) holds iface.jsonl (payload), marks.jsonl, STOP.
/// </summary>
internal static class IfaceCommand
{
    private const string Usage = @"skynet-gc iface
  start  [--out DIR] [--args ""...""] [--dota DIR] [--interfaces LIST] [--methods LIST]   launch Dota on the real Steam with the tracer (VAC risk, see README)
  attach --pid N [--out DIR] [--emu-log PATH] [--interfaces LIST] [--methods LIST]       inject into a running dota2.exe (emulator or real Steam)
    LIST: spec names or interface:version, e.g. SteamFriends017,ISteamUser:023 (any file of InterfaceLayoutCheck\header-spec).
    Default: SteamFriends017 with 11 preset methods. --methods default with --interfaces: every method the type tables support.
  mark   ""label"" [--dir DIR]                         timestamped marker
  stop   [--dir DIR]                                 restore the slots, finish the capture
  status [--dir DIR | --list] [--json]
  timeline [--dir DIR] [--iface RE] [--steamid ID] [--method RE] [--contains TEXT] [--between A B] [--limit 40] [--offset 0] [--json]
  query    [--dir DIR] --method RE [--iface RE] [--steamid ID] [--between A B] [--limit 60] [--offset 0] [--json]
  diff   [--dir DIR] [--emu-log PATH] [--json]       tracer records vs the emulator log (exit 0 = clean pass)
  selftest [--keep DIR]                              offline checks (decoders, diff rules, readers; also the hook exe's)
Omitting --dir uses the newest capture folder.";

    private sealed class Opts
    {
        public readonly List<string> Pos = new();
        private readonly Dictionary<string, string[]> _named = new(StringComparer.OrdinalIgnoreCase);

        public static Opts Parse(IEnumerable<string> args)
        {
            var o = new Opts();
            var a = args.ToList();
            for (var i = 0; i < a.Count; i++)
            {
                if (!a[i].StartsWith("--", StringComparison.Ordinal)) { o.Pos.Add(a[i]); continue; }
                var n = a[i] switch { "--json" or "--list" => 0, "--between" => 2, _ => 1 };
                if (a.Count - 1 - i < n) throw new ArgumentException("missing value for " + a[i]);
                o._named[a[i]] = a.Skip(i + 1).Take(n).ToArray();
                i += n;
            }
            return o;
        }

        public string? Get(string name) => _named.TryGetValue(name, out var v) && v.Length > 0 ? v[0] : null;
        public string[]? Get2(string name) => _named.TryGetValue(name, out var v) ? v : null;
        public bool Has(string name) => _named.ContainsKey(name);
        public int Int(string name, int def) => int.TryParse(Get(name), out var v) ? v : def;
    }

    public static string Root => Path.Combine(Path.GetTempPath(), "skynet-iface");

    public static int Run(string[] args)
    {
        var verb = args.Length > 0 ? args[0] : "";
        try
        {
            if (verb is "start" or "attach") return Proxy(args);
            if (verb == "selftest") return IfaceSelfTest.Run(Opts.Parse(args.Skip(1)).Get("--keep"));
            if (verb is not ("mark" or "stop" or "status" or "timeline" or "query" or "diff")) { Console.WriteLine(Usage); return verb == "" ? 1 : 2; }

            var o = Opts.Parse(args.Skip(1));
            if (verb == "status" && o.Has("--list")) return List(o.Has("--json"));
            var dir = ResolveDir(o.Get("--dir"));
            if (dir == null) { Console.Error.WriteLine("no capture folder found under " + Root + " (pass --dir)"); return 2; }
            switch (verb)
            {
                case "mark": return Mark(dir, o);
                case "stop": return Stop(dir);
                case "status": return Status(dir, o.Has("--json"));
                case "timeline": return Timeline(dir, o);
                case "query": return Query(dir, o);
                default: return IfaceDiff.Run(IfaceCapture.Load(dir), o.Get("--emu-log"), o.Has("--json"));
            }
        }
        catch (ArgumentException ex) { Console.Error.WriteLine(ex.Message); return 2; }
    }

    private static int Proxy(string[] args)
    {
        var exe = Path.Combine(AppContext.BaseDirectory, "hook", "skynet-iface-hook.exe");
        if (!File.Exists(exe)) { Console.Error.WriteLine("hook payload missing: " + exe); return 2; }
        var psi = new ProcessStartInfo(exe) { UseShellExecute = false };
        foreach (var a in args) psi.ArgumentList.Add(a);
        using var p = Process.Start(psi)!;
        p.WaitForExit();
        return p.ExitCode;
    }

    public static string? ResolveDir(string? dir)
    {
        if (dir != null) return Directory.Exists(dir) ? Path.GetFullPath(dir) : throw new ArgumentException("capture folder not found: " + dir);
        return Newest(1).FirstOrDefault();
    }

    private static IEnumerable<string> Newest(int n) =>
        Directory.Exists(Root)
            ? new DirectoryInfo(Root).GetDirectories().OrderByDescending(d => d.CreationTimeUtc).Take(n).Select(d => d.FullName)
            : Enumerable.Empty<string>();

    private static int Mark(string dir, Opts o)
    {
        if (o.Pos.Count == 0) { Console.Error.WriteLine("mark needs a label"); return 2; }
        var label = string.Join(" ", o.Pos);
        var marks = Path.Combine(dir, "marks.jsonl");
        var n = IfaceCapture.ReadLines(marks).Count() + 1;
        var q = Stopwatch.GetTimestamp();
        File.AppendAllText(marks, "{\"k\":\"mark\",\"n\":" + n + ",\"label\":" + JsonSerializer.Serialize(label) + ",\"q\":" + q + "}\n", new UTF8Encoding(false));
        var cap = IfaceCapture.Load(dir);
        Console.WriteLine("mark n=" + n + (cap.HasMeta ? $" @+{cap.T(q):F1}s" : ""));
        return 0;
    }

    private static int Stop(string dir)
    {
        File.WriteAllText(Path.Combine(dir, "STOP"), "");
        for (var i = 0; i < 40; i++)    // the payload polls every 250 ms, then drains and waits ~1 s
        {
            var cap = IfaceCapture.Load(dir);
            if (cap.HasStop)
            {
                Console.WriteLine($"stopped records={IfaceCapture.Long(cap.Stop, "records")} dropped={IfaceCapture.Long(cap.Stop, "dropped")} faults={IfaceCapture.Long(cap.Stop, "faults")}");
                return 0;
            }
            if (!cap.HasMeta || !cap.Alive()) break;
            Thread.Sleep(250);
        }
        Console.WriteLine("stop requested; no stop record (the target is not running or the tracer never attached)");
        return 1;
    }

    private static int List(bool json)
    {
        var rows = Newest(5).Select(d =>
        {
            var c = IfaceCapture.Load(d);
            return new { dir = d, provider = c.MetaStr("provider"), status = c.MetaStr("status"), records = c.Calls.Count(x => !x.Self), stopped = c.HasStop, alive = c.Alive() };
        }).ToList();
        if (json) Console.WriteLine(JsonSerializer.Serialize(rows));
        else foreach (var r in rows) Console.WriteLine($"{r.dir}  provider={r.provider} status={r.status} records={r.records} stopped={r.stopped} alive={r.alive}");
        if (rows.Count == 0) Console.WriteLine("no captures under " + Root);
        return 0;
    }

    private static int Status(string dir, bool json)
    {
        var c = IfaceCapture.Load(dir);
        var info = new
        {
            dir, provider = c.MetaStr("provider"), status = c.MetaStr("status") ?? "(no meta yet)", 
            ifaces = c.Ifaces.Select(i => new { i.Name, patched = i.Patched.OrderBy(x => x).ToList(), i.Skipped, i.Error }).ToList(),
            skipReasons = c.Skips.GroupBy(s => s.Why).Select(g => new { why = g.Key, count = g.Count() }).OrderByDescending(g => g.count).Take(5).ToList(), records = c.Calls.Count(x => !x.Self), dropped = c.Drops, faults = c.Faults,
            alive = c.Alive(), stopped = c.HasStop, emuLog = c.MetaStr("emu_log"), emuLogMethod = c.MetaStr("emu_log_method"),
            emuLogSize = c.HasMeta ? IfaceCapture.Long(c.Meta, "emu_log_size") : null,
            marks = c.Marks.Select(m => new { m.N, m.Label, t = Math.Round(c.T(m.Q), 1) }).ToList(),
        };
        if (json) { Console.WriteLine(JsonSerializer.Serialize(info)); return 0; }
        Console.WriteLine($"dir={info.dir}");
        Console.WriteLine($"provider={info.provider} status={info.status} alive={info.alive} stopped={info.stopped}");
        foreach (var i in info.ifaces)
            Console.WriteLine($"{i.Name}: patched={i.patched.Count}{(i.patched.Count <= 15 ? " [" + string.Join(", ", i.patched) + "]" : "")} skipped={i.Skipped}{(i.Error != null ? " error=" + i.Error : "")}");
        foreach (var s in info.skipReasons) Console.WriteLine($"  skip x{s.count}: {s.why}");
        Console.WriteLine($"records={info.records} dropped={info.dropped} faults={info.faults}");
        if (info.emuLog != null) Console.WriteLine($"emu_log={info.emuLog} ({info.emuLogMethod}) size_at_patch={info.emuLogSize}");
        Console.WriteLine("marks=" + info.marks.Count + (info.marks.Count > 0 ? ": " + string.Join("; ", info.marks.Select(m => $"{m.N} \"{m.Label}\" @+{m.t}s")) : ""));
        return 0;
    }

    private static IfaceFilter Filter(Opts o) => new()
    {
        Iface = o.Get("--iface"), Method = o.Get("--method"), Contains = o.Get("--contains"), SteamId = o.Get("--steamid"), Between = o.Get2("--between"),
    };

    private static int Timeline(string dir, Opts o)
    {
        var cap = IfaceCapture.Load(dir);
        var lines = IfaceRender.Timeline(cap, Filter(o), o.Int("--limit", 40), o.Int("--offset", 0), out var next, out var rows).ToList();
        if (o.Has("--json")) Console.WriteLine(JsonSerializer.Serialize(new { rows, nextOffset = next < 0 ? (int?)null : next }));
        else
        {
            foreach (var l in lines) Console.WriteLine(l);
            if (lines.Count == 0) Console.WriteLine("(no records)");
            if (next >= 0) Console.WriteLine("next_offset=" + next);
        }
        return 0;
    }

    private static int Query(string dir, Opts o)
    {
        if (o.Get("--method") == null) { Console.Error.WriteLine("query needs --method RE"); return 2; }
        var cap = IfaceCapture.Load(dir);
        var lines = IfaceRender.Query(cap, Filter(o), o.Int("--limit", 60), o.Int("--offset", 0), out var next, out var rows).ToList();
        if (o.Has("--json")) Console.WriteLine(JsonSerializer.Serialize(new { rows, nextOffset = next < 0 ? (int?)null : next }));
        else
        {
            foreach (var l in lines) Console.WriteLine(l);
            if (lines.Count == 0) Console.WriteLine("(no records)");
            if (next >= 0) Console.WriteLine("next_offset=" + next);
        }
        return 0;
    }
}
