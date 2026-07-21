using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using SKYNET_server.Json;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    private const uint DotaUnequipSlot = 0xFFFF;

    private static readonly JsonSerializerOptions DotaEquipmentJsonOptions = SkynetJsonSerializerOptions.CreateCompatible();

    // Script-facing shape of one equipment entry. ItemId/SteamId/UpdatedAt are
    // intentionally absent: they are recomputed server-side on save so scripts
    // never handle persistence-owned values directly.
    private sealed class DotaEquipmentJsonEntry
    {
        public uint HeroId { get; set; }
        public string? HeroName { get; set; }
        public string? Slot { get; set; }
        public uint SlotId { get; set; }
        public uint DefIndex { get; set; }
        public uint Style { get; set; }
    }

    private const string DefaultDotaPath = @"D:\Games\Steam\steamapps\common\dota 2 beta";

    public ApiDotaCosmeticOverview? GetDotaCosmeticsOverview(string token, string? search, uint? heroId, int take = 300)
    {
        lock (_sync)
        {
            if (!IsWebAdmin(token))
            {
                return null;
            }

            IEnumerable<ApiDotaItem> items = _state.DotaItems.Values;
            if (!string.IsNullOrWhiteSpace(search))
            {
                var needle = search.Trim();
                items = items.Where(item =>
                    item.DefIndex.ToString().Contains(needle, StringComparison.OrdinalIgnoreCase) ||
                    item.Name.Contains(needle, StringComparison.OrdinalIgnoreCase) ||
                    item.Prefab.Contains(needle, StringComparison.OrdinalIgnoreCase) ||
                    item.Slot.Contains(needle, StringComparison.OrdinalIgnoreCase) ||
                    item.HeroNames.Any(hero => hero.Contains(needle, StringComparison.OrdinalIgnoreCase)));
            }

            if (heroId is > 0)
            {
                items = items.Where(item => item.HeroIds.Contains(heroId.Value));
            }

            return new ApiDotaCosmeticOverview
            {
                Summary = BuildDotaCosmeticSummaryLocked(),
                Items = items
                    .OrderBy(item => item.HeroIds.Count == 0)
                    .ThenBy(item => item.HeroNames.FirstOrDefault() ?? string.Empty)
                    .ThenBy(item => item.Slot)
                    .ThenBy(item => item.Name)
                    .Take(Math.Clamp(take, 25, 1000))
                    .Select(CloneDotaItem)
                    .ToList(),
                Equipment = _state.DotaEquipment.Values
                    .SelectMany(item => item)
                    .OrderBy(item => item.HeroName)
                    .ThenBy(item => item.Slot)
                    .Select(CloneDotaEquipment)
                    .ToList(),
                Users = _state.Users.Values.OrderBy(user => user.PersonaName).Select(CloneUser).ToList()
            };
        }
    }

    public ApiDotaItemImportResult ImportDotaCosmetics(string token, ApiDotaImportRequest request)
    {
        lock (_sync)
        {
            if (!IsWebAdmin(token))
            {
                return new ApiDotaItemImportResult { Success = false, Message = "Not authorized." };
            }
        }

        var importPath = ResolveDotaContentPath(request.DotaPath);
        try
        {
            var pakPath = ResolvePakPath(importPath);
            var itemsText = VpkTextReader.ReadText(pakPath, "scripts/items/items_game.txt");
            var heroesText = VpkTextReader.ReadText(pakPath, "scripts/npc/npc_heroes.txt");
            var heroIds = DotaItemsGameParser.ParseHeroIds(heroesText);
            var heroSlots = DotaItemsGameParser.ParseHeroSlots(heroesText);
            var items = DotaItemsGameParser.ParseItems(itemsText, heroIds);

            lock (_sync)
            {
                _state.DotaItems = items.ToDictionary(item => item.DefIndex);
                _state.DotaHeroIds = heroIds;
                _state.DotaHeroSlots = heroSlots;
                _state.DotaCosmetics.DotaPath = importPath;
                _state.DotaCosmetics.LastImportAt = DateTime.UtcNow;
                _state.DotaCosmetics.LastImportStatus = $"OK: {items.Count} items, {heroIds.Count} heroes from {pakPath}";
                TouchDotaEquipmentVersionLocked();
                // The item catalog changed: persist it (routine flushes skip it).
                RequestCatalogFlush();
            }

            return new ApiDotaItemImportResult
            {
                Success = true,
                Message = "Item catalog updated.",
                ItemCount = items.Count,
                HeroCount = heroIds.Count,
                SourcePath = pakPath
            };
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException)
        {
            lock (_sync)
            {
                _state.DotaCosmetics.DotaPath = importPath;
                _state.DotaCosmetics.LastImportAt = DateTime.UtcNow;
                _state.DotaCosmetics.LastImportStatus = $"ERROR: {ex.Message}";
                SaveState();
            }

            return new ApiDotaItemImportResult
            {
                Success = false,
                Message = ex.Message,
                SourcePath = importPath
            };
        }
    }

    public bool EquipDotaItemFromAdmin(string token, ApiDotaEquipItemRequest request)
    {
        lock (_sync)
        {
            if (!IsWebAdmin(token))
            {
                return false;
            }

            var changed = EquipDotaItemLocked(request.SteamId, request.HeroId, request.HeroName, request.SlotId, request.Slot, request.DefIndex, request.Style);
            if (changed == null)
            {
                return false;
            }

            TouchDotaEquipmentVersionLocked();
            SaveState();
            EnqueueFriendEvents(request.SteamId, "dota_equipment_changed", PersonaChangeRichPresence);
            return true;
        }
    }

    public bool ClearDotaEquipmentFromAdmin(string token, ulong steamId)
    {
        lock (_sync)
        {
            if (!IsWebAdmin(token))
            {
                return false;
            }

            _state.DotaEquipment.Remove(steamId);
            TouchDotaEquipmentVersionLocked();
            SaveState();
            EnqueueFriendEvents(steamId, "dota_equipment_changed", PersonaChangeRichPresence);
            return true;
        }
    }

    private ApiDotaRuntimeInventory GetDotaRuntimeInventory(ulong steamId)
    {
        lock (_sync)
        {
            var items = _state.DotaItems.Values
                .Where(item => !item.IsDefault && !item.IsTool)
                .OrderBy(item => item.DefIndex)
                .Select(CloneDotaItem)
                .ToList();
            var equipment = _state.DotaEquipment.TryGetValue(steamId, out var equipped)
                ? equipped.Select(CloneDotaEquipment).ToList()
                : new List<ApiDotaEquipment>();
            var ownedItems = equipment
                .Where(item => item.DefIndex != 0)
                .Select(item => item.DefIndex)
                .Distinct()
                .Select(defIndex => _state.DotaItems.TryGetValue(defIndex, out var catalogItem) ? CloneDotaItem(catalogItem) : null)
                .OfType<ApiDotaItem>()
                .OrderBy(item => item.DefIndex)
                .ToList();

            return new ApiDotaRuntimeInventory
            {
                SteamId = steamId,
                Items = items,
                OwnedItems = ownedItems,
                Equipment = equipment,
                Version = BuildDotaInventoryVersionLocked()
            };
        }
    }

    private List<ApiDotaEquipment> EquipDotaItemFromGameCoordinator(ulong steamId, ulong itemId, uint heroId, uint slotId, uint style)
    {
        lock (_sync)
        {
            var defIndex = ResolveDefIndexFromItemIdLocked(steamId, itemId);
            if (defIndex == 0 && itemId != 0)
            {
                return new List<ApiDotaEquipment>();
            }

            var changed = EquipDotaItemLocked(steamId, heroId, string.Empty, slotId, string.Empty, defIndex, style, slotIdIsExplicit: true);
            if (changed == null)
            {
                return new List<ApiDotaEquipment>();
            }

            if (changed.Count == 0)
            {
                return changed;
            }

            TouchDotaEquipmentVersionLocked();
            SaveState();
            return changed;
        }
    }

    private uint ResolveDotaItemDefFromGameCoordinator(ulong steamId, ulong itemId)
    {
        lock (_sync)
        {
            return ResolveDefIndexFromItemIdLocked(steamId, itemId);
        }
    }

    private string GetDotaEquipmentJson(ulong steamId)
    {
        lock (_sync)
        {
            var entries = _state.DotaEquipment.TryGetValue(steamId, out var equipment)
                ? equipment.Select(item => new DotaEquipmentJsonEntry
                {
                    HeroId = item.HeroId,
                    HeroName = item.HeroName,
                    Slot = item.Slot,
                    SlotId = item.SlotId,
                    DefIndex = item.DefIndex,
                    Style = item.Style
                }).ToList()
                : new List<DotaEquipmentJsonEntry>();

            return JsonSerializer.Serialize(entries, DotaEquipmentJsonOptions);
        }
    }

    private bool SetDotaEquipmentJson(ulong steamId, string json)
    {
        lock (_sync)
        {
            if (!_state.Users.ContainsKey(steamId))
            {
                return false;
            }

            // Older script runtimes persisted an empty equipment table as "{}"; treat it as an empty list.
            var trimmed = string.IsNullOrWhiteSpace(json) ? "[]" : json.Trim();
            if (trimmed == "{}")
            {
                trimmed = "[]";
            }

            List<DotaEquipmentJsonEntry>? entries;
            try
            {
                entries = JsonSerializer.Deserialize<List<DotaEquipmentJsonEntry>>(trimmed, DotaEquipmentJsonOptions);
            }
            catch (JsonException)
            {
                return false;
            }

            entries ??= new List<DotaEquipmentJsonEntry>();
            if (entries.Count > 1024)
            {
                return false;
            }

            var now = DateTime.UtcNow;
            var rebuilt = new List<ApiDotaEquipment>();
            foreach (var entry in entries)
            {
                if (entry == null || entry.DefIndex == 0 || !_state.DotaItems.TryGetValue(entry.DefIndex, out var catalogItem))
                {
                    continue;
                }

                var heroName = entry.HeroName;
                if (string.IsNullOrWhiteSpace(heroName))
                {
                    heroName = catalogItem.HeroNames.FirstOrDefault(hero => _state.DotaHeroIds.TryGetValue(hero, out var id) && id == entry.HeroId)
                        ?? catalogItem.HeroNames.FirstOrDefault()
                        ?? $"hero_{entry.HeroId}";
                }

                var slotName = !string.IsNullOrWhiteSpace(entry.Slot)
                    ? entry.Slot!
                    : !string.IsNullOrWhiteSpace(catalogItem.Slot) ? catalogItem.Slot : $"slot_{entry.SlotId}";

                rebuilt.Add(new ApiDotaEquipment
                {
                    SteamId = steamId,
                    HeroId = entry.HeroId,
                    HeroName = heroName,
                    Slot = slotName,
                    SlotId = entry.SlotId,
                    DefIndex = entry.DefIndex,
                    ItemId = BuildDotaItemInstanceId(steamId, entry.DefIndex),
                    Style = entry.Style == 255 ? 0 : entry.Style,
                    UpdatedAt = now
                });
            }

            _state.DotaEquipment[steamId] = NormalizeDotaEquipmentList(rebuilt);
            TouchDotaEquipmentVersionLocked();
            SaveState();
            EnqueueFriendEvents(steamId, "dota_equipment_changed", PersonaChangeRichPresence);
            return true;
        }
    }

    private string GetDotaCatalogItemJson(uint defIndex)
    {
        lock (_sync)
        {
            if (!_state.DotaItems.TryGetValue(defIndex, out var item))
            {
                return string.Empty;
            }

            return JsonSerializer.Serialize(new
            {
                item.DefIndex,
                item.Name,
                item.Slot,
                item.Prefab,
                item.QualityId,
                item.HeroIds,
                item.HeroNames
            }, DotaEquipmentJsonOptions);
        }
    }

    private List<ApiDotaEquipment> SetDotaItemStyleFromGameCoordinator(ulong steamId, ulong itemId, uint style)
    {
        lock (_sync)
        {
            var defIndex = ResolveDefIndexFromItemIdLocked(steamId, itemId);
            var changed = SetDotaItemStyleLocked(steamId, defIndex, style);
            if (changed == null)
            {
                return new List<ApiDotaEquipment>();
            }

            if (changed.Count > 0)
            {
                TouchDotaEquipmentVersionLocked();
                SaveState();
            }

            return changed;
        }
    }

    private void UpsertDotaMatchSnapshot(ApiDotaMatch snapshot)
    {
        lock (_sync)
        {
            foreach (var player in snapshot.Players)
            {
                player.Equipment = _state.DotaEquipment.TryGetValue(player.SteamId, out var equipment)
                    ? equipment.Select(CloneDotaEquipment).ToList()
                    : new List<ApiDotaEquipment>();
            }

            _state.DotaMatches[snapshot.LobbyId] = CloneDotaMatch(snapshot);

            var cutoff = DateTime.UtcNow.AddHours(-12);
            foreach (var old in _state.DotaMatches.Where(pair => pair.Value.UpdatedAt < cutoff).Select(pair => pair.Key).ToList())
            {
                _state.DotaMatches.Remove(old);
            }

            SaveState();
        }
    }

    private string GetDotaActiveMatchJson(ulong steamId)
    {
        lock (_sync)
        {
            var match = _state.DotaMatches.Values
                .Where(candidate => IsReconnectableDotaMatch(candidate) &&
                                    candidate.Players.Any(player => player.SteamId == steamId))
                .OrderByDescending(candidate => candidate.UpdatedAt)
                .FirstOrDefault();
            if (match == null)
            {
                return string.Empty;
            }

            return SerializeDotaMatchForScript(match);
        }
    }

    private string GetDotaMatchByLobbyJson(ulong lobbyId)
    {
        if (lobbyId == 0)
        {
            return string.Empty;
        }

        lock (_sync)
        {
            return _state.DotaMatches.TryGetValue(lobbyId, out var match) && IsReconnectableDotaMatch(match)
                ? SerializeDotaMatchForScript(match)
                : string.Empty;
        }
    }

    private const int DotaLobbyStateRun = 2;

    private bool IsReconnectableDotaMatch(ApiDotaMatch match)
    {
        if (match.State != DotaLobbyStateRun ||
            string.IsNullOrWhiteSpace(match.Connect) ||
            match.Players.Count == 0)
        {
            return false;
        }

        if (!match.Dedicated)
        {
            return true;
        }

        var status = _dotaDedicatedServers.GetStatus(match.LobbyId);
        return status is not ("not_found" or "failed" or "stopped");
    }

    private static string SerializeDotaMatchForScript(ApiDotaMatch match) => JsonSerializer.Serialize(new
    {
        LobbyId = match.LobbyId.ToString(System.Globalization.CultureInfo.InvariantCulture),
        MatchId = match.MatchId.ToString(System.Globalization.CultureInfo.InvariantCulture),
        ServerSteamId = match.ServerSteamId.ToString(System.Globalization.CultureInfo.InvariantCulture),
        match.Connect,
        match.State,
        match.GameState,
        match.GameStartTime,
        match.Dedicated,
        UpdatedAtUnix = new DateTimeOffset(match.UpdatedAt).ToUnixTimeSeconds(),
        Players = match.Players.Select(player => new
        {
            SteamId = player.SteamId.ToString(System.Globalization.CultureInfo.InvariantCulture),
            player.AccountId,
            player.PersonaName,
            player.Team,
            player.Slot,
            player.CoachTeam,
            player.HeroId
        }).ToList()
    });

    private bool RemoveDotaMatchSnapshot(ulong lobbyId)
    {
        if (lobbyId == 0)
        {
            return false;
        }

        lock (_sync)
        {
            var removed = _state.DotaMatches.Remove(lobbyId);
            if (removed)
            {
                SaveState();
            }

            return removed;
        }
    }

    private List<ApiDotaEquipment>? EquipDotaItemLocked(
        ulong steamId,
        uint heroId,
        string heroName,
        uint slotId,
        string slot,
        uint defIndex,
        uint style,
        bool slotIdIsExplicit = false)
    {
        if (!_state.Users.ContainsKey(steamId))
        {
            return null;
        }

        ApiDotaItem? item = null;
        if (defIndex != 0 && !_state.DotaItems.TryGetValue(defIndex, out item))
        {
            return null;
        }

        if (item != null)
        {
            // Only the admin flow depends on the catalog slot name; the GC flow
            // carries an explicit (class, slot) pair resolved by the game client,
            // and roughly half the catalog has no inferable slot name.
            if (!slotIdIsExplicit && string.IsNullOrWhiteSpace(item.Slot))
            {
                return new List<ApiDotaEquipment>();
            }

            if (heroId == 0 && item.HeroIds.Count == 1)
            {
                heroId = item.HeroIds[0];
            }

            if (string.IsNullOrWhiteSpace(heroName))
            {
                heroName = item.HeroNames.FirstOrDefault(hero => !_state.DotaHeroIds.TryGetValue(hero, out var found) || found == heroId)
                    ?? item.HeroNames.FirstOrDefault()
                    ?? string.Empty;
            }

            if (!slotIdIsExplicit && slotId == 0)
            {
                slotId = ResolveDotaSlotId(heroId, heroName, item.Slot);
            }

            if (string.IsNullOrWhiteSpace(slot))
            {
                slot = item.Slot;
            }
        }

        if (heroId == 0 && !string.IsNullOrWhiteSpace(heroName) && _state.DotaHeroIds.TryGetValue(heroName, out var mappedHeroId))
        {
            heroId = mappedHeroId;
        }

        if (!_state.DotaEquipment.TryGetValue(steamId, out var equipment))
        {
            equipment = new List<ApiDotaEquipment>();
            _state.DotaEquipment[steamId] = equipment;
        }

        var itemInstanceId = defIndex == 0 ? 0 : BuildDotaItemInstanceId(steamId, defIndex);
        bool isUnequip = slotId == DotaUnequipSlot;
        var removed = equipment
            .Where(existing =>
                MatchesDotaUnequip(existing, heroId, defIndex, itemInstanceId, isUnequip) ||
                (!isUnequip && SameDotaEquipmentSlot(existing, heroId, slotId, slot)) ||
                (!isUnequip && defIndex != 0 && existing.HeroId == heroId && (existing.DefIndex == defIndex || existing.ItemId == itemInstanceId)))
            .Select(CloneDotaEquipment)
            .ToList();
        equipment.RemoveAll(existing =>
            MatchesDotaUnequip(existing, heroId, defIndex, itemInstanceId, isUnequip) ||
            (!isUnequip && SameDotaEquipmentSlot(existing, heroId, slotId, slot)) ||
            (!isUnequip && defIndex != 0 && existing.HeroId == heroId && (existing.DefIndex == defIndex || existing.ItemId == itemInstanceId)));

        if (defIndex == 0 || isUnequip)
        {
            return removed;
        }

        var now = DateTime.UtcNow;
        var equipped = new ApiDotaEquipment
        {
            SteamId = steamId,
            HeroId = heroId,
            HeroName = string.IsNullOrWhiteSpace(heroName) ? $"hero_{heroId}" : heroName,
            Slot = string.IsNullOrWhiteSpace(slot) ? $"slot_{slotId}" : slot,
            SlotId = slotId,
            DefIndex = defIndex,
            ItemId = itemInstanceId,
            Style = style == 255 ? 0 : style,
            UpdatedAt = now
        };

        equipment.Add(equipped);
        removed.Add(CloneDotaEquipment(equipped));
        return removed;
    }

    private void NormalizeDotaItemSlotsLocked()
    {
        foreach (var item in _state.DotaItems.Values)
        {
            item.Slot = DotaItemsGameParser.InferItemSlot(item.Slot, item.Name, item.Prefab, item.ImageInventory);
        }

        foreach (var pair in _state.DotaEquipment.ToList())
        {
            var normalized = new List<ApiDotaEquipment>();
            foreach (var equipped in pair.Value.Where(item => item != null))
            {
                if (equipped.DefIndex != 0 && _state.DotaItems.TryGetValue(equipped.DefIndex, out var catalogItem))
                {
                    catalogItem.Slot = DotaItemsGameParser.InferItemSlot(catalogItem.Slot, catalogItem.Name, catalogItem.Prefab, catalogItem.ImageInventory);
                    if (!string.IsNullOrWhiteSpace(catalogItem.Slot) &&
                        (string.IsNullOrWhiteSpace(equipped.Slot) || equipped.Slot.StartsWith("slot_", StringComparison.OrdinalIgnoreCase)))
                    {
                        equipped.Slot = catalogItem.Slot;
                    }
                }

                if (string.IsNullOrWhiteSpace(equipped.Slot))
                {
                    // GC-equipped entries always carry an explicit SlotId; keep them
                    // even when no slot name could be inferred from the catalog.
                    equipped.Slot = $"slot_{equipped.SlotId}";
                }

                if (!equipped.Slot.StartsWith("slot_", StringComparison.OrdinalIgnoreCase) &&
                    ShouldResolveDotaSlotId(equipped))
                {
                    equipped.SlotId = ResolveDotaSlotId(equipped.HeroId, equipped.HeroName, equipped.Slot);
                }

                normalized.Add(equipped);
            }

            _state.DotaEquipment[pair.Key] = NormalizeDotaEquipmentList(normalized);
        }
    }

    private List<ApiDotaEquipment>? SetDotaItemStyleLocked(ulong steamId, uint defIndex, uint style)
    {
        if (!_state.Users.ContainsKey(steamId))
        {
            return null;
        }

        if (defIndex == 0 || !_state.DotaEquipment.TryGetValue(steamId, out var equipment))
        {
            return new List<ApiDotaEquipment>();
        }

        var now = DateTime.UtcNow;
        var changed = new List<ApiDotaEquipment>();
        foreach (var equipped in equipment.Where(item => item.DefIndex == defIndex))
        {
            equipped.Style = style == 255 ? 0 : style;
            equipped.UpdatedAt = now;
            changed.Add(CloneDotaEquipment(equipped));
        }

        return changed;
    }

    private uint ResolveDefIndexFromItemIdLocked(ulong steamId, ulong itemId)
    {
        if (itemId == 0)
        {
            return 0;
        }

        foreach (var item in _state.DotaItems.Values)
        {
            if (BuildDotaItemInstanceId(steamId, item.DefIndex) == itemId)
            {
                return item.DefIndex;
            }
        }

        return 0;
    }

    private static List<ApiDotaEquipment> NormalizeDotaEquipmentList(List<ApiDotaEquipment> equipment)
    {
        return equipment
            .Where(item => item != null)
            .GroupBy(BuildDotaEquipmentSlotKey, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.OrderByDescending(item => item.UpdatedAt).First())
            .OrderBy(item => item.HeroId)
            .ThenBy(item => NormalizeDotaSlot(item.Slot))
            .ThenBy(item => item.SlotId)
            .ToList();
    }

    private static string BuildDotaEquipmentSlotKey(ApiDotaEquipment item)
    {
        var slot = NormalizeDotaSlot(item.Slot);
        if (string.IsNullOrWhiteSpace(slot))
        {
            slot = $"slot_id:{item.SlotId}";
        }

        return $"{item.SteamId}:{item.HeroId}:{slot}";
    }

    private static bool SameDotaEquipmentSlot(ApiDotaEquipment existing, uint heroId, uint slotId, string slot)
    {
        if (existing.HeroId != heroId)
        {
            return false;
        }

        var existingSlot = NormalizeDotaSlot(existing.Slot);
        var requestedSlot = NormalizeDotaSlot(slot);
        if (!string.IsNullOrWhiteSpace(existingSlot) && !string.IsNullOrWhiteSpace(requestedSlot))
        {
            return string.Equals(existingSlot, requestedSlot, StringComparison.OrdinalIgnoreCase);
        }

        return existing.SlotId == slotId;
    }

    private static bool MatchesDotaUnequip(
        ApiDotaEquipment existing,
        uint heroId,
        uint defIndex,
        ulong itemInstanceId,
        bool isUnequip)
    {
        if (!isUnequip)
        {
            return false;
        }

        if (heroId != 0 && existing.HeroId != heroId)
        {
            return false;
        }

        return (defIndex != 0 && existing.DefIndex == defIndex) ||
               (itemInstanceId != 0 && existing.ItemId == itemInstanceId);
    }

    private static string NormalizeDotaSlot(string slot)
    {
        if (string.IsNullOrWhiteSpace(slot))
        {
            return string.Empty;
        }

        var normalized = slot.Trim().ToLowerInvariant();

        // "slot_N" is a synthetic name for entries equipped through the GC without a
        // catalog slot name; treat it as unnamed so comparisons fall back to SlotId.
        return normalized.StartsWith("slot_", StringComparison.Ordinal) ? string.Empty : normalized;
    }

    private uint ResolveDotaSlotId(uint heroId, string heroName, string slot)
    {
        var normalizedSlot = NormalizeDotaSlot(slot);
        if (string.IsNullOrWhiteSpace(normalizedSlot))
        {
            return 0;
        }

        foreach (var candidateHeroName in ResolveDotaSlotHeroNames(heroId, heroName))
        {
            if (_state.DotaHeroSlots.TryGetValue(candidateHeroName, out var slots) &&
                slots.TryGetValue(normalizedSlot, out var slotId))
            {
                return slotId;
            }
        }

        return GuessDotaSlotId(normalizedSlot);
    }

    private static bool ShouldResolveDotaSlotId(ApiDotaEquipment equipped)
    {
        if (equipped.HeroId != 0 || equipped.SlotId == 0)
        {
            return true;
        }

        // Global loadout items such as terrain/map, announcers, couriers and
        // other account-wide cosmetics arrive from Dota with HeroId/Class 0 and
        // a concrete SlotId chosen by the client. The imported item catalog can
        // provide a readable slot name, but it does not define a hero slot table
        // for class 0. Re-resolving those names would collapse the original slot
        // to a guess (often 0), so after a restart the CSOEconItem.equippedState
        // no longer matches what Dota equipped. Preserve the client-provided
        // global SlotId and use the slot name only for grouping/admin display.
        return false;
    }

    private IEnumerable<string> ResolveDotaSlotHeroNames(uint heroId, string heroName)
    {
        if (!string.IsNullOrWhiteSpace(heroName))
        {
            yield return heroName.Trim();
        }

        if (heroId == 0)
        {
            yield break;
        }

        foreach (var pair in _state.DotaHeroIds)
        {
            if (pair.Value == heroId)
            {
                yield return pair.Key;
            }
        }
    }

    private ulong BuildDotaInventoryVersionLocked()
    {
        var importVersion = _state.DotaCosmetics.LastImportAt == DateTime.MinValue
            ? 0UL
            : (ulong)new DateTimeOffset(_state.DotaCosmetics.LastImportAt).ToUnixTimeMilliseconds();
        return Math.Max(WelcomeVersionSeed(), Math.Max(importVersion, _state.DotaCosmetics.EquipmentVersion));
    }

    private void TouchDotaEquipmentVersionLocked()
    {
        var now = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _state.DotaCosmetics.EquipmentVersion = Math.Max(now, _state.DotaCosmetics.EquipmentVersion + 1);
    }

    private void EnsureDotaHeroSlotsLoadedLocked()
    {
        if (_state.DotaHeroSlots.Count > 0)
        {
            return;
        }

        try
        {
            var dotaPath = string.IsNullOrWhiteSpace(_state.DotaCosmetics.DotaPath)
                ? DefaultDotaPath
                : _state.DotaCosmetics.DotaPath;
            var pakPath = ResolvePakPath(ResolveDotaContentPath(dotaPath));
            var heroesText = VpkTextReader.ReadText(pakPath, "scripts/npc/npc_heroes.txt");
            _state.DotaHeroSlots = DotaItemsGameParser.ParseHeroSlots(heroesText);
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException or FileNotFoundException)
        {
            _state.DotaHeroSlots = new Dictionary<string, Dictionary<string, uint>>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private static string ResolveDotaContentPath(string? requestedPath)
    {
        var raw = string.IsNullOrWhiteSpace(requestedPath) ? DefaultDotaPath : requestedPath.Trim().Trim('"');
        return Path.GetFullPath(raw);
    }

    private static string ResolvePakPath(string dotaPath)
    {
        var candidates = new[]
        {
            Path.Combine(dotaPath, "game", "dota", "pak01_dir.vpk"),
            Path.Combine(dotaPath, "dota", "pak01_dir.vpk"),
            Path.Combine(dotaPath, "pak01_dir.vpk")
        };

        var found = candidates.FirstOrDefault(File.Exists);
        if (found == null)
        {
            throw new FileNotFoundException("pak01_dir.vpk not found. Select the Dota 2 root or game\\dota.", candidates[0]);
        }

        return found;
    }

    private static uint GuessDotaSlotId(string slot)
    {
        if (string.IsNullOrWhiteSpace(slot))
        {
            return 0;
        }

        return slot.Trim().ToLowerInvariant() switch
        {
            "weapon" => 0,
            "offhand_weapon" => 1,
            "head" => 2,
            "shoulder" => 3,
            "arms" => 4,
            "back" => 5,
            "belt" => 6,
            "misc" => 7,
            "taunt" => 8,
            "ambient_effects" => 9,
            "summon" => 10,
            "ability" => 11,
            _ => 0
        };
    }

    private static ulong WelcomeVersionSeed() => 20;

    public static bool RunDotaCosmeticParserSelfCheck(Action<string>? write = null)
    {
        var heroIds = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase)
        {
            ["npc_dota_hero_phantom_assassin"] = 44
        };

        const string itemsText = """
            "items"
            {
                "10001"
                {
                    "name" "Self Check Hero Weapon"
                    "prefab" "wearable"
                    "item_slot" "weapon"
                    "item_quality" "immortal"
                    "used_by_heroes"
                    {
                        "npc_dota_hero_phantom_assassin" "1"
                    }
                }
                "10002"
                {
                    "name" "Self Check Courier"
                    "prefab" "courier"
                    "item_quality" "mythical"
                }
                "10003"
                {
                    "name" "Self Check Terrain"
                    "prefab" "terrain"
                    "item_quality" "unique"
                }
                "10004"
                {
                    "name" "Self Check Teleport Effect"
                    "prefab" "teleport_effect"
                    "item_quality" "rare"
                }
                "10010"
                {
                    "name" "Self Check Loading Screen"
                    "prefab" "loading_screen"
                    "item_slot" "head"
                }
                "10005"
                {
                    "name" "Self Check Tool"
                    "prefab" "tool"
                }
                "10006"
                {
                    "name" "Self Check Bundle"
                    "prefab" "bundle"
                }
                "10007"
                {
                    "name" "Self Check Gem"
                    "prefab" "socket_gem"
                }
                "10008"
                {
                    "name" "Self Check League"
                    "prefab" "league"
                }
                "10009"
                {
                    "name" "Self Check Default Courier"
                    "prefab" "courier"
                }
            }
            """;

        var parsed = DotaItemsGameParser.ParseItems(itemsText, heroIds).ToDictionary(item => item.DefIndex);
        var ok = Expect(parsed.ContainsKey(10001) && parsed[10001].HeroIds.SequenceEqual(new uint[] { 44 }) && parsed[10001].Slot == "weapon", "hero wearable imported with slot and hero id", write)
                 & Expect(parsed.ContainsKey(10002) && parsed[10002].HeroIds.Count == 0 && parsed[10002].Slot == "courier", "global courier imported without hero binding", write)
                 & Expect(parsed.ContainsKey(10003) && parsed[10003].Slot == "terrain", "global terrain imported from prefab", write)
                 & Expect(parsed.ContainsKey(10004) && parsed[10004].Slot == "teleport_effect", "global effect imported from prefab", write)
                 & Expect(parsed.ContainsKey(10010) && parsed[10010].Slot == "loading_screen", "global prefab overrides misleading hero slot", write)
                 & Expect(!parsed.ContainsKey(10005), "tools excluded", write)
                 & Expect(!parsed.ContainsKey(10006), "bundles excluded", write)
                 & Expect(!parsed.ContainsKey(10007), "socket gems excluded", write)
                 & Expect(!parsed.ContainsKey(10008), "league entries excluded", write)
                 & Expect(!parsed.ContainsKey(10009), "default global cosmetics excluded", write);

        write?.Invoke(ok ? "Dota cosmetic parser self-check passed." : "Dota cosmetic parser self-check failed.");
        return ok;
    }

    private static bool Expect(bool condition, string message, Action<string>? write)
    {
        write?.Invoke($"{(condition ? "OK" : "FAIL")}: {message}");
        return condition;
    }

    private static class VpkTextReader
    {
        private const uint Signature = 0x55AA1234;

        public static string ReadText(string dirPath, string vpkPath)
        {
            var wanted = vpkPath.Replace('\\', '/').TrimStart('/').ToLowerInvariant();
            using var stream = File.OpenRead(dirPath);
            using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: false);
            var signature = reader.ReadUInt32();
            if (signature != Signature)
            {
                throw new InvalidDataException($"Invalid VPK: signature {signature:X8}.");
            }

            var version = reader.ReadUInt32();
            var treeLength = reader.ReadUInt32();
            if (version == 2)
            {
                _ = reader.ReadUInt32();
                _ = reader.ReadUInt32();
                _ = reader.ReadUInt32();
                _ = reader.ReadUInt32();
            }

            var treeStart = stream.Position;
            while (stream.Position < treeStart + treeLength)
            {
                var extension = ReadNullString(reader);
                if (extension.Length == 0)
                {
                    break;
                }

                while (true)
                {
                    var directory = ReadNullString(reader);
                    if (directory.Length == 0)
                    {
                        break;
                    }

                    while (true)
                    {
                        var fileName = ReadNullString(reader);
                        if (fileName.Length == 0)
                        {
                            break;
                        }

                        _ = reader.ReadUInt32();
                        var preloadBytes = reader.ReadUInt16();
                        var archiveIndex = reader.ReadUInt16();
                        var entryOffset = reader.ReadUInt32();
                        var entryLength = reader.ReadUInt32();
                        _ = reader.ReadUInt16();

                        var preload = preloadBytes > 0 ? reader.ReadBytes(preloadBytes) : Array.Empty<byte>();
                        var fullPath = BuildPath(directory, fileName, extension);
                        if (!string.Equals(fullPath, wanted, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        var payload = new byte[preload.Length + entryLength];
                        Buffer.BlockCopy(preload, 0, payload, 0, preload.Length);

                        if (entryLength > 0)
                        {
                            var archivePath = archiveIndex == 0x7FFF
                                ? dirPath
                                : Path.Combine(Path.GetDirectoryName(dirPath)!,
                                    $"{Path.GetFileNameWithoutExtension(dirPath).Replace("_dir", string.Empty)}_{archiveIndex:D3}.vpk");

                            using var archive = File.OpenRead(archivePath);
                            archive.Position = entryOffset;
                            archive.ReadExactly(payload, preload.Length, (int)entryLength);
                        }

                        return Encoding.UTF8.GetString(payload);
                    }
                }
            }

            throw new FileNotFoundException($"{vpkPath} not found inside {dirPath}.", vpkPath);
        }

        private static string BuildPath(string directory, string fileName, string extension)
        {
            var name = extension == " " ? fileName : $"{fileName}.{extension}";
            return directory == " " ? name.ToLowerInvariant() : $"{directory}/{name}".ToLowerInvariant();
        }

        private static string ReadNullString(BinaryReader reader)
        {
            var bytes = new List<byte>(64);
            while (true)
            {
                var b = reader.ReadByte();
                if (b == 0)
                {
                    return Encoding.UTF8.GetString(bytes.ToArray());
                }

                bytes.Add(b);
            }
        }
    }

    private static class DotaItemsGameParser
    {
        private static readonly Regex UsedByHeroesRegex = new("\"used_by_heroes\"\\s*\\{(?<body>.*?)\\}", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex HeroNameRegex = new("\"(?<hero>npc_dota_hero_[^\"]+)\"\\s+\"?1\"?", RegexOptions.Compiled);

        private static readonly Dictionary<string, uint> QualityIds = new(StringComparer.OrdinalIgnoreCase)
        {
            ["normal"] = 0,
            ["genuine"] = 1,
            ["vintage"] = 3,
            ["unusual"] = 5,
            ["unique"] = 6,
            ["community"] = 7,
            ["developer"] = 8,
            ["selfmade"] = 9,
            ["customized"] = 10,
            ["strange"] = 11,
            ["completed"] = 12,
            ["haunted"] = 13,
            ["tournament"] = 14,
            ["favored"] = 15,
            ["ascendant"] = 16,
            ["autographed"] = 17,
            ["legacy"] = 18,
            ["exalted"] = 19,
            ["frozen"] = 20,
            ["corrupted"] = 21,
            ["lucky"] = 22,
            ["infused"] = 23
        };

        private static readonly Dictionary<string, uint> RarityIds = new(StringComparer.OrdinalIgnoreCase)
        {
            ["common"] = 1,
            ["uncommon"] = 2,
            ["rare"] = 3,
            ["mythical"] = 4,
            ["legendary"] = 5,
            ["immortal"] = 6,
            ["arcana"] = 7,
            ["ancient"] = 8,
            ["seasonal"] = 9
        };

        public static Dictionary<string, uint> ParseHeroIds(string text)
        {
            var heroes = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
            var bodyStart = FindSectionBodyStart(text, "DOTAHeroes");
            if (bodyStart < 0)
            {
                return heroes;
            }

            foreach (var child in EnumerateObjectChildren(text, bodyStart))
            {
                if (!child.Key.StartsWith("npc_dota_hero_", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var idText = FindField(child.Value, "HeroID");
                if (uint.TryParse(idText, out var heroId) && heroId != 0)
                {
                    heroes[child.Key] = heroId;
                }
            }

            return heroes;
        }

        public static Dictionary<string, Dictionary<string, uint>> ParseHeroSlots(string text)
        {
            var heroes = new Dictionary<string, Dictionary<string, uint>>(StringComparer.OrdinalIgnoreCase);
            var bodyStart = FindSectionBodyStart(text, "DOTAHeroes");
            if (bodyStart < 0)
            {
                return heroes;
            }

            foreach (var hero in EnumerateObjectChildren(text, bodyStart))
            {
                if (!hero.Key.StartsWith("npc_dota_hero_", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var slotsStart = FindSectionBodyStart(hero.Value, "ItemSlots");
                if (slotsStart < 0)
                {
                    continue;
                }

                var slots = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
                foreach (var slotBlock in EnumerateObjectChildren(hero.Value, slotsStart))
                {
                    var slotName = FindField(slotBlock.Value, "SlotName");
                    var slotIndexText = FindField(slotBlock.Value, "SlotIndex");
                    if (!string.IsNullOrWhiteSpace(slotName) &&
                        uint.TryParse(slotIndexText, out var slotIndex))
                    {
                        slots[slotName.Trim().ToLowerInvariant()] = slotIndex;
                    }
                }

                if (slots.Count > 0)
                {
                    heroes[hero.Key] = slots;
                }
            }

            return heroes;
        }

        public static List<ApiDotaItem> ParseItems(string text, Dictionary<string, uint> heroIds)
        {
            var items = new List<ApiDotaItem>();
            var bodyStart = FindSectionBodyStart(text, "items");
            if (bodyStart < 0)
            {
                throw new InvalidDataException("items_game.txt does not contain the items section.");
            }

            foreach (var child in EnumerateObjectChildren(text, bodyStart))
            {
                if (!uint.TryParse(child.Key, out var defIndex) || defIndex == 0)
                {
                    continue;
                }

                var block = child.Value;
                var name = FirstNonEmpty(FindField(block, "name"), child.Key);
                var prefab = FindField(block, "prefab");
                var imageInventory = FindField(block, "image_inventory");
                var slot = InferItemSlot(FirstNonEmpty(FindField(block, "item_slot"), FindField(block, "loadout_slot")), name, prefab, imageInventory);
                var quality = FirstNonEmpty(FindField(block, "item_quality"), "unique");
                var rarity = FindField(block, "item_rarity");
                var heroNames = ParseUsedByHeroes(block);
                var heroIdList = heroNames
                    .Select(hero => heroIds.TryGetValue(hero, out var heroId) ? heroId : 0)
                    .Where(heroId => heroId != 0)
                    .Distinct()
                    .OrderBy(heroId => heroId)
                    .ToList();

                var isDefault = name.Contains("default", StringComparison.OrdinalIgnoreCase) ||
                                prefab.Contains("default_item", StringComparison.OrdinalIgnoreCase);
                var normalizedPrefab = NormalizeImportedSlot(prefab);
                var isBundle = IsBundlePrefab(normalizedPrefab);
                var isTool = IsBlockedPrefab(normalizedPrefab) || IsBlockedSlot(slot);

                // Dota has two catalog families that both become type 1
                // CSOEconItem objects. Hero cosmetics declare used_by_heroes
                // and a hero slot. Account/global loadout cosmetics such as
                // couriers, terrains, HUDs, announcers, and effects do not
                // declare used_by_heroes; their equipable slot is identified by
                // prefab/item_slot instead. Keep both families in the owned
                // catalog so the TS GC can send the complete SO cache to the
                // client and to the dedicated server.
                if (isDefault || isTool || isBundle || !IsEquipableCosmetic(normalizedPrefab, slot, heroIdList))
                {
                    continue;
                }

                items.Add(new ApiDotaItem
                {
                    DefIndex = defIndex,
                    Name = name,
                    Prefab = prefab,
                    Slot = slot,
                    Quality = quality,
                    QualityId = QualityIds.TryGetValue(quality, out var qualityId) ? qualityId : 6,
                    Rarity = rarity,
                    RarityId = RarityIds.TryGetValue(rarity, out var rarityId) ? rarityId : 0,
                    ImageInventory = imageInventory,
                    IsBundle = isBundle,
                    IsDefault = isDefault,
                    IsTool = isTool,
                    HeroNames = heroNames,
                    HeroIds = heroIdList
                });
            }

            return items
                .GroupBy(item => item.DefIndex)
                .Select(group => group.First())
                .OrderBy(item => item.DefIndex)
                .ToList();
        }

        public static string InferItemSlot(string slot, string name, string prefab, string imageInventory)
        {
            var normalizedPrefab = NormalizeImportedSlot(prefab ?? string.Empty);
            if (GlobalLoadoutPrefabSlots.TryGetValue(normalizedPrefab, out var globalSlot))
            {
                var normalizedSlot = NormalizeImportedSlot(slot);
                return !string.IsNullOrWhiteSpace(normalizedSlot) && GlobalLoadoutSlots.Contains(normalizedSlot)
                    ? normalizedSlot
                    : globalSlot;
            }

            if (!string.IsNullOrWhiteSpace(slot))
            {
                return NormalizeImportedSlot(slot);
            }

            var fromName = InferItemSlotFromName(name);
            if (!string.IsNullOrWhiteSpace(fromName))
            {
                return fromName;
            }

            var source = $"{prefab ?? string.Empty} {imageInventory ?? string.Empty}".ToLowerInvariant();
            foreach (var candidate in KnownSlotNames)
            {
                if (source.Contains($"_{candidate}", StringComparison.OrdinalIgnoreCase) ||
                    source.Contains($"{candidate}_", StringComparison.OrdinalIgnoreCase) ||
                    source.Contains($"/{candidate}", StringComparison.OrdinalIgnoreCase))
                {
                    return candidate;
                }
            }

            return string.Empty;
        }

        private static string InferItemSlotFromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }

            if (name.TrimStart().StartsWith("Taunt:", StringComparison.OrdinalIgnoreCase))
            {
                return "taunt";
            }

            var match = Regex.Match(name, @"(?:^|[-:])\s*(?<slot>offhand weapon|ambient effects|ability effects \d+|weapon|head|shoulder|arms|back|belt|misc|taunt|summon|ability2?|armor|neck|legs|tail|costume|voice)\b", RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return string.Empty;
            }

            return NormalizeImportedSlot(match.Groups["slot"].Value);
        }

        private static string NormalizeImportedSlot(string slot)
        {
            return slot.Trim()
                .ToLowerInvariant()
                .Replace(' ', '_');
        }

        private static readonly string[] KnownSlotNames =
        {
            "offhand_weapon",
            "ambient_effects",
            "ability_effects_1",
            "ability_effects_2",
            "ability_effects_3",
            "ability_effects_4",
            "persona_selector",
            "hero_effigy",
            "hero_base",
            "weapon_persona_1",
            "offhand_weapon_persona_1",
            "head_persona_1",
            "armor_persona_1",
            "back_persona_1",
            "arms_persona_1",
            "tail_persona_1",
            "taunt_persona_1",
            "weapon",
            "head",
            "shoulder",
            "arms",
            "back",
            "belt",
            "misc",
            "taunt",
            "summon",
            "mount",
            "ability2",
            "ability",
            "ability1",
            "ability3",
            "ability4",
            "ability_ultimate",
            "armor",
            "neck",
            "legs",
            "tail",
            "costume",
            "voice",
            "announcer",
            "mega_kills",
            "courier",
            "courier_effect",
            "ward",
            "terrain",
            "weather",
            "hud_skin",
            "loading_screen",
            "cursor_pack",
            "music",
            "emblem",
            "versus_screen",
            "teleport_effect",
            "blink_effect",
            "streak_effect",
            "map_effect",
            "death_effect",
            "head_effect",
            "radiantcreeps",
            "direcreeps",
            "radiantsiegecreeps",
            "diresiegecreeps",
            "radianttowers",
            "diretowers",
            "roshan",
            "tormentor",
            "ancient",
            "summons",
            "heroic_statue",
            "pet_effigy",
            "multikill_banner"
        };

        private static readonly Dictionary<string, string> GlobalLoadoutPrefabSlots = new(StringComparer.OrdinalIgnoreCase)
        {
            ["announcer"] = "announcer",
            ["courier"] = "courier",
            ["courier_wearable"] = "courier",
            ["courier_effect"] = "courier_effect",
            ["cursor_pack"] = "cursor_pack",
            ["death_effect"] = "death_effect",
            ["direcreeps"] = "direcreeps",
            ["diresiegecreeps"] = "diresiegecreeps",
            ["diretowers"] = "diretowers",
            ["emblem"] = "emblem",
            ["head_effect"] = "head_effect",
            ["hud_skin"] = "hud_skin",
            ["loading_screen"] = "loading_screen",
            ["map_effect"] = "map_effect",
            ["music"] = "music",
            ["radiantcreeps"] = "radiantcreeps",
            ["radiantsiegecreeps"] = "radiantsiegecreeps",
            ["radianttowers"] = "radianttowers",
            ["roshan"] = "roshan",
            ["streak_effect"] = "streak_effect",
            ["terrain"] = "terrain",
            ["tormentor"] = "tormentor",
            ["versus_screen"] = "versus_screen",
            ["ward"] = "ward",
            ["weather"] = "weather",
            ["teleport_effect"] = "teleport_effect",
            ["blink_effect"] = "blink_effect"
        };

        private static readonly HashSet<string> BlockedPrefabs = new(StringComparer.OrdinalIgnoreCase)
        {
            "autograph_rune",
            "battle_pass",
            "compendium",
            "dynamic_recipe",
            "emoticon_tool",
            "event_game",
            "fantasy_ticket",
            "gift",
            "key",
            "league",
            "misc_tool",
            "pennant",
            "player_card",
            "recipe",
            "retired_treasure_chest",
            "socket_gem",
            "sticker",
            "sticker_capsule",
            "tool",
            "treasure_chest"
        };

        private static readonly HashSet<string> BlockedSlots = new(StringComparer.OrdinalIgnoreCase)
        {
            "tool",
            "socket_gem"
        };

        private static readonly HashSet<string> GlobalLoadoutSlots = new(KnownSlotNames.Where(slot =>
            GlobalLoadoutPrefabSlots.ContainsValue(slot) ||
            slot is "ancient" or "heroic_statue" or "multikill_banner" or "pet_effigy" or "summons"), StringComparer.OrdinalIgnoreCase);

        private static bool IsEquipableCosmetic(string normalizedPrefab, string slot, List<uint> heroIds)
        {
            if (GlobalLoadoutPrefabSlots.ContainsKey(normalizedPrefab) || GlobalLoadoutSlots.Contains(slot))
            {
                return true;
            }

            return heroIds.Count > 0 && !string.IsNullOrWhiteSpace(slot);
        }

        private static bool IsBundlePrefab(string normalizedPrefab)
        {
            return normalizedPrefab.Contains("bundle", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsBlockedPrefab(string normalizedPrefab)
        {
            return BlockedPrefabs.Contains(normalizedPrefab);
        }

        private static bool IsBlockedSlot(string slot)
        {
            return BlockedSlots.Contains(slot);
        }

        private static List<string> ParseUsedByHeroes(string block)
        {
            var match = UsedByHeroesRegex.Match(block);
            if (!match.Success)
            {
                return new List<string>();
            }

            return HeroNameRegex.Matches(match.Groups["body"].Value)
                .Select(m => m.Groups["hero"].Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(hero => hero)
                .ToList();
        }

        private static string FindField(string block, string field)
        {
            var regex = new Regex($"\"{Regex.Escape(field)}\"\\s+\"(?<value>[^\"]*)\"", RegexOptions.Singleline);
            var match = regex.Match(block);
            return match.Success ? match.Groups["value"].Value : string.Empty;
        }

        private static string FirstNonEmpty(params string[] values)
        {
            return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
        }

        private static int FindSectionBodyStart(string text, string sectionName)
        {
            var token = $"\"{sectionName}\"";
            var sectionIndex = text.IndexOf(token, StringComparison.OrdinalIgnoreCase);
            if (sectionIndex < 0)
            {
                return -1;
            }

            var braceIndex = text.IndexOf('{', sectionIndex + token.Length);
            return braceIndex < 0 ? -1 : braceIndex + 1;
        }

        private static IEnumerable<KeyValuePair<string, string>> EnumerateObjectChildren(string text, int bodyStart)
        {
            var index = bodyStart;
            while (index < text.Length)
            {
                SkipTrivia(text, ref index);
                if (index >= text.Length || text[index] == '}')
                {
                    yield break;
                }

                var key = ReadToken(text, ref index);
                if (string.IsNullOrEmpty(key))
                {
                    yield break;
                }

                SkipTrivia(text, ref index);
                if (index >= text.Length)
                {
                    yield break;
                }

                if (text[index] != '{')
                {
                    _ = ReadToken(text, ref index);
                    continue;
                }

                var blockStart = index;
                var blockEnd = FindMatchingBrace(text, index);
                if (blockEnd <= blockStart)
                {
                    yield break;
                }

                yield return new KeyValuePair<string, string>(key, text.Substring(blockStart, blockEnd - blockStart + 1));
                index = blockEnd + 1;
            }
        }

        private static int FindMatchingBrace(string text, int openBrace)
        {
            var depth = 0;
            var inString = false;
            for (var i = openBrace; i < text.Length; i++)
            {
                var c = text[i];
                if (c == '"' && (i == 0 || text[i - 1] != '\\'))
                {
                    inString = !inString;
                    continue;
                }

                if (inString)
                {
                    continue;
                }

                if (c == '{')
                {
                    depth++;
                }
                else if (c == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private static void SkipTrivia(string text, ref int index)
        {
            while (index < text.Length)
            {
                if (char.IsWhiteSpace(text[index]))
                {
                    index++;
                    continue;
                }

                if (index + 1 < text.Length && text[index] == '/' && text[index + 1] == '/')
                {
                    index += 2;
                    while (index < text.Length && text[index] != '\n')
                    {
                        index++;
                    }

                    continue;
                }

                break;
            }
        }

        private static string ReadToken(string text, ref int index)
        {
            SkipTrivia(text, ref index);
            if (index >= text.Length)
            {
                return string.Empty;
            }

            if (text[index] == '"')
            {
                index++;
                var start = index;
                while (index < text.Length && !(text[index] == '"' && text[index - 1] != '\\'))
                {
                    index++;
                }

                var value = text.Substring(start, Math.Max(0, index - start));
                if (index < text.Length)
                {
                    index++;
                }

                return value;
            }

            var tokenStart = index;
            while (index < text.Length && !char.IsWhiteSpace(text[index]) && text[index] != '{' && text[index] != '}')
            {
                index++;
            }

            return text.Substring(tokenStart, index - tokenStart);
        }
    }
}
