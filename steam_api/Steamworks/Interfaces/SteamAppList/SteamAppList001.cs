using System;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("STEAMAPPLIST_INTERFACE_VERSION001")]
    public class SteamAppList001 : ISteamInterface
    {
        public uint GetNumInstalledApps(IntPtr _)
        {
            return SteamEmulator.SteamAppList.GetNumInstalledApps();
        }

        public uint GetInstalledApps(IntPtr _, IntPtr pvecAppID, uint unMaxAppIDs)
        {
            return SteamEmulator.SteamAppList.GetInstalledApps(pvecAppID, unMaxAppIDs);
        }

        public int GetAppName(IntPtr _, uint nAppID, IntPtr pchName, int cchNameMax)
        {
            return SteamEmulator.SteamAppList.GetAppName(nAppID, pchName, cchNameMax);
        }

        public int GetAppInstallDir(IntPtr _, uint nAppID, IntPtr pchDirectory, int cchNameMax)
        {
            return SteamEmulator.SteamAppList.GetAppInstallDir(nAppID, pchDirectory, cchNameMax);
        }

        public int GetAppBuildId(IntPtr _, uint nAppID)  
        {
            return SteamEmulator.SteamAppList.GetAppBuildId(nAppID);
        }
    }
}
