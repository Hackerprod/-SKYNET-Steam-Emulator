using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Callback
{
    internal static class CallbackBaseInvoker
    {
        // MSVC reverses these two overloads in the vtable. This is the same
        // overload-order rule used by MsvcVTableOverloadAttribute elsewhere.
        private const int RunCallResultVTableSlot = 0;
        private const int RunCallbackVTableSlot = 1;
        private const int GetCallbackSizeVTableSlot = 2;

        private static readonly object Gate = new object();
        private static readonly Dictionary<IntPtr, RunCallbackDelegate> RunCallbackDelegates = new Dictionary<IntPtr, RunCallbackDelegate>();
        private static readonly Dictionary<IntPtr, RunCallResultDelegate> RunCallResultDelegates = new Dictionary<IntPtr, RunCallResultDelegate>();
        private static readonly Dictionary<IntPtr, GetCallbackSizeDelegate> GetCallbackSizeDelegates = new Dictionary<IntPtr, GetCallbackSizeDelegate>();
        private static readonly Dictionary<IntPtr, RunCallbackThisCallDelegate> RunCallbackThisCallDelegates = new Dictionary<IntPtr, RunCallbackThisCallDelegate>();
        private static readonly Dictionary<IntPtr, RunCallResultThisCallDelegate> RunCallResultThisCallDelegates = new Dictionary<IntPtr, RunCallResultThisCallDelegate>();
        private static readonly Dictionary<IntPtr, GetCallbackSizeThisCallDelegate> GetCallbackSizeThisCallDelegates = new Dictionary<IntPtr, GetCallbackSizeThisCallDelegate>();

        public static bool RunCallback(IntPtr self, IntPtr pvParam)
        {
            if (!TryGetVTableFunction(self, RunCallbackVTableSlot, out var function))
            {
                return false;
            }

            if (IntPtr.Size == 4)
            {
                GetDelegate(function, RunCallbackThisCallDelegates)(self, pvParam);
            }
            else
            {
                GetDelegate(function, RunCallbackDelegates)(self, pvParam);
            }
            return true;
        }

        public static bool RunCallResult(IntPtr self, IntPtr pvParam, bool ioFailure, SteamAPICall_t apiCall)
        {
            if (!TryGetVTableFunction(self, RunCallResultVTableSlot, out var function))
            {
                return false;
            }

            if (IntPtr.Size == 4)
            {
                GetDelegate(function, RunCallResultThisCallDelegates)(self, pvParam, ioFailure ? (byte)1 : (byte)0, apiCall);
            }
            else
            {
                GetDelegate(function, RunCallResultDelegates)(self, pvParam, ioFailure ? (byte)1 : (byte)0, apiCall);
            }
            return true;
        }

        public static int GetCallbackSizeBytes(IntPtr self)
        {
            if (!TryGetVTableFunction(self, GetCallbackSizeVTableSlot, out var function))
            {
                return 0;
            }

            return IntPtr.Size == 4
                ? GetDelegate(function, GetCallbackSizeThisCallDelegates)(self)
                : GetDelegate(function, GetCallbackSizeDelegates)(self);
        }

        public static int GetCallbackId(IntPtr self)
        {
            if (!IsSupportedCallbackPointer(self))
            {
                return 0;
            }

            return Marshal.ReadInt32(self, IntPtr.Size + 4);
        }

        public static byte GetCallbackFlags(IntPtr self)
        {
            if (!IsSupportedCallbackPointer(self))
            {
                return 0;
            }

            return Marshal.ReadByte(self, IntPtr.Size);
        }

        public static void RegisterCallback(IntPtr self, int callbackId)
        {
            if (!IsSupportedCallbackPointer(self) || IntPtr.Size == 4)
            {
                return;
            }

            var flags = (byte)(GetCallbackFlags(self) | CallbackConstants.Registered);
            Marshal.WriteByte(self, IntPtr.Size, flags);
            Marshal.WriteInt32(self, IntPtr.Size + 4, callbackId);
        }

        public static void UnregisterCallback(IntPtr self)
        {
            if (!IsSupportedCallbackPointer(self) || IntPtr.Size == 4)
            {
                return;
            }

            var flags = (byte)(GetCallbackFlags(self) & ~CallbackConstants.Registered);
            Marshal.WriteByte(self, IntPtr.Size, flags);
        }

        private static bool TryGetVTableFunction(IntPtr self, int slot, out IntPtr function)
        {
            function = IntPtr.Zero;
            if (!IsSupportedCallbackPointer(self))
            {
                return false;
            }

            var vtable = Marshal.ReadIntPtr(self);
            if (vtable == IntPtr.Zero)
            {
                return false;
            }

            function = Marshal.ReadIntPtr(vtable, slot * IntPtr.Size);
            return function != IntPtr.Zero;
        }

        private static bool IsSupportedCallbackPointer(IntPtr self)
        {
            if (self == IntPtr.Zero)
            {
                return false;
            }

            return IntPtr.Size == 4 || IntPtr.Size == 8;
        }

        private static TDelegate GetDelegate<TDelegate>(IntPtr function, Dictionary<IntPtr, TDelegate> cache)
            where TDelegate : class
        {
            lock (Gate)
            {
                if (!cache.TryGetValue(function, out var invoker))
                {
                    invoker = (TDelegate)(object)Marshal.GetDelegateForFunctionPointer(function, typeof(TDelegate));
                    cache[function] = invoker;
                }

                return invoker;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void RunCallbackDelegate(IntPtr self, IntPtr pvParam);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void RunCallResultDelegate(
            IntPtr self,
            IntPtr pvParam,
            byte ioFailure,
            SteamAPICall_t apiCall);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int GetCallbackSizeDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void RunCallbackThisCallDelegate(IntPtr self, IntPtr pvParam);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void RunCallResultThisCallDelegate(
            IntPtr self,
            IntPtr pvParam,
            byte ioFailure,
            SteamAPICall_t apiCall);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate int GetCallbackSizeThisCallDelegate(IntPtr self);
    }
}
