using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Managers;
using SKYNET.Types;
using SKYNET.IPC.Types;
using SKYNET.Steamworks.Interfaces;

using SteamAPICall_t = System.UInt64;
using FriendsGroupID_t = System.UInt16;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamFriends : ISteamInterface
    {
        public static SteamFriends Instance;

        public  List<ulong> QueryingAvatar;

        private Dictionary<string, string> RichPresence;
        private ConcurrentDictionary<ulong, ImageAvatar> Avatars;
        private int ImageIndex;
        private ImageAvatar DefaultAvatar;

        public SteamFriends()
        {
            Instance = this;
            InterfaceName = "SteamFriends";
            InterfaceVersion = "SteamFriends017";
            QueryingAvatar = new List<SteamAPICall_t>();
            RichPresence = new Dictionary<string, string>();
            Avatars = new ConcurrentDictionary<ulong, ImageAvatar>();
            ImageIndex = 10; 
        }

        public void Initialize()
        {
            #region Default Avatar

            DefaultAvatar = new ImageAvatar(new Bitmap(210, 210), ref ImageIndex);

            #endregion

            #region Own Avatar

            try
            {
                var Avatar = ImageHelper.GetDesktopWallpaper(true);
                ImageAvatar avatar = new ImageAvatar(Avatar, ref ImageIndex);
                Avatars.TryAdd((ulong)SteamEmulator.SteamID, avatar);
            }
            catch (Exception ex)
            {
                Write($"Error loading default avatar {ex}");
            }

            #endregion
        }

        public void ReportUserChanged(ulong SteamID, EPersonaChange changeFlags)
        {
            PersonaStateChange_t data = new PersonaStateChange_t();
            data.m_ulSteamID = SteamID;
            data.m_nChangeFlags = (int)changeFlags;
            CallbackManager.AddCallbackResult(data);
        }

        public string GetPersonaName()
        {
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
        }

        public void ActivateGameOverlayInviteDialog(ulong steamIDLobby)
        {
            try
            {
                Write($"ActivateGameOverlayInviteDialog (Lobby SteamID = {steamIDLobby})");
                OverlayType type = OverlayType.LobbyInvite;
                // TODO: Show Overlay
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        public void ActivateGameOverlayInviteDialogConnectString(string pchConnectString)
        {
            Write($"ActivateGameOverlayInviteDialogConnectString (URI = {pchConnectString})");
        }

        public void ActivateGameOverlayRemotePlayTogetherInviteDialog(ulong steamIDLobby)
        {
            Write($"ActivateGameOverlayRemotePlayTogetherInviteDialog (Lobby SteamID = {steamIDLobby})");
        }

        public void ActivateGameOverlayToStore(uint nAppID, int eFlag)
        {
            Write($"ActivateGameOverlayToStore (AppID = {nAppID}, Flag = {eFlag})");
        }

        public void ActivateGameOverlayToUser(string friendsGroupID, ulong steamID)
        {
            Write($"ActivateGameOverlayToUser (GroupID = {friendsGroupID}, SteamID = {(CSteamID)steamID})");
            OverlayType type = default;
            switch (friendsGroupID)
            {
                case "steamid":
                    type = OverlayType.SteamProfile;
                    break;
                case "chat":
                    type = OverlayType.Chat;
                    break;
                case "jointrade":
                    type = OverlayType.JoinTrade;
                    break;
                case "stats":
                    type = OverlayType.Stats;
                    break;
                case "achievements":
                    type = OverlayType.Achievements;
                    break;
                case "friendadd":
                    type = OverlayType.FriendAdd;
                    break;
                case "friendremove":
                    type = OverlayType.FriendRemove;
                    break;
                case "friendrequestaccept":
                    type = OverlayType.FriendRequestAccept;
                    break;
                case "friendrequestignore":
                    type = OverlayType.FriendRequestIgnore;
                    break;
                default:
                    break;
            }
            // TODO: Show Overlay
        }

        public void ActivateGameOverlayToWebPage(string pchURL, int eMode)
        {
            Write($"ActivateGameOverlayToWebPage {pchURL}");
        }

        public void ClearRichPresence()
        {
            Write($"ClearRichPresence");
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

        public int GetClanChatMessage(ulong steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, int peChatEntryType, ref ulong[] psteamidChatter)
        {
            //psteamidChatter = 0;
            Write($"GetClanChatMessage {steamIDClanChat}");
            return 0;
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
            var Friends = GetFriends();

            if (iFriend < 0 | iFriend > Friends.Count)
            {
                iFriend = iFriendFlags;
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
            if ((iFriendFlags & (int)EFriendFlags.k_EFriendFlagImmediate) == (int)EFriendFlags.k_EFriendFlagImmediate)
            {
                MutexHelper.Wait("Users", delegate
                {
                    var Friends = GetFriends();
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

        public bool GetFriendGamePlayed(ulong steamIDFriend, IntPtr ptrFriendGameInfo)
        {
            bool Result = false;
            FriendGameInfo_t pFriendGameInfo = Marshal.PtrToStructure<FriendGameInfo_t>(ptrFriendGameInfo);
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
                if (friend == null)
                {
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

            Marshal.StructureToPtr(pFriendGameInfo, ptrFriendGameInfo, false);
            Write($"GetFriendGamePlayed (SteamID = {steamIDFriend}) = {Result}");
            return Result;
        }

        public int GetFriendMessage(ulong steamIDFriend, int iMessageID, IntPtr pvData, int cubData, int peChatEntryType)
        {
            Write($"GetFriendMessage {steamIDFriend} {(EChatEntryType)peChatEntryType}");
            peChatEntryType = (int)EChatEntryType.ChatMsg;
            return 0;
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
                    if (friend != null) Result = friend.PersonaName;
                }

                Write($"GetFriendPersonaName (SteamID = {new CSteamID(steamIDFriend)}) = {Result}");
            });
            return Result;
        }

        public string GetFriendPersonaNameHistory(ulong steamIDFriend, int iPersonaName)
        {
            Write($"GetFriendPersonaNameHistory {steamIDFriend}");
            return "SKYNET";
        }

        public int GetFriendPersonaState(ulong steamIDFriend)
        {
            Write($"GetFriendPersonaState {steamIDFriend}");
            EPersonaState Result = EPersonaState.k_EPersonaStateOnline;
            MutexHelper.Wait("Users", delegate
            {
                if (steamIDFriend == SteamEmulator.SteamID)
                {
                    Result = EPersonaState.k_EPersonaStateOnline;
                }
                else if (GetUser(steamIDFriend) != null)
                {
                    Result = EPersonaState.k_EPersonaStateOnline;
                }
            });

            return (int)Result;
        }

        public int GetFriendRelationship(ulong steamIDFriend)
        {
            Write($"GetFriendRelationship {steamIDFriend}");
            EFriendRelationship Result = EFriendRelationship.k_EFriendRelationshipNone;

            MutexHelper.Wait("Users", delegate
            {
                var friend = GetUser(steamIDFriend);
                if (friend != null && friend.HasFriend)
                    Result = EFriendRelationship.k_EFriendRelationshipFriend;
            });

            return (int)Result;
        }

        public string GetFriendRichPresence(ulong steamIDFriend, string pchKey)
        {
            Write($"GetFriendRichPresence [{steamIDFriend}]: {pchKey}");
            if (RichPresence.ContainsKey(pchKey))
            {
                return RichPresence[pchKey];
            }
            return "";
        }

        public string GetFriendRichPresenceKeyByIndex(ulong steamIDFriend, int iKey)
        {
            Write($"GetFriendRichPresenceKeyByIndex {steamIDFriend} {iKey}");
            return "";
        }

        public int GetFriendRichPresenceKeyCount(ulong steamIDFriend)
        {
            Write($"GetFriendRichPresenceKeyCount {steamIDFriend}");
            return 0;
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
            Marshal.StructureToPtr(SteamEmulator.SteamID, pOutSteamIDMembers, false);
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

            if (Avatars.TryGetValue(steamIDFriend, out ImageAvatar avatar))
            {
                return avatar.Small;
            }
            else
            {
                RequestAvatar(steamIDFriend);
            }
            return DefaultAvatar.Small;
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
            return DefaultAvatar.Medium;
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
            return DefaultAvatar.Large;
        }

        public int GetNumChatsWithUnreadPriorityMessages()
        {
            Write($"GetNumChatsWithUnreadPriorityMessages");
            return 0;
        }

        public int GetPersonaState()
        {
            Write($"GetPersonaState");
            return (int)EPersonaState.k_EPersonaStateOnline;
        }

        public string GetPlayerNickname(ulong steamIDPlayer)
        {
            Write($"GetPlayerNickname {steamIDPlayer}");
            var Result = "";
            MutexHelper.Wait("Users", delegate
            {
                if (steamIDPlayer == SteamEmulator.SteamID)
                {
                    Result = SteamEmulator.PersonaName;
                }
                var friend = GetUser(steamIDPlayer);
                if (friend == null)
                {
                    Result = "";
                }

            });
            return Result;
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
            return friend != null && friend.HasFriend;
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
            Write($"RequestUserInformation {(CSteamID)steamIDUser}");
            return false;
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
            ReportUserChanged((ulong)SteamEmulator.SteamID, EPersonaChange.k_EPersonaChangeName);

            SteamEmulator.PersonaName = pchPersonaName;
            MutexHelper.Wait("Users", delegate
            {

            });
            var user = GetUser((ulong)SteamEmulator.SteamID);
            if (user != null)
            {
                user.PersonaName = pchPersonaName;
                IPCManager.SendUserDataUpdated(user);
            }

            return APICall;
        }

        public void SetPlayedWith(ulong steamIDUserPlayedWith)
        {
            Write($"SetPlayedWith {steamIDUserPlayedWith}");
        }

        public bool SetRichPresence(string pchKey, string pchValue)
        {
            Write($"SetRichPresence (Key = {pchKey}, Value = {pchValue})");

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

        public void UpdateUserLobby(ulong userSteamId, ulong lobbySteamId, bool BroadCast = false)
        {
            var user = GetUser(userSteamId);
            if (user != null)
            {
                user.LobbyID = lobbySteamId;
                if (BroadCast)
                {
                    IPCManager.SendUserDataUpdated(user);
                }
            }
        }

        public void UpdateUserStatus(IPC_UserDataUpdated statusChanged)
        {
            var user = GetUser((ulong)new CSteamID(statusChanged.AccountID));
            if (user != null)
            {
                if (!string.IsNullOrEmpty(statusChanged.IPAddress))
                {
                    user.IPAddress = statusChanged.IPAddress;
                }
                if (!string.IsNullOrEmpty(statusChanged.PersonaName))
                {
                    if (user.PersonaName != statusChanged.PersonaName)
                    {
                        user.PersonaName = statusChanged.PersonaName;
                        ReportUserChanged(user.SteamID, EPersonaChange.k_EPersonaChangeName);

                        if (statusChanged.AccountID == SteamEmulator.SteamID.AccountID)
                        {
                            SteamEmulator.PersonaName = statusChanged.PersonaName;
                        }
                    }
                }
                if (user.LobbyID != statusChanged.LobbyID)
                {
                    user.LobbyID = statusChanged.LobbyID;
                    // TODO: Update in SteamMatchmaking
                }
            }
        }

        private List<SteamPlayer> GetFriends()
        {
            var Friends = IPCManager.GetFriends();
            return Friends;
        }

        private SteamPlayer GetUser(ulong steamID)
        {
            var User = IPCManager.GetUser(steamID);
            return User;
        }

        public byte[] GetAvatar(ulong steamID)
        {
            if (Avatars.TryGetValue(steamID, out var avatar))
            {
                return avatar.GetImage();
            }
            return new byte[0];
        }

        public (int, int) GetImageSize(int index)
        {
            if (DefaultAvatar.Small == index) return (32, 32);
            if (DefaultAvatar.Medium == index) return (64, 64);
            if (DefaultAvatar.Large == index) return (184, 184);

            foreach (var KV in SteamFriends.Instance.Avatars)
            {
                var avatar = KV.Value;
                if (avatar.Small == index)  return (32, 32);
                if (avatar.Medium == index) return (64, 64);
                if (avatar.Large == index)  return (184, 184);
            }

            return (0, 0);
        }

        public ImageAvatar GetImageAvatar(int index)
        {
            if (DefaultAvatar.Small == index) return DefaultAvatar;
            if (DefaultAvatar.Medium == index) return DefaultAvatar;
            if (DefaultAvatar.Large == index) return DefaultAvatar;

            foreach (var KV in Avatars)
            {
                var avatar = KV.Value;
                if (avatar.Small == index || avatar.Medium == index || avatar.Large == index)
                {
                    return avatar;
                }
            }
            return null;
        }

        private void RequestAvatar(ulong steamIDFriend)
        {
            try
            {

                if (QueryingAvatar.Contains(steamIDFriend)) return;

                var User = GetUser(steamIDFriend);
                if (User != null)
                {
                    QueryingAvatar.Add(steamIDFriend);
                    IPCManager.RequestAvatar(User.SteamID);
                }
            }
            catch
            {
                if (QueryingAvatar.Contains(steamIDFriend))
                    QueryingAvatar.Remove(steamIDFriend);
            }
        }

        public void AddOrUpdateAvatar(Bitmap image, ulong steamID)
        {
            if (steamID == 0)
            {
                DefaultAvatar.UpdateImage(image);
            }
            else if (Avatars.TryGetValue(steamID, out ImageAvatar avatar))
            {
                avatar.UpdateImage(image);
            }
            else
            {
                avatar = new ImageAvatar(image, ref ImageIndex);
                Avatars.TryAdd(steamID, avatar);
            }
            if (QueryingAvatar.Contains(steamID))
                QueryingAvatar.Remove(steamID);

            ReportUserChanged(steamID, EPersonaChange.k_EPersonaChangeAvatar);
        }

        public class ImageAvatar
        {
            public int Small;
            public int Medium;
            public int Large;

            public byte[] SmallBytes;
            public byte[] MediumBytes;
            public byte[] LargeBytes;

            public uint Width;
            public uint Height;
            public byte[] Image;

            public ImageAvatar(Bitmap image, ref int imageIndex)
            {
                try
                {
                    imageIndex++;
                    Small = imageIndex;

                    imageIndex++;
                    Medium = imageIndex;

                    imageIndex++;
                    Large = imageIndex;

                    var resized32 = ImageHelper.Resize(image, 32, 32); 
                    SmallBytes = ImageHelper.ConvertToRGBA(resized32); 

                    var resized64 = ImageHelper.Resize(image, 64, 64); 
                    MediumBytes = ImageHelper.ConvertToRGBA(resized64); 

                    var resized184 = ImageHelper.Resize(image, 184, 184); 
                    LargeBytes = ImageHelper.ConvertToRGBA(resized184); 

                    var resized = ImageHelper.Resize(image, 200, 200); 
                    Image = ImageHelper.ImageToBytes(resized);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            public byte[] GetImage()
            {
                return Image;
            }


            public byte[] GetImage(int iImage)
            {
                Bitmap image = (Bitmap)ImageHelper.ImageFromBytes(Image);
                if (iImage == Small)
                {
                    if (SmallBytes.Length == 0)
                    {
                        var resized = ImageHelper.Resize(image, 32, 32);
                        SmallBytes = ImageHelper.ConvertToRGBA(resized);
                    }
                    return SmallBytes;
                } 
                if (iImage == Medium)
                {
                    if (MediumBytes.Length == 0)
                    {
                        var resized = ImageHelper.Resize(image, 64, 64);
                        MediumBytes = ImageHelper.ConvertToRGBA(resized);
                    }
                    return MediumBytes;
                }
                if (iImage == Large)
                {
                    if (LargeBytes.Length == 0)
                    {
                        var resized = ImageHelper.Resize(image, 184, 184);
                        LargeBytes = ImageHelper.ConvertToRGBA(resized);
                    }
                    return LargeBytes;
                }

                var resizedIMG = ImageHelper.Resize(image, 32, 32);
                var Bytes = ImageHelper.ConvertToRGBA(resizedIMG);
                return Bytes;
            }

            public void UpdateImage(Bitmap image)
            {
                var resized32 = ImageHelper.Resize(image, 32, 32);
                SmallBytes = ImageHelper.ConvertToRGBA(resized32);

                var resized64 = ImageHelper.Resize(image, 64, 64);
                MediumBytes = ImageHelper.ConvertToRGBA(resized64);

                var resized184 = ImageHelper.Resize(image, 184, 184);
                LargeBytes = ImageHelper.ConvertToRGBA(resized184);

                var resized = ImageHelper.Resize(image, 200, 200);
                Image = ImageHelper.ImageToBytes(resized);
            }
        }
    }
}

