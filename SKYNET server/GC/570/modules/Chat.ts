import { gc, RawMessageContext } from "../framework/gc";
import {
    CMsgClientToGCPrivateChatDemote,
    CMsgClientToGCPrivateChatInvite,
    CMsgClientToGCPrivateChatKick,
    CMsgClientToGCPrivateChatPromote,
    CMsgDOTAChatMessage,
    CMsgDOTALeaveChatChannel,
    CMsgDOTARequestChatChannelListResponse,
    CMsgGCToClientPrivateChatResponse,
    CMsgGCToClientPrivateChatResponse_Result,
    Msg,
    Proto
} from "../generated/dota";

const DEFAULT_PUBLIC_CHANNEL_NAME = "regional";
const DEFAULT_PUBLIC_CHANNEL_TYPE = 1;
const PRIVATE_CHAT_CHANNEL_TYPE = 7;

interface PrivateChatMember {
    steamId: bigint;
    accountId: number;
    personaName: string;
}

interface PrivateChatChannel {
    id: number;
    name: string;
    members: PrivateChatMember[];
    admins: number[];
}

const publicChannels: { name: string; type: number; members: number }[] = [
    { name: DEFAULT_PUBLIC_CHANNEL_NAME, type: DEFAULT_PUBLIC_CHANNEL_TYPE, members: 0 }
];
const privateChats = new Map<string, PrivateChatChannel>();
const privateChatsById = new Map<number, PrivateChatChannel>();
const privateChatsByRuntimeChannelId = new Map<bigint, PrivateChatChannel>();
let nextPrivateChatId = 900000;

export function registerChat(): void {
    const chat = new Chat();
    chat.register();
}

export class Chat {
    register(): void {
        gc.onMessage(Msg.GCRequestChatChannelList, (ctx) => this.requestChatChannelList(ctx));
        gc.onMessage(Msg.GCChatMessage, (ctx) => this.chatMessage(ctx));
        gc.onMessage(Msg.GCLeaveChatChannel, (ctx) => this.leaveChatChannel(ctx));
        gc.onMessage(Msg.ClientToGCPrivateChatInvite, (ctx) => this.privateChatInvite(ctx));
        gc.onMessage(Msg.ClientToGCPrivateChatKick, (ctx) => this.privateChatKick(ctx));
        gc.onMessage(Msg.ClientToGCPrivateChatPromote, (ctx) => this.privateChatPromote(ctx));
        gc.onMessage(Msg.ClientToGCPrivateChatDemote, (ctx) => this.privateChatDemote(ctx));
    }

    requestChatChannelList(ctx: RawMessageContext): boolean {
        const channels = publicChannels.map((channel) => ({
            channelName: channel.name,
            numMembers: channel.members,
            channelType: channel.type
        }));
        privateChats.forEach((channel) => {
            if (hasPrivateMember(channel, ctx.accountId)) {
                channels.push({
                    channelName: channel.name,
                    numMembers: channel.members.length,
                    channelType: PRIVATE_CHAT_CHANNEL_TYPE
                });
            }
        });

        ctx.reply<CMsgDOTARequestChatChannelListResponse>(
            Msg.GCRequestChatChannelListResponse,
            Proto.CMsgDOTARequestChatChannelListResponse,
            { channels }
        );
        return true;
    }

    chatMessage(ctx: RawMessageContext): boolean {
        const request = ctx.decode<CMsgDOTAChatMessage>(Proto.CMsgDOTAChatMessage);
        const privateChatChannelId = request.privateChatChannelId ?? 0;
        if (privateChatChannelId !== 0) {
            return this.privateChatMessage(ctx, request, privateChatChannelId);
        }

        const channelId = request.channelId ?? 0n;
        const privateChannel = privateChatsByRuntimeChannelId.get(channelId);
        if (privateChannel !== undefined) {
            return this.privateChatMessage(ctx, request, privateChannel.id);
        }

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

    privateChatInvite(ctx: RawMessageContext): boolean {
        const request = ctx.decode<CMsgClientToGCPrivateChatInvite>(Proto.CMsgClientToGCPrivateChatInvite);
        const targetAccountId = request.invitedAccountId ?? 0;
        const target = targetAccountId === 0 ? null : ctx.services.profiles.get(targetAccountId);
        if (target === null || target.steamId === 0n || !ctx.services.party.userExists(target.steamId)) {
            this.sendPrivateChatResponse(
                ctx,
                ctx.steamId,
                request.privateChatChannelName ?? "",
                CMsgGCToClientPrivateChatResponse_Result.FailureUnknownUser,
                ""
            );
            return true;
        }

        const channelName = privateChannelName(request.privateChatChannelName, ctx.accountId, targetAccountId);
        const channel = ensurePrivateChannel(channelName, {
            steamId: ctx.steamId,
            accountId: ctx.accountId,
            personaName: ctx.personaName
        });
        const wasMember = hasPrivateMember(channel, targetAccountId);
        if (!wasMember) {
            addPrivateMember(channel, {
                steamId: target.steamId,
                accountId: target.accountId,
                personaName: target.personaName
            });
        }

        this.sendPrivateChatResponse(
            ctx,
            ctx.steamId,
            channel.name,
            wasMember
                ? CMsgGCToClientPrivateChatResponse_Result.FailureAlreadyMember
                : CMsgGCToClientPrivateChatResponse_Result.Success,
            target.personaName
        );
        if (!wasMember) {
            this.sendPrivateChatResponse(
                ctx,
                target.steamId,
                channel.name,
                CMsgGCToClientPrivateChatResponse_Result.Success,
                ctx.personaName
            );
        }

        return true;
    }

    privateChatKick(ctx: RawMessageContext): boolean {
        const request = ctx.decode<CMsgClientToGCPrivateChatKick>(Proto.CMsgClientToGCPrivateChatKick);
        const channel = privateChats.get(request.privateChatChannelName ?? "") ?? null;
        const targetAccountId = request.kickAccountId ?? 0;
        if (channel === null || !hasPrivateMember(channel, ctx.accountId) || !isPrivateAdmin(channel, ctx.accountId)) {
            this.sendPrivateChatResponse(
                ctx,
                ctx.steamId,
                request.privateChatChannelName ?? "",
                CMsgGCToClientPrivateChatResponse_Result.FailureNoPermission,
                ""
            );
            return true;
        }

        const removed = removePrivateMember(channel, targetAccountId);
        this.sendPrivateChatResponse(
            ctx,
            ctx.steamId,
            channel.name,
            removed === null
                ? CMsgGCToClientPrivateChatResponse_Result.FailureNotAMember
                : CMsgGCToClientPrivateChatResponse_Result.Success,
            removed?.personaName ?? ""
        );
        if (removed !== null) {
            this.sendPrivateChatResponse(
                ctx,
                removed.steamId,
                channel.name,
                CMsgGCToClientPrivateChatResponse_Result.FailureNotAMember,
                ctx.personaName
            );
        }

        return true;
    }

    privateChatPromote(ctx: RawMessageContext): boolean {
        const request = ctx.decode<CMsgClientToGCPrivateChatPromote>(Proto.CMsgClientToGCPrivateChatPromote);
        return this.setPrivateAdmin(ctx, request.privateChatChannelName ?? "", request.promoteAccountId ?? 0, true);
    }

    privateChatDemote(ctx: RawMessageContext): boolean {
        const request = ctx.decode<CMsgClientToGCPrivateChatDemote>(Proto.CMsgClientToGCPrivateChatDemote);
        return this.setPrivateAdmin(ctx, request.privateChatChannelName ?? "", request.demoteAccountId ?? 0, false);
    }

    privateChatMessage(ctx: RawMessageContext, request: CMsgDOTAChatMessage, privateChatChannelId: number): boolean {
        const channel = privateChatsById.get(privateChatChannelId) ?? null;
        if (channel === null || !hasPrivateMember(channel, ctx.accountId)) {
            ctx.logger.info(
                "PrivateChat: dropped message from account " +
                    ctx.accountId +
                    " for unknown private channel " +
                    privateChatChannelId
            );
            return true;
        }

        const text = request.text ?? "";
        let message: CMsgDOTAChatMessage = {
            accountId: ctx.accountId,
            personaName: ctx.personaName,
            text,
            timestamp: ctx.clock.now(),
            privateChatChannelId,
            chatFlags: request.chatFlags,
            chatWheelMessage: request.chatWheelMessage
        };
        if (text.trim().startsWith("/roll")) {
            message = { ...message, diceRoll: request.diceRoll };
        } else if (text.trim().startsWith("/flip")) {
            message = { ...message, coinFlip: request.coinFlip };
        }

        const payload = ctx.encode(Proto.CMsgDOTAChatMessage, message);
        for (let i = 0; i < channel.members.length; i++) {
            const member = channel.members[i];
            if (member.accountId !== ctx.accountId) {
                ctx.services.lobby.queueMessage(member.steamId, Msg.GCChatMessage, payload, true);
            }
        }

        return true;
    }

    private setPrivateAdmin(ctx: RawMessageContext, channelName: string, targetAccountId: number, admin: boolean): boolean {
        const channel = privateChats.get(channelName) ?? null;
        if (channel === null || !hasPrivateMember(channel, ctx.accountId) || !isPrivateAdmin(channel, ctx.accountId)) {
            this.sendPrivateChatResponse(
                ctx,
                ctx.steamId,
                channelName,
                CMsgGCToClientPrivateChatResponse_Result.FailureNoPermission,
                ""
            );
            return true;
        }

        if (!hasPrivateMember(channel, targetAccountId)) {
            this.sendPrivateChatResponse(
                ctx,
                ctx.steamId,
                channel.name,
                CMsgGCToClientPrivateChatResponse_Result.FailureNotAMember,
                ""
            );
            return true;
        }

        if (admin && !isPrivateAdmin(channel, targetAccountId)) {
            channel.admins.push(targetAccountId);
        } else if (!admin && targetAccountId !== ctx.accountId) {
            channel.admins = channel.admins.filter((accountId) => accountId !== targetAccountId);
        }

        this.sendPrivateChatResponse(
            ctx,
            ctx.steamId,
            channel.name,
            CMsgGCToClientPrivateChatResponse_Result.Success,
            memberName(channel, targetAccountId)
        );
        return true;
    }

    private sendPrivateChatResponse(
        ctx: RawMessageContext,
        targetSteamId: bigint,
        channelName: string,
        result: number,
        username: string
    ): void {
        const message: CMsgGCToClientPrivateChatResponse = {
            privateChatChannelName: channelName,
            result,
            username
        };
        const payload = ctx.encode(Proto.CMsgGCToClientPrivateChatResponse, message);
        if (targetSteamId === ctx.steamId) {
            ctx.send(Msg.GCToClientPrivateChatResponse, Proto.CMsgGCToClientPrivateChatResponse, message);
            return;
        }

        ctx.services.lobby.queueMessage(targetSteamId, Msg.GCToClientPrivateChatResponse, payload, true);
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

function privateChannelName(channelName: string | undefined, a: number, b: number): string {
    const trimmed = (channelName ?? "").trim();
    if (trimmed.length > 0) {
        return trimmed;
    }

    const low = a < b ? a : b;
    const high = a < b ? b : a;
    return "private_" + low + "_" + high;
}

function ensurePrivateChannel(channelName: string, owner: PrivateChatMember): PrivateChatChannel {
    const existing = privateChats.get(channelName);
    if (existing !== undefined) {
        addPrivateMember(existing, owner);
        if (!isPrivateAdmin(existing, owner.accountId)) {
            existing.admins.push(owner.accountId);
        }

        return existing;
    }

    const channel: PrivateChatChannel = {
        id: nextPrivateChatId++,
        name: channelName,
        members: [],
        admins: [owner.accountId]
    };
    addPrivateMember(channel, owner);
    privateChats.set(channel.name, channel);
    privateChatsById.set(channel.id, channel);
    return channel;
}

function addPrivateMember(channel: PrivateChatChannel, member: PrivateChatMember): void {
    for (let i = 0; i < channel.members.length; i++) {
        if (channel.members[i].accountId === member.accountId) {
            channel.members[i] = member;
            return;
        }
    }

    channel.members.push(member);
}

function removePrivateMember(channel: PrivateChatChannel, accountId: number): PrivateChatMember | null {
    for (let i = 0; i < channel.members.length; i++) {
        if (channel.members[i].accountId === accountId) {
            const member = channel.members[i];
            channel.members.splice(i, 1);
            channel.admins = channel.admins.filter((adminAccountId) => adminAccountId !== accountId);
            return member;
        }
    }

    return null;
}

function hasPrivateMember(channel: PrivateChatChannel, accountId: number): boolean {
    for (let i = 0; i < channel.members.length; i++) {
        if (channel.members[i].accountId === accountId) {
            return true;
        }
    }

    return false;
}

function isPrivateAdmin(channel: PrivateChatChannel, accountId: number): boolean {
    return channel.admins.indexOf(accountId) >= 0;
}

function memberName(channel: PrivateChatChannel, accountId: number): string {
    for (let i = 0; i < channel.members.length; i++) {
        if (channel.members[i].accountId === accountId) {
            return channel.members[i].personaName;
        }
    }

    return "";
}

export function bindPrivateChatRuntimeChannel(
    channelName: string,
    runtimeChannelId: bigint,
    member: PrivateChatMember
): boolean {
    const channel = privateChats.get(channelName);
    if (channel === undefined) {
        return false;
    }

    addPrivateMember(channel, member);
    privateChatsByRuntimeChannelId.set(runtimeChannelId, channel);
    return true;
}
