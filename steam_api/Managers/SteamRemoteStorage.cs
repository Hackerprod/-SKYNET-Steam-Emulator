using SKYNET.Interface;
using Steamworks;
using System;

namespace SKYNET.Managers
{
    public class SteamRemoteStorage : ISteamRemoteStorage
    {
        public bool FileWrite(string pchFile, IntPtr pvData, int cubData)
        {
            return false;
        }

        public int FileRead(string pchFile, IntPtr pvData, int cubDataToRead)
        {
            return default;
        }

        public SteamAPICall_t FileWriteAsync(string pchFile, IntPtr pvData, uint cubData)
        {
            return default;
        }

        public SteamAPICall_t FileReadAsync(string pchFile, uint nOffset, uint cubToRead)
        {
            return default;
        }

        public bool FileReadAsyncComplete(SteamAPICall_t hReadCall, IntPtr pvBuffer, uint cubToRead)
        {
            return false;
        }

        public bool FileForget(string pchFile)
        {
            return default;
        }

        public bool FileDelete(string pchFile)
        {
            return false;
        }

        public SteamAPICall_t FileShare(string pchFile)
        {
            return default;
        }

        public bool SetSyncPlatforms(string pchFile, ERemoteStoragePlatform eRemoteStoragePlatform)
        {
            return false;
        }

        public UGCFileWriteStreamHandle_t FileWriteStreamOpen(string pchFile)
        {
            return default;
        }

        public bool FileWriteStreamWriteChunk(UGCFileWriteStreamHandle_t writeHandle, IntPtr pvData, int cubData)
        {
            return false;
        }

        public bool FileWriteStreamClose(UGCFileWriteStreamHandle_t writeHandle)
        {
            return default;
        }

        public bool FileWriteStreamCancel(UGCFileWriteStreamHandle_t writeHandle)
        {
            return default;
        }

        public bool FileExists(string pchFile)
        {
            return default;
        }

        public bool FilePersisted(string pchFile)
        {
            return false;
        }

        public int GetFileSize(string pchFile)
        {
            return 0;
        }

        public uint GetFileTimestamp(string pchFile)
        {
            return 0;
        }

        public ERemoteStoragePlatform GetSyncPlatforms(string pchFile)
        {
            return default;
        }

        public int GetFileCount()
        {
            return 0;
        }

        public string GetFileNameAndSize(int iFile, int pnFileSizeInBytes)
        {
            return default;
        }

        public bool GetQuota(uint pnTotalBytes, uint puAvailableBytes)
        {
            return false;
        }

        public bool IsCloudEnabledForAccount()
        {
            return default;
        }

        public bool IsCloudEnabledForApp()
        {
            return default;
        }

        public void SetCloudEnabledForApp(bool bEnabled)
        {
            //
        }

        public SteamAPICall_t UGCDownload(UGCHandle_t hContent, uint unPriority)
        {
            return default;
        }

        public bool GetUGCDownloadProgress(UGCHandle_t hContent, int pnBytesDownloaded, int pnBytesExpected)
        {
            return default;
        }

        public bool GetUGCDetails(UGCHandle_t hContent, AppId_t pnAppID, string ppchName, int pnFileSizeInBytes, IntPtr pSteamIDOwner)
        {
            return default;
        }

        public int UGCRead(UGCHandle_t hContent, IntPtr pvData, int cubDataToRead, uint cOffset, IntPtr eAction)
        {
            return default;
        }

        public int GetCachedUGCCount()
        {
            return default;
        }

        public UGCHandle_t GetCachedUGCHandle(int iCachedContent)
        {
            return default;
        }

        public SteamAPICall_t PublishWorkshopFile(string pchFile, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, IntPtr pTags, int int2)
        {
            return default;
        }

        public PublishedFileUpdateHandle_t CreatePublishedFileUpdateRequest(PublishedFileId_t unPublishedFileId)
        {
            return default;
        }

        public bool UpdatePublishedFileFile(PublishedFileUpdateHandle_t updateHandle, string pchFile)
        {
            return default;
        }

        public bool UpdatePublishedFilePreviewFile(PublishedFileUpdateHandle_t updateHandle, string pchPreviewFile)
        {
            return false;
        }

        public bool UpdatePublishedFileTitle(PublishedFileUpdateHandle_t updateHandle, string pchTitle)
        {
            return false;
        }

        public bool UpdatePublishedFileDescription(PublishedFileUpdateHandle_t updateHandle, string pchDescription)
        {
            return false;
        }

        public bool UpdatePublishedFileVisibility(PublishedFileUpdateHandle_t updateHandle, ERemoteStoragePublishedFileVisibility eVisibility)
        {
            return false;
        }

        public bool UpdatePublishedFileTags(PublishedFileUpdateHandle_t updateHandle, IntPtr pTags)
        {
            return default;
        }

        public SteamAPICall_t CommitPublishedFileUpdate(PublishedFileUpdateHandle_t updateHandle)
        {
            return default;
        }

        public SteamAPICall_t GetPublishedFileDetails(PublishedFileId_t unPublishedFileId, uint unMaxSecondsOld)
        {
            return default;
        }

        public SteamAPICall_t DeletePublishedFile(PublishedFileId_t unPublishedFileId)
        {
            return default;
        }

        public SteamAPICall_t EnumerateUserPublishedFiles(uint unStartIndex)
        {
            return default;
        }

        public SteamAPICall_t SubscribePublishedFile(PublishedFileId_t unPublishedFileId)
        {
            return default;
        }

        public SteamAPICall_t EnumerateUserSubscribedFiles(uint unStartIndex)
        {
            return default;
        }

        public SteamAPICall_t UnsubscribePublishedFile(PublishedFileId_t unPublishedFileId)
        {
            return default;
        }

        public bool UpdatePublishedFileSetChangeDescription(PublishedFileUpdateHandle_t updateHandle, string pchChangeDescription)
        {
            return default;
        }

        public SteamAPICall_t GetPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId)
        {
            return default;
        }

        public SteamAPICall_t UpdateUserPublishedItemVote(PublishedFileId_t unPublishedFileId, bool bVoteUp)
        {
            return default;
        }

        public SteamAPICall_t GetUserPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId)
        {
            return default;
        }

        public SteamAPICall_t EnumerateUserSharedWorkshopFiles(IntPtr steamId, uint unStartIndex, IntPtr pRequiredTags, IntPtr pExcludedTags)
        {
            return default;
        }

        public SteamAPICall_t PublishVideo(EWorkshopVideoProvider eVideoProvider, string pchVideoAccount, string pchVideoIdentifier, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, IntPtr pTags)
        {
            return default;
        }

        public SteamAPICall_t SetUserPublishedFileAction(PublishedFileId_t unPublishedFileId, EWorkshopFileAction eAction)
        {
            return default;
        }

        public SteamAPICall_t EnumeratePublishedFilesByUserAction(EWorkshopFileAction eAction, uint unStartIndex)
        {
            return default;
        }

        public SteamAPICall_t EnumeratePublishedWorkshopFiles(EWorkshopEnumerationType eEnumerationType, uint unStartIndex, uint unCount, uint unDays, IntPtr pTags, IntPtr pUserTags)
        {
            return default;
        }

        public SteamAPICall_t UGCDownloadToLocation(UGCHandle_t hContent, string pchLocation, uint unPriority)
        {
            return default;
        }

    }

}