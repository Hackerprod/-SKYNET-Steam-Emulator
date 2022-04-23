using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using SKYNET.Types;
using Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamInventory : ISteamInterface
    {
        public SteamInventory()
        {
            InterfaceVersion = "SteamInventory";
        }

        public bool AddPromoItem(uint pResultHandle, uint itemDef)
        {
            Write($"AddPromoItem");
            return false;
        }

        public bool AddPromoItems(uint pResultHandle, IntPtr pArrayItemDefs, uint unArrayLength)
        {
            Write($"AddPromoItems");
            return false;
        }

        public bool CheckResultSteamID(uint resultHandle, ulong steamIDExpected)
        {
            Write($"CheckResultSteamID");
            return false;
        }

        public bool ConsumeItem(uint pResultHandle, ulong itemConsume, uint unQuantity)
        {
            Write($"ConsumeItem");
            return false;
        }

        public bool DeserializeResult(uint pOutResultHandle, IntPtr pBuffer, uint unBufferSize, [MarshalAs(UnmanagedType.U1)] bool bRESERVED_MUST_BE_FALSE)
        {
            Write($"DeserializeResult");
            return false;
        }

        public void DestroyResult(uint resultHandle)
        {
            Write($"DestroyResult");
        }

        public bool ExchangeItems(uint pResultHandle, IntPtr pArrayGenerate, IntPtr punArrayGenerateQuantity, uint unArrayGenerateLength, ulong pArrayDestroy, uint punArrayDestroyQuantity, uint unArrayDestroyLength)
        {
            Write($"ExchangeItems");
            return false;
        }

        public bool GenerateItems(uint pResultHandle, IntPtr pArrayItemDefs, IntPtr punArrayQuantity, uint unArrayLength)
        {
            Write($"GenerateItems");
            return false;
        }

        public bool GetAllItems(uint pResultHandle)
        {
            Write($"GetAllItems");
            return false;
        }

        public bool GetEligiblePromoItemDefinitionIDs(ulong steamID, IntPtr pItemDefIDs, uint punItemDefIDsArraySize)
        {
            Write($"GetEligiblePromoItemDefinitionIDs");
            return false;
        }

        public bool GetItemDefinitionIDs(IntPtr pItemDefIDs, uint punItemDefIDsArraySize)
        {
            Write($"GetItemDefinitionIDs");
            return false;
        }

        public bool GetItemDefinitionProperty(uint iDefinition, string pchPropertyName, IntPtr pchValueBuffer, uint punValueBufferSizeOut)
        {
            Write($"GetItemDefinitionProperty");
            return false;
        }

        public bool GetItemPrice(uint iDefinition, ulong pCurrentPrice, ulong pBasePrice)
        {
            Write($"GetItemPrice");
            return false;
        }

        public bool GetItemsByID(uint pResultHandle, ulong pInstanceIDs, uint unCountInstanceIDs)
        {
            Write($"GetItemsByID");
            return false;
        }

        public bool GetItemsWithPrices(IntPtr pArrayItemDefs, IntPtr pCurrentPrices, ulong pBasePrices, uint unArrayLength)
        {
            Write($"GetItemsWithPrices");
            return false;
        }

        public uint GetNumItemsWithPrices(IntPtr self)
        {
            Write($"GetNumItemsWithPrices");
            return 0;
        }

        public bool GetResultItemProperty(uint resultHandle, uint unItemIndex, string pchPropertyName, IntPtr pchValueBuffer, uint punValueBufferSizeOut)
        {
            Write($"GetResultItemProperty");
            return false;
        }

        public bool GetResultItems(uint resultHandle, IntPtr pOutItemsArray, uint punOutItemsArraySize)
        {
            Write($"GetResultItems");
            return false;
        }

        public int GetResultStatus(uint resultHandle)
        {
            Write($"GetResultStatus");
            return (int)EResult.k_EResultOK;
        }

        public uint GetResultTimestamp(uint resultHandle)
        {
            Write($"GetResultTimestamp");
            return 0;
        }

        public bool GrantPromoItems(uint pResultHandle)
        {
            Write($"GrantPromoItems");
            return false;
        }

        public bool InspectItem(uint pResultHandle, string pchItemToken)
        {
            Write($"InspectItem");
            return false;
        }

        public bool LoadItemDefinitions(IntPtr self)
        {
            Write($"LoadItemDefinitions");
            return false;
        }

        public bool RemoveProperty(ulong handle, ulong nItemID, string pchPropertyName)
        {
            Write($"RemoveProperty");
            return false;
        }

        public ulong RequestEligiblePromoItemDefinitionsIDs(ulong steamID)
        {
            Write($"RequestEligiblePromoItemDefinitionsIDs");
            return 0;
        }

        public ulong RequestPrices(IntPtr self)
        {
            Write($"RequestPrices");
            return new ulong();
        }

        public void SendItemDropHeartbeat(IntPtr self)
        {
            Write($"SendItemDropHeartbeat");
        }

        public bool SerializeResult(uint resultHandle, IntPtr pOutBuffer, uint punOutBufferSize)
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

        public ulong StartPurchase(IntPtr pArrayItemDefs, IntPtr punArrayQuantity, uint unArrayLength)
        {
            Write($"StartPurchase");
            return new ulong();
        }

        public ulong StartUpdateProperties(IntPtr _)
        {
            Write($"StartUpdateProperties");
            return 0;
        }

        public bool SubmitUpdateProperties(ulong handle, uint pResultHandle)
        {
            Write($"SubmitUpdateProperties");
            return false;
        }

        public bool TradeItems(uint pResultHandle, ulong steamIDTradePartner, IntPtr pArrayGive, IntPtr pArrayGiveQuantity, uint nArrayGiveLength, IntPtr pArrayGet, IntPtr pArrayGetQuantity, uint nArrayGetLength)
        {
            Write($"TradeItems");
            return false;
        }

        public bool TransferItemQuantity(uint pResultHandle, ulong itemIdSource, uint unQuantity, ulong itemIdDest)
        {
            Write($"TransferItemQuantity");
            return false;
        }

        public bool TriggerItemDrop(uint pResultHandle, uint dropListDefinition)
        {
            Write($"TriggerItemDrop");
            return false;
        }

        public bool SetProperty(ulong handle, ulong nItemID, string pchPropertyName, string pchPropertyValue)
        {
            Write($"SetProperty");
            return false;
        }

        public bool SetProperty(ulong handle, ulong nItemID, string pchPropertyName, bool bValue)
        {
            Write($"SetProperty");
            return false;
        }

        public bool SetProperty(ulong handle, ulong nItemID, string pchPropertyName, long nValue)
        {
            Write($"SetProperty");
            return false;
        }

        public bool SetProperty(ulong handle, ulong nItemID, string pchPropertyName, float fValue)
        {
            Write($"SetProperty");
            return false;
        }
    }
}