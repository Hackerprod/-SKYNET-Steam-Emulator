using System;
using System.Runtime.InteropServices;

using SteamInventoryResult_t = System.Int32;
using SteamItemInstanceID_t = System.UInt64;
using SteamAPICall_t = System.UInt64;
using SteamItemDef_t = System.Int32;
using SteamInventoryUpdateHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Exported
{
    // Flat native exports for ISteamInventory. Every out-pointer is IntPtr (never
    // passed by value) so 64-bit pointers are not truncated; item instance ids and
    // update handles are 64-bit.
    public class SteamAPI_ISteamInventory
    {
        static SteamAPI_ISteamInventory()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamInventory_GetResultStatus(IntPtr _, SteamInventoryResult_t resultHandle)
        {
            return SteamEmulator.SteamInventory.GetResultStatus(resultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetResultItems(IntPtr _, SteamInventoryResult_t resultHandle, IntPtr pOutItemsArray, IntPtr punOutItemsArraySize)
        {
            return SteamEmulator.SteamInventory.GetResultItems(resultHandle, pOutItemsArray, punOutItemsArraySize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetResultItemProperty(IntPtr _, SteamInventoryResult_t resultHandle, uint unItemIndex, string pchPropertyName, IntPtr pchValueBuffer, IntPtr punValueBufferSizeOut)
        {
            return SteamEmulator.SteamInventory.GetResultItemProperty(resultHandle, unItemIndex, pchPropertyName, pchValueBuffer, punValueBufferSizeOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamInventory_GetResultTimestamp(IntPtr _, SteamInventoryResult_t resultHandle)
        {
            return SteamEmulator.SteamInventory.GetResultTimestamp(resultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_CheckResultSteamID(IntPtr _, SteamInventoryResult_t resultHandle, ulong steamIDExpected)
        {
            return SteamEmulator.SteamInventory.CheckResultSteamID(resultHandle, steamIDExpected);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInventory_DestroyResult(IntPtr _, SteamInventoryResult_t resultHandle)
        {
            SteamEmulator.SteamInventory.DestroyResult(resultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetAllItems(IntPtr _, IntPtr pResultHandle)
        {
            return SteamEmulator.SteamInventory.GetAllItems(pResultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetItemsByID(IntPtr _, IntPtr pResultHandle, IntPtr pInstanceIDs, uint unCountInstanceIDs)
        {
            return SteamEmulator.SteamInventory.GetItemsByID(pResultHandle, pInstanceIDs, unCountInstanceIDs);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_SerializeResult(IntPtr _, SteamInventoryResult_t resultHandle, IntPtr pOutBuffer, IntPtr punOutBufferSize)
        {
            return SteamEmulator.SteamInventory.SerializeResult(resultHandle, pOutBuffer, punOutBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_DeserializeResult(IntPtr _, IntPtr pOutResultHandle, IntPtr pBuffer, uint unBufferSize, [MarshalAs(UnmanagedType.U1)] bool bRESERVED_MUST_BE_FALSE)
        {
            return SteamEmulator.SteamInventory.DeserializeResult(pOutResultHandle, pBuffer, unBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GenerateItems(IntPtr _, IntPtr pResultHandle, IntPtr pArrayItemDefs, IntPtr punArrayQuantity, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.GenerateItems(pResultHandle, pArrayItemDefs, punArrayQuantity, unArrayLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GrantPromoItems(IntPtr _, IntPtr pResultHandle)
        {
            return SteamEmulator.SteamInventory.GrantPromoItems(pResultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_AddPromoItem(IntPtr _, IntPtr pResultHandle, SteamItemDef_t itemDef)
        {
            return SteamEmulator.SteamInventory.AddPromoItem(pResultHandle, itemDef);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_AddPromoItems(IntPtr _, IntPtr pResultHandle, IntPtr pArrayItemDefs, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.AddPromoItems(pResultHandle, pArrayItemDefs, unArrayLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_ConsumeItem(IntPtr _, IntPtr pResultHandle, SteamItemInstanceID_t itemConsume, uint unQuantity)
        {
            return SteamEmulator.SteamInventory.ConsumeItem(pResultHandle, itemConsume, unQuantity);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_ExchangeItems(IntPtr _, IntPtr pResultHandle, IntPtr pArrayGenerate, IntPtr punArrayGenerateQuantity, uint unArrayGenerateLength, IntPtr pArrayDestroy, IntPtr punArrayDestroyQuantity, uint unArrayDestroyLength)
        {
            return SteamEmulator.SteamInventory.ExchangeItems(pResultHandle, pArrayGenerate, punArrayGenerateQuantity, unArrayGenerateLength, pArrayDestroy, punArrayDestroyQuantity, unArrayDestroyLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_TransferItemQuantity(IntPtr _, IntPtr pResultHandle, SteamItemInstanceID_t itemIdSource, uint unQuantity, SteamItemInstanceID_t itemIdDest)
        {
            return SteamEmulator.SteamInventory.TransferItemQuantity(pResultHandle, itemIdSource, unQuantity, itemIdDest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInventory_SendItemDropHeartbeat(IntPtr _)
        {
            SteamEmulator.SteamInventory.SendItemDropHeartbeat();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_TriggerItemDrop(IntPtr _, IntPtr pResultHandle, SteamItemDef_t dropListDefinition)
        {
            return SteamEmulator.SteamInventory.TriggerItemDrop(pResultHandle, dropListDefinition);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_TradeItems(IntPtr _, IntPtr pResultHandle, ulong steamIDTradePartner, IntPtr pArrayGive, IntPtr pArrayGiveQuantity, uint nArrayGiveLength, IntPtr pArrayGet, IntPtr pArrayGetQuantity, uint nArrayGetLength)
        {
            return SteamEmulator.SteamInventory.TradeItems(pResultHandle, steamIDTradePartner, pArrayGive, pArrayGiveQuantity, nArrayGiveLength, pArrayGet, pArrayGetQuantity, nArrayGetLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_LoadItemDefinitions(IntPtr _)
        {
            return SteamEmulator.SteamInventory.LoadItemDefinitions();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetItemDefinitionIDs(IntPtr _, IntPtr pItemDefIDs, IntPtr punItemDefIDsArraySize)
        {
            return SteamEmulator.SteamInventory.GetItemDefinitionIDs(pItemDefIDs, punItemDefIDsArraySize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetItemDefinitionProperty(IntPtr _, SteamItemDef_t iDefinition, string pchPropertyName, IntPtr pchValueBuffer, IntPtr punValueBufferSizeOut)
        {
            return SteamEmulator.SteamInventory.GetItemDefinitionProperty(iDefinition, pchPropertyName, pchValueBuffer, punValueBufferSizeOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetItemPrice(IntPtr _, SteamItemDef_t iDefinition, IntPtr pCurrentPrice, IntPtr pBasePrice)
        {
            return SteamEmulator.SteamInventory.GetItemPrice(iDefinition, pCurrentPrice, pBasePrice);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetItemsWithPrices(IntPtr _, IntPtr pArrayItemDefs, IntPtr pCurrentPrices, IntPtr pBasePrices, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.GetItemsWithPrices(pArrayItemDefs, pCurrentPrices, pBasePrices, unArrayLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamInventory_GetNumItemsWithPrices(IntPtr _)
        {
            return SteamEmulator.SteamInventory.GetNumItemsWithPrices();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamInventory_RequestEligiblePromoItemDefinitionsIDs(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamInventory.RequestEligiblePromoItemDefinitionsIDs(steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_GetEligiblePromoItemDefinitionIDs(IntPtr _, ulong steamID, IntPtr pItemDefIDs, IntPtr punItemDefIDsArraySize)
        {
            return SteamEmulator.SteamInventory.GetEligiblePromoItemDefinitionIDs(steamID, pItemDefIDs, punItemDefIDsArraySize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamInventory_StartPurchase(IntPtr _, IntPtr pArrayItemDefs, IntPtr punArrayQuantity, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.StartPurchase(pArrayItemDefs, punArrayQuantity, unArrayLength);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamInventory_RequestPrices(IntPtr _)
        {
            return SteamEmulator.SteamInventory.RequestPrices();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamInventoryUpdateHandle_t SteamAPI_ISteamInventory_StartUpdateProperties(IntPtr _)
        {
            return SteamEmulator.SteamInventory.StartUpdateProperties();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_RemoveProperty(IntPtr _, SteamInventoryUpdateHandle_t handle, SteamItemInstanceID_t nItemID, string pchPropertyName)
        {
            return SteamEmulator.SteamInventory.RemoveProperty(handle, nItemID, pchPropertyName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_SetPropertyString(IntPtr _, SteamInventoryUpdateHandle_t handle, SteamItemInstanceID_t nItemID, string pchPropertyName, string pchPropertyValue)
        {
            return SteamEmulator.SteamInventory.SetPropertyString(handle, nItemID, pchPropertyName, pchPropertyValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_SetPropertyBool(IntPtr _, SteamInventoryUpdateHandle_t handle, SteamItemInstanceID_t nItemID, string pchPropertyName, [MarshalAs(UnmanagedType.U1)] bool bValue)
        {
            return SteamEmulator.SteamInventory.SetPropertyBool(handle, nItemID, pchPropertyName, bValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_SetPropertyInt64(IntPtr _, SteamInventoryUpdateHandle_t handle, SteamItemInstanceID_t nItemID, string pchPropertyName, long nValue)
        {
            return SteamEmulator.SteamInventory.SetPropertyInt64(handle, nItemID, pchPropertyName, nValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_SetPropertyFloat(IntPtr _, SteamInventoryUpdateHandle_t handle, SteamItemInstanceID_t nItemID, string pchPropertyName, float flValue)
        {
            return SteamEmulator.SteamInventory.SetPropertyFloat(handle, nItemID, pchPropertyName, flValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_SubmitUpdateProperties(IntPtr _, SteamInventoryUpdateHandle_t handle, IntPtr pResultHandle)
        {
            return SteamEmulator.SteamInventory.SubmitUpdateProperties(handle, pResultHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInventory_InspectItem(IntPtr _, IntPtr pResultHandle, string pchItemToken)
        {
            return SteamEmulator.SteamInventory.InspectItem(pResultHandle, pchItemToken);
        }
    }
}
