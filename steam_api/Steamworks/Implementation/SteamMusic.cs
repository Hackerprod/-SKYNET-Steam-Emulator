using Core.Interface;
using SKYNET.Interface;

namespace SKYNET.Managers
{
    //[Map("STEAMMUSIC_INTERFACE_VERSION")]
    //[Map("SteamMusic")]
    public class SteamMusic : IBaseInterface, ISteamMusic
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