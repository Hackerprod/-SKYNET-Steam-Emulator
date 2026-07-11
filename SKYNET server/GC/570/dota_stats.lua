function dota_stats_handle(message_type)
    local handlers = {
        [MSG.ClientToGCLookupAccountName] = function()
            return gc.Reply(MSG.ClientToGCLookupAccountNameResponse, gc.DotaStatsLookupAccountNameResponse())
        end,
        [MSG.GCSetMatchHistoryAccess] = function()
            return gc.Reply(MSG.GCSetMatchHistoryAccessResponse, gc.DotaStatsSetMatchHistoryAccessResponse())
        end,
        [MSG.DOTAGetEventPoints] = function()
            return gc.Reply(MSG.DOTAGetEventPointsResponse, gc.DotaStatsEventPointsResponse())
        end,
        [MSG.ClientToGCGetProfileCard] = function()
            return gc.Reply(MSG.ClientToGCGetProfileCardResponse, gc.DotaStatsProfileCardResponse(false))
        end,
        [MSG.ClientToGCGetProfileCardStats] = function()
            return gc.Reply(MSG.ClientToGCGetProfileCardStatsResponse, gc.DotaStatsProfileCardResponse(true))
        end,
        [MSG.ClientToGCSetProfileCardSlots] = function()
            return gc.Proto(MSG.GCToClientProfileCardUpdated, gc.DotaStatsSaveProfileCardSlots())
        end,
        [MSG.ProfileUpdate] = function()
            local handled = gc.Reply(MSG.ProfileUpdateResponse, gc.DotaStatsProfileUpdateResponse())
            gc.Proto(MSG.GCToClientProfileCardUpdated, gc.DotaStatsProfileCardUpdate())
            return handled
        end,
        [MSG.ProfileRequest] = function()
            return gc.Reply(MSG.ProfileResponse, gc.DotaStatsProfileResponse())
        end,
        [MSG.ClientToGCGetTrophyList] = function()
            return gc.Reply(MSG.ClientToGCGetTrophyListResponse, gc.DotaStatsTrophyListResponse())
        end,
        [MSG.ClientToGCGetAllHeroOrder] = function()
            return gc.Reply(MSG.ClientToGCGetAllHeroOrderResponse, gc.DotaStatsAllHeroOrderResponse())
        end,
        [MSG.ClientToGCGetAllHeroProgress] = function()
            return gc.Reply(MSG.ClientToGCGetAllHeroProgressResponse, gc.DotaStatsAllHeroProgressResponse())
        end,
        [MSG.ClientToGCPlayerStatsRequest] = function()
            return gc.Reply(MSG.GCToClientPlayerStatsResponse, gc.DotaStatsPlayerStatsResponse())
        end,
        [MSG.GCGetHeroStandings] = function()
            return gc.Reply(MSG.GCGetHeroStandingsResponse, gc.DotaStatsHeroStandingsResponse())
        end,
        [MSG.DOTAGetPlayerMatchHistory] = function()
            return gc.Reply(MSG.DOTAGetPlayerMatchHistoryResponse, gc.DotaStatsPlayerMatchHistoryResponse())
        end,
        [MSG.GCMatchDetailsRequest] = function()
            return gc.Reply(MSG.GCMatchDetailsResponse, gc.DotaStatsMatchDetailsResponse())
        end,
        [MSG.GCRetrieveMatchVote] = function()
            return gc.Reply(MSG.GCMatchVoteResponse, gc.DotaStatsMatchVoteResponse())
        end,
        [MSG.ClientToGCSocialMatchDetailsRequest] = function()
            return gc.Reply(MSG.GCToClientSocialMatchDetailsResponse, gc.DotaStatsSocialMatchDetailsResponse())
        end,
        [MSG.ClientToGCSocialMatchPostCommentRequest] = function()
            return gc.Reply(MSG.GCToClientSocialMatchPostCommentResponse, gc.DotaStatsSocialMatchPostCommentResponse())
        end,
        [MSG.ClientToGCGetProfileTickets] = function()
            return gc.Reply(MSG.ClientToGCGetProfileTicketsResponse, gc.DotaStatsProfileTicketsResponse())
        end,
        [MSG.GCGetHeroStatsHistory] = function()
            return gc.Reply(MSG.GCGetHeroStatsHistoryResponse, gc.DotaStatsHeroStatsHistoryResponse())
        end,
        [MSG.ClientToGCLatestConductScorecardRequest] = function()
            return gc.Reply(MSG.ClientToGCLatestConductScorecard, gc.DotaStatsConductScorecardResponse())
        end,
        [MSG.GCSubmitPlayerReport] = function()
            local handled = gc.Reply(MSG.GCSubmitPlayerReportResponse, gc.DotaStatsSubmitPlayerReportResponse())
            if gc.DotaStatsIsCommendReport() then
                gc.QueueToString(gc.DotaStatsReportTargetSteamIdString(), MSG.GCToClientCommendNotification, gc.DotaStatsCommendNotification())
            end
            return handled
        end,
        [MSG.GCGetHeroTimedStats] = function()
            return gc.Reply(MSG.GCGetHeroTimedStatsResponse, gc.DotaStatsHeroTimedStatsResponse())
        end,
        [MSG.HeroGlobalDataRequest] = function()
            return gc.Reply(MSG.HeroGlobalDataResponse, gc.DotaStatsHeroGlobalDataResponse())
        end,
        [MSG.ClientToGCTeammateStatsRequest] = function()
            return gc.Reply(MSG.ClientToGCTeammateStatsResponse, gc.DotaStatsTeammateStatsResponse())
        end,
        [MSG.ClientToGCRankRequest] = function()
            return gc.Reply(MSG.GCToClientRankResponse, gc.DotaStatsRankResponse())
        end,
        [MSG.ClientToGCShowcaseGetUserData] = function()
            return gc.Reply(MSG.ClientToGCShowcaseGetUserDataResponse, gc.DotaStatsShowcaseGetUserDataResponse())
        end,
        [MSG.ClientToGCRequestSocialFeed] = function()
            return gc.Reply(MSG.ClientToGCRequestSocialFeedResponse, gc.DotaStatsSocialFeedResponse())
        end,
        [MSG.ClientToGCRequestSocialFeedComments] = function()
            return gc.Reply(MSG.ClientToGCRequestSocialFeedCommentsResponse, gc.DotaStatsSocialFeedCommentsResponse())
        end,
        [MSG.ClientToGCSocialFeedPostCommentRequest] = function()
            return gc.Reply(MSG.GCToClientSocialFeedPostCommentResponse, gc.DotaStatsSocialFeedPostCommentResponse())
        end,
        [MSG.ClientToGCRequestPlayerRecentAccomplishments] = function()
            return gc.Reply(MSG.ClientToGCRequestPlayerRecentAccomplishmentsResponse, gc.DotaStatsClientRecentAccomplishmentsResponse())
        end,
        [MSG.ClientToGCRequestPlayerHeroRecentAccomplishments] = function()
            return gc.Reply(MSG.ClientToGCRequestPlayerHeroRecentAccomplishmentsResponse, gc.DotaStatsClientHeroRecentAccomplishmentsResponse())
        end,
        [MSG.ClientToGCHasPlayerVotedForMVP] = function()
            return gc.Reply(MSG.ClientToGCHasPlayerVotedForMVPResponse, gc.DotaStatsHasMvpVoteResponse())
        end,
        [MSG.ClientToGCVoteForMVP] = function()
            return gc.Reply(MSG.ClientToGCVoteForMVPResponse, gc.DotaStatsVoteForMvpResponse())
        end,
        [MSG.ClientToGCMVPVoteTimeout] = function()
            return gc.Reply(MSG.ClientToGCMVPVoteTimeoutResponse, gc.DotaStatsMvpVoteTimeoutResponse())
        end,
        [MSG.GCSubmitLobbyMVPVote] = function()
            return gc.Reply(MSG.GCSubmitLobbyMVPVoteResponse, gc.DotaStatsSubmitLobbyMvpVoteResponse())
        end,
        [MSG.SignOutMVPStats] = function()
            gc.DotaStatsRecordSignOutMvpStats()
            return gc.Ignore()
        end,
        [MSG.ClientToGCRerollPlayerChallenge] = function()
            return gc.Reply(MSG.ClientToGCRerollPlayerChallengeResponse, PB.v(1, 0))
        end
    }

    local handler = handlers[message_type]
    if handler == nil then
        return false
    end

    return handler()
end
