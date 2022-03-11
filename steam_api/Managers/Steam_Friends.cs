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
    public class Steam_Friends : SteamInterface, ISteamFriends
    {
        public string ISteamFriends004 = "ISteamFriends004";
        public string ISteamFriends005 = "ISteamFriends005";
        public string ISteamFriends006 = "ISteamFriends006";
        public string ISteamFriends007 = "ISteamFriends007";
        public string ISteamFriends008 = "ISteamFriends008";
        public string ISteamFriends009 = "ISteamFriends009";
        public string ISteamFriends010 = "ISteamFriends010";
        public string ISteamFriends011 = "ISteamFriends011";
        public string ISteamFriends012 = "ISteamFriends012";
        public string ISteamFriends013 = "ISteamFriends013";
        public string ISteamFriends014 = "ISteamFriends014";
        public string ISteamFriends015 = "ISteamFriends015";
        public string ISteamFriends016 = "ISteamFriends016";

        public List<Friend> Friends;
        public List<IntPtr> Users;

        
        public void ActivateGameOverlay([MarshalAs(UnmanagedType.LPStr)] string friendsGroupID)
        {
            Write($"Steam_Friends.ActivateGameOverlay {friendsGroupID}");
        }

        
        public void ActivateGameOverlayInviteDialog(IntPtr steamIDLobby)
        {
            Write($"Steam_Friends.ActivateGameOverlayInviteDialog {steamIDLobby}");
        }

        
        public void ActivateGameOverlayInviteDialogConnectString([MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
        {
            Write($"Steam_Friends.ActivateGameOverlayInviteDialogConnectString {pchConnectString}");
        }

        
        public void ActivateGameOverlayRemotePlayTogetherInviteDialog(IntPtr steamIDLobby)
        {
            Write($"Steam_Friends.ActivateGameOverlayRemotePlayTogetherInviteDialog {steamIDLobby}");
        }

        
        public void ActivateGameOverlayToStore(AppId_t nAppID, EOverlayToStoreFlag eFlag)
        {
            Write($"Steam_Friends.ActivateGameOverlayToStore {nAppID} {eFlag}");
        }

        
        public void ActivateGameOverlayToUser([MarshalAs(UnmanagedType.LPStr)] string friendsGroupID, IntPtr steamID)
        {
            Write($"Steam_Friends.ActivateGameOverlayToUser {friendsGroupID} {steamID}");
        }

        
        public void ActivateGameOverlayToWebPage([MarshalAs(UnmanagedType.LPStr)] string pchURL, EActivateGameOverlayToWebPageMode eMode = EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default)
        {
            Write($"Steam_Friends.ActivateGameOverlayToWebPage {pchURL}");
        }

        
        public void ClearRichPresence()
        {
            Write($"Steam_Friends.ClearRichPresence");
        }

        
        public bool CloseClanChatWindowInSteam(IntPtr steamIDClanChat)
        {
            Write($"Steam_Friends.CloseClanChatWindowInSteam {steamIDClanChat}");
            return true;
        }

        
        public SteamAPICall_t DownloadClanActivityCounts(IntPtr[] psteamIDClans, int cClansToRequest)
        {
            Write($"Steam_Friends.DownloadClanActivityCounts {cClansToRequest}");
            return (SteamAPICall_t)0;
        }

        
        public SteamAPICall_t EnumerateFollowingList(uint unStartIndex)
        {
            Write($"Steam_Friends.EnumerateFollowingList {unStartIndex}");
            return (SteamAPICall_t)0;
        }

        
        public IntPtr GetChatMemberByIndex(IntPtr steamIDClan, int iUser)
        {
            Write($"Steam_Friends.GetChatMemberByIndex {steamIDClan}");
            return IntPtr.Zero;
        }

        
        public bool GetClanActivityCounts(IntPtr steamIDClan, int pnOnline, int pnInGame, int pnChatting)
        {
            Write($"Steam_Friends.ActivateGameOverlay {steamIDClan}");
            return false;
        }

        
        public IntPtr GetClanByIndex(int iClan)
        {
            Write($"Steam_Friends.GetClanByIndex {iClan}");
            return IntPtr.Zero;
        }

        
        public int GetClanChatMemberCount(IntPtr steamIDClan)
        {
            Write($"Steam_Friends.GetClanChatMemberCount {steamIDClan}");
            return 0;
        }

        
        public int GetClanChatMessage(IntPtr steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, EChatEntryType peChatEntryType, IntPtr[] psteamidChatter)
        {
            Write($"Steam_Friends.GetClanChatMessage {steamIDClanChat}");
            return 0;
        }

        
        public int GetClanCount()
        {
            Write($"Steam_Friends.GetClanCount");
            return 0;
        }

        
        public string GetClanName(IntPtr steamIDClan)
        {
            Write($"Steam_Friends.GetClanName {steamIDClan}");
            return "";
        }

        
        public IntPtr GetClanOfficerByIndex(IntPtr steamIDClan, int iOfficer)
        {
            Write($"Steam_Friends.GetClanOfficerByIndex {steamIDClan}");
            return IntPtr.Zero;
        }

        
        public int GetClanOfficerCount(IntPtr steamIDClan)
        {
            Write($"Steam_Friends.GetClanOfficerCount {steamIDClan}");
            return 0;
        }

        
        public IntPtr GetClanOwner(IntPtr steamIDClan)
        {
            Write($"Steam_Friends.GetClanOwner {steamIDClan}");
            return IntPtr.Zero;
        }

        
        public string GetClanTag(IntPtr steamIDClan)
        {
            Write($"Steam_Friends.GetClanTag {steamIDClan}");
            return "";
        }

        
        public IntPtr GetCoplayFriend(int iCoplayFriend)
        {
            Write($"Steam_Friends.GetCoplayFriend {iCoplayFriend}");
            return IntPtr.Zero;
        }

        
        public int GetCoplayFriendCount()
        {
            Write($"Steam_Friends.GetCoplayFriendCount");
            return 0;
        }

        
        public SteamAPICall_t GetFollowerCount(IntPtr steamID)
        {
            Write($"Steam_Friends.GetFollowerCount {steamID}");
            return (SteamAPICall_t)0;
        }

        
        public IntPtr GetFriendByIndex(int iFriend, int iFriendFlags)
        {
            Write($"Steam_Friends.GetFriendByIndex {iFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)iFriend);
            if (friend == null)
            {
                return IntPtr.Zero;
            }
            return IntPtr.Zero;
        }

        
        public AppId_t GetFriendCoplayGame(IntPtr steamIDFriend)
        {
            Write($"Steam_Friends.GetFriendCoplayGame {steamIDFriend}");
            return (AppId_t)0;
        }

        
        public int GetFriendCoplayTime(IntPtr steamIDFriend)
        {
            Write($"Steam_Friends.GetFriendCoplayTime {steamIDFriend}");
            return 0;
        }

        
        public int GetFriendCount(int iFriendFlags)
        {
            Write($"Steam_Friends.GetFriendCount {iFriendFlags}");
            return 0;
        }

        
        public int GetFriendCountFromSource(IntPtr steamIDSource)
        {
            Write($"Steam_Friends.GetFriendCountFromSource {steamIDSource}");
            return 0;
        }

        
        public IntPtr GetFriendFromSourceByIndex(IntPtr steamIDSource, int iFriend)
        {
            Write($"Steam_Friends.GetFriendFromSourceByIndex {steamIDSource} {iFriend}");
            return IntPtr.Zero;
        }

        
        public bool GetFriendGamePlayed(IntPtr steamIDFriend, out FriendGameInfo_t pFriendGameInfo)
        {
            Write($"Steam_Friends.GetFriendGamePlayed {(uint)steamIDFriend}");
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

        
        public int GetFriendMessage(IntPtr steamIDFriend, int iMessageID, IntPtr pvData, int cubData, EChatEntryType peChatEntryType)
        {
            Write($"Steam_Friends.GetFriendMessage {steamIDFriend}");
            return 0;
        }

        
        public string GetFriendPersonaName(IntPtr steamIDFriend)
        {
            Write($"Steam_Friends.GetFriendPersonaName {(uint)steamIDFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
            if (friend == null)
            {
                return "";
            }
            return friend.PersonaName;
        }

        
        public string GetFriendPersonaNameHistory(IntPtr steamIDFriend, int iPersonaName)
        {
            Write($"Steam_Friends.GetFriendPersonaNameHistory {steamIDFriend}");
            return "SKYNET";
        }

        
        public EPersonaState GetFriendPersonaState(IntPtr steamIDFriend)
        {
            Write($"Steam_Friends.GetFriendPersonaState {steamIDFriend}");
            return Users.Find(f => f == steamIDFriend) == null ? EPersonaState.k_EPersonaStateOffline : EPersonaState.k_EPersonaStateOnline;
        }

        
        public EFriendRelationship GetFriendRelationship(IntPtr steamIDFriend)
        {
            Write($"Steam_Friends.GetFriendRelationship {steamIDFriend}");
            return EFriendRelationship.k_EFriendRelationshipNone;
        }

        
        public string GetFriendRichPresence(IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchKey)
        {
            Write($"Steam_Friends.GetFriendRichPresence {steamIDFriend} {pchKey}");
            return "";
        }

        
        public string GetFriendRichPresenceKeyByIndex(IntPtr steamIDFriend, int iKey)
        {
            Write($"Steam_Friends.GetFriendRichPresenceKeyByIndex {steamIDFriend} {iKey}");
            return "";
        }

        
        public int GetFriendRichPresenceKeyCount(IntPtr steamIDFriend)
        {
            Write($"Steam_Friends.GetFriendRichPresenceKeyCount {steamIDFriend}");
            return 0;
        }

        
        public int GetFriendsGroupCount()
        {
            Write($"Steam_Friends.GetFriendsGroupCount");
            return 0;
        }

        
        public FriendsGroupID_t GetFriendsGroupIDByIndex(int iFG)
        {
            Write($"Steam_Friends.GetFriendsGroupIDByIndex {iFG}");
            return (FriendsGroupID_t)0;
        }

        
        public int GetFriendsGroupMembersCount(FriendsGroupID_t friendsGroupID)
        {
            Write($"Steam_Friends.GetFriendsGroupMembersCount {friendsGroupID}");
            return 0;
        }

        
        public void GetFriendsGroupMembersList(FriendsGroupID_t friendsGroupID, IntPtr[] pOutSteamIDMembers, int nMembersCount)
        {
            Write($"Steam_Friends.GetFriendsGroupMembersList {friendsGroupID}");
        }

        
        public string GetFriendsGroupName(FriendsGroupID_t friendsGroupID)
        {
            Write($"Steam_Friends.GetFriendsGroupName {friendsGroupID}");
            return "";
        }

        
        public int GetFriendSteamLevel(IntPtr steamIDFriend)
        {
            Write($"Steam_Friends.GetFriendSteamLevel {steamIDFriend}");
            return 100;
        }

        
        public int GetSmallFriendAvatar(IntPtr steamIDFriend)
        {
            Write($"Steam_Friends.GetSmallFriendAvatar {steamIDFriend}");
            return 0;
        }

        
        public int GetMediumFriendAvatar(IntPtr steamIDFriend)
        {
            Write($"Steam_Friends.GetMediumFriendAvatar {steamIDFriend}");
            return 0;
        }

        
        public int GetLargeFriendAvatar(IntPtr steamIDFriend)
        {
            Write($"Steam_Friends.GetLargeFriendAvatar {steamIDFriend}");
            return 0;
        }

        
        public int GetNumChatsWithUnreadPriorityMessages()
        {
            Write($"Steam_Friends.GetNumChatsWithUnreadPriorityMessages");
            return 0;
        }

        
        public string GetPersonaName()
        {
            Write($"Steam_Friends.GetPersonaName");
            return SteamClient.PersonaName;
        }

        
        public EPersonaState GetPersonaState()
        {
            Write($"Steam_Friends.GetPersonaState");
            return EPersonaState.k_EPersonaStateOnline;
        }

        
        public string GetPlayerNickname(IntPtr steamIDPlayer)
        {
            Write($"Steam_Friends.GetPlayerNickname {steamIDPlayer}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDPlayer);
            if (friend == null)
            {
                return "";
            }
            return friend.PersonaName;
        }

        
        public uint GetUserRestrictions()
        {
            Write($"Steam_Friends.GetUserRestrictions");
            return 0;
        }

        
        public bool HasFriend(IntPtr steamIDFriend, int iFriendFlags)
        {
            Write($"Steam_Friends.HasFriend {steamIDFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
            return friend != null;
        }

        
        public bool InviteUserToGame(IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
        {
            Write($"Steam_Friends.InviteUserToGame {steamIDFriend} {pchConnectString}");
            return false;
        }

        
        public bool IsClanChatAdmin(IntPtr steamIDClanChat, IntPtr steamIDUser)
        {
            Write($"Steam_Friends.IsClanChatAdmin {steamIDClanChat}");
            return false;
        }

        
        public bool IsClanChatWindowOpenInSteam(IntPtr steamIDClanChat)
        {
            Write($"Steam_Friends.IsClanChatWindowOpenInSteam {steamIDClanChat}");
            return false;
        }

        
        public bool IsClanOfficialGameGroup(IntPtr steamIDClan)
        {
            Write($"Steam_Friends.IsClanOfficialGameGroup {steamIDClan}");
            return false;
        }

        
        public bool IsClanPublic(IntPtr steamIDClan)
        {
            Write($"Steam_Friends.IsClanpublic {steamIDClan}");
            return false;
        }

        
        public SteamAPICall_t IsFollowing(IntPtr steamID)
        {
            Write($"Steam_Friends.IsFollowing {steamID}");
            return (SteamAPICall_t)0;
        }

        
        public bool IsUserInSource(IntPtr steamIDUser, IntPtr steamIDSource)
        {
            Write($"Steam_Friends.IsUserInSource {steamIDUser}");
            return false;
        }

        
        public SteamAPICall_t JoinClanChatRoom(IntPtr steamIDClan)
        {
            Write($"Steam_Friends.JoinClanChatRoom {steamIDClan}");
            return (SteamAPICall_t)0;
        }

        
        public bool LeaveClanChatRoom(IntPtr steamIDClan)
        {
            Write($"Steam_Friends.LeaveClanChatRoom {steamIDClan}");
            return true;
        }

        
        public bool OpenClanChatWindowInSteam(IntPtr steamIDClanChat)
        {
            Write($"Steam_Friends.OpenClanChatWindowInSteam {steamIDClanChat}");
            return false;
        }

        
        public bool RegisterProtocolInOverlayBrowser([MarshalAs(UnmanagedType.LPStr)] string pchProtocol)
        {
            Write($"Steam_Friends.RegisterProtocolInOverlayBrowser {pchProtocol}");
            return false;
        }

        
        public bool ReplyToFriendMessage(IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchMsgToSend)
        {
            Write($"Steam_Friends.ReplyToFriendMessage {steamIDFriend} {pchMsgToSend}");
            return false;
        }

        
        public SteamAPICall_t RequestClanOfficerList(IntPtr steamIDClan)
        {
            Write($"Steam_Friends.RequestClanOfficerList {steamIDClan}");
            return (SteamAPICall_t)0;
        }

        
        public void RequestFriendRichPresence(IntPtr steamIDFriend)
        {
            Write($"Steam_Friends.RequestFriendRichPresence {steamIDFriend}");
        }

        
        public bool RequestUserInformation(IntPtr steamIDUser, bool bRequireNameOnly)
        {
            Write($"Steam_Friends.RequestUserInformation {steamIDUser}");
            return false;
        }

        
        public bool SendClanChatMessage(IntPtr steamIDClanChat, [MarshalAs(UnmanagedType.LPStr)] string pchText)
        {
            Write($"Steam_Friends.SendClanChatMessage {steamIDClanChat} {pchText}");
            return false;
        }

        
        public void SetInGameVoiceSpeaking(IntPtr steamIDUser, bool bSpeaking)
        {
            Write($"Steam_Friends.SetInGameVoiceSpeaking {steamIDUser}");
        }

        
        public bool SetListenForFriendsMessages(bool bInterceptEnabled)
        {
            Write($"Steam_Friends.SetListenForFriendsMessages {bInterceptEnabled}");
            return true;
        }

        
        public SteamAPICall_t SetPersonaName([MarshalAs(UnmanagedType.LPStr)] string pchPersonaName)
        {
            Write($"Steam_Friends.SetPersonaName {pchPersonaName}");
            SteamClient.SetPersonaName(pchPersonaName);
            return (SteamAPICall_t)1;
        }

        
        public void SetPlayedWith(IntPtr steamIDUserPlayedWith)
        {
            Write($"Steam_Friends.SetPlayedWith {steamIDUserPlayedWith}");
        }

        
        public bool SetRichPresence([MarshalAs(UnmanagedType.LPStr)] string pchKey, [MarshalAs(UnmanagedType.LPStr)] string pchValue)
        {
            Write($"Steam_Friends.SetRichPresence {pchKey} {pchValue}");
            return true;
        }
    }
}
