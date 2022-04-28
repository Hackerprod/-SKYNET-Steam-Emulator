using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKYNET.Network
{
    public class BroadcastNetwork
    {
        public event EventHandler<KeyValuePair<IPAddress, Byte[]>> PacketReceived;

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(ReceiveThread);
        }

        private void ReceiveThread(Object ThreadObject)
        {
            UdpClient udpClient = new UdpClient(28080);
            while (true)
            {
                IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                byte[] bytes = udpClient.Receive(ref remoteIpEndPoint);
                PacketReceived?.Invoke(this, new KeyValuePair<IPAddress, byte[]>(remoteIpEndPoint.Address, bytes));
            }
        }

        public void Send(string data)
        {
            Byte[] bytes = Encoding.ASCII.GetBytes(data.ToString());
            Send(bytes);
        }

        public void Send(byte[] data)
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Connect(IPAddress.Broadcast, 28080);
            udpClient.Send(data, data.Length);
            udpClient.Close();
        }
    }
}
