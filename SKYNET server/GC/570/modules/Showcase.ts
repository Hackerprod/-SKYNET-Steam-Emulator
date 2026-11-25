import { DotaProfileSnapshot, DotaShowcaseLayout, DotaShowcaseLayoutItem, HandlerContext, gc } from "../framework/gc";
import {
    CMsgClientToGCShowcaseGetUserData,
    CMsgClientToGCShowcaseGetUserDataResponse,
    CMsgClientToGCShowcaseGetUserDataResponse_EResponse,
    CMsgClientToGCShowcaseSetUserData,
    CMsgClientToGCShowcaseSetUserDataResponse,
    CMsgClientToGCShowcaseSetUserDataResponse_EResponse,
    CMsgShowcase,
    CMsgShowcaseItem,
    CMsgShowcaseItemHero,
    CMsgShowcaseItemPosition,
    EShowcaseItemFlagHero,
    EShowcaseItemState,
    EShowcaseType,
    Routes
} from "../generated/dota";
import { buildHeroEquippedEconItems } from "./shared/heroLoadout";
import {
    PROFILE_STAT_COMMENDS,
    PROFILE_STAT_FIRST_MATCH,
    PROFILE_STAT_GAMES_PLAYED,
    PROFILE_STAT_MVPS,
    PROFILE_STAT_WINS,
    profileStatScore,
    requestedAccountId
} from "./shared/profileStats";

// Profile showcase (the hero/stat panel of the Dota profile page and the mini
// profile card).
//
// Flow
//   8886 GetUserData: the client asks for the showcase of an account and a
//     type (Profile = the profile page, MiniProfile = the small card). The GC
//     answers with a background item plus a list of positioned items. The
//     layout is the one the account saved with 8888, or a generated default
//     when it never saved one. Items carry only what to draw and where; their
//     data is always hydrated here from the OWNER account (the hero's equipped
//     econ items, the stat scores), never taken from the client.
//   8888 SetUserData: the profile editor saves the whole layout of one type.
//     The layout is validated and normalised (unknown item kinds and values out
//     of range are dropped), persisted for the calling account only, and the
//     validated showcase is returned in 8889 so the client can redraw exactly
//     what was stored. It is built by the same code as 8886, so the next
//     GetUserData matches it (apart from the per-item state flags below).
//
// Default layout: the geometry below is the one observed in a real GC response
// for an account that never customised its profile (hero model on the right,
// three stats on the left of the profile page; a single hero model for the
// mini profile). The hero follows the first featured hero of the account.
//
// Not verified against a real GC: the SetUserData request/response bodies were
// never captured, so the validation limits below are local defensive limits,
// not Valve's rules, and only hero_model, stat and background items are
// supported (other item kinds are dropped and are simply absent from the
// validated showcase).

const SHOWCASE_KIND_HERO_MODEL = 1;
const SHOWCASE_KIND_STAT = 2;

// Ids of the profile backgrounds shipped in the client's
// scripts/profile_backgrounds.txt (id 0 is invalid there). background_id is a
// client resource id, not an inventory item.
const PROFILE_BACKGROUND_FIRST_ID = 1;
const PROFILE_BACKGROUND_LAST_ID = 9;
const DEFAULT_BACKGROUND_ID = PROFILE_BACKGROUND_FIRST_ID;
const DEFAULT_BACKGROUND_DIM_PROFILE = 50;
const DEFAULT_BACKGROUND_DIM_MINI = 100;
const BACKGROUND_PERCENT_MAX = 100;

// Local defensive limits (see header).
const MAX_SHOWCASE_ITEMS = 16;
const COORDINATE_LIMIT = 10000;
const SCALE_MAX = 10000;
const SIZE_MAX = 10000;
const HERO_ID_MAX = 65535;
const ITEM_ID_MAX = 0x7fffffff;
const MAX_ANIMATION_NAME_LENGTH = 64;
const ITEM_FLAGS_MASK = 1;
const HERO_FLAGS_MASK = 31;
const SUPPORTED_STAT_IDS = [
    PROFILE_STAT_WINS,
    PROFILE_STAT_COMMENDS,
    PROFILE_STAT_GAMES_PLAYED,
    PROFILE_STAT_FIRST_MATCH,
    PROFILE_STAT_MVPS
];

export function registerShowcase(): void {
    const showcase = new Showcase();
    showcase.register();
}

export class Showcase {
    register(): void {
        gc.on(Routes.ShowcaseGetUserData, (ctx) => this.getUserData(ctx));
        gc.on(Routes.ShowcaseSetUserData, (ctx) => this.setUserData(ctx));
    }

    private getUserData(
        ctx: HandlerContext<CMsgClientToGCShowcaseGetUserData, CMsgClientToGCShowcaseGetUserDataResponse>
    ): boolean {
        const type = ctx.request.showcaseType ?? EShowcaseType.ShowcaseTypeInvalid;
        const accountId = requestedAccountId(ctx.request.accountId, ctx.accountId);
        const snapshot = ctx.services.profiles.get(accountId);

        let layout: DotaShowcaseLayout | null = null;
        if (type === EShowcaseType.ShowcaseTypeProfile || type === EShowcaseType.ShowcaseTypeMiniProfile) {
            layout = ctx.services.showcase.getLayout(accountId, type) ?? buildDefaultLayout(type, snapshot);
        } else if (type === EShowcaseType.ShowcaseTypeDefaultProfile) {
            // The Default* types ask for the default layout of the matching type.
            layout = buildDefaultLayout(EShowcaseType.ShowcaseTypeProfile, snapshot);
        } else if (type === EShowcaseType.ShowcaseTypeDefaultMiniProfile) {
            layout = buildDefaultLayout(EShowcaseType.ShowcaseTypeMiniProfile, snapshot);
        }

        if (layout === null) {
            ctx.reply({ response: CMsgClientToGCShowcaseGetUserDataResponse_EResponse.UnknownShowcase });
            return true;
        }

        ctx.reply({
            response: CMsgClientToGCShowcaseGetUserDataResponse_EResponse.Success,
            showcase: buildShowcase(ctx, snapshot, layout, [])
        });
        return true;
    }

    private setUserData(
        ctx: HandlerContext<CMsgClientToGCShowcaseSetUserData, CMsgClientToGCShowcaseSetUserDataResponse>
    ): boolean {
        const type = ctx.request.showcaseType ?? EShowcaseType.ShowcaseTypeInvalid;
        // Absent sub-messages may decode as undefined or null; ?? null covers both.
        const submitted = ctx.request.showcase ?? null;
        const editableType =
            type === EShowcaseType.ShowcaseTypeProfile || type === EShowcaseType.ShowcaseTypeMiniProfile;
        // Dedicated servers have no account (accountId 0) and nothing to save for.
        if (ctx.accountId === 0 || !editableType || submitted === null) {
            ctx.reply({ response: CMsgClientToGCShowcaseSetUserDataResponse_EResponse.Invalid });
            return true;
        }

        const submittedItems = submitted.showcaseItems ?? [];
        if (submittedItems.length > MAX_SHOWCASE_ITEMS) {
            // The previous layout stays untouched.
            ctx.reply({ response: CMsgClientToGCShowcaseSetUserDataResponse_EResponse.BudgetExceeded });
            return true;
        }

        const previous = ctx.services.showcase.getLayout(ctx.accountId, type);
        const normalized = normalizeSubmittedItems(type, submittedItems);
        const layout = buildSubmittedLayout(
            type,
            ctx.request.formatVersion ?? 0,
            submitted.background ?? null,
            previous,
            normalized.items
        );
        if (!ctx.services.showcase.saveLayout(type, layout)) {
            ctx.reply({ response: CMsgClientToGCShowcaseSetUserDataResponse_EResponse.InternalError });
            return true;
        }

        const snapshot = ctx.services.profiles.get(ctx.accountId);
        ctx.reply({
            response: CMsgClientToGCShowcaseSetUserDataResponse_EResponse.Success,
            validatedShowcase: buildShowcase(ctx, snapshot, layout, normalized.reassignedIds)
        });
        return true;
    }
}

interface NormalizedItems {
    readonly items: DotaShowcaseLayoutItem[];
    // Ids the GC had to change; reported to the client as MinorModifications
    // in the SetUserData reply only (the state is not persisted).
    readonly reassignedIds: number[];
}

function buildDefaultLayout(type: number, snapshot: DotaProfileSnapshot): DotaShowcaseLayout {
    const isProfile = type === EShowcaseType.ShowcaseTypeProfile;
    const items: DotaShowcaseLayoutItem[] = [];
    // The store always provides at least one featured hero for known accounts;
    // an empty list (defensive) simply leaves the hero out.
    if (snapshot.featuredHeroIds.length > 0) {
        const heroId = snapshot.featuredHeroIds[0];
        items.push(
            isProfile
                ? layoutItem(SHOWCASE_KIND_HERO_MODEL, 3, heroId, 630, 256, 1000, 682, 682, 0, 0, 0)
                : layoutItem(SHOWCASE_KIND_HERO_MODEL, 7, heroId, 86, 23, 1000, 203, 203, 0, 0, 0)
        );
    }

    if (isProfile) {
        items.push(layoutItem(SHOWCASE_KIND_STAT, 4, PROFILE_STAT_GAMES_PLAYED, 43, 341, 2000, 100, 50, 0, 0, 0));
        items.push(layoutItem(SHOWCASE_KIND_STAT, 5, PROFILE_STAT_COMMENDS, 43, 442, 2000, 100, 50, 0, 0, 0));
        items.push(layoutItem(SHOWCASE_KIND_STAT, 6, PROFILE_STAT_MVPS, 43, 543, 2000, 100, 50, 0, 175, 516));
    }

    return {
        showcaseType: type,
        formatVersion: 0,
        backgroundId: DEFAULT_BACKGROUND_ID,
        backgroundDim: isProfile ? DEFAULT_BACKGROUND_DIM_PROFILE : DEFAULT_BACKGROUND_DIM_MINI,
        backgroundBlur: 0,
        items
    };
}

// Builds a layout item with the observed defaults. Hero models use the current
// loadout of the owner (EShowcaseItemFlagHero.UseCurrentLoadout).
function layoutItem(
    kind: number,
    showcaseItemId: number,
    refId: number,
    positionX: number,
    positionY: number,
    scale: number,
    width: number,
    height: number,
    rotation: number,
    attachmentAnchorX: number,
    attachmentAnchorY: number
): DotaShowcaseLayoutItem {
    return {
        showcaseItemId,
        kind,
        refId,
        positionX,
        positionY,
        scale,
        width,
        height,
        rotation,
        parentId: 0,
        parentAttachmentPointId: 0,
        attachmentAnchorX,
        attachmentAnchorY,
        itemFlags: 0,
        heroFlags: kind === SHOWCASE_KIND_HERO_MODEL ? EShowcaseItemFlagHero.ShowcaseItemFlagHeroUseCurrentLoadout : 0,
        heroRotation: 0,
        heroZoom: 0,
        heroAnimationName: "",
        heroAnimationPlaybackSpeed: 0,
        heroAnimationOffset: 0,
        heroSlotIndex: 0,
        heroModelIndex: 0
    };
}

function buildShowcase<TRequest, TResponse>(
    ctx: HandlerContext<TRequest, TResponse>,
    snapshot: DotaProfileSnapshot,
    layout: DotaShowcaseLayout,
    reassignedIds: number[]
): CMsgShowcase {
    const items: CMsgShowcaseItem[] = [];
    for (let i = 0; i < layout.items.length; i++) {
        const item = buildShowcaseItem(ctx, snapshot, layout, layout.items[i], reassignedIds);
        if (item !== null) {
            items.push(item);
        }
    }

    // The real GC sends no moderation_state and an item-less background item
    // (no showcase_item_id) whose loading_screen data is empty.
    return {
        showcaseItems: items,
        background: {
            itemData: {
                background: {
                    data: {},
                    dim: layout.backgroundDim,
                    blur: layout.backgroundBlur,
                    backgroundId: isValidBackgroundId(layout.backgroundId) ? layout.backgroundId : DEFAULT_BACKGROUND_ID
                }
            },
            state: EShowcaseItemState.ShowcaseItemStateOk
        }
    };
}

function buildShowcaseItem<TRequest, TResponse>(
    ctx: HandlerContext<TRequest, TResponse>,
    snapshot: DotaProfileSnapshot,
    layout: DotaShowcaseLayout,
    item: DotaShowcaseLayoutItem,
    reassignedIds: number[]
): CMsgShowcaseItem | null {
    const state =
        reassignedIds.indexOf(item.showcaseItemId) >= 0
            ? EShowcaseItemState.ShowcaseItemStateMinorModifications
            : EShowcaseItemState.ShowcaseItemStateOk;
    const itemPosition: CMsgShowcaseItemPosition = {
        positionX: item.positionX,
        positionY: item.positionY,
        scale: item.scale,
        width: item.width,
        height: item.height,
        rotation: nonZeroOrUndefined(item.rotation),
        parentId: nonZeroOrUndefined(item.parentId),
        parentAttachmentPointId: nonZeroOrUndefined(item.parentAttachmentPointId),
        attachmentAnchorX: nonZeroOrUndefined(item.attachmentAnchorX),
        attachmentAnchorY: nonZeroOrUndefined(item.attachmentAnchorY)
    };

    if (item.kind === SHOWCASE_KIND_HERO_MODEL) {
        const hero: CMsgShowcaseItemHero = {
            data: {
                econItems: buildHeroEquippedEconItems(ctx, snapshot.steamId, item.refId),
                actualHeroId: item.refId
            },
            heroId: item.refId,
            rotation: nonZeroOrUndefined(item.heroRotation),
            flags: item.heroFlags,
            animationName: item.heroAnimationName === "" ? undefined : item.heroAnimationName,
            animationPlaybackSpeed: nonZeroOrUndefined(item.heroAnimationPlaybackSpeed),
            animationOffset: nonZeroOrUndefined(item.heroAnimationOffset),
            zoom: nonZeroOrUndefined(item.heroZoom),
            slotIndex: nonZeroOrUndefined(item.heroSlotIndex),
            modelIndex: nonZeroOrUndefined(item.heroModelIndex)
        };
        return {
            showcaseItemId: item.showcaseItemId,
            itemPosition,
            itemData: { heroModel: hero },
            state,
            // The real profile page sends an explicit item flags value (0) on
            // its hero model; the mini profile does not.
            flags:
                layout.showcaseType === EShowcaseType.ShowcaseTypeProfile || item.itemFlags !== 0
                    ? item.itemFlags
                    : undefined
        };
    }

    if (item.kind === SHOWCASE_KIND_STAT) {
        return {
            showcaseItemId: item.showcaseItemId,
            itemPosition,
            itemData: {
                stat: {
                    // Always computed from the owner's data, never from the client.
                    data: { statScore: profileStatScore(snapshot, item.refId) },
                    statId: item.refId
                }
            },
            state,
            flags: item.itemFlags !== 0 ? item.itemFlags : undefined
        };
    }

    return null;
}

function buildSubmittedLayout(
    type: number,
    formatVersion: number,
    background: CMsgShowcaseItem | null,
    previous: DotaShowcaseLayout | null,
    items: DotaShowcaseLayoutItem[]
): DotaShowcaseLayout {
    const submittedBackground = background?.itemData?.background ?? null;
    const previousBackgroundId =
        previous !== null && isValidBackgroundId(previous.backgroundId) ? previous.backgroundId : DEFAULT_BACKGROUND_ID;
    const submittedBackgroundId = submittedBackground?.backgroundId ?? 0;
    const defaultDim =
        type === EShowcaseType.ShowcaseTypeProfile ? DEFAULT_BACKGROUND_DIM_PROFILE : DEFAULT_BACKGROUND_DIM_MINI;
    return {
        showcaseType: type,
        formatVersion,
        // An invalid or missing background id keeps the previous one.
        backgroundId: isValidBackgroundId(submittedBackgroundId) ? submittedBackgroundId : previousBackgroundId,
        backgroundDim: clampNumber(
            submittedBackground?.dim,
            0,
            BACKGROUND_PERCENT_MAX,
            previous?.backgroundDim ?? defaultDim
        ),
        backgroundBlur: clampNumber(
            submittedBackground?.blur,
            0,
            BACKGROUND_PERCENT_MAX,
            previous?.backgroundBlur ?? 0
        ),
        items
    };
}

// Keeps the supported, in-range items of a SetUserData request and guarantees
// unique, positive showcase item ids. Items the client sent with a missing or
// repeated id get a fresh id above every id seen in the request (accepted or
// not) and are reported back as modified.
function normalizeSubmittedItems(type: number, submittedItems: readonly CMsgShowcaseItem[]): NormalizedItems {
    let nextId = 1;
    for (let i = 0; i < submittedItems.length; i++) {
        const id = submittedItems[i].showcaseItemId ?? 0;
        if (id <= ITEM_ID_MAX && id >= nextId) {
            nextId = id + 1;
        }
    }

    const items: DotaShowcaseLayoutItem[] = [];
    const usedIds: number[] = [];
    const reassignedIds: number[] = [];
    for (let i = 0; i < submittedItems.length; i++) {
        const converted = convertSubmittedItem(type, submittedItems[i]);
        if (converted === null) {
            continue;
        }

        let id = converted.showcaseItemId;
        if (id < 1 || id > ITEM_ID_MAX || usedIds.indexOf(id) >= 0) {
            id = nextId;
            nextId++;
            reassignedIds.push(id);
        }

        usedIds.push(id);
        items.push(relinkLayoutItem(converted, id, converted.parentId));
    }

    // A parent must be one of the stored items; otherwise the item is detached.
    const normalized: DotaShowcaseLayoutItem[] = [];
    for (let i = 0; i < items.length; i++) {
        const item = items[i];
        normalized.push(
            item.parentId !== 0 && usedIds.indexOf(item.parentId) < 0
                ? relinkLayoutItem(item, item.showcaseItemId, 0)
                : item
        );
    }

    return { items: normalized, reassignedIds };
}

// Converts one submitted item to a layout item, or null when the item is not
// supported (other kinds, unknown stat id, hero id out of range) or its
// position is out of range.
function convertSubmittedItem(_type: number, item: CMsgShowcaseItem): DotaShowcaseLayoutItem | null {
    const position = item.itemPosition ?? null;
    if (position === null) {
        return null;
    }

    const positionX = position.positionX ?? 0;
    const positionY = position.positionY ?? 0;
    const scale = position.scale ?? 0;
    const width = position.width ?? 0;
    const height = position.height ?? 0;
    if (
        !isInRange(positionX, -COORDINATE_LIMIT, COORDINATE_LIMIT) ||
        !isInRange(positionY, -COORDINATE_LIMIT, COORDINATE_LIMIT) ||
        !isInRange(scale, 1, SCALE_MAX) ||
        !isInRange(width, 1, SIZE_MAX) ||
        !isInRange(height, 1, SIZE_MAX)
    ) {
        return null;
    }

    const showcaseItemId = item.showcaseItemId ?? 0;
    const rotation = clampNumber(position.rotation, 0, COORDINATE_LIMIT, 0);
    const parentId = position.parentId ?? 0;
    const parentAttachmentPointId = clampNumber(position.parentAttachmentPointId, 0, COORDINATE_LIMIT, 0);
    const attachmentAnchorX = clampNumber(position.attachmentAnchorX, 0, COORDINATE_LIMIT, 0);
    const attachmentAnchorY = clampNumber(position.attachmentAnchorY, 0, COORDINATE_LIMIT, 0);
    const itemFlags = (item.flags ?? 0) & ITEM_FLAGS_MASK;

    const hero = item.itemData?.heroModel ?? null;
    if (hero !== null) {
        const heroId = hero.heroId ?? 0;
        if (!isInRange(heroId, 1, HERO_ID_MAX)) {
            return null;
        }

        const animationName = hero.animationName ?? "";
        return {
            showcaseItemId,
            kind: SHOWCASE_KIND_HERO_MODEL,
            refId: heroId,
            positionX,
            positionY,
            scale,
            width,
            height,
            rotation,
            parentId,
            parentAttachmentPointId,
            attachmentAnchorX,
            attachmentAnchorY,
            itemFlags,
            heroFlags: (hero.flags ?? 0) & HERO_FLAGS_MASK,
            heroRotation: clampNumber(hero.rotation, 0, COORDINATE_LIMIT, 0),
            heroZoom: clampNumber(hero.zoom, 0, SCALE_MAX, 0),
            heroAnimationName: animationName.length <= MAX_ANIMATION_NAME_LENGTH ? animationName : "",
            heroAnimationPlaybackSpeed: clampNumber(hero.animationPlaybackSpeed, 0, SCALE_MAX, 0),
            heroAnimationOffset: clampNumber(hero.animationOffset, 0, SCALE_MAX, 0),
            heroSlotIndex: clampNumber(hero.slotIndex, 0, COORDINATE_LIMIT, 0),
            heroModelIndex: clampNumber(hero.modelIndex, 0, COORDINATE_LIMIT, 0)
        };
    }

    const stat = item.itemData?.stat ?? null;
    if (stat !== null) {
        const statId = stat.statId ?? 0;
        if (SUPPORTED_STAT_IDS.indexOf(statId) < 0) {
            return null;
        }

        return {
            showcaseItemId,
            kind: SHOWCASE_KIND_STAT,
            refId: statId,
            positionX,
            positionY,
            scale,
            width,
            height,
            rotation,
            parentId,
            parentAttachmentPointId,
            attachmentAnchorX,
            attachmentAnchorY,
            itemFlags,
            heroFlags: 0,
            heroRotation: 0,
            heroZoom: 0,
            heroAnimationName: "",
            heroAnimationPlaybackSpeed: 0,
            heroAnimationOffset: 0,
            heroSlotIndex: 0,
            heroModelIndex: 0
        };
    }

    return null;
}

// Returns a copy of a layout item with a different id and parent.
function relinkLayoutItem(
    item: DotaShowcaseLayoutItem,
    showcaseItemId: number,
    parentId: number
): DotaShowcaseLayoutItem {
    return {
        showcaseItemId,
        kind: item.kind,
        refId: item.refId,
        positionX: item.positionX,
        positionY: item.positionY,
        scale: item.scale,
        width: item.width,
        height: item.height,
        rotation: item.rotation,
        parentId,
        parentAttachmentPointId: item.parentAttachmentPointId,
        attachmentAnchorX: item.attachmentAnchorX,
        attachmentAnchorY: item.attachmentAnchorY,
        itemFlags: item.itemFlags,
        heroFlags: item.heroFlags,
        heroRotation: item.heroRotation,
        heroZoom: item.heroZoom,
        heroAnimationName: item.heroAnimationName,
        heroAnimationPlaybackSpeed: item.heroAnimationPlaybackSpeed,
        heroAnimationOffset: item.heroAnimationOffset,
        heroSlotIndex: item.heroSlotIndex,
        heroModelIndex: item.heroModelIndex
    };
}
function isValidBackgroundId(backgroundId: number): boolean {
    return isInRange(backgroundId, PROFILE_BACKGROUND_FIRST_ID, PROFILE_BACKGROUND_LAST_ID);
}

function isInRange(value: number, min: number, max: number): boolean {
    return value >= min && value <= max;
}

function clampNumber(value: number | undefined, min: number, max: number, fallback: number): number {
    if (value === undefined) {
        return fallback;
    }

    return Math.min(Math.max(value, min), max);
}

function nonZeroOrUndefined(value: number): number | undefined {
    return value === 0 ? undefined : value;
}
