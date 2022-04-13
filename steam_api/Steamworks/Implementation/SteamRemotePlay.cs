using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamRemotePlay : ISteamInterface
    {
        public SteamRemotePlay()
        {
            InterfaceVersion = "SteamRemotePlay";
        }

        public uint GetSessionCount(IntPtr _)
        {
            Write("GetSessionCount");
            return 0;
        }

        public uint GetSessionID(int iSessionIndex)
        {
            Write("GetSessionID");
            return default;
        }

        public IntPtr GetSessionSteamID(uint unSessionID)
        {
            Write("GetSessionSteamID");
            return default;
        }

        public string GetSessionClientName(uint unSessionID)
        {
            Write("GetSessionClientName");
            return default;
        }

        public ESteamDeviceFormFactor GetSessionClientFormFactor(uint unSessionID)
        {
            Write("GetSessionClientFormFactor");
            return default;
        }

        public bool BGetSessionClientResolution(uint unSessionID, int pnResolutionX, int pnResolutionY)
        {
            Write("BGetSessionClientResolution");
            return default;
        }

        public bool BSendRemotePlayTogetherInvite(IntPtr steamIDFriend)
        {
            Write("BSendRemotePlayTogetherInvite");
            return default;
        }
    }
}