using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamMusicRemote : ISteamInterface
    {
        public static SteamMusicRemote Instance;

        public SteamMusicRemote()
        {
            Instance = this;
            InterfaceName = "SteamMusicRemote";
            InterfaceVersion = "STEAMMUSICREMOTE_INTERFACE_VERSION001";
        }

        public bool RegisterSteamMusicRemote(string pchName)
        {
            Write($"RegisterSteamMusicRemote");
            return false;
        }

        public bool DeregisterSteamMusicRemote()
        {
            Write($"DeregisterSteamMusicRemote");
            return false;
        }

        public bool BIsCurrentMusicRemote()
        {
            Write($"BIsCurrentMusicRemote");
            return false;
        }

        public bool BActivationSuccess(bool bValue)
        {
            Write($"xxx");
            return false;
        }

        public bool SetDisplayName(string pchDisplayName)
        {
            Write($"SetDisplayName");
            return false;
        }

        public bool SetPNGIcon_64x64(IntPtr pvBuffer, uint cbBufferLength)
        {
            Write($"xxx");
            return false;
        }

        public bool EnablePlayPrevious(bool bValue)
        {
            Write($"EnablePlayPrevious");
            return false;
        }

        public bool EnablePlayNext(bool bValue)
        {
            Write($"EnablePlayNext");
            return false;
        }

        public bool EnableShuffled(bool bValue)
        {
            Write($"EnableShuffled");
            return false;
        }

        public bool EnableLooped(bool bValue)
        {
            Write($"EnableLooped");
            return false;
        }

        public bool EnableQueue(bool bValue)
        {
            Write($"EnableQueue");
            return false;
        }

        public bool EnablePlaylists(bool bValue)
        {
            Write($"EnablePlaylists");
            return false;
        }

        public bool UpdatePlaybackStatus(int nStatus)
        {
            Write($"UpdatePlaybackStatus");
            return false;
        }

        public bool UpdateShuffled(bool bValue)
        {
            Write($"UpdateShuffled");
            return false;
        }

        public bool UpdateLooped(bool bValue)
        {
            Write($"UpdateLooped");
            return false;
        }

        public bool UpdateVolume(float flValue)
        {
            Write($"UpdateVolume");
            return false;
        }

        public bool CurrentEntryWillChange()
        {
            Write($"CurrentEntryWillChange");
            return false;
        }

        public bool CurrentEntryIsAvailable(bool bAvailable)
        {
            Write($"CurrentEntryIsAvailable");
            return false;
        }

        public bool UpdateCurrentEntryText(string pchText)
        {
            Write($"UpdateCurrentEntryText");
            return false;
        }

        public bool UpdateCurrentEntryElapsedSeconds(int nValue)
        {
            Write($"UpdateCurrentEntryElapsedSeconds");
            return false;
        }

        public bool UpdateCurrentEntryCoverArt(IntPtr pvBuffer, uint cbBufferLength)
        {
            Write($"UpdateCurrentEntryCoverArt");
            return false;
        }

        public bool CurrentEntryDidChange()
        {
            Write($"CurrentEntryDidChange");
            return false;
        }

        public bool QueueWillChange()
        {
            Write($"QueueWillChange");
            return false;
        }

        public bool ResetQueueEntries()
        {
            Write($"ResetQueueEntries");
            return false;
        }

        public bool SetQueueEntry(int nID, int nPosition, string pchEntryText)
        {
            Write($"SetQueueEntry");
            return false;
        }

        public bool SetCurrentQueueEntry(int nID)
        {
            Write($"SetCurrentQueueEntry");
            return false;
        }

        public bool QueueDidChange()
        {
            Write($"QueueDidChange");
            return false;
        }

        public bool PlaylistWillChange()
        {
            Write($"PlaylistWillChange");
            return false;
        }

        public bool ResetPlaylistEntries()
        {
            Write($"ResetPlaylistEntries");
            return false;
        }

        public bool SetPlaylistEntry(int nID, int nPosition, string pchEntryText)
        {
            Write($"SetPlaylistEntry");
            return false;
        }

        public bool SetCurrentPlaylistEntry(int nID)
        {
            Write($"SetCurrentPlaylistEntry");
            return false;
        }

        public bool PlaylistDidChange()
        {
            Write($"PlaylistDidChange");
            return false;
        }
    }
}