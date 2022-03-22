using Core.Interface;
using SKYNET.Interface;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamInventory")]
    public class DSteamInventory
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate Result GetResultStatus(SteamInventoryResult_t resultHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetResultItems(SteamInventoryResult_t resultHandle, [In, Out] SteamItemDetails_t[] pOutItemsArray, ref uint punOutItemsArraySize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetResultItemProperty(SteamInventoryResult_t resultHandle, uint unItemIndex, string pchPropertyName, IntPtr pchValueBuffer, ref uint punValueBufferSizeOut);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetResultTimestamp(SteamInventoryResult_t resultHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool CheckResultSteamID(SteamInventoryResult_t resultHandle, IntPtr steamIDExpected);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void DestroyResult(SteamInventoryResult_t resultHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetAllItems(ref SteamInventoryResult_t pResultHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetItemsByID(ref SteamInventoryResult_t pResultHandle, ref uint pInstanceIDs, uint unCountInstanceIDs);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SerializeResult(SteamInventoryResult_t resultHandle, IntPtr pOutBuffer, ref uint punOutBufferSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool DeserializeResult(ref SteamInventoryResult_t pOutResultHandle, IntPtr pBuffer, uint unBufferSize, [MarshalAs(UnmanagedType.U1)] bool bRESERVED_MUST_BE_FALSE);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GenerateItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayItemDefs, [In, Out] uint[] punArrayQuantity, uint unArrayLength);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GrantPromoItems(ref SteamInventoryResult_t pResultHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddPromoItem(ref SteamInventoryResult_t pResultHandle, uint itemDef);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddPromoItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayItemDefs, uint unArrayLength);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ConsumeItem(ref SteamInventoryResult_t pResultHandle, uint itemConsume, uint unQuantity);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ExchangeItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayGenerate, [In, Out] uint[] punArrayGenerateQuantity, uint unArrayGenerateLength, [In, Out] uint[] pArrayDestroy, [In, Out] uint[] punArrayDestroyQuantity, uint unArrayDestroyLength);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool TransferItemQuantity(ref SteamInventoryResult_t pResultHandle, uint itemIdSource, uint unQuantity, uint itemIdDest);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SendItemDropHeartbeat(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool TriggerItemDrop(ref SteamInventoryResult_t pResultHandle, uint dropListDefinition);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool TradeItems(ref SteamInventoryResult_t pResultHandle, IntPtr steamIDTradePartner, [In, Out] uint[] pArrayGive, [In, Out] uint[] pArrayGiveQuantity, uint nArrayGiveLength, [In, Out] uint[] pArrayGet, [In, Out] uint[] pArrayGetQuantity, uint nArrayGetLength);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool LoadItemDefinitions(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetItemDefinitionIDs([In, Out] uint[] pItemDefIDs, ref uint punItemDefIDsArraySize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetItemDefinitionProperty(uint iDefinition, string pchPropertyName, IntPtr pchValueBuffer, ref uint punValueBufferSizeOut);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RequestEligiblePromoItemDefinitionsIDs(IntPtr steamID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetEligiblePromoItemDefinitionIDs(IntPtr steamID, [In, Out] uint[] pItemDefIDs, ref uint punItemDefIDsArraySize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t StartPurchase([In, Out] uint[] pArrayItemDefs, [In, Out] uint[] punArrayQuantity, uint unArrayLength);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RequestPrices(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetNumItemsWithPrices(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetItemsWithPrices([In, Out] uint[] pArrayItemDefs, [In, Out] ulong[] pCurrentPrices, [In, Out] ulong[] pBasePrices, uint unArrayLength);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetItemPrice(uint iDefinition, ref ulong pCurrentPrice, ref ulong pBasePrice);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr StartUpdateProperties(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RemoveProperty(IntPtr handle, uint nItemID, string pchPropertyName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetPropertyString(IntPtr handle, uint nItemID, string pchPropertyName, string pchPropertyValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetPropertyBool(IntPtr handle, uint nItemID, string pchPropertyName, [MarshalAs(UnmanagedType.U1)] bool bValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetPropertyInt64(IntPtr handle, uint nItemID, string pchPropertyName, long nValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetPropertyFloat(IntPtr handle, uint nItemID, string pchPropertyName, float flValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SubmitUpdateProperties(IntPtr handle, ref SteamInventoryResult_t pResultHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool InspectItem(ref SteamInventoryResult_t pResultHandle, string pchItemToken);

    }
}
