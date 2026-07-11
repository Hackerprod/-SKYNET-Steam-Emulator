using System.Net;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
    private const uint ServerFlagSecure = 0x02;

    public bool PutFile(string token, SkyNetRemoteStorageFileDto file)
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

    public SkyNetRemoteStorageFileDto? GetFile(string token, string fileName)
    {
        lock (_sync)
        {
            return _state.Files.TryGetValue(fileName, out var file) ? CloneFile(file) : null;
        }
    }

    public List<SkyNetRemoteStorageFileListItemDto>? ListFiles(string token)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out _))
            {
                return null;
            }

            return _state.Files.Values
                .OrderBy(f => f.FileName)
                .Select(f => new SkyNetRemoteStorageFileListItemDto { FileName = f.FileName, Size = f.Size, Timestamp = f.Timestamp })
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

    public SkyNetRemoteStorageShareDto? ShareFile(string token, string fileName)
    {
        lock (_sync)
        {
            if (!_state.Files.ContainsKey(fileName))
            {
                return null;
            }

            return new SkyNetRemoteStorageShareDto
            {
                Handle = ++_nextFileShareHandle,
                Result = ResultOk
            };
        }
    }

    public SkyNetRemoteStorageQuotaDto GetQuota(string token)
    {
        lock (_sync)
        {
            var used = (ulong)_state.Files.Values.Sum(f => (long)f.Size);
            const ulong total = 1024UL * 1024UL * 1024UL;
            return new SkyNetRemoteStorageQuotaDto
            {
                TotalBytes = total,
                AvailableBytes = total > used ? total - used : 0
            };
        }
    }

    public SkyNetAuthTicketDto CreateTicket(SkyNetAuthTicketRequestDto request)
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

            return new SkyNetAuthTicketDto
            {
                Handle = handle,
                TicketBase64 = payload,
                TicketSize = (uint)System.Text.Encoding.UTF8.GetByteCount(payload)
            };
        }
    }

    public SkyNetAuthValidateResultDto ValidateTicket(SkyNetAuthValidateRequestDto request)
    {
        lock (_sync)
        {
            var ticket = _tickets.Values.FirstOrDefault(t => t.TicketBase64 == request.TicketBase64 && t.AppId == request.AppId);
            if (ticket == null)
            {
                if (!string.IsNullOrWhiteSpace(request.TicketBase64))
                {
                    return new SkyNetAuthValidateResultDto
                    {
                        Success = true,
                        BeginAuthSessionResult = BeginAuthOk,
                        AuthSessionResponse = AuthResponseOk,
                        OwnerSteamId = request.SteamId
                    };
                }

                return new SkyNetAuthValidateResultDto
                {
                    Success = false,
                    BeginAuthSessionResult = BeginAuthInvalidTicket,
                    AuthSessionResponse = AuthResponseInvalidTicket,
                    OwnerSteamId = request.SteamId
                };
            }

            return new SkyNetAuthValidateResultDto
            {
                Success = true,
                BeginAuthSessionResult = BeginAuthOk,
                AuthSessionResponse = AuthResponseOk,
                OwnerSteamId = ticket.SteamId
            };
        }
    }

    public SkyNetConnectAuthResultDto ConnectAndAuthenticate(SkyNetConnectAuthRequestDto request)
    {
        var validation = ValidateTicket(new SkyNetAuthValidateRequestDto
        {
            AppId = request.AppId,
            SteamId = request.SteamId,
            TicketBase64 = request.AuthBlobBase64,
            GameServer = true
        });

        return new SkyNetConnectAuthResultDto
        {
            Success = validation.Success,
            SteamId = request.SteamId,
            OwnerSteamId = validation.OwnerSteamId,
            DenyReason = validation.Success ? 0 : 2,
            DenyMessage = validation.Success ? string.Empty : "Invalid auth ticket"
        };
    }

    public void EndAuthSession(SkyNetAuthEndSessionRequestDto request)
    {
    }

    public void CancelTicket(SkyNetCancelAuthTicketRequestDto request)
    {
        lock (_sync)
        {
            _tickets.Remove(request.Handle);
        }
    }

    public SkyNetGameServerResultDto RegisterGameServer(SkyNetGameServerStateDto request)
    {
        lock (_sync)
        {
            var server = request.Server ?? new SkyNetGameServerDto();
            var publicIp = server.IP != 0 ? server.IP : ToUInt32(IPAddress.Loopback);
            server.Flags &= ~ServerFlagSecure;
            server.GameTags = NormalizeGameServerTags(server.GameTags);

            _state.GameServers[(ulong)publicIp] = server;
            SaveState();
            return new SkyNetGameServerResultDto { Success = true, PublicIP = publicIp, Secure = 0, SteamId = (ulong)publicIp };
        }
    }

    public SkyNetGameServerResultDto LogOnGameServer(SkyNetGameServerStateDto request) => RegisterGameServer(request);

    public void LogOffGameServer()
    {
    }

    public bool UpdateGameServerState(SkyNetGameServerDto server)
    {
        lock (_sync)
        {
            server.Flags &= ~ServerFlagSecure;
            server.GameTags = NormalizeGameServerTags(server.GameTags);
            _state.GameServers[(ulong)(server.IP == 0 ? ToUInt32(IPAddress.Loopback) : server.IP)] = server;
            SaveState();
            return true;
        }
    }

    public bool HeartbeatGameServer(SkyNetGameServerDto server) => UpdateGameServerState(server);

    public SkyNetGameServerPublicIpDto GetPublicIp()
    {
        lock (_sync)
        {
            var ip = _state.GameServers.Values.LastOrDefault()?.IP ?? ToUInt32(IPAddress.Loopback);
            return new SkyNetGameServerPublicIpDto { PublicIP = ip };
        }
    }

    public bool DisconnectGameServerUser(ulong steamId) => true;

    public bool UpdateGameServerUserData(SkyNetGameServerUserDataDto request)
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

    public bool SendP2P(string token, SkyNetP2PPacketSendDto request)
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

    public bool SendP2PBatch(string token, SkyNetP2PPacketBatchDto request)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return false;
            }

            foreach (var packet in request.Packets ?? new List<SkyNetP2PPacketSendDto>())
            {
                if (packet?.RemoteSteamId != 0)
                {
                    EnqueueP2PLocked(session!, packet);
                }
            }

            return true;
        }
    }

    private void EnqueueP2PLocked(ApiSession session, SkyNetP2PPacketSendDto request)
    {
        EnqueueEvent(request.RemoteSteamId, new SkyNetEventDto
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

    public bool SendGCMessage(string token, SkyNetGCMessageDto request)
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

    public SkyNetGCExchangeResponseDto? ExchangeGCMessage(string token, SkyNetGCExchangeRequestDto request)
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
                PersonaName = user?.PersonaName ?? sessionUser?.PersonaName ?? string.Empty
            };

            return _gameCoordinatorPlugins.Exchange(context, request);
        }
    }

    public SkyNetGCExchangeResponseDto? PollGCMessages(string token, SkyNetGCPollRequestDto request)
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
                PersonaName = user?.PersonaName ?? sessionUser?.PersonaName ?? string.Empty
            };

            return _gameCoordinatorPlugins.Poll(context);
        }
    }
}
