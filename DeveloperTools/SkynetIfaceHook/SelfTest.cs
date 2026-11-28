using System.Runtime.InteropServices;
using System.Text;

namespace SKYNET.Iface;

/// <summary>
/// Offline checks (no game, no injection). The ABI part builds a fake 80-slot vtable out of CLR thunks (what the
/// emulator's vtable is made of), patches it, and calls each slot from native machine code (jmp [slot]) so the
/// call enters our thunk through the real x64 calling convention, with the real signatures of the 11 methods.
/// </summary>
internal static class SelfTest
{
    private static int _fail, _ok;

    private static void Check(string name, bool cond, string detail = "")
    {
        if (cond) _ok++; else { _fail++; Console.WriteLine("FAIL " + name + (detail.Length > 0 ? ": " + detail : "")); }
    }

    private static void Eq(string name, object? got, object? want) => Check(name, Equals(got, want), "got '" + got + "' want '" + want + "'");

    // Real shapes of the watched methods; also used as the fake callees.
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate int FInt0(IntPtr self);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate int FCount(IntPtr self, int flags);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate IntPtr FByIndex(IntPtr self, IntPtr ret, int index, int flags);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate int FId(IntPtr self, ulong id);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate IntPtr FIdStr(IntPtr self, ulong id);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] [return: MarshalAs(UnmanagedType.U1)] private delegate bool FGame(IntPtr self, ulong id, IntPtr info);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate IntPtr FRich(IntPtr self, ulong id, IntPtr key);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] [return: MarshalAs(UnmanagedType.U1)] private delegate bool FHas(IntPtr self, ulong id, int flags);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate void FAct(IntPtr self, IntPtr dialog, ulong id);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] [return: MarshalAs(UnmanagedType.U1)] private delegate bool FReq(IntPtr self, ulong id, [MarshalAs(UnmanagedType.U1)] bool nameOnly);

    [DllImport("kernel32.dll")] private static extern IntPtr VirtualAlloc(IntPtr addr, UIntPtr size, uint type, uint prot);

    private const ulong Friend = 76561197960275660UL;

    public static int Run(string[] args)
    {
        var spec = args.Length > 1 && args[0] == "--spec-dir" ? args[1] : Path.Combine(AppContext.BaseDirectory, "meta");
        Decoders(spec);
        Writer();
        Abi(spec);
        Request();
        Console.WriteLine("iface-hook selftest: " + _ok + " passed, " + _fail + " failed");
        return _fail == 0 ? 0 : 1;
    }

    private static void Decoders(string specDir)
    {
        var rows = Spec.Load(Path.Combine(specDir, "SteamFriends017.txt"));
        Eq("spec rows", rows.Count, 80);
        foreach (var m in Spec.DefaultMethods)
        {
            var r = rows.FirstOrDefault(x => x.Name == m);
            var plan = r == null ? null : Spec.Build("SteamFriends017", r);
            Check("preset method supported: " + m, plan != null && plan.Skip == null && plan.Arity <= 4, plan?.Skip ?? "");
        }
        var byIdx = Spec.Build("SteamFriends017", rows.First(x => x.Name == "GetFriendByIndex"));
        Check("GetFriendByIndex hidden return, arity 4", byIdx.Hidden && byIdx.Arity == 4);
        Eq("param type", rows.First(x => x.Name == "ActivateGameOverlayToUser").ParamTypes[0], "const char *");
        var ff = Types.FlagParams["iFriendFlags"];
        Eq("flags", Types.FlagNames(ff, 6), "k_EFriendFlagFriendshipRequested|k_EFriendFlagImmediate");
        Eq("flags none", Types.FlagNames(ff, 0), "none");
        Eq("flags unknown bit", Types.FlagNames(ff, 0x2000), "0x2000");
        Eq("steamid name", Types.SteamIdName(Friend), "[U:1:9932]");
        Eq("steamid invalid", Types.SteamIdName(0), "invalid");
        Eq("enum unknown value", Types.EnumName("EPersonaState", "99"), "99");
        Eq("enum name", Types.EnumName("EPersonaState", "1"), "k_EPersonaStateOnline");

        // Type tables and the support rule.
        Eq("kind handle", Types.Resolve("HSteamUser", true, out _), K.I32);
        Eq("kind enum by convention", Types.Resolve("EResult", true, out _), K.Enum);
        Eq("kind const char*", Types.Resolve("const char *", false, out _), K.CStr);
        Eq("kind char* is a buffer pointer", Types.Resolve("char *", false, out _), K.Ptr);
        Eq("kind ref", Types.Resolve("const SteamNetworkingIPAddr &", false, out _), K.Ptr);
        Check("float unsupported", Types.Resolve("float", false, out var why) == null && why!.Contains("xmm"));
        Check("unknown struct by value unsupported", Types.Resolve("CGameID", false, out why) == null && why!.Contains("CGameID"));
        var user = Spec.Load(Path.Combine(specDir, "SteamUser023.txt"));
        var plans = Spec.Select("SteamUser023", user, new string[0]);
        Check("all-methods selection builds every row", plans.Count == user.Count && plans.Count(p => p.Skip == null) > 10, plans.Count + " / " + plans.Count(p => p.Skip == null));
        Check("skip reason names the type", plans.First(p => p.Row.Name == "TrackAppUsageEvent").Skip!.Contains("CGameID"));
        Check("by name selection", Spec.Select("SteamUser023", user, new[] { "BLoggedOn" }).Count == 1);

        // Interface and version resolution against the committed spec files (247).
        string? Res(string tok) => Spec.Resolve(specDir, tok, out _);
        Eq("resolve exact", Res("SteamFriends017"), "SteamFriends017");
        Eq("resolve interface:version", Res("ISteamFriends:017"), "SteamFriends017");
        Eq("resolve user", Res("ISteamUser:23"), "SteamUser023");
        Eq("resolve upper-case family", Res("ISteamUGC:16"), "STEAMUGC_INTERFACE_VERSION016");
        Eq("resolve matchmaking not servers", Res("ISteamMatchmaking:9"), "SteamMatchMaking009");
        Eq("resolve sockets not serialized", Res("ISteamNetworkingSockets:13"), "SteamNetworkingSockets013");
        Check("version required when several exist", Spec.Resolve(specDir, "ISteamFriends", out var err) == null && err.Contains("give a version") && err.Contains("SteamFriends017"));
        Check("unknown interface", Spec.Resolve(specDir, "ISteamNope", out err) == null && err.Contains("no spec file"));
        Eq("spec files", Directory.GetFiles(specDir, "*.txt").Length, 247);
        Check("targets default says so", IfaceHookMain.Targets(specDir, null, null, out var ifs, out var ms, out var js) && ifs == "SteamFriends017" && ms.Split(',').Length == 11 && js.Contains("\"defaulted\":true"));
        Check("targets explicit = all supported", IfaceHookMain.Targets(specDir, "ISteamUser:23,ISteamFriends:17", null, out ifs, out ms, out js) && ifs == "SteamUser023,SteamFriends017" && ms == "" && js.Contains("all supported"));
        var oldErr = Console.Error; Console.SetError(TextWriter.Null);
        try { Check("targets reject unknown method", !IfaceHookMain.Targets(specDir, "ISteamUser:23", "Nope", out ifs, out ms, out js)); } finally { Console.SetError(oldErr); }

        var sb = new StringBuilder();
        Json.Str(sb, "q\"\\\n\u0001\u00EB");
        Eq("json escape", sb.ToString(), "\"q\\\"\\\\\\n\\u0001\u00EB\"");
        var rec = new Rec { I = "SteamFriends017", Seq = 3, Q = 10, QLast = 20, Tid = 7, M = "HasFriend", A = new[] { "1", "0x4" }, R = "true", C = "client.dll+0x10", Rep = 4 };
        Eq("rec json", rec.ToJson(), "{\"k\":\"call\",\"seq\":3,\"q\":10,\"tid\":7,\"i\":\"SteamFriends017\",\"m\":\"HasFriend\",\"a\":[\"1\",\"0x4\"],\"r\":\"true\",\"c\":\"client.dll+0x10\",\"rep\":4,\"qlast\":20}");

        // Strings: UTF-8, empty, null, bad pointer, and one ending at an uncommitted page (must not fault).
        var s = Marshal.AllocHGlobal(16);
        var bytes = Encoding.UTF8.GetBytes("Zo\u00EB\0");
        Marshal.Copy(bytes, 0, s, bytes.Length);
        Eq("read utf8", Mem.ReadString(s), "Zo\u00EB");
        Eq("read null", Mem.ReadString(IntPtr.Zero), "");
        Eq("read bad ptr", Mem.ReadString((IntPtr)0x10), "");
        var page = VirtualAlloc(IntPtr.Zero, (UIntPtr)8192, 0x2000, 0x04);   // reserve
        VirtualAlloc(page, (UIntPtr)4096, 0x1000, 0x04);                    // commit page 0 only
        var tail = IntPtr.Add(page, 4093);
        Marshal.Copy(Encoding.ASCII.GetBytes("abc"), 0, tail, 3);
        Eq("read to page end", Mem.ReadString(tail), "abc");
        Eq("read u64 bad", Mem.ReadU64((IntPtr)0x10), 0UL);
    }

    private static void Writer()
    {
        var path = Path.Combine(Path.GetTempPath(), "skynet-iface-selftest-" + Guid.NewGuid().ToString("N") + ".jsonl");
        var w = new TraceWriter(path);
        for (var i = 0; i < 3; i++) w.Offer(new Rec { M = "HasFriend", A = new[] { "1" }, R = "true", Q = 100 + i });
        w.Offer(new Rec { M = "HasFriend", A = new[] { "2" }, R = "false", Q = 200 });
        w.Drain();
        w.WriteLine("{\"k\":\"stop\"}");
        w.Flush();
        string[] lines;
        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) lines = new StreamReader(fs).ReadToEnd().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Eq("writer lines", lines.Length, 3);
        Check("repeat collapsed", lines[0].Contains("\"rep\":3") && lines[0].Contains("\"qlast\":102") && lines[0].Contains("\"seq\":1"), lines[0]);
        Check("second record", lines[1].Contains("\"seq\":2") && !lines[1].Contains("rep"), lines[1]);
        Eq("records counted", w.Records, 4L);
        w.Close();
        File.Delete(path);
    }

    private static IntPtr Str(string s)
    {
        var b = Encoding.UTF8.GetBytes(s + "\0");
        var p = Marshal.AllocHGlobal(b.Length);
        Marshal.Copy(b, 0, p, b.Length);
        return p;
    }

    private static void Abi(string specDir)
    {
        var rows = Spec.Load(Path.Combine(specDir, "SteamFriends017.txt"));
        var keep = new List<object>();
        var vt = Marshal.AllocHGlobal(80 * IntPtr.Size);
        var self = Marshal.AllocHGlobal(64);
        var dummy = new FInt0(_ => 0); keep.Add(dummy);
        for (var i = 0; i < 80; i++) Marshal.WriteIntPtr(vt, i * IntPtr.Size, Marshal.GetFunctionPointerForDelegate(dummy));
        var name = Str("Zo\u00EB");
        var status = Str("#DOTA_RP_IDLE");

        void Fake(string method, Delegate d) { keep.Add(d); Marshal.WriteIntPtr(vt, rows.First(r => r.Name == method).Index * IntPtr.Size, Marshal.GetFunctionPointerForDelegate(d)); }
        Fake("GetPersonaState", new FInt0(_ => 1));
        Fake("GetFriendCount", new FCount((_, f) => f == 6 ? 1 : 0));
        Fake("GetFriendByIndex", new FByIndex((_, ret, i, f) => { Marshal.WriteInt64(ret, (long)Friend); return ret; }));
        Fake("GetFriendRelationship", new FId((_, id) => 4));
        Fake("GetFriendPersonaState", new FId((_, id) => 1));
        Fake("GetFriendPersonaName", new FIdStr((_, id) => name));
        Fake("GetFriendGamePlayed", new FGame((_, id, info) => { Marshal.WriteInt64(info, 570); Marshal.WriteInt32(info, 8, 0x01020304); Marshal.WriteInt32(info, 12, 27015); Marshal.WriteInt64(info, 16, 99); return true; }));
        Fake("GetFriendRichPresence", new FRich((_, id, key) => Mem.ReadString(key) == "status" ? status : IntPtr.Zero));
        Fake("HasFriend", new FHas((_, id, f) => true));
        Fake("ActivateGameOverlayToUser", new FAct((_, d, id) => { }));
        Fake("RequestUserInformation", new FReq((_, id, n) => n));

        var recs = new List<Rec>();
        var faults = new List<string>();
        var ip = new Interposer(r => { lock (recs) recs.Add(r); }, faults.Add);
        var origPtrs = new Dictionary<string, IntPtr>();
        foreach (var m in Spec.DefaultMethods)
        {
            var row = rows.First(r => r.Name == m);
            origPtrs[m] = Marshal.ReadIntPtr(vt, row.Index * IntPtr.Size);
            ip.Patch(vt, Spec.Build("SteamFriends017", row));
        }

        T Call<T>(string method) where T : class
        {
            var row = rows.First(r => r.Name == method);
            return (T)(object)Marshal.GetDelegateForFunctionPointer(Stubs.ThroughSlot(IntPtr.Add(vt, row.Index * IntPtr.Size)), typeof(T));
        }

        Eq("GetPersonaState", Call<FInt0>("GetPersonaState")(self), 1);
        Eq("GetFriendCount", Call<FCount>("GetFriendCount")(self, 6), 1);
        var ret = Marshal.AllocHGlobal(8);
        Eq("GetFriendByIndex ret ptr", Call<FByIndex>("GetFriendByIndex")(self, ret, 0, 6), ret);
        Eq("GetFriendByIndex id", (ulong)Marshal.ReadInt64(ret), Friend);
        Eq("GetFriendRelationship", Call<FId>("GetFriendRelationship")(self, Friend), 4);
        Eq("GetFriendPersonaState", Call<FId>("GetFriendPersonaState")(self, Friend), 1);
        Eq("GetFriendPersonaName", Mem.ReadString(Call<FIdStr>("GetFriendPersonaName")(self, Friend)), "Zo\u00EB");
        var info = Marshal.AllocHGlobal(24);
        Check("GetFriendGamePlayed", Call<FGame>("GetFriendGamePlayed")(self, Friend, info));
        Eq("GetFriendRichPresence", Mem.ReadString(Call<FRich>("GetFriendRichPresence")(self, Friend, Str("status"))), "#DOTA_RP_IDLE");
        Eq("GetFriendRichPresence null", Call<FRich>("GetFriendRichPresence")(self, Friend, Str("none")), IntPtr.Zero);
        Check("HasFriend", Call<FHas>("HasFriend")(self, Friend, 4));
        Call<FAct>("ActivateGameOverlayToUser")(self, Str("friendadd"), Friend);
        Check("RequestUserInformation", Call<FReq>("RequestUserInformation")(self, Friend, true));
        Check("RequestUserInformation false", !Call<FReq>("RequestUserInformation")(self, Friend, false));

        Rec Get(string m, int n = 0) => recs.Where(r => r.M == m).ElementAt(n);
        Eq("recorded", recs.Count, 13);
        Eq("PersonaState rn", Get("GetPersonaState").Rn, "k_EPersonaStateOnline");
        Eq("Count a", string.Join(",", Get("GetFriendCount").A), "0x6");
        Eq("Count af", Get("GetFriendCount").Af, "k_EFriendFlagFriendshipRequested|k_EFriendFlagImmediate");
        Eq("ByIndex a", string.Join(",", Get("GetFriendByIndex").A), "0,0x6");
        Eq("ByIndex r", Get("GetFriendByIndex").R, Friend.ToString());
        Eq("ByIndex rn", Get("GetFriendByIndex").Rn, "[U:1:9932]");
        Eq("Relationship a", string.Join(",", Get("GetFriendRelationship").A), Friend.ToString());
        Eq("Relationship rn", Get("GetFriendRelationship").Rn, "k_EFriendRelationshipRequestInitiator");
        Eq("PersonaName r", Get("GetFriendPersonaName").R, "Zo\u00EB");
        Eq("GamePlayed r", Get("GetFriendGamePlayed").R, "true");
        Eq("GamePlayed o", Get("GetFriendGamePlayed").O, "{\"gameID\":\"570\",\"ip\":16909060,\"port\":27015,\"queryPort\":0,\"lobby\":\"99\"}");
        Eq("Rich a", string.Join(",", Get("GetFriendRichPresence").A), Friend + ",status");
        Eq("Rich r", Get("GetFriendRichPresence").R, "#DOTA_RP_IDLE");
        Eq("Rich null r", Get("GetFriendRichPresence", 1).R, "");
        Eq("Has a", string.Join(",", Get("HasFriend").A), Friend + ",0x4");
        Eq("Activate a", string.Join(",", Get("ActivateGameOverlayToUser").A), "friendadd," + Friend);
        Eq("Request a", string.Join(",", Get("RequestUserInformation").A), Friend + ",true");
        Eq("Request false a", string.Join(",", Get("RequestUserInformation", 1).A), Friend + ",false");
        Check("tid/q set", recs.All(r => r.Tid != 0 && r.Q > 0));

        // Self-call tagging and pass-through after stop.
        Interposer.SelfCall = true; Call<FInt0>("GetPersonaState")(self); Interposer.SelfCall = false;
        Check("self tagged", recs.Last().Self && ip.SelfRecorded == 1);
        ip.Stopped = true;
        var n0 = recs.Count;
        Eq("pass-through result", Call<FId>("GetFriendRelationship")(self, Friend), 4);
        Eq("pass-through records nothing", recs.Count, n0);
        ip.Stopped = false;

        // Restore rule: only if the slot still holds our thunk.
        var sl = ip.Slots.First(s => s.Plan.Row.Name == "HasFriend");
        Check("restore ours", ip.Restore(sl) && Marshal.ReadIntPtr(sl.Addr) == origPtrs["HasFriend"]);
        Check("restore again is a no-op", !ip.Restore(sl));
        var sl2 = ip.Slots.First(s => s.Plan.Row.Name == "GetFriendCount");
        var foreign = Marshal.GetFunctionPointerForDelegate(dummy);
        Marshal.WriteIntPtr(sl2.Addr, foreign);
        Check("foreign patch left alone", !ip.Restore(sl2) && Marshal.ReadIntPtr(sl2.Addr) == foreign);
        Eq("faults", faults.Count, 0);
        Generic(specDir);
        GC.KeepAlive(keep);
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] [return: MarshalAs(UnmanagedType.U1)] private delegate bool FBool0(IntPtr self);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate IntPtr FRet1(IntPtr self, IntPtr ret);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate int FHandle(IntPtr self);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate IntPtr FBuf(IntPtr self, IntPtr buf, int cb);

    // A second interface (SteamUser023) driven by the same code: handle return, bool, hidden-return CSteamID, buffer pointer.
    private static void Generic(string specDir)
    {
        var rows = Spec.Load(Path.Combine(specDir, "SteamUser023.txt"));
        var keep = new List<object>();
        var vt = Marshal.AllocHGlobal(rows.Count * IntPtr.Size);
        var self = Marshal.AllocHGlobal(64);
        for (var i = 0; i < rows.Count; i++) Marshal.WriteIntPtr(vt, i * IntPtr.Size, Marshal.GetFunctionPointerForDelegate(new FHandle(_ => 0)));
        void Fake(string m, Delegate d) { keep.Add(d); Marshal.WriteIntPtr(vt, rows.First(r => r.Name == m).Index * IntPtr.Size, Marshal.GetFunctionPointerForDelegate(d)); }
        Fake("GetHSteamUser", new FHandle(_ => 7));
        Fake("BLoggedOn", new FBool0(_ => true));
        Fake("GetSteamID", new FRet1((_, ret) => { Marshal.WriteInt64(ret, (long)Friend); return ret; }));
        Fake("GetUserDataFolder", new FBuf((_, buf, cb) => buf));

        var recs = new List<Rec>();
        var ip = new Interposer(r => { lock (recs) recs.Add(r); }, _ => { });
        var plans = Spec.Select("SteamUser023", rows, new[] { "GetHSteamUser", "BLoggedOn", "GetSteamID", "GetUserDataFolder", "TrackAppUsageEvent" });
        Eq("unsupported is reported, not patched", plans.Count(p => p.Skip != null), 1);
        foreach (var p in plans.Where(p => p.Skip == null)) ip.Patch(vt, p);
        T Call<T>(string m) where T : class => (T)(object)Marshal.GetDelegateForFunctionPointer(Stubs.ThroughSlot(IntPtr.Add(vt, rows.First(r => r.Name == m).Index * IntPtr.Size)), typeof(T));

        Eq("GetHSteamUser", Call<FHandle>("GetHSteamUser")(self), 7);
        Check("BLoggedOn", Call<FBool0>("BLoggedOn")(self));
        var ret = Marshal.AllocHGlobal(8);
        Eq("GetSteamID ptr", Call<FRet1>("GetSteamID")(self, ret), ret);
        var buf = Marshal.AllocHGlobal(32);
        Eq("GetUserDataFolder ptr", Call<FBuf>("GetUserDataFolder")(self, buf, 32), buf);
        Eq("generic records", recs.Count, 4);
        Eq("generic iface tag", string.Join(",", recs.Select(r => r.I).Distinct()), "SteamUser023");
        Eq("handle result", recs[0].R, "7");
        Eq("bool result", recs[1].R, "true");
        Eq("hidden-return steamid", recs[2].R + " " + recs[2].Rn, Friend + " [U:1:9932]");
        Check("buffer pointer arg is hex, int arg decimal", recs[3].A.Length == 2 && recs[3].A[0].StartsWith("0x") && recs[3].A[1] == "32", string.Join(",", recs[3].A));
        GC.KeepAlive(keep);
    }

    // The request-file protocol with the emulator DLL (steam_api\Managers\IfaceTracerHost.cs): the host's field extraction is
    // repeated here verbatim, and a fake poller plays the host.
    private static string HostField(string json, string name)
    {
        var m = System.Text.RegularExpressions.Regex.Match(json, "\"" + name + "\"\\s*:\\s*\"((?:[^\"\\\\]|\\\\.)*)\"");
        return m.Success ? System.Text.RegularExpressions.Regex.Unescape(m.Groups[1].Value) : string.Empty;
    }

    private static void Request()
    {
        var dir = Path.Combine(Path.GetTempPath(), "skynet-iface-selftest-req-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        var file = Path.Combine(dir, "attach-request.json");
        var cap = Path.Combine(dir, "cap dir");   // backslashes and spaces must survive the JSON round trip
        Directory.CreateDirectory(cap);

        IfaceHookMain.WriteRequest(file, 4242, cap, "SteamFriends017,SteamUser023", "A,B", "");
        var json = File.ReadAllText(file);
        Check("request written atomically (no temp file left)", Directory.GetFiles(dir, "*.tmp").Length == 0 && File.Exists(file));
        Eq("request dir round trip", HostField(json, "dir"), cap);
        Eq("request interfaces", HostField(json, "interfaces"), "SteamFriends017,SteamUser023");
        Eq("request methods", HostField(json, "methods"), "A,B");
        Check("request pid", System.Text.RegularExpressions.Regex.Match(json, "\"pid\"\\s*:\\s*(\\d+)").Groups[1].Value == "4242");
        IfaceHookMain.WriteRequest(file, 1, cap, "x", "", "");
        Eq("a new request replaces the old one", HostField(File.ReadAllText(file), "interfaces"), "x");
        File.Delete(file);

        // Fake poller: takes the request (as the host does), then writes meta waiting -> ok.
        var iface = Path.Combine(cap, "iface.jsonl");
        string? taken = null;
        var poller = new Thread(() =>
        {
            for (var i = 0; i < 100 && taken == null; i++)
            {
                Thread.Sleep(50);
                if (!File.Exists(file)) continue;
                taken = File.ReadAllText(file);
                File.Delete(file);
                File.WriteAllText(iface, "{\"k\":\"meta\",\"status\":\"waiting\"}\n");
                Thread.Sleep(600);
                File.AppendAllText(iface, "{\"k\":\"meta\",\"status\":\"ok\"}\n");
            }
        });
        poller.Start();
        var old = Console.Out; var sw = new StringWriter(); Console.SetOut(sw);
        int code;
        try { code = IfaceHookMain.ViaEmulator(file, 40, 4242, cap, "SteamFriends017", "", "", ",\"interfaces\":[\"SteamFriends017\"]"); } finally { Console.SetOut(old); }
        poller.Join();
        Check("via-emulator waits for status ok and reports", code == 0 && sw.ToString().Contains("\"status\":\"ok\"") && taken != null && HostField(taken, "dir") == cap, sw + " / " + taken);
        Eq("LastStatus reads the last meta", IfaceHookMain.LastStatus(iface), "ok");

        // Nobody picks the request up: clear error, exit 2, and the stale request is removed.
        File.Delete(iface);
        var olde = Console.Error; var se = new StringWriter(); Console.SetError(se);
        try { code = IfaceHookMain.ViaEmulator(file, 2, 4242, cap, "SteamFriends017", "", "", ""); } finally { Console.SetError(olde); }
        Check("no poller: clear error, exit 2, stale request removed", code == 2 && se.ToString().Contains("emulator did not pick up the request") && !File.Exists(file), se.ToString());
        Directory.Delete(dir, true);
    }
}