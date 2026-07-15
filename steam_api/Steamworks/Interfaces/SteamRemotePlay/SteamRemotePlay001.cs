using SKYNET.Steamworks;
using System;
using SKYNET.Helpers;

using RemotePlaySessionID_t = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
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

        public IntPtr GetSessionSteamID(IntPtr _, IntPtr ret, RemotePlaySessionID_t unSessionID)
        {
            return NativeSteamId.Write(ret, SteamEmulator.SteamRemotePlay.GetSessionSteamID(unSessionID));
        }

        public IntPtr GetSessionClientName(IntPtr _, RemotePlaySessionID_t unSessionID)
        {
            return NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamRemotePlay.GetSessionClientName(unSessionID));
        }

        public int GetSessionClientFormFactor(IntPtr _, RemotePlaySessionID_t unSessionID)
        {
            return SteamEmulator.SteamRemotePlay.GetSessionClientFormFactor(unSessionID);
        }

        public bool BGetSessionClientResolution(IntPtr _, RemotePlaySessionID_t unSessionID, IntPtr pnResolutionX, IntPtr pnResolutionY)
        {
            return SteamEmulator.SteamRemotePlay.BGetSessionClientResolution(unSessionID, pnResolutionX, pnResolutionY);
        }

        public bool BSendRemotePlayTogetherInvite(IntPtr _, ulong steamIDFriend)
        {
            return SteamEmulator.SteamRemotePlay.BSendRemotePlayTogetherInvite(steamIDFriend);
        }
    }
}
