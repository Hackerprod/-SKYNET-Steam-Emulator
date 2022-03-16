using SKYNET.Interface;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate("SteamHTMLSurface")]
    public class DSteamHTMLSurface : IBaseInterfaceMap
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool Init();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool Shutdown();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate SteamAPICall_t CreateBrowser(string pchUserAgent, string pchUserCSS);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void RemoveBrowser(IntPtr unBrowserHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void LoadURL(IntPtr unBrowserHandle, string pchURL, string pchPostData);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetSize(IntPtr unBrowserHandle, int unWidth, int unHeight);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void StopLoad(IntPtr unBrowserHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void Reload(IntPtr unBrowserHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void GoBack(IntPtr unBrowserHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void GoForward(IntPtr unBrowserHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void AddHeader(IntPtr unBrowserHandle, string pchKey, string pchValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ExecuteJavascript(IntPtr unBrowserHandle, string pchScript);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void MouseUp(IntPtr unBrowserHandle, EHTMLMouseButton eMouseButton);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void MouseDown(IntPtr unBrowserHandle, EHTMLMouseButton eMouseButton);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void MouseDoubleClick(IntPtr unBrowserHandle, EHTMLMouseButton eMouseButton);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void MouseMove(IntPtr unBrowserHandle, int x, int y);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void MouseWheel(IntPtr unBrowserHandle, int nDelta);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void KeyDown(IntPtr unBrowserHandle, int nNativeKeyCode, IntPtr IntPtr, bool bIsSystemKey = false);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void KeyUp(IntPtr unBrowserHandle, int nNativeKeyCode, IntPtr IntPtr);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void Keystring(IntPtr unBrowserHandle, int cUnicodestring, IntPtr IntPtr);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetHorizontalScroll(IntPtr unBrowserHandle, int nAbsolutePixelScroll);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetVerticalScroll(IntPtr unBrowserHandle, int nAbsolutePixelScroll);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetKeyFocus(IntPtr unBrowserHandle, bool bHasKeyFocus);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ViewSource(IntPtr unBrowserHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void CopyToClipboard(IntPtr unBrowserHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void PasteFromClipboard(IntPtr unBrowserHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void Find(IntPtr unBrowserHandle, string pchSearchStr, bool bCurrentlyInFind, bool bReverse);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void StopFind(IntPtr unBrowserHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void GetLinkAtPosition(IntPtr unBrowserHandle, int x, int y);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetCookie(string pchHostname, string pchKey, string pchValue, string pchPath, IntPtr nExpires, bool bSecure = false, bool bHTTPOnly = false);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetPageScaleFactor(IntPtr unBrowserHandle, float flZoom, int nPointX, int nPointY);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetBackgroundMode(IntPtr unBrowserHandle, bool bBackgroundMode);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetDPIScalingFactor(IntPtr unBrowserHandle, float flDPIScaling);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void OpenDeveloperTools(IntPtr unBrowserHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void AllowStartRequest(IntPtr unBrowserHandle, bool bAllowed);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void JSDialogResponse(IntPtr unBrowserHandle, bool bResult);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void FileLoadDialogResponse(IntPtr unBrowserHandle, string pchSelectedFiles);

    }
}
