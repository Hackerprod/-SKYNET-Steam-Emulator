using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    [StructLayout(LayoutKind.Sequential)]
    public class SteamHTMLSurface : ISteamInterface
    {
        public void AddHeader(uint unBrowserHandle, string pchKey, string pchValue)
        {
            Write("AddHeader");
        }

        public void AllowStartRequest(uint unBrowserHandle, bool bAllowed)
        {
            Write("AllowStartRequest");
        }

        public void CopyToClipboard(uint unBrowserHandle)
        {
            Write("CopyToClipboard");
        }

        public ulong CreateBrowser(string pchUserAgent, string pchUserCSS)
        {
            Write("CreateBrowser");
            return 0;
        }

        public void ExecuteJavascript(uint unBrowserHandle, string pchScript)
        {
            Write("ExecuteJavascript");
        }

        public void FileLoadDialogResponse(uint unBrowserHandle, string pchSelectedFiles)
        {
            Write("FileLoadDialogResponse");
        }

        public void Find(uint unBrowserHandle, string pchSearchStr, bool bCurrentlyInFind, bool bReverse)
        {
            Write("Find");
        }

        public void GetLinkAtPosition(uint unBrowserHandle, int x, int y)
        {
            Write("GetLinkAtPosition");
        }

        public void GoBack(uint unBrowserHandle)
        {
            Write("GoBack");
        }

        public void GoForward(uint unBrowserHandle)
        {
            Write("GoForward");
        }

        public bool Init(IntPtr _)
        {
            Write("Init");
            return true;
        }

        public void JSDialogResponse(uint unBrowserHandle, bool bResult)
        {
            Write("JSDialogResponse");
        }

        public void KeyDown(uint unBrowserHandle, uint nNativeKeyCode, int IntPtr, bool bIsSystemKey = false)
        {
            Write("KeyDown");
        }

        public void Keystring(uint unBrowserHandle, int cUnicodestring, IntPtr IntPtr)
        {
            Write("Keystring");
        }

        public void KeyUp(uint unBrowserHandle, uint nNativeKeyCode, int eHTMLKeyModifiers)
        {
            Write("KeyUp");
        }

        public void KeyChar(uint unBrowserHandle, uint cUnicodeChar, int eHTMLKeyModifiers)
        {
            Write("KeyChar");
        }

        public void LoadURL(uint unBrowserHandle, string pchURL, string pchPostData)
        {
            Write("LoadURL");
        }

        public void MouseDoubleClick(uint unBrowserHandle, int eMouseButton)
        {
            Write("MouseDoubleClick");
        }

        public void MouseDown(uint unBrowserHandle, int eMouseButton)
        {
            Write("MouseDown");
        }

        public void MouseMove(uint unBrowserHandle, int x, int y)
        {
            Write("MouseMove");
        }

        public void MouseUp(uint unBrowserHandle, int eMouseButton)
        {
            Write("MouseUp");
        }

        public void MouseWheel(uint unBrowserHandle, int nDelta)
        {
            Write("MouseWheel");
        }

        public void OpenDeveloperTools(uint unBrowserHandle)
        {
            Write("OpenDeveloperTools");
        }

        public void PasteFromClipboard(uint unBrowserHandle)
        {
            Write("PasteFromClipboard");
        }

        public void Reload(uint unBrowserHandle)
        {
            Write("Reload");
        }

        public void RemoveBrowser(uint unBrowserHandle)
        {
            Write("RemoveBrowser");
        }

        public void SetBackgroundMode(uint unBrowserHandle, bool bBackgroundMode)
        {
            Write("SetBackgroundMode");
        }

        public void SetCookie(string pchHostname, string pchKey, string pchValue, string pchPath, uint nExpires, bool bSecure = false, bool bHTTPOnly = false)
        {
            Write("SetCookie");
        }

        public void SetDPIScalingFactor(uint unBrowserHandle, float flDPIScaling)
        {
            Write("SetDPIScalingFactor");
        }

        public void SetHorizontalScroll(uint unBrowserHandle, uint nAbsolutePixelScroll)
        {
            Write("SetHorizontalScroll");
        }

        public void SetKeyFocus(uint unBrowserHandle, bool bHasKeyFocus)
        {
            Write("SetKeyFocus");
        }

        public void SetPageScaleFactor(uint unBrowserHandle, float flZoom, int nPointX, int nPointY)
        {
            Write("SetPageScaleFactor");
        }

        public void SetSize(uint unBrowserHandle, uint unWidth, uint unHeight)
        {
            Write("SetSize");
        }

        public void SetVerticalScroll(uint unBrowserHandle, uint nAbsolutePixelScroll)
        {
            Write("SetVerticalScroll");
        }

        public bool Shutdown(IntPtr _)
        {
            Write("Shutdown");
            return true;
        }

        public void StopFind(uint unBrowserHandle)
        {
            Write("StopFind");
        }

        public void StopLoad(uint unBrowserHandle)
        {
            Write("StopLoad");
        }

        public void ViewSource(uint unBrowserHandle)
        {
            Write("ViewSource");
        }

        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamHTMLSurface()
        {
            InterfaceVersion = "SteamHTMLSurface";
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}