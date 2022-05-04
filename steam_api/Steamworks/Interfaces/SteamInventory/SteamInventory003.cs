using System;


namespace SKYNET.Interface
{
    [Interface("STEAMINVENTORY_INTERFACE_V003")]
    public class SteamInventory003 : ISteamInterface
    {
        public int GetResultStatus(IntPtr _, uint resultHandle)
        {
            return SteamEmulator.SteamInventory.GetResultStatus(resultHandle);
        }

        public bool GetResultItems(IntPtr _, uint resultHandle, IntPtr pOutItemsArray, uint punOutItemsArraySize)
        {
            return SteamEmulator.SteamInventory.GetResultItems(resultHandle, pOutItemsArray, punOutItemsArraySize);
        }

        public bool GetResultItemProperty(IntPtr _, uint resultHandle, uint unItemIndex, string pchPropertyName, IntPtr pchValueBuffer, uint punValueBufferSizeOut)
        {
            return SteamEmulator.SteamInventory.GetResultItemProperty(resultHandle, unItemIndex, pchPropertyName, pchValueBuffer, punValueBufferSizeOut);
        }

        public uint GetResultTimestamp(IntPtr _, uint resultHandle)
        {
            return SteamEmulator.SteamInventory.GetResultTimestamp(resultHandle);
        }

        public bool CheckResultSteamID(IntPtr _, uint resultHandle, ulong steamIDExpected)
        {
            return SteamEmulator.SteamInventory.CheckResultSteamID(resultHandle, steamIDExpected);
        }

        public void DestroyResult(IntPtr _, uint resultHandle)
        {
            SteamEmulator.SteamInventory.DestroyResult(resultHandle);
        }

        public bool GetAllItems(IntPtr _, uint pResultHandle)
        {
            return SteamEmulator.SteamInventory.GetAllItems(pResultHandle);
        }

        public bool GetItemsByID(IntPtr _, uint pResultHandle, ref ulong pInstanceIDs, uint unCountInstanceIDs)
        {
            return SteamEmulator.SteamInventory.GetItemsByID(pResultHandle, ref  pInstanceIDs, unCountInstanceIDs);
        }

        public bool SerializeResult(IntPtr _, uint resultHandle, IntPtr pOutBuffer, uint punOutBufferSize)
        {
            return SteamEmulator.SteamInventory.SerializeResult(resultHandle, pOutBuffer, punOutBufferSize);
        }

        public bool DeserializeResult(IntPtr _, uint pOutResultHandle, IntPtr pBuffer, uint unBufferSize, bool bRESERVED_MUST_BE_FALSE = false)
        {
            return SteamEmulator.SteamInventory.DeserializeResult(pOutResultHandle, pBuffer, unBufferSize, false);
        }

        public bool GenerateItems(IntPtr _, uint pResultHandle, IntPtr pArrayItemDefs, IntPtr punArrayQuantity, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.GenerateItems(pResultHandle, pArrayItemDefs, punArrayQuantity, unArrayLength);
        }

        public bool GrantPromoItems(IntPtr _, uint pResultHandle)
        {
            return SteamEmulator.SteamInventory.GrantPromoItems(pResultHandle);
        }

        public bool AddPromoItem(IntPtr _, uint pResultHandle, uint itemDef)
        {
            return SteamEmulator.SteamInventory.AddPromoItem(pResultHandle, itemDef);
        }

        public bool AddPromoItems(IntPtr _, uint pResultHandle, IntPtr pArrayItemDefs, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.AddPromoItems(pResultHandle, pArrayItemDefs, unArrayLength);
        }

        public bool ConsumeItem(IntPtr _, uint pResultHandle, ulong itemConsume, uint unQuantity)
        {
            return SteamEmulator.SteamInventory.ConsumeItem(pResultHandle, itemConsume, unQuantity);
        }

        public bool ExchangeItems(IntPtr _, uint pResultHandle, IntPtr pArrayGenerate, IntPtr punArrayGenerateQuantity, uint unArrayGenerateLength, ref ulong[] pArrayDestroy, uint punArrayDestroyQuantity, uint unArrayDestroyLength )
    {
            return SteamEmulator.SteamInventory.ExchangeItems(pResultHandle, pArrayGenerate, punArrayGenerateQuantity, unArrayGenerateLength, ref pArrayDestroy, punArrayDestroyQuantity, unArrayDestroyLength);
        }

        public bool TransferItemQuantity(IntPtr _, uint pResultHandle, ulong itemIdSource, uint unQuantity, ulong itemIdDest)
        {
            return SteamEmulator.SteamInventory.TransferItemQuantity(pResultHandle, itemIdSource, unQuantity, itemIdDest);
        }

        public void SendItemDropHeartbeat(IntPtr _)
        {
            SteamEmulator.SteamInventory.SendItemDropHeartbeat(_);
        }

        public bool TriggerItemDrop(IntPtr _, uint pResultHandle, uint dropListDefinition)
        {
            return SteamEmulator.SteamInventory.TriggerItemDrop(pResultHandle, dropListDefinition);
        }

        public bool TradeItems(IntPtr _, uint pResultHandle, ulong steamIDTradePartner, IntPtr pArrayGive, IntPtr pArrayGiveQuantity, uint nArrayGiveLength, IntPtr pArrayGet, IntPtr pArrayGetQuantity, uint nArrayGetLength )
    {
            return SteamEmulator.SteamInventory.TradeItems(pResultHandle, steamIDTradePartner, pArrayGive, pArrayGiveQuantity, nArrayGiveLength, pArrayGet, pArrayGetQuantity, nArrayGetLength);
        }

        public bool LoadItemDefinitions(IntPtr _)
        {
            return SteamEmulator.SteamInventory.LoadItemDefinitions();
        }

        public bool GetItemDefinitionIDs(IntPtr _, IntPtr pItemDefIDs, uint punItemDefIDsArraySize )
    {
            return SteamEmulator.SteamInventory.GetItemDefinitionIDs(pItemDefIDs, punItemDefIDsArraySize);
        }

        public bool GetItemDefinitionProperty(IntPtr _, uint iDefinition, string pchPropertyName, IntPtr pchValueBuffer, uint punValueBufferSizeOut)
        {
            return SteamEmulator.SteamInventory.GetItemDefinitionProperty(iDefinition, pchPropertyName, pchValueBuffer, punValueBufferSizeOut);
        }

        public ulong RequestEligiblePromoItemDefinitionsIDs(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamInventory.RequestEligiblePromoItemDefinitionsIDs(steamID);
        }

        public bool GetEligiblePromoItemDefinitionIDs(IntPtr _, ulong steamID, IntPtr pItemDefIDs, uint punItemDefIDsArraySize )
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

        public bool GetItemsWithPrices(IntPtr _, IntPtr pArrayItemDefs, IntPtr pCurrentPrices, ulong pBasePrices, uint unArrayLength)
        {
            return SteamEmulator.SteamInventory.GetItemsWithPrices(pArrayItemDefs, pCurrentPrices, pBasePrices, unArrayLength);
        }

        public bool GetItemPrice(IntPtr _, uint iDefinition, ulong pCurrentPrice, ulong pBasePrice)
        {
            return SteamEmulator.SteamInventory.GetItemPrice(iDefinition, pCurrentPrice, pBasePrice);
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
            return SteamEmulator.SteamInventory.SetProperty(handle, nItemID, pchPropertyName, pchPropertyValue);
        }

        public bool SetProperty(IntPtr _, ulong handle, ulong nItemID, string pchPropertyName, bool bValue)
        {
            return SteamEmulator.SteamInventory.SetProperty(handle, nItemID, pchPropertyName, bValue);
        }

        public bool SetProperty(IntPtr _, ulong handle, ulong nItemID, string pchPropertyName, long nValue)
        {
            return SteamEmulator.SteamInventory.SetProperty(handle, nItemID, pchPropertyName, nValue);
        }

        public bool SetProperty(IntPtr _, ulong handle, ulong nItemID, string pchPropertyName, float flValue)
        {
            return SteamEmulator.SteamInventory.SetProperty(handle, nItemID, pchPropertyName, flValue);
        }

        public bool SubmitUpdateProperties(IntPtr _, ulong handle, uint pResultHandle)
        {
            return SteamEmulator.SteamInventory.SubmitUpdateProperties(handle, pResultHandle);
        }

        public bool InspectItem(IntPtr _, uint pResultHandle, string pchItemToken)
        {
            return SteamEmulator.SteamInventory.InspectItem(pResultHandle, pchItemToken);
        }
    }
}
