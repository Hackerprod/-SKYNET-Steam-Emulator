using SKYNET.Steamworks;
using System;


namespace SKYNET.Interface
{
    [Interface("SteamFriends017")]
    public class SteamFriends017 : ISteamInterface
    {
        public string GetPersonaName(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetPersonaName(_);
        }

        public ulong SetPersonaName(IntPtr _, string pchPersonaName)
        {
            return SteamEmulator.SteamFriends.SetPersonaName(pchPersonaName);
        }

        public EPersonaState GetPersonaState(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetPersonaState(_);
        }

        public int GetFriendCount(IntPtr _, int iFriendFlags)
        {
            return SteamEmulator.SteamFriends.GetFriendCount(iFriendFlags);
        }

        public ulong GetFriendByIndex(IntPtr _, int iFriend, int iFriendFlags)
        {
            return SteamEmulator.SteamFriends.GetFriendByIndex(iFriend, iFriendFlags);
        }

        public EFriendRelationship GetFriendRelationship(IntPtr _, ulong steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendRelationship(steamIDFriend);
        }

        public EPersonaState GetFriendPersonaState(IntPtr _, ulong steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendPersonaState(steamIDFriend);
        }

        public string GetFriendPersonaName(IntPtr _, ulong steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendPersonaName(steamIDFriend);
        }

        public bool GetFriendGamePlayed(ulong steamIDFriend, IntPtr  pFriendGameInfo )
        {
            return SteamEmulator.SteamFriends.GetFriendGamePlayed(steamIDFriend, pFriendGameInfo);
        }

        public string GetFriendPersonaNameHistory(IntPtr _, ulong steamIDFriend, int iPersonaName)
        {
            return SteamEmulator.SteamFriends.GetFriendPersonaNameHistory(steamIDFriend, iPersonaName);
        }

        public int GetFriendSteamLevel(IntPtr _, ulong steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendSteamLevel(steamIDFriend);
        }

        public string GetPlayerNickname(IntPtr _, ulong steamIDPlayer)
        {
            return SteamEmulator.SteamFriends.GetPlayerNickname(steamIDPlayer);
        }

        public int GetFriendsGroupCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupCount(_);
        }

        public int GetFriendsGroupIDByIndex(IntPtr _, int iFG)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupIDByIndex(iFG);
        }

        public string GetFriendsGroupName(IntPtr _, int friendsGroupID)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupName(friendsGroupID);
        }

        public int GetFriendsGroupMembersCount(IntPtr _, int friendsGroupID)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupMembersCount(friendsGroupID);
        }

        public void GetFriendsGroupMembersList(IntPtr _, int friendsGroupID, ulong pOutSteamIDMembers, int nMembersCount)
        {
            SteamEmulator.SteamFriends.GetFriendsGroupMembersList(friendsGroupID, pOutSteamIDMembers, nMembersCount);
        }

        public bool HasFriend(IntPtr _, ulong steamIDFriend, int iFriendFlags)
        {
            return SteamEmulator.SteamFriends.HasFriend(steamIDFriend, iFriendFlags);
        }

        public int GetClanCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetClanCount(_);
        }

        public ulong GetClanByIndex(IntPtr _, int iClan)
        {
            return SteamEmulator.SteamFriends.GetClanByIndex(iClan);
        }

        public string GetClanName(IntPtr _, ulong steamIDClan)
        {
            return SteamEmulator.SteamFriends.GetClanName(steamIDClan);
        }

        public string GetClanTag(IntPtr _, ulong steamIDClan)
        {
            return SteamEmulator.SteamFriends.GetClanTag(steamIDClan);
        }

        public bool GetClanActivityCounts(IntPtr _, ulong steamIDClan, int pnOnline, int pnInGame, int pnChatting)
        {
            return SteamEmulator.SteamFriends.GetClanActivityCounts(steamIDClan, pnOnline, pnInGame, pnChatting);
        }

        public ulong DownloadClanActivityCounts(IntPtr _, ulong psteamIDClans, int cClansToRequest)
        {
            return SteamEmulator.SteamFriends.DownloadClanActivityCounts(psteamIDClans, cClansToRequest);
        }

        public int GetFriendCountFromSource(IntPtr _, ulong steamIDSource)
        {
            return SteamEmulator.SteamFriends.GetFriendCountFromSource(steamIDSource);
        }

        public ulong GetFriendFromSourceByIndex(IntPtr _, ulong steamIDSource, int iFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendFromSourceByIndex(steamIDSource, iFriend);
        }

        public bool IsUserInSource(IntPtr _, ulong steamIDUser, ulong steamIDSource)
        {
            return SteamEmulator.SteamFriends.IsUserInSource(steamIDUser, steamIDSource);
        }

        public void SetInGameVoiceSpeaking(IntPtr _, ulong steamIDUser, bool bSpeaking)
        {
            SteamEmulator.SteamFriends.SetInGameVoiceSpeaking(steamIDUser, bSpeaking);
        }

        public void ActivateGameOverlay(IntPtr _, string pchDialog)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlay(pchDialog);
        }

        public void ActivateGameOverlayToUser(IntPtr _, string pchDialog, ulong steamID)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayToUser(pchDialog, steamID);
        }

        public void ActivateGameOverlayToWebPage(IntPtr _, string pchURL, int eMode)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayToWebPage(pchURL, eMode);
        }

        public void ActivateGameOverlayToStore(IntPtr _, uint nAppID, EOverlayToStoreFlag eFlag)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayToStore(nAppID, eFlag);
        }

        public void SetPlayedWith(IntPtr _, ulong steamIDUserPlayedWith)
        {
            SteamEmulator.SteamFriends.SetPlayedWith(steamIDUserPlayedWith);
        }

        public void ActivateGameOverlayInviteDialog(IntPtr _, ulong steamIDLobby)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayInviteDialog(steamIDLobby);
        }

        public int GetSmallFriendAvatar(IntPtr _, ulong steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetSmallFriendAvatar(steamIDFriend);
        }

        public int GetMediumFriendAvatar(IntPtr _, ulong steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetMediumFriendAvatar(steamIDFriend);
        }

        public int GetLargeFriendAvatar(IntPtr _, ulong steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetLargeFriendAvatar(steamIDFriend);
        }

        public bool RequestUserInformation(IntPtr _, ulong steamIDUser, bool bRequireNameOnly)
        {
            return SteamEmulator.SteamFriends.RequestUserInformation(steamIDUser, bRequireNameOnly);
        }

        public ulong RequestClanOfficerList(IntPtr _, ulong steamIDClan)
        {
            return SteamEmulator.SteamFriends.RequestClanOfficerList(steamIDClan);
        }

        public ulong GetClanOwner(IntPtr _, ulong steamIDClan)
        {
            return SteamEmulator.SteamFriends.GetClanOwner(steamIDClan);
        }

        public int GetClanOfficerCount(IntPtr _, ulong steamIDClan)
        {
            return SteamEmulator.SteamFriends.GetClanOfficerCount(steamIDClan);
        }

        public ulong GetClanOfficerByIndex(IntPtr _, ulong steamIDClan, int iOfficer)
        {
            return SteamEmulator.SteamFriends.GetClanOfficerByIndex(steamIDClan, iOfficer);
        }

        public uint GetUserRestrictions(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetUserRestrictions(_);
        }

        public bool SetRichPresence(IntPtr _, string pchKey, string pchValue)
        {
            return SteamEmulator.SteamFriends.SetRichPresence(pchKey, pchValue);
        }

        public void ClearRichPresence(IntPtr _)
        {
            SteamEmulator.SteamFriends.ClearRichPresence(_);
        }

        public string GetFriendRichPresence(IntPtr _, ulong steamIDFriend, string pchKey)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresence(steamIDFriend, pchKey);
        }

        public int GetFriendRichPresenceKeyCount(IntPtr _, ulong steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyCount(steamIDFriend);
        }

        public string GetFriendRichPresenceKeyByIndex(IntPtr _, ulong steamIDFriend, int iKey)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyByIndex(steamIDFriend, iKey);
        }

        public void RequestFriendRichPresence(IntPtr _, ulong steamIDFriend)
        {
            SteamEmulator.SteamFriends.RequestFriendRichPresence(steamIDFriend);
        }

        public bool InviteUserToGame(IntPtr _, ulong steamIDFriend, string pchConnectString)
        {
            return SteamEmulator.SteamFriends.InviteUserToGame(steamIDFriend, pchConnectString);
        }

        public int GetCoplayFriendCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetCoplayFriendCount(_);
        }

        public ulong GetCoplayFriend(IntPtr _, int iCoplayFriend)
        {
            return SteamEmulator.SteamFriends.GetCoplayFriend(iCoplayFriend);
        }

        public int GetFriendCoplayTime(IntPtr _, ulong steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendCoplayTime(steamIDFriend);
        }

        public uint GetFriendCoplayGame(IntPtr _, ulong steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendCoplayGame(steamIDFriend);
        }

        public ulong JoinClanChatRoom(IntPtr _, ulong steamIDClan)
        {
            return SteamEmulator.SteamFriends.JoinClanChatRoom(steamIDClan);
        }

        public bool LeaveClanChatRoom(IntPtr _, ulong steamIDClan)
        {
            return SteamEmulator.SteamFriends.LeaveClanChatRoom(steamIDClan);
        }

        public int GetClanChatMemberCount(IntPtr _, ulong steamIDClan)
        {
            return SteamEmulator.SteamFriends.GetClanChatMemberCount(steamIDClan);
        }

        public ulong GetChatMemberByIndex(IntPtr _, ulong steamIDClan, int iUser)
        {
            return SteamEmulator.SteamFriends.GetChatMemberByIndex(steamIDClan, iUser);
        }

        public bool SendClanChatMessage(IntPtr _, ulong steamIDClanChat, string pchText)
        {
            return SteamEmulator.SteamFriends.SendClanChatMessage(steamIDClanChat, pchText);
        }

        public int GetClanChatMessage(ulong steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, int peChatEntryType, ulong psteamidChatter)
        {
            return SteamEmulator.SteamFriends.GetClanChatMessage(steamIDClanChat, iMessage, prgchText, cchTextMax, peChatEntryType, psteamidChatter);
        }

        public bool IsClanChatAdmin(IntPtr _, ulong steamIDClanChat, ulong steamIDUser)
        {
            return SteamEmulator.SteamFriends.IsClanChatAdmin(steamIDClanChat, steamIDUser);
        }

        public bool IsClanChatWindowOpenInSteam(IntPtr _, ulong steamIDClanChat)
        {
            return SteamEmulator.SteamFriends.IsClanChatWindowOpenInSteam(steamIDClanChat);
        }

        public bool OpenClanChatWindowInSteam(IntPtr _, ulong steamIDClanChat)
        {
            return SteamEmulator.SteamFriends.OpenClanChatWindowInSteam(steamIDClanChat);
        }

        public bool CloseClanChatWindowInSteam(IntPtr _, ulong steamIDClanChat)
        {
            return SteamEmulator.SteamFriends.CloseClanChatWindowInSteam(steamIDClanChat);
        }

        public bool SetListenForFriendsMessages(IntPtr _, bool bInterceptEnabled)
        {
            return SteamEmulator.SteamFriends.SetListenForFriendsMessages(bInterceptEnabled);
        }

        public bool ReplyToFriendMessage(IntPtr _, ulong steamIDFriend, string pchMsgToSend)
        {
            return SteamEmulator.SteamFriends.ReplyToFriendMessage(steamIDFriend, pchMsgToSend);
        }

        public int GetFriendMessage(IntPtr _, ulong steamIDFriend, int iMessageID, IntPtr pvData, int cubData, EChatEntryType peChatEntryType)
        {
            return SteamEmulator.SteamFriends.GetFriendMessage(steamIDFriend, iMessageID, pvData, cubData, peChatEntryType);
        }

        public ulong GetFollowerCount(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetFollowerCount(steamID);
        }

        public ulong IsFollowing(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.IsFollowing(steamID);
        }

        public ulong EnumerateFollowingList(IntPtr _, uint unStartIndex)
        {
            return SteamEmulator.SteamFriends.EnumerateFollowingList(unStartIndex);
        }

        public bool IsClanPublic(IntPtr _, ulong steamIDClan)
        {
            return SteamEmulator.SteamFriends.IsClanPublic(steamIDClan);
        }

        public bool IsClanOfficialGameGroup(IntPtr _, ulong steamIDClan)
        {
            return SteamEmulator.SteamFriends.IsClanOfficialGameGroup(steamIDClan);
        }

        public int GetNumChatsWithUnreadPriorityMessages(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetNumChatsWithUnreadPriorityMessages(_);
        }

        public void ActivateGameOverlayRemotePlayTogetherInviteDialog(IntPtr _, ulong steamIDLobby)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayRemotePlayTogetherInviteDialog(steamIDLobby);
        }

        public bool RegisterProtocolInOverlayBrowser(IntPtr _, string pchProtocol)
        {
            return SteamEmulator.SteamFriends.RegisterProtocolInOverlayBrowser(pchProtocol);
        }

        public void ActivateGameOverlayInviteDialogConnectString(IntPtr _, string pchConnectString)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayInviteDialogConnectString(pchConnectString);
        }


    }
}
