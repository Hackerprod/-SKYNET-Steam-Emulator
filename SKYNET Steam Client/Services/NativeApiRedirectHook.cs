using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Iced.Intel;
using static Iced.Intel.AssemblerRegisters;

namespace SKYNET.Client.Services;

/// <summary>
/// Hooks LoadLibraryW/LoadLibraryExW inside a suspended x64 target process so that
/// ANY call whose requested file's name (not path - the caller may pass any
/// directory, including its own, e.g. a Unity native plugin loaded from
/// "&lt;Product&gt;_Data\Plugins\x86_64\steam_api64.dll" by absolute path) matches
/// the emulator payload's file name gets redirected to load the payload instead.
///
/// This exists because the bare-name LoadLibrary hijack in <see cref="DllInjector"/>
/// only works when the target later calls LoadLibrary with just the base file name -
/// Windows then reuses the already-injected module by name. Some loaders (Unity's
/// native plugin loader among them) resolve and pass a full absolute path instead,
/// which does not hit that reuse path, so a second, real copy of the target DLL
/// gets loaded from disk and the game never talks to the injected payload at all.
///
/// Nothing on the game's disk is touched - this only patches the target process's
/// own in-memory copy of kernel32.dll and never revisits it after the target exits.
/// </summary>
internal static class NativeApiRedirectHook
{
    private const uint MemCommitReserve = 0x3000;
    private const uint PageExecuteReadWrite = 0x40;

    /// <summary>Bytes needed for the absolute-jump trampoline (mov rax, imm64; jmp rax).</summary>
    private const int TrampolineJumpSize = 12;

    /// <summary>
    /// Allocates executable scratch memory within +/-2GB of <paramref name="anchor"/>,
    /// required for relocating any RIP-relative instruction the target function's
    /// prologue may contain. Windows treats VirtualAllocEx's address as a hint, not
    /// a guarantee, so this tries a spread of candidate addresses on both sides of
    /// the anchor until one succeeds.
    /// </summary>
    private static IntPtr AllocateNear(IntPtr processHandle, IntPtr anchor, int byteCount)
    {
        const long TwoGigabytes = 0x7FFF0000; // stay a little inside +/-2GB
        const long Stride = 0x00400000; // 4MB steps
        var anchorValue = anchor.ToInt64();

        for (var offset = 0L; offset < TwoGigabytes; offset += Stride)
        {
            foreach (var candidate in offset == 0 ? new[] { anchorValue } : new[] { anchorValue + offset, anchorValue - offset })
            {
                if (candidate <= 0)
                    continue;

                var allocation = VirtualAllocEx(processHandle, new IntPtr(candidate), (UIntPtr)byteCount, MemCommitReserve, PageExecuteReadWrite);
                if (allocation != IntPtr.Zero)
                    return allocation;
            }
        }

        throw new Win32Exception(Marshal.GetLastWin32Error(), "Could not allocate scratch memory within +/-2GB of the hooked function.");
    }

    /// <summary>
    /// Installs the redirect hook. Must be called while <paramref name="processHandle"/>
    /// is suspended and after the payload module has already been loaded into it -
    /// this only rewrites the argument LoadLibrary(Ex)W receives, it does not itself
    /// load anything. x64 targets only; the caller must not call this for x86 targets.
    /// </summary>
    public static void Install(IntPtr processHandle, string redirectToAbsolutePath)
    {
        var targetFileName = Path.GetFileName(redirectToAbsolutePath);
        if (string.IsNullOrEmpty(targetFileName))
            throw new ArgumentException("The redirect path has no file name.", nameof(redirectToAbsolutePath));

        var redirectPathBytes = Encoding.Unicode.GetBytes(Path.GetFullPath(redirectToAbsolutePath) + "\0");
        var matchNameBytes = Encoding.Unicode.GetBytes(targetFileName);

        var kernel32 = GetModuleHandle("kernel32.dll");
        if (kernel32 == IntPtr.Zero)
            throw new Win32Exception(Marshal.GetLastWin32Error(), "GetModuleHandle(kernel32.dll) failed.");

        HookFunction(processHandle, kernel32, "LoadLibraryW", matchNameBytes, redirectPathBytes);
        HookFunction(processHandle, kernel32, "LoadLibraryExW", matchNameBytes, redirectPathBytes);
    }

    private static void HookFunction(
        IntPtr processHandle,
        IntPtr kernel32,
        string exportName,
        byte[] matchNameBytes,
        byte[] redirectPathBytes)
    {
        var realAddress = GetProcAddress(kernel32, exportName);
        if (realAddress == IntPtr.Zero)
            throw new Win32Exception(Marshal.GetLastWin32Error(), $"GetProcAddress({exportName}) failed.");

        var (originalInstructions, originalByteLength) = ReadOriginalInstructions(realAddress, TrampolineJumpSize);

        // One scratch block per hooked function: [redirect path][match name][trampoline][detour].
        // The trampoline gets generous headroom: relocated instructions can encode
        // longer than their originals (e.g. a short RIP-relative form needing to
        // become a longer absolute-address form), so this cannot reuse originalByteLength.
        const int trampolineHeadroom = 256;
        var totalSize = redirectPathBytes.Length + matchNameBytes.Length + trampolineHeadroom + 1024;

        // Relocating a RIP-relative instruction only works when its new address is
        // within +/-2GB of where it originally ran (Iced's BlockEncoder enforces
        // this), so the scratch block must be allocated near realAddress rather
        // than wherever Windows would otherwise place an unhinted allocation -
        // same reasoning SteamStaticImportRebinder already applies to its own
        // remote allocation, just centered on a function address instead of an
        // image base.
        var scratch = AllocateNear(processHandle, realAddress, totalSize);

        var redirectPathAddress = scratch;
        var matchNameAddress = IntPtr.Add(redirectPathAddress, redirectPathBytes.Length);
        var trampolineAddress = IntPtr.Add(matchNameAddress, matchNameBytes.Length);
        var detourAddress = IntPtr.Add(trampolineAddress, trampolineHeadroom);

        WriteBytes(processHandle, redirectPathAddress, redirectPathBytes, "redirect path");
        WriteBytes(processHandle, matchNameAddress, matchNameBytes, "match name");

        var trampolineBytes = BuildTrampoline(
            originalInstructions,
            (ulong)trampolineAddress.ToInt64(),
            (ulong)realAddress.ToInt64() + (uint)originalByteLength);
        if (trampolineBytes.Length > trampolineHeadroom)
            throw new InvalidOperationException($"Relocated trampoline for {exportName} ({trampolineBytes.Length} bytes) exceeded its reserved headroom.");
        WriteBytes(processHandle, trampolineAddress, trampolineBytes, "trampoline");

        var detourBytes = BuildDetour(
            (ulong)detourAddress.ToInt64(),
            (ulong)matchNameAddress.ToInt64(),
            matchNameBytes.Length / 2,
            (ulong)redirectPathAddress.ToInt64(),
            (ulong)trampolineAddress.ToInt64());
        WriteBytes(processHandle, detourAddress, detourBytes, "detour");

        var entryJump = BuildAbsoluteJump((ulong)realAddress.ToInt64(), (ulong)detourAddress.ToInt64());
        WriteProtectedBytes(processHandle, realAddress, entryJump, $"{exportName} entry redirect");
    }

    /// <summary>
    /// Decodes whole instructions starting at <paramref name="address"/> until at
    /// least <paramref name="minimumBytes"/> have been covered, so the entry patch
    /// never splits an instruction and the decoded instructions can be safely
    /// relocated (see <see cref="BuildTrampoline"/>) to run on their own elsewhere.
    /// </summary>
    private static (List<Instruction> Instructions, int ByteLength) ReadOriginalInstructions(IntPtr address, int minimumBytes)
    {
        // The launcher process and the target share the same kernel32.dll base
        // address (the existing bare-name LoadLibrary hijack already depends on
        // this), so decoding our own in-process copy is equivalent to decoding
        // the target's copy.
        const int maxScan = 32;
        var buffer = new byte[maxScan];
        Marshal.Copy(address, buffer, 0, maxScan);

        var decoder = Iced.Intel.Decoder.Create(64, new ByteArrayCodeReader(buffer), (ulong)address.ToInt64());
        var instructions = new List<Instruction>();
        var length = 0;
        while (length < minimumBytes)
        {
            var instruction = decoder.Decode();
            if (instruction.IsInvalid)
                throw new InvalidOperationException("Could not decode a safe trampoline boundary (invalid instruction).");
            instructions.Add(instruction);
            length += instruction.Length;
        }

        return (instructions, length);
    }

    /// <summary>
    /// Re-encodes the original instructions at <paramref name="trampolineAddress"/>
    /// (a different address than where they were decoded from) followed by a jump
    /// back to <paramref name="resumeAddress"/> - the real function's own code right
    /// after the bytes the entry patch overwrote. Using <see cref="BlockEncoder"/>
    /// instead of copying raw bytes matters here: some of these instructions can be
    /// RIP-relative (common in modern x64 Windows DLLs), and a raw byte copy would
    /// keep the same displacement while running from a different address, computing
    /// a wrong effective address and crashing the target almost immediately -
    /// BlockEncoder re-derives each relocated instruction's encoding for its new
    /// address instead.
    /// </summary>
    private static byte[] BuildTrampoline(List<Instruction> originalInstructions, ulong trampolineAddress, ulong resumeAddress)
    {
        var jumpBack = new Assembler(64);
        jumpBack.mov(rax, resumeAddress);
        jumpBack.jmp(rax);
        var jumpBackLabel = jumpBack.CreateLabel();
        jumpBack.Label(ref jumpBackLabel);

        var allInstructions = new List<Instruction>(originalInstructions.Count + 4);
        allInstructions.AddRange(originalInstructions);
        allInstructions.AddRange(jumpBack.Instructions);

        var stream = new System.IO.MemoryStream();
        var writer = new StreamCodeWriter(stream);
        var block = new InstructionBlock(writer, allInstructions, trampolineAddress);
        if (!BlockEncoder.TryEncode(64, block, out var errorMessage, out _))
            throw new InvalidOperationException($"BlockEncoder failed to relocate the original instructions: {errorMessage}");

        return stream.ToArray();
    }

    /// <summary>Encodes "mov rax, target; jmp rax", optionally preceded by fixed prefix bytes (used for the trampoline's copied original instructions).</summary>
    private static byte[] BuildAbsoluteJump(ulong siteAddress, ulong targetAddress, byte[]? prefixBytes = null)
    {
        var c = new Assembler(64);
        c.mov(rax, targetAddress);
        c.jmp(rax);
        var jump = Encode(c, siteAddress + (ulong)(prefixBytes?.Length ?? 0));
        if (prefixBytes == null)
            return jump;

        var result = new byte[prefixBytes.Length + jump.Length];
        Array.Copy(prefixBytes, result, prefixBytes.Length);
        Array.Copy(jump, 0, result, prefixBytes.Length, jump.Length);
        return result;
    }

    /// <summary>
    /// Detour body. RCX holds lpLibFileName for both LoadLibraryW (1 arg) and
    /// LoadLibraryExW (3 args, RCX/RDX/R8) - only RCX is ever inspected or
    /// rewritten; RDX/R8 pass through untouched so LoadLibraryExW's flags keep
    /// whatever the caller set. Comparison is a simple bit-6 fold (`or 0x20`) on
    /// both sides of every code unit, applied identically to the stored match name
    /// too, so it's symmetric and only ever needs to be right for the fixed,
    /// known "steam_api(64).dll" name this is always called with.
    /// </summary>
    private static byte[] BuildDetour(
        ulong detourAddress,
        ulong matchNameAddress,
        int matchNameCharCount,
        ulong redirectPathAddress,
        ulong trampolineAddress)
    {
        var c = new Assembler(64);
        var scanLength = c.CreateLabel("scan_length");
        var lengthKnown = c.CreateLabel("length_known");
        var mismatch = c.CreateLabel("mismatch");
        var passthrough = c.CreateLabel("passthrough");

        // r10 = length (in WCHARs) of the string at rcx, found by scanning for the
        // null terminator.
        c.xor(r10, r10);
        c.Label(ref scanLength);
        c.movzx(r9d, __word_ptr[rcx + r10 * 2]);
        c.test(r9d, r9d);
        c.je(lengthKnown);
        c.inc(r10);
        c.jmp(scanLength);

        c.Label(ref lengthKnown);
        c.cmp(r10, matchNameCharCount);
        c.jl(mismatch);

        // r11 = pointer to the start of the candidate suffix: rcx + (length - N) * 2.
        // r10's length value is no longer needed once this is computed, so it is
        // free to reuse below.
        c.mov(r11, r10);
        c.sub(r11, matchNameCharCount);
        c.shl(r11, 1);
        c.add(r11, rcx);

        // r10 = the stored match-name string's address (repurposed - its length
        // value was only needed above). A 64-bit absolute address cannot be used
        // as a bare memory displacement (must fit in 32 bits), so it has to be
        // loaded into a register first, same as rcx/r11.
        //
        // Only rax/r9/r10/r11 are used anywhere in this detour, deliberately: RDX
        // and R8 are LoadLibraryExW's real hFile/dwFlags parameters and must reach
        // the real function untouched, and every other general-purpose register is
        // non-volatile (callee-saved) and would need a push/pop this detour never
        // does since every path below ends in a tail-jump, never a ret.
        c.mov(r10, matchNameAddress);

        for (var i = 0; i < matchNameCharCount; i++)
        {
            c.movzx(eax, __word_ptr[r11 + i * 2]);
            c.or(ax, 0x20);
            c.movzx(r9d, __word_ptr[r10 + i * 2]);
            c.or(r9w, 0x20);
            c.cmp(ax, r9w);
            c.jne(mismatch);
        }

        c.mov(rcx, redirectPathAddress);
        c.Label(ref passthrough);
        c.mov(rax, trampolineAddress);
        c.jmp(rax);

        c.Label(ref mismatch);
        c.jmp(passthrough);

        return Encode(c, detourAddress);
    }

    private static byte[] Encode(Assembler assembler, ulong baseAddress)
    {
        var stream = new System.IO.MemoryStream();
        var writer = new StreamCodeWriter(stream);
        assembler.Assemble(writer, baseAddress);
        return stream.ToArray();
    }

    private static void WriteProtectedBytes(IntPtr processHandle, IntPtr address, byte[] bytes, string operation)
    {
        if (!VirtualProtectEx(processHandle, address, (UIntPtr)bytes.Length, PageExecuteReadWrite, out var previousProtect))
            throw new Win32Exception(Marshal.GetLastWin32Error(), $"VirtualProtectEx failed for {operation}.");
        try
        {
            WriteBytes(processHandle, address, bytes, operation);
        }
        finally
        {
            VirtualProtectEx(processHandle, address, (UIntPtr)bytes.Length, previousProtect, out _);
        }
    }

    private static void WriteBytes(IntPtr processHandle, IntPtr address, byte[] bytes, string operation)
    {
        if (!WriteProcessMemory(processHandle, address, bytes, bytes.Length, out var written) || written.ToInt64() != bytes.Length)
            throw new Win32Exception(Marshal.GetLastWin32Error(), $"WriteProcessMemory failed for {operation}.");
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);
}
