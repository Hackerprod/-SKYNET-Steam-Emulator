using SKYNET;
using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks;
using SKYNET.Types;
using Steamworks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using SteamAPICall_t = System.UInt64;
using UGCFileWriteStreamHandle_t = System.UInt64;
using UGCHandle_t = System.UInt64;
using PublishedFileUpdateHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamRemoteStorage : ISteamInterface
    {
        public string StoragePath;
        public string AvatarCachePath;
        private List<string> StorageFiles;
        private ConcurrentDictionary<ulong, string> SharedFiles;
        private int LastFile;
        private SteamAPICall_t k_uAPICallInvalid = 0x0;
        private Dictionary<ulong, string> AsyncFilesRead;

        public SteamRemoteStorage()
        {
            InterfaceVersion = "SteamRemoteStorage";
            StorageFiles = new List<string>();
            SharedFiles = new ConcurrentDictionary<ulong, string>();
            AsyncFilesRead = new Dictionary<ulong, string>();
            LastFile = 0;

            try
            {
                string MainPath = "";
                if (SteamEmulator.Hooked)
                {
                    StoragePath = Path.Combine(SteamEmulator.EmulatorPath, "SKYNET", "Storage", SteamEmulator.AppId.ToString(), "remote");
                    modCommon.EnsureDirectoryExists(StoragePath);
                }
                else
                {
                    StoragePath = Path.Combine(modCommon.GetPath(), "SKYNET", "Storage");
                    modCommon.EnsureDirectoryExists(StoragePath);
                }
            }
            catch (Exception ex)
            {
                Write($"Error in StoragePath: {ex}");
            }

            AvatarCachePath = Path.Combine(modCommon.GetPath(), "SKYNET", "AvatarCache");
            modCommon.EnsureDirectoryExists(AvatarCachePath);

        }

        public bool FileWrite(string pchFile, string pvData, int cubData)
        {
            bool Result = false;
            MutexHelper.Wait("FileWrite", delegate
            {
                try
                {
                    string fullPath = Path.Combine(StoragePath, pchFile);
                    byte[] buffer = Encoding.Default.GetBytes(pvData);
                    File.WriteAllBytes(fullPath, buffer);
                    Write($"FileWrite {pchFile}, {buffer.Length} bytes");
                    Result = true;
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
            string Data = "";

            MutexHelper.Wait("FileRead", delegate
            {
                try
                {
                    string fullPath = Path.Combine(StoragePath, pchFile);
                    Data = File.ReadAllText(fullPath);
                    Result = Data.Length;
                }
                catch (Exception ex)
                {
                    Write($"FileRead {pchFile} {ex}");
                    Result = 0;
                }
            });

            byte[] bytes = Encoding.Default.GetBytes(Data);
            Marshal.Copy(bytes, 0, pvData, bytes.Length);
            Result = bytes.Length;

            return Result;
        }

        public SteamAPICall_t FileWriteAsync(string pchFile, string pvData, uint cubData)
        {
            Write($"FileWriteAsync {pchFile}");

            SteamAPICall_t APICall = k_uAPICallInvalid;
            try
            {
                string fullPath = Path.Combine(StoragePath, pchFile);
                modCommon.EnsureDirectoryExists(fullPath, true);
                byte[] bytes = Encoding.Default.GetBytes(pvData);
                File.WriteAllBytes(fullPath, bytes);

                RemoteStorageFileWriteAsyncComplete_t data = new RemoteStorageFileWriteAsyncComplete_t()
                {
                    m_eResult = EResult.k_EResultOK
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
                    if (File.Exists(fullPath))
                    {
                        data.m_nOffset = nOffset;
                        data.m_cubRead = cubToRead;
                        data.m_eResult = EResult.k_EResultOK;
                        data.m_hFileReadAsync = (ulong)new CSteamID();

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
                    string Data = "";
                    try
                    {
                        Data = File.ReadAllText(fullPath);
                    }
                    catch (Exception ex)
                    {
                        Write($"FileRead {fullPath} {ex}");
                    }

                    byte[] bytes = Encoding.Default.GetBytes(Data);
                    Marshal.Copy(bytes, 0, pvBuffer, bytes.Length);
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
            Write("FileForget");
            return false;
        }

        public bool FileDelete(string pchFile)
        {
            Write($"FileDelete {pchFile}");
            string fullPath = Path.Combine(StoragePath, pchFile);
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
            return false;
        }

        public SteamAPICall_t FileShare(string pchFile)
        {
            Write("FileShare");
            SteamAPICall_t APICall = k_uAPICallInvalid;
            MutexHelper.Wait("FileShare", delegate
            {
                try
                {
                    string fullPath = Path.Combine(StoragePath, pchFile);
                    RemoteStorageFileShareResult_t data = new RemoteStorageFileShareResult_t();
                    if (!File.Exists(fullPath))
                    {
                        data.m_eResult = EResult.k_EResultFileNotFound;
                    }
                    else
                    {
                        data.m_eResult = EResult.k_EResultOK;
                        data.m_hFile = (ulong)new CSteamID();
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
            MutexHelper.Wait("FileWriteStreamOpen", delegate
            {
                // TODO
            });
            return 0;
        }

        public bool FileWriteStreamWriteChunk(ulong writeHandle, IntPtr pvData, int cubData)
        {
            Write("FileWriteStreamWriteChunk");
            MutexHelper.Wait("FileWriteStreamWriteChunk", delegate
            {
                // TODO
            });
            return false;
        }

        public bool FileWriteStreamClose(ulong writeHandle)
        {
            Write("FileWriteStreamClose");
            MutexHelper.Wait("FileWriteStreamClose", delegate
            {
                // TODO
            });
            return false;
        }

        public bool FileWriteStreamCancel(ulong writeHandle)
        {
            Write("FileWriteStreamCancel");
            MutexHelper.Wait("FileWriteStreamCancel", delegate
            {
                // TODO
            });
            return false;
        }

        public bool FileExists(string pchFile)
        {
            Write($"FileExists {pchFile}");
            string fullPath = Path.Combine(StoragePath, pchFile);
            return File.Exists(fullPath);
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

        public bool GetQuota(int pnTotalBytes, int puAvailableBytes)
        {
            Write("GetQuota");
            return false;
        }

        public bool IsCloudEnabledForAccount()
        {
            Write("IsCloudEnabledForAccount");
            return false;
        }

        public bool IsCloudEnabledForApp()
        {
            Write("IsCloudEnabledForApp");
            return false;
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

        public int UGCRead(UGCHandle_t hContent, IntPtr pvData, int cubDataToRead, uint cOffset, IntPtr eAction)
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

        public void StoreAvatar(Bitmap avatar, uint accountID)
        {
            try
            {
                modCommon.EnsureDirectoryExists(AvatarCachePath);
                avatar.Save(Path.Combine(AvatarCachePath, accountID + ".jpg"));
            }
            catch 
            {

            }
        }
    }
}





