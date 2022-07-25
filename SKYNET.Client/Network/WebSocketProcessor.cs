using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using WebSocketSharp.Net.WebSockets;
using SKYNET.WEB.Types;
using SKYNET.Managers;

namespace SKYNET.Network
{
    public class WebSocketProcessor : WebSocketBehavior
    {
        public System.Net.IPEndPoint RemoteEndPoint => this.Context.UserEndPoint;

        public static event EventHandler<WEBMessage> OnMessageReceived;

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
            Write($"New client connection from {this.Context.UserEndPoint}");
            NetworkManager.WebClient = this;
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Write("Error: " + e.Message + "\r\n" + (e.Exception.StackTrace ?? string.Empty));
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Write("Alert... web socket is disconnected ");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                WEBMessage message = e.RawData.Deserialize<WEBMessage>();
                OnMessageReceived?.Invoke(this, message);
            }
            catch (Exception ex)
            {
                Write($"Error in OnMessage {ex}" );
            }
        }

        public void Disconnect()
        {
            base.Close();
        }

        private void Write(object msg)
        {
            SKYNET.Log.Write("WebSocket", msg);
        }

        public void Send(string JSON)
        {
            if (Connected)
            {
                base.Send(JSON);
            }
        }
    }
}
