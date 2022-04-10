using SKYNET.Steamworks;
using System;


namespace SKYNET.Interface
{
    [Interface("SteamFriends015")]
    public class SteamFriends015 : ISteamInterface
    {
        public string GetPersonaName(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetPersonaName(_);
        }
        public int SetPersonaName(IntPtr _, string name)
        {
            return (int)SteamEmulator.SteamFriends.SetPersonaName(_, name).m_SteamAPICall;
        }
        public uint GetPersonaState(IntPtr _)
        {
            return (uint)SteamEmulator.SteamFriends.GetPersonaState(_);
        }
        public int GetFriendCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetFriendCount(_, 0);
        }
        public ulong GetFriendByIndex(IntPtr _, int index)
        {
            return (ulong)SteamEmulator.SteamFriends.GetFriendByIndex(_, index, 0);
        }
        public uint GetFriendRelationship(IntPtr _, ulong steam_id)
        {
            return (uint)SteamEmulator.SteamFriends.GetFriendRelationship(_, steam_id);
        }
        public uint GetFriendPersonaState(IntPtr _, ulong steam_id)
        {
            return (uint)SteamEmulator.SteamFriends.GetFriendPersonaState(_, steam_id);
        }
        public string GetFriendPersonaName(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.GetFriendPersonaName(_, steam_id);
        }
        public bool GetFriendGamePlayed(IntPtr _, ulong steam_id, IntPtr friend_game_info_out)
        {
            return SteamEmulator.SteamFriends.GetFriendGamePlayed(_, steam_id, default);
        }
        public string GetFriendPersonaNameHistory(IntPtr _, ulong steam_id, int index)
        {
            return SteamEmulator.SteamFriends.GetFriendPersonaNameHistory(_, steam_id, index);
        }
        public int GetFriendSteamLevel(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.GetFriendSteamLevel(_, steam_id);
        }
        public string GetPlayerNickname(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.GetPlayerNickname(_, steam_id);
        }
        public int GetFriendsGroupCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupCount(_);
        }
        public short GetGroupIdByIndex(IntPtr _, int index)
        {
            //return SteamEmulator.SteamFriends.GetGroupIdByIndex(_, index);
            return 0;
        }
        public string GetFriendsGroupName(IntPtr _, short id)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupName(_, default);
        }
        public int GetFriendsGroupMembersCount(IntPtr _, short id)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupMembersCount(_, default);
        }
        public void GetFriendsGroupMembersList(IntPtr _, short id, ref IntPtr steam_id_out, int max_steam_id_out)
        {
            SteamEmulator.SteamFriends.GetFriendsGroupMembersList(_, default, new IntPtr[0], max_steam_id_out);
        }
        public bool HasFriend(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.HasFriend(_, steam_id, 0);
        }
        public int GetClanCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetClanCount(_);
        }
        public ulong GetClanByIndex(IntPtr _, int index)
        {
            return (ulong)SteamEmulator.SteamFriends.GetClanByIndex(_, index);
        }
        public string GetClanName(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.GetClanName(_, steam_id);
        }
        public string GetClanTag(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.GetClanTag(_, steam_id);
        }
        public bool GetClanActivityCounts(IntPtr _, ulong steam_id, ref int online, ref int in_game, ref int chatting)
        {
            return SteamEmulator.SteamFriends.GetClanActivityCounts(_, steam_id, 0, 0, 0);
        }
        public int DownloadClanActivityCounts(IntPtr _, System.UInt64[] clans, int count)
        {
            return (int)SteamEmulator.SteamFriends.DownloadClanActivityCounts(_, new IntPtr[0], count).m_SteamAPICall;
        }
        public int GetFriendCountFromSource(IntPtr _, ulong source_id)
        {
            return SteamEmulator.SteamFriends.GetFriendCountFromSource(_, source_id);
        }
        public ulong GetFriendFromSourceByIndex(IntPtr _, ulong source_id, int index)
        {
            return (ulong)SteamEmulator.SteamFriends.GetFriendFromSourceByIndex(_, source_id, index);
        }
        public bool IsUserInSource(IntPtr _, ulong steam_id, ulong source_id)
        {
            return SteamEmulator.SteamFriends.IsUserInSource(_, steam_id, source_id);
        }
        public void SetInGameVoiceSpeaking(IntPtr _, ulong steam_id, bool speaking)
        {
            SteamEmulator.SteamFriends.SetInGameVoiceSpeaking(_, steam_id, speaking);
        }
        public void ActivateGameOverlay(IntPtr _, string dialog)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlay(_, dialog);
        }
        public void ActiveGameOverlayToUser(IntPtr _, string dialog, ulong steam_id)
        {
            //SteamEmulator.SteamFriends.ActiveGameOverlayToUser(_, dialog, steam_id);
        }
        public void ActiveGameOverlayToWebPage(IntPtr _, string url)
        {
            //SteamEmulator.SteamFriends.ActiveGameOverlayToWebPage(_);
        }
        public void ActivateGameOverlayToStore(IntPtr _, uint app_id, uint flag)
        {
            //SteamEmulator.SteamFriends.ActivateGameOverlayToStore(_);
        }
        public void SetPlayedWith(IntPtr _, ulong steam_id)
        {
            SteamEmulator.SteamFriends.SetPlayedWith(_, steam_id);
        }
        public void ActivateGameOverlayInviteDialog(IntPtr _, ulong steam_id)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayInviteDialog(_, steam_id);
        }
        public int GetSmallFriendAvatar(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.GetSmallFriendAvatar(_, steam_id);
        }
        public int GetMediumFriendAvatar(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.GetMediumFriendAvatar(_, steam_id);
        }
        public int GetLargeFriendAvatar(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.GetLargeFriendAvatar(_, steam_id);
        }
        public bool RequestUserInformation(IntPtr _, ulong steam_id, bool require_name_only)
        {
            return SteamEmulator.SteamFriends.RequestUserInformation(_, steam_id, require_name_only);
        }
        public int RequestClanOfficerList(IntPtr _, ulong steam_id)
        {
            return (int)SteamEmulator.SteamFriends.RequestClanOfficerList(_, steam_id).m_SteamAPICall;
        }
        public ulong GetClanOwner(IntPtr _, ulong steam_id)
        {
            return (ulong)SteamEmulator.SteamFriends.GetClanOwner(_, steam_id);
        }
        public int GetClanOfficerCount(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.GetClanOfficerCount(_, steam_id);
        }
        public ulong GetClanOfficerByIndex(IntPtr _, ulong clan, int officer)
        {
            return (ulong)SteamEmulator.SteamFriends.GetClanOfficerByIndex(_, clan, officer);
        }
        public uint GetUserRestrictions(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetUserRestrictions(_);
        }
        public bool SetRichPresence(IntPtr _, string key, string value)
        {
            return SteamEmulator.SteamFriends.SetRichPresence(_, key, value);
        }
        public bool ClearRichPresence(IntPtr _)
        {
            // SteamEmulator.SteamFriends.ClearRichPresence(_);
            return true;
        }
        public string GetFriendRichPresence(IntPtr _, ulong steam_id, string key)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresence(_, steam_id, key);
        }
        public int GetFriendRichPresenceKeyCount(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyCount(_, steam_id);
        }
        public string GetFriendRichPresenceKeyByIndex(IntPtr _, ulong steam_id, int key)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyByIndex(_, steam_id, key);
        }
        public void RequestFriendRichPresence(IntPtr _, ulong steam_id)
        {
            SteamEmulator.SteamFriends.RequestFriendRichPresence(_, steam_id);
        }
        public bool InviteUserToGame(IntPtr _, ulong steam_id, string connect)
        {
            return SteamEmulator.SteamFriends.InviteUserToGame(_, steam_id, connect);
        }
        public int GetCoplayFriendCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetCoplayFriendCount(_);
        }
        public ulong GetCoplayFriend(IntPtr _, int index)
        {
            return (ulong)SteamEmulator.SteamFriends.GetCoplayFriend(_, index);
        }
        public int GetFriendCoplayTime(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.GetFriendCoplayTime(_, steam_id);
        }
        public uint GetFriendCoplayGame(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.GetFriendCoplayGame(_, steam_id).m_AppId;
        }
        public int JoinClanChatRoom(IntPtr _, ulong steam_id)
        {
            return (int)SteamEmulator.SteamFriends.JoinClanChatRoom(_, steam_id).m_SteamAPICall;
        }
        public bool LeaveClanChatRoom(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.LeaveClanChatRoom(_, steam_id);
        }
        public int GetClanChatMemberCount(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.GetClanChatMemberCount(_, steam_id);
        }
        public ulong GetChatMemberByIndex(IntPtr _, ulong steam_id, int index)
        {
            return (ulong)SteamEmulator.SteamFriends.GetChatMemberByIndex(_, steam_id, index);
        }
        public bool SendClanChatMessage(IntPtr _, ulong steam_id, string msg)
        {
            return SteamEmulator.SteamFriends.SendClanChatMessage(_, steam_id, msg);
        }
        public int GetClanChatMessage(IntPtr _, ulong steam_id, int index, IntPtr text_out, int max_text, uint chat_type, ref ulong chater_id)
        {
            return SteamEmulator.SteamFriends.GetClanChatMessage(_, steam_id, index, text_out, max_text, (EChatEntryType)chat_type, new IntPtr[0]);
        }
        public bool IsClanChatAdmin(IntPtr _, ulong chat_id, ulong user_id)
        {
            return SteamEmulator.SteamFriends.IsClanChatAdmin(_, chat_id, user_id);
        }
        public bool IsClanChatWindowOpenInSteam(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.IsClanChatWindowOpenInSteam(_, steam_id);
        }
        public bool OpenClanChatWindowInSteam(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.OpenClanChatWindowInSteam(_, steam_id);
        }
        public bool CloseClanChatWindowInSteam(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.CloseClanChatWindowInSteam(_, steam_id);
        }
        public bool SetListenForFriendsMessages(IntPtr _, bool intercept)
        {
            return SteamEmulator.SteamFriends.SetListenForFriendsMessages(_, intercept);
        }
        public bool ReplyToFriendMessage(IntPtr _, ulong steam_id, string msg)
        {
            return SteamEmulator.SteamFriends.ReplyToFriendMessage(_, steam_id, msg);
        }
        public int GetFriendMessage(IntPtr _, ulong steam_id, int msg_index, IntPtr b_pointer, int b_length, ref uint msg_type)
        {
            return SteamEmulator.SteamFriends.GetFriendMessage(_, steam_id, msg_index, b_pointer, b_length, (EChatEntryType)msg_type);
        }
        public int GetFollowerCount(IntPtr _, ulong steam_id)
        {
            return (int)SteamEmulator.SteamFriends.GetFollowerCount(_, steam_id).m_SteamAPICall;
        }
        public int IsFollowing(IntPtr _, ulong steam_id)
        {
            return (int)SteamEmulator.SteamFriends.IsFollowing(_, steam_id).m_SteamAPICall;
        }
        public int EnumerateFollowingList(IntPtr _, uint starting_index)
        {
            return (int)SteamEmulator.SteamFriends.EnumerateFollowingList(_, starting_index).m_SteamAPICall;
        }
        public bool IsClanPublic(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.IsClanPublic(_, steam_id);
        }
        public bool IsClanOfficialGameGroup(IntPtr _, ulong steam_id)
        {
            return SteamEmulator.SteamFriends.IsClanOfficialGameGroup(_, steam_id);
        }
    }
}
