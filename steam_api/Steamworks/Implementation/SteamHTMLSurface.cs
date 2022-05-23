using System;

using HHTMLBrowser = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamHTMLSurface : ISteamInterface
    {
        public static SteamHTMLSurface Instance;

        public SteamHTMLSurface()
        {
            Instance = this;
            InterfaceName = "SteamHTMLSurface";
            InterfaceVersion = "STEAMHTMLSURFACE_INTERFACE_VERSION_005";
        }

        public void AddHeader(HHTMLBrowser unBrowserHandle, string pchKey, string pchValue)
        {
            Write("AddHeader");
        }

        public void AllowStartRequest(HHTMLBrowser unBrowserHandle, bool bAllowed)
        {
            Write("AllowStartRequest");
        }

        public void CopyToClipboard(HHTMLBrowser unBrowserHandle)
        {
            Write("CopyToClipboard");
        }

        public ulong CreateBrowser(string pchUserAgent, string pchUserCSS)
        {
            Write("CreateBrowser");
            return 0;
        }

        public void ExecuteJavascript(HHTMLBrowser unBrowserHandle, string pchScript)
        {
            Write("ExecuteJavascript");
        }

        public void FileLoadDialogResponse(HHTMLBrowser unBrowserHandle, string pchSelectedFiles)
        {
            Write("FileLoadDialogResponse");
        }

        public void Find(HHTMLBrowser unBrowserHandle, string pchSearchStr, bool bCurrentlyInFind, bool bReverse)
        {
            Write("Find");
        }

        public void GetLinkAtPosition(HHTMLBrowser unBrowserHandle, int x, int y)
        {
            Write("GetLinkAtPosition");
        }

        public void GoBack(HHTMLBrowser unBrowserHandle)
        {
            Write("GoBack");
        }

        public void GoForward(HHTMLBrowser unBrowserHandle)
        {
            Write("GoForward");
        }

        public bool Init()
        {
            Write("Init");
            return true;
        }

        public void JSDialogResponse(HHTMLBrowser unBrowserHandle, bool bResult)
        {
            Write("JSDialogResponse");
        }

        public void KeyDown(HHTMLBrowser unBrowserHandle, uint nNativeKeyCode, int IntPtr, bool bIsSystemKey = false)
        {
            Write("KeyDown");
        }

        public void Keystring(HHTMLBrowser unBrowserHandle, int cUnicodestring, IntPtr IntPtr)
        {
            Write("Keystring");
        }

        public void KeyUp(HHTMLBrowser unBrowserHandle, uint nNativeKeyCode, int eHTMLKeyModifiers)
        {
            Write("KeyUp");
        }

        public void KeyChar(HHTMLBrowser unBrowserHandle, uint cUnicodeChar, int eHTMLKeyModifiers)
        {
            Write("KeyChar");
        }

        public void LoadURL(HHTMLBrowser unBrowserHandle, string pchURL, string pchPostData)
        {
            Write("LoadURL");
        }

        public void MouseDoubleClick(HHTMLBrowser unBrowserHandle, int eMouseButton)
        {
            Write("MouseDoubleClick");
        }

        public void MouseDown(HHTMLBrowser unBrowserHandle, int eMouseButton)
        {
            Write("MouseDown");
        }

        public void MouseMove(HHTMLBrowser unBrowserHandle, int x, int y)
        {
            Write("MouseMove");
        }

        public void MouseUp(HHTMLBrowser unBrowserHandle, int eMouseButton)
        {
            Write("MouseUp");
        }

        public void MouseWheel(HHTMLBrowser unBrowserHandle, int nDelta)
        {
            Write("MouseWheel");
        }

        public void OpenDeveloperTools(HHTMLBrowser unBrowserHandle)
        {
            Write("OpenDeveloperTools");
        }

        public void PasteFromClipboard(HHTMLBrowser unBrowserHandle)
        {
            Write("PasteFromClipboard");
        }

        public void Reload(HHTMLBrowser unBrowserHandle)
        {
            Write("Reload");
        }

        public void RemoveBrowser(HHTMLBrowser unBrowserHandle)
        {
            Write("RemoveBrowser");
        }

        public void SetBackgroundMode(HHTMLBrowser unBrowserHandle, bool bBackgroundMode)
        {
            Write("SetBackgroundMode");
        }

        public void SetCookie(string pchHostname, string pchKey, string pchValue, string pchPath, uint nExpires, bool bSecure = false, bool bHTTPOnly = false)
        {
            Write("SetCookie");
        }

        public void SetDPIScalingFactor(HHTMLBrowser unBrowserHandle, float flDPIScaling)
        {
            Write("SetDPIScalingFactor");
        }

        public void SetHorizontalScroll(HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll)
        {
            Write("SetHorizontalScroll");
        }

        public void SetKeyFocus(HHTMLBrowser unBrowserHandle, bool bHasKeyFocus)
        {
            Write("SetKeyFocus");
        }

        public void SetPageScaleFactor(HHTMLBrowser unBrowserHandle, float flZoom, int nPointX, int nPointY)
        {
            Write("SetPageScaleFactor");
        }

        public void SetSize(HHTMLBrowser unBrowserHandle, uint unWidth, uint unHeight)
        {
            Write("SetSize");
        }

        public void SetVerticalScroll(HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll)
        {
            Write("SetVerticalScroll");
        }

        public bool Shutdown()
        {
            Write("Shutdown");
            return true;
        }

        public void StopFind(HHTMLBrowser unBrowserHandle)
        {
            Write("StopFind");
        }

        public void StopLoad(HHTMLBrowser unBrowserHandle)
        {
            Write("StopLoad");
        }

        public void ViewSource(HHTMLBrowser unBrowserHandle)
        {
            Write("ViewSource");
        }
    }
}