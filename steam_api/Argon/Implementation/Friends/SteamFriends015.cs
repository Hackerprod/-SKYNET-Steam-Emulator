using System;

using Core.Interface;

namespace InterfaceFriends
{
    [Impl(Name = "SteamFriends015", ServerMapped = true)]
    public class SteamFriends015 : IBaseInterface
    {
        public string GetPersonaName()
        {
            return "Pankracio";
        }

        public int SetPersonaName(string name)
        {
            return 1;
        }

        public uint GetPersonaState()
        {
            return (uint)1;
        }

        public int GetFriendCount()
        {
            return 200;
        }

        public ulong GetFriendByIndex(int index)
        {
            return 1;
        }
        public uint GetFriendRelationship(ulong steam_id)
        {
            return (uint)1;
        }

        public uint GetFriendPersonaState(ulong steam_id)
        {
            return (uint)1;
        }

        public string GetFriendPersonaName(ulong steam_id)
        {
            return "asd";
        }

        public bool GetFriendGamePlayed(ulong steam_id, IntPtr friend_game_info_out)
        {
            return false;
        }

        public string GetFriendPersonaNameHistory(ulong steam_id, int index)
        {
            return "";
        }

        public int GetFriendSteamLevel(ulong steam_id)
        {
            return 0;
        }

        public string GetPlayerNickname(ulong steam_id)
        {
            return GetFriendPersonaName(steam_id);
        }

        // TODO: for groups we need to implement the FriendsGroupID_t
        // THESE ARE NOT CLANS!
        public int GetFriendsGroupCount()
        {
            return 0;
        }

        public ushort GetGroupIdByIndex(int index)
        {
            return 0;
        }

        public string GetFriendsGroupName(ushort id)
        {
            return "";
        }

        public int GetFriendsGroupMembersCount(ushort id)
        {
            return 0;
        }

        public void GetFriendsGroupMembersList(ushort id, ref IntPtr steam_id_out, int max_steam_id_out)
        {

        }

        public bool HasFriend(ulong steam_id)
        {
            return true;
        }

        public int GetClanCount()
        {
            return 0;
        }

        public ulong GetClanByIndex(int index)
        {
            return 0;
        }

        public string GetClanName(ulong steam_id)
        {
            return "SKYNET";
        }

        public string GetClanTag(ulong steam_id)
        {
            return "";
        }

        public bool GetClanActivityCounts(ulong steam_id, ref int online, ref int in_game, ref int chatting)
        {
            return false;
        }

        public int DownloadClanActivityCounts(ulong[] clans, int count)
        {
            // async call
            return 0;
        }

        public int GetFriendCountFromSource(ulong source_id)
        {
            return 0;
        }

        public ulong GetFriendFromSourceByIndex(ulong source_id, int index)
        {
            return 0;
        }

        public bool IsUserInSource(ulong steam_id, ulong source_id)
        {
            return false;
        }

        public void SetInGameVoiceSpeaking(ulong steam_id, bool speaking)
        {

        }

        public void ActivateGameOverlay(string dialog)
        {

        }

        public void ActiveGameOverlayToUser(string dialog, ulong steam_id)
        {

        }

        public void ActiveGameOverlayToWebPage(string url)
        {

        }

        public void ActivateGameOverlayToStore(uint app_id, uint flag)
        {

        }

        public void SetPlayedWith(ulong steam_id)
        {

        }

        public void ActivateGameOverlayInviteDialog(ulong steam_id)
        {

        }

        public int GetSmallFriendAvatar(ulong steam_id)
        {
            return 0;
        }
        public int GetMediumFriendAvatar(ulong steam_id)
        {
            return 0;
        }
        public int GetLargeFriendAvatar(ulong steam_id)
        {
            return 0;
        }

        public bool RequestUserInformation(ulong steam_id, bool require_name_only)
        {
            return true;
        }

        public int RequestClanOfficerList(ulong steam_id)
        {
            return 0;
        }

        public ulong GetClanOwner(ulong steam_id)
        {
            return 0;
        }

        public int GetClanOfficerCount(ulong steam_id)
        {
            return 0;
        }

        public ulong GetClanOfficerByIndex(ulong clan, int officer)
        {
            return 0;
        }

        public uint GetUserRestrictions()
        {
            return 0;
        }

        public bool SetRichPresence(string key, string value)
        {
            return false;
        }

        public bool ClearRichPresence()
        {
            return false;
        }

        public string GetFriendRichPresence(ulong steam_id, string key)
        {
            return "";
        }

        public int GetFriendRichPresenceKeyCount(ulong steam_id)
        {
            return 0;
        }

        public string GetFriendRichPresenceKeyByIndex(ulong steam_id, int key)
        {
            return "";
        }

        public void RequestFriendRichPresence(ulong steam_id)
        {

        }

        public bool InviteUserToGame(ulong steam_id, string connect)
        {
            return false;
        }

        public int GetCoplayFriendCount()
        {
            return 0;
        }

        public ulong GetCoplayFriend(int index)
        {
            return 0;
        }

        public int GetFriendCoplayTime(ulong steam_id)
        {
            return 0;
        }

        public uint GetFriendCoplayGame(ulong steam_id)
        {
            return 0;
        }

        public int JoinClanChatRoom(ulong steam_id)
        {
            // async job
            return 0;
        }

        public bool LeaveClanChatRoom(ulong steam_id)
        {
            return false;
        }

        public int GetClanChatMemberCount(ulong steam_id)
        {
            return 0;
        }

        public ulong GetChatMemberByIndex(ulong steam_id, int index)
        {
            return 0;
        }

        public bool SendClanChatMessage(ulong steam_id, string msg)
        {
            return false;
        }

        public int GetClanChatMessage(ulong steam_id, int index, IntPtr text_out, int max_text, uint chat_type, ref ulong chater_id)
        {
            return 0;
        }

        public bool IsClanChatAdmin(ulong chat_id, ulong user_id)
        {
            return false;
        }

        public bool IsClanChatWindowOpenInSteam(ulong steam_id)
        {
            return false;
        }

        public bool OpenClanChatWindowInSteam(ulong steam_id)
        {
            return false;
        }

        public bool CloseClanChatWindowInSteam(ulong steam_id)
        {
            return false;
        }

        public bool SetListenForFriendsMessages(bool intercept)
        {
            return false;
        }

        public bool ReplyToFriendMessage(ulong steam_id, string msg)
        {
            return false;
        }

        // Real sig is int GetFriendMessage(u64, i32, void *, i32, u32 *)
        [Buffer(Index = 2, NewPointerIndex = 2, NewSizeIndex = 3)]
        public int GetFriendMessage(ulong steam_id, int msg_index, ref IntPtr b, ref uint msg_type)
        {
            return 0;
        }

        public int GetFollowerCount(ulong steam_id)
        {
            // async call
            return 0;
        }

        public int IsFollowing(ulong steam_id)
        {
            // async call
            return 0;
        }

        public int EnumerateFollowingList(uint starting_index)
        {
            // async call
            return 0;
        }

        public bool IsClanPublic(ulong steam_id)
        {
            return false;
        }

        public bool IsClanOfficialGameGroup(ulong steam_id)
        {
            return false;
        }
    }
}