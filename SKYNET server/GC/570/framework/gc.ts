import { GcRoute, ProtoDescriptor } from "../generated/dota";

export type HandlerResult = void | boolean | Promise<void | boolean>;
export type RouteHandler<TRequest, TResponse> = (ctx: HandlerContext<TRequest, TResponse>) => HandlerResult;
export type RawMessageHandler = (ctx: RawMessageContext) => HandlerResult;

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
    readonly profiles: DotaProfileService;
    readonly social: DotaSocialService;
    readonly stats: DotaStatsService;
}

export interface DotaItemService {
    getInventory(): DotaRuntimeInventory;
    getCatalogItem(defIndex: number): DotaCatalogItem | null;
    equipItem(itemId: bigint, heroId: number, slotId: number, style: number): DotaEquipment[];
    setItemStyle(itemId: bigint, style: number): DotaEquipment[];
    queueCurrentLobbyServer(messageType: number, payload: Uint8Array): boolean;
}

export interface DotaRuntimeInventory {
    readonly steamId: bigint;
    readonly version: bigint;
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

export interface DotaProfileService {
    get(accountId: number): DotaProfileSnapshot;
    saveCardSlots(slots: DotaProfileSlot[]): void;
    saveProfileUpdate(backgroundItemId: bigint, featuredHeroIds: number[]): void;
}

export interface DotaSocialService {
    feed(accountId: number, selfOnly: boolean): DotaSocialFeedEvent[];
    comments(feedEventId: bigint): DotaSocialFeedComment[];
    postComment(feedEventId: bigint, comment: string): boolean;
}

export interface DotaStatsService {
    getEventPoints(accountId: number, eventId: number): DotaEventPoints;
    getHeroStandings(accountId: number): DotaHeroStanding[];
    getRank(accountId: number): DotaRank;
}

export interface DotaProfileSnapshot {
    readonly accountId: number;
    readonly steamId: bigint;
    readonly personaName: string;
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

export interface DotaRank {
    readonly result: number;
    readonly rankValue: number;
    readonly rankData1: number;
    readonly rankData2: number;
    readonly rankData3: number;
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
    getInventory(): DotaRuntimeInventory {
        return dotaInventory() as DotaRuntimeInventory;
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

    queueCurrentLobbyServer(messageType: number, payload: Uint8Array): boolean {
        return dotaQueueCurrentLobbyServer(messageType, payload) as boolean;
    }
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
}

class GcDotaSocialService implements DotaSocialService {
    feed(accountId: number, selfOnly: boolean): DotaSocialFeedEvent[] {
        return dotaSocialFeed(accountId, selfOnly) as DotaSocialFeedEvent[];
    }

    comments(feedEventId: bigint): DotaSocialFeedComment[] {
        return dotaSocialFeedComments(feedEventId) as DotaSocialFeedComment[];
    }

    postComment(feedEventId: bigint, comment: string): boolean {
        return dotaSocialFeedPostComment(feedEventId, comment) as boolean;
    }
}

class GcDotaStatsService implements DotaStatsService {
    getEventPoints(accountId: number, eventId: number): DotaEventPoints {
        return dotaEventPoints(accountId, eventId) as DotaEventPoints;
    }

    getHeroStandings(accountId: number): DotaHeroStanding[] {
        return dotaHeroStandings(accountId) as DotaHeroStanding[];
    }

    getRank(accountId: number): DotaRank {
        return dotaRank(accountId) as DotaRank;
    }
}

class GcServiceContainer implements GcServices {
    items: DotaItemService;
    profiles: DotaProfileService;
    social: DotaSocialService;
    stats: DotaStatsService;

    constructor() {
        this.items = new GcDotaItemService();
        this.profiles = new GcDotaProfileService();
        this.social = new GcDotaSocialService();
        this.stats = new GcDotaStatsService();
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
        send(this.route.responseId, encode(this.route.response.name, response), true);
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
