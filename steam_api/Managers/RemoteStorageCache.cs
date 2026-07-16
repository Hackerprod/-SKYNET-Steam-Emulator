using SKYNET.Managers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace SKYNET.Steamworks.Implementation
{
    /// <summary>
    /// Thread-safe in-memory cache of remote storage file metadata.
    /// Merges local disk state with the server manifest so FileExists/FileRead
    /// can avoid blocking HTTP calls on the game thread.
    /// </summary>
    public static class RemoteStorageCache
    {
        public enum ManifestStateEnum
        {
            Unknown,
            Fresh,
            Failed
        }

        public sealed class CacheEntry
        {
            public string NormalizedName { get; set; }
            public string OriginalName { get; set; }
            public int Size { get; set; }
            public uint Timestamp { get; set; }
            public uint SyncPlatforms { get; set; }
            public bool HasSyncPlatforms { get; set; }
            public string Sha256 { get; set; }
            public bool IsHydrated { get; set; }
            public bool IsPersisted { get; set; }
            public bool DeletedLocally { get; set; }
            public DateTime MissingUntil { get; set; }
        }

        private static readonly ConcurrentDictionary<string, CacheEntry> Entries =
            new ConcurrentDictionary<string, CacheEntry>(StringComparer.Ordinal);

        private static volatile ManifestStateEnum _manifestState = ManifestStateEnum.Unknown;

        public static ManifestStateEnum ManifestState => _manifestState;

        /// <summary>
        /// Canonical key for any remote storage file name: lowercase, forward slashes,
        /// no absolute paths, no drive prefixes, and no traversal segments.
        /// </summary>
        public static string Normalize(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName))
            {
                return string.Empty;
            }

            var name = rawName.Replace('\\', '/').Trim().ToLowerInvariant();

            // Reject absolute or drive-qualified paths instead of silently rewriting
            // them into valid remote names.
            if (name.IndexOf(':') >= 0 || name.StartsWith("/"))
            {
                return string.Empty;
            }

            var segments = name.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 0)
            {
                return string.Empty;
            }

            foreach (var segment in segments)
            {
                if (segment == "." || segment == "..")
                {
                    return string.Empty;
                }
            }

            return string.Join("/", segments);
        }

        /// <summary>
        /// Scans local disk under storagePath and builds hydrated entries.
        /// This must not touch the network.
        /// </summary>
        public static void Initialize(string storagePath)
        {
            Entries.Clear();
            _manifestState = ManifestStateEnum.Unknown;

            if (!Directory.Exists(storagePath))
            {
                return;
            }

            var root = Path.GetFullPath(storagePath).TrimEnd(
                Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) +
                Path.DirectorySeparatorChar;

            foreach (var fullPath in Directory.GetFiles(storagePath, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    var absolutePath = Path.GetFullPath(fullPath);
                    if (!absolutePath.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var originalName = absolutePath.Substring(root.Length).Replace('\\', '/');
                    var normalized = Normalize(originalName);
                    if (string.IsNullOrEmpty(normalized))
                    {
                        continue;
                    }

                    var info = new FileInfo(fullPath);
                    Entries[normalized] = new CacheEntry
                    {
                        NormalizedName = normalized,
                        OriginalName = originalName,
                        Size = (int)Math.Min(info.Length, int.MaxValue),
                        Timestamp = (uint)((DateTimeOffset)info.LastWriteTimeUtc).ToUnixTimeSeconds(),
                        IsHydrated = true,
                    };
                }
                catch
                {
                    // Ignore files that cannot be read while the game is starting.
                }
            }
        }

        /// <summary>
        /// Merges a server manifest. A null list means the fetch failed.
        /// </summary>
        public static void MergeRemoteList(List<SkyNetApiClient.ApiRemoteStorageFileListItem> files)
        {
            if (files == null)
            {
                _manifestState = ManifestStateEnum.Failed;
                SteamEmulator.Write("RemoteStorageCache", "Manifest load failed (null list).");
                return;
            }

            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var file in files)
            {
                if (file == null || string.IsNullOrWhiteSpace(file.FileName))
                {
                    continue;
                }

                var normalized = Normalize(file.FileName);
                if (string.IsNullOrEmpty(normalized))
                {
                    continue;
                }

                seen.Add(normalized);
                Entries.AddOrUpdate(
                    normalized,
                    key => new CacheEntry
                    {
                        NormalizedName = normalized,
                        OriginalName = file.FileName,
                        Size = file.Size,
                        Timestamp = file.Timestamp,
                        SyncPlatforms = file.SyncPlatforms,
                        HasSyncPlatforms = true,
                        Sha256 = file.Sha256,
                        IsPersisted = true,
                    },
                    (key, existing) =>
                    {
                        existing.OriginalName = file.FileName;
                        existing.Size = file.Size;
                        existing.Timestamp = file.Timestamp;
                        existing.SyncPlatforms = file.SyncPlatforms;
                        existing.HasSyncPlatforms = true;
                        existing.Sha256 = file.Sha256;
                        existing.IsPersisted = true;
                        existing.DeletedLocally = false;
                        existing.MissingUntil = DateTime.MinValue;
                        return existing;
                    });
            }

            foreach (var pair in Entries)
            {
                if (!seen.Contains(pair.Key))
                {
                    pair.Value.IsPersisted = false;
                }
            }

            _manifestState = ManifestStateEnum.Fresh;
            SteamEmulator.Write("RemoteStorageCache", $"Manifest loaded: {files.Count} remote files.");
        }

        public static CacheEntry GetEntry(string normalizedName)
        {
            if (string.IsNullOrEmpty(normalizedName))
            {
                return null;
            }

            Entries.TryGetValue(normalizedName, out var entry);
            return entry;
        }

        public static bool IsKnownMissing(string normalizedName)
        {
            if (string.IsNullOrEmpty(normalizedName) ||
                !Entries.TryGetValue(normalizedName, out var entry))
            {
                return false;
            }

            if (entry.MissingUntil <= DateTime.MinValue)
            {
                return false;
            }

            if (DateTime.UtcNow < entry.MissingUntil)
            {
                return true;
            }

            entry.MissingUntil = DateTime.MinValue;
            return false;
        }

        public static void SetHydrated(string normalizedName, string originalName, int size, string sha256)
        {
            if (string.IsNullOrEmpty(normalizedName))
            {
                return;
            }

            Entries.AddOrUpdate(
                normalizedName,
                key => new CacheEntry
                {
                    NormalizedName = normalizedName,
                    OriginalName = originalName,
                    Size = size,
                    Sha256 = sha256,
                    IsHydrated = true,
                    Timestamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                },
                (key, existing) =>
                {
                    existing.OriginalName = originalName;
                    existing.Size = size;
                    existing.Sha256 = sha256;
                    existing.IsHydrated = true;
                    existing.DeletedLocally = false;
                    existing.MissingUntil = DateTime.MinValue;
                    existing.Timestamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    return existing;
                });
        }

        public static void SetPersisted(string normalizedName)
        {
            if (string.IsNullOrEmpty(normalizedName))
            {
                return;
            }

            Entries.AddOrUpdate(
                normalizedName,
                key => new CacheEntry
                {
                    NormalizedName = normalizedName,
                    IsPersisted = true,
                },
                (key, existing) =>
                {
                    existing.IsPersisted = true;
                    existing.DeletedLocally = false;
                    existing.MissingUntil = DateTime.MinValue;
                    return existing;
                });
        }

        public static void SetSyncPlatforms(string normalizedName, uint syncPlatforms)
        {
            if (string.IsNullOrEmpty(normalizedName))
            {
                return;
            }

            Entries.AddOrUpdate(
                normalizedName,
                key => new CacheEntry
                {
                    NormalizedName = normalizedName,
                    SyncPlatforms = syncPlatforms,
                    HasSyncPlatforms = true,
                },
                (key, existing) =>
                {
                    existing.SyncPlatforms = syncPlatforms;
                    existing.HasSyncPlatforms = true;
                    return existing;
                });
        }

        public static void SetDeletedLocally(string normalizedName, TimeSpan ttl)
        {
            if (string.IsNullOrEmpty(normalizedName))
            {
                return;
            }

            var expiresAt = DateTime.UtcNow + ttl;
            Entries.AddOrUpdate(
                normalizedName,
                key => new CacheEntry
                {
                    NormalizedName = normalizedName,
                    IsHydrated = false,
                    IsPersisted = false,
                    DeletedLocally = true,
                    MissingUntil = expiresAt,
                },
                (key, existing) =>
                {
                    existing.IsHydrated = false;
                    existing.IsPersisted = false;
                    existing.DeletedLocally = true;
                    existing.MissingUntil = expiresAt;
                    return existing;
                });
        }

        public static void MarkMissing(string normalizedName, TimeSpan ttl)
        {
            if (string.IsNullOrEmpty(normalizedName))
            {
                return;
            }

            var expiresAt = DateTime.UtcNow + ttl;
            Entries.AddOrUpdate(
                normalizedName,
                key => new CacheEntry
                {
                    NormalizedName = normalizedName,
                    MissingUntil = expiresAt,
                },
                (key, existing) =>
                {
                    existing.MissingUntil = expiresAt;
                    return existing;
                });
        }
    }
}
