using SKYNET.Helper;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Managers
{
    public class LobbyManager
    {
        public static List<SteamLobby> Lobbies;

        public LobbyManager()
        {
            Lobbies = new List<SteamLobby>();
        }

        public static void Initialize()
        {

        }

        internal static void Create(SteamLobby lobby)
        {
            MutexHelper.Wait("Lobbies", delegate
            {
                Lobbies.Add(lobby);
            });
        }

        internal static void Remove(ulong lobbyID)
        {
            MutexHelper.Wait("Lobbies", delegate
            {
                Lobbies.RemoveAll(l => l.SteamID == lobbyID);
            });
        }

        internal static SteamLobby GetLobby(ulong lobbyID)
        {
            SteamLobby lobby = null;
            MutexHelper.Wait("Lobbies", delegate
            {
                lobby = Lobbies.Find(l => l.SteamID == lobbyID);
            });
            return lobby;
        }

        internal static void Update(SteamLobby lobby)
        {
            var Lobby = Lobbies.Find(l => l.SteamID == lobby.SteamID);
            if (Lobby != null)
            {
                Lobby = lobby;
            }
            else
            {
                Create(lobby);
            }
        }
    }
}
