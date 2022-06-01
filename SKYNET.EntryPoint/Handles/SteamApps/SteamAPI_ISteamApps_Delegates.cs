using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SKYNET;
using SKYNET.Steamworks;

using HSteamPipe = System.UInt32;
using HSteamUser = System.UInt32;
using SteamAPICall_t = System.UInt64;
using AppId_t = System.UInt32;
using DepotId_t = System.UInt32;

namespace SKYNET.Hook.Handles
{
    public partial class SteamAPI_ISteamApps
    {
        // Functions injection



        // Functions Delegates

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsSubscribedDelegate(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsLowViolenceDelegate(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsCybercafeDelegate(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsVACBannedDelegate(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate string SteamAPI_ISteamApps_GetCurrentGameLanguageDelegate(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate string SteamAPI_ISteamApps_GetAvailableGameLanguagesDelegate(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsSubscribedAppDelegate(IntPtr _, AppId_t appID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsDlcInstalledDelegate(IntPtr _, AppId_t appID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate UInt32 SteamAPI_ISteamApps_GetEarliestPurchaseUnixTimeDelegate(IntPtr _, AppId_t nAppID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekendDelegate(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int SteamAPI_ISteamApps_GetDLCCountDelegate(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BGetDLCDataByIndexDelegate(IntPtr _, int iDLC, uint pAppID, bool pbAvailable, string pchName, int cchNameBufferSize);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_ISteamApps_InstallDLCDelegate(IntPtr _, AppId_t nAppID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_ISteamApps_UninstallDLCDelegate(IntPtr _, AppId_t nAppID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_ISteamApps_RequestAppProofOfPurchaseKeyDelegate(IntPtr _, AppId_t nAppID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_GetCurrentBetaNameDelegate(IntPtr _, IntPtr pchName, int cchNameBufferSize);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_MarkContentCorruptDelegate(IntPtr _, bool bMissingFilesOnly);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate UInt32 SteamAPI_ISteamApps_GetInstalledDepotsDelegate(IntPtr _, uint appID, ref DepotId_t[] pvecDepots, uint cMaxDepots);

        //[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        //public delegate UInt32  GetInstalledDepotsDelegate (DepotId_t pvecDepots, UInt32 cMaxDepots);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate UInt32 SteamAPI_ISteamApps_GetAppInstallDirDelegate(IntPtr _, uint appID, string pchFolder, uint cchFolderBufferSize);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsAppInstalledDelegate(IntPtr _, AppId_t appID);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate ulong SteamAPI_ISteamApps_GetAppOwnerDelegate(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate IntPtr SteamAPI_ISteamApps_GetLaunchQueryParamDelegate(IntPtr _, string pchKey);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_GetDlcDownloadProgressDelegate(IntPtr _, AppId_t nAppID, UInt64 punBytesDownloaded, UInt64 punBytesTotal);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int SteamAPI_ISteamApps_GetAppBuildIdDelegate(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate void SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeysDelegate(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate SteamAPICall_t SteamAPI_ISteamApps_GetFileDetailsDelegate(IntPtr _, string pszFileName);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate int SteamAPI_ISteamApps_GetLaunchCommandLineDelegate(IntPtr _, string pszCommandLine, int cubCommandLine);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsSubscribedFromFamilySharingDelegate(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode, SetLastError = true)]
        public delegate bool SteamAPI_ISteamApps_BIsTimedTrialDelegate(IntPtr _, UInt32 punSecondsAllowed, UInt32 punSecondsPlayed);


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