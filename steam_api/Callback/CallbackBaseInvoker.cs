using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Callback
{
    internal static class CallbackBaseInvoker
    {
        private const int RunCallResultVTableSlot = 0;
        private const int RunCallbackVTableSlot = 1;
        private const int GetCallbackSizeVTableSlot = 2;
        private const int CallbackFlagsOffset64 = 8;
        private const int CallbackIdOffset64 = 12;

        private static readonly object Gate = new object();
        private static readonly Dictionary<IntPtr, RunCallbackDelegate> RunCallbackDelegates = new Dictionary<IntPtr, RunCallbackDelegate>();
        private static readonly Dictionary<IntPtr, RunCallResultDelegate> RunCallResultDelegates = new Dictionary<IntPtr, RunCallResultDelegate>();
        private static readonly Dictionary<IntPtr, GetCallbackSizeDelegate> GetCallbackSizeDelegates = new Dictionary<IntPtr, GetCallbackSizeDelegate>();
        private static bool unsupportedArchitectureLogged;

        public static bool RunCallback(IntPtr self, IntPtr pvParam)
        {
            if (!TryGetVTableFunction(self, RunCallbackVTableSlot, out var function))
            {
                return false;
            }

            var invoker = GetDelegate(function, RunCallbackDelegates);
            invoker(self, pvParam);
            return true;
        }

        public static bool RunCallResult(IntPtr self, IntPtr pvParam, bool ioFailure, SteamAPICall_t apiCall)
        {
            if (!TryGetVTableFunction(self, RunCallResultVTableSlot, out var function))
            {
                return false;
            }

            var invoker = GetDelegate(function, RunCallResultDelegates);
            invoker(self, pvParam, ioFailure ? (byte)1 : (byte)0, apiCall);
            return true;
        }

        public static int GetCallbackSizeBytes(IntPtr self)
        {
            if (!TryGetVTableFunction(self, GetCallbackSizeVTableSlot, out var function))
            {
                return 0;
            }

            var invoker = GetDelegate(function, GetCallbackSizeDelegates);
            return invoker(self);
        }

        public static int GetCallbackId(IntPtr self)
        {
            if (!IsSupportedCallbackPointer(self))
            {
                return 0;
            }

            return Marshal.ReadInt32(self, CallbackIdOffset64);
        }

        public static byte GetCallbackFlags(IntPtr self)
        {
            if (!IsSupportedCallbackPointer(self))
            {
                return 0;
            }

            return Marshal.ReadByte(self, CallbackFlagsOffset64);
        }

        public static void RegisterCallback(IntPtr self, int callbackId)
        {
            if (!IsSupportedCallbackPointer(self))
            {
                return;
            }

            var flags = (byte)(GetCallbackFlags(self) | CallbackConstants.Registered);
            Marshal.WriteByte(self, CallbackFlagsOffset64, flags);
            Marshal.WriteInt32(self, CallbackIdOffset64, callbackId);
        }

        public static void UnregisterCallback(IntPtr self)
        {
            if (!IsSupportedCallbackPointer(self))
            {
                return;
            }

            var flags = (byte)(GetCallbackFlags(self) & ~CallbackConstants.Registered);
            Marshal.WriteByte(self, CallbackFlagsOffset64, flags);
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

            if (IntPtr.Size == 8)
            {
                return true;
            }

            if (!unsupportedArchitectureLogged)
            {
                unsupportedArchitectureLogged = true;
                SteamEmulator.Write("CallbackManager", "CCallbackBase direct vtable dispatch is only enabled for x64");
            }

            return false;
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
    }
}
