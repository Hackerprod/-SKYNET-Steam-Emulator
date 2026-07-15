using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamApps
    {
        static SteamAPI_ISteamApps()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

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
        public static IntPtr SteamAPI_ISteamApps_GetCurrentGameLanguage(IntPtr _)
        {
            Write("SteamAPI_ISteamApps_GetCurrentGameLanguage");
            return SteamEmulator.SteamApps.GetCurrentGameLanguage();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamApps_GetAvailableGameLanguages(IntPtr _)
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
        public static bool SteamAPI_ISteamApps_BGetDLCDataByIndex(IntPtr _, int iDLC, IntPtr pAppID, IntPtr pbAvailable, IntPtr pchName, int cchNameBufferSize)
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
        public static UInt32 SteamAPI_ISteamApps_GetInstalledDepots(IntPtr _, uint appID, IntPtr pvecDepots, uint cMaxDepots)
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
        public static UInt32 SteamAPI_ISteamApps_GetAppInstallDir(IntPtr _, uint appID, IntPtr pchFolder, uint cchFolderBufferSize)
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
        public static ulong SteamAPI_ISteamApps_GetAppOwner(IntPtr _)
        {
            Write("SteamAPI_ISteamApps_GetAppOwner");
            return SteamEmulator.SteamApps.GetAppOwner().SteamID;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamApps_GetLaunchQueryParam(IntPtr _, string pchKey)
        {
            Write("SteamAPI_ISteamApps_GetLaunchQueryParam");
            return SteamEmulator.SteamApps.GetLaunchQueryParam(pchKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_GetDlcDownloadProgress(IntPtr _, uint nAppID, IntPtr punBytesDownloaded, IntPtr punBytesTotal)
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
        public static int SteamAPI_ISteamApps_GetLaunchCommandLine(IntPtr _, IntPtr pszCommandLine, int cubCommandLine)
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
        public static bool SteamAPI_ISteamApps_BIsTimedTrial(IntPtr _, IntPtr punSecondsAllowed, IntPtr punSecondsPlayed)
        {
            Write("SteamAPI_ISteamApps_BIsTimedTrial");
            return SteamEmulator.SteamApps.BIsTimedTrial(punSecondsAllowed, punSecondsPlayed);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_SetDlcContext(IntPtr _, uint nAppID)
        {
            Write("SteamAPI_ISteamApps_SetDlcContext");
            return SteamEmulator.SteamApps.SetDlcContext(nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamApps_GetNumBetas(IntPtr _, IntPtr pnAvailable, IntPtr pnPrivate)
        {
            Write("SteamAPI_ISteamApps_GetNumBetas");
            return SteamEmulator.SteamApps.GetNumBetas(pnAvailable, pnPrivate);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_GetBetaInfo(
            IntPtr _,
            int iBetaIndex,
            IntPtr punFlags,
            IntPtr punBuildID,
            IntPtr pchBetaName,
            int cchBetaName,
            IntPtr pchDescription,
            int cchDescription)
        {
            Write("SteamAPI_ISteamApps_GetBetaInfo");
            return SteamEmulator.SteamApps.GetBetaInfo(
                iBetaIndex,
                punFlags,
                punBuildID,
                pchBetaName,
                cchBetaName,
                pchDescription,
                cchDescription);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamApps_SetActiveBeta(IntPtr _, string pchBetaName)
        {
            Write("SteamAPI_ISteamApps_SetActiveBeta");
            return SteamEmulator.SteamApps.SetActiveBeta(pchBetaName);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
