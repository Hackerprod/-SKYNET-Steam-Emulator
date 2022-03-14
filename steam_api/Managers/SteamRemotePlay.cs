using SKYNET.Interface;
using System;

namespace SKYNET.Managers
{
    public class SteamRemotePlay : ISteamRemotePlay
    {
        public uint GetSessionCount()
        {
            return 0;
        }

        public uint GetSessionID(int iSessionIndex)
        {
            return default;
        }

        public IntPtr GetSessionSteamID(uint unSessionID)
        {
            return default;
        }

        public string GetSessionClientName(uint unSessionID)
        {
            return default;
        }

        public ESteamDeviceFormFactor GetSessionClientFormFactor(uint unSessionID)
        {
            return default;
        }

        public bool BGetSessionClientResolution(uint unSessionID, int pnResolutionX, int pnResolutionY)
        {
            return default;
        }

        public bool BSendRemotePlayTogetherInvite(IntPtr steamIDFriend)
        {
            return default;
        }

    }

}