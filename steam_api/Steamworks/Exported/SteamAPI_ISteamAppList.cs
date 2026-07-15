using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamAppList
    {
        static SteamAPI_ISteamAppList()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamAppList_GetAppBuildId(IntPtr _, uint nAppID)
        {
            Write("SteamAPI_ISteamAppList_GetAppBuildId");
            return SteamEmulator.SteamAppList.GetAppBuildId(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamAppList_GetAppInstallDir(IntPtr _, uint nAppID, IntPtr pchDirectory, int cchNameMax)
        {
            Write("SteamAPI_ISteamAppList_GetAppInstallDir");
            return SteamEmulator.SteamAppList.GetAppInstallDir(nAppID, pchDirectory, cchNameMax);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamAppList_GetAppName(IntPtr _, uint nAppID, IntPtr pchName, int cchNameMax)
        {
            Write("SteamAPI_ISteamAppList_GetAppName");
            return SteamEmulator.SteamAppList.GetAppName(nAppID, pchName, cchNameMax);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamAppList_GetInstalledApps(IntPtr _, IntPtr pvecAppID, uint unMaxAppIDs)
        {
            Write("SteamAPI_ISteamAppList_GetInstalledAppsn");
            return SteamEmulator.SteamAppList.GetInstalledApps(pvecAppID, unMaxAppIDs);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamAppList_GetNumInstalledApps(IntPtr _)
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
