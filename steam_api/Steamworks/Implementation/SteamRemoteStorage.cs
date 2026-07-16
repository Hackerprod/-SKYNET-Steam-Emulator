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
        private List<APIClient.ApiRemoteStorageFileListItem> RemoteStorageFiles;
        private ConcurrentDictionary<ulong, string> SharedFiles;
        private int LastFile;
        private ConcurrentDictionary<ulong, AsyncReadState> AsyncFilesRead;
        private ConcurrentDictionary<ulong, PendingWriteStream> PendingWriteStreams;
        private static readonly TimeSpan MissingRemoteFileWindow = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan MissingRemoteFileWindowShort = TimeSpan.FromSeconds(30);
        private ulong CachedQuotaTotalBytes = 1024UL * 1024UL * 1024UL;
        private ulong CachedQuotaAvailableBytes = 1024UL * 1024UL * 1024UL;

        // Steam limits: k_cchFilenameMax and the 100 MiB per-write ceiling.
        private const int MaxFilenameLength = 260;
        private const int MaxFileSize = 100 * 1024 * 1024;
        // k_ERemoteStoragePlatformAll
        private const int SyncPlatformAll = unchecked((int)0xffffffff);

        // Files whose latest local version has been queued for cloud upload but
        // not yet confirmed persisted. FilePersisted reports false while queued.
        private readonly ConcurrentDictionary<string, byte> PendingUploads =
            new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, byte> PendingDeletes =
            new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);

        // In-memory write batching. Between Begin/EndFileWriteBatch, writes and
        // deletes are only recorded here; the actual cloud sync flushes on End.
        private bool IsBatchActive;
        private readonly object BatchLock = new object();
        private readonly HashSet<string> BatchUploads = new HashSet<string>(StringComparer.Ordinal);
        private readonly HashSet<string> BatchDeletes = new HashSet<string>(StringComparer.Ordinal);

        // At most one in-flight async read per file. FileReadAsync rejects a second
        // read of the same file until its FileReadAsyncComplete clears the entry.
        private readonly ConcurrentDictionary<string, byte> ActiveAsyncReads =
            new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);

        private sealed class AsyncReadState
        {
            public string NormalizedName;
            public uint Offset;
            public byte[] Bytes;
            public EResult Result;
        }

        public SteamRemoteStorage()
        {
            Instance = this;
            InterfaceName = "SteamRemoteStorage";
            InterfaceVersion = "STEAMREMOTESTORAGE_INTERFACE_VERSION016";
            StoragePath = Path.Combine(Common.GetPath(), "SKYNET", "Storage", "Remote");
            AvatarCachePath = Path.Combine(Common.GetPath(), "SKYNET", "Images", "AvatarCache");
            StorageFiles = new List<string>();
            RemoteStorageFiles = new List<APIClient.ApiRemoteStorageFileListItem>();
            SharedFiles = new ConcurrentDictionary<ulong, string>();
            AsyncFilesRead = new ConcurrentDictionary<ulong, AsyncReadState>();
            PendingWriteStreams = new ConcurrentDictionary<ulong, PendingWriteStream>();
            LastFile = 0;

            // Populate cache from disk immediately (no network, no blocking).
            RemoteStorageCache.Initialize(StoragePath);

            // Fetch server manifest in the background with a tight timeout.
            if (APIClient.IsEnabled)
            {
                WorkQueue.Enqueue(
                    "RemoteStorage manifest-init",
                    () =>
                    {
                        var files = APIClient.ListRemoteStorageFiles(timeoutMs: 1500);
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
                    if (!ValidateFileArgs(pchFile, cubData, pvData, checkBuffer: true) || cubData <= 0)
                    {
                        Write($"FileWrite rejected invalid args {pchFile} ({cubData} bytes)");
                        return;
                    }

                    if (!TryGetStorageFilePath(pchFile, out var normalizedKey, out var fullPath))
                    {
                        Write($"FileWrite rejected invalid path {pchFile}");
                        return;
                    }

                    byte[] buffer = pvData.GetBytes(cubData);

                    if (!TryReserveQuota(fullPath, buffer.Length))
                    {
                        Write($"FileWrite {pchFile} rejected: quota exceeded ({buffer.Length} bytes, {CachedQuotaAvailableBytes} available)");
                        return;
                    }

                    Common.EnsureDirectoryExists(fullPath, true);
                    if (!AtomicWrite(fullPath, buffer))
                    {
                        return;
                    }

                    // Update cache immediately on the game thread; do not wait for WorkQueue.
                    RemoteStorageCache.SetHydrated(normalizedKey, pchFile, buffer.Length, sha256: null);
                    Result = true;
                    QueueOrBatchUpload(normalizedKey, pchFile, buffer);
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
                if (!ValidateFileArgs(pchFile, checked((int)cubData), pvData, checkBuffer: true) || cubData == 0)
                {
                    Write($"FileWriteAsync rejected invalid args {pchFile} ({cubData} bytes)");
                    return CallbackManager.AddCallbackResult(new RemoteStorageFileWriteAsyncComplete_t
                    {
                        m_eResult = EResult.k_EResultInvalidParam,
                    });
                }

                if (!TryGetStorageFilePath(pchFile, out var normalizedKey, out var fullPath))
                {
                    Write($"FileWriteAsync rejected invalid path {pchFile}");
                    return CallbackManager.AddCallbackResult(new RemoteStorageFileWriteAsyncComplete_t
                    {
                        m_eResult = EResult.k_EResultInvalidParam,
                    });
                }

                byte[] bytes = pvData.GetBytes(cubData);

                if (!TryReserveQuota(fullPath, bytes.Length))
                {
                    Write($"FileWriteAsync {pchFile} rejected: quota exceeded");
                    return CallbackManager.AddCallbackResult(new RemoteStorageFileWriteAsyncComplete_t
                    {
                        m_eResult = EResult.k_EResultLimitExceeded,
                    });
                }

                Common.EnsureDirectoryExists(fullPath, true);
                if (!AtomicWrite(fullPath, bytes))
                {
                    return CallbackManager.AddCallbackResult(new RemoteStorageFileWriteAsyncComplete_t
                    {
                        m_eResult = EResult.k_EResultIOFailure,
                    });
                }

                RemoteStorageCache.SetHydrated(normalizedKey, pchFile, bytes.Length, sha256: null);
                QueueOrBatchUpload(normalizedKey, pchFile, bytes);

                APICall = CallbackManager.AddCallbackResult(new RemoteStorageFileWriteAsyncComplete_t
                {
                    m_eResult = EResult.k_EResultOK,
                });
            }
            catch (Exception ex)
            {
                Write($"Error writing file {pchFile} {ex}");
                APICall = CallbackManager.AddCallbackResult(new RemoteStorageFileWriteAsyncComplete_t
                {
                    m_eResult = EResult.k_EResultIOFailure,
                });
            }
            return APICall;
        }

        public SteamAPICall_t FileReadAsync(string pchFile, uint nOffset, uint cubToRead)
        {
            Write($"FileReadAsync {pchFile}, offset {nOffset}, {cubToRead} bytes");
            SteamAPICall_t APICall = k_uAPICallInvalid;
            MutexHelper.Wait("FileReadAsync", delegate
            {
                string reserved = null;
                try
                {
                    // [1] Zero-length reads are invalid.
                    if (cubToRead == 0)
                    {
                        Write($"FileReadAsync {pchFile} = invalid (cubToRead == 0)");
                        return;
                    }

                    if (!ValidateFileArgs(pchFile, 0, IntPtr.Zero, checkBuffer: false) ||
                        cubToRead > int.MaxValue)
                    {
                        Write($"FileReadAsync {pchFile} = invalid args");
                        return;
                    }

                    // [2] Path must be valid.
                    if (!TryGetStorageFilePath(pchFile, out var normalizedKey, out var fullPath))
                    {
                        Write($"FileReadAsync rejected invalid path {pchFile}");
                        return;
                    }

                    // [3] Only one async read per file may be in flight.
                    if (!ActiveAsyncReads.TryAdd(normalizedKey, 0))
                    {
                        Write($"FileReadAsync {pchFile} = invalid (read already in progress)");
                        return;
                    }
                    reserved = normalizedKey;

                    // [4] File must exist locally (pull it down first if the cache knows it).
                    EnsureRemoteFileCached(pchFile);
                    if (!File.Exists(fullPath))
                    {
                        Write($"FileReadAsync {pchFile} = invalid (file not found)");
                        return;
                    }

                    long fileSize = new FileInfo(fullPath).Length;

                    // [5] Offset and range must lie inside the file.
                    if (nOffset >= fileSize || cubToRead > fileSize - nOffset)
                    {
                        Write($"FileReadAsync {pchFile} = invalid (offset {nOffset} + {cubToRead} > size {fileSize})");
                        return;
                    }

                    // Eager read of exactly the requested slice.
                    var slice = new byte[(int)cubToRead];
                    int total = 0;
                    using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, System.IO.FileShare.Read))
                    {
                        fs.Seek(nOffset, SeekOrigin.Begin);
                        while (total < slice.Length)
                        {
                            int read = fs.Read(slice, total, slice.Length - total);
                            if (read <= 0) break;
                            total += read;
                        }
                    }

                    if (total != slice.Length)
                    {
                        Write($"FileReadAsync {pchFile} = invalid (short read {total}/{slice.Length})");
                        return;
                    }

                    var handle = (ulong)CSteamID.CreateOne();
                    AsyncFilesRead[handle] = new AsyncReadState
                    {
                        NormalizedName = normalizedKey,
                        Offset = nOffset,
                        Bytes = slice,
                        Result = EResult.k_EResultOK,
                    };

                    APICall = CallbackManager.AddCallbackResult(new RemoteStorageFileReadAsyncComplete_t
                    {
                        m_hFileReadAsync = handle,
                        m_eResult = EResult.k_EResultOK,
                        m_nOffset = nOffset,
                        m_cubRead = cubToRead,
                    });

                    // Ownership of the active-read reservation passed to the handle;
                    // FileReadAsyncComplete clears it. Do not release it in finally.
                    reserved = null;
                }
                catch (Exception ex)
                {
                    Write($"FileReadAsync {pchFile} {ex}");
                }
                finally
                {
                    // Release the reservation on any early-return failure path.
                    if (reserved != null)
                    {
                        ActiveAsyncReads.TryRemove(reserved, out _);
                    }
                }
            });
            return APICall;
        }

        public bool FileReadAsyncComplete(ulong hReadCall, IntPtr pvBuffer, uint cubToRead)
        {
            Write("FileReadAsyncComplete");

            // Consume the handle exactly once: a completed async read is invalid
            // to re-complete, and leaving it mapped would leak.
            if (!AsyncFilesRead.TryRemove(hReadCall, out var state))
            {
                return false;
            }

            // Clear the per-file active-read guard now that this read is finishing,
            // whatever the buffer outcome.
            ActiveAsyncReads.TryRemove(state.NormalizedName, out _);

            // A null destination is a caller error; the handle is still consumed.
            if (pvBuffer == IntPtr.Zero)
            {
                return false;
            }

            bool ok = false;
            MutexHelper.Wait("FileReadAsyncComplete", delegate
            {
                try
                {
                    var bytes = state.Bytes ?? Array.Empty<byte>();
                    int count = (int)Math.Min(cubToRead, (uint)bytes.Length);
                    if (count > 0)
                    {
                        Marshal.Copy(bytes, 0, pvBuffer, count);
                    }
                    ok = true;
                }
                catch (Exception ex)
                {
                    Write($"FileReadAsyncComplete {state.NormalizedName} {ex}");
                }
            });
            return ok;
        }

        public bool FileForget(string pchFile)
        {
            Write($"FileForget {pchFile}");
            if (!ValidateFileArgs(pchFile, 0, IntPtr.Zero, checkBuffer: false))
            {
                return false;
            }

            if (!TryGetStorageFilePath(pchFile, out var normKey, out var fullPath))
            {
                return false;
            }

            var entry = RemoteStorageCache.GetEntry(normKey);
            bool existsLocal = File.Exists(fullPath);
            bool existsRemote = entry != null && entry.IsPersisted;
            if (!existsLocal && !existsRemote)
            {
                return false;
            }

            // Forget = stop syncing with the cloud, but keep the local file.
            RemoteStorageCache.SetForgotten(normKey);
            PendingUploads.TryRemove(normKey, out _);

            if (APIClient.IsEnabled && existsRemote)
            {
                WorkQueue.Enqueue("Forget remote storage file", () => APIClient.DeleteRemoteStorageFile(pchFile),
                    "storage:forget:" + normKey);
            }

            return true;
        }

        public bool FileDelete(string pchFile)
        {
            Write($"FileDelete {pchFile}");
            if (!ValidateFileArgs(pchFile, 0, IntPtr.Zero, checkBuffer: false))
            {
                return false;
            }

            if (!TryGetStorageFilePath(pchFile, out var normKey, out var fullPath))
            {
                Write($"FileDelete rejected invalid path {pchFile}");
                return false;
            }

            var entry = RemoteStorageCache.GetEntry(normKey);
            bool existsLocal = File.Exists(fullPath);
            bool existsRemote = entry != null && entry.IsPersisted;

            // Steam FileDelete returns false when the file did not exist at all.
            if (!existsLocal && !existsRemote)
            {
                Write($"FileDelete {pchFile} = false (not found)");
                return false;
            }

            if (existsLocal)
            {
                long freed = 0;
                try
                {
                    freed = new FileInfo(fullPath).Length;
                    File.Delete(fullPath);
                }
                catch (Exception ex)
                {
                    Write($"FileDelete {pchFile} {ex}");
                    return false;
                }
                ReleaseQuota(freed);
            }

            RemoteStorageCache.SetDeletedLocally(normKey, MissingRemoteFileWindow);

            lock (BatchLock)
            {
                if (IsBatchActive)
                {
                    BatchDeletes.Add(normKey);
                    BatchUploads.Remove(normKey);
                    return true;
                }
            }

            QueueRemoteDelete(normKey, pchFile);
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
                        if (APIClient.IsEnabled)
                        {
                            var share = APIClient.ShareRemoteStorageFile(pchFile);
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

            if (APIClient.IsEnabled)
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

            if (!APIClient.IsEnabled)
            {
                return File.Exists(fullPath);
            }

            // A queued upload or delete means the cloud copy is not up to date.
            if (PendingUploads.ContainsKey(normalizedKey) || PendingDeletes.ContainsKey(normalizedKey))
            {
                return false;
            }

            lock (BatchLock)
            {
                if (BatchUploads.Contains(normalizedKey) || BatchDeletes.Contains(normalizedKey))
                {
                    return false;
                }
            }

            var entry = RemoteStorageCache.GetEntry(normalizedKey);
            return File.Exists(fullPath) && entry != null && entry.IsPersisted;
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

            // Default: k_ERemoteStoragePlatformAll.
            return SyncPlatformAll;
        }

        public int GetFileCount()
        {
            if (APIClient.IsEnabled)
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
            if (APIClient.IsEnabled)
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

            if (APIClient.IsEnabled)
            {
                WorkQueue.Enqueue("Refresh remote storage quota", () =>
                    {
                        var quota = APIClient.GetRemoteStorageQuota();
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
            lock (BatchLock)
            {
                if (IsBatchActive)
                {
                    Write("BeginFileWriteBatch = false (batch already active)");
                    return false;
                }

                IsBatchActive = true;
                BatchUploads.Clear();
                BatchDeletes.Clear();
            }
            return true;
        }

        public bool EndFileWriteBatch()
        {
            Write("EndFileWriteBatch");
            List<string> uploads;
            List<string> deletes;
            lock (BatchLock)
            {
                if (!IsBatchActive)
                {
                    Write("EndFileWriteBatch = false (no active batch)");
                    return false;
                }

                uploads = BatchUploads.ToList();
                deletes = BatchDeletes.ToList();
                IsBatchActive = false;
                BatchUploads.Clear();
                BatchDeletes.Clear();
            }

            foreach (var normKey in deletes)
            {
                var original = RemoteStorageCache.GetEntry(normKey)?.OriginalName ?? normKey;
                QueueRemoteDelete(normKey, original);
            }

            foreach (var normKey in uploads)
            {
                if (!TryReadLocalByKey(normKey, out var originalName, out var bytes))
                {
                    continue;
                }

                QueueRemoteUpload(normKey, originalName, bytes);
            }

            return true;
        }

        private bool EnsureRemoteFileCached(string pchFile)
        {
            if (!TryGetStorageFilePath(pchFile, out var remoteKey, out var fullPath))
            {
                return false;
            }

            if (File.Exists(fullPath) || !APIClient.IsEnabled)
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

        // Strict argument validation shared by write/delete/forget entry points.
        private bool ValidateFileArgs(string pchFile, int cubData, IntPtr pvData, bool checkBuffer)
        {
            if (string.IsNullOrEmpty(pchFile)) return false;
            if (pchFile.Length > MaxFilenameLength) return false;
            if (pchFile.IndexOfAny(Path.GetInvalidPathChars()) >= 0) return false;
            if (cubData < 0) return false;
            if (cubData > MaxFileSize) return false;
            if (checkBuffer && cubData > 0 && pvData == IntPtr.Zero) return false;
            return true;
        }

        // Atomic write: stage into a sibling .tmp then replace, so a crash mid-write
        // never leaves a half-written destination. Cleans up the temp on failure.
        private static bool AtomicWrite(string fullPath, byte[] buffer)
        {
            var tempPath = fullPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                File.WriteAllBytes(tempPath, buffer);
                if (File.Exists(fullPath))
                {
                    try
                    {
                        File.Replace(tempPath, fullPath, null);
                    }
                    catch (IOException)
                    {
                        // Replace can fail on some filesystems; fall back to delete+move.
                        File.Delete(fullPath);
                        File.Move(tempPath, fullPath);
                    }
                }
                else
                {
                    File.Move(tempPath, fullPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                try { if (File.Exists(tempPath)) File.Delete(tempPath); } catch { }
                SteamEmulator.Write("SteamRemoteStorage", $"AtomicWrite failed {fullPath} {ex.Message}");
                return false;
            }
        }

        // Reserve quota for a write of newSize replacing whatever is on disk now.
        // Returns false (write rejected) when the delta would overflow the quota.
        private bool TryReserveQuota(string fullPath, long newSize)
        {
            long prevSize = 0;
            try { if (File.Exists(fullPath)) prevSize = new FileInfo(fullPath).Length; } catch { }
            long diff = newSize - prevSize;
            if (diff > 0 && (ulong)diff > CachedQuotaAvailableBytes)
            {
                return false;
            }

            if (diff > 0)
            {
                CachedQuotaAvailableBytes -= (ulong)diff;
            }
            else
            {
                ReleaseQuota(-diff);
            }
            return true;
        }

        private void ReleaseQuota(long bytes)
        {
            if (bytes <= 0) return;
            var restored = CachedQuotaAvailableBytes + (ulong)bytes;
            CachedQuotaAvailableBytes = restored > CachedQuotaTotalBytes ? CachedQuotaTotalBytes : restored;
        }

        // Route a completed write either into the open batch or straight to upload.
        private void QueueOrBatchUpload(string normalizedKey, string fileName, byte[] content)
        {
            lock (BatchLock)
            {
                if (IsBatchActive)
                {
                    BatchUploads.Add(normalizedKey);
                    BatchDeletes.Remove(normalizedKey);
                    return;
                }
            }

            QueueRemoteUpload(normalizedKey, fileName, content);
        }

        private bool TryReadLocalByKey(string normalizedKey, out string originalName, out byte[] bytes)
        {
            bytes = null;
            originalName = RemoteStorageCache.GetEntry(normalizedKey)?.OriginalName ?? normalizedKey;
            if (!TryGetStorageFilePath(originalName, out _, out var fullPath) || !File.Exists(fullPath))
            {
                return false;
            }

            try { bytes = File.ReadAllBytes(fullPath); return true; }
            catch { return false; }
        }

        private void QueueRemoteDelete(string normalizedKey, string fileName)
        {
            PendingUploads.TryRemove(normalizedKey, out _);
            if (!APIClient.IsEnabled)
            {
                return;
            }

            PendingDeletes[normalizedKey] = 1;
            WorkQueue.Enqueue("Delete remote storage file", () =>
            {
                try { APIClient.DeleteRemoteStorageFile(fileName); }
                finally { PendingDeletes.TryRemove(normalizedKey, out _); }
            }, "storage:delete:" + normalizedKey);
        }

        private void QueueRemoteUpload(string normalizedKey, string fileName, byte[] content)
        {
            if (!APIClient.IsEnabled)
            {
                PendingUploads.TryRemove(normalizedKey, out _);
                return;
            }

            var entry = RemoteStorageCache.GetEntry(normalizedKey);
            uint? syncPlatforms = entry != null && entry.HasSyncPlatforms
                ? entry.SyncPlatforms
                : (uint?)null;
            var copy = content == null ? new byte[0] : (byte[])content.Clone();

            PendingUploads[normalizedKey] = 1;
            PendingDeletes.TryRemove(normalizedKey, out _);
            WorkQueue.Enqueue("RemoteStorage upload", () =>
            {
                try
                {
                    if (APIClient.UploadRemoteStorageFile(fileName, copy, syncPlatforms))
                    {
                        RemoteStorageCache.SetPersisted(normalizedKey);
                    }
                    else
                    {
                        Write($"Remote upload failed {fileName}");
                    }
                }
                finally
                {
                    PendingUploads.TryRemove(normalizedKey, out _);
                }
            }, "remote-storage:upload:" + normalizedKey);
        }

        private void QueueRemoteFileList()
        {
            if (!APIClient.IsEnabled)
            {
                return;
            }

            WorkQueue.Enqueue("RemoteStorage list", () =>
            {
                var files = APIClient.ListRemoteStorageFiles();
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
                    var outcome = APIClient.DownloadRemoteStorageFile(fileName, out var file);

                    switch (outcome)
                    {
                        case APIClient.RemoteStorageDownloadResult.NotFound:
                            // Clean miss, logged once per TTL (this runs when the 404 is
                            // received; IsKnownMissing suppresses re-fetches until it expires).
                            Write($"RemoteStorage miss {fileName}");
                            RemoteStorageCache.MarkMissing(remoteKey, MissingRemoteFileWindow);
                            return;

                        case APIClient.RemoteStorageDownloadResult.Unauthorized:
                            // Auth problem: short throttle only; do not blacklist as absent.
                            Write($"RemoteStorage fetch unauthorized {fileName}");
                            RemoteStorageCache.MarkMissing(remoteKey, MissingRemoteFileWindowShort);
                            return;

                        case APIClient.RemoteStorageDownloadResult.Unavailable:
                        case APIClient.RemoteStorageDownloadResult.Error:
                            // Server down / timeout / 5xx: short TTL so we retry soon.
                            Write($"RemoteStorage fetch error ({outcome}) {fileName}");
                            RemoteStorageCache.MarkMissing(remoteKey, MissingRemoteFileWindowShort);
                            return;

                        case APIClient.RemoteStorageDownloadResult.Ok:
                            break;
                    }

                    if (file == null || string.IsNullOrWhiteSpace(file.ContentBase64))
                    {
                        // Ok status but empty body: treat as a transient error, short TTL.
                        Write($"RemoteStorage fetch empty payload {fileName}");
                        RemoteStorageCache.MarkMissing(remoteKey, MissingRemoteFileWindowShort);
                        return;
                    }

                    byte[] fileBytes;
                    try
                    {
                        fileBytes = Convert.FromBase64String(file.ContentBase64);
                    }
                    catch (FormatException ex)
                    {
                        // Corrupt base64: log the real error, short TTL so a re-uploaded
                        // fixed version is not blocked for the full window.
                        Write($"RemoteStorage fetch corrupt base64 {fileName}: {ex.Message}");
                        RemoteStorageCache.MarkMissing(remoteKey, MissingRemoteFileWindowShort);
                        return;
                    }

                    try
                    {
                        Common.EnsureDirectoryExists(fullPath, true);
                        if (AtomicWrite(fullPath, fileBytes))
                        {
                            RemoteStorageCache.SetHydrated(remoteKey, fileName, file.Size, file.Sha256);
                        }
                        else
                        {
                            RemoteStorageCache.MarkMissing(remoteKey, MissingRemoteFileWindowShort);
                        }
                    }
                    catch (Exception ex)
                    {
                        RemoteStorageCache.MarkMissing(remoteKey, MissingRemoteFileWindowShort);
                        Write($"RemoteStorage fetch write failed {fileName} {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    RemoteStorageCache.MarkMissing(remoteKey, MissingRemoteFileWindowShort);
                    Write($"RemoteStorage fetch failed {fileName} {ex.Message}");
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


