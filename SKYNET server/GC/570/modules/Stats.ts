import { DotaHeroStanding, HandlerContext, gc } from "../framework/gc";
import {
    CMsgClientToGCRankRequest,
    CMsgDOTAGetEventPoints,
    CMsgDOTAGetEventPointsResponse,
    CMsgGCGetHeroStandings,
    CMsgGCGetHeroStandingsResponse,
    CMsgGCGetHeroStandingsResponse_Hero,
    CMsgGCToClientRankResponse,
    Msg,
    Routes
} from "../generated/dota";

export function registerStats(): void {
    const stats = new Stats();
    stats.register();
}

export class Stats {
    register(): void {
        gc.onMessage(Msg.ClientToGCLookupAccountName, () => this.lookupAccountName());
        gc.on(Routes.GetEventPoints, (ctx) => this.getEventPoints(ctx));
        gc.onMessage(Msg.GCMatchmakingStatsRequest, () => this.matchmakingStats());
        gc.on(Routes.GetHeroStandings, (ctx) => this.getHeroStandings(ctx));
        gc.onMessage(Msg.GCGetHeroStatsHistory, () => this.getHeroStatsHistory());
        gc.onMessage(Msg.DOTAGetPlayerMatchHistory, () => this.getPlayerMatchHistory());
        gc.onMessage(Msg.GCMatchDetailsRequest, () => this.matchDetails());
        gc.onMessage(Msg.ClientToGCPlayerStatsRequest, () => this.playerStats());
        gc.onMessage(Msg.HeroGlobalDataRequest, () => this.heroGlobalData());
        gc.onMessage(Msg.ClientToGCTeammateStatsRequest, () => this.teammateStats());
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

    lookupAccountName(): boolean {
        return false;
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
    playerStats(): boolean {
        return false;
    }
    heroTimedStats(): boolean {
        return false;
    }
    heroGlobalData(): boolean {
        return false;
    }
    teammateStats(): boolean {
        return false;
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
