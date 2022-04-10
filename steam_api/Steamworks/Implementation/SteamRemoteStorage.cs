using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    [StructLayout(LayoutKind.Sequential)]
    public class SteamRemoteStorage : ISteamInterface
    {
        public bool FileWrite(string pchFile, IntPtr pvData, int cubData)
        {
            Write("FileWrite");
            return false;
        }

        public int FileRead(string pchFile, IntPtr pvData, int cubDataToRead)
        {
            Write("FileRead");
            return default;
        }

        public SteamAPICall_t FileWriteAsync(string pchFile, IntPtr pvData, uint cubData)
        {
            Write("FileWriteAsync");
            return default;
        }

        public SteamAPICall_t FileReadAsync(string pchFile, uint nOffset, uint cubToRead)
        {
            Write("FileReadAsync");
            return default;
        }

        public bool FileReadAsyncComplete(SteamAPICall_t hReadCall, IntPtr pvBuffer, uint cubToRead)
        {
            Write("FileReadAsyncComplete");
            return false;
        }

        public bool FileForget(string pchFile)
        {
            Write("FileForget");
            return default;
        }

        public bool FileDelete(string pchFile)
        {
            Write("FileDelete");
            return false;
        }

        public SteamAPICall_t FileShare(string pchFile)
        {
            Write("FileShare");
            return default;
        }

        public bool SetSyncPlatforms(string pchFile, ERemoteStoragePlatform eRemoteStoragePlatform)
        {
            Write("SetSyncPlatforms");
            return false;
        }

        public UGCFileWriteStreamHandle_t FileWriteStreamOpen(string pchFile)
        {
            Write("FileWriteStreamOpen");
            return default;
        }

        public bool FileWriteStreamWriteChunk(UGCFileWriteStreamHandle_t writeHandle, IntPtr pvData, int cubData)
        {
            Write("FileWriteStreamWriteChunk");
            return false;
        }

        public bool FileWriteStreamClose(UGCFileWriteStreamHandle_t writeHandle)
        {
            Write("FileWriteStreamClose");
            return default;
        }

        public bool FileWriteStreamCancel(UGCFileWriteStreamHandle_t writeHandle)
        {
            Write("FileWriteStreamCancel");
            return default;
        }

        public bool FileExists(string pchFile)
        {
            Write("FileExists");
            return default;
        }

        public bool FilePersisted(string pchFile)
        {
            Write("FilePersisted");
            return false;
        }

        public int GetFileSize(string pchFile)
        {
            Write("GetFileSize");
            return 0;
        }

        public uint GetFileTimestamp(string pchFile)
        {
            Write("GetFileTimestamp");
            return 0;
        }

        public ERemoteStoragePlatform GetSyncPlatforms(string pchFile)
        {
            Write("GetSyncPlatforms");
            return default;
        }

        public int GetFileCount(IntPtr _)
        {
            Write("GetFileCount");
            return 0;
        }

        public string GetFileNameAndSize(int iFile, int pnFileSizeInBytes)
        {
            Write("GetFileNameAndSize");
            return default;
        }

        public bool GetQuota(uint pnTotalBytes, uint puAvailableBytes)
        {
            Write("GetQuota");
            return false;
        }

        public bool IsCloudEnabledForAccount(IntPtr _)
        {
            Write("IsCloudEnabledForAccount");
            return default;
        }

        public bool IsCloudEnabledForApp(IntPtr _)
        {
            Write("IsCloudEnabledForApp");
            return default;
        }

        public void SetCloudEnabledForApp(bool bEnabled)
        {
            Write("SetCloudEnabledForApp");
            //
        }

        public SteamAPICall_t UGCDownload(UGCHandle_t hContent, uint unPriority)
        {
            Write("UGCDownload");
            return default;
        }

        public bool GetUGCDownloadProgress(UGCHandle_t hContent, int pnBytesDownloaded, int pnBytesExpected)
        {
            Write("GetUGCDownloadProgress");
            return default;
        }

        public bool GetUGCDetails(UGCHandle_t hContent, AppId_t pnAppID, string ppchName, int pnFileSizeInBytes, IntPtr pSteamIDOwner)
        {
            Write("GetUGCDetails");
            return default;
        }

        public int UGCRead(UGCHandle_t hContent, IntPtr pvData, int cubDataToRead, uint cOffset, IntPtr eAction)
        {
            Write("UGCRead");
            return default;
        }

        public int GetCachedUGCCount(IntPtr _)
        {
            Write("GetCachedUGCCount");
            return default;
        }

        public UGCHandle_t GetCachedUGCHandle(int iCachedContent)
        {
            Write("GetCachedUGCHandle");
            return default;
        }

        public SteamAPICall_t PublishWorkshopFile(string pchFile, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, IntPtr pTags, int int2)
        {
            Write("PublishWorkshopFile");
            return default;
        }

        public PublishedFileUpdateHandle_t CreatePublishedFileUpdateRequest(PublishedFileId_t unPublishedFileId)
        {
            Write("CreatePublishedFileUpdateRequest");
            return default;
        }

        public bool UpdatePublishedFileFile(PublishedFileUpdateHandle_t updateHandle, string pchFile)
        {
            Write("UpdatePublishedFileFile");
            return default;
        }

        public bool UpdatePublishedFilePreviewFile(PublishedFileUpdateHandle_t updateHandle, string pchPreviewFile)
        {
            Write("UpdatePublishedFilePreviewFile");
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

        public bool UpdatePublishedFileVisibility(PublishedFileUpdateHandle_t updateHandle, ERemoteStoragePublishedFileVisibility eVisibility)
        {
            Write("UpdatePublishedFileVisibility");
            return false;
        }

        public bool UpdatePublishedFileTags(PublishedFileUpdateHandle_t updateHandle, IntPtr pTags)
        {
            Write("UpdatePublishedFileTags");
            return default;
        }

        public SteamAPICall_t CommitPublishedFileUpdate(PublishedFileUpdateHandle_t updateHandle)
        {
            Write("CommitPublishedFileUpdate");
            return default;
        }

        public SteamAPICall_t GetPublishedFileDetails(PublishedFileId_t unPublishedFileId, uint unMaxSecondsOld)
        {
            Write("GetPublishedFileDetails");
            return default;
        }

        public SteamAPICall_t DeletePublishedFile(PublishedFileId_t unPublishedFileId)
        {
            Write("DeletePublishedFile");
            return default;
        }

        public SteamAPICall_t EnumerateUserPublishedFiles(uint unStartIndex)
        {
            Write("EnumerateUserPublishedFiles");
            return default;
        }

        public SteamAPICall_t SubscribePublishedFile(PublishedFileId_t unPublishedFileId)
        {
            Write("SubscribePublishedFile");
            return default;
        }

        public SteamAPICall_t EnumerateUserSubscribedFiles(uint unStartIndex)
        {
            Write("EnumerateUserSubscribedFiles");
            return default;
        }

        public SteamAPICall_t UnsubscribePublishedFile(PublishedFileId_t unPublishedFileId)
        {
            Write("UnsubscribePublishedFile");
            return default;
        }

        public bool UpdatePublishedFileSetChangeDescription(PublishedFileUpdateHandle_t updateHandle, string pchChangeDescription)
        {
            Write("UpdatePublishedFileSetChangeDescription");
            return default;
        }

        public SteamAPICall_t GetPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId)
        {
            Write("GetPublishedItemVoteDetails");
            return default;
        }

        public SteamAPICall_t UpdateUserPublishedItemVote(PublishedFileId_t unPublishedFileId, bool bVoteUp)
        {
            Write("UpdateUserPublishedItemVote");
            return default;
        }

        public SteamAPICall_t GetUserPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId)
        {
            Write("GetUserPublishedItemVoteDetails");
            return default;
        }

        public SteamAPICall_t EnumerateUserSharedWorkshopFiles(IntPtr steamId, uint unStartIndex, IntPtr pRequiredTags, IntPtr pExcludedTags)
        {
            Write("EnumerateUserSharedWorkshopFiles");
            return default;
        }

        public SteamAPICall_t PublishVideo(EWorkshopVideoProvider eVideoProvider, string pchVideoAccount, string pchVideoIdentifier, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, IntPtr pTags)
        {
            Write("PublishVideo");
            return default;
        }

        public SteamAPICall_t SetUserPublishedFileAction(PublishedFileId_t unPublishedFileId, EWorkshopFileAction eAction)
        {
            Write("SetUserPublishedFileAction");
            return default;
        }

        public SteamAPICall_t EnumeratePublishedFilesByUserAction(EWorkshopFileAction eAction, uint unStartIndex)
        {
            Write("EnumeratePublishedFilesByUserAction");
            return default;
        }

        public SteamAPICall_t EnumeratePublishedWorkshopFiles(EWorkshopEnumerationType eEnumerationType, uint unStartIndex, uint unCount, uint unDays, IntPtr pTags, IntPtr pUserTags)
        {
            Write("EnumeratePublishedWorkshopFiles");
            return default;
        }

        public SteamAPICall_t UGCDownloadToLocation(UGCHandle_t hContent, string pchLocation, uint unPriority)
        {
            Write("UGCDownloadToLocation");
            return default;
        }

        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamRemoteStorage()
        {
            InterfaceVersion = "SteamRemoteStorage";
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}