import {
    DotaGuild,
    DotaGuildEventData,
    DotaGuildInfo,
    DotaGuildInvite,
    DotaGuildMember,
    DotaGuildPersona,
    DotaGuildRole,
    DotaReporterUpdate,
    HandlerContext,
    RawMessageContext,
    gc
} from "../framework/gc";
import {
    CMsgAccountGuildEventData,
    CMsgAccountGuildInvite,
    CMsgAccountGuildsPersonaInfo,
    CMsgClientToGCAcknowledgeReporterUpdates,
    CMsgClientToGCAcceptInviteToGuild,
    CMsgClientToGCAcceptInviteToGuildResponse,
    CMsgClientToGCAcceptInviteToGuildResponse_EResponse,
    CMsgClientToGCCancelInviteToGuild,
    CMsgClientToGCCancelInviteToGuildResponse,
    CMsgClientToGCCancelInviteToGuildResponse_EResponse,
    CMsgClientToGCDeclineInviteToGuild,
    CMsgClientToGCDeclineInviteToGuildResponse,
    CMsgClientToGCDeclineInviteToGuildResponse_EResponse,
    CMsgClientToGCInviteToGuild,
    CMsgClientToGCInviteToGuildResponse,
    CMsgClientToGCInviteToGuildResponse_EResponse,
    CMsgClientToGCLeaveGuild,
    CMsgClientToGCLeaveGuildResponse,
    CMsgClientToGCLeaveGuildResponse_EResponse,
    CMsgClientToGCRequestAccountGuildEventData,
    CMsgClientToGCRequestAccountGuildEventDataResponse,
    CMsgClientToGCRequestAccountGuildEventDataResponse_EResponse,
    CMsgClientToGCRequestAccountGuildPersonaInfo,
    CMsgClientToGCRequestAccountGuildPersonaInfoBatch,
    CMsgClientToGCRequestAccountGuildPersonaInfoBatchResponse,
    CMsgClientToGCRequestAccountGuildPersonaInfoBatchResponse_EResponse,
    CMsgClientToGCRequestAccountGuildPersonaInfoResponse,
    CMsgClientToGCRequestAccountGuildPersonaInfoResponse_EResponse,
    CMsgClientToGCRequestActiveGuildChallenge,
    CMsgClientToGCRequestActiveGuildChallengeResponse,
    CMsgClientToGCRequestActiveGuildChallengeResponse_EResponse,
    CMsgClientToGCRequestGuildData,
    CMsgClientToGCRequestGuildDataResponse,
    CMsgClientToGCRequestGuildDataResponse_EResponse,
    CMsgClientToGCRequestGuildMembership,
    CMsgClientToGCRequestGuildMembershipResponse,
    CMsgClientToGCRequestGuildMembershipResponse_EResponse,
    CMsgClientToGCRequestReporterUpdates,
    CMsgClientToGCRequestReporterUpdatesResponse,
    CMsgClientToGCRequestReporterUpdatesResponse_EResponse,
    CMsgClientToGCRequestReporterUpdatesResponse_ReporterUpdate,
    CMsgGCToClientGuildDataUpdated,
    CMsgGCToClientGuildMembershipUpdated,
    CMsgGuildData,
    CMsgGuildInfo,
    CMsgGuildInvite,
    CMsgGuildMember,
    CMsgGuildPersonaInfo,
    CMsgGuildRole,
    CMsgServerToGCGetGuildContracts,
    CMsgServerToGCGetGuildContractsResponse,
    CMsgServerToGCGetGuildContractsResponse_Player,
    Msg,
    Proto,
    Routes
} from "../generated/dota";

const GUILD_DATA_SUCCESS = CMsgClientToGCRequestGuildDataResponse_EResponse.Success;
const GUILD_DATA_INVALID = CMsgClientToGCRequestGuildDataResponse_EResponse.InvalidGuild;
const GUILD_MEMBERSHIP_SUCCESS = CMsgClientToGCRequestGuildMembershipResponse_EResponse.Success;
const PERSONA_SUCCESS = CMsgClientToGCRequestAccountGuildPersonaInfoResponse_EResponse.Success;
const PERSONA_INVALID_ACCOUNT = CMsgClientToGCRequestAccountGuildPersonaInfoResponse_EResponse.InvalidAccount;
const PERSONA_BATCH_SUCCESS = CMsgClientToGCRequestAccountGuildPersonaInfoBatchResponse_EResponse.Success;
const REPORTER_UPDATES_SUCCESS = CMsgClientToGCRequestReporterUpdatesResponse_EResponse.Success;
const EVENT_DATA_SUCCESS = CMsgClientToGCRequestAccountGuildEventDataResponse_EResponse.Success;
const EVENT_DATA_INVALID_GUILD = CMsgClientToGCRequestAccountGuildEventDataResponse_EResponse.InvalidGuild;
const EVENT_DATA_NOT_MEMBER = CMsgClientToGCRequestAccountGuildEventDataResponse_EResponse.NotMember;
const ACTIVE_CHALLENGE_SUCCESS = CMsgClientToGCRequestActiveGuildChallengeResponse_EResponse.Success;
const ACTIVE_CHALLENGE_INVALID_GUILD = CMsgClientToGCRequestActiveGuildChallengeResponse_EResponse.InvalidGuild;
const GUILD_MUTATION_SUCCESS = 1;
const GUILD_MUTATION_INVALID_GUILD = 5;
const GUILD_MUTATION_NO_INVITE = 6;
const GUILD_MUTATION_GUILD_FULL = 7;
const GUILD_MUTATION_REQUESTER_NOT_MEMBER = 8;
const GUILD_MUTATION_ALREADY_MEMBER = 9;
const GUILD_MUTATION_ALREADY_INVITED = 10;
const GUILD_MUTATION_NO_PERMISSIONS = 11;
const GUILD_MUTATION_NOT_MEMBER = 12;

export function registerGuilds(): void {
    const guilds = new Guilds();
    guilds.register();
}

export class Guilds {
    register(): void {
        gc.on(Routes.RequestGuildData, (ctx) => this.requestGuildData(ctx));
        gc.on(Routes.RequestGuildMembership, (ctx) => this.requestGuildMembership(ctx));
        gc.on(Routes.RequestReporterUpdates, (ctx) => this.requestReporterUpdates(ctx));
        gc.on(Routes.RequestAccountGuildPersonaInfo, (ctx) => this.requestAccountGuildPersonaInfo(ctx));
        gc.on(Routes.RequestAccountGuildPersonaInfoBatch, (ctx) => this.requestAccountGuildPersonaInfoBatch(ctx));
        gc.on(Routes.RequestAccountGuildEventData, (ctx) => this.requestAccountGuildEventData(ctx));
        gc.onMessage(Msg.ClientToGCRequestActiveGuildChallenge, (ctx) => this.requestActiveGuildChallenge(ctx));
        gc.onMessage(Msg.ClientToGCLeaveGuild, (ctx) => this.leaveGuild(ctx));
        gc.onMessage(Msg.ClientToGCInviteToGuild, (ctx) => this.inviteToGuild(ctx));
        gc.onMessage(Msg.ClientToGCDeclineInviteToGuild, (ctx) => this.declineInviteToGuild(ctx));
        gc.onMessage(Msg.ClientToGCCancelInviteToGuild, (ctx) => this.cancelInviteToGuild(ctx));
        gc.onMessage(Msg.ClientToGCAcceptInviteToGuild, (ctx) => this.acceptInviteToGuild(ctx));
        gc.onMessage(Msg.ServerToGCGetGuildContracts, (ctx) => this.getGuildContracts(ctx));
        gc.onMessage(Msg.ClientToGCAcknowledgeReporterUpdates, (ctx) => this.acknowledgeReporterUpdates(ctx));
    }

    private requestGuildData(
        ctx: HandlerContext<CMsgClientToGCRequestGuildData, CMsgClientToGCRequestGuildDataResponse>
    ): boolean {
        const requestedGuildId = ctx.request.guildId ?? 0;
        const guildId = requestedGuildId === 0 ? ctx.services.guilds.ensureCurrent().guildId : requestedGuildId;

        const guild = ctx.services.guilds.getGuild(guildId);
        if (guild === null) {
            ctx.reply({ result: GUILD_DATA_INVALID });
            return true;
        }

        ctx.reply({
            result: GUILD_DATA_SUCCESS,
            guildData: mapGuild(guild)
        });
        return true;
    }

    private requestGuildMembership(
        ctx: HandlerContext<CMsgClientToGCRequestGuildMembership, CMsgClientToGCRequestGuildMembershipResponse>
    ): boolean {
        ctx.services.guilds.ensureCurrent();
        const membership = ctx.services.guilds.getMembership(ctx.accountId);
        ctx.reply({
            result: GUILD_MEMBERSHIP_SUCCESS,
            guildMemberships: {
                guildIds: membership.guildIds,
                guildInvites: membership.invites.map(mapAccountInvite)
            }
        });
        return true;
    }

    private requestReporterUpdates(
        ctx: HandlerContext<CMsgClientToGCRequestReporterUpdates, CMsgClientToGCRequestReporterUpdatesResponse>
    ): boolean {
        const updates = ctx.services.guilds.getReporterUpdates();
        ctx.reply({
            enumResult: REPORTER_UPDATES_SUCCESS,
            updates: updates.updates.map(mapReporterUpdate),
            numReported: updates.numReported,
            numNoActionTaken: updates.numNoActionTaken
        });
        return true;
    }

    private requestAccountGuildPersonaInfo(
        ctx: HandlerContext<
            CMsgClientToGCRequestAccountGuildPersonaInfo,
            CMsgClientToGCRequestAccountGuildPersonaInfoResponse
        >
    ): boolean {
        const accountId = ctx.request.accountId ?? ctx.accountId;
        if (accountId === ctx.accountId) {
            ctx.services.guilds.ensureCurrent();
        }

        const personaInfo = ctx.services.guilds.getPersonaInfo(accountId);
        if (personaInfo.length === 0) {
            ctx.reply({
                result: PERSONA_INVALID_ACCOUNT,
                personaInfo: { guildPersonaInfos: [] }
            });
            return true;
        }

        ctx.reply({
            result: PERSONA_SUCCESS,
            personaInfo: mapPersonaInfos(personaInfo)
        });
        return true;
    }

    private requestAccountGuildPersonaInfoBatch(
        ctx: HandlerContext<
            CMsgClientToGCRequestAccountGuildPersonaInfoBatch,
            CMsgClientToGCRequestAccountGuildPersonaInfoBatchResponse
        >
    ): boolean {
        const accountIds = ctx.request.accountIds ?? [];
        const personaInfos: CMsgAccountGuildsPersonaInfo[] = [];
        for (let i = 0; i < accountIds.length; i++) {
            const accountId = accountIds[i];
            if (accountId === ctx.accountId) {
                ctx.services.guilds.ensureCurrent();
            }

            personaInfos.push(mapPersonaInfos(ctx.services.guilds.getPersonaInfo(accountId)));
        }

        ctx.reply({
            result: PERSONA_BATCH_SUCCESS,
            personaInfos
        });
        return true;
    }

    private requestAccountGuildEventData(
        ctx: HandlerContext<
            CMsgClientToGCRequestAccountGuildEventData,
            CMsgClientToGCRequestAccountGuildEventDataResponse
        >
    ): boolean {
        const guildId = ctx.request.guildId ?? 0;
        const guild = guildId === 0 ? null : ctx.services.guilds.getGuild(guildId);
        if (guild === null) {
            ctx.reply({ result: EVENT_DATA_INVALID_GUILD, eventId: ctx.request.eventId ?? 0 });
            return true;
        }

        const eventData = ctx.services.guilds.getEventData(guildId, ctx.request.eventId ?? 0);
        if (!eventData.isMember) {
            ctx.reply({ result: EVENT_DATA_NOT_MEMBER, eventId: eventData.eventId });
            return true;
        }

        ctx.reply({
            result: EVENT_DATA_SUCCESS,
            eventId: eventData.eventId,
            eventData: mapEventData(eventData)
        });
        return true;
    }

    private requestActiveGuildChallenge(ctx: RawMessageContext): boolean {
        const request = ctx.decode<CMsgClientToGCRequestActiveGuildChallenge>(
            Proto.CMsgClientToGCRequestActiveGuildChallenge
        );
        const guildId = request.guildId ?? ctx.services.guilds.ensureCurrent().guildId;
        const guild = ctx.services.guilds.getGuild(guildId);
        if (guild === null) {
            ctx.reply<CMsgClientToGCRequestActiveGuildChallengeResponse>(
                Msg.ClientToGCRequestActiveGuildChallengeResponse,
                Proto.CMsgClientToGCRequestActiveGuildChallengeResponse,
                { result: ACTIVE_CHALLENGE_INVALID_GUILD }
            );
            return true;
        }

        ctx.reply<CMsgClientToGCRequestActiveGuildChallengeResponse>(
            Msg.ClientToGCRequestActiveGuildChallengeResponse,
            Proto.CMsgClientToGCRequestActiveGuildChallengeResponse,
            { result: ACTIVE_CHALLENGE_SUCCESS }
        );
        return true;
    }

    private leaveGuild(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgClientToGCLeaveGuild) as CMsgClientToGCLeaveGuild;
        const guildId = request.guildId ?? 0;
        const result = mapLeaveResult(ctx.services.guilds.leave(guildId));
        ctx.reply<CMsgClientToGCLeaveGuildResponse>(
            Msg.ClientToGCLeaveGuildResponse,
            Proto.CMsgClientToGCLeaveGuildResponse,
            { result }
        );
        if (result === CMsgClientToGCLeaveGuildResponse_EResponse.Success) {
            sendMembershipUpdate(ctx, ctx.accountId);
            sendGuildDataUpdate(ctx, guildId);
        }
        return true;
    }

    private inviteToGuild(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgClientToGCInviteToGuild) as CMsgClientToGCInviteToGuild;
        const guildId = request.guildId ?? 0;
        const targetAccountId = request.targetAccountId ?? 0;
        const result = mapInviteResult(ctx.services.guilds.invite(guildId, targetAccountId));
        ctx.reply<CMsgClientToGCInviteToGuildResponse>(
            Msg.ClientToGCInviteToGuildResponse,
            Proto.CMsgClientToGCInviteToGuildResponse,
            { result }
        );
        if (result === CMsgClientToGCInviteToGuildResponse_EResponse.Success) {
            sendGuildDataUpdate(ctx, guildId);
            sendMembershipUpdate(ctx, targetAccountId);
        }
        return true;
    }

    private declineInviteToGuild(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgClientToGCDeclineInviteToGuild) as CMsgClientToGCDeclineInviteToGuild;
        const guildId = request.guildId ?? 0;
        const result = mapDeclineResult(ctx.services.guilds.declineInvite(guildId));
        ctx.reply<CMsgClientToGCDeclineInviteToGuildResponse>(
            Msg.ClientToGCDeclineInviteToGuildResponse,
            Proto.CMsgClientToGCDeclineInviteToGuildResponse,
            { result }
        );
        if (result === CMsgClientToGCDeclineInviteToGuildResponse_EResponse.Success) {
            sendGuildDataUpdate(ctx, guildId);
            sendMembershipUpdate(ctx, ctx.accountId);
        }
        return true;
    }

    private cancelInviteToGuild(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgClientToGCCancelInviteToGuild) as CMsgClientToGCCancelInviteToGuild;
        const guildId = request.guildId ?? 0;
        const targetAccountId = request.targetAccountId ?? 0;
        const result = mapCancelResult(ctx.services.guilds.cancelInvite(guildId, targetAccountId));
        ctx.reply<CMsgClientToGCCancelInviteToGuildResponse>(
            Msg.ClientToGCCancelInviteToGuildResponse,
            Proto.CMsgClientToGCCancelInviteToGuildResponse,
            { result }
        );
        if (result === CMsgClientToGCCancelInviteToGuildResponse_EResponse.Success) {
            sendGuildDataUpdate(ctx, guildId);
            sendMembershipUpdate(ctx, targetAccountId);
        }
        return true;
    }

    private acceptInviteToGuild(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgClientToGCAcceptInviteToGuild) as CMsgClientToGCAcceptInviteToGuild;
        const guildId = request.guildId ?? 0;
        const result = mapAcceptResult(ctx.services.guilds.acceptInvite(guildId));
        ctx.reply<CMsgClientToGCAcceptInviteToGuildResponse>(
            Msg.ClientToGCAcceptInviteToGuildResponse,
            Proto.CMsgClientToGCAcceptInviteToGuildResponse,
            { result }
        );
        if (result === CMsgClientToGCAcceptInviteToGuildResponse_EResponse.Success) {
            sendGuildDataUpdate(ctx, guildId);
            sendMembershipUpdate(ctx, ctx.accountId);
        }
        return true;
    }

    private getGuildContracts(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgServerToGCGetGuildContracts) as CMsgServerToGCGetGuildContracts;
        const playerContracts: CMsgServerToGCGetGuildContractsResponse_Player[] = [];
        const accountIds = request.accountIds ?? [];
        // The dedicated server asks contracts by account. We answer from persisted guild membership
        // and keep the contract list empty until event contract state is implemented.
        for (let i = 0; i < accountIds.length; i++) {
            const accountId = accountIds[i];
            const membership = ctx.services.guilds.getMembership(accountId);
            if (membership.guildIds.length > 0) {
                playerContracts.push({
                    accountId,
                    guildId: membership.guildIds[0],
                    eventId: 0,
                    contracts: []
                });
            }
        }

        ctx.reply<CMsgServerToGCGetGuildContractsResponse>(
            Msg.ServerToGCGetGuildContractsResponse,
            Proto.CMsgServerToGCGetGuildContractsResponse,
            { playerContracts }
        );
        return true;
    }

    private acknowledgeReporterUpdates(ctx: RawMessageContext): boolean {
        const request = ctx.decode(
            Proto.CMsgClientToGCAcknowledgeReporterUpdates
        ) as CMsgClientToGCAcknowledgeReporterUpdates;
        ctx.services.guilds.acknowledgeReporterUpdates(request.matchIds ?? []);
        return true;
    }
}

function mapGuild(guild: DotaGuild): CMsgGuildData {
    return {
        guildId: guild.guildId,
        guildInfo: mapGuildInfo(guild.info),
        guildRoles: guild.roles.map(mapGuildRole),
        guildMembers: guild.members.map(mapGuildMember),
        guildInvites: guild.invites.map(mapGuildInvite)
    };
}

function mapGuildInfo(info: DotaGuildInfo): CMsgGuildInfo {
    return {
        guildName: info.guildName,
        guildTag: info.guildTag,
        createdTimestamp: info.createdTimestamp,
        guildLanguage: info.guildLanguage,
        guildFlags: info.guildFlags,
        guildLogo: info.guildLogo,
        guildRegion: info.guildRegion,
        guildChatGroupId: info.guildChatGroupId,
        guildDescription: info.guildDescription,
        defaultChatChannelId: info.defaultChatChannelId,
        guildPrimaryColor: info.guildPrimaryColor,
        guildSecondaryColor: info.guildSecondaryColor,
        guildPattern: info.guildPattern,
        guildRefreshTimeOffset: info.guildRefreshTimeOffset,
        guildRequiredRankTier: info.guildRequiredRankTier,
        guildMotdTimestamp: info.guildMotdTimestamp,
        guildMotd: info.guildMotd
    };
}

function mapGuildRole(role: DotaGuildRole): CMsgGuildRole {
    return {
        roleId: role.roleId,
        roleName: role.roleName,
        roleFlags: role.roleFlags,
        roleOrder: role.roleOrder
    };
}

function mapGuildMember(member: DotaGuildMember): CMsgGuildMember {
    return {
        memberAccountId: member.accountId,
        memberRoleId: member.roleId,
        memberJoinedTimestamp: member.joinedTimestamp,
        memberLastActiveTimestamp: member.lastActiveTimestamp
    };
}

function mapGuildInvite(invite: DotaGuildInvite): CMsgGuildInvite {
    return {
        requesterAccountId: invite.requesterAccountId,
        targetAccountId: invite.targetAccountId,
        timestampSent: invite.timestampSent
    };
}

function mapAccountInvite(invite: DotaGuildInvite): CMsgAccountGuildInvite {
    return {
        guildId: invite.guildId,
        requesterAccountId: invite.requesterAccountId,
        timestampSent: invite.timestampSent
    };
}

function sendGuildDataUpdate(ctx: RawMessageContext, guildId: number): void {
    const guild = ctx.services.guilds.getGuild(guildId);
    if (guild === null) {
        return;
    }

    ctx.send<CMsgGCToClientGuildDataUpdated>(Msg.GCToClientGuildDataUpdated, Proto.CMsgGCToClientGuildDataUpdated, {
        guildData: mapGuild(guild),
        updateFlags: 15
    });
}

function sendMembershipUpdate(ctx: RawMessageContext, accountId: number): void {
    const membership = ctx.services.guilds.getMembership(accountId);
    const message: CMsgGCToClientGuildMembershipUpdated = {
        guildMemberships: {
            guildIds: membership.guildIds,
            guildInvites: membership.invites.map(mapAccountInvite)
        }
    };
    const steamId = steamIdFromAccountId(accountId);
    if (steamId === ctx.steamId) {
        ctx.send<CMsgGCToClientGuildMembershipUpdated>(
            Msg.GCToClientGuildMembershipUpdated,
            Proto.CMsgGCToClientGuildMembershipUpdated,
            message
        );
        return;
    }

    ctx.services.lobby.queueMessage(
        steamId,
        Msg.GCToClientGuildMembershipUpdated,
        ctx.encode(Proto.CMsgGCToClientGuildMembershipUpdated, message),
        true
    );
}

function mapPersonaInfos(personaInfos: DotaGuildPersona[]): CMsgAccountGuildsPersonaInfo {
    return {
        guildPersonaInfos: personaInfos.map(mapPersonaInfo)
    };
}

function mapPersonaInfo(info: DotaGuildPersona): CMsgGuildPersonaInfo {
    return {
        guildId: info.guildId,
        guildTag: info.guildTag,
        guildFlags: info.guildFlags
    };
}

function mapReporterUpdate(update: DotaReporterUpdate): CMsgClientToGCRequestReporterUpdatesResponse_ReporterUpdate {
    return {
        matchId: update.matchId,
        heroId: update.heroId,
        reportReason: update.reportReason,
        timestamp: update.timestamp
    };
}

function mapEventData(eventData: DotaGuildEventData): CMsgAccountGuildEventData {
    return {
        guildPoints: eventData.guildPoints,
        contractsRefreshedTimestamp: eventData.contractsRefreshedTimestamp,
        contractSlots: [],
        completedChallengeCount: eventData.completedChallengeCount,
        challengesRefreshTimestamp: eventData.challengesRefreshTimestamp,
        guildWeeklyPercentile: eventData.guildWeeklyPercentile,
        guildWeeklyLastTimestamp: eventData.guildWeeklyLastTimestamp,
        lastWeeklyClaimTime: eventData.lastWeeklyClaimTime,
        guildCurrentPercentile: eventData.guildCurrentPercentile
    };
}

function mapLeaveResult(result: number): number {
    switch (result) {
        case GUILD_MUTATION_SUCCESS:
            return CMsgClientToGCLeaveGuildResponse_EResponse.Success;
        case GUILD_MUTATION_INVALID_GUILD:
            return CMsgClientToGCLeaveGuildResponse_EResponse.InvalidGuild;
        case GUILD_MUTATION_NOT_MEMBER:
            return CMsgClientToGCLeaveGuildResponse_EResponse.NotMember;
        default:
            return CMsgClientToGCLeaveGuildResponse_EResponse.InternalError;
    }
}

function mapInviteResult(result: number): number {
    switch (result) {
        case GUILD_MUTATION_SUCCESS:
            return CMsgClientToGCInviteToGuildResponse_EResponse.Success;
        case GUILD_MUTATION_INVALID_GUILD:
            return CMsgClientToGCInviteToGuildResponse_EResponse.InvalidGuild;
        case GUILD_MUTATION_GUILD_FULL:
            return CMsgClientToGCInviteToGuildResponse_EResponse.GuildFull;
        case GUILD_MUTATION_REQUESTER_NOT_MEMBER:
            return CMsgClientToGCInviteToGuildResponse_EResponse.RequesterNotMember;
        case GUILD_MUTATION_ALREADY_MEMBER:
            return CMsgClientToGCInviteToGuildResponse_EResponse.AlreadyAMember;
        case GUILD_MUTATION_ALREADY_INVITED:
            return CMsgClientToGCInviteToGuildResponse_EResponse.AlreadyInvited;
        case GUILD_MUTATION_NO_PERMISSIONS:
            return CMsgClientToGCInviteToGuildResponse_EResponse.NoInvitePermissions;
        default:
            return CMsgClientToGCInviteToGuildResponse_EResponse.InternalError;
    }
}

function mapDeclineResult(result: number): number {
    switch (result) {
        case GUILD_MUTATION_SUCCESS:
            return CMsgClientToGCDeclineInviteToGuildResponse_EResponse.Success;
        case GUILD_MUTATION_INVALID_GUILD:
            return CMsgClientToGCDeclineInviteToGuildResponse_EResponse.InvalidGuild;
        case GUILD_MUTATION_NO_INVITE:
            return CMsgClientToGCDeclineInviteToGuildResponse_EResponse.NoInviteFound;
        default:
            return CMsgClientToGCDeclineInviteToGuildResponse_EResponse.InternalError;
    }
}

function mapCancelResult(result: number): number {
    switch (result) {
        case GUILD_MUTATION_SUCCESS:
            return CMsgClientToGCCancelInviteToGuildResponse_EResponse.Success;
        case GUILD_MUTATION_INVALID_GUILD:
            return CMsgClientToGCCancelInviteToGuildResponse_EResponse.InvalidGuild;
        case GUILD_MUTATION_NO_INVITE:
            return CMsgClientToGCCancelInviteToGuildResponse_EResponse.NoInviteFound;
        case GUILD_MUTATION_NO_PERMISSIONS:
            return CMsgClientToGCCancelInviteToGuildResponse_EResponse.NoPermissions;
        default:
            return CMsgClientToGCCancelInviteToGuildResponse_EResponse.InternalError;
    }
}

function mapAcceptResult(result: number): number {
    switch (result) {
        case GUILD_MUTATION_SUCCESS:
            return CMsgClientToGCAcceptInviteToGuildResponse_EResponse.Success;
        case GUILD_MUTATION_INVALID_GUILD:
            return CMsgClientToGCAcceptInviteToGuildResponse_EResponse.InvalidGuild;
        case GUILD_MUTATION_NO_INVITE:
            return CMsgClientToGCAcceptInviteToGuildResponse_EResponse.NoInviteFound;
        case GUILD_MUTATION_GUILD_FULL:
            return CMsgClientToGCAcceptInviteToGuildResponse_EResponse.GuildFull;
        case GUILD_MUTATION_ALREADY_MEMBER:
            return CMsgClientToGCAcceptInviteToGuildResponse_EResponse.AlreadyInGuild;
        default:
            return CMsgClientToGCAcceptInviteToGuildResponse_EResponse.InternalError;
    }
}

function steamIdFromAccountId(accountId: number): bigint {
    return 76561197960265728n + BigInt(accountId >>> 0);
}
