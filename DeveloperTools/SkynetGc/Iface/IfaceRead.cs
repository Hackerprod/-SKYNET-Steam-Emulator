using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SKYNET.Gc.Iface;

internal sealed class IfaceCall
{
    public long Seq, Q, QLast;
    public int Tid, Rep = 1;
    public string I = "", M = "", R = "";
    public string[] A = Array.Empty<string>();
    public string? Rn, Af, C, O;
    public bool Self;
}

/// <summary>One traced interface as recorded in meta: spec name (e.g. SteamFriends017) and the methods that were patched.</summary>
internal sealed class IfaceInfo
{
    public string Name = "";
    public HashSet<string> Patched = new();
    public int Skipped;
    public string? Error;
}

internal sealed class IfaceMark
{
    public int N;
    public string Label = "";
    public long Q;
}

/// <summary>One tracer capture folder: iface.jsonl (payload), marks.jsonl (`iface mark`), STOP.</summary>
internal sealed class IfaceCapture
{
    public string Dir = "";
    public JsonElement Meta, Stop;           // last "meta" / "stop" record (a later meta replaces an earlier one)
    public bool HasMeta, HasStop;
    public List<IfaceCall> Calls = new();
    public List<IfaceMark> Marks = new();
    public List<(string I, string M, string Why)> Skips = new();
    public long Drops, Faults;

    private List<IfaceInfo>? _ifaces;
    public List<IfaceInfo> Ifaces => _ifaces ??= ParseIfaces();

    private List<IfaceInfo> ParseIfaces()
    {
        var list = new List<IfaceInfo>();
        if (HasMeta && Meta.TryGetProperty("ifaces", out var arr) && arr.ValueKind == JsonValueKind.Array)
            foreach (var e in arr.EnumerateArray())
                list.Add(new IfaceInfo
                {
                    Name = Str(e, "i") ?? "",
                    Patched = e.TryGetProperty("patched", out var p) ? p.EnumerateArray().Select(x => x.GetString() ?? "").ToHashSet() : new(),
                    Skipped = (int)(Long(e, "skipped") ?? 0),
                    Error = Str(e, "error"),
                });
        return list;
    }

    public long Q0 => HasMeta ? Long(Meta, "q0") ?? 0 : 0;
    public double Freq => HasMeta ? Long(Meta, "qfreq") ?? Stopwatch.Frequency : Stopwatch.Frequency;
    public double T(long q) => (q - Q0) / Freq;
    public string? MetaStr(string name) => HasMeta ? Str(Meta, name) : null;

    public static string? Str(JsonElement e, string name) =>
        e.ValueKind == JsonValueKind.Object && e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

    public static long? Long(JsonElement e, string name) =>
        e.ValueKind == JsonValueKind.Object && e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetInt64() : null;

    public static IEnumerable<string> ReadLines(string path)
    {
        if (!File.Exists(path)) yield break;
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        using var r = new StreamReader(fs, new UTF8Encoding(false));
        string? line;
        while ((line = r.ReadLine()) != null) if (line.Length > 0) yield return line;
    }

    public static IfaceCapture Load(string dir)
    {
        var c = new IfaceCapture { Dir = dir };
        foreach (var line in ReadLines(Path.Combine(dir, "iface.jsonl")))
        {
            try
            {
                using var doc = JsonDocument.Parse(line);
                var e = doc.RootElement;
                switch (Str(e, "k"))
                {
                    case "meta": c.Meta = e.Clone(); c.HasMeta = true; break;
                    case "stop": c.Stop = e.Clone(); c.HasStop = true; break;
                    case "drop": c.Drops += Long(e, "n") ?? 0; break;
                    case "fault": c.Faults++; break;
                    case "skip": c.Skips.Add((Str(e, "i") ?? "", Str(e, "m") ?? "", Str(e, "why") ?? "")); break;
                    case "call": c.Calls.Add(ParseCall(e)); break;
                }
            }
            catch (JsonException) { } // a line still being written
        }
        foreach (var line in ReadLines(Path.Combine(dir, "marks.jsonl")))
        {
            try
            {
                using var doc = JsonDocument.Parse(line);
                var e = doc.RootElement;
                c.Marks.Add(new IfaceMark { N = (int)(Long(e, "n") ?? 0), Label = Str(e, "label") ?? "", Q = Long(e, "q") ?? 0 });
            }
            catch (JsonException) { }
        }
        return c;
    }

    private static IfaceCall ParseCall(JsonElement e)
    {
        var q = Long(e, "q") ?? 0;
        return new IfaceCall
        {
            Seq = Long(e, "seq") ?? 0, Q = q, QLast = Long(e, "qlast") ?? q, Tid = (int)(Long(e, "tid") ?? 0), Rep = (int)(Long(e, "rep") ?? 1),
            I = Str(e, "i") ?? "", M = Str(e, "m") ?? "", R = Str(e, "r") ?? "", Rn = Str(e, "rn"), Af = Str(e, "af"), C = Str(e, "c"),
            O = e.TryGetProperty("o", out var o) ? o.GetRawText() : null,
            Self = e.TryGetProperty("self", out var s) && s.ValueKind == JsonValueKind.True,
            A = e.TryGetProperty("a", out var a) ? a.EnumerateArray().Select(x => x.GetString() ?? "").ToArray() : Array.Empty<string>(),
        };
    }

    /// <summary>A start label resolves to its first occurrence, an end label to its last; `@last` = highest mark number.</summary>
    public IfaceMark? ResolveMark(string token, bool isEnd)
    {
        if (Marks.Count == 0) return null;
        if (token == "@last") return Marks.OrderBy(m => m.N).Last();
        if (int.TryParse(token, out var n)) return Marks.FirstOrDefault(m => m.N == n);
        var hits = Marks.Where(m => string.Equals(m.Label, token, StringComparison.OrdinalIgnoreCase)).OrderBy(m => m.N).ToList();
        return hits.Count == 0 ? null : isEnd ? hits.Last() : hits.First();
    }

    public bool Alive()
    {
        var pid = Long(Meta, "pid");
        if (!HasMeta || pid == null) return false;
        try { using var p = Process.GetProcessById((int)pid.Value); return !p.HasExited && p.ProcessName.StartsWith("dota2", StringComparison.OrdinalIgnoreCase); }
        catch { return false; }
    }
}

internal sealed class IfaceFilter
{
    public string? Method, Contains, SteamId, Iface;
    public string[]? Between;

    private const ulong IndividualBase = 76561197960265728UL; // universe 1, type individual, instance 1

    /// <summary>Accepts a SteamID64, [U:1:N] or a plain account number.</summary>
    public static string? NormSteamId(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        s = s.Trim();
        var m = Regex.Match(s, @"^\[U:1:(\d+)\]$");
        if (m.Success) return (IndividualBase + ulong.Parse(m.Groups[1].Value)).ToString();
        if (!ulong.TryParse(s, out var v)) return null;
        return (v < (1UL << 32) ? IndividualBase + v : v).ToString();
    }

    public IEnumerable<IfaceCall> Apply(IfaceCapture cap, out long lo, out long hi)
    {
        lo = long.MinValue; hi = long.MaxValue;
        if (Between is { Length: 2 })
        {
            var a = cap.ResolveMark(Between[0], false) ?? throw new ArgumentException("mark not found: " + Between[0]);
            var b = cap.ResolveMark(Between[1], true) ?? throw new ArgumentException("mark not found: " + Between[1]);
            lo = a.Q; hi = b.Q;
        }
        var id = NormSteamId(SteamId);
        if (SteamId != null && id == null) throw new ArgumentException("bad --steamid: " + SteamId);
        var acct = id != null ? "[U:1:" + (ulong.Parse(id) & 0xFFFFFFFF) + "]" : null;
        var re = Method != null ? new Regex(Method, RegexOptions.IgnoreCase) : null;
        var ire = Iface != null ? new Regex(Iface, RegexOptions.IgnoreCase) : null;
        var list = new List<IfaceCall>();
        foreach (var c in cap.Calls)
        {
            if (c.Self || c.Q < lo || c.Q > hi) continue;
            if (re != null && !re.IsMatch(c.M)) continue;
            if (ire != null && !ire.IsMatch(c.I)) continue;
            if (id != null && !(c.A.Contains(id) || c.R == id || c.Rn == acct || (c.O?.Contains("\"" + id + "\"") ?? false))) continue;
            if (Contains != null && !AllText(c).Contains(Contains, StringComparison.OrdinalIgnoreCase)) continue;
            list.Add(c);
        }
        return list;
    }

    private static string AllText(IfaceCall c) => string.Join("\u0001", c.A) + "\u0001" + c.R + "\u0001" + c.Rn + "\u0001" + c.O + "\u0001" + c.Af;
}

internal static class IfaceRender
{
    private static readonly Regex Plain = new(@"^(-?\d+|0x[0-9A-Fa-f]+|true|false)$");

    private static string Val(string s) => Plain.IsMatch(s) ? s : "\"" + s + "\"";

    public static string Args(IfaceCall c) => string.Join(", ", c.A.Select(Val));

    public static string Result(IfaceCall c) => c.Rn != null ? c.R + " " + c.Rn : Val(c.R);

    private static string Extras(IfaceCall c) => (c.Af != null ? " flags=" + c.Af : "") + (c.O != null ? " o=" + c.O : "");

    // With several traced interfaces the method is shown as Interface.Method.
    private static string Name(IfaceCapture cap, IfaceCall c) => cap.Ifaces.Count > 1 ? c.I + "." + c.M : c.M;

    public static string Line(IfaceCapture cap, IfaceCall c) =>
        $"#{c.Seq} +{cap.T(c.Q):F3}s T{c.Tid} {Name(cap, c)}({Args(c)}) = {Result(c)}{Extras(c)}" +
        (c.C != null ? " @" + c.C : "") + (c.Rep > 1 ? " x" + c.Rep : "");

    public static string MarkLine(IfaceMark m) => $"--- mark {m.N} \"{m.Label}\" ---";

    public static IEnumerable<string> Timeline(IfaceCapture cap, IfaceFilter f, int limit, int offset, out int next, out List<object> rows)
    {
        var calls = f.Apply(cap, out var lo, out var hi).ToList();
        var items = new List<(long q, string text, object row)>();
        foreach (var c in calls)
            items.Add((c.Q, Line(cap, c), new { seq = c.Seq, t = Math.Round(cap.T(c.Q), 3), tid = c.Tid, i = c.I, m = c.M, a = c.A, r = c.R, rn = c.Rn, af = c.Af, o = c.O, c = c.C, rep = c.Rep }));
        if (calls.Count > 0 || f.Between != null)
        {
            var from = f.Between != null ? lo : calls.Min(x => x.Q);
            var to = f.Between != null ? hi : calls.Max(x => x.Q);
            foreach (var m in cap.Marks.Where(m => m.Q >= from && m.Q <= to))
                items.Add((m.Q, MarkLine(m), new { mark = m.N, label = m.Label, t = Math.Round(cap.T(m.Q), 3) }));
        }
        var page = items.OrderBy(i => i.q).Skip(offset).Take(limit).ToList();
        next = offset + page.Count < items.Count ? offset + page.Count : -1;
        rows = page.Select(p => p.row).ToList();
        return page.Select(p => p.text);
    }

    /// <summary>Grouped view: distinct (method, args, result) with count, seq/time range and callers.</summary>
    public static IEnumerable<string> Query(IfaceCapture cap, IfaceFilter f, int limit, int offset, out int next, out List<object> rows)
    {
        var groups = f.Apply(cap, out _, out _)
            .GroupBy(c => c.I + "\u0001" + c.M + "\u0001" + string.Join("\u0002", c.A) + "\u0001" + c.R + "\u0001" + c.Rn + "\u0001" + c.O)
            .Select(g =>
            {
                var first = g.First();
                return (first, count: g.Sum(x => x.Rep), min: g.Min(x => x.Seq), max: g.Max(x => x.Seq),
                        t0: cap.T(g.Min(x => x.Q)), t1: cap.T(g.Max(x => x.QLast)), callers: g.Select(x => x.C).Where(x => x != null).Distinct().Take(3).ToList());
            })
            .OrderBy(g => g.min).ToList();
        var page = groups.Skip(offset).Take(limit).ToList();
        next = offset + page.Count < groups.Count ? offset + page.Count : -1;
        rows = page.Select(g => (object)new { i = g.first.I, m = g.first.M, a = g.first.A, r = g.first.R, rn = g.first.Rn, o = g.first.O, count = g.count, seqFrom = g.min, seqTo = g.max, tFrom = Math.Round(g.t0, 3), tTo = Math.Round(g.t1, 3), callers = g.callers }).ToList();
        return page.Select(g =>
            $"{Name(cap, g.first)}({Args(g.first)}) -> {Result(g.first)}{Extras(g.first)}  x{g.count}  seq {g.min}..{g.max}  +{g.t0:F1}s..+{g.t1:F1}s" +
            (g.callers.Count > 0 ? "  @" + string.Join(",", g.callers) : ""));
    }
}
