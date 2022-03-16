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
        bool DeregisterSteamMusicRemote();
        bool BIsCurrentMusicRemote();
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
        bool CurrentEntryWillChange();
        bool CurrentEntryIsAvailable(bool bAvailable);
        bool UpdateCurrentEntryText(string pchText);
        bool UpdateCurrentEntryElapsedSeconds(int nValue);
        bool UpdateCurrentEntryCoverArt(IntPtr pvBuffer, uint cbBufferLength);
        bool CurrentEntryDidChange();

        // Queue
        bool QueueWillChange();
        bool ResetQueueEntries();
        bool SetQueueEntry(int nID, int nPosition, string pchEntryText);
        bool SetCurrentQueueEntry(int nID);
        bool QueueDidChange();

        // Playlist
        bool PlaylistWillChange();
        bool ResetPlaylistEntries();
        bool SetPlaylistEntry(int nID, int nPosition, string pchEntryText);
        bool SetCurrentPlaylistEntry(int nID);
        bool PlaylistDidChange();
    }
}
