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
            return SKYNET.Managers.MusicPlayerManager.IsEnabled;
        }

        public bool BIsPlaying()
        {
            Write($"BIsPlaying");
            return SKYNET.Managers.MusicPlayerManager.IsPlaying;
        }

        public int GetPlaybackStatus()
        {
            Write($"GetPlaybackStatus");
            return (int)SKYNET.Managers.MusicPlayerManager.PlaybackStatus;
        }

        public float GetVolume()
        {
            Write($"GetVolume");
            return SKYNET.Managers.MusicPlayerManager.Volume;
        }

        public void Pause()
        {
            Write($"Pause");
            SKYNET.Managers.MusicPlayerManager.Pause();
        }

        public void Play()
        {
            Write($"Play");
            SKYNET.Managers.MusicPlayerManager.Play();
        }

        public void PlayNext()
        {
            Write($"PlayNext");
            SKYNET.Managers.MusicPlayerManager.PlayNext();
        }

        public void PlayPrevious()
        {
            Write($"PlayPrevious");
            SKYNET.Managers.MusicPlayerManager.PlayPrevious();
        }

        public void SetVolume(float flVolume)
        {
            Write($"SetVolume {flVolume}");
            SKYNET.Managers.MusicPlayerManager.SetVolume(flVolume);
        }
    }
}
