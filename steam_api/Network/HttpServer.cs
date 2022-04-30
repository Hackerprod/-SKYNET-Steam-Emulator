using SKYNET.Helper.JSON;
using SKYNET.Managers;
using SKYNET.Network.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET.Network
{
    public class HttpServer
    {
        private const int SERVER_PORT = 8880;
        private HttpListener _listener;
        public event EventHandler Initialized;

        public HttpServer()
        {
            _listener = new HttpListener();
            foreach (var ipaddress in NetworkManager.GetIPAddresses())
            {
                _listener.Prefixes.Add($"http://{ipaddress}:{SERVER_PORT}/");
            }
        }

        public void Start()
        {
            _listener.Start();
            ThreadPool.QueueUserWorkItem(ListenConnections);
        }

        private void ListenConnections(object state)
        {
            Initialized?.Invoke(this, null);
            while (true)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    Process(context);
                }
                catch 
                {

                }
            }
        }

        private void Process(HttpListenerContext context)
        {
            var Response = context.Response;
            string endPoint = context.Request.Url.AbsolutePath.Remove(0, 1);

            SteamEmulator.Write("HttpServer", $"Received HTTP call {endPoint} from {context.Request.RemoteEndPoint.Address}");

            switch (endPoint)
            {
                case "Avatar":
                    ProcessAvatar(Response);
                    break;
                case "Announce":
                    ProcessAnnounce(context);
                    break;
                default:
                    Console.WriteLine($"Not implemented handle to {endPoint}");
                    break;
            }
        }

        private void ProcessAnnounce(HttpListenerContext context)
        {
            try
            {
                var IpAddress = context.Request.RemoteEndPoint.Address.ToString();

                StreamReader reader = new StreamReader(context.Request.InputStream);
                string json = reader.ReadToEnd();

                if (!string.IsNullOrEmpty(json))
                {
                    NET_Announce announce = json.FromJson<NET_Announce>();

                    if (announce != null) 
                    SteamEmulator.SteamFriends.AddOrUpdateUser(announce.AccountID, announce.PersonaName, announce.AppID, IpAddress);
                }
            }
            catch (Exception ex)
            {
                SteamEmulator.Debug("" + ex);
            }

            try
            {
                NET_Announce announce = new NET_Announce()
                {
                    PersonaName = SteamEmulator.PersonaName,
                    AccountID = (uint)SteamEmulator.SteamId
                };
                string json = announce.ToJson();
                context.Response.ContentType = "application/json";
                SendResponse(context.Response, Encoding.Default.GetBytes(json));
            }
            catch (Exception ex)
            {
                SteamEmulator.Debug("" + ex);
            }
        }

        private void ProcessAvatar(HttpListenerResponse Response)
        {
            byte[] responseBytes = SteamEmulator.SteamFriends.GetAvatar((ulong)SteamEmulator.SteamId);
            Response.ContentType = "image/jpg";
            SendResponse(Response, responseBytes);
        }

        private void SendResponse(HttpListenerResponse Response, byte[] bytes)
        {
            if (bytes.Length == 0)
            {
                Response.StatusCode = (int)HttpStatusCode.NoContent;
                Response.Close();
                return;
            }
            Response.ContentLength64 = bytes.Length;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.StatusCode = (int)HttpStatusCode.OK;
            Response.OutputStream.Flush();
        }
    }
}
