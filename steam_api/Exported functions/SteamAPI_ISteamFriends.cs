using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using SKYNET.Types;
using Steamworks;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamFriends : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlay([MarshalAs(UnmanagedType.LPStr)] string friendsGroupID)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlay {friendsGroupID}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialog(ulong steamIDLobby)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialog {steamIDLobby}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialogConnectString([MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialogConnectString {pchConnectString}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayRemotePlayTogetherInviteDialog(ulong steamIDLobby)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayRemotePlayTogetherInviteDialog {steamIDLobby}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayToStore(AppId_t nAppID, EOverlayToStoreFlag eFlag)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayToStore {nAppID} {eFlag}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayToUser([MarshalAs(UnmanagedType.LPStr)] string friendsGroupID, ulong steamID)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayToUser {friendsGroupID} {steamID}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayToWebPage([MarshalAs(UnmanagedType.LPStr)] string pchURL, EActivateGameOverlayToWebPageMode eMode = EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayToWebPage {pchURL}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ClearRichPresence()
        {
            Write($"SteamAPI_ISteamFriends_ClearRichPresence");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_CloseClanChatWindowInSteam(ulong steamIDClanChat)
        {
            Write($"SteamAPI_ISteamFriends_CloseClanChatWindowInSteam {steamIDClanChat}");
            return SteamEmulator.SteamFriends.CloseClanChatWindowInSteam(SteamEmulator.SteamFriends.MemoryAddress, steamIDClanChat);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_DownloadClanActivityCounts(IntPtr[] psteamIDClans, int cClansToRequest)
        {
            Write($"SteamAPI_ISteamFriends_DownloadClanActivityCounts {cClansToRequest}");
            return SteamEmulator.SteamFriends.DownloadClanActivityCounts(SteamEmulator.SteamFriends.MemoryAddress, psteamIDClans, cClansToRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_EnumerateFollowingList(uint unStartIndex)
        {
            Write($"SteamAPI_ISteamFriends_EnumerateFollowingList {unStartIndex}");
            return SteamEmulator.SteamFriends.EnumerateFollowingList(SteamEmulator.SteamFriends.MemoryAddress, unStartIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamFriends_GetChatMemberByIndex(ulong steamIDClan, int iUser)
        {
            Write($"SteamAPI_ISteamFriends_GetChatMemberByIndex {steamIDClan}");
            return SteamEmulator.SteamFriends.GetChatMemberByIndex(SteamEmulator.SteamFriends.MemoryAddress, steamIDClan, iUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_GetClanActivityCounts(ulong steamIDClan, int pnOnline, int pnInGame, int pnChatting)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlay {steamIDClan}");
            return SteamEmulator.SteamFriends.GetClanActivityCounts(SteamEmulator.SteamFriends.MemoryAddress, steamIDClan, pnOnline, pnInGame, pnChatting);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamFriends_GetClanByIndex(int iClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanByIndex {iClan}");
            return SteamEmulator.SteamFriends.GetClanByIndex(SteamEmulator.SteamFriends.MemoryAddress, iClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetClanChatMemberCount(ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanChatMemberCount {steamIDClan}");
            return SteamEmulator.SteamFriends.GetClanChatMemberCount(SteamEmulator.SteamFriends.MemoryAddress, steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetClanChatMessage(ulong steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, EChatEntryType peChatEntryType, IntPtr[] psteamidChatter)
        {
            Write($"SteamAPI_ISteamFriends_GetClanChatMessage {steamIDClanChat}");
            return SteamEmulator.SteamFriends.GetClanChatMessage(SteamEmulator.SteamFriends.MemoryAddress, steamIDClanChat, iMessage, prgchText, cchTextMax, peChatEntryType, psteamidChatter);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetClanCount(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetClanCount");
            return SteamEmulator.SteamFriends.GetClanCount(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetClanName(ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanName {steamIDClan}");
            return SteamEmulator.SteamFriends.GetClanName(SteamEmulator.SteamFriends.MemoryAddress, steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamFriends_GetClanOfficerByIndex(ulong steamIDClan, int iOfficer)
        {
            Write($"SteamAPI_ISteamFriends_GetClanOfficerByIndex {steamIDClan}");
            return SteamEmulator.SteamFriends.GetClanOfficerByIndex(SteamEmulator.SteamFriends.MemoryAddress, steamIDClan, iOfficer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetClanOfficerCount(ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanOfficerCount {steamIDClan}");
            return SteamEmulator.SteamFriends.GetClanOfficerCount(SteamEmulator.SteamFriends.MemoryAddress, steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamFriends_GetClanOwner(ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanOwner {steamIDClan}");
            return SteamEmulator.SteamFriends.GetClanOwner(SteamEmulator.SteamFriends.MemoryAddress, steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetClanTag(ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanTag {steamIDClan}");
            return SteamEmulator.SteamFriends.GetClanTag(SteamEmulator.SteamFriends.MemoryAddress, steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamFriends_GetCoplayFriend(int iCoplayFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetCoplayFriend {iCoplayFriend}");
            return SteamEmulator.SteamFriends.GetCoplayFriend(SteamEmulator.SteamFriends.MemoryAddress, iCoplayFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetCoplayFriendCount(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetCoplayFriendCount");
            return SteamEmulator.SteamFriends.GetCoplayFriendCount(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_GetFollowerCount(ulong steamID)
        {
            Write($"SteamAPI_ISteamFriends_GetFollowerCount {steamID}");
            return SteamEmulator.SteamFriends.GetFollowerCount(SteamEmulator.SteamFriends.MemoryAddress, steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamFriends_GetFriendByIndex(int iFriend, int iFriendFlags)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendByIndex {iFriend}");
            return SteamEmulator.SteamFriends.GetFriendByIndex(SteamEmulator.SteamFriends.MemoryAddress, iFriend, iFriendFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static AppId_t SteamAPI_ISteamFriends_GetFriendCoplayGame(ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendCoplayGame {steamIDFriend}");
            return SteamEmulator.SteamFriends.GetFriendCoplayGame(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendCoplayTime(ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendCoplayTime {steamIDFriend}");
            return SteamEmulator.SteamFriends.GetFriendCoplayTime(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendCount(int iFriendFlags)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendCount {iFriendFlags}");
            return SteamEmulator.SteamFriends.GetFriendCount(SteamEmulator.SteamFriends.MemoryAddress, iFriendFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendCountFromSource(ulong steamIDSource)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendCountFromSource {steamIDSource}");
            return SteamEmulator.SteamFriends.GetFriendCountFromSource(SteamEmulator.SteamFriends.MemoryAddress, steamIDSource);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamFriends_GetFriendFromSourceByIndex(ulong steamIDSource, int iFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendFromSourceByIndex {steamIDSource} {iFriend}");
            return SteamEmulator.SteamFriends.GetFriendFromSourceByIndex(SteamEmulator.SteamFriends.MemoryAddress, steamIDSource, iFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_GetFriendGamePlayed(ulong steamIDFriend, FriendGameInfo_t pFriendGameInfo)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendGamePlayed {(uint)steamIDFriend}");
            return SteamEmulator.SteamFriends.GetFriendGamePlayed(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend, pFriendGameInfo);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendMessage(ulong steamIDFriend, int iMessageID, IntPtr pvData, int cubData, EChatEntryType peChatEntryType)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendMessage {steamIDFriend}");
            return SteamEmulator.SteamFriends.GetFriendMessage(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend, iMessageID, pvData, cubData, peChatEntryType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetFriendPersonaName(ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendPersonaName {(uint)steamIDFriend}");
            return SteamEmulator.SteamFriends.GetFriendPersonaName(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetFriendPersonaNameHistory(ulong steamIDFriend, int iPersonaName)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendPersonaNameHistory {steamIDFriend}");
            return SteamEmulator.SteamFriends.GetFriendPersonaNameHistory(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend, iPersonaName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EPersonaState SteamAPI_ISteamFriends_GetFriendPersonaState(ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendPersonaState {steamIDFriend}");
            return SteamEmulator.SteamFriends.GetFriendPersonaState(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EFriendRelationship SteamAPI_ISteamFriends_GetFriendRelationship(ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendRelationship {steamIDFriend}");
            return SteamEmulator.SteamFriends.GetFriendRelationship(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetFriendRichPresence(ulong steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchKey)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendRichPresence {steamIDFriend} {pchKey}");
            return SteamEmulator.SteamFriends.GetFriendRichPresence(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend, pchKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetFriendRichPresenceKeyByIndex(ulong steamIDFriend, int iKey)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendRichPresenceKeyByIndex {steamIDFriend} {iKey}");
            return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyByIndex(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend, iKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendRichPresenceKeyCount(ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendRichPresenceKeyCount {steamIDFriend}");
            return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyCount(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendsGroupCount(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendsGroupCount");
            return SteamEmulator.SteamFriends.GetFriendsGroupCount(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static FriendsGroupID_t SteamAPI_ISteamFriends_GetFriendsGroupIDByIndex(int iFG)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendsGroupIDByIndex {iFG}");
            return SteamEmulator.SteamFriends.GetFriendsGroupIDByIndex(SteamEmulator.SteamFriends.MemoryAddress, iFG);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendsGroupMembersCount(FriendsGroupID_t friendsGroupID)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendsGroupMembersCount {friendsGroupID}");
            return SteamEmulator.SteamFriends.GetFriendsGroupMembersCount(SteamEmulator.SteamFriends.MemoryAddress, friendsGroupID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_GetFriendsGroupMembersList(FriendsGroupID_t friendsGroupID, IntPtr[] pOutSteamIDMembers, int nMembersCount)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendsGroupMembersList {friendsGroupID}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetFriendsGroupName(FriendsGroupID_t friendsGroupID)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendsGroupName {friendsGroupID}");
            return SteamEmulator.SteamFriends.GetFriendsGroupName(SteamEmulator.SteamFriends.MemoryAddress, friendsGroupID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendSteamLevel(ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendSteamLevel {steamIDFriend}");
            return SteamEmulator.SteamFriends.GetFriendSteamLevel(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetSmallFriendAvatar(ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetSmallFriendAvatar {steamIDFriend}");
            return SteamEmulator.SteamFriends.GetSmallFriendAvatar(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetMediumFriendAvatar(ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetMediumFriendAvatar {steamIDFriend}");
            return SteamEmulator.SteamFriends.GetMediumFriendAvatar(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetLargeFriendAvatar(ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetLargeFriendAvatar {steamIDFriend}");
            return SteamEmulator.SteamFriends.GetLargeFriendAvatar(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetNumChatsWithUnreadPriorityMessages(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetNumChatsWithUnreadPriorityMessages");
            return SteamEmulator.SteamFriends.GetNumChatsWithUnreadPriorityMessages(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetPersonaName(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetPersonaName");
            return SteamEmulator.SteamFriends.GetPersonaName(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EPersonaState SteamAPI_ISteamFriends_GetPersonaState(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetPersonaState");
            return SteamEmulator.SteamFriends.GetPersonaState(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetPlayerNickname(ulong steamIDPlayer)
        {
            Write($"SteamAPI_ISteamFriends_GetPlayerNickname {steamIDPlayer}");
            return SteamEmulator.SteamFriends.GetPlayerNickname(SteamEmulator.SteamFriends.MemoryAddress, steamIDPlayer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamFriends_GetUserRestrictions(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetUserRestrictions");
            return SteamEmulator.SteamFriends.GetUserRestrictions(_);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_HasFriend(ulong steamIDFriend, int iFriendFlags)
        {
            Write($"SteamAPI_ISteamFriends_HasFriend {steamIDFriend}");
            return SteamEmulator.SteamFriends.HasFriend(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend, iFriendFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_InviteUserToGame(ulong steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
        {
            Write($"SteamAPI_ISteamFriends_InviteUserToGame {steamIDFriend} {pchConnectString}");
            return SteamEmulator.SteamFriends.InviteUserToGame(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend, pchConnectString);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_IsClanChatAdmin(ulong steamIDClanChat, ulong steamIDUser)
        {
            Write($"SteamAPI_ISteamFriends_IsClanChatAdmin {steamIDClanChat}");
            return SteamEmulator.SteamFriends.IsClanChatAdmin(SteamEmulator.SteamFriends.MemoryAddress, steamIDClanChat, steamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_IsClanChatWindowOpenInSteam(ulong steamIDClanChat)
        {
            Write($"SteamAPI_ISteamFriends_IsClanChatWindowOpenInSteam {steamIDClanChat}");
            return SteamEmulator.SteamFriends.IsClanChatWindowOpenInSteam(SteamEmulator.SteamFriends.MemoryAddress, steamIDClanChat);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_IsClanOfficialGameGroup(ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_IsClanOfficialGameGroup {steamIDClan}");
            return SteamEmulator.SteamFriends.IsClanOfficialGameGroup(SteamEmulator.SteamFriends.MemoryAddress, steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_IsClanPublic(ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_IsClanpublic static {steamIDClan}");
            return SteamEmulator.SteamFriends.IsClanPublic(SteamEmulator.SteamFriends.MemoryAddress, steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_IsFollowing(ulong steamID)
        {
            Write($"SteamAPI_ISteamFriends_IsFollowing {steamID}");
            return SteamEmulator.SteamFriends.IsFollowing(SteamEmulator.SteamFriends.MemoryAddress, steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_IsUserInSource(ulong steamIDUser, ulong steamIDSource)
        {
            Write($"SteamAPI_ISteamFriends_IsUserInSource {steamIDUser}");
            return SteamEmulator.SteamFriends.IsUserInSource(SteamEmulator.SteamFriends.MemoryAddress, steamIDUser, steamIDSource);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_JoinClanChatRoom(ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_JoinClanChatRoom {steamIDClan}");
            return SteamEmulator.SteamFriends.JoinClanChatRoom(SteamEmulator.SteamFriends.MemoryAddress, steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_LeaveClanChatRoom(ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_LeaveClanChatRoom {steamIDClan}");
            return SteamEmulator.SteamFriends.LeaveClanChatRoom(SteamEmulator.SteamFriends.MemoryAddress, steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_OpenClanChatWindowInSteam(ulong steamIDClanChat)
        {
            Write($"SteamAPI_ISteamFriends_OpenClanChatWindowInSteam {steamIDClanChat}");
            return SteamEmulator.SteamFriends.OpenClanChatWindowInSteam(SteamEmulator.SteamFriends.MemoryAddress, steamIDClanChat);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_RegisterProtocolInOverlayBrowser([MarshalAs(UnmanagedType.LPStr)] string pchProtocol)
        {
            Write($"SteamAPI_ISteamFriends_RegisterProtocolInOverlayBrowser {pchProtocol}");
            return SteamEmulator.SteamFriends.RegisterProtocolInOverlayBrowser(SteamEmulator.SteamFriends.MemoryAddress, pchProtocol);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_ReplyToFriendMessage(ulong steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchMsgToSend)
        {
            Write($"SteamAPI_ISteamFriends_ReplyToFriendMessage {steamIDFriend} {pchMsgToSend}");
            return SteamEmulator.SteamFriends.ReplyToFriendMessage(SteamEmulator.SteamFriends.MemoryAddress, steamIDFriend, pchMsgToSend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_RequestClanOfficerList(ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_RequestClanOfficerList {steamIDClan}");
            return SteamEmulator.SteamFriends.RequestClanOfficerList(SteamEmulator.SteamFriends.MemoryAddress, steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_RequestFriendRichPresence(ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_RequestFriendRichPresence {steamIDFriend}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_RequestUserInformation(ulong steamIDUser, bool bRequireNameOnly)
        {
            Write($"SteamAPI_ISteamFriends_RequestUserInformation {steamIDUser}");
            return SteamEmulator.SteamFriends.RequestUserInformation(SteamEmulator.SteamFriends.MemoryAddress, steamIDUser, bRequireNameOnly);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_SendClanChatMessage(ulong steamIDClanChat, [MarshalAs(UnmanagedType.LPStr)] string pchText)
        {
            Write($"SteamAPI_ISteamFriends_SendClanChatMessage {steamIDClanChat} {pchText}");
            return SteamEmulator.SteamFriends.SendClanChatMessage(SteamEmulator.SteamFriends.MemoryAddress, steamIDClanChat, pchText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_SetInGameVoiceSpeaking(ulong steamIDUser, bool bSpeaking)
        {
            Write($"SteamAPI_ISteamFriends_SetInGameVoiceSpeaking {steamIDUser}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_SetListenForFriendsMessages(bool bInterceptEnabled)
        {
            Write($"SteamAPI_ISteamFriends_SetListenForFriendsMessages {bInterceptEnabled}");
            return SteamEmulator.SteamFriends.SetListenForFriendsMessages(SteamEmulator.SteamFriends.MemoryAddress, bInterceptEnabled);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_SetPersonaName([MarshalAs(UnmanagedType.LPStr)] string pchPersonaName)
        {
            Write($"SteamAPI_ISteamFriends_SetPersonaName {pchPersonaName}");
            return SteamEmulator.SteamFriends.SetPersonaName(SteamEmulator.SteamFriends.MemoryAddress, pchPersonaName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_SetPlayedWith(ulong steamIDUserPlayedWith)
        {
            Write($"SteamAPI_ISteamFriends_SetPlayedWith {steamIDUserPlayedWith}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_SetRichPresence([MarshalAs(UnmanagedType.LPStr)] string pchKey, [MarshalAs(UnmanagedType.LPStr)] string pchValue)
        {
            Write($"SteamAPI_ISteamFriends_SetRichPresence {pchKey} {pchValue}");
            return SteamEmulator.SteamFriends.SetRichPresence(SteamEmulator.SteamFriends.MemoryAddress, pchKey, pchValue);
        }
    }
}

