using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Win32;
using SKYNET.Helpers;

namespace SKYNET.Managers
{
    internal sealed class AppManifestInfo
    {
        public uint AppId { get; set; }
        public int BuildId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string InstallDirectory { get; set; } = string.Empty;
        public string ManifestPath { get; set; } = string.Empty;
    }

    /// <summary>
    /// Resolves Steam app metadata from appmanifest files without assuming that a
    /// game is installed in the default Steam library. Results are cached and
    /// invalidated when the manifest timestamp changes.
    /// </summary>
    internal static class AppManifestManager
    {
        private sealed class CacheEntry
        {
            public DateTime CheckedUtc { get; set; }
            public DateTime LastWriteUtc { get; set; }
            public string ManifestPath { get; set; } = string.Empty;
            public AppManifestInfo Manifest { get; set; }
        }

        private static readonly object Sync = new object();
        private static readonly Dictionary<uint, CacheEntry> Cache = new Dictionary<uint, CacheEntry>();
        private static readonly TimeSpan NegativeCacheLifetime = TimeSpan.FromSeconds(10);

        public static bool TryGet(uint appId, out AppManifestInfo manifest)
        {
            manifest = null;
            if (appId == 0 || appId == uint.MaxValue)
            {
                return false;
            }

            CacheEntry cached;
            lock (Sync)
            {
                Cache.TryGetValue(appId, out cached);
            }

            if (IsCurrent(cached))
            {
                manifest = cached.Manifest;
                return manifest != null;
            }

            AppManifestInfo loaded = null;
            string manifestPath = FindManifestPath(appId);
            DateTime lastWriteUtc = DateTime.MinValue;
            if (!string.IsNullOrEmpty(manifestPath))
            {
                try
                {
                    lastWriteUtc = File.GetLastWriteTimeUtc(manifestPath);
                    loaded = ParseManifest(appId, manifestPath);
                }
                catch (Exception ex)
                {
                    SteamEmulator.Write("AppManifest", $"Failed to read {manifestPath}: {ex.Message}");
                }
            }

            var replacement = new CacheEntry
            {
                CheckedUtc = DateTime.UtcNow,
                LastWriteUtc = lastWriteUtc,
                ManifestPath = manifestPath ?? string.Empty,
                Manifest = loaded
            };

            lock (Sync)
            {
                Cache[appId] = replacement;
            }

            manifest = loaded;
            return manifest != null;
        }

        private static bool IsCurrent(CacheEntry entry)
        {
            if (entry == null)
            {
                return false;
            }

            if (entry.Manifest == null)
            {
                return DateTime.UtcNow - entry.CheckedUtc < NegativeCacheLifetime;
            }

            try
            {
                return File.Exists(entry.ManifestPath) &&
                       File.GetLastWriteTimeUtc(entry.ManifestPath) == entry.LastWriteUtc;
            }
            catch
            {
                return false;
            }
        }

        private static AppManifestInfo ParseManifest(uint requestedAppId, string manifestPath)
        {
            ValveKeyValue root = ValveKeyValue.ParseFile(manifestPath);
            ValveKeyValue appState = root.Child("AppState");
            if (appState == null)
            {
                return null;
            }

            string appIdText = appState.GetValue("appid");
            if (uint.TryParse(appIdText, NumberStyles.None, CultureInfo.InvariantCulture, out uint manifestAppId) &&
                manifestAppId != requestedAppId)
            {
                return null;
            }

            int buildId = 0;
            int.TryParse(
                appState.GetValue("buildid"),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out buildId);

            string steamAppsDirectory = Path.GetDirectoryName(manifestPath) ?? string.Empty;
            string installDirectoryName = appState.GetValue("installdir");
            string installDirectory = string.IsNullOrWhiteSpace(installDirectoryName)
                ? string.Empty
                : Path.GetFullPath(Path.Combine(steamAppsDirectory, "common", installDirectoryName));

            return new AppManifestInfo
            {
                AppId = requestedAppId,
                BuildId = Math.Max(0, buildId),
                Name = appState.GetValue("name"),
                InstallDirectory = installDirectory,
                ManifestPath = manifestPath
            };
        }

        private static string FindManifestPath(uint appId)
        {
            string fileName = $"appmanifest_{appId}.acf";
            var steamAppsDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var directCandidates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            AddProcessPathCandidates(fileName, steamAppsDirectories, directCandidates);
            AddRegisteredSteamPath(Registry.CurrentUser, @"Software\Valve\Steam", steamAppsDirectories);
            AddRegisteredSteamPath(Registry.LocalMachine, @"Software\Valve\Steam", steamAppsDirectories);
            AddRegisteredSteamPath(Registry.LocalMachine, @"Software\WOW6432Node\Valve\Steam", steamAppsDirectories);

            foreach (string candidate in directCandidates)
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            var expandedDirectories = new HashSet<string>(steamAppsDirectories, StringComparer.OrdinalIgnoreCase);
            foreach (string steamAppsDirectory in steamAppsDirectories)
            {
                AddLibraryFolders(steamAppsDirectory, expandedDirectories);
            }

            foreach (string steamAppsDirectory in expandedDirectories)
            {
                string candidate = Path.Combine(steamAppsDirectory, fileName);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return string.Empty;
        }

        private static void AddProcessPathCandidates(
            string manifestFileName,
            ISet<string> steamAppsDirectories,
            ISet<string> directCandidates)
        {
            string processPath;
            try
            {
                processPath = Common.GetPath();
            }
            catch
            {
                return;
            }

            var directory = string.IsNullOrWhiteSpace(processPath)
                ? null
                : new DirectoryInfo(processPath);

            for (int depth = 0; directory != null && depth < 10; depth++, directory = directory.Parent)
            {
                directCandidates.Add(Path.Combine(directory.FullName, manifestFileName));
                if (directory.Name.Equals("steamapps", StringComparison.OrdinalIgnoreCase))
                {
                    steamAppsDirectories.Add(directory.FullName);
                }

                string nestedSteamApps = Path.Combine(directory.FullName, "steamapps");
                if (Directory.Exists(nestedSteamApps))
                {
                    steamAppsDirectories.Add(nestedSteamApps);
                }
            }
        }

        private static void AddRegisteredSteamPath(
            RegistryKey hive,
            string subKeyPath,
            ISet<string> steamAppsDirectories)
        {
            try
            {
                using (RegistryKey key = hive.OpenSubKey(subKeyPath))
                {
                    string steamPath = key?.GetValue("SteamPath") as string ??
                                       key?.GetValue("InstallPath") as string;
                    AddSteamAppsDirectory(steamPath, steamAppsDirectories);
                }
            }
            catch
            {
            }
        }

        private static void AddLibraryFolders(string steamAppsDirectory, ISet<string> directories)
        {
            string libraryFile = Path.Combine(steamAppsDirectory, "libraryfolders.vdf");
            if (!File.Exists(libraryFile))
            {
                return;
            }

            try
            {
                ValveKeyValue root = ValveKeyValue.ParseFile(libraryFile);
                ValveKeyValue libraries = root.Child("libraryfolders") ?? root;
                foreach (ValveKeyValue library in libraries.Children)
                {
                    string libraryPath = library.IsObject
                        ? library.GetValue("path")
                        : library.Value;
                    AddSteamAppsDirectory(libraryPath, directories);
                }
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("AppManifest", $"Failed to read {libraryFile}: {ex.Message}");
            }
        }

        private static void AddSteamAppsDirectory(string steamRoot, ISet<string> directories)
        {
            if (string.IsNullOrWhiteSpace(steamRoot))
            {
                return;
            }

            try
            {
                string normalized = steamRoot.Trim().Replace('/', Path.DirectorySeparatorChar);
                string steamAppsDirectory = Path.GetFileName(normalized.TrimEnd(Path.DirectorySeparatorChar))
                    .Equals("steamapps", StringComparison.OrdinalIgnoreCase)
                    ? normalized
                    : Path.Combine(normalized, "steamapps");

                if (Directory.Exists(steamAppsDirectory))
                {
                    directories.Add(Path.GetFullPath(steamAppsDirectory));
                }
            }
            catch
            {
            }
        }
    }
}
