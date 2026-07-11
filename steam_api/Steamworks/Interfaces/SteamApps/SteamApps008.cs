using SKYNET.Steamworks;
using System;
using SKYNET.Helpers;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("STEAMAPPS_INTERFACE_VERSION008")]
    public class SteamApps008 : ISteamInterface
    {
        public bool BIsSubscribed(IntPtr _)
        {
            return SteamEmulator.SteamApps.BIsSubscribed();
        }

        public bool BIsLowViolence(IntPtr _)
        {
            return SteamEmulator.SteamApps.BIsLowViolence();
        }

        public bool BIsCybercafe(IntPtr _)
        {
            return SteamEmulator.SteamApps.BIsCybercafe();
        }

        public bool BIsVACBanned(IntPtr _)
        {
            return SteamEmulator.SteamApps.BIsVACBanned();
        }

        public IntPtr GetCurrentGameLanguage(IntPtr _)
        {
            return SteamEmulator.SteamApps.GetCurrentGameLanguage();
        }

        public IntPtr GetAvailableGameLanguages(IntPtr _)
        {
            return SteamEmulator.SteamApps.GetAvailableGameLanguages();
        }

        public bool BIsSubscribedApp(IntPtr _, uint appID)
        {
            return SteamEmulator.SteamApps.BIsSubscribedApp(appID);
        }

        public bool BIsDlcInstalled(IntPtr _, uint appID)
        {
            return SteamEmulator.SteamApps.BIsDlcInstalled(appID);
        }

        public uint GetEarliestPurchaseUnixTime(IntPtr _, uint nAppID)
        {
            return SteamEmulator.SteamApps.GetEarliestPurchaseUnixTime(nAppID);
        }

        public bool BIsSubscribedFromFreeWeekend(IntPtr _)
        {
            return SteamEmulator.SteamApps.BIsSubscribedFromFreeWeekend();
        }

        public int GetDLCCount(IntPtr _)
        {
            return SteamEmulator.SteamApps.GetDLCCount();
        }

        public bool BGetDLCDataByIndex(IntPtr _, int iDLC, IntPtr pAppID, IntPtr pbAvailable, IntPtr pchName, int cchNameBufferSize)
        {
            return SteamEmulator.SteamApps.BGetDLCDataByIndex(iDLC, pAppID, pbAvailable, pchName, cchNameBufferSize);
        }

        public void InstallDLC(IntPtr _, uint nAppID)
        {
            SteamEmulator.SteamApps.InstallDLC(nAppID);
        }

        public void UninstallDLC(IntPtr _, uint nAppID)
        {
            SteamEmulator.SteamApps.UninstallDLC(nAppID);
        }

        public void RequestAppProofOfPurchaseKey(IntPtr _, uint nAppID)
        {
            SteamEmulator.SteamApps.RequestAppProofOfPurchaseKey(nAppID);
        }

        public bool GetCurrentBetaName(IntPtr _, IntPtr pchName, int cchNameBufferSize)  
        {
            return SteamEmulator.SteamApps.GetCurrentBetaName(pchName, cchNameBufferSize);
        }

        public bool MarkContentCorrupt(IntPtr _, bool bMissingFilesOnly) 
        {
            return SteamEmulator.SteamApps.MarkContentCorrupt(bMissingFilesOnly);
        }

        public uint GetInstalledDepots(IntPtr _, uint appID, IntPtr pvecDepots, uint cMaxDepots)
        {
            return SteamEmulator.SteamApps.GetInstalledDepots(appID, pvecDepots, cMaxDepots);
        }

        public uint GetAppInstallDir(IntPtr _, uint appID, IntPtr pchFolder, uint cchFolderBufferSize)
        {
            return SteamEmulator.SteamApps.GetAppInstallDir(appID, pchFolder, cchFolderBufferSize);
        }

        public bool BIsAppInstalled(IntPtr _, uint appID) 
        {
            return SteamEmulator.SteamApps.BIsAppInstalled(appID);
        }

        public IntPtr GetAppOwner(IntPtr _, IntPtr pSteamID)
        {
            return NativeSteamId.Write(pSteamID, SteamEmulator.SteamApps.GetAppOwner());
        }

        public IntPtr GetLaunchQueryParam(IntPtr _, string pchKey)
        {
            return SteamEmulator.SteamApps.GetLaunchQueryParam(pchKey);
        }

        public bool GetDlcDownloadProgress(IntPtr _, uint nAppID, IntPtr punBytesDownloaded, IntPtr punBytesTotal)
        {
            return SteamEmulator.SteamApps.GetDlcDownloadProgress(nAppID, punBytesDownloaded, punBytesTotal);
        }

        public int GetAppBuildId(IntPtr _)
        {
            return SteamEmulator.SteamApps.GetAppBuildId();
        }

        public void RequestAllProofOfPurchaseKeys(IntPtr _)
        {
            SteamEmulator.SteamApps.RequestAllProofOfPurchaseKeys();
        }

        public ulong GetFileDetails(IntPtr _, string pszFileName)
        {
            return SteamEmulator.SteamApps.GetFileDetails(pszFileName);
        }

        public int GetLaunchCommandLine(IntPtr _, IntPtr pszCommandLine, int cubCommandLine)
        {
            return SteamEmulator.SteamApps.GetLaunchCommandLine(pszCommandLine, cubCommandLine);
        }

        public bool BIsSubscribedFromFamilySharing(IntPtr _)
        {
            return SteamEmulator.SteamApps.BIsSubscribedFromFamilySharing();
        }

        public bool BIsTimedTrial(IntPtr _, IntPtr punSecondsAllowed, IntPtr punSecondsPlayed)
        {
            return SteamEmulator.SteamApps.BIsTimedTrial(punSecondsAllowed, punSecondsPlayed);
        }

        // Slots 29-32 are part of SteamApps008 in SDK 1.60+ even though the
        // interface version string remained unchanged.
        public bool SetDlcContext(IntPtr _, uint nAppID)
        {
            return SteamEmulator.SteamApps.SetDlcContext(nAppID);
        }

        public int GetNumBetas(IntPtr _, IntPtr pnAvailable, IntPtr pnPrivate)
        {
            return SteamEmulator.SteamApps.GetNumBetas(pnAvailable, pnPrivate);
        }

        public bool GetBetaInfo(
            IntPtr _,
            int iBetaIndex,
            IntPtr punFlags,
            IntPtr punBuildID,
            IntPtr pchBetaName,
            int cchBetaName,
            IntPtr pchDescription,
            int cchDescription)
        {
            return SteamEmulator.SteamApps.GetBetaInfo(
                iBetaIndex,
                punFlags,
                punBuildID,
                pchBetaName,
                cchBetaName,
                pchDescription,
                cchDescription);
        }

        public bool SetActiveBeta(IntPtr _, string pchBetaName)
        {
            return SteamEmulator.SteamApps.SetActiveBeta(pchBetaName);
        }
    }
}
