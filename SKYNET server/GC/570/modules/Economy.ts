import { Messages } from "../Messages";
import { gc } from "../framework/gc";
import { Routes } from "../generated/dota";

export class Economy {
    msg: Messages;

    constructor() {
        this.msg = new Messages();
    }

    handle(type: int32): boolean {
        if (type == this.msg.GCRequestStoreSalesData()) return this.requestStoreSalesData();
        if (type == this.msg.ClientToGCCancelUnfinalizedTransactions()) return this.cancelUnfinalizedTransactions();
        if (type == this.msg.ClientToGCAggregateMetrics()) return this.aggregateMetrics();
        return false;
    }

    private requestStoreSalesData(): boolean {
        return gc.on(Routes.RequestStoreSalesData, ctx => {
            let version: int32 = 0;
            if (ctx.request.version) {
                version = ctx.request.version as int32;
            }

            const expiration = Math.floor(now() + 86400) as int32;

            ctx.reply({
                version: version,
                expirationTime: expiration
            });
        });
    }

    private cancelUnfinalizedTransactions(): boolean {
        log("Economy: CancelUnfinalizedTransactions ignored");
        return true;
    }

    private aggregateMetrics(): boolean {
        log("Economy: AggregateMetrics ignored");
        return true;
    }
}
