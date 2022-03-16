using SKYNET.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Delegate
{
    [Delegate("SteamGameCoordinator")]
    public class DSteamGameSearch
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate GameSearchErrorCode_t AddGameSearchParams(string pchKeyToFind,  string pchValuesToFind);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate GameSearchErrorCode_t SearchForGameWithLobby(IntPtr steamIDLobby, int nPlayerMin, int nPlayerMax);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate GameSearchErrorCode_t SearchForGameSolo(int nPlayerMin, int nPlayerMax);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate GameSearchErrorCode_t AcceptGame(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate GameSearchErrorCode_t DeclineGame(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate GameSearchErrorCode_t RetrieveConnectionDetails(IntPtr steamIDHost, IntPtr pchConnectionDetails, int cubConnectionDetails);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate GameSearchErrorCode_t EndGameSearch(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate GameSearchErrorCode_t SetGameHostParams(string pchKey, string pchValue);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate GameSearchErrorCode_t SetConnectionDetails( string pchConnectionDetails, int cubConnectionDetails);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate GameSearchErrorCode_t RequestPlayersForGame(int nPlayerMin, int nPlayerMax, int nMaxTeamSize);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate GameSearchErrorCode_t HostConfirmGameStart(ulong ullUniqueGameID);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate GameSearchErrorCode_t CancelRequestPlayersForGame(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate GameSearchErrorCode_t SubmitPlayerResult(ulong ullUniqueGameID, IntPtr steamIDPlayer, PlayerResult_t EPlayerResult);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate GameSearchErrorCode_t EndGame(ulong ullUniqueGameID);
    }

}
