using SKYNET;
using SKYNET.Interface;
using System;
using System.Runtime.InteropServices;

public class SteamAPI_ISteamMusic : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusic_BIsEnabled(IntPtr _)
    {
        Write("SteamAPI_ISteamMusic_BIsEnabled");
        return SteamEmulator.SteamMusic.BIsEnabled(_);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusic_BIsPlaying(IntPtr _)
    {
        Write("SteamAPI_ISteamMusic_BIsPlaying");
        return SteamEmulator.SteamMusic.BIsPlaying(_);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static AudioPlayback_Status SteamAPI_ISteamMusic_GetPlaybackStatus(IntPtr _)
    {
        Write("SteamAPI_ISteamMusic_GetPlaybackStatus");
        return SteamEmulator.SteamMusic.GetPlaybackStatus(_);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static float SteamAPI_ISteamMusic_GetVolume(IntPtr _)
    {
        Write("SteamAPI_ISteamMusic_GetVolume");
        return SteamEmulator.SteamMusic.GetVolume(_);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamMusic_Pause()
    {
        Write("SteamAPI_ISteamMusic_Pause");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamMusic_Play()
    {
        Write("SteamAPI_ISteamMusic_Play");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamMusic_PlayNext()
    {
        Write("SteamAPI_ISteamMusic_PlayNext");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamMusic_PlayPrevious()
    {
        Write("SteamAPI_ISteamMusic_PlayPrevious");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamMusic_SetVolume(float flVolume)
    {
        Write("SteamAPI_ISteamMusic_SetVolume");
    }
}
