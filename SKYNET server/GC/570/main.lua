include("messages.lua")
include("wire.lua")
include("dota_lobby.lua")
include("dota_party.lua")
include("dota_items.lua")
include("dota_stats.lua")

function login_from_nethook()
    if gc.ClientHello ~= nil then
        runtime.Log("GCClientHello handled by DotaGcBackend.ClientHello")
        if gc.ClientHello() then
            dota_party_on_login()
            dota_lobby_on_client_hello()
            return true
        end

        runtime.Log("DotaGcBackend.ClientHello returned false; using dynamic Lua welcome fallback")
    end

    gc.Proto(MSG.GCClientConnectionStatus, gc.ConnectionStatus())
    gc.Proto(MSG.DOTAGetEventPointsResponse, gc.DotaStatsEventPointsResponse())
    gc.Proto(MSG.GCClientWelcome, gc.ClientWelcomeResponse())
    gc.Proto(MSG.SOCacheSubscribed, gc.GameSoCacheSubscribed())
    gc.Proto(MSG.SOCacheSubscribed, gc.EconSoCacheSubscribed())
    dota_party_on_login()
    gc.Proto(MSG.GCToClientProfileCardUpdated, gc.DotaStatsProfileCardUpdate())
    gc.Proto(MSG.GCClientConnectionStatus, gc.ConnectionStatusHaveSession())
    dota_lobby_on_client_hello()

    return true
end

function lobby_create_from_nethook()
    return gc.Reply(MSG.GCPracticeLobbyResponse, PB.result(1))
end

function lobby_update_from_nethook()
    return gc.Reply(MSG.GCPracticeLobbyResponse, PB.result(1))
end

function lobby_launch_from_nethook()
    return true
end

function lobby_apply_team_from_nethook()
    return gc.Proto(MSG.GCGenericResult, PB.result(1))
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
        return gc.Ignore()
    end

    local handlers = {
        [MSG.GCRequestStoreSalesData] = function() return gc.Reply(MSG.GCRequestStoreSalesDataResponse, gc.StoreSalesDataResponse()) end,
        [MSG.ClientToGCLookupAccountName] = function() return gc.Reply(MSG.ClientToGCLookupAccountNameResponse, gc.DotaStatsLookupAccountNameResponse()) end,
        [MSG.GCJoinChatChannel] = function() return gc.Reply(MSG.GCJoinChatChannelResponse, gc.DotaJoinChatChannelResponse()) end,
        [MSG.GCGameMatchSignOutPermissionRequest] = function() return gc.Reply(MSG.GCGameMatchSignOutPermissionResponse, bool_payload(1, true)) end,
        [MSG.GCGameMatchSignOutPermissionRequest2] = function() return gc.Reply(MSG.GCGameMatchSignOutPermissionResponse, bool_payload(1, true)) end,
        [MSG.GCSetMatchHistoryAccess] = function() return gc.Reply(MSG.GCSetMatchHistoryAccessResponse, PB.result(1)) end,
        [MSG.GCServerToGCRequestStatus] = function() return gc.Reply(MSG.GCServerToGCRequestStatusResponse, PB.result(0)) end,
        [MSG.GCLeaverDetected] = function() return gc.Reply(MSG.GCLeaverDetectedResponse, PB.result(1)) end,
        [MSG.GCServerToGCRealtimeStats] = function() return gc.Ignore() end,
        [MSG.GCServerToGCMatchStateHistory] = function() return gc.Ignore() end,
        [MSG.GCServerUpdateSpectatorCount] = function() return gc.Ignore() end,
        [MSG.GCLiveScoreboardUpdate] = function() return gc.Ignore() end,
        [MSG.GCMatchmakingStatsRequest] = function() return gc.Reply(MSG.GCMatchmakingStatsResponse, gc.MatchmakingStatsResponse()) end,
        [MSG.DOTAGetEventPoints] = function() return gc.Reply(MSG.DOTAGetEventPointsResponse, gc.DotaStatsEventPointsResponse()) end,
        [MSG.GCNotificationsRequest] = function() return gc.Reply(MSG.GCNotificationsResponse, gc.NotificationsResponse()) end,
        [MSG.ClientToGCEmoticonDataRequest] = function() return gc.Reply(MSG.GCToClientEmoticonData, gc.DotaEmoticonDataResponse()) end,
        [MSG.ClientToGCGetProfileCard] = function() return gc.Reply(MSG.ClientToGCGetProfileCardResponse, gc.DotaStatsProfileCardResponse(false)) end,
        [MSG.ClientToGCGetBattleReportInfo] = function() return gc.Reply(MSG.ClientToGCGetBattleReportInfoResponse, gc.Result(1)) end,
        [MSG.ClientToGCGetProfileCardStats] = function() return gc.Reply(MSG.ClientToGCGetProfileCardStatsResponse, gc.DotaStatsProfileCardResponse(true)) end,
        [MSG.ClientToGCGetProfileTickets] = function() return gc.Reply(MSG.ClientToGCGetProfileTicketsResponse, gc.ProfileTicketsResponse()) end,
        [MSG.ClientToGCGetQuestProgress] = function() return gc.Reply(MSG.ClientToGCGetQuestProgressResponse, gc.DotaQuestProgressResponse()) end,
        [MSG.ClientToGCFindTopSourceTVGames] = function() return gc.ReplyEmpty(MSG.GCToClientFindTopSourceTVGamesResponse) end,
        [MSG.GCGetHeroStandings] = function() return gc.ReplyEmpty(MSG.GCGetHeroStandingsResponse) end,
        [MSG.GCGetHeroStatsHistory] = hero_stats_history_response,
        [MSG.ClientToGCLatestConductScorecardRequest] = function() return gc.Reply(MSG.ClientToGCLatestConductScorecard, gc.DotaStatsConductScorecardResponse()) end,
        [MSG.ClientToGCMyTeamInfoRequest] = function() return gc.Reply(MSG.GCToClientTeamsInfo, gc.DotaTeamsInfoResponse()) end,
        [MSG.DOTAGetPeriodicResource] = function() return gc.Reply(MSG.DOTAGetPeriodicResourceResponse, gc.DotaPeriodicResourceResponse()) end,
        [MSG.ProfileRequest] = function() return gc.Reply(MSG.ProfileResponse, gc.ProfileResponse()) end,
        [MSG.ClientToGCRequestGuildData] = function() return gc.Reply(MSG.ClientToGCRequestGuildDataResponse, gc.Result(5)) end,
        [MSG.ClientToGCRequestGuildMembership] = function() return gc.Reply(MSG.ClientToGCRequestGuildMembershipResponse, gc.GuildMembershipResponse()) end,
        [MSG.ClientToGCUnknown8716] = function() return gc.Ignore() end,
        [MSG.ClientToGCRequestAccountGuildPersonaInfo] = function() return gc.Reply(MSG.ClientToGCRequestAccountGuildPersonaInfoResponse, gc.AccountGuildPersonaInfoResponse()) end,
        [MSG.ClientToGCRequestAccountGuildPersonaInfoBatch] = function() return gc.Reply(MSG.ClientToGCRequestAccountGuildPersonaInfoBatchResponse, gc.AccountGuildPersonaInfoBatchResponse()) end,
        [MSG.ClientToGCGetCurrentPrivateCoachingSession] = function() return gc.Reply(MSG.ClientToGCGetCurrentPrivateCoachingSessionResponse, gc.Result(1)) end,
        [MSG.ClientToGCGetAvailablePrivateCoachingSessions] = function() return gc.Reply(MSG.ClientToGCGetAvailablePrivateCoachingSessionsResponse, gc.AvailablePrivateCoachingSessionsResponse()) end,
        [MSG.ClientToGCGetAvailablePrivateCoachingSessionsSummary] = function() return gc.Reply(MSG.ClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse, gc.AvailablePrivateCoachingSessionsSummaryResponse()) end,
        [MSG.ClientToGCGetHeroStickers] = function() return gc.Reply(MSG.ClientToGCGetHeroStickersResponse, gc.DotaHeroStickersResponse()) end,
        [MSG.ClientToGCRankRequest] = function() return gc.Reply(MSG.GCToClientRankResponse, gc.DotaStatsRankResponse()) end,
        [MSG.ClientToGCShowcaseGetUserData] = function() return gc.Reply(MSG.ClientToGCShowcaseGetUserDataResponse, gc.DotaStatsShowcaseGetUserDataResponse()) end,
        [MSG.ClientToGCOverworldGetUserData] = function() return gc.Reply(MSG.ClientToGCOverworldGetUserDataResponse, gc.Result(5)) end,
        [MSG.ClientToGCMonsterHunterGetUserData] = function() return gc.Reply(MSG.ClientToGCMonsterHunterGetUserDataResponse, gc.Result(1)) end
    }

    local handler = handlers[msg]
    if handler ~= nil then
        return handler()
    end

    return false
end

function tick()
    if gc.DotaPartyTick ~= nil then
        gc.DotaPartyTick()
    end
    dota_lobby_tick()
end
