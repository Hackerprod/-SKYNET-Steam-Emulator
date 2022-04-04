using SKYNET;
using SKYNET.Helper;
using SKYNET.Steamworks;
using System;

public class SteamMusic : ISteamInterface
{
    public IntPtr MemoryAddress { get; set; }
    public string InterfaceVersion { get; set; }


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

    public AudioPlayback_Status GetPlaybackStatus(IntPtr _)
    {
        Write($"GetPlaybackStatus");
        return AudioPlayback_Status.AudioPlayback_Undefined;
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

    private void Write(string v)
    {
        Log.Write(InterfaceVersion, v);
    }
}