using SKYNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Exported
{
    public class SteamAPI_ISteamMasterServerUpdater
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMasterServerUpdater_SetActive(IntPtr _, bool bActive)
        {
            Write("SteamAPI_ISteamMasterServerUpdater_SetActive");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMasterServerUpdater_SetHeartbeatInterval(IntPtr _, int iHeartbeatInterval)
        {
            Write("SteamAPI_ISteamMasterServerUpdater_SetHeartbeatInterval");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMasterServerUpdater_HandleIncomingPacket(IntPtr _, IntPtr pData, int cbData, uint srcIP, uint srcPort)
        {
            Write("SteamAPI_ISteamMasterServerUpdater_HandleIncomingPacket");
            return SteamEmulator.SteamMasterServerUpdater.HandleIncomingPacket(pData, cbData, srcIP, srcPort);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamMasterServerUpdater_GetNextOutgoingPacket(IntPtr _, IntPtr pOut, int cbMaxOut, uint pNetAdr, uint pPort)
        {
            Write("SteamAPI_ISteamMasterServerUpdater_GetNextOutgoingPacket");
            return SteamEmulator.SteamMasterServerUpdater.GetNextOutgoingPacket(pOut, cbMaxOut, pNetAdr, pPort);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMasterServerUpdater_SetBasicServerData(IntPtr _, uint nProtocolVersion, bool bDedicatedServer, string pRegionName, string pProductName, uint nMaxReportedClients, bool bPasswordProtected, string pGameDescription)
        {
            Write("SteamAPI_ISteamMasterServerUpdater_SetBasicServerData");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMasterServerUpdater_ClearAllKeyValues(IntPtr _)
        {
            Write("SteamAPI_ISteamMasterServerUpdater_ClearAllKeyValues");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMasterServerUpdater_SetKeyValue(IntPtr _, string pKey, string pValue)
        {
            Write("SteamAPI_ISteamMasterServerUpdater_SetKeyValue");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMasterServerUpdater_NotifyShutdown(IntPtr _)
        {
            Write("SteamAPI_ISteamMasterServerUpdater_NotifyShutdown");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMasterServerUpdater_WasRestartRequested(IntPtr _)
        {
            Write("SteamAPI_ISteamMasterServerUpdater_WasRestartRequested");
            return SteamEmulator.SteamMasterServerUpdater.WasRestartRequested();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void SteamAPI_ISteamMasterServerUpdater_ForceHeartbeat(IntPtr _)
        {
            Write("SteamAPI_ISteamMasterServerUpdater_ForceHeartbeat");
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMasterServerUpdater_AddMasterServer(IntPtr _, string pServerAddress)
        {
            Write("SteamAPI_ISteamMasterServerUpdater_AddMasterServer");
            return SteamEmulator.SteamMasterServerUpdater.AddMasterServer(pServerAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool SteamAPI_ISteamMasterServerUpdater_RemoveMasterServer(IntPtr _, string pServerAddress)
        {
            Write("SteamAPI_ISteamMasterServerUpdater_RemoveMasterServer");
            return SteamEmulator.SteamMasterServerUpdater.RemoveMasterServer(pServerAddress);
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamMasterServerUpdater_GetNumMasterServers(IntPtr _)
        {
            Write("SteamAPI_ISteamMasterServerUpdater_GetNumMasterServers");
            return SteamEmulator.SteamMasterServerUpdater.GetNumMasterServers();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static int SteamAPI_ISteamMasterServerUpdater_GetMasterServerAddress(IntPtr _, int iServer, string pOut, int outBufferSize)
        {
            Write("SteamAPI_ISteamMasterServerUpdater_GetMasterServerAddress");
            return SteamEmulator.SteamMasterServerUpdater.GetMasterServerAddress(iServer, pOut, outBufferSize);
        }

        private static void Write(string msg)
        {
            SteamEmulator.Write("", msg);
        }
    }
}

