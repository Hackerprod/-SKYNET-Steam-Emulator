using System;
using System.Runtime.InteropServices;

using SteamItemInstanceID_t = System.UInt64;
using SteamInventoryResult_t = System.UInt32;
using SteamItemDef_t = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("STEAMINVENTORY_INTERFACE_V002")]
    public class SteamInventory002 : ISteamInterface
    {
        public int GetResultStatus(IntPtr _, int resultHandle)
        {
            return SteamEmulator.SteamInventory.GetResultStatus((uint)resultHandle);
        }

        public bool GetResultItems(IntPtr _, int resultHandle, IntPtr pOutItemsArray, IntPtr punOutItemsArraySize)
        {
            WriteUInt32(punOutItemsArraySize, 0);
            return SteamEmulator.SteamInventory.GetResultItems((uint)resultHandle, pOutItemsArray, 0);
        }

        public bool GetResultItemProperty(IntPtr _, int resultHandle, uint unItemIndex, string pchPropertyName, IntPtr pchValueBuffer, IntPtr punValueBufferSizeOut)
        {
            WriteUInt32(punValueBufferSizeOut, 0);
            return SteamEmulator.SteamInventory.GetResultItemProperty((uint)resultHandle, unItemIndex, pchPropertyName, pchValueBuffer, 0);
        }

        public uint GetResultTimestamp(IntPtr _, int resultHandle)
        {
            return SteamEmulator.SteamInventory.GetResultTimestamp((uint)resultHandle);
        }

        public bool CheckResultSteamID(IntPtr _, int resultHandle, ulong steamIDExpected)
        {
            return SteamEmulator.SteamInventory.CheckResultSteamID((uint)resultHandle, steamIDExpected);
        }

        public void DestroyResult(IntPtr _, int resultHandle)
        {
            SteamEmulator.SteamInventory.DestroyResult((uint)resultHandle);
        }

        public bool GetAllItems(IntPtr _, IntPtr pResultHandle)
        {
            WriteInt32(pResultHandle, 0);
            return SteamEmulator.SteamInventory.GetAllItems(0);
        }

        public bool GetItemsByID(IntPtr _, IntPtr pResultHandle, IntPtr pInstanceIDs, uint unCountInstanceIDs)
        {
            WriteInt32(pResultHandle, 0);
            return false;
        }

        public bool SerializeResult(IntPtr _, int resultHandle, IntPtr pOutBuffer, IntPtr punOutBufferSize)
        {
            WriteUInt32(punOutBufferSize, 0);
            return SteamEmulator.SteamInventory.SerializeResult((uint)resultHandle, pOutBuffer, 0);
        }

        public bool DeserializeResult(IntPtr _, IntPtr pOutResultHandle, IntPtr pBuffer, uint unBufferSize, bool bRESERVED_MUST_BE_FALSE = false)
        {
            WriteInt32(pOutResultHandle, 0);
            return false;
        }

        public bool GenerateItems(IntPtr _, IntPtr pResultHandle, IntPtr pArrayItemDefs, IntPtr punArrayQuantity, uint unArrayLength)
        {
            WriteInt32(pResultHandle, 0);
            return SteamEmulator.SteamInventory.GenerateItems(0, pArrayItemDefs, punArrayQuantity, unArrayLength);
        }

        public bool GrantPromoItems(IntPtr _, IntPtr pResultHandle)
        {
            WriteInt32(pResultHandle, 0);
            return SteamEmulator.SteamInventory.GrantPromoItems(0);
        }

        public bool AddPromoItem(IntPtr _, IntPtr pResultHandle, int itemDef)
        {
            WriteInt32(pResultHandle, 0);
            return SteamEmulator.SteamInventory.AddPromoItem(0, (uint)itemDef);
        }

        public bool AddPromoItems(IntPtr _, IntPtr pResultHandle, IntPtr pArrayItemDefs, uint unArrayLength)
        {
            WriteInt32(pResultHandle, 0);
            return SteamEmulator.SteamInventory.AddPromoItems(0, pArrayItemDefs, unArrayLength);
        }

        public bool ConsumeItem(IntPtr _, IntPtr pResultHandle, ulong itemConsume, uint unQuantity)
        {
            WriteInt32(pResultHandle, 0);
            return SteamEmulator.SteamInventory.ConsumeItem(0, itemConsume, unQuantity);
        }

        public bool ExchangeItems(IntPtr _, IntPtr pResultHandle, IntPtr pArrayGenerate, IntPtr punArrayGenerateQuantity, uint unArrayGenerateLength, IntPtr pArrayDestroy, IntPtr punArrayDestroyQuantity, uint unArrayDestroyLength)
        {
            WriteInt32(pResultHandle, 0);
            return false;
        }

        public bool TransferItemQuantity(IntPtr _, IntPtr pResultHandle, ulong itemIdSource, uint unQuantity, ulong itemIdDest)
        {
            WriteInt32(pResultHandle, 0);
            return SteamEmulator.SteamInventory.TransferItemQuantity(0, itemIdSource, unQuantity, itemIdDest);
        }

        public void SendItemDropHeartbeat(IntPtr _)
        {
            SteamEmulator.SteamInventory.SendItemDropHeartbeat();
        }

        public bool TriggerItemDrop(IntPtr _, IntPtr pResultHandle, int dropListDefinition)
        {
            WriteInt32(pResultHandle, 0);
            return SteamEmulator.SteamInventory.TriggerItemDrop(0, (uint)dropListDefinition);
        }

        public bool TradeItems(IntPtr _, IntPtr pResultHandle, ulong steamIDTradePartner, IntPtr pArrayGive, IntPtr pArrayGiveQuantity, uint nArrayGiveLength, IntPtr pArrayGet, IntPtr pArrayGetQuantity, uint nArrayGetLength)
        {
            WriteInt32(pResultHandle, 0);
            return SteamEmulator.SteamInventory.TradeItems(0, steamIDTradePartner, pArrayGive, pArrayGiveQuantity, nArrayGiveLength, pArrayGet, pArrayGetQuantity, nArrayGetLength);
        }

        public bool LoadItemDefinitions(IntPtr _)
        {
            return SteamEmulator.SteamInventory.LoadItemDefinitions();
        }

        public bool GetItemDefinitionIDs(IntPtr _, IntPtr pItemDefIDs, IntPtr punItemDefIDsArraySize)
        {
            WriteUInt32(punItemDefIDsArraySize, 0);
            return SteamEmulator.SteamInventory.GetItemDefinitionIDs(pItemDefIDs, 0);
        }

        public bool GetItemDefinitionProperty(IntPtr _, int iDefinition, string pchPropertyName, IntPtr pchValueBuffer, IntPtr punValueBufferSizeOut)
        {
            WriteUInt32(punValueBufferSizeOut, 0);
            return SteamEmulator.SteamInventory.GetItemDefinitionProperty((uint)iDefinition, pchPropertyName, pchValueBuffer, 0);
        }

        public ulong RequestEligiblePromoItemDefinitionsIDs(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamInventory.RequestEligiblePromoItemDefinitionsIDs(steamID);
        }

        public bool GetEligiblePromoItemDefinitionIDs(IntPtr _, ulong steamID, IntPtr pItemDefIDs, IntPtr punItemDefIDsArraySize)
        {
            WriteUInt32(punItemDefIDsArraySize, 0);
            return SteamEmulator.SteamInventory.GetEligiblePromoItemDefinitionIDs(steamID, pItemDefIDs, 0);
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

        public bool GetItemsWithPrices(IntPtr _, IntPtr pArrayItemDefs, IntPtr pPrices, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.GetItemsWithPrices(pArrayItemDefs, pPrices, (ulong)pPrices, unArrayLength);
        }

        public bool GetItemPrice(IntPtr _, uint iDefinition, IntPtr pPrice)
        {
            return SteamEmulator.SteamInventory.GetItemPrice(iDefinition, 0, 0);
        }

        public ulong StartUpdateProperties(IntPtr _)
        {
            return SteamEmulator.SteamInventory.StartUpdateProperties();
        }

        public bool RemoveProperty(IntPtr _, ulong handle, ulong nItemID, string pchPropertyName)
        {
            return SteamEmulator.SteamInventory.RemoveProperty(handle, nItemID, pchPropertyName);
        }

        public bool SetProperty(IntPtr _, ulong handle, ulong nItemID, string pchPropertyName, string pchPropertyValue)
        {
            //return SteamEmulator.SteamInventory.SetProperty(handle, nItemID, pchPropertyName, pchPropertyValue);
            return true;
        }

        public bool SetProperty(IntPtr _, ulong handle, ulong nItemID, string pchPropertyName, bool bValue)
        {
            //return SteamEmulator.SteamInventory.SetProperty(handle, nItemID, pchPropertyName, bValue);
            return true;
        }

        public bool SetProperty(IntPtr _, ulong handle, ulong nItemID, string pchPropertyName, long nValue)
        {
            //return SteamEmulator.SteamInventory.SetProperty(handle, nItemID, pchPropertyName, nValue);
            return true;
        }

        public bool SetProperty(IntPtr _, ulong handle, ulong nItemID, string pchPropertyName, float flValue)
        {
            //return SteamEmulator.SteamInventory.SetProperty(handle, nItemID, pchPropertyName, flValue);
            return true;
        }

        public bool SubmitUpdateProperties(IntPtr _, ulong handle, IntPtr pResultHandle)
        {
            WriteInt32(pResultHandle, 0);
            return SteamEmulator.SteamInventory.SubmitUpdateProperties(handle, 0);
        }

        private static void WriteInt32(IntPtr destination, int value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteInt32(destination, value);
            }
        }

        private static void WriteUInt32(IntPtr destination, uint value)
        {
            WriteInt32(destination, unchecked((int)value));
        }
    }
}
