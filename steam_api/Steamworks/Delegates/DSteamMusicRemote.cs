using SKYNET.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    //[Delegate("SteamMusicRemote")]
    //public class DSteamMusicRemote : IBaseInterfaceMap
    //{
    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool RegisterSteamMusicRemote(string pchName);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool DeregisterSteamMusicRemote();

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool BIsCurrentMusicRemote();

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool BActivationSuccess(bool bValue);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool SetDisplayName(string pchDisplayName);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool SetPNGIcon_64x64(IntPtr pvBuffer, uint cbBufferLength);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool EnablePlayPrevious(bool bValue);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool EnablePlayNext(bool bValue);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool EnableShuffled(bool bValue);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool EnableLooped(bool bValue);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool EnableQueue(bool bValue);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool EnablePlaylists(bool bValue);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool UpdatePlaybackStatus(AudioPlayback_Status nStatus);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool UpdateShuffled(bool bValue);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool UpdateLooped(bool bValue);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool UpdateVolume(float flValue); // volume is between 0.0 and 1.0

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool CurrentEntryWillChange();

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool CurrentEntryIsAvailable(bool bAvailable);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool UpdateCurrentEntryText(string pchText);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool UpdateCurrentEntryElapsedSeconds(int nValue);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool UpdateCurrentEntryCoverArt(IntPtr pvBuffer, uint cbBufferLength);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool CurrentEntryDidChange();

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool QueueWillChange();

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool ResetQueueEntries();

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool SetQueueEntry(int nID, int nPosition, string pchEntryText);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool SetCurrentQueueEntry(int nID);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool QueueDidChange();

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool PlaylistWillChange();

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool ResetPlaylistEntries();

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool SetPlaylistEntry(int nID, int nPosition, string pchEntryText);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool SetCurrentPlaylistEntry(int nID);

    //    [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
    //    public delegate bool PlaylistDidChange();
    //}
}
