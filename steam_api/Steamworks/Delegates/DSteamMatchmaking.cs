
using SKYNET.Steamworks;
using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamMatchmaking")]
    public class DSteamMatchmaking
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFavoriteGameCount(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetFavoriteGame(int iGame, AppId_t pnAppID, uint pnIP, uint pnConnPort, uint pnQueryPort, uint punFlags, uint pRTime32LastPlayedOnServer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int AddFavoriteGame(AppId_t nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags, uint rTime32LastPlayedOnServer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RemoveFavoriteGame(AppId_t nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RequestLobbyList(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void AddRequestLobbyListStringFilter(string pchKeyToMatch, string pchValueToMatch, ELobbyComparison eComparisonType);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void AddRequestLobbyListNumericalFilter(string pchKeyToMatch, int nValueToMatch, ELobbyComparison eComparisonType);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void AddRequestLobbyListNearValueFilter(string pchKeyToMatch, int nValueToBeCloseTo);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void AddRequestLobbyListFilterSlotsAvailable(int nSlotsAvailable);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter eLobbyDistanceFilter);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void AddRequestLobbyListResultCountFilter(int cMaxResults);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void AddRequestLobbyListCompatibleMembersFilter(IntPtr steamIDLobby);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetLobbyByIndex(int iLobby);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t CreateLobby(ELobbyType eLobbyType, int cMaxMembers);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t JoinLobby(IntPtr steamIDLobby);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void LeaveLobby(IntPtr steamIDLobby);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool InviteUserToLobby(IntPtr steamIDLobby, IntPtr steamIDInvitee);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetNumLobbyMembers(IntPtr steamIDLobby);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetLobbyMemberByIndex(IntPtr steamIDLobby, int iMember);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetLobbyData(IntPtr steamIDLobby, string pchKey);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetLobbyData(IntPtr steamIDLobby, string pchKey, string pchValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetLobbyDataCount(IntPtr steamIDLobby);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetLobbyDataByIndex(IntPtr steamIDLobby, int iLobbyData, string pchKey, int cchKeyBufferSize, string pchValue, int cchValueBufferSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool DeleteLobbyData(IntPtr steamIDLobby, string pchKey);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetLobbyMemberData(IntPtr steamIDLobby, IntPtr steamIDUser, string pchKey);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetLobbyMemberData(IntPtr steamIDLobby, string pchKey, string pchValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SendLobbyChatMsg(IntPtr steamIDLobby, IntPtr pvMsgBody, int cubMsgBody);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetLobbyChatEntry(IntPtr steamIDLobby, int iChatID, IntPtr pSteamIDUser, IntPtr pvData, int cubData, EChatEntryType peChatEntryType);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RequestLobbyData(IntPtr steamIDLobby);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetLobbyGameServer(IntPtr steamIDLobby, uint unGameServerIP, uint unGameServerPort, IntPtr steamIDGameServer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetLobbyGameServer(IntPtr steamIDLobby, uint punGameServerIP, uint punGameServerPort, IntPtr psteamIDGameServer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetLobbyMemberLimit(IntPtr steamIDLobby, int cMaxMembers);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetLobbyMemberLimit(IntPtr steamIDLobby);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetLobbyType(IntPtr steamIDLobby, ELobbyType eLobbyType);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetLobbyJoinable(IntPtr steamIDLobby, bool bLobbyJoinable);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetLobbyOwner(IntPtr steamIDLobby);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetLobbyOwner(IntPtr steamIDLobby, IntPtr steamIDNewOwner);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetLinkedLobby(IntPtr steamIDLobby, IntPtr steamIDLobbyDependent);
    }
}