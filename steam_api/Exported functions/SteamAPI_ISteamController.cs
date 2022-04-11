using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Steamworks;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamController : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_ActivateActionSet(ulong controllerHandle, int actionSetHandle)
        {
            Write("SteamAPI_ISteamController_ActivateActionSet");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_ActivateActionSetLayer(ulong controllerHandle, IntPtr actionSetLayerHandle)
        {
            Write("SteamAPI_ISteamController_ActivateActionSetLayer");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_DeactivateActionSetLayer(ulong controllerHandle, IntPtr actionSetLayerHandle)
        {
            Write("SteamAPI_ISteamController_DeactivateActionSetLayer");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_DeactivateAllActionSetLayers(ulong controllerHandle)
        {
            Write("SteamAPI_ISteamController_DeactivateAllActionSetLayers");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetActionOriginFromXboxOrigin_(ulong controllerHandle, int eOrigin)
        {
            Write("SteamAPI_ISteamController_GetActionOriginFromXboxOrigin_");
            return SteamEmulator.SteamController.GetActionOriginFromXboxOrigin_(controllerHandle, eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamController_GetActionSetHandle(string pszActionSetName)
        {
            Write("SteamAPI_ISteamController_GetActionSetHandle");
            return SteamEmulator.SteamController.GetActionSetHandle(pszActionSetName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetActiveActionSetLayers(ulong controllerHandle, ulong handlesOut)
        {
            Write("SteamAPI_ISteamController_GetActiveActionSetLayers");
            return SteamEmulator.SteamController.GetActiveActionSetLayers(controllerHandle, handlesOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamController_GetAnalogActionData(uint controllerHandle, uint analogActionHandle)
        {
            Write("SteamAPI_ISteamController_GetAnalogActionData");
            return SteamEmulator.SteamController.GetAnalogActionData(controllerHandle, analogActionHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong GetAnalogActionHandle(string pszActionName)
        {
            Write("SteamAPI_ISteamController_GetAnalogActionHandle");
            return SteamEmulator.SteamController.GetActionSetHandle(pszActionName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetAnalogActionOrigins(ulong controllerHandle, ulong actionSetHandle, uint analogActionHandle, int originsOut)
        {
            Write("SteamAPI_ISteamController_GetAnalogActionOrigins");
            return SteamEmulator.SteamController.GetAnalogActionOrigins(controllerHandle, actionSetHandle, analogActionHandle, originsOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetConnectedControllers(ulong handlesOut)
        {
            Write("SteamAPI_ISteamController_GetConnectedControllers");
            return SteamEmulator.SteamController.GetConnectedControllers(handlesOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamController_GetControllerBindingRevision(ulong controllerHandle, int pMajor, int pMinor)
        {
            Write("SteamAPI_ISteamController_GetControllerBindingRevision");
            return SteamEmulator.SteamController.GetControllerBindingRevision(controllerHandle, pMajor, pMinor);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamController_GetControllerForGamepadIndex(int nIndex)
        {
            Write("SteamAPI_ISteamController_GetControllerForGamepadIndex");
            return SteamEmulator.SteamController.GetControllerForGamepadIndex(nIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamController_GetCurrentActionSet(ulong controllerHandle)
        {
            Write("SteamAPI_ISteamController_GetCurrentActionSet");
            return SteamEmulator.SteamController.GetCurrentActionSet(controllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamController_GetDigitalActionData(ulong controllerHandle, ulong digitalActionHandle)
        {
            Write("SteamAPI_ISteamController_GetDigitalActionData");
            return SteamEmulator.SteamController.GetDigitalActionData(controllerHandle, digitalActionHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ulong SteamAPI_ISteamController_GetDigitalActionHandle(string pszActionName)
        {
            Write("SteamAPI_ISteamController_GetDigitalActionHandle");
            return SteamEmulator.SteamController.GetDigitalActionHandle(pszActionName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetDigitalActionOrigins(ulong controllerHandle, ulong actionSetHandle, ulong digitalActionHandle, int originsOut)
        {
            Write("SteamAPI_ISteamController_GetDigitalActionOrigins");
            return SteamEmulator.SteamController.GetDigitalActionOrigins(controllerHandle, actionSetHandle, digitalActionHandle, originsOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetGamepadIndexForController(ulong ulControllerHandle)
        {
            Write("SteamAPI_ISteamController_GetGamepadIndexForController");
            return SteamEmulator.SteamController.GetGamepadIndexForController(ulControllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamController_GetGlyphForActionOrigin(int eOrigin)
        {
            Write("SteamAPI_ISteamController_GetGlyphForActionOrigin");
            return SteamEmulator.SteamController.GetGlyphForActionOrigin(eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamController_GetGlyphForXboxOrigin(int eOrigin)
        {
            Write("SteamAPI_ISteamController_GetGlyphForXboxOrigin");
            return SteamEmulator.SteamController.GetGlyphForXboxOrigin(eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_GetInputTypeForHandle(ulong controllerHandle)
        {
            Write("SteamAPI_ISteamController_GetInputTypeForHandle");
            return SteamEmulator.SteamController.GetInputTypeForHandle(controllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamController_GetMotionData(ulong controllerHandle)
        {
            Write("SteamAPI_ISteamController_GetMotionData");
            return SteamEmulator.SteamController.GetMotionData(controllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamController_GetStringForActionOrigin(int eOrigin)
        {
            Write("SteamAPI_ISteamController_GetStringForActionOrigin");
            return SteamEmulator.SteamController.GetStringForActionOrigin(eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamController_GetStringForXboxOrigin(int eOrigin)
        {
            Write("SteamAPI_ISteamController_GetStringForXboxOrigin");
            return SteamEmulator.SteamController.GetStringForXboxOrigin(eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamController_Init()
        {
            Write("SteamAPI_ISteamController_Init");
            return SteamEmulator.SteamController.Init(SteamEmulator.SteamController.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_RunFrame()
        {
            Write("SteamAPI_ISteamController_RunFrame");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_SetLEDColor(ulong controllerHandle, uint nColorR, uint nColorG, uint nColorB, int nFlags)
        {
            Write("SteamAPI_ISteamController_SetLEDColor");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamController_ShowBindingPanel(ulong controllerHandle)
        {
            Write("SteamAPI_ISteamController_ShowBindingPanel");
            return SteamEmulator.SteamController.ShowBindingPanel(controllerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamController_Shutdown()
        {
            Write("SteamAPI_ISteamController_Shutdown");
            return SteamEmulator.SteamController.Shutdown(SteamEmulator.SteamController.MemoryAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_StopAnalogActionMomentum(ulong controllerHandle, uint eAction)
        {
            Write("SteamAPI_ISteamController_StopAnalogActionMomentum");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamController_TranslateActionOrigin(int eDestinationInputType, int eSourceOrigin)
        {
            Write("SteamAPI_ISteamController_TranslateActionOrigin");
            return SteamEmulator.SteamController.TranslateActionOrigin(eDestinationInputType, eSourceOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_TriggerHapticPulse(ulong controllerHandle, ESteamControllerPad eTargetPad, uint usDurationMicroSec)
        {
            Write("SteamAPI_ISteamController_TriggerHapticPulse");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_TriggerRepeatedHapticPulse(ulong controllerHandle, ESteamControllerPad eTargetPad, uint usDurationMicroSec, uint usOffMicroSec, uint unRepeat, int nFlags)
        {
            Write("SteamAPI_ISteamController_TriggerRepeatedHapticPulse");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamController_TriggerVibration(ulong controllerHandle, uint usLeftSpeed, uint usRightSpeed)
        {
            Write("SteamAPI_ISteamController_TriggerVibration");
        }
    }
}
