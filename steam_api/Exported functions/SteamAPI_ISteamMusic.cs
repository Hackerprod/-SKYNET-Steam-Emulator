using SKYNET;
using SKYNET.Steamworks;
using System;
using System.Runtime.InteropServices;

public class SteamAPI_ISteamMusic : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusic_BIsEnabled()
    {
        Write("SteamAPI_ISteamMusic_BIsEnabled");
        return SteamEmulator.SteamMusic.BIsEnabled(SteamEmulator.SteamMusic.MemoryAddress);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusic_BIsPlaying()
    {
        Write("SteamAPI_ISteamMusic_BIsPlaying");
        return SteamEmulator.SteamMusic.BIsPlaying(SteamEmulator.SteamMusic.MemoryAddress);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static AudioPlayback_Status SteamAPI_ISteamMusic_GetPlaybackStatus()
    {
        Write("SteamAPI_ISteamMusic_GetPlaybackStatus");
        return SteamEmulator.SteamMusic.GetPlaybackStatus(SteamEmulator.SteamMusic.MemoryAddress);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static float SteamAPI_ISteamMusic_GetVolume()
    {
        Write("SteamAPI_ISteamMusic_GetVolume");
        return SteamEmulator.SteamMusic.GetVolume(SteamEmulator.SteamMusic.MemoryAddress);
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
