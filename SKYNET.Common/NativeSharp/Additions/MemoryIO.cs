using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static NativeSharp.NativeMethods;

namespace NativeSharp
{
    public unsafe sealed class Pointer
    {
        private string _moduleName;
        private void* _baseAddress;
        private readonly List<uint> _offsets;

        public string ModuleName
        {
            get => _moduleName;
            set => _moduleName = value;
        }

        public void* BaseAddress
        {
            get => _baseAddress;
            set => _baseAddress = value;
        }
        public IList<uint> Offsets => _offsets;
        public Pointer()
        {
            _moduleName = string.Empty;
            _offsets = new List<uint>();
        }

        public Pointer(string moduleName, params uint[] offsets)
        {
            _moduleName = moduleName;
            _offsets = new List<uint>(offsets);
        }

        public Pointer(void* baseAddress, params uint[] offsets)
        {
            _moduleName = string.Empty;
            _baseAddress = baseAddress;
            _offsets = new List<uint>(offsets);
        }

        public Pointer(Pointer pointer)
        {
            _moduleName = pointer._moduleName;
            _baseAddress = pointer._baseAddress;
            _offsets = new List<uint>(pointer._offsets);
        }
    }

    unsafe partial class NativeProcess
    {
        #region pointer
        public void* ToAddress(Pointer pointer)
        {
            if (pointer is null)
                throw new ArgumentNullException(nameof(pointer));

            QuickDemand(ProcessAccess.MemoryRead);
            ThrowWin32ExceptionIfFalse(ToAddressInternal(_handle, pointer, out void* address));
            return address;
        }

        public bool TryToAddress(Pointer pointer, out void* address)
        {
            address = default;
            if (pointer is null)
                return false;

            if (!QuickDemandNoThrow(ProcessAccess.MemoryRead))
                return false;
            return ToAddressInternal(_handle, pointer, out address);
        }
        #endregion

        #region read
        public byte ReadByte(void* address)
        {
            QuickDemand(ProcessAccess.MemoryRead);
            ThrowWin32ExceptionIfFalse(ReadByteInternal(_handle, address, out byte value));
            return value;
        }

        public short ReadInt16(void* address)
        {
            QuickDemand(ProcessAccess.MemoryRead);
            ThrowWin32ExceptionIfFalse(ReadInt16Internal(_handle, address, out short value));
            return value;
        }

        public ushort ReadUInt16(void* address)
        {
            QuickDemand(ProcessAccess.MemoryRead);
            ThrowWin32ExceptionIfFalse(ReadUInt16Internal(_handle, address, out ushort value));
            return value;
        }

        public int ReadInt32(void* address)
        {
            QuickDemand(ProcessAccess.MemoryRead);
            ThrowWin32ExceptionIfFalse(ReadInt32Internal(_handle, address, out int value));
            return value;
        }

        public uint ReadUInt32(void* address)
        {
            QuickDemand(ProcessAccess.MemoryRead);
            ThrowWin32ExceptionIfFalse(ReadUInt32Internal(_handle, address, out uint value));
            return value;
        }

        public long ReadInt64(void* address)
        {
            QuickDemand(ProcessAccess.MemoryRead);
            ThrowWin32ExceptionIfFalse(ReadInt64Internal(_handle, address, out long value));
            return value;
        }

        public ulong ReadUInt64(void* address)
        {
            QuickDemand(ProcessAccess.MemoryRead);
            ThrowWin32ExceptionIfFalse(ReadUInt64Internal(_handle, address, out ulong value));
            return value;
        }

        public IntPtr ReadIntPtr(void* address)
        {
            QuickDemand(ProcessAccess.MemoryRead);
            ThrowWin32ExceptionIfFalse(ReadIntPtrInternal(_handle, address, out var value));
            return value;
        }

        public UIntPtr ReadUIntPtr(void* address)
        {
            QuickDemand(ProcessAccess.MemoryRead);
            ThrowWin32ExceptionIfFalse(ReadUIntPtrInternal(_handle, address, out var value));
            return value;
        }

        public float ReadSingle(void* address)
        {
            QuickDemand(ProcessAccess.MemoryRead);
            ThrowWin32ExceptionIfFalse(ReadSingleInternal(_handle, address, out float value));
            return value;
        }

        public double ReadDouble(void* address)
        {
            QuickDemand(ProcessAccess.MemoryRead);
            ThrowWin32ExceptionIfFalse(ReadDoubleInternal(_handle, address, out double value));
            return value;
        }

        public void ReadBytes(void* address, byte[] value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            ReadBytes(address, value, 0, (uint)value.Length);
        }
        public void ReadBytes(void* address, byte[] value, uint startIndex, uint length)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            if (startIndex > value.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (startIndex + length > value.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            QuickDemand(ProcessAccess.MemoryRead);
            ThrowWin32ExceptionIfFalse(ReadBytesInternal(_handle, address, value, startIndex, length));
        }
        public string ReadString(void* address, bool isEndWithDoubleZero, Encoding fromEncoding)
        {
            if (fromEncoding is null)
                throw new ArgumentNullException(nameof(fromEncoding));

            QuickDemand(ProcessAccess.MemoryRead);
            ThrowWin32ExceptionIfFalse(ReadStringInternal(_handle, address, out string value, isEndWithDoubleZero, fromEncoding));
            return value;
        }
        public bool TryReadByte(void* address, out byte value)
        {
            value = default;
            if (!QuickDemandNoThrow(ProcessAccess.MemoryRead))
                return false;
            return ReadByteInternal(_handle, address, out value);
        }
        public bool TryReadInt16(void* address, out short value)
        {
            value = default;
            if (!QuickDemandNoThrow(ProcessAccess.MemoryRead))
                return false;
            return ReadInt16Internal(_handle, address, out value);
        }
        public bool TryReadUInt16(void* address, out ushort value)
        {
            value = default;
            if (!QuickDemandNoThrow(ProcessAccess.MemoryRead))
                return false;
            return ReadUInt16Internal(_handle, address, out value);
        }
        public bool TryReadInt32(void* address, out int value)
        {
            value = default;
            if (!QuickDemandNoThrow(ProcessAccess.MemoryRead))
                return false;
            return ReadInt32Internal(_handle, address, out value);
        }
        public bool TryReadUInt32(void* address, out uint value)
        {
            value = default;
            if (!QuickDemandNoThrow(ProcessAccess.MemoryRead))
                return false;
            return ReadUInt32Internal(_handle, address, out value);
        }
        public bool TryReadInt64(void* address, out long value)
        {
            value = default;
            if (!QuickDemandNoThrow(ProcessAccess.MemoryRead))
                return false;
            return ReadInt64Internal(_handle, address, out value);
        }
        public bool TryReadUInt64(void* address, out ulong value)
        {
            value = default;
            if (!QuickDemandNoThrow(ProcessAccess.MemoryRead))
                return false;
            return ReadUInt64Internal(_handle, address, out value);
        }

        public bool TryReadIntPtr(void* address, out IntPtr value)
        {
            value = default;
            if (!QuickDemandNoThrow(ProcessAccess.MemoryRead))
                return false;
            return ReadIntPtrInternal(_handle, address, out value);
        }

        public bool TryReadUIntPtr(void* address, out UIntPtr value)
        {
            value = default;
            if (!QuickDemandNoThrow(ProcessAccess.MemoryRead))
                return false;
            return ReadUIntPtrInternal(_handle, address, out value);
        }

        public bool TryReadSingle(void* address, out float value)
        {
            value = default;
            if (!QuickDemandNoThrow(ProcessAccess.MemoryRead))
                return false;
            return ReadSingleInternal(_handle, address, out value);
        }

        public bool TryReadDouble(void* address, out double value)
        {
            value = default;
            if (!QuickDemandNoThrow(ProcessAccess.MemoryRead))
                return false;
            return ReadDoubleInternal(_handle, address, out value);
        }

        public bool TryReadBytes(void* address, byte[] value)
        {
            if (value is null)
                return false;

            return TryReadBytes(address, value, 0, (uint)value.Length);
        }

        public bool TryReadBytes(void* address, byte[] value, uint startIndex, uint length)
        {
            if (value is null)
                return false;
            if (startIndex > value.Length)
                return false;
            if (startIndex + length > value.Length)
                return false;

            if (!QuickDemandNoThrow(ProcessAccess.MemoryRead))
                return false;
            return ReadBytesInternal(_handle, address, value, startIndex, length);
        }

        public bool TryReadString(void* address, out string value, bool isEndWithDoubleZero, Encoding fromEncoding)
        {
            value = string.Empty;
            if (fromEncoding is null)
                return false;

            if (!QuickDemandNoThrow(ProcessAccess.MemoryRead))
                return false;
            return ReadStringInternal(_handle, address, out value, isEndWithDoubleZero, fromEncoding);
        }
        #endregion



        #region pointer impl
        internal static bool ToAddressInternal(void* processHandle, Pointer pointer, out void* address)
        {
            if (!Is64BitProcessInternal(processHandle, out bool is64Bit))
            {
                address = default;
                return false;
            }
            return is64Bit ? ToAddressPrivate64(processHandle, pointer, out address) : ToAddressPrivate32(processHandle, pointer, out address);
        }

        private static bool ToAddressPrivate32(void* processHandle, Pointer pointer, out void* address)
        {
            address = default;
            if (pointer.BaseAddress == null)
            {
                if (string.IsNullOrEmpty(pointer.ModuleName))
                    throw new ArgumentNullException(nameof(Pointer.ModuleName));
                pointer.BaseAddress = GetModuleHandleInternal(processHandle, false, pointer.ModuleName);
            }
            if (pointer.BaseAddress == null)
                throw new ArgumentNullException(nameof(Pointer.BaseAddress));
            uint newAddress = (uint)pointer.BaseAddress;
            var offsets = pointer.Offsets;
            if (offsets.Count > 0)
            {
                for (int i = 0; i < offsets.Count - 1; i++)
                {
                    newAddress += offsets[i];
                    if (!ReadUInt32Internal(processHandle, (void*)newAddress, out newAddress))
                        return false;
                }
                newAddress += offsets[offsets.Count - 1];
            }
            address = (void*)newAddress;
            return true;
        }

        private static bool ToAddressPrivate64(void* processHandle, Pointer pointer, out void* address)
        {
            address = default;
            if (pointer.BaseAddress == null)
            {
                if (string.IsNullOrEmpty(pointer.ModuleName))
                    throw new ArgumentNullException(nameof(Pointer.ModuleName));
                pointer.BaseAddress = GetModuleHandleInternal(processHandle, false, pointer.ModuleName);
            }
            if (pointer.BaseAddress == null)
                throw new ArgumentNullException(nameof(Pointer.BaseAddress));
            ulong newAddress = (ulong)pointer.BaseAddress;
            var offsets = pointer.Offsets;
            if (offsets.Count > 0)
            {
                for (int i = 0; i < offsets.Count - 1; i++)
                {
                    newAddress += offsets[i];
                    if (!ReadUInt64Internal(processHandle, (void*)newAddress, out newAddress))
                        return false;
                }
                newAddress += offsets[offsets.Count - 1];
            }
            address = (void*)newAddress;
            return true;
        }
        #endregion

        #region read impl
        internal static bool ReadByteInternal(void* processHandle, void* address, out byte value)
        {
            fixed (void* p = &value)
                return ReadInternal(processHandle, address, p, 1);
        }

        internal static bool ReadInt16Internal(void* processHandle, void* address, out short value)
        {
            fixed (void* p = &value)
                return ReadInternal(processHandle, address, p, 2);
        }

        internal static bool ReadUInt16Internal(void* processHandle, void* address, out ushort value)
        {
            fixed (void* p = &value)
                return ReadInternal(processHandle, address, p, 2);
        }

        internal static bool ReadInt32Internal(void* processHandle, void* address, out int value)
        {
            fixed (void* p = &value)
                return ReadInternal(processHandle, address, p, 4);
        }

        internal static bool ReadUInt32Internal(void* processHandle, void* address, out uint value)
        {
            fixed (void* p = &value)
                return ReadInternal(processHandle, address, p, 4);
        }

        internal static bool ReadInt64Internal(void* processHandle, void* address, out long value)
        {
            fixed (void* p = &value)
                return ReadInternal(processHandle, address, p, 8);
        }

        internal static bool ReadUInt64Internal(void* processHandle, void* address, out ulong value)
        {
            fixed (void* p = &value)
                return ReadInternal(processHandle, address, p, 8);
        }

        internal static bool ReadIntPtrInternal(void* processHandle, void* address, out IntPtr value)
        {
            fixed (void* p = &value)
                return ReadInternal(processHandle, address, p, (uint)IntPtr.Size);
        }

        internal static bool ReadUIntPtrInternal(void* processHandle, void* address, out UIntPtr value)
        {
            fixed (void* p = &value)
                return ReadInternal(processHandle, address, p, (uint)UIntPtr.Size);
        }

        internal static bool ReadSingleInternal(void* processHandle, void* address, out float value)
        {
            fixed (void* p = &value)
                return ReadInternal(processHandle, address, p, 4);
        }

        internal static bool ReadDoubleInternal(void* processHandle, void* address, out double value)
        {
            fixed (void* p = &value)
                return ReadInternal(processHandle, address, p, 8);
        }

        internal static bool ReadBytesInternal(void* processHandle, void* address, byte[] value)
        {
            return ReadBytesInternal(processHandle, address, value, 0, (uint)value.Length);
        }

        internal static bool ReadBytesInternal(void* processHandle, void* address, byte[] value, uint startIndex, uint length)
        {
            fixed (void* p = &value[startIndex])
                return ReadInternal(processHandle, address, p, length);
        }

        internal static bool ReadStringInternal(void* processHandle, void* address, out string value, bool isEndWithDoubleZero, Encoding fromEncoding)
        {
            // TODO: 在出现时一些特殊字符可能导致字符串被过早截取！
            const uint BASE_BUFFER_SIZE = 0x100;

            uint dummy;
            if (!ReadInternal(processHandle, address, &dummy, isEndWithDoubleZero ? 2u : 1))
            {
                value = string.Empty;
                return false;
            }
            using var stream = new MemoryStream((int)BASE_BUFFER_SIZE);
            uint bufferSize = BASE_BUFFER_SIZE;
            byte[]? bytes = null;
            bool isLastZero = false;
            do
            {
                byte[] buffer = new byte[bufferSize];
                ReadBytesInternal(processHandle, address, buffer);
                long oldPostion = stream.Position == 0 ? 0 : stream.Position - (isEndWithDoubleZero ? 2 : 1);
                stream.Write(buffer, 0, buffer.Length);
                int length = (int)(stream.Length - oldPostion);
                stream.Position = oldPostion;
                for (int i = 0; i < length; i++)
                {
                    bool isZero = stream.ReadByte() == 0;
                    if ((isEndWithDoubleZero && !isLastZero) || !isZero)
                    {
                        isLastZero = isZero;
                        continue;
                    }
                    bytes = new byte[stream.Position];
                    stream.Position = 0;
                    stream.Read(bytes, 0, bytes.Length);
                    break;
                }
                address = (byte*)address + bufferSize;
                bufferSize += BASE_BUFFER_SIZE;
            } while (bytes is null);
            if (fromEncoding.CodePage != Encoding.Unicode.CodePage)
                bytes = Encoding.Convert(fromEncoding, Encoding.Unicode, bytes);
            fixed (void* p = bytes)
                value = new string((char*)p);
            return true;
        }

        internal static bool ReadInternal(void* processHandle, void* address, void* value, uint length)
        {
            return ReadProcessMemory(processHandle, address, value, length, null);
        }
        #endregion

    }
}
