using SKYNET.Steamworks;
using SKYNET.Types;
using Steamworks;
using System;
using System.Runtime.InteropServices;

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Interface
{
    [Interface("SteamFriends017")]
    public class SteamFriends017 : ISteamInterface
    {
        public string GetPersonaName(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetPersonaName();
        }

        public SteamAPICall_t SetPersonaName(IntPtr _, string name)
        {
            return SteamEmulator.SteamFriends.SetPersonaName(name);
        }

        public uint GetPersonaState(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetPersonaState();
        }

        public int GetFriendCount(IntPtr _, int iFriendFlags)
        {
            return SteamEmulator.SteamFriends.GetFriendCount(iFriendFlags);
        }

        public CSteamID GetFriendByIndex(IntPtr _, int iFriend, int iFriendFlags)
        {
            return SteamEmulator.SteamFriends.GetFriendByIndex(iFriend, iFriendFlags);
        }

        public EFriendRelationship GetFriendRelationship(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetFriendRelationship(steamID);
        }

        public uint GetFriendPersonaState(IntPtr _, ulong steamID)
        {
            return (uint)SteamEmulator.SteamFriends.GetFriendPersonaState(steamID);
        }

        public string GetFriendPersonaName(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetFriendPersonaName(steamID);
        }

        public bool GetFriendGamePlayed(IntPtr _, ulong steamID, IntPtr friend_game_info_out)
        {
            return SteamEmulator.SteamFriends.GetFriendGamePlayed(steamID, friend_game_info_out);
        }

        public string GetFriendPersonaNameHistory(IntPtr _, ulong steamID, int index)
        {
            return SteamEmulator.SteamFriends.GetFriendPersonaNameHistory(steamID, index);
        }

        public int GetFriendSteamLevel(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetFriendSteamLevel(steamID);
        }

        public string GetPlayerNickname(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetPlayerNickname(steamID);
        }

        public int GetFriendsGroupCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupCount();
        }

        public short GetFriendsGroupIDByIndex(IntPtr _, int index)
        {
            return (short)SteamEmulator.SteamFriends.GetFriendsGroupIDByIndex(index);
        }

        public string GetFriendsGroupName(IntPtr _, short id)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupName(id);
        }

        public int GetFriendsGroupMembersCount(IntPtr _, short id)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupMembersCount(id);
        }

        public void GetFriendsGroupMembersList(IntPtr _, short friendsGroupID, IntPtr steam_id_out, int max_steam_id_out)
        {
            SteamEmulator.SteamFriends.GetFriendsGroupMembersList(friendsGroupID, steam_id_out, max_steam_id_out);
        }

        public bool HasFriend(IntPtr _, ulong steamID, int iFriendFlags)
        {
            return SteamEmulator.SteamFriends.HasFriend(steamID, iFriendFlags);
        }

        public int GetClanCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetClanCount();
        }

        public CSteamID GetClanByIndex(IntPtr _, int index)
        {
            return SteamEmulator.SteamFriends.GetClanByIndex(index);
        }

        public string GetClanName(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetClanName(steamID);
        }

        public string GetClanTag(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetClanTag(steamID);
        }

        public bool GetClanActivityCounts(IntPtr _, ulong steamID, ref int online, ref int in_game, ref int chatting)
        {
            return SteamEmulator.SteamFriends.GetClanActivityCounts(steamID, ref online, ref in_game, ref chatting);
        }

        public SteamAPICall_t DownloadClanActivityCounts(IntPtr _, UInt64[] clans, int count)
        {
            return SteamEmulator.SteamFriends.DownloadClanActivityCounts(clans, count);
        }

        public int GetFriendCountFromSource(IntPtr _, ulong source_id)
        {

            return SteamEmulator.SteamFriends.GetFriendCountFromSource(source_id);
        }

        public CSteamID GetFriendFromSourceByIndex(IntPtr _, ulong source_id, int index)
        {
            return SteamEmulator.SteamFriends.GetFriendFromSourceByIndex(source_id, index);
        }

        public bool IsUserInSource(IntPtr _, ulong steamID, ulong source_id)
        {
            return SteamEmulator.SteamFriends.IsUserInSource(steamID, source_id);
        }

        public void SetInGameVoiceSpeaking(IntPtr _, ulong steamID, bool speaking)
        {
            SteamEmulator.SteamFriends.SetInGameVoiceSpeaking(steamID, speaking);
        }

        public void ActivateGameOverlay(IntPtr _, string dialog)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlay(dialog);
        }

        public void ActivateGameOverlayToUser(IntPtr _, string dialog, ulong steamID)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayToUser(dialog, steamID);
        }

        public void ActivateGameOverlayToWebPage(IntPtr _, string pchURL, int eMode)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayToWebPage(pchURL, eMode);
        }

        public void ActivateGameOverlayToStore(IntPtr _, uint app_id, uint flag)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayToStore(app_id, flag);
        }

        public void SetPlayedWith(IntPtr _, ulong steamID)
        {
            SteamEmulator.SteamFriends.SetPlayedWith(steamID);
        }

        public void ActivateGameOverlayInviteDialog(IntPtr _, ulong steamID)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayInviteDialog(steamID);
        }

        public int GetSmallFriendAvatar(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetSmallFriendAvatar(steamID);
        }

        public int GetMediumFriendAvatar(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetMediumFriendAvatar(steamID);
        }

        public int GetLargeFriendAvatar(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetLargeFriendAvatar(steamID);
        }

        public bool RequestUserInformation(IntPtr _, ulong steamID, bool require_name_only)
        {
            return SteamEmulator.SteamFriends.RequestUserInformation(steamID, require_name_only);
        }

        public SteamAPICall_t RequestClanOfficerList(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.RequestClanOfficerList(steamID);
        }

        public CSteamID GetClanOwner(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetClanOwner(steamID);
        }

        public int GetClanOfficerCount(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetClanOfficerCount(steamID);
        }

        public CSteamID GetClanOfficerByIndex(IntPtr _, ulong clan, int officer)
        {
            return SteamEmulator.SteamFriends.GetClanOfficerByIndex(clan, officer);
        }

        public uint GetUserRestrictions(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetUserRestrictions();
        }

        public bool SetRichPresence(IntPtr _, string key, string value)
        {
            return SteamEmulator.SteamFriends.SetRichPresence(key, value);
        }

        public void ClearRichPresence(IntPtr _)
        {
            SteamEmulator.SteamFriends.ClearRichPresence();
        }

        public string GetFriendRichPresence(IntPtr _, ulong steamID, string key)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresence(steamID, key);
        }

        public int GetFriendRichPresenceKeyCount(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyCount(steamID);
        }

        public string GetFriendRichPresenceKeyByIndex(IntPtr _, ulong steamID, int key)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyByIndex(steamID, key);
        }
        public void RequestFriendRichPresence(IntPtr _, ulong steamID)
        {
            SteamEmulator.SteamFriends.RequestFriendRichPresence(steamID);
        }

        public bool InviteUserToGame(IntPtr _, ulong steamID, string connect)
        {
            return SteamEmulator.SteamFriends.InviteUserToGame(steamID, connect);
        }

        public int GetCoplayFriendCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetCoplayFriendCount();
        }

        public CSteamID GetCoplayFriend(IntPtr _, int index)
        {
            return SteamEmulator.SteamFriends.GetCoplayFriend(index);
        }

        public int GetFriendCoplayTime(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetFriendCoplayTime(steamID);
        }

        public uint GetFriendCoplayGame(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetFriendCoplayGame(steamID);
        }

        public SteamAPICall_t JoinClanChatRoom(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.JoinClanChatRoom(steamID);
        }
        public bool LeaveClanChatRoom(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.LeaveClanChatRoom(steamID);
        }

        public int GetClanChatMemberCount(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetClanChatMemberCount(steamID);
        }

        public CSteamID GetChatMemberByIndex(IntPtr _, ulong steamID, int index)
        {
            return SteamEmulator.SteamFriends.GetChatMemberByIndex(steamID, index);
        }

        public bool SendClanChatMessage(IntPtr _, ulong steamID, string msg)
        {
            return SteamEmulator.SteamFriends.SendClanChatMessage(steamID, msg);
        }

        public int GetClanChatMessage(IntPtr _, ulong steamID, int index, IntPtr text_out, int max_text, uint chat_type, ref ulong chater_id)
        {
            return SteamEmulator.SteamFriends.GetClanChatMessage(steamID, index, text_out, max_text, (int)chat_type, ref chater_id);
        }

        public bool IsClanChatAdmin(IntPtr _, ulong chat_id, ulong user_id)
        {
            return SteamEmulator.SteamFriends.IsClanChatAdmin(chat_id, user_id);
        }

        public bool IsClanChatWindowOpenInSteam(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.IsClanChatWindowOpenInSteam(steamID);
        }

        public bool OpenClanChatWindowInSteam(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.OpenClanChatWindowInSteam(steamID);
        }

        public bool CloseClanChatWindowInSteam(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.CloseClanChatWindowInSteam(steamID);
        }

        public bool SetListenForFriendsMessages(IntPtr _, bool intercept)
        {
            return SteamEmulator.SteamFriends.SetListenForFriendsMessages(intercept);
        }

        public bool ReplyToFriendMessage(IntPtr _, ulong steamID, string msg)
        {
            return SteamEmulator.SteamFriends.ReplyToFriendMessage(steamID, msg);
        }

        public int GetFriendMessage(IntPtr _, ulong steamID, int msg_index, IntPtr b_pointer, int b_length, ref uint msg_type)
        {
            return SteamEmulator.SteamFriends.GetFriendMessage(steamID, msg_index, b_pointer, b_length, msg_type);
        }

        public SteamAPICall_t GetFollowerCount(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.GetFollowerCount(steamID);
        }

        public SteamAPICall_t IsFollowing(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.IsFollowing(steamID);
        }

        public SteamAPICall_t EnumerateFollowingList(IntPtr _, uint starting_index)
        {
            return SteamEmulator.SteamFriends.EnumerateFollowingList(starting_index);
        }

        public bool IsClanPublic(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.IsClanPublic(steamID);
        }

        public bool IsClanOfficialGameGroup(IntPtr _, ulong steamID)
        {
            return SteamEmulator.SteamFriends.IsClanOfficialGameGroup(steamID);
        }

        public int GetNumChatsWithUnreadPriorityMessages(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetNumChatsWithUnreadPriorityMessages();
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
