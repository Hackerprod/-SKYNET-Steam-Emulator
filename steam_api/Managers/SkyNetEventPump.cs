using SKYNET.Callback;
using SKYNET.Network.Packets;
using SKYNET.Steamworks;
using System;

namespace SKYNET.Managers
{
    public static class SkyNetEventPump
    {
        private static readonly object SyncRoot = new object();
        private static DateTime LastPoll = DateTime.MinValue;
        private static bool Polling;

        public static void RunFrame(bool gameServer)
        {
            if (gameServer || !SkyNetApiClient.IsEnabled)
            {
                return;
            }

            lock (SyncRoot)
            {
                if (Polling)
                {
                    return;
                }

                if (DateTime.UtcNow - LastPoll < TimeSpan.FromMilliseconds(SteamEmulator.SkyNetPollIntervalMs))
                {
                    return;
                }

                Polling = true;
                LastPoll = DateTime.UtcNow;
            }

            try
            {
                var envelope = SkyNetApiClient.PollEvents();
                if (envelope?.Events == null)
                {
                    return;
                }

                foreach (var serverEvent in envelope.Events)
                {
                    ApplyEvent(serverEvent);
                }
            }
            finally
            {
                lock (SyncRoot)
                {
                    Polling = false;
                }
            }
        }

        private static void ApplyEvent(SkyNetApiClient.SkyNetEventDto serverEvent)
        {
            if (serverEvent == null || string.IsNullOrWhiteSpace(serverEvent.Type))
            {
                return;
            }

            switch (serverEvent.Type)
            {
                case "persona_state_changed":
                case "friend_added":
                case "friend_presence_changed":
                    SkyNetStateCache.UpsertFriendFromEvent(serverEvent);
                    EmitPersonaStateChange(serverEvent);
                    break;

                case "friend_removed":
                    SkyNetStateCache.RemoveFriend(serverEvent.SteamId != 0 ? serverEvent.SteamId : (ulong)new CSteamID(serverEvent.AccountId));
                    EmitPersonaStateChange(serverEvent);
                    break;

                case "stats_updated":
                    SkyNetStateCache.UpsertStat(serverEvent.SteamId != 0 ? serverEvent.SteamId : (ulong)new CSteamID(serverEvent.AccountId), serverEvent.StatName, serverEvent.StatValue, false);
                    break;

                case "achievement_unlocked":
                    SkyNetStateCache.UpsertAchievement(serverEvent.SteamId != 0 ? serverEvent.SteamId : (ulong)new CSteamID(serverEvent.AccountId), serverEvent.AchievementName, serverEvent.AchievementEarned, serverEvent.AchievementProgress, serverEvent.AchievementMaxProgress, false);
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
            }
        }

        private static void EmitPersonaStateChange(SkyNetApiClient.SkyNetEventDto serverEvent)
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
        }

        private static void ApplyLobby(SkyNetApiClient.SkyNetEventDto serverEvent)
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

        private static void RemoveLobby(SkyNetApiClient.SkyNetEventDto serverEvent)
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

        private static void ApplyP2PPacket(SkyNetApiClient.SkyNetEventDto serverEvent)
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

        private static void ApplyGCMessage(SkyNetApiClient.SkyNetEventDto serverEvent)
        {
            if (SteamEmulator.SteamGameCoordinator == null || string.IsNullOrWhiteSpace(serverEvent.PayloadBase64))
            {
                return;
            }

            try
            {
                SteamEmulator.SteamGameCoordinator.PushMessage(serverEvent.MessageType, Convert.FromBase64String(serverEvent.PayloadBase64));
            }
            catch
            {
            }
        }
    }
}
