using System;

using SteamAPICall_t = System.UInt64;
using FriendsGroupID_t = System.UInt16;
using uint16 = System.UInt16;
using uint32 = System.UInt32;
using AppId_t = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamFriends015")]
    public class SteamFriends015 : ISteamInterface
    {
        public string GetPersonaName(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetPersonaName();
        }

        public SteamAPICall_t SetPersonaName(IntPtr _, string pchPersonaName)
        {
            return SteamEmulator.SteamFriends.SetPersonaName(pchPersonaName);
        }

        public EPersonaState GetPersonaState(IntPtr _)
        {
            return (EPersonaState)SteamEmulator.SteamFriends.GetPersonaState();
        }

        public int GetFriendCount(IntPtr _, int iFriendFlags)
        {
            return SteamEmulator.SteamFriends.GetFriendCount(iFriendFlags);
        }

        public CSteamID GetFriendByIndex(IntPtr _, int iFriend, int iFriendFlags)
        {
            return SteamEmulator.SteamFriends.GetFriendByIndex(iFriend, iFriendFlags);
        }

        public EFriendRelationship GetFriendRelationship(IntPtr _, CSteamID steamIDFriend)
        {
            return (EFriendRelationship)SteamEmulator.SteamFriends.GetFriendRelationship((ulong)steamIDFriend);
        }

        public EPersonaState GetFriendPersonaState(IntPtr _, CSteamID steamIDFriend)
        {
            return (EPersonaState)SteamEmulator.SteamFriends.GetFriendPersonaState((ulong)steamIDFriend);
        }

        public string GetFriendPersonaName(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendPersonaName((ulong)steamIDFriend);
        }

        public bool GetFriendGamePlayed(IntPtr _, CSteamID steamIDFriend, IntPtr pFriendGameInfo )
        {
            return SteamEmulator.SteamFriends.GetFriendGamePlayed((ulong)steamIDFriend, pFriendGameInfo);
        }

        public string GetFriendPersonaNameHistory(IntPtr _, CSteamID steamIDFriend, int iPersonaName)
        {
            return SteamEmulator.SteamFriends.GetFriendPersonaNameHistory((ulong)steamIDFriend, iPersonaName);
        }

        public int GetFriendSteamLevel(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendSteamLevel((ulong)steamIDFriend);
        }

        public string GetPlayerNickname(IntPtr _, CSteamID steamIDPlayer)
        {
            return SteamEmulator.SteamFriends.GetPlayerNickname((ulong)steamIDPlayer);
        }

        public int GetFriendsGroupCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupCount();
        }

        public FriendsGroupID_t GetFriendsGroupIDByIndex(IntPtr _, int iFG)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupIDByIndex(iFG);
        }

        public string GetFriendsGroupName(IntPtr _, FriendsGroupID_t friendsGroupID)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupName(friendsGroupID);
        }

        public int GetFriendsGroupMembersCount(IntPtr _, FriendsGroupID_t friendsGroupID)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupMembersCount(friendsGroupID);
        }

        public void GetFriendsGroupMembersList(IntPtr _, FriendsGroupID_t friendsGroupID, IntPtr pOutSteamIDMembers, int nMembersCount)
        {
            SteamEmulator.SteamFriends.GetFriendsGroupMembersList(friendsGroupID, pOutSteamIDMembers, nMembersCount);
        }

        public bool HasFriend(IntPtr _, CSteamID steamIDFriend, int iFriendFlags)
        {
            return SteamEmulator.SteamFriends.HasFriend((ulong)steamIDFriend, iFriendFlags);
        }

        public int GetClanCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetClanCount();
        }

        public CSteamID GetClanByIndex(IntPtr _, int iClan)
        {
            return SteamEmulator.SteamFriends.GetClanByIndex(iClan);
        }

        public string GetClanName(IntPtr _, CSteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.GetClanName((ulong)steamIDClan);
        }

        public string GetClanTag(IntPtr _, CSteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.GetClanTag((ulong)steamIDClan);
        }

        public bool GetClanActivityCounts(IntPtr _, CSteamID steamIDClan, int pnOnline, int pnInGame, int pnChatting)
        {
            return SteamEmulator.SteamFriends.GetClanActivityCounts((ulong)steamIDClan, ref pnOnline, ref pnInGame, ref pnChatting);
        }

        public SteamAPICall_t DownloadClanActivityCounts(IntPtr _, IntPtr psteamIDClans, int cClansToRequest)
        {
            return SteamEmulator.SteamFriends.DownloadClanActivityCounts(psteamIDClans, cClansToRequest);
        }

        public int GetFriendCountFromSource(IntPtr _, CSteamID steamIDSource)
        {
            return SteamEmulator.SteamFriends.GetFriendCountFromSource((ulong)steamIDSource);
        }

        public CSteamID GetFriendFromSourceByIndex(IntPtr _, CSteamID steamIDSource, int iFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendFromSourceByIndex((ulong)steamIDSource, iFriend);
        }

        public bool IsUserInSource(IntPtr _, CSteamID steamIDUser, CSteamID steamIDSource)
        {
            return SteamEmulator.SteamFriends.IsUserInSource((ulong)steamIDUser, (ulong)steamIDSource);
        }

        public void SetInGameVoiceSpeaking(IntPtr _, CSteamID steamIDUser, bool bSpeaking)
        {
            SteamEmulator.SteamFriends.SetInGameVoiceSpeaking((ulong)steamIDUser, bSpeaking);
        }

        public void ActivateGameOverlay(IntPtr _, string pchDialog)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlay(pchDialog);
        }

        public void ActivateGameOverlayToUser(IntPtr _, string pchDialog, CSteamID steamID)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayToUser(pchDialog, (ulong)steamID);
        }

        public void ActivateGameOverlayToWebPage(IntPtr _, string pchURL)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayToWebPage(pchURL, 0);
        }

        public void ActivateGameOverlayToStore(IntPtr _, AppId_t nAppID, EOverlayToStoreFlag eFlag)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayToStore(nAppID, (int)eFlag);
        }

        public void SetPlayedWith(IntPtr _, CSteamID steamIDUserPlayedWith)
        {
            SteamEmulator.SteamFriends.SetPlayedWith((ulong)steamIDUserPlayedWith);
        }

        public void ActivateGameOverlayInviteDialog(IntPtr _, CSteamID steamIDLobby)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayInviteDialog((ulong)steamIDLobby);
        }

        public int GetSmallFriendAvatar(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetSmallFriendAvatar((ulong)steamIDFriend);
        }

        public int GetMediumFriendAvatar(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetMediumFriendAvatar((ulong)steamIDFriend);
        }

        public int GetLargeFriendAvatar(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetLargeFriendAvatar((ulong)steamIDFriend);
        }

        public bool RequestUserInformation(IntPtr _, CSteamID steamIDUser, bool bRequireNameOnly)
        {
            return SteamEmulator.SteamFriends.RequestUserInformation((ulong)steamIDUser, bRequireNameOnly);
        }

        public SteamAPICall_t RequestClanOfficerList(IntPtr _, CSteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.RequestClanOfficerList((ulong)steamIDClan);
        }

        public CSteamID GetClanOwner(IntPtr _, CSteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.GetClanOwner((ulong)steamIDClan);
        }

        public int GetClanOfficerCount(IntPtr _, CSteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.GetClanOfficerCount((ulong)steamIDClan);
        }

        public CSteamID GetClanOfficerByIndex(IntPtr _, CSteamID steamIDClan, int iOfficer)
        {
            return SteamEmulator.SteamFriends.GetClanOfficerByIndex((ulong)steamIDClan, iOfficer);
        }

        public uint32 GetUserRestrictions(IntPtr _)
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

        public string GetFriendRichPresence(IntPtr _, CSteamID steamIDFriend, string pchKey)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresence((ulong)steamIDFriend, pchKey);
        }

        public int GetFriendRichPresenceKeyCount(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyCount((ulong)steamIDFriend);
        }

        public string GetFriendRichPresenceKeyByIndex(IntPtr _, CSteamID steamIDFriend, int iKey)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyByIndex((ulong)steamIDFriend, iKey);
        }

        public void RequestFriendRichPresence(IntPtr _, CSteamID steamIDFriend)
        {
            SteamEmulator.SteamFriends.RequestFriendRichPresence((ulong)steamIDFriend);
        }

        public bool InviteUserToGame(IntPtr _, CSteamID steamIDFriend, string pchConnectString)
        {
            return SteamEmulator.SteamFriends.InviteUserToGame((ulong)steamIDFriend, pchConnectString);
        }

        public int GetCoplayFriendCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetCoplayFriendCount();
        }

        public CSteamID GetCoplayFriend(IntPtr _, int iCoplayFriend)
        {
            return SteamEmulator.SteamFriends.GetCoplayFriend(iCoplayFriend);
        }

        public int GetFriendCoplayTime(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendCoplayTime((ulong)steamIDFriend);
        }

        public AppId_t GetFriendCoplayGame(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamEmulator.SteamFriends.GetFriendCoplayGame((ulong)steamIDFriend);
        }

        public SteamAPICall_t JoinClanChatRoom(IntPtr _, CSteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.JoinClanChatRoom((ulong)steamIDClan);
        }

        public bool LeaveClanChatRoom(IntPtr _, CSteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.LeaveClanChatRoom((ulong)steamIDClan);
        }

        public int GetClanChatMemberCount(IntPtr _, CSteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.GetClanChatMemberCount((ulong)steamIDClan);
        }

        public CSteamID GetChatMemberByIndex(IntPtr _, CSteamID steamIDClan, int iUser)
        {
            return SteamEmulator.SteamFriends.GetChatMemberByIndex((ulong)steamIDClan, iUser);
        }

        public bool SendClanChatMessage(IntPtr _, CSteamID steamIDClanChat, string pchText)
        {
            return SteamEmulator.SteamFriends.SendClanChatMessage((ulong)steamIDClanChat, pchText);
        }

        public int GetClanChatMessage(IntPtr _, CSteamID steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, EChatEntryType peChatEntryType, ref ulong[] psteamidChatter )
        {
            return SteamEmulator.SteamFriends.GetClanChatMessage((ulong)steamIDClanChat, iMessage, prgchText, cchTextMax, (int)peChatEntryType, ref psteamidChatter);
        }

        public bool IsClanChatAdmin(IntPtr _, CSteamID steamIDClanChat, CSteamID steamIDUser)
        {
            return SteamEmulator.SteamFriends.IsClanChatAdmin((ulong)steamIDClanChat, (ulong)steamIDUser);
        }

        public bool IsClanChatWindowOpenInSteam(IntPtr _, CSteamID steamIDClanChat)
        {
            return SteamEmulator.SteamFriends.IsClanChatWindowOpenInSteam((ulong)steamIDClanChat);
        }

        public bool OpenClanChatWindowInSteam(IntPtr _, CSteamID steamIDClanChat)
        {
            return SteamEmulator.SteamFriends.OpenClanChatWindowInSteam((ulong)steamIDClanChat);
        }

        public bool CloseClanChatWindowInSteam(IntPtr _, CSteamID steamIDClanChat)
        {
            return SteamEmulator.SteamFriends.CloseClanChatWindowInSteam((ulong)steamIDClanChat);
        }

        public bool SetListenForFriendsMessages(IntPtr _, bool bInterceptEnabled)
        {
            return SteamEmulator.SteamFriends.SetListenForFriendsMessages(bInterceptEnabled);
        }

        public bool ReplyToFriendMessage(IntPtr _, CSteamID steamIDFriend, string pchMsgToSend)
        {
            return SteamEmulator.SteamFriends.ReplyToFriendMessage((ulong)steamIDFriend, pchMsgToSend);
        }

        public int GetFriendMessage(IntPtr _, CSteamID steamIDFriend, int iMessageID, IntPtr pvData, int cubData, EChatEntryType peChatEntryType)
        {
            return SteamEmulator.SteamFriends.GetFriendMessage((ulong)steamIDFriend, iMessageID, pvData, cubData, (int)peChatEntryType);
        }

        public SteamAPICall_t GetFollowerCount(IntPtr _, CSteamID steamID)
        {
            return SteamEmulator.SteamFriends.GetFollowerCount((ulong)steamID);
        }

        public SteamAPICall_t IsFollowing(IntPtr _, CSteamID steamID)
        {
            return SteamEmulator.SteamFriends.IsFollowing((ulong)steamID);
        }

        public SteamAPICall_t EnumerateFollowingList(IntPtr _, uint32 unStartIndex)
        {
            return SteamEmulator.SteamFriends.EnumerateFollowingList(unStartIndex);
        }

        public bool IsClanPublic(IntPtr _, CSteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.IsClanPublic((ulong)steamIDClan);
        }

        public bool IsClanOfficialGameGroup(IntPtr _, CSteamID steamIDClan)
        {
            return SteamEmulator.SteamFriends.IsClanOfficialGameGroup((ulong)steamIDClan);
        }
    }
}
