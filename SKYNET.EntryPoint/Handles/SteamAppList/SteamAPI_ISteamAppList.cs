using System;
using System.IO;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;


namespace SKYNET.Hook.Handles
{
    public partial class SteamAPI_ISteamAppList : BaseHook
    {
        public override bool Installed { get; set; }
        public override void Install()
        {
            base.Install<SteamAPI_ISteamAppList_GetAppBuildIdDelegate>("SteamAPI_ISteamAppList_GetAppBuildId", _SteamAPI_ISteamAppList_GetAppBuildId, new SteamAPI_ISteamAppList_GetAppBuildIdDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamAppList.SteamAPI_ISteamAppList_GetAppBuildId));
            base.Install<SteamAPI_ISteamAppList_GetAppInstallDirDelegate>("SteamAPI_ISteamAppList_GetAppInstallDir", _SteamAPI_ISteamAppList_GetAppInstallDir, new SteamAPI_ISteamAppList_GetAppInstallDirDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamAppList.SteamAPI_ISteamAppList_GetAppInstallDir));
            base.Install<SteamAPI_ISteamAppList_GetAppNameDelegate>("SteamAPI_ISteamAppList_GetAppName", _SteamAPI_ISteamAppList_GetAppName, new SteamAPI_ISteamAppList_GetAppNameDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamAppList.SteamAPI_ISteamAppList_GetAppName));
            base.Install<SteamAPI_ISteamAppList_GetInstalledAppsDelegate>("SteamAPI_ISteamAppList_GetInstalledApps", _SteamAPI_ISteamAppList_GetInstalledApps, new SteamAPI_ISteamAppList_GetInstalledAppsDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamAppList.SteamAPI_ISteamAppList_GetInstalledApps));
            base.Install<SteamAPI_ISteamAppList_GetNumInstalledAppsDelegate>("SteamAPI_ISteamAppList_GetNumInstalledApps", _SteamAPI_ISteamAppList_GetNumInstalledApps, new SteamAPI_ISteamAppList_GetNumInstalledAppsDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamAppList.SteamAPI_ISteamAppList_GetNumInstalledApps));
        }

        public override void Write(object v)
        {
            Main.Write("SteamAPI_ISteamAppList", v);
        }
    }
}
