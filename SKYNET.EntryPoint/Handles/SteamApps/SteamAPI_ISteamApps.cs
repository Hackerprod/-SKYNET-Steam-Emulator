using System;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;
using SteamAPICall_t = System.UInt64;
using AppId_t = System.UInt32;

namespace SKYNET.Hook.Handles
{
    public partial class SteamAPI_ISteamApps : BaseHook
    {
        public override bool Installed { get; set; }
        public override void Install()
        {
            base.Install<SteamAPI_ISteamApps_BIsSubscribedDelegate>("SteamAPI_ISteamApps_BIsSubscribed", _SteamAPI_ISteamApps_BIsSubscribed, new SteamAPI_ISteamApps_BIsSubscribedDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_BIsSubscribed));
            base.Install<SteamAPI_ISteamApps_BIsLowViolenceDelegate>("SteamAPI_ISteamApps_BIsLowViolence", _SteamAPI_ISteamApps_BIsLowViolence, new SteamAPI_ISteamApps_BIsLowViolenceDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_BIsLowViolence));
            base.Install<SteamAPI_ISteamApps_BIsCybercafeDelegate>("SteamAPI_ISteamApps_BIsCybercafe", _SteamAPI_ISteamApps_BIsCybercafe, new SteamAPI_ISteamApps_BIsCybercafeDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_BIsCybercafe));
            base.Install<SteamAPI_ISteamApps_BIsVACBannedDelegate>("SteamAPI_ISteamApps_BIsVACBanned", _SteamAPI_ISteamApps_BIsVACBanned, new SteamAPI_ISteamApps_BIsVACBannedDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_BIsVACBanned));
            base.Install<SteamAPI_ISteamApps_GetCurrentGameLanguageDelegate>("SteamAPI_ISteamApps_GetCurrentGameLanguage", _SteamAPI_ISteamApps_GetCurrentGameLanguage, new SteamAPI_ISteamApps_GetCurrentGameLanguageDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_GetCurrentGameLanguage));
            base.Install<SteamAPI_ISteamApps_GetAvailableGameLanguagesDelegate>("SteamAPI_ISteamApps_GetAvailableGameLanguages", _SteamAPI_ISteamApps_GetAvailableGameLanguages, new SteamAPI_ISteamApps_GetAvailableGameLanguagesDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_GetAvailableGameLanguages));
            base.Install<SteamAPI_ISteamApps_BIsSubscribedAppDelegate>("SteamAPI_ISteamApps_BIsSubscribedApp", _SteamAPI_ISteamApps_BIsSubscribedApp, new SteamAPI_ISteamApps_BIsSubscribedAppDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_BIsSubscribedApp));
            base.Install<SteamAPI_ISteamApps_BIsDlcInstalledDelegate>("SteamAPI_ISteamApps_BIsDlcInstalled", _SteamAPI_ISteamApps_BIsDlcInstalled, new SteamAPI_ISteamApps_BIsDlcInstalledDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_BIsDlcInstalled));
            base.Install<SteamAPI_ISteamApps_GetEarliestPurchaseUnixTimeDelegate>("SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime", _SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime, new SteamAPI_ISteamApps_GetEarliestPurchaseUnixTimeDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime));
            base.Install<SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekendDelegate>("SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend", _SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend, new SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekendDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend));
            base.Install<SteamAPI_ISteamApps_GetDLCCountDelegate>("SteamAPI_ISteamApps_GetDLCCount", _SteamAPI_ISteamApps_GetDLCCount, new SteamAPI_ISteamApps_GetDLCCountDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_GetDLCCount));
            base.Install<SteamAPI_ISteamApps_BGetDLCDataByIndexDelegate>("SteamAPI_ISteamApps_BGetDLCDataByIndex", _SteamAPI_ISteamApps_BGetDLCDataByIndex, new SteamAPI_ISteamApps_BGetDLCDataByIndexDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_BGetDLCDataByIndex));
            base.Install<SteamAPI_ISteamApps_InstallDLCDelegate>("SteamAPI_ISteamApps_InstallDLC", _SteamAPI_ISteamApps_InstallDLC, new SteamAPI_ISteamApps_InstallDLCDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_InstallDLC));
            base.Install<SteamAPI_ISteamApps_UninstallDLCDelegate>("SteamAPI_ISteamApps_UninstallDLC", _SteamAPI_ISteamApps_UninstallDLC, new SteamAPI_ISteamApps_UninstallDLCDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_UninstallDLC));
            base.Install<SteamAPI_ISteamApps_RequestAppProofOfPurchaseKeyDelegate>("SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey", _SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey, new SteamAPI_ISteamApps_RequestAppProofOfPurchaseKeyDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey));
            base.Install<SteamAPI_ISteamApps_GetCurrentBetaNameDelegate>("SteamAPI_ISteamApps_GetCurrentBetaName", _SteamAPI_ISteamApps_GetCurrentBetaName, new SteamAPI_ISteamApps_GetCurrentBetaNameDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_GetCurrentBetaName));
            base.Install<SteamAPI_ISteamApps_MarkContentCorruptDelegate>("SteamAPI_ISteamApps_MarkContentCorrupt", _SteamAPI_ISteamApps_MarkContentCorrupt, new SteamAPI_ISteamApps_MarkContentCorruptDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_MarkContentCorrupt));
            base.Install<SteamAPI_ISteamApps_GetInstalledDepotsDelegate>("SteamAPI_ISteamApps_GetInstalledDepots", _SteamAPI_ISteamApps_GetInstalledDepots, new SteamAPI_ISteamApps_GetInstalledDepotsDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_GetInstalledDepots));
            //base.Install<GetInstalledDepotsDelegate>("GetInstalledDepots", _GetInstalledDepots, new GetInstalledDepotsDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.GetInstalledDepots));
            base.Install<SteamAPI_ISteamApps_GetAppInstallDirDelegate>("SteamAPI_ISteamApps_GetAppInstallDir", _SteamAPI_ISteamApps_GetAppInstallDir, new SteamAPI_ISteamApps_GetAppInstallDirDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_GetAppInstallDir));
            base.Install<SteamAPI_ISteamApps_BIsAppInstalledDelegate>("SteamAPI_ISteamApps_BIsAppInstalled", _SteamAPI_ISteamApps_BIsAppInstalled, new SteamAPI_ISteamApps_BIsAppInstalledDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_BIsAppInstalled));
            base.Install<SteamAPI_ISteamApps_GetAppOwnerDelegate>("SteamAPI_ISteamApps_GetAppOwner", _SteamAPI_ISteamApps_GetAppOwner, new SteamAPI_ISteamApps_GetAppOwnerDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_GetAppOwner));
            base.Install<SteamAPI_ISteamApps_GetLaunchQueryParamDelegate>("SteamAPI_ISteamApps_GetLaunchQueryParam", _SteamAPI_ISteamApps_GetLaunchQueryParam, new SteamAPI_ISteamApps_GetLaunchQueryParamDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_GetLaunchQueryParam));
            base.Install<SteamAPI_ISteamApps_GetDlcDownloadProgressDelegate>("SteamAPI_ISteamApps_GetDlcDownloadProgress", _SteamAPI_ISteamApps_GetDlcDownloadProgress, new SteamAPI_ISteamApps_GetDlcDownloadProgressDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_GetDlcDownloadProgress));
            base.Install<SteamAPI_ISteamApps_GetAppBuildIdDelegate>("SteamAPI_ISteamApps_GetAppBuildId", _SteamAPI_ISteamApps_GetAppBuildId, new SteamAPI_ISteamApps_GetAppBuildIdDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_GetAppBuildId));
            base.Install<SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeysDelegate>("SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeys", _SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeys, new SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeysDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeys));
            base.Install<SteamAPI_ISteamApps_GetFileDetailsDelegate>("SteamAPI_ISteamApps_GetFileDetails", _SteamAPI_ISteamApps_GetFileDetails, new SteamAPI_ISteamApps_GetFileDetailsDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_GetFileDetails));
            base.Install<SteamAPI_ISteamApps_GetLaunchCommandLineDelegate>("SteamAPI_ISteamApps_GetLaunchCommandLine", _SteamAPI_ISteamApps_GetLaunchCommandLine, new SteamAPI_ISteamApps_GetLaunchCommandLineDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_GetLaunchCommandLine));
            base.Install<SteamAPI_ISteamApps_BIsSubscribedFromFamilySharingDelegate>("SteamAPI_ISteamApps_BIsSubscribedFromFamilySharing", _SteamAPI_ISteamApps_BIsSubscribedFromFamilySharing, new SteamAPI_ISteamApps_BIsSubscribedFromFamilySharingDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_BIsSubscribedFromFamilySharing));
            base.Install<SteamAPI_ISteamApps_BIsTimedTrialDelegate>("SteamAPI_ISteamApps_BIsTimedTrial", _SteamAPI_ISteamApps_BIsTimedTrial, new SteamAPI_ISteamApps_BIsTimedTrialDelegate(SKYNET.Steamworks.Exported.SteamAPI_ISteamApps.SteamAPI_ISteamApps_BIsTimedTrial));
        }

        public override void Write(object v)
        {
            Main.Write("", v);
        }
    }
}
