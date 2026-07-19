import { Messages } from "../Messages";
import { gc } from "../framework/gc";
import { CoachingResponse, Routes } from "../generated/dota";

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

    getCurrentPrivateCoachingSession(): boolean {
        return gc.on(Routes.GetCurrentPrivateCoachingSession, ctx => {
            ctx.reply({
                result: CoachingResponse.Success
            });
        });
    }

    getAvailablePrivateCoachingSessions(): boolean {
        return gc.on(Routes.GetAvailablePrivateCoachingSessions, ctx => {
            ctx.reply({
                result: CoachingResponse.Success,
                availableSessionsList: {
                    availableCoachingSessions: []
                }
            });
        });
    }

    getAvailablePrivateCoachingSessionsSummary(): boolean {
        return gc.on(Routes.GetAvailablePrivateCoachingSessionsSummary, ctx => {
            ctx.reply({
                result: CoachingResponse.Success,
                coachingSessionSummary: {
                    coachingSessionCount: 0
                }
            });
        });
    }
}
