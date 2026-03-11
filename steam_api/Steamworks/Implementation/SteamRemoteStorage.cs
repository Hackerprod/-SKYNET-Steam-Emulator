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
        private List<SkyNetApiClient.SkyNetRemoteStorageFileListItemDto> RemoteStorageFiles;
        private ConcurrentDictionary<ulong, string> SharedFiles;
        private int LastFile;
        private Dictionary<ulong, string> AsyncFilesRead;
        private ConcurrentDictionary<ulong, PendingWriteStream> PendingWriteStreams;

        public SteamRemoteStorage()
        {
            Instance = this;
            InterfaceName = "SteamRemoteStorage";
            InterfaceVersion = "STEAMREMOTESTORAGE_INTERFACE_VERSION016";
            StoragePath = Path.Combine(Common.GetPath(), "SKYNET", "Storage", "Remote");
            AvatarCachePath = Path.Combine(Common.GetPath(), "Data", "Images", "AvatarCache");
            StorageFiles = new List<string>();
            RemoteStorageFiles = new List<SkyNetApiClient.SkyNetRemoteStorageFileListItemDto>();
            SharedFiles = new ConcurrentDictionary<ulong, string>();
            AsyncFilesRead = new Dictionary<ulong, string>();
            PendingWriteStreams = new ConcurrentDictionary<ulong, PendingWriteStream>();
            LastFile = 0;
        }

        public bool FileWrite(string pchFile, IntPtr pvData, int cubData)
        {
            bool Result = false;
            MutexHelper.Wait("FileWrite", delegate
            {
                try
                {
                    string fullPath = Path.Combine(StoragePath, pchFile);
                    Common.EnsureDirectoryExists(fullPath, true);
                    byte[] buffer = pvData.GetBytes(cubData);
                    if (SkyNetApiClient.IsEnabled)
                    {
                        Result = SkyNetApiClient.UploadRemoteStorageFile(pchFile, buffer);
                        if (Result)
                        {
                            File.WriteAllBytes(fullPath, buffer);
                        }
                    }
                    else
                    {
                        File.WriteAllBytes(fullPath, buffer);
                        Result = true;
                    }
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

            MutexHelper.Wait("FileRead", delegate
            {
                try
                {
                    string fullPath = Path.Combine(StoragePath, pchFile);
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
                string fullPath = Path.Combine(StoragePath, pchFile);
                Common.EnsureDirectoryExists(fullPath, true);
                byte[] bytes = pvData.GetBytes(cubData);
                if (SkyNetApiClient.IsEnabled)
                {
                    if (!SkyNetApiClient.UploadRemoteStorageFile(pchFile, bytes))
                    {
                        return APICall;
                    }
                }

                File.WriteAllBytes(fullPath, bytes);

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

                    string fullPath = Path.Combine(StoragePath, pchFile);
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
            string fullPath = Path.Combine(StoragePath, pchFile);
            return File.Exists(fullPath);
        }

        public bool FileDelete(string pchFile)
        {
            Write($"FileDelete {pchFile}");
            string fullPath = Path.Combine(StoragePath, pchFile);
            var deletedRemotely = true;
            if (SkyNetApiClient.IsEnabled && !SkyNetApiClient.DeleteRemoteStorageFile(pchFile))
            {
                return false;
            }

            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return deletedRemotely;
        }

        public SteamAPICall_t FileShare(string pchFile)
        {
            Write("FileShare");
            SteamAPICall_t APICall = k_uAPICallInvalid;
            MutexHelper.Wait("FileShare", delegate
            {
                try
                {
                    RemoteStorageFileShareResult_t data = new RemoteStorageFileShareResult_t();
                    string fullPath = Path.Combine(StoragePath, pchFile);
                    EnsureRemoteFileCached(pchFile);
                    if (!File.Exists(fullPath))
                    {
                        data.m_eResult = EResult.k_EResultFileNotFound;
                    }
                    else
                    {
                        data.m_eResult = EResult.k_EResultOK;
                        data.m_hFile = (ulong)CSteamID.CreateOne();
                        if (SkyNetApiClient.IsEnabled)
                        {
                            var share = SkyNetApiClient.ShareRemoteStorageFile(pchFile);
                            if (share != null)
                            {
                                data.m_eResult = share.Result;
                                data.m_hFile = share.Handle;
                            }
                        }
                        data.m_rgchFilename = Encoding.Default.GetBytes(pchFile);
                        SharedFiles.TryAdd(data.m_hFile, pchFile);
                        APICall = CallbackManager.AddCallbackResult(data);
                    }
                }
                catch (Exception ex)
                {
                    Write($"Error Sharing file {pchFile} {ex}");
                }
            });
            return APICall;
        }

        public bool SetSyncPlatforms(string pchFile, int eRemoteStoragePlatform)
        {
            Write("SetSyncPlatforms");
            return false;
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
            string fullPath = Path.Combine(StoragePath, pchFile);
            bool Exists = File.Exists(fullPath);
            if (!Exists && SkyNetApiClient.IsEnabled)
            {
                Exists = EnsureRemoteFileCached(pchFile);
            }
            Write($"FileExists {pchFile} = {Exists}");
            return Exists;
        }

        public bool FilePersisted(string pchFile)
        {
            Write("FilePersisted");
            return false;
        }

        public int GetFileSize(string pchFile)
        {
            int Length = 0;

            string fullPath = Path.Combine(StoragePath, pchFile);
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
            return 0;
        }

        public int GetSyncPlatforms(string pchFile)
        {
            Write("GetSyncPlatforms");
            return 0;
        }

        public int GetFileCount()
        {
            if (SkyNetApiClient.IsEnabled)
            {
                RemoteStorageFiles = SkyNetApiClient.ListRemoteStorageFiles() ?? new List<SkyNetApiClient.SkyNetRemoteStorageFileListItemDto>();
                StorageFiles = RemoteStorageFiles.ConvertAll(file => file.FileName);
                LastFile = 0;
                Write($"GetFileCount {RemoteStorageFiles.Count}");
                return RemoteStorageFiles.Count;
            }

            if (Directory.Exists(StoragePath))
            {
                StorageFiles = Directory.GetFiles(StoragePath, "*.*", SearchOption.AllDirectories).ToList();
                LastFile = 0;
            }
            Write($"GetFileCount {StorageFiles.Count}");
            return StorageFiles.Count;
        }

        public string GetFileNameAndSize(int iFile, ref int pnFileSizeInBytes)
        {
            if (SkyNetApiClient.IsEnabled)
            {
                if (RemoteStorageFiles == null || RemoteStorageFiles.Count == 0)
                {
                    RemoteStorageFiles = SkyNetApiClient.ListRemoteStorageFiles() ?? new List<SkyNetApiClient.SkyNetRemoteStorageFileListItemDto>();
                }

                if (iFile < 0 || iFile >= RemoteStorageFiles.Count)
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
            if (SkyNetApiClient.IsEnabled)
            {
                var quota = SkyNetApiClient.GetRemoteStorageQuota();
                if (quota != null)
                {
                    pnTotalBytes = quota.TotalBytes;
                    puAvailableBytes = quota.AvailableBytes;
                    return true;
                }
            }

            pnTotalBytes = 1024 * 1024 * 1024;
            puAvailableBytes = 1024 * 1024 * 1024;
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

        public bool GetUGCDetails(UGCHandle_t hContent, uint pnAppID, string ppchName, int pnFileSizeInBytes, ulong pSteamIDOwner)
        {
            Write("GetUGCDetails");
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
            string fullPath = Path.Combine(StoragePath, pchFile);
            if (File.Exists(fullPath) || !SkyNetApiClient.IsEnabled)
            {
                return File.Exists(fullPath);
            }

            try
            {
                var file = SkyNetApiClient.DownloadRemoteStorageFile(pchFile);
                if (file == null || string.IsNullOrWhiteSpace(file.ContentBase64))
                {
                    return false;
                }

                Common.EnsureDirectoryExists(fullPath, true);
                File.WriteAllBytes(fullPath, Convert.FromBase64String(file.ContentBase64));
                return true;
            }
            catch (Exception ex)
            {
                Write($"EnsureRemoteFileCached {pchFile} {ex}");
                return false;
            }
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





