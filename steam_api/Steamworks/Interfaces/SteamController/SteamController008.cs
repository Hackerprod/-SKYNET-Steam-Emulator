using System;

using ControllerHandle_t = System.UInt64;
using ControllerActionSetHandle_t = System.UInt64;
using ControllerDigitalActionHandle_t = System.UInt64;
using ControllerAnalogActionHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamController006")]
    [Interface("SteamController007")] // Checked (some SteamController008)
    [Interface("SteamController008")]
    public class SteamController008 : ISteamInterface
    {
        public bool Init(IntPtr _)
        {
            return SteamEmulator.SteamController.Init();
        }

        public bool Shutdown(IntPtr _)
        {
            return SteamEmulator.SteamController.Shutdown();
        }

        public void RunFrame(IntPtr _)
        {
            SteamEmulator.SteamController.RunFrame();
        }

        public int GetConnectedControllers(IntPtr _, ControllerHandle_t handlesOut)
        {
            return SteamEmulator.SteamController.GetConnectedControllers(handlesOut);
        }

        public ControllerActionSetHandle_t GetActionSetHandle(IntPtr _, string pszActionSetName)
        {
            return SteamEmulator.SteamController.GetActionSetHandle(pszActionSetName);
        }

        public void ActivateActionSet(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle)
        {
            SteamEmulator.SteamController.ActivateActionSet(controllerHandle, actionSetHandle);
        }

        public ControllerActionSetHandle_t GetCurrentActionSet(IntPtr _, ControllerHandle_t controllerHandle)
        {
            return SteamEmulator.SteamController.GetCurrentActionSet(controllerHandle);
        }

        public void ActivateActionSetLayer(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetLayerHandle)
        {
            SteamEmulator.SteamController.ActivateActionSetLayer(controllerHandle, actionSetLayerHandle);
        }

        public void DeactivateActionSetLayer(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetLayerHandle)
        {
            SteamEmulator.SteamController.DeactivateActionSetLayer(controllerHandle, actionSetLayerHandle);
        }

        public void DeactivateAllActionSetLayers(IntPtr _, ControllerHandle_t controllerHandle)
        {
            SteamEmulator.SteamController.DeactivateAllActionSetLayers(controllerHandle);
        }

        public int GetActiveActionSetLayers(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t handlesOut)
        {
            return SteamEmulator.SteamController.GetActiveActionSetLayers(controllerHandle, handlesOut);
        }

        public ControllerDigitalActionHandle_t GetDigitalActionHandle(IntPtr _, string pszActionName)
        {
            return SteamEmulator.SteamController.GetDigitalActionHandle(pszActionName);
        }

        public IntPtr GetDigitalActionData(IntPtr _, ControllerHandle_t controllerHandle, ControllerDigitalActionHandle_t digitalActionHandle)
        {
            return SteamEmulator.SteamController.GetDigitalActionData(controllerHandle, digitalActionHandle);
        }

        public int GetDigitalActionOrigins(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle, ControllerDigitalActionHandle_t digitalActionHandle, int originsOut)
        {
            return SteamEmulator.SteamController.GetDigitalActionOrigins(controllerHandle, actionSetHandle, digitalActionHandle, originsOut);
        }

        public ControllerAnalogActionHandle_t GetAnalogActionHandle(IntPtr _, string pszActionName)
        {
            return SteamEmulator.SteamController.GetAnalogActionHandle(pszActionName);
        }

        public IntPtr GetAnalogActionData(IntPtr _, ControllerHandle_t controllerHandle, ControllerAnalogActionHandle_t analogActionHandle)
        {
            return SteamEmulator.SteamController.GetAnalogActionData(controllerHandle, analogActionHandle);
        }

        public int GetAnalogActionOrigins(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle, ControllerAnalogActionHandle_t analogActionHandle, int originsOut)
        {
            return SteamEmulator.SteamController.GetAnalogActionOrigins(controllerHandle, actionSetHandle, analogActionHandle, originsOut);
        }

        public string GetGlyphForActionOrigin(IntPtr _, int eOrigin)
        {
            return SteamEmulator.SteamController.GetGlyphForActionOrigin(eOrigin);
        }

        public string GetStringForActionOrigin(IntPtr _, int eOrigin)
        {
            return SteamEmulator.SteamController.GetStringForActionOrigin(eOrigin);
        }

        public void StopAnalogActionMomentum(IntPtr _, ControllerHandle_t controllerHandle, ControllerAnalogActionHandle_t eAction)
        {
            SteamEmulator.SteamController.StopAnalogActionMomentum(controllerHandle, eAction);
        }

        public IntPtr GetMotionData(IntPtr _, ControllerHandle_t controllerHandle)
        {
            return SteamEmulator.SteamController.GetMotionData(controllerHandle);
        }

        public void TriggerHapticPulse(IntPtr _, ControllerHandle_t controllerHandle, int eTargetPad, short usDurationMicroSec)
        {
            SteamEmulator.SteamController.TriggerHapticPulse(controllerHandle, eTargetPad, usDurationMicroSec);
        }

        public void TriggerRepeatedHapticPulse(IntPtr _, ControllerHandle_t controllerHandle, int eTargetPad, short usDurationMicroSec, short usOffMicroSec, short unRepeat, int nFlags)
        {
            SteamEmulator.SteamController.TriggerRepeatedHapticPulse(controllerHandle, eTargetPad, usDurationMicroSec, usOffMicroSec, unRepeat, nFlags);
        }

        public void TriggerVibration(IntPtr _, ControllerHandle_t controllerHandle, short usLeftSpeed, short usRightSpeed)
        {
            SteamEmulator.SteamController.TriggerVibration(controllerHandle, usLeftSpeed, usRightSpeed);
        }

        public void SetLEDColor(IntPtr _, ControllerHandle_t controllerHandle, int nColorR, int nColorG, int nColorB, int nFlags)
        {
            SteamEmulator.SteamController.SetLEDColor(controllerHandle, nColorR, nColorG, nColorB, nFlags);
        }

        public bool ShowBindingPanel(IntPtr _, ControllerHandle_t controllerHandle)
        {
            return SteamEmulator.SteamController.ShowBindingPanel(controllerHandle);
        }

        public int GetInputTypeForHandle(IntPtr _, ControllerHandle_t controllerHandle)
        {
            return SteamEmulator.SteamController.GetInputTypeForHandle(controllerHandle);
        }

        public ControllerHandle_t GetControllerForGamepadIndex(IntPtr _, int nIndex)
        {
            return SteamEmulator.SteamController.GetControllerForGamepadIndex(nIndex);
        }

        public int GetGamepadIndexForController(IntPtr _, ControllerHandle_t ulControllerHandle)
        {
            return SteamEmulator.SteamController.GetGamepadIndexForController(ulControllerHandle);
        }

        public string GetStringForXboxOrigin(IntPtr _, int eOrigin)
        {
            return SteamEmulator.SteamController.GetStringForXboxOrigin(eOrigin);
        }

        public string GetGlyphForXboxOrigin(IntPtr _, int eOrigin)
        {
            return SteamEmulator.SteamController.GetGlyphForXboxOrigin(eOrigin);
        }

        public int GetActionOriginFromXboxOrigin_(IntPtr _, ControllerHandle_t controllerHandle, int eOrigin)
        {
            return SteamEmulator.SteamController.GetActionOriginFromXboxOrigin_(controllerHandle, eOrigin);
        }

        public int TranslateActionOrigin(IntPtr _, int eDestinationInputType, int eSourceOrigin)
        {
            return SteamEmulator.SteamController.TranslateActionOrigin(eDestinationInputType, eSourceOrigin);
        }

        public bool GetControllerBindingRevision(IntPtr _, ControllerHandle_t controllerHandle, int pMajor, int pMinor)
        {
            return SteamEmulator.SteamController.GetControllerBindingRevision(controllerHandle, pMajor, pMinor);
        }
    }
}
