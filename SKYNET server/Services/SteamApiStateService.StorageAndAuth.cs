using System.Net;
using System.Security.Cryptography;
using SKYNET.Protocol;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    private const uint ServerFlagSecure = 0x02;

    public bool PutFile(string token, ApiRemoteStorageUploadRequest file)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return false;
            }

            if (!TryNormalizeRemotePath(file.FileName, out var normalized))
            {
                return false;
            }

            var key = MakeRemoteStorageKey(session!.SteamId, session.AppId, normalized);
            byte[] contentBytes;
            try
            {
                contentBytes = string.IsNullOrWhiteSpace(file.ContentBase64)
                    ? Array.Empty<byte>()
                    : Convert.FromBase64String(file.ContentBase64);
            }
            catch (FormatException)
            {
                return false;
            }

            var sha256 = Convert.ToHexString(SHA256.HashData(contentBytes)).ToLowerInvariant();
            var version = 1;
            var syncPlatforms = file.SyncPlatforms ?? 0xFFFFFFFFU;
            if (_state.Files.TryGetValue(key, out var existing))
            {
                version = existing.Version + 1;
                syncPlatforms = file.SyncPlatforms ?? existing.SyncPlatforms;
            }

            var storageFile = new ApiRemoteStorageFile
            {
                OwnerSteamId = session.SteamId,
                AppId = session.AppId,
                FileName = file.FileName,
                ContentBase64 = Convert.ToBase64String(contentBytes),
                Size = contentBytes.Length,
                Timestamp = ToUnixTime(DateTime.UtcNow),
                Sha256 = sha256,
                SyncPlatforms = syncPlatforms,
                Version = version,
                Persisted = true,
                DeletedAt = null
            };

            _state.Files[key] = storageFile;
            SaveState();
            return true;
        }
    }

    public ApiRemoteStorageFile? GetFile(string token, string fileName)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return null;
            }

            if (!TryNormalizeRemotePath(fileName, out var normalized))
            {
                return null;
            }

            var key = MakeRemoteStorageKey(session!.SteamId, session.AppId, normalized);
            if (_state.Files.TryGetValue(key, out var file) && file.DeletedAt == null)
            {
                return CloneFile(file);
            }

            return null;
        }
    }

    public List<ApiRemoteStorageFileListItem>? ListFiles(string token)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return null;
            }

            return _state.Files.Values
                .Where(f => f.OwnerSteamId == session!.SteamId && f.AppId == session.AppId && f.DeletedAt == null)
                .OrderBy(f => f.FileName)
                .Select(f => new ApiRemoteStorageFileListItem
                {
                    FileName = f.FileName,
                    Size = f.Size,
                    Timestamp = f.Timestamp,
                    Sha256 = f.Sha256,
                    Version = f.Version
                })
                .ToList();
        }
    }

    public bool DeleteFile(string token, string fileName)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return false;
            }

            if (!TryNormalizeRemotePath(fileName, out var normalized))
            {
                return false;
            }

            var key = MakeRemoteStorageKey(session!.SteamId, session.AppId, normalized);
            if (!_state.Files.TryGetValue(key, out var existing) || existing.DeletedAt != null)
            {
                return false;
            }

            existing.DeletedAt = DateTime.UtcNow;
            existing.Version++;
            existing.Timestamp = ToUnixTime(DateTime.UtcNow);
            SaveState();
            return true;
        }
    }

    public ApiRemoteStorageShare? ShareFile(string token, string fileName)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return null;
            }

            if (!TryNormalizeRemotePath(fileName, out var normalized))
            {
                return null;
            }

            var key = MakeRemoteStorageKey(session!.SteamId, session.AppId, normalized);
            if (!_state.Files.TryGetValue(key, out var file) || file.DeletedAt != null)
            {
                return null;
            }

            var handle = ++_nextFileShareHandle;
            var share = new ApiRemoteStorageShareRecord
            {
                Handle = handle,
                OwnerSteamId = session.SteamId,
                AppId = session.AppId,
                NormalizedName = normalized
            };

            _state.FileShares[handle] = share;
            SaveState();

            return new ApiRemoteStorageShare
            {
                Handle = handle,
                Result = ResultOk
            };
        }
    }

    public ApiRemoteStorageQuota? GetQuota(string token)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return null;
            }

            var used = (ulong)_state.Files.Values
                .Where(f => f.OwnerSteamId == session!.SteamId && f.AppId == session.AppId && f.DeletedAt == null)
                .Sum(f => (long)f.Size);

            const ulong total = 1024UL * 1024UL * 1024UL;
            return new ApiRemoteStorageQuota
            {
                TotalBytes = total,
                AvailableBytes = total > used ? total - used : 0
            };
        }
    }

    public ApiAuthTicket CreateTicket(ApiAuthTicketRequest request)
    {
        lock (_sync)
        {
            var handle = ++_nextTicketHandle;
            var payload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{handle}:{request.SteamId}:{request.AppId}:{request.GameServer}"));
            _tickets[handle] = new ApiTicket
            {
                Handle = handle,
                SteamId = request.SteamId,
                AppId = request.AppId,
                GameServer = request.GameServer,
                TicketBase64 = payload
            };

            return new ApiAuthTicket
            {
                Handle = handle,
                TicketBase64 = payload,
                TicketSize = (uint)System.Text.Encoding.UTF8.GetByteCount(payload)
            };
        }
    }

    public ApiAuthValidateResult ValidateTicket(ApiAuthValidateRequest request)
    {
        lock (_sync)
        {
            var ticket = _tickets.Values.FirstOrDefault(t => t.TicketBase64 == request.TicketBase64 && t.AppId == request.AppId);
            if (ticket == null)
            {
                if (!string.IsNullOrWhiteSpace(request.TicketBase64))
                {
                    return new ApiAuthValidateResult
                    {
                        Success = true,
                        BeginAuthSessionResult = BeginAuthOk,
                        AuthSessionResponse = AuthResponseOk,
                        OwnerSteamId = request.SteamId
                    };
                }

                return new ApiAuthValidateResult
                {
                    Success = false,
                    BeginAuthSessionResult = BeginAuthInvalidTicket,
                    AuthSessionResponse = AuthResponseInvalidTicket,
                    OwnerSteamId = request.SteamId
                };
            }

            return new ApiAuthValidateResult
            {
                Success = true,
                BeginAuthSessionResult = BeginAuthOk,
                AuthSessionResponse = AuthResponseOk,
                OwnerSteamId = ticket.SteamId
            };
        }
    }

    public ApiConnectAuthResult ConnectAndAuthenticate(ApiConnectAuthRequest request)
    {
        var validation = ValidateTicket(new ApiAuthValidateRequest
        {
            AppId = request.AppId,
            SteamId = request.SteamId,
            TicketBase64 = request.AuthBlobBase64,
            GameServer = true
        });

        return new ApiConnectAuthResult
        {
            Success = validation.Success,
            SteamId = request.SteamId,
            OwnerSteamId = validation.OwnerSteamId,
            DenyReason = validation.Success ? 0 : 2,
            DenyMessage = validation.Success ? string.Empty : "Invalid auth ticket"
        };
    }

    public void EndAuthSession(ApiAuthEndSessionRequest request)
    {
    }

    public void CancelTicket(ApiCancelAuthTicketRequest request)
    {
        lock (_sync)
        {
            _tickets.Remove(request.Handle);
        }
    }

    public ApiGameServerResult RegisterGameServer(ApiGameServerState request, string? remoteIp = null)
    {
        lock (_sync)
        {
            var server = request.Server ?? new ApiGameServer();
            var publicIp = ResolveGameServerPublicIp(server.IP, remoteIp);
            server.IP = publicIp;
            var serverId = ResolveGameServerSteamIdLocked(server, request.Anonymous);
            server.SteamId = serverId;
            server.LoggedOn = request.Server?.LoggedOn == true;
            var secure = NormalizeGameServerSecurity(server);

            _state.GameServers[serverId] = server;
            _gameServerLeases[serverId] = DateTime.UtcNow;
            _dotaDedicatedServers?.ObserveRegistration(serverId, server);
            SaveState();
            return new ApiGameServerResult { Success = true, PublicIP = publicIp, Secure = secure, SteamId = serverId };
        }
    }

    public ApiGameServerResult LogOnGameServer(ApiGameServerState request, string? remoteIp = null) =>
        RegisterGameServer(request, remoteIp);

    public bool LogOffGameServer(ulong steamId)
    {
        steamId = SteamIdAllocator.CanonicalGameServerSteamId(steamId);
        if (steamId == 0)
        {
            return false;
        }

        lock (_sync)
        {
            _gameServerLeases.Remove(steamId);
            var removed = _state.GameServers.Remove(steamId);
            if (removed)
            {
                SaveState();
            }

            return removed;
        }
    }

    public bool UpdateGameServerState(ApiGameServer server, string? remoteIp = null)
    {
        if (server == null)
        {
            return false;
        }

        lock (_sync)
        {
            var publicIp = ResolveGameServerPublicIp(server.IP, remoteIp);
            server.IP = publicIp;
            server.SteamId = SteamIdAllocator.CanonicalGameServerSteamId(server.SteamId);
            // A state update is not a way to register: the server must already be known,
            // by its ID or, when it sends none, by the endpoint it registered from.
            var serverId = server.SteamId != 0 ? server.SteamId : FindRegisteredGameServerByEndpointLocked(server.IP, server.Port);
            if (serverId == 0 ||
                !_state.GameServers.TryGetValue(serverId, out var registered) ||
                !IsGameServerLeaseLiveLocked(serverId) ||
                !IsSameGameServerEndpoint(registered, server))
            {
                return false;
            }

            server.SteamId = serverId;
            NormalizeGameServerSecurity(server);
            _state.GameServers[serverId] = server;
            _gameServerLeases[serverId] = DateTime.UtcNow;
            _dotaDedicatedServers?.ObserveRegistration(serverId, server);
            SaveState();
            return true;
        }
    }

    public bool HeartbeatGameServer(ApiGameServer server, string? remoteIp = null) =>
        RefreshGameServerLease(server, remoteIp);

    public IReadOnlyList<ApiGameServer> ListGameServers(uint appId)
    {
        lock (_sync)
        {
            PruneExpiredGameServersLocked();

            return _gameServerLeases.Keys
                .Where(steamId => _state.GameServers.TryGetValue(steamId, out var server) &&
                    server.LoggedOn &&
                    server.AdvertiseActive &&
                    (appId == 0 || server.AppId == appId))
                .Select(steamId => CloneGameServer(_state.GameServers[steamId]))
                .OrderBy(server => server.SteamId)
                .ToArray();
        }
    }

    // A registration without a live lease is dead weight: every write path that
    // upserts _state.GameServers stamps _gameServerLeases in the same call, so a
    // GameServers entry with no matching (or expired) lease can only be a server
    // that disappeared without calling LogOff (crash, alt-f4, killed process) -
    // nothing else still wants it. Removing it here (not just filtering it out of
    // ListGameServers, as before) keeps the admin overview and api-state.json from
    // accumulating dead registrations forever.
    private void PruneExpiredGameServersLocked()
    {
        var now = DateTime.UtcNow;
        var stale = _state.GameServers.Keys
            .Where(steamId => !_gameServerLeases.TryGetValue(steamId, out var leaseAt) || now - leaseAt > _gameServerLeaseTimeout)
            .ToArray();

        if (stale.Length == 0)
        {
            return;
        }

        foreach (var steamId in stale)
        {
            _gameServerLeases.Remove(steamId);
            _state.GameServers.Remove(steamId);
        }

        SaveState();
    }

    public ApiGameServerPublicIp GetPublicIp(string? remoteIp = null)
    {
        lock (_sync)
        {
            var ip = ResolveGameServerPublicIp(0, remoteIp);
            return new ApiGameServerPublicIp { PublicIP = ip };
        }
    }

    public bool DisconnectGameServerUser(ulong steamId) => true;

    public bool UpdateGameServerUserData(ApiGameServerUserData request)
    {
        lock (_sync)
        {
            if (_state.Users.TryGetValue(request.SteamId, out var user) && !string.IsNullOrWhiteSpace(request.PlayerName))
            {
                user.PersonaName = request.PlayerName;
                SaveState();
            }

            return true;
        }
    }

    private static ApiGameServer CloneGameServer(ApiGameServer server)
    {
        return new ApiGameServer
        {
            SteamId = server.SteamId,
            AppId = server.AppId,
            IP = server.IP,
            Port = server.Port,
            QueryPort = server.QueryPort,
            Flags = server.Flags,
            Secure = server.Secure,
            VersionString = server.VersionString,
            Product = server.Product,
            Description = server.Description,
            ModDir = server.ModDir,
            Dedicated = server.Dedicated,
            MaxPlayers = server.MaxPlayers,
            BotPlayers = server.BotPlayers,
            ServerName = server.ServerName,
            MapName = server.MapName,
            PasswordProtected = server.PasswordProtected,
            SpectatorPort = server.SpectatorPort,
            SpectatorServerName = server.SpectatorServerName,
            GameTags = server.GameTags,
            GameData = server.GameData,
            Region = server.Region,
            LoggedOn = server.LoggedOn,
            AdvertiseActive = server.AdvertiseActive,
            KeyValues = new Dictionary<string, string>(
                server.KeyValues ?? new Dictionary<string, string>(),
                StringComparer.OrdinalIgnoreCase),
            Players = (server.Players ?? new List<ApiGameServerPlayer>()).Select(player => new ApiGameServerPlayer
            {
                SteamId = player.SteamId,
                Name = player.Name,
                Score = player.Score,
                TimePlayedSeconds = player.TimePlayedSeconds
            }).ToList()
        };
    }

    private bool RefreshGameServerLease(ApiGameServer server, string? remoteIp)
    {
        if (server == null || server.SteamId == 0)
        {
            return false;
        }

        server.SteamId = SteamIdAllocator.CanonicalGameServerSteamId(server.SteamId);

        lock (_sync)
        {
            if (!_state.GameServers.TryGetValue(server.SteamId, out var registered))
            {
                return false;
            }

            server.IP = ResolveGameServerPublicIp(server.IP, remoteIp);
            NormalizeGameServerSecurity(server);
            _state.GameServers[server.SteamId] = server;
            _gameServerLeases[server.SteamId] = DateTime.UtcNow;
            _dotaDedicatedServers?.ObserveRegistration(server.SteamId, server);
            return true;
        }
    }

    // A game server arrives with its own SteamID (the emulator makes one before it registers and the
    // game already uses it), so it is kept as long as it is a real GameServer/AnonGameServer ID and no
    // other live server holds it from a different endpoint. Only otherwise does the server assign one.
    private ulong ResolveGameServerSteamIdLocked(ApiGameServer server, bool anonymous)
    {
        var proposed = SteamIdAllocator.CanonicalGameServerSteamId(server.SteamId);
        if (SteamIdAllocator.IsGameServerSteamId(proposed) && !IsGameServerSteamIdTakenLocked(proposed, server))
        {
            return proposed;
        }

        return CreateGameServerSteamId(anonymous);
    }

    // "Same server" = same SteamID from the same endpoint (dedicated servers share an IP, so the
    // port is part of the identity). The same ID from another endpoint while the first is still live
    // is a real collision.
    private bool IsGameServerSteamIdTakenLocked(ulong steamId, ApiGameServer server)
    {
        if (!_state.GameServers.TryGetValue(steamId, out var registered) || !IsGameServerLeaseLiveLocked(steamId))
        {
            return false;
        }

        return !IsSameGameServerEndpoint(registered, server);
    }

    // A registration counts as live while its lease has not expired. Entries restored after a restart have
    // no lease until the first heartbeat, so they do not block anything.
    private bool IsGameServerLeaseLiveLocked(ulong steamId) =>
        _gameServerLeases.TryGetValue(steamId, out var leaseAt) && DateTime.UtcNow - leaseAt <= _gameServerLeaseTimeout;

    private static bool IsSameGameServerEndpoint(ApiGameServer registered, ApiGameServer server) =>
        registered.IP == server.IP && (registered.Port == server.Port || registered.Port == 0);

    private ulong FindRegisteredGameServerByEndpointLocked(uint ip, int port)
    {
        if (port <= 0)
        {
            return 0;
        }

        foreach (var pair in _state.GameServers)
        {
            if (pair.Value.IP == ip && pair.Value.Port == port)
            {
                return pair.Key;
            }
        }

        return 0;
    }

    // Fallback assignment, one counter per type over the IDs already in use (registered servers and
    // servers reserved for a dedicated lobby), so it can never hand out one of those again.
    private ulong CreateGameServerSteamId(bool anonymous)
    {
        var inUse = _state.GameServers.Keys
            .Concat(_dotaDedicatedServers?.GetReservedGameServerSteamIds() ?? Array.Empty<ulong>())
            .Where(id => SteamIdAllocator.IsGameServerSteamId(id) && SteamIdAllocator.IsAnonymousGameServer(id) == anonymous)
            .Select(id => (uint)id);
        return SteamIdAllocator.GameServerSteamId(SteamIdAllocator.Next(inUse), anonymous);
    }

    public ApiP2PSendResult SendP2P(string token, ApiP2PPacketSend request)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return ApiP2PSendResult.Unauthorized;
            }

            if (!TryValidateP2PRequest(request, out var transport, out var error))
            {
                _logger.LogWarning(
                    "Rejected P2P frame from {SteamId} to {RemoteSteamId}: {Reason}",
                    session!.SteamId,
                    request?.RemoteSteamId ?? 0,
                    error);
                return ApiP2PSendResult.InvalidRequest;
            }

            EnqueueP2PLocked(session!, request, transport);
            return ApiP2PSendResult.Success;
        }
    }

    public ApiP2PSendResult SendP2PBatch(string token, ApiP2PPacketBatch request)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return ApiP2PSendResult.Unauthorized;
            }

            if (request?.Packets == null ||
                request.Packets.Count > P2PTransportProtocol.MaxBatchSize)
            {
                return ApiP2PSendResult.InvalidRequest;
            }

            var validated = new List<(ApiP2PPacketSend Packet, P2PTransportKind Transport)>(request.Packets.Count);
            foreach (var packet in request.Packets)
            {
                if (!TryValidateP2PRequest(packet, out var transport, out var error))
                {
                    _logger.LogWarning(
                        "Rejected P2P batch from {SteamId}: {Reason}",
                        session!.SteamId,
                        error);
                    return ApiP2PSendResult.InvalidRequest;
                }

                validated.Add((packet, transport));
            }

            foreach (var item in validated)
            {
                EnqueueP2PLocked(session!, item.Packet, item.Transport);
            }

            return ApiP2PSendResult.Success;
        }
    }

    private static bool TryValidateP2PRequest(
        ApiP2PPacketSend request,
        out P2PTransportKind transport,
        out string error)
    {
        transport = P2PTransportKind.Legacy;
        error = string.Empty;
        if (request == null || request.RemoteSteamId == 0)
        {
            error = "A destination Steam ID is required.";
            return false;
        }

        if (!P2PTransportProtocol.TryParse(
                request.TransportVersion,
                request.Transport,
                out transport,
                out error))
        {
            return false;
        }

        if (!P2PTransportProtocol.TryGetDecodedPayloadLength(request.BufferBase64, out var payloadLength))
        {
            error = "The P2P payload is not valid Base64.";
            return false;
        }

        return P2PTransportProtocol.TryValidateFrame(
            request.TransportVersion,
            transport,
            request.Channel,
            request.VirtualPort,
            request.SourceConnectionId,
            request.TargetConnectionId,
            payloadLength,
            out error);
    }

    private void EnqueueP2PLocked(
        ApiSession session,
        ApiP2PPacketSend request,
        P2PTransportKind transport)
    {
        EnqueueEvent(request.RemoteSteamId, new ApiEvent
        {
            Type = "p2p_packet",
            RemoteSteamId = session.SteamId,
            PayloadBase64 = request.BufferBase64,
            Channel = request.Channel,
            TransportVersion = request.TransportVersion,
            Transport = P2PTransportProtocol.ToWireValue(transport),
            VirtualPort = request.VirtualPort,
            SourceConnectionId = request.SourceConnectionId,
            TargetConnectionId = request.TargetConnectionId
        });
    }

    private static byte NormalizeGameServerSecurity(ApiGameServer server)
    {
        var secure = server.Secure != 0 || (server.Flags & ServerFlagSecure) != 0;
        if (secure)
        {
            server.Secure = 1;
            server.Flags |= ServerFlagSecure;
        }
        else
        {
            server.Secure = 0;
            server.Flags &= ~ServerFlagSecure;
        }

        server.GameTags = NormalizeGameServerTags(server.GameTags, secure);
        return server.Secure;
    }

    private static string NormalizeGameServerTags(string? gameTags, bool secure)
    {
        if (string.IsNullOrWhiteSpace(gameTags))
        {
            return secure ? "secure" : "insecure";
        }

        var tags = gameTags
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(tag =>
                !tag.Equals("secure", StringComparison.OrdinalIgnoreCase) &&
                !tag.Equals("insecure", StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        tags.Add(secure ? "secure" : "insecure");

        return string.Join(',', tags);
    }

    public bool SendGCMessage(string token, ApiGCMessage request)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out _))
            {
                return false;
            }

            // One-way GC sends are acknowledged here. Request/response GC traffic is
            // handled by /gamecoordinator/exchange and routed through per-app plugins.
            return true;
        }
    }

    public bool TryResolveSessionIdentity(string token, ulong requestedSteamId, uint requestedAppId, out ulong steamId, out uint appId)
    {
        lock (_sync)
        {
            steamId = 0;
            appId = 0;
            if (!TryGetSession(token, out var session))
            {
                return false;
            }

            steamId = SteamIdAllocator.CanonicalGameServerSteamId(requestedSteamId != 0 ? requestedSteamId : session!.SteamId);
            _state.Users.TryGetValue(steamId, out var user);
            appId = requestedAppId != 0 ? requestedAppId : user?.AppId ?? 0;
            return true;
        }
    }

    public bool TryResolveCurrentSessionIdentity(string token, out ulong steamId, out uint appId)
    {
        lock (_sync)
        {
            steamId = 0;
            appId = 0;
            if (!TryGetSession(token, out var session))
            {
                return false;
            }

            steamId = session!.SteamId;
            appId = session.AppId;
            return steamId != 0 && appId != 0;
        }
    }

    public ApiGCExchangeResponse? ExchangeGCMessage(string token, ApiGCExchangeRequest request, string? clientIp = null)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return null;
            }

            var contextSteamId = SteamIdAllocator.CanonicalGameServerSteamId(request.SteamId != 0 ? request.SteamId : session!.SteamId);
            _state.Users.TryGetValue(contextSteamId, out var user);
            _state.Users.TryGetValue(session!.SteamId, out var sessionUser);
            var appId = request.AppId != 0 ? request.AppId : user?.AppId ?? 0;
            var context = new GameCoordinatorContext
            {
                AppId = appId,
                SteamId = contextSteamId,
                AccountId = SteamIdToAccountId(contextSteamId),
                PersonaName = user?.PersonaName ?? sessionUser?.PersonaName ?? string.Empty,
                ClientIp = clientIp ?? string.Empty
            };

            return _gameCoordinatorPlugins.Exchange(context, request);
        }
    }

    public ApiGCExchangeResponse? PollGCMessages(string token, ApiGCPollRequest request, string? clientIp = null)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return null;
            }

            var contextSteamId = SteamIdAllocator.CanonicalGameServerSteamId(request.SteamId != 0 ? request.SteamId : session!.SteamId);
            _state.Users.TryGetValue(contextSteamId, out var user);
            _state.Users.TryGetValue(session!.SteamId, out var sessionUser);
            var appId = request.AppId != 0 ? request.AppId : user?.AppId ?? sessionUser?.AppId ?? 0;
            var context = new GameCoordinatorContext
            {
                AppId = appId,
                SteamId = contextSteamId,
                AccountId = SteamIdToAccountId(contextSteamId),
                PersonaName = user?.PersonaName ?? sessionUser?.PersonaName ?? string.Empty,
                ClientIp = clientIp ?? string.Empty
            };

            return _gameCoordinatorPlugins.Poll(context);
        }
    }
}
