using System;
using SKYNET.Interface;
using Steamworks;

namespace SKYNET.Managers
{
    [Map("SteamMatchmaking")]
    [Map("SteamMatchmaking006")]
    [Map("SteamMatchmaking007")]
    [Map("SteamMatchmaking008")]
    public class SteamMatchmaking : IBaseInterface, ISteamMatchmaking
    {
        public int AddFavoriteGame(AppId_t nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags, uint rTime32LastPlayedOnServer)
        {
            return 1;
        }

        public void AddRequestLobbyListCompatibleMembersFilter(IntPtr steamIDLobby)
        {
            //
        }

        public void AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter eLobbyDistanceFilter)
        {
            //
        }

        public void AddRequestLobbyListFilterSlotsAvailable(int nSlotsAvailable)
        {
            //
        }

        public void AddRequestLobbyListNearValueFilter(string pchKeyToMatch, int nValueToBeCloseTo)
        {
            //
        }

        public void AddRequestLobbyListNumericalFilter(string pchKeyToMatch, int nValueToMatch, ELobbyComparison eComparisonType)
        {
            //
        }

        public void AddRequestLobbyListResultCountFilter(int cMaxResults)
        {
            //
        }

        public void AddRequestLobbyListStringFilter(string pchKeyToMatch, string pchValueToMatch, ELobbyComparison eComparisonType)
        {
            //
        }

        public SteamAPICall_t CreateLobby(ELobbyType eLobbyType, int cMaxMembers)
        {
            return new SteamAPICall_t();
        }

        public bool DeleteLobbyData(IntPtr steamIDLobby, string pchKey)
        {
            return true;
        }

        public bool GetFavoriteGame(int iGame, AppId_t pnAppID, uint pnIP, uint pnConnPort, uint pnQueryPort, uint punFlags, uint pRTime32LastPlayedOnServer)
        {
            return true;
        }

        public int GetFavoriteGameCount()
        {
            return 1;
        }

        public IntPtr GetLobbyByIndex(int iLobby)
        {
            return IntPtr.Zero;
        }

        public int GetLobbyChatEntry(IntPtr steamIDLobby, int iChatID, IntPtr pSteamIDUser, IntPtr pvData, int cubData, EChatEntryType peChatEntryType)
        {
            return 1;
        }

        public string GetLobbyData(IntPtr steamIDLobby, string pchKey)
        {
            return "";
        }

        public bool GetLobbyDataByIndex(IntPtr steamIDLobby, int iLobbyData, string pchKey, int cchKeyBufferSize, string pchValue, int cchValueBufferSize)
        {
            return true;
        }

        public int GetLobbyDataCount(IntPtr steamIDLobby)
        {
            return 1;
        }

        public bool GetLobbyGameServer(IntPtr steamIDLobby, uint punGameServerIP, uint punGameServerPort, IntPtr psteamIDGameServer)
        {
            return true;
        }

        public IntPtr GetLobbyMemberByIndex(IntPtr steamIDLobby, int iMember)
        {
            return IntPtr.Zero;
        }

        public string GetLobbyMemberData(IntPtr steamIDLobby, IntPtr steamIDUser, string pchKey)
        {
            return "";
        }

        public int GetLobbyMemberLimit(IntPtr steamIDLobby)
        {
            return 1;
        }

        public IntPtr GetLobbyOwner(IntPtr steamIDLobby)
        {
            return IntPtr.Zero;
        }

        public int GetNumLobbyMembers(IntPtr steamIDLobby)
        {
            return 1;
        }

        public bool InviteUserToLobby(IntPtr steamIDLobby, IntPtr steamIDInvitee)
        {
            return true;
        }

        public SteamAPICall_t JoinLobby(IntPtr steamIDLobby)
        {
            return new SteamAPICall_t();
        }

        public void LeaveLobby(IntPtr steamIDLobby)
        {
            //
        }

        public bool RemoveFavoriteGame(AppId_t nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags)
        {
            return true;
        }

        public bool RequestLobbyData(IntPtr steamIDLobby)
        {
            return true;
        }

        public SteamAPICall_t RequestLobbyList()
        {
            return new SteamAPICall_t();
        }

        public bool SendLobbyChatMsg(IntPtr steamIDLobby, IntPtr pvMsgBody, int cubMsgBody)
        {
            return true;
        }

        public bool SetLinkedLobby(IntPtr steamIDLobby, IntPtr steamIDLobbyDependent)
        {
            return true;
        }

        public bool SetLobbyData(IntPtr steamIDLobby, string pchKey, string pchValue)
        {
            return true;
        }

        public void SetLobbyGameServer(IntPtr steamIDLobby, uint unGameServerIP, uint unGameServerPort, IntPtr steamIDGameServer)
        {
            //
        }

        public bool SetLobbyJoinable(IntPtr steamIDLobby, bool bLobbyJoinable)
        {
            return true;
        }

        public void SetLobbyMemberData(IntPtr steamIDLobby, string pchKey, string pchValue)
        {
            //
        }

        public bool SetLobbyMemberLimit(IntPtr steamIDLobby, int cMaxMembers)
        {
            return true;
        }

        public bool SetLobbyOwner(IntPtr steamIDLobby, IntPtr steamIDNewOwner)
        {
            return true;
        }

        public bool SetLobbyType(IntPtr steamIDLobby, ELobbyType eLobbyType)
        {
            return true;
        }
    }
}