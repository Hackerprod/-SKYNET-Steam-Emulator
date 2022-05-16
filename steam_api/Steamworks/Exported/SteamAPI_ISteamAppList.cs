using System;
using System.IO;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helper;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamAppList
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamAppList_GetAppBuildId(uint nAppID)
        {
            Write("SteamAPI_ISteamAppList_GetAppBuildId");
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
        public static uint SteamAPI_ISteamAppList_GetInstalledApps(IntPtr _, uint pvecAppID, uint unMaxAppIDs)
        {
            Write("SteamAPI_ISteamAppList_GetInstalledAppsn");
            return SteamEmulator.SteamAppList.GetInstalledApps(pvecAppID, unMaxAppIDs);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamAppList_GetNumInstalledApps()
        {
            Write("SteamAPI_ISteamAppList_GetNumInstalledApps");
            return SteamEmulator.SteamAppList.GetNumInstalledApps();
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
