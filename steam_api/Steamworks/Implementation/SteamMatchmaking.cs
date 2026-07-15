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
using SKYNET.Helpers;
using SKYNET.Helpers.JSON;
//using SKYNET.IPC.Types;
using SKYNET.Managers;
using SKYNET.Types;
using SKYNET.Steamworks.Interfaces;
using SKYNET.Network.Packets;

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

            string FavoriteGamesPath = Path.Combine(Common.GetPath(), "SKYNET", "Storage", "FavoriteGames.json");
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
                if (SkyNetApiClient.IsEnabled)
                {
                    var lobbyData = DataAwaiting.ToDictionary(k => k.Key, v => v.Value);
                    DataAwaiting.Clear();
                    return WorkQueue.EnqueueCallbackResult(new LobbyCreated_t
                    {
                        m_eResult = EResult.k_EResultFail,
                        m_ulSteamIDLobby = 0
                    }, () =>
                    {
                        var lobby = SkyNetApiClient.CreateLobby(SteamEmulator.AppID, eLobbyType, cMaxMembers, lobbyData);
                        if (lobby == null)
                        {
                            return new LobbyCreated_t
                            {
                                m_eResult = EResult.k_EResultFail,
                                m_ulSteamIDLobby = 0
                            };
                        }

                        LobbyManager.UpsertLobby(lobby);

                        CallbackManager.AddCallback(new LobbyEnter_t
                        {
                            m_ulSteamIDLobby = lobby.SteamID,
                            m_rgfChatPermissions = 0,
                            m_bLocked = false,
                            m_EChatRoomEnterResponse = (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess
                        });

                        CallbackManager.AddCallback(new LobbyDataUpdate_t
                        {
                            m_bSuccess = true,
                            m_ulSteamIDLobby = lobby.SteamID,
                            m_ulSteamIDMember = (ulong)SteamEmulator.SteamID
                        });

                        return new LobbyCreated_t
                        {
                            m_eResult = EResult.k_EResultOK,
                            m_ulSteamIDLobby = lobby.SteamID
                        };
                    }, name: "CreateLobby");
                }

                var LocalLobby = new SteamLobby()
                {
                    Owner = (ulong)SteamEmulator.SteamID,
                    AppID = SteamEmulator.AppID,
                    SteamID = Common.GenerateSteamID(),
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

                CallbackManager.AddCallback(lobbyEnter);
                LobbyManager.UpsertLobby(LocalLobby);
                UpdateLobby(LocalLobby, LocalLobby.SteamID);
            }
            catch (Exception)
            {
            }
            return callResult;
        }

        public bool DeleteLobbyData(ulong steamIDLobby, string pchKey)
        {
            Write($"DeleteLobbyData (Lobby SteamID: {steamIDLobby}, Key: {pchKey})");
            if (SkyNetApiClient.IsEnabled)
            {
                if (GetLobby(steamIDLobby, out var cached))
                {
                    cached.LobbyData.Remove(pchKey);
                    EmitLobbyDataUpdate(steamIDLobby, (ulong)SteamEmulator.SteamID, true);
                }

                WorkQueue.Enqueue("DeleteLobbyData", () =>
                {
                    if (SkyNetApiClient.DeleteLobbyData(steamIDLobby, pchKey))
                    {
                        QueueLobbyRefresh(steamIDLobby, (ulong)SteamEmulator.SteamID);
                    }
                }, "lobby:data-delete:" + steamIDLobby + ":" + pchKey);
                return true;
            }

            if (GetLobby(steamIDLobby, out var lobby))
            {
                if (lobby.LobbyData.Remove(pchKey))
                {
                    EmitLobbyDataUpdate(steamIDLobby, (ulong)SteamEmulator.SteamID, true);
                    return true;
                }
            }
            return false;
        }

        public bool GetFavoriteGame(
            int iGame,
            IntPtr pnAppID,
            IntPtr pnIP,
            IntPtr pnConnPort,
            IntPtr pnQueryPort,
            IntPtr punFlags,
            IntPtr pRTime32LastPlayedOnServer)
        {
            Write("GetFavoriteGame");
            try
            {
                for (int i = 0; i < FavoriteGames.Count; i++)
                {
                    if (i == iGame)
                    {
                        var game = FavoriteGames[i];
                        WriteUInt32(pnAppID, game.AppID);
                        WriteUInt32(pnIP, game.IP);
                        WriteUInt16(pnConnPort, game.ConnPort);
                        WriteUInt16(pnQueryPort, game.QueryPort);
                        WriteUInt32(punFlags, game.Flags);
                        WriteUInt32(pRTime32LastPlayedOnServer, game.Time32LastPlayedOnServer);
                        return true;
                    }
                }
            }
            catch 
            {
            }
            return false;
        }

        public bool GetFavoriteGame(int iGame, ref uint pnAppID, ref uint pnIP, ref uint pnConnPort, ref uint pnQueryPort, ref uint punFlags, ref uint pRTime32LastPlayedOnServer)
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
                    pRTime32LastPlayedOnServer = game.Time32LastPlayedOnServer;
                    return true;
                }
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
            var Response = CSteamID.Invalid;

            if (SkyNetApiClient.IsEnabled)
            {
                var filterSnapshot = CloneFilter(filters);
                WorkQueue.Enqueue("QueryLobbies index", () =>
                {
                    var lobbies = SkyNetApiClient.QueryLobbies(SteamEmulator.AppID, filterSnapshot);
                    if (lobbies != null)
                    {
                        LobbyManager.UpdateLobbies(lobbies);
                    }
                }, "lobby:query:index");
            }

            SteamLobby lobby = LobbyManager.GetLobbyByIndex(SteamEmulator.AppID, iLobby);
            if (lobby != null)
            {
                Response = (CSteamID)lobby.SteamID;
            }

            Write($"GetLobbyByIndex (Lobby Index: {iLobby}) = {(ulong)Response}");
            return Response;
        }

        public int GetLobbyChatEntry(ulong steamIDLobby, int iChatID, IntPtr pSteamIDUser, IntPtr pvData, int cubData, IntPtr peChatEntryType)
        {
            if (!LobbyManager.TryGetChat(steamIDLobby, iChatID, out var sender, out var data, out var entryType))
            {
                NativeSteamId.Write(pSteamIDUser, CSteamID.Invalid);
                if (peChatEntryType != IntPtr.Zero)
                {
                    Marshal.WriteInt32(peChatEntryType, 0);
                }
                return 0;
            }

            NativeSteamId.Write(pSteamIDUser, (CSteamID)sender);
            if (peChatEntryType != IntPtr.Zero)
            {
                Marshal.WriteInt32(peChatEntryType, entryType);
            }

            var toCopy = Math.Min(cubData, data.Length);
            if (pvData != IntPtr.Zero && toCopy > 0)
            {
                Marshal.Copy(data, 0, pvData, toCopy);
            }

            Write($"GetLobbyChatEntry (Lobby: {steamIDLobby}, id: {iChatID}) -> {toCopy} bytes from {sender}");
            return toCopy;
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
                    punGameServerIP = lobby.Gameserver.IP; //(uint)IPAddress.Parse("10.31.0.4").Address; //
                    punGameServerPort = lobby.Gameserver.Port;
                    psteamIDGameServer = lobby.Gameserver.SteamID;
                    Result = true;
                }                
            }
            Write($"GetLobbyGameServer (Lobby SteamID: {steamIDLobby}, IP = {punGameServerIP}, Port = {punGameServerPort}, GameserverID = {psteamIDGameServer}) = {Result}");
            return Result;
        }

        public bool GetLobbyGameServer(ulong steamIDLobby, IntPtr punGameServerIP, IntPtr punGameServerPort, IntPtr psteamIDGameServer)
        {
            uint ip = 0;
            uint port = 0;
            ulong steamID = 0;
            bool result = GetLobbyGameServer(steamIDLobby, ref ip, ref port, ref steamID);
            WriteUInt32(punGameServerIP, ip);
            WriteUInt16(punGameServerPort, port);
            NativeSteamId.Write(psteamIDGameServer, new CSteamID(steamID));
            return result;
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

            if (SkyNetApiClient.IsEnabled)
            {
                return WorkQueue.EnqueueCallbackResult(data, () =>
                {
                    var joinedLobby = SkyNetApiClient.JoinLobby(steamIDLobby);
                    if (joinedLobby == null)
                    {
                        return data;
                    }

                    LobbyManager.UpsertLobby(joinedLobby);
                    var joined = new LobbyEnter_t
                    {
                        m_ulSteamIDLobby = steamIDLobby,
                        m_bLocked = false,
                        m_EChatRoomEnterResponse = (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess,
                        m_rgfChatPermissions = 0
                    };
                    CallbackManager.AddCallback(new LobbyDataUpdate_t
                    {
                        m_bSuccess = true,
                        m_ulSteamIDLobby = steamIDLobby,
                        m_ulSteamIDMember = (ulong)SteamEmulator.SteamID
                    });
                    CallbackManager.AddCallback(new LobbyChatUpdate_t
                    {
                        m_ulSteamIDLobby = steamIDLobby,
                        m_ulSteamIDUserChanged = (ulong)SteamEmulator.SteamID,
                        m_ulSteamIDMakingChange = (ulong)SteamEmulator.SteamID,
                        m_rgfChatMemberStateChange = (uint)EChatMemberStateChange.k_EChatMemberStateChangeEntered
                    });

                    return joined;
                }, name: "JoinLobby");
            }

            if (GetLobby(steamIDLobby, out var lobby))
            {
                APICall = CallbackManager.AddCallbackResult(data, false);
                NetworkManager.SendLobbyJoinRequest(APICall, lobby);
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
            if (SkyNetApiClient.IsEnabled)
            {
                LobbyManager.RemoveLobby(steamIDLobby);
                CallbackManager.AddCallback(new LobbyChatUpdate_t
                {
                    m_ulSteamIDLobby = steamIDLobby,
                    m_ulSteamIDUserChanged = (ulong)SteamEmulator.SteamID,
                    m_ulSteamIDMakingChange = (ulong)SteamEmulator.SteamID,
                    m_rgfChatMemberStateChange = (uint)EChatMemberStateChange.k_EChatMemberStateChangeLeft
                });
                CallbackManager.AddCallback(new LobbyDataUpdate_t
                {
                    m_bSuccess = false,
                    m_ulSteamIDLobby = steamIDLobby,
                    m_ulSteamIDMember = (ulong)SteamEmulator.SteamID
                });
                WorkQueue.Enqueue("LeaveLobby", () => SkyNetApiClient.LeaveLobby(steamIDLobby), "lobby:leave:" + steamIDLobby);
                return;
            }

            if (GetLobby(steamIDLobby, out var lobby))
            {
                /*
                if (lobby.Owner == SteamEmulator.SteamID)
                    SendLobbyRemove(lobby);
                else
                    SendLobbyLeave(lobby.Owner, lobby.SteamID);
                */
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
            if (SkyNetApiClient.IsEnabled)
            {
                if (LobbyManager.GetLobby(steamIDLobby, out var cached))
                {
                    Result = true;
                    data.m_bSuccess = true;
                    data.m_ulSteamIDLobby = cached.SteamID;
                    data.m_ulSteamIDMember = cached.Owner;
                }

                WorkQueue.Enqueue("RequestLobbyData", () =>
                {
                    var refreshed = SkyNetApiClient.RefreshLobby(steamIDLobby);
                    var callback = data;
                    if (refreshed != null)
                    {
                        LobbyManager.UpsertLobby(refreshed);
                        callback.m_bSuccess = true;
                        callback.m_ulSteamIDLobby = refreshed.SteamID;
                        callback.m_ulSteamIDMember = refreshed.Owner;
                    }
                    CallbackManager.AddCallback(callback);
                }, "lobby:refresh:" + steamIDLobby, true);
                Write($"RequestLobbyData (Lobby SteamID: {steamIDLobby}) = {Result}");
                return true;
            }

            if (GetLobby(steamIDLobby, out var lobby))
            {
                Result = true;
                data.m_bSuccess = true;
                data.m_ulSteamIDLobby = lobby.SteamID;
                data.m_ulSteamIDMember = lobby.Owner; 
            }

            CallbackManager.AddCallback(data);
            Write($"RequestLobbyData (Lobby SteamID: {steamIDLobby}) = {Result}");
            return Result;
        }

        public SteamAPICall_t RequestLobbyList()
        {
            Write($"RequestLobbyList");
            CurrentRequest++;
            if (SkyNetApiClient.IsEnabled)
            {
                var filterSnapshot = CloneFilter(filters);
                return WorkQueue.EnqueueCallbackResult(new LobbyMatchList_t(), () =>
                {
                    var lobbies = SkyNetApiClient.QueryLobbies(SteamEmulator.AppID, filterSnapshot) ?? new List<SteamLobby>();
                    LobbyManager.UpdateLobbies(lobbies);
                    return new LobbyMatchList_t
                    {
                        m_nLobbiesMatching = (uint)lobbies.Count
                    };
                }, name: "RequestLobbyList", coalesceKey: "lobby:query:list");
            }

            SteamAPICall_t APICall = CallbackManager.AddCallbackResult(new LobbyMatchList_t(), false);
            ThreadPool.QueueUserWorkItem(WaitLobbyMatchList, APICall);
            return APICall;
        }

        private void WaitLobbyMatchList(object state)
        {
            //SteamAPICall_t APICall = (SteamAPICall_t)state;
            //Thread.Sleep(3000);
            //uint Lobbies = (uint)LobbyManager.GetLobbies(SteamEmulator.AppID).Count;
            //if (CallbackManager.GetCallResult(APICall, out var callback))
            //{
            //    LobbyMatchList_t data = new LobbyMatchList_t()
            //    {
            //        m_nLobbiesMatching = Lobbies
            //    };
            //    callback.Data = data;
            //    callback.ReadyToCall = true;
            //}
        }

        public bool SendLobbyChatMsg(ulong steamIDLobby, IntPtr pvMsgBody, int cubMsgBody)
        {
            if (pvMsgBody == IntPtr.Zero || cubMsgBody <= 0)
            {
                Write($"SendLobbyChatMsg (Lobby: {steamIDLobby}) empty body");
                return false;
            }

            var body = pvMsgBody.GetBytes(cubMsgBody);
            if (SkyNetApiClient.IsEnabled)
            {
                // Non-blocking: the server fans the message out to every member
                // (including us) as a lobby_chat event, so we receive our own copy
                // via LobbyChatMsg_t like real Steam. No local echo (would double it).
                WorkQueue.Enqueue("SendLobbyChatMsg", () => SkyNetApiClient.SendLobbyChatMsg(steamIDLobby, body), null);
            }

            Write($"SendLobbyChatMsg (Lobby: {steamIDLobby}, {body.Length} bytes)");
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
            if (SkyNetApiClient.IsEnabled)
            {
                if (steamIDLobby == 0)
                {
                    DataAwaiting[pchKey] = pchValue;
                    return true;
                }

                if (GetLobby(steamIDLobby, out var cached))
                {
                    cached.LobbyData[pchKey] = pchValue;
                    EmitLobbyDataUpdate(steamIDLobby, (ulong)SteamEmulator.SteamID, true);
                }

                Result = true;
                WorkQueue.Enqueue("SetLobbyData", () =>
                {
                    if (SkyNetApiClient.UpdateLobbyData(steamIDLobby, pchKey, pchValue))
                    {
                        QueueLobbyRefresh(steamIDLobby, (ulong)SteamEmulator.SteamID);
                    }
                }, "lobby:data:" + steamIDLobby + ":" + pchKey);
                Write($"SetLobbyData (Lobby SteamID: {steamIDLobby}, Key: {pchKey}, Value: {pchValue}) = {Result}");
                return Result;
            }

            if (GetLobby(steamIDLobby, out var lobby))
            {
                lobby.LobbyData[pchKey] = pchValue;
                EmitLobbyDataUpdate(steamIDLobby, (ulong)SteamEmulator.SteamID, true);
                Result = true;
            }

            Write($"SetLobbyData (Lobby SteamID: {steamIDLobby}, Key: {pchKey}, Value: {pchValue}) = {Result}");
            return Result;
        }

        public void SetLobbyGameServer(ulong steamIDLobby, uint unGameServerIP, uint unGameServerPort, ulong steamIDGameServer)
        {
            Write($"SetLobbyGameServer (Lobby SteamID = {steamIDLobby}, EndPoint = {unGameServerIP}:{unGameServerPort}, GameServerID = {steamIDGameServer})");

            if (SkyNetApiClient.IsEnabled)
            {
                if (GetLobby(steamIDLobby, out var cached))
                {
                    cached.Gameserver.IP = unGameServerIP;
                    cached.Gameserver.Port = unGameServerPort;
                    cached.Gameserver.SteamID = steamIDGameServer;
                    cached.Gameserver.Filled = true;
                    EmitLobbyDataUpdate(steamIDLobby, cached.Owner, true);
                }

                WorkQueue.Enqueue("SetLobbyGameServer", () =>
                {
                    if (SkyNetApiClient.SetLobbyGameServer(steamIDLobby, unGameServerIP, unGameServerPort, steamIDGameServer))
                    {
                        QueueLobbyRefresh(steamIDLobby, (ulong)SteamEmulator.SteamID);
                    }
                }, "lobby:gameserver:" + steamIDLobby, true);
            }

            LobbyGameCreated_t data = new LobbyGameCreated_t()
            {
                m_ulSteamIDLobby = steamIDLobby,
                m_ulSteamIDGameServer = steamIDGameServer,
                m_unIP = unGameServerIP,
                m_usPort = (ushort)unGameServerPort,
            };
            CallbackManager.AddCallback(data);
        }

        public bool SetLobbyJoinable(ulong steamIDLobby, bool bLobbyJoinable)
        {
            Write($"SetLobbyJoinable (Lobby SteamID: {steamIDLobby}, Joinable: {bLobbyJoinable})");
            if (SkyNetApiClient.IsEnabled)
            {
                if (GetLobby(steamIDLobby, out var cached))
                {
                    cached.Joinable = bLobbyJoinable;
                    EmitLobbyDataUpdate(steamIDLobby, cached.Owner, true);
                }
                WorkQueue.Enqueue("SetLobbyJoinable", () =>
                {
                    if (SkyNetApiClient.SetLobbyJoinable(steamIDLobby, bLobbyJoinable))
                    {
                        QueueLobbyRefresh(steamIDLobby, (ulong)SteamEmulator.SteamID);
                    }
                }, "lobby:joinable:" + steamIDLobby);
                return true;
            }

            if (GetLobby(steamIDLobby, out var lobby))
            {
                lobby.Joinable = bLobbyJoinable;
                EmitLobbyDataUpdate(steamIDLobby, lobby.Owner, true);
                return true;
            }
            return false;
        }

        public void SetLobbyMemberData(ulong steamIDLobby, string pchKey, string pchValue)
        {
            Write($"SetLobbyMemberData (Lobby SteamID: {steamIDLobby}, Key: {pchKey}, Value: {pchValue})");
            if (SkyNetApiClient.IsEnabled)
            {
                UpsertLocalMemberData(steamIDLobby, (ulong)SteamEmulator.SteamID, pchKey, pchValue);
                EmitLobbyDataUpdate(steamIDLobby, (ulong)SteamEmulator.SteamID, true);
                WorkQueue.Enqueue("SetLobbyMemberData", () =>
                {
                    if (SkyNetApiClient.SetLobbyMemberData(steamIDLobby, pchKey, pchValue))
                    {
                        QueueLobbyRefresh(steamIDLobby, (ulong)SteamEmulator.SteamID);
                    }
                }, "lobby:member-data:" + steamIDLobby + ":" + pchKey);
                return;
            }

            if (GetLobby(steamIDLobby, out var lobby))
            {
                var member = lobby.Members.Find(m => m.m_SteamID == (ulong)SteamEmulator.SteamID);
                if (member == null)
                {
                    member = new SteamLobby.LobbyMember
                    {
                        m_SteamID = (ulong)SteamEmulator.SteamID
                    };
                    lobby.Members.Add(member);
                }

                var existing = member.m_Data.Find(m => m.m_Key == pchKey);
                if (existing == null)
                {
                    member.m_Data.Add(new SteamLobby.LobbyMetaData
                    {
                        m_Key = pchKey,
                        m_Value = pchValue
                    });
                }
                else
                {
                    existing.m_Value = pchValue;
                }

                EmitLobbyDataUpdate(steamIDLobby, (ulong)SteamEmulator.SteamID, true);
            }
        }

        public bool SetLobbyMemberLimit(ulong steamIDLobby, int cMaxMembers)
        {
            Write($"SetLobbyMemberLimit (Lobby SteamID: {steamIDLobby}, MaxMembers: {cMaxMembers})");
            if (SkyNetApiClient.IsEnabled)
            {
                if (GetLobby(steamIDLobby, out var cached))
                {
                    cached.MaxMembers = cMaxMembers;
                    EmitLobbyDataUpdate(steamIDLobby, cached.Owner, true);
                }
                WorkQueue.Enqueue("SetLobbyMemberLimit", () =>
                {
                    if (SkyNetApiClient.SetLobbyMemberLimit(steamIDLobby, cMaxMembers))
                    {
                        QueueLobbyRefresh(steamIDLobby, (ulong)SteamEmulator.SteamID);
                    }
                }, "lobby:member-limit:" + steamIDLobby);
                return true;
            }

            if (GetLobby(steamIDLobby, out var lobby))
            {
                lobby.MaxMembers = cMaxMembers;
                EmitLobbyDataUpdate(steamIDLobby, lobby.Owner, true);
                return true;
            }
            return false;
        }

        public bool SetLobbyOwner(ulong steamIDLobby, ulong steamIDNewOwner)
        {
            Write($"SetLobbyOwner (Lobby SteamID: {steamIDLobby},  SteamIDNewOwner: {steamIDNewOwner})");
            if (SkyNetApiClient.IsEnabled)
            {
                if (GetLobby(steamIDLobby, out var cached))
                {
                    cached.Owner = steamIDNewOwner;
                    EmitLobbyDataUpdate(steamIDLobby, steamIDNewOwner, true);
                }
                WorkQueue.Enqueue("SetLobbyOwner", () =>
                {
                    if (SkyNetApiClient.SetLobbyOwner(steamIDLobby, steamIDNewOwner))
                    {
                        QueueLobbyRefresh(steamIDLobby, steamIDNewOwner);
                    }
                }, "lobby:owner:" + steamIDLobby);
                return true;
            }

            if (GetLobby(steamIDLobby, out var lobby))
            {
                lobby.Owner = steamIDNewOwner;
                EmitLobbyDataUpdate(steamIDLobby, steamIDNewOwner, true);
                return true;
            }
            return false;
        }

        public bool SetLobbyType(ulong steamIDLobby, int eLobbyType)
        {
            Write($"SetLobbyType (Lobby SteamID: {steamIDLobby}, LobbyType: {(ELobbyType)eLobbyType})");
            if (SkyNetApiClient.IsEnabled)
            {
                if (GetLobby(steamIDLobby, out var cached))
                {
                    cached.Type = (ELobbyType)eLobbyType;
                    EmitLobbyDataUpdate(steamIDLobby, cached.Owner, true);
                }
                WorkQueue.Enqueue("SetLobbyType", () =>
                {
                    if (SkyNetApiClient.SetLobbyType(steamIDLobby, eLobbyType))
                    {
                        QueueLobbyRefresh(steamIDLobby, (ulong)SteamEmulator.SteamID);
                    }
                }, "lobby:type:" + steamIDLobby);
                return true;
            }

            if (GetLobby(steamIDLobby, out var lobby))
            {
                lobby.Type = (ELobbyType)eLobbyType;
                EmitLobbyDataUpdate(steamIDLobby, lobby.Owner, true);
                return true;
            }
            return false;
        }

        public void CheckForPSNGameBootInvite(int iGameBootAttributes)
        {
            Write("CheckForPSNGameBootInvite");
        }

        public void JoinResponse(NET_LobbyJoinResponse JoinRespons, SteamLobby lobby)
        {
            //try
            //{
            //    if (lobby != null)
            //    {
            //        LobbyEnter_t data = new LobbyEnter_t()
            //        {
            //            m_ulSteamIDLobby = lobby.SteamID,
            //            m_bLocked = false,
            //            m_EChatRoomEnterResponse = JoinRespons.ChatRoomEnterResponse,
            //            m_rgfChatPermissions = 0
            //        };
            //        if (CallbackManager.GetCallResult(JoinRespons.CallbackHandle, out var callback))
            //        {
            //            callback.Data = data;
            //            callback.ReadyToCall = true;
            //        }
            //        else
            //        {
            //            Write($"Not found callback result for handle {JoinRespons.CallbackHandle}");
            //        }
            //    }
            //    else
            //    {
            //        if (CallbackManager.GetCallResult(JoinRespons.CallbackHandle, out var callback))
            //        {
            //            callback.ReadyToCall = true;
            //        }
            //    }
            //}
            //catch (Exception)
            //{

            //}
        }

        public void RemoveLobby(ulong LobbyID)
        {
            if (GetLobby(LobbyID, out var lobby))
            {
                EmitLobbyDataUpdate(LobbyID, lobby.Owner, false);
            }
        }

        public void LobbyDataUpdated(NET_LobbyDataUpdate lobbyDataUpdate)
        {
            EmitLobbyDataUpdate(lobbyDataUpdate.SteamIDLobby, lobbyDataUpdate.SteamIDMember, true);
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

            CallbackManager.AddCallback(data);
        }

        private void SaveFavoriteGames()
        {
            try
            {
                string FavoriteGamesPath = Path.Combine(Common.GetPath(), "SKYNET", "Storage", "FavoriteGames.json");
                Common.EnsureDirectoryExists(FavoriteGamesPath, true);
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
                EmitLobbyDataUpdate(lobby.SteamID, Member, true);
            }
        }

        private void QueueLobbyRefresh(ulong lobbyId, ulong memberId)
        {
            WorkQueue.Enqueue("Lobby refresh", () =>
            {
                var refreshed = SkyNetApiClient.RefreshLobby(lobbyId);
                if (refreshed != null)
                {
                    LobbyManager.UpsertLobby(refreshed);
                    EmitLobbyDataUpdate(lobbyId, memberId, true);
                }
            }, "lobby:refresh:" + lobbyId);
        }

        private void UpsertLocalMemberData(ulong lobbyId, ulong steamId, string key, string value)
        {
            if (!GetLobby(lobbyId, out var lobby))
            {
                return;
            }

            var member = lobby.Members.Find(m => m.m_SteamID == steamId);
            if (member == null)
            {
                member = new SteamLobby.LobbyMember
                {
                    m_SteamID = steamId
                };
                lobby.Members.Add(member);
            }

            var existing = member.m_Data.Find(m => m.m_Key == key);
            if (existing == null)
            {
                member.m_Data.Add(new SteamLobby.LobbyMetaData
                {
                    m_Key = key,
                    m_Value = value
                });
            }
            else
            {
                existing.m_Value = value;
            }
        }

        private static void WriteUInt32(IntPtr destination, uint value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteInt32(destination, unchecked((int)value));
            }
        }

        private static void WriteUInt16(IntPtr destination, uint value)
        {
            if (destination != IntPtr.Zero)
            {
                Marshal.WriteInt16(destination, unchecked((short)value));
            }
        }

        private static FilterLobby CloneFilter(FilterLobby source)
        {
            if (source == null)
            {
                return new FilterLobby();
            }

            return new FilterLobby
            {
                Distance = source.Distance,
                SlotsAvailable = source.SlotsAvailable,
                KeyToMatch = source.KeyToMatch,
                ValueToMatch = source.ValueToMatch,
                ComparisonType = source.ComparisonType,
                StringValueToMatch = source.StringValueToMatch
            };
        }

        private void EmitLobbyDataUpdate(ulong lobbyId, ulong memberId, bool success)
        {
            CallbackManager.AddCallback(new LobbyDataUpdate_t
            {
                m_bSuccess = success,
                m_ulSteamIDLobby = lobbyId,
                m_ulSteamIDMember = memberId
            });
        }

        public bool GetLobby(ulong SteamID, out SteamLobby lobby, bool byOwner = false)
        {
            if (byOwner)
                lobby = LobbyManager.GetLobbyByOwner(SteamID);
            else
                lobby = LobbyManager.GetLobby(SteamID);

            if (lobby == null && SkyNetApiClient.IsEnabled && !byOwner)
            {
                WorkQueue.Enqueue("GetLobby refresh", () =>
                {
                    var refreshed = SkyNetApiClient.RefreshLobby(SteamID);
                    if (refreshed != null)
                    {
                        LobbyManager.UpsertLobby(refreshed);
                    }
                }, "lobby:refresh:" + SteamID);
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
