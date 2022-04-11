using System;

namespace SKYNET.Interface
{
    [Interface("STEAMMUSICREMOTE_INTERFACE_VERSION001")]
    public class SteamMusicRemote001 : ISteamInterface
    {
        public bool RegisterSteamMusicRemote(IntPtr _, string pchName)
        {
            return SteamEmulator.SteamMusicRemote.RegisterSteamMusicRemote(pchName);
        }

        public bool DeregisterSteamMusicRemote(IntPtr _)
        {
            return SteamEmulator.SteamMusicRemote.DeregisterSteamMusicRemote(_);
        }

        public bool BIsCurrentMusicRemote(IntPtr _)
        {
            return SteamEmulator.SteamMusicRemote.BIsCurrentMusicRemote(_);
        }

        public bool BActivationSuccess(IntPtr _, bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.BActivationSuccess(bValue);
        }

        public bool SetDisplayName(IntPtr _, string pchDisplayName)
        {
            return SteamEmulator.SteamMusicRemote.SetDisplayName(pchDisplayName);
        }

        public bool SetPNGIcon_64x64(IntPtr _, IntPtr pvBuffer, uint cbBufferLength)
        {
            return SteamEmulator.SteamMusicRemote.SetPNGIcon_64x64(pvBuffer, cbBufferLength);
        }

        public bool EnablePlayPrevious(IntPtr _, bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.EnablePlayPrevious(bValue);
        }

        public bool EnablePlayNext(IntPtr _, bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.EnablePlayNext(bValue);
        }

        public bool EnableShuffled(IntPtr _, bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.EnableShuffled(bValue);
        }

        public bool EnableLooped(IntPtr _, bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.EnableLooped(bValue);
        }

        public bool EnableQueue(IntPtr _, bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.EnableQueue(bValue);
        }

        public bool EnablePlaylists(IntPtr _, bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.EnablePlaylists(bValue);
        }

        public bool UpdatePlaybackStatus(IntPtr _, int nStatus)
        {
            return SteamEmulator.SteamMusicRemote.UpdatePlaybackStatus(nStatus);
        }

        public bool UpdateShuffled(IntPtr _, bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.UpdateShuffled(bValue);
        }

        public bool UpdateLooped(IntPtr _, bool bValue)
        {
            return SteamEmulator.SteamMusicRemote.UpdateLooped(bValue);
        }

        public bool UpdateVolume(IntPtr _, float flValue)  // volume is between 0.0 and 1.0
        {
            return SteamEmulator.SteamMusicRemote.UpdateVolume(flValue);
        }

        public bool CurrentEntryWillChange(IntPtr _)
        {
            return SteamEmulator.SteamMusicRemote.CurrentEntryWillChange(_);
        }

        public bool CurrentEntryIsAvailable(IntPtr _, bool bAvailable)
        {
            return SteamEmulator.SteamMusicRemote.CurrentEntryIsAvailable(bAvailable);
        }

        public bool UpdateCurrentEntryText(IntPtr _, string pchText)
        {
            return SteamEmulator.SteamMusicRemote.UpdateCurrentEntryText(pchText);
        }

        public bool UpdateCurrentEntryElapsedSeconds(IntPtr _, int nValue)
        {
            return SteamEmulator.SteamMusicRemote.UpdateCurrentEntryElapsedSeconds(nValue);
        }

        public bool UpdateCurrentEntryCoverArt(IntPtr _, IntPtr pvBuffer, uint cbBufferLength)
        {
            return SteamEmulator.SteamMusicRemote.UpdateCurrentEntryCoverArt(pvBuffer, cbBufferLength);
        }

        public bool CurrentEntryDidChange(IntPtr _)
        {
            return SteamEmulator.SteamMusicRemote.CurrentEntryDidChange(_);
        }

        public bool QueueWillChange(IntPtr _)
        {
            return SteamEmulator.SteamMusicRemote.QueueWillChange(_);
        }

        public bool ResetQueueEntries(IntPtr _)
        {
            return SteamEmulator.SteamMusicRemote.ResetQueueEntries(_);
        }

        public bool SetQueueEntry(IntPtr _, int nID, int nPosition, string pchEntryText)
        {
            return SteamEmulator.SteamMusicRemote.SetQueueEntry(nID, nPosition, pchEntryText);
        }

        public bool SetCurrentQueueEntry(IntPtr _, int nID)
        {
            return SteamEmulator.SteamMusicRemote.SetCurrentQueueEntry(nID);
        }

        public bool QueueDidChange(IntPtr _)
        {
            return SteamEmulator.SteamMusicRemote.QueueDidChange(_);
        }

        public bool PlaylistWillChange(IntPtr _)
        {
            return SteamEmulator.SteamMusicRemote.PlaylistWillChange(_);
        }

        public bool ResetPlaylistEntries(IntPtr _)
        {
            return SteamEmulator.SteamMusicRemote.ResetPlaylistEntries(_);
        }

        public bool SetPlaylistEntry(IntPtr _, int nID, int nPosition, string pchEntryText)
        {
            return SteamEmulator.SteamMusicRemote.SetPlaylistEntry(nID, nPosition, pchEntryText);
        }

        public bool SetCurrentPlaylistEntry(IntPtr _, int nID)
        {
            return SteamEmulator.SteamMusicRemote.SetCurrentPlaylistEntry(nID);
        }

        public bool PlaylistDidChange(IntPtr _)
        {
            return SteamEmulator.SteamMusicRemote.PlaylistDidChange(_);
        }


    }
}
