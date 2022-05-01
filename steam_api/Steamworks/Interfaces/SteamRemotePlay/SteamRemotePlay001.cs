using Steamworks;
using System;

using RemotePlaySessionID_t = System.UInt32;

namespace SKYNET.Interface
{
    [Interface("STEAMREMOTEPLAY_INTERFACE_VERSION001")]
    public class SteamRemotePlay001 : ISteamInterface
    {
        public UInt32 GetSessionCount(IntPtr _)
        {
            return SteamEmulator.SteamRemotePlay.GetSessionCount();
        }

        public RemotePlaySessionID_t GetSessionID(IntPtr _, int iSessionIndex)
        {
            return SteamEmulator.SteamRemotePlay.GetSessionID(iSessionIndex);
        }

        public CSteamID GetSessionSteamID(IntPtr _, RemotePlaySessionID_t unSessionID)
        {
            return SteamEmulator.SteamRemotePlay.GetSessionSteamID(unSessionID);
        }

        public string GetSessionClientName(IntPtr _, RemotePlaySessionID_t unSessionID)
        {
            return SteamEmulator.SteamRemotePlay.GetSessionClientName(unSessionID);
        }

        public int GetSessionClientFormFactor(IntPtr _, RemotePlaySessionID_t unSessionID)
        {
            return SteamEmulator.SteamRemotePlay.GetSessionClientFormFactor(unSessionID);
        }

        public bool BGetSessionClientResolution(IntPtr _, RemotePlaySessionID_t unSessionID, int pnResolutionX, int pnResolutionY)
        {
            return SteamEmulator.SteamRemotePlay.BGetSessionClientResolution(unSessionID, pnResolutionX, pnResolutionY);
        }

        public bool BSendRemotePlayTogetherInvite(IntPtr _, ulong steamIDFriend)
        {
            return SteamEmulator.SteamRemotePlay.BSendRemotePlayTogetherInvite(steamIDFriend);
        }
    }
}
