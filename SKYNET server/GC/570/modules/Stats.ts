import { DotaHeroStanding, HandlerContext, gc } from "../framework/gc";
import {
    CMsgClientToGCLookupAccountName,
    CMsgClientToGCLookupAccountNameResponse,
    CMsgClientToGCPlayerStatsRequest,
    CMsgClientToGCRankRequest,
    CMsgClientToGCTeammateStatsRequest,
    CMsgClientToGCTeammateStatsResponse,
    CMsgDOTAGetEventPoints,
    CMsgDOTAGetEventPointsResponse,
    CMsgGCGetHeroStandings,
    CMsgGCGetHeroStandingsResponse,
    CMsgGCGetHeroStandingsResponse_Hero,
    CMsgGCToClientPlayerStatsResponse,
    CMsgGCToClientRankResponse,
    CMsgHeroGlobalDataRequest,
    CMsgHeroGlobalDataResponse,
    Msg,
    Routes
} from "../generated/dota";

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
        gc.onMessage(Msg.GCGetHeroStatsHistory, () => this.getHeroStatsHistory());
        gc.onMessage(Msg.DOTAGetPlayerMatchHistory, () => this.getPlayerMatchHistory());
        gc.onMessage(Msg.GCMatchDetailsRequest, () => this.matchDetails());
        gc.on(Routes.PlayerStats, (ctx) => this.playerStats(ctx));
        gc.on(Routes.HeroGlobalData, (ctx) => this.heroGlobalData(ctx));
        gc.on(Routes.TeammateStats, (ctx) => this.teammateStats(ctx));
        gc.on(Routes.RankRequest, (ctx) => this.rankRequest(ctx));
        gc.onMessage(Msg.ClientToGCShowcaseGetUserData, () => this.showcaseGetUserData());
        gc.onMessage(Msg.ClientToGCRequestPlayerRecentAccomplishments, () => this.clientRecentAccomplishments());
        gc.onMessage(Msg.ClientToGCRequestPlayerHeroRecentAccomplishments, () =>
            this.clientHeroRecentAccomplishments()
        );
        gc.onMessage(Msg.ClientToGCHasPlayerVotedForMVP, () => this.hasPlayerVotedForMvp());
        gc.onMessage(Msg.ClientToGCVoteForMVP, () => this.voteForMvp());
        gc.onMessage(Msg.ClientToGCMVPVoteTimeout, () => this.mvpVoteTimeout());
        gc.onMessage(Msg.GCSubmitLobbyMVPVote, () => this.submitLobbyMvpVote());
        gc.onMessage(Msg.SignOutMVPStats, () => this.signOutMvpStats());
        gc.onMessage(Msg.ClientToGCRerollPlayerChallenge, () => this.rerollPlayerChallenge());
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
    getHeroStatsHistory(): boolean {
        return false;
    }
    getPlayerMatchHistory(): boolean {
        return false;
    }
    matchDetails(): boolean {
        return false;
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
    showcaseGetUserData(): boolean {
        return false;
    }
    clientRecentAccomplishments(): boolean {
        return false;
    }
    clientHeroRecentAccomplishments(): boolean {
        return false;
    }
    hasPlayerVotedForMvp(): boolean {
        return false;
    }
    voteForMvp(): boolean {
        return false;
    }
    mvpVoteTimeout(): boolean {
        return false;
    }
    submitLobbyMvpVote(): boolean {
        return false;
    }
    signOutMvpStats(): boolean {
        return false;
    }
    rerollPlayerChallenge(): boolean {
        return false;
    }
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
