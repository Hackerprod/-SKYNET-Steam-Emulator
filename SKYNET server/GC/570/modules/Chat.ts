import { gc, RawMessageContext } from "../framework/gc";
import { CMsgDOTAChatMessage, CMsgDOTALeaveChatChannel, Msg, Proto } from "../generated/dota";

export function registerChat(): void {
    const chat = new Chat();
    chat.register();
}

export class Chat {
    register(): void {
        gc.onMessage(Msg.GCChatMessage, (ctx) => this.chatMessage(ctx));
        gc.onMessage(Msg.GCLeaveChatChannel, (ctx) => this.leaveChatChannel(ctx));
    }

    chatMessage(ctx: RawMessageContext): boolean {
        const request = ctx.decode<CMsgDOTAChatMessage>(Proto.CMsgDOTAChatMessage);
        const channelId = request.channelId ?? 0n;
        const channel = ctx.services.chat.get(channelId);
        if (channel === null || !channel.isMember) {
            ctx.logger.info(
                "Chat: dropped message from account " + ctx.accountId + " for unknown channel " + channelId
            );
            return true;
        }

        const message: CMsgDOTAChatMessage = {
            accountId: ctx.accountId,
            channelId,
            personaName: ctx.personaName,
            text: request.text ?? "",
            timestamp: ctx.clock.now(),
            channelUserId: channel.channelUserId,
            coinFlip: request.coinFlip,
            diceRoll: request.diceRoll,
            shareProfileAccountId: request.shareProfileAccountId,
            sharePartyId: request.sharePartyId,
            shareLobbyId: request.shareLobbyId,
            chatWheelMessage: request.chatWheelMessage
        };

        ctx.services.chat.broadcast(channelId, Msg.GCChatMessage, ctx.encode(Proto.CMsgDOTAChatMessage, message));
        return true;
    }

    leaveChatChannel(ctx: RawMessageContext): boolean {
        const request = ctx.decode<CMsgDOTALeaveChatChannel>(Proto.CMsgDOTALeaveChatChannel);
        const channelId = request.channelId ?? 0n;
        const channel = ctx.services.chat.get(channelId);
        if (channel === null || !channel.isMember) {
            return true;
        }

        ctx.services.chat.leave(channelId);
        ctx.services.chat.broadcast(
            channelId,
            Msg.GCOtherLeftChannel,
            ctx.encode(Proto.CMsgDOTAOtherLeftChatChannel, {
                channelId,
                steamId: ctx.steamId,
                channelUserId: channel.channelUserId
            }),
            false
        );
        return true;
    }
}
