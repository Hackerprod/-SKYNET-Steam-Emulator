using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Managers;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamHTTP : ISteamInterface
    {
        public SteamHTTP()
        {
            InterfaceVersion = "SteamHTTP";
        }

        public uint CreateCookieContainer(bool bAllowResponsesToModify)
        {
            Write($"CreateCookieContainer");
            return 0;
        }

        public uint CreateHTTPRequest(uint eHTTPRequestMethod, string pchAbsoluteURL)
        {
            Write($"CreateHTTPRequest");
            return 0;
        }

        public bool DeferHTTPRequest(uint hRequest)
        {
            Write($"DeferHTTPRequest");
            return true;
        }

        public bool GetHTTPDownloadProgressPct(uint hRequest, float pflPercentOut)
        {
            Write($"GetHTTPDownloadProgressPct");
            return true;
        }

        public bool GetHTTPRequestWasTimedOut(uint hRequest, bool pbWasTimedOut)
        {
            Write($"GetHTTPRequestWasTimedOut");
            return true;
        }

        public bool GetHTTPResponseBodyData(uint hRequest, IntPtr pBodyDataBuffer, uint unBufferSize)
        {
            Write($"GetHTTPResponseBodyData");
            return true;
        }

        public bool GetHTTPResponseBodySize(uint hRequest, uint unBodySize)
        {
            Write($"GetHTTPResponseBodySize");
            return true;
        }

        public bool GetHTTPResponseHeaderSize(uint hRequest, string pchHeaderName, uint unResponseHeaderSize)
        {
            Write($"GetHTTPResponseHeaderSize");
            return true;
        }

        public bool GetHTTPResponseHeaderValue(uint hRequest, string pchHeaderName, int pHeaderValueBuffer, uint unBufferSize)
        {
            Write($"GetHTTPResponseHeaderValue");
            return true;
        }

        public bool GetHTTPStreamingResponseBodyData(uint hRequest, uint cOffset, IntPtr pBodyDataBuffer, uint unBufferSize)
        {
            Write($"GetHTTPStreamingResponseBodyData");
            return true;
        }

        public bool PrioritizeHTTPRequest(uint hRequest)
        {
            Write($"PrioritizeHTTPRequest");
            return true;
        }

        public bool ReleaseCookieContainer(uint hCookieContainer)
        {
            Write($"ReleaseCookieContainer");
            return true;
        }

        public bool ReleaseHTTPRequest(uint hRequest)
        {
            Write($"ReleaseHTTPRequest");
            return true;
        }

        public bool SendHTTPRequest(uint hRequest, ulong pCallHandle)
        {
            Write($"SendHTTPRequest");
            return true;
        }

        public bool SendHTTPRequestAndStreamResponse(uint hRequest, ulong pCallHandle)
        {
            Write($"SendHTTPRequestAndStreamResponse");
            return true;
        }

        public bool SetCookie(uint hCookieContainer, string pchHost, string pchUrl, string pchCookie)
        {
            Write($"SetCookie");
            return true;
        }

        public bool SetHTTPRequestAbsoluteTimeoutMS(uint hRequest, uint unMilliseconds)
        {
            Write($"SetHTTPRequestAbsoluteTimeoutMS");
            return true;
        }

        public bool SetHTTPRequestContextValue(uint hRequest, ulong ulContextValue)
        {
            Write($"SetHTTPRequestContextValue");
            return true;
        }

        public bool SetHTTPRequestCookieContainer(uint hRequest, uint hCookieContainer)
        {
            Write($"SetHTTPRequestCookieContainer");
            return true;
        }

        public bool SetHTTPRequestGetOrPostParameter(uint hRequest, string pchParamName, string pchParamValue)
        {
            Write($"SetHTTPRequestGetOrPostParameter");
            return true;
        }

        public bool SetHTTPRequestHeaderValue(uint hRequest, string pchHeaderName, string pchHeaderValue)
        {
            Write($"SetHTTPRequestHeaderValue");
            return true;
        }

        public bool SetHTTPRequestNetworkActivityTimeout(uint hRequest, uint unTimeoutSeconds)
        {
            Write($"SetHTTPRequestNetworkActivityTimeout");
            return true;
        }

        public bool SetHTTPRequestRawPostBody(uint hRequest, string pchContentType, IntPtr pubBody, uint unBodyLen)
        {
            Write($"SetHTTPRequestRawPostBody");
            return true;
        }

        public bool SetHTTPRequestRequiresVerifiedCertificate(uint hRequest, bool bRequireVerifiedCertificate)
        {
            Write($"SetHTTPRequestRequiresVerifiedCertificate");
            return true;
        }

        public bool SetHTTPRequestUserAgentInfo(uint hRequest, string pchUserAgentInfo)
        {
            Write($"SetHTTPRequestUserAgentInfo");
            return true;
        }

        public IntPtr SteamAPI_SteamGameServerHTTP_v003(IntPtr _)
        {
            Write($"SteamAPI_SteamGameServerHTTP_v003");
            return InterfaceManager.FindOrCreateInterface("STEAMHTTP_INTERFACE_VERSION003");
        }
    }
}