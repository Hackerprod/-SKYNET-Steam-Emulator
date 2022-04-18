using SKYNET;
using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    //return new SteamAPICall_t(CallbackType.k_iRemoteStorageFileReadAsyncComplete);
    public class SteamUGC : ISteamInterface
    {
        private List<UGC> UGCQueries;
        UGCQueryHandle_t Handle;
        List<PublishedFileId_t> subscribed;

        internal class UGC
        {
            public UGCQueryHandle_t Handle;
            public List<PublishedFileId_t> return_only;
            public bool ReturnAll;
            public bool ReturnOnly;
            public List<PublishedFileId_t> results;

            public UGC()
            {
                return_only = new List<PublishedFileId_t>();
                results = new List<PublishedFileId_t>();
            }
        }

        public SteamUGC()
        {
            InterfaceVersion = "SteamUGC";
            UGCQueries = new List<UGC>();
            subscribed = new List<PublishedFileId_t>();
        }

        public UGCQueryHandle_t CreateQueryUserUGCRequest(uint unAccountID, int eListType, int eMatchingUGCType, int eSortOrder, uint nCreatorAppID, uint nConsumerAppID, uint unPage)
        {
            Write($"CreateQueryUserUGCRequest for {unAccountID}");
            return CreateOne((eListType == (int)EUserUGCList.k_EUserUGCList_Subscribed || eListType == (int)EUserUGCList.k_EUserUGCList_Published));
        }

        public UGCQueryHandle_t CreateQueryAllUGCRequest(int eQueryType, int eMatchingeMatchingUGCTypeFileType, uint nCreatorAppID, uint nConsumerAppID, uint unPage)
        {
            Write("CreateQueryAllUGCRequest");
            return CreateOne();
        }

        public UGCQueryHandle_t CreateQueryAllUGCRequest(int eQueryType, int eMatchingeMatchingUGCTypeFileType, uint nCreatorAppID, uint nConsumerAppID, string pchCursor)
        {
            Write("CreateQueryAllUGCRequest");
            return CreateOne();
        }

        public UGCQueryHandle_t CreateQueryUGCDetailsRequest(ulong pvecPublishedFileID, uint unNumPublishedFileIDs)
        {
            Write("CreateQueryUGCDetailsRequest");
            return CreateOne();
        }

        public SteamAPICall_t SendQueryUGCRequest(UGCQueryHandle_t handle)
        {
            try
            {
                Write("SendQueryUGCRequest");

                var request = UGCQueries.Find(u => u.Handle == handle);
                if (request == null) return 0;

                if (request.ReturnAll)
                {
                    request.results = subscribed;
                }

                if (request.return_only.Any())
                {
                    foreach (var item in request.return_only)
                    {
                        request.results.Add(item);
                    }
                }

                SteamUGCQueryCompleted_t data = new SteamUGCQueryCompleted_t()
                {
                    m_handle = handle,
                    m_eResult = EResult.k_EResultOK,
                    m_unNumResultsReturned = (uint)request.results.Count(),
                    m_unTotalMatchingResults = (uint)request.results.Count(),
                    m_bCachedData = false,
                };
                //return new SteamAPICall_t(CallbackType.k_iSteamUGCQueryCompleted);
                return CallbackManager.AddCallbackResult(data);
            }
            catch (Exception ex)
            {
                Write($"SendQueryUGCRequest {ex}");
            }
            return 0;
        }

        public bool GetQueryUGCResult(UGCQueryHandle_t handle, uint index, IntPtr pDetails)
        {
            Write("GetQueryUGCResult");
            var UGC = UGCQueries.Find(u => u.Handle == handle);
            if (UGC == null) return false;
            if (index >= UGC.results.Count) return false;
            foreach (var item in UGC.results)
            {
                set_details(item, pDetails);
            }
            return true;
        }

        public bool GetQueryUGCPreviewURL(UGCQueryHandle_t handle, uint index, string pchURL, uint cchURLSize)
        {
            Write("GetQueryUGCPreviewURL");
            return false;
        }

        public bool GetQueryUGCMetadata(UGCQueryHandle_t handle, uint index, string pchMetadata, uint cchMetadatasize)
        {
            Write("GetQueryUGCMetadata");
            return false;
        }

        internal uint GetQueryUGCNumTags(UGCQueryHandle_t handle, uint index)
        {
            Write("GetQueryUGCNumTags");
            return 0;
        }

        public bool GetQueryUGCChildren(UGCQueryHandle_t handle, uint index, ulong pvecPublishedFileID, uint cMaxEntries)
        {
            Write("GetQueryUGCChildren");
            return false;
        }

        internal bool GetQueryUGCTag(UGCQueryHandle_t handle, uint index, uint indexTag, string pchValue, uint cchValueSize)
        {
            Write("GetQueryUGCTag");
            return false;
        }

        internal bool GetQueryUGCTagDisplayName(UGCQueryHandle_t handle, uint index, uint indexTag, string pchValue, uint cchValueSize)
        {
            Write("GetQueryUGCTagDisplayName");
            return false;
        }

        public bool GetQueryUGCStatistic(UGCQueryHandle_t handle, uint index, int eStatType, ulong pStatValue)
        {
            Write("GetQueryUGCStatistic");
            return false;
        }

        public uint GetQueryUGCNumAdditionalPreviews(UGCQueryHandle_t handle, uint index)
        {
            Write("GetQueryUGCNumAdditionalPreviews");
            return 0;
        }

        public bool GetQueryUGCAdditionalPreview(UGCQueryHandle_t handle, uint index, uint previewIndex, string pchURLOrVideoID, uint cchURLSize, string pchOriginalFileName, uint cchOriginalFileNameSize, int pPreviewType)
        {
            Write("GetQueryUGCAdditionalPreview");
            return false;
        }

        public uint GetQueryUGCNumKeyValueTags(UGCQueryHandle_t handle, uint index)
        {
            Write("GetQueryUGCNumKeyValueTags");
            return 0;
        }

        public bool GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, string pchKey, uint cchKeySize, string pchValue, uint cchValueSize)
        {
            Write("GetQueryUGCKeyValueTag");
            return false;
        }

        public bool GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, string pchKey, string pchValue, uint cchValueSize)
        {
            Write("GetQueryUGCKeyValueTag");
            return false;
        }

        public bool ReleaseQueryUGCRequest(UGCQueryHandle_t handle)
        {
            Write("ReleaseQueryUGCRequest");
            var UGC = UGCQueries.Find(u => u.Handle == handle);
            if (UGC == null) return false;
            UGCQueries.Remove(UGC);
            return true;
        }

        public bool AddRequiredTag(UGCQueryHandle_t handle, string pTagName)
        {
            Write("AddRequiredTag");
            return false;
        }

        public bool AddRequiredTagGroup(UGCQueryHandle_t handle, IntPtr pTagGroups) // match any of the tags in this group 
        {
            Write("AddRequiredTagGroup");
            return false;
        }

        public bool AddExcludedTag(UGCQueryHandle_t handle, string pTagName)
        {
            Write("AddExcludedTag");
            return false;
        }

        public bool SetReturnOnlyIDs(UGCQueryHandle_t handle, bool bReturnOnlyIDs)
        {
            Write("SetReturnOnlyIDs");
            return true;
        }

        public bool SetReturnKeyValueTags(UGCQueryHandle_t handle, bool bReturnKeyValueTags)
        {
            Write("SetReturnKeyValueTags");
            return true;
        }

        public bool SetReturnLongDescription(UGCQueryHandle_t handle, bool bReturnLongDescription)
        {
            Write("SetReturnLongDescription");
            return true;
        }

        public bool SetReturnMetadata(UGCQueryHandle_t handle, bool bReturnMetadata)
        {
            Write("SetReturnMetadata");
            return true;
        }

        public bool SetReturnChildren(UGCQueryHandle_t handle, bool bReturnChildren)
        {
            Write("SetReturnChildren");
            return true;
        }

        public bool SetReturnAdditionalPreviews(UGCQueryHandle_t handle, bool bReturnAdditionalPreviews)
        {
            Write("SetReturnAdditionalPreviews");
            return true;
        }

        public bool SetReturnTotalOnly(UGCQueryHandle_t handle, bool bReturnTotalOnly)
        {
            Write("SetReturnTotalOnly");
            return true;
        }

        public bool SetReturnPlaytimeStats(UGCQueryHandle_t handle, uint unDays)
        {
            Write("SetReturnPlaytimeStats");
            return true;
        }

        public bool SetLanguage(UGCQueryHandle_t handle, string pchLanguage)
        {
            Write("SetLanguage");
            return true;
        }

        public bool SetAllowCachedResponse(UGCQueryHandle_t handle, uint unMaxAgeSeconds)
        {
            Write("SetAllowCachedResponse");
            return true;
        }

        public bool SetCloudFileNameFilter(UGCQueryHandle_t handle, string pMatchCloudFileName)
        {
            Write("SetCloudFileNameFilter");
            return true;
        }

        public bool SetMatchAnyTag(UGCQueryHandle_t handle, bool bMatchAnyTag)
        {
            Write("SetMatchAnyTag");
            return true;
        }

        public bool SetSearchText(UGCQueryHandle_t handle, string pSearchText)
        {
            Write("SetSearchText");
            return true;
        }

        public bool SetRankedByTrendDays(UGCQueryHandle_t handle, uint unDays)
        {
            Write("SetRankedByTrendDays");
            return true;
        }

        public bool AddRequiredKeyValueTag(UGCQueryHandle_t handle, string pKey, string pValue)
        {
            Write("AddRequiredKeyValueTag");
            return true;
        }

        public SteamAPICall_t RequestUGCDetails(ulong nPublishedFileID, uint unMaxAgeSeconds)
        {
            Write("RequestUGCDetails");
            // SteamUGCRequestUGCDetailsResult_t
            return 0;
        }

        public SteamAPICall_t CreateItem(uint nConsumerAppId, int eFileType)
        {
            Write("CreateItem");
            // CreateItemResult_t
            return 0;
        }

        public UGCUpdateHandle_t StartItemUpdate(uint nConsumerAppId, ulong nPublishedFileID) 
        {
            Write("StartItemUpdate");
            return (UGCUpdateHandle_t)0;
        }

        public bool SetItemTitle(UGCQueryHandle_t handle, string pchTitle) // change the title of an UGC item 
        {
            Write("SetItemTitle");
            return false;
        }

        public bool SetItemDescription(UGCQueryHandle_t handle, string pchDescription) // change the description of an UGC item 
        {
            Write("SetItemDescription");
            return false;
        }

        public bool SetItemUpdateLanguage(UGCQueryHandle_t handle, string pchLanguage) // specify the language of the title or description that will be set 
        {
            Write("SetItemUpdateLanguage");
            return false;
        }

        public bool SetItemMetadata(UGCQueryHandle_t handle, string pchMetaData)
        {
            Write("SetItemMetadata");
            return false;
        }

        public bool SetTimeCreatedDateRange(UGCQueryHandle_t handle, IntPtr rtStart, IntPtr rtEnd)
        {
            Write("SetTimeCreatedDateRange");
            return false;
        }

        public bool SetTimeUpdatedDateRange(UGCQueryHandle_t handle, IntPtr rtStart, IntPtr rtEnd)
        {
            Write("SetTimeUpdatedDateRange");
            return false;
        }

        public bool SetItemVisibility(UGCQueryHandle_t handle, int eVisibility) 
        {
            Write("SetItemVisibility");
            return false;
        }

        public bool SetItemTags(ulong updateHandle, IntPtr pTags)
        {
            Write("SetItemTags");
            return false;
        }

        public bool SetItemContent(UGCQueryHandle_t handle, string pszContentFolder) // update item content from this local folder 
        {
            Write("SetItemContent");
            return false;
        }

        public bool SetItemPreview(UGCQueryHandle_t handle, string pszPreviewFile) //  change preview image file for this item. pszPreviewFile points to local image file, which must be under 1MB in size 
        {
            Write("SetItemPreview");
            return false;
        }

        public bool SetAllowLegacyUpload(UGCQueryHandle_t handle, bool bAllowLegacyUpload) //  use legacy upload for a single small file. The parameter to SetItemContent() should either be a directory with one file or the full path to the file.  The file must also be less than 10MB in size. 
        {
            Write("SetAllowLegacyUpload");
            return false;
        }

        public bool RemoveAllItemKeyValueTags(UGCQueryHandle_t handle) // remove all existing key-value tags (you can add new ones via the AddItemKeyValueTag function) 
        {
            Write("RemoveAllItemKeyValueTags");
            return false;
        }

        public bool RemoveItemKeyValueTags(UGCQueryHandle_t handle, string pchKey) // remove any existing key-value tags with the specified key 
        {
            Write("RemoveItemKeyValueTags");
            return false;
        }

        public bool AddItemKeyValueTag(UGCQueryHandle_t handle, string pchKey, string pchValue) // add new key-value tags for the item. Note that there can be multiple values for a tag. 
        {
            Write("AddItemKeyValueTag");
            return false;
        }

        public bool AddItemPreviewFile(UGCQueryHandle_t handle, string pszPreviewFile, int type) //  add preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size 
        {
            Write("AddItemPreviewFile");
            return false;
        }

        public bool AddItemPreviewVideo(UGCQueryHandle_t handle, string pszVideoID) //  add preview video for this item 
        {
            Write("AddItemPreviewVideo");
            return false;
        }

        public bool UpdateItemPreviewFile(UGCQueryHandle_t handle, uint index, string pszPreviewFile) //  updates an existing preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size 
        {
            Write("UpdateItemPreviewFile");
            return false;
        }

        public bool UpdateItemPreviewVideo(UGCQueryHandle_t handle, uint index, string pszVideoID) //  updates an existing preview video for this item 
        {
            Write("UpdateItemPreviewVideo");
            return false;
        }

        public bool RemoveItemPreview(UGCQueryHandle_t handle, uint index) // remove a preview by index starting at 0 (previews are sorted) 
        {
            Write("RemoveItemPreview");
            return false;
        }

        public SteamAPICall_t SubmitItemUpdate(UGCQueryHandle_t handle, string pchChangeNote) 
        {
            Write("SubmitItemUpdate");
            // SubmitItemUpdateResult_t
            return 0;
        }

        public int GetItemUpdateProgress(UGCQueryHandle_t handle, ulong punBytesProcessed, ulong punBytesTotal)
        {
            Write("GetItemUpdateProgress");
            return 0;
        }

        public SteamAPICall_t SetUserItemVote(ulong nPublishedFileID, bool bVoteUp)
        {
            Write("SetUserItemVote");
            // SetUserItemVoteResult_t
            return 0;
        }

        public SteamAPICall_t GetUserItemVote(ulong nPublishedFileID)
        {
            Write("GetUserItemVote");
            // GetUserItemVoteResult_t
            return 0;
        }

        public SteamAPICall_t AddItemToFavorites(uint nAppId, ulong nPublishedFileID)
        {
            Write("AddItemToFavorites");
            // UserFavoriteItemsListChanged_t
            return 0;
        }

        public SteamAPICall_t RemoveItemFromFavorites(uint nAppId, ulong nPublishedFileID)
        {
            Write("RemoveItemFromFavorites");
            // UserFavoriteItemsListChanged_t
            return 0;
        }

        public SteamAPICall_t SubscribeItem(PublishedFileId_t nPublishedFileID) 
        {
            try
            {
                Write("SubscribeItem");
                subscribed.Add(nPublishedFileID);
                RemoteStorageSubscribePublishedFileResult_t data = new RemoteStorageSubscribePublishedFileResult_t()
                {
                    m_eResult = EResult.k_EResultOK,
                    m_nPublishedFileId = nPublishedFileID
                };
                return CallbackManager.AddCallbackResult(data);
            }
            catch (Exception ex)
            {
                Write($"SubscribeItem {ex}");
            }
            return 0;
        }

        public SteamAPICall_t UnsubscribeItem(PublishedFileId_t nPublishedFileID) // unsubscribe from this item, will be uninstalled after game quits 
        {
            Write("UnsubscribeItem");
            try
            {
                Write("SubscribeItem");
                subscribed.Add(nPublishedFileID);
                RemoteStorageUnsubscribePublishedFileResult_t data = new RemoteStorageUnsubscribePublishedFileResult_t()
                {
                    m_eResult = EResult.k_EResultOK,
                    m_nPublishedFileId = nPublishedFileID
                };
                if (subscribed.Contains(nPublishedFileID))
                {
                    subscribed.Remove(nPublishedFileID);
                }
                return CallbackManager.AddCallbackResult(data);
            }
            catch (Exception ex)
            {
                Write($"SubscribeItem {ex}");
            }
            return 0;
        }

        public uint GetNumSubscribedItems() // number of subscribed items  
        {
            Write("GetNumSubscribedItems");
            return (uint)subscribed.Count();
        }

        public uint GetSubscribedItems(ulong pvecPublishedFileID, uint cMaxEntries) // all subscribed item PublishFileIDs 
        {
            Write("GetSubscribedItems");
            if (cMaxEntries > subscribed.Count())
            {
                cMaxEntries = (uint)subscribed.Count();
            }
            return cMaxEntries;
        }

        public uint GetItemState(ulong nPublishedFileID)
        {
            Write("GetItemState");
            return (uint)EItemState.k_EItemStateSubscribed;
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

        public SteamAPICall_t StartPlaytimeTracking(PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs)
        {
            try
            {
                Write("StartPlaytimeTracking");
                StopPlaytimeTrackingResult_t data = new StopPlaytimeTrackingResult_t()
                {
                     m_eResult = EResult.k_EResultOK
                };
                return CallbackManager.AddCallbackResult(data);
            }
            catch (Exception ex)
            {
                Write($"StartPlaytimeTracking {ex}");
            }
            return 0;
        }

        public SteamAPICall_t StopPlaytimeTracking(ulong pvecPublishedFileID, uint unNumPublishedFileIDs)
        {
            try
            {
                Write("StopPlaytimeTracking");
                StopPlaytimeTrackingResult_t data = new StopPlaytimeTrackingResult_t()
                {
                    m_eResult = EResult.k_EResultOK
                };
                return CallbackManager.AddCallbackResult(data);
            }
            catch (Exception ex)
            {
                Write($"StopPlaytimeTracking {ex}");
            }
            return 0;
        }

        public SteamAPICall_t StopPlaytimeTrackingForAllItems()
        {
            try
            {
                Write("StopPlaytimeTracking");
                StopPlaytimeTrackingResult_t data = new StopPlaytimeTrackingResult_t()
                {
                    m_eResult = EResult.k_EResultOK
                };
                return CallbackManager.AddCallbackResult(data);
            }
            catch (Exception ex)
            {
                Write($"StopPlaytimeTracking {ex}");
            }
            return 0;
        }

        public SteamAPICall_t AddDependency(ulong nParentPublishedFileID, ulong nChildPublishedFileID)
        {
            Write("AddDependency");
            // AddAppDependencyResult_t
            return 0;
        }

        public SteamAPICall_t RemoveDependency(ulong nParentPublishedFileID, ulong nChildPublishedFileID)
        {
            Write("RemoveDependency");
            // RemoveAppDependencyResult_t
            return 0;
        }

        public SteamAPICall_t AddAppDependency(ulong nPublishedFileID, uint nAppID)
        {
            Write("AddAppDependency");
            // AddAppDependencyResult_t
            return 0;
        }

        public SteamAPICall_t RemoveAppDependency(ulong nPublishedFileID, uint nAppID)
        {
            Write("RemoveAppDependency");
            // RemoveAppDependencyResult_t
            return default;
        }

        public SteamAPICall_t GetAppDependencies(ulong nPublishedFileID)
        {
            Write("GetAppDependencies");
            // GetAppDependenciesResult_t
            return 0;
        }

        public SteamAPICall_t DeleteItem(ulong nPublishedFileID)
        {
            Write("DeleteItem");
            // DeleteItemResult_t
            return 0;
        }

        public bool ShowWorkshopEULA()
        {
            Write("ShowWorkshopEULA");
            return false;
        }

        public SteamAPICall_t GetWorkshopEULAStatus()
        {
            Write("GetWorkshopEULAStatus");
            return 0;
        }

        ////////////////////////////////////////////////////////////////////////////
        
        internal UGCQueryHandle_t CreateOne(bool returnAll = false)
        {
            UGC instance = new UGC();
            instance.ReturnAll = returnAll;
            instance.Handle = Handle;

            Handle.m_UGCQueryHandle++;

            UGCQueries.Add(instance);
            return instance.Handle;
        }

        void set_details(PublishedFileId_t id, IntPtr ptrDetails)
        {
            try
            {
                SteamUGCDetails_t pDetails = Marshal.PtrToStructure<SteamUGCDetails_t>(ptrDetails);
                if (true)
                {
                    pDetails.m_eResult = SKYNET.Result.OK;
                    pDetails.m_nPublishedFileId = id;
                    pDetails.m_eFileType = EWorkshopFileType.k_EWorkshopFileTypeCommunity;
                    pDetails.m_nCreatorAppID = SteamEmulator.AppId;
                    pDetails.m_nConsumerAppID = SteamEmulator.AppId;
                    //TODO
                }
                else
                {
                    pDetails.m_nPublishedFileId = id;
                    pDetails.m_eResult = SKYNET.Result.Fail;
                }

            }
            catch (Exception ex)
            {
                Write(ex.ToString());
            }
        }
    }
}