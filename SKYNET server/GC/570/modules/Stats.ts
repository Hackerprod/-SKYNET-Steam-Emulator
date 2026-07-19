import {
    DotaHeroRecentAccomplishments,
    DotaHeroStanding,
    DotaMatch,
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
    CMsgDOTAMatch,
    CMsgDOTAMatch_Player,
    CMsgDOTASDOHeroStatsHistory,
    CMsgDOTASubmitLobbyMVPVote,
    CMsgDOTASubmitLobbyMVPVoteResponse,
    CMsgGCGetHeroStandings,
    CMsgGCGetHeroStandingsResponse,
    CMsgGCGetHeroStandingsResponse_Hero,
    CMsgGCGetHeroStatsHistory,
    CMsgGCGetHeroStatsHistoryResponse,
    CMsgGCMatchDetailsRequest,
    CMsgGCMatchDetailsResponse,
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
const STATS_NOT_FOUND = 0;
const MATCH_OUTCOME_RADIANT = 2;
const MATCH_OUTCOME_DIRE = 3;
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
        gc.onMessage(Msg.GCMatchmakingStatsRequest, () => this.matchmakingStats());
        gc.on(Routes.GetHeroStandings, (ctx) => this.getHeroStandings(ctx));
        gc.on(Routes.GetHeroStatsHistory, (ctx) => this.getHeroStatsHistory(ctx));
        gc.on(Routes.GetPlayerMatchHistory, (ctx) => this.getPlayerMatchHistory(ctx));
        gc.on(Routes.MatchDetails, (ctx) => this.matchDetails(ctx));
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

    matchmakingStats(): boolean {
        return false;
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

    matchDetails(ctx: HandlerContext<CMsgGCMatchDetailsRequest, CMsgGCMatchDetailsResponse>): boolean {
        const match = ctx.services.stats.getMatchDetails(ctx.request.matchId ?? 0n);
        if (match !== null) {
            ctx.reply({
                result: STATS_SUCCESS,
                match: mapMatchDetails(match),
                vote: 0
            });
            return true;
        }

        ctx.reply({ result: STATS_NOT_FOUND, vote: 0 });
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

function mapMatchDetails(match: DotaMatch): CMsgDOTAMatch {
    return {
        duration: match.duration,
        starttime: match.startTime,
        players: mapMatchPlayers(match.players),
        matchId: match.matchId,
        cluster: match.cluster,
        firstBloodTime: match.firstBloodTime,
        lobbyType: match.lobbyType,
        humanPlayers: countHumanPlayers(match.players),
        gameMode: match.gameMode === 0 ? 1 : match.gameMode,
        engine: 1,
        matchFlags: match.matchFlags,
        radiantTeamScore: match.radiantScore,
        direTeamScore: match.direScore,
        matchOutcome: match.goodGuysWin ? MATCH_OUTCOME_RADIANT : MATCH_OUTCOME_DIRE
    };
}

function mapMatchPlayers(players: DotaMatchPlayer[]): CMsgDOTAMatch_Player[] {
    const mapped: CMsgDOTAMatch_Player[] = [];
    for (let i = 0; i < players.length; i++) {
        const player = players[i];
        mapped.push({
            accountId: player.accountId,
            playerSlot: player.playerSlot,
            heroId: player.heroId,
            item0: matchItem(player, 0),
            item1: matchItem(player, 1),
            item2: matchItem(player, 2),
            item3: matchItem(player, 3),
            item4: matchItem(player, 4),
            item5: matchItem(player, 5),
            expectedTeamContribution: 0,
            scaledMetric: 0,
            previousRank: 0,
            rankChange: 0,
            kills: player.kills,
            deaths: player.deaths,
            assists: player.assists,
            leaverStatus: player.leaverStatus,
            gold: player.gold,
            lastHits: player.lastHits,
            denies: player.denies,
            goldPerMin: player.gpm,
            xpPerMin: player.xpm,
            goldSpent: player.goldSpent,
            heroDamage: player.heroDamage,
            towerDamage: player.towerDamage,
            heroHealing: player.heroHealing,
            level: player.level,
            playerName: player.personaName,
            claimedFarmGold: player.claimedFarmGold,
            supportGold: player.supportGold,
            activePlusSubscription: true,
            netWorth: Math.round(player.netWorth),
            item6: matchItem(player, 6),
            item7: matchItem(player, 7),
            item8: matchItem(player, 8),
            item9: matchItem(player, 9),
            bountyRunes: player.bountyRunes,
            outpostsCaptured: player.outpostsCaptured,
            teamNumber: player.goodGuys ? 0 : 1,
            teamSlot: player.playerSlot,
            selectedFacet: player.selectedFacet,
            item10: matchItem(player, 10)
        });
    }

    return mapped;
}

function countHumanPlayers(players: DotaMatchPlayer[]): number {
    let count = 0;
    for (let i = 0; i < players.length; i++) {
        if (players[i].steamId !== 0n) {
            count++;
        }
    }

    return count;
}

function matchItem(player: DotaMatchPlayer, index: number): number {
    if (index < player.items.length) {
        return player.items[index];
    }

    return 0;
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
