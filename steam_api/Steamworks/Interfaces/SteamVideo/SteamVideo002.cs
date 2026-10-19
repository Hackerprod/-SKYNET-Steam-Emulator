using System;
using System.Collections.Generic;

namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("STEAMVIDEO_INTERFACE_V001",
        "GetVideoURL", "IsBroadcasting")]
    [InterfaceLayout("STEAMVIDEO_INTERFACE_V002",
        "GetVideoURL", "IsBroadcasting", "GetOPFSettings", "GetOPFStringForApp")]
    [InterfaceLayout("STEAMVIDEO_INTERFACE_V003",
        "GetVideoURL", "IsBroadcasting", "GetOPFSettings", "GetOPFStringForApp", "_unknown_fn_4", "_unknown_fn_5", "_unknown_fn_6", "_unknown_fn_7", "_unknown_fn_8")]
    [InterfaceLayout("STEAMVIDEO_INTERFACE_V004",
        "GetVideoURL", "IsBroadcasting", "GetOPFSettings", "GetOPFStringForApp", "_unknown_fn_4", "_unknown_fn_5", "_unknown_fn_6", "_unknown_fn_7")]
    [InterfaceLayout("STEAMVIDEO_INTERFACE_V005",
        "GetVideoURL", "IsBroadcasting", "GetOPFSettings", "GetOPFStringForApp", "_unknown_fn_4", "_unknown_fn_5", "_unknown_fn_6", "_unknown_fn_7")]
    [InterfaceLayout("STEAMVIDEO_INTERFACE_V006",
        "GetVideoURL", "IsBroadcasting", "GetOPFSettings", "GetOPFStringForApp", "_unknown_fn_4", "_unknown_fn_5", "_unknown_fn_6", "_unknown_fn_7")]
    [InterfaceLayout("STEAMVIDEO_INTERFACE_V007",
        "GetVideoURL", "IsBroadcasting", "GetOPFSettings", "GetOPFStringForApp")]
    public class SteamVideo002 : ISteamInterface
    {
        // Slots present only in the real steamclient64 CAdapterSteamVideo003..006 vtables (each is a bare "ret 0");
        // no matching method exists in any SDK header, so they stay logged no-ops.
        private static readonly HashSet<string> _stubsLogged = new HashSet<string>();

        private static int LogStub(string method)
        {
            lock (_stubsLogged)
            {
                if (_stubsLogged.Add(method))
                {
                    SteamEmulator.Write("SteamVideo", method + " not implemented");
                }
            }

            return 0;
        }

        public int _unknown_fn_4(IntPtr _) => LogStub("_unknown_fn_4");

        public int _unknown_fn_5(IntPtr _) => LogStub("_unknown_fn_5");

        public int _unknown_fn_6(IntPtr _) => LogStub("_unknown_fn_6");

        public int _unknown_fn_7(IntPtr _) => LogStub("_unknown_fn_7");

        public int _unknown_fn_8(IntPtr _) => LogStub("_unknown_fn_8");

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
