using System;
using SKYNET.Steamworks.Interfaces;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamMusicRemote : ISteamInterface
    {
        public static SteamMusicRemote Instance;

        private bool IsRegistered;
        private bool IsActive;
        private bool CurrentEntryAvailable;
        private int CurrentQueueEntryId;
        private int CurrentPlaylistEntryId;
        private int PlaybackStatus = (int)AudioPlayback_Status.AudioPlayback_Idle;
        private float Volume = 1.0f;
        private string Name = string.Empty;
        private string DisplayName = string.Empty;

        public SteamMusicRemote()
        {
            Instance = this;
            InterfaceName = "SteamMusicRemote";
            InterfaceVersion = "STEAMMUSICREMOTE_INTERFACE_VERSION001";
        }

        public bool RegisterSteamMusicRemote(string pchName)
        {
            Write($"RegisterSteamMusicRemote {pchName}");

            if (string.IsNullOrWhiteSpace(pchName))
            {
                return false;
            }

            Name = pchName;
            DisplayName = pchName;
            IsRegistered = true;
            return true;
        }

        public bool DeregisterSteamMusicRemote()
        {
            Write($"DeregisterSteamMusicRemote");
            IsRegistered = false;
            IsActive = false;
            CurrentEntryAvailable = false;
            CurrentQueueEntryId = 0;
            CurrentPlaylistEntryId = 0;
            PlaybackStatus = (int)AudioPlayback_Status.AudioPlayback_Idle;
            Name = string.Empty;
            DisplayName = string.Empty;
            return true;
        }

        public bool BIsCurrentMusicRemote()
        {
            Write($"BIsCurrentMusicRemote");
            return IsRegistered && IsActive;
        }

        public bool BActivationSuccess(bool bValue)
        {
            Write($"BActivationSuccess {bValue}");
            IsActive = IsRegistered && bValue;
            return IsRegistered;
        }

        public bool SetDisplayName(string pchDisplayName)
        {
            Write($"SetDisplayName {pchDisplayName}");

            if (!IsRegistered || string.IsNullOrWhiteSpace(pchDisplayName))
            {
                return false;
            }

            DisplayName = pchDisplayName;
            return true;
        }

        public bool SetPNGIcon_64x64(IntPtr pvBuffer, uint cbBufferLength)
        {
            Write($"SetPNGIcon_64x64 {cbBufferLength}");
            return IsRegistered && pvBuffer != IntPtr.Zero && cbBufferLength > 0;
        }

        public bool EnablePlayPrevious(bool bValue)
        {
            Write($"EnablePlayPrevious {bValue}");
            return IsRegistered;
        }

        public bool EnablePlayNext(bool bValue)
        {
            Write($"EnablePlayNext {bValue}");
            return IsRegistered;
        }

        public bool EnableShuffled(bool bValue)
        {
            Write($"EnableShuffled {bValue}");
            return IsRegistered;
        }

        public bool EnableLooped(bool bValue)
        {
            Write($"EnableLooped {bValue}");
            return IsRegistered;
        }

        public bool EnableQueue(bool bValue)
        {
            Write($"EnableQueue {bValue}");
            return IsRegistered;
        }

        public bool EnablePlaylists(bool bValue)
        {
            Write($"EnablePlaylists {bValue}");
            return IsRegistered;
        }

        public bool UpdatePlaybackStatus(int nStatus)
        {
            Write($"UpdatePlaybackStatus {nStatus}");

            if (!IsRegistered)
            {
                return false;
            }

            PlaybackStatus = nStatus;
            return true;
        }

        public bool UpdateShuffled(bool bValue)
        {
            Write($"UpdateShuffled {bValue}");
            return IsRegistered;
        }

        public bool UpdateLooped(bool bValue)
        {
            Write($"UpdateLooped {bValue}");
            return IsRegistered;
        }

        public bool UpdateVolume(float flValue)
        {
            Write($"UpdateVolume {flValue}");

            if (!IsRegistered)
            {
                return false;
            }

            Volume = ClampVolume(flValue);
            return true;
        }

        public bool CurrentEntryWillChange()
        {
            Write($"CurrentEntryWillChange");
            return IsRegistered;
        }

        public bool CurrentEntryIsAvailable(bool bAvailable)
        {
            Write($"CurrentEntryIsAvailable {bAvailable}");

            if (!IsRegistered)
            {
                return false;
            }

            CurrentEntryAvailable = bAvailable;
            return true;
        }

        public bool UpdateCurrentEntryText(string pchText)
        {
            Write($"UpdateCurrentEntryText {pchText}");
            return IsRegistered;
        }

        public bool UpdateCurrentEntryElapsedSeconds(int nValue)
        {
            Write($"UpdateCurrentEntryElapsedSeconds {nValue}");
            return IsRegistered && nValue >= 0;
        }

        public bool UpdateCurrentEntryCoverArt(IntPtr pvBuffer, uint cbBufferLength)
        {
            Write($"UpdateCurrentEntryCoverArt {cbBufferLength}");
            return IsRegistered && pvBuffer != IntPtr.Zero && cbBufferLength > 0;
        }

        public bool CurrentEntryDidChange()
        {
            Write($"CurrentEntryDidChange");
            return IsRegistered;
        }

        public bool QueueWillChange()
        {
            Write($"QueueWillChange");
            return IsRegistered;
        }

        public bool ResetQueueEntries()
        {
            Write($"ResetQueueEntries");
            if (!IsRegistered)
            {
                return false;
            }

            CurrentQueueEntryId = 0;
            return true;
        }

        public bool SetQueueEntry(int nID, int nPosition, string pchEntryText)
        {
            Write($"SetQueueEntry {nID} {nPosition} {pchEntryText}");
            return IsRegistered && nID >= 0 && nPosition >= 0;
        }

        public bool SetCurrentQueueEntry(int nID)
        {
            Write($"SetCurrentQueueEntry {nID}");

            if (!IsRegistered || nID < 0)
            {
                return false;
            }

            CurrentQueueEntryId = nID;
            return true;
        }

        public bool QueueDidChange()
        {
            Write($"QueueDidChange");
            return IsRegistered;
        }

        public bool PlaylistWillChange()
        {
            Write($"PlaylistWillChange");
            return IsRegistered;
        }

        public bool ResetPlaylistEntries()
        {
            Write($"ResetPlaylistEntries");
            if (!IsRegistered)
            {
                return false;
            }

            CurrentPlaylistEntryId = 0;
            return true;
        }

        public bool SetPlaylistEntry(int nID, int nPosition, string pchEntryText)
        {
            Write($"SetPlaylistEntry {nID} {nPosition} {pchEntryText}");
            return IsRegistered && nID >= 0 && nPosition >= 0;
        }

        public bool SetCurrentPlaylistEntry(int nID)
        {
            Write($"SetCurrentPlaylistEntry {nID}");

            if (!IsRegistered || nID < 0)
            {
                return false;
            }

            CurrentPlaylistEntryId = nID;
            return true;
        }

        public bool PlaylistDidChange()
        {
            Write($"PlaylistDidChange");
            return IsRegistered;
        }

        private static float ClampVolume(float value)
        {
            if (value < 0)
            {
                return 0;
            }

            if (value > 1)
            {
                return 1;
            }

            return value;
        }
    }
}
