//using System;
//using System.Runtime.InteropServices;
//using SKYNET;
//using SKYNET.Steamworks;
//using Steamworks;

//using SteamInventoryResult_t = System.UInt32;
//using SteamItemInstanceID_t = System.UInt64;
//using SteamAPICall_t = System.UInt64;

//namespace SKYNET.Steamworks.Exported
//{
//    public class SteamAPI_ISteamInventory 
//    {
//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_AddPromoItem(uint pResultHandle, uint itemDef)
//        {
//            Write("SteamAPI_ISteamInventory_AddPromoItem");
//            return SteamEmulator.SteamInventory.AddPromoItem(pResultHandle, itemDef);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_AddPromoItems(SteamInventoryResult_t pResultHandle, IntPtr pArrayItemDefs, uint unArrayLength)
//        {
//            Write("SteamAPI_ISteamInventory_AddPromoItems");
//            return SteamEmulator.SteamInventory.AddPromoItems(pResultHandle, pArrayItemDefs, unArrayLength);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_CheckResultSteamID(SteamInventoryResult_t resultHandle, ulong steamIDExpected)
//        {
//            Write("SteamAPI_ISteamInventory_CheckResultSteamID");
//            return SteamEmulator.SteamInventory.CheckResultSteamID(resultHandle, steamIDExpected);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_ConsumeItem(SteamInventoryResult_t pResultHandle, uint itemConsume, uint unQuantity)
//        {
//            Write("SteamAPI_ISteamInventory_ConsumeItem");
//            return SteamEmulator.SteamInventory.ConsumeItem(pResultHandle, itemConsume, unQuantity);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_DeserializeResult(SteamInventoryResult_t pOutResultHandle, IntPtr pBuffer, uint unBufferSize, [MarshalAs(UnmanagedType.U1)] bool bRESERVED_MUST_BE_FALSE)
//        {
//            Write("SteamAPI_ISteamInventory_DeserializeResult");
//            return SteamEmulator.SteamInventory.DeserializeResult(pOutResultHandle, pBuffer, unBufferSize, bRESERVED_MUST_BE_FALSE);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static void SteamAPI_ISteamInventory_DestroyResult(SteamInventoryResult_t resultHandle)
//        {
//            Write("SteamAPI_ISteamInventory_DestroyResult");
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_ExchangeItems(SteamInventoryResult_t pResultHandle, IntPtr pArrayGenerate, IntPtr punArrayGenerateQuantity, uint unArrayGenerateLength, ref SteamItemInstanceID_t[] pArrayDestroy, uint punArrayDestroyQuantity, uint unArrayDestroyLength)
//        {
//            Write("SteamAPI_ISteamInventory_ExchangeItems");
//            return SteamEmulator.SteamInventory.ExchangeItems(pResultHandle, pArrayGenerate, punArrayGenerateQuantity, unArrayGenerateLength, ref pArrayDestroy, punArrayDestroyQuantity, unArrayDestroyLength);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_GenerateItems(SteamInventoryResult_t pResultHandle, IntPtr pArrayItemDefs, IntPtr punArrayQuantity, uint unArrayLength)
//        {
//            Write("SteamAPI_ISteamInventory_GenerateItems");
//            return SteamEmulator.SteamInventory.GenerateItems(pResultHandle, pArrayItemDefs, punArrayQuantity, unArrayLength);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_GetAllItems(SteamInventoryResult_t pResultHandle)
//        {
//            Write("SteamAPI_ISteamInventory_GetAllItems");
//            return SteamEmulator.SteamInventory.GetAllItems(pResultHandle);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_GetEligiblePromoItemDefinitionIDs(ulong steamID, IntPtr pItemDefIDs, uint punItemDefIDsArraySize)
//        {
//            Write("SteamAPI_ISteamInventory_GetEligiblePromoItemDefinitionIDs");
//            return SteamEmulator.SteamInventory.GetEligiblePromoItemDefinitionIDs(steamID, pItemDefIDs, punItemDefIDsArraySize);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_GetItemDefinitionIDs(IntPtr pItemDefIDs, uint punItemDefIDsArraySize)
//        {
//            Write("SteamAPI_ISteamInventory_GetItemDefinitionIDs");
//            return SteamEmulator.SteamInventory.GetItemDefinitionIDs(pItemDefIDs, punItemDefIDsArraySize);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_GetItemDefinitionProperty(uint iDefinition, string pchPropertyName, IntPtr pchValueBuffer, uint punValueBufferSizeOut)
//        {
//            Write("SteamAPI_ISteamInventory_GetItemDefinitionProperty");
//            return SteamEmulator.SteamInventory.GetItemDefinitionProperty(iDefinition, pchPropertyName, pchValueBuffer, punValueBufferSizeOut);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_GetItemPrice(uint iDefinition, ulong pCurrentPrice, ulong pBasePrice)
//        {
//            Write("SteamAPI_ISteamInventory_GetItemPrice");
//            return SteamEmulator.SteamInventory.GetItemPrice(iDefinition, pCurrentPrice, pBasePrice);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_GetItemsByID(SteamInventoryResult_t pResultHandle, ref SteamItemInstanceID_t pInstanceIDs, uint unCountInstanceIDs)
//        {
//            Write("SteamAPI_ISteamInventory_GetItemsByID");
//            return SteamEmulator.SteamInventory.GetItemsByID(pResultHandle, ref pInstanceIDs, unCountInstanceIDs);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_GetItemsWithPrices(IntPtr pArrayItemDefs, IntPtr pCurrentPrices, ulong pBasePrices, uint unArrayLength)
//        {
//            Write("SteamAPI_ISteamInventory_GetItemsWithPrices");
//            return SteamEmulator.SteamInventory.GetItemsWithPrices(pArrayItemDefs, pCurrentPrices, pBasePrices, unArrayLength);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static uint SteamAPI_ISteamInventory_GetNumItemsWithPrices(IntPtr self)
//        {
//            Write("SteamAPI_ISteamInventory_GetNumItemsWithPrices");
//            return SteamEmulator.SteamInventory.GetNumItemsWithPrices(self);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_GetResultItemProperty(SteamInventoryResult_t resultHandle, uint unItemIndex, string pchPropertyName, IntPtr pchValueBuffer, uint punValueBufferSizeOut)
//        {
//            Write("SteamAPI_ISteamInventory_GetResultItemProperty");
//            return SteamEmulator.SteamInventory.GetResultItemProperty(resultHandle, unItemIndex, pchPropertyName, pchValueBuffer, punValueBufferSizeOut);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_GetResultItems(SteamInventoryResult_t resultHandle, IntPtr pOutItemsArray, uint punOutItemsArraySize)
//        {
//            Write("SteamAPI_ISteamInventory_GetResultItems");
//            return SteamEmulator.SteamInventory.GetResultItems(resultHandle, pOutItemsArray, punOutItemsArraySize);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static int SteamAPI_ISteamInventory_GetResultStatus(SteamInventoryResult_t resultHandle)
//        {
//            Write("SteamAPI_ISteamInventory_GetResultStatus");
//            return SteamEmulator.SteamInventory.GetResultStatus(resultHandle);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static uint SteamAPI_ISteamInventory_GetResultTimestamp(SteamInventoryResult_t resultHandle)
//        {
//            Write("SteamAPI_ISteamInventory_GetResultTimestamp");
//            return SteamEmulator.SteamInventory.GetResultTimestamp(resultHandle);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_GrantPromoItems(SteamInventoryResult_t pResultHandle)
//        {
//            Write("SteamAPI_ISteamInventory_GrantPromoItems");
//            return SteamEmulator.SteamInventory.GrantPromoItems(pResultHandle);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_InspectItem(SteamInventoryResult_t pResultHandle, string pchItemToken)
//        {
//            Write("SteamAPI_ISteamInventory_InspectItem");
//            return SteamEmulator.SteamInventory.InspectItem(pResultHandle, pchItemToken);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_LoadItemDefinitions(IntPtr self)
//        {
//            Write("SteamAPI_ISteamInventory_LoadItemDefinitions");
//            return SteamEmulator.SteamInventory.LoadItemDefinitions(self);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_RemoveProperty(ulong handle, uint nItemID, string pchPropertyName)
//        {
//            Write("SteamAPI_ISteamInventory_RemoveProperty");
//            return SteamEmulator.SteamInventory.RemoveProperty(handle, nItemID, pchPropertyName);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static SteamAPICall_t SteamAPI_ISteamInventory_RequestEligiblePromoItemDefinitionsIDs(ulong steamID)
//        {
//            Write("SteamAPI_ISteamInventory_RequestEligiblePromoItemDefinitionsIDs");
//            return SteamEmulator.SteamInventory.RequestEligiblePromoItemDefinitionsIDs(steamID);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static SteamAPICall_t SteamAPI_ISteamInventory_RequestPrices(IntPtr self)
//        {
//            Write("SteamAPI_ISteamInventory_RequestPrices");
//            return SteamEmulator.SteamInventory.RequestPrices(self);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static void SteamAPI_ISteamInventory_SendItemDropHeartbeat(IntPtr self)
//        {
//            Write("SteamAPI_ISteamInventory_SendItemDropHeartbeat");
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_SerializeResult(SteamInventoryResult_t resultHandle, IntPtr pOutBuffer, uint punOutBufferSize)
//        {
//            Write("SteamAPI_ISteamInventory_SerializeResult");
//            return SteamEmulator.SteamInventory.SerializeResult(resultHandle, pOutBuffer, punOutBufferSize);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_SetPropertyBool(IntPtr handle, uint nItemID, string pchPropertyName, [MarshalAs(UnmanagedType.U1)] bool bValue)
//        {
//            Write("SteamAPI_ISteamInventory_SetPropertyBool");
//            return SteamEmulator.SteamInventory.SetPropertyBool(handle, nItemID, pchPropertyName, bValue);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_SetPropertyFloat(IntPtr handle, uint nItemID, string pchPropertyName, float flValue)
//        {
//            Write("SteamAPI_ISteamInventory_SetPropertyFloat");
//            return SteamEmulator.SteamInventory.SetPropertyFloat(handle, nItemID, pchPropertyName, flValue);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_SetPropertyInt64(IntPtr handle, uint nItemID, string pchPropertyName, long nValue)
//        {
//            Write("SteamAPI_ISteamInventory_SetPropertyInt64");
//            return SteamEmulator.SteamInventory.SetPropertyInt64(handle, nItemID, pchPropertyName, nValue);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_SetPropertyString(IntPtr handle, uint nItemID, string pchPropertyName, string pchPropertyValue)
//        {
//            Write("SteamAPI_ISteamInventory_SetPropertyString");
//            return SteamEmulator.SteamInventory.SetPropertyString(handle, nItemID, pchPropertyName, pchPropertyValue);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static SteamAPICall_t SteamAPI_ISteamInventory_StartPurchase(IntPtr pArrayItemDefs, IntPtr punArrayQuantity, uint unArrayLength)
//        {
//            Write("SteamAPI_ISteamInventory_StartPurchase");
//            return SteamEmulator.SteamInventory.StartPurchase(pArrayItemDefs, punArrayQuantity, unArrayLength);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static ulong SteamAPI_ISteamInventory_StartUpdateProperties(IntPtr _)
//        {
//            Write("SteamAPI_ISteamInventory_StartUpdateProperties");
//            return SteamEmulator.SteamInventory.StartUpdateProperties();
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_SubmitUpdateProperties(ulong handle, SteamInventoryResult_t pResultHandle)
//        {
//            Write("SteamAPI_ISteamInventory_SubmitUpdateProperties");
//            return SteamEmulator.SteamInventory.SubmitUpdateProperties(handle, pResultHandle);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_TradeItems(uint pResultHandle, ulong steamIDTradePartner, IntPtr pArrayGive, IntPtr pArrayGiveQuantity, uint nArrayGiveLength, IntPtr pArrayGet, IntPtr pArrayGetQuantity, uint nArrayGetLength)
//        {
//            Write("SteamAPI_ISteamInventory_TradeItems");
//            return SteamEmulator.SteamInventory.TradeItems(pResultHandle, steamIDTradePartner, pArrayGive, pArrayGiveQuantity, nArrayGiveLength, pArrayGet, pArrayGetQuantity, nArrayGetLength);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_TransferItemQuantity(SteamInventoryResult_t pResultHandle, uint itemIdSource, uint unQuantity, uint itemIdDest)
//        {
//            Write("SteamAPI_ISteamInventory_TransferItemQuantity");
//            return SteamEmulator.SteamInventory.TransferItemQuantity(pResultHandle, itemIdSource, unQuantity, itemIdDest);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInventory_TriggerItemDrop(SteamInventoryResult_t pResultHandle, uint dropListDefinition)
//        {
//            Write("SteamAPI_ISteamInventory_TriggerItemDrop");
//            return SteamEmulator.SteamInventory.TriggerItemDrop(pResultHandle, dropListDefinition);
//        }

//        private static void Write(string msg)
//        {
//            SteamEmulator.Write("SteamAPI_ISteamInventory", msg);
//        }
//    }
//}
