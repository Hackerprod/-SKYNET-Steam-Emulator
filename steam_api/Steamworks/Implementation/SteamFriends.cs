using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using SKYNET.Steamworks.Types;
using SKYNET.Types;
using Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamFriends : ISteamInterface
    {
        public List<Friend> Friends;
        public List<SteamID> Users;
        public Dictionary<string, string> RichPresence;

        public SteamFriends()
        {
            InterfaceVersion = "SteamFriends";
            Friends = new List<Friend>();
            Users = new List<SteamID>();
            RichPresence = new Dictionary<string, string>();
        }

        public void ActivateGameOverlay([MarshalAs(UnmanagedType.LPStr)] string friendsGroupID)
        {
            Write($"ActivateGameOverlay {friendsGroupID}");
        }

        public void ActivateGameOverlayInviteDialog(SteamID steamIDLobby)
        {
            Write($"ActivateGameOverlayInviteDialog {steamIDLobby.ConvertToUInt64()}");
        }

        public void ActivateGameOverlayInviteDialogConnectString([MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
        {
            Write($"ActivateGameOverlayInviteDialogConnectString {pchConnectString}");
        }

        public void ActivateGameOverlayRemotePlayTogetherInviteDialog(SteamID steamIDLobby)
        {
            Write($"ActivateGameOverlayRemotePlayTogetherInviteDialog {steamIDLobby.ConvertToUInt64()}");
        }

        public void ActivateGameOverlayToStore(uint nAppID, uint eFlag)
        {
            Write($"ActivateGameOverlayToStore {nAppID} {eFlag}");
        }


        public void ActivateGameOverlayToUser([MarshalAs(UnmanagedType.LPStr)] string friendsGroupID, SteamID steamID)
        {
            Write($"ActivateGameOverlayToUser {friendsGroupID} {steamID.ConvertToUInt64()}");
        }


        public void ActivateGameOverlayToWebPage([MarshalAs(UnmanagedType.LPStr)] string pchURL, int eMode)
        {
            Write($"ActivateGameOverlayToWebPage {pchURL}");
        }


        public void ClearRichPresence()
        {
            Write($"ClearRichPresence");
        }


        public bool CloseClanChatWindowInSteam(SteamID steamIDClanChat)
        {
            Write($"CloseClanChatWindowInSteam {steamIDClanChat.ConvertToUInt64()}");
            return true;
        }


        public SteamAPICall_t DownloadClanActivityCounts(UInt64[] clans, int cClansToRequest)
        {
            Write($"DownloadClanActivityCounts {cClansToRequest}");
            return 0;
        }


        public SteamAPICall_t EnumerateFollowingList(uint unStartIndex)
        {
            Write($"EnumerateFollowingList {unStartIndex}");
            // FriendsEnumerateFollowingList_t
            return 0;
        }


        public SteamID GetChatMemberByIndex(SteamID steamIDClan, int iUser)
        {
            Write($"GetChatMemberByIndex {steamIDClan.ConvertToUInt64()}");
            return 0;
        }


        public bool GetClanActivityCounts(SteamID steamIDClan, ref int online, ref int in_game, ref int chatting)
        {
            Write($"ActivateGameOverlay {steamIDClan.ConvertToUInt64()}");
            online = 0;
            in_game = 0;
            chatting = 0;
            return true;
        }


        public SteamID GetClanByIndex(int iClan)
        {
            Write($"GetClanByIndex {iClan}");
            return 0;
        }


        public int GetClanChatMemberCount(SteamID steamIDClan)
        {
            Write($"GetClanChatMemberCount {steamIDClan.ConvertToUInt64()}");
            return 0;
        }


        public int GetClanChatMessage(SteamID steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, int peChatEntryType, ref ulong psteamidChatter)
        {
            psteamidChatter = 0;
            Write($"GetClanChatMessage {steamIDClanChat.ConvertToUInt64()}");
            return 0;
        }


        public int GetClanCount()
        {
            Write($"GetClanCount");
            return 0;
        }


        public string GetClanName(SteamID steamIDClan)
        {
            Write($"GetClanName {steamIDClan.ConvertToUInt64()}");
            return "";
        }


        public SteamID GetClanOfficerByIndex(SteamID steamIDClan, int iOfficer)
        {
            Write($"GetClanOfficerByIndex {steamIDClan.ConvertToUInt64()}");
            return 0;
        }


        public int GetClanOfficerCount(SteamID steamIDClan)
        {
            Write($"GetClanOfficerCount {steamIDClan.ConvertToUInt64()}");
            return 0;
        }


        public SteamID GetClanOwner(SteamID steamIDClan)
        {
            Write($"GetClanOwner {steamIDClan.ConvertToUInt64()}");
            return 0;
        }


        public string GetClanTag(SteamID steamIDClan)
        {
            Write($"GetClanTag {steamIDClan.ConvertToUInt64()}");
            return "";
        }


        public SteamID GetCoplayFriend(int iCoplayFriend)
        {
            Write($"GetCoplayFriend {iCoplayFriend}");
            return 0;
        }


        public int GetCoplayFriendCount()
        {
            Write($"GetCoplayFriendCount");
            return 0;
        }


        public SteamAPICall_t GetFollowerCount(SteamID steamID)
        {
            Write($"GetFollowerCount {steamID.ConvertToUInt64()}");
            // FriendsGetFollowerCount_t
            return 0;
        }


        public SteamID GetFriendByIndex(int iFriend, int iFriendFlags)
        {
            Write($"GetFriendByIndex {iFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)iFriend);
            if (friend == null)
            {
                return 0;
            }
            return friend.SteamId;
        }


        public uint GetFriendCoplayGame(SteamID steamIDFriend)
        {
            Write($"GetFriendCoplayGame {steamIDFriend.ConvertToUInt64()}");
            return (uint)0;
        }


        public int GetFriendCoplayTime(SteamID steamIDFriend)
        {
            Write($"GetFriendCoplayTime {steamIDFriend.ConvertToUInt64()}");
            return 0;
        }


        public int GetFriendCount(int iFriendFlags)
        {
            Write($"GetFriendCount {(EFriendFlags)iFriendFlags}");
            return 0;
        }


        public int GetFriendCountFromSource(SteamID steamIDSource)
        {
            Write($"GetFriendCountFromSource {steamIDSource.ConvertToUInt64()}");
            return 0;
        }


        public SteamID GetFriendFromSourceByIndex(SteamID steamIDSource, int iFriend)
        {
            Write($"GetFriendFromSourceByIndex {steamIDSource.ConvertToUInt64()} {iFriend}");
            return 0;
        }


        public bool GetFriendGamePlayed(SteamID steamIDFriend, ref FriendGameInfo_t pFriendGameInfo)
        {
            Write($"GetFriendGamePlayed {steamIDFriend.ConvertToUInt64()}" );

            pFriendGameInfo = new FriendGameInfo_t();

            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
            if (friend == null)
            {
                pFriendGameInfo.m_gameID = (CGameID)0;
                pFriendGameInfo.m_unGameIP = 0;
                pFriendGameInfo.m_usGamePort = 0;
                return false; 
            }
            pFriendGameInfo.m_gameID = (CGameID)friend.GameId;
            pFriendGameInfo.m_unGameIP = 0;
            pFriendGameInfo.m_usGamePort = 0;
            return true;
        }


        public int GetFriendMessage(SteamID steamIDFriend, int iMessageID, IntPtr pvData, int cubData, uint peChatEntryType)
        {
            Write($"GetFriendMessage {steamIDFriend.ConvertToUInt64()} {(EChatEntryType)peChatEntryType}");
            peChatEntryType = 1;
            return 0;
        }


        public string GetFriendPersonaName(ulong steamIDFriend)
        {
            Write($"------------------------- GetFriendPersonaName {steamIDFriend} | Mi ID {(ulong)SteamEmulator.SteamId}");

            string personaName = "Unknown";

            if ((ulong)steamIDFriend == (ulong)SteamEmulator.SteamId)
            {
                personaName = SteamEmulator.PersonaName;
            }
            else
            {
                Friend friend = Friends.Find(f => f.SteamId == steamIDFriend);
                if (friend != null) personaName = friend.PersonaName;
            }

            return personaName;
        }


        public string GetFriendPersonaNameHistory(SteamID steamIDFriend, int iPersonaName)
        {
            Write($"GetFriendPersonaNameHistory {steamIDFriend.ConvertToUInt64()}");
            return "SKYNET";
        }


        public int GetFriendPersonaState(SteamID steamIDFriend)
        {
            Write($"GetFriendPersonaState {steamIDFriend.ConvertToUInt64()}");
            return (int)(Users.Find(f => f.ConvertToUInt64() == steamIDFriend.ConvertToUInt64()) == null ? EPersonaState.k_EPersonaStateOffline : EPersonaState.k_EPersonaStateOnline);
        }


        public int GetFriendRelationship(SteamID steamIDFriend)
        {
            Write($"GetFriendRelationship {steamIDFriend.ConvertToUInt64()}");
            return (int)EFriendRelationship.k_EFriendRelationshipNone;
        }


        public string GetFriendRichPresence(SteamID steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchKey)
        {
            Write($"GetFriendRichPresence [{steamIDFriend.ConvertToUInt64()}]: {pchKey}");
            if (RichPresence.ContainsKey(pchKey))
            {
                return RichPresence[pchKey];
            }
            return "";
        }

        public string GetFriendRichPresenceKeyByIndex(SteamID steamIDFriend, int iKey)
        {
            Write($"GetFriendRichPresenceKeyByIndex {steamIDFriend.ConvertToUInt64()} {iKey}");
            return "";
        }

        public int GetFriendRichPresenceKeyCount(SteamID steamIDFriend)
        {
            Write($"GetFriendRichPresenceKeyCount {steamIDFriend.ConvertToUInt64()}");
            return 0;
        }

        public int GetFriendsGroupCount()
        {
            Write($"GetFriendsGroupCount");
            return 0;
        }

        public int GetFriendsGroupIDByIndex(int iFG)
        {
            Write($"GetFriendsGroupIDByIndex {iFG}");
            return (int)0;
        }


        public int GetFriendsGroupMembersCount(int friendsGroupID)
        {
            Write($"GetFriendsGroupMembersCount {friendsGroupID}");
            return 0;
        }


        public void GetFriendsGroupMembersList(short friendsGroupID, ref IntPtr pOutSteamIDMembers, int nMembersCount)
        {
            Write($"GetFriendsGroupMembersList {friendsGroupID}");
            Marshal.StructureToPtr(SteamEmulator.SteamId, pOutSteamIDMembers, false);
        }


        public string GetFriendsGroupName(int friendsGroupID)
        {
            Write($"GetFriendsGroupName {friendsGroupID}");
            return "";
        }


        public int GetFriendSteamLevel(SteamID steamIDFriend)
        {
            Write($"GetFriendSteamLevel {steamIDFriend.ConvertToUInt64()}");
            return 100;
        }


        public int GetSmallFriendAvatar(SteamID steamIDFriend)
        {
            Write($"GetSmallFriendAvatar {steamIDFriend.ConvertToUInt64()}");
            return 0;
        }


        public int GetMediumFriendAvatar(SteamID steamIDFriend)
        {
            Write($"GetMediumFriendAvatar {steamIDFriend.ConvertToUInt64()}");
            return 0;
        }


        public int GetLargeFriendAvatar(SteamID steamIDFriend)
        {
            Write($"GetLargeFriendAvatar {steamIDFriend.ConvertToUInt64()}");
            return 0;
        }


        public int GetNumChatsWithUnreadPriorityMessages()
        {
            Write($"GetNumChatsWithUnreadPriorityMessages");
            return 0;
        }


        public string GetPersonaName()
        {
            string PersonaName = SteamEmulator.PersonaName;
            Write($"GetPersonaName {PersonaName}");
            return PersonaName;
        }


        public uint GetPersonaState()
        {
            Write($"GetPersonaState");
            return (uint)EPersonaState.k_EPersonaStateOnline;
        }


        public string GetPlayerNickname(SteamID steamIDPlayer)
        {
            Write($"GetPlayerNickname {steamIDPlayer.ConvertToUInt64()}");
            if (steamIDPlayer == SteamEmulator.SteamId)
            {
                return SteamEmulator.PersonaName;
            }
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDPlayer);
            if (friend == null)
            {
                return "";
            }
            return friend.PersonaName;
        }


        public uint GetUserRestrictions()
        {
            Write($"GetUserRestrictions");
            return 0;
        }


        public bool HasFriend(SteamID steamIDFriend, int iFriendFlags)
        {
            Write($"HasFriend {steamIDFriend.ConvertToUInt64()}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
            return friend != null;
        }


        public bool InviteUserToGame(SteamID steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
        {
            Write($"InviteUserToGame {steamIDFriend.ConvertToUInt64()} {pchConnectString}");
            return false;
        }


        public bool IsClanChatAdmin(SteamID steamIDClanChat, SteamID steamIDUser)
        {
            Write($"IsClanChatAdmin {steamIDClanChat.ConvertToUInt64()}");
            return false;
        }


        public bool IsClanChatWindowOpenInSteam(SteamID steamIDClanChat)
        {
            Write($"IsClanChatWindowOpenInSteam {steamIDClanChat.ConvertToUInt64()}");
            return false;
        }


        public bool IsClanOfficialGameGroup(SteamID steamIDClan)
        {
            Write($"IsClanOfficialGameGroup {steamIDClan.ConvertToUInt64()}");
            return false;
        }


        public bool IsClanPublic(SteamID steamIDClan)
        {
            Write($"IsClanpublic {steamIDClan.ConvertToUInt64()}");
            return false;
        }


        public SteamAPICall_t IsFollowing(SteamID steamID)
        {
            Write($"IsFollowing {steamID.ConvertToUInt64()}");
            // FriendsIsFollowing_t
            return 0;
        }


        public bool IsUserInSource(SteamID steamIDUser, SteamID steamIDSource)
        {
            Write($"IsUserInSource {steamIDUser.ConvertToUInt64()}");
            return false;
        }


        public SteamAPICall_t JoinClanChatRoom(SteamID steamIDClan)
        {
            Write($"JoinClanChatRoom {steamIDClan.ConvertToUInt64()}");
            // JoinClanChatRoomCompletionResult_t
            return 0;
        }


        public bool LeaveClanChatRoom(SteamID steamIDClan)
        {
            Write($"LeaveClanChatRoom {steamIDClan.ConvertToUInt64()}");
            return true;
        }


        public bool OpenClanChatWindowInSteam(SteamID steamIDClanChat)
        {
            Write($"OpenClanChatWindowInSteam {steamIDClanChat.ConvertToUInt64()}");
            return false;
        }


        public bool RegisterProtocolInOverlayBrowser([MarshalAs(UnmanagedType.LPStr)] string pchProtocol)
        {
            Write($"RegisterProtocolInOverlayBrowser {pchProtocol}");
            return false;
        }


        public bool ReplyToFriendMessage(SteamID steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchMsgToSend)
        {
            Write($"ReplyToFriendMessage {steamIDFriend.ConvertToUInt64()} {pchMsgToSend}");
            return false;
        }


        public SteamAPICall_t RequestClanOfficerList(SteamID steamIDClan)
        {
            Write($"RequestClanOfficerList {steamIDClan.ConvertToUInt64()}");
            // ClanOfficerListResponse_t
            return 0;
        }


        public void RequestFriendRichPresence(SteamID steamIDFriend)
        {
            Write($"RequestFriendRichPresence {steamIDFriend.ConvertToUInt64()}");
        }


        public bool RequestUserInformation(SteamID steamIDUser, bool bRequireNameOnly)
        {
            Write($"RequestUserInformation {steamIDUser.ConvertToUInt64()}");
            return false;
        }


        public bool SendClanChatMessage(SteamID steamIDClanChat, [MarshalAs(UnmanagedType.LPStr)] string pchText)
        {
            Write($"SendClanChatMessage {steamIDClanChat.ConvertToUInt64()} {pchText}");
            return false;
        }


        public void SetInGameVoiceSpeaking(SteamID steamIDUser, bool bSpeaking)
        {
            Write($"SetInGameVoiceSpeaking {steamIDUser.ConvertToUInt64()}");
        }


        public bool SetListenForFriendsMessages(bool bInterceptEnabled)
        {
            Write($"SetListenForFriendsMessages {bInterceptEnabled}");
            return true;
        }


        public SteamAPICall_t SetPersonaName([MarshalAs(UnmanagedType.LPStr)] string pchPersonaName)
        {
            Write($"SetPersonaName {pchPersonaName}");
            // SetPersonaNameResponse_t

            SetPersonaNameResponse_t data = new SetPersonaNameResponse_t();
            data.m_bSuccess = true;
            data.m_bLocalSuccess = true;
            data.m_result = EResult.k_EResultOK;

            SteamEmulator.PersonaName = pchPersonaName;

            return 0;
            return new SteamAPICall_t(CallbackType.k_iSetPersonaNameResponse);
        }

        public void SetPlayedWith(SteamID steamIDUserPlayedWith)
        {
            Write($"SetPlayedWith {steamIDUserPlayedWith.ConvertToUInt64()}");
        }


        public bool SetRichPresence([MarshalAs(UnmanagedType.LPStr)] string pchKey, [MarshalAs(UnmanagedType.LPStr)] string pchValue)
        {
            Write($"SetRichPresence {pchKey} {pchValue}");

            if (!string.IsNullOrEmpty(pchValue))
            {
                if (RichPresence.ContainsKey(pchKey))
                {
                    RichPresence[pchKey] = pchValue;
                }
                else
                {
                    RichPresence.Add(pchKey, pchValue);
                }
            }
            else
            {
                if (RichPresence.ContainsKey(pchKey))
                {
                    RichPresence.Remove(pchKey);
                }
            }

            return true;
        }
    }
}

