using SKYNET.Helper;
using SKYNET.Helper.JSON;
using SKYNET.Network;
using SKYNET.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SKYNET.Managers
{
    public class NetworkManager
    {
        private static BroadcastNetwork BroadcastNetwork;

        public static void Initialize()
        {
            BroadcastNetwork = new BroadcastNetwork();
            BroadcastNetwork.DataReceived += Discovery_DataReceived;
            BroadcastNetwork.Start();

            var localEndpoint = new IPEndPoint(IPAddress.Parse("10.31.0.1"), 0);
            var udpClient = new UdpClient(localEndpoint);

            //BroadcastNetwork receiver = new BroadcastNetwork(28000);
            //receiver.DataReceived += Discovery_DataReceived;
            //Task.Run(async () => await receiver.ReceiveAsync().ConfigureAwait(false));

            SendAsync(udpClient, 28000, Encoding.Default.GetBytes("Unju"));

            //SendAsync(udpClient, 28000, Encoding.Default.GetBytes("Al berro"));

            //AnnounceClient();
        }

        private static void SendAsync(UdpClient udpClient, int destinationPort, byte[] deviceHelloPackage)
        {
            Console.WriteLine($"{nameof(SendAsync)} - Send hello package");

            var ipEndpoint = new IPEndPoint(IPAddress.Broadcast, destinationPort);
            udpClient.Send(deviceHelloPackage, deviceHelloPackage.Length, ipEndpoint);
        }

        private static void Discovery_DataReceived(byte[] arg1, System.Net.IPAddress arg2)
        {
            Console.WriteLine($"Discovery_DataReceived: {Encoding.Default.GetString(arg1)}");
        }

        private static void BroadcastNetwork_PacketReceived(object sender, BroadcastPacket e)
        {
            Write("...");
            string json = Encoding.Default.GetString(e.Data);
            NetworkMessage message = json.FromJson<NetworkMessage>();
            if (message == null)
            {
                Write("Malformed Packet received");
                return;
            }

            Write($"Received Broadcast packet type: {message.MessageType}");

            switch ((MessageType)message.MessageType)
            {
                case MessageType.NET_Announce:
                    ProcessAnnounce(message);
                    break;
                case MessageType.NET_Avatar:
                    break;
                default:
                    break;
            }
        }

        private static void ProcessAnnounce(NetworkMessage message)
        {
            Write("Receiveeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeed");
            //NET_Announce announce = message.Body.Deserialize<NET_Announce>();
            //SteamEmulator.SteamFriends.AddOrUpdateFriend(announce.AccountID, announce.PersonaName, announce.AppID);
            //SteamEmulator.SteamFriends.AddOrUpdateUser(announce.AccountID, announce.PersonaName, announce.AppID);
        }

        public static void AnnounceClient()
        {
            try
            {
                NET_Announce announce = new NET_Announce()
                {
                    PersonaName = SteamEmulator.PersonaName,
                    AccountID = (uint)SteamEmulator.SteamId
                };

                SendBroadcastMessage(announce, MessageType.NET_Announce);
            }
            catch (Exception ex)
            {
                Write($"{ex}");
            }
        }

        private static void SendBroadcastMessage(object obj, MessageType type)
        {
            if (obj == null) return;

            NetworkMessage message = new NetworkMessage()
            {
                MessageType = (int)type,
                Body = obj
            };

            string json = message.ToJson();
            byte[] Body = Encoding.Default.GetBytes(json);

        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("NetworkManager", msg);
        }
    }
}
