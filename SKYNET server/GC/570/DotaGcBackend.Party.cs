using SKYNET_server.Models;
using SKYNET_server.Services;

namespace SKYNET_server.GC.Dota2;

public sealed partial class DotaGcBackend
{
    private const int DotaPartyObjectTypeId = 2003;
    private const int DotaPartyInviteObjectTypeId = 2006;
    private const uint DotaPartyOwnerType = 2;
    private const uint DotaPartyInviteOwnerType = 4;
    private const uint EMsgGCInviteToParty = 4501;
    private const uint EMsgGCPartyInviteResponse = 4503;
    private const uint EMsgGCKickFromParty = 4504;
    private const uint EMsgGCLeaveParty = 4505;
    private const uint EMsgGCPartyMemberSetCoach = 7343;
    private const uint EMsgClientToGCSetPartyLeader = 7588;
    private const uint EMsgClientToGCPingData = 8068;
    private const uint EMsgPartyReadyCheckResponse = 8263;

    public bool DotaPartyEmitCurrent()
    {
        var party = PartyStoreOrThrow().GetPartyByMember(SteamId);
        if (party == null)
        {
            return false;
        }

        QueuePartySubscribe(SteamId, party);
        QueuePartyUpdate(party);
        return true;
    }

    public bool DotaPartyInviteToParty()
    {
        if (!TryReadFixed64Field(_requestBody, 1, out var targetSteamId) &&
            !TryReadVarintField(_requestBody, 1, out targetSteamId))
        {
            return Ignore();
        }

        var ping = TryReadLengthDelimitedField(_requestBody, 5, out var pingBody)
            ? ReadPartyPingData(pingBody)
            : new DotaPartyPingData();
        var teamId = (uint)ReadVarint(3, 0);
        var asCoach = ReadVarint(4, 0) != 0;
        var store = PartyStoreOrThrow();
        var party = store.EnsureParty(SteamId, AccountId, PersonaName, ping);
        if (party.Members.Count >= 5)
        {
            return Ignore();
        }

        QueuePartySubscribe(SteamId, party);
        QueuePartyUpdate(party);

        var invite = store.CreateInvite(party.PartyId, targetSteamId, SteamId, PersonaName, teamId, asCoach);
        QueuePartyInvite(targetSteamId, party, invite);
        return true;
    }

    public bool DotaPartyInviteResponse()
    {
        var partyId = ReadVarint(1, 0);
        var accept = ReadVarint(2, 0) != 0;
        if (partyId == 0 || !accept)
        {
            return Ignore();
        }

        var ping = TryReadLengthDelimitedField(_requestBody, 8, out var pingBody)
            ? ReadPartyPingData(pingBody)
            : new DotaPartyPingData();
        var store = PartyStoreOrThrow();
        var current = store.GetPartyByMember(SteamId);
        if (current != null && current.PartyId != partyId)
        {
            DetachPartyMember(SteamId);
        }

        var party = store.AddMember(partyId, SteamId, AccountId, PersonaName, ping);
        if (party == null)
        {
            return Ignore();
        }

        QueuePartySubscribe(SteamId, party);
        QueuePartyUpdate(party);
        return true;
    }

    public bool DotaPartyPingData()
    {
        var store = PartyStoreOrThrow();
        var party = store.GetPartyByMember(SteamId);
        if (party == null)
        {
            return Ignore();
        }

        party = store.SetMemberPing(party.PartyId, SteamId, ReadPartyPingData(_requestBody));
        if (party != null)
        {
            QueuePartyUpdate(party, SteamId);
        }

        return true;
    }

    public bool DotaPartySetLeader()
    {
        if (!TryReadFixed64Field(_requestBody, 1, out var leaderSteamId) &&
            !TryReadVarintField(_requestBody, 1, out leaderSteamId))
        {
            return Ignore();
        }

        var store = PartyStoreOrThrow();
        var party = store.GetPartyByMember(SteamId);
        if (party == null)
        {
            return Ignore();
        }

        party = store.SetLeader(party.PartyId, leaderSteamId);
        if (party != null)
        {
            QueuePartyUpdate(party);
        }

        return true;
    }

    public bool DotaPartyLeave()
    {
        DetachPartyMember(SteamId);
        return true;
    }

    public bool DotaPartyKick()
    {
        if (!TryReadFixed64Field(_requestBody, 1, out var kickedSteamId) &&
            !TryReadVarintField(_requestBody, 1, out kickedSteamId))
        {
            return Ignore();
        }

        DetachPartyMember(kickedSteamId);
        return true;
    }

    public bool DotaPartySetCoach()
    {
        var wantsCoach = ReadVarint(1, 0) != 0;
        var store = PartyStoreOrThrow();
        var party = store.GetPartyByMember(SteamId);
        if (party == null)
        {
            return Ignore();
        }

        party = store.SetMemberCoach(party.PartyId, SteamId, wantsCoach);
        if (party != null)
        {
            QueuePartyUpdate(party);
        }

        return true;
    }

    public bool DotaPartyReadyCheckRequest()
    {
        var store = PartyStoreOrThrow();
        var party = store.GetPartyByMember(SteamId);
        if (party == null)
        {
            return Reply(EMsgPartyReadyCheckResponse, BuildResult(2));
        }

        var now = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (party.ReadyFinishTimestamp > now)
        {
            return Reply(EMsgPartyReadyCheckResponse, BuildResult(1));
        }

        party = store.StartReadyCheck(party.PartyId, AccountId, 60);
        if (party != null)
        {
            QueuePartyUpdate(party);
        }

        return Reply(EMsgPartyReadyCheckResponse, BuildResult(0));
    }

    public bool DotaPartyReadyCheckAcknowledge()
    {
        var readyStatus = (uint)ReadVarint(1, 0);
        var store = PartyStoreOrThrow();
        var party = store.GetPartyByMember(SteamId);
        if (party == null)
        {
            return Ignore();
        }

        party = store.AcknowledgeReadyCheck(party.PartyId, SteamId, readyStatus);
        if (party != null)
        {
            QueuePartyUpdate(party);
        }

        return true;
    }

    private DotaPartyStore PartyStoreOrThrow()
    {
        return PartyStore ?? throw new InvalidOperationException("Dota party store is not initialized.");
    }

    private void DetachPartyMember(ulong memberSteamId)
    {
        var store = PartyStoreOrThrow();
        var party = store.GetPartyByMember(memberSteamId);
        if (party == null)
        {
            return;
        }

        var recipients = party.Members.Select(member => member.SteamId).ToList();
        if (party.Members.Count <= 2)
        {
            store.DeleteParty(party.PartyId);
            foreach (var recipient in recipients)
            {
                QueuePartyDestroy(recipient, party);
            }

            return;
        }

        var after = store.RemoveMember(party.PartyId, memberSteamId);
        QueuePartyDestroy(memberSteamId, party);
        if (after != null)
        {
            QueuePartyUpdate(after);
        }
    }

    private void QueuePartySubscribe(ulong steamId, DotaPartyState party)
    {
        QueuePartyClientProto(steamId, EMsgSOCacheSubscribed, BuildPartySoCacheSubscribed(party));
        QueuePartyClientProto(steamId, EMsgSOSingleObject, BuildPartySingleObject(party));
    }

    private void QueuePartyUpdate(DotaPartyState party, ulong exceptSteamId = 0)
    {
        var payload = BuildPartyMultipleObjects(party);
        foreach (var member in party.Members)
        {
            if (member.SteamId != exceptSteamId)
            {
                QueuePartyClientProto(member.SteamId, EMsgSOMultipleObjects, payload);
            }
        }
    }

    private void QueuePartyDestroy(ulong steamId, DotaPartyState party)
    {
        QueuePartyClientProto(steamId, EMsgSOCacheUnsubscribed, BuildPartySoCacheUnsubscribed(party.PartyId));
        QueuePartyClientProto(steamId, EMsgSOSingleObjectDestroyed, BuildPartySingleObjectDestroyed(party.PartyId));
    }

    private void QueuePartyInvite(ulong targetSteamId, DotaPartyState party, DotaPartyInvite invite)
    {
        QueuePartyClientProto(targetSteamId, EMsgSOCacheSubscribed, BuildPartyInviteSubscribed(party, invite));
    }

    private void QueuePartyClientProto(ulong recipientSteamId, uint messageType, byte[] payload)
    {
        if (recipientSteamId == SteamId)
        {
            AddProto(messageType, payload);
            return;
        }

        PendingMessageQueued?.Invoke(recipientSteamId, new SkyNetGCMessageDto
        {
            AppId = AppId,
            MessageType = messageType,
            PayloadBase64 = Encode(payload),
            Protobuf = true
        });
    }

    private static byte[] BuildPartySoCacheSubscribed(DotaPartyState party)
    {
        var response = new List<byte>();
        WriteBytesField(response, 2, BuildSubscribedType(DotaPartyObjectTypeId, BuildPartyObject(party)));
        WriteFixed64Field(response, 3, GenerateSteamObjectId());
        WriteBytesField(response, 4, BuildOwnerSoid(DotaPartyOwnerType, party.PartyId));
        return response.ToArray();
    }

    private static byte[] BuildPartySoCacheUnsubscribed(ulong partyId)
    {
        var response = new List<byte>();
        WriteBytesField(response, 2, BuildOwnerSoid(DotaPartyOwnerType, partyId));
        return response.ToArray();
    }

    private static byte[] BuildPartySingleObject(DotaPartyState party)
    {
        var response = new List<byte>();
        WriteVarintField(response, 2, DotaPartyObjectTypeId);
        WriteBytesField(response, 3, BuildPartyObject(party));
        WriteFixed64Field(response, 4, GenerateSteamObjectId());
        WriteBytesField(response, 5, BuildOwnerSoid(DotaPartyOwnerType, party.PartyId));
        return response.ToArray();
    }

    private static byte[] BuildPartySingleObjectDestroyed(ulong partyId)
    {
        var response = new List<byte>();
        WriteVarintField(response, 2, DotaPartyObjectTypeId);
        WriteFixed64Field(response, 4, GenerateSteamObjectId());
        WriteBytesField(response, 5, BuildOwnerSoid(DotaPartyOwnerType, partyId));
        return response.ToArray();
    }

    private static byte[] BuildPartyMultipleObjects(DotaPartyState party)
    {
        var response = new List<byte>();
        WriteBytesField(response, 2, BuildSubscribedType(DotaPartyObjectTypeId, BuildPartyObject(party)));
        WriteFixed64Field(response, 3, GenerateSteamObjectId());
        WriteBytesField(response, 6, BuildOwnerSoid(DotaPartyOwnerType, party.PartyId));
        return response.ToArray();
    }

    private static byte[] BuildPartyInviteSubscribed(DotaPartyState party, DotaPartyInvite invite)
    {
        var response = new List<byte>();
        WriteBytesField(response, 2, BuildSubscribedType(DotaPartyInviteObjectTypeId, BuildPartyInviteObject(party, invite)));
        WriteFixed64Field(response, 3, GenerateSteamObjectId());
        WriteBytesField(response, 4, BuildOwnerSoid(DotaPartyInviteOwnerType, invite.InviteGid));
        return response.ToArray();
    }

    private static byte[] BuildPartyObject(DotaPartyState party)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, party.PartyId);
        WriteFixed64Field(response, 2, party.LeaderSteamId);
        foreach (var member in party.Members)
        {
            WriteFixed64Field(response, 3, member.SteamId);
        }

        WriteVarintField(response, 6, party.State);
        WriteVarintField(response, 21, 0);
        foreach (var member in party.Members)
        {
            WriteBytesField(response, 29, BuildPartyMemberObject(member));
        }

        if (party.ReadyStartTimestamp != 0 || party.ReadyFinishTimestamp != 0)
        {
            WriteBytesField(response, 62, BuildReadyCheckObject(party));
        }

        return response.ToArray();
    }

    private static byte[] BuildPartyMemberObject(DotaPartyMember member)
    {
        var response = new List<byte>();
        WriteVarintField(response, 2, member.IsCoach ? 1u : 0u);
        foreach (var code in member.RegionCodes)
        {
            WriteVarintField(response, 4, code);
        }

        foreach (var ping in member.RegionPings)
        {
            WriteVarintField(response, 5, ping);
        }

        WriteVarintField(response, 6, member.RegionPingFailedBitmask);
        WriteVarintField(response, 10, member.IsPlusSubscriber ? 1u : 0u);
        return response.ToArray();
    }

    private static byte[] BuildPartyInviteObject(DotaPartyState party, DotaPartyInvite invite)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, invite.PartyId);
        WriteFixed64Field(response, 2, invite.SenderSteamId);
        WriteStringField(response, 3, invite.SenderName);
        foreach (var member in party.Members)
        {
            var item = new List<byte>();
            WriteStringField(item, 1, member.PersonaName);
            WriteFixed64Field(item, 2, member.SteamId);
            WriteVarintField(item, 4, member.IsCoach ? 1u : 0u);
            WriteBytesField(response, 4, item.ToArray());
        }

        WriteVarintField(response, 5, invite.TeamId);
        WriteVarintField(response, 6, invite.LowPriorityStatus ? 1u : 0u);
        WriteVarintField(response, 7, invite.AsCoach ? 1u : 0u);
        WriteFixed64Field(response, 8, invite.InviteGid);
        return response.ToArray();
    }

    private static byte[] BuildReadyCheckObject(DotaPartyState party)
    {
        var response = new List<byte>();
        WriteVarintField(response, 1, party.ReadyStartTimestamp);
        WriteVarintField(response, 2, party.ReadyFinishTimestamp);
        WriteVarintField(response, 3, party.ReadyInitiatorAccountId);
        foreach (var member in party.Members.Where(member => member.ReadyStatus != 0))
        {
            var item = new List<byte>();
            WriteVarintField(item, 1, member.AccountId);
            WriteVarintField(item, 2, member.ReadyStatus);
            WriteBytesField(response, 4, item.ToArray());
        }

        return response.ToArray();
    }

    private static DotaPartyPingData ReadPartyPingData(byte[] body)
    {
        return new DotaPartyPingData
        {
            RegionCodes = ReadVarintFields(body, 8).Select(value => (uint)value).ToList(),
            RegionPings = ReadVarintFields(body, 9).Select(value => (uint)value).ToList(),
            RegionPingFailedBitmask = (uint)ReadVarintField(body, 10)
        };
    }
}
