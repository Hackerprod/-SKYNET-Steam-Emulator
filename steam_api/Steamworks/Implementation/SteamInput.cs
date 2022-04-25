using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SKYNET.Managers;

using InputHandle_t = System.UInt64;
using InputActionSetHandle_t = System.UInt64;
using InputDigitalActionHandle_t = System.UInt64;
using InputAnalogActionHandle_t = System.UInt64;


namespace SKYNET.Steamworks.Implementation
{
    public class SteamInput : ISteamInterface
    {
        public SteamInput()
        {
            InterfaceVersion = "SteamInput";
        }

        public void ActivateActionSet(InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle)
        {
            Write("ActivateActionSet");
        }

        public void ActivateActionSetLayer(InputHandle_t inputHandle, InputActionSetHandle_t actionSetLayerHandle)
        {
            Write("ActivateActionSetLayer");
        }

        public bool BNewDataAvailable()
        {
            Write("BNewDataAvailable");
            return false;
        }

        public bool BWaitForData(bool bWaitForever, uint unTimeout)
        {
            Write("BWaitForData");
            return false;
        }

        public void DeactivateActionSetLayer(InputHandle_t inputHandle, InputActionSetHandle_t actionSetLayerHandle)
        {
            Write("DeactivateActionSetLayer");
        }

        public void DeactivateAllActionSetLayers(InputHandle_t inputHandle)
        {
            Write("DeactivateAllActionSetLayers");
        }

        public void EnableDeviceCallbacks()
        {
            Write("EnableDeviceCallbacks");
        }

        public int GetActionOriginFromint(InputHandle_t inputHandle, int eOrigin)
        {
            Write("GetActionOriginFromint");
            return 0;
        }

        public IntPtr GetActionSetHandle(string pszActionSetName)
        {
            Write("GetActionSetHandle");
            return IntPtr.Zero;
        }

        public int GetActiveActionSetLayers(InputHandle_t inputHandle, [In, Out] IntPtr[] handlesOut)
        {
            Write("GetActiveActionSetLayers");
            return 0;
        }

        public IntPtr GetAnalogActionData(InputHandle_t inputHandle, InputAnalogActionHandle_t analogActionHandle)
        {
            Write("GetAnalogActionData");
            return IntPtr.Zero;
        }

        public IntPtr GetAnalogActionHandle(string pszActionName)
        {
            Write("GetAnalogActionHandle");
            return IntPtr.Zero;
        }

        public int GetAnalogActionOrigins(InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputAnalogActionHandle_t analogActionHandle, ref int originsOut)
        {
            Write("GetAnalogActionOrigins");
            return 0;
        }

        public int GetConnectedControllers([In, Out] InputHandle_t[] handlesOut)
        {
            Write("GetConnectedControllers");
            return 0;
        }

        public IntPtr GetControllerForGamepadIndex(int nIndex)
        {
            Write("GetControllerForGamepadIndex");
            return IntPtr.Zero;
        }

        public IntPtr GetCurrentActionSet(InputHandle_t inputHandle)
        {
            Write("GetCurrentActionSet");
            return IntPtr.Zero;
        }

        public bool GetDeviceBindingRevision(InputHandle_t inputHandle, ref int pMajor, ref int pMinor)
        {
            Write("GetDeviceBindingRevision");
            return false;
        }

        public IntPtr GetDigitalActionData(InputHandle_t inputHandle, IntPtr digitalActionHandle)
        {
            Write("GetDigitalActionData");
            return IntPtr.Zero;
        }

        public IntPtr GetDigitalActionHandle(string pszActionName)
        {
            Write("GetDigitalActionHandle");
            return IntPtr.Zero;
        }

        public int GetDigitalActionOrigins(InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, IntPtr digitalActionHandle, ref int originsOut)
        {
            Write("GetDigitalActionOrigins");
            return 0;
        }

        public int GetGamepadIndexForController(InputHandle_t ulinputHandle)
        {
            Write("GetGamepadIndexForController");
            return 0;
        }

        public IntPtr GetGlyphForActionOrigin_Legacy(int eOrigin)
        {
            Write("GetGlyphForActionOrigin_Legacy");
            return IntPtr.Zero;
        }

        public IntPtr GetGlyphForint(int eOrigin)
        {
            Write("GetGlyphForint");
            return IntPtr.Zero;
        }

        public IntPtr GetGlyphPNGForActionOrigin(int eOrigin, int eSize, uint unFlags)
        {
            Write("GetGlyphPNGForActionOrigin");
            return IntPtr.Zero;
        }

        public IntPtr GetGlyphSVGForActionOrigin(int eOrigin, uint unFlags)
        {
            Write("GetGlyphSVGForActionOrigin");
            return IntPtr.Zero;
        }

        public int GetInputTypeForHandle(InputHandle_t inputHandle)
        {
            Write("GetInputTypeForHandle");
            return 0;
        }

        public InputMotionData_t GetMotionData(InputHandle_t inputHandle)
        {
            Write("xxx");
            return new InputMotionData_t();
        }

        public uint GetRemotePlaySessionID(InputHandle_t inputHandle)
        {
            Write("GetRemotePlaySessionID");
            return 0;
        }

        public ushort GetSessionInputConfigurationSettings()
        {
            Write("GetSessionInputConfigurationSettings");
            return 0;
        }

        public IntPtr GetStringForActionOrigin(int eOrigin)
        {
            Write("GetStringForActionOrigin");
            return IntPtr.Zero;
        }

        public IntPtr GetStringForAnalogActionName(InputAnalogActionHandle_t eActionHandle)
        {
            Write("GetStringForAnalogActionName");
            return IntPtr.Zero;
        }

        public IntPtr GetStringForDigitalActionName(InputAnalogActionHandle_t eActionHandle)
        {
            Write("GetStringForDigitalActionName");
            return IntPtr.Zero;
        }

        public IntPtr GetStringForint(int eOrigin)
        {
            Write("GetStringForint");
            return IntPtr.Zero;
        }

        public bool Init(bool bExplicitlyCallRunFrame)
        {
            Write("Init");
            return true;
        }

        public void Legacy_TriggerHapticPulse(InputHandle_t inputHandle, int eTargetPad, ushort usDurationMicroSec)
        {
            Write("Legacy_TriggerHapticPulse");
        }

        public void Legacy_TriggerRepeatedHapticPulse(InputHandle_t inputHandle, int eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags)
        {
            Write("Legacy_TriggerRepeatedHapticPulse");
        }

        public void RunFrame(bool bReservedValue)
        {
            Write("RunFrame");
        }

        public bool SetInputActionManifestFilePath(string pchInputActionManifestAbsolutePath)
        {
            Write("SetInputActionManifestFilePath");
            return true;
        }

        public void SetLEDColor(InputHandle_t inputHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags)
        {
            Write("SetLEDColor");
        }

        public bool ShowBindingPanel(InputHandle_t inputHandle)
        {
            Write("ShowBindingPanel");
            return false;
        }

        public bool Shutdown()
        {
            Write("Shutdown");
            return true;
        }

        public void StopAnalogActionMomentum(InputHandle_t inputHandle, InputAnalogActionHandle_t eAction)
        {
            Write("StopAnalogActionMomentum");
        }

        public int TranslateActionOrigin(int eDestinationInputType, int eSourceOrigin)
        {
            Write("TranslateActionOrigin");
            return 0;
        }

        public void TriggerSimpleHapticEvent(InputHandle_t inputHandle, IntPtr eHapticLocation, byte nIntensity, char nGainDB, byte nOtherIntensity, char nOtherGainDB)
        {
            Write("TriggerSimpleHapticEvent");
        }

        public void TriggerVibration(InputHandle_t inputHandle, ushort usLeftSpeed, ushort usRightSpeed)
        {
            Write("TriggerVibration");
        }

        public void TriggerVibrationExtended(InputHandle_t inputHandle, ushort usLeftSpeed, ushort usRightSpeed, ushort usLeftTriggerSpeed, ushort usRightTriggerSpeed)
        {
            Write("TriggerVibrationExtended");
        }

        public IntPtr SteamAPI_SteamInput_v005()
        {
            Write("SteamAPI_SteamInput_v005");
            return InterfaceManager.FindOrCreateInterface("SteamInput005");
        }
    }
}
