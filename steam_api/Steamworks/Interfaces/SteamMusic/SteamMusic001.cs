using System;


namespace SKYNET.Steamworks.Interfaces
{
    [Interface("STEAMMUSIC_INTERFACE_VERSION001")]
    public class SteamMusic001 : ISteamInterface
    {
        public bool BIsEnabled(IntPtr _)
        {
            return SteamEmulator.SteamMusic.BIsEnabled();
        }

        public bool BIsPlaying(IntPtr _)
        {
            return SteamEmulator.SteamMusic.BIsPlaying();
        }

        public int GetPlaybackStatus(IntPtr _)
        {
            return SteamEmulator.SteamMusic.GetPlaybackStatus();
        }

        public void Play(IntPtr _)
        {
            SteamEmulator.SteamMusic.Play();
        }

        public void Pause(IntPtr _)
        {
            SteamEmulator.SteamMusic.Pause();
        }

        public void PlayPrevious(IntPtr _)
        {
            SteamEmulator.SteamMusic.PlayPrevious();
        }

        public void PlayNext(IntPtr _)
        {
            SteamEmulator.SteamMusic.PlayNext();
        }

        public void SetVolume(IntPtr _, float flVolume)
        {
            SteamEmulator.SteamMusic.SetVolume(flVolume);
        }

        public float GetVolume(IntPtr _)
        {
            return SteamEmulator.SteamMusic.GetVolume();
        }

    }
}
