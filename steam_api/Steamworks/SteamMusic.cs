using SKYNET.Interface;

namespace SKYNET.Managers
{
    [Interface("STEAMMUSIC_INTERFACE_VERSION")]
    [Interface("SteamMusic")]
    public class SteamMusic : SteamInterface, ISteamMusic
    {
        public bool BIsEnabled()
        {
            return true;
        }

        public bool BIsPlaying()
        {
            return true;
        }

        public AudioPlayback_Status GetPlaybackStatus()
        {
            return AudioPlayback_Status.AudioPlayback_Undefined;
        }

        public float GetVolume()
        {
            return 0;
        }

        public void Pause()
        {
            //
        }

        public void Play()
        {
            //
        }

        public void PlayNext()
        {
            //
        }

        public void PlayPrevious()
        {
            //
        }

        public void SetVolume(float flVolume)
        {
            //
        }
    }
}