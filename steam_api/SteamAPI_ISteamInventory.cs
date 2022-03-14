using System;
using System.Runtime.InteropServices;
using SKYNET.Interface;
using Steamworks;

public class SteamAPI_ISteamInventory : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_AddPromoItem(ref SteamInventoryResult_t pResultHandle, uint itemDef)
    {
        Write("SteamAPI_ISteamInventory_AddPromoItem");
        return SteamClient.SteamInventory.AddPromoItem(ref pResultHandle, itemDef);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_AddPromoItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayItemDefs, uint unArrayLength)
    {
        Write("SteamAPI_ISteamInventory_AddPromoItems");
        return SteamClient.SteamInventory.AddPromoItems(ref pResultHandle, pArrayItemDefs, unArrayLength);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_CheckResultSteamID(SteamInventoryResult_t resultHandle, IntPtr steamIDExpected)
    {
        Write("SteamAPI_ISteamInventory_CheckResultSteamID");
        return SteamClient.SteamInventory.CheckResultSteamID(resultHandle, steamIDExpected);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_ConsumeItem(ref SteamInventoryResult_t pResultHandle, uint itemConsume, uint unQuantity)
    {
        Write("SteamAPI_ISteamInventory_ConsumeItem");
        return SteamClient.SteamInventory.ConsumeItem(ref pResultHandle, itemConsume, unQuantity);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_DeserializeResult(ref SteamInventoryResult_t pOutResultHandle, IntPtr pBuffer, uint unBufferSize, [MarshalAs(UnmanagedType.U1)] bool bRESERVED_MUST_BE_FALSE)
    {
        Write("SteamAPI_ISteamInventory_DeserializeResult");
        return SteamClient.SteamInventory.DeserializeResult(ref pOutResultHandle, pBuffer, unBufferSize, bRESERVED_MUST_BE_FALSE);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamInventory_DestroyResult(SteamInventoryResult_t resultHandle)
    {
        Write("SteamAPI_ISteamInventory_DestroyResult");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_ExchangeItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayGenerate, [In, Out] uint[] punArrayGenerateQuantity, uint unArrayGenerateLength, [In, Out] uint[] pArrayDestroy, [In, Out] uint[] punArrayDestroyQuantity, uint unArrayDestroyLength)
    {
        Write("SteamAPI_ISteamInventory_ExchangeItems");
        return SteamClient.SteamInventory.ExchangeItems(ref pResultHandle, pArrayGenerate, punArrayGenerateQuantity, unArrayGenerateLength, pArrayDestroy, punArrayDestroyQuantity, unArrayDestroyLength);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_GenerateItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayItemDefs, [In, Out] uint[] punArrayQuantity, uint unArrayLength)
    {
        Write("SteamAPI_ISteamInventory_GenerateItems");
        return SteamClient.SteamInventory.GenerateItems(ref pResultHandle, pArrayItemDefs, punArrayQuantity, unArrayLength);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_GetAllItems(ref SteamInventoryResult_t pResultHandle)
    {
        Write("SteamAPI_ISteamInventory_GetAllItems");
        return SteamClient.SteamInventory.GetAllItems(ref pResultHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_GetEligiblePromoItemDefinitionIDs(IntPtr steamID, [In, Out] uint[] pItemDefIDs, ref uint punItemDefIDsArraySize)
    {
        Write("SteamAPI_ISteamInventory_GetEligiblePromoItemDefinitionIDs");
        return SteamClient.SteamInventory.GetEligiblePromoItemDefinitionIDs(steamID, pItemDefIDs, ref punItemDefIDsArraySize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_GetItemDefinitionIDs([In, Out] uint[] pItemDefIDs, ref uint punItemDefIDsArraySize)
    {
        Write("SteamAPI_ISteamInventory_GetItemDefinitionIDs");
        return SteamClient.SteamInventory.GetItemDefinitionIDs(pItemDefIDs, ref punItemDefIDsArraySize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_GetItemDefinitionProperty(uint iDefinition, string pchPropertyName, IntPtr pchValueBuffer, ref uint punValueBufferSizeOut)
    {
        Write("SteamAPI_ISteamInventory_GetItemDefinitionProperty");
        return SteamClient.SteamInventory.GetItemDefinitionProperty(iDefinition, pchPropertyName, pchValueBuffer, ref punValueBufferSizeOut);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_GetItemPrice(uint iDefinition, ref ulong pCurrentPrice, ref ulong pBasePrice)
    {
        Write("SteamAPI_ISteamInventory_GetItemPrice");
        return SteamClient.SteamInventory.GetItemPrice(iDefinition, ref pCurrentPrice, ref pBasePrice);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_GetItemsByID(ref SteamInventoryResult_t pResultHandle, ref uint pInstanceIDs, uint unCountInstanceIDs)
    {
        Write("SteamAPI_ISteamInventory_GetItemsByID");
        return SteamClient.SteamInventory.GetItemsByID(ref pResultHandle, ref pInstanceIDs, unCountInstanceIDs);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_GetItemsWithPrices([In, Out] uint[] pArrayItemDefs, [In, Out] ulong[] pCurrentPrices, [In, Out] ulong[] pBasePrices, uint unArrayLength)
    {
        Write("SteamAPI_ISteamInventory_GetItemsWithPrices");
        return SteamClient.SteamInventory.GetItemsWithPrices(pArrayItemDefs, pCurrentPrices, pBasePrices, unArrayLength);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamInventory_GetNumItemsWithPrices(IntPtr self)
    {
        Write("SteamAPI_ISteamInventory_GetNumItemsWithPrices");
        return SteamClient.SteamInventory.GetNumItemsWithPrices(self);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_GetResultItemProperty(SteamInventoryResult_t resultHandle, uint unItemIndex, string pchPropertyName, IntPtr pchValueBuffer, ref uint punValueBufferSizeOut)
    {
        Write("SteamAPI_ISteamInventory_GetResultItemProperty");
        return SteamClient.SteamInventory.GetResultItemProperty(resultHandle, unItemIndex, pchPropertyName, pchValueBuffer, ref punValueBufferSizeOut);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_GetResultItems(SteamInventoryResult_t resultHandle, [In, Out] SteamItemDetails_t[] pOutItemsArray, ref uint punOutItemsArraySize)
    {
        Write("SteamAPI_ISteamInventory_GetResultItems");
        return SteamClient.SteamInventory.GetResultItems(resultHandle, pOutItemsArray, ref punOutItemsArraySize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static Result SteamAPI_ISteamInventory_GetResultStatus(SteamInventoryResult_t resultHandle)
    {
        Write("SteamAPI_ISteamInventory_GetResultStatus");
        return SteamClient.SteamInventory.GetResultStatus(resultHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamInventory_GetResultTimestamp(SteamInventoryResult_t resultHandle)
    {
        Write("SteamAPI_ISteamInventory_GetResultTimestamp");
        return SteamClient.SteamInventory.GetResultTimestamp(resultHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_GrantPromoItems(ref SteamInventoryResult_t pResultHandle)
    {
        Write("SteamAPI_ISteamInventory_GrantPromoItems");
        return SteamClient.SteamInventory.GrantPromoItems(ref pResultHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_InspectItem(ref SteamInventoryResult_t pResultHandle, string pchItemToken)
    {
        Write("SteamAPI_ISteamInventory_InspectItem");
        return SteamClient.SteamInventory.InspectItem(ref pResultHandle, pchItemToken);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_LoadItemDefinitions(IntPtr self)
    {
        Write("SteamAPI_ISteamInventory_LoadItemDefinitions");
        return SteamClient.SteamInventory.LoadItemDefinitions(self);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_RemoveProperty(IntPtr handle, uint nItemID, string pchPropertyName)
    {
        Write("SteamAPI_ISteamInventory_RemoveProperty");
        return SteamClient.SteamInventory.RemoveProperty(handle, nItemID, pchPropertyName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamInventory_RequestEligiblePromoItemDefinitionsIDs(IntPtr steamID)
    {
        Write("SteamAPI_ISteamInventory_RequestEligiblePromoItemDefinitionsIDs");
        return SteamClient.SteamInventory.RequestEligiblePromoItemDefinitionsIDs(steamID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamInventory_RequestPrices(IntPtr self)
    {
        Write("SteamAPI_ISteamInventory_RequestPrices");
        return SteamClient.SteamInventory.RequestPrices(self);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamInventory_SendItemDropHeartbeat(IntPtr self)
    {
        Write("SteamAPI_ISteamInventory_SendItemDropHeartbeat");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_SerializeResult(SteamInventoryResult_t resultHandle, IntPtr pOutBuffer, ref uint punOutBufferSize)
    {
        Write("SteamAPI_ISteamInventory_SerializeResult");
        return SteamClient.SteamInventory.SerializeResult(resultHandle, pOutBuffer, ref punOutBufferSize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_SetPropertyBool(IntPtr handle, uint nItemID, string pchPropertyName, [MarshalAs(UnmanagedType.U1)] bool bValue)
    {
        Write("SteamAPI_ISteamInventory_SetPropertyBool");
        return SteamClient.SteamInventory.SetPropertyBool(handle, nItemID, pchPropertyName, bValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_SetPropertyFloat(IntPtr handle, uint nItemID, string pchPropertyName, float flValue)
    {
        Write("SteamAPI_ISteamInventory_SetPropertyFloat");
        return SteamClient.SteamInventory.SetPropertyFloat(handle, nItemID, pchPropertyName, flValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_SetPropertyInt64(IntPtr handle, uint nItemID, string pchPropertyName, long nValue)
    {
        Write("SteamAPI_ISteamInventory_SetPropertyInt64");
        return SteamClient.SteamInventory.SetPropertyInt64(handle, nItemID, pchPropertyName, nValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_SetPropertyString(IntPtr handle, uint nItemID, string pchPropertyName, string pchPropertyValue)
    {
        Write("SteamAPI_ISteamInventory_SetPropertyString");
        return SteamClient.SteamInventory.SetPropertyString(handle, nItemID, pchPropertyName, pchPropertyValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamInventory_StartPurchase([In, Out] uint[] pArrayItemDefs, [In, Out] uint[] punArrayQuantity, uint unArrayLength)
    {
        Write("SteamAPI_ISteamInventory_StartPurchase");
        return SteamClient.SteamInventory.StartPurchase(pArrayItemDefs, punArrayQuantity, unArrayLength);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamInventory_StartUpdateProperties()
    {
        Write("SteamAPI_ISteamInventory_StartUpdateProperties");
        return SteamClient.SteamInventory.StartUpdateProperties();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_SubmitUpdateProperties(IntPtr handle, ref SteamInventoryResult_t pResultHandle)
    {
        Write("SteamAPI_ISteamInventory_SubmitUpdateProperties");
        return SteamClient.SteamInventory.SubmitUpdateProperties(handle, ref pResultHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_TradeItems(ref SteamInventoryResult_t pResultHandle, IntPtr steamIDTradePartner, [In, Out] uint[] pArrayGive, [In, Out] uint[] pArrayGiveQuantity, uint nArrayGiveLength, [In, Out] uint[] pArrayGet, [In, Out] uint[] pArrayGetQuantity, uint nArrayGetLength)
    {
        Write("SteamAPI_ISteamInventory_TradeItems");
        return SteamClient.SteamInventory.TradeItems(ref pResultHandle, steamIDTradePartner, pArrayGive, pArrayGiveQuantity, nArrayGiveLength, pArrayGet, pArrayGetQuantity, nArrayGetLength);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_TransferItemQuantity(ref SteamInventoryResult_t pResultHandle, uint itemIdSource, uint unQuantity, uint itemIdDest)
    {
        Write("SteamAPI_ISteamInventory_TransferItemQuantity");
        return SteamClient.SteamInventory.TransferItemQuantity(ref pResultHandle, itemIdSource, unQuantity, itemIdDest);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInventory_TriggerItemDrop(ref SteamInventoryResult_t pResultHandle, uint dropListDefinition)
    {
        Write("SteamAPI_ISteamInventory_TriggerItemDrop");
        return SteamClient.SteamInventory.TriggerItemDrop(ref pResultHandle, dropListDefinition);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_SteamInventory_v003()
    {
        Write("SteamAPI_SteamInventory_v003");
        return IntPtr.Zero;
    }
}
