import { DotaTeam, DotaTeamMember, RawMessageContext, gc } from "../framework/gc";
import {
    CMsgDOTACreateTeam,
    CMsgDOTACreateTeamResponse,
    CMsgDOTACreateTeamResponse_Result,
    CMsgDOTAEditTeamDetails,
    CMsgDOTAEditTeamDetailsResponse,
    CMsgDOTAEditTeamDetailsResponse_Result,
    CMsgDOTAKickTeamMember,
    CMsgDOTAKickTeamMemberResponse,
    CMsgDOTAKickTeamMemberResponse_Result,
    CMsgDOTATeamInviteGCImmediateResponseToInviter,
    CMsgDOTATeamInviteGCRequestToInvitee,
    CMsgDOTATeamInviteGCResponseToInvitee,
    CMsgDOTATeamInviteGCResponseToInviter,
    CMsgDOTATeamInviteInviteeResponseToGC,
    CMsgDOTATeamInviteInviterToGC,
    CMsgDOTATransferTeamAdmin,
    CMsgDOTATransferTeamAdminResponse,
    CMsgDOTATransferTeamAdminResponse_Result,
    CMsgGCPlayerInfoSubmit,
    CMsgGCPlayerInfoSubmitResponse,
    CMsgGCPlayerInfoSubmitResponse_EResult,
    CMsgGCRankedPlayerInfoSubmit,
    CMsgGCRankedPlayerInfoSubmitResponse,
    CMsgGCRankedPlayerInfoSubmitResponse_EResult,
    CMsgResponseTeamFanfare,
    CMsgTeamFanfare,
    ETeamInviteResult,
    Msg,
    Proto
} from "../generated/dota";

const TEAM_ROLE_MEMBER = 0;
const TEAM_ROLE_ADMIN = 1;
const TEAM_MEMBER_LIMIT = 5;

interface PendingTeamInvite {
    inviterAccountId: number;
    inviterSteamId: bigint;
    targetAccountId: number;
    teamId: number;
    teamName: string;
}

const pendingInvites: PendingTeamInvite[] = [];

export function registerTeams(): void {
    const teams = new Teams();
    teams.register();
}

export class Teams {
    register(): void {
        gc.onMessage(Msg.GCCreateTeam, (ctx) => this.createTeam(ctx));
        gc.onMessage(Msg.GCEditTeamDetails, (ctx) => this.editTeam(ctx));
        gc.onMessage(Msg.GCTeamInviteInviterToGC, (ctx) => this.invite(ctx));
        gc.onMessage(Msg.GCTeamInviteInviteeResponseToGC, (ctx) => this.inviteResponse(ctx));
        gc.onMessage(Msg.GCKickTeamMember, (ctx) => this.kickMember(ctx));
        gc.onMessage(Msg.GCTransferTeamAdmin, (ctx) => this.transferAdmin(ctx));
        gc.onMessage(Msg.TeamFanfare, (ctx) => this.teamFanfare(ctx));
        gc.onMessage(Msg.GCRankedPlayerInfoSubmit, (ctx) => this.rankedPlayerInfo(ctx));
        gc.onMessage(Msg.GCPlayerInfoSubmit, (ctx) => this.playerInfoSubmit(ctx));
    }

    private createTeam(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgDOTACreateTeam) as CMsgDOTACreateTeam;
        let result = validateCreateTeam(ctx, request);
        let teamId = 0;

        if (result === CMsgDOTACreateTeamResponse_Result.Success) {
            teamId = ctx.services.teams.nextTeamId();
            const team = ctx.services.teams.upsert(toTeamUpsert(teamId, request));
            if (team === null || !ctx.services.teams.addMember(teamId, ctx.accountId, TEAM_ROLE_ADMIN)) {
                result = CMsgDOTACreateTeamResponse_Result.UnspecifiedError;
                teamId = 0;
            }
        }

        ctx.reply<CMsgDOTACreateTeamResponse>(Msg.GCCreateTeamResponse, Proto.CMsgDOTACreateTeamResponse, {
            result,
            teamId
        });
        return true;
    }

    private editTeam(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgDOTAEditTeamDetails) as CMsgDOTAEditTeamDetails;
        const teamId = request.teamId ?? 0;
        const team = ctx.services.teams.get(teamId);
        let result: number = CMsgDOTAEditTeamDetailsResponse_Result.FailureUnspecifiedError;

        if (team !== null && isAdmin(team, ctx.accountId) && hasText(request.name) && hasText(request.tag)) {
            ctx.services.teams.upsert(toTeamUpsert(teamId, request, team));
            result = CMsgDOTAEditTeamDetailsResponse_Result.Success;
        } else if (team !== null && !isMember(team, ctx.accountId)) {
            result = CMsgDOTAEditTeamDetailsResponse_Result.FailureNotMember;
        }

        ctx.reply<CMsgDOTAEditTeamDetailsResponse>(
            Msg.GCEditTeamDetailsResponse,
            Proto.CMsgDOTAEditTeamDetailsResponse,
            { result }
        );
        return true;
    }

    private invite(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgDOTATeamInviteInviterToGC) as CMsgDOTATeamInviteInviterToGC;
        const team = ctx.services.teams.get(request.teamId ?? 0);
        let result: number = ETeamInviteResult.TeamInviteErrorUnspecified;
        let inviteeName = "";

        if (team !== null && !isAdmin(team, ctx.accountId)) {
            result = ETeamInviteResult.TeamInviteErrorInviterNotAdmin;
        } else if (team !== null && isMember(team, request.accountId ?? 0)) {
            result = ETeamInviteResult.TeamInviteErrorInviteeAlreadyMember;
        } else if (team !== null && teamMembers(team).length >= TEAM_MEMBER_LIMIT) {
            result = ETeamInviteResult.TeamInviteErrorTeamAtMemberLimit;
        } else if (team !== null && pendingInviteFor(request.accountId ?? 0) !== null) {
            result = ETeamInviteResult.TeamInviteErrorInviteeBusy;
        } else if (team !== null) {
            const targetAccount = request.accountId ?? 0;
            const accountName = ctx.services.stats.lookupAccountName(targetAccount).accountName;
            inviteeName = accountName;
            // Team invites are an ephemeral GC handshake: the inviter gets 7123 immediately,
            // the invitee receives 7124 by SteamID, and the later invitee response drives 7126/7127.
            pendingInvites.push({
                inviterAccountId: ctx.accountId,
                inviterSteamId: ctx.steamId,
                targetAccountId: targetAccount,
                teamId: team.teamId,
                teamName: team.name
            });
            queueToSteam<CMsgDOTATeamInviteGCRequestToInvitee>(
                ctx,
                steamIdFromAccountId(targetAccount),
                Msg.GCTeamInviteGCRequestToInvitee,
                Proto.CMsgDOTATeamInviteGCRequestToInvitee,
                {
                    inviterAccountId: ctx.accountId,
                    teamName: team.name,
                    teamTag: team.tag,
                    logo: team.logo
                }
            );
            result = ETeamInviteResult.TeamInviteSuccess;
        }

        ctx.reply<CMsgDOTATeamInviteGCImmediateResponseToInviter>(
            Msg.GCTeamInviteGCImmediateResponseToInviter,
            Proto.CMsgDOTATeamInviteGCImmediateResponseToInviter,
            { result, inviteeName }
        );
        return true;
    }

    private inviteResponse(ctx: RawMessageContext): boolean {
        const request = ctx.decode(
            Proto.CMsgDOTATeamInviteInviteeResponseToGC
        ) as CMsgDOTATeamInviteInviteeResponseToGC;
        const invite = takePendingInvite(ctx.accountId);
        const result = request.result ?? ETeamInviteResult.TeamInviteFailureInviteRejected;
        if (invite === null) {
            ctx.reply<CMsgDOTATeamInviteGCResponseToInvitee>(
                Msg.GCTeamInviteGCResponseToInvitee,
                Proto.CMsgDOTATeamInviteGCResponseToInvitee,
                { result: ETeamInviteResult.TeamInviteErrorUnspecified }
            );
            return true;
        }

        if (result === ETeamInviteResult.TeamInviteSuccess) {
            ctx.services.teams.addMember(invite.teamId, ctx.accountId, TEAM_ROLE_MEMBER);
        }

        ctx.reply<CMsgDOTATeamInviteGCResponseToInvitee>(
            Msg.GCTeamInviteGCResponseToInvitee,
            Proto.CMsgDOTATeamInviteGCResponseToInvitee,
            { result, teamName: invite.teamName }
        );
        queueToSteam<CMsgDOTATeamInviteGCResponseToInviter>(
            ctx,
            invite.inviterSteamId,
            Msg.GCTeamInviteGCResponseToInviter,
            Proto.CMsgDOTATeamInviteGCResponseToInviter,
            { result, inviteeName: ctx.personaName }
        );
        return true;
    }

    private kickMember(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgDOTAKickTeamMember) as CMsgDOTAKickTeamMember;
        const team = ctx.services.teams.get(request.teamId ?? 0);
        let result: number = CMsgDOTAKickTeamMemberResponse_Result.FailureUnspecifiedError;

        if (team !== null && !isAdmin(team, ctx.accountId)) {
            result = CMsgDOTAKickTeamMemberResponse_Result.FailureKickerNotAdmin;
        } else if (team !== null && !isMember(team, request.accountId ?? 0)) {
            result = CMsgDOTAKickTeamMemberResponse_Result.FailureKickeeNotMember;
        } else if (team !== null) {
            ctx.services.teams.removeMember(team.teamId, request.accountId ?? 0);
            ctx.services.teams.deletePlayerInfo(request.accountId ?? 0);
            result = CMsgDOTAKickTeamMemberResponse_Result.Success;
        }

        ctx.reply<CMsgDOTAKickTeamMemberResponse>(Msg.GCKickTeamMemberResponse, Proto.CMsgDOTAKickTeamMemberResponse, {
            result
        });
        return true;
    }

    private transferAdmin(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgDOTATransferTeamAdmin) as CMsgDOTATransferTeamAdmin;
        const team = ctx.services.teams.get(request.teamId ?? 0);
        const newAdminAccountId = request.newAdminAccountId ?? 0;
        let result: number = CMsgDOTATransferTeamAdminResponse_Result.FailureUnspecifiedError;

        if (team !== null && !isAdmin(team, ctx.accountId)) {
            result = CMsgDOTATransferTeamAdminResponse_Result.FailureNotAdmin;
        } else if (team !== null && newAdminAccountId === ctx.accountId) {
            result = CMsgDOTATransferTeamAdminResponse_Result.FailureSameAccount;
        } else if (team !== null && !isMember(team, newAdminAccountId)) {
            result = CMsgDOTATransferTeamAdminResponse_Result.FailureNotMember;
        } else if (team !== null) {
            const members = teamMembers(team);
            for (let i = 0; i < members.length; i++) {
                const member = members[i];
                ctx.services.teams.addMember(
                    team.teamId,
                    member.accountId,
                    member.accountId === newAdminAccountId ? TEAM_ROLE_ADMIN : TEAM_ROLE_MEMBER
                );
            }
            result = CMsgDOTATransferTeamAdminResponse_Result.Success;
        }

        ctx.reply<CMsgDOTATransferTeamAdminResponse>(
            Msg.GCTransferTeamAdminResponse,
            Proto.CMsgDOTATransferTeamAdminResponse,
            { result }
        );
        return true;
    }

    private teamFanfare(ctx: RawMessageContext): boolean {
        ctx.decode(Proto.CMsgTeamFanfare) as CMsgTeamFanfare;
        ctx.reply<CMsgResponseTeamFanfare>(Msg.ResponseTeamFanfare, Proto.CMsgResponseTeamFanfare, {
            fanfareGoodguys: 0,
            fanfareBadguys: 0
        });
        return true;
    }

    private rankedPlayerInfo(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgGCRankedPlayerInfoSubmit) as CMsgGCRankedPlayerInfoSubmit;
        const info = ctx.services.teams.getPlayerInfo(ctx.accountId);
        // Current protos split the old player-info flow: 7454 updates ranked display name,
        // while 7456 persists the complete team/player metadata used by profile UIs.
        ctx.services.teams.savePlayerInfo({
            accountId: ctx.accountId,
            name: info?.name ?? request.name ?? ctx.personaName,
            countryCode: info?.countryCode ?? "",
            fantasyRole: info?.fantasyRole ?? 0,
            teamId: info?.teamId ?? 0,
            sponsor: info?.sponsor ?? "",
            realName: info?.realName ?? ""
        });
        ctx.reply<CMsgGCRankedPlayerInfoSubmitResponse>(
            Msg.GCRankedPlayerInfoSubmitResponse,
            Proto.CMsgGCRankedPlayerInfoSubmitResponse,
            { result: CMsgGCRankedPlayerInfoSubmitResponse_EResult.Success }
        );
        return true;
    }

    private playerInfoSubmit(ctx: RawMessageContext): boolean {
        const request = ctx.decode(Proto.CMsgGCPlayerInfoSubmit) as CMsgGCPlayerInfoSubmit;
        const teamId = request.teamId ?? 0;
        const team = teamId === 0 ? null : ctx.services.teams.get(teamId);
        let result: number = CMsgGCPlayerInfoSubmitResponse_EResult.Success;

        if (teamId !== 0 && (team === null || !isMember(team, ctx.accountId))) {
            result = CMsgGCPlayerInfoSubmitResponse_EResult.ErrorNotMemberOfTeam;
        } else {
            ctx.services.teams.savePlayerInfo({
                accountId: ctx.accountId,
                name: request.playerName ?? ctx.personaName,
                countryCode: request.countryCode ?? "",
                fantasyRole: request.fantasyRole ?? 0,
                teamId,
                sponsor: request.sponsor ?? "",
                realName: request.realName ?? ""
            });
        }

        ctx.reply<CMsgGCPlayerInfoSubmitResponse>(
            Msg.GCPlayerInfoSubmitResponse,
            Proto.CMsgGCPlayerInfoSubmitResponse,
            { result }
        );
        return true;
    }
}

function validateCreateTeam(ctx: RawMessageContext, request: CMsgDOTACreateTeam): number {
    const name = request.name ?? "";
    const tag = request.tag ?? "";
    if (ctx.services.teams.getForAccount(ctx.accountId).length > 0) {
        return CMsgDOTACreateTeamResponse_Result.CreatorTeamLimitReached;
    }
    if (!hasText(name)) {
        return CMsgDOTACreateTeamResponse_Result.NameEmpty;
    }
    if (name.length > 20) {
        return CMsgDOTACreateTeamResponse_Result.NameTooLong;
    }
    if (!ctx.services.teams.nameAvailable(name)) {
        return CMsgDOTACreateTeamResponse_Result.NameTaken;
    }
    if (!hasText(tag)) {
        return CMsgDOTACreateTeamResponse_Result.TagEmpty;
    }
    if (tag.length > 20) {
        return CMsgDOTACreateTeamResponse_Result.TagTooLong;
    }
    if (hasLowercaseOrDigit(tag)) {
        return CMsgDOTACreateTeamResponse_Result.TagBadCharacters;
    }
    if (!ctx.services.teams.tagAvailable(tag)) {
        return CMsgDOTACreateTeamResponse_Result.TagTaken;
    }
    if ((request.logo ?? 0n) === 0n || (request.baseLogo ?? 0n) === 0n || (request.bannerLogo ?? 0n) === 0n) {
        return CMsgDOTACreateTeamResponse_Result.LogoUploadFailed;
    }
    return CMsgDOTACreateTeamResponse_Result.Success;
}

function toTeamUpsert(teamId: number, request: CMsgDOTACreateTeam | CMsgDOTAEditTeamDetails, existing?: DotaTeam) {
    return {
        teamId,
        name: request.name ?? existing?.name ?? "",
        tag: request.tag ?? existing?.tag ?? "",
        logo: request.logo ?? existing?.logo ?? 0n,
        baseLogo: request.baseLogo ?? existing?.baseLogo ?? 0n,
        bannerLogo: request.bannerLogo ?? existing?.bannerLogo ?? 0n,
        sponsorLogo: request.sponsorLogo ?? 0n,
        countryCode: request.countryCode ?? existing?.countryCode ?? "",
        url: request.url ?? existing?.url ?? "",
        pickupTeam: "pickupTeam" in request ? (request.pickupTeam ?? false) : false,
        abbreviation: request.abbreviation ?? existing?.abbreviation ?? ""
    };
}

function isMember(team: DotaTeam, accountId: number): boolean {
    return findMember(team, accountId) !== null;
}

function isAdmin(team: DotaTeam, accountId: number): boolean {
    return findMember(team, accountId)?.role === TEAM_ROLE_ADMIN;
}

function findMember(team: DotaTeam, accountId: number): DotaTeamMember | null {
    const members = teamMembers(team);
    for (let i = 0; i < members.length; i++) {
        if (members[i].accountId === accountId) {
            return members[i];
        }
    }
    return null;
}

function teamMembers(team: DotaTeam): DotaTeamMember[] {
    return Array.isArray(team.members) ? team.members : [];
}

function pendingInviteFor(accountId: number): PendingTeamInvite | null {
    for (let i = 0; i < pendingInvites.length; i++) {
        if (pendingInvites[i].targetAccountId === accountId) {
            return pendingInvites[i];
        }
    }
    return null;
}

function takePendingInvite(accountId: number): PendingTeamInvite | null {
    for (let i = 0; i < pendingInvites.length; i++) {
        if (pendingInvites[i].targetAccountId === accountId) {
            const invite = pendingInvites[i];
            pendingInvites.splice(i, 1);
            return invite;
        }
    }
    return null;
}

function steamIdFromAccountId(accountId: number): bigint {
    return 76561197960265728n + BigInt(accountId >>> 0);
}

function hasText(value: string | undefined): boolean {
    return (value ?? "").trim().length > 0;
}

function hasLowercaseOrDigit(value: string): boolean {
    for (let i = 0; i < value.length; i++) {
        const code = value.charCodeAt(i);
        if ((code >= 48 && code <= 57) || (code >= 97 && code <= 122)) {
            return true;
        }
    }
    return false;
}

function queueToSteam<TMessage>(
    ctx: RawMessageContext,
    steamId: bigint,
    messageType: number,
    proto: { name: string },
    message: TMessage
): boolean {
    if (steamId === ctx.steamId) {
        ctx.send(messageType, proto, message);
        return true;
    }

    return ctx.services.lobby.queueMessage(steamId, messageType, ctx.encode(proto, message), true);
}
