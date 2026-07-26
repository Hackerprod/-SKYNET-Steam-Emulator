using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamRemotePlay
    {
        static SteamAPI_ISteamRemotePlay()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamRemotePlay_GetSessionCount(IntPtr _)
        {
            Write("SteamAPI_ISteamRemotePlay_GetSessionCount");
            return SteamEmulator.SteamRemotePlay.GetSessionCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamRemotePlay_GetSessionID(IntPtr _, int iSessionIndex)
        {
            Write("SteamAPI_ISteamRemotePlay_GetSessionID");
            return SteamEmulator.SteamRemotePlay.GetSessionID(iSessionIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemotePlay_BSessionRemotePlayTogether(IntPtr _, uint unSessionID)
        {
            Write("SteamAPI_ISteamRemotePlay_BSessionRemotePlayTogether");
            return SteamEmulator.SteamRemotePlay.BSessionRemotePlayTogether(unSessionID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemotePlay_GetSessionSteamID(IntPtr _, uint unSessionID)
        {
            Write("SteamAPI_ISteamRemotePlay_IntPtrGetSessionSteamID");
            return SteamEmulator.SteamRemotePlay.GetSessionSteamID(unSessionID).SteamID;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamRemotePlay_GetSessionGuestID(IntPtr _, uint unSessionID)
        {
            Write("SteamAPI_ISteamRemotePlay_GetSessionGuestID");
            return SteamEmulator.SteamRemotePlay.GetSessionGuestID(unSessionID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamRemotePlay_GetSmallSessionAvatar(IntPtr _, uint unSessionID)
        {
            Write("SteamAPI_ISteamRemotePlay_GetSmallSessionAvatar");
            return SteamEmulator.SteamRemotePlay.GetSmallSessionAvatar(unSessionID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamRemotePlay_GetMediumSessionAvatar(IntPtr _, uint unSessionID)
        {
            Write("SteamAPI_ISteamRemotePlay_GetMediumSessionAvatar");
            return SteamEmulator.SteamRemotePlay.GetMediumSessionAvatar(unSessionID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamRemotePlay_GetLargeSessionAvatar(IntPtr _, uint unSessionID)
        {
            Write("SteamAPI_ISteamRemotePlay_GetLargeSessionAvatar");
            return SteamEmulator.SteamRemotePlay.GetLargeSessionAvatar(unSessionID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamRemotePlay_GetSessionClientName(IntPtr _, uint unSessionID)
        {
            Write("SteamAPI_ISteamRemotePlay_GetSessionClientName");
            return SKYNET.Helpers.NativeStringCache.ToUtf8Ptr(
                SteamEmulator.SteamRemotePlay.GetSessionClientName(unSessionID));
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamRemotePlay_GetSessionClientFormFactor(IntPtr _, uint unSessionID)
        {
            Write("SteamAPI_ISteamRemotePlay_GetSessionClientFormFactor");
            return SteamEmulator.SteamRemotePlay.GetSessionClientFormFactor(unSessionID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemotePlay_BGetSessionClientResolution(IntPtr _, uint unSessionID, IntPtr pnResolutionX, IntPtr pnResolutionY)
        {
            Write("SteamAPI_ISteamRemotePlay_BGetSessionClientResolution");
            return SteamEmulator.SteamRemotePlay.BGetSessionClientResolution(unSessionID, pnResolutionX, pnResolutionY);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemotePlay_ShowRemotePlayTogetherUI(IntPtr _)
        {
            Write("SteamAPI_ISteamRemotePlay_ShowRemotePlayTogetherUI");
            return SteamEmulator.SteamRemotePlay.ShowRemotePlayTogetherUI();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemotePlay_BSendRemotePlayTogetherInvite(IntPtr _, ulong steamIDFriend)
        {
            Write("SteamAPI_ISteamRemotePlay_BSendRemotePlayTogetherInvite");
            return SteamEmulator.SteamRemotePlay.BSendRemotePlayTogetherInvite(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemotePlay_BEnableRemotePlayTogetherDirectInput(IntPtr _)
        {
            Write("SteamAPI_ISteamRemotePlay_BEnableRemotePlayTogetherDirectInput");
            return SteamEmulator.SteamRemotePlay.BEnableRemotePlayTogetherDirectInput();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamRemotePlay_DisableRemotePlayTogetherDirectInput(IntPtr _)
        {
            Write("SteamAPI_ISteamRemotePlay_DisableRemotePlayTogetherDirectInput");
            SteamEmulator.SteamRemotePlay.DisableRemotePlayTogetherDirectInput();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamRemotePlay_GetInput(IntPtr _, IntPtr pInput, uint unMaxEvents)
        {
            Write("SteamAPI_ISteamRemotePlay_GetInput");
            return SteamEmulator.SteamRemotePlay.GetInput(pInput, unMaxEvents);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamRemotePlay_SetMouseVisibility(IntPtr _, uint unSessionID, bool bVisible)
        {
            Write("SteamAPI_ISteamRemotePlay_SetMouseVisibility");
            SteamEmulator.SteamRemotePlay.SetMouseVisibility(unSessionID, bVisible);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamRemotePlay_SetMousePosition(IntPtr _, uint unSessionID, float flNormalizedX, float flNormalizedY)
        {
            Write("SteamAPI_ISteamRemotePlay_SetMousePosition");
            SteamEmulator.SteamRemotePlay.SetMousePosition(unSessionID, flNormalizedX, flNormalizedY);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamRemotePlay_CreateMouseCursor(IntPtr _, int nWidth, int nHeight, int nHotX, int nHotY, IntPtr pBGRA, int nPitch)
        {
            Write("SteamAPI_ISteamRemotePlay_CreateMouseCursor");
            return SteamEmulator.SteamRemotePlay.CreateMouseCursor(nWidth, nHeight, nHotX, nHotY, pBGRA, nPitch);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamRemotePlay_SetMouseCursor(IntPtr _, uint unSessionID, uint unCursorID)
        {
            Write("SteamAPI_ISteamRemotePlay_SetMouseCursor");
            SteamEmulator.SteamRemotePlay.SetMouseCursor(unSessionID, unCursorID);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
