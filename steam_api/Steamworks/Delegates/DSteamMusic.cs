using Core.Interface;
using SKYNET.Interface;
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
        public delegate bool BIsEnabled();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsPlaying();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate AudioPlayback_Status GetPlaybackStatus();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void Play();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void Pause();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void PlayPrevious();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void PlayNext();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetVolume(float flVolume);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate float GetVolume();

    }
}
