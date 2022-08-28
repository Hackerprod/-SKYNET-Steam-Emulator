using SKYNET.Helpers;
using SKYNET.IPC.Types;
using SKYNET.Network;
using SKYNET.Network.Types;
using SKYNET.Steamworks;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SKYNET.Managers
{
    public class LobbyManager
    {
        public static List<SteamLobby> Lobbies;

        static LobbyManager()
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
            IPCManager.SendUpdatedLobbies();
        }

        public static void Remove(ulong lobbyID)
        {
            MutexHelper.Wait("Lobbies", delegate
            {
                Lobbies.RemoveAll(l => l.SteamID == lobbyID);
            });
            IPCManager.SendUpdatedLobbies();
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
            IPCManager.SendUpdatedLobbies();
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

        internal static void SetLobbyData(ulong steamID, string key, string value)
        {
            var Lobby = GetLobby(steamID);
            if (Lobby != null)
            {
                if (Lobby.LobbyData == null) Lobby.LobbyData = new Dictionary<string, string>();
                if (Lobby.LobbyData.ContainsKey(key))
                {
                    Lobby.LobbyData[key] = value;
                }
                else
                {
                    Lobby.LobbyData.Add(key, value);
                }
            }
            UpdateLobbyMembers(Lobby);
            IPCManager.SendUpdatedLobbies();
        }

        internal static void SetGameServerEndPoint(ulong lobbySteamID, uint iP, uint port)
        {
            var Lobby = GetLobby(lobbySteamID);
            if (Lobby != null)
            {
                if (Lobby.Gameserver == null) Lobby.Gameserver = new SteamLobby.LobbyGameserver();
                Lobby.Gameserver.IP = iP;
                Lobby.Gameserver.Port = port;
                IPCManager.SendUpdatedLobbies();
                UpdateLobbyMembers(Lobby);
            }
        }
        public static void CreateGameServer(IPC_LobbyGameserver IPC_Lobby)
        {
            if (GetLobby(IPC_Lobby.LobbyID, out var Lobby))
            {
                //uint IP = unGameServerIP != 0 ? unGameServerIP : NetworkHelper.ConvertFromIPAddress(NetworkHelper.GetIPAddress());
                if (Lobby.Gameserver == null) Lobby.Gameserver = new SteamLobby.LobbyGameserver();
                Lobby.Gameserver.SteamID = IPC_Lobby.SteamID;
                Lobby.Gameserver.IP = IPC_Lobby.IP; 
                Lobby.Gameserver.Filled = true;
                Lobby.Gameserver.Port = IPC_Lobby.Port;

                IPCManager.SendUpdatedLobbies();
                UpdateLobbyMembers(Lobby);
            }
        }

        public static void UpdateLobbyMembers(SteamLobby lobby)
        {
            NETProcessor.SendUpdateLobby(lobby, false);
        }

        //internal static void LeaveLobby(NET_LobbyLeave lobbyLeave)
        //{
        //    if (GetLobby(lobbyLeave.LobbyID, out var lobby))
        //    {
        //        lobby.Members.RemoveAll(m => m.m_SteamID == lobbyLeave.SteamID);

        //        NetworkManager.SendUpdateLobby(lobby, false);

        //        var lobbyChatUpdate = new NET_LobbyChatUpdate()
        //        {
        //            SteamIDLobby = lobbyLeave.SteamID,
        //            SteamIDUserChanged = lobbyLeave.SteamID,
        //            SteamIDMakingChange = lobbyLeave.SteamID,
        //            ChatMemberStateChange = (int)EChatMemberStateChange.k_EChatMemberStateChangeLeft
        //        };

        //        var members = lobby.Members.FindAll(m => m.m_SteamID != lobby.Owner); 

        //        foreach (var member in members)
        //        {
        //            var User = UserManager.GetUser(member.m_SteamID);
        //            if (User != null)
        //            {
        //                NetworkManager.SendTo(User.IPAddress, lobbyChatUpdate, NETMessageType.NET_LobbyChatUpdate);
        //            }
        //        }
        //    }
        //}
    }
}
