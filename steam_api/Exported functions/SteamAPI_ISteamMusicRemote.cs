using SKYNET;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class SteamAPI_ISteamMusicRemote : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_RegisterSteamMusicRemote(string pchName)
    {
        Write("SteamAPI_ISteamMusicRemote_RegisterSteamMusicRemote");
        return SteamEmulator.SteamMusicRemote.RegisterSteamMusicRemote(pchName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_DeregisterSteamMusicRemote()
    {
        Write("SteamAPI_ISteamMusicRemote_DeregisterSteamMusicRemote");
        return SteamEmulator.SteamMusicRemote.DeregisterSteamMusicRemote(SteamEmulator.SteamFriends.MemoryAddress);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_BIsCurrentMusicRemote()
    {
        Write("SteamAPI_ISteamMusicRemote_BIsCurrentMusicRemote");
        return SteamEmulator.SteamMusicRemote.BIsCurrentMusicRemote(SteamEmulator.SteamFriends.MemoryAddress);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_BActivationSuccess(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_BActivationSuccess");
        return SteamEmulator.SteamMusicRemote.BActivationSuccess(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_SetDisplayName(string pchDisplayName)
    {
        Write("SteamAPI_ISteamMusicRemote_SetDisplayName");
        return SteamEmulator.SteamMusicRemote.SetDisplayName(pchDisplayName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_SetPNGIcon_64x64(IntPtr pvBuffer, uint cbBufferLength)
    {
        Write("SteamAPI_ISteamMusicRemote_SetPNGIcon_64x64");
        return SteamEmulator.SteamMusicRemote.SetPNGIcon_64x64(pvBuffer, cbBufferLength);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_EnablePlayPrevious(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_EnablePlayPrevious");
        return SteamEmulator.SteamMusicRemote.EnablePlayPrevious(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_EnablePlayNext(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_EnablePlayNext");
        return SteamEmulator.SteamMusicRemote.EnablePlayNext(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_EnableShuffled(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_EnableShuffled");
        return SteamEmulator.SteamMusicRemote.EnableShuffled(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_EnableLooped(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_EnableLooped");
        return SteamEmulator.SteamMusicRemote.EnableLooped(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_EnableQueue(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_EnableQueue");
        return SteamEmulator.SteamMusicRemote.EnableQueue(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_EnablePlaylists(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_EnablePlaylists");
        return SteamEmulator.SteamMusicRemote.EnablePlaylists(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_UpdatePlaybackStatus(AudioPlayback_Status nStatus)
    {
        Write("SteamAPI_ISteamMusicRemote_UpdatePlaybackStatus");
        return SteamEmulator.SteamMusicRemote.UpdatePlaybackStatus(nStatus);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_UpdateShuffled(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_UpdateShuffled");
        return SteamEmulator.SteamMusicRemote.UpdateShuffled(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_UpdateLooped(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_UpdateLooped");
        return SteamEmulator.SteamMusicRemote.UpdateLooped(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_UpdateVolume(float flValue) // volume is between 0.0 and 1.0 
    {
        Write("SteamAPI_ISteamMusicRemote_UpdateVolume");
        return SteamEmulator.SteamMusicRemote.UpdateVolume(flValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_CurrentEntryWillChange()
    {
        Write("SteamAPI_ISteamMusicRemote_CurrentEntryWillChange");
        return SteamEmulator.SteamMusicRemote.CurrentEntryWillChange(SteamEmulator.SteamFriends.MemoryAddress);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_CurrentEntryIsAvailable(bool bAvailable)
    {
        Write("SteamAPI_ISteamMusicRemote_CurrentEntryIsAvailable");
        return SteamEmulator.SteamMusicRemote.CurrentEntryIsAvailable(bAvailable);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_UpdateCurrentEntryText(string pchText)
    {
        Write("SteamAPI_ISteamMusicRemote_UpdateCurrentEntryText");
        return SteamEmulator.SteamMusicRemote.UpdateCurrentEntryText(pchText);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_UpdateCurrentEntryElapsedSeconds(int nValue)
    {
        Write("SteamAPI_ISteamMusicRemote_UpdateCurrentEntryElapsedSeconds");
        return SteamEmulator.SteamMusicRemote.UpdateCurrentEntryElapsedSeconds(nValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_UpdateCurrentEntryCoverArt(IntPtr pvBuffer, uint cbBufferLength)
    {
        Write("SteamAPI_ISteamMusicRemote_UpdateCurrentEntryCoverArt");
        return SteamEmulator.SteamMusicRemote.UpdateCurrentEntryCoverArt(pvBuffer, cbBufferLength);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_CurrentEntryDidChange()
    {
        Write("SteamAPI_ISteamMusicRemote_CurrentEntryDidChange");
        return SteamEmulator.SteamMusicRemote.CurrentEntryDidChange(SteamEmulator.SteamFriends.MemoryAddress);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_QueueWillChange()
    {
        Write("SteamAPI_ISteamMusicRemote_QueueWillChange");
        return SteamEmulator.SteamMusicRemote.QueueWillChange(SteamEmulator.SteamFriends.MemoryAddress);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_ResetQueueEntries()
    {
        Write("SteamAPI_ISteamMusicRemote_ResetQueueEntries");
        return SteamEmulator.SteamMusicRemote.ResetQueueEntries(SteamEmulator.SteamFriends.MemoryAddress);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_SetQueueEntry(int nID, int nPosition, string pchEntryText)
    {
        Write("SteamAPI_ISteamMusicRemote_SetQueueEntry");
        return SteamEmulator.SteamMusicRemote.SetQueueEntry(nID, nPosition, pchEntryText);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_SetCurrentQueueEntry(int nID)
    {
        Write("SteamAPI_ISteamMusicRemote_SetCurrentQueueEntry");
        return SteamEmulator.SteamMusicRemote.SetCurrentQueueEntry(nID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_QueueDidChange()
    {
        Write("SteamAPI_ISteamMusicRemote_QueueDidChange");
        return SteamEmulator.SteamMusicRemote.QueueDidChange(SteamEmulator.SteamFriends.MemoryAddress);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_PlaylistWillChange()
    {
        Write("SteamAPI_ISteamMusicRemote_PlaylistWillChange");
        return SteamEmulator.SteamMusicRemote.PlaylistWillChange(SteamEmulator.SteamFriends.MemoryAddress);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_ResetPlaylistEntries()
    {
        Write("SteamAPI_ISteamMusicRemote_ResetPlaylistEntries");
        return SteamEmulator.SteamMusicRemote.ResetPlaylistEntries(SteamEmulator.SteamFriends.MemoryAddress);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_SetPlaylistEntry(int nID, int nPosition, string pchEntryText)
    {
        Write("SteamAPI_ISteamMusicRemote_SetPlaylistEntry");
        return SteamEmulator.SteamMusicRemote.SetPlaylistEntry(nID, nPosition, pchEntryText);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_SetCurrentPlaylistEntry(int nID)
    {
        Write("SteamAPI_ISteamMusicRemote_SetCurrentPlaylistEntry");
        return SteamEmulator.SteamMusicRemote.SetCurrentPlaylistEntry(nID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_PlaylistDidChange()
    {
        Write("SteamAPI_ISteamMusicRemote_PlaylistDidChange");
        return SteamEmulator.SteamMusicRemote.PlaylistDidChange(SteamEmulator.SteamFriends.MemoryAddress);
    }

}

