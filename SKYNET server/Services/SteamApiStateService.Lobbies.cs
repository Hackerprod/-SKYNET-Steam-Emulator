using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    public List<ApiLobby>? QueryLobbies(string token, ApiLobbyQueryRequest request)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out _))
            {
                return null;
            }

            PruneInactiveLobbiesLocked();
            IEnumerable<ApiLobby> lobbies = _state.Lobbies.Values
                .Where(l => l.AppId == request.AppId && l.Joinable)
                .Select(EnsureLobbyPeerAddressLocked);

            if (request.SlotsAvailable > 0)
            {
                lobbies = lobbies.Where(l => (l.MaxMembers - l.Members.Count) >= request.SlotsAvailable);
            }

            foreach (var filter in request.StringFilters)
            {
                if (string.IsNullOrWhiteSpace(filter.KeyToMatch))
                {
                    continue;
                }

                lobbies = lobbies.Where(l => l.LobbyData.TryGetValue(filter.KeyToMatch, out var value) && CompareString(value, filter.ValueToMatch ?? string.Empty, filter.ComparisonType));
            }

            foreach (var filter in request.NumericalFilters)
            {
                if (string.IsNullOrWhiteSpace(filter.KeyToMatch))
                {
                    continue;
                }

                lobbies = lobbies.Where(l => l.LobbyData.TryGetValue(filter.KeyToMatch, out var value) && int.TryParse(value, out var number) && CompareNumber(number, filter.ValueToMatch, filter.ComparisonType));
            }

            foreach (var filter in request.NearValueFilters)
            {
                if (string.IsNullOrWhiteSpace(filter.KeyToMatch))
                {
                    continue;
                }

                lobbies = lobbies.Where(l => l.LobbyData.TryGetValue(filter.KeyToMatch, out var value) && int.TryParse(value, out _));
            }

            if (request.NearValueFilters.Count > 0)
            {
                lobbies = lobbies.OrderBy(lobby => GetNearValueDistance(lobby, request.NearValueFilters));
            }

            if (!string.IsNullOrWhiteSpace(request.KeyToMatch))
            {
                if (!string.IsNullOrWhiteSpace(request.StringValueToMatch))
                {
                    lobbies = lobbies.Where(l => l.LobbyData.TryGetValue(request.KeyToMatch, out var value) && string.Equals(value, request.StringValueToMatch, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    lobbies = lobbies.Where(l => l.LobbyData.TryGetValue(request.KeyToMatch, out var value) && int.TryParse(value, out var number) && CompareNumber(number, request.ValueToMatch, request.ComparisonType));
                }
            }

            if (request.ResultCount > 0)
            {
                lobbies = lobbies.Take(request.ResultCount);
            }

            return lobbies.Select(CloneLobby).ToList();
        }
    }

    public ApiLobby? CreateLobby(string token, ApiCreateLobbyRequest request)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session) || session == null || !_state.Users.TryGetValue(session.SteamId, out var owner))
            {
                return null;
            }

            RemoveConflictingOwnedLobbiesLocked(session.SteamId, request.AppId, request.LobbyType);
            var lobby = new ApiLobby
            {
                SteamId = ++_nextLobbyId,
                AppId = request.AppId,
                OwnerSteamId = session.SteamId,
                LobbyType = request.LobbyType,
                MaxMembers = request.MaxMembers,
                Joinable = true,
                LobbyData = request.LobbyData ?? new Dictionary<string, string>(),
                Members = new List<ApiLobbyMember> { new() { SteamId = session.SteamId } }
            };
            EnsureLobbyPeerAddressLocked(lobby);

            owner.LobbyId = lobby.SteamId;
            _state.Lobbies[lobby.SteamId] = lobby;
            SaveState();
            EnqueueLobbyMembersEvent(lobby, "lobby_created", session.SteamId);
            return CloneLobby(EnsureLobbyPeerAddressLocked(lobby));
        }
    }

    public ApiLobby? JoinLobby(string token, ulong lobbyId)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session) || session == null || !_state.Lobbies.TryGetValue(lobbyId, out var lobby) || !lobby.Joinable)
            {
                return null;
            }

            if (!IsLobbyAliveLocked(lobby))
            {
                RemoveLobbyLocked(lobbyId);
                return null;
            }

            if (lobby.Members.All(m => m.SteamId != session.SteamId))
            {
                if (lobby.Members.Count >= lobby.MaxMembers)
                {
                    return null;
                }

                lobby.Members.Add(new ApiLobbyMember { SteamId = session.SteamId });
            }

            if (_state.Users.TryGetValue(session.SteamId, out var user))
            {
                user.LobbyId = lobbyId;
            }

            SaveState();
            EnqueueLobbyMembersEvent(lobby, "lobby_joined", session.SteamId);
            return CloneLobby(EnsureLobbyPeerAddressLocked(lobby));
        }
    }

    public bool LeaveLobby(string token, ulong lobbyId)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session) || session == null || !_state.Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                return false;
            }

            lobby.Members.RemoveAll(m => m.SteamId == session.SteamId);
            if (_state.Users.TryGetValue(session.SteamId, out var user))
            {
                user.LobbyId = 0;
            }

            if (lobby.Members.Count == 0)
            {
                _state.Lobbies.Remove(lobbyId);
                SaveState();
                EnqueueEvent(0, new ApiEvent { Type = "lobby_removed", LobbyId = lobbyId, SteamId = session.SteamId });
                return true;
            }

            if (lobby.OwnerSteamId == session.SteamId)
            {
                lobby.OwnerSteamId = lobby.Members[0].SteamId;
            }

            SaveState();
            // Member already removed above, so this reaches the remaining members.
            EnqueueLobbyMembersEvent(lobby, "lobby_left", session.SteamId);
            return true;
        }
    }

    // Relay a lobby chat message to every member (transient, not persisted). Any
    // member may send; the payload is opaque bytes, so this works for any game.
    public bool SendLobbyChatMessage(string token, ulong lobbyId, byte[] body)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session) || session == null || !_state.Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                return false;
            }

            if (lobby.Members.All(m => m.SteamId != session.SteamId))
            {
                return false;
            }

            var payload = Convert.ToBase64String(body ?? Array.Empty<byte>());
            foreach (var member in lobby.Members)
            {
                EnqueueEvent(member.SteamId, new ApiEvent
                {
                    Type = "lobby_chat",
                    LobbyId = lobbyId,
                    SteamId = session.SteamId,
                    PayloadBase64 = payload,
                });
            }

            return true;
        }
    }

    public ApiLobby? GetLobby(string token, ulong lobbyId)
    {
        lock (_sync)
        {
            if (!_state.Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                return null;
            }

            if (!IsLobbyAliveLocked(lobby))
            {
                RemoveLobbyLocked(lobbyId);
                return null;
            }

            return CloneLobby(lobby);
        }
    }

    public bool UpdateLobbyData(string token, ulong lobbyId, string key, string value)
    {
        lock (_sync)
        {
            if (!_state.Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                return false;
            }

            lobby.LobbyData[key] = value;
            EnsureLobbyPeerAddressLocked(lobby);
            SaveState();
            EnqueueLobbyMembersEvent(lobby, "lobby_updated");
            return true;
        }
    }

    public bool DeleteLobbyData(string token, ulong lobbyId, string key)
    {
        lock (_sync)
        {
            if (!_state.Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                return false;
            }

            var removed = lobby.LobbyData.Remove(key);
            if (removed)
            {
                SaveState();
                EnqueueLobbyMembersEvent(lobby, "lobby_updated");
            }

            return removed;
        }
    }

    public bool UpdateLobbySettings(string token, ulong lobbyId, ApiLobbySettingsUpdateRequest request)
    {
        lock (_sync)
        {
            if (!_state.Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                return false;
            }

            if (request.Joinable.HasValue) lobby.Joinable = request.Joinable.Value;
            if (request.LobbyType.HasValue) lobby.LobbyType = request.LobbyType.Value;
            if (request.OwnerSteamId.HasValue) lobby.OwnerSteamId = request.OwnerSteamId.Value;
            if (request.MaxMembers.HasValue) lobby.MaxMembers = request.MaxMembers.Value;
            EnsureLobbyPeerAddressLocked(lobby);
            SaveState();
            EnqueueLobbyMembersEvent(lobby, "lobby_updated");
            return true;
        }
    }

    public bool UpdateLobbyMemberData(string token, ulong lobbyId, string key, string value)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session) || session == null || !_state.Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                return false;
            }

            var member = lobby.Members.FirstOrDefault(m => m.SteamId == session.SteamId);
            if (member == null)
            {
                member = new ApiLobbyMember { SteamId = session.SteamId };
                lobby.Members.Add(member);
            }

            var item = member.Data.FirstOrDefault(d => d.Key == key);
            if (item == null)
            {
                member.Data.Add(new ApiLobbyMetaData { Key = key, Value = value });
            }
            else
            {
                item.Value = value;
            }

            SaveState();
            // Member data is a distinct Steam callback subject. Consumers use the
            // member SteamID to refresh per-player state such as readiness.
            EnqueueLobbyMembersEvent(lobby, "lobby_member_updated", session.SteamId);
            return true;
        }
    }

    public bool UpdateLobbyGameServer(string token, ulong lobbyId, ApiLobbyGameServerUpdateRequest request)
    {
        lock (_sync)
        {
            if (!_state.Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                return false;
            }

            lobby.GameServer = new ApiLobbyGameServer
            {
                SteamId = request.SteamIdGameServer,
                IP = request.IP,
                Port = request.Port
            };
            SaveState();
            // Steam delivers LobbyGameCreated to every current lobby member, not
            // solely to the owner that registered the game server.
            EnqueueLobbyMembersEvent(lobby, "lobby_game_created");
            return true;
        }
    }

    private void PruneInactiveLobbiesLocked()
    {
        var staleLobbyIds = _state.Lobbies.Values
            .Where(lobby => !IsLobbyAliveLocked(lobby))
            .Select(lobby => lobby.SteamId)
            .ToList();

        foreach (var lobbyId in staleLobbyIds)
        {
            RemoveLobbyLocked(lobbyId);
        }
    }

    private bool IsLobbyAliveLocked(ApiLobby lobby)
    {
        if (lobby.Members.Count == 0)
        {
            return lobby.LobbyType == 4 && IsUserOnlineLocked(lobby.OwnerSteamId);
        }

            return IsUserOnlineLocked(lobby.OwnerSteamId) || lobby.Members.Any(member => IsUserOnlineLocked(member.SteamId));
        }

        private ApiLobby EnsureLobbyPeerAddressLocked(ApiLobby lobby)
        {
            // Peer-hosted lobbies (Farm Together etc.) are hosted by the lobby OWNER,
            // so publicIP/internalIP must be the owner's real address, not the central
            // AdvertisedServerIp (that is only meaningful for a Dota-style central game
            // server/SDR relay). Using the advertised server IP here made every lobby
            // claim the server host as its host: on a box that shares the server IP the
            // game treats the lobby as its own and hides it, and remote peers connect to
            // the server instead of the host. Prefer the owner session IP; fall back to
            // the advertised IP only when the owner address is unknown.
            var ownerIp = _state.WebSessions.Values
                .Where(session => session.SteamId == lobby.OwnerSteamId && !string.IsNullOrWhiteSpace(session.RemoteIp))
                .OrderByDescending(session => session.LastSeenUtc)
                .Select(session => session.RemoteIp)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(ownerIp) && TryGetConfiguredAdvertisedServerIp(out var configuredIp))
            {
                ownerIp = ToIPv4String(configuredIp);
            }

            if (string.IsNullOrWhiteSpace(ownerIp))
            {
                return lobby;
            }

            // Some peer-hosted games expect the host address as lobby metadata.
            // Steam does not reserve these keys, but the legacy LAN path filled
            // them from the remote endpoint when a lobby arrived over the wire.
            // Preserve explicit game-provided values and only derive missing ones.
            if (!lobby.LobbyData.TryGetValue("publicIP", out var publicIp) || string.IsNullOrWhiteSpace(publicIp))
            {
                lobby.LobbyData["publicIP"] = ownerIp;
            }

            if (!lobby.LobbyData.TryGetValue("internalIP", out var internalIp) || string.IsNullOrWhiteSpace(internalIp))
            {
                lobby.LobbyData["internalIP"] = ownerIp;
            }

            return lobby;
        }

    private void RemoveLobbyLocked(ulong lobbyId)
    {
        if (!_state.Lobbies.Remove(lobbyId))
        {
            return;
        }

        foreach (var user in _state.Users.Values.Where(user => user.LobbyId == lobbyId))
        {
            user.LobbyId = 0;
        }

        EnqueueEvent(0, new ApiEvent { Type = "lobby_removed", LobbyId = lobbyId });
        SaveState();
    }

    private void RemoveConflictingOwnedLobbiesLocked(ulong ownerSteamId, uint appId, int lobbyType)
    {
        if (!IsRegularLobbyType(lobbyType))
        {
            return;
        }

        var conflictingLobbyIds = _state.Lobbies.Values
            .Where(lobby => lobby.AppId == appId && lobby.OwnerSteamId == ownerSteamId && IsRegularLobbyType(lobby.LobbyType))
            .Select(lobby => lobby.SteamId)
            .ToList();

        foreach (var lobbyId in conflictingLobbyIds)
        {
            RemoveLobbyLocked(lobbyId);
        }
    }

    private static bool IsRegularLobbyType(int lobbyType)
    {
        return lobbyType is >= 0 and <= 2;
    }

    private static int GetNearValueDistance(ApiLobby lobby, IReadOnlyList<ApiLobbyNearValueFilter> filters)
    {
        var distance = 0;
        foreach (var filter in filters)
        {
            if (string.IsNullOrWhiteSpace(filter.KeyToMatch) ||
                !lobby.LobbyData.TryGetValue(filter.KeyToMatch, out var value) ||
                !int.TryParse(value, out var parsed))
            {
                distance += int.MaxValue / Math.Max(1, filters.Count);
                continue;
            }

            distance += Math.Abs(parsed - filter.ValueToBeCloseTo);
        }

        return distance;
    }
}
