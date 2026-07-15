using System;
using SKYNET.Helpers;

using RemotePlayCursorID_t = System.UInt32;
using RemotePlaySessionID_t = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("STEAMREMOTEPLAY_INTERFACE_VERSION004")]
    public class SteamRemotePlay003 : ISteamInterface
    {
        public uint GetSessionCount(IntPtr _)
        {
            return SteamEmulator.SteamRemotePlay.GetSessionCount();
        }

        public RemotePlaySessionID_t GetSessionID(IntPtr _, int iSessionIndex)
        {
            return SteamEmulator.SteamRemotePlay.GetSessionID(iSessionIndex);
        }

        public bool BSessionRemotePlayTogether(IntPtr _, RemotePlaySessionID_t unSessionID)
        {
            return SteamEmulator.SteamRemotePlay.BSessionRemotePlayTogether(unSessionID);
        }

        public IntPtr GetSessionSteamID(IntPtr _, IntPtr ret, RemotePlaySessionID_t unSessionID)
        {
            return NativeSteamId.Write(ret, SteamEmulator.SteamRemotePlay.GetSessionSteamID(unSessionID));
        }

        public uint GetSessionGuestID(IntPtr _, RemotePlaySessionID_t unSessionID)
        {
            return SteamEmulator.SteamRemotePlay.GetSessionGuestID(unSessionID);
        }

        public int GetSmallSessionAvatar(IntPtr _, RemotePlaySessionID_t unSessionID)
        {
            return SteamEmulator.SteamRemotePlay.GetSmallSessionAvatar(unSessionID);
        }

        public int GetMediumSessionAvatar(IntPtr _, RemotePlaySessionID_t unSessionID)
        {
            return SteamEmulator.SteamRemotePlay.GetMediumSessionAvatar(unSessionID);
        }

        public int GetLargeSessionAvatar(IntPtr _, RemotePlaySessionID_t unSessionID)
        {
            return SteamEmulator.SteamRemotePlay.GetLargeSessionAvatar(unSessionID);
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

        public bool ShowRemotePlayTogetherUI(IntPtr _)
        {
            return SteamEmulator.SteamRemotePlay.ShowRemotePlayTogetherUI();
        }

        public bool BSendRemotePlayTogetherInvite(IntPtr _, ulong steamIDFriend)
        {
            return SteamEmulator.SteamRemotePlay.BSendRemotePlayTogetherInvite(steamIDFriend);
        }

        public bool BEnableRemotePlayTogetherDirectInput(IntPtr _)
        {
            return SteamEmulator.SteamRemotePlay.BEnableRemotePlayTogetherDirectInput();
        }

        public void DisableRemotePlayTogetherDirectInput(IntPtr _)
        {
            SteamEmulator.SteamRemotePlay.DisableRemotePlayTogetherDirectInput();
        }

        public uint GetInput(IntPtr _, IntPtr pInput, uint unMaxEvents)
        {
            return SteamEmulator.SteamRemotePlay.GetInput(pInput, unMaxEvents);
        }

        public void SetMouseVisibility(IntPtr _, RemotePlaySessionID_t unSessionID, bool bVisible)
        {
            SteamEmulator.SteamRemotePlay.SetMouseVisibility(unSessionID, bVisible);
        }

        public void SetMousePosition(IntPtr _, RemotePlaySessionID_t unSessionID, float flNormalizedX, float flNormalizedY)
        {
            SteamEmulator.SteamRemotePlay.SetMousePosition(unSessionID, flNormalizedX, flNormalizedY);
        }

        public RemotePlayCursorID_t CreateMouseCursor(IntPtr _, int nWidth, int nHeight, int nHotX, int nHotY, IntPtr pBGRA, int nPitch)
        {
            return SteamEmulator.SteamRemotePlay.CreateMouseCursor(nWidth, nHeight, nHotX, nHotY, pBGRA, nPitch);
        }

        public void SetMouseCursor(IntPtr _, RemotePlaySessionID_t unSessionID, RemotePlayCursorID_t unCursorID)
        {
            SteamEmulator.SteamRemotePlay.SetMouseCursor(unSessionID, unCursorID);
        }
    }
}
