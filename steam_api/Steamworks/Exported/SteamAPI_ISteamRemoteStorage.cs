using System;
using System.Runtime.InteropServices;
using SKYNET.Managers;

namespace SKYNET.Steamworks.Exported
{
    using SteamAPICall_t = System.UInt64;
    using UGCFileWriteStreamHandle_t = System.UInt64;
    using UGCHandle_t = System.UInt64;
    using PublishedFileUpdateHandle_t = System.UInt64;
    public class SteamAPI_ISteamRemoteStorage
    {
        static SteamAPI_ISteamRemoteStorage()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_FileWrite(IntPtr _, string pchFile, IntPtr pvData, int cubData)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileWrite");
            return SteamEmulator.SteamRemoteStorage.FileWrite(pchFile, pvData, cubData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamRemoteStorage_FileRead(IntPtr _, string pchFile, IntPtr pvData, int cubDataToRead)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileRead");
            return SteamEmulator.SteamRemoteStorage.FileRead(pchFile, pvData, cubDataToRead);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_FileWriteAsync(IntPtr _, string pchFile, IntPtr pvData, uint cubData)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileWriteAsync");
            return SteamEmulator.SteamRemoteStorage.FileWriteAsync(pchFile, pvData, cubData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_FileReadAsync(IntPtr _, string pchFile, uint nOffset, uint cubToRead)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileReadAsync");
            return SteamEmulator.SteamRemoteStorage.FileReadAsync(pchFile, nOffset, cubToRead);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_FileReadAsyncComplete(IntPtr _, SteamAPICall_t hReadCall, IntPtr pvBuffer, uint cubToRead)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileReadAsyncComplete");
            return SteamEmulator.SteamRemoteStorage.FileReadAsyncComplete(hReadCall, pvBuffer, cubToRead);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_FileForget(IntPtr _, string pchFile)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileForget");
            return SteamEmulator.SteamRemoteStorage.FileForget(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_FileDelete(IntPtr _, string pchFile)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileDelete");
            return SteamEmulator.SteamRemoteStorage.FileDelete(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_FileShare(IntPtr _, string pchFile)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileShare");
            return SteamEmulator.SteamRemoteStorage.FileShare(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_SetSyncPlatforms(IntPtr _, string pchFile, int eRemoteStoragePlatform)
        {
            Write("SteamAPI_ISteamRemoteStorage_SetSyncPlatforms");
            return SteamEmulator.SteamRemoteStorage.SetSyncPlatforms(pchFile, eRemoteStoragePlatform);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static UGCFileWriteStreamHandle_t SteamAPI_ISteamRemoteStorage_FileWriteStreamOpen(IntPtr _, string pchFile)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileWriteStreamOpen");
            return SteamEmulator.SteamRemoteStorage.FileWriteStreamOpen(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_FileWriteStreamWriteChunk(IntPtr _, UGCFileWriteStreamHandle_t writeHandle, IntPtr pvData, int cubData)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileWriteStreamWriteChunk");
            return SteamEmulator.SteamRemoteStorage.FileWriteStreamWriteChunk(writeHandle, pvData, cubData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_FileWriteStreamClose(IntPtr _, UGCFileWriteStreamHandle_t writeHandle)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileWriteStreamClose");
            return SteamEmulator.SteamRemoteStorage.FileWriteStreamClose(writeHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_FileWriteStreamCancel(IntPtr _, UGCFileWriteStreamHandle_t writeHandle)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileWriteStreamCancel");
            return SteamEmulator.SteamRemoteStorage.FileWriteStreamCancel(writeHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_FileExists(IntPtr _, string pchFile)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileExists");
            return SteamEmulator.SteamRemoteStorage.FileExists(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_FilePersisted(IntPtr _, string pchFile)
        {
            Write("SteamAPI_ISteamRemoteStorage_FilePersisted");
            return SteamEmulator.SteamRemoteStorage.FilePersisted(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamRemoteStorage_GetFileSize(IntPtr _, string pchFile)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetFileSize");
            return SteamEmulator.SteamRemoteStorage.GetFileSize(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamRemoteStorage_GetFileTimestamp(IntPtr _, string pchFile)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetFileTimestamp");
            return SteamEmulator.SteamRemoteStorage.GetFileTimestamp(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamRemoteStorage_GetSyncPlatforms(IntPtr _, string pchFile)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetSyncPlatforms");
            return SteamEmulator.SteamRemoteStorage.GetSyncPlatforms(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamRemoteStorage_GetFileCount(IntPtr _)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetFileCount");
            return SteamEmulator.SteamRemoteStorage.GetFileCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamRemoteStorage_GetFileNameAndSize(IntPtr _, int iFile, ref int pnFileSizeInBytes)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetFileNameAndSize");
            return SteamEmulator.SteamRemoteStorage.GetFileNameAndSize(iFile, ref pnFileSizeInBytes);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_GetQuota(IntPtr _, ref ulong pnTotalBytes, ref ulong puAvailableBytes)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetQuota");
            return SteamEmulator.SteamRemoteStorage.GetQuota(ref pnTotalBytes, ref puAvailableBytes);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_IsCloudEnabledForAccount(IntPtr _)
        {
            Write("SteamAPI_ISteamRemoteStorage_IsCloudEnabledForAccount");
            return SteamEmulator.SteamRemoteStorage.IsCloudEnabledForAccount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_IsCloudEnabledForApp(IntPtr _)
        {
            Write("SteamAPI_ISteamRemoteStorage_IsCloudEnabledForApp");
            return SteamEmulator.SteamRemoteStorage.IsCloudEnabledForApp();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamRemoteStorage_SetCloudEnabledForApp(IntPtr _, bool bEnabled)
        {
            Write("SteamAPI_ISteamRemoteStorage_SetCloudEnabledForApp");
            SteamEmulator.SteamRemoteStorage.SetCloudEnabledForApp(bEnabled);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_UGCDownload(IntPtr _, UGCHandle_t hContent, uint unPriority)
        {
            Write("SteamAPI_ISteamRemoteStorage_UGCDownload");
            return SteamEmulator.SteamRemoteStorage.UGCDownload(hContent, unPriority);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_GetUGCDownloadProgress(IntPtr _, UGCHandle_t hContent, int pnBytesDownloaded, int pnBytesExpected)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetUGCDownloadProgress");
            return SteamEmulator.SteamRemoteStorage.GetUGCDownloadProgress(hContent, pnBytesDownloaded, pnBytesExpected);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_GetUGCDetails(IntPtr _, UGCHandle_t hContent, uint pnAppID, string ppchName, int pnFileSizeInBytes, ulong pSteamIDOwner)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetUGCDetails");
            return SteamEmulator.SteamRemoteStorage.GetUGCDetails(hContent, pnAppID, ppchName, pnFileSizeInBytes, pSteamIDOwner);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamRemoteStorage_UGCRead(IntPtr _, UGCHandle_t hContent, IntPtr pvData, int cubDataToRead, uint cOffset, int eAction)
        {
            Write("SteamAPI_ISteamRemoteStorage_UGCRead");
            return SteamEmulator.SteamRemoteStorage.UGCRead(hContent, pvData, cubDataToRead, cOffset, eAction);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamRemoteStorage_GetCachedUGCCount(IntPtr _)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetCachedUGCCount");
            return SteamEmulator.SteamRemoteStorage.GetCachedUGCCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static UGCHandle_t SteamAPI_ISteamRemoteStorage_GetCachedUGCHandle(IntPtr _, int iCachedContent)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetCachedUGCHandle");
            return SteamEmulator.SteamRemoteStorage.GetCachedUGCHandle(iCachedContent);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_PublishWorkshopFile(IntPtr _, string pchFile, string pchPreviewFile, uint nConsumerAppId, string pchTitle, string pchDescription, int eVisibility, IntPtr pTags, int int2)
        {
            Write("SteamAPI_ISteamRemoteStorage_PublishWorkshopFile");
            return SteamEmulator.SteamRemoteStorage.PublishWorkshopFile(pchFile, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, pTags, int2);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_CreatePublishedFileUpdateRequest(IntPtr _, ulong unPublishedFileId)
        {
            Write("SteamAPI_ISteamRemoteStorage_CreatePublishedFileUpdateRequest");
            return SteamEmulator.SteamRemoteStorage.CreatePublishedFileUpdateRequest(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileFile(IntPtr _, PublishedFileUpdateHandle_t updateHandle, string pchFile)
        {
            Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileFile");
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileFile(updateHandle, pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFilePreviewFile(IntPtr _, PublishedFileUpdateHandle_t updateHandle, string pchPreviewFile)
        {
            Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFilePreviewFile");
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFilePreviewFile(updateHandle, pchPreviewFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTitle(IntPtr _, PublishedFileUpdateHandle_t updateHandle, string pchTitle)
        {
            Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTitle");
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileTitle(updateHandle, pchTitle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileDescription(IntPtr _, PublishedFileUpdateHandle_t updateHandle, string pchDescription)
        {
            Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileDescription");
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileDescription(updateHandle, pchDescription);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileVisibility(IntPtr _, PublishedFileUpdateHandle_t updateHandle, int eVisibility)
        {
            Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileVisibility");
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileVisibility(updateHandle, eVisibility);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTags(IntPtr _, PublishedFileUpdateHandle_t updateHandle, IntPtr pTags)
        {
            Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTags");
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileTags(updateHandle, pTags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_CommitPublishedFileUpdate(IntPtr _, PublishedFileUpdateHandle_t updateHandle)
        {
            Write("SteamAPI_ISteamRemoteStorage_CommitPublishedFileUpdate");
            return SteamEmulator.SteamRemoteStorage.CommitPublishedFileUpdate(updateHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_GetPublishedFileDetails(IntPtr _, ulong unPublishedFileId, uint unMaxSecondsOld)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetPublishedFileDetails");
            return SteamEmulator.SteamRemoteStorage.GetPublishedFileDetails(unPublishedFileId, unMaxSecondsOld);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_DeletePublishedFile(IntPtr _, ulong unPublishedFileId)
        {
            Write("SteamAPI_ISteamRemoteStorage_DeletePublishedFile");
            return SteamEmulator.SteamRemoteStorage.DeletePublishedFile(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_EnumerateUserPublishedFiles(IntPtr _, uint unStartIndex)
        {
            Write("SteamAPI_ISteamRemoteStorage_EnumerateUserPublishedFiles");
            return SteamEmulator.SteamRemoteStorage.EnumerateUserPublishedFiles(unStartIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_SubscribePublishedFile(IntPtr _, ulong unPublishedFileId)
        {
            Write("SteamAPI_ISteamRemoteStorage_SubscribePublishedFile");
            return SteamEmulator.SteamRemoteStorage.SubscribePublishedFile(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_EnumerateUserSubscribedFiles(IntPtr _, uint unStartIndex)
        {
            Write("SteamAPI_ISteamRemoteStorage_EnumerateUserSubscribedFiles");
            return SteamEmulator.SteamRemoteStorage.EnumerateUserSubscribedFiles(unStartIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_UnsubscribePublishedFile(IntPtr _, ulong unPublishedFileId)
        {
            Write("SteamAPI_ISteamRemoteStorage_UnsubscribePublishedFile");
            return SteamEmulator.SteamRemoteStorage.UnsubscribePublishedFile(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileSetChangeDescription(IntPtr _, PublishedFileUpdateHandle_t updateHandle, string pchChangeDescription)
        {
            Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileSetChangeDescription");
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileSetChangeDescription(updateHandle, pchChangeDescription);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_GetPublishedItemVoteDetails(IntPtr _, ulong unPublishedFileId)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetPublishedItemVoteDetails");
            return SteamEmulator.SteamRemoteStorage.GetPublishedItemVoteDetails(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_UpdateUserPublishedItemVote(IntPtr _, ulong unPublishedFileId, bool bVoteUp)
        {
            Write("SteamAPI_ISteamRemoteStorage_UpdateUserPublishedItemVote");
            return SteamEmulator.SteamRemoteStorage.UpdateUserPublishedItemVote(unPublishedFileId, bVoteUp);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_GetUserPublishedItemVoteDetails(IntPtr _, ulong unPublishedFileId)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetUserPublishedItemVoteDetails");
            return SteamEmulator.SteamRemoteStorage.GetUserPublishedItemVoteDetails(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_EnumerateUserSharedWorkshopFiles(IntPtr _, ulong steamId, uint unStartIndex, IntPtr pRequiredTags, IntPtr pExcludedTags)
        {
            Write("SteamAPI_ISteamRemoteStorage_EnumerateUserSharedWorkshopFiles");
            return SteamEmulator.SteamRemoteStorage.EnumerateUserSharedWorkshopFiles(steamId, unStartIndex, pRequiredTags, pExcludedTags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_PublishVideo(IntPtr _, int eVideoProvider, string pchVideoAccount, string pchVideoIdentifier, string pchPreviewFile, uint nConsumerAppId, string pchTitle, string pchDescription, int eVisibility, IntPtr pTags)
        {
            Write("SteamAPI_ISteamRemoteStorage_PublishVideo");
            return SteamEmulator.SteamRemoteStorage.PublishVideo(eVideoProvider, pchVideoAccount, pchVideoIdentifier, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, pTags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_SetUserPublishedFileAction(IntPtr _, ulong unPublishedFileId, int eAction)
        {
            Write("SteamAPI_ISteamRemoteStorage_SetUserPublishedFileAction");
            return SteamEmulator.SteamRemoteStorage.SetUserPublishedFileAction(unPublishedFileId, eAction);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_EnumeratePublishedFilesByUserAction(IntPtr _, int eAction, uint unStartIndex)
        {
            Write("SteamAPI_ISteamRemoteStorage_EnumeratePublishedFilesByUserAction");
            return SteamEmulator.SteamRemoteStorage.EnumeratePublishedFilesByUserAction(eAction, unStartIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_EnumeratePublishedWorkshopFiles(IntPtr _, int eEnumerationType, uint unStartIndex, uint unCount, uint unDays, IntPtr pTags, IntPtr pUserTags)
        {
            Write("SteamAPI_ISteamRemoteStorage_EnumeratePublishedWorkshopFiles");
            return SteamEmulator.SteamRemoteStorage.EnumeratePublishedWorkshopFiles(eEnumerationType, unStartIndex, unCount, unDays, pTags, pUserTags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamRemoteStorage_UGCDownloadToLocation(IntPtr _, UGCHandle_t hContent, string pchLocation, uint unPriority)
        {
            Write("SteamAPI_ISteamRemoteStorage_UGCDownloadToLocation");
            return SteamEmulator.SteamRemoteStorage.UGCDownloadToLocation(hContent, pchLocation, unPriority);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamRemoteStorage_v014()
        {
            Write("SteamAPI_SteamRemoteStorage_v014");
            return InterfaceManager.FindOrCreateInterface("STEAMREMOTESTORAGE_INTERFACE_VERSION014");
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

