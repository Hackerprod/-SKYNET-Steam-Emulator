using System.Text.Json;
using SKYNET_server.Services;

namespace SKYNET_server.GC.Dota2;

public sealed partial class DotaGcBackend
{
    private const uint LobbyInviteLifetimeSeconds = 600;
    private static readonly JsonSerializerOptions LobbyInviteJsonOptions = new(JsonSerializerDefaults.Web);

    public bool DotaUserExists(string steamId)
    {
        return ulong.TryParse(steamId, out var parsed) && parsed != 0 &&
            (UserExistsProvider == null || UserExistsProvider(parsed));
    }

    public bool DotaUserOnline(string steamId)
    {
        return ulong.TryParse(steamId, out var parsed) && parsed != 0 &&
            UserOnlineProvider != null && UserOnlineProvider(parsed);
    }

    public string DotaLobbyInviteUpsert(
        string lobbyId,
        string targetSteamId,
        string senderSteamId,
        string payloadJson)
    {
        if (!ulong.TryParse(lobbyId, out var lobby) || lobby == 0 ||
            !ulong.TryParse(targetSteamId, out var target) || target == 0 ||
            !ulong.TryParse(senderSteamId, out var sender) || sender == 0 ||
            !IsValidInvitePayload(payloadJson))
        {
            return string.Empty;
        }

        var invite = LobbyInviteStoreOrThrow().Upsert(lobby, target, sender, payloadJson, out var replaced);
        return JsonSerializer.Serialize(
            new LobbyInviteMutationDto(ToDto(invite), replaced == null ? null : ToDto(replaced)),
            LobbyInviteJsonOptions);
    }

    public string DotaLobbyInvitesForCurrent()
    {
        return SerializeInvites(LobbyInviteStoreOrThrow().GetForTarget(SteamId, LobbyInviteCutoff()));
    }

    public string DotaLobbyInviteTake(string lobbyId)
    {
        if (!ulong.TryParse(lobbyId, out var lobby) || lobby == 0)
        {
            return string.Empty;
        }

        var invite = LobbyInviteStoreOrThrow().Take(lobby, SteamId, LobbyInviteCutoff());
        return invite == null ? string.Empty : JsonSerializer.Serialize(ToDto(invite), LobbyInviteJsonOptions);
    }

    public string DotaLobbyInviteDeleteForCurrent()
    {
        return SerializeInvites(LobbyInviteStoreOrThrow().DeleteForTarget(SteamId));
    }

    public string DotaLobbyInviteDeleteLobby(string lobbyId)
    {
        return ulong.TryParse(lobbyId, out var lobby) && lobby != 0
            ? SerializeInvites(LobbyInviteStoreOrThrow().DeleteForLobby(lobby))
            : "[]";
    }

    public string DotaLobbyInvitePrune()
    {
        return SerializeInvites(LobbyInviteStoreOrThrow().PruneCreatedBefore(LobbyInviteCutoff()));
    }

    private static bool IsValidInvitePayload(string payloadJson)
    {
        if (string.IsNullOrWhiteSpace(payloadJson) || payloadJson.Length > 64 * 1024)
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(payloadJson);
            return document.RootElement.ValueKind == JsonValueKind.Object;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static string SerializeInvites(IEnumerable<DotaLobbyInviteRecord> invites)
    {
        return JsonSerializer.Serialize(invites.Select(ToDto), LobbyInviteJsonOptions);
    }

    private static LobbyInviteDto ToDto(DotaLobbyInviteRecord invite)
    {
        return new LobbyInviteDto(
            invite.LobbyId.ToString(System.Globalization.CultureInfo.InvariantCulture),
            invite.TargetSteamId.ToString(System.Globalization.CultureInfo.InvariantCulture),
            invite.InviteGid.ToString(System.Globalization.CultureInfo.InvariantCulture),
            invite.SenderSteamId.ToString(System.Globalization.CultureInfo.InvariantCulture),
            invite.PayloadJson,
            invite.CreatedAt);
    }

    private static DotaLobbyInviteStore LobbyInviteStoreOrThrow()
    {
        return LobbyInviteStore ?? throw new InvalidOperationException("Dota lobby invite store is not initialized.");
    }

    private static uint LobbyInviteCutoff()
    {
        var now = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return now > LobbyInviteLifetimeSeconds ? now - LobbyInviteLifetimeSeconds : 0;
    }

    private sealed record LobbyInviteMutationDto(LobbyInviteDto Invite, LobbyInviteDto? Replaced);

    private sealed record LobbyInviteDto(
        string LobbyId,
        string TargetSteamId,
        string InviteGid,
        string SenderSteamId,
        string PayloadJson,
        uint CreatedAt);
}
