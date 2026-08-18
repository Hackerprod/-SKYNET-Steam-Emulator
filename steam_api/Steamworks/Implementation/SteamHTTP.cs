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
using System.Globalization;

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
            Write($"SDR config path: {Path.Combine(AppContext.BaseDirectory, "SKYNET", "sdr-relays.ini")}");
        }

        public uint CreateCookieContainer(bool bAllowResponsesToModify)
        {
            Write($"CreateCookieContainer");
            return 0;
        }

        public HTTPRequestHandle CreateHTTPRequest(uint eHTTPRequestMethod, string pchAbsoluteURL)
        {
            return CreateHTTPRequest(unchecked((int)eHTTPRequestMethod), pchAbsoluteURL);
        }

        public HTTPRequestHandle CreateHTTPRequest(int eHTTPRequestMethod, string pchAbsoluteURL)
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

                request.ResponseBytes = BuildLocalResponseBytes(request);
                request.ResponseHeaders = BuildLocalResponseHeaders(request);
                data.BodySize = (uint)request.ResponseBytes.Length;

                if (IsSdrConfigRequest(request.URL))
                {
                    Write(
                        $"SendHTTPRequest: serving local SDR config, " +
                        $"body={data.BodySize}"
                    );

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

                Write(
                    $"SendHTTPRequest prepared handle={hRequest} " +
                    $"call={pCallHandle} " +
                    $"url={request.URL} " +
                    $"context={data.ContextValue} " +
                    $"success={data.RequestSuccessful} " +
                    $"status={(int)data.StatusCode} " +
                    $"body={data.BodySize} " +
                    $"preview={PreviewBody(request.ResponseBytes)} " +
                    $"active=[{DescribeActiveHandles()}]"
                );

                Result = true;
            }
            catch (Exception ex)
            {
                Write($"SendHTTPRequest handle={hRequest} failed: {ex.Message}");
            }

            Write($"SendHTTPRequest (HTTPRequestHandle = {hRequest}) = {Result}");
            return Result;
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

        private sealed class SdrRelayEntry
        {
            public string PopId;
            public string Address;
            public int Port;
            public string Description;
            public double Longitude;
            public double Latitude;
            public int Partners;
            public int Tier;
        }

        private sealed class SdrTypicalPingEntry
        {
            public string From;
            public string To;
            public int Ping;
        }

        private static string BuildSdrConfigJson()
        {
            string iniPath = Path.Combine(
            AppContext.BaseDirectory,
            "SKYNET",
            "sdr-relays.ini"
            );

            if (!File.Exists(iniPath))
            {
                throw new FileNotFoundException(
                    "SDR relay config not found",
                    iniPath
                );
            }

            var values = ParseIni(iniPath);

            uint revision = 1786739253;

            if (
                values.TryGetValue("SDR", out var sdr) &&
                sdr.TryGetValue("Revision", out var revisionText) &&
                uint.TryParse(revisionText, out var parsedRevision)
            )
            {
                revision = parsedRevision;
            }

            var relays = new List<SdrRelayEntry>();
            var typicalPings = new List<SdrTypicalPingEntry>();

            foreach (var section in values)
            {
                if (section.Key.StartsWith(
                    "Relay.",
                    StringComparison.OrdinalIgnoreCase))
                {
                    string popId =
                        section.Key.Substring("Relay.".Length);

                    var v = section.Value;

                    relays.Add(new SdrRelayEntry
                    {
                        PopId = popId,

                        Address =
                            GetIni(v, "Address", "127.0.0.1"),

                        Port =
                            GetIniInt(v, "Port", 28009),

                        Description =
                            GetIni(v, "Description", popId),

                        Longitude =
                            GetIniDouble(v, "Longitude", 0),

                        Latitude =
                            GetIniDouble(v, "Latitude", 0),

                        Partners =
                            GetIniInt(v, "Partners", 1),

                        Tier =
                            GetIniInt(v, "Tier", 0)
                    });

                    continue;
                }

                if (section.Key.StartsWith(
                    "TypicalPing.",
                    StringComparison.OrdinalIgnoreCase))
                {
                    var v = section.Value;

                    typicalPings.Add(new SdrTypicalPingEntry
                    {
                        From = GetIni(v, "From", ""),
                        To = GetIni(v, "To", ""),
                        Ping = GetIniInt(v, "Ping", 1)
                    });
                }
            }

            if (relays.Count == 0)
            {
                throw new InvalidOperationException(
                    "sdr-relays.ini contains no [Relay.*] sections"
                );
            }

            var sb = new StringBuilder();

            sb.Append("{\"revision\":");
            sb.Append(revision);
            sb.Append(",\"pops\":{");

            for (int i = 0; i < relays.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(',');
                }

                var relay = relays[i];

                sb.Append('"');
                sb.Append(JsonEscape(relay.PopId));
                sb.Append("\":{");

                sb.Append("\"desc\":\"");
                sb.Append(JsonEscape(relay.Description));
                sb.Append("\",");

                sb.Append("\"geo\":[");
                sb.Append(
                    relay.Longitude.ToString(
                        CultureInfo.InvariantCulture));
                sb.Append(',');
                sb.Append(
                    relay.Latitude.ToString(
                        CultureInfo.InvariantCulture));
                sb.Append("],");

                sb.Append("\"partners\":");
                sb.Append(relay.Partners);
                sb.Append(',');

                sb.Append("\"tier\":");
                sb.Append(relay.Tier);
                sb.Append(',');

                sb.Append("\"relays\":[{");

                sb.Append("\"ipv4\":\"");
                sb.Append(JsonEscape(relay.Address));
                sb.Append("\",");

                sb.Append("\"port_range\":[");
                sb.Append(relay.Port);
                sb.Append(',');
                sb.Append(relay.Port);
                sb.Append("]}]}");
            }

            sb.Append("},");

            sb.Append("\"certs\":[");
            for (int i = 0; i < SdrConfigCerts.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(',');
                }

                sb.Append('"');
                sb.Append(JsonEscape(SdrConfigCerts[i]));
                sb.Append('"');
            }
            sb.Append("],");

            sb.Append(
                "\"p2p_share_ip\":{\"cn\":20,\"default\":40,\"ru\":20},"
            );

            sb.Append(
                "\"relay_public_key\":\"5AC884C1045BA0FF44142AC8DCA51B8A98C8F1CB4FEE36284AFBE92FCF594932\","
            );

            sb.Append("\"revoked_keys\":[],");

            sb.Append("\"typical_pings\":[");

            bool firstPing = true;

            foreach (var ping in typicalPings)
            {
                if (
                    string.IsNullOrEmpty(ping.From) ||
                    string.IsNullOrEmpty(ping.To))
                {
                    continue;
                }

                if (!firstPing)
                {
                    sb.Append(',');
                }

                firstPing = false;

                sb.Append("[\"");
                sb.Append(JsonEscape(ping.From));
                sb.Append("\",\"");
                sb.Append(JsonEscape(ping.To));
                sb.Append("\",");
                sb.Append(ping.Ping);
                sb.Append(']');
            }

            sb.Append("],");
            sb.Append("\"success\":true}");

            return sb.ToString();
        }

        private static Dictionary<
            string,
            Dictionary<string, string>
        > ParseIni(string path)
        {
            var result =
                new Dictionary<
                    string,
                    Dictionary<string, string>
                >(StringComparer.OrdinalIgnoreCase);

            Dictionary<string, string> current = null;

            foreach (string rawLine in File.ReadAllLines(path))
            {
                string line = rawLine.Trim();

                if (
                    line.Length == 0 ||
                    line.StartsWith(";") ||
                    line.StartsWith("#"))
                {
                    continue;
                }

                if (
                    line.StartsWith("[") &&
                    line.EndsWith("]"))
                {
                    string section =
                        line.Substring(
                            1,
                            line.Length - 2
                        ).Trim();

                    current =
                        new Dictionary<string, string>(
                            StringComparer.OrdinalIgnoreCase
                        );

                    result[section] = current;
                    continue;
                }

                if (current == null)
                {
                    continue;
                }

                int equals = line.IndexOf('=');

                if (equals <= 0)
                {
                    continue;
                }

                string key =
                    line.Substring(0, equals).Trim();

                string value =
                    line.Substring(equals + 1).Trim();

                current[key] = value;
            }

            return result;
        }

        private static string GetIni(
            Dictionary<string, string> values,
            string key,
            string fallback)
        {
            return values.TryGetValue(key, out var value)
                ? value
                : fallback;
        }

        private static int GetIniInt(
            Dictionary<string, string> values,
            string key,
            int fallback)
        {
            return
                values.TryGetValue(key, out var text) &&
                int.TryParse(text, out var value)
                    ? value
                    : fallback;
        }

        private static double GetIniDouble(
            Dictionary<string, string> values,
            string key,
            double fallback)
        {
            if (!values.TryGetValue(key, out var text))
            {
                return fallback;
            }

            return double.TryParse(
                text,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var value)
                    ? value
                    : fallback;
        }

        private static string JsonEscape(string value)
        {
            if (value == null)
            {
                return "";
            }

            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");
        }

        internal static string SdrConfigJson => BuildSdrConfigJson();

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
                return "{\"success\":0,\"skynet_test\":12345}";
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
            public HTTPMethod RequestMethod;
            public string URL;
            public uint TimeoutSeconds;
            public byte[] RawPostBody;
            public byte[] ResponseBytes;
            public Dictionary<string, string> ResponseHeaders;
            public string ContentType;
        }

    }
}
