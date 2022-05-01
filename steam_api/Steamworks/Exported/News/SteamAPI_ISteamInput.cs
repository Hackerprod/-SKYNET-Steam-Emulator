//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//using SKYNET;
//using SKYNET.Managers;
//using SKYNET.Steamworks;

//using InputHandle_t = System.UInt64;
//using InputActionSetHandle_t = System.UInt64;
//using InputDigitalActionHandle_t = System.UInt64;
//using InputAnalogActionHandle_t = System.UInt64;

//namespace SKYNET.Steamworks.Exported
//{
//    public class SteamAPI_ISteamInput
//    { 
//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static void SteamAPI_ISteamInput_ActivateActionSet(InputHandle_t inputHandle, IntPtr actionSetHandle)
//        {
//            Write($"SteamAPI_ISteamInput_ActivateActionSet");
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static void SteamAPI_ISteamInput_ActivateActionSetLayer(InputHandle_t inputHandle, IntPtr actionSetLayerHandle)
//        {
//            Write($"SteamAPI_ISteamInput_ActivateActionSetLayer");
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInput_BNewDataAvailable()
//        {
//            Write($"SteamAPI_ISteamInput_BNewDataAvailable");
//            return SteamEmulator.SteamInput.BNewDataAvailable();
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInput_BWaitForData([MarshalAs(UnmanagedType.U1)] bool bWaitForever, uint unTimeout)
//        {
//            Write($"SteamAPI_ISteamInput_BWaitForData");
//            return SteamEmulator.SteamInput.BWaitForData(bWaitForever, unTimeout);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static void SteamAPI_ISteamInput_DeactivateActionSetLayer(InputHandle_t inputHandle, IntPtr actionSetLayerHandle)
//        {
//            Write($"SteamAPI_ISteamInput_DeactivateActionSetLayer");
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static void SteamAPI_ISteamInput_DeactivateAllActionSetLayers(InputHandle_t inputHandle)
//        {
//            Write($"SteamAPI_ISteamInput_DeactivateAllActionSetLayers");
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static void SteamAPI_ISteamInput_EnableDeviceCallbacks()
//        {
//            Write($"SteamAPI_ISteamInput_EnableDeviceCallbacks");
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static int SteamAPI_ISteamInput_GetActionOriginFromint(InputHandle_t inputHandle, int eOrigin)
//        {
//            Write($"SteamAPI_ISteamInput_GetActionOriginFromint");
//            return SteamEmulator.SteamInput.GetActionOriginFromint(inputHandle, eOrigin);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static InputActionSetHandle_t SteamAPI_ISteamInput_GetActionSetHandle(string pszActionSetName)
//        {
//            Write($"SteamAPI_ISteamInput_GetActionSetHandle");
//            return SteamEmulator.SteamInput.GetActionSetHandle(pszActionSetName);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static int SteamAPI_ISteamInput_GetActiveActionSetLayers(InputHandle_t inputHandle, ref InputActionSetHandle_t[] handlesOut)
//        {
//            Write($"SteamAPI_ISteamInput_GetActiveActionSetLayers");
//            return SteamEmulator.SteamInput.GetActiveActionSetLayers(inputHandle, ref handlesOut);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static IntPtr SteamAPI_ISteamInput_GetAnalogActionData(InputHandle_t inputHandle, InputAnalogActionHandle_t analogActionHandle)
//        {
//            Write($"SteamAPI_ISteamInput_GetAnalogActionData");
//            return SteamEmulator.SteamInput.GetAnalogActionData(inputHandle, analogActionHandle);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static IntPtr SteamAPI_ISteamInput_GetAnalogActionHandle(string pszActionName)
//        {
//            Write($"SteamAPI_ISteamInput_GetAnalogActionHandle");
//            return SteamEmulator.SteamInput.GetAnalogActionHandle(pszActionName);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static int SteamAPI_ISteamInput_GetAnalogActionOrigins(InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputAnalogActionHandle_t analogActionHandle, ref int[] originsOut)
//        {
//            Write($"SteamAPI_ISteamInput_GetAnalogActionOrigins");
//            return SteamEmulator.SteamInput.GetAnalogActionOrigins(inputHandle, actionSetHandle, analogActionHandle, ref originsOut);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static int SteamAPI_ISteamInput_GetConnectedControllers(ref InputHandle_t[] handlesOut)
//        {
//            Write($"SteamAPI_ISteamInput_GetConnectedControllers");
//            return SteamEmulator.SteamInput.GetConnectedControllers(ref handlesOut);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static InputHandle_t SteamAPI_ISteamInput_GetControllerForGamepadIndex(int nIndex)
//        {
//            Write($"SteamAPI_ISteamInput_GetControllerForGamepadIndex");
//            return SteamEmulator.SteamInput.GetControllerForGamepadIndex(nIndex);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static InputActionSetHandle_t SteamAPI_ISteamInput_GetCurrentActionSet(InputHandle_t inputHandle)
//        {
//            Write($"SteamAPI_ISteamInput_GetCurrentActionSet");
//            return SteamEmulator.SteamInput.GetCurrentActionSet(inputHandle);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInput_GetDeviceBindingRevision(InputHandle_t inputHandle, int pMajor, int pMinor)
//        {
//            Write($"SteamAPI_ISteamInput_GetDeviceBindingRevision");
//            return SteamEmulator.SteamInput.GetDeviceBindingRevision(inputHandle, pMajor, pMinor);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static IntPtr SteamAPI_ISteamInput_GetDigitalActionData(InputHandle_t inputHandle, InputDigitalActionHandle_t digitalActionHandle)
//        {
//            Write($"SteamAPI_ISteamInput_GetDigitalActionData");
//            return SteamEmulator.SteamInput.GetDigitalActionData(inputHandle, digitalActionHandle);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static InputDigitalActionHandle_t SteamAPI_ISteamInput_GetDigitalActionHandle(string pszActionName)
//        {
//            Write($"SteamAPI_ISteamInput_GetDigitalActionHandle");
//            return SteamEmulator.SteamInput.GetDigitalActionHandle(pszActionName);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static int SteamAPI_ISteamInput_GetDigitalActionOrigins(InputHandle_t inputHandle, InputActionSetHandle_t actionSetHandle, InputDigitalActionHandle_t digitalActionHandle, ref int[] originsOut)
//        {
//            Write($"SteamAPI_ISteamInput_GetDigitalActionOrigins");
//            return SteamEmulator.SteamInput.GetDigitalActionOrigins(inputHandle, actionSetHandle, digitalActionHandle, ref originsOut);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static int SteamAPI_ISteamInput_GetGamepadIndexForController(InputHandle_t ulinputHandle)
//        {
//            Write($"SteamAPI_ISteamInput_GetGamepadIndexForController");
//            return SteamEmulator.SteamInput.GetGamepadIndexForController(ulinputHandle);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static IntPtr SteamAPI_ISteamInput_GetGlyphForActionOrigin_Legacy(int eOrigin)
//        {
//            Write($"SteamAPI_ISteamInput_GetGlyphForActionOrigin_Legacy");
//            return SteamEmulator.SteamInput.GetGlyphForActionOrigin_Legacy(eOrigin);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static IntPtr SteamAPI_ISteamInput_GetGlyphForint(int eOrigin)
//        {
//            Write($"SteamAPI_ISteamInput_GetGlyphForint");
//            return SteamEmulator.SteamInput.GetGlyphForint(eOrigin);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static string SteamAPI_ISteamInput_GetGlyphPNGForActionOrigin(int eOrigin, int eSize, uint unFlags)
//        {
//            Write($"SteamAPI_ISteamInput_GetGlyphPNGForActionOrigin");
//            return SteamEmulator.SteamInput.GetGlyphPNGForActionOrigin(eOrigin, eSize, unFlags);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static string SteamAPI_ISteamInput_GetGlyphSVGForActionOrigin(int eOrigin, uint unFlags)
//        {
//            Write($"SteamAPI_ISteamInput_GetGlyphSVGForActionOrigin");
//            return SteamEmulator.SteamInput.GetGlyphSVGForActionOrigin(eOrigin, unFlags);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static int SteamAPI_ISteamInput_GetInputTypeForHandle(InputHandle_t inputHandle)
//        {
//            Write($"SteamAPI_ISteamInput_GetInputTypeForHandle");
//            return SteamEmulator.SteamInput.GetInputTypeForHandle(inputHandle);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static InputMotionData_t SteamAPI_ISteamInput_GetMotionData(InputHandle_t inputHandle)
//        {
//            Write($"SteamAPI_ISteamInput_GetMotionData");
//            return SteamEmulator.SteamInput.GetMotionData(inputHandle);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static uint SteamAPI_ISteamInput_GetRemotePlaySessionID(InputHandle_t inputHandle)
//        {
//            Write($"SteamAPI_ISteamInput_GetRemotePlaySessionID");
//            return SteamEmulator.SteamInput.GetRemotePlaySessionID(inputHandle);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static ushort SteamAPI_ISteamInput_GetSessionInputConfigurationSettings()
//        {
//            Write($"SteamAPI_ISteamInput_GetSessionInputConfigurationSettings");
//            return SteamEmulator.SteamInput.GetSessionInputConfigurationSettings();
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static string SteamAPI_ISteamInput_GetStringForActionOrigin(int eOrigin)
//        {
//            Write($"SteamAPI_ISteamInput_GetStringForActionOrigin");
//            return SteamEmulator.SteamInput.GetStringForActionOrigin(eOrigin);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static string SteamAPI_ISteamInput_GetStringForAnalogActionName(InputAnalogActionHandle_t eActionHandle)
//        {
//            Write($"SteamAPI_ISteamInput_GetStringForAnalogActionName");
//            return SteamEmulator.SteamInput.GetStringForAnalogActionName(eActionHandle);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static string SteamAPI_ISteamInput_GetStringForDigitalActionName(InputAnalogActionHandle_t eActionHandle)
//        {
//            Write($"SteamAPI_ISteamInput_GetStringForDigitalActionName");
//            return SteamEmulator.SteamInput.GetStringForDigitalActionName(eActionHandle);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static IntPtr SteamAPI_ISteamInput_GetStringForint(int eOrigin)
//        {
//            Write($"SteamAPI_ISteamInput_GetStringForint");
//            return SteamEmulator.SteamInput.GetStringForint(eOrigin);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInput_Init([MarshalAs(UnmanagedType.U1)] bool bExplicitlyCallRunFrame)
//        {
//            Write($"SteamAPI_ISteamInput_Init");
//            return SteamEmulator.SteamInput.Init(bExplicitlyCallRunFrame);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static void SteamAPI_ISteamInput_Legacy_TriggerHapticPulse(InputHandle_t inputHandle, IntPtr eTargetPad, ushort usDurationMicroSec)
//        {
//            Write($"SteamAPI_ISteamInput_Legacy_TriggerHapticPulse");
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static void SteamAPI_ISteamInput_Legacy_TriggerRepeatedHapticPulse(InputHandle_t inputHandle, IntPtr eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags)
//        {
//            Write($"SteamAPI_ISteamInput_Legacy_TriggerRepeatedHapticPulse");
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static void SteamAPI_ISteamInput_RunFrame([MarshalAs(UnmanagedType.U1)] bool bReservedValue)
//        {
//            Write($"SteamAPI_ISteamInput_RunFrame");
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInput_SetInputActionManifestFilePath(string pchInputActionManifestAbsolutePath)
//        {
//            Write($"SteamAPI_ISteamInput_SetInputActionManifestFilePath");
//            return SteamEmulator.SteamInput.SetInputActionManifestFilePath(pchInputActionManifestAbsolutePath);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static void SteamAPI_ISteamInput_SetLEDColor(InputHandle_t inputHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags)
//        {
//            Write($"SteamAPI_ISteamInput_SetLEDColor");
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInput_ShowBindingPanel(InputHandle_t inputHandle)
//        {
//            Write($"SteamAPI_ISteamInput_ShowBindingPanel");
//            return SteamEmulator.SteamInput.ShowBindingPanel(inputHandle);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static bool SteamAPI_ISteamInput_Shutdown()
//        {
//            Write($"SteamAPI_ISteamInput_Shutdown");
//            return SteamEmulator.SteamInput.Shutdown();
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static void SteamAPI_ISteamInput_StopAnalogActionMomentum(InputHandle_t inputHandle, IntPtr eAction)
//        {
//            Write($"SteamAPI_ISteamInput_StopAnalogActionMomentum");
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static int SteamAPI_ISteamInput_TranslateActionOrigin(int eDestinationInputType, int eSourceOrigin)
//        {
//            Write($"SteamAPI_ISteamInput_TranslateActionOrigin");
//            return SteamEmulator.SteamInput.TranslateActionOrigin(eDestinationInputType, eSourceOrigin);
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static void SteamAPI_ISteamInput_TriggerSimpleHapticEvent(InputHandle_t inputHandle, IntPtr eHapticLocation, byte nIntensity, char nGainDB, byte nOtherIntensity, char nOtherGainDB)
//        {
//            Write($"SteamAPI_ISteamInput_TriggerSimpleHapticEvent");
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static void SteamAPI_ISteamInput_TriggerVibration(InputHandle_t inputHandle, ushort usLeftSpeed, ushort usRightSpeed)
//        {
//            Write($"SteamAPI_ISteamInput_TriggerVibration");
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static void SteamAPI_ISteamInput_TriggerVibrationExtended(InputHandle_t inputHandle, ushort usLeftSpeed, ushort usRightSpeed, ushort usLeftTriggerSpeed, ushort usRightTriggerSpeed)
//        {
//            Write($"SteamAPI_ISteamInput_TriggerVibrationExtended");
//        }

//        [DllExport(CallingConvention = CallingConvention.Cdecl)]
//        public static IntPtr SteamAPI_SteamInput_v005()
//        {
//            Write($"SteamAPI_SteamInput_v005");
//            return InterfaceManager.FindOrCreateInterface("SteamInput005");
//        }

//        private static void Write(string msg)
//        {
//            SteamEmulator.Write("SteamAPI_ISteamInput", msg);
//        }
//    }
//}

