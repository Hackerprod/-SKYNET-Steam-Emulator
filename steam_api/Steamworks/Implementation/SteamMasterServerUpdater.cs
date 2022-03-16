using SKYNET.Interface;
using System;

namespace SKYNET.Managers
{
    [Map("SteamMasterServerUpdater")]
    public class SteamMasterServerUpdater : IBaseInterface, ISteamMasterServerUpdater
    {
        public void SetActive(bool bActive)
        {
            //
        }

        public void SetHeartbeatInterval(int iHeartbeatInterval)
        {
            //
        }

        public bool HandleIncomingPacket(IntPtr pData, int cbData, uint srcIP, uint srcPort)
        {
            return false;
        }

        public int GetNextOutgoingPacket(IntPtr pOut, int cbMaxOut, uint pNetAdr, uint pPort)
        {
            return 0;
        }

        public void ClearAllKeyValues()
        {
            //
        }

        public void SetKeyValue(string pKey, string pValue)
        {
            //
        }

        public void NotifyShutdown()
        {
            //
        }

        public bool WasRestartRequested()
        {
            return false;
        }

        public void ForceHeartbeat()
        {
            //
        }

        public bool AddMasterServer(string pServerAddress)
        {
            return false;
        }

        public bool RemoveMasterServer(string pServerAddress)
        {
            return false;
        }

        public int GetNumMasterServers()
        {
            return 0;
        }

        public int GetMasterServerAddress(int iServer, string pOut, int outBufferSize)
        {
            return 0;
        }

        public void SetBasicServerData(uint nProtocolVersion, bool bDedicatedServer, string pRegionName, string pProductName, uint nMaxReportedClients, bool bPasswordProtected, string pGameDescription)
        {
            //
        }
    }

}
