using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SKYNET_server.Json;
using SKYNET_server.Models;
using SKYNET_server.Persistence.Entities;
using SKYNET_server.Services;

namespace SKYNET_server.Persistence;

/// <summary>
/// Maps between the in-memory <see cref="ApiState"/> working model and the
/// relational steam.db/dota.db tables. Both directions live here so migration
/// and runtime share one proven mapping.
///
/// The write path is split so the caller can build the entity snapshot under its
/// lock (fast, in-memory) and do the database I/O outside the lock. Each table is
/// content-hashed; only tables whose content changed since the previous write are
/// rewritten, so a flush that touched one aggregate does not churn the rest.
///
/// Ephemeral fields are intentionally not persisted: user presence/rich-presence
/// and relationship flags, in-flight game sessions, tickets and event queues.
/// </summary>
public static class StatePersistence
{
    private static readonly JsonSerializerOptions JsonOpts = SkynetJsonSerializerOptions.CreateCompatible();

    /// <summary>Full write (used by the importer and round-trip check).</summary>
    public static void Save(SteamDbContext steam, DotaDbContext dota, ApiState state, bool includeCatalog)
    {
        Write(steam, dota, BuildSnapshot(state, includeCatalog), previous: null);
    }

    /// <summary>
    /// In-memory projection of the durable state into entity lists. Pure and
    /// fast: call it under the state lock, then hand the result to
    /// <see cref="Write"/> outside the lock.
    /// </summary>
    public static StateSnapshot BuildSnapshot(ApiState state, bool includeCatalog)
    {
        var snapshot = new StateSnapshot { IncludeCatalog = includeCatalog };

        snapshot.Users.AddRange(state.Users.Values
            .GroupBy(u => u.SteamId).Select(g => g.Last())
            .Select(u => new UserRecord
            {
                SteamId = u.SteamId,
                AccountId = u.AccountId,
                PersonaName = u.PersonaName,
                AppId = u.AppId,
                PlayerLevel = u.PlayerLevel,
            }));

        var edges = new HashSet<(ulong, ulong)>();
        foreach (var (steamId, friends) in state.FriendLinks)
        {
            foreach (var friend in friends)
            {
                if (edges.Add((steamId, friend)))
                {
                    snapshot.Friends.Add(new FriendEdge { SteamId = steamId, FriendSteamId = friend });
                }
            }
        }

        foreach (var request in state.FriendRequests.GroupBy(r => r.Id).Select(g => g.Last()))
        {
            snapshot.FriendRequests.Add(new FriendRequestRecord
            {
                Id = request.Id,
                FromSteamId = request.FromSteamId,
                ToSteamId = request.ToSteamId,
                Status = request.Status,
                CreatedAt = request.CreatedAt,
                RespondedAt = request.RespondedAt,
            });
        }

        foreach (var (steamId, base64) in state.Avatars)
        {
            snapshot.Avatars.Add(new AvatarRecord { SteamId = steamId, Content = SafeFromBase64(base64) });
        }

        foreach (var (steamId, envelope) in state.Stats)
        {
            foreach (var stat in envelope.Stats.GroupBy(s => s.Name).Select(g => g.Last()))
            {
                snapshot.Stats.Add(new StatRecord { SteamId = steamId, Name = stat.Name, Data = stat.Data });
            }

            foreach (var ach in envelope.Achievements.GroupBy(a => a.Name).Select(g => g.Last()))
            {
                snapshot.Achievements.Add(new AchievementRecord
                {
                    SteamId = steamId,
                    Name = ach.Name,
                    Earned = ach.Earned,
                    Date = ach.Date,
                    Progress = ach.Progress,
                    MaxProgress = ach.MaxProgress,
                });
            }
        }

        foreach (var account in state.WebAccounts.Values.GroupBy(a => a.Username).Select(g => g.Last()))
        {
            snapshot.WebAccounts.Add(new WebAccountRecord
            {
                Username = account.Username,
                PasswordHash = account.PasswordHash,
                SteamId = account.SteamId,
                IsAdmin = account.IsAdmin,
                CreatedAt = account.CreatedAt,
                LastLoginAt = account.LastLoginAt,
            });
        }

        foreach (var (token, session) in state.WebSessions)
        {
            snapshot.WebSessions.Add(new WebSessionRecord
            {
                AccessToken = token,
                RefreshToken = session.RefreshToken,
                SteamId = session.SteamId,
                ClientInstanceId = session.ClientInstanceId,
                ProcessRole = session.ProcessRole,
                RemoteIp = session.RemoteIp,
                LastSeenUtc = session.LastSeenUtc,
                WebSession = session.WebSession,
                Persistent = session.Persistent,
                ExpiresAtUtc = session.ExpiresAtUtc,
            });
        }

        foreach (var server in state.GameServers.Values.GroupBy(s => s.SteamId).Select(g => g.Last()))
        {
            snapshot.GameServers.Add(new GameServerRecord
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
                KeyValues = new Dictionary<string, string>(server.KeyValues),
            });
        }

        foreach (var lobby in state.Lobbies.Values.GroupBy(l => l.SteamId).Select(g => g.Last()))
        {
            snapshot.Lobbies.Add(new LobbyRecord
            {
                SteamId = lobby.SteamId,
                AppId = lobby.AppId,
                OwnerSteamId = lobby.OwnerSteamId,
                LobbyType = lobby.LobbyType,
                MaxMembers = lobby.MaxMembers,
                Joinable = lobby.Joinable,
                LobbyData = new Dictionary<string, string>(lobby.LobbyData),
                Members = lobby.Members.Select(m => new LobbyMemberValue
                {
                    SteamId = m.SteamId,
                    Data = m.Data.ToDictionary(d => d.Key, d => d.Value),
                }).ToList(),
                GameServer = lobby.GameServer is null ? null : new LobbyGameServerValue
                {
                    SteamId = lobby.GameServer.SteamId,
                    IP = lobby.GameServer.IP,
                    Port = lobby.GameServer.Port,
                },
            });
        }

        foreach (var (key, file) in state.Files)
        {
            var normalizedName = SteamApiStateService.TryNormalizeRemotePath(file.FileName, out var norm) ? norm : (file.FileName ?? "").Replace('\\', '/').ToLowerInvariant();
            snapshot.RemoteFiles.Add(new RemoteFileRecord
            {
                OwnerSteamId = file.OwnerSteamId,
                AppId = file.AppId,
                NormalizedName = normalizedName,
                OriginalName = file.FileName ?? string.Empty,
                Content = SafeFromBase64(file.ContentBase64),
                Size = file.Size,
                Timestamp = file.Timestamp,
                Sha256 = file.Sha256 ?? string.Empty,
                SyncPlatforms = (long)file.SyncPlatforms,
                Version = file.Version,
                Persisted = file.Persisted,
                DeletedAt = file.DeletedAt
            });
        }

        foreach (var (handle, share) in state.FileShares)
        {
            snapshot.RemoteFileShares.Add(new RemoteFileShareRecord
            {
                Handle = handle,
                OwnerSteamId = share.OwnerSteamId,
                AppId = share.AppId,
                NormalizedName = share.NormalizedName
            });
        }

        var equipKeys = new HashSet<(ulong, uint, uint, uint)>();
        foreach (var list in state.DotaEquipment.Values)
        {
            foreach (var eq in list)
            {
                if (!equipKeys.Add((eq.SteamId, eq.HeroId, eq.SlotId, eq.DefIndex)))
                {
                    continue;
                }

                snapshot.DotaEquipment.Add(new DotaEquipmentRecord
                {
                    SteamId = eq.SteamId,
                    HeroId = eq.HeroId,
                    SlotId = eq.SlotId,
                    HeroName = eq.HeroName,
                    Slot = eq.Slot,
                    DefIndex = eq.DefIndex,
                    ItemId = eq.ItemId,
                    Style = eq.Style,
                    UpdatedAt = eq.UpdatedAt,
                });
            }
        }

        foreach (var (lobbyId, match) in state.DotaMatches)
        {
            var playerKeys = new HashSet<ulong>();
            snapshot.DotaMatches.Add(new DotaMatchRecord
            {
                LobbyId = lobbyId,
                MatchId = match.MatchId,
                ServerSteamId = match.ServerSteamId,
                Connect = match.Connect,
                State = match.State,
                GameState = match.GameState,
                GameStartTime = match.GameStartTime,
                Dedicated = match.Dedicated,
                UpdatedAt = match.UpdatedAt,
                Players = match.Players.Where(p => playerKeys.Add(p.SteamId)).Select(p => new DotaMatchPlayerRecord
                {
                    LobbyId = lobbyId,
                    SteamId = p.SteamId,
                    AccountId = p.AccountId,
                    PersonaName = p.PersonaName,
                    Team = p.Team,
                    Slot = p.Slot,
                    CoachTeam = p.CoachTeam,
                    HeroId = p.HeroId,
                    Equipment = p.Equipment.Select(ToEquipmentValue).ToList(),
                }).ToList(),
            });
        }

        foreach (var (name, heroId) in state.DotaHeroIds)
        {
            snapshot.DotaHeroIds.Add(new DotaHeroIdRecord { Name = name, HeroId = heroId });
        }

        foreach (var (heroName, slots) in state.DotaHeroSlots)
        {
            foreach (var (slotName, slotId) in slots)
            {
                snapshot.DotaHeroSlots.Add(new DotaHeroSlotRecord { HeroName = heroName, SlotName = slotName, SlotId = slotId });
            }
        }

        snapshot.Cosmetics = new CosmeticSettingsRecord
        {
            Id = 1,
            DotaPath = state.DotaCosmetics.DotaPath,
            ClientVersion = state.DotaCosmetics.ClientVersion,
            LastImportAt = state.DotaCosmetics.LastImportAt,
            LastImportStatus = state.DotaCosmetics.LastImportStatus,
            EquipmentVersion = state.DotaCosmetics.EquipmentVersion,
        };

        snapshot.AppState = new AppStateRecord { Id = 1, ActiveWebSteamId = state.ActiveWebSteamId };

        if (includeCatalog)
        {
            foreach (var item in state.DotaItems.Values.GroupBy(i => i.DefIndex).Select(g => g.Last()))
            {
                snapshot.DotaItems.Add(new DotaItemRecord
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
                    HeroNames = new List<string>(item.HeroNames),
                    HeroIds = new List<uint>(item.HeroIds),
                });
            }
        }

        return snapshot;
    }

    /// <summary>
    /// Writes the snapshot to steam.db and dota.db, rewriting only the tables whose content
    /// changed vs. <paramref name="previous"/>. Returns the new per-table hashes
    /// for the caller to remember. Pass <c>null</c> to force a full write.
    /// </summary>
    public static Dictionary<string, string> Write(
        SteamDbContext steam, DotaDbContext dota, StateSnapshot snapshot, IReadOnlyDictionary<string, string>? previous)
    {
        var steamTables = new List<TableWrite>
        {
            new("steam.Users", snapshot.Users, () => steam.Users.ExecuteDelete(), () => steam.Users.AddRange(snapshot.Users)),
            new("steam.Friends", snapshot.Friends, () => steam.Friends.ExecuteDelete(), () => steam.Friends.AddRange(snapshot.Friends)),
            new("steam.FriendRequests", snapshot.FriendRequests, () => steam.FriendRequests.ExecuteDelete(), () => steam.FriendRequests.AddRange(snapshot.FriendRequests)),
            new("steam.Avatars", snapshot.Avatars, () => steam.Avatars.ExecuteDelete(), () => steam.Avatars.AddRange(snapshot.Avatars)),
            new("steam.Stats", snapshot.Stats, () => steam.Stats.ExecuteDelete(), () => steam.Stats.AddRange(snapshot.Stats)),
            new("steam.Achievements", snapshot.Achievements, () => steam.Achievements.ExecuteDelete(), () => steam.Achievements.AddRange(snapshot.Achievements)),
            new("steam.WebAccounts", snapshot.WebAccounts, () => steam.WebAccounts.ExecuteDelete(), () => steam.WebAccounts.AddRange(snapshot.WebAccounts)),
            new("steam.WebSessions", snapshot.WebSessions, () => steam.WebSessions.ExecuteDelete(), () => steam.WebSessions.AddRange(snapshot.WebSessions)),
            new("steam.RemoteFiles", snapshot.RemoteFiles, () => steam.RemoteFiles.ExecuteDelete(), () => steam.RemoteFiles.AddRange(snapshot.RemoteFiles)),
            new("steam.RemoteFileShares", snapshot.RemoteFileShares, () => steam.RemoteFileShares.ExecuteDelete(), () => steam.RemoteFileShares.AddRange(snapshot.RemoteFileShares)),
            new("steam.AppState", new[] { snapshot.AppState }, () => steam.AppState.ExecuteDelete(), () => steam.AppState.Add(snapshot.AppState)),
        };

        var dotaTables = new List<TableWrite>
        {
            new("dota.GameServers", snapshot.GameServers, () => dota.GameServers.ExecuteDelete(), () => dota.GameServers.AddRange(snapshot.GameServers)),
            new("dota.Lobbies", snapshot.Lobbies, () => dota.Lobbies.ExecuteDelete(), () => dota.Lobbies.AddRange(snapshot.Lobbies)),
            new("dota.DotaEquipment", snapshot.DotaEquipment, () => dota.DotaEquipment.ExecuteDelete(), () => dota.DotaEquipment.AddRange(snapshot.DotaEquipment)),
            // DotaMatchPlayers are cascade-deleted with their match and inserted via the Players navigation.
            new("dota.DotaMatches", snapshot.DotaMatches, () => dota.DotaMatches.ExecuteDelete(), () => dota.DotaMatches.AddRange(snapshot.DotaMatches)),
            new("dota.DotaHeroIds", snapshot.DotaHeroIds, () => dota.DotaHeroIds.ExecuteDelete(), () => dota.DotaHeroIds.AddRange(snapshot.DotaHeroIds)),
            new("dota.DotaHeroSlots", snapshot.DotaHeroSlots, () => dota.DotaHeroSlots.ExecuteDelete(), () => dota.DotaHeroSlots.AddRange(snapshot.DotaHeroSlots)),
            new("dota.CosmeticSettings", new[] { snapshot.Cosmetics }, () => dota.CosmeticSettings.ExecuteDelete(), () => dota.CosmeticSettings.Add(snapshot.Cosmetics)),
        };

        if (snapshot.IncludeCatalog)
        {
            dotaTables.Add(new("dota.DotaItems", snapshot.DotaItems, () => dota.DotaItems.ExecuteDelete(), () => dota.DotaItems.AddRange(snapshot.DotaItems)));
        }

        var hashes = new Dictionary<string, string>(StringComparer.Ordinal);
        using var steamTx = steam.Database.BeginTransaction();
        using var dotaTx = dota.Database.BeginTransaction();
        var wroteSteam = WriteTables(steamTables, previous, hashes);
        var wroteDota = WriteTables(dotaTables, previous, hashes);

        if (wroteSteam)
        {
            steam.SaveChanges();
        }

        if (wroteDota)
        {
            dota.SaveChanges();
        }

        steamTx.Commit();
        dotaTx.Commit();
        return hashes;
    }

    private static bool WriteTables(
        IEnumerable<TableWrite> tables,
        IReadOnlyDictionary<string, string>? previous,
        Dictionary<string, string> hashes)
    {
        var wrote = false;
        foreach (var table in tables)
        {
            var hash = Hash(table.Rows);
            hashes[table.Name] = hash;
            if (previous is not null && previous.TryGetValue(table.Name, out var old) && old == hash)
            {
                continue;
            }

            table.Clear();
            table.Add();
            wrote = true;
        }

        return wrote;
    }

    /// <summary>Reads the durable tables back into a fresh <see cref="ApiState"/>.</summary>
    public static ApiState Load(SteamDbContext steam, DotaDbContext dota)
    {
        var state = new ApiState();

        foreach (var u in steam.Users.AsNoTracking())
        {
            state.Users[u.SteamId] = new ApiUser
            {
                SteamId = u.SteamId,
                AccountId = u.AccountId,
                PersonaName = u.PersonaName,
                AppId = u.AppId,
                PlayerLevel = u.PlayerLevel,
            };
        }

        foreach (var e in steam.Friends.AsNoTracking())
        {
            if (!state.FriendLinks.TryGetValue(e.SteamId, out var set))
            {
                set = new HashSet<ulong>();
                state.FriendLinks[e.SteamId] = set;
            }

            set.Add(e.FriendSteamId);
        }

        state.FriendRequests = steam.FriendRequests.AsNoTracking().Select(r => new ApiFriendRequest
        {
            Id = r.Id,
            FromSteamId = r.FromSteamId,
            ToSteamId = r.ToSteamId,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            RespondedAt = r.RespondedAt,
        }).ToList();

        foreach (var a in steam.Avatars.AsNoTracking())
        {
            state.Avatars[a.SteamId] = Convert.ToBase64String(a.Content);
        }

        foreach (var s in steam.Stats.AsNoTracking())
        {
            Envelope(state, s.SteamId).Stats.Add(new ApiStat { Name = s.Name, Data = s.Data });
        }

        foreach (var a in steam.Achievements.AsNoTracking())
        {
            Envelope(state, a.SteamId).Achievements.Add(new ApiAchievement
            {
                Name = a.Name,
                Earned = a.Earned,
                Date = a.Date,
                Progress = a.Progress,
                MaxProgress = a.MaxProgress,
            });
        }

        foreach (var w in steam.WebAccounts.AsNoTracking())
        {
            state.WebAccounts[w.Username] = new ApiWebAccount
            {
                Username = w.Username,
                PasswordHash = w.PasswordHash,
                SteamId = w.SteamId,
                IsAdmin = w.IsAdmin,
                CreatedAt = w.CreatedAt,
                LastLoginAt = w.LastLoginAt,
            };
        }

        foreach (var s in steam.WebSessions.AsNoTracking())
        {
            state.WebSessions[s.AccessToken] = new ApiSession
            {
                AccessToken = s.AccessToken,
                RefreshToken = s.RefreshToken,
                SteamId = s.SteamId,
                ClientInstanceId = s.ClientInstanceId,
                ProcessRole = s.ProcessRole,
                RemoteIp = s.RemoteIp,
                LastSeenUtc = s.LastSeenUtc,
                WebSession = s.WebSession,
                Persistent = s.Persistent,
                ExpiresAtUtc = s.ExpiresAtUtc,
            };
        }

        foreach (var g in dota.GameServers.AsNoTracking())
        {
            state.GameServers[g.SteamId] = new ApiGameServer
            {
                SteamId = g.SteamId,
                AppId = g.AppId,
                IP = g.IP,
                Port = g.Port,
                QueryPort = g.QueryPort,
                Flags = g.Flags,
                Secure = g.Secure,
                VersionString = g.VersionString,
                Product = g.Product,
                Description = g.Description,
                ModDir = g.ModDir,
                Dedicated = g.Dedicated,
                MaxPlayers = g.MaxPlayers,
                BotPlayers = g.BotPlayers,
                ServerName = g.ServerName,
                MapName = g.MapName,
                PasswordProtected = g.PasswordProtected,
                SpectatorPort = g.SpectatorPort,
                SpectatorServerName = g.SpectatorServerName,
                GameTags = g.GameTags,
                GameData = g.GameData,
                Region = g.Region,
                KeyValues = new Dictionary<string, string>(g.KeyValues),
            };
        }

        foreach (var l in dota.Lobbies.AsNoTracking())
        {
            state.Lobbies[l.SteamId] = new ApiLobby
            {
                SteamId = l.SteamId,
                AppId = l.AppId,
                OwnerSteamId = l.OwnerSteamId,
                LobbyType = l.LobbyType,
                MaxMembers = l.MaxMembers,
                Joinable = l.Joinable,
                LobbyData = new Dictionary<string, string>(l.LobbyData),
                Members = l.Members.Select(m => new ApiLobbyMember
                {
                    SteamId = m.SteamId,
                    Data = m.Data.Select(d => new ApiLobbyMetaData { Key = d.Key, Value = d.Value }).ToList(),
                }).ToList(),
                GameServer = l.GameServer is null ? null : new ApiLobbyGameServer
                {
                    SteamId = l.GameServer.SteamId,
                    IP = l.GameServer.IP,
                    Port = l.GameServer.Port,
                },
            };
        }

        foreach (var f in steam.RemoteFiles.AsNoTracking())
        {
            var key = SteamApiStateService.MakeRemoteStorageKey(f.OwnerSteamId, f.AppId, f.NormalizedName);
            var content = f.Content ?? Array.Empty<byte>();
            var sha256 = f.Sha256;
            if (string.IsNullOrEmpty(sha256))
            {
                sha256 = Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant();
            }

            state.Files[key] = new ApiRemoteStorageFile
            {
                OwnerSteamId = f.OwnerSteamId,
                AppId = f.AppId,
                FileName = f.OriginalName,
                ContentBase64 = Convert.ToBase64String(content),
                Size = f.Size,
                Timestamp = f.Timestamp,
                Sha256 = sha256,
                SyncPlatforms = (uint)f.SyncPlatforms,
                Version = f.Version,
                Persisted = f.Persisted,
                DeletedAt = f.DeletedAt
            };
        }

        foreach (var s in steam.RemoteFileShares.AsNoTracking())
        {
            state.FileShares[s.Handle] = new ApiRemoteStorageShareRecord
            {
                Handle = s.Handle,
                OwnerSteamId = s.OwnerSteamId,
                AppId = s.AppId,
                NormalizedName = s.NormalizedName
            };
        }

        foreach (var i in dota.DotaItems.AsNoTracking())
        {
            state.DotaItems[i.DefIndex] = new ApiDotaItem
            {
                DefIndex = i.DefIndex,
                Name = i.Name,
                Prefab = i.Prefab,
                Slot = i.Slot,
                Quality = i.Quality,
                QualityId = i.QualityId,
                Rarity = i.Rarity,
                RarityId = i.RarityId,
                ImageInventory = i.ImageInventory,
                IsDefault = i.IsDefault,
                IsTool = i.IsTool,
                IsBundle = i.IsBundle,
                HeroNames = new List<string>(i.HeroNames),
                HeroIds = new List<uint>(i.HeroIds),
            };
        }

        foreach (var e in dota.DotaEquipment.AsNoTracking())
        {
            if (!state.DotaEquipment.TryGetValue(e.SteamId, out var list))
            {
                list = new List<ApiDotaEquipment>();
                state.DotaEquipment[e.SteamId] = list;
            }

            list.Add(new ApiDotaEquipment
            {
                SteamId = e.SteamId,
                HeroId = e.HeroId,
                SlotId = e.SlotId,
                HeroName = e.HeroName,
                Slot = e.Slot,
                DefIndex = e.DefIndex,
                ItemId = e.ItemId,
                Style = e.Style,
                UpdatedAt = e.UpdatedAt,
            });
        }

        foreach (var m in dota.DotaMatches.AsNoTracking().Include(x => x.Players))
        {
            state.DotaMatches[m.LobbyId] = new ApiDotaMatch
            {
                LobbyId = m.LobbyId,
                MatchId = m.MatchId,
                ServerSteamId = m.ServerSteamId,
                Connect = m.Connect,
                State = m.State,
                GameState = m.GameState,
                GameStartTime = m.GameStartTime,
                Dedicated = m.Dedicated,
                UpdatedAt = m.UpdatedAt,
                Players = m.Players.Select(p => new ApiDotaMatchPlayer
                {
                    SteamId = p.SteamId,
                    AccountId = p.AccountId,
                    PersonaName = p.PersonaName,
                    Team = p.Team,
                    Slot = p.Slot,
                    CoachTeam = p.CoachTeam,
                    HeroId = p.HeroId,
                    Equipment = p.Equipment.Select(FromEquipmentValue).ToList(),
                }).ToList(),
            };
        }

        foreach (var h in dota.DotaHeroIds.AsNoTracking())
        {
            state.DotaHeroIds[h.Name] = h.HeroId;
        }

        foreach (var s in dota.DotaHeroSlots.AsNoTracking())
        {
            if (!state.DotaHeroSlots.TryGetValue(s.HeroName, out var slots))
            {
                slots = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
                state.DotaHeroSlots[s.HeroName] = slots;
            }

            slots[s.SlotName] = s.SlotId;
        }

        var cosmetics = dota.CosmeticSettings.AsNoTracking().FirstOrDefault();
        if (cosmetics is not null)
        {
            state.DotaCosmetics = new ApiDotaCosmeticSettings
            {
                DotaPath = cosmetics.DotaPath,
                ClientVersion = cosmetics.ClientVersion,
                LastImportAt = cosmetics.LastImportAt,
                LastImportStatus = cosmetics.LastImportStatus,
                EquipmentVersion = cosmetics.EquipmentVersion,
            };
        }

        var appState = steam.AppState.AsNoTracking().FirstOrDefault();
        if (appState is not null)
        {
            state.ActiveWebSteamId = appState.ActiveWebSteamId;
        }

        return state;
    }

    private static string Hash(System.Collections.IEnumerable rows)
    {
        // Serialize with the concrete runtime type so every property is included.
        var json = JsonSerializer.SerializeToUtf8Bytes(rows, rows.GetType(), JsonOpts);
        return Convert.ToHexString(SHA256.HashData(json));
    }

    private static ApiStatsEnvelope Envelope(ApiState state, ulong steamId)
    {
        if (!state.Stats.TryGetValue(steamId, out var envelope))
        {
            envelope = new ApiStatsEnvelope { SteamId = steamId };
            state.Stats[steamId] = envelope;
        }

        return envelope;
    }

    private static DotaEquipmentValue ToEquipmentValue(ApiDotaEquipment e) => new()
    {
        SteamId = e.SteamId,
        HeroId = e.HeroId,
        HeroName = e.HeroName,
        Slot = e.Slot,
        SlotId = e.SlotId,
        DefIndex = e.DefIndex,
        ItemId = e.ItemId,
        Style = e.Style,
        UpdatedAt = e.UpdatedAt,
    };

    private static ApiDotaEquipment FromEquipmentValue(DotaEquipmentValue e) => new()
    {
        SteamId = e.SteamId,
        HeroId = e.HeroId,
        HeroName = e.HeroName,
        Slot = e.Slot,
        SlotId = e.SlotId,
        DefIndex = e.DefIndex,
        ItemId = e.ItemId,
        Style = e.Style,
        UpdatedAt = e.UpdatedAt,
    };

    private static byte[] SafeFromBase64(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Array.Empty<byte>();
        }

        try
        {
            return Convert.FromBase64String(value);
        }
        catch (FormatException)
        {
            return System.Text.Encoding.UTF8.GetBytes(value);
        }
    }

    private readonly record struct TableWrite(string Name, System.Collections.IEnumerable Rows, Action Clear, Action Add);
}

/// <summary>In-memory entity projection of the durable state (see BuildSnapshot).</summary>
public sealed class StateSnapshot
{
    public bool IncludeCatalog { get; set; }
    public List<UserRecord> Users { get; } = new();
    public List<FriendEdge> Friends { get; } = new();
    public List<FriendRequestRecord> FriendRequests { get; } = new();
    public List<AvatarRecord> Avatars { get; } = new();
    public List<StatRecord> Stats { get; } = new();
    public List<AchievementRecord> Achievements { get; } = new();
    public List<WebAccountRecord> WebAccounts { get; } = new();
    public List<WebSessionRecord> WebSessions { get; } = new();
    public List<GameServerRecord> GameServers { get; } = new();
    public List<LobbyRecord> Lobbies { get; } = new();
    public List<RemoteFileRecord> RemoteFiles { get; } = new();
    public List<RemoteFileShareRecord> RemoteFileShares { get; } = new();
    public List<DotaEquipmentRecord> DotaEquipment { get; } = new();
    public List<DotaMatchRecord> DotaMatches { get; } = new();
    public List<DotaHeroIdRecord> DotaHeroIds { get; } = new();
    public List<DotaHeroSlotRecord> DotaHeroSlots { get; } = new();
    public List<DotaItemRecord> DotaItems { get; } = new();
    public CosmeticSettingsRecord Cosmetics { get; set; } = new();
    public AppStateRecord AppState { get; set; } = new();
}
