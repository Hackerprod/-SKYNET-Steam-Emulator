using SKYNET.Steamworks.Implementation;
using System;
using System.Runtime.InteropServices;
using uint32 = System.UInt32;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamGameCoordinator001")]
    public class SteamGameCoordinator001 : ISteamInterface
    {
        public int SendMessage(IntPtr _, uint unMsgType, IntPtr pubData, uint cubData)
        {
            return (int)SteamEmulator.SteamGameCoordinator.SendMessage(_, unMsgType, pubData, cubData);
        }

        public bool IsMessageAvailable(IntPtr _, ref uint pcubMsgSize)
        {
            return SteamEmulator.SteamGameCoordinator.IsMessageAvailable(_, ref pcubMsgSize);
        }

        public int RetrieveMessage(IntPtr _, ref uint punMsgType, IntPtr pubDest, uint cubDest, ref uint pcubMsgSize)
        {
            return (int)SteamEmulator.SteamGameCoordinator.RetrieveMessage(_, ref punMsgType, pubDest, cubDest, ref pcubMsgSize);
        }
    }
}
