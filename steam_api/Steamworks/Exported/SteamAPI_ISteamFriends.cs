using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using SteamAPICall_t = System.UInt64;
using FriendsGroupID_t = System.UInt16;
using System.Threading.Tasks;
using SKYNET.Steamworks.Implementation;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamFriends
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlay(IntPtr _, string friendsGroupID)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlay");
            SteamFriends.Instance.ActivateGameOverlay(friendsGroupID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialog(IntPtr _, ulong steamIDLobby)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialog");
            SteamFriends.Instance.ActivateGameOverlayInviteDialog(steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialogConnectString(IntPtr _, string pchConnectString)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialogConnectString");
            SteamFriends.Instance.ActivateGameOverlayInviteDialogConnectString(pchConnectString);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayRemotePlayTogetherInviteDialog(IntPtr _, ulong steamIDLobby)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayRemotePlayTogetherInviteDialog");
            SteamFriends.Instance.ActivateGameOverlayRemotePlayTogetherInviteDialog(steamIDLobby);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayToStore(IntPtr _, uint nAppID, int eFlag)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayToStore");
            SteamFriends.Instance.ActivateGameOverlayToStore(nAppID, eFlag);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayToUser(IntPtr _, string friendsGroupID, ulong steamID)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayToUser");
            SteamFriends.Instance.ActivateGameOverlayToUser(friendsGroupID, steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ActivateGameOverlayToWebPage(IntPtr _, string pchURL, int eMode)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlayToWebPage");
            SteamFriends.Instance.ActivateGameOverlayToWebPage(pchURL, eMode);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_ClearRichPresence(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_ClearRichPresence");
            SteamFriends.Instance.ClearRichPresence();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_CloseClanChatWindowInSteam(IntPtr _, ulong steamIDClanChat)
        {
            Write($"SteamAPI_ISteamFriends_CloseClanChatWindowInSteam");
            return SteamFriends.Instance.CloseClanChatWindowInSteam(steamIDClanChat);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_DownloadClanActivityCounts(IntPtr _, IntPtr psteamIDClans, int cClansToRequest)
        {
            Write($"SteamAPI_ISteamFriends_DownloadClanActivityCounts {cClansToRequest}");
            return SteamFriends.Instance.DownloadClanActivityCounts(psteamIDClans, cClansToRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_EnumerateFollowingList(IntPtr _, uint unStartIndex)
        {
            Write($"SteamAPI_ISteamFriends_EnumerateFollowingList");
            return SteamFriends.Instance.EnumerateFollowingList(unStartIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamFriends_GetChatMemberByIndex(IntPtr _, ulong steamIDClan, int iUser)
        {
            Write($"SteamAPI_ISteamFriends_GetChatMemberByIndex");
            return SteamFriends.Instance.GetChatMemberByIndex(steamIDClan, iUser).SteamID;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_GetClanActivityCounts(IntPtr _, ulong steamIDClan, ref int pnOnline, ref int pnInGame, ref int pnChatting)
        {
            Write($"SteamAPI_ISteamFriends_ActivateGameOverlay");
            return SteamFriends.Instance.GetClanActivityCounts(steamIDClan, ref pnOnline, ref pnInGame, ref pnChatting);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamFriends_GetClanByIndex(IntPtr _, int iClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanByIndex");
            return SteamFriends.Instance.GetClanByIndex(iClan).SteamID;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetClanChatMemberCount(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanChatMemberCount");
            return SteamFriends.Instance.GetClanChatMemberCount(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetClanChatMessage(IntPtr _, ulong steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, int peChatEntryType, ref ulong[] psteamidChatter)
        {
            Write($"SteamAPI_ISteamFriends_GetClanChatMessage");
            return SteamFriends.Instance.GetClanChatMessage(steamIDClanChat, iMessage, prgchText, cchTextMax, peChatEntryType, ref psteamidChatter);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetClanCount(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetClanCount");
            return SteamFriends.Instance.GetClanCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetClanName(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanName");
            return SteamFriends.Instance.GetClanName(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamFriends_GetClanOfficerByIndex(IntPtr _, ulong steamIDClan, int iOfficer)
        {
            Write($"SteamAPI_ISteamFriends_GetClanOfficerByIndex");
            return SteamFriends.Instance.GetClanOfficerByIndex(steamIDClan, iOfficer).SteamID;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetClanOfficerCount(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanOfficerCount");
            return SteamFriends.Instance.GetClanOfficerCount(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamFriends_GetClanOwner(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanOwner");
            return SteamFriends.Instance.GetClanOwner(steamIDClan).SteamID;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetClanTag(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_GetClanTag");
            return SteamFriends.Instance.GetClanTag(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamFriends_GetCoplayFriend(IntPtr _, int iCoplayFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetCoplayFriend");
            return SteamFriends.Instance.GetCoplayFriend(iCoplayFriend).SteamID;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetCoplayFriendCount(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetCoplayFriendCount");
            return SteamFriends.Instance.GetCoplayFriendCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_GetFollowerCount(IntPtr _, ulong steamID)
        {
            Write($"SteamAPI_ISteamFriends_GetFollowerCount");
            return SteamFriends.Instance.GetFollowerCount(steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamFriends_GetFriendByIndex(IntPtr _, int iFriend, int iFriendFlags)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendByIndex");
            return SteamFriends.Instance.GetFriendByIndex(iFriend, iFriendFlags).SteamID;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamFriends_GetFriendCoplayGame(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendCoplayGame");
            return SteamFriends.Instance.GetFriendCoplayGame(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendCoplayTime(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendCoplayTime");
            return SteamFriends.Instance.GetFriendCoplayTime(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendCount(IntPtr _, int iFriendFlags)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendCount");
            return SteamFriends.Instance.GetFriendCount(iFriendFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendCountFromSource(IntPtr _, ulong steamIDSource)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendCountFromSource");
            return SteamFriends.Instance.GetFriendCountFromSource(steamIDSource);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamFriends_GetFriendFromSourceByIndex(IntPtr _, ulong steamIDSource, int iFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendFromSourceByIndex");
            return SteamFriends.Instance.GetFriendFromSourceByIndex(steamIDSource, iFriend).SteamID;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_GetFriendGamePlayed(IntPtr _, ulong steamIDFriend, IntPtr pFriendGameInfo)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendGamePlayed");
            return SteamFriends.Instance.GetFriendGamePlayed(steamIDFriend, pFriendGameInfo);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendMessage(IntPtr _, ulong steamIDFriend, int iMessageID, IntPtr pvData, int cubData, ref int peChatEntryType)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendMessage");
            return SteamFriends.Instance.GetFriendMessage(steamIDFriend, iMessageID, pvData, cubData, peChatEntryType);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetFriendPersonaName(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendPersonaName");
            return SteamFriends.Instance.GetFriendPersonaName(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetFriendPersonaNameHistory(IntPtr _, ulong steamIDFriend, int iPersonaName)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendPersonaNameHistory");
            return SteamFriends.Instance.GetFriendPersonaNameHistory(steamIDFriend, iPersonaName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendPersonaState(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendPersonaState");
            return SteamFriends.Instance.GetFriendPersonaState(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendRelationship(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendRelationship");
            return SteamFriends.Instance.GetFriendRelationship(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetFriendRichPresence(IntPtr _, ulong steamIDFriend, string pchKey)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendRichPresence");
            return SteamFriends.Instance.GetFriendRichPresence(steamIDFriend, pchKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetFriendRichPresenceKeyByIndex(IntPtr _, ulong steamIDFriend, int iKey)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendRichPresenceKeyByIndex");
            return SteamFriends.Instance.GetFriendRichPresenceKeyByIndex(steamIDFriend, iKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendRichPresenceKeyCount(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendRichPresenceKeyCount");
            return SteamFriends.Instance.GetFriendRichPresenceKeyCount(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendsGroupCount(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendsGroupCount");
            return SteamFriends.Instance.GetFriendsGroupCount();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendsGroupIDByIndex(IntPtr _, int iFG)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendsGroupIDByIndex");
            return SteamFriends.Instance.GetFriendsGroupIDByIndex(iFG);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendsGroupMembersCount(IntPtr _, FriendsGroupID_t friendsGroupID)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendsGroupMembersCountint");
            return SteamFriends.Instance.GetFriendsGroupMembersCount(friendsGroupID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_GetFriendsGroupMembersList(IntPtr _, FriendsGroupID_t friendsGroupID, IntPtr pOutSteamIDMembers, int nMembersCount)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendsGroupMembersListint");
            SteamFriends.Instance.GetFriendsGroupMembersList(friendsGroupID, pOutSteamIDMembers, nMembersCount);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetFriendsGroupName(IntPtr _, FriendsGroupID_t friendsGroupID)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendsGroupNameint");
            return SteamFriends.Instance.GetFriendsGroupName(friendsGroupID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetFriendSteamLevel(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetFriendSteamLevel");
            return SteamFriends.Instance.GetFriendSteamLevel(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetSmallFriendAvatar(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetSmallFriendAvatar");
            return SteamFriends.Instance.GetSmallFriendAvatar(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetMediumFriendAvatar(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetMediumFriendAvatar");
            return SteamFriends.Instance.GetMediumFriendAvatar(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetLargeFriendAvatar(IntPtr _, ulong steamIDFriend)
        {
            Write($"SteamAPI_ISteamFriends_GetLargeFriendAvatar");
            return SteamFriends.Instance.GetLargeFriendAvatar(steamIDFriend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetNumChatsWithUnreadPriorityMessages(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetNumChatsWithUnreadPriorityMessages");
            return SteamFriends.Instance.GetNumChatsWithUnreadPriorityMessages();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetPersonaName(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetPersonaName");
            return SteamFriends.Instance.GetPersonaName();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamFriends_GetPersonaState(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetPersonaState");
            return SteamFriends.Instance.GetPersonaState();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamFriends_GetPlayerNickname(IntPtr _, ulong steamIDPlayer)
        {
            Write($"SteamAPI_ISteamFriends_GetPlayerNickname {steamIDPlayer}");
            return SteamFriends.Instance.GetPlayerNickname(steamIDPlayer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamFriends_GetUserRestrictions(IntPtr _)
        {
            Write($"SteamAPI_ISteamFriends_GetUserRestrictions");
            return SteamFriends.Instance.GetUserRestrictions();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_HasFriend(IntPtr _, ulong steamIDFriend, int iFriendFlags)
        {
            Write($"SteamAPI_ISteamFriends_HasFriend");
            return SteamFriends.Instance.HasFriend(steamIDFriend, iFriendFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_InviteUserToGame(IntPtr _, ulong steamIDFriend, string pchConnectString)
        {
            Write($"SteamAPI_ISteamFriends_InviteUserToGame");
            return SteamFriends.Instance.InviteUserToGame(steamIDFriend, pchConnectString);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_IsClanChatAdmin(IntPtr _, ulong steamIDClanChat, ulong steamIDUser)
        {
            Write($"SteamAPI_ISteamFriends_IsClanChatAdmin");
            return SteamFriends.Instance.IsClanChatAdmin(steamIDClanChat, steamIDUser);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_IsClanChatWindowOpenInSteam(IntPtr _, ulong steamIDClanChat)
        {
            Write($"SteamAPI_ISteamFriends_IsClanChatWindowOpenInSteam");
            return SteamFriends.Instance.IsClanChatWindowOpenInSteam(steamIDClanChat);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_IsClanOfficialGameGroup(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_IsClanOfficialGameGroup");
            return SteamFriends.Instance.IsClanOfficialGameGroup(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_IsClanPublic(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_IsClanpublic static");
            return SteamFriends.Instance.IsClanPublic(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_IsFollowing(IntPtr _, ulong steamID)
        {
            Write($"SteamAPI_ISteamFriends_IsFollowing");
            return SteamFriends.Instance.IsFollowing(steamID);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_IsUserInSource(IntPtr _, ulong steamIDUser, ulong steamIDSource)
        {
            Write($"SteamAPI_ISteamFriends_IsUserInSource {steamIDUser}");
            return SteamFriends.Instance.IsUserInSource(steamIDUser, steamIDSource);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_JoinClanChatRoom(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_JoinClanChatRoom");
            return SteamFriends.Instance.JoinClanChatRoom(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_LeaveClanChatRoom(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_LeaveClanChatRoom");
            return SteamFriends.Instance.LeaveClanChatRoom(steamIDClan);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_OpenClanChatWindowInSteam(IntPtr _, ulong steamIDClanChat)
        {
            Write($"SteamAPI_ISteamFriends_OpenClanChatWindowInSteam");
            return SteamFriends.Instance.OpenClanChatWindowInSteam(steamIDClanChat);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_RegisterProtocolInOverlayBrowser(IntPtr _, string pchProtocol)
        {
            Write($"SteamAPI_ISteamFriends_RegisterProtocolInOverlayBrowser");
            return SteamFriends.Instance.RegisterProtocolInOverlayBrowser(pchProtocol);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_ReplyToFriendMessage(IntPtr _, ulong steamIDFriend, string pchMsgToSend)
        {
            Write($"SteamAPI_ISteamFriends_ReplyToFriendMessage");
            return SteamFriends.Instance.ReplyToFriendMessage(steamIDFriend, pchMsgToSend);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_RequestClanOfficerList(IntPtr _, ulong steamIDClan)
        {
            Write($"SteamAPI_ISteamFriends_RequestClanOfficerList");
            return SteamFriends.Instance.RequestClanOfficerList(steamIDClan);
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
            return SteamFriends.Instance.RequestUserInformation(steamIDUser, bRequireNameOnly);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_SendClanChatMessage(IntPtr _, ulong steamIDClanChat, string pchText)
        {
            Write($"SteamAPI_ISteamFriends_SendClanChatMessage");
            return SteamFriends.Instance.SendClanChatMessage(steamIDClanChat, pchText);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_SetInGameVoiceSpeaking(IntPtr _, ulong steamIDUser, bool bSpeaking)
        {
            Write($"SteamAPI_ISteamFriends_SetInGameVoiceSpeaking");
            SteamFriends.Instance.SetInGameVoiceSpeaking(steamIDUser, bSpeaking);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_SetListenForFriendsMessages(IntPtr _, bool bInterceptEnabled)
        {
            Write($"SteamAPI_ISteamFriends_SetListenForFriendsMessages");
            return SteamFriends.Instance.SetListenForFriendsMessages(bInterceptEnabled);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamFriends_SetPersonaName(IntPtr _, string pchPersonaName)
        {
            Write($"SteamAPI_ISteamFriends_SetPersonaName");
            return SteamFriends.Instance.SetPersonaName(pchPersonaName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamFriends_SetPlayedWith(IntPtr _, ulong steamIDUserPlayedWith)
        {
            Write($"SteamAPI_ISteamFriends_SetPlayedWith");
            SteamFriends.Instance.SetPlayedWith(steamIDUserPlayedWith);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamFriends_SetRichPresence(IntPtr _, string pchKey, string pchValue)
        {
            Write($"SteamAPI_ISteamFriends_SetRichPresence");
            return SteamFriends.Instance.SetRichPresence(pchKey, pchValue);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

