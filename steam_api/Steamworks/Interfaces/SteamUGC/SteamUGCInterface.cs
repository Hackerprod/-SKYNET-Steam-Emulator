using System;
using System.Runtime.InteropServices;

using SteamAPICall_t = System.UInt64;
using PublishedFileId_t = System.UInt64;
using UGCQueryHandle_t = System.UInt64;
using UGCUpdateHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("STEAMUGC_INTERFACE_VERSION010",
        "CreateQueryUserUGCRequest", "CreateQueryAllUGCRequest", "CreateQueryUGCDetailsRequest", "SendQueryUGCRequest",
        "GetQueryUGCResult_old", "GetQueryUGCPreviewURL", "GetQueryUGCMetadata", "GetQueryUGCChildren",
        "GetQueryUGCStatistic", "GetQueryUGCNumAdditionalPreviews", "GetQueryUGCAdditionalPreview", "GetQueryUGCNumKeyValueTags",
        "GetQueryUGCKeyValueTag", "ReleaseQueryUGCRequest", "AddRequiredTag", "AddExcludedTag",
        "SetReturnOnlyIDs", "SetReturnKeyValueTags", "SetReturnLongDescription", "SetReturnMetadata",
        "SetReturnChildren", "SetReturnAdditionalPreviews", "SetReturnTotalOnly", "SetReturnPlaytimeStats",
        "SetLanguage", "SetAllowCachedResponse", "SetCloudFileNameFilter", "SetMatchAnyTag",
        "SetSearchText", "SetRankedByTrendDays", "AddRequiredKeyValueTag", "RequestUGCDetails_old",
        "CreateItem", "StartItemUpdate", "SetItemTitle", "SetItemDescription",
        "SetItemUpdateLanguage", "SetItemMetadata", "SetItemVisibility", "SetItemTags",
        "SetItemContent", "SetItemPreview", "RemoveItemKeyValueTags", "AddItemKeyValueTag",
        "AddItemPreviewFile", "AddItemPreviewVideo", "UpdateItemPreviewFile", "UpdateItemPreviewVideo",
        "RemoveItemPreview", "SubmitItemUpdate", "GetItemUpdateProgress", "SetUserItemVote",
        "GetUserItemVote", "AddItemToFavorites", "RemoveItemFromFavorites", "SubscribeItem",
        "UnsubscribeItem", "GetNumSubscribedItems", "GetSubscribedItems", "GetItemState",
        "GetItemInstallInfo", "GetItemDownloadInfo", "DownloadItem", "BInitWorkshopForGameServer",
        "SuspendDownloads", "StartPlaytimeTracking", "StopPlaytimeTracking", "StopPlaytimeTrackingForAllItems",
        "AddDependency", "RemoveDependency", "AddAppDependency", "RemoveAppDependency",
        "GetAppDependencies", "DeleteItem")]
    [InterfaceLayout("STEAMUGC_INTERFACE_VERSION012",
        "CreateQueryUserUGCRequest", "CreateQueryAllUGCRequest__V012", "CreateQueryAllUGCRequest", "CreateQueryUGCDetailsRequest",
        "SendQueryUGCRequest", "GetQueryUGCResult_old", "GetQueryUGCPreviewURL", "GetQueryUGCMetadata",
        "GetQueryUGCChildren", "GetQueryUGCStatistic", "GetQueryUGCNumAdditionalPreviews", "GetQueryUGCAdditionalPreview",
        "GetQueryUGCNumKeyValueTags", "GetQueryUGCKeyValueTag", "ReleaseQueryUGCRequest", "AddRequiredTag",
        "AddExcludedTag", "SetReturnOnlyIDs", "SetReturnKeyValueTags", "SetReturnLongDescription",
        "SetReturnMetadata", "SetReturnChildren", "SetReturnAdditionalPreviews", "SetReturnTotalOnly",
        "SetReturnPlaytimeStats", "SetLanguage", "SetAllowCachedResponse", "SetCloudFileNameFilter",
        "SetMatchAnyTag", "SetSearchText", "SetRankedByTrendDays", "AddRequiredKeyValueTag",
        "RequestUGCDetails_old", "CreateItem", "StartItemUpdate", "SetItemTitle",
        "SetItemDescription", "SetItemUpdateLanguage", "SetItemMetadata", "SetItemVisibility",
        "SetItemTags", "SetItemContent", "SetItemPreview", "SetAllowLegacyUpload",
        "RemoveItemKeyValueTags", "AddItemKeyValueTag", "AddItemPreviewFile", "AddItemPreviewVideo",
        "UpdateItemPreviewFile", "UpdateItemPreviewVideo", "RemoveItemPreview", "SubmitItemUpdate",
        "GetItemUpdateProgress", "SetUserItemVote", "GetUserItemVote", "AddItemToFavorites",
        "RemoveItemFromFavorites", "SubscribeItem", "UnsubscribeItem", "GetNumSubscribedItems",
        "GetSubscribedItems", "GetItemState", "GetItemInstallInfo", "GetItemDownloadInfo",
        "DownloadItem", "BInitWorkshopForGameServer", "SuspendDownloads", "StartPlaytimeTracking",
        "StopPlaytimeTracking", "StopPlaytimeTrackingForAllItems", "AddDependency", "RemoveDependency",
        "AddAppDependency", "RemoveAppDependency", "GetAppDependencies", "DeleteItem")]
    [InterfaceLayout("STEAMUGC_INTERFACE_VERSION014",
        "CreateQueryUserUGCRequest", "CreateQueryAllUGCRequestCursor", "CreateQueryAllUGCRequestPage", "CreateQueryUGCDetailsRequest",
        "SendQueryUGCRequest", "GetQueryUGCResult_old", "GetQueryUGCPreviewURL", "GetQueryUGCMetadata",
        "GetQueryUGCChildren", "GetQueryUGCStatistic", "GetQueryUGCNumAdditionalPreviews", "GetQueryUGCAdditionalPreview",
        "GetQueryUGCNumKeyValueTags", "GetQueryFirstUGCKeyValueTag", "GetQueryUGCKeyValueTag", "ReleaseQueryUGCRequest",
        "AddRequiredTag", "AddRequiredTagGroup", "AddExcludedTag", "SetReturnOnlyIDs",
        "SetReturnKeyValueTags", "SetReturnLongDescription", "SetReturnMetadata", "SetReturnChildren",
        "SetReturnAdditionalPreviews", "SetReturnTotalOnly", "SetReturnPlaytimeStats", "SetLanguage",
        "SetAllowCachedResponse", "SetCloudFileNameFilter", "SetMatchAnyTag", "SetSearchText",
        "SetRankedByTrendDays", "AddRequiredKeyValueTag", "RequestUGCDetails_old", "CreateItem",
        "StartItemUpdate", "SetItemTitle", "SetItemDescription", "SetItemUpdateLanguage",
        "SetItemMetadata", "SetItemVisibility", "SetItemTags", "SetItemContent",
        "SetItemPreview", "SetAllowLegacyUpload", "RemoveAllItemKeyValueTags", "RemoveItemKeyValueTags",
        "AddItemKeyValueTag", "AddItemPreviewFile", "AddItemPreviewVideo", "UpdateItemPreviewFile",
        "UpdateItemPreviewVideo", "RemoveItemPreview", "SubmitItemUpdate", "GetItemUpdateProgress",
        "SetUserItemVote", "GetUserItemVote", "AddItemToFavorites", "RemoveItemFromFavorites",
        "SubscribeItem", "UnsubscribeItem", "GetNumSubscribedItems", "GetSubscribedItems",
        "GetItemState", "GetItemInstallInfo", "GetItemDownloadInfo", "DownloadItem",
        "BInitWorkshopForGameServer", "SuspendDownloads", "StartPlaytimeTracking", "StopPlaytimeTracking",
        "StopPlaytimeTrackingForAllItems", "AddDependency", "RemoveDependency", "AddAppDependency",
        "RemoveAppDependency", "GetAppDependencies", "DeleteItem")]
    [InterfaceLayout("STEAMUGC_INTERFACE_VERSION015",
        "CreateQueryUserUGCRequest", "CreateQueryAllUGCRequestCursor", "CreateQueryAllUGCRequestPage", "CreateQueryUGCDetailsRequest",
        "SendQueryUGCRequest", "GetQueryUGCResult_old", "GetQueryUGCNumTags", "GetQueryUGCTag",
        "GetQueryUGCTagDisplayName", "GetQueryUGCPreviewURL", "GetQueryUGCMetadata", "GetQueryUGCChildren",
        "GetQueryUGCStatistic", "GetQueryUGCNumAdditionalPreviews", "GetQueryUGCAdditionalPreview", "GetQueryUGCNumKeyValueTags",
        "GetQueryFirstUGCKeyValueTag", "GetQueryUGCKeyValueTag", "ReleaseQueryUGCRequest", "AddRequiredTag",
        "AddRequiredTagGroup", "AddExcludedTag", "SetReturnOnlyIDs", "SetReturnKeyValueTags",
        "SetReturnLongDescription", "SetReturnMetadata", "SetReturnChildren", "SetReturnAdditionalPreviews",
        "SetReturnTotalOnly", "SetReturnPlaytimeStats", "SetLanguage", "SetAllowCachedResponse",
        "SetCloudFileNameFilter", "SetMatchAnyTag", "SetSearchText", "SetRankedByTrendDays",
        "AddRequiredKeyValueTag", "RequestUGCDetails_old", "CreateItem", "StartItemUpdate",
        "SetItemTitle", "SetItemDescription", "SetItemUpdateLanguage", "SetItemMetadata",
        "SetItemVisibility", "SetItemTags", "SetItemContent", "SetItemPreview",
        "SetAllowLegacyUpload", "RemoveAllItemKeyValueTags", "RemoveItemKeyValueTags", "AddItemKeyValueTag",
        "AddItemPreviewFile", "AddItemPreviewVideo", "UpdateItemPreviewFile", "UpdateItemPreviewVideo",
        "RemoveItemPreview", "SubmitItemUpdate", "GetItemUpdateProgress", "SetUserItemVote",
        "GetUserItemVote", "AddItemToFavorites", "RemoveItemFromFavorites", "SubscribeItem",
        "UnsubscribeItem", "GetNumSubscribedItems", "GetSubscribedItems", "GetItemState",
        "GetItemInstallInfo", "GetItemDownloadInfo", "DownloadItem", "BInitWorkshopForGameServer",
        "SuspendDownloads", "StartPlaytimeTracking", "StopPlaytimeTracking", "StopPlaytimeTrackingForAllItems",
        "AddDependency", "RemoveDependency", "AddAppDependency", "RemoveAppDependency",
        "GetAppDependencies", "DeleteItem", "ShowWorkshopEULA", "GetWorkshopEULAStatus")]
    [InterfaceLayout("STEAMUGC_INTERFACE_VERSION016",
        "CreateQueryUserUGCRequest", "CreateQueryAllUGCRequestCursor", "CreateQueryAllUGCRequestPage", "CreateQueryUGCDetailsRequest",
        "SendQueryUGCRequest", "GetQueryUGCResult_old", "GetQueryUGCNumTags", "GetQueryUGCTag",
        "GetQueryUGCTagDisplayName", "GetQueryUGCPreviewURL", "GetQueryUGCMetadata", "GetQueryUGCChildren",
        "GetQueryUGCStatistic", "GetQueryUGCNumAdditionalPreviews", "GetQueryUGCAdditionalPreview", "GetQueryUGCNumKeyValueTags",
        "GetQueryFirstUGCKeyValueTag", "GetQueryUGCKeyValueTag", "ReleaseQueryUGCRequest", "AddRequiredTag",
        "AddRequiredTagGroup", "AddExcludedTag", "SetReturnOnlyIDs", "SetReturnKeyValueTags",
        "SetReturnLongDescription", "SetReturnMetadata", "SetReturnChildren", "SetReturnAdditionalPreviews",
        "SetReturnTotalOnly", "SetReturnPlaytimeStats", "SetLanguage", "SetAllowCachedResponse",
        "SetCloudFileNameFilter", "SetMatchAnyTag", "SetSearchText", "SetRankedByTrendDays",
        "SetTimeCreatedDateRange", "SetTimeUpdatedDateRange", "AddRequiredKeyValueTag", "RequestUGCDetails_old",
        "CreateItem", "StartItemUpdate", "SetItemTitle", "SetItemDescription",
        "SetItemUpdateLanguage", "SetItemMetadata", "SetItemVisibility", "SetItemTags",
        "SetItemContent", "SetItemPreview", "SetAllowLegacyUpload", "RemoveAllItemKeyValueTags",
        "RemoveItemKeyValueTags", "AddItemKeyValueTag", "AddItemPreviewFile", "AddItemPreviewVideo",
        "UpdateItemPreviewFile", "UpdateItemPreviewVideo", "RemoveItemPreview", "SubmitItemUpdate",
        "GetItemUpdateProgress", "SetUserItemVote", "GetUserItemVote", "AddItemToFavorites",
        "RemoveItemFromFavorites", "SubscribeItem", "UnsubscribeItem", "GetNumSubscribedItems",
        "GetSubscribedItems", "GetItemState", "GetItemInstallInfo", "GetItemDownloadInfo",
        "DownloadItem", "BInitWorkshopForGameServer", "SuspendDownloads", "StartPlaytimeTracking",
        "StopPlaytimeTracking", "StopPlaytimeTrackingForAllItems", "AddDependency", "RemoveDependency",
        "AddAppDependency", "RemoveAppDependency", "GetAppDependencies", "DeleteItem",
        "ShowWorkshopEULA", "GetWorkshopEULAStatus")]
    [InterfaceLayout("STEAMUGC_INTERFACE_VERSION017",
        "CreateQueryUserUGCRequest", "CreateQueryAllUGCRequestCursor", "CreateQueryAllUGCRequestPage", "CreateQueryUGCDetailsRequest",
        "SendQueryUGCRequest", "GetQueryUGCResult_old", "GetQueryUGCNumTags", "GetQueryUGCTag",
        "GetQueryUGCTagDisplayName", "GetQueryUGCPreviewURL", "GetQueryUGCMetadata", "GetQueryUGCChildren",
        "GetQueryUGCStatistic", "GetQueryUGCNumAdditionalPreviews", "GetQueryUGCAdditionalPreview", "GetQueryUGCNumKeyValueTags",
        "GetQueryFirstUGCKeyValueTag", "GetQueryUGCKeyValueTag", "GetQueryUGCContentDescriptors", "ReleaseQueryUGCRequest",
        "AddRequiredTag", "AddRequiredTagGroup", "AddExcludedTag", "SetReturnOnlyIDs",
        "SetReturnKeyValueTags", "SetReturnLongDescription", "SetReturnMetadata", "SetReturnChildren",
        "SetReturnAdditionalPreviews", "SetReturnTotalOnly", "SetReturnPlaytimeStats", "SetLanguage",
        "SetAllowCachedResponse", "SetCloudFileNameFilter", "SetMatchAnyTag", "SetSearchText",
        "SetRankedByTrendDays", "SetTimeCreatedDateRange", "SetTimeUpdatedDateRange", "AddRequiredKeyValueTag",
        "RequestUGCDetails_old", "CreateItem", "StartItemUpdate", "SetItemTitle",
        "SetItemDescription", "SetItemUpdateLanguage", "SetItemMetadata", "SetItemVisibility",
        "SetItemTags", "SetItemContent", "SetItemPreview", "SetAllowLegacyUpload",
        "RemoveAllItemKeyValueTags", "RemoveItemKeyValueTags", "AddItemKeyValueTag", "AddItemPreviewFile",
        "AddItemPreviewVideo", "UpdateItemPreviewFile", "UpdateItemPreviewVideo", "RemoveItemPreview",
        "AddContentDescriptor", "RemoveContentDescriptor", "SubmitItemUpdate", "GetItemUpdateProgress",
        "SetUserItemVote", "GetUserItemVote", "AddItemToFavorites", "RemoveItemFromFavorites",
        "SubscribeItem", "UnsubscribeItem", "GetNumSubscribedItems", "GetSubscribedItems",
        "GetItemState", "GetItemInstallInfo", "GetItemDownloadInfo", "DownloadItem",
        "BInitWorkshopForGameServer", "SuspendDownloads", "StartPlaytimeTracking", "StopPlaytimeTracking",
        "StopPlaytimeTrackingForAllItems", "AddDependency", "RemoveDependency", "AddAppDependency",
        "RemoveAppDependency", "GetAppDependencies", "DeleteItem", "ShowWorkshopEULA",
        "GetWorkshopEULAStatus")]
    [InterfaceLayout("STEAMUGC_INTERFACE_VERSION019",
        "CreateQueryUserUGCRequest", "CreateQueryAllUGCRequestCursor", "CreateQueryAllUGCRequestPage", "CreateQueryUGCDetailsRequest",
        "SendQueryUGCRequest", "GetQueryUGCResult", "GetQueryUGCNumTags", "GetQueryUGCTag",
        "GetQueryUGCTagDisplayName", "GetQueryUGCPreviewURL", "GetQueryUGCMetadata", "GetQueryUGCChildren",
        "GetQueryUGCStatistic", "GetQueryUGCNumAdditionalPreviews", "GetQueryUGCAdditionalPreview", "GetQueryUGCNumKeyValueTags",
        "GetQueryFirstUGCKeyValueTag", "GetQueryUGCKeyValueTag", "GetQueryUGCContentDescriptors", "ReleaseQueryUGCRequest",
        "AddRequiredTag", "AddRequiredTagGroup", "AddExcludedTag", "SetReturnOnlyIDs",
        "SetReturnKeyValueTags", "SetReturnLongDescription", "SetReturnMetadata", "SetReturnChildren",
        "SetReturnAdditionalPreviews", "SetReturnTotalOnly", "SetReturnPlaytimeStats", "SetLanguage",
        "SetAllowCachedResponse", "SetAdminQuery", "SetCloudFileNameFilter", "SetMatchAnyTag",
        "SetSearchText", "SetRankedByTrendDays", "SetTimeCreatedDateRange", "SetTimeUpdatedDateRange",
        "AddRequiredKeyValueTag", "RequestUGCDetails_old", "CreateItem", "StartItemUpdate",
        "SetItemTitle", "SetItemDescription", "SetItemUpdateLanguage", "SetItemMetadata",
        "SetItemVisibility", "SetItemTags__V019", "SetItemContent", "SetItemPreview",
        "SetAllowLegacyUpload", "RemoveAllItemKeyValueTags", "RemoveItemKeyValueTags", "AddItemKeyValueTag",
        "AddItemPreviewFile", "AddItemPreviewVideo", "UpdateItemPreviewFile", "UpdateItemPreviewVideo",
        "RemoveItemPreview", "AddContentDescriptor", "RemoveContentDescriptor", "SubmitItemUpdate",
        "GetItemUpdateProgress", "SetUserItemVote", "GetUserItemVote", "AddItemToFavorites",
        "RemoveItemFromFavorites", "SubscribeItem", "UnsubscribeItem", "GetNumSubscribedItems",
        "GetSubscribedItems", "GetItemState", "GetItemInstallInfo", "GetItemDownloadInfo",
        "DownloadItem", "BInitWorkshopForGameServer", "SuspendDownloads", "StartPlaytimeTracking",
        "StopPlaytimeTracking", "StopPlaytimeTrackingForAllItems", "AddDependency", "RemoveDependency",
        "AddAppDependency", "RemoveAppDependency", "GetAppDependencies", "DeleteItem",
        "ShowWorkshopEULA", "GetWorkshopEULAStatus", "GetUserContentDescriptorPreferences")]
    [InterfaceLayout("STEAMUGC_INTERFACE_VERSION020",
        "CreateQueryUserUGCRequest", "CreateQueryAllUGCRequestCursor", "CreateQueryAllUGCRequestPage", "CreateQueryUGCDetailsRequest",
        "SendQueryUGCRequest", "GetQueryUGCResult", "GetQueryUGCNumTags", "GetQueryUGCTag",
        "GetQueryUGCTagDisplayName", "GetQueryUGCPreviewURL", "GetQueryUGCMetadata", "GetQueryUGCChildren",
        "GetQueryUGCStatistic", "GetQueryUGCNumAdditionalPreviews", "GetQueryUGCAdditionalPreview", "GetQueryUGCNumKeyValueTags",
        "GetQueryFirstUGCKeyValueTag", "GetQueryUGCKeyValueTag", "GetNumSupportedGameVersions", "GetSupportedGameVersionData",
        "GetQueryUGCContentDescriptors", "ReleaseQueryUGCRequest", "AddRequiredTag", "AddRequiredTagGroup",
        "AddExcludedTag", "SetReturnOnlyIDs", "SetReturnKeyValueTags", "SetReturnLongDescription",
        "SetReturnMetadata", "SetReturnChildren", "SetReturnAdditionalPreviews", "SetReturnTotalOnly",
        "SetReturnPlaytimeStats", "SetLanguage", "SetAllowCachedResponse", "SetAdminQuery",
        "SetCloudFileNameFilter", "SetMatchAnyTag", "SetSearchText", "SetRankedByTrendDays",
        "SetTimeCreatedDateRange", "SetTimeUpdatedDateRange", "AddRequiredKeyValueTag", "RequestUGCDetails",
        "CreateItem", "StartItemUpdate", "SetItemTitle", "SetItemDescription",
        "SetItemUpdateLanguage", "SetItemMetadata", "SetItemVisibility", "SetItemTags__V019",
        "SetItemContent", "SetItemPreview", "SetAllowLegacyUpload", "RemoveAllItemKeyValueTags",
        "RemoveItemKeyValueTags", "AddItemKeyValueTag", "AddItemPreviewFile", "AddItemPreviewVideo",
        "UpdateItemPreviewFile", "UpdateItemPreviewVideo", "RemoveItemPreview", "AddContentDescriptor",
        "RemoveContentDescriptor", "SetRequiredGameVersions", "SubmitItemUpdate", "GetItemUpdateProgress",
        "SetUserItemVote", "GetUserItemVote", "AddItemToFavorites", "RemoveItemFromFavorites",
        "SubscribeItem", "UnsubscribeItem", "GetNumSubscribedItems", "GetSubscribedItems",
        "GetItemState", "GetItemInstallInfo", "GetItemDownloadInfo", "DownloadItem",
        "BInitWorkshopForGameServer", "SuspendDownloads", "StartPlaytimeTracking", "StopPlaytimeTracking",
        "StopPlaytimeTrackingForAllItems", "AddDependency", "RemoveDependency", "AddAppDependency",
        "RemoveAppDependency", "GetAppDependencies", "DeleteItem", "ShowWorkshopEULA",
        "GetWorkshopEULAStatus", "GetUserContentDescriptorPreferences")]
    [InterfaceLayout("STEAMUGC_INTERFACE_VERSION021",
        "CreateQueryUserUGCRequest__V021", "CreateQueryAllUGCRequestCursor__V021", "CreateQueryAllUGCRequestPage__V021", "CreateQueryUGCDetailsRequest__V021",
        "SendQueryUGCRequest__V021", "GetQueryUGCResult__V021", "GetQueryUGCNumTags__V021", "GetQueryUGCTag__V021",
        "GetQueryUGCTagDisplayName__V021", "GetQueryUGCPreviewURL__V021", "GetQueryUGCMetadata__V021", "GetQueryUGCChildren__V021",
        "GetQueryUGCStatistic__V021", "GetQueryUGCNumAdditionalPreviews__V021", "GetQueryUGCAdditionalPreview__V021", "GetQueryUGCNumKeyValueTags__V021",
        "GetQueryFirstUGCKeyValueTag__V021", "GetQueryUGCKeyValueTag__V021", "GetNumSupportedGameVersions__V021", "GetSupportedGameVersionData__V021",
        "GetQueryUGCContentDescriptors__V021", "ReleaseQueryUGCRequest__V021", "AddRequiredTag__V021", "AddRequiredTagGroup__V021",
        "AddExcludedTag__V021", "SetReturnOnlyIDs__V021", "SetReturnKeyValueTags__V021", "SetReturnLongDescription__V021",
        "SetReturnMetadata__V021", "SetReturnChildren__V021", "SetReturnAdditionalPreviews__V021", "SetReturnTotalOnly__V021",
        "SetReturnPlaytimeStats__V021", "SetLanguage__V021", "SetAllowCachedResponse__V021", "SetAdminQuery__V021",
        "SetCloudFileNameFilter__V021", "SetMatchAnyTag__V021", "SetSearchText__V021", "SetRankedByTrendDays__V021",
        "SetTimeCreatedDateRange__V021", "SetTimeUpdatedDateRange__V021", "AddRequiredKeyValueTag__V021", "RequestUGCDetails__V021",
        "CreateItem__V021", "StartItemUpdate__V021", "SetItemTitle__V021", "SetItemDescription__V021",
        "SetItemUpdateLanguage__V021", "SetItemMetadata__V021", "SetItemVisibility__V021", "SetItemTags__V021",
        "SetItemContent__V021", "SetItemPreview__V021", "SetAllowLegacyUpload__V021", "RemoveAllItemKeyValueTags__V021",
        "RemoveItemKeyValueTags__V021", "AddItemKeyValueTag__V021", "AddItemPreviewFile__V021", "AddItemPreviewVideo__V021",
        "UpdateItemPreviewFile__V021", "UpdateItemPreviewVideo__V021", "RemoveItemPreview__V021", "AddContentDescriptor__V021",
        "RemoveContentDescriptor__V021", "SetRequiredGameVersions__V021", "SubmitItemUpdate__V021", "GetItemUpdateProgress__V021",
        "SetUserItemVote__V021", "GetUserItemVote__V021", "AddItemToFavorites__V021", "RemoveItemFromFavorites__V021",
        "SubscribeItem__V021", "UnsubscribeItem__V021", "GetNumSubscribedItems__V021", "GetSubscribedItems__V021",
        "GetItemState", "GetItemInstallInfo", "GetItemDownloadInfo__V021", "DownloadItem__V021",
        "BInitWorkshopForGameServer__V021", "SuspendDownloads__V021", "StartPlaytimeTracking__V021", "StopPlaytimeTracking__V021",
        "StopPlaytimeTrackingForAllItems__V021", "AddDependency__V021", "RemoveDependency__V021", "AddAppDependency__V021",
        "RemoveAppDependency__V021", "GetAppDependencies__V021", "DeleteItem__V021", "ShowWorkshopEULA__V021",
        "GetWorkshopEULAStatus__V021", "GetUserContentDescriptorPreferences__V021", "SetItemsDisabledLocally", "SetSubscriptionsLoadOrder",
        "MarkDownloadedItemAsUnused", "GetNumDownloadedItems", "GetDownloadedItems")]
    public class SteamUGCInterface : ISteamInterface
    {
        public UGCQueryHandle_t CreateQueryUserUGCRequest__V021(IntPtr _, uint unAccountID, int eListType, int eMatchingUGCType, int eSortOrder, uint nCreatorAppID, uint nConsumerAppID, uint unPage) { return SteamEmulator.SteamUGC.CreateQueryUserUGCRequest(unAccountID, eListType, eMatchingUGCType, eSortOrder, nCreatorAppID, nConsumerAppID, unPage); }

        public UGCQueryHandle_t CreateQueryAllUGCRequestCursor__V021(IntPtr _, int eQueryType, int eMatchingeMatchingUGCTypeFileType, uint nCreatorAppID, uint nConsumerAppID, IntPtr pchCursor)
        {
            if (TryReadPageFromSmallPointer(pchCursor, out uint unPage))
            {
                return SteamEmulator.SteamUGC.CreateQueryAllUGCRequest(eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, unPage);
            }

            return SteamEmulator.SteamUGC.CreateQueryAllUGCRequest(eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, ReadNativeString(pchCursor));
        }

        public UGCQueryHandle_t CreateQueryAllUGCRequestPage__V021(IntPtr _, int eQueryType, int eMatchingeMatchingUGCTypeFileType, uint nCreatorAppID, uint nConsumerAppID, uint unPage) { return SteamEmulator.SteamUGC.CreateQueryAllUGCRequest(eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, unPage); }

        public UGCQueryHandle_t CreateQueryUGCDetailsRequest__V021(IntPtr _, IntPtr pvecPublishedFileID, uint unNumPublishedFileIDs) { return SteamEmulator.SteamUGC.CreateQueryUGCDetailsRequest(pvecPublishedFileID, unNumPublishedFileIDs); }

        public SteamAPICall_t SendQueryUGCRequest__V021(IntPtr _, UGCQueryHandle_t handle) { return SteamEmulator.SteamUGC.SendQueryUGCRequest(handle); }

        public bool GetQueryUGCResult__V021(IntPtr _, UGCQueryHandle_t handle, uint index, ref SteamUGCDetails_t pDetails) { return SteamEmulator.SteamUGC.GetQueryUGCResult(handle, index, ref pDetails); }

        public uint GetQueryUGCNumTags__V021(IntPtr _, UGCQueryHandle_t handle, uint index) { return SteamEmulator.SteamUGC.GetQueryUGCNumTags(handle, index); }

        public bool GetQueryUGCTag__V021(IntPtr _, UGCQueryHandle_t handle, uint index, uint indexTag, IntPtr pchValue, uint cchValueSize) { return SteamEmulator.SteamUGC.GetQueryUGCTag(handle, index, indexTag, pchValue, cchValueSize); }

        public bool GetQueryUGCTagDisplayName__V021(IntPtr _, UGCQueryHandle_t handle, uint index, uint indexTag, IntPtr pchValue, uint cchValueSize) { return SteamEmulator.SteamUGC.GetQueryUGCTagDisplayName(handle, index, indexTag, pchValue, cchValueSize); }

        public bool GetQueryUGCPreviewURL__V021(IntPtr _, UGCQueryHandle_t handle, uint index, IntPtr pchURL, uint cchURLSize) { return SteamEmulator.SteamUGC.GetQueryUGCPreviewURL(handle, index, pchURL, cchURLSize); }

        public bool GetQueryUGCMetadata__V021(IntPtr _, UGCQueryHandle_t handle, uint index, IntPtr pchMetadata, uint cchMetadatasize) { return SteamEmulator.SteamUGC.GetQueryUGCMetadata(handle, index, pchMetadata, cchMetadatasize); }

        public bool GetQueryUGCChildren__V021(IntPtr _, UGCQueryHandle_t handle, uint index, IntPtr pvecPublishedFileID, uint cMaxEntries) { return SteamEmulator.SteamUGC.GetQueryUGCChildren(handle, index, pvecPublishedFileID, cMaxEntries); }

        public bool GetQueryUGCStatistic__V021(IntPtr _, UGCQueryHandle_t handle, uint index, int eStatType, IntPtr pStatValue) { return SteamEmulator.SteamUGC.GetQueryUGCStatistic(handle, index, eStatType, pStatValue); }

        public uint GetQueryUGCNumAdditionalPreviews__V021(IntPtr _, UGCQueryHandle_t handle, uint index) { return SteamEmulator.SteamUGC.GetQueryUGCNumAdditionalPreviews(handle, index); }

        public bool GetQueryUGCAdditionalPreview__V021(IntPtr _, UGCQueryHandle_t handle, uint index, uint previewIndex, IntPtr pchURLOrVideoID, uint cchURLSize, IntPtr pchOriginalFileName, uint cchOriginalFileNameSize, IntPtr pPreviewType) { return SteamEmulator.SteamUGC.GetQueryUGCAdditionalPreview(handle, index, previewIndex, pchURLOrVideoID, cchURLSize, pchOriginalFileName, cchOriginalFileNameSize, pPreviewType); }

        public uint GetQueryUGCNumKeyValueTags__V021(IntPtr _, UGCQueryHandle_t handle, uint index) { return SteamEmulator.SteamUGC.GetQueryUGCNumKeyValueTags(handle, index); }

        public bool GetQueryFirstUGCKeyValueTag__V021(IntPtr _, UGCQueryHandle_t handle, uint index, string pchKey, IntPtr pchValue, uint cchValueSize) { return SteamEmulator.SteamUGC.GetQueryUGCKeyValueTag(handle, index, pchKey, pchValue, cchValueSize); }

        public bool GetQueryUGCKeyValueTag__V021(IntPtr _, UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, IntPtr pchKey, uint cchKeySize, IntPtr pchValue, uint cchValueSize) { return SteamEmulator.SteamUGC.GetQueryUGCKeyValueTag(handle, index, keyValueTagIndex, pchKey, cchKeySize, pchValue, cchValueSize); }

        public uint GetNumSupportedGameVersions__V021(IntPtr _, UGCQueryHandle_t handle, uint index) { return SteamEmulator.SteamUGC.GetNumSupportedGameVersions(handle, index); }

        public bool GetSupportedGameVersionData__V021(IntPtr _, UGCQueryHandle_t handle, uint index, uint versionIndex, IntPtr pchGameBranchMin, IntPtr pchGameBranchMax, uint cchGameBranchSize) { return SteamEmulator.SteamUGC.GetSupportedGameVersionData(handle, index, versionIndex, pchGameBranchMin, pchGameBranchMax, cchGameBranchSize); }

        public uint GetQueryUGCContentDescriptors__V021(IntPtr _, UGCQueryHandle_t handle, uint index, IntPtr pvecDescriptors, uint cMaxEntries) { return SteamEmulator.SteamUGC.GetQueryUGCContentDescriptors(handle, index, pvecDescriptors, cMaxEntries); }

        public bool ReleaseQueryUGCRequest__V021(IntPtr _, UGCQueryHandle_t handle) { return SteamEmulator.SteamUGC.ReleaseQueryUGCRequest(handle); }

        public bool AddRequiredTag__V021(IntPtr _, UGCQueryHandle_t handle, string pTagName) { return SteamEmulator.SteamUGC.AddRequiredTag(handle, pTagName); }

        public bool AddRequiredTagGroup__V021(IntPtr _, UGCQueryHandle_t handle, IntPtr pTagGroups) { return SteamEmulator.SteamUGC.AddRequiredTagGroup(handle, pTagGroups); }

        public bool AddExcludedTag__V021(IntPtr _, UGCQueryHandle_t handle, string pTagName) { return SteamEmulator.SteamUGC.AddExcludedTag(handle, pTagName); }

        public bool SetReturnOnlyIDs__V021(IntPtr _, UGCQueryHandle_t handle, bool bReturnOnlyIDs) { return SteamEmulator.SteamUGC.SetReturnOnlyIDs(handle, bReturnOnlyIDs); }

        public bool SetReturnKeyValueTags__V021(IntPtr _, UGCQueryHandle_t handle, bool bReturnKeyValueTags) { return SteamEmulator.SteamUGC.SetReturnKeyValueTags(handle, bReturnKeyValueTags); }

        public bool SetReturnLongDescription__V021(IntPtr _, UGCQueryHandle_t handle, bool bReturnLongDescription) { return SteamEmulator.SteamUGC.SetReturnLongDescription(handle, bReturnLongDescription); }

        public bool SetReturnMetadata__V021(IntPtr _, UGCQueryHandle_t handle, bool bReturnMetadata) { return SteamEmulator.SteamUGC.SetReturnMetadata(handle, bReturnMetadata); }

        public bool SetReturnChildren__V021(IntPtr _, UGCQueryHandle_t handle, bool bReturnChildren) { return SteamEmulator.SteamUGC.SetReturnChildren(handle, bReturnChildren); }

        public bool SetReturnAdditionalPreviews__V021(IntPtr _, UGCQueryHandle_t handle, bool bReturnAdditionalPreviews) { return SteamEmulator.SteamUGC.SetReturnAdditionalPreviews(handle, bReturnAdditionalPreviews); }

        public bool SetReturnTotalOnly__V021(IntPtr _, UGCQueryHandle_t handle, bool bReturnTotalOnly) { return SteamEmulator.SteamUGC.SetReturnTotalOnly(handle, bReturnTotalOnly); }

        public bool SetReturnPlaytimeStats__V021(IntPtr _, UGCQueryHandle_t handle, uint unDays) { return SteamEmulator.SteamUGC.SetReturnPlaytimeStats(handle, unDays); }

        public bool SetLanguage__V021(IntPtr _, UGCQueryHandle_t handle, string pchLanguage) { return SteamEmulator.SteamUGC.SetLanguage(handle, pchLanguage); }

        public bool SetAllowCachedResponse__V021(IntPtr _, UGCQueryHandle_t handle, uint unMaxAgeSeconds) { return SteamEmulator.SteamUGC.SetAllowCachedResponse(handle, unMaxAgeSeconds); }

        public bool SetAdminQuery__V021(IntPtr _, UGCUpdateHandle_t handle, bool bAdminQuery) { return SteamEmulator.SteamUGC.SetAdminQuery(handle, bAdminQuery); }

        public bool SetCloudFileNameFilter__V021(IntPtr _, UGCQueryHandle_t handle, string pMatchCloudFileName) { return SteamEmulator.SteamUGC.SetCloudFileNameFilter(handle, pMatchCloudFileName); }

        public bool SetMatchAnyTag__V021(IntPtr _, UGCQueryHandle_t handle, bool bMatchAnyTag) { return SteamEmulator.SteamUGC.SetMatchAnyTag(handle, bMatchAnyTag); }

        public bool SetSearchText__V021(IntPtr _, UGCQueryHandle_t handle, string pSearchText) { return SteamEmulator.SteamUGC.SetSearchText(handle, pSearchText); }

        public bool SetRankedByTrendDays__V021(IntPtr _, UGCQueryHandle_t handle, uint unDays) { return SteamEmulator.SteamUGC.SetRankedByTrendDays(handle, unDays); }

        public bool SetTimeCreatedDateRange__V021(IntPtr _, UGCQueryHandle_t handle, uint rtStart, uint rtEnd) { return SteamEmulator.SteamUGC.SetTimeCreatedDateRange(handle, new IntPtr(rtStart), new IntPtr(rtEnd)); }

        public bool SetTimeUpdatedDateRange__V021(IntPtr _, UGCQueryHandle_t handle, uint rtStart, uint rtEnd) { return SteamEmulator.SteamUGC.SetTimeUpdatedDateRange(handle, new IntPtr(rtStart), new IntPtr(rtEnd)); }

        public bool AddRequiredKeyValueTag__V021(IntPtr _, UGCQueryHandle_t handle, string pKey, string pValue) { return SteamEmulator.SteamUGC.AddRequiredKeyValueTag(handle, pKey, pValue); }

        public SteamAPICall_t RequestUGCDetails__V021(IntPtr _, ulong nPublishedFileID, uint unMaxAgeSeconds) { return SteamEmulator.SteamUGC.RequestUGCDetails(nPublishedFileID, unMaxAgeSeconds); }

        public SteamAPICall_t CreateItem__V021(IntPtr _, uint nConsumerAppId, int eFileType) { return SteamEmulator.SteamUGC.CreateItem(nConsumerAppId, eFileType); }

        public UGCUpdateHandle_t StartItemUpdate__V021(IntPtr _, uint nConsumerAppId, ulong nPublishedFileID) { return SteamEmulator.SteamUGC.StartItemUpdate(nConsumerAppId, nPublishedFileID); }

        public bool SetItemTitle__V021(IntPtr _, UGCQueryHandle_t handle, string pchTitle) { return SteamEmulator.SteamUGC.SetItemTitle(handle, pchTitle); }

        public bool SetItemDescription__V021(IntPtr _, UGCQueryHandle_t handle, string pchDescription) { return SteamEmulator.SteamUGC.SetItemDescription(handle, pchDescription); }

        public bool SetItemUpdateLanguage__V021(IntPtr _, UGCQueryHandle_t handle, string pchLanguage) { return SteamEmulator.SteamUGC.SetItemUpdateLanguage(handle, pchLanguage); }

        public bool SetItemMetadata__V021(IntPtr _, UGCQueryHandle_t handle, string pchMetaData) { return SteamEmulator.SteamUGC.SetItemMetadata(handle, pchMetaData); }

        public bool SetItemVisibility__V021(IntPtr _, UGCQueryHandle_t handle, int eVisibility) { return SteamEmulator.SteamUGC.SetItemVisibility(handle, eVisibility); }

        public bool SetItemTags__V021(IntPtr _, ulong updateHandle, IntPtr pTags, bool bAllowAdminTags) { return SteamEmulator.SteamUGC.SetItemTags(updateHandle, pTags, bAllowAdminTags); }

        public bool SetItemContent__V021(IntPtr _, UGCQueryHandle_t handle, string pszContentFolder) { return SteamEmulator.SteamUGC.SetItemContent(handle, pszContentFolder); }

        public bool SetItemPreview__V021(IntPtr _, UGCQueryHandle_t handle, string pszPreviewFile) { return SteamEmulator.SteamUGC.SetItemPreview(handle, pszPreviewFile); }

        public bool SetAllowLegacyUpload__V021(IntPtr _, UGCQueryHandle_t handle, bool bAllowLegacyUpload) { return SteamEmulator.SteamUGC.SetAllowLegacyUpload(handle, bAllowLegacyUpload); }

        public bool RemoveAllItemKeyValueTags__V021(IntPtr _, UGCQueryHandle_t handle) { return SteamEmulator.SteamUGC.RemoveAllItemKeyValueTags(handle); }

        public bool RemoveItemKeyValueTags__V021(IntPtr _, UGCQueryHandle_t handle, string pchKey) { return SteamEmulator.SteamUGC.RemoveItemKeyValueTags(handle, pchKey); }

        public bool AddItemKeyValueTag__V021(IntPtr _, UGCQueryHandle_t handle, string pchKey, string pchValue) { return SteamEmulator.SteamUGC.AddItemKeyValueTag(handle, pchKey, pchValue); }

        public bool AddItemPreviewFile__V021(IntPtr _, UGCQueryHandle_t handle, string pszPreviewFile, int type) { return SteamEmulator.SteamUGC.AddItemPreviewFile(handle, pszPreviewFile, type); }

        public bool AddItemPreviewVideo__V021(IntPtr _, UGCQueryHandle_t handle, string pszVideoID) { return SteamEmulator.SteamUGC.AddItemPreviewVideo(handle, pszVideoID); }

        public bool UpdateItemPreviewFile__V021(IntPtr _, UGCQueryHandle_t handle, uint index, string pszPreviewFile) { return SteamEmulator.SteamUGC.UpdateItemPreviewFile(handle, index, pszPreviewFile); }

        public bool UpdateItemPreviewVideo__V021(IntPtr _, UGCQueryHandle_t handle, uint index, string pszVideoID) { return SteamEmulator.SteamUGC.UpdateItemPreviewVideo(handle, index, pszVideoID); }

        public bool RemoveItemPreview__V021(IntPtr _, UGCQueryHandle_t handle, uint index) { return SteamEmulator.SteamUGC.RemoveItemPreview(handle, index); }

        public bool AddContentDescriptor__V021(IntPtr _, UGCQueryHandle_t handle, int descid) { return SteamEmulator.SteamUGC.AddContentDescriptor(handle, descid); }

        public bool RemoveContentDescriptor__V021(IntPtr _, UGCQueryHandle_t handle, int descid) { return SteamEmulator.SteamUGC.RemoveContentDescriptor(handle, descid); }

        public bool SetRequiredGameVersions__V021(IntPtr _, UGCQueryHandle_t handle, string pszGameBranchMin, string pszGameBranchMax) { return SteamEmulator.SteamUGC.SetRequiredGameVersions(handle, pszGameBranchMin, pszGameBranchMax); }

        public SteamAPICall_t SubmitItemUpdate__V021(IntPtr _, UGCQueryHandle_t handle, string pchChangeNote) { return SteamEmulator.SteamUGC.SubmitItemUpdate(handle, pchChangeNote); }

        public int GetItemUpdateProgress__V021(IntPtr _, UGCUpdateHandle_t handle, IntPtr punBytesProcessed, IntPtr punBytesTotal)
        {
            WriteUInt64(punBytesProcessed, 0);
            WriteUInt64(punBytesTotal, 0);
            return SteamEmulator.SteamUGC.GetItemUpdateProgress(handle, 0, 0);
        }

        public SteamAPICall_t SetUserItemVote__V021(IntPtr _, ulong nPublishedFileID, bool bVoteUp) { return SteamEmulator.SteamUGC.SetUserItemVote(nPublishedFileID, bVoteUp); }

        public SteamAPICall_t GetUserItemVote__V021(IntPtr _, ulong nPublishedFileID) { return SteamEmulator.SteamUGC.GetUserItemVote(nPublishedFileID); }

        public SteamAPICall_t AddItemToFavorites__V021(IntPtr _, uint nAppId, ulong nPublishedFileID) { return SteamEmulator.SteamUGC.AddItemToFavorites(nAppId, nPublishedFileID); }

        public SteamAPICall_t RemoveItemFromFavorites__V021(IntPtr _, uint nAppId, ulong nPublishedFileID) { return SteamEmulator.SteamUGC.RemoveItemFromFavorites(nAppId, nPublishedFileID); }

        public SteamAPICall_t SubscribeItem__V021(IntPtr _, PublishedFileId_t nPublishedFileID) { return SteamEmulator.SteamUGC.SubscribeItem(nPublishedFileID); }

        public SteamAPICall_t UnsubscribeItem__V021(IntPtr _, PublishedFileId_t nPublishedFileID) { return SteamEmulator.SteamUGC.UnsubscribeItem(nPublishedFileID); }

        public uint GetNumSubscribedItems__V021(IntPtr _, bool bIncludeLocallyDisabled) { return SteamEmulator.SteamUGC.GetNumSubscribedItems(bIncludeLocallyDisabled); }

        public uint GetSubscribedItems__V021(IntPtr _, IntPtr pvecPublishedFileID, uint cMaxEntries, bool bIncludeLocallyDisabled) { return SteamEmulator.SteamUGC.GetSubscribedItems(pvecPublishedFileID, cMaxEntries, bIncludeLocallyDisabled); }

        public bool GetItemDownloadInfo__V021(IntPtr _, ulong nPublishedFileID, IntPtr punBytesDownloaded, IntPtr punBytesTotal)
        {
            WriteUInt64(punBytesDownloaded, 0);
            WriteUInt64(punBytesTotal, 0);
            return SteamEmulator.SteamUGC.GetItemDownloadInfo(nPublishedFileID, 0, 0);
        }

        public bool DownloadItem__V021(IntPtr _, ulong nPublishedFileID, bool bHighPriority) { return SteamEmulator.SteamUGC.DownloadItem(nPublishedFileID, bHighPriority); }

        public bool BInitWorkshopForGameServer__V021(IntPtr _, uint unWorkshopDepotID, string pszFolder) { return SteamEmulator.SteamUGC.BInitWorkshopForGameServer(unWorkshopDepotID, pszFolder); }

        public void SuspendDownloads__V021(IntPtr _, bool bSuspend) { SteamEmulator.SteamUGC.SuspendDownloads(bSuspend); }

        public SteamAPICall_t StartPlaytimeTracking__V021(IntPtr _, IntPtr pvecPublishedFileID, uint unNumPublishedFileIDs) { return SteamEmulator.SteamUGC.StartPlaytimeTracking(0, unNumPublishedFileIDs); }

        public SteamAPICall_t StopPlaytimeTracking__V021(IntPtr _, IntPtr pvecPublishedFileID, uint unNumPublishedFileIDs) { return SteamEmulator.SteamUGC.StopPlaytimeTracking(0, unNumPublishedFileIDs); }

        public SteamAPICall_t StopPlaytimeTrackingForAllItems__V021(IntPtr _) { return SteamEmulator.SteamUGC.StopPlaytimeTrackingForAllItems(); }

        public SteamAPICall_t AddDependency__V021(IntPtr _, ulong nParentPublishedFileID, ulong nChildPublishedFileID) { return SteamEmulator.SteamUGC.AddDependency(nParentPublishedFileID, nChildPublishedFileID); }

        public SteamAPICall_t RemoveDependency__V021(IntPtr _, ulong nParentPublishedFileID, ulong nChildPublishedFileID) { return SteamEmulator.SteamUGC.RemoveDependency(nParentPublishedFileID, nChildPublishedFileID); }

        public SteamAPICall_t AddAppDependency__V021(IntPtr _, ulong nPublishedFileID, uint nAppID) { return SteamEmulator.SteamUGC.AddAppDependency(nPublishedFileID, nAppID); }

        public SteamAPICall_t RemoveAppDependency__V021(IntPtr _, ulong nPublishedFileID, uint nAppID) { return SteamEmulator.SteamUGC.RemoveAppDependency(nPublishedFileID, nAppID); }

        public SteamAPICall_t GetAppDependencies__V021(IntPtr _, ulong nPublishedFileID) { return SteamEmulator.SteamUGC.GetAppDependencies(nPublishedFileID); }

        public SteamAPICall_t DeleteItem__V021(IntPtr _, ulong nPublishedFileID) { return SteamEmulator.SteamUGC.DeleteItem(nPublishedFileID); }

        public bool ShowWorkshopEULA__V021(IntPtr _) { return SteamEmulator.SteamUGC.ShowWorkshopEULA(); }

        public SteamAPICall_t GetWorkshopEULAStatus__V021(IntPtr _) { return SteamEmulator.SteamUGC.GetWorkshopEULAStatus(); }

        public uint GetUserContentDescriptorPreferences__V021(IntPtr _, IntPtr pvecDescriptors, uint cMaxEntries) { return SteamEmulator.SteamUGC.GetUserContentDescriptorPreferences(pvecDescriptors, cMaxEntries); }

        public bool SetItemsDisabledLocally(IntPtr _, IntPtr pvecPublishedFileIDs, uint unNumPublishedFileIDs, bool bDisabledLocally) { return SteamEmulator.SteamUGC.SetItemsDisabledLocally(0, unNumPublishedFileIDs, bDisabledLocally); }

        public bool SetSubscriptionsLoadOrder(IntPtr _, IntPtr pvecPublishedFileIDs, uint unNumPublishedFileIDs) { return SteamEmulator.SteamUGC.SetSubscriptionsLoadOrder(0, unNumPublishedFileIDs); }

        public bool MarkDownloadedItemAsUnused(IntPtr _, ulong nPublishedFileID) { return false; }

        public uint GetNumDownloadedItems(IntPtr _) { return 0; }

        public uint GetDownloadedItems(IntPtr _, IntPtr pvecPublishedFileID, uint cMaxEntries) { return 0; }

        public ulong CreateQueryUserUGCRequest(IntPtr _, uint arg0, int arg1, int arg2, int arg3, uint arg4, uint arg5, uint arg6) { return 0; }

        public ulong CreateQueryAllUGCRequestCursor(IntPtr _, int arg0, int arg1, uint arg2, uint arg3, IntPtr arg4) { return 0; }

        public ulong CreateQueryAllUGCRequestPage(IntPtr _, int arg0, int arg1, uint arg2, uint arg3, uint arg4) { return 0; }

        public ulong CreateQueryUGCDetailsRequest(IntPtr _, IntPtr arg0, uint arg1) { return SteamEmulator.SteamUGC.CreateQueryUGCDetailsRequest(arg0, arg1); }

        public ulong SendQueryUGCRequest(IntPtr _, ulong arg0) { return 0; }

        public bool GetQueryUGCResult(IntPtr _, ulong arg0, uint arg1, IntPtr arg2) { return false; }

        public uint GetQueryUGCNumTags(IntPtr _, ulong arg0, uint arg1) { return 0; }

        public bool GetQueryUGCTag(IntPtr _, ulong arg0, uint arg1, uint arg2, IntPtr arg3, uint arg4) { return false; }

        public bool GetQueryUGCTagDisplayName(IntPtr _, ulong arg0, uint arg1, uint arg2, IntPtr arg3, uint arg4) { return false; }

        public bool GetQueryUGCPreviewURL(IntPtr _, ulong arg0, uint arg1, IntPtr arg2, uint arg3) { return false; }

        public bool GetQueryUGCMetadata(IntPtr _, ulong arg0, uint arg1, IntPtr arg2, uint arg3) { return false; }

        public bool GetQueryUGCChildren(IntPtr _, ulong arg0, uint arg1, IntPtr arg2, uint arg3) { return false; }

        public bool GetQueryUGCStatistic(IntPtr _, ulong arg0, uint arg1, int arg2, IntPtr arg3) { return false; }

        public uint GetQueryUGCNumAdditionalPreviews(IntPtr _, ulong arg0, uint arg1) { return 0; }

        public bool GetQueryUGCAdditionalPreview(IntPtr _, ulong arg0, uint arg1, uint arg2, IntPtr arg3, uint arg4, IntPtr arg5, uint arg6, IntPtr arg7) { return false; }

        public uint GetQueryUGCNumKeyValueTags(IntPtr _, ulong arg0, uint arg1) { return 0; }

        public bool GetQueryFirstUGCKeyValueTag(IntPtr _, ulong arg0, uint arg1, IntPtr arg2, IntPtr arg3, uint arg4) { return false; }

        public bool GetQueryUGCKeyValueTag(IntPtr _, ulong arg0, uint arg1, uint arg2, IntPtr arg3, uint arg4, IntPtr arg5, uint arg6) { return false; }

        public uint GetNumSupportedGameVersions(IntPtr _, ulong arg0, uint arg1) { return 0; }

        public bool GetSupportedGameVersionData(IntPtr _, ulong arg0, uint arg1, uint arg2, IntPtr arg3, IntPtr arg4, uint arg5) { return false; }

        public uint GetQueryUGCContentDescriptors(IntPtr _, ulong arg0, uint arg1, IntPtr arg2, uint arg3) { return 0; }

        public bool ReleaseQueryUGCRequest(IntPtr _, ulong arg0) { return false; }

        public bool AddRequiredTag(IntPtr _, ulong arg0, IntPtr arg1) { return false; }

        public bool AddRequiredTagGroup(IntPtr _, ulong arg0, IntPtr arg1) { return false; }

        public bool AddExcludedTag(IntPtr _, ulong arg0, IntPtr arg1) { return false; }

        public bool SetReturnOnlyIDs(IntPtr _, ulong arg0, bool arg1) { return false; }

        public bool SetReturnKeyValueTags(IntPtr _, ulong arg0, bool arg1) { return false; }

        public bool SetReturnLongDescription(IntPtr _, ulong arg0, bool arg1) { return false; }

        public bool SetReturnMetadata(IntPtr _, ulong arg0, bool arg1) { return false; }

        public bool SetReturnChildren(IntPtr _, ulong arg0, bool arg1) { return false; }

        public bool SetReturnAdditionalPreviews(IntPtr _, ulong arg0, bool arg1) { return false; }

        public bool SetReturnTotalOnly(IntPtr _, ulong arg0, bool arg1) { return false; }

        public bool SetReturnPlaytimeStats(IntPtr _, ulong arg0, uint arg1) { return false; }

        public bool SetLanguage(IntPtr _, ulong arg0, IntPtr arg1) { return false; }

        public bool SetAllowCachedResponse(IntPtr _, ulong arg0, uint arg1) { return false; }

        public bool SetAdminQuery(IntPtr _, ulong arg0, bool arg1) { return false; }

        public bool SetCloudFileNameFilter(IntPtr _, ulong arg0, IntPtr arg1) { return false; }

        public bool SetMatchAnyTag(IntPtr _, ulong arg0, bool arg1) { return false; }

        public bool SetSearchText(IntPtr _, ulong arg0, IntPtr arg1) { return false; }

        public bool SetRankedByTrendDays(IntPtr _, ulong arg0, uint arg1) { return false; }

        public bool SetTimeCreatedDateRange(IntPtr _, ulong arg0, uint arg1, uint arg2) { return false; }

        public bool SetTimeUpdatedDateRange(IntPtr _, ulong arg0, uint arg1, uint arg2) { return false; }

        public bool AddRequiredKeyValueTag(IntPtr _, ulong arg0, IntPtr arg1, IntPtr arg2) { return false; }

        public ulong RequestUGCDetails(IntPtr _, ulong arg0, uint arg1) { return SteamEmulator.SteamUGC.RequestUGCDetails(arg0, arg1); }

        public ulong CreateItem(IntPtr _, uint arg0, int arg1) { return 0; }

        public ulong StartItemUpdate(IntPtr _, uint arg0, ulong arg1) { return 0; }

        public bool SetItemTitle(IntPtr _, ulong arg0, IntPtr arg1) { return false; }

        public bool SetItemDescription(IntPtr _, ulong arg0, IntPtr arg1) { return false; }

        public bool SetItemUpdateLanguage(IntPtr _, ulong arg0, IntPtr arg1) { return false; }

        public bool SetItemMetadata(IntPtr _, ulong arg0, IntPtr arg1) { return false; }

        public bool SetItemVisibility(IntPtr _, ulong arg0, int arg1) { return false; }

        public bool SetItemTags__V019(IntPtr _, ulong arg0, IntPtr arg1, bool arg2) { return false; }

        public bool SetItemContent(IntPtr _, ulong arg0, IntPtr arg1) { return false; }

        public bool SetItemPreview(IntPtr _, ulong arg0, IntPtr arg1) { return false; }

        public bool SetAllowLegacyUpload(IntPtr _, ulong arg0, bool arg1) { return false; }

        public bool RemoveAllItemKeyValueTags(IntPtr _, ulong arg0) { return false; }

        public bool RemoveItemKeyValueTags(IntPtr _, ulong arg0, IntPtr arg1) { return false; }

        public bool AddItemKeyValueTag(IntPtr _, ulong arg0, IntPtr arg1, IntPtr arg2) { return false; }

        public bool AddItemPreviewFile(IntPtr _, ulong arg0, IntPtr arg1, int arg2) { return false; }

        public bool AddItemPreviewVideo(IntPtr _, ulong arg0, IntPtr arg1) { return false; }

        public bool UpdateItemPreviewFile(IntPtr _, ulong arg0, uint arg1, IntPtr arg2) { return false; }

        public bool UpdateItemPreviewVideo(IntPtr _, ulong arg0, uint arg1, IntPtr arg2) { return false; }

        public bool RemoveItemPreview(IntPtr _, ulong arg0, uint arg1) { return false; }

        public bool AddContentDescriptor(IntPtr _, ulong arg0, int arg1) { return false; }

        public bool RemoveContentDescriptor(IntPtr _, ulong arg0, int arg1) { return false; }

        public bool SetRequiredGameVersions(IntPtr _, ulong arg0, IntPtr arg1, IntPtr arg2) { return false; }

        public ulong SubmitItemUpdate(IntPtr _, ulong arg0, IntPtr arg1) { return 0; }

        public int GetItemUpdateProgress(IntPtr _, ulong arg0, IntPtr arg1, IntPtr arg2) { return 0; }

        public ulong SetUserItemVote(IntPtr _, ulong arg0, bool arg1) { return 0; }

        public ulong GetUserItemVote(IntPtr _, ulong arg0) { return 0; }

        public ulong AddItemToFavorites(IntPtr _, uint arg0, ulong arg1) { return 0; }

        public ulong RemoveItemFromFavorites(IntPtr _, uint arg0, ulong arg1) { return 0; }

        public ulong SubscribeItem(IntPtr _, ulong arg0) { return 0; }

        public ulong UnsubscribeItem(IntPtr _, ulong arg0) { return 0; }

        public uint GetNumSubscribedItems(IntPtr _) { return SteamEmulator.SteamUGC.GetNumSubscribedItems(); }

        public uint GetSubscribedItems(IntPtr _, IntPtr arg0, uint arg1) { return SteamEmulator.SteamUGC.GetSubscribedItems(arg0, arg1); }

        public uint GetItemState(IntPtr _, ulong arg0) { return SteamEmulator.SteamUGC.GetItemState(arg0); }

        public bool GetItemInstallInfo(IntPtr _, ulong arg0, IntPtr arg1, IntPtr arg2, uint arg3, IntPtr arg4) { return SteamEmulator.SteamUGC.GetItemInstallInfo(arg0, arg1, arg2, arg3, arg4); }

        public bool GetItemDownloadInfo(IntPtr _, ulong arg0, IntPtr arg1, IntPtr arg2) { return false; }

        public bool DownloadItem(IntPtr _, ulong arg0, bool arg1) { return false; }

        public bool BInitWorkshopForGameServer(IntPtr _, uint arg0, IntPtr arg1) { return false; }

        public void SuspendDownloads(IntPtr _, bool arg0) { }

        public ulong StartPlaytimeTracking(IntPtr _, IntPtr arg0, uint arg1) { return 0; }

        public ulong StopPlaytimeTracking(IntPtr _, IntPtr arg0, uint arg1) { return 0; }

        public ulong StopPlaytimeTrackingForAllItems(IntPtr _) { return 0; }

        public ulong AddDependency(IntPtr _, ulong arg0, ulong arg1) { return 0; }

        public ulong RemoveDependency(IntPtr _, ulong arg0, ulong arg1) { return 0; }

        public ulong AddAppDependency(IntPtr _, ulong arg0, uint arg1) { return 0; }

        public ulong RemoveAppDependency(IntPtr _, ulong arg0, uint arg1) { return 0; }

        public ulong GetAppDependencies(IntPtr _, ulong arg0) { return 0; }

        public ulong DeleteItem(IntPtr _, ulong arg0) { return 0; }

        public bool ShowWorkshopEULA(IntPtr _) { return false; }

        public ulong GetWorkshopEULAStatus(IntPtr _) { return 0; }

        public uint GetUserContentDescriptorPreferences(IntPtr _, IntPtr arg0, uint arg1) { return 0; }

        public ulong RequestUGCDetails_old(IntPtr _, ulong arg0, uint arg1) { return SteamEmulator.SteamUGC.RequestUGCDetails(arg0, arg1); }

        public bool GetQueryUGCResult_old(IntPtr _, ulong arg0, uint arg1, IntPtr arg2) { return false; }

        public bool SetItemTags(IntPtr _, ulong arg0, IntPtr arg1) { return false; }

        public ulong CreateQueryAllUGCRequest__V012(IntPtr _, int arg0, int arg1, uint arg2, uint arg3, IntPtr arg4) { return 0; }

        public ulong CreateQueryAllUGCRequest(IntPtr _, int arg0, int arg1, uint arg2, uint arg3, uint arg4) { return 0; }

        private static void WriteUInt64(IntPtr destination, ulong value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteInt64(destination, unchecked((long)value));
            }
        }

        private static bool TryReadPageFromSmallPointer(IntPtr pointer, out uint page)
        {
            long value = pointer.ToInt64();
            if (value > 0 && value < 0x10000)
            {
                page = (uint)value;
                return true;
            }

            page = 0;
            return false;
        }

        private static string ReadNativeString(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
            {
                return string.Empty;
            }

            try
            {
                return Marshal.PtrToStringAnsi(pointer) ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
