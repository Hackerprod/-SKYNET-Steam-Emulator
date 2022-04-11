using System;


namespace SKYNET.Interface
{
    [Interface("STEAMMUSIC_INTERFACE_VERSION001")]
    public class SteamMusic001 : ISteamInterface
    {
        public bool BIsEnabled(IntPtr _)
        {
            return SteamEmulator.SteamMusic.BIsEnabled(_);
        }

        public bool BIsPlaying(IntPtr _)
        {
            return SteamEmulator.SteamMusic.BIsPlaying(_);
        }

        public int GetPlaybackStatus(IntPtr _)
        {
            return SteamEmulator.SteamMusic.GetPlaybackStatus(_);
        }

        public void Play(IntPtr _)
        {
            SteamEmulator.SteamMusic.Play(_);
        }

        public void Pause(IntPtr _)
        {
            SteamEmulator.SteamMusic.Pause(_);
        }

        public void PlayPrevious(IntPtr _)
        {
            SteamEmulator.SteamMusic.PlayPrevious(_);
        }

        public void PlayNext(IntPtr _)
        {
            SteamEmulator.SteamMusic.PlayNext(_);
        }

        public void SetVolume(IntPtr _, float flVolume)
        {
            SteamEmulator.SteamMusic.SetVolume(flVolume);
        }

        public float GetVolume(IntPtr _)
        {
            return SteamEmulator.SteamMusic.GetVolume(_);
        }

    }
}
