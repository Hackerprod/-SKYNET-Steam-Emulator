using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks
{

    public enum EUserUGCList
    {
        k_EUserUGCList_Published,
        k_EUserUGCList_VotedOn,
        k_EUserUGCList_VotedUp,
        k_EUserUGCList_VotedDown,
        k_EUserUGCList_WillVoteLater,
        k_EUserUGCList_Favorited,
        k_EUserUGCList_Subscribed,
        k_EUserUGCList_UsedOrPlayed,
        k_EUserUGCList_Followed,
    };

    public enum EControllerActionOrigin
    {
        // Steam Controller
        k_EControllerActionOrigin_None,
        k_EControllerActionOrigin_A,
        k_EControllerActionOrigin_B,
        k_EControllerActionOrigin_X,
        k_EControllerActionOrigin_Y,
        k_EControllerActionOrigin_LeftBumper,
        k_EControllerActionOrigin_RightBumper,
        k_EControllerActionOrigin_LeftGrip,
        k_EControllerActionOrigin_RightGrip,
        k_EControllerActionOrigin_Start,
        k_EControllerActionOrigin_Back,
        k_EControllerActionOrigin_LeftPad_Touch,
        k_EControllerActionOrigin_LeftPad_Swipe,
        k_EControllerActionOrigin_LeftPad_Click,
        k_EControllerActionOrigin_LeftPad_DPadNorth,
        k_EControllerActionOrigin_LeftPad_DPadSouth,
        k_EControllerActionOrigin_LeftPad_DPadWest,
        k_EControllerActionOrigin_LeftPad_DPadEast,
        k_EControllerActionOrigin_RightPad_Touch,
        k_EControllerActionOrigin_RightPad_Swipe,
        k_EControllerActionOrigin_RightPad_Click,
        k_EControllerActionOrigin_RightPad_DPadNorth,
        k_EControllerActionOrigin_RightPad_DPadSouth,
        k_EControllerActionOrigin_RightPad_DPadWest,
        k_EControllerActionOrigin_RightPad_DPadEast,
        k_EControllerActionOrigin_LeftTrigger_Pull,
        k_EControllerActionOrigin_LeftTrigger_Click,
        k_EControllerActionOrigin_RightTrigger_Pull,
        k_EControllerActionOrigin_RightTrigger_Click,
        k_EControllerActionOrigin_LeftStick_Move,
        k_EControllerActionOrigin_LeftStick_Click,
        k_EControllerActionOrigin_LeftStick_DPadNorth,
        k_EControllerActionOrigin_LeftStick_DPadSouth,
        k_EControllerActionOrigin_LeftStick_DPadWest,
        k_EControllerActionOrigin_LeftStick_DPadEast,
        k_EControllerActionOrigin_Gyro_Move,
        k_EControllerActionOrigin_Gyro_Pitch,
        k_EControllerActionOrigin_Gyro_Yaw,
        k_EControllerActionOrigin_Gyro_Roll,

        // PS4 Dual Shock
        k_EControllerActionOrigin_PS4_X,
        k_EControllerActionOrigin_PS4_Circle,
        k_EControllerActionOrigin_PS4_Triangle,
        k_EControllerActionOrigin_PS4_Square,
        k_EControllerActionOrigin_PS4_LeftBumper,
        k_EControllerActionOrigin_PS4_RightBumper,
        k_EControllerActionOrigin_PS4_Options,  //Start
        k_EControllerActionOrigin_PS4_Share,    //Back
        k_EControllerActionOrigin_PS4_LeftPad_Touch,
        k_EControllerActionOrigin_PS4_LeftPad_Swipe,
        k_EControllerActionOrigin_PS4_LeftPad_Click,
        k_EControllerActionOrigin_PS4_LeftPad_DPadNorth,
        k_EControllerActionOrigin_PS4_LeftPad_DPadSouth,
        k_EControllerActionOrigin_PS4_LeftPad_DPadWest,
        k_EControllerActionOrigin_PS4_LeftPad_DPadEast,
        k_EControllerActionOrigin_PS4_RightPad_Touch,
        k_EControllerActionOrigin_PS4_RightPad_Swipe,
        k_EControllerActionOrigin_PS4_RightPad_Click,
        k_EControllerActionOrigin_PS4_RightPad_DPadNorth,
        k_EControllerActionOrigin_PS4_RightPad_DPadSouth,
        k_EControllerActionOrigin_PS4_RightPad_DPadWest,
        k_EControllerActionOrigin_PS4_RightPad_DPadEast,
        k_EControllerActionOrigin_PS4_CenterPad_Touch,
        k_EControllerActionOrigin_PS4_CenterPad_Swipe,
        k_EControllerActionOrigin_PS4_CenterPad_Click,
        k_EControllerActionOrigin_PS4_CenterPad_DPadNorth,
        k_EControllerActionOrigin_PS4_CenterPad_DPadSouth,
        k_EControllerActionOrigin_PS4_CenterPad_DPadWest,
        k_EControllerActionOrigin_PS4_CenterPad_DPadEast,
        k_EControllerActionOrigin_PS4_LeftTrigger_Pull,
        k_EControllerActionOrigin_PS4_LeftTrigger_Click,
        k_EControllerActionOrigin_PS4_RightTrigger_Pull,
        k_EControllerActionOrigin_PS4_RightTrigger_Click,
        k_EControllerActionOrigin_PS4_LeftStick_Move,
        k_EControllerActionOrigin_PS4_LeftStick_Click,
        k_EControllerActionOrigin_PS4_LeftStick_DPadNorth,
        k_EControllerActionOrigin_PS4_LeftStick_DPadSouth,
        k_EControllerActionOrigin_PS4_LeftStick_DPadWest,
        k_EControllerActionOrigin_PS4_LeftStick_DPadEast,
        k_EControllerActionOrigin_PS4_RightStick_Move,
        k_EControllerActionOrigin_PS4_RightStick_Click,
        k_EControllerActionOrigin_PS4_RightStick_DPadNorth,
        k_EControllerActionOrigin_PS4_RightStick_DPadSouth,
        k_EControllerActionOrigin_PS4_RightStick_DPadWest,
        k_EControllerActionOrigin_PS4_RightStick_DPadEast,
        k_EControllerActionOrigin_PS4_DPad_North,
        k_EControllerActionOrigin_PS4_DPad_South,
        k_EControllerActionOrigin_PS4_DPad_West,
        k_EControllerActionOrigin_PS4_DPad_East,
        k_EControllerActionOrigin_PS4_Gyro_Move,
        k_EControllerActionOrigin_PS4_Gyro_Pitch,
        k_EControllerActionOrigin_PS4_Gyro_Yaw,
        k_EControllerActionOrigin_PS4_Gyro_Roll,

        // XBox One
        k_EControllerActionOrigin_XBoxOne_A,
        k_EControllerActionOrigin_XBoxOne_B,
        k_EControllerActionOrigin_XBoxOne_X,
        k_EControllerActionOrigin_XBoxOne_Y,
        k_EControllerActionOrigin_XBoxOne_LeftBumper,
        k_EControllerActionOrigin_XBoxOne_RightBumper,
        k_EControllerActionOrigin_XBoxOne_Menu,  //Start
        k_EControllerActionOrigin_XBoxOne_View,  //Back
        k_EControllerActionOrigin_XBoxOne_LeftTrigger_Pull,
        k_EControllerActionOrigin_XBoxOne_LeftTrigger_Click,
        k_EControllerActionOrigin_XBoxOne_RightTrigger_Pull,
        k_EControllerActionOrigin_XBoxOne_RightTrigger_Click,
        k_EControllerActionOrigin_XBoxOne_LeftStick_Move,
        k_EControllerActionOrigin_XBoxOne_LeftStick_Click,
        k_EControllerActionOrigin_XBoxOne_LeftStick_DPadNorth,
        k_EControllerActionOrigin_XBoxOne_LeftStick_DPadSouth,
        k_EControllerActionOrigin_XBoxOne_LeftStick_DPadWest,
        k_EControllerActionOrigin_XBoxOne_LeftStick_DPadEast,
        k_EControllerActionOrigin_XBoxOne_RightStick_Move,
        k_EControllerActionOrigin_XBoxOne_RightStick_Click,
        k_EControllerActionOrigin_XBoxOne_RightStick_DPadNorth,
        k_EControllerActionOrigin_XBoxOne_RightStick_DPadSouth,
        k_EControllerActionOrigin_XBoxOne_RightStick_DPadWest,
        k_EControllerActionOrigin_XBoxOne_RightStick_DPadEast,
        k_EControllerActionOrigin_XBoxOne_DPad_North,
        k_EControllerActionOrigin_XBoxOne_DPad_South,
        k_EControllerActionOrigin_XBoxOne_DPad_West,
        k_EControllerActionOrigin_XBoxOne_DPad_East,

        // XBox 360
        k_EControllerActionOrigin_XBox360_A,
        k_EControllerActionOrigin_XBox360_B,
        k_EControllerActionOrigin_XBox360_X,
        k_EControllerActionOrigin_XBox360_Y,
        k_EControllerActionOrigin_XBox360_LeftBumper,
        k_EControllerActionOrigin_XBox360_RightBumper,
        k_EControllerActionOrigin_XBox360_Start,  //Start
        k_EControllerActionOrigin_XBox360_Back,  //Back
        k_EControllerActionOrigin_XBox360_LeftTrigger_Pull,
        k_EControllerActionOrigin_XBox360_LeftTrigger_Click,
        k_EControllerActionOrigin_XBox360_RightTrigger_Pull,
        k_EControllerActionOrigin_XBox360_RightTrigger_Click,
        k_EControllerActionOrigin_XBox360_LeftStick_Move,
        k_EControllerActionOrigin_XBox360_LeftStick_Click,
        k_EControllerActionOrigin_XBox360_LeftStick_DPadNorth,
        k_EControllerActionOrigin_XBox360_LeftStick_DPadSouth,
        k_EControllerActionOrigin_XBox360_LeftStick_DPadWest,
        k_EControllerActionOrigin_XBox360_LeftStick_DPadEast,
        k_EControllerActionOrigin_XBox360_RightStick_Move,
        k_EControllerActionOrigin_XBox360_RightStick_Click,
        k_EControllerActionOrigin_XBox360_RightStick_DPadNorth,
        k_EControllerActionOrigin_XBox360_RightStick_DPadSouth,
        k_EControllerActionOrigin_XBox360_RightStick_DPadWest,
        k_EControllerActionOrigin_XBox360_RightStick_DPadEast,
        k_EControllerActionOrigin_XBox360_DPad_North,
        k_EControllerActionOrigin_XBox360_DPad_South,
        k_EControllerActionOrigin_XBox360_DPad_West,
        k_EControllerActionOrigin_XBox360_DPad_East,

        // SteamController V2
        k_EControllerActionOrigin_SteamV2_A,
        k_EControllerActionOrigin_SteamV2_B,
        k_EControllerActionOrigin_SteamV2_X,
        k_EControllerActionOrigin_SteamV2_Y,
        k_EControllerActionOrigin_SteamV2_LeftBumper,
        k_EControllerActionOrigin_SteamV2_RightBumper,
        k_EControllerActionOrigin_SteamV2_LeftGrip_Lower,
        k_EControllerActionOrigin_SteamV2_LeftGrip_Upper,
        k_EControllerActionOrigin_SteamV2_RightGrip_Lower,
        k_EControllerActionOrigin_SteamV2_RightGrip_Upper,
        k_EControllerActionOrigin_SteamV2_LeftBumper_Pressure,
        k_EControllerActionOrigin_SteamV2_RightBumper_Pressure,
        k_EControllerActionOrigin_SteamV2_LeftGrip_Pressure,
        k_EControllerActionOrigin_SteamV2_RightGrip_Pressure,
        k_EControllerActionOrigin_SteamV2_LeftGrip_Upper_Pressure,
        k_EControllerActionOrigin_SteamV2_RightGrip_Upper_Pressure,
        k_EControllerActionOrigin_SteamV2_Start,
        k_EControllerActionOrigin_SteamV2_Back,
        k_EControllerActionOrigin_SteamV2_LeftPad_Touch,
        k_EControllerActionOrigin_SteamV2_LeftPad_Swipe,
        k_EControllerActionOrigin_SteamV2_LeftPad_Click,
        k_EControllerActionOrigin_SteamV2_LeftPad_Pressure,
        k_EControllerActionOrigin_SteamV2_LeftPad_DPadNorth,
        k_EControllerActionOrigin_SteamV2_LeftPad_DPadSouth,
        k_EControllerActionOrigin_SteamV2_LeftPad_DPadWest,
        k_EControllerActionOrigin_SteamV2_LeftPad_DPadEast,
        k_EControllerActionOrigin_SteamV2_RightPad_Touch,
        k_EControllerActionOrigin_SteamV2_RightPad_Swipe,
        k_EControllerActionOrigin_SteamV2_RightPad_Click,
        k_EControllerActionOrigin_SteamV2_RightPad_Pressure,
        k_EControllerActionOrigin_SteamV2_RightPad_DPadNorth,
        k_EControllerActionOrigin_SteamV2_RightPad_DPadSouth,
        k_EControllerActionOrigin_SteamV2_RightPad_DPadWest,
        k_EControllerActionOrigin_SteamV2_RightPad_DPadEast,
        k_EControllerActionOrigin_SteamV2_LeftTrigger_Pull,
        k_EControllerActionOrigin_SteamV2_LeftTrigger_Click,
        k_EControllerActionOrigin_SteamV2_RightTrigger_Pull,
        k_EControllerActionOrigin_SteamV2_RightTrigger_Click,
        k_EControllerActionOrigin_SteamV2_LeftStick_Move,
        k_EControllerActionOrigin_SteamV2_LeftStick_Click,
        k_EControllerActionOrigin_SteamV2_LeftStick_DPadNorth,
        k_EControllerActionOrigin_SteamV2_LeftStick_DPadSouth,
        k_EControllerActionOrigin_SteamV2_LeftStick_DPadWest,
        k_EControllerActionOrigin_SteamV2_LeftStick_DPadEast,
        k_EControllerActionOrigin_SteamV2_Gyro_Move,
        k_EControllerActionOrigin_SteamV2_Gyro_Pitch,
        k_EControllerActionOrigin_SteamV2_Gyro_Yaw,
        k_EControllerActionOrigin_SteamV2_Gyro_Roll,

        // Switch - Pro or Joycons used as a single input device.
        // This does not apply to a single joycon
        k_EControllerActionOrigin_Switch_A,
        k_EControllerActionOrigin_Switch_B,
        k_EControllerActionOrigin_Switch_X,
        k_EControllerActionOrigin_Switch_Y,
        k_EControllerActionOrigin_Switch_LeftBumper,
        k_EControllerActionOrigin_Switch_RightBumper,
        k_EControllerActionOrigin_Switch_Plus,  //Start
        k_EControllerActionOrigin_Switch_Minus, //Back
        k_EControllerActionOrigin_Switch_Capture,
        k_EControllerActionOrigin_Switch_LeftTrigger_Pull,
        k_EControllerActionOrigin_Switch_LeftTrigger_Click,
        k_EControllerActionOrigin_Switch_RightTrigger_Pull,
        k_EControllerActionOrigin_Switch_RightTrigger_Click,
        k_EControllerActionOrigin_Switch_LeftStick_Move,
        k_EControllerActionOrigin_Switch_LeftStick_Click,
        k_EControllerActionOrigin_Switch_LeftStick_DPadNorth,
        k_EControllerActionOrigin_Switch_LeftStick_DPadSouth,
        k_EControllerActionOrigin_Switch_LeftStick_DPadWest,
        k_EControllerActionOrigin_Switch_LeftStick_DPadEast,
        k_EControllerActionOrigin_Switch_RightStick_Move,
        k_EControllerActionOrigin_Switch_RightStick_Click,
        k_EControllerActionOrigin_Switch_RightStick_DPadNorth,
        k_EControllerActionOrigin_Switch_RightStick_DPadSouth,
        k_EControllerActionOrigin_Switch_RightStick_DPadWest,
        k_EControllerActionOrigin_Switch_RightStick_DPadEast,
        k_EControllerActionOrigin_Switch_DPad_North,
        k_EControllerActionOrigin_Switch_DPad_South,
        k_EControllerActionOrigin_Switch_DPad_West,
        k_EControllerActionOrigin_Switch_DPad_East,
        k_EControllerActionOrigin_Switch_ProGyro_Move,  // Primary Gyro in Pro Controller, or Right JoyCon
        k_EControllerActionOrigin_Switch_ProGyro_Pitch,  // Primary Gyro in Pro Controller, or Right JoyCon
        k_EControllerActionOrigin_Switch_ProGyro_Yaw,  // Primary Gyro in Pro Controller, or Right JoyCon
        k_EControllerActionOrigin_Switch_ProGyro_Roll,  // Primary Gyro in Pro Controller, or Right JoyCon
                                                        // Switch JoyCon Specific
        k_EControllerActionOrigin_Switch_RightGyro_Move,  // Right JoyCon Gyro generally should correspond to Pro's single gyro
        k_EControllerActionOrigin_Switch_RightGyro_Pitch,  // Right JoyCon Gyro generally should correspond to Pro's single gyro
        k_EControllerActionOrigin_Switch_RightGyro_Yaw,  // Right JoyCon Gyro generally should correspond to Pro's single gyro
        k_EControllerActionOrigin_Switch_RightGyro_Roll,  // Right JoyCon Gyro generally should correspond to Pro's single gyro
        k_EControllerActionOrigin_Switch_LeftGyro_Move,
        k_EControllerActionOrigin_Switch_LeftGyro_Pitch,
        k_EControllerActionOrigin_Switch_LeftGyro_Yaw,
        k_EControllerActionOrigin_Switch_LeftGyro_Roll,
        k_EControllerActionOrigin_Switch_LeftGrip_Lower, // Left JoyCon SR Button
        k_EControllerActionOrigin_Switch_LeftGrip_Upper, // Left JoyCon SL Button
        k_EControllerActionOrigin_Switch_RightGrip_Lower,  // Right JoyCon SL Button
        k_EControllerActionOrigin_Switch_RightGrip_Upper,  // Right JoyCon SR Button

        // Added in SDK 1.45
        k_EControllerActionOrigin_PS4_DPad_Move,
        k_EControllerActionOrigin_XBoxOne_DPad_Move,
        k_EControllerActionOrigin_XBox360_DPad_Move,
        k_EControllerActionOrigin_Switch_DPad_Move,

        k_EControllerActionOrigin_Count, // If Steam has added support for new controllers origins will go here.
        k_EControllerActionOrigin_MaximumPossibleValue = 32767, // Origins are currently a maximum of 16 bits.
    };
    public enum EXboxOrigin
    {
        k_EXboxOrigin_A,
        k_EXboxOrigin_B,
        k_EXboxOrigin_X,
        k_EXboxOrigin_Y,
        k_EXboxOrigin_LeftBumper,
        k_EXboxOrigin_RightBumper,
        k_EXboxOrigin_Menu,  //Start
        k_EXboxOrigin_View,  //Back
        k_EXboxOrigin_LeftTrigger_Pull,
        k_EXboxOrigin_LeftTrigger_Click,
        k_EXboxOrigin_RightTrigger_Pull,
        k_EXboxOrigin_RightTrigger_Click,
        k_EXboxOrigin_LeftStick_Move,
        k_EXboxOrigin_LeftStick_Click,
        k_EXboxOrigin_LeftStick_DPadNorth,
        k_EXboxOrigin_LeftStick_DPadSouth,
        k_EXboxOrigin_LeftStick_DPadWest,
        k_EXboxOrigin_LeftStick_DPadEast,
        k_EXboxOrigin_RightStick_Move,
        k_EXboxOrigin_RightStick_Click,
        k_EXboxOrigin_RightStick_DPadNorth,
        k_EXboxOrigin_RightStick_DPadSouth,
        k_EXboxOrigin_RightStick_DPadWest,
        k_EXboxOrigin_RightStick_DPadEast,
        k_EXboxOrigin_DPad_North,
        k_EXboxOrigin_DPad_South,
        k_EXboxOrigin_DPad_West,
        k_EXboxOrigin_DPad_East,
    };
    public enum ESteamInputType
    {
        k_ESteamInputType_Unknown,
        k_ESteamInputType_SteamController,
        k_ESteamInputType_XBox360Controller,
        k_ESteamInputType_XBoxOneController,
        k_ESteamInputType_GenericGamepad,       // DirectInput controllers
        k_ESteamInputType_PS4Controller,
        k_ESteamInputType_AppleMFiController,   // Unused
        k_ESteamInputType_AndroidController,    // Unused
        k_ESteamInputType_SwitchJoyConPair,     // Unused
        k_ESteamInputType_SwitchJoyConSingle,   // Unused
        k_ESteamInputType_SwitchProController,
        k_ESteamInputType_MobileTouch,          // Steam Link App On-screen Virtual Controller
        k_ESteamInputType_PS3Controller,        // Currently uses PS4 Origins
        k_ESteamInputType_Count,
        k_ESteamInputType_MaximumPossibleValue = 255,
    };
    public enum GameSearchErrorCode_t : int
    {
        OK = 1,
        Failed_Search_Already_In_Progress = 2,
        Failed_No_Search_In_Progress = 3,
        Failed_Not_Lobby_Leader = 4,
        Failed_No_Host_Available = 5,
        Failed_Search_Params_Invalid = 6,
        Failed_Offline = 7,
        Failed_NotAuthorized = 8,
        Failed_Unknown_Error = 9,
    }
    public enum PlayerResult_t : int
    {
        FailedToConnect = 1,
        Abandoned = 2,
        Kicked = 3,
        Incomplete = 4,
        Completed = 5,
    }

    public enum ESteamNetworkingAvailability
    {
        // Negative values indicate a problem.
        //
        // In general, we will not automatically retry unless you take some action that
        // depends on of requests this resource, such as querying the status, attempting
        // to initiate a connection, receive a connection, etc.  If you do not take any
        // action at all, we do not automatically retry in the background.
        k_ESteamNetworkingAvailability_CannotTry = -102,        // A dependent resource is missing, so this service is unavailable.  (E.g. we cannot talk to routers because Internet is down or we don't have the network config.)
        k_ESteamNetworkingAvailability_Failed = -101,           // We have tried for enough time that we would expect to have been successful by now.  We have never been successful
        k_ESteamNetworkingAvailability_Previously = -100,       // We tried and were successful at one time, but now it looks like we have a problem

        k_ESteamNetworkingAvailability_Retrying = -10,      // We previously failed and are currently retrying

        // Not a problem, but not ready either
        k_ESteamNetworkingAvailability_NeverTried = 1,      // We don't know because we haven't ever checked/tried
        k_ESteamNetworkingAvailability_Waiting = 2,         // We're waiting on a dependent resource to be acquired.  (E.g. we cannot obtain a cert until we are logged into Steam.  We cannot measure latency to relays until we have the network config.)
        k_ESteamNetworkingAvailability_Attempting = 3,      // We're actively trying now, but are not yet successful.

        k_ESteamNetworkingAvailability_Current = 100,           // Resource is online/available


        k_ESteamNetworkingAvailability_Unknown = 0,         // Internal dummy/sentinel, or value is not applicable in this context
        k_ESteamNetworkingAvailability__Force32bit = 0x7fffffff,
    };

    /// Different methods of describing the identity of a network host
    public enum EIntPtrType
    {
        // Dummy/empty/invalid.
        // Plese note that if we parse a string that we don't recognize
        // but that appears reasonable, we will NOT use this type.  Instead
        // we'll use k_EIntPtrType_UnknownType.
        k_EIntPtrType_Invalid = 0,

        //
        // Basic platform-specific identifiers.
        //
        k_EIntPtrType_SteamID = 16, // 64-bit CSteamID

        //
        // Special identifiers.
        //

        // Use their IP address (and port) as their "identity".
        // These types of identities are always unauthenticated.
        // They are useful for porting plain sockets code, and other
        // situations where you don't care about authentication.  In this
        // case, the local identity will be "localhost",
        // and the remote address will be their network address.
        //
        // We use the same type for either IPv4 or IPv6, and
        // the address is always store as IPv6.  We use IPv4
        // mapped addresses to handle IPv4.
        k_EIntPtrType_IPAddress = 1,

        // Generic string/binary blobs.  It's up to your app to interpret this.
        // This library can tell you if the remote host presented a certificate
        // signed by somebody you have chosen to trust, with this identity on it.
        // It's up to you to ultimately decide what this identity means.
        k_EIntPtrType_GenericString = 2,
        k_EIntPtrType_GenericBytes = 3,

        // This identity type is used when we parse a string that looks like is a
        // valid identity, just of a kind that we don't recognize.  In this case, we
        // can often still communicate with the peer!  Allowing such identities
        // for types we do not recognize useful is very useful for forward
        // compatibility.
        k_EIntPtrType_UnknownType = 4,

        // Make sure this enum is stored in an int.
        k_EIntPtrType__Force32bit = 0x7fffffff,
    };

    /// Detail level for diagnostic output callback.
    /// See ISteamNetworkingUtils::SetDebugOutputFunction
    /// ISteamNetworkingUtils().
    public enum ESteamNetworkingSocketsDebugOutputType
    {
        k_ESteamNetworkingSocketsDebugOutputType_None = 0,
        k_ESteamNetworkingSocketsDebugOutputType_Bug = 1, // You used the API incorrectly, or an internal error happened
        k_ESteamNetworkingSocketsDebugOutputType_Error = 2, // Run-time error condition that isn't the result of a bug.  (E.g. we are offline, cannot bind a port, etc)
        k_ESteamNetworkingSocketsDebugOutputType_Important = 3, // Nothing is wrong, but this is an important notification
        k_ESteamNetworkingSocketsDebugOutputType_Warning = 4,
        k_ESteamNetworkingSocketsDebugOutputType_Msg = 5, // Recommended amount
        k_ESteamNetworkingSocketsDebugOutputType_Verbose = 6, // Quite a bit
        k_ESteamNetworkingSocketsDebugOutputType_Debug = 7, // Practically everything
        k_ESteamNetworkingSocketsDebugOutputType_Everything = 8, // Wall of text, detailed packet contents breakdown, etc

        k_ESteamNetworkingSocketsDebugOutputType__Force32Bit = 0x7fffffff
    };

    /// Return value of ISteamNetworkintgUtils::GetConfigValue
    public enum ESteamNetworkingGetConfigValueResult
    {
        k_ESteamNetworkingGetConfigValue_BadValue = -1, // No such configuration value
        k_ESteamNetworkingGetConfigValue_BadScopeObj = -2,  // Bad connection handle, etc
        k_ESteamNetworkingGetConfigValue_BufferTooSmall = -3, // Couldn't fit the result in your buffer
        k_ESteamNetworkingGetConfigValue_OK = 1,
        k_ESteamNetworkingGetConfigValue_OKInherited = 2, // A value was not set at this level, but the effective (inherited) value was returned.

        k_ESteamNetworkingGetConfigValueResult__Force32Bit = 0x7fffffff
    };

    public enum ESteamPartyBeaconLocationData : int
    {
        k_ESteamPartyBeaconLocationDataInvalid = 0,
        k_ESteamPartyBeaconLocationDataName = 1,
        k_ESteamPartyBeaconLocationDataIconURLSmall = 2,
        k_ESteamPartyBeaconLocationDataIconURLMedium = 3,
        k_ESteamPartyBeaconLocationDataIconURLLarge = 4,
    };

    //-----------------------------------------------------------------------------
    // Purpose: The form factor of a device
    //-----------------------------------------------------------------------------
    public enum ESteamDeviceFormFactor : int
    {
        k_ESteamDeviceFormFactorUnknown = 0,
        k_ESteamDeviceFormFactorPhone = 1,
        k_ESteamDeviceFormFactorTablet = 2,
        k_ESteamDeviceFormFactorComputer = 3,
        k_ESteamDeviceFormFactorTV = 4,
    };

    public enum EVRScreenshotType : int
    {
        k_EVRScreenshotType_None = 0,
        k_EVRScreenshotType_Mono = 1,
        k_EVRScreenshotType_Stereo = 2,
        k_EVRScreenshotType_MonoCubemap = 3,
        k_EVRScreenshotType_MonoPanorama = 4,
        k_EVRScreenshotType_StereoPanorama = 5
    };

    public enum EItemPreviewType : int
    {
        k_EItemPreviewType_Image = 0,  // standard image file expected (e.g. jpg, png, gif, etc.)
        k_EItemPreviewType_YouTubeVideo = 1,   // video id is stored
        k_EItemPreviewType_Sketchfab = 2,  // model id is stored
        k_EItemPreviewType_EnvironmentMap_HorizontalCross = 3, // standard image file expected - cube map in the layout
                                                               // +---+---+-------+
                                                               // |   |Up |       |
                                                               // +---+---+---+---+
                                                               // | L | F | R | B |
                                                               // +---+---+---+---+
                                                               // |   |Dn |       |
                                                               // +---+---+---+---+
        k_EItemPreviewType_EnvironmentMap_LatLong = 4, // standard image file expected
        k_EItemPreviewType_ReservedMax = 255,  // you can specify your own types above this value
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

    // For the above transport protocol, what do we think the local machine's connectivity to the internet over ipv6 is like
    public enum ESteamIPv6ConnectivityState : int
    {
        k_ESteamIPv6ConnectivityState_Unknown = 0,  // We haven't run a test yet
        k_ESteamIPv6ConnectivityState_Good = 1,     // We have recently been able to make a request on ipv6 for the given protocol
        k_ESteamIPv6ConnectivityState_Bad = 2,      // We failed to make a request, either because this machine has no ipv6 address assigned, or it has no upstream connectivity
    };

    public enum ESteamIPv6ConnectivityProtocol : int
    {
        k_ESteamIPv6ConnectivityProtocol_Invalid = 0,
        k_ESteamIPv6ConnectivityProtocol_HTTP = 1,      // because a proxy may make this different than other protocols
        k_ESteamIPv6ConnectivityProtocol_UDP = 2,       // test UDP connectivity. Uses a port that is commonly needed for other Steam stuff. If UDP works, TCP probably works. 
    };

    //
    // ESteamIPType
    //
    public enum SteamIPType : int
    {
        Type4 = 0,
        Type6 = 1,
    }

    //
    // EResult
    //
    public enum Result : int
    {
        None = 0,
        OK = 1,
        Fail = 2,
        NoConnection = 3,
        InvalidPassword = 5,
        LoggedInElsewhere = 6,
        InvalidProtocolVer = 7,
        InvalidParam = 8,
        FileNotFound = 9,
        Busy = 10,
        InvalidState = 11,
        InvalidName = 12,
        InvalidEmail = 13,
        DuplicateName = 14,
        AccessDenied = 15,
        Timeout = 16,
        Banned = 17,
        AccountNotFound = 18,
        InvalidSteamID = 19,
        ServiceUnavailable = 20,
        NotLoggedOn = 21,
        Pending = 22,
        EncryptionFailure = 23,
        InsufficientPrivilege = 24,
        LimitExceeded = 25,
        Revoked = 26,
        Expired = 27,
        AlreadyRedeemed = 28,
        DuplicateRequest = 29,
        AlreadyOwned = 30,
        IPNotFound = 31,
        PersistFailed = 32,
        LockingFailed = 33,
        LogonSessionReplaced = 34,
        ConnectFailed = 35,
        HandshakeFailed = 36,
        IOFailure = 37,
        RemoteDisconnect = 38,
        ShoppingCartNotFound = 39,
        Blocked = 40,
        Ignored = 41,
        NoMatch = 42,
        AccountDisabled = 43,
        ServiceReadOnly = 44,
        AccountNotFeatured = 45,
        AdministratorOK = 46,
        ContentVersion = 47,
        TryAnotherCM = 48,
        PasswordRequiredToKickSession = 49,
        AlreadyLoggedInElsewhere = 50,
        Suspended = 51,
        Cancelled = 52,
        DataCorruption = 53,
        DiskFull = 54,
        RemoteCallFailed = 55,
        PasswordUnset = 56,
        ExternalAccountUnlinked = 57,
        PSNTicketInvalid = 58,
        ExternalAccountAlreadyLinked = 59,
        RemoteFileConflict = 60,
        IllegalPassword = 61,
        SameAsPreviousValue = 62,
        AccountLogonDenied = 63,
        CannotUseOldPassword = 64,
        InvalidLoginAuthCode = 65,
        AccountLogonDeniedNoMail = 66,
        HardwareNotCapableOfIPT = 67,
        IPTInitError = 68,
        ParentalControlRestricted = 69,
        FacebookQueryError = 70,
        ExpiredLoginAuthCode = 71,
        IPLoginRestrictionFailed = 72,
        AccountLockedDown = 73,
        AccountLogonDeniedVerifiedEmailRequired = 74,
        NoMatchingURL = 75,
        BadResponse = 76,
        RequirePasswordReEntry = 77,
        ValueOutOfRange = 78,
        UnexpectedError = 79,
        Disabled = 80,
        InvalidCEGSubmission = 81,
        RestrictedDevice = 82,
        RegionLocked = 83,
        RateLimitExceeded = 84,
        AccountLoginDeniedNeedTwoFactor = 85,
        ItemDeleted = 86,
        AccountLoginDeniedThrottle = 87,
        TwoFactorCodeMismatch = 88,
        TwoFactorActivationCodeMismatch = 89,
        AccountAssociatedToMultiplePartners = 90,
        NotModified = 91,
        NoMobileDevice = 92,
        TimeNotSynced = 93,
        SmsCodeFailed = 94,
        AccountLimitExceeded = 95,
        AccountActivityLimitExceeded = 96,
        PhoneActivityLimitExceeded = 97,
        RefundToWallet = 98,
        EmailSendFailure = 99,
        NotSettled = 100,
        NeedCaptcha = 101,
        GSLTDenied = 102,
        GSOwnerDenied = 103,
        InvalidItemType = 104,
        IPBanned = 105,
        GSLTExpired = 106,
        InsufficientFunds = 107,
        TooManyPending = 108,
        NoSiteLicensesFound = 109,
        WGNetworkSendExceeded = 110,
        AccountNotFriends = 111,
        LimitedUserAccount = 112,
        CantRemoveItem = 113,
        AccountDeleted = 114,
        ExistingUserCancelledLicense = 115,
        CommunityCooldown = 116,
        NoLauncherSpecified = 117,
        MustAgreeToSSA = 118,
        LauncherMigrated = 119,
        SteamRealmMismatch = 120,
        InvalidSignature = 121,
        ParseFailure = 122,
        NoVerifiedPhone = 123,
    }

    //
    // EDenyReason
    //
    public enum DenyReason : int
    {
        Invalid = 0,
        InvalidVersion = 1,
        Generic = 2,
        NotLoggedOn = 3,
        NoLicense = 4,
        Cheater = 5,
        LoggedInElseWhere = 6,
        UnknownText = 7,
        IncompatibleAnticheat = 8,
        MemoryCorruption = 9,
        IncompatibleSoftware = 10,
        SteamConnectionLost = 11,
        SteamConnectionError = 12,
        SteamResponseTimedOut = 13,
        SteamValidationStalled = 14,
        SteamOwnerLeftGuestUser = 15,
    }

    //
    // EAuthSessionResponse
    //
    public enum AuthResponse : int
    {
        OK = 0,
        UserNotConnectedToSteam = 1,
        NoLicenseOrExpired = 2,
        VACBanned = 3,
        LoggedInElseWhere = 4,
        VACCheckTimedOut = 5,
        AuthTicketCanceled = 6,
        AuthTicketInvalidAlreadyUsed = 7,
        AuthTicketInvalid = 8,
        PublisherIssuedBan = 9,
    }

    //
    // EAccountType
    //
    public enum AccountType : int
    {
        Invalid = 0,
        Individual = 1,
        Multiseat = 2,
        GameServer = 3,
        AnonGameServer = 4,
        Pending = 5,
        ContentServer = 6,
        Clan = 7,
        Chat = 8,
        ConsoleUser = 9,
        AnonUser = 10,
        Max = 11,
    }

    //
    // EChatRoomEnterResponse
    //
    public enum RoomEnter : int
    {
        Success = 1,
        DoesntExist = 2,
        NotAllowed = 3,
        Full = 4,
        Error = 5,
        Banned = 6,
        Limited = 7,
        ClanDisabled = 8,
        CommunityBan = 9,
        MemberBlockedYou = 10,
        YouBlockedMember = 11,
        RatelimitExceeded = 15,
    }

    //
    // EChatSteamIDInstanceFlags
    //
    public enum ChatSteamIDInstanceFlags : int
    {
        AccountInstanceMask = 4095,
        InstanceFlagClan = 524288,
        InstanceFlagLobby = 262144,
        InstanceFlagMMSLobby = 131072,
    }

    //
    // EBroadcastUploadResult
    //
    public enum BroadcastUploadResult : int
    {
        None = 0,
        OK = 1,
        InitFailed = 2,
        FrameFailed = 3,
        Timeout = 4,
        BandwidthExceeded = 5,
        LowFPS = 6,
        MissingKeyFrames = 7,
        NoConnection = 8,
        RelayFailed = 9,
        SettingsChanged = 10,
        MissingAudio = 11,
        TooFarBehind = 12,
        TranscodeBehind = 13,
        NotAllowedToPlay = 14,
        Busy = 15,
        Banned = 16,
        AlreadyActive = 17,
        ForcedOff = 18,
        AudioBehind = 19,
        Shutdown = 20,
        Disconnect = 21,
        VideoInitFailed = 22,
        AudioInitFailed = 23,
    }

    //
    // EMarketNotAllowedReasonFlags
    //
    public enum MarketNotAllowedReasonFlags : int
    {
        None = 0,
        TemporaryFailure = 1,
        AccountDisabled = 2,
        AccountLockedDown = 4,
        AccountLimited = 8,
        TradeBanned = 16,
        AccountNotTrusted = 32,
        SteamGuardNotEnabled = 64,
        SteamGuardOnlyRecentlyEnabled = 128,
        RecentPasswordReset = 256,
        NewPaymentMethod = 512,
        InvalidCookie = 1024,
        UsingNewDevice = 2048,
        RecentSelfRefund = 4096,
        NewPaymentMethodCannotBeVerified = 8192,
        NoRecentPurchases = 16384,
        AcceptedWalletGift = 32768,
    }

    //
    // EDurationControlProgress
    //
    public enum DurationControlProgress : int
    {
        Progress_Full = 0,
        Progress_Half = 1,
        Progress_None = 2,
        ExitSoon_3h = 3,
        ExitSoon_5h = 4,
        ExitSoon_Night = 5,
    }

    //
    // EDurationControlNotification
    //
    public enum DurationControlNotification : int
    {
        None = 0,
        DurationControlNotification1Hour = 1,
        DurationControlNotification3Hours = 2,
        HalfProgress = 3,
        NoProgress = 4,
        ExitSoon_3h = 5,
        ExitSoon_5h = 6,
        ExitSoon_Night = 7,
    }

    //
    // EDurationControlOnlineState
    //
    public enum DurationControlOnlineState : int
    {
        Invalid = 0,
        Offline = 1,
        Online = 2,
        OnlineHighPri = 3,
    }

    //
    // ESteamIPv6ConnectivityProtocol
    //
    public enum SteamIPv6ConnectivityProtocol : int
    {
        Invalid = 0,
        HTTP = 1,
        UDP = 2,
    }

    //
    // ESteamIPv6ConnectivityState
    //
    public enum SteamIPv6ConnectivityState : int
    {
        Unknown = 0,
        Good = 1,
        Bad = 2,
    }

       
    //
    // EUserRestriction
    //
    public enum UserRestriction : int
    {
        None = 0,
        Unknown = 1,
        AnyChat = 2,
        VoiceChat = 4,
        GroupChat = 8,
        Rating = 16,
        GameInvites = 32,
        Trading = 64,
    }

    //
    // EPersonaChange
    //
    public enum PersonaChange : int
    {
        Name = 1,
        Status = 2,
        ComeOnline = 4,
        GoneOffline = 8,
        GamePlayed = 16,
        GameServer = 32,
        Avatar = 64,
        JoinedSource = 128,
        LeftSource = 256,
        RelationshipChanged = 512,
        NameFirstSet = 1024,
        Broadcast = 2048,
        Nickname = 4096,
        SteamLevel = 8192,
        RichPresence = 16384,
    }

    //
    // EFloatingGamepadTextInputMode
    //
    public enum TextInputMode : int
    {
        SingleLine = 0,
        MultipleLines = 1,
        Email = 2,
        Numeric = 3,
    }

    //
    // ETextFilteringContext
    //
    public enum TextFilteringContext : int
    {
        Unknown = 0,
        GameContent = 1,
        Chat = 2,
        Name = 3,
    }

    //
    // ECheckFileSignature
    //
    public enum CheckFileSignature : int
    {
        InvalidSignature = 0,
        ValidSignature = 1,
        FileNotFound = 2,
        NoSignaturesFoundForThisApp = 3,
        NoSignaturesFoundForThisFile = 4,
    }

    //
    // EMatchMakingServerResponse
    //
    public enum MatchMakingServerResponse : int
    {
        ServerResponded = 0,
        ServerFailedToRespond = 1,
        NoServersListedOnMasterServer = 2,
    }

    //
    // EChatMemberStateChange
    //
    public enum ChatMemberStateChange : int
    {
        Entered = 1,
        Left = 2,
        Disconnected = 4,
        Kicked = 8,
        Banned = 16,
    }

    //
    // ESteamPartyBeaconLocationType
    //
    public enum SteamPartyBeaconLocationType : int
    {
        Invalid = 0,
        ChatGroup = 1,
        Max = 2,
    }

    //
    // ESteamPartyBeaconLocationData
    //
    public enum SteamPartyBeaconLocationData : int
    {
        Invalid = 0,
        Name = 1,
        IconURLSmall = 2,
        IconURLMedium = 3,
        IconURLLarge = 4,
    }
    
    //
    // EWorkshopVote
    //
    public enum WorkshopVote : int
    {
        Unvoted = 0,
        For = 1,
        Against = 2,
        Later = 3,
    }

    //
    // EUGCReadAction
    //
    public enum UGCReadAction : int
    {
        ontinueReadingUntilFinished = 0,
        ontinueReading = 1,
        lose = 2,
    }

    //
    // ERemoteStorageLocalFileChange
    //
    public enum RemoteStorageLocalFileChange : int
    {
        Invalid = 0,
        FileUpdated = 1,
        FileDeleted = 2,
    }

    //
    // ERemoteStorageFilePathType
    //
    public enum RemoteStorageFilePathType : int
    {
        Invalid = 0,
        Absolute = 1,
        APIFilename = 2,
    }

    //
    // ELeaderboardDataRequest
    //
    public enum LeaderboardDataRequest : int
    {
        Global = 0,
        GlobalAroundUser = 1,
        Friends = 2,
        Users = 3,
    }

    //
    // ERegisterActivationCodeResult
    //
    public enum RegisterActivationCodeResult : int
    {
        ResultOK = 0,
        ResultFail = 1,
        ResultAlreadyRegistered = 2,
        ResultTimeout = 3,
        AlreadyOwned = 4,
    }

    //
    // EP2PSessionError
    //
    public enum P2PSessionError : int
    {
        None = 0,
        NoRightsToApp = 2,
        Timeout = 4,
        NotRunningApp_DELETED = 1,
        DestinationNotLoggedIn_DELETED = 3,
        Max = 5,
    }

    //
    // ESNetSocketState
    //
    //
    // ESNetSocketConnectionType
    //
    //
    // EVRScreenshotType
    //
    public enum VRScreenshotType : int
    {
        None = 0,
        Mono = 1,
        Stereo = 2,
        MonoCubemap = 3,
        MonoPanorama = 4,
        StereoPanorama = 5,
    }

    //
    // EHTTPMethod
    //
    public enum HTTPMethod : int
    {
        Invalid = 0,
        GET = 1,
        HEAD = 2,
        POST = 3,
        PUT = 4,
        DELETE = 5,
        OPTIONS = 6,
        PATCH = 7,
    }

    //
    // EHTTPStatusCode
    //
    public enum HTTPStatusCode : int
    {
        Invalid = 0,
        Code100Continue = 100,
        Code101SwitchingProtocols = 101,
        Code200OK = 200,
        Code201Created = 201,
        Code202Accepted = 202,
        Code203NonAuthoritative = 203,
        Code204NoContent = 204,
        Code205ResetContent = 205,
        Code206PartialContent = 206,
        Code300MultipleChoices = 300,
        Code301MovedPermanently = 301,
        Code302Found = 302,
        Code303SeeOther = 303,
        Code304NotModified = 304,
        Code305UseProxy = 305,
        Code307TemporaryRedirect = 307,
        Code400BadRequest = 400,
        Code401Unauthorized = 401,
        Code402PaymentRequired = 402,
        Code403Forbidden = 403,
        Code404NotFound = 404,
        Code405MethodNotAllowed = 405,
        Code406NotAcceptable = 406,
        Code407ProxyAuthRequired = 407,
        Code408RequestTimeout = 408,
        Code409Conflict = 409,
        Code410Gone = 410,
        Code411LengthRequired = 411,
        Code412PreconditionFailed = 412,
        Code413RequestEntityTooLarge = 413,
        Code414RequestURITooLong = 414,
        Code415UnsupportedMediaType = 415,
        Code416RequestedRangeNotSatisfiable = 416,
        Code417ExpectationFailed = 417,
        Code4xxUnknown = 418,
        Code429TooManyRequests = 429,
        Code444ConnectionClosed = 444,
        Code500publicServerError = 500,
        Code501NotImplemented = 501,
        Code502BadGateway = 502,
        Code503ServiceUnavailable = 503,
        Code504GatewayTimeout = 504,
        Code505HTTPVersionNotSupported = 505,
        Code5xxUnknown = 599,
    }

    //
    // EInputSourceMode
    //
    public enum InputSourceMode : int
    {
        None = 0,
        Dpad = 1,
        Buttons = 2,
        FourButtons = 3,
        AbsoluteMouse = 4,
        RelativeMouse = 5,
        JoystickMove = 6,
        JoystickMouse = 7,
        JoystickCamera = 8,
        ScrollWheel = 9,
        Trigger = 10,
        TouchMenu = 11,
        MouseJoystick = 12,
        MouseRegion = 13,
        RadialMenu = 14,
        SingleButton = 15,
        Switches = 16,
    }

    //
    // EInputActionOrigin
    //
    public enum InputActionOrigin : int
    {
        None = 0,
        SteamController_A = 1,
        SteamController_B = 2,
        SteamController_X = 3,
        SteamController_Y = 4,
        SteamController_LeftBumper = 5,
        SteamController_RightBumper = 6,
        SteamController_LeftGrip = 7,
        SteamController_RightGrip = 8,
        SteamController_Start = 9,
        SteamController_Back = 10,
        SteamController_LeftPad_Touch = 11,
        SteamController_LeftPad_Swipe = 12,
        SteamController_LeftPad_Click = 13,
        SteamController_LeftPad_DPadNorth = 14,
        SteamController_LeftPad_DPadSouth = 15,
        SteamController_LeftPad_DPadWest = 16,
        SteamController_LeftPad_DPadEast = 17,
        SteamController_RightPad_Touch = 18,
        SteamController_RightPad_Swipe = 19,
        SteamController_RightPad_Click = 20,
        SteamController_RightPad_DPadNorth = 21,
        SteamController_RightPad_DPadSouth = 22,
        SteamController_RightPad_DPadWest = 23,
        SteamController_RightPad_DPadEast = 24,
        SteamController_LeftTrigger_Pull = 25,
        SteamController_LeftTrigger_Click = 26,
        SteamController_RightTrigger_Pull = 27,
        SteamController_RightTrigger_Click = 28,
        SteamController_LeftStick_Move = 29,
        SteamController_LeftStick_Click = 30,
        SteamController_LeftStick_DPadNorth = 31,
        SteamController_LeftStick_DPadSouth = 32,
        SteamController_LeftStick_DPadWest = 33,
        SteamController_LeftStick_DPadEast = 34,
        SteamController_Gyro_Move = 35,
        SteamController_Gyro_Pitch = 36,
        SteamController_Gyro_Yaw = 37,
        SteamController_Gyro_Roll = 38,
        SteamController_Reserved0 = 39,
        SteamController_Reserved1 = 40,
        SteamController_Reserved2 = 41,
        SteamController_Reserved3 = 42,
        SteamController_Reserved4 = 43,
        SteamController_Reserved5 = 44,
        SteamController_Reserved6 = 45,
        SteamController_Reserved7 = 46,
        SteamController_Reserved8 = 47,
        SteamController_Reserved9 = 48,
        SteamController_Reserved10 = 49,
        PS4_X = 50,
        PS4_Circle = 51,
        PS4_Triangle = 52,
        PS4_Square = 53,
        PS4_LeftBumper = 54,
        PS4_RightBumper = 55,
        PS4_Options = 56,
        PS4_Share = 57,
        PS4_LeftPad_Touch = 58,
        PS4_LeftPad_Swipe = 59,
        PS4_LeftPad_Click = 60,
        PS4_LeftPad_DPadNorth = 61,
        PS4_LeftPad_DPadSouth = 62,
        PS4_LeftPad_DPadWest = 63,
        PS4_LeftPad_DPadEast = 64,
        PS4_RightPad_Touch = 65,
        PS4_RightPad_Swipe = 66,
        PS4_RightPad_Click = 67,
        PS4_RightPad_DPadNorth = 68,
        PS4_RightPad_DPadSouth = 69,
        PS4_RightPad_DPadWest = 70,
        PS4_RightPad_DPadEast = 71,
        PS4_CenterPad_Touch = 72,
        PS4_CenterPad_Swipe = 73,
        PS4_CenterPad_Click = 74,
        PS4_CenterPad_DPadNorth = 75,
        PS4_CenterPad_DPadSouth = 76,
        PS4_CenterPad_DPadWest = 77,
        PS4_CenterPad_DPadEast = 78,
        PS4_LeftTrigger_Pull = 79,
        PS4_LeftTrigger_Click = 80,
        PS4_RightTrigger_Pull = 81,
        PS4_RightTrigger_Click = 82,
        PS4_LeftStick_Move = 83,
        PS4_LeftStick_Click = 84,
        PS4_LeftStick_DPadNorth = 85,
        PS4_LeftStick_DPadSouth = 86,
        PS4_LeftStick_DPadWest = 87,
        PS4_LeftStick_DPadEast = 88,
        PS4_RightStick_Move = 89,
        PS4_RightStick_Click = 90,
        PS4_RightStick_DPadNorth = 91,
        PS4_RightStick_DPadSouth = 92,
        PS4_RightStick_DPadWest = 93,
        PS4_RightStick_DPadEast = 94,
        PS4_DPad_North = 95,
        PS4_DPad_South = 96,
        PS4_DPad_West = 97,
        PS4_DPad_East = 98,
        PS4_Gyro_Move = 99,
        PS4_Gyro_Pitch = 100,
        PS4_Gyro_Yaw = 101,
        PS4_Gyro_Roll = 102,
        PS4_DPad_Move = 103,
        PS4_Reserved1 = 104,
        PS4_Reserved2 = 105,
        PS4_Reserved3 = 106,
        PS4_Reserved4 = 107,
        PS4_Reserved5 = 108,
        PS4_Reserved6 = 109,
        PS4_Reserved7 = 110,
        PS4_Reserved8 = 111,
        PS4_Reserved9 = 112,
        PS4_Reserved10 = 113,
        XBoxOne_A = 114,
        XBoxOne_B = 115,
        XBoxOne_X = 116,
        XBoxOne_Y = 117,
        XBoxOne_LeftBumper = 118,
        XBoxOne_RightBumper = 119,
        XBoxOne_Menu = 120,
        XBoxOne_View = 121,
        XBoxOne_LeftTrigger_Pull = 122,
        XBoxOne_LeftTrigger_Click = 123,
        XBoxOne_RightTrigger_Pull = 124,
        XBoxOne_RightTrigger_Click = 125,
        XBoxOne_LeftStick_Move = 126,
        XBoxOne_LeftStick_Click = 127,
        XBoxOne_LeftStick_DPadNorth = 128,
        XBoxOne_LeftStick_DPadSouth = 129,
        XBoxOne_LeftStick_DPadWest = 130,
        XBoxOne_LeftStick_DPadEast = 131,
        XBoxOne_RightStick_Move = 132,
        XBoxOne_RightStick_Click = 133,
        XBoxOne_RightStick_DPadNorth = 134,
        XBoxOne_RightStick_DPadSouth = 135,
        XBoxOne_RightStick_DPadWest = 136,
        XBoxOne_RightStick_DPadEast = 137,
        XBoxOne_DPad_North = 138,
        XBoxOne_DPad_South = 139,
        XBoxOne_DPad_West = 140,
        XBoxOne_DPad_East = 141,
        XBoxOne_DPad_Move = 142,
        XBoxOne_LeftGrip_Lower = 143,
        XBoxOne_LeftGrip_Upper = 144,
        XBoxOne_RightGrip_Lower = 145,
        XBoxOne_RightGrip_Upper = 146,
        XBoxOne_Share = 147,
        XBoxOne_Reserved6 = 148,
        XBoxOne_Reserved7 = 149,
        XBoxOne_Reserved8 = 150,
        XBoxOne_Reserved9 = 151,
        XBoxOne_Reserved10 = 152,
        XBox360_A = 153,
        XBox360_B = 154,
        XBox360_X = 155,
        XBox360_Y = 156,
        XBox360_LeftBumper = 157,
        XBox360_RightBumper = 158,
        XBox360_Start = 159,
        XBox360_Back = 160,
        XBox360_LeftTrigger_Pull = 161,
        XBox360_LeftTrigger_Click = 162,
        XBox360_RightTrigger_Pull = 163,
        XBox360_RightTrigger_Click = 164,
        XBox360_LeftStick_Move = 165,
        XBox360_LeftStick_Click = 166,
        XBox360_LeftStick_DPadNorth = 167,
        XBox360_LeftStick_DPadSouth = 168,
        XBox360_LeftStick_DPadWest = 169,
        XBox360_LeftStick_DPadEast = 170,
        XBox360_RightStick_Move = 171,
        XBox360_RightStick_Click = 172,
        XBox360_RightStick_DPadNorth = 173,
        XBox360_RightStick_DPadSouth = 174,
        XBox360_RightStick_DPadWest = 175,
        XBox360_RightStick_DPadEast = 176,
        XBox360_DPad_North = 177,
        XBox360_DPad_South = 178,
        XBox360_DPad_West = 179,
        XBox360_DPad_East = 180,
        XBox360_DPad_Move = 181,
        XBox360_Reserved1 = 182,
        XBox360_Reserved2 = 183,
        XBox360_Reserved3 = 184,
        XBox360_Reserved4 = 185,
        XBox360_Reserved5 = 186,
        XBox360_Reserved6 = 187,
        XBox360_Reserved7 = 188,
        XBox360_Reserved8 = 189,
        XBox360_Reserved9 = 190,
        XBox360_Reserved10 = 191,
        Switch_A = 192,
        Switch_B = 193,
        Switch_X = 194,
        Switch_Y = 195,
        Switch_LeftBumper = 196,
        Switch_RightBumper = 197,
        Switch_Plus = 198,
        Switch_Minus = 199,
        Switch_Capture = 200,
        Switch_LeftTrigger_Pull = 201,
        Switch_LeftTrigger_Click = 202,
        Switch_RightTrigger_Pull = 203,
        Switch_RightTrigger_Click = 204,
        Switch_LeftStick_Move = 205,
        Switch_LeftStick_Click = 206,
        Switch_LeftStick_DPadNorth = 207,
        Switch_LeftStick_DPadSouth = 208,
        Switch_LeftStick_DPadWest = 209,
        Switch_LeftStick_DPadEast = 210,
        Switch_RightStick_Move = 211,
        Switch_RightStick_Click = 212,
        Switch_RightStick_DPadNorth = 213,
        Switch_RightStick_DPadSouth = 214,
        Switch_RightStick_DPadWest = 215,
        Switch_RightStick_DPadEast = 216,
        Switch_DPad_North = 217,
        Switch_DPad_South = 218,
        Switch_DPad_West = 219,
        Switch_DPad_East = 220,
        Switch_ProGyro_Move = 221,
        Switch_ProGyro_Pitch = 222,
        Switch_ProGyro_Yaw = 223,
        Switch_ProGyro_Roll = 224,
        Switch_DPad_Move = 225,
        Switch_Reserved1 = 226,
        Switch_Reserved2 = 227,
        Switch_Reserved3 = 228,
        Switch_Reserved4 = 229,
        Switch_Reserved5 = 230,
        Switch_Reserved6 = 231,
        Switch_Reserved7 = 232,
        Switch_Reserved8 = 233,
        Switch_Reserved9 = 234,
        Switch_Reserved10 = 235,
        Switch_RightGyro_Move = 236,
        Switch_RightGyro_Pitch = 237,
        Switch_RightGyro_Yaw = 238,
        Switch_RightGyro_Roll = 239,
        Switch_LeftGyro_Move = 240,
        Switch_LeftGyro_Pitch = 241,
        Switch_LeftGyro_Yaw = 242,
        Switch_LeftGyro_Roll = 243,
        Switch_LeftGrip_Lower = 244,
        Switch_LeftGrip_Upper = 245,
        Switch_RightGrip_Lower = 246,
        Switch_RightGrip_Upper = 247,
        Switch_Reserved11 = 248,
        Switch_Reserved12 = 249,
        Switch_Reserved13 = 250,
        Switch_Reserved14 = 251,
        Switch_Reserved15 = 252,
        Switch_Reserved16 = 253,
        Switch_Reserved17 = 254,
        Switch_Reserved18 = 255,
        Switch_Reserved19 = 256,
        Switch_Reserved20 = 257,
        PS5_X = 258,
        PS5_Circle = 259,
        PS5_Triangle = 260,
        PS5_Square = 261,
        PS5_LeftBumper = 262,
        PS5_RightBumper = 263,
        PS5_Option = 264,
        PS5_Create = 265,
        PS5_Mute = 266,
        PS5_LeftPad_Touch = 267,
        PS5_LeftPad_Swipe = 268,
        PS5_LeftPad_Click = 269,
        PS5_LeftPad_DPadNorth = 270,
        PS5_LeftPad_DPadSouth = 271,
        PS5_LeftPad_DPadWest = 272,
        PS5_LeftPad_DPadEast = 273,
        PS5_RightPad_Touch = 274,
        PS5_RightPad_Swipe = 275,
        PS5_RightPad_Click = 276,
        PS5_RightPad_DPadNorth = 277,
        PS5_RightPad_DPadSouth = 278,
        PS5_RightPad_DPadWest = 279,
        PS5_RightPad_DPadEast = 280,
        PS5_CenterPad_Touch = 281,
        PS5_CenterPad_Swipe = 282,
        PS5_CenterPad_Click = 283,
        PS5_CenterPad_DPadNorth = 284,
        PS5_CenterPad_DPadSouth = 285,
        PS5_CenterPad_DPadWest = 286,
        PS5_CenterPad_DPadEast = 287,
        PS5_LeftTrigger_Pull = 288,
        PS5_LeftTrigger_Click = 289,
        PS5_RightTrigger_Pull = 290,
        PS5_RightTrigger_Click = 291,
        PS5_LeftStick_Move = 292,
        PS5_LeftStick_Click = 293,
        PS5_LeftStick_DPadNorth = 294,
        PS5_LeftStick_DPadSouth = 295,
        PS5_LeftStick_DPadWest = 296,
        PS5_LeftStick_DPadEast = 297,
        PS5_RightStick_Move = 298,
        PS5_RightStick_Click = 299,
        PS5_RightStick_DPadNorth = 300,
        PS5_RightStick_DPadSouth = 301,
        PS5_RightStick_DPadWest = 302,
        PS5_RightStick_DPadEast = 303,
        PS5_DPad_North = 304,
        PS5_DPad_South = 305,
        PS5_DPad_West = 306,
        PS5_DPad_East = 307,
        PS5_Gyro_Move = 308,
        PS5_Gyro_Pitch = 309,
        PS5_Gyro_Yaw = 310,
        PS5_Gyro_Roll = 311,
        PS5_DPad_Move = 312,
        PS5_Reserved1 = 313,
        PS5_Reserved2 = 314,
        PS5_Reserved3 = 315,
        PS5_Reserved4 = 316,
        PS5_Reserved5 = 317,
        PS5_Reserved6 = 318,
        PS5_Reserved7 = 319,
        PS5_Reserved8 = 320,
        PS5_Reserved9 = 321,
        PS5_Reserved10 = 322,
        PS5_Reserved11 = 323,
        PS5_Reserved12 = 324,
        PS5_Reserved13 = 325,
        PS5_Reserved14 = 326,
        PS5_Reserved15 = 327,
        PS5_Reserved16 = 328,
        PS5_Reserved17 = 329,
        PS5_Reserved18 = 330,
        PS5_Reserved19 = 331,
        PS5_Reserved20 = 332,
        SteamDeck_A = 333,
        SteamDeck_B = 334,
        SteamDeck_X = 335,
        SteamDeck_Y = 336,
        SteamDeck_L1 = 337,
        SteamDeck_R1 = 338,
        SteamDeck_Menu = 339,
        SteamDeck_View = 340,
        SteamDeck_LeftPad_Touch = 341,
        SteamDeck_LeftPad_Swipe = 342,
        SteamDeck_LeftPad_Click = 343,
        SteamDeck_LeftPad_DPadNorth = 344,
        SteamDeck_LeftPad_DPadSouth = 345,
        SteamDeck_LeftPad_DPadWest = 346,
        SteamDeck_LeftPad_DPadEast = 347,
        SteamDeck_RightPad_Touch = 348,
        SteamDeck_RightPad_Swipe = 349,
        SteamDeck_RightPad_Click = 350,
        SteamDeck_RightPad_DPadNorth = 351,
        SteamDeck_RightPad_DPadSouth = 352,
        SteamDeck_RightPad_DPadWest = 353,
        SteamDeck_RightPad_DPadEast = 354,
        SteamDeck_L2_SoftPull = 355,
        SteamDeck_L2 = 356,
        SteamDeck_R2_SoftPull = 357,
        SteamDeck_R2 = 358,
        SteamDeck_LeftStick_Move = 359,
        SteamDeck_L3 = 360,
        SteamDeck_LeftStick_DPadNorth = 361,
        SteamDeck_LeftStick_DPadSouth = 362,
        SteamDeck_LeftStick_DPadWest = 363,
        SteamDeck_LeftStick_DPadEast = 364,
        SteamDeck_LeftStick_Touch = 365,
        SteamDeck_RightStick_Move = 366,
        SteamDeck_R3 = 367,
        SteamDeck_RightStick_DPadNorth = 368,
        SteamDeck_RightStick_DPadSouth = 369,
        SteamDeck_RightStick_DPadWest = 370,
        SteamDeck_RightStick_DPadEast = 371,
        SteamDeck_RightStick_Touch = 372,
        SteamDeck_L4 = 373,
        SteamDeck_R4 = 374,
        SteamDeck_L5 = 375,
        SteamDeck_R5 = 376,
        SteamDeck_DPad_Move = 377,
        SteamDeck_DPad_North = 378,
        SteamDeck_DPad_South = 379,
        SteamDeck_DPad_West = 380,
        SteamDeck_DPad_East = 381,
        SteamDeck_Gyro_Move = 382,
        SteamDeck_Gyro_Pitch = 383,
        SteamDeck_Gyro_Yaw = 384,
        SteamDeck_Gyro_Roll = 385,
        SteamDeck_Reserved1 = 386,
        SteamDeck_Reserved2 = 387,
        SteamDeck_Reserved3 = 388,
        SteamDeck_Reserved4 = 389,
        SteamDeck_Reserved5 = 390,
        SteamDeck_Reserved6 = 391,
        SteamDeck_Reserved7 = 392,
        SteamDeck_Reserved8 = 393,
        SteamDeck_Reserved9 = 394,
        SteamDeck_Reserved10 = 395,
        SteamDeck_Reserved11 = 396,
        SteamDeck_Reserved12 = 397,
        SteamDeck_Reserved13 = 398,
        SteamDeck_Reserved14 = 399,
        SteamDeck_Reserved15 = 400,
        SteamDeck_Reserved16 = 401,
        SteamDeck_Reserved17 = 402,
        SteamDeck_Reserved18 = 403,
        SteamDeck_Reserved19 = 404,
        SteamDeck_Reserved20 = 405,
        Count = 406,
        MaximumPossibleValue = 32767,
    }

    //
    // EXboxOrigin
    //
    public enum XboxOrigin : int
    {
        A = 0,
        B = 1,
        X = 2,
        Y = 3,
        LeftBumper = 4,
        RightBumper = 5,
        Menu = 6,
        View = 7,
        LeftTrigger_Pull = 8,
        LeftTrigger_Click = 9,
        RightTrigger_Pull = 10,
        RightTrigger_Click = 11,
        LeftStick_Move = 12,
        LeftStick_Click = 13,
        LeftStick_DPadNorth = 14,
        LeftStick_DPadSouth = 15,
        LeftStick_DPadWest = 16,
        LeftStick_DPadEast = 17,
        RightStick_Move = 18,
        RightStick_Click = 19,
        RightStick_DPadNorth = 20,
        RightStick_DPadSouth = 21,
        RightStick_DPadWest = 22,
        RightStick_DPadEast = 23,
        DPad_North = 24,
        DPad_South = 25,
        DPad_West = 26,
        DPad_East = 27,
        Count = 28,
    }

    //
    // EControllerHapticLocation
    //
    public enum ControllerHapticLocation : int
    {
        Left = 1,
        Right = 2,
        Both = 3,
    }

    //
    // EControllerHapticType
    //
    public enum ControllerHapticType : int
    {
        Off = 0,
        Tick = 1,
        Click = 2,
    }

    //
    // ESteamInputType
    //
    public enum InputType : int
    {
        Unknown = 0,
        SteamController = 1,
        XBox360Controller = 2,
        XBoxOneController = 3,
        GenericGamepad = 4,
        PS4Controller = 5,
        AppleMFiController = 6,
        AndroidController = 7,
        SwitchJoyConPair = 8,
        SwitchJoyConSingle = 9,
        SwitchProController = 10,
        MobileTouch = 11,
        PS3Controller = 12,
        PS5Controller = 13,
        SteamDeckController = 14,
        Count = 15,
        MaximumPossibleValue = 255,
    }

    //
    // ESteamInputConfigurationEnableType
    //
    public enum SteamInputConfigurationEnableType : int
    {
        None = 0,
        Playstation = 1,
        Xbox = 2,
        Generic = 4,
        Switch = 8,
    }

    //
    // ESteamInputLEDFlag
    //
    public enum SteamInputLEDFlag : int
    {
        SetColor = 0,
        RestoreUserDefault = 1,
    }

    //
    // ESteamInputGlyphSize
    //
    public enum GlyphSize : int
    {
        Small = 0,
        Medium = 1,
        Large = 2,
        Count = 3,
    }

    //
    // ESteamInputGlyphStyle
    //
    public enum SteamInputGlyphStyle : int
    {
        Knockout = 0,
        Light = 1,
        Dark = 2,
        NeutralColorABXY = 16,
        SolidABXY = 32,
    }

    //
    // ESteamInputActionEventType
    //
    public enum SteamInputActionEventType : int
    {
        DigitalAction = 0,
        AnalogAction = 1,
    }

    //
    // EControllerActionOrigin
    //
    public enum ControllerActionOrigin : int
    {
        None = 0,
        A = 1,
        B = 2,
        X = 3,
        Y = 4,
        LeftBumper = 5,
        RightBumper = 6,
        LeftGrip = 7,
        RightGrip = 8,
        Start = 9,
        Back = 10,
        LeftPad_Touch = 11,
        LeftPad_Swipe = 12,
        LeftPad_Click = 13,
        LeftPad_DPadNorth = 14,
        LeftPad_DPadSouth = 15,
        LeftPad_DPadWest = 16,
        LeftPad_DPadEast = 17,
        RightPad_Touch = 18,
        RightPad_Swipe = 19,
        RightPad_Click = 20,
        RightPad_DPadNorth = 21,
        RightPad_DPadSouth = 22,
        RightPad_DPadWest = 23,
        RightPad_DPadEast = 24,
        LeftTrigger_Pull = 25,
        LeftTrigger_Click = 26,
        RightTrigger_Pull = 27,
        RightTrigger_Click = 28,
        LeftStick_Move = 29,
        LeftStick_Click = 30,
        LeftStick_DPadNorth = 31,
        LeftStick_DPadSouth = 32,
        LeftStick_DPadWest = 33,
        LeftStick_DPadEast = 34,
        Gyro_Move = 35,
        Gyro_Pitch = 36,
        Gyro_Yaw = 37,
        Gyro_Roll = 38,
        PS4_X = 39,
        PS4_Circle = 40,
        PS4_Triangle = 41,
        PS4_Square = 42,
        PS4_LeftBumper = 43,
        PS4_RightBumper = 44,
        PS4_Options = 45,
        PS4_Share = 46,
        PS4_LeftPad_Touch = 47,
        PS4_LeftPad_Swipe = 48,
        PS4_LeftPad_Click = 49,
        PS4_LeftPad_DPadNorth = 50,
        PS4_LeftPad_DPadSouth = 51,
        PS4_LeftPad_DPadWest = 52,
        PS4_LeftPad_DPadEast = 53,
        PS4_RightPad_Touch = 54,
        PS4_RightPad_Swipe = 55,
        PS4_RightPad_Click = 56,
        PS4_RightPad_DPadNorth = 57,
        PS4_RightPad_DPadSouth = 58,
        PS4_RightPad_DPadWest = 59,
        PS4_RightPad_DPadEast = 60,
        PS4_CenterPad_Touch = 61,
        PS4_CenterPad_Swipe = 62,
        PS4_CenterPad_Click = 63,
        PS4_CenterPad_DPadNorth = 64,
        PS4_CenterPad_DPadSouth = 65,
        PS4_CenterPad_DPadWest = 66,
        PS4_CenterPad_DPadEast = 67,
        PS4_LeftTrigger_Pull = 68,
        PS4_LeftTrigger_Click = 69,
        PS4_RightTrigger_Pull = 70,
        PS4_RightTrigger_Click = 71,
        PS4_LeftStick_Move = 72,
        PS4_LeftStick_Click = 73,
        PS4_LeftStick_DPadNorth = 74,
        PS4_LeftStick_DPadSouth = 75,
        PS4_LeftStick_DPadWest = 76,
        PS4_LeftStick_DPadEast = 77,
        PS4_RightStick_Move = 78,
        PS4_RightStick_Click = 79,
        PS4_RightStick_DPadNorth = 80,
        PS4_RightStick_DPadSouth = 81,
        PS4_RightStick_DPadWest = 82,
        PS4_RightStick_DPadEast = 83,
        PS4_DPad_North = 84,
        PS4_DPad_South = 85,
        PS4_DPad_West = 86,
        PS4_DPad_East = 87,
        PS4_Gyro_Move = 88,
        PS4_Gyro_Pitch = 89,
        PS4_Gyro_Yaw = 90,
        PS4_Gyro_Roll = 91,
        XBoxOne_A = 92,
        XBoxOne_B = 93,
        XBoxOne_X = 94,
        XBoxOne_Y = 95,
        XBoxOne_LeftBumper = 96,
        XBoxOne_RightBumper = 97,
        XBoxOne_Menu = 98,
        XBoxOne_View = 99,
        XBoxOne_LeftTrigger_Pull = 100,
        XBoxOne_LeftTrigger_Click = 101,
        XBoxOne_RightTrigger_Pull = 102,
        XBoxOne_RightTrigger_Click = 103,
        XBoxOne_LeftStick_Move = 104,
        XBoxOne_LeftStick_Click = 105,
        XBoxOne_LeftStick_DPadNorth = 106,
        XBoxOne_LeftStick_DPadSouth = 107,
        XBoxOne_LeftStick_DPadWest = 108,
        XBoxOne_LeftStick_DPadEast = 109,
        XBoxOne_RightStick_Move = 110,
        XBoxOne_RightStick_Click = 111,
        XBoxOne_RightStick_DPadNorth = 112,
        XBoxOne_RightStick_DPadSouth = 113,
        XBoxOne_RightStick_DPadWest = 114,
        XBoxOne_RightStick_DPadEast = 115,
        XBoxOne_DPad_North = 116,
        XBoxOne_DPad_South = 117,
        XBoxOne_DPad_West = 118,
        XBoxOne_DPad_East = 119,
        XBox360_A = 120,
        XBox360_B = 121,
        XBox360_X = 122,
        XBox360_Y = 123,
        XBox360_LeftBumper = 124,
        XBox360_RightBumper = 125,
        XBox360_Start = 126,
        XBox360_Back = 127,
        XBox360_LeftTrigger_Pull = 128,
        XBox360_LeftTrigger_Click = 129,
        XBox360_RightTrigger_Pull = 130,
        XBox360_RightTrigger_Click = 131,
        XBox360_LeftStick_Move = 132,
        XBox360_LeftStick_Click = 133,
        XBox360_LeftStick_DPadNorth = 134,
        XBox360_LeftStick_DPadSouth = 135,
        XBox360_LeftStick_DPadWest = 136,
        XBox360_LeftStick_DPadEast = 137,
        XBox360_RightStick_Move = 138,
        XBox360_RightStick_Click = 139,
        XBox360_RightStick_DPadNorth = 140,
        XBox360_RightStick_DPadSouth = 141,
        XBox360_RightStick_DPadWest = 142,
        XBox360_RightStick_DPadEast = 143,
        XBox360_DPad_North = 144,
        XBox360_DPad_South = 145,
        XBox360_DPad_West = 146,
        XBox360_DPad_East = 147,
        SteamV2_A = 148,
        SteamV2_B = 149,
        SteamV2_X = 150,
        SteamV2_Y = 151,
        SteamV2_LeftBumper = 152,
        SteamV2_RightBumper = 153,
        SteamV2_LeftGrip_Lower = 154,
        SteamV2_LeftGrip_Upper = 155,
        SteamV2_RightGrip_Lower = 156,
        SteamV2_RightGrip_Upper = 157,
        SteamV2_LeftBumper_Pressure = 158,
        SteamV2_RightBumper_Pressure = 159,
        SteamV2_LeftGrip_Pressure = 160,
        SteamV2_RightGrip_Pressure = 161,
        SteamV2_LeftGrip_Upper_Pressure = 162,
        SteamV2_RightGrip_Upper_Pressure = 163,
        SteamV2_Start = 164,
        SteamV2_Back = 165,
        SteamV2_LeftPad_Touch = 166,
        SteamV2_LeftPad_Swipe = 167,
        SteamV2_LeftPad_Click = 168,
        SteamV2_LeftPad_Pressure = 169,
        SteamV2_LeftPad_DPadNorth = 170,
        SteamV2_LeftPad_DPadSouth = 171,
        SteamV2_LeftPad_DPadWest = 172,
        SteamV2_LeftPad_DPadEast = 173,
        SteamV2_RightPad_Touch = 174,
        SteamV2_RightPad_Swipe = 175,
        SteamV2_RightPad_Click = 176,
        SteamV2_RightPad_Pressure = 177,
        SteamV2_RightPad_DPadNorth = 178,
        SteamV2_RightPad_DPadSouth = 179,
        SteamV2_RightPad_DPadWest = 180,
        SteamV2_RightPad_DPadEast = 181,
        SteamV2_LeftTrigger_Pull = 182,
        SteamV2_LeftTrigger_Click = 183,
        SteamV2_RightTrigger_Pull = 184,
        SteamV2_RightTrigger_Click = 185,
        SteamV2_LeftStick_Move = 186,
        SteamV2_LeftStick_Click = 187,
        SteamV2_LeftStick_DPadNorth = 188,
        SteamV2_LeftStick_DPadSouth = 189,
        SteamV2_LeftStick_DPadWest = 190,
        SteamV2_LeftStick_DPadEast = 191,
        SteamV2_Gyro_Move = 192,
        SteamV2_Gyro_Pitch = 193,
        SteamV2_Gyro_Yaw = 194,
        SteamV2_Gyro_Roll = 195,
        Switch_A = 196,
        Switch_B = 197,
        Switch_X = 198,
        Switch_Y = 199,
        Switch_LeftBumper = 200,
        Switch_RightBumper = 201,
        Switch_Plus = 202,
        Switch_Minus = 203,
        Switch_Capture = 204,
        Switch_LeftTrigger_Pull = 205,
        Switch_LeftTrigger_Click = 206,
        Switch_RightTrigger_Pull = 207,
        Switch_RightTrigger_Click = 208,
        Switch_LeftStick_Move = 209,
        Switch_LeftStick_Click = 210,
        Switch_LeftStick_DPadNorth = 211,
        Switch_LeftStick_DPadSouth = 212,
        Switch_LeftStick_DPadWest = 213,
        Switch_LeftStick_DPadEast = 214,
        Switch_RightStick_Move = 215,
        Switch_RightStick_Click = 216,
        Switch_RightStick_DPadNorth = 217,
        Switch_RightStick_DPadSouth = 218,
        Switch_RightStick_DPadWest = 219,
        Switch_RightStick_DPadEast = 220,
        Switch_DPad_North = 221,
        Switch_DPad_South = 222,
        Switch_DPad_West = 223,
        Switch_DPad_East = 224,
        Switch_ProGyro_Move = 225,
        Switch_ProGyro_Pitch = 226,
        Switch_ProGyro_Yaw = 227,
        Switch_ProGyro_Roll = 228,
        Switch_RightGyro_Move = 229,
        Switch_RightGyro_Pitch = 230,
        Switch_RightGyro_Yaw = 231,
        Switch_RightGyro_Roll = 232,
        Switch_LeftGyro_Move = 233,
        Switch_LeftGyro_Pitch = 234,
        Switch_LeftGyro_Yaw = 235,
        Switch_LeftGyro_Roll = 236,
        Switch_LeftGrip_Lower = 237,
        Switch_LeftGrip_Upper = 238,
        Switch_RightGrip_Lower = 239,
        Switch_RightGrip_Upper = 240,
        PS4_DPad_Move = 241,
        XBoxOne_DPad_Move = 242,
        XBox360_DPad_Move = 243,
        Switch_DPad_Move = 244,
        PS5_X = 245,
        PS5_Circle = 246,
        PS5_Triangle = 247,
        PS5_Square = 248,
        PS5_LeftBumper = 249,
        PS5_RightBumper = 250,
        PS5_Option = 251,
        PS5_Create = 252,
        PS5_Mute = 253,
        PS5_LeftPad_Touch = 254,
        PS5_LeftPad_Swipe = 255,
        PS5_LeftPad_Click = 256,
        PS5_LeftPad_DPadNorth = 257,
        PS5_LeftPad_DPadSouth = 258,
        PS5_LeftPad_DPadWest = 259,
        PS5_LeftPad_DPadEast = 260,
        PS5_RightPad_Touch = 261,
        PS5_RightPad_Swipe = 262,
        PS5_RightPad_Click = 263,
        PS5_RightPad_DPadNorth = 264,
        PS5_RightPad_DPadSouth = 265,
        PS5_RightPad_DPadWest = 266,
        PS5_RightPad_DPadEast = 267,
        PS5_CenterPad_Touch = 268,
        PS5_CenterPad_Swipe = 269,
        PS5_CenterPad_Click = 270,
        PS5_CenterPad_DPadNorth = 271,
        PS5_CenterPad_DPadSouth = 272,
        PS5_CenterPad_DPadWest = 273,
        PS5_CenterPad_DPadEast = 274,
        PS5_LeftTrigger_Pull = 275,
        PS5_LeftTrigger_Click = 276,
        PS5_RightTrigger_Pull = 277,
        PS5_RightTrigger_Click = 278,
        PS5_LeftStick_Move = 279,
        PS5_LeftStick_Click = 280,
        PS5_LeftStick_DPadNorth = 281,
        PS5_LeftStick_DPadSouth = 282,
        PS5_LeftStick_DPadWest = 283,
        PS5_LeftStick_DPadEast = 284,
        PS5_RightStick_Move = 285,
        PS5_RightStick_Click = 286,
        PS5_RightStick_DPadNorth = 287,
        PS5_RightStick_DPadSouth = 288,
        PS5_RightStick_DPadWest = 289,
        PS5_RightStick_DPadEast = 290,
        PS5_DPad_Move = 291,
        PS5_DPad_North = 292,
        PS5_DPad_South = 293,
        PS5_DPad_West = 294,
        PS5_DPad_East = 295,
        PS5_Gyro_Move = 296,
        PS5_Gyro_Pitch = 297,
        PS5_Gyro_Yaw = 298,
        PS5_Gyro_Roll = 299,
        XBoxOne_LeftGrip_Lower = 300,
        XBoxOne_LeftGrip_Upper = 301,
        XBoxOne_RightGrip_Lower = 302,
        XBoxOne_RightGrip_Upper = 303,
        XBoxOne_Share = 304,
        SteamDeck_A = 305,
        SteamDeck_B = 306,
        SteamDeck_X = 307,
        SteamDeck_Y = 308,
        SteamDeck_L1 = 309,
        SteamDeck_R1 = 310,
        SteamDeck_Menu = 311,
        SteamDeck_View = 312,
        SteamDeck_LeftPad_Touch = 313,
        SteamDeck_LeftPad_Swipe = 314,
        SteamDeck_LeftPad_Click = 315,
        SteamDeck_LeftPad_DPadNorth = 316,
        SteamDeck_LeftPad_DPadSouth = 317,
        SteamDeck_LeftPad_DPadWest = 318,
        SteamDeck_LeftPad_DPadEast = 319,
        SteamDeck_RightPad_Touch = 320,
        SteamDeck_RightPad_Swipe = 321,
        SteamDeck_RightPad_Click = 322,
        SteamDeck_RightPad_DPadNorth = 323,
        SteamDeck_RightPad_DPadSouth = 324,
        SteamDeck_RightPad_DPadWest = 325,
        SteamDeck_RightPad_DPadEast = 326,
        SteamDeck_L2_SoftPull = 327,
        SteamDeck_L2 = 328,
        SteamDeck_R2_SoftPull = 329,
        SteamDeck_R2 = 330,
        SteamDeck_LeftStick_Move = 331,
        SteamDeck_L3 = 332,
        SteamDeck_LeftStick_DPadNorth = 333,
        SteamDeck_LeftStick_DPadSouth = 334,
        SteamDeck_LeftStick_DPadWest = 335,
        SteamDeck_LeftStick_DPadEast = 336,
        SteamDeck_LeftStick_Touch = 337,
        SteamDeck_RightStick_Move = 338,
        SteamDeck_R3 = 339,
        SteamDeck_RightStick_DPadNorth = 340,
        SteamDeck_RightStick_DPadSouth = 341,
        SteamDeck_RightStick_DPadWest = 342,
        SteamDeck_RightStick_DPadEast = 343,
        SteamDeck_RightStick_Touch = 344,
        SteamDeck_L4 = 345,
        SteamDeck_R4 = 346,
        SteamDeck_L5 = 347,
        SteamDeck_R5 = 348,
        SteamDeck_DPad_Move = 349,
        SteamDeck_DPad_North = 350,
        SteamDeck_DPad_South = 351,
        SteamDeck_DPad_West = 352,
        SteamDeck_DPad_East = 353,
        SteamDeck_Gyro_Move = 354,
        SteamDeck_Gyro_Pitch = 355,
        SteamDeck_Gyro_Yaw = 356,
        SteamDeck_Gyro_Roll = 357,
        SteamDeck_Reserved1 = 358,
        SteamDeck_Reserved2 = 359,
        SteamDeck_Reserved3 = 360,
        SteamDeck_Reserved4 = 361,
        SteamDeck_Reserved5 = 362,
        SteamDeck_Reserved6 = 363,
        SteamDeck_Reserved7 = 364,
        SteamDeck_Reserved8 = 365,
        SteamDeck_Reserved9 = 366,
        SteamDeck_Reserved10 = 367,
        SteamDeck_Reserved11 = 368,
        SteamDeck_Reserved12 = 369,
        SteamDeck_Reserved13 = 370,
        SteamDeck_Reserved14 = 371,
        SteamDeck_Reserved15 = 372,
        SteamDeck_Reserved16 = 373,
        SteamDeck_Reserved17 = 374,
        SteamDeck_Reserved18 = 375,
        SteamDeck_Reserved19 = 376,
        SteamDeck_Reserved20 = 377,
        Count = 378,
        MaximumPossibleValue = 32767,
    }

    //
    // ESteamControllerLEDFlag
    //
    public enum SteamControllerLEDFlag : int
    {
        SetColor = 0,
        RestoreUserDefault = 1,
    }

    //
    // EUGCMatchingUGCType
    //
    public enum UgcType : int
    {
        Items = 0,
        Items_Mtx = 1,
        Items_ReadyToUse = 2,
        Collections = 3,
        Artwork = 4,
        Videos = 5,
        Screenshots = 6,
        AllGuides = 7,
        WebGuides = 8,
        IntegratedGuides = 9,
        UsableInGame = 10,
        ControllerBindings = 11,
        GameManagedItems = 12,
        All = -1,
    }

    //
    // EUserUGCListSortOrder
    //
    public enum UserUGCListSortOrder : int
    {
        CreationOrderDesc = 0,
        CreationOrderAsc = 1,
        TitleAsc = 2,
        LastUpdatedDesc = 3,
        SubscriptionDateDesc = 4,
        VoteScoreDesc = 5,
        ForModeration = 6,
    }

    //
    // EUGCQuery
    //
    public enum UGCQuery : int
    {
        RankedByVote = 0,
        RankedByPublicationDate = 1,
        AcceptedForGameRankedByAcceptanceDate = 2,
        RankedByTrend = 3,
        FavoritedByFriendsRankedByPublicationDate = 4,
        CreatedByFriendsRankedByPublicationDate = 5,
        RankedByNumTimesReported = 6,
        CreatedByFollowedUsersRankedByPublicationDate = 7,
        NotYetRated = 8,
        RankedByTotalVotesAsc = 9,
        RankedByVotesUp = 10,
        RankedByTextSearch = 11,
        RankedByTotalUniqueSubscriptions = 12,
        RankedByPlaytimeTrend = 13,
        RankedByTotalPlaytime = 14,
        RankedByAveragePlaytimeTrend = 15,
        RankedByLifetimeAveragePlaytime = 16,
        RankedByPlaytimeSessionsTrend = 17,
        RankedByLifetimePlaytimeSessions = 18,
        RankedByLastUpdatedDate = 19,
    }

    //
    // EItemUpdateStatus
    //
    public enum ItemUpdateStatus : int
    {
        Invalid = 0,
        PreparingConfig = 1,
        PreparingContent = 2,
        UploadingContent = 3,
        UploadingPreviewFile = 4,
        CommittingChanges = 5,
    }

    //
    // EItemPreviewType
    //
    public enum ItemPreviewType : int
    {
        Image = 0,
        YouTubeVideo = 1,
        Sketchfab = 2,
        EnvironmentMap_HorizontalCross = 3,
        EnvironmentMap_LatLong = 4,
        ReservedMax = 255,
    }

    //
    // ESteamItemFlags
    //
    public enum SteamItemFlags : int
    {
        NoTrade = 1,
        Removed = 256,
        Consumed = 512,
    }

    //
    // EParentalFeature
    //
    public enum ParentalFeature : int
    {
        Invalid = 0,
        Store = 1,
        Community = 2,
        Profile = 3,
        Friends = 4,
        News = 5,
        Trading = 6,
        Settings = 7,
        Console = 8,
        Browser = 9,
        ParentalSetup = 10,
        Library = 11,
        Test = 12,
        SiteLicense = 13,
        Max = 14,
    }

    //
    // ESteamDeviceFormFactor
    //
    public enum SteamDeviceFormFactor : int
    {
        Unknown = 0,
        Phone = 1,
        Tablet = 2,
        Computer = 3,
        TV = 4,
    }

    //
    // ESteamNetworkingAvailability
    //
    public enum SteamNetworkingAvailability : int
    {
        CannotTry = -102,
        Failed = -101,
        Previously = -100,
        Retrying = -10,
        NeverTried = 1,
        Waiting = 2,
        Attempting = 3,
        Current = 100,
        Unknown = 0,
        Force32bit = 2147483647,
    }

    //
    // ESteamNetworkingIdentityType
    //
    public enum NetIdentityType : int
    {
        Invalid = 0,
        SteamID = 16,
        XboxPairwiseID = 17,
        SonyPSN = 18,
        GoogleStadia = 19,
        IPAddress = 1,
        GenericString = 2,
        GenericBytes = 3,
        UnknownType = 4,
        Force32bit = 2147483647,
    }

    //
    // ESteamNetworkingFakeIPType
    //
    public enum SteamNetworkingFakeIPType : int
    {
        Invalid = 0,
        NotFake = 1,
        GlobalIPv4 = 2,
        LocalIPv4 = 3,
    }

    //
    // ESteamNetworkingConnectionState
    //
    public enum ConnectionState : int
    {
        None = 0,
        Connecting = 1,
        FindingRoute = 2,
        Connected = 3,
        ClosedByPeer = 4,
        ProblemDetectedLocally = 5,
        FinWait = -1,
        Linger = -2,
        Dead = -3,
    }

    //
    // ESteamNetConnectionEnd
    //
    public enum NetConnectionEnd : int
    {
        Invalid = 0,
        App_Min = 1000,
        App_Generic = 1000,
        App_Max = 1999,
        AppException_Min = 2000,
        AppException_Generic = 2000,
        AppException_Max = 2999,
        Local_Min = 3000,
        Local_OfflineMode = 3001,
        Local_ManyRelayConnectivity = 3002,
        Local_HostedServerPrimaryRelay = 3003,
        Local_NetworkConfig = 3004,
        Local_Rights = 3005,
        Local_P2P_ICE_NoPublicAddresses = 3006,
        Local_Max = 3999,
        Remote_Min = 4000,
        Remote_Timeout = 4001,
        Remote_BadCrypt = 4002,
        Remote_BadCert = 4003,
        Remote_BadProtocolVersion = 4006,
        Remote_P2P_ICE_NoPublicAddresses = 4007,
        Remote_Max = 4999,
        Misc_Min = 5000,
        Misc_Generic = 5001,
        Misc_publicError = 5002,
        Misc_Timeout = 5003,
        Misc_SteamConnectivity = 5005,
        Misc_NoRelaySessionsToClient = 5006,
        Misc_P2P_Rendezvous = 5008,
        Misc_P2P_NAT_Firewall = 5009,
        Misc_PeerSentNoConnection = 5010,
        Misc_Max = 5999,
    }

    //
    // ESteamNetworkingConfigScope
    //
    public enum NetConfigScope : int
    {
        Global = 1,
        SocketsInterface = 2,
        ListenSocket = 3,
        Connection = 4,
    }

    //
    // ESteamNetworkingConfigDataType
    //
    public enum NetConfigType : int
    {
        Int32 = 1,
        Int64 = 2,
        Float = 3,
        String = 4,
        Ptr = 5,
    }

    //
    // ESteamNetworkingConfigValue
    //
    public enum NetConfig : int
    {
        Invalid = 0,
        TimeoutInitial = 24,
        TimeoutConnected = 25,
        SendBufferSize = 9,
        ConnectionUserData = 40,
        SendRateMin = 10,
        SendRateMax = 11,
        NagleTime = 12,
        IP_AllowWithoutAuth = 23,
        MTU_PacketSize = 32,
        MTU_DataSize = 33,
        Unencrypted = 34,
        SymmetricConnect = 37,
        LocalVirtualPort = 38,
        DualWifi_Enable = 39,
        EnableDiagnosticsUI = 46,
        FakePacketLoss_Send = 2,
        FakePacketLoss_Recv = 3,
        FakePacketLag_Send = 4,
        FakePacketLag_Recv = 5,
        FakePacketReorder_Send = 6,
        FakePacketReorder_Recv = 7,
        FakePacketReorder_Time = 8,
        FakePacketDup_Send = 26,
        FakePacketDup_Recv = 27,
        FakePacketDup_TimeMax = 28,
        PacketTraceMaxBytes = 41,
        FakeRateLimit_Send_Rate = 42,
        FakeRateLimit_Send_Burst = 43,
        FakeRateLimit_Recv_Rate = 44,
        FakeRateLimit_Recv_Burst = 45,
        Callback_ConnectionStatusChanged = 201,
        Callback_AuthStatusChanged = 202,
        Callback_RelayNetworkStatusChanged = 203,
        Callback_MessagesSessionRequest = 204,
        Callback_MessagesSessionFailed = 205,
        Callback_CreateConnectionSignaling = 206,
        Callback_FakeIPResult = 207,
        P2P_STUN_ServerList = 103,
        P2P_Transport_ICE_Enable = 104,
        P2P_Transport_ICE_Penalty = 105,
        P2P_Transport_SDR_Penalty = 106,
        SDRClient_ConsecutitivePingTimeoutsFailInitial = 19,
        SDRClient_ConsecutitivePingTimeoutsFail = 20,
        SDRClient_MinPingsBeforePingAccurate = 21,
        SDRClient_SingleSocket = 22,
        SDRClient_ForceRelayCluster = 29,
        SDRClient_DebugTicketAddress = 30,
        SDRClient_ForceProxyAddr = 31,
        SDRClient_FakeClusterPing = 36,
        LogLevel_AckRTT = 13,
        LogLevel_PacketDecode = 14,
        LogLevel_Message = 15,
        LogLevel_PacketGaps = 16,
        LogLevel_P2PRendezvous = 17,
        LogLevel_SDRRelayPings = 18,
        DELETED_EnumerateDevVars = 35,
    }

    //
    // ESteamNetworkingGetConfigValueResult
    //
    public enum NetConfigResult : int
    {
        BadValue = -1,
        BadScopeObj = -2,
        BufferTooSmall = -3,
        OK = 1,
        OKInherited = 2,
    }

    //
    // ESteamNetworkingSocketsDebugOutputType
    //
    public enum NetDebugOutput : int
    {
        None = 0,
        Bug = 1,
        Error = 2,
        Important = 3,
        Warning = 4,
        Msg = 5,
        Verbose = 6,
        Debug = 7,
        Everything = 8,
    }

    //
    // EServerMode
    //
    public enum ServerMode : int
    {
        Invalid = 0,
        NoAuthentication = 1,
        Authentication = 2,
        AuthenticationAndSecure = 3,
    }

    public enum EActivateGameOverlayToWebPageMode
    {
        k_EActivateGameOverlayToWebPageMode_Default = 0,        // Browser will open next to all other windows that the user has open in the overlay.
                                                                // The window will remain open, even if the user closes then re-opens the overlay.

        k_EActivateGameOverlayToWebPageMode_Modal = 1           // Browser will be opened in a special overlay configuration which hides all other windows
                                                                // that the user has open in the overlay. When the user closes the overlay, the browser window
                                                                // will also close. When the user closes the browser window, the overlay will automatically close.
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

    // Feature types for parental settings
    public enum EParentalFeature : int
    {
        k_EFeatureInvalid = 0,
        k_EFeatureStore = 1,
        k_EFeatureCommunity = 2,
        k_EFeatureProfile = 3,
        k_EFeatureFriends = 4,
        k_EFeatureNews = 5,
        k_EFeatureTrading = 6,
        k_EFeatureSettings = 7,
        k_EFeatureConsole = 8,
        k_EFeatureBrowser = 9,
        k_EFeatureParentalSetup = 10,
        k_EFeatureLibrary = 11,
        k_EFeatureTest = 12,
        k_EFeatureSiteLicense = 13,
        k_EFeatureMax
    };

    //-----------------------------------------------------------------------------
    // Purpose: possible results when registering an activation code
    //-----------------------------------------------------------------------------
    public enum ERegisterActivationCodeResult : int
    {
        k_ERegisterActivationCodeResultOK = 0,
        k_ERegisterActivationCodeResultFail = 1,
        k_ERegisterActivationCodeResultAlreadyRegistered = 2,
        k_ERegisterActivationCodeResultTimeout = 3,
        k_ERegisterActivationCodeAlreadyOwned = 4,
    }

    public enum ESteamControllerPad : int
    {
        k_ESteamControllerPad_Left,
        k_ESteamControllerPad_Right
    }

    //-----------------------------------------------------------------------------
    // Purpose: list of states a friend can be in
    //-----------------------------------------------------------------------------
    public enum EPersonaState : int
    {
        k_EPersonaStateOffline = 0,         // friend is not currently logged on
        k_EPersonaStateOnline = 1,          // friend is logged on
        k_EPersonaStateBusy = 2,            // user is on, but busy
        k_EPersonaStateAway = 3,            // auto-away feature
        k_EPersonaStateSnooze = 4,          // auto-away for a long time
        k_EPersonaStateLookingToTrade = 5,  // Online, trading
        k_EPersonaStateLookingToPlay = 6,   // Online, wanting to play
        k_EPersonaStateMax,
    }

    //-----------------------------------------------------------------------------
    // Purpose: flags for enumerating friends list, or quickly checking a the relationship between users
    //-----------------------------------------------------------------------------
    [Flags]
    public enum EFriendFlags : int
    {
        k_EFriendFlagNone = 0x00,
        k_EFriendFlagBlocked = 0x01,
        k_EFriendFlagFriendshipRequested = 0x02,
        k_EFriendFlagImmediate = 0x04,          // "regular" friend
        k_EFriendFlagClanMember = 0x08,
        k_EFriendFlagOnGameServer = 0x10,
        // k_EFriendFlagHasPlayedWith	= 0x20,	// not currently used
        // k_EFriendFlagFriendOfFriend	= 0x40, // not currently used
        k_EFriendFlagRequestingFriendship = 0x80,
        k_EFriendFlagRequestingInfo = 0x100,
        k_EFriendFlagIgnored = 0x200,
        k_EFriendFlagIgnoredFriend = 0x400,
        k_EFriendFlagSuggested = 0x800,
        k_EFriendFlagAll = 0xFFFF,
    }

    //-----------------------------------------------------------------------------
    // Purpose: user restriction flags
    //-----------------------------------------------------------------------------
    public enum EUserRestriction : int
    {
        k_nUserRestrictionNone = 0, // no known chat/content restriction
        k_nUserRestrictionUnknown = 1,  // we don't know yet (user offline)
        k_nUserRestrictionAnyChat = 2,  // user is not allowed to (or can't) send/recv any chat
        k_nUserRestrictionVoiceChat = 4,    // user is not allowed to (or can't) send/recv voice chat
        k_nUserRestrictionGroupChat = 8,    // user is not allowed to (or can't) send/recv group chat
        k_nUserRestrictionRating = 16,  // user is too young according to rating in current region
        k_nUserRestrictionGameInvites = 32, // user cannot send or recv game invites (e.g. mobile)
        k_nUserRestrictionTrading = 64, // user cannot participate in trading (console, mobile)
    }

    // These values are passed as parameters to the store
    public enum EOverlayToStoreFlag : int
    {
        k_EOverlayToStoreFlag_None = 0,
        k_EOverlayToStoreFlag_AddToCart = 1,
        k_EOverlayToStoreFlag_AddToCartAndShow = 2,
    }

    // used in PersonaStateChange_t::m_nChangeFlags to describe what's changed about a user
    // these flags describe what the client has learned has changed recently, so on startup you'll see a name, avatar & relationship change for every friend
    [Flags]
    public enum EPersonaChange : int
    {
        k_EPersonaChangeName = 0x0001,
        k_EPersonaChangeStatus = 0x0002,
        k_EPersonaChangeComeOnline = 0x0004,
        k_EPersonaChangeGoneOffline = 0x0008,
        k_EPersonaChangeGamePlayed = 0x0010,
        k_EPersonaChangeGameServer = 0x0020,
        k_EPersonaChangeAvatar = 0x0040,
        k_EPersonaChangeJoinedSource = 0x0080,
        k_EPersonaChangeLeftSource = 0x0100,
        k_EPersonaChangeRelationshipChanged = 0x0200,
        k_EPersonaChangeNameFirstSet = 0x0400,
        k_EPersonaChangeFacebookInfo = 0x0800,
        k_EPersonaChangeNickname = 0x1000,
        k_EPersonaChangeSteamLevel = 0x2000,
    }

    // list of possible return values from the ISteamGameCoordinator API
    public enum EGCResults : int
    {
        k_EGCResultOK = 0,
        k_EGCResultNoMessage = 1,           // There is no message in the queue
        k_EGCResultBufferTooSmall = 2,      // The buffer is too small for the requested message
        k_EGCResultNotLoggedOn = 3,         // The client is not logged onto Steam
        k_EGCResultInvalidMessage = 4,      // Something was wrong with the message being sent with SendMessage
    }

    public enum EHTMLMouseButton : int
    {
        eHTMLMouseButton_Left = 0,
        eHTMLMouseButton_Right = 1,
        eHTMLMouseButton_Middle = 2,
    }

    public enum EMouseCursor : int
    {
        dc_user = 0,
        dc_none,
        dc_arrow,
        dc_ibeam,
        dc_hourglass,
        dc_waitarrow,
        dc_crosshair,
        dc_up,
        dc_sizenw,
        dc_sizese,
        dc_sizene,
        dc_sizesw,
        dc_sizew,
        dc_sizee,
        dc_sizen,
        dc_sizes,
        dc_sizewe,
        dc_sizens,
        dc_sizeall,
        dc_no,
        dc_hand,
        dc_blank, // don't show any custom cursor, just use your default
        dc_middle_pan,
        dc_north_pan,
        dc_north_east_pan,
        dc_east_pan,
        dc_south_east_pan,
        dc_south_pan,
        dc_south_west_pan,
        dc_west_pan,
        dc_north_west_pan,
        dc_alias,
        dc_cell,
        dc_colresize,
        dc_copycur,
        dc_verticaltext,
        dc_rowresize,
        dc_zoomin,
        dc_zoomout,
        dc_help,
        dc_custom,

        dc_last, // custom cursors start from this value and up
    }

    [Flags]
    public enum EHTMLKeyModifiers : int
    {
        k_eHTMLKeyModifier_None = 0,
        k_eHTMLKeyModifier_AltDown = 1 << 0,
        k_eHTMLKeyModifier_CtrlDown = 1 << 1,
        k_eHTMLKeyModifier_ShiftDown = 1 << 2,
    }

    [Flags]
    public enum ESteamItemFlags : int
    {
        // Item status flags - these flags are permanently attached to specific item instances
        k_ESteamItemNoTrade = 1 << 0, // This item is account-locked and cannot be traded or given away.

        // Action confirmation flags - these flags are set one time only, as part of a result set
        k_ESteamItemRemoved = 1 << 8,   // The item has been destroyed, traded away, expired, or otherwise invalidated
        k_ESteamItemConsumed = 1 << 9,  // The item quantity has been decreased by 1 via ConsumeItem API.

        // All other flag bits are currently reserved for internal Steam use at this time.
        // Do not assume anything about the state of other flags which are not defined here.
    }

    // lobby type description
    public enum ELobbyType : int
    {
        k_ELobbyTypePrivate = 0,        // only way to join the lobby is to invite to someone else
        k_ELobbyTypeFriendsOnly = 1,    // shows for friends or invitees, but not in lobby list
        k_ELobbyTypePublic = 2,         // visible for friends and in lobby list
        k_ELobbyTypeInvisible = 3,      // returned by search, but not visible to other friends
                                        //    useful if you want a user in two lobbies, for example matching groups together
                                        //	  a user can be in only one regular lobby, and up to two invisible lobbies
    }

    // lobby search filter tools
    public enum ELobbyComparison : int
    {
        k_ELobbyComparisonEqualToOrLessThan = -2,
        k_ELobbyComparisonLessThan = -1,
        k_ELobbyComparisonEqual = 0,
        k_ELobbyComparisonGreaterThan = 1,
        k_ELobbyComparisonEqualToOrGreaterThan = 2,
        k_ELobbyComparisonNotEqual = 3,
    }

    // lobby search distance. Lobby results are sorted from closest to farthest.
    public enum ELobbyDistanceFilter : int
    {
        k_ELobbyDistanceFilterClose,        // only lobbies in the same immediate region will be returned
        k_ELobbyDistanceFilterDefault,      // only lobbies in the same region or near by regions
        k_ELobbyDistanceFilterFar,          // for games that don't have many latency requirements, will return lobbies about half-way around the globe
        k_ELobbyDistanceFilterWorldwide,    // no filtering, will match lobbies as far as India to NY (not recommended, expect multiple seconds of latency between the clients)
    }

    //-----------------------------------------------------------------------------
    // Purpose: Used in ChatInfo messages - fields specific to a chat member - must fit in a uint32
    //-----------------------------------------------------------------------------
    [Flags]
    public enum EChatMemberStateChange : int
    {
        // Specific to joining / leaving the chatroom
        k_EChatMemberStateChangeEntered = 0x0001,       // This user has joined or is joining the chat room
        k_EChatMemberStateChangeLeft = 0x0002,      // This user has left or is leaving the chat room
        k_EChatMemberStateChangeDisconnected = 0x0004,      // User disconnected without leaving the chat first
        k_EChatMemberStateChangeKicked = 0x0008,        // User kicked
        k_EChatMemberStateChangeBanned = 0x0010,        // User kicked and banned
    }

    //-----------------------------------------------------------------------------
    // Purpose:
    //-----------------------------------------------------------------------------
    public enum AudioPlayback_Status : int
    {
        AudioPlayback_Undefined = 0,
        AudioPlayback_Playing = 1,
        AudioPlayback_Paused = 2,
        AudioPlayback_Idle = 3
    }

    // list of possible errors returned by SendP2PPacket() API
    // these will be posted in the P2PSessionConnectFail_t callback
    public enum EP2PSessionError : int
    {
        k_EP2PSessionErrorNone = 0,
        k_EP2PSessionErrorNotRunningApp = 1,            // target is not running the same game
        k_EP2PSessionErrorNoRightsToApp = 2,            // local user doesn't own the app that is running
        k_EP2PSessionErrorDestinationNotLoggedIn = 3,   // target user isn't connected to Steam
        k_EP2PSessionErrorTimeout = 4,                  // target isn't responding, perhaps not calling AcceptP2PSessionWithUser()
                                                        // corporate firewalls can also block this (NAT traversal is not firewall traversal)
                                                        // make sure that UDP ports 3478, 4379, and 4380 are open in an outbound direction
        k_EP2PSessionErrorMax = 5
    }

    // SendP2PPacket() send types
    // Typically k_EP2PSendUnreliable is what you want for UDP-like packets, k_EP2PSendReliable for TCP-like packets
    public enum EP2PSend : int
    {
        // Basic UDP send. Packets can't be bigger than 1200 bytes (your typical MTU size). Can be lost, or arrive out of order (rare).
        // The sending API does have some knowledge of the underlying connection, so if there is no NAT-traversal accomplished or
        // there is a recognized adjustment happening on the connection, the packet will be batched until the connection is open again.
        k_EP2PSendUnreliable = 0,

        // As above, but if the underlying p2p connection isn't yet established the packet will just be thrown away. Using this on the first
        // packet sent to a remote host almost guarantees the packet will be dropped.
        // This is only really useful for kinds of data that should never buffer up, i.e. voice payload packets
        k_EP2PSendUnreliableNoDelay = 1,

        // Reliable message send. Can send up to 1MB of data in a single message.
        // Does fragmentation/re-assembly of messages under the hood, as well as a sliding window for efficient sends of large chunks of data.
        k_EP2PSendReliable = 2,

        // As above, but applies the Nagle algorithm to the send - sends will accumulate
        // until the current MTU size (typically ~1200 bytes, but can change) or ~200ms has passed (Nagle algorithm).
        // Useful if you want to send a set of smaller messages but have the coalesced into a single packet
        // Since the reliable stream is all ordered, you can do several small message sends with k_EP2PSendReliableWithBuffering and then
        // do a normal k_EP2PSendReliable to force all the buffered data to be sent.
        k_EP2PSendReliableWithBuffering = 3,

    }

    // connection progress indicators, used by CreateP2PConnectionSocket()
    public enum ESNetSocketState : int
    {
        k_ESNetSocketStateInvalid = 0,

        // communication is valid
        k_ESNetSocketStateConnected = 1,

        // states while establishing a connection
        k_ESNetSocketStateInitiated = 10,               // the connection state machine has started

        // p2p connections
        k_ESNetSocketStateLocalCandidatesFound = 11,    // we've found our local IP info
        k_ESNetSocketStateReceivedRemoteCandidates = 12,// we've received information from the remote machine, via the Steam back-end, about their IP info

        // direct connections
        k_ESNetSocketStateChallengeHandshake = 15,      // we've received a challenge packet from the server

        // failure states
        k_ESNetSocketStateDisconnecting = 21,           // the API shut it down, and we're in the process of telling the other end
        k_ESNetSocketStateLocalDisconnect = 22,         // the API shut it down, and we've completed shutdown
        k_ESNetSocketStateTimeoutDuringConnect = 23,    // we timed out while trying to creating the connection
        k_ESNetSocketStateRemoteEndDisconnected = 24,   // the remote end has disconnected from us
        k_ESNetSocketStateConnectionBroken = 25,        // connection has been broken; either the other end has disappeared or our local network connection has broke

    }

    // describes how the socket is currently connected
    public enum ESNetSocketConnectionType : int
    {
        k_ESNetSocketConnectionTypeNotConnected = 0,
        k_ESNetSocketConnectionTypeUDP = 1,
        k_ESNetSocketConnectionTypeUDPRelay = 2,
    }

    // Ways to handle a synchronization conflict
    public enum EResolveConflict : int
    {
        k_EResolveConflictKeepClient = 1,       // The local version of each file will be used to overwrite the server version
        k_EResolveConflictKeepServer = 2,       // The server version of each file will be used to overwrite the local version
    }

    [Flags]
    public enum ERemoteStoragePlatform : int
    {
        k_ERemoteStoragePlatformNone = 0,
        k_ERemoteStoragePlatformWindows = (1 << 0),
        k_ERemoteStoragePlatformOSX = (1 << 1),
        k_ERemoteStoragePlatformPS3 = (1 << 2),
        k_ERemoteStoragePlatformLinux = (1 << 3),
        k_ERemoteStoragePlatformReserved2 = (1 << 4),

        k_ERemoteStoragePlatformAll = -1
    }

    public enum ERemoteStoragePublishedFileVisibility : int
    {
        k_ERemoteStoragePublishedFileVisibilityPublic = 0,
        k_ERemoteStoragePublishedFileVisibilityFriendsOnly = 1,
        k_ERemoteStoragePublishedFileVisibilityPrivate = 2,
    }

    public enum EWorkshopFileType : int
    {
        k_EWorkshopFileTypeFirst = 0,

        k_EWorkshopFileTypeCommunity = 0,       // normal Workshop item that can be subscribed to
        k_EWorkshopFileTypeMicrotransaction = 1,        // Workshop item that is meant to be voted on for the purpose of selling in-game
        k_EWorkshopFileTypeCollection = 2,      // a collection of Workshop or Greenlight items
        k_EWorkshopFileTypeArt = 3,     // artwork
        k_EWorkshopFileTypeVideo = 4,       // external video
        k_EWorkshopFileTypeScreenshot = 5,      // screenshot
        k_EWorkshopFileTypeGame = 6,        // Greenlight game entry
        k_EWorkshopFileTypeSoftware = 7,        // Greenlight software entry
        k_EWorkshopFileTypeConcept = 8,     // Greenlight concept
        k_EWorkshopFileTypeWebGuide = 9,        // Steam web guide
        k_EWorkshopFileTypeIntegratedGuide = 10,        // application integrated guide
        k_EWorkshopFileTypeMerch = 11,      // Workshop merchandise meant to be voted on for the purpose of being sold
        k_EWorkshopFileTypeControllerBinding = 12,      // Steam Controller bindings
        k_EWorkshopFileTypeSteamworksAccessInvite = 13,     // internal
        k_EWorkshopFileTypeSteamVideo = 14,     // Steam video
        k_EWorkshopFileTypeGameManagedItem = 15,        // managed completely by the game, not the user, and not shown on the web

        // Update k_EWorkshopFileTypeMax if you add values.
        k_EWorkshopFileTypeMax = 16

    }

    public enum EWorkshopVote : int
    {
        k_EWorkshopVoteUnvoted = 0,
        k_EWorkshopVoteFor = 1,
        k_EWorkshopVoteAgainst = 2,
        k_EWorkshopVoteLater = 3,
    }

    public enum EWorkshopFileAction : int
    {
        k_EWorkshopFileActionPlayed = 0,
        k_EWorkshopFileActionCompleted = 1,
    }

    public enum EWorkshopEnumerationType : int
    {
        k_EWorkshopEnumerationTypeRankedByVote = 0,
        k_EWorkshopEnumerationTypeRecent = 1,
        k_EWorkshopEnumerationTypeTrending = 2,
        k_EWorkshopEnumerationTypeFavoritesOfFriends = 3,
        k_EWorkshopEnumerationTypeVotedByFriends = 4,
        k_EWorkshopEnumerationTypeContentByFriends = 5,
        k_EWorkshopEnumerationTypeRecentFromFollowedUsers = 6,
    }

    public enum EWorkshopVideoProvider : int
    {
        k_EWorkshopVideoProviderNone = 0,
        k_EWorkshopVideoProviderYoutube = 1
    }

    public enum EUGCReadAction : int
    {
        // Keeps the file handle open unless the last byte is read.  You can use this when reading large files (over 100MB) in sequential chunks.
        // If the last byte is read, this will behave the same as k_EUGCRead_Close.  Otherwise, it behaves the same as k_EUGCRead_ContinueReading.
        // This value maintains the same behavior as before the EUGCReadAction parameter was introduced.
        k_EUGCRead_ContinueReadingUntilFinished = 0,

        // Keeps the file handle open.  Use this when using UGCRead to seek to different parts of the file.
        // When you are done seeking around the file, make a final call with k_EUGCRead_Close to close it.
        k_EUGCRead_ContinueReading = 1,

        // Frees the file handle.  Use this when you're done reading the content.
        // To read the file from Steam again you will need to call UGCDownload again.
        k_EUGCRead_Close = 2,
    }

    // Matching UGC types for queries
    public enum EUGCMatchingUGCType : int
    {
        k_EUGCMatchingUGCType_Items = 0,        // both mtx items and ready-to-use items
        k_EUGCMatchingUGCType_Items_Mtx = 1,
        k_EUGCMatchingUGCType_Items_ReadyToUse = 2,
        k_EUGCMatchingUGCType_Collections = 3,
        k_EUGCMatchingUGCType_Artwork = 4,
        k_EUGCMatchingUGCType_Videos = 5,
        k_EUGCMatchingUGCType_Screenshots = 6,
        k_EUGCMatchingUGCType_AllGuides = 7,        // both web guides and integrated guides
        k_EUGCMatchingUGCType_WebGuides = 8,
        k_EUGCMatchingUGCType_IntegratedGuides = 9,
        k_EUGCMatchingUGCType_UsableInGame = 10,        // ready-to-use items and integrated guides
        k_EUGCMatchingUGCType_ControllerBindings = 11,
        k_EUGCMatchingUGCType_GameManagedItems = 12,        // game managed items (not managed by users)
    }

    // Sort order for user published UGC lists (defaults to creation order descending)
    public enum EUserUGCListSortOrder : int
    {
        k_EUserUGCListSortOrder_CreationOrderDesc,
        k_EUserUGCListSortOrder_CreationOrderAsc,
        k_EUserUGCListSortOrder_TitleAsc,
        k_EUserUGCListSortOrder_LastUpdatedDesc,
        k_EUserUGCListSortOrder_SubscriptionDateDesc,
        k_EUserUGCListSortOrder_VoteScoreDesc,
        k_EUserUGCListSortOrder_ForModeration,
    }

    // Combination of sorting and filtering for queries across all UGC
    public enum EUGCQuery : int
    {
        k_EUGCQuery_RankedByVote = 0,
        k_EUGCQuery_RankedByPublicationDate = 1,
        k_EUGCQuery_AcceptedForGameRankedByAcceptanceDate = 2,
        k_EUGCQuery_RankedByTrend = 3,
        k_EUGCQuery_FavoritedByFriendsRankedByPublicationDate = 4,
        k_EUGCQuery_CreatedByFriendsRankedByPublicationDate = 5,
        k_EUGCQuery_RankedByNumTimesReported = 6,
        k_EUGCQuery_CreatedByFollowedUsersRankedByPublicationDate = 7,
        k_EUGCQuery_NotYetRated = 8,
        k_EUGCQuery_RankedByTotalVotesAsc = 9,
        k_EUGCQuery_RankedByVotesUp = 10,
        k_EUGCQuery_RankedByTextSearch = 11,
        k_EUGCQuery_RankedByTotalUniqueSubscriptions = 12,
    }

    public enum EItemUpdateStatus : int
    {
        k_EItemUpdateStatusInvalid = 0, // The item update handle was invalid, job might be finished, listen too SubmitItemUpdateResult_t
        k_EItemUpdateStatusPreparingConfig = 1, // The item update is processing configuration data
        k_EItemUpdateStatusPreparingContent = 2, // The item update is reading and processing content files
        k_EItemUpdateStatusUploadingContent = 3, // The item update is uploading content changes to Steam
        k_EItemUpdateStatusUploadingPreviewFile = 4, // The item update is uploading new preview file image
        k_EItemUpdateStatusCommittingChanges = 5  // The item update is committing all changes
    }

    [Flags]
    public enum EItemState : int
    {
        k_EItemStateNone = 0,   // item not tracked on client
        k_EItemStateSubscribed = 1, // current user is subscribed to this item. Not just cached.
        k_EItemStateLegacyItem = 2, // item was created with ISteamRemoteStorage
        k_EItemStateInstalled = 4,  // item is installed and usable (but maybe out of date)
        k_EItemStateNeedsUpdate = 8,    // items needs an update. Either because it's not installed yet or creator updated content
        k_EItemStateDownloading = 16,   // item update is currently downloading
        k_EItemStateDownloadPending = 32,   // DownloadItem() was called for this item, content isn't available until DownloadItemResult_t is fired
    }

    public enum EItemStatistic : int
    {
        k_EItemStatistic_NumSubscriptions = 0,
        k_EItemStatistic_NumFavorites = 1,
        k_EItemStatistic_NumFollowers = 2,
        k_EItemStatistic_NumUniqueSubscriptions = 3,
        k_EItemStatistic_NumUniqueFavorites = 4,
        k_EItemStatistic_NumUniqueFollowers = 5,
        k_EItemStatistic_NumUniqueWebsiteViews = 6,
        k_EItemStatistic_ReportScore = 7,
    }

    public enum EFailureType : int
    {
        k_EFailureFlushedCallbackQueue,
        k_EFailurePipeFail,
    }

    // type of data request, when downloading leaderboard entries
    public enum ELeaderboardDataRequest : int
    {
        k_ELeaderboardDataRequestGlobal = 0,
        k_ELeaderboardDataRequestGlobalAroundUser = 1,
        k_ELeaderboardDataRequestFriends = 2,
        k_ELeaderboardDataRequestUsers = 3
    }

    // the sort order of a leaderboard
    public enum ELeaderboardSortMethod : int
    {
        k_ELeaderboardSortMethodNone = 0,
        k_ELeaderboardSortMethodAscending = 1,  // top-score is lowest number
        k_ELeaderboardSortMethodDescending = 2, // top-score is highest number
    }

    // the display type (used by the Steam Community web site) for a leaderboard
    public enum ELeaderboardDisplayType : int
    {
        k_ELeaderboardDisplayTypeNone = 0,
        k_ELeaderboardDisplayTypeNumeric = 1,           // simple numerical score
        k_ELeaderboardDisplayTypeTimeSeconds = 2,       // the score represents a time, in seconds
        k_ELeaderboardDisplayTypeTimeMilliSeconds = 3,  // the score represents a time, in milliseconds
    }

    public enum ELeaderboardUploadScoreMethod : int
    {
        k_ELeaderboardUploadScoreMethodNone = 0,
        k_ELeaderboardUploadScoreMethodKeepBest = 1,    // Leaderboard will keep user's best score
        k_ELeaderboardUploadScoreMethodForceUpdate = 2, // Leaderboard will always replace score with specified
    }

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
    }

    // Input modes for the Big Picture gamepad text entry
    public enum EGamepadTextInputMode : int
    {
        k_EGamepadTextInputModeNormal = 0,
        k_EGamepadTextInputModePassword = 1
    }

    // Controls number of allowed lines for the Big Picture gamepad text entry
    public enum EGamepadTextInputLineMode : int
    {
        k_EGamepadTextInputLineModeSingleLine = 0,
        k_EGamepadTextInputLineModeMultipleLines = 1
    }

    //-----------------------------------------------------------------------------
    // results for CheckFileSignature
    //-----------------------------------------------------------------------------
    public enum ECheckFileSignature : int
    {
        k_ECheckFileSignatureInvalidSignature = 0,
        k_ECheckFileSignatureValidSignature = 1,
        k_ECheckFileSignatureFileNotFound = 2,
        k_ECheckFileSignatureNoSignaturesFoundForThisApp = 3,
        k_ECheckFileSignatureNoSignaturesFoundForThisFile = 4,
    }

    public enum EMatchMakingServerResponse : int
    {
        eServerResponded = 0,
        eServerFailedToRespond,
        eNoServersListedOnMasterServer // for the Internet query type, returned in response callback if no servers of this type match
    }

    public enum EServerMode : int
    {
        eServerModeInvalid = 0, // DO NOT USE
        eServerModeNoAuthentication = 1, // Don't authenticate user logins and don't list on the server list
        eServerModeAuthentication = 2, // Authenticate users, list on the server list, don't run VAC on clients that connect
        eServerModeAuthenticationAndSecure = 3, // Authenticate users, list on the server list and VAC protect clients
    }

    // General result codes
    public enum EResult : int
    {
        k_EResultOK = 1,                            // success
        k_EResultFail = 2,                          // generic failure
        k_EResultNoConnection = 3,                  // no/failed network connection
                                                    //	k_EResultNoConnectionRetry = 4,				// OBSOLETE - removed
        k_EResultInvalidPassword = 5,               // password/ticket is invalid
        k_EResultLoggedInElsewhere = 6,             // same user logged in elsewhere
        k_EResultInvalidProtocolVer = 7,            // protocol version is incorrect
        k_EResultInvalidParam = 8,                  // a parameter is incorrect
        k_EResultFileNotFound = 9,                  // file was not found
        k_EResultBusy = 10,                         // called method busy - action not taken
        k_EResultInvalidState = 11,                 // called object was in an invalid state
        k_EResultInvalidName = 12,                  // name is invalid
        k_EResultInvalidEmail = 13,                 // email is invalid
        k_EResultDuplicateName = 14,                // name is not unique
        k_EResultAccessDenied = 15,                 // access is denied
        k_EResultTimeout = 16,                      // operation timed out
        k_EResultBanned = 17,                       // VAC2 banned
        k_EResultAccountNotFound = 18,              // account not found
        k_EResultInvalidSteamID = 19,               // steamID is invalid
        k_EResultServiceUnavailable = 20,           // The requested service is currently unavailable
        k_EResultNotLoggedOn = 21,                  // The user is not logged on
        k_EResultPending = 22,                      // Request is pending (may be in process, or waiting on third party)
        k_EResultEncryptionFailure = 23,            // Encryption or Decryption failed
        k_EResultInsufficientPrivilege = 24,        // Insufficient privilege
        k_EResultLimitExceeded = 25,                // Too much of a good thing
        k_EResultRevoked = 26,                      // Access has been revoked (used for revoked guest passes)
        k_EResultExpired = 27,                      // License/Guest pass the user is trying to access is expired
        k_EResultAlreadyRedeemed = 28,              // Guest pass has already been redeemed by account, cannot be acked again
        k_EResultDuplicateRequest = 29,             // The request is a duplicate and the action has already occurred in the past, ignored this time
        k_EResultAlreadyOwned = 30,                 // All the games in this guest pass redemption request are already owned by the user
        k_EResultIPNotFound = 31,                   // IP address not found
        k_EResultPersistFailed = 32,                // failed to write change to the data store
        k_EResultLockingFailed = 33,                // failed to acquire access lock for this operation
        k_EResultLogonSessionReplaced = 34,
        k_EResultConnectFailed = 35,
        k_EResultHandshakeFailed = 36,
        k_EResultIOFailure = 37,
        k_EResultRemoteDisconnect = 38,
        k_EResultShoppingCartNotFound = 39,         // failed to find the shopping cart requested
        k_EResultBlocked = 40,                      // a user didn't allow it
        k_EResultIgnored = 41,                      // target is ignoring sender
        k_EResultNoMatch = 42,                      // nothing matching the request found
        k_EResultAccountDisabled = 43,
        k_EResultServiceReadOnly = 44,              // this service is not accepting content changes right now
        k_EResultAccountNotFeatured = 45,           // account doesn't have value, so this feature isn't available
        k_EResultAdministratorOK = 46,              // allowed to take this action, but only because requester is admin
        k_EResultContentVersion = 47,               // A Version mismatch in content transmitted within the Steam protocol.
        k_EResultTryAnotherCM = 48,                 // The current CM can't service the user making a request, user should try another.
        k_EResultPasswordRequiredToKickSession = 49,// You are already logged in elsewhere, this cached credential login has failed.
        k_EResultAlreadyLoggedInElsewhere = 50,     // You are already logged in elsewhere, you must wait
        k_EResultSuspended = 51,                    // Long running operation (content download) suspended/paused
        k_EResultCancelled = 52,                    // Operation canceled (typically by user: content download)
        k_EResultDataCorruption = 53,               // Operation canceled because data is ill formed or unrecoverable
        k_EResultDiskFull = 54,                     // Operation canceled - not enough disk space.
        k_EResultRemoteCallFailed = 55,             // an remote call or IPC call failed
        k_EResultPasswordUnset = 56,                // Password could not be verified as it's unset server side
        k_EResultExternalAccountUnlinked = 57,      // External account (PSN, Facebook...) is not linked to a Steam account
        k_EResultPSNTicketInvalid = 58,             // PSN ticket was invalid
        k_EResultExternalAccountAlreadyLinked = 59, // External account (PSN, Facebook...) is already linked to some other account, must explicitly request to replace/delete the link first
        k_EResultRemoteFileConflict = 60,           // The sync cannot resume due to a conflict between the local and remote files
        k_EResultIllegalPassword = 61,              // The requested new password is not legal
        k_EResultSameAsPreviousValue = 62,          // new value is the same as the old one ( secret question and answer )
        k_EResultAccountLogonDenied = 63,           // account login denied due to 2nd factor authentication failure
        k_EResultCannotUseOldPassword = 64,         // The requested new password is not legal
        k_EResultInvalidLoginAuthCode = 65,         // account login denied due to auth code invalid
        k_EResultAccountLogonDeniedNoMail = 66,     // account login denied due to 2nd factor auth failure - and no mail has been sent
        k_EResultHardwareNotCapableOfIPT = 67,      //
        k_EResultIPTInitError = 68,                 //
        k_EResultParentalControlRestricted = 69,    // operation failed due to parental control restrictions for current user
        k_EResultFacebookQueryError = 70,           // Facebook query returned an error
        k_EResultExpiredLoginAuthCode = 71,         // account login denied due to auth code expired
        k_EResultIPLoginRestrictionFailed = 72,
        k_EResultAccountLockedDown = 73,
        k_EResultAccountLogonDeniedVerifiedEmailRequired = 74,
        k_EResultNoMatchingURL = 75,
        k_EResultBadResponse = 76,                  // parse failure, missing field, etc.
        k_EResultRequirePasswordReEntry = 77,       // The user cannot complete the action until they re-enter their password
        k_EResultValueOutOfRange = 78,              // the value entered is outside the acceptable range
        k_EResultUnexpectedError = 79,              // something happened that we didn't expect to ever happen
        k_EResultDisabled = 80,                     // The requested service has been configured to be unavailable
        k_EResultInvalidCEGSubmission = 81,         // The set of files submitted to the CEG server are not valid !
        k_EResultRestrictedDevice = 82,             // The device being used is not allowed to perform this action
        k_EResultRegionLocked = 83,                 // The action could not be complete because it is region restricted
        k_EResultRateLimitExceeded = 84,            // Temporary rate limit exceeded, try again later, different from k_EResultLimitExceeded which may be permanent
        k_EResultAccountLoginDeniedNeedTwoFactor = 85,  // Need two-factor code to login
        k_EResultItemDeleted = 86,                  // The thing we're trying to access has been deleted
        k_EResultAccountLoginDeniedThrottle = 87,   // login attempt failed, try to throttle response to possible attacker
        k_EResultTwoFactorCodeMismatch = 88,        // two factor code mismatch
        k_EResultTwoFactorActivationCodeMismatch = 89,  // activation code for two-factor didn't match
        k_EResultAccountAssociatedToMultiplePartners = 90,  // account has been associated with multiple partners
        k_EResultNotModified = 91,                  // data not modified
        k_EResultNoMobileDevice = 92,               // the account does not have a mobile device associated with it
        k_EResultTimeNotSynced = 93,                // the time presented is out of range or tolerance
        k_EResultSmsCodeFailed = 94,                // SMS code failure (no match, none pending, etc.)
        k_EResultAccountLimitExceeded = 95,         // Too many accounts access this resource
        k_EResultAccountActivityLimitExceeded = 96, // Too many changes to this account
        k_EResultPhoneActivityLimitExceeded = 97,   // Too many changes to this phone
        k_EResultRefundToWallet = 98,               // Cannot refund to payment method, must use wallet
        k_EResultEmailSendFailure = 99,             // Cannot send an email
        k_EResultNotSettled = 100,                  // Can't perform operation till payment has settled
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

    }

    // Result codes to GSHandleClientDeny/Kick
    public enum EDenyReason : int
    {
        k_EDenyInvalid = 0,
        k_EDenyInvalidVersion = 1,
        k_EDenyGeneric = 2,
        k_EDenyNotLoggedOn = 3,
        k_EDenyNoLicense = 4,
        k_EDenyCheater = 5,
        k_EDenyLoggedInElseWhere = 6,
        k_EDenyUnknownText = 7,
        k_EDenyIncompatibleAnticheat = 8,
        k_EDenyMemoryCorruption = 9,
        k_EDenyIncompatibleSoftware = 10,
        k_EDenySteamConnectionLost = 11,
        k_EDenySteamConnectionError = 12,
        k_EDenySteamResponseTimedOut = 13,
        k_EDenySteamValidationStalled = 14,
        k_EDenySteamOwnerLeftGuestUser = 15,
    }

    // results from BeginAuthSession
    public enum EBeginAuthSessionResult : int
    {
        k_EBeginAuthSessionResultOK = 0,                        // Ticket is valid for this game and this steamID.
        k_EBeginAuthSessionResultInvalidTicket = 1,             // Ticket is not valid.
        k_EBeginAuthSessionResultDuplicateRequest = 2,          // A ticket has already been submitted for this steamID
        k_EBeginAuthSessionResultInvalidVersion = 3,            // Ticket is from an incompatible interface version
        k_EBeginAuthSessionResultGameMismatch = 4,              // Ticket is not for this game
        k_EBeginAuthSessionResultExpiredTicket = 5,             // Ticket has expired
    }

    // Callback values for callback ValidateAuthTicketResponse_t which is a response to BeginAuthSession
    public enum EAuthSessionResponse : int
    {
        k_EAuthSessionResponseOK = 0,                           // Steam has verified the user is online, the ticket is valid and ticket has not been reused.
        k_EAuthSessionResponseUserNotConnectedToSteam = 1,      // The user in question is not connected to steam
        k_EAuthSessionResponseNoLicenseOrExpired = 2,           // The license has expired.
        k_EAuthSessionResponseVACBanned = 3,                    // The user is VAC banned for this game.
        k_EAuthSessionResponseLoggedInElseWhere = 4,            // The user account has logged in elsewhere and the session containing the game instance has been disconnected.
        k_EAuthSessionResponseVACCheckTimedOut = 5,             // VAC has been unable to perform anti-cheat checks on this user
        k_EAuthSessionResponseAuthTicketCanceled = 6,           // The ticket has been canceled by the issuer
        k_EAuthSessionResponseAuthTicketInvalidAlreadyUsed = 7, // This ticket has already been used, it is not valid.
        k_EAuthSessionResponseAuthTicketInvalid = 8,            // This ticket is not from a user instance currently connected to steam.
        k_EAuthSessionResponsePublisherIssuedBan = 9,           // The user is banned for this game. The ban came via the web api and not VAC
    }

    // results from UserHasLicenseForApp
    public enum EUserHasLicenseForAppResult : int
    {
        k_EUserHasLicenseResultHasLicense = 0,                  // User has a license for specified app
        k_EUserHasLicenseResultDoesNotHaveLicense = 1,          // User does not have a license for the specified app
        k_EUserHasLicenseResultNoAuth = 2,                      // User has not been authenticated
    }

    // Steam account types
    public enum EAccountType : int
    {
        k_EAccountTypeInvalid = 0,
        k_EAccountTypeIndividual = 1,       // single user account
        k_EAccountTypeMultiseat = 2,        // multiseat (e.g. cybercafe) account
        k_EAccountTypeGameServer = 3,       // game server account
        k_EAccountTypeAnonGameServer = 4,   // anonymous game server account
        k_EAccountTypePending = 5,          // pending
        k_EAccountTypeContentServer = 6,    // content server
        k_EAccountTypeClan = 7,
        k_EAccountTypeChat = 8,
        k_EAccountTypeConsoleUser = 9,      // Fake SteamID for local PSN account on PS3 or Live account on 360, etc.
        k_EAccountTypeAnonUser = 10,

        // Max of 16 items in this field
        k_EAccountTypeMax
    }

    //-----------------------------------------------------------------------------
    // Purpose:
    //-----------------------------------------------------------------------------
    public enum EAppReleaseState : int
    {
        k_EAppReleaseState_Unknown = 0, // unknown, required appinfo or license info is missing
        k_EAppReleaseState_Unavailable = 1, // even if user 'just' owns it, can see game at all
        k_EAppReleaseState_Prerelease = 2,  // can be purchased and is visible in games list, nothing else. Common appInfo section released
        k_EAppReleaseState_PreloadOnly = 3, // owners can preload app, not play it. AppInfo fully released.
        k_EAppReleaseState_Released = 4,    // owners can download and play app.
    }

    //-----------------------------------------------------------------------------
    // Purpose:
    //-----------------------------------------------------------------------------
    [Flags]
    public enum EAppOwnershipFlags : int
    {
        k_EAppOwnershipFlags_None = 0x0000, // unknown
        k_EAppOwnershipFlags_OwnsLicense = 0x0001,  // owns license for this game
        k_EAppOwnershipFlags_FreeLicense = 0x0002,  // not paid for game
        k_EAppOwnershipFlags_RegionRestricted = 0x0004, // owns app, but not allowed to play in current region
        k_EAppOwnershipFlags_LowViolence = 0x0008,  // only low violence version
        k_EAppOwnershipFlags_InvalidPlatform = 0x0010,  // app not supported on current platform
        k_EAppOwnershipFlags_SharedLicense = 0x0020,    // license was granted by authorized local device
        k_EAppOwnershipFlags_FreeWeekend = 0x0040,  // owned by a free weekend licenses
        k_EAppOwnershipFlags_RetailLicense = 0x0080,    // has a retail license for game, (CD-Key etc)
        k_EAppOwnershipFlags_LicenseLocked = 0x0100,    // shared license is locked (in use) by other user
        k_EAppOwnershipFlags_LicensePending = 0x0200,   // owns app, but transaction is still pending. Can't install or play
        k_EAppOwnershipFlags_LicenseExpired = 0x0400,   // doesn't own app anymore since license expired
        k_EAppOwnershipFlags_LicensePermanent = 0x0800, // permanent license, not borrowed, or guest or freeweekend etc
        k_EAppOwnershipFlags_LicenseRecurring = 0x1000, // Recurring license, user is charged periodically
        k_EAppOwnershipFlags_LicenseCanceled = 0x2000,  // Mark as canceled, but might be still active if recurring
        k_EAppOwnershipFlags_AutoGrant = 0x4000,    // Ownership is based on any kind of autogrant license
    }

    //-----------------------------------------------------------------------------
    // Purpose: designed as flags to allow filters masks
    //-----------------------------------------------------------------------------
    [Flags]
    public enum EAppType : int
    {
        k_EAppType_Invalid = 0x000, // unknown / invalid
        k_EAppType_Game = 0x001,    // playable game, default type
        k_EAppType_Application = 0x002, // software application
        k_EAppType_Tool = 0x004,    // SDKs, editors & dedicated servers
        k_EAppType_Demo = 0x008,    // game demo
        k_EAppType_Media_DEPRECATED = 0x010,    // legacy - was used for game trailers, which are now just videos on the web
        k_EAppType_DLC = 0x020, // down loadable content
        k_EAppType_Guide = 0x040,   // game guide, PDF etc
        k_EAppType_Driver = 0x080,  // hardware driver updater (ATI, Razor etc)
        k_EAppType_Config = 0x100,  // hidden app used to config Steam features (backpack, sales, etc)
        k_EAppType_Hardware = 0x200,    // a hardware device (Steam Machine, Steam Controller, Steam Link, etc.)
                                        // 0x400 is up for grabs here
        k_EAppType_Video = 0x800,   // A video component of either a Film or TVSeries (may be the feature, an episode, preview, making-of, etc)
        k_EAppType_Plugin = 0x1000, // Plug-in types for other Apps
        k_EAppType_Music = 0x2000,  // Music files

        k_EAppType_Shortcut = 0x40000000,   // just a shortcut, client side only
        k_EAppType_DepotOnly = -2147483647, // placeholder since depots and apps share the same namespace
    }

    //-----------------------------------------------------------------------------
    // types of user game stats fields
    // WARNING: DO NOT RENUMBER EXISTING VALUES - STORED IN DATABASE
    //-----------------------------------------------------------------------------
    public enum ESteamUserStatType : int
    {
        k_ESteamUserStatTypeINVALID = 0,
        k_ESteamUserStatTypeINT = 1,
        k_ESteamUserStatTypeFLOAT = 2,
        // Read as FLOAT, set with count / session length
        k_ESteamUserStatTypeAVGRATE = 3,
        k_ESteamUserStatTypeACHIEVEMENTS = 4,
        k_ESteamUserStatTypeGROUPACHIEVEMENTS = 5,

        // max, for sanity checks
        k_ESteamUserStatTypeMAX
    }

    //-----------------------------------------------------------------------------
    // Purpose: Chat Entry Types (previously was only friend-to-friend message types)
    //-----------------------------------------------------------------------------
    public enum EChatEntryType : int
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
        k_EChatEntryTypeReserved1 = 12,
        k_EChatEntryTypeReserved2 = 13,
        k_EChatEntryTypeLinkBlocked = 14, // a link was removed by the chat filter.
    }

    //-----------------------------------------------------------------------------
    // Purpose: Chat Room Enter Responses
    //-----------------------------------------------------------------------------
    public enum EChatRoomEnterResponse : int
    {
        k_EChatRoomEnterResponseSuccess = 1,        // Success
        k_EChatRoomEnterResponseDoesntExist = 2,    // Chat doesn't exist (probably closed)
        k_EChatRoomEnterResponseNotAllowed = 3,     // General Denied - You don't have the permissions needed to join the chat
        k_EChatRoomEnterResponseFull = 4,           // Chat room has reached its maximum size
        k_EChatRoomEnterResponseError = 5,          // Unexpected Error
        k_EChatRoomEnterResponseBanned = 6,         // You are banned from this chat room and may not join
        k_EChatRoomEnterResponseLimited = 7,        // Joining this chat is not allowed because you are a limited user (no value on account)
        k_EChatRoomEnterResponseClanDisabled = 8,   // Attempt to join a clan chat when the clan is locked or disabled
        k_EChatRoomEnterResponseCommunityBan = 9,   // Attempt to join a chat when the user has a community lock on their account
        k_EChatRoomEnterResponseMemberBlockedYou = 10, // Join failed - some member in the chat has blocked you from joining
        k_EChatRoomEnterResponseYouBlockedMember = 11, // Join failed - you have blocked some member already in the chat
                                                       // k_EChatRoomEnterResponseNoRankingDataLobby = 12,  // No longer used
                                                       // k_EChatRoomEnterResponseNoRankingDataUser = 13,  //  No longer used
                                                       // k_EChatRoomEnterResponseRankOutOfRange = 14, //  No longer used
    }

    // Special flags for Chat accounts - they go in the top 8 bits
    // of the steam ID's "instance", leaving 12 for the actual instances
    [Flags]
    public enum EChatSteamIDInstanceFlags : int
    {
        k_EChatAccountInstanceMask = 0x00000FFF, // top 8 bits are flags

        k_EChatInstanceFlagClan = (Constants.k_unSteamAccountInstanceMask + 1) >> 1,    // top bit
        k_EChatInstanceFlagLobby = (Constants.k_unSteamAccountInstanceMask + 1) >> 2,   // next one down, etc
        k_EChatInstanceFlagMMSLobby = (Constants.k_unSteamAccountInstanceMask + 1) >> 3,    // next one down, etc

        // Max of 8 flags
    }

    //-----------------------------------------------------------------------------
    // Purpose: Marketing message flags that change how a client should handle them
    //-----------------------------------------------------------------------------
    [Flags]
    public enum EMarketingMessageFlags : int
    {
        k_EMarketingMessageFlagsNone = 0,
        k_EMarketingMessageFlagsHighPriority = 1 << 0,
        k_EMarketingMessageFlagsPlatformWindows = 1 << 1,
        k_EMarketingMessageFlagsPlatformMac = 1 << 2,
        k_EMarketingMessageFlagsPlatformLinux = 1 << 3,

        //aggregate flags
        k_EMarketingMessageFlagsPlatformRestrictions =
        k_EMarketingMessageFlagsPlatformWindows |
        k_EMarketingMessageFlagsPlatformMac |
        k_EMarketingMessageFlagsPlatformLinux,
    }

    //-----------------------------------------------------------------------------
    // Purpose: Possible positions to tell the overlay to show notifications in
    //-----------------------------------------------------------------------------
    public enum ENotificationPosition : int
    {
        k_EPositionTopLeft = 0,
        k_EPositionTopRight = 1,
        k_EPositionBottomLeft = 2,
        k_EPositionBottomRight = 3,
    }

    //-----------------------------------------------------------------------------
    // Purpose: Broadcast upload result details
    //-----------------------------------------------------------------------------
    public enum EBroadcastUploadResult : int
    {
        k_EBroadcastUploadResultNone = 0,   // broadcast state unknown
        k_EBroadcastUploadResultOK = 1,     // broadcast was good, no problems
        k_EBroadcastUploadResultInitFailed = 2, // broadcast init failed
        k_EBroadcastUploadResultFrameFailed = 3,    // broadcast frame upload failed
        k_EBroadcastUploadResultTimeout = 4,    // broadcast upload timed out
        k_EBroadcastUploadResultBandwidthExceeded = 5,  // broadcast send too much data
        k_EBroadcastUploadResultLowFPS = 6, // broadcast FPS too low
        k_EBroadcastUploadResultMissingKeyFrames = 7,   // broadcast sending not enough key frames
        k_EBroadcastUploadResultNoConnection = 8,   // broadcast client failed to connect to relay
        k_EBroadcastUploadResultRelayFailed = 9,    // relay dropped the upload
        k_EBroadcastUploadResultSettingsChanged = 10,   // the client changed broadcast settings
        k_EBroadcastUploadResultMissingAudio = 11,  // client failed to send audio data
        k_EBroadcastUploadResultTooFarBehind = 12,  // clients was too slow uploading
    }

    // HTTP related types
    // This enum is used in client API methods, do not re-number existing values.
    public enum EHTTPMethod : int
    {
        k_EHTTPMethodInvalid = 0,
        k_EHTTPMethodGET,
        k_EHTTPMethodHEAD,
        k_EHTTPMethodPOST,
        k_EHTTPMethodPUT,
        k_EHTTPMethodDELETE,
        k_EHTTPMethodOPTIONS,

        // The remaining HTTP methods are not yet supported, per rfc2616 section 5.1.1 only GET and HEAD are required for
        // a compliant general purpose server.  We'll likely add more as we find uses for them.

        // k_EHTTPMethodTRACE,
        // k_EHTTPMethodCONNECT
    }

    // HTTP Status codes that the server can send in response to a request, see rfc2616 section 10.3 for descriptions
    // of each of these.
    public enum EHTTPStatusCode : int
    {
        // Invalid status code (this isn't defined in HTTP, used to indicate unset in our code)
        k_EHTTPStatusCodeInvalid = 0,

        // Informational codes
        k_EHTTPStatusCode100Continue = 100,
        k_EHTTPStatusCode101SwitchingProtocols = 101,

        // Success codes
        k_EHTTPStatusCode200OK = 200,
        k_EHTTPStatusCode201Created = 201,
        k_EHTTPStatusCode202Accepted = 202,
        k_EHTTPStatusCode203NonAuthoritative = 203,
        k_EHTTPStatusCode204NoContent = 204,
        k_EHTTPStatusCode205ResetContent = 205,
        k_EHTTPStatusCode206PartialContent = 206,

        // Redirection codes
        k_EHTTPStatusCode300MultipleChoices = 300,
        k_EHTTPStatusCode301MovedPermanently = 301,
        k_EHTTPStatusCode302Found = 302,
        k_EHTTPStatusCode303SeeOther = 303,
        k_EHTTPStatusCode304NotModified = 304,
        k_EHTTPStatusCode305UseProxy = 305,
        //k_EHTTPStatusCode306Unused =				306, (used in old HTTP spec, now unused in 1.1)
        k_EHTTPStatusCode307TemporaryRedirect = 307,

        // Error codes
        k_EHTTPStatusCode400BadRequest = 400,
        k_EHTTPStatusCode401Unauthorized = 401, // You probably want 403 or something else. 401 implies you're sending a WWW-Authenticate header and the client can sent an Authorization header in response.
        k_EHTTPStatusCode402PaymentRequired = 402, // This is reserved for future HTTP specs, not really supported by clients
        k_EHTTPStatusCode403Forbidden = 403,
        k_EHTTPStatusCode404NotFound = 404,
        k_EHTTPStatusCode405MethodNotAllowed = 405,
        k_EHTTPStatusCode406NotAcceptable = 406,
        k_EHTTPStatusCode407ProxyAuthRequired = 407,
        k_EHTTPStatusCode408RequestTimeout = 408,
        k_EHTTPStatusCode409Conflict = 409,
        k_EHTTPStatusCode410Gone = 410,
        k_EHTTPStatusCode411LengthRequired = 411,
        k_EHTTPStatusCode412PreconditionFailed = 412,
        k_EHTTPStatusCode413RequestEntityTooLarge = 413,
        k_EHTTPStatusCode414RequestURITooLong = 414,
        k_EHTTPStatusCode415UnsupportedMediaType = 415,
        k_EHTTPStatusCode416RequestedRangeNotSatisfiable = 416,
        k_EHTTPStatusCode417ExpectationFailed = 417,
        k_EHTTPStatusCode4xxUnknown = 418, // 418 is reserved, so we'll use it to mean unknown
        k_EHTTPStatusCode429TooManyRequests = 429,

        // Server error codes
        k_EHTTPStatusCode500InternalServerError = 500,
        k_EHTTPStatusCode501NotImplemented = 501,
        k_EHTTPStatusCode502BadGateway = 502,
        k_EHTTPStatusCode503ServiceUnavailable = 503,
        k_EHTTPStatusCode504GatewayTimeout = 504,
        k_EHTTPStatusCode505HTTPVersionNotSupported = 505,
        k_EHTTPStatusCode5xxUnknown = 599,
    }

    // Steam universes.  Each universe is a self-contained Steam instance.
    public enum EUniverse : int
    {
        k_EUniverseInvalid = 0,
        k_EUniversePublic = 1,
        k_EUniverseBeta = 2,
        k_EUniverseInternal = 3,
        k_EUniverseDev = 4,
        // k_EUniverseRC = 5,				// no such universe anymore
        k_EUniverseMax
    }
}
