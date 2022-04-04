using System;
using SKYNET;
using SKYNET.Helper;
using SKYNET.Steamworks;
using Steamworks;

public class SteamMatchmaking : ISteamInterface
{
    public IntPtr MemoryAddress { get; set; }
    public string InterfaceVersion { get; set; }

    public int AddFavoriteGame(AppId_t nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags, uint rTime32LastPlayedOnServer)
    {
        Write("AddFavoriteGame");
        return 1;
    }

    public void AddRequestLobbyListCompatibleMembersFilter(IntPtr steamIDLobby)
    {
        Write("AddRequestLobbyListCompatibleMembersFilter");
    }

    public void AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter eLobbyDistanceFilter)
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

    public void AddRequestLobbyListNumericalFilter(string pchKeyToMatch, int nValueToMatch, ELobbyComparison eComparisonType)
    {
        Write("AddRequestLobbyListNumericalFilter");
    }

    public void AddRequestLobbyListResultCountFilter(int cMaxResults)
    {
        Write("AddRequestLobbyListResultCountFilter");
    }

    public void AddRequestLobbyListStringFilter(string pchKeyToMatch, string pchValueToMatch, ELobbyComparison eComparisonType)
    {
        Write("AddRequestLobbyListStringFilter");
    }

    public SteamAPICall_t CreateLobby(ELobbyType eLobbyType, int cMaxMembers)
    {
        Write("CreateLobby");
        return new SteamAPICall_t();
    }

    public bool DeleteLobbyData(IntPtr steamIDLobby, string pchKey)
    {
        Write("DeleteLobbyData");
        return true;
    }

    public bool GetFavoriteGame(int iGame, AppId_t pnAppID, uint pnIP, uint pnConnPort, uint pnQueryPort, uint punFlags, uint pRTime32LastPlayedOnServer)
    {
        Write("GetFavoriteGame");
        return true;
    }

    public int GetFavoriteGameCount(IntPtr _)
    {
        Write("GetFavoriteGameCount");
        return 1;
    }

    public IntPtr GetLobbyByIndex(int iLobby)
    {
        Write("GetLobbyByIndex");
        return IntPtr.Zero;
    }

    public int GetLobbyChatEntry(IntPtr steamIDLobby, int iChatID, IntPtr pSteamIDUser, IntPtr pvData, int cubData, EChatEntryType peChatEntryType)
    {
        Write("GetLobbyChatEntry");
        return 1;
    }

    public string GetLobbyData(IntPtr steamIDLobby, string pchKey)
    {
        Write("GetLobbyData");
        return "";
    }

    public bool GetLobbyDataByIndex(IntPtr steamIDLobby, int iLobbyData, string pchKey, int cchKeyBufferSize, string pchValue, int cchValueBufferSize)
    {
        Write("GetLobbyDataByIndex");
        return true;
    }

    public int GetLobbyDataCount(IntPtr steamIDLobby)
    {
        Write("GetLobbyDataCount");
        return 1;
    }

    public bool GetLobbyGameServer(IntPtr steamIDLobby, uint punGameServerIP, uint punGameServerPort, IntPtr psteamIDGameServer)
    {
        Write("GetLobbyGameServer");
        return true;
    }

    public IntPtr GetLobbyMemberByIndex(IntPtr steamIDLobby, int iMember)
    {
        Write("GetLobbyMemberByIndex");
        return IntPtr.Zero;
    }

    public string GetLobbyMemberData(IntPtr steamIDLobby, IntPtr steamIDUser, string pchKey)
    {
        Write("GetLobbyMemberData");
        return "";
    }

    public int GetLobbyMemberLimit(IntPtr steamIDLobby)
    {
        Write("GetLobbyMemberLimit");
        return 1;
    }

    public IntPtr GetLobbyOwner(IntPtr steamIDLobby)
    {
        Write("GetLobbyOwner");
        return IntPtr.Zero;
    }

    public int GetNumLobbyMembers(IntPtr steamIDLobby)
    {
        Write("GetNumLobbyMembers");
        return 1;
    }

    public bool InviteUserToLobby(IntPtr steamIDLobby, IntPtr steamIDInvitee)
    {
        Write("InviteUserToLobby");
        return true;
    }

    public SteamAPICall_t JoinLobby(IntPtr steamIDLobby)
    {
        Write("JoinLobby");
        return new SteamAPICall_t();
    }

    public void LeaveLobby(IntPtr steamIDLobby)
    {
        Write("LeaveLobby");
    }

    public bool RemoveFavoriteGame(AppId_t nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags)
    {
        Write("RemoveFavoriteGame");
        return true;
    }

    public bool RequestLobbyData(IntPtr steamIDLobby)
    {
        Write("RequestLobbyData");
        return true;
    }

    public SteamAPICall_t RequestLobbyList(IntPtr _)
    {
        Write("RequestLobbyList");
        return new SteamAPICall_t();
    }

    public bool SendLobbyChatMsg(IntPtr steamIDLobby, IntPtr pvMsgBody, int cubMsgBody)
    {
        Write("SendLobbyChatMsg");
        return true;
    }

    public bool SetLinkedLobby(IntPtr steamIDLobby, IntPtr steamIDLobbyDependent)
    {
        Write("SetLinkedLobby");
        return true;
    }

    public bool SetLobbyData(IntPtr steamIDLobby, string pchKey, string pchValue)
    {
        Write("SetLobbyData");
        return true;
    }

    public void SetLobbyGameServer(IntPtr steamIDLobby, uint unGameServerIP, uint unGameServerPort, IntPtr steamIDGameServer)
    {
        Write("SetLobbyGameServer");
    }

    public bool SetLobbyJoinable(IntPtr steamIDLobby, bool bLobbyJoinable)
    {
        Write("SetLobbyJoinable");
        return true;
    }

    public void SetLobbyMemberData(IntPtr steamIDLobby, string pchKey, string pchValue)
    {
        Write("SetLobbyMemberData");
    }

    public bool SetLobbyMemberLimit(IntPtr steamIDLobby, int cMaxMembers)
    {
        Write("SetLobbyMemberLimit");
        return true;
    }

    public bool SetLobbyOwner(IntPtr steamIDLobby, IntPtr steamIDNewOwner)
    {
        Write("SetLobbyOwner");
        return true;
    }

    public bool SetLobbyType(IntPtr steamIDLobby, ELobbyType eLobbyType)
    {
        Write("SetLobbyType");
        return true;
    }

    private void Write(string v)
    {
        SteamEmulator.Write(InterfaceVersion, v);
    }
}