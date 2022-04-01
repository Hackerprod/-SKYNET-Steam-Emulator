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
            return true;
        }

        public bool SteamAPI_ISteamApps_BIsLowViolence()
        {
            Write("SteamAPI_ISteamApps_BIsLowViolence");
            return false;
        }

        public bool SteamAPI_ISteamApps_BIsCybercafe()
        {
            Write("SteamAPI_ISteamApps_BIsCybercafe");
            return false;
        }

        public bool SteamAPI_ISteamApps_BIsVACBanned()
        {
            Write("SteamAPI_ISteamApps_BIsVACBanned");
            return false;
        }

        public string SteamAPI_ISteamApps_GetCurrentGameLanguage()
        {
            Write("SteamAPI_ISteamApps_GetCurrentGameLanguage");
            return SteamEmulator.Language;
        }

        public string SteamAPI_ISteamApps_GetAvailableGameLanguages()
        {
            Write("SteamAPI_ISteamApps_GetAvailableGameLanguages");
            //TODO?
            return "";
        }

        public bool SteamAPI_ISteamApps_BIsSubscribedApp(AppId_t appID)
        {
            Write("SteamAPI_ISteamApps_BIsSubscribedApp " + appID);
            return false;
        }


        public bool SteamAPI_ISteamApps_BIsDlcInstalled(AppId_t appID)
        {
            Write("SteamAPI_ISteamApps_BIsDlcInstalled " + appID);
            return false;

        }

        public UInt32 SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime(AppId_t nAppID)
        {
            Write("SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime");
            //TODO ?
            return 1;
        }


        public bool SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend()
        {
            Write("SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend");
            return false;
        }


        public int SteamAPI_ISteamApps_GetDLCCount()
        {
            Write("SteamAPI_ISteamApps_GetDLCCount");
            return 0;
        }


        public bool SteamAPI_ISteamApps_BGetDLCDataByIndex(int iDLC, IntPtr pAppID, bool pbAvailable, IntPtr pchName, int cchNameBufferSize)
        {
            Write("SteamAPI_ISteamApps_BGetDLCDataByIndex");
            return true;
        }


        public void SteamAPI_ISteamApps_InstallDLC(AppId_t nAppID)
        {
            Write("SteamAPI_ISteamApps_InstallDLC");
        }

        public void SteamAPI_ISteamApps_UninstallDLC(AppId_t nAppID)
        {
            Write("SteamAPI_ISteamApps_UninstallDLC");
        }


        public void SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey(AppId_t nAppID)
        {
            Write("SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey");
        }

        public bool SteamAPI_ISteamApps_GetCurrentBetaName(IntPtr pchName, int cchNameBufferSize)
        {
            Write("SteamAPI_ISteamApps_GetCurrentBetaName " + cchNameBufferSize);
            return true;
        }

        public bool SteamAPI_ISteamApps_MarkContentCorrupt(bool bMissingFilesOnly)
        {
            Write("SteamAPI_ISteamApps_MarkContentCorrupt");
            //TODO: warn user
            return true;
        }

        public UInt32 SteamAPI_ISteamApps_GetInstalledDepots(AppId_t appID, IntPtr pvecDepots, uint cMaxDepots)
        {
            Write($"SteamAPI_ISteamApps_GetInstalledDepots {appID}, {cMaxDepots}");
            //TODO not sure about the behavior of this function, I didn't actually test this.
            return 0;
        }

        //public UInt32 GetInstalledDepots(DepotId_t pvecDepots, UInt32 cMaxDepots)
        //{
        //}

        public UInt32 SteamAPI_ISteamApps_GetAppInstallDir(AppId_t appID, IntPtr pchFolder, uint cchFolderBufferSize)
        {
            Write($"SteamAPI_ISteamApps_GetAppInstallDir {appID.m_AppId} ");
            //TODO return real path instead of dll path
            string installed_path = "xd";
            return (UInt32)installed_path.Length; //Real steam always returns the actual path length, not the copied one.
        }

        public bool SteamAPI_ISteamApps_BIsAppInstalled(AppId_t appID)
        {
            Write($"SteamAPI_ISteamApps_BIsAppInstalled {appID}");
            return true;
        }

        public IntPtr SteamAPI_ISteamApps_GetAppOwner()
        {
            Write("SteamAPI_ISteamApps_GetAppOwner");
            return SteamEmulator.SteamApps.MemoryAddress;
        }

        public string SteamAPI_ISteamApps_GetLaunchQueryParam(string pchKey)
        {
            Write("SteamAPI_ISteamApps_GetLaunchQueryParam");
            return "";
        }


        public bool SteamAPI_ISteamApps_GetDlcDownloadProgress(AppId_t nAppID, UInt64 punBytesDownloaded, UInt64 punBytesTotal)
        {
            Write("SteamAPI_ISteamApps_GetDlcDownloadProgress");
            return false;
        }


        public int SteamAPI_ISteamApps_GetAppBuildId()
        {
            Write("SteamAPI_ISteamApps_GetAppBuildId");
            return 10;
        }


        public void SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeys()
        {
            Write("SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeys");
        }

        public SteamAPICall_t SteamAPI_ISteamApps_GetFileDetails(string pszFileName)
        {
            Write("SteamAPI_ISteamApps_GetFileDetails");
            return (SteamAPICall_t)0;
        }

        public int SteamAPI_ISteamApps_GetLaunchCommandLine(IntPtr pszCommandLine, int cubCommandLine)
        {
            string commandLine = Marshal.PtrToStringUni(pszCommandLine);
            Write($"SteamAPI_ISteamApps_GetLaunchCommandLine {commandLine}");
            return SteamEmulator.SteamApps.GetLaunchCommandLine(pszCommandLine, cubCommandLine);
        }

        public bool SteamAPI_ISteamApps_BIsSubscribedFromFamilySharing()
        {
            Write("SteamAPI_ISteamApps_BIsSubscribedFromFamilySharing");
            return false;
        }

        public bool SteamAPI_ISteamApps_BIsTimedTrial(UInt32 punSecondsAllowed, UInt32 punSecondsPlayed)
        {
            Write("SteamAPI_ISteamApps_BIsTimedTrial");
            return false;
        }
        public override void Write(object v)
        {
            Main.Write("Hook : SteamAPI_ISteamApps", v);
        }
    }
}
