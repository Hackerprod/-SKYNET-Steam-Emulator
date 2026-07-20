import { gc, HandlerContext, RawMessageContext } from "../framework/gc";
import {
    CMsgGCRequestStoreSalesData,
    CMsgGCRequestStoreSalesDataResponse,
    CMsgGCStorePurchaseInit,
    CMsgGCStorePurchaseInitResponse,
    Msg,
    Proto,
    Routes
} from "../generated/dota";

export function registerEconomy(): void {
    const economy = new Economy();
    economy.register();
}

export class Economy {
    register(): void {
        gc.on(Routes.RequestStoreSalesData, (ctx) => {
            this.requestStoreSalesData(ctx);
        });
        gc.onMessage(Msg.GCStorePurchaseInit, (ctx) => this.storePurchaseInit(ctx));
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

    private storePurchaseInit(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgGCStorePurchaseInit) as CMsgGCStorePurchaseInit;
        const lineItems = request.lineItems ?? [];
        const txnId = this.createTransactionId(ctx, lineItems.length);

        ctx.logger.info("Economy: StorePurchaseInit lineItems=" + lineItems.length + " txnId=" + txnId);
        ctx.reply<CMsgGCStorePurchaseInitResponse>(Msg.GCStorePurchaseInitResponse, Proto.CMsgGCStorePurchaseInitResponse, {
            result: 1,
            txnId
        });
        return true;
    }

    private cancelUnfinalizedTransactions(ctx: RawMessageContext): boolean {
        ctx.logger.info("Economy: CancelUnfinalizedTransactions ignored");
        return true;
    }

    private aggregateMetrics(ctx: RawMessageContext): boolean {
        ctx.logger.info("Economy: AggregateMetrics ignored");
        return true;
    }

    private createTransactionId(ctx: RawMessageContext, lineItemCount: number): bigint {
        const now = BigInt(ctx.clock.now());
        const account = BigInt(ctx.accountId);
        return (now << 32n) | (account << 8n) | BigInt(lineItemCount & 0xff);
    }
}
