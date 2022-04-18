using Steamworks;
using System;


namespace SKYNET.Interface
{
    [Interface("STEAMHTTP_INTERFACE_VERSION003")]
    public class SteamHTTP003 : ISteamInterface
    {
        public HTTPRequestHandle CreateHTTPRequest(IntPtr _, uint eHTTPRequestMethod, string pchAbsoluteURL)
        {
            return SteamEmulator.SteamHTTP.CreateHTTPRequest(eHTTPRequestMethod, pchAbsoluteURL);
        }

        public bool SetHTTPRequestContextValue(IntPtr _, uint hRequest, ulong ulContextValue)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestContextValue(hRequest, ulContextValue);
        }

        public bool SetHTTPRequestNetworkActivityTimeout(IntPtr _, uint hRequest, uint unTimeoutSeconds)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestNetworkActivityTimeout(hRequest, unTimeoutSeconds);
        }

        public bool SetHTTPRequestHeaderValue(IntPtr _, HTTPRequestHandle hRequest, string pchHeaderName, string pchHeaderValue)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestHeaderValue(hRequest, pchHeaderName, pchHeaderValue);
        }

        public bool SetHTTPRequestGetOrPostParameter(IntPtr _, uint hRequest, string pchParamName, string pchParamValue)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestGetOrPostParameter(hRequest, pchParamName, pchParamValue);
        }

        public bool SendHTTPRequest(IntPtr _, HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle)
        {
            return SteamEmulator.SteamHTTP.SendHTTPRequest(hRequest, ref pCallHandle);
        }

        public bool SendHTTPRequestAndStreamResponse(IntPtr _, uint hRequest, ulong pCallHandle)
        {
            return SteamEmulator.SteamHTTP.SendHTTPRequestAndStreamResponse(hRequest, pCallHandle);
        }

        public bool DeferHTTPRequest(IntPtr _, uint hRequest)
        {
            return SteamEmulator.SteamHTTP.DeferHTTPRequest(hRequest);
        }

        public bool PrioritizeHTTPRequest(IntPtr _, uint hRequest)
        {
            return SteamEmulator.SteamHTTP.PrioritizeHTTPRequest(hRequest);
        }

        public bool GetHTTPResponseHeaderSize(IntPtr _, uint hRequest, string pchHeaderName, uint unResponseHeaderSize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPResponseHeaderSize(hRequest, pchHeaderName, unResponseHeaderSize);
        }

        public bool GetHTTPResponseHeaderValue(IntPtr _, uint hRequest, string pchHeaderName, int pHeaderValueBuffer, uint unBufferSize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPResponseHeaderValue(hRequest, pchHeaderName, pHeaderValueBuffer, unBufferSize);
        }

        public bool GetHTTPResponseBodySize(IntPtr _, uint hRequest, uint unBodySize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPResponseBodySize(hRequest, unBodySize);
        }

        public bool GetHTTPResponseBodyData(IntPtr _, uint hRequest, IntPtr pBodyDataBuffer, uint unBufferSize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPResponseBodyData(hRequest, pBodyDataBuffer, unBufferSize);
        }

        public bool GetHTTPStreamingResponseBodyData(IntPtr _, uint hRequest, uint cOffset, IntPtr pBodyDataBuffer, uint unBufferSize)
        {
            return SteamEmulator.SteamHTTP.GetHTTPStreamingResponseBodyData(hRequest, cOffset, pBodyDataBuffer, unBufferSize);
        }

        public bool ReleaseHTTPRequest(IntPtr _, HTTPRequestHandle hRequest)
        {
            return SteamEmulator.SteamHTTP.ReleaseHTTPRequest(hRequest);
        }

        public bool GetHTTPDownloadProgressPct(IntPtr _, uint hRequest, float pflPercentOut)
        {
            return SteamEmulator.SteamHTTP.GetHTTPDownloadProgressPct(hRequest, pflPercentOut);
        }

        public bool SetHTTPRequestRawPostBody(IntPtr _, uint hRequest, string pchContentType, IntPtr pubBody, uint unBodyLen)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestRawPostBody(hRequest, pchContentType, pubBody, unBodyLen);
        }

        public uint CreateCookieContainer(IntPtr _, bool bAllowResponsesToModify)
        {
            return SteamEmulator.SteamHTTP.CreateCookieContainer(bAllowResponsesToModify);
        }

        public bool ReleaseCookieContainer(IntPtr _, uint hCookieContainer)
        {
            return SteamEmulator.SteamHTTP.ReleaseCookieContainer(hCookieContainer);
        }

        public bool SetCookie(IntPtr _, uint hCookieContainer, string pchHost, string pchUrl, string pchCookie)
        {
            return SteamEmulator.SteamHTTP.SetCookie(hCookieContainer, pchHost, pchUrl, pchCookie);
        }

        public bool SetHTTPRequestCookieContainer(IntPtr _, uint hRequest, uint hCookieContainer)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestCookieContainer(hRequest, hCookieContainer);
        }

        public bool SetHTTPRequestUserAgentInfo(IntPtr _, uint hRequest, string pchUserAgentInfo)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestUserAgentInfo(hRequest, pchUserAgentInfo);
        }

        public bool SetHTTPRequestRequiresVerifiedCertificate(IntPtr _, uint hRequest, bool bRequireVerifiedCertificate)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestRequiresVerifiedCertificate(hRequest, bRequireVerifiedCertificate);
        }

        public bool SetHTTPRequestAbsoluteTimeoutMS(IntPtr _, uint hRequest, uint unMilliseconds)
        {
            return SteamEmulator.SteamHTTP.SetHTTPRequestAbsoluteTimeoutMS(hRequest, unMilliseconds);
        }

        public bool GetHTTPRequestWasTimedOut(IntPtr _, uint hRequest, bool pbWasTimedOut)
        {
            return SteamEmulator.SteamHTTP.GetHTTPRequestWasTimedOut(hRequest, pbWasTimedOut);
        }


    }
}

namespace Steamworks
{
    public struct HTTPRequestHandle : System.IEquatable<HTTPRequestHandle>, System.IComparable<HTTPRequestHandle>
    {
        public static readonly HTTPRequestHandle Invalid = new HTTPRequestHandle(0);
        public uint m_HTTPRequestHandle;

        public HTTPRequestHandle(uint value)
        {
            m_HTTPRequestHandle = value;
        }

        public override string ToString()
        {
            return m_HTTPRequestHandle.ToString();
        }

        public override bool Equals(object other)
        {
            return other is HTTPRequestHandle && this == (HTTPRequestHandle)other;
        }

        public override int GetHashCode()
        {
            return m_HTTPRequestHandle.GetHashCode();
        }

        public static bool operator ==(HTTPRequestHandle x, HTTPRequestHandle y)
        {
            return x.m_HTTPRequestHandle == y.m_HTTPRequestHandle;
        }

        public static bool operator !=(HTTPRequestHandle x, HTTPRequestHandle y)
        {
            return !(x == y);
        }

        public static explicit operator HTTPRequestHandle(uint value)
        {
            return new HTTPRequestHandle(value);
        }

        public static explicit operator uint(HTTPRequestHandle that)
        {
            return that.m_HTTPRequestHandle;
        }

        public bool Equals(HTTPRequestHandle other)
        {
            return m_HTTPRequestHandle == other.m_HTTPRequestHandle;
        }

        public int CompareTo(HTTPRequestHandle other)
        {
            return m_HTTPRequestHandle.CompareTo(other.m_HTTPRequestHandle);
        }
    }
}