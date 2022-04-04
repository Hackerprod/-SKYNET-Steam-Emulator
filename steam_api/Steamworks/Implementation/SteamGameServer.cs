using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SKYNET;
using SKYNET.Helper;
using Steamworks;

public class SteamGameServer : ISteamInterface
{
    public IntPtr MemoryAddress { get; set; }
    public string InterfaceVersion { get; set; }

    public int GetHSteamUser(IntPtr _)
    {
        Write($"GetHSteamUser");
        return 1;
    }

    public int GetHSteamPipe(IntPtr _)
    {
        Write($"GetHSteamPipe");
        return 1;
    }

    public void RunCallbacks(IntPtr _)
    {
        Write($"RunCallbacks");
    }

    private void Write(string v)
    {
        Log.Write(InterfaceVersion, v);
    }
}
