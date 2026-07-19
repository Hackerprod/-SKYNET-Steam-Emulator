import {
    DotaLiveScoreboardSnapshot,
    DotaMatchStateHistorySnapshot,
    HandlerContext,
    RawMessageContext,
    gc
} from "../framework/gc";
import {
    CMsgDOTALiveScoreboardUpdate,
    CMsgDOTARealtimeGameStatsTerse_TeamDetails,
    CMsgDOTASetMatchHistoryAccess,
    CMsgDOTASetMatchHistoryAccessResponse,
    CMsgDOTASubmitPlayerReport,
    CMsgDOTASubmitPlayerReportResponse,
    CMsgDOTASubmitPlayerReportResponse_EResult,
    CMsgDOTASubmitPlayerReportResponseV2,
    CMsgDOTASubmitPlayerReportResponseV2_EResult,
    CMsgDOTASubmitPlayerReportV2,
    CMsgGameMatchSignOutPermissionRequest,
    CMsgGameMatchSignOutPermissionResponse,
    CMsgLeaverDetected,
    CMsgLeaverDetectedResponse,
    CMsgServerGCUpdateSpectatorCount,
    CMsgServerToGCMatchStateHistory,
    CMsgServerToGCRealtimeStats,
    CMsgServerToGCRequestStatus,
    CMsgServerToGCRequestStatusResponse,
    Msg,
    Proto,
    Routes
} from "../generated/dota";

const GC_SUCCESS = 1;
const ERESULT_FAIL = 2;
const SERVER_STATUS_READY = 0;
const REPORT_RESULT_SUCCESS = CMsgDOTASubmitPlayerReportResponse_EResult.Success;
const REPORT_RESULT_INVALID = CMsgDOTASubmitPlayerReportResponse_EResult.Invalid;
const REPORT_V2_RESULT_SUCCESS = CMsgDOTASubmitPlayerReportResponseV2_EResult.Success;
const REPORT_V2_RESULT_INVALID = CMsgDOTASubmitPlayerReportResponseV2_EResult.Invalid;

export function registerMatch(): void {
    const match = new Match();
    match.register();
}

export class Match {
    register(): void {
        gc.on(Routes.GameMatchSignOutPermission, (ctx) => this.signOutPermission(ctx));
        gc.on(Routes.GameBotMatchSignOutPermission, (ctx) => this.signOutPermission(ctx));
        gc.on(Routes.SetMatchHistoryAccess, (ctx) => this.setMatchHistoryAccess(ctx));
        gc.on(Routes.ServerStatus, (ctx) => this.serverStatus(ctx));
        gc.on(Routes.LeaverDetected, (ctx) => this.leaverDetected(ctx));
        gc.on(Routes.SubmitPlayerReport, (ctx) => this.submitPlayerReport(ctx));
        gc.on(Routes.SubmitPlayerReportV2, (ctx) => this.submitPlayerReportV2(ctx));
        gc.onMessage(Msg.ServerToGCRealtimeStats, (ctx) => this.realtimeStats(ctx));
        gc.onMessage(Msg.ServerToGCMatchStateHistory, (ctx) => this.matchStateHistory(ctx));
        gc.onMessage(Msg.ServerGCUpdateSpectatorCount, (ctx) => this.updateSpectatorCount(ctx));
        gc.onMessage(Msg.GCLiveScoreboardUpdate, (ctx) => this.liveScoreboardUpdate(ctx));
    }

    private signOutPermission(
        ctx: HandlerContext<CMsgGameMatchSignOutPermissionRequest, CMsgGameMatchSignOutPermissionResponse>
    ): boolean {
        const response: CMsgGameMatchSignOutPermissionResponse = {
            permissionGranted: true,
            abandonSignout: false,
            retryDelaySeconds: 0
        };
        ctx.services.match.recordSignOutPermission({
            serverVersion: ctx.request.serverVersion ?? 0,
            localAttempt: ctx.request.localAttempt ?? 0,
            totalAttempt: ctx.request.totalAttempt ?? 0,
            secondsWaited: ctx.request.secondsWaited ?? 0,
            permissionGranted: response.permissionGranted ?? false,
            abandonSignout: response.abandonSignout ?? false,
            retryDelaySeconds: response.retryDelaySeconds ?? 0
        });
        ctx.reply(response);
        return true;
    }

    private setMatchHistoryAccess(
        ctx: HandlerContext<CMsgDOTASetMatchHistoryAccess, CMsgDOTASetMatchHistoryAccessResponse>
    ): boolean {
        ctx.reply({
            eresult: ctx.services.match.setHistoryAccess(ctx.request.allow3rdPartyMatchHistory ?? false)
                ? GC_SUCCESS
                : ERESULT_FAIL
        });
        return true;
    }

    private serverStatus(
        ctx: HandlerContext<CMsgServerToGCRequestStatus, CMsgServerToGCRequestStatusResponse>
    ): boolean {
        ctx.services.match.recordServerStatus(SERVER_STATUS_READY);
        ctx.reply({ response: SERVER_STATUS_READY });
        return true;
    }

    private leaverDetected(ctx: HandlerContext<CMsgLeaverDetected, CMsgLeaverDetectedResponse>): boolean {
        const state = ctx.request.leaverState;
        ctx.services.match.recordLeaver({
            leaverSteamId: ctx.request.steamId ?? 0n,
            leaverStatus: ctx.request.leaverStatus ?? 0,
            lobbyState: state?.lobbyState ?? 0,
            gameState: state?.gameState ?? 0,
            leaverDetected: state?.leaverDetected ?? false,
            firstBloodHappened: state?.firstBloodHappened ?? false,
            discardMatchResults: state?.discardMatchResults ?? false,
            massDisconnect: state?.massDisconnect ?? false,
            serverCluster: ctx.request.serverCluster ?? 0,
            disconnectReason: ctx.request.disconnectReason ?? 0
        });
        ctx.reply({ result: GC_SUCCESS });
        return true;
    }

    private realtimeStats(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgServerToGCRealtimeStats) as CMsgServerToGCRealtimeStats;
        const delayed = request.delayed;
        if (delayed === undefined || delayed.match === undefined) {
            return true;
        }

        ctx.services.match.recordRealtimeStats({
            matchId: delayed.match.matchId ?? 0n,
            serverSteamId: delayed.match.serverSteamId ?? ctx.steamId,
            timestamp: delayed.match.timestamp ?? ctx.clock.now(),
            gameTime: delayed.match.gameTime ?? 0,
            gameState: delayed.match.gameState ?? 0,
            gameMode: delayed.match.gameMode ?? 0,
            lobbyType: delayed.match.lobbyType ?? 0,
            leagueId: delayed.match.leagueId ?? 0,
            radiantScore: scoreForTeam(delayed.teams, 2, 0),
            direScore: scoreForTeam(delayed.teams, 3, 1),
            playerCount: countRealtimePlayers(delayed.teams),
            buildingCount: delayed.buildings?.length ?? 0,
            deltaFrame: delayed.deltaFrame ?? false,
            payloadSize: ctx.payload.length
        });
        return true;
    }

    private matchStateHistory(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgServerToGCMatchStateHistory) as CMsgServerToGCMatchStateHistory;
        const summary = summarizeMatchStateHistory(request, ctx.payload.length);
        if (summary.matchId !== 0n) {
            ctx.services.match.recordMatchStateHistory(summary);
        }

        return true;
    }

    private updateSpectatorCount(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgServerGCUpdateSpectatorCount) as CMsgServerGCUpdateSpectatorCount;
        ctx.services.match.recordSpectatorCount(request.spectatorCount ?? 0);
        return true;
    }

    private liveScoreboardUpdate(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgDOTALiveScoreboardUpdate) as CMsgDOTALiveScoreboardUpdate;
        const snapshot: DotaLiveScoreboardSnapshot = {
            matchId: request.matchId ?? 0n,
            tournamentId: request.tournamentId ?? 0,
            tournamentGameId: request.tournamentGameId ?? 0,
            duration: request.duration ?? 0,
            hltvDelay: request.hltvDelay ?? 0,
            leagueId: request.leagueId ?? 0,
            radiantScore: request.teamGood?.score ?? 0,
            direScore: request.teamBad?.score ?? 0,
            playerCount: (request.teamGood?.players?.length ?? 0) + (request.teamBad?.players?.length ?? 0),
            roshanRespawnTimer: request.roshanRespawnTimer ?? 0,
            payloadSize: ctx.payload.length
        };
        ctx.services.match.recordLiveScoreboard(snapshot);
        return true;
    }

    private submitPlayerReport(
        ctx: HandlerContext<CMsgDOTASubmitPlayerReport, CMsgDOTASubmitPlayerReportResponse>
    ): boolean {
        const report = {
            targetAccountId: ctx.request.targetAccountId ?? 0,
            lobbyId: ctx.request.lobbyId ?? 0n,
            reportFlags: ctx.request.reportFlags ?? 0,
            reportReasons: [],
            comment: ctx.request.comment ?? "",
            gameTime: 0,
            debugSlot: 0,
            debugMatchId: 0n
        };
        const saved = report.targetAccountId !== 0 && ctx.services.match.savePlayerReport(report);
        ctx.reply({
            targetAccountId: report.targetAccountId,
            reportFlags: report.reportFlags,
            debugMessage: saved ? "" : "invalid report",
            enumResult: saved ? REPORT_RESULT_SUCCESS : REPORT_RESULT_INVALID
        });
        return true;
    }

    private submitPlayerReportV2(
        ctx: HandlerContext<CMsgDOTASubmitPlayerReportV2, CMsgDOTASubmitPlayerReportResponseV2>
    ): boolean {
        const report = {
            targetAccountId: ctx.request.targetAccountId ?? 0,
            lobbyId: ctx.request.lobbyId ?? 0n,
            reportFlags: 0,
            reportReasons: ctx.request.reportReason ?? [],
            comment: "",
            gameTime: ctx.request.gameTime ?? 0,
            debugSlot: ctx.request.debugSlot ?? 0,
            debugMatchId: ctx.request.debugMatchId ?? 0n
        };
        const saved = report.targetAccountId !== 0 && ctx.services.match.savePlayerReport(report);
        ctx.reply({
            targetAccountId: report.targetAccountId,
            reportReason: report.reportReasons,
            debugMessage: saved ? "" : "invalid report",
            enumResult: saved ? REPORT_V2_RESULT_SUCCESS : REPORT_V2_RESULT_INVALID
        });
        return true;
    }
}

function countRealtimePlayers(teams: CMsgDOTARealtimeGameStatsTerse_TeamDetails[] | undefined): number {
    let count = 0;
    const safeTeams = teams ?? [];
    for (let i = 0; i < safeTeams.length; i++) {
        count += safeTeams[i].players?.length ?? 0;
    }

    return count;
}

function scoreForTeam(
    teams: CMsgDOTARealtimeGameStatsTerse_TeamDetails[] | undefined,
    teamNumber: number,
    fallbackIndex: number
): number {
    const safeTeams = teams ?? [];
    for (let i = 0; i < safeTeams.length; i++) {
        const team = safeTeams[i];
        if ((team.teamNumber ?? -1) === teamNumber) {
            return team.score ?? 0;
        }
    }

    if (fallbackIndex < safeTeams.length) {
        return safeTeams[fallbackIndex].score ?? 0;
    }

    return 0;
}

function summarizeMatchStateHistory(
    request: CMsgServerToGCMatchStateHistory,
    payloadSize: number
): DotaMatchStateHistorySnapshot {
    const states = request.matchStates ?? [];
    let lastGameTime = 0;
    let radiantKills = 0;
    let direKills = 0;
    if (states.length > 0) {
        const lastState = states[states.length - 1];
        lastGameTime = lastState.gameTime ?? 0;
        const radiantState = lastState.radiantState;
        if (radiantState !== undefined) {
            radiantKills = radiantState.kills ?? 0;
        }

        const direState = lastState.direState;
        if (direState !== undefined) {
            direKills = direState.kills ?? 0;
        }
    }

    return {
        matchId: request.matchId ?? 0n,
        radiantWon: request.radiantWon ?? false,
        mmr: request.mmr ?? 0,
        stateCount: states.length,
        lastGameTime,
        radiantKills,
        direKills,
        payloadSize
    };
}
