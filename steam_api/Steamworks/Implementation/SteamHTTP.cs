using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
            Handle = 1;
        }

        public uint CreateCookieContainer(bool bAllowResponsesToModify)
        {
            Write($"CreateCookieContainer");
            return 0;
        }

        public HTTPRequestHandle CreateHTTPRequest(uint eHTTPRequestMethod, string pchAbsoluteURL)
        {
            var CreatedHandle = Handle;
            Handle++;

            HTTPRequest HttpRequest = new HTTPRequest();
            HttpRequest.URL = pchAbsoluteURL;
            HttpRequest.RequestMethod = (HTTPMethod)eHTTPRequestMethod;
            HttpRequest.Handle = CreatedHandle;
            HttpRequest.ContextValue = 0;

            HTTPRequests.Add(HttpRequest);
            Write($"CreateHTTPRequest handle={CreatedHandle} method={(HTTPMethod)eHTTPRequestMethod} url={pchAbsoluteURL} active=[{DescribeActiveHandles()}]");
            return CreatedHandle;
        }

        public bool DeferHTTPRequest(HTTPRequestHandle hRequest)
        {
            Write($"DeferHTTPRequest");
            return true;
        }

        public bool GetHTTPDownloadProgressPct(HTTPRequestHandle hRequest, IntPtr pflPercentOut)
        {
            Write($"GetHTTPDownloadProgressPct");
            if (pflPercentOut != IntPtr.Zero)
            {
                var bytes = BitConverter.GetBytes(1.0f);
                Marshal.Copy(bytes, 0, pflPercentOut, bytes.Length);
            }
            return GetHTTPRequest(hRequest) != null;
        }

        public bool GetHTTPDownloadProgressPct(HTTPRequestHandle hRequest, ref float pflPercentOut)
        {
            pflPercentOut = 1.0f;
            return GetHTTPRequest(hRequest) != null;
        }

        public bool GetHTTPRequestWasTimedOut(HTTPRequestHandle hRequest, IntPtr pbWasTimedOut)
        {
            Write($"GetHTTPRequestWasTimedOut");
            if (pbWasTimedOut != IntPtr.Zero)
            {
                Marshal.WriteByte(pbWasTimedOut, 0);
            }
            return GetHTTPRequest(hRequest) != null;
        }

        public bool GetHTTPRequestWasTimedOut(HTTPRequestHandle hRequest, ref bool pbWasTimedOut)
        {
            pbWasTimedOut = false;
            return GetHTTPRequest(hRequest) != null;
        }

        public bool GetHTTPResponseBodyData(HTTPRequestHandle hRequest, IntPtr pBodyDataBuffer, uint unBufferSize)
        {
            var request = GetHTTPRequest(hRequest);
            if (request == null)
            {
                Write($"GetHTTPResponseBodyData handle={hRequest} not found active=[{DescribeActiveHandles()}]");
                return false;
            }

            byte[] response = request.ResponseBytes ?? Array.Empty<byte>();
            Write($"GetHTTPResponseBodyData handle={hRequest} buffer={unBufferSize} body={response.Length} preview={PreviewBody(response)}");
            if (response.Length > unBufferSize)
            {
                return false;
            }

            if (response.Length > 0 && pBodyDataBuffer != IntPtr.Zero)
            {
                Marshal.Copy(response, 0, pBodyDataBuffer, response.Length);
            }
            return true;
        }

        public bool GetHTTPResponseBodySize(HTTPRequestHandle hRequest, IntPtr unBodySize)
        {
            var request = GetHTTPRequest(hRequest);
            if (request == null)
            {
                Write($"GetHTTPResponseBodySize handle={hRequest} not found active=[{DescribeActiveHandles()}]");
                return false;
            }

            if (unBodySize != IntPtr.Zero)
            {
                byte[] response = request.ResponseBytes ?? Array.Empty<byte>();
                Marshal.WriteInt32(unBodySize, response.Length);
                Write($"GetHTTPResponseBodySize handle={hRequest} body={response.Length}");
            }
            return true;
        }

        public bool GetHTTPResponseBodySize(HTTPRequestHandle hRequest, ref uint unBodySize)
        {
            var request = GetHTTPRequest(hRequest);
            if (request == null)
            {
                Write($"GetHTTPResponseBodySize(ref) handle={hRequest} not found active=[{DescribeActiveHandles()}]");
                return false;
            }

            byte[] response = request.ResponseBytes ?? Array.Empty<byte>();
            unBodySize = (uint)response.Length;
            Write($"GetHTTPResponseBodySize(ref) handle={hRequest} body={response.Length}");
            return true;
        }

        public bool GetHTTPResponseHeaderSize(HTTPRequestHandle hRequest, string pchHeaderName, IntPtr unResponseHeaderSize)
        {
            var request = GetHTTPRequest(hRequest);
            string headerValue;
            if (request != null && TryGetResponseHeader(request, pchHeaderName, out headerValue))
            {
                if (unResponseHeaderSize != IntPtr.Zero)
                {
                    Marshal.WriteInt32(unResponseHeaderSize, Encoding.UTF8.GetByteCount(headerValue) + 1);
                }

                Write($"GetHTTPResponseHeaderSize handle={hRequest} header={pchHeaderName} size={Encoding.UTF8.GetByteCount(headerValue) + 1}");
                return true;
            }

            if (unResponseHeaderSize != IntPtr.Zero)
            {
                Marshal.WriteInt32(unResponseHeaderSize, 0);
            }

            Write($"GetHTTPResponseHeaderSize handle={hRequest} header={pchHeaderName} not found");
            return false;
        }

        public bool GetHTTPResponseHeaderSize(HTTPRequestHandle hRequest, string pchHeaderName, ref uint unResponseHeaderSize)
        {
            var request = GetHTTPRequest(hRequest);
            string headerValue;
            if (request != null && TryGetResponseHeader(request, pchHeaderName, out headerValue))
            {
                unResponseHeaderSize = (uint)Encoding.UTF8.GetByteCount(headerValue) + 1;
                return true;
            }

            unResponseHeaderSize = 0;
            return false;
        }

        public bool GetHTTPResponseHeaderValue(HTTPRequestHandle hRequest, string pchHeaderName, IntPtr pHeaderValueBuffer, uint unBufferSize)
        {
            var request = GetHTTPRequest(hRequest);
            string headerValue;
            if (request == null || !TryGetResponseHeader(request, pchHeaderName, out headerValue))
            {
                Write($"GetHTTPResponseHeaderValue handle={hRequest} header={pchHeaderName} not found");
                return false;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(headerValue);
            if (pHeaderValueBuffer == IntPtr.Zero || unBufferSize < bytes.Length + 1)
            {
                Write($"GetHTTPResponseHeaderValue handle={hRequest} header={pchHeaderName} buffer={unBufferSize} needed={bytes.Length + 1}");
                return false;
            }

            Marshal.Copy(bytes, 0, pHeaderValueBuffer, bytes.Length);
            Marshal.WriteByte(pHeaderValueBuffer, bytes.Length, 0);
            Write($"GetHTTPResponseHeaderValue handle={hRequest} header={pchHeaderName} value={headerValue}");
            return true;
        }

        public bool GetHTTPResponseHeaderValue(HTTPRequestHandle hRequest, string pchHeaderName, int pHeaderValueBuffer, uint unBufferSize)
        {
            return GetHTTPResponseHeaderValue(hRequest, pchHeaderName, (IntPtr)pHeaderValueBuffer, unBufferSize);
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
            Write($"ReleaseHTTPRequest, Header: {hRequest} active-before=[{DescribeActiveHandles()}]");
            for (int i = 0; i < HTTPRequests.Count; i++)
            {
                if (HTTPRequests[i].Handle == hRequest)
                {
                    HTTPRequests.RemoveAt(i);
                    Write($"ReleaseHTTPRequest removed handle={hRequest} active-after=[{DescribeActiveHandles()}]");
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
                    Request = (uint)request.Handle,
                    ContextValue = request.ContextValue,
                    RequestSuccessful = true,
                    StatusCode = HTTPStatusCode.Code200OK,
                    BodySize = 0
                };

                var APIRequest = new RequestHTTPAPI()
                {
                    HTTPRequestCompleted = data,
                    HTTPRequestHandle = request.Handle,
                };

                bool blockSdrConfig = IsSdrConfigRequest(request.URL) &&
                    !SteamNetworkingSocketsSerialized.SecureCertMode;
                if (blockSdrConfig)
                {
                    request.ResponseBytes = Array.Empty<byte>();
                    data.RequestSuccessful = false;
                    data.StatusCode = HTTPStatusCode.Code404NotFound;
                    Write("SendHTTPRequest: SDR config disabled in insecure LAN mode");
                }
                else
                {
                    request.ResponseBytes = BuildLocalResponseBytes(request);
                }

                request.ResponseHeaders = BuildLocalResponseHeaders(request);
                data.BodySize = (uint)request.ResponseBytes.Length;
                if (IsSdrConfigRequest(request.URL))
                {
                    pCallHandle = WorkQueue.EnqueueCallbackResult(
                        data,
                        () =>
                        {
                            Thread.Sleep(250);
                            return data;
                        },
                        false,
                        "SendHTTPRequest:GetSDRConfig",
                        null,
                        true);
                }
                else
                {
                    pCallHandle = CallbackManager.AddCallbackResult(data);
                }
                APIRequest.SteamAPICall = pCallHandle;
                Write($"SendHTTPRequest prepared handle={hRequest} call={pCallHandle} url={request.URL} context={data.ContextValue} success={data.RequestSuccessful} status={(int)data.StatusCode} body={data.BodySize} preview={PreviewBody(request.ResponseBytes)} active=[{DescribeActiveHandles()}]");

                //WebRequest webrequest = WebRequest.Create(request.URL);
                //webrequest.Timeout = (int)(request.TimeoutSeconds != 0 ? request.TimeoutSeconds : 2);
                //webrequest.ContentType = request.ContentType;
                //webrequest.Method = request.RequestMethod.ToString();
                //if (request.RequestMethod == HTTPMethod.POST)
                //{
                //    // TODO: Write raw into Request stream
                //}

                //RequestState RequestState = new RequestState()
                //{
                //    Request = webrequest,
                //    HTTPRequest = APIRequest
                //};

                //webrequest.BeginGetResponse(FinishWebRequest, RequestState);

                Result = true;
            }
            catch (Exception ex)
            {
                Write($"SendHTTPRequest handle={hRequest} failed: {ex.Message}");
            }

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

            //if (CallbackManager.GetCallResult(RequestState.HTTPRequest.SteamAPICall, out var callback))
            //{
            //    callback.Data = data;
            //    callback.ReadyToCall = true;
            //}
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

            //if (CallbackManager.GetCallResult(RequestAPI.SteamAPICall, out var callback))
            //{
            //    callback.Data = data;
            //    callback.ReadyToCall = true;
            //}
        }


        // Sends the HTTP request, will return false on a bad handle, otherwise use SteamCallHandle to wait on
        // asynchronous response via callback for completion, and listen for HTTPRequestHeadersReceived_t and 
        // HTTPRequestDataReceived_t callbacks while streaming.
        public bool SendHTTPRequestAndStreamResponse(HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle)
        {
            Write($"SendHTTPRequestAndStreamResponse");
            return SendHTTPRequest(hRequest, ref pCallHandle);
        }

        public bool SendHTTPRequestAndStreamResponse(HTTPRequestHandle hRequest, SteamAPICall_t pCallHandle)
        {
            return SendHTTPRequestAndStreamResponse(hRequest, ref pCallHandle);
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
            Write($"SetHTTPRequestHeaderValue handle={hRequest} name={pchHeaderName ?? "<null>"} value={pchHeaderValue ?? "<null>"}");
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

        private string DescribeActiveHandles()
        {
            if (HTTPRequests == null || HTTPRequests.Count == 0)
            {
                return "";
            }

            var handles = new string[HTTPRequests.Count];
            for (int i = 0; i < HTTPRequests.Count; i++)
            {
                handles[i] = HTTPRequests[i].Handle.ToString();
            }

            return string.Join(",", handles);
        }

        // Full SDR network config modelled on the reference coordinator's
        // GetSDRConfig endpoint, edited with our own relay address. The certs
        // are CertTool-style CA certs regenerated for the emulator CA patched
        // into steamnetworkingsockets.dll, not the old 2021 AE8D root.
        private static readonly string[] SdrConfigCerts =
        {
            "Ii4IARIg/qqXwyx+W/aE34bxIPPEDHhdzs3ty5H8Ij5U52qjD1lFPmRVak2+lzZsKfX9QGzqjxvNMkA+v31mfE0jEauJQR50GDcixRlwb+hcaZzjP4Rf8o4cZJdyASZ0sTm/cxo84lz6OIvu71W69ufkS/+s62qgGGoD",
            "IjwIARIg/qqXwyx+W/aE34bxIPPEDHhdzs3ty5H8Ij5U52qjD1lFPmRVak2+lzZsUJrVEFCO0iNQ+pQ+UH4p9f1AbOqPG80yQDeIEI62KYx3jxivmSsn8+ZCR6G5Cx4OykN2+j7vqmDXyI/ixPXzWQ2IN/0Vei9LwKQrR4D1ZqKNXFmJv9OIRQ0=",
            "IjIIARIg/qqXwyx+W/aE34bxIPPEDHhdzs3ty5H8Ij5U52qjD1lFPmRVak2+lzZsULLKNCn1/UBs6o8bzTJAGHXwo6qT26fP4hQ9ZBC6EY+GsZpMHXSqXCKJeKw1bPkAzSp8lozfP8HXOGDFxsWRCBkmEt6iK29GVnJfM3GZAQ==",
            "IjIIARIg/qqXwyx+W/aE34bxIPPEDHhdzs3ty5H8Ij5U52qjD1lFPmRVak2+lzZsUNiiQSn1/UBs6o8bzTJA4GeaOYoTnj1dPxfowErcoUqiQ9Y1dreTq80k61isymuqh6jXlSrSb9UxWOYiHeU4EFS0RMyK46yakMe9YvERAg==",
            "IjYIARIg/qqXwyx+W/aE34bxIPPEDHhdzs3ty5H8Ij5U52qjD1lFPmRVak2+lzZsUKSZFVDcoUIp9f1AbOqPG80yQAWnjxLJBPsIscy+QBX2IAxn3nW8qesT5/6yWIcxeQ5UAMSreqBk9OxkaWb0JDbV9hED21gMFwwSwAVXS7w57wg=",
            "IjYIARIg/qqXwyx+W/aE34bxIPPEDHhdzs3ty5H8Ij5U52qjD1lFPmRVak2+lzZsUJrWE1CK+BQp9f1AbOqPG80yQPe6oyKSTCwaqh7CFm3NPpmy+F0v0PwUtIBSw72LfcJXE6gsHdJStyKNSWHzwaQU3fQvSCW4mAsSHRNN9ACZZws=",
            "IjYIARIg/qqXwyx+W/aE34bxIPPEDHhdzs3ty5H8Ij5U52qjD1lFPmRVak2+lzZsUKSZFVDcoUIp9f1AbOqPG80yQAWnjxLJBPsIscy+QBX2IAxn3nW8qesT5/6yWIcxeQ5UAMSreqBk9OxkaWb0JDbV9hED21gMFwwSwAVXS7w57wg=",
            "IjcIARIg/qqXwyx+W/aE34bxIPPEDHhdzs3ty5H8Ij5U52qjD1lFPmRVak2+lzZsULgDUIgEUKoGKfX9QGzqjxvNMkDdtqMy/lVDuntmqf87orV0++4pcO4EJw5srMil3F1J98UfakM4fJWlQSPcsROyEKPGZzKzGb+Xu1jvIBUP3wEF",
            "IjoIARIg/qqXwyx+W/aE34bxIPPEDHhdzs3ty5H8Ij5U52qjD1lFPmRVak2+lzZsUKLCPlCS8z9Qmr5DKfX9QGzqjxvNMkBUzZfJdXLs22BjNUUv/GgJCmLluyG/L7qavF2S6YxAKVLJ0MNRBSVIIA7ffyH/PWHwuuMtlmImrQ/Jy1T9BIUM",
            "IjsIARIg/qqXwyx+W/aE34bxIPPEDHhdzs3ty5H8Ij5U52qjD1lFPmRVak2+lzZsUMYFUNAFUNoFUJixECn1/UBs6o8bzTJAYSjeD9rxx1Ebye6SUwvQEMLrkSkNQnwlCd7/zLdgiRnOpbCEsSr8XmCwE+7MbNqq4VVBSREvGwxkX7Ec/reMAQ==",
            "Ij4IARIg/qqXwyx+W/aE34bxIPPEDHhdzs3ty5H8Ij5U52qjD1lFPmRVak2+lzZsULoEULAGUICKD1DexwxQZin1/UBs6o8bzTJAqPgvi4s+yjuARGZbWm33QDZzeBIc0H8PxCIRGgbhzlAMgwaTm9K2Sfs454w8joKgKLOyoc6Zw8kWhsngUZW7Bg==",
            "Ij4IARIg/qqXwyx+W/aE34bxIPPEDHhdzs3ty5H8Ij5U52qjD1lFPmRVak2+lzZsUOyjHFDtoxxQ7qMcULLkHin1/UBs6o8bzTJAfHCetKqWilvc89+cwmkVh5wHJ9RqWyfqzo7fnYOObrz+5iPAN97T0Lohsu4GfzPTPftpcJ+OlmMgxUOjcW/YCg=="
        };

        internal static readonly string SdrConfigJson = @"{""revision"":1783546117,""pops"":{""sky"":{""desc"":""SKYNET"",""geo"":[-118.25,34.05],""partners"":1,""tier"":0,""relays"":[{""ipv4"":""10.11.49.120"",""port_range"":[28009,28009]}]}},""certs"":[""" +
            string.Join(@""",""", SdrConfigCerts) +
            @"""],""p2p_share_ip"":{""cn"":20,""default"":40,""ru"":20},""relay_public_key"":""5AC884C1045BA0FF44142AC8DCA51B8A98C8F1CB4FEE36284AFBE92FCF594932"",""revoked_keys"":[],""typical_pings"":[],""success"":true}";

        private static byte[] BuildLocalResponseBytes(HTTPRequest request)
        {
            string body = BuildLocalResponseBody(request.URL);
            return Encoding.UTF8.GetBytes(body);
        }

        private static bool IsSdrConfigRequest(string url)
        {
            return !string.IsNullOrEmpty(url) &&
                url.IndexOf("ISteamApps/GetSDRConfig", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string BuildLocalResponseBody(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "{}";
            }

            if (url.IndexOf("ISteamApps/GetSDRConfig", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return SdrConfigJson;
            }

            if (url.IndexOf("events/ajaxgetpartnereventspageable", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "{\"success\":1,\"events\":[],\"results_html\":\"\",\"last_time\":0,\"more_events\":false,\"total_count\":0}";
            }

            if (url.IndexOf("proregistration/getdpcdata", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "{\"success\":true,\"leagues\":[],\"teams\":[],\"players\":[]}";
            }

            return "{}";
        }


        private static Dictionary<string, string> BuildLocalResponseHeaders(HTTPRequest request)
        {
            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Content-Type", "application/json; charset=utf-8" },
                { "Content-Length", (request.ResponseBytes ?? Array.Empty<byte>()).Length.ToString() }
            };
            return headers;
        }

        private static string PreviewBody(byte[] response)
        {
            if (response == null || response.Length == 0)
            {
                return "<empty>";
            }

            int count = Math.Min(response.Length, 160);
            return Encoding.UTF8.GetString(response, 0, count).Replace("\r", "").Replace("\n", "");
        }

        private static bool TryGetResponseHeader(HTTPRequest request, string name, out string value)
        {
            value = null;
            if (request.ResponseHeaders == null || string.IsNullOrEmpty(name))
            {
                return false;
            }

            return request.ResponseHeaders.TryGetValue(name, out value);
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
            public byte[] ResponseBytes;
            public Dictionary<string, string> ResponseHeaders;
            public string ContentType;
        }

        private class RequestState
        {
            public RequestHTTPAPI HTTPRequest;
            public WebRequest Request;
        }
    }
}
