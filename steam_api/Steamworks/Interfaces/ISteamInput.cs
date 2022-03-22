using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamInput
    {
        bool Init([MarshalAs(UnmanagedType.U1)] bool bExplicitlyCallRunFrame);

        bool Shutdown(IntPtr _);

        bool SetInputActionManifestFilePath( string pchInputActionManifestAbsolutePath);
        
        void RunFrame([MarshalAs(UnmanagedType.U1)] bool bReservedValue);
        
        bool BWaitForData([MarshalAs(UnmanagedType.U1)] bool bWaitForever, uint unTimeout);
        
        bool BNewDataAvailable(IntPtr _);

        int GetConnectedControllers([In, Out] IntPtr[] handlesOut);

        void EnableDeviceCallbacks(IntPtr _);
       
        IntPtr GetActionSetHandle( string pszActionSetName);

        void ActivateActionSet(IntPtr inputHandle, IntPtr actionSetHandle);
        
        IntPtr GetCurrentActionSet(IntPtr inputHandle);

        void ActivateActionSetLayer(IntPtr inputHandle, IntPtr actionSetLayerHandle);
        
        void DeactivateActionSetLayer(IntPtr inputHandle, IntPtr actionSetLayerHandle);

        void DeactivateAllActionSetLayers(IntPtr inputHandle);

        int GetActiveActionSetLayers(IntPtr inputHandle, [In, Out] IntPtr[] handlesOut);

        IntPtr GetDigitalActionHandle( string pszActionName);

        IntPtr GetDigitalActionData(IntPtr inputHandle, IntPtr digitalActionHandle);

        int GetDigitalActionOrigins(IntPtr inputHandle, IntPtr actionSetHandle, IntPtr digitalActionHandle, ref int originsOut);

        IntPtr GetStringForDigitalActionName(IntPtr eActionHandle);

        IntPtr GetAnalogActionHandle( string pszActionName);

        IntPtr GetAnalogActionData(IntPtr inputHandle, IntPtr analogActionHandle);

        int GetAnalogActionOrigins(IntPtr inputHandle, IntPtr actionSetHandle, IntPtr analogActionHandle, ref int originsOut);
        
        IntPtr GetGlyphPNGForActionOrigin(int eOrigin, IntPtr eSize, uint unFlags);

        IntPtr GetGlyphSVGForActionOrigin(int eOrigin, uint unFlags);

        IntPtr GetGlyphForActionOrigin_Legacy(int eOrigin);

        IntPtr GetStringForActionOrigin(int eOrigin);

        IntPtr GetStringForAnalogActionName(IntPtr eActionHandle);

        void StopAnalogActionMomentum(IntPtr inputHandle, IntPtr eAction);

        InputMotionData_t GetMotionData(IntPtr inputHandle);

        void TriggerVibration(IntPtr inputHandle, ushort usLeftSpeed, ushort usRightSpeed);

        void TriggerVibrationExtended(IntPtr inputHandle, ushort usLeftSpeed, ushort usRightSpeed, ushort usLeftTriggerSpeed, ushort usRightTriggerSpeed);

        void TriggerSimpleHapticEvent(IntPtr inputHandle, IntPtr eHapticLocation, byte nIntensity, char nGainDB, byte nOtherIntensity, char nOtherGainDB);

        void SetLEDColor(IntPtr inputHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags);

        void Legacy_TriggerHapticPulse(IntPtr inputHandle, IntPtr eTargetPad, ushort usDurationMicroSec);

        void Legacy_TriggerRepeatedHapticPulse(IntPtr inputHandle, IntPtr eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags);
        
        bool ShowBindingPanel(IntPtr inputHandle);

        ESteamInputType GetInputTypeForHandle(IntPtr inputHandle);

        IntPtr GetControllerForGamepadIndex(int nIndex);

        int GetGamepadIndexForController(IntPtr ulinputHandle);
        
        IntPtr GetStringForint(int eOrigin);

        IntPtr GetGlyphForint(int eOrigin);

        int GetActionOriginFromint(IntPtr inputHandle, int eOrigin);

        int TranslateActionOrigin(ESteamInputType eDestinationInputType, int eSourceOrigin);

        bool GetDeviceBindingRevision(IntPtr inputHandle, ref int pMajor, ref int pMinor);

        uint GetRemotePlaySessionID(IntPtr inputHandle);

        ushort GetSessionInputConfigurationSettings(IntPtr _);

    }
    public struct InputDigitalActionData_t
    {
        // The current state of this action; will be true if currently pressed
        bool bState;

        // Whether or not this action is currently available to be bound in the active action set
        bool bActive;
    };
    public struct InputAnalogActionData_t
    {
        // Type of data coming from this action, this will match what got specified in the action set
        int eMode;

        // The current state of this action; will be delta updates for mouse actions
        float x, y;

        // Whether or not this action is currently available to be bound in the active action set
        bool bActive;
    };
    public struct InputMotionData_t
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
