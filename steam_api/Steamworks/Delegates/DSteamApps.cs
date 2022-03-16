using System;
using System.Runtime.InteropServices;
using Steamworks;

namespace SKYNET.Delegate
{
    [Delegate("SteamAppList")]
    public class DSteamApps
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsSubscribed();
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsLowViolence();
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsCybercafe();
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsVACBanned();
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetCurrentGameLanguage();
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetAvailableGameLanguages();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsSubscribedApp(AppId_t appID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsDlcInstalled(AppId_t appID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate UInt32 GetEarliestPurchaseUnixTime(AppId_t nAppID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsSubscribedFromFreeWeekend();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetDLCCount();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BGetDLCDataByIndex(int iDLC, IntPtr pAppID, bool pbAvailable, IntPtr pchName, int cchNameBufferSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void InstallDLC(AppId_t nAppID);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void UninstallDLC(AppId_t nAppID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void RequestAppProofOfPurchaseKey(AppId_t nAppID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetCurrentBetaName(IntPtr pchName, int cchNameBufferSize); // returns current beta branch name, 'public' is the default branch
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool MarkContentCorrupt(bool bMissingFilesOnly); // signal Steam that game files seems corrupt or missing
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate UInt32 GetInstalledDepots(AppId_t appID, IntPtr pvecDepots, UInt32 cMaxDepots); // return installed depots in mount order

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate UInt32 GetAppInstallDir(AppId_t appID, IntPtr pchFolder, UInt32 cchFolderBufferSize);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsAppInstalled(AppId_t appID); // returns true if that app is installed (not necessarily owned)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetAppOwner();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetLaunchQueryParam(string pchKey);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetDlcDownloadProgress(AppId_t nAppID, UInt64 punBytesDownloaded, UInt64 punBytesTotal);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetAppBuildId();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void RequestAllProofOfPurchaseKeys();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t GetFileDetails(string pszFileName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetLaunchCommandLine(IntPtr pszCommandLine, int cubCommandLine);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsSubscribedFromFamilySharing();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BIsTimedTrial(UInt32 punSecondsAllowed, UInt32 punSecondsPlayed);
    }
}