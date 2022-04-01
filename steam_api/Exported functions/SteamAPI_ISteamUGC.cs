using SKYNET;
using SKYNET.Steamworks;
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
        return SteamEmulator.SteamUGC.CreateQueryUserUGCRequest(SteamEmulator.SteamUGC.MemoryAddress, unAccountID, eListType, eMatchingUGCType, eSortOrder, nCreatorAppID, nConsumerAppID, unPage);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static UGCQueryHandle_t SteamAPI_ISteamUGC_CreateQueryAllUGCRequest(int eQueryType, int eMatchingeMatchingUGCTypeFileType, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint unPage)
    {
        Write("SteamAPI_ISteamUGC_CreateQueryAllUGCRequest");
        return SteamEmulator.SteamUGC.CreateQueryAllUGCRequest(SteamEmulator.SteamUGC.MemoryAddress, eQueryType, eMatchingeMatchingUGCTypeFileType, nCreatorAppID, nConsumerAppID, unPage);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static UGCQueryHandle_t SteamAPI_ISteamUGC_CreateQueryUGCDetailsRequest(PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs)
    {
        Write("SteamAPI_ISteamUGC_CreateQueryUGCDetailsRequest");
        return SteamEmulator.SteamUGC.CreateQueryUGCDetailsRequest(SteamEmulator.SteamUGC.MemoryAddress, pvecPublishedFileID, unNumPublishedFileIDs);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamUGC_SendQueryUGCRequest(UGCQueryHandle_t handle)
    {
        Write("SteamAPI_ISteamUGC_SendQueryUGCRequest");
        return SteamEmulator.SteamUGC.SendQueryUGCRequest(SteamEmulator.SteamUGC.MemoryAddress, handle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_GetQueryUGCResult(UGCQueryHandle_t handle, uint index, IntPtr pDetails)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCResult");
        return SteamEmulator.SteamUGC.GetQueryUGCResult(SteamEmulator.SteamUGC.MemoryAddress, handle, index, pDetails);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_GetQueryUGCPreviewURL(UGCQueryHandle_t handle, uint index, string pchURL, uint cchURLSize)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCPreviewURL");
        return SteamEmulator.SteamUGC.GetQueryUGCPreviewURL(SteamEmulator.SteamUGC.MemoryAddress, handle, index, pchURL, cchURLSize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_GetQueryUGCMetadata(UGCQueryHandle_t handle, uint index, string pchMetadata, uint cchMetadatasize)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCMetadata");
        return SteamEmulator.SteamUGC.GetQueryUGCMetadata(SteamEmulator.SteamUGC.MemoryAddress, handle, index, pchMetadata, cchMetadatasize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_GetQueryUGCChildren(UGCQueryHandle_t handle, uint index, PublishedFileId_t pvecPublishedFileID, uint cMaxEntries)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCChildren");
        return SteamEmulator.SteamUGC.GetQueryUGCChildren(SteamEmulator.SteamUGC.MemoryAddress, handle, index, pvecPublishedFileID, cMaxEntries);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_GetQueryUGCStatistic(UGCQueryHandle_t handle, uint index, EItemStatistic eStatType, uint pStatValue)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCStatistic");
        return SteamEmulator.SteamUGC.GetQueryUGCStatistic(SteamEmulator.SteamUGC.MemoryAddress, handle, index, eStatType, pStatValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamUGC_GetQueryUGCNumAdditionalPreviews(UGCQueryHandle_t handle, uint index)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCNumAdditionalPreviews");
        return SteamEmulator.SteamUGC.GetQueryUGCNumAdditionalPreviews(SteamEmulator.SteamUGC.MemoryAddress, handle, index);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_GetQueryUGCAdditionalPreview(UGCQueryHandle_t handle, uint index, uint previewIndex, string pchURLOrVideoID, uint cchURLSize, string pchOriginalFileName, uint cchOriginalFileNameSize, EItemPreviewType pPreviewType)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCAdditionalPreview");
        return SteamEmulator.SteamUGC.GetQueryUGCAdditionalPreview(SteamEmulator.SteamUGC.MemoryAddress, handle, index, previewIndex, pchURLOrVideoID, cchURLSize, pchOriginalFileName, cchOriginalFileNameSize, pPreviewType);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamUGC_GetQueryUGCNumKeyValueTags(UGCQueryHandle_t handle, uint index)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCNumKeyValueTags");
        return SteamEmulator.SteamUGC.GetQueryUGCNumKeyValueTags(SteamEmulator.SteamUGC.MemoryAddress, handle, index);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, string pchKey, uint cchKeySize, string pchValue, uint cchValueSize)
    {
        Write("SteamAPI_ISteamUGC_GetQueryUGCKeyValueTag");
        return SteamEmulator.SteamUGC.GetQueryUGCKeyValueTag(SteamEmulator.SteamUGC.MemoryAddress, handle, index, keyValueTagIndex, pchKey, cchKeySize, pchValue, cchValueSize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_ReleaseQueryUGCRequest(UGCQueryHandle_t handle)
    {
        Write("SteamAPI_ISteamUGC_ReleaseQueryUGCRequest");
        return SteamEmulator.SteamUGC.ReleaseQueryUGCRequest(SteamEmulator.SteamUGC.MemoryAddress, handle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_AddRequiredTag(UGCQueryHandle_t handle, string pTagName)
    {
        Write("SteamAPI_ISteamUGC_AddRequiredTag");
        return SteamEmulator.SteamUGC.AddRequiredTag(SteamEmulator.SteamUGC.MemoryAddress, handle, pTagName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_AddRequiredTagGroup(UGCQueryHandle_t handle, IntPtr pTagGroups) // match any of the tags in this group 
    {
        Write("SteamAPI_ISteamUGC_AddRequiredTagGroup");
        return SteamEmulator.SteamUGC.AddRequiredTagGroup(SteamEmulator.SteamUGC.MemoryAddress, handle, pTagGroups);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_AddExcludedTag(UGCQueryHandle_t handle, string pTagName)
    {
        Write("SteamAPI_ISteamUGC_AddExcludedTag");
        return SteamEmulator.SteamUGC.AddExcludedTag(SteamEmulator.SteamUGC.MemoryAddress, handle, pTagName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetReturnOnlyIDs(UGCQueryHandle_t handle, bool bReturnOnlyIDs)
    {
        Write("SteamAPI_ISteamUGC_SetReturnOnlyIDs");
        return SteamEmulator.SteamUGC.SetReturnOnlyIDs(SteamEmulator.SteamUGC.MemoryAddress, handle, bReturnOnlyIDs);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetReturnKeyValueTags(UGCQueryHandle_t handle, bool bReturnKeyValueTags)
    {
        Write("SteamAPI_ISteamUGC_SetReturnKeyValueTags");
        return SteamEmulator.SteamUGC.SetReturnKeyValueTags(SteamEmulator.SteamUGC.MemoryAddress, handle, bReturnKeyValueTags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetReturnLongDescription(UGCQueryHandle_t handle, bool bReturnLongDescription)
    {
        Write("SteamAPI_ISteamUGC_SetReturnLongDescription");
        return SteamEmulator.SteamUGC.SetReturnLongDescription(SteamEmulator.SteamUGC.MemoryAddress, handle, bReturnLongDescription);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetReturnMetadata(UGCQueryHandle_t handle, bool bReturnMetadata)
    {
        Write("SteamAPI_ISteamUGC_SetReturnMetadata");
        return SteamEmulator.SteamUGC.SetReturnMetadata(SteamEmulator.SteamUGC.MemoryAddress, handle, bReturnMetadata);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetReturnChildren(UGCQueryHandle_t handle, bool bReturnChildren)
    {
        Write("SteamAPI_ISteamUGC_SetReturnChildren");
        return SteamEmulator.SteamUGC.SetReturnChildren(SteamEmulator.SteamUGC.MemoryAddress, handle, bReturnChildren);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetReturnAdditionalPreviews(UGCQueryHandle_t handle, bool bReturnAdditionalPreviews)
    {
        Write("SteamAPI_ISteamUGC_SetReturnAdditionalPreviews");
        return SteamEmulator.SteamUGC.SetReturnAdditionalPreviews(SteamEmulator.SteamUGC.MemoryAddress, handle, bReturnAdditionalPreviews);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetReturnTotalOnly(UGCQueryHandle_t handle, bool bReturnTotalOnly)
    {
        Write("SteamAPI_ISteamUGC_SetReturnTotalOnly");
        return SteamEmulator.SteamUGC.SetReturnTotalOnly(SteamEmulator.SteamUGC.MemoryAddress, handle, bReturnTotalOnly);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetReturnPlaytimeStats(UGCQueryHandle_t handle, uint unDays)
    {
        Write("SteamAPI_ISteamUGC_SetReturnPlaytimeStats");
        return SteamEmulator.SteamUGC.SetReturnPlaytimeStats(SteamEmulator.SteamUGC.MemoryAddress, handle, unDays);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetLanguage(UGCQueryHandle_t handle, string pchLanguage)
    {
        Write("SteamAPI_ISteamUGC_SetLanguage");
        return SteamEmulator.SteamUGC.SetLanguage(SteamEmulator.SteamUGC.MemoryAddress, handle, pchLanguage);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetAllowCachedResponse(UGCQueryHandle_t handle, uint unMaxAgeSeconds)
    {
        Write("SteamAPI_ISteamUGC_SetAllowCachedResponse");
        return SteamEmulator.SteamUGC.SetAllowCachedResponse(SteamEmulator.SteamUGC.MemoryAddress, handle, unMaxAgeSeconds);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetCloudFileNameFilter(UGCQueryHandle_t handle, string pMatchCloudFileName)
    {
        Write("SteamAPI_ISteamUGC_SetCloudFileNameFilter");
        return SteamEmulator.SteamUGC.SetCloudFileNameFilter(SteamEmulator.SteamUGC.MemoryAddress, handle, pMatchCloudFileName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetMatchAnyTag(UGCQueryHandle_t handle, bool bMatchAnyTag)
    {
        Write("SteamAPI_ISteamUGC_SetMatchAnyTag");
        return SteamEmulator.SteamUGC.SetMatchAnyTag(SteamEmulator.SteamUGC.MemoryAddress, handle, bMatchAnyTag);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetSearchText(UGCQueryHandle_t handle, string pSearchText)
    {
        Write("SteamAPI_ISteamUGC_SetSearchText");
        return SteamEmulator.SteamUGC.SetSearchText(SteamEmulator.SteamUGC.MemoryAddress, handle, pSearchText);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetRankedByTrendDays(UGCQueryHandle_t handle, uint unDays)
    {
        Write("SteamAPI_ISteamUGC_SetRankedByTrendDays");
        return SteamEmulator.SteamUGC.SetRankedByTrendDays(SteamEmulator.SteamUGC.MemoryAddress, handle, unDays);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_AddRequiredKeyValueTag(UGCQueryHandle_t handle, string pKey, string pValue)
    {
        Write("SteamAPI_ISteamUGC_AddRequiredKeyValueTag");
        return SteamEmulator.SteamUGC.AddRequiredKeyValueTag(SteamEmulator.SteamUGC.MemoryAddress, handle, pKey, pValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamUGC_RequestUGCDetails(PublishedFileId_t nPublishedFileID, uint unMaxAgeSeconds)
    {
        Write("SteamAPI_ISteamUGC_RequestUGCDetails");
        return SteamEmulator.SteamUGC.RequestUGCDetails(SteamEmulator.SteamUGC.MemoryAddress, nPublishedFileID, unMaxAgeSeconds);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamUGC_CreateItem(AppId_t nConsumerAppId, EWorkshopFileType eFileType) // create new item for this app with no content attached yet 
    {
        Write("SteamAPI_ISteamUGC_CreateItem");
        return SteamEmulator.SteamUGC.CreateItem(SteamEmulator.SteamUGC.MemoryAddress, nConsumerAppId, eFileType);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static UGCUpdateHandle_t SteamAPI_ISteamUGC_StartItemUpdate(AppId_t nConsumerAppId, PublishedFileId_t nPublishedFileID) // start an UGC item update. Set changed properties before commiting update with CommitItemUpdate() 
    {
        Write("SteamAPI_ISteamUGC_StartItemUpdate");
        return SteamEmulator.SteamUGC.StartItemUpdate(SteamEmulator.SteamUGC.MemoryAddress, nConsumerAppId, nPublishedFileID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetItemTitle(UGCUpdateHandle_t handle, string pchTitle) // change the title of an UGC item 
    {
        Write("SteamAPI_ISteamUGC_SetItemTitle");
        return SteamEmulator.SteamUGC.SetItemTitle(SteamEmulator.SteamUGC.MemoryAddress, handle, pchTitle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamUGC_SetItemDescription(UGCUpdateHandle_t handle, string pchDescription) // change the description of an UGC item 
    {
        Write("SteamAPI_ISteamUGC_SetItemDescription");
        return SteamEmulator.SteamUGC.SetItemDescription(SteamEmulator.SteamUGC.MemoryAddress, handle, pchDescription);
    }

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_SetItemUpdateLanguage(UGCUpdateHandle_t handle, string pchLanguage) // specify the language of the title or description that will be set 
    //{
    //    Write("SteamAPI_ISteamUGC_SetItemUpdateLanguage");
    //    return SteamEmulator.SteamUGC.SetItemUpdateLanguage(handle, pchLanguage);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_SetItemMetadata(UGCUpdateHandle_t handle, string pchMetaData) // change the metadata of an UGC item (max = k_cchDeveloperMetadataMax) 
    //{
    //    Write("SteamAPI_ISteamUGC_SetItemMetadata");
    //    return SteamEmulator.SteamUGC.SetItemMetadata(handle, pchMetaData);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_SetItemVisibility(UGCUpdateHandle_t handle, ERemoteStoragePublishedFileVisibility eVisibility) // change the visibility of an UGC item 
    //{
    //    Write("SteamAPI_ISteamUGC_SetItemVisibility");
    //    return SteamEmulator.SteamUGC.SetItemVisibility(handle, eVisibility);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_SetItemTags(UGCUpdateHandle_t updateHandle, IntPtr pTags) // change the tags of an UGC item 
    //{
    //    Write("SteamAPI_ISteamUGC_SetItemTags");
    //    return SteamEmulator.SteamUGC.SetItemTags(updateHandle, pTags);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_SetItemContent(UGCUpdateHandle_t handle, string pszContentFolder) // update item content from this local folder 
    //{
    //    Write("SteamAPI_ISteamUGC_SetItemContent");
    //    return SteamEmulator.SteamUGC.SetItemContent(handle, pszContentFolder);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_SetItemPreview(UGCUpdateHandle_t handle, string pszPreviewFile) //  change preview image file for this item. pszPreviewFile points to local image file, which must be under 1MB in size 
    //{
    //    Write("SteamAPI_ISteamUGC_SetItemPreview");
    //    return SteamEmulator.SteamUGC.SetItemPreview(handle, pszPreviewFile);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_SetAllowLegacyUpload(UGCUpdateHandle_t handle, bool bAllowLegacyUpload) //  use legacy upload for a single small file. The parameter to SetItemContent() should either be a directory with one file or the full path to the file.  The file must also be less than 10MB in size. 
    //{
    //    Write("SteamAPI_ISteamUGC_SetAllowLegacyUpload");
    //    return SteamEmulator.SteamUGC.SetAllowLegacyUpload(handle, bAllowLegacyUpload);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_RemoveAllItemKeyValueTags(UGCUpdateHandle_t handle) // remove all existing key-value tags (you can add new ones via the AddItemKeyValueTag function) 
    //{
    //    Write("SteamAPI_ISteamUGC_RemoveAllItemKeyValueTags");
    //    return SteamEmulator.SteamUGC.RemoveAllItemKeyValueTags(handle);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_RemoveItemKeyValueTags(UGCUpdateHandle_t handle, string pchKey) // remove any existing key-value tags with the specified key 
    //{
    //    Write("SteamAPI_ISteamUGC_RemoveItemKeyValueTags");
    //    return SteamEmulator.SteamUGC.RemoveItemKeyValueTags(handle, pchKey);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_AddItemKeyValueTag(UGCUpdateHandle_t handle, string pchKey, string pchValue) // add new key-value tags for the item. Note that there can be multiple values for a tag. 
    //{
    //    Write("SteamAPI_ISteamUGC_AddItemKeyValueTag");
    //    return SteamEmulator.SteamUGC.AddItemKeyValueTag(handle, pchKey, pchValue);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_AddItemPreviewFile(UGCUpdateHandle_t handle, string pszPreviewFile, EItemPreviewType type) //  add preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size 
    //{
    //    Write("SteamAPI_ISteamUGC_AddItemPreviewFile");
    //    return SteamEmulator.SteamUGC.AddItemPreviewFile(handle, pszPreviewFile, type);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_AddItemPreviewVideo(UGCUpdateHandle_t handle, string pszVideoID) //  add preview video for this item 
    //{
    //    Write("SteamAPI_ISteamUGC_AddItemPreviewVideo");
    //    return SteamEmulator.SteamUGC.AddItemPreviewVideo(handle, pszVideoID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_UpdateItemPreviewFile(UGCUpdateHandle_t handle, uint index, string pszPreviewFile) //  updates an existing preview file for this item. pszPreviewFile points to local file, which must be under 1MB in size 
    //{
    //    Write("SteamAPI_ISteamUGC_UpdateItemPreviewFile");
    //    return SteamEmulator.SteamUGC.UpdateItemPreviewFile(handle, index, pszPreviewFile);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_UpdateItemPreviewVideo(UGCUpdateHandle_t handle, uint index, string pszVideoID) //  updates an existing preview video for this item 
    //{
    //    Write("SteamAPI_ISteamUGC_UpdateItemPreviewVideo");
    //    return SteamEmulator.SteamUGC.UpdateItemPreviewVideo(handle, index, pszVideoID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_RemoveItemPreview(UGCUpdateHandle_t handle, uint index) // remove a preview by index starting at 0 (previews are sorted) 
    //{
    //    Write("SteamAPI_ISteamUGC_RemoveItemPreview");
    //    return SteamEmulator.SteamUGC.RemoveItemPreview(handle, index);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_SubmitItemUpdate(UGCUpdateHandle_t handle, string pchChangeNote) // commit update process started with StartItemUpdate() 
    //{
    //    Write("SteamAPI_ISteamUGC_SubmitItemUpdate");
    //    return SteamEmulator.SteamUGC.SubmitItemUpdate(handle, pchChangeNote);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static int SteamAPI_ISteamUGC_GetItemUpdateProgress(UGCUpdateHandle_t handle, uint punBytesProcessed, uint punBytesTotal)
    //{
    //    Write("SteamAPI_ISteamUGC_GetItemUpdateProgress");
    //    return SteamEmulator.SteamUGC.GetItemUpdateProgress(handle, punBytesProcessed, punBytesTotal);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_SetUserItemVote(PublishedFileId_t nPublishedFileID, bool bVoteUp)
    //{
    //    Write("SteamAPI_ISteamUGC_SetUserItemVote");
    //    return SteamEmulator.SteamUGC.SetUserItemVote(nPublishedFileID, bVoteUp);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_GetUserItemVote(PublishedFileId_t nPublishedFileID)
    //{
    //    Write("SteamAPI_ISteamUGC_GetUserItemVote");
    //    return SteamEmulator.SteamUGC.GetUserItemVote(nPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_AddItemToFavorites(AppId_t nAppId, PublishedFileId_t nPublishedFileID)
    //{
    //    Write("SteamAPI_ISteamUGC_AddItemToFavorites");
    //    return SteamEmulator.SteamUGC.AddItemToFavorites(nAppId, nPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_RemoveItemFromFavorites(AppId_t nAppId, PublishedFileId_t nPublishedFileID)
    //{
    //    Write("SteamAPI_ISteamUGC_RemoveItemFromFavorites");
    //    return SteamEmulator.SteamUGC.RemoveItemFromFavorites(nAppId, nPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_SubscribeItem(PublishedFileId_t nPublishedFileID) // subscribe to this item, will be installed ASAP 
    //{
    //    Write("SteamAPI_ISteamUGC_SubscribeItem");
    //    return SteamEmulator.SteamUGC.SubscribeItem(nPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_UnsubscribeItem(PublishedFileId_t nPublishedFileID) // unsubscribe from this item, will be uninstalled after game quits 
    //{
    //    Write("SteamAPI_ISteamUGC_UnsubscribeItem");
    //    return SteamEmulator.SteamUGC.UnsubscribeItem(nPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static uint SteamAPI_ISteamUGC_GetNumSubscribedItems() // number of subscribed items  
    //{
    //    Write("SteamAPI_ISteamUGC_GetNumSubscribedItems");
    //    return SteamEmulator.SteamUGC.GetNumSubscribedItems();
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static uint SteamAPI_ISteamUGC_GetSubscribedItems(PublishedFileId_t pvecPublishedFileID, uint cMaxEntries) // all subscribed item PublishFileIDs 
    //{
    //    Write("SteamAPI_ISteamUGC_GetSubscribedItems");
    //    return SteamEmulator.SteamUGC.GetSubscribedItems(pvecPublishedFileID, cMaxEntries);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static uint SteamAPI_ISteamUGC_GetItemState(PublishedFileId_t nPublishedFileID)
    //{
    //    Write("SteamAPI_ISteamUGC_GetItemState");
    //    return SteamEmulator.SteamUGC.GetItemState(nPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_GetItemInstallInfo(PublishedFileId_t nPublishedFileID, uint punSizeOnDisk, string pchFolder, uint cchFolderSize, uint punTimeStamp)
    //{
    //    Write("SteamAPI_ISteamUGC_GetItemInstallInfo");
    //    return SteamEmulator.SteamUGC.GetItemInstallInfo(nPublishedFileID, punSizeOnDisk, pchFolder, cchFolderSize, punTimeStamp);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_GetItemDownloadInfo(PublishedFileId_t nPublishedFileID, uint punBytesDownloaded, uint punBytesTotal)
    //{
    //    Write("SteamAPI_ISteamUGC_GetItemDownloadInfo");
    //    return SteamEmulator.SteamUGC.GetItemDownloadInfo(nPublishedFileID, punBytesDownloaded, punBytesTotal);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_DownloadItem(PublishedFileId_t nPublishedFileID, bool bHighPriority)
    //{
    //    Write("SteamAPI_ISteamUGC_DownloadItem");
    //    return SteamEmulator.SteamUGC.DownloadItem(nPublishedFileID, bHighPriority);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static bool SteamAPI_ISteamUGC_BInitWorkshopForGameServer(uint unWorkshopDepotID, string pszFolder)
    //{
    //    Write("SteamAPI_ISteamUGC_BInitWorkshopForGameServer");
    //    return SteamEmulator.SteamUGC.BInitWorkshopForGameServer(unWorkshopDepotID, pszFolder);
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
    //    return SteamEmulator.SteamUGC.StartPlaytimeTracking(pvecPublishedFileID, unNumPublishedFileIDs);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_StopPlaytimeTracking(PublishedFileId_t pvecPublishedFileID, uint unNumPublishedFileIDs)
    //{
    //    Write("SteamAPI_ISteamUGC_StopPlaytimeTracking");
    //    return SteamEmulator.SteamUGC.StopPlaytimeTracking(pvecPublishedFileID, unNumPublishedFileIDs);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_StopPlaytimeTrackingForAllItems()
    //{
    //    Write("SteamAPI_ISteamUGC_StopPlaytimeTrackingForAllItems");
    //    return SteamEmulator.SteamUGC.StopPlaytimeTrackingForAllItems();
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_AddDependency(PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID)
    //{
    //    Write("SteamAPI_ISteamUGC_AddDependency");
    //    return SteamEmulator.SteamUGC.AddDependency(nParentPublishedFileID, nChildPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_RemoveDependency(PublishedFileId_t nParentPublishedFileID, PublishedFileId_t nChildPublishedFileID)
    //{
    //    Write("SteamAPI_ISteamUGC_RemoveDependency");
    //    return SteamEmulator.SteamUGC.RemoveDependency(nParentPublishedFileID, nChildPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_AddAppDependency(PublishedFileId_t nPublishedFileID, AppId_t nAppID)
    //{
    //    Write("SteamAPI_ISteamUGC_AddAppDependency");
    //    return SteamEmulator.SteamUGC.AddAppDependency(nPublishedFileID, nAppID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_RemoveAppDependency(PublishedFileId_t nPublishedFileID, AppId_t nAppID)
    //{
    //    Write("SteamAPI_ISteamUGC_RemoveAppDependency");
    //    return SteamEmulator.SteamUGC.RemoveAppDependency(nPublishedFileID, nAppID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_GetAppDependencies(PublishedFileId_t nPublishedFileID)
    //{
    //    Write("SteamAPI_ISteamUGC_GetAppDependencies");
    //    return SteamEmulator.SteamUGC.GetAppDependencies(nPublishedFileID);
    //}

    //[DllExport(CallingConvention = CallingConvention.Cdecl)]
    //public static SteamAPICall_t SteamAPI_ISteamUGC_DeleteItem(PublishedFileId_t nPublishedFileID)
    //{
    //    Write("SteamAPI_ISteamUGC_DeleteItem");
    //    return SteamEmulator.SteamUGC.DeleteItem(nPublishedFileID);
    //}

}
