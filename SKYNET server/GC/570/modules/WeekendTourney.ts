import { RawMessageContext, gc } from "../framework/gc";
import { CMsgGenericResult, CMsgWeekendTourneySchedule, Msg, Proto } from "../generated/dota";

const SUCCESS = 1;

export function registerWeekendTourney(): void {
    const weekendTourney = new WeekendTourney();
    weekendTourney.register();
}

export class WeekendTourney {
    register(): void {
        gc.onMessage(Msg.DOTAGetWeekendTourneySchedule, (ctx) => this.getSchedule(ctx));
        gc.onMessage(Msg.ClientToGCWeekendTourneyOpts, (ctx) => this.weekendTourneyOpts(ctx));
    }

    private getSchedule(ctx: RawMessageContext): boolean {
        ctx.reply<CMsgWeekendTourneySchedule>(Msg.DOTAWeekendTourneySchedule, Proto.CMsgWeekendTourneySchedule, {
            divisions: []
        });
        return true;
    }

    private weekendTourneyOpts(ctx: RawMessageContext): boolean {
        ctx.reply<CMsgGenericResult>(Msg.ClientToGCWeekendTourneyOptsResponse, Proto.CMsgGenericResult, {
            eresult: SUCCESS
        });
        return true;
    }
}
