using SKYNET.Steamworks.Interfaces;
using System;


namespace SKYNET.Steamworks.Implementation
{
    public class SteamGameSearch : ISteamInterface
    {
        public static SteamGameSearch Instance;

        public SteamGameSearch()
        {
            Instance = this;
            InterfaceName = "SteamGameSearch";
            InterfaceVersion = "SteamMatchGameSearch001";
        }

        public int AcceptGame()
        {
            Write("AcceptGame");
            return (int)GameSearchErrorCode_t.Failed_Offline;
        }

        public int AddGameSearchParams(string pchKeyToFind, string pchValuesToFind)
        {
            Write("AddGameSearchParams");
            return (int)GameSearchErrorCode_t.Failed_Offline;
        }

        public int CancelRequestPlayersForGame()
        {
            Write("CancelRequestPlayersForGame");
            return (int)GameSearchErrorCode_t.Failed_Offline;
        }

        public int DeclineGame()
        {
            Write("DeclineGame");
            return (int)GameSearchErrorCode_t.Failed_Offline;
        }

        public int EndGame(ulong ullUniqueGameID)
        {
            Write("EndGame");
            return (int)GameSearchErrorCode_t.Failed_Offline;
        }

        public int EndGameSearch()
        {
            Write("EndGameSearch");
            return (int)GameSearchErrorCode_t.Failed_Offline;
        }

        public int HostConfirmGameStart(ulong ullUniqueGameID)
        {
            Write("HostConfirmGameStart");
            return (int)GameSearchErrorCode_t.Failed_Offline;
        }

        public int RequestPlayersForGame(int nPlayerMin, int nPlayerMax, int nMaxTeamSize)
        {
            Write("RequestPlayersForGame");
            return (int)GameSearchErrorCode_t.Failed_Offline;
        }

        public int RetrieveConnectionDetails(ulong steamIDHost, IntPtr pchConnectionDetails, int cubConnectionDetails)
        {
            Write("RetrieveConnectionDetails");
            return (int)GameSearchErrorCode_t.Failed_Offline;
        }

        public int SearchForGameSolo(int nPlayerMin, int nPlayerMax)
        {
            Write("SearchForGameSolo");
            return (int)GameSearchErrorCode_t.Failed_Offline;
        }

        public int SearchForGameWithLobby(ulong steamIDLobby, int nPlayerMin, int nPlayerMax)
        {
            Write("SearchForGameWithLobby");
            return (int)GameSearchErrorCode_t.Failed_Offline;
        }

        public int SetConnectionDetails(string pchConnectionDetails, int cubConnectionDetails)
        {
            Write("SetConnectionDetails");
            return (int)GameSearchErrorCode_t.Failed_Offline;
        }

        public int SetGameHostParams(string pchKey, string pchValue)
        {
            Write("SetGameHostParams");
            return (int)GameSearchErrorCode_t.Failed_Offline;
        }

        public int SubmitPlayerResult(ulong ullUniqueGameID, ulong steamIDPlayer, int EPlayerResult)
        {
            Write("SubmitPlayerResult");
            return (int)GameSearchErrorCode_t.Failed_Offline;
        }
    }
}