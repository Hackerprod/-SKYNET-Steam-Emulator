using System;
using SKYNET.Managers;
using System.Net;
using System.Net.Http;
using Steamworks;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using SKYNET.Callback;

using SteamAPICall_t = System.UInt64;
using HTTPRequestHandle = System.UInt32;
using HTTPCookieContainerHandle = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamHTTP : ISteamInterface
    {
        private List<HTTPRequest> HTTPRequests;
        private uint Handle;
        private SteamAPICall_t k_uAPICallInvalid = 0x0;

        public SteamHTTP()
        {
            InterfaceName = "SteamHTTP";
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

        public bool DeferHTTPRequest(HTTPRequestHandle hRequest)
        {
            Write($"DeferHTTPRequest");
            return true;
        }

        public bool GetHTTPDownloadProgressPct(HTTPRequestHandle hRequest, float pflPercentOut)
        {
            Write($"GetHTTPDownloadProgressPct");
            return true;
        }

        public bool GetHTTPRequestWasTimedOut(HTTPRequestHandle hRequest, bool pbWasTimedOut)
        {
            Write($"GetHTTPRequestWasTimedOut");
            return true;
        }

        public bool GetHTTPResponseBodyData(HTTPRequestHandle hRequest, IntPtr pBodyDataBuffer, uint unBufferSize)
        {
            Write($"GetHTTPResponseBodyData");
            return true;
        }

        public bool GetHTTPResponseBodySize(HTTPRequestHandle hRequest, uint unBodySize)
        {
            Write($"GetHTTPResponseBodySize");
            return true;
        }

        public bool GetHTTPResponseHeaderSize(HTTPRequestHandle hRequest, string pchHeaderName, uint unResponseHeaderSize)
        {
            Write($"GetHTTPResponseHeaderSize");
            return true;
        }

        public bool GetHTTPResponseHeaderValue(HTTPRequestHandle hRequest, string pchHeaderName, int pHeaderValueBuffer, uint unBufferSize)
        {
            Write($"GetHTTPResponseHeaderValue");
            return true;
        }

        public bool GetHTTPStreamingResponseBodyData(HTTPRequestHandle hRequest, uint cOffset, IntPtr pBodyDataBuffer, uint unBufferSize)
        {
            Write($"GetHTTPStreamingResponseBodyData");
            return true;
        }

        public bool PrioritizeHTTPRequest(HTTPRequestHandle hRequest)
        {
            Write($"PrioritizeHTTPRequest");
            return true;
        }

        public bool ReleaseCookieContainer(HTTPCookieContainerHandle hCookieContainer)
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
                Write($"SendHTTPRequest, Not found request for Handle: {hRequest}");
                return false;
            }

            HTTPRequestCompleted_t data = new HTTPRequestCompleted_t()
            {
                Request = (uint)request.Handle
            };

            //try
            //{
            //    WebRequest webrequest = HttpWebRequest.Create(request.URL);
            //    webrequest.Method = request.RequestMethod.ToString();
            //    HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse();
            //    StreamReader reader = new StreamReader(response.GetResponseStream());
            //    string content = reader.ReadToEnd();
            //    request.Response = content;

            //    data.ContextValue = request.ContextValue;
            //    data.RequestSuccessful = true;
            //    data.StatusCode = (HTTPStatusCode)response.StatusCode;
            //    data.BodySize = (uint)content.Length;
            //}
            //catch (Exception ex)
            {
                data.ContextValue = request.ContextValue;
                data.RequestSuccessful = false;
                data.StatusCode = HTTPStatusCode.Code404NotFound;
                data.BodySize = 0;
            }

            pCallHandle = CallbackManager.AddCallbackResult(data);
            return true;
        }

        // Sends the HTTP request, will return false on a bad handle, otherwise use SteamCallHandle to wait on
        // asynchronous response via callback for completion, and listen for HTTPRequestHeadersReceived_t and 
        // HTTPRequestDataReceived_t callbacks while streaming.
        public bool SendHTTPRequestAndStreamResponse(HTTPRequestHandle hRequest, SteamAPICall_t pCallHandle)
        {
            Write($"SendHTTPRequestAndStreamResponse");
            return true;
        }

        public bool SetCookie(HTTPCookieContainerHandle hCookieContainer, string pchHost, string pchUrl, string pchCookie)
        {
            Write($"SetCookie");
            return true;
        }

        public bool SetHTTPRequestAbsoluteTimeoutMS(HTTPRequestHandle hRequest, uint unMilliseconds)
        {
            Write($"SetHTTPRequestAbsoluteTimeoutMS");
            return true;
        }

        public bool SetHTTPRequestContextValue(HTTPRequestHandle hRequest, ulong ulContextValue)
        {
            Write($"SetHTTPRequestContextValue");
            HTTPRequest request = HTTPRequests.Find(r => r.Handle == hRequest);
            if (request == null)
            {
                return false;
            }
            request.ContextValue = ulContextValue;
            return true; 
        }

        public bool SetHTTPRequestCookieContainer(HTTPRequestHandle hRequest, HTTPCookieContainerHandle hCookieContainer)
        {
            Write($"SetHTTPRequestCookieContainer");
            return true;
        }

        public bool SetHTTPRequestGetOrPostParameter(HTTPRequestHandle hRequest, string pchParamName, string pchParamValue)
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

        public bool SetHTTPRequestNetworkActivityTimeout(HTTPRequestHandle hRequest, uint unTimeoutSeconds)
        {
            Write($"SetHTTPRequestNetworkActivityTimeout");
            return true;
        }

        public bool SetHTTPRequestRawPostBody(HTTPRequestHandle hRequest, string pchContentType, IntPtr pubBody, uint unBodyLen)
        {
            Write($"SetHTTPRequestRawPostBody");
            return true;
        }

        public bool SetHTTPRequestRequiresVerifiedCertificate(HTTPRequestHandle hRequest, bool bRequireVerifiedCertificate)
        {
            Write($"SetHTTPRequestRequiresVerifiedCertificate");
            return true;
        }

        public bool SetHTTPRequestUserAgentInfo(HTTPRequestHandle hRequest, string pchUserAgentInfo)
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