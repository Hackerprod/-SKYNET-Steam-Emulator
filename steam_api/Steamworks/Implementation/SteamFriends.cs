using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Steamworks;
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
            Write($"ActivateGameOverlayInviteDialog {steamIDLobby}");
        }

        public void ActivateGameOverlayInviteDialogConnectString([MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
        {
            Write($"ActivateGameOverlayInviteDialogConnectString {pchConnectString}");
        }

        public void ActivateGameOverlayRemotePlayTogetherInviteDialog(SteamID steamIDLobby)
        {
            Write($"ActivateGameOverlayRemotePlayTogetherInviteDialog {steamIDLobby}");
        }

        public void ActivateGameOverlayToStore(uint nAppID, uint eFlag)
        {
            Write($"ActivateGameOverlayToStore {nAppID} {eFlag}");
        }


        public void ActivateGameOverlayToUser([MarshalAs(UnmanagedType.LPStr)] string friendsGroupID, SteamID steamID)
        {
            Write($"ActivateGameOverlayToUser {friendsGroupID} {steamID}");
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
            Write($"CloseClanChatWindowInSteam {steamIDClanChat}");
            return true;
        }


        public ulong DownloadClanActivityCounts(UInt64[] clans, int cClansToRequest)
        {
            Write($"DownloadClanActivityCounts {cClansToRequest}");
            return (ulong)0;
        }


        public ulong EnumerateFollowingList(uint unStartIndex)
        {
            Write($"EnumerateFollowingList {unStartIndex}");
            return (ulong)0;
        }


        public SteamID GetChatMemberByIndex(SteamID steamIDClan, int iUser)
        {
            Write($"GetChatMemberByIndex {steamIDClan}");
            return 0;
        }


        public bool GetClanActivityCounts(SteamID steamIDClan, ref int online, ref int in_game, ref int chatting)
        {
            Write($"ActivateGameOverlay {steamIDClan}");
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
            Write($"GetClanChatMemberCount {steamIDClan}");
            return 0;
        }


        public int GetClanChatMessage(SteamID steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, int peChatEntryType, ref ulong psteamidChatter)
        {
            psteamidChatter = 0;
            Write($"GetClanChatMessage {steamIDClanChat}");
            return 0;
        }


        public int GetClanCount()
        {
            Write($"GetClanCount");
            return 0;
        }


        public string GetClanName(SteamID steamIDClan)
        {
            Write($"GetClanName {steamIDClan}");
            return "";
        }


        public SteamID GetClanOfficerByIndex(SteamID steamIDClan, int iOfficer)
        {
            Write($"GetClanOfficerByIndex {steamIDClan}");
            return 0;
        }


        public int GetClanOfficerCount(SteamID steamIDClan)
        {
            Write($"GetClanOfficerCount {steamIDClan}");
            return 0;
        }


        public SteamID GetClanOwner(SteamID steamIDClan)
        {
            Write($"GetClanOwner {steamIDClan}");
            return 0;
        }


        public string GetClanTag(SteamID steamIDClan)
        {
            Write($"GetClanTag {steamIDClan}");
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


        public ulong GetFollowerCount(SteamID steamID)
        {
            Write($"GetFollowerCount {steamID}");
            return (ulong)0;
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
            Write($"GetFriendCoplayGame {steamIDFriend}");
            return (uint)0;
        }


        public int GetFriendCoplayTime(SteamID steamIDFriend)
        {
            Write($"GetFriendCoplayTime {steamIDFriend}");
            return 0;
        }


        public int GetFriendCount(int iFriendFlags)
        {
            Write($"GetFriendCount {(EFriendFlags)iFriendFlags}");
            return 0;
        }


        public int GetFriendCountFromSource(SteamID steamIDSource)
        {
            Write($"GetFriendCountFromSource {steamIDSource}");
            return 0;
        }


        public SteamID GetFriendFromSourceByIndex(SteamID steamIDSource, int iFriend)
        {
            Write($"GetFriendFromSourceByIndex {steamIDSource} {iFriend}");
            return 0;
        }


        public bool GetFriendGamePlayed(SteamID steamIDFriend, ref FriendGameInfo_t pFriendGameInfo)
        {
            Write($"GetFriendGamePlayed");

            pFriendGameInfo = new FriendGameInfo_t();

            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
            if (friend == null)
            {
                pFriendGameInfo.GameID = 0;
                pFriendGameInfo.GameIP = 0;
                pFriendGameInfo.GamePort = 0;
                return true; // false
            }
            pFriendGameInfo.GameID = friend.GameId;
            pFriendGameInfo.GameIP = 0;
            pFriendGameInfo.GamePort = 0;
            return true;
        }


        public int GetFriendMessage(SteamID steamIDFriend, int iMessageID, IntPtr pvData, int cubData, uint peChatEntryType)
        {
            Write($"GetFriendMessage {steamIDFriend} {(EChatEntryType)peChatEntryType}");
            peChatEntryType = 1;
            return 0;
        }


        public string GetFriendPersonaName(SteamID steamIDFriend)
        {
            Write($"GetFriendPersonaName {steamIDFriend}");
            Friend friend = Friends.Find(f => f.SteamId == steamIDFriend);
            if (friend == null)
            {
                return "";
            }
            return friend.PersonaName;
        }


        public string GetFriendPersonaNameHistory(SteamID steamIDFriend, int iPersonaName)
        {
            Write($"GetFriendPersonaNameHistory {steamIDFriend}");
            return "SKYNET";
        }


        public int GetFriendPersonaState(SteamID steamIDFriend)
        {
            Write($"GetFriendPersonaState {steamIDFriend}");
            return (int)(Users.Find(f => f.ConvertToUInt64() == steamIDFriend.ConvertToUInt64()) == null ? EPersonaState.k_EPersonaStateOffline : EPersonaState.k_EPersonaStateOnline);
        }


        public int GetFriendRelationship(SteamID steamIDFriend)
        {
            Write($"GetFriendRelationship {steamIDFriend}");
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
            Write($"GetFriendRichPresenceKeyByIndex {steamIDFriend} {iKey}");
            return "";
        }

        public int GetFriendRichPresenceKeyCount(SteamID steamIDFriend)
        {
            Write($"GetFriendRichPresenceKeyCount {steamIDFriend}");
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
            Write($"GetFriendSteamLevel {steamIDFriend}");
            return 100;
        }


        public int GetSmallFriendAvatar(SteamID steamIDFriend)
        {
            Write($"GetSmallFriendAvatar {steamIDFriend}");
            return 0;
        }


        public int GetMediumFriendAvatar(SteamID steamIDFriend)
        {
            Write($"GetMediumFriendAvatar {steamIDFriend}");
            return 0;
        }


        public int GetLargeFriendAvatar(SteamID steamIDFriend)
        {
            Write($"GetLargeFriendAvatar {steamIDFriend}");
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
            Write($"GetPlayerNickname {steamIDPlayer}");
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
            Write($"HasFriend {steamIDFriend}");
            Friend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
            return friend != null;
        }


        public bool InviteUserToGame(SteamID steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchConnectString)
        {
            Write($"InviteUserToGame {steamIDFriend} {pchConnectString}");
            return false;
        }


        public bool IsClanChatAdmin(SteamID steamIDClanChat, SteamID steamIDUser)
        {
            Write($"IsClanChatAdmin {steamIDClanChat}");
            return false;
        }


        public bool IsClanChatWindowOpenInSteam(SteamID steamIDClanChat)
        {
            Write($"IsClanChatWindowOpenInSteam {steamIDClanChat}");
            return false;
        }


        public bool IsClanOfficialGameGroup(SteamID steamIDClan)
        {
            Write($"IsClanOfficialGameGroup {steamIDClan}");
            return false;
        }


        public bool IsClanPublic(SteamID steamIDClan)
        {
            Write($"IsClanpublic {steamIDClan}");
            return false;
        }


        public ulong IsFollowing(SteamID steamID)
        {
            Write($"IsFollowing {steamID}");
            return (ulong)0;
        }


        public bool IsUserInSource(SteamID steamIDUser, SteamID steamIDSource)
        {
            Write($"IsUserInSource {steamIDUser}");
            return false;
        }


        public ulong JoinClanChatRoom(SteamID steamIDClan)
        {
            Write($"JoinClanChatRoom {steamIDClan}");
            return (ulong)0;
        }


        public bool LeaveClanChatRoom(SteamID steamIDClan)
        {
            Write($"LeaveClanChatRoom {steamIDClan}");
            return true;
        }


        public bool OpenClanChatWindowInSteam(SteamID steamIDClanChat)
        {
            Write($"OpenClanChatWindowInSteam {steamIDClanChat}");
            return false;
        }


        public bool RegisterProtocolInOverlayBrowser([MarshalAs(UnmanagedType.LPStr)] string pchProtocol)
        {
            Write($"RegisterProtocolInOverlayBrowser {pchProtocol}");
            return false;
        }


        public bool ReplyToFriendMessage(SteamID steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchMsgToSend)
        {
            Write($"ReplyToFriendMessage {steamIDFriend} {pchMsgToSend}");
            return false;
        }


        public ulong RequestClanOfficerList(SteamID steamIDClan)
        {
            Write($"RequestClanOfficerList {steamIDClan}");
            return 0;
        }


        public void RequestFriendRichPresence(SteamID steamIDFriend)
        {
            Write($"RequestFriendRichPresence {steamIDFriend}");
        }


        public bool RequestUserInformation(SteamID steamIDUser, bool bRequireNameOnly)
        {
            Write($"RequestUserInformation {steamIDUser}");
            return false;
        }


        public bool SendClanChatMessage(SteamID steamIDClanChat, [MarshalAs(UnmanagedType.LPStr)] string pchText)
        {
            Write($"SendClanChatMessage {steamIDClanChat} {pchText}");
            return false;
        }


        public void SetInGameVoiceSpeaking(SteamID steamIDUser, bool bSpeaking)
        {
            Write($"SetInGameVoiceSpeaking {steamIDUser}");
        }


        public bool SetListenForFriendsMessages(bool bInterceptEnabled)
        {
            Write($"SetListenForFriendsMessages {bInterceptEnabled}");
            return true;
        }


        public SteamAPICall_t SetPersonaName([MarshalAs(UnmanagedType.LPStr)] string pchPersonaName)
        {
            Write($"SetPersonaName {pchPersonaName}");

            SetPersonaNameResponse_t data = new SetPersonaNameResponse_t();
            data.Success = true;
            data.LocalSuccess = true;
            data.Result = SKYNET.Result.OK;

            SteamEmulator.PersonaName = pchPersonaName;

            var b = new Callback.Buffer();
            b.SetAlignment(4);

            b.WriteString(pchPersonaName);
            b.WriteBool(data.Success);
            b.WriteInt((int)data.Result);

            PostCallback(data, CallbackType.k_iSetPersonaNameResponse, b);

            return new SteamAPICall_t(CallbackType.k_iSetPersonaNameResponse);
        }

        public void SetPlayedWith(SteamID steamIDUserPlayedWith)
        {
            Write($"SetPlayedWith {steamIDUserPlayedWith}");
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

