using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamUser
    {
        // returns the HSteamUser this interface represents
        // this is only used internally by the API, and by a few select interfaces that support multi-user
        HSteamUser GetHSteamUser(IntPtr _);

        // returns true if the Steam client current has a live connection to the Steam servers. 
        // If false, it means there is no active connection due to either a networking issue on the local machine, or the Steam server is down/busy.
        // The Steam client will automatically be trying to recreate the connection as often as possible.
        bool BLoggedOn(IntPtr _);

        // returns the IntPtr of the account currently logged into the Steam client
        // a IntPtr is a unique identifier for an account, and used to differentiate users in all parts of the Steamworks API
        IntPtr GetSteamID(IntPtr _);

        // Multiplayer Authentication functions

        // InitiateGameConnection() starts the state machine for authenticating the game client with the game server
        // It is the client portion of a three-way handshake between the client, the game server, and the steam servers
        //
        // Parameters:
        // void pAuthBlob - a pointer to empty memory that will be filled in with the authentication token.
        // int cbMaxAuthBlob - the number of bytes of allocated memory in pBlob. Should be at least 2048 bytes.
        // IntPtr steamIDGameServer - the steamID of the game server, received from the game server by the client
        // CGameID gameID - the ID of the current game. For games without mods, this is just CGameID( <appID> )
        // uint unIPServer, uint16 usPortServer - the IP address of the game server
        // bool bSecure - whether or not the client thinks that the game server is reporting itself as secure (i.e. VAC is running)
        //
        // return value - returns the number of bytes written to pBlob. If the return is 0, then the buffer passed in was too small, and the call has failed
        // The contents of pBlob should then be sent to the game server, for it to use to complete the authentication process.
        int InitiateGameConnection(IntPtr _, IntPtr pAuthBlob, int cbMaxAuthBlob, IntPtr steamIDGameServer, uint unIPServer, uint usPortServer, bool bSecure);

        // notify of disconnect
        // needs to occur when the game client leaves the specified game server, needs to match with the InitiateGameConnection() call
        void TerminateGameConnection(IntPtr _, uint unIPServer, uint usPortServer);

        // Legacy functions

        // used by only a few games to track usage events
        void TrackAppUsageEvent(IntPtr _, IntPtr gameID, int eAppUsageEvent, string pchExtraInfo = "");

        // get the local storage folder for current Steam account to write application data, e.g. save games, configs etc.
        // this will usually be something like "C:\Progam Files\Steam\userdata\<SteamID>\<AppID>\local"
        bool GetUserDataFolder(IntPtr _, string pchBuffer, int cubBuffer);

        // Starts voice recording. Once started, use GetVoice() to get the data
        void StartVoiceRecording(IntPtr _);

        // Stops voice recording. Because people often release push-to-talk keys early, the system will keep recording for
        // a little bit after this function is called. GetVoice() should continue to be called until it returns
        // k_eVoiceResultNotRecording
        void StopVoiceRecording(IntPtr _);

        // Determine the size of captured audio data that is available from GetVoice.
        // Most applications will only use compressed data and should ignore the other
        // parameters, which exist primarily for backwards compatibility. See comments
        // below for further explanation of "uncompressed" data.
        EVoiceResult GetAvailableVoice(IntPtr _, uint pcbCompressed, uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated);

        // ---------------------------------------------------------------------------
        // NOTE: "uncompressed" audio is a deprecated feature and should not be used
        // by most applications. It is raw single-channel 16-bit PCM wave data which
        // may have been run through preprocessing filters and/or had silence removed,
        // so the uncompressed audio could have a shorter duration than you expect.
        // There may be no data at all during long periods of silence. Also, fetching
        // uncompressed audio will cause GetVoice to discard any leftover compressed
        // audio, so you must fetch both types at once. Finally, GetAvailableVoice is
        // not precisely accurate when the uncompressed size is requested. So if you
        // really need to use uncompressed audio, you should call GetVoice frequently
        // with two very large (20kb+) output buffers instead of trying to allocate
        // perfectly-sized buffers. But most applications should ignore all of these
        // details and simply leave the "uncompressed" parameters as NULL/zero.
        // ---------------------------------------------------------------------------

        // Read captured audio data from the microphone buffer. This should be called
        // at least once per frame, and preferably every few milliseconds, to keep the
        // microphone input delay as low as possible. Most applications will only use
        // compressed data and should pass NULL/zero for the "uncompressed" parameters.
        // Compressed data can be transmitted by your application and decoded into raw
        // using the DecompressVoice function below.
        EVoiceResult GetVoice(IntPtr _, bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated);

        // Decodes the compressed voice data returned by GetVoice. The output data is
        // raw single-channel 16-bit PCM audio. The decoder supports any sample rate
        // from 11025 to 48000; see GetVoiceOptimalSampleRate() below for details.
        // If the output buffer is not large enough, then nBytesWritten will be set
        // to the required buffer size, and k_EVoiceResultBufferTooSmall is returned.
        // It is suggested to start with a 20kb buffer and reallocate as necessary.
        EVoiceResult DecompressVoice(IntPtr _, IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, uint nBytesWritten, uint nDesiredSampleRate);

        // This returns the native sample rate of the Steam voice decompressor; using
        // this sample rate for DecompressVoice will perform the least CPU processing.
        // However, the final audio quality will depend on how well the audio device
        // (and/or your application's audio output SDK) deals with lower sample rates.
        // You may find that you get the best audio output quality when you ignore
        // this function and use the native sample rate of your audio output device,
        // which is usually 48000 or 44100.
        uint GetVoiceOptimalSampleRate(IntPtr _);

        // Retrieve ticket to be sent to the entity who wishes to authenticate you. 
        // pcbTicket retrieves the length of the actual ticket.
        HAuthTicket GetAuthSessionTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, uint pcbTicket);

        // Authenticate ticket from entity steamID to be sure it is valid and isnt reused
        // Registers for callbacks if the entity goes offline or cancels the ticket ( see ValidateAuthTicketResponse_t callback and EAuthSessionResponse )
        EBeginAuthSessionResult BeginAuthSession(IntPtr _, IntPtr pAuthTicket, int cbAuthTicket, IntPtr steamID);

        // Stop tracking started by BeginAuthSession - called when no longer playing game with this entity
        void EndAuthSession(IntPtr _, IntPtr steamID);

        // Cancel auth ticket from GetAuthSessionTicket, called when no longer playing game with the entity you gave the ticket to
        void CancelAuthTicket(IntPtr _, HAuthTicket hAuthTicket);

        // After receiving a user's authentication data, and passing it to BeginAuthSession, use this function
        // to determine if the user owns downloadable content specified by the provided AppID.
        EUserHasLicenseForAppResult UserHasLicenseForApp(IntPtr _, IntPtr steamID, AppId_t appID);

        // returns true if this users looks like they are behind a NAT device. Only valid once the user has connected to steam 
        // (i.e a SteamServersConnected_t has been issued) and may not catch all forms of NAT.
        bool BIsBehindNAT(IntPtr _);

        // set data to be replicated to friends so that they can join your game
        // IntPtr steamIDGameServer - the steamID of the game server, received from the game server by the client
        // uint unIPServer, uint16 usPortServer - the IP address of the game server
        void AdvertiseGame(IntPtr _, IntPtr steamIDGameServer, uint unIPServer, uint usPortServer);

        // Requests a ticket encrypted with an app specific shared key
        // pDataToInclude, cbDataToInclude will be encrypted into the ticket
        // ( This is asynchronous, you must wait for the ticket to be completed by the server )

        SteamAPICall_t RequestEncryptedAppTicket(IntPtr _, IntPtr pDataToInclude, int cbDataToInclude);

        // Retrieves a finished ticket.
        // If no ticket is available, or your buffer is too small, returns false.
        // Upon exit, pcbTicket will be either the size of the ticket copied into your buffer
        // (if true was returned), or the size needed (if false was returned).  To determine the
        // proper size of the ticket, you can pass pTicket=NULL and cbMaxTicket=0; if a ticket
        // is available, pcbTicket will contain the size needed, otherwise it will be zero.
        bool GetEncryptedAppTicket(IntPtr _, IntPtr pTicket, int cbMaxTicket, uint pcbTicket);

        // Trading Card badges data access
        // if you only have one set of cards, the series will be 1
        // the user has can have two different badges for a series; the regular (max level 5) and the foil (max level 1)
        int GetGameBadgeLevel(IntPtr _, int nSeries, bool bFoil);

        // gets the Steam Level of the user, as shown on their profile
        int GetPlayerSteamLevel(IntPtr _);

        // Requests a URL which authenticates an in-game browser for store check-out,
        // and then redirects to the specified URL. As long as the in-game browser
        // accepts and handles session cookies, Steam microtransaction checkout pages
        // will automatically recognize the user instead of presenting a login page.
        // The result of this API call will be a StoreAuthURLResponse_t callback.
        // NOTE: The URL has a very short lifetime to prevent history-snooping attacks,
        // so you should only call this API when you are about to launch the browser,
        // or else immediately navigate to the result URL using a hidden browser window.
        // NOTE 2: The resulting authorization cookie has an expiration time of one day,
        // so it would be a good idea to request and visit a new auth URL every 12 hours.

        SteamAPICall_t RequestStoreAuthURL(IntPtr _, string pchRedirectURL);

        // gets whether the users phone number is verified 
        bool BIsPhoneVerified(IntPtr _);

        // gets whether the user has two factor enabled on their account
        bool BIsTwoFactorEnabled(IntPtr _);

        // gets whether the users phone number is identifying
        bool BIsPhoneIdentifying(IntPtr _);

        // gets whether the users phone number is awaiting (re)verification
        bool BIsPhoneRequiringVerification(IntPtr _);


        SteamAPICall_t GetMarketEligibility(IntPtr _);

        // Retrieves anti indulgence / duration control for current user

        SteamAPICall_t GetDurationControl(IntPtr _);

        // Advise steam china duration control system about the online state of the game.
        // This will prevent offline gameplay time from counting against a user's
        // playtime limits.
        bool BSetDurationControlOnlineState(IntPtr _, EDurationControlOnlineState eNewState);

    }
    // Error codes for use with the voice functions
    public enum EVoiceResult : int
    {
        k_EVoiceResultOK = 0,
        k_EVoiceResultNotInitialized = 1,
        k_EVoiceResultNotRecording = 2,
        k_EVoiceResultNoData = 3,
        k_EVoiceResultBufferTooSmall = 4,
        k_EVoiceResultDataCorrupted = 5,
        k_EVoiceResultRestricted = 6,
        k_EVoiceResultUnsupportedCodec = 7,
        k_EVoiceResultReceiverOutOfDate = 8,
        k_EVoiceResultReceiverDidNotAnswer = 9,

    };
    //
    // Specifies a game's online state in relation to duration control
    //
    public enum EDurationControlOnlineState : int
    {
        k_EDurationControlOnlineState_Invalid = 0,              // nil value
        k_EDurationControlOnlineState_Offline = 1,              // currently in offline play - single-player, offline co-op, etc.
        k_EDurationControlOnlineState_Online = 2,               // currently in online play
        k_EDurationControlOnlineState_OnlineHighPri = 3,        // currently in online play and requests not to be interrupted
    };
    // results from UserHasLicenseForApp
    public enum EUserHasLicenseForAppResult : int
    {
        k_EUserHasLicenseResultHasLicense = 0,                  // User has a license for specified app
        k_EUserHasLicenseResultDoesNotHaveLicense = 1,          // User does not have a license for the specified app
        k_EUserHasLicenseResultNoAuth = 2,                      // User has not been authenticated
    };
    // results from BeginAuthSession
    public enum EBeginAuthSessionResult : int
    {
        k_EBeginAuthSessionResultOK = 0,                        // Ticket is valid for this game and this steamID.
        k_EBeginAuthSessionResultInvalidTicket = 1,             // Ticket is not valid.
        k_EBeginAuthSessionResultDuplicateRequest = 2,          // A ticket has already been submitted for this steamID
        k_EBeginAuthSessionResultInvalidVersion = 3,            // Ticket is from an incompatible interface version
        k_EBeginAuthSessionResultGameMismatch = 4,              // Ticket is not for this game
        k_EBeginAuthSessionResultExpiredTicket = 5,             // Ticket has expired
    };
}
