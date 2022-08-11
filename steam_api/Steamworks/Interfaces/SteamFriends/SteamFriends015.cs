using System;
using SKYNET.Steamworks.Implementation;

using SteamAPICall_t = System.UInt64;
using FriendsGroupID_t = System.UInt16;
using uint32 = System.UInt32;
using AppId_t = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamFriends015")]
    public class SteamFriends015 : ISteamInterface
    {
        public string GetPersonaName(IntPtr _)
        {
            return SteamFriends.Instance.GetPersonaName();
        }

        public SteamAPICall_t SetPersonaName(IntPtr _, string pchPersonaName)
        {
            return SteamFriends.Instance.SetPersonaName(pchPersonaName);
        }

        public EPersonaState GetPersonaState(IntPtr _)
        {
            return (EPersonaState)SteamFriends.Instance.GetPersonaState();
        }

        public int GetFriendCount(IntPtr _, int iFriendFlags)
        {
            return SteamFriends.Instance.GetFriendCount(iFriendFlags);
        }

        public CSteamID GetFriendByIndex(IntPtr _, int iFriend, int iFriendFlags)
        {
            return SteamFriends.Instance.GetFriendByIndex(iFriend, iFriendFlags);
        }

        public EFriendRelationship GetFriendRelationship(IntPtr _, CSteamID steamIDFriend)
        {
            return (EFriendRelationship) SteamFriends.Instance.GetFriendRelationship((ulong)steamIDFriend);
        }

        public EPersonaState GetFriendPersonaState(IntPtr _, CSteamID steamIDFriend)
        {
            return (EPersonaState) SteamFriends.Instance.GetFriendPersonaState((ulong)steamIDFriend);
        }

        public string GetFriendPersonaName(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendPersonaName((ulong)steamIDFriend);
        }

        public bool GetFriendGamePlayed(IntPtr _, ulong steamIDFriend, ref FriendGameInfo_t pFriendGameInfo )
        {
            return SteamFriends.Instance.GetFriendGamePlayed(steamIDFriend, ref pFriendGameInfo);
        }

        public string GetFriendPersonaNameHistory(IntPtr _, ulong steamIDFriend, int iPersonaName)
        {
            return SteamFriends.Instance.GetFriendPersonaNameHistory(steamIDFriend, iPersonaName);
        }

        public int GetFriendSteamLevel(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendSteamLevel((ulong)steamIDFriend);
        }

        public string GetPlayerNickname(IntPtr _, CSteamID steamIDPlayer)
        {
            return SteamFriends.Instance.GetPlayerNickname((ulong)steamIDPlayer);
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

        public bool HasFriend(IntPtr _, CSteamID steamIDFriend, int iFriendFlags)
        {
            return SteamFriends.Instance.HasFriend((ulong)steamIDFriend, iFriendFlags);
        }

        public int GetClanCount(IntPtr _)
        {
            return SteamFriends.Instance.GetClanCount();
        }

        public CSteamID GetClanByIndex(IntPtr _, int iClan)
        {
            return SteamFriends.Instance.GetClanByIndex(iClan);
        }

        public string GetClanName(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.GetClanName((ulong)steamIDClan);
        }

        public string GetClanTag(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.GetClanTag((ulong)steamIDClan);
        }

        public bool GetClanActivityCounts(IntPtr _, CSteamID steamIDClan, int pnOnline, int pnInGame, int pnChatting)
        {
            return SteamFriends.Instance.GetClanActivityCounts((ulong)steamIDClan, ref pnOnline, ref pnInGame, ref pnChatting);
        }

        public SteamAPICall_t DownloadClanActivityCounts(IntPtr _, IntPtr psteamIDClans, int cClansToRequest)
        {
            return SteamFriends.Instance.DownloadClanActivityCounts(psteamIDClans, cClansToRequest);
        }

        public int GetFriendCountFromSource(IntPtr _, CSteamID steamIDSource)
        {
            return SteamFriends.Instance.GetFriendCountFromSource((ulong)steamIDSource);
        }

        public CSteamID GetFriendFromSourceByIndex(IntPtr _, CSteamID steamIDSource, int iFriend)
        {
            return SteamFriends.Instance.GetFriendFromSourceByIndex((ulong)steamIDSource, iFriend);
        }

        public bool IsUserInSource(IntPtr _, CSteamID steamIDUser, CSteamID steamIDSource)
        {
            return SteamFriends.Instance.IsUserInSource((ulong)steamIDUser, (ulong)steamIDSource);
        }

        public void SetInGameVoiceSpeaking(IntPtr _, CSteamID steamIDUser, bool bSpeaking)
        {
            SteamFriends.Instance.SetInGameVoiceSpeaking((ulong)steamIDUser, bSpeaking);
        }

        public void ActivateGameOverlay(IntPtr _, string pchDialog)
        {
            SteamFriends.Instance.ActivateGameOverlay(pchDialog);
        }

        public void ActivateGameOverlayToUser(IntPtr _, string pchDialog, CSteamID steamID)
        {
            SteamFriends.Instance.ActivateGameOverlayToUser(pchDialog, (ulong)steamID);
        }

        public void ActivateGameOverlayToWebPage(IntPtr _, string pchURL)
        {
            SteamFriends.Instance.ActivateGameOverlayToWebPage(pchURL, 0);
        }

        public void ActivateGameOverlayToStore(IntPtr _, AppId_t nAppID, EOverlayToStoreFlag eFlag)
        {
            SteamFriends.Instance.ActivateGameOverlayToStore(nAppID, (int)eFlag);
        }

        public void SetPlayedWith(IntPtr _, CSteamID steamIDUserPlayedWith)
        {
            SteamFriends.Instance.SetPlayedWith((ulong)steamIDUserPlayedWith);
        }

        public void ActivateGameOverlayInviteDialog(IntPtr _, CSteamID steamIDLobby)
        {
            SteamFriends.Instance.ActivateGameOverlayInviteDialog((ulong)steamIDLobby);
        }

        public int GetSmallFriendAvatar(IntPtr _, ulong steamIDFriend)
        {
            return SteamFriends.Instance.GetSmallFriendAvatar(steamIDFriend);
        }

        public int GetMediumFriendAvatar(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamFriends.Instance.GetMediumFriendAvatar((ulong)steamIDFriend);
        }

        public int GetLargeFriendAvatar(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamFriends.Instance.GetLargeFriendAvatar((ulong)steamIDFriend);
        }

        public bool RequestUserInformation(IntPtr _, CSteamID steamIDUser, bool bRequireNameOnly)
        {
            return SteamFriends.Instance.RequestUserInformation((ulong)steamIDUser, bRequireNameOnly);
        }

        public SteamAPICall_t RequestClanOfficerList(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.RequestClanOfficerList((ulong)steamIDClan);
        }

        public CSteamID GetClanOwner(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.GetClanOwner((ulong)steamIDClan);
        }

        public int GetClanOfficerCount(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.GetClanOfficerCount((ulong)steamIDClan);
        }

        public CSteamID GetClanOfficerByIndex(IntPtr _, CSteamID steamIDClan, int iOfficer)
        {
            return SteamFriends.Instance.GetClanOfficerByIndex((ulong)steamIDClan, iOfficer);
        }

        public uint32 GetUserRestrictions(IntPtr _)
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

        public string GetFriendRichPresence(IntPtr _, CSteamID steamIDFriend, string pchKey)
        {
            return SteamFriends.Instance.GetFriendRichPresence((ulong)steamIDFriend, pchKey);
        }

        public int GetFriendRichPresenceKeyCount(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendRichPresenceKeyCount((ulong)steamIDFriend);
        }

        public string GetFriendRichPresenceKeyByIndex(IntPtr _, CSteamID steamIDFriend, int iKey)
        {
            return SteamFriends.Instance.GetFriendRichPresenceKeyByIndex((ulong)steamIDFriend, iKey);
        }

        public void RequestFriendRichPresence(IntPtr _, CSteamID steamIDFriend)
        {
            SteamFriends.Instance.RequestFriendRichPresence((ulong)steamIDFriend);
        }

        public bool InviteUserToGame(IntPtr _, CSteamID steamIDFriend, string pchConnectString)
        {
            return SteamFriends.Instance.InviteUserToGame((ulong)steamIDFriend, pchConnectString);
        }

        public int GetCoplayFriendCount(IntPtr _)
        {
            return SteamFriends.Instance.GetCoplayFriendCount();
        }

        public CSteamID GetCoplayFriend(IntPtr _, int iCoplayFriend)
        {
            return SteamFriends.Instance.GetCoplayFriend(iCoplayFriend);
        }

        public int GetFriendCoplayTime(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendCoplayTime((ulong)steamIDFriend);
        }

        public AppId_t GetFriendCoplayGame(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendCoplayGame((ulong)steamIDFriend);
        }

        public SteamAPICall_t JoinClanChatRoom(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.JoinClanChatRoom((ulong)steamIDClan);
        }

        public bool LeaveClanChatRoom(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.LeaveClanChatRoom((ulong)steamIDClan);
        }

        public int GetClanChatMemberCount(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.GetClanChatMemberCount((ulong)steamIDClan);
        }

        public CSteamID GetChatMemberByIndex(IntPtr _, CSteamID steamIDClan, int iUser)
        {
            return SteamFriends.Instance.GetChatMemberByIndex((ulong)steamIDClan, iUser);
        }

        public bool SendClanChatMessage(IntPtr _, CSteamID steamIDClanChat, string pchText)
        {
            return SteamFriends.Instance.SendClanChatMessage((ulong)steamIDClanChat, pchText);
        }

        public int GetClanChatMessage(IntPtr _, CSteamID steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, EChatEntryType peChatEntryType, ref ulong[] psteamidChatter )
        {
            return SteamFriends.Instance.GetClanChatMessage((ulong)steamIDClanChat, iMessage, prgchText, cchTextMax, (int)peChatEntryType, ref psteamidChatter);
        }

        public bool IsClanChatAdmin(IntPtr _, CSteamID steamIDClanChat, CSteamID steamIDUser)
        {
            return SteamFriends.Instance.IsClanChatAdmin((ulong)steamIDClanChat, (ulong)steamIDUser);
        }

        public bool IsClanChatWindowOpenInSteam(IntPtr _, CSteamID steamIDClanChat)
        {
            return SteamFriends.Instance.IsClanChatWindowOpenInSteam((ulong)steamIDClanChat);
        }

        public bool OpenClanChatWindowInSteam(IntPtr _, CSteamID steamIDClanChat)
        {
            return SteamFriends.Instance.OpenClanChatWindowInSteam((ulong)steamIDClanChat);
        }

        public bool CloseClanChatWindowInSteam(IntPtr _, CSteamID steamIDClanChat)
        {
            return SteamFriends.Instance.CloseClanChatWindowInSteam((ulong)steamIDClanChat);
        }

        public bool SetListenForFriendsMessages(IntPtr _, bool bInterceptEnabled)
        {
            return SteamFriends.Instance.SetListenForFriendsMessages(bInterceptEnabled);
        }

        public bool ReplyToFriendMessage(IntPtr _, CSteamID steamIDFriend, string pchMsgToSend)
        {
            return SteamFriends.Instance.ReplyToFriendMessage((ulong)steamIDFriend, pchMsgToSend);
        }

        public int GetFriendMessage(IntPtr _, CSteamID steamIDFriend, int iMessageID, IntPtr pvData, int cubData, EChatEntryType peChatEntryType)
        {
            return SteamFriends.Instance.GetFriendMessage((ulong)steamIDFriend, iMessageID, pvData, cubData, (int)peChatEntryType);
        }

        public SteamAPICall_t GetFollowerCount(IntPtr _, CSteamID steamID)
        {
            return SteamFriends.Instance.GetFollowerCount((ulong)steamID);
        }

        public SteamAPICall_t IsFollowing(IntPtr _, CSteamID steamID)
        {
            return SteamFriends.Instance.IsFollowing((ulong)steamID);
        }

        public SteamAPICall_t EnumerateFollowingList(IntPtr _, uint32 unStartIndex)
        {
            return SteamFriends.Instance.EnumerateFollowingList(unStartIndex);
        }

        public bool IsClanPublic(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.IsClanPublic((ulong)steamIDClan);
        }

        public bool IsClanOfficialGameGroup(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.IsClanOfficialGameGroup((ulong)steamIDClan);
        }
    }
}
