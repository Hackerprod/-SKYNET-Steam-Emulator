using SKYNET.Interface;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public class SteamAPI_ISteamUGC : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static UGCQueryHandle_t SteamAPI_ISteamUGC_CreateQueryUserUGCRequest(IntPtr unAccountID, IntPtr eListType, int eMatchingUGCType, int eSortOrder, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage)
    {
        Write("SteamAPI_ISteamUGC_CreateQueryUserUGCRequest");
        return SteamClient.SteamUGC.CreateQueryUserUGCRequest(unAccountID, eListType, eMatchingUGCType, eSortOrder, nCreatorAppID, nConsumerAppID, unPage);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static UGCQueryHandle_t SteamAPI_ISteamUGC_CreateQueryAllUGCRequest(int eQueryType, int eMatchingeMatchingUGCTypeFileType, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage)
    {
        Write("SteamAPI_ISteamUGC_CreateQueryAllUGCRequest");
        return SteamClient.SteamUGC.CreateQueryAllUGCRequest(eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, unPage);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static UGCQueryHandle_t SteamAPI_ISteamUGC_CreateQueryUGCDetailsRequest(PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs)
    {
        Write("SteamAPI_ISteamUGC_CreateQueryUGCDetailsRequest");
        return SteamClient.SteamUGC.CreateQueryUGCDetailsRequest(pvecPublishedFileID, unNumPublishedFileIDs);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamUGC_SendQueryUGCRequest(UGCQueryHandle_t handle)
    {
        Write("SteamAPI_ISteamUGC_SendQueryUGCRequest");
        return SteamClient.SteamUGC.SendQueryUGCRequest(handle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_GetQueryUGCResult(UGCQueryHandle_t handle, uint index, IntPtr pDetails)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCResult");
        return SteamClient.SteamUGC.GetQueryUGCResult(handle, index, pDetails);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_GetQueryUGCPreviewURL(UGCQueryHandle_t handle, uint index, string pchURL, uint cchURLSize)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCPreviewURL");
        return SteamClient.SteamUGC.GetQueryUGCPreviewURL(handle, index, pchURL, cchURLSize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_GetQueryUGCMetadata(UGCQueryHandle_t handle, uint index, string pchMetadata, uint cchMetadatasize)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCMetadata");
        return SteamClient.SteamUGC.GetQueryUGCMetadata(handle, index, pchMetadata, cchMetadatasize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_GetQueryUGCChildren(UGCQueryHandle_t handle, uint index, PublishedFileId_t pvecPublishedFileID, uint cMaxEntries)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCChildren");
        return SteamClient.SteamUGC.GetQueryUGCChildren(handle, index, pvecPublishedFileID, cMaxEntries);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_GetQueryUGCStatistic(UGCQueryHandle_t handle, uint index, EItemStatistic eStatType, uint pStatValue)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCStatistic");
        return SteamClient.SteamUGC.GetQueryUGCStatistic(handle, index, eStatType, pStatValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamUGC_GetQueryUGCNumAdditionalPreviews(UGCQueryHandle_t handle, uint index)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCNumAdditionalPreviews");
        return SteamClient.SteamUGC.GetQueryUGCNumAdditionalPreviews(handle, index);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_GetQueryUGCAdditionalPreview(UGCQueryHandle_t handle, uint index, uint previewIndex, string pchURLOrVideoID, uint cchURLSize, string pchOriginalFileName, uint cchOriginalFileNameSize, EItemPreviewType pPreviewType)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCAdditionalPreview");
        return SteamClient.SteamUGC.GetQueryUGCAdditionalPreview(handle, index, previewIndex, pchURLOrVideoID, cchURLSize, pchOriginalFileName, cchOriginalFileNameSize, pPreviewType);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamUGC_GetQueryUGCNumKeyValueTags(UGCQueryHandle_t handle, uint index)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCNumKeyValueTags");
        return SteamClient.SteamUGC.GetQueryUGCNumKeyValueTags(handle, index);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, string pchKey, uint cchKeySize, string pchValue, uint cchValueSize)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCKeyValueTag");
        return SteamClient.SteamUGC.GetQueryUGCKeyValueTag(handle, index, keyValueTagIndex, pchKey, cchKeySize, pchValue, cchValueSize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_ReleaseQueryUGCRequest(UGCQueryHandle_t handle)
    {
        Write("SteamAPI_ISteamUGC_ReleaseQueryUGCRequest");
        return SteamClient.SteamUGC.ReleaseQueryUGCRequest(handle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_AddRequiredTag(UGCQueryHandle_t handle, string pTagName)
    {
        Write("SteamAPI_ISteamUGC_AddRequiredTag");
        return SteamClient.SteamUGC.AddRequiredTag(handle, pTagName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_AddRequiredTagGroup(UGCQueryHandle_t handle, IntPtr pTagGroups) // match any of the tags in this group 
    {
        Write("SteamAPI_ISteamUGC_AddRequiredTagGroup");
        return SteamClient.SteamUGC.AddRequiredTagGroup(handle, pTagGroups);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_AddExcludedTag(UGCQueryHandle_t handle, string pTagName)
    {
        Write("SteamAPI_ISteamUGC_AddExcludedTag");
        return SteamClient.SteamUGC.AddExcludedTag(handle, pTagName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetReturnOnlyIDs(UGCQueryHandle_t handle, bool bReturnOnlyIDs)
    {
        Write("SteamAPI_ISteamUGC_SetReturnOnlyIDs");
        return SteamClient.SteamUGC.SetReturnOnlyIDs(handle, bReturnOnlyIDs);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetReturnKeyValueTags(UGCQueryHandle_t handle, bool bReturnKeyValueTags)
    {
        Write("SteamAPI_ISteamUGC_SetReturnKeyValueTags");
        return SteamClient.SteamUGC.SetReturnKeyValueTags(handle, bReturnKeyValueTags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetReturnLongDescription(UGCQueryHandle_t handle, bool bReturnLongDescription)
    {
        Write("SteamAPI_ISteamUGC_SetReturnLongDescription");
        return SteamClient.SteamUGC.SetReturnLongDescription(handle, bReturnLongDescription);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetReturnMetadata(UGCQueryHandle_t handle, bool bReturnMetadata)
    {
        Write("SteamAPI_ISteamUGC_SetReturnMetadata");
        return SteamClient.SteamUGC.SetReturnMetadata(handle, bReturnMetadata);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetReturnChildren(UGCQueryHandle_t handle, bool bReturnChildren)
    {
        Write("SteamAPI_ISteamUGC_SetReturnChildren");
        return SteamClient.SteamUGC.SetReturnChildren(handle, bReturnChildren);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetReturnAdditionalPreviews(UGCQueryHandle_t handle, bool bReturnAdditionalPreviews)
    {
        Write("SteamAPI_ISteamUGC_SetReturnAdditionalPreviews");
        return SteamClient.SteamUGC.SetReturnAdditionalPreviews(handle, bReturnAdditionalPreviews);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetReturnTotalOnly(UGCQueryHandle_t handle, bool bReturnTotalOnly)
    {
        Write("SteamAPI_ISteamUGC_SetReturnTotalOnly");
        return SteamClient.SteamUGC.SetReturnTotalOnly(handle, bReturnTotalOnly);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetReturnPlaytimeStats(UGCQueryHandle_t handle, uint unDays)
    {
        Write("SteamAPI_ISteamUGC_SetReturnPlaytimeStats");
        return SteamClient.SteamUGC.SetReturnPlaytimeStats(handle, unDays);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetLanguage(UGCQueryHandle_t handle, string pchLanguage)
    {
        Write("SteamAPI_ISteamUGC_SetLanguage");
        return SteamClient.SteamUGC.SetLanguage(handle, pchLanguage);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetAllowCachedResponse(UGCQueryHandle_t handle, uint unMaxAgeSeconds)
    {
        Write("SteamAPI_ISteamUGC_SetAllowCachedResponse");
        return SteamClient.SteamUGC.SetAllowCachedResponse(handle, unMaxAgeSeconds);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetCloudFileNameFilter(UGCQueryHandle_t handle, string pMatchCloudFileName)
    {
        Write("SteamAPI_ISteamUGC_SetCloudFileNameFilter");
        return SteamClient.SteamUGC.SetCloudFileNameFilter(handle, pMatchCloudFileName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetMatchAnyTag(UGCQueryHandle_t handle, bool bMatchAnyTag)
    {
        Write("SteamAPI_ISteamUGC_SetMatchAnyTag");
        return SteamClient.SteamUGC.SetMatchAnyTag(handle, bMatchAnyTag);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetSearchText(UGCQueryHandle_t handle, string pSearchText)
    {
        Write("SteamAPI_ISteamUGC_SetSearchText");
        return SteamClient.SteamUGC.SetSearchText(handle, pSearchText);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetRankedByTrendDays(UGCQueryHandle_t handle, uint unDays)
    {
        Write("SteamAPI_ISteamUGC_SetRankedByTrendDays");
        return SteamClient.SteamUGC.SetRankedByTrendDays(handle, unDays);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_AddRequiredKeyValueTag(UGCQueryHandle_t handle, string pKey, string pValue)
    {
        Write("SteamAPI_ISteamUGC_AddRequiredKeyValueTag");
        return SteamClient.SteamUGC.AddRequiredKeyValueTag(handle, pKey, pValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamUGC_RequestUGCDetails(PublishedFileId_t nPublishedFileID, uint unMaxAgeSeconds)
    {
        Write("SteamAPI_ISteamUGC_RequestUGCDetails");
        return SteamClient.SteamUGC.RequestUGCDetails(nPublishedFileID, unMaxAgeSeconds);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamUGC_CreateItem(AppId_t nConsumerAppId, EWorkshopFileType eFileType) // create new item for this app with no content attached yet 
    {
        Write("SteamAPI_ISteamUGC_CreateItem");
        return SteamClient.SteamUGC.CreateItem(nConsumerAppId, eFileType);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static UGCUpdateHandle_t SteamAPI_ISteamUGC_StartItemUpdate(AppId_t nConsumerAppId, PublishedFileId_t nPublishedFileID) // start an UGC item update. Set changed properties before commiting update with CommitItemUpdate() 
    {
        Write("SteamAPI_ISteamUGC_StartItemUpdate");
        return SteamClient.SteamUGC.StartItemUpdate(nConsumerAppId, nPublishedFileID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetItemTitle(UGCUpdateHandle_t handle, string pchTitle) // change the title of an UGC item 
    {
        Write("SteamAPI_ISteamUGC_SetItemTitle");
        return SteamClient.SteamUGC.SetItemTitle(handle, pchTitle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetItemDescription(UGCUpdateHandle_t handle, string pchDescription) // change the description of an UGC item 
    {
        Write("SteamAPI_ISteamUGC_SetItemDescription");
        return SteamClient.SteamUGC.SetItemDescription(handle, pchDescription);
    }

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_SetItemUpdateLanguage(UGCUpdateHandle_t handle, string pchLanguage) // specify the language of the title or description that will be set 
    //{
    //    Write("SteamAPI_ISteamUGC_SetItemUpdateLanguage");
    //    return SteamClient.SteamUGC.SetItemUpdateLanguage(handle, pchLanguage);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_SetItemMetadata(UGCUpdateHandle_t handle, string pchMetaData) // change the metadata of an UGC item (max = k_cchDeveloperMetadataMax) 
    //{
    //    Write("SteamAPI_ISteamUGC_SetItemMetadata");
    //    return SteamClient.SteamUGC.SetItemMetadata(handle, pchMetaData);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_SetItemVisibility(UGCUpdateHandle_t handle, ERemoteStoragePublishedFileVisibility eVisibility) // change the visibility of an UGC item 
    //{
    //    Write("SteamAPI_ISteamUGC_SetItemVisibility");
    //    return SteamClient.SteamUGC.SetItemVisibility(handle, eVisibility);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_SetItemTags(UGCUpdateHandle_t updateHandle, IntPtr pTags) // change the tags of an UGC item 
    //{
    //    Write("SteamAPI_ISteamUGC_SetItemTags");
    //    return SteamClient.SteamUGC.SetItemTags(updateHandle, pTags);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_SetItemContent(UGCUpdateHandle_t handle, string pszContentFolder) // update item content from this local folder 
    //{
    //    Write("SteamAPI_ISteamUGC_SetItemContent");
    //    return SteamClient.SteamUGC.SetItemContent(handle, pszContentFolder);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_SetItemPreview(UGCUpdateHandle_t handle, string pszPreviewFile) //  change preview image file for this item. pszPreviewFile points to local image file, which must be under 1MB in size 
    //{
    //    Write("SteamAPI_ISteamUGC_SetItemPreview");
    //    return SteamClient.SteamUGC.SetItemPreview(handle, pszPreviewFile);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_SetAllowLegacyUpload(UGCUpdateHandle_t handle, bool bAllowLegacyUpload) //  use legacy upload for a single small file. The parameter to SetItemContent() should either be a directory with one file or the full path to the file.  The file must also be less than 10MB in size. 
    //{
    //    Write("SteamAPI_ISteamUGC_SetAllowLegacyUpload");
    //    return SteamClient.SteamUGC.SetAllowLegacyUpload(handle, bAllowLegacyUpload);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_RemoveAllItemKeyValueTags(UGCUpdateHandle_t handle) // remove all existing key-value tags (you can add new ones via the AddItemKeyValueTag function) 
    //{
    //    Write("SteamAPI_ISteamUGC_RemoveAllItemKeyValueTags");
    //    return SteamClient.SteamUGC.RemoveAllItemKeyValueTags(handle);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_RemoveItemKeyValueTags(UGCUpdateHandle_t handle, string pchKey) // remove any existing key-value tags with the specified key 
    //{
    //    Write("SteamAPI_ISteamUGC_RemoveItemKeyValueTags");
    //    return SteamClient.SteamUGC.RemoveItemKeyValueTags(handle, pchKey);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_AddItemKeyValueTag(UGCUpdateHandle_t handle, string pchKey, string pchValue) // add new key-value tags for the item. Note that there can be multiple values for a tag. 
    //{
    //    Write("SteamAPI_ISteamUGC_AddItemKeyValueTag");
    //    return SteamClient.SteamUGC.AddItemKeyValueTag(handle, pchKey, pchValue);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_AddItemPreviewFile(UGCUpdateHandle_t handle, string pszPreviewFile, EItemPreviewType type) //  add preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size 
    //{
    //    Write("SteamAPI_ISteamUGC_AddItemPreviewFile");
    //    return SteamClient.SteamUGC.AddItemPreviewFile(handle, pszPreviewFile, type);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_AddItemPreviewVideo(UGCUpdateHandle_t handle, string pszVideoID) //  add preview video for this item 
    //{
    //    Write("SteamAPI_ISteamUGC_AddItemPreviewVideo");
    //    return SteamClient.SteamUGC.AddItemPreviewVideo(handle, pszVideoID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_UpdateItemPreviewFile(UGCUpdateHandle_t handle, uint index, string pszPreviewFile) //  updates an existing preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size 
    //{
    //    Write("SteamAPI_ISteamUGC_UpdateItemPreviewFile");
    //    return SteamClient.SteamUGC.UpdateItemPreviewFile(handle, index, pszPreviewFile);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_UpdateItemPreviewVideo(UGCUpdateHandle_t handle, uint index, string pszVideoID) //  updates an existing preview video for this item 
    //{
    //    Write("SteamAPI_ISteamUGC_UpdateItemPreviewVideo");
    //    return SteamClient.SteamUGC.UpdateItemPreviewVideo(handle, index, pszVideoID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_RemoveItemPreview(UGCUpdateHandle_t handle, uint index) // remove a preview by index starting at 0 (previews are sorted) 
    //{
    //    Write("SteamAPI_ISteamUGC_RemoveItemPreview");
    //    return SteamClient.SteamUGC.RemoveItemPreview(handle, index);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_SubmitItemUpdate(UGCUpdateHandle_t handle, string pchChangeNote) // commit update process started with StartItemUpdate() 
    //{
    //    Write("SteamAPI_ISteamUGC_SubmitItemUpdate");
    //    return SteamClient.SteamUGC.SubmitItemUpdate(handle, pchChangeNote);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static int SteamAPI_ISteamUGC_GetItemUpdateProgress(UGCUpdateHandle_t handle, uint punBytesProcessed, uint punBytesTotal)
    //{
    //    Write("SteamAPI_ISteamUGC_GetItemUpdateProgress");
    //    return SteamClient.SteamUGC.GetItemUpdateProgress(handle, punBytesProcessed, punBytesTotal);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_SetUserItemVote(PublishedFileId_t nPublishedFileID, bool bVoteUp)
    //{
    //    Write("SteamAPI_ISteamUGC_SetUserItemVote");
    //    return SteamClient.SteamUGC.SetUserItemVote(nPublishedFileID, bVoteUp);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_GetUserItemVote(PublishedFileId_t nPublishedFileID)
    //{
    //    Write("SteamAPI_ISteamUGC_GetUserItemVote");
    //    return SteamClient.SteamUGC.GetUserItemVote(nPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_AddItemToFavorites(AppId_t nAppId, PublishedFileId_t nPublishedFileID)
    //{
    //    Write("SteamAPI_ISteamUGC_AddItemToFavorites");
    //    return SteamClient.SteamUGC.AddItemToFavorites(nAppId, nPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_RemoveItemFromFavorites(AppId_t nAppId, PublishedFileId_t nPublishedFileID)
    //{
    //    Write("SteamAPI_ISteamUGC_RemoveItemFromFavorites");
    //    return SteamClient.SteamUGC.RemoveItemFromFavorites(nAppId, nPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_SubscribeItem(PublishedFileId_t nPublishedFileID) // subscribe to this item, will be installed ASAP 
    //{
    //    Write("SteamAPI_ISteamUGC_SubscribeItem");
    //    return SteamClient.SteamUGC.SubscribeItem(nPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_UnsubscribeItem(PublishedFileId_t nPublishedFileID) // unsubscribe from this item, will be uninstalled after game quits 
    //{
    //    Write("SteamAPI_ISteamUGC_UnsubscribeItem");
    //    return SteamClient.SteamUGC.UnsubscribeItem(nPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static uint SteamAPI_ISteamUGC_GetNumSubscribedItems() // number of subscribed items  
    //{
    //    Write("SteamAPI_ISteamUGC_GetNumSubscribedItems");
    //    return SteamClient.SteamUGC.GetNumSubscribedItems();
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static uint SteamAPI_ISteamUGC_GetSubscribedItems(PublishedFileId_t pvecPublishedFileID, uint cMaxEntries) // all subscribed item PublishFileIDs 
    //{
    //    Write("SteamAPI_ISteamUGC_GetSubscribedItems");
    //    return SteamClient.SteamUGC.GetSubscribedItems(pvecPublishedFileID, cMaxEntries);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static uint SteamAPI_ISteamUGC_GetItemState(PublishedFileId_t nPublishedFileID)
    //{
    //    Write("SteamAPI_ISteamUGC_GetItemState");
    //    return SteamClient.SteamUGC.GetItemState(nPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_GetItemInstallInfo(PublishedFileId_t nPublishedFileID, uint punSizeOnDisk, string pchFolder, uint cchFolderSize, uint punTimeStamp)
    //{
    //    Write("SteamAPI_ISteamUGC_GetItemInstallInfo");
    //    return SteamClient.SteamUGC.GetItemInstallInfo(nPublishedFileID, punSizeOnDisk, pchFolder, cchFolderSize, punTimeStamp);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_GetItemDownloadInfo(PublishedFileId_t nPublishedFileID, uint punBytesDownloaded, uint punBytesTotal)
    //{
    //    Write("SteamAPI_ISteamUGC_GetItemDownloadInfo");
    //    return SteamClient.SteamUGC.GetItemDownloadInfo(nPublishedFileID, punBytesDownloaded, punBytesTotal);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_DownloadItem(PublishedFileId_t nPublishedFileID, bool bHighPriority)
    //{
    //    Write("SteamAPI_ISteamUGC_DownloadItem");
    //    return SteamClient.SteamUGC.DownloadItem(nPublishedFileID, bHighPriority);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_BInitWorkshopForGameServer(uint unWorkshopDepotID, string pszFolder)
    //{
    //    Write("SteamAPI_ISteamUGC_BInitWorkshopForGameServer");
    //    return SteamClient.SteamUGC.BInitWorkshopForGameServer(unWorkshopDepotID, pszFolder);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static void SteamAPI_ISteamUGC_SuspendDownloads(bool bSuspend)
    //{
    //    Write("SteamAPI_ISteamUGC_SuspendDownloads");
    //    //
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_StartPlaytimeTracking(PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs)
    //{
    //    Write("SteamAPI_ISteamUGC_StartPlaytimeTracking");
    //    return SteamClient.SteamUGC.StartPlaytimeTracking(pvecPublishedFileID, unNumPublishedFileIDs);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_StopPlaytimeTracking(PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs)
    //{
    //    Write("SteamAPI_ISteamUGC_StopPlaytimeTracking");
    //    return SteamClient.SteamUGC.StopPlaytimeTracking(pvecPublishedFileID, unNumPublishedFileIDs);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_StopPlaytimeTrackingForAllItems()
    //{
    //    Write("SteamAPI_ISteamUGC_StopPlaytimeTrackingForAllItems");
    //    return SteamClient.SteamUGC.StopPlaytimeTrackingForAllItems();
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_AddDependency(PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID)
    //{
    //    Write("SteamAPI_ISteamUGC_AddDependency");
    //    return SteamClient.SteamUGC.AddDependency(nParentPublishedFileID, nChildPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_RemoveDependency(PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID)
    //{
    //    Write("SteamAPI_ISteamUGC_RemoveDependency");
    //    return SteamClient.SteamUGC.RemoveDependency(nParentPublishedFileID, nChildPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_AddAppDependency(PublishedFileId_t nPublishedFileID, AppId_t nAppID)
    //{
    //    Write("SteamAPI_ISteamUGC_AddAppDependency");
    //    return SteamClient.SteamUGC.AddAppDependency(nPublishedFileID, nAppID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_RemoveAppDependency(PublishedFileId_t nPublishedFileID, AppId_t nAppID)
    //{
    //    Write("SteamAPI_ISteamUGC_RemoveAppDependency");
    //    return SteamClient.SteamUGC.RemoveAppDependency(nPublishedFileID, nAppID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_GetAppDependencies(PublishedFileId_t nPublishedFileID)
    //{
    //    Write("SteamAPI_ISteamUGC_GetAppDependencies");
    //    return SteamClient.SteamUGC.GetAppDependencies(nPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_DeleteItem(PublishedFileId_t nPublishedFileID)
    //{
    //    Write("SteamAPI_ISteamUGC_DeleteItem");
    //    return SteamClient.SteamUGC.DeleteItem(nPublishedFileID);
    //}

}
