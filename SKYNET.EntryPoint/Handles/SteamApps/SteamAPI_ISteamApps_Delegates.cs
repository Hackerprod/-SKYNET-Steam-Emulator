using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SKYNET;
using Steamworks;

namespace SKYNET.Hook.Handles
{
    public partial class SteamAPI_ISteamApps
    {
        // Functions injection



        // Functions Delegates

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsSubscribedDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsLowViolenceDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsCybercafeDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsVACBannedDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate string SteamAPI_ISteamApps_GetCurrentGameLanguageDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate string SteamAPI_ISteamApps_GetAvailableGameLanguagesDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsSubscribedAppDelegate(AppId_t appID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsDlcInstalledDelegate(AppId_t appID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate UInt32 SteamAPI_ISteamApps_GetEarliestPurchaseUnixTimeDelegate(AppId_t nAppID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekendDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int SteamAPI_ISteamApps_GetDLCCountDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BGetDLCDataByIndexDelegate(int iDLC, IntPtr pAppID, bool pbAvailable, IntPtr pchName, int cchNameBufferSize);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_ISteamApps_InstallDLCDelegate(AppId_t nAppID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_ISteamApps_UninstallDLCDelegate(AppId_t nAppID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_ISteamApps_RequestAppProofOfPurchaseKeyDelegate(AppId_t nAppID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_GetCurrentBetaNameDelegate(IntPtr pchName, int cchNameBufferSize);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_MarkContentCorruptDelegate(bool bMissingFilesOnly);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate UInt32 SteamAPI_ISteamApps_GetInstalledDepotsDelegate(AppId_t appID, IntPtr pvecDepots, uint cMaxDepots);

        //[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        //public delegate UInt32  GetInstalledDepotsDelegate (DepotId_t pvecDepots, UInt32 cMaxDepots);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate UInt32 SteamAPI_ISteamApps_GetAppInstallDirDelegate(AppId_t appID, IntPtr pchFolder, uint cchFolderBufferSize);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsAppInstalledDelegate(AppId_t appID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate ulong SteamAPI_ISteamApps_GetAppOwnerDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate string SteamAPI_ISteamApps_GetLaunchQueryParamDelegate(string pchKey);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_GetDlcDownloadProgressDelegate(AppId_t nAppID, UInt64 punBytesDownloaded, UInt64 punBytesTotal);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int SteamAPI_ISteamApps_GetAppBuildIdDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeysDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate SteamAPICall_t SteamAPI_ISteamApps_GetFileDetailsDelegate(string pszFileName);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int SteamAPI_ISteamApps_GetLaunchCommandLineDelegate(string pszCommandLine, int cubCommandLine);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsSubscribedFromFamilySharingDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsTimedTrialDelegate(UInt32 punSecondsAllowed, UInt32 punSecondsPlayed);


        // Functions Delegates

        public static SteamAPI_ISteamApps_BIsSubscribedDelegate _SteamAPI_ISteamApps_BIsSubscribed;

        public static SteamAPI_ISteamApps_BIsLowViolenceDelegate _SteamAPI_ISteamApps_BIsLowViolence;

        public static SteamAPI_ISteamApps_BIsCybercafeDelegate _SteamAPI_ISteamApps_BIsCybercafe;

        public static SteamAPI_ISteamApps_BIsVACBannedDelegate _SteamAPI_ISteamApps_BIsVACBanned;

        public static SteamAPI_ISteamApps_GetCurrentGameLanguageDelegate _SteamAPI_ISteamApps_GetCurrentGameLanguage;

        public static SteamAPI_ISteamApps_GetAvailableGameLanguagesDelegate _SteamAPI_ISteamApps_GetAvailableGameLanguages;

        public static SteamAPI_ISteamApps_BIsSubscribedAppDelegate _SteamAPI_ISteamApps_BIsSubscribedApp;

        public static SteamAPI_ISteamApps_BIsDlcInstalledDelegate _SteamAPI_ISteamApps_BIsDlcInstalled;

        public static SteamAPI_ISteamApps_GetEarliestPurchaseUnixTimeDelegate _SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime;

        public static SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekendDelegate _SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend;

        public static SteamAPI_ISteamApps_GetDLCCountDelegate _SteamAPI_ISteamApps_GetDLCCount;

        public static SteamAPI_ISteamApps_BGetDLCDataByIndexDelegate _SteamAPI_ISteamApps_BGetDLCDataByIndex;

        public static SteamAPI_ISteamApps_InstallDLCDelegate _SteamAPI_ISteamApps_InstallDLC;

        public static SteamAPI_ISteamApps_UninstallDLCDelegate _SteamAPI_ISteamApps_UninstallDLC;

        public static SteamAPI_ISteamApps_RequestAppProofOfPurchaseKeyDelegate _SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey;

        public static SteamAPI_ISteamApps_GetCurrentBetaNameDelegate _SteamAPI_ISteamApps_GetCurrentBetaName;

        public static SteamAPI_ISteamApps_MarkContentCorruptDelegate _SteamAPI_ISteamApps_MarkContentCorrupt;

        public static SteamAPI_ISteamApps_GetInstalledDepotsDelegate _SteamAPI_ISteamApps_GetInstalledDepots;

        //public static GetInstalledDepotsDelegate _GetInstalledDepots;

        public static SteamAPI_ISteamApps_GetAppInstallDirDelegate _SteamAPI_ISteamApps_GetAppInstallDir;

        public static SteamAPI_ISteamApps_BIsAppInstalledDelegate _SteamAPI_ISteamApps_BIsAppInstalled;

        public static SteamAPI_ISteamApps_GetAppOwnerDelegate _SteamAPI_ISteamApps_GetAppOwner;

        public static SteamAPI_ISteamApps_GetLaunchQueryParamDelegate _SteamAPI_ISteamApps_GetLaunchQueryParam;

        public static SteamAPI_ISteamApps_GetDlcDownloadProgressDelegate _SteamAPI_ISteamApps_GetDlcDownloadProgress;

        public static SteamAPI_ISteamApps_GetAppBuildIdDelegate _SteamAPI_ISteamApps_GetAppBuildId;

        public static SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeysDelegate _SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeys;

        public static SteamAPI_ISteamApps_GetFileDetailsDelegate _SteamAPI_ISteamApps_GetFileDetails;

        public static SteamAPI_ISteamApps_GetLaunchCommandLineDelegate _SteamAPI_ISteamApps_GetLaunchCommandLine;

        public static SteamAPI_ISteamApps_BIsSubscribedFromFamilySharingDelegate _SteamAPI_ISteamApps_BIsSubscribedFromFamilySharing;

        public static SteamAPI_ISteamApps_BIsTimedTrialDelegate _SteamAPI_ISteamApps_BIsTimedTrial;


    }
}