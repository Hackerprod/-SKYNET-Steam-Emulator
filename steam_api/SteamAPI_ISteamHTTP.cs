using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Steamworks;

public class SteamAPI_ISteamHTTP : BaseCalls
{
    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_GetHTTPRequestWasTimedOut(uint hRequest, ref bool pbWasTimedOut)
    {
        Write($"SteamAPI_ISteamHTTP_GetHTTPRequestWasTimedOut");
        return SteamClient.SteamHTTP.GetHTTPRequestWasTimedOut(hRequest, ref pbWasTimedOut);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS(uint hRequest, uint unMilliseconds)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS");
        return SteamClient.SteamHTTP.SetHTTPRequestAbsoluteTimeoutMS(hRequest, unMilliseconds);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate(uint hRequest, bool bRequireVerifiedCertificate)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate");
        return SteamClient.SteamHTTP.SetHTTPRequestRequiresVerifiedCertificate(hRequest, bRequireVerifiedCertificate);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestUserAgentInfo(uint hRequest, string pchUserAgentInfo)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestUserAgentInfo");
        return SteamClient.SteamHTTP.SetHTTPRequestUserAgentInfo(hRequest, pchUserAgentInfo);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestCookieContainer(uint hRequest, uint hCookieContainer)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestCookieContainer");
        return SteamClient.SteamHTTP.SetHTTPRequestCookieContainer(hRequest, hCookieContainer);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetCookie(uint hCookieContainer, string pchHost, string pchUrl, string pchCookie)
    {
        Write($"SteamAPI_ISteamHTTP_SetCookie");
        return SteamClient.SteamHTTP.SetCookie(hCookieContainer, pchHost, pchUrl, pchCookie);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_ReleaseCookieContainer(uint hCookieContainer)
    {
        Write($"SteamAPI_ISteamHTTP_ReleaseCookieContainer");
        return SteamClient.SteamHTTP.ReleaseCookieContainer(hCookieContainer);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static uint SteamAPI_ISteamHTTP_CreateCookieContainer(bool bAllowResponsesToModify)
    {
        Write($"SteamAPI_ISteamHTTP_CreateCookieContainer");
        return SteamClient.SteamHTTP.CreateCookieContainer(bAllowResponsesToModify);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestRawPostBody(uint hRequest, string pchContentType, IntPtr pubBody, uint unBodyLen)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestRawPostBody");
        return SteamClient.SteamHTTP.SetHTTPRequestRawPostBody(hRequest, pchContentType, pubBody, unBodyLen);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_GetHTTPDownloadProgressPct(uint hRequest, ref float pflPercentOut)
    {
        Write($"SteamAPI_ISteamHTTP_GetHTTPDownloadProgressPct");
        return SteamClient.SteamHTTP.GetHTTPDownloadProgressPct(hRequest, ref pflPercentOut);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_ReleaseHTTPRequest(uint hRequest)
    {
        Write($"SteamAPI_ISteamHTTP_ReleaseHTTPRequest");
        return SteamClient.SteamHTTP.ReleaseHTTPRequest(hRequest);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_GetHTTPStreamingResponseBodyData(uint hRequest, uint cOffset, IntPtr pBodyDataBuffer, uint unBufferSize)
    {
        Write($"SteamAPI_ISteamHTTP_GetHTTPStreamingResponseBodyData");
        return SteamClient.SteamHTTP.GetHTTPStreamingResponseBodyData(hRequest, cOffset, pBodyDataBuffer, unBufferSize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_GetHTTPResponseBodyData(uint hRequest, IntPtr pBodyDataBuffer, uint unBufferSize)
    {
        Write($"SteamAPI_ISteamHTTP_GetHTTPResponseBodyData");
        return SteamClient.SteamHTTP.GetHTTPResponseBodyData(hRequest, pBodyDataBuffer, unBufferSize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_GetHTTPResponseBodySize(uint hRequest, ref uint unBodySize)
    {
        Write($"SteamAPI_ISteamHTTP_GetHTTPResponseBodySize");
        return SteamClient.SteamHTTP.GetHTTPResponseBodySize(hRequest, ref unBodySize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_GetHTTPResponseHeaderValue(uint hRequest, string pchHeaderName, IntPtr pHeaderValueBuffer, uint unBufferSize)
    {
        Write($"SteamAPI_ISteamHTTP_GetHTTPResponseHeaderValue");
        return SteamClient.SteamHTTP.GetHTTPResponseHeaderValue(hRequest, pchHeaderName, pHeaderValueBuffer, unBufferSize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_GetHTTPResponseHeaderSize(uint hRequest, string pchHeaderName, ref uint unResponseHeaderSize)
    {
        Write($"SteamAPI_ISteamHTTP_GetHTTPResponseHeaderSize");
        return SteamClient.SteamHTTP.GetHTTPResponseHeaderSize(hRequest, pchHeaderName, ref unResponseHeaderSize);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_PrioritizeHTTPRequest(uint hRequest)
    {
        Write($"SteamAPI_ISteamHTTP_PrioritizeHTTPRequest");
        return SteamClient.SteamHTTP.PrioritizeHTTPRequest(hRequest);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_DeferHTTPRequest(uint hRequest)
    {
        Write($"SteamAPI_ISteamHTTP_DeferHTTPRequest");
        return SteamClient.SteamHTTP.DeferHTTPRequest(hRequest);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SendHTTPRequestAndStreamResponse(uint hRequest, ref ulong pCallHandle)
    {
        Write($"SteamAPI_ISteamHTTP_SendHTTPRequestAndStreamResponse");
        return SteamClient.SteamHTTP.SendHTTPRequestAndStreamResponse(hRequest, ref pCallHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SendHTTPRequest(uint hRequest, ref ulong pCallHandle)
    {
        Write($"SteamAPI_ISteamHTTP_SendHTTPRequest");
                return SteamClient.SteamHTTP.SendHTTPRequest(hRequest, ref pCallHandle);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestGetOrPostParameter(uint hRequest, string pchParamName, string pchParamValue)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestGetOrPostParameter");
                return SteamClient.SteamHTTP.SetHTTPRequestGetOrPostParameter(hRequest, pchParamName, pchParamValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestHeaderValue(uint hRequest, string pchHeaderName, string pchHeaderValue)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestHeaderValue");
                return SteamClient.SteamHTTP.SetHTTPRequestHeaderValue(hRequest, pchHeaderName, pchHeaderValue);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestNetworkActivityTimeout(uint hRequest, uint unTimeoutSeconds)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestNetworkActivityTimeout");
                return SteamClient.SteamHTTP.SetHTTPRequestNetworkActivityTimeout(hRequest, unTimeoutSeconds);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_SteamGameServerHTTP_v003()
    {
        Write($"SteamAPI_SteamGameServerHTTP_v003");
        return SteamClient.SteamHTTP.SteamAPI_SteamGameServerHTTP_v003();
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static IntPtr SteamAPI_ISteamHTTP_CreateHTTPRequest(/*HTTPMethod*/uint eHTTPRequestMethod, string pchAbsoluteURL)
    {
        Write($"SteamAPI_ISteamHTTP_CreateHTTPRequest");
        return SteamClient.SteamHTTP.CreateHTTPRequest(eHTTPRequestMethod, pchAbsoluteURL);
    }

    [DllExport(CallingConvention = CallingConvention.Cdecl)]
    public static bool SteamAPI_ISteamHTTP_SetHTTPRequestContextValue(uint hRequest, ulong ulContextValue)
    {
        Write($"SteamAPI_ISteamHTTP_SetHTTPRequestContextValue");
        return SteamClient.SteamHTTP.SetHTTPRequestContextValue(hRequest, ulContextValue);
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

