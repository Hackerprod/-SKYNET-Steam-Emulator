using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helper;
using SKYNET.Interface;
using SKYNET.Types;
using Steamworks;

public class SteamAPI_ISteamFriends : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamFriends_ActivateGameOverlay([MarshalAs(UnmanagedType.LPStr)] string friendsGroupID)
    {
        Write($"SteamAPI_ISteamFriends_ActivateGameOverlay {friendsGroupID}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialog(IntPtr steamIDLobby)
    {
        Write($"SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialog {steamIDLobby}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialogConnectString([MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
    {
        Write($"SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialogConnectString {pchConnectString}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamFriends_ActivateGameOverlayRemotePlayTogetherInviteDialog(IntPtr steamIDLobby)
    {
        Write($"SteamAPI_ISteamFriends_ActivateGameOverlayRemotePlayTogetherInviteDialog {steamIDLobby}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamFriends_ActivateGameOverlayToStore(AppId_t nAppID, EOverlayToStoreFlag eFlag)
    {
        Write($"SteamAPI_ISteamFriends_ActivateGameOverlayToStore {nAppID} {eFlag}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamFriends_ActivateGameOverlayToUser([MarshalAs(UnmanagedType.LPStr)] string friendsGroupID, IntPtr steamID)
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
    public static bool SteamAPI_ISteamFriends_CloseClanChatWindowInSteam(IntPtr _, IntPtr steamIDClanChat)
    {
        Write($"SteamAPI_ISteamFriends_CloseClanChatWindowInSteam {steamIDClanChat}");
        return SteamEmulator.SteamFriends.CloseClanChatWindowInSteam(_, steamIDClanChat);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamFriends_DownloadClanActivityCounts(IntPtr _, IntPtr[] psteamIDClans, int cClansToRequest)
    {
        Write($"SteamAPI_ISteamFriends_DownloadClanActivityCounts {cClansToRequest}");
        return SteamEmulator.SteamFriends.DownloadClanActivityCounts(_, psteamIDClans, cClansToRequest);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamFriends_EnumerateFollowingList(IntPtr _, uint unStartIndex)
    {
        Write($"SteamAPI_ISteamFriends_EnumerateFollowingList {unStartIndex}");
        return SteamEmulator.SteamFriends.EnumerateFollowingList(_, unStartIndex);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamFriends_GetChatMemberByIndex(IntPtr _, IntPtr steamIDClan, int iUser)
    {
        Write($"SteamAPI_ISteamFriends_GetChatMemberByIndex {steamIDClan}");
        return SteamEmulator.SteamFriends.GetChatMemberByIndex(_, steamIDClan, iUser);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_GetClanActivityCounts(IntPtr _, IntPtr steamIDClan, int pnOnline, int pnInGame, int pnChatting)
    {
        Write($"SteamAPI_ISteamFriends_ActivateGameOverlay {steamIDClan}");
        return SteamEmulator.SteamFriends.GetClanActivityCounts(_, steamIDClan, pnOnline, pnInGame, pnChatting);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamFriends_GetClanByIndex(IntPtr _, int iClan)
    {
        Write($"SteamAPI_ISteamFriends_GetClanByIndex {iClan}");
        return SteamEmulator.SteamFriends.GetClanByIndex(_, iClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetClanChatMemberCount(IntPtr _, IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_GetClanChatMemberCount {steamIDClan}");
        return SteamEmulator.SteamFriends.GetClanChatMemberCount(_, steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetClanChatMessage(IntPtr _, IntPtr steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, EChatEntryType peChatEntryType, IntPtr[] psteamidChatter)
    {
        Write($"SteamAPI_ISteamFriends_GetClanChatMessage {steamIDClanChat}");
        return SteamEmulator.SteamFriends.GetClanChatMessage(_, steamIDClanChat, iMessage, prgchText, cchTextMax, peChatEntryType, psteamidChatter);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetClanCount(IntPtr _)
    {
        Write($"SteamAPI_ISteamFriends_GetClanCount");
        return SteamEmulator.SteamFriends.GetClanCount(_);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamFriends_GetClanName(IntPtr _, IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_GetClanName {steamIDClan}");
        return SteamEmulator.SteamFriends.GetClanName(_, steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamFriends_GetClanOfficerByIndex(IntPtr _, IntPtr steamIDClan, int iOfficer)
    {
        Write($"SteamAPI_ISteamFriends_GetClanOfficerByIndex {steamIDClan}");
        return SteamEmulator.SteamFriends.GetClanOfficerByIndex(_, steamIDClan, iOfficer);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetClanOfficerCount(IntPtr _, IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_GetClanOfficerCount {steamIDClan}");
        return SteamEmulator.SteamFriends.GetClanOfficerCount(_, steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamFriends_GetClanOwner(IntPtr _, IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_GetClanOwner {steamIDClan}");
        return SteamEmulator.SteamFriends.GetClanOwner(_, steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamFriends_GetClanTag(IntPtr _, IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_GetClanTag {steamIDClan}");
        return SteamEmulator.SteamFriends.GetClanTag(_, steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamFriends_GetCoplayFriend(IntPtr _, int iCoplayFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetCoplayFriend {iCoplayFriend}");
        return SteamEmulator.SteamFriends.GetCoplayFriend(_, iCoplayFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetCoplayFriendCount(IntPtr _)
    {
        Write($"SteamAPI_ISteamFriends_GetCoplayFriendCount");
        return SteamEmulator.SteamFriends.GetCoplayFriendCount(_);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamFriends_GetFollowerCount(IntPtr _, IntPtr steamID)
    {
        Write($"SteamAPI_ISteamFriends_GetFollowerCount {steamID}");
        return SteamEmulator.SteamFriends.GetFollowerCount(_, steamID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamFriends_GetFriendByIndex(IntPtr _, int iFriend, int iFriendFlags)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendByIndex {iFriend}");
        return SteamEmulator.SteamFriends.GetFriendByIndex(_, iFriend, iFriendFlags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static AppId_t SteamAPI_ISteamFriends_GetFriendCoplayGame(IntPtr _, IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendCoplayGame {steamIDFriend}");
        return SteamEmulator.SteamFriends.GetFriendCoplayGame(_, steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetFriendCoplayTime(IntPtr _, IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendCoplayTime {steamIDFriend}");
        return SteamEmulator.SteamFriends.GetFriendCoplayTime(_, steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetFriendCount(IntPtr _, int iFriendFlags)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendCount {iFriendFlags}");
        return SteamEmulator.SteamFriends.GetFriendCount(_, iFriendFlags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetFriendCountFromSource(IntPtr _, IntPtr steamIDSource)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendCountFromSource {steamIDSource}");
        return SteamEmulator.SteamFriends.GetFriendCountFromSource(_, steamIDSource);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamFriends_GetFriendFromSourceByIndex(IntPtr _, IntPtr steamIDSource, int iFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendFromSourceByIndex {steamIDSource} {iFriend}");
        return SteamEmulator.SteamFriends.GetFriendFromSourceByIndex(_, steamIDSource, iFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_GetFriendGamePlayed(IntPtr _, IntPtr steamIDFriend, out FriendGameInfo_t pFriendGameInfo)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendGamePlayed {(uint)steamIDFriend}");
        return SteamEmulator.SteamFriends.GetFriendGamePlayed(_, steamIDFriend, out pFriendGameInfo);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetFriendMessage(IntPtr _, IntPtr steamIDFriend, int iMessageID, IntPtr pvData, int cubData, EChatEntryType peChatEntryType)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendMessage {steamIDFriend}");
        return SteamEmulator.SteamFriends.GetFriendMessage(_, steamIDFriend, iMessageID, pvData, cubData, peChatEntryType);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamFriends_GetFriendPersonaName(IntPtr _, IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendPersonaName {(uint)steamIDFriend}");
        return SteamEmulator.SteamFriends.GetFriendPersonaName(_, steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamFriends_GetFriendPersonaNameHistory(IntPtr _, IntPtr steamIDFriend, int iPersonaName)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendPersonaNameHistory {steamIDFriend}");
        return SteamEmulator.SteamFriends.GetFriendPersonaNameHistory(_, steamIDFriend, iPersonaName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static EPersonaState SteamAPI_ISteamFriends_GetFriendPersonaState(IntPtr _, IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendPersonaState {steamIDFriend}");
        return SteamEmulator.SteamFriends.GetFriendPersonaState(_, steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static EFriendRelationship SteamAPI_ISteamFriends_GetFriendRelationship(IntPtr _, IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendRelationship {steamIDFriend}");
        return SteamEmulator.SteamFriends.GetFriendRelationship(_, steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamFriends_GetFriendRichPresence(IntPtr _, IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchKey)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendRichPresence {steamIDFriend} {pchKey}");
        return SteamEmulator.SteamFriends.GetFriendRichPresence(_, steamIDFriend, pchKey);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamFriends_GetFriendRichPresenceKeyByIndex(IntPtr _, IntPtr steamIDFriend, int iKey)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendRichPresenceKeyByIndex {steamIDFriend} {iKey}");
        return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyByIndex(_, steamIDFriend, iKey);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetFriendRichPresenceKeyCount(IntPtr _, IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendRichPresenceKeyCount {steamIDFriend}");
        return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyCount(_, steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetFriendsGroupCount(IntPtr _)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendsGroupCount");
        return SteamEmulator.SteamFriends.GetFriendsGroupCount(_);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static FriendsGroupID_t SteamAPI_ISteamFriends_GetFriendsGroupIDByIndex(IntPtr _, int iFG)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendsGroupIDByIndex {iFG}");
        return SteamEmulator.SteamFriends.GetFriendsGroupIDByIndex(_, iFG);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetFriendsGroupMembersCount(IntPtr _, FriendsGroupID_t friendsGroupID)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendsGroupMembersCount {friendsGroupID}");
        return SteamEmulator.SteamFriends.GetFriendsGroupMembersCount(_, friendsGroupID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamFriends_GetFriendsGroupMembersList(IntPtr _, FriendsGroupID_t friendsGroupID, IntPtr[] pOutSteamIDMembers, int nMembersCount)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendsGroupMembersList {friendsGroupID}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamFriends_GetFriendsGroupName(IntPtr _, FriendsGroupID_t friendsGroupID)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendsGroupName {friendsGroupID}");
        return SteamEmulator.SteamFriends.GetFriendsGroupName(_, friendsGroupID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetFriendSteamLevel(IntPtr _, IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendSteamLevel {steamIDFriend}");
        return SteamEmulator.SteamFriends.GetFriendSteamLevel(_, steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetSmallFriendAvatar(IntPtr _, IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetSmallFriendAvatar {steamIDFriend}");
        return SteamEmulator.SteamFriends.GetSmallFriendAvatar(_, steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetMediumFriendAvatar(IntPtr _, IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetMediumFriendAvatar {steamIDFriend}");
        return SteamEmulator.SteamFriends.GetMediumFriendAvatar(_, steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetLargeFriendAvatar(IntPtr _, IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetLargeFriendAvatar {steamIDFriend}");
        return SteamEmulator.SteamFriends.GetLargeFriendAvatar(_, steamIDFriend);
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
    public static string SteamAPI_ISteamFriends_GetPlayerNickname(IntPtr _, IntPtr steamIDPlayer)
    {
        Write($"SteamAPI_ISteamFriends_GetPlayerNickname {steamIDPlayer}");
        return SteamEmulator.SteamFriends.GetPlayerNickname(_, steamIDPlayer);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamFriends_GetUserRestrictions(IntPtr _)
    {
        Write($"SteamAPI_ISteamFriends_GetUserRestrictions");
        return SteamEmulator.SteamFriends.GetUserRestrictions(_);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_HasFriend(IntPtr _, IntPtr steamIDFriend, int iFriendFlags)
    {
        Write($"SteamAPI_ISteamFriends_HasFriend {steamIDFriend}");
        return SteamEmulator.SteamFriends.HasFriend(_, steamIDFriend, iFriendFlags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_InviteUserToGame(IntPtr _, IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
    {
        Write($"SteamAPI_ISteamFriends_InviteUserToGame {steamIDFriend} {pchConnectString}");
        return SteamEmulator.SteamFriends.InviteUserToGame(_, steamIDFriend, pchConnectString);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_IsClanChatAdmin(IntPtr _, IntPtr steamIDClanChat, IntPtr steamIDUser)
    {
        Write($"SteamAPI_ISteamFriends_IsClanChatAdmin {steamIDClanChat}");
        return SteamEmulator.SteamFriends.IsClanChatAdmin(_, steamIDClanChat, steamIDUser);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_IsClanChatWindowOpenInSteam(IntPtr _, IntPtr steamIDClanChat)
    {
        Write($"SteamAPI_ISteamFriends_IsClanChatWindowOpenInSteam {steamIDClanChat}");
        return SteamEmulator.SteamFriends.IsClanChatWindowOpenInSteam(_, steamIDClanChat);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_IsClanOfficialGameGroup(IntPtr _, IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_IsClanOfficialGameGroup {steamIDClan}");
        return SteamEmulator.SteamFriends.IsClanOfficialGameGroup(_, steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_IsClanPublic(IntPtr _, IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_IsClanpublic static {steamIDClan}");
        return SteamEmulator.SteamFriends.IsClanPublic(_, steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamFriends_IsFollowing(IntPtr _, IntPtr steamID)
    {
        Write($"SteamAPI_ISteamFriends_IsFollowing {steamID}");
        return SteamEmulator.SteamFriends.IsFollowing(_, steamID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_IsUserInSource(IntPtr _, IntPtr steamIDUser, IntPtr steamIDSource)
    {
        Write($"SteamAPI_ISteamFriends_IsUserInSource {steamIDUser}");
        return SteamEmulator.SteamFriends.IsUserInSource(_, steamIDUser, steamIDSource);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamFriends_JoinClanChatRoom(IntPtr _, IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_JoinClanChatRoom {steamIDClan}");
        return SteamEmulator.SteamFriends.JoinClanChatRoom(_, steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_LeaveClanChatRoom(IntPtr _, IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_LeaveClanChatRoom {steamIDClan}");
        return SteamEmulator.SteamFriends.LeaveClanChatRoom(_, steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_OpenClanChatWindowInSteam(IntPtr _, IntPtr steamIDClanChat)
    {
        Write($"SteamAPI_ISteamFriends_OpenClanChatWindowInSteam {steamIDClanChat}");
        return SteamEmulator.SteamFriends.OpenClanChatWindowInSteam(_, steamIDClanChat);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_RegisterProtocolInOverlayBrowser(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchProtocol)
    {
        Write($"SteamAPI_ISteamFriends_RegisterProtocolInOverlayBrowser {pchProtocol}");
        return SteamEmulator.SteamFriends.RegisterProtocolInOverlayBrowser(_, pchProtocol);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_ReplyToFriendMessage(IntPtr _, IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchMsgToSend)
    {
        Write($"SteamAPI_ISteamFriends_ReplyToFriendMessage {steamIDFriend} {pchMsgToSend}");
        return SteamEmulator.SteamFriends.ReplyToFriendMessage(_, steamIDFriend, pchMsgToSend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamFriends_RequestClanOfficerList(IntPtr _, IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_RequestClanOfficerList {steamIDClan}");
        return SteamEmulator.SteamFriends.RequestClanOfficerList(_, steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamFriends_RequestFriendRichPresence(IntPtr _, IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_RequestFriendRichPresence {steamIDFriend}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_RequestUserInformation(IntPtr _, IntPtr steamIDUser, bool bRequireNameOnly)
    {
        Write($"SteamAPI_ISteamFriends_RequestUserInformation {steamIDUser}");
        return SteamEmulator.SteamFriends.RequestUserInformation(_, steamIDUser, bRequireNameOnly);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_SendClanChatMessage(IntPtr _, IntPtr steamIDClanChat, [MarshalAs(UnmanagedType.LPStr)] string pchText)
    {
        Write($"SteamAPI_ISteamFriends_SendClanChatMessage {steamIDClanChat} {pchText}");
        return SteamEmulator.SteamFriends.SendClanChatMessage(_, steamIDClanChat, pchText);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamFriends_SetInGameVoiceSpeaking(IntPtr _, IntPtr steamIDUser, bool bSpeaking)
    {
        Write($"SteamAPI_ISteamFriends_SetInGameVoiceSpeaking {steamIDUser}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_SetListenForFriendsMessages(IntPtr _, bool bInterceptEnabled)
    {
        Write($"SteamAPI_ISteamFriends_SetListenForFriendsMessages {bInterceptEnabled}");
        return SteamEmulator.SteamFriends.SetListenForFriendsMessages(_, bInterceptEnabled);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamFriends_SetPersonaName(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchPersonaName)
    {
        Write($"SteamAPI_ISteamFriends_SetPersonaName {pchPersonaName}");
        return SteamEmulator.SteamFriends.SetPersonaName(_, pchPersonaName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamFriends_SetPlayedWith(IntPtr _, IntPtr steamIDUserPlayedWith)
    {
        Write($"SteamAPI_ISteamFriends_SetPlayedWith {steamIDUserPlayedWith}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_SetRichPresence(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchKey, [MarshalAs(UnmanagedType.LPStr)] string pchValue)
    {
        Write($"SteamAPI_ISteamFriends_SetRichPresence {pchKey} {pchValue}");
        return SteamEmulator.SteamFriends.SetRichPresence(_, pchKey, pchValue);
    }
}

