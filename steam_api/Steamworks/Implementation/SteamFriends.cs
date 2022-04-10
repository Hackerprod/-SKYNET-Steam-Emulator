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
    [StructLayout(LayoutKind.Sequential)]
    public class SteamFriends : ISteamInterface
    {
        public void ActivateGameOverlay(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string friendsGroupID)
        {
            Write($"ActivateGameOverlay {friendsGroupID}");
        }

        public void ActivateGameOverlayInviteDialog(IntPtr _, ulong steamIDLobby)
        {
            Write($"ActivateGameOverlayInviteDialog {steamIDLobby}");
        }

        public void ActivateGameOverlayInviteDialogConnectString(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
        {
            Write($"ActivateGameOverlayInviteDialogConnectString {pchConnectString}");
        }

        public void ActivateGameOverlayRemotePlayTogetherInviteDialog(IntPtr _, ulong steamIDLobby)
        {
            Write($"ActivateGameOverlayRemotePlayTogetherInviteDialog {steamIDLobby}");
        }

        public void ActivateGameOverlayToStore(IntPtr _, AppId_t nAppID, EOverlayToStoreFlag eFlag)
        {
            Write($"ActivateGameOverlayToStore {nAppID} {eFlag}");
        }


        public void ActivateGameOverlayToUser(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string friendsGroupID, ulong steamID)
        {
            Write($"ActivateGameOverlayToUser {friendsGroupID} {steamID}");
        }


        public void ActivateGameOverlayToWebPage(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchURL, EActivateGameOverlayToWebPageMode eMode = EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default)
        {
            Write($"ActivateGameOverlayToWebPage {pchURL}");
        }


        public void ClearRichPresence(IntPtr _)
        {
            Write($"ClearRichPresence");
        }


        public bool CloseClanChatWindowInSteam(IntPtr _, ulong steamIDClanChat)
        {
            Write($"CloseClanChatWindowInSteam {steamIDClanChat}");
            return true;
        }


        public SteamAPICall_t DownloadClanActivityCounts(IntPtr _, IntPtr[] psteamIDClans, int cClansToRequest)
        {
            Write($"DownloadClanActivityCounts {cClansToRequest}");
            return (SteamAPICall_t)0;
        }


        public SteamAPICall_t EnumerateFollowingList(IntPtr _, uint unStartIndex)
        {
            Write($"EnumerateFollowingList {unStartIndex}");
            return (SteamAPICall_t)0;
        }


        public IntPtr GetChatMemberByIndex(IntPtr _, ulong steamIDClan, int iUser)
        {
            Write($"GetChatMemberByIndex {steamIDClan}");
            return IntPtr.Zero;
        }


        public bool GetClanActivityCounts(IntPtr _, ulong steamIDClan, int pnOnline, int pnInGame, int pnChatting)
        {
            Write($"ActivateGameOverlay {steamIDClan}");
            return false;
        }


        public IntPtr GetClanByIndex(IntPtr _, int iClan)
        {
            Write($"GetClanByIndex {iClan}");
            return IntPtr.Zero;
        }


        public int GetClanChatMemberCount(IntPtr _, ulong steamIDClan)
        {
            Write($"GetClanChatMemberCount {steamIDClan}");
            return 0;
        }


        public int GetClanChatMessage(IntPtr _, ulong steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, EChatEntryType peChatEntryType, IntPtr[] psteamidChatter)
        {
            Write($"GetClanChatMessage {steamIDClanChat}");
            return 0;
        }


        public int GetClanCount(IntPtr _)
        {
            Write($"GetClanCount");
            return 0;
        }


        public string GetClanName(IntPtr _, ulong steamIDClan)
        {
            Write($"GetClanName {steamIDClan}");
            return "";
        }


        public IntPtr GetClanOfficerByIndex(IntPtr _, ulong steamIDClan, int iOfficer)
        {
            Write($"GetClanOfficerByIndex {steamIDClan}");
            return IntPtr.Zero;
        }


        public int GetClanOfficerCount(IntPtr _, ulong steamIDClan)
        {
            Write($"GetClanOfficerCount {steamIDClan}");
            return 0;
        }


        public IntPtr GetClanOwner(IntPtr _, ulong steamIDClan)
        {
            Write($"GetClanOwner {steamIDClan}");
            return IntPtr.Zero;
        }


        public string GetClanTag(IntPtr _, ulong steamIDClan)
        {
            Write($"GetClanTag {steamIDClan}");
            return "";
        }


        public IntPtr GetCoplayFriend(IntPtr _, int iCoplayFriend)
        {
            Write($"GetCoplayFriend {iCoplayFriend}");
            return IntPtr.Zero;
        }


        public int GetCoplayFriendCount(IntPtr _)
        {
            Write($"GetCoplayFriendCount");
            return 0;
        }


        public SteamAPICall_t GetFollowerCount(IntPtr _, ulong steamID)
        {
            Write($"GetFollowerCount {steamID}");
            return (SteamAPICall_t)0;
        }


        public IntPtr GetFriendByIndex(IntPtr _, int iFriend, int iFriendFlags)
        {
            Write($"GetFriendByIndex {iFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)iFriend);
            if (friend == null)
            {
                return IntPtr.Zero;
            }
            return IntPtr.Zero;
        }


        public AppId_t GetFriendCoplayGame(IntPtr _, ulong steamIDFriend)
        {
            Write($"GetFriendCoplayGame {steamIDFriend}");
            return (AppId_t)0;
        }


        public int GetFriendCoplayTime(IntPtr _, ulong steamIDFriend)
        {
            Write($"GetFriendCoplayTime {steamIDFriend}");
            return 0;
        }


        public int GetFriendCount(IntPtr _, int iFriendFlags)
        {
            Write($"GetFriendCount {iFriendFlags}");
            return 0;
        }


        public int GetFriendCountFromSource(IntPtr _, ulong steamIDSource)
        {
            Write($"GetFriendCountFromSource {steamIDSource}");
            return 0;
        }


        public IntPtr GetFriendFromSourceByIndex(IntPtr _, ulong steamIDSource, int iFriend)
        {
            Write($"GetFriendFromSourceByIndex {steamIDSource} {iFriend}");
            return IntPtr.Zero;
        }


        public bool GetFriendGamePlayed(IntPtr _, ulong steamIDFriend, FriendGameInfo_t pFriendGameInfo)
        {
            Write($"GetFriendGamePlayed {(uint)steamIDFriend}");
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


        public int GetFriendMessage(IntPtr _, ulong steamIDFriend, int iMessageID, IntPtr pvData, int cubData, EChatEntryType peChatEntryType)
        {
            Write($"GetFriendMessage {steamIDFriend}");
            return 0;
        }


        public string GetFriendPersonaName(IntPtr _, ulong steamIDFriend)
        {
            Write($"GetFriendPersonaName {(uint)steamIDFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
            if (friend == null)
            {
                return "";
            }
            return friend.PersonaName;
        }


        public string GetFriendPersonaNameHistory(IntPtr _, ulong steamIDFriend, int iPersonaName)
        {
            Write($"GetFriendPersonaNameHistory {steamIDFriend}");
            return "SKYNET";
        }


        public EPersonaState GetFriendPersonaState(IntPtr _, ulong steamIDFriend)
        {
            Write($"GetFriendPersonaState {steamIDFriend}");
            return Users.Find(f => f == steamIDFriend) == null ? EPersonaState.k_EPersonaStateOffline : EPersonaState.k_EPersonaStateOnline;
        }


        public EFriendRelationship GetFriendRelationship(IntPtr _, ulong steamIDFriend)
        {
            Write($"GetFriendRelationship {steamIDFriend}");
            return EFriendRelationship.k_EFriendRelationshipNone;
        }


        public string GetFriendRichPresence(IntPtr _, ulong steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchKey)
        {
            Write($"GetFriendRichPresence {steamIDFriend} {pchKey}");
            return "";
        }


        public string GetFriendRichPresenceKeyByIndex(IntPtr _, ulong steamIDFriend, int iKey)
        {
            Write($"GetFriendRichPresenceKeyByIndex {steamIDFriend} {iKey}");
            return "";
        }


        public int GetFriendRichPresenceKeyCount(IntPtr _, ulong steamIDFriend)
        {
            Write($"GetFriendRichPresenceKeyCount {steamIDFriend}");
            return 0;
        }


        public int GetFriendsGroupCount(IntPtr _)
        {
            Write($"GetFriendsGroupCount");
            return 0;
        }


        public FriendsGroupID_t GetFriendsGroupIDByIndex(IntPtr _, int iFG)
        {
            Write($"GetFriendsGroupIDByIndex {iFG}");
            return (FriendsGroupID_t)0;
        }


        public int GetFriendsGroupMembersCount(IntPtr _, FriendsGroupID_t friendsGroupID)
        {
            Write($"GetFriendsGroupMembersCount {friendsGroupID}");
            return 0;
        }


        public void GetFriendsGroupMembersList(IntPtr _, FriendsGroupID_t friendsGroupID, IntPtr[] pOutSteamIDMembers, int nMembersCount)
        {
            Write($"GetFriendsGroupMembersList {friendsGroupID}");
        }


        public string GetFriendsGroupName(IntPtr _, FriendsGroupID_t friendsGroupID)
        {
            Write($"GetFriendsGroupName {friendsGroupID}");
            return "";
        }


        public int GetFriendSteamLevel(IntPtr _, ulong steamIDFriend)
        {
            Write($"GetFriendSteamLevel {steamIDFriend}");
            return 100;
        }


        public int GetSmallFriendAvatar(IntPtr _, ulong steamIDFriend)
        {
            Write($"GetSmallFriendAvatar {steamIDFriend}");
            return 0;
        }


        public int GetMediumFriendAvatar(IntPtr _, ulong steamIDFriend)
        {
            Write($"GetMediumFriendAvatar {steamIDFriend}");
            return 0;
        }


        public int GetLargeFriendAvatar(IntPtr _, ulong steamIDFriend)
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


        public string GetPlayerNickname(IntPtr _, ulong steamIDPlayer)
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


        public bool HasFriend(IntPtr _, ulong steamIDFriend, int iFriendFlags)
        {
            Write($"HasFriend {steamIDFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
            return friend != null;
        }


        public bool InviteUserToGame(IntPtr _, ulong steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
        {
            Write($"InviteUserToGame {steamIDFriend} {pchConnectString}");
            return false;
        }


        public bool IsClanChatAdmin(IntPtr _, ulong steamIDClanChat, ulong steamIDUser)
        {
            Write($"IsClanChatAdmin {steamIDClanChat}");
            return false;
        }


        public bool IsClanChatWindowOpenInSteam(IntPtr _, ulong steamIDClanChat)
        {
            Write($"IsClanChatWindowOpenInSteam {steamIDClanChat}");
            return false;
        }


        public bool IsClanOfficialGameGroup(IntPtr _, ulong steamIDClan)
        {
            Write($"IsClanOfficialGameGroup {steamIDClan}");
            return false;
        }


        public bool IsClanPublic(IntPtr _, ulong steamIDClan)
        {
            Write($"IsClanpublic {steamIDClan}");
            return false;
        }


        public SteamAPICall_t IsFollowing(IntPtr _, ulong steamID)
        {
            Write($"IsFollowing {steamID}");
            return (SteamAPICall_t)0;
        }


        public bool IsUserInSource(IntPtr _, ulong steamIDUser, ulong steamIDSource)
        {
            Write($"IsUserInSource {steamIDUser}");
            return false;
        }


        public SteamAPICall_t JoinClanChatRoom(IntPtr _, ulong steamIDClan)
        {
            Write($"JoinClanChatRoom {steamIDClan}");
            return (SteamAPICall_t)0;
        }


        public bool LeaveClanChatRoom(IntPtr _, ulong steamIDClan)
        {
            Write($"LeaveClanChatRoom {steamIDClan}");
            return true;
        }


        public bool OpenClanChatWindowInSteam(IntPtr _, ulong steamIDClanChat)
        {
            Write($"OpenClanChatWindowInSteam {steamIDClanChat}");
            return false;
        }


        public bool RegisterProtocolInOverlayBrowser(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchProtocol)
        {
            Write($"RegisterProtocolInOverlayBrowser {pchProtocol}");
            return false;
        }


        public bool ReplyToFriendMessage(IntPtr _, ulong steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchMsgToSend)
        {
            Write($"ReplyToFriendMessage {steamIDFriend} {pchMsgToSend}");
            return false;
        }


        public SteamAPICall_t RequestClanOfficerList(IntPtr _, ulong steamIDClan)
        {
            Write($"RequestClanOfficerList {steamIDClan}");
            return (SteamAPICall_t)0;
        }


        public void RequestFriendRichPresence(IntPtr _, ulong steamIDFriend)
        {
            Write($"RequestFriendRichPresence {steamIDFriend}");
        }


        public bool RequestUserInformation(IntPtr _, ulong steamIDUser, bool bRequireNameOnly)
        {
            Write($"RequestUserInformation {steamIDUser}");
            return false;
        }


        public bool SendClanChatMessage(IntPtr _, ulong steamIDClanChat, [MarshalAs(UnmanagedType.LPStr)] string pchText)
        {
            Write($"SendClanChatMessage {steamIDClanChat} {pchText}");
            return false;
        }


        public void SetInGameVoiceSpeaking(IntPtr _, ulong steamIDUser, bool bSpeaking)
        {
            Write($"SetInGameVoiceSpeaking {steamIDUser}");
        }


        public bool SetListenForFriendsMessages(IntPtr _, bool bInterceptEnabled)
        {
            Write($"SetListenForFriendsMessages {bInterceptEnabled}");
            return true;
        }


        public SteamAPICall_t SetPersonaName(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchPersonaName)
        {
            Write($"SetPersonaName {pchPersonaName}");
            SteamEmulator.PersonaName = pchPersonaName;
            return (SteamAPICall_t)1;
        }


        public void SetPlayedWith(IntPtr _, ulong steamIDUserPlayedWith)
        {
            Write($"SetPlayedWith {steamIDUserPlayedWith}");
        }


        public bool SetRichPresence(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchKey, [MarshalAs(UnmanagedType.LPStr)] string pchValue)
        {
            Write($"SetRichPresence {pchKey} {pchValue}");
            return true;
        }

        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public List<Friend> Friends;
        public List<ulong> Users;

        public SteamFriends()
        {
            InterfaceVersion = "SteamFriends";
            Friends = new List<Friend>();
            Users = new List<ulong>();
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}

