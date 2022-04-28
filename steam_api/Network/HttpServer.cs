using SKYNET.Managers;
using System;
using System.Collections.Generic;
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

        public HttpServer()
        {
            _listener = new HttpListener();
            foreach (var ipaddress in NetworkManager.GetIPAddresses())
            {
                Console.WriteLine($"Using address: http://{ipaddress}:{SERVER_PORT}/");
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
            Console.WriteLine($"Started HTTP listen on port 8880");
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
            byte[] responseBytes = new byte[0];
            switch (endPoint)
            {
                case "Avatar":
                    responseBytes = SteamEmulator.SteamFriends.GetAvatar((ulong)SteamEmulator.SteamId);
                    break;
                default:
                    Console.WriteLine($"Not implemented handle to {endPoint}");
                    break;
            }

            if (responseBytes.Length == 0)
            {
                Response.StatusCode = (int)HttpStatusCode.NoContent;
                Response.Close();
                return;
            }

            Response.ContentType = "image/jpg";
            Response.ContentLength64 = responseBytes.Length;
            Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            Response.StatusCode = (int)HttpStatusCode.OK;
            Response.OutputStream.Flush();
        }
    }
}
