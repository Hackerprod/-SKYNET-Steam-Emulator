using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamInventory
    {
                
        Result GetResultStatus(SteamInventoryResult_t resultHandle);

        bool GetResultItems(SteamInventoryResult_t resultHandle, [In, Out] SteamItemDetails_t[] pOutItemsArray, ref uint punOutItemsArraySize);

        bool GetResultItemProperty(SteamInventoryResult_t resultHandle, uint unItemIndex, string pchPropertyName, IntPtr pchValueBuffer, ref uint punValueBufferSizeOut);

        uint GetResultTimestamp(SteamInventoryResult_t resultHandle);

        bool CheckResultSteamID(SteamInventoryResult_t resultHandle, IntPtr steamIDExpected);

        void DestroyResult(SteamInventoryResult_t resultHandle);

        bool GetAllItems(ref SteamInventoryResult_t pResultHandle);

        bool GetItemsByID(ref SteamInventoryResult_t pResultHandle, ref uint pInstanceIDs, uint unCountInstanceIDs);

        bool SerializeResult(SteamInventoryResult_t resultHandle, IntPtr pOutBuffer, ref uint punOutBufferSize);

        bool DeserializeResult(ref SteamInventoryResult_t pOutResultHandle, IntPtr pBuffer, uint unBufferSize, [MarshalAs(UnmanagedType.U1)] bool bRESERVED_MUST_BE_FALSE);

        bool GenerateItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayItemDefs, [In, Out] uint[] punArrayQuantity, uint unArrayLength);

        bool GrantPromoItems(ref SteamInventoryResult_t pResultHandle);

        bool AddPromoItem(ref SteamInventoryResult_t pResultHandle, uint itemDef);

        bool AddPromoItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayItemDefs, uint unArrayLength);

        bool ConsumeItem(ref SteamInventoryResult_t pResultHandle, uint itemConsume, uint unQuantity);

        bool ExchangeItems(ref SteamInventoryResult_t pResultHandle, [In, Out] uint[] pArrayGenerate, [In, Out] uint[] punArrayGenerateQuantity, uint unArrayGenerateLength, [In, Out] uint[] pArrayDestroy, [In, Out] uint[] punArrayDestroyQuantity, uint unArrayDestroyLength);

        bool TransferItemQuantity(ref SteamInventoryResult_t pResultHandle, uint itemIdSource, uint unQuantity, uint itemIdDest);

        void SendItemDropHeartbeat(IntPtr self);

        bool TriggerItemDrop(ref SteamInventoryResult_t pResultHandle, uint dropListDefinition);

        bool TradeItems(ref SteamInventoryResult_t pResultHandle, IntPtr steamIDTradePartner, [In, Out] uint[] pArrayGive, [In, Out] uint[] pArrayGiveQuantity, uint nArrayGiveLength, [In, Out] uint[] pArrayGet, [In, Out] uint[] pArrayGetQuantity, uint nArrayGetLength);

        bool LoadItemDefinitions(IntPtr self);

        bool GetItemDefinitionIDs([In, Out] uint[] pItemDefIDs, ref uint punItemDefIDsArraySize);

        bool GetItemDefinitionProperty(uint iDefinition, string pchPropertyName, IntPtr pchValueBuffer, ref uint punValueBufferSizeOut);

        SteamAPICall_t RequestEligiblePromoItemDefinitionsIDs(IntPtr steamID);

        bool GetEligiblePromoItemDefinitionIDs(IntPtr steamID, [In, Out] uint[] pItemDefIDs, ref uint punItemDefIDsArraySize);
        
        SteamAPICall_t StartPurchase([In, Out] uint[] pArrayItemDefs, [In, Out] uint[] punArrayQuantity, uint unArrayLength);

        SteamAPICall_t RequestPrices(IntPtr self);

        uint GetNumItemsWithPrices(IntPtr self);

        bool GetItemsWithPrices([In, Out] uint[] pArrayItemDefs, [In, Out] ulong[] pCurrentPrices, [In, Out] ulong[] pBasePrices, uint unArrayLength);

        bool GetItemPrice(uint iDefinition, ref ulong pCurrentPrice, ref ulong pBasePrice);

        IntPtr StartUpdateProperties(IntPtr _);

        bool RemoveProperty(IntPtr handle, uint nItemID, string pchPropertyName);

        bool SetPropertyString(IntPtr handle, uint nItemID, string pchPropertyName, string pchPropertyValue);

        bool SetPropertyBool(IntPtr handle, uint nItemID, string pchPropertyName, [MarshalAs(UnmanagedType.U1)] bool bValue);

        bool SetPropertyInt64(IntPtr handle, uint nItemID, string pchPropertyName, long nValue);

        bool SetPropertyFloat(IntPtr handle, uint nItemID, string pchPropertyName, float flValue);

        bool SubmitUpdateProperties(IntPtr handle, ref SteamInventoryResult_t pResultHandle);

        bool InspectItem(ref SteamInventoryResult_t pResultHandle, string pchItemToken);

    }
    public struct SteamItemDetails_t
    {
        SteamItemInstanceID_t m_itemId;
        SteamItemDef_t m_iDefinition;
        uint m_unQuantity;
        uint m_unFlags; // see ESteamItemFlags
    };

}
