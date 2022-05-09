using System;
using HHTMLBrowser = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("STEAMHTMLSURFACE_INTERFACE_VERSION_004")]
    public class SteamHTMLSurface004 : ISteamInterface
    {
        //        public ~ISteamHTMLSurface004(IntPtr _) {}
        //{
        //    return  SteamEmulator.SteamHTMLSurface.ISteamHTMLSurface004(_);
        //}

        public bool Init(IntPtr _)
        {
            return SteamEmulator.SteamHTMLSurface.Init();
        }

        public bool Shutdown(IntPtr _)
        {
            return SteamEmulator.SteamHTMLSurface.Shutdown();
        }

        public ulong CreateBrowser(IntPtr _, string pchUserAgent, string pchUserCSS)
        {
            return SteamEmulator.SteamHTMLSurface.CreateBrowser(pchUserAgent, pchUserCSS);
        }

        public void RemoveBrowser(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.RemoveBrowser(unBrowserHandle);
        }

        public void LoadURL(IntPtr _, HHTMLBrowser unBrowserHandle, string pchURL, string pchPostData)
        {
            SteamEmulator.SteamHTMLSurface.LoadURL(unBrowserHandle, pchURL, pchPostData);
        }

        public void SetSize(IntPtr _, HHTMLBrowser unBrowserHandle, uint unWidth, uint unHeight)
        {
            SteamEmulator.SteamHTMLSurface.SetSize(unBrowserHandle, unWidth, unHeight);
        }

        public void StopLoad(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.StopLoad(unBrowserHandle);
        }

        public void Reload(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.Reload(unBrowserHandle);
        }

        public void GoBack(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.GoBack(unBrowserHandle);
        }

        public void GoForward(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.GoForward(unBrowserHandle);
        }

        public void AddHeader(IntPtr _, HHTMLBrowser unBrowserHandle, string pchKey, string pchValue)
        {
            SteamEmulator.SteamHTMLSurface.AddHeader(unBrowserHandle, pchKey, pchValue);
        }

        public void ExecuteJavascript(IntPtr _, HHTMLBrowser unBrowserHandle, string pchScript)
        {
            SteamEmulator.SteamHTMLSurface.ExecuteJavascript(unBrowserHandle, pchScript);
        }

        public void MouseUp(IntPtr _, HHTMLBrowser unBrowserHandle, int eMouseButton)
        {
            SteamEmulator.SteamHTMLSurface.MouseUp(unBrowserHandle, eMouseButton);
        }

        public void MouseDown(IntPtr _, HHTMLBrowser unBrowserHandle, int eMouseButton)
        {
            SteamEmulator.SteamHTMLSurface.MouseDown(unBrowserHandle, eMouseButton);
        }

        public void MouseDoubleClick(IntPtr _, HHTMLBrowser unBrowserHandle, int eMouseButton)
        {
            SteamEmulator.SteamHTMLSurface.MouseDoubleClick(unBrowserHandle, eMouseButton);
        }

        public void MouseMove(IntPtr _, HHTMLBrowser unBrowserHandle, int x, int y)
        {
            SteamEmulator.SteamHTMLSurface.MouseMove(unBrowserHandle, x, y);
        }

        public void MouseWheel(IntPtr _, HHTMLBrowser unBrowserHandle, int nDelta)
        {
            SteamEmulator.SteamHTMLSurface.MouseWheel(unBrowserHandle, nDelta);
        }

        public void KeyDown(IntPtr _, HHTMLBrowser unBrowserHandle, uint nNativeKeyCode, int eHTMLKeyModifiers)
        {
            SteamEmulator.SteamHTMLSurface.KeyDown(unBrowserHandle, nNativeKeyCode, eHTMLKeyModifiers);
        }

        public void KeyUp(IntPtr _, HHTMLBrowser unBrowserHandle, uint nNativeKeyCode, int eHTMLKeyModifiers)
        {
            SteamEmulator.SteamHTMLSurface.KeyUp(unBrowserHandle, nNativeKeyCode, eHTMLKeyModifiers);
        }

        public void KeyChar(IntPtr _, HHTMLBrowser unBrowserHandle, uint cUnicodeChar, int eHTMLKeyModifiers)
        {
            SteamEmulator.SteamHTMLSurface.KeyChar(unBrowserHandle, cUnicodeChar, eHTMLKeyModifiers);
        }

        public void SetHorizontalScroll(IntPtr _, HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll)
        {
            SteamEmulator.SteamHTMLSurface.SetHorizontalScroll(unBrowserHandle, nAbsolutePixelScroll);
        }

        public void SetVerticalScroll(IntPtr _, HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll)
        {
            SteamEmulator.SteamHTMLSurface.SetVerticalScroll(unBrowserHandle, nAbsolutePixelScroll);
        }

        public void SetKeyFocus(IntPtr _, HHTMLBrowser unBrowserHandle, bool bHasKeyFocus)
        {
            SteamEmulator.SteamHTMLSurface.SetKeyFocus(unBrowserHandle, bHasKeyFocus);
        }

        public void ViewSource(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.ViewSource(unBrowserHandle);
        }

        public void CopyToClipboard(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.CopyToClipboard(unBrowserHandle);
        }

        public void PasteFromClipboard(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.PasteFromClipboard(unBrowserHandle);
        }

        public void Find(IntPtr _, HHTMLBrowser unBrowserHandle, string pchSearchStr, bool bCurrentlyInFind, bool bReverse)
        {
            SteamEmulator.SteamHTMLSurface.Find(unBrowserHandle, pchSearchStr, bCurrentlyInFind, bReverse);
        }

        public void StopFind(IntPtr _, HHTMLBrowser unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.StopFind(unBrowserHandle);
        }

        public void GetLinkAtPosition(IntPtr _, HHTMLBrowser unBrowserHandle, int x, int y)
        {
            SteamEmulator.SteamHTMLSurface.GetLinkAtPosition(unBrowserHandle, x, y);
        }

        public void SetCookie(IntPtr _, string pchHostname, string pchKey, string pchValue, string pchPath = "/", uint nExpires = 0, bool bSecure = false, bool bHTTPOnly = false)
        {
            SteamEmulator.SteamHTMLSurface.SetCookie(pchHostname, pchKey, pchValue, pchPath, nExpires, bSecure, bHTTPOnly);
        }

        public void SetPageScaleFactor(IntPtr _, HHTMLBrowser unBrowserHandle, float flZoom, int nPointX, int nPointY)
        {
            SteamEmulator.SteamHTMLSurface.SetPageScaleFactor(unBrowserHandle, flZoom, nPointX, nPointY);
        }

        public void SetBackgroundMode(IntPtr _, HHTMLBrowser unBrowserHandle, bool bBackgroundMode)
        {
            SteamEmulator.SteamHTMLSurface.SetBackgroundMode(unBrowserHandle, bBackgroundMode);
        }

        public void SetDPIScalingFactor(IntPtr _, HHTMLBrowser unBrowserHandle, float flDPIScaling)
        {
            SteamEmulator.SteamHTMLSurface.SetDPIScalingFactor(unBrowserHandle, flDPIScaling);
        }

        public void AllowStartRequest(IntPtr _, HHTMLBrowser unBrowserHandle, bool bAllowed)
        {
            SteamEmulator.SteamHTMLSurface.AllowStartRequest(unBrowserHandle, bAllowed);
        }

        public void JSDialogResponse(IntPtr _, HHTMLBrowser unBrowserHandle, bool bResult)
        {
            SteamEmulator.SteamHTMLSurface.JSDialogResponse(unBrowserHandle, bResult);
        }

        public void FileLoadDialogResponse(IntPtr _, HHTMLBrowser unBrowserHandle, string pchSelectedFiles)
        {
            SteamEmulator.SteamHTMLSurface.FileLoadDialogResponse(unBrowserHandle, pchSelectedFiles);
        }
    }
}
