using System;
using System.Runtime.InteropServices;
using System.Text;

using DepotId_t = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamApps : ISteamInterface
    {
        public SteamApps()
        {
            InterfaceName = "SteamApps";
            InterfaceVersion = "STEAMAPPS_INTERFACE_VERSION008";
        }

        public bool BIsSubscribed()
        {
            Write("BIsSubscribed");
            return true;
        }

        public bool BIsLowViolence()
        {
            Write("BIsLowViolence");
            return false;
        }

        public bool BIsCybercafe()
        {
            Write("BIsCybercafe");
            return false;
        }

        public bool BIsVACBanned()
        {
            Write("BIsVACBanned");
            return false;
        }

        public string GetCurrentGameLanguage()
        {
            Write("GetCurrentGameLanguage");
            return SteamEmulator.Language;
        }

        public string GetAvailableGameLanguages()
        {
            Write("GetAvailableGameLanguages");
            //TODO?
            return "";
        }

        public bool BIsSubscribedApp(uint appID)
        {
            Write("BIsSubscribedApp " + appID);
            return false;
        }

        public bool BIsDlcInstalled(uint appID)
        {
            Write("BIsDlcInstalled " + appID);
            return true;
        }

        public UInt32 GetEarliestPurchaseUnixTime(uint nAppID)
        {
            Write("GetEarliestPurchaseUnixTime");
            //TODO ?
            return 1;
        }

        public bool BIsSubscribedFromFreeWeekend()
        {
            Write("BIsSubscribedFromFreeWeekend");
            return false;
        }

        public int GetDLCCount()
        {
            Write("GetDLCCount");
            return 0;
        }

        public bool BGetDLCDataByIndex(int iDLC, uint pAppID, bool pbAvailable, string pchName, int cchNameBufferSize)
        {
            Write("BGetDLCDataByIndex");
            return true;
        }

        public void InstallDLC(uint nAppID)
        {
            Write("InstallDLC");
        }

        public void UninstallDLC(uint nAppID)
        {
            Write("UninstallDLC");
        }

        public void RequestAppProofOfPurchaseKey(uint nAppID)
        {
            Write("RequestAppProofOfPurchaseKey");
        }

        public bool GetCurrentBetaName(IntPtr pchName, int cchNameBufferSize)
        {
            try
            {
                Write("GetCurrentBetaName");
                byte[] PublicBeta = Encoding.Default.GetBytes("Public");
                if (PublicBeta.Length > cchNameBufferSize) return false;
                Marshal.Copy(PublicBeta, 0, pchName, PublicBeta.Length);
                return true;
            }
            catch (Exception ex)
            {
                Write("GetCurrentBetaName " + ex);
                return false;
            }
        }

        public bool MarkContentCorrupt(bool bMissingFilesOnly)
        {
            Write("MarkContentCorrupt");
            return true;
        }

        public UInt32 GetInstalledDepots(uint appID, ref DepotId_t[] pvecDepots, uint cMaxDepots)
        {
            Write($"GetInstalledDepots {appID}, {cMaxDepots}");
            return 0;
        }

        public UInt32 GetAppInstallDir(uint appID, string pchFolder, uint cchFolderBufferSize)
        {
            Write($"GetAppInstallDir {appID} {pchFolder} {cchFolderBufferSize}");
            string installed_path = modCommon.GetPath();
            return (uint)installed_path.Length;
        }

        public bool BIsAppInstalled(uint appID)
        {
            Write($"BIsAppInstalled {appID}");
            return true;
        }

        public CSteamID GetAppOwner()
        {
            Write("GetAppOwner");
            return (CSteamID)SteamEmulator.SteamId;
        }

        public IntPtr GetLaunchQueryParam(string pchKey)
        {
            Write("GetLaunchQueryParam");
            return default;
        }

        public bool GetDlcDownloadProgress(uint nAppID, UInt64 punBytesDownloaded, UInt64 punBytesTotal)
        {
            Write("GetDlcDownloadProgress");
            return false;
        }


        public int GetAppBuildId()
        {
            Write("GetAppBuildId");
            return 10;
        }

        public void RequestAllProofOfPurchaseKeys()
        {
            Write("RequestAllProofOfPurchaseKeys");
        }

        public ulong GetFileDetails(string pszFileName)
        {
            Write("GetFileDetails");
            return 0;
        }

        public bool BIsSubscribedFromFamilySharing()
        {
            Write("BIsSubscribedFromFamilySharing");
            return false;
        }

        public bool BIsTimedTrial(UInt32 punSecondsAllowed, UInt32 punSecondsPlayed)
        {
            Write("BIsTimedTrial");
            return false;
        }

        public int GetLaunchCommandLine(string pszCommandLine, int cubCommandLine)
        {
            Write("GetLaunchCommandLine");
            return 0;
        }
    }
}
