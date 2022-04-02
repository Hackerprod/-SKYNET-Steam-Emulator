using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Steamworks;
using Steamworks;

public class SteamHTMLSurface : SteamInterface
{
    public void AddHeader(IntPtr unBrowserHandle, string pchKey, string pchValue)
    {
        Write("AddHeader");
    }

    public void AllowStartRequest(IntPtr unBrowserHandle, bool bAllowed)
    {
        Write("AllowStartRequest");
    }

    public void CopyToClipboard(IntPtr unBrowserHandle)
    {
        Write("CopyToClipboard");
    }

    public SteamAPICall_t CreateBrowser(string pchUserAgent, string pchUserCSS)
    {
        Write("CreateBrowser");
        return SteamAPICall_t.Invalid;
    }

    public void ExecuteJavascript(IntPtr unBrowserHandle, string pchScript)
    {
        Write("ExecuteJavascript");
    }

    public void FileLoadDialogResponse(IntPtr unBrowserHandle, string pchSelectedFiles)
    {
        Write("FileLoadDialogResponse");
    }

    public void Find(IntPtr unBrowserHandle, string pchSearchStr, bool bCurrentlyInFind, bool bReverse)
    {
        Write("Find");
    }

    public void GetLinkAtPosition(IntPtr unBrowserHandle, int x, int y)
    {
        Write("GetLinkAtPosition");
    }

    public void GoBack(IntPtr unBrowserHandle)
    {
        Write("GoBack");
    }

    public void GoForward(IntPtr unBrowserHandle)
    {
        Write("GoForward");
    }

    public bool Init(IntPtr _)
    {
        Write("Init");
        return true;
    }

    public void JSDialogResponse(IntPtr unBrowserHandle, bool bResult)
    {
        Write("JSDialogResponse");
    }

    public void KeyDown(IntPtr unBrowserHandle, int nNativeKeyCode, IntPtr IntPtr, bool bIsSystemKey = false)
    {
        Write("KeyDown");
    }

    public void Keystring(IntPtr unBrowserHandle, int cUnicodestring, IntPtr IntPtr)
    {
        Write("Keystring");
    }

    public void KeyUp(IntPtr unBrowserHandle, int nNativeKeyCode, IntPtr IntPtr)
    {
        Write("KeyUp");
    }

    public void LoadURL(IntPtr unBrowserHandle, string pchURL, string pchPostData)
    {
        Write("LoadURL");
    }

    public void MouseDoubleClick(IntPtr unBrowserHandle, EHTMLMouseButton eMouseButton)
    {
        Write("MouseDoubleClick");
    }

    public void MouseDown(IntPtr unBrowserHandle, EHTMLMouseButton eMouseButton)
    {
        Write("MouseDown");
    }

    public void MouseMove(IntPtr unBrowserHandle, int x, int y)
    {
        Write("MouseMove");
    }

    public void MouseUp(IntPtr unBrowserHandle, EHTMLMouseButton eMouseButton)
    {
        Write("MouseUp");
    }

    public void MouseWheel(IntPtr unBrowserHandle, int nDelta)
    {
        Write("MouseWheel");
    }

    public void OpenDeveloperTools(IntPtr unBrowserHandle)
    {
        Write("OpenDeveloperTools");
    }

    public void PasteFromClipboard(IntPtr unBrowserHandle)
    {
        Write("PasteFromClipboard");
    }

    public void Reload(IntPtr unBrowserHandle)
    {
        Write("Reload");
    }

    public void RemoveBrowser(IntPtr unBrowserHandle)
    {
        Write("RemoveBrowser");
    }

    public void SetBackgroundMode(IntPtr unBrowserHandle, bool bBackgroundMode)
    {
        Write("SetBackgroundMode");
    }

    public void SetCookie(string pchHostname, string pchKey, string pchValue, string pchPath, IntPtr nExpires, bool bSecure = false, bool bHTTPOnly = false)
    {
        Write("SetCookie");
    }

    public void SetDPIScalingFactor(IntPtr unBrowserHandle, float flDPIScaling)
    {
        Write("SetDPIScalingFactor");
    }

    public void SetHorizontalScroll(IntPtr unBrowserHandle, int nAbsolutePixelScroll)
    {
        Write("SetHorizontalScroll");
    }

    public void SetKeyFocus(IntPtr unBrowserHandle, bool bHasKeyFocus)
    {
        Write("SetKeyFocus");
    }

    public void SetPageScaleFactor(IntPtr unBrowserHandle, float flZoom, int nPointX, int nPointY)
    {
        Write("SetPageScaleFactor");
    }

    public void SetSize(IntPtr unBrowserHandle, int unWidth, int unHeight)
    {
        Write("SetSize");
    }

    public void SetVerticalScroll(IntPtr unBrowserHandle, int nAbsolutePixelScroll)
    {
        Write("SetVerticalScroll");
    }

    public bool Shutdown(IntPtr _)
    {
        Write("Shutdown");
        return true;
    }

    public void StopFind(IntPtr unBrowserHandle)
    {
        Write("StopFind");
    }

    public void StopLoad(IntPtr unBrowserHandle)
    {
        Write("StopLoad");
    }

    public void ViewSource(IntPtr unBrowserHandle)
    {
        Write("ViewSource");
    }

    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}