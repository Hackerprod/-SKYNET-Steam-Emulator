using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    // These flat wrappers only forward to the virtual methods of objects implemented by the game
    // (ISteamNetworkingConnectionCustomSignaling and ISteamNetworkingCustomSignalingRecvContext).
    public class SteamAPI_ISteamNetworkingCustomSignaling
    {
        private const int SendSignalSlot = 0;
        private const int ReleaseSlot = 1;
        private const int OnConnectRequestSlot = 0;
        private const int SendRejectionSignalSlot = 1;

        static SteamAPI_ISteamNetworkingCustomSignaling()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamNetworkingConnectionCustomSignaling_SendSignal(IntPtr _, uint hConn, IntPtr info, IntPtr pMsg, int cbMsg)
        {
            Write("SteamAPI_ISteamNetworkingConnectionCustomSignaling_SendSignal");
            var function = GetVirtualFunction(_, SendSignalSlot);
            if (function == IntPtr.Zero)
            {
                return false;
            }

            return IntPtr.Size == 4
                ? ((SendSignalThisCall)Marshal.GetDelegateForFunctionPointer(function, typeof(SendSignalThisCall)))(_, hConn, info, pMsg, cbMsg) != 0
                : ((SendSignalCdecl)Marshal.GetDelegateForFunctionPointer(function, typeof(SendSignalCdecl)))(_, hConn, info, pMsg, cbMsg) != 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamNetworkingConnectionCustomSignaling_Release(IntPtr _)
        {
            Write("SteamAPI_ISteamNetworkingConnectionCustomSignaling_Release");
            var function = GetVirtualFunction(_, ReleaseSlot);
            if (function == IntPtr.Zero)
            {
                return;
            }

            if (IntPtr.Size == 4)
            {
                ((ReleaseThisCall)Marshal.GetDelegateForFunctionPointer(function, typeof(ReleaseThisCall)))(_);
            }
            else
            {
                ((ReleaseCdecl)Marshal.GetDelegateForFunctionPointer(function, typeof(ReleaseCdecl)))(_);
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamNetworkingCustomSignalingRecvContext_OnConnectRequest(IntPtr _, uint hConn, IntPtr identityPeer)
        {
            Write("SteamAPI_ISteamNetworkingCustomSignalingRecvContext_OnConnectRequest");
            var function = GetVirtualFunction(_, OnConnectRequestSlot);
            if (function == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            return IntPtr.Size == 4
                ? ((OnConnectRequestThisCall)Marshal.GetDelegateForFunctionPointer(function, typeof(OnConnectRequestThisCall)))(_, hConn, identityPeer)
                : ((OnConnectRequestCdecl)Marshal.GetDelegateForFunctionPointer(function, typeof(OnConnectRequestCdecl)))(_, hConn, identityPeer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamNetworkingCustomSignalingRecvContext_SendRejectionSignal(IntPtr _, IntPtr identityPeer, IntPtr pMsg, int cbMsg)
        {
            Write("SteamAPI_ISteamNetworkingCustomSignalingRecvContext_SendRejectionSignal");
            var function = GetVirtualFunction(_, SendRejectionSignalSlot);
            if (function == IntPtr.Zero)
            {
                return;
            }

            if (IntPtr.Size == 4)
            {
                ((SendRejectionSignalThisCall)Marshal.GetDelegateForFunctionPointer(function, typeof(SendRejectionSignalThisCall)))(_, identityPeer, pMsg, cbMsg);
            }
            else
            {
                ((SendRejectionSignalCdecl)Marshal.GetDelegateForFunctionPointer(function, typeof(SendRejectionSignalCdecl)))(_, identityPeer, pMsg, cbMsg);
            }
        }

        private static IntPtr GetVirtualFunction(IntPtr self, int slot)
        {
            if (self == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            var vtable = Marshal.ReadIntPtr(self);
            return vtable == IntPtr.Zero ? IntPtr.Zero : Marshal.ReadIntPtr(vtable, slot * IntPtr.Size);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate byte SendSignalCdecl(IntPtr self, uint hConn, IntPtr info, IntPtr pMsg, int cbMsg);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate byte SendSignalThisCall(IntPtr self, uint hConn, IntPtr info, IntPtr pMsg, int cbMsg);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ReleaseCdecl(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void ReleaseThisCall(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr OnConnectRequestCdecl(IntPtr self, uint hConn, IntPtr identityPeer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate IntPtr OnConnectRequestThisCall(IntPtr self, uint hConn, IntPtr identityPeer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void SendRejectionSignalCdecl(IntPtr self, IntPtr identityPeer, IntPtr pMsg, int cbMsg);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void SendRejectionSignalThisCall(IntPtr self, IntPtr identityPeer, IntPtr pMsg, int cbMsg);

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
