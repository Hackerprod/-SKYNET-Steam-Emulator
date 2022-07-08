using System;

namespace SKYNET.Steamworks
{
    public static class Constants
    {
        public const int k_cubAppProofOfPurchaseKeyMax = 64;

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
        public const int k_cchPersonaNameMax = 128;
        public const int k_cwchPersonaNameMax = 32;
        public const int k_cubChatMetadataMax = 8192;
        public const int k_cchMaxRichPresenceKeys = 20;
        public const int k_cchMaxRichPresenceKeyLength = 64;
        public const int k_cchMaxRichPresenceValueLength = 256;

        public const UInt32 k_unServerFlagNone = 0x00;
        public const UInt32 k_unServerFlagActive = 0x01;        // server has users playing
        public const UInt32 k_unServerFlagSecure = 0x02;        // server wants to be secure
        public const UInt32 k_unServerFlagDedicated = 0x04;     // server is dedicated
        public const UInt32 k_unServerFlagLinux = 0x08;         // linux build
        public const UInt32 k_unServerFlagPassworded = 0x10;    // password protected
        public const UInt32 k_unServerFlagPrivate = 0x20;       // server shouldn't list on master server and
                                                                // won't enforce authentication of users that connect to the server.
                                                                // Useful when you run a server where the clients may not
                                                                // be connected to the internet but you want them to play (i.e LANs)
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


        //-----------------------------------------------------------------------------
        // Purpose: Base values for callback identifiers, each callback must
        //			have a unique ID.
        //-----------------------------------------------------------------------------
        public const int k_iSteamNetworkingSocketsCallbacks = 1220;
        public const int k_iSteamNetworkingMessagesCallbacks = 1250;
        public const int k_iSteamNetworkingUtilsCallbacks = 1280;
        public const int k_iSteamRemoteStorageCallbacks = 1300;
        public const int k_iSteamHTTPCallbacks = 2100;
        // NOTE: 2500-2599 are reserved
        public const int k_iSteamUGCCallbacks = 3400;
        public const int k_iSteamGameNotificationCallbacks = 4400;
        public const int k_iSteamVideoCallbacks = 4600;
        public const int k_iSteamInventoryCallbacks = 4700;
        public const int k_ISteamParentalSettingsCallbacks = 5000;
        public const int k_iSteamGameSearchCallbacks = 5200;
        public const int k_iSteamPartiesCallbacks = 5300;
        public const int k_iSteamSTARCallbacks = 5500;
        public const int k_iSteamRemotePlayCallbacks = 5700;
        public const int k_iSteamChatCallbacks = 5900;

        public const ushort STEAMGAMESERVER_QUERY_PORT_SHARED = 0xffff;
        public const int k_unSteamUserDefaultInstance = 1; // fixed instance for all individual users

        public const int k_nMaxReturnPorts = 8;
        public const int k_cchMaxSteamNetworkingErrMsg = 1024;
        public const int k_cchSteamNetworkingMaxConnectionCloseReason = 128;
        public const int k_cchSteamNetworkingMaxConnectionDescription = 128;
        public const int k_cchSteamNetworkingMaxConnectionAppName = 32;

        public const int k_nSteamNetworkConnectionInfoFlags_Unauthenticated = 1; // We don't have a certificate for the remote host.
        public const int k_nSteamNetworkConnectionInfoFlags_Unencrypted = 2; // Information is being sent out over a wire unencrypted (by this library)
        public const int k_nSteamNetworkConnectionInfoFlags_LoopbackBuffers = 4; // Internal loopback buffers.  Won't be true for localhost.  (You can check the address to determine that.)  This implies k_nSteamNetworkConnectionInfoFlags_FastLAN
        public const int k_nSteamNetworkConnectionInfoFlags_Fast = 8; // The connection is "fast" and "reliable".  Either internal/localhost (check the address to find out), or the peer is on the same LAN.  (Probably.  It's based on the address and the ping time, this is actually hard to determine unambiguously).
        public const int k_nSteamNetworkConnectionInfoFlags_Relayed = 16; // The connection is relayed somehow (SDR or TURN).
        public const int k_nSteamNetworkConnectionInfoFlags_DualWifi = 32; // We're taking advantage of dual-wifi multi-path
                                                                           //
                                                                           // Network messages
                                                                           //
                                                                           /// Max size of a single message that we can SEND.
                                                                           /// Note: We might be wiling to receive larger messages,
                                                                           /// and our peer might, too.
        public const int k_cbMaxSteamNetworkingSocketsMessageSizeSend = 512 * 1024;
        public const int k_nSteamNetworkingSend_Unreliable = 0;
        public const int k_nSteamNetworkingSend_NoNagle = 1;
        public const int k_nSteamNetworkingSend_UnreliableNoNagle = k_nSteamNetworkingSend_Unreliable | k_nSteamNetworkingSend_NoNagle;
        public const int k_nSteamNetworkingSend_NoDelay = 4;
        public const int k_nSteamNetworkingSend_UnreliableNoDelay = k_nSteamNetworkingSend_Unreliable | k_nSteamNetworkingSend_NoDelay | k_nSteamNetworkingSend_NoNagle;
        public const int k_nSteamNetworkingSend_Reliable = 8;
        public const int k_nSteamNetworkingSend_ReliableNoNagle = k_nSteamNetworkingSend_Reliable | k_nSteamNetworkingSend_NoNagle;
        public const int k_nSteamNetworkingSend_UseCurrentThread = 16;
        public const int k_nSteamNetworkingSend_AutoRestartBrokenSession = 32;
        public const int k_cchMaxSteamNetworkingPingLocationString = 1024;
        public const int k_nSteamNetworkingPing_Failed = -1;
        public const int k_nSteamNetworkingPing_Unknown = -2;
        public const int k_nSteamNetworkingConfig_P2P_Transport_ICE_Enable_Default = -1; // Special value - use user defaults
        public const int k_nSteamNetworkingConfig_P2P_Transport_ICE_Enable_Disable = 0; // Do not do any ICE work at all or share any IP addresses with peer
        public const int k_nSteamNetworkingConfig_P2P_Transport_ICE_Enable_Relay = 1; // Relayed connection via TURN server.
        public const int k_nSteamNetworkingConfig_P2P_Transport_ICE_Enable_Private = 2; // host addresses that appear to be link-local or RFC1918 addresses
        public const int k_nSteamNetworkingConfig_P2P_Transport_ICE_Enable_Public = 4; // STUN reflexive addresses, or host address that isn't a "private" address
        public const int k_nSteamNetworkingConfig_P2P_Transport_ICE_Enable_All = 0x7fffffff;
        public const ulong k_ulPartyBeaconIdInvalid = 0;
        public const int STEAM_INPUT_MAX_COUNT = 16;
        public const int STEAM_INPUT_MAX_ANALOG_ACTIONS = 16;
        public const int STEAM_INPUT_MAX_DIGITAL_ACTIONS = 128;
        public const int STEAM_INPUT_MAX_ORIGINS = 8;
        public const int STEAM_INPUT_MAX_ACTIVE_LAYERS = 16;
        // When sending an option to a specific controller handle, you can send to all devices via this command
        public const ulong STEAM_INPUT_HANDLE_ALL_CONTROLLERS = 0xFFFFFFFFFFFFFFFF;
        public const float STEAM_INPUT_MIN_ANALOG_ACTION_DATA = -1.0f;
        public const float STEAM_INPUT_MAX_ANALOG_ACTION_DATA = 1.0f;
    }
}