using System;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("STEAMUGC_INTERFACE_VERSION020")]
    public class SteamUGC020Generated : ISteamInterface
    {
        public ulong CreateQueryUserUGCRequest(IntPtr _, uint arg0, int arg1, int arg2, int arg3, uint arg4, uint arg5, uint arg6) { return 0; }
        public ulong CreateQueryAllUGCRequestPage(IntPtr _, int arg0, int arg1, uint arg2, uint arg3, uint arg4) { return 0; }
        public ulong CreateQueryAllUGCRequestCursor(IntPtr _, int arg0, int arg1, uint arg2, uint arg3, IntPtr arg4) { return 0; }
        public ulong CreateQueryUGCDetailsRequest(IntPtr _, IntPtr arg0, uint arg1) { return 0; }
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
        public bool GetQueryUGCKeyValueTag(IntPtr _, ulong arg0, uint arg1, uint arg2, IntPtr arg3, uint arg4, IntPtr arg5, uint arg6) { return false; }
        public bool GetQueryFirstUGCKeyValueTag(IntPtr _, ulong arg0, uint arg1, IntPtr arg2, IntPtr arg3, uint arg4) { return false; }
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
        public ulong RequestUGCDetails(IntPtr _, ulong arg0, uint arg1) { return 0; }
        public ulong CreateItem(IntPtr _, uint arg0, int arg1) { return 0; }
        public ulong StartItemUpdate(IntPtr _, uint arg0, ulong arg1) { return 0; }
        public bool SetItemTitle(IntPtr _, ulong arg0, IntPtr arg1) { return false; }
        public bool SetItemDescription(IntPtr _, ulong arg0, IntPtr arg1) { return false; }
        public bool SetItemUpdateLanguage(IntPtr _, ulong arg0, IntPtr arg1) { return false; }
        public bool SetItemMetadata(IntPtr _, ulong arg0, IntPtr arg1) { return false; }
        public bool SetItemVisibility(IntPtr _, ulong arg0, int arg1) { return false; }
        public bool SetItemTags(IntPtr _, ulong arg0, IntPtr arg1, bool arg2) { return false; }
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
        public uint GetNumSubscribedItems(IntPtr _) { return 0; }
        public uint GetSubscribedItems(IntPtr _, IntPtr arg0, uint arg1) { return 0; }
        public uint GetItemState(IntPtr _, ulong arg0) { return 0; }
        public bool GetItemInstallInfo(IntPtr _, ulong arg0, IntPtr arg1, IntPtr arg2, uint arg3, IntPtr arg4) { return false; }
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
    }
}
