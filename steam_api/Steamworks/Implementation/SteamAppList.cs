using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamAppList : ISteamInterface
    {
        public static SteamAppList Instance;

        public SteamAppList()
        {
            Instance = this;
            InterfaceName = "SteamAppList";
            InterfaceVersion = "STEAMAPPLIST_INTERFACE_VERSION001";
        }

        public int GetAppBuildId(uint nAppID)
        {
            Write($"GetAppBuildId {nAppID}");
            return 10;
        }

        public int GetAppInstallDir(uint nAppID, string pchDirectory, int cchNameMax)
        {
            Write($"GetAppInstallDir {nAppID}");
            return AppContentManager.TryGetAppInstallPath(nAppID, out string path) && !string.IsNullOrEmpty(path)
                ? Encoding.UTF8.GetByteCount(path)
                : -1;
        }

        public int GetAppInstallDir(uint nAppID, IntPtr pchDirectory, int cchNameMax)
        {
            if (!AppContentManager.TryGetAppInstallPath(nAppID, out string path) || string.IsNullOrEmpty(path))
            {
                NativeStringCache.WriteUtf8Buffer(pchDirectory, cchNameMax, string.Empty);
                Write($"GetAppInstallDir {nAppID} = -1");
                return -1;
            }

            NativeStringCache.WriteUtf8Buffer(pchDirectory, cchNameMax, path);
            int length = Encoding.UTF8.GetByteCount(path);
            Write($"GetAppInstallDir {nAppID} = {path} ({length} bytes)");
            return length;
        }

        public int GetAppName(uint nAppID, string pchName, int cchNameMax)
        {
            Write($"GetAppName {nAppID}");
            return TryGetAppName(nAppID, out string name) ? Encoding.UTF8.GetByteCount(name) : -1;
        }

        public int GetAppName(uint nAppID, IntPtr pchName, int cchNameMax)
        {
            if (!TryGetAppName(nAppID, out string name))
            {
                NativeStringCache.WriteUtf8Buffer(pchName, cchNameMax, string.Empty);
                Write($"GetAppName {nAppID} = -1");
                return -1;
            }

            NativeStringCache.WriteUtf8Buffer(pchName, cchNameMax, name);
            int length = Encoding.UTF8.GetByteCount(name);
            Write($"GetAppName {nAppID} = {name}");
            return length;
        }

        public uint GetInstalledApps(uint pvecAppID, uint unMaxAppIDs)
        {
            Write($"GetInstalledApps legacy max={unMaxAppIDs}");
            return (uint)Math.Min(AppContentManager.GetInstalledApps().Count, unMaxAppIDs);
        }

        public uint GetInstalledApps(IntPtr pvecAppID, uint unMaxAppIDs)
        {
            IReadOnlyList<uint> apps = AppContentManager.GetInstalledApps();
            uint count = (uint)Math.Min(apps.Count, unMaxAppIDs);
            if (pvecAppID != IntPtr.Zero && count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Marshal.WriteInt32(pvecAppID, i * sizeof(uint), unchecked((int)apps[i]));
                }
            }

            Write($"GetInstalledApps max={unMaxAppIDs} = {count}");
            return count;
        }

        public uint GetNumInstalledApps()
        {
            uint count = (uint)AppContentManager.GetInstalledApps().Count;
            Write($"GetNumInstalledApps = {count}");
            return count;
        }

        private static bool TryGetAppName(uint appId, out string name)
        {
            if (DLCManager.TryGetName(appId, out name) && !string.IsNullOrEmpty(name))
            {
                return true;
            }

            if (appId == SteamEmulator.AppID)
            {
                name = $"App {appId}";
                return true;
            }

            name = string.Empty;
            return false;
        }
    }
}
