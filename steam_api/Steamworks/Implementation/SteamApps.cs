using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using SKYNET.Callback;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;
using SKYNET.Types;
using DepotId_t = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamApps : ISteamInterface
    {
        public static SteamApps Instance;

        public SteamApps()
        {
            Instance = this;
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
            string Language = SteamEmulator.Language;
            Write($"GetCurrentGameLanguage = {Language}");
            return Language;
        }

        public string GetAvailableGameLanguages()
        {
            Write("GetAvailableGameLanguages");
            //TODO?
            return "";
        }

        public bool BIsSubscribedApp(uint appID)
        {
            bool result;
            if (appID == 0)
            {
                result = true;                      // appid 0 is always owned
            }
            else if (appID == SteamEmulator.AppID)
            {
                result = true;                      // the base game itself
            }
            else
            {
                result = DLCManager.HasDLC(appID);  // owned DLC (or unlock-all)
            }

            Write($"BIsSubscribedApp (AppID = {appID}) = {result}");
            return result;
        }

        public bool BIsDlcInstalled(uint appID)
        {
            bool result = appID == 0 || DLCManager.HasDLC(appID);
            Write($"BIsDlcInstalled (AppID = {appID}) = {result}");
            return result;
        }

        public UInt32 GetEarliestPurchaseUnixTime(uint nAppID)
        {
            Write($"GetEarliestPurchaseUnixTime (AppID = {nAppID})");
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
            int count = DLCManager.Count;
            Write($"GetDLCCount {count}");
            return count;
        }

        public bool BGetDLCDataByIndex(int iDLC, IntPtr pAppID, IntPtr pbAvailable, IntPtr pchName, int cchNameBufferSize)
        {
            if (!DLCManager.TryGet(iDLC, out uint appId, out bool available, out string name))
            {
                Write($"BGetDLCDataByIndex (iDLC = {iDLC}) = false");
                return false;
            }

            if (pAppID != IntPtr.Zero)
            {
                Marshal.WriteInt32(pAppID, (int)appId);
            }

            if (pbAvailable != IntPtr.Zero)
            {
                Marshal.WriteByte(pbAvailable, (byte)(available ? 1 : 0));
            }

            if (pchName != IntPtr.Zero && cchNameBufferSize > 0)
            {
                byte[] nameBytes = Encoding.UTF8.GetBytes(name ?? string.Empty);
                int copyLength = nameBytes.Length;
                if (copyLength > cchNameBufferSize - 1)
                {
                    copyLength = cchNameBufferSize - 1;
                }
                if (copyLength < 0)
                {
                    copyLength = 0;
                }

                if (copyLength > 0)
                {
                    Marshal.Copy(nameBytes, 0, pchName, copyLength);
                }
                Marshal.WriteByte(pchName, copyLength, 0); // null terminator
            }

            Write($"BGetDLCDataByIndex (iDLC = {iDLC}) AppID = {appId} Available = {available} Name = {name}");
            return true;
        }

        public void InstallDLC(uint nAppID)
        {
            Write($"InstallDLC (AppID = {nAppID})");
            // The DLC is already considered installed; notify the game so callers
            // that wait for DlcInstalled_t after InstallDLC can proceed.
            if (nAppID != 0)
            {
                CallbackManager.AddCallback(new DlcInstalled_t { AppID = nAppID });
            }
        }

        public void UninstallDLC(uint nAppID)
        {
            Write($"UninstallDLC (AppID = {nAppID})");
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
            string installed_path = Common.GetPath();
            return (uint)installed_path.Length;
        }

        public bool BIsAppInstalled(uint appID)
        {
            Write($"BIsAppInstalled {appID}");
            return true;
        }

        public CSteamID GetAppOwner()
        {
            Write($"GetAppOwner");
            return SteamEmulator.SteamID;
        }

        public IntPtr GetLaunchQueryParam(string pchKey)
        {
            Write("GetLaunchQueryParam");
            return default;
        }

        public bool GetDlcDownloadProgress(uint nAppID, UInt64 punBytesDownloaded, UInt64 punBytesTotal)
        {
            Write($"GetDlcDownloadProgress (AppID = {nAppID})");
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
