using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using SteamAPICall_t = System.UInt64;
using UGCFileWriteStreamHandle_t = System.UInt64;
using UGCHandle_t = System.UInt64;
using PublishedFileUpdateHandle_t = System.UInt64;
using System.Drawing;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamRemoteStorage : ISteamInterface
    {
        public static SteamRemoteStorage Instance;

        public string StoragePath;
        public string AvatarCachePath;
        private List<string> StorageFiles;
        private List<SkyNetApiClient.ApiRemoteStorageFileListItem> RemoteStorageFiles;
        private ConcurrentDictionary<ulong, string> SharedFiles;
        private int LastFile;
        private Dictionary<ulong, string> AsyncFilesRead;
        private ConcurrentDictionary<ulong, PendingWriteStream> PendingWriteStreams;
        private static readonly TimeSpan MissingRemoteFileWindow = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan MissingRemoteFileWindowShort = TimeSpan.FromSeconds(30);
        private ulong CachedQuotaTotalBytes = 1024UL * 1024UL * 1024UL;
        private ulong CachedQuotaAvailableBytes = 1024UL * 1024UL * 1024UL;

        public SteamRemoteStorage()
        {
            Instance = this;
            InterfaceName = "SteamRemoteStorage";
            InterfaceVersion = "STEAMREMOTESTORAGE_INTERFACE_VERSION016";
            StoragePath = Path.Combine(Common.GetPath(), "SKYNET", "Storage", "Remote");
            AvatarCachePath = Path.Combine(Common.GetPath(), "SKYNET", "Images", "AvatarCache");
            StorageFiles = new List<string>();
            RemoteStorageFiles = new List<SkyNetApiClient.ApiRemoteStorageFileListItem>();
            SharedFiles = new ConcurrentDictionary<ulong, string>();
            AsyncFilesRead = new Dictionary<ulong, string>();
            PendingWriteStreams = new ConcurrentDictionary<ulong, PendingWriteStream>();
            LastFile = 0;

            // Populate cache from disk immediately (no network, no blocking).
            RemoteStorageCache.Initialize(StoragePath);

            // Fetch server manifest in the background with a tight timeout.
            if (SkyNetApiClient.IsEnabled)
            {
                WorkQueue.Enqueue(
                    "RemoteStorage manifest-init",
                    () =>
                    {
                        var files = SkyNetApiClient.ListRemoteStorageFiles(timeoutMs: 1500);
                        RemoteStorageCache.MergeRemoteList(files);
                    },
                    coalesceKey: "storage:manifest-init",
                    highPriority: true);
            }
        }

        public bool FileWrite(string pchFile, IntPtr pvData, int cubData)
        {
            bool Result = false;
            MutexHelper.Wait("FileWrite", delegate
            {
                try
                {
                    if (!TryGetStorageFilePath(pchFile, out var normalizedKey, out var fullPath))
                    {
                        Write($"FileWrite rejected invalid path {pchFile}");
                        return;
                    }

                    Common.EnsureDirectoryExists(fullPath, true);
                    byte[] buffer = pvData.GetBytes(cubData);
                    File.WriteAllBytes(fullPath, buffer);
                    // Update cache immediately on the game thread; do not wait for WorkQueue.
                    RemoteStorageCache.SetHydrated(normalizedKey, pchFile, buffer.Length, sha256: null);
                    Result = true;
                    QueueRemoteUpload(pchFile, buffer);
                    Write($"FileWrite {pchFile}, {buffer.Length} bytes");
                }
                catch (Exception ex)
                {
                    Write($"FileWrite {pchFile} {ex}");
                }
            });
            return Result;
        }

        public int FileRead(string pchFile, IntPtr pvData, int cubDataToRead)
        {
            Write($"FileRead {pchFile}");
            int Result = 0;
            byte[] bytes = default;
            if (!TryGetStorageFilePath(pchFile, out var normalizedKey, out var fullPath))
            {
                Write($"FileRead {pchFile} = 0 (invalid path)");
                return 0;
            }

            // Known missing: skip all I/O immediately.
            if (RemoteStorageCache.IsKnownMissing(normalizedKey))
            {
                Write($"FileRead {pchFile} = 0 (known missing)");
                return 0;
            }

            MutexHelper.Wait("FileRead", delegate
            {
                try
                {
                    EnsureRemoteFileCached(pchFile);
                    if (File.Exists(fullPath))
                    {
                        bytes = File.ReadAllBytes(fullPath);
                        Result = Math.Min(bytes.Length, cubDataToRead);
                    }
                }
                catch (Exception ex)
                {
                    Write($"FileRead {pchFile} {ex}");
                    Result = 0;
                }
            });

            if (bytes != null && bytes.Length > 0 && pvData != IntPtr.Zero)
            {
                Marshal.Copy(bytes, 0, pvData, Result);
            }

            return Result;
        }

        public SteamAPICall_t FileWriteAsync(string pchFile, IntPtr pvData, uint cubData)
        {
            Write($"FileWriteAsync {pchFile}, {cubData} bytes");

            SteamAPICall_t APICall = k_uAPICallInvalid;
            try
            {
                if (!TryGetStorageFilePath(pchFile, out var normalizedKey, out var fullPath))
                {
                    Write($"FileWriteAsync rejected invalid path {pchFile}");
                    return APICall;
                }

                Common.EnsureDirectoryExists(fullPath, true);
                byte[] bytes = pvData.GetBytes(cubData);
                File.WriteAllBytes(fullPath, bytes);
                RemoteStorageCache.SetHydrated(normalizedKey, pchFile, bytes.Length, sha256: null);
                QueueRemoteUpload(pchFile, bytes);

                RemoteStorageFileWriteAsyncComplete_t data = new RemoteStorageFileWriteAsyncComplete_t()
                {
                    m_eResult = EResult.k_EResultOK,
                };

                APICall = CallbackManager.AddCallbackResult(data);
            }
            catch
            {
                Write($"Error writing file {pchFile}");
            }
            return APICall;
        }

        public SteamAPICall_t FileReadAsync(string pchFile, uint nOffset, uint cubToRead)
        {
            Write($"FileReadAsync {pchFile}");
            SteamAPICall_t APICall = k_uAPICallInvalid;
            MutexHelper.Wait("FileReadAsync", delegate
            {
                try
                {
                    RemoteStorageFileReadAsyncComplete_t data = new RemoteStorageFileReadAsyncComplete_t();
                    data.m_eResult = EResult.k_EResultFail;

                    if (!TryGetStorageFilePath(pchFile, out _, out var fullPath))
                    {
                        Write($"FileReadAsync rejected invalid path {pchFile}");
                        return;
                    }

                    EnsureRemoteFileCached(pchFile);
                    if (File.Exists(fullPath))
                    {
                        var info = new FileInfo(fullPath);
                        data.m_nOffset = nOffset;
                        data.m_cubRead = (uint)Math.Min((long)cubToRead, Math.Max(0, info.Length - nOffset));
                        data.m_eResult = EResult.k_EResultOK;
                        data.m_hFileReadAsync = (ulong)CSteamID.CreateOne();

                        if (!AsyncFilesRead.ContainsKey(data.m_hFileReadAsync))
                        {
                            AsyncFilesRead.Add(data.m_hFileReadAsync, fullPath);
                        }
                    }

                    APICall = CallbackManager.AddCallbackResult(data);
                }
                catch
                {
                    Write($"Error reading file {pchFile}");
                }
            });
            return APICall;
        }

        public bool FileReadAsyncComplete(ulong hReadCall, IntPtr pvBuffer, uint cubToRead)
        {
            Write("FileReadAsyncComplete");

            if (AsyncFilesRead.ContainsKey(hReadCall))
            {
                MutexHelper.Wait("FileReadAsyncComplete", delegate
                {
                    string fullPath = AsyncFilesRead[hReadCall];
                    byte[] bytes = default;
                    if (File.Exists(fullPath))
                    {
                        try
                        {
                            bytes = File.ReadAllBytes(fullPath);
                            if (pvBuffer == IntPtr.Zero)
                            {
                                return;
                            }

                            Marshal.Copy(bytes, 0, pvBuffer, Math.Min((int)cubToRead, bytes.Length));
                        }
                        catch (Exception ex)
                        {
                            Write($"FileRead {fullPath} {ex}");
                        }
                    }
                });
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool FileForget(string pchFile)
        {
            Write($"FileForget {pchFile}");
            if (!TryGetStorageFilePath(pchFile, out _, out var fullPath))
            {
                return false;
            }

            return File.Exists(fullPath);
        }

        public bool FileDelete(string pchFile)
        {
            Write($"FileDelete {pchFile}");
            if (!TryGetStorageFilePath(pchFile, out var normKey, out var fullPath))
            {
                Write($"FileDelete rejected invalid path {pchFile}");
                return false;
            }

            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            RemoteStorageCache.SetDeletedLocally(normKey, MissingRemoteFileWindow);

            if (SkyNetApiClient.IsEnabled)
            {
                WorkQueue.Enqueue("Delete remote storage file", () => SkyNetApiClient.DeleteRemoteStorageFile(pchFile),
                    "storage:delete:" + normKey);
            }

            return true;
        }

        public SteamAPICall_t FileShare(string pchFile)
        {
            Write("FileShare");
            try
            {
                if (!TryGetStorageFilePath(pchFile, out _, out var fullPath))
                {
                    Write($"FileShare rejected invalid path {pchFile}");
                    return ulong.MaxValue;
                }

                EnsureRemoteFileCached(pchFile);
                if (!File.Exists(fullPath))
                {
                    return CallbackManager.AddCallbackResult(new RemoteStorageFileShareResult_t
                    {
                        m_eResult = EResult.k_EResultFileNotFound,
                        m_rgchFilename = Encoding.Default.GetBytes(pchFile)
                    });
                }

                var pending = new RemoteStorageFileShareResult_t
                {
                    m_eResult = EResult.k_EResultOK,
                    m_hFile = (ulong)CSteamID.CreateOne(),
                    m_rgchFilename = Encoding.Default.GetBytes(pchFile)
                };

                return WorkQueue.EnqueueCallbackResult(
                    pending,
                    () =>
                    {
                        var data = pending;
                        if (SkyNetApiClient.IsEnabled)
                        {
                            var share = SkyNetApiClient.ShareRemoteStorageFile(pchFile);
                            if (share != null)
                            {
                                data.m_eResult = share.Result;
                                data.m_hFile = share.Handle;
                            }
                        }

                        SharedFiles.TryAdd(data.m_hFile, pchFile);
                        return data;
                    },
                    false,
                    "Share remote storage file",
                    "storage:share:" + NormalizeRemoteFileName(pchFile),
                    false);
            }
            catch (Exception ex)
            {
                Write($"Error Sharing file {pchFile} {ex}");
                return CallbackManager.AddCallbackResult(new RemoteStorageFileShareResult_t
                {
                    m_eResult = EResult.k_EResultFail,
                    m_rgchFilename = Encoding.Default.GetBytes(pchFile)
                });
            }
        }

        public bool SetSyncPlatforms(string pchFile, int eRemoteStoragePlatform)
        {
            Write("SetSyncPlatforms");
            var normalizedKey = NormalizeRemoteFileName(pchFile);
            if (string.IsNullOrEmpty(normalizedKey))
            {
                return false;
            }

            RemoteStorageCache.SetSyncPlatforms(normalizedKey, unchecked((uint)eRemoteStoragePlatform));
            return true;
        }

        public UGCFileWriteStreamHandle_t FileWriteStreamOpen(string pchFile)
        {
            Write("FileWriteStreamOpen");
            var handle = (ulong)CSteamID.CreateOne();
            PendingWriteStreams[handle] = new PendingWriteStream
            {
                FileName = pchFile,
                Buffer = new List<byte>()
            };
            return handle;
        }

        public bool FileWriteStreamWriteChunk(ulong writeHandle, IntPtr pvData, int cubData)
        {
            Write("FileWriteStreamWriteChunk");
            if (!PendingWriteStreams.TryGetValue(writeHandle, out var pending))
            {
                return false;
            }

            pending.Buffer.AddRange(pvData.GetBytes(cubData));
            return true;
        }

        public bool FileWriteStreamClose(ulong writeHandle)
        {
            Write("FileWriteStreamClose");
            if (!PendingWriteStreams.TryRemove(writeHandle, out var pending))
            {
                return false;
            }

            var data = pending.Buffer.ToArray();
            var ptr = Marshal.AllocHGlobal(data.Length);
            try
            {
                if (data.Length > 0)
                {
                    Marshal.Copy(data, 0, ptr, data.Length);
                }

                return FileWrite(pending.FileName, ptr, data.Length);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public bool FileWriteStreamCancel(ulong writeHandle)
        {
            Write("FileWriteStreamCancel");
            return PendingWriteStreams.TryRemove(writeHandle, out _);
        }

        public bool FileExists(string pchFile)
        {
            if (!TryGetStorageFilePath(pchFile, out var normalizedKey, out var fullPath))
            {
                Write($"FileExists {pchFile} = False (invalid path)");
                return false;
            }

            // [1] Local disk is authoritative.
            if (File.Exists(fullPath))
            {
                RemoteStorageCache.SetHydrated(normalizedKey, pchFile, (int)new FileInfo(fullPath).Length, sha256: null);
                Write($"FileExists {pchFile} = True (local)");
                return true;
            }

            // [2] TTL not expired; skip network.
            if (RemoteStorageCache.IsKnownMissing(normalizedKey))
            {
                Write($"FileExists {pchFile} = False (known missing)");
                return false;
            }

            if (SkyNetApiClient.IsEnabled)
            {
                var entry = RemoteStorageCache.GetEntry(normalizedKey);

                // [3] Manifest says server has it but we do not have it locally; queue fetch.
                if (entry != null && entry.IsPersisted && !entry.IsHydrated)
                {
                    QueueRemoteFileFetch(pchFile, fullPath, normalizedKey);
                    Write($"FileExists {pchFile} = False (fetch queued)");
                    return false;
                }

                // [4] Manifest fresh: confirmed absent.
                if (RemoteStorageCache.ManifestState == RemoteStorageCache.ManifestStateEnum.Fresh)
                {
                    RemoteStorageCache.MarkMissing(normalizedKey, MissingRemoteFileWindow);
                    Write($"FileExists {pchFile} = False (manifest fresh, not found)");
                    return false;
                }

                // [5] Manifest unknown/failed: queue fetch + short TTL to avoid spam.
                QueueRemoteFileFetch(pchFile, fullPath, normalizedKey);
                RemoteStorageCache.MarkMissing(normalizedKey, MissingRemoteFileWindowShort);
            }

            Write($"FileExists {pchFile} = False");
            return false;
        }

        public bool FilePersisted(string pchFile)
        {
            Write("FilePersisted");
            if (!TryGetStorageFilePath(pchFile, out var normalizedKey, out var fullPath))
            {
                return false;
            }

            if (!SkyNetApiClient.IsEnabled)
            {
                return File.Exists(fullPath);
            }

            var entry = RemoteStorageCache.GetEntry(normalizedKey);
            return entry != null && entry.IsPersisted;
        }

        public int GetFileSize(string pchFile)
        {
            int Length = 0;

            if (!TryGetStorageFilePath(pchFile, out _, out var fullPath))
            {
                Write($"GetFileSize {pchFile}, 0 bytes (invalid path)");
                return 0;
            }

            EnsureRemoteFileCached(pchFile);
            if (File.Exists(fullPath))
            {
                FileInfo info = new FileInfo(fullPath);
                Length = (int)info.Length;
            }

            Write($"GetFileSize {pchFile}, {Length} bytes");

            return Length;
        }

        public uint GetFileTimestamp(string pchFile)
        {
            Write("GetFileTimestamp");
            if (!TryGetStorageFilePath(pchFile, out var normalizedKey, out var fullPath))
            {
                return 0;
            }

            if (File.Exists(fullPath))
            {
                return (uint)((DateTimeOffset)new FileInfo(fullPath).LastWriteTimeUtc).ToUnixTimeSeconds();
            }

            var entry = RemoteStorageCache.GetEntry(normalizedKey);
            return entry?.Timestamp ?? 0;
        }

        public int GetSyncPlatforms(string pchFile)
        {
            Write("GetSyncPlatforms");
            var normalizedKey = NormalizeRemoteFileName(pchFile);
            if (string.IsNullOrEmpty(normalizedKey))
            {
                return 0;
            }

            var entry = RemoteStorageCache.GetEntry(normalizedKey);
            if (entry != null && entry.HasSyncPlatforms)
            {
                return unchecked((int)entry.SyncPlatforms);
            }

            return unchecked((int)0xffffffff);
        }

        public int GetFileCount()
        {
            if (SkyNetApiClient.IsEnabled)
            {
                QueueRemoteFileList();
                RefreshLocalFileList();
                Write($"GetFileCount {StorageFiles.Count}");
                return StorageFiles.Count;
            }

            RefreshLocalFileList();
            Write($"GetFileCount {StorageFiles.Count}");
            return StorageFiles.Count;
        }

        public string GetFileNameAndSize(int iFile, ref int pnFileSizeInBytes)
        {
            if (SkyNetApiClient.IsEnabled)
            {
                if (RemoteStorageFiles == null || RemoteStorageFiles.Count == 0)
                {
                    QueueRemoteFileList();
                }

                if (iFile >= 0 && iFile < StorageFiles.Count)
                {
                    string filename = StorageFiles[iFile];
                    pnFileSizeInBytes = File.Exists(filename) ? (int)new FileInfo(filename).Length : 0;
                    return ToRemoteStorageName(filename);
                }

                if (RemoteStorageFiles == null || iFile < 0 || iFile >= RemoteStorageFiles.Count)
                {
                    return string.Empty;
                }

                var file = RemoteStorageFiles[iFile];
                pnFileSizeInBytes = file.Size;
                return file.FileName ?? string.Empty;
            }

            if (StorageFiles.Count == 0)
            {
                Write("GetFileNameAndSize");
                return "";
            }

            if (LastFile < StorageFiles.Count)
            {
                string filename = StorageFiles[LastFile];
                pnFileSizeInBytes = (int)new FileInfo(filename).Length;

                if (LastFile == (StorageFiles.Count - 1))
                {
                    LastFile = 0;
                }
                else
                {
                    LastFile++;
                }

                filename = filename.Replace(StoragePath + @"\", "");

                Write($"GetFileNameAndSize {filename}, {pnFileSizeInBytes} bytes");

                return filename;
            }

            return "";
        }

        public bool GetQuota(ref ulong pnTotalBytes, ref ulong puAvailableBytes)
        {
            Write("GetQuota");
            pnTotalBytes = CachedQuotaTotalBytes;
            puAvailableBytes = CachedQuotaAvailableBytes;

            if (SkyNetApiClient.IsEnabled)
            {
                WorkQueue.Enqueue("Refresh remote storage quota", () =>
                    {
                        var quota = SkyNetApiClient.GetRemoteStorageQuota();
                        if (quota != null)
                        {
                            CachedQuotaTotalBytes = quota.TotalBytes;
                            CachedQuotaAvailableBytes = quota.AvailableBytes;
                        }
                    },
                    "storage:quota");
            }

            return true;
        }

        public bool IsCloudEnabledForAccount()
        {
            Write("IsCloudEnabledForAccount");
            return true;
        }

        public bool IsCloudEnabledForApp()
        {
            Write("IsCloudEnabledForApp");
            return true;
        }

        public void SetCloudEnabledForApp(bool bEnabled)
        {
            Write("SetCloudEnabledForApp");
        }

        public SteamAPICall_t UGCDownload(UGCHandle_t hContent, uint unPriority)
        {
            Write("UGCDownload");
            return 0;
        }

        public bool GetUGCDownloadProgress(UGCHandle_t hContent, int pnBytesDownloaded, int pnBytesExpected)
        {
            Write("GetUGCDownloadProgress");
            return false;
        }

        public bool GetUGCDownloadProgress(UGCHandle_t hContent, IntPtr pnBytesDownloaded, IntPtr pnBytesExpected)
        {
            Write("GetUGCDownloadProgress");
            if (pnBytesDownloaded != IntPtr.Zero)
            {
                Marshal.WriteInt32(pnBytesDownloaded, 0);
            }
            if (pnBytesExpected != IntPtr.Zero)
            {
                Marshal.WriteInt32(pnBytesExpected, 0);
            }
            return false;
        }

        public bool GetUGCDetails(UGCHandle_t hContent, uint pnAppID, string ppchName, int pnFileSizeInBytes, ulong pSteamIDOwner)
        {
            Write("GetUGCDetails");
            return false;
        }

        public bool GetUGCDetails(UGCHandle_t hContent, IntPtr pnAppID, IntPtr ppchName, IntPtr pnFileSizeInBytes, IntPtr pSteamIDOwner)
        {
            Write("GetUGCDetails");
            if (pnAppID != IntPtr.Zero)
            {
                Marshal.WriteInt32(pnAppID, unchecked((int)SteamEmulator.AppID));
            }
            if (ppchName != IntPtr.Zero)
            {
                Marshal.WriteIntPtr(ppchName, NativeStringCache.ToUtf8Ptr(string.Empty));
            }
            if (pnFileSizeInBytes != IntPtr.Zero)
            {
                Marshal.WriteInt32(pnFileSizeInBytes, 0);
            }
            if (pSteamIDOwner != IntPtr.Zero)
            {
                Marshal.WriteInt64(pSteamIDOwner, unchecked((long)(ulong)SteamEmulator.SteamID));
            }
            return false;
        }

        public int UGCRead(UGCHandle_t hContent, IntPtr pvData, int cubDataToRead, uint cOffset, int eAction)
        {
            Write("UGCRead");
            return 0;
        }

        public int GetCachedUGCCount()
        {
            Write("GetCachedUGCCount");
            return 0;
        }

        public UGCHandle_t GetCachedUGCHandle(int iCachedContent)
        {
            Write("GetCachedUGCHandle");
            return 0;
        }

        public SteamAPICall_t PublishWorkshopFile(string pchFile, string pchPreviewFile, uint nConsumerAppId, string pchTitle, string pchDescription, int eVisibility, IntPtr pTags, int int2)
        {
            Write("PublishWorkshopFile");
            return 0;
        }

        public ulong CreatePublishedFileUpdateRequest(ulong unPublishedFileId)
        {
            Write("CreatePublishedFileUpdateRequest");
            return 0;
        }

        public bool UpdatePublishedFileFile(PublishedFileUpdateHandle_t updateHandle, string pchFile)
        {
            Write("UpdatePublishedFileFile");
            return false;
        }

        public bool UpdatePublishedFilePreviewFile(PublishedFileUpdateHandle_t updateHandle, string pchPreviewFile)
        {
            Write("UpdatePublishedFilePreviewFile");
            return false;
        }

        public void GetFileListFromServer()
        {
            Write("GetFileListFromServer");
        }

        public bool FileFetch(string pchFile)
        {
            Write("FileFetch");
            return false;
        }

        public bool FilePersist(string pchFile)
        {
            Write("FilePersist");
            return false;
        }

        public bool SynchronizeToClient()
        {
            Write("SynchronizeToClient");
            return false;
        }

        public bool SynchronizeToServer()
        {
            Write("SynchronizeToServer");
            return false;
        }

        public bool ResetFileRequestState()
        {
            Write("ResetFileRequestState");
            return false;
        }

        public bool UpdatePublishedFileTitle(PublishedFileUpdateHandle_t updateHandle, string pchTitle)
        {
            Write("UpdatePublishedFileTitle");
            return false;
        }

        public bool UpdatePublishedFileDescription(PublishedFileUpdateHandle_t updateHandle, string pchDescription)
        {
            Write("UpdatePublishedFileDescription");
            return false;
        }

        public bool UpdatePublishedFileVisibility(PublishedFileUpdateHandle_t updateHandle, int eVisibility)
        {
            Write("UpdatePublishedFileVisibility");
            return false;
        }

        public bool UpdatePublishedFileTags(PublishedFileUpdateHandle_t updateHandle, IntPtr pTags)
        {
            Write("UpdatePublishedFileTags");
            return false;
        }

        public SteamAPICall_t CommitPublishedFileUpdate(PublishedFileUpdateHandle_t updateHandle)
        {
            Write("CommitPublishedFileUpdate");
            return 0;
        }

        public SteamAPICall_t GetPublishedFileDetails(ulong unPublishedFileId, uint unMaxSecondsOld)
        {
            Write("GetPublishedFileDetails");
            return 0;
        }

        public SteamAPICall_t DeletePublishedFile(ulong unPublishedFileId)
        {
            Write("DeletePublishedFile");
            return 0;
        }

        public SteamAPICall_t EnumerateUserPublishedFiles(uint unStartIndex)
        {
            Write("EnumerateUserPublishedFiles");
            SteamAPICall_t APICall = k_uAPICallInvalid;
            MutexHelper.Wait("EnumerateUserPublishedFiles", delegate
            {
                RemoteStorageEnumerateUserPublishedFilesResult_t data = new RemoteStorageEnumerateUserPublishedFilesResult_t();
                data.m_eResult = EResult.k_EResultOK;
                data.m_nResultsReturned = 0;
                data.m_nTotalResultCount = 0;
                APICall = CallbackManager.AddCallbackResult(data);
            });

            return APICall;
        }

        public SteamAPICall_t SubscribePublishedFile(ulong unPublishedFileId)
        {
            Write("SubscribePublishedFile");
            return 0;
        }

        public SteamAPICall_t EnumerateUserSubscribedFiles(uint unStartIndex)
        {
            Write("EnumerateUserSubscribedFiles");
            return 0;
        }

        public SteamAPICall_t UnsubscribePublishedFile(ulong unPublishedFileId)
        {
            Write("UnsubscribePublishedFile");
            return 0;
        }

        public bool UpdatePublishedFileSetChangeDescription(PublishedFileUpdateHandle_t updateHandle, string pchChangeDescription)
        {
            Write("UpdatePublishedFileSetChangeDescription");
            return false;
        }

        public SteamAPICall_t GetPublishedItemVoteDetails(ulong unPublishedFileId)
        {
            Write("GetPublishedItemVoteDetails");
            return 0;
        }

        public SteamAPICall_t UpdateUserPublishedItemVote(ulong unPublishedFileId, bool bVoteUp)
        {
            Write("UpdateUserPublishedItemVote");
            return 0;
        }

        public SteamAPICall_t GetUserPublishedItemVoteDetails(ulong unPublishedFileId)
        {
            Write("GetUserPublishedItemVoteDetails");
            return 0;
        }

        public SteamAPICall_t EnumerateUserSharedWorkshopFiles(ulong steamId, uint unStartIndex, IntPtr pRequiredTags, IntPtr pExcludedTags)
        {
            Write("EnumerateUserSharedWorkshopFiles");
            SteamAPICall_t APICall = k_uAPICallInvalid;
            MutexHelper.Wait("EnumerateUserSharedWorkshopFiles", delegate
            {
                RemoteStorageEnumerateUserPublishedFilesResult_t data = new RemoteStorageEnumerateUserPublishedFilesResult_t();
                data.m_eResult = EResult.k_EResultOK;
                data.m_nResultsReturned = 0;
                data.m_nTotalResultCount = 0;
                APICall = CallbackManager.AddCallbackResult(data);
            });
            return APICall;
        }

        public SteamAPICall_t PublishVideo(int eVideoProvider, string pchVideoAccount, string pchVideoIdentifier, string pchPreviewFile, uint nConsumerAppId, string pchTitle, string pchDescription, int eVisibility, IntPtr pTags)
        {
            Write("PublishVideo");
            return 0;
        }

        public SteamAPICall_t SetUserPublishedFileAction(ulong unPublishedFileId, int eAction)
        {
            Write("SetUserPublishedFileAction");
            return 0;
        }

        public SteamAPICall_t EnumeratePublishedFilesByUserAction(int eAction, uint unStartIndex)
        {
            Write("EnumeratePublishedFilesByUserAction");
            return 0;
        }

        public SteamAPICall_t EnumeratePublishedWorkshopFiles(int eEnumerationType, uint unStartIndex, uint unCount, uint unDays, IntPtr pTags, IntPtr pUserTags)
        {
            Write("EnumeratePublishedWorkshopFiles");
            return 0;
        }

        public SteamAPICall_t UGCDownloadToLocation(ulong hContent, string pchLocation, uint unPriority)
        {
            Write("UGCDownloadToLocation");
            return 0;
        }

        public int GetLocalFileChangeCount()
        {
            Write("GetLocalFileChangeCount");
            return 0;
        }

        public string GetLocalFileChange(int iFile, int pEChangeType, int pEFilePathType)
        {
            Write("GetLocalFileChange");
            return "";
        }

        public IntPtr GetLocalFileChange(int iFile, IntPtr pEChangeType, IntPtr pEFilePathType)
        {
            Write("GetLocalFileChange");
            if (pEChangeType != IntPtr.Zero)
            {
                Marshal.WriteInt32(pEChangeType, 0);
            }
            if (pEFilePathType != IntPtr.Zero)
            {
                Marshal.WriteInt32(pEFilePathType, 0);
            }
            return NativeStringCache.ToUtf8Ptr(string.Empty);
        }

        public bool BeginFileWriteBatch()
        {
            Write("BeginFileWriteBatch");
            return true;
        }

        public bool EndFileWriteBatch()
        {
            Write("EndFileWriteBatch");
            return true;
        }

        private bool EnsureRemoteFileCached(string pchFile)
        {
            if (!TryGetStorageFilePath(pchFile, out var remoteKey, out var fullPath))
            {
                return false;
            }

            if (File.Exists(fullPath) || !SkyNetApiClient.IsEnabled)
            {
                return File.Exists(fullPath);
            }

            if (RemoteStorageCache.IsKnownMissing(remoteKey))
            {
                return false;
            }

            var entry = RemoteStorageCache.GetEntry(remoteKey);
            if (entry != null && entry.IsPersisted && !entry.IsHydrated)
            {
                QueueRemoteFileFetch(pchFile, fullPath, remoteKey);
                return false;
            }

            if (RemoteStorageCache.ManifestState == RemoteStorageCache.ManifestStateEnum.Fresh)
            {
                RemoteStorageCache.MarkMissing(remoteKey, MissingRemoteFileWindow);
                return false;
            }

            QueueRemoteFileFetch(pchFile, fullPath, remoteKey);
            RemoteStorageCache.MarkMissing(remoteKey, MissingRemoteFileWindowShort);
            return false;
        }

        private void QueueRemoteUpload(string fileName, byte[] content)
        {
            if (!SkyNetApiClient.IsEnabled)
            {
                return;
            }

            var normalizedKey = NormalizeRemoteFileName(fileName);
            var entry = RemoteStorageCache.GetEntry(normalizedKey);
            uint? syncPlatforms = entry != null && entry.HasSyncPlatforms
                ? entry.SyncPlatforms
                : (uint?)null;
            var copy = content == null ? new byte[0] : (byte[])content.Clone();
            WorkQueue.Enqueue("RemoteStorage upload", () =>
            {
                if (SkyNetApiClient.UploadRemoteStorageFile(fileName, copy, syncPlatforms))
                {
                    RemoteStorageCache.SetPersisted(normalizedKey);
                }
                else
                {
                    Write($"Remote upload failed {fileName}");
                }
            }, "remote-storage:upload:" + normalizedKey);
        }

        private void QueueRemoteFileList()
        {
            if (!SkyNetApiClient.IsEnabled)
            {
                return;
            }

            WorkQueue.Enqueue("RemoteStorage list", () =>
            {
                var files = SkyNetApiClient.ListRemoteStorageFiles();
                RemoteStorageCache.MergeRemoteList(files);
                if (files != null)
                {
                    RemoteStorageFiles = files;
                }
            }, "remote-storage:list");
        }

        private void QueueRemoteFileFetch(string fileName, string fullPath, string remoteKey)
        {
            WorkQueue.Enqueue("RemoteStorage fetch", () =>
            {
                try
                {
                    System.Net.HttpStatusCode? statusCode;
                    var file = SkyNetApiClient.DownloadRemoteStorageFile(fileName, out statusCode);

                    if (statusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // Server confirmed 404. Use a TTL, not a permanent miss.
                        RemoteStorageCache.MarkMissing(remoteKey, MissingRemoteFileWindow);
                        Write($"Remote fetch: file not found on server {fileName}");
                        return;
                    }

                    if (file == null || string.IsNullOrWhiteSpace(file.ContentBase64))
                    {
                        // Transient error. Retry after the window.
                        RemoteStorageCache.MarkMissing(remoteKey, MissingRemoteFileWindow);
                        return;
                    }

                    Common.EnsureDirectoryExists(fullPath, true);
                    File.WriteAllBytes(fullPath, Convert.FromBase64String(file.ContentBase64));
                    RemoteStorageCache.SetHydrated(remoteKey, fileName, file.Size, file.Sha256);
                }
                catch (Exception ex)
                {
                    RemoteStorageCache.MarkMissing(remoteKey, MissingRemoteFileWindow);
                    Write($"Remote fetch failed {fileName} {ex.Message}");
                }
            }, "remote-storage:fetch:" + remoteKey);
        }

        private void RefreshLocalFileList()
        {
            if (Directory.Exists(StoragePath))
            {
                StorageFiles = Directory.GetFiles(StoragePath, "*.*", SearchOption.AllDirectories).ToList();
                LastFile = 0;
            }
            else
            {
                StorageFiles = new List<string>();
            }
        }

        private string ToRemoteStorageName(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return string.Empty;
            }

            var root = Path.GetFullPath(StoragePath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var path = Path.GetFullPath(fullPath);
            return path.StartsWith(root, StringComparison.OrdinalIgnoreCase)
                ? path.Substring(root.Length).Replace('\\', '/')
                : Path.GetFileName(path);
        }

        private static string NormalizeRemoteFileName(string pchFile)
        {
            return RemoteStorageCache.Normalize(pchFile);
        }

        private bool TryGetStorageFilePath(string pchFile, out string normalizedKey, out string fullPath)
        {
            normalizedKey = NormalizeRemoteFileName(pchFile);
            fullPath = null;
            if (string.IsNullOrEmpty(normalizedKey))
            {
                return false;
            }

            var root = Path.GetFullPath(StoragePath).TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

            var relative = (pchFile ?? string.Empty).Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
            var candidate = Path.GetFullPath(Path.Combine(root, relative));
            if (!candidate.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            fullPath = candidate;
            return true;
        }

        public void StoreAvatar(Bitmap avatar, uint accountID)
        {
            try
            {
                Common.EnsureDirectoryExists(AvatarCachePath);
                avatar.Save(Path.Combine(AvatarCachePath, accountID + ".jpg"));
            }
            catch
            {
            }
        }

        private sealed class PendingWriteStream
        {
            public string FileName { get; set; }
            public List<byte> Buffer { get; set; }
        }
    }
}


