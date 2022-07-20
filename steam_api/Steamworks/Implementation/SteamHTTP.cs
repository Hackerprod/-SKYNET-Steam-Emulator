using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using SKYNET.Callback;
using SKYNET.Steamworks.Interfaces;
using SKYNET.Managers;

using SteamAPICall_t = System.UInt64;
using HTTPRequestHandle = System.UInt32;
using HTTPCookieContainerHandle = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamHTTP : ISteamInterface
    {
        public static SteamHTTP Instance;

        private List<HTTPRequest> HTTPRequests;
        private uint Handle;

        public SteamHTTP()
        {
            Instance = this;
            InterfaceName = "SteamHTTP";
            InterfaceVersion = "STEAMHTTP_INTERFACE_VERSION003";
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

            var CreatedHandle = Handle;
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
            var Result = false;
            try
            {
                HTTPRequest request = GetHTTPRequest(hRequest);
                if (request == null)
                {
                    Write($"SendHTTPRequest, Not found request for Handle: {hRequest}");
                    return false;
                }
                HTTPRequestCompleted_t data = new HTTPRequestCompleted_t()
                {
                    Request = (uint)request.Handle
                };

                var APIRequest = new RequestHTTPAPI()
                {
                    HTTPRequestCompleted = data,
                    HTTPRequestHandle = request.Handle,
                };

                pCallHandle = CallbackManager.AddCallbackResult(data, false);
                APIRequest.SteamAPICall = pCallHandle;

                WebRequest webrequest = WebRequest.Create(request.URL);
                webrequest.ContentType = request.ContentType;
                webrequest.Method = request.RequestMethod.ToString();
                if (request.RequestMethod == HTTPMethod.POST)
                {
                    // TODO: Write raw into Request stream
                }

                RequestState RequestState = new RequestState()
                {
                    Request = webrequest,
                    HTTPRequest = APIRequest
                };

                webrequest.BeginGetResponse(FinishWebRequest, RequestState);

                Result = true;
            }
            catch { }

            Write($"SendHTTPRequest (HTTPRequestHandle = {hRequest}) = {Result}");

            return Result;
        }

        void FinishWebRequest(IAsyncResult ar)
        {
            RequestState RequestState = (RequestState)ar.AsyncState;
            HTTPRequestCompleted_t data = RequestState.HTTPRequest.HTTPRequestCompleted;
            HTTPRequest request = GetHTTPRequest(RequestState.HTTPRequest.HTTPRequestHandle);

            try
            {

                // Get the WebRequest from RequestState.  
                var webRequest = RequestState.Request;
                var webResponse = (HttpWebResponse)webRequest.EndGetResponse(ar);
                //Stream ResponseStream = resp.GetResponseStream();
                var reader = new StreamReader(webResponse.GetResponseStream());
                var content = reader.ReadToEnd();

                data.ContextValue = request.ContextValue;
                data.RequestSuccessful = true;
                data.StatusCode = (HTTPStatusCode)webResponse.StatusCode;
                data.BodySize = (uint)content.Length;
            }
            catch (Exception ex)
            {
                data.ContextValue = request.ContextValue;
                data.RequestSuccessful = false;
                data.StatusCode = HTTPStatusCode.Code404NotFound;
                data.BodySize = 0;
            }

            if (CallbackManager.GetCallResult(RequestState.HTTPRequest.SteamAPICall, out var callback))
            {
                callback.Data = data;
                callback.ReadyToCall = true;
            }
        }

        private void SendRequest(object state)
        {
            Write("XxxxxxxxxxxxxxxxxxxxxxxxxxxxX");
            RequestHTTPAPI RequestAPI = (RequestHTTPAPI)state;
            HTTPRequestCompleted_t data = RequestAPI.HTTPRequestCompleted;
            HTTPRequest request = GetHTTPRequest(RequestAPI.HTTPRequestHandle);
            if (request == null) return;

            try
            {
                WebRequest webrequest = WebRequest.Create(request.URL);
                webrequest.Method = request.RequestMethod.ToString();
                HttpWebResponse response = (HttpWebResponse)webrequest.BeginGetResponse(FinishWebRequest, RequestAPI);
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();
                request.Response = content;
                Write(content);

                data.ContextValue = request.ContextValue;
                data.RequestSuccessful = true;
                data.StatusCode = (HTTPStatusCode)response.StatusCode;
                data.BodySize = (uint)content.Length;
            }
            catch (Exception ex)
            {
                data.ContextValue = request.ContextValue;
                data.RequestSuccessful = false;
                data.StatusCode = HTTPStatusCode.Code404NotFound;
                data.BodySize = 0;
                Write(ex);
            }

            if (CallbackManager.GetCallResult(RequestAPI.SteamAPICall, out var callback))
            {
                callback.Data = data;
                callback.ReadyToCall = true;
            }
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
            var Result = false;
            HTTPRequest request = GetHTTPRequest(hRequest);
            if (request != null)
            {
                request.ContextValue = ulContextValue;
                Result = true;
            }
            Write($"SetHTTPRequestContextValue (HTTPRequestHandle = {hRequest}, ContextValue = {ulContextValue}) = {Result}");
            return Result; 
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
            HTTPRequest request = GetHTTPRequest(hRequest);
            if (request == null)
            {
                return false;
            }
            return true;
        }

        public bool SetHTTPRequestNetworkActivityTimeout(HTTPRequestHandle hRequest, uint unTimeoutSeconds)
        {
            Write($"SetHTTPRequestNetworkActivityTimeout (HTTPRequestHandle = {hRequest}, TimeoutSeconds = {unTimeoutSeconds})");
            HTTPRequest request = GetHTTPRequest(hRequest);
            if (request != null)
            {
                request.TimeoutSeconds = unTimeoutSeconds;
                return true;
            }
            return false;
        }

        public bool SetHTTPRequestRawPostBody(HTTPRequestHandle hRequest, string pchContentType, IntPtr pubBody, uint unBodyLen)
        {
            Write($"SetHTTPRequestRawPostBody (HTTPRequestHandle = {hRequest}, ContentType = {pchContentType})");

            HTTPRequest request = GetHTTPRequest(hRequest);
            if (request == null)
            {
                Write($"SendHTTPRequest, Not found request for Handle: {hRequest}");
                return false;
            }

            byte[] Body = new byte[unBodyLen];
            Marshal.Copy(pubBody, Body, 0, (int)unBodyLen);

            request.RawPostBody = Body;
            request.ContentType = pchContentType;

            Write(Encoding.Default.GetString(Body));

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

        private HTTPRequest GetHTTPRequest(HTTPRequestHandle hTTPRequestHandle)
        {
            return HTTPRequests.Find(r => r.Handle == hTTPRequestHandle);
        }

        private class RequestHTTPAPI
        {
            public HTTPRequestCompleted_t HTTPRequestCompleted { get; set; }
            public SteamAPICall_t SteamAPICall { get; set; }
            public HTTPRequestHandle HTTPRequestHandle { get; set; }
        }

        private class HTTPRequest
        {
            public HTTPRequestHandle Handle;
            public ulong ContextValue;
            public string Response;
            public HTTPMethod RequestMethod;
            public string URL;
            public uint TimeoutSeconds;
            public byte[] RawPostBody;
            public string ContentType;
        }

        private class RequestState
        {
            public RequestHTTPAPI HTTPRequest;
            public WebRequest Request;
        }
    }
}