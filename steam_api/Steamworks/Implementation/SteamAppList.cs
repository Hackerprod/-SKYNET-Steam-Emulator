using SKYNET.Steamworks.Interfaces;
using SKYNET.Helpers;
using System;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamAppList : ISteamInterface
    {
        public static SteamAppList Instance;

        public SteamAppList()
        {
            Instance = this;
            InterfaceName = "SteamAppList";
            InterfaceVersion = "STEAMAPPLIST_INTERFACE_VERSION001";
        }

        public int GetAppBuildId(uint nAppID)
        {
            Write("GetAppBuildId");
            return 10;
        }

        public int GetAppInstallDir(uint nAppID, string pchDirectory, int cchNameMax)
        {
            Write("GetAppInstallDir");
            return -1;
        }

        public int GetAppInstallDir(uint nAppID, IntPtr pchDirectory, int cchNameMax)
        {
            Write("GetAppInstallDir");
            NativeStringCache.WriteUtf8Buffer(pchDirectory, cchNameMax, string.Empty);
            return -1;
        }

        public int GetAppName(uint nAppID, string pchName, int cchNameMax)
        {
            Write("GetAppName\n");
            return -1;
        }

        public int GetAppName(uint nAppID, IntPtr pchName, int cchNameMax)
        {
            Write("GetAppName\n");
            NativeStringCache.WriteUtf8Buffer(pchName, cchNameMax, string.Empty);
            return -1;
        }

        public uint GetInstalledApps(uint pvecAppID, uint unMaxAppIDs)
        {
            Write("GetInstalledApps\n");
            return 0;
        }

        public uint GetInstalledApps(IntPtr pvecAppID, uint unMaxAppIDs)
        {
            Write("GetInstalledApps\n");
            return 0;
        }

        public uint GetNumInstalledApps()
        {
            Write("GetNumInstalledApps\n");
            return 0;
        }
    }
}
