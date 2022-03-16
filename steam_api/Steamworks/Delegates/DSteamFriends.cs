using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SKYNET.Interface;
using Steamworks;

namespace SKYNET.Delegate
{
    [Delegate("SteamController")]
    public class DSteamFriends
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetPersonaName();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t SetPersonaName([MarshalAs(UnmanagedType.LPStr)] string pchPersonaName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EPersonaState GetPersonaState();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFriendCount(int iFriendFlags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetFriendByIndex(int iFriend, int iFriendFlags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EFriendRelationship GetFriendRelationship(IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EPersonaState GetFriendPersonaState(IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetFriendPersonaName(IntPtr steamIDFriend);


        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetFriendGamePlayed(IntPtr steamIDFriend, out FriendGameInfo_t pFriendGameInfo );

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetFriendPersonaNameHistory(IntPtr steamIDFriend, int iPersonaName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFriendSteamLevel(IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetPlayerNickname(IntPtr steamIDPlayer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFriendsGroupCount();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate FriendsGroupID_t GetFriendsGroupIDByIndex(int iFG);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetFriendsGroupName(FriendsGroupID_t friendsGroupID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFriendsGroupMembersCount(FriendsGroupID_t friendsGroupID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void GetFriendsGroupMembersList(FriendsGroupID_t friendsGroupID, IntPtr[] pOutSteamIDMembers, int nMembersCount);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool HasFriend(IntPtr steamIDFriend, int iFriendFlags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetClanCount();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetClanByIndex(int iClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetClanName(IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetClanTag(IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetClanActivityCounts(IntPtr steamIDClan, int pnOnline, int pnInGame, int pnChatting);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t DownloadClanActivityCounts(IntPtr[] psteamIDClans, int cClansToRequest);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFriendCountFromSource(IntPtr steamIDSource);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetFriendFromSourceByIndex(IntPtr steamIDSource, int iFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsUserInSource(IntPtr steamIDUser, IntPtr steamIDSource);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetInGameVoiceSpeaking(IntPtr steamIDUser, bool bSpeaking);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateGameOverlay([MarshalAs(UnmanagedType.LPStr)] string pchDialog);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateGameOverlayToUser([MarshalAs(UnmanagedType.LPStr)] string pchDialog, IntPtr steamID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateGameOverlayToWebPage([MarshalAs(UnmanagedType.LPStr)] string pchURL, EActivateGameOverlayToWebPageMode eMode = EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateGameOverlayToStore(AppId_t nAppID, EOverlayToStoreFlag eFlag);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetPlayedWith(IntPtr steamIDUserPlayedWith);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateGameOverlayInviteDialog(IntPtr steamIDLobby);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetSmallFriendAvatar(IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetMediumFriendAvatar(IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetLargeFriendAvatar(IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RequestUserInformation(IntPtr steamIDUser, bool bRequireNameOnly);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t RequestClanOfficerList(IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetClanOwner(IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetClanOfficerCount(IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetClanOfficerByIndex(IntPtr steamIDClan, int iOfficer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate UInt32 GetUserRestrictions();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetRichPresence([MarshalAs(UnmanagedType.LPStr)] string pchKey, [MarshalAs(UnmanagedType.LPStr)] string pchValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ClearRichPresence();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetFriendRichPresence(IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchKey);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFriendRichPresenceKeyCount(IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetFriendRichPresenceKeyByIndex(IntPtr steamIDFriend, int iKey);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void RequestFriendRichPresence(IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool InviteUserToGame(IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchConnectString);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetCoplayFriendCount();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetCoplayFriend(int iCoplayFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFriendCoplayTime(IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate AppId_t GetFriendCoplayGame(IntPtr steamIDFriend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t JoinClanChatRoom(IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool LeaveClanChatRoom(IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetClanChatMemberCount(IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetChatMemberByIndex(IntPtr steamIDClan, int iUser);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SendClanChatMessage(IntPtr steamIDClanChat, [MarshalAs(UnmanagedType.LPStr)] string pchText);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetClanChatMessage(IntPtr steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, EChatEntryType peChatEntryType, IntPtr[] psteamidChatter );

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsClanChatAdmin(IntPtr steamIDClanChat, IntPtr steamIDUser);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsClanChatWindowOpenInSteam(IntPtr steamIDClanChat);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool OpenClanChatWindowInSteam(IntPtr steamIDClanChat);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool CloseClanChatWindowInSteam(IntPtr steamIDClanChat);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetListenForFriendsMessages(bool bInterceptEnabled);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ReplyToFriendMessage(IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchMsgToSend);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetFriendMessage(IntPtr steamIDFriend, int iMessageID, IntPtr pvData, int cubData, EChatEntryType peChatEntryType);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t GetFollowerCount(IntPtr steamID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t IsFollowing(IntPtr steamID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t EnumerateFollowingList(UInt32 unStartIndex);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsClanPublic(IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool IsClanOfficialGameGroup(IntPtr steamIDClan);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetNumChatsWithUnreadPriorityMessages();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateGameOverlayRemotePlayTogetherInviteDialog(IntPtr steamIDLobby);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RegisterProtocolInOverlayBrowser([MarshalAs(UnmanagedType.LPStr)] string pchProtocol);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateGameOverlayInviteDialogConnectString([MarshalAs(UnmanagedType.LPStr)] string pchConnectString);
    }
}
