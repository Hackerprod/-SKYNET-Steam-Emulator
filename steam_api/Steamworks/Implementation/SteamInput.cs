using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Core.Interface;
using SKYNET.Interface;

//[Map("SteamInput")]
public class SteamInput : IBaseInterface, ISteamInput
{
    public void ActivateActionSet(IntPtr inputHandle, IntPtr actionSetHandle)
    {
        //
    }

    public void ActivateActionSetLayer(IntPtr inputHandle, IntPtr actionSetLayerHandle)
    {
        //
    }

    public bool BNewDataAvailable(IntPtr _)
    {
        return false;
    }

    public bool BWaitForData([MarshalAs(UnmanagedType.U1)] bool bWaitForever, uint unTimeout)
    {
        return false;
    }

    public void DeactivateActionSetLayer(IntPtr inputHandle, IntPtr actionSetLayerHandle)
    {
        //
    }

    public void DeactivateAllActionSetLayers(IntPtr inputHandle)
    {
        //
    }

    public void EnableDeviceCallbacks(IntPtr _)
    {
        //
    }

    public int GetActionOriginFromint(IntPtr inputHandle, int eOrigin)
    {
        return 0;
    }

    public IntPtr GetActionSetHandle(string pszActionSetName)
    {
        return IntPtr.Zero;
    }

    public int GetActiveActionSetLayers(IntPtr inputHandle, [In, Out] IntPtr[] handlesOut)
    {
        return 0;
    }

    public IntPtr GetAnalogActionData(IntPtr inputHandle, IntPtr analogActionHandle)
    {
        return IntPtr.Zero;
    }

    public IntPtr GetAnalogActionHandle(string pszActionName)
    {
        return IntPtr.Zero;
    }

    public int GetAnalogActionOrigins(IntPtr inputHandle, IntPtr actionSetHandle, IntPtr analogActionHandle, ref int originsOut)
    {
        return 0;
    }

    public int GetConnectedControllers([In, Out] IntPtr[] handlesOut)
    {
        return 0;
    }

    public IntPtr GetControllerForGamepadIndex(int nIndex)
    {
        return IntPtr.Zero;
    }

    public IntPtr GetCurrentActionSet(IntPtr inputHandle)
    {
        return IntPtr.Zero;
    }

    public bool GetDeviceBindingRevision(IntPtr inputHandle, ref int pMajor, ref int pMinor)
    {
        return false;
    }

    public IntPtr GetDigitalActionData(IntPtr inputHandle, IntPtr digitalActionHandle)
    {
        return IntPtr.Zero;
    }

    public IntPtr GetDigitalActionHandle(string pszActionName)
    {
        return IntPtr.Zero;
    }

    public int GetDigitalActionOrigins(IntPtr inputHandle, IntPtr actionSetHandle, IntPtr digitalActionHandle, ref int originsOut)
    {
        return 0;
    }

    public int GetGamepadIndexForController(IntPtr ulinputHandle)
    {
        return 0;
    }

    public IntPtr GetGlyphForActionOrigin_Legacy(int eOrigin)
    {
        return IntPtr.Zero;
    }

    public IntPtr GetGlyphForint(int eOrigin)
    {
        return IntPtr.Zero;
    }

    public IntPtr GetGlyphPNGForActionOrigin(int eOrigin, IntPtr eSize, uint unFlags)
    {
        return IntPtr.Zero;
    }

    public IntPtr GetGlyphSVGForActionOrigin(int eOrigin, uint unFlags)
    {
        return IntPtr.Zero;
    }

    public ESteamInputType GetInputTypeForHandle(IntPtr inputHandle)
    {
        return ESteamInputType.k_ESteamInputType_Unknown;
    }

    public InputMotionData_t GetMotionData(IntPtr inputHandle)
    {
        return new InputMotionData_t();
    }

    public uint GetRemotePlaySessionID(IntPtr inputHandle)
    {
        return 0;
    }

    public ushort GetSessionInputConfigurationSettings(IntPtr _)
    {
        return 0;
    }

    public IntPtr GetStringForActionOrigin(int eOrigin)
    {
        return IntPtr.Zero;
    }

    public IntPtr GetStringForAnalogActionName(IntPtr eActionHandle)
    {
        return IntPtr.Zero;
    }

    public IntPtr GetStringForDigitalActionName(IntPtr eActionHandle)
    {
        return IntPtr.Zero;
    }

    public IntPtr GetStringForint(int eOrigin)
    {
        return IntPtr.Zero;
    }

    public bool Init([MarshalAs(UnmanagedType.U1)] bool bExplicitlyCallRunFrame)
    {
        return true;
    }

    public void Legacy_TriggerHapticPulse(IntPtr inputHandle, IntPtr eTargetPad, ushort usDurationMicroSec)
    {
        //
    }

    public void Legacy_TriggerRepeatedHapticPulse(IntPtr inputHandle, IntPtr eTargetPad, ushort usDurationMicroSec, ushort usOffMicroSec, ushort unRepeat, uint nFlags)
    {
        //
    }

    public void RunFrame([MarshalAs(UnmanagedType.U1)] bool bReservedValue)
    {
        //
    }

    public bool SetInputActionManifestFilePath(string pchInputActionManifestAbsolutePath)
    {
        return true;
    }

    public void SetLEDColor(IntPtr inputHandle, byte nColorR, byte nColorG, byte nColorB, uint nFlags)
    {
        //
    }

    public bool ShowBindingPanel(IntPtr inputHandle)
    {
        return false;
    }

    public bool Shutdown(IntPtr _)
    {
        return true;
    }

    public void StopAnalogActionMomentum(IntPtr inputHandle, IntPtr eAction)
    {
        //
    }

    public int TranslateActionOrigin(ESteamInputType eDestinationInputType, int eSourceOrigin)
    {
        return 0;
    }

    public void TriggerSimpleHapticEvent(IntPtr inputHandle, IntPtr eHapticLocation, byte nIntensity, char nGainDB, byte nOtherIntensity, char nOtherGainDB)
    {
        //
    }

    public void TriggerVibration(IntPtr inputHandle, ushort usLeftSpeed, ushort usRightSpeed)
    {
        //
    }

    public void TriggerVibrationExtended(IntPtr inputHandle, ushort usLeftSpeed, ushort usRightSpeed, ushort usLeftTriggerSpeed, ushort usRightTriggerSpeed)
    {
        //
    }

    public IntPtr SteamAPI_SteamInput_v005(IntPtr _)
    {
        return IntPtr.Zero;
    }
}
