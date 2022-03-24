using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SKYNET;
using Steamworks;

//public class SteamAPI_ISteamMatchmaking : BaseCalls
//{
//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static int SteamAPI_ISteamMatchmaking_GetFavoriteGameCount(IntPtr self)
//    {
//        return SteamEmulator.SteamMatchmaking.GetFavoriteGameCount();
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamMatchmaking_GetFavoriteGame(IntPtr self, int iGame, AppId_t pnAppID, uint pnIP, uint pnConnPort, uint pnQueryPort, uint punFlags, uint pRTime32LastPlayedOnServer)
//    {
//        return SteamEmulator.SteamMatchmaking.GetFavoriteGame(iGame, pnAppID, pnIP, pnConnPort, pnQueryPort, punFlags, pRTime32LastPlayedOnServer);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static int SteamAPI_ISteamMatchmaking_AddFavoriteGame(IntPtr self, AppId_t nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags, uint rTime32LastPlayedOnServer)
//    {
//        return SteamEmulator.SteamMatchmaking.AddFavoriteGame(nAppID, nIP, nConnPort, nQueryPort, unFlags, rTime32LastPlayedOnServer);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamMatchmaking_RemoveFavoriteGame(IntPtr self, AppId_t nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags)
//    {
//        return SteamEmulator.SteamMatchmaking.RemoveFavoriteGame(nAppID, nIP, nConnPort, nQueryPort, unFlags);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static SteamAPICall_t SteamAPI_ISteamMatchmaking_RequestLobbyList(IntPtr self)
//    {
//        return SteamEmulator.SteamMatchmaking.RequestLobbyList();
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static void SteamAPI_ISteamMatchmaking_AddRequestLobbyListStringFilter(IntPtr self, string pchKeyToMatch, string pchValueToMatch, ELobbyComparison eComparisonType)
//    {
//        SteamEmulator.SteamMatchmaking.AddRequestLobbyListStringFilter(pchKeyToMatch, pchValueToMatch, eComparisonType);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static void SteamAPI_ISteamMatchmaking_AddRequestLobbyListNumericalFilter(IntPtr self, string pchKeyToMatch, int nValueToMatch, ELobbyComparison eComparisonType)
//    {
//        SteamEmulator.SteamMatchmaking.AddRequestLobbyListNumericalFilter(pchKeyToMatch, nValueToMatch, eComparisonType);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static void SteamAPI_ISteamMatchmaking_AddRequestLobbyListNearValueFilter(IntPtr self, string pchKeyToMatch, int nValueToBeCloseTo)
//    {
//        SteamEmulator.SteamMatchmaking.AddRequestLobbyListNearValueFilter(pchKeyToMatch, nValueToBeCloseTo);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static void SteamAPI_ISteamMatchmaking_AddRequestLobbyListFilterSlotsAvailable(IntPtr self, int nSlotsAvailable)
//    {
//        SteamEmulator.SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(nSlotsAvailable);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static void SteamAPI_ISteamMatchmaking_AddRequestLobbyListDistanceFilter(IntPtr self, ELobbyDistanceFilter eLobbyDistanceFilter)
//    {
//        SteamEmulator.SteamMatchmaking.AddRequestLobbyListDistanceFilter(eLobbyDistanceFilter);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static void SteamAPI_ISteamMatchmaking_AddRequestLobbyListResultCountFilter(IntPtr self, int cMaxResults)
//    {
//        SteamEmulator.SteamMatchmaking.AddRequestLobbyListResultCountFilter(cMaxResults);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static void SteamAPI_ISteamMatchmaking_AddRequestLobbyListCompatibleMembersFilter(IntPtr self, IntPtr steamIDLobby)
//    {
//        SteamEmulator.SteamMatchmaking.AddRequestLobbyListCompatibleMembersFilter(steamIDLobby);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static IntPtr SteamAPI_ISteamMatchmaking_GetLobbyByIndex(IntPtr self, int iLobby)
//    {
//        return SteamEmulator.SteamMatchmaking.GetLobbyByIndex(iLobby);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static SteamAPICall_t SteamAPI_ISteamMatchmaking_CreateLobby(IntPtr self, ELobbyType eLobbyType, int cMaxMembers)
//    {
//        return SteamEmulator.SteamMatchmaking.CreateLobby(eLobbyType, cMaxMembers);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static SteamAPICall_t SteamAPI_ISteamMatchmaking_JoinLobby(IntPtr self, IntPtr steamIDLobby)
//    {
//        return SteamEmulator.SteamMatchmaking.JoinLobby(steamIDLobby);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static void SteamAPI_ISteamMatchmaking_LeaveLobby(IntPtr self, IntPtr steamIDLobby)
//    {
//        SteamEmulator.SteamMatchmaking.LeaveLobby(steamIDLobby);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamMatchmaking_InviteUserToLobby(IntPtr self, IntPtr steamIDLobby, IntPtr steamIDInvitee)
//    {
//        return SteamEmulator.SteamMatchmaking.InviteUserToLobby(steamIDLobby, steamIDInvitee);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static int SteamAPI_ISteamMatchmaking_GetNumLobbyMembers(IntPtr self, IntPtr steamIDLobby)
//    {
//        return SteamEmulator.SteamMatchmaking.GetNumLobbyMembers(steamIDLobby);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static IntPtr SteamAPI_ISteamMatchmaking_GetLobbyMemberByIndex(IntPtr self, IntPtr steamIDLobby, int iMember)
//    {
//        return SteamEmulator.SteamMatchmaking.GetLobbyMemberByIndex(steamIDLobby, iMember);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static string SteamAPI_ISteamMatchmaking_GetLobbyData(ISteamMatchmaking self, IntPtr steamIDLobby, string pchKey)
//    {
//        return SteamEmulator.SteamMatchmaking.GetLobbyData(steamIDLobby, pchKey);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamMatchmaking_SetLobbyData(IntPtr self, IntPtr steamIDLobby, string pchKey, string pchValue)
//    {
//        return SteamEmulator.SteamMatchmaking.SetLobbyData(steamIDLobby, pchKey, pchValue);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static int SteamAPI_ISteamMatchmaking_GetLobbyDataCount(IntPtr self, IntPtr steamIDLobby)
//    {
//        return SteamEmulator.SteamMatchmaking.GetLobbyDataCount(steamIDLobby);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamMatchmaking_GetLobbyDataByIndex(IntPtr self, IntPtr steamIDLobby, int iLobbyData, string pchKey, int cchKeyBufferSize, string pchValue, int cchValueBufferSize)
//    {
//        return SteamEmulator.SteamMatchmaking.GetLobbyDataByIndex(steamIDLobby, iLobbyData, pchKey, cchKeyBufferSize, pchValue, cchValueBufferSize);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamMatchmaking_DeleteLobbyData(IntPtr self, IntPtr steamIDLobby, string pchKey)
//    {
//        return SteamEmulator.SteamMatchmaking.DeleteLobbyData(steamIDLobby, pchKey);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static string SteamAPI_ISteamMatchmaking_GetLobbyMemberData(IntPtr self, IntPtr steamIDLobby, IntPtr steamIDUser, string pchKey)
//    {
//        return SteamEmulator.SteamMatchmaking.GetLobbyMemberData(steamIDLobby, steamIDUser, pchKey);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static void SteamAPI_ISteamMatchmaking_SetLobbyMemberData(IntPtr self, IntPtr steamIDLobby, string pchKey, string pchValue)
//    {
//        SteamEmulator.SteamMatchmaking.SetLobbyMemberData(steamIDLobby, pchKey, pchValue);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamMatchmaking_SendLobbyChatMsg(IntPtr self, IntPtr steamIDLobby, IntPtr pvMsgBody, int cubMsgBody)
//    {
//        return SteamEmulator.SteamMatchmaking.SendLobbyChatMsg(steamIDLobby, pvMsgBody, cubMsgBody);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static int SteamAPI_ISteamMatchmaking_GetLobbyChatEntry(IntPtr self, IntPtr steamIDLobby, int iChatID, IntPtr pSteamIDUser, IntPtr pvData, int cubData, EChatEntryType peChatEntryType)
//    {
//        return SteamEmulator.SteamMatchmaking.GetLobbyChatEntry(steamIDLobby, iChatID, pSteamIDUser, pvData, cubData, peChatEntryType);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamMatchmaking_RequestLobbyData(IntPtr self, IntPtr steamIDLobby)
//    {
//        return SteamEmulator.SteamMatchmaking.RequestLobbyData(steamIDLobby);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static void SteamAPI_ISteamMatchmaking_SetLobbyGameServer(IntPtr self, IntPtr steamIDLobby, uint unGameServerIP, uint unGameServerPort, IntPtr steamIDGameServer)
//    {
//        SteamEmulator.SteamMatchmaking.SetLobbyGameServer(steamIDLobby, unGameServerIP, unGameServerPort, steamIDGameServer);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamMatchmaking_GetLobbyGameServer(IntPtr self, IntPtr steamIDLobby, uint punGameServerIP, uint punGameServerPort, IntPtr psteamIDGameServer)
//    {
//        return SteamEmulator.SteamMatchmaking.GetLobbyGameServer(steamIDLobby, punGameServerIP, punGameServerPort, psteamIDGameServer);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamMatchmaking_SetLobbyMemberLimit(IntPtr self, IntPtr steamIDLobby, int cMaxMembers)
//    {
//        return SteamEmulator.SteamMatchmaking.SetLobbyMemberLimit(steamIDLobby, cMaxMembers);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static int SteamAPI_ISteamMatchmaking_GetLobbyMemberLimit(IntPtr self, IntPtr steamIDLobby)
//    {
//        return SteamEmulator.SteamMatchmaking.GetLobbyMemberLimit(steamIDLobby);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamMatchmaking_SetLobbyType(IntPtr self, IntPtr steamIDLobby, ELobbyType eLobbyType)
//    {
//        return SteamEmulator.SteamMatchmaking.SetLobbyType(steamIDLobby, eLobbyType);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamMatchmaking_SetLobbyJoinable(IntPtr self, IntPtr steamIDLobby, bool bLobbyJoinable)
//    {
//        return SteamEmulator.SteamMatchmaking.SetLobbyJoinable(steamIDLobby, bLobbyJoinable);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static IntPtr SteamAPI_ISteamMatchmaking_GetLobbyOwner(IntPtr self, IntPtr steamIDLobby)
//    {
//        return SteamEmulator.SteamMatchmaking.GetLobbyOwner(steamIDLobby);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamMatchmaking_SetLobbyOwner(IntPtr self, IntPtr steamIDLobby, IntPtr steamIDNewOwner)
//    {
//        return SteamEmulator.SteamMatchmaking.SetLobbyOwner(steamIDLobby, steamIDNewOwner);
//    }

//    [DllExport(CallingConvention = CallingConvention.Cdecl)]
//    public static bool SteamAPI_ISteamMatchmaking_SetLinkedLobby(IntPtr self, IntPtr steamIDLobby, IntPtr steamIDLobbyDependent)
//    {
//        return SteamEmulator.SteamMatchmaking.SetLinkedLobby(steamIDLobby, steamIDLobbyDependent);
//    }

//}

