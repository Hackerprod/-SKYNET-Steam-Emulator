using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Interface
{
    public interface ISteamGameSearch
    {

        GameSearchErrorCode_t AddGameSearchParams(string pchKeyToFind,  string pchValuesToFind);

        GameSearchErrorCode_t SearchForGameWithLobby(IntPtr steamIDLobby, int nPlayerMin, int nPlayerMax);

        GameSearchErrorCode_t SearchForGameSolo(int nPlayerMin, int nPlayerMax);

        GameSearchErrorCode_t AcceptGame(IntPtr self);

        GameSearchErrorCode_t DeclineGame(IntPtr self);

        GameSearchErrorCode_t RetrieveConnectionDetails(IntPtr steamIDHost, IntPtr pchConnectionDetails, int cubConnectionDetails);

        GameSearchErrorCode_t EndGameSearch(IntPtr self);

        GameSearchErrorCode_t SetGameHostParams(string pchKey, string pchValue);

        GameSearchErrorCode_t SetConnectionDetails( string pchConnectionDetails, int cubConnectionDetails);

        GameSearchErrorCode_t RequestPlayersForGame(int nPlayerMin, int nPlayerMax, int nMaxTeamSize);

        GameSearchErrorCode_t HostConfirmGameStart(ulong ullUniqueGameID);

        GameSearchErrorCode_t CancelRequestPlayersForGame(IntPtr self);

        GameSearchErrorCode_t SubmitPlayerResult(ulong ullUniqueGameID, IntPtr steamIDPlayer, PlayerResult_t EPlayerResult);

        GameSearchErrorCode_t EndGame(ulong ullUniqueGameID);
    }
    public enum GameSearchErrorCode_t : int
    {
        OK = 1,
        Failed_Search_Already_In_Progress = 2,
        Failed_No_Search_In_Progress = 3,
        Failed_Not_Lobby_Leader = 4,
        Failed_No_Host_Available = 5,
        Failed_Search_Params_Invalid = 6,
        Failed_Offline = 7,
        Failed_NotAuthorized = 8,
        Failed_Unknown_Error = 9,
    }
    public enum PlayerResult_t : int
    {
        FailedToConnect = 1,
        Abandoned = 2,
        Kicked = 3,
        Incomplete = 4,
        Completed = 5,
    }
}
