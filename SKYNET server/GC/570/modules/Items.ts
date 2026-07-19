import { gc } from "../framework/gc";
import { Msg } from "../generated/dota";

export function registerItems(): void {
    const items = new Items();
    items.register();
}

export class Items {
    register(): void {
        gc.onMessage(Msg.ClientToGCEquipItems as number, () => this.equipItems());
        gc.onMessage(Msg.ClientToGCSetItemStyle as number, () => this.setItemStyle());
        gc.onMessage(Msg.SOCacheSubscriptionRefresh as number, () => this.cacheSubscriptionRefresh());
    }

    equipItems(): boolean {
        return false;
    }
    setItemStyle(): boolean {
        return false;
    }
    cacheSubscriptionRefresh(): boolean {
        return false;
    }
}
