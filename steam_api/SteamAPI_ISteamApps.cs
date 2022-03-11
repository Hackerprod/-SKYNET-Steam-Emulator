using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SKYNET.Interface;
using Steamworks;

public class SteamAPI_ISteamApps : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamApps_BIsSubscribed()
    {
        Write("SteamAPI_ISteamApps_BIsSubscribed");
        return true;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamApps_BIsLowViolence()
    {
        Write("SteamAPI_ISteamApps_BIsLowViolence");
        return false;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamApps_BIsCybercafe()
    {
        Write("SteamAPI_ISteamApps_BIsCybercafe");
        return false;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamApps_BIsVACBanned()
    {
        Write("SteamAPI_ISteamApps_BIsVACBanned");
        return false;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamApps_GetCurrentGameLanguage()
    {
        Write("SteamAPI_ISteamApps_GetCurrentGameLanguage");
        return SteamClient.Language; ;
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamApps_GetAvailableGameLanguages()
    {
        Write("SteamAPI_ISteamApps_GetAvailableGameLanguages");
        //TODO?
        return "";
    }

    // only use this member if you need to check ownership of another game related to yours, a demo for example
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamApps_BIsSubscribedApp(AppId_t appID)
    {
        Write("SteamAPI_ISteamApps_BIsSubscribedApp " + appID);
        return false;
    }


    // Takes AppID of DLC and checks if the user owns the DLC & if the DLC is installed
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamApps_BIsDlcInstalled(AppId_t appID)
    {
        Write("SteamAPI_ISteamApps_BIsDlcInstalled " + appID);
        return false;

    }

    // returns the Unix time of the purchase of the app
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static UInt32 SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime(AppId_t nAppID)
    {
        Write("SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime");
        //TODO ?
        return 1;
    }


    // Checks if the user is subscribed to the current app through a free weekend
    // This function will return false for users who have a retail or other type of license
    // Before using, please ask your Valve technical contact how to package and secure your free weekened
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend()
    {
        Write("SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend");
        return false;
    }


    // Returns the number of DLC pieces for the running app
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamApps_GetDLCCount()
    {
        Write("SteamAPI_ISteamApps_GetDLCCount");
        return 0;
    }


    // Returns metadata for DLC by index, of range [0, GetDLCCount()]
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamApps_BGetDLCDataByIndex(int iDLC, IntPtr pAppID, bool pbAvailable, IntPtr pchName, int cchNameBufferSize)
    {
        Write("SteamAPI_ISteamApps_BGetDLCDataByIndex");
        return true;
    }


    // Install/Uninstall control for optional DLC
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamApps_InstallDLC(AppId_t nAppID)
    {
        Write("SteamAPI_ISteamApps_InstallDLC");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamApps_UninstallDLC(AppId_t nAppID)
    {
        Write("SteamAPI_ISteamApps_UninstallDLC");
    }


    // Request legacy cd-key for yourself or owned DLC. If you are interested in this
    // data then make sure you provide us with a list of valid keys to be distributed
    // to users when they purchase the game, before the game ships.
    // You'll receive an AppProofOfPurchaseKeyResponse_t callback when
    // the key is available (which may be immediately).
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey(AppId_t nAppID)
    {
        Write("SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey");
    }

    // returns current beta branch name, 'public' is the default branch
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamApps_GetCurrentBetaName(IntPtr pchName, int cchNameBufferSize)
    {
        Write("SteamAPI_ISteamApps_GetCurrentBetaName " + cchNameBufferSize);
        return true;
    }

    // signal Steam that game files seems corrupt or missing
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamApps_MarkContentCorrupt(bool bMissingFilesOnly)
    {
        Write("SteamAPI_ISteamApps_MarkContentCorrupt");
        //TODO: warn user
        return true;
    }

    // return installed depots in mount order
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static UInt32 SteamAPI_ISteamApps_GetInstalledDepots(AppId_t appID, IntPtr pvecDepots, uint cMaxDepots)
    {
        Write($"SteamAPI_ISteamApps_GetInstalledDepots {appID}, {cMaxDepots}");
        //TODO not sure about the behavior of this function, I didn't actually test this.
        return 0;
    }

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static UInt32 GetInstalledDepots(DepotId_t pvecDepots, UInt32 cMaxDepots)
    //{
    //    PRINT_DEBUG("GetInstalledDepots old");
    //    return 0;
    //}

    // returns current app install folder for AppID, returns folder name length
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static UInt32 SteamAPI_ISteamApps_GetAppInstallDir(AppId_t appID, IntPtr pchFolder, uint cchFolderBufferSize)
    {
        Write($"SteamAPI_ISteamApps_GetAppInstallDir {appID} {pchFolder} {cchFolderBufferSize}");
        //TODO return real path instead of dll path
        string installed_path = "xd";
        return (UInt32)installed_path.Length; //Real steam always returns the actual path length, not the copied one.
    }

    // returns true if that app is installed (not necessarily owned)
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamApps_BIsAppInstalled(AppId_t appID)
    {
        Write($"SteamAPI_ISteamApps_BIsAppInstalled {appID}");
        return true;
    }

    // returns the SteamID of the original owner. If different from current user, it's borrowed
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static CSteamID SteamAPI_ISteamApps_GetAppOwner()
    {
        Write("SteamAPI_ISteamApps_GetAppOwner");
        return new CSteamID(SteamClient.SteamId);
    }

    // Returns the associated launch param if the game is run via steam://run/<appid>//?param1=value1;param2=value2;param3=value3 etc.
    // Parameter names starting with the character '@' are reserved for internal use and will always return and empty string.
    // Parameter names starting with an underscore '_' are reserved for steam features -- they can be queried by the game,
    // but it is advised that you not param names beginning with an underscore for your own features.
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamApps_GetLaunchQueryParam(string pchKey)
    {
        Write("SteamAPI_ISteamApps_GetLaunchQueryParam");
        return "";
    }


    // get download progress for optional DLC
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamApps_GetDlcDownloadProgress(AppId_t nAppID, UInt64 punBytesDownloaded, UInt64 punBytesTotal)
    {
        Write("SteamAPI_ISteamApps_GetDlcDownloadProgress");
        return false;
    }


    // return the buildid of this app, may change at any time based on backend updates to the game
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamApps_GetAppBuildId()
    {
        Write("SteamAPI_ISteamApps_GetAppBuildId");
        return 10;
    }


    // Request all proof of purchase keys for the calling appid and asociated DLC.
    // A series of AppProofOfPurchaseKeyResponse_t callbacks will be sent with
    // appropriate appid values, ending with a final callback where the m_nAppId
    // member is k_uAppIdInvalid (zero).
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeys()
    {
        Write("SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeys");
    }


    //STEAM_CALL_RESULT(FileDetailsResult_t )
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamApps_GetFileDetails(string pszFileName)
    {
        Write("SteamAPI_ISteamApps_GetFileDetails");
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
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamApps_GetLaunchCommandLine(IntPtr pszCommandLine, int cubCommandLine)
    {
        Write("SteamAPI_ISteamApps_GetLaunchCommandLine");
        return 0;
    }

    // Check if user borrowed this game via Family Sharing, If true, call GetAppOwner() to get the lender SteamID
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamApps_BIsSubscribedFromFamilySharing()
    {
        Write("SteamAPI_ISteamApps_BIsSubscribedFromFamilySharing");
        return false;
    }

    // check if game is a timed trial with limited playtime
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamApps_BIsTimedTrial(UInt32 punSecondsAllowed, UInt32 punSecondsPlayed)
    {
        Write("SteamAPI_ISteamApps_BIsTimedTrial");
        return false;
    }

}

