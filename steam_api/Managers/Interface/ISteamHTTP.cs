using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamHTTP
    {
        bool GetHTTPRequestWasTimedOut(uint hRequest, ref bool pbWasTimedOut);
        bool SetHTTPRequestAbsoluteTimeoutMS(uint hRequest, uint unMilliseconds);
        bool SetHTTPRequestRequiresVerifiedCertificate(uint hRequest, bool bRequireVerifiedCertificate);
        bool SetHTTPRequestUserAgentInfo(uint hRequest, string pchUserAgentInfo);
        bool SetHTTPRequestCookieContainer(uint hRequest, uint hCookieContainer);
        bool SetCookie(uint hCookieContainer, string pchHost, string pchUrl, string pchCookie);
        bool ReleaseCookieContainer(uint hCookieContainer);
        uint CreateCookieContainer(bool bAllowResponsesToModify);
        bool SetHTTPRequestRawPostBody(uint hRequest, string pchContentType, IntPtr pubBody, uint unBodyLen);
        bool GetHTTPDownloadProgressPct(uint hRequest, ref float pflPercentOut);
        bool ReleaseHTTPRequest(uint hRequest);
        bool GetHTTPStreamingResponseBodyData(uint hRequest, uint cOffset, IntPtr pBodyDataBuffer, uint unBufferSize);
        bool GetHTTPResponseBodyData(uint hRequest, IntPtr pBodyDataBuffer, uint unBufferSize);
        bool GetHTTPResponseBodySize(uint hRequest, ref uint unBodySize);
        bool GetHTTPResponseHeaderValue(uint hRequest, string pchHeaderName, IntPtr pHeaderValueBuffer, uint unBufferSize);
        bool GetHTTPResponseHeaderSize(uint hRequest, string pchHeaderName, ref uint unResponseHeaderSize);
        bool PrioritizeHTTPRequest(uint hRequest);
        bool DeferHTTPRequest(uint hRequest);
        bool SendHTTPRequestAndStreamResponse(uint hRequest, ref ulong pCallHandle);
        bool SendHTTPRequest(uint hRequest, ref ulong pCallHandle);
        bool SetHTTPRequestGetOrPostParameter(uint hRequest, string pchParamName, string pchParamValue);
        bool SetHTTPRequestHeaderValue(uint hRequest, string pchHeaderName, string pchHeaderValue);
        bool SetHTTPRequestNetworkActivityTimeout(uint hRequest, uint unTimeoutSeconds);
        IntPtr SteamAPI_SteamGameServerHTTP_v003();
        IntPtr CreateHTTPRequest(/*HTTPMethod*/uint eHTTPRequestMethod, string pchAbsoluteURL);
        bool SetHTTPRequestContextValue(uint hRequest, ulong ulContextValue);

    }
}
