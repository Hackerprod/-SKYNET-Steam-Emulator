using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Steamworks;
using SKYNET.Types;
using Steamworks;
using SKYNET.Helper;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamGameCoordinator : ISteamInterface
    {
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
            uint msgType = GetGCMsg(punMsgType);

            Write($"RetrieveMessage [{msgType}]");
            return EGCResults.k_EGCResultOK;
        }

        public EGCResults SendMessage_(uint unMsgType, IntPtr pubData, uint cubData)
        {
            uint msgType = GetGCMsg(unMsgType);
            byte[] bytes = pubData.GetBytes(cubData);

            Write($"SendMessage [{msgType}], {bytes.Length} bytes");

            return EGCResults.k_EGCResultOK;
        }

        private uint GetGCMsg(uint msg)
        {
            return msg & 0x7FFFFFFFu;
        }
    }
}
