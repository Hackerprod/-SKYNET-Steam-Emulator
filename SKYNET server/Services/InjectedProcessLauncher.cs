using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SKYNET_server.Services;

/// <summary>
/// Starts a Windows process suspended, loads a DLL from an explicit payload path,
/// then resumes the process. This keeps launch-time emulator DLL selection out of
/// the target application's install directory: when the target later asks Windows
/// for a module with the same file name, the loader can reuse the already-loaded
/// payload module instead of reading a stale on-disk copy beside the executable.
/// </summary>
internal static class InjectedProcessLauncher
{
    public static Process Launch(
        string executablePath,
        string payloadDllPath,
        IReadOnlyList<string> arguments,
        string workingDirectory,
        IReadOnlyDictionary<string, string> environment,
        bool showWindow)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("DLL payload injection is only supported on Windows.");
        }

        if (!File.Exists(executablePath))
        {
            throw new FileNotFoundException("Executable not found.", executablePath);
        }

        if (!File.Exists(payloadDllPath))
        {
            throw new FileNotFoundException("Injection payload DLL not found.", payloadDllPath);
        }

        var startupInfo = new STARTUPINFO
        {
            cb = Marshal.SizeOf<STARTUPINFO>(),
            dwFlags = STARTF_USESHOWWINDOW,
            wShowWindow = showWindow ? SW_SHOWNORMAL : SW_HIDE
        };
        var processInfo = new PROCESS_INFORMATION();
        var commandLine = new StringBuilder(BuildCommandLine(executablePath, arguments));
        var environmentBlock = BuildEnvironmentBlock(environment);

        const uint createSuspended = 0x00000004;
        const uint createNoWindow = 0x08000000;
        const uint createUnicodeEnvironment = 0x00000400;
        var creationFlags = createSuspended | createUnicodeEnvironment | (showWindow ? 0 : createNoWindow);

        try
        {
            if (!CreateProcess(
                    executablePath,
                    commandLine,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    false,
                    creationFlags,
                    environmentBlock,
                    workingDirectory,
                    ref startupInfo,
                    out processInfo))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateProcess failed.");
            }

            try
            {
                InjectInto(processInfo.hProcess, payloadDllPath);
                AllowSetForegroundWindow(processInfo.dwProcessId);
                if (ResumeThread(processInfo.hThread) == uint.MaxValue)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "ResumeThread failed.");
                }

                return Process.GetProcessById((int)processInfo.dwProcessId);
            }
            catch
            {
                try { TerminateProcess(processInfo.hProcess, 1); } catch { }
                throw;
            }
        }
        finally
        {
            if (environmentBlock != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(environmentBlock);
            }

            if (processInfo.hThread != IntPtr.Zero)
            {
                CloseHandle(processInfo.hThread);
            }

            if (processInfo.hProcess != IntPtr.Zero)
            {
                CloseHandle(processInfo.hProcess);
            }
        }
    }

    public static string BuildCommandLine(string executablePath, IEnumerable<string> arguments)
    {
        var parts = new List<string> { QuoteArgument(executablePath) };
        parts.AddRange(arguments.Select(QuoteArgument));
        return string.Join(" ", parts);
    }

    private static IntPtr BuildEnvironmentBlock(IReadOnlyDictionary<string, string> overrides)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
        {
            if (entry.Key is string key)
            {
                values[key] = entry.Value?.ToString() ?? string.Empty;
            }
        }

        foreach (var (key, value) in overrides)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                values[key] = value ?? string.Empty;
            }
        }

        var block = string.Join(
            '\0',
            values
                .OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)
                .Select(pair => $"{pair.Key}={pair.Value}")) + "\0\0";
        var bytes = Encoding.Unicode.GetBytes(block);
        var pointer = Marshal.AllocHGlobal(bytes.Length);
        Marshal.Copy(bytes, 0, pointer, bytes.Length);
        return pointer;
    }

    private static string QuoteArgument(string argument)
    {
        if (argument.Length == 0)
        {
            return "\"\"";
        }

        if (!argument.Any(static ch => char.IsWhiteSpace(ch) || ch == '"'))
        {
            return argument;
        }

        var builder = new StringBuilder();
        builder.Append('"');
        var backslashes = 0;

        foreach (var character in argument)
        {
            if (character == '\\')
            {
                backslashes++;
                continue;
            }

            if (character == '"')
            {
                builder.Append('\\', backslashes * 2 + 1);
                builder.Append(character);
                backslashes = 0;
                continue;
            }

            if (backslashes > 0)
            {
                builder.Append('\\', backslashes);
                backslashes = 0;
            }

            builder.Append(character);
        }

        if (backslashes > 0)
        {
            builder.Append('\\', backslashes * 2);
        }

        builder.Append('"');
        return builder.ToString();
    }

    private static void InjectInto(IntPtr processHandle, string payloadDllPath)
    {
        var kernel = GetModuleHandle("kernel32.dll");
        var loadLibrary = GetProcAddress(kernel, "LoadLibraryW");
        if (loadLibrary == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "GetProcAddress(LoadLibraryW) failed.");
        }

        var pathBytes = Encoding.Unicode.GetBytes(payloadDllPath + "\0");
        const uint memCommitReserve = 0x3000;
        const uint pageReadWrite = 0x04;
        var remoteMemory = VirtualAllocEx(processHandle, IntPtr.Zero, (uint)pathBytes.Length, memCommitReserve, pageReadWrite);
        if (remoteMemory == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "VirtualAllocEx failed.");
        }

        try
        {
            if (!WriteProcessMemory(processHandle, remoteMemory, pathBytes, (uint)pathBytes.Length, out _))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "WriteProcessMemory failed.");
            }

            var remoteThread = CreateRemoteThread(processHandle, IntPtr.Zero, 0, loadLibrary, remoteMemory, 0, out _);
            if (remoteThread == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateRemoteThread failed.");
            }

            try
            {
                const uint infinite = 0xFFFFFFFF;
                WaitForSingleObject(remoteThread, infinite);
                GetExitCodeThread(remoteThread, out var exitCode);
                if (exitCode == 0)
                {
                    throw new InvalidOperationException("LoadLibraryW in the target returned NULL.");
                }
            }
            finally
            {
                CloseHandle(remoteThread);
            }
        }
        finally
        {
            const uint memRelease = 0x8000;
            VirtualFreeEx(processHandle, remoteMemory, 0, memRelease);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public uint dwProcessId;
        public uint dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct STARTUPINFO
    {
        public int cb;
        public string? lpReserved;
        public string? lpDesktop;
        public string? lpTitle;
        public int dwX;
        public int dwY;
        public int dwXSize;
        public int dwYSize;
        public int dwXCountChars;
        public int dwYCountChars;
        public int dwFillAttribute;
        public int dwFlags;
        public short wShowWindow;
        public short cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }

    private const int STARTF_USESHOWWINDOW = 0x00000001;
    private const short SW_HIDE = 0;
    private const short SW_SHOWNORMAL = 1;

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CreateProcess(
        string? lpApplicationName,
        StringBuilder lpCommandLine,
        IntPtr lpProcessAttributes,
        IntPtr lpThreadAttributes,
        bool bInheritHandles,
        uint dwCreationFlags,
        IntPtr lpEnvironment,
        string? lpCurrentDirectory,
        ref STARTUPINFO lpStartupInfo,
        out PROCESS_INFORMATION lpProcessInformation);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint ResumeThread(IntPtr hThread);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool TerminateProcess(IntPtr hProcess, uint exitCode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, out IntPtr bytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr threadAttributes, uint stackSize, IntPtr startAddress, IntPtr parameter, uint creationFlags, out uint threadId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint WaitForSingleObject(IntPtr handle, uint milliseconds);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetExitCodeThread(IntPtr thread, out uint exitCode);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern IntPtr GetProcAddress(IntPtr module, string procedureName);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr GetModuleHandle(string moduleName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr handle);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool AllowSetForegroundWindow(uint processId);
}
