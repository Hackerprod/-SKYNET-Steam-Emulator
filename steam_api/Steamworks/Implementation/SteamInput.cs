using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;


namespace SKYNET.Steamworks.Implementation
{
    public class SteamInput : ISteamInterface
    {
        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamInput()
        {
            InterfaceVersion = "SteamInput";
        }

        public void ActivateActionSet(IntPtr inputHandle, IntPtr actionSetHandle)
        {
            Write("ActivateActionSet");
        }

        public void ActivateActionSetLayer(IntPtr inputHandle, IntPtr actionSetLayerHandle)
        {
            Write("ActivateActionSetLayer");
        }

        public bool BNewDataAvailable(IntPtr _)
        {
            Write("BNewDataAvailable");
            return false;
        }

        public bool BWaitForData([MarshalAs(UnmanagedType.U1)] bool bWaitForever, uint unTimeout)
        {
            Write("BWaitForData");
            return false;
        }

        public void DeactivateActionSetLayer(IntPtr inputHandle, IntPtr actionSetLayerHandle)
        {
            Write("DeactivateActionSetLayer");
        }

        public void DeactivateAllActionSetLayers(IntPtr inputHandle)
        {
            Write("DeactivateAllActionSetLayers");
        }

        public void EnableDeviceCallbacks(IntPtr _)
        {
            Write("EnableDeviceCallbacks");
        }

        public int GetActionOriginFromint(IntPtr inputHandle, int eOrigin)
        {
            Write("GetActionOriginFromint");
            return 0;
        }

        public IntPtr GetActionSetHandle(string pszActionSetName)
        {
            Write("GetActionSetHandle");
            return IntPtr.Zero;
        }

        public int GetActiveActionSetLayers(IntPtr inputHandle, [In, Out] IntPtr[] handlesOut)
        {
            Write("GetActiveActionSetLayers");
            return 0;
        }

        public IntPtr GetAnalogActionData(IntPtr inputHandle, IntPtr analogActionHandle)
        {
            Write("GetAnalogActionData");
            return IntPtr.Zero;
        }

        public IntPtr GetAnalogActionHandle(string pszActionName)
        {
            Write("GetAnalogActionHandle");
            return IntPtr.Zero;
        }

        public int GetAnalogActionOrigins(IntPtr inputHandle, IntPtr actionSetHandle, IntPtr analogActionHandle, ref int originsOut)
        {
            Write("GetAnalogActionOrigins");
            return 0;
        }

        public int GetConnectedControllers([In, Out] IntPtr[] handlesOut)
        {
            Write("GetConnectedControllers");
            return 0;
        }

        public IntPtr GetControllerForGamepadIndex(int nIndex)
        {
            Write("GetControllerForGamepadIndex");
            return IntPtr.Zero;
        }

        public IntPtr GetCurrentActionSet(IntPtr inputHandle)
        {
            Write("GetCurrentActionSet");
            return IntPtr.Zero;
        }

        public bool GetDeviceBindingRevision(IntPtr inputHandle, ref int pMajor, ref int pMinor)
        {
            Write("GetDeviceBindingRevision");
            return false;
        }

        public IntPtr GetDigitalActionData(IntPtr inputHandle, IntPtr digitalActionHandle)
        {
            Write("GetDigitalActionData");
            return IntPtr.Zero;
        }

        public IntPtr GetDigitalActionHandle(string pszActionName)
        {
            Write("GetDigitalActionHandle");
            return IntPtr.Zero;
        }

        public int GetDigitalActionOrigins(IntPtr inputHandle, IntPtr actionSetHandle, IntPtr digitalActionHandle, ref int originsOut)
        {
            Write("GetDigitalActionOrigins");
            return 0;
        }

        public int GetGamepadIndexForController(IntPtr ulinputHandle)
        {
            Write("GetGamepadIndexForController");
            return 0;
        }

        public IntPtr GetGlyphForActionOrigin_Legacy(int eOrigin)
        {
            Write("GetGlyphForActionOrigin_Legacy");
            return IntPtr.Zero;
        }

        public IntPtr GetGlyphForint(int eOrigin)
        {
            Write("GetGlyphForint");
            return IntPtr.Zero;
        }

        public IntPtr GetGlyphPNGForActionOrigin(int eOrigin, IntPtr eSize, uint unFlags)
        {
            Write("GetGlyphPNGForActionOrigin");
            return IntPtr.Zero;
        }

        public IntPtr GetGlyphSVGForActionOrigin(int eOrigin, uint unFlags)
        {
            Write("GetGlyphSVGForActionOrigin");
            return IntPtr.Zero;
        }

        public ESteamInputType GetInputTypeForHandle(IntPtr inputHandle)
        {
            Write("GetInputTypeForHandle");
            return ESteamInputType.k_ESteamInputType_Unknown;
        }

        public InputMotionData_t GetMotionData(IntPtr inputHandle)
        {
            Write("xxx");
            return new InputMotionData_t();
        }

        public uint GetRemotePlaySessionID(IntPtr inputHandle)
        {
            Write("GetRemotePlaySessionID");
            return 0;
        }

        public ushort GetSessionInputConfigurationSettings(IntPtr _)
        {
            Write("GetSessionInputConfigurationSettings");
            return 0;
        }

        public IntPtr GetStringForActionOrigin(int eOrigin)
        {
            Write("GetStringForActionOrigin");
            return IntPtr.Zero;
        }

        public IntPtr GetStringForAnalogActionName(IntPtr eActionHandle)
        {
            Write("GetStringForAnalogActionName");
            return IntPtr.Zero;
        }

        public IntPtr GetStringForDigitalActionName(IntPtr eActionHandle)
        {
            Write("GetStringForDigitalActionName");
            return IntPtr.Zero;
        }

        public IntPtr GetStringForint(int eOrigin)
        {
            Write("GetStringForint");
            return IntPtr.Zero;
        }

        public bool Init([MarshalAs(UnmanagedType.U1)] bool bExplicitlyCallRunFrame)
        {
            Write("Init");
            return true;
        }

        public void Legacy_TriggerHapticPulse(IntPtr inputHandle, IntPtr eTargetPad, ushort usDurationMicroSec)
        {
            Write("Legacy_TriggerHapticPulse");
        }

        public void Legacy_TriggerRepeatedHapticPulse(IntPtr inputHandle, IntPtr eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags)
        {
            Write("Legacy_TriggerRepeatedHapticPulse");
        }

        public void RunFrame([MarshalAs(UnmanagedType.U1)] bool bReservedValue)
        {
            Write("RunFrame");
        }

        public bool SetInputActionManifestFilePath(string pchInputActionManifestAbsolutePath)
        {
            Write("SetInputActionManifestFilePath");
            return true;
        }

        public void SetLEDColor(IntPtr inputHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags)
        {
            Write("SetLEDColor");
        }

        public bool ShowBindingPanel(IntPtr inputHandle)
        {
            Write("ShowBindingPanel");
            return false;
        }

        public bool Shutdown(IntPtr _)
        {
            Write("Shutdown");
            return true;
        }

        public void StopAnalogActionMomentum(IntPtr inputHandle, IntPtr eAction)
        {
            Write("StopAnalogActionMomentum");
        }

        public int TranslateActionOrigin(ESteamInputType eDestinationInputType, int eSourceOrigin)
        {
            Write("TranslateActionOrigin");
            return 0;
        }

        public void TriggerSimpleHapticEvent(IntPtr inputHandle, IntPtr eHapticLocation, byte nIntensity, char nGainDB, byte nOtherIntensity, char nOtherGainDB)
        {
            Write("TriggerSimpleHapticEvent");
        }

        public void TriggerVibration(IntPtr inputHandle, ushort usLeftSpeed, ushort usRightSpeed)
        {
            Write("TriggerVibration");
        }

        public void TriggerVibrationExtended(IntPtr inputHandle, ushort usLeftSpeed, ushort usRightSpeed, ushort usLeftTriggerSpeed, ushort usRightTriggerSpeed)
        {
            Write("TriggerVibrationExtended");
        }

        public IntPtr SteamAPI_SteamInput_v005(IntPtr _)
        {
            Write("SteamAPI_SteamInput_v005");
            return SteamEmulator.SteamInput.MemoryAddress;
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}
