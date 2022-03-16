using System;

namespace SKYNET.Interface
{
    public interface ISteamMasterServerUpdater
    {
        // Call this as often as you like to tell the master server updater whether or not
        // you want it to be active (default: off).
        void SetActive(bool bActive);

        // You usually don't need to modify this.
        // Pass -1 to use the default value for iHeartbeatInterval.
        // Some mods change this.
        void SetHeartbeatInterval(int iHeartbeatInterval);


        // These are in GameSocketShare mode, where instead of ISteamMasterServerUpdater creating its own
        // socket to talk to the master server on, it lets the game use its socket to forward messages
        // back and forth. This prevents us from requiring server ops to open up yet another port
        // in their firewalls.
        //
        // the IP address and port should be in host order, i.e 127.0.0.1 == 0x7f000001

        // These are used when you've elected to multiplex the game server's UDP socket
        // rather than having the master server updater use its own sockets.
        // 
        // Source games use this to simplify the job of the server admins, so they 
        // don't have to open up more ports on their firewalls.

        // Call this when a packet that starts with 0xFFFFFFFF comes in. That means
        // it's for us.
        bool HandleIncomingPacket(IntPtr pData, int cbData, uint srcIP, uint srcPort);

        // AFTER calling HandleIncomingPacket for any packets that came in that frame, call this.
        // This gets a packet that the master server updater needs to send out on UDP.
        // It returns the length of the packet it wants to send, or 0 if there are no more packets to send.
        // Call this each frame until it returns 0.
        int GetNextOutgoingPacket(IntPtr pOut, int cbMaxOut, uint pNetAdr, uint pPort);


        // Functions to set various fields that are used to respond to queries.

        // Call this to set basic data that is passed to the server browser.
        void SetBasicServerData(
            uint nProtocolVersion,
           bool bDedicatedServer,

            string pRegionName,

            string pProductName,
            uint nMaxReportedClients,

           bool bPasswordProtected,

            string pGameDescription);

        // Call this to clear the whole list of key/values that are sent in rules queries.
        void ClearAllKeyValues();

        // Call this to add/update a key/value pair.
        void SetKeyValue(string pKey, string pValue);


        // You can call this upon shutdown to clear out data stored for this game server and
        // to tell the master servers that this server is going away.
        void NotifyShutdown();

        // Returns true if the master server has requested a restart.
        // Only returns true once per request.
        bool WasRestartRequested();

        // Force it to request a heartbeat from the master servers.
        void ForceHeartbeat();

        // Manually edit and query the master server list.
        // It will provide name resolution and use the default master server port if none is provided.
        bool AddMasterServer(string pServerAddress);
        bool RemoveMasterServer(string pServerAddress);

        int GetNumMasterServers();

        // Returns the # of bytes written to pOut.
        int GetMasterServerAddress(int iServer, string pOut, int outBufferSize);
    }
}