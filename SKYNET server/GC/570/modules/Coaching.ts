import { gc, HandlerContext } from "../framework/gc";
import {
    CMsgClientToGCGetAvailablePrivateCoachingSessions,
    CMsgClientToGCGetAvailablePrivateCoachingSessionsResponse_EResponse,
    CMsgClientToGCGetAvailablePrivateCoachingSessionsResponse,
    CMsgClientToGCGetAvailablePrivateCoachingSessionsSummary,
    CMsgClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse_EResponse,
    CMsgClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse,
    CMsgClientToGCGetCurrentPrivateCoachingSession,
    CMsgClientToGCGetCurrentPrivateCoachingSessionResponse_EResponse,
    CMsgClientToGCGetCurrentPrivateCoachingSessionResponse,
    Routes
} from "../generated/dota";

export function registerCoaching(): void {
    const coaching = new Coaching();
    coaching.register();
}

export class Coaching {
    register(): void {
        gc.on(Routes.GetCurrentPrivateCoachingSession, (ctx) => {
            this.getCurrentPrivateCoachingSession(ctx);
        });
        gc.on(Routes.GetAvailablePrivateCoachingSessions, (ctx) => {
            this.getAvailablePrivateCoachingSessions(ctx);
        });
        gc.on(Routes.GetAvailablePrivateCoachingSessionsSummary, (ctx) => {
            this.getAvailablePrivateCoachingSessionsSummary(ctx);
        });
    }

    getCurrentPrivateCoachingSession(
        ctx: HandlerContext<
            CMsgClientToGCGetCurrentPrivateCoachingSession,
            CMsgClientToGCGetCurrentPrivateCoachingSessionResponse
        >
    ): void {
        ctx.reply({
            result: CMsgClientToGCGetCurrentPrivateCoachingSessionResponse_EResponse.Success
        });
    }

    getAvailablePrivateCoachingSessions(
        ctx: HandlerContext<
            CMsgClientToGCGetAvailablePrivateCoachingSessions,
            CMsgClientToGCGetAvailablePrivateCoachingSessionsResponse
        >
    ): void {
        ctx.reply({
            result: CMsgClientToGCGetAvailablePrivateCoachingSessionsResponse_EResponse.Success,
            availableSessionsList: {
                availableCoachingSessions: []
            }
        });
    }

    getAvailablePrivateCoachingSessionsSummary(
        ctx: HandlerContext<
            CMsgClientToGCGetAvailablePrivateCoachingSessionsSummary,
            CMsgClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse
        >
    ): void {
        ctx.reply({
            result: CMsgClientToGCGetAvailablePrivateCoachingSessionsSummaryResponse_EResponse.Success,
            coachingSessionSummary: {
                coachingSessionCount: 0
            }
        });
    }
}
