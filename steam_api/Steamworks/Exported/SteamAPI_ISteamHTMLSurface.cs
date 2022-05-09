﻿using System;
using System.Runtime.InteropServices;
using SKYNET.Steamworks;


using SteamAPICall_t = System.UInt64;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamHTMLSurface
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SteamAPI_ISteamHTMLSurface_AddHeader(IntPtr _, IntPtr unBrowserHandle, string pchKey, string pchValue)
        {
            Write("SteamAPI_ISteamHTMLSurface_AddHeader");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_AllowStartRequest(IntPtr _, IntPtr unBrowserHandle, bool bAllowed)
        {
            Write("SteamAPI_ISteamHTMLSurface_AllowStartRequest");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_CopyToClipboard(IntPtr _, IntPtr unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_CopyToClipboard");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static SteamAPICall_t SteamAPI_ISteamHTMLSurface_CreateBrowser(IntPtr _, string pchUserAgent, string pchUserCSS)
        {
            Write("SteamAPI_ISteamHTMLSurface_CreateBrowser");
            return SteamEmulator.SteamHTMLSurface.CreateBrowser(pchUserAgent, pchUserCSS);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_ExecuteJavascript(IntPtr _, IntPtr unBrowserHandle, string pchScript)
        {
            Write("SteamAPI_ISteamHTMLSurface_ExecuteJavascript");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_FileLoadDialogResponse(IntPtr _, IntPtr unBrowserHandle, string pchSelectedFiles)
        {
            Write("SteamAPI_ISteamHTMLSurface_FileLoadDialogResponse");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_Find(IntPtr _, IntPtr unBrowserHandle, string pchSearchStr, bool bCurrentlyInFind, bool bReverse)
        {
            Write("SteamAPI_ISteamHTMLSurface_Find");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_GetLinkAtPosition(IntPtr _, IntPtr unBrowserHandle, int x, int y)
        {
            Write("SteamAPI_ISteamHTMLSurface_GetLinkAtPosition");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_GoBack(IntPtr _, IntPtr unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_GoBack");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_GoForward(IntPtr _, IntPtr unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_GoForward");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTMLSurface_Init(IntPtr _)
        {
            Write("SteamAPI_ISteamHTMLSurface_Init");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_JSDialogResponse(IntPtr _, IntPtr unBrowserHandle, bool bResult)
        {
            Write("SteamAPI_ISteamHTMLSurface_JSDialogResponse");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_KeyDown(IntPtr _, IntPtr unBrowserHandle, int nNativeKeyCode, IntPtr IntPtr, bool bIsSystemKey = false)
        {
            Write("SteamAPI_ISteamHTMLSurface_KeyDown");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_Keystring(IntPtr _, IntPtr unBrowserHandle, int cUnicodestring, IntPtr IntPtr)
        {
            Write("SteamAPI_ISteamHTMLSurface_Keystring");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_KeyUp(IntPtr _, IntPtr unBrowserHandle, int nNativeKeyCode, IntPtr IntPtr)
        {
            Write("SteamAPI_ISteamHTMLSurface_KeyUp");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_LoadURL(IntPtr _, IntPtr unBrowserHandle, string pchURL, string pchPostData)
        {
            Write("SteamAPI_ISteamHTMLSurface_LoadURL");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_MouseDoubleClick(IntPtr _, IntPtr unBrowserHandle, EHTMLMouseButton eMouseButton)
        {
            Write("SteamAPI_ISteamHTMLSurface_MouseDoubleClick");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_MouseDown(IntPtr _, IntPtr unBrowserHandle, EHTMLMouseButton eMouseButton)
        {
            Write("SteamAPI_ISteamHTMLSurface_MouseDown");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_MouseMove(IntPtr _, IntPtr unBrowserHandle, int x, int y)
        {
            Write("SteamAPI_ISteamHTMLSurface_MouseMove");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_MouseUp(IntPtr _, IntPtr unBrowserHandle, EHTMLMouseButton eMouseButton)
        {
            Write("SteamAPI_ISteamHTMLSurface_MouseUp");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_MouseWheel(IntPtr _, IntPtr unBrowserHandle, int nDelta)
        {
            Write("SteamAPI_ISteamHTMLSurface_MouseWheel");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_OpenDeveloperTools(IntPtr _, IntPtr unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_OpenDeveloperTools");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_PasteFromClipboard(IntPtr _, IntPtr unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_PasteFromClipboard");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_Reload(IntPtr _, IntPtr unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_Reload");
        }
        [DllExport(CallingConvention = CallingConvention.Cdecl)]

        public static void SteamAPI_ISteamHTMLSurface_RemoveBrowser(IntPtr _, IntPtr unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_RemoveBrowser");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SetBackgroundMode(IntPtr _, IntPtr unBrowserHandle, bool bBackgroundMode)
        {
            Write("SteamAPI_ISteamHTMLSurface_SetBackgroundMode");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SetCookie(IntPtr _, string pchHostname, string pchKey, string pchValue, string pchPath, IntPtr nExpires, bool bSecure = false, bool bHTTPOnly = false)
        {
            Write("SteamAPI_ISteamHTMLSurface_SetCookie");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SetDPIScalingFactor(IntPtr _, IntPtr unBrowserHandle, float flDPIScaling)
        {
            Write("SteamAPI_ISteamHTMLSurface_SetDPIScalingFactor");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SetHorizontalScroll(IntPtr _, IntPtr unBrowserHandle, int nAbsolutePixelScroll)
        {
            Write("SteamAPI_ISteamHTMLSurface_SetHorizontalScroll");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SetKeyFocus(IntPtr _, IntPtr unBrowserHandle, bool bHasKeyFocus)
        {
            Write("SteamAPI_ISteamHTMLSurface_SetKeyFocus");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SetPageScaleFactor(IntPtr _, IntPtr unBrowserHandle, float flZoom, int nPointX, int nPointY)
        {
            Write("SteamAPI_ISteamHTMLSurface_SetPageScaleFactor");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SetSize(IntPtr _, IntPtr unBrowserHandle, int unWidth, int unHeight)
        {
            Write("SteamAPI_ISteamHTMLSurface_SetSize");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_SetVerticalScroll(IntPtr _, IntPtr unBrowserHandle, int nAbsolutePixelScroll)
        {
            Write("SteamAPI_ISteamHTMLSurface_SetVerticalScroll");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTMLSurface_Shutdown(IntPtr _)
        {
            Write("SteamAPI_ISteamHTMLSurface_Shutdown");
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_StopFind(IntPtr _, IntPtr unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_StopFind");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_StopLoad(IntPtr _, IntPtr unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_StopLoad");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamHTMLSurface_ViewSource(IntPtr _, IntPtr unBrowserHandle)
        {
            Write("SteamAPI_ISteamHTMLSurface_ViewSource");
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}