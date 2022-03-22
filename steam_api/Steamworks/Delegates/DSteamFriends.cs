using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Core.Interface;
using SKYNET.Interface;
using Steamworks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamFriends")]
    public class DSteamFriends 
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetPersonaName(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t SetPersonaName(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchPersonaName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EPersonaState GetPersonaState(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFriendCount(IntPtr _, int iFriendFlags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetFriendByIndex(IntPtr _, int iFriend, int iFriendFlags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EFriendRelationship GetFriendRelationship(IntPtr _, IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EPersonaState GetFriendPersonaState(IntPtr _, IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetFriendPersonaName(IntPtr _, IntPtr steamIDFriend);


        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetFriendGamePlayed(IntPtr _, IntPtr steamIDFriend, out FriendGameInfo_t pFriendGameInfo);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetFriendPersonaNameHistory(IntPtr _, IntPtr steamIDFriend, int iPersonaName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFriendSteamLevel(IntPtr _, IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetPlayerNickname(IntPtr _, IntPtr steamIDPlayer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFriendsGroupCount(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate FriendsGroupID_t GetFriendsGroupIDByIndex(IntPtr _, int iFG);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetFriendsGroupName(IntPtr _, FriendsGroupID_t friendsGroupID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFriendsGroupMembersCount(IntPtr _, FriendsGroupID_t friendsGroupID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void GetFriendsGroupMembersList(IntPtr _, FriendsGroupID_t friendsGroupID, IntPtr[] pOutSteamIDMembers, int nMembersCount);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool HasFriend(IntPtr _, IntPtr steamIDFriend, int iFriendFlags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetClanCount(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetClanByIndex(IntPtr _, int iClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetClanName(IntPtr _, IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetClanTag(IntPtr _, IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetClanActivityCounts(IntPtr _, IntPtr steamIDClan, int pnOnline, int pnInGame, int pnChatting);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t DownloadClanActivityCounts(IntPtr _, IntPtr[] psteamIDClans, int cClansToRequest);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFriendCountFromSource(IntPtr _, IntPtr steamIDSource);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetFriendFromSourceByIndex(IntPtr _, IntPtr steamIDSource, int iFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsUserInSource(IntPtr _, IntPtr steamIDUser, IntPtr steamIDSource);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetInGameVoiceSpeaking(IntPtr _, IntPtr steamIDUser, bool bSpeaking);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateGameOverlay(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchDialog);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateGameOverlayToUser(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchDialog, IntPtr steamID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateGameOverlayToWebPage(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchURL, EActivateGameOverlayToWebPageMode eMode = EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateGameOverlayToStore(IntPtr _, AppId_t nAppID, EOverlayToStoreFlag eFlag);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetPlayedWith(IntPtr _, IntPtr steamIDUserPlayedWith);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateGameOverlayInviteDialog(IntPtr _, IntPtr steamIDLobby);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetSmallFriendAvatar(IntPtr _, IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetMediumFriendAvatar(IntPtr _, IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetLargeFriendAvatar(IntPtr _, IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RequestUserInformation(IntPtr _, IntPtr steamIDUser, bool bRequireNameOnly);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RequestClanOfficerList(IntPtr _, IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetClanOwner(IntPtr _, IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetClanOfficerCount(IntPtr _, IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetClanOfficerByIndex(IntPtr _, IntPtr steamIDClan, int iOfficer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate UInt32 GetUserRestrictions(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetRichPresence(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchKey, [MarshalAs(UnmanagedType.LPStr)] string pchValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ClearRichPresence(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetFriendRichPresence(IntPtr _, IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchKey);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFriendRichPresenceKeyCount(IntPtr _, IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetFriendRichPresenceKeyByIndex(IntPtr _, IntPtr steamIDFriend, int iKey);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void RequestFriendRichPresence(IntPtr _, IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool InviteUserToGame(IntPtr _, IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchConnectString);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetCoplayFriendCount(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetCoplayFriend(IntPtr _, int iCoplayFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFriendCoplayTime(IntPtr _, IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate AppId_t GetFriendCoplayGame(IntPtr _, IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t JoinClanChatRoom(IntPtr _, IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool LeaveClanChatRoom(IntPtr _, IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetClanChatMemberCount(IntPtr _, IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetChatMemberByIndex(IntPtr _, IntPtr steamIDClan, int iUser);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SendClanChatMessage(IntPtr _, IntPtr steamIDClanChat, [MarshalAs(UnmanagedType.LPStr)] string pchText);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetClanChatMessage(IntPtr _, IntPtr steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, EChatEntryType peChatEntryType, IntPtr[] psteamidChatter);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsClanChatAdmin(IntPtr _, IntPtr steamIDClanChat, IntPtr steamIDUser);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsClanChatWindowOpenInSteam(IntPtr _, IntPtr steamIDClanChat);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool OpenClanChatWindowInSteam(IntPtr _, IntPtr steamIDClanChat);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool CloseClanChatWindowInSteam(IntPtr _, IntPtr steamIDClanChat);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetListenForFriendsMessages(IntPtr _, bool bInterceptEnabled);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ReplyToFriendMessage(IntPtr _, IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchMsgToSend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFriendMessage(IntPtr _, IntPtr steamIDFriend, int iMessageID, IntPtr pvData, int cubData, EChatEntryType peChatEntryType);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t GetFollowerCount(IntPtr _, IntPtr steamID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t IsFollowing(IntPtr _, IntPtr steamID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t EnumerateFollowingList(IntPtr _, uint unStartIndex);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsClanPublic(IntPtr _, IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsClanOfficialGameGroup(IntPtr _, IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetNumChatsWithUnreadPriorityMessages(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateGameOverlayRemotePlayTogetherInviteDialog(IntPtr _, IntPtr steamIDLobby);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RegisterProtocolInOverlayBrowser(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchProtocol);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateGameOverlayInviteDialogConnectString(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchConnectString);
    }
}
