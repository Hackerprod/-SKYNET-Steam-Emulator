using SKYNET.Interfaces;
using SKYNET.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace SKYNET.Managers
{
    public class ConnectionsManager 
    {
        public static ConcurrentDictionary<ulong, IConnection> TCPConnectionsWithSteamId;
        public static WebSocketServer webSocketServer;

        public static void Initialize()
        {
            TCPConnectionsWithSteamId = new ConcurrentDictionary<ulong, IConnection>();

            webSocketServer = new WebSocketServer(27000);
            webSocketServer.AddWebSocketService<WebSocketProcessor>("/Steam");
            webSocketServer.Start();
        }

        internal static void AddOrUpdate(ulong steamId, WebSocketProcessor conn)
        {
            if (steamId == 0) return;

            if (TCPConnectionsWithSteamId.TryGetValue(steamId, out IConnection cmConnection))
            {
                TCPConnectionsWithSteamId[steamId] = conn;
            }
            else
            {
                TCPConnectionsWithSteamId.TryAdd(steamId, conn);
            }
        }
    }
}
