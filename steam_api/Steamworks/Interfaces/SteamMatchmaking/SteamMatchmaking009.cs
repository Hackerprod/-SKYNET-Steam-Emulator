using System;

namespace SKYNET.Interface
{
    [Interface("SteamMatchMaking009")]
    public class SteamMatchMaking009 : ISteamInterface
    {
        public int GetFavoriteGameCount(IntPtr _)
        {
            return SteamEmulator.SteamMatchmaking.GetFavoriteGameCount();
        }

        public bool GetFavoriteGame(IntPtr _, int iGame, uint pnAppID, uint pnIP, uint pnConnPort, uint pnQueryPort, uint punFlags, uint pRTime32LastPlayedOnServer)
        {
            return SteamEmulator.SteamMatchmaking.GetFavoriteGame(iGame, pnAppID, pnIP, pnConnPort, pnQueryPort, punFlags, pRTime32LastPlayedOnServer);
        }

        public int AddFavoriteGame(IntPtr _, uint nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags, uint rTime32LastPlayedOnServer)
        {
            return SteamEmulator.SteamMatchmaking.AddFavoriteGame(nAppID, nIP, nConnPort, nQueryPort, unFlags, rTime32LastPlayedOnServer);
        }

        public bool RemoveFavoriteGame(IntPtr _, uint nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags)
        {
            return SteamEmulator.SteamMatchmaking.RemoveFavoriteGame(nAppID, nIP, nConnPort, nQueryPort, unFlags);
        }

        public ulong RequestLobbyList(IntPtr _)
        {
            return SteamEmulator.SteamMatchmaking.RequestLobbyList();
        }

        public void AddRequestLobbyListStringFilter(IntPtr _, string pchKeyToMatch, string pchValueToMatch, int eComparisonType)
        {
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListStringFilter(pchKeyToMatch, pchValueToMatch, eComparisonType);
        }

        public void AddRequestLobbyListNumericalFilter(IntPtr _, string pchKeyToMatch, int nValueToMatch, int eComparisonType)
        {
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListNumericalFilter(pchKeyToMatch, nValueToMatch, eComparisonType);
        }

        public void AddRequestLobbyListNearValueFilter(IntPtr _, string pchKeyToMatch, int nValueToBeCloseTo)
        {
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListNearValueFilter(pchKeyToMatch, nValueToBeCloseTo);
        }

        public void AddRequestLobbyListFilterSlotsAvailable(IntPtr _, int nSlotsAvailable)
        {
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(nSlotsAvailable);
        }

        public void AddRequestLobbyListDistanceFilter(IntPtr _, int eLobbyDistanceFilter)
        {
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListDistanceFilter(eLobbyDistanceFilter);
        }

        public void AddRequestLobbyListResultCountFilter(IntPtr _, int cMaxResults)
        {
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListResultCountFilter(cMaxResults);
        }

        public void AddRequestLobbyListCompatibleMembersFilter(IntPtr _, ulong steamIDLobby)
        {
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListCompatibleMembersFilter(steamIDLobby);
        }

        public ulong GetLobbyByIndex(IntPtr _, int iLobby)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyByIndex(iLobby);
        }

        public ulong CreateLobby(IntPtr _, int eLobbyType, int cMaxMembers)
        {
            return SteamEmulator.SteamMatchmaking.CreateLobby(eLobbyType, cMaxMembers);
        }

        public ulong JoinLobby(IntPtr _, ulong steamIDLobby)
        {
            return SteamEmulator.SteamMatchmaking.JoinLobby(steamIDLobby);
        }

        public void LeaveLobby(IntPtr _, ulong steamIDLobby)
        {
            SteamEmulator.SteamMatchmaking.LeaveLobby(steamIDLobby);
        }

        public bool InviteUserToLobby(IntPtr _, ulong steamIDLobby, ulong steamIDInvitee)
        {
            return SteamEmulator.SteamMatchmaking.InviteUserToLobby(steamIDLobby, steamIDInvitee);
        }

        public int GetNumLobbyMembers(IntPtr _, ulong steamIDLobby)
        {
            return SteamEmulator.SteamMatchmaking.GetNumLobbyMembers(steamIDLobby);
        }

        public ulong GetLobbyMemberByIndex(IntPtr _, ulong steamIDLobby, int iMember)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyMemberByIndex(steamIDLobby, iMember);
        }

        public string GetLobbyData(IntPtr _, ulong steamIDLobby, string pchKey)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyData(steamIDLobby, pchKey);
        }

        public bool SetLobbyData(IntPtr _, ulong steamIDLobby, string pchKey, string pchValue)
        {
            return SteamEmulator.SteamMatchmaking.SetLobbyData(steamIDLobby, pchKey, pchValue);
        }

        public int GetLobbyDataCount(IntPtr _, ulong steamIDLobby)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyDataCount(steamIDLobby);
        }

        public bool GetLobbyDataByIndex(IntPtr _, ulong steamIDLobby, int iLobbyData, string pchKey, int cchKeyBufferSize, string pchValue, int cchValueBufferSize)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyDataByIndex(steamIDLobby, iLobbyData, pchKey, cchKeyBufferSize, pchValue, cchValueBufferSize);
        }

        public bool DeleteLobbyData(IntPtr _, ulong steamIDLobby, string pchKey)
        {
            return SteamEmulator.SteamMatchmaking.DeleteLobbyData(steamIDLobby, pchKey);
        }

        public string GetLobbyMemberData(IntPtr _, ulong steamIDLobby, ulong steamIDUser, string pchKey)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyMemberData(steamIDLobby, steamIDUser, pchKey);
        }

        public void SetLobbyMemberData(IntPtr _, ulong steamIDLobby, string pchKey, string pchValue)
        {
            SteamEmulator.SteamMatchmaking.SetLobbyMemberData(steamIDLobby, pchKey, pchValue);
        }

        public bool SendLobbyChatMsg(IntPtr _, ulong steamIDLobby, IntPtr pvMsgBody, int cubMsgBody)
        {
            return SteamEmulator.SteamMatchmaking.SendLobbyChatMsg(steamIDLobby, pvMsgBody, cubMsgBody);
        }

        public int GetLobbyChatEntry(ulong steamIDLobby, int iChatID, ulong pSteamIDUser, IntPtr pvData, int cubData, int peChatEntryType)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyChatEntry(steamIDLobby, iChatID, pSteamIDUser, pvData, cubData, peChatEntryType);
        }

        public bool RequestLobbyData(IntPtr _, ulong steamIDLobby)
        {
            return SteamEmulator.SteamMatchmaking.RequestLobbyData(steamIDLobby);
        }

        public void SetLobbyGameServer(IntPtr _, ulong steamIDLobby, uint unGameServerIP, uint unGameServerPort, ulong steamIDGameServer)
        {
            SteamEmulator.SteamMatchmaking.SetLobbyGameServer(steamIDLobby, unGameServerIP, unGameServerPort, steamIDGameServer);
        }

        public bool GetLobbyGameServer(ulong steamIDLobby, uint punGameServerIP, uint punGameServerPort, ulong psteamIDGameServer)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyGameServer(steamIDLobby, punGameServerIP, punGameServerPort, psteamIDGameServer);
        }

        public bool SetLobbyMemberLimit(IntPtr _, ulong steamIDLobby, int cMaxMembers)
        {
            return SteamEmulator.SteamMatchmaking.SetLobbyMemberLimit(steamIDLobby, cMaxMembers);
        }

        public int GetLobbyMemberLimit(IntPtr _, ulong steamIDLobby)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyMemberLimit(steamIDLobby);
        }

        public bool SetLobbyType(IntPtr _, ulong steamIDLobby, int eLobbyType)
        {
            return SteamEmulator.SteamMatchmaking.SetLobbyType(steamIDLobby, eLobbyType);
        }

        public bool SetLobbyJoinable(IntPtr _, ulong steamIDLobby, bool bLobbyJoinable)
        {
            return SteamEmulator.SteamMatchmaking.SetLobbyJoinable(steamIDLobby, bLobbyJoinable);
        }

        public ulong GetLobbyOwner(IntPtr _, ulong steamIDLobby)
        {
            return SteamEmulator.SteamMatchmaking.GetLobbyOwner(steamIDLobby);
        }

        public bool SetLobbyOwner(IntPtr _, ulong steamIDLobby, ulong steamIDNewOwner)
        {
            return SteamEmulator.SteamMatchmaking.SetLobbyOwner(steamIDLobby, steamIDNewOwner);
        }

        public bool SetLinkedLobby(IntPtr _, ulong steamIDLobby, ulong steamIDLobbyDependent)
        {
            return SteamEmulator.SteamMatchmaking.SetLinkedLobby(steamIDLobby, steamIDLobbyDependent);
        }

        public void CheckForPSNGameBootInvite(IntPtr _, int iGameBootAttributes)
        {
            SteamEmulator.SteamMatchmaking.CheckForPSNGameBootInvite(iGameBootAttributes);
        }


    }
}
