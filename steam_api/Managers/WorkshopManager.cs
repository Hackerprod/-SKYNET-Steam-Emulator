using SKYNET.Helper;
using SKYNET.Helpers;
using SKYNET.Helpers.JSON;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SKYNET.Managers
{
    /// <summary>
    /// Maintains Steam Workshop's local client view. The server owns catalog and
    /// subscription state; this cache makes the synchronous ISteamUGC read calls
    /// deterministic and keeps the last server snapshot available while offline.
    /// Workshop content remains local and is resolved from conventional Steam or
    /// emulator-managed content roots.
    /// </summary>
    public static class WorkshopManager
    {
        private static readonly object Gate = new object();
        private static readonly Dictionary<ulong, APIClient.WorkshopSubscriptionDto> Subscriptions =
            new Dictionary<ulong, APIClient.WorkshopSubscriptionDto>();
        private static readonly Dictionary<ulong, InstallInfo> InstallInfoCache =
            new Dictionary<ulong, InstallInfo>();

        private static ulong CurrentSteamId;
        private static uint CurrentAppId;
        private static bool Loaded;

        public static void Initialize()
        {
            EnsureIdentity((ulong)SteamEmulator.SteamID, SteamEmulator.InternalAppId);
        }

        public static void ApplyServerSnapshot(
            IEnumerable<APIClient.WorkshopSubscriptionDto> subscriptions,
            ulong steamId,
            uint appId)
        {
            EnsureIdentity(steamId, appId);

            lock (Gate)
            {
                Subscriptions.Clear();
                if (subscriptions != null)
                {
                    foreach (var subscription in subscriptions)
                    {
                        if (IsValidSubscription(subscription, appId))
                        {
                            Subscriptions[subscription.PublishedFileId] = CloneSubscription(subscription);
                        }
                    }
                }

                InstallInfoCache.Clear();
                QueueSaveSnapshotLocked();
            }
        }

        public static APIClient.WorkshopSubscriptionDto[] GetSubscriptions(bool includeLocallyDisabled)
        {
            EnsureIdentity((ulong)SteamEmulator.SteamID, SteamEmulator.InternalAppId);
            lock (Gate)
            {
                return Subscriptions.Values
                    .Where(subscription => includeLocallyDisabled || !subscription.DisabledLocally)
                    .OrderBy(subscription => subscription.SubscribedAtUtc)
                    .ThenBy(subscription => subscription.PublishedFileId)
                    .Select(CloneSubscription)
                    .ToArray();
            }
        }

        public static bool TryGetItem(
            ulong publishedFileId,
            out APIClient.WorkshopItemDto item)
        {
            EnsureIdentity((ulong)SteamEmulator.SteamID, SteamEmulator.InternalAppId);
            lock (Gate)
            {
                if (Subscriptions.TryGetValue(publishedFileId, out var subscription) &&
                    subscription.Item != null)
                {
                    item = CloneItem(subscription.Item);
                    return true;
                }
            }

            item = null;
            return false;
        }

        public static void UpsertSubscription(APIClient.WorkshopSubscriptionDto subscription)
        {
            if (!IsValidSubscription(subscription, SteamEmulator.InternalAppId))
            {
                return;
            }

            EnsureIdentity((ulong)SteamEmulator.SteamID, SteamEmulator.InternalAppId);
            lock (Gate)
            {
                Subscriptions[subscription.PublishedFileId] = CloneSubscription(subscription);
                InstallInfoCache.Remove(subscription.PublishedFileId);
                QueueSaveSnapshotLocked();
            }
        }

        public static void RemoveSubscription(ulong publishedFileId)
        {
            EnsureIdentity((ulong)SteamEmulator.SteamID, SteamEmulator.InternalAppId);
            lock (Gate)
            {
                if (Subscriptions.Remove(publishedFileId))
                {
                    InstallInfoCache.Remove(publishedFileId);
                    QueueSaveSnapshotLocked();
                }
            }
        }

        public static uint GetItemState(ulong publishedFileId)
        {
            EnsureIdentity((ulong)SteamEmulator.SteamID, SteamEmulator.InternalAppId);
            APIClient.WorkshopSubscriptionDto subscription;
            lock (Gate)
            {
                if (!Subscriptions.TryGetValue(publishedFileId, out subscription))
                {
                    return (uint)EItemState.k_EItemStateNone;
                }

                subscription = CloneSubscription(subscription);
            }

            uint state = (uint)EItemState.k_EItemStateSubscribed;
            if (subscription.DisabledLocally)
            {
                return state;
            }

            if (TryGetInstallInfo(publishedFileId, out _))
            {
                state |= (uint)EItemState.k_EItemStateInstalled;
            }
            else
            {
                state |= (uint)EItemState.k_EItemStateNeedsUpdate;
            }

            return state;
        }

        public static bool TryGetInstallInfo(ulong publishedFileId, out InstallInfo info)
        {
            EnsureIdentity((ulong)SteamEmulator.SteamID, SteamEmulator.InternalAppId);

            APIClient.WorkshopItemDto item = null;
            lock (Gate)
            {
                if (!Subscriptions.TryGetValue(publishedFileId, out var subscription) ||
                    subscription.DisabledLocally)
                {
                    info = null;
                    return false;
                }

                item = subscription.Item;
                if (InstallInfoCache.TryGetValue(publishedFileId, out var cached) &&
                    Directory.Exists(cached.Folder))
                {
                    info = cached.Clone();
                    return true;
                }
            }

            var folder = ResolveInstallDirectory(publishedFileId);
            if (string.IsNullOrEmpty(folder))
            {
                info = null;
                return false;
            }

            var directory = new DirectoryInfo(folder);
            var timestamp = ToUnixTime(directory.LastWriteTimeUtc);
            var size = item == null ? 0UL : ToUnsignedSize(Math.Max(item.FileSize, item.TotalFilesSize));
            var resolved = new InstallInfo
            {
                Folder = directory.FullName,
                SizeOnDisk = size,
                Timestamp = timestamp
            };

            lock (Gate)
            {
                InstallInfoCache[publishedFileId] = resolved;
            }

            info = resolved.Clone();
            return true;
        }

        public static APIClient.WorkshopItemDto TryReadInstalledItem(ulong publishedFileId)
        {
            var folder = ResolveInstallDirectory(publishedFileId);
            if (string.IsNullOrEmpty(folder))
            {
                return null;
            }

            foreach (var name in new[] { "workshop-item.json", ".workshop.json" })
            {
                var metadataPath = Path.Combine(folder, name);
                if (!File.Exists(metadataPath))
                {
                    continue;
                }

                try
                {
                    var item = File.ReadAllText(metadataPath).FromJson<APIClient.WorkshopItemDto>();
                    if (item == null)
                    {
                        continue;
                    }

                    item.PublishedFileId = publishedFileId;
                    item.CreatorAppId = item.CreatorAppId == 0 ? SteamEmulator.ReportedAppId : item.CreatorAppId;
                    item.ConsumerAppId = SteamEmulator.ReportedAppId;
                    item.OwnerSteamId = item.OwnerSteamId == 0
                        ? (ulong)SteamEmulator.SteamID
                        : item.OwnerSteamId;
                    item.TimeCreated = item.TimeCreated == 0
                        ? ToUnixTime(File.GetCreationTimeUtc(metadataPath))
                        : item.TimeCreated;
                    item.TimeUpdated = item.TimeUpdated == 0
                        ? ToUnixTime(File.GetLastWriteTimeUtc(metadataPath))
                        : item.TimeUpdated;
                    return item;
                }
                catch (Exception ex)
                {
                    SteamEmulator.Write(
                        "Workshop",
                        $"Ignoring invalid metadata {metadataPath}: {ex.Message}");
                }
            }

            return null;
        }

        private static void EnsureIdentity(ulong steamId, uint appId)
        {
            lock (Gate)
            {
                if (Loaded && CurrentSteamId == steamId && CurrentAppId == appId)
                {
                    return;
                }

                CurrentSteamId = steamId;
                CurrentAppId = appId;
                Subscriptions.Clear();
                InstallInfoCache.Clear();
                Loaded = true;
                LoadSnapshotLocked();
            }
        }

        private static void LoadSnapshotLocked()
        {
            var path = GetSnapshotPath(CurrentSteamId, CurrentAppId);
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                var snapshot = File.ReadAllText(path).FromJson<WorkshopSnapshot>();
                if (snapshot == null ||
                    snapshot.SteamId != CurrentSteamId ||
                    snapshot.AppId != CurrentAppId ||
                    snapshot.Subscriptions == null)
                {
                    return;
                }

                foreach (var subscription in snapshot.Subscriptions)
                {
                    if (IsValidSubscription(subscription, CurrentAppId))
                    {
                        Subscriptions[subscription.PublishedFileId] = CloneSubscription(subscription);
                    }
                }
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("Workshop", $"Unable to load subscription snapshot: {ex.Message}");
            }
        }

        private static void QueueSaveSnapshotLocked()
        {
            var snapshot = new WorkshopSnapshot
            {
                SteamId = CurrentSteamId,
                AppId = CurrentAppId,
                Subscriptions = Subscriptions.Values.Select(CloneSubscription).ToList()
            };
            var path = GetSnapshotPath(CurrentSteamId, CurrentAppId);
            var key = $"workshop-snapshot:{CurrentSteamId}:{CurrentAppId}";

            WorkQueue.Enqueue(
                "Save Workshop subscription snapshot",
                () => SaveSnapshot(path, snapshot),
                coalesceKey: key);
        }

        private static void SaveSnapshot(string path, WorkshopSnapshot snapshot)
        {
            string temporaryPath = null;
            try
            {
                var directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                temporaryPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
                File.WriteAllText(temporaryPath, snapshot.ToJson());
                if (File.Exists(path))
                {
                    try
                    {
                        File.Replace(temporaryPath, path, null);
                    }
                    catch (PlatformNotSupportedException)
                    {
                        ReplaceSnapshotByCopy(temporaryPath, path);
                    }
                    catch (IOException)
                    {
                        ReplaceSnapshotByCopy(temporaryPath, path);
                    }
                }
                else
                {
                    File.Move(temporaryPath, path);
                }
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("Workshop", $"Unable to save subscription snapshot: {ex.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(temporaryPath) && File.Exists(temporaryPath))
                {
                    try
                    {
                        File.Delete(temporaryPath);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static void ReplaceSnapshotByCopy(string temporaryPath, string path)
        {
            File.Copy(temporaryPath, path, true);
            File.Delete(temporaryPath);
        }

        private static string ResolveInstallDirectory(ulong publishedFileId)
        {
            var appId = SteamEmulator.InternalAppId.ToString();
            var itemId = publishedFileId.ToString();
            var gamePath = Common.GetPath();
            var candidates = new List<string>();

            AddContentRootCandidates(candidates, SteamEmulator.WorkshopContentRoot, appId, itemId);
            AddCandidate(candidates, Path.Combine(gamePath, "SKYNET", "Workshop", "Content", appId, itemId));
            AddCandidate(candidates, Path.Combine(gamePath, "workshop", "content", appId, itemId));
            AddCandidate(candidates, Path.Combine(gamePath, "workshop", itemId));

            var current = new DirectoryInfo(gamePath);
            while (current != null)
            {
                if (string.Equals(current.Name, "steamapps", StringComparison.OrdinalIgnoreCase))
                {
                    AddCandidate(candidates, Path.Combine(current.FullName, "workshop", "content", appId, itemId));
                    break;
                }
                current = current.Parent;
            }

            foreach (var candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (Directory.Exists(candidate))
                {
                    return Path.GetFullPath(candidate);
                }
            }

            return string.Empty;
        }

        private static void AddContentRootCandidates(
            ICollection<string> candidates,
            string contentRoot,
            string appId,
            string itemId)
        {
            if (string.IsNullOrWhiteSpace(contentRoot))
            {
                return;
            }

            AddCandidate(candidates, Path.Combine(contentRoot, appId, itemId));
            AddCandidate(candidates, Path.Combine(contentRoot, itemId));
        }

        private static void AddCandidate(ICollection<string> candidates, string candidate)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(candidate))
                {
                    candidates.Add(Path.GetFullPath(candidate));
                }
            }
            catch
            {
                // A malformed optional root is ignored; remaining standard roots still work.
            }
        }

        private static string GetSnapshotPath(ulong steamId, uint appId)
        {
            return Path.Combine(
                Common.GetPath(),
                "SKYNET",
                "Workshop",
                appId.ToString(),
                steamId.ToString(),
                "subscriptions.json");
        }

        private static bool IsValidSubscription(
            APIClient.WorkshopSubscriptionDto subscription,
            uint appId)
        {
            return subscription != null &&
                   subscription.PublishedFileId != 0 &&
                   subscription.Item != null &&
                   subscription.Item.PublishedFileId == subscription.PublishedFileId &&
                   subscription.Item.ConsumerAppId == appId;
        }

        private static APIClient.WorkshopSubscriptionDto CloneSubscription(
            APIClient.WorkshopSubscriptionDto value)
        {
            return new APIClient.WorkshopSubscriptionDto
            {
                PublishedFileId = value.PublishedFileId,
                SubscribedAtUtc = value.SubscribedAtUtc,
                DisabledLocally = value.DisabledLocally,
                Item = CloneItem(value.Item)
            };
        }

        private static APIClient.WorkshopItemDto CloneItem(APIClient.WorkshopItemDto value)
        {
            if (value == null)
            {
                return null;
            }

            return new APIClient.WorkshopItemDto
            {
                PublishedFileId = value.PublishedFileId,
                CreatorAppId = value.CreatorAppId,
                ConsumerAppId = value.ConsumerAppId,
                OwnerSteamId = value.OwnerSteamId,
                FileType = value.FileType,
                Title = value.Title ?? string.Empty,
                Description = value.Description ?? string.Empty,
                Tags = value.Tags ?? string.Empty,
                FileName = value.FileName ?? string.Empty,
                Metadata = value.Metadata ?? string.Empty,
                PreviewUrl = value.PreviewUrl ?? string.Empty,
                Visibility = value.Visibility,
                Banned = value.Banned,
                AcceptedForUse = value.AcceptedForUse,
                TimeCreated = value.TimeCreated,
                TimeUpdated = value.TimeUpdated,
                FileSize = value.FileSize,
                TotalFilesSize = value.TotalFilesSize,
                VotesUp = value.VotesUp,
                VotesDown = value.VotesDown,
                Score = value.Score
            };
        }

        private static uint ToUnixTime(DateTime utc)
        {
            var seconds = new DateTimeOffset(utc.ToUniversalTime()).ToUnixTimeSeconds();
            return seconds <= 0 ? 0 : seconds >= uint.MaxValue ? uint.MaxValue : (uint)seconds;
        }

        private static ulong ToUnsignedSize(long size)
        {
            return size <= 0 ? 0UL : (ulong)size;
        }

        public sealed class InstallInfo
        {
            public string Folder { get; set; }
            public ulong SizeOnDisk { get; set; }
            public uint Timestamp { get; set; }

            public InstallInfo Clone()
            {
                return new InstallInfo
                {
                    Folder = Folder,
                    SizeOnDisk = SizeOnDisk,
                    Timestamp = Timestamp
                };
            }
        }

        private sealed class WorkshopSnapshot
        {
            public ulong SteamId { get; set; }
            public uint AppId { get; set; }
            public List<APIClient.WorkshopSubscriptionDto> Subscriptions { get; set; }
        }
    }
}
