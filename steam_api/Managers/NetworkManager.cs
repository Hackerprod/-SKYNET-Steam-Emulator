using SKYNET.Helper;
using SKYNET.Helper.JSON;
using SKYNET.Network;
using SKYNET.Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SKYNET.Managers
{
    public class NetworkManager
    {
        private static BroadcastNetwork BroadcastNetwork;
        private static System.Timers.Timer Timer;

        public static void Initialize()
        {
            HttpServer httpServer = new HttpServer();
            httpServer.Start();

            BroadcastNetwork = new BroadcastNetwork();
            BroadcastNetwork.PacketReceived += BroadcastNetwork_PacketReceived; ;
            BroadcastNetwork.Start();

            AnnounceClient();

            ThreadPool.QueueUserWorkItem(StartTimer);
        }

        private static void StartTimer(object threadObj)
        {
            Timer = new System.Timers.Timer();
            Timer.AutoReset = false;
            Timer.Interval = 60000;
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
        }

        private static void BroadcastNetwork_PacketReceived(object sender, KeyValuePair<IPAddress, byte[]> KeyValue)
        {
            try
            {
                string Content = Encoding.Default.GetString(KeyValue.Value);
                NetworkMessage message = Content.FromJson<NetworkMessage>();
                ProcessMessage(message, KeyValue.Key);
            }
            catch (Exception ex)
            {
                Write($"Error parsing incoming message");
            }
        }

        private static void ProcessMessage(NetworkMessage message, IPAddress sender)
        {
            switch ((MessageType)message.MessageType)
            {
                case MessageType.NET_Announce:
                    string Ip = sender.ToString();
                    ProcessAnnounce(message, Ip);
                    break;
                case MessageType.NET_Avatar:
                    break;
                default:
                    break;
            }
        }

        private static void ProcessAnnounce(NetworkMessage message, string senderAddress)
        {
            NET_Announce announce = message.ParsedBody.FromJson<NET_Announce>();
            SteamEmulator.SteamFriends.AddOrUpdateUser(announce.AccountID, announce.PersonaName, announce.AppID, senderAddress);
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
                ParsedBody = obj.ToJson()
            };

            string json = message.ToJson();
            byte[] Body = Encoding.Default.GetBytes(json);
            SendBroadcastMessage(Body);
        }

        public static void SendBroadcastMessage(byte[] Body)
        {
            BroadcastNetwork.Send(Body);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("NetworkManager", msg);
        }

        public static List<IPAddress> GetIPAddresses()
        {
            var Addresses = new List<IPAddress>();
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            IPAddress iPAddress = null;
            IPAddress[] addressList = hostEntry.AddressList;
            foreach (IPAddress iPAddress2 in addressList)
            {
                if (iPAddress2.AddressFamily == AddressFamily.InterNetwork)
                {
                    Addresses.Add(iPAddress2);
                }
            }
            return Addresses;
        }

        public static IPAddress GetIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            IPAddress iPAddress = null;
            IPAddress[] addressList = hostEntry.AddressList;
            foreach (IPAddress iPAddress2 in addressList)
            {
                if (iPAddress2.AddressFamily == AddressFamily.InterNetwork)
                {
                    iPAddress = iPAddress2;
                }
            }
            return iPAddress;
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            AnnounceClient();
            Timer.Interval = 60000;
            Timer.Start();
        }
    }
}
