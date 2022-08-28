using SKYNET.Network.Types;
using System;
using WebSocketSharp;

namespace SKYNET.Network
{
    public class WebSocketClient
    {
        public bool Connected;

        private WebSocket _webSocket;
        public string ServerIP = "";

        public WebSocketClient(string serverIP)
        {
            _webSocket = new WebSocket();
            _webSocket.OnOpen += OnWebSocketOpen;
            _webSocket.OnError += OnWebSocketError;
            _webSocket.OnClose += OnWebSocketClose;
            _webSocket.OnMessage += OnWebSocketMessage;
            ServerIP = serverIP;
            Connect();
        }

        public void Connect()
        {
            try
            {
                _webSocket.Close();
                if (!string.IsNullOrEmpty(ServerIP))
                {
                    string url = $"ws://{ServerIP}:27000/Steam/";
                    _webSocket.CreateNewURL(url);
                    _webSocket.ConnectAsync();
                }
            }
            catch
            {
                
            }
        }

        private void OnWebSocketOpen(object sender, EventArgs e)
        {
            if (Connected)
            {
            //    if (frmMain.ConnToChat)
            //    {
            //        //SendLoginRequest();
            //        //frmMain.WriteInChat(modCommon.Language.Translate("11.001", "Bienvenido de nuevo."));
            //    }
            //    else
            //    {
            //        frmMain.ConnToChat = true;
            //    }
            }

            Connected = true;
        }
        private void OnWebSocketClose(object sender, CloseEventArgs e)
        {
            Connected = false;
        }

        private void OnWebSocketError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            Connected = false;
        }

        private void OnWebSocketMessage(object sender, MessageEventArgs e)
        {
            try
            {
                NETMessage message = e.RawData.Deserialize<NETMessage>();
                NETProcessor.Process(message);
            }
            catch (Exception ex)
            {
            }
        }

        public void Send(NETMessage message)
        {
            try
            {
                _webSocket.Send(message.Serialize());
            }
            catch {}            
        }

        public void Close()
        {
            try
            {
                _webSocket.Close();
            }
            catch { }
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       