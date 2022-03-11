using System;
using System.IO;
using System.Runtime.InteropServices;
using SKYNET.Helper;
using SKYNET.Interface;
using Steamworks;

namespace SKYNET.Managers
{
    public class Steam_AppList : SteamInterface, ISteamAppList
    {
        public int GetAppBuildId(AppId_t nAppID)
        {
            Log.Write("Steam_AppList::GetAppBuildId");
            return 10;
        }

        public int GetAppInstallDir(AppId_t nAppID, IntPtr pchDirectory, int cchNameMax)
        {
            var mempchDirectory = Helpers.TakeMemory();
            Write("Steam_AppList::GetAppInstallDir");
            return -1;
        }

        public int GetAppName(AppId_t nAppID, IntPtr pchName, int cchNameMax)
        {
            var mempchName = Helpers.TakeMemory();
            Write("Steam_AppList::GetAppName\n");
            return -1;
        }

        public uint GetInstalledApps(AppId_t pvecAppID, uint unMaxAppIDs)
        {
            Write("Steam_Applist::GetInstalledApps\n");
            return 0;
        }

        public uint GetNumInstalledApps()
        {
            Write("Steam_Applist::GetNumInstalledApps\n");
            return 0;
        }


    }
}