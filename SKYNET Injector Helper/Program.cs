using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SKYNET.InjectorHelper;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length != 4)
                throw new ArgumentException("Expected four base64-encoded arguments: exe, DLL, command line, working directory.");
            if (IntPtr.Size != 4)
                throw new InvalidOperationException("The x86 injector helper is not running as a 32-bit process.");

            var exePath = Decode(args[0]);
            var dllPath = Decode(args[1]);
            var arguments = Decode(args[2]);
            var workingDirectory = Decode(args[3]);
            var processId = LaunchAndInject(exePath, dllPath, arguments, workingDirectory);
            Console.Out.WriteLine($"OK {processId}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static string Decode(string value)
    {
        if (string.IsNullOrEmpty(value) || value[0] != 'B')
            throw new FormatException("The injector helper received an invalid encoded argument.");

        return Encoding.UTF8.GetString(Convert.FromBase64String(value.Substring(1)));
    }

    private static uint LaunchAndInject(string exePath, string dllPath, string arguments, string workingDirectory)
    {
        if (!File.Exists(exePath)) throw new FileNotFoundException("Executable not found", exePath);
        if (!File.Exists(dllPath)) throw new FileNotFoundException("Injection DLL not found", dllPath);

        var startupInfo = new STARTUPINFO
        {
            cb = Marshal.SizeOf<STARTUPINFO>(),
            dwFlags = STARTF_USESHOWWINDOW,
            wShowWindow = SW_SHOWNORMAL
        };
        var commandLineText = $"\"{exePath}\" {arguments}".Trim();
        var commandLine = new StringBuilder(commandLineText, Math.Max(commandLineText.Length + 1, 260));
        if (!CreateProcess(exePath, commandLine, IntPtr.Zero, IntPtr.Zero, false, CREATE_SUSPENDED,
                IntPtr.Zero, workingDirectory, ref startupInfo, out var processInfo))
            throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateProcess failed in the x86 injector helper");

        try
        {
            Inject(processInfo.hProcess, dllPath);
            AllowSetForegroundWindow(processInfo.dwProcessId);
            if (ResumeThread(processInfo.hThread) == unchecked((uint)-1))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "ResumeThread failed in the x86 injector helper");
            return processInfo.dwProcessId;
        }
        catch
        {
            TerminateProcess(processInfo.hProcess, 1);
            throw;
        }
        finally
        {
            if (processInfo.hThread != IntPtr.Zero) CloseHandle(processInfo.hThread);
            if (processInfo.hProcess != IntPtr.Zero) CloseHandle(processInfo.hProcess);
        }
    }

    private static void Inject(IntPtr processHandle, string dllPath)
    {
        var kernel = GetModuleHandle("kernel32.dll");
        var loadLibrary = GetProcAddress(kernel, "LoadLibraryW");
        if (loadLibrary == IntPtr.Zero)
            throw new Win32Exception(Marshal.GetLastWin32Error(), "GetProcAddress(LoadLibraryW) failed in the x86 injector helper");

        var pathBytes = Encoding.Unicode.GetBytes(Path.GetFullPath(dllPath) + "\0");
        var remotePath = VirtualAllocEx(processHandle, IntPtr.Zero, (uint)pathBytes.Length, MEM_COMMIT_RESERVE, PAGE_READWRITE);
        if (remotePath == IntPtr.Zero)
            throw new Win32Exception(Marshal.GetLastWin32Error(), "VirtualAllocEx failed in the x86 injector helper");

        try
        {
            if (!WriteProcessMemory(processHandle, remotePath, pathBytes, (uint)pathBytes.Length, out var written) ||
                written.ToInt64() != pathBytes.Length)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "WriteProcessMemory failed in the x86 injector helper");

            var thread = CreateRemoteThread(processHandle, IntPtr.Zero, 0, loadLibrary, remotePath, 0, out _);
            if (thread == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateRemoteThread failed in the x86 injector helper");

            try
            {
                var wait = WaitForSingleObject(thread, INJECTION_TIMEOUT_MS);
                if (wait == WAIT_TIMEOUT)
                    throw new TimeoutException("Timed out while loading the x86 payload into the target process.");
                if (wait != WAIT_OBJECT_0)
                    throw new Win32Exception(Marshal.GetLastWin32Error(), $"Waiting for the x86 injection thread failed (wait=0x{wait:X8})");
                if (!GetExitCodeThread(thread, out var moduleHandle) || moduleHandle == 0)
                    throw new InvalidOperationException("LoadLibraryW in the x86 target returned NULL (injection failed).");
            }
            finally
            {
                CloseHandle(thread);
            }
        }
        finally
        {
            VirtualFreeEx(processHandle, remotePath, 0, MEM_RELEASE);
        }
    }

    private const uint CREATE_SUSPENDED = 0x00000004;
    private const uint MEM_COMMIT_RESERVE = 0x3000;
    private const uint MEM_RELEASE = 0x8000;
    private const uint PAGE_READWRITE = 0x04;
    private const uint WAIT_OBJECT_0 = 0;
    private const uint WAIT_TIMEOUT = 0x00000102;
    private const uint INJECTION_TIMEOUT_MS = 15000;
    private const int STARTF_USESHOWWINDOW = 0x00000001;
    private const short SW_SHOWNORMAL = 1;

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
        public int dwX, dwY, dwXSize, dwYSize, dwXCountChars, dwYCountChars, dwFillAttribute, dwFlags;
        public short wShowWindow, cbReserved2;
        public IntPtr lpReserved2, hStdInput, hStdOutput, hStdError;
    }

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CreateProcess(string lpApplicationName, StringBuilder lpCommandLine,
        IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags,
        IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

    [DllImport("kernel32.dll", SetLastError = true)] private static extern uint ResumeThread(IntPtr hThread);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern bool TerminateProcess(IntPtr hProcess, uint exitCode);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr address, uint size, uint allocationType, uint protect);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr address, uint size, uint freeType);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr address, byte[] buffer, uint size, out IntPtr written);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr attributes, uint stackSize, IntPtr startAddress, IntPtr parameter, uint flags, out uint threadId);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern uint WaitForSingleObject(IntPtr handle, uint milliseconds);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern bool GetExitCodeThread(IntPtr thread, out uint exitCode);
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)] private static extern IntPtr GetProcAddress(IntPtr module, string name);
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)] private static extern IntPtr GetModuleHandle(string moduleName);
    [DllImport("kernel32.dll", SetLastError = true)] private static extern bool CloseHandle(IntPtr handle);
    [DllImport("user32.dll", SetLastError = true)] private static extern bool AllowSetForegroundWindow(uint processId);
}
