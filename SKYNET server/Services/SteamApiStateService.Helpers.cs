using System.Net;
using System.Text.Json;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    public static string? GetBearerToken(HttpRequest request)
    {
        var header = request.Headers.Authorization.ToString();
        return header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? header["Bearer ".Length..].Trim()
            : null;
    }

    private bool TryGetSession(string token, out ApiSession? session)
    {
        session = null;
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        return _sessions.TryGetValue(token, out session);
    }

    private bool TryGetUserByToken(string token, out SkyNetUserDto user)
    {
        user = new SkyNetUserDto();
        return TryGetSession(token, out var session) && _state.Users.TryGetValue(session!.SteamId, out user!);
    }

    private SkyNetUserDto EnsureUser(ulong steamId, uint accountId, uint appId, string personaName)
    {
        if (!_state.Users.TryGetValue(steamId, out var user))
        {
            user = new SkyNetUserDto
            {
                SteamId = steamId,
                AccountId = accountId != 0 ? accountId : SteamIdToAccountId(steamId),
                AppId = appId,
                PersonaName = string.IsNullOrWhiteSpace(personaName) ? $"User{SteamIdToAccountId(steamId)}" : personaName,
                HasFriend = true,
                PersonaState = 1,
                PlayerLevel = 1
            };
            _state.Users[steamId] = user;
        }
        else
        {
            user.AppId = appId != 0 ? appId : user.AppId;
            if (!string.IsNullOrWhiteSpace(personaName))
            {
                user.PersonaName = personaName;
            }
        }

        if (!_state.Stats.ContainsKey(steamId))
        {
            _state.Stats[steamId] = new SkyNetStatsEnvelopeDto
            {
                SteamId = steamId,
                CurrentPlayers = 1,
                Stats = new List<SkyNetStatDto> { new() { Name = "wins", Data = 0 }, new() { Name = "kills", Data = 0 } },
                Achievements = new List<SkyNetAchievementDto> { new() { Name = "first_launch", Earned = true, Date = DateTime.UtcNow } }
            };
        }

        EnsureSeedFriends(steamId, user.AppId);
        SaveState();
        return user;
    }

    private void EnsureSeedFriends(ulong selfSteamId, uint appId)
    {
        if (_state.FriendLinks.ContainsKey(selfSteamId))
        {
            return;
        }

        var friends = _uiMockService.GetFriends();
        var links = new HashSet<ulong>();
        for (var index = 0; index < friends.Count; index++)
        {
            var friendSteamId = ToSteamId((uint)(2000 + index + 1));
            if (!_state.Users.ContainsKey(friendSteamId))
            {
                _state.Users[friendSteamId] = new SkyNetUserDto
                {
                    SteamId = friendSteamId,
                    AccountId = (uint)(2000 + index + 1),
                    AppId = appId,
                    PersonaName = friends[index].DisplayName,
                    HasFriend = true,
                    PersonaState = 1,
                    PlayerLevel = 1,
                    RichPresence = string.IsNullOrWhiteSpace(friends[index].CurrentGame)
                        ? new Dictionary<string, string>()
                        : new Dictionary<string, string> { ["status"] = friends[index].CurrentGame }
                };
            }

            links.Add(friendSteamId);
        }

        _state.FriendLinks[selfSteamId] = links;
    }

    private void EnqueueFriendEvents(ulong steamId, string type, int flags)
    {
        if (!_state.Users.TryGetValue(steamId, out var user))
        {
            return;
        }

        EnqueueEvent(steamId, new SkyNetEventDto
        {
            Type = type,
            SteamId = steamId,
            AccountId = user.AccountId,
            PersonaName = user.PersonaName,
            AppId = user.AppId,
            ChangeFlags = flags,
            RichPresence = new Dictionary<string, string>(user.RichPresence)
        });
    }

    private void EnqueueLobbyEvent(SkyNetLobbyDto lobby, string type, ulong recipientSteamId)
    {
        EnqueueEvent(recipientSteamId, new SkyNetEventDto
        {
            Type = type,
            LobbyId = lobby.SteamId,
            Lobby = CloneLobby(lobby)
        });
    }

    private void EnqueueEvent(ulong recipientSteamId, SkyNetEventDto evt)
    {
        _events.Add(new ApiQueuedEvent
        {
            Sequence = _nextEventSequence++,
            RecipientSteamId = recipientSteamId,
            Event = evt
        });
    }

    private void LoadState()
    {
        if (File.Exists(_statePath))
        {
            var json = File.ReadAllText(_statePath);
            _state = JsonSerializer.Deserialize<ApiState>(json, _jsonOptions) ?? new ApiState();
        }
        else
        {
            _state = new ApiState();
        }
    }

    private void SaveState()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_statePath)!);
        File.WriteAllText(_statePath, JsonSerializer.Serialize(_state, _jsonOptions));
    }

    private static SkyNetUserDto CloneUser(SkyNetUserDto user) => new()
    {
        AccountId = user.AccountId,
        SteamId = user.SteamId,
        PersonaName = user.PersonaName,
        AppId = user.AppId,
        LobbyId = user.LobbyId,
        HasFriend = user.HasFriend,
        PersonaState = user.PersonaState,
        PlayerLevel = user.PlayerLevel,
        RichPresence = new Dictionary<string, string>(user.RichPresence)
    };

    private static SkyNetStatsEnvelopeDto CloneStats(SkyNetStatsEnvelopeDto stats) => new()
    {
        SteamId = stats.SteamId,
        CurrentPlayers = stats.CurrentPlayers,
        Stats = stats.Stats.Select(s => new SkyNetStatDto { Name = s.Name, Data = s.Data }).ToList(),
        Achievements = stats.Achievements.Select(a => new SkyNetAchievementDto
        {
            Name = a.Name,
            Earned = a.Earned,
            Date = a.Date,
            Progress = a.Progress,
            MaxProgress = a.MaxProgress
        }).ToList()
    };

    private static SkyNetLobbyDto CloneLobby(SkyNetLobbyDto lobby) => new()
    {
        SteamId = lobby.SteamId,
        AppId = lobby.AppId,
        OwnerSteamId = lobby.OwnerSteamId,
        LobbyType = lobby.LobbyType,
        MaxMembers = lobby.MaxMembers,
        Joinable = lobby.Joinable,
        LobbyData = new Dictionary<string, string>(lobby.LobbyData),
        Members = lobby.Members.Select(m => new SkyNetLobbyMemberDto
        {
            SteamId = m.SteamId,
            Data = m.Data.Select(d => new SkyNetLobbyMetaDataDto { Key = d.Key, Value = d.Value }).ToList()
        }).ToList(),
        GameServer = lobby.GameServer == null ? null : new SkyNetLobbyGameServerDto
        {
            SteamId = lobby.GameServer.SteamId,
            IP = lobby.GameServer.IP,
            Port = lobby.GameServer.Port
        }
    };

    private static SkyNetEventDto CloneEvent(SkyNetEventDto evt) => new()
    {
        Type = evt.Type,
        SteamId = evt.SteamId,
        AccountId = evt.AccountId,
        PersonaName = evt.PersonaName,
        AppId = evt.AppId,
        LobbyId = evt.LobbyId,
        ChangeFlags = evt.ChangeFlags,
        RichPresence = new Dictionary<string, string>(evt.RichPresence),
        StatName = evt.StatName,
        StatValue = evt.StatValue,
        AchievementName = evt.AchievementName,
        AchievementEarned = evt.AchievementEarned,
        AchievementProgress = evt.AchievementProgress,
        AchievementMaxProgress = evt.AchievementMaxProgress,
        Lobby = evt.Lobby == null ? null : CloneLobby(evt.Lobby),
        PayloadBase64 = evt.PayloadBase64,
        MessageType = evt.MessageType,
        Channel = evt.Channel,
        RemoteSteamId = evt.RemoteSteamId
    };

    private static SkyNetRemoteStorageFileDto CloneFile(SkyNetRemoteStorageFileDto file) => new()
    {
        FileName = file.FileName,
        ContentBase64 = file.ContentBase64,
        Size = file.Size,
        Timestamp = file.Timestamp
    };

    private static bool CompareNumber(int left, int right, int comparisonType) => comparisonType switch
    {
        -2 => left < right,
        -1 => left <= right,
        0 => left == right,
        1 => left >= right,
        2 => left > right,
        _ => left == right
    };

    private static ulong ToSteamId(uint accountId) => 76561197960265728UL + accountId;
    private static uint SteamIdToAccountId(ulong steamId) => (uint)(steamId - 76561197960265728UL);
    private static uint ToUnixTime(DateTime value) => (uint)new DateTimeOffset(value).ToUnixTimeSeconds();

    private static uint ToUInt32(IPAddress ip)
    {
        var bytes = ip.GetAddressBytes();
        Array.Reverse(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }
}
