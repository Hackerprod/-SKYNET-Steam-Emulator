using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Steamworks;

using ControllerHandle_t = System.UInt64;
using ControllerActionSetHandle_t = System.UInt64;
using ControllerDigitalActionHandle_t = System.UInt64;
using ControllerAnalogActionHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamController
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_ActivateActionSet(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle)
        {
            Write("SteamAPI_ISteamController_ActivateActionSet");
            SteamEmulator.SteamController.ActivateActionSet(controllerHandle, controllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_ActivateActionSetLayer(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetLayerHandle)
        {
            Write("SteamAPI_ISteamController_ActivateActionSetLayer");
            SteamEmulator.SteamController.ActivateActionSetLayer(controllerHandle, actionSetLayerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_DeactivateActionSetLayer(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetLayerHandle)
        {
            Write("SteamAPI_ISteamController_DeactivateActionSetLayer");
            SteamEmulator.SteamController.DeactivateActionSetLayer(controllerHandle, actionSetLayerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_DeactivateAllActionSetLayers(IntPtr _, ControllerHandle_t controllerHandle)
        {
            Write("SteamAPI_ISteamController_DeactivateAllActionSetLayers");
            SteamEmulator.SteamController.DeactivateAllActionSetLayers(controllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetActionOriginFromXboxOrigin_(IntPtr _, ControllerHandle_t controllerHandle, int eOrigin)
        {
            Write("SteamAPI_ISteamController_GetActionOriginFromXboxOrigin_");
            return SteamEmulator.SteamController.GetActionOriginFromXboxOrigin_(controllerHandle, eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ControllerActionSetHandle_t SteamAPI_ISteamController_GetActionSetHandle(IntPtr _, string pszActionSetName)
        {
            Write("SteamAPI_ISteamController_GetActionSetHandle");
            return SteamEmulator.SteamController.GetActionSetHandle(pszActionSetName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetActiveActionSetLayers(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t handlesOut)
        {
            Write("SteamAPI_ISteamController_GetActiveActionSetLayers");
            return SteamEmulator.SteamController.GetActiveActionSetLayers(controllerHandle, handlesOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamController_GetAnalogActionData(IntPtr _, uint controllerHandle, uint analogActionHandle)
        {
            Write("SteamAPI_ISteamController_GetAnalogActionData");
            return SteamEmulator.SteamController.GetAnalogActionData(controllerHandle, analogActionHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ControllerActionSetHandle_t GetAnalogActionHandle(IntPtr _, string pszActionName)
        {
            Write("SteamAPI_ISteamController_GetAnalogActionHandle");
            return SteamEmulator.SteamController.GetActionSetHandle(pszActionName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetAnalogActionOrigins(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle, uint analogActionHandle, int originsOut)
        {
            Write("SteamAPI_ISteamController_GetAnalogActionOrigins");
            return SteamEmulator.SteamController.GetAnalogActionOrigins(controllerHandle, actionSetHandle, analogActionHandle, originsOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetConnectedControllers(IntPtr _, ControllerHandle_t handlesOut)
        {
            Write("SteamAPI_ISteamController_GetConnectedControllers");
            return SteamEmulator.SteamController.GetConnectedControllers(handlesOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamController_GetControllerBindingRevision(IntPtr _, ControllerHandle_t controllerHandle, int pMajor, int pMinor)
        {
            Write("SteamAPI_ISteamController_GetControllerBindingRevision");
            return SteamEmulator.SteamController.GetControllerBindingRevision(controllerHandle, pMajor, pMinor);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ControllerHandle_t SteamAPI_ISteamController_GetControllerForGamepadIndex(IntPtr _, int nIndex)
        {
            Write("SteamAPI_ISteamController_GetControllerForGamepadIndex");
            return SteamEmulator.SteamController.GetControllerForGamepadIndex(nIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ControllerActionSetHandle_t SteamAPI_ISteamController_GetCurrentActionSet(IntPtr _, ControllerHandle_t controllerHandle)
        {
            Write("SteamAPI_ISteamController_GetCurrentActionSet");
            return SteamEmulator.SteamController.GetCurrentActionSet(controllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamController_GetDigitalActionData(IntPtr _, ControllerHandle_t controllerHandle, ControllerDigitalActionHandle_t digitalActionHandle)
        {
            Write("SteamAPI_ISteamController_GetDigitalActionData");
            return SteamEmulator.SteamController.GetDigitalActionData(controllerHandle, digitalActionHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ControllerDigitalActionHandle_t SteamAPI_ISteamController_GetDigitalActionHandle(IntPtr _, string pszActionName)
        {
            Write("SteamAPI_ISteamController_GetDigitalActionHandle");
            return SteamEmulator.SteamController.GetDigitalActionHandle(pszActionName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetDigitalActionOrigins(IntPtr _, ControllerHandle_t controllerHandle, ControllerActionSetHandle_t actionSetHandle, ControllerDigitalActionHandle_t digitalActionHandle, int originsOut)
        {
            Write("SteamAPI_ISteamController_GetDigitalActionOrigins");
            return SteamEmulator.SteamController.GetDigitalActionOrigins(controllerHandle, actionSetHandle, digitalActionHandle, originsOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetGamepadIndexForController(IntPtr _, ControllerHandle_t ulControllerHandle)
        {
            Write("SteamAPI_ISteamController_GetGamepadIndexForController");
            return SteamEmulator.SteamController.GetGamepadIndexForController(ulControllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamController_GetGlyphForActionOrigin(IntPtr _, int eOrigin)
        {
            Write("SteamAPI_ISteamController_GetGlyphForActionOrigin");
            return SteamEmulator.SteamController.GetGlyphForActionOrigin(eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamController_GetGlyphForXboxOrigin(IntPtr _, int eOrigin)
        {
            Write("SteamAPI_ISteamController_GetGlyphForXboxOrigin");
            return SteamEmulator.SteamController.GetGlyphForXboxOrigin(eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetInputTypeForHandle(IntPtr _, ControllerHandle_t controllerHandle)
        {
            Write("SteamAPI_ISteamController_GetInputTypeForHandle");
            return SteamEmulator.SteamController.GetInputTypeForHandle(controllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamController_GetMotionData(IntPtr _, ControllerHandle_t controllerHandle)
        {
            Write("SteamAPI_ISteamController_GetMotionData");
            return SteamEmulator.SteamController.GetMotionData(controllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamController_GetStringForActionOrigin(IntPtr _, int eOrigin)
        {
            Write("SteamAPI_ISteamController_GetStringForActionOrigin");
            return SteamEmulator.SteamController.GetStringForActionOrigin(eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamController_GetStringForXboxOrigin(IntPtr _, int eOrigin)
        {
            Write("SteamAPI_ISteamController_GetStringForXboxOrigin");
            return SteamEmulator.SteamController.GetStringForXboxOrigin(eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamController_Init(IntPtr _)
        {
            Write("SteamAPI_ISteamController_Init");
            return SteamEmulator.SteamController.Init();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_RunFrame(IntPtr _)
        {
            Write("SteamAPI_ISteamController_RunFrame");
            SteamEmulator.SteamController.RunFrame();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_SetLEDColor(IntPtr _, ControllerHandle_t controllerHandle, int nColorR, int nColorG, int nColorB, int nFlags)
        {
            Write("SteamAPI_ISteamController_SetLEDColor");
            SteamEmulator.SteamController.SetLEDColor(controllerHandle, nColorR, nColorG, nColorB, nFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamController_ShowBindingPanel(IntPtr _, ControllerHandle_t controllerHandle)
        {
            Write("SteamAPI_ISteamController_ShowBindingPanel");
            return SteamEmulator.SteamController.ShowBindingPanel(controllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamController_Shutdown(IntPtr _)
        {
            Write("SteamAPI_ISteamController_Shutdown");
            return SteamEmulator.SteamController.Shutdown();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_StopAnalogActionMomentum(IntPtr _, ControllerHandle_t controllerHandle, uint eAction)
        {
            Write("SteamAPI_ISteamController_StopAnalogActionMomentum");
            SteamEmulator.SteamController.StopAnalogActionMomentum(controllerHandle, eAction);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_TranslateActionOrigin(IntPtr _, int eDestinationInputType, int eSourceOrigin)
        {
            Write("SteamAPI_ISteamController_TranslateActionOrigin");
            return SteamEmulator.SteamController.TranslateActionOrigin(eDestinationInputType, eSourceOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_TriggerHapticPulse(IntPtr _, ControllerHandle_t controllerHandle, int eTargetPad, short usDurationMicroSec)
        {
            Write("SteamAPI_ISteamController_TriggerHapticPulse");
            SteamEmulator.SteamController.TriggerHapticPulse(controllerHandle, eTargetPad, usDurationMicroSec);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_TriggerRepeatedHapticPulse(IntPtr _, ControllerHandle_t controllerHandle, short eTargetPad, short usDurationMicroSec, short usOffMicroSec, short unRepeat, int nFlags)
        {
            Write("SteamAPI_ISteamController_TriggerRepeatedHapticPulse");
            SteamEmulator.SteamController.TriggerRepeatedHapticPulse(controllerHandle, eTargetPad, usDurationMicroSec, usOffMicroSec, unRepeat, nFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_TriggerVibration(IntPtr _, ControllerHandle_t controllerHandle, short usLeftSpeed, short usRightSpeed)
        {
            Write("SteamAPI_ISteamController_TriggerVibration");
            SteamEmulator.SteamController.TriggerVibration(controllerHandle, usLeftSpeed, usRightSpeed);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}