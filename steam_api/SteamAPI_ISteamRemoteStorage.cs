using SKYNET.Interface;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class SteamAPI_ISteamRemoteStorage : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FileWrite(string pchFile, IntPtr pvData, int cubData)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileWrite");
        return SteamClient.SteamRemoteStorage.FileWrite(pchFile, pvData, cubData);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamRemoteStorage_FileRead(string pchFile, IntPtr pvData, int cubDataToRead)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileRead");
        return SteamClient.SteamRemoteStorage.FileRead(pchFile, pvData, cubDataToRead);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_FileWriteAsync(string pchFile, IntPtr pvData, uint cubData)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileWriteAsync");
        return SteamClient.SteamRemoteStorage.FileWriteAsync(pchFile, pvData, cubData);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_FileReadAsync(string pchFile, uint nOffset, uint cubToRead)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileReadAsync");
        return SteamClient.SteamRemoteStorage.FileReadAsync(pchFile, nOffset, cubToRead);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FileReadAsyncComplete(SteamAPICall_t hReadCall, IntPtr pvBuffer, uint cubToRead)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileReadAsyncComplete");
        return SteamClient.SteamRemoteStorage.FileReadAsyncComplete(hReadCall, pvBuffer, cubToRead);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FileForget(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileForget");
        return SteamClient.SteamRemoteStorage.FileForget(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FileDelete(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileDelete");
        return SteamClient.SteamRemoteStorage.FileDelete(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_FileShare(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileShare");
        return SteamClient.SteamRemoteStorage.FileShare(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_SetSyncPlatforms(string pchFile, ERemoteStoragePlatform eRemoteStoragePlatform)
    {
        Write("SteamAPI_ISteamRemoteStorage_SetSyncPlatforms");
        return SteamClient.SteamRemoteStorage.SetSyncPlatforms(pchFile, eRemoteStoragePlatform);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static UGCFileWriteStreamHandle_t SteamAPI_ISteamRemoteStorage_FileWriteStreamOpen(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileWriteStreamOpen");
        return SteamClient.SteamRemoteStorage.FileWriteStreamOpen(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FileWriteStreamWriteChunk(UGCFileWriteStreamHandle_t writeHandle, IntPtr pvData, int cubData)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileWriteStreamWriteChunk");
        return SteamClient.SteamRemoteStorage.FileWriteStreamWriteChunk(writeHandle, pvData, cubData);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FileWriteStreamClose(UGCFileWriteStreamHandle_t writeHandle)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileWriteStreamClose");
        return SteamClient.SteamRemoteStorage.FileWriteStreamClose(writeHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FileWriteStreamCancel(UGCFileWriteStreamHandle_t writeHandle)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileWriteStreamCancel");
        return SteamClient.SteamRemoteStorage.FileWriteStreamCancel(writeHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FileExists(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileExists");
        return SteamClient.SteamRemoteStorage.FileExists(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FilePersisted(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_FilePersisted");
        return SteamClient.SteamRemoteStorage.FilePersisted(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamRemoteStorage_GetFileSize(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetFileSize");
        return SteamClient.SteamRemoteStorage.GetFileSize(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamRemoteStorage_GetFileTimestamp(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetFileTimestamp");
        return SteamClient.SteamRemoteStorage.GetFileTimestamp(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ERemoteStoragePlatform SteamAPI_ISteamRemoteStorage_GetSyncPlatforms(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetSyncPlatforms");
        return SteamClient.SteamRemoteStorage.GetSyncPlatforms(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamRemoteStorage_GetFileCount()
    {
        Write("SteamAPI_ISteamRemoteStorage_GetFileCount");
        return SteamClient.SteamRemoteStorage.GetFileCount();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamRemoteStorage_GetFileNameAndSize(int iFile, int pnFileSizeInBytes)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetFileNameAndSize");
        return SteamClient.SteamRemoteStorage.GetFileNameAndSize(iFile, pnFileSizeInBytes);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_GetQuota(uint pnTotalBytes, uint puAvailableBytes)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetQuota");
        return SteamClient.SteamRemoteStorage.GetQuota(pnTotalBytes, puAvailableBytes);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_IsCloudEnabledForAccount()
    {
        Write("SteamAPI_ISteamRemoteStorage_IsCloudEnabledForAccount");
        return SteamClient.SteamRemoteStorage.IsCloudEnabledForAccount();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_IsCloudEnabledForApp()
    {
        Write("SteamAPI_ISteamRemoteStorage_IsCloudEnabledForApp");
        return SteamClient.SteamRemoteStorage.IsCloudEnabledForApp();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamRemoteStorage_SetCloudEnabledForApp(bool bEnabled)
    {
        Write("SteamAPI_ISteamRemoteStorage_SetCloudEnabledForApp");
        SteamClient.SteamRemoteStorage.SetCloudEnabledForApp(bEnabled);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_UGCDownload(UGCHandle_t hContent, uint unPriority)
    {
        Write("SteamAPI_ISteamRemoteStorage_UGCDownload");
        return SteamClient.SteamRemoteStorage.UGCDownload(hContent, unPriority);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_GetUGCDownloadProgress(UGCHandle_t hContent, int pnBytesDownloaded, int pnBytesExpected)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetUGCDownloadProgress");
        return SteamClient.SteamRemoteStorage.GetUGCDownloadProgress(hContent, pnBytesDownloaded, pnBytesExpected);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_GetUGCDetails(UGCHandle_t hContent, AppId_t pnAppID, string ppchName, int pnFileSizeInBytes, IntPtr pSteamIDOwner)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetUGCDetails");
        return SteamClient.SteamRemoteStorage.GetUGCDetails(hContent, pnAppID, ppchName, pnFileSizeInBytes, pSteamIDOwner);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamRemoteStorage_UGCRead(UGCHandle_t hContent, IntPtr pvData, int cubDataToRead, uint cOffset, IntPtr eAction)
    {
        Write("SteamAPI_ISteamRemoteStorage_UGCRead");
        return SteamClient.SteamRemoteStorage.UGCRead(hContent, pvData, cubDataToRead, cOffset, eAction);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamRemoteStorage_GetCachedUGCCount()
    {
        Write("SteamAPI_ISteamRemoteStorage_GetCachedUGCCount");
        return SteamClient.SteamRemoteStorage.GetCachedUGCCount();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static UGCHandle_t SteamAPI_ISteamRemoteStorage_GetCachedUGCHandle(int iCachedContent)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetCachedUGCHandle");
        return SteamClient.SteamRemoteStorage.GetCachedUGCHandle(iCachedContent);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_PublishWorkshopFile(string pchFile, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, IntPtr pTags, int int2)
    {
        Write("SteamAPI_ISteamRemoteStorage_PublishWorkshopFile");
        return SteamClient.SteamRemoteStorage.PublishWorkshopFile(pchFile, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, pTags, int2);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static PublishedFileUpdateHandle_t SteamAPI_ISteamRemoteStorage_CreatePublishedFileUpdateRequest(PublishedFileId_t unPublishedFileId)
    {
        Write("SteamAPI_ISteamRemoteStorage_CreatePublishedFileUpdateRequest");
        return SteamClient.SteamRemoteStorage.CreatePublishedFileUpdateRequest(unPublishedFileId);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileFile(PublishedFileUpdateHandle_t updateHandle, string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileFile");
        return SteamClient.SteamRemoteStorage.UpdatePublishedFileFile(updateHandle, pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFilePreviewFile(PublishedFileUpdateHandle_t updateHandle, string pchPreviewFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFilePreviewFile");
        return SteamClient.SteamRemoteStorage.UpdatePublishedFilePreviewFile(updateHandle, pchPreviewFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTitle(PublishedFileUpdateHandle_t updateHandle, string pchTitle)
    {
        Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTitle");
        return SteamClient.SteamRemoteStorage.UpdatePublishedFileTitle(updateHandle, pchTitle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileDescription(PublishedFileUpdateHandle_t updateHandle, string pchDescription)
    {
        Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileDescription");
        return SteamClient.SteamRemoteStorage.UpdatePublishedFileDescription(updateHandle, pchDescription);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileVisibility(PublishedFileUpdateHandle_t updateHandle, ERemoteStoragePublishedFileVisibility eVisibility)
    {
        Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileVisibility");
        return SteamClient.SteamRemoteStorage.UpdatePublishedFileVisibility(updateHandle, eVisibility);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTags(PublishedFileUpdateHandle_t updateHandle, IntPtr pTags)
    {
        Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTags");
        return SteamClient.SteamRemoteStorage.UpdatePublishedFileTags(updateHandle, pTags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_CommitPublishedFileUpdate(PublishedFileUpdateHandle_t updateHandle)
    {
        Write("SteamAPI_ISteamRemoteStorage_CommitPublishedFileUpdate");
        return SteamClient.SteamRemoteStorage.CommitPublishedFileUpdate(updateHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_GetPublishedFileDetails(PublishedFileId_t unPublishedFileId, uint unMaxSecondsOld)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetPublishedFileDetails");
        return SteamClient.SteamRemoteStorage.GetPublishedFileDetails(unPublishedFileId, unMaxSecondsOld);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_DeletePublishedFile(PublishedFileId_t unPublishedFileId)
    {
        Write("SteamAPI_ISteamRemoteStorage_DeletePublishedFile");
        return SteamClient.SteamRemoteStorage.DeletePublishedFile(unPublishedFileId);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_EnumerateUserPublishedFiles(uint unStartIndex)
    {
        Write("SteamAPI_ISteamRemoteStorage_EnumerateUserPublishedFiles");
        return SteamClient.SteamRemoteStorage.EnumerateUserPublishedFiles(unStartIndex);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_SubscribePublishedFile(PublishedFileId_t unPublishedFileId)
    {
        Write("SteamAPI_ISteamRemoteStorage_SubscribePublishedFile");
        return SteamClient.SteamRemoteStorage.SubscribePublishedFile(unPublishedFileId);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_EnumerateUserSubscribedFiles(uint unStartIndex)
    {
        Write("SteamAPI_ISteamRemoteStorage_EnumerateUserSubscribedFiles");
        return SteamClient.SteamRemoteStorage.EnumerateUserSubscribedFiles(unStartIndex);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_UnsubscribePublishedFile(PublishedFileId_t unPublishedFileId)
    {
        Write("SteamAPI_ISteamRemoteStorage_UnsubscribePublishedFile");
        return SteamClient.SteamRemoteStorage.UnsubscribePublishedFile(unPublishedFileId);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileSetChangeDescription(PublishedFileUpdateHandle_t updateHandle, string pchChangeDescription)
    {
        Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileSetChangeDescription");
        return SteamClient.SteamRemoteStorage.UpdatePublishedFileSetChangeDescription(updateHandle, pchChangeDescription);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_GetPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetPublishedItemVoteDetails");
        return SteamClient.SteamRemoteStorage.GetPublishedItemVoteDetails(unPublishedFileId);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_UpdateUserPublishedItemVote(PublishedFileId_t unPublishedFileId, bool bVoteUp)
    {
        Write("SteamAPI_ISteamRemoteStorage_UpdateUserPublishedItemVote");
        return SteamClient.SteamRemoteStorage.UpdateUserPublishedItemVote(unPublishedFileId, bVoteUp);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_GetUserPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetUserPublishedItemVoteDetails");
        return SteamClient.SteamRemoteStorage.GetUserPublishedItemVoteDetails(unPublishedFileId);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_EnumerateUserSharedWorkshopFiles(IntPtr steamId, uint unStartIndex, IntPtr pRequiredTags, IntPtr pExcludedTags)
    {
        Write("SteamAPI_ISteamRemoteStorage_EnumerateUserSharedWorkshopFiles");
        return SteamClient.SteamRemoteStorage.EnumerateUserSharedWorkshopFiles(steamId, unStartIndex, pRequiredTags, pExcludedTags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_PublishVideo(EWorkshopVideoProvider eVideoProvider, string pchVideoAccount, string pchVideoIdentifier, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, IntPtr pTags)
    {
        Write("SteamAPI_ISteamRemoteStorage_PublishVideo");
        return SteamClient.SteamRemoteStorage.PublishVideo(eVideoProvider, pchVideoAccount, pchVideoIdentifier, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, pTags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_SetUserPublishedFileAction(PublishedFileId_t unPublishedFileId, EWorkshopFileAction eAction)
    {
        Write("SteamAPI_ISteamRemoteStorage_SetUserPublishedFileAction");
        return SteamClient.SteamRemoteStorage.SetUserPublishedFileAction(unPublishedFileId, eAction);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_EnumeratePublishedFilesByUserAction(EWorkshopFileAction eAction, uint unStartIndex)
    {
        Write("SteamAPI_ISteamRemoteStorage_EnumeratePublishedFilesByUserAction");
        return SteamClient.SteamRemoteStorage.EnumeratePublishedFilesByUserAction(eAction, unStartIndex);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_EnumeratePublishedWorkshopFiles(EWorkshopEnumerationType eEnumerationType, uint unStartIndex, uint unCount, uint unDays, IntPtr pTags, IntPtr pUserTags)
    {
        Write("SteamAPI_ISteamRemoteStorage_EnumeratePublishedWorkshopFiles");
        return SteamClient.SteamRemoteStorage.EnumeratePublishedWorkshopFiles(eEnumerationType, unStartIndex, unCount, unDays, pTags, pUserTags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_UGCDownloadToLocation(UGCHandle_t hContent, string pchLocation, uint unPriority)
    {
        Write("SteamAPI_ISteamRemoteStorage_UGCDownloadToLocation");
        return SteamClient.SteamRemoteStorage.UGCDownloadToLocation(hContent, pchLocation, unPriority);
    }

}

