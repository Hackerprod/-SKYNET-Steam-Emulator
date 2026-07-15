using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Network.Packets;
using SKYNET.Steamworks;
using System;
using System.Text;
using System.Threading;

namespace SKYNET.Managers
{
    public static class EventPump
    {
        private static int Started;

        public static void RunFrame(bool gameServer)
        {
            if (gameServer || IsDedicatedProcess || !SkyNetApiClient.IsEnabled)
            {
                return;
            }

            EnsureStarted();
        }

        private static bool IsDedicatedProcess =>
            string.Equals(Environment.GetEnvironmentVariable("SKYNET_PROCESS_ROLE"), "dedicated", StringComparison.OrdinalIgnoreCase);

        private static void EnsureStarted()
        {
            if (Interlocked.Exchange(ref Started, 1) == 1)
            {
                return;
            }

            var thread = new Thread(PumpLoop)
            {
                IsBackground = true,
                Name = "SKYNET event pump"
            };
            thread.Start();
        }

        private static void PumpLoop()
        {
            while (true)
            {
                try
                {
                    if (!SkyNetApiClient.IsEnabled)
                    {
                        Thread.Sleep(250);
                        continue;
                    }

                    var envelope = SkyNetApiClient.PollEvents(1000);
                    if (envelope?.Events != null)
                    {
                        foreach (var serverEvent in envelope.Events)
                        {
                            ApplyEvent(serverEvent);
                        }
                    }
                }
                catch
                {
                    Thread.Sleep(Math.Max(50, SteamEmulator.PollIntervalMs));
                }
            }
        }

        private static void ApplyEvent(SkyNetApiClient.ApiEvent serverEvent)
        {
            if (serverEvent == null || string.IsNullOrWhiteSpace(serverEvent.Type))
            {
                return;
            }

            switch (serverEvent.Type)
            {
                case "persona_state_changed":
                case "friend_added":
                case "friend_request_received":
                case "friend_request_sent":
                case "friend_presence_changed":
                    StateCache.UpsertFriendFromEvent(serverEvent);
                    EmitPersonaStateChange(serverEvent);
                    break;

                case "friend_removed":
                    StateCache.RemoveFriend(serverEvent.SteamId != 0 ? serverEvent.SteamId : (ulong)new CSteamID(serverEvent.AccountId));
                    EmitPersonaStateChange(serverEvent);
                    break;

                case "stats_updated":
                    StateCache.UpsertStat(serverEvent.SteamId != 0 ? serverEvent.SteamId : (ulong)new CSteamID(serverEvent.AccountId), serverEvent.StatName, serverEvent.StatValue, false);
                    break;

                case "achievement_unlocked":
                    StateCache.UpsertAchievement(serverEvent.SteamId != 0 ? serverEvent.SteamId : (ulong)new CSteamID(serverEvent.AccountId), serverEvent.AchievementName, serverEvent.AchievementEarned, serverEvent.AchievementProgress, serverEvent.AchievementMaxProgress, false);
                    break;

                case "lobby_updated":
                case "lobby_created":
                case "lobby_joined":
                    ApplyLobby(serverEvent);
                    break;

                case "lobby_left":
                case "lobby_removed":
                    RemoveLobby(serverEvent);
                    break;

                case "p2p_packet":
                    ApplyP2PPacket(serverEvent);
                    break;

                case "gc_message":
                    ApplyGCMessage(serverEvent);
                    break;

                case "game_server_change_requested":
                    if (IsDedicatedProcess)
                    {
                        return;
                    }

                    EmitGameServerChangeRequested(serverEvent);
                    break;
            }
        }

        private static void EmitPersonaStateChange(SkyNetApiClient.ApiEvent serverEvent)
        {
            var steamId = serverEvent.SteamId != 0
                ? serverEvent.SteamId
                : (ulong)new CSteamID(serverEvent.AccountId);

            var callback = new PersonaStateChange_t
            {
                m_ulSteamID = steamId,
                m_nChangeFlags = serverEvent.ChangeFlags != 0
                    ? serverEvent.ChangeFlags
                    : (int)EPersonaChange.k_EPersonaChangeName
            };

            CallbackManager.AddCallback(callback);

            if ((callback.m_nChangeFlags & (int)EPersonaChange.k_EPersonaChangeAvatar) != 0)
            {
                ThreadPool.QueueUserWorkItem(_ => SkyNetApiClient.RefreshAvatar(steamId));
            }
        }

        private static void ApplyLobby(SkyNetApiClient.ApiEvent serverEvent)
        {
            var lobby = serverEvent.Lobby == null ? null : SkyNetApiClient.MapLobbyForEvents(serverEvent.Lobby);
            if (lobby == null)
            {
                return;
            }

            LobbyManager.UpsertLobby(lobby);

            CallbackManager.AddCallback(new LobbyDataUpdate_t
            {
                m_bSuccess = true,
                m_ulSteamIDLobby = lobby.SteamID,
                m_ulSteamIDMember = lobby.Owner
            });
        }

        private static void RemoveLobby(SkyNetApiClient.ApiEvent serverEvent)
        {
            var lobbyId = serverEvent.Lobby?.SteamId ?? serverEvent.LobbyId;
            if (lobbyId == 0)
            {
                return;
            }

            LobbyManager.RemoveLobby(lobbyId);

            CallbackManager.AddCallback(new LobbyDataUpdate_t
            {
                m_bSuccess = false,
                m_ulSteamIDLobby = lobbyId,
                m_ulSteamIDMember = serverEvent.SteamId
            });
        }

        private static void ApplyP2PPacket(SkyNetApiClient.ApiEvent serverEvent)
        {
            if (SteamEmulator.SteamNetworking == null || string.IsNullOrWhiteSpace(serverEvent.PayloadBase64))
            {
                return;
            }

            var remoteSteamId = serverEvent.RemoteSteamId != 0
                ? serverEvent.RemoteSteamId
                : serverEvent.SteamId;

            SteamEmulator.SteamNetworking.ProcessP2PPacket(new NET_P2PPacket
            {
                AccountID = ((ulong)SteamEmulator.SteamID).GetAccountID(),
                Sender = remoteSteamId.GetAccountID(),
                Buffer = serverEvent.PayloadBase64,
                P2PSendType = 0,
                Channel = serverEvent.Channel
            });
        }

        private static void ApplyGCMessage(SkyNetApiClient.ApiEvent serverEvent)
        {
            if (SteamEmulator.SteamGameCoordinator == null || string.IsNullOrWhiteSpace(serverEvent.PayloadBase64))
            {
                return;
            }

            try
            {
                SteamEmulator.SteamGameCoordinator.PushServerMessage(
                    serverEvent.MessageType,
                    Convert.FromBase64String(serverEvent.PayloadBase64),
                    serverEvent.TargetJobId ?? ulong.MaxValue,
                    serverEvent.Protobuf);
            }
            catch
            {
            }
        }

        private static void EmitGameServerChangeRequested(SkyNetApiClient.ApiEvent serverEvent)
        {
            var server = (serverEvent.Server ?? string.Empty).Trim();
            if (server.Length == 0)
            {
                return;
            }

            CallbackManager.AddCallback(new GameServerChangeRequested_t
            {
                m_rgchServer = FixedUtf8(server, 64),
                m_rgchPassword = FixedUtf8(serverEvent.Password ?? string.Empty, 64)
            });
            SteamEmulator.Write("EventPump", $"GameServerChangeRequested server={server}");
        }

        private static byte[] FixedUtf8(string value, int size)
        {
            var buffer = new byte[size];
            if (size <= 0 || string.IsNullOrEmpty(value))
            {
                return buffer;
            }

            var bytes = Encoding.UTF8.GetBytes(value);
            Array.Copy(bytes, buffer, Math.Min(bytes.Length, size - 1));
            return buffer;
        }
    }
}
