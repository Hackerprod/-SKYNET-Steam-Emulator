using SKYNET;
using SKYNET.Steamworks;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Exported
{
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
        public static ulong SteamAPI_ISteamRemoteStorage_FileWriteAsync(string pchFile, IntPtr pvData, uint cubData)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileWriteAsync");
            return SteamEmulator.SteamRemoteStorage.FileWriteAsync(pchFile, pvData, cubData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_FileReadAsync(string pchFile, uint nOffset, uint cubToRead)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileReadAsync");
            return SteamEmulator.SteamRemoteStorage.FileReadAsync(pchFile, nOffset, cubToRead);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_FileReadAsyncComplete(ulong hReadCall, IntPtr pvBuffer, uint cubToRead)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileReadAsyncComplete");
            return SteamEmulator.SteamRemoteStorage.FileReadAsyncComplete(hReadCall, pvBuffer, cubToRead);
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
        public static ulong SteamAPI_ISteamRemoteStorage_FileShare(string pchFile)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileShare");
            return SteamEmulator.SteamRemoteStorage.FileShare(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_SetSyncPlatforms(string pchFile, int eRemoteStoragePlatform)
        {
            Write("SteamAPI_ISteamRemoteStorage_SetSyncPlatforms");
            return SteamEmulator.SteamRemoteStorage.SetSyncPlatforms(pchFile, eRemoteStoragePlatform);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_FileWriteStreamOpen(string pchFile)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileWriteStreamOpen");
            return SteamEmulator.SteamRemoteStorage.FileWriteStreamOpen(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_FileWriteStreamWriteChunk(ulong writeHandle, IntPtr pvData, int cubData)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileWriteStreamWriteChunk");
            return SteamEmulator.SteamRemoteStorage.FileWriteStreamWriteChunk(writeHandle, pvData, cubData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_FileWriteStreamClose(ulong writeHandle)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileWriteStreamClose");
            return SteamEmulator.SteamRemoteStorage.FileWriteStreamClose(writeHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_FileWriteStreamCancel(ulong writeHandle)
        {
            Write("SteamAPI_ISteamRemoteStorage_FileWriteStreamCancel");
            return SteamEmulator.SteamRemoteStorage.FileWriteStreamCancel(writeHandle);
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
        public static int SteamAPI_ISteamRemoteStorage_GetSyncPlatforms(string pchFile)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetSyncPlatforms");
            return SteamEmulator.SteamRemoteStorage.GetSyncPlatforms(pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamRemoteStorage_GetFileCount()
        {
            Write("SteamAPI_ISteamRemoteStorage_GetFileCount");
            return SteamEmulator.SteamRemoteStorage.GetFileCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamRemoteStorage_GetFileNameAndSize(int iFile, int pnFileSizeInBytes)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetFileNameAndSize");
            return SteamEmulator.SteamRemoteStorage.GetFileNameAndSize(iFile, pnFileSizeInBytes);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_GetQuota(int pnTotalBytes, int puAvailableBytes)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetQuota");
            return SteamEmulator.SteamRemoteStorage.GetQuota(pnTotalBytes, puAvailableBytes);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_IsCloudEnabledForAccount()
        {
            Write("SteamAPI_ISteamRemoteStorage_IsCloudEnabledForAccount");
            return SteamEmulator.SteamRemoteStorage.IsCloudEnabledForAccount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_IsCloudEnabledForApp()
        {
            Write("SteamAPI_ISteamRemoteStorage_IsCloudEnabledForApp");
            return SteamEmulator.SteamRemoteStorage.IsCloudEnabledForApp();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamRemoteStorage_SetCloudEnabledForApp(bool bEnabled)
        {
            Write("SteamAPI_ISteamRemoteStorage_SetCloudEnabledForApp");
            SteamEmulator.SteamRemoteStorage.SetCloudEnabledForApp(bEnabled);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_UGCDownload(ulong hContent, uint unPriority)
        {
            Write("SteamAPI_ISteamRemoteStorage_UGCDownload");
            return SteamEmulator.SteamRemoteStorage.UGCDownload(hContent, unPriority);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_GetUGCDownloadProgress(ulong hContent, int pnBytesDownloaded, int pnBytesExpected)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetUGCDownloadProgress");
            return SteamEmulator.SteamRemoteStorage.GetUGCDownloadProgress(hContent, pnBytesDownloaded, pnBytesExpected);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_GetUGCDetails(ulong hContent, uint pnAppID, string ppchName, int pnFileSizeInBytes, ulong pSteamIDOwner)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetUGCDetails");
            return SteamEmulator.SteamRemoteStorage.GetUGCDetails(hContent, pnAppID, ppchName, pnFileSizeInBytes, pSteamIDOwner);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamRemoteStorage_UGCRead(ulong hContent, IntPtr pvData, int cubDataToRead, uint cOffset, IntPtr eAction)
        {
            Write("SteamAPI_ISteamRemoteStorage_UGCRead");
            return SteamEmulator.SteamRemoteStorage.UGCRead(hContent, pvData, cubDataToRead, cOffset, eAction);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamRemoteStorage_GetCachedUGCCount()
        {
            Write("SteamAPI_ISteamRemoteStorage_GetCachedUGCCount");
            return SteamEmulator.SteamRemoteStorage.GetCachedUGCCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_GetCachedUGCHandle(int iCachedContent)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetCachedUGCHandle");
            return SteamEmulator.SteamRemoteStorage.GetCachedUGCHandle(iCachedContent);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_PublishWorkshopFile(string pchFile, string pchPreviewFile, uint nConsumerAppId, string pchTitle, string pchDescription, int eVisibility, IntPtr pTags, int int2)
        {
            Write("SteamAPI_ISteamRemoteStorage_PublishWorkshopFile");
            return SteamEmulator.SteamRemoteStorage.PublishWorkshopFile(pchFile, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, pTags, int2);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_CreatePublishedFileUpdateRequest(ulong unPublishedFileId)
        {
            Write("SteamAPI_ISteamRemoteStorage_CreatePublishedFileUpdateRequest");
            return SteamEmulator.SteamRemoteStorage.CreatePublishedFileUpdateRequest(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileFile(ulong updateHandle, string pchFile)
        {
            Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileFile");
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileFile(updateHandle, pchFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFilePreviewFile(ulong updateHandle, string pchPreviewFile)
        {
            Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFilePreviewFile");
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFilePreviewFile(updateHandle, pchPreviewFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTitle(ulong updateHandle, string pchTitle)
        {
            Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTitle");
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileTitle(updateHandle, pchTitle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileDescription(ulong updateHandle, string pchDescription)
        {
            Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileDescription");
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileDescription(updateHandle, pchDescription);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileVisibility(ulong updateHandle, int eVisibility)
        {
            Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileVisibility");
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileVisibility(updateHandle, eVisibility);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTags(ulong updateHandle, IntPtr pTags)
        {
            Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileTags");
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileTags(updateHandle, pTags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_CommitPublishedFileUpdate(ulong updateHandle)
        {
            Write("SteamAPI_ISteamRemoteStorage_CommitPublishedFileUpdate");
            return SteamEmulator.SteamRemoteStorage.CommitPublishedFileUpdate(updateHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_GetPublishedFileDetails(ulong unPublishedFileId, uint unMaxSecondsOld)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetPublishedFileDetails");
            return SteamEmulator.SteamRemoteStorage.GetPublishedFileDetails(unPublishedFileId, unMaxSecondsOld);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_DeletePublishedFile(ulong unPublishedFileId)
        {
            Write("SteamAPI_ISteamRemoteStorage_DeletePublishedFile");
            return SteamEmulator.SteamRemoteStorage.DeletePublishedFile(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_EnumerateUserPublishedFiles(uint unStartIndex)
        {
            Write("SteamAPI_ISteamRemoteStorage_EnumerateUserPublishedFiles");
            return SteamEmulator.SteamRemoteStorage.EnumerateUserPublishedFiles(unStartIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_SubscribePublishedFile(ulong unPublishedFileId)
        {
            Write("SteamAPI_ISteamRemoteStorage_SubscribePublishedFile");
            return SteamEmulator.SteamRemoteStorage.SubscribePublishedFile(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_EnumerateUserSubscribedFiles(uint unStartIndex)
        {
            Write("SteamAPI_ISteamRemoteStorage_EnumerateUserSubscribedFiles");
            return SteamEmulator.SteamRemoteStorage.EnumerateUserSubscribedFiles(unStartIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_UnsubscribePublishedFile(ulong unPublishedFileId)
        {
            Write("SteamAPI_ISteamRemoteStorage_UnsubscribePublishedFile");
            return SteamEmulator.SteamRemoteStorage.UnsubscribePublishedFile(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamRemoteStorage_UpdatePublishedFileSetChangeDescription(ulong updateHandle, string pchChangeDescription)
        {
            Write("SteamAPI_ISteamRemoteStorage_UpdatePublishedFileSetChangeDescription");
            return SteamEmulator.SteamRemoteStorage.UpdatePublishedFileSetChangeDescription(updateHandle, pchChangeDescription);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_GetPublishedItemVoteDetails(ulong unPublishedFileId)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetPublishedItemVoteDetails");
            return SteamEmulator.SteamRemoteStorage.GetPublishedItemVoteDetails(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_UpdateUserPublishedItemVote(ulong unPublishedFileId, bool bVoteUp)
        {
            Write("SteamAPI_ISteamRemoteStorage_UpdateUserPublishedItemVote");
            return SteamEmulator.SteamRemoteStorage.UpdateUserPublishedItemVote(unPublishedFileId, bVoteUp);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_GetUserPublishedItemVoteDetails(ulong unPublishedFileId)
        {
            Write("SteamAPI_ISteamRemoteStorage_GetUserPublishedItemVoteDetails");
            return SteamEmulator.SteamRemoteStorage.GetUserPublishedItemVoteDetails(unPublishedFileId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_EnumerateUserSharedWorkshopFiles(ulong steamId, uint unStartIndex, IntPtr pRequiredTags, IntPtr pExcludedTags)
        {
            Write("SteamAPI_ISteamRemoteStorage_EnumerateUserSharedWorkshopFiles");
            return SteamEmulator.SteamRemoteStorage.EnumerateUserSharedWorkshopFiles(steamId, unStartIndex, pRequiredTags, pExcludedTags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_PublishVideo(int eVideoProvider, string pchVideoAccount, string pchVideoIdentifier, string pchPreviewFile, uint nConsumerAppId, string pchTitle, string pchDescription, int eVisibility, IntPtr pTags)
        {
            Write("SteamAPI_ISteamRemoteStorage_PublishVideo");
            return SteamEmulator.SteamRemoteStorage.PublishVideo(eVideoProvider, pchVideoAccount, pchVideoIdentifier, pchPreviewFile, nConsumerAppId, pchTitle, pchDescription, eVisibility, pTags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_SetUserPublishedFileAction(ulong unPublishedFileId, int eAction)
        {
            Write("SteamAPI_ISteamRemoteStorage_SetUserPublishedFileAction");
            return SteamEmulator.SteamRemoteStorage.SetUserPublishedFileAction(unPublishedFileId, eAction);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_EnumeratePublishedFilesByUserAction(int eAction, uint unStartIndex)
        {
            Write("SteamAPI_ISteamRemoteStorage_EnumeratePublishedFilesByUserAction");
            return SteamEmulator.SteamRemoteStorage.EnumeratePublishedFilesByUserAction(eAction, unStartIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_EnumeratePublishedWorkshopFiles(int eEnumerationType, uint unStartIndex, uint unCount, uint unDays, IntPtr pTags, IntPtr pUserTags)
        {
            Write("SteamAPI_ISteamRemoteStorage_EnumeratePublishedWorkshopFiles");
            return SteamEmulator.SteamRemoteStorage.EnumeratePublishedWorkshopFiles(eEnumerationType, unStartIndex, unCount, unDays, pTags, pUserTags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamRemoteStorage_UGCDownloadToLocation(ulong hContent, string pchLocation, uint unPriority)
        {
            Write("SteamAPI_ISteamRemoteStorage_UGCDownloadToLocation");
            return SteamEmulator.SteamRemoteStorage.UGCDownloadToLocation(hContent, pchLocation, unPriority);
        }
    }
}

