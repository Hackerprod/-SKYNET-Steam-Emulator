using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Steamworks;
using Steamworks;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamInventory : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_AddPromoItem(ref SteamInventoryResult_t pResultHandle, uint itemDef)
        {
            Write("SteamAPI_ISteamInventory_AddPromoItem");
            return SteamEmulator.SteamInventory.AddPromoItem(ref pResultHandle, itemDef);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_AddPromoItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayItemDefs, uint unArrayLength)
        {
            Write("SteamAPI_ISteamInventory_AddPromoItems");
            return SteamEmulator.SteamInventory.AddPromoItems(ref pResultHandle, pArrayItemDefs, unArrayLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_CheckResultSteamID(SteamInventoryResult_t resultHandle, IntPtr steamIDExpected)
        {
            Write("SteamAPI_ISteamInventory_CheckResultSteamID");
            return SteamEmulator.SteamInventory.CheckResultSteamID(resultHandle, steamIDExpected);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_ConsumeItem(ref SteamInventoryResult_t pResultHandle, uint itemConsume, uint unQuantity)
        {
            Write("SteamAPI_ISteamInventory_ConsumeItem");
            return SteamEmulator.SteamInventory.ConsumeItem(ref pResultHandle, itemConsume, unQuantity);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_DeserializeResult(ref SteamInventoryResult_t pOutResultHandle, IntPtr pBuffer, uint unBufferSize, [MarshalAs(UnmanagedType.U1)] bool bRESERVED_MUST_BE_FALSE)
        {
            Write("SteamAPI_ISteamInventory_DeserializeResult");
            return SteamEmulator.SteamInventory.DeserializeResult(ref pOutResultHandle, pBuffer, unBufferSize, bRESERVED_MUST_BE_FALSE);
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
            return SteamEmulator.SteamInventory.ExchangeItems(ref pResultHandle, pArrayGenerate, punArrayGenerateQuantity, unArrayGenerateLength, pArrayDestroy, punArrayDestroyQuantity, unArrayDestroyLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GenerateItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayItemDefs, [In, Out] uint[] punArrayQuantity, uint unArrayLength)
        {
            Write("SteamAPI_ISteamInventory_GenerateItems");
            return SteamEmulator.SteamInventory.GenerateItems(ref pResultHandle, pArrayItemDefs, punArrayQuantity, unArrayLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetAllItems(ref SteamInventoryResult_t pResultHandle)
        {
            Write("SteamAPI_ISteamInventory_GetAllItems");
            return SteamEmulator.SteamInventory.GetAllItems(ref pResultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetEligiblePromoItemDefinitionIDs(IntPtr steamID, [In, Out] uint[] pItemDefIDs, ref uint punItemDefIDsArraySize)
        {
            Write("SteamAPI_ISteamInventory_GetEligiblePromoItemDefinitionIDs");
            return SteamEmulator.SteamInventory.GetEligiblePromoItemDefinitionIDs(steamID, pItemDefIDs, ref punItemDefIDsArraySize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetItemDefinitionIDs([In, Out] uint[] pItemDefIDs, ref uint punItemDefIDsArraySize)
        {
            Write("SteamAPI_ISteamInventory_GetItemDefinitionIDs");
            return SteamEmulator.SteamInventory.GetItemDefinitionIDs(pItemDefIDs, ref punItemDefIDsArraySize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetItemDefinitionProperty(uint iDefinition, string pchPropertyName, IntPtr pchValueBuffer, ref uint punValueBufferSizeOut)
        {
            Write("SteamAPI_ISteamInventory_GetItemDefinitionProperty");
            return SteamEmulator.SteamInventory.GetItemDefinitionProperty(iDefinition, pchPropertyName, pchValueBuffer, ref punValueBufferSizeOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetItemPrice(uint iDefinition, ref ulong pCurrentPrice, ref ulong pBasePrice)
        {
            Write("SteamAPI_ISteamInventory_GetItemPrice");
            return SteamEmulator.SteamInventory.GetItemPrice(iDefinition, ref pCurrentPrice, ref pBasePrice);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetItemsByID(ref SteamInventoryResult_t pResultHandle, ref uint pInstanceIDs, uint unCountInstanceIDs)
        {
            Write("SteamAPI_ISteamInventory_GetItemsByID");
            return SteamEmulator.SteamInventory.GetItemsByID(ref pResultHandle, ref pInstanceIDs, unCountInstanceIDs);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetItemsWithPrices([In, Out] uint[] pArrayItemDefs, [In, Out] ulong[] pCurrentPrices, [In, Out] ulong[] pBasePrices, uint unArrayLength)
        {
            Write("SteamAPI_ISteamInventory_GetItemsWithPrices");
            return SteamEmulator.SteamInventory.GetItemsWithPrices(pArrayItemDefs, pCurrentPrices, pBasePrices, unArrayLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamInventory_GetNumItemsWithPrices(IntPtr self)
        {
            Write("SteamAPI_ISteamInventory_GetNumItemsWithPrices");
            return SteamEmulator.SteamInventory.GetNumItemsWithPrices(self);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetResultItemProperty(SteamInventoryResult_t resultHandle, uint unItemIndex, string pchPropertyName, IntPtr pchValueBuffer, ref uint punValueBufferSizeOut)
        {
            Write("SteamAPI_ISteamInventory_GetResultItemProperty");
            return SteamEmulator.SteamInventory.GetResultItemProperty(resultHandle, unItemIndex, pchPropertyName, pchValueBuffer, ref punValueBufferSizeOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetResultItems(SteamInventoryResult_t resultHandle, [In, Out] SteamItemDetails_t[] pOutItemsArray, ref uint punOutItemsArraySize)
        {
            Write("SteamAPI_ISteamInventory_GetResultItems");
            return SteamEmulator.SteamInventory.GetResultItems(resultHandle, pOutItemsArray, ref punOutItemsArraySize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static Result SteamAPI_ISteamInventory_GetResultStatus(SteamInventoryResult_t resultHandle)
        {
            Write("SteamAPI_ISteamInventory_GetResultStatus");
            return SteamEmulator.SteamInventory.GetResultStatus(resultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamInventory_GetResultTimestamp(SteamInventoryResult_t resultHandle)
        {
            Write("SteamAPI_ISteamInventory_GetResultTimestamp");
            return SteamEmulator.SteamInventory.GetResultTimestamp(resultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GrantPromoItems(ref SteamInventoryResult_t pResultHandle)
        {
            Write("SteamAPI_ISteamInventory_GrantPromoItems");
            return SteamEmulator.SteamInventory.GrantPromoItems(ref pResultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_InspectItem(ref SteamInventoryResult_t pResultHandle, string pchItemToken)
        {
            Write("SteamAPI_ISteamInventory_InspectItem");
            return SteamEmulator.SteamInventory.InspectItem(ref pResultHandle, pchItemToken);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_LoadItemDefinitions(IntPtr self)
        {
            Write("SteamAPI_ISteamInventory_LoadItemDefinitions");
            return SteamEmulator.SteamInventory.LoadItemDefinitions(self);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_RemoveProperty(IntPtr handle, uint nItemID, string pchPropertyName)
        {
            Write("SteamAPI_ISteamInventory_RemoveProperty");
            return SteamEmulator.SteamInventory.RemoveProperty(handle, nItemID, pchPropertyName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamInventory_RequestEligiblePromoItemDefinitionsIDs(IntPtr steamID)
        {
            Write("SteamAPI_ISteamInventory_RequestEligiblePromoItemDefinitionsIDs");
            return SteamEmulator.SteamInventory.RequestEligiblePromoItemDefinitionsIDs(steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamInventory_RequestPrices(IntPtr self)
        {
            Write("SteamAPI_ISteamInventory_RequestPrices");
            return SteamEmulator.SteamInventory.RequestPrices(self);
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
            return SteamEmulator.SteamInventory.SerializeResult(resultHandle, pOutBuffer, ref punOutBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_SetPropertyBool(IntPtr handle, uint nItemID, string pchPropertyName, [MarshalAs(UnmanagedType.U1)] bool bValue)
        {
            Write("SteamAPI_ISteamInventory_SetPropertyBool");
            return SteamEmulator.SteamInventory.SetPropertyBool(handle, nItemID, pchPropertyName, bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_SetPropertyFloat(IntPtr handle, uint nItemID, string pchPropertyName, float flValue)
        {
            Write("SteamAPI_ISteamInventory_SetPropertyFloat");
            return SteamEmulator.SteamInventory.SetPropertyFloat(handle, nItemID, pchPropertyName, flValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_SetPropertyInt64(IntPtr handle, uint nItemID, string pchPropertyName, long nValue)
        {
            Write("SteamAPI_ISteamInventory_SetPropertyInt64");
            return SteamEmulator.SteamInventory.SetPropertyInt64(handle, nItemID, pchPropertyName, nValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_SetPropertyString(IntPtr handle, uint nItemID, string pchPropertyName, string pchPropertyValue)
        {
            Write("SteamAPI_ISteamInventory_SetPropertyString");
            return SteamEmulator.SteamInventory.SetPropertyString(handle, nItemID, pchPropertyName, pchPropertyValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamInventory_StartPurchase([In, Out] uint[] pArrayItemDefs, [In, Out] uint[] punArrayQuantity, uint unArrayLength)
        {
            Write("SteamAPI_ISteamInventory_StartPurchase");
            return SteamEmulator.SteamInventory.StartPurchase(pArrayItemDefs, punArrayQuantity, unArrayLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamInventory_StartUpdateProperties(IntPtr _)
        {
            Write("SteamAPI_ISteamInventory_StartUpdateProperties");
            return SteamEmulator.SteamInventory.StartUpdateProperties(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_SubmitUpdateProperties(IntPtr handle, ref SteamInventoryResult_t pResultHandle)
        {
            Write("SteamAPI_ISteamInventory_SubmitUpdateProperties");
            return SteamEmulator.SteamInventory.SubmitUpdateProperties(handle, ref pResultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_TradeItems(ref SteamInventoryResult_t pResultHandle, IntPtr steamIDTradePartner, [In, Out] uint[] pArrayGive, [In, Out] uint[] pArrayGiveQuantity, uint nArrayGiveLength, [In, Out] uint[] pArrayGet, [In, Out] uint[] pArrayGetQuantity, uint nArrayGetLength)
        {
            Write("SteamAPI_ISteamInventory_TradeItems");
            return SteamEmulator.SteamInventory.TradeItems(ref pResultHandle, steamIDTradePartner, pArrayGive, pArrayGiveQuantity, nArrayGiveLength, pArrayGet, pArrayGetQuantity, nArrayGetLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_TransferItemQuantity(ref SteamInventoryResult_t pResultHandle, uint itemIdSource, uint unQuantity, uint itemIdDest)
        {
            Write("SteamAPI_ISteamInventory_TransferItemQuantity");
            return SteamEmulator.SteamInventory.TransferItemQuantity(ref pResultHandle, itemIdSource, unQuantity, itemIdDest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_TriggerItemDrop(ref SteamInventoryResult_t pResultHandle, uint dropListDefinition)
        {
            Write("SteamAPI_ISteamInventory_TriggerItemDrop");
            return SteamEmulator.SteamInventory.TriggerItemDrop(ref pResultHandle, dropListDefinition);
        }
    }
}
