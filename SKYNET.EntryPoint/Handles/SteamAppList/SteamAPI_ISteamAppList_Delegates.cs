
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Hook.Handles
{
    public partial class SteamAPI_ISteamAppList
    {
        // Functions Delegates

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int SteamAPI_ISteamAppList_GetAppBuildIdDelegate(uint nAppID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int SteamAPI_ISteamAppList_GetAppInstallDirDelegate(uint nAppID, string pchDirectory, int cchNameMax);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int SteamAPI_ISteamAppList_GetAppNameDelegate(uint nAppID, string pchName, int cchNameMax);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate uint SteamAPI_ISteamAppList_GetInstalledAppsDelegate(IntPtr _, uint pvecAppID, uint unMaxAppIDs);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate uint SteamAPI_ISteamAppList_GetNumInstalledAppsDelegate();


        // Functions Delegates

        public static SteamAPI_ISteamAppList_GetAppBuildIdDelegate _SteamAPI_ISteamAppList_GetAppBuildId;

        public static SteamAPI_ISteamAppList_GetAppInstallDirDelegate _SteamAPI_ISteamAppList_GetAppInstallDir;

        public static SteamAPI_ISteamAppList_GetAppNameDelegate _SteamAPI_ISteamAppList_GetAppName;

        public static SteamAPI_ISteamAppList_GetInstalledAppsDelegate _SteamAPI_ISteamAppList_GetInstalledApps;

        public static SteamAPI_ISteamAppList_GetNumInstalledAppsDelegate _SteamAPI_ISteamAppList_GetNumInstalledApps;



    }
}
