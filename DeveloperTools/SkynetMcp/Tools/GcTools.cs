using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ModelContextProtocol.Server;

namespace SKYNET.Mcp.Tools;

/// <summary>The existing skynet-gc GC capture and analysis commands.</summary>
[McpServerToolType]
public static class GcTools
{
    [McpServerTool(Name = "gc_capture_start"), Description(
        "Start dota2.exe against the real Steam client (must be running and logged in) with the GC hook; records the decrypted GameCoordinator traffic to gc.jsonl. Returns {dir,pid}. " +
        "Launching Dota against the real Steam client with an injected DLL carries VAC risk; use it with an account you accept that risk for, and stop the capture if a VAC popup appears.")]
    public static async Task<string> GcCaptureStart([Description("extra Dota arguments")] string? args = null, [Description("output folder")] string? @out = null)
    {
        var dir = @out ?? Engine.NewDir(Engine.GcRoot);
        var a = new List<string> { "capture", "start", "--out", dir };
        if (args != null) { a.Add("--args"); a.Add(args); }
        var log = Path.Combine(dir, "capture.log");
        var p = Engine.StartBackground(Engine.GcExe, a, log);
        var gamePid = 0;
        for (var i = 0; i < 60 && gamePid == 0; i++)
        {
            await Task.Delay(1000);
            if (File.Exists(log)) foreach (var l in File.ReadAllLines(log)) { var m = Regex.Match(l, @"^dota2 pid\s*:\s*(\d+)"); if (m.Success) gamePid = int.Parse(m.Groups[1].Value); }
            if (p.HasExited && gamePid == 0) return "capture failed: " + Engine.Cap(File.Exists(log) ? File.ReadAllText(log) : "no output");
        }
        if (gamePid == 0) return "no dota2 pid after 60s; see " + log;
        Engine.WriteSession(dir, "gc", p.Id, gamePid);
        return JsonSerializer.Serialize(new { dir, pid = gamePid });
    }

    [McpServerTool(Name = "gc_capture_stop"), Description(
        "End a gc_capture_start capture and report the record count of gc.jsonl. The capture has no stop channel: with close_game=true (default) the injector process and dota2.exe are ended. " +
        "With close_game=false only the skynet-gc launcher process ends (not its children); dota2.exe and the hook exe keep running and the hook keeps appending to gc.jsonl until Dota exits.")]
    public static string GcCaptureStop(string? dir = null, bool close_game = true)
    {
        dir ??= Engine.Newest(Engine.GcRoot);
        if (dir == null) return "no capture folder found under " + Engine.GcRoot;
        var s = Engine.ReadSession(dir);
        if (s == null) return "no mcp-session.json in " + dir + " (the capture was not started by gc_capture_start)";
        try { using var inj = Process.GetProcessById(s.Value.Pid); inj.Kill(close_game); } catch { }   // the process tree includes the game: only kill it when asked to
        if (close_game) Engine.CloseGame(s.Value.GamePid);
        var gc = Path.Combine(dir, "gc.jsonl");
        var n = File.Exists(gc) ? File.ReadLines(gc).Count() : 0;
        return $"records={n} dir={dir}" + (close_game ? " (injector and game closed)" : " (launcher process ended; the game and the hook keep running and recording until Dota exits)");
    }

    [McpServerTool(Name = "gc_analyze"), Description("Decode a gc_capture into per-message JSON and flow reports (skynet-gc analyze); returns totals and the message table (top 30).")]
    public static async Task<string> GcAnalyze(string? dir = null, int? after = null, int? window = null, string? baseline = null)
    {
        dir ??= Engine.Newest(Engine.GcRoot);
        if (dir == null) return "no capture folder found under " + Engine.GcRoot;
        var a = new List<string> { "analyze", dir };
        if (after != null) { a.Add("--after"); a.Add(after.Value.ToString()); }
        if (window != null) { a.Add("--window"); a.Add(window.Value.ToString()); }
        if (baseline != null) { a.Add("--baseline"); a.Add(baseline); }
        var (code, text) = await Engine.Run(Engine.GcExe, a, 300_000);
        var summary = SummaryPath(dir);
        if (summary == null) return $"analyze exit {code}\n{Engine.Cap(text)}";
        using var doc = JsonDocument.Parse(File.ReadAllText(summary));
        var r = doc.RootElement;
        var sb = new StringBuilder($"files={r.GetProperty("totalFiles").GetInt32()} decoded={r.GetProperty("decodedPayloads").GetInt32()} undecoded={r.GetProperty("undecodedPayloads").GetInt32()} out={Path.GetDirectoryName(summary)}\n");
        foreach (var m in r.GetProperty("messages").EnumerateArray().OrderByDescending(m => m.GetProperty("Count").GetInt32()).Take(30))
            sb.AppendLine($"{m.GetProperty("Direction").GetString()} {m.GetProperty("GcMessageType").GetUInt32()} {m.GetProperty("GcMessageName").GetString()} x{m.GetProperty("Count").GetInt32()} errors={m.GetProperty("DecodeErrors").GetInt32()}");
        return Engine.Cap(sb.ToString());
    }

    [McpServerTool(Name = "gc_messages"), Description("List decoded GC messages from a gc_analyze result (Sequence Direction Name PayloadLength jobs). With sequence: the decoded JSON of that record, truncated to max_chars.")]
    public static string GcMessages(string? dir = null, [Description("regex on the message name")] string? name = null, string? direction = null,
        int? sequence = null, int limit = 40, int offset = 0, int max_chars = Engine.DefaultMaxChars)
    {
        dir ??= Engine.Newest(Engine.GcRoot);
        var summary = dir == null ? null : SummaryPath(dir);
        if (summary == null) return "no summary.json: run gc_analyze first";
        using var doc = JsonDocument.Parse(File.ReadAllText(summary));
        var records = doc.RootElement.GetProperty("records").EnumerateArray();
        if (sequence != null)
        {
            var rec = records.FirstOrDefault(x => x.GetProperty("Sequence").GetInt32() == sequence.Value);
            if (rec.ValueKind != JsonValueKind.Object) return "no record with sequence " + sequence;
            var file = Path.Combine(Path.GetDirectoryName(summary)!, rec.GetProperty("File").GetString()!);
            return File.Exists(file) ? Engine.Cap(File.ReadAllText(file), max_chars) : "decoded file missing: " + file;
        }
        var re = name != null ? new Regex(name, RegexOptions.IgnoreCase) : null;
        var rows = records.Where(x => (re == null || re.IsMatch(x.GetProperty("GcMessageName").GetString() ?? ""))
                                   && (direction == null || string.Equals(x.GetProperty("Direction").GetString(), direction, StringComparison.OrdinalIgnoreCase))).ToList();
        var sb = new StringBuilder();
        foreach (var x in rows.Skip(offset).Take(limit))
            sb.AppendLine($"{x.GetProperty("Sequence").GetInt32()} {x.GetProperty("Direction").GetString()} {x.GetProperty("GcMessageName").GetString()} len={x.GetProperty("PayloadLength").GetInt32()} job={x.GetProperty("SourceJobId")}->{x.GetProperty("TargetJobId")}");
        if (offset + limit < rows.Count) sb.AppendLine("next_offset=" + (offset + limit));
        return Engine.Cap(sb.ToString(), max_chars);
    }

    [McpServerTool(Name = "gc_decode"), Description("Decode one GC message body (protobuf, without GC header) to JSON.")]
    public static async Task<string> GcDecode([Description("GC message id")] uint type, [Description("body, base64")] string body_b64, [Description("client | server")] string side = "client")
    {
        var tmp = Path.Combine(Path.GetTempPath(), "skynet-gc-body-" + Guid.NewGuid().ToString("N") + ".bin");
        try
        {
            await File.WriteAllBytesAsync(tmp, Convert.FromBase64String(body_b64));
            return await Engine.Gc(new[] { "decode", type.ToString(), tmp, side }, 60_000);
        }
        finally { try { File.Delete(tmp); } catch { } }
    }

    // `analyze` writes <dir>\gc-json\summary.json by default; accept the capture folder or the output folder.
    private static string? SummaryPath(string dir)
    {
        foreach (var c in new[] { Path.Combine(dir, "summary.json"), Path.Combine(dir, "gc-json", "summary.json") })
            if (File.Exists(c)) return c;
        return null;
    }
}
