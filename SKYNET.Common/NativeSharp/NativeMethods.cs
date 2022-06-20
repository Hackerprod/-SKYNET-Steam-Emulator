using System.Runtime.InteropServices;
using System.Text;

namespace NativeSharp {
	internal static unsafe class NativeMethods {
		public const uint STANDARD_RIGHTS_REQUIRED = 0xF0000;
		public const uint LIST_MODULES_ALL = 0x3;
		public const uint MAX_MODULE_NAME32 = 255;
		public const uint MAX_PATH = 260;
		public const uint INFINITE = 0xFFFFFFFF;

		[StructLayout(LayoutKind.Sequential)]
		public struct MEMORY_BASIC_INFORMATION {
			public static readonly uint UnmanagedSize = (uint)sizeof(MEMORY_BASIC_INFORMATION);

			public void* BaseAddress;
			public void* AllocationBase;
			public uint AllocationProtect;
			public void* RegionSize;
			public uint State;
			public uint Protect;
			public uint Type;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct IMAGE_EXPORT_DIRECTORY {
			public static readonly uint UnmanagedSize = (uint)sizeof(IMAGE_EXPORT_DIRECTORY);

			public uint Characteristics;
			public uint TimeDateStamp;
			public ushort MajorVersion;
			public ushort MinorVersion;
			public uint Name;
			public uint Base;
			public uint NumberOfFunctions;
			public uint NumberOfNames;
			public uint AddressOfFunctions;
			public uint AddressOfNames;
			public uint AddressOfNameOrdinals;
		}

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern void* OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(void* hObject);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern uint GetCurrentProcessId();

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern uint GetProcessId(void* Process);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern void* GetCurrentProcess();

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWow64Process(void* hProcess, [MarshalAs(UnmanagedType.Bool)] out bool Wow64Process);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ReadProcessMemory(void* hProcess, void* lpBaseAddress, void* lpBuffer, uint nSize, uint* lpNumberOfBytesRead);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool WriteProcessMemory(void* hProcess, void* lpBaseAddress, void* lpBuffer, uint nSize, uint* lpNumberOfBytesWritten);

		[DllImport("psapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnumProcesses(uint* pProcessIds, uint cb, out uint pBytesReturned);

		[DllImport("psapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnumProcessModulesEx(void* hProcess, void** lphModule, uint cb, out uint lpcbNeeded, uint dwFilterFlag);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool QueryFullProcessImageName(void* hProcess, uint dwFlags, StringBuilder lpExeName, uint* lpdwSize);

		[DllImport("psapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetModuleBaseName(void* hProcess, void* hModule, StringBuilder lpBaseName, uint nSize);

		[DllImport("psapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetModuleFileNameEx(void* hProcess, void* hModule, StringBuilder lpFilename, uint nSize);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern void* VirtualAllocEx(void* hProcess, void* lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool VirtualFreeEx(void* hProcess, void* lpAddress, uint dwSize, uint dwFreeType);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool VirtualQueryEx(void* hProcess, void* lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool VirtualProtectEx(void* hProcess, void* lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern void* CreateRemoteThread(void* hProcess, void* lpThreadAttributes, uint dwStackSize, void* lpStartAddress, void* lpParameter, uint dwCreationFlags, uint* lpThreadId);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern uint WaitForSingleObject(void* hHandle, uint dwMilliseconds);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetExitCodeThread(void* hThread, out uint lpExitCode);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern void* GetModuleHandle(string? lpModuleName);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern void* LoadLibraryEx(string? lpLibFileName, void* hFile, uint dwFlags);
	}
}
