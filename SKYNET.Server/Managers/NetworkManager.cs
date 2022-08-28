using SKYNET.Network;
using System;
using WebSocketSharp.Server;

namespace SKYNET.Managers
{
    public class NetworkManager
    { 
        private static WebServer WebServer;
        public static WebSocketServer webSocketServer;

        public static void Initialize()
        {
            webSocketServer = new WebSocketServer(27000);
            webSocketServer.AddWebSocketService<WebSocketProcessor>("/Steam");

            try
            {
                webSocketServer.Start();
                Write($"Initialized main server on port {27000}");
            }
            catch (Exception)
            {
                Write($"Error initializing main server on port {27000}");
            }


            WebServer = new WebServer(27080);
            if (WebServer.Start())
            {
                Write($"Initialized Web server on port {27080}");
            }
            else
            {
                Write($"Error initializing Web server on port {27080}");
            }
        }

        private static void Write(object msg)
        {
            Log.Write("NetworkManager", msg);
        }
    }
}
