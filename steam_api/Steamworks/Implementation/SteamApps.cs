using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamApps : ISteamInterface
    {
        public SteamApps()
        {
            InterfaceVersion = "SteamApps";
        }

        public bool BIsSubscribed(IntPtr _)
        {
            Write("BIsSubscribed");
            return true;
        }

        public bool BIsLowViolence(IntPtr _)
        {
            Write("BIsLowViolence");
            return false;
        }

        public bool BIsCybercafe(IntPtr _)
        {
            Write("BIsCybercafe");
            return false;
        }

        public bool BIsVACBanned(IntPtr _)
        {
            Write("BIsVACBanned");
            return false;
        }

        public string GetCurrentGameLanguage(IntPtr _)
        {
            Write("GetCurrentGameLanguage");
            return SteamEmulator.Language;
        }

        public string GetAvailableGameLanguages(IntPtr _)
        {
            Write("GetAvailableGameLanguages");
            //TODO?
            return "";
        }

        public bool BIsSubscribedApp(uint appID)
        {
            Write("BIsSubscribedApp " + appID);
            return false;
        }

        public bool BIsDlcInstalled(uint appID)
        {
            Write("BIsDlcInstalled " + appID);
            return false;

        }

        public UInt32 GetEarliestPurchaseUnixTime(uint nAppID)
        {
            Write("GetEarliestPurchaseUnixTime");
            //TODO ?
            return 1;
        }

        public bool BIsSubscribedFromFreeWeekend(IntPtr _)
        {
            Write("BIsSubscribedFromFreeWeekend");
            return false;
        }

        public int GetDLCCount(IntPtr _)
        {
            Write("GetDLCCount");
            return 0;
        }

        public bool BGetDLCDataByIndex(int iDLC, uint pAppID, bool pbAvailable, string pchName, int cchNameBufferSize)
        {
            Write("BGetDLCDataByIndex");
            return true;
        }

        public void InstallDLC(uint nAppID)
        {
            Write("InstallDLC");
        }

        public void UninstallDLC(uint nAppID)
        {
            Write("UninstallDLC");
        }

        public void RequestAppProofOfPurchaseKey(uint nAppID)
        {
            Write("RequestAppProofOfPurchaseKey");
        }

        public bool GetCurrentBetaName(string pchName, int cchNameBufferSize)
        {
            Write("GetCurrentBetaName " + pchName);
            return true;
        }

        public bool MarkContentCorrupt(bool bMissingFilesOnly)
        {
            Write("MarkContentCorrupt");
            return true;
        }

        public UInt32 GetInstalledDepots(uint appID, uint pvecDepots, uint cMaxDepots)
        {
            Write($"GetInstalledDepots {appID}, {cMaxDepots}");
            return 0;
        }

        public UInt32 GetAppInstallDir(uint appID, string pchFolder, uint cchFolderBufferSize)
        {
            Write($"GetAppInstallDir {appID} {pchFolder} {cchFolderBufferSize}");
            string installed_path = "xd";
            return (UInt32)installed_path.Length;
        }

        public bool BIsAppInstalled(uint appID)
        {
            Write($"BIsAppInstalled {appID}");
            return true;
        }

        public ulong GetAppOwner()
        {
            Write("GetAppOwner");
            return 0;
        }

        public string GetLaunchQueryParam(string pchKey)
        {
            Write("GetLaunchQueryParam");
            return "";
        }

        public bool GetDlcDownloadProgress(uint nAppID, UInt64 punBytesDownloaded, UInt64 punBytesTotal)
        {
            Write("GetDlcDownloadProgress");
            return false;
        }


        public int GetAppBuildId(IntPtr _)
        {
            Write("GetAppBuildId");
            return 10;
        }

        public void RequestAllProofOfPurchaseKeys(IntPtr _)
        {
            Write("RequestAllProofOfPurchaseKeys");
        }

        public ulong GetFileDetails(string pszFileName)
        {
            Write("GetFileDetails");
            return 0;
        }

        public bool BIsSubscribedFromFamilySharing(IntPtr _)
        {
            Write("BIsSubscribedFromFamilySharing");
            return false;
        }

        public bool BIsTimedTrial(UInt32 punSecondsAllowed, UInt32 punSecondsPlayed)
        {
            Write("BIsTimedTrial");
            return false;
        }

        public int GetLaunchCommandLine(string pszCommandLine, int cubCommandLine)
        {
            Write("GetLaunchCommandLine");
            return 0;
        }
    }
}
