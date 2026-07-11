-- Dota item/equip flow. The rules live here and are hot-reloadable; C# only
-- provides dumb storage and codecs:
--   gc.DotaResolveItemDef(itemIdString)      -> defIndex (0 = unknown)
--   gc.DotaGetEquipmentJson()                -> JSON array of equipment entries
--   gc.DotaSetEquipmentJson(json)            -> persist list (version bump + save)
--   gc.DotaCatalogItemJson(defIndex)         -> catalog info for one item
--   gc.DotaItemsClientUpdateFor(csv)         -> SOMultipleObjects payload for defs
--   gc.DotaQueueItemsToServerFor(server,csv) -> destroy/create singles to server
--   runtime.JsonToTable(json) / runtime.TableToJson(table)
--
-- Equipment entry fields (JSON <-> Lua table): HeroId, HeroName, Slot, SlotId,
-- DefIndex, Style. ItemId is recomputed server-side, never handled here.

DOTA_ITEMS = DOTA_ITEMS or {}

local UNEQUIP_SLOT = 65535

local function load_equipment()
    local json = gc.DotaGetEquipmentJson()
    if json == nil or json == "" then
        return {}
    end

    local ok, list = pcall(function() return runtime.JsonToTable(json) end)
    if not ok or list == nil then
        runtime.Log("items: failed to parse equipment json")
        return {}
    end

    return list
end

local function save_equipment(list)
    local ok = gc.DotaSetEquipmentJson(runtime.TableToJson(list))
    if not ok then
        runtime.Log("items: failed to persist equipment")
    end

    return ok
end

local function mark_changed(changed, def)
    if def ~= nil and def ~= 0 then
        changed[def] = true
    end
end

local function changed_csv(changed)
    local parts = {}
    for def in pairs(changed) do
        parts[#parts + 1] = tostring(def)
    end

    return table.concat(parts, ",")
end

-- Applies one CMsgAdjustItemEquippedState against the equipment list.
-- Mirrors real GC semantics:
--   item_id == 0            -> clear whatever occupies (hero, slot)
--   new_slot == 0xFFFF      -> unequip this item from this hero
--   otherwise               -> replace (hero, slot) and any previous slot this
--                              item held on the SAME hero (equipping on one hero
--                              must not touch other heroes).
local function apply_equip(list, changed, hero, slot, def, style)
    local result = {}
    for _, entry in ipairs(list) do
        local remove
        if slot == UNEQUIP_SLOT then
            remove = def ~= 0 and entry.HeroId == hero and entry.DefIndex == def
        else
            local same_slot = entry.HeroId == hero and entry.SlotId == slot
            local same_item = def ~= 0 and entry.HeroId == hero and entry.DefIndex == def
            remove = same_slot or same_item
        end

        if remove then
            mark_changed(changed, entry.DefIndex)
        else
            result[#result + 1] = entry
        end
    end

    if def ~= 0 and slot ~= UNEQUIP_SLOT then
        if style == 255 then
            style = 0
        end

        result[#result + 1] = { HeroId = hero, SlotId = slot, DefIndex = def, Style = style }
        mark_changed(changed, def)
    end

    return result
end

local function send_item_updates(changed)
    local csv = changed_csv(changed)
    if csv == "" then
        return false
    end

    local update = gc.DotaItemsClientUpdateFor(csv)
    if update ~= nil and update ~= "" then
        gc.Proto(MSG.SOCacheUpdated, update)
    end

    if dota_lobby_current_server ~= nil then
        local server_steam = dota_lobby_current_server()
        if server_steam ~= nil and server_steam ~= "" and server_steam ~= "0" then
            gc.DotaQueueItemsToServerFor(server_steam, csv)
        end
    end

    return true
end

local function handle_equip_items()
    local list = load_equipment()
    local changed = {}

    local count = gc.FieldCount(1)
    for i = 1, count do
        local equip = gc.ReadBytesAt(1, i)
        local item_id = runtime.ProtoVarintString(equip, 1, 1, "0")
        local hero = runtime.ProtoVarint(equip, 2, 1, 0)
        local slot = runtime.ProtoVarint(equip, 3, 1, 0)
        local style = runtime.ProtoVarint(equip, 4, 1, 0)

        local def = 0
        if item_id ~= "0" then
            def = gc.DotaResolveItemDef(item_id)
            if def == 0 then
                runtime.Log("equip: unknown item id " .. item_id)
            end
        end

        runtime.Log("equip def=" .. tostring(def) .. " hero=" .. tostring(hero) .. " slot=" .. tostring(slot) .. " style=" .. tostring(style))
        list = apply_equip(list, changed, hero, slot, def, style)
    end

    if next(changed) ~= nil then
        save_equipment(list)
        send_item_updates(changed)
    end

    return gc.Reply(MSG.ClientToGCEquipItemsResponse, gc.DotaEquipItemsResponse())
end

local function handle_set_item_style()
    local item_id = gc.ReadVarintString(1, "0")
    local style = gc.ReadVarint(2, 0)
    local def = gc.DotaResolveItemDef(item_id)
    runtime.Log("set item style def=" .. tostring(def) .. " style=" .. tostring(style))

    local list = load_equipment()
    local changed = {}
    if def ~= 0 then
        for _, entry in ipairs(list) do
            if entry.DefIndex == def then
                entry.Style = (style == 255) and 0 or style
                mark_changed(changed, def)
            end
        end
    end

    local ok = next(changed) ~= nil
    if ok then
        save_equipment(list)
        send_item_updates(changed)
    end

    return gc.Reply(MSG.ClientToGCSetItemStyleResponse, gc.DotaSetItemStyleResponse(ok))
end

local function handle_cache_refresh()
    gc.Proto(MSG.SOCacheSubscribed, gc.GameSoCacheSubscribed())
    gc.Proto(MSG.SOCacheSubscribed, gc.EconSoCacheSubscribed())
    return true
end

function dota_items_handle(message_type)
    local handlers = {
        [MSG.ClientToGCEquipItems] = handle_equip_items,
        [MSG.ClientToGCSetItemStyle] = handle_set_item_style,
        [MSG.SOCacheSubscriptionRefresh] = handle_cache_refresh
    }

    local handler = handlers[message_type]
    if handler == nil then
        return false
    end

    return handler()
end
