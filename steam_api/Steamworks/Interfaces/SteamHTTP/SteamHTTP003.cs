using Steamworks;
using System;

using SteamAPICall_t = System.UInt64;
using HTTPRequestHandle = System.UInt32;
using HTTPCookieContainerHandle = System.UInt32;


namespace SKYNET.Interface
{
    [Interface("STEAMHTTP_INTERFACE_VERSION002")]
    [Interface("STEAMHTTP_INTERFACE_VERSION003")]
    public class SteamHTTP003 : ISteamInterface
    {
        public HTTPRequestHandle CreateHTTPRequest(IntPtr _, uint eHTTPRequestMethod, string pchAbsoluteURL)
        {
            return SteamEmulator.SteamHTTP.CreateHTTPRequest(eHTTPRequestMethod, pchAbsoluteURL);
        }

        public bool SetHTTPRequestContextValue(IntPtr _, HTTPRequestHandle hRequest, UInt64 ulContextValue)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestContextValue(hRequest, ulContextValue);
        }

        public bool SetHTTPRequestNetworkActivityTimeout(IntPtr _, HTTPRequestHandle hRequest, uint unTimeoutSeconds)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestNetworkActivityTimeout(hRequest, unTimeoutSeconds);
        }

        public bool SetHTTPRequestHeaderValue(IntPtr _, HTTPRequestHandle hRequest, string pchHeaderName, string pchHeaderValue)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestHeaderValue(hRequest, pchHeaderName, pchHeaderValue);
        }

        public bool SetHTTPRequestGetOrPostParameter(IntPtr _, HTTPRequestHandle hRequest, string pchParamName, string pchParamValue)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestGetOrPostParameter(hRequest, pchParamName, pchParamValue);
        }

        public bool SendHTTPRequest(IntPtr _, HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle)
        {
            return SteamEmulator.SteamHTTP.SendHTTPRequest(hRequest, ref pCallHandle);
        }

        public bool SendHTTPRequestAndStreamResponse(IntPtr _, HTTPRequestHandle hRequest, SteamAPICall_t pCallHandle)
        {
            return SteamEmulator.SteamHTTP.SendHTTPRequestAndStreamResponse(hRequest, pCallHandle);
        }

        public bool DeferHTTPRequest(IntPtr _, HTTPRequestHandle hRequest)
        {
            return SteamEmulator.SteamHTTP.DeferHTTPRequest(hRequest);
        }

        public bool PrioritizeHTTPRequest(IntPtr _, HTTPRequestHandle hRequest)
        {
            return SteamEmulator.SteamHTTP.PrioritizeHTTPRequest(hRequest);
        }

        public bool GetHTTPResponseHeaderSize(IntPtr _, HTTPRequestHandle hRequest, string pchHeaderName, uint unResponseHeaderSize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPResponseHeaderSize(hRequest, pchHeaderName, unResponseHeaderSize);
        }

        public bool GetHTTPResponseHeaderValue(IntPtr _, HTTPRequestHandle hRequest, string pchHeaderName, int pHeaderValueBuffer, uint unBufferSize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPResponseHeaderValue(hRequest, pchHeaderName, pHeaderValueBuffer, unBufferSize);
        }

        public bool GetHTTPResponseBodySize(IntPtr _, HTTPRequestHandle hRequest, uint unBodySize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPResponseBodySize(hRequest, unBodySize);
        }

        public bool GetHTTPResponseBodyData(IntPtr _, HTTPRequestHandle hRequest, IntPtr pBodyDataBuffer, uint unBufferSize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPResponseBodyData(hRequest, pBodyDataBuffer, unBufferSize);
        }

        public bool GetHTTPStreamingResponseBodyData(IntPtr _, HTTPRequestHandle hRequest, uint cOffset, IntPtr pBodyDataBuffer, uint unBufferSize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPStreamingResponseBodyData(hRequest, cOffset, pBodyDataBuffer, unBufferSize);
        }

        public bool ReleaseHTTPRequest(IntPtr _, HTTPRequestHandle hRequest)
        {
            return SteamEmulator.SteamHTTP.ReleaseHTTPRequest(hRequest);
        }

        public bool GetHTTPDownloadProgressPct(IntPtr _, HTTPRequestHandle hRequest, float pflPercentOut)
        {
            return SteamEmulator.SteamHTTP.GetHTTPDownloadProgressPct(hRequest, pflPercentOut);
        }

        public bool SetHTTPRequestRawPostBody(IntPtr _, HTTPRequestHandle hRequest, string pchContentType, IntPtr pubBody, uint unBodyLen)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestRawPostBody(hRequest, pchContentType, pubBody, unBodyLen);
        }

        public HTTPCookieContainerHandle CreateCookieContainer(IntPtr _, bool bAllowResponsesToModify)
        {
            return SteamEmulator.SteamHTTP.CreateCookieContainer(bAllowResponsesToModify);
        }

        public bool ReleaseCookieContainer(IntPtr _, HTTPCookieContainerHandle hCookieContainer)
        {
            return SteamEmulator.SteamHTTP.ReleaseCookieContainer(hCookieContainer);
        }

        public bool SetCookie(IntPtr _, HTTPCookieContainerHandle hCookieContainer, string pchHost, string pchUrl, string pchCookie)
        {
            return SteamEmulator.SteamHTTP.SetCookie(hCookieContainer, pchHost, pchUrl, pchCookie);
        }

        public bool SetHTTPRequestCookieContainer(IntPtr _, HTTPRequestHandle hRequest, HTTPCookieContainerHandle hCookieContainer)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestCookieContainer(hRequest, hCookieContainer);
        }

        public bool SetHTTPRequestUserAgentInfo(IntPtr _, HTTPRequestHandle hRequest, string pchUserAgentInfo)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestUserAgentInfo(hRequest, pchUserAgentInfo);
        }

        public bool SetHTTPRequestRequiresVerifiedCertificate(IntPtr _, HTTPRequestHandle hRequest, bool bRequireVerifiedCertificate)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestRequiresVerifiedCertificate(hRequest, bRequireVerifiedCertificate);
        }

        public bool SetHTTPRequestAbsoluteTimeoutMS(IntPtr _, HTTPRequestHandle hRequest, uint unMilliseconds)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestAbsoluteTimeoutMS(hRequest, unMilliseconds);
        }

        public bool GetHTTPRequestWasTimedOut(IntPtr _, HTTPRequestHandle hRequest, bool pbWasTimedOut)
        {
            return SteamEmulator.SteamHTTP.GetHTTPRequestWasTimedOut(hRequest, pbWasTimedOut);
        }


    }
}