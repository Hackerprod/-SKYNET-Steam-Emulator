using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamUGC
    {
        // Query UGC associated with a user. Creator app id or consumer app id must be valid and be set to the current running app. unPage should start at 1.
        UGCQueryHandle_t CreateQueryUserUGCRequest(IntPtr unAccountID, IntPtr eListType, int eMatchingUGCType, int eSortOrder, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage);

        // Query for all matching UGC. Creator app id or consumer app id must be valid and be set to the current running app. unPage should start at 1.

        UGCQueryHandle_t CreateQueryAllUGCRequest(int eQueryType, int eMatchingeMatchingUGCTypeFileType, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage);

        // Query for all matching UGC using the new deep paging interface. Creator app id or consumer app id must be valid and be set to the current running app. pchCursor should be set to NULL or "" to get the first result set.

        UGCQueryHandle_t CreateQueryAllUGCRequest(int eQueryType, int eMatchingeMatchingUGCTypeFileType, AppId_t nCreatorAppID, AppId_t nConsumerAppID, string pchCursor);

        // Query for the details of the given published file ids (the RequestUGCDetails call is deprecated and replaced with this)
        UGCQueryHandle_t CreateQueryUGCDetailsRequest(PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs);

        // Send the query to Steam

        SteamAPICall_t SendQueryUGCRequest(UGCQueryHandle_t handle);

        // Retrieve an individual result after receiving the callback for querying UGC
        bool GetQueryUGCResult(UGCQueryHandle_t handle, uint index, IntPtr pDetails);
        bool GetQueryUGCPreviewURL(UGCQueryHandle_t handle, uint index, string pchURL, uint cchURLSize);
        bool GetQueryUGCMetadata(UGCQueryHandle_t handle, uint index, string pchMetadata, uint cchMetadatasize);
        bool GetQueryUGCChildren(UGCQueryHandle_t handle, uint index, PublishedFileId_t pvecPublishedFileID, uint cMaxEntries);
        bool GetQueryUGCStatistic(UGCQueryHandle_t handle, uint index, EItemStatistic eStatType, uint pStatValue);
        uint GetQueryUGCNumAdditionalPreviews(UGCQueryHandle_t handle, uint index);
        bool GetQueryUGCAdditionalPreview(UGCQueryHandle_t handle, uint index, uint previewIndex, string pchURLOrVideoID, uint cchURLSize, string pchOriginalFileName, uint cchOriginalFileNameSize, EItemPreviewType pPreviewType);
        uint GetQueryUGCNumKeyValueTags(UGCQueryHandle_t handle, uint index);

        bool GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, string pchKey, uint cchKeySize, string pchValue, uint cchValueSize);

        // Return the first value matching the pchKey. Note that a key may map to multiple values.  Returns false if there was an error or no matching value was found.

        bool GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, string pchKey, string pchValue, uint cchValueSize);

        // Release the request to free up memory, after retrieving results
        bool ReleaseQueryUGCRequest(UGCQueryHandle_t handle);

        // Options to set for querying UGC
        bool AddRequiredTag(UGCQueryHandle_t handle, string pTagName);
        bool AddRequiredTagGroup(UGCQueryHandle_t handle, IntPtr pTagGroups); // match any of the tags in this group
        bool AddExcludedTag(UGCQueryHandle_t handle, string pTagName);
        bool SetReturnOnlyIDs(UGCQueryHandle_t handle, bool bReturnOnlyIDs);
        bool SetReturnKeyValueTags(UGCQueryHandle_t handle, bool bReturnKeyValueTags);
        bool SetReturnLongDescription(UGCQueryHandle_t handle, bool bReturnLongDescription);
        bool SetReturnMetadata(UGCQueryHandle_t handle, bool bReturnMetadata);
        bool SetReturnChildren(UGCQueryHandle_t handle, bool bReturnChildren);
        bool SetReturnAdditionalPreviews(UGCQueryHandle_t handle, bool bReturnAdditionalPreviews);
        bool SetReturnTotalOnly(UGCQueryHandle_t handle, bool bReturnTotalOnly);
        bool SetReturnPlaytimeStats(UGCQueryHandle_t handle, uint unDays);
        bool SetLanguage(UGCQueryHandle_t handle, string pchLanguage);
        bool SetAllowCachedResponse(UGCQueryHandle_t handle, uint unMaxAgeSeconds);

        // Options only for querying user UGC
        bool SetCloudFileNameFilter(UGCQueryHandle_t handle, string pMatchCloudFileName);

        // Options only for querying all UGC
        bool SetMatchAnyTag(UGCQueryHandle_t handle, bool bMatchAnyTag);
        bool SetSearchText(UGCQueryHandle_t handle, string pSearchText);
        bool SetRankedByTrendDays(UGCQueryHandle_t handle, uint unDays);
        bool AddRequiredKeyValueTag(UGCQueryHandle_t handle, string pKey, string pValue);

        // DEPRECATED - Use CreateQueryUGCDetailsRequest call above instead!

        SteamAPICall_t RequestUGCDetails(PublishedFileId_t nPublishedFileID, uint unMaxAgeSeconds);

        // Steam Workshop Creator API

        SteamAPICall_t CreateItem(AppId_t nConsumerAppId, EWorkshopFileType eFileType); // create new item for this app with no content attached yet

        UGCUpdateHandle_t StartItemUpdate(AppId_t nConsumerAppId, PublishedFileId_t nPublishedFileID); // start an UGC item update. Set changed properties before commiting update with CommitItemUpdate()

        bool SetItemTitle(UGCUpdateHandle_t handle, string pchTitle); // change the title of an UGC item
        bool SetItemDescription(UGCUpdateHandle_t handle, string pchDescription); // change the description of an UGC item
        bool SetItemUpdateLanguage(UGCUpdateHandle_t handle, string pchLanguage); // specify the language of the title or description that will be set
        bool SetItemMetadata(UGCUpdateHandle_t handle, string pchMetaData); // change the metadata of an UGC item (max = k_cchDeveloperMetadataMax)
        bool SetItemVisibility(UGCUpdateHandle_t handle, ERemoteStoragePublishedFileVisibility eVisibility); // change the visibility of an UGC item
        bool SetItemTags(UGCUpdateHandle_t updateHandle, IntPtr pTags); // change the tags of an UGC item
        bool SetItemContent(UGCUpdateHandle_t handle, string pszContentFolder); // update item content from this local folder
        bool SetItemPreview(UGCUpdateHandle_t handle, string pszPreviewFile); //  change preview image file for this item. pszPreviewFile points to local image file, which must be under 1MB in size
        bool SetAllowLegacyUpload(UGCUpdateHandle_t handle, bool bAllowLegacyUpload); //  use legacy upload for a single small file. The parameter to SetItemContent() should either be a directory with one file or the full path to the file.  The file must also be less than 10MB in size.
        bool RemoveAllItemKeyValueTags(UGCUpdateHandle_t handle); // remove all existing key-value tags (you can add new ones via the AddItemKeyValueTag function)
        bool RemoveItemKeyValueTags(UGCUpdateHandle_t handle, string pchKey); // remove any existing key-value tags with the specified key
        bool AddItemKeyValueTag(UGCUpdateHandle_t handle, string pchKey, string pchValue); // add new key-value tags for the item. Note that there can be multiple values for a tag.
        bool AddItemPreviewFile(UGCUpdateHandle_t handle, string pszPreviewFile, EItemPreviewType type); //  add preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size
        bool AddItemPreviewVideo(UGCUpdateHandle_t handle, string pszVideoID); //  add preview video for this item
        bool UpdateItemPreviewFile(UGCUpdateHandle_t handle, uint index, string pszPreviewFile); //  updates an existing preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size
        bool UpdateItemPreviewVideo(UGCUpdateHandle_t handle, uint index, string pszVideoID); //  updates an existing preview video for this item
        bool RemoveItemPreview(UGCUpdateHandle_t handle, uint index); // remove a preview by index starting at 0 (previews are sorted)


        SteamAPICall_t SubmitItemUpdate(UGCUpdateHandle_t handle, string pchChangeNote); // commit update process started with StartItemUpdate()
        int GetItemUpdateProgress(UGCUpdateHandle_t handle, uint punBytesProcessed, uint punBytesTotal);

        // Steam Workshop Consumer API

        SteamAPICall_t SetUserItemVote(PublishedFileId_t nPublishedFileID, bool bVoteUp);

        SteamAPICall_t GetUserItemVote(PublishedFileId_t nPublishedFileID);

        SteamAPICall_t AddItemToFavorites(AppId_t nAppId, PublishedFileId_t nPublishedFileID);

        SteamAPICall_t RemoveItemFromFavorites(AppId_t nAppId, PublishedFileId_t nPublishedFileID);

        SteamAPICall_t SubscribeItem(PublishedFileId_t nPublishedFileID); // subscribe to this item, will be installed ASAP

        SteamAPICall_t UnsubscribeItem(PublishedFileId_t nPublishedFileID); // unsubscribe from this item, will be uninstalled after game quits
        uint GetNumSubscribedItems(); // number of subscribed items 
        uint GetSubscribedItems(PublishedFileId_t pvecPublishedFileID, uint cMaxEntries); // all subscribed item PublishFileIDs

        // get EItemState flags about item on this client
        uint GetItemState(PublishedFileId_t nPublishedFileID);

        // get info about currently installed content on disc for items that have k_EItemStateInstalled set
        // if k_EItemStateLegacyItem is set, pchFolder contains the path to the legacy file itself (not a folder)
        bool GetItemInstallInfo(PublishedFileId_t nPublishedFileID, uint punSizeOnDisk, string pchFolder, uint cchFolderSize, uint punTimeStamp);

        // get info about pending update for items that have k_EItemStateNeedsUpdate set. punBytesTotal will be valid after download started once
        bool GetItemDownloadInfo(PublishedFileId_t nPublishedFileID, uint punBytesDownloaded, uint punBytesTotal);

        // download new or update already installed item. If function returns true, wait for DownloadItemResult_t. If the item is already installed,
        // then files on disk should not be used until callback received. If item is not subscribed to, it will be cached for some time.
        // If bHighPriority is set, any other item download will be suspended and this item downloaded ASAP.
        bool DownloadItem(PublishedFileId_t nPublishedFileID, bool bHighPriority);

        // game servers can set a specific workshop folder before issuing any UGC commands.
        // This is helpful if you want to support multiple game servers running out of the same install folder
        bool BInitWorkshopForGameServer(uint unWorkshopDepotID, string pszFolder);

        // SuspendDownloads( true ) will suspend all workshop downloads until SuspendDownloads( false ) is called or the game ends
        void SuspendDownloads(bool bSuspend);

        // usage tracking

        SteamAPICall_t StartPlaytimeTracking(PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs);

        SteamAPICall_t StopPlaytimeTracking(PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs);

        SteamAPICall_t StopPlaytimeTrackingForAllItems();

        // parent-child relationship or dependency management

        SteamAPICall_t AddDependency(PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID);

        SteamAPICall_t RemoveDependency(PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID);

        // add/remove app dependence/requirements (usually DLC)

        SteamAPICall_t AddAppDependency(PublishedFileId_t nPublishedFileID, AppId_t nAppID);

        SteamAPICall_t RemoveAppDependency(PublishedFileId_t nPublishedFileID, AppId_t nAppID);
        // request app dependencies. note that whatever callback you register for GetAppDependenciesResult_t may be called multiple times
        // until all app dependencies have been returned

        SteamAPICall_t GetAppDependencies(PublishedFileId_t nPublishedFileID);

        // delete the item without prompting the user

        SteamAPICall_t DeleteItem(PublishedFileId_t nPublishedFileID);

    }
    public enum EItemPreviewType : int
    {
        k_EItemPreviewType_Image = 0,  // standard image file expected (e.g. jpg, png, gif, etc.)
        k_EItemPreviewType_YouTubeVideo = 1,   // video id is stored
        k_EItemPreviewType_Sketchfab = 2,  // model id is stored
        k_EItemPreviewType_EnvironmentMap_HorizontalCross = 3, // standard image file expected - cube map in the layout
                                                                // +---+---+-------+
                                                                // |   |Up |       |
                                                                // +---+---+---+---+
                                                                // | L | F | R | B |
                                                                // +---+---+---+---+
                                                                // |   |Dn |       |
                                                                // +---+---+---+---+
        k_EItemPreviewType_EnvironmentMap_LatLong = 4, // standard image file expected
        k_EItemPreviewType_ReservedMax = 255,  // you can specify your own types above this value
    };
    public enum EWorkshopFileType : int
    {
        k_EWorkshopFileTypeFirst = 0,

        k_EWorkshopFileTypeCommunity = 0,      // normal Workshop item that can be subscribed to
        k_EWorkshopFileTypeMicrotransaction = 1,       // Workshop item that is meant to be voted on for the purpose of selling in-game
        k_EWorkshopFileTypeCollection = 2,     // a collection of Workshop or Greenlight items
        k_EWorkshopFileTypeArt = 3,    // artwork
        k_EWorkshopFileTypeVideo = 4,      // external video
        k_EWorkshopFileTypeScreenshot = 5,     // screenshot
        k_EWorkshopFileTypeGame = 6,       // Greenlight game entry
        k_EWorkshopFileTypeSoftware = 7,       // Greenlight software entry
        k_EWorkshopFileTypeConcept = 8,    // Greenlight concept
        k_EWorkshopFileTypeWebGuide = 9,       // Steam web guide
        k_EWorkshopFileTypeIntegratedGuide = 10,       // application integrated guide
        k_EWorkshopFileTypeMerch = 11,     // Workshop merchandise meant to be voted on for the purpose of being sold
        k_EWorkshopFileTypeControllerBinding = 12,     // Steam Controller bindings
        k_EWorkshopFileTypeSteamworksAccessInvite = 13,    // internal
        k_EWorkshopFileTypeSteamVideo = 14,    // Steam video
        k_EWorkshopFileTypeGameManagedItem = 15,       // managed completely by the game, not the user, and not shown on the web

        // Update k_EWorkshopFileTypeMax if you add values.
        k_EWorkshopFileTypeMax = 16

    };
    public enum EItemStatistic : int
    {
        k_EItemStatistic_NumSubscriptions = 0,
        k_EItemStatistic_NumFavorites = 1,
        k_EItemStatistic_NumFollowers = 2,
        k_EItemStatistic_NumUniqueSubscriptions = 3,
        k_EItemStatistic_NumUniqueFavorites = 4,
        k_EItemStatistic_NumUniqueFollowers = 5,
        k_EItemStatistic_NumUniqueWebsiteViews = 6,
        k_EItemStatistic_ReportScore = 7,
        k_EItemStatistic_NumSecondsPlayed = 8,
        k_EItemStatistic_NumPlaytimeSessions = 9,
        k_EItemStatistic_NumComments = 10,
        k_EItemStatistic_NumSecondsPlayedDuringTimePeriod = 11,
        k_EItemStatistic_NumPlaytimeSessionsDuringTimePeriod = 12,
    };
}
