using System;
using SKYNET.Steamworks.Implementation;

using SteamAPICall_t = System.UInt64;
using FriendsGroupID_t = System.UInt16;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamFriends017")]
    public class SteamFriends017 : ISteamInterface
    {
        public string GetPersonaName(IntPtr _)
        {
            return SteamFriends.Instance.GetPersonaName();
        }

        public SteamAPICall_t SetPersonaName(IntPtr _, string name)
        {
            return SteamFriends.Instance.SetPersonaName(name);
        }

        public int GetPersonaState(IntPtr _)
        {
            return SteamFriends.Instance.GetPersonaState();
        }

        public int GetFriendCount(IntPtr _, int iFriendFlags)
        {
            return SteamFriends.Instance.GetFriendCount(iFriendFlags);
        }

        public CSteamID GetFriendByIndex(IntPtr _, int iFriend, int iFriendFlags)
        {
            return SteamFriends.Instance.GetFriendByIndex(iFriend, iFriendFlags);
        }

        public int GetFriendRelationship(IntPtr _, ulong steamID)
        {
            return SteamFriends.Instance.GetFriendRelationship(steamID);
        }

        public int GetFriendPersonaState(IntPtr _, ulong steamID)
        {
            return SteamFriends.Instance.GetFriendPersonaState(steamID);
        }

        public string GetFriendPersonaName(IntPtr _, ulong steamID)
        {
            return SteamFriends.Instance.GetFriendPersonaName(steamID);
        }

        public bool GetFriendGamePlayed(IntPtr _, ulong steamID, ref FriendGameInfo_t pFriendGameInfo)
        {
            return SteamFriends.Instance.GetFriendGamePlayed(steamID, ref pFriendGameInfo);
        }

        public string GetFriendPersonaNameHistory(IntPtr _, ulong steamID, int index)
        {
            return SteamFriends.Instance.GetFriendPersonaNameHistory(steamID, index);
        }

        public int GetFriendSteamLevel(IntPtr _, ulong steamID)
        {
            return SteamFriends.Instance.GetFriendSteamLevel(steamID);
        }

        public string GetPlayerNickname(IntPtr _, ulong steamID)
        {
            return SteamFriends.Instance.GetPlayerNickname(steamID);
        }

        public int GetFriendsGroupCount(IntPtr _)
        {
            return SteamFriends.Instance.GetFriendsGroupCount();
        }

        public FriendsGroupID_t GetFriendsGroupIDByIndex(IntPtr _, int iFG)
        {
            return SteamFriends.Instance.GetFriendsGroupIDByIndex(iFG);
        }

        public string GetFriendsGroupName(IntPtr _, FriendsGroupID_t friendsGroupID)
        {
            return SteamFriends.Instance.GetFriendsGroupName(friendsGroupID);
        }

        public int GetFriendsGroupMembersCount(IntPtr _, FriendsGroupID_t friendsGroupID)
        {
            return SteamFriends.Instance.GetFriendsGroupMembersCount(friendsGroupID);
        }

        public void GetFriendsGroupMembersList(IntPtr _, FriendsGroupID_t friendsGroupID, ref ulong[] pOutSteamIDMembers, int nMembersCount)
        {
            SteamFriends.Instance.GetFriendsGroupMembersList(friendsGroupID, ref pOutSteamIDMembers, nMembersCount);
        }

        public bool HasFriend(IntPtr _, ulong steamIDFriend, int iFriendFlags)
        {
            return SteamFriends.Instance.HasFriend(steamIDFriend, iFriendFlags);
        }

        public int GetClanCount(IntPtr _)
        {
            return SteamFriends.Instance.GetClanCount();
        }

        public CSteamID GetClanByIndex(IntPtr _, int iClan)
        {
            return SteamFriends.Instance.GetClanByIndex(iClan);
        }

        public string GetClanName(IntPtr _, ulong steamIDClan)
        {
            return SteamFriends.Instance.GetClanName(steamIDClan);
        }

        public string GetClanTag(IntPtr _, ulong steamIDClan)
        {
            return SteamFriends.Instance.GetClanTag(steamIDClan);
        }

        public bool GetClanActivityCounts(IntPtr _, ulong steamIDClan, ref int pnOnline, ref int pnInGame, ref int pnChatting)
        {
            return SteamFriends.Instance.GetClanActivityCounts(steamIDClan, ref pnOnline, ref pnInGame, ref pnChatting);
        }

        public SteamAPICall_t DownloadClanActivityCounts(IntPtr _, IntPtr psteamIDClans, int cClansToRequest)
        {
            return SteamFriends.Instance.DownloadClanActivityCounts(psteamIDClans, cClansToRequest);
        }

        public int GetFriendCountFromSource(IntPtr _, ulong steamIDSource)
        {
            return SteamFriends.Instance.GetFriendCountFromSource(steamIDSource);
        }

        public CSteamID GetFriendFromSourceByIndex(IntPtr _, ulong steamIDSource, int iFriend)
        {
            return SteamFriends.Instance.GetFriendFromSourceByIndex(steamIDSource, iFriend);
        }

        public bool IsUserInSource(IntPtr _, ulong steamIDUser, ulong steamIDSource)
        {
            return SteamFriends.Instance.IsUserInSource(steamIDUser, steamIDSource);
        }

        public void SetInGameVoiceSpeaking(IntPtr _, ulong steamIDUser, bool bSpeaking)
        {
            SteamFriends.Instance.SetInGameVoiceSpeaking(steamIDUser, bSpeaking);
        }

        public void ActivateGameOverlay(IntPtr _, string pchDialog)
        {
            SteamFriends.Instance.ActivateGameOverlay(pchDialog);
        }

        public void ActivateGameOverlayToUser(IntPtr _, string pchDialog, ulong steamID)
        {
            SteamFriends.Instance.ActivateGameOverlayToUser(pchDialog, steamID);
        }

        public void ActivateGameOverlayToWebPage(IntPtr _, string pchURL, int eMode)
        {
            SteamFriends.Instance.ActivateGameOverlayToWebPage(pchURL, eMode);
        }

        public void ActivateGameOverlayToStore(IntPtr _, uint nAppID, int eFlag)
        {
            SteamFriends.Instance.ActivateGameOverlayToStore(nAppID, eFlag);
        }

        public void SetPlayedWith(IntPtr _, ulong steamIDUserPlayedWith)
        {
            SteamFriends.Instance.SetPlayedWith(steamIDUserPlayedWith);
        }

        public void ActivateGameOverlayInviteDialog(IntPtr _, ulong steamIDLobby)
        {
            SteamFriends.Instance.ActivateGameOverlayInviteDialog(steamIDLobby);
        }

        public int GetSmallFriendAvatar(IntPtr _, ulong steamIDFriend)
        {
            return SteamFriends.Instance.GetSmallFriendAvatar(steamIDFriend);
        }

        public int GetMediumFriendAvatar(IntPtr _, ulong steamIDFriend)
        {
            return SteamFriends.Instance.GetMediumFriendAvatar(steamIDFriend);
        }

        public int GetLargeFriendAvatar(IntPtr _, ulong steamIDFriend)
        {
            return SteamFriends.Instance.GetLargeFriendAvatar(steamIDFriend);
        }

        public bool RequestUserInformation(IntPtr _, ulong steamIDUser, bool bRequireNameOnly)
        {
            return SteamFriends.Instance.RequestUserInformation(steamIDUser, bRequireNameOnly);
        }

        public SteamAPICall_t RequestClanOfficerList(IntPtr _, ulong steamIDClan)
        {
            return SteamFriends.Instance.RequestClanOfficerList(steamIDClan);
        }

        public CSteamID GetClanOwner(IntPtr _, ulong steamIDClan)
        {
            return SteamFriends.Instance.GetClanOwner(steamIDClan);
        }

        public int GetClanOfficerCount(IntPtr _, ulong steamIDClan)
        {
            return SteamFriends.Instance.GetClanOfficerCount(steamIDClan);
        }

        public CSteamID GetClanOfficerByIndex(IntPtr _, ulong steamIDClan, int iOfficer)
        {
            return SteamFriends.Instance.GetClanOfficerByIndex(steamIDClan, iOfficer);
        }

        public UInt32 GetUserRestrictions(IntPtr _)
        {
            return SteamFriends.Instance.GetUserRestrictions();
        }

        public bool SetRichPresence(IntPtr _, string pchKey, string pchValue)
        {
            return SteamFriends.Instance.SetRichPresence(pchKey, pchValue);
        }

        public void ClearRichPresence(IntPtr _)
        {
            SteamFriends.Instance.ClearRichPresence();
        }

        public string GetFriendRichPresence(IntPtr _, ulong steamIDFriend, string pchKey)
        {
            return SteamFriends.Instance.GetFriendRichPresence(steamIDFriend, pchKey);
        }

        public int GetFriendRichPresenceKeyCount(IntPtr _, ulong steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendRichPresenceKeyCount(steamIDFriend);
        }

        public string GetFriendRichPresenceKeyByIndex(IntPtr _, ulong steamIDFriend, int iKey)
        {
            return SteamFriends.Instance.GetFriendRichPresenceKeyByIndex(steamIDFriend, iKey);
        }
        public void RequestFriendRichPresence(IntPtr _, ulong steamIDFriend)
        {
            SteamFriends.Instance.RequestFriendRichPresence(steamIDFriend);
        }

        public bool InviteUserToGame(IntPtr _, ulong steamIDFriend, string pchConnectString)
        {
            return SteamFriends.Instance.InviteUserToGame(steamIDFriend, pchConnectString);
        }

        public int GetCoplayFriendCount(IntPtr _)
        {
            return SteamFriends.Instance.GetCoplayFriendCount();
        }

        public CSteamID GetCoplayFriend(IntPtr _, int iCoplayFriend)
        {
            return SteamFriends.Instance.GetCoplayFriend(iCoplayFriend);
        }

        public int GetFriendCoplayTime(IntPtr _, ulong steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendCoplayTime(steamIDFriend);
        }

        public uint GetFriendCoplayGame(IntPtr _, ulong steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendCoplayGame(steamIDFriend);
        }

        public SteamAPICall_t JoinClanChatRoom(IntPtr _, ulong steamIDClan)
        {
            return SteamFriends.Instance.JoinClanChatRoom(steamIDClan);
        }

        public bool LeaveClanChatRoom(IntPtr _, ulong steamIDClan)
        {
            return SteamFriends.Instance.LeaveClanChatRoom(steamIDClan);
        }

        public int GetClanChatMemberCount(IntPtr _, ulong steamIDClan)
        {
            return SteamFriends.Instance.GetClanChatMemberCount(steamIDClan);
        }

        public CSteamID GetChatMemberByIndex(IntPtr _, ulong steamIDClan, int iUser)
        {
            return SteamFriends.Instance.GetChatMemberByIndex(steamIDClan, iUser);
        }

        public bool SendClanChatMessage(IntPtr _, ulong steamIDClanChat, string pchText)
        {
            return SteamFriends.Instance.SendClanChatMessage(steamIDClanChat, pchText);
        }

        public int GetClanChatMessage(IntPtr _, ulong steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, int peChatEntryType, ref ulong[] psteamidChatter)
        {
            return SteamFriends.Instance.GetClanChatMessage(steamIDClanChat, iMessage, prgchText, cchTextMax, peChatEntryType, ref psteamidChatter);
        }

        public bool IsClanChatAdmin(IntPtr _, ulong steamIDClanChat, ulong steamIDUser)
        {
            return SteamFriends.Instance.IsClanChatAdmin(steamIDClanChat, steamIDUser);
        }

        public bool IsClanChatWindowOpenInSteam(IntPtr _, ulong steamIDClanChat)
        {
            return SteamFriends.Instance.IsClanChatWindowOpenInSteam(steamIDClanChat);
        }

        public bool OpenClanChatWindowInSteam(IntPtr _, ulong steamIDClanChat)
        {
            return SteamFriends.Instance.OpenClanChatWindowInSteam(steamIDClanChat);
        }

        public bool CloseClanChatWindowInSteam(IntPtr _, ulong steamIDClanChat)
        {
            return SteamFriends.Instance.CloseClanChatWindowInSteam(steamIDClanChat);
        }

        public bool SetListenForFriendsMessages(IntPtr _, bool bInterceptEnabled)
        {
            return SteamFriends.Instance.SetListenForFriendsMessages(bInterceptEnabled);
        }

        public bool ReplyToFriendMessage(IntPtr _, ulong steamIDFriend, string pchMsgToSend)
        {
            return SteamFriends.Instance.ReplyToFriendMessage(steamIDFriend, pchMsgToSend);
        }

        public int GetFriendMessage(IntPtr _, ulong steamIDFriend, int iMessageID, IntPtr pvData, int cubData, ref int peChatEntryType)
        {
            return SteamFriends.Instance.GetFriendMessage(steamIDFriend, iMessageID, pvData, cubData, peChatEntryType);
        }

        public SteamAPICall_t GetFollowerCount(IntPtr _, ulong steamID)
        {
            return SteamFriends.Instance.GetFollowerCount(steamID);
        }

        public SteamAPICall_t IsFollowing(IntPtr _, ulong steamID)
        {
            return SteamFriends.Instance.IsFollowing(steamID);
        }

        public SteamAPICall_t EnumerateFollowingList(IntPtr _, UInt32 unStartIndex)
        {
            return SteamFriends.Instance.EnumerateFollowingList(unStartIndex);
        }

        public bool IsClanPublic(IntPtr _, ulong steamIDClan)
        {
            return SteamFriends.Instance.IsClanPublic(steamIDClan);
        }

        public bool IsClanOfficialGameGroup(IntPtr _, ulong steamIDClan)
        {
            return SteamFriends.Instance.IsClanOfficialGameGroup(steamIDClan);
        }

        public int GetNumChatsWithUnreadPriorityMessages(IntPtr _)
        {
            return SteamFriends.Instance.GetNumChatsWithUnreadPriorityMessages();
        }

        public void ActivateGameOverlayRemotePlayTogetherInviteDialog(IntPtr _, ulong steamIDLobby)
        {
            SteamFriends.Instance.ActivateGameOverlayRemotePlayTogetherInviteDialog(steamIDLobby);
        }

        public bool RegisterProtocolInOverlayBrowser(IntPtr _, string pchProtocol)
        {
            return SteamFriends.Instance.RegisterProtocolInOverlayBrowser(pchProtocol);
        }

        public void ActivateGameOverlayInviteDialogConnectString(IntPtr _, string pchConnectString)
        {
            SteamFriends.Instance.ActivateGameOverlayInviteDialogConnectString(pchConnectString);
        }
    }
}
