using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    [StructLayout(LayoutKind.Sequential)]
    public class SteamMusic : ISteamInterface
    {
        public bool BIsEnabled(IntPtr _)
        {
            Write($"BIsEnabled");
            return true;
        }

        public bool BIsPlaying(IntPtr _)
        {
            Write($"BIsPlaying");
            return true;
        }

        public int GetPlaybackStatus(IntPtr _)
        {
            Write($"GetPlaybackStatus");
            return (int)AudioPlayback_Status.AudioPlayback_Undefined;
        }

        public float GetVolume(IntPtr _)
        {
            Write($"GetVolume");
            return 0;
        }

        public void Pause(IntPtr _)
        {
            Write($"Pause");
        }

        public void Play(IntPtr _)
        {
            Write($"Play");
        }

        public void PlayNext(IntPtr _)
        {
            Write($"PlayNext");
        }

        public void PlayPrevious(IntPtr _)
        {
            Write($"PlayPrevious");
        }

        public void SetVolume(float flVolume)
        {
            Write($"SetVolume");
        }

        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamMusic()
        {
            InterfaceVersion = "SteamMusic";
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}