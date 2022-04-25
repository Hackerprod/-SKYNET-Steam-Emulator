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

using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamFriends : ISteamInterface
    {
        private List<SteamFriend> Friends;
        private List<ulong> Users;
        private Dictionary<string, string> RichPresence;
        private ConcurrentDictionary<ulong, ImageAvatar> Avatars;
        private int ImageIndex;
        private ImageAvatar DefaultAvatar;
        private SteamAPICall_t k_uAPICallInvalid = 0x0;

        public SteamFriends()
        {
            InterfaceVersion = "SteamFriends";
            Friends = new List<SteamFriend>();
            Users = new List<ulong>();
            RichPresence = new Dictionary<string, string>();
            Avatars = new ConcurrentDictionary<ulong, ImageAvatar>();
            ImageIndex = 10;

            #region Default Avatar

            DefaultAvatar = new ImageAvatar(Resources.Image, 1);

            #endregion

            #region Own Avatar

            string fileName = Path.Combine(modCommon.GetPath(), "SKYNET", "Avatar.jpg");
            Bitmap Avatar = default;
            if (File.Exists(fileName))
            {
                Avatar = (Bitmap)Bitmap.FromFile(fileName);
            }
            else
            {
                Avatar = ImageHelper.GetDesktopWallpaper();
            }

            ImageAvatar avatar = new ImageAvatar(Avatar, ImageIndex);
            Avatars.TryAdd((ulong)SteamEmulator.SteamId, avatar);
            ImageIndex = avatar.Large + 1;

            #endregion
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

        public void ActivateGameOverlayToStore(uint nAppID, uint eFlag)
        {
            Write($"ActivateGameOverlayToStore {nAppID} {eFlag}");
        }


        public void ActivateGameOverlayToUser(string friendsGroupID, ulong steamID)
        {
            Write($"ActivateGameOverlayToUser {friendsGroupID} {(CSteamID)steamID}");

            switch (friendsGroupID)
            {
                case "friendadd":
                    // TODO
                    break;
                default:
                    break;
            }
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


        public SteamAPICall_t DownloadClanActivityCounts(UInt64[] clans, int cClansToRequest)
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
            Write($"GetFriendByIndex {iFriend}");
            CSteamID Result = (CSteamID)0;

            MutexHelper.Wait("GetFriendByIndex", delegate
            {
                if (Friends.Count > iFriend)
                {
                    SteamFriend friend = Friends[iFriend];
                    if (friend != null)
                    {
                        Result = (CSteamID)friend.SteamId;
                    }
                }
            });

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
            Write($"GetFriendCount {(EFriendFlags)iFriendFlags}");
            int Result = 0;
            MutexHelper.Wait("GetFriendCount", delegate
            {
                Result = Friends.Count;
            });
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
            Write($"GetFriendGamePlayed {steamIDFriend}");

            FriendGameInfo_t pFriendGameInfo = Marshal.PtrToStructure<FriendGameInfo_t>(ptrFriendGameInfo);

            if (steamIDFriend == (ulong)SteamEmulator.SteamId)
            {
                pFriendGameInfo.GameID = (uint)SteamEmulator.GameID;
                pFriendGameInfo.GameIP = 0;
                pFriendGameInfo.GamePort = 0;
            }
            else
            {
                SteamFriend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
                if (friend == null)
                {
                    pFriendGameInfo.GameID = 0;
                    pFriendGameInfo.GameIP = 0;
                    pFriendGameInfo.GamePort = 0;
                    return false;
                }
                pFriendGameInfo.GameID = friend.GameId;
                pFriendGameInfo.GameIP = 0;
                pFriendGameInfo.GamePort = 0;
            }

            Marshal.StructureToPtr(pFriendGameInfo, ptrFriendGameInfo, false);
            return true;
        }


        public int GetFriendMessage(ulong steamIDFriend, int iMessageID, IntPtr pvData, int cubData, uint peChatEntryType)
        {
            Write($"GetFriendMessage {steamIDFriend} {(EChatEntryType)peChatEntryType}");
            peChatEntryType = 1;
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
                    SteamFriend friend = Friends.Find(f => f.SteamId == steamIDFriend);
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


        public EPersonaState GetFriendPersonaState(ulong steamIDFriend)
        {
            Write($"GetFriendPersonaState {steamIDFriend}");
            EPersonaState Result = EPersonaState.k_EPersonaStateOnline;
            MutexHelper.Wait("GetFriendPersonaState", delegate
            {
                if (steamIDFriend == (ulong)SteamEmulator.SteamId)
                {
                    Result = EPersonaState.k_EPersonaStateOnline;
                }
                else if (Users.Find(f => f == steamIDFriend) != 0)
                {
                    Result = EPersonaState.k_EPersonaStateOnline;
                }
            });

            return Result;
        }


        public EFriendRelationship GetFriendRelationship(ulong steamIDFriend)
        {
            Write($"GetFriendRelationship {steamIDFriend}");
            EFriendRelationship Result = EFriendRelationship.k_EFriendRelationshipNone;

            MutexHelper.Wait("GetFriendRelationship", delegate
            {
                SteamFriend friend = Friends.Find(f => f.SteamId == steamIDFriend);
                if (friend != null)
                    Result = EFriendRelationship.k_EFriendRelationshipFriend;
            });

            return Result;
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

        public void GetFriendsGroupMembersList(short friendsGroupID, IntPtr pOutSteamIDMembers, int nMembersCount)
        {
            Write($"GetFriendsGroupMembersList {friendsGroupID}");
            Marshal.StructureToPtr(SteamEmulator.SteamId, pOutSteamIDMembers, false);
        }

        public string GetFriendsGroupName(int friendsGroupID)
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
            return DefaultAvatar.Small;
        }

        public int GetMediumFriendAvatar(ulong steamIDFriend)
        {
            Write($"GetMediumFriendAvatar {(CSteamID)steamIDFriend}");
            if (Avatars.TryGetValue(steamIDFriend, out ImageAvatar avatar))
            {
                return avatar.Medium;
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
            return DefaultAvatar.Large;
        }

        public int GetNumChatsWithUnreadPriorityMessages()
        {
            Write($"GetNumChatsWithUnreadPriorityMessages");
            return 0;
        }

        public uint GetPersonaState()
        {
            Write($"GetPersonaState");
            return (uint)EPersonaState.k_EPersonaStateOnline;
        }

        public string GetPlayerNickname(ulong steamIDPlayer)
        {
            Write($"GetPlayerNickname {steamIDPlayer}");
            if (steamIDPlayer == (ulong)SteamEmulator.SteamId)
            {
                return SteamEmulator.PersonaName;
            }
            SteamFriend friend = Friends.Find(f => f.AccountId == (uint)steamIDPlayer);
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
            SteamFriend friend = Friends.Find(f => f.AccountId == (uint)steamIDFriend);
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
            MutexHelper.Wait("SetPersonaName", delegate
            {
                SetPersonaNameResponse_t data = new SetPersonaNameResponse_t();
                data.Success = true;
                data.LocalSuccess = true;
                data.Result = EResult.k_EResultOK;

                SteamEmulator.PersonaName = pchPersonaName;

                APICall = CallbackManager.AddCallbackResult(data);
            });
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
            public Bitmap Image;

            public ImageAvatar(Bitmap data, int imageIndex)
            {
                Small = imageIndex;
                imageIndex++;

                Medium = imageIndex;
                imageIndex++;

                Large = imageIndex;
                imageIndex++;

                Image = data;

                var resized32 = ImageHelper.Resize(Image, 32, 32);
                SmallBytes = ImageHelper.ConvertToRGBA(resized32);

                var resized64 = ImageHelper.Resize(Image, 64, 64);
                MediumBytes = ImageHelper.ConvertToRGBA(resized64);

                var resized184 = ImageHelper.Resize(Image, 184, 184);
                LargeBytes = ImageHelper.ConvertToRGBA(resized184);
            }

            public byte[] GetImage(int iImage)
            {
                int size = 0;
                if (iImage == Small)
                {
                    if (SmallBytes.Length == 0)
                    {
                        var resized = ImageHelper.Resize(Image, 32, 32);
                        SmallBytes = ImageHelper.ConvertToRGBA(resized);
                    }
                    return SmallBytes;
                } 
                if (iImage == Medium)
                {
                    if (MediumBytes.Length == 0)
                    {
                        var resized = ImageHelper.Resize(Image, 64, 64);
                        MediumBytes = ImageHelper.ConvertToRGBA(resized);
                    }
                    return MediumBytes;
                }
                if (iImage == Large)
                {
                    if (LargeBytes.Length == 0)
                    {
                        var resized = ImageHelper.Resize(Image, 184, 184);
                        LargeBytes = ImageHelper.ConvertToRGBA(resized);
                    }
                    return LargeBytes;
                }

                var resizedIMG = ImageHelper.Resize(Image, 32, 32);
                var Bytes = ImageHelper.ConvertToRGBA(resizedIMG);
                return Bytes;
            }
        }
    }
}

