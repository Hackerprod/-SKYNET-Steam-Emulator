using System.Security.Cryptography;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    private const ushort InventoryConsumedFlag = 512;
    private const ushort InventoryRemovedFlag = 256;
    private static readonly byte[] InventorySerializeMagic = { (byte)'S', (byte)'K', (byte)'I', (byte)'V' };
    private const ushort InventorySerializeVersion = 1;
    private readonly GameInventoryCatalogService _inventoryCatalog;
    private byte[] _inventorySigningKey = Array.Empty<byte>();
    private ulong _nextInventoryItemId;

    private void InitializeInventorySigningKey(string dataRoot)
    {
        var path = Path.Combine(dataRoot, "inventory", "serialize.key");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        try
        {
            if (File.Exists(path))
            {
                var existing = File.ReadAllBytes(path);
                if (existing.Length == 32) { _inventorySigningKey = existing; return; }
            }
            _inventorySigningKey = RandomNumberGenerator.GetBytes(32);
            File.WriteAllBytes(path, _inventorySigningKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not load inventory signing key {Path}", path);
            _inventorySigningKey = RandomNumberGenerator.GetBytes(32);
        }
    }

    public List<ApiInventoryItem> GetAllItems(string token)
    {
        lock (_sync)
        {
            return TryGetInventorySessionLocked(token, out var session, out var appId)
                ? _state.Inventory.Values.Where(item => item.SteamId == session!.SteamId && item.AppId == appId && (item.Flags & (InventoryConsumedFlag | InventoryRemovedFlag)) == 0 && item.Quantity > 0).Select(CloneInventoryItem).ToList()
                : new List<ApiInventoryItem>();
        }
    }

    public List<ApiInventoryItem> GetItemsByID(string token, ulong[] itemIds)
    {
        lock (_sync)
        {
            return TryGetInventorySessionLocked(token, out var session, out var appId)
                ? GetInventoryItemsLocked(session!.SteamId, appId, itemIds, includeInactive: false)
                : new List<ApiInventoryItem>();
        }
    }

    public List<ApiInventoryItem>? GenerateItems(string token, int[] defIds, uint[] quantities)
    {
        lock (_sync)
        {
            if (!TryGetInventorySessionLocked(token, out var session, out var appId)) return new List<ApiInventoryItem>();
            var generated = GrantInventoryDefinitionsLocked(session!.SteamId, appId, defIds, quantities, requirePromo: false);
            SaveState();
            return generated;
        }
    }

    public List<ApiInventoryItem>? AddPromoItem(string token, int? defId)
    {
        lock (_sync)
        {
            if (!TryGetInventorySessionLocked(token, out var session, out var appId)) return new List<ApiInventoryItem>();
            var definitions = _inventoryCatalog.Get(appId);
            var ids = defId.HasValue ? new[] { defId.Value } : definitions.Where(IsPromo).Select(item => item.DefId).ToArray();
            var granted = GrantInventoryDefinitionsLocked(session!.SteamId, appId, ids, null, requirePromo: true);
            SaveState();
            return granted;
        }
    }

    public List<ApiInventoryItem>? AddPromoItems(string token, int[] defIds)
    {
        lock (_sync)
        {
            if (!TryGetInventorySessionLocked(token, out var session, out var appId)) return new List<ApiInventoryItem>();
            var granted = GrantInventoryDefinitionsLocked(session!.SteamId, appId, defIds ?? Array.Empty<int>(), null, requirePromo: true);
            SaveState();
            return granted;
        }
    }

    public bool ConsumeItem(string token, ulong itemId, uint quantity)
    {
        lock (_sync)
        {
            if (quantity == 0 || !TryGetInventorySessionLocked(token, out var session, out var appId) ||
                !_state.Inventory.TryGetValue(itemId, out var item) || item.SteamId != session!.SteamId || item.AppId != appId ||
                (item.Flags & (InventoryConsumedFlag | InventoryRemovedFlag)) != 0 || item.Quantity == 0)
                return false;
            item.Quantity = quantity == 0 || quantity >= item.Quantity ? 0 : item.Quantity - quantity;
            if (item.Quantity == 0) item.Flags |= InventoryConsumedFlag;
            SaveState();
            return true;
        }
    }

    public List<ApiInventoryItem>? ExchangeItem(string token, ulong[] itemIds, uint[] quantities, int[] defIds, uint[] generatedQuantities)
    {
        lock (_sync)
        {
            if (!TryGetInventorySessionLocked(token, out var session, out var appId)) return new List<ApiInventoryItem>();
            var consume = itemIds ?? Array.Empty<ulong>();
            var required = new Dictionary<ulong, uint>();
            for (var i = 0; i < consume.Length; i++)
            {
                var amount = quantities != null && i < quantities.Length ? quantities[i] : 1u;
                if (amount == 0) return null;
                required[consume[i]] = required.TryGetValue(consume[i], out var current) ? current + amount : amount;
            }
            if (quantities != null && quantities.Length > consume.Length) return null;
            if (!CanGrantInventoryDefinitionsLocked(appId, defIds, generatedQuantities, requirePromo: false)) return null;
            foreach (var pair in required)
            {
                if (!_state.Inventory.TryGetValue(pair.Key, out var item) || item.SteamId != session!.SteamId || item.AppId != appId ||
                    (item.Flags & (InventoryConsumedFlag | InventoryRemovedFlag)) != 0 || item.Quantity < pair.Value)
                    return new List<ApiInventoryItem>();
            }
            foreach (var pair in required)
            {
                var item = _state.Inventory[pair.Key];
                item.Quantity -= pair.Value;
                if (item.Quantity == 0) item.Flags |= InventoryConsumedFlag;
            }
            var generated = GrantInventoryDefinitionsLocked(session!.SteamId, appId, defIds, generatedQuantities, requirePromo: false)!;
            SaveState();
            return generated;
        }
    }

    public List<ApiInventoryItem>? TransferItem(string token, ulong sourceItemId, uint quantity, ulong destinationItemId)
    {
        lock (_sync)
        {
            if (quantity == 0 || sourceItemId == destinationItemId ||
                !TryGetInventorySessionLocked(token, out var session, out var appId) ||
                !_state.Inventory.TryGetValue(sourceItemId, out var source) ||
                !_state.Inventory.TryGetValue(destinationItemId, out var destination) ||
                source.SteamId != session!.SteamId || source.AppId != appId ||
                destination.SteamId != session.SteamId || destination.AppId != appId ||
                source.DefId != destination.DefId ||
                (source.Flags & (InventoryConsumedFlag | InventoryRemovedFlag)) != 0 ||
                (destination.Flags & (InventoryConsumedFlag | InventoryRemovedFlag)) != 0 ||
                source.Quantity < quantity ||
                destination.Quantity > uint.MaxValue - quantity)
                return null;

            source.Quantity -= quantity;
            if (source.Quantity == 0) source.Flags |= InventoryConsumedFlag;
            destination.Quantity += quantity;
            SaveState();
            return new[] { CloneInventoryItem(source), CloneInventoryItem(destination) }.ToList();
        }
    }

    public ApiInventoryOperationResult? GetInventoryOperationResult(string token, IEnumerable<ApiInventoryItem> items)
    {
        lock (_sync)
        {
            if (!TryGetInventorySessionLocked(token, out var session, out var appId)) return null;
            var snapshot = items?.Select(CloneInventoryItem).ToList() ?? new List<ApiInventoryItem>();
            var timestamp = NowInventoryUnix();
            return new ApiInventoryOperationResult
            {
                Success = true,
                Items = snapshot,
                TimestampUnix = timestamp,
                OwnerSteamId = session!.SteamId,
                SerializedBlobBase64 = Convert.ToBase64String(SerializeInventoryBlob(appId, session.SteamId, timestamp, snapshot))
            };
        }
    }

    public ApiInventorySerializedResult? SerializeInventoryResult(string token, ulong[] itemIds)
    {
        lock (_sync)
        {
            if (!TryGetInventorySessionLocked(token, out var session, out var appId)) return null;
            var items = GetInventoryItemsLocked(session!.SteamId, appId, itemIds, includeInactive: true);
            if ((itemIds?.Length ?? 0) != items.Select(item => item.ItemId).Distinct().Count()) return null;
            return new ApiInventorySerializedResult
            {
                BlobBase64 = Convert.ToBase64String(SerializeInventoryBlob(appId, session.SteamId, NowInventoryUnix(), items))
            };
        }
    }

    public ApiInventoryDeserializedResult? DeserializeInventoryResult(string token, string blobBase64)
    {
        lock (_sync)
        {
            if (!TryGetInventorySessionLocked(token, out var session, out var sessionAppId) || string.IsNullOrWhiteSpace(blobBase64)) return null;
            try
            {
                var blob = Convert.FromBase64String(blobBase64);
                if (!TryReadInventoryBlob(blob, out var appId, out var steamId, out var timestamp, out var items) ||
                    appId != sessionAppId || steamId != session!.SteamId) return null;
                return new ApiInventoryDeserializedResult { Success = true, SteamId = steamId, AppId = appId, TimestampUnix = timestamp, Items = items, BlobBase64 = blobBase64 };
            }
            catch (FormatException) { return null; }
            catch (EndOfStreamException) { return null; }
        }
    }

    public List<ApiInventoryItem> GetInactiveInventoryItems(string token, ulong[] itemIds)
    {
        lock (_sync)
        {
            return TryGetInventorySessionLocked(token, out var session, out var appId)
                ? GetInventoryItemsLocked(session!.SteamId, appId, itemIds, includeInactive: true)
                : new List<ApiInventoryItem>();
        }
    }

    private bool TryGetInventorySessionLocked(string token, out ApiSession? session, out uint appId)
    {
        session = null;
        appId = 0;
        if (!TryGetSession(token, out session) || session == null) return false;
        appId = session.AppId != 0 ? session.AppId : (_state.Users.TryGetValue(session.SteamId, out var user) ? user.AppId : 0);
        return appId != 0;
    }

    private List<ApiInventoryItem> GetInventoryItemsLocked(ulong steamId, uint appId, IEnumerable<ulong>? itemIds, bool includeInactive)
    {
        var ids = itemIds ?? Array.Empty<ulong>();
        var result = new List<ApiInventoryItem>();
        foreach (var id in ids)
        {
            if (!_state.Inventory.TryGetValue(id, out var item) || item.SteamId != steamId || item.AppId != appId) continue;
            if (!includeInactive && ((item.Flags & (InventoryConsumedFlag | InventoryRemovedFlag)) != 0 || item.Quantity == 0)) continue;
            result.Add(CloneInventoryItem(item));
        }
        return result;
    }

    private List<ApiInventoryItem>? GrantInventoryDefinitionsLocked(ulong steamId, uint appId, IEnumerable<int>? defIds, IReadOnlyList<uint>? quantities, bool requirePromo)
    {
        var ids = (defIds ?? Array.Empty<int>()).ToArray();
        if (!CanGrantInventoryDefinitionsLocked(appId, ids, quantities, requirePromo)) return null;

        var generated = new List<ApiInventoryItem>(ids.Length);
        for (var i = 0; i < ids.Length; i++)
        {
            var defId = ids[i];
            var quantity = quantities != null && i < quantities.Count ? quantities[i] : 1u;
            var item = new ApiInventoryItem { ItemId = ++_nextInventoryItemId, DefId = defId, SteamId = steamId, AppId = appId, Quantity = quantity };
            _state.Inventory[item.ItemId] = item;
            generated.Add(CloneInventoryItem(item));
        }
        return generated;
    }

    private bool CanGrantInventoryDefinitionsLocked(uint appId, IEnumerable<int>? defIds, IReadOnlyList<uint>? quantities, bool requirePromo)
    {
        var ids = (defIds ?? Array.Empty<int>()).ToArray();
        if (quantities != null && quantities.Count != ids.Length) return false;
        var definitions = _inventoryCatalog.Get(appId).ToDictionary(item => item.DefId);
        for (var i = 0; i < ids.Length; i++)
        {
            if (!definitions.TryGetValue(ids[i], out var definition) || (requirePromo && !IsPromo(definition))) return false;
            if (quantities != null && quantities[i] == 0) return false;
        }
        return true;
    }

    private static bool IsPromo(ApiInventoryItemDef definition) => definition.Properties.TryGetValue("promo", out var value) &&
        (value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase) || value.Equals("yes", StringComparison.OrdinalIgnoreCase));

    private static ApiInventoryItem CloneInventoryItem(ApiInventoryItem item) => new()
    {
        ItemId = item.ItemId, DefId = item.DefId, SteamId = item.SteamId, AppId = item.AppId,
        Quantity = item.Quantity, Flags = item.Flags, Properties = new Dictionary<string, string>(item.Properties ?? new())
    };

    private static uint NowInventoryUnix() => (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    private byte[] SerializeInventoryBlob(uint appId, ulong steamId, uint timestamp, IReadOnlyList<ApiInventoryItem> items)
    {
        using var ms = new MemoryStream();
        using (var writer = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
        {
            writer.Write(InventorySerializeMagic);
            writer.Write(InventorySerializeVersion);
            writer.Write(appId);
            writer.Write(steamId);
            writer.Write((ulong)timestamp);
            writer.Write(items.Count);
            foreach (var item in items)
            {
                writer.Write(item.ItemId); writer.Write(item.DefId);
                writer.Write((ushort)Math.Min(item.Quantity, ushort.MaxValue)); writer.Write(item.Flags);
            }
        }
        var body = ms.ToArray();
        var mac = ComputeInventoryHmac(body);
        return body.Concat(mac).ToArray();
    }

    private bool TryReadInventoryBlob(byte[] blob, out uint appId, out ulong steamId, out uint timestamp, out List<ApiInventoryItem> items)
    {
        appId = 0; steamId = 0; timestamp = 0; items = new List<ApiInventoryItem>();
        if (blob.Length < 4 + 2 + 4 + 8 + 8 + 4 + 32) return false;
        var bodyLength = blob.Length - 32;
        var body = blob[..bodyLength];
        if (!CryptographicOperations.FixedTimeEquals(ComputeInventoryHmac(body), blob[bodyLength..])) return false;
        using var ms = new MemoryStream(body);
        using var reader = new BinaryReader(ms);
        if (!reader.ReadBytes(4).SequenceEqual(InventorySerializeMagic) || reader.ReadUInt16() != InventorySerializeVersion) return false;
        appId = reader.ReadUInt32(); steamId = reader.ReadUInt64(); timestamp = (uint)reader.ReadUInt64();
        var count = reader.ReadInt32();
        if (count < 0 || count > 100000) return false;
        for (var i = 0; i < count; i++)
            items.Add(new ApiInventoryItem { ItemId = reader.ReadUInt64(), DefId = reader.ReadInt32(), Quantity = reader.ReadUInt16(), Flags = reader.ReadUInt16(), SteamId = steamId, AppId = appId });
        return ms.Position == ms.Length;
    }

    private byte[] ComputeInventoryHmac(byte[] body)
    {
        using var hmac = new HMACSHA256(_inventorySigningKey);
        return hmac.ComputeHash(body);
    }
}
