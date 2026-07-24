using System.Collections.Generic;
using System.Linq;
using SKYNET.Types;

namespace SKYNET.Managers
{
    /// <summary>
    /// Central store for DLC catalog visibility, ownership and runtime install state.
    /// Unlock-all grants ownership/install state while explicit uninstall overrides
    /// remain effective for the lifetime of the process.
    /// </summary>
    public static class DLCManager
    {
        private static readonly object Sync = new object();
        private static readonly HashSet<uint> InstalledOverrides = new HashSet<uint>();
        private static readonly HashSet<uint> UninstalledOverrides = new HashSet<uint>();

        public static bool UnlockAll
        {
            get
            {
                lock (Sync)
                {
                    return SteamEmulator.UnlockAllDLC;
                }
            }
            set
            {
                lock (Sync)
                {
                    SteamEmulator.UnlockAllDLC = value;
                }
            }
        }

        private static List<DLC> Store
        {
            get
            {
                if (SteamEmulator.DLCs == null)
                {
                    SteamEmulator.DLCs = new List<DLC>();
                }
                return SteamEmulator.DLCs;
            }
        }

        public static void Clear()
        {
            lock (Sync)
            {
                Store.Clear();
                InstalledOverrides.Clear();
                UninstalledOverrides.Clear();
            }
        }

        public static void AddOrUpdate(
            uint appId,
            string name,
            bool available = true,
            bool owned = true,
            bool installed = true)
        {
            if (appId == 0)
            {
                return;
            }

            lock (Sync)
            {
                var existing = Store.Find(d => d.AppId == appId);
                if (existing != null)
                {
                    existing.Name = name ?? string.Empty;
                    existing.Available = available;
                    existing.Owned = owned;
                    existing.Installed = installed;
                    return;
                }

                Store.Add(new DLC
                {
                    AppId = appId,
                    Name = name ?? string.Empty,
                    Available = available,
                    Owned = owned,
                    Installed = installed
                });
            }
        }

        public static int Count
        {
            get
            {
                lock (Sync)
                {
                    return Store.Count;
                }
            }
        }

        /// <summary>
        /// Returns true when the given DLC AppId is both owned and installed.
        /// </summary>
        public static bool HasDLC(uint appId)
        {
            lock (Sync)
            {
                return IsOwnedLocked(appId) && IsInstalledLocked(appId);
            }
        }

        public static bool IsOwned(uint appId)
        {
            lock (Sync)
            {
                return IsOwnedLocked(appId);
            }
        }

        public static bool IsInstalled(uint appId)
        {
            lock (Sync)
            {
                return IsInstalledLocked(appId);
            }
        }

        public static bool TryInstall(uint appId)
        {
            lock (Sync)
            {
                if (!IsOwnedLocked(appId))
                {
                    return false;
                }

                UninstalledOverrides.Remove(appId);
                InstalledOverrides.Add(appId);

                var entry = Store.Find(d => d.AppId == appId);
                if (entry != null)
                {
                    entry.Installed = true;
                }

                return true;
            }
        }

        public static bool TryUninstall(uint appId)
        {
            lock (Sync)
            {
                if (!IsOwnedLocked(appId))
                {
                    return false;
                }

                InstalledOverrides.Remove(appId);
                UninstalledOverrides.Add(appId);

                var entry = Store.Find(d => d.AppId == appId);
                if (entry != null)
                {
                    entry.Installed = false;
                }

                return true;
            }
        }

        public static bool TryGet(int index, out uint appId, out bool available, out string name)
        {
            appId = 0;
            available = false;
            name = string.Empty;

            lock (Sync)
            {
                if (index < 0 || index >= Store.Count)
                {
                    return false;
                }

                var entry = Store[index];
                appId = entry.AppId;
                available = entry.Available;
                name = entry.Name ?? string.Empty;
                return true;
            }
        }

        public static IReadOnlyList<uint> GetInstalledAppIds()
        {
            lock (Sync)
            {
                return Store
                    .Where(d => d.AppId != 0 && d.Installed && IsInstalledLocked(d.AppId))
                    .Select(d => d.AppId)
                    .ToArray();
            }
        }

        public static bool TryGetName(uint appId, out string name)
        {
            name = string.Empty;

            lock (Sync)
            {
                var entry = Store.Find(d => d.AppId == appId);
                if (entry == null)
                {
                    return false;
                }

                name = entry.Name ?? string.Empty;
                return true;
            }
        }

        private static bool IsOwnedLocked(uint appId)
        {
            if (appId == 0)
            {
                return false;
            }

            if (SteamEmulator.UnlockAllDLC)
            {
                return true;
            }

            var entry = Store.Find(d => d.AppId == appId);
            return entry != null && entry.Owned;
        }

        private static bool IsInstalledLocked(uint appId)
        {
            if (appId == 0 || UninstalledOverrides.Contains(appId))
            {
                return false;
            }

            if (InstalledOverrides.Contains(appId) || SteamEmulator.UnlockAllDLC)
            {
                return true;
            }

            var entry = Store.Find(d => d.AppId == appId);
            return entry != null && entry.Installed;
        }
    }
}
