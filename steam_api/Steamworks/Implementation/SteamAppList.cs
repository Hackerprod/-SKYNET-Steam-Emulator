using System;
using System.IO;
using System.Runtime.InteropServices;

using SKYNET;
using SKYNET.Helper;
using Steamworks;

public class SteamAppList : ISteamInterface
{
    public IntPtr MemoryAddress { get; set; }
    public string InterfaceVersion { get; set; }


    public int GetAppBuildId(AppId_t nAppID)
    {
        Write("GetAppBuildId");
        return 10;
    }

    public int GetAppInstallDir(AppId_t nAppID, IntPtr pchDirectory, int cchNameMax)
    {
        Write("GetAppInstallDir");
        return -1;
    }

    public int GetAppName(AppId_t nAppID, IntPtr pchName, int cchNameMax)
    {
        Write("GetAppName\n");
        return -1;
    }

    public uint GetInstalledApps(AppId_t pvecAppID, uint unMaxAppIDs)
    {
        Write("GetInstalledApps\n");
        return 0;
    }

    public uint GetNumInstalledApps(IntPtr _)
    {
        Write("GetNumInstalledApps\n");
        return 0;
    }

    private void Write(string v)
    {
       SteamEmulator.Write(InterfaceVersion, v);
    }
}
