using System;
using System.Runtime.InteropServices;
using Core.Interface;
using SKYNET;
using SKYNET.Helper;
using SKYNET.Interface;
using Steamworks;

//[Map("SteamApps")]
public class SteamApps : IBaseInterface, ISteamApps
{

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

    // Request legacy cd-key for yourself or owned DLC. If you are interested in this
    // data then make sure you provide us with a list of valid keys to be distributed
    // to users when they purchase the game, before the game ships.
    // You'll receive an AppProofOfPurchaseKeyResponse_t callback when
    // the key is available (which may be immediately).

    public void RequestAppProofOfPurchaseKey(AppId_t nAppID)
    {
        Write("RequestAppProofOfPurchaseKey");
    }

    // returns current beta branch name, 'public' is the default branch

    public bool GetCurrentBetaName(IntPtr pchName, int cchNameBufferSize)
    {
        Write("GetCurrentBetaName " + cchNameBufferSize);
        return true;
    }

    // signal Steam that game files seems corrupt or missing

    public bool MarkContentCorrupt(bool bMissingFilesOnly)
    {
        Write("MarkContentCorrupt");
        //TODO: warn user
        return true;
    }

    // return installed depots in mount order

    public UInt32 GetInstalledDepots(AppId_t appID, IntPtr pvecDepots, uint cMaxDepots)
    {
        Write($"GetInstalledDepots {appID}, {cMaxDepots}");
        //TODO not sure about the behavior of this function, I didn't actually test this.
        return 0;
    }

    //
    //public UInt32 GetInstalledDepots(DepotId_t pvecDepots, UInt32 cMaxDepots)
    //{
    //    PRINT_DEBUG("GetInstalledDepots old");
    //    return 0;
    //}

    // returns current app install folder for AppID, returns folder name length

    public UInt32 GetAppInstallDir(AppId_t appID, IntPtr pchFolder, uint cchFolderBufferSize)
    {
        Write($"GetAppInstallDir {appID} {pchFolder} {cchFolderBufferSize}");
        //TODO return real path instead of dll path
        string installed_path = "xd";
        return (UInt32)installed_path.Length; //Real steam always returns the actual path length, not the copied one.
    }

    // returns true if that app is installed (not necessarily owned)

    public bool BIsAppInstalled(AppId_t appID)
    {
        Write($"BIsAppInstalled {appID}");
        return true;
    }

    // returns the SteamID of the original owner. If different from current user, it's borrowed

    public IntPtr GetAppOwner(IntPtr _)
    {
        Write("GetAppOwner");
        return IntPtr.Zero;
    }

    // Returns the associated launch param if the game is run via steam://run/<appid>//?param1=value1;param2=value2;param3=value3 etc.
    // Parameter names starting with the character '@' are reserved for internal use and will always return and empty string.
    // Parameter names starting with an underscore '_' are reserved for steam features -- they can be queried by the game,
    // but it is advised that you not param names beginning with an underscore for your own features.

    public string GetLaunchQueryParam(string pchKey)
    {
        Write("GetLaunchQueryParam");
        return "";
    }


    // get download progress for optional DLC

    public bool GetDlcDownloadProgress(AppId_t nAppID, UInt64 punBytesDownloaded, UInt64 punBytesTotal)
    {
        Write("GetDlcDownloadProgress");
        return false;
    }


    // return the buildid of this app, may change at any time based on backend updates to the game

    public int GetAppBuildId(IntPtr _)
    {
        Write("GetAppBuildId");
        return 10;
    }


    // Request all proof of purchase keys for the calling appid and asociated DLC.
    // A series of AppProofOfPurchaseKeyResponse_t callbacks will be sent with
    // appropriate appid values, ending with a final callback where the m_nAppId
    // member is k_uAppIdInvalid (zero).

    public void RequestAllProofOfPurchaseKeys(IntPtr _)
    {
        Write("RequestAllProofOfPurchaseKeys");
    }


    //STEAM_CALL_RESULT(FileDetailsResult_t )

    public SteamAPICall_t GetFileDetails(string pszFileName)
    {
        Write("GetFileDetails");
        return (SteamAPICall_t)0;
    }

    // Get command line if game was launched via Steam URL, e.g. steam://run/<appid>//<command line>/.
    // This method of passing a connect string (used when joining via rich presence, accepting an
    // invite, etc) is preferable to passing the connect string on the operating system command
    // line, which is a security risk.  In order for rich presence joins to go through this
    // path and not be placed on the OS command line, you must set a value in your app's
    // configuration on Steam.  Ask Valve for help with this.
    //
    // If game was already running and launched again, the NewUrlLaunchParameters_t will be fired.

    public int GetLaunchCommandLine(IntPtr pszCommandLine, int cubCommandLine)
    {
        Write("GetLaunchCommandLine");
        return 0;
    }

    // Check if user borrowed this game via Family Sharing, If true, call GetAppOwner(IntPtr _) to get the lender SteamID

    public bool BIsSubscribedFromFamilySharing(IntPtr _)
    {
        Write("BIsSubscribedFromFamilySharing");
        return false;
    }

    // check if game is a timed trial with limited playtime

    public bool BIsTimedTrial(UInt32 punSecondsAllowed, UInt32 punSecondsPlayed)
    {
        Write("BIsTimedTrial");
        return false;
    }

    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}
