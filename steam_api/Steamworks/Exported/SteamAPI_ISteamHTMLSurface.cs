using System;
using System.Runtime.InteropServices;

using SteamAPICall_t = System.UInt64;
using HHTMLBrowser = System.UInt32;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamHTMLSurface
    {
        static SteamAPI_ISteamHTMLSurface()
        {
            if (!SteamEmulator.Initialized && !SteamEmulator.Initializing)
            {
                SteamEmulator.Initialize();
            }
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SteamAPI_ISteamHTMLSurface_AddHeader(IntPtr _, HHTMLBrowser unBrowserHandle, string pchKey, string pchValue)
        {
            SteamAPI_ISteamHTMLSurface_AddHeader(_, unBrowserHandle, pchKey, pchValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_AddHeader(IntPtr _, HHTMLBrowser unBrowserHandle, string pchKey, string pchValue)
        {
            Write("SteamAPI_ISteamHTMLSurface_AddHeader");
            SteamEmulator.SteamHTMLSurface.AddHeader(unBrowserHandle, pchKey, pchValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_AllowStartRequest(IntPtr _, HHTMLBrowser unBrowserHandle, bool bAllowed)
        {
            Write("SteamAPI_ISteamHTMLSurface_AllowStartRequest");
            SteamEmulator.SteamHTMLSurface.AllowStartRequest(unBrowserHandle, bAllowed);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_CopyToClipboard(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_CopyToClipboard");
            SteamEmulator.SteamHTMLSurface.CopyToClipboard(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamHTMLSurface_CreateBrowser(IntPtr _, string pchUserAgent, string pchUserCSS)
        {
            Write("SteamAPI_ISteamHTMLSurface_CreateBrowser");
            return SteamEmulator.SteamHTMLSurface.CreateBrowser(pchUserAgent, pchUserCSS);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_ExecuteJavascript(IntPtr _, HHTMLBrowser unBrowserHandle, string pchScript)
        {
            Write("SteamAPI_ISteamHTMLSurface_ExecuteJavascript");
            SteamEmulator.SteamHTMLSurface.ExecuteJavascript(unBrowserHandle, pchScript);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_FileLoadDialogResponse(IntPtr _, HHTMLBrowser unBrowserHandle, IntPtr pchSelectedFiles)
        {
            Write("SteamAPI_ISteamHTMLSurface_FileLoadDialogResponse");
            SteamEmulator.SteamHTMLSurface.FileLoadDialogResponse(unBrowserHandle, pchSelectedFiles);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_Find(IntPtr _, HHTMLBrowser unBrowserHandle, string pchSearchStr, bool bCurrentlyInFind, bool bReverse)
        {
            Write("SteamAPI_ISteamHTMLSurface_Find");
            SteamEmulator.SteamHTMLSurface.Find(unBrowserHandle, pchSearchStr, bCurrentlyInFind, bReverse);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_GetLinkAtPosition(IntPtr _, HHTMLBrowser unBrowserHandle, int x, int y)
        {
            Write("SteamAPI_ISteamHTMLSurface_GetLinkAtPosition");
            SteamEmulator.SteamHTMLSurface.GetLinkAtPosition(unBrowserHandle, x, y);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_GoBack(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_GoBack");
            SteamEmulator.SteamHTMLSurface.GoBack(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_GoForward(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_GoForward");
            SteamEmulator.SteamHTMLSurface.GoForward(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTMLSurface_Init(IntPtr _)
        {
            Write("SteamAPI_ISteamHTMLSurface_Init");
            return SteamEmulator.SteamHTMLSurface.Init();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_JSDialogResponse(IntPtr _, HHTMLBrowser unBrowserHandle, bool bResult)
        {
            Write("SteamAPI_ISteamHTMLSurface_JSDialogResponse");
            SteamEmulator.SteamHTMLSurface.JSDialogResponse(unBrowserHandle, bResult);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_KeyDown(IntPtr _, HHTMLBrowser unBrowserHandle, uint nNativeKeyCode, int eHTMLKeyModifiers, bool bIsSystemKey = false)
        {
            Write("SteamAPI_ISteamHTMLSurface_KeyDown");
            SteamEmulator.SteamHTMLSurface.KeyDown(unBrowserHandle, nNativeKeyCode, eHTMLKeyModifiers, bIsSystemKey);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_KeyChar(IntPtr _, HHTMLBrowser unBrowserHandle, uint cUnicodeChar, int eHTMLKeyModifiers)
        {
            Write("SteamAPI_ISteamHTMLSurface_KeyChar");
            SteamEmulator.SteamHTMLSurface.KeyChar(unBrowserHandle, cUnicodeChar, eHTMLKeyModifiers);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_Keystring(IntPtr _, HHTMLBrowser unBrowserHandle, int cUnicodestring, IntPtr IntPtr)
        {
            Write("SteamAPI_ISteamHTMLSurface_Keystring");
            SteamEmulator.SteamHTMLSurface.Keystring(unBrowserHandle, cUnicodestring, IntPtr);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_KeyUp(IntPtr _, HHTMLBrowser unBrowserHandle, uint nNativeKeyCode, int eHTMLKeyModifiers)
        {
            Write("SteamAPI_ISteamHTMLSurface_KeyUp");
            SteamEmulator.SteamHTMLSurface.KeyUp(unBrowserHandle, nNativeKeyCode, eHTMLKeyModifiers);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_LoadURL(IntPtr _, HHTMLBrowser unBrowserHandle, string pchURL, string pchPostData)
        {
            Write("SteamAPI_ISteamHTMLSurface_LoadURL");
            SteamEmulator.SteamHTMLSurface.LoadURL(unBrowserHandle, pchURL, pchPostData);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_MouseDoubleClick(IntPtr _, HHTMLBrowser unBrowserHandle, EHTMLMouseButton eMouseButton)
        {
            Write("SteamAPI_ISteamHTMLSurface_MouseDoubleClick");
            SteamEmulator.SteamHTMLSurface.MouseDoubleClick(unBrowserHandle, (int)eMouseButton);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_MouseDown(IntPtr _, HHTMLBrowser unBrowserHandle, EHTMLMouseButton eMouseButton)
        {
            Write("SteamAPI_ISteamHTMLSurface_MouseDown");
            SteamEmulator.SteamHTMLSurface.MouseDown(unBrowserHandle, (int)eMouseButton);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_MouseMove(IntPtr _, HHTMLBrowser unBrowserHandle, int x, int y)
        {
            Write("SteamAPI_ISteamHTMLSurface_MouseMove");
            SteamEmulator.SteamHTMLSurface.MouseMove(unBrowserHandle, x, y);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_MouseUp(IntPtr _, HHTMLBrowser unBrowserHandle, EHTMLMouseButton eMouseButton)
        {
            Write("SteamAPI_ISteamHTMLSurface_MouseUp");
            SteamEmulator.SteamHTMLSurface.MouseUp(unBrowserHandle, (int)eMouseButton);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_MouseWheel(IntPtr _, HHTMLBrowser unBrowserHandle, int nDelta)
        {
            Write("SteamAPI_ISteamHTMLSurface_MouseWheel");
            SteamEmulator.SteamHTMLSurface.MouseWheel(unBrowserHandle, nDelta);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_OpenDeveloperTools(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_OpenDeveloperTools");
            SteamEmulator.SteamHTMLSurface.OpenDeveloperTools(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_PasteFromClipboard(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_PasteFromClipboard");
            SteamEmulator.SteamHTMLSurface.PasteFromClipboard(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_Reload(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_Reload");
            SteamEmulator.SteamHTMLSurface.Reload(unBrowserHandle);
        }
        [DllExport(CallingConvention = CallingConvention.Cdecl)]

        public static void SteamAPI_ISteamHTMLSurface_RemoveBrowser(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_RemoveBrowser");
            SteamEmulator.SteamHTMLSurface.RemoveBrowser(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SetBackgroundMode(IntPtr _, HHTMLBrowser unBrowserHandle, bool bBackgroundMode)
        {
            Write("SteamAPI_ISteamHTMLSurface_SetBackgroundMode");
            SteamEmulator.SteamHTMLSurface.SetBackgroundMode(unBrowserHandle, bBackgroundMode);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SetCookie(IntPtr _, string pchHostname, string pchKey, string pchValue, string pchPath, uint nExpires, bool bSecure = false, bool bHTTPOnly = false)
        {
            Write("SteamAPI_ISteamHTMLSurface_SetCookie");
            SteamEmulator.SteamHTMLSurface.SetCookie(pchHostname, pchKey, pchValue, pchPath, nExpires, bSecure, bHTTPOnly);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SetDPIScalingFactor(IntPtr _, HHTMLBrowser unBrowserHandle, float flDPIScaling)
        {
            Write("SteamAPI_ISteamHTMLSurface_SetDPIScalingFactor");
            SteamEmulator.SteamHTMLSurface.SetDPIScalingFactor(unBrowserHandle, flDPIScaling);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SetHorizontalScroll(IntPtr _, HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll)
        {
            Write("SteamAPI_ISteamHTMLSurface_SetHorizontalScroll");
            SteamEmulator.SteamHTMLSurface.SetHorizontalScroll(unBrowserHandle, nAbsolutePixelScroll);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SetKeyFocus(IntPtr _, HHTMLBrowser unBrowserHandle, bool bHasKeyFocus)
        {
            Write("SteamAPI_ISteamHTMLSurface_SetKeyFocus");
            SteamEmulator.SteamHTMLSurface.SetKeyFocus(unBrowserHandle, bHasKeyFocus);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SetPageScaleFactor(IntPtr _, HHTMLBrowser unBrowserHandle, float flZoom, int nPointX, int nPointY)
        {
            Write("SteamAPI_ISteamHTMLSurface_SetPageScaleFactor");
            SteamEmulator.SteamHTMLSurface.SetPageScaleFactor(unBrowserHandle, flZoom, nPointX, nPointY);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SetSize(IntPtr _, HHTMLBrowser unBrowserHandle, uint unWidth, uint unHeight)
        {
            Write("SteamAPI_ISteamHTMLSurface_SetSize");
            SteamEmulator.SteamHTMLSurface.SetSize(unBrowserHandle, unWidth, unHeight);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SetVerticalScroll(IntPtr _, HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll)
        {
            Write("SteamAPI_ISteamHTMLSurface_SetVerticalScroll");
            SteamEmulator.SteamHTMLSurface.SetVerticalScroll(unBrowserHandle, nAbsolutePixelScroll);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTMLSurface_Shutdown(IntPtr _)
        {
            Write("SteamAPI_ISteamHTMLSurface_Shutdown");
            return SteamEmulator.SteamHTMLSurface.Shutdown();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_StopFind(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_StopFind");
            SteamEmulator.SteamHTMLSurface.StopFind(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_StopLoad(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_StopLoad");
            SteamEmulator.SteamHTMLSurface.StopLoad(unBrowserHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_ViewSource(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_ViewSource");
            SteamEmulator.SteamHTMLSurface.ViewSource(unBrowserHandle);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}
