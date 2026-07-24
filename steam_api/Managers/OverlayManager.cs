using Overlay.Core;
using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;

namespace SKYNET.Managers
{
    public class OverlayManager
    {
        public static Direct3DVersion version;

        private static readonly object Sync = new object();
        private static readonly ManualResetEventSlim InitializationCompleted = new ManualResetEventSlim(true);
        private static OverlayService Service;
        private static bool Initialized;
        private static bool Initializing;

        public static void Initialize()
        {
            ThreadPool.QueueUserWorkItem(_ => EnsureInitialized());
        }

        public static bool IsOverlayEnabled()
        {
            return true;
        }

        public static bool ShowHome(string title = null)
        {
            QueueIdentityRefresh();
            QueueStatsRefresh(GetCurrentSteamId());

            var request = new OverlayRequest
            {
                Kind = OverlayKind.Home,
                Title = title ?? "SKYNETEMU",
                User = BuildCurrentUser(),
                Users = BuildPeopleList(),
                Stats = BuildStats(GetCurrentSteamId()),
                Achievements = BuildAchievements(GetCurrentSteamId()),
                Summary = BuildHomeSummary(),
                Activities = BuildHomeActivities()
            };

            return Show(request);
        }

        public static bool ShowPeople(string title = null)
        {
            APIClient.QueueFriendsRefresh();

            return Show(new OverlayRequest
            {
                Kind = OverlayKind.People,
                Title = title ?? "People",
                User = BuildCurrentUser(),
                Users = BuildPeopleList()
            });
        }

        public static bool ShowInvite(ulong lobbyId, string connectString = null)
        {
            APIClient.QueueFriendsRefresh();

            var hasLobby = lobbyId != 0;
            var hasConnectString = !string.IsNullOrWhiteSpace(connectString);

            return Show(new OverlayRequest
            {
                Kind = OverlayKind.Invite,
                Title = "Invite Friends",
                SessionId = hasLobby ? lobbyId : (ulong?)null,
                Message = hasConnectString ? connectString : null,
                User = BuildCurrentUser(),
                Users = BuildPeopleList().Where(u => u.HasFriend).ToList(),
                InviteUserAction = hasLobby || hasConnectString
                    ? (friend, complete) => QueueInvite(hasLobby ? lobbyId : 0, friend, hasLobby ? null : connectString, complete)
                    : null,
                Metadata = new Dictionary<string, string>
                {
                    ["connect"] = connectString ?? string.Empty
                }
            });
        }

        // Steam delivers LobbyInvite_t when an invitation arrives. The overlay
        // owns the explicit accept action, then posts GameLobbyJoinRequested_t so
        // the game retains authority over whether and how it joins the lobby.
        public static void ShowLobbyInvite(ulong inviterSteamId, ulong lobbyId, string inviterName, string gameName)
        {
            if (lobbyId == 0)
            {
                return;
            }

            APIClient.QueueFriendsRefresh();
            var inviter = BuildInviteSender(inviterSteamId, inviterName);
            var displayName = string.IsNullOrWhiteSpace(inviter?.PersonaName) ? "A friend" : inviter.PersonaName;
            var displayGameName = string.IsNullOrWhiteSpace(gameName) ? "a game" : gameName;

            Show(new OverlayRequest
            {
                Kind = OverlayKind.ConfirmAction,
                Title = "Lobby Invitation",
                Message = displayName + " invited you to play " + displayGameName + ".",
                User = inviter,
                PrimaryActionText = "Join Lobby",
                PrimaryAction = () =>
                {
                    CallbackManager.AddCallback(new GameLobbyJoinRequested_t
                    {
                        m_steamIDLobby = lobbyId,
                        m_steamIDFriend = inviterSteamId
                    });
                    Write($"Accepted lobby invite lobby={lobbyId} inviter={inviterSteamId}");
                },
                SecondaryActionText = "Dismiss",
                SecondaryAction = () => Write($"Dismissed lobby invite lobby={lobbyId} inviter={inviterSteamId}")
            });
        }

        public static void ShowGameInvite(ulong inviterSteamId, string connectString, string inviterName, string gameName)
        {
            if (string.IsNullOrWhiteSpace(connectString))
            {
                return;
            }

            APIClient.QueueFriendsRefresh();
            var inviter = BuildInviteSender(inviterSteamId, inviterName);
            var displayName = string.IsNullOrWhiteSpace(inviter?.PersonaName) ? "A friend" : inviter.PersonaName;
            var displayGameName = string.IsNullOrWhiteSpace(gameName) ? "a game" : gameName;

            Show(new OverlayRequest
            {
                Kind = OverlayKind.ConfirmAction,
                Title = "Game Invitation",
                Message = displayName + " invited you to play " + displayGameName + ".",
                User = inviter,
                PrimaryActionText = "Join Game",
                PrimaryAction = () =>
                {
                    CallbackManager.AddCallback(new GameRichPresenceJoinRequested_t
                    {
                        SteamIDFriend = inviterSteamId,
                        Connect = EncodeConnectString(connectString)
                    });
                    Write($"Accepted game invite inviter={inviterSteamId}");
                },
                SecondaryActionText = "Dismiss",
                SecondaryAction = () => Write($"Dismissed game invite inviter={inviterSteamId}")
            });
        }

        public static bool ShowStore(uint appId, int flag)
        {
            var effectiveAppId = appId == 0 ? SteamEmulator.AppID : appId;

            return Show(new OverlayRequest
            {
                Kind = OverlayKind.Store,
                Title = effectiveAppId == 0 ? "Store" : $"Store - App {effectiveAppId}",
                AppId = effectiveAppId == 0 ? (uint?)null : effectiveAppId,
                User = BuildCurrentUser(),
                StoreItems = new List<OverlayStoreItem>(),
                Metadata = new Dictionary<string, string>
                {
                    ["storeFlag"] = flag.ToString()
                }
            });
        }

        public static bool ShowWebPage(string url, int mode)
        {
            return Show(new OverlayRequest
            {
                Kind = OverlayKind.WebPage,
                Title = "Web Page",
                Url = NormalizeUrl(url),
                User = BuildCurrentUser(),
                Metadata = new Dictionary<string, string>
                {
                    ["mode"] = mode.ToString()
                }
            });
        }

        public static bool ShowUser(string dialog, ulong steamId)
        {
            var effectiveSteamId = steamId == 0 ? GetCurrentSteamId() : steamId;
            var normalized = (dialog ?? string.Empty).Trim().ToLowerInvariant();
            OverlayKind kind;
            string title;

            switch (normalized)
            {
                case "chat":
                    kind = OverlayKind.UserChat;
                    title = "Chat";
                    break;
                case "stats":
                    kind = OverlayKind.UserStats;
                    title = "Stats";
                    break;
                case "achievements":
                    kind = OverlayKind.UserAchievements;
                    title = "Achievements";
                    break;
                default:
                    kind = OverlayKind.UserProfile;
                    title = "User Profile";
                    break;
            }

            QueueUserRefresh(effectiveSteamId);
            QueueStatsRefresh(effectiveSteamId);

            var user = BuildUser(effectiveSteamId);
            if (user != null && !string.IsNullOrWhiteSpace(user.PersonaName))
            {
                title = user.PersonaName;
            }

            return Show(new OverlayRequest
            {
                Kind = kind,
                Title = title,
                UserId = effectiveSteamId,
                AppId = SteamEmulator.AppID,
                User = user,
                Stats = BuildStats(effectiveSteamId),
                Achievements = BuildAchievements(effectiveSteamId),
                Metadata = new Dictionary<string, string>
                {
                    ["dialog"] = normalized,
                    ["steamId"] = effectiveSteamId.ToString()
                }
            });
        }

        public static bool ShowSettings()
        {
            return Show(new OverlayRequest
            {
                Kind = OverlayKind.Settings,
                Title = "Settings",
                User = BuildCurrentUser(),
                Metadata = BuildSettingsMetadata()
            });
        }

        public static bool Hide()
        {
            try
            {
                if (!EnsureInitialized())
                {
                    return false;
                }

                Service.Hide();
                return true;
            }
            catch (Exception ex)
            {
                Write($"Hide failed: {ex}");
                return false;
            }
        }

        public static bool Show(OverlayRequest request)
        {
            try
            {
                if (request == null)
                {
                    return false;
                }

                if (!EnsureInitialized())
                {
                    return false;
                }

                Service.Show(request);
                Write($"Show {request.Kind} title={request.Title ?? string.Empty}");
                return true;
            }
            catch (Exception ex)
            {
                Write($"Show failed: {ex}");
                return false;
            }
        }

        private static bool EnsureInitialized()
        {
            bool waitForCurrentInitialization = false;

            lock (Sync)
            {
                if (Initialized && Service != null)
                {
                    return true;
                }

                if (Initializing)
                {
                    waitForCurrentInitialization = true;
                }
                else
                {
                    Initializing = true;
                    InitializationCompleted.Reset();
                }
            }

            if (waitForCurrentInitialization)
            {
                return WaitForInitialization();
            }

            try
            {
                version = DetectDirect3DVersion();
                var target = ResolveTargetWindowHandle();
                var overlay = new OverlayService();
                overlay.OverlayActivated += OnOverlayActivated;
                overlay.Initialize(new OverlayOptions
                {
                    TargetWindowHandle = target,
                    FollowTargetWindow = target != IntPtr.Zero,
                    TopMost = true,
                    DimBackground = true,
                    Margin = 48
                });

                lock (Sync)
                {
                    Service = overlay;
                    Initialized = true;
                    Initializing = false;
                    InitializationCompleted.Set();
                }

                Write($"Overlay initialized target=0x{target.ToInt64():X} d3d={version}");
                return true;
            }
            catch (Exception ex)
            {
                lock (Sync)
                {
                    Service = null;
                    Initialized = false;
                    Initializing = false;
                    InitializationCompleted.Set();
                }

                Write($"Overlay initialization failed: {ex}");
                return false;
            }
        }

        private static bool WaitForInitialization()
        {
            if (!InitializationCompleted.Wait(TimeSpan.FromMilliseconds(1500)))
            {
                return false;
            }

            lock (Sync)
            {
                return Initialized && Service != null;
            }
        }

        private static void OnOverlayActivated(object sender, OverlayActivatedEventArgs e)
        {
            SteamEmulator.GameOverlay = e.Active;
            CallbackManager.AddCallback(new GameOverlayActivated_t
            {
                Active = e.Active ? (byte)1 : (byte)0,
                UserInitiated = e.UserInitiated,
                AppID = SteamEmulator.AppID,
                OverlayPID = (uint)Process.GetCurrentProcess().Id
            });
        }

        private static IntPtr ResolveTargetWindowHandle()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                for (int i = 0; i < 20; i++)
                {
                    process.Refresh();
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        return process.MainWindowHandle;
                    }

                    Thread.Sleep(50);
                }

                return SKYNET.Helpers.NativeMethods.GetForegroundWindow();
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        private static Direct3DVersion DetectDirect3DVersion()
        {
            if (SKYNET.Helpers.NativeMethods.GetModuleHandle("d3d11.dll") != IntPtr.Zero ||
                SKYNET.Helpers.NativeMethods.GetModuleHandle("d3d11_1.dll") != IntPtr.Zero)
            {
                return Direct3DVersion.Direct3D11;
            }

            if (SKYNET.Helpers.NativeMethods.GetModuleHandle("d3d10.dll") != IntPtr.Zero ||
                SKYNET.Helpers.NativeMethods.GetModuleHandle("d3d10_1.dll") != IntPtr.Zero)
            {
                return Direct3DVersion.Direct3D10;
            }

            if (SKYNET.Helpers.NativeMethods.GetModuleHandle("d3d9.dll") != IntPtr.Zero)
            {
                return Direct3DVersion.Direct3D9;
            }

            return Direct3DVersion.Unknown;
        }

        private static string NormalizeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            var value = url.Trim();
            if (value.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                value = "https://" + value;
            }

            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                return value;
            }

            return uri.ToString();
        }

        private static void QueueIdentityRefresh()
        {
            APIClient.QueueSelfRefresh();
            APIClient.QueueFriendsRefresh();
        }

        private static void QueueUserRefresh(ulong steamId)
        {
            if (steamId == 0)
            {
                return;
            }

            if (steamId == GetCurrentSteamId())
            {
                APIClient.QueueSelfRefresh();
                return;
            }

            APIClient.QueueUserProfileRefresh(steamId, true);
        }

        private static void QueueStatsRefresh(ulong steamId)
        {
            if (steamId == 0)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    if (steamId == GetCurrentSteamId())
                    {
                        APIClient.RefreshCurrentStats();
                    }
                    else
                    {
                        APIClient.RefreshStatsForUser(steamId);
                    }
                }
                catch (Exception ex)
                {
                    Write($"Stats refresh failed for {steamId}: {ex.Message}");
                }
            });
        }

        private static OverlayUser BuildCurrentUser()
        {
            if (StateCache.TryGetSelf(out var self))
            {
                return ToOverlayUser(self);
            }

            var steamId = GetCurrentSteamId();
            if (steamId == 0)
            {
                return null;
            }

            return new OverlayUser
            {
                SteamId = steamId,
                AccountId = GetCurrentAccountId(),
                PersonaName = SteamEmulator.PersonaName ?? string.Empty,
                AppId = SteamEmulator.AppID,
                PersonaState = 1,
                HasFriend = true,
                FriendRelationship = 3,
                IsSelf = true,
                AvatarPng = TryGetAvatarPng(steamId),
                Status = SteamEmulator.AppID == 0 ? "Online" : $"Playing App {SteamEmulator.AppID}"
            };
        }

        private static OverlayUser BuildUser(ulong steamId)
        {
            if (steamId == 0)
            {
                return null;
            }

            if (steamId == GetCurrentSteamId())
            {
                return BuildCurrentUser();
            }

            if (StateCache.TryGetFriend(steamId, out var friend))
            {
                return ToOverlayUser(friend);
            }

            return new OverlayUser
            {
                SteamId = steamId,
                AccountId = steamId.GetAccountID(),
                PersonaState = 0,
                IsSelf = false,
                Status = APIClient.IsUserProfileKnownMissing(steamId) ? "Profile not found" : "Profile pending"
            };
        }

        private static OverlayUser BuildInviteSender(ulong steamId, string personaName)
        {
            var sender = BuildUser(steamId) ?? new OverlayUser { SteamId = steamId };
            if (!string.IsNullOrWhiteSpace(personaName))
            {
                sender.PersonaName = personaName;
            }

            sender.AvatarPng ??= TryGetAvatarPng(steamId);
            return sender;
        }

        private static List<OverlayUser> BuildPeopleList()
        {
            var users = StateCache.GetFriends()
                .Select(ToOverlayUser)
                .Where(u => u != null)
                .OrderByDescending(u => u.PersonaState != 0)
                .ThenBy(u => u.PersonaName)
                .ThenBy(u => u.SteamId)
                .ToList();

            var self = BuildCurrentUser();
            if (self != null && users.All(u => u.SteamId != self.SteamId))
            {
                users.Insert(0, self);
            }

            return users;
        }

        private static List<OverlayStat> BuildStats(ulong steamId)
        {
            if (steamId == 0)
            {
                return new List<OverlayStat>();
            }

            return StateCache.GetStats(steamId)
                .Where(s => !string.IsNullOrWhiteSpace(s.Name))
                .OrderBy(s => s.Name)
                .Select(s => new OverlayStat
                {
                    Name = s.Name,
                    Value = s.Data,
                    DisplayValue = s.Data.ToString()
                })
                .ToList();
        }

        private static List<OverlayAchievement> BuildAchievements(ulong steamId)
        {
            if (steamId == 0)
            {
                return new List<OverlayAchievement>();
            }

            return StateCache.GetAchievements(steamId)
                .Where(a => !string.IsNullOrWhiteSpace(a.Name))
                .OrderByDescending(a => a.Earned)
                .ThenBy(a => a.Name)
                .Select(a => new OverlayAchievement
                {
                    Name = a.Name,
                    Earned = a.Earned,
                    Date = a.Date,
                    Progress = a.Progress,
                    MaxProgress = a.MaxProgress
                })
                .ToList();
        }

        private static List<OverlaySummaryItem> BuildHomeSummary()
        {
            var steamId = GetCurrentSteamId();
            var friends = StateCache.GetFriends();
            var stats = BuildStats(steamId);
            var achievements = BuildAchievements(steamId);
            var currentPlayers = StateCache.GetCurrentPlayers();
            var level = StateCache.GetSelfPlayerLevel();

            return new List<OverlaySummaryItem>
            {
                new OverlaySummaryItem
                {
                    Label = "AppID",
                    Value = SteamEmulator.AppID == 0 ? "Unknown" : SteamEmulator.AppID.ToString(),
                    Tone = "accent"
                },
                new OverlaySummaryItem
                {
                    Label = "Friends",
                    Value = friends.Count.ToString(),
                    Tone = friends.Count > 0 ? "success" : "warning"
                },
                new OverlaySummaryItem
                {
                    Label = "Current Players",
                    Value = currentPlayers.ToString(),
                    Tone = currentPlayers > 0 ? "success" : "accent"
                },
                new OverlaySummaryItem
                {
                    Label = "Player Level",
                    Value = level.ToString(),
                    Tone = level > 0 ? "success" : "accent"
                },
                new OverlaySummaryItem
                {
                    Label = "Stats",
                    Value = stats.Count.ToString(),
                    Tone = stats.Count > 0 ? "success" : "accent"
                },
                new OverlaySummaryItem
                {
                    Label = "Achievements",
                    Value = achievements.Count(a => a.Earned) + "/" + achievements.Count,
                    Tone = achievements.Any(a => a.Earned) ? "success" : "accent"
                }
            };
        }

        private static List<OverlayActivityItem> BuildHomeActivities()
        {
            var activities = new List<OverlayActivityItem>();
            var self = BuildCurrentUser();
            if (self != null)
            {
                activities.Add(new OverlayActivityItem
                {
                    Title = string.IsNullOrWhiteSpace(self.PersonaName) ? self.SteamId.ToString() : self.PersonaName,
                    Detail = self.Status,
                    Timestamp = DateTime.UtcNow
                });
            }

            foreach (var user in BuildPeopleList().Where(u => u.SteamId != self?.SteamId).Take(6))
            {
                activities.Add(new OverlayActivityItem
                {
                    Title = string.IsNullOrWhiteSpace(user.PersonaName) ? user.SteamId.ToString() : user.PersonaName,
                    Detail = user.Status,
                    Timestamp = DateTime.UtcNow
                });
            }

            return activities;
        }

        private static Dictionary<string, string> BuildSettingsMetadata()
        {
            return new Dictionary<string, string>
            {
                ["AppID"] = SteamEmulator.AppID.ToString(),
                ["SteamID"] = GetCurrentSteamId().ToString(),
                ["AccountID"] = GetCurrentAccountId().ToString(),
                ["Persona"] = SteamEmulator.PersonaName ?? string.Empty,
                ["Language"] = SteamEmulator.Language ?? string.Empty,
                ["Server URL"] = SteamEmulator.ServerUrl ?? string.Empty,
                ["Server API"] = SteamEmulator.UseServerApi.ToString(),
                ["Secure Networking"] = SteamEmulator.SecureNetworking.ToString(),
                ["Voice Capture"] = SteamEmulator.EnableVoiceCapture.ToString(),
                ["HTTP Timeout"] = SteamEmulator.HttpTimeoutMs.ToString(),
                ["Poll Interval"] = SteamEmulator.PollIntervalMs.ToString()
            };
        }

        private static OverlayUser ToOverlayUser(SteamPlayer player)
        {
            if (player == null)
            {
                return null;
            }

            var isSelf = player.SteamID == GetCurrentSteamId();
            var localAppId = SteamEmulator.AppID;
            var appId = isSelf && localAppId != 0 ? localAppId : player.GameID;
            var lobbyId = isSelf &&
                          player.GameID != 0 &&
                          localAppId != 0 &&
                          player.GameID != localAppId
                ? 0
                : player.LobbyID;

            return new OverlayUser
            {
                SteamId = player.SteamID,
                AccountId = player.AccountID,
                PersonaName = player.PersonaName ?? string.Empty,
                AppId = appId,
                LobbyId = lobbyId,
                PersonaState = player.PersonaState,
                HasFriend = player.HasFriend,
                FriendRelationship = player.FriendRelationship,
                IsSelf = isSelf,
                AvatarPng = TryGetAvatarPng(player.SteamID),
                RichPresence = new Dictionary<string, string>(player.RichPresence ?? new Dictionary<string, string>()),
                Status = BuildStatus(player, appId, lobbyId)
            };
        }

        private static byte[] TryGetAvatarPng(ulong steamId)
        {
            if (steamId == 0 || steamId == ulong.MaxValue || SteamEmulator.SteamFriends == null)
            {
                return new byte[0];
            }

            try
            {
                if (!SteamEmulator.SteamFriends.TryGetAvatarBitmap(steamId, out var bitmap) || bitmap == null)
                {
                    return new byte[0];
                }

                using (bitmap)
                using (var stream = new MemoryStream())
                {
                    if (IsPlaceholderAvatar(bitmap))
                    {
                        return new byte[0];
                    }

                    bitmap.Save(stream, ImageFormat.Png);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Write($"Avatar load failed for {steamId}: {ex.Message}");
                return new byte[0];
            }
        }

        private static bool IsPlaceholderAvatar(Bitmap bitmap)
        {
            if (bitmap == null || bitmap.Width <= 0 || bitmap.Height <= 0)
            {
                return true;
            }

            var stepX = Math.Max(1, bitmap.Width / 8);
            var stepY = Math.Max(1, bitmap.Height / 8);
            Color? first = null;
            var samples = 0;
            var uniform = 0;
            var dark = 0;
            var transparent = 0;

            for (var y = 0; y < bitmap.Height; y += stepY)
            {
                for (var x = 0; x < bitmap.Width; x += stepX)
                {
                    var color = bitmap.GetPixel(x, y);
                    samples++;

                    if (color.A < 8)
                    {
                        transparent++;
                    }

                    if (color.R <= 40 && color.G <= 40 && color.B <= 40)
                    {
                        dark++;
                    }

                    if (!first.HasValue)
                    {
                        first = color;
                    }
                    else if (Math.Abs(first.Value.R - color.R) <= 3 &&
                             Math.Abs(first.Value.G - color.G) <= 3 &&
                             Math.Abs(first.Value.B - color.B) <= 3 &&
                             Math.Abs(first.Value.A - color.A) <= 3)
                    {
                        uniform++;
                    }
                }
            }

            if (samples == 0)
            {
                return true;
            }

            return transparent == samples ||
                   (dark >= samples * 0.95 && uniform >= (samples - 1) * 0.90);
        }

        private static string BuildStatus(SteamPlayer player, uint appId, ulong lobbyId)
        {
            if (player == null)
            {
                return string.Empty;
            }

            if (player.RichPresence != null &&
                player.RichPresence.TryGetValue("status", out var richStatus) &&
                !string.IsNullOrWhiteSpace(richStatus))
            {
                return richStatus;
            }

            if (player.RichPresence != null &&
                player.RichPresence.TryGetValue("steam_display", out var display) &&
                !string.IsNullOrWhiteSpace(display))
            {
                return display;
            }

            if (appId != 0)
            {
                var gameStatus = appId == SteamEmulator.AppID
                    ? "In game"
                    : $"Playing App {appId}";

                if (lobbyId != 0)
                {
                    gameStatus += $" - Lobby {lobbyId}";
                }

                return gameStatus;
            }

            switch (player.PersonaState)
            {
                case 1:
                    return "Online";
                case 2:
                    return "Busy";
                case 3:
                    return "Away";
                case 4:
                    return "Snooze";
                case 5:
                    return "Looking to trade";
                case 6:
                    return "Looking to play";
                case 7:
                    return "Invisible";
                default:
                    return "Offline";
            }
        }

        private static ulong GetCurrentSteamId()
        {
            try
            {
                return (ulong)SteamEmulator.SteamID;
            }
            catch
            {
                return 0;
            }
        }

        private static uint GetCurrentAccountId()
        {
            try
            {
                return SteamEmulator.SteamID.GetAccountID();
            }
            catch
            {
                return 0;
            }
        }

        private static void Write(object msg)
        {
            SteamEmulator.Write("OverlayManager", msg);
        }

        private static void QueueInvite(ulong lobbyId, OverlayUser friend, string connectString, Action<bool> complete)
        {
            if (friend == null || friend.SteamId == 0)
            {
                complete?.Invoke(false);
                return;
            }

            var thread = new Thread(() =>
            {
                var success = false;
                try
                {
                    success = lobbyId != 0
                        ? APIClient.InviteUserToLobby(lobbyId, friend.SteamId)
                        : APIClient.InviteUserToGame(friend.SteamId, connectString);
                    Write($"Invite {(lobbyId != 0 ? "lobby=" + lobbyId : "game")} friend={friend.SteamId} success={success}");
                }
                catch (Exception ex)
                {
                    Write($"Invite dispatch failed for friend={friend.SteamId}: {ex.Message}");
                }
                finally
                {
                    complete?.Invoke(success);
                }
            })
            {
                IsBackground = true,
                Name = "SKYNET overlay invite"
            };
            thread.Start();
        }

        private static byte[] EncodeConnectString(string connectString)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(connectString ?? string.Empty);
            var buffer = new byte[256];
            Array.Copy(bytes, buffer, Math.Min(bytes.Length, buffer.Length - 1));
            return buffer;
        }

        public enum Direct3DVersion
        {
            Unknown,
            AutoDetect,
            Direct3D9,
            Direct3D10,
            Direct3D10_1,
            Direct3D11,
            Direct3D11_1,
        }
    }
}
