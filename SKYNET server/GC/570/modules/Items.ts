import {
    DotaCatalogItem,
    DotaEquipment,
    DotaRuntimeInventory,
    HandlerContext,
    RawMessageContext,
    gc
} from "../framework/gc";
import {
    CMsgClientToGCEquipItems,
    CMsgClientToGCEquipItemsResponse,
    CMsgClientToGCSetItemStyle,
    CMsgClientToGCSetItemStyleResponse,
    CMsgSOCacheSubscribed,
    CMsgSOCacheSubscriptionRefresh,
    CSODOTAGameAccountClient,
    CSOEconItem,
    CSOEconGameAccountClient,
    CSOEconItemEquipped,
    CMsgSOMultipleObjects_SingleObject,
    CMsgSOSingleObject,
    CMsgClientToGCSetItemStyleResponse_ESetStyle,
    Msg,
    Proto,
    Routes
} from "../generated/dota";

const ECON_ITEM_TYPE_ID = 1;
const OWNER_TYPE_STEAM_ID = 1;
const ECON_SERVICE_ID = 1;
const DOTA_SERVICE_GAME = 0;
const DOTA_SERVICE_ECON = 1;
const GAME_ACCOUNT_TYPE_ID = 7;
const ITEM_SCHEMA_TYPE_ID = 2010;
const DEFAULT_INVENTORY_POSITION = 1;
const DEFAULT_QUANTITY = 1;
const DEFAULT_LEVEL = 1;
const DEFAULT_QUALITY = 6;
const DEFAULT_ORIGIN = 2;
const STYLE_NONE = 0;
const STYLE_DEFAULT_SENTINEL = 255;

export function registerItems(): void {
    const items = new Items();
    items.register();
}

export function buildEconSoCacheSubscribed<TRequest, TResponse>(
    ctx: HandlerContext<TRequest, TResponse>
): CMsgSOCacheSubscribed {
    const inventory = ctx.services.items.getInventory();
    const itemObjects: Uint8Array[] = [];
    for (let i = 0; i < inventory.ownedItems.length; i++) {
        const item = inventory.ownedItems[i];
        itemObjects.push(
            ctx.encode(
                Proto.CSOEconItem,
                buildEconItem(inventory, item, equipmentForDefIndex(inventory, item.defIndex))
            )
        );
    }

    return {
        objects: [
            {
                typeId: GAME_ACCOUNT_TYPE_ID,
                objectData: [ctx.encode(Proto.CSOEconGameAccountClient, buildEconGameAccount())]
            },
            {
                typeId: GAME_ACCOUNT_TYPE_ID,
                objectData: [ctx.encode(Proto.CSODOTAGameAccountClient, buildDotaGameAccount(ctx.accountId))]
            },
            {
                typeId: ITEM_SCHEMA_TYPE_ID
            },
            {
                typeId: ECON_ITEM_TYPE_ID,
                objectData: itemObjects
            }
        ],
        version: inventory.version,
        ownerSoid: {
            type: OWNER_TYPE_STEAM_ID,
            id: ctx.steamId
        },
        serviceId: DOTA_SERVICE_ECON,
        serviceList: [DOTA_SERVICE_GAME],
        syncVersion: 1n
    };
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
                    ctx.services.items.queueCurrentLobbyServer(
                        Msg.SOSingleObjectDestroyed,
                        ctx.encode(Proto.CMsgSOSingleObject, this.buildSingleObject(ctx, inventory, { id: itemId }))
                    );
                }

                ctx.services.items.queueCurrentLobbyServer(
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

export function buildEconItem(
    inventory: DotaRuntimeInventory,
    item: DotaCatalogItem,
    equipment: DotaEquipment[]
): CSOEconItem {
    const equippedState: CSOEconItemEquipped[] = [];
    let selectedStyle = STYLE_NONE;
    for (let i = 0; i < equipment.length; i++) {
        const equipped = equipment[i];
        selectedStyle = equipped.style;
        equippedState.push({
            newClass: equipped.heroId,
            newSlot: equipped.slotId
        });
    }

    return {
        id: buildDotaItemInstanceId(inventory.steamId, item.defIndex),
        accountId: steamIdToAccountId(inventory.steamId),
        inventory: inventoryPosition(inventory, item.defIndex),
        defIndex: item.defIndex,
        quantity: DEFAULT_QUANTITY,
        level: DEFAULT_LEVEL,
        quality: item.qualityId === 0 ? DEFAULT_QUALITY : item.qualityId,
        origin: DEFAULT_ORIGIN,
        style: selectedStyle,
        equippedState
    };
}

function buildEconGameAccount(): CSOEconGameAccountClient {
    return {
        additionalBackpackSlots: 0,
        trialAccount: false,
        eligibleForOnlinePlay: true,
        needToChooseMostHelpfulFriend: false,
        inCoachesList: false,
        tradeBanExpiration: 0,
        duelBanExpiration: 0,
        madeFirstPurchase: false
    };
}

function buildDotaGameAccount(accountId: number): CSODOTAGameAccountClient {
    return {
        accountId
    };
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

export function equipmentForDefIndex(inventory: DotaRuntimeInventory, defIndex: number): DotaEquipment[] {
    const result: DotaEquipment[] = [];
    for (let i = 0; i < inventory.equipment.length; i++) {
        const item = inventory.equipment[i];
        if (item.defIndex === defIndex) {
            result.push(item);
        }
    }

    return result;
}

function inventoryPosition(inventory: DotaRuntimeInventory, defIndex: number): number {
    let position = DEFAULT_INVENTORY_POSITION;
    for (let i = 0; i < inventory.ownedItems.length; i++) {
        if (inventory.ownedItems[i].defIndex === defIndex) {
            return position;
        }

        position++;
    }

    return DEFAULT_INVENTORY_POSITION;
}

function buildDotaItemInstanceId(steamIdValue: bigint, defIndex: number): bigint {
    const accountBits = steamIdValue & 0xffffffffn;
    return 0x7000000000000000n | (accountBits << 20n) | BigInt(defIndex);
}

function steamIdToAccountId(steamIdValue: bigint): number {
    const base = 76561197960265728n;
    if (steamIdValue >= base) {
        return Number(steamIdValue - base);
    }

    return Number(steamIdValue & 0xffffffffn);
}
