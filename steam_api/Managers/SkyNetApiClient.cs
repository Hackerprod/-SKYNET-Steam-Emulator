using SKYNET.Helpers;
using SKYNET.Helpers.JSON;
using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace SKYNET.Managers
{
    public static class SkyNetApiClient
    {
        private static readonly object ClientLock = new object();
        private static readonly TimeSpan RefreshWindow = TimeSpan.FromSeconds(15);
        private static readonly TimeSpan MissingUserProfileWindow = TimeSpan.FromMinutes(5);
        private static HttpClient Client;
        private static JavaScriptSerializer Serializer;
        private static bool Configured;
        private static readonly ConcurrentDictionary<ulong, DateTime> MissingUserProfiles = new ConcurrentDictionary<ulong, DateTime>();
        private static readonly ConcurrentDictionary<ulong, byte> PendingUserProfileRefreshes = new ConcurrentDictionary<ulong, byte>();
        private static int FriendsRefreshQueued;
        private static int SelfRefreshQueued;
        private static int SessionHandshakeRunning;
        private static readonly ConcurrentDictionary<string, string> PendingPresence = new ConcurrentDictionary<string, string>();
        private static readonly AutoResetEvent PresenceSignal = new AutoResetEvent(false);
        private static int PresenceDispatcherStarted;

        // True once a server session token exists. Safe to read on the game
        // thread without blocking; the handshake that sets it runs in background.
        public static bool IsConnected => IsEnabled && !string.IsNullOrWhiteSpace(SteamEmulator.SkyNetAccessToken);
        private const int MaxP2PQueue = 2048;
        private const int MaxP2PBatch = 64;
        private static readonly ConcurrentQueue<SkyNetP2PPacketSendDto> P2PQueue = new ConcurrentQueue<SkyNetP2PPacketSendDto>();
        private static readonly AutoResetEvent P2PQueueSignal = new AutoResetEvent(false);
        private static int P2PQueueCount;
        private static int P2PDispatcherStarted;

        public static bool IsEnabled => SteamEmulator.UseServerApi &&
            (!string.IsNullOrWhiteSpace(SteamEmulator.SkyNetServerUrl) || SteamEmulator.SkyNetDiscoveryPort > 0);

        public static void Initialize()
        {
        }

        // Non-blocking on the caller (game) thread: returns whether a session
        // token already exists, and otherwise kicks off the handshake in the
        // background. The blocking network round-trip never runs on the game
        // thread, so latency to the server no longer stalls frames.
        public static bool EnsureSession()
        {
            if (!IsEnabled)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(SteamEmulator.SkyNetAccessToken))
            {
                return true;
            }

            QueueSessionHandshake();
            return false;
        }

        private static void QueueSessionHandshake()
        {
            if (!IsEnabled || !string.IsNullOrWhiteSpace(SteamEmulator.SkyNetAccessToken))
            {
                return;
            }

            if (Interlocked.Exchange(ref SessionHandshakeRunning, 1) == 1)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    if (EstablishSessionBlocking())
                    {
                        RefreshSelf(true);
                        RefreshFriends(true);
                        SteamEmulator.SteamUser?.OnServerSessionConnected();
                    }
                }
                catch (Exception ex)
                {
                    SteamEmulator.Write("SkyNetApiClient", $"Session handshake failed: {ex.Message}");
                }
                finally
                {
                    Interlocked.Exchange(ref SessionHandshakeRunning, 0);
                }
            });
        }

        // The actual blocking handshake. Only ever called from the background
        // worker above, never from the game thread.
        private static bool EstablishSessionBlocking()
        {
            if (!IsEnabled)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(SteamEmulator.SkyNetServerUrl))
            {
                var discovered = TryDiscoverServerUrl();
                if (string.IsNullOrWhiteSpace(discovered))
                {
                    return false;
                }

                SteamEmulator.SkyNetServerUrl = discovered;
                Configured = false;
            }

            if (!string.IsNullOrWhiteSpace(SteamEmulator.SkyNetAccessToken))
            {
                return true;
            }

            ConfigureClient();

            var request = new SkyNetSessionRequestDto
            {
                AccountId = SteamEmulator.SteamID.GetAccountID(),
                SteamId = (ulong)SteamEmulator.SteamID,
                AppId = SteamEmulator.AppID,
                PersonaName = SteamEmulator.PersonaName,
                ClientInstanceId = SteamEmulator.SkyNetClientInstanceId,
                UseActiveWebUser = SteamEmulator.SkyNetUseActiveWebUser
            };

            var session = Send<SkyNetSessionDto>(HttpMethod.Post, "api/auth/steam/session", request);
            if (session == null)
            {
                return false;
            }

            ApplySession(session);
            return true;
        }

        // Tell the server we are going offline (game shutting down) so friends
        // flip to offline immediately instead of waiting out the presence timeout.
        // Time-boxed and best-effort so shutdown never hangs on a slow server.
        public static void GoOffline()
        {
            if (!IsEnabled || string.IsNullOrWhiteSpace(SteamEmulator.SkyNetAccessToken))
            {
                return;
            }

            var thread = new Thread(() =>
            {
                try
                {
                    Send<VoidDto>(HttpMethod.Post, "api/presence/offline");
                }
                catch
                {
                }
            })
            {
                IsBackground = true
            };

            thread.Start();
            thread.Join(1500);
        }

        public static bool RefreshSelf(bool force = false)
        {
            if (!IsEnabled)
            {
                return false;
            }

            if (!force && !SkyNetStateCache.NeedsSelfRefresh(RefreshWindow))
            {
                return true;
            }

            if (!EnsureSession())
            {
                return false;
            }

            var self = Send<SkyNetUserDto>(HttpMethod.Get, "api/users/me");
            if (self == null)
            {
                return false;
            }

            SkyNetStateCache.ApplySelf(self);
            return true;
        }

        public static bool RefreshFriends(bool force = false)
        {
            if (!IsEnabled)
            {
                return false;
            }

            if (!force && !SkyNetStateCache.NeedsFriendsRefresh(RefreshWindow))
            {
                return true;
            }

            if (!EnsureSession())
            {
                SkyNetStateCache.ClearFriends();
                return false;
            }

            var friends = Send<List<SkyNetUserDto>>(HttpMethod.Get, "api/friends");
            if (friends == null)
            {
                SkyNetStateCache.ClearFriends();
                return false;
            }

            SkyNetStateCache.ApplyFriends(friends);
            return true;
        }

        public static void QueueSelfRefresh(bool force = false)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (!force && !SkyNetStateCache.NeedsSelfRefresh(RefreshWindow))
            {
                return;
            }

            if (Interlocked.Exchange(ref SelfRefreshQueued, 1) == 1)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    if (RefreshSelf(force) && SteamEmulator.SteamFriends != null)
                    {
                        SteamEmulator.SteamFriends.ReportUserChanged(
                            (ulong)SteamEmulator.SteamID,
                            EPersonaChange.k_EPersonaChangeName |
                            EPersonaChange.k_EPersonaChangeAvatar |
                            EPersonaChange.k_EPersonaChangeStatus);
                    }
                }
                finally
                {
                    Interlocked.Exchange(ref SelfRefreshQueued, 0);
                }
            });
        }

        public static void QueueFriendsRefresh(bool force = false)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (!force && !SkyNetStateCache.NeedsFriendsRefresh(RefreshWindow))
            {
                return;
            }

            if (Interlocked.Exchange(ref FriendsRefreshQueued, 1) == 1)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    if (RefreshFriends(force))
                    {
                        ReportCachedFriendsChanged();
                    }
                }
                finally
                {
                    Interlocked.Exchange(ref FriendsRefreshQueued, 0);
                }
            });
        }

        public static bool QueueUserProfileRefresh(ulong steamId, bool refreshFriends = false)
        {
            if (!IsEnabled || steamId == 0 || steamId == ulong.MaxValue)
            {
                return false;
            }

            if (steamId == (ulong)SteamEmulator.SteamID)
            {
                QueueSelfRefresh();
                return false;
            }

            if (SkyNetStateCache.TryGetFriend(steamId, out _))
            {
                return false;
            }

            if (IsUserProfileKnownMissing(steamId))
            {
                return false;
            }

            if (!PendingUserProfileRefreshes.TryAdd(steamId, 0))
            {
                return false;
            }

            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    if (RefreshUserProfile(steamId) && SteamEmulator.SteamFriends != null)
                    {
                        SteamEmulator.SteamFriends.ReportUserChanged(
                            steamId,
                            EPersonaChange.k_EPersonaChangeName |
                            EPersonaChange.k_EPersonaChangeAvatar |
                            EPersonaChange.k_EPersonaChangeRelationshipChanged |
                            EPersonaChange.k_EPersonaChangeGamePlayed);
                    }

                    if (refreshFriends)
                    {
                        RefreshFriends();
                    }
                }
                finally
                {
                    PendingUserProfileRefreshes.TryRemove(steamId, out _);
                }
            });

            return true;
        }

        public static bool IsUserProfileKnownMissing(ulong steamId)
        {
            if (!MissingUserProfiles.TryGetValue(steamId, out var expiresAt))
            {
                return false;
            }

            if (DateTime.UtcNow < expiresAt)
            {
                return true;
            }

            MissingUserProfiles.TryRemove(steamId, out _);
            return false;
        }

        public static bool RefreshUserProfile(ulong steamId)
        {
            if (!IsEnabled)
            {
                return false;
            }

            if (IsUserProfileKnownMissing(steamId))
            {
                return false;
            }

            EnsureSession();
            HttpStatusCode? statusCode;
            var user = Send<SkyNetUserDto>(HttpMethod.Get, $"api/users/{steamId}", null, out statusCode);
            if (user == null)
            {
                if (statusCode == HttpStatusCode.NotFound)
                {
                    MissingUserProfiles[steamId] = DateTime.UtcNow + MissingUserProfileWindow;
                }

                return false;
            }

            MissingUserProfiles.TryRemove(steamId, out _);
            SkyNetStateCache.UpsertUser(user);
            return true;
        }

        public static bool SendFriendRequest(ulong steamId)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            var ok = Send<VoidDto>(HttpMethod.Post, "api/friends/request", new SkyNetFriendActionRequestDto
            {
                SteamId = steamId
            }) != null;

            if (ok)
            {
                RefreshFriends(true);
            }

            return ok;
        }

        public static bool AcceptFriendRequest(ulong steamId)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            var ok = Send<VoidDto>(HttpMethod.Post, $"api/friends/{steamId}/accept") != null;
            if (ok)
            {
                RefreshFriends(true);
            }

            return ok;
        }

        public static bool RemoveFriendOrRequest(ulong steamId)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            var ok = Send<VoidDto>(HttpMethod.Post, $"api/friends/{steamId}/remove") != null;
            if (ok)
            {
                RefreshFriends(true);
            }

            return ok;
        }

        public static bool RefreshCurrentStats(bool force = false)
        {
            return RefreshStatsForUser((ulong)SteamEmulator.SteamID, "api/stats/me", force);
        }

        public static bool RefreshStatsForUser(ulong steamId, bool force = false)
        {
            return RefreshStatsForUser(steamId, $"api/stats/users/{steamId}", force);
        }

        public static List<SteamLobby> QueryLobbies(uint appId, Steamworks.Implementation.SteamMatchmaking.FilterLobby filter)
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            var response = Send<List<SkyNetLobbyDto>>(HttpMethod.Post, "api/lobbies/query", new SkyNetLobbyQueryRequestDto
            {
                AppId = appId,
                Distance = filter?.Distance ?? 0,
                SlotsAvailable = filter?.SlotsAvailable ?? 0,
                KeyToMatch = filter?.KeyToMatch,
                ValueToMatch = filter?.ValueToMatch ?? 0,
                ComparisonType = filter == null ? 0 : (int)filter.ComparisonType,
                StringValueToMatch = filter?.StringValueToMatch
            });

            return response == null ? null : MapLobbies(response);
        }

        public static SteamLobby CreateLobby(uint appId, int lobbyType, int maxMembers, Dictionary<string, string> lobbyData)
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            var response = Send<SkyNetLobbyDto>(HttpMethod.Post, "api/lobbies", new SkyNetCreateLobbyRequestDto
            {
                AppId = appId,
                LobbyType = lobbyType,
                MaxMembers = maxMembers,
                LobbyData = lobbyData ?? new Dictionary<string, string>()
            });

            return MapLobby(response);
        }

        public static SteamLobby JoinLobby(ulong lobbyId)
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            var response = Send<SkyNetLobbyDto>(HttpMethod.Post, $"api/lobbies/{lobbyId}/join");
            return MapLobby(response);
        }

        public static bool LeaveLobby(ulong lobbyId)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Post, $"api/lobbies/{lobbyId}/leave") != null;
        }

        public static SteamLobby RefreshLobby(ulong lobbyId)
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            var response = Send<SkyNetLobbyDto>(HttpMethod.Get, $"api/lobbies/{lobbyId}");
            return MapLobby(response);
        }

        public static bool UpdateLobbyData(ulong lobbyId, string key, string value)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Put, $"api/lobbies/{lobbyId}/data", new SkyNetLobbyDataUpdateRequestDto
            {
                Key = key,
                Value = value
            }) != null;
        }

        public static bool DeleteLobbyData(ulong lobbyId, string key)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Post, $"api/lobbies/{lobbyId}/data/delete", new SkyNetLobbyDeleteDataRequestDto
            {
                Key = key
            }) != null;
        }

        public static bool SetLobbyJoinable(ulong lobbyId, bool joinable)
        {
            return UpdateLobbySettings(lobbyId, new SkyNetLobbySettingsUpdateRequestDto
            {
                Joinable = joinable
            });
        }

        public static bool SetLobbyType(ulong lobbyId, int lobbyType)
        {
            return UpdateLobbySettings(lobbyId, new SkyNetLobbySettingsUpdateRequestDto
            {
                LobbyType = lobbyType
            });
        }

        public static bool SetLobbyOwner(ulong lobbyId, ulong ownerSteamId)
        {
            return UpdateLobbySettings(lobbyId, new SkyNetLobbySettingsUpdateRequestDto
            {
                OwnerSteamId = ownerSteamId
            });
        }

        public static bool SetLobbyMemberLimit(ulong lobbyId, int maxMembers)
        {
            return UpdateLobbySettings(lobbyId, new SkyNetLobbySettingsUpdateRequestDto
            {
                MaxMembers = maxMembers
            });
        }

        public static bool SetLobbyMemberData(ulong lobbyId, string key, string value)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Put, $"api/lobbies/{lobbyId}/member-data", new SkyNetLobbyDataUpdateRequestDto
            {
                Key = key,
                Value = value
            }) != null;
        }

        public static bool SetLobbyGameServer(ulong lobbyId, uint ip, uint port, ulong steamIdGameServer)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Put, $"api/lobbies/{lobbyId}/gameserver", new SkyNetLobbyGameServerUpdateRequestDto
            {
                IP = ip,
                Port = port,
                SteamIdGameServer = steamIdGameServer
            }) != null;
        }

        public static bool UploadRemoteStorageFile(string fileName, byte[] content)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Put, "api/storage/files", new SkyNetRemoteStorageFileDto
            {
                FileName = fileName,
                ContentBase64 = Convert.ToBase64String(content ?? new byte[0])
            }) != null;
        }

        public static SkyNetRemoteStorageFileDto DownloadRemoteStorageFile(string fileName)
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            return Send<SkyNetRemoteStorageFileDto>(HttpMethod.Get, $"api/storage/files/{Uri.EscapeDataString(fileName)}");
        }

        public static List<SkyNetRemoteStorageFileListItemDto> ListRemoteStorageFiles()
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            return Send<List<SkyNetRemoteStorageFileListItemDto>>(HttpMethod.Get, "api/storage/files");
        }

        public static bool DeleteRemoteStorageFile(string fileName)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Post, "api/storage/files/delete", new SkyNetRemoteStorageDeleteRequestDto
            {
                FileName = fileName
            }) != null;
        }

        public static SkyNetRemoteStorageShareDto ShareRemoteStorageFile(string fileName)
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            return Send<SkyNetRemoteStorageShareDto>(HttpMethod.Post, "api/storage/files/share", new SkyNetRemoteStorageDeleteRequestDto
            {
                FileName = fileName
            });
        }

        public static SkyNetRemoteStorageQuotaDto GetRemoteStorageQuota()
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            return Send<SkyNetRemoteStorageQuotaDto>(HttpMethod.Get, "api/storage/quota");
        }

        public static SkyNetAuthTicketDto CreateAuthSessionTicket(bool gameServer, int cbMaxTicket)
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            return Send<SkyNetAuthTicketDto>(HttpMethod.Post, "api/auth/tickets/session", new SkyNetAuthTicketRequestDto
            {
                AppId = SteamEmulator.AppID,
                SteamId = (ulong)(gameServer ? SteamEmulator.SteamID_GS : SteamEmulator.SteamID),
                GameServer = gameServer,
                TicketBufferSize = cbMaxTicket
            });
        }

        public static SkyNetAuthValidateResultDto ValidateAuthSessionTicket(byte[] ticket, ulong steamId, bool gameServer)
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            return Send<SkyNetAuthValidateResultDto>(HttpMethod.Post, "api/auth/tickets/validate", new SkyNetAuthValidateRequestDto
            {
                SteamId = steamId,
                TicketBase64 = Convert.ToBase64String(ticket ?? new byte[0]),
                GameServer = gameServer,
                AppId = SteamEmulator.AppID
            });
        }

        public static SkyNetConnectAuthResultDto ConnectAndAuthenticate(uint ipClient, byte[] authBlob, ulong steamId)
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            return Send<SkyNetConnectAuthResultDto>(HttpMethod.Post, "api/gameservers/users/connect", new SkyNetConnectAuthRequestDto
            {
                IpClient = ipClient,
                SteamId = steamId,
                AuthBlobBase64 = Convert.ToBase64String(authBlob ?? new byte[0]),
                AppId = SteamEmulator.AppID
            });
        }

        public static bool EndAuthSession(ulong steamId, bool gameServer)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Post, "api/auth/tickets/end-session", new SkyNetAuthEndSessionRequestDto
            {
                SteamId = steamId,
                GameServer = gameServer
            }) != null;
        }

        public static bool CancelAuthTicket(uint handle, bool gameServer)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Post, "api/auth/tickets/cancel", new SkyNetCancelAuthTicketRequestDto
            {
                Handle = handle,
                GameServer = gameServer
            }) != null;
        }

        public static SkyNetGameServerResultDto RegisterGameServer(GameServerData server)
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            return Send<SkyNetGameServerResultDto>(HttpMethod.Post, "api/gameservers/register", new SkyNetGameServerStateDto
            {
                Server = MapGameServer(server),
                Anonymous = false
            });
        }

        public static SkyNetGameServerResultDto LogOnGameServer(GameServerData server, string token, bool anonymous)
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            return Send<SkyNetGameServerResultDto>(HttpMethod.Post, "api/gameservers/logon", new SkyNetGameServerStateDto
            {
                Server = MapGameServer(server),
                Token = token,
                Anonymous = anonymous
            });
        }

        public static bool LogOffGameServer()
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Post, "api/gameservers/logoff") != null;
        }

        public static bool UpdateGameServerState(GameServerData server)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Put, "api/gameservers/state", MapGameServer(server)) != null;
        }

        public static bool HeartbeatGameServer(GameServerData server)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Post, "api/gameservers/heartbeat", MapGameServer(server)) != null;
        }

        public static uint GetGameServerPublicIP()
        {
            if (!IsEnabled)
            {
                return 0;
            }

            EnsureSession();
            var response = Send<SkyNetGameServerPublicIpDto>(HttpMethod.Get, "api/gameservers/public-ip");
            return response?.PublicIP ?? 0;
        }

        public static bool DisconnectGameServerUser(ulong steamId)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Post, "api/gameservers/users/disconnect", new SkyNetDisconnectGameServerUserDto
            {
                SteamId = steamId
            }) != null;
        }

        public static bool UpdateGameServerUserData(ulong steamId, string playerName, uint score)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Put, "api/gameservers/users/data", new SkyNetGameServerUserDataDto
            {
                SteamId = steamId,
                PlayerName = playerName,
                Score = score
            }) != null;
        }

        public static SkyNetStatsEnvelopeDto RequestGameServerUserStats(ulong steamId)
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            return Send<SkyNetStatsEnvelopeDto>(HttpMethod.Get, $"api/gameservers/stats/users/{steamId}");
        }

        public static bool StoreGameServerUserStats(ulong steamId, List<SkyNetStatDto> stats, List<SkyNetAchievementDto> achievements)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Put, $"api/gameservers/stats/users/{steamId}", new SkyNetStoreStatsRequestDto
            {
                SteamId = steamId,
                Stats = stats ?? new List<SkyNetStatDto>(),
                Achievements = achievements ?? new List<SkyNetAchievementDto>()
            }) != null;
        }

        public static bool SendP2PPacket(ulong remoteSteamId, byte[] buffer, int sendType, int channel)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            StartP2PDispatcher();
            var queuedCount = Interlocked.Increment(ref P2PQueueCount);
            if (queuedCount > MaxP2PQueue)
            {
                if (sendType == 0 || sendType == 1 || queuedCount > MaxP2PQueue * 2)
                {
                    Interlocked.Decrement(ref P2PQueueCount);
                    SteamEmulator.Write("SkyNetApiClient", $"Dropping P2P packet because queue is saturated (RemoteSteamId = {remoteSteamId}, SendType = {sendType}, Queue = {queuedCount})");
                    return sendType == 0 || sendType == 1;
                }
            }

            P2PQueue.Enqueue(new SkyNetP2PPacketSendDto
            {
                RemoteSteamId = remoteSteamId,
                BufferBase64 = Convert.ToBase64String(buffer ?? new byte[0]),
                SendType = sendType,
                Channel = channel
            });
            P2PQueueSignal.Set();
            return true;
        }

        public static bool SendGCMessage(uint messageType, byte[] buffer)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Post, "api/gamecoordinator/messages", new SkyNetGCMessageDto
            {
                MessageType = messageType,
                PayloadBase64 = Convert.ToBase64String(buffer ?? new byte[0])
            }) != null;
        }

        public static SdrCertDto RequestSdrCert()
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            return Send<SdrCertDto>(HttpMethod.Post, "api/networking/sdr/cert", new SdrCertRequestDto
            {
                SteamId = (ulong)SteamEmulator.SteamID,
                AppId = SteamEmulator.AppID
            });
        }

        public static SkyNetGCExchangeResponseDto ExchangeGCMessage(uint appId, uint messageType, byte[] body, ulong sourceJobId, bool gameServerContext = false)
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            bool gameServerMessage = gameServerContext || IsGameServerGCMessage(messageType);
            return Send<SkyNetGCExchangeResponseDto>(HttpMethod.Post, "api/gamecoordinator/exchange", new SkyNetGCExchangeRequestDto
            {
                AppId = appId,
                MessageType = messageType,
                BodyBase64 = Convert.ToBase64String(body ?? new byte[0]),
                SourceJobId = sourceJobId,
                SteamId = (ulong)(gameServerMessage ? SteamEmulator.SteamID_GS : SteamEmulator.SteamID),
                GameServer = gameServerMessage
            });
        }

        private static bool IsGameServerGCMessage(uint messageType)
        {
            switch (messageType)
            {
                case 28:
                case 4007:
                case 4506:
                case 4508:
                case 4511:
                case 7004:
                case 7026:
                case 7034:
                case 7072:
                case 7088:
                case 7200:
                case 7381:
                case 7450:
                case 7497:
                case 7530:
                case 7531:
                case 8041:
                case 8255:
                case 8330:
                case 8870:
                    return true;
                default:
                    return false;
            }
        }

        public static SkyNetGCExchangeResponseDto PollGCMessages(uint appId, bool gameServer = false)
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            return Send<SkyNetGCExchangeResponseDto>(HttpMethod.Post, "api/gamecoordinator/poll", new SkyNetGCPollRequestDto
            {
                AppId = appId,
                SteamId = (ulong)(gameServer ? SteamEmulator.SteamID_GS : SteamEmulator.SteamID),
                GameServer = gameServer
            });
        }

        public static bool StoreStats()
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();

            var request = new SkyNetStoreStatsRequestDto
            {
                SteamId = (ulong)SteamEmulator.SteamID,
                Stats = SkyNetStateCache.GetStats((ulong)SteamEmulator.SteamID).ConvertAll(stat => new SkyNetStatDto
                {
                    Name = stat.Name,
                    Data = stat.Data
                }),
                Achievements = SkyNetStateCache.GetAchievements((ulong)SteamEmulator.SteamID).ConvertAll(achievement => new SkyNetAchievementDto
                {
                    Name = achievement.Name,
                    Earned = achievement.Earned,
                    Date = achievement.Date,
                    Progress = achievement.Progress,
                    MaxProgress = achievement.MaxProgress
                })
            };

            var stored = Send<VoidDto>(HttpMethod.Put, "api/stats/me", request) != null;
            if (stored)
            {
                SkyNetStateCache.MarkStatsStored();
            }

            return stored;
        }

        public static bool UpdatePersonaName(string personaName)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            var response = Send<SkyNetUserDto>(new HttpMethod("PATCH"), "api/users/me/persona", new SkyNetPersonaUpdateDto
            {
                PersonaName = personaName
            });

            if (response == null)
            {
                return false;
            }

            SkyNetStateCache.ApplySelf(response);
            return true;
        }

        // Non-blocking: the game sets rich presence dozens of times per update.
        // We only stash the latest value per key (coalescing) and let a
        // background thread push it to the server, so this never stalls a frame.
        public static bool SetRichPresence(string key, string value)
        {
            if (!IsEnabled || key == null)
            {
                return false;
            }

            PendingPresence[key] = value ?? string.Empty;
            StartPresenceDispatcher();
            PresenceSignal.Set();
            return true;
        }

        private static void StartPresenceDispatcher()
        {
            if (Interlocked.Exchange(ref PresenceDispatcherStarted, 1) == 1)
            {
                return;
            }

            var thread = new Thread(PresenceDispatchLoop)
            {
                IsBackground = true,
                Name = "SkyNetPresence"
            };
            thread.Start();
        }

        private static void PresenceDispatchLoop()
        {
            while (true)
            {
                try
                {
                    PresenceSignal.WaitOne(250);

                    if (!IsEnabled || PendingPresence.IsEmpty)
                    {
                        continue;
                    }

                    // Wait until the session is up; keep the pending values so
                    // the latest presence is sent once we are connected.
                    if (!EnsureSession())
                    {
                        Thread.Sleep(200);
                        continue;
                    }

                    foreach (var key in PendingPresence.Keys)
                    {
                        if (!PendingPresence.TryRemove(key, out var value))
                        {
                            continue;
                        }

                        try
                        {
                            Send<VoidDto>(HttpMethod.Put, "api/presence", new SkyNetPresenceUpdateDto
                            {
                                Key = key,
                                Value = value
                            });
                        }
                        catch
                        {
                            // Presence is ephemeral; drop on failure. The game
                            // re-sends it constantly, so it self-heals.
                        }
                    }
                }
                catch
                {
                    Thread.Sleep(200);
                }
            }
        }

        public static bool RefreshAvatar(ulong steamId)
        {
            if (!IsEnabled)
            {
                return false;
            }

            try
            {
                ConfigureClient();
                EnsureSession();

                using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/users/{steamId}/avatar"))
                {
                    ApplyAuthorization(request);
                    using (var response = GetClient().SendAsync(request).GetAwaiter().GetResult())
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            return false;
                        }

                        if (response.Headers.TryGetValues("X-SKYNET-Avatar-SteamId", out var steamIdHeaders))
                        {
                            var returnedSteamId = steamIdHeaders.FirstOrDefault();
                            if (!ulong.TryParse(returnedSteamId, out var parsedSteamId) || parsedSteamId != steamId)
                            {
                                SteamEmulator.Write("SkyNetApiClient", $"Rejected avatar identity mismatch requested={steamId} returned={returnedSteamId}");
                                return false;
                            }
                        }

                        var isDefault = response.Headers.TryGetValues("X-SKYNET-Avatar-Default", out var defaultHeaders) &&
                            bool.TryParse(defaultHeaders.FirstOrDefault(), out var parsedDefault) && parsedDefault;

                        var data = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                        using (var stream = new MemoryStream(data))
                        {
                            var avatar = (Bitmap)Image.FromStream(stream);
                            SteamEmulator.SteamFriends.AddOrUpdateAvatar(avatar, steamId);
                            if (isDefault)
                            {
                                SteamEmulator.SteamFriends.RemoveCachedAvatar(steamId);
                            }
                            else
                            {
                                SteamEmulator.SteamFriends.StoreCachedAvatar(avatar, steamId);
                            }
                            avatar.Dispose();
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("SkyNetApiClient", $"RefreshAvatar {ex.Message}");
                return false;
            }
        }

        public static bool UploadSelfAvatar(Bitmap avatar)
        {
            if (!IsEnabled || avatar == null || string.IsNullOrWhiteSpace(SteamEmulator.SkyNetAccessToken))
            {
                return false;
            }

            try
            {
                ConfigureClient();
                byte[] data;
                using (var resized = ImageHelper.Resize(avatar, 184, 184))
                using (var stream = new MemoryStream())
                {
                    resized.Save(stream, ImageFormat.Png);
                    data = stream.ToArray();
                }

                return Send<VoidDto>(HttpMethod.Put, "api/users/me/avatar", new SkyNetAvatarUpdateDto
                {
                    ContentBase64 = Convert.ToBase64String(data)
                }) != null;
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("SkyNetApiClient", $"UploadSelfAvatar {ex.Message}");
                return false;
            }
        }

        public static SkyNetEventEnvelopeDto PollEvents(int waitMs = 0)
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            var cursor = Uri.EscapeDataString(SkyNetStateCache.GetEventCursor());
            var envelope = Send<SkyNetEventEnvelopeDto>(HttpMethod.Get, $"api/events?since={cursor}&waitMs={Math.Max(0, waitMs)}");
            if (envelope != null && !string.IsNullOrWhiteSpace(envelope.Cursor))
            {
                SkyNetStateCache.SetEventCursor(envelope.Cursor);
            }

            return envelope;
        }

        private static bool RefreshStatsForUser(ulong steamId, string path, bool force)
        {
            if (!IsEnabled)
            {
                return false;
            }

            if (!force && !SkyNetStateCache.NeedsStatsRefresh(steamId, RefreshWindow))
            {
                return true;
            }

            EnsureSession();
            var response = Send<SkyNetStatsEnvelopeDto>(HttpMethod.Get, path);
            if (response == null)
            {
                return false;
            }

            SkyNetStateCache.ApplyStats(steamId, response.Stats);
            SkyNetStateCache.ApplyAchievements(steamId, response.Achievements);
            SkyNetStateCache.SetCurrentPlayers(response.CurrentPlayers);
            return true;
        }

        private static bool UpdateLobbySettings(ulong lobbyId, SkyNetLobbySettingsUpdateRequestDto request)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Put, $"api/lobbies/{lobbyId}/settings", request) != null;
        }

        private static SkyNetGameServerDto MapGameServer(GameServerData server)
        {
            if (server == null)
            {
                return null;
            }

            return new SkyNetGameServerDto
            {
                SteamId = (ulong)SteamEmulator.SteamID_GS,
                AppId = server.AppId,
                IP = server.IP,
                Port = server.Port,
                QueryPort = server.QueryPort,
                Flags = server.Flags,
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
                KeyValues = server.KeyValues == null ? new Dictionary<string, string>() : new Dictionary<string, string>(server.KeyValues)
            };
        }

        private static List<SteamLobby> MapLobbies(IEnumerable<SkyNetLobbyDto> lobbies)
        {
            var mapped = new List<SteamLobby>();
            if (lobbies == null)
            {
                return mapped;
            }

            foreach (var lobby in lobbies)
            {
                var mappedLobby = MapLobby(lobby);
                if (mappedLobby != null)
                {
                    mapped.Add(mappedLobby);
                }
            }

            return mapped;
        }

        internal static SteamLobby MapLobbyForEvents(SkyNetLobbyDto lobby)
        {
            return MapLobby(lobby);
        }

        private static SteamLobby MapLobby(SkyNetLobbyDto lobby)
        {
            if (lobby == null)
            {
                return null;
            }

            var mapped = new SteamLobby
            {
                SteamID = lobby.SteamId,
                AppID = lobby.AppId,
                Owner = lobby.OwnerSteamId,
                Type = (ELobbyType)lobby.LobbyType,
                MaxMembers = lobby.MaxMembers,
                Joinable = lobby.Joinable,
                LobbyData = lobby.LobbyData ?? new Dictionary<string, string>(),
                Gameserver = new SteamLobby.LobbyGameserver
                {
                    SteamID = lobby.GameServer?.SteamId ?? 0,
                    IP = lobby.GameServer?.IP ?? 0,
                    Port = lobby.GameServer?.Port ?? 0,
                    Filled = lobby.GameServer != null && (lobby.GameServer.SteamId != 0 || lobby.GameServer.IP != 0 || lobby.GameServer.Port != 0)
                }
            };

            if (lobby.Members != null)
            {
                foreach (var member in lobby.Members)
                {
                    mapped.Members.Add(new SteamLobby.LobbyMember
                    {
                        m_SteamID = member.SteamId,
                        m_Data = member.Data == null
                            ? new List<SteamLobby.LobbyMetaData>()
                            : member.Data.ConvertAll(item => new SteamLobby.LobbyMetaData
                            {
                                m_Key = item.Key,
                                m_Value = item.Value
                            })
                    });
                }
            }

            return mapped;
        }

        private static void ApplySession(SkyNetSessionDto session)
        {
            SteamEmulator.SkyNetAccessToken = session.AccessToken ?? string.Empty;
            SteamEmulator.SkyNetRefreshToken = session.RefreshToken ?? string.Empty;

            if (session.User != null)
            {
                var previousSteamId = (ulong)SteamEmulator.SteamID;
                var sessionSteamId = session.User.SteamId != 0
                    ? session.User.SteamId
                    : (session.User.AccountId != 0 ? (ulong)new CSteamID(session.User.AccountId) : 0UL);
                if (sessionSteamId != 0 && previousSteamId != sessionSteamId)
                {
                    SteamEmulator.SteamID = new CSteamID(sessionSteamId);
                    SkyNetStateCache.ResetForIdentityChange();
                    SteamEmulator.SteamFriends?.OnIdentityChanged(previousSteamId, sessionSteamId);
                }

                if (!string.IsNullOrWhiteSpace(session.User.PersonaName))
                {
                    SteamEmulator.PersonaName = session.User.PersonaName;
                }

                SkyNetStateCache.ApplySelf(session.User);
            }

            ConfigureClient();

            if (SteamEmulator.SteamFriends != null)
            {
                SteamEmulator.SteamFriends.SyncSelfAvatarWithServer();
            }
        }

        private static void ConfigureClient()
        {
            if (!IsEnabled)
            {
                return;
            }

            lock (ClientLock)
            {
                if (!Uri.TryCreate(SteamEmulator.SkyNetServerUrl, UriKind.Absolute, out var baseAddress))
                {
                    return;
                }

                var client = GetClient();

                if (!Configured || client.BaseAddress == null || client.BaseAddress != baseAddress)
                {
                    client.BaseAddress = baseAddress;
                    client.Timeout = TimeSpan.FromMilliseconds(Math.Max(250, SteamEmulator.SkyNetHttpTimeoutMs));
                    Configured = true;
                }

            }
        }

        private static HttpClient GetClient()
        {
            lock (ClientLock)
            {
                if (Client == null)
                {
                    Client = new HttpClient(new HttpClientHandler { UseProxy = false })
                    {
                        Timeout = TimeSpan.FromMilliseconds(Math.Max(250, SteamEmulator.SkyNetHttpTimeoutMs))
                    };
                }

                return Client;
            }
        }

        private static JavaScriptSerializer GetSerializer()
        {
            if (Serializer == null)
            {
                Serializer = new JavaScriptSerializer();
            }

            return Serializer;
        }

        private static T Send<T>(HttpMethod method, string path, object body = null, bool retryOnUnauthorized = true)
            where T : class
        {
            HttpStatusCode? statusCode;
            return Send<T>(method, path, body, out statusCode, retryOnUnauthorized);
        }

        private static T Send<T>(HttpMethod method, string path, object body, out HttpStatusCode? statusCode, bool retryOnUnauthorized = true)
            where T : class
        {
            statusCode = null;
            try
            {
                ConfigureClient();
                using (var request = new HttpRequestMessage(method, path))
                {
                    ApplyAuthorization(request);

                    if (body != null)
                    {
                        request.Content = new StringContent(body.ToJson(), Encoding.UTF8, "application/json");
                    }

                    using (var response = GetClient().SendAsync(request).GetAwaiter().GetResult())
                    {
                        statusCode = response.StatusCode;
                        if (!response.IsSuccessStatusCode)
                        {
                            SteamEmulator.Write("SkyNetApiClient", $"{method} {path} failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}");
                            if (response.StatusCode == HttpStatusCode.Unauthorized)
                            {
                                ResetSession();
                                if (retryOnUnauthorized && !IsSessionEndpoint(path) && EnsureSession())
                                {
                                    return Send<T>(method, path, body, out statusCode, false);
                                }
                            }

                            return null;
                        }

                        if (typeof(T) == typeof(VoidDto))
                        {
                            return new VoidDto() as T;
                        }

                        var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        if (string.IsNullOrWhiteSpace(json))
                        {
                            return null;
                        }

                        return GetSerializer().Deserialize<T>(json);
                    }
                }
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("SkyNetApiClient", $"{method} {path} failed: {ex.Message}");
                return null;
            }
        }

        private static void ReportCachedFriendsChanged()
        {
            if (SteamEmulator.SteamFriends == null)
            {
                return;
            }

            foreach (var friend in SkyNetStateCache.GetFriends())
            {
                SteamEmulator.SteamFriends.ReportUserChanged(
                    friend.SteamID,
                    EPersonaChange.k_EPersonaChangeName |
                    EPersonaChange.k_EPersonaChangeAvatar |
                    EPersonaChange.k_EPersonaChangeRelationshipChanged |
                    EPersonaChange.k_EPersonaChangeGamePlayed);
            }
        }

        private static bool IsSessionEndpoint(string path)
        {
            return string.Equals(path, "api/auth/steam/session", StringComparison.OrdinalIgnoreCase);
        }

        private static void ResetSession()
        {
            SteamEmulator.SkyNetAccessToken = string.Empty;
            SteamEmulator.SkyNetRefreshToken = string.Empty;
        }

        private static void ApplyAuthorization(HttpRequestMessage request)
        {
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(SteamEmulator.SkyNetAccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SteamEmulator.SkyNetAccessToken);
            }
        }

        private static void StartP2PDispatcher()
        {
            if (Interlocked.Exchange(ref P2PDispatcherStarted, 1) == 1)
            {
                return;
            }

            var thread = new Thread(P2PDispatchLoop)
            {
                IsBackground = true,
                Name = "SKYNET P2P HTTP dispatcher"
            };
            thread.Start();
        }

        private static void P2PDispatchLoop()
        {
            while (true)
            {
                try
                {
                    P2PQueueSignal.WaitOne(25);
                    if (!IsEnabled || P2PQueueCount <= 0)
                    {
                        continue;
                    }

                    if (!EnsureSession())
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    var batch = new List<SkyNetP2PPacketSendDto>(MaxP2PBatch);
                    while (batch.Count < MaxP2PBatch && P2PQueue.TryDequeue(out var packet))
                    {
                        Interlocked.Decrement(ref P2PQueueCount);
                        batch.Add(packet);
                    }

                    if (batch.Count == 0)
                    {
                        continue;
                    }

                    if (Send<VoidDto>(HttpMethod.Post, "api/network/p2p/send-batch", new SkyNetP2PPacketBatchDto { Packets = batch }) == null)
                    {
                        foreach (var packet in batch)
                        {
                            Send<VoidDto>(HttpMethod.Post, "api/network/p2p/send", packet);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SteamEmulator.Write("SkyNetApiClient", $"P2P dispatch failed: {ex.Message}");
                    Thread.Sleep(100);
                }
            }
        }

        private static void DiscoverServerIfNeeded()
        {
            if (!SteamEmulator.UseServerApi || IsConfiguredServerReachable())
            {
                return;
            }

            var discovered = TryDiscoverServerUrl();
            if (!string.IsNullOrWhiteSpace(discovered))
            {
                SteamEmulator.SkyNetServerUrl = discovered;
                Configured = false;
                SteamEmulator.Write("SkyNetApiClient", $"Discovered SKYNET server at {discovered}");
            }
        }

        private static bool IsConfiguredServerReachable()
        {
            if (string.IsNullOrWhiteSpace(SteamEmulator.SkyNetServerUrl))
            {
                return false;
            }

            try
            {
                var request = WebRequest.CreateHttp(SteamEmulator.SkyNetServerUrl);
                request.Method = "GET";
                request.Timeout = 300;
                request.ReadWriteTimeout = 300;
                using (var response = request.GetResponse())
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static string TryDiscoverServerUrl()
        {
            try
            {
                using (var udp = new UdpClient())
                {
                    udp.EnableBroadcast = true;
                    udp.Client.ReceiveTimeout = 750;
                    var payload = Encoding.UTF8.GetBytes("SKYNET_DISCOVER");
                    udp.Send(payload, payload.Length, new IPEndPoint(IPAddress.Broadcast, SteamEmulator.SkyNetDiscoveryPort));
                    var remote = new IPEndPoint(IPAddress.Any, 0);
                    var response = udp.Receive(ref remote);
                    var text = Encoding.UTF8.GetString(response).Trim();
                    const string prefix = "SKYNET_SERVER ";
                    return text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                        ? text.Substring(prefix.Length).Trim()
                        : string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public sealed class SkyNetSessionRequestDto
        {
            public uint AccountId { get; set; }
            public ulong SteamId { get; set; }
            public uint AppId { get; set; }
            public string PersonaName { get; set; }
            public string ClientInstanceId { get; set; }
            public bool UseActiveWebUser { get; set; }
        }

        public sealed class SkyNetSessionDto
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public SkyNetUserDto User { get; set; }
        }

        public sealed class SkyNetUserDto
        {
            public uint AccountId { get; set; }
            public ulong SteamId { get; set; }
            public string PersonaName { get; set; }
            public uint AppId { get; set; }
            public ulong LobbyId { get; set; }
            public bool HasFriend { get; set; }
            public int FriendRelationship { get; set; }
            public int PersonaState { get; set; }
            public int PlayerLevel { get; set; }
            public Dictionary<string, string> RichPresence { get; set; }
        }

        public sealed class SkyNetPersonaUpdateDto
        {
            public string PersonaName { get; set; }
        }

        public sealed class SkyNetPresenceUpdateDto
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public sealed class SkyNetAvatarUpdateDto
        {
            public string ContentBase64 { get; set; }
        }

        public sealed class SkyNetFriendActionRequestDto
        {
            public ulong SteamId { get; set; }
            public string Identifier { get; set; }
        }

        public sealed class SkyNetStatDto
        {
            public string Name { get; set; }
            public uint Data { get; set; }
        }

        public sealed class SkyNetAchievementDto
        {
            public string Name { get; set; }
            public bool Earned { get; set; }
            public DateTime Date { get; set; }
            public uint Progress { get; set; }
            public uint MaxProgress { get; set; }
        }

        public sealed class SkyNetStatsEnvelopeDto
        {
            public ulong SteamId { get; set; }
            public List<SkyNetStatDto> Stats { get; set; }
            public List<SkyNetAchievementDto> Achievements { get; set; }
            public int CurrentPlayers { get; set; }
        }

        public sealed class SkyNetStoreStatsRequestDto
        {
            public ulong SteamId { get; set; }
            public List<SkyNetStatDto> Stats { get; set; }
            public List<SkyNetAchievementDto> Achievements { get; set; }
        }

        public sealed class SkyNetEventEnvelopeDto
        {
            public string Cursor { get; set; }
            public List<SkyNetEventDto> Events { get; set; }
        }

        public sealed class SkyNetEventDto
        {
            public string Type { get; set; }
            public ulong SteamId { get; set; }
            public uint AccountId { get; set; }
            public string PersonaName { get; set; }
            public uint AppId { get; set; }
            public ulong LobbyId { get; set; }
            public int PersonaState { get; set; }
            public int ChangeFlags { get; set; }
            public Dictionary<string, string> RichPresence { get; set; }
            public string StatName { get; set; }
            public uint StatValue { get; set; }
            public string AchievementName { get; set; }
            public bool AchievementEarned { get; set; }
            public uint AchievementProgress { get; set; }
            public uint AchievementMaxProgress { get; set; }
            public SkyNetLobbyDto Lobby { get; set; }
            public string PayloadBase64 { get; set; }
            public uint MessageType { get; set; }
            public ulong? TargetJobId { get; set; }
            public bool Protobuf { get; set; }
            public int Channel { get; set; }
            public ulong RemoteSteamId { get; set; }
            public int FriendRelationship { get; set; }
            public string RequestId { get; set; }
        }

        public sealed class VoidDto
        {
        }

        public sealed class SkyNetLobbyQueryRequestDto
        {
            public uint AppId { get; set; }
            public int Distance { get; set; }
            public int SlotsAvailable { get; set; }
            public string KeyToMatch { get; set; }
            public int ValueToMatch { get; set; }
            public int ComparisonType { get; set; }
            public string StringValueToMatch { get; set; }
        }

        public sealed class SkyNetCreateLobbyRequestDto
        {
            public uint AppId { get; set; }
            public int LobbyType { get; set; }
            public int MaxMembers { get; set; }
            public Dictionary<string, string> LobbyData { get; set; }
        }

        public sealed class SkyNetLobbyDataUpdateRequestDto
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public sealed class SkyNetLobbyDeleteDataRequestDto
        {
            public string Key { get; set; }
        }

        public sealed class SkyNetLobbySettingsUpdateRequestDto
        {
            public bool? Joinable { get; set; }
            public int? LobbyType { get; set; }
            public ulong? OwnerSteamId { get; set; }
            public int? MaxMembers { get; set; }
        }

        public sealed class SkyNetLobbyGameServerUpdateRequestDto
        {
            public ulong SteamIdGameServer { get; set; }
            public uint IP { get; set; }
            public uint Port { get; set; }
        }

        public sealed class SkyNetLobbyDto
        {
            public ulong SteamId { get; set; }
            public uint AppId { get; set; }
            public ulong OwnerSteamId { get; set; }
            public int LobbyType { get; set; }
            public int MaxMembers { get; set; }
            public bool Joinable { get; set; }
            public Dictionary<string, string> LobbyData { get; set; }
            public List<SkyNetLobbyMemberDto> Members { get; set; }
            public SkyNetLobbyGameServerDto GameServer { get; set; }
        }

        public sealed class SkyNetLobbyMemberDto
        {
            public ulong SteamId { get; set; }
            public List<SkyNetLobbyMetaDataDto> Data { get; set; }
        }

        public sealed class SkyNetLobbyMetaDataDto
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public sealed class SkyNetLobbyGameServerDto
        {
            public ulong SteamId { get; set; }
            public uint IP { get; set; }
            public uint Port { get; set; }
        }

        public sealed class SkyNetRemoteStorageFileDto
        {
            public string FileName { get; set; }
            public string ContentBase64 { get; set; }
            public int Size { get; set; }
            public uint Timestamp { get; set; }
        }

        public sealed class SkyNetRemoteStorageFileListItemDto
        {
            public string FileName { get; set; }
            public int Size { get; set; }
            public uint Timestamp { get; set; }
        }

        public sealed class SkyNetRemoteStorageDeleteRequestDto
        {
            public string FileName { get; set; }
        }

        public sealed class SkyNetRemoteStorageShareDto
        {
            public ulong Handle { get; set; }
            public EResult Result { get; set; }
        }

        public sealed class SkyNetRemoteStorageQuotaDto
        {
            public ulong TotalBytes { get; set; }
            public ulong AvailableBytes { get; set; }
        }

        public sealed class SkyNetAuthTicketRequestDto
        {
            public uint AppId { get; set; }
            public ulong SteamId { get; set; }
            public bool GameServer { get; set; }
            public int TicketBufferSize { get; set; }
        }

        public sealed class SkyNetAuthTicketDto
        {
            public uint Handle { get; set; }
            public string TicketBase64 { get; set; }
            public uint TicketSize { get; set; }
        }

        public sealed class SkyNetAuthValidateRequestDto
        {
            public ulong SteamId { get; set; }
            public string TicketBase64 { get; set; }
            public bool GameServer { get; set; }
            public uint AppId { get; set; }
        }

        public sealed class SkyNetAuthValidateResultDto
        {
            public int BeginAuthSessionResult { get; set; }
            public int AuthSessionResponse { get; set; }
            public ulong OwnerSteamId { get; set; }
            public bool Success { get; set; }
        }

        public sealed class SkyNetConnectAuthRequestDto
        {
            public uint IpClient { get; set; }
            public ulong SteamId { get; set; }
            public string AuthBlobBase64 { get; set; }
            public uint AppId { get; set; }
        }

        public sealed class SkyNetConnectAuthResultDto
        {
            public bool Success { get; set; }
            public ulong SteamId { get; set; }
            public ulong OwnerSteamId { get; set; }
            public int DenyReason { get; set; }
            public string DenyMessage { get; set; }
        }

        public sealed class SkyNetAuthEndSessionRequestDto
        {
            public ulong SteamId { get; set; }
            public bool GameServer { get; set; }
        }

        public sealed class SkyNetCancelAuthTicketRequestDto
        {
            public uint Handle { get; set; }
            public bool GameServer { get; set; }
        }

        public sealed class SkyNetGameServerStateDto
        {
            public SkyNetGameServerDto Server { get; set; }
            public string Token { get; set; }
            public bool Anonymous { get; set; }
        }

        public sealed class SkyNetGameServerResultDto
        {
            public bool Success { get; set; }
            public uint PublicIP { get; set; }
            public byte Secure { get; set; }
            public ulong SteamId { get; set; }
        }

        public sealed class SkyNetGameServerPublicIpDto
        {
            public uint PublicIP { get; set; }
        }

        public sealed class SkyNetGameServerUserDataDto
        {
            public ulong SteamId { get; set; }
            public string PlayerName { get; set; }
            public uint Score { get; set; }
        }

        public sealed class SkyNetDisconnectGameServerUserDto
        {
            public ulong SteamId { get; set; }
        }

        public sealed class SkyNetGameServerDto
        {
            public ulong SteamId { get; set; }
            public uint AppId { get; set; }
            public uint IP { get; set; }
            public int Port { get; set; }
            public int QueryPort { get; set; }
            public uint Flags { get; set; }
            public string VersionString { get; set; }
            public string Product { get; set; }
            public string Description { get; set; }
            public string ModDir { get; set; }
            public bool Dedicated { get; set; }
            public int MaxPlayers { get; set; }
            public int BotPlayers { get; set; }
            public string ServerName { get; set; }
            public string MapName { get; set; }
            public bool PasswordProtected { get; set; }
            public uint SpectatorPort { get; set; }
            public string SpectatorServerName { get; set; }
            public string GameTags { get; set; }
            public string GameData { get; set; }
            public string Region { get; set; }
            public Dictionary<string, string> KeyValues { get; set; }
        }

        public sealed class SkyNetP2PPacketSendDto
        {
            public ulong RemoteSteamId { get; set; }
            public string BufferBase64 { get; set; }
            public int SendType { get; set; }
            public int Channel { get; set; }
        }

        public sealed class SkyNetP2PPacketBatchDto
        {
            public List<SkyNetP2PPacketSendDto> Packets { get; set; }
        }

        public sealed class SkyNetGCMessageDto
        {
            public uint AppId { get; set; }
            public uint MessageType { get; set; }
            public string PayloadBase64 { get; set; }
            public ulong? TargetJobId { get; set; }
            public bool Protobuf { get; set; }
        }

        public sealed class SdrCertRequestDto
        {
            public ulong SteamId { get; set; }
            public uint AppId { get; set; }
        }

        public sealed class SdrCertDto
        {
            public string CertBase64 { get; set; }
            public string SignatureBase64 { get; set; }
            public string PrivateKeyBase64 { get; set; }
            public string PublicKeyBase64 { get; set; }
            public ulong CaKeyId { get; set; }
        }

        public sealed class SkyNetGCExchangeRequestDto
        {
            public uint AppId { get; set; }
            public uint MessageType { get; set; }
            public string BodyBase64 { get; set; }
            public ulong SourceJobId { get; set; }
            public ulong SteamId { get; set; }
            public bool GameServer { get; set; }
        }

        public sealed class SkyNetGCPollRequestDto
        {
            public uint AppId { get; set; }
            public ulong SteamId { get; set; }
            public bool GameServer { get; set; }
        }

        public sealed class SkyNetGCExchangeResponseDto
        {
            public bool Handled { get; set; }
            public List<SkyNetGCMessageDto> Messages { get; set; }
        }
    }
}
