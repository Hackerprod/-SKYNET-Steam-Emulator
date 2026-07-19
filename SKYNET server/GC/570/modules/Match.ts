import { gc } from "../framework/gc";
import { Msg } from "../generated/dota";

export function registerMatch(): void {
    const match = new Match();
    match.register();
}

export class Match {
    register(): void {
        gc.onMessage(Msg.GCGameMatchSignOutPermissionRequest as number, () => this.signOutPermission());
        gc.onMessage(Msg.GCGameBotMatchSignOutPermissionRequest as number, () => this.signOutPermission());
        gc.onMessage(Msg.GCSetMatchHistoryAccess as number, () => this.setMatchHistoryAccess());
        gc.onMessage(Msg.ServerToGCRequestStatus as number, () => this.serverStatus());
        gc.onMessage(Msg.GCLeaverDetected as number, () => this.leaverDetected());
        gc.onMessage(Msg.ServerToGCRealtimeStats as number, () => this.realtimeStats());
        gc.onMessage(Msg.ServerToGCMatchStateHistory as number, () => this.matchStateHistory());
        gc.onMessage(Msg.ServerGCUpdateSpectatorCount as number, () => this.updateSpectatorCount());
        gc.onMessage(Msg.GCLiveScoreboardUpdate as number, () => this.liveScoreboardUpdate());
        gc.onMessage(Msg.GCSubmitPlayerReport as number, () => this.submitPlayerReport());
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
