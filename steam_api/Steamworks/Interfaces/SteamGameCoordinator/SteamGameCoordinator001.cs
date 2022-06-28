using System;
using System.Runtime.InteropServices;
using uint32 = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamGameCoordinator001")]
    public class SteamGameCoordinator001 : ISteamInterface
    {
        public EGCResults SendMessage(uint32 unMsgType, IntPtr pubData, uint32 cubData)
        {
            return SteamEmulator.SteamGameCoordinator.SendMessage(unMsgType, pubData, cubData);
        }

        public unsafe bool IsMessageAvailable(uint* pcubMsgSize)
        {
            return SteamEmulator.SteamGameCoordinator.IsMessageAvailable(pcubMsgSize);
        }

        public unsafe int RetrieveMessage(uint* punMsgType, IntPtr pubDest, uint32 cubDest, uint* pcubMsgSize)
        {
            return SteamEmulator.SteamGameCoordinator.RetrieveMessage(punMsgType, pubDest, cubDest, pcubMsgSize);
        }
    }
}
