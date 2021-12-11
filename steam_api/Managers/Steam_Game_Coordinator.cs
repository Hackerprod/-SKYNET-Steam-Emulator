using System;
using SKYNET.Interface;
using SKYNET.Types;

namespace SKYNET.Managers
{
    public class Steam_GameCoordinator : ISteamGameCoordinator
    {
        public bool IsMessageAvailable(uint pcubMsgSize)
        {
            return false;
        }

        public EGCResults RetrieveMessage(uint punMsgType, IntPtr pubDest, uint cubDest, uint pcubMsgSize)
        {
            return EGCResults.k_EGCResultNoMessage;
        }

        public EGCResults SendMessage_(uint unMsgType, IntPtr pubData, uint cubData)
        {
            return EGCResults.k_EGCResultOK;
        }
    }
}