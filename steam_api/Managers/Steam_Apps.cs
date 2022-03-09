using System;
using System.Runtime.InteropServices;
using SKYNET.Helper;
using SKYNET.Interface;
using Steamworks;

namespace SKYNET.Managers
{
    public class Steam_Apps : SteamInterface//, ISteamApps
    {

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool BIsSubscribed()
        {
            Write("BIsSubscribed");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool BIsLowViolence()
        {
            Write("BIsLowViolence");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool BIsCybercafe()
        {
            Write("BIsCybercafe");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool BIsVACBanned()
        {
            Write("BIsVACBanned");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string GetCurrentGameLanguage()
        {
            Write("GetCurrentGameLanguage");
            return SteamClient.Language; ;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string GetAvailableGameLanguages()
        {
            Write("GetAvailableGameLanguages");
            //TODO?
            return "";
        }

        // only use this member if you need to check ownership of another game related to yours, a demo for example
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool BIsSubscribedApp(AppId_t appID)
        {
            Write("BIsSubscribedApp " + appID);
            return false;
        }


        // Takes AppID of DLC and checks if the user owns the DLC & if the DLC is installed
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool BIsDlcInstalled(AppId_t appID)
        {
            Write("BIsDlcInstalled " + appID);
            return false;

        }

        // returns the Unix time of the purchase of the app
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static UInt32 GetEarliestPurchaseUnixTime(AppId_t nAppID)
        {
            Write("GetEarliestPurchaseUnixTime");
            //TODO ?
            return 1;
        }


        // Checks if the user is subscribed to the current app through a free weekend
        // This function will return false for users who have a retail or other type of license
        // Before using, please ask your Valve technical contact how to package and secure your free weekened
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool BIsSubscribedFromFreeWeekend()
        {
            Write("BIsSubscribedFromFreeWeekend");
            return false;
        }


        // Returns the number of DLC pieces for the running app
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetDLCCount()
        {
            Write("GetDLCCount");
            return 0;
        }


        // Returns metadata for DLC by index, of range [0, GetDLCCount()]
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool BGetDLCDataByIndex(int iDLC, IntPtr pAppID, bool pbAvailable, IntPtr pchName, int cchNameBufferSize)
        {
            Write("BGetDLCDataByIndex");
            return true;
        }


        // Install/Uninstall control for optional DLC
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void InstallDLC(AppId_t nAppID)
        {
            Write("InstallDLC");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void UninstallDLC(AppId_t nAppID)
        {
            Write("UninstallDLC");
        }


        // Request legacy cd-key for yourself or owned DLC. If you are interested in this
        // data then make sure you provide us with a list of valid keys to be distributed
        // to users when they purchase the game, before the game ships.
        // You'll receive an AppProofOfPurchaseKeyResponse_t callback when
        // the key is available (which may be immediately).
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void RequestAppProofOfPurchaseKey(AppId_t nAppID)
        {
            Write("RequestAppProofOfPurchaseKey");
        }

        // returns current beta branch name, 'public' is the default branch
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool GetCurrentBetaName(IntPtr pchName, int cchNameBufferSize)
        {
            Write("GetCurrentBetaName " + cchNameBufferSize);
            return true;
        }

        // signal Steam that game files seems corrupt or missing
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool MarkContentCorrupt(bool bMissingFilesOnly)
        {
            Write("MarkContentCorrupt");
            //TODO: warn user
            return true;
        }

        // return installed depots in mount order
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static UInt32 GetInstalledDepots(AppId_t appID, IntPtr pvecDepots, uint cMaxDepots)
        {
            Write($"GetInstalledDepots {appID}, {cMaxDepots}");
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
        public static UInt32 GetAppInstallDir(AppId_t appID, IntPtr pchFolder, uint cchFolderBufferSize)
        {
            Write($"GetAppInstallDir {appID} {pchFolder} {cchFolderBufferSize}");
            //TODO return real path instead of dll path
            string installed_path = "xd";
            return (UInt32)installed_path.Length; //Real steam always returns the actual path length, not the copied one.
        }

        // returns true if that app is installed (not necessarily owned)
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool BIsAppInstalled(AppId_t appID)
        {
            Write($"BIsAppInstalled {appID}");
            return true;
        }

        // returns the SteamID of the original owner. If different from current user, it's borrowed
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID GetAppOwner()
        {
            Write("GetAppOwner");
            return new CSteamID(SteamClient.SteamId);
        }

        // Returns the associated launch param if the game is run via steam://run/<appid>//?param1=value1;param2=value2;param3=value3 etc.
        // Parameter names starting with the character '@' are reserved for internal use and will always return and empty string.
        // Parameter names starting with an underscore '_' are reserved for steam features -- they can be queried by the game,
        // but it is advised that you not param names beginning with an underscore for your own features.
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string GetLaunchQueryParam(string pchKey)
        {
            Write("GetLaunchQueryParam");
            return "";
        }


        // get download progress for optional DLC
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool GetDlcDownloadProgress(AppId_t nAppID, UInt64 punBytesDownloaded, UInt64 punBytesTotal)
        {
            Write("GetDlcDownloadProgress");
            return false;
        }


        // return the buildid of this app, may change at any time based on backend updates to the game
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetAppBuildId()
        {
            Write("GetAppBuildId");
            return 10;
        }


        // Request all proof of purchase keys for the calling appid and asociated DLC.
        // A series of AppProofOfPurchaseKeyResponse_t callbacks will be sent with
        // appropriate appid values, ending with a final callback where the m_nAppId
        // member is k_uAppIdInvalid (zero).
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void RequestAllProofOfPurchaseKeys()
        {
            Write("RequestAllProofOfPurchaseKeys");
        }


        //STEAM_CALL_RESULT(FileDetailsResult_t )
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t GetFileDetails(string pszFileName)
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
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetLaunchCommandLine(IntPtr pszCommandLine, int cubCommandLine)
        {
            Write("GetLaunchCommandLine");
            return 0;
        }

        // Check if user borrowed this game via Family Sharing, If true, call GetAppOwner() to get the lender SteamID
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool BIsSubscribedFromFamilySharing()
        {
            Write("BIsSubscribedFromFamilySharing");
            return false;
        }

        // check if game is a timed trial with limited playtime
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool BIsTimedTrial(UInt32 punSecondsAllowed, UInt32 punSecondsPlayed)
        {
            Write("BIsTimedTrial");
            return false;
        }
    }
}