using System;
using System.Runtime.InteropServices;
using SKYNET.Interface;
using Steamworks;

namespace SKYNET.Managers
{
    [Map("STEAMINVENTORY_INTERFACE")]
    [Map("SteamInventory")]
    public class SteamInventory : IBaseInterface, ISteamInventory
    {
        public bool AddPromoItem(ref SteamInventoryResult_t pResultHandle, uint itemDef)
        {
            return false;
        }

        public bool AddPromoItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayItemDefs, uint unArrayLength)
        {
            return false;
        }

        public bool CheckResultSteamID(SteamInventoryResult_t resultHandle, IntPtr steamIDExpected)
        {
            return false;
        }

        public bool ConsumeItem(ref SteamInventoryResult_t pResultHandle, uint itemConsume, uint unQuantity)
        {
            return false;
        }

        public bool DeserializeResult(ref SteamInventoryResult_t pOutResultHandle, IntPtr pBuffer, uint unBufferSize, [MarshalAs(UnmanagedType.U1)] bool bRESERVED_MUST_BE_FALSE)
        {
            return false;
        }

        public void DestroyResult(SteamInventoryResult_t resultHandle)
        {
            //
        }

        public bool ExchangeItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayGenerate, [In, Out] uint[] punArrayGenerateQuantity, uint unArrayGenerateLength, [In, Out] uint[] pArrayDestroy, [In, Out] uint[] punArrayDestroyQuantity, uint unArrayDestroyLength)
        {
            return false;
        }

        public bool GenerateItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayItemDefs, [In, Out] uint[] punArrayQuantity, uint unArrayLength)
        {
            return false;
        }

        public bool GetAllItems(ref SteamInventoryResult_t pResultHandle)
        {
            return false;
        }

        public bool GetEligiblePromoItemDefinitionIDs(IntPtr steamID, [In, Out] uint[] pItemDefIDs, ref uint punItemDefIDsArraySize)
        {
            return false;
        }

        public bool GetItemDefinitionIDs([In, Out] uint[] pItemDefIDs, ref uint punItemDefIDsArraySize)
        {
            return false;
        }

        public bool GetItemDefinitionProperty(uint iDefinition, string pchPropertyName, IntPtr pchValueBuffer, ref uint punValueBufferSizeOut)
        {
            return false;
        }

        public bool GetItemPrice(uint iDefinition, ref ulong pCurrentPrice, ref ulong pBasePrice)
        {
            return false;
        }

        public bool GetItemsByID(ref SteamInventoryResult_t pResultHandle, ref uint pInstanceIDs, uint unCountInstanceIDs)
        {
            return false;
        }

        public bool GetItemsWithPrices([In, Out] uint[] pArrayItemDefs, [In, Out] ulong[] pCurrentPrices, [In, Out] ulong[] pBasePrices, uint unArrayLength)
        {
            return false;
        }

        public uint GetNumItemsWithPrices(IntPtr self)
        {
            return 0;
        }

        public bool GetResultItemProperty(SteamInventoryResult_t resultHandle, uint unItemIndex, string pchPropertyName, IntPtr pchValueBuffer, ref uint punValueBufferSizeOut)
        {
            return false;
        }

        public bool GetResultItems(SteamInventoryResult_t resultHandle, [In, Out] SteamItemDetails_t[] pOutItemsArray, ref uint punOutItemsArraySize)
        {
            return false;
        }

        public Result GetResultStatus(SteamInventoryResult_t resultHandle)
        {
            return Result.OK;
        }

        public uint GetResultTimestamp(SteamInventoryResult_t resultHandle)
        {
            return 0;
        }

        public bool GrantPromoItems(ref SteamInventoryResult_t pResultHandle)
        {
            return false;
        }

        public bool InspectItem(ref SteamInventoryResult_t pResultHandle, string pchItemToken)
        {
            return false;
        }

        public bool LoadItemDefinitions(IntPtr self)
        {
            return false;
        }

        public bool RemoveProperty(IntPtr handle, uint nItemID, string pchPropertyName)
        {
            return false;
        }

        public SteamAPICall_t RequestEligiblePromoItemDefinitionsIDs(IntPtr steamID)
        {
            return new SteamAPICall_t();
        }

        public SteamAPICall_t RequestPrices(IntPtr self)
        {
            return new SteamAPICall_t();
        }

        public void SendItemDropHeartbeat(IntPtr self)
        {
            //
        }

        public bool SerializeResult(SteamInventoryResult_t resultHandle, IntPtr pOutBuffer, ref uint punOutBufferSize)
        {
            return false;
        }

        public bool SetPropertyBool(IntPtr handle, uint nItemID, string pchPropertyName, [MarshalAs(UnmanagedType.U1)] bool bValue)
        {
            return false;
        }

        public bool SetPropertyFloat(IntPtr handle, uint nItemID, string pchPropertyName, float flValue)
        {
            return false;
        }

        public bool SetPropertyInt64(IntPtr handle, uint nItemID, string pchPropertyName, long nValue)
        {
            return false;
        }

        public bool SetPropertyString(IntPtr handle, uint nItemID, string pchPropertyName, string pchPropertyValue)
        {
            return false;
        }

        public SteamAPICall_t StartPurchase([In, Out] uint[] pArrayItemDefs, [In, Out] uint[] punArrayQuantity, uint unArrayLength)
        {
            return new SteamAPICall_t();
        }

        public IntPtr StartUpdateProperties()
        {
            return IntPtr.Zero;
        }

        public bool SubmitUpdateProperties(IntPtr handle, ref SteamInventoryResult_t pResultHandle)
        {
            return false;
        }

        public bool TradeItems(ref SteamInventoryResult_t pResultHandle, IntPtr steamIDTradePartner, [In, Out] uint[] pArrayGive, [In, Out] uint[] pArrayGiveQuantity, uint nArrayGiveLength, [In, Out] uint[] pArrayGet, [In, Out] uint[] pArrayGetQuantity, uint nArrayGetLength)
        {
            return false;
        }

        public bool TransferItemQuantity(ref SteamInventoryResult_t pResultHandle, uint itemIdSource, uint unQuantity, uint itemIdDest)
        {
            return false;
        }

        public bool TriggerItemDrop(ref SteamInventoryResult_t pResultHandle, uint dropListDefinition)
        {
            return false;
        }
    }
}