using System;
using SKYNET.Helpers;

namespace SKYNET.Steamworks.Interfaces
{
    /// <summary>
    /// ISteamApps v009. Method order is the native vtable contract; keep it aligned
    /// with isteamapps.h rather than collapsing it into a newer interface version.
    /// </summary>
    [Interface("STEAMAPPS_INTERFACE_VERSION009")]
    public class SteamApps009 : ISteamInterface
    {
        public bool BIsSubscribed(IntPtr _) => SteamEmulator.SteamApps.BIsSubscribed();
        public bool BIsLowViolence(IntPtr _) => SteamEmulator.SteamApps.BIsLowViolence();
        public bool BIsCybercafe(IntPtr _) => SteamEmulator.SteamApps.BIsCybercafe();
        public bool BIsVACBanned(IntPtr _) => SteamEmulator.SteamApps.BIsVACBanned();
        public IntPtr GetCurrentGameLanguage(IntPtr _) => SteamEmulator.SteamApps.GetCurrentGameLanguage();
        public IntPtr GetAvailableGameLanguages(IntPtr _) => SteamEmulator.SteamApps.GetAvailableGameLanguages();
        public bool BIsSubscribedApp(IntPtr _, uint appID) => SteamEmulator.SteamApps.BIsSubscribedApp(appID);
        public bool BIsDlcInstalled(IntPtr _, uint appID) => SteamEmulator.SteamApps.BIsDlcInstalled(appID);
        public uint GetEarliestPurchaseUnixTime(IntPtr _, uint appID) => SteamEmulator.SteamApps.GetEarliestPurchaseUnixTime(appID);
        public bool BIsSubscribedFromFreeWeekend(IntPtr _) => SteamEmulator.SteamApps.BIsSubscribedFromFreeWeekend();
        public int GetDLCCount(IntPtr _) => SteamEmulator.SteamApps.GetDLCCount();

        public bool BGetDLCDataByIndex(IntPtr _, int index, IntPtr appID, IntPtr available, IntPtr name, int nameLength)
        {
            return SteamEmulator.SteamApps.BGetDLCDataByIndex(index, appID, available, name, nameLength);
        }

        public void InstallDLC(IntPtr _, uint appID) => SteamEmulator.SteamApps.InstallDLC(appID);
        public void UninstallDLC(IntPtr _, uint appID) => SteamEmulator.SteamApps.UninstallDLC(appID);
        public void RequestAppProofOfPurchaseKey(IntPtr _, uint appID) => SteamEmulator.SteamApps.RequestAppProofOfPurchaseKey(appID);
        public bool GetCurrentBetaName(IntPtr _, IntPtr name, int nameLength) => SteamEmulator.SteamApps.GetCurrentBetaName(name, nameLength);
        public bool MarkContentCorrupt(IntPtr _, bool missingFilesOnly) => SteamEmulator.SteamApps.MarkContentCorrupt(missingFilesOnly);
        public uint GetInstalledDepots(IntPtr _, uint appID, IntPtr depots, uint maxDepots) => SteamEmulator.SteamApps.GetInstalledDepots(appID, depots, maxDepots);
        public uint GetAppInstallDir(IntPtr _, uint appID, IntPtr folder, uint folderLength) => SteamEmulator.SteamApps.GetAppInstallDir(appID, folder, folderLength);
        public bool BIsAppInstalled(IntPtr _, uint appID) => SteamEmulator.SteamApps.BIsAppInstalled(appID);
        public IntPtr GetAppOwner(IntPtr _, IntPtr steamId) => NativeSteamId.Write(steamId, SteamEmulator.SteamApps.GetAppOwner());
        public IntPtr GetLaunchQueryParam(IntPtr _, string key) => SteamEmulator.SteamApps.GetLaunchQueryParam(key);
        public bool GetDlcDownloadProgress(IntPtr _, uint appID, IntPtr downloaded, IntPtr total) => SteamEmulator.SteamApps.GetDlcDownloadProgress(appID, downloaded, total);
        public int GetAppBuildId(IntPtr _) => SteamEmulator.SteamApps.GetAppBuildId();
        public void RequestAllProofOfPurchaseKeys(IntPtr _) => SteamEmulator.SteamApps.RequestAllProofOfPurchaseKeys();
        public ulong GetFileDetails(IntPtr _, string fileName) => SteamEmulator.SteamApps.GetFileDetails(fileName);
        public int GetLaunchCommandLine(IntPtr _, IntPtr commandLine, int commandLineLength) => SteamEmulator.SteamApps.GetLaunchCommandLine(commandLine, commandLineLength);
        public bool BIsSubscribedFromFamilySharing(IntPtr _) => SteamEmulator.SteamApps.BIsSubscribedFromFamilySharing();
        public bool BIsTimedTrial(IntPtr _, IntPtr secondsAllowed, IntPtr secondsPlayed) => SteamEmulator.SteamApps.BIsTimedTrial(secondsAllowed, secondsPlayed);
        public bool SetDlcContext(IntPtr _, uint appID) => SteamEmulator.SteamApps.SetDlcContext(appID);
        public int GetNumBetas(IntPtr _, IntPtr available, IntPtr privateBetas) => SteamEmulator.SteamApps.GetNumBetas(available, privateBetas);

        public bool GetBetaInfo(
            IntPtr _,
            int betaIndex,
            IntPtr flags,
            IntPtr buildId,
            IntPtr betaName,
            int betaNameLength,
            IntPtr description,
            int descriptionLength,
            IntPtr lastUpdated)
        {
            return SteamEmulator.SteamApps.GetBetaInfo(
                betaIndex,
                flags,
                buildId,
                betaName,
                betaNameLength,
                description,
                descriptionLength,
                lastUpdated);
        }

        public bool SetActiveBeta(IntPtr _, string betaName) => SteamEmulator.SteamApps.SetActiveBeta(betaName);
    }
}
