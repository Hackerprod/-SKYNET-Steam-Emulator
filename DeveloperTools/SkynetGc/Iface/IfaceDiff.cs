using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SKYNET.Gc.Iface;

/// <summary>
/// iface diff: compares the tracer's records with the lines the emulator logged while the tracer was active
/// (byte range [emu_log_size at patch, emu_log_size at stop) of steam_api*.log). Works for any traced interface:
/// the emulator's lines are "&lt;Interface&gt;: &lt;Method&gt; ..." and only methods that were patched are compared.
/// Default rule (generic): every value the trace recorded (non-pointer arguments, result) must appear among the tokens
/// of some log line of that method, and every logged line must be explained by some trace record. Methods listed in
/// Rules get an exact structured comparison instead (the ISteamFriends v017 methods are the first rows).
/// All comparisons are over sets of distinct tuples, because the emulator drops consecutive identical log lines.
/// The first and last Margin lines of the window, the Margin lines before it and the Margin after it only excuse
/// differences: a call can be logged just before the tracer records it (or the reverse).
/// </summary>
internal static class IfaceDiff
{
    private const int Margin = 10;

    internal sealed record Tup(string Key, string Result);   // exact rule item: Key = iface.method(args)

    internal sealed class Rule
    {
        public Regex Log = null!;                                          // over the text after "<Interface>: "
        public Func<Match, (string[] Args, string Result)> FromLog = null!;
        public Func<IfaceCall, (string[] Args, string Result)> FromCall = null!;
    }

    internal sealed class LogItem
    {
        public string Iface = "", Method = "", Text = "";
        public Tup? Exact;
        public HashSet<string>? Tokens;
    }

    private static string Num(string s) =>
        s.StartsWith("0x", StringComparison.OrdinalIgnoreCase) && long.TryParse(s.AsSpan(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var h)
            ? h.ToString(CultureInfo.InvariantCulture) : s.Trim();

    private static (string[], string) X(string[] a, string r) => (a, r);
    private static string A(IfaceCall c, int i) => i < c.A.Length ? c.A[i] : "";

    // Exact rules, by interface spec name then method. The log formats are those of steam_api\Steamworks\Implementation\SteamFriends.cs.
    internal static readonly Dictionary<string, Dictionary<string, Rule>> Rules = new()
    {
        ["SteamFriends017"] = new()
        {
            ["GetPersonaState"] = new Rule { Log = new(@"^GetPersonaState\b"), FromLog = m => X(new string[0], ""), FromCall = c => X(new string[0], "") },   // log prints a constant
            ["GetFriendCount"] = new Rule { Log = new(@"^GetFriendCount \(FriendFlags = (0x[0-9A-Fa-f]+|\d+)\) = (-?\d+)$"), FromLog = m => X(new[] { Num(m.Groups[1].Value) }, m.Groups[2].Value), FromCall = c => X(new[] { Num(A(c, 0)) }, c.R) },
            ["GetFriendByIndex"] = new Rule
            {
                Log = new(@"^GetFriendByIndex \(Index = (-?\d+), FriendFlags = (0x[0-9A-Fa-f]+|\d+)\) = (invalid|\[U:\d+:\d+\] \((\d+)\))$"),
                FromLog = m => X(new[] { m.Groups[1].Value, Num(m.Groups[2].Value) }, m.Groups[3].Value == "invalid" ? "0" : m.Groups[4].Value),
                FromCall = c => X(new[] { A(c, 0), Num(A(c, 1)) }, c.R),
            },
            // The first value printed is what the game receives; "(real X)" is the emulator's internal state.
            ["GetFriendRelationship"] = new Rule { Log = new(@"^GetFriendRelationship \(SteamID = (\d+)\) = (\S+?)(?: \(real \S+\))?$"), FromLog = m => X(new[] { m.Groups[1].Value }, m.Groups[2].Value), FromCall = c => X(new[] { A(c, 0) }, c.Rn ?? c.R) },
            ["GetFriendPersonaState"] = new Rule { Log = new(@"^GetFriendPersonaState (\d+) = (\S+)$"), FromLog = m => X(new[] { m.Groups[1].Value }, m.Groups[2].Value), FromCall = c => X(new[] { A(c, 0) }, c.Rn ?? c.R) },
            ["GetFriendPersonaName"] = new Rule { Log = new(@"^GetFriendPersonaName \(SteamID = \[U:\d+:\d+\] \((\d+)\)\) = (.*)$"), FromLog = m => X(new[] { m.Groups[1].Value }, m.Groups[2].Value), FromCall = c => X(new[] { A(c, 0) }, c.R) },
            // Out struct ignored.
            ["GetFriendGamePlayed"] = new Rule { Log = new(@"^GetFriendGamePlayed \(SteamID = (\d+)\) = (True|False)$"), FromLog = m => X(new[] { m.Groups[1].Value }, m.Groups[2].Value.ToLowerInvariant()), FromCall = c => X(new[] { A(c, 0) }, c.R) },
            ["GetFriendRichPresence"] = new Rule { Log = new(@"^GetFriendRichPresence \(SteamID = (\d+), Key = (.*?)\) = ?(.*)$"), FromLog = m => X(new[] { m.Groups[1].Value, m.Groups[2].Value }, m.Groups[3].Value), FromCall = c => X(new[] { A(c, 0), A(c, 1) }, c.R) },
            // The log line has no flags or result for HasFriend: presence of (method, SteamID) only.
            ["HasFriend"] = new Rule { Log = new(@"^HasFriend (\d+)$"), FromLog = m => X(new[] { m.Groups[1].Value }, ""), FromCall = c => X(new[] { A(c, 0) }, "") },
            // The dialog is logged in a field named GroupID.
            ["ActivateGameOverlayToUser"] = new Rule { Log = new(@"^ActivateGameOverlayToUser \(GroupID = (.*), SteamID = \[U:\d+:\d+\] \((\d+)\)\)$"), FromLog = m => X(new[] { m.Groups[1].Value, m.Groups[2].Value }, ""), FromCall = c => X(new[] { A(c, 0), A(c, 1) }, "") },
            // The result is the composite "queued:.. available:..": arguments only.
            ["RequestUserInformation"] = new Rule { Log = new(@"^RequestUserInformation \(SteamID = (\d+), RequireNameOnly = (True|False)\) = "), FromLog = m => X(new[] { m.Groups[1].Value, m.Groups[2].Value.ToLowerInvariant() }, ""), FromCall = c => X(new[] { A(c, 0), A(c, 1) }, "") },
        },
    };

    private static readonly Regex TokenRx = new(@"0x[0-9A-Fa-f]+|\d+|[A-Za-z_][A-Za-z0-9_]*", RegexOptions.Compiled);

    /// <summary>Numbers (hex normalised to decimal) and identifiers of a text, lower-cased: the format-independent atoms.</summary>
    internal static HashSet<string> Tokens(string? s)
    {
        var set = new HashSet<string>();
        if (string.IsNullOrEmpty(s)) return set;
        foreach (Match m in TokenRx.Matches(s))
            set.Add(m.Value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Num(m.Value) : m.Value.ToLowerInvariant());
        return set;
    }

    private sealed class TraceItem
    {
        public string Iface = "", Method = "", Text = "";
        public Tup? Exact;
        public HashSet<string> ArgAtoms = new();
        public List<HashSet<string>> ResultAlts = new();   // the numeric value or its name: the log may print either
    }

    private static TraceItem FromCall(IfaceCall c)
    {
        var t = new TraceItem { Iface = c.I, Method = c.M, Text = c.I + "." + c.M + "(" + string.Join(", ", c.A) + ") = " + c.R + (c.Rn != null ? " " + c.Rn : "") };
        if (Rules.TryGetValue(c.I, out var rs) && rs.TryGetValue(c.M, out var rule))
        {
            var (args, res) = rule.FromCall(c);
            t.Exact = new Tup(c.I + "." + c.M + "(" + string.Join(",", args) + ")", res);
            return t;
        }
        foreach (var a in c.A)
            if (!(a.StartsWith("0x", StringComparison.OrdinalIgnoreCase) && a.Length >= 10)) t.ArgAtoms.UnionWith(Tokens(a));   // pointers never appear in a log line
        foreach (var r in new[] { c.R, c.Rn })
            if (r != null && Tokens(r) is { Count: > 0 } alt) t.ResultAlts.Add(alt);
        return t;
    }

    /// <summary>One emulator log line as a comparable item, or null if it is not a line of a traced method.</summary>
    internal static LogItem? ParseLine(string line, IReadOnlyList<IfaceInfo> ifaces)
    {
        var t = line.TrimEnd('\r').TrimStart();
        var colon = t.IndexOf(": ", StringComparison.Ordinal);
        if (colon <= 0) return null;
        var info = ifaces.FirstOrDefault(i => string.Equals(i.Name, t.Substring(0, colon), StringComparison.OrdinalIgnoreCase));
        if (info == null) return null;
        var rest = t.Substring(colon + 2);
        var m = Regex.Match(rest, @"^(\w+)");
        if (!m.Success || !info.Patched.Contains(m.Value)) return null;
        var item = new LogItem { Iface = info.Name, Method = m.Value, Text = rest };
        if (Rules.TryGetValue(info.Name, out var rs) && rs.TryGetValue(m.Value, out var rule) && rule.Log.Match(rest) is { Success: true } rm)
        {
            var (args, res) = rule.FromLog(rm);
            item.Exact = new Tup(info.Name + "." + m.Value + "(" + string.Join(",", args) + ")", res);
        }
        else item.Tokens = Tokens(rest.Substring(m.Length));
        return item;
    }

    // Complete lines of [from, to); a partial first line (range not starting at a line start) and a partial last line are dropped.
    private static List<string> LinesOf(string path, long from, long to, bool dropPartialFirst)
    {
        if (to <= from) return new List<string>();
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        fs.Seek(from, SeekOrigin.Begin);
        var buf = new byte[Math.Max(0, Math.Min(to, fs.Length) - from)];
        var n = 0;
        while (n < buf.Length) { var r = fs.Read(buf, n, buf.Length - n); if (r <= 0) break; n += r; }
        var lines = Encoding.UTF8.GetString(buf, 0, n).Split('\n').ToList();
        lines.RemoveAt(lines.Count - 1);                       // "" after the final newline, or a partial last line
        if (dropPartialFirst && lines.Count > 0) lines.RemoveAt(0);
        return lines;
    }

    public static int Run(IfaceCapture cap, string? emuLogOverride, bool json)
    {
        var emuLog = emuLogOverride ?? cap.MetaStr("emu_log");
        if (string.IsNullOrEmpty(emuLog) || !File.Exists(emuLog))
        {
            Console.Error.WriteLine("diff needs the emulator log: " + (emuLog ?? "meta has no emu_log (real Steam capture, or the log was not found)") + "; pass --emu-log PATH");
            return 2;
        }
        var start = IfaceCapture.Long(cap.Meta, "emu_log_size");
        if (start == null) { Console.Error.WriteLine("meta has no emu_log_size (tracer not attached yet or never patched)"); return 2; }
        var fileLen = new FileInfo(emuLog).Length;
        var end = cap.HasStop ? IfaceCapture.Long(cap.Stop, "emu_log_size") ?? fileLen : fileLen;

        var reasons = new List<string>();
        if (cap.Drops > 0) reasons.Add($"{cap.Drops} records dropped (queue full)");
        if (cap.MetaStr("status") == "emu-log-unchanged") reasons.Add("emulator log did not grow while calls were traced (file logging disabled or wrong log?)");
        if (cap.MetaStr("status") is { } st && st != "ok" && st != "emu-log-unchanged") reasons.Add("tracer status " + st);

        var window = LinesOf(emuLog, start.Value, end, !IsLineStart(emuLog, start.Value));
        var before = LinesOf(emuLog, Math.Max(0, start.Value - 65536), start.Value, start.Value > 65536);
        var after = LinesOf(emuLog, end, Math.Min(fileLen, end + 65536), !IsLineStart(emuLog, end));
        var core = window.Count > 2 * Margin ? window.Skip(Margin).Take(window.Count - 2 * Margin).ToList() : new List<string>();
        if (core.Count == 0) reasons.Add($"window has only {window.Count} lines (need more than {2 * Margin}; extend the scenario)");

        List<LogItem> Items(IEnumerable<string> ls) => ls.Select(l => ParseLine(l, cap.Ifaces)).Where(x => x != null).Select(x => x!).ToList();
        var coreItems = Items(core);
        var marginItems = Items(before.TakeLast(Margin)).Concat(Items(after.Take(Margin))).Concat(Items(window.Take(Margin))).Concat(Items(window.TakeLast(Margin))).ToList();
        var calls = cap.Calls.Where(c => !c.Self && cap.Ifaces.Any(i => i.Name == c.I)).ToList();
        // A traced method the emulator never logs (zero lines for it in the whole window, e.g. a getter on a path that does not log)
        // cannot be compared: it is reported as a note, not a failure. A method the log shows elsewhere in the window still must match.
        var seenInLog = Items(window).Select(i => i.Iface + "." + i.Method).ToHashSet();
        var uncomparable = calls.Select(c => c.I + "." + c.M).Distinct().Where(k => !seenInLog.Contains(k)).OrderBy(k => k).ToList();
        var trace = calls.Where(c => !uncomparable.Contains(c.I + "." + c.M)).Select(FromCall).ToList();

        var mismatch = new List<string>(); var onlyTrace = new List<string>(); var onlyLog = new List<string>();
        var matched = 0;

        // Exact rules: sets of (iface.method(args), result).
        var logExact = coreItems.Where(i => i.Exact != null).Select(i => i.Exact!).ToHashSet();
        var marginExact = marginItems.Where(i => i.Exact != null).Select(i => i.Exact!).ToHashSet();
        var traceExact = trace.Where(t => t.Exact != null).Select(t => t.Exact!).ToHashSet();
        var logKeys = logExact.Select(t => t.Key).ToHashSet();
        var traceKeys = traceExact.Select(t => t.Key).ToHashSet();
        var reported = new HashSet<string>();
        foreach (var t in traceExact.Where(t => !logExact.Contains(t) && !marginExact.Contains(t)).OrderBy(t => t.Key))
        {
            if (logKeys.Contains(t.Key))
            {
                reported.Add(t.Key);
                mismatch.Add(t.Key + " = " + t.Result + "  (log: " + string.Join("|", logExact.Where(l => l.Key == t.Key).Select(l => l.Result)) + ")");
            }
            else onlyTrace.Add(t.Key + " = " + t.Result);
        }
        foreach (var t in logExact.Where(t => !traceExact.Contains(t)).OrderBy(t => t.Key))
            if (!traceKeys.Contains(t.Key)) onlyLog.Add(t.Key + " = " + t.Result);
            else if (!reported.Contains(t.Key)) mismatch.Add(t.Key + " = " + t.Result + "  (trace: " + string.Join("|", traceExact.Where(x => x.Key == t.Key).Select(x => x.Result)) + ")");
        matched += traceExact.Count(t => logExact.Contains(t));

        // Generic rule: values of a trace record must all appear in one log line of that method (any line of the window, margins
        // included), and each core log line must contain the values of some trace record.
        var allLog = coreItems.Concat(marginItems).Where(i => i.Tokens != null).ToList();
        bool Explains(TraceItem t, LogItem l) => t.Iface == l.Iface && t.Method == l.Method && t.ArgAtoms.IsSubsetOf(l.Tokens!) && (t.ResultAlts.Count == 0 || t.ResultAlts.Any(a => a.IsSubsetOf(l.Tokens!)));
        var genericTrace = trace.Where(t => t.Exact == null).GroupBy(t => t.Text).Select(g => g.First()).ToList();
        foreach (var t in genericTrace.OrderBy(t => t.Text))
        {
            if (allLog.Any(l => Explains(t, l))) { matched++; continue; }
            (allLog.Any(l => l.Iface == t.Iface && l.Method == t.Method) ? mismatch : onlyTrace).Add(t.Text + "  (no log line holds these values)");
        }
        foreach (var l in coreItems.Where(i => i.Tokens != null).GroupBy(i => i.Iface + "." + i.Text).Select(g => g.First()).OrderBy(l => l.Text))
            if (!trace.Any(t => t.Exact == null && Explains(t, l))) onlyLog.Add(l.Iface + "." + l.Text);

        var needed = new[] { "GetFriendRelationship", "GetFriendCount", "GetFriendByIndex", "GetFriendPersonaState", "ActivateGameOverlayToUser" };
        var missing = cap.Ifaces.Any(i => i.Name == "SteamFriends017") ? needed.Where(m => !calls.Any(c => c.I == "SteamFriends017" && c.M == m)).ToList() : new List<string>();
        var covered = cap.Ifaces.SelectMany(i => i.Patched.Select(m => i.Name + "." + m)).Count(k => calls.Any(c => c.I + "." + c.M == k));
        if (matched == 0 && mismatch.Count + onlyTrace.Count + onlyLog.Count == 0) reasons.Add("nothing could be compared (no traced call has a log line in the window)");
        var pass = mismatch.Count == 0 && onlyTrace.Count == 0 && onlyLog.Count == 0 && reasons.Count == 0;
        var verdict = pass ? "pass" : reasons.Count > 0 && mismatch.Count + onlyTrace.Count + onlyLog.Count == 0 ? "inconclusive" : "fail";

        if (json)
        {
            Console.WriteLine(JsonSerializer.Serialize(new
            {
                result = verdict, windowBytes = end - start.Value, logLines = core.Count, traceRecords = calls.Count, matched, methodsSeen = covered,
                mismatch, onlyInTrace = onlyTrace, onlyInLog = onlyLog, reasons, missingMethods = missing, notComparable = uncomparable,
            }));
        }
        else
        {
            Console.WriteLine("result: " + verdict.ToUpperInvariant());
            foreach (var r in reasons) Console.WriteLine("  inconclusive: " + r);
            Console.WriteLine($"window: {end - start.Value} bytes, {core.Count} log lines, {calls.Count} trace records, {covered} methods seen");
            Console.WriteLine("matched " + matched);
            Print("mismatch", mismatch); Print("only in trace", onlyTrace); Print("only in log", onlyLog);
            if (uncomparable.Count > 0) Console.WriteLine("note: emulator log has no lines for " + string.Join(", ", uncomparable) + " (cannot be compared)");
            if (missing.Count > 0) Console.WriteLine("note: no record in the window for " + string.Join(", ", missing) + " (scenario may be too small)");
        }
        return pass ? 0 : 1;
    }

    private static bool IsLineStart(string path, long pos)
    {
        if (pos <= 0) return true;
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        if (pos > fs.Length) return true;
        fs.Seek(pos - 1, SeekOrigin.Begin);
        return fs.ReadByte() == '\n';
    }

    private static void Print(string title, List<string> items)
    {
        Console.WriteLine(title + ": " + items.Count);
        foreach (var i in items.Take(20)) Console.WriteLine("  " + i);
        if (items.Count > 20) Console.WriteLine($"  +{items.Count - 20} more");
    }
}
