using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Managers;
using System.Net;
using System.Net.Http;
using Steamworks;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamHTTP : ISteamInterface
    {
        private List<HTTPRequest> HTTPRequests;
        private uint Handle;
        public SteamHTTP()
        {
            InterfaceVersion = "SteamHTTP";
            HTTPRequests = new List<HTTPRequest>();
            Handle = 0;
        }

        public uint CreateCookieContainer(bool bAllowResponsesToModify)
        {
            Write($"CreateCookieContainer");
            return 0;
        }

        public HTTPRequestHandle CreateHTTPRequest(uint eHTTPRequestMethod, string pchAbsoluteURL)
        {
            Write($"CreateHTTPRequest {(HTTPMethod)eHTTPRequestMethod} {pchAbsoluteURL}");

            var CreatedHandle = (HTTPRequestHandle)Handle;
            Handle++;

            HTTPRequest HttpRequest = new HTTPRequest();
            HttpRequest.URL = pchAbsoluteURL;
            HttpRequest.RequestMethod = (HTTPMethod)eHTTPRequestMethod;
            HttpRequest.Handle = CreatedHandle;
            HttpRequest.ContextValue = 0;

            HTTPRequests.Add(HttpRequest);
            return CreatedHandle;
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

        public bool ReleaseHTTPRequest(HTTPRequestHandle hRequest)
        {
            Write($"ReleaseHTTPRequest, Header: {hRequest}");
            for (int i = 0; i < HTTPRequests.Count; i++)
            {
                if (HTTPRequests[i].Handle == hRequest)
                {
                    HTTPRequests.RemoveAt(i);
                    Write("---------------------------------------- removed");
                    return true;
                }
            }
            return false;
        }

        public bool SendHTTPRequest(HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle)
        {
            Write($"SendHTTPRequest, Handle: {hRequest}");

            HTTPRequest request = HTTPRequests.Find(r => r.Handle == hRequest);
            if (request == null)
            {
                return false;
            }

            HTTPRequestCompleted_t data = new HTTPRequestCompleted_t()
            {
                m_hRequest = request.Handle
            };

            try
            {
                WebRequest webrequest = HttpWebRequest.Create(request.URL);
                webrequest.Method = request.RequestMethod.ToString();
                HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();
                request.Response = content;

                data.m_ulContextValue = request.ContextValue;
                data.m_bRequestSuccessful = true;
                data.m_eStatusCode = (EHTTPStatusCode)response.StatusCode;
                data.m_unBodySize = (uint)content.Length;
            }
            catch (Exception ex)
            {
                data.m_ulContextValue = request.ContextValue;
                data.m_bRequestSuccessful = false;
                data.m_eStatusCode = EHTTPStatusCode.k_EHTTPStatusCode404NotFound;
                data.m_unBodySize = 0;
            }

            //pCallHandle = CallbackManager.AddCallbackResult(data);
            pCallHandle = new SteamAPICall_t(CallbackType.k_iHTTPRequestCompleted);
            return true;
        }

        // Sends the HTTP request, will return false on a bad handle, otherwise use SteamCallHandle to wait on
        // asynchronous response via callback for completion, and listen for HTTPRequestHeadersReceived_t and 
        // HTTPRequestDataReceived_t callbacks while streaming.
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

        public bool SetHTTPRequestHeaderValue(HTTPRequestHandle hRequest, string pchHeaderName, string pchHeaderValue)
        {
            Write($"SetHTTPRequestHeaderValue, Handle {hRequest}");
            HTTPRequest request = HTTPRequests.Find(r => r.Handle == hRequest);
            if (request == null)
            {
                return false;
            }

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
    public class HTTPRequest
    {
        public HTTPRequestHandle Handle;
        public ulong ContextValue;
        public string Response;
        public HTTPMethod RequestMethod;
        public string URL;
    }
}