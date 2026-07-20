import { DotaCatalogItem, DotaEquipment, DotaRuntimeInventory, GcContextBase } from "../framework/gc";
import {
    CMsgSOCacheSubscribed,
    CSODOTAGameAccountClient,
    CSOEconGameAccountClient,
    CSOEconItem,
    CSOEconItemEquipped,
    Proto
} from "../generated/dota";

export const ECON_ITEM_TYPE_ID = 1;
export const OWNER_TYPE_STEAM_ID = 1;
export const ECON_SERVICE_ID = 1;

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

export function buildEconSoCacheSubscribed(ctx: GcContextBase): CMsgSOCacheSubscribed {
    return buildEconSoCacheSubscribedForInventory(ctx, ctx.services.items.getInventory());
}

export function buildEconSoCacheSubscribedForInventory(
    ctx: GcContextBase,
    inventory: DotaRuntimeInventory
): CMsgSOCacheSubscribed {
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
                objectData: [
                    ctx.encode(
                        Proto.CSODOTAGameAccountClient,
                        buildDotaGameAccount(steamIdToAccountId(inventory.steamId))
                    )
                ]
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
            id: inventory.steamId
        },
        serviceId: DOTA_SERVICE_ECON,
        serviceList: [DOTA_SERVICE_GAME],
        syncVersion: 1n
    };
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

export function buildDotaItemInstanceId(steamIdValue: bigint, defIndex: number): bigint {
    const accountBits = steamIdValue & 0xffffffffn;
    return 0x7000000000000000n | (accountBits << 20n) | BigInt(defIndex);
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

function steamIdToAccountId(steamIdValue: bigint): number {
    const base = 76561197960265728n;
    if (steamIdValue >= base) {
        return Number(steamIdValue - base);
    }

    return Number(steamIdValue & 0xffffffffn);
}
