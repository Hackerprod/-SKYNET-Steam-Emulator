using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Network
{
    public class TCPServer
    {
        private Socket _serverSocket;
        private IPEndPoint _localEndPoint;
        private List<ClientSocket> ConnectedClients;
        public int Port = 28880;
        public bool Started { get; private set; }

        public event EventHandler<Socket> OnConnected;
        public event EventHandler<Socket> OnDisconnected;
        public event EventHandler<NetPacket> OnDataReceived;

        public TCPServer(int port)
        {
            ConnectedClients = new List<ClientSocket>();
            Port = port;
        }

        internal void NotifyUserDisconnected(ClientSocket clientSockets)
        {
            OnDisconnected?.Invoke(this, clientSockets.socket);
            ConnectedClients.Remove(clientSockets);
        }

        public void Start(object state)
        {
            this._localEndPoint = new IPEndPoint(IPAddress.Any, Port);
            this._serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                this._serverSocket.Bind(this._localEndPoint);
                this._serverSocket.Listen(4096);
                this._serverSocket.BeginAccept(new AsyncCallback(this.EndAccept), this._serverSocket);
                this.Started = true;
            }
            catch (Exception ex)
            {
                SteamEmulator.Write("NetworkManager", $"Error starting TCP server: {ex}");
            }
        }

        private void EndAccept(IAsyncResult ar)
        {
            try
            {
                Socket socket = ((Socket)ar.AsyncState).EndAccept(ar);
                OnConnected?.Invoke(this, socket);
                ClientSocket client = new ClientSocket(socket);
                client.OnDataReceived += Client_OnDataReceived;
                client.BeginReceiving();

            }
            catch (NullReferenceException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception exception)
            {
                //ILog.Write("EndAccept: Error accepting client.", exception);
            }
            try
            {
                this._serverSocket.BeginAccept(new AsyncCallback(this.EndAccept), this._serverSocket);
            }
            catch (NullReferenceException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception exception2)
            {
                //ILog.Write("EndAccept: Error accepting client.", exception2);
            }
        }

        private void Client_OnDataReceived(object sender, NetPacket networkMessage)
        {
            OnDataReceived?.Invoke(this, networkMessage);
        }

        public void Stop()
        {
            _serverSocket.Close();
            _serverSocket.Dispose();
        }

        internal void DisconnectAll()
        {
            foreach (ClientSocket conn in ConnectedClients)
            {
                conn.Stop();
                conn.socket.Close();
            }
            ConnectedClients.Clear();
        }
    }
    public class NetPacket
    {
        public Socket Sender;
        public byte[] Data;
    }
}
