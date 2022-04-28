using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET.Network
{
    public class BroadcastNetwork 
    {
        public event Action<byte[], IPAddress> DataReceived;

        Thread receiveThread;

        // udpclient object
        UdpClient client;

        //constants
        const int INIT_PORT = 8123;
        const string MULTICAST_ADDR = "239.0.0.222";

        // start from unity3d
        public void Start()
        {
            ThreadPool.QueueUserWorkItem(ReceiveData);
        }

        // receive thread
        private void ReceiveData(object ThreadObj)
        {
            client = new UdpClient();

            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, INIT_PORT);
            client.Client.Bind(localEp);

            client.JoinMulticastGroup(IPAddress.Parse(MULTICAST_ADDR));
            while (true)
            {
                try
                {
                    byte[] data = client.Receive(ref localEp);
                    DataReceived?.Invoke(data, localEp.Address);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.ToString());
                }
            }
        }

        public void Send(string data)
        {
            Byte[] senddata = Encoding.ASCII.GetBytes(data.ToString());
            Send(senddata);
        }

        public void Send(byte[] data)
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Connect(IPAddress.Broadcast, 8123);
            udpClient.Send(data, data.Length);
            udpClient.Close();
        }
    }
}
