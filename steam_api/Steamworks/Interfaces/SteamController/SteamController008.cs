using System;

namespace SKYNET.Interface
{
    [Interface("SteamController008")]
    public class SteamController008 : ISteamInterface
    {
        public bool Init(IntPtr _)
        {
            return SteamEmulator.SteamController.Init(_);
        }

        public bool Shutdown(IntPtr _)
        {
            return SteamEmulator.SteamController.Shutdown(_);
        }

        public void RunFrame(IntPtr _)
        {
            SteamEmulator.SteamController.RunFrame(_);
        }

        public int GetConnectedControllers(IntPtr _, ulong handlesOut)
        {
            return SteamEmulator.SteamController.GetConnectedControllers(handlesOut);
        }

        public ulong GetActionSetHandle(IntPtr _, string pszActionSetName)
        {
            return SteamEmulator.SteamController.GetActionSetHandle(pszActionSetName);
        }

        public void ActivateActionSet(IntPtr _, ulong controllerHandle, ulong actionSetHandle)
        {
            SteamEmulator.SteamController.ActivateActionSet(controllerHandle, actionSetHandle);
        }

        public ulong GetCurrentActionSet(IntPtr _, ulong controllerHandle)
        {
            return SteamEmulator.SteamController.GetCurrentActionSet(controllerHandle);
        }

        public void ActivateActionSetLayer(IntPtr _, ulong controllerHandle, ulong actionSetLayerHandle)
        {
            SteamEmulator.SteamController.ActivateActionSetLayer(controllerHandle, actionSetLayerHandle);
        }

        public void DeactivateActionSetLayer(IntPtr _, ulong controllerHandle, ulong actionSetLayerHandle)
        {
            SteamEmulator.SteamController.DeactivateActionSetLayer(controllerHandle, actionSetLayerHandle);
        }

        public void DeactivateAllActionSetLayers(IntPtr _, ulong controllerHandle)
        {
            SteamEmulator.SteamController.DeactivateAllActionSetLayers(controllerHandle);
        }

        public int GetActiveActionSetLayers(IntPtr _, ulong controllerHandle, ulong handlesOut)
        {
            return SteamEmulator.SteamController.GetActiveActionSetLayers(controllerHandle, handlesOut);
        }

        public ulong GetDigitalActionHandle(IntPtr _, string pszActionName)
        {
            return SteamEmulator.SteamController.GetDigitalActionHandle(pszActionName);
        }

        public IntPtr GetDigitalActionData(IntPtr _, ulong controllerHandle, ulong digitalActionHandle)
        {
            return SteamEmulator.SteamController.GetDigitalActionData(controllerHandle, digitalActionHandle);
        }

        public int GetDigitalActionOrigins(IntPtr _, ulong controllerHandle, ulong actionSetHandle, ulong digitalActionHandle, int originsOut)
        {
            return SteamEmulator.SteamController.GetDigitalActionOrigins(controllerHandle, actionSetHandle, digitalActionHandle, originsOut);
        }

        public ulong GetAnalogActionHandle(IntPtr _, string pszActionName)
        {
            return SteamEmulator.SteamController.GetAnalogActionHandle(pszActionName);
        }

        public IntPtr GetAnalogActionData(IntPtr _, ulong controllerHandle, ulong analogActionHandle)
        {
            return SteamEmulator.SteamController.GetAnalogActionData(controllerHandle, analogActionHandle);
        }

        public int GetAnalogActionOrigins(IntPtr _, ulong controllerHandle, ulong actionSetHandle, ulong analogActionHandle, int originsOut)
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

        public void StopAnalogActionMomentum(IntPtr _, ulong controllerHandle, ulong eAction)
        {
            SteamEmulator.SteamController.StopAnalogActionMomentum(controllerHandle, eAction);
        }

        public IntPtr GetMotionData(IntPtr _, ulong controllerHandle)
        {
            return SteamEmulator.SteamController.GetMotionData(controllerHandle);
        }

        public void TriggerHapticPulse(IntPtr _, ulong controllerHandle, int eTargetPad, short usDurationMicroSec)
        {
            SteamEmulator.SteamController.TriggerHapticPulse(controllerHandle, eTargetPad, usDurationMicroSec);
        }

        public void TriggerRepeatedHapticPulse(IntPtr _, ulong controllerHandle, int eTargetPad, short usDurationMicroSec, short usOffMicroSec, short unRepeat, int nFlags)
        {
            SteamEmulator.SteamController.TriggerRepeatedHapticPulse(controllerHandle, eTargetPad, usDurationMicroSec, usOffMicroSec, unRepeat, nFlags);
        }

        public void TriggerVibration(IntPtr _, ulong controllerHandle, short usLeftSpeed, short usRightSpeed)
        {
            SteamEmulator.SteamController.TriggerVibration(controllerHandle, usLeftSpeed, usRightSpeed);
        }

        public void SetLEDColor(IntPtr _, ulong controllerHandle, int nColorR, int nColorG, int nColorB, int nFlags)
        {
            SteamEmulator.SteamController.SetLEDColor(controllerHandle, nColorR, nColorG, nColorB, nFlags);
        }

        public bool ShowBindingPanel(IntPtr _, ulong controllerHandle)
        {
            return SteamEmulator.SteamController.ShowBindingPanel(controllerHandle);
        }

        public int GetInputTypeForHandle(IntPtr _, ulong controllerHandle)
        {
            return SteamEmulator.SteamController.GetInputTypeForHandle(controllerHandle);
        }

        public ulong GetControllerForGamepadIndex(IntPtr _, int nIndex)
        {
            return SteamEmulator.SteamController.GetControllerForGamepadIndex(nIndex);
        }

        public int GetGamepadIndexForController(IntPtr _, ulong ulControllerHandle)
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

        public int GetActionOriginFromXboxOrigin_(IntPtr _, ulong controllerHandle, int eOrigin)
        {
            return SteamEmulator.SteamController.GetActionOriginFromXboxOrigin_(controllerHandle, eOrigin);
        }

        public int TranslateActionOrigin(IntPtr _, int eDestinationInputType, int eSourceOrigin)
        {
            return SteamEmulator.SteamController.TranslateActionOrigin(eDestinationInputType, eSourceOrigin);
        }

        public bool GetControllerBindingRevision(IntPtr _, ulong controllerHandle, int pMajor, int pMinor)
        {
            return SteamEmulator.SteamController.GetControllerBindingRevision(controllerHandle, pMajor, pMinor);
        }
    }
}
