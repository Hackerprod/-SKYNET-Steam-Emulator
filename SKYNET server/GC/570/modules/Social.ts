import { DotaSocialFeedComment, DotaSocialFeedEvent, gc, HandlerContext, RawMessageContext } from "../framework/gc";
import {
    CMsgClientToGCSocialMatchDetailsRequest,
    CMsgClientToGCSocialMatchPostCommentRequest,
    CMsgClientToGCFindTopSourceTVGames,
    CMsgClientToGCEmoticonDataRequest,
    CMsgClientToGCSocialFeedPostCommentRequest,
    CMsgDOTAJoinChatChannel,
    CMsgDOTAJoinChatChannelResponse,
    CMsgDOTAJoinChatChannelResponse_Result,
    CMsgGCMatchDetailsRequest,
    CMsgGCMatchDetailsResponse,
    CMsgGCToClientSocialMatchDetailsResponse,
    CMsgGCToClientSocialMatchDetailsResponse_Comment,
    CMsgGCToClientSocialMatchPostCommentResponse,
    CMsgGCToClientEmoticonData,
    CMsgGCToClientFindTopSourceTVGamesResponse,
    CMsgGCToClientSocialFeedPostCommentResponse,
    CMsgGenericResult,
    CMsgGCNotificationsMarkReadRequest,
    CMsgGCNotificationsRequest,
    CMsgGCNotificationsResponse,
    CMsgGCNotificationsUpdate_EResult,
    CMsgSocialFeedCommentsRequest,
    CMsgSocialFeedCommentsResponse,
    CMsgSocialFeedCommentsResponse_FeedComment,
    CMsgSocialFeedCommentsResponse_Result,
    CMsgSocialFeedRequest,
    CMsgSocialFeedResponse,
    CMsgSocialFeedResponse_FeedEvent,
    CMsgSocialFeedResponse_Result,
    Msg,
    Proto,
    Routes
} from "../generated/dota";
import { mapMatchDetails } from "./shared/matchMapping";
import { bindPrivateChatRuntimeChannel } from "./Chat";

const SOCIAL_SUCCESS = 1;
const SOCIAL_NOT_FOUND = 0;

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
        gc.onMessage(Msg.GCNotificationsMarkReadRequest, (ctx) => this.notificationsMarkRead(ctx));
        gc.on(Routes.EmoticonData, (ctx) => this.emoticonData(ctx));
        gc.on(Routes.FindTopSourceTvGames, (ctx) => {
            return this.findTopSourceTvGames(ctx);
        });
        gc.on(Routes.MatchDetails, (ctx) => this.socialMatchDetails(ctx));
        gc.onMessage(Msg.ClientToGCSocialMatchDetailsRequest, (ctx) => this.legacySocialMatchDetails(ctx));
        gc.onMessage(Msg.ClientToGCSocialMatchPostCommentRequest, (ctx) => this.legacySocialMatchPostComment(ctx));
        gc.on(Routes.RequestSocialFeed, (ctx) => this.requestSocialFeed(ctx));
        gc.on(Routes.RequestSocialFeedComments, (ctx) => this.requestSocialFeedComments(ctx));
        gc.on(Routes.SocialFeedPostComment, (ctx) => this.socialFeedPostComment(ctx));
    }

    joinChatChannel(ctx: HandlerContext<CMsgDOTAJoinChatChannel, CMsgDOTAJoinChatChannelResponse>): boolean {
        const channelName = normalizeChannelName(ctx.request.channelName);
        const channelType = ctx.request.channelType ?? 0;
        const channel = ctx.services.chat.join(channelName, channelType);

        if (channel === null) {
            ctx.reply({
                response: 0,
                channelName,
                channelType,
                result: CMsgDOTAJoinChatChannelResponse_Result.ChannelFull
            });
            return true;
        }

        bindPrivateChatRuntimeChannel(channel.channelName, channel.channelId, {
            steamId: ctx.steamId,
            accountId: ctx.accountId,
            personaName: ctx.personaName
        });

        ctx.reply({
            response: 0,
            channelName: channel.channelName,
            channelId: channel.channelId,
            maxMembers: channel.maxMembers,
            members: [
                {
                    steamId: ctx.steamId,
                    personaName: ctx.personaName,
                    channelUserId: channel.channelUserId,
                    status: 0
                }
            ],
            channelType: channel.channelType,
            result: CMsgDOTAJoinChatChannelResponse_Result.JoinSuccess,
            channelUserId: channel.channelUserId,
            specialPrivileges: 0
        });

        if (channel.justJoined) {
            ctx.services.chat.broadcast(
                channel.channelId,
                Msg.GCOtherJoinedChannel,
                ctx.encode(Proto.CMsgDOTAOtherJoinedChatChannel, {
                    channelId: channel.channelId,
                    personaName: ctx.personaName,
                    steamId: ctx.steamId,
                    channelUserId: channel.channelUserId
                }),
                false
            );
        }

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

    notificationsMarkRead(ctx: RawMessageContext): boolean {
        ctx.decode(Proto.CMsgGCNotificationsMarkReadRequest) as CMsgGCNotificationsMarkReadRequest;
        ctx.reply<CMsgGenericResult>(Msg.GCGenericResult, Proto.CMsgGenericResult, { eresult: 2 });
        return true;
    }

    emoticonData(ctx: HandlerContext<CMsgClientToGCEmoticonDataRequest, CMsgGCToClientEmoticonData>): boolean {
        const access = ctx.services.social.emoticonAccess();
        ctx.reply({
            emoticonAccess: {
                accountId: access.accountId,
                unlockedEmoticons: access.unlockedEmoticons
            }
        });
        return true;
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
    socialMatchDetails(ctx: HandlerContext<CMsgGCMatchDetailsRequest, CMsgGCMatchDetailsResponse>): boolean {
        const match = ctx.services.stats.getMatchDetails(ctx.request.matchId ?? 0n);
        if (match === null) {
            ctx.reply({ result: SOCIAL_NOT_FOUND, vote: 0 });
            return true;
        }

        ctx.reply({
            result: SOCIAL_SUCCESS,
            match: mapMatchDetails(match),
            vote: 0
        });
        return true;
    }

    legacySocialMatchDetails(ctx: RawMessageContext): boolean {
        const request = ctx.decode(
            Proto.CMsgClientToGCSocialMatchDetailsRequest
        ) as CMsgClientToGCSocialMatchDetailsRequest;
        ctx.reply<CMsgGCToClientSocialMatchDetailsResponse>(
            Msg.GCToClientSocialMatchDetailsResponse,
            Proto.CMsgGCToClientSocialMatchDetailsResponse,
            {
                success: true,
                comments: mapLegacySocialMatchComments(ctx.services.social.matchComments(request.matchId ?? 0n))
            }
        );
        return true;
    }

    legacySocialMatchPostComment(ctx: RawMessageContext): boolean {
        const request = ctx.decode(
            Proto.CMsgClientToGCSocialMatchPostCommentRequest
        ) as CMsgClientToGCSocialMatchPostCommentRequest;
        ctx.reply<CMsgGCToClientSocialMatchPostCommentResponse>(
            Msg.GCToClientSocialMatchPostCommentResponse,
            Proto.CMsgGCToClientSocialMatchPostCommentResponse,
            {
                success: ctx.services.social.postMatchComment(request.matchId ?? 0n, request.comment ?? "")
            }
        );
        return true;
    }

    socialMatchPostComment(
        ctx: HandlerContext<CMsgClientToGCSocialFeedPostCommentRequest, CMsgGCToClientSocialFeedPostCommentResponse>
    ): boolean {
        const targetId = ctx.request.eventId ?? 0n;
        const comment = ctx.request.comment ?? "";
        ctx.reply({
            success:
                ctx.services.social.postMatchComment(targetId, comment) ||
                ctx.services.social.postComment(targetId, comment)
        });
        return true;
    }
    requestSocialFeed(ctx: HandlerContext<CMsgSocialFeedRequest, CMsgSocialFeedResponse>): boolean {
        const accountId = ctx.request.accountId ?? ctx.accountId;
        ctx.reply({
            result: CMsgSocialFeedResponse_Result.Success,
            feedEvents: mapSocialFeedEvents(ctx.services.social.feed(accountId, ctx.request.selfOnly ?? false))
        });
        return true;
    }

    requestSocialFeedComments(
        ctx: HandlerContext<CMsgSocialFeedCommentsRequest, CMsgSocialFeedCommentsResponse>
    ): boolean {
        const feedEventId = ctx.request.feedEventId ?? 0n;
        ctx.reply({
            result: CMsgSocialFeedCommentsResponse_Result.Success,
            feedComments: mapSocialFeedComments(ctx.services.social.comments(feedEventId))
        });
        return true;
    }

    socialFeedPostComment(
        ctx: HandlerContext<CMsgClientToGCSocialFeedPostCommentRequest, CMsgGCToClientSocialFeedPostCommentResponse>
    ): boolean {
        return this.socialMatchPostComment(ctx);
    }
}

function mapSocialFeedEvents(events: DotaSocialFeedEvent[]): CMsgSocialFeedResponse_FeedEvent[] {
    const mapped: CMsgSocialFeedResponse_FeedEvent[] = [];
    for (let i = 0; i < events.length; i++) {
        const item = events[i];
        mapped.push({
            feedEventId: item.feedEventId,
            accountId: item.accountId,
            timestamp: item.timestamp,
            commentCount: item.commentCount,
            eventType: item.eventType,
            eventSubType: item.eventSubType,
            paramBigInt1: item.paramBigInt1,
            paramInt1: item.paramInt1,
            paramInt2: item.paramInt2,
            paramInt3: item.paramInt3,
            paramString: item.paramString
        });
    }

    return mapped;
}

function mapSocialFeedComments(comments: DotaSocialFeedComment[]): CMsgSocialFeedCommentsResponse_FeedComment[] {
    const mapped: CMsgSocialFeedCommentsResponse_FeedComment[] = [];
    for (let i = 0; i < comments.length; i++) {
        const comment = comments[i];
        mapped.push({
            commenterAccountId: comment.commenterAccountId,
            timestamp: comment.timestamp,
            commentText: comment.commentText
        });
    }

    return mapped;
}

function mapLegacySocialMatchComments(
    comments: DotaSocialFeedComment[]
): CMsgGCToClientSocialMatchDetailsResponse_Comment[] {
    const mapped: CMsgGCToClientSocialMatchDetailsResponse_Comment[] = [];
    for (let i = 0; i < comments.length; i++) {
        const comment = comments[i];
        mapped.push({
            accountId: comment.commenterAccountId,
            comment: comment.commentText,
            personaName: comment.personaName,
            timestamp: comment.timestamp
        });
    }

    return mapped;
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
