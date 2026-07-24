using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;
using SKYNET.Types;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamApps : ISteamInterface
    {
        public static SteamApps Instance;

        private uint currentDlcContext;
        private string activeBetaName;

        public SteamApps()
        {
            Instance = this;
            InterfaceName = "SteamApps";
            InterfaceVersion = "STEAMAPPS_INTERFACE_VERSION008";
            activeBetaName = string.Empty;
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

        public IntPtr GetCurrentGameLanguage()
        {
            string Language = SteamEmulator.Language;
            Write($"GetCurrentGameLanguage = {Language}");
            return NativeStringCache.ToUtf8Ptr(Language);
        }

        public IntPtr GetAvailableGameLanguages()
        {
            Write("GetAvailableGameLanguages");
            return NativeStringCache.ToUtf8Ptr(SteamEmulator.Language);
        }

        public bool BIsSubscribedApp(uint appID)
        {
            bool result = AppEntitlementManager.HasLicense(appID);
            Write($"BIsSubscribedApp (AppID = {appID}) = {result}");
            return result;
        }

        public bool BIsDlcInstalled(uint appID)
        {
            bool result = DLCManager.HasDLC(appID);
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
            if (DLCManager.TryInstall(nAppID))
            {
                CallbackManager.AddCallback(new DlcInstalled_t { AppID = nAppID });
            }
            else
            {
                Write($"InstallDLC rejected: DLC {nAppID} is not owned");
            }
        }

        public void UninstallDLC(uint nAppID)
        {
            bool removed = DLCManager.TryUninstall(nAppID);
            Write($"UninstallDLC (AppID = {nAppID}) = {removed}");
        }

        public void RequestAppProofOfPurchaseKey(uint nAppID)
        {
            Write("RequestAppProofOfPurchaseKey");
        }

        public bool GetCurrentBetaName(IntPtr pchName, int cchNameBufferSize)
        {
            Write("GetCurrentBetaName");
            WriteUtf8Buffer(pchName, cchNameBufferSize, activeBetaName);
            return !string.IsNullOrEmpty(activeBetaName);
        }

        public bool MarkContentCorrupt(bool bMissingFilesOnly)
        {
            Write("MarkContentCorrupt");
            return true;
        }

        public UInt32 GetInstalledDepots(uint appID, IntPtr pvecDepots, uint cMaxDepots)
        {
            IReadOnlyList<uint> depots = AppContentManager.GetInstalledDepots(appID);
            uint count = (uint)Math.Min(depots.Count, cMaxDepots);

            if (pvecDepots != IntPtr.Zero && count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Marshal.WriteInt32(pvecDepots, i * sizeof(uint), unchecked((int)depots[i]));
                }
            }

            Write($"GetInstalledDepots {appID}, max={cMaxDepots} = {count}");
            return count;
        }

        public UInt32 GetAppInstallDir(uint appID, IntPtr pchFolder, uint cchFolderBufferSize)
        {
            string installed_path;
            if (!AppContentManager.TryGetAppInstallPath(appID, out installed_path))
            {
                installed_path = Common.GetPath();
            }

            if (string.IsNullOrEmpty(installed_path))
            {
                Write($"GetAppInstallDir {appID} = <disabled>");
                return 0;
            }

            uint copied = (uint)WriteUtf8Buffer(pchFolder, cchFolderBufferSize, installed_path);
            Write($"GetAppInstallDir {appID} = {installed_path} ({copied} bytes)");
            return copied;
        }

        public bool BIsAppInstalled(uint appID)
        {
            bool installed = AppContentManager.IsAppInstalled(appID);
            Write($"BIsAppInstalled {appID} = {installed}");
            return installed;
        }

        public CSteamID GetAppOwner()
        {
            Write($"GetAppOwner");
            return SteamEmulator.SteamID;
        }

        public IntPtr GetLaunchQueryParam(string pchKey)
        {
            Write($"GetLaunchQueryParam {pchKey}");
            return NativeStringCache.ToUtf8Ptr(string.Empty);
        }

        public bool GetDlcDownloadProgress(uint nAppID, IntPtr punBytesDownloaded, IntPtr punBytesTotal)
        {
            bool installed = DLCManager.IsInstalled(nAppID);
            ulong bytesTotal = installed ? GetInstalledContentSize(nAppID) : 0;
            WriteUInt64(punBytesDownloaded, bytesTotal);
            WriteUInt64(punBytesTotal, bytesTotal);
            Write($"GetDlcDownloadProgress (AppID = {nAppID}) installed={installed} bytes={bytesTotal}");
            return installed;
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

        public bool BIsTimedTrial(IntPtr punSecondsAllowed, IntPtr punSecondsPlayed)
        {
            Write("BIsTimedTrial");
            WriteUInt32(punSecondsAllowed, 0);
            WriteUInt32(punSecondsPlayed, 0);
            return false;
        }

        public int GetLaunchCommandLine(IntPtr pszCommandLine, int cubCommandLine)
        {
            Write("GetLaunchCommandLine");
            WriteUtf8Buffer(pszCommandLine, cubCommandLine, string.Empty);
            return 0;
        }

        public bool SetDlcContext(uint nAppID)
        {
            if (nAppID == 0)
            {
                currentDlcContext = 0;
                Write("SetDlcContext cleared");
                return true;
            }

            if (!DLCManager.IsOwned(nAppID))
            {
                Write($"SetDlcContext (AppID = {nAppID}) = false");
                return false;
            }

            currentDlcContext = nAppID;
            Write($"SetDlcContext (AppID = {currentDlcContext}) = true");
            return true;
        }

        public int GetNumBetas(IntPtr pnAvailable, IntPtr pnPrivate)
        {
            WriteUInt32(pnAvailable, 1);
            WriteUInt32(pnPrivate, 0);
            Write("GetNumBetas = 1");
            return 1;
        }

        public bool GetBetaInfo(
            int iBetaIndex,
            IntPtr punFlags,
            IntPtr punBuildID,
            IntPtr pchBetaName,
            int cchBetaName,
            IntPtr pchDescription,
            int cchDescription,
            IntPtr punLastUpdated)
        {
            if (iBetaIndex != 0)
            {
                Write($"GetBetaInfo ({iBetaIndex}) = false");
                return false;
            }

            WriteUInt32(punFlags, 0);
            WriteUInt32(punBuildID, unchecked((uint)GetAppBuildId()));
            WriteUtf8Buffer(pchBetaName, cchBetaName, "public");
            WriteUtf8Buffer(pchDescription, cchDescription, "Default branch");
            WriteUInt32(punLastUpdated, 0);
            Write("GetBetaInfo (0) = public");
            return true;
        }

        public bool GetBetaInfo(
            int iBetaIndex,
            IntPtr punFlags,
            IntPtr punBuildID,
            IntPtr pchBetaName,
            int cchBetaName,
            IntPtr pchDescription,
            int cchDescription)
        {
            return GetBetaInfo(
                iBetaIndex,
                punFlags,
                punBuildID,
                pchBetaName,
                cchBetaName,
                pchDescription,
                cchDescription,
                IntPtr.Zero);
        }

        public bool SetActiveBeta(string pchBetaName)
        {
            string requested = (pchBetaName ?? string.Empty).Trim();
            if (requested.Length == 0 || requested.Equals("public", StringComparison.OrdinalIgnoreCase))
            {
                activeBetaName = string.Empty;
                Write("SetActiveBeta = public");
                return true;
            }

            Write($"SetActiveBeta ({requested}) = false");
            return false;
        }

        private static int WriteUtf8Buffer(IntPtr destination, uint destinationSize, string value)
        {
            return WriteUtf8Buffer(destination, destinationSize > int.MaxValue ? int.MaxValue : (int)destinationSize, value);
        }

        private static int WriteUtf8Buffer(IntPtr destination, int destinationSize, string value)
        {
            if (destination == IntPtr.Zero || destinationSize <= 0)
            {
                return 0;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
            int length = Math.Min(bytes.Length, destinationSize - 1);
            if (length > 0)
            {
                Marshal.Copy(bytes, 0, destination, length);
            }

            Marshal.WriteByte(destination, length, 0);
            return length;
        }

        private static void WriteUInt32(IntPtr destination, uint value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteInt32(destination, unchecked((int)value));
            }
        }

        private static void WriteUInt64(IntPtr destination, ulong value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteInt64(destination, unchecked((long)value));
            }
        }

        private static ulong GetInstalledContentSize(uint appID)
        {
            if (!AppContentManager.TryGetAppInstallPath(appID, out string path) || string.IsNullOrEmpty(path) || !System.IO.Directory.Exists(path))
            {
                return 0;
            }

            ulong total = 0;
            try
            {
                foreach (string file in System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.AllDirectories))
                {
                    try
                    {
                        total += unchecked((ulong)new System.IO.FileInfo(file).Length);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }

            return total;
        }
    }
}
