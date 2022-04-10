using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    [StructLayout(LayoutKind.Sequential)]
    public class SteamController : ISteamInterface
    {
        public void ActivateActionSet(IntPtr controllerHandle, int actionSetHandle)
        {
            Write("ActivateActionSet");
        }

        public void ActivateActionSetLayer(IntPtr controllerHandle, IntPtr actionSetLayerHandle)
        {
            Write("ActivateActionSetLayer");
        }

        public void DeactivateActionSetLayer(IntPtr controllerHandle, IntPtr actionSetLayerHandle)
        {
            Write("DeactivateActionSetLayer");
        }

        public void DeactivateAllActionSetLayers(IntPtr controllerHandle)
        {
            Write("DeactivateAllActionSetLayers");
        }

        public EControllerActionOrigin GetActionOriginFromXboxOrigin_(IntPtr controllerHandle, EXboxOrigin eOrigin)
        {
            Write("GetActionOriginFromXboxOrigin_");
            return EControllerActionOrigin.k_EControllerActionOrigin_None;
        }

        public int GetActionSetHandle(string pszActionSetName)
        {
            Write("GetActionSetHandle");
            if (string.IsNullOrEmpty(pszActionSetName))
            {
                return 0;
            }
            if (action_handles.ContainsKey(pszActionSetName.ToUpper()))
            {
                return action_handles[pszActionSetName.ToUpper()];
            }
            return 0;
        }

        public int GetActiveActionSetLayers(IntPtr controllerHandle, int handlesOut)
        {
            Write("GetActiveActionSetLayers");
            return 0;
        }

        public IntPtr GetAnalogActionData(IntPtr _, uint controllerHandle, uint analogActionHandle)
        {
            Write("GetAnalogActionData");
            //ControllerAnalogActionData_t data = new ControllerAnalogActionData_t();
            //data.eMode = 0;
            //data.x = data.y = 0;
            //data.bActive = false;
            return IntPtr.Zero;
        }

        public int GetAnalogActionHandle(string pszActionName)
        {
            Write("GetAnalogActionHandle");
            if (string.IsNullOrEmpty(pszActionName))
            {
                return 0;
            }
            if (analog_action_handles.ContainsKey(pszActionName.ToUpper()))
            {
                return analog_action_handles[pszActionName.ToUpper()];
            }
            return 0;
        }

        public int GetAnalogActionOrigins(IntPtr controllerHandle, int actionSetHandle, uint analogActionHandle, EControllerActionOrigin originsOut)
        {
            Write("GetAnalogActionOrigins");
            return 0;
        }

        public int GetConnectedControllers(IntPtr handles, IntPtr handlesOut)
        {
            Write("GetConnectedControllers");
            return 0;
        }

        public bool GetControllerBindingRevision(IntPtr controllerHandle, int pMajor, int pMinor)
        {
            Write("GetControllerBindingRevision");
            return false;
        }

        public int GetControllerForGamepadIndex(int nIndex)
        {
            Write("GetControllerForGamepadIndex");
            return 0;
        }

        public int GetCurrentActionSet(IntPtr controllerHandle)
        {
            Write("GetCurrentActionSet");
            return 0;
        }

        public IntPtr GetDigitalActionData(IntPtr _, IntPtr controllerHandle, int digitalActionHandle)
        {
            Write("GetDigitalActionData");
            ControllerDigitalActionData_t digitalData = new ControllerDigitalActionData_t();
            digitalData.bActive = false;
            digitalData.bState = false;
            return IntPtr.Zero;
        }

        public int GetDigitalActionHandle(string pszActionName)
        {
            Write("GetDigitalActionHandle");
            if (string.IsNullOrEmpty(pszActionName))
            {
                return 0;
            }
            if (digital_action_handles.ContainsKey(pszActionName.ToUpper()))
            {
                return digital_action_handles[pszActionName.ToUpper()];
            }
            return 0;
        }

        public int GetDigitalActionOrigins(IntPtr controllerHandle, int actionSetHandle, int digitalActionHandle, EControllerActionOrigin originsOut)
        {
            Write("GetDigitalActionOrigins");
            return 0;
        }

        public int GetGamepadIndexForController(IntPtr ulControllerHandle)
        {
            Write("GetGamepadIndexForController");
            return 0;
        }

        public string GetGlyphForActionOrigin(EControllerActionOrigin eOrigin)
        {
            Write("GetGlyphForActionOrigin");
            return "";
        }

        public string GetGlyphForXboxOrigin(EXboxOrigin eOrigin)
        {
            Write("GetGlyphForXboxOrigin");
            return "";
        }

        public ESteamInputType GetInputTypeForHandle(IntPtr controllerHandle)
        {
            Write("GetInputTypeForHandle");
            return 0;
        }

        public ControllerMotionData_t GetMotionData(IntPtr controllerHandle)
        {
            Write("GetMotionData");
            return default;
        }

        public string GetStringForActionOrigin(EControllerActionOrigin eOrigin)
        {
            Write("GetStringForActionOrigin");
            return "";
        }

        public string GetStringForXboxOrigin(EXboxOrigin eOrigin)
        {
            Write("GetStringForXboxOrigin");
            return "";
        }

        public bool Init(IntPtr _)
        {
            Write("Init");
            return true;
        }

        public void RunFrame(IntPtr _)
        {
            Write("RunFrame");
        }

        public void SetLEDColor(IntPtr controllerHandle, uint nColorR, uint nColorG, uint nColorB, int nFlags)
        {
            Write("SetLEDColor");
        }

        public bool ShowBindingPanel(IntPtr controllerHandle)
        {
            Write("ShowBindingPanel");
            return true;
        }

        public bool Shutdown(IntPtr _)
        {
            Write("Shutdown");
            return true;
        }

        public void StopAnalogActionMomentum(IntPtr controllerHandle, uint eAction)
        {
            Write("StopAnalogActionMomentum");
        }

        public EControllerActionOrigin TranslateActionOrigin(ESteamInputType eDestinationInputType, EControllerActionOrigin eSourceOrigin)
        {
            Write("TranslateActionOrigin");
            return default;
        }

        public void TriggerHapticPulse(IntPtr controllerHandle, ESteamControllerPad eTargetPad, uint usDurationMicroSec)
        {
            Write("TriggerHapticPulse");
        }

        public void TriggerRepeatedHapticPulse(IntPtr controllerHandle, ESteamControllerPad eTargetPad, uint usDurationMicroSec, uint usOffMicroSec, uint unRepeat, int nFlags)
        {
            Write("TriggerRepeatedHapticPulse");
        }

        public void TriggerVibration(IntPtr controllerHandle, uint usLeftSpeed, uint usRightSpeed)
        {
            Write("TriggerVibration");
        }

        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }


        private Dictionary<string, int> action_handles;
        private Dictionary<string, int> digital_action_handles;
        private Dictionary<string, int> analog_action_handles;

        public SteamController()
        {
            action_handles = new Dictionary<string, int>();
            digital_action_handles = new Dictionary<string, int>();
            analog_action_handles = new Dictionary<string, int>();
            InterfaceVersion = "SteamController";
        }
        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}
