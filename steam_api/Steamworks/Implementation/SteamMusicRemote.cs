using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    [StructLayout(LayoutKind.Sequential)]
    public class SteamMusicRemote : ISteamInterface
    {
        public bool RegisterSteamMusicRemote(string pchName)
        {
            Write($"RegisterSteamMusicRemote");
            return false;
        }

        public bool DeregisterSteamMusicRemote(IntPtr _)
        {
            Write($"DeregisterSteamMusicRemote");
            return false;
        }

        public bool BIsCurrentMusicRemote(IntPtr _)
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

        public bool CurrentEntryWillChange(IntPtr _)
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

        public bool CurrentEntryDidChange(IntPtr _)
        {
            Write($"CurrentEntryDidChange");
            return false;
        }

        public bool QueueWillChange(IntPtr _)
        {
            Write($"QueueWillChange");
            return false;
        }

        public bool ResetQueueEntries(IntPtr _)
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

        public bool QueueDidChange(IntPtr _)
        {
            Write($"QueueDidChange");
            return false;
        }

        public bool PlaylistWillChange(IntPtr _)
        {
            Write($"PlaylistWillChange");
            return false;
        }

        public bool ResetPlaylistEntries(IntPtr _)
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

        public bool PlaylistDidChange(IntPtr _)
        {
            Write($"PlaylistDidChange");
            return false;
        }

        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamMusicRemote()
        {
            InterfaceVersion = "SteamMusicRemote";
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}