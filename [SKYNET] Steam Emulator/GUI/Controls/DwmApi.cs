using System;
using System.Drawing;
using System.Runtime.InteropServices;

public class DwmApi
{
    [StructLayout(LayoutKind.Explicit)]
    public struct RECT
    {
        [FieldOffset(12)]
        public int bottom;

        [FieldOffset(0)]
        public int left;

        [FieldOffset(8)]
        public int right;

        [FieldOffset(4)]
        public int top;

        public int Height => bottom - top;

        public Size Size => new Size(Width, Height);

        public int Width => right - left;

        public RECT(Rectangle rect)
        {
            left = rect.Left;
            top = rect.Top;
            right = rect.Right;
            bottom = rect.Bottom;
        }

        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public void Set()
        {
            left = InlineAssignHelper(ref top, InlineAssignHelper(ref right, InlineAssignHelper(ref bottom, 0)));
        }

        public void Set(Rectangle rect)
        {
            left = rect.Left;
            top = rect.Top;
            right = rect.Right;
            bottom = rect.Bottom;
        }

        public void Set(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle(left, top, right - left, bottom - top);
        }

        private static T InlineAssignHelper<T>(ref T target, T value)
        {
            target = value;
            return value;
        }
    }

    public struct DWM_BLURBEHIND
    {
        public const int DWM_BB_ENABLE = 1;

        public const int DWM_BB_BLURREGION = 2;

        public const int DWM_BB_TRANSITIONONMAXIMIZED = 4;

        public int dwFlags;

        public int fEnable;

        public IntPtr hRgnBlur;

        public int fTransitionOnMaximized;

        public static DWM_BLURBEHIND Enable = new DWM_BLURBEHIND(enable: true);

        public static DWM_BLURBEHIND Disable = new DWM_BLURBEHIND(enable: false);

        private DWM_BLURBEHIND(bool enable)
        {
            dwFlags = 1;
            fEnable = (enable ? 1 : 0);
            hRgnBlur = IntPtr.Zero;
            fTransitionOnMaximized = 0;
        }
    }

    public struct DWM_PRESENT_PARAMETERS
    {
        public int cbSize;

        public int fQueue;

        public long cRefreshStart;

        public int cBuffer;

        public int fUseSourceRate;

        public UNSIGNED_RATIO rateSource;

        public int cRefreshesPerFrame;

        public DWM_SOURCE_FRAME_SAMPLING eSampling;
    }

    public struct DWM_THUMBNAIL_PROPERTIES
    {
        public int dwFlags;

        public RECT rcDestination;

        public RECT rcSource;

        public byte opacity;

        public int fVisible;

        public int fSourceClientAreaOnly;
    }

    public struct DWM_TIMING_INFO
    {
        public int cbSize;

        public UNSIGNED_RATIO rateRefresh;

        public UNSIGNED_RATIO rateCompose;

        public long qpcVBlank;

        public long cRefresh;

        public long qpcCompose;

        public long cFrame;

        public long cRefreshFrame;

        public long cRefreshConfirmed;

        public int cFlipsOutstanding;

        public long cFrameCurrent;

        public long cFramesAvailable;

        public long cFrameCleared;

        public long cFramesReceived;

        public long cFramesDisplayed;

        public long cFramesDropped;

        public long cFramesMissed;
    }

    public struct UNSIGNED_RATIO
    {
        public int uiNumerator;

        public int uiDenominator;
    }

    public struct MARGINS
    {
        public int cxLeftWidth;

        public int cxRightWidth;

        public int cyTopHeight;

        public int cyBottomHeight;

        public MARGINS(int Left, int Right, int Top, int Bottom)
        {
            cxLeftWidth = Left;
            cxRightWidth = Right;
            cyTopHeight = Top;
            cyBottomHeight = Bottom;
        }
    }

    public struct WTA_OPTIONS
    {
        public uint Flags;

        public uint Mask;
    }

    public enum DWM_SOURCE_FRAME_SAMPLING
    {
        DWM_SOURCE_FRAME_SAMPLING_POINT,
        DWM_SOURCE_FRAME_SAMPLING_COVERAGE,
        DWM_SOURCE_FRAME_SAMPLING_LAST
    }

    public enum DWMNCRENDERINGPOLICY
    {
        DWMNCRP_USEWINDOWSTYLE,
        DWMNCRP_DISABLED,
        DWMNCRP_ENABLED
    }

    public enum DWMWINDOWATTRIBUTE
    {
        DWMWA_ALLOW_NCPAINT = 4,
        DWMWA_CAPTION_BUTTON_BOUNDS = 5,
        DWMWA_FLIP3D_POLICY = 8,
        DWMWA_FORCE_ICONIC_REPRESENTATION = 7,
        DWMWA_LAST = 9,
        DWMWA_NCRENDERING_ENABLED = 1,
        DWMWA_NCRENDERING_POLICY = 2,
        DWMWA_NONCLIENT_RTL_LAYOUT = 6,
        DWMWA_TRANSITIONS_FORCEDISABLED = 3
    }

    public enum WindowThemeAttributeType
    {
        WTA_NONCLIENT = 1
    }

    public const int DWM_BB_BLURREGION = 2;

    public const int DWM_BB_ENABLE = 1;

    public const int DWM_BB_TRANSITIONONMAXIMIZED = 4;

    public const string DWM_COMPOSED_EVENT_BASE_NAME = "DwmComposedEvent_";

    public const string DWM_COMPOSED_EVENT_NAME_FORMAT = "%s%d";

    public const int DWM_COMPOSED_EVENT_NAME_MAX_LENGTH = 64;

    public const int DWM_FRAME_DURATION_DEFAULT = -1;

    public const int DWM_TNP_OPACITY = 4;

    public const int DWM_TNP_RECTDESTINATION = 1;

    public const int DWM_TNP_RECTSOURCE = 2;

    public const int DWM_TNP_SOURCECLIENTAREAONLY = 16;

    public const int DWM_TNP_VISIBLE = 8;

    public const int WM_DWMCOMPOSITIONCHANGED = 798;

    public static uint WTNCA_NODRAWCAPTION = 1u;

    public static uint WTNCA_NODRAWICON = 2u;

    public static uint WTNCA_NOSYSMENU = 4u;

    public static uint WTNCA_NOMIRRORHELP = 8u;

    public static readonly bool DwmApiAvailable = Environment.OSVersion.Version.Major >= 6;

    [DllImport("dwmapi.dll")]
    public static extern int DwmDefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref IntPtr result);

    [DllImport("dwmapi.dll")]
    public static extern int DwmEnableComposition(int fEnable);

    [DllImport("dwmapi.dll")]
    public static extern int DwmEnableMMCSS(int fEnableMMCSS);

    [DllImport("dwmapi.dll")]
    public static extern int DwmExtendFrameIntoClientArea(IntPtr hdc, ref MARGINS marInset);

    [DllImport("dwmapi.dll")]
    public static extern int DwmGetColorizationColor(ref int pcrColorization, ref int pfOpaqueBlend);

    [DllImport("dwmapi.dll")]
    public static extern int DwmGetCompositionTimingInfo(IntPtr hwnd, ref DWM_TIMING_INFO pTimingInfo);

    [DllImport("dwmapi.dll")]
    public static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, IntPtr pvAttribute, int cbAttribute);

    [DllImport("dwmapi.dll")]
    public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

    [DllImport("dwmapi.dll")]
    public static extern int DwmIsCompositionEnabled(out bool pfEnabled);

    [DllImport("dwmapi.dll")]
    public static extern int DwmModifyPreviousDxFrameDuration(IntPtr hwnd, int cRefreshes, int fRelative);

    [DllImport("dwmapi.dll")]
    public static extern int DwmQueryThumbnailSourceSize(IntPtr hThumbnail, ref Size pSize);

    [DllImport("dwmapi.dll")]
    public static extern int DwmRegisterThumbnail(IntPtr hwndDestination, IntPtr hwndSource, ref Size pMinimizedSize, ref IntPtr phThumbnailId);

    [DllImport("dwmapi.dll")]
    public static extern int DwmSetDxFrameDuration(IntPtr hwnd, int cRefreshes);

    [DllImport("dwmapi.dll")]
    public static extern int DwmSetPresentParameters(IntPtr hwnd, ref DWM_PRESENT_PARAMETERS pPresentParams);

    [DllImport("dwmapi.dll")]
    public static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, IntPtr pvAttribute, int cbAttribute);

    [DllImport("dwmapi.dll")]
    public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    [DllImport("dwmapi.dll")]
    public static extern int DwmUnregisterThumbnail(IntPtr hThumbnailId);

    [DllImport("dwmapi.dll")]
    public static extern int DwmUpdateThumbnailProperties(IntPtr hThumbnailId, ref DWM_THUMBNAIL_PROPERTIES ptnProperties);

    [DllImport("dwmapi.dll")]
    public static extern int DwmEnableBlurBehindWindow(IntPtr hWnd, ref DWM_BLURBEHIND pBlurBehind);

    [DllImport("uxtheme.dll")]
    public static extern int SetWindowThemeAttribute(IntPtr hWnd, WindowThemeAttributeType wtype, ref WTA_OPTIONS attributes, uint size);
}