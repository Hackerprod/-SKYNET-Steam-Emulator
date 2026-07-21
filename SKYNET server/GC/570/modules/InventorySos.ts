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
const DEFAULT_FLAGS = 0;
const DEFAULT_ORIGIN = 2;
const STYLE_NONE = 0;
const WELCOME_VERSION = 20n;

interface EconSoCacheOptions {
    readonly onlyEquipped?: boolean;
    readonly econObjectsOnly?: boolean;
    readonly syncVersion?: bigint;
}

export function buildEconSoCacheSubscribed(ctx: GcContextBase): CMsgSOCacheSubscribed {
    return buildEconSoCacheSubscribedForInventory(ctx, ctx.services.items.getInventory());
}

export function buildEconSoCacheSubscribedForInventory(
    ctx: GcContextBase,
    inventory: DotaRuntimeInventory,
    options: EconSoCacheOptions = {}
): CMsgSOCacheSubscribed {
    // This is the authoritative econ snapshot Dota consumes for cosmetics.
    // The emulator grants the imported Dota catalog as the player's owned econ
    // inventory. Dota's armory reads these CSOEconItem objects to decide which
    // cosmetics are equipable, and reads equippedState to render the selected
    // loadout in hero preview and in-game.
    const itemObjects: Uint8Array[] = [];
    const items = econItemsForCache(inventory, options.onlyEquipped === true);
    for (let i = 0; i < items.length; i++) {
        const item = items[i];
        itemObjects.push(
            ctx.encode(
                Proto.CSOEconItem,
                buildEconItem(inventory, item, equipmentForDefIndex(inventory, item.defIndex), i + 1)
            )
        );
    }

    const objects =
        options.econObjectsOnly === true
            ? []
            : [
                  // Keep both account objects under the same SO type. The old Lua GC
                  // sent this pair before item type 1; current Dota accepts the econ
                  // item list only after the account owner cache is established.
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
                      typeId: ITEM_SCHEMA_TYPE_ID,
                      objectData: []
                  }
              ];
    objects.push({
        typeId: ECON_ITEM_TYPE_ID,
        objectData: itemObjects
    });

    return {
        objects,
        version: inventory.version,
        ownerSoid: {
            type: OWNER_TYPE_STEAM_ID,
            id: inventory.steamId
        },
        serviceId: DOTA_SERVICE_ECON,
        serviceList: [DOTA_SERVICE_GAME],
        syncVersion: options.syncVersion ?? 1n
    };
}

export function buildGameOwnerSoCacheSubscribedForInventory(
    inventory: DotaRuntimeInventory,
    syncVersion: bigint
): CMsgSOCacheSubscribed {
    return {
        // Dedicated/listen servers expect the player owner cache under service 0
        // before the service 1 econ item cache. The legacy GC sent this as a
        // separate SOCacheSubscribed targeted at the server SteamID; keeping the
        // owner/econ caches split prevents global loadout objects from changing
        // how the game server interprets hero equippedState entries.
        objects: [],
        version: WELCOME_VERSION,
        ownerSoid: {
            type: OWNER_TYPE_STEAM_ID,
            id: inventory.steamId
        },
        serviceId: DOTA_SERVICE_GAME,
        serviceList: [DOTA_SERVICE_ECON],
        syncVersion
    };
}

export function buildEconItem(
    inventory: DotaRuntimeInventory,
    item: DotaCatalogItem,
    equipment: DotaEquipment[],
    position?: number
): CSOEconItem {
    const equippedState: CSOEconItemEquipped[] = [];
    let selectedStyle = STYLE_NONE;
    for (let i = 0; i < equipment.length; i++) {
        const equipped = equipment[i];
        selectedStyle = equipped.style;
        // newClass/newSlot is the loadout binding. If this array is missing or
        // points at the wrong hero/slot, Dota still sees the item in inventory
        // but does not render it on the hero in-game.
        equippedState.push({
            newClass: equipped.heroId,
            newSlot: equipped.slotId
        });
    }

    return {
        // Item IDs must be stable across the client and dedicated server. The
        // equipment request sends item_id, the GC resolves it back to defIndex,
        // then this deterministic ID lets SO updates replace the same object.
        id: buildDotaItemInstanceId(inventory.steamId, item.defIndex),
        accountId: steamIdToAccountId(inventory.steamId),
        inventory: position ?? inventoryPosition(inventory, item.defIndex),
        defIndex: item.defIndex,
        quantity: DEFAULT_QUANTITY,
        level: DEFAULT_LEVEL,
        quality: item.qualityId === 0 ? DEFAULT_QUALITY : item.qualityId,
        flags: DEFAULT_FLAGS,
        origin: DEFAULT_ORIGIN,
        // style is duplicated at item level because Dota reads it separately
        // from equippedState when applying cosmetic variants.
        style: selectedStyle,
        equippedState
    };
}

function econItemsForCache(inventory: DotaRuntimeInventory, onlyEquipped: boolean): DotaCatalogItem[] {
    const allItems =
        inventory.catalogItems !== undefined && inventory.catalogItems.length > 0
            ? inventory.catalogItems
            : inventory.ownedItems;
    if (!onlyEquipped) {
        return allItems;
    }

    const result: DotaCatalogItem[] = [];
    for (let i = 0; i < allItems.length; i++) {
        const item = allItems[i];
        if (equipmentForDefIndex(inventory, item.defIndex).length > 0) {
            result.push(item);
        }
    }

    return result;
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
    const items =
        inventory.catalogItems !== undefined && inventory.catalogItems.length > 0
            ? inventory.catalogItems
            : inventory.ownedItems;
    let position = DEFAULT_INVENTORY_POSITION;
    for (let i = 0; i < items.length; i++) {
        if (items[i].defIndex === defIndex) {
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
