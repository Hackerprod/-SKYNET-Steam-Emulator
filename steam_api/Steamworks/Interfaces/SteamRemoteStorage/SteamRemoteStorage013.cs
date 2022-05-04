using Steamworks;
using System;

using SteamAPICall_t = System.UInt64;
using UGCFileWriteStreamHandle_t = System.UInt64;

namespace SKYNET.Interface
{
    [Interface("STEAMREMOTESTORAGE_INTERFACE_VERSION013")]
    public class SteamRemoteStorage013 : ISteamInterface
    {
        public bool FileWrite(IntPtr _, string pchFile, string pvData, int cubData)
        {
            return SteamEmulator.SteamRemoteStorage.FileWrite(pchFile, pvData, cubData);
        }

        public int FileRead(IntPtr _, string pchFile, IntPtr pvData, int cubDataToRead)
        {
            return SteamEmulator.SteamRemoteStorage.FileRead(pchFile, pvData, cubDataToRead);
        }

        public SteamAPICall_t FileWriteAsync(IntPtr _, string pchFile, string pvData, uint cubData)
        {
            return SteamEmulator.SteamRemoteStorage.FileWriteAsync(pchFile, pvData, cubData);
        }

        public SteamAPICall_t FileReadAsync(IntPtr _, string pchFile, uint nOffset, uint cubToRead)
        {
            return SteamEmulator.SteamRemoteStorage.FileReadAsync(pchFile, nOffset, cubToRead);
        }

        public bool FileReadAsyncComplete(IntPtr _, ulong hReadCall, IntPtr pvBuffer, uint cubToRead)
        {
            return SteamEmulator.SteamRemoteStorage.FileReadAsyncComplete(hReadCall, pvBuffer, cubToRead);
        }

        public bool FileForget(IntPtr _, string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.FileForget(pchFile);
        }

        public bool FileDelete(IntPtr _, string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.FileDelete(pchFile);
        }

        public SteamAPICall_t FileShare(IntPtr _, string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.FileShare(pchFile);
        }

        public bool SetSyncPlatforms(IntPtr _, string pchFile, int eRemoteStoragePlatform)
        {
            return SteamEmulator.SteamRemoteStorage.SetSyncPlatforms(pchFile, eRemoteStoragePlatform);
        }

        public UGCFileWriteStreamHandle_t FileWriteStreamOpen(IntPtr _, string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.FileWriteStreamOpen(pchFile);
        }

        public bool FileWriteStreamWriteChunk(IntPtr _, ulong writeHandle, IntPtr pvData, int cubData)
        {
            return SteamEmulator.SteamRemoteStorage.FileWriteStreamWriteChunk(writeHandle, pvData, cubData);
        }

        public bool FileWriteStreamClose(IntPtr _, ulong writeHandle)
        {
            return SteamEmulator.SteamRemoteStorage.FileWriteStreamClose(writeHandle);
        }

        public bool FileWriteStreamCancel(IntPtr _, ulong writeHandle)
        {
            return SteamEmulator.SteamRemoteStorage.FileWriteStreamCancel(writeHandle);
        }

        public bool FileExists(IntPtr _, string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.FileExists(pchFile);
        }

        public bool FilePersisted(IntPtr _, string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.FilePersisted(pchFile);
        }

        public int GetFileSize(IntPtr _, string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.GetFileSize(pchFile);
        }

        public long GetFileTimestamp(IntPtr _, string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.GetFileTimestamp(pchFile);
        }

        public int GetSyncPlatforms(IntPtr _, string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.GetSyncPlatforms(pchFile);
        }

        public int GetFileCount(IntPtr _)
        {
            return SteamEmulator.SteamRemoteStorage.GetFileCount();
        }

        public string GetFileNameAndSize(IntPtr _, int iFile, ref int pnFileSizeInBytes)
        {
            return SteamEmulator.SteamRemoteStorage.GetFileNameAndSize(iFile, ref pnFileSizeInBytes);
        }

        public bool GetQuota(IntPtr _, ref ulong pnTotalBytes, ref ulong puAvailableBytes)
        {
            return SteamEmulator.SteamRemoteStorage.GetQuota(ref pnTotalBytes, ref puAvailableBytes);
        }

        public bool IsCloudEnabledForAccount(IntPtr _)
        {
            return SteamEmulator.SteamRemoteStorage.IsCloudEnabledForAccount();
        }

        public bool IsCloudEnabledForApp(IntPtr _)
        {
            return SteamEmulator.SteamRemoteStorage.IsCloudEnabledForApp();
        }

        public void SetCloudEnabledForApp(IntPtr _, bool bEnabled)
        {
            SteamEmulator.SteamRemoteStorage.SetCloudEnabledForApp(bEnabled);
        }

        public ulong UGCDownload(IntPtr _, ulong hContent, uint unPriority)
        {
            return SteamEmulator.SteamRemoteStorage.UGCDownload(hContent, unPriority);
        }

        public bool GetUGCDownloadProgress(IntPtr _, ulong hContent, int pnBytesDownloaded, int pnBytesExpected)
        {
            return SteamEmulator.SteamRemoteStorage.GetUGCDownloadProgress(hContent, pnBytesDownloaded, pnBytesExpected);
        }

        public bool GetUGCDetails(ulong hContent, uint pnAppID, string ppchName, int pnFileSizeInBytes, ulong pSteamIDOwner)
        {
            return SteamEmulator.SteamRemoteStorage.GetUGCDetails(hContent, pnAppID, ppchName, pnFileSizeInBytes, pSteamIDOwner);
        }

        public int UGCRead(IntPtr _, ulong hContent, IntPtr pvData, int cubDataToRead, uint cOffset, IntPtr eAction)
        {
            return SteamEmulator.SteamRemoteStorage.UGCRead(hContent, pvData, cubDataToRead, cOffset, eAction);
        }

        public int GetCachedUGCCount(IntPtr _)
        {
            return SteamEmulator.SteamRemoteStorage.GetCachedUGCCount();
        }

        public ulong PublishWorkshopFile(IntPtr _, string pchFile, string pchPreviewFile, uint nConsumerAppId, string pchTitle, string pchDescription, int eVisibility, IntPtr pTags, int eWorkshopFileType)
        {
            return SteamEmulator.SteamRemoteStorage.PublishWorkshopFile(pchFile, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, pTags, eWorkshopFileType);
        }

        public ulong CreatePublishedFileUpdateRequest(IntPtr _, ulong unPublishedFileId)
        {
            return SteamEmulator.SteamRemoteStorage.CreatePublishedFileUpdateRequest(unPublishedFileId);
        }

        public bool UpdatePublishedFileFile(IntPtr _, ulong updateHandle, string pchFile)
        {
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileFile(updateHandle, pchFile);
        }

        public bool UpdatePublishedFilePreviewFile(IntPtr _, ulong updateHandle, string pchPreviewFile)
        {
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFilePreviewFile(updateHandle, pchPreviewFile);
        }

        public bool UpdatePublishedFileTitle(IntPtr _, ulong updateHandle, string pchTitle)
        {
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileTitle(updateHandle, pchTitle);
        }

        public bool UpdatePublishedFileDescription(IntPtr _, ulong updateHandle, string pchDescription)
        {
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileDescription(updateHandle, pchDescription);
        }

        public bool UpdatePublishedFileVisibility(IntPtr _, ulong updateHandle, int eVisibility)
        {
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileVisibility(updateHandle, eVisibility);
        }

        public bool UpdatePublishedFileTags(IntPtr _, ulong updateHandle, IntPtr pTags)
        {
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileTags(updateHandle, pTags);
        }

        public ulong CommitPublishedFileUpdate(IntPtr _, ulong updateHandle)
        {
            return SteamEmulator.SteamRemoteStorage.CommitPublishedFileUpdate(updateHandle);
        }

        public ulong GetPublishedFileDetails(IntPtr _, ulong unPublishedFileId, uint unMaxSecondsOld)
        {
            return SteamEmulator.SteamRemoteStorage.GetPublishedFileDetails(unPublishedFileId, unMaxSecondsOld);
        }

        public ulong DeletePublishedFile(IntPtr _, ulong unPublishedFileId)
        {
            return SteamEmulator.SteamRemoteStorage.DeletePublishedFile(unPublishedFileId);
        }

        public SteamAPICall_t EnumerateUserPublishedFiles(IntPtr _, uint unStartIndex)
        {
            return SteamEmulator.SteamRemoteStorage.EnumerateUserPublishedFiles(unStartIndex);
        }

        public ulong SubscribePublishedFile(IntPtr _, ulong unPublishedFileId)
        {
            return SteamEmulator.SteamRemoteStorage.SubscribePublishedFile(unPublishedFileId);
        }

        public ulong EnumerateUserSubscribedFiles(IntPtr _, uint unStartIndex)
        {
            return SteamEmulator.SteamRemoteStorage.EnumerateUserSubscribedFiles(unStartIndex);
        }

        public ulong UnsubscribePublishedFile(IntPtr _, ulong unPublishedFileId)
        {
            return SteamEmulator.SteamRemoteStorage.UnsubscribePublishedFile(unPublishedFileId);
        }

        public bool UpdatePublishedFileSetChangeDescription(IntPtr _, ulong updateHandle, string pchChangeDescription)
        {
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileSetChangeDescription(updateHandle, pchChangeDescription);
        }

        public ulong GetPublishedItemVoteDetails(IntPtr _, ulong unPublishedFileId)
        {
            return SteamEmulator.SteamRemoteStorage.GetPublishedItemVoteDetails(unPublishedFileId);
        }

        public ulong UpdateUserPublishedItemVote(IntPtr _, ulong unPublishedFileId, bool bVoteUp)
        {
            return SteamEmulator.SteamRemoteStorage.UpdateUserPublishedItemVote(unPublishedFileId, bVoteUp);
        }

        public ulong GetUserPublishedItemVoteDetails(IntPtr _, ulong unPublishedFileId)
        {
            return SteamEmulator.SteamRemoteStorage.GetUserPublishedItemVoteDetails(unPublishedFileId);
        }

        public SteamAPICall_t EnumerateUserSharedWorkshopFiles(IntPtr _, ulong steamId, uint unStartIndex, IntPtr pRequiredTags, IntPtr pExcludedTags)
        {
            return SteamEmulator.SteamRemoteStorage.EnumerateUserSharedWorkshopFiles(steamId, unStartIndex, pRequiredTags, pExcludedTags);
        }

        public ulong PublishVideo(IntPtr _, int eVideoProvider, string pchVideoAccount, string pchVideoIdentifier, string pchPreviewFile, uint nConsumerAppId, string pchTitle, string pchDescription, int eVisibility, IntPtr pTags)
        {
            return SteamEmulator.SteamRemoteStorage.PublishVideo(eVideoProvider, pchVideoAccount, pchVideoIdentifier, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, pTags);
        }

        public ulong SetUserPublishedFileAction(IntPtr _, ulong unPublishedFileId, int eAction)
        {
            return SteamEmulator.SteamRemoteStorage.SetUserPublishedFileAction(unPublishedFileId, eAction);
        }

        public ulong EnumeratePublishedFilesByUserAction(IntPtr _, int eAction, uint unStartIndex)
        {
            return SteamEmulator.SteamRemoteStorage.EnumeratePublishedFilesByUserAction(eAction, unStartIndex);
        }

        public ulong EnumeratePublishedWorkshopFiles(IntPtr _, int eEnumerationType, uint unStartIndex, uint unCount, uint unDays, IntPtr pTags, IntPtr pUserTags)
        {
            return SteamEmulator.SteamRemoteStorage.EnumeratePublishedWorkshopFiles(eEnumerationType, unStartIndex, unCount, unDays, pTags, pUserTags);
        }

        public ulong UGCDownloadToLocation(IntPtr _, ulong hContent, string pchLocation, uint unPriority)
        {
            return SteamEmulator.SteamRemoteStorage.UGCDownloadToLocation(hContent, pchLocation, unPriority);
        }
    }
}
