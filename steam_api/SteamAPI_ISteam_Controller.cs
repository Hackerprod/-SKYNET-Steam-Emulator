using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SKYNET.Interface;

namespace SKYNET.Managers
{
    public class SteamAPI_ISteamController : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_ActivateActionSet(IntPtr controllerHandle, int actionSetHandle)
        {
            Write("SteamAPI_ISteamController_ActivateActionSet");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_ActivateActionSetLayer(IntPtr controllerHandle, IntPtr actionSetLayerHandle)
        {
            Write("SteamAPI_ISteamController_ActivateActionSetLayer");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_DeactivateActionSetLayer(IntPtr controllerHandle, IntPtr actionSetLayerHandle)
        {
            Write("SteamAPI_ISteamController_DeactivateActionSetLayer");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_DeactivateAllActionSetLayers(IntPtr controllerHandle)
        {
            Write("SteamAPI_ISteamController_DeactivateAllActionSetLayers");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EControllerActionOrigin SteamAPI_ISteamController_GetActionOriginFromXboxOrigin_(IntPtr controllerHandle, EXboxOrigin eOrigin)
        {
            Write("SteamAPI_ISteamController_GetActionOriginFromXboxOrigin_");
            return SteamClient.steam_Controller.GetActionOriginFromXboxOrigin_(controllerHandle, eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetActionSetHandle(string pszActionSetName)
        {
            Write("SteamAPI_ISteamController_GetActionSetHandle");
            return SteamClient.steam_Controller.GetActionSetHandle(pszActionSetName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetActiveActionSetLayers(IntPtr controllerHandle, int handlesOut)
        {
            Write("SteamAPI_ISteamController_GetActiveActionSetLayers");
            return SteamClient.steam_Controller.GetActiveActionSetLayers(controllerHandle, handlesOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ControllerAnalogActionData_t SteamAPI_ISteamController_GetAnalogActionData(uint controllerHandle, uint analogActionHandle)
        {
            Write("SteamAPI_ISteamController_GetAnalogActionData");
            return SteamClient.steam_Controller.GetAnalogActionData(controllerHandle, analogActionHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int GetAnalogActionHandle(string pszActionName)
        {
            Write("SteamAPI_ISteamController_GetAnalogActionHandle");
            return SteamClient.steam_Controller.GetActionSetHandle(pszActionName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetAnalogActionOrigins(IntPtr controllerHandle, int actionSetHandle, uint analogActionHandle, EControllerActionOrigin originsOut)
        {
            Write("SteamAPI_ISteamController_GetAnalogActionOrigins");
            return SteamClient.steam_Controller.GetAnalogActionOrigins(controllerHandle, actionSetHandle, analogActionHandle, originsOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetConnectedControllers(IntPtr handles, IntPtr handlesOut)
        {
            Write("SteamAPI_ISteamController_GetConnectedControllers");
            return SteamClient.steam_Controller.GetConnectedControllers(handles, handlesOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamController_GetControllerBindingRevision(IntPtr controllerHandle, int pMajor, int pMinor)
        {
            Write("SteamAPI_ISteamController_GetControllerBindingRevision");
            return SteamClient.steam_Controller.GetControllerBindingRevision(controllerHandle, pMajor, pMinor);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetControllerForGamepadIndex(int nIndex)
        {
            Write("SteamAPI_ISteamController_GetControllerForGamepadIndex");
            return SteamClient.steam_Controller.GetControllerForGamepadIndex(nIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetCurrentActionSet(IntPtr controllerHandle)
        {
            Write("SteamAPI_ISteamController_GetCurrentActionSet");
            return SteamClient.steam_Controller.GetCurrentActionSet(controllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ControllerDigitalActionData_t SteamAPI_ISteamController_GetDigitalActionData(IntPtr controllerHandle, int digitalActionHandle)
        {
            Write("SteamAPI_ISteamController_GetDigitalActionData");
            return SteamClient.steam_Controller.GetDigitalActionData(controllerHandle, digitalActionHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetDigitalActionHandle(string pszActionName)
        {
            Write("SteamAPI_ISteamController_GetDigitalActionHandle");
            return SteamClient.steam_Controller.GetDigitalActionHandle(pszActionName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetDigitalActionOrigins(IntPtr controllerHandle, int actionSetHandle, int digitalActionHandle, EControllerActionOrigin originsOut)
        {
            Write("SteamAPI_ISteamController_GetDigitalActionOrigins");
            return SteamClient.steam_Controller.GetDigitalActionOrigins(controllerHandle, actionSetHandle, digitalActionHandle, originsOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetGamepadIndexForController(IntPtr ulControllerHandle)
        {
            Write("SteamAPI_ISteamController_GetGamepadIndexForController");
            return SteamClient.steam_Controller.GetGamepadIndexForController(ulControllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamController_GetGlyphForActionOrigin(EControllerActionOrigin eOrigin)
        {
            Write("SteamAPI_ISteamController_GetGlyphForActionOrigin");
            return SteamClient.steam_Controller.GetGlyphForActionOrigin(eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamController_GetGlyphForXboxOrigin(EXboxOrigin eOrigin)
        {
            Write("SteamAPI_ISteamController_GetGlyphForXboxOrigin");
            return SteamClient.steam_Controller.GetGlyphForXboxOrigin(eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ESteamInputType SteamAPI_ISteamController_GetInputTypeForHandle(IntPtr controllerHandle)
        {
            Write("SteamAPI_ISteamController_GetInputTypeForHandle");
            return SteamClient.steam_Controller.GetInputTypeForHandle(controllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ControllerMotionData_t SteamAPI_ISteamController_GetMotionData(IntPtr controllerHandle)
        {
            Write("SteamAPI_ISteamController_GetMotionData");
            return SteamClient.steam_Controller.GetMotionData(controllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamController_GetStringForActionOrigin(EControllerActionOrigin eOrigin)
        {
            Write("SteamAPI_ISteamController_GetStringForActionOrigin");
            return SteamClient.steam_Controller.GetStringForActionOrigin(eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamController_GetStringForXboxOrigin(EXboxOrigin eOrigin)
        {
            Write("SteamAPI_ISteamController_GetStringForXboxOrigin");
            return SteamClient.steam_Controller.GetStringForXboxOrigin(eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamController_Init()
        {
            Write("SteamAPI_ISteamController_Init");
            return SteamClient.steam_Controller.Init();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_RunFrame()
        {
            Write("SteamAPI_ISteamController_RunFrame");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_SetLEDColor(IntPtr controllerHandle, uint nColorR, uint nColorG, uint nColorB, int nFlags)
        {
            Write("SteamAPI_ISteamController_SetLEDColor");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamController_ShowBindingPanel(IntPtr controllerHandle)
        {
            Write("SteamAPI_ISteamController_ShowBindingPanel");
            return SteamClient.steam_Controller.ShowBindingPanel(controllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamController_Shutdown()
        {
            Write("SteamAPI_ISteamController_Shutdown");
            return SteamClient.steam_Controller.Shutdown();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_StopAnalogActionMomentum(IntPtr controllerHandle, uint eAction)
        {
            Write("SteamAPI_ISteamController_StopAnalogActionMomentum");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EControllerActionOrigin SteamAPI_ISteamController_TranslateActionOrigin(ESteamInputType eDestinationInputType, EControllerActionOrigin eSourceOrigin)
        {
            Write("SteamAPI_ISteamController_TranslateActionOrigin");
            return SteamClient.steam_Controller.TranslateActionOrigin(eDestinationInputType, eSourceOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_TriggerHapticPulse(IntPtr controllerHandle, ESteamControllerPad eTargetPad, uint usDurationMicroSec)
        {
            Write("SteamAPI_ISteamController_TriggerHapticPulse");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_TriggerRepeatedHapticPulse(IntPtr controllerHandle, ESteamControllerPad eTargetPad, uint usDurationMicroSec, uint usOffMicroSec, uint unRepeat, int nFlags)
        {
            Write("SteamAPI_ISteamController_TriggerRepeatedHapticPulse");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_TriggerVibration(IntPtr controllerHandle, uint usLeftSpeed, uint usRightSpeed)
        {
            Write("SteamAPI_ISteamController_TriggerVibration");
        }
    }
}