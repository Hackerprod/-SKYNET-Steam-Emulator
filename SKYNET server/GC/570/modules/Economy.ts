import { Messages } from "../Messages";

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

    requestStoreSalesData(): boolean { return false; }
    cancelUnfinalizedTransactions(): boolean { return false; }
    aggregateMetrics(): boolean { return false; }
}
