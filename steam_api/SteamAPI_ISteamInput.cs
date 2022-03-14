using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SKYNET.Interface;

public class SteamAPI_ISteamInput : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamInput_ActivateActionSet(IntPtr inputHandle, IntPtr actionSetHandle)
    {
        Write($"SteamAPI_ISteamInput_ActivateActionSet");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamInput_ActivateActionSetLayer(IntPtr inputHandle, IntPtr actionSetLayerHandle)
    {
        Write($"SteamAPI_ISteamInput_ActivateActionSetLayer");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInput_BNewDataAvailable()
    {
        Write($"SteamAPI_ISteamInput_BNewDataAvailable");
        return SteamClient.SteamInput.BNewDataAvailable();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInput_BWaitForData([MarshalAs(UnmanagedType.U1)] bool bWaitForever, uint unTimeout)
    {
        Write($"SteamAPI_ISteamInput_BWaitForData");
        return SteamClient.SteamInput.BWaitForData(bWaitForever, unTimeout);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamInput_DeactivateActionSetLayer(IntPtr inputHandle, IntPtr actionSetLayerHandle)
    {
        Write($"SteamAPI_ISteamInput_DeactivateActionSetLayer");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamInput_DeactivateAllActionSetLayers(IntPtr inputHandle)
    {
        Write($"SteamAPI_ISteamInput_DeactivateAllActionSetLayers");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamInput_EnableDeviceCallbacks()
    {
        Write($"SteamAPI_ISteamInput_EnableDeviceCallbacks");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamInput_GetActionOriginFromint(IntPtr inputHandle, int eOrigin)
    {
        Write($"SteamAPI_ISteamInput_GetActionOriginFromint");
        return SteamClient.SteamInput.GetActionOriginFromint(inputHandle, eOrigin);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamInput_GetActionSetHandle(string pszActionSetName)
    {
        Write($"SteamAPI_ISteamInput_GetActionSetHandle");
        return SteamClient.SteamInput.GetActionSetHandle(pszActionSetName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamInput_GetActiveActionSetLayers(IntPtr inputHandle, [In, Out] IntPtr[] handlesOut)
    {
        Write($"SteamAPI_ISteamInput_GetActiveActionSetLayers");
        return SteamClient.SteamInput.GetActiveActionSetLayers(inputHandle, handlesOut);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static InputAnalogActionData_t SteamAPI_ISteamInput_GetAnalogActionData(IntPtr inputHandle, IntPtr analogActionHandle)
    {
        Write($"SteamAPI_ISteamInput_GetAnalogActionData");
        return SteamClient.SteamInput.GetAnalogActionData(inputHandle, analogActionHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamInput_GetAnalogActionHandle(string pszActionName)
    {
        Write($"SteamAPI_ISteamInput_GetAnalogActionHandle");
        return SteamClient.SteamInput.GetAnalogActionHandle(pszActionName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamInput_GetAnalogActionOrigins(IntPtr inputHandle, IntPtr actionSetHandle, IntPtr analogActionHandle, ref int originsOut)
    {
        Write($"SteamAPI_ISteamInput_GetAnalogActionOrigins");
        return SteamClient.SteamInput.GetAnalogActionOrigins(inputHandle, actionSetHandle, analogActionHandle, ref originsOut);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamInput_GetConnectedControllers([In, Out] IntPtr[] handlesOut)
    {
        Write($"SteamAPI_ISteamInput_GetConnectedControllers");
        return SteamClient.SteamInput.GetConnectedControllers(handlesOut);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamInput_GetControllerForGamepadIndex(int nIndex)
    {
        Write($"SteamAPI_ISteamInput_GetControllerForGamepadIndex");
        return SteamClient.SteamInput.GetControllerForGamepadIndex(nIndex);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamInput_GetCurrentActionSet(IntPtr inputHandle)
    {
        Write($"SteamAPI_ISteamInput_GetCurrentActionSet");
        return SteamClient.SteamInput.GetCurrentActionSet(inputHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInput_GetDeviceBindingRevision(IntPtr inputHandle, ref int pMajor, ref int pMinor)
    {
        Write($"SteamAPI_ISteamInput_GetDeviceBindingRevision");
        return SteamClient.SteamInput.GetDeviceBindingRevision(inputHandle, ref pMajor, ref pMinor);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static InputDigitalActionData_t SteamAPI_ISteamInput_GetDigitalActionData(IntPtr inputHandle, IntPtr digitalActionHandle)
    {
        Write($"SteamAPI_ISteamInput_GetDigitalActionData");
        return SteamClient.SteamInput.GetDigitalActionData(inputHandle, digitalActionHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamInput_GetDigitalActionHandle(string pszActionName)
    {
        Write($"SteamAPI_ISteamInput_GetDigitalActionHandle");
        return SteamClient.SteamInput.GetDigitalActionHandle(pszActionName);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamInput_GetDigitalActionOrigins(IntPtr inputHandle, IntPtr actionSetHandle, IntPtr digitalActionHandle, ref int originsOut)
    {
        Write($"SteamAPI_ISteamInput_GetDigitalActionOrigins");
        return SteamClient.SteamInput.GetDigitalActionOrigins(inputHandle, actionSetHandle, digitalActionHandle, ref originsOut);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamInput_GetGamepadIndexForController(IntPtr ulinputHandle)
    {
        Write($"SteamAPI_ISteamInput_GetGamepadIndexForController");
        return SteamClient.SteamInput.GetGamepadIndexForController(ulinputHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamInput_GetGlyphForActionOrigin_Legacy(int eOrigin)
    {
        Write($"SteamAPI_ISteamInput_GetGlyphForActionOrigin_Legacy");
        return SteamClient.SteamInput.GetGlyphForActionOrigin_Legacy(eOrigin);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamInput_GetGlyphForint(int eOrigin)
    {
        Write($"SteamAPI_ISteamInput_GetGlyphForint");
        return SteamClient.SteamInput.GetGlyphForint(eOrigin);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamInput_GetGlyphPNGForActionOrigin(int eOrigin, IntPtr eSize, uint unFlags)
    {
        Write($"SteamAPI_ISteamInput_GetGlyphPNGForActionOrigin");
        return SteamClient.SteamInput.GetGlyphPNGForActionOrigin(eOrigin, eSize, unFlags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamInput_GetGlyphSVGForActionOrigin(int eOrigin, uint unFlags)
    {
        Write($"SteamAPI_ISteamInput_GetGlyphSVGForActionOrigin");
        return SteamClient.SteamInput.GetGlyphSVGForActionOrigin(eOrigin, unFlags);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ESteamInputType SteamAPI_ISteamInput_GetInputTypeForHandle(IntPtr inputHandle)
    {
        Write($"SteamAPI_ISteamInput_GetInputTypeForHandle");
        return SteamClient.SteamInput.GetInputTypeForHandle(inputHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static InputMotionData_t SteamAPI_ISteamInput_GetMotionData(IntPtr inputHandle)
    {
        Write($"SteamAPI_ISteamInput_GetMotionData");
        return SteamClient.SteamInput.GetMotionData(inputHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamInput_GetRemotePlaySessionID(IntPtr inputHandle)
    {
        Write($"SteamAPI_ISteamInput_GetRemotePlaySessionID");
        return SteamClient.SteamInput.GetRemotePlaySessionID(inputHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static ushort SteamAPI_ISteamInput_GetSessionInputConfigurationSettings()
    {
        Write($"SteamAPI_ISteamInput_GetSessionInputConfigurationSettings");
        return SteamClient.SteamInput.GetSessionInputConfigurationSettings();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamInput_GetStringForActionOrigin(int eOrigin)
    {
        Write($"SteamAPI_ISteamInput_GetStringForActionOrigin");
        return SteamClient.SteamInput.GetStringForActionOrigin(eOrigin);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamInput_GetStringForAnalogActionName(IntPtr eActionHandle)
    {
        Write($"SteamAPI_ISteamInput_GetStringForAnalogActionName");
        return SteamClient.SteamInput.GetStringForAnalogActionName(eActionHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamInput_GetStringForDigitalActionName(IntPtr eActionHandle)
    {
        Write($"SteamAPI_ISteamInput_GetStringForDigitalActionName");
        return SteamClient.SteamInput.GetStringForDigitalActionName(eActionHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamInput_GetStringForint(int eOrigin)
    {
        Write($"SteamAPI_ISteamInput_GetStringForint");
        return SteamClient.SteamInput.GetStringForint(eOrigin);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInput_Init([MarshalAs(UnmanagedType.U1)] bool bExplicitlyCallRunFrame)
    {
        Write($"SteamAPI_ISteamInput_Init");
        return SteamClient.SteamInput.Init(bExplicitlyCallRunFrame);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamInput_Legacy_TriggerHapticPulse(IntPtr inputHandle, IntPtr eTargetPad, ushort usDurationMicroSec)
    {
        Write($"SteamAPI_ISteamInput_Legacy_TriggerHapticPulse");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamInput_Legacy_TriggerRepeatedHapticPulse(IntPtr inputHandle, IntPtr eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags)
    {
        Write($"SteamAPI_ISteamInput_Legacy_TriggerRepeatedHapticPulse");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamInput_RunFrame([MarshalAs(UnmanagedType.U1)] bool bReservedValue)
    {
        Write($"SteamAPI_ISteamInput_RunFrame");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInput_SetInputActionManifestFilePath(string pchInputActionManifestAbsolutePath)
    {
        Write($"SteamAPI_ISteamInput_SetInputActionManifestFilePath");
        return SteamClient.SteamInput.SetInputActionManifestFilePath(pchInputActionManifestAbsolutePath);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamInput_SetLEDColor(IntPtr inputHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags)
    {
        Write($"SteamAPI_ISteamInput_SetLEDColor");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInput_ShowBindingPanel(IntPtr inputHandle)
    {
        Write($"SteamAPI_ISteamInput_ShowBindingPanel");
        return SteamClient.SteamInput.ShowBindingPanel(inputHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamInput_Shutdown()
    {
        Write($"SteamAPI_ISteamInput_Shutdown");
        return SteamClient.SteamInput.Shutdown();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamInput_StopAnalogActionMomentum(IntPtr inputHandle, IntPtr eAction)
    {
        Write($"SteamAPI_ISteamInput_StopAnalogActionMomentum");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static int SteamAPI_ISteamInput_TranslateActionOrigin(ESteamInputType eDestinationInputType, int eSourceOrigin)
    {
        Write($"SteamAPI_ISteamInput_TranslateActionOrigin");
        return SteamClient.SteamInput.TranslateActionOrigin(eDestinationInputType, eSourceOrigin);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamInput_TriggerSimpleHapticEvent(IntPtr inputHandle, IntPtr eHapticLocation, byte nIntensity, char nGainDB, byte nOtherIntensity, char nOtherGainDB)
    {
        Write($"SteamAPI_ISteamInput_TriggerSimpleHapticEvent");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamInput_TriggerVibration(IntPtr inputHandle, ushort usLeftSpeed, ushort usRightSpeed)
    {
        Write($"SteamAPI_ISteamInput_TriggerVibration");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static void SteamAPI_ISteamInput_TriggerVibrationExtended(IntPtr inputHandle, ushort usLeftSpeed, ushort usRightSpeed, ushort usLeftTriggerSpeed, ushort usRightTriggerSpeed)
    {
        Write($"SteamAPI_ISteamInput_TriggerVibrationExtended");
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_SteamInput_v005()
    {
        Write($"SteamAPI_SteamInput_v005");
        return IntPtr.Zero;
    }
}

