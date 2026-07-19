import { gc } from "../framework/gc";
import { Msg } from "../generated/dota";

export function registerMatch(): void {
    const match = new Match();
    match.register();
}

export class Match {
    register(): void {
        gc.onMessage(Msg.GCGameMatchSignOutPermissionRequest, () => this.signOutPermission());
        gc.onMessage(Msg.GCGameBotMatchSignOutPermissionRequest, () => this.signOutPermission());
        gc.onMessage(Msg.GCSetMatchHistoryAccess, () => this.setMatchHistoryAccess());
        gc.onMessage(Msg.ServerToGCRequestStatus, () => this.serverStatus());
        gc.onMessage(Msg.GCLeaverDetected, () => this.leaverDetected());
        gc.onMessage(Msg.ServerToGCRealtimeStats, () => this.realtimeStats());
        gc.onMessage(Msg.ServerToGCMatchStateHistory, () => this.matchStateHistory());
        gc.onMessage(Msg.ServerGCUpdateSpectatorCount, () => this.updateSpectatorCount());
        gc.onMessage(Msg.GCLiveScoreboardUpdate, () => this.liveScoreboardUpdate());
        gc.onMessage(Msg.GCSubmitPlayerReport, () => this.submitPlayerReport());
    }

    signOutPermission(): boolean {
        return false;
    }
    setMatchHistoryAccess(): boolean {
        return false;
    }
    serverStatus(): boolean {
        return false;
    }
    leaverDetected(): boolean {
        return false;
    }
    realtimeStats(): boolean {
        return false;
    }
    matchStateHistory(): boolean {
        return false;
    }
    updateSpectatorCount(): boolean {
        return false;
    }
    liveScoreboardUpdate(): boolean {
        return false;
    }
    submitPlayerReport(): boolean {
        return false;
    }
}
