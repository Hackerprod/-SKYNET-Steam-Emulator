using System;

namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("STEAMVIDEO_INTERFACE_V001",
        "GetVideoURL", "IsBroadcasting")]
    [InterfaceLayout("STEAMVIDEO_INTERFACE_V002",
        "GetVideoURL", "IsBroadcasting", "GetOPFSettings", "GetOPFStringForApp")]
    [InterfaceLayout("STEAMVIDEO_INTERFACE_V003",
        "GetVideoURL", "IsBroadcasting", "GetOPFSettings", "GetOPFStringForApp")]
    [InterfaceLayout("STEAMVIDEO_INTERFACE_V004",
        "GetVideoURL", "IsBroadcasting", "GetOPFSettings", "GetOPFStringForApp")]
    [InterfaceLayout("STEAMVIDEO_INTERFACE_V005",
        "GetVideoURL", "IsBroadcasting", "GetOPFSettings", "GetOPFStringForApp")]
    [InterfaceLayout("STEAMVIDEO_INTERFACE_V006",
        "GetVideoURL", "IsBroadcasting", "GetOPFSettings", "GetOPFStringForApp")]
    [InterfaceLayout("STEAMVIDEO_INTERFACE_V007",
        "GetVideoURL", "IsBroadcasting", "GetOPFSettings", "GetOPFStringForApp")]
    public class SteamVideo002 : ISteamInterface
    {
        public void GetVideoURL(IntPtr _, uint unVideoAppID)
        {
            SteamEmulator.SteamVideo.GetVideoURL(unVideoAppID);
        }

        public bool IsBroadcasting(IntPtr _, IntPtr pnNumViewers)
        {
            return SteamEmulator.SteamVideo.IsBroadcasting(pnNumViewers);
        }

        public void GetOPFSettings(IntPtr _, uint unVideoAppID)
        {
            SteamEmulator.SteamVideo.GetOPFSettings(unVideoAppID);
        }

        public bool GetOPFStringForApp(IntPtr _, uint unVideoAppID, IntPtr pchBuffer, IntPtr pnBufferSize)
        {
            return SteamEmulator.SteamVideo.GetOPFStringForApp(unVideoAppID, pchBuffer, pnBufferSize);
        }

    }
}
