using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helper;
using SKYNET.Steamworks;
using SKYNET.Types;
using Steamworks;

using SteamAPICall_t = System.UInt64;
using FriendsGroupID_t = System.UInt16;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Exported
{

    public class SteamAPI_SteamFriends
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlay(IntPtr _, string friendsGroupID)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlay");
            SteamEmulator.SteamFriends.ActivateGameOverlay(friendsGroupID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialog(IntPtr _, ulong steamIDLobby)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialog");
            SteamEmulator.SteamFriends.ActivateGameOverlayInviteDialog(steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialogConnectString(IntPtr _, string pchConnectString)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialogConnectString");
            SteamEmulator.SteamFriends.ActivateGameOverlayInviteDialogConnectString(pchConnectString);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayRemotePlayTogetherInviteDialog(IntPtr _, ulong steamIDLobby)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayRemotePlayTogetherInviteDialog");
            SteamEmulator.SteamFriends.ActivateGameOverlayRemotePlayTogetherInviteDialog(steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayToStore(IntPtr _, uint nAppID, int eFlag)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayToStore");
            SteamEmulator.SteamFriends.ActivateGameOverlayToStore(nAppID, eFlag);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayToUser(IntPtr _, string friendsGroupID, ulong steamID)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayToUser");
            SteamEmulator.SteamFriends.ActivateGameOverlayToUser(friendsGroupID, steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayToWebPage(IntPtr _, string pchURL, int eMode)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayToWebPage");
            SteamEmulator.SteamFriends.ActivateGameOverlayToWebPage(pchURL, eMode);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ClearRichPresence(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_ClearRichPresence");
            SteamEmulator.SteamFriends.ClearRichPresence();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_CloseClanChatWindowInSteam(IntPtr _, ulong steamIDClanChat)
        {
            Write($"SteamAPI_ISteamFriends_CloseClanChatWindowInSteam");
            return SteamEmulator.SteamFriends.CloseClanChatWindowInSteam(steamIDClanChat);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_DownloadClanActivityCounts(IntPtr _, IntPtr psteamIDClans, int cClansToRequest)
        {
            Write($"SteamAPI_ISteamFriends_DownloadClanActivityCounts {cClansToRequest}");
            return SteamEmulator.SteamFriends.DownloadClanActivityCounts(psteamIDClans, cClansToRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_EnumerateFollowingList(IntPtr _, uint unStartIndex)
        {
            Write($"SteamAPI_ISteamFriends_EnumerateFollowingList");
            return SteamEmulator.SteamFriends.EnumerateFollowingList(unStartIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID SteamAPI_ISteamFriends_GetChatMemberByIndex(IntPtr _, ulong steamIDClan, int iUser)
        {
            Write($"SteamAPI_ISteamFriends_GetChatMemberByIndex");
            return SteamEmulator.SteamFriends.GetChatMemberByIndex(steamIDClan, iUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_GetClanActivityCounts(IntPtr _, ulong steamIDClan, ref int pnOnline, ref int pnInGame, ref int pnChatting)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlay");
            return SteamEmulator.SteamFriends.GetClanActivityCounts(steamIDClan, ref pnOnline, ref pnInGame, ref pnChatting);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID SteamAPI_ISteamFriends_GetClanByIndex(IntPtr _, int iClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanByIndex");
            return SteamEmulator.SteamFriends.GetClanByIndex(iClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetClanChatMemberCount(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanChatMemberCount");
            return SteamEmulator.SteamFriends.GetClanChatMemberCount(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetClanChatMessage(IntPtr _, ulong steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, int peChatEntryType, ref ulong psteamidChatter)
        {
            Write($"SteamAPI_ISteamFriends_GetClanChatMessage");
            return SteamEmulator.SteamFriends.GetClanChatMessage(steamIDClanChat, iMessage, prgchText, cchTextMax, peChatEntryType, ref psteamidChatter);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetClanCount(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetClanCount");
            return SteamEmulator.SteamFriends.GetClanCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetClanName(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanName");
            return SteamEmulator.SteamFriends.GetClanName(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID SteamAPI_ISteamFriends_GetClanOfficerByIndex(IntPtr _, ulong steamIDClan, int iOfficer)
        {
            Write($"SteamAPI_ISteamFriends_GetClanOfficerByIndex");
            return SteamEmulator.SteamFriends.GetClanOfficerByIndex(steamIDClan, iOfficer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetClanOfficerCount(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanOfficerCount");
            return SteamEmulator.SteamFriends.GetClanOfficerCount(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID SteamAPI_ISteamFriends_GetClanOwner(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanOwner");
            return SteamEmulator.SteamFriends.GetClanOwner(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetClanTag(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanTag");
            return SteamEmulator.SteamFriends.GetClanTag(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID SteamAPI_ISteamFriends_GetCoplayFriend(IntPtr _, int iCoplayFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetCoplayFriend");
            return SteamEmulator.SteamFriends.GetCoplayFriend(iCoplayFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetCoplayFriendCount(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetCoplayFriendCount");
            return SteamEmulator.SteamFriends.GetCoplayFriendCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_GetFollowerCount(IntPtr _, ulong steamID)
        {
            Write($"SteamAPI_ISteamFriends_GetFollowerCount");
            return SteamEmulator.SteamFriends.GetFollowerCount(steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID SteamAPI_ISteamFriends_GetFriendByIndex(IntPtr _, int iFriend, int iFriendFlags)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendByIndex");
            return SteamEmulator.SteamFriends.GetFriendByIndex(iFriend, iFriendFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamFriends_GetFriendCoplayGame(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendCoplayGame");
            return SteamEmulator.SteamFriends.GetFriendCoplayGame(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendCoplayTime(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendCoplayTime");
            return SteamEmulator.SteamFriends.GetFriendCoplayTime(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendCount(IntPtr _, int iFriendFlags)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendCount");
            return SteamEmulator.SteamFriends.GetFriendCount(iFriendFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendCountFromSource(IntPtr _, ulong steamIDSource)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendCountFromSource");
            return SteamEmulator.SteamFriends.GetFriendCountFromSource(steamIDSource);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID SteamAPI_ISteamFriends_GetFriendFromSourceByIndex(IntPtr _, ulong steamIDSource, int iFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendFromSourceByIndex");
            return SteamEmulator.SteamFriends.GetFriendFromSourceByIndex(steamIDSource, iFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_GetFriendGamePlayed(IntPtr _, ulong steamIDFriend, IntPtr pFriendGameInfo)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendGamePlayed");
            return SteamEmulator.SteamFriends.GetFriendGamePlayed(steamIDFriend, pFriendGameInfo);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendMessage(IntPtr _, ulong steamIDFriend, int iMessageID, IntPtr pvData, int cubData, ref int peChatEntryType)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendMessage");
            return SteamEmulator.SteamFriends.GetFriendMessage(steamIDFriend, iMessageID, pvData, cubData, ref peChatEntryType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetFriendPersonaName(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendPersonaName");
            return SteamEmulator.SteamFriends.GetFriendPersonaName(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetFriendPersonaNameHistory(IntPtr _, ulong steamIDFriend, int iPersonaName)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendPersonaNameHistory");
            return SteamEmulator.SteamFriends.GetFriendPersonaNameHistory(steamIDFriend, iPersonaName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendPersonaState(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendPersonaState");
            return SteamEmulator.SteamFriends.GetFriendPersonaState(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendRelationship(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendRelationship");
            return SteamEmulator.SteamFriends.GetFriendRelationship(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetFriendRichPresence(IntPtr _, ulong steamIDFriend, string pchKey)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendRichPresence");
            return SteamEmulator.SteamFriends.GetFriendRichPresence(steamIDFriend, pchKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetFriendRichPresenceKeyByIndex(IntPtr _, ulong steamIDFriend, int iKey)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendRichPresenceKeyByIndex");
            return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyByIndex(steamIDFriend, iKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendRichPresenceKeyCount(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendRichPresenceKeyCount");
            return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyCount(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendsGroupCount(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendsGroupCount");
            return SteamEmulator.SteamFriends.GetFriendsGroupCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendsGroupIDByIndex(IntPtr _, int iFG)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendsGroupIDByIndex");
            return SteamEmulator.SteamFriends.GetFriendsGroupIDByIndex(iFG);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendsGroupMembersCount(IntPtr _, FriendsGroupID_t friendsGroupID)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendsGroupMembersCountint");
            return SteamEmulator.SteamFriends.GetFriendsGroupMembersCount(friendsGroupID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_GetFriendsGroupMembersList(IntPtr _, FriendsGroupID_t friendsGroupID, IntPtr pOutSteamIDMembers, int nMembersCount)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendsGroupMembersListint");
            SteamEmulator.SteamFriends.GetFriendsGroupMembersList(friendsGroupID, pOutSteamIDMembers, nMembersCount);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetFriendsGroupName(IntPtr _, FriendsGroupID_t friendsGroupID)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendsGroupNameint");
            return SteamEmulator.SteamFriends.GetFriendsGroupName(friendsGroupID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendSteamLevel(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendSteamLevel");
            return SteamEmulator.SteamFriends.GetFriendSteamLevel(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetSmallFriendAvatar(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetSmallFriendAvatar");
            return SteamEmulator.SteamFriends.GetSmallFriendAvatar(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetMediumFriendAvatar(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetMediumFriendAvatar");
            return SteamEmulator.SteamFriends.GetMediumFriendAvatar(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetLargeFriendAvatar(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetLargeFriendAvatar");
            return SteamEmulator.SteamFriends.GetLargeFriendAvatar(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetNumChatsWithUnreadPriorityMessages(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetNumChatsWithUnreadPriorityMessages");
            return SteamEmulator.SteamFriends.GetNumChatsWithUnreadPriorityMessages();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetPersonaName(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetPersonaName");
            return SteamEmulator.SteamFriends.GetPersonaName();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetPersonaState(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetPersonaState");
            return SteamEmulator.SteamFriends.GetPersonaState();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetPlayerNickname(IntPtr _, ulong steamIDPlayer)
        {
            Write($"SteamAPI_ISteamFriends_GetPlayerNickname {steamIDPlayer}");
            return SteamEmulator.SteamFriends.GetPlayerNickname(steamIDPlayer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamFriends_GetUserRestrictions(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetUserRestrictions");
            return SteamEmulator.SteamFriends.GetUserRestrictions();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_HasFriend(IntPtr _, ulong steamIDFriend, int iFriendFlags)
        {
            Write($"SteamAPI_ISteamFriends_HasFriend");
            return SteamEmulator.SteamFriends.HasFriend(steamIDFriend, iFriendFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_InviteUserToGame(IntPtr _, ulong steamIDFriend, string pchConnectString)
        {
            Write($"SteamAPI_ISteamFriends_InviteUserToGame");
            return SteamEmulator.SteamFriends.InviteUserToGame(steamIDFriend, pchConnectString);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_IsClanChatAdmin(IntPtr _, ulong steamIDClanChat, ulong steamIDUser)
        {
            Write($"SteamAPI_ISteamFriends_IsClanChatAdmin");
            return SteamEmulator.SteamFriends.IsClanChatAdmin(steamIDClanChat, steamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_IsClanChatWindowOpenInSteam(IntPtr _, ulong steamIDClanChat)
        {
            Write($"SteamAPI_ISteamFriends_IsClanChatWindowOpenInSteam");
            return SteamEmulator.SteamFriends.IsClanChatWindowOpenInSteam(steamIDClanChat);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_IsClanOfficialGameGroup(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_IsClanOfficialGameGroup");
            return SteamEmulator.SteamFriends.IsClanOfficialGameGroup(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_IsClanPublic(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_IsClanpublic static");
            return SteamEmulator.SteamFriends.IsClanPublic(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_IsFollowing(IntPtr _, ulong steamID)
        {
            Write($"SteamAPI_ISteamFriends_IsFollowing");
            return SteamEmulator.SteamFriends.IsFollowing(steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_IsUserInSource(IntPtr _, ulong steamIDUser, ulong steamIDSource)
        {
            Write($"SteamAPI_ISteamFriends_IsUserInSource {steamIDUser}");
            return SteamEmulator.SteamFriends.IsUserInSource(steamIDUser, steamIDSource);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_JoinClanChatRoom(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_JoinClanChatRoom");
            return SteamEmulator.SteamFriends.JoinClanChatRoom(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_LeaveClanChatRoom(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_LeaveClanChatRoom");
            return SteamEmulator.SteamFriends.LeaveClanChatRoom(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_OpenClanChatWindowInSteam(IntPtr _, ulong steamIDClanChat)
        {
            Write($"SteamAPI_ISteamFriends_OpenClanChatWindowInSteam");
            return SteamEmulator.SteamFriends.OpenClanChatWindowInSteam(steamIDClanChat);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_RegisterProtocolInOverlayBrowser(IntPtr _, string pchProtocol)
        {
            Write($"SteamAPI_ISteamFriends_RegisterProtocolInOverlayBrowser");
            return SteamEmulator.SteamFriends.RegisterProtocolInOverlayBrowser(pchProtocol);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_ReplyToFriendMessage(IntPtr _, ulong steamIDFriend, string pchMsgToSend)
        {
            Write($"SteamAPI_ISteamFriends_ReplyToFriendMessage");
            return SteamEmulator.SteamFriends.ReplyToFriendMessage(steamIDFriend, pchMsgToSend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_RequestClanOfficerList(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_RequestClanOfficerList");
            return SteamEmulator.SteamFriends.RequestClanOfficerList(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_RequestFriendRichPresence(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_RequestFriendRichPresence");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_RequestUserInformation(IntPtr _, ulong steamIDUser, bool bRequireNameOnly)
        {
            Write($"SteamAPI_ISteamFriends_RequestUserInformation");
            return SteamEmulator.SteamFriends.RequestUserInformation(steamIDUser, bRequireNameOnly);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_SendClanChatMessage(IntPtr _, ulong steamIDClanChat, string pchText)
        {
            Write($"SteamAPI_ISteamFriends_SendClanChatMessage");
            return SteamEmulator.SteamFriends.SendClanChatMessage(steamIDClanChat, pchText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_SetInGameVoiceSpeaking(IntPtr _, ulong steamIDUser, bool bSpeaking)
        {
            Write($"SteamAPI_ISteamFriends_SetInGameVoiceSpeaking");
            SteamEmulator.SteamFriends.SetInGameVoiceSpeaking(steamIDUser, bSpeaking);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_SetListenForFriendsMessages(IntPtr _, bool bInterceptEnabled)
        {
            Write($"SteamAPI_ISteamFriends_SetListenForFriendsMessages");
            return SteamEmulator.SteamFriends.SetListenForFriendsMessages(bInterceptEnabled);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_SetPersonaName(IntPtr _, string pchPersonaName)
        {
            Write($"SteamAPI_ISteamFriends_SetPersonaName");
            return SteamEmulator.SteamFriends.SetPersonaName(pchPersonaName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_SetPlayedWith(IntPtr _, ulong steamIDUserPlayedWith)
        {
            Write($"SteamAPI_ISteamFriends_SetPlayedWith");
            SteamEmulator.SteamFriends.SetPlayedWith(steamIDUserPlayedWith);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_SetRichPresence(IntPtr _, string pchKey, string pchValue)
        {
            Write($"SteamAPI_ISteamFriends_SetRichPresence");
            return SteamEmulator.SteamFriends.SetRichPresence(pchKey, pchValue);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("SteamAPI_SteamController", msg);
        }
    }
}

