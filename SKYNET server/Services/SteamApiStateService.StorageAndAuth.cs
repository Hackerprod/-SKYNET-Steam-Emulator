using System.Net;
using SKYNET_server.Models;

namespace SKYNET_server.Services;

public sealed partial class SteamApiStateService
{
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
            _state.GameServers[(ulong)publicIp] = server;
            SaveState();
            return new SkyNetGameServerResultDto { Success = true, PublicIP = publicIp, Secure = (byte)(server.Flags & 1), SteamId = (ulong)publicIp };
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

            EnqueueEvent(request.RemoteSteamId, new SkyNetEventDto
            {
                Type = "p2p_packet",
                RemoteSteamId = session.SteamId,
                PayloadBase64 = request.BufferBase64,
                Channel = request.Channel
            });

            return true;
        }
    }

    public bool SendGCMessage(string token, SkyNetGCMessageDto request)
    {
        lock (_sync)
        {
            if (!TryGetSession(token, out var session))
            {
                return false;
            }

            EnqueueEvent(session.SteamId, new SkyNetEventDto
            {
                Type = "gc_message",
                SteamId = session.SteamId,
                MessageType = request.MessageType,
                PayloadBase64 = request.PayloadBase64
            });

            return true;
        }
    }
}
