using SKYNET.Steamworks.Interfaces;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamMusic : ISteamInterface
    {
        public static SteamMusic Instance;

        public SteamMusic()
        {
            Instance = this;
            InterfaceName = "SteamMusic";
            InterfaceVersion = "STEAMMUSIC_INTERFACE_VERSION001";
        }

        public bool BIsEnabled()
        {
            Write($"BIsEnabled");
            return true;
        }

        public bool BIsPlaying()
        {
            Write($"BIsPlaying");
            return true;
        }

        public int GetPlaybackStatus()
        {
            Write($"GetPlaybackStatus");
            return (int)AudioPlayback_Status.AudioPlayback_Undefined;
        }

        public float GetVolume()
        {
            Write($"GetVolume");
            return 0;
        }

        public void Pause()
        {
            Write($"Pause");
        }

        public void Play()
        {
            Write($"Play");
        }

        public void PlayNext()
        {
            Write($"PlayNext");
        }

        public void PlayPrevious()
        {
            Write($"PlayPrevious");
        }

        public void SetVolume(float flVolume)
        {
            Write($"SetVolume");
        }
    }
}