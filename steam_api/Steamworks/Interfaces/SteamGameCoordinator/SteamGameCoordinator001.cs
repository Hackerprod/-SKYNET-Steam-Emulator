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
        public int SendMessage(IntPtr _, uint unMsgType, IntPtr pubData, uint cubData)
        {
            return (int)SteamEmulator.SteamGameCoordinator.SendMessage(unMsgType, pubData, cubData);
        }

        public bool IsMessageAvailable(IntPtr _, ref uint pcubMsgSize)
        {
            return SteamEmulator.SteamGameCoordinator.IsMessageAvailable(ref pcubMsgSize);
        }

        public int RetrieveMessage(IntPtr _, ref uint punMsgType, IntPtr pubDest, uint cubDest, ref uint pcubMsgSize)
        {
            return (int)SteamEmulator.SteamGameCoordinator.RetrieveMessage(ref punMsgType, pubDest, cubDest, ref pcubMsgSize);
        }


    }
}
