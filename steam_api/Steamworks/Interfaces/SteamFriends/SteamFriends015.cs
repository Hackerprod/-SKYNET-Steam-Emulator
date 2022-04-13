using SKYNET.Steamworks;
using SKYNET.Steamworks.Types;
using System;


namespace SKYNET.Interface
{
    [Interface("SteamFriends015")]
    public class SteamFriends015 : ISteamInterface
    {
        public string GetPersonaName(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetPersonaName();
        }
        public int SetPersonaName(IntPtr _, string name)
        {
            return (int)SteamEmulator.SteamFriends.SetPersonaName(name);
        }
        public uint GetPersonaState(IntPtr _)
        {
            return (uint)SteamEmulator.SteamFriends.GetPersonaState();
        }
        public int GetFriendCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetFriendCount(0);
        }
        public ulong GetFriendByIndex(IntPtr _, int index)
        {
            return (ulong)SteamEmulator.SteamFriends.GetFriendByIndex(index, 0);
        }
        public uint GetFriendRelationship(IntPtr _, SteamID steam_id)
        {
            return (uint)SteamEmulator.SteamFriends.GetFriendRelationship(steam_id);
        }
        public uint GetFriendPersonaState(IntPtr _, SteamID steam_id)
        {
            return (uint)SteamEmulator.SteamFriends.GetFriendPersonaState(steam_id);
        }
        public string GetFriendPersonaName(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.GetFriendPersonaName(steam_id);
        }
        public bool GetFriendGamePlayed(IntPtr _, SteamID steam_id, IntPtr friend_game_info_out)
        {
            return SteamEmulator.SteamFriends.GetFriendGamePlayed(steam_id, default);
        }
        public string GetFriendPersonaNameHistory(IntPtr _, SteamID steam_id, int index)
        {
            return SteamEmulator.SteamFriends.GetFriendPersonaNameHistory(steam_id, index);
        }
        public int GetFriendSteamLevel(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.GetFriendSteamLevel(steam_id);
        }
        public string GetPlayerNickname(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.GetPlayerNickname(steam_id);
        }
        public int GetFriendsGroupCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupCount();
        }
        public short GetGroupIdByIndex(IntPtr _, int index)
        {
            //return SteamEmulator.SteamFriends.GetGroupIdByIndex(index);
            return 0;
        }
        public string GetFriendsGroupName(IntPtr _, short id)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupName(default);
        }
        public int GetFriendsGroupMembersCount(IntPtr _, short id)
        {
            return SteamEmulator.SteamFriends.GetFriendsGroupMembersCount(default);
        }
        public void GetFriendsGroupMembersList(IntPtr _, short id, SteamID steam_id_out, int max_steam_id_out)
        {
            SteamEmulator.SteamFriends.GetFriendsGroupMembersList(id, steam_id_out, max_steam_id_out);
        }
        public bool HasFriend(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.HasFriend(steam_id, 0);
        }
        public int GetClanCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetClanCount();
        }
        public ulong GetClanByIndex(IntPtr _, int index)
        {
            return (ulong)SteamEmulator.SteamFriends.GetClanByIndex(index);
        }
        public string GetClanName(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.GetClanName(steam_id);
        }
        public string GetClanTag(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.GetClanTag(steam_id);
        }
        public bool GetClanActivityCounts(IntPtr _, SteamID steam_id, ref int online, ref int in_game, ref int chatting)
        {
            return SteamEmulator.SteamFriends.GetClanActivityCounts(steam_id, 0, 0, 0);
        }
        public int DownloadClanActivityCounts(IntPtr _, ulong clans, int count)
        {
            return (int)SteamEmulator.SteamFriends.DownloadClanActivityCounts(clans, count);
        }
        public int GetFriendCountFromSource(IntPtr _, ulong source_id)
        {
            return SteamEmulator.SteamFriends.GetFriendCountFromSource(source_id);
        }
        public ulong GetFriendFromSourceByIndex(IntPtr _, ulong source_id, int index)
        {
            return (ulong)SteamEmulator.SteamFriends.GetFriendFromSourceByIndex(source_id, index);
        }
        public bool IsUserInSource(IntPtr _, SteamID steam_id, ulong source_id)
        {
            return SteamEmulator.SteamFriends.IsUserInSource(steam_id, source_id);
        }
        public void SetInGameVoiceSpeaking(IntPtr _, SteamID steam_id, bool speaking)
        {
            SteamEmulator.SteamFriends.SetInGameVoiceSpeaking(steam_id, speaking);
        }
        public void ActivateGameOverlay(IntPtr _, string dialog)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlay(dialog);
        }
        public void ActiveGameOverlayToUser(IntPtr _, string dialog, SteamID steam_id)
        {
            //SteamEmulator.SteamFriends.ActiveGameOverlayToUser(dialog, steam_id);
        }
        public void ActiveGameOverlayToWebPage(IntPtr _, string url)
        {
            //SteamEmulator.SteamFriends.ActiveGameOverlayToWebPage();
        }
        public void ActivateGameOverlayToStore(IntPtr _, uint app_id, uint flag)
        {
            //SteamEmulator.SteamFriends.ActivateGameOverlayToStore();
        }
        public void SetPlayedWith(IntPtr _, SteamID steam_id)
        {
            SteamEmulator.SteamFriends.SetPlayedWith(steam_id);
        }
        public void ActivateGameOverlayInviteDialog(IntPtr _, SteamID steam_id)
        {
            SteamEmulator.SteamFriends.ActivateGameOverlayInviteDialog(steam_id);
        }
        public int GetSmallFriendAvatar(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.GetSmallFriendAvatar(steam_id);
        }
        public int GetMediumFriendAvatar(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.GetMediumFriendAvatar(steam_id);
        }
        public int GetLargeFriendAvatar(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.GetLargeFriendAvatar(steam_id);
        }
        public bool RequestUserInformation(IntPtr _, SteamID steam_id, bool require_name_only)
        {
            return SteamEmulator.SteamFriends.RequestUserInformation(steam_id, require_name_only);
        }
        public int RequestClanOfficerList(IntPtr _, SteamID steam_id)
        {
            return (int)SteamEmulator.SteamFriends.RequestClanOfficerList(steam_id);
        }
        public ulong GetClanOwner(IntPtr _, SteamID steam_id)
        {
            return (ulong)SteamEmulator.SteamFriends.GetClanOwner(steam_id);
        }
        public int GetClanOfficerCount(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.GetClanOfficerCount(steam_id);
        }
        public ulong GetClanOfficerByIndex(IntPtr _, ulong clan, int officer)
        {
            return (ulong)SteamEmulator.SteamFriends.GetClanOfficerByIndex(clan, officer);
        }
        public uint GetUserRestrictions(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetUserRestrictions();
        }
        public bool SetRichPresence(IntPtr _, string key, string value)
        {
            return SteamEmulator.SteamFriends.SetRichPresence(key, value);
        }
        public bool ClearRichPresence(IntPtr _)
        {
            // SteamEmulator.SteamFriends.ClearRichPresence();
            return true;
        }
        public string GetFriendRichPresence(IntPtr _, SteamID steam_id, string key)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresence(steam_id, key);
        }
        public int GetFriendRichPresenceKeyCount(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyCount(steam_id);
        }
        public string GetFriendRichPresenceKeyByIndex(IntPtr _, SteamID steam_id, int key)
        {
            return SteamEmulator.SteamFriends.GetFriendRichPresenceKeyByIndex(steam_id, key);
        }
        public void RequestFriendRichPresence(IntPtr _, SteamID steam_id)
        {
            SteamEmulator.SteamFriends.RequestFriendRichPresence(steam_id);
        }
        public bool InviteUserToGame(IntPtr _, SteamID steam_id, string connect)
        {
            return SteamEmulator.SteamFriends.InviteUserToGame(steam_id, connect);
        }
        public int GetCoplayFriendCount(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetCoplayFriendCount();
        }
        public ulong GetCoplayFriend(IntPtr _, int index)
        {
            return (ulong)SteamEmulator.SteamFriends.GetCoplayFriend(index);
        }
        public int GetFriendCoplayTime(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.GetFriendCoplayTime(steam_id);
        }
        public uint GetFriendCoplayGame(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.GetFriendCoplayGame(steam_id);
        }
        public int JoinClanChatRoom(IntPtr _, SteamID steam_id)
        {
            return (int)SteamEmulator.SteamFriends.JoinClanChatRoom(steam_id);
        }
        public bool LeaveClanChatRoom(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.LeaveClanChatRoom(steam_id);
        }
        public int GetClanChatMemberCount(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.GetClanChatMemberCount(steam_id);
        }
        public ulong GetChatMemberByIndex(IntPtr _, SteamID steam_id, int index)
        {
            return (ulong)SteamEmulator.SteamFriends.GetChatMemberByIndex(steam_id, index);
        }
        public bool SendClanChatMessage(IntPtr _, SteamID steam_id, string msg)
        {
            return SteamEmulator.SteamFriends.SendClanChatMessage(steam_id, msg);
        }
        public int GetClanChatMessage(IntPtr _, SteamID steam_id, int index, IntPtr text_out, int max_text, int chat_type, ulong chater_id)
        {
            return SteamEmulator.SteamFriends.GetClanChatMessage(steam_id, index, text_out, max_text, chat_type, chater_id);
        }
        public bool IsClanChatAdmin(IntPtr _, ulong chat_id, ulong user_id)
        {
            return SteamEmulator.SteamFriends.IsClanChatAdmin(chat_id, user_id);
        }
        public bool IsClanChatWindowOpenInSteam(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.IsClanChatWindowOpenInSteam(steam_id);
        }
        public bool OpenClanChatWindowInSteam(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.OpenClanChatWindowInSteam(steam_id);
        }
        public bool CloseClanChatWindowInSteam(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.CloseClanChatWindowInSteam(steam_id);
        }
        public bool SetListenForFriendsMessages(IntPtr _, bool intercept)
        {
            return SteamEmulator.SteamFriends.SetListenForFriendsMessages(intercept);
        }
        public bool ReplyToFriendMessage(IntPtr _, SteamID steam_id, string msg)
        {
            return SteamEmulator.SteamFriends.ReplyToFriendMessage(steam_id, msg);
        }
        public int GetFriendMessage(IntPtr _, SteamID steam_id, int msg_index, IntPtr b_pointer, int b_length, ref uint msg_type)
        {
            return SteamEmulator.SteamFriends.GetFriendMessage(steam_id, msg_index, b_pointer, b_length, (EChatEntryType)msg_type);
        }
        public int GetFollowerCount(IntPtr _, SteamID steam_id)
        {
            return (int)SteamEmulator.SteamFriends.GetFollowerCount(steam_id);
        }
        public int IsFollowing(IntPtr _, SteamID steam_id)
        {
            return (int)SteamEmulator.SteamFriends.IsFollowing(steam_id);
        }
        public int EnumerateFollowingList(IntPtr _, uint starting_index)
        {
            return (int)SteamEmulator.SteamFriends.EnumerateFollowingList(starting_index);
        }
        public bool IsClanPublic(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.IsClanPublic(steam_id);
        }
        public bool IsClanOfficialGameGroup(IntPtr _, SteamID steam_id)
        {
            return SteamEmulator.SteamFriends.IsClanOfficialGameGroup(steam_id);
        }
    }
}
