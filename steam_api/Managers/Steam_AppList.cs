using System;
using System.IO;
using System.Runtime.InteropServices;
using SKYNET.Helper;
using SKYNET.Interface;
using Steamworks;

namespace SKYNET.Managers
{
    public class Steam_AppList : SteamInterface//, ISteamAppList
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetAppBuildId(AppId_t nAppID)
        {
            Log.Write("Steam_AppList::GetAppBuildId");
            return 10;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetAppInstallDir(AppId_t nAppID, IntPtr pchDirectory, int cchNameMax)
        {
            var mempchDirectory = Helpers.TakeMemory();
            PRINT_DEBUG("Steam_AppList::GetAppInstallDir");
            return -1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetAppName(AppId_t nAppID, IntPtr pchName, int cchNameMax)
        {
            var mempchName = Helpers.TakeMemory();
            PRINT_DEBUG("Steam_AppList::GetAppName\n");
            return -1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint GetInstalledApps(AppId_t pvecAppID, uint unMaxAppIDs)
        {
            PRINT_DEBUG("Steam_Applist::GetInstalledApps\n");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint GetNumInstalledApps()
        {
            PRINT_DEBUG("Steam_Applist::GetNumInstalledApps\n");
            return 0;
        }


    }
}