using System;

namespace NativeSharp {
	public sealed unsafe partial class NativeModule {
		private readonly NativeProcess _process;
		private readonly void* _handle;

		public NativeProcess Process => _process;

		public void* Handle => _handle;

		public bool IsInvalid => _handle == null;

		public NativeModule(NativeProcess process, IntPtr handle) : this(process, (void*)handle) {
		}

		public NativeModule(NativeProcess process, void* handle) {
			if (process is null)
				throw new ArgumentNullException(nameof(process));

			_process = process;
			_handle = handle;
		}
	}
}
