using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SKYNET.Helpers
{
    public static class IntPtrExtentions
    {
        public static string GetUnicodeString(this IntPtr ptr)
        {
            STRING uni = Marshal.PtrToStructure<STRING>(ptr);
            return uni.Content;
        }

        public struct STRING
        {
            public ushort Length;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string Content;
        }

        public unsafe static byte[] ReadBytes(this IntPtr address, int count)
        {
            byte[] array = new byte[count];
            byte* ptr = (byte*)(void*)address;
            for (int i = 0; i < count; i++)
            {
                array[i] = ptr[i];
            }
            return array;
        }

        public static IntPtr GetPtr(this byte[] buffer)
        {
            GCHandle gCHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            return gCHandle.AddrOfPinnedObject();
        }

        public static byte[] GetBytes(this IntPtr buffer, uint length)
        {
            return GetBytes(buffer, (int)length);
        }

        public static byte[] GetBytes(this IntPtr buffer, int length)
        {
            byte[] array = new byte[length];
            Marshal.Copy(buffer, array, 0, length);
            return array;
        }

        public static T[] ToTypedArray<T>(this Array input)
        {
            return input?.Cast<T>().ToArray();
        }
        public static IEnumerable<TAttr> GetCustomAttributes<TAttr>(this Type type, bool inherit = false, Func<TAttr, bool> predicate = null) where TAttr : Attribute
        {
            return type.GetCustomAttributes(typeof(TAttr), inherit).Cast<TAttr>().Where(predicate ?? ((Func<TAttr, bool>)((TAttr a) => true)));
        }

        public static bool IsBlittable(this Type T)
        {
            if ((object)T == null)
            {
                return false;
            }
            while (T.IsArray)
            {
                T = T.GetElementType();
            }
            if (T.IsEnum)
            {
                return true;
            }
            try
            {
                Marshal.SizeOf(T);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static object ToType(this IntPtr ptr, Type t)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }
            return Marshal.PtrToStructure(ptr, t);
        }

        internal static T ToType<T>(this IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return default(T);
            }
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }
    }
}
