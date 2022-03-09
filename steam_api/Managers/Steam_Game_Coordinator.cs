using System;
using System.Runtime.InteropServices;
using SKYNET.Interface;
using SKYNET.Types;

namespace SKYNET.Managers
{
    public class Steam_GameCoordinator : SteamInterface, ISteamGameCoordinator
    {

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
    }
}