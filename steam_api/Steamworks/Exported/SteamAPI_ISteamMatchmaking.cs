using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SKYNET;
using SKYNET.Steamworks;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamMatchmaking 
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamMatchmaking_GetFavoriteGameCount(IntPtr _)
        {
            Write("SteamAPI_ISteamMatchmaking_GetFavoriteGameCount");
            return SteamEmulator.SteamMatchmaking.GetFavoriteGameCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMatchmaking_GetFavoriteGame(IntPtr _, int iGame, uint pnAppID, uint pnIP, uint pnConnPort, uint pnQueryPort, uint punFlags, uint pRTime32LastPlayedOnServer)
        {
            Write("SteamAPI_ISteamMatchmaking_GetFavoriteGame");
            return SteamEmulator.SteamMatchmaking.GetFavoriteGame(iGame, pnAppID, pnIP, pnConnPort, pnQueryPort, punFlags, pRTime32LastPlayedOnServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamMatchmaking_AddFavoriteGame(IntPtr _, uint nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags, uint rTime32LastPlayedOnServer)
        {
            Write("SteamAPI_ISteamMatchmaking_AddFavoriteGame");
            return SteamEmulator.SteamMatchmaking.AddFavoriteGame(nAppID, nIP, nConnPort, nQueryPort, unFlags, rTime32LastPlayedOnServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMatchmaking_RemoveFavoriteGame(IntPtr _, uint nAppID, uint nIP, uint nConnPort, uint nQueryPort, uint unFlags)
        {
            Write("SteamAPI_ISteamMatchmaking_RemoveFavoriteGame");
            return SteamEmulator.SteamMatchmaking.RemoveFavoriteGame(nAppID, nIP, nConnPort, nQueryPort, unFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamMatchmaking_RequestLobbyList(IntPtr _)
        {
            Write("SteamAPI_ISteamMatchmaking_RequestLobbyList");
            return SteamEmulator.SteamMatchmaking.RequestLobbyList();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmaking_AddRequestLobbyListStringFilter(IntPtr _, string pchKeyToMatch, string pchValueToMatch, int eComparisonType)
        {
            Write("SteamAPI_ISteamMatchmaking_AddRequestLobbyListStringFilter");
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListStringFilter(pchKeyToMatch, pchValueToMatch, eComparisonType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmaking_AddRequestLobbyListNumericalFilter(IntPtr _, string pchKeyToMatch, int nValueToMatch, int eComparisonType)
        {
            Write("SteamAPI_ISteamMatchmaking_AddRequestLobbyListNumericalFilter");
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListNumericalFilter(pchKeyToMatch, nValueToMatch, eComparisonType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmaking_AddRequestLobbyListNearValueFilter(IntPtr _, string pchKeyToMatch, int nValueToBeCloseTo)
        {
            Write("SteamAPI_ISteamMatchmaking_AddRequestLobbyListNearValueFilter");
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListNearValueFilter(pchKeyToMatch, nValueToBeCloseTo);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmaking_AddRequestLobbyListFilterSlotsAvailable(IntPtr _, int nSlotsAvailable)
        {
            Write("SteamAPI_ISteamMatchmaking_AddRequestLobbyListFilterSlotsAvailable");
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(nSlotsAvailable);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmaking_AddRequestLobbyListDistanceFilter(IntPtr _, int eLobbyDistanceFilter)
        {
            Write("SteamAPI_ISteamMatchmaking_AddRequestLobbyListDistanceFilter");
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListDistanceFilter(eLobbyDistanceFilter);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmaking_AddRequestLobbyListResultCountFilter(IntPtr _, int cMaxResults)
        {
            Write("SteamAPI_ISteamMatchmaking_AddRequestLobbyListResultCountFilter");
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListResultCountFilter(cMaxResults);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmaking_AddRequestLobbyListCompatibleMembersFilter(IntPtr _, ulong steamIDLobby)
        {
            Write("SteamAPI_ISteamMatchmaking_AddRequestLobbyListCompatibleMembersFilter");
            SteamEmulator.SteamMatchmaking.AddRequestLobbyListCompatibleMembersFilter(steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamMatchmaking_GetLobbyByIndex(IntPtr _, int iLobby)
        {
            Write("SteamAPI_ISteamMatchmaking_GetLobbyByIndex");
            return SteamEmulator.SteamMatchmaking.GetLobbyByIndex(iLobby).SteamID;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamMatchmaking_CreateLobby(IntPtr _, int eLobbyType, int cMaxMembers)
        {
            Write("SteamAPI_ISteamMatchmaking_CreateLobby");
            return SteamEmulator.SteamMatchmaking.CreateLobby(eLobbyType, cMaxMembers);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamMatchmaking_JoinLobby(IntPtr _, ulong steamIDLobby)
        {
            Write("SteamAPI_ISteamMatchmaking_JoinLobby");
            return SteamEmulator.SteamMatchmaking.JoinLobby(steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmaking_LeaveLobby(IntPtr _, ulong steamIDLobby)
        {
            Write("SteamAPI_ISteamMatchmaking_LeaveLobby");
            SteamEmulator.SteamMatchmaking.LeaveLobby(steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMatchmaking_InviteUserToLobby(IntPtr _, ulong steamIDLobby, ulong steamIDInvitee)
        {
            Write("SteamAPI_ISteamMatchmaking_InviteUserToLobby");
            return SteamEmulator.SteamMatchmaking.InviteUserToLobby(steamIDLobby, steamIDInvitee);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamMatchmaking_GetNumLobbyMembers(IntPtr _, ulong steamIDLobby)
        {
            Write("SteamAPI_ISteamMatchmaking_GetNumLobbyMembers");
            return SteamEmulator.SteamMatchmaking.GetNumLobbyMembers(steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamMatchmaking_GetLobbyMemberByIndex(IntPtr _, ulong steamIDLobby, int iMember)
        {
            Write("SteamAPI_ISteamMatchmaking_GetLobbyMemberByIndex");
            return SteamEmulator.SteamMatchmaking.GetLobbyMemberByIndex(steamIDLobby, iMember).SteamID;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamMatchmaking_GetLobbyData(IntPtr _, ulong steamIDLobby, string pchKey)
        {
            Write("SteamAPI_ISteamMatchmaking_GetLobbyData");
            return SteamEmulator.SteamMatchmaking.GetLobbyData(steamIDLobby, pchKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMatchmaking_SetLobbyData(IntPtr _, ulong steamIDLobby, string pchKey, string pchValue)
        {
            Write("SteamAPI_ISteamMatchmaking_SetLobbyData");
            return SteamEmulator.SteamMatchmaking.SetLobbyData(steamIDLobby, pchKey, pchValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamMatchmaking_GetLobbyDataCount(IntPtr _, ulong steamIDLobby)
        {
            Write("SteamAPI_ISteamMatchmaking_GetLobbyDataCount");
            return SteamEmulator.SteamMatchmaking.GetLobbyDataCount(steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMatchmaking_GetLobbyDataByIndex(IntPtr _, ulong steamIDLobby, int iLobbyData, IntPtr pchKey, int cchKeyBufferSize, IntPtr pchValue, int cchValueBufferSize)
        {
            Write("SteamAPI_ISteamMatchmaking_GetLobbyDataByIndex");
            return SteamEmulator.SteamMatchmaking.GetLobbyDataByIndex(steamIDLobby, iLobbyData, pchKey, cchKeyBufferSize, pchValue, cchValueBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMatchmaking_DeleteLobbyData(IntPtr _, ulong steamIDLobby, string pchKey)
        {
            Write("SteamAPI_ISteamMatchmaking_DeleteLobbyData");
            return SteamEmulator.SteamMatchmaking.DeleteLobbyData(steamIDLobby, pchKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamMatchmaking_GetLobbyMemberData(IntPtr _, ulong steamIDLobby, ulong steamIDUser, string pchKey)
        {
            Write("SteamAPI_ISteamMatchmaking_GetLobbyMemberData");
            return SteamEmulator.SteamMatchmaking.GetLobbyMemberData(steamIDLobby, steamIDUser, pchKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmaking_SetLobbyMemberData(IntPtr _, ulong steamIDLobby, string pchKey, string pchValue)
        {
            Write("SteamAPI_ISteamMatchmaking_SetLobbyMemberData");
            SteamEmulator.SteamMatchmaking.SetLobbyMemberData(steamIDLobby, pchKey, pchValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMatchmaking_SendLobbyChatMsg(IntPtr _, ulong steamIDLobby, IntPtr pvMsgBody, int cubMsgBody)
        {
            Write("SteamAPI_ISteamMatchmaking_SendLobbyChatMsg");
            return SteamEmulator.SteamMatchmaking.SendLobbyChatMsg(steamIDLobby, pvMsgBody, cubMsgBody);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamMatchmaking_GetLobbyChatEntry(IntPtr _, ulong steamIDLobby, int iChatID, ulong pSteamIDUser, IntPtr pvData, int cubData, int peChatEntryType)
        {
            Write("SteamAPI_ISteamMatchmaking_GetLobbyChatEntry");
            return SteamEmulator.SteamMatchmaking.GetLobbyChatEntry(steamIDLobby, iChatID, pSteamIDUser, pvData, cubData, peChatEntryType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMatchmaking_RequestLobbyData(IntPtr _, ulong steamIDLobby)
        {
            Write("SteamAPI_ISteamMatchmaking_RequestLobbyData");
            return SteamEmulator.SteamMatchmaking.RequestLobbyData(steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMatchmaking_SetLobbyGameServer(IntPtr _, ulong steamIDLobby, uint unGameServerIP, uint unGameServerPort, ulong steamIDGameServer)
        {
            Write("SteamAPI_ISteamMatchmaking_SetLobbyGameServer");
            SteamEmulator.SteamMatchmaking.SetLobbyGameServer(steamIDLobby, unGameServerIP, unGameServerPort, steamIDGameServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMatchmaking_GetLobbyGameServer(IntPtr _, ulong steamIDLobby, uint punGameServerIP, uint punGameServerPort, ulong psteamIDGameServer)
        {
            Write("SteamAPI_ISteamMatchmaking_GetLobbyGameServer");
            return SteamEmulator.SteamMatchmaking.GetLobbyGameServer(steamIDLobby, punGameServerIP, punGameServerPort, psteamIDGameServer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMatchmaking_SetLobbyMemberLimit(IntPtr _, ulong steamIDLobby, int cMaxMembers)
        {
            Write("SteamAPI_ISteamMatchmaking_SetLobbyMemberLimit");
            return SteamEmulator.SteamMatchmaking.SetLobbyMemberLimit(steamIDLobby, cMaxMembers);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamMatchmaking_GetLobbyMemberLimit(IntPtr _, ulong steamIDLobby)
        {
            Write("SteamAPI_ISteamMatchmaking_GetLobbyMemberLimit");
            return SteamEmulator.SteamMatchmaking.GetLobbyMemberLimit(steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMatchmaking_SetLobbyType(IntPtr _, ulong steamIDLobby, int eLobbyType)
        {
            Write("SteamAPI_ISteamMatchmaking_SetLobbyType");
            return SteamEmulator.SteamMatchmaking.SetLobbyType(steamIDLobby, eLobbyType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMatchmaking_SetLobbyJoinable(IntPtr _, ulong steamIDLobby, bool bLobbyJoinable)
        {
            Write("SteamAPI_ISteamMatchmaking_SetLobbyJoinable");
            return SteamEmulator.SteamMatchmaking.SetLobbyJoinable(steamIDLobby, bLobbyJoinable);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamMatchmaking_GetLobbyOwner(IntPtr _, ulong steamIDLobby)
        {
            Write("SteamAPI_ISteamMatchmaking_GetLobbyOwner");
            return SteamEmulator.SteamMatchmaking.GetLobbyOwner(steamIDLobby).SteamID;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMatchmaking_SetLobbyOwner(IntPtr _, ulong steamIDLobby, ulong steamIDNewOwner)
        {
            Write("SteamAPI_ISteamMatchmaking_SetLobbyOwner");
            return SteamEmulator.SteamMatchmaking.SetLobbyOwner(steamIDLobby, steamIDNewOwner);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMatchmaking_SetLinkedLobby(IntPtr _, ulong steamIDLobby, ulong steamIDLobbyDependent)
        {
            Write("SteamAPI_ISteamMatchmaking_SetLinkedLobby");
            return SteamEmulator.SteamMatchmaking.SetLinkedLobby(steamIDLobby, steamIDLobbyDependent);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

