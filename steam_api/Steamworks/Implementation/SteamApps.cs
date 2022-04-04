using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helper;
using Steamworks;

public class SteamApps : ISteamInterface
{
    public IntPtr MemoryAddress { get; set; }
    public string InterfaceVersion { get; set; }


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

    public bool BIsSubscribedApp(AppId_t appID)
    {
        Write("BIsSubscribedApp " + appID);
        return false;
    }

    public bool BIsDlcInstalled(AppId_t appID)
    {
        Write("BIsDlcInstalled " + appID);
        return false;

    }

    public UInt32 GetEarliestPurchaseUnixTime(AppId_t nAppID)
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

    public bool BGetDLCDataByIndex(int iDLC, IntPtr pAppID, bool pbAvailable, IntPtr pchName, int cchNameBufferSize)
    {
        Write("BGetDLCDataByIndex");
        return true;
    }

    public void InstallDLC(AppId_t nAppID)
    {
        Write("InstallDLC");
    }

    public void UninstallDLC(AppId_t nAppID)
    {
        Write("UninstallDLC");
    }

    public void RequestAppProofOfPurchaseKey(AppId_t nAppID)
    {
        Write("RequestAppProofOfPurchaseKey");
    }

    public bool GetCurrentBetaName(IntPtr pchName, int cchNameBufferSize)
    {
        Write("GetCurrentBetaName " + cchNameBufferSize);
        return true;
    }

    public bool MarkContentCorrupt(bool bMissingFilesOnly)
    {
        Write("MarkContentCorrupt");
        return true;
    }

    public UInt32 GetInstalledDepots(AppId_t appID, IntPtr pvecDepots, uint cMaxDepots)
    {
        Write($"GetInstalledDepots {appID}, {cMaxDepots}");
        return 0;
    }

    public UInt32 GetAppInstallDir(AppId_t appID, IntPtr pchFolder, uint cchFolderBufferSize)
    {
        Write($"GetAppInstallDir {appID} {pchFolder} {cchFolderBufferSize}");
        string installed_path = "xd";
        return (UInt32)installed_path.Length; 
    }

    public bool BIsAppInstalled(AppId_t appID)
    {
        Write($"BIsAppInstalled {appID}");
        return true;
    }

    public IntPtr GetAppOwner(IntPtr _)
    {
        Write("GetAppOwner");
        return IntPtr.Zero;
    }

    public string GetLaunchQueryParam(string pchKey)
    {
        Write("GetLaunchQueryParam");
        return "";
    }

    public bool GetDlcDownloadProgress(AppId_t nAppID, UInt64 punBytesDownloaded, UInt64 punBytesTotal)
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

    public SteamAPICall_t GetFileDetails(string pszFileName)
    {
        Write("GetFileDetails");
        return (SteamAPICall_t)0;
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

    public int GetLaunchCommandLine(IntPtr pszCommandLine, int cubCommandLine)
    {
        Write("GetLaunchCommandLine");
        return 0;
    }

    private void Write(string v)
    {
        SteamEmulator.Write(InterfaceVersion, v);
    }
}
