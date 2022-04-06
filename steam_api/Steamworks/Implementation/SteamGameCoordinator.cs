using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using SKYNET.Types;
using Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamGameCoordinator : ISteamInterface
    {
        public IntPtr MemoryAddress { get; set; }
        public string InterfaceVersion { get; set; }

        public SteamGameCoordinator()
        {
            InterfaceVersion = "SteamGameCoordinator";
        }

        public bool IsMessageAvailable(uint pcubMsgSize)
        {
            Write("IsMessageAvailable");
            return false;
        }

        public EGCResults RetrieveMessage(uint punMsgType, IntPtr pubDest, uint cubDest, uint pcubMsgSize)
        {
            Write("RetrieveMessage");
            return EGCResults.k_EGCResultNoMessage;
        }

        public EGCResults SendMessage_(uint unMsgType, IntPtr pubData, uint cubData)
        {
            Write("SendMessage_");
            return EGCResults.k_EGCResultOK;
        }

        private void Write(string v)
        {
            SteamEmulator.Write(InterfaceVersion, v);
        }
    }
}
