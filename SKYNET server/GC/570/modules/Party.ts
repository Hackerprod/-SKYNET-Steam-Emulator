import {
    DotaPartyInvite,
    DotaPartyPingData,
    DotaPartyState,
    GcContextBase,
    HandlerContext,
    RawMessageContext,
    gc
} from "../framework/gc";
import {
    CMsgClientPingData,
    CMsgDOTASetGroupLeader,
    CMsgDOTAPartyMemberSetCoach,
    CMsgInvitationCreated,
    CMsgInviteToParty,
    CMsgKickFromParty,
    CMsgLeaveParty,
    CMsgGCToClientRequestMMInfo,
    CMsgPartyInviteResponse,
    CMsgPartyReadyCheckAcknowledge,
    CMsgPartyReadyCheckRequest,
    CMsgPartyReadyCheckResponse,
    CMsgReadyCheckStatus,
    CMsgReadyCheckStatus_ReadyMember,
    CSODOTAParty,
    CSODOTAPartyInvite,
    CSODOTAPartyInvite_PartyMember,
    CSODOTAPartyMember,
    EReadyCheckRequestResult,
    EReadyCheckStatus,
    Msg,
    Proto,
    Routes
} from "../generated/dota";
import { DOTA_SO_PARTY, DOTA_SO_PARTY_INVITE, SO_OWNER_INVITE, SO_OWNER_PARTY_GROUP } from "./shared/soIds";

const PARTY_OBJECT_TYPE_ID = DOTA_SO_PARTY;
const PARTY_INVITE_OBJECT_TYPE_ID = DOTA_SO_PARTY_INVITE;
const PARTY_OWNER_TYPE = SO_OWNER_PARTY_GROUP;
const PARTY_INVITE_OWNER_TYPE = SO_OWNER_INVITE;
const PARTY_MAX_PLAYERS = 5;
const PARTY_INVITE_LIFETIME_SECONDS = 600;
const READY_CHECK_DURATION_SECONDS = 60;
const READY_STATUS_UNKNOWN = EReadyCheckStatus.ReadyCheckStatusUnknown;

type PartyContext = GcContextBase;

export function registerParty(): Party {
    const party = new Party();
    party.register();
    return party;
}

export function emitCurrentParty(ctx: PartyContext): boolean {
    let emitted = false;
    const currentParty = ctx.services.party.getCurrent();
    if (currentParty !== null) {
        queuePartySubscribe(ctx, ctx.steamId, currentParty);
        queuePartyUpdate(ctx, currentParty);
        emitted = true;
    }

    const invites = ctx.services.party.getInvitesForTarget(ctx.steamId);
    for (let i = 0; i < invites.length; i++) {
        const invite = invites[i];
        const party = ctx.services.party.getById(invite.partyId);
        if (party === null || party.members.length >= PARTY_MAX_PLAYERS || hasMember(party, ctx.steamId)) {
            ctx.services.party.takeInvite(invite.partyId);
            continue;
        }

        queuePartyInvite(ctx, ctx.steamId, party, invite);
        emitted = true;
    }

    return emitted;
}

export class Party {
    register(): void {
        gc.on(Routes.InviteToParty, (ctx) => this.inviteToParty(ctx));
        gc.on(Routes.PartyReadyCheck, (ctx) => this.readyCheckRequest(ctx));
        gc.onMessage(Msg.GCPartyInviteResponse, (ctx) => this.inviteResponse(ctx));
        gc.onMessage(Msg.ClientToGCMMInfo, (ctx) => this.clientMmInfo(ctx));
        gc.onMessage(Msg.ClientToGCPingData, (ctx) => this.pingData(ctx));
        gc.onMessage(Msg.ClientToGCSetPartyLeader, (ctx) => this.setLeader(ctx));
        gc.onMessage(Msg.GCLeaveParty, (ctx) => this.leave(ctx));
        gc.onMessage(Msg.GCKickFromParty, (ctx) => this.kick(ctx));
        gc.onMessage(Msg.GCPartyMemberSetCoach, (ctx) => this.setCoach(ctx));
        gc.onMessage(Msg.PartyReadyCheckAcknowledge, (ctx) => this.readyCheckAcknowledge(ctx));
    }

    private inviteToParty(ctx: HandlerContext<CMsgInviteToParty, CMsgInvitationCreated>): boolean {
        const targetSteamId = ctx.request.steamId ?? 0n;
        if (targetSteamId === 0n || targetSteamId === ctx.steamId || !ctx.services.party.userExists(targetSteamId)) {
            ctx.reply({ groupId: 0n, steamId: targetSteamId, userOffline: true });
            return true;
        }

        const ping = normalizePing(ctx.request.pingData);
        const party = ctx.services.party.ensureCurrent(ping);
        if (party.members.length >= PARTY_MAX_PLAYERS || hasMember(party, targetSteamId)) {
            ctx.reply({ groupId: 0n, steamId: targetSteamId, userOffline: false });
            return true;
        }

        ctx.send<CMsgGCToClientRequestMMInfo>(
            Msg.GCToClientRequestMMInfo,
            Proto.CMsgGCToClientRequestMMInfo,
            {}
        );
        queuePartySubscribe(ctx, ctx.steamId, party);
        queuePartyUpdate(ctx, party);

        const created = ctx.services.party.createInvite(
            party.partyId,
            targetSteamId,
            ctx.request.teamId ?? 0,
            ctx.request.asCoach ?? false
        );
        if (created === null) {
            ctx.reply({ groupId: 0n, steamId: targetSteamId, userOffline: true });
            return true;
        }

        const replacedInvite = created.replaced;
        if (replacedInvite !== null) {
            queuePartyInviteDestroy(ctx, targetSteamId, replacedInvite);
        }

        queuePartyInvite(ctx, targetSteamId, party, created.invite);
        ctx.reply({
            groupId: party.partyId,
            steamId: targetSteamId,
            userOffline: !ctx.services.party.userOnline(targetSteamId)
        });
        return true;
    }

    private clientMmInfo(ctx: RawMessageContext): boolean {
        ctx.decode(Proto.CMsgClientToGCMMInfo);
        return true;
    }

    private inviteResponse(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgPartyInviteResponse) as CMsgPartyInviteResponse;
        const partyId = request.partyId ?? 0n;
        if (partyId === 0n) {
            return true;
        }

        const invite = ctx.services.party.takeInvite(partyId);
        if (invite === null) {
            return true;
        }

        queuePartyInviteDestroy(ctx, ctx.steamId, invite);
        if (!(request.accept ?? false)) {
            return true;
        }

        const invitedParty = ctx.services.party.getById(partyId);
        if (invitedParty === null || invitedParty.members.length >= PARTY_MAX_PLAYERS) {
            return true;
        }

        const current = ctx.services.party.getCurrent();
        if (current !== null && current.partyId !== partyId) {
            detachPartyMember(ctx, current, ctx.steamId);
        }

        const cancelled = ctx.services.party.deleteInvitesForTarget(ctx.steamId);
        for (let i = 0; i < cancelled.length; i++) {
            queuePartyInviteDestroy(ctx, ctx.steamId, cancelled[i]);
        }

        const party = ctx.services.party.addMember(partyId, normalizePing(request.pingData), invite.asCoach);
        if (party === null) {
            return true;
        }

        queuePartySubscribe(ctx, ctx.steamId, party);
        queuePartyUpdate(ctx, party);
        if (party.members.length >= PARTY_MAX_PLAYERS) {
            destroyPartyInvites(ctx, ctx.services.party.deleteInvitesForParty(party.partyId));
        }

        return true;
    }

    private pingData(ctx: RawMessageContext): boolean {
        const party = ctx.services.party.getCurrent();
        if (party === null) {
            return true;
        }

        const request = ctx.decode(Proto.CMsgClientPingData) as CMsgClientPingData;
        const updated = ctx.services.party.setMemberPing(party.partyId, ctx.steamId, normalizePing(request));
        if (updated !== null) {
            queuePartyUpdate(ctx, updated, ctx.steamId);
        }

        return true;
    }

    private setLeader(ctx: RawMessageContext): boolean {
        const party = ctx.services.party.getCurrent();
        if (party === null || party.leaderSteamId !== ctx.steamId) {
            return true;
        }

        const request = ctx.decode(Proto.CMsgDOTASetGroupLeader) as CMsgDOTASetGroupLeader;
        const newLeaderSteamId = request.newLeaderSteamid ?? 0n;
        if (newLeaderSteamId === 0n || !hasMember(party, newLeaderSteamId)) {
            return true;
        }

        const updated = ctx.services.party.setLeader(party.partyId, newLeaderSteamId);
        if (updated !== null) {
            queuePartyUpdate(ctx, updated);
        }

        return true;
    }

    private leave(ctx: RawMessageContext): boolean {
        ctx.decode(Proto.CMsgLeaveParty) as CMsgLeaveParty;
        const party = ctx.services.party.getCurrent();
        if (party !== null) {
            detachPartyMember(ctx, party, ctx.steamId);
        }

        return true;
    }

    private kick(ctx: RawMessageContext): boolean {
        const party = ctx.services.party.getCurrent();
        if (party === null || party.leaderSteamId !== ctx.steamId) {
            return true;
        }

        const request = ctx.decode(Proto.CMsgKickFromParty) as CMsgKickFromParty;
        const kickedSteamId = request.steamId ?? 0n;
        if (kickedSteamId === 0n || kickedSteamId === party.leaderSteamId || !hasMember(party, kickedSteamId)) {
            return true;
        }

        detachPartyMember(ctx, party, kickedSteamId);
        return true;
    }

    private setCoach(ctx: RawMessageContext): boolean {
        const party = ctx.services.party.getCurrent();
        if (party === null) {
            return true;
        }

        const request = ctx.decode(Proto.CMsgDOTAPartyMemberSetCoach) as CMsgDOTAPartyMemberSetCoach;
        const updated = ctx.services.party.setMemberCoach(party.partyId, ctx.steamId, request.wantsCoach ?? false);
        if (updated !== null) {
            queuePartyUpdate(ctx, updated);
        }

        return true;
    }

    private readyCheckRequest(ctx: HandlerContext<CMsgPartyReadyCheckRequest, CMsgPartyReadyCheckResponse>): boolean {
        const party = ctx.services.party.getCurrent();
        if (party === null) {
            ctx.reply({ result: EReadyCheckRequestResult.ReadyCheckRequestResultNotInParty });
            return true;
        }

        if (party.readyFinishTimestamp > ctx.clock.now()) {
            ctx.reply({ result: EReadyCheckRequestResult.ReadyCheckRequestResultAlreadyInProgress });
            return true;
        }

        const updated = ctx.services.party.startReadyCheck(party.partyId, READY_CHECK_DURATION_SECONDS);
        if (updated === null) {
            ctx.reply({ result: EReadyCheckRequestResult.ReadyCheckRequestResultUnknownError });
            return true;
        }

        queuePartyUpdate(ctx, updated);
        ctx.reply({ result: EReadyCheckRequestResult.ReadyCheckRequestResultSuccess });
        return true;
    }

    private readyCheckAcknowledge(ctx: RawMessageContext): boolean {
        const party = ctx.services.party.getCurrent();
        if (party === null || party.readyFinishTimestamp < ctx.clock.now()) {
            return true;
        }

        const request = ctx.decode(Proto.CMsgPartyReadyCheckAcknowledge) as CMsgPartyReadyCheckAcknowledge;
        const readyStatus = request.readyStatus ?? READY_STATUS_UNKNOWN;
        const updated = ctx.services.party.acknowledgeReadyCheck(party.partyId, readyStatus);
        if (updated !== null) {
            queuePartyUpdate(ctx, updated);
        }

        return true;
    }

    tick(): void {
        const ctx = gc.createRawContext(0);
        const now = ctx.clock.now();
        const cutoff = now > PARTY_INVITE_LIFETIME_SECONDS ? now - PARTY_INVITE_LIFETIME_SECONDS : 0;
        const expired = ctx.services.party.pruneInvitesCreatedBefore(cutoff);
        destroyPartyInvites(ctx, expired);
    }
}

function detachPartyMember(ctx: PartyContext, party: DotaPartyState, memberSteamId: bigint): void {
    const recipients = partyMemberSteamIds(party);
    if (party.members.length <= 2) {
        const invites = ctx.services.party.deleteInvitesForParty(party.partyId);
        ctx.services.party.deleteParty(party.partyId);
        for (let i = 0; i < recipients.length; i++) {
            queuePartyDestroy(ctx, recipients[i], party);
        }

        destroyPartyInvites(ctx, invites);
        return;
    }

    const updated = ctx.services.party.removeMember(party.partyId, memberSteamId);
    queuePartyDestroy(ctx, memberSteamId, party);
    if (updated !== null) {
        queuePartyUpdate(ctx, updated);
    }
}

function queuePartySubscribe(ctx: PartyContext, targetSteamId: bigint, party: DotaPartyState): void {
    queueMessage(ctx, targetSteamId, Msg.SOCacheSubscribed, Proto.CMsgSOCacheSubscribed, {
        objects: [
            {
                typeId: PARTY_OBJECT_TYPE_ID,
                objectData: [ctx.encode(Proto.CSODOTAParty, buildPartyObject(party))]
            }
        ],
        version: objectVersion(ctx),
        ownerSoid: partyOwner(party.partyId)
    });
}

function queuePartyUpdate(ctx: PartyContext, party: DotaPartyState, exceptSteamId = 0n): void {
    const payload = ctx.encode(Proto.CMsgSOMultipleObjects, {
        objectsModified: [
            {
                typeId: PARTY_OBJECT_TYPE_ID,
                objectData: ctx.encode(Proto.CSODOTAParty, buildPartyObject(party))
            }
        ],
        version: objectVersion(ctx),
        ownerSoid: partyOwner(party.partyId)
    });

    const members = partyMemberSteamIds(party);
    for (let i = 0; i < members.length; i++) {
        if (members[i] !== exceptSteamId) {
            ctx.services.party.queueMessage(members[i], Msg.SOCacheUpdated, payload, true);
        }
    }
}

function queuePartyDestroy(ctx: PartyContext, targetSteamId: bigint, party: DotaPartyState): void {
    queueMessage(ctx, targetSteamId, Msg.SOCacheUnsubscribed, Proto.CMsgSOCacheUnsubscribed, {
        ownerSoid: partyOwner(party.partyId)
    });

    queueMessage(ctx, targetSteamId, Msg.SOSingleObjectDestroyed, Proto.CMsgSOSingleObject, {
        typeId: PARTY_OBJECT_TYPE_ID,
        version: objectVersion(ctx),
        ownerSoid: partyOwner(party.partyId)
    });
}

function queuePartyInvite(
    ctx: PartyContext,
    targetSteamId: bigint,
    party: DotaPartyState,
    invite: DotaPartyInvite
): void {
    queueMessage(ctx, targetSteamId, Msg.SOCacheSubscribed, Proto.CMsgSOCacheSubscribed, {
        objects: [
            {
                typeId: PARTY_INVITE_OBJECT_TYPE_ID,
                objectData: [ctx.encode(Proto.CSODOTAPartyInvite, buildPartyInviteObject(party, invite))]
            }
        ],
        version: objectVersion(ctx),
        ownerSoid: partyInviteOwner(invite.inviteGid)
    });
}

function queuePartyInviteDestroy(ctx: PartyContext, targetSteamId: bigint, invite: DotaPartyInvite): void {
    queueMessage(ctx, targetSteamId, Msg.SOCacheUnsubscribed, Proto.CMsgSOCacheUnsubscribed, {
        ownerSoid: partyInviteOwner(invite.inviteGid)
    });
}

function destroyPartyInvites(ctx: PartyContext, invites: DotaPartyInvite[]): void {
    for (let i = 0; i < invites.length; i++) {
        queuePartyInviteDestroy(ctx, invites[i].targetSteamId, invites[i]);
    }
}

function buildPartyObject(party: DotaPartyState): CSODOTAParty {
    const members: CSODOTAPartyMember[] = [];
    for (let i = 0; i < party.members.length; i++) {
        const member = party.members[i];
        members.push({
            isCoach: member.isCoach,
            regionPingCodes: member.regionCodes,
            regionPingTimes: member.regionPings,
            regionPingFailedBitmask: member.regionPingFailedBitmask,
            isPlusSubscriber: member.isPlusSubscriber
        });
    }

    return {
        partyId: party.partyId,
        leaderId: party.leaderSteamId,
        memberIds: partyMemberSteamIds(party),
        state: party.state,
        gameModes: 0,
        members,
        readyCheck: buildReadyCheck(party)
    };
}

function buildPartyInviteObject(party: DotaPartyState, invite: DotaPartyInvite): CSODOTAPartyInvite {
    const members: CSODOTAPartyInvite_PartyMember[] = [];
    for (let i = 0; i < party.members.length; i++) {
        const member = party.members[i];
        members.push({
            name: member.personaName,
            steamId: member.steamId,
            isCoach: member.isCoach
        });
    }

    return {
        groupId: invite.partyId,
        senderId: invite.senderSteamId,
        senderName: invite.senderName,
        members,
        teamId: invite.teamId,
        lowPriorityStatus: invite.lowPriorityStatus,
        asCoach: invite.asCoach,
        inviteGid: invite.inviteGid
    };
}

function buildReadyCheck(party: DotaPartyState): CMsgReadyCheckStatus | undefined {
    if (party.readyStartTimestamp === 0 && party.readyFinishTimestamp === 0) {
        return undefined;
    }

    const readyMembers: CMsgReadyCheckStatus_ReadyMember[] = [];
    for (let i = 0; i < party.members.length; i++) {
        const member = party.members[i];
        if (member.readyStatus !== READY_STATUS_UNKNOWN) {
            readyMembers.push({
                accountId: member.accountId,
                readyStatus: member.readyStatus
            });
        }
    }

    return {
        startTimestamp: party.readyStartTimestamp,
        finishTimestamp: party.readyFinishTimestamp,
        initiatorAccountId: party.readyInitiatorAccountId,
        readyMembers
    };
}

function normalizePing(ping: CMsgClientPingData | undefined): DotaPartyPingData {
    return {
        regionCodes: copyNumbers(ping?.regionCodes),
        regionPings: copyNumbers(ping?.regionPings),
        regionPingFailedBitmask: ping?.regionPingFailedBitmask ?? 0
    };
}

function partyMemberSteamIds(party: DotaPartyState): bigint[] {
    const ids: bigint[] = [];
    for (let i = 0; i < party.members.length; i++) {
        ids.push(party.members[i].steamId);
    }

    return ids;
}

function hasMember(party: DotaPartyState, steamId: bigint): boolean {
    for (let i = 0; i < party.members.length; i++) {
        if (party.members[i].steamId === steamId) {
            return true;
        }
    }

    return false;
}

function copyNumbers(values: number[] | undefined): number[] {
    const result: number[] = [];
    const source = values ?? [];
    for (let i = 0; i < source.length; i++) {
        result.push(source[i]);
    }

    return result;
}

function queueMessage<TMessage>(
    ctx: PartyContext,
    targetSteamId: bigint,
    messageType: number,
    proto: { readonly name: string },
    message: TMessage
): void {
    ctx.services.party.queueMessage(targetSteamId, messageType, ctx.encode(proto, message), true);
}

function partyOwner(partyId: bigint): { type: number; id: bigint } {
    return {
        type: PARTY_OWNER_TYPE,
        id: partyId
    };
}

function partyInviteOwner(inviteGid: bigint): { type: number; id: bigint } {
    return {
        type: PARTY_INVITE_OWNER_TYPE,
        id: inviteGid
    };
}

function objectVersion(ctx: PartyContext): bigint {
    return (BigInt(ctx.clock.now()) << 20n) | (ctx.steamId & 0xfffffn);
}
