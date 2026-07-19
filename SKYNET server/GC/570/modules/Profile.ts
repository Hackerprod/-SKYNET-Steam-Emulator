import {
    DotaHeroStats,
    DotaProfileSnapshot,
    DotaProfileSlot,
    DotaRecentMatch,
    HandlerContext,
    gc
} from "../framework/gc";
import {
    CMsgClientToGCGetAllHeroProgress,
    CMsgClientToGCGetAllHeroProgressResponse,
    CMsgClientToGCGetAllHeroOrder,
    CMsgClientToGCGetAllHeroOrderResponse,
    CMsgClientToGCGetProfileCard,
    CMsgClientToGCGetProfileCardStats,
    CMsgClientToGCGetProfileTickets,
    CMsgClientToGCGetTrophyList,
    CMsgClientToGCGetTrophyListResponse,
    CMsgClientToGCSetProfileCardSlots,
    CMsgDOTAProfileCard,
    CMsgDOTAProfileCard_Slot,
    CMsgDOTAProfileTickets,
    CMsgProfileRequest,
    CMsgProfileResponse,
    CMsgProfileResponse_EResponse,
    CMsgProfileResponse_FeaturedHero,
    CMsgProfileResponse_MatchInfo,
    CMsgProfileUpdate,
    CMsgProfileUpdateResponse,
    CMsgProfileUpdateResponse_Result,
    CMsgRecentMatchInfo,
    CMsgSuccessfulHero,
    Msg,
    Routes
} from "../generated/dota";
import { buildEconItem, equipmentForDefIndex } from "./Items";

const PROFILE_SLOT_STAT = 1;
const PROFILE_SLOT_TROPHY = 2;
const PROFILE_SLOT_ITEM = 3;
const PROFILE_SLOT_HERO = 4;
const PROFILE_SLOT_EMOTICON = 5;
const PROFILE_SLOT_TEAM = 6;
const MATCH_OUTCOME_RADIANT_WIN = 2;
const MATCH_OUTCOME_DIRE_WIN = 3;

export function registerProfile(): void {
    const profile = new Profile();
    profile.register();
}

export class Profile {
    register(): void {
        gc.on(Routes.GetProfileCard, (ctx) => this.getProfileCard(ctx));
        gc.on(Routes.GetProfileCardStats, (ctx) => this.getProfileCardStats(ctx));
        gc.on(Routes.SetProfileCardSlots, (ctx) => this.setProfileCardSlots(ctx));
        gc.on(Routes.GetProfileTickets, (ctx) => this.getProfileTickets(ctx));
        gc.on(Routes.ProfileRequest, (ctx) => this.profileRequest(ctx));
        gc.on(Routes.ProfileUpdate, (ctx) => this.profileUpdate(ctx));
        gc.on(Routes.GetTrophyList, (ctx) => this.getTrophyList(ctx));
        gc.on(Routes.GetAllHeroOrder, (ctx) => this.getAllHeroOrder(ctx));
        gc.on(Routes.GetAllHeroProgress, (ctx) => this.getAllHeroProgress(ctx));
        gc.onMessage(Msg.ClientToGCGetQuestProgress, () => false);
        gc.onMessage(Msg.ClientToGCLatestConductScorecardRequest, () => false);
        gc.onMessage(Msg.ClientToGCMyTeamInfoRequest, () => false);
        gc.onMessage(Msg.DOTAGetPeriodicResource, () => false);
        gc.onMessage(Msg.ClientToGCGetBattleReportInfo, () => false);
        gc.onMessage(Msg.ClientToGCGetHeroStickers, () => false);
        gc.onMessage(Msg.ClientToGCOverworldGetUserData, () => false);
        gc.onMessage(Msg.ClientToGCMonsterHunterGetUserData, () => false);
    }

    private getProfileCard(ctx: HandlerContext<CMsgClientToGCGetProfileCard, CMsgDOTAProfileCard>): boolean {
        const accountId = requestedAccountId(ctx.request.accountId, ctx.accountId);
        ctx.reply(buildProfileCard(ctx.services.profiles.get(accountId)));
        return true;
    }

    private getProfileCardStats(ctx: HandlerContext<CMsgClientToGCGetProfileCardStats, CMsgDOTAProfileCard>): boolean {
        ctx.reply(buildProfileCard(ctx.services.profiles.get(ctx.accountId)));
        return true;
    }

    private setProfileCardSlots(ctx: HandlerContext<CMsgClientToGCSetProfileCardSlots, CMsgDOTAProfileCard>): boolean {
        const slots = normalizeProfileSlots(ctx.request.slots ?? []);
        ctx.services.profiles.saveCardSlots(slots);
        ctx.reply(buildProfileCard(ctx.services.profiles.get(ctx.accountId)));
        return true;
    }

    private getProfileTickets(ctx: HandlerContext<CMsgClientToGCGetProfileTickets, CMsgDOTAProfileTickets>): boolean {
        const accountId = requestedAccountId(ctx.request.accountId, ctx.accountId);
        ctx.reply({
            result: 1,
            accountId,
            leaguePasses: []
        });
        return true;
    }

    private profileRequest(ctx: HandlerContext<CMsgProfileRequest, CMsgProfileResponse>): boolean {
        const snapshot = ctx.services.profiles.get(requestedAccountId(ctx.request.accountId, ctx.accountId));
        ctx.reply(buildProfileResponse(ctx, snapshot));
        return true;
    }

    private profileUpdate(ctx: HandlerContext<CMsgProfileUpdate, CMsgProfileUpdateResponse>): boolean {
        ctx.services.profiles.saveProfileUpdate(ctx.request.backgroundItemId ?? 0n, ctx.request.featuredHeroIds ?? []);
        ctx.reply({ result: CMsgProfileUpdateResponse_Result.Success });
        return true;
    }

    private getTrophyList(
        ctx: HandlerContext<CMsgClientToGCGetTrophyList, CMsgClientToGCGetTrophyListResponse>
    ): boolean {
        const snapshot = ctx.services.profiles.get(requestedAccountId(ctx.request.accountId, ctx.accountId));
        const trophies = [];
        for (let i = 0; i < snapshot.trophies.length; i++) {
            const trophy = snapshot.trophies[i];
            trophies.push({
                trophyId: trophy.trophyId,
                trophyScore: trophy.trophyScore,
                lastUpdated: trophy.lastUpdated
            });
        }

        ctx.reply({ trophies });
        return true;
    }

    private getAllHeroOrder(
        ctx: HandlerContext<CMsgClientToGCGetAllHeroOrder, CMsgClientToGCGetAllHeroOrderResponse>
    ): boolean {
        const snapshot = ctx.services.profiles.get(ctx.accountId);
        ctx.reply({ heroIds: snapshot.allHeroProgress.heroIds });
        return true;
    }

    private getAllHeroProgress(
        ctx: HandlerContext<CMsgClientToGCGetAllHeroProgress, CMsgClientToGCGetAllHeroProgressResponse>
    ): boolean {
        const snapshot = ctx.services.profiles.get(requestedAccountId(ctx.request.accountId, ctx.accountId));
        const progress = snapshot.allHeroProgress;
        ctx.reply({
            accountId: progress.accountId,
            currHeroId: progress.currentHeroId,
            lapsCompleted: progress.lapsCompleted,
            currHeroGames: progress.currentHeroGames,
            currLapTimeStarted: progress.currentLapStarted,
            currLapGames: progress.currentLapGames,
            bestLapGames: progress.bestLapGames,
            bestLapTime: progress.bestLapTime,
            lapHeroesCompleted: progress.lapHeroesCompleted,
            lapHeroesRemaining: progress.lapHeroesRemaining,
            nextHeroId: progress.nextHeroId,
            prevHeroId: progress.previousHeroId,
            prevHeroGames: progress.previousHeroGames,
            prevAvgTries: 1,
            currAvgTries: 1,
            nextAvgTries: 1,
            fullLapAvgTries: 1,
            currLapAvgTries: 1,
            profileName: progress.profileName,
            startHeroId: progress.startHeroId
        });
        return true;
    }
}

function buildProfileResponse<TRequest, TResponse>(
    ctx: HandlerContext<TRequest, TResponse>,
    snapshot: DotaProfileSnapshot
): CMsgProfileResponse {
    if (snapshot.recentMatches.length > 0) {
        return {
            featuredHeroes: buildFeaturedHeroes(ctx, snapshot),
            recentMatches: buildRecentMatches(snapshot),
            successfulHeroes: buildSuccessfulHeroes(snapshot),
            recentMatchDetails: buildRecentMatchDetails(snapshot.recentMatches[0]),
            result: CMsgProfileResponse_EResponse.Success
        };
    }

    return {
        featuredHeroes: buildFeaturedHeroes(ctx, snapshot),
        recentMatches: [],
        successfulHeroes: buildSuccessfulHeroes(snapshot),
        result: CMsgProfileResponse_EResponse.Success
    };
}

function buildProfileCard(snapshot: DotaProfileSnapshot): CMsgDOTAProfileCard {
    return {
        accountId: snapshot.accountId,
        slots: buildProfileCardSlots(snapshot),
        badgePoints: snapshot.badgePoints,
        eventId: snapshot.activeEventId,
        rankTier: snapshot.rankTier,
        leaderboardRank: snapshot.leaderboardRank,
        isPlusSubscriber: snapshot.isPlusSubscriber,
        plusOriginalStartDate: snapshot.plusOriginalStartDate,
        rankTierScore: snapshot.rankTierScore,
        leaderboardRankCore: snapshot.leaderboardRank,
        title: 0,
        lifetimeGames: snapshot.lifetimeGames,
        eventLevel: snapshot.level
    };
}

function buildProfileCardSlots(snapshot: DotaProfileSnapshot): CMsgDOTAProfileCard_Slot[] {
    const slots: CMsgDOTAProfileCard_Slot[] = [];
    for (let i = 0; i < snapshot.slots.length; i++) {
        const profileSlot = snapshot.slots[i];
        const slot = buildProfileCardSlot(snapshot, profileSlot);
        if (slot !== null) {
            slots.push(slot);
        }
    }

    return slots;
}

function buildProfileCardSlot(
    snapshot: DotaProfileSnapshot,
    profileSlot: DotaProfileSlot
): CMsgDOTAProfileCard_Slot | null {
    if (profileSlot.slotType === PROFILE_SLOT_STAT) {
        const statId = Number(profileSlot.slotValue);
        return {
            slotId: profileSlot.slotId,
            stat: {
                statId,
                statScore: profileStatScore(snapshot, statId)
            }
        };
    }

    if (profileSlot.slotType === PROFILE_SLOT_TROPHY) {
        const trophyId = Number(profileSlot.slotValue);
        const trophy = findTrophyScore(snapshot, trophyId);
        return {
            slotId: profileSlot.slotId,
            trophy: {
                trophyId,
                trophyScore: trophy
            }
        };
    }

    if (profileSlot.slotType === PROFILE_SLOT_ITEM) {
        return {
            slotId: profileSlot.slotId,
            item: {
                itemId: profileSlot.slotValue
            }
        };
    }

    if (profileSlot.slotType === PROFILE_SLOT_HERO) {
        const heroId = Number(profileSlot.slotValue & 0xffffn);
        const hero = findHeroStats(snapshot, heroId);
        return {
            slotId: profileSlot.slotId,
            hero: {
                heroId,
                heroWins: hero?.wins ?? 0,
                heroLosses: hero?.losses ?? 0
            }
        };
    }

    if (profileSlot.slotType === PROFILE_SLOT_EMOTICON) {
        return {
            slotId: profileSlot.slotId,
            emoticon: {
                emoticonId: Number(profileSlot.slotValue)
            }
        };
    }

    if (profileSlot.slotType === PROFILE_SLOT_TEAM) {
        return {
            slotId: profileSlot.slotId,
            team: {
                teamId: Number(profileSlot.slotValue)
            }
        };
    }

    return null;
}

function buildFeaturedHeroes<TRequest, TResponse>(
    ctx: HandlerContext<TRequest, TResponse>,
    snapshot: DotaProfileSnapshot
): CMsgProfileResponse_FeaturedHero[] {
    const featuredHeroes: CMsgProfileResponse_FeaturedHero[] = [];
    const inventory = ctx.services.items.getInventory();
    for (let i = 0; i < snapshot.featuredHeroIds.length && i < 3; i++) {
        const heroId = snapshot.featuredHeroIds[i];
        const equippedEconItems = [];
        for (let itemIndex = 0; itemIndex < inventory.equipment.length; itemIndex++) {
            const equipped = inventory.equipment[itemIndex];
            if (equipped.heroId !== heroId) {
                continue;
            }

            const catalogItem = ctx.services.items.getCatalogItem(equipped.defIndex);
            if (catalogItem !== null) {
                equippedEconItems.push(
                    buildEconItem(inventory, catalogItem, equipmentForDefIndex(inventory, equipped.defIndex))
                );
            }
        }

        featuredHeroes.push({
            heroId,
            equippedEconItems,
            manuallySet: true,
            plusHeroXp: (findHeroStats(snapshot, heroId)?.wins ?? 0) * 100
        });
    }

    return featuredHeroes;
}

function buildRecentMatches(snapshot: DotaProfileSnapshot): CMsgProfileResponse_MatchInfo[] {
    const recentMatches: CMsgProfileResponse_MatchInfo[] = [];
    for (let i = 0; i < snapshot.recentMatches.length; i++) {
        const match = snapshot.recentMatches[i];
        recentMatches.push({
            matchId: match.matchId,
            matchTimestamp: match.startTime,
            performanceRating: match.winner ? 1 : 2,
            heroId: match.heroId,
            wonMatch: match.winner
        });
    }

    return recentMatches;
}

function buildRecentMatchDetails(match: DotaRecentMatch): CMsgRecentMatchInfo {
    return {
        matchId: match.matchId,
        gameMode: match.gameMode,
        kills: match.kills,
        deaths: match.deaths,
        assists: match.assists,
        duration: match.duration,
        playerSlot: match.playerSlot,
        matchOutcome: match.winner ? MATCH_OUTCOME_RADIANT_WIN : MATCH_OUTCOME_DIRE_WIN,
        timestamp: match.startTime,
        lobbyType: match.lobbyType,
        teamNumber: match.team
    };
}

function buildSuccessfulHeroes(snapshot: DotaProfileSnapshot): CMsgSuccessfulHero[] {
    const heroes: CMsgSuccessfulHero[] = [];
    const count = snapshot.heroes.length < 5 ? snapshot.heroes.length : 5;
    for (let i = 0; i < count; i++) {
        const hero = snapshot.heroes[i];
        const total = hero.wins + hero.losses;
        heroes.push({
            heroId: hero.heroId,
            winPercent: total === 0 ? 0 : (hero.wins * 100) / total,
            longestStreak: hero.bestWinStreak
        });
    }

    return heroes;
}

function normalizeProfileSlots(
    slots: readonly { readonly slotId?: number; readonly slotType?: number; readonly slotValue?: bigint }[]
): DotaProfileSlot[] {
    const normalized: DotaProfileSlot[] = [];
    for (let i = 0; i < slots.length; i++) {
        const slot = slots[i];
        const slotId = slot.slotId ?? 0;
        if (slotId === 0) {
            continue;
        }

        normalized.push({
            slotId,
            slotType: slot.slotType ?? 0,
            slotValue: slot.slotValue ?? 0n
        });
    }

    return normalized;
}

function requestedAccountId(value: number | undefined, fallback: number): number {
    const accountId = value ?? 0;
    if (accountId === 0) {
        return fallback;
    }

    return accountId;
}

function profileStatScore(snapshot: DotaProfileSnapshot, statId: number): number {
    if (statId === 3) {
        return snapshot.globalStats.gamesWon;
    }

    if (statId === 4) {
        return snapshot.conduct.commendCount;
    }

    if (statId === 5) {
        return snapshot.globalStats.matchCount;
    }

    if (statId === 6) {
        return snapshot.firstMatchTime;
    }

    if (statId === 8) {
        return snapshot.mvpCount;
    }

    return 0;
}

function findHeroStats(snapshot: DotaProfileSnapshot, heroId: number): DotaHeroStats | null {
    for (let i = 0; i < snapshot.heroes.length; i++) {
        if (snapshot.heroes[i].heroId === heroId) {
            return snapshot.heroes[i];
        }
    }

    return null;
}

function findTrophyScore(snapshot: DotaProfileSnapshot, trophyId: number): number {
    for (let i = 0; i < snapshot.trophies.length; i++) {
        if (snapshot.trophies[i].trophyId === trophyId) {
            return snapshot.trophies[i].trophyScore;
        }
    }

    return 0;
}
