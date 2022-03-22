using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Helper
{
    public class MemoryHelper
    {

        #region AddressOf

        /// <summary>
        /// Provides the current address of the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public unsafe static System.IntPtr AddressOf(object obj)
        {
            if (obj == null) return System.IntPtr.Zero;

            System.TypedReference reference = __makeref(obj);

            System.TypedReference* pRef = &reference;

            return (System.IntPtr)pRef; //(&pRef)
        }

        /// <summary>
        /// Provides the current address of the given element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public unsafe static System.IntPtr AddressOf<T>(T t)
        //refember ReferenceTypes are references to the CLRHeader
        //where TOriginal : struct
        {
            System.TypedReference reference = __makeref(t);

            return *(System.IntPtr*)(&reference);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        unsafe static System.IntPtr AddressOfRef<T>(ref T t)
        //refember ReferenceTypes are references to the CLRHeader
        //where TOriginal : struct
        {
            System.TypedReference reference = __makeref(t);

            System.TypedReference* pRef = &reference;

            return (System.IntPtr)pRef; //(&pRef)
        }

        /// <summary>
        /// Returns the unmanaged address of the given array.
        /// </summary>
        /// <param name="array"></param>
        /// <returns><see cref="IntPtr.Zero"/> if null, otherwise the address of the array</returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public unsafe static System.IntPtr AddressOfByteArray(byte[] array)
        {
            if (array == null) return System.IntPtr.Zero;

            fixed (byte* ptr = array)
                return (System.IntPtr)(ptr - 2 * sizeof(void*)); //Todo staticaly determine size of void?
        }

        #endregion

        public static IntPtr MemoryAddress(object obj)
        {
            //return AddressHelper.GetAddress(obj);

            //return AddressOf(obj);

            GCHandle Handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
            return GCHandle.ToIntPtr(Handle);
        }
    }
    public class AddressHelper
    {
        private static object mutualObject;
        private static ObjectReinterpreter reinterpreter;

        static AddressHelper()
        {
            AddressHelper.mutualObject = new object();
            AddressHelper.reinterpreter = new ObjectReinterpreter();
            AddressHelper.reinterpreter.AsObject = new ObjectWrapper();
        }

        public static IntPtr GetAddress(object obj)
        {
            lock (AddressHelper.mutualObject)
            {
                AddressHelper.reinterpreter.AsObject.Object = obj;
                IntPtr address = AddressHelper.reinterpreter.AsIntPtr.Value;
                AddressHelper.reinterpreter.AsObject.Object = null;
                return address;
            }
        }

        public static T GetInstance<T>(IntPtr address)
        {
            lock (AddressHelper.mutualObject)
            {
                AddressHelper.reinterpreter.AsIntPtr.Value = address;
                return (T)AddressHelper.reinterpreter.AsObject.Object;
            }
        }

        // I bet you thought C# was type-safe.
        [StructLayout(LayoutKind.Explicit)]
        private struct ObjectReinterpreter
        {
            [FieldOffset(0)] public ObjectWrapper AsObject;
            [FieldOffset(0)] public IntPtrWrapper AsIntPtr;
        }

        private class ObjectWrapper
        {
            public object Object;
        }

        private class IntPtrWrapper
        {
            public IntPtr Value;
        }
    }

}
