using System;

namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("STEAMAPPTICKET_INTERFACE_VERSION001",
        "GetAppOwnershipTicketData")]
    public class SteamAppTicket001 : ISteamInterface
    {
        public uint GetAppOwnershipTicketData(IntPtr _, uint nAppID, IntPtr pvBuffer, uint cbBufferLength, IntPtr piAppId, IntPtr piSteamId, IntPtr piSignature, IntPtr pcbSignature)
        {
            SteamEmulator.Write("SteamAppTicket", "GetAppOwnershipTicketData not implemented");
            return 0;
        }
    }
}
