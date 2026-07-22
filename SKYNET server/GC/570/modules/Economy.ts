import { gc, HandlerContext, RawMessageContext } from "../framework/gc";
import {
    CMsgGCRequestStoreSalesData,
    CMsgGCRequestStoreSalesDataResponse,
    CMsgGCStorePurchaseCancel,
    CMsgGCStorePurchaseCancelResponse,
    CMsgGCStorePurchaseInit,
    CMsgGCStorePurchaseInitResponse,
    CMsgPurchaseHeroRandomRelic,
    CMsgPurchaseHeroRandomRelicResponse,
    CMsgPurchaseItemWithEventPoints,
    CMsgPurchaseItemWithEventPointsResponse,
    CMsgPurchaseItemWithEventPointsResponse_Result,
    EPurchaseHeroRelicResult,
    Msg,
    Proto,
    Routes
} from "../generated/dota";

// The legacy client sends 8256/8257 for hero relic purchases. Current protos
// name the same payload shape as PurchaseHeroRandomRelic, so keep the protocol
// IDs and encode with the generated current descriptor.
const LEGACY_PURCHASE_HERO_RELIC_MESSAGE_ID = 8256;
const LEGACY_PURCHASE_HERO_RELIC_RESPONSE_MESSAGE_ID = 8257;

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
        gc.onMessage(Msg.GCStorePurchaseCancel, (ctx) => this.storePurchaseCancel(ctx));
        gc.onMessage(Msg.PurchaseItemWithEventPoints, (ctx) => this.purchaseItemWithEventPoints(ctx));
        gc.onMessage(LEGACY_PURCHASE_HERO_RELIC_MESSAGE_ID, (ctx) => this.purchaseHeroRelic(ctx, true));
        gc.onMessage(Msg.PurchaseHeroRandomRelic, (ctx) => this.purchaseHeroRelic(ctx, false));
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
        ctx.reply<CMsgGCStorePurchaseInitResponse>(
            Msg.GCStorePurchaseInitResponse,
            Proto.CMsgGCStorePurchaseInitResponse,
            {
                result: 1,
                txnId
            }
        );
        return true;
    }

    private storePurchaseCancel(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgGCStorePurchaseCancel) as CMsgGCStorePurchaseCancel;
        ctx.logger.info("Economy: StorePurchaseCancel txnId=" + (request.txnId ?? 0n));
        ctx.reply<CMsgGCStorePurchaseCancelResponse>(
            Msg.GCStorePurchaseCancelResponse,
            Proto.CMsgGCStorePurchaseCancelResponse,
            {
                result: 1
            }
        );
        return true;
    }

    private purchaseItemWithEventPoints(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgPurchaseItemWithEventPoints) as CMsgPurchaseItemWithEventPoints;
        ctx.logger.info(
            "Economy: PurchaseItemWithEventPoints itemDef=" +
                (request.itemDef ?? 0) +
                " quantity=" +
                (request.quantity ?? 0) +
                " eventId=" +
                (request.eventId ?? 0)
        );
        ctx.reply<CMsgPurchaseItemWithEventPointsResponse>(
            Msg.PurchaseItemWithEventPointsResponse,
            Proto.CMsgPurchaseItemWithEventPointsResponse,
            {
                result: CMsgPurchaseItemWithEventPointsResponse_Result.Success
            }
        );
        return true;
    }

    private purchaseHeroRelic(ctx: RawMessageContext, legacyMessageId: boolean): boolean {
        const request = ctx.decode(Proto.CMsgPurchaseHeroRandomRelic) as CMsgPurchaseHeroRandomRelic;
        ctx.logger.info(
            "Economy: PurchaseHeroRelic heroId=" +
                (request.heroId ?? 0) +
                " rarity=" +
                (request.relicRarity ?? 0) +
                " legacyId=" +
                (legacyMessageId ? "true" : "false")
        );
        ctx.reply<CMsgPurchaseHeroRandomRelicResponse>(
            legacyMessageId ? LEGACY_PURCHASE_HERO_RELIC_RESPONSE_MESSAGE_ID : Msg.PurchaseHeroRandomRelicResponse,
            Proto.CMsgPurchaseHeroRandomRelicResponse,
            {
                result: EPurchaseHeroRelicResult.PurchaseHeroRelicResultSuccess,
                killEaterType: 0
            }
        );
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
