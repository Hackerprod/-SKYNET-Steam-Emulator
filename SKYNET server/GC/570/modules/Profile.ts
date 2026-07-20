import {
    DotaHeroStats,
    DotaHeroSticker,
    DotaMatchPlayer,
    DotaProfileSnapshot,
    DotaProfileSlot,
    DotaQuestProgress,
    DotaRecentMatch,
    DotaTeam,
    HandlerContext,
    RawMessageContext,
    gc
} from "../framework/gc";
import {
    CMsgBattleReport,
    CMsgBattleReportAggregateStats,
    CMsgBattleReportAggregateStats_CMsgBattleReportAggregate,
    CMsgBattleReportAggregateStats_CMsgBattleReportStat,
    CMsgBattleReportGame,
    CMsgBattleReportInfo,
    CMsgClientToGCAcknowledgeBattleReport,
    CMsgClientToGCAcknowledgeBattleReportResponse,
    CMsgClientToGCAcknowledgeBattleReportResponse_EResponse,
    CMsgClientToGCGetBattleReport,
    CMsgClientToGCGetBattleReportAggregateStats,
    CMsgClientToGCGetBattleReportAggregateStatsResponse,
    CMsgClientToGCGetBattleReportAggregateStatsResponse_EResponse,
    CMsgClientToGCGetBattleReportInfo,
    CMsgClientToGCGetBattleReportInfoResponse,
    CMsgClientToGCGetBattleReportInfoResponse_EResponse,
    CMsgClientToGCGetBattleReportMatchHistory,
    CMsgClientToGCGetBattleReportMatchHistoryResponse,
    CMsgClientToGCGetBattleReportMatchHistoryResponse_EResponse,
    CMsgClientToGCGetBattleReportResponse,
    CMsgClientToGCGetBattleReportResponse_EResponse,
    CMsgClientToGCGetAllHeroProgress,
    CMsgClientToGCGetAllHeroProgressResponse,
    CMsgClientToGCGetAllHeroOrder,
    CMsgClientToGCGetAllHeroOrderResponse,
    CMsgClientToGCGetHeroStickers,
    CMsgClientToGCGetHeroStickersResponse,
    CMsgClientToGCGetHeroStickersResponse_EResponse,
    CMsgClientToGCGetProfileCard,
    CMsgClientToGCGetProfileCardStats,
    CMsgClientToGCGetProfileTickets,
    CMsgClientToGCGetQuestProgress,
    CMsgClientToGCGetQuestProgressResponse,
    CMsgClientToGCMonsterHunterGetUserData,
    CMsgClientToGCMonsterHunterGetUserDataResponse,
    CMsgClientToGCMonsterHunterGetUserDataResponse_EResponse,
    CMsgClientToGCOverworldGetUserData,
    CMsgClientToGCOverworldGetUserDataResponse,
    CMsgClientToGCOverworldGetUserDataResponse_EResponse,
    CMsgClientToGCSetHeroSticker,
    CMsgClientToGCSetHeroStickerResponse,
    CMsgClientToGCSetHeroStickerResponse_EResponse,
    CMsgClientToGCGetTrophyList,
    CMsgClientToGCGetTrophyListResponse,
    CMsgClientToGCSetProfileCardSlots,
    CMsgDOTAGetPeriodicResource,
    CMsgDOTAGetPeriodicResourceResponse,
    CMsgDOTAProfileCard,
    CMsgDOTAProfileCard_Slot,
    CMsgDOTAProfileTickets,
    CMsgDOTATeamInfo,
    CMsgDOTATeamsInfo,
    CMsgPlayerConductScorecard,
    CMsgPlayerConductScorecardRequest,
    CMsgStickerHero,
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
    Proto,
    Routes
} from "../generated/dota";
import { buildEconItem, equipmentForDefIndex } from "./InventorySos";
import { normalizeConductScorecard } from "./shared/conduct";

const PROFILE_SLOT_STAT = 1;
const PROFILE_SLOT_TROPHY = 2;
const PROFILE_SLOT_ITEM = 3;
const PROFILE_SLOT_HERO = 4;
const PROFILE_SLOT_EMOTICON = 5;
const PROFILE_SLOT_TEAM = 6;
const MATCH_OUTCOME_RADIANT_WIN = 2;
const MATCH_OUTCOME_DIRE_WIN = 3;
const BATTLE_REPORT_WINDOW_SECONDS = 30 * 24 * 60 * 60;
const BATTLE_REPORT_MATCH_LIMIT = 100;
const TEAM_RADIANT = 2;
const TEAM_DIRE = 3;

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
        gc.on(Routes.GetQuestProgress, (ctx) => this.getQuestProgress(ctx));
        gc.on(Routes.LatestConductScorecard, (ctx) => this.latestConductScorecard(ctx));
        gc.onMessage(Msg.ClientToGCMyTeamInfoRequest, (ctx) => this.myTeamInfo(ctx));
        gc.on(Routes.GetPeriodicResource, (ctx) => this.getPeriodicResource(ctx));
        gc.on(Routes.GetBattleReport, (ctx) => this.getBattleReport(ctx));
        gc.on(Routes.GetBattleReportAggregateStats, (ctx) => this.getBattleReportAggregateStats(ctx));
        gc.on(Routes.GetBattleReportInfo, (ctx) => this.getBattleReportInfo(ctx));
        gc.on(Routes.GetBattleReportMatchHistory, (ctx) => this.getBattleReportMatchHistory(ctx));
        gc.on(Routes.AcknowledgeBattleReport, (ctx) => this.acknowledgeBattleReport(ctx));
        gc.on(Routes.SetHeroSticker, (ctx) => this.setHeroSticker(ctx));
        gc.on(Routes.GetHeroStickers, (ctx) => this.getHeroStickers(ctx));
        gc.on(Routes.OverworldGetUserData, (ctx) => this.overworldGetUserData(ctx));
        gc.on(Routes.MonsterHunterGetUserData, (ctx) => this.monsterHunterGetUserData(ctx));
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

    private getQuestProgress(
        ctx: HandlerContext<CMsgClientToGCGetQuestProgress, CMsgClientToGCGetQuestProgressResponse>
    ): boolean {
        ctx.reply({
            success: true,
            quests: mapQuestProgress(ctx.services.profiles.getQuestProgress(ctx.request.questIds ?? []))
        });
        return true;
    }

    private latestConductScorecard(
        ctx: HandlerContext<CMsgPlayerConductScorecardRequest, CMsgPlayerConductScorecard>
    ): boolean {
        ctx.reply(normalizeConductScorecard(ctx.services.profiles.getConductScorecard()));
        return true;
    }

    private myTeamInfo(ctx: RawMessageContext): boolean {
        const response: CMsgDOTATeamsInfo = { teams: mapTeams(ctx.services.teams.getForAccount(ctx.accountId)) };
        ctx.reply(Msg.GCToClientTeamsInfo, Proto.CMsgDOTATeamsInfo, response);
        return true;
    }

    private getPeriodicResource(
        ctx: HandlerContext<CMsgDOTAGetPeriodicResource, CMsgDOTAGetPeriodicResourceResponse>
    ): boolean {
        const resource = ctx.services.profiles.getPeriodicResource(
            requestedAccountId(ctx.request.accountId, ctx.accountId),
            ctx.request.periodicResourceId ?? 0
        );
        ctx.reply({
            periodicResourceMax: resource.resourceMax,
            periodicResourceUsed: resource.resourceUsed
        });
        return true;
    }

    private getBattleReport(
        ctx: HandlerContext<CMsgClientToGCGetBattleReport, CMsgClientToGCGetBattleReportResponse>
    ): boolean {
        const matches = battleReportMatches(ctx, ctx.request.accountId, ctx.request.timestamp, ctx.request.duration);
        ctx.reply({
            response: CMsgClientToGCGetBattleReportResponse_EResponse.Success,
            report: buildBattleReport(matches),
            aggregateStats: buildBattleReportAggregateStats(matches, ctx.request.accountId ?? ctx.accountId, []),
            info: buildBattleReportInfo(matches, ctx.clock.now())
        });
        return true;
    }

    private getBattleReportAggregateStats(
        ctx: HandlerContext<
            CMsgClientToGCGetBattleReportAggregateStats,
            CMsgClientToGCGetBattleReportAggregateStatsResponse
        >
    ): boolean {
        const matches = battleReportMatches(ctx, ctx.accountId, ctx.request.timestamp, ctx.request.duration);
        ctx.reply({
            response: CMsgClientToGCGetBattleReportAggregateStatsResponse_EResponse.Success,
            aggregateStats: buildBattleReportAggregateStats(matches, ctx.accountId, ctx.request.aggregateKeys ?? [])
        });
        return true;
    }

    private getBattleReportInfo(
        ctx: HandlerContext<CMsgClientToGCGetBattleReportInfo, CMsgClientToGCGetBattleReportInfoResponse>
    ): boolean {
        const matches = battleReportMatches(ctx, ctx.request.accountId, undefined, undefined);
        ctx.reply({
            response: CMsgClientToGCGetBattleReportInfoResponse_EResponse.Success,
            battleReportInfoList: {
                battleReportInfo: matches.length === 0 ? [] : [buildBattleReportInfo(matches, ctx.clock.now())]
            }
        });
        return true;
    }

    private getBattleReportMatchHistory(
        ctx: HandlerContext<
            CMsgClientToGCGetBattleReportMatchHistory,
            CMsgClientToGCGetBattleReportMatchHistoryResponse
        >
    ): boolean {
        const matches = battleReportMatches(ctx, ctx.request.accountId, ctx.request.timestamp, ctx.request.duration);
        ctx.reply({
            response: CMsgClientToGCGetBattleReportMatchHistoryResponse_EResponse.Success,
            games: { games: mapBattleReportGames(matches) }
        });
        return true;
    }

    private acknowledgeBattleReport(
        ctx: HandlerContext<CMsgClientToGCAcknowledgeBattleReport, CMsgClientToGCAcknowledgeBattleReportResponse>
    ): boolean {
        ctx.reply({ response: CMsgClientToGCAcknowledgeBattleReportResponse_EResponse.Success });
        return true;
    }

    private setHeroSticker(
        ctx: HandlerContext<CMsgClientToGCSetHeroSticker, CMsgClientToGCSetHeroStickerResponse>
    ): boolean {
        const success = ctx.services.profiles.setHeroSticker(ctx.request.heroId ?? 0, ctx.request.newItemId ?? 0n);
        ctx.reply({
            response: success
                ? CMsgClientToGCSetHeroStickerResponse_EResponse.Success
                : CMsgClientToGCSetHeroStickerResponse_EResponse.InternalError
        });
        return true;
    }

    private getHeroStickers(
        ctx: HandlerContext<CMsgClientToGCGetHeroStickers, CMsgClientToGCGetHeroStickersResponse>
    ): boolean {
        ctx.reply({
            response: CMsgClientToGCGetHeroStickersResponse_EResponse.Success,
            stickerHeroes: { heroes: mapHeroStickers(ctx.services.profiles.getHeroStickers()) }
        });
        return true;
    }

    private overworldGetUserData(
        ctx: HandlerContext<CMsgClientToGCOverworldGetUserData, CMsgClientToGCOverworldGetUserDataResponse>
    ): boolean {
        const state = ctx.services.profiles.getOverworldState(ctx.request.overworldId ?? 0);
        ctx.reply({
            response: CMsgClientToGCOverworldGetUserDataResponse_EResponse.Success,
            userData: {
                tokenInventory: { tokenCounts: [] },
                overworldNodes: [],
                overworldPaths: [],
                currentNodeId: state.currentNodeId,
                minigameData: [],
                lastRelatedHeroId: state.lastRelatedHeroId,
                overworldVersion: state.overworldVersion
            }
        });
        return true;
    }

    private monsterHunterGetUserData(
        ctx: HandlerContext<CMsgClientToGCMonsterHunterGetUserData, CMsgClientToGCMonsterHunterGetUserDataResponse>
    ): boolean {
        const state = ctx.services.profiles.getMonsterHunterState();
        ctx.reply({
            response: CMsgClientToGCMonsterHunterGetUserDataResponse_EResponse.Success,
            userData: {
                materialInventory: { materialCounts: [] },
                heroCodex: [],
                unlockedCount: state.unlockedCount
            }
        });
        return true;
    }
}

function mapQuestProgress(progress: DotaQuestProgress[]): CMsgClientToGCGetQuestProgressResponse["quests"] {
    return progress.map((quest) => ({
        questId: quest.questId,
        completedChallenges: quest.completedChallenges.map((challenge) => ({
            challengeId: challenge.challengeId,
            timeCompleted: challenge.timeCompleted,
            attempts: challenge.attempts,
            heroId: challenge.heroId,
            templateId: challenge.templateId,
            questRank: challenge.questRank
        }))
    }));
}

function battleReportMatches<TRequest, TResponse>(
    ctx: HandlerContext<TRequest, TResponse>,
    accountIdValue: number | undefined,
    timestamp: number | undefined,
    duration: number | undefined
): DotaMatchPlayer[] {
    const accountId = requestedAccountId(accountIdValue, ctx.accountId);
    const matches = ctx.services.stats.getMatchHistory(accountId, 0n, BATTLE_REPORT_MATCH_LIMIT, 0, true);
    if (timestamp === undefined || timestamp === 0) {
        return matches;
    }

    const windowDuration = duration === undefined || duration === 0 ? BATTLE_REPORT_WINDOW_SECONDS : duration;
    const windowEnd = timestamp + windowDuration;
    return matches.filter((match) => match.startTime >= timestamp && match.startTime <= windowEnd);
}

function buildBattleReport(matches: DotaMatchPlayer[]): CMsgBattleReport {
    return {
        games: mapBattleReportGames(matches),
        highlights: { highlights: [] }
    };
}

function mapBattleReportGames(matches: DotaMatchPlayer[]): CMsgBattleReportGame[] {
    return matches.map((match) => ({
        heroId: match.heroId,
        kills: match.kills,
        deaths: match.deaths,
        assists: match.assists,
        lastHits: match.lastHits,
        gpm: match.gpm,
        xpm: match.xpm,
        role: roleFromPlayerSlot(match.playerSlot),
        outcome: match.winner ? 0 : 1,
        laneOutcome: -1,
        ranked: false,
        matchId: match.matchId,
        predictedPosition: roleFromPlayerSlot(match.playerSlot),
        secondsDead: 0,
        winningTeam: match.goodGuysWin ? TEAM_RADIANT : TEAM_DIRE,
        partyGame: false,
        startTime: match.startTime,
        denies: match.denies,
        playerSlot: match.playerSlot,
        supportGold: match.supportGold,
        heroDamage: match.heroDamage,
        heroHealing: match.heroHealing,
        towerDamage: match.towerDamage,
        duration: match.duration,
        gameMode: match.gameMode,
        lobbyType: match.lobbyType,
        item0: match.items[0] ?? 0,
        item1: match.items[1] ?? 0,
        item2: match.items[2] ?? 0,
        item3: match.items[3] ?? 0,
        item4: match.items[4] ?? 0,
        item5: match.items[5] ?? 0,
        selectedFacet: match.selectedFacet
    }));
}

function buildBattleReportInfo(matches: DotaMatchPlayer[], now: number): CMsgBattleReportInfo {
    const latest = matches.length === 0 ? null : matches[0];
    return {
        timestamp: latest?.startTime ?? now,
        duration: BATTLE_REPORT_WINDOW_SECONDS,
        acknowledged: false,
        featuredHeroId: latest?.heroId ?? 0,
        featuredPosition: latest === null ? 0 : roleFromPlayerSlot(latest.playerSlot),
        gamesPlayed: matches.length,
        medalCounts: []
    };
}

function buildBattleReportAggregateStats(
    matches: DotaMatchPlayer[],
    accountId: number,
    keys: readonly { readonly heroId?: number; readonly predictedPosition?: number }[]
): CMsgBattleReportAggregateStats {
    const aggregateKeys =
        keys.length === 0
            ? uniqueBattleReportKeys(matches)
            : keys.map((key) => ({
                  heroId: key.heroId ?? 0,
                  predictedPosition: key.predictedPosition ?? -1
              }));

    return {
        result: aggregateKeys.map((key) =>
            buildBattleReportAggregate(matches, accountId, key.heroId, key.predictedPosition)
        )
    };
}

function uniqueBattleReportKeys(
    matches: DotaMatchPlayer[]
): { readonly heroId: number; readonly predictedPosition: number }[] {
    const seen = new Set<string>();
    const result: { heroId: number; predictedPosition: number }[] = [];
    for (const match of matches) {
        const key = `${match.heroId}:${roleFromPlayerSlot(match.playerSlot)}`;
        if (!seen.has(key)) {
            seen.add(key);
            result.push({ heroId: match.heroId, predictedPosition: roleFromPlayerSlot(match.playerSlot) });
        }
    }

    return result;
}

function buildBattleReportAggregate(
    matches: DotaMatchPlayer[],
    accountId: number,
    heroId: number,
    predictedPosition: number
): CMsgBattleReportAggregateStats_CMsgBattleReportAggregate {
    const filtered = matches.filter(
        (match) =>
            match.accountId === accountId &&
            (heroId === 0 || match.heroId === heroId) &&
            (predictedPosition < 0 || roleFromPlayerSlot(match.playerSlot) === predictedPosition)
    );

    return {
        heroId,
        predictedPosition,
        gameCount: filtered.length,
        winCount: filtered.filter((match) => match.winner).length,
        laneWinCount: 0,
        kills: stat(filtered.map((match) => match.kills)),
        deaths: stat(filtered.map((match) => match.deaths)),
        assists: stat(filtered.map((match) => match.assists)),
        lastHits: stat(filtered.map((match) => match.lastHits)),
        denies: stat(filtered.map((match) => match.denies)),
        gpm: stat(filtered.map((match) => match.gpm)),
        xpm: stat(filtered.map((match) => match.xpm)),
        supportGold: stat(filtered.map((match) => match.supportGold)),
        heroDamage: stat(filtered.map((match) => match.heroDamage)),
        heroHealing: stat(filtered.map((match) => match.heroHealing)),
        towerDamage: stat(filtered.map((match) => match.towerDamage)),
        duration: stat(filtered.map((match) => match.duration))
    };
}

function stat(values: number[]): CMsgBattleReportAggregateStats_CMsgBattleReportStat {
    if (values.length === 0) {
        return { mean: 0, stdev: 0 };
    }

    const mean = values.reduce((sum, value) => sum + value, 0) / values.length;
    const variance = values.reduce((sum, value) => sum + (value - mean) * (value - mean), 0) / values.length;
    return { mean, stdev: Math.sqrt(variance) };
}

function roleFromPlayerSlot(playerSlot: number): number {
    const slot = playerSlot % 5;
    if (slot < 0 || slot > 4) {
        return -1;
    }

    return slot;
}

function mapHeroStickers(stickers: DotaHeroSticker[]): CMsgStickerHero[] {
    return stickers.map((sticker) => ({
        heroId: sticker.heroId,
        itemDefId: sticker.itemDefId,
        quality: sticker.quality,
        sourceItemId: sticker.sourceItemId
    }));
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

function mapTeams(source: DotaTeam[]): CMsgDOTATeamInfo[] {
    const teams: CMsgDOTATeamInfo[] = [];
    for (let i = 0; i < source.length; i++) {
        const team = source[i];
        teams.push({
            teamId: team.teamId,
            name: team.name,
            tag: team.tag,
            ugcLogo: team.logo,
            ugcBaseLogo: team.baseLogo,
            ugcBannerLogo: team.bannerLogo,
            urlLogo: team.logoUrl,
            abbreviation: team.abbreviation,
            countryCode: team.countryCode,
            url: team.url,
            wins: team.wins,
            losses: team.losses,
            gamesPlayedTotal: team.gamesPlayedTotal,
            gamesPlayedMatchmaking: team.gamesPlayedMatchmaking,
            region: team.region
        });
    }

    return teams;
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
