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

        public uint GetInstalledApps(IntPtr _, uint pvecAppID, uint unMaxAppIDs)
        {
            return SteamEmulator.SteamAppList.GetInstalledApps(pvecAppID, unMaxAppIDs);
        }

        public int GetAppName(uint nAppID, string pchName, int cchNameMax)  
        {
            return SteamEmulator.SteamAppList.GetAppName(nAppID, pchName, cchNameMax);
        }

        public int GetAppInstallDir(IntPtr _, uint nAppID, string pchDirectory, int cchNameMax)  // returns -1 if no dir was found
        {
            return SteamEmulator.SteamAppList.GetAppInstallDir(nAppID, pchDirectory, cchNameMax);
        }

        public int GetAppBuildId(IntPtr _, uint nAppID)  
        {
            return SteamEmulator.SteamAppList.GetAppBuildId(nAppID);
        }
    }
}
