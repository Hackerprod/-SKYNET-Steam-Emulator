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
        public static TCPServer P2PServer;
        public static ClientSocket P2PSocket;

        private static List<ulong> P2PSession;
        private static ConcurrentDictionary<string, ClientSocket> Connections;

        public static void Initialize()
        {
            Connections = new ConcurrentDictionary<string, ClientSocket>();
            P2PSession = new List<ulong>();

            P2PServer = new TCPServer(3333);
            P2PServer.OnDataReceived += P2PServer_OnDataReceived;
            P2PServer.OnConnected += P2PServer_OnConnected;

            ThreadPool.QueueUserWorkItem(P2PServer.Start);
        }

        private static void P2PServer_OnConnected(object sender, ClientSocket e)
        {
            string IPAddress = ((IPEndPoint)e.socket.RemoteEndPoint).Address.ToString();
            Write($"P2P Client connected from {IPAddress}");
            if (!Connections.ContainsKey(IPAddress))
            {
                Connections.TryAdd(IPAddress, e);
            }
        }

        private static void P2PServer_OnDataReceived(object sender, NetPacket packet)
        {
            SteamEmulator.Debug($"Received P2P packet from {((IPEndPoint)packet.Sender.RemoteEndPoint).Address}");

            RegisterConnection(packet);

            string json = Encoding.Default.GetString(packet.Data);
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
                byte[] bytes = Convert.FromBase64String(P2PPacket.Buffer);
                SteamEmulator.SteamNetworking.AddP2PPacket(P2PPacket);
            }
            catch
            {
            }
        }

        private static void RegisterConnection(NetPacket packet)
        {
            string IPAddress = ((IPEndPoint)packet.Sender.RemoteEndPoint).Address.ToString();

            if (!Connections.ContainsKey(IPAddress))
            {
                Connections.TryAdd(IPAddress, packet.Sender);
            }
        }

        private static void P2PSocket_OnDataReceived(object sender, NetPacket packet)
        {
            SteamEmulator.Debug($"Received P2P packet from {((IPEndPoint)packet.Sender.RemoteEndPoint).Address}");
            string json = Encoding.Default.GetString(packet.Data);
            NET_P2PPacket P2PPacket = json.FromJson<NET_P2PPacket>();

            if (P2PPacket == null) return;

            try
            {
                byte[] bytes = Convert.FromBase64String(P2PPacket.Buffer);
                SteamEmulator.SteamNetworking.AddP2PPacket(P2PPacket);
            }
            catch
            {
            }
        }

        public static bool SendP2PTo(ulong steamIDRemote, byte[] bytes, int eP2PSendType, int nChannel)
        {
            try
            {
                NET_P2PPacket p2p = new NET_P2PPacket()
                {
                    AccountID = steamIDRemote.GetAccountID(),
                    Buffer = Convert.ToBase64String(bytes),
                    Sender = SteamEmulator.SteamID.AccountId,
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
            if (Connections.TryGetValue(iPAddress, out var clientSocket))
            {
                return clientSocket.Send(bytes);
            }
            else
            {
                if (IPAddress.TryParse(iPAddress, out _))
                {
                    try
                    {
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        SocketData SocketData = new SocketData() { socket = socket, Data = bytes };
                        socket.BeginConnect(iPAddress, 3333, ConnectionCallback, SocketData);
                    }
                    catch
                    {
                    }
                }
                return false;
            }
        }

        #region Conection callback

        private static void ConnectionCallback(IAsyncResult ar)
        {
            try
            {
                SocketData SocketData = ((SocketData)ar.AsyncState);
                SocketData.socket.EndConnect(ar);
                string IPAddress = ((IPEndPoint)SocketData.socket.RemoteEndPoint).Address.ToString();
                Write($"Connected to {IPAddress}");

                byte[] bytes = SocketData.Data;
                SocketData.socket.Send(bytes);

                ClientSocket client = new ClientSocket(SocketData.socket);
                client.OnDataReceived += P2PSocket_OnDataReceived;
                client.BeginReceiving();
                Connections.TryAdd(IPAddress, client);
            }
            catch
            {
            }
        }

        #endregion

        private static void Write(object msg)
        {
            SteamEmulator.Write("P2PNetworking", msg);
        }

        private class SocketData
        {
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
                        KV.Value.socket.Disconnect(false);
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
