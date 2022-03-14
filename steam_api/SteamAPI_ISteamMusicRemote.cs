using SKYNET.Interface;
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
        return SteamClient.SteamMusicRemote.RegisterSteamMusicRemote(pchName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_DeregisterSteamMusicRemote()
    {
        Write("SteamAPI_ISteamMusicRemote_DeregisterSteamMusicRemote");
        return SteamClient.SteamMusicRemote.DeregisterSteamMusicRemote();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_BIsCurrentMusicRemote()
    {
        Write("SteamAPI_ISteamMusicRemote_BIsCurrentMusicRemote");
        return SteamClient.SteamMusicRemote.BIsCurrentMusicRemote();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_BActivationSuccess(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_BActivationSuccess");
        return SteamClient.SteamMusicRemote.BActivationSuccess(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_SetDisplayName(string pchDisplayName)
    {
        Write("SteamAPI_ISteamMusicRemote_SetDisplayName");
        return SteamClient.SteamMusicRemote.SetDisplayName(pchDisplayName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_SetPNGIcon_64x64(IntPtr pvBuffer, uint cbBufferLength)
    {
        Write("SteamAPI_ISteamMusicRemote_SetPNGIcon_64x64");
        return SteamClient.SteamMusicRemote.SetPNGIcon_64x64(pvBuffer, cbBufferLength);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_EnablePlayPrevious(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_EnablePlayPrevious");
        return SteamClient.SteamMusicRemote.EnablePlayPrevious(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_EnablePlayNext(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_EnablePlayNext");
        return SteamClient.SteamMusicRemote.EnablePlayNext(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_EnableShuffled(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_EnableShuffled");
        return SteamClient.SteamMusicRemote.EnableShuffled(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_EnableLooped(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_EnableLooped");
        return SteamClient.SteamMusicRemote.EnableLooped(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_EnableQueue(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_EnableQueue");
        return SteamClient.SteamMusicRemote.EnableQueue(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_EnablePlaylists(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_EnablePlaylists");
        return SteamClient.SteamMusicRemote.EnablePlaylists(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_UpdatePlaybackStatus(AudioPlayback_Status nStatus)
    {
        Write("SteamAPI_ISteamMusicRemote_UpdatePlaybackStatus");
        return SteamClient.SteamMusicRemote.UpdatePlaybackStatus(nStatus);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_UpdateShuffled(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_UpdateShuffled");
        return SteamClient.SteamMusicRemote.UpdateShuffled(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_UpdateLooped(bool bValue)
    {
        Write("SteamAPI_ISteamMusicRemote_UpdateLooped");
        return SteamClient.SteamMusicRemote.UpdateLooped(bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_UpdateVolume(float flValue) // volume is between 0.0 and 1.0 
    {
        Write("SteamAPI_ISteamMusicRemote_UpdateVolume");
        return SteamClient.SteamMusicRemote.UpdateVolume(flValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_CurrentEntryWillChange()
    {
        Write("SteamAPI_ISteamMusicRemote_CurrentEntryWillChange");
        return SteamClient.SteamMusicRemote.CurrentEntryWillChange();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_CurrentEntryIsAvailable(bool bAvailable)
    {
        Write("SteamAPI_ISteamMusicRemote_CurrentEntryIsAvailable");
        return SteamClient.SteamMusicRemote.CurrentEntryIsAvailable(bAvailable);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_UpdateCurrentEntryText(string pchText)
    {
        Write("SteamAPI_ISteamMusicRemote_UpdateCurrentEntryText");
        return SteamClient.SteamMusicRemote.UpdateCurrentEntryText(pchText);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_UpdateCurrentEntryElapsedSeconds(int nValue)
    {
        Write("SteamAPI_ISteamMusicRemote_UpdateCurrentEntryElapsedSeconds");
        return SteamClient.SteamMusicRemote.UpdateCurrentEntryElapsedSeconds(nValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_UpdateCurrentEntryCoverArt(IntPtr pvBuffer, uint cbBufferLength)
    {
        Write("SteamAPI_ISteamMusicRemote_UpdateCurrentEntryCoverArt");
        return SteamClient.SteamMusicRemote.UpdateCurrentEntryCoverArt(pvBuffer, cbBufferLength);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_CurrentEntryDidChange()
    {
        Write("SteamAPI_ISteamMusicRemote_CurrentEntryDidChange");
        return SteamClient.SteamMusicRemote.CurrentEntryDidChange();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_QueueWillChange()
    {
        Write("SteamAPI_ISteamMusicRemote_QueueWillChange");
        return SteamClient.SteamMusicRemote.QueueWillChange();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_ResetQueueEntries()
    {
        Write("SteamAPI_ISteamMusicRemote_ResetQueueEntries");
        return SteamClient.SteamMusicRemote.ResetQueueEntries();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_SetQueueEntry(int nID, int nPosition, string pchEntryText)
    {
        Write("SteamAPI_ISteamMusicRemote_SetQueueEntry");
        return SteamClient.SteamMusicRemote.SetQueueEntry(nID, nPosition, pchEntryText);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_SetCurrentQueueEntry(int nID)
    {
        Write("SteamAPI_ISteamMusicRemote_SetCurrentQueueEntry");
        return SteamClient.SteamMusicRemote.SetCurrentQueueEntry(nID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_QueueDidChange()
    {
        Write("SteamAPI_ISteamMusicRemote_QueueDidChange");
        return SteamClient.SteamMusicRemote.QueueDidChange();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_PlaylistWillChange()
    {
        Write("SteamAPI_ISteamMusicRemote_PlaylistWillChange");
        return SteamClient.SteamMusicRemote.PlaylistWillChange();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_ResetPlaylistEntries()
    {
        Write("SteamAPI_ISteamMusicRemote_ResetPlaylistEntries");
        return SteamClient.SteamMusicRemote.ResetPlaylistEntries();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_SetPlaylistEntry(int nID, int nPosition, string pchEntryText)
    {
        Write("SteamAPI_ISteamMusicRemote_SetPlaylistEntry");
        return SteamClient.SteamMusicRemote.SetPlaylistEntry(nID, nPosition, pchEntryText);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_SetCurrentPlaylistEntry(int nID)
    {
        Write("SteamAPI_ISteamMusicRemote_SetCurrentPlaylistEntry");
        return SteamClient.SteamMusicRemote.SetCurrentPlaylistEntry(nID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamMusicRemote_PlaylistDidChange()
    {
        Write("SteamAPI_ISteamMusicRemote_PlaylistDidChange");
        return SteamClient.SteamMusicRemote.PlaylistDidChange();
    }

}

