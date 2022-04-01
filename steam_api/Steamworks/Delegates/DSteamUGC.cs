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
    [Delegate(Name = "SteamUGC")]
    public class DSteamUGC 
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate UGCQueryHandle_t CreateQueryUserUGCRequest(IntPtr _, IntPtr unAccountID, IntPtr eListType, int eMatchingUGCType, int eSortOrder, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate UGCQueryHandle_t CreateQueryAllUGCRequest(IntPtr _, int eQueryType, int eMatchingeMatchingUGCTypeFileType, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate UGCQueryHandle_t CreateQueryUGCDetailsRequest(IntPtr _, PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t SendQueryUGCRequest(IntPtr _, UGCQueryHandle_t handle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetQueryUGCResult(IntPtr _, UGCQueryHandle_t handle, uint index, IntPtr pDetails);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetQueryUGCPreviewURL(IntPtr _, UGCQueryHandle_t handle, uint index, string pchURL, uint cchURLSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetQueryUGCMetadata(IntPtr _, UGCQueryHandle_t handle, uint index, string pchMetadata, uint cchMetadatasize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetQueryUGCChildren(IntPtr _, UGCQueryHandle_t handle, uint index, PublishedFileId_t pvecPublishedFileID, uint cMaxEntries);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetQueryUGCStatistic(IntPtr _, UGCQueryHandle_t handle, uint index, EItemStatistic eStatType, uint pStatValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetQueryUGCNumAdditionalPreviews(IntPtr _, UGCQueryHandle_t handle, uint index);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetQueryUGCAdditionalPreview(IntPtr _, UGCQueryHandle_t handle, uint index, uint previewIndex, string pchURLOrVideoID, uint cchURLSize, string pchOriginalFileName, uint cchOriginalFileNameSize, EItemPreviewType pPreviewType);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetQueryUGCNumKeyValueTags(IntPtr _, UGCQueryHandle_t handle, uint index);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetQueryUGCKeyValueTag(IntPtr _, UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, string pchKey, uint cchKeySize, string pchValue, uint cchValueSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ReleaseQueryUGCRequest(IntPtr _, UGCQueryHandle_t handle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddRequiredTag(IntPtr _, UGCQueryHandle_t handle, string pTagName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddRequiredTagGroup(IntPtr _, UGCQueryHandle_t handle, IntPtr pTagGroups); // match any of the tags in this group

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddExcludedTag(IntPtr _, UGCQueryHandle_t handle, string pTagName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetReturnOnlyIDs(IntPtr _, UGCQueryHandle_t handle, bool bReturnOnlyIDs);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetReturnKeyValueTags(IntPtr _, UGCQueryHandle_t handle, bool bReturnKeyValueTags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetReturnLongDescription(IntPtr _, UGCQueryHandle_t handle, bool bReturnLongDescription);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetReturnMetadata(IntPtr _, UGCQueryHandle_t handle, bool bReturnMetadata);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetReturnChildren(IntPtr _, UGCQueryHandle_t handle, bool bReturnChildren);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetReturnAdditionalPreviews(IntPtr _, UGCQueryHandle_t handle, bool bReturnAdditionalPreviews);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetReturnTotalOnly(IntPtr _, UGCQueryHandle_t handle, bool bReturnTotalOnly);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetReturnPlaytimeStats(IntPtr _, UGCQueryHandle_t handle, uint unDays);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetLanguage(IntPtr _, UGCQueryHandle_t handle, string pchLanguage);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetAllowCachedResponse(IntPtr _, UGCQueryHandle_t handle, uint unMaxAgeSeconds);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetCloudFileNameFilter(IntPtr _, UGCQueryHandle_t handle, string pMatchCloudFileName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetMatchAnyTag(IntPtr _, UGCQueryHandle_t handle, bool bMatchAnyTag);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetSearchText(IntPtr _, UGCQueryHandle_t handle, string pSearchText);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetRankedByTrendDays(IntPtr _, UGCQueryHandle_t handle, uint unDays);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddRequiredKeyValueTag(IntPtr _, UGCQueryHandle_t handle, string pKey, string pValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RequestUGCDetails(IntPtr _, PublishedFileId_t nPublishedFileID, uint unMaxAgeSeconds);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t CreateItem(IntPtr _, AppId_t nConsumerAppId, EWorkshopFileType eFileType); // create new item for this app with no content attached yet

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate UGCUpdateHandle_t StartItemUpdate(IntPtr _, AppId_t nConsumerAppId, PublishedFileId_t nPublishedFileID); // start an UGC item update. Set changed properties before commiting update with CommitItemUpdate(IntPtr _, )

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetItemTitle(IntPtr _, UGCUpdateHandle_t handle, string pchTitle); // change the title of an UGC item

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetItemDescription(IntPtr _, UGCUpdateHandle_t handle, string pchDescription); // change the description of an UGC item

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetItemUpdateLanguage(IntPtr _, UGCUpdateHandle_t handle, string pchLanguage); // specify the language of the title or description that will be set

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetItemMetadata(IntPtr _, UGCUpdateHandle_t handle, string pchMetaData); // change the metadata of an UGC item (IntPtr _, max = k_cchDeveloperMetadataMax)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetItemVisibility(IntPtr _, UGCUpdateHandle_t handle, ERemoteStoragePublishedFileVisibility eVisibility); // change the visibility of an UGC item

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetItemTags(IntPtr _, UGCUpdateHandle_t updateHandle, IntPtr pTags); // change the tags of an UGC item

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetItemContent(IntPtr _, UGCUpdateHandle_t handle, string pszContentFolder); // update item content from this local folder

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetItemPreview(IntPtr _, UGCUpdateHandle_t handle, string pszPreviewFile); //  change preview image file for this item. pszPreviewFile points to local image file, which must be under 1MB in size

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetAllowLegacyUpload(IntPtr _, UGCUpdateHandle_t handle, bool bAllowLegacyUpload); //  use legacy upload for a single small file. The parameter to SetItemContent(IntPtr _, ) should either be a directory with one file or the full path to the file.  The file must also be less than 10MB in size.

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RemoveAllItemKeyValueTags(IntPtr _, UGCUpdateHandle_t handle); // remove all existing key-value tags (IntPtr _, you can add new ones via the AddItemKeyValueTag function)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RemoveItemKeyValueTags(IntPtr _, UGCUpdateHandle_t handle, string pchKey); // remove any existing key-value tags with the specified key

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddItemKeyValueTag(IntPtr _, UGCUpdateHandle_t handle, string pchKey, string pchValue); // add new key-value tags for the item. Note that there can be multiple values for a tag.

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddItemPreviewFile(IntPtr _, UGCUpdateHandle_t handle, string pszPreviewFile, EItemPreviewType type); //  add preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddItemPreviewVideo(IntPtr _, UGCUpdateHandle_t handle, string pszVideoID); //  add preview video for this item

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdateItemPreviewFile(IntPtr _, UGCUpdateHandle_t handle, uint index, string pszPreviewFile); //  updates an existing preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdateItemPreviewVideo(IntPtr _, UGCUpdateHandle_t handle, uint index, string pszVideoID); //  updates an existing preview video for this item

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RemoveItemPreview(IntPtr _, UGCUpdateHandle_t handle, uint index); // remove a preview by index starting at 0 (IntPtr _, previews are sorted)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t SubmitItemUpdate(IntPtr _, UGCUpdateHandle_t handle, string pchChangeNote); // commit update process started with StartItemUpdate(IntPtr _, )

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetItemUpdateProgress(IntPtr _, UGCUpdateHandle_t handle, uint punBytesProcessed, uint punBytesTotal);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t SetUserItemVote(IntPtr _, PublishedFileId_t nPublishedFileID, bool bVoteUp);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t GetUserItemVote(IntPtr _, PublishedFileId_t nPublishedFileID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t AddItemToFavorites(IntPtr _, AppId_t nAppId, PublishedFileId_t nPublishedFileID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RemoveItemFromFavorites(IntPtr _, AppId_t nAppId, PublishedFileId_t nPublishedFileID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t SubscribeItem(IntPtr _, PublishedFileId_t nPublishedFileID); // subscribe to this item, will be installed ASAP

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t UnsubscribeItem(IntPtr _, PublishedFileId_t nPublishedFileID); // unsubscribe from this item, will be uninstalled after game quits

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetNumSubscribedItems(IntPtr _); // number of subscribed items 

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetSubscribedItems(IntPtr _, PublishedFileId_t pvecPublishedFileID, uint cMaxEntries); // all subscribed item PublishFileIDs

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetItemState(IntPtr _, PublishedFileId_t nPublishedFileID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetItemInstallInfo(IntPtr _, PublishedFileId_t nPublishedFileID, uint punSizeOnDisk, string pchFolder, uint cchFolderSize, uint punTimeStamp);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetItemDownloadInfo(IntPtr _, PublishedFileId_t nPublishedFileID, uint punBytesDownloaded, uint punBytesTotal);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool DownloadItem(IntPtr _, PublishedFileId_t nPublishedFileID, bool bHighPriority);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BInitWorkshopForGameServer(IntPtr _, uint unWorkshopDepotID, string pszFolder);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SuspendDownloads(IntPtr _, bool bSuspend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t StartPlaytimeTracking(IntPtr _, PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t StopPlaytimeTracking(IntPtr _, PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t StopPlaytimeTrackingForAllItems(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t AddDependency(IntPtr _, PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RemoveDependency(IntPtr _, PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t AddAppDependency(IntPtr _, PublishedFileId_t nPublishedFileID, AppId_t nAppID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RemoveAppDependency(IntPtr _, PublishedFileId_t nPublishedFileID, AppId_t nAppID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t GetAppDependencies(IntPtr _, PublishedFileId_t nPublishedFileID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t DeleteItem(IntPtr _, PublishedFileId_t nPublishedFileID);

    }
}
