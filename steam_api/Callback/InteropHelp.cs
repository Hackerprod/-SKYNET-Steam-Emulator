using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks
{
    public class InteropHelp
    {
        public static void TestIfPlatformSupported()
        {
#if !UNITY_EDITOR && !UNITY_STANDALONE && !STEAMWORKS_WIN && !STEAMWORKS_LIN_OSX
            throw new System.InvalidOperationException("Steamworks functions can only be called on platforms that Steam is available on.");
#endif
        }


        // This continues to exist for both 'out string' and strings returned by Steamworks functions.
        public static string PtrToStringUTF8(IntPtr nativeUtf8)
        {
            if (nativeUtf8 == IntPtr.Zero)
            {
                return null;
            }

            int len = 0;

            while (Marshal.ReadByte(nativeUtf8, len) != 0)
            {
                ++len;
            }

            if (len == 0)
            {
                return string.Empty;
            }

            byte[] buffer = new byte[len];
            Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        public static string ByteArrayToStringUTF8(byte[] buffer)
        {
            int length = 0;
            while (length < buffer.Length && buffer[length] != 0)
            {
                length++;
            }

            return Encoding.UTF8.GetString(buffer, 0, length);
        }

        public static void StringToByteArrayUTF8(string str, byte[] outArrayBuffer, int outArrayBufferSize)
        {
            outArrayBuffer = new byte[outArrayBufferSize];
            int length = Encoding.UTF8.GetBytes(str, 0, str.Length, outArrayBuffer, 0);
            outArrayBuffer[length] = 0;
        }

        // This is for 'const char *' arguments which we need to ensure do not get GC'd while Steam is using them.
        // We can't use an ICustomMarshaler because Unity crashes when a string between 96 and 127 characters long is defined/initialized at the top of class scope...
#if UNITY_EDITOR || UNITY_STANDALONE || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX
		public class UTF8StringHandle : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid {
			public UTF8StringHandle(string str)
				: base(true) {
				if (str == null) {
					SetHandle(IntPtr.Zero);
					return;
				}

				// +1 for '\0'
				byte[] strbuf = new byte[Encoding.UTF8.GetByteCount(str) + 1];
				Encoding.UTF8.GetBytes(str, 0, str.Length, strbuf, 0);
				IntPtr buffer = Marshal.AllocHGlobal(strbuf.Length);
				Marshal.Copy(strbuf, 0, buffer, strbuf.Length);

				SetHandle(buffer);
			}

			protected override bool ReleaseHandle() {
				if (!IsInvalid) {
					Marshal.FreeHGlobal(handle);
				}
				return true;
			}
		}
#else
        public class UTF8StringHandle : IDisposable
        {
            public UTF8StringHandle(string str) { }
            public void Dispose() { }
        }
#endif

    }

}
