using System.Net;
using System.Security.Cryptography;
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

    public ApiGameServerResult RegisterGameServer(ApiGameServerState request)
    {
        lock (_sync)
        {
            var server = request.Server ?? new ApiGameServer();
            var publicIp = ResolveGameServerPublicIp(server.IP);
            server.IP = publicIp;
            var serverId = server.SteamId != 0 ? server.SteamId : (ulong)publicIp;
            server.SteamId = serverId;
            var secure = NormalizeGameServerSecurity(server);

            _state.GameServers[serverId] = server;
            _dotaDedicatedServers.ObserveRegistration(serverId, server);
            SaveState();
            return new ApiGameServerResult { Success = true, PublicIP = publicIp, Secure = secure, SteamId = serverId };
        }
    }

    public ApiGameServerResult LogOnGameServer(ApiGameServerState request) => RegisterGameServer(request);

    public void LogOffGameServer()
    {
    }

    public bool UpdateGameServerState(ApiGameServer server)
    {
        lock (_sync)
        {
            var publicIp = ResolveGameServerPublicIp(server.IP);
            server.IP = publicIp;
            var serverId = server.SteamId != 0 ? server.SteamId : (ulong)publicIp;
            server.SteamId = serverId;
            NormalizeGameServerSecurity(server);
            _state.GameServers[serverId] = server;
            _dotaDedicatedServers.ObserveRegistration(serverId, server);
            SaveState();
            return true;
        }
    }

    public bool HeartbeatGameServer(ApiGameServer server) => UpdateGameServerState(server);

    public ApiGameServerPublicIp GetPublicIp()
    {
        lock (_sync)
        {
            var ip = ResolveGameServerPublicIp(_state.GameServers.Values.LastOrDefault()?.IP ?? 0);
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

    public bool SendP2P(string token, ApiP2PPacketSend request)
    {
        lock (_sync)
        {
            if (request == null || request.RemoteSteamId == 0 || !TryGetSession(token, out var session))
            {
                return false;
            }

            EnqueueP2PLocked(session!, request);

            return true;
        }
    }

    public bool SendP2PBatch(string token, ApiP2PPacketBatch request)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return false;
            }

            foreach (var packet in request?.Packets ?? new List<ApiP2PPacketSend>())
            {
                if (packet == null || packet.RemoteSteamId == 0)
                {
                    continue;
                }

                EnqueueP2PLocked(session!, packet);
            }

            return true;
        }
    }

    private void EnqueueP2PLocked(ApiSession session, ApiP2PPacketSend request)
    {
        EnqueueEvent(request.RemoteSteamId, new ApiEvent
        {
            Type = "p2p_packet",
            RemoteSteamId = session.SteamId,
            PayloadBase64 = request.BufferBase64,
            Channel = request.Channel,
            Transport = string.IsNullOrWhiteSpace(request.Transport) ? "legacy" : request.Transport,
            VirtualPort = request.VirtualPort
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

            steamId = requestedSteamId != 0 ? requestedSteamId : session!.SteamId;
            _state.Users.TryGetValue(steamId, out var user);
            appId = requestedAppId != 0 ? requestedAppId : user?.AppId ?? 0;
            return true;
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

            var contextSteamId = request.SteamId != 0 ? request.SteamId : session!.SteamId;
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

            var contextSteamId = request.SteamId != 0 ? request.SteamId : session!.SteamId;
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
