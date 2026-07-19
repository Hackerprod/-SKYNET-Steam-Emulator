import { gc, HandlerContext, RawMessageContext } from "../framework/gc";
import { CMsgGCRequestStoreSalesData, CMsgGCRequestStoreSalesDataResponse, Msg, Routes } from "../generated/dota";

export function registerEconomy(): void {
    const economy = new Economy();
    economy.register();
}

export class Economy {
    register(): void {
        gc.on(Routes.RequestStoreSalesData, (ctx) => {
            this.requestStoreSalesData(ctx);
        });
        gc.onMessage(Msg.ClientToGCCancelUnfinalizedTransactions, (ctx) => this.cancelUnfinalizedTransactions(ctx));
        gc.onMessage(Msg.ClientToGCAggregateMetrics, (ctx) => this.aggregateMetrics(ctx));
    }

    private requestStoreSalesData(
        ctx: HandlerContext<CMsgGCRequestStoreSalesData, CMsgGCRequestStoreSalesDataResponse>
    ): void {
        let version: number = 0;
        if (ctx.request.version) {
            version = ctx.request.version as number;
        }

        const expiration = Math.floor(ctx.clock.now() + 86400) as number;

        ctx.reply({
            version: version,
            expirationTime: expiration
        });
    }

    private cancelUnfinalizedTransactions(ctx: RawMessageContext): boolean {
        ctx.logger.info("Economy: CancelUnfinalizedTransactions ignored");
        return true;
    }

    private aggregateMetrics(ctx: RawMessageContext): boolean {
        ctx.logger.info("Economy: AggregateMetrics ignored");
        return true;
    }
}
