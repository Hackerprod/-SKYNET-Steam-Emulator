using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;

using ControllerHandle_t = System.UInt64;
using ControllerActionSetHandle_t = System.UInt64;
using ControllerDigitalActionHandle_t = System.UInt64;
using ControllerAnalogActionHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamController : ISteamInterface
    {
        private Dictionary<string, ControllerActionSetHandle_t> ActionHandles;
        private Dictionary<string, ControllerDigitalActionHandle_t> DigitalHandles;
        private Dictionary<string, ControllerAnalogActionHandle_t> AnalogHandles;

        public SteamController()
        {
            ActionHandles = new Dictionary<string, ControllerActionSetHandle_t>();
            DigitalHandles = new Dictionary<string, ControllerDigitalActionHandle_t>();
            AnalogHandles = new Dictionary<string, ControllerAnalogActionHandle_t>();
            InterfaceVersion = "SteamController";
        }

        public void ActivateActionSet(ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle)
        {
            Write("ActivateActionSet");
        }

        public void ActivateActionSetLayer(ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetLayerHandle)
        {
            Write("ActivateActionSetLayer");
        }

        public void DeactivateActionSetLayer(ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetLayerHandle)
        {
            Write("DeactivateActionSetLayer");
        }

        public void DeactivateAllActionSetLayers(ControllerHandle_t controllerHandle)
        {
            Write("DeactivateAllActionSetLayers");
        }

        public int GetActionOriginFromXboxOrigin_(ControllerHandle_t controllerHandle, int eOrigin)
        {
            Write("GetActionOriginFromXboxOrigin_");
            return 0;
        }

        public ControllerActionSetHandle_t GetActionSetHandle(string pszActionSetName)
        {
            Write("GetActionSetHandle");
            if (string.IsNullOrEmpty(pszActionSetName))
            {
                return 0;
            }
            if (ActionHandles.ContainsKey(pszActionSetName.ToUpper()))
            {
                return ActionHandles[pszActionSetName.ToUpper()];
            }
            return 0;
        }

        public int GetActiveActionSetLayers(ControllerHandle_t controllerHandle, ControllerActionSetHandle_t handlesOut)
        {
            Write("GetActiveActionSetLayers");
            return 0;
        }

        public IntPtr GetAnalogActionData(ControllerHandle_t controllerHandle, ControllerAnalogActionHandle_t analogActionHandle)
        {
            Write("GetAnalogActionData");
            //ControllerAnalogActionData_t data = new ControllerAnalogActionData_t();
            //data.eMode = 0;
            //data.x = data.y = 0;
            //data.bActive = false;
            return IntPtr.Zero;
        }

        public ControllerAnalogActionHandle_t GetAnalogActionHandle(string pszActionName)
        {
            Write("GetAnalogActionHandle");
            if (string.IsNullOrEmpty(pszActionName))
            {
                return 0;
            }
            if (AnalogHandles.ContainsKey(pszActionName.ToUpper()))
            {
                return AnalogHandles[pszActionName.ToUpper()];
            }
            return 0;
        }

        public int GetAnalogActionOrigins(ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle, ControllerAnalogActionHandle_t analogActionHandle, int originsOut)
        {
            Write("GetAnalogActionOrigins");
            return 0;
        }

        public int GetConnectedControllers(ControllerHandle_t handlesOut)
        {
            Write("GetConnectedControllers");
            return 0;
        }

        public bool GetControllerBindingRevision(ControllerHandle_t controllerHandle, int pMajor, int pMinor)
        {
            Write("GetControllerBindingRevision");
            return false;
        }

        public ControllerHandle_t GetControllerForGamepadIndex(int nIndex)
        {
            Write("GetControllerForGamepadIndex");
            return 0;
        }

        public ControllerActionSetHandle_t GetCurrentActionSet(ControllerHandle_t controllerHandle)
        {
            Write("GetCurrentActionSet");
            return 0;
        }

        public IntPtr GetDigitalActionData(ControllerHandle_t controllerHandle, ControllerDigitalActionHandle_t digitalActionHandle)
        {
            Write("GetDigitalActionData");
            ControllerDigitalActionData_t digitalData = new ControllerDigitalActionData_t();
            digitalData.bActive = false;
            digitalData.bState = false;
            return IntPtr.Zero;
        }

        public ControllerDigitalActionHandle_t GetDigitalActionHandle(string pszActionName)
        {
            Write("GetDigitalActionHandle");
            if (string.IsNullOrEmpty(pszActionName))
            {
                return 0;
            }
            if (DigitalHandles.ContainsKey(pszActionName.ToUpper()))
            {
                return DigitalHandles[pszActionName.ToUpper()];
            }
            return 0;
        }

        public int GetDigitalActionOrigins(ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle, ControllerDigitalActionHandle_t digitalActionHandle, int originsOut)
        {
            Write("GetDigitalActionOrigins");
            return 0;
        }

        public int GetGamepadIndexForController(ControllerHandle_t ulControllerHandle)
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

        public int GetInputTypeForHandle(ControllerHandle_t controllerHandle)
        {
            Write("GetInputTypeForHandle");
            return 0;
        }

        public IntPtr GetMotionData(ControllerHandle_t controllerHandle)
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

        public bool Init()
        {
            Write("Init");
            return true;
        }

        public void RunFrame()
        {
            Write("RunFrame");
        }

        public void SetLEDColor(ControllerHandle_t controllerHandle, int nColorR, int nColorG, int nColorB, int nFlags)
        {
            Write("SetLEDColor");
        }

        public bool ShowBindingPanel(ControllerHandle_t controllerHandle)
        {
            Write("ShowBindingPanel");
            return true;
        }

        public bool Shutdown()
        {
            Write("Shutdown");
            return true;
        }

        public void StopAnalogActionMomentum(ControllerHandle_t controllerHandle, ControllerAnalogActionHandle_t eAction)
        {
            Write("StopAnalogActionMomentum");
        }

        public int TranslateActionOrigin(int eDestinationInputType, int eSourceOrigin)
        {
            Write("TranslateActionOrigin");
            return default;
        }

        public void TriggerHapticPulse(ControllerHandle_t controllerHandle, int eTargetPad, short usDurationMicroSec)
        {
            Write("TriggerHapticPulse");
        }

        public void TriggerRepeatedHapticPulse(ControllerHandle_t controllerHandle, int eTargetPad, short usDurationMicroSec, short usOffMicroSec, short unRepeat, int nFlags)
        {
            Write("TriggerRepeatedHapticPulse");
        }

        public void TriggerVibration(ControllerHandle_t controllerHandle, short usLeftSpeed, short usRightSpeed)
        {
            Write("TriggerVibration");
        }
    }
}
