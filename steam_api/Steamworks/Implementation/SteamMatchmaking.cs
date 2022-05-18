using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using SKYNET;
using SKYNET.Callback;
using SKYNET.Helper.JSON;
using SKYNET.Managers;
using SKYNET.Network.Packets;
using SKYNET.Steamworks;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamMatchmaking : ISteamInterface
    {
        public uint CurrentRequest;
        public ConcurrentDictionary<ulong, SteamLobby> Lobbies;
        private FilterLobby filters;
        private List<FavoriteGame> FavoriteGames;

        public SteamMatchmaking()
        {
            InterfaceName = "SteamMatchmaking";
            InterfaceVersion = "SteamMatchMaking009";
            Lobbies = new ConcurrentDictionary<SteamAPICall_t, SteamLobby>();
            filters = new FilterLobby();
            CurrentRequest = 0;
            FavoriteGames = new List<FavoriteGame>();

            string FavoriteGamesPath = Path.Combine(modCommon.GetPath(), "SKYNET", "Storage", "FavoriteGames.json");
            if (File.Exists(FavoriteGamesPath))
            {
                string fileContent = File.ReadAllText(FavoriteGamesPath);
                FavoriteGames = fileContent.FromJson<List<FavoriteGame>>();
            }
            //CreateTestLobby();
        }

        public int AddFavoriteGame(uint nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags, uint rTime32LastPlayedOnServer)
        {
            Write("AddFavoriteGame");
            var Game = new FavoriteGame()
            {
                AppID = nAppID,
                IP = nIP,
                ConnPort = nConnPort,
                QueryPort = nQueryPort,
                Flags = unFlags,
                Time32LastPlayedOnServer = rTime32LastPlayedOnServer
            };
            FavoriteGames.Add(Game);
            SaveFavoriteGames();
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
            ulong callResult = 0;
            try
            {
                var LocalLobby = new SteamLobby()
                {
                    Owner = (ulong)SteamEmulator.SteamID,
                    SteamID = modCommon.GenerateSteamID(),
                    Type = (ELobbyType)eLobbyType,
                    MaxMembers = cMaxMembers
                };
                LocalLobby.Members.Add(new SteamLobby.LobbyMember()
                {
                    m_SteamID = (ulong)SteamEmulator.SteamID
                });

                LobbyCreated_t data = new LobbyCreated_t()
                {
                    m_eResult = EResult.k_EResultOK,
                    m_ulSteamIDLobby = LocalLobby.SteamID
                };

                if (!Lobbies.TryAdd(LocalLobby.SteamID, LocalLobby))
                {
                    data.m_eResult = EResult.k_EResultFail;
                }

                SteamEmulator.SteamFriends.UpdateUserLobby((ulong)SteamEmulator.SteamID, LocalLobby.SteamID);

                callResult = CallbackManager.AddCallbackResult(data);
            }
            catch (Exception)
            {

            }

            return callResult;
        }

        public bool DeleteLobbyData(ulong steamIDLobby, string pchKey)
        {
            Write($"DeleteLobbyData (Lobby SteamID: {steamIDLobby}, Key: {pchKey})");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                lobby.LobbyData.Clear();
                return true;
            }
            return false;
        }

        public bool GetFavoriteGame(int iGame, ref uint pnAppID, ref uint pnIP, ref uint pnConnPort, ref uint pnQueryPort, ref uint punFlags, uint pRTime32LastPlayedOnServer)
        {
            Write("GetFavoriteGame");
            try
            {
                for (int i = 0; i < FavoriteGames.Count; i++)
                {
                    if (i == iGame)
                    {
                        var game = FavoriteGames[i];
                        pnAppID = game.AppID;
                        pnIP = game.IP;
                        pnConnPort = game.ConnPort;
                        pnQueryPort = game.QueryPort;
                        punFlags = game.Flags;
                        return true;
                    }
                }
            }
            catch 
            {
            }
            return false;
        }

        public int GetFavoriteGameCount()
        {
            Write("GetFavoriteGameCount");
            return FavoriteGames.Count;
        }

        public CSteamID GetLobbyByIndex(int iLobby)
        {
            int index = 0;
            CSteamID Response = CSteamID.Invalid;
            foreach (var lobby in Lobbies)
            {
                if (index == iLobby)
                {
                    Response = new CSteamID(lobby.Value.SteamID);
                }
                index++;
            }
            Write($"GetLobbyByIndex (Lobby Index: {iLobby}) = {(ulong)Response}");
            return Response;
        }

        public int GetLobbyChatEntry(ulong steamIDLobby, int iChatID, ulong pSteamIDUser, IntPtr pvData, int cubData, int peChatEntryType)
        {
            Write("GetLobbyChatEntry");
            return 1;
        }

        public string GetLobbyData(ulong steamIDLobby, string pchKey)
        {
            string Result = "";
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                if (lobby.LobbyData.ContainsKey(pchKey))
                {
                    Result = lobby.LobbyData[pchKey];
                }
            }
            Write($"GetLobbyData (Lobby SteamID: {steamIDLobby}, Key: {pchKey}) = {Result}");
            return Result;
        }

        public bool GetLobbyDataByIndex(ulong steamIDLobby, int iLobbyData, IntPtr pchKey, int cchKeyBufferSize, IntPtr pchValue, int cchValueBufferSize)
        {
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                (var Key, var Value) = lobby.GetDataByIndex(iLobbyData);
                Marshal.Copy(Encoding.Default.GetBytes(Key), 0, pchKey, Encoding.Default.GetBytes(Key).Length);
                Marshal.Copy(Encoding.Default.GetBytes(Value), 0, pchValue, Encoding.Default.GetBytes(Value).Length);
                Write($"GetLobbyDataByIndex (Lobby SteamID: {steamIDLobby},  LobbyData index: {iLobbyData}) = ({Key} {Value})");
                return true;
            }
            Write($"GetLobbyDataByIndex (Lobby SteamID: {steamIDLobby},  LobbyData index: {iLobbyData})");
            return false;
        }

        public int GetLobbyDataCount(ulong steamIDLobby)
        {
            int Count = 0;
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                Count = lobby.LobbyData.Count;
            }
            Write($"GetLobbyDataCount (Lobby SteamID: {steamIDLobby}) = {Count}");
            return Count;
        }

        public bool GetLobbyGameServer(ulong steamIDLobby, uint punGameServerIP, uint punGameServerPort, ulong psteamIDGameServer)
        {
            Write("GetLobbyGameServer");
            return true;
        }

        public CSteamID GetLobbyMemberByIndex(ulong steamIDLobby, int iMember)
        {
            ulong MemberByIndex = 0;
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                if (lobby.Members.Count > iMember)
                {
                    MemberByIndex = lobby.Members[iMember].m_SteamID;
                }
            }
            Write($"GetLobbyMemberByIndex (Lobby SteamID: {steamIDLobby}, Index: {iMember}) = {MemberByIndex}");
            return new CSteamID(MemberByIndex);
        }

        public string GetLobbyMemberData(ulong steamIDLobby, ulong steamIDUser, string pchKey)
        {
            Write($"GetLobbyMemberData (Lobby SteamID: {steamIDLobby}, User SteamID: {steamIDUser} Key: {pchKey})");
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
            Write($"GetLobbyMemberLimit (Lobby SteamID: {steamIDLobby})");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                return lobby.MaxMembers;
            }
            return 0;
        }

        public CSteamID GetLobbyOwner(ulong steamIDLobby)
        {
            ulong Owner = 0;
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                Owner = lobby.Owner;
            }
            Write($"GetLobbyOwner (Lobby SteamID: {steamIDLobby}) = {Owner}");
            return new CSteamID(Owner); 
        }

        public int GetNumLobbyMembers(ulong steamIDLobby)
        {
            int members = 0;
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                members = lobby.Members.Count;
            }
            Write($"GetNumLobbyMembers (Lobby SteamID: {steamIDLobby}) = {members}");
            return members;
        }

        public bool InviteUserToLobby(ulong steamIDLobby, ulong steamIDInvitee)
        {
            Write("InviteUserToLobby");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                // TODO: Send through socket
            }
            return true;
        }

        public SteamAPICall_t JoinLobby(ulong steamIDLobby)
        {
            Write($"JoinLobby (Lobby SteamID: {steamIDLobby})");

            LobbyEnter_t data = new LobbyEnter_t()
            {
                 m_ulSteamIDLobby = steamIDLobby,
                 m_bLocked = false,
                 m_EChatRoomEnterResponse = (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess,
                 m_rgfChatPermissions = 1
            };

            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                if (lobby.MaxMembers >= lobby.Members.Count)
                {
                    data.m_EChatRoomEnterResponse = 0;
                }
                if (!lobby.Joinable)
                {
                    //data.m_EChatRoomEnterResponse = 0;
                }
                if (data.m_EChatRoomEnterResponse == (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
                {
                    NetworkManager.SendLobbyJoinRequest(lobby);
                }
            }
            return CallbackManager.AddCallbackResult(data);
        }

        public void LeaveLobby(ulong steamIDLobby)
        {
            Write($"LeaveLobby (Lobby SteamID: {steamIDLobby})");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                NetworkManager.SendLobbyLeave(lobby.Owner, lobby.SteamID);
                Lobbies.TryRemove(steamIDLobby, out _);
            }
        }

        public bool RemoveFavoriteGame(uint nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags)
        {
            Write("RemoveFavoriteGame");
            var game = FavoriteGames.Find(g => g.AppID == nAppID && g.IP == nIP && g.ConnPort == nConnPort);
            FavoriteGames.Remove(game);
            return true;
        }

        public bool RequestLobbyData(ulong steamIDLobby)
        {
            bool Result = false;
            LobbyDataUpdate_t data = new LobbyDataUpdate_t()
            {
                m_bSuccess = false,
            };
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                Result = true;
                data.m_bSuccess = true;
                data.m_ulSteamIDLobby = lobby.SteamID;
                data.m_ulSteamIDMember = lobby.Owner; 
            }

            CallbackManager.AddCallbackResult(data);
            Write($"RequestLobbyData (Lobby SteamID: {steamIDLobby}) = {Result}");
            return Result;
        }

        public SteamAPICall_t RequestLobbyList()
        {
            Write($"RequestLobbyList");
            CurrentRequest++;
            NetworkManager.RequestLobbyList(CurrentRequest);

            SteamAPICall_t APICall = CallbackManager.AddCallbackResult(new LobbyMatchList_t(), false);
            ThreadPool.QueueUserWorkItem(WaitLobbyMatchList, APICall);
            return APICall;
        }

        private void WaitLobbyMatchList(object state)
        {
            SteamAPICall_t APICall = (SteamAPICall_t)state;
            Thread.Sleep(3000);
            if (CallbackManager.CallbackResults.TryGetValue(APICall, out var callback))
            {
                LobbyMatchList_t data = new LobbyMatchList_t()
                {
                    m_nLobbiesMatching = (uint)Lobbies.Count
                };
                callback.Data = data;
                callback.ReadyToCall = true;
            }
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
            Write($"SetLobbyData (Lobby SteamID: {steamIDLobby}, Key: {pchKey}, Value: {pchValue})");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                lobby.LobbyData[pchKey] = pchValue;
                NetworkManager.BroadcastLobbyData(lobby, pchKey, pchValue);
                //LobbyDataUpdate_t
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
            Write($"SetLobbyJoinable (Lobby SteamID: {steamIDLobby}, Joinable: {bLobbyJoinable})");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                lobby.Joinable = bLobbyJoinable;
                return false;
            }
            return true;
        }

        public void SetLobbyMemberData(ulong steamIDLobby, string pchKey, string pchValue)
        {
            Write($"SetLobbyMemberData (Lobby SteamID: {steamIDLobby}, Key: {pchKey}, Value: {pchValue})");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                lobby.LobbyData[pchKey] = pchValue;
            }
        }

        public bool SetLobbyMemberLimit(ulong steamIDLobby, int cMaxMembers)
        {
            Write($"SetLobbyMemberLimit (Lobby SteamID: {steamIDLobby}, MaxMembers: {cMaxMembers})");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                lobby.MaxMembers = cMaxMembers;
                return true;
            }
            return false;
        }

        public bool SetLobbyOwner(ulong steamIDLobby, ulong steamIDNewOwner)
        {
            Write($"SetLobbyOwner (Lobby SteamID: {steamIDLobby},  SteamIDNewOwner: {steamIDNewOwner})");
            if (Lobbies.TryGetValue(steamIDLobby, out var lobby))
            {
                lobby.Owner = steamIDNewOwner;
                return true;
            }
            return false;
        }

        public bool SetLobbyType(ulong steamIDLobby, int eLobbyType)
        {
            Write($"SetLobbyType (Lobby SteamID: {steamIDLobby}, LobbyType: {(ELobbyType)eLobbyType})");
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

        public void JoinResponse(bool success, SteamLobby lobby)
        {
            try
            {
                if (success)
                {
                    Lobbies[lobby.SteamID] = lobby;

                    LobbyDataUpdate_t data = new LobbyDataUpdate_t()
                    {
                        m_bSuccess = true,
                        m_ulSteamIDLobby = lobby.SteamID,
                        m_ulSteamIDMember = lobby.Owner
                    };

                    CallbackManager.AddCallbackResult(data);
                }
                else
                {
                    
                }
            }
            catch (Exception)
            {

            }
        }

        public void LobbyDataUpdated(NET_LobbyDataUpdate lobbyDataUpdate)
        {
            if (Lobbies.TryGetValue(lobbyDataUpdate.LobbyID, out var lobby))
            {
                lobby.LobbyData[lobbyDataUpdate.Key] = lobbyDataUpdate.Value;

                LobbyDataUpdate_t data = new LobbyDataUpdate_t()
                {
                    m_bSuccess = true,
                    m_ulSteamIDLobby = lobby.SteamID,
                    m_ulSteamIDMember = lobby.Owner
                };

                CallbackManager.AddCallbackResult(data);
            }
        }

        public void LobbyChatUpdated(NET_LobbyChatUpdate lobbyChatUpdate)
        {
            LobbyChatUpdate_t data = new LobbyChatUpdate_t()
            {
                m_ulSteamIDLobby = lobbyChatUpdate.SteamIDLobby,
                m_ulSteamIDUserChanged = lobbyChatUpdate.SteamIDUserChanged,
                m_rgfChatMemberStateChange = lobbyChatUpdate.ChatMemberStateChange,
                m_ulSteamIDMakingChange = lobbyChatUpdate.SteamIDMakingChange
            };

            CallbackManager.AddCallbackResult(data);
        }

        private void SaveFavoriteGames()
        {
            try
            {
                string FavoriteGamesPath = Path.Combine(modCommon.GetPath(), "SKYNET", "Storage", "FavoriteGames.json");
                modCommon.EnsureDirectoryExists(FavoriteGamesPath, true);
                string json = FavoriteGames.ToJson();
                File.WriteAllText(FavoriteGamesPath, json);
            }
            catch
            {
            }
        }

        private void CreateTestLobby()
        {
            var id = modCommon.GenerateSteamID();
            Lobbies.TryAdd(id, new SteamLobby()
            {
                SteamID = id,
                Joinable = true,
                MaxMembers = 20,
                Owner = (ulong)SteamEmulator.SteamID,
                Type = ELobbyType.k_ELobbyTypePublic,
                Members = { new SteamLobby.LobbyMember() { m_SteamID = (ulong)SteamEmulator.SteamID } },
                LobbyData =
                {
                    { "lobby_type", "2" },
                    { "0", "172" },
                    { "1", "1" },
                    { "2", "1" },
                    { "3", "2000" },
                    { "4", "10" },
                    { "5", "0" },
                    { "6", "1" },
                    { "join_in_progress", "0" },
                    { "session_state", "0" },
                    { "name", SteamEmulator.PersonaName },
                    { "owner", ((ulong)SteamEmulator.SteamID).ToString() },
                    { "internalIP", "10.31.0.1" },
                    { "publicIP", "10.31.0.1" },
                }
            });
        }

        public SteamLobby GetLobby(ulong SteamID)
        {
            Lobbies.TryGetValue(SteamID, out var lobby);
            return lobby;
        }

        public bool GetLobby(ulong SteamID, out SteamLobby lobby)
        {
            return Lobbies.TryGetValue(SteamID, out lobby);
        }

        public SteamLobby GetLobbyByOwner(ulong ownerID)
        {
            return SteamEmulator.SteamMatchmaking.Lobbies.Where(l => l.Value.Owner == ownerID).Select(l => l.Value).FirstOrDefault();
        }

        public class SteamLobby
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

            public class LobbyGameserver
            {
                public ulong SteamID { get; set; }
                public uint IP { get; set; }
                public uint Port { get; set; }
            }

            public class LobbyMember
            {
                public ulong m_SteamID;
                public List<LobbyMetaData> m_Data;
                public LobbyMember()
                {
                    m_Data = new List<LobbyMetaData>();
                }
            }

            public class LobbyMetaData
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

        internal class FavoriteGame
        {
            public uint AppID { get; set; }
            public uint IP { get; set; }
            public uint ConnPort { get; set; }
            public uint QueryPort { get; set; }
            public uint Flags { get; set; }
            public uint Time32LastPlayedOnServer { get; set; }
        }
    }
}