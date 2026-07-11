using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Text;

namespace SKYNET.Helpers
{
    internal static class NativeStringCache
    {
        private static readonly ConcurrentDictionary<string, IntPtr> CachedStrings = new ConcurrentDictionary<string, IntPtr>();

        public static IntPtr ToUtf8Ptr(string value)
        {
            value = value ?? string.Empty;
            return CachedStrings.GetOrAdd(value, AllocateUtf8);
        }

        public static IntPtr ToUtf8PtrOrNull(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return IntPtr.Zero;
            }

            return ToUtf8Ptr(value);
        }

        private static IntPtr AllocateUtf8(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            IntPtr ptr = Marshal.AllocHGlobal(bytes.Length + 1);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            Marshal.WriteByte(ptr, bytes.Length, 0);
            return ptr;
        }

        public static bool WriteUtf8Buffer(IntPtr destination, int destinationSize, string value)
        {
            if (destination == IntPtr.Zero || destinationSize <= 0)
            {
                return false;
            }

            value = value ?? string.Empty;
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            int length = Math.Min(bytes.Length, destinationSize - 1);
            if (length > 0)
            {
                Marshal.Copy(bytes, 0, destination, length);
            }

            Marshal.WriteByte(destination, length, 0);
            return true;
        }
    }
}
