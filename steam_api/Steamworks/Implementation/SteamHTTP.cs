using System;
using Core.Interface;
using SKYNET;


//[Map("STEAMHTTP_INTERFACE_VERSION")]
//[Map("SteamHTTP")]
public class SteamHTTP : IBaseInterface
{
    public uint CreateCookieContainer(bool bAllowResponsesToModify)
    {
        return 0;
    }

    public IntPtr CreateHTTPRequest(uint eHTTPRequestMethod, string pchAbsoluteURL)
    {
        return IntPtr.Zero;
    }

    public bool DeferHTTPRequest(uint hRequest)
    {
        return true;
    }

    public bool GetHTTPDownloadProgressPct(uint hRequest, ref float pflPercentOut)
    {
        return true;
    }

    public bool GetHTTPRequestWasTimedOut(uint hRequest, ref bool pbWasTimedOut)
    {
        return true;
    }

    public bool GetHTTPResponseBodyData(uint hRequest, IntPtr pBodyDataBuffer, uint unBufferSize)
    {
        return true;
    }

    public bool GetHTTPResponseBodySize(uint hRequest, ref uint unBodySize)
    {
        return true;
    }

    public bool GetHTTPResponseHeaderSize(uint hRequest, string pchHeaderName, ref uint unResponseHeaderSize)
    {
        return true;
    }

    public bool GetHTTPResponseHeaderValue(uint hRequest, string pchHeaderName, IntPtr pHeaderValueBuffer, uint unBufferSize)
    {
        return true;
    }

    public bool GetHTTPStreamingResponseBodyData(uint hRequest, uint cOffset, IntPtr pBodyDataBuffer, uint unBufferSize)
    {
        return true;
    }

    public bool PrioritizeHTTPRequest(uint hRequest)
    {
        return true;
    }

    public bool ReleaseCookieContainer(uint hCookieContainer)
    {
        return true;
    }

    public bool ReleaseHTTPRequest(uint hRequest)
    {
        return true;
    }

    public bool SendHTTPRequest(uint hRequest, ref ulong pCallHandle)
    {
        return true;
    }

    public bool SendHTTPRequestAndStreamResponse(uint hRequest, ref ulong pCallHandle)
    {
        return true;
    }

    public bool SetCookie(uint hCookieContainer, string pchHost, string pchUrl, string pchCookie)
    {
        return true;
    }

    public bool SetHTTPRequestAbsoluteTimeoutMS(uint hRequest, uint unMilliseconds)
    {
        return true;
    }

    public bool SetHTTPRequestContextValue(uint hRequest, ulong ulContextValue)
    {
        return true;
    }

    public bool SetHTTPRequestCookieContainer(uint hRequest, uint hCookieContainer)
    {
        return true;
    }

    public bool SetHTTPRequestGetOrPostParameter(uint hRequest, string pchParamName, string pchParamValue)
    {
        return true;
    }

    public bool SetHTTPRequestHeaderValue(uint hRequest, string pchHeaderName, string pchHeaderValue)
    {
        return true;
    }

    public bool SetHTTPRequestNetworkActivityTimeout(uint hRequest, uint unTimeoutSeconds)
    {
        return true;
    }

    public bool SetHTTPRequestRawPostBody(uint hRequest, string pchContentType, IntPtr pubBody, uint unBodyLen)
    {
        return true;
    }

    public bool SetHTTPRequestRequiresVerifiedCertificate(uint hRequest, bool bRequireVerifiedCertificate)
    {
        return true;
    }

    public bool SetHTTPRequestUserAgentInfo(uint hRequest, string pchUserAgentInfo)
    {
        return true;
    }

    public IntPtr SteamAPI_SteamGameServerHTTP_v003(IntPtr _)
    {
        return IntPtr.Zero;
    }

    private void Write(string v)
    {
        Main.Write(InterfaceVersion, v);
    }
}