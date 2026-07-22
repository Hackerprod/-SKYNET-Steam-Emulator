import { GcRoute, ProtoDescriptor } from "../generated/dota";

export type HandlerResult = void | boolean | Promise<void | boolean>;
export type RouteHandler<TRequest, TResponse> = (ctx: HandlerContext<TRequest, TResponse>) => HandlerResult;
export type RawMessageHandler = (ctx: RawMessageContext) => HandlerResult;

export interface GcContextBase {
    readonly steamId: bigint;
    readonly accountId: number;
    readonly personaName: string;
    readonly services: GcServices;
    readonly clock: Clock;
    readonly logger: Logger;
    readonly signal: AbortSignal;
    send<TMessage>(messageType: number, proto: ProtoDescriptor<TMessage>, message: TMessage): void;
    encode<TMessage>(proto: ProtoDescriptor<TMessage>, message: TMessage): Uint8Array;
}

export interface HandlerContext<TRequest, TResponse> {
    readonly route: GcRoute<TRequest, TResponse>;
    readonly request: TRequest;
    readonly steamId: bigint;
    readonly accountId: number;
    readonly personaName: string;
    readonly services: GcServices;
    readonly clock: Clock;
    readonly logger: Logger;
    readonly signal: AbortSignal;
    reply(response: TResponse): void;
    send<TMessage>(messageType: number, proto: ProtoDescriptor<TMessage>, message: TMessage): void;
    encode<TMessage>(proto: ProtoDescriptor<TMessage>, message: TMessage): Uint8Array;
}

export interface RawMessageContext {
    readonly messageType: number;
    readonly payload: Uint8Array;
    readonly steamId: bigint;
    readonly accountId: number;
    readonly personaName: string;
    readonly services: GcServices;
    readonly clock: Clock;
    readonly logger: Logger;
    readonly signal: AbortSignal;
    reply<TMessage>(messageType: number, proto: ProtoDescriptor<TMessage>, message: TMessage): void;
    send<TMessage>(messageType: number, proto: ProtoDescriptor<TMessage>, message: TMessage): void;
    encode<TMessage>(proto: ProtoDescriptor<TMessage>, message: TMessage): Uint8Array;
    decode<TMessage>(proto: ProtoDescriptor<TMessage>): TMessage;
}

export interface AbortSignal {
    readonly aborted: boolean;
    readonly reason: string | null;
    throwIfAborted(): void;
}

export interface Clock {
    now(): number;
}

export interface Logger {
    info(message: string): void;
}

export interface GcServices {
    readonly items: DotaItemService;
    readonly match: DotaMatchService;
    readonly party: DotaPartyService;
    readonly profiles: DotaProfileService;
    readonly social: DotaSocialService;
    readonly chat: DotaChatService;
    readonly guilds: DotaGuildService;
    readonly stats: DotaStatsService;
    readonly lobby: DotaLobbyService;
    readonly teams: DotaTeamService;
}

export interface DotaItemService {
    getInventory(steamId?: bigint): DotaRuntimeInventory;
    getCatalogItem(defIndex: number): DotaCatalogItem | null;
    equipItem(itemId: bigint, heroId: number, slotId: number, style: number): DotaEquipment[];
    setItemStyle(itemId: bigint, style: number): DotaEquipment[];
}

export interface DotaLobbyService {
    queueMessage(steamId: bigint, messageType: number, payload: Uint8Array, protobuf?: boolean): boolean;
    publishSnapshot(snapshot: DotaLobbyMatchSnapshot): boolean;
    listSnapshots(): DotaLobbyMatchSnapshot[];
    removeSnapshot(lobbyId: bigint): boolean;
    startDedicatedServer(lobbyId: bigint, map: string): DotaDedicatedLaunchResult | null;
    releaseDedicatedServer(lobbyId: bigint, reason: string): boolean;
    resolveGameServerConnectIp(publicIp: string, privateIp: string, fallbackIp: string): string;
    resolveGameServerConnectIps(publicIp: string, privateIp: string, fallbackIp: string): string;
}

export interface DotaDedicatedLaunchResult {
    readonly started: boolean;
    readonly port: number;
    readonly state: string;
    readonly error: string;
}

export interface DotaLobbyMatchSnapshot {
    readonly lobbyId: bigint;
    readonly matchId: bigint;
    readonly serverSteamId: bigint;
    readonly connect: string;
    readonly state: number;
    readonly gameState: number;
    readonly gameStartTime: number;
    readonly dedicated: boolean;
    readonly updatedAtUnix?: number;
    readonly players: DotaLobbyMatchPlayer[];
}

export interface DotaLobbyMatchPlayer {
    readonly steamId: bigint;
    readonly accountId: number;
    readonly personaName: string;
    readonly team: number;
    readonly slot: number;
    readonly coachTeam: number;
    readonly heroId: number;
}

export interface DotaTeamService {
    get(teamId: number): DotaTeam | null;
    getForAccount(accountId?: number): DotaTeam[];
    nextTeamId(): number;
    upsert(team: DotaTeamUpsert): DotaTeam | null;
    addMember(teamId: number, accountId: number, role: number): boolean;
    removeMember(teamId: number, accountId: number): boolean;
    remove(teamId: number): boolean;
    nameAvailable(name: string, exceptTeamId?: number): boolean;
    tagAvailable(tag: string, exceptTeamId?: number): boolean;
    getPlayerInfo(accountId: number): DotaTeamPlayerInfo | null;
    savePlayerInfo(info: DotaTeamPlayerInfoUpsert): DotaTeamPlayerInfo | null;
    deletePlayerInfo(accountId: number): boolean;
}

export interface DotaTeam {
    readonly teamId: number;
    readonly name: string;
    readonly tag: string;
    readonly role: number | null;
    readonly logo: bigint;
    readonly baseLogo: bigint;
    readonly bannerLogo: bigint;
    readonly logoUrl: string;
    readonly abbreviation: string;
    readonly countryCode: string;
    readonly url: string;
    readonly wins: number;
    readonly losses: number;
    readonly gamesPlayedTotal: number;
    readonly gamesPlayedMatchmaking: number;
    readonly region: number;
    readonly members: DotaTeamMember[];
}

export interface DotaTeamMember {
    readonly accountId: number;
    readonly role: number;
}

export interface DotaTeamUpsert {
    readonly teamId: number;
    readonly name: string;
    readonly tag: string;
    readonly logo: bigint;
    readonly baseLogo: bigint;
    readonly bannerLogo: bigint;
    readonly sponsorLogo: bigint;
    readonly countryCode: string;
    readonly url: string;
    readonly pickupTeam: boolean;
    readonly abbreviation: string;
}

export interface DotaTeamPlayerInfo {
    readonly accountId: number;
    readonly name: string;
    readonly countryCode: string;
    readonly fantasyRole: number;
    readonly teamId: number;
    readonly sponsor: string;
    readonly realName: string;
}

export interface DotaTeamPlayerInfoUpsert {
    readonly accountId: number;
    readonly name: string;
    readonly countryCode: string;
    readonly fantasyRole: number;
    readonly teamId: number;
    readonly sponsor: string;
    readonly realName: string;
}

export interface DotaRuntimeInventory {
    readonly steamId: bigint;
    readonly version: bigint;
    readonly catalogItems?: DotaCatalogItem[];
    readonly ownedItems: DotaCatalogItem[];
    readonly equipment: DotaEquipment[];
}

export interface DotaCatalogItem {
    readonly defIndex: number;
    readonly name: string;
    readonly qualityId: number;
}

export interface DotaEquipment {
    readonly steamId: bigint;
    readonly heroId: number;
    readonly slotId: number;
    readonly defIndex: number;
    readonly itemId: bigint;
    readonly style: number;
}

export interface DotaPartyService {
    getCurrent(): DotaPartyState | null;
    getById(partyId: bigint): DotaPartyState | null;
    ensureCurrent(ping: DotaPartyPingData): DotaPartyState;
    addMember(partyId: bigint, ping: DotaPartyPingData, isCoach: boolean): DotaPartyState | null;
    removeMember(partyId: bigint, steamId: bigint): DotaPartyState | null;
    deleteParty(partyId: bigint): boolean;
    setLeader(partyId: bigint, leaderSteamId: bigint): DotaPartyState | null;
    setMemberCoach(partyId: bigint, steamId: bigint, isCoach: boolean): DotaPartyState | null;
    setMemberPing(partyId: bigint, steamId: bigint, ping: DotaPartyPingData): DotaPartyState | null;
    startReadyCheck(partyId: bigint, durationSeconds: number): DotaPartyState | null;
    acknowledgeReadyCheck(partyId: bigint, readyStatus: number): DotaPartyState | null;
    createInvite(
        partyId: bigint,
        targetSteamId: bigint,
        teamId: number,
        asCoach: boolean
    ): DotaPartyInviteMutation | null;
    takeInvite(partyId: bigint): DotaPartyInvite | null;
    getInvitesForTarget(targetSteamId: bigint): DotaPartyInvite[];
    deleteInvitesForTarget(targetSteamId: bigint): DotaPartyInvite[];
    deleteInvitesForParty(partyId: bigint): DotaPartyInvite[];
    pruneInvitesCreatedBefore(cutoff: number): DotaPartyInvite[];
    userExists(steamId: bigint): boolean;
    userOnline(steamId: bigint): boolean;
    queueMessage(steamId: bigint, messageType: number, payload: Uint8Array, protobuf?: boolean): boolean;
}

export interface DotaPartyState {
    readonly partyId: bigint;
    readonly leaderSteamId: bigint;
    readonly state: number;
    readonly permanent: boolean;
    readonly readyStartTimestamp: number;
    readonly readyFinishTimestamp: number;
    readonly readyInitiatorAccountId: number;
    readonly members: DotaPartyMember[];
}

export interface DotaPartyMember {
    readonly steamId: bigint;
    readonly accountId: number;
    readonly personaName: string;
    readonly position: number;
    readonly isCoach: boolean;
    readonly isPlusSubscriber: boolean;
    readonly regionCodes: number[];
    readonly regionPings: number[];
    readonly regionPingFailedBitmask: number;
    readonly readyStatus: number;
}

export interface DotaPartyInvite {
    readonly inviteGid: bigint;
    readonly partyId: bigint;
    readonly targetSteamId: bigint;
    readonly senderSteamId: bigint;
    readonly senderName: string;
    readonly teamId: number;
    readonly asCoach: boolean;
    readonly lowPriorityStatus: boolean;
    readonly createdAt: number;
}

export interface DotaPartyInviteMutation {
    readonly invite: DotaPartyInvite;
    readonly replaced: DotaPartyInvite | null;
}

export interface DotaPartyPingData {
    readonly regionCodes: number[];
    readonly regionPings: number[];
    readonly regionPingFailedBitmask: number;
}

export interface DotaProfileService {
    get(accountId: number): DotaProfileSnapshot;
    saveCardSlots(slots: DotaProfileSlot[]): void;
    saveProfileUpdate(backgroundItemId: bigint, featuredHeroIds: number[]): void;
    getConductScorecard(): DotaConductScorecard;
    getQuestProgress(questIds: number[]): DotaQuestProgress[];
    getPeriodicResource(accountId: number, resourceId: number): DotaPeriodicResource;
    getHeroStickers(): DotaHeroSticker[];
    setHeroSticker(heroId: number, itemId: bigint): boolean;
    getOverworldState(overworldId: number): DotaOverworldState;
    getMonsterHunterState(): DotaMonsterHunterState;
}

export interface DotaSocialService {
    emoticonAccess(): DotaEmoticonAccess;
    feed(accountId: number, selfOnly: boolean): DotaSocialFeedEvent[];
    comments(feedEventId: bigint): DotaSocialFeedComment[];
    matchComments(matchId: bigint): DotaSocialFeedComment[];
    postComment(feedEventId: bigint, comment: string): boolean;
    postMatchComment(matchId: bigint, comment: string): boolean;
}

export interface DotaChatService {
    all(): DotaChatChannelSummary[];
    join(channelName: string, channelType: number): DotaChatChannel | null;
    get(channelId: bigint): DotaChatChannel | null;
    leave(channelId: bigint): boolean;
    broadcast(channelId: bigint, messageType: number, payload: Uint8Array, includeSelf?: boolean): number;
}

export interface DotaChatChannelSummary {
    readonly channelId: bigint;
    readonly channelName: string;
    readonly channelType: number;
    readonly maxMembers: number;
    readonly numMembers: number;
}

export interface DotaChatChannel {
    readonly channelId: bigint;
    readonly channelName: string;
    readonly channelType: number;
    readonly maxMembers: number;
    readonly isMember: boolean;
    readonly channelUserId: number;
    readonly justJoined: boolean;
    readonly members: DotaChatMember[];
}

export interface DotaChatMember {
    readonly steamId: bigint;
    readonly accountId: number;
    readonly personaName: string;
    readonly channelUserId: number;
}

export interface DotaGuildService {
    ensureCurrent(): DotaGuild;
    getMembership(accountId?: number): DotaGuildMembership;
    getGuild(guildId: number): DotaGuild | null;
    getPersonaInfo(accountId: number): DotaGuildPersona[];
    getEventData(guildId: number, eventId: number): DotaGuildEventData;
    invite(guildId: number, targetAccountId: number): number;
    declineInvite(guildId: number): number;
    cancelInvite(guildId: number, targetAccountId: number): number;
    acceptInvite(guildId: number): number;
    leave(guildId: number): number;
    getReporterUpdates(): DotaReporterUpdates;
    acknowledgeReporterUpdates(matchIds: bigint[]): boolean;
}

export interface DotaGuild {
    readonly guildId: number;
    readonly info: DotaGuildInfo;
    readonly roles: DotaGuildRole[];
    readonly members: DotaGuildMember[];
    readonly invites: DotaGuildInvite[];
}

export interface DotaGuildInfo {
    readonly guildName: string;
    readonly guildTag: string;
    readonly createdTimestamp: number;
    readonly guildLanguage: number;
    readonly guildFlags: number;
    readonly guildLogo: bigint;
    readonly guildRegion: number;
    readonly guildChatGroupId: bigint;
    readonly guildDescription: string;
    readonly defaultChatChannelId: bigint;
    readonly guildPrimaryColor: number;
    readonly guildSecondaryColor: number;
    readonly guildPattern: number;
    readonly guildRefreshTimeOffset: number;
    readonly guildRequiredRankTier: number;
    readonly guildMotdTimestamp: number;
    readonly guildMotd: string;
}

export interface DotaGuildRole {
    readonly roleId: number;
    readonly roleName: string;
    readonly roleFlags: number;
    readonly roleOrder: number;
}

export interface DotaGuildMember {
    readonly accountId: number;
    readonly roleId: number;
    readonly joinedTimestamp: number;
    readonly lastActiveTimestamp: number;
}

export interface DotaGuildInvite {
    readonly guildId: number;
    readonly requesterAccountId: number;
    readonly targetAccountId: number;
    readonly timestampSent: number;
}

export interface DotaGuildMembership {
    readonly guildIds: number[];
    readonly invites: DotaGuildInvite[];
}

export interface DotaGuildPersona {
    readonly guildId: number;
    readonly guildTag: string;
    readonly guildFlags: number;
}

export interface DotaGuildEventData {
    readonly guildId: number;
    readonly eventId: number;
    readonly isMember: boolean;
    readonly guildPoints: number;
    readonly contractsRefreshedTimestamp: number;
    readonly completedChallengeCount: number;
    readonly challengesRefreshTimestamp: number;
    readonly guildWeeklyPercentile: number;
    readonly guildWeeklyLastTimestamp: number;
    readonly lastWeeklyClaimTime: number;
    readonly guildCurrentPercentile: number;
}

export interface DotaReporterUpdates {
    readonly updates: DotaReporterUpdate[];
    readonly numReported: number;
    readonly numNoActionTaken: number;
}

export interface DotaReporterUpdate {
    readonly matchId: bigint;
    readonly heroId: number;
    readonly reportReason: number;
    readonly timestamp: number;
}

export interface DotaMatchService {
    recordSignOutPermission(request: DotaMatchSignOutPermissionAudit): boolean;
    setHistoryAccess(allow: boolean): boolean;
    recordServerStatus(response: number): boolean;
    recordLeaver(event: DotaLeaverEvent): boolean;
    recordRealtimeStats(snapshot: DotaRealtimeStatsSnapshot): boolean;
    recordMatchStateHistory(history: DotaMatchStateHistorySnapshot): boolean;
    recordSpectatorCount(spectatorCount: number): boolean;
    recordLiveScoreboard(snapshot: DotaLiveScoreboardSnapshot): boolean;
    savePlayerReport(report: DotaPlayerReport): boolean;
}

export interface DotaMatchSignOutPermissionAudit {
    readonly serverVersion: number;
    readonly localAttempt: number;
    readonly totalAttempt: number;
    readonly secondsWaited: number;
    readonly permissionGranted: boolean;
    readonly abandonSignout: boolean;
    readonly retryDelaySeconds: number;
}

export interface DotaLeaverEvent {
    readonly leaverSteamId: bigint;
    readonly leaverStatus: number;
    readonly lobbyState: number;
    readonly gameState: number;
    readonly leaverDetected: boolean;
    readonly firstBloodHappened: boolean;
    readonly discardMatchResults: boolean;
    readonly massDisconnect: boolean;
    readonly serverCluster: number;
    readonly disconnectReason: number;
}

export interface DotaRealtimeStatsSnapshot {
    readonly matchId: bigint;
    readonly serverSteamId: bigint;
    readonly timestamp: number;
    readonly gameTime: number;
    readonly gameState: number;
    readonly gameMode: number;
    readonly lobbyType: number;
    readonly leagueId: number;
    readonly radiantScore: number;
    readonly direScore: number;
    readonly playerCount: number;
    readonly buildingCount: number;
    readonly deltaFrame: boolean;
    readonly payloadSize: number;
}

export interface DotaMatchStateHistorySnapshot {
    readonly matchId: bigint;
    readonly radiantWon: boolean;
    readonly mmr: number;
    readonly stateCount: number;
    readonly lastGameTime: number;
    readonly radiantKills: number;
    readonly direKills: number;
    readonly payloadSize: number;
}

export interface DotaLiveScoreboardSnapshot {
    readonly matchId: bigint;
    readonly tournamentId: number;
    readonly tournamentGameId: number;
    readonly duration: number;
    readonly hltvDelay: number;
    readonly leagueId: number;
    readonly radiantScore: number;
    readonly direScore: number;
    readonly playerCount: number;
    readonly roshanRespawnTimer: number;
    readonly payloadSize: number;
}

export interface DotaPlayerReport {
    readonly targetAccountId: number;
    readonly lobbyId: bigint;
    readonly reportFlags: number;
    readonly reportReasons: number[];
    readonly comment: string;
    readonly gameTime: number;
    readonly debugSlot: number;
    readonly debugMatchId: bigint;
}

export interface DotaStatsService {
    lookupAccountName(accountId: number): DotaAccountName;
    getEventPoints(accountId: number, eventId: number): DotaEventPoints;
    getHeroStandings(accountId: number): DotaHeroStanding[];
    getHeroGlobalData(accountId: number, heroId: number): DotaHeroGlobalData;
    getPlayerStats(accountId: number): DotaPlayerStats;
    getRank(accountId: number): DotaRank;
    getTeammateStats(accountId: number): DotaTeammateStats[];
    getMatchHistory(
        accountId: number,
        startAtMatchId: bigint,
        requested: number,
        heroId: number,
        includePractice: boolean
    ): DotaMatchPlayer[];
    getMatchDetails(matchId: bigint): DotaMatch | null;
    getHeroStatsHistory(accountId: number, heroId: number): DotaMatchPlayer[];
    getMatchVotes(matchId: bigint): DotaMatchVoteSummary;
    getShowcaseStats(accountId: number): DotaShowcaseStats;
    getRecentAccomplishments(accountId: number): DotaPlayerRecentAccomplishments;
    getHeroRecentAccomplishments(accountId: number, heroId: number): DotaHeroRecentAccomplishments;
    hasMvpVote(matchId: bigint): boolean;
    voteForMvp(matchId: bigint, votedAccountId: number): boolean;
    finalizeMvpVote(matchId: bigint): boolean;
    submitLobbyMvpVote(targetAccountId: number): DotaLobbyMvpVoteResult;
    recordSignOutMvpStats(matchId: bigint, players: DotaSignOutMvpPlayer[]): boolean;
    rerollPlayerChallenge(): boolean;
}

export interface DotaAccountName {
    readonly accountId: number;
    readonly accountName: string;
}

export interface DotaMatchVoteSummary {
    readonly success: boolean;
    readonly vote: number;
    readonly positiveVotes: number;
    readonly negativeVotes: number;
}

export interface DotaProfileSnapshot {
    readonly accountId: number;
    readonly steamId: bigint;
    readonly personaName: string;
    readonly backgroundItemDefIndex: number;
    readonly rankTier: number;
    readonly rankTierScore: number;
    readonly leaderboardRank: number;
    readonly badgePoints: number;
    readonly eventPoints: number;
    readonly activeEventId: number;
    readonly lifetimeGames: number;
    readonly level: number;
    readonly isPlusSubscriber: boolean;
    readonly plusOriginalStartDate: number;
    readonly firstMatchTime: number;
    readonly mvpCount: number;
    readonly globalStats: DotaGlobalStats;
    readonly conduct: DotaConduct;
    readonly slots: DotaProfileSlot[];
    readonly heroes: DotaHeroStats[];
    readonly trophies: DotaTrophy[];
    readonly featuredHeroIds: number[];
    readonly recentMatches: DotaRecentMatch[];
    readonly allHeroProgress: DotaAllHeroProgress;
}

export interface DotaGlobalStats {
    readonly gamesWon: number;
    readonly gamesLost: number;
    readonly matchCount: number;
}

export interface DotaConduct {
    readonly commendCount: number;
    readonly rawBehaviorScore: number;
    readonly reportsCount?: number;
    readonly matchesAbandoned?: number;
    readonly commsReports?: number;
}

export interface DotaConductScorecard {
    readonly accountId: number;
    readonly matchId: bigint;
    readonly seqNum: number;
    readonly reasons: number;
    readonly matchesInReport: number;
    readonly matchesClean: number;
    readonly matchesReported: number;
    readonly matchesAbandoned: number;
    readonly reportsCount: number;
    readonly reportsParties: number;
    readonly commendCount: number;
    readonly date: number;
    readonly rawBehaviorScore: number;
    readonly oldRawBehaviorScore: number;
    readonly commsReports: number;
    readonly commsParties: number;
    readonly behaviorRating: number;
}

export interface DotaQuestProgress {
    readonly questId: number;
    readonly completedChallenges: DotaQuestChallenge[];
}

export interface DotaQuestChallenge {
    readonly challengeId: number;
    readonly timeCompleted: number;
    readonly attempts: number;
    readonly heroId: number;
    readonly templateId: number;
    readonly questRank: number;
}

export interface DotaPeriodicResource {
    readonly accountId: number;
    readonly resourceId: number;
    readonly resourceMax: number;
    readonly resourceUsed: number;
}

export interface DotaHeroSticker {
    readonly heroId: number;
    readonly itemDefId: number;
    readonly quality: number;
    readonly sourceItemId: bigint;
}

export interface DotaOverworldState {
    readonly overworldId: number;
    readonly currentNodeId: number;
    readonly lastRelatedHeroId: number;
    readonly overworldVersion: number;
}

export interface DotaMonsterHunterState {
    readonly unlockedCount: number;
}

export interface DotaProfileSlot {
    readonly slotId: number;
    readonly slotType: number;
    readonly slotValue: bigint;
}

export interface DotaHeroStats {
    readonly heroId: number;
    readonly wins: number;
    readonly losses: number;
    readonly bestWinStreak: number;
}

export interface DotaTrophy {
    readonly trophyId: number;
    readonly trophyScore: number;
    readonly lastUpdated: number;
}

export interface DotaRecentMatch {
    readonly matchId: bigint;
    readonly startTime: number;
    readonly duration: number;
    readonly heroId: number;
    readonly kills: number;
    readonly deaths: number;
    readonly assists: number;
    readonly winner: boolean;
    readonly gameMode: number;
    readonly lobbyType: number;
    readonly playerSlot: number;
    readonly team: number;
}

export interface DotaAllHeroProgress {
    readonly accountId: number;
    readonly heroIds: number[];
    readonly currentHeroId: number;
    readonly nextHeroId: number;
    readonly previousHeroId: number;
    readonly startHeroId: number;
    readonly lapsCompleted: number;
    readonly currentHeroGames: number;
    readonly currentLapStarted: number;
    readonly currentLapGames: number;
    readonly bestLapGames: number;
    readonly bestLapTime: number;
    readonly lapHeroesCompleted: number;
    readonly lapHeroesRemaining: number;
    readonly previousHeroGames: number;
    readonly profileName: string;
}

export interface DotaSocialFeedEvent {
    readonly feedEventId: bigint;
    readonly accountId: number;
    readonly timestamp: number;
    readonly commentCount: number;
    readonly eventType: number;
    readonly eventSubType: number;
    readonly paramBigInt1: bigint;
    readonly paramInt1: number;
    readonly paramInt2: number;
    readonly paramInt3: number;
    readonly paramString: string;
}

export interface DotaEmoticonAccess {
    readonly accountId: number;
    readonly unlockedEmoticons: Uint8Array;
    readonly updatedAt: number;
}

export interface DotaSocialFeedComment {
    readonly feedEventId: bigint;
    readonly commenterAccountId: number;
    readonly personaName: string;
    readonly commentText: string;
    readonly timestamp: number;
}

export interface DotaEventPoints {
    readonly accountId: number;
    readonly eventId: number;
    readonly totalPoints: number;
    readonly totalPremiumPoints: number;
    readonly points: number;
    readonly premiumPoints: number;
    readonly owned: boolean;
    readonly auditAction: number;
    readonly activeSeasonId: number;
}

export interface DotaPlayerStats {
    readonly accountId: number;
    readonly matchCount: number;
    readonly meanGpm: number;
    readonly meanXppm: number;
    readonly meanLasthits: number;
    readonly rampages: number;
    readonly tripleKills: number;
    readonly firstBloodClaimed: number;
    readonly firstBloodGiven: number;
    readonly couriersKilled: number;
    readonly aegisesSnatched: number;
    readonly cheesesEaten: number;
    readonly creepsStacked: number;
    readonly fightScore: number;
    readonly farmScore: number;
    readonly supportScore: number;
    readonly pushScore: number;
    readonly versatilityScore: number;
    readonly meanNetworth: number;
    readonly meanDamage: number;
    readonly meanHeals: number;
    readonly rapiersPurchased: number;
}

export interface DotaHeroStanding {
    readonly heroId: number;
    readonly wins: number;
    readonly losses: number;
    readonly winStreak: number;
    readonly bestWinStreak: number;
    readonly avgKills: number;
    readonly avgDeaths: number;
    readonly avgAssists: number;
    readonly avgGpm: number;
    readonly avgXpm: number;
    readonly bestKills: number;
    readonly bestAssists: number;
    readonly bestGpm: number;
    readonly bestXpm: number;
    readonly performance: number;
    readonly networthPeak: number;
    readonly lasthitPeak: number;
    readonly denyPeak: number;
    readonly damagePeak: number;
    readonly longestGamePeak: number;
    readonly healingPeak: number;
    readonly avgLasthits: number;
    readonly avgDenies: number;
}

export interface DotaHeroGlobalData {
    readonly heroId: number;
    readonly heroDataPerChunk: DotaHeroGlobalDataChunk[];
}

export interface DotaHeroGlobalDataChunk {
    readonly rankChunk: number;
    readonly heroAverages: DotaGlobalHeroAverages;
}

export interface DotaGlobalHeroAverages {
    readonly lastRun: number;
    readonly avgGoldPerMin: number;
    readonly avgXpPerMin: number;
    readonly avgKills: number;
    readonly avgDeaths: number;
    readonly avgAssists: number;
    readonly avgLastHits: number;
    readonly avgDenies: number;
    readonly avgNetWorth: number;
}

export interface DotaRank {
    readonly result: number;
    readonly rankValue: number;
    readonly rankData1: number;
    readonly rankData2: number;
    readonly rankData3: number;
}

export interface DotaTeammateStats {
    readonly accountId: number;
    readonly games: number;
    readonly wins: number;
    readonly mostRecentGameTimestamp: number;
    readonly mostRecentGameMatchId: bigint;
    readonly performance: number;
}

export interface DotaMatch {
    readonly matchId: bigint;
    readonly ownerSteamId: bigint;
    readonly serverSteamId: bigint;
    readonly startTime: number;
    readonly duration: number;
    readonly gameMode: number;
    readonly lobbyType: number;
    readonly goodGuysWin: boolean;
    readonly matchFlags: number;
    readonly radiantScore: number;
    readonly direScore: number;
    readonly cluster: number;
    readonly firstBloodTime: number;
    readonly players: DotaMatchPlayer[];
}

export interface DotaMatchPlayer {
    readonly matchId: bigint;
    readonly accountId: number;
    readonly steamId: bigint;
    readonly personaName: string;
    readonly team: number;
    readonly playerSlot: number;
    readonly heroId: number;
    readonly kills: number;
    readonly deaths: number;
    readonly assists: number;
    readonly winner: boolean;
    readonly goodGuys: boolean;
    readonly gold: number;
    readonly goldSpent: number;
    readonly gpm: number;
    readonly xpm: number;
    readonly lastHits: number;
    readonly denies: number;
    readonly heroDamage: number;
    readonly towerDamage: number;
    readonly heroHealing: number;
    readonly level: number;
    readonly netWorth: number;
    readonly supportGold: number;
    readonly claimedFarmGold: number;
    readonly bountyRunes: number;
    readonly outpostsCaptured: number;
    readonly selectedFacet: number;
    readonly leaverStatus: number;
    readonly items: number[];
    readonly startTime: number;
    readonly duration: number;
    readonly gameMode: number;
    readonly lobbyType: number;
    readonly goodGuysWin: boolean;
    readonly matchFlags: number;
    readonly radiantScore: number;
    readonly direScore: number;
    readonly cluster: number;
    readonly firstBloodTime: number;
}

export interface DotaShowcaseStats {
    readonly gamesWon: number;
    readonly commendCount: number;
    readonly mvpCount: number;
}

export interface DotaPlayerRecentAccomplishments {
    readonly recentOutcomes: DotaRecentMatchOutcomes;
    readonly totalRecord: DotaPlayerMatchRecord;
    readonly predictionStreak: number;
    readonly plusPredictionStreak: number;
    readonly recentCommends: DotaPlayerRecentCommends;
    readonly firstMatchTimestamp: number;
    readonly lastMatch: DotaPlayerRecentMatchInfo | null;
    readonly recentMvps: DotaRecentMatchOutcomes;
}

export interface DotaHeroRecentAccomplishments {
    readonly recentOutcomes: DotaRecentMatchOutcomes;
    readonly totalRecord: DotaPlayerMatchRecord;
    readonly lastMatch: DotaPlayerRecentMatchInfo | null;
}

export interface DotaRecentMatchOutcomes {
    readonly outcomes: number;
    readonly matchCount: number;
}

export interface DotaPlayerMatchRecord {
    readonly wins: number;
    readonly losses: number;
}

export interface DotaPlayerRecentCommends {
    readonly commends: number;
    readonly matchCount: number;
}

export interface DotaPlayerRecentMatchInfo {
    readonly matchId: bigint;
    readonly timestamp: number;
    readonly duration: number;
    readonly win: boolean;
    readonly heroId: number;
    readonly kills: number;
    readonly deaths: number;
    readonly assists: number;
}

export interface DotaLobbyMvpVoteResult {
    readonly targetAccountId: number;
    readonly result: number;
}

export interface DotaSignOutMvpPlayer {
    readonly accountId?: number;
    readonly rank?: number;
}

function currentSteamId(): bigint {
    return steamId();
}

function currentAccountId(): number {
    return accountId();
}

function currentPersonaName(): string {
    return personaName();
}

class GcClock {
    now(): number {
        return now() as number;
    }
}

class GcLogger {
    info(message: string): void {
        log(message);
    }
}

class GcAbortSignal implements AbortSignal {
    aborted: boolean;
    reason: string | null;

    constructor() {
        this.aborted = false;
        this.reason = null;
    }

    throwIfAborted(): void {
        if (this.aborted) {
            throw new Error("Operation was aborted");
        }
    }
}

class GcDotaItemService implements DotaItemService {
    getInventory(steamId?: bigint): DotaRuntimeInventory {
        return steamId === undefined
            ? (dotaInventory() as DotaRuntimeInventory)
            : (dotaInventory(steamId) as DotaRuntimeInventory);
    }

    getCatalogItem(defIndex: number): DotaCatalogItem | null {
        return dotaCatalogItem(defIndex) as DotaCatalogItem | null;
    }

    equipItem(itemId: bigint, heroId: number, slotId: number, style: number): DotaEquipment[] {
        return dotaEquipItem(itemId, heroId, slotId, style) as DotaEquipment[];
    }

    setItemStyle(itemId: bigint, style: number): DotaEquipment[] {
        return dotaSetItemStyle(itemId, style) as DotaEquipment[];
    }
}

class GcDotaLobbyService implements DotaLobbyService {
    queueMessage(steamId: bigint, messageType: number, payload: Uint8Array, protobuf = true): boolean {
        return dotaQueueGcMessage(steamId, messageType, payload, protobuf);
    }

    publishSnapshot(snapshot: DotaLobbyMatchSnapshot): boolean {
        return dotaPublishMatchSnapshot(snapshot);
    }

    listSnapshots(): DotaLobbyMatchSnapshot[] {
        return dotaListMatchSnapshots() as DotaLobbyMatchSnapshot[];
    }

    removeSnapshot(lobbyId: bigint): boolean {
        return dotaRemoveMatchSnapshot(lobbyId);
    }

    startDedicatedServer(lobbyId: bigint, map: string): DotaDedicatedLaunchResult | null {
        return dotaStartDedicatedServer(lobbyId, map) as DotaDedicatedLaunchResult | null;
    }

    releaseDedicatedServer(lobbyId: bigint, reason: string): boolean {
        return dotaReleaseDedicatedServer(lobbyId, reason);
    }

    resolveGameServerConnectIp(publicIp: string, privateIp: string, fallbackIp: string): string {
        return dotaResolveGameServerConnectIp(publicIp, privateIp, fallbackIp);
    }

    resolveGameServerConnectIps(publicIp: string, privateIp: string, fallbackIp: string): string {
        return dotaResolveGameServerConnectIps(publicIp, privateIp, fallbackIp);
    }
}

class GcDotaTeamService implements DotaTeamService {
    get(teamId: number): DotaTeam | null {
        return dotaTeam(teamId) as DotaTeam | null;
    }

    getForAccount(accountId?: number): DotaTeam[] {
        return accountId === undefined
            ? (dotaTeamsForAccount() as DotaTeam[])
            : (dotaTeamsForAccount(accountId) as DotaTeam[]);
    }

    nextTeamId(): number {
        return dotaNextTeamId();
    }

    upsert(team: DotaTeamUpsert): DotaTeam | null {
        return dotaUpsertTeam(team.teamId, team.name, team.tag, stringifyHostJson(toTeamJson(team))) as DotaTeam | null;
    }

    addMember(teamId: number, accountId: number, role: number): boolean {
        return dotaAddTeamMember(teamId, accountId, role);
    }

    removeMember(teamId: number, accountId: number): boolean {
        return dotaRemoveTeamMember(teamId, accountId);
    }

    remove(teamId: number): boolean {
        return dotaRemoveTeam(teamId);
    }

    nameAvailable(name: string, exceptTeamId: number = 0): boolean {
        return dotaTeamNameAvailable(name, String(exceptTeamId));
    }

    tagAvailable(tag: string, exceptTeamId: number = 0): boolean {
        return dotaTeamTagAvailable(tag, String(exceptTeamId));
    }

    getPlayerInfo(accountId: number): DotaTeamPlayerInfo | null {
        return dotaTeamPlayerInfo(accountId) as DotaTeamPlayerInfo | null;
    }

    savePlayerInfo(info: DotaTeamPlayerInfoUpsert): DotaTeamPlayerInfo | null {
        return dotaUpsertTeamPlayerInfo(
            info.accountId,
            info.name,
            info.countryCode,
            info.fantasyRole,
            info.teamId,
            info.sponsor,
            info.realName
        ) as DotaTeamPlayerInfo | null;
    }

    deletePlayerInfo(accountId: number): boolean {
        return dotaDeleteTeamPlayerInfo(accountId);
    }
}

function toTeamJson(team: DotaTeamUpsert): unknown {
    return {
        name: team.name,
        tag: team.tag,
        logo: team.logo,
        teamLogo: team.logo,
        baseLogo: team.baseLogo,
        teamBaseLogo: team.baseLogo,
        bannerLogo: team.bannerLogo,
        teamBannerLogo: team.bannerLogo,
        sponsorLogo: team.sponsorLogo,
        countryCode: team.countryCode,
        url: team.url,
        pickupTeam: team.pickupTeam,
        abbreviation: team.abbreviation,
        region: 0,
        wins: 0,
        losses: 0,
        gamesPlayedTotal: 0,
        gamesPlayedMatchmaking: 0
    };
}

function stringifyHostJson(value: unknown): string {
    return JSON.stringify(value, (_key, field) => (typeof field === "bigint" ? field.toString() : field));
}

class GcDotaProfileService implements DotaProfileService {
    get(accountId: number): DotaProfileSnapshot {
        return dotaProfile(accountId) as DotaProfileSnapshot;
    }

    saveCardSlots(slots: DotaProfileSlot[]): void {
        dotaSaveProfileSlots(slots);
    }

    saveProfileUpdate(backgroundItemId: bigint, featuredHeroIds: number[]): void {
        dotaSaveProfileUpdate(backgroundItemId, featuredHeroIds);
    }

    getConductScorecard(): DotaConductScorecard {
        return dotaProfileConductScorecard() as DotaConductScorecard;
    }

    getQuestProgress(questIds: number[]): DotaQuestProgress[] {
        return dotaProfileQuestProgress(questIds) as DotaQuestProgress[];
    }

    getPeriodicResource(accountId: number, resourceId: number): DotaPeriodicResource {
        return dotaProfilePeriodicResource(accountId, resourceId) as DotaPeriodicResource;
    }

    getHeroStickers(): DotaHeroSticker[] {
        return dotaProfileHeroStickers() as DotaHeroSticker[];
    }

    setHeroSticker(heroId: number, itemId: bigint): boolean {
        return dotaProfileSetHeroSticker(heroId, itemId) as boolean;
    }

    getOverworldState(overworldId: number): DotaOverworldState {
        return dotaProfileOverworldState(overworldId) as DotaOverworldState;
    }

    getMonsterHunterState(): DotaMonsterHunterState {
        return dotaProfileMonsterHunterState() as DotaMonsterHunterState;
    }
}

class GcDotaSocialService implements DotaSocialService {
    emoticonAccess(): DotaEmoticonAccess {
        return dotaSocialEmoticonAccess() as DotaEmoticonAccess;
    }

    feed(accountId: number, selfOnly: boolean): DotaSocialFeedEvent[] {
        return dotaSocialFeed(accountId, selfOnly) as DotaSocialFeedEvent[];
    }

    comments(feedEventId: bigint): DotaSocialFeedComment[] {
        return dotaSocialFeedComments(feedEventId) as DotaSocialFeedComment[];
    }

    matchComments(matchId: bigint): DotaSocialFeedComment[] {
        return dotaSocialMatchComments(matchId) as DotaSocialFeedComment[];
    }

    postComment(feedEventId: bigint, comment: string): boolean {
        return dotaSocialFeedPostComment(feedEventId, comment) as boolean;
    }

    postMatchComment(matchId: bigint, comment: string): boolean {
        return dotaSocialMatchPostComment(matchId, comment) as boolean;
    }
}

class GcDotaChatService implements DotaChatService {
    all(): DotaChatChannelSummary[] {
        return dotaChatChannels() as DotaChatChannelSummary[];
    }

    join(channelName: string, channelType: number): DotaChatChannel | null {
        return dotaChatJoinChannel(channelName, channelType) as DotaChatChannel | null;
    }

    get(channelId: bigint): DotaChatChannel | null {
        return dotaChatChannel(channelId) as DotaChatChannel | null;
    }

    leave(channelId: bigint): boolean {
        return dotaChatLeaveChannel(channelId) as boolean;
    }

    broadcast(channelId: bigint, messageType: number, payload: Uint8Array, includeSelf: boolean = true): number {
        return dotaChatBroadcast(channelId, messageType, payload, includeSelf) as number;
    }
}

class GcDotaGuildService implements DotaGuildService {
    ensureCurrent(): DotaGuild {
        return dotaGuildEnsureCurrent() as DotaGuild;
    }

    getMembership(accountId: number = 0): DotaGuildMembership {
        return dotaGuildMembership(accountId) as DotaGuildMembership;
    }

    getGuild(guildId: number): DotaGuild | null {
        return dotaGuild(guildId) as DotaGuild | null;
    }

    getPersonaInfo(accountId: number): DotaGuildPersona[] {
        return dotaGuildPersonaInfo(accountId) as DotaGuildPersona[];
    }

    getEventData(guildId: number, eventId: number): DotaGuildEventData {
        return dotaGuildEventData(guildId, eventId) as DotaGuildEventData;
    }

    invite(guildId: number, targetAccountId: number): number {
        return dotaGuildInvite(guildId, targetAccountId);
    }

    declineInvite(guildId: number): number {
        return dotaGuildDeclineInvite(guildId);
    }

    cancelInvite(guildId: number, targetAccountId: number): number {
        return dotaGuildCancelInvite(guildId, targetAccountId);
    }

    acceptInvite(guildId: number): number {
        return dotaGuildAcceptInvite(guildId);
    }

    leave(guildId: number): number {
        return dotaGuildLeave(guildId);
    }

    getReporterUpdates(): DotaReporterUpdates {
        return dotaReporterUpdates() as DotaReporterUpdates;
    }

    acknowledgeReporterUpdates(matchIds: bigint[]): boolean {
        return dotaAcknowledgeReporterUpdates(matchIds);
    }
}

class GcDotaMatchService implements DotaMatchService {
    recordSignOutPermission(request: DotaMatchSignOutPermissionAudit): boolean {
        return dotaRecordMatchSignOutPermission(request);
    }

    setHistoryAccess(allow: boolean): boolean {
        return dotaSetMatchHistoryAccess(allow);
    }

    recordServerStatus(response: number): boolean {
        return dotaRecordServerStatus(response);
    }

    recordLeaver(event: DotaLeaverEvent): boolean {
        return dotaRecordLeaver(event);
    }

    recordRealtimeStats(snapshot: DotaRealtimeStatsSnapshot): boolean {
        return dotaRecordRealtimeStats(snapshot);
    }

    recordMatchStateHistory(history: DotaMatchStateHistorySnapshot): boolean {
        return dotaRecordMatchStateHistory(history);
    }

    recordSpectatorCount(spectatorCount: number): boolean {
        return dotaRecordSpectatorCount(spectatorCount);
    }

    recordLiveScoreboard(snapshot: DotaLiveScoreboardSnapshot): boolean {
        return dotaRecordLiveScoreboard(snapshot);
    }

    savePlayerReport(report: DotaPlayerReport): boolean {
        return dotaSavePlayerReport(report);
    }
}

class GcDotaPartyService implements DotaPartyService {
    getCurrent(): DotaPartyState | null {
        return dotaPartyCurrent() as DotaPartyState | null;
    }

    getById(partyId: bigint): DotaPartyState | null {
        return dotaPartyById(partyId) as DotaPartyState | null;
    }

    ensureCurrent(ping: DotaPartyPingData): DotaPartyState {
        return dotaPartyEnsureCurrent(ping) as DotaPartyState;
    }

    addMember(partyId: bigint, ping: DotaPartyPingData, isCoach: boolean): DotaPartyState | null {
        return dotaPartyAddMember(partyId, ping, isCoach) as DotaPartyState | null;
    }

    removeMember(partyId: bigint, steamId: bigint): DotaPartyState | null {
        return dotaPartyRemoveMember(partyId, steamId) as DotaPartyState | null;
    }

    deleteParty(partyId: bigint): boolean {
        return dotaPartyDelete(partyId);
    }

    setLeader(partyId: bigint, leaderSteamId: bigint): DotaPartyState | null {
        return dotaPartySetLeader(partyId, leaderSteamId) as DotaPartyState | null;
    }

    setMemberCoach(partyId: bigint, steamId: bigint, isCoach: boolean): DotaPartyState | null {
        return dotaPartySetCoach(partyId, steamId, isCoach) as DotaPartyState | null;
    }

    setMemberPing(partyId: bigint, steamId: bigint, ping: DotaPartyPingData): DotaPartyState | null {
        return dotaPartySetPing(partyId, steamId, ping) as DotaPartyState | null;
    }

    startReadyCheck(partyId: bigint, durationSeconds: number): DotaPartyState | null {
        return dotaPartyStartReadyCheck(partyId, durationSeconds) as DotaPartyState | null;
    }

    acknowledgeReadyCheck(partyId: bigint, readyStatus: number): DotaPartyState | null {
        return dotaPartyAcknowledgeReadyCheck(partyId, readyStatus) as DotaPartyState | null;
    }

    createInvite(
        partyId: bigint,
        targetSteamId: bigint,
        teamId: number,
        asCoach: boolean
    ): DotaPartyInviteMutation | null {
        return dotaPartyCreateInvite(partyId, targetSteamId, teamId, asCoach) as DotaPartyInviteMutation | null;
    }

    takeInvite(partyId: bigint): DotaPartyInvite | null {
        return dotaPartyTakeInvite(partyId) as DotaPartyInvite | null;
    }

    getInvitesForTarget(targetSteamId: bigint): DotaPartyInvite[] {
        return dotaPartyInvitesForTarget(targetSteamId) as DotaPartyInvite[];
    }

    deleteInvitesForTarget(targetSteamId: bigint): DotaPartyInvite[] {
        return dotaPartyDeleteInvitesForTarget(targetSteamId) as DotaPartyInvite[];
    }

    deleteInvitesForParty(partyId: bigint): DotaPartyInvite[] {
        return dotaPartyDeleteInvitesForParty(partyId) as DotaPartyInvite[];
    }

    pruneInvitesCreatedBefore(cutoff: number): DotaPartyInvite[] {
        return dotaPartyPruneInvitesCreatedBefore(cutoff) as DotaPartyInvite[];
    }

    userExists(steamId: bigint): boolean {
        return dotaPartyUserExists(steamId);
    }

    userOnline(steamId: bigint): boolean {
        return dotaPartyUserOnline(steamId);
    }

    queueMessage(steamId: bigint, messageType: number, payload: Uint8Array, protobuf = true): boolean {
        return dotaQueueGcMessage(steamId, messageType, payload, protobuf);
    }
}

class GcDotaStatsService implements DotaStatsService {
    lookupAccountName(accountId: number): DotaAccountName {
        return dotaLookupAccountName(accountId) as DotaAccountName;
    }

    getEventPoints(accountId: number, eventId: number): DotaEventPoints {
        return dotaEventPoints(accountId, eventId) as DotaEventPoints;
    }

    getHeroStandings(accountId: number): DotaHeroStanding[] {
        return dotaHeroStandings(accountId) as DotaHeroStanding[];
    }

    getHeroGlobalData(accountId: number, heroId: number): DotaHeroGlobalData {
        return dotaHeroGlobalData(accountId, heroId) as DotaHeroGlobalData;
    }

    getPlayerStats(accountId: number): DotaPlayerStats {
        return dotaPlayerStats(accountId) as DotaPlayerStats;
    }

    getRank(accountId: number): DotaRank {
        return dotaRank(accountId) as DotaRank;
    }

    getTeammateStats(accountId: number): DotaTeammateStats[] {
        return dotaTeammateStats(accountId) as DotaTeammateStats[];
    }

    getMatchHistory(
        accountId: number,
        startAtMatchId: bigint,
        requested: number,
        heroId: number,
        includePractice: boolean
    ): DotaMatchPlayer[] {
        return dotaMatchHistory(accountId, startAtMatchId, requested, heroId, includePractice) as DotaMatchPlayer[];
    }

    getMatchDetails(matchId: bigint): DotaMatch | null {
        return dotaMatchDetails(matchId) as DotaMatch | null;
    }

    getHeroStatsHistory(accountId: number, heroId: number): DotaMatchPlayer[] {
        return dotaHeroStatsHistory(accountId, heroId) as DotaMatchPlayer[];
    }

    getMatchVotes(matchId: bigint): DotaMatchVoteSummary {
        return dotaMatchVotes(matchId) as DotaMatchVoteSummary;
    }

    getShowcaseStats(accountId: number): DotaShowcaseStats {
        return dotaShowcaseStats(accountId) as DotaShowcaseStats;
    }

    getRecentAccomplishments(accountId: number): DotaPlayerRecentAccomplishments {
        return dotaRecentAccomplishments(accountId) as DotaPlayerRecentAccomplishments;
    }

    getHeroRecentAccomplishments(accountId: number, heroId: number): DotaHeroRecentAccomplishments {
        return dotaHeroRecentAccomplishments(accountId, heroId) as DotaHeroRecentAccomplishments;
    }

    hasMvpVote(matchId: bigint): boolean {
        return dotaHasMvpVote(matchId);
    }

    voteForMvp(matchId: bigint, votedAccountId: number): boolean {
        return dotaVoteForMvp(matchId, votedAccountId);
    }

    finalizeMvpVote(matchId: bigint): boolean {
        return dotaFinalizeMvpVote(matchId);
    }

    submitLobbyMvpVote(targetAccountId: number): DotaLobbyMvpVoteResult {
        return dotaSubmitLobbyMvpVote(targetAccountId) as DotaLobbyMvpVoteResult;
    }

    recordSignOutMvpStats(matchId: bigint, players: DotaSignOutMvpPlayer[]): boolean {
        return dotaRecordSignOutMvpStats(matchId, players);
    }

    rerollPlayerChallenge(): boolean {
        return dotaRerollPlayerChallenge();
    }
}

class GcServiceContainer implements GcServices {
    items: DotaItemService;
    match: DotaMatchService;
    party: DotaPartyService;
    profiles: DotaProfileService;
    social: DotaSocialService;
    chat: DotaChatService;
    guilds: DotaGuildService;
    stats: DotaStatsService;
    lobby: DotaLobbyService;
    teams: DotaTeamService;

    constructor() {
        this.items = new GcDotaItemService();
        this.match = new GcDotaMatchService();
        this.party = new GcDotaPartyService();
        this.profiles = new GcDotaProfileService();
        this.social = new GcDotaSocialService();
        this.chat = new GcDotaChatService();
        this.guilds = new GcDotaGuildService();
        this.stats = new GcDotaStatsService();
        this.lobby = new GcDotaLobbyService();
        this.teams = new GcDotaTeamService();
    }
}

class GcHandlerContext<TRequest, TResponse> implements HandlerContext<TRequest, TResponse> {
    route: GcRoute<TRequest, TResponse>;
    request: TRequest;
    steamId: bigint;
    accountId: number;
    personaName: string;
    services: GcServices;
    clock: Clock;
    logger: Logger;
    signal: AbortSignal;

    constructor(route: GcRoute<TRequest, TResponse>) {
        this.route = route;
        this.request = decode(route.request.name, body()) as TRequest;
        this.steamId = currentSteamId();
        this.accountId = currentAccountId();
        this.personaName = currentPersonaName();
        this.services = new GcServiceContainer();
        this.clock = new GcClock();
        this.logger = new GcLogger();
        this.signal = new GcAbortSignal();
    }

    reply(response: TResponse): void {
        reply(this.route.responseId, encode(this.route.response.name, response), true);
    }

    send<TMessage>(messageType: number, proto: ProtoDescriptor<TMessage>, message: TMessage): void {
        send(messageType, encode(proto.name, message), true);
    }

    encode<TMessage>(proto: ProtoDescriptor<TMessage>, message: TMessage): Uint8Array {
        return encode(proto.name, message) as Uint8Array;
    }
}

class GcRawMessageContext implements RawMessageContext {
    messageType: number;
    payload: Uint8Array;
    steamId: bigint;
    accountId: number;
    personaName: string;
    services: GcServices;
    clock: Clock;
    logger: Logger;
    signal: AbortSignal;

    constructor(currentMessageType: number) {
        this.messageType = currentMessageType;
        this.payload = body() as Uint8Array;
        this.steamId = currentSteamId();
        this.accountId = currentAccountId();
        this.personaName = currentPersonaName();
        this.services = new GcServiceContainer();
        this.clock = new GcClock();
        this.logger = new GcLogger();
        this.signal = new GcAbortSignal();
    }

    reply<TMessage>(messageType: number, proto: ProtoDescriptor<TMessage>, message: TMessage): void {
        reply(messageType, encode(proto.name, message), true);
    }

    send<TMessage>(messageType: number, proto: ProtoDescriptor<TMessage>, message: TMessage): void {
        send(messageType, encode(proto.name, message), true);
    }

    encode<TMessage>(proto: ProtoDescriptor<TMessage>, message: TMessage): Uint8Array {
        return encode(proto.name, message) as Uint8Array;
    }

    decode<TMessage>(proto: ProtoDescriptor<TMessage>): TMessage {
        return decode(proto.name, this.payload) as TMessage;
    }
}

const emptyProto: ProtoDescriptor<unknown> = { name: "" };
const emptyRoute: GcRoute<unknown, unknown> = {
    requestId: 0,
    responseId: 0,
    request: emptyProto,
    response: emptyProto
};
const unregisteredRouteHandler: RouteHandler<unknown, unknown> = () => false;
const unregisteredRawHandler: RawMessageHandler = () => false;

interface RegisteredHandler {
    readonly messageId: number;
    readonly raw: boolean;
    readonly route: GcRoute<unknown, unknown>;
    readonly routeHandler: RouteHandler<unknown, unknown>;
    readonly rawHandler: RawMessageHandler;
    readonly source: string;
}

class GcRouter {
    handlers: Map<number, RegisteredHandler>;

    constructor() {
        this.handlers = new Map<number, RegisteredHandler>();
    }

    on<TRequest, TResponse>(route: GcRoute<TRequest, TResponse>, handler: RouteHandler<TRequest, TResponse>): void {
        this.register({
            messageId: route.requestId,
            raw: false,
            route: route as GcRoute<unknown, unknown>,
            routeHandler: handler as RouteHandler<unknown, unknown>,
            rawHandler: unregisteredRawHandler,
            source: route.request.name
        });
    }

    onMessage(messageId: number, handler: RawMessageHandler): void {
        this.register({
            messageId,
            raw: true,
            route: emptyRoute,
            routeHandler: unregisteredRouteHandler,
            rawHandler: handler,
            source: "raw message " + messageId
        });
    }

    async dispatch(): Promise<boolean> {
        const current = messageType();
        if (!this.handlers.has(current)) {
            return false;
        }

        const registration = this.handlers.get(current) as RegisteredHandler;
        try {
            let result: HandlerResult = true;
            if (registration.raw) {
                result = registration.rawHandler(this.createRawContext(current));
            } else {
                result = registration.routeHandler(this.createContext(registration.route));
            }

            const resolved = await result;
            if (resolved === false) {
                return false;
            }
            return true;
        } catch (error) {
            this.logDispatchError(current, registration, String(error));
            throw error;
        }
    }

    private register(registration: RegisteredHandler): void {
        if (this.handlers.has(registration.messageId)) {
            throw new Error(
                "Duplicate GC handler for message " +
                    registration.messageId +
                    " while registering " +
                    registration.source
            );
        }

        this.handlers.set(registration.messageId, registration);
    }

    private logDispatchError(messageId: number, registration: RegisteredHandler, error: string): void {
        log(
            "GC handler failed. messageId=" +
                messageId +
                " source=" +
                registration.source +
                " steamId=" +
                currentSteamId() +
                " error=" +
                error
        );
    }

    createContext<TRequest, TResponse>(route: GcRoute<TRequest, TResponse>): HandlerContext<TRequest, TResponse> {
        return new GcHandlerContext(route);
    }

    createRawContext(currentMessageType: number): RawMessageContext {
        return new GcRawMessageContext(currentMessageType);
    }
}

export const gc = new GcRouter();
