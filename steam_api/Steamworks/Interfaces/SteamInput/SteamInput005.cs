using System;
using SKYNET.Helpers;

using InputHandle_t = System.UInt64;
using InputActionSetHandle_t = System.UInt64;
using InputDigitalActionHandle_t = System.UInt64;
using InputAnalogActionHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamInput005")]
    public class SteamInput005 : ISteamInterface
    {
        public bool Init(IntPtr _, bool bExplicitlyCallRunFrame) => SteamEmulator.SteamInput.Init(bExplicitlyCallRunFrame);
        public bool Shutdown(IntPtr _) => SteamEmulator.SteamInput.Shutdown();
        public bool SetInputActionManifestFilePath(IntPtr _, string pchInputActionManifestAbsolutePath) => SteamEmulator.SteamInput.SetInputActionManifestFilePath(pchInputActionManifestAbsolutePath);
        public void RunFrame(IntPtr _, bool bReservedValue) => SteamEmulator.SteamInput.RunFrame(bReservedValue);
        public bool BWaitForData(IntPtr _, bool bWaitForever, uint unTimeout) => SteamEmulator.SteamInput.BWaitForData(bWaitForever, unTimeout);
        public bool BNewDataAvailable(IntPtr _) => SteamEmulator.SteamInput.BNewDataAvailable();
        public int GetConnectedControllers(IntPtr _, IntPtr handlesOut) => SteamEmulator.SteamInput.GetConnectedControllers(handlesOut);
        public void EnableDeviceCallbacks(IntPtr _) => SteamEmulator.SteamInput.EnableDeviceCallbacks();
        public void EnableActionEventCallbacks(IntPtr _, IntPtr pCallback) => SteamEmulator.SteamInput.EnableDeviceCallbacks();
        public InputActionSetHandle_t GetActionSetHandle(IntPtr _, string pszActionSetName) => SteamEmulator.SteamInput.GetActionSetHandle(pszActionSetName);
        public void ActivateActionSet(IntPtr _, InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle) => SteamEmulator.SteamInput.ActivateActionSet(inputHandle, actionSetHandle);
        public InputActionSetHandle_t GetCurrentActionSet(IntPtr _, InputHandle_t inputHandle) => SteamEmulator.SteamInput.GetCurrentActionSet(inputHandle);
        public void ActivateActionSetLayer(IntPtr _, InputHandle_t inputHandle, InputActionSetHandle_t actionSetLayerHandle) => SteamEmulator.SteamInput.ActivateActionSetLayer(inputHandle, actionSetLayerHandle);
        public void DeactivateActionSetLayer(IntPtr _, InputHandle_t inputHandle, InputActionSetHandle_t actionSetLayerHandle) => SteamEmulator.SteamInput.DeactivateActionSetLayer(inputHandle, actionSetLayerHandle);
        public void DeactivateAllActionSetLayers(IntPtr _, InputHandle_t inputHandle) => SteamEmulator.SteamInput.DeactivateAllActionSetLayers(inputHandle);
        public int GetActiveActionSetLayers(IntPtr _, InputHandle_t inputHandle, IntPtr handlesOut) => SteamEmulator.SteamInput.GetActiveActionSetLayers(inputHandle, handlesOut);
        public InputDigitalActionHandle_t GetDigitalActionHandle(IntPtr _, string pszActionName) => SteamEmulator.SteamInput.GetDigitalActionHandle(pszActionName);
        public InputDigitalActionData_t GetDigitalActionData(IntPtr _, InputHandle_t inputHandle, InputDigitalActionHandle_t digitalActionHandle) => SteamEmulator.SteamInput.GetDigitalActionData(inputHandle, digitalActionHandle);
        public int GetDigitalActionOrigins(IntPtr _, InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputDigitalActionHandle_t digitalActionHandle, IntPtr originsOut) => SteamEmulator.SteamInput.GetDigitalActionOrigins(inputHandle, actionSetHandle, digitalActionHandle, originsOut);
        public IntPtr GetStringForDigitalActionName(IntPtr _, InputDigitalActionHandle_t eActionHandle) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamInput.GetStringForDigitalActionName(eActionHandle));
        public InputAnalogActionHandle_t GetAnalogActionHandle(IntPtr _, string pszActionName) => SteamEmulator.SteamInput.GetAnalogActionHandle(pszActionName);
        public InputAnalogActionData_t GetAnalogActionData(IntPtr _, InputHandle_t inputHandle, InputAnalogActionHandle_t analogActionHandle) => SteamEmulator.SteamInput.GetAnalogActionData(inputHandle, analogActionHandle);
        public int GetAnalogActionOrigins(IntPtr _, InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputAnalogActionHandle_t analogActionHandle, IntPtr originsOut) => SteamEmulator.SteamInput.GetAnalogActionOrigins(inputHandle, actionSetHandle, analogActionHandle, originsOut);
        public IntPtr GetGlyphPNGForActionOrigin(IntPtr _, int eOrigin, int eSize, uint unFlags) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamInput.GetGlyphPNGForActionOrigin(eOrigin, eSize, unFlags));
        public IntPtr GetGlyphSVGForActionOrigin(IntPtr _, int eOrigin, uint unFlags) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamInput.GetGlyphSVGForActionOrigin(eOrigin, unFlags));
        public IntPtr GetGlyphForActionOrigin_Legacy(IntPtr _, int eOrigin) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamInput.GetGlyphForActionOrigin(eOrigin));
        public IntPtr GetStringForActionOrigin(IntPtr _, int eOrigin) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamInput.GetStringForActionOrigin(eOrigin));
        public IntPtr GetStringForAnalogActionName(IntPtr _, InputAnalogActionHandle_t eActionHandle) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamInput.GetStringForAnalogActionName(eActionHandle));
        public void StopAnalogActionMomentum(IntPtr _, InputHandle_t inputHandle, InputAnalogActionHandle_t eAction) => SteamEmulator.SteamInput.StopAnalogActionMomentum(inputHandle, eAction);
        public InputMotionData_t GetMotionData(IntPtr _, InputHandle_t inputHandle) => SteamEmulator.SteamInput.GetMotionData(inputHandle);
        public void TriggerVibration(IntPtr _, InputHandle_t inputHandle, ushort usLeftSpeed, ushort usRightSpeed) => SteamEmulator.SteamInput.TriggerVibration(inputHandle, usLeftSpeed, usRightSpeed);
        public void TriggerVibrationExtended(IntPtr _, InputHandle_t inputHandle, ushort usLeftSpeed, ushort usRightSpeed, ushort usLeftTriggerSpeed, ushort usRightTriggerSpeed) => SteamEmulator.SteamInput.TriggerVibrationExtended(inputHandle, usLeftSpeed, usRightSpeed, usLeftTriggerSpeed, usRightTriggerSpeed);
        public void TriggerSimpleHapticEvent(IntPtr _, InputHandle_t inputHandle, int eHapticLocation, byte nIntensity, sbyte nGainDB, byte nOtherIntensity, sbyte nOtherGainDB) => SteamEmulator.SteamInput.TriggerSimpleHapticEvent(inputHandle, eHapticLocation, nIntensity, nGainDB, nOtherIntensity, nOtherGainDB);
        public void SetLEDColor(IntPtr _, InputHandle_t inputHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags) => SteamEmulator.SteamInput.SetLEDColor(inputHandle, nColorR, nColorG, nColorB, nFlags);
        public void Legacy_TriggerHapticPulse(IntPtr _, InputHandle_t inputHandle, ESteamControllerPad eTargetPad, ushort usDurationMicroSec) => SteamEmulator.SteamInput.TriggerHapticPulse(inputHandle, eTargetPad, usDurationMicroSec);
        public void Legacy_TriggerRepeatedHapticPulse(IntPtr _, InputHandle_t inputHandle, ESteamControllerPad eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags) => SteamEmulator.SteamInput.TriggerRepeatedHapticPulse(inputHandle, eTargetPad, usDurationMicroSec, usOffMicroSec, unRepeat, nFlags);
        public bool ShowBindingPanel(IntPtr _, InputHandle_t inputHandle) => SteamEmulator.SteamInput.ShowBindingPanel(inputHandle);
        public int GetInputTypeForHandle(IntPtr _, InputHandle_t inputHandle) => SteamEmulator.SteamInput.GetInputTypeForHandle(inputHandle);
        public InputHandle_t GetControllerForGamepadIndex(IntPtr _, int nIndex) => SteamEmulator.SteamInput.GetControllerForGamepadIndex(nIndex);
        public int GetGamepadIndexForController(IntPtr _, InputHandle_t ulinputHandle) => SteamEmulator.SteamInput.GetGamepadIndexForController(ulinputHandle);
        public IntPtr GetStringForXboxOrigin(IntPtr _, int eOrigin) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamInput.GetStringForXboxOrigin(eOrigin));
        public IntPtr GetGlyphForXboxOrigin(IntPtr _, int eOrigin) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamInput.GetGlyphForXboxOrigin(eOrigin));
        public int GetActionOriginFromXboxOrigin(IntPtr _, InputHandle_t inputHandle, int eOrigin) => SteamEmulator.SteamInput.GetActionOriginFromXboxOrigin(inputHandle, eOrigin);
        public int TranslateActionOrigin(IntPtr _, int eDestinationInputType, int eSourceOrigin) => SteamEmulator.SteamInput.TranslateActionOrigin(eDestinationInputType, eSourceOrigin);
        public bool GetDeviceBindingRevision(IntPtr _, InputHandle_t inputHandle, IntPtr pMajor, IntPtr pMinor) => SteamEmulator.SteamInput.GetDeviceBindingRevision(inputHandle, pMajor, pMinor);
        public uint GetRemotePlaySessionID(IntPtr _, InputHandle_t inputHandle) => SteamEmulator.SteamInput.GetRemotePlaySessionID(inputHandle);
        public ushort GetSessionInputConfigurationSettings(IntPtr _) => SteamEmulator.SteamInput.GetSessionInputConfigurationSettings();
    }
}
