using System;

namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("SteamNetworkingFakeUDPPort001",
        "DestroyFakeUDPPort", "SendMessageToFakeIP", "ReceiveMessages", "ScheduleCleanup")]
    public class SteamNetworkingFakeUDPPort001 : ISteamInterface
    {
        public void DestroyFakeUDPPort(IntPtr _)
        {
            SteamEmulator.Write("SteamNetworkingFakeUDPPort", "DestroyFakeUDPPort");
        }

        public int SendMessageToFakeIP(IntPtr _, IntPtr remoteAddress, IntPtr pData, uint cbData, int nSendFlags)
        {
            SteamEmulator.Write("SteamNetworkingFakeUDPPort", "SendMessageToFakeIP");
            return (int)EResult.k_EResultFail;
        }

        public int ReceiveMessages(IntPtr _, IntPtr ppOutMessages, int nMaxMessages)
        {
            SteamEmulator.Write("SteamNetworkingFakeUDPPort", "ReceiveMessages");
            return 0;
        }

        public void ScheduleCleanup(IntPtr _, IntPtr remoteAddress)
        {
            SteamEmulator.Write("SteamNetworkingFakeUDPPort", "ScheduleCleanup");
        }
    }
}
