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
        public void ActivateActionSet(ulong controllerHandle, ulong actionSetHandle)
        {
            Write("ActivateActionSet");
        }

        public void ActivateActionSetLayer(ulong controllerHandle, ulong actionSetLayerHandle)
        {
            Write("ActivateActionSetLayer");
        }

        public void DeactivateActionSetLayer(ulong controllerHandle, ulong actionSetLayerHandle)
        {
            Write("DeactivateActionSetLayer");
        }

        public void DeactivateAllActionSetLayers(ulong controllerHandle)
        {
            Write("DeactivateAllActionSetLayers");
        }

        public int GetActionOriginFromXboxOrigin_(ulong controllerHandle, int eOrigin)
        {
            Write("GetActionOriginFromXboxOrigin_");
            return 0;
        }

        public ulong GetActionSetHandle(string pszActionSetName)
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

        public int GetActiveActionSetLayers(ulong controllerHandle, ulong handlesOut)
        {
            Write("GetActiveActionSetLayers");
            return 0;
        }

        public IntPtr GetAnalogActionData(ulong controllerHandle, ulong analogActionHandle)
        {
            Write("GetAnalogActionData");
            //ControllerAnalogActionData_t data = new ControllerAnalogActionData_t();
            //data.eMode = 0;
            //data.x = data.y = 0;
            //data.bActive = false;
            return IntPtr.Zero;
        }

        public ulong GetAnalogActionHandle(string pszActionName)
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

        public int GetAnalogActionOrigins(ulong controllerHandle, ulong actionSetHandle, ulong analogActionHandle, int originsOut)
        {
            Write("GetAnalogActionOrigins");
            return 0;
        }

        public int GetConnectedControllers(ulong handlesOut)
        {
            Write("GetConnectedControllers");
            return 0;
        }

        public bool GetControllerBindingRevision(ulong controllerHandle, int pMajor, int pMinor)
        {
            Write("GetControllerBindingRevision");
            return false;
        }

        public ulong GetControllerForGamepadIndex(int nIndex)
        {
            Write("GetControllerForGamepadIndex");
            return 0;
        }

        public ulong GetCurrentActionSet(ulong controllerHandle)
        {
            Write("GetCurrentActionSet");
            return 0;
        }

        public IntPtr GetDigitalActionData(ulong controllerHandle, ulong digitalActionHandle)
        {
            Write("GetDigitalActionData");
            ControllerDigitalActionData_t digitalData = new ControllerDigitalActionData_t();
            digitalData.bActive = false;
            digitalData.bState = false;
            return IntPtr.Zero;
        }

        public ulong GetDigitalActionHandle(string pszActionName)
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

        public int GetDigitalActionOrigins(ulong controllerHandle, ulong actionSetHandle, ulong digitalActionHandle, int originsOut)
        {
            Write("GetDigitalActionOrigins");
            return 0;
        }

        public int GetGamepadIndexForController(ulong ulControllerHandle)
        {
            Write("GetGamepadIndexForController");
            return 0;
        }

        public string GetGlyphForActionOrigin(int eOrigin)
        {
            Write("GetGlyphForActionOrigin");
            return "";
        }

        public string GetGlyphForXboxOrigin(int eOrigin)
        {
            Write("GetGlyphForXboxOrigin");
            return "";
        }

        public int GetInputTypeForHandle(ulong controllerHandle)
        {
            Write("GetInputTypeForHandle");
            return 0;
        }

        public IntPtr GetMotionData(ulong controllerHandle)
        {
            Write("GetMotionData");
            return default;
        }

        public string GetStringForActionOrigin(int eOrigin)
        {
            Write("GetStringForActionOrigin");
            return "";
        }

        public string GetStringForXboxOrigin(int eOrigin)
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

        public void SetLEDColor(ulong controllerHandle, int nColorR, int nColorG, int nColorB, int nFlags)
        {
            Write("SetLEDColor");
        }

        public bool ShowBindingPanel(ulong controllerHandle)
        {
            Write("ShowBindingPanel");
            return true;
        }

        public bool Shutdown(IntPtr _)
        {
            Write("Shutdown");
            return true;
        }

        public void StopAnalogActionMomentum(ulong controllerHandle, ulong eAction)
        {
            Write("StopAnalogActionMomentum");
        }

        public int TranslateActionOrigin(int eDestinationInputType, int eSourceOrigin)
        {
            Write("TranslateActionOrigin");
            return default;
        }

        public void TriggerHapticPulse(ulong controllerHandle, int eTargetPad, short usDurationMicroSec)
        {
            Write("TriggerHapticPulse");
        }

        public void TriggerRepeatedHapticPulse(ulong controllerHandle, int eTargetPad, short usDurationMicroSec, short usOffMicroSec, short unRepeat, int nFlags)
        {
            Write("TriggerRepeatedHapticPulse");
        }

        public void TriggerVibration(ulong controllerHandle, short usLeftSpeed, short usRightSpeed)
        {
            Write("TriggerVibration");
        }

        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }


        private Dictionary<string, ulong> action_handles;
        private Dictionary<string, ulong> digital_action_handles;
        private Dictionary<string, ulong> analog_action_handles;

        public SteamController()
        {
            action_handles = new Dictionary<string, ulong>();
            digital_action_handles = new Dictionary<string, ulong>();
            analog_action_handles = new Dictionary<string, ulong>();
            InterfaceVersion = "SteamController";
        }
        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}
