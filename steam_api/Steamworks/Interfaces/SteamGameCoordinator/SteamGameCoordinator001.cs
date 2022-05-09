using System;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamGameCoordinator001")]
    public class SteamGameCoordinator001 : ISteamInterface
    {
        public int SendMessage(IntPtr _, uint unMsgType, IntPtr pubData, uint cubData)
        {
            return (int)SteamEmulator.SteamGameCoordinator.SendMessage(unMsgType, pubData, cubData);
        }

        public bool IsMessageAvailable(IntPtr _, ref int pcubMsgSize)
        {
            return SteamEmulator.SteamGameCoordinator.IsMessageAvailable(ref pcubMsgSize);
        }

        public int RetrieveMessage(IntPtr _, ref uint punMsgType, IntPtr pubDest, uint cubDest, ref uint pcubMsgSize)
        {
            return (int)SteamEmulator.SteamGameCoordinator.RetrieveMessage(ref punMsgType, pubDest, cubDest, ref pcubMsgSize);
        }
    }
}
