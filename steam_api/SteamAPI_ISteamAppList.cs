using System;
using System.IO;
using System.Runtime.InteropServices;
using SKYNET.Helper;
using SKYNET.Interface;
using Steamworks;

namespace SKYNET.Managers
{
    public class SteamAPI_ISteamAppList : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamAppList_GetAppBuildId(AppId_t nAppID)
        {
            Log.Write("Steam_AppList::GetAppBuildId");
            return SteamClient.steam_AppList.GetAppBuildId(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamAppList_GetAppInstallDir(AppId_t nAppID, IntPtr pchDirectory, int cchNameMax)
        {
            var mempchDirectory = Helpers.TakeMemory();
            Write("SteamAPI_ISteamAppList_GetAppInstallDir");
            return SteamClient.steam_AppList.GetAppInstallDir(nAppID, pchDirectory, cchNameMax);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamAppList_GetAppName(AppId_t nAppID, IntPtr pchName, int cchNameMax)
        {
            var mempchName = Helpers.TakeMemory();
            Write("SteamAPI_ISteamAppList_GetAppName");
            return SteamClient.steam_AppList.GetAppName(nAppID, pchName, cchNameMax);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamAppList_GetInstalledApps(AppId_t pvecAppID, uint unMaxAppIDs)
        {
            Write("SteamAPI_ISteamAppList_GetInstalledAppsn");
            return SteamClient.steam_AppList.GetInstalledApps(pvecAppID, unMaxAppIDs);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamAppList_GetNumInstalledApps()
        {
            Write("SteamAPI_ISteamAppList_GetNumInstalledApps");
            return SteamClient.steam_AppList.GetNumInstalledApps();
        }


    }
}