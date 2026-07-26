using System;
using System.Runtime.InteropServices;
using SKYNET.Managers;

using InputHandle_t = System.UInt64;
using InputActionSetHandle_t = System.UInt64;
using InputDigitalActionHandle_t = System.UInt64;
using InputAnalogActionHandle_t = System.UInt64;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamInput
    {
        static SteamAPI_ISteamInput()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInput_ActivateActionSet(IntPtr _, InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle)
        {
            Write($"SteamAPI_ISteamInput_ActivateActionSet");
            SteamEmulator.SteamInput.ActivateActionSet(inputHandle, actionSetHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInput_ActivateActionSetLayer(IntPtr _, InputHandle_t inputHandle, InputActionSetHandle_t actionSetLayerHandle)
        {
            Write($"SteamAPI_ISteamInput_ActivateActionSetLayer");
            SteamEmulator.SteamInput.ActivateActionSetLayer(inputHandle, actionSetLayerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInput_BNewDataAvailable(IntPtr _)
        {
            Write($"SteamAPI_ISteamInput_BNewDataAvailable");
            return SteamEmulator.SteamInput.BNewDataAvailable();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInput_BWaitForData(IntPtr _, bool bWaitForever, uint unTimeout)
        {
            Write($"SteamAPI_ISteamInput_BWaitForData");
            return SteamEmulator.SteamInput.BWaitForData(bWaitForever, unTimeout);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInput_DeactivateActionSetLayer(IntPtr _, InputHandle_t inputHandle, InputActionSetHandle_t actionSetLayerHandle)
        {
            Write($"SteamAPI_ISteamInput_DeactivateActionSetLayer");
            SteamEmulator.SteamInput.DeactivateActionSetLayer(inputHandle, actionSetLayerHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInput_DeactivateAllActionSetLayers(IntPtr _, InputHandle_t inputHandle)
        {
            Write($"SteamAPI_ISteamInput_DeactivateAllActionSetLayers");
            SteamEmulator.SteamInput.DeactivateAllActionSetLayers(inputHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInput_EnableDeviceCallbacks(IntPtr _)
        {
            Write($"SteamAPI_ISteamInput_EnableDeviceCallbacks");
            SteamEmulator.SteamInput.EnableDeviceCallbacks();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInput_EnableActionEventCallbacks(IntPtr _, IntPtr pCallback)
        {
            Write("SteamAPI_ISteamInput_EnableActionEventCallbacks");
            SteamEmulator.SteamInput.EnableActionEventCallbacks(pCallback);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamInput_GetActionOriginFromXboxOrigin(
            IntPtr _,
            InputHandle_t inputHandle,
            int eOrigin)
        {
            Write("SteamAPI_ISteamInput_GetActionOriginFromXboxOrigin");
            return SteamEmulator.SteamInput.GetActionOriginFromXboxOrigin(inputHandle, eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamInput_GetGlyphForXboxOrigin(IntPtr _, int eOrigin)
        {
            Write("SteamAPI_ISteamInput_GetGlyphForXboxOrigin");
            return SKYNET.Helpers.NativeStringCache.ToUtf8Ptr(
                SteamEmulator.SteamInput.GetGlyphForXboxOrigin(eOrigin));
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamInput_GetStringForXboxOrigin(IntPtr _, int eOrigin)
        {
            Write("SteamAPI_ISteamInput_GetStringForXboxOrigin");
            return SKYNET.Helpers.NativeStringCache.ToUtf8Ptr(
                SteamEmulator.SteamInput.GetStringForXboxOrigin(eOrigin));
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamInput_GetActionOriginFromint(IntPtr _, InputHandle_t inputHandle, int eOrigin)
        {
            Write($"SteamAPI_ISteamInput_GetActionOriginFromint");
            return SteamEmulator.SteamInput.GetActionOriginFromint(inputHandle, eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static InputActionSetHandle_t SteamAPI_ISteamInput_GetActionSetHandle(IntPtr _, string pszActionSetName)
        {
            Write($"SteamAPI_ISteamInput_GetActionSetHandle");
            return SteamEmulator.SteamInput.GetActionSetHandle(pszActionSetName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamInput_GetActiveActionSetLayers(IntPtr _, InputHandle_t inputHandle, IntPtr handlesOut)
        {
            Write($"SteamAPI_ISteamInput_GetActiveActionSetLayers");
            return SteamEmulator.SteamInput.GetActiveActionSetLayers(inputHandle, handlesOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static InputAnalogActionData_t SteamAPI_ISteamInput_GetAnalogActionData(IntPtr _, InputHandle_t inputHandle, InputAnalogActionHandle_t analogActionHandle)
        {
            Write($"SteamAPI_ISteamInput_GetAnalogActionData");
            return SteamEmulator.SteamInput.GetAnalogActionData(inputHandle, analogActionHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static InputAnalogActionHandle_t SteamAPI_ISteamInput_GetAnalogActionHandle(IntPtr _, string pszActionName)
        {
            Write($"SteamAPI_ISteamInput_GetAnalogActionHandle");
            return SteamEmulator.SteamInput.GetAnalogActionHandle(pszActionName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamInput_GetAnalogActionOrigins(IntPtr _, InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputAnalogActionHandle_t analogActionHandle, IntPtr originsOut)
        {
            Write($"SteamAPI_ISteamInput_GetAnalogActionOrigins");
            return SteamEmulator.SteamInput.GetAnalogActionOrigins(inputHandle, actionSetHandle, analogActionHandle, originsOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamInput_GetConnectedControllers(IntPtr _, IntPtr handlesOut)
        {
            Write($"SteamAPI_ISteamInput_GetConnectedControllers");
            return SteamEmulator.SteamInput.GetConnectedControllers(handlesOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static InputHandle_t SteamAPI_ISteamInput_GetControllerForGamepadIndex(IntPtr _, int nIndex)
        {
            Write($"SteamAPI_ISteamInput_GetControllerForGamepadIndex");
            return SteamEmulator.SteamInput.GetControllerForGamepadIndex(nIndex);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static InputActionSetHandle_t SteamAPI_ISteamInput_GetCurrentActionSet(IntPtr _, InputHandle_t inputHandle)
        {
            Write($"SteamAPI_ISteamInput_GetCurrentActionSet");
            return SteamEmulator.SteamInput.GetCurrentActionSet(inputHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInput_GetDeviceBindingRevision(IntPtr _, InputHandle_t inputHandle, IntPtr pMajor, IntPtr pMinor)
        {
            Write($"SteamAPI_ISteamInput_GetDeviceBindingRevision");
            return SteamEmulator.SteamInput.GetDeviceBindingRevision(inputHandle, pMajor, pMinor);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static InputDigitalActionData_t SteamAPI_ISteamInput_GetDigitalActionData(IntPtr _, InputHandle_t inputHandle, InputDigitalActionHandle_t digitalActionHandle)
        {
            Write($"SteamAPI_ISteamInput_GetDigitalActionData");
            return SteamEmulator.SteamInput.GetDigitalActionData(inputHandle, digitalActionHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static InputDigitalActionHandle_t SteamAPI_ISteamInput_GetDigitalActionHandle(IntPtr _, string pszActionName)
        {
            Write($"SteamAPI_ISteamInput_GetDigitalActionHandle");
            return SteamEmulator.SteamInput.GetDigitalActionHandle(pszActionName);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamInput_GetDigitalActionOrigins(IntPtr _, InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputDigitalActionHandle_t digitalActionHandle, IntPtr originsOut)
        {
            Write($"SteamAPI_ISteamInput_GetDigitalActionOrigins");
            return SteamEmulator.SteamInput.GetDigitalActionOrigins(inputHandle, actionSetHandle, digitalActionHandle, originsOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamInput_GetGamepadIndexForController(IntPtr _, InputHandle_t ulinputHandle)
        {
            Write($"SteamAPI_ISteamInput_GetGamepadIndexForController");
            return SteamEmulator.SteamInput.GetGamepadIndexForController(ulinputHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamInput_GetGlyphForActionOrigin_Legacy(IntPtr _, int eOrigin)
        {
            Write($"SteamAPI_ISteamInput_GetGlyphForActionOrigin_Legacy");
            return SteamEmulator.SteamInput.GetGlyphForActionOrigin_Legacy(eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamInput_GetGlyphForint(IntPtr _, int eOrigin)
        {
            Write($"SteamAPI_ISteamInput_GetGlyphForint");
            return SteamEmulator.SteamInput.GetGlyphForint(eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamInput_GetGlyphPNGForActionOrigin(IntPtr _, int eOrigin, int eSize, uint unFlags)
        {
            Write($"SteamAPI_ISteamInput_GetGlyphPNGForActionOrigin");
            return SteamEmulator.SteamInput.GetGlyphPNGForActionOrigin(eOrigin, eSize, unFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamInput_GetGlyphSVGForActionOrigin(IntPtr _, int eOrigin, uint unFlags)
        {
            Write($"SteamAPI_ISteamInput_GetGlyphSVGForActionOrigin");
            return SteamEmulator.SteamInput.GetGlyphSVGForActionOrigin(eOrigin, unFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamInput_GetInputTypeForHandle(IntPtr _, InputHandle_t inputHandle)
        {
            Write($"SteamAPI_ISteamInput_GetInputTypeForHandle");
            return SteamEmulator.SteamInput.GetInputTypeForHandle(inputHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static InputMotionData_t SteamAPI_ISteamInput_GetMotionData(IntPtr _, InputHandle_t inputHandle)
        {
            Write($"SteamAPI_ISteamInput_GetMotionData");
            return SteamEmulator.SteamInput.GetMotionData(inputHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamInput_GetRemotePlaySessionID(IntPtr _, InputHandle_t inputHandle)
        {
            Write($"SteamAPI_ISteamInput_GetRemotePlaySessionID");
            return SteamEmulator.SteamInput.GetRemotePlaySessionID(inputHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static ushort SteamAPI_ISteamInput_GetSessionInputConfigurationSettings(IntPtr _)
        {
            Write($"SteamAPI_ISteamInput_GetSessionInputConfigurationSettings");
            return SteamEmulator.SteamInput.GetSessionInputConfigurationSettings();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamInput_GetStringForActionOrigin(IntPtr _, int eOrigin)
        {
            Write($"SteamAPI_ISteamInput_GetStringForActionOrigin");
            return SteamEmulator.SteamInput.GetStringForActionOrigin(eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamInput_GetStringForAnalogActionName(IntPtr _, InputAnalogActionHandle_t eActionHandle)
        {
            Write($"SteamAPI_ISteamInput_GetStringForAnalogActionName");
            return SteamEmulator.SteamInput.GetStringForAnalogActionName(eActionHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_ISteamInput_GetStringForDigitalActionName(IntPtr _, InputAnalogActionHandle_t eActionHandle)
        {
            Write($"SteamAPI_ISteamInput_GetStringForDigitalActionName");
            return SteamEmulator.SteamInput.GetStringForDigitalActionName(eActionHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamInput_GetStringForint(IntPtr _, int eOrigin)
        {
            Write($"SteamAPI_ISteamInput_GetStringForint");
            return SteamEmulator.SteamInput.GetStringForint(eOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInput_Init(IntPtr _, bool bExplicitlyCallRunFrame)
        {
            Write($"SteamAPI_ISteamInput_Init");
            return SteamEmulator.SteamInput.Init(bExplicitlyCallRunFrame);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInput_Legacy_TriggerHapticPulse(IntPtr _, InputHandle_t inputHandle, int eTargetPad, ushort usDurationMicroSec)
        {
            Write($"SteamAPI_ISteamInput_Legacy_TriggerHapticPulse");
            SteamEmulator.SteamInput.Legacy_TriggerHapticPulse(inputHandle, eTargetPad, usDurationMicroSec);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInput_Legacy_TriggerRepeatedHapticPulse(IntPtr _, InputHandle_t inputHandle, int eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags)
        {
            Write($"SteamAPI_ISteamInput_Legacy_TriggerRepeatedHapticPulse");
            SteamEmulator.SteamInput.Legacy_TriggerRepeatedHapticPulse(inputHandle, eTargetPad, usDurationMicroSec, usOffMicroSec, unRepeat, nFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInput_RunFrame(IntPtr _, bool bReservedValue)
        {
            Write($"SteamAPI_ISteamInput_RunFrame");
            SteamEmulator.SteamInput.RunFrame(bReservedValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInput_SetInputActionManifestFilePath(IntPtr _, string pchInputActionManifestAbsolutePath)
        {
            Write($"SteamAPI_ISteamInput_SetInputActionManifestFilePath");
            return SteamEmulator.SteamInput.SetInputActionManifestFilePath(pchInputActionManifestAbsolutePath);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInput_SetLEDColor(IntPtr _, InputHandle_t inputHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags)
        {
            Write($"SteamAPI_ISteamInput_SetLEDColor");
            SteamEmulator.SteamInput.SetLEDColor(inputHandle, nColorR, nColorG, nColorB, nFlags);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInput_SetDualSenseTriggerEffect(
            IntPtr _,
            InputHandle_t inputHandle,
            IntPtr pParam)
        {
            Write("SteamAPI_ISteamInput_SetDualSenseTriggerEffect");
            SteamEmulator.SteamInput.SetDualSenseTriggerEffect(inputHandle, pParam);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInput_ShowBindingPanel(IntPtr _, InputHandle_t inputHandle)
        {
            Write($"SteamAPI_ISteamInput_ShowBindingPanel");
            return SteamEmulator.SteamInput.ShowBindingPanel(inputHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamInput_Shutdown(IntPtr _)
        {
            Write($"SteamAPI_ISteamInput_Shutdown");
            return SteamEmulator.SteamInput.Shutdown();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInput_StopAnalogActionMomentum(IntPtr _, InputHandle_t inputHandle, InputAnalogActionHandle_t eAction)
        {
            Write($"SteamAPI_ISteamInput_StopAnalogActionMomentum");
            SteamEmulator.SteamInput.StopAnalogActionMomentum(inputHandle, eAction);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamInput_TranslateActionOrigin(IntPtr _, int eDestinationInputType, int eSourceOrigin)
        {
            Write($"SteamAPI_ISteamInput_TranslateActionOrigin");
            return SteamEmulator.SteamInput.TranslateActionOrigin(eDestinationInputType, eSourceOrigin);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInput_TriggerSimpleHapticEvent(IntPtr _, InputHandle_t inputHandle, int eHapticLocation, byte nIntensity, sbyte nGainDB, byte nOtherIntensity, sbyte nOtherGainDB)
        {
            Write($"SteamAPI_ISteamInput_TriggerSimpleHapticEvent");
            SteamEmulator.SteamInput.TriggerSimpleHapticEvent(inputHandle, eHapticLocation, nIntensity, nGainDB, nOtherIntensity, nOtherGainDB);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInput_TriggerVibration(IntPtr _, InputHandle_t inputHandle, ushort usLeftSpeed, ushort usRightSpeed)
        {
            Write($"SteamAPI_ISteamInput_TriggerVibration");
            SteamEmulator.SteamInput.TriggerVibration(inputHandle, usLeftSpeed, usRightSpeed);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamInput_TriggerVibrationExtended(IntPtr _, InputHandle_t inputHandle, ushort usLeftSpeed, ushort usRightSpeed, ushort usLeftTriggerSpeed, ushort usRightTriggerSpeed)
        {
            Write($"SteamAPI_ISteamInput_TriggerVibrationExtended");
            SteamEmulator.SteamInput.TriggerVibrationExtended(inputHandle, usLeftSpeed, usRightSpeed, usLeftTriggerSpeed, usRightTriggerSpeed);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_SteamInput_v005(IntPtr _)
        {
            Write($"SteamAPI_SteamInput_v005");
            return InterfaceManager.FindOrCreateInterface("SteamInput005");
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
