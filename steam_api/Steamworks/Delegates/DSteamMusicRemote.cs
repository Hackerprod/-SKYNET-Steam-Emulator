using SKYNET.Delegate.Helper;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamMusicRemote")]
    public class DSteamMusicRemote
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RegisterSteamMusicRemote(string pchName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool DeregisterSteamMusicRemote(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsCurrentMusicRemote(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BActivationSuccess(bool bValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetDisplayName(string pchDisplayName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetPNGIcon_64x64(IntPtr pvBuffer, uint cbBufferLength);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool EnablePlayPrevious(bool bValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool EnablePlayNext(bool bValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool EnableShuffled(bool bValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool EnableLooped(bool bValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool EnableQueue(bool bValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool EnablePlaylists(bool bValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdatePlaybackStatus(AudioPlayback_Status nStatus);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdateShuffled(bool bValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdateLooped(bool bValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdateVolume(float flValue); // volume is between 0.0 and 1.0

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool CurrentEntryWillChange(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool CurrentEntryIsAvailable(bool bAvailable);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdateCurrentEntryText(string pchText);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdateCurrentEntryElapsedSeconds(int nValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdateCurrentEntryCoverArt(IntPtr pvBuffer, uint cbBufferLength);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool CurrentEntryDidChange(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool QueueWillChange(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ResetQueueEntries(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetQueueEntry(int nID, int nPosition, string pchEntryText);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetCurrentQueueEntry(int nID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool QueueDidChange(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool PlaylistWillChange(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ResetPlaylistEntries(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetPlaylistEntry(int nID, int nPosition, string pchEntryText);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetCurrentPlaylistEntry(int nID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool PlaylistDidChange(IntPtr _);
    }
}
