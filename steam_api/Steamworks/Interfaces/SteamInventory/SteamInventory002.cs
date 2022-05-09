using System;


namespace SKYNET.Steamworks.Interfaces
{
    [Interface("STEAMINVENTORY_INTERFACE_V002")]
    public class SteamInventory002 : ISteamInterface
    {
        public int GetResultStatus(IntPtr _, int resultHandle)
        {
            return SteamEmulator.SteamInventory.GetResultStatus((uint)resultHandle);
        }

        public bool GetResultItems(IntPtr _, int resultHandle, IntPtr pOutItemsArray, uint punOutItemsArraySize)
        {
            return SteamEmulator.SteamInventory.GetResultItems((uint)resultHandle, pOutItemsArray, punOutItemsArraySize);
        }

        public bool GetResultItemProperty(IntPtr _, int resultHandle, uint unItemIndex, string pchPropertyName, IntPtr pchValueBuffer, uint punValueBufferSizeOut)
        {
            return SteamEmulator.SteamInventory.GetResultItemProperty((uint)resultHandle, unItemIndex, pchPropertyName, pchValueBuffer, punValueBufferSizeOut);
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

        public bool GetAllItems(IntPtr _, int pResultHandle)
        {
            return SteamEmulator.SteamInventory.GetAllItems((uint)pResultHandle);
        }

        public bool GetItemsByID(IntPtr _, int pResultHandle, ref ulong pInstanceIDs, uint unCountInstanceIDs)
        {
            return SteamEmulator.SteamInventory.GetItemsByID((uint)pResultHandle, ref pInstanceIDs, unCountInstanceIDs);
        }

        public bool SerializeResult(IntPtr _, int resultHandle, IntPtr pOutBuffer, uint punOutBufferSize)
        {
            return SteamEmulator.SteamInventory.SerializeResult((uint)resultHandle, pOutBuffer, punOutBufferSize);
        }

        public bool DeserializeResult(IntPtr _, int pOutResultHandle, IntPtr pBuffer, uint unBufferSize, bool bRESERVED_MUST_BE_FALSE = false)
        {
            return SteamEmulator.SteamInventory.DeserializeResult((uint)pOutResultHandle, pBuffer, unBufferSize, false);
        }

        public bool GenerateItems(IntPtr _, int pResultHandle, IntPtr pArrayItemDefs, IntPtr punArrayQuantity, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.GenerateItems((uint)pResultHandle, pArrayItemDefs, punArrayQuantity, unArrayLength);
        }

        public bool GrantPromoItems(IntPtr _, int pResultHandle)
        {
            return SteamEmulator.SteamInventory.GrantPromoItems((uint)pResultHandle);
        }

        public bool AddPromoItem(IntPtr _, int pResultHandle, int itemDef)
        {
            return SteamEmulator.SteamInventory.AddPromoItem((uint)pResultHandle, (uint)itemDef);
        }

        public bool AddPromoItems(IntPtr _, int pResultHandle, IntPtr pArrayItemDefs, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.AddPromoItems((uint)pResultHandle, pArrayItemDefs, unArrayLength);
        }

        public bool ConsumeItem(IntPtr _, int pResultHandle, ulong itemConsume, uint unQuantity)
        {
            return SteamEmulator.SteamInventory.ConsumeItem((uint)pResultHandle, itemConsume, unQuantity);
        }

        public bool ExchangeItems(IntPtr _, int pResultHandle, IntPtr pArrayGenerate, IntPtr punArrayGenerateQuantity, uint unArrayGenerateLength, ref ulong[] pArrayDestroy, uint punArrayDestroyQuantity, uint unArrayDestroyLength)
        {
            return SteamEmulator.SteamInventory.ExchangeItems((uint)pResultHandle, pArrayGenerate, punArrayGenerateQuantity, unArrayGenerateLength, ref pArrayDestroy, punArrayDestroyQuantity, unArrayDestroyLength);
        }

        public bool TransferItemQuantity(IntPtr _, int pResultHandle, ulong itemIdSource, uint unQuantity, ulong itemIdDest)
        {
            return SteamEmulator.SteamInventory.TransferItemQuantity((uint)pResultHandle, itemIdSource, unQuantity, itemIdDest);
        }

        public void SendItemDropHeartbeat(IntPtr _)
        {
            SteamEmulator.SteamInventory.SendItemDropHeartbeat(_);
        }

        public bool TriggerItemDrop(IntPtr _, int pResultHandle, int dropListDefinition)
        {
            return SteamEmulator.SteamInventory.TriggerItemDrop((uint)pResultHandle, (uint)dropListDefinition);
        }

        public bool TradeItems(IntPtr _, int pResultHandle, ulong steamIDTradePartner, IntPtr pArrayGive, IntPtr pArrayGiveQuantity, uint nArrayGiveLength, IntPtr pArrayGet, IntPtr pArrayGetQuantity, uint nArrayGetLength)
        {
            return SteamEmulator.SteamInventory.TradeItems((uint)pResultHandle, steamIDTradePartner, pArrayGive, pArrayGiveQuantity, nArrayGiveLength, pArrayGet, pArrayGetQuantity, nArrayGetLength);
        }

        public bool LoadItemDefinitions(IntPtr _)
        {
            return SteamEmulator.SteamInventory.LoadItemDefinitions();
        }

        public bool GetItemDefinitionIDs(IntPtr _, IntPtr pItemDefIDs, uint punItemDefIDsArraySize)
        {
            return SteamEmulator.SteamInventory.GetItemDefinitionIDs(pItemDefIDs, punItemDefIDsArraySize);
        }

        public bool GetItemDefinitionProperty(IntPtr _, int iDefinition, string pchPropertyName, IntPtr pchValueBuffer, uint punValueBufferSizeOut)
        {
            return SteamEmulator.SteamInventory.GetItemDefinitionProperty((uint)iDefinition, pchPropertyName, pchValueBuffer, punValueBufferSizeOut);
        }

        public ulong RequestEligiblePromoItemDefinitionsIDs(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamInventory.RequestEligiblePromoItemDefinitionsIDs(steamID);
        }

        public bool GetEligiblePromoItemDefinitionIDs(IntPtr _, ulong steamID, IntPtr pItemDefIDs, uint punItemDefIDsArraySize)
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

        public bool GetItemsWithPrices(IntPtr _, IntPtr pArrayItemDefs, IntPtr pPrices, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.GetItemsWithPrices(pArrayItemDefs, pPrices, (ulong)pPrices, unArrayLength);
        }

        public bool GetItemPrice(IntPtr _, uint iDefinition, ulong pPrice)
        {
            return SteamEmulator.SteamInventory.GetItemPrice(iDefinition, pPrice, pPrice);
        }

        public ulong StartUpdateProperties(IntPtr _)
        {
            return SteamEmulator.SteamInventory.StartUpdateProperties();
        }

        public bool RemoveProperty(IntPtr _, ulong handle, ulong nItemID, string pchPropertyName)
        {
            return SteamEmulator.SteamInventory.RemoveProperty(handle, nItemID, pchPropertyName);
        }

        public bool SetProperty(IntPtr _, IntPtr handle, ulong nItemID, string pchPropertyName, string pchPropertyValue)
        {
            //return SteamEmulator.SteamInventory.SetProperty(handle, nItemID, pchPropertyName, pchPropertyValue);
            return true;
        }

        public bool SetProperty(IntPtr _, IntPtr handle, ulong nItemID, string pchPropertyName, bool bValue)
        {
            //return SteamEmulator.SteamInventory.SetProperty(handle, nItemID, pchPropertyName, bValue);
            return true;
        }

        public bool SetProperty(IntPtr _, IntPtr handle, ulong nItemID, string pchPropertyName, long nValue)
        {
            //return SteamEmulator.SteamInventory.SetProperty(handle, nItemID, pchPropertyName, nValue);
            return true;
        }

        public bool SetProperty(IntPtr _, IntPtr handle, ulong nItemID, string pchPropertyName, float flValue)
        {
            //return SteamEmulator.SteamInventory.SetProperty(handle, nItemID, pchPropertyName, flValue);
            return true;
        }

        public bool SubmitUpdateProperties(IntPtr _, ulong handle, uint pResultHandle)
        {
            return SteamEmulator.SteamInventory.SubmitUpdateProperties(handle, pResultHandle);
        }
    }
}
