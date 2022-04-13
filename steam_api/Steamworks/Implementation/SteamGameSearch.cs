using System;
using System.Runtime.InteropServices;
using SKYNET;
using SKYNET.Steamworks;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamGameSearch : ISteamInterface
    {
        public SteamGameSearch()
        {
            InterfaceVersion = "SteamGameSearch";
        }

        public int AcceptGame(IntPtr self)
        {
            Write("AcceptGame");
            return (int)GameSearchErrorCode_t.OK;
        }

        public int AddGameSearchParams(string pchKeyToFind, string pchValuesToFind)
        {
            Write("AddGameSearchParams");
            return (int)GameSearchErrorCode_t.OK;
        }

        public int CancelRequestPlayersForGame(IntPtr self)
        {
            Write("CancelRequestPlayersForGame");
            return (int)GameSearchErrorCode_t.OK;
        }

        public int DeclineGame(IntPtr self)
        {
            Write("DeclineGame");
            return (int)GameSearchErrorCode_t.OK;
        }

        public int EndGame(ulong ullUniqueGameID)
        {
            Write("EndGame");
            return (int)GameSearchErrorCode_t.OK;
        }

        public int EndGameSearch(IntPtr self)
        {
            Write("EndGameSearch");
            return (int)GameSearchErrorCode_t.OK;
        }

        public int HostConfirmGameStart(ulong ullUniqueGameID)
        {
            Write("HostConfirmGameStart");
            return (int)GameSearchErrorCode_t.OK;
        }

        public int RequestPlayersForGame(int nPlayerMin, int nPlayerMax, int nMaxTeamSize)
        {
            Write("RequestPlayersForGame");
            return (int)GameSearchErrorCode_t.OK;
        }

        public int RetrieveConnectionDetails(ulong steamIDHost, IntPtr pchConnectionDetails, int cubConnectionDetails)
        {
            Write("RetrieveConnectionDetails");
            return (int)GameSearchErrorCode_t.OK;
        }

        public int SearchForGameSolo(int nPlayerMin, int nPlayerMax)
        {
            Write("SearchForGameSolo");
            return (int)GameSearchErrorCode_t.OK;
        }

        public int SearchForGameWithLobby(ulong steamIDLobby, int nPlayerMin, int nPlayerMax)
        {
            Write("SearchForGameWithLobby");
            return (int)GameSearchErrorCode_t.OK;
        }

        public int SetConnectionDetails(string pchConnectionDetails, int cubConnectionDetails)
        {
            Write("SetConnectionDetails");
            return (int)GameSearchErrorCode_t.OK;
        }

        public int SetGameHostParams(string pchKey, string pchValue)
        {
            Write("SetGameHostParams");
            return (int)GameSearchErrorCode_t.OK;
        }

        public int SubmitPlayerResult(ulong ullUniqueGameID, ulong steamIDPlayer, int EPlayerResult)
        {
            Write("SubmitPlayerResult");
            return (int)GameSearchErrorCode_t.OK;
        }
    }
}