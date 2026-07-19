import { gc, HandlerContext, RawMessageContext } from "../framework/gc";
import {
    CMsgClientToGCFindTopSourceTVGames,
    CMsgDOTAJoinChatChannel,
    CMsgDOTAJoinChatChannelResponse,
    CMsgGCToClientFindTopSourceTVGamesResponse,
    CMsgGCNotificationsRequest,
    CMsgGCNotificationsResponse,
    CMsgGCNotificationsUpdate_EResult,
    Msg,
    Routes
} from "../generated/dota";

const JOIN_CHAT_SUCCESS = 0;
const JOIN_CHAT_RESPONSE_JOINED = 1;
const DEFAULT_CHAT_MAX_MEMBERS = 250;

export function registerSocial(): void {
    const social = new Social();
    social.register();
}

export class Social {
    register(): void {
        gc.on(Routes.JoinChatChannel, (ctx) => {
            return this.joinChatChannel(ctx);
        });
        gc.on(Routes.Notifications, (ctx) => {
            return this.notifications(ctx);
        });
        gc.onMessage(Msg.ClientToGCEmoticonDataRequest, (ctx) => this.emoticonData(ctx));
        gc.on(Routes.FindTopSourceTvGames, (ctx) => {
            return this.findTopSourceTvGames(ctx);
        });
        gc.onMessage(Msg.ClientToGCRequestSocialFeed, () => this.requestSocialFeed());
        gc.onMessage(Msg.ClientToGCRequestSocialFeedComments, () => this.requestSocialFeedComments());
        gc.onMessage(Msg.ClientToGCSocialFeedPostCommentRequest, () => this.socialFeedPostComment());
    }

    joinChatChannel(ctx: HandlerContext<CMsgDOTAJoinChatChannel, CMsgDOTAJoinChatChannelResponse>): boolean {
        const channelName = normalizeChannelName(ctx.request.channelName);
        const channelType = ctx.request.channelType ?? 0;

        ctx.reply({
            response: JOIN_CHAT_RESPONSE_JOINED,
            channelName,
            channelId: buildStableChatChannelId(channelName, channelType),
            maxMembers: DEFAULT_CHAT_MAX_MEMBERS,
            members: [
                {
                    steamId: ctx.steamId,
                    personaName: ctx.personaName,
                    channelUserId: ctx.accountId,
                    status: 0
                }
            ],
            channelType,
            result: JOIN_CHAT_SUCCESS,
            channelUserId: ctx.accountId
        });
        return true;
    }

    notifications(ctx: HandlerContext<CMsgGCNotificationsRequest, CMsgGCNotificationsResponse>): boolean {
        ctx.reply({
            update: {
                result: CMsgGCNotificationsUpdate_EResult.Success,
                notifications: []
            }
        });
        return true;
    }
    emoticonData(ctx: RawMessageContext): boolean {
        ctx.logger.info("Social: EmoticonData deferred until TS can read real user emoticon unlock state");
        return false;
    }
    findTopSourceTvGames(
        ctx: HandlerContext<CMsgClientToGCFindTopSourceTVGames, CMsgGCToClientFindTopSourceTVGamesResponse>
    ): boolean {
        ctx.reply({
            searchKey: ctx.request.searchKey ?? "",
            leagueId: ctx.request.leagueId ?? 0,
            heroId: ctx.request.heroId ?? 0,
            startGame: ctx.request.startGame ?? 0,
            numGames: 0,
            gameListIndex: ctx.request.gameListIndex ?? 0,
            gameList: [],
            specificGames: false
        });
        return true;
    }
    socialMatchDetails(): boolean {
        return false;
    }
    socialMatchPostComment(): boolean {
        return false;
    }
    requestSocialFeed(): boolean {
        return false;
    }
    requestSocialFeedComments(): boolean {
        return false;
    }
    socialFeedPostComment(): boolean {
        return false;
    }
}

function normalizeChannelName(channelName: string | undefined): string {
    if (channelName === undefined) {
        return "regional";
    }

    const trimmed = channelName.trim();
    if (trimmed.length === 0) {
        return "regional";
    }

    return trimmed;
}

function buildStableChatChannelId(channelName: string, channelType: number): bigint {
    const bytes = toUtf8Bytes(channelType + ":" + channelName.toLowerCase());
    const limit = 18446744073709551616n;
    let hash = 14695981039346656037n;
    for (let i = 0; i < bytes.length; i++) {
        hash = hash ^ BigInt(bytes[i]);
        hash = (hash * 1099511628211n) % limit;
    }

    if (hash === 0n) {
        return 1n;
    }

    return hash;
}

function toUtf8Bytes(value: string): Uint8Array {
    const bytes: number[] = [];
    for (let i = 0; i < value.length; i++) {
        let codePoint = value.charCodeAt(i);
        if (codePoint >= 0xd800 && codePoint <= 0xdbff && i + 1 < value.length) {
            const next = value.charCodeAt(i + 1);
            if (next >= 0xdc00 && next <= 0xdfff) {
                codePoint = 0x10000 + (codePoint - 0xd800) * 0x400 + (next - 0xdc00);
                i++;
            }
        }

        appendUtf8(bytes, codePoint);
    }

    return new Uint8Array(bytes);
}

function appendUtf8(bytes: number[], codePoint: number): void {
    if (codePoint <= 0x7f) {
        bytes.push(codePoint);
    } else if (codePoint <= 0x7ff) {
        bytes.push(0xc0 | (codePoint >> 6));
        bytes.push(0x80 | (codePoint & 0x3f));
    } else if (codePoint <= 0xffff) {
        bytes.push(0xe0 | (codePoint >> 12));
        bytes.push(0x80 | ((codePoint >> 6) & 0x3f));
        bytes.push(0x80 | (codePoint & 0x3f));
    } else {
        bytes.push(0xf0 | (codePoint >> 18));
        bytes.push(0x80 | ((codePoint >> 12) & 0x3f));
        bytes.push(0x80 | ((codePoint >> 6) & 0x3f));
        bytes.push(0x80 | (codePoint & 0x3f));
    }
}
