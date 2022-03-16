using SKYNET.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate("SteamController")]
    public class DSteamController : IBaseInterfaceMap
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool Init();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool Shutdown();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void RunFrame();
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetConnectedControllers(IntPtr handles, IntPtr handlesOut);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetActionSetHandle(string pszActionSetName);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateActionSet(IntPtr controllerHandle, int actionSetHandle);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetCurrentActionSet(IntPtr controllerHandle);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ActivateActionSetLayer(IntPtr controllerHandle, IntPtr actionSetLayerHandle);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void DeactivateActionSetLayer(IntPtr controllerHandle, IntPtr actionSetLayerHandle);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void DeactivateAllActionSetLayers(IntPtr controllerHandle);
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetActiveActionSetLayers(IntPtr controllerHandle, int handlesOut );
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetDigitalActionHandle(string pszActionName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ControllerDigitalActionData_t GetDigitalActionData(IntPtr controllerHandle, int digitalActionHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetDigitalActionOrigins(IntPtr controllerHandle, int actionSetHandle, int digitalActionHandle, EControllerActionOrigin originsOut );

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetAnalogActionHandle(string pszActionName);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ControllerAnalogActionData_t GetAnalogActionData(uint controllerHandle, uint analogActionHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetAnalogActionOrigins(IntPtr controllerHandle, int actionSetHandle, uint analogActionHandle, EControllerActionOrigin originsOut );

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetGlyphForActionOrigin(EControllerActionOrigin eOrigin);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetStringForActionOrigin(EControllerActionOrigin eOrigin);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void StopAnalogActionMomentum(IntPtr controllerHandle, uint eAction);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ControllerMotionData_t GetMotionData(IntPtr controllerHandle);

        //-----------------------------------------------------------------------------
        // OUTPUTS
        //-----------------------------------------------------------------------------

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void TriggerHapticPulse(IntPtr controllerHandle, ESteamControllerPad eTargetPad, uint usDurationMicroSec);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void TriggerRepeatedHapticPulse(IntPtr controllerHandle, ESteamControllerPad eTargetPad, uint usDurationMicroSec, uint usOffMicroSec, uint unRepeat, int nFlags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void TriggerVibration(IntPtr controllerHandle, uint usLeftSpeed, uint usRightSpeed);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetLEDColor(IntPtr controllerHandle, uint nColorR, uint nColorG, uint nColorB, int nFlags);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ShowBindingPanel(IntPtr controllerHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate ESteamInputType GetInputTypeForHandle(IntPtr controllerHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetControllerForGamepadIndex(int nIndex);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetGamepadIndexForController(IntPtr ulControllerHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetStringForXboxOrigin(EXboxOrigin eOrigin);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate string GetGlyphForXboxOrigin(EXboxOrigin eOrigin);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EControllerActionOrigin GetActionOriginFromXboxOrigin_(IntPtr controllerHandle, EXboxOrigin eOrigin);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate EControllerActionOrigin TranslateActionOrigin(ESteamInputType eDestinationInputType, EControllerActionOrigin eSourceOrigin);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetControllerBindingRevision(IntPtr controllerHandle, int pMajor, int pMinor);
    }
}
