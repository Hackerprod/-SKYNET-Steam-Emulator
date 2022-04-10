using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SKYNET.Steamworks.Implementation
{
    [StructLayout(LayoutKind.Sequential)]
    public class SteamRemotePlay : ISteamInterface
    {
        public uint GetSessionCount(IntPtr _)
        {
            Write("GetSessionCount");
            return 0;
        }

        public uint GetSessionID(int iSessionIndex)
        {
            Write("GetSessionID");
            return default;
        }

        public IntPtr GetSessionSteamID(uint unSessionID)
        {
            Write("GetSessionSteamID");
            return default;
        }

        public string GetSessionClientName(uint unSessionID)
        {
            Write("GetSessionClientName");
            return default;
        }

        public ESteamDeviceFormFactor GetSessionClientFormFactor(uint unSessionID)
        {
            Write("GetSessionClientFormFactor");
            return default;
        }

        public bool BGetSessionClientResolution(uint unSessionID, int pnResolutionX, int pnResolutionY)
        {
            Write("BGetSessionClientResolution");
            return default;
        }

        public bool BSendRemotePlayTogetherInvite(IntPtr steamIDFriend)
        {
            Write("BSendRemotePlayTogetherInvite");
            return default;
        }

        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamRemotePlay()
        {
            InterfaceVersion = "SteamRemotePlay";
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}