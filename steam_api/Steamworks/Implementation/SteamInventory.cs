using System;
using System.Runtime.InteropServices;
using System.Text;
using SKYNET.Managers;
using SKYNET.Steamworks;
using SKYNET.Steamworks.Interfaces;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    // ISteamInventory implementation. All out-pointers are IntPtr and honour the
    // two-call sizing contract; the authoritative store lives in InventoryManager.
    // Result handles are int (SDK), invalid = -1. Item instance ids are ulong.
    public class SteamInventory : ISteamInterface
    {
        public static SteamInventory Instance;

        private static readonly int ItemDetailsSize = Marshal.SizeOf(typeof(SteamItemDetails_t));

        public SteamInventory()
        {
            Instance = this;
            InterfaceName = "SteamInventory";
            InterfaceVersion = "STEAMINVENTORY_INTERFACE_V003";
            if (ItemDetailsSize != 16)
            {
                Write($"WARNING: SteamItemDetails_t marshals to {ItemDetailsSize} bytes, expected 16");
            }
            EnsureInit();
        }

        private static void EnsureInit()
        {
            InventoryManager.Initialize(SteamEmulator.AppID, (ulong)SteamEmulator.SteamID);
        }

        // ===================== results =====================

        public int GetResultStatus(int resultHandle)
        {
            return (int)InventoryManager.GetStatus(resultHandle);
        }

        public bool GetResultItems(int resultHandle, IntPtr pOutItemsArray, IntPtr punOutItemsArraySize)
        {
            var result = InventoryManager.GetResult(resultHandle);
            if (result == null)
            {
                return false;
            }

            int count = result.Items.Length;
            if (pOutItemsArray == IntPtr.Zero)
            {
                WriteU32(punOutItemsArraySize, count);
                return true;
            }

            int capacity = punOutItemsArraySize == IntPtr.Zero ? count : Marshal.ReadInt32(punOutItemsArraySize);
            if (capacity < count)
            {
                WriteU32(punOutItemsArraySize, count);
                return false;
            }

            for (int i = 0; i < count; i++)
            {
                var it = result.Items[i];
                var d = new SteamItemDetails_t
                {
                    m_itemId = it.ItemId,
                    m_iDefinition = it.DefId,
                    m_unQuantity = (ushort)Math.Min(it.Quantity, ushort.MaxValue),
                    m_unFlags = it.Flags,
                };
                Marshal.StructureToPtr(d, IntPtr.Add(pOutItemsArray, i * ItemDetailsSize), false);
            }
            WriteU32(punOutItemsArraySize, count);
            return true;
        }

        public bool GetResultItemProperty(int resultHandle, uint unItemIndex, string pchPropertyName, IntPtr pchValueBuffer, IntPtr punValueBufferSizeOut)
        {
            var result = InventoryManager.GetResult(resultHandle);
            if (result == null || unItemIndex >= (uint)result.Items.Length)
            {
                return false;
            }

            var props = result.Items[unItemIndex].Properties;
            string value;
            if (string.IsNullOrEmpty(pchPropertyName))
            {
                // Null/empty name => comma-separated list of property names.
                value = string.Join(",", props.Keys);
            }
            else if (!props.TryGetValue(pchPropertyName, out value))
            {
                // Unknown property => false, like GetItemDefinitionProperty.
                WriteU32(punValueBufferSizeOut, 0);
                return false;
            }

            return WriteStringOut(value, pchValueBuffer, punValueBufferSizeOut);
        }

        public uint GetResultTimestamp(int resultHandle)
        {
            return InventoryManager.GetResultTimestamp(resultHandle);
        }

        public bool CheckResultSteamID(int resultHandle, ulong steamIDExpected)
        {
            return InventoryManager.CheckResultSteamID(resultHandle, steamIDExpected);
        }

        public void DestroyResult(int resultHandle)
        {
            InventoryManager.DestroyResult(resultHandle);
        }

        // ===================== read =====================

        public bool GetAllItems(IntPtr pResultHandle)
        {
            EnsureInit();
            int h = InventoryManager.GetAllItems();
            return WriteResult(pResultHandle, h);
        }

        public bool GetItemsByID(IntPtr pResultHandle, IntPtr pInstanceIDs, uint unCountInstanceIDs)
        {
            EnsureInit();
            var ids = ReadUInt64Array(pInstanceIDs, unCountInstanceIDs);
            int h = InventoryManager.GetItemsByID(ids);
            return WriteResult(pResultHandle, h);
        }

        // ===================== serialize =====================

        public bool SerializeResult(int resultHandle, IntPtr pOutBuffer, IntPtr punOutBufferSize)
        {
            var blob = InventoryManager.SerializeResult(resultHandle);
            if (blob == null)
            {
                return false;
            }

            if (pOutBuffer == IntPtr.Zero)
            {
                WriteU32(punOutBufferSize, blob.Length);
                return true;
            }

            int capacity = punOutBufferSize == IntPtr.Zero ? blob.Length : Marshal.ReadInt32(punOutBufferSize);
            if (capacity < blob.Length)
            {
                WriteU32(punOutBufferSize, blob.Length);
                return false;
            }

            Marshal.Copy(blob, 0, pOutBuffer, blob.Length);
            WriteU32(punOutBufferSize, blob.Length);
            return true;
        }

        public bool DeserializeResult(IntPtr pOutResultHandle, IntPtr pBuffer, uint unBufferSize, bool bReserved = false)
        {
            EnsureInit();
            // The SDK requires the reserved flag to be false.
            if (bReserved)
            {
                WriteResultHandle(pOutResultHandle, InventoryManager.ResultInvalid);
                return false;
            }

            byte[] blob = null;
            if (pBuffer != IntPtr.Zero && unBufferSize > 0)
            {
                blob = new byte[unBufferSize];
                Marshal.Copy(pBuffer, blob, 0, (int)unBufferSize);
            }
            // ResultInvalid => corrupt/invalid buffer => false, per Steam contract.
            int h = InventoryManager.DeserializeResult(blob);
            return WriteResult(pOutResultHandle, h);
        }

        // ===================== mutations =====================

        public bool GenerateItems(IntPtr pResultHandle, IntPtr pArrayItemDefs, IntPtr punArrayQuantity, uint unArrayLength)
        {
            EnsureInit();
            var defs = ReadInt32Array(pArrayItemDefs, unArrayLength);
            var qty = ReadUInt32Array(punArrayQuantity, unArrayLength);
            int h = InventoryManager.Generate(defs, qty);
            return WriteResult(pResultHandle, h);
        }

        public bool GrantPromoItems(IntPtr pResultHandle)
        {
            EnsureInit();
            int h = InventoryManager.GrantPromoItems();
            return WriteResult(pResultHandle, h);
        }

        public bool AddPromoItem(IntPtr pResultHandle, int itemDef)
        {
            EnsureInit();
            int h = InventoryManager.AddPromoItem(itemDef);
            return WriteResult(pResultHandle, h);
        }

        public bool AddPromoItems(IntPtr pResultHandle, IntPtr pArrayItemDefs, uint unArrayLength)
        {
            EnsureInit();
            var defs = ReadInt32Array(pArrayItemDefs, unArrayLength);
            int h = InventoryManager.AddPromoItems(defs);
            return WriteResult(pResultHandle, h);
        }

        public bool ConsumeItem(IntPtr pResultHandle, ulong itemConsume, uint unQuantity)
        {
            EnsureInit();
            int h = InventoryManager.ConsumeItem(itemConsume, unQuantity);
            return WriteResult(pResultHandle, h);
        }

        public bool ExchangeItems(IntPtr pResultHandle, IntPtr pArrayGenerate, IntPtr punArrayGenerateQuantity, uint unArrayGenerateLength, IntPtr pArrayDestroy, IntPtr punArrayDestroyQuantity, uint unArrayDestroyLength)
        {
            EnsureInit();
            var genDefs = ReadInt32Array(pArrayGenerate, unArrayGenerateLength);
            var genQty = ReadUInt32Array(punArrayGenerateQuantity, unArrayGenerateLength);
            var destroyIds = ReadUInt64Array(pArrayDestroy, unArrayDestroyLength);
            var destroyQty = ReadUInt32Array(punArrayDestroyQuantity, unArrayDestroyLength);
            int h = InventoryManager.ExchangeItems(genDefs, genQty, destroyIds, destroyQty);
            return WriteResult(pResultHandle, h);
        }

        public bool TransferItemQuantity(IntPtr pResultHandle, ulong itemIdSource, uint unQuantity, ulong itemIdDest)
        {
            EnsureInit();
            int h = InventoryManager.TransferItemQuantity(itemIdSource, unQuantity, itemIdDest);
            return WriteResult(pResultHandle, h);
        }

        public bool TriggerItemDrop(IntPtr pResultHandle, int dropListDefinition)
        {
            EnsureInit();
            int h = InventoryManager.TriggerItemDrop(dropListDefinition);
            return WriteResult(pResultHandle, h);
        }

        public bool TradeItems(IntPtr pResultHandle, ulong steamIDTradePartner, IntPtr pArrayGive, IntPtr pArrayGiveQuantity, uint nArrayGiveLength, IntPtr pArrayGet, IntPtr pArrayGetQuantity, uint nArrayGetLength)
        {
            EnsureInit();
            // P2P trade is out of scope: no result handle, report false.
            WriteResultHandle(pResultHandle, InventoryManager.ResultInvalid);
            return false;
        }

        public bool InspectItem(IntPtr pResultHandle, string pchItemToken)
        {
            EnsureInit();
            int h = InventoryManager.GetItemsByID(Array.Empty<ulong>());
            return WriteResult(pResultHandle, h);
        }

        public void SendItemDropHeartbeat()
        {
        }

        // ===================== definitions =====================

        public bool LoadItemDefinitions()
        {
            EnsureInit();
            return InventoryManager.LoadItemDefinitions();
        }

        public bool GetItemDefinitionIDs(IntPtr pItemDefIDs, IntPtr punItemDefIDsArraySize)
        {
            EnsureInit();
            return WriteInt32ArrayOut(InventoryManager.GetDefinitionIds(), pItemDefIDs, punItemDefIDsArraySize);
        }

        public bool GetItemDefinitionProperty(int iDefinition, string pchPropertyName, IntPtr pchValueBuffer, IntPtr punValueBufferSizeOut)
        {
            EnsureInit();
            if (!InventoryManager.TryGetDefinitionProperty(iDefinition, pchPropertyName, out var value))
            {
                WriteU32(punValueBufferSizeOut, 0);
                return false;
            }
            return WriteStringOut(value, pchValueBuffer, punValueBufferSizeOut);
        }

        public SteamAPICall_t RequestEligiblePromoItemDefinitionsIDs(ulong steamID)
        {
            EnsureInit();
            return InventoryManager.RequestEligiblePromoItemDefinitionsIDs(steamID);
        }

        public bool GetEligiblePromoItemDefinitionIDs(ulong steamID, IntPtr pItemDefIDs, IntPtr punItemDefIDsArraySize)
        {
            EnsureInit();
            return WriteInt32ArrayOut(InventoryManager.GetPromoDefinitionIds(), pItemDefIDs, punItemDefIDsArraySize);
        }

        // ===================== prices =====================

        public SteamAPICall_t RequestPrices()
        {
            EnsureInit();
            return InventoryManager.RequestPrices();
        }

        public SteamAPICall_t StartPurchase(IntPtr pArrayItemDefs, IntPtr punArrayQuantity, uint unArrayLength)
        {
            EnsureInit();
            var defs = ReadInt32Array(pArrayItemDefs, unArrayLength);
            var qty = ReadUInt32Array(punArrayQuantity, unArrayLength);
            return InventoryManager.StartPurchase(defs, qty);
        }

        public uint GetNumItemsWithPrices()
        {
            EnsureInit();
            return InventoryManager.GetNumItemsWithPrices();
        }

        // v003: separate current + base price arrays.
        public bool GetItemsWithPrices(IntPtr pArrayItemDefs, IntPtr pCurrentPrices, IntPtr pBasePrices, uint unArrayLength)
        {
            EnsureInit();
            var priced = InventoryManager.GetPricedDefinitions();
            int n = Math.Min((int)unArrayLength, priced.Length);
            for (int i = 0; i < n; i++)
            {
                if (pArrayItemDefs != IntPtr.Zero) Marshal.WriteInt32(IntPtr.Add(pArrayItemDefs, i * 4), priced[i].DefId);
                if (pCurrentPrices != IntPtr.Zero) Marshal.WriteInt64(IntPtr.Add(pCurrentPrices, i * 8), (long)priced[i].PriceCents);
                if (pBasePrices != IntPtr.Zero) Marshal.WriteInt64(IntPtr.Add(pBasePrices, i * 8), (long)priced[i].PriceCents);
            }
            return true;
        }

        // v002: single prices array.
        public bool GetItemsWithPricesV2(IntPtr pArrayItemDefs, IntPtr pPrices, uint unArrayLength)
        {
            EnsureInit();
            var priced = InventoryManager.GetPricedDefinitions();
            int n = Math.Min((int)unArrayLength, priced.Length);
            for (int i = 0; i < n; i++)
            {
                if (pArrayItemDefs != IntPtr.Zero) Marshal.WriteInt32(IntPtr.Add(pArrayItemDefs, i * 4), priced[i].DefId);
                if (pPrices != IntPtr.Zero) Marshal.WriteInt64(IntPtr.Add(pPrices, i * 8), (long)priced[i].PriceCents);
            }
            return true;
        }

        // v003: current + base out-pointers.
        public bool GetItemPrice(int iDefinition, IntPtr pCurrentPrice, IntPtr pBasePrice)
        {
            EnsureInit();
            if (!InventoryManager.TryGetItemPrice(iDefinition, out var current, out var basePrice))
            {
                return false;
            }
            if (pCurrentPrice != IntPtr.Zero) Marshal.WriteInt64(pCurrentPrice, (long)current);
            if (pBasePrice != IntPtr.Zero) Marshal.WriteInt64(pBasePrice, (long)basePrice);
            return true;
        }

        // v002: single price out-pointer.
        public bool GetItemPriceV2(int iDefinition, IntPtr pPrice)
        {
            EnsureInit();
            if (!InventoryManager.TryGetItemPrice(iDefinition, out var current, out _))
            {
                return false;
            }
            if (pPrice != IntPtr.Zero) Marshal.WriteInt64(pPrice, (long)current);
            return true;
        }

        // ===================== property updates =====================

        public ulong StartUpdateProperties()
        {
            EnsureInit();
            return InventoryManager.StartUpdateProperties();
        }

        public bool RemoveProperty(ulong handle, ulong nItemID, string pchPropertyName)
        {
            return InventoryManager.RemoveProperty(handle, nItemID, pchPropertyName);
        }

        public bool SetPropertyString(ulong handle, ulong nItemID, string pchPropertyName, string pchPropertyValue)
        {
            return InventoryManager.SetProperty(handle, nItemID, pchPropertyName, pchPropertyValue);
        }

        public bool SetPropertyBool(ulong handle, ulong nItemID, string pchPropertyName, bool bValue)
        {
            return InventoryManager.SetProperty(handle, nItemID, pchPropertyName, bValue ? "true" : "false");
        }

        public bool SetPropertyInt64(ulong handle, ulong nItemID, string pchPropertyName, long nValue)
        {
            return InventoryManager.SetProperty(handle, nItemID, pchPropertyName, nValue.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public bool SetPropertyFloat(ulong handle, ulong nItemID, string pchPropertyName, float flValue)
        {
            return InventoryManager.SetProperty(handle, nItemID, pchPropertyName, flValue.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public bool SubmitUpdateProperties(ulong handle, IntPtr pResultHandle)
        {
            EnsureInit();
            int h = InventoryManager.SubmitUpdateProperties(handle);
            return WriteResult(pResultHandle, h);
        }

        // ===================== marshalling helpers =====================

        private static void WriteU32(IntPtr destination, int value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteInt32(destination, value);
            }
        }

        private static void WriteResultHandle(IntPtr destination, int handle)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteInt32(destination, handle);
            }
        }

        // Writes the handle and reports success: false when the manager refused the
        // op (disabled, invalid) by returning ResultInvalid.
        private static bool WriteResult(IntPtr destination, int handle)
        {
            WriteResultHandle(destination, handle);
            return handle != InventoryManager.ResultInvalid;
        }

        private static int[] ReadInt32Array(IntPtr source, uint length)
        {
            if (source == IntPtr.Zero || length == 0)
            {
                return Array.Empty<int>();
            }
            var a = new int[length];
            Marshal.Copy(source, a, 0, (int)length);
            return a;
        }

        private static uint[] ReadUInt32Array(IntPtr source, uint length)
        {
            var signed = ReadInt32Array(source, length);
            var a = new uint[signed.Length];
            for (int i = 0; i < signed.Length; i++)
            {
                a[i] = unchecked((uint)signed[i]);
            }
            return a;
        }

        private static ulong[] ReadUInt64Array(IntPtr source, uint length)
        {
            if (source == IntPtr.Zero || length == 0)
            {
                return Array.Empty<ulong>();
            }
            var signed = new long[length];
            Marshal.Copy(source, signed, 0, (int)length);
            var a = new ulong[length];
            for (int i = 0; i < signed.Length; i++)
            {
                a[i] = unchecked((ulong)signed[i]);
            }
            return a;
        }

        private static bool WriteInt32ArrayOut(int[] values, IntPtr pOut, IntPtr punSize)
        {
            int count = values.Length;
            if (pOut == IntPtr.Zero)
            {
                WriteU32(punSize, count);
                return true;
            }

            int capacity = punSize == IntPtr.Zero ? count : Marshal.ReadInt32(punSize);
            if (capacity < count)
            {
                WriteU32(punSize, count);
                return false;
            }

            if (count > 0)
            {
                Marshal.Copy(values, 0, pOut, count);
            }
            WriteU32(punSize, count);
            return true;
        }

        private static bool WriteStringOut(string value, IntPtr buffer, IntPtr punSize)
        {
            var bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
            int need = bytes.Length + 1; // include null terminator

            if (buffer == IntPtr.Zero)
            {
                WriteU32(punSize, need);
                return true;
            }

            int capacity = punSize == IntPtr.Zero ? 0 : Marshal.ReadInt32(punSize);
            if (capacity < need)
            {
                WriteU32(punSize, need);
                return false;
            }

            if (bytes.Length > 0)
            {
                Marshal.Copy(bytes, 0, buffer, bytes.Length);
            }
            Marshal.WriteByte(buffer, bytes.Length, 0);
            WriteU32(punSize, need);
            return true;
        }
    }
}
