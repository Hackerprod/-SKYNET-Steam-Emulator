using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SKYNET.Network
{
    public class ClientSocket
    {
        public Socket socket;
        public event EventHandler<NetPacket> OnDataReceived;

        public ClientSocket(Socket _socket)
        {
            this.socket = _socket;
        }

        public void BeginReceiving()
        {
            (new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    if (StopReceiving) break;

                    if (!IsSocketConnected(socket))
                    {
                        break;
                    }
                    if (socket.Available != 0)
                    {
                        List<byte> list = new List<byte>();
                        while (socket.Available > 0 && socket.Connected)
                        {
                            byte[] array3 = new byte[1];
                            socket.Receive(array3, 0, 1, SocketFlags.None);
                            list.AddRange(array3);

                        }

                        if (list.Count > 0)
                        {
                            OnDataReceived?.Invoke(this, new NetPacket() { Sender = socket, Data = list.ToArray() });
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

        public void Stop()
        {
            StopReceiving = true;
        }
    }
}