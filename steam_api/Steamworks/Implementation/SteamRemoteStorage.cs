using SKYNET;
using SKYNET.Steamworks;
using Steamworks;
using System;

public class SteamRemoteStorage : SteamInterface
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

    public bool FileReadAsyncComplete(IntPtr _, SteamAPICall_t hReadCall, IntPtr pvBuffer, uint cubToRead)
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

    public bool FileWriteStreamWriteChunk(IntPtr _, UGCFileWriteStreamHandle_t writeHandle, IntPtr pvData, int cubData)
    {
        Write("FileWriteStreamWriteChunk");
        return false;
    }

    public bool FileWriteStreamClose(IntPtr _, UGCFileWriteStreamHandle_t writeHandle)
    {
        Write("FileWriteStreamClose");
        return default;
    }

    public bool FileWriteStreamCancel(IntPtr _, UGCFileWriteStreamHandle_t writeHandle)
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

    public SteamAPICall_t UGCDownload(IntPtr _, UGCHandle_t hContent, uint unPriority)
    {
        Write("UGCDownload");
        return default;
    }

    public bool GetUGCDownloadProgress(IntPtr _, UGCHandle_t hContent, int pnBytesDownloaded, int pnBytesExpected)
    {
        Write("GetUGCDownloadProgress");
        return default;
    }

    public bool GetUGCDetails(IntPtr _, UGCHandle_t hContent, AppId_t pnAppID, string ppchName, int pnFileSizeInBytes, IntPtr pSteamIDOwner)
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

    public PublishedFileUpdateHandle_t CreatePublishedFileUpdateRequest(IntPtr _, PublishedFileId_t unPublishedFileId)
    {
        Write("CreatePublishedFileUpdateRequest");
        return default;
    }

    public bool UpdatePublishedFileFile(IntPtr _, PublishedFileUpdateHandle_t updateHandle, string pchFile)
    {
        Write("UpdatePublishedFileFile");
        return default;
    }

    public bool UpdatePublishedFilePreviewFile(IntPtr _, PublishedFileUpdateHandle_t updateHandle, string pchPreviewFile)
    {
        Write("UpdatePublishedFilePreviewFile");
        return false;
    }

    public bool UpdatePublishedFileTitle(IntPtr _, PublishedFileUpdateHandle_t updateHandle, string pchTitle)
    {
        Write("UpdatePublishedFileTitle");
        return false;
    }

    public bool UpdatePublishedFileDescription(IntPtr _, PublishedFileUpdateHandle_t updateHandle, string pchDescription)
    {
        Write("UpdatePublishedFileDescription");
        return false;
    }

    public bool UpdatePublishedFileVisibility(IntPtr _, PublishedFileUpdateHandle_t updateHandle, ERemoteStoragePublishedFileVisibility eVisibility)
    {
        Write("UpdatePublishedFileVisibility");
        return false;
    }

    public bool UpdatePublishedFileTags(IntPtr _, PublishedFileUpdateHandle_t updateHandle, IntPtr pTags)
    {
        Write("UpdatePublishedFileTags");
        return default;
    }

    public SteamAPICall_t CommitPublishedFileUpdate(IntPtr _, PublishedFileUpdateHandle_t updateHandle)
    {
        Write("CommitPublishedFileUpdate");
        return default;
    }

    public SteamAPICall_t GetPublishedFileDetails(IntPtr _, PublishedFileId_t unPublishedFileId, uint unMaxSecondsOld)
    {
        Write("GetPublishedFileDetails");
        return default;
    }

    public SteamAPICall_t DeletePublishedFile(IntPtr _, PublishedFileId_t unPublishedFileId)
    {
        Write("DeletePublishedFile");
        return default;
    }

    public SteamAPICall_t EnumerateUserPublishedFiles(uint unStartIndex)
    {
        Write("EnumerateUserPublishedFiles");
        return default;
    }

    public SteamAPICall_t SubscribePublishedFile(IntPtr _, PublishedFileId_t unPublishedFileId)
    {
        Write("SubscribePublishedFile");
        return default;
    }

    public SteamAPICall_t EnumerateUserSubscribedFiles(uint unStartIndex)
    {
        Write("EnumerateUserSubscribedFiles");
        return default;
    }

    public SteamAPICall_t UnsubscribePublishedFile(IntPtr _, PublishedFileId_t unPublishedFileId)
    {
        Write("UnsubscribePublishedFile");
        return default;
    }

    public bool UpdatePublishedFileSetChangeDescription(IntPtr _, PublishedFileUpdateHandle_t updateHandle, string pchChangeDescription)
    {
        Write("UpdatePublishedFileSetChangeDescription");
        return default;
    }

    public SteamAPICall_t GetPublishedItemVoteDetails(IntPtr _, PublishedFileId_t unPublishedFileId)
    {
        Write("GetPublishedItemVoteDetails");
        return default;
    }

    public SteamAPICall_t UpdateUserPublishedItemVote(IntPtr _, PublishedFileId_t unPublishedFileId, bool bVoteUp)
    {
        Write("UpdateUserPublishedItemVote");
        return default;
    }

    public SteamAPICall_t GetUserPublishedItemVoteDetails(IntPtr _, PublishedFileId_t unPublishedFileId)
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

    public SteamAPICall_t SetUserPublishedFileAction(IntPtr _, PublishedFileId_t unPublishedFileId, EWorkshopFileAction eAction)
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

    public SteamAPICall_t UGCDownloadToLocation(IntPtr _, UGCHandle_t hContent, string pchLocation, uint unPriority)
    {
        Write("UGCDownloadToLocation");
        return default;
    }


    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}