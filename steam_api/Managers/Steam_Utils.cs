using System;
using System.Runtime.InteropServices;
using SKYNET.Interface;

namespace SKYNET.Managers
{
    public class Steam_Utils : SteamInterface//, ISteamUtils
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr GetAPICallFailureReason()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr GetAPICallResult()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr GetAppID()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr GetConnectedUniverse()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr GetCSERIPPort()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr GetCurrentBatteryPower()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr GetImageRGBA()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr GetImageSize()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr GetIPCCallCount()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr GetIPCountry()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr GetSecondsSinceAppActive()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr GetSecondsSinceComputerActive()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr GetServerRealTime()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr IsAPICallCompleted()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr IsOverlayEnabled()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr OverlayNeedsPresent()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr RunFrame()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SetOverlayNotificationPosition()
        {
            return (IntPtr)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SetWarningMessageHook()
        {
            //
        }
    }
}