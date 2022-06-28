using SKYNET.Types;
using SKYNET.Common;
using System;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;
using SKYNET.Managers;
using WebSocketSharp.Net.WebSockets;
using SKYNET.WEB.Types;

namespace SKYNET.Network
{
    public class WebSocketProcessor : WebSocketBehavior
    {
        public System.Net.IPEndPoint RemoteEndPoint => this.Context.UserEndPoint;

        public static event EventHandler<WebMessage> OnMessageReceived;

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
                WebMessage message = e.RawData.Deserialize<WebMessage>();
                OnMessageReceived?.Invoke(this, message);
            }
            catch 
            {
                Write($"Error in OnMessage {Encoding.ASCII.GetString(e.RawData)}" );
            }
        }

        public void Disconnect()
        {
            base.Close();
        }

        private void Write(object msg)
        {
            Common.Log.Write("WebSocket", msg);
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
