using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SKYNET;
using SKYNET.Hook;
using Steamworks;

namespace SKYNET.Hook.Handles
{
    public partial class SteamAPI_ISteamApps : BaseHook
    {
        public override bool Installed { get; set; }
        public override void Install()
        {
            base.Install<SteamAPI_ISteamApps_BIsSubscribedDelegate>("SteamAPI_ISteamApps_BIsSubscribed", _SteamAPI_ISteamApps_BIsSubscribed, new SteamAPI_ISteamApps_BIsSubscribedDelegate(SteamAPI_ISteamApps_BIsSubscribed));
            base.Install<SteamAPI_ISteamApps_BIsLowViolenceDelegate>("SteamAPI_ISteamApps_BIsLowViolence", _SteamAPI_ISteamApps_BIsLowViolence, new SteamAPI_ISteamApps_BIsLowViolenceDelegate(SteamAPI_ISteamApps_BIsLowViolence));
            base.Install<SteamAPI_ISteamApps_BIsCybercafeDelegate>("SteamAPI_ISteamApps_BIsCybercafe", _SteamAPI_ISteamApps_BIsCybercafe, new SteamAPI_ISteamApps_BIsCybercafeDelegate(SteamAPI_ISteamApps_BIsCybercafe));
            base.Install<SteamAPI_ISteamApps_BIsVACBannedDelegate>("SteamAPI_ISteamApps_BIsVACBanned", _SteamAPI_ISteamApps_BIsVACBanned, new SteamAPI_ISteamApps_BIsVACBannedDelegate(SteamAPI_ISteamApps_BIsVACBanned));
            base.Install<SteamAPI_ISteamApps_GetCurrentGameLanguageDelegate>("SteamAPI_ISteamApps_GetCurrentGameLanguage", _SteamAPI_ISteamApps_GetCurrentGameLanguage, new SteamAPI_ISteamApps_GetCurrentGameLanguageDelegate(SteamAPI_ISteamApps_GetCurrentGameLanguage));
            base.Install<SteamAPI_ISteamApps_GetAvailableGameLanguagesDelegate>("SteamAPI_ISteamApps_GetAvailableGameLanguages", _SteamAPI_ISteamApps_GetAvailableGameLanguages, new SteamAPI_ISteamApps_GetAvailableGameLanguagesDelegate(SteamAPI_ISteamApps_GetAvailableGameLanguages));
            base.Install<SteamAPI_ISteamApps_BIsSubscribedAppDelegate>("SteamAPI_ISteamApps_BIsSubscribedApp", _SteamAPI_ISteamApps_BIsSubscribedApp, new SteamAPI_ISteamApps_BIsSubscribedAppDelegate(SteamAPI_ISteamApps_BIsSubscribedApp));
            base.Install<SteamAPI_ISteamApps_BIsDlcInstalledDelegate>("SteamAPI_ISteamApps_BIsDlcInstalled", _SteamAPI_ISteamApps_BIsDlcInstalled, new SteamAPI_ISteamApps_BIsDlcInstalledDelegate(SteamAPI_ISteamApps_BIsDlcInstalled));
            base.Install<SteamAPI_ISteamApps_GetEarliestPurchaseUnixTimeDelegate>("SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime", _SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime, new SteamAPI_ISteamApps_GetEarliestPurchaseUnixTimeDelegate(SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime));
            base.Install<SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekendDelegate>("SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend", _SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend, new SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekendDelegate(SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend));
            base.Install<SteamAPI_ISteamApps_GetDLCCountDelegate>("SteamAPI_ISteamApps_GetDLCCount", _SteamAPI_ISteamApps_GetDLCCount, new SteamAPI_ISteamApps_GetDLCCountDelegate(SteamAPI_ISteamApps_GetDLCCount));
            base.Install<SteamAPI_ISteamApps_BGetDLCDataByIndexDelegate>("SteamAPI_ISteamApps_BGetDLCDataByIndex", _SteamAPI_ISteamApps_BGetDLCDataByIndex, new SteamAPI_ISteamApps_BGetDLCDataByIndexDelegate(SteamAPI_ISteamApps_BGetDLCDataByIndex));
            base.Install<SteamAPI_ISteamApps_InstallDLCDelegate>("SteamAPI_ISteamApps_InstallDLC", _SteamAPI_ISteamApps_InstallDLC, new SteamAPI_ISteamApps_InstallDLCDelegate(SteamAPI_ISteamApps_InstallDLC));
            base.Install<SteamAPI_ISteamApps_UninstallDLCDelegate>("SteamAPI_ISteamApps_UninstallDLC", _SteamAPI_ISteamApps_UninstallDLC, new SteamAPI_ISteamApps_UninstallDLCDelegate(SteamAPI_ISteamApps_UninstallDLC));
            base.Install<SteamAPI_ISteamApps_RequestAppProofOfPurchaseKeyDelegate>("SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey", _SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey, new SteamAPI_ISteamApps_RequestAppProofOfPurchaseKeyDelegate(SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey));
            base.Install<SteamAPI_ISteamApps_GetCurrentBetaNameDelegate>("SteamAPI_ISteamApps_GetCurrentBetaName", _SteamAPI_ISteamApps_GetCurrentBetaName, new SteamAPI_ISteamApps_GetCurrentBetaNameDelegate(SteamAPI_ISteamApps_GetCurrentBetaName));
            base.Install<SteamAPI_ISteamApps_MarkContentCorruptDelegate>("SteamAPI_ISteamApps_MarkContentCorrupt", _SteamAPI_ISteamApps_MarkContentCorrupt, new SteamAPI_ISteamApps_MarkContentCorruptDelegate(SteamAPI_ISteamApps_MarkContentCorrupt));
            base.Install<SteamAPI_ISteamApps_GetInstalledDepotsDelegate>("SteamAPI_ISteamApps_GetInstalledDepots", _SteamAPI_ISteamApps_GetInstalledDepots, new SteamAPI_ISteamApps_GetInstalledDepotsDelegate(SteamAPI_ISteamApps_GetInstalledDepots));
            //base.Install<GetInstalledDepotsDelegate>("GetInstalledDepots", _GetInstalledDepots, new GetInstalledDepotsDelegate(GetInstalledDepots));
            base.Install<SteamAPI_ISteamApps_GetAppInstallDirDelegate>("SteamAPI_ISteamApps_GetAppInstallDir", _SteamAPI_ISteamApps_GetAppInstallDir, new SteamAPI_ISteamApps_GetAppInstallDirDelegate(SteamAPI_ISteamApps_GetAppInstallDir));
            base.Install<SteamAPI_ISteamApps_BIsAppInstalledDelegate>("SteamAPI_ISteamApps_BIsAppInstalled", _SteamAPI_ISteamApps_BIsAppInstalled, new SteamAPI_ISteamApps_BIsAppInstalledDelegate(SteamAPI_ISteamApps_BIsAppInstalled));
            base.Install<SteamAPI_ISteamApps_GetAppOwnerDelegate>("SteamAPI_ISteamApps_GetAppOwner", _SteamAPI_ISteamApps_GetAppOwner, new SteamAPI_ISteamApps_GetAppOwnerDelegate(SteamAPI_ISteamApps_GetAppOwner));
            base.Install<SteamAPI_ISteamApps_GetLaunchQueryParamDelegate>("SteamAPI_ISteamApps_GetLaunchQueryParam", _SteamAPI_ISteamApps_GetLaunchQueryParam, new SteamAPI_ISteamApps_GetLaunchQueryParamDelegate(SteamAPI_ISteamApps_GetLaunchQueryParam));
            base.Install<SteamAPI_ISteamApps_GetDlcDownloadProgressDelegate>("SteamAPI_ISteamApps_GetDlcDownloadProgress", _SteamAPI_ISteamApps_GetDlcDownloadProgress, new SteamAPI_ISteamApps_GetDlcDownloadProgressDelegate(SteamAPI_ISteamApps_GetDlcDownloadProgress));
            base.Install<SteamAPI_ISteamApps_GetAppBuildIdDelegate>("SteamAPI_ISteamApps_GetAppBuildId", _SteamAPI_ISteamApps_GetAppBuildId, new SteamAPI_ISteamApps_GetAppBuildIdDelegate(SteamAPI_ISteamApps_GetAppBuildId));
            base.Install<SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeysDelegate>("SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeys", _SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeys, new SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeysDelegate(SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeys));
            base.Install<SteamAPI_ISteamApps_GetFileDetailsDelegate>("SteamAPI_ISteamApps_GetFileDetails", _SteamAPI_ISteamApps_GetFileDetails, new SteamAPI_ISteamApps_GetFileDetailsDelegate(SteamAPI_ISteamApps_GetFileDetails));
            base.Install<SteamAPI_ISteamApps_GetLaunchCommandLineDelegate>("SteamAPI_ISteamApps_GetLaunchCommandLine", _SteamAPI_ISteamApps_GetLaunchCommandLine, new SteamAPI_ISteamApps_GetLaunchCommandLineDelegate(SteamAPI_ISteamApps_GetLaunchCommandLine));
            base.Install<SteamAPI_ISteamApps_BIsSubscribedFromFamilySharingDelegate>("SteamAPI_ISteamApps_BIsSubscribedFromFamilySharing", _SteamAPI_ISteamApps_BIsSubscribedFromFamilySharing, new SteamAPI_ISteamApps_BIsSubscribedFromFamilySharingDelegate(SteamAPI_ISteamApps_BIsSubscribedFromFamilySharing));
            base.Install<SteamAPI_ISteamApps_BIsTimedTrialDelegate>("SteamAPI_ISteamApps_BIsTimedTrial", _SteamAPI_ISteamApps_BIsTimedTrial, new SteamAPI_ISteamApps_BIsTimedTrialDelegate(SteamAPI_ISteamApps_BIsTimedTrial));
        }


        public bool SteamAPI_ISteamApps_BIsSubscribed()
        {
            Write("SteamAPI_ISteamApps_BIsSubscribed");
            return SteamEmulator.SteamApps.BIsSubscribed();
        }

        public bool SteamAPI_ISteamApps_BIsLowViolence()
        {
            Write("SteamAPI_ISteamApps_BIsLowViolence");
            return SteamEmulator.SteamApps.BIsLowViolence();
        }

        public bool SteamAPI_ISteamApps_BIsCybercafe()
        {
            Write("SteamAPI_ISteamApps_BIsCybercafe");
            return SteamEmulator.SteamApps.BIsCybercafe();
        }

        public bool SteamAPI_ISteamApps_BIsVACBanned()
        {
            Write("SteamAPI_ISteamApps_BIsVACBanned");
            return SteamEmulator.SteamApps.BIsVACBanned();
        }

        public string SteamAPI_ISteamApps_GetCurrentGameLanguage()
        {
            Write("SteamAPI_ISteamApps_GetCurrentGameLanguage");
            return SteamEmulator.SteamApps.GetCurrentGameLanguage();
        }

        public string SteamAPI_ISteamApps_GetAvailableGameLanguages()
        {
            Write("SteamAPI_ISteamApps_GetAvailableGameLanguages");
            return SteamEmulator.SteamApps.GetAvailableGameLanguages();
        }

        public bool SteamAPI_ISteamApps_BIsSubscribedApp(uint appID)
        {
            Write("SteamAPI_ISteamApps_BIsSubscribedApp " + appID);
            return SteamEmulator.SteamApps.BIsSubscribedApp(appID);
        }


        public bool SteamAPI_ISteamApps_BIsDlcInstalled(uint appID)
        {
            Write("SteamAPI_ISteamApps_BIsDlcInstalled " + appID);
            return SteamEmulator.SteamApps.BIsDlcInstalled(appID);
        }

        public UInt32 SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime(uint nAppID)
        {
            Write("SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime");
            return SteamEmulator.SteamApps.GetEarliestPurchaseUnixTime(nAppID);
        }


        public bool SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend()
        {
            Write("SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend");
            return SteamEmulator.SteamApps.BIsSubscribedFromFreeWeekend();
        }


        public int SteamAPI_ISteamApps_GetDLCCount()
        {
            Write("SteamAPI_ISteamApps_GetDLCCount");
            return SteamEmulator.SteamApps.GetDLCCount();
        }


        public bool SteamAPI_ISteamApps_BGetDLCDataByIndex(int iDLC, uint pAppID, bool pbAvailable, string pchName, int cchNameBufferSize)
        {
            Write("SteamAPI_ISteamApps_BGetDLCDataByIndex");
            return SteamEmulator.SteamApps.BGetDLCDataByIndex(iDLC, pAppID, pbAvailable, pchName, cchNameBufferSize);
        }


        public void SteamAPI_ISteamApps_InstallDLC(uint nAppID)
        {
            Write("SteamAPI_ISteamApps_InstallDLC");
            SteamEmulator.SteamApps.InstallDLC(nAppID);
        }

        public void SteamAPI_ISteamApps_UninstallDLC(uint nAppID)
        {
            Write("SteamAPI_ISteamApps_UninstallDLC");
            SteamEmulator.SteamApps.UninstallDLC(nAppID);
        }


        public void SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey(uint nAppID)
        {
            Write("SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey");
            SteamEmulator.SteamApps.RequestAppProofOfPurchaseKey(nAppID);
        }

        public bool SteamAPI_ISteamApps_GetCurrentBetaName(string pchName, int cchNameBufferSize)
        {
            Write("SteamAPI_ISteamApps_GetCurrentBetaName " + cchNameBufferSize);
            return SteamEmulator.SteamApps.GetCurrentBetaName(pchName, cchNameBufferSize);
        }

        public bool SteamAPI_ISteamApps_MarkContentCorrupt(bool bMissingFilesOnly)
        {
            Write("SteamAPI_ISteamApps_MarkContentCorrupt");
            return SteamEmulator.SteamApps.MarkContentCorrupt(bMissingFilesOnly);
        }

        public UInt32 SteamAPI_ISteamApps_GetInstalledDepots(uint appID, uint pvecDepots, uint cMaxDepots)
        {
            Write($"SteamAPI_ISteamApps_GetInstalledDepots {appID}, {cMaxDepots}");
            return SteamEmulator.SteamApps.GetInstalledDepots(appID, pvecDepots, cMaxDepots);
        }

        //public UInt32 GetInstalledDepots(DepotId_t pvecDepots, UInt32 cMaxDepots)
        //{
        //}

        public UInt32 SteamAPI_ISteamApps_GetAppInstallDir(uint appID, string pchFolder, uint cchFolderBufferSize)
        {
            Write($"SteamAPI_ISteamApps_GetAppInstallDir {appID} ");
            return SteamEmulator.SteamApps.GetAppInstallDir(appID, pchFolder, cchFolderBufferSize);
        }

        public bool SteamAPI_ISteamApps_BIsAppInstalled(uint appID)
        {
            Write($"SteamAPI_ISteamApps_BIsAppInstalled {appID}");
            return SteamEmulator.SteamApps.BIsAppInstalled(appID);
        }

        public ulong SteamAPI_ISteamApps_GetAppOwner()
        {
            Write("SteamAPI_ISteamApps_GetAppOwner");
            return SteamEmulator.SteamApps.GetAppOwner();
        }

        public string SteamAPI_ISteamApps_GetLaunchQueryParam(string pchKey)
        {
            Write("SteamAPI_ISteamApps_GetLaunchQueryParam");
            return SteamEmulator.SteamApps.GetLaunchQueryParam(pchKey);
        }


        public bool SteamAPI_ISteamApps_GetDlcDownloadProgress(uint nAppID, UInt64 punBytesDownloaded, UInt64 punBytesTotal)
        {
            Write("SteamAPI_ISteamApps_GetDlcDownloadProgress");
            return SteamEmulator.SteamApps.GetDlcDownloadProgress(nAppID, punBytesDownloaded, punBytesTotal);
        }


        public int SteamAPI_ISteamApps_GetAppBuildId()
        {
            Write("SteamAPI_ISteamApps_GetAppBuildId");
            return SteamEmulator.SteamApps.GetAppBuildId();
        }


        public void SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeys()
        {
            Write("SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeys");
            SteamEmulator.SteamApps.RequestAllProofOfPurchaseKeys();
        }

        public ulong SteamAPI_ISteamApps_GetFileDetails(string pszFileName)
        {
            Write("SteamAPI_ISteamApps_GetFileDetails");
            return SteamEmulator.SteamApps.GetFileDetails(pszFileName);
        }

        public int SteamAPI_ISteamApps_GetLaunchCommandLine(string pszCommandLine, int cubCommandLine)
        {
            Write($"SteamAPI_ISteamApps_GetLaunchCommandLine {pszCommandLine}");
            return SteamEmulator.SteamApps.GetLaunchCommandLine(pszCommandLine, cubCommandLine);
        }

        public bool SteamAPI_ISteamApps_BIsSubscribedFromFamilySharing()
        {
            Write("SteamAPI_ISteamApps_BIsSubscribedFromFamilySharing");
            return SteamEmulator.SteamApps.BIsSubscribedFromFamilySharing();
        }

        public bool SteamAPI_ISteamApps_BIsTimedTrial(UInt32 punSecondsAllowed, UInt32 punSecondsPlayed)
        {
            Write("SteamAPI_ISteamApps_BIsTimedTrial");
            return SteamEmulator.SteamApps.BIsTimedTrial(punSecondsAllowed, punSecondsPlayed);
        }
        public override void Write(object v)
        {
            Main.Write("SteamAPI_ISteamApps : SteamAPI_ISteamApps", v);
        }
    }
}
