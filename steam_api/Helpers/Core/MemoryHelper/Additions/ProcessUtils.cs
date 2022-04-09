using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using static NativeSharp.NativeMethods;

namespace NativeSharp {
	[Flags]
	public enum LoadModuleFlags : uint {
		/// <summary />
		AsDatafile = 0x00000002,
		/// <summary />
		WithAlteredSearchPath = 0x00000008,
		/// <summary />
		IgnoreCodeAuthzLevel = 0x00000010,
		/// <summary />
		AsImageResource = 0x00000020,
		/// <summary />
		AsDatafileExclusive = 0x00000040,
		/// <summary />
		RequireSignedTarget = 0x00000080,
		/// <summary />
		SearchDllLoadDir = 0x00000100,
		/// <summary />
		SearchApplicationDir = 0x00000200,
		/// <summary />
		SearchUserDirs = 0x00000400,
		/// <summary />
		SearchSystem32 = 0x00000800,
		/// <summary />
		SearchDefaultDirs = 0x00001000
	}

	unsafe partial class NativeProcess
    {
		public static IEnumerable<uint> GetProcessIdsByName(string processName)
        {
			if (string.IsNullOrEmpty(processName))
				throw new ArgumentNullException(nameof(processName));

			foreach (uint processId in GetAllProcessIds()) {
				using var process = Open(processId, ProcessAccess.QueryInformation);
				if (process == InvalidProcess)
					continue;
				if (string.Equals(process.Name, processName, StringComparison.OrdinalIgnoreCase))
					yield return processId;
			}
		}

		public static uint[] GetAllProcessIds()
        {
			uint[]? buffer = null;
			uint bytesReturned = 0;
			do {
				if (buffer is null)
					buffer = new uint[0x200];
				else
					buffer = new uint[buffer.Length * 2];
				fixed (uint* p = buffer) {
					if (!EnumProcesses(p, (uint)(buffer.Length * 4), out bytesReturned))
						return default;
				}
			} while (bytesReturned == buffer.Length * 4);
			uint[] processIds = new uint[bytesReturned / 4];
			for (int i = 0; i < processIds.Length; i++)
				processIds[i] = buffer[i];
			return processIds;
		}

		public static NativeModule LoadModule(string modulePath)
        {
			return LoadModule(modulePath, 0);
		}
		public static NativeModule LoadModule(string modulePath, LoadModuleFlags flags)
        {
			if (string.IsNullOrEmpty(modulePath))
				throw new ArgumentNullException(nameof(modulePath));

			return CurrentProcess.UnsafeGetModule(LoadLibraryEx(modulePath, null, (uint)flags));
		}

		internal static bool Is64BitProcessInternal(void* processHandle, out bool is64Bit)
        {
			is64Bit = false;
			if (!NativeEnvironment.Is64BitOperatingSystem)
				return true;
			if (!IsWow64Process(processHandle, out bool isWow64))
				return false;
			is64Bit = !isWow64;
			return true;
		}

		internal static void* GetModuleHandleInternal(void* processHandle, bool first, string moduleName)
        {
			void* moduleHandle;
			if (!EnumProcessModulesEx(processHandle, &moduleHandle, (uint)IntPtr.Size, out uint size, LIST_MODULES_ALL))
				return null;
			if (first)
				return moduleHandle;
			void*[] moduleHandles = new void*[size / (uint)IntPtr.Size];
			fixed (void** p = moduleHandles) {
				if (!EnumProcessModulesEx(processHandle, p, size, out _, LIST_MODULES_ALL))
					return null;
			}
			var moduleNameBuffer = new StringBuilder((int)MAX_MODULE_NAME32);
			for (int i = 0; i < moduleHandles.Length; i++) {
				if (!GetModuleBaseName(processHandle, moduleHandles[i], moduleNameBuffer, MAX_MODULE_NAME32))
					return null;
				if (moduleNameBuffer.ToString().Equals(moduleName, StringComparison.OrdinalIgnoreCase))
					return moduleHandles[i];
			}
			return null;
		}

		internal static void ThrowWin32ExceptionIfFalse(bool result)
        {
			if (!result)
				throw new Win32Exception();
		}
	}
}
