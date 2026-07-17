using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using SKYNET.Callback;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Types;
//using SKYNET.IPC.Types;
using SKYNET.Steamworks.Interfaces;

using SteamAPICall_t = System.UInt64;
using FriendsGroupID_t = System.UInt16;
using SKYNET.Network.Packets;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamFriends : ISteamInterface
    {
        public static SteamFriends Instance;

        public List<SKYNET.Types.SteamUser> Users;

        public List<ulong> QueryingAvatar;

        private ConcurrentDictionary<ulong, ImageAvatar> Avatars;
        private ConcurrentDictionary<int, ImageAvatar> AvatarsByHandle;
        private readonly object AvatarSync = new object();
        private readonly object AvatarCacheSync = new object();
        private int ImageIndex;
        private ImageAvatar DefaultAvatar;

        public SteamFriends()
        {
            Instance = this;
            InterfaceName = "SteamFriends";
            InterfaceVersion = "SteamFriends018";
            Users = new List<SKYNET.Types.SteamUser>();
            QueryingAvatar = new List<SteamAPICall_t>();
            Avatars = new ConcurrentDictionary<ulong, ImageAvatar>();
            AvatarsByHandle = new ConcurrentDictionary<int, ImageAvatar>();
            ImageIndex = 10;
        }

        public void Initialize()
        {
            #region Default Avatar

            EnsureDefaultAvatar();

            #endregion

            // The server owns avatar identity. Loading the local wallpaper here
            // used the pre-session SteamID and could assign our image to another
            // account when the active web user changed.
        }

        public void ReportUserChanged(ulong SteamID, EPersonaChange changeFlags)
        {
            PersonaStateChange_t data = new PersonaStateChange_t();
            data.m_ulSteamID = SteamID;
            data.m_nChangeFlags = (int)changeFlags;
            CallbackManager.AddCallback(data);
        }

        public string GetPersonaName()
        {
            APIClient.QueueSelfRefresh();
            string PersonaName = SteamEmulator.PersonaName;
            Write($"GetPersonaName {PersonaName}");
            return PersonaName;
        }

        public short GetGroupIdByIndex(int index)
        {
            Write($"GetGroupIdByIndex {index}");
            return 0;
        }

        public void ActivateGameOverlay(string friendsGroupID)
        {
            Write($"ActivateGameOverlay {friendsGroupID}");
            OpenOverlayDialog(friendsGroupID);
        }

        public void ActivateGameOverlayInviteDialog(ulong steamIDLobby)
        {
            try
            {
                Write($"ActivateGameOverlayInviteDialog (Lobby SteamID = {steamIDLobby})");
                OverlayManager.ShowInvite(steamIDLobby);
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        public void ActivateGameOverlayInviteDialogConnectString(string pchConnectString)
        {
            Write($"ActivateGameOverlayInviteDialogConnectString (URI = {pchConnectString})");
            OverlayManager.ShowInvite(0, pchConnectString);
        }

        public void ActivateGameOverlayRemotePlayTogetherInviteDialog(ulong steamIDLobby)
        {
            Write($"ActivateGameOverlayRemotePlayTogetherInviteDialog (Lobby SteamID = {steamIDLobby})");
            OverlayManager.ShowInvite(steamIDLobby);
        }

        public void ActivateGameOverlayToStore(uint nAppID, int eFlag)
        {
            Write($"ActivateGameOverlayToStore (AppID = {nAppID}, Flag = {eFlag})");
            OverlayManager.ShowStore(nAppID, eFlag);
        }

        public void ActivateGameOverlayToUser(string friendsGroupID, ulong steamID)
        {
            Write($"ActivateGameOverlayToUser (GroupID = {friendsGroupID}, SteamID = {(CSteamID)steamID})");
            switch ((friendsGroupID ?? string.Empty).ToLowerInvariant())
            {
                case "steamid":
                    OverlayManager.ShowUser("profile", steamID);
                    break;
                case "chat":
                    OverlayManager.ShowUser("chat", steamID);
                    break;
                case "jointrade":
                    OverlayManager.ShowUser("profile", steamID);
                    break;
                case "stats":
                    OverlayManager.ShowUser("stats", steamID);
                    break;
                case "achievements":
                    OverlayManager.ShowUser("achievements", steamID);
                    break;
                case "friendadd":
                    WorkQueue.Enqueue("Send friend request", () => APIClient.SendFriendRequest(steamID),
                        "friends:request:" + steamID);
                    OverlayManager.ShowUser("profile", steamID);
                    break;
                case "friendremove":
                    WorkQueue.Enqueue("Remove friend or request", () => APIClient.RemoveFriendOrRequest(steamID),
                        "friends:remove:" + steamID);
                    OverlayManager.ShowUser("profile", steamID);
                    break;
                case "friendrequestaccept":
                    WorkQueue.Enqueue("Accept friend request", () => APIClient.AcceptFriendRequest(steamID),
                        "friends:accept:" + steamID);
                    OverlayManager.ShowUser("profile", steamID);
                    break;
                case "friendrequestignore":
                    WorkQueue.Enqueue("Ignore friend request", () => APIClient.RemoveFriendOrRequest(steamID),
                        "friends:ignore:" + steamID);
                    OverlayManager.ShowUser("profile", steamID);
                    break;
                default:
                    OverlayManager.ShowUser("profile", steamID);
                    break;
            }
        }

        public void ActivateGameOverlayToWebPage(string pchURL, int eMode)
        {
            Write($"ActivateGameOverlayToWebPage {pchURL}");
            OverlayManager.ShowWebPage(pchURL, eMode);
        }

        private void OpenOverlayDialog(string dialog)
        {
            var normalized = (dialog ?? string.Empty).Trim().ToLowerInvariant();
            switch (normalized)
            {
                case "friends":
                case "players":
                case "chatroomgroup":
                    OverlayManager.ShowPeople("People");
                    break;

                case "settings":
                    OverlayManager.ShowSettings();
                    break;

                case "stats":
                    OverlayManager.ShowUser("stats", (ulong)SteamEmulator.SteamID);
                    break;

                case "achievements":
                    OverlayManager.ShowUser("achievements", (ulong)SteamEmulator.SteamID);
                    break;

                case "community":
                case "officialgamegroup":
                    OverlayManager.ShowHome("SKYNETEMU");
                    break;

                default:
                    if (normalized.StartsWith("chatroomgroup/", StringComparison.Ordinal))
                    {
                        OverlayManager.ShowPeople("People");
                    }
                    else
                    {
                        OverlayManager.ShowHome("SKYNETEMU");
                    }
                    break;
            }
        }

        public void ClearRichPresence()
        {
            Write($"ClearRichPresence");
            //IPCManager.SendClearRichPresence();
        }

        public bool CloseClanChatWindowInSteam(ulong steamIDClanChat)
        {
            Write($"CloseClanChatWindowInSteam {steamIDClanChat}");
            return true;
        }

        public SteamAPICall_t DownloadClanActivityCounts(IntPtr clans, int cClansToRequest)
        {
            Write($"DownloadClanActivityCounts {cClansToRequest}");
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t RequestEquippedProfileItems(ulong steamID)
        {
            Write($"RequestEquippedProfileItems (SteamID = {steamID})");
            return k_uAPICallInvalid;
        }

        public bool BHasEquippedProfileItem(ulong steamID, int itemType)
        {
            Write($"BHasEquippedProfileItem (SteamID = {steamID}, ItemType = {itemType})");
            return false;
        }

        public string GetProfileItemPropertyString(ulong steamID, int itemType, int prop)
        {
            Write($"GetProfileItemPropertyString (SteamID = {steamID}, ItemType = {itemType}, Prop = {prop})");
            return string.Empty;
        }

        public uint GetProfileItemPropertyUint(ulong steamID, int itemType, int prop)
        {
            Write($"GetProfileItemPropertyUint (SteamID = {steamID}, ItemType = {itemType}, Prop = {prop})");
            return 0;
        }

        public SteamAPICall_t EnumerateFollowingList(uint unStartIndex)
        {
            Write($"EnumerateFollowingList {unStartIndex}");
            // FriendsEnumerateFollowingList_t
            return k_uAPICallInvalid;
        }

        public CSteamID GetChatMemberByIndex(ulong steamIDClan, int iUser)
        {
            Write($"GetChatMemberByIndex {steamIDClan}");
            return CSteamID.Invalid;
        }

        public bool GetClanActivityCounts(ulong steamIDClan, ref int online, ref int in_game, ref int chatting)
        {
            Write($"ActivateGameOverlay {steamIDClan}");
            online = 0;
            in_game = 0;
            chatting = 0;
            return true;
        }

        public CSteamID GetClanByIndex(int iClan)
        {
            Write($"GetClanByIndex {iClan}");
            return CSteamID.Invalid;
        }

        public int GetClanChatMemberCount(ulong steamIDClan)
        {
            Write($"GetClanChatMemberCount {steamIDClan}");
            return 0;
        }

        public int GetClanChatMessage(ulong steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, IntPtr peChatEntryType, IntPtr psteamidChatter)
        {
            Write($"GetClanChatMessage {steamIDClanChat}");
            if (peChatEntryType != IntPtr.Zero)
            {
                Marshal.WriteInt32(peChatEntryType, (int)EChatEntryType.ChatMsg);
            }
            NativeSteamId.Write(psteamidChatter, CSteamID.Invalid);
            return 0;
        }

        public int GetClanChatMessage(ulong steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, int peChatEntryType, ref ulong[] psteamidChatter)
        {
            peChatEntryType = (int)EChatEntryType.ChatMsg;
            return GetClanChatMessage(steamIDClanChat, iMessage, prgchText, cchTextMax, IntPtr.Zero, IntPtr.Zero);
        }

        public int GetClanCount()
        {
            Write($"GetClanCount");
            return 0;
        }

        public string GetClanName(ulong steamIDClan)
        {
            Write($"GetClanName {steamIDClan}");
            return "";
        }

        public CSteamID GetClanOfficerByIndex(ulong steamIDClan, int iOfficer)
        {
            Write($"GetClanOfficerByIndex {steamIDClan}");
            return CSteamID.Invalid;
        }

        public int GetClanOfficerCount(ulong steamIDClan)
        {
            Write($"GetClanOfficerCount {steamIDClan}");
            return 0;
        }

        public CSteamID GetClanOwner(ulong steamIDClan)
        {
            Write($"GetClanOwner {steamIDClan}");
            return CSteamID.Invalid;
        }

        public string GetClanTag(ulong steamIDClan)
        {
            Write($"GetClanTag {steamIDClan}");
            return "";
        }

        public CSteamID GetCoplayFriend(int iCoplayFriend)
        {
            Write($"GetCoplayFriend {iCoplayFriend}");
            return CSteamID.Invalid;
        }

        public int GetCoplayFriendCount()
        {
            Write($"GetCoplayFriendCount");
            return 0;
        }

        public SteamAPICall_t GetFollowerCount(ulong steamID)
        {
            Write($"GetFollowerCount {steamID}");
            // FriendsGetFollowerCount_t
            return k_uAPICallInvalid;
        }

        public CSteamID GetFriendByIndex(int iFriend, int iFriendFlags)
        {
            var Friends = GetFriends(iFriendFlags);

            if (iFriend < 0 || iFriend >= Friends.Count)
            {
                Write($"GetFriendByIndex (Index = {iFriend}, FriendFlags = {iFriendFlags}) = invalid");
                return CSteamID.Invalid;
            }

            CSteamID Result = CSteamID.Invalid;
            MutexHelper.Wait("GetFriendByIndex", delegate
            {
                if (Friends.Count > iFriend)
                {
                    var friend = Friends[iFriend];
                    if (friend != null)
                    {
                        Result = new CSteamID(friend.SteamID);
                    }
                }
            });
            Write($"GetFriendByIndex (Index = {iFriend}, FriendFlags = {iFriendFlags}) = {Result.ToString()}");
            return Result;
        }

        public uint GetFriendCoplayGame(ulong steamIDFriend)
        {
            Write($"GetFriendCoplayGame {steamIDFriend}");
            return (uint)0;
        }

        public int GetFriendCoplayTime(ulong steamIDFriend)
        {
            Write($"GetFriendCoplayTime {steamIDFriend}");
            return 0;
        }

        public int GetFriendCount(int iFriendFlags)
        {
            int Result = 0;
            if (iFriendFlags != (int)EFriendFlags.k_EFriendFlagNone)
            {
                if (APIClient.IsEnabled)
                {
                    APIClient.QueueFriendsRefresh();
                }
                MutexHelper.Wait("Users", delegate
                {
                    var Friends = GetFriends(iFriendFlags);
                    Result = Friends.Count;
                });
            }
            Write($"GetFriendCount {Result}");
            return Result;
        }

        public int GetFriendCountFromSource(ulong steamIDSource)
        {
            Write($"GetFriendCountFromSource {steamIDSource}");
            return 0;
        }

        public CSteamID GetFriendFromSourceByIndex(ulong steamIDSource, int iFriend)
        {
            Write($"GetFriendFromSourceByIndex {steamIDSource} {iFriend}");
            return CSteamID.Invalid;
        }

        public bool GetFriendGamePlayed(ulong steamIDFriend, ref FriendGameInfo_t pFriendGameInfo)
        {
            bool Result = false;
            if (steamIDFriend == SteamEmulator.SteamID)
            {
                pFriendGameInfo.GameID = SteamEmulator.AppID;
                pFriendGameInfo.GameIP = 0;
                pFriendGameInfo.GamePort = 0;
                Result = true;
            }
            else
            {
                var friend = GetUser(steamIDFriend);
                if (friend == null || friend.GameID == 0)
                {
                    // No live game (offline / not playing) -> not "in game".
                    pFriendGameInfo.GameID = 0;
                    pFriendGameInfo.GameIP = 0;
                    pFriendGameInfo.GamePort = 0;
                    Result = false;
                }
                else
                {
                    pFriendGameInfo.GameID = friend.GameID;
                    pFriendGameInfo.GameIP = 0;
                    pFriendGameInfo.GamePort = 0;
                    pFriendGameInfo.steamIDLobby = friend.LobbyID;
                    Result = true;
                }
            }

            Write($"GetFriendGamePlayed (SteamID = {steamIDFriend}) = {Result}");
            return Result;
        }

        public int GetFriendMessage(ulong steamIDFriend, int iMessageID, IntPtr pvData, int cubData, IntPtr peChatEntryType)
        {
            Write($"GetFriendMessage {steamIDFriend}");
            if (peChatEntryType != IntPtr.Zero)
            {
                Marshal.WriteInt32(peChatEntryType, (int)EChatEntryType.ChatMsg);
            }
            return 0;
        }

        public int GetFriendMessage(ulong steamIDFriend, int iMessageID, IntPtr pvData, int cubData, int peChatEntryType)
        {
            return GetFriendMessage(steamIDFriend, iMessageID, pvData, cubData, IntPtr.Zero);
        }
        public string GetFriendPersonaName(ulong steamIDFriend)
        {
            string Result = "Unknown";
            MutexHelper.Wait("Users", delegate
            {
                if (steamIDFriend == SteamEmulator.SteamID)
                {
                    Result = SteamEmulator.PersonaName;
                }
                else if (steamIDFriend == 65535)
                {
                    Result = SteamEmulator.PersonaName;
                }
                else
                {
                    var friend = GetUser(steamIDFriend);
                    if (friend != null)
                    {
                        Result = string.IsNullOrWhiteSpace(friend.PersonaName)
                            ? BuildFallbackPersonaName(steamIDFriend)
                            : friend.PersonaName;
                    }
                }

                Write($"GetFriendPersonaName (SteamID = {new CSteamID(steamIDFriend)}) = {Result}");
            });
            return Result;
        }

        public string GetFriendPersonaNameHistory(ulong steamIDFriend, int iPersonaName)
        {
            Write($"GetFriendPersonaNameHistory {steamIDFriend}");
            return string.Empty;
        }

        public int GetFriendPersonaState(ulong steamIDFriend)
        {
            EPersonaState Result = EPersonaState.k_EPersonaStateOffline;
            MutexHelper.Wait("Users", delegate
            {
                if (steamIDFriend == SteamEmulator.SteamID)
                {
                    Result = EPersonaState.k_EPersonaStateOnline;
                    return;
                }

                var friend = GetUser(steamIDFriend);
                if (friend != null)
                {
                    // Reflect the real server-reported state; a disconnected friend
                    // must show as offline, not permanently online.
                    Result = (EPersonaState)friend.PersonaState;
                }
            });

            Write($"GetFriendPersonaState {steamIDFriend} = {Result}");
            return (int)Result;
        }

        public int GetFriendRelationship(ulong steamIDFriend)
        {
            EFriendRelationship Result = EFriendRelationship.k_EFriendRelationshipNone;
            MutexHelper.Wait("Users", delegate
            {
                var friend = GetUser(steamIDFriend);
                if (friend != null)
                {
                    Result = friend.FriendRelationship == 0
                        ? (friend.HasFriend ? EFriendRelationship.k_EFriendRelationshipFriend : EFriendRelationship.k_EFriendRelationshipNone)
                        : (EFriendRelationship)friend.FriendRelationship;
                }
            });
            Write($"GetFriendRelationship (SteamID = {steamIDFriend}) = {Result}");
            return (int)Result;
        }

        public string GetFriendRichPresence(ulong steamIDFriend, string pchKey)
        {
            string Result = "";
            var friend = GetUser(steamIDFriend);
            if (friend != null)
            {
                //IPCManager.GetRichPresence(steamIDFriend, pchKey);
                if (friend.RichPresence.ContainsKey(pchKey))
                {
                    Result = friend.RichPresence[pchKey];
                }
            }
            Write($"GetFriendRichPresence (SteamID = {steamIDFriend}, Key = {pchKey}) = {Result}");
            return Result;
        }

        public string GetFriendRichPresenceKeyByIndex(ulong steamIDFriend, int iKey)
        {
            string Result = "";
            var friend = GetUser(steamIDFriend);
            if (friend != null)
            {
                int current = 0;
                foreach (var item in friend.RichPresence)
                {
                    if (current == iKey)
                    {
                        Result = item.Key;
                        break;
                    }
                    current++;
                }
            }
            Write($"GetFriendRichPresenceKeyByIndex (SteamID ={steamIDFriend}, Key index = {iKey}) = {Result}");
            return Result;
        }

        public int GetFriendRichPresenceKeyCount(ulong steamIDFriend)
        {
            var Result = 0;
            var friend = GetUser(steamIDFriend);
            if (friend != null)
            {
                Result = friend.RichPresence.Count;
            }
            Write($"GetFriendRichPresenceKeyCount (SteamID ={steamIDFriend}) = {Result}");
            return Result;
        }

        public int GetFriendsGroupCount()
        {
            Write($"GetFriendsGroupCount");
            return 0;
        }

        public FriendsGroupID_t GetFriendsGroupIDByIndex(int iFG)
        {
            Write($"GetFriendsGroupIDByIndex {iFG}");
            return (int)0;
        }

        public int GetFriendsGroupMembersCount(FriendsGroupID_t friendsGroupID)
        {
            Write($"GetFriendsGroupMembersCount {friendsGroupID}");
            return 0;
        }

        public void GetFriendsGroupMembersList(FriendsGroupID_t friendsGroupID, IntPtr pOutSteamIDMembers, int nMembersCount)
        {
            Write($"GetFriendsGroupMembersList {friendsGroupID}");
            if (pOutSteamIDMembers == IntPtr.Zero || nMembersCount <= 0)
            {
                return;
            }

            for (int i = 0; i < nMembersCount; i++)
            {
                NativeSteamId.Write(IntPtr.Add(pOutSteamIDMembers, i * sizeof(ulong)), CSteamID.Invalid);
            }
        }

        public void GetFriendsGroupMembersList(FriendsGroupID_t friendsGroupID, ref ulong[] pOutSteamIDMembers, int nMembersCount)
        {
            Write($"GetFriendsGroupMembersList {friendsGroupID}");
        }

        public string GetFriendsGroupName(FriendsGroupID_t friendsGroupID)
        {
            Write($"GetFriendsGroupName {friendsGroupID}");
            return "";
        }

        public int GetFriendSteamLevel(ulong steamIDFriend)
        {
            Write($"GetFriendSteamLevel {steamIDFriend}");
            return 100;
        }

        public int GetSmallFriendAvatar(ulong steamIDFriend)
        {
            Write($"GetSmallFriendAvatar {(CSteamID)steamIDFriend}");

            if (steamIDFriend == 65535) steamIDFriend = (ulong)SteamEmulator.SteamID;

            if (Avatars.TryGetValue((ulong)steamIDFriend, out ImageAvatar avatar))
            {
                return avatar.Small;
            }
            else
            {
                RequestAvatar((ulong)steamIDFriend);
            }
            return EnsureDefaultAvatar().Small;
        }

        public int GetMediumFriendAvatar(ulong steamIDFriend)
        {
            Write($"GetMediumFriendAvatar {(CSteamID)steamIDFriend}");

            if (steamIDFriend == 65535) steamIDFriend = (ulong)SteamEmulator.SteamID;

            if (Avatars.TryGetValue(steamIDFriend, out ImageAvatar avatar))
            {
                return avatar.Medium;
            }
            else
            {
                RequestAvatar(steamIDFriend);
            }
            return EnsureDefaultAvatar().Medium;
        }

        public int GetLargeFriendAvatar(ulong steamIDFriend)
        {
            Write($"GetLargeFriendAvatar {(CSteamID)steamIDFriend}");

            if (steamIDFriend == 65535) steamIDFriend = (ulong)SteamEmulator.SteamID;

            if (Avatars.TryGetValue(steamIDFriend, out ImageAvatar avatar))
            {
                return avatar.Large;
            }
            else
            {
                RequestAvatar(steamIDFriend);
            }
            return EnsureDefaultAvatar().Large;
        }

        public int GetNumChatsWithUnreadPriorityMessages()
        {
            Write($"GetNumChatsWithUnreadPriorityMessages");
            return 0;
        }

        public int GetPersonaState()
        {
            Write($"GetPersonaState  = k_EPersonaStateOnline");
            return (int)EPersonaState.k_EPersonaStateOnline;
        }

        public string GetPlayerNickname(ulong steamIDPlayer)
        {
            Write($"GetPlayerNickname {steamIDPlayer}");
            return null;
        }

        public uint GetUserRestrictions()
        {
            Write($"GetUserRestrictions");
            return 0;
        }

        public bool HasFriend(ulong steamIDFriend, int iFriendFlags)
        {
            Write($"HasFriend {steamIDFriend}");
            var friend = GetUser(steamIDFriend);
            return friend != null && MatchesFriendFlags(friend, iFriendFlags);
        }

        public bool InviteUserToGame(ulong steamIDFriend, string pchConnectString)
        {
            Write($"InviteUserToGame {steamIDFriend} {pchConnectString}");
            return false;
        }

        public bool IsClanChatAdmin(ulong steamIDClanChat, ulong steamIDUser)
        {
            Write($"IsClanChatAdmin {steamIDClanChat}");
            return false;
        }

        public bool IsClanChatWindowOpenInSteam(ulong steamIDClanChat)
        {
            Write($"IsClanChatWindowOpenInSteam {steamIDClanChat}");
            return false;
        }

        public bool IsClanOfficialGameGroup(ulong steamIDClan)
        {
            Write($"IsClanOfficialGameGroup {steamIDClan}");
            return false;
        }

        public bool IsClanPublic(ulong steamIDClan)
        {
            Write($"IsClanpublic {steamIDClan}");
            return false;
        }

        public SteamAPICall_t IsFollowing(ulong steamID)
        {
            Write($"IsFollowing {steamID}");
            // FriendsIsFollowing_t
            return k_uAPICallInvalid;
        }

        public bool IsUserInSource(ulong steamIDUser, ulong steamIDSource)
        {
            Write($"IsUserInSource {steamIDUser}");
            return false;
        }

        public SteamAPICall_t JoinClanChatRoom(ulong steamIDClan)
        {
            Write($"JoinClanChatRoom {steamIDClan}");
            // JoinClanChatRoomCompletionResult_t
            return k_uAPICallInvalid;
        }

        public bool LeaveClanChatRoom(ulong steamIDClan)
        {
            Write($"LeaveClanChatRoom {steamIDClan}");
            return true;
        }

        public bool OpenClanChatWindowInSteam(ulong steamIDClanChat)
        {
            Write($"OpenClanChatWindowInSteam {steamIDClanChat}");
            return false;
        }

        public bool RegisterProtocolInOverlayBrowser(string pchProtocol)
        {
            Write($"RegisterProtocolInOverlayBrowser {pchProtocol}");
            return false;
        }

        public bool ReplyToFriendMessage(ulong steamIDFriend, string pchMsgToSend)
        {
            Write($"ReplyToFriendMessage {steamIDFriend} {pchMsgToSend}");
            return false;
        }

        public SteamAPICall_t RequestClanOfficerList(ulong steamIDClan)
        {
            Write($"RequestClanOfficerList {steamIDClan}");
            // ClanOfficerListResponse_t
            return k_uAPICallInvalid;
        }

        public void RequestFriendRichPresence(ulong steamIDFriend)
        {
            Write($"RequestFriendRichPresence {steamIDFriend}");
        }

        public bool RequestUserInformation(ulong steamIDUser, bool bRequireNameOnly)
        {
            bool queued = false;
            if (steamIDUser == (ulong)SteamEmulator.SteamID)
            {
                APIClient.QueueSelfRefresh();
            }
            else
            {
                queued = APIClient.QueueUserProfileRefresh(steamIDUser, false);
            }

            var User = GetUser(steamIDUser);
            var available = User != null && !string.IsNullOrWhiteSpace(User.PersonaName);
            var requesting = queued && !available;
            Write($"RequestUserInformation (SteamID = {steamIDUser}, RequireNameOnly = {bRequireNameOnly}) = queued:{queued} available:{available} requesting:{requesting}");
            return requesting;
        }

        public bool SendClanChatMessage(ulong steamIDClanChat, string pchText)
        {
            Write($"SendClanChatMessage {steamIDClanChat} {pchText}");
            return false;
        }

        public void SetInGameVoiceSpeaking(ulong steamIDUser, bool bSpeaking)
        {
            Write($"SetInGameVoiceSpeaking {steamIDUser}");
        }

        public bool SetListenForFriendsMessages(bool bInterceptEnabled)
        {
            Write($"SetListenForFriendsMessages {bInterceptEnabled}");
            return true;
        }

        public SteamAPICall_t SetPersonaName(string pchPersonaName)
        {
            Write($"SetPersonaName {pchPersonaName}");
            SteamAPICall_t APICall = k_uAPICallInvalid;

            SetPersonaNameResponse_t data = new SetPersonaNameResponse_t();
            data.m_bSuccess = true;
            data.m_bLocalSuccess = true;
            data.m_result = EResult.k_EResultOK;

            APICall = CallbackManager.AddCallbackResult(data);
            SteamEmulator.PersonaName = pchPersonaName;
            if (APIClient.IsEnabled)
            {
                WorkQueue.Enqueue("Update persona name", () =>
                {
                    if (!APIClient.UpdatePersonaName(pchPersonaName))
                    {
                        Write($"SetPersonaName backend update failed for {pchPersonaName}");
                    }
                }, "friends:persona-name", true);
            }
            else
            {
                SteamEmulator.PersonaName = pchPersonaName;
            }
            ReportUserChanged((ulong)SteamEmulator.SteamID, EPersonaChange.k_EPersonaChangeName);

            return APICall;
        }

        public void AddOrUpdateUser(uint accountID, string personaName, uint appID, string senderAddress = "")
        {
            SKYNET.Types.SteamUser steamUser = Users.Find((SKYNET.Types.SteamUser u) => u.AccountID == accountID);
            if (steamUser == null)
            {
                CSteamID cSteamID = new CSteamID(accountID);
                steamUser = new SKYNET.Types.SteamUser
                {
                    PersonaName = personaName,
                    AccountID = accountID,
                    SteamID = (ulong)cSteamID,
                    GameID = appID,
                    IPAddress = senderAddress,
                    HasFriend = true
                };
                Users.Add(steamUser);
                Write($"Added user {personaName} {cSteamID}, from {senderAddress}");
            }
            else
            {
                steamUser.PersonaName = personaName;
            }
        }

        public void UpdateUserStatus(NET_UserDataUpdated statusChanged, string ipaddress)
        {
            SKYNET.Types.SteamUser steamUser = Users.Find((SKYNET.Types.SteamUser f) => f.AccountID == statusChanged.AccountID);
            if (steamUser != null)
            {
                steamUser.LobbyID = statusChanged.LobbyID;
                if (steamUser.PersonaName != statusChanged.PersonaName)
                {
                    steamUser.PersonaName = statusChanged.PersonaName;
                    ReportUserChanged(steamUser.SteamID, EPersonaChange.k_EPersonaChangeName);
                }
                if (steamUser.LobbyID != statusChanged.LobbyID)
                {
                    steamUser.LobbyID = statusChanged.LobbyID;
                }
            }
            else
            {
                steamUser = new SKYNET.Types.SteamUser
                {
                    AccountID = statusChanged.AccountID,
                    SteamID = (ulong)new CSteamID(statusChanged.AccountID),
                    HasFriend = true,
                    PersonaName = statusChanged.PersonaName,
                    IPAddress = ipaddress
                };
                Users.Add(steamUser);
            }
        }

        public void SetPlayedWith(ulong steamIDUserPlayedWith)
        {
            Write($"SetPlayedWith {steamIDUserPlayedWith}");
        }

        public bool SetRichPresence(string pchKey, string pchValue)
        {
            Write($"SetRichPresence (Key = {pchKey}, Value = {pchValue})");
            if (APIClient.IsEnabled)
            {
                WorkQueue.Enqueue("SetRichPresence", () => APIClient.SetRichPresence(pchKey, pchValue),
                    "presence:" + (pchKey ?? string.Empty));
            }
            return true;
        }

        /*
        public void UpdateUserStatus(IPC_UserDataUpdated statusChanged)
        {
            switch (statusChanged.Type)
            {
                case IPC_UserDataUpdated.UpdateType.PersonaName:
                    ReportUserChanged(new CSteamID(statusChanged.AccountID).SteamID, EPersonaChange.k_EPersonaChangeName);
                    break;
                case IPC_UserDataUpdated.UpdateType.LobbyID:
                    break;
                default:
                    break;
            }
        }
        */

        private List<SteamPlayer> GetFriends()
        {
            return GetFriends((int)EFriendFlags.k_EFriendFlagImmediate);
        }

        private List<SteamPlayer> GetFriends(int friendFlags)
        {
            if (APIClient.IsEnabled)
            {
                APIClient.QueueFriendsRefresh();
                return StateCache.GetFriends().FindAll(friend => MatchesFriendFlags(friend, friendFlags));
            }

            return UserManager.GetFriends().FindAll(friend => MatchesFriendFlags(friend, friendFlags));
        }

        private static bool MatchesFriendFlags(SteamPlayer friend, int friendFlags)
        {
            if (friend == null || friendFlags == (int)EFriendFlags.k_EFriendFlagNone)
            {
                return false;
            }

            var relationship = friend.FriendRelationship == 0
                ? (friend.HasFriend ? EFriendRelationship.k_EFriendRelationshipFriend : EFriendRelationship.k_EFriendRelationshipNone)
                : (EFriendRelationship)friend.FriendRelationship;

            if ((friendFlags & (int)EFriendFlags.k_EFriendFlagAll) == (int)EFriendFlags.k_EFriendFlagAll)
            {
                return relationship != EFriendRelationship.k_EFriendRelationshipNone;
            }

            if ((friendFlags & (int)EFriendFlags.k_EFriendFlagImmediate) != 0 &&
                relationship == EFriendRelationship.k_EFriendRelationshipFriend)
            {
                return true;
            }

            if ((friendFlags & (int)EFriendFlags.k_EFriendFlagFriendshipRequested) != 0 &&
                relationship == EFriendRelationship.k_EFriendRelationshipRequestInitiator)
            {
                return true;
            }

            if ((friendFlags & (int)EFriendFlags.k_EFriendFlagRequestingFriendship) != 0 &&
                relationship == EFriendRelationship.k_EFriendRelationshipRequestRecipient)
            {
                return true;
            }

            return false;
        }

        public SteamPlayer GetUser(ulong steamID)
        {
            if (steamID == (ulong)SteamEmulator.SteamID)
            {
                if (APIClient.IsEnabled)
                {
                    APIClient.QueueSelfRefresh();
                    if (StateCache.TryGetSelf(out var self))
                    {
                        return self;
                    }
                }

                return new SteamPlayer
                {
                    AccountID = SteamEmulator.SteamID.GetAccountID(),
                    SteamID = (ulong)SteamEmulator.SteamID,
                    PersonaName = SteamEmulator.PersonaName,
                    HasFriend = true
                };
            }

            if (APIClient.IsEnabled)
            {
                if (StateCache.TryGetFriend(steamID, out var friend))
                {
                    return friend;
                }

                APIClient.QueueUserProfileRefresh(steamID);
                return new SteamPlayer
                {
                    AccountID = new CSteamID(steamID).AccountID,
                    SteamID = steamID,
                    PersonaName = BuildFallbackPersonaName(steamID),
                    HasFriend = false,
                    FriendRelationship = (int)EFriendRelationship.k_EFriendRelationshipNone
                };
            }

            return UserManager.GetUser(steamID);
        }

        private static string BuildFallbackPersonaName(ulong steamID)
        {
            var accountID = new CSteamID(steamID).AccountID;
            return accountID != 0 ? $"Player {accountID}" : $"Player {steamID}";
        }

        public SKYNET.Types.SteamUser GetUser2(ulong steamID)
        {
            return Users.Find((SKYNET.Types.SteamUser u) => u.SteamID == steamID);
        }

        public byte[] GetAvatar(ulong steamID)
        {
            if (Avatars.TryGetValue(steamID, out var avatar))
            {
                return avatar.GetImage();
            }
            return EnsureDefaultAvatar().GetImage();
        }

        public bool TryGetAvatarBitmap(ulong steamID, out Bitmap bitmap)
        {
            bitmap = null;

            if (steamID == 65535)
            {
                steamID = (ulong)SteamEmulator.SteamID;
            }

            if (steamID == 0 || steamID == ulong.MaxValue)
            {
                return false;
            }

            if (Avatars.TryGetValue(steamID, out var avatar))
            {
                var bytes = avatar.GetImage();
                if (bytes != null && bytes.Length > 0)
                {
                    try
                    {
                        using (var ms = new MemoryStream(bytes))
                        using (var loaded = Image.FromStream(ms))
                        {
                            bitmap = new Bitmap(loaded);
                        }
                        return true;
                    }
                    catch
                    {
                        bitmap = null;
                    }
                }
            }

            if (TryLoadCachedAvatar(steamID, out var cachedAvatar))
            {
                try
                {
                    using (var cacheCopy = new Bitmap(cachedAvatar))
                    {
                        AddOrUpdateAvatar(cacheCopy, steamID);
                    }
                    bitmap = cachedAvatar;
                    return true;
                }
                catch
                {
                    cachedAvatar.Dispose();
                    bitmap = null;
                }
            }

            RequestAvatar(steamID);
            return false;
        }

        public (int, int) GetImageSize(int index)
        {
            if (AvatarsByHandle.TryGetValue(index, out var avatar))
            {
                if (avatar.Small == index)  return (32, 32);
                if (avatar.Medium == index) return (64, 64);
                if (avatar.Large == index)  return (184, 184);
            }

            return (0, 0);
        }

        public ImageAvatar GetImageAvatar(int index)
        {
            AvatarsByHandle.TryGetValue(index, out var avatar);
            return avatar;
        }

        private ImageAvatar EnsureDefaultAvatar()
        {
            if (DefaultAvatar == null)
            {
                lock (AvatarSync)
                {
                    if (DefaultAvatar == null)
                    {
                        DefaultAvatar = new ImageAvatar(null, AllocateImageHandle);
                        RegisterAvatarHandles(DefaultAvatar);
                    }
                }
            }
            return DefaultAvatar;
        }

        private int AllocateImageHandle()
        {
            return Interlocked.Increment(ref ImageIndex);
        }

        private void RegisterAvatarHandles(ImageAvatar avatar)
        {
            AvatarsByHandle[avatar.Small] = avatar;
            AvatarsByHandle[avatar.Medium] = avatar;
            AvatarsByHandle[avatar.Large] = avatar;
        }

        private void RequestAvatar(ulong steamIDFriend)
        {
            try
            {
                if (steamIDFriend == 0)
                {
                    return;
                }

                lock (QueryingAvatar)
                {
                    if (QueryingAvatar.Contains(steamIDFriend))
                    {
                        return;
                    }

                    QueryingAvatar.Add(steamIDFriend);
                }

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        if (TryLoadCachedAvatar(steamIDFriend, out var cachedAvatar))
                        {
                            AddOrUpdateAvatar(cachedAvatar, steamIDFriend);
                            cachedAvatar.Dispose();
                        }

                        // Disk is only a fast first frame. Revalidate every
                        // process lifetime so changed/default avatars cannot
                        // remain pinned forever.
                        APIClient.RefreshAvatar(steamIDFriend);
                    }
                    catch (Exception ex)
                    {
                        Write($"RequestAvatar {steamIDFriend} {ex.Message}");
                    }
                    finally
                    {
                        lock (QueryingAvatar)
                        {
                            QueryingAvatar.Remove(steamIDFriend);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Write($"RequestAvatar {steamIDFriend} {ex.Message}");
                lock (QueryingAvatar)
                {
                    QueryingAvatar.Remove(steamIDFriend);
                }
            }
        }

        public void SyncSelfAvatarWithServer()
        {
            var steamId = (ulong)SteamEmulator.SteamID;
            if (steamId != 0 && steamId != ulong.MaxValue)
            {
                RequestAvatar(steamId);
            }
        }

        public void OnIdentityChanged(ulong previousSteamId, ulong currentSteamId)
        {
            if (previousSteamId == currentSteamId)
            {
                return;
            }

            Avatars.Clear();
            AvatarsByHandle.Clear();
            RegisterAvatarHandles(EnsureDefaultAvatar());
            lock (QueryingAvatar)
            {
                QueryingAvatar.Clear();
            }
        }

        private bool TryLoadCachedAvatar(ulong steamID, out Bitmap avatar)
        {
            avatar = null;
            try
            {
                var candidate = GetAvatarCachePath(steamID);
                if (File.Exists(candidate))
                {
                    using (var loaded = (Bitmap)Image.FromFile(candidate))
                    {
                        avatar = new Bitmap(loaded);
                    }
                    return true;
                }
            }
            catch
            {
                avatar = null;
            }

            return false;
        }

        public void StoreCachedAvatar(Bitmap avatar, ulong steamID)
        {
            if (avatar == null || steamID == 0)
            {
                return;
            }

            var destination = GetAvatarCachePath(steamID);
            var temporary = destination + "." + Guid.NewGuid().ToString("N") + ".tmp";
            lock (AvatarCacheSync)
            {
                try
                {
                    Common.EnsureDirectoryExists(Path.GetDirectoryName(destination));
                    avatar.Save(temporary, ImageFormat.Png);
                    if (File.Exists(destination))
                    {
                        try
                        {
                            File.Replace(temporary, destination, null);
                        }
                        catch
                        {
                            File.Delete(destination);
                            File.Move(temporary, destination);
                        }
                    }
                    else
                    {
                        File.Move(temporary, destination);
                    }
                }
                finally
                {
                    if (File.Exists(temporary))
                    {
                        File.Delete(temporary);
                    }
                }
            }
        }

        public void RemoveCachedAvatar(ulong steamID)
        {
            if (steamID == 0)
            {
                return;
            }

            lock (AvatarCacheSync)
            {
                var path = GetAvatarCachePath(steamID);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private static string GetAvatarCachePath(ulong steamID)
        {
            var server = (SteamEmulator.ServerUrl ?? string.Empty).Trim().TrimEnd('/').ToLowerInvariant();
            byte[] hash;
            using (var sha = SHA256.Create())
            {
                hash = sha.ComputeHash(Encoding.UTF8.GetBytes(server));
            }

            var namespaceKey = BitConverter.ToString(hash, 0, 8).Replace("-", string.Empty).ToLowerInvariant();
            return Path.Combine(
                Common.GetPath(),
                "Data",
                "Images",
                "AvatarCache",
                "server_" + namespaceKey,
                steamID.ToString() + ".png");
        }

        public void AddOrUpdateAvatar(Bitmap image, ulong steamID)
        {
            ImageAvatar imageAvatar;
            if (steamID == 0)
            {
                imageAvatar = EnsureDefaultAvatar();
                imageAvatar.UpdateImage(image);
            }
            else if (Avatars.TryGetValue(steamID, out imageAvatar))
            {
                imageAvatar.UpdateImage(image);
            }
            else
            {
                var created = new ImageAvatar(image, AllocateImageHandle);
                if (Avatars.TryAdd(steamID, created))
                {
                    imageAvatar = created;
                    RegisterAvatarHandles(imageAvatar);
                }
                else
                {
                    imageAvatar = Avatars[steamID];
                    imageAvatar.UpdateImage(image);
                }
            }

            lock (QueryingAvatar)
            {
                QueryingAvatar.Remove(steamID);
            }

            if (steamID != 0)
            {
                CallbackManager.AddCallback(new AvatarImageLoaded_t
                {
                    SteamID = steamID,
                    Image = imageAvatar.Large,
                    Wide = 184,
                    Tall = 184
                });
                ReportUserChanged(steamID, EPersonaChange.k_EPersonaChangeAvatar);
            }
        }

        public class ImageAvatar
        {
            private readonly object SyncRoot = new object();

            public int Small;
            public int Medium;
            public int Large;

            public byte[] SmallBytes;
            public byte[] MediumBytes;
            public byte[] LargeBytes;

            public uint Width;
            public uint Height;
            public byte[] Image;

            public ImageAvatar(Bitmap image, Func<int> allocateHandle)
            {
                Small = allocateHandle();
                Medium = allocateHandle();
                Large = allocateHandle();

                SetImage(image);
            }

            private void SetImage(Bitmap image)
            {
                try
                {
                    if (image == null)
                    {
                        SetBlankImage();
                        return;
                    }

                    byte[] small;
                    byte[] medium;
                    byte[] large;
                    byte[] encoded;
                    using (var resized32 = ImageHelper.Resize(image, 32, 32))
                    using (var resized64 = ImageHelper.Resize(image, 64, 64))
                    using (var resized184 = ImageHelper.Resize(image, 184, 184))
                    using (var resized = ImageHelper.Resize(image, 200, 200))
                    {
                        small = ImageHelper.ConvertToRGBA(resized32);
                        medium = ImageHelper.ConvertToRGBA(resized64);
                        large = ImageHelper.ConvertToRGBA(resized184);
                        encoded = ImageHelper.ImageToBytes(resized);
                    }

                    lock (SyncRoot)
                    {
                        SmallBytes = small;
                        MediumBytes = medium;
                        LargeBytes = large;
                        Image = encoded;
                    }
                }
                catch
                {
                    SetBlankImage();
                }
            }

            public byte[] GetImage()
            {
                lock (SyncRoot)
                {
                    return Image ?? new byte[0];
                }
            }


            public byte[] GetImage(int iImage)
            {
                lock (SyncRoot)
                {
                    if (iImage == Small)
                    {
                        if (SmallBytes == null || SmallBytes.Length == 0) SmallBytes = BlankRGBA(32, 32);
                        return SmallBytes;
                    }
                    if (iImage == Medium)
                    {
                        if (MediumBytes == null || MediumBytes.Length == 0) MediumBytes = BlankRGBA(64, 64);
                        return MediumBytes;
                    }
                    if (iImage == Large)
                    {
                        if (LargeBytes == null || LargeBytes.Length == 0) LargeBytes = BlankRGBA(184, 184);
                        return LargeBytes;
                    }
                }

                return BlankRGBA(32, 32);
            }

            public void UpdateImage(Bitmap image)
            {
                SetImage(image);
            }

            private void SetBlankImage()
            {
                lock (SyncRoot)
                {
                    SmallBytes = BlankRGBA(32, 32);
                    MediumBytes = BlankRGBA(64, 64);
                    LargeBytes = BlankRGBA(184, 184);
                    Image = new byte[0];
                }
            }

            private static byte[] BlankRGBA(int width, int height)
            {
                var bytes = new byte[width * height * 4];
                for (int i = 0; i < bytes.Length; i += 4)
                {
                    bytes[i] = 32;
                    bytes[i + 1] = 32;
                    bytes[i + 2] = 32;
                    bytes[i + 3] = 255;
                }
                return bytes;
            }
        }
    }
}

