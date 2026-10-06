using System;
using System.Runtime.InteropServices;

using ClientUnifiedMessageHandle = System.UInt64;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamUnifiedMessages
    {
        static SteamAPI_ISteamUnifiedMessages()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ClientUnifiedMessageHandle SteamAPI_ISteamUnifiedMessages_SendMethod(IntPtr _, string pchServiceMethod, IntPtr pRequestBuffer, uint unRequestBufferSize, ulong unContext)
        {
            Write("SteamAPI_ISteamUnifiedMessages_SendMethod");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUnifiedMessages_GetMethodResponseInfo(IntPtr _, ClientUnifiedMessageHandle hHandle, IntPtr punResponseSize, IntPtr peResult)
        {
            Write("SteamAPI_ISteamUnifiedMessages_GetMethodResponseInfo");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUnifiedMessages_GetMethodResponseData(IntPtr _, ClientUnifiedMessageHandle hHandle, IntPtr pResponseBuffer, uint unResponseBufferSize, bool bAutoRelease)
        {
            Write("SteamAPI_ISteamUnifiedMessages_GetMethodResponseData");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUnifiedMessages_ReleaseMethod(IntPtr _, ClientUnifiedMessageHandle hHandle)
        {
            Write("SteamAPI_ISteamUnifiedMessages_ReleaseMethod");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUnifiedMessages_SendNotification(IntPtr _, string pchServiceNotification, IntPtr pNotificationBuffer, uint unNotificationBufferSize)
        {
            Write("SteamAPI_ISteamUnifiedMessages_SendNotification");
            return false;
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
