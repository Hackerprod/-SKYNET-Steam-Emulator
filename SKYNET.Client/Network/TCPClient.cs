using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SKYNET.Network
{
    public class TCPClient
    {
        public Socket Socket;
        public event EventHandler<NetPacket> OnDataReceived;

        public TCPClient(Socket _socket)
        {
            Socket = _socket;
        }

        public void BeginReceiving()
        {
            (new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    if (StopReceiving) break;

                    if (!IsSocketConnected(Socket))
                    {
                        break;
                    }
                    if (Socket.Available != 0)
                    {
                        List<byte> list = new List<byte>();
                        while (Socket.Available > 0 && Socket.Connected)
                        {
                            byte[] array3 = new byte[1];
                            Socket.Receive(array3, 0, 1, SocketFlags.None);
                            list.AddRange(array3);

                        }

                        if (list.Count > 0)
                        {
                            OnDataReceived?.Invoke(this, new NetPacket() { Sender = this, Data = list.ToArray() });
                        }
                    }
                }

            })).Start();

        }
        private bool IsSocketConnected(Socket s)
        {
            try
            {
                bool flag = s.Poll(1000, SelectMode.SelectRead);
                bool flag2 = s.Available == 0;
                if ((flag && flag2) || !s.Connected)
                {
                    return false;
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public bool StopReceiving { get; private set; }
        public IPEndPoint RemoteEndPoint => (IPEndPoint)Socket?.RemoteEndPoint;

        public void Stop()
        {
            StopReceiving = true;
        }

        public bool Send(byte[] bytes)
        {
            try
            {
                if (Socket.Connected)
                {
                    Socket.Send(bytes);
                    return true;
                }
            }
            catch 
            {
            }
            return false;
        }
    }
}