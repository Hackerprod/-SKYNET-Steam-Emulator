using SKYNET_server.Services;

namespace SKYNET_server.GC.Dota2;

public sealed partial class DotaGcBackend
{
    private const uint EMsgDotaStatsSuccess = 1;
    private const uint SocialFeedResultSuccess = 0;
    private const uint MatchOutcomeRadVictory = 2;
    private const uint MatchOutcomeDireVictory = 3;

    public string DotaStatsLookupAccountNameResponse()
    {
        var accountId = (uint)ReadVarint(1, AccountId);
        var profile = GetStatsProfile(accountId);
        var response = new List<byte>();
        WriteVarintField(response, 1, profile.AccountId);
        WriteStringField(response, 2, profile.PersonaName);
        return Encode(response.ToArray());
    }

    public string DotaStatsProfileCardResponse(bool forceLocalAccount)
    {
        var accountId = AccountId;
        if (!forceLocalAccount && TryReadVarintField(_requestBody, 1, out var requestedAccountId) && requestedAccountId != 0)
        {
            accountId = (uint)requestedAccountId;
        }

        return Encode(BuildDotaStatsProfileCard(accountId));
    }

    public string DotaStatsProfileCardUpdate()
    {
        return Encode(BuildDotaStatsProfileCard(AccountId));
    }

    public string DotaStatsSaveProfileCardSlots()
    {
        var slots = new List<DotaStatsProfileSlot>();
        foreach (var slotBody in ReadLengthDelimitedFields(_requestBody, 1))
        {
            TryReadVarintField(slotBody, 1, out var slotId);
            TryReadVarintField(slotBody, 2, out var slotType);
            TryReadVarintField(slotBody, 3, out var slotValue);
            if (slotId != 0)
            {
                slots.Add(new DotaStatsProfileSlot
                {
                    AccountId = AccountId,
                    SlotId = (uint)slotId,
                    SlotType = (uint)slotType,
                    SlotValue = slotValue
                });
            }
        }

        StatsStore?.SaveProfileSlots(AccountId, slots);
        return Encode(BuildDotaStatsProfileCard(AccountId));
    }

    public string DotaStatsProfileUpdateResponse()
    {
        TryReadVarintField(_requestBody, 1, out var backgroundItemId);
        var featuredHeroes = ReadVarintFields(_requestBody, 2).Select(value => (uint)value).Where(value => value != 0).ToList();
        StatsStore?.SaveProfileUpdate(AccountId, backgroundItemId, featuredHeroes);
        return Encode(BuildResult(0));
    }

    public string DotaStatsProfileResponse()
    {
        var accountId = (uint)ReadVarint(1, AccountId);
        return Encode(BuildDotaStatsProfileResponse(accountId));
    }

    public string DotaStatsTrophyListResponse()
    {
        var accountId = (uint)ReadVarint(1, AccountId);
        var response = new List<byte>();
        foreach (var trophy in StatsStoreOrThrow().GetTrophies(accountId))
        {
            var item = new List<byte>();
            WriteVarintField(item, 1, trophy.TrophyId);
            WriteVarintField(item, 2, trophy.TrophyScore);
            WriteVarintField(item, 3, trophy.LastUpdated);
            WriteBytesField(response, 2, item.ToArray());
        }

        return Encode(response.ToArray());
    }

    public string DotaStatsAllHeroOrderResponse()
    {
        var accountId = (uint)ReadVarint(1, AccountId);
        var progress = StatsStoreOrThrow().GetAllHeroProgress(accountId);
        var response = new List<byte>();
        foreach (var heroId in progress.HeroIds)
        {
            WriteVarintField(response, 1, heroId);
        }

        return Encode(response.ToArray());
    }

    public string DotaStatsAllHeroProgressResponse()
    {
        var accountId = (uint)ReadVarint(1, AccountId);
        var progress = StatsStoreOrThrow().GetAllHeroProgress(accountId);
        var response = new List<byte>();
        WriteVarintField(response, 1, progress.AccountId);
        WriteVarintField(response, 2, progress.CurrentHeroId);
        WriteVarintField(response, 3, progress.LapsCompleted);
        WriteVarintField(response, 4, progress.CurrentHeroGames);
        WriteVarintField(response, 5, progress.CurrentLapStarted);
        WriteVarintField(response, 6, progress.CurrentLapGames);
        WriteVarintField(response, 7, progress.BestLapGames);
        WriteVarintField(response, 8, progress.BestLapTime);
        WriteVarintField(response, 9, progress.LapHeroesCompleted);
        WriteVarintField(response, 10, progress.LapHeroesRemaining);
        WriteVarintField(response, 11, progress.NextHeroId);
        WriteVarintField(response, 12, progress.PreviousHeroId);
        WriteVarintField(response, 13, progress.PreviousHeroGames);
        WriteFloatField(response, 14, 1.0f);
        WriteFloatField(response, 15, 1.0f);
        WriteFloatField(response, 16, 1.0f);
        WriteFloatField(response, 17, 1.0f);
        WriteFloatField(response, 18, 1.0f);
        WriteStringField(response, 19, progress.ProfileName);
        WriteVarintField(response, 20, progress.StartHeroId);
        return Encode(response.ToArray());
    }

    public string DotaStatsPlayerStatsResponse()
    {
        var accountId = (uint)ReadVarint(1, AccountId);
        return Encode(BuildDotaStatsPlayerStatsResponse(accountId));
    }

    public string DotaStatsHeroStandingsResponse()
    {
        var response = new List<byte>();
        foreach (var hero in StatsStoreOrThrow().GetHeroStandings(AccountId))
        {
            var row = new List<byte>();
            WriteVarintField(row, 1, hero.HeroId);
            WriteVarintField(row, 2, hero.Wins);
            WriteVarintField(row, 3, hero.Losses);
            WriteVarintField(row, 4, hero.WinStreak);
            WriteVarintField(row, 5, hero.BestWinStreak);
            WriteFloatField(row, 6, (float)hero.AvgKills);
            WriteFloatField(row, 7, (float)hero.AvgDeaths);
            WriteFloatField(row, 8, (float)hero.AvgAssists);
            WriteFloatField(row, 9, (float)hero.AvgGpm);
            WriteFloatField(row, 10, (float)hero.AvgXpm);
            WriteVarintField(row, 11, hero.BestKills);
            WriteVarintField(row, 12, hero.BestAssists);
            WriteVarintField(row, 13, hero.BestGpm);
            WriteVarintField(row, 14, hero.BestXpm);
            WriteFloatField(row, 15, (float)hero.Performance);
            WriteVarintField(row, 20, hero.NetworthPeak);
            WriteVarintField(row, 21, hero.LasthitPeak);
            WriteVarintField(row, 22, hero.DenyPeak);
            WriteVarintField(row, 23, hero.DamagePeak);
            WriteVarintField(row, 24, hero.LongestGamePeak);
            WriteVarintField(row, 25, hero.HealingPeak);
            WriteFloatField(row, 26, (float)hero.AvgLastHits);
            WriteFloatField(row, 27, (float)hero.AvgDenies);
            WriteBytesField(response, 1, row.ToArray());
        }

        return Encode(response.ToArray());
    }

    public string DotaStatsPlayerMatchHistoryResponse()
    {
        var accountId = (uint)ReadVarint(1, AccountId);
        TryReadVarintField(_requestBody, 2, out var startAtMatchId);
        var requested = (uint)ReadVarint(3, 20);
        var heroId = (uint)ReadVarint(4, 0);
        var requestId = (uint)ReadVarint(5, 0);
        var includePractice = ReadVarint(7, 0) != 0;
        var response = new List<byte>();
        foreach (var match in StatsStoreOrThrow().GetMatchHistory(accountId, startAtMatchId, requested, heroId, includePractice))
        {
            var row = new List<byte>();
            WriteVarintField(row, 1, match.MatchId);
            WriteVarintField(row, 2, match.StartTime);
            WriteVarintField(row, 3, match.HeroId);
            WriteVarintField(row, 4, match.Winner ? 1u : 0u);
            WriteVarintField(row, 5, match.GameMode);
            WriteVarintField(row, 6, 0);
            WriteVarintField(row, 7, 0);
            WriteVarintField(row, 8, match.LobbyType);
            WriteVarintField(row, 9, 0);
            WriteVarintField(row, 10, match.LeaverStatus == 0 ? 0u : 1u);
            WriteVarintField(row, 11, match.Duration);
            WriteVarintField(row, 12, 1);
            WriteVarintField(row, 13, 1);
            WriteVarintField(row, 14, 0);
            WriteVarintField(row, 22, match.SelectedFacet);
            WriteBytesField(response, 1, row.ToArray());
        }

        WriteVarintField(response, 2, requestId);
        return Encode(response.ToArray());
    }

    public string DotaStatsMatchDetailsResponse()
    {
        TryReadVarintField(_requestBody, 1, out var matchId);
        var response = new List<byte>();
        var match = StatsStoreOrThrow().GetMatch(matchId);
        if (match == null)
        {
            WriteVarintField(response, 1, 0);
            WriteVarintField(response, 3, 0);
            return Encode(response.ToArray());
        }

        WriteVarintField(response, 1, EMsgDotaStatsSuccess);
        WriteBytesField(response, 2, BuildDotaStatsMatch(match));
        WriteVarintField(response, 3, 0);
        return Encode(response.ToArray());
    }

    public string DotaStatsMatchVoteResponse()
    {
        TryReadVarintField(_requestBody, 1, out var matchId);
        var vote = StatsStoreOrThrow().GetMatchVotes(matchId, AccountId);
        var response = new List<byte>();
        WriteVarintField(response, 1, vote.Success ? 0u : 1u);
        WriteVarintField(response, 2, vote.Vote);
        WriteVarintField(response, 3, vote.PositiveVotes);
        WriteVarintField(response, 4, vote.NegativeVotes);
        return Encode(response.ToArray());
    }

    public string DotaStatsSocialMatchDetailsResponse()
    {
        TryReadVarintField(_requestBody, 1, out var matchId);
        var response = new List<byte>();
        var match = StatsStoreOrThrow().GetMatch(matchId);
        WriteVarintField(response, 1, 1);
        if (match != null)
        {
            foreach (var comment in StatsStoreOrThrow().GetSocialMatchComments(matchId))
            {
                WriteBytesField(response, 2, BuildDotaStatsSocialComment(comment));
            }
        }

        return Encode(response.ToArray());
    }

    public string DotaStatsSocialMatchPostCommentResponse()
    {
        TryReadVarintField(_requestBody, 1, out var matchId);
        TryReadStringField(_requestBody, 2, out var comment);
        var success = StatsStoreOrThrow().SaveSocialMatchComment(matchId, AccountId, PersonaName, comment);
        var response = new List<byte>();
        WriteVarintField(response, 1, success ? 1u : 0u);
        return Encode(response.ToArray());
    }

    public string DotaStatsProfileTicketsResponse()
    {
        return ProfileTicketsResponse();
    }

    public string DotaStatsHeroStatsHistoryResponse()
    {
        var heroId = (uint)ReadVarint(1, 0);
        var response = new List<byte>();
        if (heroId != 0)
        {
            WriteVarintField(response, 1, heroId);
        }

        foreach (var match in StatsStoreOrThrow().GetRecentMatches(AccountId, 20, heroId))
        {
            var record = new List<byte>();
            WriteVarintField(record, 1, match.MatchId);
            WriteVarintField(record, 2, match.GameMode);
            WriteVarintField(record, 3, match.LobbyType);
            WriteVarintField(record, 4, match.StartTime);
            WriteVarintField(record, 5, match.Winner ? 1u : 0u);
            WriteVarintField(record, 6, match.Gpm);
            WriteVarintField(record, 7, match.Xpm);
            WriteVarintField(record, 8, match.Kills);
            WriteVarintField(record, 9, match.Deaths);
            WriteVarintField(record, 10, match.Assists);
            WriteBytesField(response, 2, record.ToArray());
        }

        WriteVarintField(response, 3, EMsgDotaStatsSuccess);
        return Encode(response.ToArray());
    }

    public string DotaStatsConductScorecardResponse()
    {
        var conduct = StatsStoreOrThrow().GetConduct(AccountId);
        var response = new List<byte>();
        WriteVarintField(response, 1, conduct.AccountId);
        WriteVarintField(response, 2, conduct.MatchId);
        WriteVarintField(response, 3, 0);
        WriteVarintField(response, 4, 0);
        WriteVarintField(response, 5, conduct.MatchesInReport);
        WriteVarintField(response, 6, conduct.MatchesClean);
        WriteVarintField(response, 7, conduct.MatchesReported);
        WriteVarintField(response, 8, conduct.MatchesAbandoned);
        WriteVarintField(response, 9, conduct.ReportsCount);
        WriteVarintField(response, 10, conduct.ReportsParties);
        WriteVarintField(response, 11, conduct.CommendCount);
        WriteVarintField(response, 14, conduct.Date);
        WriteVarintField(response, 17, conduct.RawBehaviorScore);
        WriteVarintField(response, 18, conduct.OldRawBehaviorScore);
        WriteVarintField(response, 19, conduct.CommsReports);
        WriteVarintField(response, 20, conduct.CommsReports);
        WriteVarintField(response, 21, conduct.BehaviorRating);
        return Encode(response.ToArray());
    }

    public string DotaStatsSubmitPlayerReportResponse()
    {
        TryReadVarintField(_requestBody, 1, out var targetAccountId);
        TryReadVarintField(_requestBody, 2, out var reportFlags);
        TryReadVarintField(_requestBody, 4, out var lobbyId);
        TryReadStringField(_requestBody, 5, out var comment);
        StatsStoreOrThrow().SaveReport(SteamId, AccountId, (uint)targetAccountId, lobbyId, (uint)reportFlags, comment);

        var response = new List<byte>();
        WriteVarintField(response, 1, targetAccountId);
        WriteVarintField(response, 2, reportFlags);
        WriteStringField(response, 4, string.Empty);
        WriteVarintField(response, 5, EMsgDotaStatsSuccess);
        return Encode(response.ToArray());
    }

    public string DotaStatsCommendNotification()
    {
        TryReadVarintField(_requestBody, 2, out var reportFlags);
        var response = new List<byte>();
        WriteVarintField(response, 1, AccountId);
        WriteStringField(response, 2, PersonaName);
        WriteVarintField(response, 3, reportFlags);
        WriteVarintField(response, 4, 0);
        return Encode(response.ToArray());
    }

    public string DotaStatsReportTargetSteamIdString()
    {
        TryReadVarintField(_requestBody, 1, out var targetAccountId);
        return (76561197960265728UL + targetAccountId).ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    public bool DotaStatsIsCommendReport()
    {
        return TryReadVarintField(_requestBody, 2, out var reportFlags) && reportFlags == 3840;
    }

    public string DotaStatsHeroTimedStatsResponse()
    {
        var heroId = (uint)ReadVarint(1, 0);
        var response = new List<byte>();
        WriteVarintField(response, 1, heroId);
        var hero = StatsStoreOrThrow().GetHeroStandings(AccountId).FirstOrDefault(item => item.HeroId == heroId);
        var chunk = new List<byte>();
        WriteVarintField(chunk, 1, 0);
        foreach (var time in new[] { 600u, 1200u, 1800u, 2400u, 3000u })
        {
            var allStats = BuildTimedAverages(hero);
            var container = new List<byte>();
            WriteVarintField(container, 1, time);
            WriteBytesField(container, 2, allStats);
            WriteBytesField(container, 3, allStats);
            WriteBytesField(container, 4, allStats);
            WriteBytesField(container, 5, BuildTimedStdDevs());
            WriteBytesField(container, 6, BuildTimedStdDevs());
            WriteBytesField(chunk, 2, container.ToArray());
        }

        WriteBytesField(response, 2, chunk.ToArray());
        return Encode(response.ToArray());
    }

    public string DotaStatsHeroGlobalDataResponse()
    {
        var heroId = (uint)ReadVarint(1, 0);
        var response = new List<byte>();
        WriteVarintField(response, 1, heroId);
        var hero = StatsStoreOrThrow().GetHeroStandings(AccountId).FirstOrDefault(item => item.HeroId == heroId);
        var averages = new List<byte>();
        WriteVarintField(averages, 1, (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        WriteVarintField(averages, 3, hero == null ? 0u : (uint)Math.Round(hero.AvgGpm));
        WriteVarintField(averages, 4, hero == null ? 0u : (uint)Math.Round(hero.AvgXpm));
        WriteVarintField(averages, 5, hero == null ? 0u : (uint)Math.Round(hero.AvgKills));
        WriteVarintField(averages, 6, hero == null ? 0u : (uint)Math.Round(hero.AvgDeaths));
        WriteVarintField(averages, 7, hero == null ? 0u : (uint)Math.Round(hero.AvgAssists));
        WriteVarintField(averages, 8, hero == null ? 0u : (uint)Math.Round(hero.AvgLastHits));
        WriteVarintField(averages, 9, hero == null ? 0u : (uint)Math.Round(hero.AvgDenies));
        WriteVarintField(averages, 10, hero?.NetworthPeak ?? 0);

        var chunk = new List<byte>();
        WriteVarintField(chunk, 1, 0);
        WriteBytesField(chunk, 3, averages.ToArray());
        WriteBytesField(response, 2, chunk.ToArray());
        return Encode(response.ToArray());
    }

    public string DotaStatsTeammateStatsResponse()
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, 1);
        foreach (var teammate in StatsStoreOrThrow().GetTeammateStats(AccountId))
        {
            var row = new List<byte>();
            WriteVarintField(row, 1, teammate.AccountId);
            WriteVarintField(row, 2, teammate.Games);
            WriteVarintField(row, 3, teammate.Wins);
            WriteVarintField(row, 4, teammate.MostRecentGameTimestamp);
            WriteVarintField(row, 5, teammate.MostRecentGameMatchId);
            var performance = teammate.Games == 0 ? 0f : teammate.Wins * 100f / teammate.Games;
            WriteFloatField(row, 100, performance);
            WriteBytesField(response, 2, row.ToArray());
        }

        return Encode(response.ToArray());
    }

    public string DotaStatsRankResponse()
    {
        var profile = GetStatsProfile(AccountId);
        var response = new List<byte>();
        WriteVarintField(response, 1, 0);
        WriteVarintField(response, 2, profile.RankTier);
        WriteVarintField(response, 3, profile.RankScore);
        WriteVarintField(response, 4, profile.RankTierScore);
        WriteVarintField(response, 5, profile.LeaderboardRank);
        return Encode(response.ToArray());
    }

    public string DotaStatsSocialFeedResponse()
    {
        var accountId = (uint)ReadVarint(1, AccountId);
        var selfOnly = ReadVarint(2, 0) != 0;
        var response = new List<byte>();
        WriteVarintField(response, 1, SocialFeedResultSuccess);
        foreach (var item in StatsStoreOrThrow().GetSocialFeed(accountId, selfOnly))
        {
            var row = new List<byte>();
            WriteVarintField(row, 1, item.FeedEventId);
            WriteVarintField(row, 2, item.AccountId);
            WriteVarintField(row, 3, item.Timestamp);
            WriteVarintField(row, 4, item.CommentCount);
            WriteVarintField(row, 5, item.EventType);
            WriteVarintField(row, 6, item.EventSubType);
            WriteVarintField(row, 7, item.ParamBigInt1);
            WriteVarintField(row, 8, item.ParamInt1);
            WriteVarintField(row, 9, item.ParamInt2);
            WriteVarintField(row, 10, item.ParamInt3);
            WriteStringField(row, 11, item.ParamString);
            WriteBytesField(response, 2, row.ToArray());
        }

        return Encode(response.ToArray());
    }

    public string DotaStatsSocialFeedCommentsResponse()
    {
        TryReadVarintField(_requestBody, 1, out var feedEventId);
        var response = new List<byte>();
        WriteVarintField(response, 1, SocialFeedResultSuccess);
        foreach (var comment in StatsStoreOrThrow().GetSocialFeedComments(feedEventId))
        {
            var row = new List<byte>();
            WriteVarintField(row, 1, comment.AccountId);
            WriteVarintField(row, 2, comment.Timestamp);
            WriteStringField(row, 3, comment.Comment);
            WriteBytesField(response, 3, row.ToArray());
        }

        return Encode(response.ToArray());
    }

    public string DotaStatsSocialFeedPostCommentResponse()
    {
        TryReadVarintField(_requestBody, 1, out var feedEventId);
        TryReadStringField(_requestBody, 2, out var comment);
        var success = StatsStoreOrThrow().SaveSocialFeedComment(feedEventId, AccountId, comment);
        var response = new List<byte>();
        WriteVarintField(response, 1, success ? 1u : 0u);
        return Encode(response.ToArray());
    }

    public string DotaStatsEventPointsResponse()
    {
        var eventId = (uint)ReadVarint(1, ActiveEventId);
        var accountId = (uint)ReadVarint(2, AccountId);
        var profile = GetStatsProfile(accountId);
        var response = new List<byte>();
        WriteVarintField(response, 1, profile.EventPoints);
        WriteVarintField(response, 2, 0);
        WriteVarintField(response, 3, eventId);
        WriteVarintField(response, 4, profile.EventPoints);
        WriteVarintField(response, 5, 0);
        WriteVarintField(response, 7, profile.AccountId);
        WriteVarintField(response, 8, 1);
        WriteVarintField(response, 9, 35);
        WriteVarintField(response, 10, ActiveEventId);
        return Encode(response.ToArray());
    }

    public string DotaStatsBatchPlayerResourcesResponse()
    {
        var response = new List<byte>();
        foreach (var account in ReadVarintFields(_requestBody, 1).Select(value => (uint)value))
        {
            var profile = GetStatsProfile(account);
            var conduct = StatsStoreOrThrow().GetConduct(account);
            var row = new List<byte>();
            WriteVarintField(row, 1, profile.AccountId);
            WriteVarintField(row, 4, profile.RankTier);
            WriteVarintField(row, 5, profile.RankTier == 0 ? 0u : 1u);
            WriteVarintField(row, 6, 0);
            WriteVarintField(row, 7, profile.LifetimeGames == 0 ? 1u : 0u);
            WriteVarintField(row, 8, 0);
            WriteVarintField(row, 9, 5);
            WriteVarintField(row, 10, 5);
            WriteVarintField(row, 11, profile.Wins);
            WriteVarintField(row, 12, profile.Losses);
            WriteVarintField(row, 13, 0);
            WriteVarintField(row, 14, conduct.RawBehaviorScore);
            WriteVarintField(row, 15, conduct.RawBehaviorScore);
            WriteVarintField(row, 16, 0);
            WriteBytesField(response, 6, row.ToArray());
        }

        return Encode(response.ToArray());
    }

    public string DotaStatsShowcaseGetUserDataResponse()
    {
        var accountId = (uint)ReadVarint(1, AccountId);
        if (accountId == 0)
        {
            accountId = AccountId;
        }

        var response = new List<byte>();
        WriteVarintField(response, 1, EMsgDotaStatsSuccess);
        WriteBytesField(response, 2, BuildDotaStatsShowcase(accountId));
        return Encode(response.ToArray());
    }

    public string DotaStatsHasMvpVoteResponse()
    {
        var matchId = ReadVarint(1, 0);
        var response = new List<byte>();
        WriteVarintField(response, 1, StatsStoreOrThrow().HasMvpVote(matchId, AccountId) ? 1u : 0u);
        return Encode(response.ToArray());
    }

    public string DotaStatsVoteForMvpResponse()
    {
        var matchId = ReadVarint(1, 0);
        var votedAccountId = (uint)ReadVarint(3, 0);
        var response = new List<byte>();
        WriteVarintField(response, 1, StatsStoreOrThrow().SaveMvpVote(matchId, AccountId, votedAccountId) ? 1u : 0u);
        return Encode(response.ToArray());
    }

    public string DotaStatsMvpVoteTimeoutResponse()
    {
        var matchId = ReadVarint(1, 0);
        var response = new List<byte>();
        WriteVarintField(response, 1, StatsStoreOrThrow().FinalizeMvpVotes(matchId) ? 1u : 0u);
        return Encode(response.ToArray());
    }

    public string DotaStatsSubmitLobbyMvpVoteResponse()
    {
        var targetAccountId = (uint)ReadVarint(1, 0);
        var latestMatchId = StatsStoreOrThrow().GetRecentMatches(AccountId, 1).FirstOrDefault()?.MatchId ?? 0;
        var ok = latestMatchId != 0 && StatsStoreOrThrow().SaveMvpVote(latestMatchId, AccountId, targetAccountId);
        if (ok)
        {
            StatsStoreOrThrow().FinalizeMvpVotes(latestMatchId);
        }

        var response = new List<byte>();
        WriteVarintField(response, 1, targetAccountId);
        WriteVarintField(response, 2, ok ? EMsgDotaStatsSuccess : 2u);
        return Encode(response.ToArray());
    }

    public bool DotaStatsRecordSignOutMvpStats()
    {
        var matchId = ReadVarint(1, 0);
        var winners = new List<uint>();
        foreach (var playerBody in ReadLengthDelimitedFields(_requestBody, 5))
        {
            var accountId = (uint)ReadVarintField(playerBody, 3);
            var rank = (uint)ReadVarintField(playerBody, 33);
            if (accountId != 0 && rank == 1)
            {
                winners.Add(accountId);
            }
        }

        return StatsStoreOrThrow().SetMatchMvps(matchId, winners);
    }

    public string DotaStatsGameMatchSignOutResponse(string fallbackMatchId, uint gameMode = 1, uint lobbyType = 1, uint startTime = 0, string serverSteamId = "0")
    {
        var fallback = ParseUInt64(fallbackMatchId);
        var match = ParseDotaStatsMatchSignOut(fallback, gameMode, lobbyType, startTime, ParseUInt64(serverSteamId));
        StatsStoreOrThrow().RecordMatch(match);
        return Encode(BuildDotaStatsGameMatchSignOutResponse(match));
    }

    public string DotaStatsServerRecentAccomplishmentsResponse()
    {
        var accountId = (uint)ReadVarint(1, AccountId);
        var heroId = (uint)ReadVarint(2, 0);
        var response = new List<byte>();
        WriteVarintField(response, 1, EMsgDotaStatsSuccess);
        WriteBytesField(response, 2, BuildRecentAccomplishments(accountId, heroId));
        return Encode(response.ToArray());
    }

    public string DotaStatsClientRecentAccomplishmentsResponse()
    {
        var accountId = (uint)ReadVarint(1, AccountId);
        var response = new List<byte>();
        WriteVarintField(response, 1, EMsgDotaStatsSuccess);
        WriteBytesField(response, 2, BuildPlayerRecentAccomplishments(accountId));
        return Encode(response.ToArray());
    }

    public string DotaStatsClientHeroRecentAccomplishmentsResponse()
    {
        var accountId = (uint)ReadVarint(1, AccountId);
        var heroId = (uint)ReadVarint(2, 0);
        var response = new List<byte>();
        WriteVarintField(response, 1, EMsgDotaStatsSuccess);
        WriteBytesField(response, 2, BuildHeroRecentAccomplishments(accountId, heroId));
        return Encode(response.ToArray());
    }

    public string DotaStatsSetMatchHistoryAccessResponse()
    {
        return Encode(BuildResult(1));
    }

    private DotaStatsStore StatsStoreOrThrow()
    {
        return StatsStore ?? throw new InvalidOperationException("Dota stats store is not initialized.");
    }

    private DotaStatsProfile GetStatsProfile(uint accountId)
    {
        if (accountId == AccountId)
        {
            return StatsStoreOrThrow().EnsureProfile(SteamId, AccountId, PersonaName);
        }

        return StatsStoreOrThrow().GetProfile(accountId);
    }

    private byte[] BuildDotaStatsProfileCard(uint accountId)
    {
        var profile = GetStatsProfile(accountId);
        var global = StatsStoreOrThrow().GetGlobalStats(accountId);
        var heroes = StatsStoreOrThrow().GetHeroStandings(accountId).ToDictionary(hero => hero.HeroId);
        var response = new List<byte>();
        WriteVarintField(response, 1, accountId);

        foreach (var slotInfo in StatsStoreOrThrow().GetProfileSlots(accountId))
        {
            var slot = new List<byte>();
            WriteVarintField(slot, 1, slotInfo.SlotId);
            switch (slotInfo.SlotType)
            {
                case 1:
                    var statId = (uint)slotInfo.SlotValue;
                    var stat = new List<byte>();
                    WriteVarintField(stat, 1, statId);
                    WriteVarintField(stat, 2, ProfileStatScore(statId, accountId, global));
                    WriteBytesField(slot, 3, stat.ToArray());
                    break;
                case 2:
                    var trophy = StatsStoreOrThrow().GetTrophies(accountId).FirstOrDefault(item => item.TrophyId == (uint)slotInfo.SlotValue);
                    var trophyBody = new List<byte>();
                    WriteVarintField(trophyBody, 1, (uint)slotInfo.SlotValue);
                    WriteVarintField(trophyBody, 2, trophy?.TrophyScore ?? 0);
                    WriteBytesField(slot, 2, trophyBody.ToArray());
                    break;
                case 3:
                    var item = new List<byte>();
                    WriteVarintField(item, 2, slotInfo.SlotValue);
                    WriteBytesField(slot, 4, item.ToArray());
                    break;
                case 4:
                    var heroId = (uint)(slotInfo.SlotValue & 0xFFFF);
                    heroes.TryGetValue(heroId, out var heroStats);
                    var hero = new List<byte>();
                    WriteVarintField(hero, 1, heroId);
                    WriteVarintField(hero, 2, heroStats?.Wins ?? 0);
                    WriteVarintField(hero, 3, heroStats?.Losses ?? 0);
                    WriteBytesField(slot, 5, hero.ToArray());
                    break;
                case 5:
                    var emoticon = new List<byte>();
                    WriteVarintField(emoticon, 1, slotInfo.SlotValue);
                    WriteBytesField(slot, 6, emoticon.ToArray());
                    break;
                case 6:
                    var team = new List<byte>();
                    WriteVarintField(team, 1, slotInfo.SlotValue);
                    WriteBytesField(slot, 7, team.ToArray());
                    break;
            }

            WriteBytesField(response, 3, slot.ToArray());
        }

        WriteVarintField(response, 4, profile.BadgePoints);
        WriteVarintField(response, 5, profile.EventPoints);
        WriteVarintField(response, 6, ActiveEventId);
        WriteVarintField(response, 8, profile.RankTier);
        WriteVarintField(response, 9, profile.LeaderboardRank);
        WriteVarintField(response, 10, 0);
        WriteVarintField(response, 11, 0);
        WriteVarintField(response, 12, profile.RankTierScore);
        WriteVarintField(response, 17, profile.LeaderboardRank);
        WriteVarintField(response, 23, 0);
        WriteVarintField(response, 25, profile.LifetimeGames);
        WriteVarintField(response, 26, profile.Level);
        return response.ToArray();
    }

    private uint ProfileStatScore(uint statId, uint accountId, DotaStatsGlobalStats global)
    {
        return statId switch
        {
            3 => global.GamesWon,
            4 => StatsStoreOrThrow().GetConduct(accountId).CommendCount,
            5 => global.MatchCount,
            6 => StatsStoreOrThrow().GetFirstMatchTime(accountId),
            8 => StatsStoreOrThrow().GetMvpCount(accountId),
            _ => 0
        };
    }

    private byte[] BuildDotaStatsShowcase(uint accountId)
    {
        var global = StatsStoreOrThrow().GetGlobalStats(accountId);
        var conduct = StatsStoreOrThrow().GetConduct(accountId);
        var mvpCount = StatsStoreOrThrow().GetMvpCount(accountId);
        var response = new List<byte>();
        WriteBytesField(response, 1, BuildDotaStatsShowcaseStatItem(1, 3, global.GamesWon, 0, 0));
        WriteBytesField(response, 1, BuildDotaStatsShowcaseStatItem(2, 4, conduct.CommendCount, 280, 0));
        WriteBytesField(response, 1, BuildDotaStatsShowcaseStatItem(3, 8, mvpCount, 560, 0));
        WriteVarintField(response, 4, 0);
        return response.ToArray();
    }

    private static byte[] BuildDotaStatsShowcaseStatItem(uint itemId, uint statId, uint score, uint x, uint y)
    {
        var statData = new List<byte>();
        WriteVarintField(statData, 1, score);

        var stat = new List<byte>();
        WriteBytesField(stat, 1, statData.ToArray());
        WriteVarintField(stat, 2, statId);

        var itemData = new List<byte>();
        WriteBytesField(itemData, 15, stat.ToArray());

        var position = new List<byte>();
        WriteVarintField(position, 1, x);
        WriteVarintField(position, 2, y);
        WriteVarintField(position, 3, 100);
        WriteVarintField(position, 4, 240);
        WriteVarintField(position, 5, 120);

        var item = new List<byte>();
        WriteVarintField(item, 1, itemId);
        WriteBytesField(item, 2, position.ToArray());
        WriteBytesField(item, 3, itemData.ToArray());
        WriteVarintField(item, 4, 0);
        return item.ToArray();
    }

    private byte[] BuildDotaStatsProfileResponse(uint accountId)
    {
        var response = new List<byte>();
        foreach (var heroId in StatsStoreOrThrow().GetFeaturedHeroes(accountId).Take(3))
        {
            var hero = StatsStoreOrThrow().GetHeroStandings(accountId).FirstOrDefault(item => item.HeroId == heroId);
            var item = new List<byte>();
            WriteVarintField(item, 1, heroId);
            WriteVarintField(item, 3, 1);
            WriteVarintField(item, 4, (hero?.Wins ?? 0) * 100);
            WriteBytesField(response, 2, item.ToArray());
        }

        var recent = StatsStoreOrThrow().GetRecentMatches(accountId, 8);
        foreach (var match in recent)
        {
            var row = new List<byte>();
            WriteVarintField(row, 1, match.MatchId);
            WriteVarintField(row, 2, match.StartTime);
            WriteVarintField(row, 3, match.Winner ? 1u : 2u);
            WriteVarintField(row, 4, match.HeroId);
            WriteVarintField(row, 5, match.Winner ? 1u : 0u);
            WriteBytesField(response, 3, row.ToArray());
        }

        foreach (var hero in StatsStoreOrThrow().GetHeroStandings(accountId).Take(5))
        {
            var successful = new List<byte>();
            WriteVarintField(successful, 1, hero.HeroId);
            var total = hero.Wins + hero.Losses;
            WriteFloatField(successful, 2, total == 0 ? 0 : hero.Wins * 100f / total);
            WriteVarintField(successful, 3, hero.BestWinStreak);
            WriteBytesField(response, 4, successful.ToArray());
        }

        var lastMatch = recent.FirstOrDefault();
        if (lastMatch != null)
        {
            WriteBytesField(response, 5, BuildRecentMatchInfo(lastMatch));
        }

        WriteVarintField(response, 6, EMsgDotaStatsSuccess);
        return response.ToArray();
    }

    private byte[] BuildDotaStatsPlayerStatsResponse(uint accountId)
    {
        var stats = StatsStoreOrThrow().GetGlobalStats(accountId);
        var response = new List<byte>();
        WriteVarintField(response, 1, accountId);
        WriteVarintField(response, 3, stats.MatchCount);
        WriteFloatField(response, 4, stats.MediaGpm);
        WriteFloatField(response, 5, stats.MediaXpm);
        WriteFloatField(response, 6, stats.MediaLastHits);
        WriteVarintField(response, 7, stats.Rampages);
        WriteVarintField(response, 8, stats.TripleKills);
        WriteVarintField(response, 9, stats.FirstBloodsReceived);
        WriteVarintField(response, 10, stats.FirstBloodsGiven);
        WriteVarintField(response, 11, stats.CouriersKilled);
        WriteVarintField(response, 12, stats.AegisesSnatched);
        WriteVarintField(response, 13, stats.CheesesEaten);
        WriteVarintField(response, 14, stats.CreepsStacked);
        WriteFloatField(response, 15, 0.5f + Math.Min(1f, (float)stats.AvgFightScore) / 2f);
        WriteFloatField(response, 16, 0.5f + Math.Min(1f, (float)stats.AvgFarmScore / 500f) / 2f);
        WriteFloatField(response, 17, 0.5f + Math.Min(1f, (float)stats.AvgSupportScore / 10000f) / 2f);
        WriteFloatField(response, 18, 0.5f + Math.Min(1f, (float)stats.AvgPushScore / 2500f) / 2f);
        WriteFloatField(response, 19, 0.5f + Math.Min(1f, (float)stats.PlayedHeroCount / 123f) / 2f);
        WriteFloatField(response, 20, (float)stats.MeanNetworth);
        WriteFloatField(response, 21, (float)stats.MeanDamage);
        WriteFloatField(response, 22, (float)stats.MeanHeals);
        WriteVarintField(response, 23, stats.RapiersPurchased);
        return response.ToArray();
    }

    private byte[] BuildDotaStatsMatch(DotaStatsMatch match)
    {
        var response = new List<byte>();
        WriteVarintField(response, 3, match.Duration);
        WriteFixed32Field(response, 4, match.StartTime);
        foreach (var player in match.Players)
        {
            WriteBytesField(response, 5, BuildDotaStatsMatchPlayer(player));
        }

        WriteVarintField(response, 6, match.MatchId);
        WriteVarintField(response, 10, match.Cluster);
        WriteVarintField(response, 12, match.FirstBloodTime);
        WriteVarintField(response, 16, match.LobbyType);
        WriteVarintField(response, 17, (uint)match.Players.Count(player => player.SteamId != 0));
        WriteVarintField(response, 31, match.GameMode == 0 ? 1u : match.GameMode);
        WriteVarintField(response, 34, 1);
        WriteVarintField(response, 44, 1);
        WriteVarintField(response, 46, match.MatchFlags);
        WriteVarintField(response, 48, match.RadiantScore);
        WriteVarintField(response, 49, match.DireScore);
        WriteVarintField(response, 50, match.GoodGuysWin ? MatchOutcomeRadVictory : MatchOutcomeDireVictory);
        return response.ToArray();
    }

    private byte[] BuildDotaStatsMatchPlayer(DotaStatsMatchPlayer player)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, player.AccountId);
        WriteVarintField(response, 2, player.PlayerSlot);
        WriteVarintField(response, 3, player.HeroId);
        var itemFields = new[] { 4, 5, 6, 7, 8, 9, 59, 60, 61, 76, 83 };
        for (var i = 0; i < itemFields.Length; i++)
        {
            WriteVarintField(response, itemFields[i], i < player.Items.Count ? player.Items[i] : 0);
        }

        WriteFloatField(response, 10, 0);
        WriteFloatField(response, 11, 0);
        WriteVarintField(response, 12, 0);
        WriteVarintField(response, 13, 0);
        WriteVarintField(response, 14, player.Kills);
        WriteVarintField(response, 15, player.Deaths);
        WriteVarintField(response, 16, player.Assists);
        WriteVarintField(response, 17, player.LeaverStatus);
        WriteVarintField(response, 18, player.Gold);
        WriteVarintField(response, 19, player.LastHits);
        WriteVarintField(response, 20, player.Denies);
        WriteVarintField(response, 21, player.Gpm);
        WriteVarintField(response, 22, player.Xpm);
        WriteVarintField(response, 23, player.GoldSpent);
        WriteVarintField(response, 24, player.HeroDamage);
        WriteVarintField(response, 25, player.TowerDamage);
        WriteVarintField(response, 26, player.HeroHealing);
        WriteVarintField(response, 27, player.Level);
        WriteStringField(response, 29, player.PersonaName);
        WriteVarintField(response, 42, player.ClaimedFarmGold);
        WriteVarintField(response, 43, player.SupportGold);
        WriteVarintField(response, 51, 1);
        WriteVarintField(response, 52, (uint)Math.Round(player.NetWorth));
        WriteVarintField(response, 77, player.BountyRunes);
        WriteVarintField(response, 78, player.OutpostsCaptured);
        WriteVarintField(response, 80, player.GoodGuys ? 0u : 1u);
        WriteVarintField(response, 81, player.PlayerSlot);
        WriteVarintField(response, 82, player.SelectedFacet);
        return response.ToArray();
    }

    private byte[] BuildRecentMatchInfo(DotaStatsMatchPlayer match)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, match.MatchId);
        WriteVarintField(response, 2, match.GameMode);
        WriteVarintField(response, 3, match.Kills);
        WriteVarintField(response, 4, match.Deaths);
        WriteVarintField(response, 5, match.Assists);
        WriteVarintField(response, 6, match.Duration);
        WriteVarintField(response, 7, match.PlayerSlot);
        WriteVarintField(response, 8, match.GoodGuysWin ? MatchOutcomeRadVictory : MatchOutcomeDireVictory);
        WriteVarintField(response, 9, match.StartTime);
        WriteVarintField(response, 10, match.LobbyType);
        WriteVarintField(response, 11, match.GoodGuys ? 0u : 1u);
        return response.ToArray();
    }

    private byte[] BuildRecentAccomplishments(uint accountId, uint heroId)
    {
        var response = new List<byte>();
        WriteBytesField(response, 1, BuildPlayerRecentAccomplishments(accountId));
        WriteBytesField(response, 2, BuildHeroRecentAccomplishments(accountId, heroId));
        return response.ToArray();
    }

    private byte[] BuildPlayerRecentAccomplishments(uint accountId)
    {
        var stats = StatsStoreOrThrow().GetGlobalStats(accountId);
        var recent = StatsStoreOrThrow().GetRecentMatches(accountId, 1).FirstOrDefault();
        var conduct = StatsStoreOrThrow().GetConduct(accountId);
        var matchCount = StatsStoreOrThrow().GetMatchCount(accountId);
        var response = new List<byte>();
        WriteBytesField(response, 1, BuildRecentOutcomes(0, matchCount));
        WriteBytesField(response, 2, BuildRecentRecord(stats.GamesWon, stats.GamesLost));
        WriteVarintField(response, 3, 0);
        WriteVarintField(response, 4, 0);
        WriteBytesField(response, 5, BuildRecentCommends(conduct.CommendCount, matchCount));
        WriteVarintField(response, 6, StatsStoreOrThrow().GetFirstMatchTime(accountId));
        if (recent != null)
        {
            WriteBytesField(response, 7, BuildPlayerRecentMatchInfo(recent));
        }

        WriteBytesField(response, 8, BuildRecentOutcomes(StatsStoreOrThrow().GetMvpCount(accountId), matchCount));
        return response.ToArray();
    }

    private byte[] BuildHeroRecentAccomplishments(uint accountId, uint heroId)
    {
        var heroStats = StatsStoreOrThrow().GetHeroStandings(accountId).FirstOrDefault(hero => hero.HeroId == heroId);
        var recent = StatsStoreOrThrow().GetRecentMatches(accountId, 1, heroId).FirstOrDefault();
        var matchCount = StatsStoreOrThrow().GetMatchCount(accountId, heroId);
        var response = new List<byte>();
        WriteBytesField(response, 1, BuildRecentOutcomes(0, matchCount));
        WriteBytesField(response, 2, BuildRecentRecord(heroStats?.Wins ?? 0, heroStats?.Losses ?? 0));
        if (recent != null)
        {
            WriteBytesField(response, 3, BuildPlayerRecentMatchInfo(recent));
        }

        return response.ToArray();
    }

    private static byte[] BuildPlayerRecentMatchInfo(DotaStatsMatchPlayer match)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, match.MatchId);
        WriteVarintField(response, 2, match.StartTime);
        WriteVarintField(response, 3, match.Duration);
        WriteVarintField(response, 4, match.Winner ? 1u : 0u);
        WriteVarintField(response, 5, match.HeroId);
        WriteVarintField(response, 6, match.Kills);
        WriteVarintField(response, 7, match.Deaths);
        WriteVarintField(response, 8, match.Assists);
        return response.ToArray();
    }

    private static byte[] BuildRecentRecord(uint wins, uint losses)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, wins);
        WriteVarintField(response, 2, losses);
        return response.ToArray();
    }

    private static byte[] BuildRecentOutcomes(uint outcomes, uint matchCount)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, outcomes);
        WriteVarintField(response, 2, matchCount);
        return response.ToArray();
    }

    private static byte[] BuildRecentCommends(uint commends, uint matchCount)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, commends);
        WriteVarintField(response, 2, matchCount);
        return response.ToArray();
    }

    private static byte[] BuildDotaStatsSocialComment(DotaStatsComment comment)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, comment.AccountId);
        WriteStringField(response, 2, comment.PersonaName);
        WriteVarintField(response, 3, comment.Timestamp);
        WriteStringField(response, 4, comment.Comment);
        return response.ToArray();
    }

    private static byte[] BuildTimedAverages(DotaStatsHeroStats? hero)
    {
        var response = new List<byte>();
        WriteFloatField(response, 2, (float)(hero?.AvgKills ?? 0));
        WriteFloatField(response, 3, (float)(hero?.AvgDeaths ?? 0));
        WriteFloatField(response, 4, (float)(hero?.AvgAssists ?? 0));
        WriteFloatField(response, 5, hero?.NetworthPeak ?? 0);
        WriteFloatField(response, 6, (float)(hero?.AvgLastHits ?? 0));
        WriteFloatField(response, 7, (float)(hero?.AvgDenies ?? 0));
        return response.ToArray();
    }

    private static byte[] BuildTimedStdDevs()
    {
        var response = new List<byte>();
        for (var field = 2; field <= 14; field++)
        {
            WriteFloatField(response, field, 1f);
        }

        return response.ToArray();
    }

    private byte[] BuildDotaStatsGameMatchSignOutResponse(DotaStatsMatch match)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, match.MatchId);
        WriteFixed32Field(response, 2, 0);
        WriteVarintField(response, 5, 0);
        WriteFixed32Field(response, 7, 1);

        var heroStatsByAccount = new Dictionary<uint, IReadOnlyDictionary<uint, DotaStatsHeroStats>>();
        foreach (var player in match.Players)
        {
            if (!heroStatsByAccount.TryGetValue(player.AccountId, out var heroStats))
            {
                heroStats = StatsStoreOrThrow()
                    .GetHeroStandings(player.AccountId)
                    .ToDictionary(hero => hero.HeroId);
                heroStatsByAccount[player.AccountId] = heroStats;
            }

            heroStats.TryGetValue(player.HeroId, out var standing);
            var metadata = new List<byte>();
            WriteVarintField(metadata, 1, player.HeroId);
            WriteVarintField(metadata, 2, ToX16(standing?.AvgKills ?? player.Kills));
            WriteVarintField(metadata, 3, ToX16(standing?.AvgDeaths ?? player.Deaths));
            WriteVarintField(metadata, 4, ToX16(standing?.AvgAssists ?? player.Assists));
            WriteVarintField(metadata, 5, ToX16(standing?.AvgGpm ?? player.Gpm));
            WriteVarintField(metadata, 6, ToX16(standing?.AvgXpm ?? player.Xpm));
            WriteVarintField(metadata, 7, ToX16(standing?.BestKills ?? player.Kills));
            WriteVarintField(metadata, 8, ToX16(standing?.BestAssists ?? player.Assists));
            WriteVarintField(metadata, 9, ToX16(standing?.BestGpm ?? player.Gpm));
            WriteVarintField(metadata, 10, ToX16(standing?.BestXpm ?? player.Xpm));
            WriteVarintField(metadata, 11, standing?.WinStreak ?? (player.Winner ? 1u : 0u));
            WriteVarintField(metadata, 12, standing?.BestWinStreak ?? (player.Winner ? 1u : 0u));
            WriteVarintField(metadata, 13, (standing?.Wins ?? (player.Winner ? 1u : 0u)) + (standing?.Losses ?? (player.Winner ? 0u : 1u)));
            WriteBytesField(response, 9, metadata.ToArray());
        }

        return response.ToArray();
    }

    private static uint ToX16(double value)
    {
        if (value <= 0)
        {
            return 0;
        }

        return (uint)Math.Round(value * 16d);
    }

    private DotaStatsMatch ParseDotaStatsMatchSignOut(ulong fallbackMatchId, uint gameMode, uint lobbyType, uint startTime, ulong serverSteamId)
    {
        TryReadVarintField(_requestBody, 1, out var matchId);
        if (matchId == 0)
        {
            matchId = fallbackMatchId != 0 ? fallbackMatchId : GenerateSteamObjectId();
        }

        TryReadVarintField(_requestBody, 2, out var duration);
        TryReadVarintField(_requestBody, 3, out var goodGuysWinRaw);
        TryReadFixed32Field(_requestBody, 4, out var date);
        TryReadVarintField(_requestBody, 10, out var cluster);
        TryReadVarintField(_requestBody, 12, out var firstBloodTime);
        TryReadVarintField(_requestBody, 38, out var matchFlags);
        var hasWinningTeam = TryReadVarintField(_requestBody, 56, out var winningTeam);
        var teamScores = ReadVarintFields(_requestBody, 39);

        var match = new DotaStatsMatch
        {
            MatchId = matchId,
            OwnerSteamId = SteamId,
            ServerSteamId = serverSteamId != 0 ? serverSteamId : SteamId,
            StartTime = startTime != 0 ? startTime : date != 0 ? date : (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Duration = (uint)duration,
            GameMode = gameMode == 0 ? 1 : gameMode,
            LobbyType = lobbyType,
            GoodGuysWin = hasWinningTeam ? winningTeam == 0 : goodGuysWinRaw != 0,
            MatchFlags = (uint)matchFlags,
            RadiantScore = teamScores.Count > 0 ? (uint)teamScores[0] : 0,
            DireScore = teamScores.Count > 1 ? (uint)teamScores[1] : 0,
            Cluster = (uint)cluster,
            FirstBloodTime = (uint)firstBloodTime,
            RawSignoutBase64 = Encode(_requestBody)
        };

        var extras = ParseSignoutExtras(_requestBody);
        var teams = ReadLengthDelimitedFields(_requestBody, 6);
        for (var teamIndex = 0; teamIndex < teams.Count; teamIndex++)
        {
            var playerBodies = ReadLengthDelimitedFields(teams[teamIndex], 1);
            for (var playerIndex = 0; playerIndex < playerBodies.Count; playerIndex++)
            {
                var player = ParseSignoutPlayer(playerBodies[playerIndex], (uint)teamIndex, (uint)playerIndex, match);
                if (player.AccountId != 0 && extras.TryGetValue(player.AccountId, out var extra))
                {
                    ApplySignoutExtra(player, extra);
                }

                if (player.SteamId != 0)
                {
                    match.Players.Add(player);
                }
            }
        }

        return match;
    }

    private DotaStatsMatchPlayer ParseSignoutPlayer(byte[] body, uint teamIndex, uint playerIndex, DotaStatsMatch match)
    {
        var hasSteam = TryReadFixed64Field(body, 1, out var steamId) || TryReadVarintField(body, 1, out steamId);
        var accountId = hasSteam ? SteamIdToAccountId(steamId) : 0;
        TryReadVarintField(body, 3, out var heroId);
        TryReadVarintField(body, 5, out var gold);
        TryReadVarintField(body, 6, out var kills);
        TryReadVarintField(body, 7, out var deaths);
        TryReadVarintField(body, 8, out var assists);
        TryReadVarintField(body, 9, out var leaverStatus);
        TryReadVarintField(body, 10, out var lastHits);
        TryReadVarintField(body, 11, out var denies);
        TryReadVarintField(body, 12, out var gpm);
        TryReadVarintField(body, 13, out var xpm);
        TryReadVarintField(body, 14, out var goldSpent);
        TryReadVarintField(body, 15, out var level);
        TryReadVarintField(body, 27, out var claimedFarmGold);
        TryReadVarintField(body, 28, out var supportGold);
        TryReadVarintField(body, 34, out var netWorth);
        TryReadVarintField(body, 37, out var heroDamage);
        TryReadVarintField(body, 38, out var towerDamage);
        TryReadVarintField(body, 39, out var heroHealing);
        TryReadVarintField(body, 57, out var bountyRunes);
        TryReadVarintField(body, 58, out var outpostsCaptured);
        TryReadVarintField(body, 62, out var playerSlot);
        TryReadVarintField(body, 75, out var teamNumber);
        TryReadVarintField(body, 82, out var selectedFacet);

        var goodGuys = teamIndex == 0;
        var player = new DotaStatsMatchPlayer
        {
            MatchId = match.MatchId,
            SteamId = steamId,
            AccountId = accountId,
            PersonaName = accountId == AccountId ? PersonaName : $"User{accountId}",
            Team = teamNumber != 0 ? (uint)teamNumber : teamIndex,
            PlayerSlot = playerSlot != 0 ? (uint)playerSlot : playerIndex + (teamIndex == 0 ? 0u : 128u),
            HeroId = (uint)heroId,
            Kills = (uint)kills,
            Deaths = (uint)deaths,
            Assists = (uint)assists,
            Winner = (match.GoodGuysWin && goodGuys) || (!match.GoodGuysWin && !goodGuys),
            GoodGuys = goodGuys,
            Gold = (uint)gold,
            GoldSpent = (uint)goldSpent,
            Gpm = (uint)gpm,
            Xpm = (uint)xpm,
            LastHits = (uint)lastHits,
            Denies = (uint)denies,
            HeroDamage = (uint)heroDamage,
            TowerDamage = (uint)towerDamage,
            HeroHealing = (uint)heroHealing,
            Level = (uint)level,
            NetWorth = netWorth,
            SupportGold = (uint)supportGold,
            ClaimedFarmGold = (uint)claimedFarmGold,
            BountyRunes = (uint)bountyRunes,
            OutpostsCaptured = (uint)outpostsCaptured,
            SelectedFacet = (uint)selectedFacet,
            LeaverStatus = (uint)leaverStatus,
            StartTime = match.StartTime,
            Duration = match.Duration,
            GameMode = match.GameMode,
            LobbyType = match.LobbyType,
            GoodGuysWin = match.GoodGuysWin,
            MatchFlags = match.MatchFlags,
            RadiantScore = match.RadiantScore,
            DireScore = match.DireScore,
            Cluster = match.Cluster,
            FirstBloodTime = match.FirstBloodTime
        };
        player.Items.AddRange(ReadVarintFields(body, 4).Select(value => (uint)value));
        player.Talents.AddRange(ReadVarintFields(body, 41).Select(value => (uint)value));
        return player;
    }

    private Dictionary<uint, SignoutExtra> ParseSignoutExtras(byte[] body)
    {
        var extras = new Dictionary<uint, SignoutExtra>();
        foreach (var extraBody in ReadLengthDelimitedFields(body, 20))
        {
            TryReadVarintField(extraBody, 1, out var id);
            if (id != 8013 || !TryReadLengthDelimitedField(extraBody, 2, out var contents))
            {
                continue;
            }

            var extra = new SignoutExtra
            {
                AccountId = (uint)ReadVarintField(contents, 1),
                HeroId = (uint)ReadVarintField(contents, 4),
                Rampages = (uint)ReadVarintField(contents, 5),
                TripleKills = (uint)ReadVarintField(contents, 6),
                FirstBloodClaimed = (uint)ReadVarintField(contents, 7),
                FirstBloodGiven = (uint)ReadVarintField(contents, 8),
                CouriersKilled = (uint)ReadVarintField(contents, 9),
                AegisesSnatched = (uint)ReadVarintField(contents, 10),
                CheesesEaten = (uint)ReadVarintField(contents, 11),
                CreepsStacked = (uint)ReadVarintField(contents, 12),
                FightScore = ReadFloatField(contents, 13),
                FarmScore = ReadFloatField(contents, 14),
                SupportScore = ReadFloatField(contents, 15),
                PushScore = ReadFloatField(contents, 16),
                NetWorth = ReadFloatField(contents, 24),
                Damage = ReadFloatField(contents, 25),
                Heals = ReadFloatField(contents, 26),
                RapiersPurchased = (uint)ReadVarintField(contents, 27)
            };
            if (extra.AccountId != 0)
            {
                extras[extra.AccountId] = extra;
            }
        }

        return extras;
    }

    private static void ApplySignoutExtra(DotaStatsMatchPlayer player, SignoutExtra extra)
    {
        player.Rampages = extra.Rampages;
        player.TripleKills = extra.TripleKills;
        player.FirstBloodClaimed = extra.FirstBloodClaimed;
        player.FirstBloodGiven = extra.FirstBloodGiven;
        player.CouriersKilled = extra.CouriersKilled;
        player.AegisesSnatched = extra.AegisesSnatched;
        player.CheesesEaten = extra.CheesesEaten;
        player.CreepsStacked = extra.CreepsStacked;
        player.FightScore = extra.FightScore;
        player.FarmScore = extra.FarmScore;
        player.SupportScore = extra.SupportScore;
        player.PushScore = extra.PushScore;
        player.NetWorth = extra.NetWorth == 0 ? player.NetWorth : extra.NetWorth;
        player.Damage = extra.Damage;
        player.Heals = extra.Heals;
        player.RapiersPurchased = extra.RapiersPurchased;
    }

    private static ulong ReadVarintField(byte[] body, int fieldNumber)
    {
        return TryReadVarintField(body, fieldNumber, out var value) ? value : 0;
    }

    private static float ReadFloatField(byte[] body, int fieldNumber)
    {
        return TryReadFloatField(body, fieldNumber, out var value) ? value : 0f;
    }

    private static bool TryReadFixed32Field(byte[] source, int expectedFieldNumber, out uint value)
    {
        value = 0;
        var index = 0;
        while (source != null && index < source.Length)
        {
            if (!TryReadVarint(source, ref index, out var key))
            {
                return false;
            }

            var fieldNumber = (int)(key >> 3);
            var wireType = (int)(key & 7);
            if (wireType == 5)
            {
                if (index + 4 > source.Length)
                {
                    return false;
                }

                if (fieldNumber == expectedFieldNumber)
                {
                    value = BitConverter.ToUInt32(source, index);
                    return true;
                }

                index += 4;
            }
            else if (!SkipField(source, ref index, wireType))
            {
                return false;
            }
        }

        return false;
    }

    private static bool TryReadFloatField(byte[] source, int expectedFieldNumber, out float value)
    {
        value = 0;
        var index = 0;
        while (source != null && index < source.Length)
        {
            if (!TryReadVarint(source, ref index, out var key))
            {
                return false;
            }

            var fieldNumber = (int)(key >> 3);
            var wireType = (int)(key & 7);
            if (wireType == 5)
            {
                if (index + 4 > source.Length)
                {
                    return false;
                }

                if (fieldNumber == expectedFieldNumber)
                {
                    value = BitConverter.ToSingle(source, index);
                    return true;
                }

                index += 4;
            }
            else if (!SkipField(source, ref index, wireType))
            {
                return false;
            }
        }

        return false;
    }

    private sealed class SignoutExtra
    {
        public uint AccountId { get; init; }
        public uint HeroId { get; init; }
        public uint Rampages { get; init; }
        public uint TripleKills { get; init; }
        public uint FirstBloodClaimed { get; init; }
        public uint FirstBloodGiven { get; init; }
        public uint CouriersKilled { get; init; }
        public uint AegisesSnatched { get; init; }
        public uint CheesesEaten { get; init; }
        public uint CreepsStacked { get; init; }
        public double FightScore { get; init; }
        public double FarmScore { get; init; }
        public double SupportScore { get; init; }
        public double PushScore { get; init; }
        public double NetWorth { get; init; }
        public double Damage { get; init; }
        public double Heals { get; init; }
        public uint RapiersPurchased { get; init; }
    }
}
