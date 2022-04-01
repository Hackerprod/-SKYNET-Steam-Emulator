using SKYNET.Delegate.Helper;
using SKYNET.Steamworks;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamRemoteStorage")]
    public class DSteamRemoteStorage 
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool FileWrite(string pchFile, IntPtr pvData, int cubData);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int FileRead(string pchFile, IntPtr pvData, int cubDataToRead);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t FileWriteAsync(string pchFile, IntPtr pvData, uint cubData);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t FileReadAsync(string pchFile, uint nOffset, uint cubToRead);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool FileReadAsyncComplete(IntPtr _, SteamAPICall_t hReadCall, IntPtr pvBuffer, uint cubToRead);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool FileForget(string pchFile);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool FileDelete(string pchFile);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t FileShare(string pchFile);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetSyncPlatforms(string pchFile, ERemoteStoragePlatform eRemoteStoragePlatform);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate UGCFileWriteStreamHandle_t FileWriteStreamOpen(string pchFile);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool FileWriteStreamWriteChunk(IntPtr _, UGCFileWriteStreamHandle_t writeHandle, IntPtr pvData, int cubData);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool FileWriteStreamClose(IntPtr _, UGCFileWriteStreamHandle_t writeHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool FileWriteStreamCancel(IntPtr _, UGCFileWriteStreamHandle_t writeHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool FileExists(string pchFile);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool FilePersisted(string pchFile);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFileSize(string pchFile);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetFileTimestamp(string pchFile);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ERemoteStoragePlatform GetSyncPlatforms(string pchFile);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFileCount(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetFileNameAndSize(int iFile, int pnFileSizeInBytes);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetQuota(uint pnTotalBytes, uint puAvailableBytes);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsCloudEnabledForAccount(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsCloudEnabledForApp(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetCloudEnabledForApp(bool bEnabled);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t UGCDownload(IntPtr _, UGCHandle_t hContent, uint unPriority);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetUGCDownloadProgress(IntPtr _, UGCHandle_t hContent, int pnBytesDownloaded, int pnBytesExpected);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetUGCDetails(IntPtr _, UGCHandle_t hContent, AppId_t pnAppID, string ppchName, int pnFileSizeInBytes, IntPtr pSteamIDOwner);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int UGCRead(UGCHandle_t hContent, IntPtr pvData, int cubDataToRead, uint cOffset, IntPtr eAction);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetCachedUGCCount(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate UGCHandle_t GetCachedUGCHandle(int iCachedContent);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t PublishWorkshopFile(string pchFile, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, IntPtr pTags, int int2);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate PublishedFileUpdateHandle_t CreatePublishedFileUpdateRequest(IntPtr _, PublishedFileId_t unPublishedFileId);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdatePublishedFileFile(IntPtr _, PublishedFileUpdateHandle_t updateHandle, string pchFile);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdatePublishedFilePreviewFile(IntPtr _, PublishedFileUpdateHandle_t updateHandle, string pchPreviewFile);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdatePublishedFileTitle(IntPtr _, PublishedFileUpdateHandle_t updateHandle, string pchTitle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdatePublishedFileDescription(IntPtr _, PublishedFileUpdateHandle_t updateHandle, string pchDescription);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdatePublishedFileVisibility(IntPtr _, PublishedFileUpdateHandle_t updateHandle, ERemoteStoragePublishedFileVisibility eVisibility);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdatePublishedFileTags(IntPtr _, PublishedFileUpdateHandle_t updateHandle, IntPtr pTags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t CommitPublishedFileUpdate(IntPtr _, PublishedFileUpdateHandle_t updateHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t GetPublishedFileDetails(IntPtr _, PublishedFileId_t unPublishedFileId, uint unMaxSecondsOld);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t DeletePublishedFile(IntPtr _, PublishedFileId_t unPublishedFileId);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t EnumerateUserPublishedFiles(uint unStartIndex);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t SubscribePublishedFile(IntPtr _, PublishedFileId_t unPublishedFileId);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t EnumerateUserSubscribedFiles(uint unStartIndex);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t UnsubscribePublishedFile(IntPtr _, PublishedFileId_t unPublishedFileId);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdatePublishedFileSetChangeDescription(IntPtr _, PublishedFileUpdateHandle_t updateHandle, string pchChangeDescription);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t GetPublishedItemVoteDetails(IntPtr _, PublishedFileId_t unPublishedFileId);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t UpdateUserPublishedItemVote(IntPtr _, PublishedFileId_t unPublishedFileId, bool bVoteUp);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t GetUserPublishedItemVoteDetails(IntPtr _, PublishedFileId_t unPublishedFileId);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t EnumerateUserSharedWorkshopFiles(IntPtr steamId, uint unStartIndex, IntPtr pRequiredTags, IntPtr pExcludedTags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t PublishVideo(EWorkshopVideoProvider eVideoProvider, string pchVideoAccount, string pchVideoIdentifier, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, IntPtr pTags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t SetUserPublishedFileAction(IntPtr _, PublishedFileId_t unPublishedFileId, EWorkshopFileAction eAction);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t EnumeratePublishedFilesByUserAction(EWorkshopFileAction eAction, uint unStartIndex);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t EnumeratePublishedWorkshopFiles(EWorkshopEnumerationType eEnumerationType, uint unStartIndex, uint unCount, uint unDays, IntPtr pTags, IntPtr pUserTags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t UGCDownloadToLocation(IntPtr _, UGCHandle_t hContent, string pchLocation, uint unPriority);

    }
}
