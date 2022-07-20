using SKYNET.Steamworks.Implementation;
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
            return SteamGameCoordinator.Instance.SendMessage(unMsgType, pubData, cubData);
        }

        public unsafe bool IsMessageAvailable(uint pcubMsgSize)
        {
            return SteamGameCoordinator.Instance.IsMessageAvailable(pcubMsgSize);
        }

        public unsafe int RetrieveMessage(ref uint punMsgType, IntPtr pubDest, uint32 cubDest, ref uint pcubMsgSize)
        {
            return SteamGameCoordinator.Instance.RetrieveMessage(ref punMsgType, pubDest, cubDest, ref pcubMsgSize);
        }
    }
}
