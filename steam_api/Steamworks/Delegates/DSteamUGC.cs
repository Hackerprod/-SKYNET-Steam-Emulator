using SKYNET.Interface;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate("SteamUGC")]
    public class DSteamUGC : IBaseInterfaceMap
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate UGCQueryHandle_t CreateQueryUserUGCRequest(IntPtr unAccountID, IntPtr eListType, int eMatchingUGCType, int eSortOrder, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate UGCQueryHandle_t CreateQueryAllUGCRequest(int eQueryType, int eMatchingeMatchingUGCTypeFileType, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate UGCQueryHandle_t CreateQueryUGCDetailsRequest(PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t SendQueryUGCRequest(UGCQueryHandle_t handle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetQueryUGCResult(UGCQueryHandle_t handle, uint index, IntPtr pDetails);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetQueryUGCPreviewURL(UGCQueryHandle_t handle, uint index, string pchURL, uint cchURLSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetQueryUGCMetadata(UGCQueryHandle_t handle, uint index, string pchMetadata, uint cchMetadatasize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetQueryUGCChildren(UGCQueryHandle_t handle, uint index, PublishedFileId_t pvecPublishedFileID, uint cMaxEntries);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetQueryUGCStatistic(UGCQueryHandle_t handle, uint index, EItemStatistic eStatType, uint pStatValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetQueryUGCNumAdditionalPreviews(UGCQueryHandle_t handle, uint index);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetQueryUGCAdditionalPreview(UGCQueryHandle_t handle, uint index, uint previewIndex, string pchURLOrVideoID, uint cchURLSize, string pchOriginalFileName, uint cchOriginalFileNameSize, EItemPreviewType pPreviewType);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetQueryUGCNumKeyValueTags(UGCQueryHandle_t handle, uint index);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, string pchKey, uint cchKeySize, string pchValue, uint cchValueSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ReleaseQueryUGCRequest(UGCQueryHandle_t handle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddRequiredTag(UGCQueryHandle_t handle, string pTagName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddRequiredTagGroup(UGCQueryHandle_t handle, IntPtr pTagGroups); // match any of the tags in this group

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddExcludedTag(UGCQueryHandle_t handle, string pTagName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetReturnOnlyIDs(UGCQueryHandle_t handle, bool bReturnOnlyIDs);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetReturnKeyValueTags(UGCQueryHandle_t handle, bool bReturnKeyValueTags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetReturnLongDescription(UGCQueryHandle_t handle, bool bReturnLongDescription);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetReturnMetadata(UGCQueryHandle_t handle, bool bReturnMetadata);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetReturnChildren(UGCQueryHandle_t handle, bool bReturnChildren);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetReturnAdditionalPreviews(UGCQueryHandle_t handle, bool bReturnAdditionalPreviews);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetReturnTotalOnly(UGCQueryHandle_t handle, bool bReturnTotalOnly);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetReturnPlaytimeStats(UGCQueryHandle_t handle, uint unDays);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetLanguage(UGCQueryHandle_t handle, string pchLanguage);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetAllowCachedResponse(UGCQueryHandle_t handle, uint unMaxAgeSeconds);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetCloudFileNameFilter(UGCQueryHandle_t handle, string pMatchCloudFileName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetMatchAnyTag(UGCQueryHandle_t handle, bool bMatchAnyTag);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetSearchText(UGCQueryHandle_t handle, string pSearchText);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetRankedByTrendDays(UGCQueryHandle_t handle, uint unDays);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddRequiredKeyValueTag(UGCQueryHandle_t handle, string pKey, string pValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RequestUGCDetails(PublishedFileId_t nPublishedFileID, uint unMaxAgeSeconds);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t CreateItem(AppId_t nConsumerAppId, EWorkshopFileType eFileType); // create new item for this app with no content attached yet

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate UGCUpdateHandle_t StartItemUpdate(AppId_t nConsumerAppId, PublishedFileId_t nPublishedFileID); // start an UGC item update. Set changed properties before commiting update with CommitItemUpdate()

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetItemTitle(UGCUpdateHandle_t handle, string pchTitle); // change the title of an UGC item

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetItemDescription(UGCUpdateHandle_t handle, string pchDescription); // change the description of an UGC item

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetItemUpdateLanguage(UGCUpdateHandle_t handle, string pchLanguage); // specify the language of the title or description that will be set

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetItemMetadata(UGCUpdateHandle_t handle, string pchMetaData); // change the metadata of an UGC item (max = k_cchDeveloperMetadataMax)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetItemVisibility(UGCUpdateHandle_t handle, ERemoteStoragePublishedFileVisibility eVisibility); // change the visibility of an UGC item

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetItemTags(UGCUpdateHandle_t updateHandle, IntPtr pTags); // change the tags of an UGC item

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetItemContent(UGCUpdateHandle_t handle, string pszContentFolder); // update item content from this local folder

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetItemPreview(UGCUpdateHandle_t handle, string pszPreviewFile); //  change preview image file for this item. pszPreviewFile points to local image file, which must be under 1MB in size

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetAllowLegacyUpload(UGCUpdateHandle_t handle, bool bAllowLegacyUpload); //  use legacy upload for a single small file. The parameter to SetItemContent() should either be a directory with one file or the full path to the file.  The file must also be less than 10MB in size.

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RemoveAllItemKeyValueTags(UGCUpdateHandle_t handle); // remove all existing key-value tags (you can add new ones via the AddItemKeyValueTag function)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RemoveItemKeyValueTags(UGCUpdateHandle_t handle, string pchKey); // remove any existing key-value tags with the specified key

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddItemKeyValueTag(UGCUpdateHandle_t handle, string pchKey, string pchValue); // add new key-value tags for the item. Note that there can be multiple values for a tag.

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddItemPreviewFile(UGCUpdateHandle_t handle, string pszPreviewFile, EItemPreviewType type); //  add preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddItemPreviewVideo(UGCUpdateHandle_t handle, string pszVideoID); //  add preview video for this item

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdateItemPreviewFile(UGCUpdateHandle_t handle, uint index, string pszPreviewFile); //  updates an existing preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool UpdateItemPreviewVideo(UGCUpdateHandle_t handle, uint index, string pszVideoID); //  updates an existing preview video for this item

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RemoveItemPreview(UGCUpdateHandle_t handle, uint index); // remove a preview by index starting at 0 (previews are sorted)

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t SubmitItemUpdate(UGCUpdateHandle_t handle, string pchChangeNote); // commit update process started with StartItemUpdate()

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetItemUpdateProgress(UGCUpdateHandle_t handle, uint punBytesProcessed, uint punBytesTotal);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t SetUserItemVote(PublishedFileId_t nPublishedFileID, bool bVoteUp);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t GetUserItemVote(PublishedFileId_t nPublishedFileID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t AddItemToFavorites(AppId_t nAppId, PublishedFileId_t nPublishedFileID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RemoveItemFromFavorites(AppId_t nAppId, PublishedFileId_t nPublishedFileID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t SubscribeItem(PublishedFileId_t nPublishedFileID); // subscribe to this item, will be installed ASAP

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t UnsubscribeItem(PublishedFileId_t nPublishedFileID); // unsubscribe from this item, will be uninstalled after game quits

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetNumSubscribedItems(); // number of subscribed items 

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetSubscribedItems(PublishedFileId_t pvecPublishedFileID, uint cMaxEntries); // all subscribed item PublishFileIDs

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetItemState(PublishedFileId_t nPublishedFileID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetItemInstallInfo(PublishedFileId_t nPublishedFileID, uint punSizeOnDisk, string pchFolder, uint cchFolderSize, uint punTimeStamp);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetItemDownloadInfo(PublishedFileId_t nPublishedFileID, uint punBytesDownloaded, uint punBytesTotal);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool DownloadItem(PublishedFileId_t nPublishedFileID, bool bHighPriority);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BInitWorkshopForGameServer(uint unWorkshopDepotID, string pszFolder);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SuspendDownloads(bool bSuspend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t StartPlaytimeTracking(PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t StopPlaytimeTracking(PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t StopPlaytimeTrackingForAllItems();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t AddDependency(PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RemoveDependency(PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t AddAppDependency(PublishedFileId_t nPublishedFileID, AppId_t nAppID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RemoveAppDependency(PublishedFileId_t nPublishedFileID, AppId_t nAppID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t GetAppDependencies(PublishedFileId_t nPublishedFileID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t DeleteItem(PublishedFileId_t nPublishedFileID);

    }
}
