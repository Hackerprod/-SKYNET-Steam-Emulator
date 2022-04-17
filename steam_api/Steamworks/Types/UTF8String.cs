#region Usings

using System;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace SKYNET.Steamworks.Types
{
    public class Utf8String : SafeHandle
    {
        public Utf8String(string c_sourceString) : base(IntPtr.Zero, true)
        {
            if (c_sourceString == null)
            {
                SetHandle(IntPtr.Zero);
                return;
            }

            var a_byteArray = new byte[Encoding.UTF8.GetByteCount(c_sourceString) + 1];

            Encoding.UTF8.GetBytes(c_sourceString, 0, c_sourceString.Length, a_byteArray, 0);

            var a_byteBuffer = Marshal.AllocHGlobal(a_byteArray.Length);

            Marshal.Copy(a_byteArray, 0, a_byteBuffer, a_byteArray.Length);

            SetHandle(a_byteBuffer);
        }

        #region Static Methods

        public static string ToString(IntPtr c_nativeStringPtr)
        {
            if (c_nativeStringPtr == IntPtr.Zero)
            {
                return string.Empty;
            }

            var a_length = 0;

            while (Marshal.ReadByte(c_nativeStringPtr, a_length) != 0)
            {
                ++a_length;
            }

            if (a_length == 0)
            {
                return string.Empty;
            }

            var a_buffer = new byte[a_length];

            Marshal.Copy(c_nativeStringPtr, a_buffer, 0, a_buffer.Length);

            return Encoding.UTF8.GetString(a_buffer);
        }

        #endregion

        #region Overrides of SafeHandle

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                Marshal.FreeHGlobal(handle);
            }

            return true;
        }

        public override bool IsInvalid { get; }

        #endregion
    }
}