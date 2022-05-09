using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Interfaces
{
    [Interface("SteamMatchGameSearch001")]
    public class SteamMatchGameSearch001 : ISteamInterface
    {
        public int AddGameSearchParams(IntPtr _, string pchKeyToFind, string pchValuesToFind)
        {
            return SteamEmulator.SteamGameSearch.AddGameSearchParams(pchKeyToFind, pchValuesToFind);
        }

        public int SearchForGameWithLobby(IntPtr _, ulong steamIDLobby, int nPlayerMin, int nPlayerMax)
        {
            return SteamEmulator.SteamGameSearch.SearchForGameWithLobby(steamIDLobby, nPlayerMin, nPlayerMax);
        }

        public int SearchForGameSolo(IntPtr _, int nPlayerMin, int nPlayerMax)
        {
            return SteamEmulator.SteamGameSearch.SearchForGameSolo(nPlayerMin, nPlayerMax);
        }

        public int AcceptGame(IntPtr _)
        {
            return SteamEmulator.SteamGameSearch.AcceptGame();
        }

        public int DeclineGame(IntPtr _)
        {
            return SteamEmulator.SteamGameSearch.DeclineGame();
        }

        public int RetrieveConnectionDetails(IntPtr _, ulong steamIDHost, IntPtr pchConnectionDetails, int cubConnectionDetails)
        {
            return SteamEmulator.SteamGameSearch.RetrieveConnectionDetails(steamIDHost, pchConnectionDetails, cubConnectionDetails);
        }

        public int EndGameSearch(IntPtr _)
        {
            return SteamEmulator.SteamGameSearch.EndGameSearch();
        }

        public int SetGameHostParams(IntPtr _, string pchKey, string pchValue)
        {
            return SteamEmulator.SteamGameSearch.SetGameHostParams(pchKey, pchValue);
        }

        public int SetConnectionDetails(IntPtr _, string pchConnectionDetails, int cubConnectionDetails)
        {
            return SteamEmulator.SteamGameSearch.SetConnectionDetails(pchConnectionDetails, cubConnectionDetails);
        }

        public int RequestPlayersForGame(IntPtr _, int nPlayerMin, int nPlayerMax, int nMaxTeamSize)
        {
            return SteamEmulator.SteamGameSearch.RequestPlayersForGame(nPlayerMin, nPlayerMax, nMaxTeamSize);
        }

        public int HostConfirmGameStart(IntPtr _, ulong ullUniqueGameID)
        {
            return SteamEmulator.SteamGameSearch.HostConfirmGameStart(ullUniqueGameID);
        }

        public int CancelRequestPlayersForGame(IntPtr _)
        {
            return SteamEmulator.SteamGameSearch.CancelRequestPlayersForGame();
        }

        public int SubmitPlayerResult(IntPtr _, ulong ullUniqueGameID, ulong steamIDPlayer, int EPlayerResult)
        {
            return SteamEmulator.SteamGameSearch.SubmitPlayerResult(ullUniqueGameID, steamIDPlayer, EPlayerResult);
        }

        public int EndGame(IntPtr _, ulong ullUniqueGameID)
        {
            return SteamEmulator.SteamGameSearch.EndGame(ullUniqueGameID);
        }

    }
}

