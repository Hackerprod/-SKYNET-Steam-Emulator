using SKYNET.Steamworks;
using SKYNET.Steamworks.Types;
using SKYNET.Types;
using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Interface
{
    [Interface("SteamFriends017")]
    public class SteamFriends017_ : ISteamInterface
    {
        public string GetPersonaName(IntPtr _)
        {

            return "Federiko";
        }
        public int SetPersonaName(IntPtr _, string name)
        {



            return 1;
        }
        public uint GetPersonaState(IntPtr _)
        {

            return 1;
        }
        public int GetFriendCount(IntPtr _)
        {
            return 0;
        }
        public ulong GetFriendByIndex(IntPtr _, int index)
        {
            return 0;
        }
        public uint GetFriendRelationship(IntPtr _, ulong steam_id)
        {

            return 0;
        }
        public uint GetFriendPersonaState(IntPtr _, ulong steam_id)
        {

            return 0;
        }
        public string GetFriendPersonaName(IntPtr _, ulong steam_id)
        {
            SteamEmulator.Write("xd", "--------------------------------------- GetFriendPersonaName " + steam_id);
            return "Fefo";
        }
        public bool GetFriendGamePlayed(IntPtr _, ulong steam_id, IntPtr friend_game_info_out)
        {


            return false;
        }
        public string GetFriendPersonaNameHistory(IntPtr _, ulong steam_id, int index)
        {


            return "";
        }
        public int GetFriendSteamLevel(IntPtr _, ulong steam_id)
        {

            return 0;
        }
        public string GetPlayerNickname(IntPtr _, ulong steam_id)
        {


            return "";
        }
        public int GetFriendsGroupCount(IntPtr _)
        {


            return 0;
        }
        public short GetGroupIdByIndex(IntPtr _, int index)
        {



            return 0;
        }
        public string GetFriendsGroupName(IntPtr _, short id)
        {


            return "";
        }
        public int GetFriendsGroupMembersCount(IntPtr _, short id)
        {

            return 0;
        }
        public void GetFriendsGroupMembersList(IntPtr _, short id, ref IntPtr steam_id_out, int max_steam_id_out)
        {

            steam_id_out = IntPtr.Zero;


        }
        public bool HasFriend(IntPtr _, ulong steam_id)
        {

            return false;
        }
        public int GetClanCount(IntPtr _)
        {


            return 0;
        }
        public ulong GetClanByIndex(IntPtr _, int index)
        {


            return 0;
        }
        public string GetClanName(IntPtr _, ulong steam_id)
        {


            return "";
        }
        public string GetClanTag(IntPtr _, ulong steam_id)
        {


            return "";
        }
        public bool GetClanActivityCounts(IntPtr _, ulong steam_id, ref int online, ref int in_game, ref int chatting)
        {


            online = 0;
            in_game = 0;
            chatting = 0;


            return false;
        }
        public int DownloadClanActivityCounts(IntPtr _, System.UInt64[] clans, int count)
        {


            return 0;
        }
        public int GetFriendCountFromSource(IntPtr _, ulong source_id)
        {


            return 0;
        }
        public ulong GetFriendFromSourceByIndex(IntPtr _, ulong source_id, int index)
        {


            return 0;
        }
        public bool IsUserInSource(IntPtr _, ulong steam_id, ulong source_id)
        {



            return false;
        }
        public void SetInGameVoiceSpeaking(IntPtr _, ulong steam_id, bool speaking)
        {



        }
        public void ActivateGameOverlay(IntPtr _, string dialog)
        {




        }
        public void ActiveGameOverlayToUser(IntPtr _, string dialog, ulong steam_id)
        {



        }
        public void ActiveGameOverlayToWebPage(IntPtr _, string url)
        {


        }
        public void ActivateGameOverlayToStore(IntPtr _, uint app_id, uint flag)
        {


        }
        public void SetPlayedWith(IntPtr _, ulong steam_id)
        {


        }
        public void ActivateGameOverlayInviteDialog(IntPtr _, ulong steam_id)
        {




        }
        public int GetSmallFriendAvatar(IntPtr _, ulong steam_id)
        {




            return 0;
        }
        public int GetMediumFriendAvatar(IntPtr _, ulong steam_id)
        {





            return 0;
        }
        public int GetLargeFriendAvatar(IntPtr _, ulong steam_id)
        {





            return 0;
        }
        public bool RequestUserInformation(IntPtr _, ulong steam_id, bool require_name_only)
        {





            return false;
        }
        public int RequestClanOfficerList(IntPtr _, ulong steam_id)
        {




            return 0;
        }
        public ulong GetClanOwner(IntPtr _, ulong steam_id)
        {




            return 0;
        }
        public int GetClanOfficerCount(IntPtr _, ulong steam_id)
        {




            return 0;
        }
        public ulong GetClanOfficerByIndex(IntPtr _, ulong clan, int officer)
        {




            return 0;
        }
        public uint GetUserRestrictions(IntPtr _)
        {




            return 0;
        }
        public bool SetRichPresence(IntPtr _, string key, string value)
        {





            return false;
        }
        public bool ClearRichPresence(IntPtr _)
        {


            return false;
        }
        public string GetFriendRichPresence(IntPtr _, ulong steam_id, string key)
        {



            return "";
        }
        public int GetFriendRichPresenceKeyCount(IntPtr _, ulong steam_id)
        {



            return 0;
        }
        public string GetFriendRichPresenceKeyByIndex(IntPtr _, ulong steam_id, int key)
        {



            return "";
        }
        public void RequestFriendRichPresence(IntPtr _, ulong steam_id)
        {









        }
        public bool InviteUserToGame(IntPtr _, ulong steam_id, string connect)
        {









            return false;
        }
        public int GetCoplayFriendCount(IntPtr _)
        {









            return 0;
        }
        public ulong GetCoplayFriend(IntPtr _, int index)
        {









            return 0;
        }
        public int GetFriendCoplayTime(IntPtr _, ulong steam_id)
        {









            return 0;
        }
        public uint GetFriendCoplayGame(IntPtr _, ulong steam_id)
        {









            return 0;
        }
        public int JoinClanChatRoom(IntPtr _, ulong steam_id)
        {









            return 0;
        }
        public bool LeaveClanChatRoom(IntPtr _, ulong steam_id)
        {









            return false;
        }
        public int GetClanChatMemberCount(IntPtr _, ulong steam_id)
        {









            return 0;
        }
        public ulong GetChatMemberByIndex(IntPtr _, ulong steam_id, int index)
        {









            return 0;
        }
        public bool SendClanChatMessage(IntPtr _, ulong steam_id, string msg)
        {









            return false;
        }
        public int GetClanChatMessage(IntPtr _, ulong steam_id, int index, IntPtr text_out, int max_text, uint chat_type, ref ulong chater_id)
        {







            chater_id = 0;


            return 0;
        }
        public bool IsClanChatAdmin(IntPtr _, ulong chat_id, ulong user_id)
        {









            return false;
        }
        public bool IsClanChatWindowOpenInSteam(IntPtr _, ulong steam_id)
        {









            return false;
        }
        public bool OpenClanChatWindowInSteam(IntPtr _, ulong steam_id)
        {









            return false;
        }
        public bool CloseClanChatWindowInSteam(IntPtr _, ulong steam_id)
        {









            return false;
        }
        public bool SetListenForFriendsMessages(IntPtr _, bool intercept)
        {









            return false;
        }
        public bool ReplyToFriendMessage(IntPtr _, ulong steam_id, string msg)
        {









            return false;
        }
        public int GetFriendMessage(IntPtr _, ulong steam_id, int msg_index, IntPtr b_pointer, int b_length, ref uint msg_type)
        {
            return 0;
        }
        public int GetFollowerCount(IntPtr _, ulong steam_id)
        {









            return 0;
        }
        public int IsFollowing(IntPtr _, ulong steam_id)
        {









            return 0;
        }
        public int EnumerateFollowingList(IntPtr _, uint starting_index)
        {









            return 0;
        }
        public bool IsClanPublic(IntPtr _, ulong steam_id)
        {









            return false;
        }
        public bool IsClanOfficialGameGroup(IntPtr _, ulong steam_id)
        {









            return false;
        }
        public int GetNumChatsWithUnreadPriorityMessages(IntPtr _)
        {
            return SteamEmulator.SteamFriends.GetNumChatsWithUnreadPriorityMessages();
        }

        public void ActivateGameOverlayRemotePlayTogetherInviteDialog(IntPtr _, SteamID steamIDLobby)
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
