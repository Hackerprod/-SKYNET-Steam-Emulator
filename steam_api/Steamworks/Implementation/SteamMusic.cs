using SKYNET.Steamworks.Interfaces;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamMusic : ISteamInterface
    {
        public static SteamMusic Instance;

        private bool IsPlaying;
        private float Volume = 1.0f;

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
            return IsPlaying;
        }

        public int GetPlaybackStatus()
        {
            Write($"GetPlaybackStatus");
            return (int)(IsPlaying
                ? AudioPlayback_Status.AudioPlayback_Playing
                : AudioPlayback_Status.AudioPlayback_Idle);
        }

        public float GetVolume()
        {
            Write($"GetVolume");
            return Volume;
        }

        public void Pause()
        {
            Write($"Pause");
            IsPlaying = false;
        }

        public void Play()
        {
            Write($"Play");
            IsPlaying = true;
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
            Write($"SetVolume {flVolume}");
            if (flVolume < 0)
            {
                Volume = 0;
                return;
            }

            if (flVolume > 1)
            {
                Volume = 1;
                return;
            }

            Volume = flVolume;
        }
    }
}
