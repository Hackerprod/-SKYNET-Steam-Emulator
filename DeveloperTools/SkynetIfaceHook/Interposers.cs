using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SKYNET.Iface;

// x64 has one calling convention and every argument of the watched methods is integer-class (this, CSteamID by
// value, int, bool, pointers; a CSteamID return is a hidden pointer in the second register). Passing the
// register values as `long` and forwarding the same values therefore reproduces the call exactly; RAX is
// forwarded unchanged.
[UnmanagedFunctionPointer(CallingConvention.Winapi)] internal delegate long D4(long a, long b, long c, long d);
[UnmanagedFunctionPointer(CallingConvention.Winapi)] internal delegate long D8(long a1, long a2, long a3, long a4, long a5, long a6, long a7, long a8);

/// <summary>Reads that return false instead of raising an access violation inside the game.</summary>
internal static class Mem
{
    [DllImport("kernel32.dll")] private static extern bool ReadProcessMemory(IntPtr h, IntPtr addr, byte[] buf, IntPtr size, out IntPtr read);
    [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentProcess();

    public static byte[]? Read(IntPtr p, int len)
    {
        if (p == IntPtr.Zero) return null;
        var buf = new byte[len];
        return ReadProcessMemory(GetCurrentProcess(), p, buf, (IntPtr)len, out var got) && (long)got == len ? buf : null;
    }

    public static ulong ReadU64(IntPtr p)
    {
        var b = Read(p, 8);
        return b == null ? 0 : BitConverter.ToUInt64(b, 0);
    }

    /// <summary>NUL-terminated UTF-8, capped at 512 bytes, read page by page so a string at a page end is safe.</summary>
    public static string ReadString(IntPtr p, int max = 512)
    {
        if (p == IntPtr.Zero) return "";
        var buf = new byte[max];
        var n = 0;
        while (n < max)
        {
            var a = (long)p + n;
            var chunk = (int)Math.Min(max - n, 4096 - (a & 0xFFF));
            var tmp = new byte[chunk];
            if (!ReadProcessMemory(GetCurrentProcess(), (IntPtr)a, tmp, (IntPtr)chunk, out var got) || (long)got == 0) break;
            var g = (int)(long)got;
            var z = Array.IndexOf(tmp, (byte)0, 0, g);
            var take = z >= 0 ? z : g;
            Buffer.BlockCopy(tmp, 0, buf, n, take);
            n += take;
            if (z >= 0) break;
        }
        return Encoding.UTF8.GetString(buf, 0, n);
    }
}

/// <summary>
/// Tiny native trampolines. The original slot target may itself be a CLR thunk (the emulator's vtable is made of them);
/// Marshal.GetDelegateForFunctionPointer would unwrap such a thunk to its managed delegate and fail the cast, so the
/// original is always called through a native `jmp` stub, which works for any target.
/// </summary>
internal static class Stubs
{
    [DllImport("kernel32.dll")] private static extern IntPtr VirtualAlloc(IntPtr addr, UIntPtr size, uint type, uint prot);
    [DllImport("kernel32.dll")] private static extern bool VirtualProtect(IntPtr addr, UIntPtr size, uint prot, out uint old);

    /// <summary>mov rax, target; jmp rax.</summary>
    public static IntPtr JumpTo(IntPtr target) => Make(0x48, 0xB8, BitConverter.GetBytes((long)target), 0xFF, 0xE0);

    /// <summary>mov rax, slotAddr; mov rax, [rax]; jmp rax: calls whatever the slot holds now.</summary>
    public static IntPtr ThroughSlot(IntPtr slotAddr) => Make(0x48, 0xB8, BitConverter.GetBytes((long)slotAddr), 0x48, 0x8B, 0x00, 0xFF, 0xE0);

    private static IntPtr Make(byte b0, byte b1, byte[] imm, params byte[] tail)
    {
        var code = new List<byte> { b0, b1 };
        code.AddRange(imm);
        code.AddRange(tail);
        var mem = VirtualAlloc(IntPtr.Zero, (UIntPtr)64, 0x3000, 0x04);
        if (mem == IntPtr.Zero) throw new OutOfMemoryException("VirtualAlloc failed for a call stub");
        Marshal.Copy(code.ToArray(), 0, mem, code.Count);
        VirtualProtect(mem, (UIntPtr)64, 0x20, out _); // RW while writing, then execute-read
        return mem;
    }
}

internal sealed class Slot
{
    public Plan Plan = null!;
    public IntPtr Addr, Orig, Thunk;
    public D4? Orig4;
    public D8? Orig8;
    public Delegate Detour = null!;
    public int Faults;
}

/// <summary>Finds the first frame outside the CLR, our own code and Steam: "module+0xRVA" of the game's caller.</summary>
internal static class Caller
{
    [DllImport("ntdll.dll")] private static extern ushort RtlCaptureStackBackTrace(uint skip, uint count, IntPtr[] frames, out uint hash);
    [DllImport("ntdll.dll")] private static extern IntPtr RtlPcToFileHeader(IntPtr pc, out IntPtr baseOfImage);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] private static extern uint GetModuleFileName(IntPtr module, StringBuilder name, uint size);

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<long, string> Names = new System.Collections.Concurrent.ConcurrentDictionary<long, string>();
    private static readonly string[] Skip = { "clr", "coreclr", "mscor", "system", "steam_api", "steamclient", "easyhook", "skynet-iface", "ntdll", "kernel" };

    public static string? Find()
    {
        var frames = new IntPtr[24];
        var n = RtlCaptureStackBackTrace(0, 24, frames, out _);
        for (var i = 0; i < n; i++)
        {
            if (RtlPcToFileHeader(frames[i], out var img) == IntPtr.Zero) continue; // JIT code has no image
            var name = Names.GetOrAdd((long)img, _ =>
            {
                var sb = new StringBuilder(260);
                GetModuleFileName(img, sb, 260);
                return Path.GetFileName(sb.ToString()).ToLowerInvariant();
            });
            if (name.Length == 0 || Skip.Any(s => name.StartsWith(s, StringComparison.Ordinal))) continue;
            return name + "+0x" + ((long)frames[i] - (long)img).ToString("X");
        }
        return null;
    }
}

/// <summary>
/// Vtable interposers. The only patch is one aligned 8-byte data store per slot; no code is touched.
/// Delegates and Slot objects are never freed (a thread may still be inside a thunk after stop).
/// </summary>
internal sealed class Interposer
{
    [DllImport("kernel32.dll")] private static extern bool VirtualProtect(IntPtr addr, UIntPtr size, uint prot, out uint old);

    private static readonly List<object> Keep = new List<object>();   // lives as long as the process
    [ThreadStatic] private static bool _inside;
    [ThreadStatic] public static bool SelfCall;                         // tags the self-check calls

    private readonly Action<Rec> _emit;
    private readonly Action<string> _fault;
    public readonly List<Slot> Slots = new List<Slot>();
    public volatile bool Stopped;
    public int SelfRecorded, FaultCount;

    public Interposer(Action<Rec> emit, Action<string> fault) { _emit = emit; _fault = fault; }

    public Slot Patch(IntPtr vtable, Plan plan)
    {
        var s = new Slot { Plan = plan, Addr = IntPtr.Add(vtable, plan.Row.Index * IntPtr.Size) };
        s.Orig = Marshal.ReadIntPtr(s.Addr);
        if (plan.Arity <= 4)
        {
            s.Orig4 = Marshal.GetDelegateForFunctionPointer<D4>(Stubs.JumpTo(s.Orig));
            s.Detour = new D4((a, b, c, d) => Call(s, new[] { a, b, c, d }, s.Orig4, null));
        }
        else
        {
            s.Orig8 = Marshal.GetDelegateForFunctionPointer<D8>(Stubs.JumpTo(s.Orig));
            s.Detour = new D8((a, b, c, d, e, f, g, h) => Call(s, new[] { a, b, c, d, e, f, g, h }, null, s.Orig8));
        }
        s.Thunk = Marshal.GetFunctionPointerForDelegate(s.Detour);
        Keep.Add(s);
        Write(s.Addr, s.Thunk);
        Slots.Add(s);
        return s;
    }

    private static readonly object WriteLock = new object();   // protect/store/protect must not interleave between threads

    private static void Write(IntPtr addr, IntPtr value)
    {
        lock (WriteLock)
        {
            VirtualProtect(addr, (UIntPtr)IntPtr.Size, 0x04, out var old); // PAGE_READWRITE (Steam's vtable page is read-only)
            Marshal.WriteIntPtr(addr, value);
            VirtualProtect(addr, (UIntPtr)IntPtr.Size, old, out _);
        }
    }

    /// <summary>Restores a slot only if it still holds OUR thunk (something else may have patched it since).</summary>
    public bool Restore(Slot s)
    {
        if (Marshal.ReadIntPtr(s.Addr) != s.Thunk) return false;
        Write(s.Addr, s.Orig);
        return true;
    }

    // `orig` runs outside every try so the game sees exactly what Steam would do; only our own work is guarded.
    private long Call(Slot s, long[] regs, D4? o4, D8? o8)
    {
        if (Stopped || _inside) return o4 != null ? o4(regs[0], regs[1], regs[2], regs[3]) : o8!(regs[0], regs[1], regs[2], regs[3], regs[4], regs[5], regs[6], regs[7]);
        _inside = true;
        Rec? rec = null;
        try
        {
            rec = new Rec { I = s.Plan.Iface, M = s.Plan.Row.Name, Q = Stopwatch.GetTimestamp(), Tid = Environment.CurrentManagedThreadId, Self = SelfCall };
            rec.A = Decoder.Args(s.Plan, regs, out rec.Af);
        }
        catch (Exception ex) { rec = null; Failed(s, ex); }

        long r;
        try { r = o4 != null ? o4(regs[0], regs[1], regs[2], regs[3]) : o8!(regs[0], regs[1], regs[2], regs[3], regs[4], regs[5], regs[6], regs[7]); }
        finally { _inside = false; }

        if (rec != null)
        {
            try
            {
                Decoder.Result(s.Plan, regs, r, rec);
                rec.C = Caller.Find();
                rec.QLast = rec.Q;
                if (rec.Self) Interlocked.Increment(ref SelfRecorded);
                _emit(rec);
            }
            catch (Exception ex) { Failed(s, ex); }
        }
        return r;
    }

    private void Failed(Slot s, Exception ex)
    {
        Interlocked.Increment(ref FaultCount);
        var n = Interlocked.Increment(ref s.Faults);
        // Runs on the game's thread: nothing in here may throw into native code.
        try { if (n == 1 || n == 50) _fault("{\"k\":\"fault\",\"m\":" + Json.Str(s.Plan.Iface + "." + s.Plan.Row.Name + ": " + ex.GetType().Name + ": " + ex.Message) + ",\"n\":" + n + "}"); } catch { }
        try { if (n >= 50) Restore(s); } catch { } // a method that keeps failing to decode stops being observed
    }
}
