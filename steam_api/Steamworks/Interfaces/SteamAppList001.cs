using System;

namespace SKYNET.Interface
{
    [Interface("STEAMAPPLIST_INTERFACE_VERSION001")]
    public class SteamAppList001 : ISteamInterface
    {
        public uint GetNumInstalledApps(IntPtr _)
        {
            return SteamEmulator.SteamAppList.GetNumInstalledApps(_);
        }

        public uint GetInstalledApps(IntPtr _, IntPtr pvecAppID, uint unMaxAppIDs)
        {
            return SteamEmulator.SteamAppList.GetInstalledApps(pvecAppID, unMaxAppIDs);
        }

        public int GetAppName(IntPtr _, IntPtr nAppID, string pchName, int cchNameMax) 
        {
            return SteamEmulator.SteamAppList.GetAppName(nAppID, pchName, cchNameMax);
        }

        public int GetAppInstallDir(IntPtr _, IntPtr nAppID, string pchDirectory, int cchNameMax)  
        {
            return SteamEmulator.SteamAppList.GetAppInstallDir(nAppID, pchDirectory, cchNameMax);
        }

        public int GetAppBuildId(IntPtr _, IntPtr nAppID)  
        {
            return SteamEmulator.SteamAppList.GetAppBuildId(nAppID);
        }


    }
}
