using System;

namespace SKYNET.Steamworks.Interfaces
{
    /// <summary>Legacy master-server updater ABI used by Left 4 Dead.</summary>
    [Interface("SteamMasterServerUpdater001")]
    [Interface("SteamMasterServerUpdater002")]
    public class SteamMasterServerUpdater001 : ISteamInterface
    {
        public void SetActive(IntPtr _, bool bActive)
        {
            SteamEmulator.SteamMasterServerUpdater.SetActive(bActive);
        }

        public void SetHeartbeatInterval(IntPtr _, int iHeartbeatInterval)
        {
            SteamEmulator.SteamMasterServerUpdater.SetHeartbeatInterval(iHeartbeatInterval);
        }

        public bool HandleIncomingPacket(IntPtr _, IntPtr pData, int cbData, uint srcIP, uint srcPort)
        {
            return SteamEmulator.SteamMasterServerUpdater.HandleIncomingPacket(pData, cbData, srcIP, srcPort);
        }

        public int GetNextOutgoingPacket(IntPtr _, IntPtr pOut, int cbMaxOut, IntPtr pNetAdr, IntPtr pPort)
        {
            return SteamEmulator.SteamMasterServerUpdater.GetNextOutgoingPacket(pOut, cbMaxOut, 0, 0);
        }

        public void SetBasicServerData(
            IntPtr _,
            uint nProtocolVersion,
            bool bDedicatedServer,
            string pRegionName,
            string pProductName,
            uint nMaxReportedClients,
            bool bPasswordProtected,
            string pGameDescription)
        {
            SteamEmulator.SteamMasterServerUpdater.SetBasicServerData(
                nProtocolVersion,
                bDedicatedServer,
                pRegionName,
                pProductName,
                nMaxReportedClients,
                bPasswordProtected,
                pGameDescription);
        }

        public void ClearAllKeyValues(IntPtr _)
        {
            SteamEmulator.SteamMasterServerUpdater.ClearAllKeyValues();
        }

        public void SetKeyValue(IntPtr _, string pKey, string pValue)
        {
            SteamEmulator.SteamMasterServerUpdater.SetKeyValue(pKey, pValue);
        }

        public void NotifyShutdown(IntPtr _)
        {
            SteamEmulator.SteamMasterServerUpdater.NotifyShutdown();
        }

        public bool WasRestartRequested(IntPtr _)
        {
            return SteamEmulator.SteamMasterServerUpdater.WasRestartRequested();
        }

        public void ForceHeartbeat(IntPtr _)
        {
            SteamEmulator.SteamMasterServerUpdater.ForceHeartbeat();
        }

        public bool AddMasterServer(IntPtr _, string pServerAddress)
        {
            return SteamEmulator.SteamMasterServerUpdater.AddMasterServer(pServerAddress);
        }

        public bool RemoveMasterServer(IntPtr _, string pServerAddress)
        {
            return SteamEmulator.SteamMasterServerUpdater.RemoveMasterServer(pServerAddress);
        }

        public int GetNumMasterServers(IntPtr _)
        {
            return SteamEmulator.SteamMasterServerUpdater.GetNumMasterServers();
        }

        public int GetMasterServerAddress(IntPtr _, int iServer, IntPtr pOut, int outBufferSize)
        {
            return 0;
        }
    }
}
