using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Helper.JSON;
using SKYNET.IPC.Types;
using SKYNET.Managers;
using SKYNET.Types;
using SKYNET.Steamworks.Interfaces;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamMatchmaking : ISteamInterface
    {
        public  static SteamMatchmaking Instance;
        public  uint CurrentRequest;
        public ConcurrentDictionary<string, string> DataAwaiting;
        private FilterLobby filters;
        private List<FavoriteGame> FavoriteGames;

        public SteamMatchmaking()
        {
            Instance = this;
            InterfaceName = "SteamMatchmaking";
            InterfaceVersion = "SteamMatchMaking009";

            DataAwaiting = new ConcurrentDictionary<string, string>();
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
                    AppID = SteamEmulator.AppID,
                    SteamID = modCommon.GenerateSteamID(),
                    Type = (ELobbyType)eLobbyType,
                    MaxMembers = cMaxMembers,
                    Gameserver = new SteamLobby.LobbyGameserver()
                    {
                        IP = SteamGameServer.Instance.ServerData.IP,
                        Port = (uint)SteamGameServer.Instance.ServerData.QueryPort,
                    }
                };
                LocalLobby.Members.Add(new SteamLobby.LobbyMember()
                {
                    m_SteamID = (ulong)SteamEmulator.SteamID
                });

                if (DataAwaiting.Any())
                {
                    foreach (var KV in DataAwaiting)
                    {
                        LocalLobby.LobbyData.Add(KV.Key, KV.Value);
                    }
                    DataAwaiting.Clear();
                }

                LobbyCreated_t data = new LobbyCreated_t()
                {
                    m_eResult = EResult.k_EResultOK,
                    m_ulSteamIDLobby = LocalLobby.SteamID
                };

                callResult = CallbackManager.AddCallbackResult(data);

                LobbyEnter_t lobbyEnter = new LobbyEnter_t()
                {
                    m_ulSteamIDLobby = LocalLobby.SteamID,
                    m_rgfChatPermissions = 0,
                    m_bLocked = false,
                    m_EChatRoomEnterResponse = (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess
                };

                CallbackManager.AddCallbackResult(lobbyEnter);

                IPCManager.SendCreateLobby(LocalLobby);

                UpdateLobby(LocalLobby, LocalLobby.SteamID);

                SteamFriends.Instance.UpdateUserLobby((ulong)SteamEmulator.SteamID, LocalLobby.SteamID);
            }
            catch (Exception)
            {
            }
            return callResult;
        }

        public bool DeleteLobbyData(ulong steamIDLobby, string pchKey)
        {
            Write($"DeleteLobbyData (Lobby SteamID: {steamIDLobby}, Key: {pchKey})");
            if (GetLobby(steamIDLobby, out var lobby))
            {
                lobby.LobbyData.Clear();
                UpdateLobby(lobby, lobby.SteamID);
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
            var index = 0;
            var Response = CSteamID.Invalid;

            SteamLobby lobby = LobbyManager.GetLobbyByIndex(SteamEmulator.AppID, iLobby);
            if (lobby != null)
            {
                Response = (CSteamID)lobby.SteamID;
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

            if (GetLobby(steamIDLobby, out var lobby))
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
            if (GetLobby(steamIDLobby, out var lobby))
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
            if (GetLobby(steamIDLobby, out var lobby))
            {
                Count = lobby.LobbyData.Count;
            }
            Write($"GetLobbyDataCount (Lobby SteamID: {steamIDLobby}) = {Count}");
            return Count;
        }

        public bool GetLobbyGameServer(ulong steamIDLobby, ref uint punGameServerIP, ref uint punGameServerPort, ref ulong psteamIDGameServer)
        {
            var Result = false;
            if (GetLobby(steamIDLobby, out var lobby))
            {
                var Gameserver = lobby.Gameserver;
                if (Gameserver.Filled)
                {
                    punGameServerIP = (uint)IPAddress.Parse("10.31.0.4").Address; //lobby.Gameserver.IP;
                    punGameServerPort = lobby.Gameserver.Port;
                    psteamIDGameServer = lobby.Gameserver.SteamID;
                    Result = true;
                }                
            }
            Write($"GetLobbyGameServer (Lobby SteamID: {steamIDLobby}, IP = {punGameServerIP}, Port = {punGameServerPort}, GameserverID = {psteamIDGameServer}) = {Result}");
            return Result;
        }

        public CSteamID GetLobbyMemberByIndex(ulong steamIDLobby, int iMember)
        {
            ulong MemberByIndex = 0;
            if (GetLobby(steamIDLobby, out var lobby))
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
            if (GetLobby(steamIDLobby, out var lobby))
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
            int Result = 0;
            if (GetLobby(steamIDLobby, out var lobby))
            {
                Result = lobby.MaxMembers;
            }
            Write($"GetLobbyMemberLimit (Lobby SteamID: {steamIDLobby}) = {Result}");
            return Result;
        }

        public CSteamID GetLobbyOwner(ulong steamIDLobby)
        {
            ulong Owner = 0;
            if (GetLobby(steamIDLobby, out var lobby))
            {
                Owner = lobby.Owner;
            }
            Write($"GetLobbyOwner (Lobby SteamID: {steamIDLobby}) = {Owner}");
            return new CSteamID(Owner); 
        }

        public int GetNumLobbyMembers(ulong steamIDLobby)
        {
            int members = 0;
            if (GetLobby(steamIDLobby, out var lobby))
            {
                members = lobby.Members.Count;
            }
            Write($"GetNumLobbyMembers (Lobby SteamID: {steamIDLobby}) = {members}");
            return members;
        }

        public bool InviteUserToLobby(ulong steamIDLobby, ulong steamIDInvitee)
        {
            Write("InviteUserToLobby");
            if (GetLobby(steamIDLobby, out var lobby))
            {
                // TODO: Send through socket
            }
            return true;
        }

        public SteamAPICall_t JoinLobby(ulong steamIDLobby)
        {
            Write($"JoinLobby (Lobby SteamID: {steamIDLobby})");

            SteamAPICall_t APICall = 0;
            LobbyEnter_t data = new LobbyEnter_t()
            {
                m_ulSteamIDLobby = steamIDLobby,
                m_bLocked = false,
                m_EChatRoomEnterResponse = (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseError,
                m_rgfChatPermissions = 0
            };

            if (GetLobby(steamIDLobby, out var lobby))
            {
                APICall = CallbackManager.AddCallbackResult(data, false);
                IPCManager.SendLobbyJoinRequest(APICall, lobby);
            }
            else
            {
                return CallbackManager.AddCallbackResult(data);
            }

            return APICall;
        }

        public void LeaveLobby(ulong steamIDLobby)
        {
            Write($"LeaveLobby (Lobby SteamID: {steamIDLobby})");
            if (GetLobby(steamIDLobby, out var lobby))
            {
                if (lobby.Owner == SteamEmulator.SteamID)
                    IPCManager.SendLobbyRemove(lobby);
                else
                    IPCManager.SendLobbyLeave(lobby.Owner, lobby.SteamID);
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
            if (GetLobby(steamIDLobby, out var lobby))
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
            IPCManager.SendLobbyListRequest(CurrentRequest);
            SteamAPICall_t APICall = CallbackManager.AddCallbackResult(new LobbyMatchList_t(), false);
            ThreadPool.QueueUserWorkItem(WaitLobbyMatchList, APICall);
            return APICall;
        }

        private void WaitLobbyMatchList(object state)
        {
            SteamAPICall_t APICall = (SteamAPICall_t)state;
            Thread.Sleep(3000);
            uint Lobbies = (uint)LobbyManager.GetLobbies(SteamEmulator.AppID).Count;
            if (CallbackManager.GetCallResult(APICall, out var callback))
            {
                LobbyMatchList_t data = new LobbyMatchList_t()
                {
                    m_nLobbiesMatching = Lobbies
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
            var Result = false;
            Result = IPCManager.SetLobbyData(steamIDLobby, pchKey, pchValue);
            Write($"SetLobbyData (Lobby SteamID: {steamIDLobby}, Key: {pchKey}, Value: {pchValue}) = {Result}");
            return Result;
        }

        public void SetLobbyGameServer(ulong steamIDLobby, uint unGameServerIP, uint unGameServerPort, ulong steamIDGameServer)
        {
            Write($"SetLobbyGameServer (Lobby SteamID = {steamIDLobby}, EndPoint = {unGameServerIP}:{unGameServerPort}, GameServerID = {steamIDGameServer})");
            if (GetLobby(steamIDLobby, out var lobby))
            {
                Write($"SetLobbyGameServer *** IP = {lobby.Gameserver.IP}, Port = {lobby.Gameserver.Port})");

                uint IP = unGameServerIP != 0 ? unGameServerIP : NetworkHelper.ConvertFromIPAddress(NetworkHelper.GetIPAddress());
                
                lobby.Gameserver.SteamID = steamIDGameServer;
                lobby.Gameserver.IP = IP;
                lobby.Gameserver.Filled = true;

                if (unGameServerPort != 0)
                {
                    lobby.Gameserver.Port = unGameServerPort;
                }

                LobbyGameCreated_t data = new LobbyGameCreated_t()
                {
                    m_ulSteamIDLobby = lobby.Gameserver.SteamID,
                    m_ulSteamIDGameServer = steamIDGameServer,
                    m_unIP = lobby.Gameserver.IP,
                    m_usPort = (ushort)lobby.Gameserver.Port, 
                };
                CallbackManager.AddCallbackResult(data);

                IPCManager.SendLobbyGameServer(lobby);
                UpdateLobby(lobby, lobby.SteamID);
            }
        }

        public bool SetLobbyJoinable(ulong steamIDLobby, bool bLobbyJoinable)
        {
            Write($"SetLobbyJoinable (Lobby SteamID: {steamIDLobby}, Joinable: {bLobbyJoinable})");
            if (GetLobby(steamIDLobby, out var lobby))
            {
                lobby.Joinable = bLobbyJoinable;
                UpdateLobby(lobby, lobby.SteamID);
                return false;
            }
            return true;
        }

        public void SetLobbyMemberData(ulong steamIDLobby, string pchKey, string pchValue)
        {
            Write($"SetLobbyMemberData (Lobby SteamID: {steamIDLobby}, Key: {pchKey}, Value: {pchValue})");
            if (GetLobby(steamIDLobby, out var lobby))
            {
                lobby.LobbyData[pchKey] = pchValue;
                UpdateLobby(lobby, lobby.SteamID);
            }
        }

        public bool SetLobbyMemberLimit(ulong steamIDLobby, int cMaxMembers)
        {
            Write($"SetLobbyMemberLimit (Lobby SteamID: {steamIDLobby}, MaxMembers: {cMaxMembers})");
            if (GetLobby(steamIDLobby, out var lobby))
            {
                lobby.MaxMembers = cMaxMembers;
                UpdateLobby(lobby, lobby.SteamID);
                return true;
            }
            return false;
        }

        public bool SetLobbyOwner(ulong steamIDLobby, ulong steamIDNewOwner)
        {
            Write($"SetLobbyOwner (Lobby SteamID: {steamIDLobby},  SteamIDNewOwner: {steamIDNewOwner})");
            if (GetLobby(steamIDLobby, out var lobby))
            {
                lobby.Owner = steamIDNewOwner;
                UpdateLobby(lobby, lobby.SteamID);
                return true;
            }
            return false;
        }

        public bool SetLobbyType(ulong steamIDLobby, int eLobbyType)
        {
            Write($"SetLobbyType (Lobby SteamID: {steamIDLobby}, LobbyType: {(ELobbyType)eLobbyType})");
            if (GetLobby(steamIDLobby, out var lobby))
            {
                lobby.Type = (ELobbyType)eLobbyType;
                //UpdateLobby(lobby, lobby.SteamID);
                return true;
            }
            return false;
        }

        public void CheckForPSNGameBootInvite(int iGameBootAttributes)
        {
            Write("CheckForPSNGameBootInvite");
        }

        public void JoinResponse(IPC_LobbyJoinResponse JoinRespons, SteamLobby lobby)
        {
            try
            {
                if (lobby != null)
                {
                    LobbyEnter_t data = new LobbyEnter_t()
                    {
                        m_ulSteamIDLobby = lobby.SteamID,
                        m_bLocked = false,
                        m_EChatRoomEnterResponse = JoinRespons.ChatRoomEnterResponse,
                        m_rgfChatPermissions = 0
                    };
                    if (CallbackManager.GetCallResult(JoinRespons.CallbackHandle, out var callback))
                    {
                        callback.Data = data;
                        callback.ReadyToCall = true;
                    }
                    else
                    {
                        Write($"Not found callback result for handle {JoinRespons.CallbackHandle}");
                    }

                    SteamFriends.Instance.UpdateUserLobby((ulong)SteamEmulator.SteamID, lobby.SteamID);
                }
                else
                {
                    if (CallbackManager.GetCallResult(JoinRespons.CallbackHandle, out var callback))
                    {
                        callback.ReadyToCall = true;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public void RemoveLobby(ulong LobbyID)
        {
            if (GetLobby(LobbyID, out var lobby))
            {
                LobbyDataUpdate_t data = new LobbyDataUpdate_t()
                {
                    m_bSuccess = false,
                    m_ulSteamIDLobby = LobbyID,
                    m_ulSteamIDMember = lobby.Owner
                };

                CallbackManager.AddCallbackResult(data);
            }
        }

        public void LobbyDataUpdated(IPC_LobbyDataUpdate lobbyDataUpdate)
        {
            LobbyDataUpdate_t data = new LobbyDataUpdate_t()
            {
                m_bSuccess = true,
                m_ulSteamIDLobby = lobbyDataUpdate.SteamIDLobby,
                m_ulSteamIDMember = lobbyDataUpdate.SteamIDMember
            };
            CallbackManager.AddCallbackResult(data);

            var lobby = lobbyDataUpdate.SerializedLobby.FromJson<SteamLobby>();
            if (lobby != null)
            {
                foreach (var member in lobby.Members)
                {
                    SteamFriends.Instance.UpdateUserLobby(member.m_SteamID, lobbyDataUpdate.SteamIDLobby);
                }
            }
        }

        public void LobbyChatUpdated(IPC_LobbyChatUpdate lobbyChatUpdate)
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

        public SteamLobby GetLobby()
        {
            return null;
        }

        //private void CreateTestLobby()
        //{
        //    var id = modCommon.GenerateSteamID();
        //    Lobbies.TryAdd(id, new SteamLobby()
        //    {
        //        SteamID = id,
        //        AppID = SteamEmulator.AppID,
        //        Joinable = true,
        //        MaxMembers = 20,
        //        Owner = (ulong)SteamEmulator.SteamID,
        //        Type = ELobbyType.k_ELobbyTypePublic,
        //        Members = { },
        //        LobbyData =
        //        {
        //            { "lobby_type", "2" },
        //            { "0", "172" },
        //            { "1", "1" },
        //            { "2", "1" },
        //            { "3", "2000" },
        //            { "4", "10" },
        //            { "5", "0" },
        //            { "6", "1" },
        //            { "join_in_progress", "0" },
        //            { "session_state", "0" },
        //            { "name", SteamEmulator.PersonaName },
        //            { "owner", ((ulong)SteamEmulator.SteamID).ToString() },
        //            { "internalIP", "10.31.0.1" },
        //            { "publicIP", "10.31.0.1" },
        //            { "guid", Guid.NewGuid().ToString() },
        //            { "currentmembers", "1" },
        //        },
        //        Gameserver = new SteamLobby.LobbyGameserver()
        //        {
        //            Filled = true,
        //            IP = NetworkHelper.ConvertFromIPAddress(NetworkHelper.GetIPAddress()),
        //            Port = 27015,
        //            SteamID = (ulong)SteamEmulator.SteamID_GS 
        //        }
        //    });
        //}

        public void UpdateLobby(SteamLobby lobby, ulong Member, bool IncludeOwner = false)
        {
            if (IncludeOwner)
            {
                var data = new LobbyDataUpdate_t()
                {
                    m_bSuccess = true,
                    m_ulSteamIDLobby = lobby.SteamID,
                    m_ulSteamIDMember = Member
                };
                CallbackManager.AddCallbackResult(data);
            }

            var Members = lobby.Members.FindAll(m => m.m_SteamID != SteamEmulator.SteamID);
            foreach (var member in Members)
            {
                IPCManager.SendLobbyDataUpdate(member.m_SteamID, lobby.SteamID, Member, lobby);
            }
        }

        public bool GetLobby(ulong SteamID, out SteamLobby lobby, bool byOwner = false)
        {
            if (byOwner)
                lobby = LobbyManager.GetLobbyByOwner(SteamID);
            else
                lobby = LobbyManager.GetLobby(SteamID);

            if (lobby == null)
            {
                IPCManager.SendLobbiesRequest();
            }

            return lobby != null;
        }

        public class FilterLobby
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