using SKYNET.Steamworks;
using SKYNET.Types;
using Steamworks;
using System;


namespace SKYNET.Interface
{
    [Interface("SteamFriends017")]
    public class SteamFriends017 : ISteamInterface
    {
        public string GetPersonaName(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetPersonaName();
        }

        public ulong SetPersonaName(IntPtr _, string pchPersonaName)
        {
            return SteamEmulator.SteamFriends.SetPersonaName(pchPersonaName);
        }

        public EPersonaState GetPersonaState(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetPersonaState();
        }

        public int GetFriendCount(IntPtr _, int iFriendFlags)
        {
            return SteamEmulator.SteamFriends.GetFriendCount(iFriendFlags);
        }

        public ulong GetFriendByIndex(IntPtr _, int iFriend, int iFriendFlags)
        {
            return SteamEmulator.SteamFriends.GetFriendByIndex(iFriend, iFriendFlags);
        }

        public int GetFriendRelationship(IntPtr _, SteamID steamIDFriend)
        {
            return (int)SteamEmulator.SteamFriends.GetFriendRelationship(steamIDFriend);
        }

        public int GetFriendPersonaState(IntPtr _, SteamID steamIDFriend)
        {
            return (int)SteamEmulator.SteamFriends.GetFriendPersonaState(steamIDFriend);
        }

        public string GetFriendPersonaName(IntPtr _, SteamID steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendPersonaName(steamIDFriend);
        }

        public bool GetFriendGamePlayed(IntPtr _, SteamID steamIDFriend, IntPtr pFriendGameInfo )
        {
            bool result = SteamEmulator.SteamFriends.GetFriendGamePlayed(steamIDFriend, pFriendGameInfo);
            return result;
        }

        public string GetFriendPersonaNameHistory(IntPtr _, SteamID steamIDFriend, int iPersonaName)
        {
            return SteamEmulator.SteamFriends.GetFriendPersonaNameHistory(steamIDFriend, iPersonaName);
        }

        public int GetFriendSteamLevel(IntPtr _, SteamID steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendSteamLevel(steamIDFriend);
        }

        public string GetPlayerNickname(IntPtr _, SteamID steamIDPlayer)
        {
            return SteamEmulator.SteamFriends.GetPlayerNickname(steamIDPlayer);
        }

        public int GetFriendsGroupCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupCount();
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

        public void GetFriendsGroupMembersList(IntPtr _, int friendsGroupID, IntPtr pOutSteamIDMembers, int nMembersCount)
        {
            SteamEmulator.SteamFriends.GetFriendsGroupMembersList(friendsGroupID, pOutSteamIDMembers, nMembersCount);
        }

        public bool HasFriend(IntPtr _, SteamID steamIDFriend, int iFriendFlags)
        {
            return SteamEmulator.SteamFriends.HasFriend(steamIDFriend, iFriendFlags);
        }

        public int GetClanCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetClanCount();
        }

        public ulong GetClanByIndex(IntPtr _, int iClan)
        {
            return SteamEmulator.SteamFriends.GetClanByIndex(iClan);
        }

        public string GetClanName(IntPtr _, SteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.GetClanName(steamIDClan);
        }

        public string GetClanTag(IntPtr _, SteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.GetClanTag(steamIDClan);
        }

        public bool GetClanActivityCounts(IntPtr _, SteamID steamIDClan, int pnOnline, int pnInGame, int pnChatting)
        {
            return SteamEmulator.SteamFriends.GetClanActivityCounts(steamIDClan, pnOnline, pnInGame, pnChatting);
        }

        public ulong DownloadClanActivityCounts(IntPtr _, ulong psteamIDClans, int cClansToRequest)
        {
            return SteamEmulator.SteamFriends.DownloadClanActivityCounts(psteamIDClans, cClansToRequest);
        }

        public int GetFriendCountFromSource(IntPtr _, SteamID steamIDSource)
        {
            return SteamEmulator.SteamFriends.GetFriendCountFromSource(steamIDSource);
        }

        public ulong GetFriendFromSourceByIndex(IntPtr _, SteamID steamIDSource, int iFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendFromSourceByIndex(steamIDSource, iFriend);
        }

        public bool IsUserInSource(IntPtr _, SteamID steamIDUser, SteamID steamIDSource)
        {
            return SteamEmulator.SteamFriends.IsUserInSource(steamIDUser, steamIDSource);
        }

        public void SetInGameVoiceSpeaking(IntPtr _, SteamID steamIDUser, bool bSpeaking)
        {
            SteamEmulator.SteamFriends.SetInGameVoiceSpeaking(steamIDUser, bSpeaking);
        }

        public void ActivateGameOverlay(IntPtr _, string pchDialog)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlay(pchDialog);
        }

        public void ActivateGameOverlayToUser(IntPtr _, string pchDialog, SteamID steamID)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayToUser(pchDialog, steamID);
        }

        public void ActivateGameOverlayToWebPage(IntPtr _, string pchURL, int eMode)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayToWebPage(pchURL, eMode);
        }

        public void ActivateGameOverlayToStore(IntPtr _, uint nAppID, int eFlag)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayToStore(nAppID, eFlag);
        }

        public void SetPlayedWith(IntPtr _, SteamID steamIDUserPlayedWith)
        {
            SteamEmulator.SteamFriends.SetPlayedWith(steamIDUserPlayedWith);
        }

        public void ActivateGameOverlayInviteDialog(IntPtr _, SteamID steamIDLobby)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayInviteDialog(steamIDLobby);
        }

        public int GetSmallFriendAvatar(IntPtr _, SteamID steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetSmallFriendAvatar(steamIDFriend);
        }

        public int GetMediumFriendAvatar(IntPtr _, SteamID steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetMediumFriendAvatar(steamIDFriend);
        }

        public int GetLargeFriendAvatar(IntPtr _, SteamID steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetLargeFriendAvatar(steamIDFriend);
        }

        public bool RequestUserInformation(IntPtr _, SteamID steamIDUser, bool bRequireNameOnly)
        {
            return SteamEmulator.SteamFriends.RequestUserInformation(steamIDUser, bRequireNameOnly);
        }

        public ulong RequestClanOfficerList(IntPtr _, SteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.RequestClanOfficerList(steamIDClan);
        }

        public ulong GetClanOwner(IntPtr _, SteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.GetClanOwner(steamIDClan);
        }

        public int GetClanOfficerCount(IntPtr _, SteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.GetClanOfficerCount(steamIDClan);
        }

        public ulong GetClanOfficerByIndex(IntPtr _, SteamID steamIDClan, int iOfficer)
        {
            return SteamEmulator.SteamFriends.GetClanOfficerByIndex(steamIDClan, iOfficer);
        }

        public uint GetUserRestrictions(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetUserRestrictions();
        }

        public bool SetRichPresence(IntPtr _, string pchKey, string pchValue)
        {
            return SteamEmulator.SteamFriends.SetRichPresence(pchKey, pchValue);
        }

        public void ClearRichPresence(IntPtr _)
        {
            SteamEmulator.SteamFriends.ClearRichPresence();
        }

        public string GetFriendRichPresence(IntPtr _, SteamID steamIDFriend, string pchKey)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresence(steamIDFriend, pchKey);
        }

        public int GetFriendRichPresenceKeyCount(IntPtr _, SteamID steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyCount(steamIDFriend);
        }

        public string GetFriendRichPresenceKeyByIndex(IntPtr _, SteamID steamIDFriend, int iKey)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyByIndex(steamIDFriend, iKey);
        }

        public void RequestFriendRichPresence(IntPtr _, SteamID steamIDFriend)
        {
            SteamEmulator.SteamFriends.RequestFriendRichPresence(steamIDFriend);
        }

        public bool InviteUserToGame(IntPtr _, SteamID steamIDFriend, string pchConnectString)
        {
            return SteamEmulator.SteamFriends.InviteUserToGame(steamIDFriend, pchConnectString);
        }

        public int GetCoplayFriendCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetCoplayFriendCount();
        }

        public ulong GetCoplayFriend(IntPtr _, int iCoplayFriend)
        {
            return SteamEmulator.SteamFriends.GetCoplayFriend(iCoplayFriend);
        }

        public int GetFriendCoplayTime(IntPtr _, SteamID steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendCoplayTime(steamIDFriend);
        }

        public uint GetFriendCoplayGame(IntPtr _, SteamID steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendCoplayGame(steamIDFriend);
        }

        public ulong JoinClanChatRoom(IntPtr _, SteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.JoinClanChatRoom(steamIDClan);
        }

        public bool LeaveClanChatRoom(IntPtr _, SteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.LeaveClanChatRoom(steamIDClan);
        }

        public int GetClanChatMemberCount(IntPtr _, SteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.GetClanChatMemberCount(steamIDClan);
        }

        public ulong GetChatMemberByIndex(IntPtr _, SteamID steamIDClan, int iUser)
        {
            return SteamEmulator.SteamFriends.GetChatMemberByIndex(steamIDClan, iUser);
        }

        public bool SendClanChatMessage(IntPtr _, SteamID steamIDClanChat, string pchText)
        {
            return SteamEmulator.SteamFriends.SendClanChatMessage(steamIDClanChat, pchText);
        }

        public int GetClanChatMessage(SteamID steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, int peChatEntryType, ulong psteamidChatter)
        {
            return SteamEmulator.SteamFriends.GetClanChatMessage(steamIDClanChat, iMessage, prgchText, cchTextMax, peChatEntryType, psteamidChatter);
        }

        public bool IsClanChatAdmin(IntPtr _, SteamID steamIDClanChat, SteamID steamIDUser)
        {
            return SteamEmulator.SteamFriends.IsClanChatAdmin(steamIDClanChat, steamIDUser);
        }

        public bool IsClanChatWindowOpenInSteam(IntPtr _, SteamID steamIDClanChat)
        {
            return SteamEmulator.SteamFriends.IsClanChatWindowOpenInSteam(steamIDClanChat);
        }

        public bool OpenClanChatWindowInSteam(IntPtr _, SteamID steamIDClanChat)
        {
            return SteamEmulator.SteamFriends.OpenClanChatWindowInSteam(steamIDClanChat);
        }

        public bool CloseClanChatWindowInSteam(IntPtr _, SteamID steamIDClanChat)
        {
            return SteamEmulator.SteamFriends.CloseClanChatWindowInSteam(steamIDClanChat);
        }

        public bool SetListenForFriendsMessages(IntPtr _, bool bInterceptEnabled)
        {
            return SteamEmulator.SteamFriends.SetListenForFriendsMessages(bInterceptEnabled);
        }

        public bool ReplyToFriendMessage(IntPtr _, SteamID steamIDFriend, string pchMsgToSend)
        {
            return SteamEmulator.SteamFriends.ReplyToFriendMessage(steamIDFriend, pchMsgToSend);
        }

        public int GetFriendMessage(IntPtr _, SteamID steamIDFriend, int iMessageID, IntPtr pvData, int cubData, EChatEntryType peChatEntryType)
        {
            return SteamEmulator.SteamFriends.GetFriendMessage(steamIDFriend, iMessageID, pvData, cubData, peChatEntryType);
        }

        public ulong GetFollowerCount(IntPtr _, SteamID steamID)
        {
            return SteamEmulator.SteamFriends.GetFollowerCount(steamID);
        }

        public ulong IsFollowing(IntPtr _, SteamID steamID)
        {
            return SteamEmulator.SteamFriends.IsFollowing(steamID);
        }

        public ulong EnumerateFollowingList(IntPtr _, uint unStartIndex)
        {
            return SteamEmulator.SteamFriends.EnumerateFollowingList(unStartIndex);
        }

        public bool IsClanPublic(IntPtr _, SteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.IsClanPublic(steamIDClan);
        }

        public bool IsClanOfficialGameGroup(IntPtr _, SteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.IsClanOfficialGameGroup(steamIDClan);
        }

        public int GetNumChatsWithUnreadPriorityMessages(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetNumChatsWithUnreadPriorityMessages();
        }

        public void ActivateGameOverlayRemotePlayTogetherInviteDialog(IntPtr _, SteamID steamIDLobby)
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
