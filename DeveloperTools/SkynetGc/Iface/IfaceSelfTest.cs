using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace SKYNET.Gc.Iface;

/// <summary>
/// iface selftest [--keep DIR]: offline checks on a synthetic capture and a synthetic emulator log (no game, no
/// injection), then the hook exe's own selftest. --keep leaves the synthetic capture in DIR (used to drive skynet-mcp).
/// </summary>
internal static class IfaceSelfTest
{
    private static int _ok, _fail;

    private static void Check(string name, bool cond, string detail = "")
    {
        if (cond) _ok++; else { _fail++; Console.WriteLine("FAIL " + name + (detail.Length > 0 ? ": " + detail : "")); }
    }

    private const string Id = "76561197960275660";
    private const string P = " SteamFriends017: ";

    // Runs a command with Console.Out captured.
    private static (int code, string text) Cap(Func<int> f)
    {
        var old = Console.Out;
        var sw = new StringWriter();
        Console.SetOut(sw);
        try { var c = f(); return (c, sw.ToString()); }
        finally { Console.SetOut(old); }
    }

    [Flags] internal enum Fault { None = 0, Mismatch = 1, OnlyTrace = 2, OnlyLog = 4, Drop = 8, Unchanged = 16, EdgeOnly = 32, GenMismatch = 64, NoPersonaLog = 128, PersonaLogOnly = 256 }

    /// <summary>Builds capture + emulator log. Returns the capture folder.</summary>
    internal static string Synth(string dir, Fault fault = Fault.None)
    {
        Directory.CreateDirectory(dir);
        var log = Path.Combine(dir, "steam_api.client.log");
        var utf8 = new UTF8Encoding(false);
        File.WriteAllText(log, "", utf8);
        void Log(params string[] lines) => File.AppendAllLines(log, lines, utf8);

        Log(" SteamAPI: startup", P + "GetFriendRichPresence (SteamID = " + Id + ", Key = boot) = ");     // before the window
        var s0 = new FileInfo(log).Length;

        var q0 = Stopwatch.GetTimestamp();
        var step = Stopwatch.Frequency / 100;
        var seq = 0;
        var calls = new List<string>();
        void Call(string m, string[] a, string r, string? rn = null, string extra = "", bool self = false, int rep = 1, string iface = "SteamFriends017")
        {
            seq++;
            calls.Add("{\"k\":\"call\",\"seq\":" + seq + ",\"q\":" + (q0 + seq * step) + ",\"tid\":9216,\"i\":\"" + iface + "\",\"m\":\"" + m + "\",\"a\":" + JsonSerializer.Serialize(a) +
                      ",\"r\":" + JsonSerializer.Serialize(r) + (rn != null ? ",\"rn\":" + JsonSerializer.Serialize(rn) : "") + extra +
                      ",\"c\":\"client.dll+0x5A1B20\"" + (self ? ",\"self\":true" : "") + (rep > 1 ? ",\"rep\":" + rep + ",\"qlast\":" + (q0 + seq * step + 5) : "") + "}");
        }
        string Rich(string k, string v) => P + "GetFriendRichPresence (SteamID = " + Id + ", Key = " + k + ") = " + v;

        Call("GetPersonaState", Array.Empty<string>(), "1", "k_EPersonaStateOnline", self: true);
        // Head margin (10 lines): logged and traced.
        for (var i = 0; i < 10; i++) { Log(Rich("h" + i, "")); Call("GetFriendRichPresence", new[] { Id, "h" + i }, ""); }
        // Core lines: the 11 methods in the emulator's formats.
        Log(P + "GetFriendCount (FriendFlags = 0x6) = 1"); Call("GetFriendCount", new[] { "0x6" }, "1", null, ",\"af\":\"k_EFriendFlagFriendshipRequested|k_EFriendFlagImmediate\"");
        Log(P + "GetFriendByIndex (Index = 0, FriendFlags = 6) = [U:1:9932] (" + Id + ")"); Call("GetFriendByIndex", new[] { "0", "0x6" }, Id, "[U:1:9932]");
        Log(P + "GetFriendByIndex (Index = 1, FriendFlags = 6) = invalid"); Call("GetFriendByIndex", new[] { "1", "0x6" }, "0", "invalid");
        Log(P + "GetFriendRelationship (SteamID = " + Id + ") = k_EFriendRelationshipFriend (real k_EFriendRelationshipRequestInitiator)");
        Call("GetFriendRelationship", new[] { Id }, (fault & Fault.Mismatch) != 0 ? "4" : "3", (fault & Fault.Mismatch) != 0 ? "k_EFriendRelationshipRequestInitiator" : "k_EFriendRelationshipFriend", rep: 3);
        Log(P + "GetFriendPersonaName (SteamID = [U:1:9932] (" + Id + ")) = Zo\u00EB = x"); Call("GetFriendPersonaName", new[] { Id }, "Zo\u00EB = x");
        Log(P + "GetFriendPersonaState " + Id + " = k_EPersonaStateOnline"); Call("GetFriendPersonaState", new[] { Id }, "1", "k_EPersonaStateOnline");
        Log(P + "GetFriendGamePlayed (SteamID = " + Id + ") = True"); Call("GetFriendGamePlayed", new[] { Id }, "true", null, ",\"o\":{\"gameID\":\"570\",\"ip\":0,\"port\":0,\"queryPort\":0,\"lobby\":\"0\"}");
        Log(Rich("status", "#DOTA_RP_IDLE")); Call("GetFriendRichPresence", new[] { Id, "status" }, "#DOTA_RP_IDLE");
        Log(Rich("party", "")); Call("GetFriendRichPresence", new[] { Id, "party" }, "");
        Log(P + "HasFriend " + Id); Call("HasFriend", new[] { Id, "0x4" }, "true");
        if ((fault & Fault.NoPersonaLog) == 0) Log(P + "GetPersonaState  = k_EPersonaStateOnline");
        if ((fault & Fault.PersonaLogOnly) == 0) Call("GetPersonaState", Array.Empty<string>(), "1", "k_EPersonaStateOnline");
        Log(P + "ActivateGameOverlayToUser (GroupID = friendadd, SteamID = [U:1:9932] (" + Id + "))"); Call("ActivateGameOverlayToUser", new[] { "friendadd", Id }, "");
        Log(P + "RequestUserInformation (SteamID = " + Id + ", RequireNameOnly = False) = queued:False available:True requesting:False"); Call("RequestUserInformation", new[] { Id, "false" }, "true");
        // The emulator suppresses consecutive duplicates: the traced repeat has no second log line.
        Call("GetFriendCount", new[] { "0x6" }, "1");
        for (var i = 0; i < 4; i++) { Log(Rich("c" + i, "")); Call("GetFriendRichPresence", new[] { Id, "c" + i }, ""); }
        // A second interface with no exact rule: the generic comparison.
        Log(" SteamUser023: GetHSteamUser = 7"); Call("GetHSteamUser", new string[0], (fault & Fault.GenMismatch) != 0 ? "8" : "7", iface: "SteamUser023");
        Log(" SteamUser023: BLoggedOn = True"); Call("BLoggedOn", new string[0], "true", iface: "SteamUser023");
        Log(" SteamUser023: GetSteamID = [U:1:9932] (" + Id + ")"); Call("GetSteamID", new string[0], Id, "[U:1:9932]", iface: "SteamUser023");
        Log(" SteamUser023: GetUserDataFolder (Buffer = 0x7FFA11223344, Size = 260) = True"); Call("GetUserDataFolder", new[] { "0x7FFA11223344", "260" }, "true", iface: "SteamUser023");
        if ((fault & Fault.OnlyTrace) != 0) Call("GetFriendRichPresence", new[] { Id, "only-trace" }, "x");
        if ((fault & Fault.OnlyLog) != 0) Log(Rich("only-log", "y"));
        // Tail margin (10 lines).
        for (var i = 0; i < 10; i++) { Log(Rich("t" + i, "")); Call("GetFriendRichPresence", new[] { Id, "t" + i }, ""); }
        if ((fault & Fault.EdgeOnly) != 0) Log(Rich("edge-log", "z"));            // logged at the very end of the window, never traced: skew, excused
        var s1 = new FileInfo(log).Length;
        Log(Rich("after", ""));                                                   // after the window
        if ((fault & Fault.EdgeOnly) != 0) Call("GetFriendRichPresence", new[] { Id, "after" }, ""); // traced just before stop, logged just after: excused

        var meta = (string status, long? size) => "{\"k\":\"meta\",\"v\":1,\"provider\":\"emulator\",\"pid\":1,\"qfreq\":" + Stopwatch.Frequency + ",\"q0\":" + q0 +
            ",\"ifaces\":[{\"i\":\"SteamFriends017\",\"slots\":80,\"patched\":[\"GetPersonaState\",\"GetFriendCount\",\"GetFriendByIndex\",\"GetFriendRelationship\",\"GetFriendPersonaState\",\"GetFriendPersonaName\",\"GetFriendGamePlayed\",\"GetFriendRichPresence\",\"HasFriend\",\"ActivateGameOverlayToUser\",\"RequestUserInformation\"],\"skipped\":0,\"error\":null},{\"i\":\"SteamUser023\",\"slots\":30,\"patched\":[\"GetHSteamUser\",\"BLoggedOn\",\"GetSteamID\",\"GetUserDataFolder\"],\"skipped\":1,\"error\":null}],\"status\":\"" + status +
            "\",\"emu_log\":" + JsonSerializer.Serialize(log) + ",\"emu_log_method\":\"suffix-env\",\"emu_log_size\":" + (size?.ToString() ?? "null") + "}";
        var lines = new List<string> { meta("waiting", null), meta("ok", s0) };
        lines.Add("{\"k\":\"skip\",\"i\":\"SteamUser023\",\"m\":\"TrackAppUsageEvent\",\"why\":\"parameter gameID: unknown or by-value struct type CGameID\"}");
        lines.AddRange(calls);
        if ((fault & Fault.Drop) != 0) lines.Add("{\"k\":\"drop\",\"n\":7}");
        if ((fault & Fault.Unchanged) != 0) lines.Add(meta("emu-log-unchanged", s0));
        lines.Add("{\"k\":\"stop\",\"q\":" + (q0 + (seq + 5) * step) + ",\"emu_log_size\":" + s1 + ",\"records\":" + seq + ",\"dropped\":0,\"faults\":0}");
        File.WriteAllLines(Path.Combine(dir, "iface.jsonl"), lines, utf8);
        // Marks at known times: 1 "baseline", 2 "action", 3 "action" (same label twice), 4 "end".
        var marks = new[] { (1, "baseline", 5), (2, "action", 12), (3, "action", 20), (4, "end", 30) };
        File.WriteAllLines(Path.Combine(dir, "marks.jsonl"),
            marks.Select(m => "{\"k\":\"mark\",\"n\":" + m.Item1 + ",\"label\":\"" + m.Item2 + "\",\"q\":" + (q0 + m.Item3 * step + step / 2) + "}"), utf8);
        return dir;
    }

    public static int Run(string? keep)
    {
        var root = keep ?? Path.Combine(Path.GetTempPath(), "skynet-iface-selftest-" + Guid.NewGuid().ToString("N"));
        var dir = Synth(Path.Combine(root, "ok"));
        Parsers();
        Readers(dir);
        Diffs(root);
        if (keep == null) { try { Directory.Delete(root, true); } catch { } }
        else Console.WriteLine("synthetic capture kept: " + dir);

        var hook = Path.Combine(AppContext.BaseDirectory, "hook", "skynet-iface-hook.exe");
        if (File.Exists(hook))
        {
            using var p = Process.Start(new ProcessStartInfo(hook, "selftest") { UseShellExecute = false })!;
            p.WaitForExit();
            Check("hook exe selftest", p.ExitCode == 0, "exit " + p.ExitCode);
        }
        else Console.WriteLine("note: " + hook + " not found, hook selftest skipped");
        Console.WriteLine("iface selftest: " + _ok + " passed, " + _fail + " failed");
        return _fail == 0 ? 0 : 1;
    }

    private static void Parsers()
    {
        var ifs = new List<IfaceInfo>
        {
            new() { Name = "SteamFriends017", Patched = new HashSet<string>(new[] { "GetPersonaState", "GetFriendCount", "GetFriendByIndex", "GetFriendRelationship", "GetFriendPersonaState", "GetFriendPersonaName", "GetFriendGamePlayed", "GetFriendRichPresence", "HasFriend", "ActivateGameOverlayToUser", "RequestUserInformation" }) },
            new() { Name = "SteamUser023", Patched = new HashSet<string>(new[] { "BLoggedOn", "GetUserDataFolder" }) },
        };
        string? K(string line) { var it = IfaceDiff.ParseLine(line, ifs); return it?.Exact == null ? null : it.Exact.Key + "=" + it.Exact.Result; }
        var g = IfaceDiff.ParseLine(" SteamUser023: BLoggedOn = True", ifs);
        Check("generic line has tokens, no exact rule", g != null && g.Exact == null && g.Tokens!.Contains("true"));
        Check("generic: only patched methods", IfaceDiff.ParseLine(" SteamUser023: GetSteamID = 5", ifs) == null);
        Check("generic: pointer in log is a normalised number", IfaceDiff.ParseLine(" SteamUser023: GetUserDataFolder (Buffer = 0x10, Size = 260) = True", ifs)!.Tokens!.IsSupersetOf(new[] { "16", "260", "true", "buffer" }));
        Check("count hex", K(P + "GetFriendCount (FriendFlags = 0x6) = 1") == "SteamFriends017.GetFriendCount(6)=1");
        Check("byindex dec flags", K(P + "GetFriendByIndex (Index = 0, FriendFlags = 6) = [U:1:9932] (" + Id + ")") == "SteamFriends017.GetFriendByIndex(0,6)=" + Id);
        Check("byindex invalid", K(P + "GetFriendByIndex (Index = 1, FriendFlags = 6) = invalid") == "SteamFriends017.GetFriendByIndex(1,6)=0");
        Check("relationship real", K(P + "GetFriendRelationship (SteamID = 5) = k_EFriendRelationshipFriend (real k_EFriendRelationshipRequestInitiator)") == "SteamFriends017.GetFriendRelationship(5)=k_EFriendRelationshipFriend");
        Check("relationship plain", K(P + "GetFriendRelationship (SteamID = 5) = k_EFriendRelationshipNone") == "SteamFriends017.GetFriendRelationship(5)=k_EFriendRelationshipNone");
        Check("name with = and utf8", K(P + "GetFriendPersonaName (SteamID = [U:1:9] (5)) = Zo\u00EB = x") == "SteamFriends017.GetFriendPersonaName(5)=Zo\u00EB = x");
        Check("rich empty value", K(P + "GetFriendRichPresence (SteamID = 5, Key = party) = ") == "SteamFriends017.GetFriendRichPresence(5,party)=");
        Check("rich empty value, trailing space trimmed", K(P + "GetFriendRichPresence (SteamID = 5, Key = party) =") == "SteamFriends017.GetFriendRichPresence(5,party)=");
        Check("gameplayed lower", K(P + "GetFriendGamePlayed (SteamID = 5) = False") == "SteamFriends017.GetFriendGamePlayed(5)=false");
        Check("hasfriend presence", K(P + "HasFriend 5") == "SteamFriends017.HasFriend(5)=");
        Check("personastate constant", K(P + "GetPersonaState  = k_EPersonaStateOnline") == "SteamFriends017.GetPersonaState()=");
        Check("activate", K(P + "ActivateGameOverlayToUser (GroupID = friendadd, SteamID = [U:1:9] (5))") == "SteamFriends017.ActivateGameOverlayToUser(friendadd,5)=");
        Check("requestuserinfo args only", K(P + "RequestUserInformation (SteamID = 5, RequireNameOnly = True) = queued:True available:False requesting:True") == "SteamFriends017.RequestUserInformation(5,true)=");
        Check("prefix anchored", K(" SteamFriends016: HasFriend 5") == null && K(" Other: GetPersonaState") == null && K("GetFriendCount (FriendFlags = 0x6) = 1") == null);
        Check("ignored method", K(P + "GetPersonaName Zoe") == null);
    }

    private static void Readers(string dir)
    {
        var cap = IfaceCapture.Load(dir);
        Check("load meta", cap.HasMeta && cap.MetaStr("status") == "ok" && cap.HasStop);
        Check("load calls", cap.Calls.Count == 43 && cap.Calls.Count(c => c.Self) == 1, "calls=" + cap.Calls.Count);
        Check("marks", cap.Marks.Count == 4);
        Check("mark start=first end=last", cap.ResolveMark("action", false)!.N == 2 && cap.ResolveMark("action", true)!.N == 3 && cap.ResolveMark("@last", true)!.N == 4 && cap.ResolveMark("3", false)!.Label == "action");

        var (c1, t1) = Cap(() => IfaceCommand.Run(new[] { "timeline", "--dir", dir, "--method", "Relationship|Count" }));
        Check("timeline exit", c1 == 0);
        Check("timeline format", t1.Contains("GetFriendRelationship(" + Id + ") = 3 k_EFriendRelationshipFriend @client.dll+0x5A1B20 x3") && t1.Contains("T9216") && t1.Contains("+0.") , t1);
        Check("timeline marks interleaved", Cap(() => IfaceCommand.Run(new[] { "timeline", "--dir", dir, "--limit", "100" })).text.Contains("--- mark 2 \"action\" ---"));
        var (_, tb) = Cap(() => IfaceCommand.Run(new[] { "timeline", "--dir", dir, "--between", "baseline", "action", "--limit", "100" }));
        Check("between baseline..action(last)", tb.Contains("--- mark 1") && tb.Contains("--- mark 3") && !tb.Contains("--- mark 4") && !tb.Contains("#2 "), tb);
        var (_, tb2) = Cap(() => IfaceCommand.Run(new[] { "timeline", "--dir", dir, "--between", "action", "@last", "--limit", "100" }));
        Check("between first action..@last", tb2.Contains("--- mark 2") && tb2.Contains("--- mark 4") && !tb2.Contains("--- mark 1"), tb2);
        foreach (var form in new[] { Id, "[U:1:9932]", "9932" })
        {
            var (_, ts) = Cap(() => IfaceCommand.Run(new[] { "timeline", "--dir", dir, "--steamid", form, "--method", "GetFriendByIndex", "--limit", "100" }));
            Check("steamid form " + form, ts.Contains("GetFriendByIndex(0, 0x6) = " + Id + " [U:1:9932]") && !ts.Contains("GetFriendByIndex(1,"), ts);
        }
        var (_, tc) = Cap(() => IfaceCommand.Run(new[] { "timeline", "--dir", dir, "--contains", "DOTA_RP" }));
        Check("contains", tc.Contains("#") && tc.Trim().Split('\n').Length == 1, tc);
        var (_, page) = Cap(() => IfaceCommand.Run(new[] { "timeline", "--dir", dir, "--limit", "5", "--offset", "0" }));
        Check("pagination next_offset", page.Contains("next_offset="), page);
        var (cj, tj) = Cap(() => IfaceCommand.Run(new[] { "timeline", "--dir", dir, "--limit", "3", "--json" }));
        Check("timeline json", cj == 0 && JsonDocument.Parse(tj).RootElement.GetProperty("rows").GetArrayLength() == 3);
        var (cq, tq) = Cap(() => IfaceCommand.Run(new[] { "query", "--dir", dir, "--method", "GetFriendCount|GetFriendRelationship" }));
        Check("query grouped", cq == 0 && tq.Contains("GetFriendCount(0x6) -> 1") && tq.Contains("x2  seq") && tq.Contains("-> 3 k_EFriendRelationshipFriend  x3") && tq.Contains("@client.dll+0x5A1B20"), tq);
        var oldErr = Console.Error; Console.SetError(TextWriter.Null);
        try { Check("query needs method", Cap(() => IfaceCommand.Run(new[] { "query", "--dir", dir })).code == 2); } finally { Console.SetError(oldErr); }
        var (_, qj) = Cap(() => IfaceCommand.Run(new[] { "query", "--dir", dir, "--method", "HasFriend", "--json" }));
        Check("query json", JsonDocument.Parse(qj).RootElement.GetProperty("rows")[0].GetProperty("count").GetInt32() == 1);
        var (cs, ts2) = Cap(() => IfaceCommand.Run(new[] { "status", "--dir", dir }));
        Check("status", cs == 0 && ts2.Contains("provider=emulator status=ok") && ts2.Contains("SteamFriends017: patched=11") && ts2.Contains("SteamUser023: patched=4") && ts2.Contains("skip x1:") && ts2.Contains("records=42") && ts2.Contains("dropped=0") && ts2.Contains("marks=4") && ts2.Contains("stopped=True"), ts2);
        var (_, sj) = Cap(() => IfaceCommand.Run(new[] { "status", "--dir", dir, "--json" }));
        Check("status json", JsonDocument.Parse(sj).RootElement.GetProperty("records").GetInt32() == 42);
        var (_, tf) = Cap(() => IfaceCommand.Run(new[] { "timeline", "--dir", dir, "--iface", "SteamUser", "--limit", "100" }));
        Check("iface filter and Interface.Method names", tf.Contains("SteamUser023.BLoggedOn() = true") && tf.Contains("SteamUser023.GetSteamID() = " + Id + " [U:1:9932]") && !tf.Contains("SteamFriends017"), tf);
        var (cm, tm) = Cap(() => IfaceCommand.Run(new[] { "mark", "from", "the", "cli", "--dir", dir }));
        Check("mark cli", cm == 0 && tm.StartsWith("mark n=5") && IfaceCapture.Load(dir).Marks.Last().Label == "from the cli", tm);
    }

    private static void Diffs(string root)
    {
        (int code, string text) D(string name, Fault f) => Cap(() => IfaceDiff.Run(IfaceCapture.Load(Synth(Path.Combine(root, name), f)), null, false));

        var ok = D("d-ok", Fault.None);
        Check("diff clean pass", ok.code == 0 && ok.text.Contains("result: PASS") && ok.text.Contains("mismatch: 0") && ok.text.Contains("only in trace: 0") && ok.text.Contains("only in log: 0"), ok.text);
        var edge = D("d-edge", Fault.EdgeOnly);
        Check("edge skew excused", edge.code == 0, edge.text);
        var mm = D("d-mm", Fault.Mismatch);
        Check("mismatch detected", mm.code == 1 && mm.text.Contains("result: FAIL") && mm.text.Contains("mismatch: 1") && mm.text.Contains("GetFriendRelationship(" + Id + ")"), mm.text);
        var ot = D("d-ot", Fault.OnlyTrace);
        Check("only in trace", ot.code == 1 && ot.text.Contains("only in trace: 1") && ot.text.Contains("only-trace"), ot.text);
        var ol = D("d-ol", Fault.OnlyLog);
        Check("only in log", ol.code == 1 && ol.text.Contains("only in log: 1") && ol.text.Contains("only-log"), ol.text);
        var dr = D("d-dr", Fault.Drop);
        Check("drops are inconclusive", dr.code == 1 && dr.text.Contains("INCONCLUSIVE") && dr.text.Contains("7 records dropped"), dr.text);
        var un = D("d-un", Fault.Unchanged);
        Check("unchanged log is inconclusive", un.code == 1 && un.text.Contains("INCONCLUSIVE") && un.text.Contains("did not grow"), un.text);
        var np = D("d-np", Fault.NoPersonaLog);
        Check("method absent from the log: PASS with a note", np.code == 0 && np.text.Contains("result: PASS") && np.text.Contains("emulator log has no lines for SteamFriends017.GetPersonaState (cannot be compared)"), np.text);
        var pl = D("d-pl", Fault.PersonaLogOnly);
        Check("method in the log but never traced: FAIL", pl.code == 1 && pl.text.Contains("only in log: 1") && pl.text.Contains("GetPersonaState"), pl.text);
        var gm = D("d-gm", Fault.GenMismatch);
        Check("generic mismatch detected", gm.code == 1 && gm.text.Contains("mismatch: 1") && gm.text.Contains("SteamUser023.GetHSteamUser"), gm.text);
        var js = Cap(() => IfaceDiff.Run(IfaceCapture.Load(Path.Combine(root, "d-mm")), null, true));
        Check("diff json", JsonDocument.Parse(js.text).RootElement.GetProperty("result").GetString() == "fail");
        // The override path and a missing log.
        var cap = IfaceCapture.Load(Path.Combine(root, "d-ok"));
        Check("override path", Cap(() => IfaceDiff.Run(cap, Path.Combine(root, "d-ok", "steam_api.client.log"), false)).code == 0);
        var old = Console.Error; Console.SetError(TextWriter.Null);
        try { Check("missing log is usage error", IfaceDiff.Run(cap, Path.Combine(root, "nope.log"), false) == 2); } finally { Console.SetError(old); }
    }
}
