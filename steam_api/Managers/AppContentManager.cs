using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SKYNET.Managers
{
    /// <summary>
    /// Runtime view of Steam content installed beside the launched game.
    /// DLC ownership is handled by <see cref="DLCManager"/>; this manager exposes
    /// the generic content surfaces games use after ownership checks: installed
    /// apps, app install directories, and installed depots.
    /// </summary>
    public static class AppContentManager
    {
        private static readonly object Sync = new object();
        private static readonly HashSet<uint> InstalledApps = new HashSet<uint>();
        private static readonly Dictionary<uint, string> AppInstallPaths = new Dictionary<uint, string>();
        private static readonly List<uint> GlobalDepots = new List<uint>();
        private static readonly Dictionary<uint, List<uint>> AppDepots = new Dictionary<uint, List<uint>>();

        public static void Clear()
        {
            lock (Sync)
            {
                InstalledApps.Clear();
                AppInstallPaths.Clear();
                GlobalDepots.Clear();
                AppDepots.Clear();
            }
        }

        public static void MarkInstalled(uint appId)
        {
            if (appId == 0 || appId == uint.MaxValue)
            {
                return;
            }

            lock (Sync)
            {
                InstalledApps.Add(appId);
            }
        }

        public static void SetAppInstallPath(uint appId, string path)
        {
            if (appId == 0 || appId == uint.MaxValue)
            {
                return;
            }

            lock (Sync)
            {
                AppInstallPaths[appId] = path ?? string.Empty;
                InstalledApps.Add(appId);
            }
        }

        public static void AddGlobalDepot(uint depotId)
        {
            if (depotId == 0 || depotId == uint.MaxValue)
            {
                return;
            }

            lock (Sync)
            {
                if (!GlobalDepots.Contains(depotId))
                {
                    GlobalDepots.Add(depotId);
                }
            }
        }

        public static void AddAppDepot(uint appId, uint depotId)
        {
            if (appId == 0 || appId == uint.MaxValue || depotId == 0 || depotId == uint.MaxValue)
            {
                return;
            }

            lock (Sync)
            {
                if (!AppDepots.TryGetValue(appId, out var depots))
                {
                    depots = new List<uint>();
                    AppDepots[appId] = depots;
                }

                if (!depots.Contains(depotId))
                {
                    depots.Add(depotId);
                }
            }
        }

        public static int InstalledAppCount
        {
            get
            {
                lock (Sync)
                {
                    return InstalledApps.Count;
                }
            }
        }

        public static int AppPathCount
        {
            get
            {
                lock (Sync)
                {
                    return AppInstallPaths.Count;
                }
            }
        }

        public static int DepotCount
        {
            get
            {
                lock (Sync)
                {
                    return GlobalDepots.Count + AppDepots.Values.Sum(d => d.Count);
                }
            }
        }

        public static bool IsAppInstalled(uint appId)
        {
            if (appId == 0 || appId == uint.MaxValue)
            {
                return false;
            }

            if (appId == SteamEmulator.AppID)
            {
                return true;
            }

            lock (Sync)
            {
                if (InstalledApps.Contains(appId))
                {
                    return true;
                }
            }

            // Some games query DLC AppIDs through BIsAppInstalled even though
            // Steam documents BIsDlcInstalled as the DLC-specific API. Treating
            // installed DLC content as installed app content matches practical
            // emulator behavior without granting unknown AppIDs.
            return DLCManager.IsInstalled(appId);
        }

        public static bool TryGetAppInstallPath(uint appId, out string path)
        {
            path = string.Empty;

            if (appId == 0 || appId == uint.MaxValue)
            {
                return false;
            }

            lock (Sync)
            {
                if (AppInstallPaths.TryGetValue(appId, out path))
                {
                    return true;
                }
            }

            string detectedDlcPath = Path.Combine(Common.GetPath(), "SKYNET", "DLC", appId.ToString());
            if (Directory.Exists(detectedDlcPath))
            {
                path = detectedDlcPath;
                return true;
            }

            if (appId == SteamEmulator.AppID)
            {
                path = Common.GetPath();
                return true;
            }

            return false;
        }

        public static IReadOnlyList<uint> GetInstalledDepots(uint appId)
        {
            lock (Sync)
            {
                if (AppDepots.TryGetValue(appId, out var appDepots) && appDepots.Count > 0)
                {
                    return appDepots.ToArray();
                }

                return GlobalDepots.ToArray();
            }
        }

        public static IReadOnlyList<uint> GetInstalledApps()
        {
            var apps = new SortedSet<uint>();
            if (SteamEmulator.AppID != 0 && SteamEmulator.AppID != uint.MaxValue)
            {
                apps.Add(SteamEmulator.AppID);
            }

            lock (Sync)
            {
                foreach (uint appId in InstalledApps)
                {
                    apps.Add(appId);
                }

                foreach (uint appId in AppInstallPaths.Keys)
                {
                    apps.Add(appId);
                }
            }

            foreach (uint appId in DLCManager.GetInstalledAppIds())
            {
                apps.Add(appId);
            }

            return apps.ToArray();
        }
    }
}
