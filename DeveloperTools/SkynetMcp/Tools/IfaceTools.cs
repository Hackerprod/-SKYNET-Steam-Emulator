using System.ComponentModel;
using System.Text.Json;
using System.Text.RegularExpressions;
using ModelContextProtocol.Server;

namespace SKYNET.Mcp.Tools;

/// <summary>Steam interface tracer (skynet-gc iface ...), generic over any interface of the committed header-spec. `dir` is optional everywhere: omitted = newest capture.</summary>
[McpServerToolType]
public static class IfaceTools
{
    private static void Targets(List<string> a, string? interfaces, string? methods)
    {
        if (interfaces != null) { a.Add("--interfaces"); a.Add(interfaces); }
        if (methods != null) { a.Add("--methods"); a.Add(methods); }
    }

    private static List<string> Dir(string? dir) => dir == null ? new() : new() { "--dir", dir };

    [McpServerTool(Name = "iface_start"), Description(
        "Start tracing calls of Steam interfaces in dota2.exe (generic: any interface/version of InterfaceLayoutCheck/header-spec, e.g. SteamFriends017, ISteamUser:023, ISteamMatchmaking:9; default SteamFriends017 with 11 preset methods). mode=launch starts Dota against the real Steam client (Steam must be running and logged in); " +
        "mode=attach injects into a running dota2.exe (pid; for a game started by the launcher/emulator pass via_emulator=true, which needs the environment variable SKYNET_IFACE_TRACER = <path of skynet-iface-hook.exe> to have been set when the game was started; the launcher overwrites steam_api.ini, so the ini key is not reliable). " +
        "Launching Dota against the real Steam client with an injected DLL carries VAC risk; use it with an account you accept that risk for, and stop the capture if a VAC popup appears. " +
        "Methods whose signature the tracer cannot decode yet (floats, by-value structs) are reported as skip records, not patched. Returns {dir,pid,interfaces,status} once the tracer wrote its meta (status ok = patched), or the error text.")]
    public static async Task<string> IfaceStart(
        [Description("launch | attach")] string mode,
        [Description("attach: pid of dota2.exe")] int? pid = null,
        [Description("launch: extra Dota arguments")] string? args = null,
        [Description("attach (emulator): path of steam_api*.log; default is found automatically")] string? emu_log = null,
        [Description("attach: let the emulator DLL inside the game load the tracer (needs SKYNET_IFACE_TRACER set when the game started); use this for a game started by the launcher, a second EasyHook injection fails there")] bool via_emulator = false,
        [Description("comma list: spec name (SteamUser023) or interface:version (ISteamUser:23); default SteamFriends017")] string? interfaces = null,
        [Description("comma list of method names; default: the preset (no interfaces given) or every method with a supported signature")] string? methods = null)
    {
        var dir = Engine.NewDir(Engine.IfaceRoot);
        int gamePid;
        int injector = 0;
        if (mode == "attach")
        {
            if (pid == null) return "attach needs pid";
            var a = new List<string> { "iface", "attach", "--pid", pid.Value.ToString(), "--out", dir };
            if (emu_log != null) { a.Add("--emu-log"); a.Add(emu_log); }
            Targets(a, interfaces, methods);
            if (via_emulator) a.Add("--via-emulator");
            var (code, text) = await Engine.Run(Engine.GcExe, a, 90_000);
            if (code != 0) return $"injection failed (exit {code}); stop and report, do not work around: {Engine.Cap(text)}";
            gamePid = pid.Value;
        }
        else if (mode == "launch")
        {
            var a = new List<string> { "iface", "start", "--out", dir };
            if (args != null) { a.Add("--args"); a.Add(args); }
            Targets(a, interfaces, methods);
            var log = Path.Combine(dir, "start.log");
            var p = Engine.StartBackground(Engine.GcExe, a, log);
            injector = p.Id;
            gamePid = 0;
            for (var i = 0; i < 90 && gamePid == 0; i++)    // the hook exe prints {"dir":..,"pid":N} right after injection
            {
                await Task.Delay(1000);
                if (File.Exists(log)) foreach (var l in File.ReadAllLines(log)) { var m = Regex.Match(l, "\"pid\":(\\d+)"); if (l.StartsWith("{\"dir\"") && m.Success) gamePid = int.Parse(m.Groups[1].Value); }
                if (p.HasExited && gamePid == 0) return "launch failed: " + Engine.Cap(File.Exists(log) ? File.ReadAllText(log) : "no output");
            }
            if (gamePid == 0) return "launch: no pid after 90s; see " + log;
        }
        else return "mode must be launch or attach";

        Engine.WriteSession(dir, "iface", injector, gamePid);
        var status = await Engine.WaitMeta(dir, 60);
        return JsonSerializer.Serialize(new { dir, pid = gamePid, status = status ?? "no meta written yet (the game may not have logged in to Steam yet)" });
    }

    [McpServerTool(Name = "iface_stop"), Description("Stop the capture: restores the patched vtable slots and writes the final record. Returns stopped, records, dropped, faults.")]
    public static async Task<string> IfaceStop(string? dir = null, [Description("also close dota2.exe")] bool close_game = false)
    {
        var a = new List<string> { "iface", "stop" }; a.AddRange(Dir(dir));
        var r = await Engine.Gc(a, 60_000);
        if (close_game)
        {
            var d = dir ?? Engine.Newest(Engine.IfaceRoot);
            if (d != null && Engine.ReadSession(d) is { } s && s.GamePid != 0) Engine.CloseGame(s.GamePid);
        }
        return r;
    }

    [McpServerTool(Name = "iface_mark"), Description("Add a labelled marker to the capture (use before each UI action); timeline and query can slice between markers.")]
    public static Task<string> IfaceMark(string label, string? dir = null)
    {
        var a = new List<string> { "iface", "mark", label }; a.AddRange(Dir(dir));
        return Engine.Gc(a, 30_000);
    }

    [McpServerTool(Name = "iface_status"), Description("Capture state: provider (steam|emulator), status, patched/skipped methods, records, dropped, faults, alive, marks. Without dir: the 5 newest captures.")]
    public static Task<string> IfaceStatus(string? dir = null)
    {
        var a = new List<string> { "iface", "status" };
        if (dir == null) a.Add("--list"); else a.AddRange(Dir(dir));
        return Engine.Gc(a, 30_000);
    }

    [McpServerTool(Name = "iface_timeline"), Description(
        "Chronological calls as '#seq +t Method(args) = result [name] @caller xN' (Interface.Method when several are traced) with markers interleaved. Filters: iface (regex on the spec name), steamid (id64, [U:1:N] or account number), method (regex), contains (text), between_from/between_to (mark number, label or @last).")]
    public static Task<string> IfaceTimeline(string? dir = null, string? steamid = null, string? method = null, string? contains = null,
        string? between_from = null, string? between_to = null, int limit = 40, int offset = 0, [Description("regex on the interface spec name, e.g. SteamUser")] string? iface = null)
    {
        var a = new List<string> { "iface", "timeline" }; a.AddRange(Dir(dir));
        Filters(a, steamid, method, contains, between_from, between_to, iface);
        a.AddRange(new[] { "--limit", limit.ToString(), "--offset", offset.ToString() });
        return Engine.GcPaged(a, offset);
    }

    [McpServerTool(Name = "iface_query"), Description(
        "Grouped view of calls: distinct (args -> result) with count, seq/time range and callers. The compact answer to 'what did Steam return for X'.")]
    public static Task<string> IfaceQuery([Description("regex of method names")] string method, string? dir = null, string? steamid = null,
        string? between_from = null, string? between_to = null, int limit = 60, int offset = 0, [Description("regex on the interface spec name, e.g. SteamUser")] string? iface = null)
    {
        var a = new List<string> { "iface", "query", "--method", method }; a.AddRange(Dir(dir));
        Filters(a, steamid, null, null, between_from, between_to, iface);
        a.AddRange(new[] { "--limit", limit.ToString(), "--offset", offset.ToString() });
        return Engine.GcPaged(a, offset);
    }

    [McpServerTool(Name = "iface_diff"), Description(
        "Emulator gate (any traced interface): compares the traced calls with the lines the emulator logged while tracing (byte range of steam_api*.log; exact rules for the ISteamFriends v017 methods, value-containment for the others). 'result: PASS' means no mismatch, nothing only in trace or only in log; dropped records or an unchanged log make it INCONCLUSIVE.")]
    public static Task<string> IfaceDiff(string? dir = null, [Description("override of meta.emu_log")] string? emu_log = null)
    {
        var a = new List<string> { "iface", "diff" }; a.AddRange(Dir(dir));
        if (emu_log != null) { a.Add("--emu-log"); a.Add(emu_log); }
        return Engine.Gc(a, 60_000);
    }

    private static void Filters(List<string> a, string? steamid, string? method, string? contains, string? from, string? to, string? iface)
    {
        if (iface != null) { a.Add("--iface"); a.Add(iface); }
        if (steamid != null) { a.Add("--steamid"); a.Add(steamid); }
        if (method != null) { a.Add("--method"); a.Add(method); }
        if (contains != null) { a.Add("--contains"); a.Add(contains); }
        if (from != null || to != null) { a.Add("--between"); a.Add(from ?? "1"); a.Add(to ?? "@last"); }
    }
}
