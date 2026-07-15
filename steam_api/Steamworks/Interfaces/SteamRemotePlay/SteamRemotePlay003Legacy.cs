using System;
using SKYNET.Helpers;

using RemotePlayCursorID_t = System.UInt32;
using RemotePlaySessionID_t = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("STEAMREMOTEPLAY_INTERFACE_VERSION003")]
    public class SteamRemotePlay003Legacy : ISteamInterface
    {
        public uint GetSessionCount(IntPtr _) => SteamEmulator.SteamRemotePlay.GetSessionCount();
        public RemotePlaySessionID_t GetSessionID(IntPtr _, int iSessionIndex) => SteamEmulator.SteamRemotePlay.GetSessionID(iSessionIndex);
        public IntPtr GetSessionSteamID(IntPtr _, IntPtr ret, RemotePlaySessionID_t unSessionID) => NativeSteamId.Write(ret, SteamEmulator.SteamRemotePlay.GetSessionSteamID(unSessionID));
        public IntPtr GetSessionClientName(IntPtr _, RemotePlaySessionID_t unSessionID) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamRemotePlay.GetSessionClientName(unSessionID));
        public int GetSessionClientFormFactor(IntPtr _, RemotePlaySessionID_t unSessionID) => SteamEmulator.SteamRemotePlay.GetSessionClientFormFactor(unSessionID);
        public bool BGetSessionClientResolution(IntPtr _, RemotePlaySessionID_t unSessionID, IntPtr pnResolutionX, IntPtr pnResolutionY) => SteamEmulator.SteamRemotePlay.BGetSessionClientResolution(unSessionID, pnResolutionX, pnResolutionY);
        public bool ShowRemotePlayTogetherUI(IntPtr _) => SteamEmulator.SteamRemotePlay.ShowRemotePlayTogetherUI();
        public bool BSendRemotePlayTogetherInvite(IntPtr _, ulong steamIDFriend) => SteamEmulator.SteamRemotePlay.BSendRemotePlayTogetherInvite(steamIDFriend);
        public bool BEnableRemotePlayTogetherDirectInput(IntPtr _) => SteamEmulator.SteamRemotePlay.BEnableRemotePlayTogetherDirectInput();
        public void DisableRemotePlayTogetherDirectInput(IntPtr _) => SteamEmulator.SteamRemotePlay.DisableRemotePlayTogetherDirectInput();
        public uint GetInput(IntPtr _, IntPtr pInput, uint unMaxEvents) => SteamEmulator.SteamRemotePlay.GetInput(pInput, unMaxEvents);
        public void SetMouseVisibility(IntPtr _, RemotePlaySessionID_t unSessionID, bool bVisible) => SteamEmulator.SteamRemotePlay.SetMouseVisibility(unSessionID, bVisible);
        public void SetMousePosition(IntPtr _, RemotePlaySessionID_t unSessionID, float flNormalizedX, float flNormalizedY) => SteamEmulator.SteamRemotePlay.SetMousePosition(unSessionID, flNormalizedX, flNormalizedY);
        public RemotePlayCursorID_t CreateMouseCursor(IntPtr _, int nWidth, int nHeight, int nHotX, int nHotY, IntPtr pBGRA, int nPitch) => SteamEmulator.SteamRemotePlay.CreateMouseCursor(nWidth, nHeight, nHotX, nHotY, pBGRA, nPitch);
        public void SetMouseCursor(IntPtr _, RemotePlaySessionID_t unSessionID, RemotePlayCursorID_t unCursorID) => SteamEmulator.SteamRemotePlay.SetMouseCursor(unSessionID, unCursorID);
    }
}
