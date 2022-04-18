﻿namespace Steamworks
{
    public static class Constants
    {
        public const string STEAMAPPLIST_INTERFACE_VERSION = "STEAMAPPLIST_INTERFACE_VERSION001";
        public const string STEAMAPPS_INTERFACE_VERSION = "STEAMAPPS_INTERFACE_VERSION007";
        public const string STEAMAPPTICKET_INTERFACE_VERSION = "STEAMAPPTICKET_INTERFACE_VERSION001";
        public const string STEAMCLIENT_INTERFACE_VERSION = "SteamClient017";
        public const string STEAMCONTROLLER_INTERFACE_VERSION = "STEAMCONTROLLER_INTERFACE_VERSION";
        public const string STEAMFRIENDS_INTERFACE_VERSION = "SteamFriends015";
        public const string STEAMGAMECOORDINATOR_INTERFACE_VERSION = "SteamGameCoordinator001";
        public const string STEAMGAMESERVER_INTERFACE_VERSION = "SteamGameServer012";
        public const string STEAMGAMESERVERSTATS_INTERFACE_VERSION = "SteamGameServerStats001";
        public const string STEAMHTMLSURFACE_INTERFACE_VERSION = "STEAMHTMLSURFACE_INTERFACE_VERSION_003";
        public const string STEAMHTTP_INTERFACE_VERSION = "STEAMHTTP_INTERFACE_VERSION002";
        public const string STEAMINVENTORY_INTERFACE_VERSION = "STEAMINVENTORY_INTERFACE_V001";
        public const string STEAMMATCHMAKING_INTERFACE_VERSION = "SteamMatchMaking009";
        public const string STEAMMATCHMAKINGSERVERS_INTERFACE_VERSION = "SteamMatchMakingServers002";
        public const string STEAMMUSIC_INTERFACE_VERSION = "STEAMMUSIC_INTERFACE_VERSION001";
        public const string STEAMMUSICREMOTE_INTERFACE_VERSION = "STEAMMUSICREMOTE_INTERFACE_VERSION001";
        public const string STEAMNETWORKING_INTERFACE_VERSION = "SteamNetworking005";
        public const string STEAMREMOTESTORAGE_INTERFACE_VERSION = "STEAMREMOTESTORAGE_INTERFACE_VERSION012";
        public const string STEAMSCREENSHOTS_INTERFACE_VERSION = "STEAMSCREENSHOTS_INTERFACE_VERSION002";
        public const string STEAMUGC_INTERFACE_VERSION = "STEAMUGC_INTERFACE_VERSION007";
        public const string STEAMUNIFIEDMESSAGES_INTERFACE_VERSION = "STEAMUNIFIEDMESSAGES_INTERFACE_VERSION001";
        public const string STEAMUSER_INTERFACE_VERSION = "SteamUser018";
        public const string STEAMUSERSTATS_INTERFACE_VERSION = "STEAMUSERSTATS_INTERFACE_VERSION011";
        public const string STEAMUTILS_INTERFACE_VERSION = "SteamUtils007";
        public const string STEAMVIDEO_INTERFACE_VERSION = "STEAMVIDEO_INTERFACE_V001";
        public const int k_cubAppProofOfPurchaseKeyMax = 64; // max bytes of a legacy cd key we support
                                                             //-----------------------------------------------------------------------------
                                                             // Purpose: Base values for callback identifiers, each callback must
                                                             //			have a unique ID.
                                                             //-----------------------------------------------------------------------------
        public const int k_iSteamUserCallbacks = 100;
        public const int k_iSteamGameServerCallbacks = 200;
        public const int k_iSteamFriendsCallbacks = 300;
        public const int k_iSteamBillingCallbacks = 400;
        public const int k_iSteamMatchmakingCallbacks = 500;
        public const int k_iSteamContentServerCallbacks = 600;
        public const int k_iSteamUtilsCallbacks = 700;
        public const int k_iClientFriendsCallbacks = 800;
        public const int k_iClientUserCallbacks = 900;
        public const int k_iSteamAppsCallbacks = 1000;
        public const int k_iSteamUserStatsCallbacks = 1100;
        public const int k_iSteamNetworkingCallbacks = 1200;
        public const int k_iClientRemoteStorageCallbacks = 1300;
        public const int k_iClientDepotBuilderCallbacks = 1400;
        public const int k_iSteamGameServerItemsCallbacks = 1500;
        public const int k_iClientUtilsCallbacks = 1600;
        public const int k_iSteamGameCoordinatorCallbacks = 1700;
        public const int k_iSteamGameServerStatsCallbacks = 1800;
        public const int k_iSteam2AsyncCallbacks = 1900;
        public const int k_iSteamGameStatsCallbacks = 2000;
        public const int k_iClientHTTPCallbacks = 2100;
        public const int k_iClientScreenshotsCallbacks = 2200;
        public const int k_iSteamScreenshotsCallbacks = 2300;
        public const int k_iClientAudioCallbacks = 2400;
        public const int k_iClientUnifiedMessagesCallbacks = 2500;
        public const int k_iSteamStreamLauncherCallbacks = 2600;
        public const int k_iClientControllerCallbacks = 2700;
        public const int k_iSteamControllerCallbacks = 2800;
        public const int k_iClientParentalSettingsCallbacks = 2900;
        public const int k_iClientDeviceAuthCallbacks = 3000;
        public const int k_iClientNetworkDeviceManagerCallbacks = 3100;
        public const int k_iClientMusicCallbacks = 3200;
        public const int k_iClientRemoteClientManagerCallbacks = 3300;
        public const int k_iClientUGCCallbacks = 3400;
        public const int k_iSteamStreamClientCallbacks = 3500;
        public const int k_IClientProductBuilderCallbacks = 3600;
        public const int k_iClientShortcutsCallbacks = 3700;
        public const int k_iClientRemoteControlManagerCallbacks = 3800;
        public const int k_iSteamAppListCallbacks = 3900;
        public const int k_iSteamMusicCallbacks = 4000;
        public const int k_iSteamMusicRemoteCallbacks = 4100;
        public const int k_iClientVRCallbacks = 4200;
        public const int k_iClientReservedCallbacks = 4300;
        public const int k_iSteamReservedCallbacks = 4400;
        public const int k_iSteamHTMLSurfaceCallbacks = 4500;
        public const int k_iClientVideoCallbacks = 4600;
        public const int k_iClientInventoryCallbacks = 4700;
        // maximum length of friend group name (not including terminating nul!)
        public const int k_cchMaxFriendsGroupName = 64;
        // maximum number of groups a single user is allowed
        public const int k_cFriendsGroupLimit = 100;
        public const int k_cEnumerateFollowersMax = 50;
        // maximum number of characters in a user's name. Two flavors; one for UTF-8 and one for UTF-16.
        // The UTF-8 version has to be very generous to accomodate characters that get large when encoded
        // in UTF-8.
        public const int k_cchPersonaNameMax = 128;
        public const int k_cwchPersonaNameMax = 32;
        // size limit on chat room or member metadata
        public const int k_cubChatMetadataMax = 8192;
        // size limits on Rich Presence data
        public const int k_cchMaxRichPresenceKeys = 20;
        public const int k_cchMaxRichPresenceKeyLength = 64;
        public const int k_cchMaxRichPresenceValueLength = 256;
        // game server flags
        public const int k_unServerFlagNone = 0x00;
        public const int k_unServerFlagActive = 0x01; // server has users playing
        public const int k_unServerFlagSecure = 0x02; // server wants to be secure
        public const int k_unServerFlagDedicated = 0x04; // server is dedicated
        public const int k_unServerFlagLinux = 0x08; // linux build
        public const int k_unServerFlagPassworded = 0x10; // password protected
        public const int k_unServerFlagPrivate = 0x20; // server shouldn't list on master server and
                                                       // game server flags
        public const int k_unFavoriteFlagNone = 0x00;
        public const int k_unFavoriteFlagFavorite = 0x01; // this game favorite entry is for the favorites list
        public const int k_unFavoriteFlagHistory = 0x02; // this game favorite entry is for the history list
                                                         //-----------------------------------------------------------------------------
                                                         // Purpose: Defines the largest allowed file size. Cloud files cannot be written
                                                         // in a single chunk over 100MB (and cannot be over 200MB total.)
                                                         //-----------------------------------------------------------------------------
        public const int k_unMaxCloudFileChunkSize = 100 * 1024 * 1024;
        public const int k_cchPublishedDocumentTitleMax = 128 + 1;
        public const int k_cchPublishedDocumentDescriptionMax = 8000;
        public const int k_cchPublishedDocumentChangeDescriptionMax = 8000;
        public const int k_unEnumeratePublishedFilesMaxResults = 50;
        public const int k_cchTagListMax = 1024 + 1;
        public const int k_cchFilenameMax = 260;
        public const int k_cchPublishedFileURLMax = 256;
        public const int k_nScreenshotMaxTaggedUsers = 32;
        public const int k_nScreenshotMaxTaggedPublishedFiles = 32;
        public const int k_cubUFSTagTypeMax = 255;
        public const int k_cubUFSTagValueMax = 255;
        // Required with of a thumbnail provided to AddScreenshotToLibrary.  If you do not provide a thumbnail
        // one will be generated.
        public const int k_ScreenshotThumbWidth = 200;
        public const int kNumUGCResultsPerPage = 50;
        public const int k_cchDeveloperMetadataMax = 5000;
        // size limit on stat or achievement name (UTF-8 encoded)
        public const int k_cchStatNameMax = 128;
        // maximum number of bytes for a leaderboard name (UTF-8 encoded)
        public const int k_cchLeaderboardNameMax = 128;
        // maximum number of details int32's storable for a single leaderboard entry
        public const int k_cLeaderboardDetailsMax = 64;
        //
        // Max size (in bytes of UTF-8 data, not in characters) of server fields, including null terminator.
        // WARNING: These cannot be changed easily, without breaking clients using old interfaces.
        //
        public const int k_cbMaxGameServerGameDir = 32;
        public const int k_cbMaxGameServerMapName = 32;
        public const int k_cbMaxGameServerGameDescription = 64;
        public const int k_cbMaxGameServerName = 64;
        public const int k_cbMaxGameServerTags = 128;
        public const int k_cbMaxGameServerGameData = 2048;
        public const int k_unSteamAccountIDMask = -1;
        public const int k_unSteamAccountInstanceMask = 0x000FFFFF;
        // we allow 3 simultaneous user account instances right now, 1= desktop, 2 = console, 4 = web, 0 = all
        public const int k_unSteamUserDesktopInstance = 1;
        public const int k_unSteamUserConsoleInstance = 2;
        public const int k_unSteamUserWebInstance = 4;
        public const int k_cchGameExtraInfoMax = 64;
        public const int k_nSteamEncryptedAppTicketSymmetricKeyLen = 32;
        public const int k_cubSaltSize = 8;
        public const ulong k_GIDNil = 0xffffffffffffffff;
        public const ulong k_TxnIDNil = k_GIDNil;
        public const ulong k_TxnIDUnknown = 0;
        public const int k_uPackageIdFreeSub = 0x0;
        public const int k_uPackageIdInvalid = -1;
        public const ulong k_ulAssetClassIdInvalid = 0x0;
        public const int k_uPhysicalItemIdInvalid = 0x0;
        public const int k_uCellIDInvalid = -1;
        public const int k_uPartnerIdInvalid = 0;
        // callbacks
        public const int MAX_STEAM_CONTROLLERS = 16;
        public const int STEAM_RIGHT_TRIGGER_MASK = 0x0000001;
        public const int STEAM_LEFT_TRIGGER_MASK = 0x0000002;
        public const int STEAM_RIGHT_BUMPER_MASK = 0x0000004;
        public const int STEAM_LEFT_BUMPER_MASK = 0x0000008;
        public const int STEAM_BUTTON_0_MASK = 0x0000010;
        public const int STEAM_BUTTON_1_MASK = 0x0000020;
        public const int STEAM_BUTTON_2_MASK = 0x0000040;
        public const int STEAM_BUTTON_3_MASK = 0x0000080;
        public const int STEAM_TOUCH_0_MASK = 0x0000100;
        public const int STEAM_TOUCH_1_MASK = 0x0000200;
        public const int STEAM_TOUCH_2_MASK = 0x0000400;
        public const int STEAM_TOUCH_3_MASK = 0x0000800;
        public const int STEAM_BUTTON_MENU_MASK = 0x0001000;
        public const int STEAM_BUTTON_STEAM_MASK = 0x0002000;
        public const int STEAM_BUTTON_ESCAPE_MASK = 0x0004000;
        public const int STEAM_BUTTON_BACK_LEFT_MASK = 0x0008000;
        public const int STEAM_BUTTON_BACK_RIGHT_MASK = 0x0010000;
        public const int STEAM_BUTTON_LEFTPAD_CLICKED_MASK = 0x0020000;
        public const int STEAM_BUTTON_RIGHTPAD_CLICKED_MASK = 0x0040000;
        public const int STEAM_LEFTPAD_FINGERDOWN_MASK = 0x0080000;
        public const int STEAM_RIGHTPAD_FINGERDOWN_MASK = 0x0100000;
        public const int STEAM_JOYSTICK_BUTTON_MASK = 0x0400000;
        public const short MASTERSERVERUPDATERPORT_USEGAMESOCKETSHARE = -1;
        public const int INVALID_HTTPREQUEST_HANDLE = 0;
        // maximum number of characters a lobby metadata key can be
        public const byte k_nMaxLobbyKeyLength = 255;
        public const int k_SteamMusicNameMaxLength = 255;
        public const int k_SteamMusicPNGMaxLength = 65535;
        //-----------------------------------------------------------------------------
        // Constants used for query ports.
        //-----------------------------------------------------------------------------
        public const int QUERY_PORT_NOT_INITIALIZED = 0xFFFF; // We haven't asked the GS for this query port's actual value yet.
        public const int QUERY_PORT_ERROR = 0xFFFE; // We were unable to get the query port for this server.
    }
}