using Core.Interface;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Delegate
{
    [Delegate(Name = "SteamMasterServerUpdater")]
    public class DSteamMasterServerUpdater
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetActive(bool bActive);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetHeartbeatInterval(int iHeartbeatInterval);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool HandleIncomingPacket(IntPtr pData, int cbData, uint srcIP, uint srcPort);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetNextOutgoingPacket(IntPtr pOut, int cbMaxOut, uint pNetAdr, uint pPort);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetBasicServerData(uint nProtocolVersion, bool bDedicatedServer, string pRegionName, string pProductName, uint nMaxReportedClients, bool bPasswordProtected, string pGameDescription);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ClearAllKeyValues();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void SetKeyValue(string pKey, string pValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void NotifyShutdown();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool WasRestartRequested();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void ForceHeartbeat();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool AddMasterServer(string pServerAddress);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate bool RemoveMasterServer(string pServerAddress);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetNumMasterServers();

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int GetMasterServerAddress(int iServer, string pOut, int outBufferSize);
    }
}