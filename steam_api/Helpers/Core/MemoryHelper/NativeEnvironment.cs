using System;
using static NativeSharp.NativeMethods;

namespace NativeSharp {
	public static unsafe class NativeEnvironment {
		private static readonly bool _is64BitOperatingSystem = GetIs64BitOperatingSystem();

		public static bool Is64BitOperatingSystem => _is64BitOperatingSystem;

		private static bool GetIs64BitOperatingSystem() {
			bool is64BitOperatingSystem;
			if (IntPtr.Size == 8)
				is64BitOperatingSystem = true;
			else
				IsWow64Process(GetCurrentProcess(), out is64BitOperatingSystem);
			return is64BitOperatingSystem;
		}
	}
}
