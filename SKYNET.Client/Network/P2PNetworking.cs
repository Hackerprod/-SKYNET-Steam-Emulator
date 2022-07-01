using SKYNET.Client;
using SKYNET.Common;
using SKYNET.Helper;
using SKYNET.IPC.Types;
using SKYNET.Managers;
using SKYNET.Network.Packets;
using SKYNET.Steamworks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SKYNET.Network
{
    public class P2PNetworking
    {
        public static int Port;
        private static ConcurrentDictionary<string, UdpClient> Connections;

        public static void Initialize()
        {
            Connections = new ConcurrentDictionary<string, UdpClient>();
            Port = 3333;

            if (NetworkHelper.IsAvailablePort(Port))
            {
                Write($"Initializing P2P server on port {Port}");
                ThreadPool.QueueUserWorkItem(ReceiveThread);
            }
            else
            {
                Write($"Error initializing P2P server, port {Port} is in use");
            }

        }

        private static void ReceiveThread(Object ThreadObject)
        {
            UdpClient udpClient = new UdpClient(Port);
            while (true)
            {
                try
                {
                    IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, Port);
                    byte[] bytes = udpClient.Receive(ref remoteIpEndPoint);
                    OnDataReceived(bytes, remoteIpEndPoint);
                }
                catch
                {
                }
            }
        }

        private static void OnDataReceived(byte[] Data, IPEndPoint EndPoint)
        {
            Write($"Received P2P packet from {EndPoint.Address}");

            string json = Encoding.Default.GetString(Data);
            NET_P2PPacket P2PPacket = json.Deserialize<NET_P2PPacket>();
            IPCManager.SendP2PPacket(P2PPacket);
        }

        private static void RegisterConnection(string IPAddress, UdpClient client)
        {
            if (!Connections.ContainsKey(IPAddress))
            {
                Connections.TryAdd(IPAddress, client);
            }
        }

        public static bool SendP2PTo(ulong steamIDRemote, IPC_P2PPacket P2PPacket)
        {
            try
            {
                var p2p = new NET_P2PPacket()
                {
                    AccountID = P2PPacket.AccountID,
                    Buffer = P2PPacket.Buffer,
                    Sender = P2PPacket.Sender,
                    P2PSendType = P2PPacket.P2PSendType,
                    Channel = P2PPacket.Channel,
                };

                string json = p2p.ToJson();
                byte[] packet = Encoding.Default.GetBytes(json);

                string iPAddress = GetIPAddress(steamIDRemote);

                if (IPAddress.TryParse(iPAddress, out _))
                {
                    return SendTo(iPAddress, packet);
                }
                else
                {
                    Write($"Not found User {steamIDRemote} to send P2P packet");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Write(ex.Message + " " + ex.StackTrace);
            }
            return false;
        }

        private static string GetIPAddress(ulong steamIDRemote)
        {
            var user = UserManager.GetUser(steamIDRemote);
            if (user != null)
            {
                return user.IPAddress;
            }
            var lobby = LobbyManager.GetLobbyByGameserver(steamIDRemote);
            if (lobby != null)
            {
                user = UserManager.GetUser(lobby.Owner);
                if (user != null)
                {
                    return user.IPAddress;
                }
            }
            return "";
        }

        private static bool SendTo(string iPAddress, byte[] bytes)
        {
            Write($"Connecting to {iPAddress}");
            if (Connections.ContainsKey(iPAddress) && Connections[iPAddress].Client.Connected)
            {
                try
                {
                    var client = Connections[iPAddress];
                    client.Send(bytes, bytes.Length);
                    return true;
                }
                catch
                {
                    Connections.TryRemove(iPAddress, out var _);
                }
            }

            if (IPAddress.TryParse(iPAddress, out _))
            {
                try
                {
                    UdpClient udpClient = new UdpClient();
                    udpClient.Connect(iPAddress, Port);
                    udpClient.Send(bytes, bytes.Length);
                    RegisterConnection(iPAddress, udpClient);
                    return true;
                }
                catch
                {
                }
            }
            return false;
        }

        private static void Write(object msg)
        {
            Log.Write("P2PNetworking", msg);
        }

        private class SocketData
        {
            public string Address { get; set; }
            public Socket socket { get; set; }
            public byte[] Data { get; set; }
        }

        internal static void CloseConnections()
        {
            try
            {
                foreach (var KV in Connections)
                {
                    try
                    {
                        //KV.Value.Socket.Disconnect(false);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {

            }
            Connections.Clear();
        }
    }
}
