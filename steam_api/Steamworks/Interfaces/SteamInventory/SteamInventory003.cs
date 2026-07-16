using System;
using System.Runtime.InteropServices;

using SteamInventoryResult_t = System.Int32;
using SteamItemInstanceID_t = System.UInt64;
using SteamAPICall_t = System.UInt64;
using SteamItemDef_t = System.Int32;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("STEAMINVENTORY_INTERFACE_V003")]
    public class SteamInventory003 : ISteamInterface
    {
        public int GetResultStatus(IntPtr _, SteamInventoryResult_t resultHandle)
        {
            return SteamEmulator.SteamInventory.GetResultStatus(resultHandle);
        }

        public bool GetResultItems(IntPtr _, SteamInventoryResult_t resultHandle, IntPtr pOutItemsArray, IntPtr punOutItemsArraySize)
        {
            return SteamEmulator.SteamInventory.GetResultItems(resultHandle, pOutItemsArray, punOutItemsArraySize);
        }

        public bool GetResultItemProperty(IntPtr _, SteamInventoryResult_t resultHandle, uint unItemIndex, string pchPropertyName, IntPtr pchValueBuffer, IntPtr punValueBufferSizeOut)
        {
            return SteamEmulator.SteamInventory.GetResultItemProperty(resultHandle, unItemIndex, pchPropertyName, pchValueBuffer, punValueBufferSizeOut);
        }

        public uint GetResultTimestamp(IntPtr _, SteamInventoryResult_t resultHandle)
        {
            return SteamEmulator.SteamInventory.GetResultTimestamp(resultHandle);
        }

        public bool CheckResultSteamID(IntPtr _, SteamInventoryResult_t resultHandle, ulong steamIDExpected)
        {
            return SteamEmulator.SteamInventory.CheckResultSteamID(resultHandle, steamIDExpected);
        }

        public void DestroyResult(IntPtr _, SteamInventoryResult_t resultHandle)
        {
            SteamEmulator.SteamInventory.DestroyResult(resultHandle);
        }

        public bool GetAllItems(IntPtr _, IntPtr pResultHandle)
        {
            return SteamEmulator.SteamInventory.GetAllItems(pResultHandle);
        }

        public bool GetItemsByID(IntPtr _, IntPtr pResultHandle, IntPtr pInstanceIDs, uint unCountInstanceIDs)
        {
            return SteamEmulator.SteamInventory.GetItemsByID(pResultHandle, pInstanceIDs, unCountInstanceIDs);
        }

        public bool SerializeResult(IntPtr _, SteamInventoryResult_t resultHandle, IntPtr pOutBuffer, IntPtr punOutBufferSize)
        {
            return SteamEmulator.SteamInventory.SerializeResult(resultHandle, pOutBuffer, punOutBufferSize);
        }

        public bool DeserializeResult(IntPtr _, IntPtr pOutResultHandle, IntPtr pBuffer, uint unBufferSize, bool bRESERVED_MUST_BE_FALSE = false)
        {
            return SteamEmulator.SteamInventory.DeserializeResult(pOutResultHandle, pBuffer, unBufferSize);
        }

        public bool GenerateItems(IntPtr _, IntPtr pResultHandle, IntPtr pArrayItemDefs, IntPtr punArrayQuantity, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.GenerateItems(pResultHandle, pArrayItemDefs, punArrayQuantity, unArrayLength);
        }

        public bool GrantPromoItems(IntPtr _, IntPtr pResultHandle)
        {
            return SteamEmulator.SteamInventory.GrantPromoItems(pResultHandle);
        }

        public bool AddPromoItem(IntPtr _, IntPtr pResultHandle, SteamItemDef_t itemDef)
        {
            return SteamEmulator.SteamInventory.AddPromoItem(pResultHandle, itemDef);
        }

        public bool AddPromoItems(IntPtr _, IntPtr pResultHandle, IntPtr pArrayItemDefs, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.AddPromoItems(pResultHandle, pArrayItemDefs, unArrayLength);
        }

        public bool ConsumeItem(IntPtr _, IntPtr pResultHandle, SteamItemInstanceID_t itemConsume, uint unQuantity)
        {
            return SteamEmulator.SteamInventory.ConsumeItem(pResultHandle, itemConsume, unQuantity);
        }

        public bool ExchangeItems(IntPtr _, IntPtr pResultHandle, IntPtr pArrayGenerate, IntPtr punArrayGenerateQuantity, uint unArrayGenerateLength, IntPtr pArrayDestroy, IntPtr punArrayDestroyQuantity, uint unArrayDestroyLength)
        {
            return SteamEmulator.SteamInventory.ExchangeItems(pResultHandle, pArrayGenerate, punArrayGenerateQuantity, unArrayGenerateLength, pArrayDestroy, punArrayDestroyQuantity, unArrayDestroyLength);
        }

        public bool TransferItemQuantity(IntPtr _, IntPtr pResultHandle, SteamItemInstanceID_t itemIdSource, uint unQuantity, SteamItemInstanceID_t itemIdDest)
        {
            return SteamEmulator.SteamInventory.TransferItemQuantity(pResultHandle, itemIdSource, unQuantity, itemIdDest);
        }

        public void SendItemDropHeartbeat(IntPtr _)
        {
            SteamEmulator.SteamInventory.SendItemDropHeartbeat();
        }

        public bool TriggerItemDrop(IntPtr _, IntPtr pResultHandle, SteamItemDef_t dropListDefinition)
        {
            return SteamEmulator.SteamInventory.TriggerItemDrop(pResultHandle, dropListDefinition);
        }

        public bool TradeItems(IntPtr _, IntPtr pResultHandle, ulong steamIDTradePartner, IntPtr pArrayGive, IntPtr pArrayGiveQuantity, uint nArrayGiveLength, IntPtr pArrayGet, IntPtr pArrayGetQuantity, uint nArrayGetLength)
        {
            return SteamEmulator.SteamInventory.TradeItems(pResultHandle, steamIDTradePartner, pArrayGive, pArrayGiveQuantity, nArrayGiveLength, pArrayGet, pArrayGetQuantity, nArrayGetLength);
        }

        public bool LoadItemDefinitions(IntPtr _)
        {
            return SteamEmulator.SteamInventory.LoadItemDefinitions();
        }

        public bool GetItemDefinitionIDs(IntPtr _, IntPtr pItemDefIDs, IntPtr punItemDefIDsArraySize)
        {
            return SteamEmulator.SteamInventory.GetItemDefinitionIDs(pItemDefIDs, punItemDefIDsArraySize);
        }

        public bool GetItemDefinitionProperty(IntPtr _, SteamItemDef_t iDefinition, string pchPropertyName, IntPtr pchValueBuffer, IntPtr punValueBufferSizeOut)
        {
            return SteamEmulator.SteamInventory.GetItemDefinitionProperty(iDefinition, pchPropertyName, pchValueBuffer, punValueBufferSizeOut);
        }

        public ulong RequestEligiblePromoItemDefinitionsIDs(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamInventory.RequestEligiblePromoItemDefinitionsIDs(steamID);
        }

        public bool GetEligiblePromoItemDefinitionIDs(IntPtr _, ulong steamID, IntPtr pItemDefIDs, IntPtr punItemDefIDsArraySize)
        {
            return SteamEmulator.SteamInventory.GetEligiblePromoItemDefinitionIDs(steamID, pItemDefIDs, punItemDefIDsArraySize);
        }

        public ulong StartPurchase(IntPtr _, IntPtr pArrayItemDefs, IntPtr punArrayQuantity, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.StartPurchase(pArrayItemDefs, punArrayQuantity, unArrayLength);
        }

        public ulong RequestPrices(IntPtr _)
        {
            return SteamEmulator.SteamInventory.RequestPrices();
        }

        public uint GetNumItemsWithPrices(IntPtr _)
        {
            return SteamEmulator.SteamInventory.GetNumItemsWithPrices();
        }

        public bool GetItemsWithPrices(IntPtr _, IntPtr pArrayItemDefs, IntPtr pCurrentPrices, IntPtr pBasePrices, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.GetItemsWithPrices(pArrayItemDefs, pCurrentPrices, pBasePrices, unArrayLength);
        }

        public bool GetItemPrice(IntPtr _, SteamItemDef_t iDefinition, IntPtr pCurrentPrice, IntPtr pBasePrice)
        {
            return SteamEmulator.SteamInventory.GetItemPrice(iDefinition, pCurrentPrice, pBasePrice);
        }

        public ulong StartUpdateProperties(IntPtr _)
        {
            return SteamEmulator.SteamInventory.StartUpdateProperties();
        }

        public bool RemoveProperty(IntPtr _, ulong handle, SteamItemInstanceID_t nItemID, string pchPropertyName)
        {
            return SteamEmulator.SteamInventory.RemoveProperty(handle, nItemID, pchPropertyName);
        }

        public bool SetPropertyString(IntPtr _, ulong handle, SteamItemInstanceID_t nItemID, string pchPropertyName, string pchPropertyValue)
        {
            return SteamEmulator.SteamInventory.SetPropertyString(handle, nItemID, pchPropertyName, pchPropertyValue);
        }

        public bool SetPropertyBool(IntPtr _, ulong handle, SteamItemInstanceID_t nItemID, string pchPropertyName, bool bValue)
        {
            return SteamEmulator.SteamInventory.SetPropertyBool(handle, nItemID, pchPropertyName, bValue);
        }

        public bool SetPropertyInt64(IntPtr _, ulong handle, SteamItemInstanceID_t nItemID, string pchPropertyName, long nValue)
        {
            return SteamEmulator.SteamInventory.SetPropertyInt64(handle, nItemID, pchPropertyName, nValue);
        }

        public bool SetPropertyFloat(IntPtr _, ulong handle, SteamItemInstanceID_t nItemID, string pchPropertyName, float flValue)
        {
            return SteamEmulator.SteamInventory.SetPropertyFloat(handle, nItemID, pchPropertyName, flValue);
        }

        public bool SubmitUpdateProperties(IntPtr _, ulong handle, IntPtr pResultHandle)
        {
            return SteamEmulator.SteamInventory.SubmitUpdateProperties(handle, pResultHandle);
        }

        public bool InspectItem(IntPtr _, IntPtr pResultHandle, string pchItemToken)
        {
            return SteamEmulator.SteamInventory.InspectItem(pResultHandle, pchItemToken);
        }
    }
}
