using System;
using SKYNET.Helpers;

using ControllerHandle_t = System.UInt64;
using ControllerActionSetHandle_t = System.UInt64;
using ControllerDigitalActionHandle_t = System.UInt64;
using ControllerAnalogActionHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamController007")]
    public class SteamController007 : ISteamInterface
    {
        public bool Init(IntPtr _) => SteamEmulator.SteamController.Init();
        public bool Shutdown(IntPtr _) => SteamEmulator.SteamController.Shutdown();
        public void RunFrame(IntPtr _) => SteamEmulator.SteamController.RunFrame();
        public int GetConnectedControllers(IntPtr _, IntPtr handlesOut) => SteamEmulator.SteamController.GetConnectedControllers(handlesOut);
        public ControllerActionSetHandle_t GetActionSetHandle(IntPtr _, string pszActionSetName) => SteamEmulator.SteamController.GetActionSetHandle(pszActionSetName);
        public void ActivateActionSet(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle) => SteamEmulator.SteamController.ActivateActionSet(controllerHandle, actionSetHandle);
        public ControllerActionSetHandle_t GetCurrentActionSet(IntPtr _, ControllerHandle_t controllerHandle) => SteamEmulator.SteamController.GetCurrentActionSet(controllerHandle);
        public void ActivateActionSetLayer(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetLayerHandle) => SteamEmulator.SteamController.ActivateActionSetLayer(controllerHandle, actionSetLayerHandle);
        public void DeactivateActionSetLayer(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetLayerHandle) => SteamEmulator.SteamController.DeactivateActionSetLayer(controllerHandle, actionSetLayerHandle);
        public void DeactivateAllActionSetLayers(IntPtr _, ControllerHandle_t controllerHandle) => SteamEmulator.SteamController.DeactivateAllActionSetLayers(controllerHandle);
        public int GetActiveActionSetLayers(IntPtr _, ControllerHandle_t controllerHandle, IntPtr handlesOut) => SteamEmulator.SteamController.GetActiveActionSetLayers(controllerHandle, handlesOut);
        public ControllerDigitalActionHandle_t GetDigitalActionHandle(IntPtr _, string pszActionName) => SteamEmulator.SteamController.GetDigitalActionHandle(pszActionName);
        public ControllerDigitalActionData_t GetDigitalActionData(IntPtr _, ControllerHandle_t controllerHandle, ControllerDigitalActionHandle_t digitalActionHandle) => SteamEmulator.SteamController.GetDigitalActionData(controllerHandle, digitalActionHandle);
        public int GetDigitalActionOrigins(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle, ControllerDigitalActionHandle_t digitalActionHandle, IntPtr originsOut) => SteamEmulator.SteamController.GetDigitalActionOrigins(controllerHandle, actionSetHandle, digitalActionHandle, originsOut);
        public ControllerAnalogActionHandle_t GetAnalogActionHandle(IntPtr _, string pszActionName) => SteamEmulator.SteamController.GetAnalogActionHandle(pszActionName);
        public ControllerAnalogActionData_t GetAnalogActionData(IntPtr _, ControllerHandle_t controllerHandle, ControllerAnalogActionHandle_t analogActionHandle) => SteamEmulator.SteamController.GetAnalogActionData(controllerHandle, analogActionHandle);
        public int GetAnalogActionOrigins(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle, ControllerAnalogActionHandle_t analogActionHandle, IntPtr originsOut) => SteamEmulator.SteamController.GetAnalogActionOrigins(controllerHandle, actionSetHandle, analogActionHandle, originsOut);
        public IntPtr GetGlyphForActionOrigin(IntPtr _, int eOrigin) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamController.GetGlyphForActionOrigin(eOrigin));
        public IntPtr GetStringForActionOrigin(IntPtr _, int eOrigin) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamController.GetStringForActionOrigin(eOrigin));
        public void StopAnalogActionMomentum(IntPtr _, ControllerHandle_t controllerHandle, ControllerAnalogActionHandle_t eAction) => SteamEmulator.SteamController.StopAnalogActionMomentum(controllerHandle, eAction);
        public ControllerMotionData_t GetMotionData(IntPtr _, ControllerHandle_t controllerHandle) => SteamEmulator.SteamController.GetMotionData(controllerHandle);
        public void TriggerHapticPulse(IntPtr _, ControllerHandle_t controllerHandle, int eTargetPad, ushort usDurationMicroSec) => SteamEmulator.SteamController.TriggerHapticPulse(controllerHandle, eTargetPad, usDurationMicroSec);
        public void TriggerRepeatedHapticPulse(IntPtr _, ControllerHandle_t controllerHandle, int eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags) => SteamEmulator.SteamController.TriggerRepeatedHapticPulse(controllerHandle, eTargetPad, usDurationMicroSec, usOffMicroSec, unRepeat, nFlags);
        public void TriggerVibration(IntPtr _, ControllerHandle_t controllerHandle, ushort usLeftSpeed, ushort usRightSpeed) => SteamEmulator.SteamController.TriggerVibration(controllerHandle, usLeftSpeed, usRightSpeed);
        public void SetLEDColor(IntPtr _, ControllerHandle_t controllerHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags) => SteamEmulator.SteamController.SetLEDColor(controllerHandle, nColorR, nColorG, nColorB, nFlags);
        public bool ShowBindingPanel(IntPtr _, ControllerHandle_t controllerHandle) => SteamEmulator.SteamController.ShowBindingPanel(controllerHandle);
        public int GetInputTypeForHandle(IntPtr _, ControllerHandle_t controllerHandle) => SteamEmulator.SteamController.GetInputTypeForHandle(controllerHandle);
        public ControllerHandle_t GetControllerForGamepadIndex(IntPtr _, int nIndex) => SteamEmulator.SteamController.GetControllerForGamepadIndex(nIndex);
        public int GetGamepadIndexForController(IntPtr _, ControllerHandle_t ulControllerHandle) => SteamEmulator.SteamController.GetGamepadIndexForController(ulControllerHandle);
        public IntPtr GetStringForXboxOrigin(IntPtr _, int eOrigin) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamController.GetStringForXboxOrigin(eOrigin));
        public IntPtr GetGlyphForXboxOrigin(IntPtr _, int eOrigin) => NativeStringCache.ToUtf8Ptr(SteamEmulator.SteamController.GetGlyphForXboxOrigin(eOrigin));
        public int GetActionOriginFromXboxOrigin_(IntPtr _, ControllerHandle_t controllerHandle, int eOrigin) => SteamEmulator.SteamController.GetActionOriginFromXboxOrigin_(controllerHandle, eOrigin);
        public int TranslateActionOrigin(IntPtr _, int eDestinationInputType, int eSourceOrigin) => SteamEmulator.SteamController.TranslateActionOrigin(eDestinationInputType, eSourceOrigin);
        public bool GetControllerBindingRevision(IntPtr _, ControllerHandle_t controllerHandle, IntPtr pMajor, IntPtr pMinor) => SteamEmulator.SteamController.GetControllerBindingRevision(controllerHandle, pMajor, pMinor);
    }
}
