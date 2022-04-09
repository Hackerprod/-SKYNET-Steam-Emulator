using System;
using System.Collections.Generic;
using System.Text;
using static NativeSharp.NativeMethods;

namespace NativeSharp {
	/// <summary>
	/// 导出函数信息
	/// </summary>
	public sealed unsafe class ExportFunctionInfo {
		/// <summary>
		/// 表示一个空的实例
		/// </summary>
		public readonly static ExportFunctionInfo Empty = new ExportFunctionInfo(null, string.Empty, 0);

		private readonly void* _address;
		private readonly string _name;
		private readonly ushort _ordinal;

		/// <summary>
		/// 地址
		/// </summary>
		public void* Address => _address;

		/// <summary>
		/// 名称
		/// </summary>
		public string Name => _name;

		/// <summary>
		/// 序号
		/// </summary>
		public ushort Ordinal => _ordinal;

		/// <summary>
		/// 构造器
		/// </summary>
		/// <param name="address">地址</param>
		/// <param name="name">名称</param>
		/// <param name="ordinal">序号</param>
		public ExportFunctionInfo(void* address, string name, ushort ordinal) {
			_address = address;
			_name = name ?? string.Empty;
			_ordinal = ordinal;
		}
	}

	unsafe partial class NativeModule
    {

		public void* GetFunctionAddress(string functionName)
        {
			_process.QuickDemand(ProcessAccess.MemoryRead | ProcessAccess.QueryInformation);
			return GetFunctionAddressInternal(_process.Handle, _handle, functionName);
		}

		internal static void* GetFunctionAddressInternal(void* processHandle, string moduleName, string functionName)
        {
			void* moduleHandle = NativeProcess.GetModuleHandleInternal(processHandle, false, moduleName);
			if (moduleHandle == null)
				return null;
			return GetFunctionAddressInternal(processHandle, moduleHandle, functionName);
		}

		internal static void* GetFunctionAddressInternal(void* processHandle, void* moduleHandle, string functionName)
        {
			if (!SafeGetExportTableInfo((IntPtr)processHandle, (IntPtr)moduleHandle, out var ied, out uint[] nameOffsets))
				return null;
			for (uint i = 0; i < ied.NumberOfNames; i++) {
				if (!NativeProcess.ReadStringInternal(processHandle, (byte*)moduleHandle + nameOffsets[i], out string name, false, Encoding.ASCII) || name != functionName)
					continue;
				if (!NativeProcess.ReadUInt16Internal(processHandle, (byte*)moduleHandle + ied.AddressOfNameOrdinals + (i * 2), out ushort ordinal))
					continue;
				if (!NativeProcess.ReadUInt32Internal(processHandle, (byte*)moduleHandle + ied.AddressOfFunctions + (ordinal * 4), out uint addressOffset))
					continue;
				return (byte*)moduleHandle + addressOffset;
			}
			return null;
		}

		public IEnumerable<ExportFunctionInfo> EnumerateFunctionInfos()
        {
			_process.QuickDemand(ProcessAccess.MemoryRead | ProcessAccess.QueryInformation);
			return EnumerateFunctionInfosInternal((IntPtr)_process.Handle, (IntPtr)_handle);
		}

		internal static IEnumerable<ExportFunctionInfo> EnumerateFunctionInfosInternal(IntPtr processHandle, IntPtr moduleHandle)
        {
			if (!SafeGetExportTableInfo(processHandle, moduleHandle, out var ied, out uint[] nameOffsets))
				yield break;
			for (uint i = 0; i < ied.NumberOfNames; i++)
            {
				if (SafeGetExportFunctionInfo(processHandle, moduleHandle, ied, nameOffsets, i, out var functionInfo))
					yield return functionInfo;
				else
					yield break;
			}
		}

		private static bool SafeGetExportTableInfo(IntPtr processHandle, IntPtr moduleHandle, out IMAGE_EXPORT_DIRECTORY ied, out uint[] nameOffsets)
        {
			ied = default;
			nameOffsets = default;
			if (!NativeProcess.ReadUInt32Internal((void*)processHandle, (byte*)moduleHandle + 0x3C, out uint ntHeaderOffset))
				return false;
			if (!NativeProcess.Is64BitProcessInternal((void*)processHandle, out bool is64Bit))
				return false;
			uint iedRVA;
			if (is64Bit) {
				if (!NativeProcess.ReadUInt32Internal((void*)processHandle, (byte*)moduleHandle + ntHeaderOffset + 0x88, out iedRVA))
					return false;
			}
			else {
				if (!NativeProcess.ReadUInt32Internal((void*)processHandle, (byte*)moduleHandle + ntHeaderOffset + 0x78, out iedRVA))
					return false;
			}
			fixed (void* p = &ied) {
				if (!NativeProcess.ReadInternal((void*)processHandle, (byte*)moduleHandle + iedRVA, p, IMAGE_EXPORT_DIRECTORY.UnmanagedSize))
					return false;
			}
			if (ied.NumberOfNames == 0)
				return true;
			nameOffsets = new uint[ied.NumberOfNames];
			fixed (void* p = nameOffsets) {
				if (!NativeProcess.ReadInternal((void*)processHandle, (byte*)moduleHandle + ied.AddressOfNames, p, ied.NumberOfNames * 4))
					return false;
			}
			return true;
		}

		private static bool SafeGetExportFunctionInfo(IntPtr processHandle, IntPtr moduleHandle, IMAGE_EXPORT_DIRECTORY ied, uint[] nameOffsets, uint i, out ExportFunctionInfo functionInfo)
        {
			functionInfo = ExportFunctionInfo.Empty;
			if (!NativeProcess.ReadStringInternal((void*)processHandle, (byte*)moduleHandle + nameOffsets[i], out string functionName, false, Encoding.ASCII))
				return false;
			if (!NativeProcess.ReadUInt16Internal((void*)processHandle, (byte*)moduleHandle + ied.AddressOfNameOrdinals + (i * 2), out ushort ordinal))
				return false;
			if (!NativeProcess.ReadUInt32Internal((void*)processHandle, (byte*)moduleHandle + ied.AddressOfFunctions + (ordinal * 4), out uint addressOffset))
				return false;
			functionInfo = new ExportFunctionInfo((byte*)moduleHandle + addressOffset, functionName, ordinal);
			return true;
		}
	}
}
