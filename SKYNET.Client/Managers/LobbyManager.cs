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

        public static void Create(SteamLobby lobby)
        {
            MutexHelper.Wait("Lobbies", delegate
            {
                Lobbies.Add(lobby);
            });
        }

        public static void Remove(ulong lobbyID)
        {
            MutexHelper.Wait("Lobbies", delegate
            {
                Lobbies.RemoveAll(l => l.SteamID == lobbyID);
            });
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

        public static void Update(SteamLobby lobby)
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

        public static SteamLobby GetLobbyByOwner(ulong steamID)
        {
            return Lobbies.Where(l => l.Owner == steamID).Select(l => l).FirstOrDefault();
        }

        public static SteamLobby GetLobbyByGameserver(ulong steamID_GS)
        {
            return Lobbies.Where(l => l.Gameserver.SteamID == steamID_GS).Select(l => l).FirstOrDefault();
        }
    }
}
