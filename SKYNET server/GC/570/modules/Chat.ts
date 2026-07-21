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

        const text = request.text ?? "";
        let message: CMsgDOTAChatMessage = {
            accountId: ctx.accountId,
            channelId,
            personaName: ctx.personaName,
            text,
            timestamp: ctx.clock.now(),
            channelUserId: channel.channelUserId,
            shareProfileAccountId: request.shareProfileAccountId,
            sharePartyId: request.sharePartyId,
            shareLobbyId: request.shareLobbyId,
            chatWheelMessage: request.chatWheelMessage
        };

        // Dota locally echoes the sender's normal channel text. The GC only
        // fans the sanitized message out to other channel members. Command
        // result payloads are preserved only for explicit command messages so
        // decoded default dice/coin fields cannot turn plain chat into /roll.
        if (text.trim().startsWith("/roll")) {
            message = { ...message, diceRoll: request.diceRoll };
        } else if (text.trim().startsWith("/flip")) {
            message = { ...message, coinFlip: request.coinFlip };
        }

        ctx.services.chat.broadcast(channelId, Msg.GCChatMessage, ctx.encode(Proto.CMsgDOTAChatMessage, message), false);
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
