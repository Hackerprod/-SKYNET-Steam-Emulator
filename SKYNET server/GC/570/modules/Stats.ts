import {
    DotaHeroRecentAccomplishments,
    DotaHeroStanding,
    DotaMatchPlayer,
    DotaPlayerMatchRecord,
    DotaPlayerRecentAccomplishments,
    DotaPlayerRecentCommends,
    DotaPlayerRecentMatchInfo,
    DotaRecentMatchOutcomes,
    DotaShowcaseStats,
    DotaSignOutMvpPlayer,
    HandlerContext,
    RawMessageContext,
    gc
} from "../framework/gc";
import {
    CMsgClientToGCHasPlayerVotedForMVP,
    CMsgClientToGCHasPlayerVotedForMVPResponse,
    CMsgClientToGCLookupAccountName,
    CMsgClientToGCLookupAccountNameResponse,
    CMsgClientToGCMVPVoteTimeout,
    CMsgClientToGCMVPVoteTimeoutResponse,
    CMsgClientToGCPlayerStatsRequest,
    CMsgClientToGCRankRequest,
    CMsgClientToGCRequestPlayerHeroRecentAccomplishments,
    CMsgClientToGCRequestPlayerHeroRecentAccomplishmentsResponse,
    CMsgClientToGCRequestPlayerRecentAccomplishments,
    CMsgClientToGCRequestPlayerRecentAccomplishmentsResponse,
    CMsgClientToGCRerollPlayerChallenge,
    CMsgClientToGCShowcaseGetUserData,
    CMsgClientToGCShowcaseGetUserDataResponse,
    CMsgClientToGCTeammateStatsRequest,
    CMsgClientToGCTeammateStatsResponse,
    CMsgClientToGCVoteForMVP,
    CMsgClientToGCVoteForMVPResponse,
    CMsgDOTAGetEventPoints,
    CMsgDOTAGetEventPointsResponse,
    CMsgDOTAGetPlayerMatchHistory,
    CMsgDOTAGetPlayerMatchHistoryResponse,
    CMsgDOTAGetPlayerMatchHistoryResponse_Match,
    CMsgDOTAMatchmakingStatsResponse,
    CMsgDOTASDOHeroStatsHistory,
    CMsgDOTASubmitLobbyMVPVote,
    CMsgDOTASubmitLobbyMVPVoteResponse,
    CMsgGCGetHeroStandings,
    CMsgGCGetHeroStandingsResponse,
    CMsgGCGetHeroStandingsResponse_Hero,
    CMsgGCGetHeroStatsHistory,
    CMsgGCGetHeroStatsHistoryResponse,
    CMsgGCToClientPlayerStatsResponse,
    CMsgGCToClientRankResponse,
    CMsgGCRerollPlayerChallengeResponse,
    CMsgHeroGlobalDataRequest,
    CMsgHeroGlobalDataResponse,
    CMsgPlayerHeroRecentAccomplishments,
    CMsgPlayerMatchRecord,
    CMsgPlayerRecentAccomplishments,
    CMsgPlayerRecentCommends,
    CMsgPlayerRecentMatchInfo,
    CMsgPlayerRecentMatchOutcomes,
    CMsgShowcase,
    CMsgShowcaseItem,
    Msg,
    Proto,
    Routes
} from "../generated/dota";

const STATS_SUCCESS = 1;
const REROLL_SUCCESS = 0;
const REROLL_SERVER_ERROR = 4;

export function registerStats(): void {
    const stats = new Stats();
    stats.register();
}

export class Stats {
    register(): void {
        gc.on(Routes.LookupAccountName, (ctx) => this.lookupAccountName(ctx));
        gc.on(Routes.GetEventPoints, (ctx) => this.getEventPoints(ctx));
        gc.onMessage(Msg.GCMatchmakingStatsRequest, (ctx) => this.matchmakingStats(ctx));
        gc.on(Routes.GetHeroStandings, (ctx) => this.getHeroStandings(ctx));
        gc.on(Routes.GetHeroStatsHistory, (ctx) => this.getHeroStatsHistory(ctx));
        gc.on(Routes.GetPlayerMatchHistory, (ctx) => this.getPlayerMatchHistory(ctx));
        gc.on(Routes.PlayerStats, (ctx) => this.playerStats(ctx));
        gc.on(Routes.HeroGlobalData, (ctx) => this.heroGlobalData(ctx));
        gc.on(Routes.TeammateStats, (ctx) => this.teammateStats(ctx));
        gc.on(Routes.RankRequest, (ctx) => this.rankRequest(ctx));
        gc.on(Routes.ShowcaseGetUserData, (ctx) => this.showcaseGetUserData(ctx));
        gc.on(Routes.RequestPlayerRecentAccomplishments, (ctx) => this.clientRecentAccomplishments(ctx));
        gc.on(Routes.RequestPlayerHeroRecentAccomplishments, (ctx) => this.clientHeroRecentAccomplishments(ctx));
        gc.on(Routes.HasPlayerVotedForMvp, (ctx) => this.hasPlayerVotedForMvp(ctx));
        gc.on(Routes.VoteForMvp, (ctx) => this.voteForMvp(ctx));
        gc.on(Routes.MvpVoteTimeout, (ctx) => this.mvpVoteTimeout(ctx));
        gc.on(Routes.SubmitLobbyMvpVote, (ctx) => this.submitLobbyMvpVote(ctx));
        gc.onMessage(Msg.SignOutMVPStats, (ctx) => this.signOutMvpStats(ctx));
        gc.on(Routes.RerollPlayerChallenge, (ctx) => this.rerollPlayerChallenge(ctx));
    }

    lookupAccountName(
        ctx: HandlerContext<CMsgClientToGCLookupAccountName, CMsgClientToGCLookupAccountNameResponse>
    ): boolean {
        ctx.reply(ctx.services.stats.lookupAccountName(ctx.request.accountId ?? ctx.accountId));
        return true;
    }

    getEventPoints(ctx: HandlerContext<CMsgDOTAGetEventPoints, CMsgDOTAGetEventPointsResponse>): boolean {
        const points = ctx.services.stats.getEventPoints(
            ctx.request.accountId ?? ctx.accountId,
            ctx.request.eventId ?? 0
        );
        ctx.reply({
            totalPoints: points.totalPoints,
            totalPremiumPoints: points.totalPremiumPoints,
            eventId: points.eventId,
            points: points.points,
            premiumPoints: points.premiumPoints,
            completedActions: [],
            accountId: points.accountId,
            owned: points.owned,
            auditAction: points.auditAction,
            activeSeasonId: points.activeSeasonId
        });
        return true;
    }

    matchmakingStats(ctx: RawMessageContext): boolean {
        ctx.reply<CMsgDOTAMatchmakingStatsResponse>(
            Msg.GCMatchmakingStatsResponse,
            Proto.CMsgDOTAMatchmakingStatsResponse,
            {
                matchgroupsVersion: 0,
                legacySearchingPlayersByGroupSource2: [],
                matchGroups: []
            }
        );
        return true;
    }

    getHeroStandings(ctx: HandlerContext<CMsgGCGetHeroStandings, CMsgGCGetHeroStandingsResponse>): boolean {
        ctx.reply({
            standings: mapHeroStandings(ctx.services.stats.getHeroStandings(ctx.accountId))
        });
        return true;
    }

    getHeroStatsHistory(ctx: HandlerContext<CMsgGCGetHeroStatsHistory, CMsgGCGetHeroStatsHistoryResponse>): boolean {
        const heroId = ctx.request.heroId ?? 0;
        ctx.reply({
            heroId,
            records: mapHeroStatsHistory(ctx.services.stats.getHeroStatsHistory(ctx.accountId, heroId)),
            result: STATS_SUCCESS
        });
        return true;
    }

    getPlayerMatchHistory(
        ctx: HandlerContext<CMsgDOTAGetPlayerMatchHistory, CMsgDOTAGetPlayerMatchHistoryResponse>
    ): boolean {
        const accountId = requestedAccountId(ctx.request.accountId, ctx.accountId);
        const matches = ctx.services.stats.getMatchHistory(
            accountId,
            ctx.request.startAtMatchId ?? 0n,
            ctx.request.matchesRequested ?? 20,
            ctx.request.heroId ?? 0,
            ctx.request.includePracticeMatches ?? false
        );
        ctx.reply({
            matches: mapMatchHistory(matches),
            requestId: ctx.request.requestId ?? 0
        });
        return true;
    }

    retrieveMatchVote(): boolean {
        return false;
    }

    playerStats(ctx: HandlerContext<CMsgClientToGCPlayerStatsRequest, CMsgGCToClientPlayerStatsResponse>): boolean {
        ctx.reply(ctx.services.stats.getPlayerStats(ctx.request.accountId ?? ctx.accountId));
        return true;
    }

    heroTimedStats(): boolean {
        return false;
    }

    heroGlobalData(ctx: HandlerContext<CMsgHeroGlobalDataRequest, CMsgHeroGlobalDataResponse>): boolean {
        const data = ctx.services.stats.getHeroGlobalData(ctx.accountId, ctx.request.heroId ?? 0);
        ctx.reply({
            heroId: data.heroId,
            heroDataPerChunk: data.heroDataPerChunk.map((chunk) => ({
                rankChunk: chunk.rankChunk,
                heroAverages: {
                    lastRun: chunk.heroAverages.lastRun,
                    avgGoldPerMin: chunk.heroAverages.avgGoldPerMin,
                    avgXpPerMin: chunk.heroAverages.avgXpPerMin,
                    avgKills: chunk.heroAverages.avgKills,
                    avgDeaths: chunk.heroAverages.avgDeaths,
                    avgAssists: chunk.heroAverages.avgAssists,
                    avgLastHits: chunk.heroAverages.avgLastHits,
                    avgDenies: chunk.heroAverages.avgDenies,
                    avgNetWorth: chunk.heroAverages.avgNetWorth
                }
            }))
        });
        return true;
    }

    teammateStats(
        ctx: HandlerContext<CMsgClientToGCTeammateStatsRequest, CMsgClientToGCTeammateStatsResponse>
    ): boolean {
        ctx.reply({
            success: true,
            teammateStats: ctx.services.stats.getTeammateStats(ctx.accountId)
        });
        return true;
    }

    rankRequest(ctx: HandlerContext<CMsgClientToGCRankRequest, CMsgGCToClientRankResponse>): boolean {
        ctx.reply(ctx.services.stats.getRank(ctx.accountId));
        return true;
    }

    showcaseGetUserData(
        ctx: HandlerContext<CMsgClientToGCShowcaseGetUserData, CMsgClientToGCShowcaseGetUserDataResponse>
    ): boolean {
        const accountId = requestedAccountId(ctx.request.accountId, ctx.accountId);
        ctx.reply({
            response: STATS_SUCCESS,
            showcase: mapShowcase(ctx.services.stats.getShowcaseStats(accountId))
        });
        return true;
    }

    clientRecentAccomplishments(
        ctx: HandlerContext<
            CMsgClientToGCRequestPlayerRecentAccomplishments,
            CMsgClientToGCRequestPlayerRecentAccomplishmentsResponse
        >
    ): boolean {
        const accountId = requestedAccountId(ctx.request.accountId, ctx.accountId);
        ctx.reply({
            result: STATS_SUCCESS,
            playerAccomplishments: mapPlayerRecentAccomplishments(
                ctx.services.stats.getRecentAccomplishments(accountId)
            )
        });
        return true;
    }

    clientHeroRecentAccomplishments(
        ctx: HandlerContext<
            CMsgClientToGCRequestPlayerHeroRecentAccomplishments,
            CMsgClientToGCRequestPlayerHeroRecentAccomplishmentsResponse
        >
    ): boolean {
        const accountId = requestedAccountId(ctx.request.accountId, ctx.accountId);
        ctx.reply({
            result: STATS_SUCCESS,
            heroAccomplishments: mapHeroRecentAccomplishments(
                ctx.services.stats.getHeroRecentAccomplishments(accountId, ctx.request.heroId ?? 0)
            )
        });
        return true;
    }

    hasPlayerVotedForMvp(
        ctx: HandlerContext<CMsgClientToGCHasPlayerVotedForMVP, CMsgClientToGCHasPlayerVotedForMVPResponse>
    ): boolean {
        ctx.reply({
            result: ctx.services.stats.hasMvpVote(ctx.request.matchId ?? 0n)
        });
        return true;
    }

    voteForMvp(ctx: HandlerContext<CMsgClientToGCVoteForMVP, CMsgClientToGCVoteForMVPResponse>): boolean {
        ctx.reply({
            result: ctx.services.stats.voteForMvp(ctx.request.matchId ?? 0n, ctx.request.accountId ?? 0)
        });
        return true;
    }

    mvpVoteTimeout(ctx: HandlerContext<CMsgClientToGCMVPVoteTimeout, CMsgClientToGCMVPVoteTimeoutResponse>): boolean {
        ctx.reply({
            result: ctx.services.stats.finalizeMvpVote(ctx.request.matchId ?? 0n)
        });
        return true;
    }

    submitLobbyMvpVote(ctx: HandlerContext<CMsgDOTASubmitLobbyMVPVote, CMsgDOTASubmitLobbyMVPVoteResponse>): boolean {
        const result = ctx.services.stats.submitLobbyMvpVote(ctx.request.targetAccountId ?? 0);
        ctx.reply({
            targetAccountId: result.targetAccountId,
            eresult: result.result
        });
        return true;
    }

    signOutMvpStats(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgSignOutMVPStats);
        const players = request.players ?? [];
        const winners: DotaSignOutMvpPlayer[] = [];
        for (let i = 0; i < players.length; i++) {
            const player = players[i];
            winners.push({
                accountId: player.accountId ?? 0,
                rank: player.rank ?? 0
            });
        }

        return ctx.services.stats.recordSignOutMvpStats(request.matchId ?? 0n, winners);
    }

    rerollPlayerChallenge(
        ctx: HandlerContext<CMsgClientToGCRerollPlayerChallenge, CMsgGCRerollPlayerChallengeResponse>
    ): boolean {
        ctx.reply({
            result: ctx.services.stats.rerollPlayerChallenge() ? REROLL_SUCCESS : REROLL_SERVER_ERROR
        });
        return true;
    }
}

function requestedAccountId(value: number | undefined, fallback: number): number {
    const accountId = value ?? 0;
    return accountId === 0 ? fallback : accountId;
}

function mapHeroStandings(heroes: DotaHeroStanding[]): CMsgGCGetHeroStandingsResponse_Hero[] {
    const mapped: CMsgGCGetHeroStandingsResponse_Hero[] = [];
    for (let i = 0; i < heroes.length; i++) {
        const hero = heroes[i];
        mapped.push({
            heroId: hero.heroId,
            wins: hero.wins,
            losses: hero.losses,
            winStreak: hero.winStreak,
            bestWinStreak: hero.bestWinStreak,
            avgKills: hero.avgKills,
            avgDeaths: hero.avgDeaths,
            avgAssists: hero.avgAssists,
            avgGpm: hero.avgGpm,
            avgXpm: hero.avgXpm,
            bestKills: hero.bestKills,
            bestAssists: hero.bestAssists,
            bestGpm: hero.bestGpm,
            bestXpm: hero.bestXpm,
            performance: hero.performance,
            networthPeak: hero.networthPeak,
            lasthitPeak: hero.lasthitPeak,
            denyPeak: hero.denyPeak,
            damagePeak: hero.damagePeak,
            longestGamePeak: hero.longestGamePeak,
            healingPeak: hero.healingPeak,
            avgLasthits: hero.avgLasthits,
            avgDenies: hero.avgDenies
        });
    }

    return mapped;
}

function mapHeroStatsHistory(matches: DotaMatchPlayer[]): CMsgDOTASDOHeroStatsHistory[] {
    const mapped: CMsgDOTASDOHeroStatsHistory[] = [];
    for (let i = 0; i < matches.length; i++) {
        const match = matches[i];
        mapped.push({
            matchId: match.matchId,
            gameMode: match.gameMode,
            lobbyType: match.lobbyType,
            startTime: match.startTime,
            won: match.winner,
            gpm: match.gpm,
            xpm: match.xpm,
            kills: match.kills,
            deaths: match.deaths,
            assists: match.assists
        });
    }

    return mapped;
}

function mapMatchHistory(matches: DotaMatchPlayer[]): CMsgDOTAGetPlayerMatchHistoryResponse_Match[] {
    const mapped: CMsgDOTAGetPlayerMatchHistoryResponse_Match[] = [];
    for (let i = 0; i < matches.length; i++) {
        const match = matches[i];
        mapped.push({
            matchId: match.matchId,
            startTime: match.startTime,
            heroId: match.heroId,
            winner: match.winner,
            gameMode: match.gameMode,
            rankChange: 0,
            previousRank: 0,
            lobbyType: match.lobbyType,
            soloRank: false,
            abandon: match.leaverStatus !== 0,
            duration: match.duration,
            engine: 1,
            activePlusSubscription: true,
            seasonalRank: false,
            selectedFacet: match.selectedFacet
        });
    }

    return mapped;
}

function mapShowcase(stats: DotaShowcaseStats): CMsgShowcase {
    return {
        showcaseItems: [
            showcaseStat(1, 3, stats.gamesWon, 0, 0),
            showcaseStat(2, 4, stats.commendCount, 280, 0),
            showcaseStat(3, 8, stats.mvpCount, 560, 0)
        ],
        moderationState: 0
    };
}

function showcaseStat(itemId: number, statId: number, score: number, x: number, y: number): CMsgShowcaseItem {
    return {
        showcaseItemId: itemId,
        itemPosition: {
            positionX: x,
            positionY: y,
            scale: 100,
            width: 240,
            height: 120
        },
        itemData: {
            stat: {
                data: {
                    statScore: score
                },
                statId
            }
        },
        state: 0
    };
}

function mapPlayerRecentAccomplishments(source: DotaPlayerRecentAccomplishments): CMsgPlayerRecentAccomplishments {
    return {
        recentOutcomes: mapRecentOutcomes(source.recentOutcomes),
        totalRecord: mapRecentRecord(source.totalRecord),
        predictionStreak: source.predictionStreak,
        plusPredictionStreak: source.plusPredictionStreak,
        recentCommends: mapRecentCommends(source.recentCommends),
        firstMatchTimestamp: source.firstMatchTimestamp,
        lastMatch: mapOptionalPlayerRecentMatchInfo(source.lastMatch),
        recentMvps: mapRecentOutcomes(source.recentMvps)
    };
}

function mapHeroRecentAccomplishments(source: DotaHeroRecentAccomplishments): CMsgPlayerHeroRecentAccomplishments {
    return {
        recentOutcomes: mapRecentOutcomes(source.recentOutcomes),
        totalRecord: mapRecentRecord(source.totalRecord),
        lastMatch: mapOptionalPlayerRecentMatchInfo(source.lastMatch)
    };
}

function mapOptionalPlayerRecentMatchInfo(
    source: DotaPlayerRecentMatchInfo | null
): CMsgPlayerRecentMatchInfo | undefined {
    if (source !== null) {
        return mapPlayerRecentMatchInfo(source);
    }

    return undefined;
}

function mapPlayerRecentMatchInfo(source: DotaPlayerRecentMatchInfo): CMsgPlayerRecentMatchInfo {
    return {
        matchId: source.matchId,
        timestamp: source.timestamp,
        duration: source.duration,
        win: source.win,
        heroId: source.heroId,
        kills: source.kills,
        deaths: source.deaths,
        assists: source.assists
    };
}

function mapRecentOutcomes(source: DotaRecentMatchOutcomes): CMsgPlayerRecentMatchOutcomes {
    return {
        outcomes: source.outcomes ?? 0,
        matchCount: source.matchCount ?? 0
    };
}

function mapRecentRecord(source: DotaPlayerMatchRecord): CMsgPlayerMatchRecord {
    return {
        wins: source.wins ?? 0,
        losses: source.losses ?? 0
    };
}

function mapRecentCommends(source: DotaPlayerRecentCommends): CMsgPlayerRecentCommends {
    return {
        commends: source.commends ?? 0,
        matchCount: source.matchCount ?? 0
    };
}
