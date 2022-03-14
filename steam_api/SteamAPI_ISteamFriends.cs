using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    public static bool SteamAPI_ISteamFriends_CloseClanChatWindowInSteam(IntPtr steamIDClanChat)
    {
        Write($"SteamAPI_ISteamFriends_CloseClanChatWindowInSteam {steamIDClanChat}");
        return SteamClient.SteamFriends.CloseClanChatWindowInSteam(steamIDClanChat);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamFriends_DownloadClanActivityCounts(IntPtr[] psteamIDClans, int cClansToRequest)
    {
        Write($"SteamAPI_ISteamFriends_DownloadClanActivityCounts {cClansToRequest}");
        return SteamClient.SteamFriends.DownloadClanActivityCounts(psteamIDClans, cClansToRequest);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamFriends_EnumerateFollowingList(uint unStartIndex)
    {
        Write($"SteamAPI_ISteamFriends_EnumerateFollowingList {unStartIndex}");
        return SteamClient.SteamFriends.EnumerateFollowingList(unStartIndex);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamFriends_GetChatMemberByIndex(IntPtr steamIDClan, int iUser)
    {
        Write($"SteamAPI_ISteamFriends_GetChatMemberByIndex {steamIDClan}");
        return SteamClient.SteamFriends.GetChatMemberByIndex(steamIDClan, iUser);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_GetClanActivityCounts(IntPtr steamIDClan, int pnOnline, int pnInGame, int pnChatting)
    {
        Write($"SteamAPI_ISteamFriends_ActivateGameOverlay {steamIDClan}");
        return SteamClient.SteamFriends.GetClanActivityCounts(steamIDClan, pnOnline, pnInGame, pnChatting);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamFriends_GetClanByIndex(int iClan)
    {
        Write($"SteamAPI_ISteamFriends_GetClanByIndex {iClan}");
        return SteamClient.SteamFriends.GetClanByIndex(iClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetClanChatMemberCount(IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_GetClanChatMemberCount {steamIDClan}");
        return SteamClient.SteamFriends.GetClanChatMemberCount(steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetClanChatMessage(IntPtr steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, EChatEntryType peChatEntryType, IntPtr[] psteamidChatter)
    {
        Write($"SteamAPI_ISteamFriends_GetClanChatMessage {steamIDClanChat}");
        return SteamClient.SteamFriends.GetClanChatMessage(steamIDClanChat, iMessage, prgchText, cchTextMax, peChatEntryType, psteamidChatter);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetClanCount()
    {
        Write($"SteamAPI_ISteamFriends_GetClanCount");
        return SteamClient.SteamFriends.GetClanCount();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamFriends_GetClanName(IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_GetClanName {steamIDClan}");
        return SteamClient.SteamFriends.GetClanName(steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamFriends_GetClanOfficerByIndex(IntPtr steamIDClan, int iOfficer)
    {
        Write($"SteamAPI_ISteamFriends_GetClanOfficerByIndex {steamIDClan}");
        return SteamClient.SteamFriends.GetClanOfficerByIndex(steamIDClan, iOfficer);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetClanOfficerCount(IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_GetClanOfficerCount {steamIDClan}");
        return SteamClient.SteamFriends.GetClanOfficerCount(steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamFriends_GetClanOwner(IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_GetClanOwner {steamIDClan}");
        return SteamClient.SteamFriends.GetClanOwner(steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamFriends_GetClanTag(IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_GetClanTag {steamIDClan}");
        return SteamClient.SteamFriends.GetClanTag(steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamFriends_GetCoplayFriend(int iCoplayFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetCoplayFriend {iCoplayFriend}");
        return SteamClient.SteamFriends.GetCoplayFriend(iCoplayFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetCoplayFriendCount()
    {
        Write($"SteamAPI_ISteamFriends_GetCoplayFriendCount");
        return SteamClient.SteamFriends.GetCoplayFriendCount();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamFriends_GetFollowerCount(IntPtr steamID)
    {
        Write($"SteamAPI_ISteamFriends_GetFollowerCount {steamID}");
        return SteamClient.SteamFriends.GetFollowerCount(steamID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamFriends_GetFriendByIndex(int iFriend, int iFriendFlags)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendByIndex {iFriend}");
        return SteamClient.SteamFriends.GetFriendByIndex(iFriend, iFriendFlags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static AppId_t SteamAPI_ISteamFriends_GetFriendCoplayGame(IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendCoplayGame {steamIDFriend}");
        return SteamClient.SteamFriends.GetFriendCoplayGame(steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetFriendCoplayTime(IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendCoplayTime {steamIDFriend}");
        return SteamClient.SteamFriends.GetFriendCoplayTime(steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetFriendCount(int iFriendFlags)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendCount {iFriendFlags}");
        return SteamClient.SteamFriends.GetFriendCount(iFriendFlags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetFriendCountFromSource(IntPtr steamIDSource)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendCountFromSource {steamIDSource}");
        return SteamClient.SteamFriends.GetFriendCountFromSource(steamIDSource);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamFriends_GetFriendFromSourceByIndex(IntPtr steamIDSource, int iFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendFromSourceByIndex {steamIDSource} {iFriend}");
        return SteamClient.SteamFriends.GetFriendFromSourceByIndex(steamIDSource, iFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_GetFriendGamePlayed(IntPtr steamIDFriend, out FriendGameInfo_t pFriendGameInfo)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendGamePlayed {(uint)steamIDFriend}");
        return SteamClient.SteamFriends.GetFriendGamePlayed(steamIDFriend, out pFriendGameInfo);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetFriendMessage(IntPtr steamIDFriend, int iMessageID, IntPtr pvData, int cubData, EChatEntryType peChatEntryType)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendMessage {steamIDFriend}");
        return SteamClient.SteamFriends.GetFriendMessage(steamIDFriend, iMessageID, pvData, cubData, peChatEntryType);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamFriends_GetFriendPersonaName(IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendPersonaName {(uint)steamIDFriend}");
        return SteamClient.SteamFriends.GetFriendPersonaName(steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamFriends_GetFriendPersonaNameHistory(IntPtr steamIDFriend, int iPersonaName)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendPersonaNameHistory {steamIDFriend}");
        return SteamClient.SteamFriends.GetFriendPersonaNameHistory(steamIDFriend, iPersonaName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static EPersonaState SteamAPI_ISteamFriends_GetFriendPersonaState(IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendPersonaState {steamIDFriend}");
        return SteamClient.SteamFriends.GetFriendPersonaState(steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static EFriendRelationship SteamAPI_ISteamFriends_GetFriendRelationship(IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendRelationship {steamIDFriend}");
        return SteamClient.SteamFriends.GetFriendRelationship(steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamFriends_GetFriendRichPresence(IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchKey)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendRichPresence {steamIDFriend} {pchKey}");
        return SteamClient.SteamFriends.GetFriendRichPresence(steamIDFriend, pchKey);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamFriends_GetFriendRichPresenceKeyByIndex(IntPtr steamIDFriend, int iKey)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendRichPresenceKeyByIndex {steamIDFriend} {iKey}");
        return SteamClient.SteamFriends.GetFriendRichPresenceKeyByIndex(steamIDFriend, iKey);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetFriendRichPresenceKeyCount(IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendRichPresenceKeyCount {steamIDFriend}");
        return SteamClient.SteamFriends.GetFriendRichPresenceKeyCount(steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetFriendsGroupCount()
    {
        Write($"SteamAPI_ISteamFriends_GetFriendsGroupCount");
        return SteamClient.SteamFriends.GetFriendsGroupCount();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static FriendsGroupID_t SteamAPI_ISteamFriends_GetFriendsGroupIDByIndex(int iFG)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendsGroupIDByIndex {iFG}");
        return SteamClient.SteamFriends.GetFriendsGroupIDByIndex(iFG);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetFriendsGroupMembersCount(FriendsGroupID_t friendsGroupID)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendsGroupMembersCount {friendsGroupID}");
        return SteamClient.SteamFriends.GetFriendsGroupMembersCount(friendsGroupID);
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
        return SteamClient.SteamFriends.GetFriendsGroupName(friendsGroupID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetFriendSteamLevel(IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetFriendSteamLevel {steamIDFriend}");
        return SteamClient.SteamFriends.GetFriendSteamLevel(steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetSmallFriendAvatar(IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetSmallFriendAvatar {steamIDFriend}");
        return SteamClient.SteamFriends.GetSmallFriendAvatar(steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetMediumFriendAvatar(IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetMediumFriendAvatar {steamIDFriend}");
        return SteamClient.SteamFriends.GetMediumFriendAvatar(steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetLargeFriendAvatar(IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_GetLargeFriendAvatar {steamIDFriend}");
        return SteamClient.SteamFriends.GetLargeFriendAvatar(steamIDFriend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamFriends_GetNumChatsWithUnreadPriorityMessages()
    {
        Write($"SteamAPI_ISteamFriends_GetNumChatsWithUnreadPriorityMessages");
        return SteamClient.SteamFriends.GetNumChatsWithUnreadPriorityMessages();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamFriends_GetPersonaName()
    {
        Write($"SteamAPI_ISteamFriends_GetPersonaName");
        return SteamClient.SteamFriends.GetPersonaName();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static EPersonaState SteamAPI_ISteamFriends_GetPersonaState()
    {
        Write($"SteamAPI_ISteamFriends_GetPersonaState");
        return SteamClient.SteamFriends.GetPersonaState();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static string SteamAPI_ISteamFriends_GetPlayerNickname(IntPtr steamIDPlayer)
    {
        Write($"SteamAPI_ISteamFriends_GetPlayerNickname {steamIDPlayer}");
        return SteamClient.SteamFriends.GetPlayerNickname(steamIDPlayer);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamFriends_GetUserRestrictions()
    {
        Write($"SteamAPI_ISteamFriends_GetUserRestrictions");
        return SteamClient.SteamFriends.GetUserRestrictions();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_HasFriend(IntPtr steamIDFriend, int iFriendFlags)
    {
        Write($"SteamAPI_ISteamFriends_HasFriend {steamIDFriend}");
        return SteamClient.SteamFriends.HasFriend(steamIDFriend, iFriendFlags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_InviteUserToGame(IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
    {
        Write($"SteamAPI_ISteamFriends_InviteUserToGame {steamIDFriend} {pchConnectString}");
        return SteamClient.SteamFriends.InviteUserToGame(steamIDFriend, pchConnectString);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_IsClanChatAdmin(IntPtr steamIDClanChat, IntPtr steamIDUser)
    {
        Write($"SteamAPI_ISteamFriends_IsClanChatAdmin {steamIDClanChat}");
        return SteamClient.SteamFriends.IsClanChatAdmin(steamIDClanChat, steamIDUser);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_IsClanChatWindowOpenInSteam(IntPtr steamIDClanChat)
    {
        Write($"SteamAPI_ISteamFriends_IsClanChatWindowOpenInSteam {steamIDClanChat}");
        return SteamClient.SteamFriends.IsClanChatWindowOpenInSteam(steamIDClanChat);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_IsClanOfficialGameGroup(IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_IsClanOfficialGameGroup {steamIDClan}");
        return SteamClient.SteamFriends.IsClanOfficialGameGroup(steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_IsClanPublic(IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_IsClanpublic static {steamIDClan}");
        return SteamClient.SteamFriends.IsClanPublic(steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamFriends_IsFollowing(IntPtr steamID)
    {
        Write($"SteamAPI_ISteamFriends_IsFollowing {steamID}");
        return SteamClient.SteamFriends.IsFollowing(steamID);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_IsUserInSource(IntPtr steamIDUser, IntPtr steamIDSource)
    {
        Write($"SteamAPI_ISteamFriends_IsUserInSource {steamIDUser}");
        return SteamClient.SteamFriends.IsUserInSource(steamIDUser, steamIDSource);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamFriends_JoinClanChatRoom(IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_JoinClanChatRoom {steamIDClan}");
        return SteamClient.SteamFriends.JoinClanChatRoom(steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_LeaveClanChatRoom(IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_LeaveClanChatRoom {steamIDClan}");
        return SteamClient.SteamFriends.LeaveClanChatRoom(steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_OpenClanChatWindowInSteam(IntPtr steamIDClanChat)
    {
        Write($"SteamAPI_ISteamFriends_OpenClanChatWindowInSteam {steamIDClanChat}");
        return SteamClient.SteamFriends.OpenClanChatWindowInSteam(steamIDClanChat);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_RegisterProtocolInOverlayBrowser([MarshalAs(UnmanagedType.LPStr)] string pchProtocol)
    {
        Write($"SteamAPI_ISteamFriends_RegisterProtocolInOverlayBrowser {pchProtocol}");
        return SteamClient.SteamFriends.RegisterProtocolInOverlayBrowser(pchProtocol);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_ReplyToFriendMessage(IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchMsgToSend)
    {
        Write($"SteamAPI_ISteamFriends_ReplyToFriendMessage {steamIDFriend} {pchMsgToSend}");
        return SteamClient.SteamFriends.ReplyToFriendMessage(steamIDFriend, pchMsgToSend);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamFriends_RequestClanOfficerList(IntPtr steamIDClan)
    {
        Write($"SteamAPI_ISteamFriends_RequestClanOfficerList {steamIDClan}");
        return SteamClient.SteamFriends.RequestClanOfficerList(steamIDClan);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamFriends_RequestFriendRichPresence(IntPtr steamIDFriend)
    {
        Write($"SteamAPI_ISteamFriends_RequestFriendRichPresence {steamIDFriend}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_RequestUserInformation(IntPtr steamIDUser, bool bRequireNameOnly)
    {
        Write($"SteamAPI_ISteamFriends_RequestUserInformation {steamIDUser}");
        return SteamClient.SteamFriends.RequestUserInformation(steamIDUser, bRequireNameOnly);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_SendClanChatMessage(IntPtr steamIDClanChat, [MarshalAs(UnmanagedType.LPStr)] string pchText)
    {
        Write($"SteamAPI_ISteamFriends_SendClanChatMessage {steamIDClanChat} {pchText}");
        return SteamClient.SteamFriends.SendClanChatMessage(steamIDClanChat, pchText);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamFriends_SetInGameVoiceSpeaking(IntPtr steamIDUser, bool bSpeaking)
    {
        Write($"SteamAPI_ISteamFriends_SetInGameVoiceSpeaking {steamIDUser}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_SetListenForFriendsMessages(bool bInterceptEnabled)
    {
        Write($"SteamAPI_ISteamFriends_SetListenForFriendsMessages {bInterceptEnabled}");
        return SteamClient.SteamFriends.SetListenForFriendsMessages(bInterceptEnabled);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static SteamAPICall_t SteamAPI_ISteamFriends_SetPersonaName([MarshalAs(UnmanagedType.LPStr)] string pchPersonaName)
    {
        Write($"SteamAPI_ISteamFriends_SetPersonaName {pchPersonaName}");
        return SteamClient.SteamFriends.SetPersonaName(pchPersonaName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamFriends_SetPlayedWith(IntPtr steamIDUserPlayedWith)
    {
        Write($"SteamAPI_ISteamFriends_SetPlayedWith {steamIDUserPlayedWith}");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamFriends_SetRichPresence([MarshalAs(UnmanagedType.LPStr)] string pchKey, [MarshalAs(UnmanagedType.LPStr)] string pchValue)
    {
        Write($"SteamAPI_ISteamFriends_SetRichPresence {pchKey} {pchValue}");
        return SteamClient.SteamFriends.SetRichPresence(pchKey, pchValue);
    }
}

