using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using SKYNET.Types;
using Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamFriends : ISteamInterface
    {
        public List<Friend> Friends;
        public List<ulong> Users;

        public SteamFriends()
        {
            InterfaceVersion = "SteamFriends";
            Friends = new List<Friend>();
            Users = new List<ulong>();
        }

        public void ActivateGameOverlay([MarshalAs(UnmanagedType.LPStr)] string friendsGroupID)
        {
            Write($"ActivateGameOverlay {friendsGroupID}");
        }

        public void ActivateGameOverlayInviteDialog(ulong steamIDLobby)
        {
            Write($"ActivateGameOverlayInviteDialog {steamIDLobby}");
        }

        public void ActivateGameOverlayInviteDialogConnectString([MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
        {
            Write($"ActivateGameOverlayInviteDialogConnectString {pchConnectString}");
        }

        public void ActivateGameOverlayRemotePlayTogetherInviteDialog(ulong steamIDLobby)
        {
            Write($"ActivateGameOverlayRemotePlayTogetherInviteDialog {steamIDLobby}");
        }

        public void ActivateGameOverlayToStore(uint nAppID, EOverlayToStoreFlag eFlag)
        {
            Write($"ActivateGameOverlayToStore {nAppID} {eFlag}");
        }


        public void ActivateGameOverlayToUser([MarshalAs(UnmanagedType.LPStr)] string friendsGroupID, ulong steamID)
        {
            Write($"ActivateGameOverlayToUser {friendsGroupID} {steamID}");
        }


        public void ActivateGameOverlayToWebPage([MarshalAs(UnmanagedType.LPStr)] string pchURL, int eMode)
        {
            Write($"ActivateGameOverlayToWebPage {pchURL}");
        }


        public void ClearRichPresence(IntPtr _)
        {
            Write($"ClearRichPresence");
        }


        public bool CloseClanChatWindowInSteam(ulong steamIDClanChat)
        {
            Write($"CloseClanChatWindowInSteam {steamIDClanChat}");
            return true;
        }


        public ulong DownloadClanActivityCounts(ulong psteamIDClans, int cClansToRequest)
        {
            Write($"DownloadClanActivityCounts {cClansToRequest}");
            return (ulong)0;
        }


        public ulong EnumerateFollowingList(uint unStartIndex)
        {
            Write($"EnumerateFollowingList {unStartIndex}");
            return (ulong)0;
        }


        public ulong GetChatMemberByIndex(ulong steamIDClan, int iUser)
        {
            Write($"GetChatMemberByIndex {steamIDClan}");
            return 0;
        }


        public bool GetClanActivityCounts(ulong steamIDClan, int pnOnline, int pnInGame, int pnChatting)
        {
            Write($"ActivateGameOverlay {steamIDClan}");
            return false;
        }


        public ulong GetClanByIndex(int iClan)
        {
            Write($"GetClanByIndex {iClan}");
            return 0;
        }


        public int GetClanChatMemberCount(ulong steamIDClan)
        {
            Write($"GetClanChatMemberCount {steamIDClan}");
            return 0;
        }


        public int GetClanChatMessage(ulong steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, int peChatEntryType, ulong psteamidChatter)
        {
            Write($"GetClanChatMessage {steamIDClanChat}");
            return 0;
        }


        public int GetClanCount(IntPtr _)
        {
            Write($"GetClanCount");
            return 0;
        }


        public string GetClanName(ulong steamIDClan)
        {
            Write($"GetClanName {steamIDClan}");
            return "";
        }


        public ulong GetClanOfficerByIndex(ulong steamIDClan, int iOfficer)
        {
            Write($"GetClanOfficerByIndex {steamIDClan}");
            return 0;
        }


        public int GetClanOfficerCount(ulong steamIDClan)
        {
            Write($"GetClanOfficerCount {steamIDClan}");
            return 0;
        }


        public ulong GetClanOwner(ulong steamIDClan)
        {
            Write($"GetClanOwner {steamIDClan}");
            return 0;
        }


        public string GetClanTag(ulong steamIDClan)
        {
            Write($"GetClanTag {steamIDClan}");
            return "";
        }


        public ulong GetCoplayFriend(int iCoplayFriend)
        {
            Write($"GetCoplayFriend {iCoplayFriend}");
            return 0;
        }


        public int GetCoplayFriendCount(IntPtr _)
        {
            Write($"GetCoplayFriendCount");
            return 0;
        }


        public ulong GetFollowerCount(ulong steamID)
        {
            Write($"GetFollowerCount {steamID}");
            return (ulong)0;
        }


        public ulong GetFriendByIndex(int iFriend, int iFriendFlags)
        {
            Write($"GetFriendByIndex {iFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)iFriend);
            if (friend == null)
            {
                return 0;
            }
            return friend.SteamId;
        }


        public uint GetFriendCoplayGame(ulong steamIDFriend)
        {
            Write($"GetFriendCoplayGame {steamIDFriend}");
            return (uint)0;
        }


        public int GetFriendCoplayTime(ulong steamIDFriend)
        {
            Write($"GetFriendCoplayTime {steamIDFriend}");
            return 0;
        }


        public int GetFriendCount(int iFriendFlags)
        {
            Write($"GetFriendCount {iFriendFlags}");
            return 0;
        }


        public int GetFriendCountFromSource(ulong steamIDSource)
        {
            Write($"GetFriendCountFromSource {steamIDSource}");
            return 0;
        }


        public ulong GetFriendFromSourceByIndex(ulong steamIDSource, int iFriend)
        {
            Write($"GetFriendFromSourceByIndex {steamIDSource} {iFriend}");
            return 0;
        }


        public bool GetFriendGamePlayed(ulong steamIDFriend, IntPtr ptrFriendGameInfo)
        {
            Write($"GetFriendGamePlayed");

            FriendGameInfo_t pFriendGameInfo = Marshal.PtrToStructure<FriendGameInfo_t>(ptrFriendGameInfo);
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


        public int GetFriendMessage(ulong steamIDFriend, int iMessageID, IntPtr pvData, int cubData, EChatEntryType peChatEntryType)
        {
            Write($"GetFriendMessage {steamIDFriend}");
            return 0;
        }


        public string GetFriendPersonaName(ulong steamIDFriend)
        {
            Write($"GetFriendPersonaName {(uint)steamIDFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
            if (friend == null)
            {
                return "";
            }
            return friend.PersonaName;
        }


        public string GetFriendPersonaNameHistory(ulong steamIDFriend, int iPersonaName)
        {
            Write($"GetFriendPersonaNameHistory {steamIDFriend}");
            return "SKYNET";
        }


        public EPersonaState GetFriendPersonaState(ulong steamIDFriend)
        {
            Write($"GetFriendPersonaState {steamIDFriend}");
            return Users.Find(f => f == steamIDFriend) == null ? EPersonaState.k_EPersonaStateOffline : EPersonaState.k_EPersonaStateOnline;
        }


        public EFriendRelationship GetFriendRelationship(ulong steamIDFriend)
        {
            Write($"GetFriendRelationship {steamIDFriend}");
            return EFriendRelationship.k_EFriendRelationshipNone;
        }


        public string GetFriendRichPresence(ulong steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchKey)
        {
            Write($"GetFriendRichPresence {steamIDFriend} {pchKey}");
            return "";
        }


        public string GetFriendRichPresenceKeyByIndex(ulong steamIDFriend, int iKey)
        {
            Write($"GetFriendRichPresenceKeyByIndex {steamIDFriend} {iKey}");
            return "";
        }


        public int GetFriendRichPresenceKeyCount(ulong steamIDFriend)
        {
            Write($"GetFriendRichPresenceKeyCount {steamIDFriend}");
            return 0;
        }


        public int GetFriendsGroupCount(IntPtr _)
        {
            Write($"GetFriendsGroupCount");
            return 0;
        }


        public int GetFriendsGroupIDByIndex(int iFG)
        {
            Write($"GetFriendsGroupIDByIndex {iFG}");
            return (int)0;
        }


        public int GetFriendsGroupMembersCount(int friendsGroupID)
        {
            Write($"GetFriendsGroupMembersCount {friendsGroupID}");
            return 0;
        }


        public void GetFriendsGroupMembersList(int friendsGroupID, ulong pOutSteamIDMembers, int nMembersCount)
        {
            Write($"GetFriendsGroupMembersList {friendsGroupID}");
        }


        public string GetFriendsGroupName(int friendsGroupID)
        {
            Write($"GetFriendsGroupName {friendsGroupID}");
            return "";
        }


        public int GetFriendSteamLevel(ulong steamIDFriend)
        {
            Write($"GetFriendSteamLevel {steamIDFriend}");
            return 100;
        }


        public int GetSmallFriendAvatar(ulong steamIDFriend)
        {
            Write($"GetSmallFriendAvatar {steamIDFriend}");
            return 0;
        }


        public int GetMediumFriendAvatar(ulong steamIDFriend)
        {
            Write($"GetMediumFriendAvatar {steamIDFriend}");
            return 0;
        }


        public int GetLargeFriendAvatar(ulong steamIDFriend)
        {
            Write($"GetLargeFriendAvatar {steamIDFriend}");
            return 0;
        }


        public int GetNumChatsWithUnreadPriorityMessages(IntPtr _)
        {
            Write($"GetNumChatsWithUnreadPriorityMessages");
            return 0;
        }


        public string GetPersonaName(IntPtr _)
        {
            Write($"GetPersonaName");
            return SteamEmulator.PersonaName;
        }


        public EPersonaState GetPersonaState(IntPtr _)
        {
            Write($"GetPersonaState");
            return EPersonaState.k_EPersonaStateOnline;
        }


        public string GetPlayerNickname(ulong steamIDPlayer)
        {
            Write($"GetPlayerNickname {steamIDPlayer}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDPlayer);
            if (friend == null)
            {
                return "";
            }
            return friend.PersonaName;
        }


        public uint GetUserRestrictions(IntPtr _)
        {
            Write($"GetUserRestrictions");
            return 0;
        }


        public bool HasFriend(ulong steamIDFriend, int iFriendFlags)
        {
            Write($"HasFriend {steamIDFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
            return friend != null;
        }


        public bool InviteUserToGame(ulong steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
        {
            Write($"InviteUserToGame {steamIDFriend} {pchConnectString}");
            return false;
        }


        public bool IsClanChatAdmin(ulong steamIDClanChat, ulong steamIDUser)
        {
            Write($"IsClanChatAdmin {steamIDClanChat}");
            return false;
        }


        public bool IsClanChatWindowOpenInSteam(ulong steamIDClanChat)
        {
            Write($"IsClanChatWindowOpenInSteam {steamIDClanChat}");
            return false;
        }


        public bool IsClanOfficialGameGroup(ulong steamIDClan)
        {
            Write($"IsClanOfficialGameGroup {steamIDClan}");
            return false;
        }


        public bool IsClanPublic(ulong steamIDClan)
        {
            Write($"IsClanpublic {steamIDClan}");
            return false;
        }


        public ulong IsFollowing(ulong steamID)
        {
            Write($"IsFollowing {steamID}");
            return (ulong)0;
        }


        public bool IsUserInSource(ulong steamIDUser, ulong steamIDSource)
        {
            Write($"IsUserInSource {steamIDUser}");
            return false;
        }


        public ulong JoinClanChatRoom(ulong steamIDClan)
        {
            Write($"JoinClanChatRoom {steamIDClan}");
            return (ulong)0;
        }


        public bool LeaveClanChatRoom(ulong steamIDClan)
        {
            Write($"LeaveClanChatRoom {steamIDClan}");
            return true;
        }


        public bool OpenClanChatWindowInSteam(ulong steamIDClanChat)
        {
            Write($"OpenClanChatWindowInSteam {steamIDClanChat}");
            return false;
        }


        public bool RegisterProtocolInOverlayBrowser([MarshalAs(UnmanagedType.LPStr)] string pchProtocol)
        {
            Write($"RegisterProtocolInOverlayBrowser {pchProtocol}");
            return false;
        }


        public bool ReplyToFriendMessage(ulong steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchMsgToSend)
        {
            Write($"ReplyToFriendMessage {steamIDFriend} {pchMsgToSend}");
            return false;
        }


        public ulong RequestClanOfficerList(ulong steamIDClan)
        {
            Write($"RequestClanOfficerList {steamIDClan}");
            return 0;
        }


        public void RequestFriendRichPresence(ulong steamIDFriend)
        {
            Write($"RequestFriendRichPresence {steamIDFriend}");
        }


        public bool RequestUserInformation(ulong steamIDUser, bool bRequireNameOnly)
        {
            Write($"RequestUserInformation {steamIDUser}");
            return false;
        }


        public bool SendClanChatMessage(ulong steamIDClanChat, [MarshalAs(UnmanagedType.LPStr)] string pchText)
        {
            Write($"SendClanChatMessage {steamIDClanChat} {pchText}");
            return false;
        }


        public void SetInGameVoiceSpeaking(ulong steamIDUser, bool bSpeaking)
        {
            Write($"SetInGameVoiceSpeaking {steamIDUser}");
        }


        public bool SetListenForFriendsMessages(bool bInterceptEnabled)
        {
            Write($"SetListenForFriendsMessages {bInterceptEnabled}");
            return true;
        }


        public ulong SetPersonaName([MarshalAs(UnmanagedType.LPStr)] string pchPersonaName)
        {
            Write($"SetPersonaName {pchPersonaName}");
            SteamEmulator.PersonaName = pchPersonaName;
            return 1;
        }


        public void SetPlayedWith(ulong steamIDUserPlayedWith)
        {
            Write($"SetPlayedWith {steamIDUserPlayedWith}");
        }


        public bool SetRichPresence([MarshalAs(UnmanagedType.LPStr)] string pchKey, [MarshalAs(UnmanagedType.LPStr)] string pchValue)
        {
            Write($"SetRichPresence {pchKey} {pchValue}");
            return true;
        }
    }
}

