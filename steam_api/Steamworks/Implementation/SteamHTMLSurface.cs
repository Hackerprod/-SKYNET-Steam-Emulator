using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SKYNET.Managers;
using SKYNET.Steamworks.Interfaces;

using HHTMLBrowser = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public sealed class SteamHTMLSurface : ISteamInterface
    {
        private readonly object gate = new object();
        private HtmlSurfaceHost host;

        public static SteamHTMLSurface Instance;

        public SteamHTMLSurface()
        {
            Instance = this;
            InterfaceName = "SteamHTMLSurface";
            InterfaceVersion = "STEAMHTMLSURFACE_INTERFACE_VERSION_005";
        }

        public bool Init()
        {
            lock (gate)
            {
                if (host == null)
                {
                    host = new HtmlSurfaceHost();
                }
                return host.Start();
            }
        }

        public bool Shutdown()
        {
            HtmlSurfaceHost current;
            lock (gate)
            {
                current = host;
                host = null;
            }
            current?.Dispose();
            return true;
        }

        public ulong CreateBrowser(string pchUserAgent, string pchUserCSS)
        {
            return GetHost()?.CreateBrowser(pchUserAgent, pchUserCSS) ?? 0;
        }

        public void RemoveBrowser(HHTMLBrowser unBrowserHandle)
        {
            GetHost()?.RemoveBrowser(unBrowserHandle);
        }

        public void LoadURL(HHTMLBrowser unBrowserHandle, string pchURL, string pchPostData)
        {
            GetHost()?.LoadUrl(unBrowserHandle, pchURL, pchPostData);
        }

        public void AllowStartRequest(HHTMLBrowser unBrowserHandle, bool bAllowed)
        {
            GetHost()?.AllowStartRequest(unBrowserHandle, bAllowed);
        }

        public void AddHeader(HHTMLBrowser unBrowserHandle, string pchKey, string pchValue)
        {
            GetHost()?.AddHeader(unBrowserHandle, pchKey, pchValue);
        }

        public void SetSize(HHTMLBrowser unBrowserHandle, uint unWidth, uint unHeight)
        {
            GetHost()?.SetSize(unBrowserHandle, unWidth, unHeight);
        }

        public void SetDPIScalingFactor(HHTMLBrowser unBrowserHandle, float flDPIScaling)
        {
            GetHost()?.SetDpiScaling(unBrowserHandle, flDPIScaling);
        }

        public void SetPageScaleFactor(HHTMLBrowser unBrowserHandle, float flZoom, int nPointX, int nPointY)
        {
            GetHost()?.SetPageScale(unBrowserHandle, flZoom);
        }

        public void SetBackgroundMode(HHTMLBrowser unBrowserHandle, bool bBackgroundMode)
        {
            GetHost()?.SetBackgroundMode(unBrowserHandle, bBackgroundMode);
        }

        public void ExecuteJavascript(HHTMLBrowser unBrowserHandle, string pchScript)
        {
            GetHost()?.ExecuteJavaScript(unBrowserHandle, pchScript);
        }

        public void Reload(HHTMLBrowser unBrowserHandle)
        {
            GetHost()?.Reload(unBrowserHandle);
        }

        public void StopLoad(HHTMLBrowser unBrowserHandle)
        {
            GetHost()?.StopLoad(unBrowserHandle);
        }

        public void GoBack(HHTMLBrowser unBrowserHandle)
        {
            GetHost()?.GoBack(unBrowserHandle);
        }

        public void GoForward(HHTMLBrowser unBrowserHandle)
        {
            GetHost()?.GoForward(unBrowserHandle);
        }

        public void SetHorizontalScroll(HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll)
        {
            GetHost()?.SetHorizontalScroll(unBrowserHandle, nAbsolutePixelScroll);
        }

        public void SetVerticalScroll(HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll)
        {
            GetHost()?.SetVerticalScroll(unBrowserHandle, nAbsolutePixelScroll);
        }

        public void SetKeyFocus(HHTMLBrowser unBrowserHandle, bool bHasKeyFocus)
        {
            GetHost()?.SetKeyFocus(unBrowserHandle, bHasKeyFocus);
        }

        public void KeyDown(HHTMLBrowser unBrowserHandle, uint nNativeKeyCode, int eHTMLKeyModifiers, bool bIsSystemKey = false)
        {
            GetHost()?.SendKey(unBrowserHandle, 0x0100, nNativeKeyCode, bIsSystemKey);
        }

        public void KeyUp(HHTMLBrowser unBrowserHandle, uint nNativeKeyCode, int eHTMLKeyModifiers)
        {
            GetHost()?.SendKey(unBrowserHandle, 0x0101, nNativeKeyCode, false);
        }

        public void KeyChar(HHTMLBrowser unBrowserHandle, uint cUnicodeChar, int eHTMLKeyModifiers)
        {
            GetHost()?.SendKey(unBrowserHandle, 0x0102, cUnicodeChar, false);
        }

        public void Keystring(HHTMLBrowser unBrowserHandle, int cUnicodestring, IntPtr pchString)
        {
            if (pchString == IntPtr.Zero || cUnicodestring <= 0)
            {
                return;
            }
            var text = Marshal.PtrToStringUni(pchString, cUnicodestring) ?? string.Empty;
            foreach (var character in text)
            {
                KeyChar(unBrowserHandle, character, 0);
            }
        }

        public void MouseMove(HHTMLBrowser unBrowserHandle, int x, int y)
        {
            GetHost()?.SendMouseMove(unBrowserHandle, x, y);
        }

        public void MouseDown(HHTMLBrowser unBrowserHandle, int eMouseButton)
        {
            GetHost()?.SendMouseButton(unBrowserHandle, eMouseButton, true, false);
        }

        public void MouseUp(HHTMLBrowser unBrowserHandle, int eMouseButton)
        {
            GetHost()?.SendMouseButton(unBrowserHandle, eMouseButton, false, false);
        }

        public void MouseDoubleClick(HHTMLBrowser unBrowserHandle, int eMouseButton)
        {
            GetHost()?.SendMouseButton(unBrowserHandle, eMouseButton, true, true);
        }

        public void MouseWheel(HHTMLBrowser unBrowserHandle, int nDelta)
        {
            GetHost()?.SendMouseWheel(unBrowserHandle, nDelta);
        }

        public void CopyToClipboard(HHTMLBrowser unBrowserHandle)
        {
            GetHost()?.CopyToClipboard(unBrowserHandle);
        }

        public void PasteFromClipboard(HHTMLBrowser unBrowserHandle)
        {
            GetHost()?.PasteFromClipboard(unBrowserHandle);
        }

        public void Find(HHTMLBrowser unBrowserHandle, string pchSearchStr, bool bCurrentlyInFind, bool bReverse)
        {
            GetHost()?.Find(unBrowserHandle, pchSearchStr, bReverse);
        }

        public void StopFind(HHTMLBrowser unBrowserHandle)
        {
            GetHost()?.Find(unBrowserHandle, string.Empty, false);
        }

        public void GetLinkAtPosition(HHTMLBrowser unBrowserHandle, int x, int y)
        {
            GetHost()?.GetLinkAtPosition(unBrowserHandle, x, y);
        }

        public void JSDialogResponse(HHTMLBrowser unBrowserHandle, bool bResult)
        {
            GetHost()?.JavaScriptDialogResponse(unBrowserHandle, bResult);
        }

        public void FileLoadDialogResponse(HHTMLBrowser unBrowserHandle, string pchSelectedFiles)
        {
            GetHost()?.FileDialogResponse(
                unBrowserHandle,
                string.IsNullOrWhiteSpace(pchSelectedFiles)
                    ? Array.Empty<string>()
                    : new[] { pchSelectedFiles });
        }

        public void FileLoadDialogResponse(HHTMLBrowser unBrowserHandle, IntPtr pchSelectedFiles)
        {
            const int maximumFiles = 256;
            const int maximumPathBytes = 32768;
            var files = new List<string>();
            if (pchSelectedFiles != IntPtr.Zero)
            {
                for (var index = 0; index < maximumFiles; index++)
                {
                    var pathPointer = Marshal.ReadIntPtr(
                        pchSelectedFiles,
                        checked(index * IntPtr.Size));
                    if (pathPointer == IntPtr.Zero)
                    {
                        break;
                    }

                    var path = ReadUtf8String(pathPointer, maximumPathBytes);
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        files.Add(path);
                    }
                }
            }
            GetHost()?.FileDialogResponse(unBrowserHandle, files);
        }

        public void OpenDeveloperTools(HHTMLBrowser unBrowserHandle)
        {
        }

        public void ViewSource(HHTMLBrowser unBrowserHandle)
        {
        }

        public void SetCookie(string pchHostname, string pchKey, string pchValue, string pchPath, uint nExpires, bool bSecure = false, bool bHTTPOnly = false)
        {
            if (string.IsNullOrWhiteSpace(pchHostname) || string.IsNullOrWhiteSpace(pchKey))
            {
                return;
            }
            var scheme = bSecure ? "https" : "http";
            var hostName = pchHostname.Contains("://") ? pchHostname : $"{scheme}://{pchHostname}";
            try
            {
                var uri = new Uri(hostName);
                var cookie = $"{pchKey}={Uri.EscapeDataString(pchValue ?? string.Empty)}; path={(string.IsNullOrEmpty(pchPath) ? "/" : pchPath)}";
                if (nExpires != 0)
                {
                    cookie += $"; expires={DateTimeOffset.FromUnixTimeSeconds(nExpires).UtcDateTime:R}";
                }
                if (bSecure)
                {
                    cookie += "; secure";
                }
                InternetSetCookieEx(uri.AbsoluteUri, null, cookie, bHTTPOnly ? 0x00002000 : 0, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("HTML cookie", ex);
            }
        }

        private HtmlSurfaceHost GetHost()
        {
            if (!Init())
            {
                return null;
            }
            lock (gate)
            {
                return host;
            }
        }

        private static string ReadUtf8String(IntPtr source, int maximumBytes)
        {
            var length = 0;
            while (length < maximumBytes && Marshal.ReadByte(source, length) != 0)
            {
                length++;
            }
            if (length == 0)
            {
                return string.Empty;
            }

            var bytes = new byte[length];
            Marshal.Copy(source, bytes, 0, length);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        [DllImport("wininet.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint InternetSetCookieEx(
            string url,
            string cookieName,
            string cookieData,
            int flags,
            IntPtr reserved);
    }
}
