using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SKYNET.Helper;
using SKYNET.Interface;
using SKYNET.Types;
using Steamworks;
using Steamworks.Data;

namespace SKYNET.Managers
{
    public class Steam_Friends : SteamInterface//, ISteamFriends
    {
        public static string ISteamFriends004 = "ISteamFriends004";
        public static string ISteamFriends005 = "ISteamFriends005";
        public static string ISteamFriends006 = "ISteamFriends006";
        public static string ISteamFriends007 = "ISteamFriends007";
        public static string ISteamFriends008 = "ISteamFriends008";
        public static string ISteamFriends009 = "ISteamFriends009";
        public static string ISteamFriends010 = "ISteamFriends010";
        public static string ISteamFriends011 = "ISteamFriends011";
        public static string ISteamFriends012 = "ISteamFriends012";
        public static string ISteamFriends013 = "ISteamFriends013";
        public static string ISteamFriends014 = "ISteamFriends014";
        public static string ISteamFriends015 = "ISteamFriends015";
        public static string ISteamFriends016 = "ISteamFriends016";

        public static List<Friend> Friends;
        public static List<CSteamID> Users;

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ActivateGameOverlay([MarshalAs(UnmanagedType.LPStr)] string friendsGroupID)
        {
            PRINT_DEBUG($"Steam_Friends.ActivateGameOverlay {friendsGroupID}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ActivateGameOverlayInviteDialog(CSteamID steamIDLobby)
        {
            PRINT_DEBUG($"Steam_Friends.ActivateGameOverlayInviteDialog {steamIDLobby}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ActivateGameOverlayInviteDialogConnectString([MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
        {
            PRINT_DEBUG($"Steam_Friends.ActivateGameOverlayInviteDialogConnectString {pchConnectString}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ActivateGameOverlayRemotePlayTogetherInviteDialog(CSteamID steamIDLobby)
        {
            PRINT_DEBUG($"Steam_Friends.ActivateGameOverlayRemotePlayTogetherInviteDialog {steamIDLobby}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ActivateGameOverlayToStore(AppId_t nAppID, EOverlayToStoreFlag eFlag)
        {
            PRINT_DEBUG($"Steam_Friends.ActivateGameOverlayToStore {nAppID} {eFlag}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ActivateGameOverlayToUser([MarshalAs(UnmanagedType.LPStr)] string friendsGroupID, CSteamID steamID)
        {
            PRINT_DEBUG($"Steam_Friends.ActivateGameOverlayToUser {friendsGroupID} {steamID}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ActivateGameOverlayToWebPage([MarshalAs(UnmanagedType.LPStr)] string pchURL, EActivateGameOverlayToWebPageMode eMode = EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default)
        {
            PRINT_DEBUG($"Steam_Friends.ActivateGameOverlayToWebPage {pchURL}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void ClearRichPresence()
        {
            PRINT_DEBUG($"Steam_Friends.ClearRichPresence");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool CloseClanChatWindowInSteam(CSteamID steamIDClanChat)
        {
            PRINT_DEBUG($"Steam_Friends.CloseClanChatWindowInSteam {steamIDClanChat}");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t DownloadClanActivityCounts(CSteamID[] psteamIDClans, int cClansToRequest)
        {
            PRINT_DEBUG($"Steam_Friends.DownloadClanActivityCounts {cClansToRequest}");
            return (SteamAPICall_t)0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t EnumerateFollowingList(uint unStartIndex)
        {
            PRINT_DEBUG($"Steam_Friends.EnumerateFollowingList {unStartIndex}");
            return (SteamAPICall_t)0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID GetChatMemberByIndex(CSteamID steamIDClan, int iUser)
        {
            PRINT_DEBUG($"Steam_Friends.GetChatMemberByIndex {steamIDClan}");
            return CSteamID.Nil;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool GetClanActivityCounts(CSteamID steamIDClan, int pnOnline, int pnInGame, int pnChatting)
        {
            PRINT_DEBUG($"Steam_Friends.ActivateGameOverlay {steamIDClan}");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID GetClanByIndex(int iClan)
        {
            PRINT_DEBUG($"Steam_Friends.GetClanByIndex {iClan}");
            return CSteamID.Nil;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetClanChatMemberCount(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.GetClanChatMemberCount {steamIDClan}");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetClanChatMessage(CSteamID steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, EChatEntryType peChatEntryType, CSteamID[] psteamidChatter)
        {
            PRINT_DEBUG($"Steam_Friends.GetClanChatMessage {steamIDClanChat}");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetClanCount()
        {
            PRINT_DEBUG($"Steam_Friends.GetClanCount");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string GetClanName(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.GetClanName {steamIDClan}");
            return "";
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID GetClanOfficerByIndex(CSteamID steamIDClan, int iOfficer)
        {
            PRINT_DEBUG($"Steam_Friends.GetClanOfficerByIndex {steamIDClan}");
            return CSteamID.Nil;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetClanOfficerCount(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.GetClanOfficerCount {steamIDClan}");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID GetClanOwner(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.GetClanOwner {steamIDClan}");
            return CSteamID.Nil;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string GetClanTag(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.GetClanTag {steamIDClan}");
            return "";
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID GetCoplayFriend(int iCoplayFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetCoplayFriend {iCoplayFriend}");
            return CSteamID.Nil;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetCoplayFriendCount()
        {
            PRINT_DEBUG($"Steam_Friends.GetCoplayFriendCount");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t GetFollowerCount(CSteamID steamID)
        {
            PRINT_DEBUG($"Steam_Friends.GetFollowerCount {steamID}");
            return (SteamAPICall_t)0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID GetFriendByIndex(int iFriend, int iFriendFlags)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendByIndex {iFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)iFriend);
            if (friend == null)
            {
                return CSteamID.Nil;
            }
            return new CSteamID(friend.SteamId);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static AppId_t GetFriendCoplayGame(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendCoplayGame {steamIDFriend}");
            return (AppId_t)0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetFriendCoplayTime(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendCoplayTime {steamIDFriend}");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetFriendCount(int iFriendFlags)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendCount {iFriendFlags}");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetFriendCountFromSource(CSteamID steamIDSource)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendCountFromSource {steamIDSource}");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static CSteamID GetFriendFromSourceByIndex(CSteamID steamIDSource, int iFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendFromSourceByIndex {steamIDSource} {iFriend}");
            return CSteamID.Nil;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool GetFriendGamePlayed(CSteamID steamIDFriend, out FriendGameInfo_t pFriendGameInfo)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendGamePlayed {(uint)steamIDFriend}");
            pFriendGameInfo = new FriendGameInfo_t();

            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
            if (friend == null)
            {
                pFriendGameInfo.GameID = 0;
                pFriendGameInfo.GameIP = 0;
                pFriendGameInfo.GamePort = 0;
                return false;
            }
            pFriendGameInfo.GameID = friend.GameId;
            pFriendGameInfo.GameIP = 0;
            pFriendGameInfo.GamePort = 0;
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetFriendMessage(CSteamID steamIDFriend, int iMessageID, IntPtr pvData, int cubData, EChatEntryType peChatEntryType)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendMessage {steamIDFriend}");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string GetFriendPersonaName(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendPersonaName {(uint)steamIDFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
            if (friend == null)
            {
                return "";
            }
            return friend.PersonaName;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string GetFriendPersonaNameHistory(CSteamID steamIDFriend, int iPersonaName)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendPersonaNameHistory {steamIDFriend}");
            return "SKYNET";
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EPersonaState GetFriendPersonaState(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendPersonaState {steamIDFriend}");
            return Users.Find(f => f == steamIDFriend) == null ? EPersonaState.k_EPersonaStateOffline : EPersonaState.k_EPersonaStateOnline;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EFriendRelationship GetFriendRelationship(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendRelationship {steamIDFriend}");
            return EFriendRelationship.k_EFriendRelationshipNone;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string GetFriendRichPresence(CSteamID steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchKey)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendRichPresence {steamIDFriend} {pchKey}");
            return "";
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string GetFriendRichPresenceKeyByIndex(CSteamID steamIDFriend, int iKey)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendRichPresenceKeyByIndex {steamIDFriend} {iKey}");
            return "";
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetFriendRichPresenceKeyCount(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendRichPresenceKeyCount {steamIDFriend}");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetFriendsGroupCount()
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendsGroupCount");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static FriendsGroupID_t GetFriendsGroupIDByIndex(int iFG)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendsGroupIDByIndex {iFG}");
            return (FriendsGroupID_t)0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetFriendsGroupMembersCount(FriendsGroupID_t friendsGroupID)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendsGroupMembersCount {friendsGroupID}");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void GetFriendsGroupMembersList(FriendsGroupID_t friendsGroupID, CSteamID[] pOutSteamIDMembers, int nMembersCount)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendsGroupMembersList {friendsGroupID}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string GetFriendsGroupName(FriendsGroupID_t friendsGroupID)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendsGroupName {friendsGroupID}");
            return "";
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetFriendSteamLevel(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendSteamLevel {steamIDFriend}");
            return 100;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetSmallFriendAvatar(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetSmallFriendAvatar {steamIDFriend}");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetMediumFriendAvatar(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetMediumFriendAvatar {steamIDFriend}");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetLargeFriendAvatar(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetLargeFriendAvatar {steamIDFriend}");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetNumChatsWithUnreadPriorityMessages()
        {
            PRINT_DEBUG($"Steam_Friends.GetNumChatsWithUnreadPriorityMessages");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string GetPersonaName()
        {
            PRINT_DEBUG($"Steam_Friends.GetPersonaName");
            return SteamClient.PersonaName;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EPersonaState GetPersonaState()
        {
            PRINT_DEBUG($"Steam_Friends.GetPersonaState");
            return EPersonaState.k_EPersonaStateOnline;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string GetPlayerNickname(CSteamID steamIDPlayer)
        {
            PRINT_DEBUG($"Steam_Friends.GetPlayerNickname {steamIDPlayer}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDPlayer);
            if (friend == null)
            {
                return "";
            }
            return friend.PersonaName;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint GetUserRestrictions()
        {
            PRINT_DEBUG($"Steam_Friends.GetUserRestrictions");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool HasFriend(CSteamID steamIDFriend, int iFriendFlags)
        {
            PRINT_DEBUG($"Steam_Friends.HasFriend {steamIDFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
            return friend != null;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool InviteUserToGame(CSteamID steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
        {
            PRINT_DEBUG($"Steam_Friends.InviteUserToGame {steamIDFriend} {pchConnectString}");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool IsClanChatAdmin(CSteamID steamIDClanChat, CSteamID steamIDUser)
        {
            PRINT_DEBUG($"Steam_Friends.IsClanChatAdmin {steamIDClanChat}");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool IsClanChatWindowOpenInSteam(CSteamID steamIDClanChat)
        {
            PRINT_DEBUG($"Steam_Friends.IsClanChatWindowOpenInSteam {steamIDClanChat}");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool IsClanOfficialGameGroup(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.IsClanOfficialGameGroup {steamIDClan}");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool IsClanPublic(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.IsClanpublic static {steamIDClan}");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t IsFollowing(CSteamID steamID)
        {
            PRINT_DEBUG($"Steam_Friends.IsFollowing {steamID}");
            return (SteamAPICall_t)0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool IsUserInSource(CSteamID steamIDUser, CSteamID steamIDSource)
        {
            PRINT_DEBUG($"Steam_Friends.IsUserInSource {steamIDUser}");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t JoinClanChatRoom(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.JoinClanChatRoom {steamIDClan}");
            return (SteamAPICall_t)0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool LeaveClanChatRoom(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.LeaveClanChatRoom {steamIDClan}");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool OpenClanChatWindowInSteam(CSteamID steamIDClanChat)
        {
            PRINT_DEBUG($"Steam_Friends.OpenClanChatWindowInSteam {steamIDClanChat}");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool RegisterProtocolInOverlayBrowser([MarshalAs(UnmanagedType.LPStr)] string pchProtocol)
        {
            PRINT_DEBUG($"Steam_Friends.RegisterProtocolInOverlayBrowser {pchProtocol}");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool ReplyToFriendMessage(CSteamID steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchMsgToSend)
        {
            PRINT_DEBUG($"Steam_Friends.ReplyToFriendMessage {steamIDFriend} {pchMsgToSend}");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t RequestClanOfficerList(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.RequestClanOfficerList {steamIDClan}");
            return (SteamAPICall_t)0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void RequestFriendRichPresence(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.RequestFriendRichPresence {steamIDFriend}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool RequestUserInformation(CSteamID steamIDUser, bool bRequireNameOnly)
        {
            PRINT_DEBUG($"Steam_Friends.RequestUserInformation {steamIDUser}");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SendClanChatMessage(CSteamID steamIDClanChat, [MarshalAs(UnmanagedType.LPStr)] string pchText)
        {
            PRINT_DEBUG($"Steam_Friends.SendClanChatMessage {steamIDClanChat} {pchText}");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SetInGameVoiceSpeaking(CSteamID steamIDUser, bool bSpeaking)
        {
            PRINT_DEBUG($"Steam_Friends.SetInGameVoiceSpeaking {steamIDUser}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SetListenForFriendsMessages(bool bInterceptEnabled)
        {
            PRINT_DEBUG($"Steam_Friends.SetListenForFriendsMessages {bInterceptEnabled}");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SetPersonaName([MarshalAs(UnmanagedType.LPStr)] string pchPersonaName)
        {
            PRINT_DEBUG($"Steam_Friends.SetPersonaName {pchPersonaName}");
            SteamClient.SetPersonaName(pchPersonaName);
            return (SteamAPICall_t)1;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SetPlayedWith(CSteamID steamIDUserPlayedWith)
        {
            PRINT_DEBUG($"Steam_Friends.SetPlayedWith {steamIDUserPlayedWith}");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SetRichPresence([MarshalAs(UnmanagedType.LPStr)] string pchKey, [MarshalAs(UnmanagedType.LPStr)] string pchValue)
        {
            PRINT_DEBUG($"Steam_Friends.SetRichPresence {pchKey} {pchValue}");
            return true;
        }
    }
}
