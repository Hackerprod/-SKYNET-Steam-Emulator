using System.Net;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    private const uint ServerFlagSecure = 0x02;

    public bool PutFile(string token, ApiRemoteStorageFile file)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out _))
            {
                return false;
            }

            file.Size = string.IsNullOrWhiteSpace(file.ContentBase64) ? 0 : Convert.FromBase64String(file.ContentBase64).Length;
            file.Timestamp = ToUnixTime(DateTime.UtcNow);
            _state.Files[file.FileName] = file;
            SaveState();
            return true;
        }
    }

    public ApiRemoteStorageFile? GetFile(string token, string fileName)
    {
        lock (_sync)
        {
            return _state.Files.TryGetValue(fileName, out var file) ? CloneFile(file) : null;
        }
    }

    public List<ApiRemoteStorageFileListItem>? ListFiles(string token)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out _))
            {
                return null;
            }

            return _state.Files.Values
                .OrderBy(f => f.FileName)
                .Select(f => new ApiRemoteStorageFileListItem { FileName = f.FileName, Size = f.Size, Timestamp = f.Timestamp })
                .ToList();
        }
    }

    public bool DeleteFile(string token, string fileName)
    {
        lock (_sync)
        {
            var removed = _state.Files.Remove(fileName);
            if (removed)
            {
                SaveState();
            }

            return removed;
        }
    }

    public ApiRemoteStorageShare? ShareFile(string token, string fileName)
    {
        lock (_sync)
        {
            if (!_state.Files.ContainsKey(fileName))
            {
                return null;
            }

            return new ApiRemoteStorageShare
            {
                Handle = ++_nextFileShareHandle,
                Result = ResultOk
            };
        }
    }

    public ApiRemoteStorageQuota GetQuota(string token)
    {
        lock (_sync)
        {
            var used = (ulong)_state.Files.Values.Sum(f => (long)f.Size);
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
            server.Flags &= ~ServerFlagSecure;
            server.GameTags = NormalizeGameServerTags(server.GameTags);

            _state.GameServers[serverId] = server;
            _dotaDedicatedServers.ObserveRegistration(serverId, server);
            SaveState();
            return new ApiGameServerResult { Success = true, PublicIP = publicIp, Secure = 0, SteamId = serverId };
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
            server.Flags &= ~ServerFlagSecure;
            server.GameTags = NormalizeGameServerTags(server.GameTags);
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
            if (!TryGetSession(token, out var session))
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

            foreach (var packet in request.Packets ?? new List<ApiP2PPacketSend>())
            {
                if (packet?.RemoteSteamId != 0)
                {
                    EnqueueP2PLocked(session!, packet);
                }
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
            Channel = request.Channel
        });
    }

    private static string NormalizeGameServerTags(string? gameTags)
    {
        if (string.IsNullOrWhiteSpace(gameTags))
        {
            return "insecure";
        }

        var tags = gameTags
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(tag => tag.Equals("secure", StringComparison.OrdinalIgnoreCase) ? "insecure" : tag)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (!tags.Any(tag => tag.Equals("insecure", StringComparison.OrdinalIgnoreCase)))
        {
            tags.Add("insecure");
        }

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
