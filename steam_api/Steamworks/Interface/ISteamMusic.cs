using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamMusic
    {
        bool BIsEnabled();
        bool BIsPlaying();

        AudioPlayback_Status GetPlaybackStatus();

        void Play();
        void Pause();
        void PlayPrevious();
        void PlayNext();

        // volume is between 0.0 and 1.0
        void SetVolume(float flVolume);
        float GetVolume();

    }
    public enum AudioPlayback_Status
    {
        AudioPlayback_Undefined = 0,
        AudioPlayback_Playing = 1,
        AudioPlayback_Paused = 2,
        AudioPlayback_Idle = 3
    };
}
