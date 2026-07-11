using System.Collections.Generic;
using SKYNET.Types;

namespace SKYNET.Managers
{
    /// <summary>
    /// Central store and query logic for DLC ownership. When <see cref="UnlockAll"/>
    /// is enabled every DLC is reported as owned/installed; otherwise only explicitly
    /// configured DLCs that are marked available are reported.
    /// </summary>
    public static class DLCManager
    {
        private static readonly object Sync = new object();

        public static bool UnlockAll
        {
            get { return SteamEmulator.UnlockAllDLC; }
            set { SteamEmulator.UnlockAllDLC = value; }
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
            }
        }

        public static void AddOrUpdate(uint appId, string name, bool available = true)
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
                    return;
                }

                Store.Add(new DLC
                {
                    AppId = appId,
                    Name = name ?? string.Empty,
                    Available = available
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
        /// Returns true when the given DLC AppId should be reported as owned/installed.
        /// </summary>
        public static bool HasDLC(uint appId)
        {
            if (UnlockAll)
            {
                return true;
            }

            lock (Sync)
            {
                var entry = Store.Find(d => d.AppId == appId);
                return entry != null && entry.Available;
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
    }
}
