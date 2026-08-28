using System;
using SKYNET.Helpers;

namespace SKYNET.Steamworks.Interfaces
{
    /// <summary>
    /// Legacy ISteamMatchmaking ABI used by Left 4 Dead. Version 007 predates
    /// the distance and result-count lobby filters introduced in version 008.
    /// </summary>
    [Interface("SteamMatchMaking007")]
    public class SteamMatchMaking007 : ISteamInterface
    {
        public int GetFavoriteGameCount(IntPtr _) => SteamEmulator.SteamMatchmaking.GetFavoriteGameCount();

        public bool GetFavoriteGame(IntPtr _, int iGame, IntPtr pnAppID, IntPtr pnIP, IntPtr pnConnPort, IntPtr pnQueryPort, IntPtr punFlags, IntPtr pRTime32LastPlayedOnServer) =>
            SteamEmulator.SteamMatchmaking.GetFavoriteGame(iGame, pnAppID, pnIP, pnConnPort, pnQueryPort, punFlags, pRTime32LastPlayedOnServer);

        public int AddFavoriteGame(IntPtr _, uint nAppID, uint nIP, ushort nConnPort, ushort nQueryPort, uint unFlags, uint rTime32LastPlayedOnServer) =>
            SteamEmulator.SteamMatchmaking.AddFavoriteGame(nAppID, nIP, nConnPort, nQueryPort, unFlags, rTime32LastPlayedOnServer);

        public bool RemoveFavoriteGame(IntPtr _, uint nAppID, uint nIP, ushort nConnPort, ushort nQueryPort, uint unFlags) =>
            SteamEmulator.SteamMatchmaking.RemoveFavoriteGame(nAppID, nIP, nConnPort, nQueryPort, unFlags);

        public ulong RequestLobbyList(IntPtr _) => SteamEmulator.SteamMatchmaking.RequestLobbyList();

        public void AddRequestLobbyListStringFilter(IntPtr _, string pchKeyToMatch, string pchValueToMatch, int eComparisonType) =>
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListStringFilter(pchKeyToMatch, pchValueToMatch, eComparisonType);

        public void AddRequestLobbyListNumericalFilter(IntPtr _, string pchKeyToMatch, int nValueToMatch, int eComparisonType) =>
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListNumericalFilter(pchKeyToMatch, nValueToMatch, eComparisonType);

        public void AddRequestLobbyListNearValueFilter(IntPtr _, string pchKeyToMatch, int nValueToBeCloseTo) =>
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListNearValueFilter(pchKeyToMatch, nValueToBeCloseTo);

        public void AddRequestLobbyListFilterSlotsAvailable(IntPtr _, int nSlotsAvailable) =>
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(nSlotsAvailable);

        public IntPtr GetLobbyByIndex(IntPtr _, IntPtr ret, int iLobby) =>
            NativeSteamId.Write(ret, SteamEmulator.SteamMatchmaking.GetLobbyByIndex(iLobby));

        public ulong CreateLobby(IntPtr _, int eLobbyType, int cMaxMembers) =>
            SteamEmulator.SteamMatchmaking.CreateLobby(eLobbyType, cMaxMembers);

        public ulong JoinLobby(IntPtr _, ulong steamIDLobby) =>
            SteamEmulator.SteamMatchmaking.JoinLobby(steamIDLobby);

        public void LeaveLobby(IntPtr _, ulong steamIDLobby) =>
            SteamEmulator.SteamMatchmaking.LeaveLobby(steamIDLobby);

        public bool InviteUserToLobby(IntPtr _, ulong steamIDLobby, ulong steamIDInvitee) =>
            SteamEmulator.SteamMatchmaking.InviteUserToLobby(steamIDLobby, steamIDInvitee);

        public int GetNumLobbyMembers(IntPtr _, ulong steamIDLobby) =>
            SteamEmulator.SteamMatchmaking.GetNumLobbyMembers(steamIDLobby);

        public IntPtr GetLobbyMemberByIndex(IntPtr _, IntPtr ret, ulong steamIDLobby, int iMember) =>
            NativeSteamId.Write(ret, SteamEmulator.SteamMatchmaking.GetLobbyMemberByIndex(steamIDLobby, iMember));

        public IntPtr GetLobbyData(IntPtr _, ulong steamIDLobby, string pchKey) =>
            NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamMatchmaking.GetLobbyData(steamIDLobby, pchKey));

        public bool SetLobbyData(IntPtr _, ulong steamIDLobby, string pchKey, string pchValue) =>
            SteamEmulator.SteamMatchmaking.SetLobbyData(steamIDLobby, pchKey, pchValue);

        public int GetLobbyDataCount(IntPtr _, ulong steamIDLobby) =>
            SteamEmulator.SteamMatchmaking.GetLobbyDataCount(steamIDLobby);

        public bool GetLobbyDataByIndex(IntPtr _, ulong steamIDLobby, int iLobbyData, IntPtr pchKey, int cchKeyBufferSize, IntPtr pchValue, int cchValueBufferSize) =>
            SteamEmulator.SteamMatchmaking.GetLobbyDataByIndex(steamIDLobby, iLobbyData, pchKey, cchKeyBufferSize, pchValue, cchValueBufferSize);

        public bool DeleteLobbyData(IntPtr _, ulong steamIDLobby, string pchKey) =>
            SteamEmulator.SteamMatchmaking.DeleteLobbyData(steamIDLobby, pchKey);

        public IntPtr GetLobbyMemberData(IntPtr _, ulong steamIDLobby, ulong steamIDUser, string pchKey) =>
            NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamMatchmaking.GetLobbyMemberData(steamIDLobby, steamIDUser, pchKey));

        public void SetLobbyMemberData(IntPtr _, ulong steamIDLobby, string pchKey, string pchValue) =>
            SteamEmulator.SteamMatchmaking.SetLobbyMemberData(steamIDLobby, pchKey, pchValue);

        public bool SendLobbyChatMsg(IntPtr _, ulong steamIDLobby, IntPtr pvMsgBody, int cubMsgBody) =>
            SteamEmulator.SteamMatchmaking.SendLobbyChatMsg(steamIDLobby, pvMsgBody, cubMsgBody);

        public int GetLobbyChatEntry(IntPtr _, ulong steamIDLobby, int iChatID, IntPtr pSteamIDUser, IntPtr pvData, int cubData, IntPtr peChatEntryType) =>
            SteamEmulator.SteamMatchmaking.GetLobbyChatEntry(steamIDLobby, iChatID, pSteamIDUser, pvData, cubData, peChatEntryType);

        public bool RequestLobbyData(IntPtr _, ulong steamIDLobby) =>
            SteamEmulator.SteamMatchmaking.RequestLobbyData(steamIDLobby);

        public void SetLobbyGameServer(IntPtr _, ulong steamIDLobby, uint unGameServerIP, ushort unGameServerPort, ulong steamIDGameServer) =>
            SteamEmulator.SteamMatchmaking.SetLobbyGameServer(steamIDLobby, unGameServerIP, unGameServerPort, steamIDGameServer);

        public bool GetLobbyGameServer(IntPtr _, ulong steamIDLobby, IntPtr punGameServerIP, IntPtr punGameServerPort, IntPtr psteamIDGameServer) =>
            SteamEmulator.SteamMatchmaking.GetLobbyGameServer(steamIDLobby, punGameServerIP, punGameServerPort, psteamIDGameServer);

        public bool SetLobbyMemberLimit(IntPtr _, ulong steamIDLobby, int cMaxMembers) =>
            SteamEmulator.SteamMatchmaking.SetLobbyMemberLimit(steamIDLobby, cMaxMembers);

        public int GetLobbyMemberLimit(IntPtr _, ulong steamIDLobby) =>
            SteamEmulator.SteamMatchmaking.GetLobbyMemberLimit(steamIDLobby);

        public bool SetLobbyType(IntPtr _, ulong steamIDLobby, int eLobbyType) =>
            SteamEmulator.SteamMatchmaking.SetLobbyType(steamIDLobby, eLobbyType);

        public bool SetLobbyJoinable(IntPtr _, ulong steamIDLobby, bool bLobbyJoinable) =>
            SteamEmulator.SteamMatchmaking.SetLobbyJoinable(steamIDLobby, bLobbyJoinable);

        public IntPtr GetLobbyOwner(IntPtr _, IntPtr ret, ulong steamIDLobby) =>
            NativeSteamId.Write(ret, SteamEmulator.SteamMatchmaking.GetLobbyOwner(steamIDLobby));

        public bool SetLobbyOwner(IntPtr _, ulong steamIDLobby, ulong steamIDNewOwner) =>
            SteamEmulator.SteamMatchmaking.SetLobbyOwner(steamIDLobby, steamIDNewOwner);
    }
}
