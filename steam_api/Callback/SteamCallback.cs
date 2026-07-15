using SKYNET.Helpers;
using SKYNET.Steamworks;
using System;
using System.Runtime.InteropServices;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Callback
{
    public sealed class SteamCallback
    {
        public CallbackType CallbackType { get; set; }
        public CallbackType BaseType => ((int)CallbackType).GetCallbackType();
        public IntPtr Pointer { get; }
        public SteamAPICall_t SteamAPICall { get; set; }
        public bool HasGameserver => (CallbackBaseInvoker.GetCallbackFlags(Pointer) & CallbackConstants.GameServer) != 0;
        public bool HasResult { get; }
        public DateTime Created { get; }

        private bool VTableLogged { get; set; }
        private bool TraceNativeDispatch => (int)CallbackType == 1296;

        public SteamCallback(IntPtr pointer, bool hasResult = false)
            : this(pointer, CallbackBaseInvoker.GetCallbackId(pointer), hasResult)
        {
        }

        public SteamCallback(IntPtr pointer, int iCallback, bool hasResult = false)
        {
            Pointer = pointer;
            HasResult = hasResult;
            Created = DateTime.Now;
            CallbackType = (CallbackType)iCallback;
        }

        public void Run(ICallbackData data)
        {
            IntPtr pvParam = Marshal.AllocHGlobal(data.DataSize);
            try
            {
                Marshal.StructureToPtr(data, pvParam, false);
                Run(pvParam);
            }
            finally
            {
                Marshal.FreeHGlobal(pvParam);
            }
        }

        public void Run(ICallbackData data, bool bIOFailure, ulong hSteamAPICall)
        {
            IntPtr pvParam = Marshal.AllocHGlobal(data.DataSize);
            try
            {
                Marshal.StructureToPtr(data, pvParam, false);

                if (HasResult)
                {
                    Run(pvParam, bIOFailure, hSteamAPICall);
                }
                else
                {
                    Run(pvParam);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pvParam);
            }
        }

        public bool Run(IntPtr pvParam)
        {
            if (Pointer == IntPtr.Zero)
            {
                return false;
            }

            if (TraceNativeDispatch)
            {
                LogNativeState("RunCallback");
            }

            return CallbackBaseInvoker.RunCallback(Pointer, pvParam);
        }

        public bool Run(IntPtr pvParam, bool bIOFailure, ulong hSteamAPICall)
        {
            if (Pointer == IntPtr.Zero)
            {
                return false;
            }

            ulong beforeApiCall = 0;
            if (TraceNativeDispatch)
            {
                LogNativeState("RunCallResult");
                beforeApiCall = ReadCallResultHandle();
            }

            bool invoked = CallbackBaseInvoker.RunCallResult(Pointer, pvParam, bIOFailure, hSteamAPICall);

            if (TraceNativeDispatch)
            {
                ulong afterApiCall = ReadCallResultHandle();
                SteamEmulator.Write(
                    "CallbackManager",
                    $"RunCallResult ptr=0x{Pointer.ToInt64():X} callback={(int)CallbackType} handle={hSteamAPICall} objectHandleBefore={beforeApiCall} objectHandleAfter={afterApiCall} ioFailure={bIOFailure} invoked={invoked}");
            }

            return invoked;
        }

        public int GetCallbackSizeBytes()
        {
            return CallbackBaseInvoker.GetCallbackSizeBytes(Pointer);
        }

        public void Register()
        {
            if (Pointer != IntPtr.Zero)
            {
                CallbackBaseInvoker.RegisterCallback(Pointer, (int)CallbackType);
            }
        }

        public void Unregister()
        {
            if (Pointer != IntPtr.Zero)
            {
                CallbackBaseInvoker.UnregisterCallback(Pointer);
            }
        }

        private void LogNativeState(string reason)
        {
            if (VTableLogged)
            {
                return;
            }

            VTableLogged = true;
            SteamEmulator.Write(
                "CallbackManager",
                $"{reason} ptr=0x{Pointer.ToInt64():X} callback={(int)CallbackType} nativeCallback={CallbackBaseInvoker.GetCallbackId(Pointer)} flags={CallbackBaseInvoker.GetCallbackFlags(Pointer)} nativeSize={GetCallbackSizeBytes()} objectHandle={ReadCallResultHandle()}");
        }

        private ulong ReadCallResultHandle()
        {
            if (Pointer == IntPtr.Zero)
            {
                return 0;
            }

            try
            {
                return (ulong)Marshal.ReadInt64(Pointer, IntPtr.Size + 8);
            }
            catch
            {
                return 0;
            }
        }
    }
}
