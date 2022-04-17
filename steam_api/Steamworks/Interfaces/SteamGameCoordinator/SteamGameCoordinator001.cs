using SKYNET.Interface;
using SKYNET.Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    [Interface("SteamGameCoordinator001")]
    public class SteamGameCoordinator001 : ISteamInterface
    {
        public EGCResults SendMessage_(IntPtr _, uint unMsgType, IntPtr pubData, uint cubData)
        {
            return SteamEmulator.SteamGameCoordinator.SendMessage_(unMsgType, pubData, cubData);
        }

        public bool IsMessageAvailable(IntPtr _, uint pcubMsgSize)
        {
            return SteamEmulator.SteamGameCoordinator.IsMessageAvailable(pcubMsgSize);
        }

        public EGCResults RetrieveMessage(IntPtr _, uint punMsgType, IntPtr pubDest, uint cubDest, uint pcubMsgSize)
        {
            return SteamEmulator.SteamGameCoordinator.RetrieveMessage(punMsgType, pubDest, cubDest, pcubMsgSize);
        }


    }
}
