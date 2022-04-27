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

        // public string IP = "127.0.0.1"; default local
        public int port; // define > init

        //constants
        const int INIT_PORT = 28000;
        const string MULTICAST_ADDR = "239.0.0.222";

        // start from unity3d
        public void Start()
        {
            Console.WriteLine("UDPSend.init()");

            // define port
            port = INIT_PORT;

            // status
            Console.WriteLine("Listening on 127.0.0.1 : " + port);


            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = false;
            receiveThread.SetApartmentState(ApartmentState.Unknown);
            receiveThread.Start();
        }

        // receive thread
        private void ReceiveData()
        {
            client = new UdpClient();

            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, port);
            client.Client.Bind(localEp);

            client.JoinMulticastGroup(IPAddress.Parse(MULTICAST_ADDR));
            while (true)
            {
                Console.WriteLine("123");
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
    }
}
