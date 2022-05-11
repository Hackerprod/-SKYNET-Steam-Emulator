using System;

using uint32 = System.UInt32;
using uint64 = System.UInt64;
using ClientUnifiedMessageHandle = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("STEAMUNIFIEDMESSAGES_INTERFACE_VERSION001")]
    public class SteamUnifiedMessages001 : ISteamInterface
    {
        private ClientUnifiedMessageHandle k_InvalidUnifiedMessageHandle = 0;

        public ClientUnifiedMessageHandle SendMethod(IntPtr _, char pchServiceMethod, IntPtr pRequestBuffer, uint32 unRequestBufferSize, uint64 unContext)
        {
            Write("ClientUnifiedMessageHandle");
            return k_InvalidUnifiedMessageHandle;
        }

        public bool GetMethodResponseInfo(IntPtr _, ClientUnifiedMessageHandle hHandle, uint32 punResponseSize, EResult peResult)
        {
            Write("GetMethodResponseInfo");
            return false;
        }

        public bool GetMethodResponseData(IntPtr _, ClientUnifiedMessageHandle hHandle, IntPtr pResponseBuffer, uint32 unResponseBufferSize, bool bAutoRelease)
        {
            Write("GetMethodResponseData");
            return false;
        }

        public bool ReleaseMethod(IntPtr _, ClientUnifiedMessageHandle hHandle)
        {
            Write("ReleaseMethod");
            return false;
        }

        public bool SendNotification(IntPtr _, char pchServiceNotification, IntPtr pNotificationBuffer, uint32 unNotificationBufferSize)
        {
            Write("SendNotification");
            return false;
        }

        private void Write(object msg)
        {
            SteamEmulator.Write("SteamUnifiedMessages001", msg);
        }
    }
}
