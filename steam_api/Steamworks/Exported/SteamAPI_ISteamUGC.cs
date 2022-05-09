using System;
using System.Runtime.InteropServices;
using SKYNET.Managers;

using SteamAPICall_t = System.UInt64;
using PublishedFileId_t = System.UInt64;
using UGCQueryHandle_t = System.UInt64;
using UGCUpdateHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamUGC 
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static UGCQueryHandle_t SteamAPI_ISteamUGC_CreateQueryUserUGCRequest(IntPtr _, uint unAccountID, int eListType, int eMatchingUGCType, int eSortOrder, uint nCreatorAppID, uint nConsumerAppID, uint unPage)
        {
            Write("SteamAPI_ISteamUGC_CreateQueryUserUGCRequest");
            return SteamEmulator.SteamUGC.CreateQueryUserUGCRequest(unAccountID, eListType, eMatchingUGCType, eSortOrder, nCreatorAppID, nConsumerAppID, unPage);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static UGCQueryHandle_t SteamAPI_ISteamUGC_CreateQueryAllUGCRequest(IntPtr _, int eQueryType, int eMatchingeMatchingUGCTypeFileType, uint nCreatorAppID, uint nConsumerAppID, uint unPage)
        {
            Write("SteamAPI_ISteamUGC_CreateQueryAllUGCRequest");
            return SteamEmulator.SteamUGC.CreateQueryAllUGCRequest(eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, unPage);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static UGCQueryHandle_t SteamAPI_ISteamUGC_CreateQueryUGCDetailsRequest(IntPtr _, PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs)
        {
            Write("SteamAPI_ISteamUGC_CreateQueryUGCDetailsRequest");
            return SteamEmulator.SteamUGC.CreateQueryUGCDetailsRequest(pvecPublishedFileID, unNumPublishedFileIDs);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_SendQueryUGCRequest(IntPtr _, UGCQueryHandle_t handle)
        {
            Write("SteamAPI_ISteamUGC_SendQueryUGCRequest");
            return SteamEmulator.SteamUGC.SendQueryUGCRequest(handle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_GetQueryUGCResult(IntPtr _, UGCQueryHandle_t handle, uint index, IntPtr pDetails)
        {
            Write("SteamAPI_ISteamUGC_GetQueryUGCResult");
            return SteamEmulator.SteamUGC.GetQueryUGCResult(handle, index, pDetails);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_GetQueryUGCPreviewURL(IntPtr _, UGCQueryHandle_t handle, uint index, string pchURL, uint cchURLSize)
        {
            Write("SteamAPI_ISteamUGC_GetQueryUGCPreviewURL");
            return SteamEmulator.SteamUGC.GetQueryUGCPreviewURL(handle, index, pchURL, cchURLSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_GetQueryUGCMetadata(IntPtr _, UGCQueryHandle_t handle, uint index, string pchMetadata, uint cchMetadatasize)
        {
            Write("SteamAPI_ISteamUGC_GetQueryUGCMetadata");
            return SteamEmulator.SteamUGC.GetQueryUGCMetadata(handle, index, pchMetadata, cchMetadatasize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_GetQueryUGCChildren(IntPtr _, UGCQueryHandle_t handle, uint index, PublishedFileId_t pvecPublishedFileID, uint cMaxEntries)
        {
            Write("SteamAPI_ISteamUGC_GetQueryUGCChildren");
            return SteamEmulator.SteamUGC.GetQueryUGCChildren(handle, index, pvecPublishedFileID, cMaxEntries);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_GetQueryUGCStatistic(IntPtr _, UGCQueryHandle_t handle, uint index, int eStatType, uint pStatValue)
        {
            Write("SteamAPI_ISteamUGC_GetQueryUGCStatistic");
            return SteamEmulator.SteamUGC.GetQueryUGCStatistic(handle, index, eStatType, pStatValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUGC_GetQueryUGCNumAdditionalPreviews(IntPtr _, UGCQueryHandle_t handle, uint index)
        {
            Write("SteamAPI_ISteamUGC_GetQueryUGCNumAdditionalPreviews");
            return SteamEmulator.SteamUGC.GetQueryUGCNumAdditionalPreviews(handle, index);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_GetQueryUGCAdditionalPreview(IntPtr _, UGCQueryHandle_t handle, uint index, uint previewIndex, string pchURLOrVideoID, uint cchURLSize, string pchOriginalFileName, uint cchOriginalFileNameSize, int pPreviewType)
        {
            Write("SteamAPI_ISteamUGC_GetQueryUGCAdditionalPreview");
            return SteamEmulator.SteamUGC.GetQueryUGCAdditionalPreview(handle, index, previewIndex, pchURLOrVideoID, cchURLSize, pchOriginalFileName, cchOriginalFileNameSize, pPreviewType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUGC_GetQueryUGCNumKeyValueTags(IntPtr _, UGCQueryHandle_t handle, uint index)
        {
            Write("SteamAPI_ISteamUGC_GetQueryUGCNumKeyValueTags");
            return SteamEmulator.SteamUGC.GetQueryUGCNumKeyValueTags(handle, index);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_GetQueryUGCKeyValueTag(IntPtr _, UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, string pchKey, uint cchKeySize, string pchValue, uint cchValueSize)
        {
            Write("SteamAPI_ISteamUGC_GetQueryUGCKeyValueTag");
            return SteamEmulator.SteamUGC.GetQueryUGCKeyValueTag(handle, index, keyValueTagIndex, pchKey, cchKeySize, pchValue, cchValueSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_ReleaseQueryUGCRequest(IntPtr _, UGCQueryHandle_t handle)
        {
            Write("SteamAPI_ISteamUGC_ReleaseQueryUGCRequest");
            return SteamEmulator.SteamUGC.ReleaseQueryUGCRequest(handle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_AddRequiredTag(IntPtr _, UGCQueryHandle_t handle, string pTagName)
        {
            Write("SteamAPI_ISteamUGC_AddRequiredTag");
            return SteamEmulator.SteamUGC.AddRequiredTag(handle, pTagName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_AddRequiredTagGroup(IntPtr _, UGCQueryHandle_t handle, IntPtr pTagGroups) // match any of the tags in this group 
        {
            Write("SteamAPI_ISteamUGC_AddRequiredTagGroup");
            return SteamEmulator.SteamUGC.AddRequiredTagGroup(handle, pTagGroups);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_AddExcludedTag(IntPtr _, UGCQueryHandle_t handle, string pTagName)
        {
            Write("SteamAPI_ISteamUGC_AddExcludedTag");
            return SteamEmulator.SteamUGC.AddExcludedTag(handle, pTagName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetReturnOnlyIDs(IntPtr _, UGCQueryHandle_t handle, bool bReturnOnlyIDs)
        {
            Write("SteamAPI_ISteamUGC_SetReturnOnlyIDs");
            return SteamEmulator.SteamUGC.SetReturnOnlyIDs(handle, bReturnOnlyIDs);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetReturnKeyValueTags(IntPtr _, UGCQueryHandle_t handle, bool bReturnKeyValueTags)
        {
            Write("SteamAPI_ISteamUGC_SetReturnKeyValueTags");
            return SteamEmulator.SteamUGC.SetReturnKeyValueTags(handle, bReturnKeyValueTags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetReturnLongDescription(IntPtr _, UGCQueryHandle_t handle, bool bReturnLongDescription)
        {
            Write("SteamAPI_ISteamUGC_SetReturnLongDescription");
            return SteamEmulator.SteamUGC.SetReturnLongDescription(handle, bReturnLongDescription);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetReturnMetadata(IntPtr _, UGCQueryHandle_t handle, bool bReturnMetadata)
        {
            Write("SteamAPI_ISteamUGC_SetReturnMetadata");
            return SteamEmulator.SteamUGC.SetReturnMetadata(handle, bReturnMetadata);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetReturnChildren(IntPtr _, UGCQueryHandle_t handle, bool bReturnChildren)
        {
            Write("SteamAPI_ISteamUGC_SetReturnChildren");
            return SteamEmulator.SteamUGC.SetReturnChildren(handle, bReturnChildren);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetReturnAdditionalPreviews(IntPtr _, UGCQueryHandle_t handle, bool bReturnAdditionalPreviews)
        {
            Write("SteamAPI_ISteamUGC_SetReturnAdditionalPreviews");
            return SteamEmulator.SteamUGC.SetReturnAdditionalPreviews(handle, bReturnAdditionalPreviews);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetReturnTotalOnly(IntPtr _, UGCQueryHandle_t handle, bool bReturnTotalOnly)
        {
            Write("SteamAPI_ISteamUGC_SetReturnTotalOnly");
            return SteamEmulator.SteamUGC.SetReturnTotalOnly(handle, bReturnTotalOnly);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetReturnPlaytimeStats(IntPtr _, UGCQueryHandle_t handle, uint unDays)
        {
            Write("SteamAPI_ISteamUGC_SetReturnPlaytimeStats");
            return SteamEmulator.SteamUGC.SetReturnPlaytimeStats(handle, unDays);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetLanguage(IntPtr _, UGCQueryHandle_t handle, string pchLanguage)
        {
            Write("SteamAPI_ISteamUGC_SetLanguage");
            return SteamEmulator.SteamUGC.SetLanguage(handle, pchLanguage);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetAllowCachedResponse(IntPtr _, UGCQueryHandle_t handle, uint unMaxAgeSeconds)
        {
            Write("SteamAPI_ISteamUGC_SetAllowCachedResponse");
            return SteamEmulator.SteamUGC.SetAllowCachedResponse(handle, unMaxAgeSeconds);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetCloudFileNameFilter(IntPtr _, UGCQueryHandle_t handle, string pMatchCloudFileName)
        {
            Write("SteamAPI_ISteamUGC_SetCloudFileNameFilter");
            return SteamEmulator.SteamUGC.SetCloudFileNameFilter(handle, pMatchCloudFileName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetMatchAnyTag(IntPtr _, UGCQueryHandle_t handle, bool bMatchAnyTag)
        {
            Write("SteamAPI_ISteamUGC_SetMatchAnyTag");
            return SteamEmulator.SteamUGC.SetMatchAnyTag(handle, bMatchAnyTag);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetSearchText(IntPtr _, UGCQueryHandle_t handle, string pSearchText)
        {
            Write("SteamAPI_ISteamUGC_SetSearchText");
            return SteamEmulator.SteamUGC.SetSearchText(handle, pSearchText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetRankedByTrendDays(IntPtr _, UGCQueryHandle_t handle, uint unDays)
        {
            Write("SteamAPI_ISteamUGC_SetRankedByTrendDays");
            return SteamEmulator.SteamUGC.SetRankedByTrendDays(handle, unDays);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_AddRequiredKeyValueTag(IntPtr _, UGCQueryHandle_t handle, string pKey, string pValue)
        {
            Write("SteamAPI_ISteamUGC_AddRequiredKeyValueTag");
            return SteamEmulator.SteamUGC.AddRequiredKeyValueTag(handle, pKey, pValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_RequestUGCDetails(IntPtr _, PublishedFileId_t nPublishedFileID, uint unMaxAgeSeconds)
        {
            Write("SteamAPI_ISteamUGC_RequestUGCDetails");
            return SteamEmulator.SteamUGC.RequestUGCDetails(nPublishedFileID, unMaxAgeSeconds);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_CreateItem(IntPtr _, uint nConsumerAppId, int eFileType) 
        {
            Write("SteamAPI_ISteamUGC_CreateItem");
            return SteamEmulator.SteamUGC.CreateItem(nConsumerAppId, eFileType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static UGCUpdateHandle_t SteamAPI_ISteamUGC_StartItemUpdate(IntPtr _, uint nConsumerAppId, PublishedFileId_t nPublishedFileID) 
        {
            Write("SteamAPI_ISteamUGC_StartItemUpdate");
            return SteamEmulator.SteamUGC.StartItemUpdate(nConsumerAppId, nPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetItemTitle(IntPtr _, UGCUpdateHandle_t handle, string pchTitle) // change the title of an UGC item 
        {
            Write("SteamAPI_ISteamUGC_SetItemTitle");
            return SteamEmulator.SteamUGC.SetItemTitle(handle, pchTitle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetItemDescription(IntPtr _, UGCUpdateHandle_t handle, string pchDescription) // change the description of an UGC item 
        {
            Write("SteamAPI_ISteamUGC_SetItemDescription");
            return SteamEmulator.SteamUGC.SetItemDescription(handle, pchDescription);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetItemUpdateLanguage(IntPtr _, UGCUpdateHandle_t handle, string pchLanguage) // specify the language of the title or description that will be set 
        {
            Write("SteamAPI_ISteamUGC_SetItemUpdateLanguage");
            return SteamEmulator.SteamUGC.SetItemUpdateLanguage(handle, pchLanguage);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetItemMetadata(IntPtr _, UGCUpdateHandle_t handle, string pchMetaData) // change the metadata of an UGC item (max = k_cchDeveloperMetadataMax) 
        {
            Write("SteamAPI_ISteamUGC_SetItemMetadata");
            return SteamEmulator.SteamUGC.SetItemMetadata(handle, pchMetaData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetItemVisibility(IntPtr _, UGCUpdateHandle_t handle, int eVisibility) // change the visibility of an UGC item 
        {
            Write("SteamAPI_ISteamUGC_SetItemVisibility");
            return SteamEmulator.SteamUGC.SetItemVisibility(handle, eVisibility);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetItemTags(IntPtr _, UGCUpdateHandle_t updateHandle, IntPtr pTags) // change the tags of an UGC item 
        {
            Write("SteamAPI_ISteamUGC_SetItemTags");
            return SteamEmulator.SteamUGC.SetItemTags(updateHandle, pTags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetItemContent(IntPtr _, UGCUpdateHandle_t handle, string pszContentFolder) // update item content from this local folder 
        {
            Write("SteamAPI_ISteamUGC_SetItemContent");
            return SteamEmulator.SteamUGC.SetItemContent(handle, pszContentFolder);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetItemPreview(IntPtr _, UGCUpdateHandle_t handle, string pszPreviewFile) //  change preview image file for this item. pszPreviewFile points to local image file, which must be under 1MB in size 
        {
            Write("SteamAPI_ISteamUGC_SetItemPreview");
            return SteamEmulator.SteamUGC.SetItemPreview(handle, pszPreviewFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_SetAllowLegacyUpload(IntPtr _, UGCUpdateHandle_t handle, bool bAllowLegacyUpload) //  use legacy upload for a single small file. The parameter to SetItemContent() should either be a directory with one file or the full path to the file.  The file must also be less than 10MB in size. 
        {
            Write("SteamAPI_ISteamUGC_SetAllowLegacyUpload");
            return SteamEmulator.SteamUGC.SetAllowLegacyUpload(handle, bAllowLegacyUpload);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_RemoveAllItemKeyValueTags(IntPtr _, UGCUpdateHandle_t handle) // remove all existing key-value tags (you can add new ones via the AddItemKeyValueTag function) 
        {
            Write("SteamAPI_ISteamUGC_RemoveAllItemKeyValueTags");
            return SteamEmulator.SteamUGC.RemoveAllItemKeyValueTags(handle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_RemoveItemKeyValueTags(IntPtr _, UGCUpdateHandle_t handle, string pchKey) // remove any existing key-value tags with the specified key 
        {
            Write("SteamAPI_ISteamUGC_RemoveItemKeyValueTags");
            return SteamEmulator.SteamUGC.RemoveItemKeyValueTags(handle, pchKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_AddItemKeyValueTag(IntPtr _, UGCUpdateHandle_t handle, string pchKey, string pchValue) // add new key-value tags for the item. Note that there can be multiple values for a tag. 
        {
            Write("SteamAPI_ISteamUGC_AddItemKeyValueTag");
            return SteamEmulator.SteamUGC.AddItemKeyValueTag(handle, pchKey, pchValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_AddItemPreviewFile(IntPtr _, UGCUpdateHandle_t handle, string pszPreviewFile, int type) //  add preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size 
        {
            Write("SteamAPI_ISteamUGC_AddItemPreviewFile");
            return SteamEmulator.SteamUGC.AddItemPreviewFile(handle, pszPreviewFile, type);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_AddItemPreviewVideo(IntPtr _, UGCUpdateHandle_t handle, string pszVideoID) //  add preview video for this item 
        {
            Write("SteamAPI_ISteamUGC_AddItemPreviewVideo");
            return SteamEmulator.SteamUGC.AddItemPreviewVideo(handle, pszVideoID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_UpdateItemPreviewFile(IntPtr _, UGCUpdateHandle_t handle, uint index, string pszPreviewFile) //  updates an existing preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size 
        {
            Write("SteamAPI_ISteamUGC_UpdateItemPreviewFile");
            return SteamEmulator.SteamUGC.UpdateItemPreviewFile(handle, index, pszPreviewFile);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_UpdateItemPreviewVideo(IntPtr _, UGCUpdateHandle_t handle, uint index, string pszVideoID) //  updates an existing preview video for this item 
        {
            Write("SteamAPI_ISteamUGC_UpdateItemPreviewVideo");
            return SteamEmulator.SteamUGC.UpdateItemPreviewVideo(handle, index, pszVideoID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_RemoveItemPreview(IntPtr _, UGCUpdateHandle_t handle, uint index) // remove a preview by index starting at 0 (previews are sorted) 
        {
            Write("SteamAPI_ISteamUGC_RemoveItemPreview");
            return SteamEmulator.SteamUGC.RemoveItemPreview(handle, index);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_SubmitItemUpdate(IntPtr _, UGCUpdateHandle_t handle, string pchChangeNote) // commit update process started with StartItemUpdate() 
        {
            Write("SteamAPI_ISteamUGC_SubmitItemUpdate");
            return SteamEmulator.SteamUGC.SubmitItemUpdate(handle, pchChangeNote);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamUGC_GetItemUpdateProgress(IntPtr _, UGCUpdateHandle_t handle, uint punBytesProcessed, uint punBytesTotal)
        {
            Write("SteamAPI_ISteamUGC_GetItemUpdateProgress");
            return SteamEmulator.SteamUGC.GetItemUpdateProgress(handle, punBytesProcessed, punBytesTotal);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_SetUserItemVote(IntPtr _, PublishedFileId_t nPublishedFileID, bool bVoteUp)
        {
            Write("SteamAPI_ISteamUGC_SetUserItemVote");
            return SteamEmulator.SteamUGC.SetUserItemVote(nPublishedFileID, bVoteUp);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_GetUserItemVote(IntPtr _, PublishedFileId_t nPublishedFileID)
        {
            Write("SteamAPI_ISteamUGC_GetUserItemVote");
            return SteamEmulator.SteamUGC.GetUserItemVote(nPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_AddItemToFavorites(IntPtr _, uint nAppId, PublishedFileId_t nPublishedFileID)
        {
            Write("SteamAPI_ISteamUGC_AddItemToFavorites");
            return SteamEmulator.SteamUGC.AddItemToFavorites(nAppId, nPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_RemoveItemFromFavorites(IntPtr _, uint nAppId, PublishedFileId_t nPublishedFileID)
        {
            Write("SteamAPI_ISteamUGC_RemoveItemFromFavorites");
            return SteamEmulator.SteamUGC.RemoveItemFromFavorites(nAppId, nPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_SubscribeItem(IntPtr _, PublishedFileId_t nPublishedFileID) // subscribe to this item, will be installed ASAP 
        {
            Write("SteamAPI_ISteamUGC_SubscribeItem");
            return SteamEmulator.SteamUGC.SubscribeItem(nPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_UnsubscribeItem(IntPtr _, PublishedFileId_t nPublishedFileID) // unsubscribe from this item, will be uninstalled after game quits 
        {
            Write("SteamAPI_ISteamUGC_UnsubscribeItem");
            return SteamEmulator.SteamUGC.UnsubscribeItem(nPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUGC_GetNumSubscribedItems(IntPtr _) // number of subscribed items  
        {
            Write("SteamAPI_ISteamUGC_GetNumSubscribedItems");
            return SteamEmulator.SteamUGC.GetNumSubscribedItems();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUGC_GetSubscribedItems(IntPtr _, PublishedFileId_t pvecPublishedFileID, uint cMaxEntries) // all subscribed item PublishFileIDs 
        {
            Write("SteamAPI_ISteamUGC_GetSubscribedItems");
            return SteamEmulator.SteamUGC.GetSubscribedItems(pvecPublishedFileID, cMaxEntries);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamUGC_GetItemState(IntPtr _, PublishedFileId_t nPublishedFileID)
        {
            Write("SteamAPI_ISteamUGC_GetItemState");
            return SteamEmulator.SteamUGC.GetItemState(nPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_GetItemInstallInfo(IntPtr _, PublishedFileId_t nPublishedFileID, uint punSizeOnDisk, string pchFolder, uint cchFolderSize, uint punTimeStamp)
        {
            Write("SteamAPI_ISteamUGC_GetItemInstallInfo");
            return SteamEmulator.SteamUGC.GetItemInstallInfo(nPublishedFileID, punSizeOnDisk, pchFolder, cchFolderSize, punTimeStamp);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_GetItemDownloadInfo(IntPtr _, PublishedFileId_t nPublishedFileID, uint punBytesDownloaded, uint punBytesTotal)
        {
            Write("SteamAPI_ISteamUGC_GetItemDownloadInfo");
            return SteamEmulator.SteamUGC.GetItemDownloadInfo(nPublishedFileID, punBytesDownloaded, punBytesTotal);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_DownloadItem(IntPtr _, PublishedFileId_t nPublishedFileID, bool bHighPriority)
        {
            Write("SteamAPI_ISteamUGC_DownloadItem");
            return SteamEmulator.SteamUGC.DownloadItem(nPublishedFileID, bHighPriority);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamUGC_BInitWorkshopForGameServer(IntPtr _, uint unWorkshopDepotID, string pszFolder)
        {
            Write("SteamAPI_ISteamUGC_BInitWorkshopForGameServer");
            return SteamEmulator.SteamUGC.BInitWorkshopForGameServer(unWorkshopDepotID, pszFolder);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamUGC_SuspendDownloads(IntPtr _, bool bSuspend)
        {
            Write("SteamAPI_ISteamUGC_SuspendDownloads");
            //
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_StartPlaytimeTracking(IntPtr _, PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs)
        {
            Write("SteamAPI_ISteamUGC_StartPlaytimeTracking");
            return SteamEmulator.SteamUGC.StartPlaytimeTracking(pvecPublishedFileID, unNumPublishedFileIDs);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_StopPlaytimeTracking(IntPtr _, PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs)
        {
            Write("SteamAPI_ISteamUGC_StopPlaytimeTracking");
            return SteamEmulator.SteamUGC.StopPlaytimeTracking(pvecPublishedFileID, unNumPublishedFileIDs);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_StopPlaytimeTrackingForAllItems(IntPtr _)
        {
            Write("SteamAPI_ISteamUGC_StopPlaytimeTrackingForAllItems");
            return SteamEmulator.SteamUGC.StopPlaytimeTrackingForAllItems();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_AddDependency(IntPtr _, PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID)
        {
            Write("SteamAPI_ISteamUGC_AddDependency");
            return SteamEmulator.SteamUGC.AddDependency(nParentPublishedFileID, nChildPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_RemoveDependency(IntPtr _, PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID)
        {
            Write("SteamAPI_ISteamUGC_RemoveDependency");
            return SteamEmulator.SteamUGC.RemoveDependency(nParentPublishedFileID, nChildPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_AddAppDependency(IntPtr _, PublishedFileId_t nPublishedFileID, uint nAppID)
        {
            Write("SteamAPI_ISteamUGC_AddAppDependency");
            return SteamEmulator.SteamUGC.AddAppDependency(nPublishedFileID, nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_RemoveAppDependency(IntPtr _, PublishedFileId_t nPublishedFileID, uint nAppID)
        {
            Write("SteamAPI_ISteamUGC_RemoveAppDependency");
            return SteamEmulator.SteamUGC.RemoveAppDependency(nPublishedFileID, nAppID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_GetAppDependencies(IntPtr _, PublishedFileId_t nPublishedFileID)
        {
            Write("SteamAPI_ISteamUGC_GetAppDependencies");
            return SteamEmulator.SteamUGC.GetAppDependencies(nPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamUGC_DeleteItem(IntPtr _, PublishedFileId_t nPublishedFileID)
        {
            Write("SteamAPI_ISteamUGC_DeleteItem");
            return SteamEmulator.SteamUGC.DeleteItem(nPublishedFileID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamUGC_v014()
        {
            Write("SteamAPI_SteamUGC_v014");
            return InterfaceManager.FindOrCreateInterface("STEAMUGC_INTERFACE_VERSION014");
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
