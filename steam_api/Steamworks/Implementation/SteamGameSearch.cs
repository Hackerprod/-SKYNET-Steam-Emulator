using System;
using SKYNET.Interface;

namespace SKYNET.Managers
{

    [Map("SteamGameSearch")]
    [Map("SteamMatchGameSearch")]
    public class SteamGameSearch : IBaseInterface, ISteamGameSearch
    {
        public GameSearchErrorCode_t AcceptGame(IntPtr self)
        {
            return GameSearchErrorCode_t.OK;
        }

        public GameSearchErrorCode_t AddGameSearchParams(string pchKeyToFind, string pchValuesToFind)
        {
            return GameSearchErrorCode_t.OK;
        }

        public GameSearchErrorCode_t CancelRequestPlayersForGame(IntPtr self)
        {
            return GameSearchErrorCode_t.OK;
        }

        public GameSearchErrorCode_t DeclineGame(IntPtr self)
        {
            return GameSearchErrorCode_t.OK;
        }

        public GameSearchErrorCode_t EndGame(ulong ullUniqueGameID)
        {
            return GameSearchErrorCode_t.OK;
        }

        public GameSearchErrorCode_t EndGameSearch(IntPtr self)
        {
            return GameSearchErrorCode_t.OK;
        }

        public GameSearchErrorCode_t HostConfirmGameStart(ulong ullUniqueGameID)
        {
            return GameSearchErrorCode_t.OK;
        }

        public GameSearchErrorCode_t RequestPlayersForGame(int nPlayerMin, int nPlayerMax, int nMaxTeamSize)
        {
            return GameSearchErrorCode_t.OK;
        }

        public GameSearchErrorCode_t RetrieveConnectionDetails(IntPtr steamIDHost, IntPtr pchConnectionDetails, int cubConnectionDetails)
        {
            return GameSearchErrorCode_t.OK;
        }

        public GameSearchErrorCode_t SearchForGameSolo(int nPlayerMin, int nPlayerMax)
        {
            return GameSearchErrorCode_t.OK;
        }

        public GameSearchErrorCode_t SearchForGameWithLobby(IntPtr steamIDLobby, int nPlayerMin, int nPlayerMax)
        {
            return GameSearchErrorCode_t.OK;
        }

        public GameSearchErrorCode_t SetConnectionDetails(string pchConnectionDetails, int cubConnectionDetails)
        {
            return GameSearchErrorCode_t.OK;
        }

        public GameSearchErrorCode_t SetGameHostParams(string pchKey, string pchValue)
        {
            return GameSearchErrorCode_t.OK;
        }

        public GameSearchErrorCode_t SubmitPlayerResult(ulong ullUniqueGameID, IntPtr steamIDPlayer, PlayerResult_t EPlayerResult)
        {
            return GameSearchErrorCode_t.OK;
        }
    }
}