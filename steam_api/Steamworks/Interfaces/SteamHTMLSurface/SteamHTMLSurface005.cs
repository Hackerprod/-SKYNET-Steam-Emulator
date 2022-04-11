using System;


namespace SKYNET.Interface
{
    [Interface("STEAMHTMLSURFACE_INTERFACE_VERSION_005")]
    public class SteamHTMLSurface005 : ISteamInterface
    {
        //        public ~ISteamHTMLSurface(IntPtr _) {}
        //{
        //    return  SteamEmulator.SteamHTMLSurface.ISteamHTMLSurface(_);
        //}

        public bool Init(IntPtr _)
        {
            return SteamEmulator.SteamHTMLSurface.Init(_);
        }

        public bool Shutdown(IntPtr _)
        {
            return SteamEmulator.SteamHTMLSurface.Shutdown(_);
        }

        public ulong CreateBrowser(IntPtr _, string pchUserAgent, string pchUserCSS)
        {
            return SteamEmulator.SteamHTMLSurface.CreateBrowser(pchUserAgent, pchUserCSS);
        }

        public void RemoveBrowser(IntPtr _, uint unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.RemoveBrowser(unBrowserHandle);
        }

        public void LoadURL(IntPtr _, uint unBrowserHandle, string pchURL, string pchPostData)
        {
            SteamEmulator.SteamHTMLSurface.LoadURL(unBrowserHandle, pchURL, pchPostData);
        }

        public void SetSize(IntPtr _, uint unBrowserHandle, uint unWidth, uint unHeight)
        {
            SteamEmulator.SteamHTMLSurface.SetSize(unBrowserHandle, unWidth, unHeight);
        }

        public void StopLoad(IntPtr _, uint unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.StopLoad(unBrowserHandle);
        }

        public void Reload(IntPtr _, uint unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.Reload(unBrowserHandle);
        }

        public void GoBack(IntPtr _, uint unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.GoBack(unBrowserHandle);
        }

        public void GoForward(IntPtr _, uint unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.GoForward(unBrowserHandle);
        }

        public void AddHeader(IntPtr _, uint unBrowserHandle, string pchKey, string pchValue)
        {
            SteamEmulator.SteamHTMLSurface.AddHeader(unBrowserHandle, pchKey, pchValue);
        }

        public void ExecuteJavascript(IntPtr _, uint unBrowserHandle, string pchScript)
        {
            SteamEmulator.SteamHTMLSurface.ExecuteJavascript(unBrowserHandle, pchScript);
        }

        public void MouseUp(IntPtr _, uint unBrowserHandle, int eMouseButton)
        {
            SteamEmulator.SteamHTMLSurface.MouseUp(unBrowserHandle, eMouseButton);
        }

        public void MouseDown(IntPtr _, uint unBrowserHandle, int eMouseButton)
        {
            SteamEmulator.SteamHTMLSurface.MouseDown(unBrowserHandle, eMouseButton);
        }

        public void MouseDoubleClick(IntPtr _, uint unBrowserHandle, int eMouseButton)
        {
            SteamEmulator.SteamHTMLSurface.MouseDoubleClick(unBrowserHandle, eMouseButton);
        }

        public void MouseMove(IntPtr _, uint unBrowserHandle, int x, int y)
        {
            SteamEmulator.SteamHTMLSurface.MouseMove(unBrowserHandle, x, y);
        }

        public void MouseWheel(IntPtr _, uint unBrowserHandle, int nDelta)
        {
            SteamEmulator.SteamHTMLSurface.MouseWheel(unBrowserHandle, nDelta);
        }

        public void KeyDown(IntPtr _, uint unBrowserHandle, uint nNativeKeyCode, int eHTMLKeyModifiers, bool bIsSystemKey = false)
        {
            SteamEmulator.SteamHTMLSurface.KeyDown(unBrowserHandle, nNativeKeyCode, eHTMLKeyModifiers, false);
        }

        public void KeyUp(IntPtr _, uint unBrowserHandle, uint nNativeKeyCode, int eHTMLKeyModifiers)
        {
            SteamEmulator.SteamHTMLSurface.KeyUp(unBrowserHandle, nNativeKeyCode, eHTMLKeyModifiers);
        }

        public void KeyChar(IntPtr _, uint unBrowserHandle, uint cUnicodeChar, int eHTMLKeyModifiers)
        {
            SteamEmulator.SteamHTMLSurface.KeyChar(unBrowserHandle, cUnicodeChar, eHTMLKeyModifiers);
        }

        public void SetHorizontalScroll(IntPtr _, uint unBrowserHandle, uint nAbsolutePixelScroll)
        {
            SteamEmulator.SteamHTMLSurface.SetHorizontalScroll(unBrowserHandle, nAbsolutePixelScroll);
        }

        public void SetVerticalScroll(IntPtr _, uint unBrowserHandle, uint nAbsolutePixelScroll)
        {
            SteamEmulator.SteamHTMLSurface.SetVerticalScroll(unBrowserHandle, nAbsolutePixelScroll);
        }

        public void SetKeyFocus(IntPtr _, uint unBrowserHandle, bool bHasKeyFocus)
        {
            SteamEmulator.SteamHTMLSurface.SetKeyFocus(unBrowserHandle, bHasKeyFocus);
        }

        public void ViewSource(IntPtr _, uint unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.ViewSource(unBrowserHandle);
        }

        public void CopyToClipboard(IntPtr _, uint unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.CopyToClipboard(unBrowserHandle);
        }

        public void PasteFromClipboard(IntPtr _, uint unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.PasteFromClipboard(unBrowserHandle);
        }

        public void Find(IntPtr _, uint unBrowserHandle, string pchSearchStr, bool bCurrentlyInFind, bool bReverse)
        {
            SteamEmulator.SteamHTMLSurface.Find(unBrowserHandle, pchSearchStr, bCurrentlyInFind, bReverse);
        }

        public void StopFind(IntPtr _, uint unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.StopFind(unBrowserHandle);
        }

        public void GetLinkAtPosition(IntPtr _, uint unBrowserHandle, int x, int y)
        {
            SteamEmulator.SteamHTMLSurface.GetLinkAtPosition(unBrowserHandle, x, y);
        }

        public void SetCookie(IntPtr _, string pchHostname, string pchKey, string pchValue, string pchPath = "/", uint nExpires = 0, bool bSecure = false, bool bHTTPOnly = false)
        {
            SteamEmulator.SteamHTMLSurface.SetCookie(pchHostname, pchKey, pchValue, pchPath, nExpires, bSecure, bHTTPOnly);
        }

        public void SetPageScaleFactor(IntPtr _, uint unBrowserHandle, float flZoom, int nPointX, int nPointY)
        {
            SteamEmulator.SteamHTMLSurface.SetPageScaleFactor(unBrowserHandle, flZoom, nPointX, nPointY);
        }

        public void SetBackgroundMode(IntPtr _, uint unBrowserHandle, bool bBackgroundMode)
        {
            SteamEmulator.SteamHTMLSurface.SetBackgroundMode(unBrowserHandle, bBackgroundMode);
        }

        public void SetDPIScalingFactor(IntPtr _, uint unBrowserHandle, float flDPIScaling)
        {
            SteamEmulator.SteamHTMLSurface.SetDPIScalingFactor(unBrowserHandle, flDPIScaling);
        }

        public void OpenDeveloperTools(IntPtr _, uint unBrowserHandle)
        {
            SteamEmulator.SteamHTMLSurface.OpenDeveloperTools(unBrowserHandle);
        }

        public void AllowStartRequest(IntPtr _, uint unBrowserHandle, bool bAllowed)
        {
            SteamEmulator.SteamHTMLSurface.AllowStartRequest(unBrowserHandle, bAllowed);
        }

        public void JSDialogResponse(IntPtr _, uint unBrowserHandle, bool bResult)
        {
            SteamEmulator.SteamHTMLSurface.JSDialogResponse(unBrowserHandle, bResult);
        }

        public void FileLoadDialogResponse(IntPtr _, uint unBrowserHandle, string pchSelectedFiles)
        {
            SteamEmulator.SteamHTMLSurface.FileLoadDialogResponse(unBrowserHandle, pchSelectedFiles);
        }
    }
}
