using System;
using SKYNET.Helper;
using SKYNET.Interface;
using Steamworks;

namespace SKYNET.Managers
{
    public class Steam_AppList : ISteamAppList
    {
        public int GetAppBuildId(AppId_t nAppID)
        {
            Log.Write("Steam_AppList::GetAppBuildId");
            return 10;
        }

        public int GetAppInstallDir(AppId_t nAppID, IntPtr pchDirectory, int cchNameMax)
        {
            PRINT_DEBUG("Steam_AppList::GetAppInstallDir");
            return -1;
        }



        public int GetAppName(AppId_t nAppID, IntPtr pchName, int cchNameMax)
        {
            PRINT_DEBUG("Steam_AppList::GetAppName\n");
            return -1;
        }

        public uint GetInstalledApps(AppId_t pvecAppID, uint unMaxAppIDs)
        {
            PRINT_DEBUG("Steam_Applist::GetInstalledApps\n");
            return 0;
        }

        public uint GetNumInstalledApps()
        {
            PRINT_DEBUG("Steam_Applist::GetNumInstalledApps\n");
            return 0;
        }

        private void PRINT_DEBUG(string v)
        {
            Log.Write(v);
        }
    }
}