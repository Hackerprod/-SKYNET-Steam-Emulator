import { gc, HandlerContext, RawMessageContext } from "../framework/gc";
import {
    CMsgGCNotificationsRequest,
    CMsgGCNotificationsResponse,
    CMsgGCNotificationsUpdate_EResult,
    Msg,
    Routes
} from "../generated/dota";

export function registerSocial(): void {
    const social = new Social();
    social.register();
}

export class Social {
    register(): void {
        gc.onMessage(Msg.GCJoinChatChannel as number, () => this.joinChatChannel());
        gc.on(Routes.Notifications, (ctx) => {
            this.notifications(ctx);
        });
        gc.onMessage(Msg.ClientToGCEmoticonDataRequest as number, (ctx) => this.emoticonData(ctx));
        gc.onMessage(Msg.ClientToGCFindTopSourceTVGames as number, () => this.findTopSourceTvGames());
        gc.onMessage(Msg.ClientToGCRequestSocialFeed as number, () => this.requestSocialFeed());
        gc.onMessage(Msg.ClientToGCRequestSocialFeedComments as number, () => this.requestSocialFeedComments());
        gc.onMessage(Msg.ClientToGCSocialFeedPostCommentRequest as number, () => this.socialFeedPostComment());
    }

    joinChatChannel(): boolean {
        return false;
    }
    notifications(ctx: HandlerContext<CMsgGCNotificationsRequest, CMsgGCNotificationsResponse>): void {
        ctx.reply({
            update: {
                result: CMsgGCNotificationsUpdate_EResult.Success,
                notifications: []
            }
        });
    }
    emoticonData(ctx: RawMessageContext): boolean {
        ctx.logger.info("Social: EmoticonData deferred until TS can read real user emoticon unlock state");
        return false;
    }
    findTopSourceTvGames(): boolean {
        return false;
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
