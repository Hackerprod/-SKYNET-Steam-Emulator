import { Messages } from "../Messages";
import { gc } from "../framework/gc";
import { NotificationResult, Routes } from "../generated/dota";

export class Social {
    msg: Messages;

    constructor() {
        this.msg = new Messages();
    }

    handle(type: int32): boolean {
        if (type == this.msg.GCJoinChatChannel()) return this.joinChatChannel();
        if (type == this.msg.GCNotificationsRequest()) return this.notifications();
        if (type == this.msg.ClientToGCEmoticonDataRequest()) return this.emoticonData();
        if (type == this.msg.ClientToGCFindTopSourceTVGames()) return this.findTopSourceTvGames();
        if (type == this.msg.ClientToGCSocialMatchDetailsRequest()) return this.socialMatchDetails();
        if (type == this.msg.ClientToGCSocialMatchPostCommentRequest()) return this.socialMatchPostComment();
        if (type == this.msg.ClientToGCRequestSocialFeed()) return this.requestSocialFeed();
        if (type == this.msg.ClientToGCRequestSocialFeedComments()) return this.requestSocialFeedComments();
        if (type == this.msg.ClientToGCSocialFeedPostCommentRequest()) return this.socialFeedPostComment();
        return false;
    }

    joinChatChannel(): boolean { return false; }
    notifications(): boolean {
        return gc.on(Routes.Notifications, ctx => {
            ctx.reply({
                update: {
                    result: NotificationResult.Success,
                    notifications: []
                }
            });
        });
    }
    emoticonData(): boolean {
        log("Social: EmoticonData deferred until TS can read real user emoticon unlock state");
        return false;
    }
    findTopSourceTvGames(): boolean { return false; }
    socialMatchDetails(): boolean { return false; }
    socialMatchPostComment(): boolean { return false; }
    requestSocialFeed(): boolean { return false; }
    requestSocialFeedComments(): boolean { return false; }
    socialFeedPostComment(): boolean { return false; }
}
