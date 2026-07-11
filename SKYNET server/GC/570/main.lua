include("messages.lua")
include("wire.lua")
include("dota_lobby.lua")
include("dota_party.lua")
include("dota_items.lua")
include("dota_stats.lua")
if runtime.Exists("routes.generated.lua") then
    include("routes.generated.lua")
end

function fixture(direction, message_type, index)
    if observed_fixture == nil then return nil end
    return observed_fixture(direction, message_type, index or 1)
end

function observed_proto(message_type, index)
    local payload = fixture("server", message_type, index)
    if payload == nil then return false end
    return gc.Proto(message_type, payload)
end

function observed_reply(message_type, index)
    local payload = fixture("server", message_type, index)
    if payload == nil then return false end
    return gc.Reply(message_type, payload)
end

function observed_reply_cycle(message_type, count, bucket)
    return observed_reply(message_type, gc.NextObservedIndex(bucket or tostring(message_type), count))
end

function observed_proto_cycle(message_type, count, bucket)
    return observed_proto(message_type, gc.NextObservedIndex(bucket or tostring(message_type), count))
end

function observed_sequence(items)
    local handled = false
    for _, item in ipairs(items) do
        local kind = item[1]
        local message_type = item[2]
        local index = item[3] or 1
        if kind == "reply" then
            handled = observed_reply(message_type, index) or handled
        else
            handled = observed_proto(message_type, index) or handled
        end
    end

    return handled
end

function login_from_nethook()
    if gc.ClientHello ~= nil then
        runtime.Log("GCClientHello handled by DotaGcBackend.ClientHello")
        if gc.ClientHello() then
            dota_lobby_on_client_hello()
            return true
        end

        runtime.Log("DotaGcBackend.ClientHello returned false; falling back to Lua replay")
    end

    gc.Proto(MSG.GCClientConnectionStatus, gc.ConnectionStatus())
    gc.Proto(MSG.GCClientWelcome, gc.ClientWelcomeResponse())

    local items = {
        { "proto", MSG.ObservedWelcomeAux, 1 },
        { "proto", MSG.DOTAGetEventPointsResponse, 1 },
        { "proto", MSG.ObservedWelcomeAux, 2 },
        { "proto", MSG.DOTAGetEventPointsResponse, 2 },
        { "proto", MSG.ObservedWelcomeAux, 3 },
        { "proto", MSG.DOTAGetEventPointsResponse, 3 },
        { "proto", MSG.ObservedWelcomeAux, 4 },
        { "proto", MSG.DOTAGetEventPointsResponse, 4 },
        { "proto", MSG.ObservedWelcomeAux, 5 },
        { "proto", MSG.DOTAGetEventPointsResponse, 5 },
        { "proto", MSG.ObservedWelcomeAux, 6 },
        { "proto", MSG.DOTAGetEventPointsResponse, 6 },
        { "proto", MSG.ObservedWelcomeAux, 7 },
        { "proto", MSG.DOTAGetEventPointsResponse, 7 },
        { "proto", MSG.ObservedWelcomeAux, 8 },
        { "proto", MSG.DOTAGetEventPointsResponse, 8 }
    }

    observed_sequence(items)
    gc.Proto(MSG.SOCacheSubscribed, gc.GameSoCacheSubscribed())
    gc.Proto(MSG.SOCacheSubscribed, gc.EconSoCacheSubscribed())
    dota_party_on_login()
    observed_proto(MSG.GCToClientGuildMembershipUpdated, 1)
    observed_proto(MSG.GCToClientOverwatchCasesAvailable, 1)
    gc.Proto(MSG.GCToClientProfileCardUpdated, gc.DotaStatsProfileCardUpdate())
    gc.Proto(MSG.GCClientConnectionStatus, gc.ConnectionStatusHaveSession())
    dota_lobby_on_client_hello()

    return true
end

function lobby_create_from_nethook()
    local handled = observed_proto(MSG.SOCacheSubscribed, 2)
    handled = observed_reply(MSG.GCPracticeLobbyResponse, 1) or handled
    return handled
end

function lobby_update_from_nethook()
    local handled = observed_proto_cycle(MSG.SOCacheUpdated, 7, "lobby_so_update")
    handled = observed_reply_cycle(MSG.GCPracticeLobbyResponse, 6, "lobby_response") or handled
    return handled
end

function lobby_launch_from_nethook()
    return observed_proto(MSG.SOCacheUpdated, 7)
end

function lobby_apply_team_from_nethook()
    local handled = observed_proto_cycle(MSG.SOCacheUpdated, 7, "lobby_apply_team")
    handled = observed_proto(MSG.GCGenericResult, 1) or handled
    return handled
end

function bool_payload(field, value)
    return PB.v(field, value and 1 or 0)
end

function hero_stats_history_response()
    local hero_id = gc.ReadVarint(1, 0)
    local payload = PB.v(3, 1)
    if hero_id ~= 0 then
        payload = PB.cat(PB.v(1, hero_id), payload)
    end

    return gc.Reply(MSG.GCGetHeroStatsHistoryResponse, payload)
end

function handle()
    local msg = gc.MessageType

    if msg == MSG.GCClientHello then
        return login_from_nethook()
    end

    if dota_lobby_handle(msg) then
        return true
    end

    if dota_party_handle(msg) then
        return true
    end

    if dota_items_handle(msg) then
        return true
    end

    if dota_stats_handle(msg) then
        return true
    end

    if msg == MSG.ClientToGCAggregateMetrics or msg == MSG.ClientToGCCancelUnfinalizedTransactions then
        if msg == MSG.ClientToGCCancelUnfinalizedTransactions and observed_reply(MSG.ClientToGCCancelUnfinalizedTransactionsResponse, 1) then
            return true
        end
        return gc.Ignore()
    end

    local handlers = {
        [MSG.GCRequestStoreSalesData] = function() return observed_reply(MSG.GCRequestStoreSalesDataUpToDateResponse, 1) or gc.Reply(MSG.GCRequestStoreSalesDataResponse, gc.StoreSalesDataResponse()) end,
        [MSG.ClientToGCLookupAccountName] = function() return observed_reply_cycle(MSG.ClientToGCLookupAccountNameResponse, 5, "lookup_account_name") end,
        [MSG.GCJoinChatChannel] = function() return observed_reply_cycle(MSG.GCJoinChatChannelResponse, 2, "join_chat_channel") end,
        [MSG.GCGameMatchSignOutPermissionRequest] = function() return gc.Reply(MSG.GCGameMatchSignOutPermissionResponse, bool_payload(1, true)) end,
        [MSG.GCGameMatchSignOutPermissionRequest2] = function() return gc.Reply(MSG.GCGameMatchSignOutPermissionResponse, bool_payload(1, true)) end,
        [MSG.GCSetMatchHistoryAccess] = function() return gc.Reply(MSG.GCSetMatchHistoryAccessResponse, PB.result(1)) end,
        [MSG.GCServerToGCRequestStatus] = function() return gc.Reply(MSG.GCServerToGCRequestStatusResponse, PB.result(0)) end,
        [MSG.GCLeaverDetected] = function() return gc.Reply(MSG.GCLeaverDetectedResponse, PB.result(1)) end,
        [MSG.GCServerToGCRealtimeStats] = function() return gc.Ignore() end,
        [MSG.GCServerToGCMatchStateHistory] = function() return gc.Ignore() end,
        [MSG.GCServerUpdateSpectatorCount] = function() return gc.Ignore() end,
        [MSG.GCLiveScoreboardUpdate] = function() return gc.Ignore() end,
        [MSG.GCMatchmakingStatsRequest] = function() return observed_reply_cycle(MSG.GCMatchmakingStatsResponse, 5, "matchmaking_stats") or gc.Reply(MSG.GCMatchmakingStatsResponse, gc.MatchmakingStatsResponse()) end,
        [MSG.DOTAGetEventPoints] = function() return observed_reply_cycle(MSG.DOTAGetEventPointsResponse, 10, "event_points") or gc.Reply(MSG.DOTAGetEventPointsResponse, gc.EventPointsResponse()) end,
        [MSG.GCNotificationsRequest] = function() return observed_reply(MSG.GCNotificationsResponse, 1) or gc.Reply(MSG.GCNotificationsResponse, gc.NotificationsResponse()) end,
        [MSG.ClientToGCEmoticonDataRequest] = function() return observed_reply(MSG.GCToClientEmoticonData, 1) end,
        [MSG.ClientToGCGetProfileCard] = function() return observed_reply(MSG.ClientToGCGetProfileCardResponse, 1) or gc.Reply(MSG.ClientToGCGetProfileCardResponse, gc.ProfileCardResponse(false)) end,
        [MSG.ClientToGCGetBattleReportInfo] = function() return observed_reply(MSG.ClientToGCGetBattleReportInfoResponse, 1) end,
        [MSG.ClientToGCGetProfileCardStats] = function() return observed_reply(MSG.ClientToGCGetProfileCardStatsResponse, 1) or gc.Reply(MSG.ClientToGCGetProfileCardStatsResponse, gc.ProfileCardResponse(true)) end,
        [MSG.ClientToGCGetProfileTickets] = function() return gc.Reply(MSG.ClientToGCGetProfileTicketsResponse, gc.ProfileTicketsResponse()) end,
        [MSG.ClientToGCGetQuestProgress] = function() return observed_reply(MSG.ClientToGCGetQuestProgressResponse, 1) or gc.Reply(MSG.ClientToGCGetQuestProgressResponse, gc.Result(1)) end,
        [MSG.ClientToGCFindTopSourceTVGames] = function() return observed_reply(MSG.GCToClientFindTopSourceTVGamesResponse, 1) or gc.ReplyEmpty(MSG.GCToClientFindTopSourceTVGamesResponse) end,
        [MSG.GCGetHeroStandings] = function() return gc.ReplyEmpty(MSG.GCGetHeroStandingsResponse) end,
        [MSG.GCGetHeroStatsHistory] = hero_stats_history_response,
        [MSG.ClientToGCLatestConductScorecardRequest] = function() return observed_reply(MSG.ClientToGCLatestConductScorecard, 1) end,
        [MSG.ClientToGCMyTeamInfoRequest] = function() return observed_reply(MSG.GCToClientTeamsInfo, 1) end,
        [MSG.DOTAGetPeriodicResource] = function() return observed_reply(MSG.DOTAGetPeriodicResourceResponse, 1) end,
        [MSG.ProfileRequest] = function() return gc.Reply(MSG.ProfileResponse, gc.ProfileResponse()) end,
        [MSG.ClientToGCRequestGuildData] = function() return observed_reply_cycle(MSG.ClientToGCRequestGuildDataResponse, 3, "guild_data") or gc.Reply(MSG.ClientToGCRequestGuildDataResponse, gc.Result(5)) end,
        [MSG.ClientToGCRequestGuildMembership] = function() return observed_reply(MSG.ClientToGCRequestGuildMembershipResponse, 1) or gc.Reply(MSG.ClientToGCRequestGuildMembershipResponse, gc.GuildMembershipResponse()) end,
        [MSG.ClientToGCUnknown8716] = function() return gc.Ignore() end,
        [MSG.ClientToGCRequestAccountGuildPersonaInfo] = function() return gc.Reply(MSG.ClientToGCRequestAccountGuildPersonaInfoResponse, gc.AccountGuildPersonaInfoResponse()) end,
        [MSG.ClientToGCRequestAccountGuildPersonaInfoBatch] = function() return observed_reply(MSG.ClientToGCRequestAccountGuildPersonaInfoBatchResponse, 1) or gc.Reply(MSG.ClientToGCRequestAccountGuildPersonaInfoBatchResponse, gc.AccountGuildPersonaInfoBatchResponse()) end,
        [MSG.ClientToGCGetCurrentPrivateCoachingSession] = function() return observed_reply(MSG.ClientToGCGetCurrentPrivateCoachingSessionResponse, 1) or gc.Reply(MSG.ClientToGCGetCurrentPrivateCoachingSessionResponse, gc.Result(1)) end,
        [MSG.ClientToGCGetAvailablePrivateCoachingSessions] = function() return gc.Reply(MSG.ClientToGCGetAvailablePrivateCoachingSessionsResponse, gc.AvailablePrivateCoachingSessionsResponse()) end,
        [MSG.ClientToGCGetAvailablePrivateCoachingSessionsSummary] = function() return observed_reply(MSG.ClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse, 1) or gc.Reply(MSG.ClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse, gc.AvailablePrivateCoachingSessionsSummaryResponse()) end,
        [MSG.ClientToGCGetHeroStickers] = function() return observed_reply(MSG.ClientToGCGetHeroStickersResponse, 1) end,
        [MSG.ClientToGCRankRequest] = function() return observed_reply_cycle(MSG.GCToClientRankResponse, 3, "rank_request") or gc.Reply(MSG.GCToClientRankResponse, "") end,
        [MSG.ClientToGCShowcaseGetUserData] = function() return gc.Reply(MSG.ClientToGCShowcaseGetUserDataResponse, gc.DotaStatsShowcaseGetUserDataResponse()) end,
        [MSG.ClientToGCOverworldGetUserData] = function() return observed_reply(MSG.ClientToGCOverworldGetUserDataResponse, 1) or gc.Reply(MSG.ClientToGCOverworldGetUserDataResponse, gc.Result(5)) end,
        [MSG.ClientToGCMonsterHunterGetUserData] = function() return observed_reply(MSG.ClientToGCMonsterHunterGetUserDataResponse, 1) or gc.Reply(MSG.ClientToGCMonsterHunterGetUserDataResponse, gc.Result(1)) end
    }

    local handler = handlers[msg]
    if handler ~= nil then
        return handler()
    end

    return false
end

function tick()
    dota_lobby_tick()
end
