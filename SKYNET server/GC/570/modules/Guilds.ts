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
    CMsgClientToGCRequestAccountGuildEventData,
    CMsgClientToGCRequestAccountGuildEventDataResponse,
    CMsgClientToGCRequestAccountGuildEventDataResponse_EResponse,
    CMsgClientToGCRequestAccountGuildPersonaInfo,
    CMsgClientToGCRequestAccountGuildPersonaInfoBatch,
    CMsgClientToGCRequestAccountGuildPersonaInfoBatchResponse,
    CMsgClientToGCRequestAccountGuildPersonaInfoBatchResponse_EResponse,
    CMsgClientToGCRequestAccountGuildPersonaInfoResponse,
    CMsgClientToGCRequestAccountGuildPersonaInfoResponse_EResponse,
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
    CMsgGuildData,
    CMsgGuildInfo,
    CMsgGuildInvite,
    CMsgGuildMember,
    CMsgGuildPersonaInfo,
    CMsgGuildRole,
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
