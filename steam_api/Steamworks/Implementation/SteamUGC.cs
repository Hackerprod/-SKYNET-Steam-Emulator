using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using SteamAPICall_t = System.UInt64;
using PublishedFileId_t = System.UInt64;
using UGCQueryHandle_t = System.UInt64;
using UGCUpdateHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamUGC : ISteamInterface
    {
        public static SteamUGC Instance;

        private List<UGC> UGCQueries;
        private UGCQueryHandle_t Handle;
        private List<PublishedFileId_t> subscribed;

        public SteamUGC()
        {
            Instance = this;
            InterfaceName = "SteamUGC";
            InterfaceVersion = "STEAMUGC_INTERFACE_VERSION012";
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
            Write("SendQueryUGCRequest");
            SteamAPICall_t APICall = k_uAPICallInvalid;
            MutexHelper.Wait("SendQueryUGCRequest", delegate
            {
                try
                {
                    var request = UGCQueries.Find(u => u.Handle == handle);
                    if (request != null)
                    {
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

                        //SteamUGCQueryCompleted_t data = new SteamUGCQueryCompleted_t()
                        //{
                        //    m_handle = handle,
                        //    m_eResult = EResult.k_EResultOK,
                        //    m_unNumResultsReturned = (uint)request.results.Count(),
                        //    m_unTotalMatchingResults = (uint)request.results.Count(),
                        //    m_bCachedData = false,
                        //};
                        ////APICall = new SteamAPICall_t(CallbackType.k_iSteamUGCQueryCompleted);
                        //APICall = CallbackManager.AddCallbackResult(data, SteamUGCQueryCompleted_t.k_iCallback);
                    }
                }
                catch (Exception ex)
                {
                    Write($"SendQueryUGCRequest {ex}");
                }
            });
            return APICall;
        }

        public bool GetQueryUGCResult(UGCQueryHandle_t handle, uint index, ref SteamUGCDetails_t pDetails)
        {
            Write("GetQueryUGCResult");
            bool Result = false;
            MutexHelper.Wait("GetQueryUGCResult", delegate
            {
                var UGC = UGCQueries.Find(u => u.Handle == handle);
                if (UGC != null && (index < UGC.results.Count))
                {
                    foreach (var item in UGC.results)
                    {
                        //set_details(item, pDetails);
                    }
                }
                Result = true;
            });
            return Result;
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

        public bool GetQueryUGCChildren(UGCQueryHandle_t handle, uint index, ref PublishedFileId_t[] pvecPublishedFileID, uint cMaxEntries)
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
            bool Return = false;
            MutexHelper.Wait("ReleaseQueryUGCRequest", delegate
            {
                var UGC = UGCQueries.Find(u => u.Handle == handle);
                if (UGC != null)
                {
                    UGCQueries.Remove(UGC);
                    Return = true;
                }
            });
            return Return;
        }

        public bool AddRequiredTag(UGCQueryHandle_t handle, string pTagName)
        {
            Write("AddRequiredTag");
            return false;
        }

        public bool AddRequiredTagGroup(UGCQueryHandle_t handle, IntPtr pTagGroups) 
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
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t CreateItem(uint nConsumerAppId, int eFileType)
        {
            Write("CreateItem");
            // CreateItemResult_t
            return k_uAPICallInvalid;
        }

        public UGCUpdateHandle_t StartItemUpdate(uint nConsumerAppId, ulong nPublishedFileID) 
        {
            Write("StartItemUpdate");
            return (UGCUpdateHandle_t)0;
        }

        public bool SetItemTitle(UGCQueryHandle_t handle, string pchTitle) 
        {
            Write("SetItemTitle");
            return false;
        }

        public bool SetItemDescription(UGCQueryHandle_t handle, string pchDescription) 
        {
            Write("SetItemDescription");
            return false;
        }

        public bool SetItemUpdateLanguage(UGCQueryHandle_t handle, string pchLanguage) 
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

        public bool SetItemContent(UGCQueryHandle_t handle, string pszContentFolder) 
        {
            Write("SetItemContent");
            return false;
        }

        public bool SetItemPreview(UGCQueryHandle_t handle, string pszPreviewFile) 
        {
            Write("SetItemPreview");
            return false;
        }

        public bool SetAllowLegacyUpload(UGCQueryHandle_t handle, bool bAllowLegacyUpload) 
        {
            Write("SetAllowLegacyUpload");
            return false;
        }

        public bool RemoveAllItemKeyValueTags(UGCQueryHandle_t handle) 
        {
            Write("RemoveAllItemKeyValueTags");
            return false;
        }

        public bool RemoveItemKeyValueTags(UGCQueryHandle_t handle, string pchKey) 
        {
            Write("RemoveItemKeyValueTags");
            return false;
        }

        public bool AddItemKeyValueTag(UGCQueryHandle_t handle, string pchKey, string pchValue) 
        {
            Write("AddItemKeyValueTag");
            return false;
        }

        public bool AddItemPreviewFile(UGCQueryHandle_t handle, string pszPreviewFile, int type) 
        {
            Write("AddItemPreviewFile");
            return false;
        }

        public bool AddItemPreviewVideo(UGCQueryHandle_t handle, string pszVideoID) 
        {
            Write("AddItemPreviewVideo");
            return false;
        }

        public bool UpdateItemPreviewFile(UGCQueryHandle_t handle, uint index, string pszPreviewFile) 
        {
            Write("UpdateItemPreviewFile");
            return false;
        }

        public bool UpdateItemPreviewVideo(UGCQueryHandle_t handle, uint index, string pszVideoID) 
        {
            Write("UpdateItemPreviewVideo");
            return false;
        }

        public bool RemoveItemPreview(UGCQueryHandle_t handle, uint index) 
        {
            Write("RemoveItemPreview");
            return false;
        }

        public SteamAPICall_t SubmitItemUpdate(UGCQueryHandle_t handle, string pchChangeNote) 
        {
            Write("SubmitItemUpdate");
            // SubmitItemUpdateResult_t
            return k_uAPICallInvalid;
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
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t GetUserItemVote(ulong nPublishedFileID)
        {
            Write("GetUserItemVote");
            // GetUserItemVoteResult_t
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t AddItemToFavorites(uint nAppId, ulong nPublishedFileID)
        {
            Write("AddItemToFavorites");
            // UserFavoriteItemsListChanged_t
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t RemoveItemFromFavorites(uint nAppId, ulong nPublishedFileID)
        {
            Write("RemoveItemFromFavorites");
            // UserFavoriteItemsListChanged_t
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t SubscribeItem(PublishedFileId_t nPublishedFileID) 
        {
            Write("SubscribeItem");
            SteamAPICall_t APICall = k_uAPICallInvalid;
            MutexHelper.Wait("SubscribeItem", delegate
            {
                try
                {
                    subscribed.Add(nPublishedFileID);
                    //RemoteStorageSubscribePublishedFileResult_t data = new RemoteStorageSubscribePublishedFileResult_t()
                    //{
                    //    m_eResult = EResult.k_EResultOK,
                    //    m_nPublishedFileId = nPublishedFileID
                    //};
                    //APICall = CallbackManager.AddCallbackResult(data, RemoteStorageSubscribePublishedFileResult_t.k_iCallback);
                }
                catch (Exception ex)
                {
                    Write($"SubscribeItem {ex}");
                }
            });

            return APICall;
        }

        public SteamAPICall_t UnsubscribeItem(PublishedFileId_t nPublishedFileID) 
        {
            Write("UnsubscribeItem");
            SteamAPICall_t APICall = k_uAPICallInvalid;
            MutexHelper.Wait("UnsubscribeItem", delegate
            {
                try
                {
                    if (subscribed.Contains(nPublishedFileID))
                    {
                        subscribed.Remove(nPublishedFileID);
                    }

                    RemoteStorageUnsubscribePublishedFileResult_t data = new RemoteStorageUnsubscribePublishedFileResult_t()
                    {
                        m_eResult = EResult.k_EResultOK,
                        m_nPublishedFileId = nPublishedFileID
                    };
                    APICall = CallbackManager.AddCallbackResult(data);
                }
                catch (Exception ex)
                {
                    Write($"SubscribeItem {ex}");
                }
            });

            return k_uAPICallInvalid;
        }

        public uint GetNumSubscribedItems() 
        {
            Write("GetNumSubscribedItems");
            return (uint)subscribed.Count();
        }

        public uint GetSubscribedItems(ulong pvecPublishedFileID, uint cMaxEntries) 
        {
            Write("GetSubscribedItems");
            uint MaxEntries = cMaxEntries;
            MutexHelper.Wait("GetSubscribedItems", delegate
            {
                if (MaxEntries > subscribed.Count())
                {
                    MaxEntries = (uint)subscribed.Count();
                }
            });
            cMaxEntries = MaxEntries;
            return MaxEntries;
        }

        public uint GetItemState(ulong nPublishedFileID)
        {
            Write("GetItemState");
            MutexHelper.Wait("GetItemState", delegate
            {
                // TODO
            });
            return (uint)EItemState.k_EItemStateSubscribed;
        }

        public bool GetItemInstallInfo(ulong nPublishedFileID, ulong punSizeOnDisk, string pchFolder, uint cchFolderSize, uint punTimeStamp)
        {
            Write("GetItemInstallInfo");
            MutexHelper.Wait("GetItemInstallInfo", delegate
            {
                // TODO
            });
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
            Write("StartPlaytimeTracking");
            SteamAPICall_t ApiCall = k_uAPICallInvalid;
            MutexHelper.Wait("StartPlaytimeTracking", delegate
            {
                try
                {
                    StopPlaytimeTrackingResult_t data = new StopPlaytimeTrackingResult_t()
                    {
                        m_eResult = EResult.k_EResultOK
                    };
                    ApiCall = CallbackManager.AddCallbackResult(data);
                }
                catch (Exception ex)
                {
                    Write($"StartPlaytimeTracking {ex}");
                }
            });
            return ApiCall;
        }

        public SteamAPICall_t StopPlaytimeTracking(ulong pvecPublishedFileID, uint unNumPublishedFileIDs)
        {
            Write("StopPlaytimeTracking");
            SteamAPICall_t ApiCall = k_uAPICallInvalid;
            MutexHelper.Wait("StopPlaytimeTracking", delegate
            {
                try
                {
                    StopPlaytimeTrackingResult_t data = new StopPlaytimeTrackingResult_t()
                    {
                        m_eResult = EResult.k_EResultOK
                    };
                    ApiCall = CallbackManager.AddCallbackResult(data);
                }
                catch (Exception ex)
                {
                    Write($"StopPlaytimeTracking {ex}");
                }
            });
            return ApiCall;

        }

        public SteamAPICall_t StopPlaytimeTrackingForAllItems()
        {
            try
            {
                Write("StopPlaytimeTracking");
                //StopPlaytimeTrackingResult_t data = new StopPlaytimeTrackingResult_t()
                //{
                //    m_eResult = EResult.k_EResultOK
                //};
                //return CallbackManager.AddCallbackResult(data, StopPlaytimeTrackingResult_t.k_iCallback);
            }
            catch (Exception ex)
            {
                Write($"StopPlaytimeTracking {ex}");
            }
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t AddDependency(ulong nParentPublishedFileID, ulong nChildPublishedFileID)
        {
            Write("AddDependency");
            // AddAppDependencyResult_t
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t RemoveDependency(ulong nParentPublishedFileID, ulong nChildPublishedFileID)
        {
            Write("RemoveDependency");
            // RemoveAppDependencyResult_t
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t AddAppDependency(ulong nPublishedFileID, uint nAppID)
        {
            Write("AddAppDependency");
            // AddAppDependencyResult_t
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t RemoveAppDependency(ulong nPublishedFileID, uint nAppID)
        {
            Write("RemoveAppDependency");
            // RemoveAppDependencyResult_t
            return 0;
        }

        public SteamAPICall_t GetAppDependencies(ulong nPublishedFileID)
        {
            Write("GetAppDependencies");
            // GetAppDependenciesResult_t
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t DeleteItem(ulong nPublishedFileID)
        {
            Write("DeleteItem");
            // DeleteItemResult_t
            return k_uAPICallInvalid;
        }

        public bool ShowWorkshopEULA()
        {
            Write("ShowWorkshopEULA");
            return false;
        }

        public SteamAPICall_t GetWorkshopEULAStatus()
        {
            Write("GetWorkshopEULAStatus");
            return k_uAPICallInvalid;
        }

        ////////////////////////////////////////////////////////////////////////////
        
        internal UGCQueryHandle_t CreateOne(bool returnAll = false)
        {
            UGC instance = new UGC();

            MutexHelper.Wait("CreateOne", delegate
            {
                instance.ReturnAll = returnAll;
                instance.Handle = Handle;

                Handle++;
            });

            UGCQueries.Add(instance);
            return instance.Handle;
        }

        void set_details(PublishedFileId_t id, IntPtr ptrDetails)
        {
            try
            {
                SteamUGCDetails_t pDetails = Marshal.PtrToStructure<SteamUGCDetails_t>(ptrDetails);
                pDetails.m_eResult = EResult.k_EResultOK;
                pDetails.m_nPublishedFileId = id;
                pDetails.m_eFileType = EWorkshopFileType.k_EWorkshopFileTypeCommunity;
                pDetails.m_nCreatorAppID = SteamEmulator.AppID;
                pDetails.m_nConsumerAppID = SteamEmulator.AppID;

                Marshal.StructureToPtr(pDetails, ptrDetails, false);
            }
            catch (Exception ex)
            {
                Write(ex.ToString());
            }
        }

        internal class UGC
        {
            public UGCQueryHandle_t Handle;
            public List<PublishedFileId_t> return_only;
            public List<PublishedFileId_t> results;
            public bool ReturnAll;
            public bool ReturnOnly;

            public UGC()
            {
                return_only = new List<PublishedFileId_t>();
                results = new List<PublishedFileId_t>();
            }
        }
    }


}