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
    [Interface("SteamInput001")] // Cheched (some SteamInput002)
    [Interface("SteamInput002")]
    public class SteamInput002 : ISteamInterface
    {
        public bool Init(IntPtr _)
        {
            return SteamEmulator.SteamInput.Init();
        }

        public bool Shutdown(IntPtr _)
        {
            return SteamEmulator.SteamInput.Shutdown();
        }

        public void RunFrame(IntPtr _)
        {
            SteamEmulator.SteamInput.RunFrame();
        }

        public int GetConnectedControllers(IntPtr _, ref InputHandle_t[] handlesOut)
        {
            return SteamEmulator.SteamInput.GetConnectedControllers(ref handlesOut);
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

        public string GetGlyphForActionOrigin(IntPtr _, int eOrigin)
        {
            return SteamEmulator.SteamInput.GetGlyphForActionOrigin(eOrigin);
        }

        public string GetStringForActionOrigin(IntPtr _, int eOrigin)
        {
            return SteamEmulator.SteamInput.GetStringForActionOrigin(eOrigin);
        }

        public void StopAnalogActionMomentum(IntPtr _, InputHandle_t inputHandle, InputAnalogActionHandle_t eAction)
        {
            SteamEmulator.SteamInput.StopAnalogActionMomentum(inputHandle, eAction);
        }

        public IntPtr GetMotionData(IntPtr _, InputHandle_t inputHandle)
        {
            return SteamEmulator.SteamInput.GetMotionData(inputHandle);
        }

        public void TriggerVibration(IntPtr _, InputHandle_t inputHandle, short usLeftSpeed, short usRightSpeed)
        {
            SteamEmulator.SteamInput.TriggerVibration(inputHandle, usLeftSpeed, usRightSpeed);
        }

        public void SetLEDColor(IntPtr _, InputHandle_t inputHandle, int nColorR, int nColorG, int nColorB, int nFlags)
        {
            SteamEmulator.SteamInput.SetLEDColor(inputHandle, nColorR, nColorG, nColorB, nFlags);
        }

        public void TriggerHapticPulse(IntPtr _, InputHandle_t inputHandle, ESteamControllerPad eTargetPad, short usDurationMicroSec)
        {
            SteamEmulator.SteamInput.TriggerHapticPulse(inputHandle, eTargetPad, usDurationMicroSec);
        }

        public void TriggerRepeatedHapticPulse(IntPtr _, InputHandle_t inputHandle, ESteamControllerPad eTargetPad, short usDurationMicroSec, short usOffMicroSec, short unRepeat, int nFlags)
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
    }
}
