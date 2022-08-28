using SKYNET.Interfaces;
using SKYNET.Network;
using SKYNET.Network.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SKYNET.Managers
{
    public class ConnectionsManager 
    {
        public static ConcurrentDictionary<ulong, IConnection> ClientConnections;

        public static int ActiveConnections;

        public static void Initialize()
        {
            ClientConnections = new ConcurrentDictionary<ulong, IConnection>();
        }

        internal static void AddOrUpdate(ulong steamId, WebSocketProcessor conn)
        {
            if (steamId == 0) return;

            if (ClientConnections.TryGetValue(steamId, out IConnection cmConnection))
            {
                ClientConnections[steamId] = conn;
            }
            else
            {
                ClientConnections.TryAdd(steamId, conn);
            }
        }

        public static bool IsLoggedIn(uint accountID, out IPAddress ipAlreadyLoggedIn)
        {
            ipAlreadyLoggedIn = null;
            IConnection conn = Get(accountID);

            if (conn != null && conn.Connected)
            {
                string IpString = conn.RemoteEndPoint.Address.ToString();
                if (IPAddress.TryParse(IpString, out ipAlreadyLoggedIn))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public static void Remove(WebSocketProcessor connection)
        {
            List<ulong> steamIds = GetByAddress(connection.RemoteEndPoint.Address);
            if (!steamIds.Any())
            {
                return;
            }

            foreach (var connectionSteamId in steamIds)
            {
                RemoveConnection(connectionSteamId);
            }
        }

        private static void RemoveConnection(ulong steamid)
        {
            IConnection conn = Get(steamid);
            if (conn != null)
            {
                if (ClientConnections.TryRemove(steamid, out IConnection _))
                {
                    //Coordinator.PlayerClosedGame(steamid);
                    Write(string.Format("Connection for {0} was removed successfully.", steamid));

                    var user = DBManager.Users.Get(steamid);
                    if (user != null)
                    {
                        DBManager.Users.SetLastLogoff(user.AccountID, DateTime.Now.ToTimestamp()); 
                        DBManager.Users.SetPlayingState(user.AccountID, false);
                    }
                }
                else
                {
                    Write(string.Format("Error removing connection for {0}.", steamid));
                }
            }
            else
                Write(string.Format("Connection for {0} was not found.", steamid));
        }

        public static List<ulong> GetByAddress(IPAddress RemoteAddress)
        {
            return (from c in ClientConnections where object.Equals(c.Value.RemoteEndPoint.Address, RemoteAddress) select c.Key).ToList();
        }

        public static IConnection Get(ulong steamId)
        {
            ClientConnections.TryGetValue(steamId, out IConnection result);
            return result;
        }

        internal static void Send(ulong steamID, NET_Base message, NETMessageType messageType)
        {
            try
            {
                if (ClientConnections.TryGetValue(steamID, out IConnection connection))
                {
                    NETMessage NETMessage = new NETMessage(messageType, message);
                    connection.Send(NETMessage);
                }
            }
            catch 
            {
            }
        }

        public static void BroadcastMessage(NET_Base message, NETMessageType msgType)
        {
            NETMessage NETMessage = new NETMessage(msgType, message);

            foreach (var connection in ClientConnections)
            {
                try
                {
                    connection.Value.Send(NETMessage);
                }
                catch { }
            } 
        }
        

        private static void Write(object msg)
        {
            Log.Write("Connections", msg);
        }
    }
}
