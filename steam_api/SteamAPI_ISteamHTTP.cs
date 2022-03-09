using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Steamworks;

public class SteamAPI_ISteamHTTP : BaseCalls
{
    [DllExport("SteamAPI_ISteamHTTP_GetHTTPRequestWasTimedOut", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_GetHTTPRequestWasTimedOut(IntPtr instancePtr, uint hRequest, ref bool pbWasTimedOut)
    {
        Write($"SteamAPI_ISteamHTTP_GetHTTPRequestWasTimedOut");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS(IntPtr instancePtr, uint hRequest, uint unMilliseconds)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate(IntPtr instancePtr, uint hRequest, bool bRequireVerifiedCertificate)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_SetHTTPRequestUserAgentInfo", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestUserAgentInfo(IntPtr instancePtr, uint hRequest, string pchUserAgentInfo)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestUserAgentInfo");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_SetHTTPRequestCookieContainer", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestCookieContainer(IntPtr instancePtr, uint hRequest, uint hCookieContainer)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestCookieContainer");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_SetCookie", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetCookie(IntPtr instancePtr, uint hCookieContainer, string pchHost, string pchUrl, string pchCookie)
    {
        Write($"SteamAPI_ISteamHTTP_SetCookie");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_ReleaseCookieContainer", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_ReleaseCookieContainer(IntPtr instancePtr, uint hCookieContainer)
    {
        Write($"SteamAPI_ISteamHTTP_ReleaseCookieContainer");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_CreateCookieContainer", CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamHTTP_CreateCookieContainer(IntPtr instancePtr, bool bAllowResponsesToModify)
    {
        Write($"SteamAPI_ISteamHTTP_CreateCookieContainer");
        return 1;
    }

    [DllExport("SteamAPI_ISteamHTTP_SetHTTPRequestRawPostBody", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestRawPostBody(IntPtr instancePtr, uint hRequest, string pchContentType, IntPtr pubBody, uint unBodyLen)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestRawPostBody");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_GetHTTPDownloadProgressPct", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_GetHTTPDownloadProgressPct(IntPtr instancePtr, uint hRequest, ref float pflPercentOut)
    {
        Write($"SteamAPI_ISteamHTTP_GetHTTPDownloadProgressPct");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_ReleaseHTTPRequest", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_ReleaseHTTPRequest(IntPtr instancePtr, uint hRequest)
    {
        Write($"SteamAPI_ISteamHTTP_ReleaseHTTPRequest");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_GetHTTPStreamingResponseBodyData", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_GetHTTPStreamingResponseBodyData(IntPtr instancePtr, uint hRequest, uint cOffset, IntPtr pBodyDataBuffer, uint unBufferSize)
    {
        Write($"SteamAPI_ISteamHTTP_GetHTTPStreamingResponseBodyData");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_GetHTTPResponseBodyData", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_GetHTTPResponseBodyData(IntPtr instancePtr, uint hRequest, IntPtr pBodyDataBuffer, uint unBufferSize)
    {
        Write($"SteamAPI_ISteamHTTP_GetHTTPResponseBodyData");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_GetHTTPResponseBodySize", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_GetHTTPResponseBodySize(IntPtr instancePtr, uint hRequest, ref uint unBodySize)
    {
        Write($"SteamAPI_ISteamHTTP_GetHTTPResponseBodySize");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_GetHTTPResponseHeaderValue", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_GetHTTPResponseHeaderValue(IntPtr instancePtr, uint hRequest, string pchHeaderName, IntPtr pHeaderValueBuffer, uint unBufferSize)
    {
        Write($"SteamAPI_ISteamHTTP_GetHTTPResponseHeaderValue");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_GetHTTPResponseHeaderSize", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_GetHTTPResponseHeaderSize(IntPtr instancePtr, uint hRequest, string pchHeaderName, ref uint unResponseHeaderSize)
    {
        Write($"SteamAPI_ISteamHTTP_GetHTTPResponseHeaderSize");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_PrioritizeHTTPRequest", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_PrioritizeHTTPRequest(IntPtr instancePtr, uint hRequest)
    {
        Write($"SteamAPI_ISteamHTTP_PrioritizeHTTPRequest");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_DeferHTTPRequest", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_DeferHTTPRequest(IntPtr instancePtr, uint hRequest)
    {
        Write($"SteamAPI_ISteamHTTP_DeferHTTPRequest");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_SendHTTPRequestAndStreamResponse", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SendHTTPRequestAndStreamResponse(IntPtr instancePtr, uint hRequest, ref ulong pCallHandle)
    {
        Write($"SteamAPI_ISteamHTTP_SendHTTPRequestAndStreamResponse");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_SendHTTPRequest", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SendHTTPRequest(IntPtr instancePtr, uint hRequest, ref ulong pCallHandle)
    {
        Write($"SteamAPI_ISteamHTTP_SendHTTPRequest");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_SetHTTPRequestGetOrPostParameter", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestGetOrPostParameter(IntPtr instancePtr, uint hRequest, string pchParamName, string pchParamValue)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestGetOrPostParameter");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_SetHTTPRequestHeaderValue", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestHeaderValue(IntPtr instancePtr, uint hRequest, string pchHeaderName, string pchHeaderValue)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestHeaderValue");
        return true;
    }

    [DllExport("SteamAPI_ISteamHTTP_SetHTTPRequestNetworkActivityTimeout", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestNetworkActivityTimeout(IntPtr instancePtr, uint hRequest, uint unTimeoutSeconds)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestNetworkActivityTimeout");
        return true;
    }

    [DllExport("SteamAPI_SteamGameServerHTTP_v003", CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_SteamGameServerHTTP_v003()
    {
        Write($"SteamAPI_SteamGameServerHTTP_v003");
        return IntPtr.Zero;
    }

    [DllExport("SteamAPI_ISteamHTTP_CreateHTTPRequest", CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamHTTP_CreateHTTPRequest(IntPtr self, /*HTTPMethod*/uint eHTTPRequestMethod, string pchAbsoluteURL)
    {
        Write($"SteamAPI_ISteamHTTP_CreateHTTPRequest");
        return IntPtr.Zero;
    }

    [DllExport("SteamAPI_ISteamHTTP_SetHTTPRequestContextValue", CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestContextValue(IntPtr instancePtr, uint hRequest, ulong ulContextValue)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestContextValue");
        return true;
    }
    internal enum HTTPMethod : int
    {
        Invalid = 0,
        GET = 1,
        HEAD = 2,
        POST = 3,
        PUT = 4,
        DELETE = 5,
        OPTIONS = 6,
        PATCH = 7,
    }
}

