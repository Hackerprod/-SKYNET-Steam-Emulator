import { HandlerContext } from "../../framework/gc";
import { CSOEconItem } from "../../generated/dota";
import { buildEconItem, equipmentForDefIndex } from "../InventorySos";

// Builds the econ items a given account has equipped on one hero, for profile
// payloads that render a hero with its loadout (profile FeaturedHeroes and the
// showcase hero_model).
//
// The inventory MUST be read for the account that owns the profile. The host
// getInventory() falls back to the caller's inventory when no steam id is
// given (or when it is zero), which would show the viewer's items on someone
// else's hero. An unknown owner (steam id 0) therefore yields no items.
//
// Only the bindings for this hero are emitted in equipped_state: the client
// expects a single new_class entry matching the rendered hero.
export function buildHeroEquippedEconItems<TRequest, TResponse>(
    ctx: HandlerContext<TRequest, TResponse>,
    steamId: bigint,
    heroId: number
): CSOEconItem[] {
    const econItems: CSOEconItem[] = [];
    if (steamId === 0n) {
        return econItems;
    }

    const inventory = ctx.services.items.getInventory(steamId);
    const seenDefIndexes: number[] = [];
    for (let i = 0; i < inventory.equipment.length; i++) {
        const equipped = inventory.equipment[i];
        if (equipped.heroId !== heroId || seenDefIndexes.indexOf(equipped.defIndex) >= 0) {
            continue;
        }

        const catalogItem = ctx.services.items.getCatalogItem(equipped.defIndex);
        if (catalogItem === null) {
            continue;
        }

        seenDefIndexes.push(equipped.defIndex);
        const heroEquipment = equipmentForDefIndex(inventory, equipped.defIndex).filter(
            (binding) => binding.heroId === heroId
        );
        econItems.push(buildEconItem(inventory, catalogItem, heroEquipment));
    }

    return econItems;
}
