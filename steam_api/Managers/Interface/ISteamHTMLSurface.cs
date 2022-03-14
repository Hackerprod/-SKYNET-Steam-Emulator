using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamHTMLSurface
    {
        bool Init();
        bool Shutdown();

        // Create a browser object for display of a html page, when creation is complete the call handle
        // will return a HTML_BrowserReady_t callback for the IntPtr of your new browser.
        //   The user agent string is a substring to be added to the general user agent string so you can
        // identify your client on web servers.
        //   The userCSS string lets you apply a CSS style sheet to every displayed page, leave null if
        // you do not require this functionality.
        //
        // YOU MUST HAVE IMPLEMENTED HANDLERS FOR HTML_BrowserReady_t, HTML_StartRequest_t,
        // HTML_JSAlert_t, HTML_JSConfirm_t, and HTML_FileOpenDialog_t! See the CALLBACKS
        // section of this interface (AllowStartRequest, etc) for more details. If you do
        // not implement these callback handlers, the browser may appear to hang instead of
        // navigating to new pages or triggering javascript popups.
        //

        SteamAPICall_t CreateBrowser(string pchUserAgent, string pchUserCSS);

        // Call this when you are done with a html surface, this lets us free the resources being used by it
        void RemoveBrowser(IntPtr unBrowserHandle);

        // Navigate to this URL, results in a HTML_StartRequest_t as the request commences 
        void LoadURL(IntPtr unBrowserHandle, string pchURL, string pchPostData);

        // Tells the surface the size in pixels to display the surface
        void SetSize(IntPtr unBrowserHandle, int unWidth, int unHeight);

        // Stop the load of the current html page
        void StopLoad(IntPtr unBrowserHandle);
        // Reload (most likely from local cache) the current page
        void Reload(IntPtr unBrowserHandle);
        // navigate back in the page history
        void GoBack(IntPtr unBrowserHandle);
        // navigate forward in the page history
        void GoForward(IntPtr unBrowserHandle);

        // add this header to any url requests from this browser
        void AddHeader(IntPtr unBrowserHandle, string pchKey, string pchValue);
        // run this javascript script in the currently loaded page
        void ExecuteJavascript(IntPtr unBrowserHandle, string pchScript);

        // Mouse click and mouse movement commands
        void MouseUp(IntPtr unBrowserHandle, EHTMLMouseButton eMouseButton);
        void MouseDown(IntPtr unBrowserHandle, EHTMLMouseButton eMouseButton);
        void MouseDoubleClick(IntPtr unBrowserHandle, EHTMLMouseButton eMouseButton);
        // x and y are relative to the HTML bounds
        void MouseMove(IntPtr unBrowserHandle, int x, int y);
        // nDelta is pixels of scroll
        void MouseWheel(IntPtr unBrowserHandle, int nDelta);

        // keyboard interactions, native keycode is the  key code value from your OS, system key flags the key to not
        // be sent as a typed stringacter as well as a key down
        void KeyDown(IntPtr unBrowserHandle, int nNativeKeyCode, IntPtr IntPtr, bool bIsSystemKey = false);
        void KeyUp(IntPtr unBrowserHandle, int nNativeKeyCode, IntPtr IntPtr);
        // cUnicodestring is the unicode stringacter point for this keypress (and potentially multiple strings per press)
        void Keystring(IntPtr unBrowserHandle, int cUnicodestring, IntPtr IntPtr);

        // programmatically scroll this many pixels on the page
        void SetHorizontalScroll(IntPtr unBrowserHandle, int nAbsolutePixelScroll);
        void SetVerticalScroll(IntPtr unBrowserHandle, int nAbsolutePixelScroll);

        // tell the html control if it has key focus currently, controls showing the I-beam cursor in text controls amongst other things
        void SetKeyFocus(IntPtr unBrowserHandle, bool bHasKeyFocus);

        // open the current pages html code in the local editor of choice, used for debugging
        void ViewSource(IntPtr unBrowserHandle);
        // copy the currently selected text on the html page to the local clipboard
        void CopyToClipboard(IntPtr unBrowserHandle);
        // paste from the local clipboard to the current html page
        void PasteFromClipboard(IntPtr unBrowserHandle);

        // find this string in the browser, if bCurrentlyInFind is true then instead cycle to the next matching element
        void Find(IntPtr unBrowserHandle, string pchSearchStr, bool bCurrentlyInFind, bool bReverse);
        // cancel a currently running find
        void StopFind(IntPtr unBrowserHandle);

        // return details about the link at position x,y on the current page
        void GetLinkAtPosition(IntPtr unBrowserHandle, int x, int y);

        // set a webcookie for the hostname in question
        void SetCookie(string pchHostname, string pchKey, string pchValue, string pchPath, IntPtr nExpires, bool bSecure = false, bool bHTTPOnly = false);

        // Zoom the current page by flZoom ( from 0.0 to 2.0, so to zoom to 120% use 1.2 ), zooming around point X,Y in the page (use 0,0 if you don't care)
        void SetPageScaleFactor(IntPtr unBrowserHandle, float flZoom, int nPointX, int nPointY);

        // Enable/disable low-resource background mode, where javascript and repaint timers are throttled, resources are
        // more aggressively purged from memory, and audio/video elements are paused. When background mode is enabled,
        // all HTML5 video and audio objects will execute ".pause()" and gain the property "._steam_background_paused = 1".
        // When background mode is disabled, any video or audio objects with that property will resume with ".play()".
        void SetBackgroundMode(IntPtr unBrowserHandle, bool bBackgroundMode);

        // Scale the output display space by this factor, this is useful when displaying content on high dpi devices.
        // Specifies the ratio between physical and logical pixels.
        void SetDPIScalingFactor(IntPtr unBrowserHandle, float flDPIScaling);

        // Open HTML/JS developer tools
        void OpenDeveloperTools(IntPtr unBrowserHandle);

        // CALLBACKS
        //
        //  These set of functions are used as responses to callback requests
        //

        // You MUST call this in response to a HTML_StartRequest_t callback
        //  Set bAllowed to true to allow this navigation, false to cancel it and stay 
        // on the current page. You can use this feature to limit the valid pages
        // allowed in your HTML surface.
        void AllowStartRequest(IntPtr unBrowserHandle, bool bAllowed);

        // You MUST call this in response to a HTML_JSAlert_t or HTML_JSConfirm_t callback
        //  Set bResult to true for the OK option of a confirm, use false otherwise
        void JSDialogResponse(IntPtr unBrowserHandle, bool bResult);

        // You MUST call this in response to a HTML_FileOpenDialog_t callback

        void FileLoadDialogResponse(IntPtr unBrowserHandle, string pchSelectedFiles);

    }
    public enum EHTMLMouseButton
    {
        eHTMLMouseButton_Left = 0,
        eHTMLMouseButton_Right = 1,
        eHTMLMouseButton_Middle = 2,
    };
}
