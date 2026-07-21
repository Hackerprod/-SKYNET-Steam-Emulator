using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
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

    // Identity of the machine that made the request. On a LAN / ZeroTier network
    // each PC has a distinct address, so this is what ties a game client to the
    // web session the same PC logged in with.
    public static string? GetClientIp(HttpRequest request)
    {
        return NormalizeIp(request.HttpContext.Connection.RemoteIpAddress);
    }

    public static string? NormalizeIp(IPAddress? address)
    {
        if (address == null)
        {
            return null;
        }

        if (address.IsIPv4MappedToIPv6)
        {
            address = address.MapToIPv4();
        }

        // Treat every loopback form (127.0.0.1, ::1) as the same host so the web
        // browser and the game on a single PC always match.
        return IPAddress.IsLoopback(address) ? "127.0.0.1" : address.ToString();
    }

    private bool TryGetSession(string token, out ApiSession? session)
    {
        session = null;
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        if (!_sessions.TryGetValue(token, out session))
        {
            if (!_state.WebSessions.TryGetValue(token, out session))
            {
                return false;
            }

            _sessions[token] = session;
        }

        var now = DateTime.UtcNow;
        if (IsSessionExpired(session, now))
        {
            RemoveSessionLocked(token);
            SaveState();
            return false;
        }

        session.LastSeenUtc = now;
        return true;
    }

    private bool TryGetUserByToken(string token, out ApiUser user)
    {
        user = new ApiUser();
        return TryGetSession(token, out var session) && _state.Users.TryGetValue(session!.SteamId, out user!);
    }

    private ApiUser EnsureUser(ulong steamId, uint accountId, uint appId, string personaName)
    {
        if (!_state.Users.TryGetValue(steamId, out var user))
        {
            user = new ApiUser
            {
                SteamId = steamId,
                AccountId = accountId != 0 ? accountId : SteamIdToAccountId(steamId),
                AppId = appId,
                PersonaName = string.IsNullOrWhiteSpace(personaName) ? $"User{SteamIdToAccountId(steamId)}" : personaName,
                HasFriend = false,
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
            _state.Stats[steamId] = new ApiStatsEnvelope
            {
                SteamId = steamId,
                CurrentPlayers = 1,
                Stats = new List<ApiStat> { new() { Name = "wins", Data = 0 }, new() { Name = "kills", Data = 0 } },
                Achievements = new List<ApiAchievement> { new() { Name = "first_launch", Earned = true, Date = DateTime.UtcNow } }
            };
        }

        if (!_state.FriendLinks.ContainsKey(steamId))
        {
            _state.FriendLinks[steamId] = new HashSet<ulong>();
        }

        SaveState();
        return user;
    }

    private DotaStatsAccountIdentity? ResolveDotaStatsIdentity(uint accountId)
    {
        lock (_sync)
        {
            var steamId = ToSteamId(accountId);
            if (_state.Users.TryGetValue(steamId, out var user))
            {
                return new DotaStatsAccountIdentity(user.AccountId, user.SteamId, user.PersonaName);
            }

            var byAccount = _state.Users.Values.FirstOrDefault(user => user.AccountId == accountId);
            return byAccount == null
                ? new DotaStatsAccountIdentity(accountId, steamId, $"User{accountId}")
                : new DotaStatsAccountIdentity(byAccount.AccountId, byAccount.SteamId, byAccount.PersonaName);
        }
    }

    private void EnqueueFriendEvents(ulong steamId, string type, int flags)
    {
        if (!_state.Users.TryGetValue(steamId, out var user))
        {
            return;
        }

        var recipients = new HashSet<ulong> { steamId };
        if (_state.FriendLinks.TryGetValue(steamId, out var linkedUsers))
        {
            foreach (var linkedUser in linkedUsers)
            {
                recipients.Add(linkedUser);
            }
        }

        foreach (var request in _state.FriendRequests.Where(IsPending))
        {
            if (request.FromSteamId == steamId)
            {
                recipients.Add(request.ToSteamId);
            }
            else if (request.ToSteamId == steamId)
            {
                recipients.Add(request.FromSteamId);
            }
        }

        // Carry the live presence in every friend event so clients never keep a
        // stale "online / in game" for someone who left. Offline users report no
        // game, lobby or rich presence.
        var online = IsUserOnlineLocked(steamId) ? 1 : 0;
        foreach (var recipient in recipients)
        {
            var relationship = GetRelationshipLocked(recipient, steamId);
            EnqueueEvent(recipient, new ApiEvent
            {
                Type = type,
                SteamId = steamId,
                AccountId = user.AccountId,
                PersonaName = user.PersonaName,
                AppId = online == 1 ? user.AppId : 0,
                LobbyId = online == 1 ? user.LobbyId : 0,
                PersonaState = online,
                ChangeFlags = flags,
                RichPresence = online == 1
                    ? new Dictionary<string, string>(user.RichPresence)
                    : new Dictionary<string, string>(),
                FriendRelationship = relationship
            });
        }
    }

    private void EnqueueLobbyEvent(ApiLobby lobby, string type, ulong recipientSteamId)
    {
        EnqueueEvent(recipientSteamId, new ApiEvent
        {
            Type = type,
            LobbyId = lobby.SteamId,
            Lobby = CloneLobby(lobby)
        });
    }

    // Deliver a lobby event to its members only (not a global broadcast), so a
    // client only ever hears about lobbies it belongs to. changedSteamId is the
    // member the event is about (join/leave), enabling LobbyChatUpdate on clients.
    private void EnqueueLobbyMembersEvent(ApiLobby lobby, string type, ulong changedSteamId = 0)
    {
        var clone = CloneLobby(lobby);
        foreach (var member in lobby.Members)
        {
            EnqueueEvent(member.SteamId, new ApiEvent
            {
                Type = type,
                LobbyId = lobby.SteamId,
                SteamId = changedSteamId,
                Lobby = clone,
            });
        }
    }

    private void EnqueueEvent(ulong recipientSteamId, ApiEvent evt)
    {
        EnqueueEvent(recipientSteamId, evt, string.Empty, string.Empty);
    }

    private void EnqueueEvent(
        ulong recipientSteamId,
        ApiEvent evt,
        string recipientProcessRole,
        string recipientClientInstanceId)
    {
        _events.Add(new ApiQueuedEvent
        {
            Sequence = _nextEventSequence++,
            RecipientSteamId = recipientSteamId,
            RecipientProcessRole = recipientProcessRole ?? string.Empty,
            RecipientClientInstanceId = recipientClientInstanceId ?? string.Empty,
            Event = evt
        });

        Monitor.PulseAll(_sync);
        TrimQueuedEventsLocked();
    }

    private void EnqueueGcMessageEvent(ulong recipientSteamId, ApiGCMessage message)
    {
        lock (_sync)
        {
            EnqueueEvent(recipientSteamId, new ApiEvent
            {
                Type = "gc_message",
                AppId = message.AppId,
                MessageType = message.MessageType,
                PayloadBase64 = message.PayloadBase64,
                TargetJobId = message.TargetJobId,
                Protobuf = message.Protobuf
            });
        }

        _gameCoordinatorTrace.Record("push", message.AppId, recipientSteamId, message.MessageType,
            GameCoordinatorTraceService.EstimatePayloadSize(message.PayloadBase64));
    }

    private void EnqueueGameServerChangeRequestedEvent(ulong recipientSteamId, string server, string password)
    {
        lock (_sync)
        {
            EnqueueEvent(recipientSteamId, new ApiEvent
            {
                Type = "game_server_change_requested",
                SteamId = recipientSteamId,
                Server = server ?? string.Empty,
                Password = password ?? string.Empty
            }, "client", string.Empty);
        }
    }

    private void TrimQueuedEventsLocked()
    {
        const int maxEvents = 4096;
        if (_events.Count <= maxEvents)
        {
            return;
        }

        _events.RemoveRange(0, _events.Count - maxEvents);
    }

    private void ExpireStaleSessionsLocked()
    {
        var now = DateTime.UtcNow;
        var expired = _sessions
            .Where(pair => IsSessionExpired(pair.Value, now))
            .Select(pair => pair.Key)
            .ToList();

        foreach (var token in expired)
        {
            RemoveSessionLocked(token);
        }

        var stalePersistent = _state.WebSessions
            .Where(pair => IsSessionExpired(pair.Value, now))
            .Select(pair => pair.Key)
            .ToList();

        foreach (var token in stalePersistent)
        {
            RemoveSessionLocked(token);
        }

        var activeWebSessionCleared = false;
        if (_state.ActiveWebSteamId != 0 && !HasActiveWebSessionLocked(_state.ActiveWebSteamId))
        {
            _state.ActiveWebSteamId = 0;
            activeWebSessionCleared = true;
        }

        if (expired.Count > 0 || stalePersistent.Count > 0 || activeWebSessionCleared)
        {
            SaveState();
        }
    }

    private bool IsSessionExpired(ApiSession session, DateTime now)
    {
        if (session.WebSession || session.Persistent)
        {
            return session.ExpiresAtUtc != DateTime.MinValue && session.ExpiresAtUtc <= now;
        }

        return session.LastSeenUtc < now - _sessionTimeout;
    }

    private void RemoveSessionLocked(string token)
    {
        _sessions.Remove(token);
        _state.WebSessions.Remove(token);
    }

    private bool HasActiveWebSessionLocked(ulong steamId)
    {
        var now = DateTime.UtcNow;
        foreach (var pair in _state.WebSessions.ToList())
        {
            if (IsSessionExpired(pair.Value, now))
            {
                RemoveSessionLocked(pair.Key);
                continue;
            }

            _sessions[pair.Key] = pair.Value;
        }

        return _sessions.Values.Any(session =>
            session.WebSession &&
            session.SteamId == steamId &&
            !IsSessionExpired(session, now));
    }

    private void NormalizeState()
    {
        _state.Users ??= new Dictionary<ulong, ApiUser>();
        _state.FriendLinks ??= new Dictionary<ulong, HashSet<ulong>>();
        _state.FriendRequests ??= new List<ApiFriendRequest>();
        _state.Avatars ??= new Dictionary<ulong, string>();
        _state.Stats ??= new Dictionary<ulong, ApiStatsEnvelope>();
        _state.Lobbies ??= new Dictionary<ulong, ApiLobby>();
        _state.Files ??= new Dictionary<string, ApiRemoteStorageFile>(StringComparer.OrdinalIgnoreCase);
        _state.Files = new Dictionary<string, ApiRemoteStorageFile>(_state.Files, StringComparer.OrdinalIgnoreCase);
        _state.FileShares ??= new Dictionary<ulong, ApiRemoteStorageShareRecord>();
        _state.GameServers ??= new Dictionary<ulong, ApiGameServer>();
        // State files written before dedicated support used the host IP as key.
        // Several dedicated processes share an IP, so current entries are keyed
        // by their game-server SteamID instead.
        if (_state.GameServers.Count > 0)
        {
            var normalizedServers = new Dictionary<ulong, ApiGameServer>();
            foreach (var pair in _state.GameServers)
            {
                var server = pair.Value ?? new ApiGameServer();
                var key = server.SteamId != 0 ? server.SteamId : pair.Key;
                server.SteamId = key;
                normalizedServers[key] = server;
            }
            _state.GameServers = normalizedServers;
        }
        _state.WebAccounts ??= new Dictionary<string, ApiWebAccount>(StringComparer.OrdinalIgnoreCase);
        _state.WebSessions = new Dictionary<string, ApiSession>(_state.WebSessions ?? new Dictionary<string, ApiSession>(), StringComparer.Ordinal);
        _state.DotaItems ??= new Dictionary<uint, ApiDotaItem>();
        _state.DotaEquipment ??= new Dictionary<ulong, List<ApiDotaEquipment>>();
        _state.DotaMatches ??= new Dictionary<ulong, ApiDotaMatch>();
        _state.DotaHeroIds ??= new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
        _state.DotaHeroSlots = NormalizeDotaHeroSlots(_state.DotaHeroSlots);
        _state.DotaCosmetics ??= new ApiDotaCosmeticSettings();
        EnsureDotaHeroSlotsLoadedLocked();
        NormalizeDotaItemSlotsLocked();
        var now = DateTime.UtcNow;
        foreach (var pair in _state.WebSessions.ToList())
        {
            pair.Value.WebSession = true;
            pair.Value.Persistent = true;
            pair.Value.ProcessRole = "web";
            if (pair.Value.ExpiresAtUtc == DateTime.MinValue)
            {
                pair.Value.ExpiresAtUtc = now.AddDays(30);
            }

            if (!_state.Users.ContainsKey(pair.Value.SteamId) || IsSessionExpired(pair.Value, now))
            {
                _state.WebSessions.Remove(pair.Key);
                continue;
            }

            _sessions[pair.Key] = pair.Value;
        }

        if (_state.ActiveWebSteamId != 0 && (!_state.Users.ContainsKey(_state.ActiveWebSteamId) || !HasActiveWebSessionLocked(_state.ActiveWebSteamId)))
        {
            _state.ActiveWebSteamId = 0;
        }

        foreach (var user in _state.Users.Values)
        {
            user.FriendRelationship = FriendRelationshipNone;
            _state.FriendLinks.TryAdd(user.SteamId, new HashSet<ulong>());
        }

        foreach (var request in _state.FriendRequests)
        {
            if (string.IsNullOrWhiteSpace(request.Id))
            {
                request.Id = Guid.NewGuid().ToString("N");
            }

            if (string.IsNullOrWhiteSpace(request.Status))
            {
                request.Status = "pending";
            }
        }

        foreach (var item in _state.DotaItems.Values)
        {
            item.HeroNames ??= new List<string>();
            item.HeroIds ??= new List<uint>();
        }

        foreach (var pair in _state.DotaEquipment.ToList())
        {
            var list = pair.Value ?? new List<ApiDotaEquipment>();
            foreach (var item in list)
            {
                if (item.ItemId == 0)
                {
                    item.ItemId = BuildDotaItemInstanceId(item.SteamId, item.DefIndex);
                }
            }

            _state.DotaEquipment[pair.Key] = NormalizeDotaEquipmentList(list);
        }

        foreach (var match in _state.DotaMatches.Values)
        {
            match.Players ??= new List<ApiDotaMatchPlayer>();
            foreach (var player in match.Players)
            {
                player.Equipment ??= new List<ApiDotaEquipment>();
            }
        }
    }

    // Persist state on demand (e.g. on graceful shutdown).
    public void FlushState()
    {
        FlushStateToDatabase();
    }

    // Durable state now lives in steam.db/dota.db. Mutation sites still call SaveState();
    // it just marks the state dirty and wakes the background flusher, which
    // coalesces bursts and writes to SQLite off the request path (see
    // SteamApiStateService.Persistence).
    private void SaveState()
    {
        RequestStateFlush();
    }

    private static ApiUser CloneUser(ApiUser user) => new()
    {
        AccountId = user.AccountId,
        SteamId = user.SteamId,
        PersonaName = user.PersonaName,
        AppId = user.AppId,
        LobbyId = user.LobbyId,
        HasFriend = user.HasFriend,
        FriendRelationship = user.FriendRelationship,
        PersonaState = user.PersonaState,
        PlayerLevel = user.PlayerLevel,
        GameState = user.GameState,
        HeroId = user.HeroId,
        RichPresence = new Dictionary<string, string>(user.RichPresence)
    };

    private static ApiStatsEnvelope CloneStats(ApiStatsEnvelope stats) => new()
    {
        SteamId = stats.SteamId,
        CurrentPlayers = stats.CurrentPlayers,
        Stats = stats.Stats.Select(s => new ApiStat { Name = s.Name, Data = s.Data }).ToList(),
        Achievements = stats.Achievements.Select(a => new ApiAchievement
        {
            Name = a.Name,
            Earned = a.Earned,
            Date = a.Date,
            Progress = a.Progress,
            MaxProgress = a.MaxProgress
        }).ToList()
    };

    private static ApiLobby CloneLobby(ApiLobby lobby) => new()
    {
        SteamId = lobby.SteamId,
        AppId = lobby.AppId,
        OwnerSteamId = lobby.OwnerSteamId,
        LobbyType = lobby.LobbyType,
        MaxMembers = lobby.MaxMembers,
        Joinable = lobby.Joinable,
        LobbyData = new Dictionary<string, string>(lobby.LobbyData),
        Members = lobby.Members.Select(m => new ApiLobbyMember
        {
            SteamId = m.SteamId,
            Data = m.Data.Select(d => new ApiLobbyMetaData { Key = d.Key, Value = d.Value }).ToList()
        }).ToList(),
        GameServer = lobby.GameServer == null ? null : new ApiLobbyGameServer
        {
            SteamId = lobby.GameServer.SteamId,
            IP = lobby.GameServer.IP,
            Port = lobby.GameServer.Port
        }
    };

    private static ApiEvent CloneEvent(ApiEvent evt) => new()
    {
        Type = evt.Type,
        SteamId = evt.SteamId,
        AccountId = evt.AccountId,
        PersonaName = evt.PersonaName,
        AppId = evt.AppId,
        LobbyId = evt.LobbyId,
        PersonaState = evt.PersonaState,
        ChangeFlags = evt.ChangeFlags,
        RichPresence = new Dictionary<string, string>(evt.RichPresence ?? new Dictionary<string, string>()),
        StatName = evt.StatName,
        StatValue = evt.StatValue,
        AchievementName = evt.AchievementName,
        AchievementEarned = evt.AchievementEarned,
        AchievementProgress = evt.AchievementProgress,
        AchievementMaxProgress = evt.AchievementMaxProgress,
        Lobby = evt.Lobby == null ? null : CloneLobby(evt.Lobby),
        PayloadBase64 = evt.PayloadBase64,
        MessageType = evt.MessageType,
        TargetJobId = evt.TargetJobId,
        Protobuf = evt.Protobuf,
        Channel = evt.Channel,
        RemoteSteamId = evt.RemoteSteamId,
        FriendRelationship = evt.FriendRelationship,
        RequestId = evt.RequestId,
        Server = evt.Server,
        Password = evt.Password
    };

    private static ApiRemoteStorageFile CloneFile(ApiRemoteStorageFile file) => new()
    {
        OwnerSteamId = file.OwnerSteamId,
        AppId = file.AppId,
        FileName = file.FileName,
        ContentBase64 = file.ContentBase64,
        Size = file.Size,
        Timestamp = file.Timestamp,
        Sha256 = file.Sha256,
        SyncPlatforms = file.SyncPlatforms,
        Version = file.Version,
        Persisted = file.Persisted,
        DeletedAt = file.DeletedAt
    };

    private static ApiDotaItem CloneDotaItem(ApiDotaItem item) => new()
    {
        DefIndex = item.DefIndex,
        Name = item.Name,
        Prefab = item.Prefab,
        Slot = item.Slot,
        Quality = item.Quality,
        QualityId = item.QualityId,
        Rarity = item.Rarity,
        RarityId = item.RarityId,
        ImageInventory = item.ImageInventory,
        IsDefault = item.IsDefault,
        IsTool = item.IsTool,
        IsBundle = item.IsBundle,
        HeroNames = item.HeroNames.ToList(),
        HeroIds = item.HeroIds.ToList()
    };

    private static ApiDotaEquipment CloneDotaEquipment(ApiDotaEquipment item) => new()
    {
        SteamId = item.SteamId,
        HeroId = item.HeroId,
        HeroName = item.HeroName,
        Slot = item.Slot,
        SlotId = item.SlotId,
        DefIndex = item.DefIndex,
        ItemId = item.ItemId,
        Style = item.Style,
        UpdatedAt = item.UpdatedAt
    };

    private static ApiDotaMatch CloneDotaMatch(ApiDotaMatch match) => new()
    {
        LobbyId = match.LobbyId,
        MatchId = match.MatchId,
        ServerSteamId = match.ServerSteamId,
        Connect = match.Connect,
        State = match.State,
        GameState = match.GameState,
        GameStartTime = match.GameStartTime,
        Dedicated = match.Dedicated,
        UpdatedAt = match.UpdatedAt,
        Players = match.Players.Select(player => new ApiDotaMatchPlayer
        {
            SteamId = player.SteamId,
            AccountId = player.AccountId,
            PersonaName = player.PersonaName,
            Team = player.Team,
            Slot = player.Slot,
            CoachTeam = player.CoachTeam,
            HeroId = player.HeroId,
            Equipment = player.Equipment.Select(CloneDotaEquipment).ToList()
        }).ToList()
    };

    private ApiDotaCosmeticSummary BuildDotaCosmeticSummaryLocked() => new()
    {
        ItemCount = _state.DotaItems.Count,
        HeroCount = _state.DotaHeroIds.Count,
        EquippedCount = _state.DotaEquipment.Values.Sum(items => items.Count),
        DotaPath = _state.DotaCosmetics.DotaPath,
        LastImportAt = _state.DotaCosmetics.LastImportAt,
        LastImportStatus = _state.DotaCosmetics.LastImportStatus
    };

    private static Dictionary<string, Dictionary<string, uint>> NormalizeDotaHeroSlots(
        Dictionary<string, Dictionary<string, uint>>? source)
    {
        var normalized = new Dictionary<string, Dictionary<string, uint>>(StringComparer.OrdinalIgnoreCase);
        if (source == null)
        {
            return normalized;
        }

        foreach (var hero in source)
        {
            if (string.IsNullOrWhiteSpace(hero.Key) || hero.Value == null)
            {
                continue;
            }

            var slots = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
            foreach (var slot in hero.Value)
            {
                if (!string.IsNullOrWhiteSpace(slot.Key))
                {
                    slots[slot.Key.Trim().ToLowerInvariant()] = slot.Value;
                }
            }

            normalized[hero.Key.Trim()] = slots;
        }

        return normalized;
    }

    private static ulong BuildDotaItemInstanceId(ulong steamId, uint defIndex)
    {
        ulong accountBits = steamId & 0xFFFFFFFFUL;
        return 0x7000000000000000UL | (accountBits << 20) | defIndex;
    }

    private uint ResolveGameServerPublicIp(uint candidate)
    {
        if (TryGetConfiguredAdvertisedGameServerIp(out var configured))
        {
            return configured;
        }

        return candidate != 0 ? candidate : ToUInt32(IPAddress.Loopback);
    }

    private string ResolveDotaGameServerConnectIp(
        string clientIp,
        string publicIpValue,
        string privateIpValue,
        string fallbackIp)
    {
        if (IsLoopbackClient(clientIp))
        {
            if (TryGetConfiguredAdvertisedGameServerIp(out var configuredForLocal))
            {
                return ToIPv4String(configuredForLocal);
            }

            var hostIp = EnumerateUsableHostIPv4().FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(hostIp))
            {
                return hostIp;
            }

            return "127.0.0.1";
        }

        // Prefer a host-local interface on the same subnet as the requesting client.
        // A multi-homed host (WiFi + ZeroTier) must advertise the address that client
        // can actually route to, not a single hardcoded interface. This takes priority
        // over the configured/advertised IP precisely so ZeroTier peers (10.0.0.x) get
        // the host's ZeroTier address instead of the WiFi one.
        if (TryPickHostIpForClientSubnet(clientIp, out var sameSubnetHostIp))
        {
            return sameSubnetHostIp;
        }

        if (TryGetConfiguredAdvertisedGameServerIp(out var configured))
        {
            return ToIPv4String(configured);
        }

        if (TryNormalizeUsableIPv4(clientIp, out var normalizedClientIp))
        {
            return normalizedClientIp;
        }

        if (TryParseIPv4Value(publicIpValue, out var publicIp) && IsUsableIPv4(publicIp))
        {
            return ToIPv4String(publicIp);
        }

        if (TryParseIPv4Value(privateIpValue, out var privateIp) && IsUsableIPv4(privateIp))
        {
            return ToIPv4String(privateIp);
        }

        if (TryNormalizeUsableIPv4(fallbackIp, out var normalizedFallbackIp))
        {
            return normalizedFallbackIp;
        }

        return string.IsNullOrWhiteSpace(fallbackIp) ? "127.0.0.1" : fallbackIp.Trim();
    }

    // Builds a space-separated pair "primaryIp secondaryIp" for the Dota connect
    // field. The dedicated server binds every interface (0.0.0.0), so we advertise
    // the two most useful host addresses (e.g. WiFi + ZeroTier); the client tries
    // both endpoints and connects over whichever one it can route to.
    private string ResolveDotaGameServerConnectIps(
        string clientIp,
        string publicIpValue,
        string privateIpValue,
        string fallbackIp)
    {
        var ordered = new List<string>();

        void Add(string? value, bool allowLoopback = false)
        {
            if (string.IsNullOrWhiteSpace(value) ||
                !IPAddress.TryParse(value.Trim(), out var parsed) ||
                parsed.AddressFamily != AddressFamily.InterNetwork)
            {
                return;
            }

            var isLoopback = IPAddress.IsLoopback(parsed);
            if (isLoopback && !allowLoopback)
            {
                return;
            }

            if (!isLoopback && !IsUsableIPv4(ToUInt32(parsed)))
            {
                return;
            }

            var text = isLoopback ? "127.0.0.1" : parsed.ToString();
            if (!ordered.Contains(text))
            {
                ordered.Add(text);
            }
        }

        // 1) Best single match (subnet-aware → configured → CMsgGameServerInfo chain).
        Add(ResolveDotaGameServerConnectIp(clientIp, publicIpValue, privateIpValue, fallbackIp));

        // 2) The configured/advertised address so LAN peers always have it explicitly.
        if (TryGetConfiguredAdvertisedGameServerIp(out var configured))
        {
            Add(ToIPv4String(configured));
        }

        // 3) Every other usable host interface (covers ZeroTier / overlay NICs).
        foreach (var ip in EnumerateUsableHostIPv4())
        {
            Add(ip);
        }

        if (ordered.Count == 0 && IsLoopbackClient(clientIp))
        {
            Add("127.0.0.1", allowLoopback: true);
        }

        if (ordered.Count == 0)
        {
            return string.IsNullOrWhiteSpace(fallbackIp) ? "127.0.0.1" : fallbackIp.Trim();
        }

        return string.Join(' ', ordered.Take(2));
    }

    private static IEnumerable<string> EnumerateUsableHostIPv4()
    {
        var result = new List<string>();
        try
        {
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus != OperationalStatus.Up ||
                    nic.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                foreach (var unicast in nic.GetIPProperties().UnicastAddresses)
                {
                    if (unicast.Address.AddressFamily == AddressFamily.InterNetwork &&
                        IsUsableIPv4(ToUInt32(unicast.Address)))
                    {
                        result.Add(unicast.Address.ToString());
                    }
                }
            }
        }
        catch
        {
            // Interface enumeration is best-effort; fall back to what we already have.
        }

        return result;
    }

    private static bool IsLoopbackClient(string? clientIp)
    {
        if (string.IsNullOrWhiteSpace(clientIp) ||
            !IPAddress.TryParse(clientIp.Trim(), out var parsed))
        {
            return false;
        }

        return IPAddress.IsLoopback(parsed);
    }

    private bool TryGetConfiguredAdvertisedGameServerIp(out uint ip)
    {
        ip = 0;
        var advertised = _gameServerSettings.Current.AdvertisedGameServerIp;
        if (string.IsNullOrWhiteSpace(advertised) ||
            string.Equals(advertised, "auto", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!IPAddress.TryParse(advertised, out var parsed) ||
            parsed.AddressFamily != AddressFamily.InterNetwork)
        {
            return false;
        }

        ip = ToUInt32(parsed);
        return true;
    }

    private static bool TryNormalizeUsableIPv4(string? value, out string ip)
    {
        ip = string.Empty;
        if (string.IsNullOrWhiteSpace(value) ||
            !IPAddress.TryParse(value.Trim(), out var parsed) ||
            parsed.AddressFamily != AddressFamily.InterNetwork)
        {
            return false;
        }

        var numeric = ToUInt32(parsed);
        if (!IsUsableIPv4(numeric))
        {
            return false;
        }

        ip = parsed.ToString();
        return true;
    }

    private static bool TryParseIPv4Value(string? value, out uint ip)
    {
        ip = 0;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        value = value.Trim();
        if (IPAddress.TryParse(value, out var parsed) && parsed.AddressFamily == AddressFamily.InterNetwork)
        {
            ip = ToUInt32(parsed);
            return true;
        }

        if (!ulong.TryParse(value, out var raw) || raw > uint.MaxValue)
        {
            return false;
        }

        ip = (uint)raw;
        return true;
    }

    private static bool IsUsableIPv4(uint ip)
    {
        var a = (byte)(ip >> 24);
        var b = (byte)(ip >> 16);
        var c = (byte)(ip >> 8);
        var d = (byte)ip;

        if (a == 0 || a == 127 || a == 255 || (a == 169 && b == 254))
        {
            return false;
        }

        return !(a == 0 && b == 0 && c == 0 && d == 0) &&
               !(a == 255 && b == 255 && c == 255 && d == 255);
    }

    private static bool TryPickHostIpForClientSubnet(string? clientIp, out string hostIp)
    {
        hostIp = string.Empty;
        if (!IPAddress.TryParse(clientIp?.Trim(), out var client) ||
            client.AddressFamily != AddressFamily.InterNetwork ||
            !IsUsableIPv4(ToUInt32(client)))
        {
            return false;
        }

        try
        {
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus != OperationalStatus.Up ||
                    nic.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }

                foreach (var unicast in nic.GetIPProperties().UnicastAddresses)
                {
                    if (unicast.Address.AddressFamily != AddressFamily.InterNetwork ||
                        !IsUsableIPv4(ToUInt32(unicast.Address)))
                    {
                        continue;
                    }

                    var mask = unicast.IPv4Mask;
                    if (mask == null || Equals(mask, IPAddress.Any))
                    {
                        continue;
                    }

                    if (SameSubnet(unicast.Address, client, mask))
                    {
                        hostIp = unicast.Address.ToString();
                        return true;
                    }
                }
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    private static bool SameSubnet(IPAddress a, IPAddress b, IPAddress mask)
    {
        var ab = a.GetAddressBytes();
        var bb = b.GetAddressBytes();
        var mb = mask.GetAddressBytes();
        if (ab.Length != bb.Length || ab.Length != mb.Length)
        {
            return false;
        }

        for (var i = 0; i < ab.Length; i++)
        {
            if ((ab[i] & mb[i]) != (bb[i] & mb[i]))
            {
                return false;
            }
        }

        return true;
    }

    private static string ToIPv4String(uint ip)
    {
        return $"{(byte)(ip >> 24)}.{(byte)(ip >> 16)}.{(byte)(ip >> 8)}.{(byte)ip}";
    }

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

    public static bool TryNormalizeRemotePath(string path, out string normalized)
    {
        normalized = string.Empty;
        if (string.IsNullOrWhiteSpace(path)) return false;

        if (path.Contains("..") || path.Contains(":") || Path.IsPathRooted(path))
        {
            return false;
        }

        var parts = path.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0 || parts.Any(p => p == "." || p == ".."))
        {
            return false;
        }

        normalized = string.Join("/", parts).ToLowerInvariant();
        return !string.IsNullOrWhiteSpace(normalized);
    }

    public static string MakeRemoteStorageKey(ulong ownerSteamId, uint appId, string normalizedName)
    {
        return $"{ownerSteamId}:{appId}:{normalizedName}";
    }

    public bool IsValidToken(string token)
    {
        lock (_sync)
        {
            return TryGetSession(token, out _);
        }
    }
}
