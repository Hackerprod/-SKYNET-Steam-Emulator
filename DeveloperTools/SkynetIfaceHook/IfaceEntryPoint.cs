using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using EasyHook;

namespace SKYNET.Iface;

/// <summary>
/// Injected into dota2.exe (real Steam or our emulator DLL). Finds ISteamFriends v017 the way the GC hook finds
/// the GC interface, patches the vtable slots of the watched methods and records each call to iface.jsonl
/// until a STOP file appears. See DeveloperTools\SkynetGc\README.md (iface).
/// </summary>
public sealed class IfaceEntryPoint : IEntryPoint
{
    private readonly string _dir, _mode, _emuLogOverride, _specDir;
    private readonly string[] _ifaceNames, _methodNames;
    private TraceWriter _w = null!;
    private Interposer _ip = null!;

    private sealed class IfaceState
    {
        public string Name = "";
        public int Slots;
        public List<string> Patched = new List<string>();
        public int Skipped;
        public string? Error;
    }

    // Meta record fields; a later "meta" line in iface.jsonl replaces an earlier one.
    private string _provider = "unknown", _status = "waiting", _emuLog = "", _emuLogMethod = "";
    private long _emuSize = -1;
    private readonly List<IfaceState> _states = new List<IfaceState>();
    private string _modules = "{}", _exports = "{}";
    private readonly long _q0 = Stopwatch.GetTimestamp();

    public IfaceEntryPoint(RemoteHooking.IContext context, string outDir, string mode, string emuLog, string specDir, string ifaces, string methods)
    {
        _dir = outDir; _mode = mode; _emuLogOverride = emuLog; _specDir = specDir;
        _ifaceNames = ifaces.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        _methodNames = methods.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public void Run(RemoteHooking.IContext context, string outDir, string mode, string emuLog, string specDir, string ifaces, string methods)
    {
        if (_mode == "launch") RemoteHooking.WakeUpProcess();
        try
        {
            Directory.CreateDirectory(_dir);
            _w = new TraceWriter(Path.Combine(_dir, "iface.jsonl"));
            WriteMeta();
            Main();
        }
        catch (Exception ex)
        {
            _status = "error: " + ex.Message;
            // Never leave slots patched behind a dead tracer.
            try { if (_ip != null) { _ip.Stopped = true; RestoreAll(); } } catch { }
            try { WriteMeta(); _w.Flush(); } catch { }
        }
    }

    private string StopFile => Path.Combine(_dir, "STOP");

    private void Main()
    {
        // Same start as GcHookEntryPoint: steam_api64 up and a logged-in user; then the interface objects the game already holds.
        Native.FindDelegate find;
        int user;
        while (true)
        {
            if (File.Exists(StopFile)) { _status = "stopped-before-attach"; Finish(false); return; }
            if (TryUser(out user, out find)) break;
            Thread.Sleep(100);
        }

        _provider = Native.GetModuleHandle("steamclient64.dll") != IntPtr.Zero ? "steam" : "emulator";
        _modules = Modules();
        _exports = Exports();
        _ip = new Interposer(r => _w.Offer(r), line => _w.WriteLine(line));

        var selfCheck = new List<KeyValuePair<IntPtr, Slot>>();
        foreach (var name in _ifaceNames)
        {
            var st = new IfaceState { Name = name };
            _states.Add(st);
            var specPath = Path.Combine(_specDir, name + ".txt");
            if (!File.Exists(specPath)) { st.Error = "no spec file"; continue; }
            var rows = Spec.Load(specPath);
            st.Slots = rows.Count;
            var obj = find(user, name);
            if (obj == IntPtr.Zero) { st.Error = "not available (null)"; continue; }
            var vtable = Marshal.ReadIntPtr(obj);
            // Real Steam: the adapter's vtable must have the slot count the spec says. The emulator has no steamclient64.
            if (_provider == "steam")
            {
                var n = CountSlots(vtable);
                if (n != rows.Count) { st.Error = "slot-count-mismatch: " + n + " != " + rows.Count; continue; }
            }
            var plans = Spec.Select(name, rows, _methodNames);
            foreach (var plan in plans.Where(p => p.Skip != null))
            {
                st.Skipped++;
                _w.WriteLine("{\"k\":\"skip\",\"i\":" + Json.Str(name) + ",\"m\":" + Json.Str(plan.Row.Name) + ",\"why\":" + Json.Str(plan.Skip) + "}");
            }
            Slot? check = null;
            foreach (var plan in plans.Where(p => p.Skip == null))
            {
                var s = _ip.Patch(vtable, plan);
                st.Patched.Add(plan.Row.Name);
                // Observe, never alter: the self-check only calls a read-only getter (Get*/Is*) with no arguments.
                if (check == null && plan.Arity == 1 && plan.Ret != K.Void && (plan.Row.Name.StartsWith("Get", StringComparison.Ordinal) || plan.Row.Name.StartsWith("Is", StringComparison.Ordinal))) check = s;
            }
            if (check != null) selfCheck.Add(new KeyValuePair<IntPtr, Slot>(obj, check));
            else if (st.Patched.Count > 0) _w.WriteLine("{\"k\":\"selfcheck\",\"i\":" + Json.Str(name) + ",\"status\":\"skipped: no read-only Get*/Is* method without arguments\"}");
        }

        if (_states.All(s => s.Patched.Count == 0)) { _status = "nothing-patched: " + string.Join("; ", _states.Select(s => s.Name + " " + (s.Error ?? "no supported methods"))); Finish(false); return; }

        foreach (var sc in selfCheck)
            if (!SelfCheck(sc.Key, sc.Value, out var why))
            {
                RestoreAll();
                _status = "reverted: " + why;
                Finish(false);
                return;
            }

        // The tracer's own acquire and self-check lines are already in the emulator log; the diff window starts here.
        FindEmuLog();
        _emuSize = EmuSize();
        _status = "ok";
        WriteMeta();
        _w.Flush();

        while (!File.Exists(StopFile)) Thread.Sleep(250);   // ends with the process otherwise
        Finish(true);
    }

    private bool TryUser(out int user, out Native.FindDelegate find)
    {
        user = 0; find = null!;
        var api = Native.GetModuleHandle("steam_api64.dll");
        if (api == IntPtr.Zero) return false;
        var getUser = Native.GetProcAddress(api, "SteamAPI_GetHSteamUser");
        var f = Native.GetProcAddress(api, "SteamInternal_FindOrCreateUserInterface");
        if (getUser == IntPtr.Zero || f == IntPtr.Zero) return false;
        user = Marshal.GetDelegateForFunctionPointer<Native.GetUserDelegate>(getUser)();
        find = Marshal.GetDelegateForFunctionPointer<Native.FindDelegate>(f);
        return user != 0;
    }
    // Vtable length = consecutive entries that point into executable memory (a vtable ends where the next RTTI pointer
    // into .rdata begins).
    private static int CountSlots(IntPtr vtable)
    {
        var n = 0;
        while (n < 256)
        {
            var p = Marshal.ReadIntPtr(vtable, n * IntPtr.Size);
            if (p == IntPtr.Zero || !Native.IsExecutable(p)) break;
            n++;
        }
        return n;
    }

    // Plumbing check: the original and the (now patched) slot must agree, and the call through the slot must be recorded.
    private bool SelfCheck(IntPtr iface, Slot s, out string why)
    {
        why = "";
        for (var attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                var viaSlot = Marshal.GetDelegateForFunctionPointer<D4>(Stubs.ThroughSlot(s.Addr));
                var before = _ip.SelfRecorded;
                var r1 = s.Orig4!((long)iface, 0, 0, 0);
                Interposer.SelfCall = true;
                long r2;
                try { r2 = viaSlot((long)iface, 0, 0, 0); } finally { Interposer.SelfCall = false; }
                if ((int)r1 == (int)r2 && _ip.SelfRecorded > before) return true;
                why = s.Plan.Iface + "." + s.Plan.Row.Name + " self-check mismatch " + (int)r1 + " vs " + (int)r2 + ", recorded=" + (_ip.SelfRecorded - before);
            }
            catch (Exception ex) { why = "self-check exception: " + ex.Message; }
            Thread.Sleep(50);
        }
        return false;
    }

    private void RestoreAll() { foreach (var s in _ip.Slots) _ip.Restore(s); }

    private void Finish(bool patched)
    {
        if (!patched) { _w.Drain(); WriteMeta(); _w.Flush(); return; }
        // Size first, then stop recording: every log line before this size was produced while we were watching.
        var sizeAtStop = EmuSize();
        _ip.Stopped = true;     // detours are pass-through from here (also after a chained second attach)
        RestoreAll();
        _w.Drain();
        var calls = _w.Records - _ip.SelfRecorded;
        var grew = _emuSize < 0 || sizeAtStop > _emuSize;
        if (_emuLog.Length > 0 && !grew && calls > 0) _status = "emu-log-unchanged";
        WriteMeta();
        _w.WriteLine("{\"k\":\"stop\",\"q\":" + Stopwatch.GetTimestamp() + ",\"emu_log_size\":" + (sizeAtStop < 0 ? "null" : sizeAtStop.ToString()) +
                     ",\"records\":" + _w.Records + ",\"dropped\":" + _w.Dropped + ",\"faults\":" + _ip.FaultCount + "}");
        _w.Flush();
        Thread.Sleep(1000); // threads may still be inside a thunk; delegates are never freed
    }

    private void FindEmuLog()
    {
        if (_provider != "emulator") return;
        try
        {
            if (_emuLogOverride.Length > 0) { _emuLog = _emuLogOverride; _emuLogMethod = "override"; return; }
            var main = Process.GetCurrentProcess().MainModule!.FileName;
            var folder = Path.Combine(Path.GetDirectoryName(main)!, "SKYNET");
            // Same name rule as steam_api\Helpers\Log.cs: steam_api.log or steam_api.<SKYNET_LOG_SUFFIX>.log.
            var suffix = Environment.GetEnvironmentVariable("SKYNET_LOG_SUFFIX");
            var name = "steam_api.log";
            if (!string.IsNullOrWhiteSpace(suffix))
            {
                foreach (var bad in Path.GetInvalidFileNameChars()) suffix = suffix!.Replace(bad, '_');
                name = "steam_api." + suffix + ".log";
            }
            var exact = Path.Combine(folder, name);
            if (File.Exists(exact)) { _emuLog = exact; _emuLogMethod = "suffix-env"; return; }
            var newest = new DirectoryInfo(folder).GetFiles("steam_api*.log").OrderByDescending(f => f.LastWriteTimeUtc).FirstOrDefault();
            if (newest != null) { _emuLog = newest.FullName; _emuLogMethod = "glob-newest"; }
        }
        catch { }
    }

    private long EmuSize()
    {
        try { return _emuLog.Length > 0 && File.Exists(_emuLog) ? new FileInfo(_emuLog).Length : -1; } catch { return -1; }
    }

    private void WriteMeta()
    {
        var sb = new StringBuilder(1024);
        sb.Append("{\"k\":\"meta\",\"v\":1,\"provider\":").Append(Json.Str(_provider));
        sb.Append(",\"pid\":").Append(Process.GetCurrentProcess().Id);
        sb.Append(",\"qfreq\":").Append(Stopwatch.Frequency).Append(",\"q0\":").Append(_q0);
        sb.Append(",\"wall\":").Append(Json.Str(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")));
        sb.Append(",\"ifaces\":[");
        for (var i = 0; i < _states.Count; i++)
        {
            var s = _states[i];
            sb.Append(i > 0 ? "," : "").Append("{\"i\":").Append(Json.Str(s.Name)).Append(",\"slots\":").Append(s.Slots).Append(",\"patched\":").Append(Json.Arr(s.Patched))
              .Append(",\"skipped\":").Append(s.Skipped).Append(",\"error\":").Append(Json.Str(s.Error)).Append('}');
        }
        sb.Append(']');
        sb.Append(",\"status\":").Append(Json.Str(_status));
        sb.Append(",\"modules\":").Append(_modules).Append(",\"exports\":").Append(_exports);
        sb.Append(",\"emu_log\":").Append(_emuLog.Length > 0 ? Json.Str(_emuLog) : "null");
        sb.Append(",\"emu_log_method\":").Append(_emuLogMethod.Length > 0 ? Json.Str(_emuLogMethod) : "null");
        sb.Append(",\"emu_log_size\":").Append(_emuSize >= 0 ? _emuSize.ToString() : "null").Append('}');
        _w.WriteLine(sb.ToString());
    }

    private static string Modules()
    {
        var sb = new StringBuilder("{");
        var first = true;
        foreach (var name in new[] { "steam_api64.dll", "steamclient64.dll", "dota2.exe" })
        {
            string? text = null;
            try
            {
                foreach (ProcessModule m in Process.GetCurrentProcess().Modules)
                    if (string.Equals(m.ModuleName, name, StringComparison.OrdinalIgnoreCase))
                    { text = m.FileName + " " + FileVersionInfo.GetVersionInfo(m.FileName).FileVersion; break; }
            }
            catch { }
            if (!first) sb.Append(',');
            first = false;
            sb.Append(Json.Str(Path.GetFileNameWithoutExtension(name))).Append(':').Append(Json.Str(text));
        }
        return sb.Append('}').ToString();
    }

    // Export names of the two Steam DLLs; Phase 2 (callbacks) decides from them whether a pump export exists.
    private static string Exports()
    {
        var sb = new StringBuilder("{");
        var first = true;
        foreach (var name in new[] { "steam_api64.dll", "steamclient64.dll" })
        {
            var h = Native.GetModuleHandle(name);
            if (!first) sb.Append(',');
            first = false;
            sb.Append(Json.Str(Path.GetFileNameWithoutExtension(name))).Append(':');
            sb.Append(h == IntPtr.Zero ? "null" : Json.Arr(Native.ExportNames(h)));
        }
        return sb.Append('}').ToString();
    }
}

internal static class Native
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate int GetUserDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)] public delegate IntPtr FindDelegate(int user, string version);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] public static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll", CharSet = CharSet.Ansi)] public static extern IntPtr GetProcAddress(IntPtr module, string name);
    [DllImport("kernel32.dll")] private static extern UIntPtr VirtualQuery(IntPtr addr, out MemInfo info, UIntPtr len);

    [StructLayout(LayoutKind.Sequential)]
    private struct MemInfo { public IntPtr BaseAddress, AllocationBase; public uint AllocationProtect, Pad1; public UIntPtr RegionSize; public uint State, Protect, Type, Pad2; }

    public static bool IsExecutable(IntPtr p)
    {
        if (VirtualQuery(p, out var mi, (UIntPtr)Marshal.SizeOf(typeof(MemInfo))) == UIntPtr.Zero) return false;
        return mi.State == 0x1000 && (mi.Protect & 0xF0) != 0; // MEM_COMMIT and any PAGE_EXECUTE_*
    }

    public static List<string> ExportNames(IntPtr module, int max = 1500)
    {
        var names = new List<string>();
        try
        {
            var nt = Marshal.ReadInt32(module, 0x3C);
            var dirRva = Marshal.ReadInt32(module, nt + 0x88);       // PE32+ DataDirectory[0]
            if (dirRva == 0) return names;
            var dir = IntPtr.Add(module, dirRva);
            var count = Marshal.ReadInt32(dir, 0x18);
            var namesRva = Marshal.ReadInt32(dir, 0x20);
            for (var i = 0; i < count && i < max; i++)
                names.Add(Marshal.PtrToStringAnsi(IntPtr.Add(module, Marshal.ReadInt32(IntPtr.Add(module, namesRva + i * 4)))) ?? "");
        }
        catch { }
        return names;
    }
}
