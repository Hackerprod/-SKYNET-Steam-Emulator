using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    [StructLayout(LayoutKind.Sequential)]
    public class SteamInventory : ISteamInterface
    {
        public bool AddPromoItem(ref SteamInventoryResult_t pResultHandle, uint itemDef)
        {
            Write($"AddPromoItem");
            return false;
        }

        public bool AddPromoItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayItemDefs, uint unArrayLength)
        {
            Write($"AddPromoItems");
            return false;
        }

        public bool CheckResultSteamID(SteamInventoryResult_t resultHandle, IntPtr steamIDExpected)
        {
            Write($"CheckResultSteamID");
            return false;
        }

        public bool ConsumeItem(ref SteamInventoryResult_t pResultHandle, uint itemConsume, uint unQuantity)
        {
            Write($"ConsumeItem");
            return false;
        }

        public bool DeserializeResult(ref SteamInventoryResult_t pOutResultHandle, IntPtr pBuffer, uint unBufferSize, [MarshalAs(UnmanagedType.U1)] bool bRESERVED_MUST_BE_FALSE)
        {
            Write($"DeserializeResult");
            return false;
        }

        public void DestroyResult(SteamInventoryResult_t resultHandle)
        {
            Write($"DestroyResult");
        }

        public bool ExchangeItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayGenerate, [In, Out] uint[] punArrayGenerateQuantity, uint unArrayGenerateLength, [In, Out] uint[] pArrayDestroy, [In, Out] uint[] punArrayDestroyQuantity, uint unArrayDestroyLength)
        {
            Write($"ExchangeItems");
            return false;
        }

        public bool GenerateItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayItemDefs, [In, Out] uint[] punArrayQuantity, uint unArrayLength)
        {
            Write($"GenerateItems");
            return false;
        }

        public bool GetAllItems(ref SteamInventoryResult_t pResultHandle)
        {
            Write($"GetAllItems");
            return false;
        }

        public bool GetEligiblePromoItemDefinitionIDs(IntPtr steamID, [In, Out] uint[] pItemDefIDs, ref uint punItemDefIDsArraySize)
        {
            Write($"GetEligiblePromoItemDefinitionIDs");
            return false;
        }

        public bool GetItemDefinitionIDs([In, Out] uint[] pItemDefIDs, ref uint punItemDefIDsArraySize)
        {
            Write($"GetItemDefinitionIDs");
            return false;
        }

        public bool GetItemDefinitionProperty(uint iDefinition, string pchPropertyName, IntPtr pchValueBuffer, ref uint punValueBufferSizeOut)
        {
            Write($"GetItemDefinitionProperty");
            return false;
        }

        public bool GetItemPrice(uint iDefinition, ref ulong pCurrentPrice, ref ulong pBasePrice)
        {
            Write($"GetItemPrice");
            return false;
        }

        public bool GetItemsByID(ref SteamInventoryResult_t pResultHandle, ref uint pInstanceIDs, uint unCountInstanceIDs)
        {
            Write($"GetItemsByID");
            return false;
        }

        public bool GetItemsWithPrices([In, Out] uint[] pArrayItemDefs, [In, Out] ulong[] pCurrentPrices, [In, Out] ulong[] pBasePrices, uint unArrayLength)
        {
            Write($"GetItemsWithPrices");
            return false;
        }

        public uint GetNumItemsWithPrices(IntPtr self)
        {
            Write($"GetNumItemsWithPrices");
            return 0;
        }

        public bool GetResultItemProperty(SteamInventoryResult_t resultHandle, uint unItemIndex, string pchPropertyName, IntPtr pchValueBuffer, ref uint punValueBufferSizeOut)
        {
            Write($"GetResultItemProperty");
            return false;
        }

        public bool GetResultItems(SteamInventoryResult_t resultHandle, [In, Out] SteamItemDetails_t[] pOutItemsArray, ref uint punOutItemsArraySize)
        {
            Write($"GetResultItems");
            return false;
        }

        public Result GetResultStatus(SteamInventoryResult_t resultHandle)
        {
            Write($"GetResultStatus");
            return Result.OK;
        }

        public uint GetResultTimestamp(SteamInventoryResult_t resultHandle)
        {
            Write($"GetResultTimestamp");
            return 0;
        }

        public bool GrantPromoItems(ref SteamInventoryResult_t pResultHandle)
        {
            Write($"GrantPromoItems");
            return false;
        }

        public bool InspectItem(ref SteamInventoryResult_t pResultHandle, string pchItemToken)
        {
            Write($"InspectItem");
            return false;
        }

        public bool LoadItemDefinitions(IntPtr self)
        {
            Write($"LoadItemDefinitions");
            return false;
        }

        public bool RemoveProperty(IntPtr handle, uint nItemID, string pchPropertyName)
        {
            Write($"RemoveProperty");
            return false;
        }

        public SteamAPICall_t RequestEligiblePromoItemDefinitionsIDs(IntPtr steamID)
        {
            Write($"RequestEligiblePromoItemDefinitionsIDs");
            return new SteamAPICall_t();
        }

        public SteamAPICall_t RequestPrices(IntPtr self)
        {
            Write($"RequestPrices");
            return new SteamAPICall_t();
        }

        public void SendItemDropHeartbeat(IntPtr self)
        {
            Write($"SendItemDropHeartbeat");
        }

        public bool SerializeResult(SteamInventoryResult_t resultHandle, IntPtr pOutBuffer, ref uint punOutBufferSize)
        {
            Write($"SerializeResult");
            return false;
        }

        public bool SetPropertyBool(IntPtr handle, uint nItemID, string pchPropertyName, [MarshalAs(UnmanagedType.U1)] bool bValue)
        {
            Write($"SetPropertyBool");
            return false;
        }

        public bool SetPropertyFloat(IntPtr handle, uint nItemID, string pchPropertyName, float flValue)
        {
            Write($"SetPropertyFloat");
            return false;
        }

        public bool SetPropertyInt64(IntPtr handle, uint nItemID, string pchPropertyName, long nValue)
        {
            Write($"SetPropertyInt64");
            return false;
        }

        public bool SetPropertyString(IntPtr handle, uint nItemID, string pchPropertyName, string pchPropertyValue)
        {
            Write($"SetPropertyString");
            return false;
        }

        public SteamAPICall_t StartPurchase([In, Out] uint[] pArrayItemDefs, [In, Out] uint[] punArrayQuantity, uint unArrayLength)
        {
            Write($"StartPurchase");
            return new SteamAPICall_t();
        }

        public IntPtr StartUpdateProperties(IntPtr _)
        {
            Write($"StartUpdateProperties");
            return IntPtr.Zero;
        }

        public bool SubmitUpdateProperties(IntPtr handle, ref SteamInventoryResult_t pResultHandle)
        {
            Write($"SubmitUpdateProperties");
            return false;
        }

        public bool TradeItems(ref SteamInventoryResult_t pResultHandle, IntPtr steamIDTradePartner, [In, Out] uint[] pArrayGive, [In, Out] uint[] pArrayGiveQuantity, uint nArrayGiveLength, [In, Out] uint[] pArrayGet, [In, Out] uint[] pArrayGetQuantity, uint nArrayGetLength)
        {
            Write($"TradeItems");
            return false;
        }

        public bool TransferItemQuantity(ref SteamInventoryResult_t pResultHandle, uint itemIdSource, uint unQuantity, uint itemIdDest)
        {
            Write($"TransferItemQuantity");
            return false;
        }

        public bool TriggerItemDrop(ref SteamInventoryResult_t pResultHandle, uint dropListDefinition)
        {
            Write($"TriggerItemDrop");
            return false;
        }

        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamInventory()
        {
            InterfaceVersion = "SteamInventory";
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}