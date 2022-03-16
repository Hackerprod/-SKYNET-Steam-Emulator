using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate("SteamHTMLSurface")]
    public class DSteamHTTP : SteamDelegate
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetHTTPRequestWasTimedOut(uint hRequest, ref bool pbWasTimedOut);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetHTTPRequestAbsoluteTimeoutMS(uint hRequest, uint unMilliseconds);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetHTTPRequestRequiresVerifiedCertificate(uint hRequest, bool bRequireVerifiedCertificate);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetHTTPRequestUserAgentInfo(uint hRequest, string pchUserAgentInfo);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetHTTPRequestCookieContainer(uint hRequest, uint hCookieContainer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetCookie(uint hCookieContainer, string pchHost, string pchUrl, string pchCookie);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ReleaseCookieContainer(uint hCookieContainer);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate uint CreateCookieContainer(bool bAllowResponsesToModify);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetHTTPRequestRawPostBody(uint hRequest, string pchContentType, IntPtr pubBody, uint unBodyLen);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetHTTPDownloadProgressPct(uint hRequest, ref float pflPercentOut);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool ReleaseHTTPRequest(uint hRequest);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetHTTPStreamingResponseBodyData(uint hRequest, uint cOffset, IntPtr pBodyDataBuffer, uint unBufferSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetHTTPResponseBodyData(uint hRequest, IntPtr pBodyDataBuffer, uint unBufferSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetHTTPResponseBodySize(uint hRequest, ref uint unBodySize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetHTTPResponseHeaderValue(uint hRequest, string pchHeaderName, IntPtr pHeaderValueBuffer, uint unBufferSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool GetHTTPResponseHeaderSize(uint hRequest, string pchHeaderName, ref uint unResponseHeaderSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool PrioritizeHTTPRequest(uint hRequest);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool DeferHTTPRequest(uint hRequest);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SendHTTPRequestAndStreamResponse(uint hRequest, ref ulong pCallHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SendHTTPRequest(uint hRequest, ref ulong pCallHandle);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetHTTPRequestGetOrPostParameter(uint hRequest, string pchParamName, string pchParamValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetHTTPRequestHeaderValue(uint hRequest, string pchHeaderName, string pchHeaderValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetHTTPRequestNetworkActivityTimeout(uint hRequest, uint unTimeoutSeconds);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr SteamAPI_SteamGameServerHTTP_v003();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate IntPtr CreateHTTPRequest(/*HTTPMethod*/uint eHTTPRequestMethod, string pchAbsoluteURL);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool SetHTTPRequestContextValue(uint hRequest, ulong ulContextValue);

    }
}
