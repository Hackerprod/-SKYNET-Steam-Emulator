DOTA = DOTA or {}
DOTA.state = DOTA.state or {
    lobbies = {},
    by_steam = {},
    by_server = {},
    seq = 0,
    match_seq = 0
}
DOTA.state.closed_lobbies_by_steam = DOTA.state.closed_lobbies_by_steam or {}

local LOBBY_OBJECT_TYPE = 2004
local LOBBY_INVITE_OBJECT_TYPE = 2013
local LOBBY_PERSONA_OBJECT_TYPE = 2014
local LOBBY_BROADCAST_OBJECT_TYPE = 2015
local LOBBY_MEMBER_OBJECT_TYPE = 2016

local TEAM_GOOD = 0
local TEAM_BAD = 1
local TEAM_SPECTATOR = 3
local TEAM_POOL = 4
local TEAM_NONE = 5

local LOBBY_UI = 0
local LOBBY_SERVER_SETUP = 1
local LOBBY_RUN = 2
local LOBBY_POSTGAME = 3
local LOBBY_SERVER_ASSIGN = 6

local GAME_INIT = 0
local GAME_WAIT_FOR_PLAYERS = 1
local GAME_HERO_SELECTION = 2

local CONNECTED_REASON_HEARTBEAT = 1
local CONNECTED_REASON_GAME_STATE = 2
local CONNECTED_REASON_PLAYER_CONNECTED = 4
local CONNECTED_REASON_PLAYER_HERO = 5
local CONNECTED_REASON_PLAYER_DISCONNECTED_CONSEQUENCES = 6
local CONNECTED_REASON_PLAYER_DISCONNECTED_NO_CONSEQUENCES = 7
local CONNECTED_REASON_MASS_DISCONNECT = 11

local LEAVER_NONE = 0
local LEAVER_DISCONNECTED = 1
local LEAVER_DISCONNECTED_TOO_LONG = 2
local LEAVER_ABANDONED = 3

local JOIN_SUCCESS = 0
local JOIN_ALREADY_IN_GAME = 1
local JOIN_INVALID_LOBBY = 2
local JOIN_BAD_PASSWORD = 3
local JOIN_FULL = 9

local MAX_MEMBERS = 20
local LOBBY_TIMEOUT_SECONDS = 3600
local POSTGAME_CLEANUP_SECONDS = 45
local RECONNECT_TIMEOUT_SECONDS = 600
local ACTIVE_EVENT_ID = 54

local DISABLED_RANDOM_HERO_BITS = { 0, 194756608, 1780819944 }
local LOBBY_EVENT_IDS = { 19, 26, 39, ACTIVE_EVENT_ID, 57 }
local LOBBY_SELECTION_PRIORITY_ID = 8821
local LOBBY_STARTUP_HERO_IDS = {
    1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
    11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
    21, 22, 23, 25, 26, 27, 28, 29, 30, 31,
    32, 33, 34, 35, 36, 37, 38, 39, 40, 41,
    42, 43, 44, 45, 46, 47, 48, 49, 50, 51,
    52, 53, 54, 55, 56, 57, 58, 59, 60, 61,
    62, 63, 64, 65, 66, 67, 68, 69, 70, 71,
    72, 73, 74, 75, 76, 77, 78, 79, 80, 81,
    82, 83, 84, 85, 86, 87, 88, 89, 90, 91,
    92, 93, 94, 95, 96, 97, 98, 99, 100, 101,
    102, 103, 104, 105, 106, 107, 108, 109, 110, 111,
    112, 113, 114, 119, 120, 121, 123, 126, 128, 129,
    135, 136, 137, 138, 145
}
local LOBBY_STARTUP_HERO_PROGRESS = {
    { 3, 1400 }, { 5, 10244 }, { 6, 300 }, { 7, 1075 }, { 9, 75 },
    { 13, 25 }, { 14, 675 }, { 16, 525 }, { 17, 25 }, { 20, 75 },
    { 21, 4540 }, { 22, 1075 }, { 25, 50 }, { 26, 2650 }, { 27, 5404 },
    { 30, 4179 }, { 31, 1050 }, { 32, 100 }, { 33, 750 }, { 34, 75 },
    { 35, 1050 }, { 36, 1025 }, { 37, 1550 }, { 39, 150 }, { 40, 50 },
    { 43, 400 }, { 44, 50 }, { 45, 1300 }, { 48, 450 }, { 50, 1900 },
    { 53, 1500 }, { 54, 75 }, { 58, 250 }, { 60, 100 }, { 62, 750 },
    { 63, 125 }, { 64, 3574 }, { 65, 350 }, { 68, 3025 }, { 71, 700 },
    { 75, 1575 }, { 76, 50 }, { 77, 550 }, { 78, 100 }, { 79, 100 },
    { 81, 100 }, { 83, 1600 }, { 84, 1000 }, { 85, 150 }, { 86, 525 },
    { 87, 550 }, { 88, 100 }, { 90, 100 }, { 91, 75 }, { 92, 125 },
    { 99, 575 }, { 100, 25 }, { 101, 950 }, { 104, 50 }, { 105, 1050 },
    { 108, 50 }, { 111, 650 }, { 112, 1150 }, { 119, 4299 }, { 121, 6164 },
    { 126, 75 }, { 128, 350 }, { 129, 350 }, { 138, 1725 }, { 400, 1 },
    { 401, 1 }, { 402, 1 }, { 410, 6 }, { 460, 4 }, { 461, 3 },
    { 497, 241 }, { 507, 155 }
}
local LOBBY_ADDITIONAL_ACCOUNT_RANGES = {
    { 170, 170 }, { 258, 258 }, { 595, 602 }, { 805, 807 }, { 818, 819 },
    { 3001, 3002 }, { 5001, 5004 }, { 6001, 6002 }, { 7001, 7002 },
    { 9001, 9002 }, { 14001, 14002 }, { 16001, 16002 }, { 20001, 20002 },
    { 21001, 21004 }, { 22001, 22002 }, { 25001, 25002 }, { 26001, 26002 },
    { 27001, 27004 }, { 30001, 30004 }, { 31001, 31002 }, { 32001, 32002 },
    { 33001, 33002 }, { 34001, 34002 }, { 35001, 35002 }, { 36001, 36002 },
    { 37001, 37002 }, { 39001, 39002 }, { 40001, 40002 }, { 43001, 43002 },
    { 44001, 44002 }, { 45001, 45002 }, { 48001, 48002 }, { 50001, 50002 },
    { 53001, 53002 }, { 54001, 54002 }, { 58001, 58002 }, { 60001, 60002 },
    { 62001, 62002 }, { 63001, 63002 }, { 64001, 64004 }, { 65001, 65002 },
    { 68001, 68004 }, { 71001, 71002 }, { 75001, 75002 }, { 76001, 76002 },
    { 77001, 77002 }, { 78001, 78002 }, { 79001, 79002 }, { 81001, 81002 },
    { 83001, 83002 }, { 84001, 84002 }, { 85001, 85002 }, { 86001, 86002 },
    { 87001, 87002 }, { 88001, 88002 }, { 90001, 90002 }, { 91001, 91002 },
    { 92001, 92002 }, { 99001, 99002 }, { 101001, 101002 }, { 104001, 104002 },
    { 105001, 105002 }, { 108001, 108002 }, { 111001, 111002 },
    { 112001, 112002 }, { 119001, 119004 }, { 121001, 121004 },
    { 126001, 126002 }, { 128001, 128002 }, { 129001, 129002 },
    { 138001, 138002 }
}

local function state()
    return DOTA.state
end

local function current_steam()
    return tostring(gc.SteamIdString or "0")
end

local function current_account()
    return tonumber(gc.AccountId or 0) or 0
end

local function current_name()
    local name = tostring(gc.PersonaName or "")
    if name == "" then
        return "User" .. tostring(current_account())
    end

    return name
end

-- Source IP of the machine talking to the GC right now (the launcher's address
-- as seen by the server). Empty when unavailable.
local function current_ip()
    return tostring(gc.ClientIp or "")
end

-- Only addresses another player can actually reach are useful for the connect
-- string; loopback means "same machine as the SKYNET server", which a remote
-- player cannot use.
local function is_routable_ip(ip)
    return ip ~= nil and ip ~= "" and ip ~= "127.0.0.1" and ip ~= "::1" and ip ~= "0.0.0.0"
end

local function bump_seq()
    local s = state()
    s.seq = (s.seq or 0) + 1
    return s.seq
end

local function refresh_lobby(lobby)
    lobby.version = PB.next_id()
    lobby.last_activity = PB.now()
    lobby.seq = bump_seq()
end

local function member_count(lobby)
    local count = 0
    for _, _ in pairs(lobby.members) do
        count = count + 1
    end

    return count
end

local function owner_soid(owner_type, owner_id)
    return PB.cat(PB.v(1, owner_type), PB.vs(2, owner_id))
end

local function subscribed_type(type_id, ...)
    local parts = { PB.v(1, type_id) }
    local payloads = { ... }
    for _, payload in ipairs(payloads) do
        parts[#parts + 1] = PB.bytes(2, payload)
    end

    return PB.cat(table.unpack(parts))
end

local function single_object(type_id, payload)
    return PB.cat(PB.v(1, type_id), PB.bytes(2, payload))
end

local function lobby_member_payload(member)
    local parts = {
        PB.f64s(1, member.steam),
        PB.v(3, member.team or TEAM_POOL),
        PB.v(7, member.slot or 0)
    }

    if (member.hero or 0) ~= 0 then
        parts[#parts + 1] = PB.v(8, member.hero)
    end

    if (member.leaver or 0) ~= 0 then
        parts[#parts + 1] = PB.v(16, member.leaver)
    end

    if (member.coach or TEAM_NONE) ~= TEAM_NONE then
        parts[#parts + 1] = PB.v(23, member.coach)
    end

    return PB.cat(table.unpack(parts))
end

local function selection_priority_rule(lobby)
    local choice = PB.v(1, 0)
    return PB.cat(PB.v(1, LOBBY_SELECTION_PRIORITY_ID), PB.bytes(2, choice))
end

local function lobby_team_details(index)
    if index == 1 then
        return PB.cat(
            PB.str(1, "SKYNET"),
            PB.bytes(3, ""),
            PB.v(4, 7733573),
            PB.vs(5, "3255294647392078090"),
            PB.vs(6, "7163376947542189088"),
            PB.vs(7, "7954877705993612385"),
            PB.v(8, 0),
            PB.bytes(20, ""),
            PB.bytes(21, "")
        )
    end

    return PB.v(8, 0)
end

local function member_order(lobby)
    local ordered = {}
    for _, steam in ipairs(lobby.order) do
        if lobby.members[steam] ~= nil then
            ordered[#ordered + 1] = lobby.members[steam]
        end
    end

    return ordered
end

local function lobby_object(lobby)
    local running_with_direct_connect = (lobby.state or LOBBY_UI) == LOBBY_RUN and
        lobby.connect ~= nil and lobby.connect ~= ""
    -- Dota treats direct-connect RUN lobbies without Valve VAC policy as LAN
    -- transport for UI/auth warning purposes.  Keep lobby.lan semantic state
    -- unchanged; this only affects the serialized RUN cache sent to clients.
    local encoded_lan = (lobby.lan or running_with_direct_connect) and 1 or 0
    local parts = {
        PB.vs(1, lobby.id),
        PB.v(3, lobby.game_mode or 1),
        PB.v(4, lobby.state or LOBBY_UI),
        PB.f64s(11, lobby.leader_steam),
        PB.v(12, 1),
        PB.v(13, lobby.allow_cheats and 1 or 0),
        PB.v(14, 0),
        PB.str(16, lobby.name or "Room 1"),
        PB.v(21, lobby.region or 0),
        PB.v(28, lobby.game_state or GAME_INIT),
        PB.v(31, lobby.allow_spectating and 1 or 0),
        PB.v(36, 3),
        PB.v(42, 0),
        PB.v(43, 0),
        PB.v(44, 0),
        PB.v(46, 0),
        PB.v(47, 0),
        PB.v(48, 0),
        PB.v(51, 0),
        PB.v(53, lobby.dota_tv_delay or 0),
        PB.v(57, encoded_lan),
        PB.bytes(62, selection_priority_rule(lobby)),
        PB.v(75, lobby.visibility or 0),
        PB.v(82, 0),
        PB.v(88, 0),
        PB.v(93, 3),
        PB.v(94, 0),
        PB.v(95, 0),
        PB.v(97, 0),
        PB.v(110, 0),
        PB.v(113, 0)
    }

    if (lobby.state or LOBBY_UI) ~= LOBBY_UI then
        parts[#parts + 1] = PB.bytes(17, lobby_team_details(1))
        parts[#parts + 1] = PB.bytes(17, lobby_team_details(2))
        parts[#parts + 1] = PB.v(103, ACTIVE_EVENT_ID)
        parts[#parts + 1] = PB.v(104, ACTIVE_EVENT_ID)
    end

    if lobby.connect ~= nil and lobby.connect ~= "" then
        parts[#parts + 1] = PB.str(5, lobby.connect)
    end

    if running_with_direct_connect then
        parts[#parts + 1] = PB.v(65, 0)
    end

    if lobby.server_steam ~= nil and lobby.server_steam ~= "0" then
        parts[#parts + 1] = PB.f64s(6, lobby.server_steam)
    end

    if lobby.match_id ~= nil and lobby.match_id ~= "0" then
        parts[#parts + 1] = PB.vs(30, lobby.match_id)
    end

    if lobby.match_outcome ~= nil and lobby.match_outcome ~= 0 then
        parts[#parts + 1] = PB.v(70, lobby.match_outcome)
    end

    if (lobby.game_start_time or 0) ~= 0 then
        parts[#parts + 1] = PB.v(87, lobby.game_start_time)
    end

    local index = 0
    for _, member in ipairs(member_order(lobby)) do
        parts[#parts + 1] = PB.bytes(120, lobby_member_payload(member))
        parts[#parts + 1] = PB.v(121, index)
        index = index + 1
    end

    -- The old coordinator retains voluntarily abandoned non-leaders in
    -- v2_left_members (field 7), while disconnected players stay in all_members
    -- so the same SteamID can reconnect to the running match.
    for _, member in ipairs(lobby.left_members or {}) do
        parts[#parts + 1] = PB.bytes(7, lobby_member_payload(member))
    end

    parts[#parts + 1] = PB.v(127, 0)
    parts[#parts + 1] = PB.v(128, PB.now())
    return PB.cat(table.unpack(parts))
end

local function lobby_persona_object(lobby)
    local parts = {}
    for _, member in ipairs(member_order(lobby)) do
        parts[#parts + 1] = PB.bytes(1, PB.str(1, member.name or ""))
    end

    parts[#parts + 1] = PB.v(2, 0)
    return PB.cat(table.unpack(parts))
end

local function plus_subscription_data(member)
    local parts = {}
    for _, row in ipairs(LOBBY_STARTUP_HERO_PROGRESS) do
        parts[#parts + 1] = PB.bytes(1, PB.cat(PB.v(1, row[1]), PB.v(2, row[2])))
    end

    return PB.cat(table.unpack(parts))
end

local function additional_startup_range(row)
    return PB.cat(PB.v(1, row[1]), PB.v(2, row[2]))
end

local function additional_startup_account_data(member)
    local parts = {
        PB.v(1, member.account or 0),
        PB.bytes(2, plus_subscription_data(member))
    }

    for _, row in ipairs(LOBBY_ADDITIONAL_ACCOUNT_RANGES) do
        parts[#parts + 1] = PB.bytes(3, additional_startup_range(row))
    end

    return PB.cat(table.unpack(parts))
end

local function lobby_broadcast_object(lobby)
    local parts = { PB.bytes(1, "") }
    if (lobby.state or LOBBY_UI) == LOBBY_UI then
        return PB.cat(table.unpack(parts))
    end

    for _, member in ipairs(member_order(lobby)) do
        local extra = PB.cat(
            PB.v(1, MSG.LobbyAdditionalAccountData),
            PB.bytes(2, additional_startup_account_data(member))
        )
        parts[#parts + 1] = PB.bytes(2, extra)
    end

    return PB.cat(table.unpack(parts))
end

local function lobby_event_points(lobby, event_id)
    local parts = { PB.v(1, event_id) }
    for _, member in ipairs(member_order(lobby)) do
        parts[#parts + 1] = PB.bytes(2, PB.cat(
            PB.v(1, member.account or 0),
            PB.v(2, 0),
            PB.v(3, 0),
            PB.v(4, 1),
            PB.v(7, 0),
            PB.vs(12, "0"),
            PB.v(26, 0),
            PB.v(27, 0),
            PB.v(28, 0)
        ))
    end

    return PB.cat(table.unpack(parts))
end

local function lobby_member_summary_object(lobby)
    local parts = {}
    for _, member in ipairs(member_order(lobby)) do
        local entry_parts = {
            PB.f64s(1, member.steam),
            PB.v(3, 0),
            PB.v(9, 0),
            PB.v(10, 1),
            PB.v(11, 1),
            PB.v(12, 0),
            PB.v(13, 0)
        }

        if (lobby.state or LOBBY_UI) ~= LOBBY_UI then
            for _, mask in ipairs(DISABLED_RANDOM_HERO_BITS) do
                entry_parts[#entry_parts + 1] = PB.f32(16, mask)
            end
        end

        entry_parts[#entry_parts + 1] = PB.v(19, 75)
        entry_parts[#entry_parts + 1] = PB.v(19, 0)
        entry_parts[#entry_parts + 1] = PB.v(19, 0)
        entry_parts[#entry_parts + 1] = PB.v(19, 0)

        local entry = PB.cat(table.unpack(entry_parts))
        parts[#parts + 1] = PB.bytes(1, entry)
    end

    parts[#parts + 1] = PB.f32(2, 0)
    if (lobby.state or LOBBY_UI) ~= LOBBY_UI then
        for _, event_id in ipairs(LOBBY_EVENT_IDS) do
            parts[#parts + 1] = PB.bytes(3, lobby_event_points(lobby, event_id))
        end
    end

    return PB.cat(table.unpack(parts))
end

local function lobby_so_cache_subscribed(lobby)
    return PB.cat(
        PB.bytes(2, subscribed_type(LOBBY_OBJECT_TYPE, lobby_object(lobby))),
        PB.bytes(2, subscribed_type(LOBBY_INVITE_OBJECT_TYPE, "")),
        PB.bytes(2, subscribed_type(LOBBY_PERSONA_OBJECT_TYPE, lobby_persona_object(lobby))),
        PB.bytes(2, subscribed_type(LOBBY_BROADCAST_OBJECT_TYPE, lobby_broadcast_object(lobby))),
        PB.bytes(2, subscribed_type(LOBBY_MEMBER_OBJECT_TYPE, lobby_member_summary_object(lobby))),
        PB.f64s(3, lobby.version),
        PB.bytes(4, owner_soid(3, lobby.id))
    )
end

local function lobby_server_so_cache_subscribed(lobby)
    return lobby_so_cache_subscribed(lobby)
end

local function lobby_so_cache_unsubscribed(lobby)
    return PB.bytes(2, owner_soid(3, lobby.id))
end

local function lobby_single_object(lobby)
    return PB.cat(
        PB.v(2, LOBBY_OBJECT_TYPE),
        PB.bytes(3, lobby_object(lobby)),
        PB.f64s(4, lobby.version),
        PB.bytes(5, owner_soid(3, lobby.id))
    )
end

local function lobby_multiple_objects(lobby)
    return PB.cat(
        PB.bytes(2, subscribed_type(LOBBY_BROADCAST_OBJECT_TYPE, lobby_broadcast_object(lobby))),
        PB.bytes(2, subscribed_type(LOBBY_OBJECT_TYPE, lobby_object(lobby))),
        PB.bytes(2, subscribed_type(LOBBY_PERSONA_OBJECT_TYPE, lobby_persona_object(lobby))),
        PB.bytes(2, subscribed_type(LOBBY_INVITE_OBJECT_TYPE, "")),
        PB.bytes(2, subscribed_type(LOBBY_MEMBER_OBJECT_TYPE, lobby_member_summary_object(lobby))),
        PB.f64s(3, lobby.version),
        PB.bytes(6, owner_soid(3, lobby.id))
    )
end

local function lobby_server_multiple_objects(lobby)
    return lobby_multiple_objects(lobby)
end

local function queue_proto(steam, message_type, payload)
    steam = tostring(steam or "0")
    if steam == current_steam() then
        return gc.Proto(message_type, payload)
    end

    if state().by_server[steam] ~= nil then
        return gc.QueueToPollString(steam, message_type, payload)
    end

    return gc.QueueToString(steam, message_type, payload)
end

local function remember_closed_lobby(steam, lobby_id)
    steam = tostring(steam or "0")
    lobby_id = tostring(lobby_id or "0")
    if steam == "0" or lobby_id == "0" then
        return
    end

    local pending = state().closed_lobbies_by_steam[steam]
    if pending == nil then
        pending = {}
        state().closed_lobbies_by_steam[steam] = pending
    end

    for _, existing in ipairs(pending) do
        if tostring(existing) == lobby_id then
            return
        end
    end

    pending[#pending + 1] = lobby_id
end

local function emit_pending_closed_lobbies()
    local steam = current_steam()
    local pending = state().closed_lobbies_by_steam[steam]
    if pending == nil or #pending == 0 then
        return false
    end

    for _, lobby_id in ipairs(pending) do
        local owner = owner_soid(3, lobby_id)
        gc.Proto(MSG.SOCacheUnsubscribed, PB.bytes(2, owner))
        runtime.Log("emit_closed_lobby steam=" .. steam .. " lobby=" .. tostring(lobby_id))
    end

    state().closed_lobbies_by_steam[steam] = nil
    return true
end

local function parse_json_table(json)
    if json == nil or json == "" then
        return nil
    end

    local ok, value = pcall(function() return runtime.JsonToTable(json) end)
    return ok and value or nil
end

local function lobby_invite_records(json)
    return parse_json_table(json) or {}
end

local function lobby_invite_object(record, metadata)
    local parts = {
        PB.vs(1, record.lobbyId or "0"),
        PB.f64s(2, record.senderSteamId or "0"),
        PB.str(3, metadata.senderName or ""),
        PB.f64s(6, record.inviteGid or "0")
    }

    for _, member in ipairs(metadata.members or {}) do
        parts[#parts + 1] = PB.bytes(4, PB.cat(
            PB.str(1, member.name or ""),
            PB.f64s(2, member.steam or "0")
        ))
    end

    local custom_game_id = tostring(metadata.customGameId or "0")
    local custom_game_crc = tostring(metadata.customGameCrc or "0")
    local custom_game_timestamp = tonumber(metadata.customGameTimestamp or 0) or 0
    if custom_game_id ~= "0" then
        parts[#parts + 1] = PB.vs(5, custom_game_id)
    end
    if custom_game_crc ~= "0" then
        parts[#parts + 1] = PB.f64s(7, custom_game_crc)
    end
    if custom_game_timestamp ~= 0 then
        parts[#parts + 1] = PB.f32(8, custom_game_timestamp)
    end

    return PB.cat(table.unpack(parts))
end

local function lobby_invite_subscribed(record)
    local metadata = parse_json_table(record.payloadJson) or {}
    return PB.cat(
        PB.bytes(2, subscribed_type(LOBBY_INVITE_OBJECT_TYPE, lobby_invite_object(record, metadata))),
        PB.f64s(3, record.inviteGid or PB.next_id()),
        PB.bytes(4, owner_soid(4, record.lobbyId or "0"))
    )
end

local function lobby_invite_unsubscribed(lobby_id)
    return PB.bytes(2, owner_soid(4, lobby_id or "0"))
end

local function queue_lobby_invite_record(record)
    if record == nil or tostring(record.targetSteamId or "0") == "0" then
        return false
    end

    return queue_proto(record.targetSteamId, MSG.SOCacheSubscribed, lobby_invite_subscribed(record))
end

local function queue_lobby_invite_removed(record)
    if record == nil or tostring(record.targetSteamId or "0") == "0" then
        return false
    end

    return queue_proto(record.targetSteamId, MSG.SOCacheUnsubscribed,
        lobby_invite_unsubscribed(record.lobbyId))
end

local function invitation_created(group_id, target_steam, user_offline)
    return PB.cat(
        PB.vs(1, group_id or "0"),
        PB.f64s(2, target_steam or "0"),
        PB.v(3, user_offline and 1 or 0)
    )
end

local function batch_player_resources_payload()
    local parts = {}
    local count = gc.FieldCount(1)

    if count == 0 then
        count = 1
        local row = PB.cat(
            PB.v(1, current_account()),
            PB.v(2, 0),
            PB.v(3, 0),
            PB.v(4, 0),
            PB.v(5, 0),
            PB.v(6, 0)
        )
        parts[#parts + 1] = PB.bytes(6, row)
    else
        for i = 1, count do
            local account = gc.ReadVarintAt(1, i, current_account())
            local row = PB.cat(
                PB.v(1, account),
                PB.v(2, 0),
                PB.v(3, 0),
                PB.v(4, 0),
                PB.v(5, 0),
                PB.v(6, 0)
            )
            parts[#parts + 1] = PB.bytes(6, row)
        end
    end

    return PB.cat(table.unpack(parts)), count
end

local function emit_pending_batch_player_resources(lobby)
    local pending = state().pending_batch_player_resources
    if pending == nil or pending.sent == true then
        return false
    end

    local target = tostring(pending.steam or "0")
    if target == "0" or target == "" then
        target = tostring(lobby.server_steam or current_steam())
    end

    runtime.Log("batch_player_resources emit target=" .. target .. " job=" .. tostring(pending.job))
    if target == current_steam() then
        gc.MessageWithTargetJobString(MSG.GCRequestBatchPlayerResourcesResponse, pending.payload, tostring(pending.job))
    elseif state().by_server[target] ~= nil then
        gc.QueueReplyToPollString(target, MSG.GCRequestBatchPlayerResourcesResponse, pending.payload, tostring(pending.job))
    else
        gc.QueueReplyToString(target, MSG.GCRequestBatchPlayerResourcesResponse, pending.payload, tostring(pending.job))
    end

    pending.sent = true
    state().pending_batch_player_resources = nil
    return true
end

local function snapshot_players(lobby)
    local rows = {}
    for _, member in ipairs(member_order(lobby)) do
        local name = tostring(member.name or ""):gsub("[\t\r\n]", " ")
        rows[#rows + 1] = table.concat({
            tostring(member.steam),
            tostring(member.account or 0),
            name,
            tostring(member.team or 0),
            tostring(member.slot or 0),
            tostring(member.coach or TEAM_NONE),
            tostring(member.hero or 0)
        }, "\t")
    end

    return table.concat(rows, "\n")
end

local function publish_lobby(lobby)
    gc.PublishDotaMatchSnapshot(
        lobby.id,
        lobby.match_id or "0",
        lobby.server_steam or "0",
        lobby.connect or "",
        lobby.state or 0,
        lobby.game_state or 0,
        lobby.game_start_time or 0,
        lobby.dedicated == true,
        snapshot_players(lobby))
end

local function queue_lobby_items_to_server(lobby)
    if gc.QueueLobbyPlayerItemsToServer == nil then
        runtime.Log("items: QueueLobbyPlayerItemsToServer unavailable")
        return false
    end

    local server = tostring(lobby.server_steam or "0")
    if server == "0" then
        return false
    end

    local ok = gc.QueueLobbyPlayerItemsToServer(server, snapshot_players(lobby))
    runtime.Log("items: queue_lobby_items_to_server server=" .. server .. " ok=" .. tostring(ok))
    return ok
end

local function broadcast_lobby(lobby, except_steam, include_server)
    local payload = lobby_multiple_objects(lobby)
    if include_server == true and lobby.server_steam ~= nil and lobby.server_steam ~= "0" and lobby.server_steam ~= except_steam then
        queue_proto(lobby.server_steam, MSG.SOCacheUpdated, lobby_server_multiple_objects(lobby))
    end

    for _, member in ipairs(member_order(lobby)) do
        if member.steam ~= except_steam then
            queue_proto(member.steam, MSG.SOCacheUpdated, payload)
        end
    end
end

local function request_game_server_change(lobby)
    if lobby == nil or lobby.connect == nil or lobby.connect == "" or gc.DotaRequestGameServerChange == nil then
        return
    end

    local server = tostring(lobby.server_steam or "0")
    for _, member in ipairs(member_order(lobby)) do
        if member.steam ~= server then
            local ok = gc.DotaRequestGameServerChange(member.steam, lobby.connect, "")
            runtime.Log("game_server_change_requested lobby=" .. tostring(lobby.id) ..
                " steam=" .. tostring(member.steam) ..
                " connect=" .. tostring(lobby.connect) ..
                " ok=" .. tostring(ok))
        end
    end
end

local function ensure_member(lobby, steam, account, name)
    local member = lobby.members[steam]
    if member ~= nil then
        if name ~= nil and name ~= "" then
            member.name = name
        end
        member.last_seen = PB.now()
        state().by_steam[steam] = lobby.id
        return member
    end

    member = {
        steam = steam,
        account = account or 0,
        name = name ~= nil and name ~= "" and name or ("User" .. tostring(account or 0)),
        team = steam == lobby.leader_steam and TEAM_GOOD or TEAM_POOL,
        slot = steam == lobby.leader_steam and 1 or 0,
        coach = TEAM_NONE,
        hero = 0,
        leaver = LEAVER_DISCONNECTED,
        last_seen = PB.now()
    }

    lobby.members[steam] = member
    lobby.order[#lobby.order + 1] = steam
    state().by_steam[steam] = lobby.id
    return member
end

local function current_lobby()
    local lobby_id = state().by_steam[current_steam()]
    if lobby_id == nil then
        return nil, nil
    end

    local lobby = state().lobbies[lobby_id]
    if lobby == nil then
        state().by_steam[current_steam()] = nil
        return nil, nil
    end

    return lobby, ensure_member(lobby, current_steam(), current_account(), current_name())
end

local function remove_member(lobby, steam)
    lobby.members[steam] = nil
    state().by_steam[steam] = nil
    local next_order = {}
    for _, candidate in ipairs(lobby.order) do
        if candidate ~= steam then
            next_order[#next_order + 1] = candidate
        end
    end
    lobby.order = next_order
end

local function persisted_match_for_current()
    if gc.DotaGetActiveMatchJson == nil then
        return nil
    end

    local json = gc.DotaGetActiveMatchJson()
    if json == nil or json == "" then
        return nil
    end

    local ok, match = pcall(function() return runtime.JsonToTable(json) end)
    if not ok or match == nil then
        runtime.Log("persisted_match parse failed steam=" .. current_steam())
        return nil
    end

    return match
end

local function persisted_match_for_lobby(lobby_id)
    if gc.DotaGetMatchJson == nil or lobby_id == nil or tostring(lobby_id) == "0" then
        return nil
    end

    local json = gc.DotaGetMatchJson(tostring(lobby_id))
    if json == nil or json == "" then
        return nil
    end

    local ok, match = pcall(function() return runtime.JsonToTable(json) end)
    if not ok or match == nil then
        runtime.Log("persisted_match parse failed lobby=" .. tostring(lobby_id))
        return nil
    end

    return match
end

-- Lua is hot-reloadable. After a reload the in-memory table is new but the
-- durable match snapshot still identifies a dedicated process owned by us.
-- Clear that process before a new lobby replaces the old state.
local function release_persisted_match(match, reason)
    if match == nil then
        return false
    end

    local lobby_id = tostring(match.LobbyId or "0")
    if lobby_id == "0" then
        return false
    end

    if match.Dedicated == true and gc.DotaReleaseDedicatedServer ~= nil then
        gc.DotaReleaseDedicatedServer(lobby_id, tostring(reason or "persisted_release"))
    end
    if gc.DotaRemoveMatchSnapshot ~= nil then
        gc.DotaRemoveMatchSnapshot(lobby_id)
    end
    runtime.Log("release_persisted_match lobby=" .. lobby_id .. " dedicated=" .. tostring(match.Dedicated == true) ..
        " reason=" .. tostring(reason or "unknown"))
    return true
end

local function hydrate_persisted_match(match, require_current_member)
    if match == nil then
        return nil
    end

    local lobby_id = tostring(match.LobbyId or "0")
    local updated_at = tonumber(match.UpdatedAtUnix or 0) or 0
    if lobby_id == "0" or updated_at < PB.now() - RECONNECT_TIMEOUT_SECONDS then
        return nil
    end

    local existing = state().lobbies[lobby_id]
    if existing ~= nil then
        return existing
    end

    local players = match.Players or {}
    local first = players[1]
    if first == nil then
        return nil
    end

    local lobby = {
        id = lobby_id,
        leader_steam = tostring(first.SteamId or current_steam()),
        leader_account = tonumber(first.AccountId or 0) or 0,
        version = PB.next_id(),
        state = tonumber(match.State or LOBBY_RUN) or LOBBY_RUN,
        game_mode = 1,
        name = "Recovered match",
        region = 0,
        lan = match.Dedicated ~= true,
        allow_cheats = match.AllowCheats == true,
        dedicated = match.Dedicated == true,
        allow_spectating = false,
        dota_tv_delay = 0,
        visibility = 0,
        game_state = tonumber(match.GameState or GAME_INIT) or GAME_INIT,
        pass_key = "",
        members = {},
        order = {},
        left_members = {},
        server_steam = tostring(match.ServerSteamId or "0"),
        server_port = 27015,
        connect = tostring(match.Connect or ""),
        match_id = tostring(match.MatchId or "0"),
        game_start_time = tonumber(match.GameStartTime or 0) or 0,
        realtime_sent = true,
        last_activity = updated_at,
        seq = bump_seq()
    }

    for _, row in ipairs(players) do
        local steam = tostring(row.SteamId or "0")
        if steam ~= "0" then
            local member = {
                steam = steam,
                account = tonumber(row.AccountId or 0) or 0,
                name = tostring(row.PersonaName or ""),
                team = tonumber(row.Team or TEAM_POOL) or TEAM_POOL,
                slot = tonumber(row.Slot or 0) or 0,
                coach = tonumber(row.CoachTeam or TEAM_NONE) or TEAM_NONE,
                hero = tonumber(row.HeroId or 0) or 0,
                leaver = require_current_member ~= false and steam == current_steam() and LEAVER_DISCONNECTED or LEAVER_NONE,
                disconnected_at = require_current_member ~= false and steam == current_steam() and PB.now() or nil,
                last_seen = PB.now()
            }
            lobby.members[steam] = member
            lobby.order[#lobby.order + 1] = steam
            state().by_steam[steam] = lobby.id
        end
    end

    if require_current_member ~= false and lobby.members[current_steam()] == nil then
        return nil
    end

    state().lobbies[lobby.id] = lobby
    if lobby.server_steam ~= "0" then
        state().by_server[lobby.server_steam] = lobby.id
    end
    runtime.Log("persisted_match hydrated lobby=" .. lobby.id .. " match=" .. lobby.match_id ..
        " steam=" .. current_steam() .. " age=" .. tostring(PB.now() - updated_at))
    return lobby
end

local function delete_lobby_invites_for_group(lobby_id)
    if gc.DotaLobbyInviteDeleteLobby == nil then
        return
    end

    for _, record in ipairs(lobby_invite_records(gc.DotaLobbyInviteDeleteLobby(tostring(lobby_id)))) do
        queue_lobby_invite_removed(record)
    end
end

local function delete_current_lobby_invites()
    if gc.DotaLobbyInviteDeleteForCurrent == nil then
        return
    end

    for _, record in ipairs(lobby_invite_records(gc.DotaLobbyInviteDeleteForCurrent())) do
        queue_lobby_invite_removed(record)
    end
end

local function emit_current_lobby_invites()
    if gc.DotaLobbyInvitesForCurrent == nil then
        return false
    end

    local emitted = false
    for _, record in ipairs(lobby_invite_records(gc.DotaLobbyInvitesForCurrent())) do
        local lobby_id = tostring(record.lobbyId or "0")
        local lobby = state().lobbies[lobby_id]
        if lobby == nil then
            lobby = hydrate_persisted_match(persisted_match_for_lobby(lobby_id), false)
        end

        if lobby == nil or lobby.state ~= LOBBY_UI or member_count(lobby) >= MAX_MEMBERS or
            lobby.members[current_steam()] ~= nil then
            local removed = parse_json_table(gc.DotaLobbyInviteTake(lobby_id))
            if removed ~= nil then
                queue_lobby_invite_removed(removed)
            end
        else
            queue_lobby_invite_record(record)
            emitted = true
        end
    end

    return emitted
end

local function destroy_lobby(lobby, reason)
    if lobby == nil or state().lobbies[lobby.id] == nil then
        return false
    end

    runtime.Log("destroy_lobby id=" .. tostring(lobby.id) .. " match=" .. tostring(lobby.match_id or "0") ..
        " reason=" .. tostring(reason or "unknown"))
    local unsubscribed = lobby_so_cache_unsubscribed(lobby)
    for _, member in ipairs(member_order(lobby)) do
        state().by_steam[member.steam] = nil
        remember_closed_lobby(member.steam, lobby.id)
        queue_proto(member.steam, MSG.SOCacheUnsubscribed, unsubscribed)
    end

    local server = tostring(lobby.server_steam or "0")
    if server ~= "0" then
        -- Queue before dropping by_server so game servers use the poll channel.
        queue_proto(server, MSG.SOCacheUnsubscribed, unsubscribed)
        state().by_server[server] = nil
    end

    if lobby.dedicated == true and gc.DotaReleaseDedicatedServer ~= nil then
        gc.DotaReleaseDedicatedServer(tostring(lobby.id), tostring(reason or "destroy_lobby"))
    end

    delete_lobby_invites_for_group(lobby.id)

    state().lobbies[lobby.id] = nil
    if gc.DotaRemoveMatchSnapshot ~= nil then
        gc.DotaRemoveMatchSnapshot(tostring(lobby.id))
    end
    return true
end

local function leave_current(keep_lobby_id)
    local steam = current_steam()
    local lobby_id = state().by_steam[steam]
    if lobby_id == nil or lobby_id == keep_lobby_id then
        return
    end

    local lobby = state().lobbies[lobby_id]
    if lobby == nil then
        state().by_steam[steam] = nil
        return
    end

    local recipients = {}
    for _, member in ipairs(member_order(lobby)) do
        recipients[#recipients + 1] = member.steam
    end

    local destroy = lobby.leader_steam == steam or member_count(lobby) <= 1
    if destroy then
        destroy_lobby(lobby, "practice_lobby_leave")
        return
    end

    remove_member(lobby, steam)
    refresh_lobby(lobby)
    queue_proto(steam, MSG.SOCacheUnsubscribed, lobby_so_cache_unsubscribed(lobby))
    broadcast_lobby(lobby, steam, false)
    publish_lobby(lobby)
end

local function abandon_current_game()
    local steam = current_steam()
    local lobby_id = state().by_steam[steam]
    local lobby = lobby_id ~= nil and state().lobbies[lobby_id] or nil
    if lobby == nil then
        local persisted = persisted_match_for_current()
        local persisted_lobby_id = persisted ~= nil and tostring(persisted.LobbyId or "0") or "0"
        if persisted_lobby_id ~= "0" then
            gc.Proto(MSG.SOCacheUnsubscribed, PB.bytes(2, owner_soid(3, persisted_lobby_id)))
            release_persisted_match(persisted, "abandon_persisted_match")
            runtime.Log("abandon_current_game cleared persisted lobby=" .. persisted_lobby_id .. " steam=" .. steam)
            return true
        end

        runtime.Log("abandon_current_game ignored without active or persisted lobby steam=" .. steam)
        return true
    end

    if lobby.leader_steam == steam or member_count(lobby) <= 1 then
        destroy_lobby(lobby, "leader_abandoned")
        return true
    end

    local member = lobby.members[steam]
    if member ~= nil then
        lobby.left_members = lobby.left_members or {}
        local left = {}
        for key, value in pairs(member) do
            left[key] = value
        end
        left.leaver = LEAVER_ABANDONED
        left.disconnected_at = PB.now()
        lobby.left_members[#lobby.left_members + 1] = left
    end

    remove_member(lobby, steam)
    refresh_lobby(lobby)
    queue_proto(steam, MSG.SOCacheUnsubscribed, lobby_so_cache_unsubscribed(lobby))
    broadcast_lobby(lobby, steam, true)
    publish_lobby(lobby)
    runtime.Log("abandon_current_game member=" .. steam .. " lobby=" .. tostring(lobby.id) ..
        " remaining=" .. tostring(member_count(lobby)))
    return true
end

local function apply_details(lobby, details)
    if details == nil or details == "" then
        return
    end

    local game_name = runtime.ProtoString(details, 2)
    if game_name ~= nil and game_name ~= "" then
        lobby.name = game_name
    end

    local region = runtime.ProtoVarint(details, 4, 1, 0)
    if region ~= 0 then lobby.region = region end

    local game_mode = runtime.ProtoVarint(details, 5, 1, 0)
    if game_mode ~= 0 then lobby.game_mode = game_mode end

    local pass_key = runtime.ProtoString(details, 15)
    if pass_key ~= nil and pass_key ~= "" then
        lobby.pass_key = pass_key
    end

    lobby.dota_tv_delay = runtime.ProtoVarint(details, 24, 1, lobby.dota_tv_delay or 0)
    local allow_cheats = runtime.ProtoVarint(details, 10, 1, lobby.allow_cheats and 1 or 0)
    lobby.allow_cheats = allow_cheats ~= 0
    local lan = runtime.ProtoVarint(details, 25, 1, lobby.lan and 1 or 0)
    lobby.lan = lan ~= 0
    lobby.custom_game_mode = runtime.ProtoString(details, 26)
    lobby.custom_map = runtime.ProtoString(details, 27)
    lobby.custom_game_id = runtime.ProtoVarint(details, 29, 1, lobby.custom_game_id or 0)
    lobby.custom_min_players = runtime.ProtoVarint(details, 30, 1, lobby.custom_min_players or 0)
    lobby.custom_max_players = runtime.ProtoVarint(details, 31, 1, lobby.custom_max_players or 0)
    lobby.visibility = runtime.ProtoVarint(details, 33, 1, lobby.visibility or 0)
    lobby.custom_game_crc = runtime.ProtoFixed64String(details, 34, 1, tostring(lobby.custom_game_crc or "0"))
    lobby.custom_game_timestamp = runtime.ProtoFixed32(details, 37, 1, lobby.custom_game_timestamp or 0)
end

local function create_lobby()
    emit_pending_closed_lobbies()
    if state().by_steam[current_steam()] == nil then
        release_persisted_match(persisted_match_for_current(), "create_replaces_persisted_match")
    end
    leave_current(nil)

    local id = PB.next_id()
    local lobby = {
        id = id,
        leader_steam = current_steam(),
        leader_account = current_account(),
        version = PB.next_id(),
        state = LOBBY_UI,
        game_mode = 1,
        name = "Room 1",
        region = 0,
        lan = true,
        allow_cheats = false,
        allow_spectating = false,
        dota_tv_delay = 0,
        visibility = 0,
        game_state = GAME_INIT,
        pass_key = "",
        members = {},
        order = {},
        server_steam = "0",
        server_port = 27015,
        connect = "",
        match_id = "0",
        match_outcome = 0,
        game_start_time = 0,
        realtime_sent = false,
        last_activity = PB.now(),
        seq = bump_seq()
    }

    local pass_key = gc.ReadString(5)
    if pass_key ~= nil and pass_key ~= "" then
        lobby.pass_key = pass_key
    end

    apply_details(lobby, gc.ReadBytes(7))
    local member = ensure_member(lobby, current_steam(), current_account(), current_name())
    member.team = TEAM_GOOD
    member.slot = 1
    member.coach = TEAM_NONE

    state().lobbies[id] = lobby
    refresh_lobby(lobby)
    runtime.Log("create_lobby id=" .. tostring(lobby.id) .. " steam=" .. current_steam() ..
        " account=" .. tostring(current_account()) .. " members=" .. tostring(member_count(lobby)))
    gc.Proto(MSG.SOCacheSubscribed, lobby_so_cache_subscribed(lobby))
    gc.Proto(MSG.SOSingleObject, lobby_single_object(lobby))
    publish_lobby(lobby)
    return gc.Reply(MSG.GCPracticeLobbyResponse, PB.result(1))
end

local function join_lobby_state(lobby, pass_key, bypass_password)
    local result = JOIN_INVALID_LOBBY
    if lobby == nil then
        result = JOIN_INVALID_LOBBY
    elseif lobby.state ~= LOBBY_UI then
        result = JOIN_ALREADY_IN_GAME
    elseif bypass_password ~= true and lobby.pass_key ~= nil and lobby.pass_key ~= "" and lobby.pass_key ~= pass_key then
        result = JOIN_BAD_PASSWORD
    elseif member_count(lobby) >= MAX_MEMBERS and lobby.members[current_steam()] == nil then
        result = JOIN_FULL
    else
        leave_current(lobby.id)
        local member = ensure_member(lobby, current_steam(), current_account(), current_name())
        if member.steam ~= lobby.leader_steam then
            member.team = TEAM_POOL
            member.slot = 0
            member.coach = TEAM_NONE
        end

        refresh_lobby(lobby)
        gc.Proto(MSG.SOCacheSubscribed, lobby_so_cache_subscribed(lobby))
        gc.Proto(MSG.SOSingleObject, lobby_single_object(lobby))
        broadcast_lobby(lobby, current_steam(), false)
        publish_lobby(lobby)
        delete_current_lobby_invites()
        result = JOIN_SUCCESS
    end

    return result
end

local function join_lobby()
    local lobby_id = gc.ReadVarintString(1, "0")
    local pass_key = gc.ReadString(3)
    local lobby = state().lobbies[lobby_id]
    if lobby == nil then
        lobby = hydrate_persisted_match(persisted_match_for_lobby(lobby_id), false)
    end

    local result = join_lobby_state(lobby, pass_key, false)
    return gc.Reply(MSG.GCPracticeLobbyJoinResponse, PB.result(result))
end

local function invite_to_lobby()
    local target = gc.ReadFixed64String(1, "0")
    if target == "0" then
        target = gc.ReadVarintString(1, "0")
    end

    local lobby, sender = current_lobby()
    local target_lobby_id = state().by_steam[target]
    local target_lobby = target_lobby_id ~= nil and state().lobbies[target_lobby_id] or nil
    local target_known = gc.DotaUserExists == nil or gc.DotaUserExists(target)
    local invalid = target == "0" or target == current_steam() or not target_known or
        lobby == nil or sender == nil or lobby.state ~= LOBBY_UI or
        lobby.members[target] ~= nil or member_count(lobby) >= MAX_MEMBERS or
        (target_lobby ~= nil and target_lobby.state ~= LOBBY_UI)
    if invalid or gc.DotaLobbyInviteUpsert == nil then
        return gc.Reply(MSG.GCInvitationCreated, invitation_created("0", target, not target_known))
    end

    local members = {}
    for _, member in ipairs(member_order(lobby)) do
        members[#members + 1] = {
            name = member.name or "",
            steam = tostring(member.steam or "0")
        }
    end

    local metadata = {
        senderName = current_name(),
        members = members,
        customGameId = tostring(lobby.custom_game_id or "0"),
        customGameCrc = tostring(lobby.custom_game_crc or "0"),
        customGameTimestamp = lobby.custom_game_timestamp or 0
    }
    local payload_json = runtime.TableToJson(metadata)
    local mutation = parse_json_table(gc.DotaLobbyInviteUpsert(
        tostring(lobby.id), target, current_steam(), payload_json))
    if mutation == nil or mutation.invite == nil then
        return gc.Reply(MSG.GCInvitationCreated, invitation_created("0", target, false))
    end

    if mutation.replaced ~= nil then
        queue_lobby_invite_removed(mutation.replaced)
    end
    queue_lobby_invite_record(mutation.invite)
    runtime.Log("lobby_invite created lobby=" .. tostring(lobby.id) ..
        " sender=" .. current_steam() .. " target=" .. target ..
        " invite=" .. tostring(mutation.invite.inviteGid or "0"))
    local offline = gc.DotaUserOnline ~= nil and not gc.DotaUserOnline(target)
    return gc.Reply(MSG.GCInvitationCreated, invitation_created(lobby.id, target, offline))
end

local function lobby_invite_response()
    local lobby_id = gc.ReadFixed64String(1, "0")
    if lobby_id == "0" then
        lobby_id = gc.ReadVarintString(1, "0")
    end

    local accept = gc.ReadVarint(2, 0) ~= 0
    if lobby_id == "0" or gc.DotaLobbyInviteTake == nil then
        return gc.Reply(MSG.GCPracticeLobbyJoinResponse, PB.result(JOIN_INVALID_LOBBY))
    end

    local record = parse_json_table(gc.DotaLobbyInviteTake(lobby_id))
    if record == nil then
        return gc.Reply(MSG.GCPracticeLobbyJoinResponse, PB.result(JOIN_INVALID_LOBBY))
    end

    queue_lobby_invite_removed(record)
    runtime.Log("lobby_invite response lobby=" .. lobby_id .. " target=" .. current_steam() ..
        " accept=" .. tostring(accept))
    if not accept then
        return true
    end

    local metadata = parse_json_table(record.payloadJson) or {}
    local expected_crc = tostring(metadata.customGameCrc or "0")
    local expected_timestamp = tonumber(metadata.customGameTimestamp or 0) or 0
    local client_crc = gc.ReadFixed64String(6, "0")
    local client_timestamp = gc.ReadFixed32(7, 0)
    if (expected_crc ~= "0" and client_crc ~= "0" and expected_crc ~= client_crc) or
        (expected_timestamp ~= 0 and client_timestamp ~= 0 and expected_timestamp ~= client_timestamp) then
        return gc.Reply(MSG.GCPracticeLobbyJoinResponse, PB.result(JOIN_INVALID_LOBBY))
    end

    local lobby = state().lobbies[lobby_id]
    if lobby == nil then
        lobby = hydrate_persisted_match(persisted_match_for_lobby(lobby_id), false)
    end

    return gc.Reply(MSG.GCPracticeLobbyJoinResponse, PB.result(join_lobby_state(lobby, "", true)))
end

local function set_details()
    local lobby = current_lobby()
    if lobby == nil then
        return gc.Reply(MSG.GCPracticeLobbyResponse, PB.result(0))
    end

    apply_details(lobby, gc.BodyBase64)
    refresh_lobby(lobby)
    broadcast_lobby(lobby, nil, false)
    publish_lobby(lobby)
    return gc.Reply(MSG.GCPracticeLobbyResponse, PB.result(1))
end

local function set_team_slot()
    local lobby, member = current_lobby()
    if lobby == nil then
        return gc.Reply(MSG.GCPracticeLobbyResponse, PB.result(0))
    end

    local next_team = gc.ReadVarint(1, member.team or TEAM_POOL)
    local next_slot = gc.ReadVarint(2, member.slot or 0)
    if next_team ~= TEAM_POOL then
        for _, candidate in ipairs(member_order(lobby)) do
            if candidate.steam ~= member.steam and candidate.team == next_team and candidate.slot == next_slot then
                return gc.Reply(MSG.GCPracticeLobbyResponse, PB.result(0))
            end
        end
    end

    member.team = next_team
    member.slot = next_team == TEAM_POOL and 0 or next_slot
    member.coach = TEAM_NONE
    refresh_lobby(lobby)
    broadcast_lobby(lobby, nil, false)
    publish_lobby(lobby)
    return gc.Reply(MSG.GCPracticeLobbyResponse, PB.result(1))
end

local function set_coach()
    local lobby, member = current_lobby()
    if lobby == nil then
        return gc.Reply(MSG.GCPracticeLobbyResponse, PB.result(0))
    end

    local coach_team = gc.ReadVarint(1, TEAM_NONE)
    if coach_team == TEAM_NONE then
        member.coach = TEAM_NONE
        member.team = TEAM_POOL
        member.slot = 0
    else
        member.coach = coach_team
        member.team = TEAM_SPECTATOR
        member.slot = 1
    end

    refresh_lobby(lobby)
    broadcast_lobby(lobby, nil, false)
    publish_lobby(lobby)
    return gc.Reply(MSG.GCPracticeLobbyResponse, PB.result(1))
end

local function apply_team()
    local lobby = current_lobby()
    if lobby ~= nil then
        refresh_lobby(lobby)
        broadcast_lobby(lobby, "0", true)
        publish_lobby(lobby)
    end

    return gc.Reply(MSG.GCGenericResult, PB.result(1))
end

local function ensure_match_id(lobby)
    if lobby.match_id == nil or lobby.match_id == "0" then
        local seed = PB.now() * 5
        if (state().match_seq or 0) < seed then
            state().match_seq = seed
        end
        state().match_seq = state().match_seq + 1
        lobby.match_id = tostring(state().match_seq)
        runtime.Log("assigned_match_id lobby=" .. tostring(lobby.id) .. " match_id=" .. tostring(lobby.match_id))
    end

    return lobby.match_id
end

local function launch_lobby()
    local lobby = current_lobby()
    if lobby ~= nil then
        -- The captured Steam flow publishes match_id in SERVERSETUP, before a
        -- game server is attached. Adding match_id only with RUN can make Dota
        -- discard that transition and remain in INIT on subsequent lobbies.
        ensure_match_id(lobby)
        -- The current client marks a local lobby with lan=true. It owns the
        -- listen-server process, so its later LANServerAvailable message is the
        -- only valid trigger for attaching that server. Non-local lobbies use a
        -- supervisor reservation and stay SERVERASSIGN until the dedicated
        -- process reports GCGameServerInfo from the reserved game port.
        -- Match_Helper.LaunchWithDedicated in the reference coordinator uses a
        -- non-LAN lobby with an explicit server_region. Region zero is the
        -- local/listen-server route even if an older client omits the LAN bit.
        lobby.dedicated = lobby.lan ~= true and (lobby.region or 0) ~= 0
        if lobby.dedicated == true then
            local port_text = gc.DotaStartDedicatedServer ~= nil and
                gc.DotaStartDedicatedServer(tostring(lobby.id), tostring(lobby.custom_map or "dota")) or "0"
            local dedicated_port = tonumber(port_text or "0") or 0
            if dedicated_port == 0 then
                lobby.dedicated = false
                lobby.state = LOBBY_UI
                lobby.connect = ""
                lobby.server_steam = "0"
                lobby.game_start_time = 0
                lobby.game_state = GAME_INIT
                lobby.realtime_sent = false
                refresh_lobby(lobby)
                broadcast_lobby(lobby, nil, false)
                publish_lobby(lobby)
                gc.Proto(MSG.GCToClientBroadcastNotification,
                    PB.str(1, "Could not start the dedicated server for the lobby."))
                runtime.Log("launch_lobby dedicated failed id=" .. tostring(lobby.id))
                return true
            end

            lobby.state = LOBBY_SERVER_ASSIGN
            lobby.server_port = dedicated_port
            runtime.Log("launch_lobby dedicated id=" .. tostring(lobby.id) .. " port=" .. tostring(dedicated_port) ..
                " members=" .. tostring(member_count(lobby)))
        else
            lobby.state = LOBBY_SERVER_SETUP
            runtime.Log("launch_lobby local id=" .. tostring(lobby.id) .. " members=" .. tostring(member_count(lobby)))
        end
        lobby.connect = ""
        lobby.server_steam = "0"
        lobby.game_start_time = 0
        lobby.game_state = GAME_INIT
        lobby.realtime_sent = false
        refresh_lobby(lobby)
        broadcast_lobby(lobby, nil, false)
        publish_lobby(lobby)
    end

    return true
end

local function waiting_lobby()
    local selected = nil
    for _, lobby in pairs(state().lobbies) do
        if lobby.state == LOBBY_SERVER_SETUP and lobby.dedicated ~= true and
            (selected == nil or (lobby.seq or 0) > (selected.seq or 0)) then
            selected = lobby
        end
    end

    return selected
end

local function connect_string(lobby)
    local port = lobby.server_port ~= nil and lobby.server_port ~= 0 and lobby.server_port or 27015

    -- The Dota connect field holds two space-separated endpoints; the client tries
    -- both and connects over whichever it can route to. When the host is multi-homed
    -- (WiFi + ZeroTier) advertise one address per interface so every peer reaches it.
    local endpoints = {}
    local seen = {}
    local function add_endpoint(ip)
        if ip == nil or ip == "" or ip == "::1" or ip == "0.0.0.0" then
            return
        end

        local endpoint = ip .. ":" .. tostring(port)
        if seen[endpoint] == true then
            return
        end

        seen[endpoint] = true
        endpoints[#endpoints + 1] = endpoint
    end

    local ips = lobby.server_ips
    if ips ~= nil and ips ~= "" then
        for ip in string.gmatch(ips, "%S+") do
            add_endpoint(ip)
        end
    end

    if #endpoints == 0 then
        -- Fall back to the single captured address, then loopback (host == server).
        local ip = lobby.server_ip
        if not is_routable_ip(ip) then
            ip = "127.0.0.1"
        end
        endpoints[1] = ip .. ":" .. tostring(port)
    end

    local first = endpoints[1]
    local second = endpoints[2] or first
    return first .. " " .. second
end

local function mark_lobby_members_connected(lobby)
    for _, member in ipairs(member_order(lobby)) do
        member.leaver = LEAVER_NONE
        member.disconnected_at = nil
        member.disconnect_reason = nil
        member.connected_once = false
    end
end

local function lobby_has_connected_player(lobby)
    for _, member in ipairs(member_order(lobby)) do
        if member.connected_once == true then
            return true
        end
    end

    return false
end

local function start_realtime_stats(lobby)
    if lobby.server_steam == nil or lobby.server_steam == "0" or lobby.realtime_sent == true then
        return
    end

    queue_proto(lobby.server_steam, MSG.GCToServerRealtimeStatsStartStop, PB.v(1, 1))
    lobby.realtime_sent = true
end

local function same_running_server(lobby)
    return lobby ~= nil and lobby.state == LOBBY_RUN and tostring(lobby.server_steam or "0") == current_steam()
end

local function rebroadcast_running_lobby_to_members(lobby, reason)
    if lobby == nil then
        return true
    end

    refresh_lobby(lobby)
    broadcast_lobby(lobby, tostring(lobby.server_steam or "0"), false)
    publish_lobby(lobby)
    runtime.Log(tostring(reason or "running_lobby") .. " rebroadcast RUN lobby=" .. tostring(lobby.id) ..
        " server=" .. tostring(lobby.server_steam or "0") ..
        " connect=" .. tostring(lobby.connect or ""))
    return true
end

local function attach_server(lobby, mark_run)
    if lobby == nil then
        runtime.Log("attach_server: no lobby found for server " .. current_steam() .. " (mark_run=" .. tostring(mark_run) .. ")")
        return false
    end

    local first_attach = tostring(lobby.server_steam or "0") ~= current_steam()

    runtime.Log("attach_server: lobby " .. tostring(lobby.id) .. " server=" .. current_steam() ..
        " members=" .. tostring(member_count(lobby)) .. " mark_run=" .. tostring(mark_run) ..
        " first_attach=" .. tostring(first_attach))

    local old_server = lobby.server_steam
    if old_server ~= nil and old_server ~= "0" then
        state().by_server[old_server] = nil
    end

    lobby.server_steam = current_steam()
    state().by_server[lobby.server_steam] = lobby.id

    local port = gc.ReadVarint(3, 0)
    if port ~= 0 then
        lobby.server_port = port
    elseif lobby.server_port == nil or lobby.server_port == 0 then
        lobby.server_port = 27015
    end

    -- Remember the launcher's reachable address so the match connect string
    -- points other players at the real host, not 127.0.0.1. Dedicated/listen
    -- servers running beside SKYNET reach the web API over loopback, so use the
    -- IPs from CMsgGameServerInfo before falling back to ClientIp.
    local public_ip = gc.ReadVarintString(1, "0")
    local private_ip = gc.ReadVarintString(2, "0")
    local previous_ip = tostring(lobby.server_ip or "")
    local server_ip = current_ip()
    if gc.DotaResolveGameServerConnectIp ~= nil then
        server_ip = gc.DotaResolveGameServerConnectIp(public_ip, private_ip, previous_ip)
    end
    if server_ip ~= nil and server_ip ~= "" then
        lobby.server_ip = server_ip
    end
    -- Collect up to two reachable host addresses (WiFi + ZeroTier). The connect
    -- field carries two endpoints, so each peer gets one it can route to.
    if gc.DotaGetHostConnectIps ~= nil then
        local ips = gc.DotaGetHostConnectIps(public_ip, private_ip, previous_ip)
        if ips ~= nil and ips ~= "" then
            lobby.server_ips = ips
        end
    end
    runtime.Log("attach_server endpoint lobby=" .. tostring(lobby.id) ..
        " ip=" .. tostring(lobby.server_ip or "") ..
        " ips=" .. tostring(lobby.server_ips or "") ..
        " port=" .. tostring(lobby.server_port or 0) ..
        " public=" .. tostring(public_ip) ..
        " private=" .. tostring(private_ip) ..
        " client_ip=" .. current_ip())

    if mark_run == true then
        ensure_match_id(lobby)
        lobby.state = LOBBY_RUN
        lobby.connect = connect_string(lobby)
        runtime.Log("attach_server RUN lobby=" .. tostring(lobby.id) .. " connect=" .. tostring(lobby.connect or ""))
        lobby.game_state = GAME_INIT
        lobby.server_setup_ready = false
        lobby.lan_after_setup_count = 0
        mark_lobby_members_connected(lobby)
        if lobby.game_start_time == nil or lobby.game_start_time == 0 then
            lobby.game_start_time = PB.now()
        end
    elseif lobby.state ~= LOBBY_RUN then
        if lobby.dedicated == true then
            -- Match the captured coordinator ordering: GameServerInfo attaches
            -- the reserved dedicated server in SERVERSETUP, and the server's
            -- LobbyInitialized edge promotes the lobby to RUN. Publishing RUN
            -- directly from 4508 leaves current clients with an active match UI
            -- but no actual NetChan dial to the dedicated process.
            ensure_match_id(lobby)
            lobby.state = LOBBY_SERVER_SETUP
            lobby.connect = ""
            runtime.Log("attach_server dedicated SETUP lobby=" .. tostring(lobby.id) ..
                " server=" .. tostring(lobby.server_steam or "0"))
            lobby.game_state = GAME_INIT
            lobby.server_setup_ready = true
            lobby.lan_after_setup_count = 0
            if lobby.game_start_time == nil or lobby.game_start_time == 0 then
                lobby.game_start_time = PB.now()
            end
        else
            lobby.state = LOBBY_SERVER_SETUP
            lobby.server_setup_ready = true
            lobby.lan_after_setup_count = 0
        end
    end

    if lobby.dedicated == true then
        -- Dedicated Dota servers have a two-stage contract.  The old, working
        -- coordinator sends player econ caches and the SERVERSETUP lobby cache
        -- on GameServerInfo (4508).  ServerAvailable (4506) then carries only
        -- the RUN update.  Sending the caches a second time after 4506 makes a
        -- current dedicated process tear down during its map transition.
        if mark_run == true then
            refresh_lobby(lobby)
            queue_proto(lobby.server_steam, MSG.SOCacheUpdated, lobby_server_multiple_objects(lobby))
            emit_pending_batch_player_resources(lobby)
            broadcast_lobby(lobby, tostring(lobby.server_steam or "0"), false)
            request_game_server_change(lobby)
        else
            refresh_lobby(lobby)
            queue_lobby_items_to_server(lobby)
            queue_proto(lobby.server_steam, MSG.SOCacheSubscribed, lobby_server_so_cache_subscribed(lobby))
            -- Current Dota builds do not transition a dedicated process out of
            -- INIT from CacheSubscribed alone.  The legacy coordinator predates
            -- that requirement, while the current-client capture shows that one
            -- SERVERSETUP UpdateMultiple after the cache is the activation edge
            -- which makes the server emit 4506.  This is deliberately before
            -- RUN, and is not a duplicate cache delivery.
            queue_proto(lobby.server_steam, MSG.SOCacheUpdated, lobby_server_multiple_objects(lobby))
            lobby.server_setup_update_attempts = 1
            lobby.server_setup_update_at = PB.now()
            -- The dedicated already owns the cache above.  Only lobby members
            -- need this SERVERSETUP transition, exactly as the legacy GC did.
            broadcast_lobby(lobby, "0", false)
        end
    elseif mark_run == true then
        refresh_lobby(lobby)
        queue_proto(lobby.server_steam, MSG.SOCacheUpdated, lobby_server_multiple_objects(lobby))

        -- Listen-server ordering is independently validated by the local
        -- hero-selection and cosmetic path.  Do not fold it into the dedicated
        -- path above: the two server types emit different GC transitions.
        emit_pending_batch_player_resources(lobby)
        queue_proto(lobby.server_steam, MSG.SOCacheSubscribed, lobby_server_so_cache_subscribed(lobby))
        queue_lobby_items_to_server(lobby)
    else
        refresh_lobby(lobby)
        queue_proto(lobby.server_steam, MSG.SOCacheSubscribed, lobby_server_so_cache_subscribed(lobby))
        broadcast_lobby(lobby, "0", true)
    end

    publish_lobby(lobby)
    return true
end

local function game_server_hello()
    local version = gc.ReadVarint(1, 20)
    -- CMsgClientWelcome must establish the dedicated server's own service-0
    -- cache.  A partial cache (only version/service_id) happens to pass the
    -- first handshake but leaves the server with no owner subscription when it
    -- changes level after ServerAvailable, which crashes current Dota builds.
    local cache_version = PB.next_id()
    local server_cache = PB.cat(
        PB.f64s(3, cache_version),
        PB.bytes(4, owner_soid(1, current_steam())),
        PB.v(5, 0),
        PB.v(6, 1),
        PB.f64s(7, cache_version)
    )
    local welcome = PB.cat(
        PB.v(1, version ~= 0 and version or 20),
        PB.bytes(3, server_cache),
        PB.v(9, 20)
    )

    gc.Proto(MSG.GCGameServerWelcome, welcome)
    return true
end

local function game_server_info()
    local lobby = nil
    local reported_port = gc.ReadVarint(3, 0)
    local assigned_id = state().by_server[current_steam()]
    if assigned_id ~= nil then
        lobby = state().lobbies[assigned_id]
    end

    if lobby == nil and gc.DotaClaimDedicatedServer ~= nil then
        local claimed_id = gc.DotaClaimDedicatedServer(current_steam(), reported_port)
        if claimed_id ~= nil and claimed_id ~= "" and claimed_id ~= "0" then
            lobby = state().lobbies[tostring(claimed_id)]
            if lobby ~= nil then
                runtime.Log("game_server_info claimed dedicated lobby=" .. tostring(lobby.id) ..
                    " server=" .. current_steam() .. " port=" .. tostring(reported_port))
            end
        end
    end

    if lobby == nil and gc.DotaDedicatedServerPortReserved ~= nil and gc.DotaDedicatedServerPortReserved(reported_port) then
        runtime.Log("game_server_info rejected reserved dedicated server=" .. current_steam() ..
            " port=" .. tostring(reported_port) .. " because its lobby claim failed")
        return true
    end

    if lobby == nil then
        lobby = waiting_lobby()
    end

    if same_running_server(lobby) then
        runtime.Log("game_server_info ignored duplicate for lobby=" .. tostring(lobby.id) .. " server=" .. current_steam())
        return true
    end

    -- GameServerInfo binds the reserved dedicated process to this lobby. The
    -- connect-bearing RUN cache is sent only after the server confirms setup.
    return attach_server(lobby, false) or true
end

local function lobby_initialized()
    local lobby = nil
    local assigned_id = state().by_server[current_steam()]
    if assigned_id ~= nil then
        lobby = state().lobbies[assigned_id]
    end

    if lobby == nil then
        lobby = waiting_lobby()
    end

    if same_running_server(lobby) then
        runtime.Log("lobby_initialized ignored duplicate for lobby=" .. tostring(lobby.id) .. " server=" .. current_steam())
        if lobby.dedicated == true then
            return rebroadcast_running_lobby_to_members(lobby, "lobby_initialized")
        end
        return true
    end

    return attach_server(lobby, true) or true
end

local function lan_server_available()
    local lobby_id = gc.ReadFixed64String(1, "0")
    if lobby_id == "0" then
        lobby_id = gc.ReadVarintString(1, "0")
    end

    local lobby = state().lobbies[lobby_id]
    local recovered = false
    if lobby == nil then
        lobby = hydrate_persisted_match(persisted_match_for_lobby(lobby_id), false)
        recovered = lobby ~= nil
    end
    if lobby ~= nil then
        if lobby.dedicated == true then
            -- A dedicated server must claim its supervisor reservation through
            -- GCGameServerInfo. LANServerAvailable is only authoritative for a
            -- listen server started by the lobby owner.
            runtime.Log("lan_server_available ignored for dedicated lobby=" .. tostring(lobby.id) ..
                " server=" .. current_steam())
            return true
        end
        if same_running_server(lobby) then
            runtime.Log((recovered and "lan_server_available recovered lobby=" or "lan_server_available ignored duplicate for lobby=") ..
                tostring(lobby.id) .. " server=" .. current_steam())
            return true
        end

        local old_server = lobby.server_steam
        if old_server ~= nil and old_server ~= "0" and old_server ~= current_steam() then
            state().by_server[old_server] = nil
        end

        lobby.server_steam = current_steam()
        state().by_server[lobby.server_steam] = lobby.id
        runtime.Log((recovered and "lan_server_available recovered lobby=" or "lan_server_available reserved lobby=") ..
            tostring(lobby.id) .. " server=" .. current_steam())

        -- A reused listen-server process does not always emit 4508 for the next
        -- lobby. Initialize its SERVERSETUP cache from 4511 in that case so the
        -- later RUN update cannot arrive before the owner is subscribed.
        if not recovered and lobby.state == LOBBY_SERVER_SETUP and lobby.server_setup_ready ~= true then
            runtime.Log("lan_server_available initializing setup cache lobby=" .. tostring(lobby.id) ..
                " server=" .. current_steam())
            return attach_server(lobby, false) or true
        end

        if recovered then
            refresh_lobby(lobby)
            broadcast_lobby(lobby, nil, true)
            publish_lobby(lobby)
        end

        if lobby.state == LOBBY_SERVER_SETUP and lobby.server_setup_ready == true then
            lobby.lan_after_setup_count = (lobby.lan_after_setup_count or 0) + 1
            runtime.Log("lan_server_available setup heartbeat lobby=" .. tostring(lobby.id) ..
                " count=" .. tostring(lobby.lan_after_setup_count))
        end
    end

    return true
end

local function server_available()
    local lobby = nil
    local assigned_id = state().by_server[current_steam()]
    if assigned_id ~= nil then
        lobby = state().lobbies[assigned_id]
    end

    if lobby == nil then
        lobby = waiting_lobby()
    end

    if same_running_server(lobby) then
        runtime.Log("server_available ignored duplicate for lobby=" .. tostring(lobby.id) .. " server=" .. current_steam())
        if lobby.dedicated == true then
            return rebroadcast_running_lobby_to_members(lobby, "server_available")
        end
        return true
    end

    return attach_server(lobby, true) or true
end

local function connected_players()
    local lobby_id = state().by_server[current_steam()]
    local lobby = lobby_id ~= nil and state().lobbies[lobby_id] or nil
    if lobby == nil then
        return true
    end

    local send_reason = gc.ReadVarint(8, 0)
    local logged_state = gc.ReadVarint(2, 0)
    local count = gc.FieldCount(1)
    local disconnected_count = gc.FieldCount(7)
    local changed = false
    runtime.Log("connected_players lobby=" .. tostring(lobby.id) .. " reason=" .. tostring(send_reason) ..
        " state=" .. tostring(logged_state) .. " connected=" .. tostring(count) ..
        " disconnected=" .. tostring(disconnected_count) .. " server=" .. current_steam())
    if send_reason == CONNECTED_REASON_GAME_STATE then
        changed = (lobby.game_state or GAME_INIT) ~= logged_state or changed
        lobby.game_state = logged_state
    elseif logged_state ~= GAME_INIT then
        changed = (lobby.game_state or GAME_INIT) ~= logged_state or changed
        lobby.game_state = logged_state
    end
    -- A no-op player heartbeat still proves that the match is active, but it
    -- must not bump the SO version or rewrite the persistent snapshot.
    lobby.last_activity = PB.now()

    -- A player stays in all_members while disconnected. The game server sends
    -- the same SteamID in connected_players again after a successful reconnect.
    for i = 1, count do
        local player = gc.ReadBytesAt(1, i)
        local steam = runtime.ProtoFixed64String(player, 1, 1, "0")
        if steam == "0" then
            steam = runtime.ProtoVarintString(player, 1, 1, "0")
        end

        local member = lobby.members[steam]
        if member ~= nil then
            member.connected_once = true
            local hero = runtime.ProtoVarint(player, 2, 1, 0)
            if hero ~= 0 and hero ~= (member.hero or 0) then
                member.hero = hero
                changed = true
            end

            local previous_leaver = member.leaver or LEAVER_NONE
            local previous_disconnected_at = member.disconnected_at
            local previous_disconnect_reason = member.disconnect_reason
            local leaver_state = runtime.ProtoBytes(player, 3, 1)
            local next_leaver = LEAVER_NONE
            if leaver_state == nil or leaver_state == "" then
                next_leaver = LEAVER_NONE
            else
                next_leaver = runtime.ProtoVarint(leaver_state, 1, 1, previous_leaver)
            end
            member.leaver = next_leaver
            if next_leaver == LEAVER_NONE then
                member.disconnected_at = nil
                member.disconnect_reason = nil
            end
            member.last_seen = PB.now()
            if previous_leaver ~= next_leaver or
                (next_leaver == LEAVER_NONE and
                    (previous_disconnected_at ~= nil or previous_disconnect_reason ~= nil)) then
                changed = true
                if next_leaver == LEAVER_NONE then
                    runtime.Log("player_reconnected lobby=" .. tostring(lobby.id) .. " steam=" .. steam ..
                        " reason=" .. tostring(send_reason))
                end
            end
        end
    end

    for i = 1, disconnected_count do
        local player = gc.ReadBytesAt(7, i)
        local steam = runtime.ProtoFixed64String(player, 1, 1, "0")
        if steam == "0" then
            steam = runtime.ProtoVarintString(player, 1, 1, "0")
        end

        local member = lobby.members[steam]
        if member ~= nil then
            if member.connected_once ~= true then
                runtime.Log("player_disconnect_ignored_before_join lobby=" .. tostring(lobby.id) ..
                    " steam=" .. steam .. " send_reason=" .. tostring(send_reason) ..
                    " game_state=" .. tostring(logged_state))
                member.last_seen = PB.now()
            else
                local previous_leaver = member.leaver or LEAVER_NONE
                local previous_reason = member.disconnect_reason
                local first_disconnect = member.disconnected_at == nil
                local leaver_state = runtime.ProtoBytes(player, 3, 1)
                local leaver = LEAVER_DISCONNECTED
                if leaver_state ~= nil and leaver_state ~= "" then
                    leaver = runtime.ProtoVarint(leaver_state, 1, 1, LEAVER_DISCONNECTED)
                    if leaver == LEAVER_NONE then
                        leaver = LEAVER_DISCONNECTED
                    end
                end

                local disconnect_reason = runtime.ProtoVarint(player, 4, 1, 0)
                member.leaver = leaver
                member.disconnect_reason = disconnect_reason
                -- Preserve the first disconnect instant. Repeated server reports
                -- must not postpone the ten-minute reconnect timeout forever.
                if first_disconnect then
                    member.disconnected_at = PB.now()
                end
                member.last_seen = PB.now()
                if first_disconnect or previous_leaver ~= leaver or previous_reason ~= disconnect_reason then
                    changed = true
                    runtime.Log("player_disconnected lobby=" .. tostring(lobby.id) .. " steam=" .. steam ..
                        " leaver=" .. tostring(leaver) .. " network_reason=" .. tostring(member.disconnect_reason) ..
                        " send_reason=" .. tostring(send_reason))
                end
            end
        end
    end

    -- A reused listen-server process can report HEARTBEAT/INIT instead of
    -- GAME_STATE/INIT. Both are valid synchronization points once 4508 has
    -- attached the SERVERSETUP lobby; waiting for a player entry deadlocks
    -- 7450 and leaves the game in INIT.
    if lobby.state == LOBBY_SERVER_SETUP and lobby.server_setup_ready == true and
        (count > 0 or (logged_state == GAME_INIT and
            (send_reason == CONNECTED_REASON_HEARTBEAT or send_reason == CONNECTED_REASON_GAME_STATE))) then
        runtime.Log("connected_players advancing server setup lobby=" .. tostring(lobby.id) ..
            " reason=" .. tostring(send_reason) .. " state=" .. tostring(logged_state) ..
            " count=" .. tostring(count))
        return attach_server(lobby, true) or true
    end

    if lobby.game_state >= GAME_WAIT_FOR_PLAYERS then
        start_realtime_stats(lobby)
    end

    if not changed then
        return true
    end

    refresh_lobby(lobby)
    broadcast_lobby(lobby, nil, true)
    publish_lobby(lobby)
    return true
end

local function player_failed_to_connect()
    local lobby_id = state().by_server[current_steam()]
    local lobby = lobby_id ~= nil and state().lobbies[lobby_id] or nil
    if lobby == nil then
        return true
    end

    -- CMsgDOTAPlayerFailedToConnect is a per-player, retryable report.  It is
    -- not a match teardown signal: clearing the lobby here removed the
    -- dedicated server precisely when a remote client needed to retry.  Keep
    -- the RUN lobby and its reservation intact, record only the failed or
    -- abandoned members, and let ConnectedPlayers clear this state on a later
    -- successful reconnect.
    local changed = false
    local failed = 0
    local abandoned = 0
    local function mark_failed_members(field, is_abandoned)
        local count = gc.FieldCount(field)
        for index = 1, count do
            local steam = gc.ReadFixed64AtString(field, index, "0")
            local member = steam ~= "0" and lobby.members[steam] or nil
            if member ~= nil then
                if member.connected_once ~= true and not is_abandoned then
                    runtime.Log("player_failed_to_connect_ignored_before_join lobby=" .. tostring(lobby.id) ..
                        " steam=" .. steam)
                else
                    local next_leaver = is_abandoned and LEAVER_ABANDONED or LEAVER_DISCONNECTED
                    if member.leaver ~= next_leaver or member.disconnected_at == nil then
                        changed = true
                    end
                    member.leaver = next_leaver
                    member.disconnect_reason = 0
                    member.disconnected_at = member.disconnected_at or PB.now()
                    member.last_seen = PB.now()
                    if is_abandoned then
                        abandoned = abandoned + 1
                    else
                        failed = failed + 1
                    end
                end
            else
                runtime.Log("player_failed_to_connect unknown member=" .. tostring(steam) ..
                    " lobby=" .. tostring(lobby.id))
            end
        end
    end

    -- CMsgDOTAPlayerFailedToConnect:
    --   1 = failed_loaders (fixed64), 2 = abandoned_loaders (fixed64).
    mark_failed_members(1, false)
    mark_failed_members(2, true)

    if changed then
        refresh_lobby(lobby)
        broadcast_lobby(lobby, nil, true)
        publish_lobby(lobby)
    end
    runtime.Log("player_failed_to_connect preserved lobby=" .. tostring(lobby.id) ..
        " server=" .. tostring(lobby.server_steam or "0") ..
        " failed=" .. tostring(failed) .. " abandoned=" .. tostring(abandoned))
    return true
end

local function sign_out()
    local lobby_id = state().by_server[current_steam()]
    local lobby = lobby_id ~= nil and state().lobbies[lobby_id] or nil
    local match_id = "0"
    local recipients = {}
    local notify_clients = true
    if lobby ~= nil then
        match_id = ensure_match_id(lobby)
        for _, member in ipairs(member_order(lobby)) do
            recipients[#recipients + 1] = member.steam
        end
        if lobby.dedicated == true and not lobby_has_connected_player(lobby) then
            notify_clients = false
            runtime.Log("match_sign_out_suppressed_before_join lobby=" .. tostring(lobby.id) ..
                " match=" .. tostring(match_id) .. " server=" .. current_steam())
        end
    end

    local game_mode = lobby ~= nil and (lobby.game_mode or 1) or 1
    local game_start_time = lobby ~= nil and (lobby.game_start_time or 0) or 0
    local server_steam = lobby ~= nil and (lobby.server_steam or current_steam()) or current_steam()
    local response = gc.DotaStatsGameMatchSignOutResponse(tostring(match_id), game_mode, 1, game_start_time, tostring(server_steam))
    if lobby ~= nil and lobby.state == LOBBY_RUN then
        local good_guys_win = gc.ReadVarint(3, 0) ~= 0
        lobby.state = LOBBY_POSTGAME
        lobby.game_state = GAME_INIT
        lobby.match_outcome = good_guys_win and 2 or 3
        lobby.signed_out = true
        lobby.signed_out_at = PB.now()
        lobby.last_activity = PB.now()
        refresh_lobby(lobby)
        broadcast_lobby(lobby, nil, true)
        publish_lobby(lobby)
        runtime.Log("match_sign_out_postgame lobby=" .. tostring(lobby.id) ..
            " match=" .. tostring(match_id) ..
            " outcome=" .. tostring(lobby.match_outcome))
    elseif lobby ~= nil then
        refresh_lobby(lobby)
        broadcast_lobby(lobby, nil, true)
        publish_lobby(lobby)
    end

    gc.Reply(MSG.GCGameMatchSignOutResponse, response)
    if match_id ~= "0" and notify_clients == true then
        local signed_out = PB.vs(1, match_id)
        for _, recipient in ipairs(recipients) do
            queue_proto(recipient, MSG.GCToClientMatchSignedOut, signed_out)
        end
    end

    return true
end

local function request_batch_player_resources()
    local payload, count = batch_player_resources_payload()
    local job = tostring(gc.SourceJobId or "18446744073709551615")

    -- Do not turn this into an immediate MessageWithTargetJobString response.
    -- Dota requests 7450 before the local game server has consumed the RUN
    -- lobby. attach_server(..., true) releases this exact job as 7451 once that
    -- state transition is visible, matching the captured and validated flow.
    state().pending_batch_player_resources = {
        steam = current_steam(),
        payload = payload,
        job = job,
        sent = false
    }

    runtime.Log("batch_player_resources deferred accounts=" .. tostring(count) ..
        " steam=" .. current_steam() .. " job=" .. tostring(gc.SourceJobId))

    local lobby_id = state().by_server[current_steam()]
    local lobby = lobby_id ~= nil and state().lobbies[lobby_id] or nil
    if lobby ~= nil and lobby.state == LOBBY_RUN then
        emit_pending_batch_player_resources(lobby)
    end

    return true
end

local function recent_accomplishments()
    return gc.Reply(MSG.ServerToGCRequestPlayerRecentAccomplishmentsResponse, gc.DotaStatsServerRecentAccomplishmentsResponse())
end

-- CMsgPracticeLobbyListResponseEntry: the shared entry used by the practice,
-- friend-practice and generic lobby list responses. It lets other players
-- discover an open lobby (id + members) so they can join it.
local function lobby_list_entry(lobby)
    local parts = {
        PB.vs(1, lobby.id),
        PB.v(6, (lobby.pass_key ~= nil and lobby.pass_key ~= "") and 1 or 0),
        PB.v(7, lobby.leader_account or 0),
        PB.str(10, lobby.name or "Room"),
        PB.v(12, lobby.game_mode or 1),
        PB.v(13, 1),                       -- friend_present: surface it on the LAN
        PB.v(14, member_count(lobby)),     -- players
        PB.v(16, MAX_MEMBERS),             -- max_player_count
        PB.v(17, lobby.region or 0)        -- server_region
    }

    for _, member in ipairs(member_order(lobby)) do
        local entry = PB.cat(PB.v(1, member.account or 0), PB.str(2, member.name or ""))
        parts[#parts + 1] = PB.bytes(5, entry)
    end

    return PB.cat(table.unpack(parts))
end

-- Builds the repeated "lobbies" field for a list response. Only open (UI-state)
-- lobbies with members are joinable, so those are the ones advertised. The
-- practice-list response numbers the field 2, the others use 1.
local function lobby_list_payload(lobbies_field)
    local parts = {}
    for _, lobby in pairs(state().lobbies) do
        if lobby.state == LOBBY_UI and member_count(lobby) > 0 then
            parts[#parts + 1] = PB.bytes(lobbies_field, lobby_list_entry(lobby))
        end
    end

    if #parts == 0 then
        return ""
    end

    return PB.cat(table.unpack(parts))
end

local function friend_practice_lobby_list()
    return gc.Reply(MSG.GCFriendPracticeLobbyListResponse, lobby_list_payload(1))
end

local function lobby_list()
    return gc.Reply(MSG.GCLobbyListResponse, lobby_list_payload(1))
end

local function practice_lobby_list()
    return gc.Reply(MSG.GCPracticeLobbyListResponse, lobby_list_payload(2))
end

function dota_lobby_handle(message_type)
    local handlers = {
        [MSG.GCPracticeLobbyCreate] = create_lobby,
        [MSG.GCAbandonCurrentGame] = abandon_current_game,
        [MSG.GCPracticeLobbyLeave] = function() leave_current(nil) return true end,
        [MSG.GCPracticeLobbyJoin] = join_lobby,
        [MSG.GCInviteToLobby] = invite_to_lobby,
        [MSG.GCLobbyInviteResponse] = lobby_invite_response,
        [MSG.GCPracticeLobbyList] = practice_lobby_list,
        [MSG.GCFriendPracticeLobbyListRequest] = friend_practice_lobby_list,
        [MSG.GCLobbyList] = lobby_list,
        [MSG.GCPracticeLobbySetDetails] = set_details,
        [MSG.GCPracticeLobbySetTeamSlot] = set_team_slot,
        [MSG.GCPracticeLobbySetCoach] = set_coach,
        [MSG.GCApplyTeamToPracticeLobby] = apply_team,
        [MSG.GCPracticeLobbyLaunch] = launch_lobby,
        [MSG.GCGameServerHello] = game_server_hello,
        [MSG.GCGameServerInfo] = game_server_info,
        [MSG.GCLANServerAvailable] = lan_server_available,
        [MSG.GCServerAvailable] = server_available,
        [MSG.GCConnectedPlayers] = connected_players,
        [MSG.GCPlayerFailedToConnect] = player_failed_to_connect,
        [MSG.GCGameMatchSignOut] = sign_out,
        [MSG.GCGameMatchSignOut2] = sign_out,
        [MSG.GCRequestBatchPlayerResources] = request_batch_player_resources,
        [MSG.ServerToGCRequestPlayerRecentAccomplishments] = recent_accomplishments,
        [MSG.ServerToGCLobbyInitialized] = lobby_initialized
    }

    local handler = handlers[message_type]
    if handler == nil then
        return false
    end

    return handler()
end

function dota_lobby_on_client_hello()
    local emitted_closed_lobby = emit_pending_closed_lobbies()
    local emitted_invite = emit_current_lobby_invites()
    local steam = current_steam()
    local lobby_id = state().by_steam[steam]
    local lobby = lobby_id ~= nil and state().lobbies[lobby_id] or nil
    if lobby == nil then
        lobby = hydrate_persisted_match(persisted_match_for_current())
    end
    local member = lobby ~= nil and lobby.members[steam] or nil
    if lobby == nil or member == nil then
        return emitted_closed_lobby or emitted_invite
    end

    member.last_seen = PB.now()
    runtime.Log("client_hello restoring lobby=" .. tostring(lobby.id) .. " steam=" .. steam ..
        " state=" .. tostring(lobby.state) .. " game_state=" .. tostring(lobby.game_state) ..
        " leaver=" .. tostring(member.leaver or LEAVER_NONE))
    gc.Proto(MSG.SOCacheSubscribed, lobby_so_cache_subscribed(lobby))
    gc.Proto(MSG.SOSingleObject, lobby_single_object(lobby))
    return true
end

function dota_lobby_current_server()
    local lobby_id = state().by_steam[current_steam()]
    if lobby_id == nil then
        return "0"
    end

    local lobby = state().lobbies[lobby_id]
    if lobby == nil or lobby.server_steam == nil then
        return "0"
    end

    return tostring(lobby.server_steam)
end

function dota_lobby_tick()
    if gc.DotaLobbyInvitePrune ~= nil then
        for _, record in ipairs(lobby_invite_records(gc.DotaLobbyInvitePrune())) do
            queue_lobby_invite_removed(record)
        end
    end

    local now = PB.now()
    local cutoff = now - LOBBY_TIMEOUT_SECONDS
    local expired = {}
    local expired_reasons = {}
    for id, lobby in pairs(state().lobbies) do
        if lobby.signed_out == true and (lobby.signed_out_at or 0) > 0 and
            now - lobby.signed_out_at >= POSTGAME_CLEANUP_SECONDS then
            expired[#expired + 1] = id
            expired_reasons[id] = "postgame_cleanup"
        elseif (lobby.last_activity or 0) < cutoff then
            expired[#expired + 1] = id
            expired_reasons[id] = "inactive_timeout"
        else
            local changed = false
            if lobby.dedicated == true and lobby.state == LOBBY_SERVER_ASSIGN and gc.DotaDedicatedServerStatus ~= nil then
                local dedicated_status = tostring(gc.DotaDedicatedServerStatus(tostring(lobby.id)) or "")
                if dedicated_status == "failed" or dedicated_status == "stopped" or dedicated_status == "not_found" then
                    runtime.Log("dedicated_start_failed lobby=" .. tostring(lobby.id) .. " status=" .. dedicated_status)
                    lobby.dedicated = false
                    lobby.state = LOBBY_UI
                    lobby.server_steam = "0"
                    lobby.connect = ""
                    lobby.game_state = GAME_INIT
                    lobby.game_start_time = 0
                    lobby.realtime_sent = false
                    changed = true
                    local notice = PB.str(1, "The dedicated server could not start the match.")
                    for _, member in ipairs(member_order(lobby)) do
                        queue_proto(member.steam, MSG.GCToClientBroadcastNotification, notice)
                    end
                end
            end

            -- Map loading and the GC poll loop race on some dedicated starts.
            -- If the first SERVERSETUP update was delivered while Dota was
            -- still switching loop modes it is consumed without producing
            -- ServerAvailable (4506). Re-issue only the SO update, with a new
            -- version, after the lobby cache has had time to install. Caches
            -- and RUN are never retransmitted here; 4506 remains authoritative.
            if lobby.dedicated == true and lobby.state == LOBBY_SERVER_SETUP and
                lobby.server_setup_ready == true and lobby.server_steam ~= nil and
                lobby.server_steam ~= "0" then
                local attempts = tonumber(lobby.server_setup_update_attempts or 0) or 0
                local last_update = tonumber(lobby.server_setup_update_at or 0) or 0
                if attempts > 0 and attempts < 3 and now - last_update >= 2 then
                    refresh_lobby(lobby)
                    queue_proto(lobby.server_steam, MSG.SOCacheUpdated, lobby_server_multiple_objects(lobby))
                    lobby.server_setup_update_attempts = attempts + 1
                    lobby.server_setup_update_at = now
                    changed = true
                    runtime.Log("dedicated_setup_update_retry lobby=" .. tostring(lobby.id) ..
                        " server=" .. tostring(lobby.server_steam) ..
                        " attempt=" .. tostring(lobby.server_setup_update_attempts))
                end
            end
            for _, member in ipairs(member_order(lobby)) do
                if member.leaver == LEAVER_DISCONNECTED and member.disconnected_at ~= nil and
                    now - member.disconnected_at >= RECONNECT_TIMEOUT_SECONDS then
                    member.leaver = LEAVER_DISCONNECTED_TOO_LONG
                    changed = true
                    runtime.Log("player_reconnect_timeout lobby=" .. tostring(lobby.id) ..
                        " steam=" .. tostring(member.steam))
                end
            end
            if changed then
                refresh_lobby(lobby)
                broadcast_lobby(lobby, nil, true)
                publish_lobby(lobby)
            end
        end
    end

    for _, id in ipairs(expired) do
        local lobby = state().lobbies[id]
        if lobby ~= nil then
            destroy_lobby(lobby, expired_reasons[id] or "inactive_timeout")
        end
    end
end
