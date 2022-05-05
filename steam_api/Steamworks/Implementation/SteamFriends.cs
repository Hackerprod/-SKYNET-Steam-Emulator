using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Managers;
using SKYNET.Properties;
using SKYNET.Types;
using Steamworks;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using SteamAPICall_t = System.UInt64;
using FriendsGroupID_t = System.UInt16;
using SKYNET.Overlay;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamFriends : ISteamInterface
    {
        public List<SKYNET.Types.SteamUser> Users;
        public List<ulong> QueryingAvatar;
        private Dictionary<string, string> RichPresence;
        private ConcurrentDictionary<ulong, ImageAvatar> Avatars;

        private int ImageIndex;
        private ImageAvatar DefaultAvatar;
        private SteamAPICall_t k_uAPICallInvalid = 0x0;

        public SteamFriends()
        {
            InterfaceName = "SteamFriends";
            Users   = new List<SKYNET.Types.SteamUser>();
            QueryingAvatar = new List<SteamAPICall_t>();
            RichPresence = new Dictionary<string, string>();
            Avatars = new ConcurrentDictionary<ulong, ImageAvatar>();
            ImageIndex = 10;

            #region Default Avatar

            DefaultAvatar = new ImageAvatar(Resources.Image, ref ImageIndex);
            
            #endregion

            #region Own Avatar

            try
            {
                string fileName = Path.Combine(modCommon.GetPath(), "SKYNET", "Avatar.jpg");
                if (!File.Exists(fileName))
                    fileName = Path.Combine(modCommon.GetPath(), "SKYNET", "Avatar.png");

                Bitmap Avatar = default;
                if (File.Exists(fileName))
                {
                    Avatar = (Bitmap)Bitmap.FromFile(fileName);
                }
                else
                {
                    Avatar = ImageHelper.GetDesktopWallpaper(true);
                }
                if (Avatar != null)
                {
                    ImageAvatar avatar = new ImageAvatar(Avatar, ref ImageIndex); 
                    Avatars.TryAdd((ulong)SteamEmulator.SteamId, avatar);
                }
            }
            catch (Exception ex)
            {
                Write($"Error loading default avatar {ex}");
            }

            #endregion

            #region Own User

            Users.Add(new Types.SteamUser()
            {
                //AccountId = SteamEmulator.SteamId.AccountId,
                GameId = SteamEmulator.AppId,
                HasFriend = false,
                PersonaName = SteamEmulator.PersonaName,
                SteamId = (ulong)SteamEmulator.SteamId,
                IPAddress = NetworkManager.GetIPAddress().ToString()
            });

            #endregion

            Users.Add(new Types.SteamUser()
            {
                AccountId = 1001,
                GameId = 570,
                HasFriend = true,
                PersonaName = "Yohel.com",
                SteamId = (ulong)new CSteamID(1001),
                IPAddress = "10.31.0.1"
            });
            Users.Add(new Types.SteamUser()
            {
                AccountId = 1002,
                GameId = 570,
                HasFriend = true,
                PersonaName = "Elier",
                SteamId = (ulong)new CSteamID(1002),
                IPAddress = "10.31.0.2"
            });
            Users.Add(new Types.SteamUser()
            {
                AccountId = 1003,
                GameId = 570,
                HasFriend = true,
                PersonaName = "Yusniel",
                SteamId = (ulong)new CSteamID(1003),
                IPAddress = "10.31.0.3"
            });
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

        public void ActivateGameOverlay(string friendsGroupID)
        {
            Write($"ActivateGameOverlay {friendsGroupID}");
        }

        public void ActivateGameOverlayInviteDialog(ulong steamIDLobby)
        {
            Write($"ActivateGameOverlayInviteDialog {steamIDLobby}");
        }

        public void ActivateGameOverlayInviteDialogConnectString(string pchConnectString)
        {
            Write($"ActivateGameOverlayInviteDialogConnectString {pchConnectString}");
        }

        public void ActivateGameOverlayRemotePlayTogetherInviteDialog(ulong steamIDLobby)
        {
            Write($"ActivateGameOverlayRemotePlayTogetherInviteDialog {steamIDLobby}");
        }

        public void ActivateGameOverlayToStore(uint nAppID, int eFlag)
        {
            Write($"ActivateGameOverlayToStore {nAppID} {eFlag}");
        }

        public void ActivateGameOverlayToUser(string friendsGroupID, ulong steamID)
        {
            Write($"ActivateGameOverlayToUser {friendsGroupID} {(CSteamID)steamID}");

            switch (friendsGroupID)
            {
                case "steamid":
                    // Opens the overlay web browser to the specified user or groups profile.
                    break;
                case "chat":
                    // Opens a chat window to the specified user, or joins the group chat.
                    break;
                case "jointrade":
                    // Opens a window to a Steam Trading session that was started with the ISteamEconomy / StartTrade Web API.
                    break;
                case "stats":
                    // Opens the overlay web browser to the specified user's stats.
                    break;
                case "achievements":
                    // Opens the overlay web browser to the specified user's achievements.
                    break;
                case "friendadd":
                    // Opens the overlay in minimal mode prompting the user to add the target user as a friend.
                    break;
                case "friendremove":
                    // Opens the overlay in minimal mode prompting the user to remove the target friend.
                    break;
                case "friendrequestaccept":
                    // Opens the overlay in minimal mode prompting the user to accept an incoming friend invite.
                    break;
                case "friendrequestignore":
                    // Opens the overlay in minimal mode prompting the user to ignore an incoming friend invite.
                    break;
                default:
                    break;
            }

            var overlay = new frmOverlay();
            overlay.ShowDialog();

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
            return (CSteamID)0;
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
            return (CSteamID)0;
        }

        public int GetClanChatMemberCount(ulong steamIDClan)
        {
            Write($"GetClanChatMemberCount {steamIDClan}");
            return 0;
        }

        public int GetClanChatMessage(ulong steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, int peChatEntryType, ref ulong psteamidChatter)
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

        public string GetClanName(ulong steamIDClan)
        {
            Write($"GetClanName {steamIDClan}");
            return "";
        }

        public CSteamID GetClanOfficerByIndex(ulong steamIDClan, int iOfficer)
        {
            Write($"GetClanOfficerByIndex {steamIDClan}");
            return (CSteamID)0;
        }

        public int GetClanOfficerCount(ulong steamIDClan)
        {
            Write($"GetClanOfficerCount {steamIDClan}");
            return 0;
        }

        public CSteamID GetClanOwner(ulong steamIDClan)
        {
            Write($"GetClanOwner {steamIDClan}");
            return (CSteamID)0;
        }

        public string GetClanTag(ulong steamIDClan)
        {
            Write($"GetClanTag {steamIDClan}");
            return "";
        }

        public CSteamID GetCoplayFriend(int iCoplayFriend)
        {
            Write($"GetCoplayFriend {iCoplayFriend}");
            return (CSteamID)0;
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
            string msg = $"GetFriendByIndex, Index: {iFriend} | ";
            CSteamID Result = new CSteamID(0);
            int index = iFriendFlags; // BUG Take flags as index

            MutexHelper.Wait("GetFriendByIndex", delegate
            {
                var Friends = Users.FindAll(f => f.HasFriend);
                if (Friends.Count > index)
                {
                    var friend = Friends[index];
                    if (friend != null)
                    {
                        Result = (CSteamID)friend.SteamId;
                    }
                }
            });
            Write(msg + Result);
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
            SteamEmulator.Debug($"GetFriendCount Flags {iFriendFlags} - {(int)EFriendFlags.k_EFriendFlagImmediate} | {iFriendFlags & (int)EFriendFlags.k_EFriendFlagImmediate}");
            int Result = 0;
            if ((iFriendFlags & (int)EFriendFlags.k_EFriendFlagImmediate) == (int)EFriendFlags.k_EFriendFlagImmediate)
            {
                MutexHelper.Wait("GetFriendCount", delegate
                {
                    var Friends = Users.FindAll(f => f.HasFriend);
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
            return (CSteamID)0;
        }

        public bool GetFriendGamePlayed(ulong steamIDFriend, IntPtr ptrFriendGameInfo)
        {
            Write($"GetFriendGamePlayed {(CSteamID)steamIDFriend}");

            bool Result = false;
            FriendGameInfo_t pFriendGameInfo = Marshal.PtrToStructure<FriendGameInfo_t>(ptrFriendGameInfo);

            if (steamIDFriend == (ulong)SteamEmulator.SteamId)
            {
                pFriendGameInfo.GameID = (uint)SteamEmulator.GameID;
                pFriendGameInfo.GameIP = 0;
                pFriendGameInfo.GamePort = 0;
            }
            else
            {
                var friend = Users.Find(f => f.AccountId == steamIDFriend.GetAccountID());
                if (friend == null)
                {
                    pFriendGameInfo.GameID = 0;
                    pFriendGameInfo.GameIP = 0;
                    pFriendGameInfo.GamePort = 0;
                    Result = true;
                }
                else
                {
                    pFriendGameInfo.GameID = friend.GameId;
                    pFriendGameInfo.GameIP = 0;
                    pFriendGameInfo.GamePort = 0;
                }
            }

            Marshal.StructureToPtr(pFriendGameInfo, ptrFriendGameInfo, false);
            return Result;
        }

        public int GetFriendMessage(ulong steamIDFriend, int iMessageID, IntPtr pvData, int cubData, ref int peChatEntryType)
        {
            Write($"GetFriendMessage {steamIDFriend} {(EChatEntryType)peChatEntryType}");
            peChatEntryType = (int)EChatEntryType.ChatMsg;
            return 0;
        }

        public string GetFriendPersonaName(ulong steamIDFriend)
        {
            string Result = "Unknown";
            MutexHelper.Wait("FileReadAsyncComplete", delegate
            {
                if ((ulong)steamIDFriend == (ulong)SteamEmulator.SteamId)
                {
                    Result = SteamEmulator.PersonaName;
                }
                else
                {
                    var friend = Users.Find(f => f.SteamId == steamIDFriend);
                    if (friend != null) Result = friend.PersonaName;
                }

                Write($"GetFriendPersonaName {new CSteamID(steamIDFriend)} | {Result}");
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
            MutexHelper.Wait("GetFriendPersonaState", delegate
            {
                if (steamIDFriend == (ulong)SteamEmulator.SteamId)
                {
                    Result = EPersonaState.k_EPersonaStateOnline;
                }
                else if (Users.Find(f => f.SteamId == steamIDFriend) != null)
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

            MutexHelper.Wait("GetFriendRelationship", delegate
            {
                var friend = Users.Find(f => f.SteamId == steamIDFriend);
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
            Marshal.StructureToPtr(SteamEmulator.SteamId, pOutSteamIDMembers, false);
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
            if (Avatars.TryGetValue(steamIDFriend, out ImageAvatar avatar))
            {
                return avatar.Small;
            }
            else
            {
                avatar = LoadFromCache(steamIDFriend);
                if (avatar != null)
                {
                    return avatar.Small;
                }
                ThreadPool.QueueUserWorkItem(RequestAvatar, steamIDFriend);
            }
            return DefaultAvatar.Small;
        }

        public int GetMediumFriendAvatar(ulong steamIDFriend)
        {
            Write($"GetMediumFriendAvatar {(CSteamID)steamIDFriend}");
            if (Avatars.TryGetValue(steamIDFriend, out ImageAvatar avatar))
            {
                return avatar.Medium;
            }
            else
            {
                avatar = LoadFromCache(steamIDFriend);
                if (avatar != null)
                {
                    return avatar.Medium;
                }
                ThreadPool.QueueUserWorkItem(RequestAvatar, steamIDFriend);
            }
            return DefaultAvatar.Medium;
        }

        public int GetLargeFriendAvatar(ulong steamIDFriend)
        {
            Write($"GetLargeFriendAvatar {(CSteamID)steamIDFriend}");
            if (Avatars.TryGetValue(steamIDFriend, out ImageAvatar avatar))
            {
                return avatar.Large;
            }
            else
            {
                avatar = LoadFromCache(steamIDFriend);
                if (avatar != null)
                {
                    return avatar.Large;
                }
                ThreadPool.QueueUserWorkItem(RequestAvatar, steamIDFriend);
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
            if (steamIDPlayer == (ulong)SteamEmulator.SteamId)
            {
                return SteamEmulator.PersonaName;
            }
            var friend = Users.Find(f => f.AccountId == (uint)steamIDPlayer);
            if (friend == null)
            {
                return "";
            }
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
            var friend = Users.Find(f => f.AccountId == (uint)steamIDFriend && f.HasFriend);
            return friend != null;
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
            data.Success = true;
            data.LocalSuccess = true;
            data.Result = EResult.k_EResultOK;

            SteamEmulator.PersonaName = pchPersonaName;

            APICall = CallbackManager.AddCallbackResult(data);
            ReportUserChanged((ulong)SteamEmulator.SteamId, EPersonaChange.k_EPersonaChangeName);

            return APICall;
        }

        public void SetPlayedWith(ulong steamIDUserPlayedWith)
        {
            Write($"SetPlayedWith {steamIDUserPlayedWith}");
        }

        public bool SetRichPresence(string pchKey, string pchValue)
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

            foreach (var KV in SteamEmulator.SteamFriends.Avatars)
            {
                var avatar = KV.Value;
                if (avatar.Small == index)  return (32, 32);
                if (avatar.Medium == index) return (64, 64);
                if (avatar.Large == index)  return (184, 184);
            }

            return (0, 0);
        }

        public void AddOrUpdateUser(uint accountID, string personaName, uint appID, string senderAddress = "")
        {
            var user = Users.Find(u => u.AccountId == accountID);
            if (user == null)
            {
                CSteamID steamID = new CSteamID(accountID);
                user = new SKYNET.Types.SteamUser()
                {
                    PersonaName = personaName,
                    AccountId = accountID,
                    SteamId = (ulong)steamID,
                    GameId = appID,
                    IPAddress = senderAddress,
                    HasFriend = true
                };
                Users.Add(user);
                Write($"Added user {personaName} {steamID}, from {senderAddress}");
            }
            else
            {
                user.PersonaName = personaName;
            }
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

        private void RequestAvatar(object threadObj)
        {
            ulong steamIDFriend = (ulong)threadObj;

            try
            {

                if (QueryingAvatar.Contains(steamIDFriend)) return;

                var User = Users.Find(u => u.SteamId == steamIDFriend);
                if (User != null)
                {
                    QueryingAvatar.Add(steamIDFriend);
                    NetworkManager.RequestAvatar(User.IPAddress);
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
            if (Avatars.TryGetValue(steamID, out ImageAvatar avatar))
            {
                avatar.UpdateImage(image);
            }
            else
            {
                avatar = new ImageAvatar(image, ref ImageIndex);
                Avatars.TryAdd(steamID, avatar);
                ReportUserChanged(steamID, EPersonaChange.k_EPersonaChangeAvatar);
            }
            if (QueryingAvatar.Contains(steamID))
                QueryingAvatar.Remove(steamID);
        }

        private ImageAvatar LoadFromCache(ulong steamIDFriend)
        {
            string fullPath = Path.Combine(SteamEmulator.SteamRemoteStorage.AvatarCachePath, steamIDFriend.GetAccountID() + ".jpg");
            if (File.Exists(fullPath))
            {
                try
                {
                    var image = (Bitmap)Bitmap.FromFile(fullPath);
                    if (image != null)
                    {
                        ImageAvatar avatar = new ImageAvatar(image, ref ImageIndex);
                        Avatars.TryAdd(steamIDFriend, avatar);
                        return avatar;
                    }
                }
                catch (Exception)
                {
                }
            }
            return null;
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

