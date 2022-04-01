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
    [Delegate(Name = "SteamMusic")]
    public class DSteamMusic
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsEnabled(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsPlaying(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate AudioPlayback_Status GetPlaybackStatus(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void Play(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void Pause(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void PlayPrevious(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void PlayNext(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetVolume(float flVolume);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate float GetVolume(IntPtr _);

    }
}
