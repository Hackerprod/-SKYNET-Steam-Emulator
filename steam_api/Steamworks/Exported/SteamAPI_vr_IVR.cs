using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using int32_t = System.Int32;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_vr_IVR
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRSystem_GetWindowBounds(IntPtr _, int32_t pnX, int32_t pnY, uint32_t pnWidth, uint32_t pnHeight)
        {
            Write("SteamAPI_vr_IVRSystem_GetWindowBounds");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRSystem_GetRecommendedRenderTargetSize(IntPtr _, uint32_t pnWidth, uint32_t pnHeight)
        {
            Write("SteamAPI_vr_IVRSystem_GetRecommendedRenderTargetSize");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRSystem_GetEyeOutputViewport(IntPtr _, IntPtr eEye, uint32_t pnX, uint32_t pnY, uint32_t pnWidth, uint32_t pnHeight)
        {
            Write("SteamAPI_vr_IVRSystem_GetEyeOutputViewport");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_vr_IVRSystem_GetProjectionMatrix(IntPtr _, IntPtr eEye, float fNearZ, float fFarZ, IntPtr eProjType)
        {
            Write("SteamAPI_vr_IVRSystem_GetProjectionMatrix");
            return IntPtr.Zero;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRSystem_GetProjectionRaw(IntPtr _, IntPtr eEye, float pfLeft, float pfRight, float pfTop, float pfBottom)
        {
            Write("SteamAPI_vr_IVRSystem_GetProjectionRaw");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_vr_IVRSystem_ComputeDistortion(IntPtr _, IntPtr eEye, float fU, float fV)
        {
            Write("SteamAPI_vr_IVRSystem_ComputeDistortion");
            return IntPtr.Zero;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_vr_IVRSystem_GetEyeToHeadTransform(IntPtr _, IntPtr eEye)
        {
            Write("SteamAPI_vr_IVRSystem_GetEyeToHeadTransform");
            return IntPtr.Zero;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_vr_IVRSystem_GetTimeSinceLastVsync(IntPtr _, float pfSecondsSinceLastVsync, uint64_t pulFrameCounter)
        {
            Write("SteamAPI_vr_IVRSystem_GetTimeSinceLastVsync");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int32_t SteamAPI_vr_IVRSystem_GetD3D9AdapterIndex(IntPtr _)
        {
            Write("SteamAPI_vr_IVRSystem_GetD3D9AdapterIndex");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRSystem_GetDXGIOutputInfo(IntPtr _, int32_t pnAdapterIndex, int32_t pnAdapterOutputIndex)
        {
            Write("SteamAPI_vr_IVRSystem_GetDXGIOutputInfo");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRSystem_AttachToWindow(IntPtr _, IntPtr hWnd)
        {
            Write("SteamAPI_vr_IVRSystem_AttachToWindow");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRSystem_GetDeviceToAbsoluteTrackingPose(IntPtr _, IntPtr eOrigin, float fPredictedSecondsToPhotonsFromNow, IntPtr pTrackedDevicePoseArray, uint32_t unTrackedDevicePoseArrayCount)
        {
            Write("SteamAPI_vr_IVRSystem_GetDeviceToAbsoluteTrackingPose");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRSystem_ResetSeatedZeroPose(IntPtr _)
        {
            Write("SteamAPI_vr_IVRSystem_ResetSeatedZeroPose");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_vr_IVRSystem_GetSeatedZeroPoseToStandingAbsoluteTrackingPose(IntPtr _)
        {
            Write("SteamAPI_vr_IVRSystem_GetSeatedZeroPoseToStandingAbsoluteTrackingPose");
            return IntPtr.Zero;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_vr_IVRSystem_LoadRenderModel(IntPtr _, string pchRenderModelName, IntPtr pRenderModel)
        {
            Write("SteamAPI_vr_IVRSystem_LoadRenderModel");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRSystem_FreeRenderModel(IntPtr _, IntPtr pRenderModel)
        {
            Write("SteamAPI_vr_IVRSystem_FreeRenderModel");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_vr_IVRSystem_GetIntPtr(IntPtr _, IntPtr unDeviceIndex)
        {
            Write("SteamAPI_vr_IVRSystem_GetIntPtr");
            return IntPtr.Zero;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_vr_IVRSystem_IsTrackedDeviceConnected(IntPtr _, IntPtr unDeviceIndex)
        {
            Write("SteamAPI_vr_IVRSystem_IsTrackedDeviceConnected");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_vr_IVRSystem_GetBoolIntPtr(IntPtr _, IntPtr unDeviceIndex, IntPtr prop, IntPtr pError)
        {
            Write("SteamAPI_vr_IVRSystem_GetBoolIntPtr");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static float SteamAPI_vr_IVRSystem_GetFloatIntPtr(IntPtr _, IntPtr unDeviceIndex, IntPtr prop, IntPtr pError)
        {
            Write("SteamAPI_vr_IVRSystem_GetFloatIntPtr");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int32_t SteamAPI_vr_IVRSystem_GetInt32IntPtr(IntPtr _, IntPtr unDeviceIndex, IntPtr prop, IntPtr pError)
        {
            Write("SteamAPI_vr_IVRSystem_GetInt32IntPtr");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint64_t SteamAPI_vr_IVRSystem_GetUint64IntPtr(IntPtr _, IntPtr unDeviceIndex, IntPtr prop, IntPtr pError)
        {
            Write("SteamAPI_vr_IVRSystem_GetUint64IntPtr");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_vr_IVRSystem_GetMatrix34IntPtr(IntPtr _, IntPtr unDeviceIndex, IntPtr prop, IntPtr pError)
        {
            Write("SteamAPI_vr_IVRSystem_GetMatrix34IntPtr");
            return IntPtr.Zero;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint32_t SteamAPI_vr_IVRSystem_GetStringIntPtr(IntPtr _, IntPtr unDeviceIndex, IntPtr prop, IntPtr pchValue, uint32_t unBufferSize, IntPtr pError)
        {
            Write("SteamAPI_vr_IVRSystem_GetStringIntPtr");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static string SteamAPI_vr_IVRSystem_GetPropErrorNameFromEnum(IntPtr _, IntPtr error)
        {
            Write("SteamAPI_vr_IVRSystem_GetPropErrorNameFromEnum");
            return "";
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]

        public static bool SteamAPI_vr_IVRSystem_PollNextEvent(IntPtr _, IntPtr pEvent)
        {
            Write("SteamAPI_vr_IVRSystem_PollNextEvent");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]

        public static string SteamAPI_vr_IVRSystem_GetEventTypeNameFromEnum(IntPtr _, int eType)
        {
            Write("SteamAPI_vr_IVRSystem_GetEventTypeNameFromEnum");
            return "";
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]

        public static IntPtr SteamAPI_vr_IVRSystem_GetHiddenAreaMesh(IntPtr _, IntPtr eEye)
        {
            Write("SteamAPI_vr_IVRSystem_GetHiddenAreaMesh");
            return IntPtr.Zero;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_vr_IVRChaperone_GetCalibrationState(IntPtr _)
        {
            Write("SteamAPI_vr_IVRChaperone_GetCalibrationState");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_vr_IVRChaperone_GetSoftBoundsInfo(IntPtr _, IntPtr pInfo)
        {
            Write("SteamAPI_vr_IVRChaperone_GetSoftBoundsInfo");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]

        public static bool SteamAPI_vr_IVRChaperone_GetHardBoundsInfo(IntPtr _, IntPtr pQuadsBuffer, uint32_t punQuadsCount)
        {
            Write("SteamAPI_vr_IVRChaperone_GetHardBoundsInfo");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_vr_IVRChaperone_GetSeatedBoundsInfo(IntPtr _, IntPtr pInfo)
        {
            Write("SteamAPI_vr_IVRChaperone_GetSeatedBoundsInfo");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint32_t SteamAPI_vr_IVRCompositor_GetLastError(IntPtr _, IntPtr pchBuffer, uint32_t unBufferSize)
        {
            Write("SteamAPI_vr_IVRCompositor_GetLastError");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRCompositor_SetVSync(IntPtr _, bool bVSync)
        {
            Write("SteamAPI_vr_IVRCompositor_SetVSync");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_vr_IVRCompositor_GetVSync(IntPtr _)
        {
            Write("SteamAPI_vr_IVRCompositor_GetVSync");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRCompositor_SetGamma(IntPtr _, float fGamma)
        {
            Write("SteamAPI_vr_IVRCompositor_SetGamma");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static float SteamAPI_vr_IVRCompositor_GetGamma(IntPtr _)
        {
            Write("SteamAPI_vr_IVRCompositor_GetGamma");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRCompositor_SetGraphicsDevice(IntPtr _, int eType, IntPtr pDevice)
        {
            Write("SteamAPI_vr_IVRCompositor_SetGraphicsDevice");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRCompositor_WaitGetPoses(IntPtr _, IntPtr pPoseArray, uint32_t unPoseArrayCount)
        {
            Write("SteamAPI_vr_IVRCompositor_WaitGetPoses");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRCompositor_Submit(IntPtr _, IntPtr eEye, IntPtr pTexture, IntPtr pBounds)
        {
            Write("SteamAPI_vr_IVRCompositor_Submit");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRCompositor_ClearLastSubmittedFrame(IntPtr _)
        {
            Write("SteamAPI_vr_IVRCompositor_ClearLastSubmittedFrame");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRCompositor_GetOverlayDefaults(IntPtr _, IntPtr pSettings)
        {
            Write("SteamAPI_vr_IVRCompositor_GetOverlayDefaults");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRCompositor_SetOverlay(IntPtr _, IntPtr pTexture, IntPtr pSettings)
        {
            Write("SteamAPI_vr_IVRCompositor_SetOverlay");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRCompositor_SetOverlayRaw(IntPtr _, IntPtr buffer, uint32_t width, uint32_t height, uint32_t depth, IntPtr pSettings)
        {
            Write("SteamAPI_vr_IVRCompositor_SetOverlayRaw");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRCompositor_SetOverlayFromFile(IntPtr _, string pchFilePath, IntPtr pSettings)
        {
            Write("SteamAPI_vr_IVRCompositor_SetOverlayFromFile");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRCompositor_ClearOverlay(IntPtr _)
        {
            Write("SteamAPI_vr_IVRCompositor_ClearOverlay");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_vr_IVRCompositor_GetFrameTiming(IntPtr _, IntPtr pTiming, uint32_t unFramesAgo)
        {
            Write("SteamAPI_vr_IVRCompositor_GetFrameTiming");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRCompositor_FadeToColor(IntPtr _, float fSeconds, float fRed, float fGreen, float fBlue, float fAlpha, bool bBackground)
        {
            Write("SteamAPI_vr_IVRCompositor_FadeToColor");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRCompositor_FadeGrid(IntPtr _, float fSeconds, bool bFadeIn)
        {
            Write("SteamAPI_vr_IVRCompositor_FadeGrid");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRCompositor_CompositorBringToFront(IntPtr _)
        {
            Write("SteamAPI_vr_IVRCompositor_CompositorBringToFront");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRCompositor_CompositorGoToBack(IntPtr _)
        {
            Write("SteamAPI_vr_IVRCompositor_CompositorGoToBack");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRCompositor_CompositorQuit(IntPtr _)
        {
            Write("SteamAPI_vr_IVRCompositor_CompositorQuit");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_vr_IVRCompositor_IsFullscreen(IntPtr _)
        {
            Write("SteamAPI_vr_IVRCompositor_IsFullscreen");
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint32_t SteamAPI_vr_IVRControlPanel_GetDriverCount(IntPtr _)
        {
            Write("SteamAPI_vr_IVRControlPanel_GetDriverCount");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint32_t SteamAPI_vr_IVRControlPanel_GetDriverId(IntPtr _, uint32_t unDriverIndex, IntPtr pchBuffer, uint32_t unBufferLen)
        {
            Write("SteamAPI_vr_IVRControlPanel_GetDriverId");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint32_t SteamAPI_vr_IVRControlPanel_GetDriverDisplayCount(IntPtr _, string pchDriverId)
        {
            Write("SteamAPI_vr_IVRControlPanel_GetDriverDisplayCount");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint32_t SteamAPI_vr_IVRControlPanel_GetDriverDisplayId(IntPtr _, string pchDriverId, uint32_t unDisplayIndex, IntPtr pchBuffer, uint32_t unBufferLen)
        {
            Write("SteamAPI_vr_IVRControlPanel_GetDriverDisplayId");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint32_t SteamAPI_vr_IVRControlPanel_GetDriverDisplayModelNumber(IntPtr _, string pchDriverId, string pchDisplayId, IntPtr pchBuffer, uint32_t unBufferLen)
        {
            Write("SteamAPI_vr_IVRControlPanel_GetDriverDisplayModelNumber");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint32_t SteamAPI_vr_IVRControlPanel_GetDriverDisplaySerialNumber(IntPtr _, string pchDriverId, string pchDisplayId, IntPtr pchBuffer, uint32_t unBufferLen)
        {
            Write("SteamAPI_vr_IVRControlPanel_GetDriverDisplaySerialNumber");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint32_t SteamAPI_vr_IVRControlPanel_LoadSharedResource(IntPtr _, string pchResourceName, IntPtr pchBuffer, uint32_t unBufferLen)
        {
            Write("SteamAPI_vr_IVRControlPanel_LoadSharedResource");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static float SteamAPI_vr_IVRControlPanel_GetIPD(IntPtr _)
        {
            Write("SteamAPI_vr_IVRControlPanel_GetIPD");
            return 0;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_vr_IVRControlPanel_SetIPD(IntPtr _, float fIPD)
        {
            Write("SteamAPI_vr_IVRControlPanel_SetIPD");
        }

        private static void Write(object msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
