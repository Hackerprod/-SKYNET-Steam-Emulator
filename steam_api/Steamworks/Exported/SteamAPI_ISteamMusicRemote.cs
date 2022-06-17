using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamMusicRemote
    {
        static SteamAPI_ISteamMusicRemote()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_RegisterSteamMusicRemote(IntPtr _, string pchName)
        {
            Write("SteamAPI_ISteamMusicRemote_RegisterSteamMusicRemote");
            return SteamEmulator.SteamMusicRemote.RegisterSteamMusicRemote(pchName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_DeregisterSteamMusicRemote(IntPtr _)
        {
            Write("SteamAPI_ISteamMusicRemote_DeregisterSteamMusicRemote");
            return SteamEmulator.SteamMusicRemote.DeregisterSteamMusicRemote();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_BIsCurrentMusicRemote(IntPtr _)
        {
            Write("SteamAPI_ISteamMusicRemote_BIsCurrentMusicRemote");
            return SteamEmulator.SteamMusicRemote.BIsCurrentMusicRemote();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_BActivationSuccess(IntPtr _, bool bValue)
        {
            Write("SteamAPI_ISteamMusicRemote_BActivationSuccess");
            return SteamEmulator.SteamMusicRemote.BActivationSuccess(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_SetDisplayName(IntPtr _, string pchDisplayName)
        {
            Write("SteamAPI_ISteamMusicRemote_SetDisplayName");
            return SteamEmulator.SteamMusicRemote.SetDisplayName(pchDisplayName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_SetPNGIcon_64x64(IntPtr _, IntPtr pvBuffer, uint cbBufferLength)
        {
            Write("SteamAPI_ISteamMusicRemote_SetPNGIcon_64x64");
            return SteamEmulator.SteamMusicRemote.SetPNGIcon_64x64(pvBuffer, cbBufferLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_EnablePlayPrevious(IntPtr _, bool bValue)
        {
            Write("SteamAPI_ISteamMusicRemote_EnablePlayPrevious");
            return SteamEmulator.SteamMusicRemote.EnablePlayPrevious(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_EnablePlayNext(IntPtr _, bool bValue)
        {
            Write("SteamAPI_ISteamMusicRemote_EnablePlayNext");
            return SteamEmulator.SteamMusicRemote.EnablePlayNext(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_EnableShuffled(IntPtr _, bool bValue)
        {
            Write("SteamAPI_ISteamMusicRemote_EnableShuffled");
            return SteamEmulator.SteamMusicRemote.EnableShuffled(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_EnableLooped(IntPtr _, bool bValue)
        {
            Write("SteamAPI_ISteamMusicRemote_EnableLooped");
            return SteamEmulator.SteamMusicRemote.EnableLooped(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_EnableQueue(IntPtr _, bool bValue)
        {
            Write("SteamAPI_ISteamMusicRemote_EnableQueue");
            return SteamEmulator.SteamMusicRemote.EnableQueue(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_EnablePlaylists(IntPtr _, bool bValue)
        {
            Write("SteamAPI_ISteamMusicRemote_EnablePlaylists");
            return SteamEmulator.SteamMusicRemote.EnablePlaylists(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_UpdatePlaybackStatus(IntPtr _, int nStatus)
        {
            Write("SteamAPI_ISteamMusicRemote_UpdatePlaybackStatus");
            return SteamEmulator.SteamMusicRemote.UpdatePlaybackStatus(nStatus);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_UpdateShuffled(IntPtr _, bool bValue)
        {
            Write("SteamAPI_ISteamMusicRemote_UpdateShuffled");
            return SteamEmulator.SteamMusicRemote.UpdateShuffled(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_UpdateLooped(IntPtr _, bool bValue)
        {
            Write("SteamAPI_ISteamMusicRemote_UpdateLooped");
            return SteamEmulator.SteamMusicRemote.UpdateLooped(bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_UpdateVolume(IntPtr _, float flValue)  
        {
            Write("SteamAPI_ISteamMusicRemote_UpdateVolume");
            return SteamEmulator.SteamMusicRemote.UpdateVolume(flValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_CurrentEntryWillChange(IntPtr _)
        {
            Write("SteamAPI_ISteamMusicRemote_CurrentEntryWillChange");
            return SteamEmulator.SteamMusicRemote.CurrentEntryWillChange();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_CurrentEntryIsAvailable(IntPtr _, bool bAvailable)
        {
            Write("SteamAPI_ISteamMusicRemote_CurrentEntryIsAvailable");
            return SteamEmulator.SteamMusicRemote.CurrentEntryIsAvailable(bAvailable);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_UpdateCurrentEntryText(IntPtr _, string pchText)
        {
            Write("SteamAPI_ISteamMusicRemote_UpdateCurrentEntryText");
            return SteamEmulator.SteamMusicRemote.UpdateCurrentEntryText(pchText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_UpdateCurrentEntryElapsedSeconds(IntPtr _, int nValue)
        {
            Write("SteamAPI_ISteamMusicRemote_UpdateCurrentEntryElapsedSeconds");
            return SteamEmulator.SteamMusicRemote.UpdateCurrentEntryElapsedSeconds(nValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_UpdateCurrentEntryCoverArt(IntPtr _, IntPtr pvBuffer, uint cbBufferLength)
        {
            Write("SteamAPI_ISteamMusicRemote_UpdateCurrentEntryCoverArt");
            return SteamEmulator.SteamMusicRemote.UpdateCurrentEntryCoverArt(pvBuffer, cbBufferLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_CurrentEntryDidChange(IntPtr _)
        {
            Write("SteamAPI_ISteamMusicRemote_CurrentEntryDidChange");
            return SteamEmulator.SteamMusicRemote.CurrentEntryDidChange();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_QueueWillChange(IntPtr _)
        {
            Write("SteamAPI_ISteamMusicRemote_QueueWillChange");
            return SteamEmulator.SteamMusicRemote.QueueWillChange();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_ResetQueueEntries(IntPtr _)
        {
            Write("SteamAPI_ISteamMusicRemote_ResetQueueEntries");
            return SteamEmulator.SteamMusicRemote.ResetQueueEntries();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_SetQueueEntry(IntPtr _, int nID, int nPosition, string pchEntryText)
        {
            Write("SteamAPI_ISteamMusicRemote_SetQueueEntry");
            return SteamEmulator.SteamMusicRemote.SetQueueEntry(nID, nPosition, pchEntryText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_SetCurrentQueueEntry(IntPtr _, int nID)
        {
            Write("SteamAPI_ISteamMusicRemote_SetCurrentQueueEntry");
            return SteamEmulator.SteamMusicRemote.SetCurrentQueueEntry(nID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_QueueDidChange(IntPtr _)
        {
            Write("SteamAPI_ISteamMusicRemote_QueueDidChange");
            return SteamEmulator.SteamMusicRemote.QueueDidChange();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_PlaylistWillChange(IntPtr _)
        {
            Write("SteamAPI_ISteamMusicRemote_PlaylistWillChange");
            return SteamEmulator.SteamMusicRemote.PlaylistWillChange();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_ResetPlaylistEntries(IntPtr _)
        {
            Write("SteamAPI_ISteamMusicRemote_ResetPlaylistEntries");
            return SteamEmulator.SteamMusicRemote.ResetPlaylistEntries();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_SetPlaylistEntry(IntPtr _, int nID, int nPosition, string pchEntryText)
        {
            Write("SteamAPI_ISteamMusicRemote_SetPlaylistEntry");
            return SteamEmulator.SteamMusicRemote.SetPlaylistEntry(nID, nPosition, pchEntryText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_SetCurrentPlaylistEntry(IntPtr _, int nID)
        {
            Write("SteamAPI_ISteamMusicRemote_SetCurrentPlaylistEntry");
            return SteamEmulator.SteamMusicRemote.SetCurrentPlaylistEntry(nID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMusicRemote_PlaylistDidChange(IntPtr _)
        {
            Write("SteamAPI_ISteamMusicRemote_PlaylistDidChange");
            return SteamEmulator.SteamMusicRemote.PlaylistDidChange();
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

