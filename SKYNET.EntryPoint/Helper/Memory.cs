using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Helpers
{
    public class Memory
    {
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

        public static IntPtr CreateInMemoryModule(string library)
        {
            int num = 2;
            int num2 = 1024;
            int num3 = 8;
            int num4 = 32;
            int num5 = 16;
            uint num6 = 4096u;
            uint num7 = 8192u;
            uint flProtect = 4u;
            Process currentProcess = Process.GetCurrentProcess();
            IntPtr hProcess = OpenProcess(num | num2 | num3 | num4 | num5, bInheritHandle: false, currentProcess.Id);
            IntPtr procAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            IntPtr intPtr = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)((library.Length + 1) * Marshal.SizeOf(typeof(char))), num6 | num7, flProtect);
            WriteProcessMemory(hProcess, intPtr, Encoding.Default.GetBytes(library), (uint)((library.Length + 1) * Marshal.SizeOf(typeof(char))), out var _);
            return CreateRemoteThread(hProcess, IntPtr.Zero, 0u, procAddress, intPtr, 0u, IntPtr.Zero);
        }
    }
}
