using Core.Interface;

using System;

//[Map("SteamMasterServerUpdater")]
public class SteamMasterServerUpdater : IBaseInterface
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

    public void ClearAllKeyValues(IntPtr _)
    {
        //
    }

    public void SetKeyValue(string pKey, string pValue)
    {
        //
    }

    public void NotifyShutdown(IntPtr _)
    {
        //
    }

    public bool WasRestartRequested(IntPtr _)
    {
        return false;
    }

    public void ForceHeartbeat(IntPtr _)
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

    public int GetNumMasterServers(IntPtr _)
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

