using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamMusic
    {
        bool BIsEnabled(IntPtr _);
        bool BIsPlaying(IntPtr _);

        AudioPlayback_Status GetPlaybackStatus(IntPtr _);

        void Play(IntPtr _);
        void Pause(IntPtr _);
        void PlayPrevious(IntPtr _);
        void PlayNext(IntPtr _);

        // volume is between 0.0 and 1.0
        void SetVolume(float flVolume);
        float GetVolume(IntPtr _);

    }
    public enum AudioPlayback_Status
    {
        AudioPlayback_Undefined = 0,
        AudioPlayback_Playing = 1,
        AudioPlayback_Paused = 2,
        AudioPlayback_Idle = 3
    };
}
