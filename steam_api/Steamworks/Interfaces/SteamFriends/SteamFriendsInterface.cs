using SKYNET.Helpers;
using SKYNET.Steamworks.Implementation;
using SKYNET.Steamworks.Types;
using System.Runtime.InteropServices;
using System;

using AppId_t = System.UInt32;
using FriendsGroupID_t = System.Int16;
using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("SteamFriends005",
        "GetPersonaName__V005", "SetPersonaNameOld", "GetPersonaState__V005", "GetFriendCount",
        "GetFriendByIndex", "GetFriendRelationship__V005", "GetFriendPersonaState__V005", "GetFriendPersonaName__V005",
        "GetFriendAvatar", "GetFriendGamePlayed", "GetFriendPersonaNameHistory__V005", "HasFriend",
        "GetClanCount", "GetClanByIndex", "GetClanName__V005", "GetFriendCountFromSource",
        "GetFriendFromSourceByIndex", "IsUserInSource", "SetInGameVoiceSpeaking", "ActivateGameOverlay",
        "ActivateGameOverlayToUser", "ActivateGameOverlayToWebPage__V005", "ActivateGameOverlayToStore__V005", "SetPlayedWith")]
    [InterfaceLayout("SteamFriends015",
        "GetPersonaName__V005", "SetPersonaName", "GetPersonaState__V005", "GetFriendCount",
        "GetFriendByIndex", "GetFriendRelationship__V015", "GetFriendPersonaState__V015", "GetFriendPersonaName__V015",
        "GetFriendGamePlayed", "GetFriendPersonaNameHistory__V005", "GetFriendSteamLevel__V015", "GetPlayerNickname__V015",
        "GetFriendsGroupCount", "GetFriendsGroupIDByIndex", "GetFriendsGroupName__V015", "GetFriendsGroupMembersCount",
        "GetFriendsGroupMembersList", "HasFriend__V015", "GetClanCount", "GetClanByIndex",
        "GetClanName__V015", "GetClanTag__V015", "GetClanActivityCounts__V015", "DownloadClanActivityCounts",
        "GetFriendCountFromSource__V015", "GetFriendFromSourceByIndex__V015", "IsUserInSource__V015", "SetInGameVoiceSpeaking__V015",
        "ActivateGameOverlay", "ActivateGameOverlayToUser__V015", "ActivateGameOverlayToWebPage__V005", "ActivateGameOverlayToStore__V015",
        "SetPlayedWith__V015", "ActivateGameOverlayInviteDialog__V015", "GetSmallFriendAvatar", "GetMediumFriendAvatar__V015",
        "GetLargeFriendAvatar__V015", "RequestUserInformation__V015", "RequestClanOfficerList__V015", "GetClanOwner__V015",
        "GetClanOfficerCount__V015", "GetClanOfficerByIndex__V015", "GetUserRestrictions", "SetRichPresence",
        "ClearRichPresence", "GetFriendRichPresence__V015", "GetFriendRichPresenceKeyCount__V015", "GetFriendRichPresenceKeyByIndex__V015",
        "RequestFriendRichPresence__V015", "InviteUserToGame__V015", "GetCoplayFriendCount", "GetCoplayFriend",
        "GetFriendCoplayTime__V015", "GetFriendCoplayGame__V015", "JoinClanChatRoom__V015", "LeaveClanChatRoom__V015",
        "GetClanChatMemberCount__V015", "GetChatMemberByIndex__V015", "SendClanChatMessage__V015", "GetClanChatMessage__V015",
        "IsClanChatAdmin__V015", "IsClanChatWindowOpenInSteam__V015", "OpenClanChatWindowInSteam__V015", "CloseClanChatWindowInSteam__V015",
        "SetListenForFriendsMessages", "ReplyToFriendMessage__V015", "GetFriendMessage__V015", "GetFollowerCount__V015",
        "IsFollowing__V015", "EnumerateFollowingList", "IsClanPublic__V015", "IsClanOfficialGameGroup__V015")]
    [InterfaceLayout("SteamFriends017",
        "GetPersonaName", "SetPersonaName", "GetPersonaState", "GetFriendCount",
        "GetFriendByIndex", "GetFriendRelationship", "GetFriendPersonaState", "GetFriendPersonaName",
        "GetFriendGamePlayed", "GetFriendPersonaNameHistory", "GetFriendSteamLevel", "GetPlayerNickname",
        "GetFriendsGroupCount", "GetFriendsGroupIDByIndex", "GetFriendsGroupName", "GetFriendsGroupMembersCount",
        "GetFriendsGroupMembersList", "HasFriend", "GetClanCount", "GetClanByIndex",
        "GetClanName", "GetClanTag", "GetClanActivityCounts", "DownloadClanActivityCounts",
        "GetFriendCountFromSource", "GetFriendFromSourceByIndex", "IsUserInSource", "SetInGameVoiceSpeaking",
        "ActivateGameOverlay", "ActivateGameOverlayToUser", "ActivateGameOverlayToWebPage", "ActivateGameOverlayToStore",
        "SetPlayedWith", "ActivateGameOverlayInviteDialog", "GetSmallFriendAvatar", "GetMediumFriendAvatar",
        "GetLargeFriendAvatar", "RequestUserInformation", "RequestClanOfficerList", "GetClanOwner",
        "GetClanOfficerCount", "GetClanOfficerByIndex", "GetUserRestrictions", "SetRichPresence",
        "ClearRichPresence", "GetFriendRichPresence", "GetFriendRichPresenceKeyCount", "GetFriendRichPresenceKeyByIndex",
        "RequestFriendRichPresence", "InviteUserToGame", "GetCoplayFriendCount", "GetCoplayFriend",
        "GetFriendCoplayTime", "GetFriendCoplayGame", "JoinClanChatRoom", "LeaveClanChatRoom",
        "GetClanChatMemberCount", "GetChatMemberByIndex", "SendClanChatMessage", "GetClanChatMessage__V017",
        "IsClanChatAdmin", "IsClanChatWindowOpenInSteam", "OpenClanChatWindowInSteam", "CloseClanChatWindowInSteam",
        "SetListenForFriendsMessages", "ReplyToFriendMessage", "GetFriendMessage__V017", "GetFollowerCount",
        "IsFollowing", "EnumerateFollowingList", "IsClanPublic", "IsClanOfficialGameGroup",
        "GetNumChatsWithUnreadPriorityMessages", "ActivateGameOverlayRemotePlayTogetherInviteDialog", "RegisterProtocolInOverlayBrowser", "ActivateGameOverlayInviteDialogConnectString",
        "RequestEquippedProfileItems", "BHasEquippedProfileItem", "GetProfileItemPropertyString", "GetProfileItemPropertyUint")]
    [InterfaceLayout("SteamFriends018",
        "GetPersonaName", "GetPersonaState", "GetFriendCount", "GetFriendByIndex",
        "GetFriendRelationship", "GetFriendPersonaState", "GetFriendPersonaName", "GetFriendGamePlayed",
        "GetFriendPersonaNameHistory", "GetFriendSteamLevel", "GetPlayerNickname", "GetFriendsGroupCount",
        "GetFriendsGroupIDByIndex", "GetFriendsGroupName", "GetFriendsGroupMembersCount", "GetFriendsGroupMembersList__V018",
        "HasFriend", "GetClanCount", "GetClanByIndex", "GetClanName",
        "GetClanTag", "GetClanActivityCounts", "DownloadClanActivityCounts", "GetFriendCountFromSource",
        "GetFriendFromSourceByIndex", "IsUserInSource", "SetInGameVoiceSpeaking", "ActivateGameOverlay",
        "ActivateGameOverlayToUser", "ActivateGameOverlayToWebPage", "ActivateGameOverlayToStore", "SetPlayedWith",
        "ActivateGameOverlayInviteDialog", "GetSmallFriendAvatar", "GetMediumFriendAvatar", "GetLargeFriendAvatar",
        "RequestUserInformation", "RequestClanOfficerList", "GetClanOwner", "GetClanOfficerCount",
        "GetClanOfficerByIndex", "SetRichPresence", "ClearRichPresence", "GetFriendRichPresence",
        "GetFriendRichPresenceKeyCount", "GetFriendRichPresenceKeyByIndex", "RequestFriendRichPresence", "InviteUserToGame",
        "GetCoplayFriendCount", "GetCoplayFriend", "GetFriendCoplayTime", "GetFriendCoplayGame",
        "JoinClanChatRoom", "LeaveClanChatRoom", "GetClanChatMemberCount", "GetChatMemberByIndex",
        "SendClanChatMessage", "GetClanChatMessage", "IsClanChatAdmin", "IsClanChatWindowOpenInSteam",
        "OpenClanChatWindowInSteam", "CloseClanChatWindowInSteam", "SetListenForFriendsMessages", "ReplyToFriendMessage",
        "GetFriendMessage", "GetFollowerCount", "IsFollowing", "EnumerateFollowingList",
        "IsClanPublic", "IsClanOfficialGameGroup", "GetNumChatsWithUnreadPriorityMessages", "ActivateGameOverlayRemotePlayTogetherInviteDialog",
        "RegisterProtocolInOverlayBrowser", "ActivateGameOverlayInviteDialogConnectString", "RequestEquippedProfileItems", "BHasEquippedProfileItem",
        "GetProfileItemPropertyString", "GetProfileItemPropertyUint")]
    public class SteamFriendsInterface : ISteamInterface
    {
        private static void WriteInt32(IntPtr destination, int value)
        {
            if (destination != IntPtr.Zero)
            {
                System.Runtime.InteropServices.Marshal.WriteInt32(destination, value);
            }
        }
        private static void WriteUInt64(IntPtr destination, ulong value)
        {
            if (destination != IntPtr.Zero)
            {
                System.Runtime.InteropServices.Marshal.WriteInt64(destination, unchecked((long)value));
            }
        }

        public string GetPersonaName__V005(IntPtr _)
        {
            return SteamFriends.Instance.GetPersonaName();
        }

        public IntPtr GetPersonaName(IntPtr _) { return NativeStringCache.ToUtf8Ptr(SteamFriends.Instance.GetPersonaName()); }

        public void SetPersonaNameOld(IntPtr _, string pchPersonaName)
        {
            SteamFriends.Instance.SetPersonaName(pchPersonaName);
        }

        public EPersonaState GetPersonaState__V005(IntPtr _)
        {
            return (EPersonaState)SteamFriends.Instance.GetPersonaState();
        }

        public int GetPersonaState(IntPtr _) { return SteamFriends.Instance.GetPersonaState(); }

        public int GetFriendCount(IntPtr _, int iFriendFlags) { return SteamFriends.Instance.GetFriendCount(iFriendFlags); }

        public IntPtr GetFriendByIndex(IntPtr _, IntPtr ret, int iFriend, int iFriendFlags) { return NativeSteamId.Write(ret, SteamFriends.Instance.GetFriendByIndex(iFriend, iFriendFlags)); }

        public EFriendRelationship GetFriendRelationship__V005(IntPtr _, ulong steamIDFriend) =>
            (EFriendRelationship)SteamFriends.Instance.GetFriendRelationship(steamIDFriend);

        public EFriendRelationship GetFriendRelationship__V015(IntPtr _, CSteamID steamIDFriend)
        {
            return (EFriendRelationship) SteamFriends.Instance.GetFriendRelationship((ulong)steamIDFriend);
        }

        public int GetFriendRelationship(IntPtr _, ulong steamID) { return SteamFriends.Instance.GetFriendRelationship(steamID); }

        public EPersonaState GetFriendPersonaState__V005(IntPtr _, ulong steamIDFriend) =>
            (EPersonaState)SteamFriends.Instance.GetFriendPersonaState(steamIDFriend);

        public EPersonaState GetFriendPersonaState__V015(IntPtr _, CSteamID steamIDFriend)
        {
            return (EPersonaState) SteamFriends.Instance.GetFriendPersonaState((ulong)steamIDFriend);
        }

        public int GetFriendPersonaState(IntPtr _, ulong steamID) { return SteamFriends.Instance.GetFriendPersonaState(steamID); }

        public string GetFriendPersonaName__V005(IntPtr _, ulong steamIDFriend) =>
            SteamFriends.Instance.GetFriendPersonaName(steamIDFriend);

        public string GetFriendPersonaName__V015(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendPersonaName((ulong)steamIDFriend);
        }

        public IntPtr GetFriendPersonaName(IntPtr _, ulong steamID) { return NativeStringCache.ToUtf8Ptr(SteamFriends.Instance.GetFriendPersonaName(steamID)); }

        public int GetFriendAvatar(IntPtr _, ulong steamIDFriend, int eAvatarSize)
        {
            switch (eAvatarSize)
            {
                case 0:
                    return SteamFriends.Instance.GetSmallFriendAvatar(steamIDFriend);
                case 1:
                    return SteamFriends.Instance.GetMediumFriendAvatar(steamIDFriend);
                case 2:
                    return SteamFriends.Instance.GetLargeFriendAvatar(steamIDFriend);
                default:
                    return 0;
            }
        }

        public bool GetFriendGamePlayed(IntPtr _, ulong steamID, ref FriendGameInfo_t pFriendGameInfo) { return SteamFriends.Instance.GetFriendGamePlayed(steamID, ref pFriendGameInfo); }

        public string GetFriendPersonaNameHistory__V005(IntPtr _, ulong steamIDFriend, int iPersonaName)
        {
            return SteamFriends.Instance.GetFriendPersonaNameHistory(steamIDFriend, iPersonaName);
        }

        public IntPtr GetFriendPersonaNameHistory(IntPtr _, ulong steamID, int index) { return NativeStringCache.ToUtf8Ptr(SteamFriends.Instance.GetFriendPersonaNameHistory(steamID, index)); }

        public bool HasFriend(IntPtr _, ulong steamIDFriend, int iFriendFlags) { return SteamFriends.Instance.HasFriend(steamIDFriend, iFriendFlags); }

        public bool HasFriend__V015(IntPtr _, CSteamID steamIDFriend, int iFriendFlags)
        {
            return SteamFriends.Instance.HasFriend((ulong)steamIDFriend, iFriendFlags);
        }

        public int GetClanCount(IntPtr _) { return SteamFriends.Instance.GetClanCount(); }

        public IntPtr GetClanByIndex(IntPtr _, IntPtr ret, int iClan) { return NativeSteamId.Write(ret, SteamFriends.Instance.GetClanByIndex(iClan)); }

        public string GetClanName__V005(IntPtr _, ulong steamIDClan) =>
            SteamFriends.Instance.GetClanName(steamIDClan);

        public string GetClanName__V015(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.GetClanName((ulong)steamIDClan);
        }

        public IntPtr GetClanName(IntPtr _, ulong steamIDClan) { return NativeStringCache.ToUtf8Ptr(SteamFriends.Instance.GetClanName(steamIDClan)); }

        public int GetFriendCountFromSource(IntPtr _, ulong steamIDSource) { return SteamFriends.Instance.GetFriendCountFromSource(steamIDSource); }

        public int GetFriendCountFromSource__V015(IntPtr _, CSteamID steamIDSource)
        {
            return SteamFriends.Instance.GetFriendCountFromSource((ulong)steamIDSource);
        }

        public IntPtr GetFriendFromSourceByIndex(IntPtr _, IntPtr ret, ulong steamIDSource, int iFriend) { return NativeSteamId.Write(ret, SteamFriends.Instance.GetFriendFromSourceByIndex(steamIDSource, iFriend)); }

        public IntPtr GetFriendFromSourceByIndex__V015(IntPtr _, IntPtr ret, CSteamID steamIDSource, int iFriend)
        {
            return NativeSteamId.Write(ret, SteamFriends.Instance.GetFriendFromSourceByIndex((ulong)steamIDSource, iFriend));
        }

        public bool IsUserInSource(IntPtr _, ulong steamIDUser, ulong steamIDSource) { return SteamFriends.Instance.IsUserInSource(steamIDUser, steamIDSource); }

        public bool IsUserInSource__V015(IntPtr _, CSteamID steamIDUser, CSteamID steamIDSource)
        {
            return SteamFriends.Instance.IsUserInSource((ulong)steamIDUser, (ulong)steamIDSource);
        }

        public void SetInGameVoiceSpeaking(IntPtr _, ulong steamIDUser, bool bSpeaking) { SteamFriends.Instance.SetInGameVoiceSpeaking(steamIDUser, bSpeaking); }

        public void SetInGameVoiceSpeaking__V015(IntPtr _, CSteamID steamIDUser, bool bSpeaking)
        {
            SteamFriends.Instance.SetInGameVoiceSpeaking((ulong)steamIDUser, bSpeaking);
        }

        public void ActivateGameOverlay(IntPtr _, string pchDialog) { SteamFriends.Instance.ActivateGameOverlay(pchDialog); }

        public void ActivateGameOverlayToUser(IntPtr _, string pchDialog, ulong steamID) { SteamFriends.Instance.ActivateGameOverlayToUser(pchDialog, steamID); }

        public void ActivateGameOverlayToUser__V015(IntPtr _, string pchDialog, CSteamID steamID)
        {
            SteamFriends.Instance.ActivateGameOverlayToUser(pchDialog, (ulong)steamID);
        }

        public void ActivateGameOverlayToWebPage__V005(IntPtr _, string pchURL)
        {
            SteamFriends.Instance.ActivateGameOverlayToWebPage(pchURL, 0);
        }

        public void ActivateGameOverlayToWebPage(IntPtr _, string pchURL, int eMode) { SteamFriends.Instance.ActivateGameOverlayToWebPage(pchURL, eMode); }

        public void ActivateGameOverlayToStore__V005(IntPtr _, uint nAppID) =>
            SteamFriends.Instance.ActivateGameOverlayToStore(nAppID, 0);

        public void ActivateGameOverlayToStore__V015(IntPtr _, AppId_t nAppID, EOverlayToStoreFlag eFlag)
        {
            SteamFriends.Instance.ActivateGameOverlayToStore(nAppID, (int)eFlag);
        }

        public void ActivateGameOverlayToStore(IntPtr _, uint nAppID, int eFlag) { SteamFriends.Instance.ActivateGameOverlayToStore(nAppID, eFlag); }

        public void SetPlayedWith(IntPtr _, ulong steamIDUserPlayedWith) { SteamFriends.Instance.SetPlayedWith(steamIDUserPlayedWith); }

        public void SetPlayedWith__V015(IntPtr _, CSteamID steamIDUserPlayedWith)
        {
            SteamFriends.Instance.SetPlayedWith((ulong)steamIDUserPlayedWith);
        }

        public SteamAPICall_t SetPersonaName(IntPtr _, string name)
        {
            return SteamFriends.Instance.SetPersonaName(name);
        }

        public int GetFriendSteamLevel__V015(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendSteamLevel((ulong)steamIDFriend);
        }

        public int GetFriendSteamLevel(IntPtr _, ulong steamID) { return SteamFriends.Instance.GetFriendSteamLevel(steamID); }

        public string GetPlayerNickname__V015(IntPtr _, CSteamID steamIDPlayer)
        {
            return SteamFriends.Instance.GetPlayerNickname((ulong)steamIDPlayer);
        }

        public IntPtr GetPlayerNickname(IntPtr _, ulong steamID) { return NativeStringCache.ToUtf8PtrOrNull(SteamFriends.Instance.GetPlayerNickname(steamID)); }

        public int GetFriendsGroupCount(IntPtr _) { return SteamFriends.Instance.GetFriendsGroupCount(); }

        public FriendsGroupID_t GetFriendsGroupIDByIndex(IntPtr _, int iFG) { return SteamFriends.Instance.GetFriendsGroupIDByIndex(iFG); }

        public string GetFriendsGroupName__V015(IntPtr _, FriendsGroupID_t friendsGroupID)
        {
            return SteamFriends.Instance.GetFriendsGroupName(friendsGroupID);
        }

        public IntPtr GetFriendsGroupName(IntPtr _, FriendsGroupID_t friendsGroupID) { return NativeStringCache.ToUtf8Ptr(SteamFriends.Instance.GetFriendsGroupName(friendsGroupID)); }

        public int GetFriendsGroupMembersCount(IntPtr _, FriendsGroupID_t friendsGroupID) { return SteamFriends.Instance.GetFriendsGroupMembersCount(friendsGroupID); }

        public void GetFriendsGroupMembersList(IntPtr _, FriendsGroupID_t friendsGroupID, ref ulong[] pOutSteamIDMembers, int nMembersCount)
        {
            SteamFriends.Instance.GetFriendsGroupMembersList(friendsGroupID, ref pOutSteamIDMembers, nMembersCount);
        }

        public void GetFriendsGroupMembersList__V018(IntPtr _, FriendsGroupID_t friendsGroupID, IntPtr pOutSteamIDMembers, int nMembersCount) { SteamFriends.Instance.GetFriendsGroupMembersList(friendsGroupID, pOutSteamIDMembers, nMembersCount); }

        public string GetClanTag__V015(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.GetClanTag((ulong)steamIDClan);
        }

        public IntPtr GetClanTag(IntPtr _, ulong steamIDClan) { return NativeStringCache.ToUtf8Ptr(SteamFriends.Instance.GetClanTag(steamIDClan)); }

        public bool GetClanActivityCounts__V015(IntPtr _, CSteamID steamIDClan, IntPtr pnOnline, IntPtr pnInGame, IntPtr pnChatting)
        {
            int online = 0;
            int inGame = 0;
            int chatting = 0;
            bool result = SteamFriends.Instance.GetClanActivityCounts((ulong)steamIDClan, ref online, ref inGame, ref chatting);
            WriteInt32(pnOnline, online);
            WriteInt32(pnInGame, inGame);
            WriteInt32(pnChatting, chatting);
            return result;
        }

        public bool GetClanActivityCounts(IntPtr _, ulong steamIDClan, ref int pnOnline, ref int pnInGame, ref int pnChatting) { return SteamFriends.Instance.GetClanActivityCounts(steamIDClan, ref pnOnline, ref pnInGame, ref pnChatting); }

        public SteamAPICall_t DownloadClanActivityCounts(IntPtr _, IntPtr psteamIDClans, int cClansToRequest) { return SteamFriends.Instance.DownloadClanActivityCounts(psteamIDClans, cClansToRequest); }

        public void ActivateGameOverlayInviteDialog__V015(IntPtr _, CSteamID steamIDLobby)
        {
            SteamFriends.Instance.ActivateGameOverlayInviteDialog((ulong)steamIDLobby);
        }

        public void ActivateGameOverlayInviteDialog(IntPtr _, ulong steamIDLobby) { SteamFriends.Instance.ActivateGameOverlayInviteDialog(steamIDLobby); }

        public int GetSmallFriendAvatar(IntPtr _, ulong steamIDFriend) { return SteamFriends.Instance.GetSmallFriendAvatar(steamIDFriend); }

        public int GetMediumFriendAvatar__V015(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamFriends.Instance.GetMediumFriendAvatar((ulong)steamIDFriend);
        }

        public int GetMediumFriendAvatar(IntPtr _, ulong steamIDFriend) { return SteamFriends.Instance.GetMediumFriendAvatar(steamIDFriend); }

        public int GetLargeFriendAvatar__V015(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamFriends.Instance.GetLargeFriendAvatar((ulong)steamIDFriend);
        }

        public int GetLargeFriendAvatar(IntPtr _, ulong steamIDFriend) { return SteamFriends.Instance.GetLargeFriendAvatar(steamIDFriend); }

        public bool RequestUserInformation__V015(IntPtr _, CSteamID steamIDUser, bool bRequireNameOnly)
        {
            return SteamFriends.Instance.RequestUserInformation((ulong)steamIDUser, bRequireNameOnly);
        }

        public bool RequestUserInformation(IntPtr _, ulong steamIDUser, bool bRequireNameOnly) { return SteamFriends.Instance.RequestUserInformation(steamIDUser, bRequireNameOnly); }

        public SteamAPICall_t RequestClanOfficerList__V015(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.RequestClanOfficerList((ulong)steamIDClan);
        }

        public SteamAPICall_t RequestClanOfficerList(IntPtr _, ulong steamIDClan) { return SteamFriends.Instance.RequestClanOfficerList(steamIDClan); }

        public IntPtr GetClanOwner__V015(IntPtr _, IntPtr ret, CSteamID steamIDClan)
        {
            return NativeSteamId.Write(ret, SteamFriends.Instance.GetClanOwner((ulong)steamIDClan));
        }

        public IntPtr GetClanOwner(IntPtr _, IntPtr ret, ulong steamIDClan) { return NativeSteamId.Write(ret, SteamFriends.Instance.GetClanOwner(steamIDClan)); }

        public int GetClanOfficerCount__V015(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.GetClanOfficerCount((ulong)steamIDClan);
        }

        public int GetClanOfficerCount(IntPtr _, ulong steamIDClan) { return SteamFriends.Instance.GetClanOfficerCount(steamIDClan); }

        public IntPtr GetClanOfficerByIndex__V015(IntPtr _, IntPtr ret, CSteamID steamIDClan, int iOfficer)
        {
            return NativeSteamId.Write(ret, SteamFriends.Instance.GetClanOfficerByIndex((ulong)steamIDClan, iOfficer));
        }

        public IntPtr GetClanOfficerByIndex(IntPtr _, IntPtr ret, ulong steamIDClan, int iOfficer) { return NativeSteamId.Write(ret, SteamFriends.Instance.GetClanOfficerByIndex(steamIDClan, iOfficer)); }

        public UInt32 GetUserRestrictions(IntPtr _)
        {
            return SteamFriends.Instance.GetUserRestrictions();
        }

        public bool SetRichPresence(IntPtr _, string pchKey, string pchValue) { return SteamFriends.Instance.SetRichPresence(pchKey, pchValue); }

        public void ClearRichPresence(IntPtr _) { SteamFriends.Instance.ClearRichPresence(); }

        public string GetFriendRichPresence__V015(IntPtr _, CSteamID steamIDFriend, string pchKey)
        {
            return SteamFriends.Instance.GetFriendRichPresence((ulong)steamIDFriend, pchKey);
        }

        public IntPtr GetFriendRichPresence(IntPtr _, ulong steamIDFriend, string pchKey) { return NativeStringCache.ToUtf8Ptr(SteamFriends.Instance.GetFriendRichPresence(steamIDFriend, pchKey)); }

        public int GetFriendRichPresenceKeyCount__V015(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendRichPresenceKeyCount((ulong)steamIDFriend);
        }

        public int GetFriendRichPresenceKeyCount(IntPtr _, ulong steamIDFriend) { return SteamFriends.Instance.GetFriendRichPresenceKeyCount(steamIDFriend); }

        public string GetFriendRichPresenceKeyByIndex__V015(IntPtr _, CSteamID steamIDFriend, int iKey)
        {
            return SteamFriends.Instance.GetFriendRichPresenceKeyByIndex((ulong)steamIDFriend, iKey);
        }

        public IntPtr GetFriendRichPresenceKeyByIndex(IntPtr _, ulong steamIDFriend, int iKey) { return NativeStringCache.ToUtf8Ptr(SteamFriends.Instance.GetFriendRichPresenceKeyByIndex(steamIDFriend, iKey)); }

        public void RequestFriendRichPresence__V015(IntPtr _, CSteamID steamIDFriend)
        {
            SteamFriends.Instance.RequestFriendRichPresence((ulong)steamIDFriend);
        }

        public void RequestFriendRichPresence(IntPtr _, ulong steamIDFriend) { SteamFriends.Instance.RequestFriendRichPresence(steamIDFriend); }

        public bool InviteUserToGame__V015(IntPtr _, CSteamID steamIDFriend, string pchConnectString)
        {
            return SteamFriends.Instance.InviteUserToGame((ulong)steamIDFriend, pchConnectString);
        }

        public bool InviteUserToGame(IntPtr _, ulong steamIDFriend, string pchConnectString) { return SteamFriends.Instance.InviteUserToGame(steamIDFriend, pchConnectString); }

        public int GetCoplayFriendCount(IntPtr _) { return SteamFriends.Instance.GetCoplayFriendCount(); }

        public IntPtr GetCoplayFriend(IntPtr _, IntPtr ret, int iCoplayFriend) { return NativeSteamId.Write(ret, SteamFriends.Instance.GetCoplayFriend(iCoplayFriend)); }

        public int GetFriendCoplayTime__V015(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendCoplayTime((ulong)steamIDFriend);
        }

        public int GetFriendCoplayTime(IntPtr _, ulong steamIDFriend) { return SteamFriends.Instance.GetFriendCoplayTime(steamIDFriend); }

        public AppId_t GetFriendCoplayGame__V015(IntPtr _, CSteamID steamIDFriend)
        {
            return SteamFriends.Instance.GetFriendCoplayGame((ulong)steamIDFriend);
        }

        public uint GetFriendCoplayGame(IntPtr _, ulong steamIDFriend) { return SteamFriends.Instance.GetFriendCoplayGame(steamIDFriend); }

        public SteamAPICall_t JoinClanChatRoom__V015(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.JoinClanChatRoom((ulong)steamIDClan);
        }

        public SteamAPICall_t JoinClanChatRoom(IntPtr _, ulong steamIDClan) { return SteamFriends.Instance.JoinClanChatRoom(steamIDClan); }

        public bool LeaveClanChatRoom__V015(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.LeaveClanChatRoom((ulong)steamIDClan);
        }

        public bool LeaveClanChatRoom(IntPtr _, ulong steamIDClan) { return SteamFriends.Instance.LeaveClanChatRoom(steamIDClan); }

        public int GetClanChatMemberCount__V015(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.GetClanChatMemberCount((ulong)steamIDClan);
        }

        public int GetClanChatMemberCount(IntPtr _, ulong steamIDClan) { return SteamFriends.Instance.GetClanChatMemberCount(steamIDClan); }

        public IntPtr GetChatMemberByIndex__V015(IntPtr _, IntPtr ret, CSteamID steamIDClan, int iUser)
        {
            return NativeSteamId.Write(ret, SteamFriends.Instance.GetChatMemberByIndex((ulong)steamIDClan, iUser));
        }

        public IntPtr GetChatMemberByIndex(IntPtr _, IntPtr ret, ulong steamIDClan, int iUser) { return NativeSteamId.Write(ret, SteamFriends.Instance.GetChatMemberByIndex(steamIDClan, iUser)); }

        public bool SendClanChatMessage__V015(IntPtr _, CSteamID steamIDClanChat, string pchText)
        {
            return SteamFriends.Instance.SendClanChatMessage((ulong)steamIDClanChat, pchText);
        }

        public bool SendClanChatMessage(IntPtr _, ulong steamIDClanChat, string pchText) { return SteamFriends.Instance.SendClanChatMessage(steamIDClanChat, pchText); }

        public int GetClanChatMessage__V015(IntPtr _, CSteamID steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, IntPtr peChatEntryType, IntPtr psteamidChatter )
        {
            ulong[] chatter = new ulong[1];
            int result = SteamFriends.Instance.GetClanChatMessage((ulong)steamIDClanChat, iMessage, prgchText, cchTextMax, 0, ref chatter);
            WriteInt32(peChatEntryType, 0);
            WriteUInt64(psteamidChatter, chatter.Length > 0 ? chatter[0] : 0);
            return result;
        }

        public int GetClanChatMessage__V017(IntPtr _, ulong steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, IntPtr peChatEntryType, IntPtr psteamidChatter)
        {
            ulong[] chatter = new ulong[1];
            int result = SteamFriends.Instance.GetClanChatMessage(steamIDClanChat, iMessage, prgchText, cchTextMax, 0, ref chatter);
            if (peChatEntryType != IntPtr.Zero)
            {
                Marshal.WriteInt32(peChatEntryType, 0);
            }

            if (psteamidChatter != IntPtr.Zero)
            {
                Marshal.WriteInt64(psteamidChatter, unchecked((long)(chatter.Length > 0 ? chatter[0] : 0)));
            }

            return result;
        }

        public int GetClanChatMessage(IntPtr _, ulong steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, IntPtr peChatEntryType, IntPtr psteamidChatter) { return SteamFriends.Instance.GetClanChatMessage(steamIDClanChat, iMessage, prgchText, cchTextMax, peChatEntryType, psteamidChatter); }

        public bool IsClanChatAdmin__V015(IntPtr _, CSteamID steamIDClanChat, CSteamID steamIDUser)
        {
            return SteamFriends.Instance.IsClanChatAdmin((ulong)steamIDClanChat, (ulong)steamIDUser);
        }

        public bool IsClanChatAdmin(IntPtr _, ulong steamIDClanChat, ulong steamIDUser) { return SteamFriends.Instance.IsClanChatAdmin(steamIDClanChat, steamIDUser); }

        public bool IsClanChatWindowOpenInSteam__V015(IntPtr _, CSteamID steamIDClanChat)
        {
            return SteamFriends.Instance.IsClanChatWindowOpenInSteam((ulong)steamIDClanChat);
        }

        public bool IsClanChatWindowOpenInSteam(IntPtr _, ulong steamIDClanChat) { return SteamFriends.Instance.IsClanChatWindowOpenInSteam(steamIDClanChat); }

        public bool OpenClanChatWindowInSteam__V015(IntPtr _, CSteamID steamIDClanChat)
        {
            return SteamFriends.Instance.OpenClanChatWindowInSteam((ulong)steamIDClanChat);
        }

        public bool OpenClanChatWindowInSteam(IntPtr _, ulong steamIDClanChat) { return SteamFriends.Instance.OpenClanChatWindowInSteam(steamIDClanChat); }

        public bool CloseClanChatWindowInSteam__V015(IntPtr _, CSteamID steamIDClanChat)
        {
            return SteamFriends.Instance.CloseClanChatWindowInSteam((ulong)steamIDClanChat);
        }

        public bool CloseClanChatWindowInSteam(IntPtr _, ulong steamIDClanChat) { return SteamFriends.Instance.CloseClanChatWindowInSteam(steamIDClanChat); }

        public bool SetListenForFriendsMessages(IntPtr _, bool bInterceptEnabled) { return SteamFriends.Instance.SetListenForFriendsMessages(bInterceptEnabled); }

        public bool ReplyToFriendMessage__V015(IntPtr _, CSteamID steamIDFriend, string pchMsgToSend)
        {
            return SteamFriends.Instance.ReplyToFriendMessage((ulong)steamIDFriend, pchMsgToSend);
        }

        public bool ReplyToFriendMessage(IntPtr _, ulong steamIDFriend, string pchMsgToSend) { return SteamFriends.Instance.ReplyToFriendMessage(steamIDFriend, pchMsgToSend); }

        public int GetFriendMessage__V015(IntPtr _, CSteamID steamIDFriend, int iMessageID, IntPtr pvData, int cubData, IntPtr peChatEntryType)
        {
            int result = SteamFriends.Instance.GetFriendMessage((ulong)steamIDFriend, iMessageID, pvData, cubData, 0);
            WriteInt32(peChatEntryType, 0);
            return result;
        }

        public int GetFriendMessage__V017(IntPtr _, ulong steamIDFriend, int iMessageID, IntPtr pvData, int cubData, ref int peChatEntryType)
        {
            return SteamFriends.Instance.GetFriendMessage(steamIDFriend, iMessageID, pvData, cubData, peChatEntryType);
        }

        public int GetFriendMessage(IntPtr _, ulong steamIDFriend, int iMessageID, IntPtr pvData, int cubData, IntPtr peChatEntryType) { return SteamFriends.Instance.GetFriendMessage(steamIDFriend, iMessageID, pvData, cubData, peChatEntryType); }

        public SteamAPICall_t GetFollowerCount__V015(IntPtr _, CSteamID steamID)
        {
            return SteamFriends.Instance.GetFollowerCount((ulong)steamID);
        }

        public SteamAPICall_t GetFollowerCount(IntPtr _, ulong steamID) { return SteamFriends.Instance.GetFollowerCount(steamID); }

        public SteamAPICall_t IsFollowing__V015(IntPtr _, CSteamID steamID)
        {
            return SteamFriends.Instance.IsFollowing((ulong)steamID);
        }

        public SteamAPICall_t IsFollowing(IntPtr _, ulong steamID) { return SteamFriends.Instance.IsFollowing(steamID); }

        public SteamAPICall_t EnumerateFollowingList(IntPtr _, uint unStartIndex) { return SteamFriends.Instance.EnumerateFollowingList(unStartIndex); }

        public bool IsClanPublic__V015(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.IsClanPublic((ulong)steamIDClan);
        }

        public bool IsClanPublic(IntPtr _, ulong steamIDClan) { return SteamFriends.Instance.IsClanPublic(steamIDClan); }

        public bool IsClanOfficialGameGroup__V015(IntPtr _, CSteamID steamIDClan)
        {
            return SteamFriends.Instance.IsClanOfficialGameGroup((ulong)steamIDClan);
        }

        public bool IsClanOfficialGameGroup(IntPtr _, ulong steamIDClan) { return SteamFriends.Instance.IsClanOfficialGameGroup(steamIDClan); }

        public int GetNumChatsWithUnreadPriorityMessages(IntPtr _) { return SteamFriends.Instance.GetNumChatsWithUnreadPriorityMessages(); }

        public void ActivateGameOverlayRemotePlayTogetherInviteDialog(IntPtr _, ulong steamIDLobby) { SteamFriends.Instance.ActivateGameOverlayRemotePlayTogetherInviteDialog(steamIDLobby); }

        public bool RegisterProtocolInOverlayBrowser(IntPtr _, string pchProtocol) { return SteamFriends.Instance.RegisterProtocolInOverlayBrowser(pchProtocol); }

        public void ActivateGameOverlayInviteDialogConnectString(IntPtr _, string pchConnectString) { SteamFriends.Instance.ActivateGameOverlayInviteDialogConnectString(pchConnectString); }

        public SteamAPICall_t RequestEquippedProfileItems(IntPtr _, ulong steamID) { return SteamFriends.Instance.RequestEquippedProfileItems(steamID); }

        public bool BHasEquippedProfileItem(IntPtr _, ulong steamID, int itemType) { return SteamFriends.Instance.BHasEquippedProfileItem(steamID, itemType); }

        public IntPtr GetProfileItemPropertyString(IntPtr _, ulong steamID, int itemType, int prop) { return NativeStringCache.ToUtf8Ptr(SteamFriends.Instance.GetProfileItemPropertyString(steamID, itemType, prop)); }

        public uint GetProfileItemPropertyUint(IntPtr _, ulong steamID, int itemType, int prop) { return SteamFriends.Instance.GetProfileItemPropertyUint(steamID, itemType, prop); }
    }
}
