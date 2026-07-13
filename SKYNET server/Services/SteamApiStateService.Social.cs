using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    public List<SkyNetUserDto> GetWebUsersWithRelationships(string token)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return new List<SkyNetUserDto>();
            }

            foreach (var account in _state.WebAccounts.Values)
            {
                if (!_state.Users.ContainsKey(account.SteamId))
                {
                    EnsureUser(account.SteamId, SteamIdToAccountId(account.SteamId), DefaultAppId, account.Username);
                }
            }

            return _state.Users.Values
                .OrderBy(u => u.PersonaName)
                .Select(user => CloneUserForViewerLocked(user, session!.SteamId))
                .ToList();
        }
    }

    public SkyNetUserProfileDto? GetWebUserProfile(string token, ulong steamId)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session) || !_state.Users.TryGetValue(steamId, out var user))
            {
                return null;
            }

            if (!_state.Stats.TryGetValue(steamId, out var stats))
            {
                stats = new SkyNetStatsEnvelopeDto { SteamId = steamId, CurrentPlayers = 1 };
                _state.Stats[steamId] = stats;
            }

            return new SkyNetUserProfileDto
            {
                User = CloneUserForViewerLocked(user, session!.SteamId),
                Stats = CloneStats(stats),
                FriendRelationship = GetRelationshipLocked(session.SteamId, steamId),
                IsSelf = session.SteamId == steamId,
                IncomingRequest = FindPendingRequestLocked(steamId, session.SteamId) is { } incoming ? CloneFriendRequestViewLocked(incoming) : null,
                OutgoingRequest = FindPendingRequestLocked(session.SteamId, steamId) is { } outgoing ? CloneFriendRequestViewLocked(outgoing) : null
            };
        }
    }

    public List<SkyNetFriendRequestViewDto> GetIncomingFriendRequests(string token)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return new List<SkyNetFriendRequestViewDto>();
            }

            return _state.FriendRequests
                .Where(request => IsPending(request) && request.ToSteamId == session!.SteamId)
                .OrderByDescending(request => request.CreatedAt)
                .Select(CloneFriendRequestViewLocked)
                .ToList();
        }
    }

    public List<SkyNetFriendRequestViewDto> GetOutgoingFriendRequests(string token)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return new List<SkyNetFriendRequestViewDto>();
            }

            return _state.FriendRequests
                .Where(request => IsPending(request) && request.FromSteamId == session!.SteamId)
                .OrderByDescending(request => request.CreatedAt)
                .Select(CloneFriendRequestViewLocked)
                .ToList();
        }
    }

    public bool SendFriendRequest(string token, ulong targetSteamId)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return false;
            }

            return SendFriendRequestLocked(session!.SteamId, targetSteamId);
        }
    }

    public bool SendFriendRequest(string token, string target)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session) || !TryResolveUserLocked(target, out var targetUser))
            {
                return false;
            }

            return SendFriendRequestLocked(session!.SteamId, targetUser.SteamId);
        }
    }

    public bool AcceptFriendRequest(string token, string requestId)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return false;
            }

            var request = _state.FriendRequests.FirstOrDefault(r =>
                IsPending(r) &&
                r.Id.Equals(requestId ?? string.Empty, StringComparison.OrdinalIgnoreCase) &&
                r.ToSteamId == session!.SteamId);

            return request != null && AcceptFriendRequestLocked(request);
        }
    }

    public bool AcceptFriendRequestFrom(string token, ulong fromSteamId)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return false;
            }

            var request = FindPendingRequestLocked(fromSteamId, session!.SteamId);
            return request != null && AcceptFriendRequestLocked(request);
        }
    }

    public bool DeclineFriendRequest(string token, string requestId)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return false;
            }

            var request = _state.FriendRequests.FirstOrDefault(r =>
                IsPending(r) &&
                r.Id.Equals(requestId ?? string.Empty, StringComparison.OrdinalIgnoreCase) &&
                r.ToSteamId == session!.SteamId);

            return request != null && CloseFriendRequestLocked(request, "declined");
        }
    }

    public bool CancelFriendRequest(string token, string requestId)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return false;
            }

            var request = _state.FriendRequests.FirstOrDefault(r =>
                IsPending(r) &&
                r.Id.Equals(requestId ?? string.Empty, StringComparison.OrdinalIgnoreCase) &&
                r.FromSteamId == session!.SteamId);

            return request != null && CloseFriendRequestLocked(request, "cancelled");
        }
    }

    public bool RemoveFriendOrRequest(string token, ulong otherSteamId)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return false;
            }

            return RemoveFriendOrRequestLocked(session!.SteamId, otherSteamId);
        }
    }

    private bool SendFriendRequestLocked(ulong fromSteamId, ulong targetSteamId)
    {
        if (fromSteamId == targetSteamId || !_state.Users.ContainsKey(fromSteamId) || !_state.Users.ContainsKey(targetSteamId))
        {
            return false;
        }

        if (AreFriendsLocked(fromSteamId, targetSteamId))
        {
            return true;
        }

        var reverseRequest = FindPendingRequestLocked(targetSteamId, fromSteamId);
        if (reverseRequest != null)
        {
            return AcceptFriendRequestLocked(reverseRequest);
        }

        var existing = FindPendingRequestLocked(fromSteamId, targetSteamId);
        if (existing != null)
        {
            return true;
        }

        var request = new SkyNetFriendRequestDto
        {
            Id = Guid.NewGuid().ToString("N"),
            FromSteamId = fromSteamId,
            ToSteamId = targetSteamId,
            Status = "pending",
            CreatedAt = DateTime.UtcNow
        };

        _state.FriendRequests.Add(request);
        SaveState();
        EnqueueRelationshipEventLocked(targetSteamId, fromSteamId, "friend_request_received", FriendRelationshipRequestRecipient, request.Id);
        EnqueueRelationshipEventLocked(fromSteamId, targetSteamId, "friend_request_sent", FriendRelationshipRequestInitiator, request.Id);
        return true;
    }

    private bool AcceptFriendRequestLocked(SkyNetFriendRequestDto request)
    {
        if (!IsPending(request) || !_state.Users.ContainsKey(request.FromSteamId) || !_state.Users.ContainsKey(request.ToSteamId))
        {
            return false;
        }

        request.Status = "accepted";
        request.RespondedAt = DateTime.UtcNow;
        LinkFriendsLocked(request.FromSteamId, request.ToSteamId);
        SaveState();
        EnqueueRelationshipEventLocked(request.FromSteamId, request.ToSteamId, "friend_added", FriendRelationshipFriend, request.Id);
        EnqueueRelationshipEventLocked(request.ToSteamId, request.FromSteamId, "friend_added", FriendRelationshipFriend, request.Id);
        return true;
    }

    private bool CloseFriendRequestLocked(SkyNetFriendRequestDto request, string status)
    {
        if (!IsPending(request))
        {
            return false;
        }

        request.Status = status;
        request.RespondedAt = DateTime.UtcNow;
        SaveState();
        EnqueueRelationshipEventLocked(request.FromSteamId, request.ToSteamId, "friend_removed", FriendRelationshipNone, request.Id);
        EnqueueRelationshipEventLocked(request.ToSteamId, request.FromSteamId, "friend_removed", FriendRelationshipNone, request.Id);
        return true;
    }

    private bool RemoveFriendOrRequestLocked(ulong steamId, ulong otherSteamId)
    {
        if (steamId == otherSteamId || !_state.Users.ContainsKey(otherSteamId))
        {
            return false;
        }

        var changed = false;
        if (AreFriendsLocked(steamId, otherSteamId))
        {
            UnlinkFriendsLocked(steamId, otherSteamId);
            changed = true;
        }

        foreach (var request in _state.FriendRequests.Where(request =>
                     IsPending(request) &&
                     ((request.FromSteamId == steamId && request.ToSteamId == otherSteamId) ||
                      (request.FromSteamId == otherSteamId && request.ToSteamId == steamId))))
        {
            request.Status = request.FromSteamId == steamId ? "cancelled" : "declined";
            request.RespondedAt = DateTime.UtcNow;
            changed = true;
        }

        if (!changed)
        {
            return false;
        }

        SaveState();
        EnqueueRelationshipEventLocked(steamId, otherSteamId, "friend_removed", FriendRelationshipNone, string.Empty);
        EnqueueRelationshipEventLocked(otherSteamId, steamId, "friend_removed", FriendRelationshipNone, string.Empty);
        return true;
    }

    private List<SkyNetUserDto> GetKnownSocialUsersLocked(ulong steamId)
    {
        var result = new Dictionary<ulong, SkyNetUserDto>();

        if (_state.FriendLinks.TryGetValue(steamId, out var friendLinks))
        {
            foreach (var friendSteamId in friendLinks.Where(_state.Users.ContainsKey))
            {
                result[friendSteamId] = CloneUserForViewerLocked(_state.Users[friendSteamId], steamId);
            }
        }

        return result.Values.OrderBy(user => user.PersonaName).ToList();
    }

    private SkyNetUserDto CloneUserForViewerLocked(SkyNetUserDto user, ulong viewerSteamId)
    {
        var clone = CloneUser(user);
        clone.FriendRelationship = GetRelationshipLocked(viewerSteamId, user.SteamId);
        clone.HasFriend = clone.FriendRelationship == FriendRelationshipFriend;
        ApplyDerivedPresenceLocked(clone);
        return clone;
    }

    // Fills the viewer-facing GameState/HeroId from live presence + active match,
    // so friends see "in match (hero X)" / "in menu" / "offline" instead of a
    // bare online flag with stale rich presence.
    private void ApplyDerivedPresenceLocked(SkyNetUserDto clone)
    {
        if (!IsUserOnlineLocked(clone.SteamId))
        {
            // Offline: report no game/lobby/rich presence so friends never show
            // an offline user as "In game". GetFriendGamePlayed on the client is
            // driven by AppId, so it must be zeroed here.
            clone.PersonaState = 0;
            clone.GameState = "offline";
            clone.HeroId = 0;
            clone.AppId = 0;
            clone.LobbyId = 0;
            clone.RichPresence.Clear();
            return;
        }

        clone.PersonaState = 1;
        clone.HeroId = 0;
        clone.GameState = "menu";

        var now = DateTime.UtcNow;
        foreach (var match in _state.DotaMatches.Values)
        {
            if ((now - match.UpdatedAt).TotalMinutes > 5)
            {
                continue;
            }

            var player = match.Players.FirstOrDefault(p => p.SteamId == clone.SteamId);
            if (player != null)
            {
                clone.GameState = "in_match";
                clone.HeroId = player.HeroId;
                break;
            }
        }
    }

    private int GetRelationshipLocked(ulong viewerSteamId, ulong otherSteamId)
    {
        if (viewerSteamId == otherSteamId)
        {
            return FriendRelationshipFriend;
        }

        if (AreFriendsLocked(viewerSteamId, otherSteamId))
        {
            return FriendRelationshipFriend;
        }

        if (FindPendingRequestLocked(otherSteamId, viewerSteamId) != null)
        {
            return FriendRelationshipRequestRecipient;
        }

        if (FindPendingRequestLocked(viewerSteamId, otherSteamId) != null)
        {
            return FriendRelationshipRequestInitiator;
        }

        return FriendRelationshipNone;
    }

    private bool AreFriendsLocked(ulong leftSteamId, ulong rightSteamId)
    {
        return _state.FriendLinks.TryGetValue(leftSteamId, out var links) && links.Contains(rightSteamId);
    }

    private SkyNetFriendRequestDto? FindPendingRequestLocked(ulong fromSteamId, ulong toSteamId)
    {
        return _state.FriendRequests.FirstOrDefault(request =>
            IsPending(request) &&
            request.FromSteamId == fromSteamId &&
            request.ToSteamId == toSteamId);
    }

    private SkyNetFriendRequestViewDto CloneFriendRequestViewLocked(SkyNetFriendRequestDto request)
    {
        _state.Users.TryGetValue(request.FromSteamId, out var fromUser);
        _state.Users.TryGetValue(request.ToSteamId, out var toUser);

        return new SkyNetFriendRequestViewDto
        {
            Id = request.Id,
            FromUser = fromUser == null ? new SkyNetUserDto { SteamId = request.FromSteamId } : CloneUserForViewerLocked(fromUser, request.ToSteamId),
            ToUser = toUser == null ? new SkyNetUserDto { SteamId = request.ToSteamId } : CloneUserForViewerLocked(toUser, request.FromSteamId),
            Status = request.Status,
            CreatedAt = request.CreatedAt
        };
    }

    private void EnqueueRelationshipEventLocked(ulong recipientSteamId, ulong relatedSteamId, string type, int relationship, string requestId)
    {
        if (!_state.Users.TryGetValue(relatedSteamId, out var relatedUser))
        {
            return;
        }

        var online = IsUserOnlineLocked(relatedSteamId) ? 1 : 0;
        EnqueueEvent(recipientSteamId, new SkyNetEventDto
        {
            Type = type,
            SteamId = relatedUser.SteamId,
            AccountId = relatedUser.AccountId,
            PersonaName = relatedUser.PersonaName,
            AppId = online == 1 ? relatedUser.AppId : 0,
            LobbyId = online == 1 ? relatedUser.LobbyId : 0,
            PersonaState = online,
            ChangeFlags = PersonaChangeRelationship,
            RichPresence = online == 1
                ? new Dictionary<string, string>(relatedUser.RichPresence)
                : new Dictionary<string, string>(),
            FriendRelationship = relationship,
            RequestId = requestId ?? string.Empty
        });
    }

    private static bool IsPending(SkyNetFriendRequestDto request)
    {
        return request.Status.Equals("pending", StringComparison.OrdinalIgnoreCase);
    }
}
