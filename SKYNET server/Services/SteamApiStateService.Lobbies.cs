using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    public List<SkyNetLobbyDto>? QueryLobbies(string token, SkyNetLobbyQueryRequestDto request)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out _))
            {
                return null;
            }

            IEnumerable<SkyNetLobbyDto> lobbies = _state.Lobbies.Values.Where(l => l.AppId == request.AppId);

            if (request.SlotsAvailable > 0)
            {
                lobbies = lobbies.Where(l => (l.MaxMembers - l.Members.Count) >= request.SlotsAvailable);
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

            return lobbies.Select(CloneLobby).ToList();
        }
    }

    public SkyNetLobbyDto? CreateLobby(string token, SkyNetCreateLobbyRequestDto request)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session) || !_state.Users.TryGetValue(session.SteamId, out var owner))
            {
                return null;
            }

            var lobby = new SkyNetLobbyDto
            {
                SteamId = ++_nextLobbyId,
                AppId = request.AppId,
                OwnerSteamId = session.SteamId,
                LobbyType = request.LobbyType,
                MaxMembers = request.MaxMembers,
                Joinable = true,
                LobbyData = request.LobbyData ?? new Dictionary<string, string>(),
                Members = new List<SkyNetLobbyMemberDto> { new() { SteamId = session.SteamId } }
            };

            owner.LobbyId = lobby.SteamId;
            _state.Lobbies[lobby.SteamId] = lobby;
            SaveState();
            EnqueueLobbyEvent(lobby, "lobby_created", 0);
            return CloneLobby(lobby);
        }
    }

    public SkyNetLobbyDto? JoinLobby(string token, ulong lobbyId)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session) || !_state.Lobbies.TryGetValue(lobbyId, out var lobby) || !lobby.Joinable)
            {
                return null;
            }

            if (lobby.Members.All(m => m.SteamId != session.SteamId))
            {
                if (lobby.Members.Count >= lobby.MaxMembers)
                {
                    return null;
                }

                lobby.Members.Add(new SkyNetLobbyMemberDto { SteamId = session.SteamId });
            }

            if (_state.Users.TryGetValue(session.SteamId, out var user))
            {
                user.LobbyId = lobbyId;
            }

            SaveState();
            EnqueueLobbyEvent(lobby, "lobby_joined", 0);
            return CloneLobby(lobby);
        }
    }

    public bool LeaveLobby(string token, ulong lobbyId)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session) || !_state.Lobbies.TryGetValue(lobbyId, out var lobby))
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
                EnqueueEvent(0, new SkyNetEventDto { Type = "lobby_removed", LobbyId = lobbyId, SteamId = session.SteamId });
                return true;
            }

            if (lobby.OwnerSteamId == session.SteamId)
            {
                lobby.OwnerSteamId = lobby.Members[0].SteamId;
            }

            SaveState();
            EnqueueEvent(0, new SkyNetEventDto { Type = "lobby_left", LobbyId = lobbyId, SteamId = session.SteamId });
            return true;
        }
    }

    public SkyNetLobbyDto? GetLobby(string token, ulong lobbyId)
    {
        lock (_sync)
        {
            return _state.Lobbies.TryGetValue(lobbyId, out var lobby) ? CloneLobby(lobby) : null;
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
            SaveState();
            EnqueueLobbyEvent(lobby, "lobby_updated", 0);
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
                EnqueueLobbyEvent(lobby, "lobby_updated", 0);
            }

            return removed;
        }
    }

    public bool UpdateLobbySettings(string token, ulong lobbyId, SkyNetLobbySettingsUpdateRequestDto request)
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
            SaveState();
            EnqueueLobbyEvent(lobby, "lobby_updated", 0);
            return true;
        }
    }

    public bool UpdateLobbyMemberData(string token, ulong lobbyId, string key, string value)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session) || !_state.Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                return false;
            }

            var member = lobby.Members.FirstOrDefault(m => m.SteamId == session.SteamId);
            if (member == null)
            {
                member = new SkyNetLobbyMemberDto { SteamId = session.SteamId };
                lobby.Members.Add(member);
            }

            var item = member.Data.FirstOrDefault(d => d.Key == key);
            if (item == null)
            {
                member.Data.Add(new SkyNetLobbyMetaDataDto { Key = key, Value = value });
            }
            else
            {
                item.Value = value;
            }

            SaveState();
            EnqueueLobbyEvent(lobby, "lobby_updated", 0);
            return true;
        }
    }

    public bool UpdateLobbyGameServer(string token, ulong lobbyId, SkyNetLobbyGameServerUpdateRequestDto request)
    {
        lock (_sync)
        {
            if (!_state.Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                return false;
            }

            lobby.GameServer = new SkyNetLobbyGameServerDto
            {
                SteamId = request.SteamIdGameServer,
                IP = request.IP,
                Port = request.Port
            };
            SaveState();
            EnqueueLobbyEvent(lobby, "lobby_updated", 0);
            return true;
        }
    }
}
