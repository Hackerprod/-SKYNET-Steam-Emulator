using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SKYNET;
using Steamworks;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_SteamApps
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_BIsSubscribed(IntPtr _)
        {
            Write("SteamAPI_ISteamApps_BIsSubscribed");
            return SteamEmulator.SteamApps.BIsSubscribed();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_BIsLowViolence(IntPtr _)
        {
            Write("SteamAPI_ISteamApps_BIsLowViolence");
            return SteamEmulator.SteamApps.BIsLowViolence();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_BIsCybercafe(IntPtr _)
        {
            Write("SteamAPI_ISteamApps_BIsCybercafe");
            return SteamEmulator.SteamApps.BIsCybercafe();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_BIsVACBanned(IntPtr _)
        {
            Write("SteamAPI_ISteamApps_BIsVACBanned");
            return SteamEmulator.SteamApps.BIsVACBanned();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamApps_GetCurrentGameLanguage(IntPtr _)
        {
            Write("SteamAPI_ISteamApps_GetCurrentGameLanguage");
            return SteamEmulator.SteamApps.GetCurrentGameLanguage();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamApps_GetAvailableGameLanguages(IntPtr _)
        {
            Write("SteamAPI_ISteamApps_GetAvailableGameLanguages");
            return SteamEmulator.SteamApps.GetAvailableGameLanguages();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_BIsSubscribedApp(IntPtr _, uint appID)
        {
            Write("SteamAPI_ISteamApps_BIsSubscribedApp");
            return SteamEmulator.SteamApps.BIsSubscribedApp(appID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_BIsDlcInstalled(IntPtr _, uint appID)
        {
            Write("SteamAPI_ISteamApps_BIsDlcInstalled " + appID);
            return SteamEmulator.SteamApps.BIsDlcInstalled(appID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static UInt32 SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime(IntPtr _, uint nAppID)
        {
            Write("SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime");
            return SteamEmulator.SteamApps.GetEarliestPurchaseUnixTime(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend(IntPtr _)
        {
            Write("SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend");
            return SteamEmulator.SteamApps.BIsSubscribedFromFreeWeekend();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamApps_GetDLCCount(IntPtr _)
        {
            Write("SteamAPI_ISteamApps_GetDLCCount");
            return SteamEmulator.SteamApps.GetDLCCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_BGetDLCDataByIndex(IntPtr _, int iDLC, uint pAppID, bool pbAvailable, string pchName, int cchNameBufferSize)
        {
            Write("SteamAPI_ISteamApps_BGetDLCDataByIndex");
            return SteamEmulator.SteamApps.BGetDLCDataByIndex(iDLC, pAppID, pbAvailable, pchName, cchNameBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamApps_InstallDLC(IntPtr _, uint nAppID)
        {
            Write("SteamAPI_ISteamApps_InstallDLC");
            SteamEmulator.SteamApps.InstallDLC(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamApps_UninstallDLC(IntPtr _, uint nAppID)
        {
            Write("SteamAPI_ISteamApps_UninstallDLC");
            SteamEmulator.SteamApps.UninstallDLC(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey(IntPtr _, uint nAppID)
        {
            Write("SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey");
            SteamEmulator.SteamApps.RequestAppProofOfPurchaseKey(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_GetCurrentBetaName(IntPtr _, IntPtr pchName, int cchNameBufferSize)
        {
            Write("SteamAPI_ISteamApps_GetCurrentBetaName ");
            return SteamEmulator.SteamApps.GetCurrentBetaName(pchName, cchNameBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_MarkContentCorrupt(IntPtr _, bool bMissingFilesOnly)
        {
            Write("SteamAPI_ISteamApps_MarkContentCorrupt");
            return SteamEmulator.SteamApps.MarkContentCorrupt(bMissingFilesOnly);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static UInt32 SteamAPI_ISteamApps_GetInstalledDepots(IntPtr _, uint appID, uint pvecDepots, uint cMaxDepots)
        {
            Write($"SteamAPI_ISteamApps_GetInstalledDepots");
            return SteamEmulator.SteamApps.GetInstalledDepots(appID, pvecDepots, cMaxDepots);
        }

        //[DllExport(CallingConvention = CallingConvention.Cdecl)]
        //public static UInt32 GetInstalledDepots(DepotId_t pvecDepots, UInt32 cMaxDepots)
        //{
        //    PRINT_DEBUG("GetInstalledDepots old");
        //    return 0;
        //}

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static UInt32 SteamAPI_ISteamApps_GetAppInstallDir(IntPtr _, uint appID, string pchFolder, uint cchFolderBufferSize)
        {
            Write($"SteamAPI_ISteamApps_GetAppInstallDir");
            return SteamEmulator.SteamApps.GetAppInstallDir(appID, pchFolder, cchFolderBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_BIsAppInstalled(IntPtr _, uint appID)
        {
            Write($"SteamAPI_ISteamApps_BIsAppInstalled");
            return SteamEmulator.SteamApps.BIsAppInstalled(appID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID SteamAPI_ISteamApps_GetAppOwner(IntPtr _)
        {
            Write("SteamAPI_ISteamApps_GetAppOwner");
            return SteamEmulator.SteamApps.GetAppOwner();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamApps_GetLaunchQueryParam(IntPtr _, string pchKey)
        {
            Write("SteamAPI_ISteamApps_GetLaunchQueryParam");
            return SteamEmulator.SteamApps.GetLaunchQueryParam(pchKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_GetDlcDownloadProgress(IntPtr _, uint nAppID, UInt64 punBytesDownloaded, UInt64 punBytesTotal)
        {
            Write("SteamAPI_ISteamApps_GetDlcDownloadProgress");
            return SteamEmulator.SteamApps.GetDlcDownloadProgress(nAppID, punBytesDownloaded, punBytesTotal);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamApps_GetAppBuildId(IntPtr _)
        {
            Write("SteamAPI_ISteamApps_GetAppBuildId");
            return SteamEmulator.SteamApps.GetAppBuildId();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeys(IntPtr _)
        {
            Write("SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeys");
            SteamEmulator.SteamApps.RequestAllProofOfPurchaseKeys();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamApps_GetFileDetails(IntPtr _, string pszFileName)
        {
            Write("SteamAPI_ISteamApps_GetFileDetails");
            return SteamEmulator.SteamApps.GetFileDetails(pszFileName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamApps_GetLaunchCommandLine(IntPtr _, string pszCommandLine, int cubCommandLine)
        {
            Write("SteamAPI_ISteamApps_GetLaunchCommandLine");
            return SteamEmulator.SteamApps.GetLaunchCommandLine(pszCommandLine, cubCommandLine);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_BIsSubscribedFromFamilySharing(IntPtr _)
        {
            Write("SteamAPI_ISteamApps_BIsSubscribedFromFamilySharing");
            return SteamEmulator.SteamApps.BIsSubscribedFromFamilySharing();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_BIsTimedTrial(IntPtr _, UInt32 punSecondsAllowed, UInt32 punSecondsPlayed)
        {
            Write("SteamAPI_ISteamApps_BIsTimedTrial");
            return SteamEmulator.SteamApps.BIsTimedTrial(punSecondsAllowed, punSecondsPlayed);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

