using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Helper.JSON;
using SKYNET.Managers;
using SKYNET.Network.Packets;
using SKYNET.Steamworks;
using SKYNET.Steamworks.Implementation;
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
        public  static int Port;
        private static List<ulong> P2PSession;
        private static ConcurrentDictionary<string, UdpClient> Connections;

        public static void Initialize()
        {
            Connections = new ConcurrentDictionary<string, UdpClient>();
            P2PSession = new List<ulong>();
            Port = 3333;

            ThreadPool.QueueUserWorkItem(ReceiveThread);
        }

        private static void ReceiveThread(Object ThreadObject)
        {
            UdpClient udpClient = new UdpClient(Port);
            while (true)
            {
                IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, Port);
                byte[] bytes = udpClient.Receive(ref remoteIpEndPoint);
                OnDataReceived(bytes, remoteIpEndPoint);
            }
        }

        private static void OnDataReceived(byte[] Data, IPEndPoint EndPoint)
        {
            SteamEmulator.Debug($"Received P2P packet from {EndPoint.Address}");

            string json = Encoding.Default.GetString(Data);
            NET_P2PPacket P2PPacket = json.FromJson<NET_P2PPacket>();

            if (P2PPacket == null) return;

            ulong steamIDRemote = (ulong)new CSteamID(P2PPacket.Sender);
            if (!P2PSession.Contains(steamIDRemote))
            {
                P2PSessionRequest_t data = new P2PSessionRequest_t()
                {
                    m_steamIDRemote = steamIDRemote
                };
                CallbackManager.AddCallbackResult(data);
                P2PSession.Add(steamIDRemote);
            }

            try
            {
                SteamEmulator.SteamNetworking.AddP2PPacket(P2PPacket);
            }
            catch
            {
            }
        }

        private static void RegisterConnection(string IPAddress, UdpClient client)
        {
            if (!Connections.ContainsKey(IPAddress))
            {
                Connections.TryAdd(IPAddress, client);
            }
        }

        public static bool SendP2PTo(ulong steamIDRemote, byte[] bytes, int eP2PSendType, int nChannel)
        {
            try
            {
                var p2p = new NET_P2PPacket()
                {
                    AccountID = steamIDRemote.GetAccountID(),
                    Buffer = bytes,
                    Sender = SteamEmulator.SteamID.AccountID,
                    P2PSendType = eP2PSendType,
                    Channel = nChannel
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
            var user = SteamFriends.Instance.GetUser(steamIDRemote);
            if (user != null)
            {
                return user.IPAddress;
            }
            var lobby = SteamEmulator.SteamMatchmaking.GetLobbyByGameserver(steamIDRemote);
            if (lobby != null)
            {
                user = SteamFriends.Instance.GetUser(lobby.Owner);
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
            SteamEmulator.Write("P2PNetworking", msg);
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
