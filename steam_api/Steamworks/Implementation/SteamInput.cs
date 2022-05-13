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
            InterfaceName = "SteamInput";
            InterfaceVersion = "SteamInput001";
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

        public InputActionSetHandle_t GetActionSetHandle(string pszActionSetName)
        {
            Write("GetActionSetHandle");
            return 0;
        }

        public int GetActiveActionSetLayers(InputHandle_t inputHandle, ref InputActionSetHandle_t[] handlesOut)
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

        public int GetAnalogActionOrigins(InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputAnalogActionHandle_t analogActionHandle, ref int[] originsOut)
        {
            Write("GetAnalogActionOrigins");
            return 0;
        }

        public int GetConnectedControllers(ref InputHandle_t[] handlesOut)
        {
            Write("GetConnectedControllers");
            return 0;
        }

        public InputHandle_t GetControllerForGamepadIndex(int nIndex)
        {
            Write("GetControllerForGamepadIndex");
            return 0;
        }

        public InputActionSetHandle_t GetCurrentActionSet(InputHandle_t inputHandle)
        {
            Write("GetCurrentActionSet");
            return 0;
        }

        public bool GetDeviceBindingRevision(InputHandle_t inputHandle, int pMajor, int pMinor)
        {
            Write("GetDeviceBindingRevision");
            return false;
        }

        public IntPtr GetDigitalActionData(InputHandle_t inputHandle, InputDigitalActionHandle_t digitalActionHandle)
        {
            Write("GetDigitalActionData");
            // InputDigitalActionData_t type
            return IntPtr.Zero;
        }

        public InputDigitalActionHandle_t GetDigitalActionHandle(string pszActionName)
        {
            Write("GetDigitalActionHandle");
            return 0;
        }

        public int GetDigitalActionOrigins(InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputDigitalActionHandle_t digitalActionHandle, ref int[] originsOut)
        {
            Write("GetDigitalActionOrigins");
            return 0;
        }

        public string GetGlyphForActionOrigin(int eOrigin)
        {
            Write("GetGlyphForActionOrigin");
            return "";
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

        public string GetGlyphPNGForActionOrigin(int eOrigin, int eSize, uint unFlags)
        {
            Write("GetGlyphPNGForActionOrigin");
            return "";
        }

        public string GetGlyphSVGForActionOrigin(int eOrigin, uint unFlags)
        {
            Write("GetGlyphSVGForActionOrigin");
            return "";
        }

        public int GetInputTypeForHandle(InputHandle_t inputHandle)
        {
            Write("GetInputTypeForHandle");
            return 0;
        }

        public void TriggerHapticPulse(ulong inputHandle, ESteamControllerPad eTargetPad, short usDurationMicroSec)
        {
            Write("TriggerHapticPulse");
        }

        public void TriggerRepeatedHapticPulse(ulong inputHandle, ESteamControllerPad eTargetPad, short usDurationMicroSec, short usOffMicroSec, short unRepeat, int nFlags)
        {
            Write("TriggerRepeatedHapticPulse");
        }

        public IntPtr GetMotionData(InputHandle_t inputHandle)
        {
            Write("GetMotionData");
            return IntPtr.Zero;
        }

        public ushort GetSessionInputConfigurationSettings()
        {
            Write("GetSessionInputConfigurationSettings");
            return 0;
        }

        public string GetStringForActionOrigin(int eOrigin)
        {
            Write("GetStringForActionOrigin");
            return "";
        }

        public string GetStringForAnalogActionName(InputAnalogActionHandle_t eActionHandle)
        {
            Write("GetStringForAnalogActionName");
            return "";
        }

        public string GetStringForXboxOrigin(int eOrigin)
        {
            Write("GetStringForXboxOrigin");
            return "";
        }

        public string GetGlyphForXboxOrigin(int eOrigin)
        {
            Write("GetGlyphForXboxOrigin");
            return "";
        }

        public int GetActionOriginFromXboxOrigin(InputHandle_t inputHandle, int eOrigin)
        {
            Write("GetActionOriginFromXboxOrigin");
            return 0;
        }

        public string GetStringForDigitalActionName(InputAnalogActionHandle_t eActionHandle)
        {
            Write("GetStringForDigitalActionName");
            return "";
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

        public bool Init()
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

        public void RunFrame()
        {
            Write("RunFrame");
        }

        public bool SetInputActionManifestFilePath(string pchInputActionManifestAbsolutePath)
        {
            Write("SetInputActionManifestFilePath");
            return true;
        }

        public void SetLEDColor(InputHandle_t inputHandle, int nColorR, int nColorG, int nColorB, int nFlags)
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

        public void TriggerSimpleHapticEvent(InputHandle_t inputHandle, int eHapticLocation, int nIntensity, string nGainDB, int nOtherIntensity, string nOtherGainDB)
        {
            Write("TriggerSimpleHapticEvent");
        }

        public void TriggerVibration(InputHandle_t inputHandle, short usLeftSpeed, short usRightSpeed)
        {
            Write("TriggerVibration");
        }

        public void TriggerVibrationExtended(InputHandle_t inputHandle, short usLeftSpeed, short usRightSpeed, short usLeftTriggerSpeed, short usRightTriggerSpeed)
        {
            Write("TriggerVibrationExtended");
        }

        public IntPtr SteamAPI_SteamInput_v005()
        {
            Write("SteamAPI_SteamInput_v005");
            return InterfaceManager.FindOrCreateInterface("SteamInput005");
        }

        public uint GetRemotePlaySessionID(InputHandle_t inputHandle)
        {
            Write("GetRemotePlaySessionID");
            return 0;
        }
    }
}
