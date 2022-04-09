using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static NativeSharp.NativeMethods;

namespace NativeSharp {
	unsafe partial class NativeProcess
    {
		public bool Is64Bit
        {
			get
            {
				QuickDemand(ProcessAccess.QueryInformation);
				Is64BitProcessInternal(_handle, out bool is64Bit);
				return is64Bit;
			}
		}

		public string Name => Path.GetFileName(ImagePath);

        public string ImagePath {
			get {
				QuickDemand(ProcessAccess.QueryInformation);
				var iamgePath = new StringBuilder((int)MAX_PATH);
				uint size = MAX_PATH;
				return QueryFullProcessImageName(_handle, 0, iamgePath, &size) ? iamgePath.ToString() : string.Empty;
			}
		}

		public List<NativeModule> GetModules()
        {
			QuickDemand(ProcessAccess.QueryInformation);
			void* moduleHandle;
            if (!EnumProcessModulesEx(_handle, &moduleHandle, (uint)IntPtr.Size, out uint size, LIST_MODULES_ALL))
                return new List<NativeModule>(); 

			void*[] moduleHandles = new void*[size / (uint)IntPtr.Size];
			fixed (void** p = moduleHandles)
            {
				if (!EnumProcessModulesEx(_handle, p, size, out _, LIST_MODULES_ALL))
					return new List<NativeModule>();
            }
			var modules = new NativeModule[moduleHandles.Length];
			for (int i = 0; i < modules.Length; i++)
				modules[i] = UnsafeGetModule(moduleHandles[i]);

			return modules.ToList();
		}

		public NativeModule GetMainModule() {
			QuickDemand(ProcessAccess.QueryInformation);
			return UnsafeGetModule(IsCurrentProcess ? GetModuleHandle(null) : GetModuleHandleInternal(_handle, true, string.Empty));
		}

		public NativeModule UnsafeGetModule(void* moduleHandle) {
			return new NativeModule(this, moduleHandle);
		}

		public override string ToString() {
			return $"{Name} (Id: 0x{Id:X})";
		}
	}
}
