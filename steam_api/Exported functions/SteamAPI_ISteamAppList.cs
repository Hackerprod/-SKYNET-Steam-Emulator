using System;
using System.IO;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using Steamworks;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamAppList : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamAppList_GetAppBuildId(uint nAppID)
        {
            Write("Steam_AppList::GetAppBuildId");
            return SteamEmulator.SteamAppList.GetAppBuildId(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamAppList_GetAppInstallDir(uint nAppID, string pchDirectory, int cchNameMax)
        {
            Write("SteamAPI_ISteamAppList_GetAppInstallDir");
            return SteamEmulator.SteamAppList.GetAppInstallDir(nAppID, pchDirectory, cchNameMax);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamAppList_GetAppName(uint nAppID, string pchName, int cchNameMax)
        {
            Write("SteamAPI_ISteamAppList_GetAppName");
            return SteamEmulator.SteamAppList.GetAppName(nAppID, pchName, cchNameMax);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamAppList_GetInstalledApps(uint pvecAppID, uint unMaxAppIDs)
        {
            Write("SteamAPI_ISteamAppList_GetInstalledAppsn");
            return SteamEmulator.SteamAppList.GetInstalledApps(pvecAppID, unMaxAppIDs);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamAppList_GetNumInstalledApps(IntPtr _)
        {
            Write("SteamAPI_ISteamAppList_GetNumInstalledApps");
            return SteamEmulator.SteamAppList.GetNumInstalledApps(_);
        }
    }
}
