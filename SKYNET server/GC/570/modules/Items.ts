import { Messages } from "../Messages";

export class Items {
    msg: Messages;

    constructor() {
        this.msg = new Messages();
    }

    handle(type: int32): boolean {
        if (type == this.msg.ClientToGCEquipItems()) return this.equipItems();
        if (type == this.msg.ClientToGCSetItemStyle()) return this.setItemStyle();
        if (type == this.msg.SOCacheSubscriptionRefresh()) return this.cacheSubscriptionRefresh();
        return false;
    }

    equipItems(): boolean { return false; }
    setItemStyle(): boolean { return false; }
    cacheSubscriptionRefresh(): boolean { return false; }
}
