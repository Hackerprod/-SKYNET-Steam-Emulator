using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SKYNET.Helper
{
	public static class NativeMethods
	{
        public static IntPtr GetProcAddress(string InModule, string InSymbolName)
        {
            IntPtr moduleHandle = GetModuleHandle(InModule);
            if (moduleHandle == IntPtr.Zero)
            {
                //EntryPoint.Write($"{InModule} is not loaded into the current process.");
                return IntPtr.Zero;
            }
            IntPtr procAddress = GetProcAddress(moduleHandle, InSymbolName);
            if (procAddress == IntPtr.Zero)
            {
                //EntryPoint.Write($"{InSymbolName} does not exist.");
                return IntPtr.Zero;
            }
            return procAddress;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        internal const uint IMAGE_NT_SIGNATURE = 17744u;

		public const uint LIST_MODULES_ALL = 3u;

		public const uint PROCESS_ALL_ACCESS = 2035711u;

		public const short INVALID_HANDLE_VALUE = -1;

		[DllImport("kernel32.dll")]
		internal static extern uint ResumeThread(IntPtr hThread);

		[DllImport("kernel32.dll")]
		internal static extern uint SuspendThread(IntPtr hThread);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
		internal static extern IntPtr LoadLibraryA(string lpFileName);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern IntPtr LoadLibraryW(string lpFileName);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
		internal static extern IntPtr LoadModule(string lpModuleName, IntPtr lpParameterBlock);

		[DllImport("ntdll.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern uint LdrLoadDll(IntPtr pathToFile, IntPtr flags, IntPtr moduleFileName, IntPtr moduleHandle);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
		private unsafe static extern void MoveMemory(void* dst, void* src, int size);
		public unsafe static byte[] ReadBytes(IntPtr address, int count)
		{
			byte[] array = new byte[count];
			byte* ptr = (byte*)(void*)address;
			for (int i = 0; i < count; i++)
			{
				array[i] = ptr[i];
			}
			return array;
		}

		public unsafe static void WriteBytes(IntPtr address, byte[] bytes)
		{
			byte* ptr = (byte*)(void*)address;
			for (int i = 0; i < bytes.Length; i++)
			{
				ptr[i] = bytes[i];
			}
		}

		[DllImport("psapi.dll")]
		public static extern bool EnumProcessModulesEx(IntPtr hProcess, [In][Out][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] IntPtr[] lphModule, int cb, [MarshalAs(UnmanagedType.U4)] out int lpcbNeeded, uint dwFilterFlag);

		[DllImport("psapi.dll")]
		public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In][MarshalAs(UnmanagedType.U4)] int nSize);

		[DllImport("User32.dll")]
		public static extern int GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

		[DllImport("kernel32")]
		public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr hHandle);

	}
}
