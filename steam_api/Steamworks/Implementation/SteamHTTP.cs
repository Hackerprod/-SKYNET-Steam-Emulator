using System;
using SKYNET;
using SKYNET.Helpers;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamHTTP : ISteamInterface
    {
        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamHTTP()
        {
            InterfaceVersion = "SteamHTTP";
        }

        public uint CreateCookieContainer(bool bAllowResponsesToModify)
        {
            Write($"CreateCookieContainer");
            return 0;
        }

        public IntPtr CreateHTTPRequest(uint eHTTPRequestMethod, string pchAbsoluteURL)
        {
            Write($"CreateHTTPRequest");
            return IntPtr.Zero;
        }

        public bool DeferHTTPRequest(uint hRequest)
        {
            Write($"DeferHTTPRequest");
            return true;
        }

        public bool GetHTTPDownloadProgressPct(uint hRequest, ref float pflPercentOut)
        {
            Write($"GetHTTPDownloadProgressPct");
            return true;
        }

        public bool GetHTTPRequestWasTimedOut(uint hRequest, ref bool pbWasTimedOut)
        {
            Write($"GetHTTPRequestWasTimedOut");
            return true;
        }

        public bool GetHTTPResponseBodyData(uint hRequest, IntPtr pBodyDataBuffer, uint unBufferSize)
        {
            Write($"GetHTTPResponseBodyData");
            return true;
        }

        public bool GetHTTPResponseBodySize(uint hRequest, ref uint unBodySize)
        {
            Write($"GetHTTPResponseBodySize");
            return true;
        }

        public bool GetHTTPResponseHeaderSize(uint hRequest, string pchHeaderName, ref uint unResponseHeaderSize)
        {
            Write($"GetHTTPResponseHeaderSize");
            return true;
        }

        public bool GetHTTPResponseHeaderValue(uint hRequest, string pchHeaderName, IntPtr pHeaderValueBuffer, uint unBufferSize)
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

        public bool SendHTTPRequest(uint hRequest, ref ulong pCallHandle)
        {
            Write($"SendHTTPRequest");
            return true;
        }

        public bool SendHTTPRequestAndStreamResponse(uint hRequest, ref ulong pCallHandle)
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
            return SteamEmulator.SteamGameServerHTTP.MemoryAddress;
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}