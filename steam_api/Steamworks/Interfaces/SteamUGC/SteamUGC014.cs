using System;

using SteamAPICall_t = System.UInt64;
using PublishedFileId_t = System.UInt64;
using UGCQueryHandle_t = System.UInt64;
using UGCUpdateHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("STEAMUGC_INTERFACE_VERSION010")]
    [Interface("STEAMUGC_INTERFACE_VERSION012")]
    [Interface("STEAMUGC_INTERFACE_VERSION014")]
    public class SteamUGC014 : ISteamInterface
    {
        public UGCQueryHandle_t CreateQueryUserUGCRequest(IntPtr _, uint unAccountID, int eListType, int eMatchingUGCType, int eSortOrder, uint nCreatorAppID, uint nConsumerAppID, uint unPage)
        {
            return SteamEmulator.SteamUGC.CreateQueryUserUGCRequest(unAccountID, eListType, eMatchingUGCType, eSortOrder, nCreatorAppID, nConsumerAppID, unPage);
        }

        public UGCQueryHandle_t CreateQueryAllUGCRequest(IntPtr _, int eQueryType, int eMatchingeMatchingUGCTypeFileType, uint nCreatorAppID, uint nConsumerAppID, uint unPage)
        {
            return SteamEmulator.SteamUGC.CreateQueryAllUGCRequest(eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, unPage);
        }

        public UGCQueryHandle_t CreateQueryAllUGCRequest(IntPtr _, int eQueryType, int eMatchingeMatchingUGCTypeFileType, uint nCreatorAppID, uint nConsumerAppID, string pchCursor)
        {
            return SteamEmulator.SteamUGC.CreateQueryAllUGCRequest(eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, pchCursor);
        }

        public UGCQueryHandle_t CreateQueryUGCDetailsRequest(IntPtr _, ulong pvecPublishedFileID, uint unNumPublishedFileIDs)
        {
            return SteamEmulator.SteamUGC.CreateQueryUGCDetailsRequest(pvecPublishedFileID, unNumPublishedFileIDs);
        }

        public SteamAPICall_t SendQueryUGCRequest(IntPtr _, UGCQueryHandle_t handle)
        {
            return SteamEmulator.SteamUGC.SendQueryUGCRequest(handle);
        }

        public bool GetQueryUGCResult(IntPtr _, UGCQueryHandle_t handle, uint index, ref SteamUGCDetails_t pDetails)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCResult(handle, index, ref pDetails);
        }

        public bool GetQueryUGCPreviewURL(IntPtr _, UGCQueryHandle_t handle, uint index, string pchURL, uint cchURLSize)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCPreviewURL(handle, index, pchURL, cchURLSize);
        }

        public bool GetQueryUGCMetadata(IntPtr _, UGCQueryHandle_t handle, uint index, string pchMetadata, uint cchMetadatasize)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCMetadata(handle, index, pchMetadata, cchMetadatasize);
        }

        public bool GetQueryUGCChildren(IntPtr _, UGCQueryHandle_t handle, uint index, ref ulong[] pvecPublishedFileID, uint cMaxEntries)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCChildren(handle, index, ref pvecPublishedFileID, cMaxEntries);
        }

        public bool GetQueryUGCStatistic(IntPtr _, UGCQueryHandle_t handle, uint index, int eStatType, ulong pStatValue)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCStatistic(handle, index, eStatType, pStatValue);
        }

        public uint GetQueryUGCNumAdditionalPreviews(IntPtr _, UGCQueryHandle_t handle, uint index)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCNumAdditionalPreviews(handle, index);
        }

        public bool GetQueryUGCAdditionalPreview(IntPtr _, UGCQueryHandle_t handle, uint index, uint previewIndex, string pchURLOrVideoID, uint cchURLSize, string pchOriginalFileName, uint cchOriginalFileNameSize, int pPreviewType)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCAdditionalPreview(handle, index, previewIndex, pchURLOrVideoID, cchURLSize, pchOriginalFileName, cchOriginalFileNameSize, pPreviewType);
        }

        public uint GetQueryUGCNumKeyValueTags(IntPtr _, UGCQueryHandle_t handle, uint index)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCNumKeyValueTags(handle, index);
        }

        public bool GetQueryUGCKeyValueTag(IntPtr _, UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, string pchKey, uint cchKeySize, string pchValue, uint cchValueSize)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCKeyValueTag(handle, index, keyValueTagIndex, pchKey, cchKeySize, pchValue, cchValueSize);
        }

        public bool GetQueryUGCKeyValueTag(IntPtr _, UGCQueryHandle_t handle, uint index, string pchKey, string pchValue, uint cchValueSize)
        {
            return SteamEmulator.SteamUGC.GetQueryUGCKeyValueTag(handle, index, pchKey, pchValue, cchValueSize);
        }

        public bool ReleaseQueryUGCRequest(IntPtr _, UGCQueryHandle_t handle)
        {
            return SteamEmulator.SteamUGC.ReleaseQueryUGCRequest(handle);
        }

        public bool AddRequiredTag(IntPtr _, UGCQueryHandle_t handle, string pTagName)
        {
            return SteamEmulator.SteamUGC.AddRequiredTag(handle, pTagName);
        }

        public bool AddRequiredTagGroup(IntPtr _, UGCQueryHandle_t handle, IntPtr pTagGroups )  
{
    return  SteamEmulator.SteamUGC.AddRequiredTagGroup(handle, pTagGroups);
}

    public bool AddExcludedTag(IntPtr _, UGCQueryHandle_t handle, string pTagName)
    {
        return SteamEmulator.SteamUGC.AddExcludedTag(handle, pTagName);
    }

    public bool SetReturnOnlyIDs(IntPtr _, UGCQueryHandle_t handle, bool bReturnOnlyIDs)
    {
        return SteamEmulator.SteamUGC.SetReturnOnlyIDs(handle, bReturnOnlyIDs);
    }

    public bool SetReturnKeyValueTags(IntPtr _, UGCQueryHandle_t handle, bool bReturnKeyValueTags)
    {
        return SteamEmulator.SteamUGC.SetReturnKeyValueTags(handle, bReturnKeyValueTags);
    }

    public bool SetReturnLongDescription(IntPtr _, UGCQueryHandle_t handle, bool bReturnLongDescription)
    {
        return SteamEmulator.SteamUGC.SetReturnLongDescription(handle, bReturnLongDescription);
    }

    public bool SetReturnMetadata(IntPtr _, UGCQueryHandle_t handle, bool bReturnMetadata)
    {
        return SteamEmulator.SteamUGC.SetReturnMetadata(handle, bReturnMetadata);
    }

    public bool SetReturnChildren(IntPtr _, UGCQueryHandle_t handle, bool bReturnChildren)
    {
        return SteamEmulator.SteamUGC.SetReturnChildren(handle, bReturnChildren);
    }

    public bool SetReturnAdditionalPreviews(IntPtr _, UGCQueryHandle_t handle, bool bReturnAdditionalPreviews)
    {
        return SteamEmulator.SteamUGC.SetReturnAdditionalPreviews(handle, bReturnAdditionalPreviews);
    }

    public bool SetReturnTotalOnly(IntPtr _, UGCQueryHandle_t handle, bool bReturnTotalOnly)
    {
        return SteamEmulator.SteamUGC.SetReturnTotalOnly(handle, bReturnTotalOnly);
    }

    public bool SetReturnPlaytimeStats(IntPtr _, UGCQueryHandle_t handle, uint unDays)
    {
        return SteamEmulator.SteamUGC.SetReturnPlaytimeStats(handle, unDays);
    }

    public bool SetLanguage(IntPtr _, UGCQueryHandle_t handle, string pchLanguage)
    {
        return SteamEmulator.SteamUGC.SetLanguage(handle, pchLanguage);
    }

    public bool SetAllowCachedResponse(IntPtr _, UGCQueryHandle_t handle, uint unMaxAgeSeconds)
    {
        return SteamEmulator.SteamUGC.SetAllowCachedResponse(handle, unMaxAgeSeconds);
    }

    public bool SetCloudFileNameFilter(IntPtr _, UGCQueryHandle_t handle, string pMatchCloudFileName)
    {
        return SteamEmulator.SteamUGC.SetCloudFileNameFilter(handle, pMatchCloudFileName);
    }

    public bool SetMatchAnyTag(IntPtr _, UGCQueryHandle_t handle, bool bMatchAnyTag)
    {
        return SteamEmulator.SteamUGC.SetMatchAnyTag(handle, bMatchAnyTag);
    }

    public bool SetSearchText(IntPtr _, UGCQueryHandle_t handle, string pSearchText)
    {
        return SteamEmulator.SteamUGC.SetSearchText(handle, pSearchText);
    }

    public bool SetRankedByTrendDays(IntPtr _, UGCQueryHandle_t handle, uint unDays)
    {
        return SteamEmulator.SteamUGC.SetRankedByTrendDays(handle, unDays);
    }

    public bool AddRequiredKeyValueTag(IntPtr _, UGCQueryHandle_t handle, string pKey, string pValue)
    {
        return SteamEmulator.SteamUGC.AddRequiredKeyValueTag(handle, pKey, pValue);
    }

    public SteamAPICall_t RequestUGCDetails(IntPtr _, ulong nPublishedFileID, uint unMaxAgeSeconds)
    {
        return SteamEmulator.SteamUGC.RequestUGCDetails(nPublishedFileID, unMaxAgeSeconds);
    }

    public SteamAPICall_t CreateItem(IntPtr _, uint nConsumerAppId, int eFileType)
    {
        return SteamEmulator.SteamUGC.CreateItem(nConsumerAppId, eFileType);
    }

    public UGCUpdateHandle_t StartItemUpdate(uint nConsumerAppId, ulong nPublishedFileID)
    {
        return SteamEmulator.SteamUGC.StartItemUpdate(nConsumerAppId, nPublishedFileID);
    }

    public bool SetItemTitle(IntPtr _, UGCQueryHandle_t handle, string pchTitle)
    {
        return SteamEmulator.SteamUGC.SetItemTitle(handle, pchTitle);
    }

    public bool SetItemDescription(IntPtr _, UGCQueryHandle_t handle, string pchDescription)
    {
        return SteamEmulator.SteamUGC.SetItemDescription(handle, pchDescription);
    }

    public bool SetItemUpdateLanguage(IntPtr _, UGCQueryHandle_t handle, string pchLanguage)
    {
        return SteamEmulator.SteamUGC.SetItemUpdateLanguage(handle, pchLanguage);
    }

    public bool SetItemMetadata(IntPtr _, UGCQueryHandle_t handle, string pchMetaData)
    {
        return SteamEmulator.SteamUGC.SetItemMetadata(handle, pchMetaData);
    }

    public bool SetItemVisibility(IntPtr _, UGCQueryHandle_t handle, int eVisibility)
    {
        return SteamEmulator.SteamUGC.SetItemVisibility(handle, eVisibility);
    }

    public bool SetItemTags(IntPtr _, ulong updateHandle, IntPtr pTags) 
    {
        return SteamEmulator.SteamUGC.SetItemTags(updateHandle, pTags);
    }

    public bool SetItemContent(IntPtr _, UGCQueryHandle_t handle, string pszContentFolder) 
    {
        return SteamEmulator.SteamUGC.SetItemContent(handle, pszContentFolder);
    }

    public bool SetItemPreview(IntPtr _, UGCQueryHandle_t handle, string pszPreviewFile) 
    {
        return SteamEmulator.SteamUGC.SetItemPreview(handle, pszPreviewFile);
    }

    public bool SetAllowLegacyUpload(IntPtr _, UGCQueryHandle_t handle, bool bAllowLegacyUpload) 
    {
        return SteamEmulator.SteamUGC.SetAllowLegacyUpload(handle, bAllowLegacyUpload);
    }

    public bool RemoveAllItemKeyValueTags(IntPtr _, UGCQueryHandle_t handle) 
    {
        return SteamEmulator.SteamUGC.RemoveAllItemKeyValueTags(handle);
    }

    public bool RemoveItemKeyValueTags(IntPtr _, UGCQueryHandle_t handle, string pchKey) 
    {
        return SteamEmulator.SteamUGC.RemoveItemKeyValueTags(handle, pchKey);
    }

    public bool AddItemKeyValueTag(IntPtr _, UGCQueryHandle_t handle, string pchKey, string pchValue)
    {
        return SteamEmulator.SteamUGC.AddItemKeyValueTag(handle, pchKey, pchValue);
    }

    public bool AddItemPreviewFile(IntPtr _, UGCQueryHandle_t handle, string pszPreviewFile, int type)  
    {
        return SteamEmulator.SteamUGC.AddItemPreviewFile(handle, pszPreviewFile, type);
    }

    public bool AddItemPreviewVideo(IntPtr _, UGCQueryHandle_t handle, string pszVideoID)
    {
        return SteamEmulator.SteamUGC.AddItemPreviewVideo(handle, pszVideoID);
    }

    public bool UpdateItemPreviewFile(IntPtr _, UGCQueryHandle_t handle, uint index, string pszPreviewFile) 
    {
        return SteamEmulator.SteamUGC.UpdateItemPreviewFile(handle, index, pszPreviewFile);
    }

    public bool UpdateItemPreviewVideo(IntPtr _, UGCQueryHandle_t handle, uint index, string pszVideoID) 
    {
        return SteamEmulator.SteamUGC.UpdateItemPreviewVideo(handle, index, pszVideoID);
    }

    public bool RemoveItemPreview(IntPtr _, UGCQueryHandle_t handle, uint index)
    {
        return SteamEmulator.SteamUGC.RemoveItemPreview(handle, index);
    }

    public SteamAPICall_t SubmitItemUpdate(IntPtr _, UGCQueryHandle_t handle, string pchChangeNote) 
    {
        return SteamEmulator.SteamUGC.SubmitItemUpdate(handle, pchChangeNote);
    }

    public int GetItemUpdateProgress(IntPtr _, UGCQueryHandle_t handle, ulong punBytesProcessed, ulong punBytesTotal)
    {
        return SteamEmulator.SteamUGC.GetItemUpdateProgress(handle, punBytesProcessed, punBytesTotal);
    }

    public SteamAPICall_t SetUserItemVote(IntPtr _, ulong nPublishedFileID, bool bVoteUp)
    {
        return SteamEmulator.SteamUGC.SetUserItemVote(nPublishedFileID, bVoteUp);
    }

    public SteamAPICall_t GetUserItemVote(IntPtr _, ulong nPublishedFileID)
    {
        return SteamEmulator.SteamUGC.GetUserItemVote(nPublishedFileID);
    }

    public SteamAPICall_t AddItemToFavorites(IntPtr _, uint nAppId, ulong nPublishedFileID)
    {
        return SteamEmulator.SteamUGC.AddItemToFavorites(nAppId, nPublishedFileID);
    }

    public SteamAPICall_t RemoveItemFromFavorites(IntPtr _, uint nAppId, ulong nPublishedFileID)
    {
        return SteamEmulator.SteamUGC.RemoveItemFromFavorites(nAppId, nPublishedFileID);
    }

    public SteamAPICall_t SubscribeItem(IntPtr _, PublishedFileId_t nPublishedFileID) 
    {
        return SteamEmulator.SteamUGC.SubscribeItem(nPublishedFileID);
    }

    public SteamAPICall_t UnsubscribeItem(IntPtr _, PublishedFileId_t nPublishedFileID) 
    {
        return SteamEmulator.SteamUGC.UnsubscribeItem(nPublishedFileID);
    }

    public uint GetNumSubscribedItems(IntPtr _)  // number of subscribed items 
    {
        return SteamEmulator.SteamUGC.GetNumSubscribedItems();
    }

    public uint GetSubscribedItems(IntPtr _, ulong pvecPublishedFileID, uint cMaxEntries)  // all subscribed item PublishFileIDs
    {
        return SteamEmulator.SteamUGC.GetSubscribedItems(pvecPublishedFileID, cMaxEntries);
    }

    public uint GetItemState(IntPtr _, ulong nPublishedFileID)
    {
        return SteamEmulator.SteamUGC.GetItemState(nPublishedFileID);
    }

    public bool GetItemInstallInfo(IntPtr _, ulong nPublishedFileID, ulong punSizeOnDisk, string pchFolder, uint cchFolderSize, uint punTimeStamp)
    {
        return SteamEmulator.SteamUGC.GetItemInstallInfo(nPublishedFileID, punSizeOnDisk, pchFolder, cchFolderSize, punTimeStamp);
    }

    public bool GetItemDownloadInfo(IntPtr _, ulong nPublishedFileID, ulong punBytesDownloaded, ulong punBytesTotal)
    {
        return SteamEmulator.SteamUGC.GetItemDownloadInfo(nPublishedFileID, punBytesDownloaded, punBytesTotal);
    }

    public bool DownloadItem(IntPtr _, ulong nPublishedFileID, bool bHighPriority)
    {
        return SteamEmulator.SteamUGC.DownloadItem(nPublishedFileID, bHighPriority);
    }

    public bool BInitWorkshopForGameServer(IntPtr _, uint unWorkshopDepotID, string pszFolder)
    {
        return SteamEmulator.SteamUGC.BInitWorkshopForGameServer(unWorkshopDepotID, pszFolder);
    }

    public void SuspendDownloads(IntPtr _, bool bSuspend)
    {
        SteamEmulator.SteamUGC.SuspendDownloads(bSuspend);
    }

    public SteamAPICall_t StartPlaytimeTracking(IntPtr _, PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs)
    {
        return SteamEmulator.SteamUGC.StartPlaytimeTracking(pvecPublishedFileID, unNumPublishedFileIDs);
    }

    public SteamAPICall_t StopPlaytimeTracking(IntPtr _, ulong pvecPublishedFileID, uint unNumPublishedFileIDs)
    {
        return SteamEmulator.SteamUGC.StopPlaytimeTracking(pvecPublishedFileID, unNumPublishedFileIDs);
    }

    public SteamAPICall_t StopPlaytimeTrackingForAllItems(IntPtr _)
    {
        return SteamEmulator.SteamUGC.StopPlaytimeTrackingForAllItems();
    }

    public SteamAPICall_t AddDependency(IntPtr _, ulong nParentPublishedFileID, ulong nChildPublishedFileID)
    {
        return SteamEmulator.SteamUGC.AddDependency(nParentPublishedFileID, nChildPublishedFileID);
    }

    public SteamAPICall_t RemoveDependency(IntPtr _, ulong nParentPublishedFileID, ulong nChildPublishedFileID)
    {
        return SteamEmulator.SteamUGC.RemoveDependency(nParentPublishedFileID, nChildPublishedFileID);
    }

    public SteamAPICall_t AddAppDependency(IntPtr _, ulong nPublishedFileID, uint nAppID)
    {
        return SteamEmulator.SteamUGC.AddAppDependency(nPublishedFileID, nAppID);
    }

    public SteamAPICall_t RemoveAppDependency(IntPtr _, ulong nPublishedFileID, uint nAppID)
    {
        return SteamEmulator.SteamUGC.RemoveAppDependency(nPublishedFileID, nAppID);
    }

    public SteamAPICall_t GetAppDependencies(IntPtr _, ulong nPublishedFileID)
    {
        return SteamEmulator.SteamUGC.GetAppDependencies(nPublishedFileID);
    }

    public SteamAPICall_t DeleteItem(IntPtr _, ulong nPublishedFileID)
    {
        return SteamEmulator.SteamUGC.DeleteItem(nPublishedFileID);
    }


}
}
