using Core.Interface;
using SKYNET.Interface;
using Steamworks;
using System;

namespace SKYNET.Managers
{
    //[Map("STEAMUGC_INTERFACE_VERSION")]
    //[Map("SteamUGC")]
    public class SteamUGC : IBaseInterface, ISteamUGC
    {
        public UGCQueryHandle_t CreateQueryUserUGCRequest(IntPtr unAccountID, IntPtr eListType, int eMatchingUGCType, int eSortOrder, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage)
        {
            return default;
        }

        public UGCQueryHandle_t CreateQueryAllUGCRequest(int eQueryType, int eMatchingeMatchingUGCTypeFileType, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage)
        {
            return default;
        }

        public UGCQueryHandle_t CreateQueryAllUGCRequest(int eQueryType, int eMatchingeMatchingUGCTypeFileType, AppId_t nCreatorAppID, AppId_t nConsumerAppID, string pchCursor)
        {
            return default;
        }

        public UGCQueryHandle_t CreateQueryUGCDetailsRequest(PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs)
        {
            return default;
        }

        public SteamAPICall_t SendQueryUGCRequest(UGCQueryHandle_t handle)
        {
            return default;
        }

        public bool GetQueryUGCResult(UGCQueryHandle_t handle, uint index, IntPtr pDetails)
        {
            return false;
        }

        public bool GetQueryUGCPreviewURL(UGCQueryHandle_t handle, uint index, string pchURL, uint cchURLSize)
        {
            return false;
        }

        public bool GetQueryUGCMetadata(UGCQueryHandle_t handle, uint index, string pchMetadata, uint cchMetadatasize)
        {
            return false;
        }

        public bool GetQueryUGCChildren(UGCQueryHandle_t handle, uint index, PublishedFileId_t pvecPublishedFileID, uint cMaxEntries)
        {
            return false;
        }

        public bool GetQueryUGCStatistic(UGCQueryHandle_t handle, uint index, EItemStatistic eStatType, uint pStatValue)
        {
            return false;
        }

        public uint GetQueryUGCNumAdditionalPreviews(UGCQueryHandle_t handle, uint index)
        {
            return 0;
        }

        public bool GetQueryUGCAdditionalPreview(UGCQueryHandle_t handle, uint index, uint previewIndex, string pchURLOrVideoID, uint cchURLSize, string pchOriginalFileName, uint cchOriginalFileNameSize, EItemPreviewType pPreviewType)
        {
            return false;
        }

        public uint GetQueryUGCNumKeyValueTags(UGCQueryHandle_t handle, uint index)
        {
            return 0;
        }

        public bool GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, string pchKey, uint cchKeySize, string pchValue, uint cchValueSize)
        {
            return false;
        }

        public bool GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, string pchKey, string pchValue, uint cchValueSize)
        {
            return false;
        }

        public bool ReleaseQueryUGCRequest(UGCQueryHandle_t handle)
        {
            return false;
        }

        public bool AddRequiredTag(UGCQueryHandle_t handle, string pTagName)
        {
            return false;
        }

        public bool AddRequiredTagGroup(UGCQueryHandle_t handle, IntPtr pTagGroups) // match any of the tags in this group 
        {
            return false;
        }

        public bool AddExcludedTag(UGCQueryHandle_t handle, string pTagName)
        {
            return false;
        }

        public bool SetReturnOnlyIDs(UGCQueryHandle_t handle, bool bReturnOnlyIDs)
        {
            return false;
        }

        public bool SetReturnKeyValueTags(UGCQueryHandle_t handle, bool bReturnKeyValueTags)
        {
            return false;
        }

        public bool SetReturnLongDescription(UGCQueryHandle_t handle, bool bReturnLongDescription)
        {
            return false;
        }

        public bool SetReturnMetadata(UGCQueryHandle_t handle, bool bReturnMetadata)
        {
            return false;
        }

        public bool SetReturnChildren(UGCQueryHandle_t handle, bool bReturnChildren)
        {
            return false;
        }

        public bool SetReturnAdditionalPreviews(UGCQueryHandle_t handle, bool bReturnAdditionalPreviews)
        {
            return false;
        }

        public bool SetReturnTotalOnly(UGCQueryHandle_t handle, bool bReturnTotalOnly)
        {
            return false;
        }

        public bool SetReturnPlaytimeStats(UGCQueryHandle_t handle, uint unDays)
        {
            return false;
        }

        public bool SetLanguage(UGCQueryHandle_t handle, string pchLanguage)
        {
            return false;
        }

        public bool SetAllowCachedResponse(UGCQueryHandle_t handle, uint unMaxAgeSeconds)
        {
            return false;
        }

        public bool SetCloudFileNameFilter(UGCQueryHandle_t handle, string pMatchCloudFileName)
        {
            return false;
        }

        public bool SetMatchAnyTag(UGCQueryHandle_t handle, bool bMatchAnyTag)
        {
            return false;
        }

        public bool SetSearchText(UGCQueryHandle_t handle, string pSearchText)
        {
            return false;
        }

        public bool SetRankedByTrendDays(UGCQueryHandle_t handle, uint unDays)
        {
            return false;
        }

        public bool AddRequiredKeyValueTag(UGCQueryHandle_t handle, string pKey, string pValue)
        {
            return false;
        }

        public SteamAPICall_t RequestUGCDetails(PublishedFileId_t nPublishedFileID, uint unMaxAgeSeconds)
        {
            return default;
        }

        public SteamAPICall_t CreateItem(AppId_t nConsumerAppId, EWorkshopFileType eFileType) // create new item for this app with no content attached yet 
        {
            return default;
        }

        public UGCUpdateHandle_t StartItemUpdate(AppId_t nConsumerAppId, PublishedFileId_t nPublishedFileID) // start an UGC item update. Set changed properties before commiting update with CommitItemUpdate() 
        {
            return default;
        }

        public bool SetItemTitle(UGCUpdateHandle_t handle, string pchTitle) // change the title of an UGC item 
        {
            return false;
        }

        public bool SetItemDescription(UGCUpdateHandle_t handle, string pchDescription) // change the description of an UGC item 
        {
            return false;
        }

        public bool SetItemUpdateLanguage(UGCUpdateHandle_t handle, string pchLanguage) // specify the language of the title or description that will be set 
        {
            return false;
        }

        public bool SetItemMetadata(UGCUpdateHandle_t handle, string pchMetaData) // change the metadata of an UGC item (max = k_cchDeveloperMetadataMax) 
        {
            return false;
        }

        public bool SetItemVisibility(UGCUpdateHandle_t handle, ERemoteStoragePublishedFileVisibility eVisibility) // change the visibility of an UGC item 
        {
            return false;
        }

        public bool SetItemTags(UGCUpdateHandle_t updateHandle, IntPtr pTags) // change the tags of an UGC item 
        {
            return false;
        }

        public bool SetItemContent(UGCUpdateHandle_t handle, string pszContentFolder) // update item content from this local folder 
        {
            return false;
        }

        public bool SetItemPreview(UGCUpdateHandle_t handle, string pszPreviewFile) //  change preview image file for this item. pszPreviewFile points to local image file, which must be under 1MB in size 
        {
            return false;
        }

        public bool SetAllowLegacyUpload(UGCUpdateHandle_t handle, bool bAllowLegacyUpload) //  use legacy upload for a single small file. The parameter to SetItemContent() should either be a directory with one file or the full path to the file.  The file must also be less than 10MB in size. 
        {
            return false;
        }

        public bool RemoveAllItemKeyValueTags(UGCUpdateHandle_t handle) // remove all existing key-value tags (you can add new ones via the AddItemKeyValueTag function) 
        {
            return false;
        }

        public bool RemoveItemKeyValueTags(UGCUpdateHandle_t handle, string pchKey) // remove any existing key-value tags with the specified key 
        {
            return false;
        }

        public bool AddItemKeyValueTag(UGCUpdateHandle_t handle, string pchKey, string pchValue) // add new key-value tags for the item. Note that there can be multiple values for a tag. 
        {
            return false;
        }

        public bool AddItemPreviewFile(UGCUpdateHandle_t handle, string pszPreviewFile, EItemPreviewType type) //  add preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size 
        {
            return false;
        }

        public bool AddItemPreviewVideo(UGCUpdateHandle_t handle, string pszVideoID) //  add preview video for this item 
        {
            return false;
        }

        public bool UpdateItemPreviewFile(UGCUpdateHandle_t handle, uint index, string pszPreviewFile) //  updates an existing preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size 
        {
            return false;
        }

        public bool UpdateItemPreviewVideo(UGCUpdateHandle_t handle, uint index, string pszVideoID) //  updates an existing preview video for this item 
        {
            return false;
        }

        public bool RemoveItemPreview(UGCUpdateHandle_t handle, uint index) // remove a preview by index starting at 0 (previews are sorted) 
        {
            return false;
        }

        public SteamAPICall_t SubmitItemUpdate(UGCUpdateHandle_t handle, string pchChangeNote) // commit update process started with StartItemUpdate() 
        {
            return default;
        }

        public int GetItemUpdateProgress(UGCUpdateHandle_t handle, uint punBytesProcessed, uint punBytesTotal)
        {
            return 0;
        }

        public SteamAPICall_t SetUserItemVote(PublishedFileId_t nPublishedFileID, bool bVoteUp)
        {
            return default;
        }

        public SteamAPICall_t GetUserItemVote(PublishedFileId_t nPublishedFileID)
        {
            return default;
        }

        public SteamAPICall_t AddItemToFavorites(AppId_t nAppId, PublishedFileId_t nPublishedFileID)
        {
            return default;
        }

        public SteamAPICall_t RemoveItemFromFavorites(AppId_t nAppId, PublishedFileId_t nPublishedFileID)
        {
            return default;
        }

        public SteamAPICall_t SubscribeItem(PublishedFileId_t nPublishedFileID) // subscribe to this item, will be installed ASAP 
        {
            return default;
        }

        public SteamAPICall_t UnsubscribeItem(PublishedFileId_t nPublishedFileID) // unsubscribe from this item, will be uninstalled after game quits 
        {
            return default;
        }

        public uint GetNumSubscribedItems() // number of subscribed items  
        {
            return 0;
        }

        public uint GetSubscribedItems(PublishedFileId_t pvecPublishedFileID, uint cMaxEntries) // all subscribed item PublishFileIDs 
        {
            return 0;
        }

        public uint GetItemState(PublishedFileId_t nPublishedFileID)
        {
            return 0;
        }

        public bool GetItemInstallInfo(PublishedFileId_t nPublishedFileID, uint punSizeOnDisk, string pchFolder, uint cchFolderSize, uint punTimeStamp)
        {
            return false;
        }

        public bool GetItemDownloadInfo(PublishedFileId_t nPublishedFileID, uint punBytesDownloaded, uint punBytesTotal)
        {
            return false;
        }

        public bool DownloadItem(PublishedFileId_t nPublishedFileID, bool bHighPriority)
        {
            return false;
        }

        public bool BInitWorkshopForGameServer(uint unWorkshopDepotID, string pszFolder)
        {
            return false;
        }

        public void SuspendDownloads(bool bSuspend)
        {
            //
        }

        public SteamAPICall_t StartPlaytimeTracking(PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs)
        {
            return default;
        }

        public SteamAPICall_t StopPlaytimeTracking(PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs)
        {
            return default;
        }

        public SteamAPICall_t StopPlaytimeTrackingForAllItems()
        {
            return default;
        }

        public SteamAPICall_t AddDependency(PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID)
        {
            return default;
        }

        public SteamAPICall_t RemoveDependency(PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID)
        {
            return default;
        }

        public SteamAPICall_t AddAppDependency(PublishedFileId_t nPublishedFileID, AppId_t nAppID)
        {
            return default;
        }

        public SteamAPICall_t RemoveAppDependency(PublishedFileId_t nPublishedFileID, AppId_t nAppID)
        {
            return default;
        }

        public SteamAPICall_t GetAppDependencies(PublishedFileId_t nPublishedFileID)
        {
            return default;
        }

        public SteamAPICall_t DeleteItem(PublishedFileId_t nPublishedFileID)
        {
            return default;
        }

    }

}