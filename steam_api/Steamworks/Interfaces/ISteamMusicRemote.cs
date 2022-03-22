using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamMusicRemote
    {
        // Service Definition
        bool RegisterSteamMusicRemote(string pchName);
        bool DeregisterSteamMusicRemote(IntPtr _);
        bool BIsCurrentMusicRemote(IntPtr _);
        bool BActivationSuccess(bool bValue);

        bool SetDisplayName(string pchDisplayName);
        bool SetPNGIcon_64x64(IntPtr pvBuffer, uint cbBufferLength);

        // Abilities for the user interface
        bool EnablePlayPrevious(bool bValue);
        bool EnablePlayNext(bool bValue);
        bool EnableShuffled(bool bValue);
        bool EnableLooped(bool bValue);
        bool EnableQueue(bool bValue);
        bool EnablePlaylists(bool bValue);

        // Status
        bool UpdatePlaybackStatus(AudioPlayback_Status nStatus);
        bool UpdateShuffled(bool bValue);
        bool UpdateLooped(bool bValue);
        bool UpdateVolume(float flValue); // volume is between 0.0 and 1.0

        // Current Entry
        bool CurrentEntryWillChange(IntPtr _);
        bool CurrentEntryIsAvailable(bool bAvailable);
        bool UpdateCurrentEntryText(string pchText);
        bool UpdateCurrentEntryElapsedSeconds(int nValue);
        bool UpdateCurrentEntryCoverArt(IntPtr pvBuffer, uint cbBufferLength);
        bool CurrentEntryDidChange(IntPtr _);

        // Queue
        bool QueueWillChange(IntPtr _);
        bool ResetQueueEntries(IntPtr _);
        bool SetQueueEntry(int nID, int nPosition, string pchEntryText);
        bool SetCurrentQueueEntry(int nID);
        bool QueueDidChange(IntPtr _);

        // Playlist
        bool PlaylistWillChange(IntPtr _);
        bool ResetPlaylistEntries(IntPtr _);
        bool SetPlaylistEntry(int nID, int nPosition, string pchEntryText);
        bool SetCurrentPlaylistEntry(int nID);
        bool PlaylistDidChange(IntPtr _);
    }
}
