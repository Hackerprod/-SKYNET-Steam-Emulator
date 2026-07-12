using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using SKYNET_server.GC.Dota2;
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
            _state.Stats[steamId] = new SkyNetStatsEnvelopeDto
            {
                SteamId = steamId,
                CurrentPlayers = 1,
                Stats = new List<SkyNetStatDto> { new() { Name = "wins", Data = 0 }, new() { Name = "kills", Data = 0 } },
                Achievements = new List<SkyNetAchievementDto> { new() { Name = "first_launch", Earned = true, Date = DateTime.UtcNow } }
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
            EnqueueEvent(recipient, new SkyNetEventDto
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
                    : new Dictionary<string, string>()
            });
        }
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

        Monitor.PulseAll(_sync);
        TrimQueuedEventsLocked();
    }

    private void EnqueueGcMessageEvent(ulong recipientSteamId, SkyNetGCMessageDto message)
    {
        lock (_sync)
        {
            EnqueueEvent(recipientSteamId, new SkyNetEventDto
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

        if (expired.Count > 0)
        {
            DotaGcBackend.ExpireInactiveSessions(_sessions.Values.Select(session => session.SteamId).ToHashSet());
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

    private void LoadState()
    {
        if (!File.Exists(_statePath))
        {
            _state = new ApiState();
            return;
        }

        try
        {
            var json = File.ReadAllText(_statePath);
            _state = string.IsNullOrWhiteSpace(json)
                ? new ApiState()
                : JsonSerializer.Deserialize<ApiState>(json, _jsonOptions) ?? new ApiState();
        }
        catch (JsonException)
        {
            _state = new ApiState();
        }
        catch (IOException)
        {
            _state = new ApiState();
        }
    }

    private void NormalizeState()
    {
        _state.Users ??= new Dictionary<ulong, SkyNetUserDto>();
        _state.FriendLinks ??= new Dictionary<ulong, HashSet<ulong>>();
        _state.FriendRequests ??= new List<SkyNetFriendRequestDto>();
        _state.Avatars ??= new Dictionary<ulong, string>();
        _state.Stats ??= new Dictionary<ulong, SkyNetStatsEnvelopeDto>();
        _state.Lobbies ??= new Dictionary<ulong, SkyNetLobbyDto>();
        _state.Files ??= new Dictionary<string, SkyNetRemoteStorageFileDto>(StringComparer.OrdinalIgnoreCase);
        _state.GameServers ??= new Dictionary<ulong, SkyNetGameServerDto>();
        // State files written before dedicated support used the host IP as key.
        // Several dedicated processes share an IP, so current entries are keyed
        // by their game-server SteamID instead.
        if (_state.GameServers.Count > 0)
        {
            var normalizedServers = new Dictionary<ulong, SkyNetGameServerDto>();
            foreach (var pair in _state.GameServers)
            {
                var server = pair.Value ?? new SkyNetGameServerDto();
                var key = server.SteamId != 0 ? server.SteamId : pair.Key;
                server.SteamId = key;
                normalizedServers[key] = server;
            }
            _state.GameServers = normalizedServers;
        }
        _state.WebAccounts ??= new Dictionary<string, SkyNetWebAccountDto>(StringComparer.OrdinalIgnoreCase);
        _state.WebSessions = new Dictionary<string, ApiSession>(_state.WebSessions ?? new Dictionary<string, ApiSession>(), StringComparer.Ordinal);
        _state.DotaItems ??= new Dictionary<uint, SkyNetDotaItemDto>();
        _state.DotaEquipment ??= new Dictionary<ulong, List<SkyNetDotaEquipmentDto>>();
        _state.DotaMatches ??= new Dictionary<ulong, SkyNetDotaMatchDto>();
        _state.DotaHeroIds ??= new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
        _state.DotaHeroSlots = NormalizeDotaHeroSlots(_state.DotaHeroSlots);
        _state.DotaCosmetics ??= new SkyNetDotaCosmeticSettingsDto();
        EnsureDotaHeroSlotsLoadedLocked();
        NormalizeDotaItemSlotsLocked();
        var now = DateTime.UtcNow;
        foreach (var pair in _state.WebSessions.ToList())
        {
            pair.Value.WebSession = true;
            pair.Value.Persistent = true;
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
            var list = pair.Value ?? new List<SkyNetDotaEquipmentDto>();
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
            match.Players ??= new List<SkyNetDotaMatchPlayerDto>();
            foreach (var player in match.Players)
            {
                player.Equipment ??= new List<SkyNetDotaEquipmentDto>();
            }
        }
    }

    // Persist state on demand (e.g. on graceful shutdown).
    public void FlushState()
    {
        SaveState();
    }

    // Remove atomic-write temp files left behind by force-killed previous runs.
    // The write path names them "<state>.<pid>.<guid>.tmp"; a clean run deletes
    // its own, but a hard kill between write and move orphans them (~10 MB each).
    private void CleanupOrphanStateTempFiles()
    {
        try
        {
            var dir = Path.GetDirectoryName(_statePath)!;
            if (!Directory.Exists(dir))
            {
                return;
            }

            foreach (var file in Directory.EnumerateFiles(dir, $"{Path.GetFileName(_statePath)}.*.tmp"))
            {
                try { File.Delete(file); } catch { /* locked/absent: best effort */ }
            }
        }
        catch
        {
            // best effort; never block startup on cleanup
        }
    }

    private void SaveState()
    {
        lock (_sync)
        {
            var stateDirectory = Path.GetDirectoryName(_statePath)!;
            Directory.CreateDirectory(stateDirectory);

            var tempPath = Path.Combine(
                stateDirectory,
                $"{Path.GetFileName(_statePath)}.{Environment.ProcessId}.{Guid.NewGuid():N}.tmp");

            try
            {
                File.WriteAllText(tempPath, JsonSerializer.Serialize(_state, _jsonOptions), Encoding.UTF8);

                for (var attempt = 0; ; attempt++)
                {
                    try
                    {
                        File.Move(tempPath, _statePath, true);
                        break;
                    }
                    catch (Exception ex) when (IsTransientStateSaveException(ex) && attempt < 5)
                    {
                        System.Threading.Thread.Sleep(25 * (attempt + 1));
                    }
                }
            }
            finally
            {
                try
                {
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }
    }

    private static bool IsTransientStateSaveException(Exception exception) =>
        exception is IOException or UnauthorizedAccessException;

    private static SkyNetUserDto CloneUser(SkyNetUserDto user) => new()
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
        TargetJobId = evt.TargetJobId,
        Protobuf = evt.Protobuf,
        Channel = evt.Channel,
        RemoteSteamId = evt.RemoteSteamId,
        FriendRelationship = evt.FriendRelationship,
        RequestId = evt.RequestId
    };

    private static SkyNetRemoteStorageFileDto CloneFile(SkyNetRemoteStorageFileDto file) => new()
    {
        FileName = file.FileName,
        ContentBase64 = file.ContentBase64,
        Size = file.Size,
        Timestamp = file.Timestamp
    };

    private static SkyNetDotaItemDto CloneDotaItem(SkyNetDotaItemDto item) => new()
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

    private static SkyNetDotaEquipmentDto CloneDotaEquipment(SkyNetDotaEquipmentDto item) => new()
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

    private static SkyNetDotaMatchDto CloneDotaMatch(SkyNetDotaMatchDto match) => new()
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
        Players = match.Players.Select(player => new SkyNetDotaMatchPlayerDto
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

    private SkyNetDotaCosmeticSummaryDto BuildDotaCosmeticSummaryLocked() => new()
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

        void Add(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) ||
                !IPAddress.TryParse(value.Trim(), out var parsed) ||
                parsed.AddressFamily != AddressFamily.InterNetwork ||
                !IsUsableIPv4(ToUInt32(parsed)))
            {
                return;
            }

            var text = parsed.ToString();
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

    private bool TryGetConfiguredAdvertisedGameServerIp(out uint ip)
    {
        ip = 0;
        if (string.IsNullOrWhiteSpace(_advertisedGameServerIp) ||
            string.Equals(_advertisedGameServerIp, "auto", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!IPAddress.TryParse(_advertisedGameServerIp, out var parsed) ||
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
}
