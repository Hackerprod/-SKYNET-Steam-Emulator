using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SKYNET;
using Steamworks;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamHTTP : BaseCalls
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_GetHTTPRequestWasTimedOut(uint hRequest, ref bool pbWasTimedOut)
        {
            Write($"SteamAPI_ISteamHTTP_GetHTTPRequestWasTimedOut");
            return SteamEmulator.SteamHTTP.GetHTTPRequestWasTimedOut(hRequest, ref pbWasTimedOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS(uint hRequest, uint unMilliseconds)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS");
            return SteamEmulator.SteamHTTP.SetHTTPRequestAbsoluteTimeoutMS(hRequest, unMilliseconds);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate(uint hRequest, bool bRequireVerifiedCertificate)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate");
            return SteamEmulator.SteamHTTP.SetHTTPRequestRequiresVerifiedCertificate(hRequest, bRequireVerifiedCertificate);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestUserAgentInfo(uint hRequest, string pchUserAgentInfo)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestUserAgentInfo");
            return SteamEmulator.SteamHTTP.SetHTTPRequestUserAgentInfo(hRequest, pchUserAgentInfo);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestCookieContainer(uint hRequest, uint hCookieContainer)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestCookieContainer");
            return SteamEmulator.SteamHTTP.SetHTTPRequestCookieContainer(hRequest, hCookieContainer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetCookie(uint hCookieContainer, string pchHost, string pchUrl, string pchCookie)
        {
            Write($"SteamAPI_ISteamHTTP_SetCookie");
            return SteamEmulator.SteamHTTP.SetCookie(hCookieContainer, pchHost, pchUrl, pchCookie);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_ReleaseCookieContainer(uint hCookieContainer)
        {
            Write($"SteamAPI_ISteamHTTP_ReleaseCookieContainer");
            return SteamEmulator.SteamHTTP.ReleaseCookieContainer(hCookieContainer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamHTTP_CreateCookieContainer(bool bAllowResponsesToModify)
        {
            Write($"SteamAPI_ISteamHTTP_CreateCookieContainer");
            return SteamEmulator.SteamHTTP.CreateCookieContainer(bAllowResponsesToModify);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestRawPostBody(uint hRequest, string pchContentType, IntPtr pubBody, uint unBodyLen)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestRawPostBody");
            return SteamEmulator.SteamHTTP.SetHTTPRequestRawPostBody(hRequest, pchContentType, pubBody, unBodyLen);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_GetHTTPDownloadProgressPct(uint hRequest, ref float pflPercentOut)
        {
            Write($"SteamAPI_ISteamHTTP_GetHTTPDownloadProgressPct");
            return SteamEmulator.SteamHTTP.GetHTTPDownloadProgressPct(hRequest, ref pflPercentOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_ReleaseHTTPRequest(uint hRequest)
        {
            Write($"SteamAPI_ISteamHTTP_ReleaseHTTPRequest");
            return SteamEmulator.SteamHTTP.ReleaseHTTPRequest(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_GetHTTPStreamingResponseBodyData(uint hRequest, uint cOffset, IntPtr pBodyDataBuffer, uint unBufferSize)
        {
            Write($"SteamAPI_ISteamHTTP_GetHTTPStreamingResponseBodyData");
            return SteamEmulator.SteamHTTP.GetHTTPStreamingResponseBodyData(hRequest, cOffset, pBodyDataBuffer, unBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_GetHTTPResponseBodyData(uint hRequest, IntPtr pBodyDataBuffer, uint unBufferSize)
        {
            Write($"SteamAPI_ISteamHTTP_GetHTTPResponseBodyData");
            return SteamEmulator.SteamHTTP.GetHTTPResponseBodyData(hRequest, pBodyDataBuffer, unBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_GetHTTPResponseBodySize(uint hRequest, ref uint unBodySize)
        {
            Write($"SteamAPI_ISteamHTTP_GetHTTPResponseBodySize");
            return SteamEmulator.SteamHTTP.GetHTTPResponseBodySize(hRequest, ref unBodySize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_GetHTTPResponseHeaderValue(uint hRequest, string pchHeaderName, IntPtr pHeaderValueBuffer, uint unBufferSize)
        {
            Write($"SteamAPI_ISteamHTTP_GetHTTPResponseHeaderValue");
            return SteamEmulator.SteamHTTP.GetHTTPResponseHeaderValue(hRequest, pchHeaderName, pHeaderValueBuffer, unBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_GetHTTPResponseHeaderSize(uint hRequest, string pchHeaderName, ref uint unResponseHeaderSize)
        {
            Write($"SteamAPI_ISteamHTTP_GetHTTPResponseHeaderSize");
            return SteamEmulator.SteamHTTP.GetHTTPResponseHeaderSize(hRequest, pchHeaderName, ref unResponseHeaderSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_PrioritizeHTTPRequest(uint hRequest)
        {
            Write($"SteamAPI_ISteamHTTP_PrioritizeHTTPRequest");
            return SteamEmulator.SteamHTTP.PrioritizeHTTPRequest(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_DeferHTTPRequest(uint hRequest)
        {
            Write($"SteamAPI_ISteamHTTP_DeferHTTPRequest");
            return SteamEmulator.SteamHTTP.DeferHTTPRequest(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SendHTTPRequestAndStreamResponse(uint hRequest, ref ulong pCallHandle)
        {
            Write($"SteamAPI_ISteamHTTP_SendHTTPRequestAndStreamResponse");
            return SteamEmulator.SteamHTTP.SendHTTPRequestAndStreamResponse(hRequest, ref pCallHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SendHTTPRequest(uint hRequest, ref ulong pCallHandle)
        {
            Write($"SteamAPI_ISteamHTTP_SendHTTPRequest");
            return SteamEmulator.SteamHTTP.SendHTTPRequest(hRequest, ref pCallHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestGetOrPostParameter(uint hRequest, string pchParamName, string pchParamValue)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestGetOrPostParameter");
            return SteamEmulator.SteamHTTP.SetHTTPRequestGetOrPostParameter(hRequest, pchParamName, pchParamValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestHeaderValue(uint hRequest, string pchHeaderName, string pchHeaderValue)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestHeaderValue");
            return SteamEmulator.SteamHTTP.SetHTTPRequestHeaderValue(hRequest, pchHeaderName, pchHeaderValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestNetworkActivityTimeout(uint hRequest, uint unTimeoutSeconds)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestNetworkActivityTimeout");
            return SteamEmulator.SteamHTTP.SetHTTPRequestNetworkActivityTimeout(hRequest, unTimeoutSeconds);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static IntPtr SteamAPI_ISteamHTTP_CreateHTTPRequest(/*HTTPMethod*/uint eHTTPRequestMethod, string pchAbsoluteURL)
        {
            Write($"SteamAPI_ISteamHTTP_CreateHTTPRequest");
            return SteamEmulator.SteamHTTP.CreateHTTPRequest(eHTTPRequestMethod, pchAbsoluteURL);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestContextValue(uint hRequest, ulong ulContextValue)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestContextValue");
            return SteamEmulator.SteamHTTP.SetHTTPRequestContextValue(hRequest, ulContextValue);
        }

        public enum HTTPMethod : int
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
}

