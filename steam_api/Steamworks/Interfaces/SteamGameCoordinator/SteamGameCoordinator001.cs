using SKYNET.Steamworks.Implementation;
using System;

namespace SKYNET.Steamworks.Interfaces
{
    [InterfaceLayout("SteamGameCoordinator001",
        "SendMessage", "IsMessageAvailable", "RetrieveMessage")]
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
