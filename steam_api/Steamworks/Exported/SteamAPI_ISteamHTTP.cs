using System;
using System.Runtime.InteropServices;

using SteamAPICall_t = System.UInt64;
using HTTPRequestHandle = System.UInt32;
using HTTPCookieContainerHandle = System.UInt32;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamHTTP
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_GetHTTPRequestWasTimedOut(IntPtr _, uint hRequest, bool pbWasTimedOut)
        {
            Write($"SteamAPI_ISteamHTTP_GetHTTPRequestWasTimedOut");
            return SteamEmulator.SteamHTTP.GetHTTPRequestWasTimedOut(hRequest, pbWasTimedOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS(IntPtr _, uint hRequest, uint unMilliseconds)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS");
            return SteamEmulator.SteamHTTP.SetHTTPRequestAbsoluteTimeoutMS(hRequest, unMilliseconds);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate(IntPtr _, uint hRequest, bool bRequireVerifiedCertificate)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate");
            return SteamEmulator.SteamHTTP.SetHTTPRequestRequiresVerifiedCertificate(hRequest, bRequireVerifiedCertificate);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestUserAgentInfo(IntPtr _, uint hRequest, string pchUserAgentInfo)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestUserAgentInfo");
            return SteamEmulator.SteamHTTP.SetHTTPRequestUserAgentInfo(hRequest, pchUserAgentInfo);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestCookieContainer(IntPtr _, uint hRequest, uint hCookieContainer)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestCookieContainer");
            return SteamEmulator.SteamHTTP.SetHTTPRequestCookieContainer(hRequest, hCookieContainer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetCookie(IntPtr _, uint hCookieContainer, string pchHost, string pchUrl, string pchCookie)
        {
            Write($"SteamAPI_ISteamHTTP_SetCookie");
            return SteamEmulator.SteamHTTP.SetCookie(hCookieContainer, pchHost, pchUrl, pchCookie);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_ReleaseCookieContainer(IntPtr _, uint hCookieContainer)
        {
            Write($"SteamAPI_ISteamHTTP_ReleaseCookieContainer");
            return SteamEmulator.SteamHTTP.ReleaseCookieContainer(hCookieContainer);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static uint SteamAPI_ISteamHTTP_CreateCookieContainer(IntPtr _, bool bAllowResponsesToModify)
        {
            Write($"SteamAPI_ISteamHTTP_CreateCookieContainer");
            return SteamEmulator.SteamHTTP.CreateCookieContainer(bAllowResponsesToModify);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestRawPostBody(IntPtr _, uint hRequest, string pchContentType, IntPtr pubBody, uint unBodyLen)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestRawPostBody");
            return SteamEmulator.SteamHTTP.SetHTTPRequestRawPostBody(hRequest, pchContentType, pubBody, unBodyLen);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_GetHTTPDownloadProgressPct(IntPtr _, uint hRequest, float pflPercentOut)
        {
            Write($"SteamAPI_ISteamHTTP_GetHTTPDownloadProgressPct");
            return SteamEmulator.SteamHTTP.GetHTTPDownloadProgressPct(hRequest, pflPercentOut);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_ReleaseHTTPRequest(IntPtr _, uint hRequest)
        {
            Write($"SteamAPI_ISteamHTTP_ReleaseHTTPRequest");
            return SteamEmulator.SteamHTTP.ReleaseHTTPRequest(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_GetHTTPStreamingResponseBodyData(IntPtr _, uint hRequest, uint cOffset, IntPtr pBodyDataBuffer, uint unBufferSize)
        {
            Write($"SteamAPI_ISteamHTTP_GetHTTPStreamingResponseBodyData");
            return SteamEmulator.SteamHTTP.GetHTTPStreamingResponseBodyData(hRequest, cOffset, pBodyDataBuffer, unBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_GetHTTPResponseBodyData(IntPtr _, uint hRequest, IntPtr pBodyDataBuffer, uint unBufferSize)
        {
            Write($"SteamAPI_ISteamHTTP_GetHTTPResponseBodyData");
            return SteamEmulator.SteamHTTP.GetHTTPResponseBodyData(hRequest, pBodyDataBuffer, unBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_GetHTTPResponseBodySize(IntPtr _, uint hRequest, uint unBodySize)
        {
            Write($"SteamAPI_ISteamHTTP_GetHTTPResponseBodySize");
            return SteamEmulator.SteamHTTP.GetHTTPResponseBodySize(hRequest, unBodySize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_GetHTTPResponseHeaderValue(IntPtr _, uint hRequest, string pchHeaderName, int pHeaderValueBuffer, uint unBufferSize)
        {
            Write($"SteamAPI_ISteamHTTP_GetHTTPResponseHeaderValue");
            return SteamEmulator.SteamHTTP.GetHTTPResponseHeaderValue(hRequest, pchHeaderName, pHeaderValueBuffer, unBufferSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_GetHTTPResponseHeaderSize(IntPtr _, uint hRequest, string pchHeaderName, uint unResponseHeaderSize)
        {
            Write($"SteamAPI_ISteamHTTP_GetHTTPResponseHeaderSize");
            return SteamEmulator.SteamHTTP.GetHTTPResponseHeaderSize(hRequest, pchHeaderName, unResponseHeaderSize);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_PrioritizeHTTPRequest(IntPtr _, uint hRequest)
        {
            Write($"SteamAPI_ISteamHTTP_PrioritizeHTTPRequest");
            return SteamEmulator.SteamHTTP.PrioritizeHTTPRequest(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_DeferHTTPRequest(IntPtr _, uint hRequest)
        {
            Write($"SteamAPI_ISteamHTTP_DeferHTTPRequest");
            return SteamEmulator.SteamHTTP.DeferHTTPRequest(hRequest);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SendHTTPRequestAndStreamResponse(IntPtr _, uint hRequest, ulong pCallHandle)
        {
            Write($"SteamAPI_ISteamHTTP_SendHTTPRequestAndStreamResponse");
            return SteamEmulator.SteamHTTP.SendHTTPRequestAndStreamResponse(hRequest, pCallHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SendHTTPRequest(IntPtr _, uint hRequest, ref ulong pCallHandle)
        {
            Write($"SteamAPI_ISteamHTTP_SendHTTPRequest");
            return SteamEmulator.SteamHTTP.SendHTTPRequest(hRequest, ref pCallHandle);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestGetOrPostParameter(IntPtr _, uint hRequest, string pchParamName, string pchParamValue)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestGetOrPostParameter");
            return SteamEmulator.SteamHTTP.SetHTTPRequestGetOrPostParameter(hRequest, pchParamName, pchParamValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestHeaderValue(IntPtr _, uint hRequest, string pchHeaderName, string pchHeaderValue)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestHeaderValue");
            return SteamEmulator.SteamHTTP.SetHTTPRequestHeaderValue(hRequest, pchHeaderName, pchHeaderValue);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestNetworkActivityTimeout(IntPtr _, uint hRequest, uint unTimeoutSeconds)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestNetworkActivityTimeout");
            return SteamEmulator.SteamHTTP.SetHTTPRequestNetworkActivityTimeout(hRequest, unTimeoutSeconds);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static HTTPRequestHandle SteamAPI_ISteamHTTP_CreateHTTPRequest(IntPtr _, uint eHTTPRequestMethod, string pchAbsoluteURL)
        {
            Write($"SteamAPI_ISteamHTTP_CreateHTTPRequest");
            return SteamEmulator.SteamHTTP.CreateHTTPRequest(eHTTPRequestMethod, pchAbsoluteURL);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamHTTP_SetHTTPRequestContextValue(IntPtr _, uint hRequest, ulong ulContextValue)
        {
            Write($"SteamAPI_ISteamHTTP_SetHTTPRequestContextValue");
            return SteamEmulator.SteamHTTP.SetHTTPRequestContextValue(hRequest, ulContextValue);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

