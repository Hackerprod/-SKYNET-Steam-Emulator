using SKYNET;
using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamRemoteStorage : ISteamInterface
    {
        public string StoragePath;
        private List<string> StorageFiles;
        private int LastFile;

        public SteamRemoteStorage()
        {
            InterfaceVersion = "SteamRemoteStorage";
            StorageFiles = new List<string>();
            LastFile = 0;

            try
            {
                StoragePath = Path.Combine(SteamEmulator.EmulatorPath, "Data", "Storage", SteamEmulator.AppId.ToString());
                modCommon.EnsureDirectoryExists(StoragePath);
            }
            catch (Exception)
            {
                StoragePath = Path.Combine(modCommon.GetPath(), SteamEmulator.AppId.ToString());
            }
        }

        public bool FileWrite(string pchFile, IntPtr pvData, int cubData)
        {
            try
            {
                string fullPath = Path.Combine(StoragePath, pchFile);
                byte[] buffer = pvData.GetBytes(cubData);
                File.WriteAllBytes(fullPath, buffer);
                Write($"FileWrite {pchFile}, {buffer.Length} bytes");
                return true;
            }
            catch
            {
                Write($"FileWrite {pchFile}");
                return false;
            }
        }

        public int FileRead(string pchFile, IntPtr pvData, int cubDataToRead)
        {
            try
            {
                Write($"FileRead {pchFile}");
                string fullPath = Path.Combine(StoragePath, pchFile);
                byte[] bytes = File.ReadAllBytes(fullPath);
                pvData = bytes.GetPtr();
                return bytes.Length;
            }
            catch
            {
                return 0;
            }
        }

        public SteamAPICall_t FileWriteAsync(string pchFile, IntPtr pvData, uint cubData)
        {
            Write($"FileWriteAsync {pchFile}");
            try
            {
                string fullPath = Path.Combine(StoragePath, pchFile);
                modCommon.EnsureDirectoryExists(fullPath, true);
                byte[] bytes = pvData.GetBytes(cubData);
                File.WriteAllBytes(fullPath, bytes);

                //RemoteStorageFileWriteAsyncComplete_t data = new RemoteStorageFileWriteAsyncComplete_t()
                //{
                //    m_eResult = SKYNET.Types.EResult.k_EResultOK  
                //};
                ////return CallbackManager.AddCallbackResult(data);
                //return new SteamAPICall_t(CallbackType.k_iRemoteStorageFileWriteAsyncComplete);
            }
            catch
            {
                Write($"Error writing file {pchFile}");
            }
            return 0;
        }

        public SteamAPICall_t FileReadAsync(string pchFile, uint nOffset, uint cubToRead)
        {
            Write($"FileReadAsync {pchFile}");
            try
            {
                return new SteamAPICall_t(CallbackType.RemoteStorageFileReadAsyncComplete);
            }
            catch
            {
                Write($"Error reading file {pchFile}");
                return 0;
            }
        }

        public bool FileReadAsyncComplete(ulong hReadCall, IntPtr pvBuffer, uint cubToRead)
        {
            Write("FileReadAsyncComplete");
            return false;
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

        public ulong FileShare(string pchFile)
        {
            Write("FileShare");
            return default;
        }

        public bool SetSyncPlatforms(string pchFile, int eRemoteStoragePlatform)
        {
            Write("SetSyncPlatforms");
            return false;
        }

        public ulong FileWriteStreamOpen(string pchFile)
        {
            Write("FileWriteStreamOpen");
            return default;
        }

        public bool FileWriteStreamWriteChunk(ulong writeHandle, IntPtr pvData, int cubData)
        {
            Write("FileWriteStreamWriteChunk");
            return false;
        }

        public bool FileWriteStreamClose(ulong writeHandle)
        {
            Write("FileWriteStreamClose");
            return default;
        }

        public bool FileWriteStreamCancel(ulong writeHandle)
        {
            Write("FileWriteStreamCancel");
            return default;
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

            Write($"GetFileSize {pchFile} [{Length}]");

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
            return default;
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

        public string GetFileNameAndSize(int iFile, int pnFileSizeInBytes)
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
            return default;
        }

        public bool IsCloudEnabledForApp()
        {
            Write("IsCloudEnabledForApp");
            return default;
        }

        public void SetCloudEnabledForApp(bool bEnabled)
        {
            Write("SetCloudEnabledForApp");
        }

        public ulong UGCDownload(ulong hContent, uint unPriority)
        {
            Write("UGCDownload");
            return default;
        }

        public bool GetUGCDownloadProgress(ulong hContent, int pnBytesDownloaded, int pnBytesExpected)
        {
            Write("GetUGCDownloadProgress");
            return default;
        }

        public bool GetUGCDetails(ulong hContent, uint pnAppID, string ppchName, int pnFileSizeInBytes, ulong pSteamIDOwner)
        {
            Write("GetUGCDetails");
            return default;
        }

        public int UGCRead(ulong hContent, IntPtr pvData, int cubDataToRead, uint cOffset, IntPtr eAction)
        {
            Write("UGCRead");
            return default;
        }

        public int GetCachedUGCCount()
        {
            Write("GetCachedUGCCount");
            return default;
        }

        public ulong GetCachedUGCHandle(int iCachedContent)
        {
            Write("GetCachedUGCHandle");
            return default;
        }

        public ulong PublishWorkshopFile(string pchFile, string pchPreviewFile, uint nConsumerAppId, string pchTitle, string pchDescription, int eVisibility, IntPtr pTags, int int2)
        {
            Write("PublishWorkshopFile");
            return default;
        }

        public ulong CreatePublishedFileUpdateRequest(ulong unPublishedFileId)
        {
            Write("CreatePublishedFileUpdateRequest");
            return default;
        }

        public bool UpdatePublishedFileFile(ulong updateHandle, string pchFile)
        {
            Write("UpdatePublishedFileFile");
            return default;
        }

        public bool UpdatePublishedFilePreviewFile(ulong updateHandle, string pchPreviewFile)
        {
            Write("UpdatePublishedFilePreviewFile");
            return false;
        }

        public void GetFileListFromServer()
        {
            Write("GetFileListFromServer");
        }

        public bool UpdatePublishedFileTitle(ulong updateHandle, string pchTitle)
        {
            Write("UpdatePublishedFileTitle");
            return false;
        }

        public bool UpdatePublishedFileDescription(ulong updateHandle, string pchDescription)
        {
            Write("UpdatePublishedFileDescription");
            return false;
        }

        public bool UpdatePublishedFileVisibility(ulong updateHandle, int eVisibility)
        {
            Write("UpdatePublishedFileVisibility");
            return false;
        }

        public bool UpdatePublishedFileTags(ulong updateHandle, IntPtr pTags)
        {
            Write("UpdatePublishedFileTags");
            return default;
        }

        public ulong CommitPublishedFileUpdate(ulong updateHandle)
        {
            Write("CommitPublishedFileUpdate");
            return default;
        }

        public ulong GetPublishedFileDetails(ulong unPublishedFileId, uint unMaxSecondsOld)
        {
            Write("GetPublishedFileDetails");
            return default;
        }

        public ulong DeletePublishedFile(ulong unPublishedFileId)
        {
            Write("DeletePublishedFile");
            return default;
        }

        public ulong EnumerateUserPublishedFiles(uint unStartIndex)
        {
            Write("EnumerateUserPublishedFiles");
            return default;
        }

        public ulong SubscribePublishedFile(ulong unPublishedFileId)
        {
            Write("SubscribePublishedFile");
            return default;
        }

        public ulong EnumerateUserSubscribedFiles(uint unStartIndex)
        {
            Write("EnumerateUserSubscribedFiles");
            return default;
        }

        public ulong UnsubscribePublishedFile(ulong unPublishedFileId)
        {
            Write("UnsubscribePublishedFile");
            return default;
        }

        public bool UpdatePublishedFileSetChangeDescription(ulong updateHandle, string pchChangeDescription)
        {
            Write("UpdatePublishedFileSetChangeDescription");
            return default;
        }

        public ulong GetPublishedItemVoteDetails(ulong unPublishedFileId)
        {
            Write("GetPublishedItemVoteDetails");
            return default;
        }

        public ulong UpdateUserPublishedItemVote(ulong unPublishedFileId, bool bVoteUp)
        {
            Write("UpdateUserPublishedItemVote");
            return default;
        }

        public ulong GetUserPublishedItemVoteDetails(ulong unPublishedFileId)
        {
            Write("GetUserPublishedItemVoteDetails");
            return default;
        }

        public ulong EnumerateUserSharedWorkshopFiles(ulong steamId, uint unStartIndex, IntPtr pRequiredTags, IntPtr pExcludedTags)
        {
            Write("EnumerateUserSharedWorkshopFiles");
            return default;
        }

        public ulong PublishVideo(int eVideoProvider, string pchVideoAccount, string pchVideoIdentifier, string pchPreviewFile, uint nConsumerAppId, string pchTitle, string pchDescription, int eVisibility, IntPtr pTags)
        {
            Write("PublishVideo");
            return default;
        }

        public ulong SetUserPublishedFileAction(ulong unPublishedFileId, int eAction)
        {
            Write("SetUserPublishedFileAction");
            return default;
        }

        public ulong EnumeratePublishedFilesByUserAction(int eAction, uint unStartIndex)
        {
            Write("EnumeratePublishedFilesByUserAction");
            return default;
        }

        public ulong EnumeratePublishedWorkshopFiles(int eEnumerationType, uint unStartIndex, uint unCount, uint unDays, IntPtr pTags, IntPtr pUserTags)
        {
            Write("EnumeratePublishedWorkshopFiles");
            return default;
        }

        public ulong UGCDownloadToLocation(ulong hContent, string pchLocation, uint unPriority)
        {
            Write("UGCDownloadToLocation");
            return default;
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

        private unsafe byte[] ReadBytes(IntPtr buffer, int count)
        {
            byte[] array = new byte[count];
            byte* ptr = (byte*)(void*)buffer;
            for (int i = 0; i < count; i++)
            {
                array[i] = ptr[i];
            }
            return array;
        }


        public void PostCallback(int callback_id, SKYNET.Callback.Buffer b)
        {
            CallbackHandler.PostCallback(1, 1, callback_id, b);
        }
    }
}

public struct SteamAPICall_t : IEquatable<SteamAPICall_t>, IComparable<SteamAPICall_t>
{
    public ulong Value;

    public SteamAPICall_t(CallbackType value)
    {
        Value = (ulong)value;
    }

    public static implicit operator SteamAPICall_t(ulong value)
    {
        SteamAPICall_t result = default(SteamAPICall_t);
        result.Value = value;
        return result;
    }

    public static implicit operator ulong(SteamAPICall_t value)
    {
        return value.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override bool Equals(object p)
    {
        return Equals((SteamAPICall_t)p);
    }

    public bool Equals(SteamAPICall_t p)
    {
        return p.Value == Value;
    }

    public static bool operator ==(SteamAPICall_t a, SteamAPICall_t b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(SteamAPICall_t a, SteamAPICall_t b)
    {
        return !a.Equals(b);
    }

    public int CompareTo(SteamAPICall_t other)
    {
        return Value.CompareTo(other.Value);
    }
}



