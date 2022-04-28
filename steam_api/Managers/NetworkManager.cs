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
using static SKYNET.Network.ClassNetworkServerBroadcast;

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

            AnnounceClient();
        }

        private static void Discovery_DataReceived(byte[] msg, IPAddress EndPoint)
        {
            try
            {
                string Content = Encoding.Default.GetString(msg);
                NetworkMessage message = Content.FromJson<NetworkMessage>();
                ProcessMessage(message);
            }
            catch (Exception ex)
            {
                Write($"Error parsing incoming message");
            }
        }

        private static void ProcessMessage(NetworkMessage message)
        {
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
            NET_Announce announce = message.ParsedBody.FromJson<NET_Announce>();
            SteamEmulator.SteamFriends.AddOrUpdateUser(announce.AccountID, announce.PersonaName, announce.AppID);
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
    }
}
