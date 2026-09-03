using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Managers
{
    /// <summary>
    /// Client-side result/cache bridge for server-authoritative ISteamInventory.
    /// Inventory definitions and item ownership are fetched from SKYNET; this class
    /// only retains transient result handles and the latest in-memory definitions.
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

        private static int _nextResultHandle = 1;          // 0 is a valid SDK handle; we simply start at 1
        private static long _nextUpdateHandle = 1;
        private static volatile bool _definitionsLoaded;
        private static volatile bool _initialized;

        private static uint _appId;
        private static ulong _owner;

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
            // Per-user isolation: two accounts sharing the same install/appid must
            // not see each other's items.
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

        private static int BeginServerResult(string name, Func<APIClient.InventoryOperationResultDto> operation, bool fullUpdate)
        {
            if (!Enabled || !APIClient.IsEnabled || operation == null) return ResultInvalid;
            var handle = NextResultHandle();
            Results[handle] = new InventoryResult
            {
                Handle = handle,
                Status = EResult.k_EResultPending,
                TimestampUnix = NowUnix(),
                OwnerSteamID = _owner,
                Items = Array.Empty<InventoryItem>()
            };

            if (!WorkQueue.Enqueue(name, () =>
            {
                APIClient.InventoryOperationResultDto response = null;
                try { response = operation(); }
                catch (Exception ex) { SteamEmulator.Write("InventoryManager", $"{name} error: {ex.Message}"); }
                CompleteServerResult(handle, response, fullUpdate);
            }, null, true))
            {
                CompleteServerResult(handle, null, fullUpdate: false);
            }

            return handle;
        }

        private static void CompleteServerResult(int handle, APIClient.InventoryOperationResultDto response, bool fullUpdate)
        {
            if (!Results.TryGetValue(handle, out var result)) return;
            var success = response != null && response.Success;
            var items = response?.Items == null ? new List<InventoryItem>() : response.Items.Select(Map).ToList();
            result.Status = success ? EResult.k_EResultOK : EResult.k_EResultFail;
            result.TimestampUnix = response?.TimestampUnix ?? NowUnix();
            result.OwnerSteamID = response?.OwnerSteamId ?? _owner;
            result.Items = items.ToArray();
            if (!string.IsNullOrWhiteSpace(response?.SerializedBlobBase64))
            {
                try { result.SerializedBlob = Convert.FromBase64String(response.SerializedBlobBase64); }
                catch (FormatException) { result.SerializedBlob = null; }
            }

            if (fullUpdate && success)
                CallbackManager.AddCallback(new SteamInventoryFullUpdate_t { Handle = handle });
            CallbackManager.AddCallback(new SteamInventoryResultReady_t { Handle = handle, Result = result.Status });
        }

        private static InventoryItem Map(APIClient.ApiInventoryItem item) => new InventoryItem
        {
            ItemId = item.ItemId,
            DefId = item.DefId,
            Quantity = item.Quantity,
            Flags = item.Flags,
            AcquiredUnix = 0,
            Properties = new Dictionary<string, string>(item.Properties ?? new Dictionary<string, string>(), StringComparer.Ordinal)
        };

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

            if (fullUpdate && status == EResult.k_EResultOK)
            {
                // Steam guarantees that a full refresh notification is delivered
                // immediately before the matching result-ready notification.
                CallbackManager.AddCallback(new SteamInventoryFullUpdate_t { Handle = handle });
            }
            CallbackManager.AddCallback(new SteamInventoryResultReady_t { Handle = handle, Result = status });
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
            return BeginServerResult("Inventory GetAllItems", () => APIClient.GetInventoryItems(), fullUpdate: true);
        }

        public static int GetItemsByID(ulong[] ids)
        {
            return BeginServerResult("Inventory GetItemsByID", () => APIClient.GetInventoryItemsById(ids), fullUpdate: false);
        }

        // ================= mutations =================

        public static int Generate(int[] defs, uint[] qtys)
        {
            if (!AllowGenerate)
            {
                return MakeResult(EResult.k_EResultFail, null, false);
            }
            return BeginServerResult("Inventory GenerateItems", () => APIClient.GenerateInventoryItems(defs, qtys), fullUpdate: true);
        }

        public static int GrantPromoItems()
        {
            if (!AutoGrantPromos) return MakeResult(EResult.k_EResultFail, null, false);
            return BeginServerResult("Inventory GrantPromoItems", () => APIClient.AddInventoryPromoItems(GetPromoDefinitionIds()), fullUpdate: true);
        }

        public static int AddPromoItem(int def)
        {
            lock (StoreLock)
            {
                if (Definitions.TryGetValue(def, out var definition) && !HasPromoRule(definition))
                {
                    return MakeResult(EResult.k_EResultOK, Array.Empty<InventoryItem>(), false);
                }
            }
            return BeginServerResult("Inventory AddPromoItem", () => APIClient.AddInventoryPromoItem(def), fullUpdate: true);
        }

        public static int AddPromoItems(int[] defs)
        {
            int[] copy;
            lock (StoreLock)
            {
                copy = (defs ?? Array.Empty<int>())
                    .Where(def => !Definitions.TryGetValue(def, out var definition) || HasPromoRule(definition))
                    .ToArray();
            }
            if (copy.Length == 0)
            {
                return MakeResult(EResult.k_EResultOK, Array.Empty<InventoryItem>(), false);
            }
            return BeginServerResult("Inventory AddPromoItems", () => APIClient.AddInventoryPromoItems(copy), fullUpdate: true);
        }

        private static int GrantDefs(int[] defs, uint[] qtys, bool requirePromo)
        {
            return requirePromo
                ? BeginServerResult("Inventory GrantPromoItems", () => APIClient.AddInventoryPromoItems(defs), fullUpdate: true)
                : BeginServerResult("Inventory GenerateItems", () => APIClient.GenerateInventoryItems(defs, qtys), fullUpdate: true);
        }

        public static int ConsumeItem(ulong itemId, uint quantity)
        {
            return BeginServerResult("Inventory ConsumeItem", () => APIClient.ConsumeInventoryItem(itemId, quantity), fullUpdate: true);
        }

        public static int TransferItemQuantity(ulong src, uint quantity, ulong dest)
        {
            return BeginServerResult("Inventory TransferItemQuantity", () => APIClient.TransferInventoryItem(src, quantity, dest), fullUpdate: true);
        }

        public static int ExchangeItems(int[] genDefs, uint[] genQty, ulong[] destroyIds, uint[] destroyQty)
        {
            return BeginServerResult("Inventory ExchangeItems", () => APIClient.ExchangeInventoryItems(destroyIds, destroyQty, genDefs, genQty), fullUpdate: true);
        }

        public static int TriggerItemDrop(int dropListDef)
        {
            if (!Enabled) return ResultInvalid;
            return BeginServerResult("Inventory TriggerItemDrop",
                () => APIClient.TriggerInventoryItemDrop(dropListDef), fullUpdate: true);
        }

        // ================= property updates =================

        public static ulong StartUpdateProperties()
        {
            if (!Enabled) return UpdateHandleInvalid;
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
            if (!Enabled) return ResultInvalid;
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

            return MakeResult(EResult.k_EResultOK, affected.ToArray(), fullUpdate: true);
        }

        // ================= definitions =================

        public static bool LoadItemDefinitions()
        {
            if (!Enabled) return false;
            WorkQueue.Enqueue("Inventory LoadItemDefinitions", () =>
            {
                try
                {
                    var definitions = APIClient.GetInventoryDefinitions(_appId);
                    if (definitions != null)
                    {
                        lock (StoreLock)
                        {
                            Definitions.Clear();
                            foreach (var definition in definitions)
                            {
                                var raw = new Dictionary<string, string>(definition.Properties ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase)
                                {
                                    ["name"] = definition.Name ?? string.Empty,
                                    ["type"] = definition.Type ?? string.Empty,
                                    ["tradable"] = definition.Tradable ? "1" : "0",
                                    ["marketable"] = definition.Marketable ? "1" : "0"
                                };
                                Definitions[definition.DefId] = new ItemDefinition { DefId = definition.DefId, Raw = raw };
                            }
                        }
                    }
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
                return Definitions.Values.Where(IsPromoEligible).Select(d => d.DefId).ToArray();
            }
        }

        public static int[] GetEligiblePromoDefinitionIds()
        {
            lock (StoreLock)
            {
                return Definitions.Values.Where(IsManualPromo).Select(d => d.DefId).ToArray();
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
            if (!Enabled) return 0; // k_uAPICallInvalid
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
            if (!Enabled) return 0; // k_uAPICallInvalid
            ulong orderId = (ulong)DateTime.UtcNow.Ticks;
            ulong transId = orderId ^ 0x5A5A5A5AUL;

            if (AutoGrantPurchases && defs != null && defs.Length > 0)
            {
                // Grant locally since there is no real payment path.
                BeginServerResult("Inventory StartPurchase", () => APIClient.PurchaseInventoryItems(defs, qtys), fullUpdate: true);
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
            if (!Enabled) return 0; // k_uAPICallInvalid
            int count;
            lock (StoreLock)
            {
                count = Definitions.Values.Count(IsManualPromo);
            }
            return CallbackManager.AddCallbackResult(new SteamInventoryEligiblePromoItemDefIDs_t
            {
                Result = EResult.k_EResultOK,
                SteamID = steamID,
                UmEligiblePromoItemDefs = count,
                CachedData = false,
            });
        }

        public static byte[] SerializeResult(int handle)
        {
            if (!Results.TryGetValue(handle, out var r))
            {
                return null;
            }
            if (!IsResultReady(r.Status))
            {
                return null;
            }
            if (r.SerializedBlob != null)
            {
                return r.SerializedBlob;
            }

            var response = APIClient.SerializeInventoryResult(r.Items.Select(item => item.ItemId).ToArray());
            if (response == null || string.IsNullOrWhiteSpace(response.BlobBase64)) return null;
            try
            {
                r.SerializedBlob = Convert.FromBase64String(response.BlobBase64);
                return r.SerializedBlob;
            }
            catch (FormatException)
            {
                return null;
            }
        }

        // Parses a signed blob into a NEW result handle. On any failure returns a
        // result handle with k_EResultFail (never a bare false), per SDK expectation.
        public static int DeserializeResult(byte[] blob, bool reserved = false)
        {
            if (!Enabled || reserved)
            {
                return MakeResult(EResult.k_EResultFail, null, false);
            }

            if (blob == null || blob.Length == 0 || !APIClient.IsEnabled)
            {
                return MakeResult(EResult.k_EResultFail, null, false);
            }
            try
            {
                var response = APIClient.DeserializeInventoryResult(Convert.ToBase64String(blob));
                if (response == null || (!response.Success && response.Status != (int)EResult.k_EResultExpired))
                {
                    return MakeResult(EResult.k_EResultFail, null, false);
                }
                var items = response.Items == null ? Array.Empty<InventoryItem>() : response.Items.Select(Map).ToArray();
                var handle = NextResultHandle();
                Results[handle] = new InventoryResult
                {
                    Handle = handle,
                    Status = (EResult)response.Status,
                    TimestampUnix = response.TimestampUnix,
                    OwnerSteamID = response.SteamId,
                    Items = items,
                    SerializedBlob = string.IsNullOrWhiteSpace(response.BlobBase64) ? blob : Convert.FromBase64String(response.BlobBase64)
                };
                CallbackManager.AddCallback(new SteamInventoryResultReady_t { Handle = handle, Result = (EResult)response.Status });
                return handle;
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("InventoryManager", $"DeserializeResult error: {ex.Message}");
                return MakeResult(EResult.k_EResultFail, null, false);
            }
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

        private static bool IsResultReady(EResult status)
        {
            return status == EResult.k_EResultOK || status == EResult.k_EResultExpired;
        }

        private static bool IsPromoEligible(ItemDefinition definition)
        {
            if (!definition.Raw.TryGetValue("promo", out var value) || string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var rules = value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(rule => rule.Trim()).ToArray();
            if (rules.Length == 0 || rules.Any(rule => rule.Equals("manual", StringComparison.OrdinalIgnoreCase))) return false;
            if (rules.Any(rule => rule == "1" || rule.Equals("true", StringComparison.OrdinalIgnoreCase) || rule.Equals("yes", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            var ownershipRules = rules
                .Where(rule => rule.StartsWith("owns:", StringComparison.OrdinalIgnoreCase))
                .Select(rule => rule.Substring("owns:".Length).Trim())
                .ToArray();
            if (ownershipRules.Length == 0)
            {
                return rules.Any(rule => rule.StartsWith("ach:", StringComparison.OrdinalIgnoreCase) ||
                    rule.StartsWith("played:", StringComparison.OrdinalIgnoreCase));
            }

            return ownershipRules.Any(appId => uint.TryParse(appId, out var id) && AppEntitlementManager.HasLicense(id));
        }

        private static bool HasPromoRule(ItemDefinition definition)
        {
            return definition.Raw.TryGetValue("promo", out var value) && !string.IsNullOrWhiteSpace(value);
        }

        private static bool IsManualPromo(ItemDefinition definition)
        {
            if (!HasPromoRule(definition)) return false;
            return definition.Raw["promo"].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Any(rule => rule.Trim().Equals("manual", StringComparison.OrdinalIgnoreCase));
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
