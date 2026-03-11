using SKYNET.Helpers;
using SKYNET.Helpers.JSON;
using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Script.Serialization;

namespace SKYNET.Managers
{
    public static class SkyNetApiClient
    {
        private static readonly object ClientLock = new object();
        private static readonly HttpClient Client = new HttpClient();
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();
        private static readonly TimeSpan RefreshWindow = TimeSpan.FromSeconds(15);
        private static bool Configured;

        public static bool IsEnabled => SteamEmulator.UseServerApi && !string.IsNullOrWhiteSpace(SteamEmulator.SkyNetServerUrl);

        public static void Initialize()
        {
            ConfigureClient();
        }

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

            ConfigureClient();

            var request = new SkyNetSessionRequestDto
            {
                AccountId = SteamEmulator.SteamID.GetAccountID(),
                SteamId = (ulong)SteamEmulator.SteamID,
                AppId = SteamEmulator.AppID,
                PersonaName = SteamEmulator.PersonaName
            };

            var session = Send<SkyNetSessionDto>(HttpMethod.Post, "api/auth/steam/session", request);
            if (session == null)
            {
                return false;
            }

            ApplySession(session);
            return true;
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

            EnsureSession();
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

            EnsureSession();
            var friends = Send<List<SkyNetUserDto>>(HttpMethod.Get, "api/friends");
            if (friends == null)
            {
                return false;
            }

            SkyNetStateCache.ApplyFriends(friends);
            return true;
        }

        public static bool RefreshUserProfile(ulong steamId)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            var user = Send<SkyNetUserDto>(HttpMethod.Get, $"api/users/{steamId}");
            if (user == null)
            {
                return false;
            }

            SkyNetStateCache.UpsertUser(user);
            return true;
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
            return Send<VoidDto>(HttpMethod.Post, "api/network/p2p/send", new SkyNetP2PPacketSendDto
            {
                RemoteSteamId = remoteSteamId,
                BufferBase64 = Convert.ToBase64String(buffer ?? new byte[0]),
                SendType = sendType,
                Channel = channel
            }) != null;
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

        public static bool SetRichPresence(string key, string value)
        {
            if (!IsEnabled)
            {
                return false;
            }

            EnsureSession();
            return Send<VoidDto>(HttpMethod.Put, "api/presence", new SkyNetPresenceUpdateDto
            {
                Key = key,
                Value = value
            }) != null;
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
                    using (var response = Client.SendAsync(request).GetAwaiter().GetResult())
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            return false;
                        }

                        var data = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                        using (var stream = new MemoryStream(data))
                        {
                            var avatar = (Bitmap)Image.FromStream(stream);
                            SteamEmulator.SteamFriends.AddOrUpdateAvatar(avatar, steamId);
                            SteamEmulator.SteamRemoteStorage.StoreAvatar(avatar, steamId.GetAccountID());
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

        public static SkyNetEventEnvelopeDto PollEvents()
        {
            if (!IsEnabled)
            {
                return null;
            }

            EnsureSession();
            var cursor = Uri.EscapeDataString(SkyNetStateCache.GetEventCursor());
            var envelope = Send<SkyNetEventEnvelopeDto>(HttpMethod.Get, $"api/events?since={cursor}");
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
                SkyNetStateCache.ApplySelf(session.User);
            }

            ConfigureClient();
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

                if (!Configured || Client.BaseAddress == null || Client.BaseAddress != baseAddress)
                {
                    Client.BaseAddress = baseAddress;
                    Client.Timeout = TimeSpan.FromSeconds(5);
                    Configured = true;
                }

                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrWhiteSpace(SteamEmulator.SkyNetAccessToken))
                {
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SteamEmulator.SkyNetAccessToken);
                }
                else
                {
                    Client.DefaultRequestHeaders.Authorization = null;
                }
            }
        }

        private static T Send<T>(HttpMethod method, string path, object body = null)
            where T : class
        {
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

                    using (var response = Client.SendAsync(request).GetAwaiter().GetResult())
                    {
                        if (!response.IsSuccessStatusCode)
                        {
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

                        return Serializer.Deserialize<T>(json);
                    }
                }
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("SkyNetApiClient", $"{method} {path} failed: {ex.Message}");
                return null;
            }
        }

        private static void ApplyAuthorization(HttpRequestMessage request)
        {
            if (!string.IsNullOrWhiteSpace(SteamEmulator.SkyNetAccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SteamEmulator.SkyNetAccessToken);
            }
        }

        public sealed class SkyNetSessionRequestDto
        {
            public uint AccountId { get; set; }
            public ulong SteamId { get; set; }
            public uint AppId { get; set; }
            public string PersonaName { get; set; }
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
            public int Channel { get; set; }
            public ulong RemoteSteamId { get; set; }
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

        public sealed class SkyNetGCMessageDto
        {
            public uint MessageType { get; set; }
            public string PayloadBase64 { get; set; }
        }
    }
}
