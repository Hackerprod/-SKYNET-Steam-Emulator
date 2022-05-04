using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Steamworks;
using Steamworks;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamMatchmaking : ISteamInterface
    {
        public SteamMatchmaking()
        {
            InterfaceName = "SteamMatchmaking";
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
        }

        public void AddRequestLobbyListFilterSlotsAvailable(int nSlotsAvailable)
        {
            Write("AddRequestLobbyListFilterSlotsAvailable");
        }

        public void AddRequestLobbyListNearValueFilter(string pchKeyToMatch, int nValueToBeCloseTo)
        {
            Write("AddRequestLobbyListNearValueFilter");
        }

        public void AddRequestLobbyListNumericalFilter(string pchKeyToMatch, int nValueToMatch, int eComparisonType)
        {
            Write("AddRequestLobbyListNumericalFilter");
        }

        public void AddRequestLobbyListResultCountFilter(int cMaxResults)
        {
            Write("AddRequestLobbyListResultCountFilter");
        }

        public void AddRequestLobbyListStringFilter(string pchKeyToMatch, string pchValueToMatch, int eComparisonType)
        {
            Write("AddRequestLobbyListStringFilter");
        }

        public SteamAPICall_t CreateLobby(int eLobbyType, int cMaxMembers)
        {
            Write("CreateLobby");
            return new ulong();
        }

        public bool DeleteLobbyData(ulong steamIDLobby, string pchKey)
        {
            Write("DeleteLobbyData");
            return true;
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
            return "";
        }

        public bool GetLobbyDataByIndex(ulong steamIDLobby, int iLobbyData, string pchKey, int cchKeyBufferSize, string pchValue, int cchValueBufferSize)
        {
            Write("GetLobbyDataByIndex");
            return true;
        }

        public int GetLobbyDataCount(ulong steamIDLobby)
        {
            Write("GetLobbyDataCount");
            return 1;
        }

        public bool GetLobbyGameServer(ulong steamIDLobby, uint punGameServerIP, uint punGameServerPort, ulong psteamIDGameServer)
        {
            Write("GetLobbyGameServer");
            return true;
        }

        public ulong GetLobbyMemberByIndex(ulong steamIDLobby, int iMember)
        {
            Write("GetLobbyMemberByIndex");
            return 0;
        }

        public string GetLobbyMemberData(ulong steamIDLobby, ulong steamIDUser, string pchKey)
        {
            Write("GetLobbyMemberData");
            return "";
        }

        public int GetLobbyMemberLimit(ulong steamIDLobby)
        {
            Write("GetLobbyMemberLimit");
            return 1;
        }

        public CSteamID GetLobbyOwner(ulong steamIDLobby)
        {
            Write("GetLobbyOwner");
            return (CSteamID)0;
        }

        public int GetNumLobbyMembers(ulong steamIDLobby)
        {
            Write("GetNumLobbyMembers");
            return 1;
        }

        public bool InviteUserToLobby(ulong steamIDLobby, ulong steamIDInvitee)
        {
            Write("InviteUserToLobby");
            return true;
        }

        public SteamAPICall_t JoinLobby(ulong steamIDLobby)
        {
            Write("JoinLobby");
            return new ulong();
        }

        public void LeaveLobby(ulong steamIDLobby)
        {
            Write("LeaveLobby");
        }

        public bool RemoveFavoriteGame(uint nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags)
        {
            Write("RemoveFavoriteGame");
            return true;
        }

        public bool RequestLobbyData(ulong steamIDLobby)
        {
            Write("RequestLobbyData");
            return true;
        }

        public SteamAPICall_t RequestLobbyList()
        {
            Write("RequestLobbyList");
            return new ulong();
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
            Write("SetLobbyData");
            return true;
        }

        public void SetLobbyGameServer(ulong steamIDLobby, uint unGameServerIP, uint unGameServerPort, ulong steamIDGameServer)
        {
            Write("SetLobbyGameServer");
        }

        public bool SetLobbyJoinable(ulong steamIDLobby, bool bLobbyJoinable)
        {
            Write("SetLobbyJoinable");
            return true;
        }

        public void SetLobbyMemberData(ulong steamIDLobby, string pchKey, string pchValue)
        {
            Write("SetLobbyMemberData");
        }

        public bool SetLobbyMemberLimit(ulong steamIDLobby, int cMaxMembers)
        {
            Write("SetLobbyMemberLimit");
            return true;
        }

        public bool SetLobbyOwner(ulong steamIDLobby, ulong steamIDNewOwner)
        {
            Write("SetLobbyOwner");
            return true;
        }

        public bool SetLobbyType(ulong steamIDLobby, int eLobbyType)
        {
            Write("SetLobbyType");
            return true;
        }

        public void CheckForPSNGameBootInvite(int iGameBootAttributes)
        {
            Write("CheckForPSNGameBootInvite");
        }
    }
}