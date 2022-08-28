using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using WebSocketSharp.Net.WebSockets;
using SKYNET.WEB.Types;
using SKYNET.Interfaces;
using SKYNET.Managers;
using SKYNET.Processors;
using SKYNET.Network.Types;
using System.Collections.Generic;

namespace SKYNET.Network
{
    public class WebSocketProcessor : WebSocketBehavior, IConnection
    {
        public System.Net.IPEndPoint RemoteEndPoint => this.Context.UserEndPoint;

        public bool Connected
        {
            get
            {
                WebSocketContext context = Context;
                bool? obj;
                if (context == null)
                {
                    obj = null;
                }
                else
                {
                    WebSocket webSocket = context.WebSocket;
                    obj = ((webSocket != null) ? new bool?(webSocket.IsAlive) : null);
                }
                return obj ?? false;
            }
        }

        protected override void OnOpen()
        {
            ConnectionsManager.ActiveConnections += 1;
            Write($"New client connected from {this.Context.UserEndPoint}");
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Write("Error: " + e.Message + "\r\n" + (e.Exception.StackTrace ?? string.Empty));
        }

        protected override void OnClose(CloseEventArgs e)
        {
            try
            {
                ConnectionsManager.ActiveConnections -= 1;
                ConnectionsManager.Remove(this);
                Write($"Client disconnected from {this.Context.UserEndPoint}");
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                NETMessage message = e.RawData.Deserialize<NETMessage>();
                UpdateConnections(message);
                NETProcessor.Process(this, message);
            }
            catch (Exception ex)
            {
                Write($"Error in OnMessage {ex}" );
            }
        }

        private void UpdateConnections(NETMessage message)
        {
            ConnectionsManager.AddOrUpdate(message.SteamID, this);
        }

        public void Disconnect()
        {
            base.Close();
        }

        private void Write(object msg)
        {
            SKYNET.Log.Write("WebSocket", msg);
        }

        public void Send(NETMessage msg)
        {
            if (Connected)
            {
                base.Send(msg.Serialize());
            }
        }
    }
}
