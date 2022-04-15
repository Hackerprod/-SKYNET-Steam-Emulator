using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamUGC : ISteamInterface
    {
        public SteamUGC()
        {
            InterfaceVersion = "SteamUGC";
        }

        public ulong CreateQueryUserUGCRequest(uint unAccountID, int eListType, int eMatchingUGCType, int eSortOrder, uint nCreatorAppID, uint nConsumerAppID, uint unPage)
        {
            Write($"CreateQueryUserUGCRequest for {unAccountID}");
            return 0;
        }

        public ulong CreateQueryAllUGCRequest(int eQueryType, int eMatchingeMatchingUGCTypeFileType, uint nCreatorAppID, uint nConsumerAppID, uint unPage)
        {
            Write("CreateQueryAllUGCRequest");
            return default;
        }

        public ulong CreateQueryAllUGCRequest(int eQueryType, int eMatchingeMatchingUGCTypeFileType, uint nCreatorAppID, uint nConsumerAppID, string pchCursor)
        {
            Write("CreateQueryAllUGCRequest");
            return default;
        }

        public ulong CreateQueryUGCDetailsRequest(ulong pvecPublishedFileID, uint unNumPublishedFileIDs)
        {
            Write("CreateQueryUGCDetailsRequest");
            return default;
        }

        public ulong SendQueryUGCRequest(ulong handle)
        {
            Write("SendQueryUGCRequest");
            return 0;
        }

        public bool GetQueryUGCResult(ulong handle, uint index, IntPtr pDetails)
        {
            Write("GetQueryUGCResult");
            return false;
        }

        public bool GetQueryUGCPreviewURL(ulong handle, uint index, string pchURL, uint cchURLSize)
        {
            Write("GetQueryUGCPreviewURL");
            return false;
        }

        public bool GetQueryUGCMetadata(ulong handle, uint index, string pchMetadata, uint cchMetadatasize)
        {
            Write("GetQueryUGCMetadata");
            return false;
        }

        internal uint GetQueryUGCNumTags(ulong handle, uint index)
        {
            Write("GetQueryUGCNumTags");
            return 0;
        }

        public bool GetQueryUGCChildren(ulong handle, uint index, ulong pvecPublishedFileID, uint cMaxEntries)
        {
            Write("GetQueryUGCChildren");
            return false;
        }

        internal bool GetQueryUGCTag(ulong handle, uint index, uint indexTag, string pchValue, uint cchValueSize)
        {
            Write("GetQueryUGCTag");
            return false;
        }

        internal bool GetQueryUGCTagDisplayName(ulong handle, uint index, uint indexTag, string pchValue, uint cchValueSize)
        {
            Write("GetQueryUGCTagDisplayName");
            return false;
        }

        public bool GetQueryUGCStatistic(ulong handle, uint index, int eStatType, ulong pStatValue)
        {
            Write("GetQueryUGCStatistic");
            return false;
        }

        public uint GetQueryUGCNumAdditionalPreviews(ulong handle, uint index)
        {
            Write("GetQueryUGCNumAdditionalPreviews");
            return 0;
        }

        public bool GetQueryUGCAdditionalPreview(ulong handle, uint index, uint previewIndex, string pchURLOrVideoID, uint cchURLSize, string pchOriginalFileName, uint cchOriginalFileNameSize, int pPreviewType)
        {
            Write("GetQueryUGCAdditionalPreview");
            return false;
        }

        public uint GetQueryUGCNumKeyValueTags(ulong handle, uint index)
        {
            Write("GetQueryUGCNumKeyValueTags");
            return 0;
        }

        public bool GetQueryUGCKeyValueTag(ulong handle, uint index, uint keyValueTagIndex, string pchKey, uint cchKeySize, string pchValue, uint cchValueSize)
        {
            Write("GetQueryUGCKeyValueTag");
            return false;
        }

        public bool GetQueryUGCKeyValueTag(ulong handle, uint index, string pchKey, string pchValue, uint cchValueSize)
        {
            Write("GetQueryUGCKeyValueTag");
            return false;
        }

        public bool ReleaseQueryUGCRequest(ulong handle)
        {
            Write("ReleaseQueryUGCRequest");
            return false;
        }

        public bool AddRequiredTag(ulong handle, string pTagName)
        {
            Write("AddRequiredTag");
            return false;
        }

        public bool AddRequiredTagGroup(ulong handle, IntPtr pTagGroups) // match any of the tags in this group 
        {
            Write("AddRequiredTagGroup");
            return false;
        }

        public bool AddExcludedTag(ulong handle, string pTagName)
        {
            Write("AddExcludedTag");
            return false;
        }

        public bool SetReturnOnlyIDs(ulong handle, bool bReturnOnlyIDs)
        {
            Write("SetReturnOnlyIDs");
            return false;
        }

        public bool SetReturnKeyValueTags(ulong handle, bool bReturnKeyValueTags)
        {
            Write("SetReturnKeyValueTags");
            return true;
        }

        public bool SetReturnLongDescription(ulong handle, bool bReturnLongDescription)
        {
            Write("SetReturnLongDescription");
            return true;
        }

        public bool SetReturnMetadata(ulong handle, bool bReturnMetadata)
        {
            Write("SetReturnMetadata");
            return false;
        }

        public bool SetReturnChildren(ulong handle, bool bReturnChildren)
        {
            Write("SetReturnChildren");
            return false;
        }

        public bool SetReturnAdditionalPreviews(ulong handle, bool bReturnAdditionalPreviews)
        {
            Write("SetReturnAdditionalPreviews");
            return false;
        }

        public bool SetReturnTotalOnly(ulong handle, bool bReturnTotalOnly)
        {
            Write("SetReturnTotalOnly");
            return false;
        }

        public bool SetReturnPlaytimeStats(ulong handle, uint unDays)
        {
            Write("SetReturnPlaytimeStats");
            return false;
        }

        public bool SetLanguage(ulong handle, string pchLanguage)
        {
            Write("SetLanguage");
            return false;
        }

        public bool SetAllowCachedResponse(ulong handle, uint unMaxAgeSeconds)
        {
            Write("SetAllowCachedResponse");
            return false;
        }

        public bool SetCloudFileNameFilter(ulong handle, string pMatchCloudFileName)
        {
            Write("SetCloudFileNameFilter");
            return false;
        }

        public bool SetMatchAnyTag(ulong handle, bool bMatchAnyTag)
        {
            Write("SetMatchAnyTag");
            return false;
        }

        public bool SetSearchText(ulong handle, string pSearchText)
        {
            Write("SetSearchText");
            return false;
        }

        public bool SetRankedByTrendDays(ulong handle, uint unDays)
        {
            Write("SetRankedByTrendDays");
            return false;
        }

        public bool AddRequiredKeyValueTag(ulong handle, string pKey, string pValue)
        {
            Write("AddRequiredKeyValueTag");
            return false;
        }

        public ulong RequestUGCDetails(ulong nPublishedFileID, uint unMaxAgeSeconds)
        {
            Write("RequestUGCDetails");
            return default;
        }

        public ulong CreateItem(uint nConsumerAppId, int eFileType)
        {
            Write("CreateItem");
            return default;
        }

        public ulong StartItemUpdate(uint nConsumerAppId, ulong nPublishedFileID) 
        {
            Write("StartItemUpdate");
            return default;
        }

        public bool SetItemTitle(ulong handle, string pchTitle) // change the title of an UGC item 
        {
            Write("SetItemTitle");
            return false;
        }

        public bool SetItemDescription(ulong handle, string pchDescription) // change the description of an UGC item 
        {
            Write("SetItemDescription");
            return false;
        }

        public bool SetItemUpdateLanguage(ulong handle, string pchLanguage) // specify the language of the title or description that will be set 
        {
            Write("SetItemUpdateLanguage");
            return false;
        }

        public bool SetItemMetadata(ulong handle, string pchMetaData)
        {
            Write("SetItemMetadata");
            return false;
        }

        public bool SetTimeCreatedDateRange(ulong handle, IntPtr rtStart, IntPtr rtEnd)
        {
            Write("SetTimeCreatedDateRange");
            return false;
        }

        public bool SetTimeUpdatedDateRange(ulong handle, IntPtr rtStart, IntPtr rtEnd)
        {
            Write("SetTimeUpdatedDateRange");
            return false;
        }

        public bool SetItemVisibility(ulong handle, int eVisibility) 
        {
            Write("SetItemVisibility");
            return false;
        }

        public bool SetItemTags(ulong updateHandle, IntPtr pTags)
        {
            Write("SetItemTags");
            return false;
        }

        public bool SetItemContent(ulong handle, string pszContentFolder) // update item content from this local folder 
        {
            Write("SetItemContent");
            return false;
        }

        public bool SetItemPreview(ulong handle, string pszPreviewFile) //  change preview image file for this item. pszPreviewFile points to local image file, which must be under 1MB in size 
        {
            Write("SetItemPreview");
            return false;
        }

        public bool SetAllowLegacyUpload(ulong handle, bool bAllowLegacyUpload) //  use legacy upload for a single small file. The parameter to SetItemContent() should either be a directory with one file or the full path to the file.  The file must also be less than 10MB in size. 
        {
            Write("SetAllowLegacyUpload");
            return false;
        }

        public bool RemoveAllItemKeyValueTags(ulong handle) // remove all existing key-value tags (you can add new ones via the AddItemKeyValueTag function) 
        {
            Write("RemoveAllItemKeyValueTags");
            return false;
        }

        public bool RemoveItemKeyValueTags(ulong handle, string pchKey) // remove any existing key-value tags with the specified key 
        {
            Write("RemoveItemKeyValueTags");
            return false;
        }

        public bool AddItemKeyValueTag(ulong handle, string pchKey, string pchValue) // add new key-value tags for the item. Note that there can be multiple values for a tag. 
        {
            Write("AddItemKeyValueTag");
            return false;
        }

        public bool AddItemPreviewFile(ulong handle, string pszPreviewFile, int type) //  add preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size 
        {
            Write("AddItemPreviewFile");
            return false;
        }

        public bool AddItemPreviewVideo(ulong handle, string pszVideoID) //  add preview video for this item 
        {
            Write("AddItemPreviewVideo");
            return false;
        }

        public bool UpdateItemPreviewFile(ulong handle, uint index, string pszPreviewFile) //  updates an existing preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size 
        {
            Write("UpdateItemPreviewFile");
            return false;
        }

        public bool UpdateItemPreviewVideo(ulong handle, uint index, string pszVideoID) //  updates an existing preview video for this item 
        {
            Write("UpdateItemPreviewVideo");
            return false;
        }

        public bool RemoveItemPreview(ulong handle, uint index) // remove a preview by index starting at 0 (previews are sorted) 
        {
            Write("RemoveItemPreview");
            return false;
        }

        public ulong SubmitItemUpdate(ulong handle, string pchChangeNote) 
        {
            Write("SubmitItemUpdate");
            return default;
        }

        public int GetItemUpdateProgress(ulong handle, ulong punBytesProcessed, ulong punBytesTotal)
        {
            Write("GetItemUpdateProgress");
            return 0;
        }

        public ulong SetUserItemVote(ulong nPublishedFileID, bool bVoteUp)
        {
            Write("SetUserItemVote");
            return default;
        }

        public ulong GetUserItemVote(ulong nPublishedFileID)
        {
            Write("GetUserItemVote");
            return default;
        }

        public ulong AddItemToFavorites(uint nAppId, ulong nPublishedFileID)
        {
            Write("AddItemToFavorites");
            return default;
        }

        public ulong RemoveItemFromFavorites(uint nAppId, ulong nPublishedFileID)
        {
            Write("RemoveItemFromFavorites");
            return default;
        }

        public ulong SubscribeItem(ulong nPublishedFileID) // subscribe to this item, will be installed ASAP 
        {
            Write("SubscribeItem");
            return default;
        }

        public ulong UnsubscribeItem(ulong nPublishedFileID) // unsubscribe from this item, will be uninstalled after game quits 
        {
            Write("UnsubscribeItem");
            return default;
        }

        public uint GetNumSubscribedItems() // number of subscribed items  
        {
            Write("GetNumSubscribedItems");
            return 0;
        }

        public uint GetSubscribedItems(ulong pvecPublishedFileID, uint cMaxEntries) // all subscribed item PublishFileIDs 
        {
            Write("GetSubscribedItems");
            return 0;
        }

        public uint GetItemState(ulong nPublishedFileID)
        {
            Write("GetItemState");
            return 0;
        }

        public bool GetItemInstallInfo(ulong nPublishedFileID, ulong punSizeOnDisk, string pchFolder, uint cchFolderSize, uint punTimeStamp)
        {
            Write("GetItemInstallInfo");
            return false;
        }

        public bool GetItemDownloadInfo(ulong nPublishedFileID, ulong punBytesDownloaded, ulong punBytesTotal)
        {
            Write("GetItemDownloadInfo");
            return false;
        }

        public bool DownloadItem(ulong nPublishedFileID, bool bHighPriority)
        {
            Write("DownloadItem");
            return false;
        }

        public bool BInitWorkshopForGameServer(uint unWorkshopDepotID, string pszFolder)
        {
            Write("BInitWorkshopForGameServer");
            return false;
        }

        public void SuspendDownloads(bool bSuspend)
        {
            Write("SuspendDownloads");
            //
        }

        public ulong StartPlaytimeTracking(ulong pvecPublishedFileID, uint unNumPublishedFileIDs)
        {
            Write("StartPlaytimeTracking");
            return default;
        }

        public ulong StopPlaytimeTracking(ulong pvecPublishedFileID, uint unNumPublishedFileIDs)
        {
            Write("StopPlaytimeTracking");
            return default;
        }

        public ulong StopPlaytimeTrackingForAllItems()
        {
            Write("StopPlaytimeTrackingForAllItems");
            return default;
        }

        public ulong AddDependency(ulong nParentPublishedFileID, ulong nChildPublishedFileID)
        {
            Write("AddDependency");
            return default;
        }

        public ulong RemoveDependency(ulong nParentPublishedFileID, ulong nChildPublishedFileID)
        {
            Write("RemoveDependency");
            return default;
        }

        public ulong AddAppDependency(ulong nPublishedFileID, uint nAppID)
        {
            Write("AddAppDependency");
            return default;
        }

        public ulong RemoveAppDependency(ulong nPublishedFileID, uint nAppID)
        {
            Write("RemoveAppDependency");
            return default;
        }

        public ulong GetAppDependencies(ulong nPublishedFileID)
        {
            Write("GetAppDependencies");
            return default;
        }

        public ulong DeleteItem(ulong nPublishedFileID)
        {
            Write("DeleteItem");
            return default;
        }

        public bool ShowWorkshopEULA()
        {
            Write("ShowWorkshopEULA");
            return false;
        }

        public ulong GetWorkshopEULAStatus()
        {
            Write("GetWorkshopEULAStatus");
            return 0;
        }
    }
}