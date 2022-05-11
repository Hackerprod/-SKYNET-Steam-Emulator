using System;
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

        public bool IsMessageAvailable(ref uint32 pcubMsgSize)
        {
            return SteamEmulator.SteamGameCoordinator.IsMessageAvailable(ref pcubMsgSize);
        }

        public EGCResults RetrieveMessage(uint32 punMsgType, IntPtr pubDest, uint32 cubDest, ref uint32 pcubMsgSize)
        {
            return SteamEmulator.SteamGameCoordinator.RetrieveMessage(ref punMsgType, pubDest, cubDest, ref pcubMsgSize);
        }
    }
}
