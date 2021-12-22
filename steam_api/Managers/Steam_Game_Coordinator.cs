using System;
using System.Runtime.InteropServices;
using SKYNET.Interface;
using SKYNET.Types;

namespace SKYNET.Managers
{
    public class Steam_GameCoordinator : SteamInterface//, ISteamGameCoordinator
    {

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static bool IsMessageAvailable(uint pcubMsgSize)
        {
            return false;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EGCResults RetrieveMessage(uint punMsgType, IntPtr pubDest, uint cubDest, uint pcubMsgSize)
        {
            return EGCResults.k_EGCResultNoMessage;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static EGCResults SendMessage_(uint unMsgType, IntPtr pubData, uint cubData)
        {
            return EGCResults.k_EGCResultOK;
        }
    }
}