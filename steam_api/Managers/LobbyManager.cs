using SKYNET.Helpers;
using SKYNET.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SKYNET.Managers
{
    public class LobbyManager
    {
        public static List<SteamLobby> Lobbies;

        // Per-lobby chat backlog. Filled from "lobby_chat" events on the pump
        // thread; read by GetLobbyChatEntry on the game thread. The message index
        // reported by LobbyChatMsg_t.m_iChatID indexes into this buffer.
        private static readonly ConcurrentDictionary<ulong, ChatBuffer> ChatBuffers = new();

        static LobbyManager()
        {
            Lobbies = new List<SteamLobby>();
        }

        public static int AppendChat(ulong lobbyId, ulong sender, byte[] data, byte entryType)
        {
            return ChatBuffers.GetOrAdd(lobbyId, _ => new ChatBuffer()).Append(sender, data ?? Array.Empty<byte>(), entryType);
        }

        public static bool TryGetChat(ulong lobbyId, int chatId, out ulong sender, out byte[] data, out byte entryType)
        {
            if (ChatBuffers.TryGetValue(lobbyId, out var buffer))
            {
                return buffer.TryGet(chatId, out sender, out data, out entryType);
            }

            sender = 0;
            data = Array.Empty<byte>();
            entryType = 0;
            return false;
        }

        public static void ClearChat(ulong lobbyId)
        {
            ChatBuffers.TryRemove(lobbyId, out _);
        }

        private sealed class ChatBuffer
        {
            private readonly object _sync = new();
            private readonly List<(ulong Sender, byte[] Data, byte Type)> _entries = new();

            public int Append(ulong sender, byte[] data, byte type)
            {
                lock (_sync)
                {
                    _entries.Add((sender, data, type));
                    return _entries.Count - 1;
                }
            }

            public bool TryGet(int index, out ulong sender, out byte[] data, out byte type)
            {
                lock (_sync)
                {
                    if (index >= 0 && index < _entries.Count)
                    {
                        (sender, data, type) = _entries[index];
                        return true;
                    }
                }

                sender = 0;
                data = Array.Empty<byte>();
                type = 0;
                return false;
            }
        }

        public static SteamLobby GetLobby(ulong lobbyID)
        {
            SteamLobby lobby = null;
            MutexHelper.Wait("Lobbies", delegate
            {
                lobby = Lobbies.Find(l => l.SteamID == lobbyID);
            });
            return lobby;
        }

        public static bool GetLobby(ulong lobbyID, out SteamLobby lobby)
        {
            lobby = GetLobby(lobbyID);
            return lobby != null;
        }

        public static SteamLobby GetLobbyByOwner(ulong steamID)
        {
            return Lobbies.Where(l => l.Owner == steamID).FirstOrDefault();
        }

        public static SteamLobby GetLobbyByGameserver(ulong steamID_GS)
        {
            return Lobbies.Where(l => l.Gameserver.SteamID == steamID_GS).Select(l => l).FirstOrDefault();
        }

        internal static SteamLobby GetLobbyByIndex(uint appID, int index)
        {
            var current = 0;
            var lobbies = Lobbies.Where(l => l.AppID == appID);
            foreach (var lobby in lobbies)
            {
                if (current == index)
                {
                    return lobby;
                }
                current++;
            }
            return null;
        }

        public static List<SteamLobby> GetLobbies(uint appID)
        {
            return Lobbies.FindAll(l => l.AppID == appID); 
        }

        internal static void UpdateLobbies(List<SteamLobby> UpdatedList)
        {
            MutexHelper.Wait("Lobbies", delegate
            {
                Lobbies.Clear();
                Lobbies.AddRange(UpdatedList);
            });
        }

        internal static void UpsertLobby(SteamLobby updatedLobby)
        {
            if (updatedLobby == null)
            {
                return;
            }

            MutexHelper.Wait("Lobbies", delegate
            {
                var current = Lobbies.FindIndex(l => l.SteamID == updatedLobby.SteamID);
                if (current >= 0)
                {
                    Lobbies[current] = updatedLobby;
                }
                else
                {
                    Lobbies.Add(updatedLobby);
                }
            });
        }

        internal static void RemoveLobby(ulong lobbyId)
        {
            MutexHelper.Wait("Lobbies", delegate
            {
                Lobbies.RemoveAll(l => l.SteamID == lobbyId);
            });
        }
    }
}
