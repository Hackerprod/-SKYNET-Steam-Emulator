using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SKYNET.Client.Services;

/// <summary>
/// Launches a process suspended and injects a DLL into it before it runs, using the
/// classic VirtualAllocEx + CreateRemoteThread(LoadLibraryW) technique. Nothing on the
/// game's disk is touched: our steam_api DLL is loaded from the path prepared by
/// GameLauncher, normally a per-build shadow copy of the launcher's payload.
///
/// Why this makes the game use OUR steam_api: the game loads steam_api64.dll
/// dynamically by bare name. Windows' loader returns an already-loaded module with the
/// same base name instead of re-reading disk, so once we've injected our
/// "steam_api64.dll", the game's later LoadLibrary("steam_api64.dll") resolves to ours.
/// </summary>
public static class DllInjector
{
    /// <summary>Launches <paramref name="exePath"/> suspended, injects <paramref name="dllPath"/>, resumes.</summary>
    public static Process LaunchAndInject(string exePath, string dllPath, string arguments, string workingDir)
    {
        if (!File.Exists(exePath)) throw new FileNotFoundException("Executable not found", exePath);
        if (!File.Exists(dllPath)) throw new FileNotFoundException("Injection DLL not found", dllPath);

        var si = new STARTUPINFO
        {
            cb = Marshal.SizeOf<STARTUPINFO>(),
            dwFlags = STARTF_USESHOWWINDOW,
            wShowWindow = SW_SHOWNORMAL
        };
        var pi = new PROCESS_INFORMATION();

        // CreateProcess wants a mutable command line; arg0 should be the exe.
        string cmdLine = $"\"{exePath}\" {arguments}".Trim();
        var cmd = new StringBuilder(cmdLine, Math.Max(cmdLine.Length + 1, 260));

        const uint CREATE_SUSPENDED = 0x00000004;
        if (!CreateProcess(exePath, cmd, IntPtr.Zero, IntPtr.Zero, false,
                CREATE_SUSPENDED, IntPtr.Zero, workingDir, ref si, out pi))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateProcess failed");
        }

        try
        {
            InjectInto(pi.hProcess, dllPath);
            AllowSetForegroundWindow(pi.dwProcessId);
            if (ResumeThread(pi.hThread) == unchecked((uint)-1))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "ResumeThread failed");

            return Process.GetProcessById((int)pi.dwProcessId);
        }
        catch
        {
            try { TerminateProcess(pi.hProcess, 1); } catch { }
            throw;
        }
        finally
        {
            if (pi.hThread != IntPtr.Zero) CloseHandle(pi.hThread);
            if (pi.hProcess != IntPtr.Zero) CloseHandle(pi.hProcess);
        }
    }

    private static void InjectInto(IntPtr hProcess, string dllPath)
    {
        // kernel32 (and thus LoadLibraryW) sits at the same address in every process
        // of the session, so our local address is valid in the remote process.
        IntPtr hKernel = GetModuleHandle("kernel32.dll");
        IntPtr loadLibrary = GetProcAddress(hKernel, "LoadLibraryW");
        if (loadLibrary == IntPtr.Zero)
            throw new Win32Exception(Marshal.GetLastWin32Error(), "GetProcAddress(LoadLibraryW) failed");

        byte[] pathBytes = Encoding.Unicode.GetBytes(dllPath + "\0");
        const uint MEM_COMMIT_RESERVE = 0x3000;
        const uint PAGE_READWRITE = 0x04;

        IntPtr remoteMem = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)pathBytes.Length, MEM_COMMIT_RESERVE, PAGE_READWRITE);
        if (remoteMem == IntPtr.Zero)
            throw new Win32Exception(Marshal.GetLastWin32Error(), "VirtualAllocEx failed");

        try
        {
            if (!WriteProcessMemory(hProcess, remoteMem, pathBytes, (uint)pathBytes.Length, out _))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "WriteProcessMemory failed");

            IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibrary, remoteMem, 0, out _);
            if (hThread == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateRemoteThread failed");

            try
            {
                const uint INFINITE = 0xFFFFFFFF;
                WaitForSingleObject(hThread, INFINITE);
                // LoadLibraryW returns the module handle (nonzero) on success. On x64
                // the 32-bit exit code is truncated, so treat 0 as the only failure.
                GetExitCodeThread(hThread, out uint exit);
                if (exit == 0)
                    throw new InvalidOperationException("LoadLibraryW in the target returned NULL (injection failed).");
            }
            finally
            {
                CloseHandle(hThread);
            }
        }
        finally
        {
            const uint MEM_RELEASE = 0x8000;
            VirtualFreeEx(hProcess, remoteMem, 0, MEM_RELEASE);
        }
    }

    // ================= P/Invoke =================

    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESS_INFORMATION { public IntPtr hProcess; public IntPtr hThread; public uint dwProcessId; public uint dwThreadId; }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct STARTUPINFO
    {
        public int cb; public string? lpReserved; public string? lpDesktop; public string? lpTitle;
        public int dwX, dwY, dwXSize, dwYSize, dwXCountChars, dwYCountChars, dwFillAttribute, dwFlags;
        public short wShowWindow, cbReserved2; public IntPtr lpReserved2, hStdInput, hStdOutput, hStdError;
    }

    private const int STARTF_USESHOWWINDOW = 0x00000001;
    private const short SW_SHOWNORMAL = 1;

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CreateProcess(string? lpApplicationName, StringBuilder lpCommandLine,
        IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags,
        IntPtr lpEnvironment, string? lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint ResumeThread(IntPtr hThread);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize,
        IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out uint lpThreadId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetExitCodeThread(IntPtr hThread, out uint lpExitCode);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool AllowSetForegroundWindow(uint dwProcessId);
}
