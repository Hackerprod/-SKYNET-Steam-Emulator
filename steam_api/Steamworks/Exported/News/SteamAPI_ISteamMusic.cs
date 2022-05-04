using SKYNET;
using SKYNET.Managers;
using SKYNET.Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamMusic 
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusic_BIsEnabled(IntPtr _)
        {
            Write("SteamAPI_ISteamMusic_BIsEnabled");
            return SteamEmulator.SteamMusic.BIsEnabled();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusic_BIsPlaying(IntPtr _)
        {
            Write("SteamAPI_ISteamMusic_BIsPlaying");
            return SteamEmulator.SteamMusic.BIsPlaying();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamMusic_GetPlaybackStatus(IntPtr _)
        {
            Write("SteamAPI_ISteamMusic_GetPlaybackStatus");
            return SteamEmulator.SteamMusic.GetPlaybackStatus();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static float SteamAPI_ISteamMusic_GetVolume(IntPtr _)
        {
            Write("SteamAPI_ISteamMusic_GetVolume");
            return SteamEmulator.SteamMusic.GetVolume();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMusic_Pause(IntPtr _)
        {
            Write("SteamAPI_ISteamMusic_Pause");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMusic_Play(IntPtr _)
        {
            Write("SteamAPI_ISteamMusic_Play");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMusic_PlayNext(IntPtr _)
        {
            Write("SteamAPI_ISteamMusic_PlayNext");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMusic_PlayPrevious(IntPtr _)
        {
            Write("SteamAPI_ISteamMusic_PlayPrevious");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMusic_SetVolume(IntPtr _, float flVolume)
        {
            Write("SteamAPI_ISteamMusic_SetVolume");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamMusic_v001()
        {
            Write("SteamAPI_SteamMusic_v001");
            return InterfaceManager.FindOrCreateInterface("SteamMusic001");
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
