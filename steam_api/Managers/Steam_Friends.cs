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
    public class Steam_Friends : ISteamFriends
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
        public List<CSteamID> Users;

        public void ActivateGameOverlay([MarshalAs(UnmanagedType.LPStr)] string friendsGroupID)
        {
            PRINT_DEBUG($"Steam_Friends.ActivateGameOverlay {friendsGroupID}");
        }

        public void ActivateGameOverlayInviteDialog(CSteamID steamIDLobby)
        {
            PRINT_DEBUG($"Steam_Friends.ActivateGameOverlayInviteDialog {steamIDLobby}");
        }

        public void ActivateGameOverlayInviteDialogConnectString([MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
        {
            PRINT_DEBUG($"Steam_Friends.ActivateGameOverlayInviteDialogConnectString {pchConnectString}");
        }

        public void ActivateGameOverlayRemotePlayTogetherInviteDialog(CSteamID steamIDLobby)
        {
            PRINT_DEBUG($"Steam_Friends.ActivateGameOverlayRemotePlayTogetherInviteDialog {steamIDLobby}");
        }

        public void ActivateGameOverlayToStore(AppId_t nAppID, EOverlayToStoreFlag eFlag)
        {
            PRINT_DEBUG($"Steam_Friends.ActivateGameOverlayToStore {nAppID} {eFlag}");
        }

        public void ActivateGameOverlayToUser([MarshalAs(UnmanagedType.LPStr)] string friendsGroupID, CSteamID steamID)
        {
            PRINT_DEBUG($"Steam_Friends.ActivateGameOverlayToUser {friendsGroupID} {steamID}");
        }

        public void ActivateGameOverlayToWebPage([MarshalAs(UnmanagedType.LPStr)] string pchURL, EActivateGameOverlayToWebPageMode eMode = EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default)
        {
            PRINT_DEBUG($"Steam_Friends.ActivateGameOverlayToWebPage {pchURL}");
        }

        public void ClearRichPresence()
        {
            PRINT_DEBUG($"Steam_Friends.ClearRichPresence");
        }

        public bool CloseClanChatWindowInSteam(CSteamID steamIDClanChat)
        {
            PRINT_DEBUG($"Steam_Friends.CloseClanChatWindowInSteam {steamIDClanChat}");
            return true;
        }

        public SteamAPICall_t DownloadClanActivityCounts(CSteamID[] psteamIDClans, int cClansToRequest)
        {
            PRINT_DEBUG($"Steam_Friends.DownloadClanActivityCounts {cClansToRequest}");
            return (SteamAPICall_t)0;
        }

        public SteamAPICall_t EnumerateFollowingList(uint unStartIndex)
        {
            PRINT_DEBUG($"Steam_Friends.EnumerateFollowingList {unStartIndex}");
            return (SteamAPICall_t)0;
        }

        public CSteamID GetChatMemberByIndex(CSteamID steamIDClan, int iUser)
        {
            PRINT_DEBUG($"Steam_Friends.GetChatMemberByIndex {steamIDClan}");
            return CSteamID.Nil;
        }

        public bool GetClanActivityCounts(CSteamID steamIDClan, int pnOnline, int pnInGame, int pnChatting)
        {
            PRINT_DEBUG($"Steam_Friends.ActivateGameOverlay {steamIDClan}");
            return false;
        }

        public CSteamID GetClanByIndex(int iClan)
        {
            PRINT_DEBUG($"Steam_Friends.GetClanByIndex {iClan}");
            return CSteamID.Nil;
        }

        public int GetClanChatMemberCount(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.GetClanChatMemberCount {steamIDClan}");
            return 0;
        }

        public int GetClanChatMessage(CSteamID steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, EChatEntryType peChatEntryType, CSteamID[] psteamidChatter)
        {
            PRINT_DEBUG($"Steam_Friends.GetClanChatMessage {steamIDClanChat}");
            return 0;
        }

        public int GetClanCount()
        {
            PRINT_DEBUG($"Steam_Friends.GetClanCount");
            return 0;
        }

        public string GetClanName(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.GetClanName {steamIDClan}");
            return "";
        }

        public CSteamID GetClanOfficerByIndex(CSteamID steamIDClan, int iOfficer)
        {
            PRINT_DEBUG($"Steam_Friends.GetClanOfficerByIndex {steamIDClan}");
            return CSteamID.Nil;
        }

        public int GetClanOfficerCount(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.GetClanOfficerCount {steamIDClan}");
            return 0;
        }

        public CSteamID GetClanOwner(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.GetClanOwner {steamIDClan}");
            return CSteamID.Nil;
        }

        public string GetClanTag(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.GetClanTag {steamIDClan}");
            return "";
        }

        public CSteamID GetCoplayFriend(int iCoplayFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetCoplayFriend {iCoplayFriend}");
            return CSteamID.Nil;
        }

        public int GetCoplayFriendCount()
        {
            PRINT_DEBUG($"Steam_Friends.GetCoplayFriendCount");
            return 0;
        }

        public SteamAPICall_t GetFollowerCount(CSteamID steamID)
        {
            PRINT_DEBUG($"Steam_Friends.GetFollowerCount {steamID}");
            return (SteamAPICall_t)0;
        }

        public CSteamID GetFriendByIndex(int iFriend, int iFriendFlags)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendByIndex {iFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)iFriend);
            if (friend == null)
            {
                return CSteamID.Nil;
            }
            return new CSteamID(friend.SteamId);
        }

        public AppId_t GetFriendCoplayGame(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendCoplayGame {steamIDFriend}");
            return (AppId_t)0;
        }

        public int GetFriendCoplayTime(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendCoplayTime {steamIDFriend}");
            return 0;
        }

        public int GetFriendCount(int iFriendFlags)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendCount {iFriendFlags}");
            return 0;
        }

        public int GetFriendCountFromSource(CSteamID steamIDSource)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendCountFromSource {steamIDSource}");
            return 0;
        }

        public CSteamID GetFriendFromSourceByIndex(CSteamID steamIDSource, int iFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendFromSourceByIndex {steamIDSource} {iFriend}");
            return CSteamID.Nil;
        }

        public bool GetFriendGamePlayed(CSteamID steamIDFriend, out FriendGameInfo_t pFriendGameInfo)
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

        public int GetFriendMessage(CSteamID steamIDFriend, int iMessageID, IntPtr pvData, int cubData, EChatEntryType peChatEntryType)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendMessage {steamIDFriend}");
            return 0;
        }

        public string GetFriendPersonaName(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendPersonaName {(uint)steamIDFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
            if (friend == null)
            {
                return "";
            }
            return friend.PersonaName;
        }

        public string GetFriendPersonaNameHistory(CSteamID steamIDFriend, int iPersonaName)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendPersonaNameHistory {steamIDFriend}");
            return "SKYNET";
        }

        public EPersonaState GetFriendPersonaState(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendPersonaState {steamIDFriend}");
            return Users.Find(f => f == steamIDFriend) == null ? EPersonaState.k_EPersonaStateOffline : EPersonaState.k_EPersonaStateOnline;
        }

        public EFriendRelationship GetFriendRelationship(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendRelationship {steamIDFriend}");
            return EFriendRelationship.k_EFriendRelationshipNone;
        }

        public string GetFriendRichPresence(CSteamID steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchKey)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendRichPresence {steamIDFriend} {pchKey}");
            return "";
        }

        public string GetFriendRichPresenceKeyByIndex(CSteamID steamIDFriend, int iKey)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendRichPresenceKeyByIndex {steamIDFriend} {iKey}");
            return "";
        }

        public int GetFriendRichPresenceKeyCount(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendRichPresenceKeyCount {steamIDFriend}");
            return 0;
        }

        public int GetFriendsGroupCount()
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendsGroupCount");
            return 0;
        }

        public FriendsGroupID_t GetFriendsGroupIDByIndex(int iFG)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendsGroupIDByIndex {iFG}");
            return (FriendsGroupID_t)0;
        }

        public int GetFriendsGroupMembersCount(FriendsGroupID_t friendsGroupID)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendsGroupMembersCount {friendsGroupID}");
            return 0;
        }

        public void GetFriendsGroupMembersList(FriendsGroupID_t friendsGroupID, CSteamID[] pOutSteamIDMembers, int nMembersCount)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendsGroupMembersList {friendsGroupID}");
        }

        public string GetFriendsGroupName(FriendsGroupID_t friendsGroupID)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendsGroupName {friendsGroupID}");
            return "";
        }

        public int GetFriendSteamLevel(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetFriendSteamLevel {steamIDFriend}");
            return 100;
        }

        public int GetSmallFriendAvatar(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetSmallFriendAvatar {steamIDFriend}");
            return 0;
        }

        public int GetMediumFriendAvatar(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetMediumFriendAvatar {steamIDFriend}");
            return 0;
        }

        public int GetLargeFriendAvatar(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.GetLargeFriendAvatar {steamIDFriend}");
            return 0;
        }

        public int GetNumChatsWithUnreadPriorityMessages()
        {
            PRINT_DEBUG($"Steam_Friends.GetNumChatsWithUnreadPriorityMessages");
            return 0;
        }

        public string GetPersonaName()
        {
            PRINT_DEBUG($"Steam_Friends.GetPersonaName");
            return SteamClient.PersonaName;
        }

        public EPersonaState GetPersonaState()
        {
            PRINT_DEBUG($"Steam_Friends.GetPersonaState");
            return EPersonaState.k_EPersonaStateOnline;
        }

        public string GetPlayerNickname(CSteamID steamIDPlayer)
        {
            PRINT_DEBUG($"Steam_Friends.GetPlayerNickname {steamIDPlayer}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDPlayer);
            if (friend == null)
            {
                return "";
            }
            return friend.PersonaName;
        }

        public uint GetUserRestrictions()
        {
            PRINT_DEBUG($"Steam_Friends.GetUserRestrictions");
            return 0;
        }
        public bool HasFriend(CSteamID steamIDFriend, int iFriendFlags)
        {
            PRINT_DEBUG($"Steam_Friends.HasFriend {steamIDFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
            return friend != null;
        }

        public bool InviteUserToGame(CSteamID steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
        {
            PRINT_DEBUG($"Steam_Friends.InviteUserToGame {steamIDFriend} {pchConnectString}");
            return false;
        }

        public bool IsClanChatAdmin(CSteamID steamIDClanChat, CSteamID steamIDUser)
        {
            PRINT_DEBUG($"Steam_Friends.IsClanChatAdmin {steamIDClanChat}");
            return false;
        }

        public bool IsClanChatWindowOpenInSteam(CSteamID steamIDClanChat)
        {
            PRINT_DEBUG($"Steam_Friends.IsClanChatWindowOpenInSteam {steamIDClanChat}");
            return false;
        }

        public bool IsClanOfficialGameGroup(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.IsClanOfficialGameGroup {steamIDClan}");
            return false;
        }

        public bool IsClanPublic(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.IsClanPublic {steamIDClan}");
            return false;
        }

        public SteamAPICall_t IsFollowing(CSteamID steamID)
        {
            PRINT_DEBUG($"Steam_Friends.IsFollowing {steamID}");
            return (SteamAPICall_t)0;
        }

        public bool IsUserInSource(CSteamID steamIDUser, CSteamID steamIDSource)
        {
            PRINT_DEBUG($"Steam_Friends.IsUserInSource {steamIDUser}");
            return false;
        }

        public SteamAPICall_t JoinClanChatRoom(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.JoinClanChatRoom {steamIDClan}");
            return (SteamAPICall_t)0;
        }

        public bool LeaveClanChatRoom(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.LeaveClanChatRoom {steamIDClan}");
            return true;
        }

        public bool OpenClanChatWindowInSteam(CSteamID steamIDClanChat)
        {
            PRINT_DEBUG($"Steam_Friends.OpenClanChatWindowInSteam {steamIDClanChat}");
            return false;
        }

        public bool RegisterProtocolInOverlayBrowser([MarshalAs(UnmanagedType.LPStr)] string pchProtocol)
        {
            PRINT_DEBUG($"Steam_Friends.RegisterProtocolInOverlayBrowser {pchProtocol}");
            return false;
        }

        public bool ReplyToFriendMessage(CSteamID steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchMsgToSend)
        {
            PRINT_DEBUG($"Steam_Friends.ReplyToFriendMessage {steamIDFriend} {pchMsgToSend}");
            return false;
        }

        public SteamAPICall_t RequestClanOfficerList(CSteamID steamIDClan)
        {
            PRINT_DEBUG($"Steam_Friends.RequestClanOfficerList {steamIDClan}");
            return (SteamAPICall_t)0;
        }

        public void RequestFriendRichPresence(CSteamID steamIDFriend)
        {
            PRINT_DEBUG($"Steam_Friends.RequestFriendRichPresence {steamIDFriend}");
        }

        public bool RequestUserInformation(CSteamID steamIDUser, bool bRequireNameOnly)
        {
            PRINT_DEBUG($"Steam_Friends.RequestUserInformation {steamIDUser}");
            return false;
        }

        public bool SendClanChatMessage(CSteamID steamIDClanChat, [MarshalAs(UnmanagedType.LPStr)] string pchText)
        {
            PRINT_DEBUG($"Steam_Friends.SendClanChatMessage {steamIDClanChat} {pchText}");
            return false;
        }

        public void SetInGameVoiceSpeaking(CSteamID steamIDUser, bool bSpeaking)
        {
            PRINT_DEBUG($"Steam_Friends.SetInGameVoiceSpeaking {steamIDUser}");
        }

        public bool SetListenForFriendsMessages(bool bInterceptEnabled)
        {
            PRINT_DEBUG($"Steam_Friends.SetListenForFriendsMessages {bInterceptEnabled}");
            return true;
        }

        public SteamAPICall_t SetPersonaName([MarshalAs(UnmanagedType.LPStr)] string pchPersonaName)
        {
            PRINT_DEBUG($"Steam_Friends.SetPersonaName {pchPersonaName}");
            SteamClient.SetPersonaName(pchPersonaName);
            return (SteamAPICall_t)1;
        }

        public void SetPlayedWith(CSteamID steamIDUserPlayedWith)
        {
            PRINT_DEBUG($"Steam_Friends.SetPlayedWith {steamIDUserPlayedWith}");
        }

        public bool SetRichPresence([MarshalAs(UnmanagedType.LPStr)] string pchKey, [MarshalAs(UnmanagedType.LPStr)] string pchValue)
        {
            PRINT_DEBUG($"Steam_Friends.SetRichPresence {pchKey} {pchValue}");
            return true;
        }
        private void PRINT_DEBUG(object msg)
        {
            Log.Write(msg);
        }
    }
}
