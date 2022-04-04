using System;
using System.IO;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helper;
using Steamworks;

namespace SKYNET.Hook.Handles
{
    public partial class SteamAPI_ISteamAppList : BaseHook
    {
        public override bool Installed { get; set; }
        public override void Install()
        {
            base.Install<SteamAPI_ISteamAppList_GetAppBuildIdDelegate>("SteamAPI_ISteamAppList_GetAppBuildId", _SteamAPI_ISteamAppList_GetAppBuildId, new SteamAPI_ISteamAppList_GetAppBuildIdDelegate(SteamAPI_ISteamAppList_GetAppBuildId));
            base.Install<SteamAPI_ISteamAppList_GetAppInstallDirDelegate>("SteamAPI_ISteamAppList_GetAppInstallDir", _SteamAPI_ISteamAppList_GetAppInstallDir, new SteamAPI_ISteamAppList_GetAppInstallDirDelegate(SteamAPI_ISteamAppList_GetAppInstallDir));
            base.Install<SteamAPI_ISteamAppList_GetAppNameDelegate>("SteamAPI_ISteamAppList_GetAppName", _SteamAPI_ISteamAppList_GetAppName, new SteamAPI_ISteamAppList_GetAppNameDelegate(SteamAPI_ISteamAppList_GetAppName));
            base.Install<SteamAPI_ISteamAppList_GetInstalledAppsDelegate>("SteamAPI_ISteamAppList_GetInstalledApps", _SteamAPI_ISteamAppList_GetInstalledApps, new SteamAPI_ISteamAppList_GetInstalledAppsDelegate(SteamAPI_ISteamAppList_GetInstalledApps));
            base.Install<SteamAPI_ISteamAppList_GetNumInstalledAppsDelegate>("SteamAPI_ISteamAppList_GetNumInstalledApps", _SteamAPI_ISteamAppList_GetNumInstalledApps, new SteamAPI_ISteamAppList_GetNumInstalledAppsDelegate(SteamAPI_ISteamAppList_GetNumInstalledApps));
        }

        public int SteamAPI_ISteamAppList_GetAppBuildId(AppId_t nAppID)
        {
            Log.Write("Steam_AppList::GetAppBuildId");
            return SteamEmulator.SteamAppList.GetAppBuildId(nAppID);
        }

        public int SteamAPI_ISteamAppList_GetAppInstallDir(AppId_t nAppID, IntPtr pchDirectory, int cchNameMax)
        {
            Write("SteamAPI_ISteamAppList_GetAppInstallDir");
            return SteamEmulator.SteamAppList.GetAppInstallDir(nAppID, pchDirectory, cchNameMax);
        }

        public int SteamAPI_ISteamAppList_GetAppName(AppId_t nAppID, IntPtr pchName, int cchNameMax)
        {
            Write("SteamAPI_ISteamAppList_GetAppName");
            return SteamEmulator.SteamAppList.GetAppName(nAppID, pchName, cchNameMax);
        }

        public uint SteamAPI_ISteamAppList_GetInstalledApps(AppId_t pvecAppID, uint unMaxAppIDs)
        {
            Write("SteamAPI_ISteamAppList_GetInstalledAppsn");
            return SteamEmulator.SteamAppList.GetInstalledApps(pvecAppID, unMaxAppIDs);
        }

        public uint SteamAPI_ISteamAppList_GetNumInstalledApps(IntPtr _)
        {
            Write("SteamAPI_ISteamAppList_GetNumInstalledApps");
            return SteamEmulator.SteamAppList.GetNumInstalledApps(_);
        }

        public override void Write(object v)
        {
            Main.Write("SteamAPI_ISteamAppList", v);
        }
    }
}
