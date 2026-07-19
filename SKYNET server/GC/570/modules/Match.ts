import { Messages } from "../Messages";

export class Match {
    msg: Messages;

    constructor() {
        this.msg = new Messages();
    }

    handle(type: int32): boolean {
        if (type == this.msg.GCGameMatchSignOutPermissionRequest()) return this.signOutPermission();
        if (type == this.msg.GCGameMatchSignOutPermissionRequest2()) return this.signOutPermission();
        if (type == this.msg.GCSetMatchHistoryAccess()) return this.setMatchHistoryAccess();
        if (type == this.msg.GCServerToGCRequestStatus()) return this.serverStatus();
        if (type == this.msg.GCLeaverDetected()) return this.leaverDetected();
        if (type == this.msg.GCServerToGCRealtimeStats()) return this.realtimeStats();
        if (type == this.msg.GCServerToGCMatchStateHistory()) return this.matchStateHistory();
        if (type == this.msg.GCServerUpdateSpectatorCount()) return this.updateSpectatorCount();
        if (type == this.msg.GCLiveScoreboardUpdate()) return this.liveScoreboardUpdate();
        if (type == this.msg.GCSubmitPlayerReport()) return this.submitPlayerReport();
        return false;
    }

    signOutPermission(): boolean { return false; }
    setMatchHistoryAccess(): boolean { return false; }
    serverStatus(): boolean { return false; }
    leaverDetected(): boolean { return false; }
    realtimeStats(): boolean { return false; }
    matchStateHistory(): boolean { return false; }
    updateSpectatorCount(): boolean { return false; }
    liveScoreboardUpdate(): boolean { return false; }
    submitPlayerReport(): boolean { return false; }
}
