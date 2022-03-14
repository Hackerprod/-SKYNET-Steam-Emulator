using Steamworks;
using System;

namespace SKYNET.Interface
{
    public interface ISteamUtils
    {
        uint GetSecondsSinceAppActive();
        uint GetSecondsSinceComputerActive();

        // the universe this client is connecting to
        EUniverse GetConnectedUniverse();

        // Steam server time.  Number of seconds since January 1, 1970, GMT (i.e unix time)
        uint GetServerRealTime();

        // returns the 2 digit ISO 3166-1-alpha-2 format country code this client is running in (as looked up via an IP-to-location database)
        // e.g "US" or "UK".
        string GetIPCountry();

        // returns true if the image exists, and valid sizes were filled out
        bool GetImageSize(int iImage, uint pnWidth, uint pnHeight);

        // returns true if the image exists, and the buffer was successfully filled out
        // results are returned in RGBA format
        // the destination buffer size should be 4  height  width  sizeof(char)
        bool GetImageRGBA(int iImage, uint pubDest, int nDestBufferSize);

        // returns the IP of the reporting server for valve - currently only used in Source engine games
        bool GetCSERIPPort(uint unIP, uint usPort);

        // return the amount of battery power left in the current system in % [0..100], 255 for being on AC power
        uint GetCurrentBatteryPower();

        // returns the appID of the current process
        uint GetAppID();

        // Sets the position where the overlay instance for the currently calling game should show notifications.
        // This position is per-game and if this function is called from outside of a game context it will do nothing.
        void SetOverlayNotificationPosition(ENotificationPosition eNotificationPosition);

        // API asynchronous call results
        // can be used directly, but more commonly used via the callback dispatch API (see steam_api.h)
        bool IsAPICallCompleted(SteamAPICall_t hSteamAPICall, bool pbFailed);
        ESteamAPICallFailure GetAPICallFailureReason(SteamAPICall_t hSteamAPICall);
        bool GetAPICallResult(SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, bool pbFailed);

        // Deprecated. Applications should use SteamAPI_RunCallbacks() instead. Game servers do not need to call this function.

        // returns the number of IPC calls made since the last time this function was called
        // Used for perf debugging so you can understand how many IPC calls your game makes per frame
        // Every IPC call is at minimum a thread context switch if not a process one so you want to rate
        // control how often you do them.
        uint GetIPCCallCount();

        // API warning handling
        // 'int' is the severity; 0 for msg, 1 for warning
        // 'string ' is the text of the message
        // callbacks will occur directly after the API function is called that generated the warning or message
        void SetWarningMessageHook(SteamAPIWarningMessageHook_t pFunction);

        // Returns true if the overlay is running & the user can access it. The overlay process could take a few seconds to
        // start & hook the game process, so this function will initially return false while the overlay is loading.
        bool IsOverlayEnabled();

        // Normally this call is unneeded if your game has a constantly running frame loop that calls the 
        // D3D Present API, or OGL SwapBuffers API every frame.
        //
        // However, if you have a game that only refreshes the screen on an event driven basis then that can break 
        // the overlay, as it uses your Present/SwapBuffers calls to drive it's internal frame loop and it may also
        // need to Present() to the screen any time an even needing a notification happens or when the overlay is
        // brought up over the game by a user.  You can use this API to ask the overlay if it currently need a present
        // in that case, and then you can check for this periodically (roughly 33hz is desirable) and make sure you
        // refresh the screen with Present or SwapBuffers to allow the overlay to do it's work.
        bool BOverlayNeedsPresent();

        // Asynchronous call to check if an executable file has been signed using the public key set on the signing tab
        // of the partner site, for example to refuse to load modified executable files.  
        // The result is returned in CheckFileSignature_t.
        //   k_ECheckFileSignatureNoSignaturesFoundForThisApp - This app has not been configured on the signing tab of the partner site to enable this function.
        //   k_ECheckFileSignatureNoSignaturesFoundForThisFile - This file is not listed on the signing tab for the partner site.
        //   k_ECheckFileSignatureFileNotFound - The file does not exist on disk.
        //   k_ECheckFileSignatureInvalidSignature - The file exists, and the signing tab has been set for this file, but the file is either not signed or the signature does not match.
        //   k_ECheckFileSignatureValidSignature - The file is signed and the signature is valid.

        SteamAPICall_t CheckFileSignature(string szFileName);

        // Activates the Big Picture text input dialog which only supports gamepad input
        bool ShowGamepadTextInput(EGamepadTextInputMode eInputMode, EGamepadTextInputLineMode eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText);

        // Returns previously entered text & length
        uint GetEnteredGamepadTextLength();
        bool GetEnteredGamepadTextInput(string pchText, uint cchText);

        // returns the language the steam client is running in, you probably want ISteamApps::GetCurrentGameLanguage instead, this is for very special usage cases
        string GetSteamUILanguage();

        // returns true if Steam itself is running in VR mode
        bool IsSteamRunningInVR();

        // Sets the inset of the overlay notification from the corner specified by SetOverlayNotificationPosition.
        void SetOverlayNotificationInset(int nHorizontalInset, int nVerticalInset);

        // returns true if Steam & the Steam Overlay are running in Big Picture mode
        // Games much be launched through the Steam client to enable the Big Picture overlay. During development,
        // a game can be added as a non-steam game to the developers library to test this feature
        bool IsSteamInBigPictureMode();

        // ask SteamUI to create and render its OpenVR dashboard
        void StartVRDashboard();

        // Returns true if the HMD content will be streamed via Steam Remote Play
        bool IsVRHeadsetStreamingEnabled();

        // Set whether the HMD content will be streamed via Steam Remote Play
        // If this is set to true, then the scene in the HMD headset will be streamed, and remote input will not be allowed.
        // If this is set to false, then the application window will be streamed instead, and remote input will be allowed.
        // The default is true unless "VRHeadsetStreaming" "0" is in the extended appinfo for a game.
        // (this is useful for games that have asymmetric multiplayer gameplay)
        void SetVRHeadsetStreamingEnabled(bool bEnabled);

        // Returns whether this steam client is a Steam China specific client, vs the global client.
        bool IsSteamChinaLauncher();

        // Initializes text filtering.
        //   Returns false if filtering is unavailable for the language the user is currently running in.
        bool InitFilterText();

        // Filters the provided input message and places the filtered result into pchOutFilteredText.
        //   pchOutFilteredText is where the output will be placed, even if no filtering or censoring is performed
        //   nByteSizeOutFilteredText is the size (in bytes) of pchOutFilteredText
        //   pchInputText is the input string that should be filtered, which can be ASCII or UTF-8
        //   bLegalOnly should be false if you want profanity and legally required filtering (where required) and true if you want legally required filtering only
        //   Returns the number of characters (not bytes) filtered.
        int FilterText(string pchOutFilteredText, uint nByteSizeOutFilteredText, string pchInputMessage, bool bLegalOnly);

        // Return what we believe your current ipv6 connectivity to "the internet" is on the specified protocol.
        // This does NOT tell you if the Steam client is currently connected to Steam via ipv6.
        ESteamIPv6ConnectivityState GetIPv6ConnectivityState(ESteamIPv6ConnectivityProtocol eProtocol);
    }
    // For the above transport protocol, what do we think the local machine's connectivity to the internet over ipv6 is like
    public enum ESteamIPv6ConnectivityState : int
    {
        k_ESteamIPv6ConnectivityState_Unknown = 0,  // We haven't run a test yet
        k_ESteamIPv6ConnectivityState_Good = 1,     // We have recently been able to make a request on ipv6 for the given protocol
        k_ESteamIPv6ConnectivityState_Bad = 2,      // We failed to make a request, either because this machine has no ipv6 address assigned, or it has no upstream connectivity
    };
    // Input modes for the Big Picture gamepad text entry
    public enum EGamepadTextInputMode : int
    {
        k_EGamepadTextInputModeNormal = 0,
        k_EGamepadTextInputModePassword = 1
    };
    // Controls number of allowed lines for the Big Picture gamepad text entry
    public enum EGamepadTextInputLineMode : int
    {
        k_EGamepadTextInputLineModeSingleLine = 0,
        k_EGamepadTextInputLineModeMultipleLines = 1
    };
    //-----------------------------------------------------------------------------
    // Purpose: Possible positions to tell the overlay to show notifications in
    //-----------------------------------------------------------------------------
    public enum ENotificationPosition : int
    {
        k_EPositionTopLeft = 0,
        k_EPositionTopRight = 1,
        k_EPositionBottomLeft = 2,
        k_EPositionBottomRight = 3,
    };
    // Steam API call failure results
    public enum ESteamAPICallFailure : int
    {
        k_ESteamAPICallFailureNone = -1,            // no failure
        k_ESteamAPICallFailureSteamGone = 0,        // the local Steam process has gone away
        k_ESteamAPICallFailureNetworkFailure = 1,   // the network connection to Steam has been broken, or was already broken
                                                    // SteamServersDisconnected_t callback will be sent around the same time
                                                    // SteamServersConnected_t will be sent when the client is able to talk to the Steam servers again
        k_ESteamAPICallFailureInvalidHandle = 2,    // the SteamAPICall_t handle passed in no longer exists
        k_ESteamAPICallFailureMismatchedCallback = 3,// GetAPICallResult() was called with the wrong callback type for this API call
    };
    public enum ESteamIPv6ConnectivityProtocol : int
    {
        k_ESteamIPv6ConnectivityProtocol_Invalid = 0,
        k_ESteamIPv6ConnectivityProtocol_HTTP = 1,      // because a proxy may make this different than other protocols
        k_ESteamIPv6ConnectivityProtocol_UDP = 2,       // test UDP connectivity. Uses a port that is commonly needed for other Steam stuff. If UDP works, TCP probably works. 
    };
}