using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Helpers.JSON;
using SKYNET.Steamworks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Managers
{
    /// <summary>
    /// Authoritative, local-first inventory store for ISteamInventory. Items live
    /// in a plain Dictionary under a single lock (compound ops stay atomic);
    /// transient result/update handles use concurrent maps. Persisted to disk so
    /// inventories survive restarts. Server sync is optional and never blocks the
    /// game thread.
    /// </summary>
    public static class InventoryManager
    {
        // ---- SDK sentinels ----
        public const int ResultInvalid = -1;               // k_SteamInventoryResultInvalid
        public const ulong ItemInstanceInvalid = 0xFFFFFFFFFFFFFFFFUL;
        public const ulong UpdateHandleInvalid = 0xFFFFFFFFFFFFFFFFUL;

        // ---- Config (defaults; overridable via ApplyConfig) ----
        public static bool Enabled = true;
        public static bool AutoGrantPurchases = true;
        public static bool AutoGrantPromos = true;
        public static bool AllowGenerate = true;
        public static string Currency = "USD";

        // ---- Authoritative store: Dictionary + single lock ----
        private static readonly object StoreLock = new object();
        private static readonly Dictionary<ulong, InventoryItem> Items = new Dictionary<ulong, InventoryItem>();
        private static readonly Dictionary<int, ItemDefinition> Definitions = new Dictionary<int, ItemDefinition>();

        // ---- Transient handles: concurrent ----
        private static readonly ConcurrentDictionary<int, InventoryResult> Results = new ConcurrentDictionary<int, InventoryResult>();
        private static readonly ConcurrentDictionary<ulong, PropertyUpdate> Updates = new ConcurrentDictionary<ulong, PropertyUpdate>();
        private static readonly ConcurrentDictionary<int, uint> DropCooldowns = new ConcurrentDictionary<int, uint>();

        private static int _nextResultHandle = 1;          // 0 is a valid SDK handle; we simply start at 1
        private static long _nextUpdateHandle = 1;
        private static long _nextItemId = 1;               // persisted in meta.json
        private static volatile bool _definitionsLoaded;
        private static volatile bool _initialized;

        private static uint _appId;
        private static ulong _owner;
        private static string _root;
        private static byte[] _serializeSecret;

        public static bool DefinitionsLoaded => _definitionsLoaded;

        // ================= lifecycle / persistence =================

        public static void Initialize(uint appId, ulong steamId)
        {
            if (_initialized)
            {
                return;
            }

            _appId = appId;
            _owner = steamId;
            _root = Path.Combine(Common.GetPath(), "SKYNET", "Inventory", appId.ToString());

            try
            {
                Common.EnsureDirectoryExists(_root);
                LoadMeta();
                LoadItems();
                LoadDefinitionsFromDisk();
                EnsureSecret();
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("InventoryManager", $"Initialize error: {ex.Message}");
            }

            _initialized = true;
        }

        public static void ApplyConfig(bool enabled, bool autoPurchase, bool autoPromo, bool allowGenerate, string currency)
        {
            Enabled = enabled;
            AutoGrantPurchases = autoPurchase;
            AutoGrantPromos = autoPromo;
            AllowGenerate = allowGenerate;
            if (!string.IsNullOrWhiteSpace(currency))
            {
                Currency = currency;
            }
        }

        private static string ItemsPath => Path.Combine(_root, "items.json");
        private static string MetaPath => Path.Combine(_root, "meta.json");
        private static string DefsPath => Path.Combine(_root, "defs.json");
        private static string SecretPath => Path.Combine(Common.GetPath(), "SKYNET", "Inventory", "serialize.key");

        private static void LoadMeta()
        {
            if (!File.Exists(MetaPath))
            {
                return;
            }

            var meta = File.ReadAllText(MetaPath).FromJson<InventoryMetaDto>();
            if (meta != null && meta.NextItemId > 0)
            {
                _nextItemId = meta.NextItemId;
            }
        }

        private static void PersistMeta()
        {
            try
            {
                AtomicWriteText(MetaPath, new InventoryMetaDto { NextItemId = Interlocked.Read(ref _nextItemId), Version = 1 }.ToJson());
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("InventoryManager", $"PersistMeta error: {ex.Message}");
            }
        }

        private static void LoadItems()
        {
            if (!File.Exists(ItemsPath))
            {
                return;
            }

            var dto = File.ReadAllText(ItemsPath).FromJson<InventoryItemsDto>();
            if (dto?.Items == null)
            {
                return;
            }

            lock (StoreLock)
            {
                Items.Clear();
                foreach (var it in dto.Items)
                {
                    if (it == null || it.ItemId == 0)
                    {
                        continue;
                    }
                    it.Properties = it.Properties ?? new Dictionary<string, string>(StringComparer.Ordinal);
                    Items[it.ItemId] = it;
                }
            }
        }

        private static void PersistItems()
        {
            try
            {
                InventoryItemsDto dto;
                lock (StoreLock)
                {
                    dto = new InventoryItemsDto { Items = Items.Values.ToList() };
                }
                AtomicWriteText(ItemsPath, dto.ToJson());
                PersistMeta();
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("InventoryManager", $"PersistItems error: {ex.Message}");
            }
        }

        private static void LoadDefinitionsFromDisk()
        {
            if (!File.Exists(DefsPath))
            {
                return;
            }

            var dto = File.ReadAllText(DefsPath).FromJson<ItemDefinitionsDto>();
            if (dto?.Definitions == null)
            {
                return;
            }

            lock (StoreLock)
            {
                Definitions.Clear();
                foreach (var d in dto.Definitions)
                {
                    if (d == null)
                    {
                        continue;
                    }
                    d.Raw = d.Raw ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    Definitions[d.DefId] = d;
                }
            }
            _definitionsLoaded = true;
        }

        private static void AtomicWriteText(string path, string content)
        {
            var temp = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            File.WriteAllText(temp, content, new UTF8Encoding(false));
            if (File.Exists(path))
            {
                File.Replace(temp, path, null);
            }
            else
            {
                File.Move(temp, path);
            }
        }

        // ================= id / handle allocation =================

        private static ulong NextItemId()
        {
            var id = (ulong)Interlocked.Increment(ref _nextItemId);
            return id;
        }

        private static int NextResultHandle()
        {
            // Skip the invalid sentinel; wrap safely.
            int h = Interlocked.Increment(ref _nextResultHandle);
            if (h == ResultInvalid || h < 0)
            {
                h = Interlocked.Increment(ref _nextResultHandle);
            }
            return h;
        }

        // ================= result lifecycle =================

        // Creates a completed result and posts SteamInventoryResultReady_t (and,
        // for full-inventory results, SteamInventoryFullUpdate_t).
        private static int MakeResult(EResult status, InventoryItem[] items, bool fullUpdate)
        {
            int handle = NextResultHandle();
            var result = new InventoryResult
            {
                Handle = handle,
                Status = status,
                TimestampUnix = NowUnix(),
                OwnerSteamID = _owner,
                Items = items ?? Array.Empty<InventoryItem>(),
            };
            Results[handle] = result;

            CallbackManager.AddCallback(new SteamInventoryResultReady_t { Handle = handle, Result = status });
            if (fullUpdate && status == EResult.k_EResultOK)
            {
                CallbackManager.AddCallback(new SteamInventoryFullUpdate_t { Handle = handle });
            }
            return handle;
        }

        public static EResult GetStatus(int handle)
        {
            if (Results.TryGetValue(handle, out var r))
            {
                return r.Status;
            }
            // Unknown handle: never report OK.
            return EResult.k_EResultInvalidParam;
        }

        public static InventoryResult GetResult(int handle)
        {
            Results.TryGetValue(handle, out var r);
            return r;
        }

        public static uint GetResultTimestamp(int handle)
        {
            return Results.TryGetValue(handle, out var r) ? r.TimestampUnix : 0;
        }

        public static bool CheckResultSteamID(int handle, ulong expected)
        {
            return Results.TryGetValue(handle, out var r) && r.OwnerSteamID == expected;
        }

        public static void DestroyResult(int handle)
        {
            Results.TryRemove(handle, out _);
        }

        // ================= read ops =================

        public static int GetAllItems()
        {
            InventoryItem[] snapshot;
            lock (StoreLock)
            {
                snapshot = Items.Values.Where(i => (i.Flags & (ushort)SteamItemFlags.Removed) == 0)
                                       .Select(Clone).ToArray();
            }
            return MakeResult(EResult.k_EResultOK, snapshot, fullUpdate: true);
        }

        public static int GetItemsByID(ulong[] ids)
        {
            InventoryItem[] snapshot;
            lock (StoreLock)
            {
                snapshot = (ids ?? Array.Empty<ulong>())
                    .Where(id => Items.ContainsKey(id))
                    .Select(id => Clone(Items[id]))
                    .ToArray();
            }
            return MakeResult(EResult.k_EResultOK, snapshot, fullUpdate: false);
        }

        // ================= mutations =================

        public static int Generate(int[] defs, uint[] qtys)
        {
            if (!AllowGenerate)
            {
                return MakeResult(EResult.k_EResultFail, null, false);
            }
            return GrantDefs(defs, qtys, requirePromo: false);
        }

        public static int GrantPromoItems()
        {
            int[] promo;
            lock (StoreLock)
            {
                promo = Definitions.Values.Where(d => d.Promo).Select(d => d.DefId).ToArray();
            }
            return GrantDefs(promo, null, requirePromo: true);
        }

        public static int AddPromoItem(int def)
        {
            return GrantDefs(new[] { def }, null, requirePromo: true);
        }

        public static int AddPromoItems(int[] defs)
        {
            return GrantDefs(defs, null, requirePromo: true);
        }

        private static int GrantDefs(int[] defs, uint[] qtys, bool requirePromo)
        {
            if (defs == null || defs.Length == 0)
            {
                return MakeResult(EResult.k_EResultOK, Array.Empty<InventoryItem>(), false);
            }

            var granted = new List<InventoryItem>();
            lock (StoreLock)
            {
                for (int i = 0; i < defs.Length; i++)
                {
                    int def = defs[i];
                    uint qty = qtys != null && i < qtys.Length ? qtys[i] : 1u;
                    if (qty == 0)
                    {
                        qty = 1;
                    }

                    if (requirePromo && Definitions.TryGetValue(def, out var d) && !d.Promo)
                    {
                        continue;
                    }

                    var item = new InventoryItem
                    {
                        ItemId = NextItemId(),
                        DefId = def,
                        Quantity = qty,
                        Flags = 0,
                        AcquiredUnix = NowUnix(),
                        Properties = new Dictionary<string, string>(StringComparer.Ordinal),
                    };
                    Items[item.ItemId] = item;
                    granted.Add(Clone(item));
                }
            }

            PersistItems();
            return MakeResult(EResult.k_EResultOK, granted.ToArray(), fullUpdate: true);
        }

        public static int ConsumeItem(ulong itemId, uint quantity)
        {
            InventoryItem snapshot = null;
            EResult status = EResult.k_EResultFail;
            lock (StoreLock)
            {
                if (Items.TryGetValue(itemId, out var item))
                {
                    if (quantity == 0 || quantity >= item.Quantity)
                    {
                        item.Quantity = 0;
                        item.Flags |= (ushort)SteamItemFlags.Consumed;
                        Items.Remove(itemId);
                    }
                    else
                    {
                        item.Quantity -= quantity;
                    }
                    snapshot = Clone(item);
                    status = EResult.k_EResultOK;
                }
            }

            if (status == EResult.k_EResultOK)
            {
                PersistItems();
            }
            return MakeResult(status, snapshot == null ? Array.Empty<InventoryItem>() : new[] { snapshot }, fullUpdate: status == EResult.k_EResultOK);
        }

        public static int TransferItemQuantity(ulong src, uint quantity, ulong dest)
        {
            EResult status = EResult.k_EResultFail;
            var affected = new List<InventoryItem>();
            lock (StoreLock)
            {
                if (Items.TryGetValue(src, out var s) && s.Quantity >= quantity && quantity > 0)
                {
                    s.Quantity -= quantity;
                    if (dest == ItemInstanceInvalid || !Items.TryGetValue(dest, out var d))
                    {
                        // New stack from the source definition.
                        d = new InventoryItem
                        {
                            ItemId = NextItemId(),
                            DefId = s.DefId,
                            Quantity = 0,
                            AcquiredUnix = NowUnix(),
                            Properties = new Dictionary<string, string>(StringComparer.Ordinal),
                        };
                        Items[d.ItemId] = d;
                    }

                    if (d.DefId == s.DefId)
                    {
                        d.Quantity += quantity;
                        if (s.Quantity == 0)
                        {
                            Items.Remove(src);
                        }
                        affected.Add(Clone(d));
                        if (Items.ContainsKey(src))
                        {
                            affected.Add(Clone(s));
                        }
                        status = EResult.k_EResultOK;
                    }
                    else
                    {
                        s.Quantity += quantity; // rollback
                    }
                }
            }

            if (status == EResult.k_EResultOK)
            {
                PersistItems();
            }
            return MakeResult(status, affected.ToArray(), fullUpdate: status == EResult.k_EResultOK);
        }

        public static int ExchangeItems(int[] genDefs, uint[] genQty, ulong[] destroyIds, uint[] destroyQty)
        {
            EResult status = EResult.k_EResultFail;
            var outItems = new List<InventoryItem>();
            lock (StoreLock)
            {
                // Validate all destroy inputs are present with enough quantity.
                bool ok = true;
                if (destroyIds != null)
                {
                    for (int i = 0; i < destroyIds.Length; i++)
                    {
                        uint need = destroyQty != null && i < destroyQty.Length ? destroyQty[i] : 1u;
                        if (!Items.TryGetValue(destroyIds[i], out var it) || it.Quantity < need)
                        {
                            ok = false;
                            break;
                        }
                    }
                }

                if (ok)
                {
                    if (destroyIds != null)
                    {
                        for (int i = 0; i < destroyIds.Length; i++)
                        {
                            uint need = destroyQty != null && i < destroyQty.Length ? destroyQty[i] : 1u;
                            var it = Items[destroyIds[i]];
                            it.Quantity = need >= it.Quantity ? 0 : it.Quantity - need;
                            if (it.Quantity == 0)
                            {
                                Items.Remove(destroyIds[i]);
                            }
                        }
                    }

                    if (genDefs != null)
                    {
                        for (int i = 0; i < genDefs.Length; i++)
                        {
                            uint qty = genQty != null && i < genQty.Length ? genQty[i] : 1u;
                            var item = new InventoryItem
                            {
                                ItemId = NextItemId(),
                                DefId = genDefs[i],
                                Quantity = qty == 0 ? 1u : qty,
                                AcquiredUnix = NowUnix(),
                                Properties = new Dictionary<string, string>(StringComparer.Ordinal),
                            };
                            Items[item.ItemId] = item;
                            outItems.Add(Clone(item));
                        }
                    }
                    status = EResult.k_EResultOK;
                }
            }

            if (status == EResult.k_EResultOK)
            {
                PersistItems();
            }
            return MakeResult(status, outItems.ToArray(), fullUpdate: status == EResult.k_EResultOK);
        }

        public static int TriggerItemDrop(int dropListDef)
        {
            // Rate-limit per droplist: at most one drop per 60s.
            uint now = NowUnix();
            if (DropCooldowns.TryGetValue(dropListDef, out var last) && now - last < 60)
            {
                return MakeResult(EResult.k_EResultOK, Array.Empty<InventoryItem>(), false);
            }
            DropCooldowns[dropListDef] = now;

            int[] pool;
            lock (StoreLock)
            {
                pool = Definitions.TryGetValue(dropListDef, out var d)
                    ? d.DropList.ToArray()
                    : Array.Empty<int>();
            }
            if (pool.Length == 0)
            {
                return MakeResult(EResult.k_EResultOK, Array.Empty<InventoryItem>(), false);
            }

            int pick = pool[new Random(unchecked((int)(now ^ (uint)dropListDef))).Next(pool.Length)];
            return GrantDefs(new[] { pick }, null, requirePromo: false);
        }

        // ================= property updates =================

        public static ulong StartUpdateProperties()
        {
            var handle = (ulong)Interlocked.Increment(ref _nextUpdateHandle);
            Updates[handle] = new PropertyUpdate();
            return handle;
        }

        public static bool SetProperty(ulong handle, ulong itemId, string name, string value)
        {
            if (!Updates.TryGetValue(handle, out var u) || string.IsNullOrEmpty(name))
            {
                return false;
            }
            u.Set(itemId, name, value);
            return true;
        }

        public static bool RemoveProperty(ulong handle, ulong itemId, string name)
        {
            if (!Updates.TryGetValue(handle, out var u) || string.IsNullOrEmpty(name))
            {
                return false;
            }
            u.Remove(itemId, name);
            return true;
        }

        public static int SubmitUpdateProperties(ulong handle)
        {
            if (!Updates.TryRemove(handle, out var u))
            {
                return MakeResult(EResult.k_EResultInvalidParam, null, false);
            }

            var affected = new List<InventoryItem>();
            lock (StoreLock)
            {
                foreach (var change in u.Changes)
                {
                    if (!Items.TryGetValue(change.ItemId, out var item))
                    {
                        continue;
                    }
                    if (change.Remove)
                    {
                        item.Properties.Remove(change.Name);
                    }
                    else
                    {
                        item.Properties[change.Name] = change.Value ?? string.Empty;
                    }
                    affected.Add(Clone(item));
                }
            }

            PersistItems();
            return MakeResult(EResult.k_EResultOK, affected.ToArray(), fullUpdate: true);
        }

        // ================= definitions =================

        public static bool LoadItemDefinitions()
        {
            WorkQueue.Enqueue("Inventory LoadItemDefinitions", () =>
            {
                try
                {
                    LoadDefinitionsFromDisk();
                }
                catch (Exception ex)
                {
                    SteamEmulator.Write("InventoryManager", $"LoadItemDefinitions error: {ex.Message}");
                }
                _definitionsLoaded = true;
                CallbackManager.AddCallback(new SteamInventoryDefinitionUpdate_t());
            }, "inventory:load-defs");
            return true;
        }

        public static int[] GetDefinitionIds()
        {
            lock (StoreLock)
            {
                return Definitions.Keys.ToArray();
            }
        }

        public static int[] GetPromoDefinitionIds()
        {
            lock (StoreLock)
            {
                return Definitions.Values.Where(d => d.Promo).Select(d => d.DefId).ToArray();
            }
        }

        public static bool TryGetDefinitionProperty(int def, string propertyName, out string value)
        {
            value = string.Empty;
            lock (StoreLock)
            {
                if (!Definitions.TryGetValue(def, out var d))
                {
                    return false;
                }

                // Null/empty name => comma-separated list of property names (SDK behaviour).
                if (string.IsNullOrEmpty(propertyName))
                {
                    value = string.Join(",", d.Raw.Keys);
                    return true;
                }

                return d.Raw.TryGetValue(propertyName, out value);
            }
        }

        // ================= prices =================

        public static SteamAPICall_t RequestPrices()
        {
            var currency = Encoding.ASCII.GetBytes((Currency ?? "USD").PadRight(4, '\0').Substring(0, 4));
            return CallbackManager.AddCallbackResult(new SteamInventoryRequestPricesResult_t
            {
                Result = EResult.k_EResultOK,
                Currency = currency,
            });
        }

        public static uint GetNumItemsWithPrices()
        {
            lock (StoreLock)
            {
                return (uint)Definitions.Values.Count(d => d.PriceCents > 0);
            }
        }

        public static bool TryGetItemPrice(int def, out ulong current, out ulong basePrice)
        {
            current = 0;
            basePrice = 0;
            lock (StoreLock)
            {
                if (Definitions.TryGetValue(def, out var d) && d.PriceCents > 0)
                {
                    current = d.PriceCents;
                    basePrice = d.PriceCents;
                    return true;
                }
            }
            return false;
        }

        public static ItemDefinition[] GetPricedDefinitions()
        {
            lock (StoreLock)
            {
                return Definitions.Values.Where(d => d.PriceCents > 0).ToArray();
            }
        }

        // ================= purchase / promo (async call results) =================

        public static SteamAPICall_t StartPurchase(int[] defs, uint[] qtys)
        {
            ulong orderId = (ulong)DateTime.UtcNow.Ticks;
            ulong transId = orderId ^ 0x5A5A5A5AUL;

            if (AutoGrantPurchases && defs != null && defs.Length > 0)
            {
                // Grant locally since there is no real payment path.
                GrantDefs(defs, qtys, requirePromo: false);
            }

            return CallbackManager.AddCallbackResult(new SteamInventoryStartPurchaseResult_t
            {
                Result = AutoGrantPurchases ? EResult.k_EResultOK : EResult.k_EResultFail,
                OrderID = orderId,
                TransID = transId,
            });
        }

        public static SteamAPICall_t RequestEligiblePromoItemDefinitionsIDs(ulong steamID)
        {
            int count;
            lock (StoreLock)
            {
                count = Definitions.Values.Count(d => d.Promo);
            }
            return CallbackManager.AddCallbackResult(new SteamInventoryEligiblePromoItemDefIDs_t
            {
                Result = EResult.k_EResultOK,
                SteamID = steamID,
                UmEligiblePromoItemDefs = count,
                CachedData = false,
            });
        }

        // ================= serialize / deserialize (HMAC-signed) =================

        private static readonly byte[] SerializeMagic = { (byte)'S', (byte)'K', (byte)'I', (byte)'V' };
        private const ushort SerializeVersion = 1;

        private static void EnsureSecret()
        {
            try
            {
                if (File.Exists(SecretPath))
                {
                    _serializeSecret = File.ReadAllBytes(SecretPath);
                    if (_serializeSecret != null && _serializeSecret.Length == 32)
                    {
                        return;
                    }
                }

                _serializeSecret = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(_serializeSecret);
                }
                Common.EnsureDirectoryExists(SecretPath, true);
                File.WriteAllBytes(SecretPath, _serializeSecret);
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("InventoryManager", $"EnsureSecret error: {ex.Message}");
                _serializeSecret = _serializeSecret ?? new byte[32];
            }
        }

        public static byte[] SerializeResult(int handle)
        {
            if (!Results.TryGetValue(handle, out var r))
            {
                return null;
            }
            if (r.SerializedBlob != null)
            {
                return r.SerializedBlob;
            }

            using (var ms = new MemoryStream())
            using (var w = new BinaryWriter(ms))
            {
                w.Write(SerializeMagic);
                w.Write(SerializeVersion);
                w.Write(_appId);
                w.Write(r.OwnerSteamID);
                w.Write((ulong)r.TimestampUnix);
                w.Write(r.Items.Length);
                foreach (var it in r.Items)
                {
                    w.Write(it.ItemId);
                    w.Write(it.DefId);
                    w.Write((ushort)Math.Min(it.Quantity, ushort.MaxValue));
                    w.Write(it.Flags);
                }

                var body = ms.ToArray();
                var mac = ComputeHmac(body);
                var blob = new byte[body.Length + mac.Length];
                Buffer.BlockCopy(body, 0, blob, 0, body.Length);
                Buffer.BlockCopy(mac, 0, blob, body.Length, mac.Length);
                r.SerializedBlob = blob;
                return blob;
            }
        }

        // Parses a signed blob into a NEW result handle. On any failure returns a
        // result handle with k_EResultFail (never a bare false), per SDK expectation.
        public static int DeserializeResult(byte[] blob)
        {
            try
            {
                if (blob == null || blob.Length < 4 + 2 + 4 + 8 + 8 + 4 + 32)
                {
                    return MakeResult(EResult.k_EResultFail, null, false);
                }

                int macOffset = blob.Length - 32;
                var body = new byte[macOffset];
                Buffer.BlockCopy(blob, 0, body, 0, macOffset);
                var mac = new byte[32];
                Buffer.BlockCopy(blob, macOffset, mac, 0, 32);

                var expected = ComputeHmac(body);
                if (!ConstantTimeEquals(expected, mac))
                {
                    return MakeResult(EResult.k_EResultFail, null, false);
                }

                using (var ms = new MemoryStream(body))
                using (var rd = new BinaryReader(ms))
                {
                    var magic = rd.ReadBytes(4);
                    if (magic.Length != 4 || magic[0] != 'S' || magic[1] != 'K' || magic[2] != 'I' || magic[3] != 'V')
                    {
                        return MakeResult(EResult.k_EResultFail, null, false);
                    }
                    ushort ver = rd.ReadUInt16();
                    if (ver != SerializeVersion)
                    {
                        return MakeResult(EResult.k_EResultFail, null, false);
                    }
                    uint appId = rd.ReadUInt32();
                    if (appId != _appId)
                    {
                        return MakeResult(EResult.k_EResultFail, null, false);
                    }
                    ulong owner = rd.ReadUInt64();
                    uint ts = (uint)rd.ReadUInt64();
                    int count = rd.ReadInt32();
                    if (count < 0 || count > 100000)
                    {
                        return MakeResult(EResult.k_EResultFail, null, false);
                    }

                    var items = new InventoryItem[count];
                    for (int i = 0; i < count; i++)
                    {
                        items[i] = new InventoryItem
                        {
                            ItemId = rd.ReadUInt64(),
                            DefId = rd.ReadInt32(),
                            Quantity = rd.ReadUInt16(),
                            Flags = rd.ReadUInt16(),
                            Properties = new Dictionary<string, string>(StringComparer.Ordinal),
                        };
                    }

                    int handle = NextResultHandle();
                    Results[handle] = new InventoryResult
                    {
                        Handle = handle,
                        Status = EResult.k_EResultOK,
                        TimestampUnix = ts,
                        OwnerSteamID = owner,
                        Items = items,
                    };
                    CallbackManager.AddCallback(new SteamInventoryResultReady_t { Handle = handle, Result = EResult.k_EResultOK });
                    return handle;
                }
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("InventoryManager", $"DeserializeResult error: {ex.Message}");
                return MakeResult(EResult.k_EResultFail, null, false);
            }
        }

        private static byte[] ComputeHmac(byte[] body)
        {
            using (var hmac = new HMACSHA256(_serializeSecret ?? new byte[32]))
            {
                return hmac.ComputeHash(body);
            }
        }

        private static bool ConstantTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        // ================= helpers =================

        private static InventoryItem Clone(InventoryItem i)
        {
            return new InventoryItem
            {
                ItemId = i.ItemId,
                DefId = i.DefId,
                Quantity = i.Quantity,
                Flags = i.Flags,
                AcquiredUnix = i.AcquiredUnix,
                Properties = new Dictionary<string, string>(i.Properties ?? new Dictionary<string, string>(), StringComparer.Ordinal),
            };
        }

        private static uint NowUnix()
        {
            return (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        // ================= nested state =================

        private sealed class PropertyUpdate
        {
            public readonly List<PropChange> Changes = new List<PropChange>();

            public void Set(ulong itemId, string name, string value)
            {
                Changes.Add(new PropChange { ItemId = itemId, Name = name, Value = value, Remove = false });
            }

            public void Remove(ulong itemId, string name)
            {
                Changes.Add(new PropChange { ItemId = itemId, Name = name, Remove = true });
            }
        }

        private struct PropChange
        {
            public ulong ItemId;
            public string Name;
            public string Value;
            public bool Remove;
        }

        // ================= persistence DTOs =================

        public sealed class InventoryMetaDto
        {
            public long NextItemId { get; set; }
            public int Version { get; set; }
        }

        public sealed class InventoryItemsDto
        {
            public List<InventoryItem> Items { get; set; }
        }

        public sealed class ItemDefinitionsDto
        {
            public List<ItemDefinition> Definitions { get; set; }
        }
    }

    // ================= model =================

    public sealed class InventoryItem
    {
        public ulong ItemId { get; set; }
        public int DefId { get; set; }
        public uint Quantity { get; set; }
        public ushort Flags { get; set; }
        public uint AcquiredUnix { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>(StringComparer.Ordinal);
    }

    // Flexible definition: Raw is the source of truth; typed fields are derived.
    public sealed class ItemDefinition
    {
        public int DefId { get; set; }
        public Dictionary<string, string> Raw { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string Type => Get("type");
        public bool Promo => GetBool("promo");
        public bool Tradable => GetBool("tradable");
        public bool Marketable => GetBool("marketable");
        public ulong PriceCents => GetULong("price");

        public List<int> DropList
        {
            get
            {
                var raw = Get("droplist");
                if (string.IsNullOrWhiteSpace(raw))
                {
                    return new List<int>();
                }
                var list = new List<int>();
                foreach (var part in raw.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (int.TryParse(part.Trim(), out var v))
                    {
                        list.Add(v);
                    }
                }
                return list;
            }
        }

        private string Get(string key)
        {
            return Raw != null && Raw.TryGetValue(key, out var v) ? v : string.Empty;
        }

        private bool GetBool(string key)
        {
            var v = Get(key);
            return v == "true" || v == "1" || v == "yes";
        }

        private ulong GetULong(string key)
        {
            return ulong.TryParse(Get(key), out var v) ? v : 0UL;
        }
    }

    public sealed class InventoryResult
    {
        public int Handle { get; set; }
        public EResult Status { get; set; }
        public uint TimestampUnix { get; set; }
        public ulong OwnerSteamID { get; set; }
        public InventoryItem[] Items { get; set; } = Array.Empty<InventoryItem>();
        public byte[] SerializedBlob { get; set; }
    }
}
