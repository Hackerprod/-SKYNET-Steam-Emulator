using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Steamworks;

namespace SKYNET.Interface
{
    public interface ISteamFriends
    {
        // Creates a communication pipe to the Steam client.
        // returns the local players name - guaranteed to not be NULL.
        // this is the same name as on the users community profile page
        // this is stored in UTF-8 format
        // like all the other interface functions that return a char , it's important that this pointer is not saved
        // off; it will eventually be free'd or re-allocated
        string GetPersonaName(IntPtr _);

        // Sets the player name, stores it on the server and publishes the changes to all friends who are online.
        // Changes take place locally immediately, and a PersonaStateChange_t is posted, presuming success.
        //
        // The final results are available through the return value SteamAPICall_t, using SetPersonaNameResponse_t.
        //
        // If the name change fails to happen on the server, then an additional global PersonaStateChange_t will be posted
        // to change the name back, in addition to the SetPersonaNameResponse_t callback.

        SteamAPICall_t SetPersonaName(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchPersonaName);

        // gets the status of the current user
        EPersonaState GetPersonaState(IntPtr _);

        // friend iteration
        // takes a set of k_EFriendFlags, and returns the number of users the client knows about who meet that criteria
        // then GetFriendByIndex() can then be used to return the id's of each of those users
        int GetFriendCount(IntPtr _, int iFriendFlags);

        // returns the steamID of a user
        // iFriend is a index of range [0, GetFriendCount())
        // iFriendsFlags must be the same value as used in GetFriendCount()
        // the returned IntPtr can then be used by all the functions below to access details about the user
        IntPtr GetFriendByIndex(IntPtr _, int iFriend, int iFriendFlags);

        // returns a relationship to a user
        EFriendRelationship GetFriendRelationship(IntPtr _, IntPtr steamIDFriend);

        // returns the current status of the specified user
        // this will only be known by the local user if steamIDFriend is in their friends list; on the same game server; in a chat room or lobby; or in a small group with the local user
        EPersonaState GetFriendPersonaState(IntPtr _, IntPtr steamIDFriend);

        // returns the name another user - guaranteed to not be NULL.
        // same rules as GetFriendPersonaState() apply as to whether or not the user knowns the name of the other user
        // note that on first joining a lobby, chat room or game server the local user will not known the name of the other users automatically; that information will arrive asyncronously
        // 
        string GetFriendPersonaName(IntPtr _, IntPtr steamIDFriend);

        // returns true if the friend is actually in a game, and fills in pFriendGameInfo with an extra details 
        bool GetFriendGamePlayed(IntPtr _, IntPtr steamIDFriend, out FriendGameInfo_t pFriendGameInfo );
        // accesses old friends names - returns an empty string when their are no more items in the history
        string GetFriendPersonaNameHistory(IntPtr _, IntPtr steamIDFriend, int iPersonaName);
        // friends steam level
        int GetFriendSteamLevel(IntPtr _, IntPtr steamIDFriend);

        // Returns nickname the current user has set for the specified player. Returns NULL if the no nickname has been set for that player.
        // DEPRECATED: GetPersonaName follows the Steam nickname preferences, so apps shouldn't need to care about nicknames explicitly.
        string GetPlayerNickname(IntPtr _, IntPtr steamIDPlayer);

        // friend grouping (tag) apis
        // returns the number of friends groups
        int GetFriendsGroupCount(IntPtr _);
        // returns the friends group ID for the given index (invalid indices return k_FriendsGroupID_Invalid)
        FriendsGroupID_t GetFriendsGroupIDByIndex(IntPtr _, int iFG);
        // returns the name for the given friends group (NULL in the case of invalid friends group IDs)
        string GetFriendsGroupName(IntPtr _, FriendsGroupID_t friendsGroupID);
        // returns the number of members in a given friends group
        int GetFriendsGroupMembersCount(IntPtr _, FriendsGroupID_t friendsGroupID);
        // gets up to nMembersCount members of the given friends group, if fewer exist than requested those positions' SteamIDs will be invalid
        void GetFriendsGroupMembersList(IntPtr _, FriendsGroupID_t friendsGroupID, IntPtr[] pOutSteamIDMembers, int nMembersCount);

        // returns true if the specified user meets any of the criteria specified in iFriendFlags
        // iFriendFlags can be the union (binary or, |) of one or more k_EFriendFlags values
        bool HasFriend(IntPtr _, IntPtr steamIDFriend, int iFriendFlags);

        // clan (group) iteration and access functions
        int GetClanCount(IntPtr _);
        IntPtr GetClanByIndex(IntPtr _, int iClan);
        string GetClanName(IntPtr _, IntPtr steamIDClan);
        string GetClanTag(IntPtr _, IntPtr steamIDClan);
        // returns the most recent information we have about what's happening in a clan
        bool GetClanActivityCounts(IntPtr _, IntPtr steamIDClan, int pnOnline, int pnInGame, int pnChatting);

        SteamAPICall_t DownloadClanActivityCounts(IntPtr _, IntPtr[] psteamIDClans, int cClansToRequest);

        // iterators for getting users in a chat room, lobby, game server or clan
        // note that large clans that cannot be iterated by the local user
        // note that the current user must be in a lobby to retrieve IntPtrs of other users in that lobby
        // steamIDSource can be the steamID of a group, game server, lobby or chat room
        int GetFriendCountFromSource(IntPtr _, IntPtr steamIDSource);
        IntPtr GetFriendFromSourceByIndex(IntPtr _, IntPtr steamIDSource, int iFriend);

        // returns true if the local user can see that steamIDUser is a member or in steamIDSource
        bool IsUserInSource(IntPtr _, IntPtr steamIDUser, IntPtr steamIDSource);

        // User is in a game pressing the talk button (will suppress the microphone for all voice comms from the Steam friends UI)
        void SetInGameVoiceSpeaking(IntPtr _, IntPtr steamIDUser, bool bSpeaking);

        // activates the game overlay, with an optional dialog to open 
        // valid options include "Friends", "Community", "Players", "Settings", "OfficialGameGroup", "Stats", "Achievements",
        // "chatroomgroup/nnnn"
        void ActivateGameOverlay(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchDialog);

        // activates game overlay to a specific place
        // valid options are
        //		"steamid" - opens the overlay web browser to the specified user or groups profile
        //		"chat" - opens a chat window to the specified user, or joins the group chat 
        //		"jointrade" - opens a window to a Steam Trading session that was started with the ISteamEconomy/StartTrade Web API
        //		"stats" - opens the overlay web browser to the specified user's stats
        //		"achievements" - opens the overlay web browser to the specified user's achievements
        //		"friendadd" - opens the overlay in minimal mode prompting the user to add the target user as a friend
        //		"friendremove" - opens the overlay in minimal mode prompting the user to remove the target friend
        //		"friendrequestaccept" - opens the overlay in minimal mode prompting the user to accept an incoming friend invite
        //		"friendrequestignore" - opens the overlay in minimal mode prompting the user to ignore an incoming friend invite
        void ActivateGameOverlayToUser(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchDialog, IntPtr steamID);

        // activates game overlay web browser directly to the specified URL
        // full address with protocol type is required, e.g. http://www.steamgames.com/
        void ActivateGameOverlayToWebPage(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchURL, EActivateGameOverlayToWebPageMode eMode = EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);

        // activates game overlay to store page for app
        void ActivateGameOverlayToStore(IntPtr _, AppId_t nAppID, EOverlayToStoreFlag eFlag);

        // Mark a target user as 'played with'. This is a client-side only feature that requires that the calling user is 
        // in game 
        void SetPlayedWith(IntPtr _, IntPtr steamIDUserPlayedWith);

        // activates game overlay to open the invite dialog. Invitations will be sent for the provided lobby.
        void ActivateGameOverlayInviteDialog(IntPtr _, IntPtr steamIDLobby);

        // gets the small (32x32) avatar of the current user, which is a handle to be used in IClientUtils::GetImageRGBA(), or 0 if none set
        int GetSmallFriendAvatar(IntPtr _, IntPtr steamIDFriend);

        // gets the medium (64x64) avatar of the current user, which is a handle to be used in IClientUtils::GetImageRGBA(), or 0 if none set
        int GetMediumFriendAvatar(IntPtr _, IntPtr steamIDFriend);

        // gets the large (184x184) avatar of the current user, which is a handle to be used in IClientUtils::GetImageRGBA(), or 0 if none set
        // returns -1 if this image has yet to be loaded, in this case wait for a AvatarImageLoaded_t callback and then call this again
        int GetLargeFriendAvatar(IntPtr _, IntPtr steamIDFriend);

        // requests information about a user - persona name & avatar
        // if bRequireNameOnly is set, then the avatar of a user isn't downloaded 
        // - it's a lot slower to download avatars and churns the local cache, so if you don't need avatars, don't request them
        // if returns true, it means that data is being requested, and a PersonaStateChanged_t callback will be posted when it's retrieved
        // if returns false, it means that we already have all the details about that user, and functions can be called immediately
        bool RequestUserInformation(IntPtr _, IntPtr steamIDUser, bool bRequireNameOnly);

        SteamAPICall_t RequestClanOfficerList(IntPtr _, IntPtr steamIDClan);

        // iteration of clan officers - can only be done when a RequestClanOfficerList() call has completed

        // returns the steamID of the clan owner
        IntPtr GetClanOwner(IntPtr _, IntPtr steamIDClan);
        // returns the number of officers in a clan (including the owner)
        int GetClanOfficerCount(IntPtr _, IntPtr steamIDClan);
        // returns the steamID of a clan officer, by index, of range [0,GetClanOfficerCount)
        IntPtr GetClanOfficerByIndex(IntPtr _, IntPtr steamIDClan, int iOfficer);
        // if current user is chat restricted, he can't send or receive any text/voice chat messages.
        // the user can't see custom avatars. But the user can be online and send/recv game invites.
        // a chat restricted user can't add friends or join any groups.
        UInt32 GetUserRestrictions(IntPtr _);

        // Rich Presence data is automatically shared between friends who are in the same game
        // Each user has a set of Key/Value pairs
        // Note the following limits: k_cchMaxRichPresenceKeys, k_cchMaxRichPresenceKeyLength, k_cchMaxRichPresenceValueLength
        // There are five magic keys:
        //		"status"  - a UTF-8 string that will show up in the 'view game info' dialog in the Steam friends list
        //		"connect" - a UTF-8 string that contains the command-line for how a friend can connect to a game
        //		"steam_display"				- Names a rich presence localization token that will be displayed in the viewing user's selected language
        //									  in the Steam client UI. For more info: https://partner.steamgames.com/doc/api/ISteamFriends#richpresencelocalization
        //		"steam_player_group"		- When set, indicates to the Steam client that the player is a member of a particular group. Players in the same group
        //									  may be organized together in various places in the Steam UI.
        //		"steam_player_group_size"	- When set, indicates the total number of players in the steam_player_group. The Steam client may use this number to
        //									  display additional information about a group when all of the members are not part of a user's friends list.
        // GetFriendRichPresence() returns an empty string "" if no value is set
        // SetRichPresence() to a NULL or an empty string deletes the key
        // You can iterate the current set of keys for a friend with GetFriendRichPresenceKeyCount()
        // and GetFriendRichPresenceKeyByIndex() (typically only used for debugging)
        bool SetRichPresence(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchKey, [MarshalAs(UnmanagedType.LPStr)] string pchValue);
        void ClearRichPresence(IntPtr _);
        string GetFriendRichPresence(IntPtr _, IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchKey);
        int GetFriendRichPresenceKeyCount(IntPtr _, IntPtr steamIDFriend);
        string GetFriendRichPresenceKeyByIndex(IntPtr _, IntPtr steamIDFriend, int iKey);
        // Requests rich presence for a specific user.
        void RequestFriendRichPresence(IntPtr _, IntPtr steamIDFriend);

        // Rich invite support.
        // If the target accepts the invite, a GameRichPresenceJoinRequested_t callback is posted containing the connect string.
        // (Or you can configure your game so that it is passed on the command line instead.  This is a deprecated path; ask us if you really need this.)
        bool InviteUserToGame(IntPtr _, IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchConnectString);

        // recently-played-with friends iteration
        // this iterates the entire list of users recently played with, across games
        // GetFriendCoplayTime() returns as a unix time
        int GetCoplayFriendCount(IntPtr _);
        IntPtr GetCoplayFriend(IntPtr _, int iCoplayFriend);
        int GetFriendCoplayTime(IntPtr _, IntPtr steamIDFriend);
        AppId_t GetFriendCoplayGame(IntPtr _, IntPtr steamIDFriend);

        SteamAPICall_t JoinClanChatRoom(IntPtr _, IntPtr steamIDClan);
        bool LeaveClanChatRoom(IntPtr _, IntPtr steamIDClan);
        int GetClanChatMemberCount(IntPtr _, IntPtr steamIDClan);
        IntPtr GetChatMemberByIndex(IntPtr _, IntPtr steamIDClan, int iUser);
        bool SendClanChatMessage(IntPtr _, IntPtr steamIDClanChat, [MarshalAs(UnmanagedType.LPStr)] string pchText);
        int GetClanChatMessage(IntPtr _, IntPtr steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, EChatEntryType peChatEntryType, IntPtr[] psteamidChatter );
        bool IsClanChatAdmin(IntPtr _, IntPtr steamIDClanChat, IntPtr steamIDUser);

        // interact with the Steam (game overlay / desktop)
        bool IsClanChatWindowOpenInSteam(IntPtr _, IntPtr steamIDClanChat);
        bool OpenClanChatWindowInSteam(IntPtr _, IntPtr steamIDClanChat);
        bool CloseClanChatWindowInSteam(IntPtr _, IntPtr steamIDClanChat);

        // peer-to-peer chat interception
        // this is so you can show P2P chats inline in the game
        bool SetListenForFriendsMessages(IntPtr _, bool bInterceptEnabled);
        bool ReplyToFriendMessage(IntPtr _, IntPtr steamIDFriend, [MarshalAs(UnmanagedType.LPStr)] string pchMsgToSend);
        int GetFriendMessage(IntPtr _, IntPtr steamIDFriend, int iMessageID, IntPtr pvData, int cubData, EChatEntryType peChatEntryType);

        SteamAPICall_t GetFollowerCount(IntPtr _, IntPtr steamID);
        SteamAPICall_t IsFollowing(IntPtr _, IntPtr steamID);

        SteamAPICall_t EnumerateFollowingList(IntPtr _, UInt32 unStartIndex);

        bool IsClanPublic(IntPtr _, IntPtr steamIDClan);
        bool IsClanOfficialGameGroup(IntPtr _, IntPtr steamIDClan);

        /// Return the number of chats (friends or chat rooms) with unread messages.
        /// A "priority" message is one that would generate some sort of toast or
        /// notification, and depends on user settings.
        ///
        /// You can register for UnreadChatMessagesChanged_t callbacks to know when this
        /// has potentially changed.
        int GetNumChatsWithUnreadPriorityMessages(IntPtr _);

        // activates game overlay to open the remote play together invite dialog. Invitations will be sent for remote play together
        void ActivateGameOverlayRemotePlayTogetherInviteDialog(IntPtr _, IntPtr steamIDLobby);

        // Call this before calling ActivateGameOverlayToWebPage() to have the Steam Overlay Browser block navigations
        // to your specified protocol (scheme) uris and instead dispatch a OverlayBrowserProtocolNavigation_t callback to your game.
        // ActivateGameOverlayToWebPage() must have been called with k_EActivateGameOverlayToWebPageMode_Modal
        bool RegisterProtocolInOverlayBrowser(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchProtocol);

        // Activates the game overlay to open an invite dialog that will send the provided Rich Presence connect string to selected friends
        void ActivateGameOverlayInviteDialogConnectString(IntPtr _, [MarshalAs(UnmanagedType.LPStr)] string pchConnectString);
    }
    public enum EPersonaState
    {
        k_EPersonaStateOffline = 0,         // friend is not currently logged on
        k_EPersonaStateOnline = 1,          // friend is logged on
        k_EPersonaStateBusy = 2,            // user is on, but busy
        k_EPersonaStateAway = 3,            // auto-away feature
        k_EPersonaStateSnooze = 4,          // auto-away for a long time
        k_EPersonaStateLookingToTrade = 5,  // Online, trading
        k_EPersonaStateLookingToPlay = 6,   // Online, wanting to play
        k_EPersonaStateInvisible = 7,       // Online, but appears offline to friends.  This status is never published to clients.
        k_EPersonaStateMax,
    };
    public enum EFriendRelationship
    {
        k_EFriendRelationshipNone = 0,
        k_EFriendRelationshipBlocked = 1,           // this doesn't get stored; the user has just done an Ignore on an friendship invite
        k_EFriendRelationshipRequestRecipient = 2,
        k_EFriendRelationshipFriend = 3,
        k_EFriendRelationshipRequestInitiator = 4,
        k_EFriendRelationshipIgnored = 5,           // this is stored; the user has explicit blocked this other user from comments/chat/etc
        k_EFriendRelationshipIgnoredFriend = 6,
        k_EFriendRelationshipSuggested_DEPRECATED = 7,      // was used by the original implementation of the facebook linking feature, but now unused.

        // keep this updated
        k_EFriendRelationshipMax = 8,
    };
    public enum EOverlayToStoreFlag
    {
        k_EOverlayToStoreFlag_None = 0,
        k_EOverlayToStoreFlag_AddToCart = 1,
        k_EOverlayToStoreFlag_AddToCartAndShow = 2,
    };
    public enum EChatEntryType
    {
        k_EChatEntryTypeInvalid = 0,
        k_EChatEntryTypeChatMsg = 1,        // Normal text message from another user
        k_EChatEntryTypeTyping = 2,         // Another user is typing (not used in multi-user chat)
        k_EChatEntryTypeInviteGame = 3,     // Invite from other user into that users current game
        k_EChatEntryTypeEmote = 4,          // text emote message (deprecated, should be treated as ChatMsg)
                                            //k_EChatEntryTypeLobbyGameStart = 5,	// lobby game is starting (dead - listen for LobbyGameCreated_t callback instead)
        k_EChatEntryTypeLeftConversation = 6, // user has left the conversation ( closed chat window )
                                              // Above are previous FriendMsgType entries, now merged into more generic chat entry types
        k_EChatEntryTypeEntered = 7,        // user has entered the conversation (used in multi-user chat and group chat)
        k_EChatEntryTypeWasKicked = 8,      // user was kicked (data: 64-bit steamid of actor performing the kick)
        k_EChatEntryTypeWasBanned = 9,      // user was banned (data: 64-bit steamid of actor performing the ban)
        k_EChatEntryTypeDisconnected = 10,  // user disconnected
        k_EChatEntryTypeHistoricalChat = 11,    // a chat message from user's chat history or offilne message
                                                //k_EChatEntryTypeReserved1 = 12, // No longer used
                                                //k_EChatEntryTypeReserved2 = 13, // No longer used
        k_EChatEntryTypeLinkBlocked = 14, // a link was removed by the chat filter.
    };
    public enum EActivateGameOverlayToWebPageMode
    {
        k_EActivateGameOverlayToWebPageMode_Default = 0,        // Browser will open next to all other windows that the user has open in the overlay.
                                                                // The window will remain open, even if the user closes then re-opens the overlay.

        k_EActivateGameOverlayToWebPageMode_Modal = 1           // Browser will be opened in a special overlay configuration which hides all other windows
                                                                // that the user has open in the overlay. When the user closes the overlay, the browser window
                                                                // will also close. When the user closes the browser window, the overlay will automatically close.
    };
}
