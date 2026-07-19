import { DotaSocialFeedComment, DotaSocialFeedEvent, gc, HandlerContext, RawMessageContext } from "../framework/gc";
import {
    CMsgClientToGCFindTopSourceTVGames,
    CMsgClientToGCSocialFeedPostCommentRequest,
    CMsgDOTAJoinChatChannel,
    CMsgDOTAJoinChatChannelResponse,
    CMsgDOTAJoinChatChannelResponse_Result,
    CMsgGCToClientFindTopSourceTVGamesResponse,
    CMsgGCToClientSocialFeedPostCommentResponse,
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

        ctx.reply({
            response: 0,
            channelName: channel.channelName,
            channelId: channel.channelId,
            maxMembers: channel.maxMembers,
            members: channel.members.map((member) => ({
                steamId: member.steamId,
                personaName: member.personaName,
                channelUserId: member.channelUserId,
                status: 0
            })),
            channelType: channel.channelType,
            result: CMsgDOTAJoinChatChannelResponse_Result.JoinSuccess,
            gcInitiatedJoin: false,
            channelUserId: channel.channelUserId,
            welcomeMessage: "",
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
        ctx.reply({
            success: ctx.services.social.postComment(ctx.request.eventId ?? 0n, ctx.request.comment ?? "")
        });
        return true;
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
