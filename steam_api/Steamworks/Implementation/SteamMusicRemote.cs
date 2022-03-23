using System;
using Core.Interface;
using SKYNET;
using SKYNET.Interface;

//[Map("STEAMMUSICREMOTE_INTERFACE_VERSION")]
//[Map("SteamMusicRemote")]
public class SteamMusicRemote : IBaseInterface, ISteamMusicRemote
{
    public bool RegisterSteamMusicRemote(string pchName)
    {
        return false;
    }

    public bool DeregisterSteamMusicRemote(IntPtr _)
    {
        return false;
    }

    public bool BIsCurrentMusicRemote(IntPtr _)
    {
        return false;
    }

    public bool BActivationSuccess(bool bValue)
    {
        return false;
    }

    public bool SetDisplayName(string pchDisplayName)
    {
        return false;
    }

    public bool SetPNGIcon_64x64(IntPtr pvBuffer, uint cbBufferLength)
    {
        return false;
    }

    public bool EnablePlayPrevious(bool bValue)
    {
        return false;
    }

    public bool EnablePlayNext(bool bValue)
    {
        return false;
    }

    public bool EnableShuffled(bool bValue)
    {
        return false;
    }

    public bool EnableLooped(bool bValue)
    {
        return false;
    }

    public bool EnableQueue(bool bValue)
    {
        return false;
    }

    public bool EnablePlaylists(bool bValue)
    {
        return false;
    }

    public bool UpdatePlaybackStatus(AudioPlayback_Status nStatus)
    {
        return false;
    }

    public bool UpdateShuffled(bool bValue)
    {
        return false;
    }

    public bool UpdateLooped(bool bValue)
    {
        return false;
    }

    public bool UpdateVolume(float flValue) // volume is between 0.0 and 1.0 
    {
        return false;
    }

    public bool CurrentEntryWillChange(IntPtr _)
    {
        return false;
    }

    public bool CurrentEntryIsAvailable(bool bAvailable)
    {
        return false;
    }

    public bool UpdateCurrentEntryText(string pchText)
    {
        return false;
    }

    public bool UpdateCurrentEntryElapsedSeconds(int nValue)
    {
        return false;
    }

    public bool UpdateCurrentEntryCoverArt(IntPtr pvBuffer, uint cbBufferLength)
    {
        return false;
    }

    public bool CurrentEntryDidChange(IntPtr _)
    {
        return false;
    }

    public bool QueueWillChange(IntPtr _)
    {
        return false;
    }

    public bool ResetQueueEntries(IntPtr _)
    {
        return false;
    }

    public bool SetQueueEntry(int nID, int nPosition, string pchEntryText)
    {
        return false;
    }

    public bool SetCurrentQueueEntry(int nID)
    {
        return false;
    }

    public bool QueueDidChange(IntPtr _)
    {
        return false;
    }

    public bool PlaylistWillChange(IntPtr _)
    {
        return false;
    }

    public bool ResetPlaylistEntries(IntPtr _)
    {
        return false;
    }

    public bool SetPlaylistEntry(int nID, int nPosition, string pchEntryText)
    {
        return false;
    }

    public bool SetCurrentPlaylistEntry(int nID)
    {
        return false;
    }

    public bool PlaylistDidChange(IntPtr _)
    {
        return false;
    }

    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}