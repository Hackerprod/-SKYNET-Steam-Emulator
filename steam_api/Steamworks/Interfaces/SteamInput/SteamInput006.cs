using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SKYNET.Interface;

using InputHandle_t = System.UInt64;
using InputActionSetHandle_t = System.UInt64;
using InputDigitalActionHandle_t = System.UInt64;
using InputAnalogActionHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamInput006")]
    public class SteamInput006 : ISteamInterface
    {
        public bool Init(IntPtr _, bool bExplicitlyCallRunFrame)
        {
            return SteamEmulator.SteamInput.Init(bExplicitlyCallRunFrame);
        }

        public bool Shutdown(IntPtr _)
        {
            return SteamEmulator.SteamInput.Shutdown();
        }

        public bool SetInputActionManifestFilePath(IntPtr _, string pchInputActionManifestAbsolutePath)
        {
            return SteamEmulator.SteamInput.SetInputActionManifestFilePath(pchInputActionManifestAbsolutePath);
        }

        public void RunFrame(IntPtr _, bool bReservedValue)
        {
            SteamEmulator.SteamInput.RunFrame(bReservedValue);
        }

        public bool BWaitForData(IntPtr _, bool bWaitForever, UInt32 unTimeout)
        {
            return SteamEmulator.SteamInput.BWaitForData(bWaitForever, unTimeout);
        }

        public bool BNewDataAvailable(IntPtr _)
        {
            return SteamEmulator.SteamInput.BNewDataAvailable();
        }

        public int GetConnectedControllers(IntPtr _, ref InputHandle_t[] handlesOut)
        {
            return SteamEmulator.SteamInput.GetConnectedControllers(ref handlesOut);
        }

        public void EnableDeviceCallbacks(IntPtr _)
        {
            SteamEmulator.SteamInput.EnableDeviceCallbacks();
        }

        public void EnableActionEventCallbacks(IntPtr _, IntPtr pCallback)
        {
            SteamEmulator.SteamInput.EnableDeviceCallbacks();
        }

        public InputActionSetHandle_t GetActionSetHandle(IntPtr _, string pszActionSetName)
        {
            return SteamEmulator.SteamInput.GetActionSetHandle(pszActionSetName);
        }

        public void ActivateActionSet(IntPtr _, InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle)
        {
            SteamEmulator.SteamInput.ActivateActionSet(inputHandle, actionSetHandle);
        }
        public InputActionSetHandle_t GetCurrentActionSet(IntPtr _, InputHandle_t inputHandle)
        {
            return SteamEmulator.SteamInput.GetCurrentActionSet(inputHandle);
        }

        public void ActivateActionSetLayer(IntPtr _, InputHandle_t inputHandle, InputActionSetHandle_t actionSetLayerHandle)
        {
            SteamEmulator.SteamInput.ActivateActionSetLayer(inputHandle, actionSetLayerHandle);
        }

        public void DeactivateActionSetLayer(IntPtr _, InputHandle_t inputHandle, InputActionSetHandle_t actionSetLayerHandle)
        {
            SteamEmulator.SteamInput.DeactivateActionSetLayer(inputHandle, actionSetLayerHandle);
        }

        public void DeactivateAllActionSetLayers(IntPtr _, InputHandle_t inputHandle)
        {
            SteamEmulator.SteamInput.DeactivateAllActionSetLayers(inputHandle);
        }

        public int GetActiveActionSetLayers(IntPtr _, InputHandle_t inputHandle, ref InputActionSetHandle_t[] handlesOut)
        {
            return SteamEmulator.SteamInput.GetActiveActionSetLayers(inputHandle, ref handlesOut);
        }

        public InputDigitalActionHandle_t GetDigitalActionHandle(IntPtr _, string pszActionName)
        {
            return SteamEmulator.SteamInput.GetDigitalActionHandle(pszActionName);
        }

        public IntPtr GetDigitalActionData(IntPtr _, InputHandle_t inputHandle, InputDigitalActionHandle_t digitalActionHandle)
        {
            return SteamEmulator.SteamInput.GetDigitalActionData(inputHandle, digitalActionHandle);
        }

        public int GetDigitalActionOrigins(IntPtr _, InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputDigitalActionHandle_t digitalActionHandle, ref int[] originsOut)
        {
            return SteamEmulator.SteamInput.GetDigitalActionOrigins(inputHandle, actionSetHandle, digitalActionHandle, ref originsOut);
        }

        public string GetStringForDigitalActionName(IntPtr _, InputDigitalActionHandle_t eActionHandle)
        {
            return SteamEmulator.SteamInput.GetStringForDigitalActionName(eActionHandle);
        }

        public IntPtr GetAnalogActionHandle(IntPtr _, string pszActionName)
        {
            return SteamEmulator.SteamInput.GetAnalogActionHandle(pszActionName);
        }

        public IntPtr GetAnalogActionData(IntPtr _, InputHandle_t inputHandle, InputAnalogActionHandle_t analogActionHandle)
        {
            return SteamEmulator.SteamInput.GetAnalogActionData(inputHandle, analogActionHandle);
        }

        public int GetAnalogActionOrigins(IntPtr _, InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputAnalogActionHandle_t analogActionHandle, ref int[] originsOut)
        {
            return SteamEmulator.SteamInput.GetAnalogActionOrigins(inputHandle, actionSetHandle, analogActionHandle, ref originsOut);
        }

        public string GetGlyphPNGForActionOrigin(IntPtr _, int eOrigin, int eSize, UInt32 unFlags)
        {
            return SteamEmulator.SteamInput.GetGlyphPNGForActionOrigin(eOrigin, eSize, unFlags);
        }

        public string GetGlyphSVGForActionOrigin(IntPtr _, int eOrigin, UInt32 unFlags)
        {
            return SteamEmulator.SteamInput.GetGlyphSVGForActionOrigin(eOrigin, unFlags);
        }

        public string GetGlyphForActionOrigin(IntPtr _, int eOrigin)
        {
            return SteamEmulator.SteamInput.GetGlyphForActionOrigin(eOrigin);
        }

        public string GetStringForActionOrigin(IntPtr _, int eOrigin)
        {
            return SteamEmulator.SteamInput.GetStringForActionOrigin(eOrigin);
        }

        public string GetStringForAnalogActionName(IntPtr _, InputAnalogActionHandle_t eActionHandle )
        {
            return SteamEmulator.SteamInput.GetStringForAnalogActionName(eActionHandle);
        }

        public void StopAnalogActionMomentum(IntPtr _, InputHandle_t inputHandle, InputAnalogActionHandle_t eAction)
        {
            SteamEmulator.SteamInput.StopAnalogActionMomentum(inputHandle, eAction);
        }

        public InputMotionData_t GetMotionData(IntPtr _, InputHandle_t inputHandle)
        {
            return SteamEmulator.SteamInput.GetMotionData(inputHandle);
        }

        public void TriggerVibration(IntPtr _, InputHandle_t inputHandle, short usLeftSpeed, short usRightSpeed)
        {
            SteamEmulator.SteamInput.TriggerVibration(inputHandle, usLeftSpeed, usRightSpeed);
        }

        public void TriggerVibrationExtended(IntPtr _, InputHandle_t inputHandle, short usLeftSpeed, short usRightSpeed, short usLeftTriggerSpeed, short usRightTriggerSpeed)
        {
            SteamEmulator.SteamInput.TriggerVibrationExtended(inputHandle, usLeftSpeed, usRightSpeed, usLeftTriggerSpeed, usRightTriggerSpeed);
        }

        public void TriggerSimpleHapticEvent(IntPtr _, InputHandle_t inputHandle, int eHapticLocation, int nIntensity, string nGainDB, int nOtherIntensity, string nOtherGainDB)
        {
            SteamEmulator.SteamInput.TriggerSimpleHapticEvent(inputHandle, eHapticLocation, nIntensity, nGainDB, nOtherIntensity, nOtherGainDB);
        }

        public void SetLEDColor(IntPtr _, InputHandle_t inputHandle, int nColorR, int nColorG, int nColorB, int nFlags)
        {
            SteamEmulator.SteamInput.SetLEDColor(inputHandle, nColorR, nColorG, nColorB, nFlags);
        }

        public void Legacy_TriggerHapticPulse(IntPtr _, InputHandle_t inputHandle, ESteamControllerPad eTargetPad, short usDurationMicroSec)
        {
            SteamEmulator.SteamInput.TriggerHapticPulse(inputHandle, eTargetPad, usDurationMicroSec);
        }

        public void Legacy_TriggerRepeatedHapticPulse(IntPtr _, InputHandle_t inputHandle, ESteamControllerPad eTargetPad, short usDurationMicroSec, short usOffMicroSec, short unRepeat, int nFlags)
        {
            SteamEmulator.SteamInput.TriggerRepeatedHapticPulse(inputHandle, eTargetPad, usDurationMicroSec, usOffMicroSec, unRepeat, nFlags);
        }

        public bool ShowBindingPanel(IntPtr _, InputHandle_t inputHandle)
        {
            return SteamEmulator.SteamInput.ShowBindingPanel(inputHandle);
        }

        public int GetInputTypeForHandle(IntPtr _, InputHandle_t inputHandle)
        {
            return SteamEmulator.SteamInput.GetInputTypeForHandle(inputHandle);
        }

        public InputHandle_t GetControllerForGamepadIndex(IntPtr _, int nIndex)
        {
            return SteamEmulator.SteamInput.GetControllerForGamepadIndex(nIndex);
        }

        public int GetGamepadIndexForController(IntPtr _, InputHandle_t ulinputHandle)
        {
            return SteamEmulator.SteamInput.GetGamepadIndexForController(ulinputHandle);
        }

        public string GetStringForXboxOrigin(IntPtr _, int eOrigin)
        {
            return SteamEmulator.SteamInput.GetStringForXboxOrigin(eOrigin);
        }

        public string GetGlyphForXboxOrigin(IntPtr _, int eOrigin)
        {
            return SteamEmulator.SteamInput.GetGlyphForXboxOrigin(eOrigin);
        }

        public int GetActionOriginFromXboxOrigin(IntPtr _, InputHandle_t inputHandle, int eOrigin)
        {
            return SteamEmulator.SteamInput.GetActionOriginFromXboxOrigin(inputHandle, eOrigin);
        }

        public int TranslateActionOrigin(IntPtr _, int eDestinationInputType, int eSourceOrigin)
        {
            return SteamEmulator.SteamInput.TranslateActionOrigin(eDestinationInputType, eSourceOrigin);
        }

        public bool GetDeviceBindingRevision(IntPtr _, InputHandle_t inputHandle, int pMajor, int pMinor)
        {
            return SteamEmulator.SteamInput.GetDeviceBindingRevision(inputHandle, pMajor, pMinor);
        }

        public UInt32 GetRemotePlaySessionID(IntPtr _, InputHandle_t inputHandle)
        {
            return SteamEmulator.SteamInput.GetRemotePlaySessionID(inputHandle);
        }

        public UInt16 GetSessionInputConfigurationSettings(IntPtr _)
        {
            return SteamEmulator.SteamInput.GetSessionInputConfigurationSettings();
        }

    }
}
