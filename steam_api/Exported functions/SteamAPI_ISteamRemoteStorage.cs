using SKYNET;
using SKYNET.Steamworks;
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
        return SteamEmulator.SteamRemoteStorage.FileWrite(pchFile, pvData, cubData);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamRemoteStorage_FileRead(string pchFile, IntPtr pvData, int cubDataToRead)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileRead");
        return SteamEmulator.SteamRemoteStorage.FileRead(pchFile, pvData, cubDataToRead);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_FileWriteAsync(string pchFile, IntPtr pvData, uint cubData)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileWriteAsync");
        return SteamEmulator.SteamRemoteStorage.FileWriteAsync(pchFile, pvData, cubData);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_FileReadAsync(string pchFile, uint nOffset, uint cubToRead)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileReadAsync");
        return SteamEmulator.SteamRemoteStorage.FileReadAsync(pchFile, nOffset, cubToRead);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FileReadAsyncComplete(SteamAPICall_t hReadCall, IntPtr pvBuffer, uint cubToRead)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileReadAsyncComplete");
        return SteamEmulator.SteamRemoteStorage.FileReadAsyncComplete(SteamEmulator.SteamRemoteStorage.MemoryAddress, hReadCall, pvBuffer, cubToRead);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FileForget(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileForget");
        return SteamEmulator.SteamRemoteStorage.FileForget(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FileDelete(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileDelete");
        return SteamEmulator.SteamRemoteStorage.FileDelete(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_FileShare(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileShare");
        return SteamEmulator.SteamRemoteStorage.FileShare(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_SetSyncPlatforms(string pchFile, ERemoteStoragePlatform eRemoteStoragePlatform)
    {
        Write("SteamAPI_ISteamRemoteStorage_SetSyncPlatforms");
        return SteamEmulator.SteamRemoteStorage.SetSyncPlatforms(pchFile, eRemoteStoragePlatform);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static UGCFileWriteStreamHandle_t SteamAPI_ISteamRemoteStorage_FileWriteStreamOpen(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileWriteStreamOpen");
        return SteamEmulator.SteamRemoteStorage.FileWriteStreamOpen(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FileWriteStreamWriteChunk(UGCFileWriteStreamHandle_t writeHandle, IntPtr pvData, int cubData)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileWriteStreamWriteChunk");
        return SteamEmulator.SteamRemoteStorage.FileWriteStreamWriteChunk(SteamEmulator.SteamRemoteStorage.MemoryAddress, writeHandle, pvData, cubData);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FileWriteStreamClose(UGCFileWriteStreamHandle_t writeHandle)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileWriteStreamClose");
        return SteamEmulator.SteamRemoteStorage.FileWriteStreamClose(SteamEmulator.SteamRemoteStorage.MemoryAddress, writeHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FileWriteStreamCancel(UGCFileWriteStreamHandle_t writeHandle)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileWriteStreamCancel");
        return SteamEmulator.SteamRemoteStorage.FileWriteStreamCancel(SteamEmulator.SteamRemoteStorage.MemoryAddress, writeHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FileExists(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_FileExists");
        return SteamEmulator.SteamRemoteStorage.FileExists(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_FilePersisted(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_FilePersisted");
        return SteamEmulator.SteamRemoteStorage.FilePersisted(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamRemoteStorage_GetFileSize(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetFileSize");
        return SteamEmulator.SteamRemoteStorage.GetFileSize(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamRemoteStorage_GetFileTimestamp(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetFileTimestamp");
        return SteamEmulator.SteamRemoteStorage.GetFileTimestamp(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ERemoteStoragePlatform SteamAPI_ISteamRemoteStorage_GetSyncPlatforms(string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetSyncPlatforms");
        return SteamEmulator.SteamRemoteStorage.GetSyncPlatforms(pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamRemoteStorage_GetFileCount()
    {
        Write("SteamAPI_ISteamRemoteStorage_GetFileCount");
        return SteamEmulator.SteamRemoteStorage.GetFileCount(SteamEmulator.SteamRemoteStorage.MemoryAddress);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamRemoteStorage_GetFileNameAndSize(int iFile, int pnFileSizeInBytes)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetFileNameAndSize");
        return SteamEmulator.SteamRemoteStorage.GetFileNameAndSize(iFile, pnFileSizeInBytes);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_GetQuota(uint pnTotalBytes, uint puAvailableBytes)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetQuota");
        return SteamEmulator.SteamRemoteStorage.GetQuota(pnTotalBytes, puAvailableBytes);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_IsCloudEnabledForAccount()
    {
        Write("SteamAPI_ISteamRemoteStorage_IsCloudEnabledForAccount");
        return SteamEmulator.SteamRemoteStorage.IsCloudEnabledForAccount(SteamEmulator.SteamRemoteStorage.MemoryAddress);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_IsCloudEnabledForApp()
    {
        Write("SteamAPI_ISteamRemoteStorage_IsCloudEnabledForApp");
        return SteamEmulator.SteamRemoteStorage.IsCloudEnabledForApp(SteamEmulator.SteamRemoteStorage.MemoryAddress);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamRemoteStorage_SetCloudEnabledForApp(bool bEnabled)
    {
        Write("SteamAPI_ISteamRemoteStorage_SetCloudEnabledForApp");
        SteamEmulator.SteamRemoteStorage.SetCloudEnabledForApp(bEnabled);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_UGCDownload(UGCHandle_t hContent, uint unPriority)
    {
        Write("SteamAPI_ISteamRemoteStorage_UGCDownload");
        return SteamEmulator.SteamRemoteStorage.UGCDownload(SteamEmulator.SteamRemoteStorage.MemoryAddress, hContent, unPriority);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_GetUGCDownloadProgress(UGCHandle_t hContent, int pnBytesDownloaded, int pnBytesExpected)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetUGCDownloadProgress");
        return SteamEmulator.SteamRemoteStorage.GetUGCDownloadProgress(SteamEmulator.SteamRemoteStorage.MemoryAddress, hContent, pnBytesDownloaded, pnBytesExpected);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_GetUGCDetails(UGCHandle_t hContent, AppId_t pnAppID, string ppchName, int pnFileSizeInBytes, IntPtr pSteamIDOwner)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetUGCDetails");
        return SteamEmulator.SteamRemoteStorage.GetUGCDetails(SteamEmulator.SteamRemoteStorage.MemoryAddress, hContent, pnAppID, ppchName, pnFileSizeInBytes, pSteamIDOwner);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamRemoteStorage_UGCRead(UGCHandle_t hContent, IntPtr pvData, int cubDataToRead, uint cOffset, IntPtr eAction)
    {
        Write("SteamAPI_ISteamRemoteStorage_UGCRead");
        return SteamEmulator.SteamRemoteStorage.UGCRead(hContent, pvData, cubDataToRead, cOffset, eAction);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamRemoteStorage_GetCachedUGCCount()
    {
        Write("SteamAPI_ISteamRemoteStorage_GetCachedUGCCount");
        return SteamEmulator.SteamRemoteStorage.GetCachedUGCCount(SteamEmulator.SteamRemoteStorage.MemoryAddress);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static UGCHandle_t SteamAPI_ISteamRemoteStorage_GetCachedUGCHandle(int iCachedContent)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetCachedUGCHandle");
        return SteamEmulator.SteamRemoteStorage.GetCachedUGCHandle(iCachedContent);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_PublishWorkshopFile(string pchFile, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, IntPtr pTags, int int2)
    {
        Write("SteamAPI_ISteamRemoteStorage_PublishWorkshopFile");
        return SteamEmulator.SteamRemoteStorage.PublishWorkshopFile(pchFile, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, pTags, int2);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static PublishedFileUpdateHandle_t SteamAPI_ISteamRemoteStorage_CreatePublishedFileUpdateRequest(PublishedFileId_t unPublishedFileId)
    {
        Write("SteamAPI_ISteamRemoteStorage_CreatePublishedFileUpdateRequest");
        return SteamEmulator.SteamRemoteStorage.CreatePublishedFileUpdateRequest(SteamEmulator.SteamRemoteStorage.MemoryAddress, unPublishedFileId);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileFile(PublishedFileUpdateHandle_t updateHandle, string pchFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileFile");
        return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileFile(SteamEmulator.SteamRemoteStorage.MemoryAddress, updateHandle, pchFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFilePreviewFile(PublishedFileUpdateHandle_t updateHandle, string pchPreviewFile)
    {
        Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFilePreviewFile");
        return SteamEmulator.SteamRemoteStorage.UpdatePublishedFilePreviewFile(SteamEmulator.SteamRemoteStorage.MemoryAddress, updateHandle, pchPreviewFile);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTitle(PublishedFileUpdateHandle_t updateHandle, string pchTitle)
    {
        Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTitle");
        return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileTitle(SteamEmulator.SteamRemoteStorage.MemoryAddress, updateHandle, pchTitle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileDescription(PublishedFileUpdateHandle_t updateHandle, string pchDescription)
    {
        Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileDescription");
        return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileDescription(SteamEmulator.SteamRemoteStorage.MemoryAddress, updateHandle, pchDescription);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileVisibility(PublishedFileUpdateHandle_t updateHandle, ERemoteStoragePublishedFileVisibility eVisibility)
    {
        Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileVisibility");
        return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileVisibility(SteamEmulator.SteamRemoteStorage.MemoryAddress, updateHandle, eVisibility);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTags(PublishedFileUpdateHandle_t updateHandle, IntPtr pTags)
    {
        Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTags");
        return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileTags(SteamEmulator.SteamRemoteStorage.MemoryAddress, updateHandle, pTags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_CommitPublishedFileUpdate(PublishedFileUpdateHandle_t updateHandle)
    {
        Write("SteamAPI_ISteamRemoteStorage_CommitPublishedFileUpdate");
        return SteamEmulator.SteamRemoteStorage.CommitPublishedFileUpdate(SteamEmulator.SteamRemoteStorage.MemoryAddress, updateHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_GetPublishedFileDetails(PublishedFileId_t unPublishedFileId, uint unMaxSecondsOld)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetPublishedFileDetails");
        return SteamEmulator.SteamRemoteStorage.GetPublishedFileDetails(SteamEmulator.SteamRemoteStorage.MemoryAddress, unPublishedFileId, unMaxSecondsOld);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_DeletePublishedFile(PublishedFileId_t unPublishedFileId)
    {
        Write("SteamAPI_ISteamRemoteStorage_DeletePublishedFile");
        return SteamEmulator.SteamRemoteStorage.DeletePublishedFile(SteamEmulator.SteamRemoteStorage.MemoryAddress, unPublishedFileId);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_EnumerateUserPublishedFiles(uint unStartIndex)
    {
        Write("SteamAPI_ISteamRemoteStorage_EnumerateUserPublishedFiles");
        return SteamEmulator.SteamRemoteStorage.EnumerateUserPublishedFiles(unStartIndex);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_SubscribePublishedFile(PublishedFileId_t unPublishedFileId)
    {
        Write("SteamAPI_ISteamRemoteStorage_SubscribePublishedFile");
        return SteamEmulator.SteamRemoteStorage.SubscribePublishedFile(SteamEmulator.SteamRemoteStorage.MemoryAddress, unPublishedFileId);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_EnumerateUserSubscribedFiles(uint unStartIndex)
    {
        Write("SteamAPI_ISteamRemoteStorage_EnumerateUserSubscribedFiles");
        return SteamEmulator.SteamRemoteStorage.EnumerateUserSubscribedFiles(unStartIndex);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_UnsubscribePublishedFile(PublishedFileId_t unPublishedFileId)
    {
        Write("SteamAPI_ISteamRemoteStorage_UnsubscribePublishedFile");
        return SteamEmulator.SteamRemoteStorage.UnsubscribePublishedFile(SteamEmulator.SteamRemoteStorage.MemoryAddress, unPublishedFileId);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileSetChangeDescription(PublishedFileUpdateHandle_t updateHandle, string pchChangeDescription)
    {
        Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileSetChangeDescription");
        return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileSetChangeDescription(SteamEmulator.SteamRemoteStorage.MemoryAddress, updateHandle, pchChangeDescription);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_GetPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetPublishedItemVoteDetails");
        return SteamEmulator.SteamRemoteStorage.GetPublishedItemVoteDetails(SteamEmulator.SteamRemoteStorage.MemoryAddress, unPublishedFileId);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_UpdateUserPublishedItemVote(PublishedFileId_t unPublishedFileId, bool bVoteUp)
    {
        Write("SteamAPI_ISteamRemoteStorage_UpdateUserPublishedItemVote");
        return SteamEmulator.SteamRemoteStorage.UpdateUserPublishedItemVote(SteamEmulator.SteamRemoteStorage.MemoryAddress, unPublishedFileId, bVoteUp);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_GetUserPublishedItemVoteDetails(PublishedFileId_t unPublishedFileId)
    {
        Write("SteamAPI_ISteamRemoteStorage_GetUserPublishedItemVoteDetails");
        return SteamEmulator.SteamRemoteStorage.GetUserPublishedItemVoteDetails(SteamEmulator.SteamRemoteStorage.MemoryAddress, unPublishedFileId);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_EnumerateUserSharedWorkshopFiles(IntPtr steamId, uint unStartIndex, IntPtr pRequiredTags, IntPtr pExcludedTags)
    {
        Write("SteamAPI_ISteamRemoteStorage_EnumerateUserSharedWorkshopFiles");
        return SteamEmulator.SteamRemoteStorage.EnumerateUserSharedWorkshopFiles(steamId, unStartIndex, pRequiredTags, pExcludedTags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_PublishVideo(EWorkshopVideoProvider eVideoProvider, string pchVideoAccount, string pchVideoIdentifier, string pchPreviewFile, AppId_t nConsumerAppId, string pchTitle, string pchDescription, ERemoteStoragePublishedFileVisibility eVisibility, IntPtr pTags)
    {
        Write("SteamAPI_ISteamRemoteStorage_PublishVideo");
        return SteamEmulator.SteamRemoteStorage.PublishVideo(eVideoProvider, pchVideoAccount, pchVideoIdentifier, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, pTags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_SetUserPublishedFileAction(PublishedFileId_t unPublishedFileId, EWorkshopFileAction eAction)
    {
        Write("SteamAPI_ISteamRemoteStorage_SetUserPublishedFileAction");
        return SteamEmulator.SteamRemoteStorage.SetUserPublishedFileAction(SteamEmulator.SteamRemoteStorage.MemoryAddress, unPublishedFileId, eAction);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_EnumeratePublishedFilesByUserAction(EWorkshopFileAction eAction, uint unStartIndex)
    {
        Write("SteamAPI_ISteamRemoteStorage_EnumeratePublishedFilesByUserAction");
        return SteamEmulator.SteamRemoteStorage.EnumeratePublishedFilesByUserAction(eAction, unStartIndex);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_EnumeratePublishedWorkshopFiles(EWorkshopEnumerationType eEnumerationType, uint unStartIndex, uint unCount, uint unDays, IntPtr pTags, IntPtr pUserTags)
    {
        Write("SteamAPI_ISteamRemoteStorage_EnumeratePublishedWorkshopFiles");
        return SteamEmulator.SteamRemoteStorage.EnumeratePublishedWorkshopFiles(eEnumerationType, unStartIndex, unCount, unDays, pTags, pUserTags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_UGCDownloadToLocation(UGCHandle_t hContent, string pchLocation, uint unPriority)
    {
        Write("SteamAPI_ISteamRemoteStorage_UGCDownloadToLocation");
        return SteamEmulator.SteamRemoteStorage.UGCDownloadToLocation(SteamEmulator.SteamRemoteStorage.MemoryAddress, hContent, pchLocation, unPriority);
    }

}

