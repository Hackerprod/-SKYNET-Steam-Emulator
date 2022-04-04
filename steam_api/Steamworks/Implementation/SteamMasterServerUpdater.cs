using SKYNET;
using SKYNET.Helper;
using System;

public class SteamMasterServerUpdater : ISteamInterface
{
    public IntPtr MemoryAddress { get; set; }
    public string InterfaceVersion { get; set; }


    public void SetActive(bool bActive)
    {
        Write($"SetActive");
    }

    public void SetHeartbeatInterval(int iHeartbeatInterval)
    {
        Write($"SetHeartbeatInterval");
    }

    public bool HandleIncomingPacket(IntPtr pData, int cbData, uint srcIP, uint srcPort)
    {
        Write($"HandleIncomingPacket");
        return false;
    }

    public int GetNextOutgoingPacket(IntPtr pOut, int cbMaxOut, uint pNetAdr, uint pPort)
    {
        Write($"GetNextOutgoingPacket");
        return 0;
    }

    public void ClearAllKeyValues(IntPtr _)
    {
        Write($"ClearAllKeyValues");
    }

    public void SetKeyValue(string pKey, string pValue)
    {
        Write($"SetKeyValue");
    }

    public void NotifyShutdown(IntPtr _)
    {
        Write($"NotifyShutdown");
    }

    public bool WasRestartRequested(IntPtr _)
    {
        Write($"WasRestartRequested");
        return false;
    }

    public void ForceHeartbeat(IntPtr _)
    {
        Write($"ForceHeartbeat");
    }

    public bool AddMasterServer(string pServerAddress)
    {
        Write($"AddMasterServer");
        return false;
    }

    public bool RemoveMasterServer(string pServerAddress)
    {
        Write($"RemoveMasterServer");
        return false;
    }

    public int GetNumMasterServers(IntPtr _)
    {
        Write($"GetNumMasterServers");
        return 0;
    }

    public int GetMasterServerAddress(int iServer, string pOut, int outBufferSize)
    {
        Write($"GetMasterServerAddress");
        return 0;
    }

    public void SetBasicServerData(uint nProtocolVersion, bool bDedicatedServer, string pRegionName, string pProductName, uint nMaxReportedClients, bool bPasswordProtected, string pGameDescription)
    {
        Write($"SetBasicServerData");
    }

    private void Write(string v)
    {
        Log.Write(InterfaceVersion, v);
    }
}

