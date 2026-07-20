import { DotaEquipment, DotaRuntimeInventory, HandlerContext, RawMessageContext, gc } from "../framework/gc";
import {
    CMsgClientToGCEquipItems,
    CMsgClientToGCEquipItemsResponse,
    CMsgClientToGCSetItemStyle,
    CMsgClientToGCSetItemStyleResponse,
    CMsgSOCacheSubscriptionRefresh,
    CSOEconItem,
    CMsgSOMultipleObjects_SingleObject,
    CMsgSOSingleObject,
    CMsgClientToGCSetItemStyleResponse_ESetStyle,
    Msg,
    Proto,
    Routes
} from "../generated/dota";
import {
    ECON_ITEM_TYPE_ID,
    ECON_SERVICE_ID,
    OWNER_TYPE_STEAM_ID,
    buildDotaItemInstanceId,
    buildEconItem,
    equipmentForDefIndex
} from "./InventorySos";
import { queueCurrentLobbyServer as queueCurrentLobbyServerMessage } from "./Lobby";

const STYLE_NONE = 0;
const STYLE_DEFAULT_SENTINEL = 255;

export function registerItems(): void {
    const items = new Items();
    items.register();
}

export class Items {
    register(): void {
        gc.on(Routes.EquipItems, (ctx) => this.equipItems(ctx));
        gc.on(Routes.SetItemStyle, (ctx) => this.setItemStyle(ctx));
        gc.onMessage(Msg.SOCacheSubscriptionRefresh, (ctx) => this.cacheSubscriptionRefresh(ctx));
    }

    private equipItems(ctx: HandlerContext<CMsgClientToGCEquipItems, CMsgClientToGCEquipItemsResponse>): boolean {
        const equips = ctx.request.equips ?? [];
        let changed: DotaEquipment[] = [];

        for (let i = 0; i < equips.length; i++) {
            const equip = equips[i];
            const itemId = equip.itemId ?? 0n;
            const heroId = equip.newClass ?? 0;
            const slotId = equip.newSlot ?? 0;
            const style = normalizeStyle(equip.styleIndex ?? STYLE_NONE);
            const itemChanges = ctx.services.items.equipItem(itemId, heroId, slotId, style);
            changed = appendEquipment(changed, itemChanges);
        }

        const inventory = ctx.services.items.getInventory();
        if (changed.length > 0) {
            this.sendClientItemChanges(ctx, inventory, changed);
            this.queueServerItemChanges(ctx, inventory, changed);
        }

        ctx.reply({ soCacheVersionId: inventory.version });
        return true;
    }

    private setItemStyle(ctx: HandlerContext<CMsgClientToGCSetItemStyle, CMsgClientToGCSetItemStyleResponse>): boolean {
        const itemId = ctx.request.itemId ?? 0n;
        const style = normalizeStyle(ctx.request.styleIndex ?? STYLE_NONE);
        const changed = ctx.services.items.setItemStyle(itemId, style);
        const inventory = ctx.services.items.getInventory();

        if (changed.length > 0) {
            this.sendClientItemChanges(ctx, inventory, changed);
            this.queueServerItemChanges(ctx, inventory, changed);
        }

        ctx.reply({
            response:
                changed.length > 0
                    ? CMsgClientToGCSetItemStyleResponse_ESetStyle.KSetStyleSucceeded
                    : CMsgClientToGCSetItemStyleResponse_ESetStyle.KSetStyleFailed
        });
        return true;
    }

    private cacheSubscriptionRefresh(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgSOCacheSubscriptionRefresh) as CMsgSOCacheSubscriptionRefresh;
        const owner = request.ownerSoid;
        ctx.logger.info("SOCacheSubscriptionRefresh ownerType=" + (owner?.type ?? 0) + " ownerId=" + (owner?.id ?? 0n));
        return true;
    }

    private sendClientItemChanges<TRequest, TResponse>(
        ctx: HandlerContext<TRequest, TResponse>,
        inventory: DotaRuntimeInventory,
        changed: DotaEquipment[]
    ): void {
        const objectsModified = this.buildChangedObjects(ctx, inventory, changed);
        if (objectsModified.length === 0) {
            return;
        }

        ctx.send(Msg.SOCacheUpdated, Proto.CMsgSOMultipleObjects, {
            objectsModified,
            version: inventory.version,
            ownerSoid: {
                type: OWNER_TYPE_STEAM_ID,
                id: inventory.steamId
            },
            serviceId: ECON_SERVICE_ID
        });
    }

    private queueServerItemChanges<TRequest, TResponse>(
        ctx: HandlerContext<TRequest, TResponse>,
        inventory: DotaRuntimeInventory,
        changed: DotaEquipment[]
    ): void {
        const defIndexes = distinctChangedDefIndexes(changed);
        for (let i = 0; i < defIndexes.length; i++) {
            const defIndex = defIndexes[i];
            const item = ctx.services.items.getCatalogItem(defIndex);
            if (item !== null) {
                const equipment = equipmentForDefIndex(inventory, defIndex);
                const itemId = buildDotaItemInstanceId(inventory.steamId, item.defIndex);
                if (equipment.length === 0) {
                    queueCurrentLobbyServer(
                        ctx,
                        Msg.SOSingleObjectDestroyed,
                        ctx.encode(Proto.CMsgSOSingleObject, this.buildSingleObject(ctx, inventory, { id: itemId }))
                    );
                }

                queueCurrentLobbyServer(
                    ctx,
                    Msg.SOSingleObject,
                    ctx.encode(
                        Proto.CMsgSOSingleObject,
                        this.buildSingleObject(ctx, inventory, buildEconItem(inventory, item, equipment))
                    )
                );
            }
        }
    }

    private buildChangedObjects<TRequest, TResponse>(
        ctx: HandlerContext<TRequest, TResponse>,
        inventory: DotaRuntimeInventory,
        changed: DotaEquipment[]
    ): CMsgSOMultipleObjects_SingleObject[] {
        const result: CMsgSOMultipleObjects_SingleObject[] = [];
        const defIndexes = distinctChangedDefIndexes(changed);
        for (let i = 0; i < defIndexes.length; i++) {
            const defIndex = defIndexes[i];
            const item = ctx.services.items.getCatalogItem(defIndex);
            if (item !== null) {
                result.push({
                    typeId: ECON_ITEM_TYPE_ID,
                    objectData: ctx.encode(
                        Proto.CSOEconItem,
                        buildEconItem(inventory, item, equipmentForDefIndex(inventory, defIndex))
                    )
                });
            }
        }

        return result;
    }

    private buildSingleObject<TRequest, TResponse>(
        ctx: HandlerContext<TRequest, TResponse>,
        inventory: DotaRuntimeInventory,
        objectData: CSOEconItem
    ): CMsgSOSingleObject {
        return {
            typeId: ECON_ITEM_TYPE_ID,
            objectData: ctx.encode(Proto.CSOEconItem, objectData),
            version: inventory.version,
            ownerSoid: {
                type: OWNER_TYPE_STEAM_ID,
                id: inventory.steamId
            },
            serviceId: ECON_SERVICE_ID
        };
    }
}

function queueCurrentLobbyServer<TRequest, TResponse>(
    ctx: HandlerContext<TRequest, TResponse>,
    messageType: number,
    payload: Uint8Array
): boolean {
    return queueCurrentLobbyServerMessage(ctx, messageType, payload);
}

function appendEquipment(left: DotaEquipment[], right: DotaEquipment[]): DotaEquipment[] {
    for (let i = 0; i < right.length; i++) {
        left.push(right[i]);
    }

    return left;
}

function normalizeStyle(style: number): number {
    return style === STYLE_DEFAULT_SENTINEL ? STYLE_NONE : style;
}

function distinctChangedDefIndexes(changed: DotaEquipment[]): number[] {
    const result: number[] = [];
    for (let i = 0; i < changed.length; i++) {
        const defIndex = changed[i].defIndex;
        if (defIndex === 0 || containsNumber(result, defIndex)) {
            continue;
        }

        result.push(defIndex);
    }

    return result;
}

function containsNumber(values: number[], value: number): boolean {
    for (let i = 0; i < values.length; i++) {
        if (values[i] === value) {
            return true;
        }
    }

    return false;
}
