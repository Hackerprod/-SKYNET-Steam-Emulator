using System;
using System.Runtime.InteropServices;
using SKYNET.Interface;

namespace SKYNET.Managers
{
    public class Steam_Utils : SteamInterface, ISteamUtils
    {
        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr GetAPICallFailureReason()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr GetAPICallResult()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr GetAppID()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr GetConnectedUniverse()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr GetCSERIPPort()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr GetCurrentBatteryPower()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr GetImageRGBA()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr GetImageSize()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr GetIPCCallCount()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr GetIPCountry()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr GetSecondsSinceAppActive()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr GetSecondsSinceComputerActive()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr GetServerRealTime()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr IsAPICallCompleted()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr IsOverlayEnabled()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr OverlayNeedsPresent()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr RunFrame()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public IntPtr SetOverlayNotificationPosition()
        {
            return (IntPtr)1;
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        public void SetWarningMessageHook()
        {
            //
        }
    }
}