using System;
using System.Text;
using static NativeSharp.NativeMethods;

namespace NativeSharp {
	unsafe partial class NativeModule {
		public string Name {
			get {
				_process.QuickDemand(ProcessAccess.MemoryRead | ProcessAccess.QueryInformation);
				var name = new StringBuilder((int)MAX_MODULE_NAME32);
				return GetModuleBaseName(_process.Handle, _handle, name, MAX_MODULE_NAME32) ? name.ToString() : string.Empty;
			}
		}
		public string ImagePath {
			get {
				_process.QuickDemand(ProcessAccess.MemoryRead | ProcessAccess.QueryInformation);
				var iamgePath = new StringBuilder((int)MAX_PATH);
				return GetModuleFileNameEx(_process.Handle, _handle, iamgePath, MAX_PATH) ? iamgePath.ToString() : string.Empty;
			}
		}

		/// <summary />
		public override string ToString() {
			return $"{Name} (Address: 0x{((IntPtr)Handle).ToString(_process.Is64Bit ? "X16" : "X8")})";
		}
	}
}
