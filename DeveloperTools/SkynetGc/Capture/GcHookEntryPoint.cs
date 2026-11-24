using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using EasyHook;

namespace SKYNET.Gc.Capture;

/// <summary>
/// Injected into dota2.exe (real Steam). Hooks the three methods of the real
/// ISteamGameCoordinator and appends every GC message, already decrypted by Steam, to a
/// JSON-lines file. Nothing else Steam does is recorded.
/// Record: {"t":ms,"dir":"send|recv","type":N,"proto":bool,"size":N,"b64":"..."}
/// where b64 is the exact buffer handed to / received from the interface
/// (8-byte or 18-byte GC header + protobuf body).
/// </summary>
public sealed class GcHookEntryPoint : IEntryPoint
{
    private readonly string _logPath;
    private readonly object _lock = new object();
    private readonly DateTime _start = DateTime.UtcNow;
    private readonly List<LocalHook> _hooks = new List<LocalHook>();
    private SendDelegate? _origSend;
    private RetrieveDelegate? _origRetrieve;
    private bool _hooked;

    public GcHookEntryPoint(RemoteHooking.IContext context, string logPath) { _logPath = logPath; }

    public void Run(RemoteHooking.IContext context, string logPath)
    {
        Log("meta", 0, 0, IntPtr.Zero, 0, "hook-loaded");
        RemoteHooking.WakeUpProcess();

        // steam_api64 loads and initialises well after process start; poll until the GC
        // interface can be resolved, then patch its three methods once.
        while (true)
        {
            try
            {
                if (!_hooked) TryHook();
            }
            catch (Exception ex) { Log("meta", 0, 0, IntPtr.Zero, 0, "hook-error: " + ex.Message); }
            Thread.Sleep(250);
        }
    }

    private void TryHook()
    {
        var api = GetModuleHandle("steam_api64.dll");
        if (api == IntPtr.Zero) return;
        var getUser = GetProcAddress(api, "SteamAPI_GetHSteamUser");
        var find = GetProcAddress(api, "SteamInternal_FindOrCreateUserInterface");
        if (getUser == IntPtr.Zero || find == IntPtr.Zero) return;

        var user = Marshal.GetDelegateForFunctionPointer<GetUserDelegate>(getUser)();
        if (user == 0) return;
        var iface = Marshal.GetDelegateForFunctionPointer<FindDelegate>(find)(user, "SteamGameCoordinator001");
        if (iface == IntPtr.Zero) return;

        var vtable = Marshal.ReadIntPtr(iface);
        var pSend = Marshal.ReadIntPtr(vtable, 0 * IntPtr.Size);
        var pRetrieve = Marshal.ReadIntPtr(vtable, 2 * IntPtr.Size);

        _origSend = Marshal.GetDelegateForFunctionPointer<SendDelegate>(pSend);
        _origRetrieve = Marshal.GetDelegateForFunctionPointer<RetrieveDelegate>(pRetrieve);

        var hSend = LocalHook.Create(pSend, new SendDelegate(SendDetour), this);
        hSend.ThreadACL.SetExclusiveACL(new int[0]);
        var hRecv = LocalHook.Create(pRetrieve, new RetrieveDelegate(RetrieveDetour), this);
        hRecv.ThreadACL.SetExclusiveACL(new int[0]);
        _hooks.Add(hSend);
        _hooks.Add(hRecv);
        _hooked = true;
        Log("meta", 0, 0, IntPtr.Zero, 0, "gc-interface-hooked");
    }

    private int SendDetour(IntPtr self, uint type, IntPtr data, uint size)
    {
        try { Log("send", type & 0x7FFFFFFF, (type & 0x80000000) != 0 ? 1 : 0, data, size, null); } catch { }
        return _origSend!(self, type, data, size);
    }

    private int RetrieveDetour(IntPtr self, ref uint type, IntPtr dest, uint destSize, ref uint size)
    {
        var result = _origRetrieve!(self, ref type, dest, destSize, ref size);
        if (result == 0)
        {
            try { Log("recv", type & 0x7FFFFFFF, (type & 0x80000000) != 0 ? 1 : 0, dest, size, null); } catch { }
        }
        return result;
    }

    private void Log(string dir, uint type, int proto, IntPtr data, uint size, string? note)
    {
        var sb = new StringBuilder(128 + (int)size * 2);
        sb.Append("{\"t\":").Append((long)(DateTime.UtcNow - _start).TotalMilliseconds)
          .Append(",\"dir\":\"").Append(dir).Append("\",\"type\":").Append(type)
          .Append(",\"proto\":").Append(proto == 1 ? "true" : "false")
          .Append(",\"size\":").Append(size);
        if (data != IntPtr.Zero && size > 0)
        {
            var bytes = new byte[size];
            Marshal.Copy(data, bytes, 0, (int)size);
            sb.Append(",\"b64\":\"").Append(Convert.ToBase64String(bytes)).Append('"');
        }
        if (note != null) sb.Append(",\"note\":\"").Append(note.Replace("\\", "\\\\").Replace("\"", "'")).Append('"');
        sb.Append("}\n");
        lock (_lock)
        {
            using (var fs = new FileStream(_logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                var b = Encoding.UTF8.GetBytes(sb.ToString());
                fs.Write(b, 0, b.Length);
            }
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] private delegate int GetUserDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)] private delegate IntPtr FindDelegate(int user, string version);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate int SendDelegate(IntPtr self, uint type, IntPtr data, uint size);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] private delegate int RetrieveDelegate(IntPtr self, ref uint type, IntPtr dest, uint destSize, ref uint size);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr GetModuleHandle(string name);
    [DllImport("kernel32.dll", CharSet = CharSet.Ansi)] private static extern IntPtr GetProcAddress(IntPtr module, string name);
}


