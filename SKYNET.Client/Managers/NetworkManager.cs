using System;
using System.Net;
using System.Net.Sockets;
using WebSocketSharp.Server;
using SKYNET.Helpers;
using SKYNET.IPC.Types;
using SKYNET.Network;
using SKYNET.Types;
using System.Collections.Generic;
using SKYNET.Network.Types;

namespace SKYNET.Managers
{
    public class NetworkManager
    {
        public static EventHandler<NET_ChatMessage> OnChatMessage;
        public static WebSocketProcessor WebClient;
        public static WebSocketClient WebSocketClient;

        private static WebServer WebServer;
        private static WebSocketServer WebSocket;

        public static void Initialize()
        {
            WebSocket = new WebSocketServer(8888);
            WebSocket.AddWebSocketService<WebSocketProcessor>("/OnMessage");
            WebSocket.Start();

            WebServer = new WebServer(27088);
            WebServer.Start();

            string serverIP = Settings.ServerIP == null ? "" : Settings.ServerIP.ToString();
            WebSocketClient = new WebSocketClient(serverIP);
        }

        private static void Write(object msg)
        {
            Log.Write("NetworkManager", msg);
        }

    }
}
