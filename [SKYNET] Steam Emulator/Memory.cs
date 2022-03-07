using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SKYNET
{
    public class Memory
    {
        public struct STARTUPINFO
        {
            public uint cb;

            public string lpReserved;

            public string lpDesktop;

            public string lpTitle;

            public uint dwX;

            public uint dwY;

            public uint dwXSize;

            public uint dwYSize;

            public uint dwXCountChars;

            public uint dwYCountChars;

            public uint dwFillAttribute;

            public uint dwFlags;

            public short wShowWindow;

            public short cbReserved2;

            public IntPtr lpReserved2;

            public IntPtr hStdInput;

            public IntPtr hStdOutput;

            public IntPtr hStdError;
        }

        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;

            public IntPtr hThread;

            public uint dwProcessId;

            public uint dwThreadId;
        }

        [Flags]
        public enum ProcessCreationFlags : uint
        {
            ZERO_FLAG = 0x0u,
            CREATE_BREAKAWAY_FROM_JOB = 0x1000000u,
            CREATE_DEFAULT_ERROR_MODE = 0x4000000u,
            CREATE_NEW_CONSOLE = 0x10u,
            CREATE_NEW_PROCESS_GROUP = 0x200u,
            CREATE_NO_WINDOW = 0x8000000u,
            CREATE_PROTECTED_PROCESS = 0x40000u,
            CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x2000000u,
            CREATE_SEPARATE_WOW_VDM = 0x1000u,
            CREATE_SHARED_WOW_VDM = 0x1000u,
            CREATE_SUSPENDED = 0x4u,
            CREATE_UNICODE_ENVIRONMENT = 0x400u,
            DEBUG_ONLY_THIS_PROCESS = 0x2u,
            DEBUG_PROCESS = 0x1u,
            DETACHED_PROCESS = 0x8u,
            EXTENDED_STARTUPINFO_PRESENT = 0x80000u,
            INHERIT_PARENT_AFFINITY = 0x10000u
        }

        private const int ProcessCreateThread = 2;

        private const int ProcessQueryInformation = 1024;

        private const int ProcessVMOperation = 8;

        private const int ProcessVMWrite = 32;

        private const int ProcessVMRead = 16;

        private const uint MemoryCommit = 4096u;

        private const uint MemoryReserve = 8192u;

        private const uint PageReadWrite = 4u;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationgFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        public static extern uint ResumeThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        public static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, ProcessCreationFlags dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        public static IntPtr InjectDll(int processId, string dllPath)
        {
            IntPtr procHandle = OpenProcess(1082, bInheritHandle: false, processId);
            IntPtr loadLibraryAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            IntPtr allocateMemoryAdress = VirtualAllocEx(procHandle, IntPtr.Zero, (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char))), 12288u, 4u);
            WriteProcessMemory(procHandle, allocateMemoryAdress, Encoding.Default.GetBytes(dllPath), (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char))), out var _);
            return CreateRemoteThread(procHandle, IntPtr.Zero, 0u, loadLibraryAddress, allocateMemoryAdress, 0u, IntPtr.Zero);
        }

        public static PROCESS_INFORMATION CreateSuspendedProcess(string exePath, string commandLines = "")
        {
            STARTUPINFO si = default(STARTUPINFO);
            PROCESS_INFORMATION pi = default(PROCESS_INFORMATION);
            bool success = CreateProcess(exePath, commandLines, IntPtr.Zero, IntPtr.Zero, bInheritHandles: false, ProcessCreationFlags.CREATE_SUSPENDED, IntPtr.Zero, null, ref si, out pi);
            return pi;
        }

        internal static void WakeUpProcess(IntPtr targetProcess)
        {
            ResumeThread(targetProcess);
        }

        internal static void CreateAndInject(string exePath, string dllPath, string v)
        {
            PROCESS_INFORMATION targetProcess = CreateSuspendedProcess(exePath);
            if (true)
            {
                IntPtr result = InjectDll((int)targetProcess.dwProcessId, dllPath);
                if (result == IntPtr.Zero)
                {
                    //Process.GetProcessById((int)targetProcess.dwProcessId)?.Kill();
                    modCommon.Show("Error injecting dll file");
                    return;
                }
                WakeUpProcess(targetProcess.hThread);
            }
        }
    }
}