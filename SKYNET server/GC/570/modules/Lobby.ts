import {
    DotaLobbyMatchPlayer,
    DotaLobbyMatchSnapshot,
    DotaLobbyService,
    DotaTeam,
    DotaPlayerRecentMatchInfo,
    GcContextBase,
    HandlerContext,
    RawMessageContext,
    gc
} from "../framework/gc";
import {
    CLobbyTeamDetails,
    CMsgApplyTeamToPracticeLobby,
    CMsgClientWelcome,
    CMsgConnectedPlayers,
    CMsgDOTAPlayerFailedToConnect,
    CMsgDOTADestroyLobbyRequest,
    CMsgDOTADestroyLobbyResponse,
    CMsgDOTADestroyLobbyResponse_Result,
    CMsgDOTAPopup,
    CMsgDOTAPopup_PopupID,
    CMsgFriendPracticeLobbyListRequest,
    CMsgFriendPracticeLobbyListResponse,
    CMsgGCToClientMatchSignedOut,
    CMsgGCToServerRealtimeStatsStartStop,
    CMsgGameMatchSignOut,
    CMsgGameMatchSignoutResponse,
    CMsgGameServerInfo,
    CMsgGenericResult,
    CMsgInviteToLobby,
    CMsgInvitationCreated,
    CMsgLANServerAvailable,
    CMsgLobbyInviteResponse,
    CMsgLobbyList,
    CMsgLobbyListResponse,
    CMsgPracticeLobbyKick,
    CMsgPracticeLobbyKickFromTeam,
    CMsgPracticeLobbyCreate,
    CMsgPracticeLobbyJoin,
    CMsgPracticeLobbyJoinResponse,
    CMsgPracticeLobbyLaunch,
    CMsgPracticeLobbyList,
    CMsgPracticeLobbyListResponse,
    CMsgPracticeLobbyListResponseEntry,
    CMsgPracticeLobbySetCoach,
    CMsgPracticeLobbySetDetails,
    CMsgPracticeLobbySetTeamSlot,
    CMsgPlayerRecentMatchInfo,
    CMsgRecentAccomplishments,
    CMsgSOCacheSubscribed,
    CMsgSOCacheSubscribed_SubscribedType,
    CMsgSOCacheUnsubscribed,
    CMsgSOMultipleObjects,
    CMsgSOMultipleObjects_SingleObject,
    CMsgSOSingleObject,
    CMsgServerToGCRequestBatchPlayerResources,
    CMsgServerToGCRequestBatchPlayerResourcesResponse,
    CMsgServerToGCRequestBatchPlayerResourcesResponse_Result,
    CMsgServerToGCRequestPlayerRecentAccomplishments,
    CMsgServerToGCRequestPlayerRecentAccomplishmentsResponse,
    CSODOTALobby,
    CSODOTALobbyInvite,
    CSODOTALobbyMember,
    CSODOTAServerLobby,
    CSODOTAServerStaticLobby,
    CSODOTAStaticLobby,
    DOTABotDifficulty,
    DOTALobbyVisibility,
    DOTASelectionPriorityRules,
    LobbyDotaPauseSetting,
    LobbyDotaTVDelay,
    Msg,
    Proto,
    ProtoDescriptor,
    Routes
} from "../generated/dota";
import { buildEconSoCacheSubscribedForInventory, buildGameOwnerSoCacheSubscribedForInventory } from "./InventorySos";
import { normalizeConduct } from "./shared/conduct";

const LOBBY_OBJECT_TYPE_ID = 2004;
const LOBBY_INVITE_OBJECT_TYPE_ID = 2013;
const LOBBY_STATIC_OBJECT_TYPE_ID = 2014;
const LOBBY_SERVER_OBJECT_TYPE_ID = 2015;
const LOBBY_SERVER_STATIC_OBJECT_TYPE_ID = 2016;
const LOBBY_OWNER_TYPE = 3;
const LOBBY_INVITE_OWNER_TYPE = 4;
const LOBBY_SERVICE_ID = 0;
const WELCOME_VERSION = 20;
// Dedicated servers accept the GC welcome only when it carries the current
// server-side welcome version observed from the working coordinator flow.
const GAME_SERVER_WELCOME_VERSION = 6860;

const TEAM_GOOD = 0;
const TEAM_BAD = 1;
const TEAM_SPECTATOR = 3;
const TEAM_POOL = 4;
const TEAM_NONE = 5;

const LOBBY_UI = 0;
const LOBBY_SERVER_SETUP = 1;
const LOBBY_RUN = 2;
const GAME_INIT = 0;
const GAME_HERO_SELECTION = 2;

const CONNECTED_REASON_GAME_STATE = 2;
const CONNECTED_REASON_PLAYER_HERO = 5;
const LEAVER_NONE = 0;
const LEAVER_DISCONNECTED = 1;

const JOIN_SUCCESS = 0;
const JOIN_ALREADY_IN_GAME = 1;
const JOIN_INVALID_LOBBY = 2;
const JOIN_BAD_PASSWORD = 3;
const JOIN_FULL = 9;

const MAX_MEMBERS = 20;
const LOBBY_TIMEOUT_SECONDS = 3600;
const POSTGAME_CLEANUP_SECONDS = 45;
const DEFAULT_PORT = 27015;
const DEFAULT_GAME_MODE = 1;
const DEFAULT_LOBBY_NAME = "Room 1";
const DEFAULT_SERVER_REGION = 0;
const DEFAULT_CM_PICK = 0;
// Lobby extra message 8821 is the selection-priority rule payload Dota expects
// in the initial lobby SO cache. Omitting it leaves current clients with a
// visibly incomplete lobby setup even though the protobuf decodes.
const LOBBY_EXTRA_MESSAGE_ID = 8821;
const LOBBY_EXTRA_MESSAGE_CONTENTS = new Uint8Array([8, 0]);
const DEFAULT_SERVER_STATIC_BANNED_HERO_IDS = [75, 0, 0, 0];
// The generated Msg map does not expose legacy popup 7102, but the proto does.
// Lobby kick uses it exactly like the old GC before unsubscribing the target.
const DOTA_POPUP_MESSAGE_ID = 7102;
const SUCCESS = 1;
const FAILURE = 0;

interface LobbyState {
    lobbyId: bigint;
    leaderSteamId: bigint;
    leaderAccountId: number;
    version: bigint;
    createdAt: number;
    updatedAt: number;
    state: number;
    gameState: number;
    gameMode: number;
    gameName: string;
    serverRegion: number;
    cmPick: number;
    lan: boolean;
    allowCheats: boolean;
    fillWithBots: boolean;
    allowSpectating: boolean;
    passKey: string;
    leagueId: number;
    seriesType: number;
    radiantSeriesWins: number;
    direSeriesWins: number;
    allchat: boolean;
    dotaTvDelay: number;
    customGameMode: string;
    customMapName: string;
    customDifficulty: number;
    customGameId: bigint;
    customMinPlayers: number;
    customMaxPlayers: number;
    customGameCrc: bigint;
    customGameTimestamp: number;
    customGamePenalties: boolean;
    visibility: number;
    pauseSetting: number;
    selectionPriorityRules: number;
    botDifficultyRadiant: number;
    botDifficultyDire: number;
    numSpectators: number;
    teamDetails: CLobbyTeamDetails[];
    requestedHeroIds: number[];
    requestedHeroTeams: number[];
    serverSteamId: bigint;
    serverPort: number;
    serverPublicIp: string;
    serverPrivateIp: string;
    connect: string;
    matchId: bigint;
    gameStartTime: number;
    realtimeStatsStartStopSent: boolean;
    playerItemsServerSteamId: bigint;
    members: LobbyMemberState[];
}

interface LobbyMemberState {
    steamId: bigint;
    accountId: number;
    personaName: string;
    team: number;
    slot: number;
    coachTeam: number;
    heroId: number;
    leaverStatus: number;
    connectedOnce: boolean;
    lastSeen: number;
}

interface LobbyInviteState {
    inviteId: bigint;
    lobbyId: bigint;
    targetSteamId: bigint;
    senderSteamId: bigint;
    senderName: string;
    createdAt: number;
}

interface LobbyStore {
    lobbies: Map<bigint, LobbyState>;
    bySteam: Map<bigint, bigint>;
    byServer: Map<bigint, bigint>;
    invites: Map<bigint, LobbyInviteState>;
    nextId: bigint;
    nextMatchId: bigint;
}

const store: LobbyStore = {
    lobbies: new Map<bigint, LobbyState>(),
    bySteam: new Map<bigint, bigint>(),
    byServer: new Map<bigint, bigint>(),
    invites: new Map<bigint, LobbyInviteState>(),
    nextId: 0n,
    nextMatchId: 0n
};

export function registerLobby(): Lobby {
    const lobby = new Lobby();
    lobby.register();
    return lobby;
}

export function emitCurrentLobby<TRequest, TResponse>(ctx: HandlerContext<TRequest, TResponse>): void {
    const lobby = currentLobby(ctx.steamId);
    if (lobby !== null) {
        subscribeToLobby(ctx, ctx.steamId, lobby);
    }

    emitCurrentLobbyInvites(ctx);
}

export function queueCurrentLobbyServer<TRequest, TResponse>(
    ctx: HandlerContext<TRequest, TResponse>,
    messageType: number,
    payload: Uint8Array
): boolean {
    const lobby = currentLobby(ctx.steamId);
    if (lobby === null || lobby.serverSteamId === 0n) {
        return false;
    }

    return ctx.services.lobby.queueMessage(lobby.serverSteamId, messageType, payload, true);
}

export class Lobby {
    register(): void {
        gc.on(Routes.PracticeLobbyCreate, (ctx) => this.createLobby(ctx));
        gc.onMessage(Msg.GCAbandonCurrentGame, (ctx) => this.abandonCurrentGame(ctx));
        gc.onMessage(Msg.GCPracticeLobbyLeave, (ctx) => this.leaveLobby(ctx));
        gc.on(Routes.PracticeLobbyJoin, (ctx) => this.joinLobby(ctx));
        gc.onMessage(Msg.GCInviteToLobby, (ctx) => this.inviteToLobby(ctx));
        gc.onMessage(Msg.GCLobbyInviteResponse, (ctx) => this.lobbyInviteResponse(ctx));
        gc.on(Routes.PracticeLobbyList, (ctx) => this.practiceLobbyList(ctx));
        gc.on(Routes.FriendPracticeLobbyList, (ctx) => this.friendPracticeLobbyList(ctx));
        gc.onMessage(Msg.GCLobbyList, (ctx) => this.lobbyList(ctx));
        gc.on(Routes.PracticeLobbySetDetails, (ctx) => this.setDetails(ctx));
        gc.on(Routes.PracticeLobbySetTeamSlot, (ctx) => this.setTeamSlot(ctx));
        gc.on(Routes.PracticeLobbySetCoach, (ctx) => this.setCoach(ctx));
        gc.on(Routes.ApplyTeamToPracticeLobby, (ctx) => this.applyTeam(ctx));
        gc.onMessage(Msg.GCPracticeLobbyKick, (ctx) => this.kickLobbyMember(ctx));
        gc.onMessage(Msg.GCPracticeLobbyKickFromTeam, (ctx) => this.kickLobbyMemberFromTeam(ctx));
        gc.onMessage(Msg.GCBalancedShuffleLobby, (ctx) => this.balancedShuffleLobby(ctx));
        gc.onMessage(Msg.GCFlipLobbyTeams, (ctx) => this.flipLobbyTeams(ctx));
        gc.onMessage(Msg.GCClearPracticeLobbyTeam, (ctx) => this.clearPracticeLobbyTeam(ctx));
        gc.onMessage(Msg.DestroyLobbyRequest, (ctx) => this.destroyLobbyRequest(ctx));
        gc.onMessage(Msg.ClientToGCRequestSteamDatagramTicket, (ctx) => this.steamDatagramTicket(ctx));
        gc.on(Routes.PracticeLobbyLaunch, (ctx) => this.launchLobby(ctx));
        gc.onMessage(Msg.GCGameServerHello, (ctx) => this.gameServerHello(ctx));
        gc.onMessage(Msg.GCGameServerInfo, (ctx) => this.gameServerInfo(ctx));
        gc.onMessage(Msg.GCLANServerAvailable, (ctx) => this.lanServerAvailable(ctx));
        gc.onMessage(Msg.GCServerAvailable, (ctx) => this.serverAvailable(ctx));
        gc.onMessage(Msg.GCConnectedPlayers, (ctx) => this.connectedPlayers(ctx));
        gc.onMessage(Msg.GCPlayerFailedToConnect, (ctx) => this.playerFailedToConnect(ctx));
        gc.onMessage(Msg.GCGameMatchSignOut, (ctx) => this.signOut(ctx));
        gc.onMessage(Msg.GCGameBotMatchSignOut, (ctx) => this.signOut(ctx));
        gc.on(Routes.RequestBatchPlayerResources, (ctx) => this.requestBatchPlayerResources(ctx));
        gc.on(Routes.ServerRequestPlayerRecentAccomplishments, (ctx) => this.recentAccomplishments(ctx));
        gc.onMessage(Msg.ServerToGCLobbyInitialized, (ctx) => this.lobbyInitialized(ctx));
    }

    private createLobby(ctx: HandlerContext<CMsgPracticeLobbyCreate, CMsgGenericResult>): boolean {
        leaveCurrent(ctx, 0n);
        const lobby = createLobbyState(ctx);
        applyCreateRequest(lobby, ctx.request);
        const member = ensureMember(lobby, ctx.steamId, ctx.accountId, ctx.personaName, ctx.clock.now(), ctx);
        member.team = TEAM_GOOD;
        member.slot = 1;
        member.coachTeam = TEAM_NONE;
        refreshLobby(lobby, ctx.clock.now());

        subscribeToLobby(ctx, ctx.steamId, lobby);
        publishLobby(ctx.services.lobby, lobby);
        ctx.reply({ eresult: SUCCESS });
        return true;
    }

    private abandonCurrentGame(ctx: RawMessageContext): boolean {
        leaveCurrent(ctx, 0n);
        return true;
    }

    private leaveLobby(ctx: RawMessageContext): boolean {
        if (currentLobby(ctx.steamId) === null) {
            leavePublishedLobbyState(ctx);
        } else {
            leaveCurrent(ctx, 0n);
        }

        return true;
    }

    private joinLobby(ctx: HandlerContext<CMsgPracticeLobbyJoin, CMsgPracticeLobbyJoinResponse>): boolean {
        const lobbyId = ctx.request.lobbyId ?? 0n;
        const passKey = ctx.request.passKey ?? "";
        const lobby = store.lobbies.get(lobbyId) ?? null;
        let result = JOIN_INVALID_LOBBY;

        if (lobby === null) {
            result = JOIN_INVALID_LOBBY;
        } else if (lobby.state !== LOBBY_UI) {
            result = JOIN_ALREADY_IN_GAME;
        } else if (lobby.passKey !== "" && lobby.passKey !== passKey) {
            result = JOIN_BAD_PASSWORD;
        } else if (lobby.members.length >= MAX_MEMBERS && findMember(lobby, ctx.steamId) === null) {
            result = JOIN_FULL;
        } else {
            leaveCurrent(ctx, lobby.lobbyId);
            const member = ensureMember(lobby, ctx.steamId, ctx.accountId, ctx.personaName, ctx.clock.now(), ctx);
            if (member.steamId !== lobby.leaderSteamId) {
                member.team = TEAM_POOL;
                member.slot = 0;
                member.coachTeam = TEAM_NONE;
            }

            refreshLobby(lobby, ctx.clock.now());
            subscribeToLobbyObjectOnly(ctx, ctx.steamId, lobby);
            ctx.reply({ result: JOIN_SUCCESS });
            // Joiner already got the full lobby via subscribe + single object;
            // notify the existing members (incl. leader) so they see the new
            // player. Excluding the joiner avoids a redundant 26 on top of 24/21.
            broadcastLobby(ctx, lobby, ctx.steamId, false);
            publishLobby(ctx.services.lobby, lobby);
            return true;
        }

        ctx.reply({ result });
        return true;
    }

    private inviteToLobby(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgInviteToLobby) as CMsgInviteToLobby;
        const lobby = currentLobby(ctx.steamId);
        const targetSteamId = request.steamId ?? 0n;
        let userOffline = true;

        if (lobby !== null && targetSteamId !== 0n) {
            const invite = createInvite(lobby, ctx, targetSteamId);
            sendInvite(ctx, invite);
            userOffline = false;
        }

        ctx.reply<CMsgInvitationCreated>(Msg.GCInvitationCreated, Proto.CMsgInvitationCreated, {
            groupId: lobby?.lobbyId ?? 0n,
            steamId: targetSteamId,
            userOffline
        });
        return true;
    }

    private lobbyInviteResponse(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgLobbyInviteResponse) as CMsgLobbyInviteResponse;
        const lobbyId = request.lobbyId ?? 0n;
        const accepted = request.accept ?? false;
        const invite = takeInvite(lobbyId, ctx.steamId);
        if (invite !== null) {
            sendTo(ctx, ctx.steamId, Msg.SOCacheUnsubscribed, buildInviteUnsubscribed(invite.lobbyId));
        }

        if (!accepted || lobbyId === 0n) {
            return true;
        }

        const lobby = store.lobbies.get(lobbyId) ?? null;
        if (lobby === null) {
            return true;
        }

        joinLobbyState(ctx, lobby, "", true);
        return true;
    }

    private practiceLobbyList(ctx: HandlerContext<CMsgPracticeLobbyList, CMsgPracticeLobbyListResponse>): boolean {
        ctx.reply({ lobbies: listLobbies(ctx.services.lobby, ctx.request.region ?? 0, ctx.request.gameMode ?? 0, ctx.steamId) });
        return true;
    }

    private friendPracticeLobbyList(
        ctx: HandlerContext<CMsgFriendPracticeLobbyListRequest, CMsgFriendPracticeLobbyListResponse>
    ): boolean {
        ctx.reply({ lobbies: listLobbies(ctx.services.lobby, 0, 0, ctx.steamId) });
        return true;
    }

    private lobbyList(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgLobbyList) as CMsgLobbyList;
        ctx.reply<CMsgLobbyListResponse>(Msg.GCLobbyListResponse, Proto.CMsgLobbyListResponse, {
            lobbies: listLobbies(ctx.services.lobby, request.serverRegion ?? 0, request.gameMode ?? 0, ctx.steamId)
        });
        return true;
    }

    private setDetails(ctx: HandlerContext<CMsgPracticeLobbySetDetails, CMsgGenericResult>): boolean {
        const lobby = currentLobby(ctx.steamId);
        if (lobby === null) {
            ctx.reply({ eresult: FAILURE });
            return true;
        }

        applyDetails(lobby, ctx.request);
        refreshLobby(lobby, ctx.clock.now());
        broadcastLobby(ctx, lobby, 0n, false);
        publishLobby(ctx.services.lobby, lobby);
        ctx.reply({ eresult: SUCCESS });
        return true;
    }

    private setTeamSlot(ctx: HandlerContext<CMsgPracticeLobbySetTeamSlot, CMsgGenericResult>): boolean {
        const lobby = currentLobby(ctx.steamId);
        const member = lobby === null ? null : findMember(lobby, ctx.steamId);
        if (lobby === null || member === null) {
            ctx.reply({ eresult: FAILURE });
            return true;
        }

        const nextTeam = ctx.request.team ?? member.team;
        const nextSlot = ctx.request.slot ?? member.slot;
        if (nextTeam !== TEAM_POOL && slotOccupied(lobby, ctx.steamId, nextTeam, nextSlot)) {
            ctx.reply({ eresult: FAILURE });
            return true;
        }

        member.team = nextTeam;
        member.slot = nextTeam === TEAM_POOL ? 0 : nextSlot;
        member.coachTeam = TEAM_NONE;
        member.lastSeen = ctx.clock.now();
        refreshLobby(lobby, ctx.clock.now());
        broadcastLobby(ctx, lobby, 0n, false);
        publishLobby(ctx.services.lobby, lobby);
        ctx.reply({ eresult: SUCCESS });
        return true;
    }

    private setCoach(ctx: HandlerContext<CMsgPracticeLobbySetCoach, CMsgGenericResult>): boolean {
        const lobby = currentLobby(ctx.steamId);
        const member = lobby === null ? null : findMember(lobby, ctx.steamId);
        if (lobby === null || member === null) {
            ctx.reply({ eresult: FAILURE });
            return true;
        }

        const coachTeam = ctx.request.team ?? TEAM_NONE;
        if (coachTeam === TEAM_NONE) {
            member.coachTeam = TEAM_NONE;
            member.team = TEAM_POOL;
            member.slot = 0;
        } else {
            member.coachTeam = coachTeam;
            member.team = TEAM_SPECTATOR;
            member.slot = 1;
        }

        member.lastSeen = ctx.clock.now();
        refreshLobby(lobby, ctx.clock.now());
        broadcastLobby(ctx, lobby, 0n, false);
        publishLobby(ctx.services.lobby, lobby);
        ctx.reply({ eresult: SUCCESS });
        return true;
    }

    private applyTeam(ctx: HandlerContext<CMsgApplyTeamToPracticeLobby, CMsgGenericResult>): boolean {
        const lobby = currentLobby(ctx.steamId);
        if (lobby !== null) {
            applyTeamToLobby(ctx, lobby);
            refreshLobby(lobby, ctx.clock.now());
            broadcastLobby(ctx, lobby, 0n, true);
            publishLobby(ctx.services.lobby, lobby);
        }

        ctx.reply({ eresult: SUCCESS });
        return true;
    }

    private kickLobbyMember(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgPracticeLobbyKick) as CMsgPracticeLobbyKick;
        kickMemberByAccountId(ctx, request.accountId ?? 0);
        return true;
    }

    private kickLobbyMemberFromTeam(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgPracticeLobbyKickFromTeam) as CMsgPracticeLobbyKickFromTeam;
        kickMemberByAccountId(ctx, request.accountId ?? 0);
        return true;
    }

    private balancedShuffleLobby(ctx: RawMessageContext): boolean {
        const lobby = currentLobby(ctx.steamId);
        if (lobby === null) {
            return true;
        }

        shuffleLobbyTeams(lobby);
        refreshLobby(lobby, ctx.clock.now());
        broadcastLobby(ctx, lobby, 0n, false);
        publishLobby(ctx.services.lobby, lobby);
        return true;
    }

    private flipLobbyTeams(ctx: RawMessageContext): boolean {
        const lobby = currentLobby(ctx.steamId);
        if (lobby === null) {
            return true;
        }

        flipLobbyTeams(lobby);
        refreshLobby(lobby, ctx.clock.now());
        broadcastLobby(ctx, lobby, 0n, false);
        publishLobby(ctx.services.lobby, lobby);
        return true;
    }

    private clearPracticeLobbyTeam(ctx: RawMessageContext): boolean {
        const lobby = currentLobby(ctx.steamId);
        const member = lobby === null ? null : findMember(lobby, ctx.steamId);
        if (lobby === null || member === null) {
            return true;
        }

        clearTeamDetailForMember(lobby, member);
        refreshLobby(lobby, ctx.clock.now());
        broadcastLobby(ctx, lobby, 0n, false);
        publishLobby(ctx.services.lobby, lobby);
        return true;
    }

    private destroyLobbyRequest(ctx: RawMessageContext): boolean {
        ctx.decode(Proto.CMsgDOTADestroyLobbyRequest) as CMsgDOTADestroyLobbyRequest;
        const lobby = currentLobby(ctx.steamId);
        ctx.reply<CMsgDOTADestroyLobbyResponse>(Msg.DestroyLobbyResponse, Proto.CMsgDOTADestroyLobbyResponse, {
            result:
                lobby === null
                    ? CMsgDOTADestroyLobbyResponse_Result.ErrorUnknown
                    : CMsgDOTADestroyLobbyResponse_Result.Success
        });

        if (lobby !== null) {
            destroyLobby(lobby, ctx, "destroy-request");
        }

        return true;
    }

    private steamDatagramTicket(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgClientToGCRequestSteamDatagramTicket);
        const serverSteamId = request.serverSteamId ?? 0n;
        const lobby = serverSteamId === 0n ? null : serverLobby(serverSteamId);
        ctx.logger.info(
            "Lobby: SteamDatagramTicket request server=" +
                serverSteamId +
                " found=" +
                (lobby === null ? "false" : "true")
        );
        // Legacy GC acknowledged the request through logging only. Current
        // clients also have an explicit response proto, so send an empty ticket
        // response to close the request without claiming SDR auth we do not own.
        ctx.reply(
            Msg.ClientToGCRequestSteamDatagramTicketResponse,
            Proto.CMsgClientToGCRequestSteamDatagramTicketResponse,
            {
                serializedTicket: new Uint8Array(0),
                message: lobby === null ? "server_not_found" : "ok"
            }
        );
        return true;
    }

    private launchLobby(ctx: HandlerContext<CMsgPracticeLobbyLaunch, CMsgGenericResult>): boolean {
        const lobby = currentLobby(ctx.steamId);
        if (lobby === null) {
            ctx.reply({ eresult: FAILURE });
            return true;
        }

        prepareLobbyForLaunch(ctx, lobby);
        markTeamsIncomplete(lobby);
        lobby.state = LOBBY_SERVER_SETUP;
        lobby.connect = "";
        lobby.gameStartTime = 0;
        lobby.gameState = GAME_INIT;
        lobby.realtimeStatsStartStopSent = false;
        // The region is the authoritative transport selector: 0 is the
        // owner's local/listen server, non-zero is a supervised dedicated server.
        lobby.lan = lobby.serverRegion === 0;
        refreshLobby(lobby, ctx.clock.now());
        broadcastLobby(ctx, lobby, 0n, false);
        publishLobby(ctx.services.lobby, lobby);

        let launchResult = SUCCESS;
        if (lobby.serverRegion !== 0) {
            const map = lobby.customMapName === "" ? "dota" : lobby.customMapName;
            const result = ctx.services.lobby.startDedicatedServer(lobby.lobbyId, map);
            if (result !== null && result.started) {
                lobby.serverPort = result.port;
                ctx.logger.info(
                    "dedicated started lobby=" + lobby.lobbyId + " port=" + result.port + " state=" + result.state
                );
            } else {
                lobby.state = LOBBY_UI;
                lobby.lan = true;
                launchResult = FAILURE;
                ctx.logger.info("dedicated start failed lobby=" + lobby.lobbyId + " error=" + (result?.error ?? ""));
            }

            refreshLobby(lobby, ctx.clock.now());
            broadcastLobby(ctx, lobby, 0n, false);
            publishLobby(ctx.services.lobby, lobby);
        }

        ctx.reply({ eresult: launchResult });
        return true;
    }

    private gameServerHello(ctx: RawMessageContext): boolean {
        ctx.reply<CMsgClientWelcome>(Msg.GCGameServerWelcome, Proto.CMsgClientWelcome, {
            version: GAME_SERVER_WELCOME_VERSION,
            gcSocacheFileVersion: WELCOME_VERSION
        });
        return true;
    }

    private gameServerInfo(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgGameServerInfo) as CMsgGameServerInfo;
        const lobby = serverLobby(ctx.steamId) ?? waitingLobby();
        if (lobby !== null) {
            updateServerInfo(lobby, ctx, request);
            attachServer(ctx, lobby, false);
        }

        return true;
    }

    private lanServerAvailable(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgLANServerAvailable) as CMsgLANServerAvailable;
        const lobby = store.lobbies.get(request.lobbyId ?? 0n) ?? null;
        if (lobby !== null) {
            attachServer(ctx, lobby, false);
        }

        return true;
    }

    private serverAvailable(ctx: RawMessageContext): boolean {
        const lobby = serverLobby(ctx.steamId) ?? waitingLobby();
        if (lobby !== null) {
            attachServer(ctx, lobby, true);
        }

        return true;
    }

    private lobbyInitialized(ctx: RawMessageContext): boolean {
        const lobby = serverLobby(ctx.steamId) ?? waitingLobby();
        if (lobby !== null) {
            attachServer(ctx, lobby, true);
        }

        return true;
    }

    private connectedPlayers(ctx: RawMessageContext): boolean {
        const lobby = serverLobby(ctx.steamId);
        if (lobby === null) {
            return true;
        }

        const request = ctx.decode(Proto.CMsgConnectedPlayers) as CMsgConnectedPlayers;
        const sendReason = request.sendReason ?? 0;
        const nextGameState = request.gameState ?? GAME_INIT;
        if (sendReason === CONNECTED_REASON_GAME_STATE || nextGameState !== GAME_INIT) {
            lobby.gameState = nextGameState;
        }

        if (sendReason === CONNECTED_REASON_GAME_STATE || sendReason === CONNECTED_REASON_PLAYER_HERO) {
            applyConnectedPlayers(lobby, request, ctx.clock.now());
        }

        if (lobby.gameState === GAME_HERO_SELECTION) {
            startRealtimeStats(ctx, lobby);
        }

        refreshLobby(lobby, ctx.clock.now());
        broadcastLobby(ctx, lobby, 0n, true);
        publishLobby(ctx.services.lobby, lobby);
        return true;
    }

    private playerFailedToConnect(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgDOTAPlayerFailedToConnect) as CMsgDOTAPlayerFailedToConnect;
        const lobby = serverLobby(ctx.steamId);
        if (lobby !== null) {
            const failedSteamId = firstSteamId(request.failedLoaders ?? request.abandonedLoaders ?? []);
            const member = findMember(lobby, failedSteamId);
            if (member !== null) {
                member.leaverStatus = LEAVER_DISCONNECTED;
            }

            lobby.state = LOBBY_UI;
            refreshLobby(lobby, ctx.clock.now());
            broadcastLobby(ctx, lobby, 0n, true);
            publishLobby(ctx.services.lobby, lobby);
        }

        return true;
    }

    private signOut(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgGameMatchSignOut) as CMsgGameMatchSignOut;
        const lobby = serverLobby(ctx.steamId);
        const matchId = lobby === null ? (request.matchId ?? 0n) : ensureMatchId(lobby);
        const recipients = lobby === null ? [] : lobby.members.map((member) => member.steamId);

        if (lobby !== null) {
            refreshLobby(lobby, ctx.clock.now());
            broadcastLobby(ctx, lobby, 0n, true);
            publishLobby(ctx.services.lobby, lobby);
        }

        ctx.reply<CMsgGameMatchSignoutResponse>(Msg.GCGameMatchSignOutResponse, Proto.CMsgGameMatchSignoutResponse, {
            matchId
        });

        const signedOut: CMsgGCToClientMatchSignedOut = { matchId };
        for (let i = 0; i < recipients.length; i++) {
            sendTo(ctx, recipients[i], Msg.GCToClientMatchSignedOut, signedOut);
        }

        return true;
    }

    private requestBatchPlayerResources(
        ctx: HandlerContext<
            CMsgServerToGCRequestBatchPlayerResources,
            CMsgServerToGCRequestBatchPlayerResourcesResponse
        >
    ): boolean {
        const accountIds = ctx.request.accountIds ?? [];
        const results: CMsgServerToGCRequestBatchPlayerResourcesResponse_Result[] = [];
        for (let i = 0; i < accountIds.length; i++) {
            const profile = ctx.services.profiles.get(accountIds[i]);
            const conduct = normalizeConduct(profile.conduct);
            results.push({
                accountId: accountIds[i],
                rank: profile.rankTier,
                rankCalibrated: profile.rankTier > 0,
                lowPriority: false,
                isNewPlayer: profile.lifetimeGames < 10,
                isGuidePlayer: false,
                commLevel: conduct.rawBehaviorScore,
                behaviorLevel: conduct.rawBehaviorScore,
                wins: profile.globalStats.gamesWon,
                losses: profile.globalStats.gamesLost,
                commScore: conduct.rawBehaviorScore,
                behaviorScore: conduct.rawBehaviorScore,
                rankUncertainty: 0
            });
        }

        ctx.reply({ results });
        return true;
    }

    private recentAccomplishments(
        ctx: HandlerContext<
            CMsgServerToGCRequestPlayerRecentAccomplishments,
            CMsgServerToGCRequestPlayerRecentAccomplishmentsResponse
        >
    ): boolean {
        const accountId = ctx.request.accountId ?? ctx.accountId;
        const heroId = ctx.request.heroId ?? 0;
        const player = ctx.services.stats.getRecentAccomplishments(accountId);
        const hero = ctx.services.stats.getHeroRecentAccomplishments(accountId, heroId);
        const accomplishments: CMsgRecentAccomplishments = {
            playerAccomplishments: {
                recentOutcomes: {
                    outcomes: player.recentOutcomes.outcomes,
                    matchCount: player.recentOutcomes.matchCount
                },
                totalRecord: { wins: player.totalRecord.wins, losses: player.totalRecord.losses },
                predictionStreak: player.predictionStreak,
                plusPredictionStreak: player.plusPredictionStreak,
                recentCommends: {
                    commends: player.recentCommends.commends,
                    matchCount: player.recentCommends.matchCount
                },
                firstMatchTimestamp: player.firstMatchTimestamp,
                lastMatch: mapRecentMatch(player.lastMatch),
                recentMvps: {
                    outcomes: player.recentMvps.outcomes,
                    matchCount: player.recentMvps.matchCount
                }
            },
            heroAccomplishments: {
                recentOutcomes: {
                    outcomes: hero.recentOutcomes.outcomes,
                    matchCount: hero.recentOutcomes.matchCount
                },
                totalRecord: { wins: hero.totalRecord.wins, losses: hero.totalRecord.losses },
                lastMatch: mapRecentMatch(hero.lastMatch)
            }
        };

        ctx.reply({
            result: SUCCESS,
            playerAccomplishments: accomplishments
        });
        return true;
    }

    tick(): void {
        const nowSeconds = Math.floor(new Date().getTime() / 1000);
        const expired: LobbyState[] = [];
        store.lobbies.forEach((lobby) => {
            const timeout = lobby.state === LOBBY_RUN ? LOBBY_TIMEOUT_SECONDS : POSTGAME_CLEANUP_SECONDS;
            if (nowSeconds - lobby.updatedAt > timeout) {
                expired.push(lobby);
            }
        });

        for (let i = 0; i < expired.length; i++) {
            destroyLobby(expired[i], undefined, "timeout");
        }
    }
}

function createLobbyState(ctx: HandlerContext<CMsgPracticeLobbyCreate, CMsgGenericResult>): LobbyState {
    const now = ctx.clock.now();
    const lobbyId = nextId(now);
    const lobby: LobbyState = {
        lobbyId,
        leaderSteamId: ctx.steamId,
        leaderAccountId: ctx.accountId,
        version: nextId(now),
        createdAt: now,
        updatedAt: now,
        state: LOBBY_UI,
        gameState: GAME_INIT,
        gameMode: DEFAULT_GAME_MODE,
        gameName: DEFAULT_LOBBY_NAME,
        serverRegion: DEFAULT_SERVER_REGION,
        cmPick: DEFAULT_CM_PICK,
        lan: DEFAULT_SERVER_REGION === 0,
        allowCheats: false,
        fillWithBots: false,
        allowSpectating: false,
        passKey: "",
        leagueId: 0,
        seriesType: 0,
        radiantSeriesWins: 0,
        direSeriesWins: 0,
        allchat: false,
        dotaTvDelay: LobbyDotaTVDelay.LobbyDotaTV120,
        customGameMode: "",
        customMapName: "",
        customDifficulty: 0,
        customGameId: 0n,
        customMinPlayers: 0,
        customMaxPlayers: 0,
        customGameCrc: 0n,
        customGameTimestamp: 0,
        customGamePenalties: false,
        visibility: DOTALobbyVisibility.Public,
        pauseSetting: LobbyDotaPauseSetting.Unlimited,
        selectionPriorityRules: DOTASelectionPriorityRules.KDOTASelectionPriorityRulesManual,
        botDifficultyRadiant: DOTABotDifficulty.BotDifficultyUnfair,
        botDifficultyDire: DOTABotDifficulty.BotDifficultyUnfair,
        numSpectators: 0,
        teamDetails: [],
        requestedHeroIds: [],
        requestedHeroTeams: [],
        serverSteamId: 0n,
        serverPort: DEFAULT_PORT,
        serverPublicIp: "",
        serverPrivateIp: "",
        connect: "",
        matchId: 0n,
        gameStartTime: 0,
        realtimeStatsStartStopSent: false,
        playerItemsServerSteamId: 0n,
        members: []
    };

    store.lobbies.set(lobby.lobbyId, lobby);
    ensureMember(lobby, ctx.steamId, ctx.accountId, ctx.personaName, now, ctx);
    return lobby;
}

function applyCreateRequest(lobby: LobbyState, request: CMsgPracticeLobbyCreate): void {
    lobby.passKey = request.passKey ?? lobby.passKey;
    const details = request.lobbyDetails;
    if (details !== undefined) {
        applyDetails(lobby, details);
    } else {
        // Always guarantee 2 team_details entries even when the create
        // request carries no lobby_details, matching the legacy GC.
        lobby.teamDetails = normalizeLobbyTeamDetails(lobby.teamDetails);
    }

    normalizeLobbyLocation(lobby);
    lobby.allowSpectating = false;
}

function applyDetails(lobby: LobbyState, details: CMsgPracticeLobbySetDetails): void {
    lobby.gameName = details.gameName ?? lobby.gameName;
    // Dota's client indexes team_details[0]/[1] directly for team banners;
    // the legacy GC always guarantees exactly 2 entries after applying
    // details (padding with empty/incomplete ones), never an empty array.
    lobby.teamDetails = normalizeLobbyTeamDetails(details.teamDetails ?? lobby.teamDetails);
    lobby.serverRegion = details.serverRegion ?? lobby.serverRegion;
    lobby.gameMode = details.gameMode ?? lobby.gameMode;
    lobby.cmPick = details.cmPick ?? lobby.cmPick;
    lobby.allowCheats = details.allowCheats ?? lobby.allowCheats;
    lobby.fillWithBots = details.fillWithBots ?? lobby.fillWithBots;
    lobby.allowSpectating = details.allowSpectating ?? lobby.allowSpectating;
    lobby.passKey = details.passKey ?? lobby.passKey;
    lobby.leagueId = details.leagueid ?? lobby.leagueId;
    lobby.seriesType = details.seriesType ?? lobby.seriesType;
    lobby.radiantSeriesWins = details.radiantSeriesWins ?? lobby.radiantSeriesWins;
    lobby.direSeriesWins = details.direSeriesWins ?? lobby.direSeriesWins;
    lobby.allchat = details.allchat ?? lobby.allchat;
    lobby.dotaTvDelay = details.dotaTvDelay ?? lobby.dotaTvDelay;
    lobby.lan = details.lan ?? lobby.lan;
    lobby.customGameMode = details.customGameMode ?? lobby.customGameMode;
    lobby.customMapName = details.customMapName ?? lobby.customMapName;
    lobby.customDifficulty = details.customDifficulty ?? lobby.customDifficulty;
    lobby.customGameId = details.customGameId ?? lobby.customGameId;
    lobby.customMinPlayers = details.customMinPlayers ?? lobby.customMinPlayers;
    lobby.customMaxPlayers = details.customMaxPlayers ?? lobby.customMaxPlayers;
    lobby.customGameCrc = details.customGameCrc ?? lobby.customGameCrc;
    lobby.customGameTimestamp = details.customGameTimestamp ?? lobby.customGameTimestamp;
    lobby.customGamePenalties = details.customGamePenalties ?? lobby.customGamePenalties;
    lobby.visibility = details.visibility ?? lobby.visibility;
    lobby.pauseSetting = details.pauseSetting ?? lobby.pauseSetting;
    lobby.selectionPriorityRules = details.selectionPriorityRules ?? lobby.selectionPriorityRules;
    lobby.botDifficultyRadiant = details.botDifficultyRadiant ?? lobby.botDifficultyRadiant;
    lobby.botDifficultyDire = details.botDifficultyDire ?? lobby.botDifficultyDire;
    lobby.requestedHeroIds = details.requestedHeroIds ?? lobby.requestedHeroIds;
    lobby.requestedHeroTeams = details.requestedHeroTeams ?? lobby.requestedHeroTeams;
    normalizeLobbyLocation(lobby);
}

function normalizeLobbyLocation(lobby: LobbyState): void {
    // Dota can send stale lan=true while carrying the persisted region.
    // Keep the region as source of truth so the lobby header and launch route agree.
    lobby.lan = lobby.serverRegion === 0;
}

function applyTeamToLobby(
    ctx: HandlerContext<CMsgApplyTeamToPracticeLobby, CMsgGenericResult>,
    lobby: LobbyState
): void {
    const member = findMember(lobby, ctx.steamId);
    const teamIndex = member === null ? -1 : member.team === TEAM_GOOD ? 0 : member.team === TEAM_BAD ? 1 : -1;
    if (teamIndex < 0) {
        return;
    }

    const requestedTeamId = ctx.request.teamId ?? 0;
    const selectedTeam =
        requestedTeamId === 0 ? firstAccountTeam(ctx, ctx.accountId) : ctx.services.teams.get(requestedTeamId);
    if (selectedTeam === null) {
        return;
    }

    lobby.teamDetails = normalizeLobbyTeamDetails(lobby.teamDetails);
    const oppositeIndex = teamIndex === 0 ? 1 : 0;
    if (lobby.teamDetails[oppositeIndex].teamId === selectedTeam.teamId) {
        lobby.teamDetails[oppositeIndex] = emptyLobbyTeamDetail();
    }

    lobby.teamDetails[teamIndex] = lobbyTeamDetail(selectedTeam);
}

function firstAccountTeam(
    ctx: HandlerContext<CMsgApplyTeamToPracticeLobby, CMsgGenericResult>,
    accountId: number
): DotaTeam | null {
    const teams = ctx.services.teams.getForAccount(accountId);
    return teams.length === 0 ? null : teams[0];
}

function lobbyTeamDetail(team: DotaTeam): CLobbyTeamDetails {
    return {
        teamName: team.name,
        teamTag: team.tag,
        teamId: team.teamId,
        teamLogo: team.logo,
        teamBaseLogo: team.baseLogo,
        teamBannerLogo: team.bannerLogo,
        teamComplete: true,
        teamLogoUrl: team.logoUrl,
        teamAbbreviation: team.abbreviation
    };
}

function markTeamsIncomplete(lobby: LobbyState): void {
    lobby.teamDetails = normalizeLobbyTeamDetails(lobby.teamDetails);
    for (let i = 0; i < lobby.teamDetails.length; i++) {
        lobby.teamDetails[i] = {
            ...lobby.teamDetails[i],
            teamComplete: false
        };
    }
}

function normalizeLobbyTeamDetails(details: CLobbyTeamDetails[]): CLobbyTeamDetails[] {
    const normalized = [emptyLobbyTeamDetail(), emptyLobbyTeamDetail()];
    for (let i = 0; i < details.length && i < normalized.length; i++) {
        normalized[i] = details[i];
    }

    return normalized;
}

function emptyLobbyTeamDetail(): CLobbyTeamDetails {
    return {
        teamName: "",
        teamTag: "",
        teamId: 0,
        teamLogo: 0n,
        teamBaseLogo: 0n,
        teamBannerLogo: 0n,
        teamComplete: false,
        teamLogoUrl: "",
        teamAbbreviation: ""
    };
}

function joinLobbyState(ctx: RawMessageContext, lobby: LobbyState, passKey: string, bypassPassword: boolean): boolean {
    if (lobby.state !== LOBBY_UI || (!bypassPassword && lobby.passKey !== "" && lobby.passKey !== passKey)) {
        return false;
    }

    leaveCurrent(ctx, lobby.lobbyId);
    const member = ensureMember(lobby, ctx.steamId, ctx.accountId, ctx.personaName, ctx.clock.now(), ctx);
    if (member.steamId !== lobby.leaderSteamId) {
        member.team = TEAM_POOL;
        member.slot = 0;
        member.coachTeam = TEAM_NONE;
    }

    refreshLobby(lobby, ctx.clock.now());
    subscribeToLobbyObjectOnly(ctx, ctx.steamId, lobby);
    broadcastLobby(ctx, lobby, ctx.steamId, false);
    publishLobby(ctx.services.lobby, lobby);
    return true;
}

function currentLobby(steamId: bigint): LobbyState | null {
    const lobbyId = store.bySteam.get(steamId);
    return lobbyId === undefined ? null : (store.lobbies.get(lobbyId) ?? null);
}

export function isSteamIdInLobby(steamId: bigint): boolean {
    return currentLobby(steamId) !== null;
}

function serverLobby(steamId: bigint): LobbyState | null {
    const lobbyId = store.byServer.get(steamId);
    return lobbyId === undefined ? null : (store.lobbies.get(lobbyId) ?? null);
}

function waitingLobby(): LobbyState | null {
    let selected: LobbyState | null = null;
    store.lobbies.forEach((lobby) => {
        if (lobby.state === LOBBY_SERVER_SETUP && (selected === null || lobby.version > selected.version)) {
            selected = lobby;
        }
    });

    return selected;
}

function ensureMember(
    lobby: LobbyState,
    steamId: bigint,
    accountId: number,
    personaName: string,
    now: number,
    _ctx: GcContextBase
): LobbyMemberState {
    const existing = findMember(lobby, steamId);
    if (existing !== null) {
        existing.personaName = personaName === "" ? existing.personaName : personaName;
        existing.lastSeen = now;
        store.bySteam.set(steamId, lobby.lobbyId);
        return existing;
    }

    const member: LobbyMemberState = {
        steamId,
        accountId,
        personaName: personaName === "" ? "User" + accountId : personaName,
        team: steamId === lobby.leaderSteamId ? TEAM_GOOD : TEAM_POOL,
        slot: steamId === lobby.leaderSteamId ? 1 : 0,
        coachTeam: TEAM_NONE,
        heroId: 0,
        leaverStatus: LEAVER_DISCONNECTED,
        connectedOnce: false,
        lastSeen: now
    };
    lobby.members.push(member);
    store.bySteam.set(steamId, lobby.lobbyId);
    return member;
}

function findMember(lobby: LobbyState, steamId: bigint): LobbyMemberState | null {
    for (let i = 0; i < lobby.members.length; i++) {
        if (lobby.members[i].steamId === steamId) {
            return lobby.members[i];
        }
    }

    return null;
}

function slotOccupied(lobby: LobbyState, steamId: bigint, team: number, slot: number): boolean {
    for (let i = 0; i < lobby.members.length; i++) {
        const member = lobby.members[i];
        if (member.steamId !== steamId && member.team === team && member.slot === slot) {
            return true;
        }
    }

    return false;
}

function kickMemberByAccountId(ctx: RawMessageContext, accountId: number): void {
    const lobby = currentLobby(ctx.steamId);
    if (lobby === null || accountId === 0) {
        return;
    }

    const member = findMemberByAccountId(lobby, accountId);
    if (member === null) {
        return;
    }

    removeLobbyMember(ctx, lobby, member, true);
    refreshLobby(lobby, ctx.clock.now());
    broadcastLobby(ctx, lobby, member.steamId, false);
    publishLobby(ctx.services.lobby, lobby);
}

function findMemberByAccountId(lobby: LobbyState, accountId: number): LobbyMemberState | null {
    for (let i = 0; i < lobby.members.length; i++) {
        if (lobby.members[i].accountId === accountId) {
            return lobby.members[i];
        }
    }

    return null;
}

function removeLobbyMember(ctx: GcContextBase, lobby: LobbyState, member: LobbyMemberState, kicked: boolean): void {
    lobby.members = lobby.members.filter((candidate) => candidate.steamId !== member.steamId);
    store.bySteam.delete(member.steamId);

    if (kicked) {
        sendTo(ctx, member.steamId, DOTA_POPUP_MESSAGE_ID, {
            id: CMsgDOTAPopup_PopupID.KickedFromLobby,
            customText: lobby.gameName
        } as CMsgDOTAPopup);
    }

    sendTo(ctx, member.steamId, Msg.SOCacheUnsubscribed, buildLobbySoCacheUnsubscribed(lobby));
}

function shuffleLobbyTeams(lobby: LobbyState): void {
    const candidates = lobby.members.filter(
        (member) => member.coachTeam === TEAM_NONE && (member.team === TEAM_GOOD || member.team === TEAM_BAD)
    );
    const ordered = candidates.slice().sort((left, right) => {
        if (left.slot !== right.slot) {
            return left.slot - right.slot;
        }

        return left.accountId - right.accountId;
    });

    for (let i = 0; i < ordered.length; i++) {
        ordered[i].team = i % 2 === 0 ? TEAM_GOOD : TEAM_BAD;
        ordered[i].slot = Math.floor(i / 2) + 1;
    }

    clearTeamDetail(lobby, TEAM_GOOD);
    clearTeamDetail(lobby, TEAM_BAD);
}

function flipLobbyTeams(lobby: LobbyState): void {
    for (let i = 0; i < lobby.members.length; i++) {
        const member = lobby.members[i];
        if (member.team === TEAM_GOOD) {
            member.team = TEAM_BAD;
        } else if (member.team === TEAM_BAD) {
            member.team = TEAM_GOOD;
        }
    }

    lobby.teamDetails = normalizeLobbyTeamDetails(lobby.teamDetails);
    const radiant = { ...lobby.teamDetails[0] };
    lobby.teamDetails[0] = { ...lobby.teamDetails[1] };
    lobby.teamDetails[1] = radiant;
}

function clearTeamDetailForMember(lobby: LobbyState, member: LobbyMemberState): void {
    clearTeamDetail(lobby, member.team);
}

function clearTeamDetail(lobby: LobbyState, team: number): void {
    lobby.teamDetails = normalizeLobbyTeamDetails(lobby.teamDetails);
    if (team === TEAM_GOOD) {
        lobby.teamDetails[0] = emptyLobbyTeamDetail();
    } else if (team === TEAM_BAD) {
        lobby.teamDetails[1] = emptyLobbyTeamDetail();
    }
}

function leaveCurrent(ctx: GcContextBase, keepLobbyId: bigint): void {
    const lobby = currentLobby(ctx.steamId);
    if (lobby === null || lobby.lobbyId === keepLobbyId) {
        return;
    }

    const destroy = lobby.leaderSteamId === ctx.steamId || lobby.members.length <= 1;
    if (destroy) {
        destroyLobby(lobby, ctx, "leave");
        return;
    }

    const member = findMember(lobby, ctx.steamId);
    if (member !== null) {
        removeLobbyMember(ctx, lobby, member, false);
    }
    refreshLobby(lobby, ctx.clock.now());
    broadcastLobby(ctx, lobby, ctx.steamId, false);
    publishLobby(ctx.services.lobby, lobby);
}

function leavePublishedLobbyState(ctx: GcContextBase): void {
    const snapshots = ctx.services.lobby.listSnapshots();
    for (let i = 0; i < snapshots.length; i++) {
        const snapshot = snapshots[i];
        if (snapshot.state !== LOBBY_UI || !snapshot.players.some((player) => player.steamId === ctx.steamId)) {
            continue;
        }

        ctx.send(Msg.SOCacheUnsubscribed, Proto.CMsgSOCacheUnsubscribed, buildLobbySoCacheUnsubscribedForId(snapshot.lobbyId));
        ctx.services.lobby.removeSnapshot(snapshot.lobbyId);
    }
}

function destroyLobby(lobby: LobbyState, ctx: GcContextBase | undefined, reason: string): void {
    const unsubscribe = ctx === undefined ? null : buildLobbySoCacheUnsubscribed(lobby);
    for (let i = 0; i < lobby.members.length; i++) {
        store.bySteam.delete(lobby.members[i].steamId);
        if (ctx !== undefined && unsubscribe !== null) {
            sendTo(ctx, lobby.members[i].steamId, Msg.SOCacheUnsubscribed, unsubscribe);
        }
    }

    if (lobby.serverSteamId !== 0n) {
        store.byServer.delete(lobby.serverSteamId);
        if (ctx !== undefined && unsubscribe !== null) {
            sendTo(ctx, lobby.serverSteamId, Msg.SOCacheUnsubscribed, unsubscribe);
            ctx.services.lobby.releaseDedicatedServer(lobby.lobbyId, reason);
        }
    }

    store.lobbies.delete(lobby.lobbyId);
    if (ctx !== undefined) {
        ctx.services.lobby.removeSnapshot(lobby.lobbyId);
    }
}

function refreshLobby(lobby: LobbyState, now: number): void {
    lobby.version = nextId(now);
    lobby.updatedAt = now;
}

function attachServer(ctx: RawMessageContext, lobby: LobbyState, markRun: boolean): void {
    const previousServerSteamId = lobby.serverSteamId;
    lobby.serverSteamId = ctx.steamId;
    if (previousServerSteamId !== 0n && previousServerSteamId !== ctx.steamId) {
        lobby.playerItemsServerSteamId = 0n;
    }

    store.byServer.set(ctx.steamId, lobby.lobbyId);
    if (lobby.serverPort === 0) {
        lobby.serverPort = DEFAULT_PORT;
    }

    if (markRun) {
        ensureMatchId(lobby);
        lobby.state = LOBBY_RUN;
        lobby.lan = lobby.serverRegion === 0;
        lobby.connect = buildConnectString(ctx.services.lobby, lobby);
        lobby.gameStartTime = lobby.gameStartTime === 0 ? ctx.clock.now() : lobby.gameStartTime;
        for (let i = 0; i < lobby.members.length; i++) {
            lobby.members[i].leaverStatus = LEAVER_NONE;
        }
        startRealtimeStats(ctx, lobby);
    } else if (lobby.state !== LOBBY_RUN) {
        // Dedicated flow is two-stage: 4508/GameServerInfo installs the
        // SERVER_SETUP cache, then 4506/7034 promotes the same server to RUN.
        lobby.state = LOBBY_SERVER_SETUP;
    }

    refreshLobby(lobby, ctx.clock.now());
    if (lobby.playerItemsServerSteamId !== lobby.serverSteamId) {
        // Player cosmetics reach the game server as two owner-scoped SO cache
        // subscriptions: service 0 announces the player owner and service 1
        // carries the equipped CSOEconItem objects. Hero items and global
        // loadout effects are both normal econ items; their equippedState
        // (class/slot) tells Dota where to render them. Dedicated servers get
        // this during GameServerInfo before RUN; listen/local servers may first
        // become addressable on LANServerAvailable/ServerAvailable, so send it
        // once to the first SteamID that actually owns the match.
        sendLobbyPlayerItemsToServer(ctx, lobby);
        lobby.playerItemsServerSteamId = lobby.serverSteamId;
    }
    broadcastLobby(ctx, lobby, 0n, true);
    sendTo(ctx, lobby.serverSteamId, Msg.SOCacheSubscribed, buildLobbySoCacheSubscribed(ctx, lobby));
    publishLobby(ctx.services.lobby, lobby);
}

function updateServerInfo(lobby: LobbyState, ctx: RawMessageContext, request: CMsgGameServerInfo): void {
    lobby.serverPort = request.serverPort ?? lobby.serverPort;
    lobby.serverPublicIp = ipFromUint32(request.serverPublicIpAddr ?? 0);
    lobby.serverPrivateIp = ipFromUint32(request.serverPrivateIpAddr ?? 0);
    ctx.logger.info(
        "server info steam=" +
            ctx.steamId +
            " lobby=" +
            lobby.lobbyId +
            " port=" +
            lobby.serverPort +
            " public=" +
            lobby.serverPublicIp +
            " private=" +
            lobby.serverPrivateIp
    );
}

function buildConnectString(services: DotaLobbyService, lobby: LobbyState): string {
    const fallback = lobby.serverPrivateIp === "" ? "127.0.0.1" : lobby.serverPrivateIp;
    const ips = services.resolveGameServerConnectIps(lobby.serverPublicIp, lobby.serverPrivateIp, fallback).split(" ");
    const endpoints: string[] = [];
    for (let i = 0; i < ips.length; i++) {
        const ip = ips[i].trim();
        if (ip !== "" && endpoints.indexOf(ip + ":" + lobby.serverPort) < 0) {
            endpoints.push(ip + ":" + lobby.serverPort);
        }
    }

    if (endpoints.length === 0) {
        endpoints.push(fallback + ":" + lobby.serverPort);
    }

    return endpoints.join(" ");
}

function startRealtimeStats(ctx: RawMessageContext, lobby: LobbyState): void {
    if (lobby.serverSteamId === 0n || lobby.realtimeStatsStartStopSent) {
        return;
    }

    const payload: CMsgGCToServerRealtimeStatsStartStop = { delayed: true };
    ctx.services.lobby.queueMessage(
        lobby.serverSteamId,
        Msg.GCToServerRealtimeStatsStartStop,
        ctx.encode(Proto.CMsgGCToServerRealtimeStatsStartStop, payload),
        true
    );
    lobby.realtimeStatsStartStopSent = true;
}

function sendLobbyPlayerItemsToServer(ctx: RawMessageContext, lobby: LobbyState): void {
    if (lobby.serverSteamId === 0n) {
        return;
    }

    // Dedicated/listen servers need every player's econ owner cache, not just
    // the lobby cache. Target this SOCacheSubscribed at the server SteamID so
    // the server owns each member's CSOEconItem list before RUN/hero spawn.
    // Without this step items can appear equipped in the client UI while the
    // in-game hero uses defaults because the server never saw equippedState.
    for (let i = 0; i < lobby.members.length; i++) {
        const member = lobby.members[i];
        const inventory = ctx.services.items.getInventory(member.steamId);
        const ownerSync = inventory.version === 0n ? 1n : inventory.version;
        sendTo(
            ctx,
            lobby.serverSteamId,
            Msg.SOCacheSubscribed,
            buildGameOwnerSoCacheSubscribedForInventory(inventory, ownerSync)
        );
        sendTo(
            ctx,
            lobby.serverSteamId,
            Msg.SOCacheSubscribed,
            buildEconSoCacheSubscribedForInventory(ctx, inventory, {
                onlyEquipped: true,
                econObjectsOnly: true,
                syncVersion: ownerSync
            })
        );
    }
}

function applyConnectedPlayers(lobby: LobbyState, request: CMsgConnectedPlayers, now: number): void {
    const connected = request.connectedPlayers ?? [];
    for (let i = 0; i < connected.length; i++) {
        const player = connected[i];
        const steamId = player.steamId ?? 0n;
        const member = findMember(lobby, steamId);
        if (member !== null) {
            member.heroId = player.heroId ?? member.heroId;
            member.leaverStatus = LEAVER_NONE;
            member.connectedOnce = true;
            member.lastSeen = now;
        }
    }

    const disconnected = request.disconnectedPlayers ?? [];
    for (let i = 0; i < disconnected.length; i++) {
        const player = disconnected[i];
        const member = findMember(lobby, player.steamId ?? 0n);
        if (member !== null) {
            member.leaverStatus = LEAVER_DISCONNECTED;
            member.lastSeen = now;
        }
    }
}

function prepareLobbyForLaunch(ctx: GcContextBase, lobby: LobbyState): void {
    if (lobby.allowSpectating) {
        let spectators = 0;
        for (let i = 0; i < lobby.members.length; i++) {
            const member = lobby.members[i];
            if (member.team === TEAM_POOL || member.team === TEAM_SPECTATOR) {
                member.team = TEAM_SPECTATOR;
                member.slot = 0;
                spectators += 1;
            }
        }

        lobby.numSpectators = spectators;
        return;
    }

    const removed: bigint[] = [];
    lobby.members = lobby.members.filter((member) => {
        const keep = member.team !== TEAM_POOL && member.team !== TEAM_SPECTATOR;
        if (!keep) {
            removed.push(member.steamId);
        }

        return keep;
    });

    for (let i = 0; i < removed.length; i++) {
        store.bySteam.delete(removed[i]);
        sendTo(ctx, removed[i], Msg.SOCacheUnsubscribed, buildLobbySoCacheUnsubscribed(lobby));
    }

    lobby.numSpectators = 0;
}

function ensureMatchId(lobby: LobbyState): bigint {
    if (lobby.matchId === 0n) {
        if (store.nextMatchId === 0n) {
            store.nextMatchId = BigInt(lobby.updatedAt) * 5n;
        }

        store.nextMatchId += 1n;
        lobby.matchId = store.nextMatchId;
    }

    return lobby.matchId;
}

function buildLobbySoCacheSubscribed(ctx: GcContextBase, lobby: LobbyState): CMsgSOCacheSubscribed {
    return {
        objects: [
            subscribedType(LOBBY_OBJECT_TYPE_ID, [ctx.encode(Proto.CSODOTALobby, buildLobbyObject(lobby))]),
            // Practice lobby caches include an empty lobby-invite SO bucket.
            // Dota uses the same type id for pending lobby invites targeted
            // at owner type 4, so both flows must stay on 2013.
            subscribedType(LOBBY_INVITE_OBJECT_TYPE_ID, [
                ctx.encode(Proto.CSODOTALobbyInvite, buildLobbyInviteObject())
            ]),
            subscribedType(LOBBY_STATIC_OBJECT_TYPE_ID, [
                ctx.encode(Proto.CSODOTAStaticLobby, buildStaticLobbyObject(lobby))
            ]),
            subscribedType(LOBBY_SERVER_OBJECT_TYPE_ID, [
                ctx.encode(Proto.CSODOTAServerLobby, buildServerLobbyObject(lobby))
            ]),
            subscribedType(LOBBY_SERVER_STATIC_OBJECT_TYPE_ID, [
                ctx.encode(Proto.CSODOTAServerStaticLobby, buildServerStaticLobbyObject(lobby))
            ])
        ],
        version: lobby.version,
        ownerSoid: { type: LOBBY_OWNER_TYPE, id: lobby.lobbyId }
    };
}

function buildLobbyObjectOnlySubscribed(ctx: GcContextBase, lobby: LobbyState): CMsgSOCacheSubscribed {
    // Joining an existing practice lobby follows the legacy GC wire flow:
    // the joining client receives a cache subscription containing only the
    // mutable lobby object (2004), then a single-object create for the same
    // object. Static lobby/server buckets are sent when the lobby is created,
    // but not during join; sending them here makes this client build unstable.
    return {
        objects: [subscribedType(LOBBY_OBJECT_TYPE_ID, [ctx.encode(Proto.CSODOTALobby, buildLobbyObject(lobby))])],
        version: lobby.version,
        ownerSoid: { type: LOBBY_OWNER_TYPE, id: lobby.lobbyId }
    };
}

function buildLobbySoCacheUnsubscribed(lobby: LobbyState): CMsgSOCacheUnsubscribed {
    return buildLobbySoCacheUnsubscribedForId(lobby.lobbyId);
}

function buildLobbySoCacheUnsubscribedForId(lobbyId: bigint): CMsgSOCacheUnsubscribed {
    return {
        ownerSoid: { type: LOBBY_OWNER_TYPE, id: lobbyId }
    };
}

function buildLobbySingleObject(ctx: GcContextBase, lobby: LobbyState): CMsgSOSingleObject {
    return {
        typeId: LOBBY_OBJECT_TYPE_ID,
        objectData: ctx.encode(Proto.CSODOTALobby, buildLobbyObject(lobby)),
        version: lobby.version + 1n,
        ownerSoid: { type: LOBBY_OWNER_TYPE, id: lobby.lobbyId }
    };
}

function buildLobbyMultipleObjects(ctx: GcContextBase, lobby: LobbyState): CMsgSOMultipleObjects {
    // Existing lobby members are notified like the legacy GC: UpdateMultiple
    // carries only CSODOTALobby (2004). Member names and server-static data are
    // part of the create-time SO cache, not the join-time update.
    const objectsModified: CMsgSOMultipleObjects_SingleObject[] = [
        {
            typeId: LOBBY_OBJECT_TYPE_ID,
            objectData: ctx.encode(Proto.CSODOTALobby, buildLobbyObject(lobby))
        }
    ];

    return {
        objectsModified,
        version: lobby.version,
        ownerSoid: { type: LOBBY_OWNER_TYPE, id: lobby.lobbyId },
        serviceId: LOBBY_SERVICE_ID
    };
}

function buildLobbyObject(lobby: LobbyState): CSODOTALobby {
    return {
        lobbyId: lobby.lobbyId,
        gameMode: lobby.gameMode,
        state: lobby.state,
        connect: lobby.connect,
        serverId: lobby.serverSteamId,
        leaderId: lobby.leaderSteamId,
        lobbyType: 1,
        allowCheats: lobby.allowCheats,
        fillWithBots: lobby.fillWithBots,
        gameName: lobby.gameName,
        teamDetails: lobby.teamDetails,
        serverRegion: lobby.serverRegion,
        gameState: lobby.gameState,
        numSpectators: lobby.numSpectators,
        cmPick: lobby.cmPick,
        matchId: lobby.matchId,
        allowSpectating: lobby.allowSpectating,
        passKey: lobby.passKey,
        leagueid: lobby.leagueId,
        penaltyLevelRadiant: 0,
        penaltyLevelDire: 0,
        seriesType: lobby.seriesType,
        radiantSeriesWins: lobby.radiantSeriesWins,
        direSeriesWins: lobby.direSeriesWins,
        allchat: lobby.allchat,
        dotaTvDelay: lobby.dotaTvDelay,
        customGameMode: lobby.customGameMode,
        customMapName: lobby.customMapName,
        customDifficulty: lobby.customDifficulty,
        lan: lobby.lan,
        extraMessages: [{ id: LOBBY_EXTRA_MESSAGE_ID, contents: LOBBY_EXTRA_MESSAGE_CONTENTS }],
        customGameId: lobby.customGameId,
        customMinPlayers: lobby.customMinPlayers,
        customMaxPlayers: lobby.customMaxPlayers,
        visibility: lobby.visibility,
        customGameCrc: lobby.customGameCrc,
        customGameTimestamp: lobby.customGameTimestamp,
        previousMatchOverride: 0n,
        customGamePenalties: lobby.customGamePenalties,
        gameStartTime: lobby.gameStartTime,
        pauseSetting: lobby.pauseSetting,
        botDifficultyRadiant: lobby.botDifficultyRadiant,
        botDifficultyDire: lobby.botDifficultyDire,
        botRadiant: 0n,
        botDire: 0n,
        selectionPriorityRules: lobby.selectionPriorityRules,
        leagueNodeId: 0,
        leaguePhase: 0,
        withScenarioSave: false,
        allMembers: buildLobbyMembers(lobby),
        memberIndices: memberIndices(lobby.members.length),
        ...(lobby.requestedHeroIds.length > 0 ? { requestedHeroIds: lobby.requestedHeroIds } : {}),
        ...(lobby.requestedHeroTeams.length > 0 ? { requestedHeroTeams: lobby.requestedHeroTeams } : {}),
        lobbyCreationTime: lobby.createdAt
    };
}

function buildLobbyMembers(lobby: LobbyState): CSODOTALobbyMember[] {
    const members: CSODOTALobbyMember[] = [];
    for (let i = 0; i < lobby.members.length; i++) {
        const member = lobby.members[i];
        members.push({
            id: member.steamId,
            heroId: member.heroId,
            team: member.team,
            slot: member.slot,
            leaverStatus: member.leaverStatus,
            coachTeam: member.coachTeam === TEAM_NONE ? undefined : member.coachTeam
        });
    }

    return members;
}

function buildStaticLobbyObject(lobby: LobbyState): CSODOTAStaticLobby {
    return {
        allMembers: lobby.members.map((member) => ({
            name: member.personaName
        })),
        isPlayerDraft: false
    };
}

function buildLobbyInviteObject(): CSODOTALobbyInvite {
    return {};
}

function buildServerLobbyObject(lobby: LobbyState): CSODOTAServerLobby {
    return {
        allMembers: lobby.members.map(() => ({}))
    };
}

function buildServerStaticLobbyObject(lobby: LobbyState): CSODOTAServerStaticLobby {
    return {
        allMembers: lobby.members.map((member) => ({
            steamId: member.steamId,
            wasMvpLastGame: false,
            isPlusSubscriber: true,
            favoriteTeamPacked: 0n,
            isSteamChina: false,
            bannedHeroIds: DEFAULT_SERVER_STATIC_BANNED_HERO_IDS
        })),
        postPatchStrategyTimeBuffer: 0
    };
}

function subscribedType(typeId: number, objectData: Uint8Array[]): CMsgSOCacheSubscribed_SubscribedType {
    return { typeId, objectData };
}

function memberIndices(count: number): number[] {
    const indices: number[] = [];
    for (let i = 0; i < count; i++) {
        indices.push(i);
    }

    return indices;
}

function broadcastLobby(ctx: GcContextBase, lobby: LobbyState, exceptSteamId: bigint, includeServer: boolean): void {
    const payload = buildLobbyMultipleObjects(ctx, lobby);
    if (includeServer && lobby.serverSteamId !== 0n && lobby.serverSteamId !== exceptSteamId) {
        sendTo(ctx, lobby.serverSteamId, Msg.SOCacheUpdated, payload);
    }

    for (let i = 0; i < lobby.members.length; i++) {
        const steamId = lobby.members[i].steamId;
        // Notify every subscribed member except the caller-designated one.
        // The leader is a lobby SO-cache subscriber too, so mutations the
        // leader triggers (rename, slot, apply-team) MUST reach the leader;
        // callers exclude only the acting client via exceptSteamId when the
        // acting client already received the state through subscribe/reply.
        if (steamId !== exceptSteamId) {
            sendTo(ctx, steamId, Msg.SOCacheUpdated, payload);
        }
    }
}

function subscribeToLobby(ctx: GcContextBase, steamId: bigint, lobby: LobbyState): void {
    sendTo(ctx, steamId, Msg.SOCacheSubscribed, buildLobbySoCacheSubscribed(ctx, lobby));
    sendTo(ctx, steamId, Msg.SOSingleObject, buildLobbySingleObject(ctx, lobby));
}

function subscribeToLobbyObjectOnly(ctx: GcContextBase, steamId: bigint, lobby: LobbyState): void {
    sendTo(ctx, steamId, Msg.SOCacheSubscribed, buildLobbyObjectOnlySubscribed(ctx, lobby));
    sendTo(ctx, steamId, Msg.SOSingleObject, buildLobbySingleObject(ctx, lobby));
}

type LobbyOutboundMessage =
    | CMsgSOCacheSubscribed
    | CMsgSOCacheUnsubscribed
    | CMsgSOSingleObject
    | CMsgSOMultipleObjects
    | CMsgGCToClientMatchSignedOut
    | CMsgInvitationCreated
    | CMsgDOTAPopup
    | CMsgGenericResult;

function sendTo(ctx: GcContextBase, steamId: bigint, messageType: number, message: LobbyOutboundMessage): boolean {
    if (messageType === Msg.SOCacheSubscribed) {
        return sendTyped(ctx, steamId, messageType, Proto.CMsgSOCacheSubscribed, message as CMsgSOCacheSubscribed);
    }

    if (messageType === Msg.SOCacheUnsubscribed) {
        return sendTyped(ctx, steamId, messageType, Proto.CMsgSOCacheUnsubscribed, message as CMsgSOCacheUnsubscribed);
    }

    if (messageType === Msg.SOSingleObject) {
        return sendTyped(ctx, steamId, messageType, Proto.CMsgSOSingleObject, message as CMsgSOSingleObject);
    }

    if (messageType === Msg.SOCacheUpdated) {
        return sendTyped(ctx, steamId, messageType, Proto.CMsgSOMultipleObjects, message as CMsgSOMultipleObjects);
    }

    if (messageType === DOTA_POPUP_MESSAGE_ID) {
        return sendTyped(ctx, steamId, messageType, Proto.CMsgDOTAPopup, message as CMsgDOTAPopup);
    }

    if (messageType === Msg.GCToClientMatchSignedOut) {
        return sendTyped(
            ctx,
            steamId,
            messageType,
            Proto.CMsgGCToClientMatchSignedOut,
            message as CMsgGCToClientMatchSignedOut
        );
    }

    if (messageType === Msg.GCInvitationCreated) {
        return sendTyped(ctx, steamId, messageType, Proto.CMsgInvitationCreated, message as CMsgInvitationCreated);
    }

    return sendTyped(ctx, steamId, messageType, Proto.CMsgGenericResult, message as CMsgGenericResult);
}

function sendTyped<TMessage>(
    ctx: GcContextBase,
    steamId: bigint,
    messageType: number,
    proto: ProtoDescriptor<TMessage>,
    message: TMessage
): boolean {
    const payload = ctx.encode(proto, message);
    if (steamId === ctx.steamId || steamId === 0n) {
        ctx.send(messageType, proto, message);
        return true;
    }

    return ctx.services.lobby.queueMessage(steamId, messageType, payload, true);
}

function publishLobby(services: DotaLobbyService, lobby: LobbyState): void {
    services.publishSnapshot(buildMatchSnapshot(lobby));
}

function buildMatchSnapshot(lobby: LobbyState): DotaLobbyMatchSnapshot {
    const players: DotaLobbyMatchPlayer[] = [];
    for (let i = 0; i < lobby.members.length; i++) {
        const member = lobby.members[i];
        players.push({
            steamId: member.steamId,
            accountId: member.accountId,
            personaName: member.personaName,
            team: member.team,
            slot: member.slot,
            coachTeam: member.coachTeam,
            heroId: member.heroId
        });
    }

    return {
        lobbyId: lobby.lobbyId,
        matchId: lobby.matchId,
        serverSteamId: lobby.serverSteamId,
        connect: lobby.connect,
        state: lobby.state,
        gameState: lobby.gameState,
        gameStartTime: lobby.gameStartTime,
        dedicated: !lobby.lan,
        players
    };
}

function listLobbies(
    _service: DotaLobbyService,
    region: number,
    gameMode: number,
    _requesterSteamId: bigint
): CMsgPracticeLobbyListResponseEntry[] {
    return collectLobbies(region, gameMode, true);
}

function collectLobbies(
    region: number,
    gameMode: number,
    strictRegion: boolean
): CMsgPracticeLobbyListResponseEntry[] {
    const result: CMsgPracticeLobbyListResponseEntry[] = [];
    const seen = new Set<bigint>();
    store.lobbies.forEach((lobby) => {
        if (lobby.state !== LOBBY_UI) {
            return;
        }

        if (strictRegion && region !== 0 && lobby.serverRegion !== region) {
            return;
        }

        if (gameMode !== 0 && lobby.gameMode !== gameMode) {
            return;
        }

        seen.add(lobby.lobbyId);
        // Lobby browse responses intentionally keep the same compact shape as
        // the legacy GC. Dota only needs these fields to render/join a public
        // practice lobby; extra entry metadata has caused inconsistent browse
        // behavior between clients.
        result.push({
            id: lobby.lobbyId,
            leaderAccountId: lobby.leaderAccountId,
            name: lobby.gameName,
            gameMode: lobby.gameMode,
            players: lobby.members.length,
            serverRegion: lobby.serverRegion,
            requiresPassKey: lobby.passKey !== "",
            leagueId: 0
        });
    });

    // Public lobby browse is memory-owned like the legacy GC. Persisted match
    // snapshots are for reconnect/running-game recovery; advertising them as
    // practice lobbies creates ghost entries that cannot be joined because no
    // live lobby object exists in the TS store.
    return result;
}

function createInvite(lobby: LobbyState, ctx: GcContextBase, targetSteamId: bigint): LobbyInviteState {
    const invite: LobbyInviteState = {
        inviteId: nextId(ctx.clock.now()),
        lobbyId: lobby.lobbyId,
        targetSteamId,
        senderSteamId: ctx.steamId,
        senderName: ctx.personaName,
        createdAt: ctx.clock.now()
    };
    store.invites.set(invite.inviteId, invite);
    return invite;
}

function sendInvite(ctx: GcContextBase, invite: LobbyInviteState): void {
    sendTo(ctx, invite.targetSteamId, Msg.SOCacheSubscribed, buildInviteSubscribed(ctx, invite));
}

function buildInviteSubscribed(ctx: GcContextBase, invite: LobbyInviteState): CMsgSOCacheSubscribed {
    return {
        objects: [
            {
                typeId: LOBBY_INVITE_OBJECT_TYPE_ID,
                objectData: [ctx.encode(Proto.CSODOTALobbyInvite, buildInviteObject(invite))]
            }
        ],
        version: invite.inviteId,
        ownerSoid: { type: LOBBY_INVITE_OWNER_TYPE, id: invite.lobbyId },
        serviceId: LOBBY_SERVICE_ID,
        syncVersion: invite.inviteId
    };
}

function buildInviteUnsubscribed(lobbyId: bigint): CMsgSOCacheUnsubscribed {
    return { ownerSoid: { type: LOBBY_INVITE_OWNER_TYPE, id: lobbyId } };
}

function buildInviteObject(invite: LobbyInviteState): CSODOTALobbyInvite {
    const lobby = store.lobbies.get(invite.lobbyId) ?? null;
    return {
        groupId: invite.lobbyId,
        senderId: invite.senderSteamId,
        senderName: invite.senderName,
        inviteGid: invite.inviteId,
        customGameId: lobby?.customGameId ?? 0n,
        customGameCrc: lobby?.customGameCrc ?? 0n,
        customGameTimestamp: lobby?.customGameTimestamp ?? 0,
        members:
            lobby?.members.map((member) => ({
                name: member.personaName,
                steamId: member.steamId
            })) ?? []
    };
}

function emitCurrentLobbyInvites<TRequest, TResponse>(ctx: HandlerContext<TRequest, TResponse>): void {
    store.invites.forEach((invite) => {
        if (invite.targetSteamId === ctx.steamId) {
            sendInvite(ctx, invite);
        }
    });
}

function takeInvite(lobbyId: bigint, targetSteamId: bigint): LobbyInviteState | null {
    let selected: LobbyInviteState | null = null;
    const invites = Array.from(store.invites.values());
    for (let i = 0; i < invites.length; i++) {
        const invite = invites[i];
        if (invite.lobbyId === lobbyId && invite.targetSteamId === targetSteamId) {
            selected = invite;
        }
    }

    if (selected !== null) {
        store.invites.delete(selected.inviteId);
    }

    return selected;
}

function firstSteamId(values: bigint[]): bigint {
    return values.length === 0 ? 0n : values[0];
}

function mapRecentMatch(source: DotaPlayerRecentMatchInfo | null): CMsgPlayerRecentMatchInfo | undefined {
    if (source === null) {
        return undefined;
    }

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

function nextId(nowSeconds: number): bigint {
    store.nextId = (store.nextId % 16777215n) + 1n;
    return (BigInt(nowSeconds) << 24n) + store.nextId;
}

function ipFromUint32(value: number): string {
    if (value === 0) {
        return "";
    }

    const a = value & 255;
    const b = (value >> 8) & 255;
    const c = (value >> 16) & 255;
    const d = (value >> 24) & 255;
    return a + "." + b + "." + c + "." + d;
}
