using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using SKYNET;
using SKYNET.Callback;
using SKYNET.Managers;
using SKYNET.Steamworks;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamMatchmaking : ISteamInterface
    {
        private const int CallbackTimeOut = 20;
        private ConcurrentDictionary<ulong, SteamLobby> Lobbies;
        private FilterLobby filters;

        public SteamMatchmaking()
        {
            InterfaceName = "SteamMatchmaking";
            Lobbies = new ConcurrentDictionary<SteamAPICall_t, SteamLobby>();
            filters = new FilterLobby();

            var tempID = modCommon.GenerateSteamID();
            Lobbies.TryAdd(tempID, new SteamLobby()
            {
                Joinable = true,
                MaxMembers = 10,
                Owner = (ulong)SteamEmulator.SteamId,
                SteamID = tempID,
                Type = ELobbyType.k_ELobbyTypePublic
            });
        }

        public int AddFavoriteGame(uint nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags, uint rTime32LastPlayedOnServer)
        {
            Write("AddFavoriteGame");
            return 1;
        }

        public void AddRequestLobbyListCompatibleMembersFilter(ulong steamIDLobby)
        {
            Write("AddRequestLobbyListCompatibleMembersFilter");
            
        }

        public void AddRequestLobbyListDistanceFilter(int eLobbyDistanceFilter)
        {
            Write("AddRequestLobbyListDistanceFilter");
            filters.Distance = eLobbyDistanceFilter;
        }

        public void AddRequestLobbyListFilterSlotsAvailable(int nSlotsAvailable)
        {
            Write("AddRequestLobbyListFilterSlotsAvailable");
            filters.SlotsAvailable = nSlotsAvailable;
        }

        public void AddRequestLobbyListNearValueFilter(string pchKeyToMatch, int nValueToBeCloseTo)
        {
            Write("AddRequestLobbyListNearValueFilter");
        }

        public void AddRequestLobbyListNumericalFilter(string pchKeyToMatch, int nValueToMatch, int eComparisonType)
        {
            Write("AddRequestLobbyListNumericalFilter");
            filters.KeyToMatch = pchKeyToMatch;
            filters.ValueToMatch = nValueToMatch;
            filters.ComparisonType = (ELobbyComparison)eComparisonType;
        }

        public void AddRequestLobbyListResultCountFilter(int cMaxResults)
        {
            Write("AddRequestLobbyListResultCountFilter");
        }

        public void AddRequestLobbyListStringFilter(string pchKeyToMatch, string pchValueToMatch, int eComparisonType)
        {
            Write("AddRequestLobbyListStringFilter");
            filters.KeyToMatch = pchKeyToMatch;
            filters.StringValueToMatch = pchValueToMatch;
            filters.ComparisonType = (ELobbyComparison)eComparisonType;
        }

        public SteamAPICall_t CreateLobby(int eLobbyType, int cMaxMembers)
        {
            Write($"CreateLobby {(ELobbyType)eLobbyType} limit {cMaxMembers} members");
            var LocalLobby = new SteamLobby()
            {
                Owner = (ulong)SteamEmulator.SteamId, 
                SteamID = modCommon.GenerateSteamID(),
                Type = (ELobbyType)eLobbyType,
                MaxMembers =  cMaxMembers
            };

            LobbyCreated_t data = new LobbyCreated_t()
            {
                m_eResult = Types.EResult.k_EResultOK, 
                m_ulSteamIDLobby = LocalLobby.SteamID
            };

            LobbyEnter_t enterData = new LobbyEnter_t()
            {
                m_ulSteamIDLobby = LocalLobby.SteamID,
                m_rgfChatPermissions = 0,
                m_bLocked = false,
                m_EChatRoomEnterResponse = 1
            };

            if (!Lobbies.TryAdd(LocalLobby.SteamID, LocalLobby))
            {
                data.m_eResult = Types.EResult.k_EResultFail;
            }

            SteamEmulator.SteamFriends.UpdateUserLobby((ulong)SteamEmulator.SteamId, LocalLobby.SteamID);

            ulong callResult = CallbackManager.AddCallbackResult(data);
            CallbackManager.AddCallback(data);

            CallbackManager.AddCallbackResult(enterData);
            CallbackManager.AddCallback(enterData);

            return callResult;
        }

        public bool DeleteLobbyData(ulong steamIDLobby, string pchKey)
        {
            Write("DeleteLobbyData");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                lobby.LobbyData.Clear();
                return true;
            }
            return false;
        }

        public bool GetFavoriteGame(int iGame, uint pnAppID, uint pnIP, uint pnConnPort, uint pnQueryPort, uint punFlags, uint pRTime32LastPlayedOnServer)
        {
            Write("GetFavoriteGame");
            return true;
        }

        public int GetFavoriteGameCount()
        {
            Write("GetFavoriteGameCount");
            return 1;
        }

        public ulong GetLobbyByIndex(int iLobby)
        {
            Write("GetLobbyByIndex");
            int index = 0;
            foreach (var lobby in Lobbies)
            {
                if (index == iLobby)
                {
                    return lobby.Key;
                }
                index++;
            }
            return 0;
        }

        public int GetLobbyChatEntry(ulong steamIDLobby, int iChatID, ulong pSteamIDUser, IntPtr pvData, int cubData, int peChatEntryType)
        {
            Write("GetLobbyChatEntry");
            return 1;
        }

        public string GetLobbyData(ulong steamIDLobby, string pchKey)
        {
            Write("GetLobbyData");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                if (lobby.LobbyData.ContainsKey(pchKey))
                {
                    return lobby.LobbyData[pchKey];
                }
            }
            return "";
        }

        public bool GetLobbyDataByIndex(ulong steamIDLobby, int iLobbyData, IntPtr pchKey, int cchKeyBufferSize, IntPtr pchValue, int cchValueBufferSize)
        {
            Write($"GetLobbyDataByIndex {steamIDLobby} {iLobbyData}");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                (var Key, var Value) = lobby.GetDataByIndex(iLobbyData);
                Marshal.Copy(Encoding.Default.GetBytes(Key), 0, pchKey, Encoding.Default.GetBytes(Key).Length);
                Marshal.Copy(Encoding.Default.GetBytes(Value), 0, pchValue, Encoding.Default.GetBytes(Value).Length);
            }
            return false;
        }

        public int GetLobbyDataCount(ulong steamIDLobby)
        {
            Write("GetLobbyDataCount");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                return lobby.LobbyData.Count;
            }
            return 0;
        }

        public bool GetLobbyGameServer(ulong steamIDLobby, uint punGameServerIP, uint punGameServerPort, ulong psteamIDGameServer)
        {
            Write("GetLobbyGameServer");
            return true;
        }

        public ulong GetLobbyMemberByIndex(ulong steamIDLobby, int iMember)
        {
            Write("GetLobbyMemberByIndex");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                if (lobby.Members.Count > iMember)
                {
                    return lobby.Members[iMember].m_SteamID;
                }
            }
            return 0;
        }

        public string GetLobbyMemberData(ulong steamIDLobby, ulong steamIDUser, string pchKey)
        {
            Write($"GetLobbyMemberData {steamIDLobby} {steamIDUser} {pchKey}");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                var member = lobby.Members.Find(m => m.m_SteamID == steamIDUser);
                if (member != null)
                {
                    var pchData = member.m_Data.Find(m => m.m_Key == pchKey);
                    return pchData == null ? "" : pchData.m_Value;
                }
            }
            return "";
        }

        public int GetLobbyMemberLimit(ulong steamIDLobby)
        {
            Write($"GetLobbyMemberLimit {steamIDLobby}");
            return 1;
        }

        public ulong GetLobbyOwner(ulong steamIDLobby)
        {
            Write($"GetLobbyOwner {steamIDLobby}");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                return lobby.Owner;
            }
            return 0;
        }

        public int GetNumLobbyMembers(ulong steamIDLobby)
        {
            Write("GetNumLobbyMembers");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                return lobby.Members.Count;
            }
            return 0;
        }

        public bool InviteUserToLobby(ulong steamIDLobby, ulong steamIDInvitee)
        {
            Write("InviteUserToLobby");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                // TODO: Send trougth socket
            }
            return true;
        }

        public SteamAPICall_t JoinLobby(ulong steamIDLobby)
        {
            Write("JoinLobby");

            LobbyEnter_t data = new LobbyEnter_t()
            {
                 m_ulSteamIDLobby = steamIDLobby,
                 m_bLocked = false,
                 m_EChatRoomEnterResponse = 1
            };

            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                if (lobby.MaxMembers == lobby.Members.Count)
                {
                    data.m_EChatRoomEnterResponse = 0;
                }
                if (!lobby.Joinable)
                {
                    data.m_EChatRoomEnterResponse = 0;
                }
            }
            return CallbackManager.AddCallbackResult(data);
        }

        public void LeaveLobby(ulong steamIDLobby)
        {
            Write("LeaveLobby");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {

            }
            // TODO: Send broadcast info 
        }

        public bool RemoveFavoriteGame(uint nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags)
        {
            Write("RemoveFavoriteGame");
            return true;
        }

        public bool RequestLobbyData(ulong steamIDLobby)
        {
            Write("RequestLobbyData");

            LobbyDataUpdate_t data = new LobbyDataUpdate_t()
            {
                m_bSuccess = 0
            };

            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {

            }

            CallbackManager.AddCallbackResult(data);

            return true;
        }

        public SteamAPICall_t RequestLobbyList()
        {
            Write("RequestLobbyList");
            // TODO: Request lobby list to network
            LobbyMatchList_t data = new LobbyMatchList_t()
            {
                m_nLobbiesMatching = 1
            };
            return CallbackManager.AddCallbackResult(data);
        }

        public bool SendLobbyChatMsg(ulong steamIDLobby, IntPtr pvMsgBody, int cubMsgBody)
        {
            Write("SendLobbyChatMsg");
            return true;
        }

        public bool SetLinkedLobby(ulong steamIDLobby, ulong steamIDLobbyDependent)
        {
            Write("SetLinkedLobby");
            return true;
        }

        public bool SetLobbyData(ulong steamIDLobby, string pchKey, string pchValue)
        {
            Write($"SetLobbyData {steamIDLobby} {pchKey} {pchValue}");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                lobby.LobbyData[pchKey] = pchValue;
                return true;
            }
            return false;
        }

        public void SetLobbyGameServer(ulong steamIDLobby, uint unGameServerIP, uint unGameServerPort, ulong steamIDGameServer)
        {
            Write("SetLobbyGameServer");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                lobby.Gameserver.SteamID = steamIDGameServer;
                lobby.Gameserver.IP = unGameServerIP;
                lobby.Gameserver.Port = unGameServerPort;
            }
        }

        public bool SetLobbyJoinable(ulong steamIDLobby, bool bLobbyJoinable)
        {
            Write("SetLobbyJoinable");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                lobby.Joinable = bLobbyJoinable;
                return false;
            }
            return true;
        }

        public void SetLobbyMemberData(ulong steamIDLobby, string pchKey, string pchValue)
        {
            Write($"SetLobbyMemberData {steamIDLobby} {pchKey} {pchValue}");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                lobby.LobbyData[pchKey] = pchValue;
            }
        }

        public bool SetLobbyMemberLimit(ulong steamIDLobby, int cMaxMembers)
        {
            Write("SetLobbyMemberLimit");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                lobby.MaxMembers = cMaxMembers;
                return true;
            }
            return false;
        }

        public bool SetLobbyOwner(ulong steamIDLobby, ulong steamIDNewOwner)
        {
            Write("SetLobbyOwner");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                lobby.Owner = steamIDNewOwner;
                return true;
            }
            return false;
        }

        public bool SetLobbyType(ulong steamIDLobby, int eLobbyType)
        {
            Write("SetLobbyType");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                lobby.Type = (ELobbyType)eLobbyType;
                return true;
            }
            return false;
        }

        public void CheckForPSNGameBootInvite(int iGameBootAttributes)
        {
            Write("CheckForPSNGameBootInvite");
        }

        private class SteamLobby
        {
            public ulong SteamID { get; set; }
            public ulong Owner { get; set; }
            public ELobbyType Type { get; set; }
            public List<LobbyMember> Members { get; set; }
            public Dictionary<string, string> LobbyData { get; set; }
            public int MaxMembers { get; set; }
            public bool Joinable { get; set; }
            public LobbyGameserver Gameserver { get; set; }

            public SteamLobby()
            {
                Members = new List<LobbyMember>();
                LobbyData = new Dictionary<string, string>();
                Type = ELobbyType.k_ELobbyTypePublic;
                Gameserver = new LobbyGameserver();
            }

            public (string Key, string Value) GetDataByIndex(int iLobbyData)
            {
                string key = "";
                string value = "";
                int index = 0;
                foreach (var item in LobbyData)
                {
                    if (index == iLobbyData)
                    {
                        key = item.Key;
                        value = item.Value;
                        break;
                    }
                    index++;
                }
                return (key, value);
            }

            internal class LobbyGameserver
            {
                public ulong SteamID { get; set; }
                public uint IP { get; set; }
                public uint Port { get; set; }
            }

            internal class LobbyMember
            {
                public ulong m_SteamID;
                public List<LobbyMetaData> m_Data;
                public LobbyMember()
                {
                    m_Data = new List<LobbyMetaData>();
                }
            }

            internal class LobbyMetaData
            {
                public string m_Key;
                public string m_Value;
            }
        }

        internal class FilterLobby
        {
            public int Distance { get; set; }
            public int SlotsAvailable { get; set; }
            public string KeyToMatch { get; set; }
            public int ValueToMatch { get; set; }
            public ELobbyComparison ComparisonType { get; set; }
            public string StringValueToMatch { get; set; }
        }
    }
}