import { Messages } from "../Messages";

export class Coaching {
    msg: Messages;

    constructor() {
        this.msg = new Messages();
    }

    handle(type: int32): boolean {
        if (type == this.msg.ClientToGCGetCurrentPrivateCoachingSession()) return this.getCurrentPrivateCoachingSession();
        if (type == this.msg.ClientToGCGetAvailablePrivateCoachingSessions()) return this.getAvailablePrivateCoachingSessions();
        if (type == this.msg.ClientToGCGetAvailablePrivateCoachingSessionsSummary()) return this.getAvailablePrivateCoachingSessionsSummary();
        return false;
    }

    getCurrentPrivateCoachingSession(): boolean { return false; }
    getAvailablePrivateCoachingSessions(): boolean { return false; }
    getAvailablePrivateCoachingSessionsSummary(): boolean { return false; }
}
