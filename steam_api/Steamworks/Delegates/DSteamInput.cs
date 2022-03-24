using Core.Interface;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamInput")]
    public class DSteamInput 
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool Init([MarshalAs(UnmanagedType.U1)] bool bExplicitlyCallRunFrame);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool Shutdown(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetInputActionManifestFilePath(string pchInputActionManifestAbsolutePath);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void RunFrame([MarshalAs(UnmanagedType.U1)] bool bReservedValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BWaitForData([MarshalAs(UnmanagedType.U1)] bool bWaitForever, uint unTimeout);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool BNewDataAvailable(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetConnectedControllers([In, Out] IntPtr[] handlesOut);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void EnableDeviceCallbacks(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetActionSetHandle(string pszActionSetName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateActionSet(IntPtr inputHandle, IntPtr actionSetHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetCurrentActionSet(IntPtr inputHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateActionSetLayer(IntPtr inputHandle, IntPtr actionSetLayerHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void DeactivateActionSetLayer(IntPtr inputHandle, IntPtr actionSetLayerHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void DeactivateAllActionSetLayers(IntPtr inputHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetActiveActionSetLayers(IntPtr inputHandle, [In, Out] IntPtr[] handlesOut);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetDigitalActionHandle(string pszActionName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetDigitalActionData(IntPtr inputHandle, IntPtr digitalActionHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetDigitalActionOrigins(IntPtr inputHandle, IntPtr actionSetHandle, IntPtr digitalActionHandle, ref int originsOut);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetStringForDigitalActionName(IntPtr eActionHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetAnalogActionHandle(string pszActionName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetAnalogActionData(IntPtr inputHandle, IntPtr analogActionHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetAnalogActionOrigins(IntPtr inputHandle, IntPtr actionSetHandle, IntPtr analogActionHandle, ref int originsOut);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetGlyphPNGForActionOrigin(int eOrigin, IntPtr eSize, uint unFlags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetGlyphSVGForActionOrigin(int eOrigin, uint unFlags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetGlyphForActionOrigin_Legacy(int eOrigin);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetStringForActionOrigin(int eOrigin);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetStringForAnalogActionName(IntPtr eActionHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void StopAnalogActionMomentum(IntPtr inputHandle, IntPtr eAction);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate InputMotionData_t GetMotionData(IntPtr inputHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void TriggerVibration(IntPtr inputHandle, ushort usLeftSpeed, ushort usRightSpeed);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void TriggerVibrationExtended(IntPtr inputHandle, ushort usLeftSpeed, ushort usRightSpeed, ushort usLeftTriggerSpeed, ushort usRightTriggerSpeed);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void TriggerSimpleHapticEvent(IntPtr inputHandle, IntPtr eHapticLocation, byte nIntensity, char nGainDB, byte nOtherIntensity, char nOtherGainDB);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetLEDColor(IntPtr inputHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void Legacy_TriggerHapticPulse(IntPtr inputHandle, IntPtr eTargetPad, ushort usDurationMicroSec);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void Legacy_TriggerRepeatedHapticPulse(IntPtr inputHandle, IntPtr eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ShowBindingPanel(IntPtr inputHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ESteamInputType GetInputTypeForHandle(IntPtr inputHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetControllerForGamepadIndex(int nIndex);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetGamepadIndexForController(IntPtr ulinputHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetStringForint(int eOrigin);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr GetGlyphForint(int eOrigin);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetActionOriginFromint(IntPtr inputHandle, int eOrigin);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int TranslateActionOrigin(ESteamInputType eDestinationInputType, int eSourceOrigin);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetDeviceBindingRevision(IntPtr inputHandle, ref int pMajor, ref int pMinor);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint GetRemotePlaySessionID(IntPtr inputHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ushort GetSessionInputConfigurationSettings(IntPtr _);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr SteamAPI_SteamInput_v005(IntPtr _);

    }
}
