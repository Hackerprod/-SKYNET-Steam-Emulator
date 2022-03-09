using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamController
    {
        bool Init();
        bool Shutdown();
        void RunFrame();
        int GetConnectedControllers(IntPtr handles, IntPtr handlesOut);
        int GetActionSetHandle(string pszActionSetName);
        void ActivateActionSet(IntPtr controllerHandle, int actionSetHandle);
        int GetCurrentActionSet(IntPtr controllerHandle);
        void ActivateActionSetLayer(IntPtr controllerHandle, IntPtr actionSetLayerHandle);
        void DeactivateActionSetLayer(IntPtr controllerHandle, IntPtr actionSetLayerHandle);
        void DeactivateAllActionSetLayers(IntPtr controllerHandle);
        int GetActiveActionSetLayers(IntPtr controllerHandle, int handlesOut );
        int GetDigitalActionHandle(string pszActionName);

        ControllerDigitalActionData_t GetDigitalActionData(IntPtr controllerHandle, int digitalActionHandle);

        int GetDigitalActionOrigins(IntPtr controllerHandle, int actionSetHandle, int digitalActionHandle, EControllerActionOrigin originsOut );

        int GetAnalogActionHandle(string pszActionName);

        ControllerAnalogActionData_t GetAnalogActionData(uint controllerHandle, uint analogActionHandle);

        int GetAnalogActionOrigins(IntPtr controllerHandle, int actionSetHandle, uint analogActionHandle, EControllerActionOrigin originsOut );

        string GetGlyphForActionOrigin(EControllerActionOrigin eOrigin);

        string GetStringForActionOrigin(EControllerActionOrigin eOrigin);

        void StopAnalogActionMomentum(IntPtr controllerHandle, uint eAction);

        ControllerMotionData_t GetMotionData(IntPtr controllerHandle);

        //-----------------------------------------------------------------------------
        // OUTPUTS
        //-----------------------------------------------------------------------------

        void TriggerHapticPulse(IntPtr controllerHandle, ESteamControllerPad eTargetPad, uint usDurationMicroSec);

        void TriggerRepeatedHapticPulse(IntPtr controllerHandle, ESteamControllerPad eTargetPad, uint usDurationMicroSec, uint usOffMicroSec, uint unRepeat, int nFlags);

        void TriggerVibration(IntPtr controllerHandle, uint usLeftSpeed, uint usRightSpeed);

        void SetLEDColor(IntPtr controllerHandle, uint nColorR, uint nColorG, uint nColorB, int nFlags);

        bool ShowBindingPanel(IntPtr controllerHandle);

        ESteamInputType GetInputTypeForHandle(IntPtr controllerHandle);

        int GetControllerForGamepadIndex(int nIndex);

        int GetGamepadIndexForController(IntPtr ulControllerHandle);

        string GetStringForXboxOrigin(EXboxOrigin eOrigin);

        string GetGlyphForXboxOrigin(EXboxOrigin eOrigin);

        EControllerActionOrigin GetActionOriginFromXboxOrigin_(IntPtr controllerHandle, EXboxOrigin eOrigin);

        EControllerActionOrigin TranslateActionOrigin(ESteamInputType eDestinationInputType, EControllerActionOrigin eSourceOrigin);

        bool GetControllerBindingRevision(IntPtr controllerHandle, int pMajor, int pMinor);


    }
    public struct ControllerDigitalActionData_t
    {
        // The current state of this action; will be true if currently pressed
        public bool bState;

        // Whether or not this action is currently available to be bound in the active action set
        public bool bActive;
    };
    public enum ESteamControllerPad
    {
        k_ESteamControllerPad_Left,
        k_ESteamControllerPad_Right
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
    public struct ControllerAnalogActionData_t
    {
        // Type of data coming from this action, this will match what got specified in the action set
        public int eMode;

        // The current state of this action; will be delta updates for mouse actions
        public float x, y;

        // Whether or not this action is currently available to be bound in the active action set
        public bool bActive;
    };

    public struct ControllerMotionData_t
    {
        // Sensor-fused absolute rotation; will drift in heading
        float rotQuatX;
        float rotQuatY;
        float rotQuatZ;
        float rotQuatW;

        // Positional acceleration
        float posAccelX;
        float posAccelY;
        float posAccelZ;

        // Angular velocity
        float rotVelX;
        float rotVelY;
        float rotVelZ;
    };
}
